using ErezeptValidator.Data;
using ErezeptValidator.Models.Validation;
using ErezeptValidator.Models.ValueObjects;
using ErezeptValidator.Services.CodeLookup;
using ErezeptValidator.Services.Validation.Helpers;

namespace ErezeptValidator.Services.Validation.Validators;

/// <summary>
/// Validator for calculation rules (CALC-001 to CALC-003)
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
        var sokString = FhirDataExtractor.ExtractSokCode(lineItem);
        var (_, factorValue) = FhirDataExtractor.ExtractFactor(lineItem);
        var (priceCodeString, priceAmount) = FhirDataExtractor.ExtractPrice(lineItem);

        // Try to create value objects
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
            return; // Skip CALC-001 for these special codes
        }

        // CALC-001: Standard Promilleanteil formula validation
        await ValidateStandardFactorCalculationAsync(lineItem, actualFactor, lineNumber, result);
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
}
