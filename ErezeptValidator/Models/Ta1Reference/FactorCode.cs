using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErezeptValidator.Models.Ta1Reference;

/// <summary>
/// Factor identifier code (Faktorkennzeichen) used in E-Rezept billing.
/// Defines how quantities or doses are measured/calculated.
/// </summary>
[Table("factor_codes", Schema = "ta1_reference")]
public class FactorCode
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 2-character factor code (e.g., "11", "55", "57", "99")
    /// </summary>
    [Required]
    [StringLength(2)]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Content/meaning of the factor code (German)
    /// </summary>
    [Required]
    [Column("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description (English)
    /// </summary>
    [Required]
    [Column("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Use case / application scenarios
    /// </summary>
    [Required]
    [Column("use_case")]
    public string UseCase { get; set; } = string.Empty;

    /// <summary>
    /// Record creation timestamp
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
