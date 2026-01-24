using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErezeptValidator.Models.Ta1Reference;

/// <summary>
/// Special identifier code (Sonderkennzeichen) for E-Rezept billing.
/// Represents both SOK1 (standard federal codes) and SOK2 (contract-specific codes).
/// </summary>
[Table("special_codes", Schema = "ta1_reference")]
public class SpecialCode
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 8-digit special code (e.g., "09999005", "02567018")
    /// </summary>
    [Required]
    [StringLength(8)]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description of the code
    /// </summary>
    [Required]
    [Column("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Code type: "SOK1" (standard federal) or "SOK2" (contract-specific)
    /// </summary>
    [Required]
    [StringLength(10)]
    [Column("code_type")]
    public string CodeType { get; set; } = string.Empty;

    /// <summary>
    /// VAT rate code: 0=0%, 1=7%, 2=19%, NULL=N/A
    /// </summary>
    [Column("vat_rate")]
    public short? VatRate { get; set; }

    /// <summary>
    /// E-Rezept compatibility: 0=not compatible, 1=compatible, 2=special handling required
    /// </summary>
    [Column("e_rezept")]
    public short ERezept { get; set; }

    /// <summary>
    /// Description of applicable pharmacy discount rules (e.g., "§ 130 Abs. 1/1a SGB V")
    /// </summary>
    [Column("pharmacy_discount")]
    public string? PharmacyDiscount { get; set; }

    /// <summary>
    /// Whether additional data is required for this code (Zusatzdaten field)
    /// </summary>
    [Column("requires_additional_data")]
    public bool RequiresAdditionalData { get; set; }

    /// <summary>
    /// Valid from billing month (format: YYYY-MM, e.g., "2024-01")
    /// </summary>
    [StringLength(7)]
    [Column("valid_from_billing_month")]
    public string? ValidFromBillingMonth { get; set; }

    /// <summary>
    /// Valid from dispensing date
    /// </summary>
    [Column("valid_from_dispensing_date")]
    public DateOnly? ValidFromDispensingDate { get; set; }

    /// <summary>
    /// Expired billing month (format: YYYY-MM, e.g., "2024-12")
    /// </summary>
    [StringLength(7)]
    [Column("expired_billing_month")]
    public string? ExpiredBillingMonth { get; set; }

    /// <summary>
    /// Expired dispensing date
    /// </summary>
    [Column("expired_dispensing_date")]
    public DateOnly? ExpiredDispensingDate { get; set; }

    /// <summary>
    /// Category grouping (e.g., "medication", "compounding", "cannabis", "fee", "service")
    /// </summary>
    [StringLength(50)]
    [Column("category")]
    public string? Category { get; set; }

    /// <summary>
    /// For SOK2: which organization/association the code is assigned to
    /// </summary>
    [StringLength(100)]
    [Column("issued_to")]
    public string? IssuedTo { get; set; }

    /// <summary>
    /// For SOK2: when the code was issued
    /// </summary>
    [Column("issued_date")]
    public DateOnly? IssuedDate { get; set; }

    /// <summary>
    /// Additional notes or special instructions
    /// </summary>
    [Column("notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// Record creation timestamp
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Record last update timestamp
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Check if the code is currently valid based on a given dispensing date
    /// </summary>
    /// <param name="dispensingDate">Date to check validity against</param>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValidOnDate(DateOnly dispensingDate)
    {
        // Check valid_from
        if (ValidFromDispensingDate.HasValue && dispensingDate < ValidFromDispensingDate.Value)
        {
            return false;
        }

        // Check expired
        if (ExpiredDispensingDate.HasValue && dispensingDate > ExpiredDispensingDate.Value)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Check if the code is compatible with E-Rezept
    /// </summary>
    public bool IsErezeptCompatible => ERezept != 0;

    /// <summary>
    /// Get VAT percentage from code
    /// </summary>
    public decimal? GetVatPercentage()
    {
        return VatRate switch
        {
            0 => 0m,
            1 => 7m,
            2 => 19m,
            _ => null
        };
    }
}
