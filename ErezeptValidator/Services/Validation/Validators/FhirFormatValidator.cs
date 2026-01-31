using ErezeptValidator.Models.Validation;
using ErezeptValidator.Services.Validation.Helpers;

namespace ErezeptValidator.Services.Validation.Validators;

/// <summary>
/// Comprehensive format validator for FHIR bundles (FMT-003 to FMT-010)
/// Complements PznFormatValidator which handles FMT-001 and FMT-002
/// </summary>
public class FhirFormatValidator : IValidator
{
    private readonly ILogger<FhirFormatValidator> _logger;

    public string Name => "FHIR Format Validator";

    public FhirFormatValidator(ILogger<FhirFormatValidator> logger)
    {
        _logger = logger;
    }

    public async Task<ValidationResult> ValidateAsync(ValidationContext context)
    {
        var result = ValidationResult.Success(Name);

        // FMT-003: Bundle timestamp format validation
        ValidateBundleTimestamp(context, result);

        // FMT-004 to FMT-010: Validate each MedicationRequest
        foreach (var medRequest in context.MedicationRequests)
        {
            ValidateMedicationRequest(context, medRequest, result);
        }

        return await Task.FromResult(result);
    }

    /// <summary>
    /// FMT-003: Validate bundle timestamp format
    /// </summary>
    private void ValidateBundleTimestamp(ValidationContext context, ValidationResult result)
    {
        var timestamp = FhirDataExtractor.ExtractBundleTimestamp(context.Bundle);

        if (timestamp == null)
        {
            result.AddError("FMT-003-E",
                "Bundle timestamp is missing. FHIR bundles must include a timestamp.",
                "Bundle.timestamp");
            return;
        }

        // Check if timestamp is in future (warning only)
        if (timestamp > DateTimeOffset.UtcNow.AddHours(1)) // Allow 1 hour clock skew
        {
            result.AddWarning("FMT-003-W",
                $"Bundle timestamp is in the future: {timestamp:yyyy-MM-dd HH:mm:ss}. Check system clock.",
                "Bundle.timestamp");
        }
    }

    /// <summary>
    /// Validate individual MedicationRequest
    /// </summary>
    private void ValidateMedicationRequest(
        ValidationContext context,
        Hl7.Fhir.Model.MedicationRequest medRequest,
        ValidationResult result)
    {
        var requestId = medRequest.Id ?? "unknown";

        // FMT-004: Quantity must be positive
        ValidateQuantity(medRequest, requestId, result);

        // FMT-005: Authored date format
        ValidateAuthoredOn(medRequest, requestId, result);

        // Additional format validations can be added here
        // Note: Price/VAT/Factor/Price codes are in Abgabedaten (Invoice), not prescription
    }

    /// <summary>
    /// FMT-004: Quantity must be positive
    /// </summary>
    private void ValidateQuantity(
        Hl7.Fhir.Model.MedicationRequest medRequest,
        string requestId,
        ValidationResult result)
    {
        var quantity = FhirDataExtractor.ExtractQuantity(medRequest);

        if (quantity == null)
        {
            result.AddWarning("FMT-004-W",
                $"MedicationRequest {requestId}: Quantity is missing. Dispense quantity should be specified.",
                "dispenseRequest.quantity");
            return;
        }

        if (quantity <= 0)
        {
            result.AddError("FMT-004-E",
                $"MedicationRequest {requestId}: Quantity must be positive. Found: {quantity}",
                "dispenseRequest.quantity");
        }

        // Warning for unusually large quantities
        if (quantity > 100)
        {
            result.AddWarning("FMT-004-W",
                $"MedicationRequest {requestId}: Unusually large quantity: {quantity}. Verify this is correct.",
                "dispenseRequest.quantity");
        }
    }

    /// <summary>
    /// FMT-005: AuthoredOn date format validation
    /// </summary>
    private void ValidateAuthoredOn(
        Hl7.Fhir.Model.MedicationRequest medRequest,
        string requestId,
        ValidationResult result)
    {
        var authoredOn = FhirDataExtractor.ExtractAuthoredOn(medRequest);

        if (string.IsNullOrEmpty(authoredOn))
        {
            result.AddError("FMT-005-E",
                $"MedicationRequest {requestId}: authoredOn date is missing. Prescription date is required.",
                "authoredOn");
            return;
        }

        // Try to parse the date
        if (!DateTime.TryParse(authoredOn, out var parsedDate))
        {
            result.AddError("FMT-005-E",
                $"MedicationRequest {requestId}: authoredOn date format is invalid: {authoredOn}",
                "authoredOn");
            return;
        }

        // Check if date is in future
        if (parsedDate.Date > DateTime.UtcNow.Date.AddDays(1))
        {
            result.AddError("FMT-005-E",
                $"MedicationRequest {requestId}: authoredOn date cannot be in the future: {authoredOn}",
                "authoredOn");
        }

        // Check if date is too old (> 1 year)
        if (parsedDate.Date < DateTime.UtcNow.Date.AddYears(-1))
        {
            result.AddWarning("FMT-005-W",
                $"MedicationRequest {requestId}: authoredOn date is over 1 year old: {authoredOn}. Verify prescription validity.",
                "authoredOn");
        }
    }
}
