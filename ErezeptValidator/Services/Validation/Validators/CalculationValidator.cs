namespace ErezeptValidator.Services.Validation.Validators;

/// <summary>
/// Validator for calculation-level validation rules (CALC-001 to CALC-003)
/// Validates price and factor calculations
/// </summary>
public class CalculationValidator : BaseValidator
{
    public override string ValidatorName => "CalculationValidator";
    public override int Order => 3; // Run after format and general validation

    public CalculationValidator(ILogger<CalculationValidator> logger) : base(logger)
    {
    }

    protected override async Task ExecuteValidationAsync(ValidationContext context)
    {
        // Calculation validations are synchronous
        await Task.CompletedTask;

        foreach (var lineItem in context.Request.LineItems)
        {
            ValidateGrossPrice(context, lineItem);
            ValidateFactorValueFormat(context, lineItem);
            ValidateFactorValuePositive(context, lineItem);

            context.RulesChecked += 3; // 3 rules per line item
        }
    }

    /// <summary>
    /// CALC-001: Gross price must be greater than zero
    /// (Duplicate of FMT-005, but in calculation context)
    /// </summary>
    private void ValidateGrossPrice(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (lineItem.GrossPrice <= 0)
        {
            context.AddError(
                code: "CALC-001",
                message: $"Gross price calculation failed: price must be positive. Got: {lineItem.GrossPrice:F2} EUR",
                lineNumber: lineItem.LineNumber,
                field: "GrossPrice",
                suggestion: "Verify price calculation includes all components (base price + surcharges)"
            );
        }

        // Additional check for unusually high prices (potential data entry error)
        if (lineItem.GrossPrice > 10000)
        {
            context.AddWarning(
                code: "CALC-001",
                message: $"Gross price is unusually high: {lineItem.GrossPrice:F2} EUR",
                lineNumber: lineItem.LineNumber,
                field: "GrossPrice",
                recommendation: "Verify this price is correct. Prices over 10,000 EUR are rare."
            );
        }
    }

    /// <summary>
    /// CALC-002: Factor value format consistency
    /// Factor value must be consistent with the factor code's expected format
    /// </summary>
    private void ValidateFactorValueFormat(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (!lineItem.FactorValue.HasValue || string.IsNullOrWhiteSpace(lineItem.FactorCode))
            return; // No factor to validate

        var factorValue = lineItem.FactorValue.Value;

        // Factor code 11 and 99: Anteil in Promille (parts per thousand, range: 0-1000)
        if (lineItem.FactorCode == "11" || lineItem.FactorCode == "99")
        {
            if (factorValue < 0 || factorValue > 1000)
            {
                context.AddError(
                    code: "CALC-002",
                    message: $"Factor value {factorValue} is out of valid range for factor code {lineItem.FactorCode} (Anteil in Promille). Expected: 0-1000‰",
                    lineNumber: lineItem.LineNumber,
                    field: "FactorValue",
                    suggestion: "Factor value must be between 0 and 1000 promille (parts per thousand)"
                );
            }
        }

        // Factor code 55 and 57: Einzeldosis in Milligramm (single dose in mg, typically 0-500mg)
        if (lineItem.FactorCode == "55" || lineItem.FactorCode == "57")
        {
            if (factorValue < 0)
            {
                context.AddError(
                    code: "CALC-002",
                    message: $"Factor value {factorValue} cannot be negative for factor code {lineItem.FactorCode} (Einzeldosis mg)",
                    lineNumber: lineItem.LineNumber,
                    field: "FactorValue",
                    suggestion: "Single dose must be a positive value in milligrams"
                );
            }

            if (factorValue > 500)
            {
                context.AddWarning(
                    code: "CALC-002",
                    message: $"Factor value {factorValue}mg is unusually high for factor code {lineItem.FactorCode} (Einzeldosis). Typical range: 0-500mg",
                    lineNumber: lineItem.LineNumber,
                    field: "FactorValue",
                    recommendation: "Verify the dosage is correct. Single doses above 500mg are uncommon for opioid substitution."
                );
            }
        }

        // Check for decimal precision (values should have at most 3 decimal places)
        var decimalPlaces = GetDecimalPlaces(factorValue);
        if (decimalPlaces > 3)
        {
            context.AddWarning(
                code: "CALC-002",
                message: $"Factor value {factorValue} has {decimalPlaces} decimal places. Standard precision is 3 decimal places.",
                lineNumber: lineItem.LineNumber,
                field: "FactorValue",
                recommendation: "Round factor value to 3 decimal places for consistency"
            );
        }
    }

    /// <summary>
    /// CALC-003: Factor value must be greater than zero
    /// </summary>
    private void ValidateFactorValuePositive(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (!lineItem.FactorValue.HasValue || string.IsNullOrWhiteSpace(lineItem.FactorCode))
            return; // No factor to validate

        if (lineItem.FactorValue.Value <= 0)
        {
            context.AddError(
                code: "CALC-003",
                message: $"Factor value must be positive. Got: {lineItem.FactorValue.Value}",
                lineNumber: lineItem.LineNumber,
                field: "FactorValue",
                suggestion: "Factor value must be greater than zero"
            );
        }
    }

    /// <summary>
    /// Helper method to count decimal places in a decimal value
    /// </summary>
    private int GetDecimalPlaces(decimal value)
    {
        var valueString = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var decimalIndex = valueString.IndexOf('.');

        if (decimalIndex == -1)
            return 0;

        return valueString.Length - decimalIndex - 1;
    }
}
