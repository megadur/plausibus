using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErezeptValidator.Models.Ta1Reference;

/// <summary>
/// Audit log for validation requests and results.
/// Stores validation history for compliance and debugging purposes.
/// </summary>
[Table("validation_logs", Schema = "ta1_reference")]
public class ValidationLog
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// E-Rezept prescription ID (Task ID)
    /// </summary>
    [StringLength(100)]
    [Column("prescription_id")]
    public string? PrescriptionId { get; set; }

    /// <summary>
    /// When the validation was performed
    /// </summary>
    [Column("validation_timestamp")]
    public DateTime ValidationTimestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Overall validation result: "PASS", "ERROR", "WARNING"
    /// </summary>
    [Required]
    [StringLength(20)]
    [Column("validation_result")]
    public string ValidationResult { get; set; } = string.Empty;

    /// <summary>
    /// List of validation rules that were triggered (JSONB)
    /// Format: ["FMT-001", "GEN-006", ...]
    /// </summary>
    [Column("rules_triggered", TypeName = "jsonb")]
    public string? RulesTriggered { get; set; }

    /// <summary>
    /// Array of error objects (JSONB)
    /// Format: [{"ruleCode": "FMT-001", "severity": "ERROR", "message": "...", ...}, ...]
    /// </summary>
    [Column("errors", TypeName = "jsonb")]
    public string? Errors { get; set; }

    /// <summary>
    /// Array of warning objects (JSONB)
    /// Format: [{"ruleCode": "FMT-002", "severity": "WARNING", "message": "...", ...}, ...]
    /// </summary>
    [Column("warnings", TypeName = "jsonb")]
    public string? Warnings { get; set; }

    /// <summary>
    /// Complete prescription data that was validated (JSONB)
    /// Stores the full request for audit purposes
    /// </summary>
    [Column("prescription_data", TypeName = "jsonb")]
    public string? PrescriptionData { get; set; }
}
