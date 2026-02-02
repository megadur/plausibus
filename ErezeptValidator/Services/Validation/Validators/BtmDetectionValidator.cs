using ErezeptValidator.Data;
using ErezeptValidator.Models.Abdata;
using ErezeptValidator.Models.Validation;
using ErezeptValidator.Services.Validation.Helpers;
using Hl7.Fhir.Model;

namespace ErezeptValidator.Services.Validation.Validators;

/// <summary>
/// Validates BTM (Betäubungsmittel/controlled substances) prescriptions according to BtMG and TA1 rules
/// Implements BTM-001 to BTM-004 validation rules
/// </summary>
public class BtmDetectionValidator : IValidator
{
    private readonly IPznRepository _pznRepository;
    private readonly ILogger<BtmDetectionValidator> _logger;

    // BTM fee SOK code (TA1 Anhang 1/2)
    private const string BTM_FEE_SOK_CODE = "02567001";

    public string Name => "BTM Validator";

    public BtmDetectionValidator(
        IPznRepository pznRepository,
        ILogger<BtmDetectionValidator> logger)
    {
        _pznRepository = pznRepository;
        _logger = logger;
    }

    public async Task<ValidationResult> ValidateAsync(ValidationContext context)
    {
        var result = ValidationResult.Success(Name);

        // Detect BTM medications and store in context
        var btmPzns = await DetectBtmMedicationsAsync(context, result);

        // Only run BTM validation if BTM medications are present
        if (btmPzns.Count == 0)
        {
            _logger.LogDebug("No BTM medications detected, skipping BTM validation");
            return result;
        }

        _logger.LogInformation("Validating {Count} BTM medications", btmPzns.Count);

        // BTM-001: E-BTM Fee Special Code (if applicable)
        ValidateBtmFeeSpecialCode(context, btmPzns, result);

        // BTM-002: All Pharmaceuticals Must Be Listed
        ValidateAllPharmaceuticalsListed(context, btmPzns, result);

        // BTM-003: BTM Seven-Day Validity Rule
        ValidateSevenDayValidity(context, result);

        // BTM-004: BTM Diagnosis Requirement
        ValidateDiagnosisRequirement(context, result);

        return result;
    }

    /// <summary>
    /// Detect BTM medications via ABDATA and store flags in context
    /// </summary>
    private async Task<HashSet<string>> DetectBtmMedicationsAsync(ValidationContext context, ValidationResult result)
    {
        var btmPzns = new HashSet<string>();

        // Batch lookup for all PZNs
        if (context.PznCodes.Count > 0)
        {
            var articles = await _pznRepository.GetByPznBatchAsync(context.PznCodes.ToArray());

            foreach (var pzn in context.PznCodes)
            {
                if (articles.TryGetValue(pzn, out var article))
                {
                    // Check BTM status (field 08)
                    // 0 = keine Angabe, 1 = nein, 2 = BTM, 3 = Exempt preparation, 4 = T-Rezept
                    if (article.Btm == 2)
                    {
                        btmPzns.Add(pzn);
                        result.AddInfo("BTM-DETECTED",
                            $"BTM medication detected: {pzn} - {article.Name}",
                            "PZN", pzn);

                        // Store BTM flag for other validators
                        context.Metadata[$"btm_{pzn}"] = true;
                        context.Metadata[$"article_{pzn}"] = article;
                    }
                    else if (article.Btm == 3)
                    {
                        result.AddInfo("BTM-EXEMPT",
                            $"BTM exempt preparation detected: {pzn} - {article.Name}",
                            "PZN", pzn);
                    }
                    else if (article.Btm == 4)
                    {
                        result.AddInfo("T-REZEPT-DETECTED",
                            $"T-Rezept medication detected: {pzn} - {article.Name}",
                            "PZN", pzn);

                        context.Metadata[$"trezept_{pzn}"] = true;
                        context.Metadata[$"article_{pzn}"] = article;
                    }
                }
            }
        }

        return btmPzns;
    }

