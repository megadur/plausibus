using ErezeptValidator.Models.Validation;
using ErezeptValidator.Services.CodeLookup;
using ErezeptValidator.Services.Validation.Helpers;
using Hl7.Fhir.Model;

namespace ErezeptValidator.Services.Validation.Validators;

/// <summary>
/// Validator for Abgabedaten bundles (GEN-001 to GEN-008)
/// Only executes when bundle type is Abgabedaten
/// </summary>
public class FhirAbgabedatenValidator : IValidator
{
    private readonly ICodeLookupService _codeLookupService;
    private readonly ILogger<FhirAbgabedatenValidator> _logger;

    public string Name => "FHIR Abgabedaten Validator";

    public FhirAbgabedatenValidator(
        ICodeLookupService codeLookupService,
        ILogger<FhirAbgabedatenValidator> logger)
    {
        _codeLookupService = codeLookupService;
        _logger = logger;
    }

    public async System.Threading.Tasks.Task<ValidationResult> ValidateAsync(ValidationContext context)
    {
        var result = ValidationResult.Success(Name);

        // Only validate Abgabedaten bundles
        if (context.BundleType != BundleType.Abgabedaten)
        {
            _logger.LogDebug("Skipping Abgabedaten validation - bundle type is {BundleType}", context.BundleType);
            return result;
        }

        _logger.LogInformation("Validating Abgabedaten bundle with {InvoiceCount} invoices", context.Invoices.Count);

        // GEN-002: Dispensing date validation
        ValidateDispensingDate(context, result);

        // Validate each Invoice line item
        foreach (var invoice in context.Invoices)
        {
            foreach (var lineItem in invoice.LineItem ?? Enumerable.Empty<Invoice.LineItemComponent>())
            {
                var lineNumber = lineItem.Sequence ?? 0;

                await ValidateLineItemAsync(context, lineItem, lineNumber, result);
            }
        }

        return result;
    }

    /// <summary>
    /// Validate individual Invoice line item
    /// </summary>
    private async System.Threading.Tasks.Task ValidateLineItemAsync(
        ValidationContext context,
        Invoice.LineItemComponent lineItem,
        int lineNumber,
        ValidationResult result)
    {
        var pzn = FhirDataExtractor.ExtractPznFromLineItem(lineItem);
        var sok = FhirDataExtractor.ExtractSokCode(lineItem);

        // GEN-001: PZN or SOK required (mutually exclusive)
        ValidatePznOrSokRequired(pzn, sok, lineNumber, result);

        // GEN-003 & GEN-004: Factor and price code validation
        var (factorCode, factorValue) = FhirDataExtractor.ExtractFactor(lineItem);
        var (priceCode, priceAmount) = FhirDataExtractor.ExtractPrice(lineItem);

        if (!string.IsNullOrEmpty(factorCode))
        {
            await ValidateFactorCodeAsync(factorCode, lineNumber, result);
        }

        if (!string.IsNullOrEmpty(priceCode))
        {
            await ValidatePriceCodeAsync(priceCode, lineNumber, result);
        }

        // GEN-005: Factor/price consistency
        ValidateFactorPriceConsistency(factorCode, factorValue, priceCode, priceAmount, lineNumber, result);

        // GEN-006, GEN-007, GEN-008: SOK validations
        if (!string.IsNullOrEmpty(sok))
        {
            await ValidateSokAsync(context, sok, lineItem, lineNumber, result);
        }
    }

    /// <summary>
    /// GEN-001: PZN or SOK required (mutually exclusive)
    /// </summary>
    private void ValidatePznOrSokRequired(string? pzn, string? sok, int lineNumber, ValidationResult result)
    {
        var hasPzn = !string.IsNullOrWhiteSpace(pzn);
        var hasSok = !string.IsNullOrWhiteSpace(sok);

        if (!hasPzn && !hasSok)
        {
            result.AddError("GEN-001-E",
                $"Line {lineNumber}: Either PZN or SOK code must be provided",
                "chargeItem");
        }
        else if (hasPzn && hasSok)
        {
            result.AddError("GEN-001-E",
                $"Line {lineNumber}: PZN and SOK are mutually exclusive. Found both: PZN={pzn}, SOK={sok}",
                "chargeItem");
        }
    }

    /// <summary>
    /// GEN-002: Dispensing date must not be in the future
    /// </summary>
    private void ValidateDispensingDate(ValidationContext context, ValidationResult result)
    {
        if (context.DispensingDate == null)
        {
            result.AddWarning("GEN-002-W",
                "Dispensing date not found in bundle. Unable to validate temporal constraints.",
                "MedicationDispense.whenHandedOver");
            return;
        }

        var now = DateTimeOffset.UtcNow;
        if (context.DispensingDate > now)
        {
            result.AddError("GEN-002-E",
                $"Dispensing date cannot be in the future. Got: {context.DispensingDate:yyyy-MM-dd HH:mm}, Current: {now:yyyy-MM-dd HH:mm} UTC",
                "MedicationDispense.whenHandedOver");
        }
    }

    /// <summary>
    /// GEN-003: Factor code must be valid
    /// </summary>
    private async System.Threading.Tasks.Task ValidateFactorCodeAsync(string factorCode, int lineNumber, ValidationResult result)
    {
        var factor = await _codeLookupService.GetFactorCodeAsync(factorCode);

        if (factor == null)
        {
            result.AddError("GEN-003-E",
                $"Line {lineNumber}: Factor code '{factorCode}' is not valid according to TA1 reference data",
                "priceComponent.factor");
        }
        else
        {
            _logger.LogDebug("Line {LineNumber}: Factor code {Code} validated: {Description}",
                lineNumber, factorCode, factor.Description);
        }
    }

