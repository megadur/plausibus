using ErezeptValidator.Data;
using ErezeptValidator.Models.Abdata;
using ErezeptValidator.Models.Validation;
using ErezeptValidator.Services.Validation.Helpers;
using Hl7.Fhir.Model;

namespace ErezeptValidator.Services.Validation.Validators;

/// <summary>
/// Validates Cannabis prescriptions according to § 31 Abs. 6 SGB V and TA1 Version 039
/// Implements CAN-001 to CAN-005 validation rules
/// </summary>
public class CannabisValidator : IValidator
{
    private readonly IPznRepository _pznRepository;
    private readonly ILogger<CannabisValidator> _logger;

    // Cannabis special codes (TA1 Annex 10)
    private static readonly HashSet<string> ValidCannabisSokCodes = new()
    {
        "06461446", // Dried cannabis flowers (getrocknete Blüten)
        "06461423", // Cannabis extracts
        "06460665", // Dronabinol preparation type 1
        "06460694", // Dronabinol preparation type 2
        "06460748", // Cannabis preparation type 3
        "06460754"  // Cannabis preparation type 4
    };

    public string Name => "Cannabis Validator";

    public CannabisValidator(
        IPznRepository pznRepository,
        ILogger<CannabisValidator> logger)
    {
        _pznRepository = pznRepository;
        _logger = logger;
    }

    public async System.Threading.Tasks.Task<ValidationResult> ValidateAsync(ValidationContext context)
    {
        var result = ValidationResult.Success(Name);

        // Detect Cannabis medications and store in context
        var cannabisPzns = await DetectCannabisMedicationsAsync(context, result);
        var cannabisSokCodes = DetectCannabisSokCodes(context);

        // Only run Cannabis validation if Cannabis medications or SOK codes are present
        if (cannabisPzns.Count == 0 && cannabisSokCodes.Count == 0)
        {
            _logger.LogDebug("No Cannabis medications detected, skipping Cannabis validation");
            return result;
        }

        _logger.LogInformation("Validating {PznCount} Cannabis PZNs and {SokCount} Cannabis SOK codes",
            cannabisPzns.Count, cannabisSokCodes.Count);

        // CAN-001: Cannabis Special Codes
        ValidateCannabisSokCodes(context, cannabisSokCodes, result);

        // CAN-002: No BTM/T-Rezept Substances in Cannabis Preparations
        await ValidateNoBtmSubstancesAsync(context, cannabisPzns, result);

        // CAN-003: Cannabis Factor Field Value
        ValidateCannabisFactor(context, cannabisSokCodes, result);

        // CAN-004: Cannabis Bruttopreis Calculation
        ValidateCannabisPriceCalculation(context, cannabisSokCodes, result);

        // CAN-005: Cannabis Manufacturing Data Required
        ValidateManufacturingData(context, cannabisSokCodes, result);

        return result;
    }

    /// <summary>
    /// Detect Cannabis medications via ABDATA and store flags in context
    /// </summary>
    private async System.Threading.Tasks.Task<HashSet<string>> DetectCannabisMedicationsAsync(ValidationContext context, ValidationResult result)
    {
        var cannabisPzns = new HashSet<string>();

        // Batch lookup for all PZNs
        if (context.PznCodes.Count > 0)
        {
            var articles = await _pznRepository.GetByPznBatchAsync(context.PznCodes.ToArray());

            foreach (var pzn in context.PznCodes)
            {
                if (articles.TryGetValue(pzn, out var article))
                {
                    // Check Cannabis status (field G8)
                    // 0 = keine Angabe, 1 = nein, 2 = Cannabis MedCanG §2 Nr.1, 3 = Cannabis MedCanG §2 Nr.2
                    if (article.IsCannabis)
                    {
                        cannabisPzns.Add(pzn);

                        string cannabisType = article.Cannabis == 2
                            ? "Cannabis MedCanG §2 Nr.1"
                            : "Cannabis MedCanG §2 Nr.2";

                        result.AddInfo("CANNABIS-DETECTED",
                            $"Cannabis medication detected: {pzn} - {article.Name} ({cannabisType})",
                            "PZN", pzn);

                        // Store Cannabis flag for other validators
                        context.Metadata[$"cannabis_{pzn}"] = true;
                        context.Metadata[$"cannabis_type_{pzn}"] = article.Cannabis;
                        context.Metadata[$"article_{pzn}"] = article;
                    }
                }
            }
        }

        return cannabisPzns;
    }

