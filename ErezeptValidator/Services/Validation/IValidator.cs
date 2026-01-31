using ErezeptValidator.Models.Validation;

namespace ErezeptValidator.Services.Validation;

/// <summary>
/// Base interface for all validators in the validation pipeline
/// </summary>
public interface IValidator
{
    /// <summary>
    /// Validate the prescription and add errors/warnings to the context
    /// </summary>
    /// <param name="context">Validation context containing prescription data and results</param>
    Task ValidateAsync(ValidationContext context);

    /// <summary>
    /// Name of this validator for logging and debugging
    /// </summary>
    string ValidatorName { get; }

    /// <summary>
    /// Execution order (lower numbers run first)
    /// </summary>
    int Order { get; }
}
