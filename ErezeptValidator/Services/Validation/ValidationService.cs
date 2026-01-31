using ErezeptValidator.Models.Validation;
using ErezeptValidator.Services.Validation.Validators;

namespace ErezeptValidator.Services.Validation;

/// <summary>
/// Main validation service that orchestrates all validators using Chain of Responsibility pattern
/// </summary>
public class ValidationService : IValidationService
{
    private readonly ILogger<ValidationService> _logger;
    private readonly IEnumerable<IValidator> _validators;

    public ValidationService(
        ILogger<ValidationService> logger,
        FormatValidator formatValidator,
        GeneralRuleValidator generalRuleValidator,
        CalculationValidator calculationValidator)
    {
        _logger = logger;

        // Register validators in execution order (Order property determines sequence)
        _validators = new List<IValidator>
        {
            formatValidator,
            generalRuleValidator,
            calculationValidator
        }.OrderBy(v => v.Order).ToList();

        _logger.LogInformation("ValidationService initialized with {ValidatorCount} validators", _validators.Count());
    }

    /// <summary>
    /// Validate a prescription request through the complete validation pipeline
    /// </summary>
    public async Task<PrescriptionValidationResponse> ValidateAsync(PrescriptionValidationRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogInformation("Starting validation for prescription {PrescriptionId} with {LineItemCount} line items",
            request.PrescriptionId, request.LineItems.Count);

        // Create validation context
        var context = new ValidationContext(request);

        try
        {
            // Execute validators in order (Chain of Responsibility pattern)
            foreach (var validator in _validators)
            {
                _logger.LogDebug("Executing {ValidatorName} (Order: {Order})",
                    validator.ValidatorName, validator.Order);

                await validator.ValidateAsync(context);

                // Stop if critical errors found (optional: can be configured per validator)
                if (context.StopValidation)
                {
                    _logger.LogWarning("Validation stopped early due to critical errors after {ValidatorName}",
                        validator.ValidatorName);
                    break;
                }
            }

            // Build response
            var response = BuildResponse(context);

            _logger.LogInformation("Validation completed for {PrescriptionId}: Result={Result}, Errors={ErrorCount}, Warnings={WarningCount}, Duration={DurationMs}ms",
                request.PrescriptionId,
                response.ValidationResult,
                response.Errors.Count,
                response.Warnings.Count,
                response.Summary.DurationMs);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during validation of {PrescriptionId}", request.PrescriptionId);

            // Return error response
            return new PrescriptionValidationResponse
            {
                ValidationResult = "ERROR",
                PrescriptionId = request.PrescriptionId,
                Errors = new List<ValidationError>
                {
                    new ValidationError
                    {
                        Code = "INTERNAL-ERROR",
                        Message = $"Internal validation error: {ex.Message}",
                        Severity = "ERROR",
                        DetectedAt = DateTime.UtcNow
                    }
                },
                Summary = new ValidationSummary
                {
                    ErrorCount = 1,
                    DurationMs = context.Stopwatch.ElapsedMilliseconds
                },
                Metadata = new ValidationMetadata
                {
                    ValidatedAt = DateTime.UtcNow,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                }
            };
        }
    }

    /// <summary>
    /// Build the final validation response from context
    /// </summary>
    private PrescriptionValidationResponse BuildResponse(ValidationContext context)
    {
        context.Stopwatch.Stop();

        var response = new PrescriptionValidationResponse
        {
            ValidationResult = context.GetValidationResult(),
            PrescriptionId = context.Request.PrescriptionId,
            Errors = context.Errors,
            Warnings = context.Warnings,
            Summary = new ValidationSummary
            {
                TotalRulesChecked = context.RulesChecked,
                ErrorCount = context.Errors.Count,
                WarningCount = context.Warnings.Count,
                LineItemsValidated = context.Request.LineItems.Count,
                DurationMs = context.Stopwatch.ElapsedMilliseconds
            },
            Metadata = new ValidationMetadata
            {
                ValidatedAt = DateTime.UtcNow,
                ValidatorVersion = "1.0.0-mvp",
                RulesVersion = "TA1-039-2025",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            }
        };

        return response;
    }
}