    /// <summary>
    /// BTM-001: E-BTM Fee Special Code
    /// Validates that BTM prescriptions include the E-BTM fee special code
    /// </summary>
    private void ValidateBtmFeeSpecialCode(ValidationContext context, HashSet<string> btmPzns, ValidationResult result)
    {
        // Only applies to Abgabedaten bundles
        if (context.BundleType != BundleType.Abgabedaten)
        {
            return;
        }

        // Check if E-BTM fee SOK code is present in Invoice line items
        bool hasBtmFee = false;
        int btmFeeCount = 0;

        foreach (var invoice in context.Invoices)
        {
            foreach (var lineItem in invoice.LineItem ?? Enumerable.Empty<Invoice.LineItemComponent>())
            {
                var sok = FhirDataExtractor.ExtractSokCode(lineItem);
                if (sok == BTM_FEE_SOK_CODE)
                {
                    hasBtmFee = true;
                    btmFeeCount++;

                    // BTM-001: Validate factor = count of BTM lines
                    var (_, factorValue) = FhirDataExtractor.ExtractFactor(lineItem);
                    if (factorValue.HasValue)
                    {
                        int expectedFactor = btmPzns.Count;
                        if (Math.Abs(factorValue.Value - expectedFactor) > 0.001m)
                        {
                            result.AddError("BTM-001-E",
                                $"BTM fee factor incorrect. Expected factor {expectedFactor} (count of BTM line items), " +
                                $"found {factorValue.Value}. Each BTM medication requires one fee unit.",
                                "Invoice.LineItem.Factor");
                        }
                    }
                }
            }
        }

        // Note: E-BTM fee is only required when E-BTM system is fully implemented
        // Currently adding as INFO, can be upgraded to ERROR when E-BTM is mandatory
        if (!hasBtmFee && btmPzns.Count > 0)
        {
            result.AddInfo("BTM-001-I",
                $"BTM prescription with {btmPzns.Count} controlled substance(s) detected. " +
                $"E-BTM fee special code ({BTM_FEE_SOK_CODE}) should be included when E-BTM system is active.",
                "Invoice.LineItem");

            _logger.LogInformation(
                "BTM prescription without E-BTM fee code - {Count} BTM medications found",
                btmPzns.Count);
        }
    }

    /// <summary>
    /// BTM-002: All Pharmaceuticals Must Be Listed
    /// Validates that all BTM line items have complete information (PZN, quantity, price)
    /// </summary>
    private void ValidateAllPharmaceuticalsListed(ValidationContext context, HashSet<string> btmPzns, ValidationResult result)
    {
        // Only applies to Abgabedaten bundles
        if (context.BundleType != BundleType.Abgabedaten)
        {
            return;
        }

        foreach (var invoice in context.Invoices)
        {
            int lineNumber = 0;
            foreach (var lineItem in invoice.LineItem ?? Enumerable.Empty<Invoice.LineItemComponent>())
            {
                lineNumber++;
                var pzn = FhirDataExtractor.ExtractPznFromLineItem(lineItem);

                // Check if this line item contains a BTM medication
                if (!string.IsNullOrEmpty(pzn) && btmPzns.Contains(pzn))
                {
                    var errors = new List<string>();

                    // Validate PZN present (already checked above, but keeping for completeness)
                    if (string.IsNullOrEmpty(pzn))
                    {
                        errors.Add("PZN missing");
                    }

                    // Validate quantity present
                    var dispensedQty = FhirDataExtractor.ExtractDispensedQuantity(lineItem);
                    if (!dispensedQty.HasValue || dispensedQty.Value <= 0)
                    {
                        errors.Add("Quantity (Menge) missing or invalid");
                    }

                    // Validate price present
                    var (_, priceAmount) = FhirDataExtractor.ExtractPrice(lineItem);
                    if (!priceAmount.HasValue || priceAmount.Value <= 0)
                    {
                        errors.Add("Price (Bruttopreis) missing or invalid");
                    }

                    if (errors.Count > 0)
                    {
                        result.AddError("BTM-002-E",
                            $"BTM medication at line {lineNumber} (PZN {pzn}) has incomplete information. " +
                            $"All controlled substances must be fully documented: {string.Join(", ", errors)}. " +
                            $"Reference: BtMG requirements for controlled substance documentation.",
                            $"Invoice.LineItem[{lineNumber}]");

                        _logger.LogWarning(
                            "BTM-002 validation failed for line {LineNumber} PZN {Pzn}: {Errors}",
                            lineNumber, pzn, string.Join(", ", errors));
                    }
                }
            }
        }
    }

