using ErezeptValidator.Data;
using ErezeptValidator.Models.Validation;
using ErezeptValidator.Models.ValueObjects;
using ErezeptValidator.Services.CodeLookup;
using ErezeptValidator.Services.Validation.Helpers;

namespace ErezeptValidator.Services.Validation.Validators;

/// <summary>
/// Validator for calculation rules (CALC-001 to CALC-007)
/// Validates factors, prices, and special calculations for Abgabedaten bundles
/// Uses value objects for type safety and business logic encapsulation
/// </summary>
public class CalculationValidator : IValidator
{
    private readonly ICodeLookupService _codeLookupService;
    private readonly IPznRepository _pznRepository;
    private readonly ILogger<CalculationValidator> _logger;

    public string Name => "Calculation Validator";

    public CalculationValidator(
        ICodeLookupService codeLookupService,
        IPznRepository pznRepository,
        ILogger<CalculationValidator> logger)
    {
        _codeLookupService = codeLookupService;
        _pznRepository = pznRepository;
        _logger = logger;
    }

    public async System.Threading.Tasks.Task<ValidationResult> ValidateAsync(ValidationContext context)
    {
        var result = ValidationResult.Success(Name);

        // Only validate Abgabedaten bundles
        if (context.BundleType != BundleType.Abgabedaten)
        {
            _logger.LogDebug("Skipping calculation validation - bundle type is {BundleType}", context.BundleType);
            return result;
        }

        _logger.LogInformation("Validating calculations for {InvoiceCount} invoices", context.Invoices.Count);

        // Validate each Invoice line item
        foreach (var invoice in context.Invoices)
        {
            foreach (var lineItem in invoice.LineItem ?? Enumerable.Empty<Hl7.Fhir.Model.Invoice.LineItemComponent>())
            {
                var lineNumber = lineItem.Sequence ?? 0;
                await ValidateLineItemCalculationsAsync(lineItem, lineNumber, result);
            }
        }

        return result;
    }

    /// <summary>
    /// Validate calculations for a single Invoice line item
    /// </summary>
    private async System.Threading.Tasks.Task ValidateLineItemCalculationsAsync(
        Hl7.Fhir.Model.Invoice.LineItemComponent lineItem,
        int lineNumber,
        ValidationResult result)
    {
        // Extract as value objects
        var pznString = FhirDataExtractor.ExtractPznFromLineItem(lineItem);
        var sokString = FhirDataExtractor.ExtractSokCode(lineItem);
        var (_, factorValue) = FhirDataExtractor.ExtractFactor(lineItem);
        var (priceCodeString, priceAmount) = FhirDataExtractor.ExtractPrice(lineItem);

        // Try to create value objects
        Pzn.TryCreate(pznString, out var pzn);
        SokCode.TryCreate(sokString, out var sok);
        PromilleFactor.TryParse(factorValue, out var actualFactor);
        Money.TryParse(priceAmount, out var actualPrice);
        PriceIdentifier.TryCreate(priceCodeString, out var priceId);

        // CALC-003: Artificial insemination special code validation
        if (sok.IsArtificialInsemination)
        {
            ValidateArtificialInseminationCode(actualFactor, actualPrice, priceId, lineNumber, result);
            return; // Skip other validations for this special case
        }

        // CALC-002: Special codes without quantity reference
        if (sok.HasNoQuantityReference)
        {
            ValidateSpecialCodeFactor(sok, actualFactor, lineNumber, result);
            // Don't return - still validate price calculation if applicable
        }
        else
        {
            // CALC-001: Standard Promilleanteil formula validation (only for quantity-based)
            await ValidateStandardFactorCalculationAsync(lineItem, actualFactor, lineNumber, result);
        }

        // CALC-004: Basic price calculation validation
        if (!pzn.Equals(default(Pzn)) && !priceId.Equals(default(PriceIdentifier)) && priceId.UsesAbdataBasePrice)
        {
            await ValidatePriceCalculationAsync(pzn, actualFactor, priceId, actualPrice, lineNumber, result);
        }

        // CALC-005: VAT exclusion for compounding preparations
        if (!sok.Equals(default(SokCode)) && sok.IsCompounding)
        {
            await ValidateVatExclusionAsync(sok, priceId, lineNumber, result);
        }
    }