    /// <summary>
    /// Detect Cannabis SOK codes in Invoice line items
    /// </summary>
    private HashSet<string> DetectCannabisSokCodes(ValidationContext context)
    {
        var cannabisSokCodes = new HashSet<string>();

        // Only applies to Abgabedaten bundles
        if (context.BundleType != BundleType.Abgabedaten)
        {
            return cannabisSokCodes;
        }

        foreach (var invoice in context.Invoices)
        {
            foreach (var lineItem in invoice.LineItem ?? Enumerable.Empty<Invoice.LineItemComponent>())
            {
                var sok = FhirDataExtractor.ExtractSokCode(lineItem);
                if (!string.IsNullOrEmpty(sok) && ValidCannabisSokCodes.Contains(sok))
                {
                    cannabisSokCodes.Add(sok);
                    context.Metadata[$"cannabis_sok_{sok}"] = true;
                }
            }
        }

        return cannabisSokCodes;
    }

    /// <summary>
    /// CAN-001: Cannabis Special Codes
    /// Validates that Cannabis preparations use valid special codes from Annex 10
    /// </summary>
    private void ValidateCannabisSokCodes(ValidationContext context, HashSet<string> cannabisSokCodes, ValidationResult result)
    {
        // Only applies to Abgabedaten bundles
        if (context.BundleType != BundleType.Abgabedaten)
        {
            return;
        }

        // If we detected Cannabis PZNs but no valid Cannabis SOK codes, that's an error
        var hasCannabisArticles = context.Metadata.Keys.Any(k => k.StartsWith("cannabis_") && k.Contains("_pzn"));

        if (hasCannabisArticles && cannabisSokCodes.Count == 0)
        {
            result.AddError("CAN-001-E",
                $"Cannabis preparation detected but no valid Cannabis special code found. " +
                $"Expected one of: {string.Join(", ", ValidCannabisSokCodes)} per § 31 Abs. 6 SGB V.",
                "Invoice.LineItem");

            _logger.LogWarning("CAN-001: Cannabis preparation without valid SOK code");
        }
        else if (cannabisSokCodes.Count > 0)
        {
            result.AddInfo("CAN-001-I",
                $"Cannabis special codes present: {string.Join(", ", cannabisSokCodes)}",
                "Invoice.LineItem");
        }
    }

    /// <summary>
    /// CAN-002: Cannabis - No BTM/T-Rezept Substances
    /// Ensures Cannabis preparations do not contain BTM or T-Rezept substances
    /// </summary>
    private async System.Threading.Tasks.Task ValidateNoBtmSubstancesAsync(ValidationContext context, HashSet<string> cannabisPzns, ValidationResult result)
    {
        // Check if any Cannabis line items also have BTM or T-Rezept flags
        var articles = await _pznRepository.GetByPznBatchAsync(cannabisPzns.ToArray());

        foreach (var pzn in cannabisPzns)
        {
            if (articles.TryGetValue(pzn, out var article))
            {
                if (article.IsBtm)
                {
                    result.AddError("CAN-002-E",
                        $"Cannabis preparation {pzn} ({article.Name}) contains BTM substance. " +
                        $"Cannabis and BTM are mutually exclusive per TA1 Section 4.14.2. " +
                        $"Use separate BTM billing process.",
                        "PZN", pzn);

                    _logger.LogWarning("CAN-002: Cannabis with BTM substance detected - PZN {Pzn}", pzn);
                }
                else if (article.IsTRezept)
                {
                    result.AddError("CAN-002-E",
                        $"Cannabis preparation {pzn} ({article.Name}) contains T-Rezept substance. " +
                        $"Cannabis and T-Rezept are mutually exclusive per TA1 Section 4.14.2. " +
                        $"Use separate T-Rezept billing process.",
                        "PZN", pzn);

                    _logger.LogWarning("CAN-002: Cannabis with T-Rezept substance detected - PZN {Pzn}", pzn);
                }
            }
        }
    }

