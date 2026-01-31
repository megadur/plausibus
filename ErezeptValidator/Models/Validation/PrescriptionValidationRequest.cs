using System.ComponentModel.DataAnnotations;

namespace ErezeptValidator.Models.Validation;

/// <summary>
/// Request model for prescription validation
/// </summary>
public class PrescriptionValidationRequest
{
    /// <summary>
    /// Prescription identifier (e.g., Task ID from FHIR)
    /// Example: "Task/160.000.000.012.345.67"
    /// </summary>
    [Required]
    public string PrescriptionId { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the medication was dispensed
    /// Used for temporal SOK validation and date-based rules
    /// </summary>
    [Required]
    public DateTime DispensingDate { get; set; }

    /// <summary>
    /// Whether this is an E-Rezept (electronic prescription)
    /// Affects SOK compatibility validation (E-Rezept field)
    /// </summary>
    public bool IsErezept { get; set; } = true;

    /// <summary>
    /// List of line items in the prescription
    /// Each line item represents a medication or service
    /// </summary>
    [Required]
    [MinLength(1, ErrorMessage = "At least one line item is required")]
    public List<PrescriptionLineItem> LineItems { get; set; } = new();

    /// <summary>
    /// Optional pharmacy identifier for logging/tracking
    /// </summary>
    public string? PharmacyId { get; set; }

    /// <summary>
    /// Optional prescriber identifier for logging/tracking
    /// </summary>
    public string? PrescriberId { get; set; }
}
