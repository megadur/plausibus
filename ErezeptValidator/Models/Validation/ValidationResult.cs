namespace ErezeptValidator.Models.Validation;

/// <summary>
/// Result of validation operation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Name of the validator that produced this result
    /// </summary>
    public required string ValidatorName { get; set; }

    /// <summary>
    /// List of validation issues found
    /// </summary>
    public List<ValidationIssue> Issues { get; set; } = new();

    /// <summary>
    /// Whether validation passed (no errors)
    /// </summary>
    public bool IsValid => !Issues.Any(i => i.Severity == ValidationSeverity.Error);

    /// <summary>
    /// Count of errors
    /// </summary>
    public int ErrorCount => Issues.Count(i => i.Severity == ValidationSeverity.Error);

    /// <summary>
    /// Count of warnings
    /// </summary>
    public int WarningCount => Issues.Count(i => i.Severity == ValidationSeverity.Warning);

    /// <summary>
    /// Count of info messages
    /// </summary>
    public int InfoCount => Issues.Count(i => i.Severity == ValidationSeverity.Info);

    /// <summary>
    /// Add an issue to the result
    /// </summary>
    public void AddIssue(ValidationIssue issue)
    {
        Issues.Add(issue);
    }

    /// <summary>
    /// Add an error
    /// </summary>
    public void AddError(string code, string message, string? field = null, string? pzn = null)
    {
        AddIssue(ValidationIssue.Error(code, message, field, pzn));
    }

    /// <summary>
    /// Add a warning
    /// </summary>
    public void AddWarning(string code, string message, string? field = null, string? pzn = null)
    {
        AddIssue(ValidationIssue.Warning(code, message, field, pzn));
    }

    /// <summary>
    /// Add an info message
    /// </summary>
    public void AddInfo(string code, string message, string? field = null, string? pzn = null)
    {
        AddIssue(ValidationIssue.Info(code, message, field, pzn));
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static ValidationResult Success(string validatorName)
    {
        return new ValidationResult { ValidatorName = validatorName };
    }
}
