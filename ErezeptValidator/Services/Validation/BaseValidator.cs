using Microsoft.Extensions.Logging;
using ErezeptValidator.Models.Validation;

namespace ErezeptValidator.Services.Validation;

/// <summary>
/// Base class for all validators providing common functionality
/// </summary>
public abstract class BaseValidator : IValidator
{
    protected readonly ILogger Logger;

    protected BaseValidator(ILogger logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Name of this validator for logging
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Validate the prescription
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(ValidationContext context)
    {
        var result = ValidationResult.Success(Name);

        Logger.LogDebug("Starting {ValidatorName} validation", Name);

        try
        {
            await ExecuteValidationAsync(context, result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in {ValidatorName} validation", Name);
            result.AddError("INTERNAL-ERROR", $"Internal validation error in {Name}: {ex.Message}");
        }

        if (result.ErrorCount > 0 || result.WarningCount > 0)
        {
            Logger.LogInformation("{ValidatorName} found {ErrorCount} errors and {WarningCount} warnings",
                Name, result.ErrorCount, result.WarningCount);
        }
        else
        {
            Logger.LogDebug("{ValidatorName} validation passed", Name);
        }

        return result;
    }

    /// <summary>
    /// Execute the actual validation logic (implemented by derived classes)
    /// </summary>
    protected abstract Task ExecuteValidationAsync(ValidationContext context, ValidationResult result);
}
