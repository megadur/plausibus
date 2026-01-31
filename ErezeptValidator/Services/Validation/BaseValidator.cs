using Microsoft.Extensions.Logging;

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
    public abstract string ValidatorName { get; }

    /// <summary>
    /// Execution order (lower runs first)
    /// </summary>
    public abstract int Order { get; }

    /// <summary>
    /// Validate the prescription
    /// </summary>
    public async Task ValidateAsync(ValidationContext context)
    {
        Logger.LogDebug("Starting {ValidatorName} validation", ValidatorName);

        var errorCountBefore = context.Errors.Count;
        var warningCountBefore = context.Warnings.Count;

        try
        {
            await ExecuteValidationAsync(context);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in {ValidatorName} validation", ValidatorName);
            context.AddError(
                code: "INTERNAL-ERROR",
                message: $"Internal validation error in {ValidatorName}: {ex.Message}",
                suggestion: "Contact support if this error persists"
            );
        }

        var errorsAdded = context.Errors.Count - errorCountBefore;
        var warningsAdded = context.Warnings.Count - warningCountBefore;

        if (errorsAdded > 0 || warningsAdded > 0)
        {
            Logger.LogInformation("{ValidatorName} found {ErrorCount} errors and {WarningCount} warnings",
                ValidatorName, errorsAdded, warningsAdded);
        }
        else
        {
            Logger.LogDebug("{ValidatorName} validation passed", ValidatorName);
        }
    }

    /// <summary>
    /// Execute the actual validation logic (implemented by derived classes)
    /// </summary>
    protected abstract Task ExecuteValidationAsync(ValidationContext context);
}
