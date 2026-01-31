namespace ErezeptValidator.Models.Validation;

/// <summary>
/// Represents a single validation issue (error, warning, or info)
/// </summary>
public class ValidationIssue
{
    /// <summary>
    /// TA1 rule code (e.g., "FMT-001-E", "BTM-002-W")
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Severity level
    /// </summary>
    public required ValidationSeverity Severity { get; set; }

    /// <summary>
    /// Human-readable message
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Field or location where the issue occurred (optional)
    /// </summary>
    public string? Field { get; set; }

    /// <summary>
    /// PZN associated with this issue (optional)
    /// </summary>
    public string? Pzn { get; set; }

    /// <summary>
    /// Additional context or details (optional)
    /// </summary>
    public Dictionary<string, object>? Details { get; set; }

    /// <summary>
    /// Creates an error issue
    /// </summary>
    public static ValidationIssue Error(string code, string message, string? field = null, string? pzn = null)
    {
        return new ValidationIssue
        {
            Code = code,
            Severity = ValidationSeverity.Error,
            Message = message,
            Field = field,
            Pzn = pzn
        };
    }

    /// <summary>
    /// Creates a warning issue
    /// </summary>
    public static ValidationIssue Warning(string code, string message, string? field = null, string? pzn = null)
    {
        return new ValidationIssue
        {
            Code = code,
            Severity = ValidationSeverity.Warning,
            Message = message,
            Field = field,
            Pzn = pzn
        };
    }

    /// <summary>
    /// Creates an info issue
    /// </summary>
    public static ValidationIssue Info(string code, string message, string? field = null, string? pzn = null)
    {
        return new ValidationIssue
        {
            Code = code,
            Severity = ValidationSeverity.Info,
            Message = message,
            Field = field,
            Pzn = pzn
        };
    }
}