    /// <summary>
    /// CALC-001: Standard Promilleanteil (per mille) formula validation
    /// Formula: Factor = (Dispensed_Quantity / Package_Quantity) × 1000
    /// </summary>
    private async System.Threading.Tasks.Task ValidateStandardFactorCalculationAsync(
        Hl7.Fhir.Model.Invoice.LineItemComponent lineItem,
        PromilleFactor actualFactor,
        int lineNumber,
        ValidationResult result)
    {
        if (actualFactor.Equals(default(PromilleFactor)))
        {
            // No factor provided - this is acceptable for some line items
            return;
        }

        // Extract quantities as value objects
        var dispensedQty = FhirDataExtractor.ExtractDispensedQuantity(lineItem);
        var packageQty = FhirDataExtractor.ExtractPackageQuantity(lineItem);

        if (!Quantity.TryParse(dispensedQty, out var dispensed) ||
            !Quantity.TryParse(packageQty, out var package))
        {
            _logger.LogDebug(
                "Cannot validate factor calculation for line {LineNumber} - missing quantity information",
                lineNumber);
            return;
        }

        // Validate package quantity is not zero
        if (package.IsZero)
        {
            result.AddError(
                "CALC-001-E",
                $"Invalid package quantity (zero) for line item {lineNumber}. Cannot calculate factor.",
                $"LineItem[{lineNumber}].PackageQuantity");
            return;
        }

        // Calculate expected factor using value objects
        var expectedFactor = PromilleFactor.FromRatio(dispensed, package);

        // Compare with tolerance
        if (!actualFactor.EqualsWithinTolerance(expectedFactor))
        {
            result.AddError(
                "CALC-001-E",
                $"Factor (Promilleanteil) calculation incorrect for line item {lineNumber}. " +
                $"Expected: {expectedFactor}, Found: {actualFactor}. " +
                $"Calculation: ({dispensed} / {package}) × 1000 = {expectedFactor}",
                $"LineItem[{lineNumber}].Factor");
        }
    }

    /// <summary>
    /// CALC-002: Special codes without quantity reference must have factor 1.0
    /// </summary>
    private void ValidateSpecialCodeFactor(
        SokCode sokCode,
        PromilleFactor actualFactor,
        int lineNumber,
        ValidationResult result)
    {
        if (actualFactor.Equals(default(PromilleFactor)))
        {
            result.AddError(
                "CALC-002-E",
                $"Special code {sokCode} without quantity reference requires a factor. " +
                $"Expected: {PromilleFactor.One}, Found: (none)",
                $"LineItem[{lineNumber}].Factor");
            return;
        }

        var expectedFactor = PromilleFactor.One;

        if (!actualFactor.EqualsWithinTolerance(expectedFactor))
        {
            result.AddError(
                "CALC-002-E",
                $"Special code {sokCode} without quantity reference must have factor {expectedFactor}. " +
                $"Expected: {expectedFactor}, Found: {actualFactor}",
                $"LineItem[{lineNumber}].Factor");
        }
    }

    /// <summary>
    /// CALC-003: Artificial insemination special code (09999643) validation
    /// Must have: Factor = 1000.0, Price = 0.00 EUR, Price Identifier = "90"
    /// </summary>
    private void ValidateArtificialInseminationCode(
        PromilleFactor actualFactor,
        Money actualPrice,
        PriceIdentifier actualPriceId,
        int lineNumber,
        ValidationResult result)
    {
        var errors = new List<string>();

        // Expected values as value objects
        var expectedFactor = PromilleFactor.ArtificialInsemination;
        var expectedPrice = Money.Zero;
        var expectedPriceId = PriceIdentifier.ArtificialInsemination;

        // Validate Factor = 1000.0
        if (actualFactor.Equals(default(PromilleFactor)))
        {
            errors.Add($"Factor must be {expectedFactor} (found: none)");
        }
        else if (!actualFactor.EqualsWithinTolerance(expectedFactor))
        {
            errors.Add($"Factor must be {expectedFactor} (found: {actualFactor})");
        }

        // Validate Price = 0.00
        if (actualPrice.Equals(default(Money)))
        {
            errors.Add($"Price must be {expectedPrice} (found: none)");
        }
        else if (actualPrice != expectedPrice)
        {
            errors.Add($"Price must be {expectedPrice} (found: {actualPrice})");
        }

        // Validate Price Identifier = "90"
        if (actualPriceId.Equals(default(PriceIdentifier)))
        {
            errors.Add($"Price identifier must be '{expectedPriceId}' (found: none)");
        }
        else if (actualPriceId != expectedPriceId)
        {
            errors.Add($"Price identifier must be '{expectedPriceId}' (found: '{actualPriceId}')");
        }

        if (errors.Count > 0)
        {
            result.AddError(
                "CALC-003-E",
                $"Artificial insemination special code ({SokCode.ArtificialInsemination}) validation failed " +
                $"for line item {lineNumber}: {string.Join("; ", errors)}",
                $"LineItem[{lineNumber}].ArtificialInsemination");
        }
    }