    /// <summary>
    /// CAN-003: Cannabis - Faktor Field Value
    /// Validates that Cannabis special code line has Factor = 1
    /// </summary>
    private void ValidateCannabisFactor(ValidationContext context, HashSet<string> cannabisSokCodes, ValidationResult result)
    {
        // Only applies to Abgabedaten bundles
        if (context.BundleType != BundleType.Abgabedaten)
        {
            return;
        }

        foreach (var invoice in context.Invoices)
        {
            int lineNumber = 0;
            foreach (var lineItem in invoice.LineItem ?? Enumerable.Empty<Invoice.LineItemComponent>())
            {
                lineNumber++;
                var sok = FhirDataExtractor.ExtractSokCode(lineItem);

                if (!string.IsNullOrEmpty(sok) && cannabisSokCodes.Contains(sok))
                {
                    var (factorId, factorValue) = FhirDataExtractor.ExtractFactor(lineItem);

                    if (factorValue.HasValue)
                    {
                        // Factor must be exactly 1 (or 1.000000)
                        if (Math.Abs(factorValue.Value - 1.0m) > 0.000001m)
                        {
                            result.AddError("CAN-003-E",
                                $"Cannabis special code line {lineNumber} (SOK {sok}) must have Factor = '1'. " +
                                $"Found: {factorValue.Value}. Detailed manufacturing data uses calculated factors per REZ-018.",
                                $"Invoice.LineItem[{lineNumber}].Factor");

                            _logger.LogWarning(
                                "CAN-003: Cannabis factor validation failed for line {LineNumber} SOK {Sok}: Factor = {Factor}",
                                lineNumber, sok, factorValue.Value);
                        }
                    }
                    else
                    {
                        result.AddError("CAN-003-E",
                            $"Cannabis special code line {lineNumber} (SOK {sok}) missing Factor field. " +
                            $"Factor must be present and equal to '1'.",
                            $"Invoice.LineItem[{lineNumber}].Factor");
                    }
                }
            }
        }
    }

    /// <summary>
    /// CAN-004: Cannabis - Bruttopreis Calculation
    /// Validates gross price calculation for Cannabis preparations
    /// </summary>
    private void ValidateCannabisPriceCalculation(ValidationContext context, HashSet<string> cannabisSokCodes, ValidationResult result)
    {
        // Only applies to Abgabedaten bundles
        if (context.BundleType != BundleType.Abgabedaten)
        {
            return;
        }

        foreach (var invoice in context.Invoices)
        {
            int lineNumber = 0;
            foreach (var lineItem in invoice.LineItem ?? Enumerable.Empty<Invoice.LineItemComponent>())
            {
                lineNumber++;
                var sok = FhirDataExtractor.ExtractSokCode(lineItem);

                if (!string.IsNullOrEmpty(sok) && cannabisSokCodes.Contains(sok))
                {
                    var (priceId, priceValue) = FhirDataExtractor.ExtractPrice(lineItem);

                    if (!priceValue.HasValue || priceValue.Value <= 0)
                    {
                        result.AddError("CAN-004-E",
                            $"Cannabis special code line {lineNumber} (SOK {sok}) missing or invalid Bruttopreis. " +
                            $"Gross price must equal total amount to be billed including all fees per AMPreisV.",
                            $"Invoice.LineItem[{lineNumber}].Price");

                        _logger.LogWarning(
                            "CAN-004: Cannabis price validation failed for line {LineNumber} SOK {Sok}",
                            lineNumber, sok);
                    }
                    else
                    {
                        // Basic price validation - detailed calculation would require full ingredient breakdown
                        result.AddInfo("CAN-004-I",
                            $"Cannabis special code line {lineNumber} (SOK {sok}) has Bruttopreis: {priceValue.Value:F2} EUR. " +
                            $"Verify against Annex 10 pricing tables and AMPreisV rules.",
                            $"Invoice.LineItem[{lineNumber}].Price");
                    }
                }
            }
        }
    }

