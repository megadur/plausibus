namespace ErezeptValidator.Models.Validation;

/// <summary>
/// Represents a non-blocking validation warning
/// </summary>
public class ValidationWarning
{
    /// <summary>
    /// Warning code (e.g., FMT-002 for PZN checksum)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable warning message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Line number in the prescription where the warning occurred (1-based)
    /// </summary>
    public int? LineNumber { get; set; }

    /// <summary>
    /// Field or property that triggered the warning
    /// </summary>
    public string? Field { get; set; }

    /// <summary>
    /// Recommended action to resolve the warning
    /// </summary>
    public string? Recommendation { get; set; }

    /// <summary>
    /// Timestamp when the warning was detected
    /// </summary>
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}
