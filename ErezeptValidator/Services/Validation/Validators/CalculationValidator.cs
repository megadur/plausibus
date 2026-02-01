using ErezeptValidator.Data;
using ErezeptValidator.Models.Validation;
using ErezeptValidator.Services.CodeLookup;
using ErezeptValidator.Services.Validation.Helpers;
using Hl7.Fhir.Model;

namespace ErezeptValidator.Services.Validation.Validators;

/// <summary>
/// Validator for calculation rules (CALC-001 to CALC-003)
/// Validates factors, prices, and special calculations for Abgabedaten bundles
/// </summary>
public class CalculationValidator : IValidator
{
    private readonly ICodeLookupService _codeLookupService;
    private readonly IPznRepository _pznRepository;
    private readonly ILogger<CalculationValidator> _logger;

    // Special codes without quantity reference (CALC-002)
    private static readonly HashSet<string> SpecialCodesWithoutQuantityReference = new()
    {
        "1.1.1", "1.1.2", "1.2.1", "1.2.2",
        "1.3.1", "1.3.2",
        "1.6.5",
        "1.10.2", "1.10.3"
    };

    // Artificial insemination marker code (CALC-003)
    private const string ArtificialInseminationCode = "09999643";
    private const decimal ArtificialInseminationFactor = 1000.000000m;
    private const decimal ArtificialInseminationPrice = 0.00m;
    private const string ArtificialInseminationPriceId = "90";

