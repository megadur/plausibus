namespace ErezeptValidator.Models.Validation;

/// <summary>
/// Represents a validation error with severity and details
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Error code (e.g., FMT-001, GEN-006, BTM-001)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Line number in the prescription where the error occurred (1-based)
    /// </summary>
    public int? LineNumber { get; set; }

    /// <summary>
    /// Field or property that caused the error
    /// </summary>
    public string? Field { get; set; }

    /// <summary>
    /// Severity level: ERROR (blocking), WARNING (non-blocking)
    /// </summary>
    public string Severity { get; set; } = "ERROR";

    /// <summary>
    /// Suggested correction or additional context
    /// </summary>
    public string? Suggestion { get; set; }

    /// <summary>
    /// Timestamp when the error was detected
    /// </summary>
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}