    /// <summary>
    /// GEN-004: Price code must be valid
    /// </summary>
    private async System.Threading.Tasks.Task ValidatePriceCodeAsync(string priceCode, int lineNumber, ValidationResult result)
    {
        var price = await _codeLookupService.GetPriceCodeAsync(priceCode);

        if (price == null)
        {
            result.AddError("GEN-004-E",
                $"Line {lineNumber}: Price code '{priceCode}' is not valid according to TA1 reference data",
                "priceComponent.type");
        }
        else
        {
            _logger.LogDebug("Line {LineNumber}: Price code {Code} validated: {Description}",
                lineNumber, priceCode, price.Description);
        }
    }

    /// <summary>
    /// GEN-005: Factor/price code consistency
    /// </summary>
    private void ValidateFactorPriceConsistency(
        string? factorCode,
        decimal? factorValue,
        string? priceCode,
        decimal? priceAmount,
        int lineNumber,
        ValidationResult result)
    {
        var hasFactorCode = !string.IsNullOrWhiteSpace(factorCode);
        var hasFactorValue = factorValue.HasValue;
        var hasPriceCode = !string.IsNullOrWhiteSpace(priceCode);
        var hasPriceAmount = priceAmount.HasValue;

        // If factor code is provided, factor value must be provided
        if (hasFactorCode && !hasFactorValue)
        {
            result.AddError("GEN-005-E",
                $"Line {lineNumber}: Factor code '{factorCode}' provided but factor value is missing",
                "priceComponent.factor");
        }

        // If price code is provided, price amount must be provided
        if (hasPriceCode && !hasPriceAmount)
        {
            result.AddError("GEN-005-E",
                $"Line {lineNumber}: Price code '{priceCode}' provided but price amount is missing",
                "priceComponent.amount");
        }

        // If factor value is provided without code
        if (!hasFactorCode && hasFactorValue)
        {
            result.AddWarning("GEN-005-W",
                $"Line {lineNumber}: Factor value {factorValue} provided but factor code is missing",
                "priceComponent.factor");
        }

        // If price amount is provided without code
        if (!hasPriceCode && hasPriceAmount)
        {
            result.AddWarning("GEN-005-W",
                $"Line {lineNumber}: Price amount {priceAmount} provided but price code is missing",
                "priceComponent.type");
        }
    }

    /// <summary>
    /// Validate SOK code (GEN-006, GEN-007, GEN-008)
    /// </summary>
    private async System.Threading.Tasks.Task ValidateSokAsync(
        ValidationContext context,
        string sok,
        Invoice.LineItemComponent lineItem,
        int lineNumber,
        ValidationResult result)
    {
        var sokCode = await _codeLookupService.GetSpecialCodeAsync(sok);

        if (sokCode == null)
        {
            result.AddError("GEN-006-E",
                $"Line {lineNumber}: SOK code '{sok}' not found in TA1 reference database",
                "chargeItem");
            return;
        }

        // GEN-006: SOK temporal validation
        if (context.DispensingDate.HasValue)
        {
            var dispensingDate = DateOnly.FromDateTime(context.DispensingDate.Value.DateTime);
            var isValid = await _codeLookupService.ValidateSokTemporalAsync(sok, dispensingDate);

            if (!isValid)
            {
                if (sokCode.ValidFromDispensingDate.HasValue && dispensingDate < sokCode.ValidFromDispensingDate.Value)
                {
                    result.AddError("GEN-006-E",
                        $"Line {lineNumber}: SOK code '{sok}' is not yet valid on {dispensingDate:yyyy-MM-dd}. Valid from: {sokCode.ValidFromDispensingDate:yyyy-MM-dd}",
                        "chargeItem");
                }
                else if (sokCode.ExpiredDispensingDate.HasValue && dispensingDate > sokCode.ExpiredDispensingDate.Value)
                {
                    result.AddError("GEN-006-E",
                        $"Line {lineNumber}: SOK code '{sok}' expired on {sokCode.ExpiredDispensingDate:yyyy-MM-dd}. Dispensing date: {dispensingDate:yyyy-MM-dd}",
                        "chargeItem");
                }
            }
        }

        // GEN-007: E-Rezept SOK compatibility
        // For now, assume all bundles are E-Rezept (can be enhanced later)
        var isErezept = true;
        var isCompatible = await _codeLookupService.ValidateSokErezeptCompatibilityAsync(sok, isErezept);

        if (!isCompatible)
        {
            if (isErezept && sokCode.ERezept == 0)
            {
                result.AddError("GEN-007-E",
                    $"Line {lineNumber}: SOK code '{sok}' is not compatible with E-Rezept. This is a paper prescription-only SOK code.",
                    "chargeItem");
            }
            else if (!isErezept && sokCode.ERezept == 2)
            {
                result.AddError("GEN-007-E",
                    $"Line {lineNumber}: SOK code '{sok}' is mandatory for E-Rezept and cannot be used on paper prescriptions",
                    "chargeItem");
            }
        }

        // GEN-008: VAT rate consistency
        var vatRate = FhirDataExtractor.ExtractVatRate(lineItem);
        if (vatRate.HasValue)
        {
            var isVatConsistent = await _codeLookupService.ValidateSokVatRateAsync(sok, (short)vatRate.Value);

            if (!isVatConsistent && sokCode.VatRate.HasValue)
            {
                var expectedVatPercentage = sokCode.GetVatPercentage();

                result.AddError("GEN-008-E",
                    $"Line {lineNumber}: VAT rate mismatch for SOK code '{sok}'. Expected: {expectedVatPercentage}%, Got: {vatRate}%",
                    "priceComponent.extension[MwStSatz]");
            }
        }
    }
}