    // Tolerance for decimal comparisons (accounts for floating point precision)
    private const decimal DecimalTolerance = 0.000001m;

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
            foreach (var lineItem in invoice.LineItem ?? Enumerable.Empty<Invoice.LineItemComponent>())
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
        Invoice.LineItemComponent lineItem,
        int lineNumber,
        ValidationResult result)
    {
        var sok = FhirDataExtractor.ExtractSokCode(lineItem);
        var (factorCode, factorValue) = FhirDataExtractor.ExtractFactor(lineItem);
        var (priceCode, priceAmount) = FhirDataExtractor.ExtractPrice(lineItem);

        // CALC-003: Artificial insemination special code validation
        if (sok == ArtificialInseminationCode)
        {
            ValidateArtificialInseminationCode(factorValue, priceAmount, priceCode, lineNumber, result);
            return; // Skip other validations for this special case
        }

        // CALC-002: Special codes without quantity reference
        if (!string.IsNullOrEmpty(sok) && SpecialCodesWithoutQuantityReference.Contains(sok))
        {
            ValidateSpecialCodeFactor(sok, factorValue, lineNumber, result);
            return; // Skip CALC-001 for these special codes
        }

        // CALC-001: Standard Promilleanteil formula validation
        await ValidateStandardFactorCalculationAsync(lineItem, factorValue, lineNumber, result);
    }

    /// <summary>
    /// CALC-001: Standard Promilleanteil (per mille) formula validation
    /// Formula: Factor = (Dispensed_Quantity / Package_Quantity) × 1000
    /// </summary>
    private async System.Threading.Tasks.Task ValidateStandardFactorCalculationAsync(
        Invoice.LineItemComponent lineItem,
        decimal? actualFactor,
        int lineNumber,
        ValidationResult result)
    {
        if (actualFactor == null)
        {
            // No factor provided - this is acceptable for some line items
            return;
        }

        // Extract quantities
        var dispensedQty = FhirDataExtractor.ExtractDispensedQuantity(lineItem);
        var packageQty = FhirDataExtractor.ExtractPackageQuantity(lineItem);

        // If we can't determine quantities, we can't validate the calculation
        // This is a warning, not an error, as the data might be structured differently
        if (dispensedQty == null || packageQty == null)
        {
            _logger.LogDebug(
                "Cannot validate factor calculation for line {LineNumber} - missing quantity information (dispensed: {Dispensed}, package: {Package})",
                lineNumber, dispensedQty, packageQty);
            return;
        }

        // Validate package quantity is not zero
        if (packageQty == 0)
        {
            result.AddError(
                "CALC-001-E",
                $"Invalid package quantity (zero) for line item {lineNumber}. Cannot calculate factor.",
                $"LineItem[{lineNumber}].PackageQuantity");
            return;
        }

        // Calculate expected factor: (dispensed / package) * 1000
        var expectedFactor = (dispensedQty.Value / packageQty.Value) * 1000m;

        // Normalize both values for comparison (handle different decimal representations)
        var normalizedExpected = NormalizeFactor(expectedFactor);
        var normalizedActual = NormalizeFactor(actualFactor.Value);

        if (!AreFactorsEqual(normalizedExpected, normalizedActual))
        {
            result.AddError(
                "CALC-001-E",
                $"Factor (Promilleanteil) calculation incorrect for line item {lineNumber}. " +
                $"Expected: {FormatFactor(normalizedExpected)}, Found: {FormatFactor(normalizedActual)}. " +
                $"Calculation: ({dispensedQty} / {packageQty}) × 1000 = {FormatFactor(normalizedExpected)}",
                $"LineItem[{lineNumber}].Factor");
        }
    }

    /// <summary>
    /// CALC-002: Special codes without quantity reference must have factor 1.000000
    /// </summary>
    private void ValidateSpecialCodeFactor(
        string sokCode,
        decimal? actualFactor,
        int lineNumber,
        ValidationResult result)
    {
        if (actualFactor == null)
        {
            result.AddError(
                "CALC-002-E",
                $"Special code {sokCode} without quantity reference requires a factor. Expected: 1.000000, Found: (none)",
                $"LineItem[{lineNumber}].Factor");
            return;
        }

        var expectedFactor = 1.000000m;
        var normalizedActual = NormalizeFactor(actualFactor.Value);

        if (!AreFactorsEqual(expectedFactor, normalizedActual))
        {
            result.AddError(
                "CALC-002-E",
                $"Special code {sokCode} without quantity reference must have factor 1.000000. " +
                $"Expected: 1.000000, Found: {FormatFactor(normalizedActual)}",
                $"LineItem[{lineNumber}].Factor");
        }
    }

    /// <summary>
    /// CALC-003: Artificial insemination special code (09999643) validation
    /// Must have: Factor = 1000.000000, Price = 0.00, Price Identifier = "90"
    /// </summary>
    private void ValidateArtificialInseminationCode(
        decimal? actualFactor,
        decimal? actualPrice,
        string? actualPriceId,
        int lineNumber,
        ValidationResult result)
    {
        var errors = new List<string>();

        // Validate Factor = 1000.000000
        if (actualFactor == null)
        {
            errors.Add($"Factor must be {ArtificialInseminationFactor} (found: none)");
        }
        else if (!AreFactorsEqual(ArtificialInseminationFactor, actualFactor.Value))
        {
            errors.Add($"Factor must be {ArtificialInseminationFactor} (found: {FormatFactor(actualFactor.Value)})");
        }

        // Validate Price = 0.00
        if (actualPrice == null)
        {
            errors.Add($"Price must be {ArtificialInseminationPrice:F2} (found: none)");
        }
        else if (!ArePricesEqual(ArtificialInseminationPrice, actualPrice.Value))
        {
            errors.Add($"Price must be {ArtificialInseminationPrice:F2} (found: {actualPrice.Value:F2})");
        }

        // Validate Price Identifier = "90"
        if (string.IsNullOrEmpty(actualPriceId))
        {
            errors.Add($"Price identifier must be '{ArtificialInseminationPriceId}' (found: none)");
        }
        else if (actualPriceId != ArtificialInseminationPriceId)
        {
            errors.Add($"Price identifier must be '{ArtificialInseminationPriceId}' (found: '{actualPriceId}')");
        }

        if (errors.Count > 0)
        {
            result.AddError(
                "CALC-003-E",
                $"Artificial insemination special code (09999643) validation failed for line item {lineNumber}: " +
                string.Join("; ", errors),
                $"LineItem[{lineNumber}].ArtificialInsemination");
        }
    }

    /// <summary>
    /// Normalize factor for comparison (handles different decimal representations)
    /// e.g., 1, 1.0, 1.000000 should all be considered equal
    /// </summary>
    private decimal NormalizeFactor(decimal factor)
    {
        // Round to 6 decimal places (per TA1 spec)
        return Math.Round(factor, 6, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Check if two factors are equal within tolerance
    /// </summary>
    private bool AreFactorsEqual(decimal expected, decimal actual)
    {
        return Math.Abs(expected - actual) < DecimalTolerance;
    }

    /// <summary>
    /// Check if two prices are equal within tolerance
    /// </summary>
    private bool ArePricesEqual(decimal expected, decimal actual)
    {
        // Prices use 2 decimal places
        var roundedExpected = Math.Round(expected, 2, MidpointRounding.AwayFromZero);
        var roundedActual = Math.Round(actual, 2, MidpointRounding.AwayFromZero);
        return Math.Abs(roundedExpected - roundedActual) < 0.01m;
    }

    /// <summary>
    /// Format factor for display (removes unnecessary trailing zeros)
    /// </summary>
    private string FormatFactor(decimal factor)
    {
        // Format with up to 6 decimal places, removing trailing zeros
        var formatted = factor.ToString("0.######");
        return formatted;
    }
}
