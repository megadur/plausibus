namespace ErezeptValidator.Models.Validation;

/// <summary>
/// Response model for prescription validation
/// </summary>
public class PrescriptionValidationResponse
{
    /// <summary>
    /// Overall validation result: PASS, FAIL, or INCOMPLETE
    /// </summary>
    public string ValidationResult { get; set; } = "INCOMPLETE";

    /// <summary>
    /// Summary of validation results
    /// </summary>
    public ValidationSummary Summary { get; set; } = new();

    /// <summary>
    /// List of validation errors (blocking issues)
    /// </summary>
    public List<ValidationError> Errors { get; set; } = new();

    /// <summary>
    /// List of validation warnings (non-blocking issues)
    /// </summary>
    public List<ValidationWarning> Warnings { get; set; } = new();

    /// <summary>
    /// Metadata about the validation process
    /// </summary>
    public ValidationMetadata Metadata { get; set; } = new();

    /// <summary>
    /// Prescription identifier from the request
    /// </summary>
    public string? PrescriptionId { get; set; }
}

/// <summary>
/// Summary statistics for validation results
/// </summary>
public class ValidationSummary
{
    /// <summary>
    /// Total number of validation rules checked
    /// </summary>
    public int TotalRulesChecked { get; set; }

    /// <summary>
    /// Number of errors found
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// Number of warnings found
    /// </summary>
    public int WarningCount { get; set; }

    /// <summary>
    /// Number of line items validated
    /// </summary>
    public int LineItemsValidated { get; set; }

    /// <summary>
    /// Validation duration in milliseconds
    /// </summary>
    public long DurationMs { get; set; }
}

/// <summary>
/// Metadata about the validation process
/// </summary>
public class ValidationMetadata
{
    /// <summary>
    /// Timestamp when validation was performed
    /// </summary>
    public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Validator version (semantic versioning)
    /// </summary>
    public string ValidatorVersion { get; set; } = "1.0.0-mvp";

    /// <summary>
    /// TA1 specification version used for validation
    /// </summary>
    public string RulesVersion { get; set; } = "TA1-039-2025";

    /// <summary>
    /// Server environment (Development, Production, etc.)
    /// </summary>
    public string? Environment { get; set; }
}
