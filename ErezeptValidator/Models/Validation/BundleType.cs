namespace ErezeptValidator.Models.Validation;

/// <summary>
/// Type of FHIR bundle being validated
/// </summary>
public enum BundleType
{
    /// <summary>
    /// Unknown or unsupported bundle type
    /// </summary>
    Unknown,

    /// <summary>
    /// Prescription bundle (VerordnungArzt) - Contains MedicationRequest resources
    /// </summary>
    Prescription,

    /// <summary>
    /// Dispensing/billing bundle (eAbgabedaten) - Contains Invoice and MedicationDispense resources
    /// </summary>
    Abgabedaten
}
