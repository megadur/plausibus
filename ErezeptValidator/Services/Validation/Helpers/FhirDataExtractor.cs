using Hl7.Fhir.Model;

namespace ErezeptValidator.Services.Validation.Helpers;

/// <summary>
/// Helper class to extract validation-relevant data from FHIR bundles
/// </summary>
public static class FhirDataExtractor
{
    /// <summary>
    /// Extract PZN code from Medication resource
    /// </summary>
    public static string? ExtractPzn(Medication medication)
    {
        return medication.Code?.Coding?
            .FirstOrDefault(c => c.System == "http://fhir.de/CodeSystem/ifa/pzn")
            ?.Code;
    }

    /// <summary>
    /// Extract medication name from Medication resource
    /// </summary>
    public static string? ExtractMedicationName(Medication medication)
    {
        return medication.Code?.Text;
    }

    /// <summary>
    /// Extract quantity from MedicationRequest
    /// </summary>
    public static decimal? ExtractQuantity(MedicationRequest medRequest)
    {
        return medRequest.DispenseRequest?.Quantity?.Value;
    }

    /// <summary>
    /// Extract authored date from MedicationRequest
    /// </summary>
    public static string? ExtractAuthoredOn(MedicationRequest medRequest)
    {
        return medRequest.AuthoredOn;
    }

    /// <summary>
    /// Extract dosage instruction text from MedicationRequest
    /// </summary>
    public static string? ExtractDosageText(MedicationRequest medRequest)
    {
        return medRequest.DosageInstruction?.FirstOrDefault()?.Text;
    }

    /// <summary>
    /// Find referenced Medication resource in bundle
    /// </summary>
    public static Medication? FindReferencedMedication(
        MedicationRequest medRequest,
        Bundle bundle)
    {
        if (medRequest.Medication is not ResourceReference medicationRef)
        {
            return null;
        }

        var reference = medicationRef.Reference;
        if (string.IsNullOrEmpty(reference))
        {
            return null;
        }

        // Try to find by FullUrl or by Resource ID
        return bundle.Entry?
            .FirstOrDefault(e =>
                e.FullUrl == reference ||
                (e.Resource is Medication med && (
                    med.Id == reference ||
                    $"Medication/{med.Id}" == reference
                )))
            ?.Resource as Medication;
    }

    /// <summary>
    /// Extract all MedicationRequests from bundle
    /// </summary>
    public static List<MedicationRequest> ExtractMedicationRequests(Bundle bundle)
    {
        return bundle.Entry?
            .Where(e => e.Resource is MedicationRequest)
            .Select(e => (MedicationRequest)e.Resource)
            .ToList() ?? new List<MedicationRequest>();
    }

    /// <summary>
    /// Extract all Medications from bundle and map by reference
    /// </summary>
    public static Dictionary<string, Medication> ExtractMedications(Bundle bundle)
    {
        var medications = new Dictionary<string, Medication>();

        foreach (var entry in bundle.Entry ?? Enumerable.Empty<Bundle.EntryComponent>())
        {
            if (entry.Resource is not Medication medication)
            {
                continue;
            }

            // Store by ID
            if (!string.IsNullOrEmpty(medication.Id))
            {
                medications[medication.Id] = medication;
                medications[$"Medication/{medication.Id}"] = medication;
            }

            // Store by FullUrl
            if (!string.IsNullOrEmpty(entry.FullUrl))
            {
                medications[entry.FullUrl] = medication;
            }
        }

        return medications;
    }

    /// <summary>
    /// Extract package size from Medication
    /// </summary>
    public static string? ExtractPackageSize(Medication medication)
    {
        // Look for KBV_EX_ERP_Medication_PackagingSize extension
        var extension = medication.Amount?.Numerator?.Extension?
            .FirstOrDefault(e => e.Url == "https://fhir.kbv.de/StructureDefinition/KBV_EX_ERP_Medication_PackagingSize");

        return (extension?.Value as FhirString)?.Value;
    }

    /// <summary>
    /// Extract norm size (N1, N2, N3) from Medication
    /// </summary>
    public static string? ExtractNormSize(Medication medication)
    {
        var extension = medication.Extension?
            .FirstOrDefault(e => e.Url == "http://fhir.de/StructureDefinition/normgroesse");

        return (extension?.Value as Code)?.Value;
    }

    /// <summary>
    /// Check if substitution is allowed
    /// </summary>
    public static bool IsSubstitutionAllowed(MedicationRequest medRequest)
    {
        if (medRequest.Substitution?.Allowed is FhirBoolean allowedBoolean)
        {
            return allowedBoolean.Value ?? true;
        }
        return true;
    }

    /// <summary>
    /// Extract bundle timestamp
    /// </summary>
    public static DateTimeOffset? ExtractBundleTimestamp(Bundle bundle)
    {
        if (bundle.TimestampElement?.Value != null)
        {
            return bundle.TimestampElement.Value;
        }
        return null;
    }

    /// <summary>
    /// Extract prescription ID from bundle identifier
    /// </summary>
    public static string? ExtractPrescriptionId(Bundle bundle)
    {
        return bundle.Identifier?.Value;
    }

    /// <summary>
    /// Extract SOK code from Invoice line item extensions
    /// </summary>
    public static string? ExtractSokCode(Invoice.LineItemComponent lineItem)
    {
        // SOK codes may be in extensions or coding systems
        // Check for SOK in chargeItem coding
        if (lineItem.ChargeItem is CodeableConcept chargeItemCode)
        {
            var sokCoding = chargeItemCode.Coding?
                .FirstOrDefault(c => c.System?.Contains("sok") == true ||
                                   c.System?.Contains("special") == true);
            if (sokCoding != null)
            {
                return sokCoding.Code;
            }
        }
        return null;
    }

