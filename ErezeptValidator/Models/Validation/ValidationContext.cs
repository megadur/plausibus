using Hl7.Fhir.Model;

namespace ErezeptValidator.Models.Validation;

/// <summary>
/// Context passed to validators containing the FHIR Bundle and extracted data
/// </summary>
public class ValidationContext
{
    /// <summary>
    /// The FHIR Bundle being validated
    /// </summary>
    public required Bundle Bundle { get; set; }

    /// <summary>
    /// Detected bundle type (Prescription or Abgabedaten)
    /// </summary>
    public BundleType BundleType { get; set; } = BundleType.Unknown;

    // Prescription-specific resources

    /// <summary>
    /// MedicationRequests found in the bundle (Prescription bundles)
    /// </summary>
    public List<MedicationRequest> MedicationRequests { get; set; } = new();

    /// <summary>
    /// Medications referenced by MedicationRequests
    /// </summary>
    public Dictionary<string, Medication> Medications { get; set; } = new();

    // Abgabedaten-specific resources

    /// <summary>
    /// MedicationDispenses found in the bundle (Abgabedaten bundles)
    /// </summary>
    public List<MedicationDispense> MedicationDispenses { get; set; } = new();

    /// <summary>
    /// Invoices found in the bundle (Abgabedaten bundles)
    /// </summary>
    public List<Invoice> Invoices { get; set; } = new();

    /// <summary>
    /// Dispensing date extracted from the bundle (Abgabedaten bundles)
    /// </summary>
    public DateTimeOffset? DispensingDate { get; set; }

    // Common properties

    /// <summary>
    /// PZN codes extracted from medications
    /// </summary>
    public List<string> PznCodes { get; set; } = new();

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}
