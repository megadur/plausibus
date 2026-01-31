using ErezeptValidator.Models.Validation;
using Hl7.Fhir.Model;

namespace ErezeptValidator.Services.Validation;

/// <summary>
/// Validation pipeline that executes validators in sequence
/// </summary>
public class ValidationPipeline
{
    private readonly IEnumerable<IValidator> _validators;
    private readonly ILogger<ValidationPipeline> _logger;

    public ValidationPipeline(IEnumerable<IValidator> validators, ILogger<ValidationPipeline> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    /// <summary>
    /// Execute all validators against the FHIR bundle
    /// </summary>
    public async Task<List<ValidationResult>> ValidateAsync(Bundle bundle)
    {
        _logger.LogInformation("Starting validation pipeline for bundle {BundleId}", bundle.Id);

        // Build validation context
        var context = BuildContext(bundle);

        // Execute validators
        var results = new List<ValidationResult>();
        foreach (var validator in _validators)
        {
            try
            {
                _logger.LogDebug("Executing validator: {ValidatorName}", validator.Name);
                var result = await validator.ValidateAsync(context);
                results.Add(result);

                _logger.LogDebug("Validator {ValidatorName} completed: {ErrorCount} errors, {WarningCount} warnings",
                    validator.Name, result.ErrorCount, result.WarningCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Validator {ValidatorName} failed with exception", validator.Name);
                var errorResult = new ValidationResult
                {
                    ValidatorName = validator.Name
                };
                errorResult.AddError("SYS-001", $"Validator failed: {ex.Message}");
                results.Add(errorResult);
            }
        }

        var totalErrors = results.Sum(r => r.ErrorCount);
        var totalWarnings = results.Sum(r => r.WarningCount);
        _logger.LogInformation("Validation pipeline completed: {ErrorCount} errors, {WarningCount} warnings",
            totalErrors, totalWarnings);

        return results;
    }

    /// <summary>
    /// Build validation context from FHIR bundle
    /// </summary>
    private ValidationContext BuildContext(Bundle bundle)
    {
        var context = new ValidationContext
        {
            Bundle = bundle
        };

        // Detect bundle type based on profile or resources
        context.BundleType = DetectBundleType(bundle);

        _logger.LogInformation("Detected bundle type: {BundleType}", context.BundleType);

        // Extract resources based on bundle type
        foreach (var entry in bundle.Entry ?? Enumerable.Empty<Bundle.EntryComponent>())
        {
            switch (entry.Resource)
            {
                case MedicationRequest medRequest:
                    context.MedicationRequests.Add(medRequest);
                    break;

                case Medication medication:
                    // Store medications by ID and fullUrl
                    if (!string.IsNullOrEmpty(medication.Id))
                    {
                        context.Medications[medication.Id] = medication;
                        context.Medications[$"Medication/{medication.Id}"] = medication;
                    }
                    if (!string.IsNullOrEmpty(entry.FullUrl))
                    {
                        context.Medications[entry.FullUrl] = medication;
                    }
                    break;

                case MedicationDispense medDispense:
                    context.MedicationDispenses.Add(medDispense);
                    // Extract dispensing date from first MedicationDispense
                    if (context.DispensingDate == null && medDispense.WhenHandedOver != null)
                    {
                        if (DateTimeOffset.TryParse(medDispense.WhenHandedOver, out var dispensingDate))
                        {
                            context.DispensingDate = dispensingDate;
                        }
                    }
                    break;

                case Invoice invoice:
                    context.Invoices.Add(invoice);
                    break;
            }
        }

        // Extract PZN codes from medications
        foreach (var medication in context.Medications.Values.Distinct())
        {
            var pzn = medication.Code?.Coding?
                .FirstOrDefault(c => c.System == "http://fhir.de/CodeSystem/ifa/pzn")
                ?.Code;

            if (!string.IsNullOrEmpty(pzn) && !context.PznCodes.Contains(pzn))
            {
                context.PznCodes.Add(pzn);
            }
        }

        // Extract PZN codes from Invoice line items (Abgabedaten bundles)
        foreach (var invoice in context.Invoices)
        {
            foreach (var lineItem in invoice.LineItem ?? Enumerable.Empty<Invoice.LineItemComponent>())
            {
                // ChargeItem can be either CodeableConcept or Reference
                if (lineItem.ChargeItem is CodeableConcept chargeItemCode)
                {
                    var pzn = chargeItemCode.Coding?
                        .FirstOrDefault(c => c.System == "http://fhir.de/CodeSystem/ifa/pzn")
                        ?.Code;

                    if (!string.IsNullOrEmpty(pzn) && !context.PznCodes.Contains(pzn))
                    {
                        context.PznCodes.Add(pzn);
                    }
                }
            }
        }

        return context;
    }

    /// <summary>
    /// Detect bundle type based on profile or resource types
    /// </summary>
    private BundleType DetectBundleType(Bundle bundle)
    {
        // Check bundle profile
        var profile = bundle.Meta?.Profile?.FirstOrDefault();
        if (profile != null)
        {
            if (profile.Contains("KBV_PR_ERP_Bundle") || profile.Contains("erp-prescription"))
            {
                return BundleType.Prescription;
            }
            if (profile.Contains("AbgabedatenBundle") || profile.Contains("DAV-PR-ERP"))
            {
                return BundleType.Abgabedaten;
            }
        }

        // Fallback: detect by resource types
        var hasInvoice = bundle.Entry?.Any(e => e.Resource is Invoice) ?? false;
        var hasMedicationDispense = bundle.Entry?.Any(e => e.Resource is MedicationDispense) ?? false;
        var hasMedicationRequest = bundle.Entry?.Any(e => e.Resource is MedicationRequest) ?? false;

        if (hasInvoice || hasMedicationDispense)
        {
            return BundleType.Abgabedaten;
        }
        if (hasMedicationRequest)
        {
            return BundleType.Prescription;
        }

        return BundleType.Unknown;
    }
}