    /// <summary>
    /// CAN-005: Cannabis - Manufacturing Data Required
    /// Validates that all Cannabis preparations include required manufacturing data
    /// </summary>
    private void ValidateManufacturingData(ValidationContext context, HashSet<string> cannabisSokCodes, ValidationResult result)
    {
        // Only applies to Abgabedaten bundles
        if (context.BundleType != BundleType.Abgabedaten)
        {
            return;
        }

        // Check for Manufacturing extension in MedicationDispense resources
        foreach (var medicationDispense in context.MedicationDispenses)
        {
            // Look for manufacturing/compounding data in extensions
            var manufacturingExtension = medicationDispense.Extension?
                .FirstOrDefault(e => e.Url?.Contains("manufacturing") == true ||
                                   e.Url?.Contains("compounding") == true ||
                                   e.Url?.Contains("Herstellung") == true);

            bool hasManufacturingData = manufacturingExtension != null;

            // Also check for batch number as alternative indicator
            if (!hasManufacturingData && !string.IsNullOrEmpty(medicationDispense.WhenHandedOver))
            {
                // If we have a dispensing date but no manufacturing data, check if Cannabis SOK present
                if (cannabisSokCodes.Count > 0)
                {
                    // Look for manufacturer identifier in extensions
                    var manufacturerExt = medicationDispense.Extension?
                        .Any(e => e.Url?.Contains("manufacturer") == true ||
                                 e.Url?.Contains("Hersteller") == true) ?? false;

                    hasManufacturingData = manufacturerExt;
                }
            }

            if (cannabisSokCodes.Count > 0 && !hasManufacturingData)
            {
                result.AddWarning("CAN-005-W",
                    $"Cannabis preparation detected but manufacturing data (Herstellungssegment) may be incomplete. " +
                    $"Cannabis requires: manufacturer ID, timestamp, counter, and batch designation per TA1 Section 4.14.2.",
                    "MedicationDispense.Extension");

                _logger.LogWarning(
                    "CAN-005: Cannabis prescription missing complete manufacturing data - MedicationDispense {Id}",
                    medicationDispense.Id);
            }
        }

        // Also check Invoice line items for manufacturing segment indicators
        foreach (var invoice in context.Invoices)
        {
            int lineNumber = 0;
            foreach (var lineItem in invoice.LineItem ?? Enumerable.Empty<Invoice.LineItemComponent>())
            {
                lineNumber++;
                var sok = FhirDataExtractor.ExtractSokCode(lineItem);

                if (!string.IsNullOrEmpty(sok) && cannabisSokCodes.Contains(sok))
                {
                    // Check for manufacturing timestamp extension
                    var hasTimestamp = lineItem.Extension?
                        .Any(e => e.Url?.Contains("timestamp") == true ||
                                 e.Url?.Contains("Zeitstempel") == true) ?? false;

                    // Check for counter extension
                    var hasCounter = lineItem.Extension?
                        .Any(e => e.Url?.Contains("counter") == true ||
                                 e.Url?.Contains("Zaehler") == true) ?? false;

                    // Check for manufacturer identifier
                    var hasManufacturer = lineItem.Extension?
                        .Any(e => e.Url?.Contains("manufacturer") == true ||
                                 e.Url?.Contains("Hersteller") == true) ?? false;

                    if (!hasTimestamp || !hasCounter || !hasManufacturer)
                    {
                        var missingFields = new List<string>();
                        if (!hasManufacturer) missingFields.Add("Manufacturer ID");
                        if (!hasTimestamp) missingFields.Add("Timestamp");
                        if (!hasCounter) missingFields.Add("Counter");

                        result.AddError("CAN-005-E",
                            $"Cannabis special code line {lineNumber} (SOK {sok}) missing required manufacturing data: " +
                            $"{string.Join(", ", missingFields)}. " +
                            $"All Cannabis preparations require complete Herstellungssegment per TA1 Section 4.14.2.",
                            $"Invoice.LineItem[{lineNumber}]");

                        _logger.LogWarning(
                            "CAN-005: Cannabis line {LineNumber} missing manufacturing fields: {Fields}",
                            lineNumber, string.Join(", ", missingFields));
                    }
                }
            }
        }
    }
}
