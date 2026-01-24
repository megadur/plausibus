using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErezeptValidator.Models.Ta1Reference;

/// <summary>
/// Price identifier code (Preiskennzeichen) used in E-Rezept billing.
/// Defines the type of price being reported (purchase price, billing price, contract price, etc.).
/// </summary>
[Table("price_codes", Schema = "ta1_reference")]
public class PriceCode
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 2-character price code (e.g., "11", "12", "13", "14", "15", "16", "17", "21", "90")
    /// </summary>
    [Required]
    [StringLength(2)]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Content/meaning of the price code (German)
    /// </summary>
    [Required]
    [Column("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Tax status (e.g., "excl. VAT", "incl. VAT")
    /// </summary>
    [Required]
    [StringLength(20)]
    [Column("tax_status")]
    public string TaxStatus { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description (English)
    /// </summary>
    [Required]
    [Column("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Record creation timestamp
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
