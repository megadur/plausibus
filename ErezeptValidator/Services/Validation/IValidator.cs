using ErezeptValidator.Models.Validation;

namespace ErezeptValidator.Services.Validation;

/// <summary>
/// Interface for all validators
/// </summary>
public interface IValidator
{
    /// <summary>
    /// Name of the validator
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Validate the context and return results
    /// </summary>
    Task<ValidationResult> ValidateAsync(ValidationContext context);
}
