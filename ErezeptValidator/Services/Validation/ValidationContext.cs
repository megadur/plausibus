using ErezeptValidator.Models.Validation;
using System.Diagnostics;

namespace ErezeptValidator.Services.Validation;

/// <summary>
/// Context object that flows through the validation pipeline,
/// accumulating errors and warnings
/// </summary>
public class ValidationContext
{
    /// <summary>
    /// The prescription being validated
    /// </summary>
    public PrescriptionValidationRequest Request { get; }

    /// <summary>
    /// Accumulated validation errors (blocking issues)
    /// </summary>
    public List<ValidationError> Errors { get; } = new();

    /// <summary>
    /// Accumulated validation warnings (non-blocking issues)
    /// </summary>
    public List<ValidationWarning> Warnings { get; } = new();

    /// <summary>
    /// Stopwatch for tracking validation duration
    /// </summary>
    public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();

    /// <summary>
    /// Number of validation rules executed
    /// </summary>
    public int RulesChecked { get; set; }

    /// <summary>
    /// Whether validation should stop early due to critical errors
    /// </summary>
    public bool StopValidation { get; set; }

    public ValidationContext(PrescriptionValidationRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    /// <summary>
    /// Add a validation error
    /// </summary>
    public void AddError(string code, string message, int? lineNumber = null, string? field = null, string? suggestion = null)
    {
        Errors.Add(new ValidationError
        {
            Code = code,
            Message = message,
            LineNumber = lineNumber,
            Field = field,
            Severity = "ERROR",
            Suggestion = suggestion,
            DetectedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Add a validation warning
    /// </summary>
    public void AddWarning(string code, string message, int? lineNumber = null, string? field = null, string? recommendation = null)
    {
        Warnings.Add(new ValidationWarning
        {
            Code = code,
            Message = message,
            LineNumber = lineNumber,
            Field = field,
            Recommendation = recommendation,
            DetectedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Check if validation has any errors
    /// </summary>
    public bool HasErrors => Errors.Count > 0;

    /// <summary>
    /// Check if validation has any warnings
    /// </summary>
    public bool HasWarnings => Warnings.Count > 0;

    /// <summary>
    /// Get validation result status
    /// </summary>
    public string GetValidationResult()
    {
        if (StopValidation && HasErrors)
            return "INCOMPLETE"; // Validation stopped due to critical errors

        return HasErrors ? "FAIL" : "PASS";
    }
}
