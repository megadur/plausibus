using System.Text.RegularExpressions;

namespace ErezeptValidator.Services.Validation.Validators;

/// <summary>
/// Validator for format-level validation rules (FMT-001 to FMT-010)
/// Validates basic data format constraints before business logic validation
/// </summary>
public class FormatValidator : BaseValidator
{
    public override string ValidatorName => "FormatValidator";
    public override int Order => 1; // Run first

    public FormatValidator(ILogger<FormatValidator> logger) : base(logger)
    {
    }

    protected override async Task ExecuteValidationAsync(ValidationContext context)
    {
        // Format validations are synchronous, but we use Task for interface consistency
        await Task.CompletedTask;

        foreach (var lineItem in context.Request.LineItems)
        {
            ValidatePznFormat(context, lineItem);
            ValidatePznChecksum(context, lineItem);
            ValidateQuantity(context, lineItem);
            ValidateGrossPrice(context, lineItem);
            ValidateVatRate(context, lineItem);
            ValidateFactorCode(context, lineItem);
            ValidateFactorValue(context, lineItem);
            ValidatePriceCode(context, lineItem);
            ValidatePriceValue(context, lineItem);

            context.RulesChecked += 10; // 10 rules per line item
        }

        // FMT-003: Timestamp format (dispensing date)
        ValidateDispensingDateFormat(context);
        context.RulesChecked += 1;
    }

    /// <summary>
    /// FMT-001: PZN format must be 8 digits (if PZN is provided)
    /// </summary>
    private void ValidatePznFormat(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (string.IsNullOrWhiteSpace(lineItem.Pzn))
            return; // PZN is optional (might have SOK instead)

        var pznPattern = @"^\d{8}$";
        if (!Regex.IsMatch(lineItem.Pzn, pznPattern))
        {
            context.AddError(
                code: "FMT-001",
                message: $"PZN must be exactly 8 digits. Got: '{lineItem.Pzn}'",
                lineNumber: lineItem.LineNumber,
                field: "Pzn",
                suggestion: "Ensure PZN is padded with leading zeros to 8 digits (e.g., 01234567)"
            );
        }
    }

    /// <summary>
    /// FMT-002: PZN checksum validation (Modulo 11) - WARNING only
    /// </summary>
    private void ValidatePznChecksum(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (string.IsNullOrWhiteSpace(lineItem.Pzn) || lineItem.Pzn.Length != 8)
            return; // Skip if PZN is missing or invalid format (FMT-001 will catch format errors)

        if (!IsValidPznChecksum(lineItem.Pzn))
        {
            context.AddWarning(
                code: "FMT-002",
                message: $"PZN checksum validation failed for: {lineItem.Pzn}",
                lineNumber: lineItem.LineNumber,
                field: "Pzn",
                recommendation: "Verify PZN is correct. Invalid checksum may indicate a typo."
            );
        }
    }

    /// <summary>
    /// FMT-003: Dispensing date format validation
    /// </summary>
    private void ValidateDispensingDateFormat(ValidationContext context)
    {
        // DateTime is a struct, so it's always valid
        // Check for obviously invalid dates (like year 1 or future dates beyond reasonable)
        if (context.Request.DispensingDate.Year < 2020)
        {
            context.AddError(
                code: "FMT-003",
                message: $"Dispensing date is too far in the past: {context.Request.DispensingDate:yyyy-MM-dd}",
                field: "DispensingDate",
                suggestion: "Dispensing date should be between 2020 and now"
            );
        }

        if (context.Request.DispensingDate.Year > DateTime.UtcNow.Year + 1)
        {
            context.AddError(
                code: "FMT-003",
                message: $"Dispensing date is too far in the future: {context.Request.DispensingDate:yyyy-MM-dd}",
                field: "DispensingDate",
                suggestion: "Check the date format and ensure it's in the correct time zone"
            );
        }
    }

    /// <summary>
    /// FMT-004: Quantity must be positive
    /// </summary>
    private void ValidateQuantity(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (lineItem.Quantity <= 0)
        {
            context.AddError(
                code: "FMT-004",
                message: $"Quantity must be positive. Got: {lineItem.Quantity}",
                lineNumber: lineItem.LineNumber,
                field: "Quantity",
                suggestion: "Quantity must be greater than zero"
            );
        }
    }

