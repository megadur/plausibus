namespace ErezeptValidator.Models.Validation;

/// <summary>
/// Severity level for validation issues
/// </summary>
public enum ValidationSeverity
{
    /// <summary>
    /// Critical error - prescription cannot be processed
    /// </summary>
    Error,

    /// <summary>
    /// Warning - prescription can be processed but may have issues
    /// </summary>
    Warning,

    /// <summary>
    /// Information - for tracking purposes only
    /// </summary>
    Info
}