    /// <summary>
    /// Extract factor code and value from Invoice line item
    /// </summary>
    public static (string? Code, decimal? Value) ExtractFactor(Invoice.LineItemComponent lineItem)
    {
        // Factor is in priceComponent
        var priceComponent = lineItem.PriceComponent?.FirstOrDefault();
        if (priceComponent == null)
        {
            return (null, null);
        }

        // Factor value
        var factorValue = priceComponent.Factor;

        // Factor code may be in extensions
        string? factorCode = null;
        var factorExtension = priceComponent.Extension?
            .FirstOrDefault(e => e.Url?.Contains("Factor") == true ||
                               e.Url?.Contains("factor") == true);

        if (factorExtension?.Value is Code codeValue)
        {
            factorCode = codeValue.Value;
        }

        return (factorCode, factorValue);
    }

    /// <summary>
    /// Extract price code and amount from Invoice line item
    /// </summary>
    public static (string? Code, decimal? Amount) ExtractPrice(Invoice.LineItemComponent lineItem)
    {
        var priceComponent = lineItem.PriceComponent?.FirstOrDefault();
        if (priceComponent == null)
        {
            return (null, null);
        }

        // Price amount
        var amount = priceComponent.Amount?.Value;

        // Price code may be in type or extensions
        string? priceCode = priceComponent.Type?.ToString();

        return (priceCode, amount);
    }

    /// <summary>
    /// Extract VAT rate from Invoice line item
    /// </summary>
    public static decimal? ExtractVatRate(Invoice.LineItemComponent lineItem)
    {
        // VAT rate is in extension DAV-EX-ERP-MwStSatz
        var priceComponent = lineItem.PriceComponent?.FirstOrDefault();
        var vatExtension = priceComponent?.Extension?
            .FirstOrDefault(e => e.Url?.Contains("MwStSatz") == true);

        if (vatExtension?.Value is FhirDecimal vatDecimal)
        {
            return vatDecimal.Value;
        }

        return null;
    }

    /// <summary>
    /// Extract all Invoices from bundle
    /// </summary>
    public static List<Invoice> ExtractInvoices(Bundle bundle)
    {
        return bundle.Entry?
            .Where(e => e.Resource is Invoice)
            .Select(e => (Invoice)e.Resource)
            .ToList() ?? new List<Invoice>();
    }

    /// <summary>
    /// Extract PZN from Invoice line item
    /// </summary>
    public static string? ExtractPznFromLineItem(Invoice.LineItemComponent lineItem)
    {
        if (lineItem.ChargeItem is CodeableConcept chargeItemCode)
        {
            return chargeItemCode.Coding?
                .FirstOrDefault(c => c.System == "http://fhir.de/CodeSystem/ifa/pzn")
                ?.Code;
        }
        return null;
    }

    /// <summary>
    /// Extract dispensed quantity from Invoice line item
    /// </summary>
    public static decimal? ExtractDispensedQuantity(Invoice.LineItemComponent lineItem)
    {
        // Quantity may be in priceComponent or in extensions
        var priceComponent = lineItem.PriceComponent?.FirstOrDefault();
        if (priceComponent?.Factor != null)
        {
            // If factor exists, it might represent the quantity relationship
            // However, we need the actual dispensed quantity value
            // Check for quantity extensions
            var quantityExt = priceComponent.Extension?
                .FirstOrDefault(e => e.Url?.Contains("Quantity") == true ||
                                   e.Url?.Contains("quantity") == true);

            if (quantityExt?.Value is FhirDecimal qtyDecimal)
            {
                return qtyDecimal.Value;
            }
            else if (quantityExt?.Value is Quantity qtyValue)
            {
                return qtyValue.Value;
            }
        }

        return null;
    }

    /// <summary>
    /// Extract package quantity from Invoice line item or Medication
    /// </summary>
    public static decimal? ExtractPackageQuantity(Invoice.LineItemComponent lineItem, Medication? medication = null)
    {
        // Try to get from extensions first
        var packageExt = lineItem.Extension?
            .FirstOrDefault(e => e.Url?.Contains("Package") == true ||
                               e.Url?.Contains("package") == true);

        if (packageExt?.Value is FhirDecimal pkgDecimal)
        {
            return pkgDecimal.Value;
        }
        else if (packageExt?.Value is Quantity pkgQty)
        {
            return pkgQty.Value;
        }

        // If not in line item, try to get from Medication resource
        if (medication != null)
        {
            var packageSize = ExtractPackageSize(medication);
            if (decimal.TryParse(packageSize, out var size))
            {
                return size;
            }
        }

        return null;
    }

    /// <summary>
    /// Extract MedicationDispenses from bundle
    /// </summary>
    public static List<MedicationDispense> ExtractMedicationDispenses(Bundle bundle)
    {
        return bundle.Entry?
            .Where(e => e.Resource is MedicationDispense)
            .Select(e => (MedicationDispense)e.Resource)
            .ToList() ?? new List<MedicationDispense>();
    }

    /// <summary>
    /// Extract dispensing date from MedicationDispense
    /// </summary>
    public static DateTimeOffset? ExtractDispensingDate(List<MedicationDispense> medicationDispenses)
    {
        var dispense = medicationDispenses.FirstOrDefault();
        if (dispense?.WhenHandedOver != null)
        {
            return DateTimeOffset.Parse(dispense.WhenHandedOver);
        }
        return null;
    }
}