    /// <summary>
    /// FMT-005: Gross price must be positive
    /// </summary>
    private void ValidateGrossPrice(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (lineItem.GrossPrice <= 0)
        {
            context.AddError(
                code: "FMT-005",
                message: $"Gross price must be positive. Got: {lineItem.GrossPrice:F2} EUR",
                lineNumber: lineItem.LineNumber,
                field: "GrossPrice",
                suggestion: "Gross price must be greater than zero"
            );
        }
    }

    /// <summary>
    /// FMT-006: VAT rate must be valid (0, 1, or 2)
    /// </summary>
    private void ValidateVatRate(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (lineItem.VatRate < 0 || lineItem.VatRate > 2)
        {
            context.AddError(
                code: "FMT-006",
                message: $"VAT rate must be 0 (0%), 1 (7%), or 2 (19%). Got: {lineItem.VatRate}",
                lineNumber: lineItem.LineNumber,
                field: "VatRate",
                suggestion: "Use valid VAT rate codes: 0=0%, 1=7%, 2=19%"
            );
        }
    }

    /// <summary>
    /// FMT-007: Factor code format (2 digits)
    /// </summary>
    private void ValidateFactorCode(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (string.IsNullOrWhiteSpace(lineItem.FactorCode))
            return; // Factor code is optional

        var factorCodePattern = @"^\d{2}$";
        if (!Regex.IsMatch(lineItem.FactorCode, factorCodePattern))
        {
            context.AddError(
                code: "FMT-007",
                message: $"Factor code must be exactly 2 digits. Got: '{lineItem.FactorCode}'",
                lineNumber: lineItem.LineNumber,
                field: "FactorCode",
                suggestion: "Use valid 2-digit factor codes (e.g., 11, 55, 57, 99)"
            );
        }
    }

    /// <summary>
    /// FMT-008: Factor value must be positive (if factor code is present)
    /// </summary>
    private void ValidateFactorValue(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (string.IsNullOrWhiteSpace(lineItem.FactorCode))
            return; // No factor code, so no factor value required

        if (!lineItem.FactorValue.HasValue || lineItem.FactorValue.Value <= 0)
        {
            context.AddError(
                code: "FMT-008",
                message: $"Factor value must be positive when factor code is provided. Got: {lineItem.FactorValue?.ToString() ?? "null"}",
                lineNumber: lineItem.LineNumber,
                field: "FactorValue",
                suggestion: "Provide a positive factor value matching the factor code"
            );
        }
    }

    /// <summary>
    /// FMT-009: Price code format (2 digits)
    /// </summary>
    private void ValidatePriceCode(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (string.IsNullOrWhiteSpace(lineItem.PriceCode))
            return; // Price code is optional

        var priceCodePattern = @"^\d{2}$";
        if (!Regex.IsMatch(lineItem.PriceCode, priceCodePattern))
        {
            context.AddError(
                code: "FMT-009",
                message: $"Price code must be exactly 2 digits. Got: '{lineItem.PriceCode}'",
                lineNumber: lineItem.LineNumber,
                field: "PriceCode",
                suggestion: "Use valid 2-digit price codes (e.g., 11, 12, 13, 14, 15, 16, 17, 21, 90)"
            );
        }
    }

    /// <summary>
    /// FMT-010: Price value must be positive (if price code is present)
    /// </summary>
    private void ValidatePriceValue(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (string.IsNullOrWhiteSpace(lineItem.PriceCode))
            return; // No price code, so no price value required

        if (!lineItem.PriceValue.HasValue || lineItem.PriceValue.Value <= 0)
        {
            context.AddError(
                code: "FMT-010",
                message: $"Price value must be positive when price code is provided. Got: {lineItem.PriceValue?.ToString() ?? "null"}",
                lineNumber: lineItem.LineNumber,
                field: "PriceValue",
                suggestion: "Provide a positive price value matching the price code"
            );
        }
    }

    /// <summary>
    /// Validate PZN checksum using Modulo 11 algorithm
    /// </summary>
    private bool IsValidPznChecksum(string pzn)
    {
        if (pzn.Length != 8 || !pzn.All(char.IsDigit))
            return false;

        // Modulo 11 algorithm for PZN checksum
        var sum = 0;
        for (var i = 0; i < 7; i++)
        {
            sum += (pzn[i] - '0') * (i + 2); // Weights: 2, 3, 4, 5, 6, 7, 8
        }

        var checkDigit = sum % 11;
        var expectedCheckDigit = pzn[7] - '0';

        return checkDigit == expectedCheckDigit;
    }
}