    /// <summary>
    /// BTM-003: BTM Seven-Day Validity Rule
    /// Warning if BTM prescription dispensed more than 7 days after issuance
    /// </summary>
    private void ValidateSevenDayValidity(ValidationContext context, ValidationResult result)
    {
        // Extract prescription date (authoredOn from MedicationRequest)
        DateTimeOffset? prescriptionDate = null;
        foreach (var medRequest in context.MedicationRequests)
        {
            if (!string.IsNullOrEmpty(medRequest.AuthoredOn))
            {
                if (DateTimeOffset.TryParse(medRequest.AuthoredOn, out var date))
                {
                    prescriptionDate = date;
                    break;
                }
            }
        }

        // Extract dispensing date
        var dispensingDate = context.DispensingDate;

        if (prescriptionDate.HasValue && dispensingDate.HasValue)
        {
            var daysDifference = (dispensingDate.Value - prescriptionDate.Value).TotalDays;

            if (daysDifference > 7)
            {
                result.AddWarning("BTM-003-W",
                    $"BTM prescription may be expired. Dispensed {daysDifference:F1} days after prescription date. " +
                    $"BTM prescriptions are valid for 7 days per BtMG §3. " +
                    $"Prescription date: {prescriptionDate.Value:yyyy-MM-dd}, " +
                    $"Dispensing date: {dispensingDate.Value:yyyy-MM-dd}",
                    "MedicationRequest.AuthoredOn");

                _logger.LogWarning(
                    "BTM-003: Potential expired prescription - {Days} days between prescription and dispensing",
                    daysDifference);
            }
            else if (daysDifference > 5)
            {
                // Info message when approaching 7-day limit
                result.AddInfo("BTM-003-I",
                    $"BTM prescription dispensed {daysDifference:F1} days after issuance (approaching 7-day limit).",
                    "MedicationRequest.AuthoredOn");
            }
        }
        else
        {
            _logger.LogDebug(
                "Cannot validate BTM-003: Prescription date {PrescriptionDate}, Dispensing date {DispensingDate}",
                prescriptionDate, dispensingDate);
        }
    }

    /// <summary>
    /// BTM-004: BTM Diagnosis Requirement
    /// Warning if BTM prescription missing diagnosis code (ICD-10)
    /// </summary>
    private void ValidateDiagnosisRequirement(ValidationContext context, ValidationResult result)
    {
        // Look for diagnosis information in the bundle
        bool hasDiagnosis = false;

        // Check for Condition resources (FHIR standard for diagnosis)
        var conditionResources = context.Bundle.Entry?
            .Where(e => e.Resource is Condition)
            .Select(e => e.Resource as Condition)
            .Where(c => c != null)
            .Cast<Condition>()
            .ToList() ?? new List<Condition>();

        if (conditionResources.Count > 0)
        {
            foreach (var condition in conditionResources)
            {
                if (condition?.Code?.Coding?.Any(c => c.System?.Contains("icd") == true) == true)
                {
                    hasDiagnosis = true;
                    var icdCode = condition.Code.Coding.First(c => c.System?.Contains("icd") == true).Code;
                    result.AddInfo("BTM-004-I",
                        $"BTM prescription includes diagnosis code: {icdCode}",
                        "Condition.Code");
                    break;
                }
            }
        }

        // Check for diagnosis in extensions (alternative encoding)
        foreach (var medRequest in context.MedicationRequests)
        {
            var diagnosisExtension = medRequest.Extension?
                .FirstOrDefault(e => e.Url?.Contains("diagnosis") == true ||
                                   e.Url?.Contains("Diagnose") == true);

            if (diagnosisExtension != null)
            {
                hasDiagnosis = true;
                break;
            }
        }

        if (!hasDiagnosis)
        {
            result.AddWarning("BTM-004-W",
                "BTM prescription does not include diagnosis information (ICD-10 code). " +
                "BTM prescriptions should include diagnosis code per BtMG §3 for documentation purposes.",
                "Condition.Code");

            _logger.LogWarning("BTM-004: BTM prescription missing diagnosis code");
        }
    }
}