    /// <summary>
    /// CALC-004: Basic price calculation validation
    /// Formula: Price = (Factor / 1000) × Base_Price_per_PriceIdentifier
    /// </summary>
    private async System.Threading.Tasks.Task ValidatePriceCalculationAsync(
        Pzn pzn,
        PromilleFactor factor,
        PriceIdentifier priceId,
        Money actualPrice,
        int lineNumber,
        ValidationResult result)
    {
        // Skip if missing required values
        if (factor.Equals(default(PromilleFactor)) || actualPrice.Equals(default(Money)))
        {
            _logger.LogDebug(
                "Skipping CALC-004 for line {LineNumber} - missing factor or price",
                lineNumber);
            return;
        }

        // Look up article in ABDATA to get base price
        var article = await _pznRepository.GetByPznAsync(pzn.ToString());
        if (article == null)
        {
            _logger.LogWarning(
                "Cannot validate price calculation for line {LineNumber} - PZN {Pzn} not found in ABDATA",
                lineNumber, pzn);
            return; // PZN validation happens elsewhere (PznExistsValidator)
        }

        // Get base price from ABDATA based on price identifier
        Money basePrice;
        string priceField = priceId.GetAbdataPriceField() ?? "";

        switch (priceField)
        {
            case "AEK":
                basePrice = Money.FromCents(article.ApoEk);
                break;
            case "AVK":
                basePrice = Money.FromCents(article.ApoVk);
                break;
            default:
                // Contracted prices (12, 15, 16, 17, 21) don't use ABDATA
                _logger.LogDebug(
                    "Skipping CALC-004 for line {LineNumber} - price identifier {PriceId} uses contracted price",
                    lineNumber, priceId);
                return;
        }

        // Calculate expected price: (Factor / 1000) × Base_Price
        var expectedPrice = CalculateExpectedPrice(factor, basePrice);

        // Compare with tolerance (0.01 EUR for rounding differences)
        var tolerance = Money.Euro(0.01m);
        var difference = Money.Abs(actualPrice - expectedPrice);

        if (difference > tolerance)
        {
            result.AddError(
                "CALC-004-E",
                $"Price calculation incorrect for line item {lineNumber}. " +
                $"Formula: ({factor} / 1000) × {basePrice} ({priceField} from ABDATA). " +
                $"Expected: {expectedPrice}, Found: {actualPrice}, " +
                $"Difference: {difference}",
                $"LineItem[{lineNumber}].Price");

            _logger.LogWarning(
                "CALC-004 failed for line {LineNumber}: Expected {Expected}, Found {Actual}",
                lineNumber, expectedPrice, actualPrice);
        }
        else
        {
            _logger.LogDebug(
                "CALC-004 passed for line {LineNumber}: {ActualPrice} matches expected {ExpectedPrice}",
                lineNumber, actualPrice, expectedPrice);
        }
    }

    /// <summary>
    /// Calculate expected price using formula: (Factor / 1000) × Base_Price
    /// </summary>
    private Money CalculateExpectedPrice(PromilleFactor factor, Money basePrice)
    {
        // Convert factor to decimal (divide by 1000 to get the multiplier)
        decimal factorAsDecimal = factor.ToDecimal();
        decimal multiplier = factorAsDecimal / 1000m;

        // Multiply base price by the multiplier
        var expectedPrice = basePrice * multiplier;

        // Round to 2 decimal places (EUR cent precision)
        return expectedPrice.Round();
    }

    /// <summary>
    /// CALC-005: VAT Exclusion in Price Field (basic check for compounding)
    /// Full implementation deferred to REZ-xxx validation rules
    /// </summary>
    private async System.Threading.Tasks.Task ValidateVatExclusionAsync(
        SokCode sok,
        PriceIdentifier priceId,
        int lineNumber,
        ValidationResult result)
    {
        // Only applies to compounding preparations
        if (!sok.IsCompounding)
        {
            return;
        }

        // For compounding, price identifier should use net price (excl. VAT)
        // Check if price identifier is documented as excluding VAT
        if (!priceId.Equals(default(PriceIdentifier)))
        {
            var priceCode = await _codeLookupService.GetPriceCodeAsync(priceId.ToString());

            if (priceCode != null && priceCode.TaxStatus.Contains("incl", StringComparison.OrdinalIgnoreCase))
            {
                result.AddError(
                    "CALC-005-E",
                    $"Compounding line item {lineNumber} uses price identifier '{priceId}' with tax status '{priceCode.TaxStatus}'. " +
                    $"Compounding prices must exclude VAT (tax status should be 'excl. VAT'). " +
                    $"Reference: TA1 Section 4.14.2",
                    $"LineItem[{lineNumber}].PriceIdentifier");

                _logger.LogWarning(
                    "CALC-005 failed for compounding line {LineNumber}: Price identifier {PriceId} has tax status '{TaxStatus}'",
                    lineNumber, priceId, priceCode.TaxStatus);
            }
        }

        // Note: Full VAT calculation validation will be implemented in REZ-xxx validation rules
        // which will have more context about expected compounding prices
    }
}
