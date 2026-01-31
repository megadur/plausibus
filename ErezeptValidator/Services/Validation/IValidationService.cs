using ErezeptValidator.Models.Validation;

namespace ErezeptValidator.Services.Validation;

/// <summary>
/// Service interface for validating prescriptions according to TA1 rules
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Validate a prescription request and return detailed results
    /// </summary>
    /// <param name="request">Prescription validation request with line items</param>
    /// <returns>Validation response with errors, warnings, and metadata</returns>
    Task<PrescriptionValidationResponse> ValidateAsync(PrescriptionValidationRequest request);
}
