using ErezeptValidator.Services.CodeLookup;

namespace ErezeptValidator.Services.Validation.Validators;

/// <summary>
/// Validator for general business logic rules (GEN-001 to GEN-008)
/// Validates business constraints using TA1 reference data
/// </summary>
public class GeneralRuleValidator : BaseValidator
{
    private readonly ICodeLookupService _codeLookupService;

    public override string ValidatorName => "GeneralRuleValidator";
    public override int Order => 2; // Run after format validation

    public GeneralRuleValidator(
        ICodeLookupService codeLookupService,
        ILogger<GeneralRuleValidator> logger) : base(logger)
    {
        _codeLookupService = codeLookupService;
    }

    protected override async Task ExecuteValidationAsync(ValidationContext context)
    {
        // GEN-002: Dispensing date validation (applies to whole prescription)
        await ValidateDispensingDateAsync(context);
        context.RulesChecked += 1;

        foreach (var lineItem in context.Request.LineItems)
        {
            await ValidatePznOrSokRequiredAsync(context, lineItem);
            await ValidateFactorCodeAsync(context, lineItem);
            await ValidatePriceCodeAsync(context, lineItem);
            await ValidateFactorPriceConsistencyAsync(context, lineItem);
            await ValidateSokTemporalAsync(context, lineItem);
            await ValidateSokErezeptCompatibilityAsync(context, lineItem);
            await ValidateSokVatRateAsync(context, lineItem);

            context.RulesChecked += 7; // 7 rules per line item
        }
    }

    /// <summary>
    /// GEN-001: PZN or SOK required (mutually exclusive)
    /// </summary>
    private async Task ValidatePznOrSokRequiredAsync(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        await Task.CompletedTask; // Synchronous validation

        var hasPzn = !string.IsNullOrWhiteSpace(lineItem.Pzn);
        var hasSok = !string.IsNullOrWhiteSpace(lineItem.Sok);

        if (!hasPzn && !hasSok)
        {
            context.AddError(
                code: "GEN-001",
                message: "Either PZN or SOK code must be provided",
                lineNumber: lineItem.LineNumber,
                field: "Pzn/Sok",
                suggestion: "Provide either a valid PZN (pharmaceutical product) or SOK (special service code)"
            );
        }
        else if (hasPzn && hasSok)
        {
            context.AddError(
                code: "GEN-001",
                message: "PZN and SOK are mutually exclusive. Only one should be provided.",
                lineNumber: lineItem.LineNumber,
                field: "Pzn/Sok",
                suggestion: "Remove either PZN or SOK - only one identifier per line item"
            );
        }
    }

    /// <summary>
    /// GEN-002: Dispensing date must not be in the future
    /// </summary>
    private async Task ValidateDispensingDateAsync(ValidationContext context)
    {
        await Task.CompletedTask; // Synchronous validation

        var now = DateTime.UtcNow;
        if (context.Request.DispensingDate > now)
        {
            context.AddError(
                code: "GEN-002",
                message: $"Dispensing date cannot be in the future. Got: {context.Request.DispensingDate:yyyy-MM-dd HH:mm}, Current time: {now:yyyy-MM-dd HH:mm} UTC",
                field: "DispensingDate",
                suggestion: "Ensure dispensing date is in the past or present. Check time zone conversion."
            );
        }
    }

    /// <summary>
    /// GEN-003: Factor code must be valid if present
    /// </summary>
    private async Task ValidateFactorCodeAsync(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (string.IsNullOrWhiteSpace(lineItem.FactorCode))
            return; // Factor code is optional

        var factorCode = await _codeLookupService.GetFactorCodeAsync(lineItem.FactorCode);

        if (factorCode == null)
        {
            context.AddError(
                code: "GEN-003",
                message: $"Factor code '{lineItem.FactorCode}' is not valid according to TA1 reference data",
                lineNumber: lineItem.LineNumber,
                field: "FactorCode",
                suggestion: "Use valid factor codes: 11 (Anteil in Promille), 55 (Einzeldosis mg - take-home), 57 (Einzeldosis mg - supervised), 99 (Anteil einer Packung)"
            );
        }
        else
        {
            Logger.LogDebug("Factor code {Code} validated: {Description}", lineItem.FactorCode, factorCode.Description);
        }
    }

    /// <summary>
    /// GEN-004: Price code must be valid if present
    /// </summary>
    private async Task ValidatePriceCodeAsync(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (string.IsNullOrWhiteSpace(lineItem.PriceCode))
            return; // Price code is optional

        var priceCode = await _codeLookupService.GetPriceCodeAsync(lineItem.PriceCode);

        if (priceCode == null)
        {
            context.AddError(
                code: "GEN-004",
                message: $"Price code '{lineItem.PriceCode}' is not valid according to TA1 reference data",
                lineNumber: lineItem.LineNumber,
                field: "PriceCode",
                suggestion: "Use valid price codes: 11-17 (various pricing methods), 21 (Rezepturpreis), 90 (Sonstiges)"
            );
        }
        else
        {
            Logger.LogDebug("Price code {Code} validated: {Description}", lineItem.PriceCode, priceCode.Description);
        }
    }

    /// <summary>
    /// GEN-005: Factor/price code consistency
    /// </summary>
    private async Task ValidateFactorPriceConsistencyAsync(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        await Task.CompletedTask; // Synchronous validation

        var hasFactorCode = !string.IsNullOrWhiteSpace(lineItem.FactorCode);
        var hasFactorValue = lineItem.FactorValue.HasValue;
        var hasPriceCode = !string.IsNullOrWhiteSpace(lineItem.PriceCode);
        var hasPriceValue = lineItem.PriceValue.HasValue;

        // If factor code is provided, factor value must be provided
        if (hasFactorCode && !hasFactorValue)
        {
            context.AddError(
                code: "GEN-005",
                message: $"Factor code '{lineItem.FactorCode}' provided but factor value is missing",
                lineNumber: lineItem.LineNumber,
                field: "FactorValue",
                suggestion: "Provide a factor value when using a factor code"
            );
        }

        // If price code is provided, price value must be provided
        if (hasPriceCode && !hasPriceValue)
        {
            context.AddError(
                code: "GEN-005",
                message: $"Price code '{lineItem.PriceCode}' provided but price value is missing",
                lineNumber: lineItem.LineNumber,
                field: "PriceValue",
                suggestion: "Provide a price value when using a price code"
            );
        }

        // If factor value is provided without code
        if (!hasFactorCode && hasFactorValue)
        {
            context.AddWarning(
                code: "GEN-005",
                message: $"Factor value {lineItem.FactorValue} provided but factor code is missing",
                lineNumber: lineItem.LineNumber,
                field: "FactorCode",
                recommendation: "Provide a factor code or remove the factor value"
            );
        }

        // If price value is provided without code
        if (!hasPriceCode && hasPriceValue)
        {
            context.AddWarning(
                code: "GEN-005",
                message: $"Price value {lineItem.PriceValue} provided but price code is missing",
                lineNumber: lineItem.LineNumber,
                field: "PriceCode",
                recommendation: "Provide a price code or remove the price value"
            );
        }
    }

    /// <summary>
    /// GEN-006: SOK temporal validation (check if code is valid on dispensing date)
    /// </summary>
    private async Task ValidateSokTemporalAsync(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (string.IsNullOrWhiteSpace(lineItem.Sok))
            return; // SOK not provided

        var dispensingDate = DateOnly.FromDateTime(context.Request.DispensingDate);
        var isValid = await _codeLookupService.ValidateSokTemporalAsync(lineItem.Sok, dispensingDate);

        if (!isValid)
        {
            var sokCode = await _codeLookupService.GetSpecialCodeAsync(lineItem.Sok);
            if (sokCode == null)
            {
                context.AddError(
                    code: "GEN-006",
                    message: $"SOK code '{lineItem.Sok}' not found in TA1 reference database",
                    lineNumber: lineItem.LineNumber,
                    field: "Sok",
                    suggestion: "Verify the SOK code is correct and exists in the current TA1 reference data"
                );
            }
            else if (sokCode.ValidFromDispensingDate.HasValue && dispensingDate < sokCode.ValidFromDispensingDate.Value)
            {
                context.AddError(
                    code: "GEN-006",
                    message: $"SOK code '{lineItem.Sok}' is not yet valid on {dispensingDate:yyyy-MM-dd}. Valid from: {sokCode.ValidFromDispensingDate:yyyy-MM-dd}",
                    lineNumber: lineItem.LineNumber,
                    field: "Sok",
                    suggestion: $"This SOK code will be valid starting {sokCode.ValidFromDispensingDate:yyyy-MM-dd}"
                );
            }
            else if (sokCode.ExpiredDispensingDate.HasValue && dispensingDate > sokCode.ExpiredDispensingDate.Value)
            {
                context.AddError(
                    code: "GEN-006",
                    message: $"SOK code '{lineItem.Sok}' expired on {sokCode.ExpiredDispensingDate:yyyy-MM-dd}. Dispensing date: {dispensingDate:yyyy-MM-dd}",
                    lineNumber: lineItem.LineNumber,
                    field: "Sok",
                    suggestion: $"This SOK code expired on {sokCode.ExpiredDispensingDate:yyyy-MM-dd}. Use an alternative SOK code."
                );
            }
        }
    }

    /// <summary>
    /// GEN-007: E-Rezept SOK compatibility validation
    /// </summary>
    private async Task ValidateSokErezeptCompatibilityAsync(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (string.IsNullOrWhiteSpace(lineItem.Sok))
            return; // SOK not provided

        var isCompatible = await _codeLookupService.ValidateSokErezeptCompatibilityAsync(lineItem.Sok, context.Request.IsErezept);

        if (!isCompatible)
        {
            var sokCode = await _codeLookupService.GetSpecialCodeAsync(lineItem.Sok);
            if (sokCode != null)
            {
                if (context.Request.IsErezept && sokCode.ERezept == 0)
                {
                    context.AddError(
                        code: "GEN-007",
                        message: $"SOK code '{lineItem.Sok}' is not compatible with E-Rezept. This is a paper prescription-only SOK code.",
                        lineNumber: lineItem.LineNumber,
                        field: "Sok",
                        suggestion: "Use an E-Rezept compatible SOK code or process as paper prescription"
                    );
                }
                else if (!context.Request.IsErezept && sokCode.ERezept == 2)
                {
                    context.AddError(
                        code: "GEN-007",
                        message: $"SOK code '{lineItem.Sok}' is mandatory for E-Rezept and cannot be used on paper prescriptions",
                        lineNumber: lineItem.LineNumber,
                        field: "Sok",
                        suggestion: "This SOK code can only be used with electronic prescriptions"
                    );
                }
            }
        }
    }

    /// <summary>
    /// GEN-008: VAT rate consistency with SOK code
    /// </summary>
    private async Task ValidateSokVatRateAsync(ValidationContext context, Models.Validation.PrescriptionLineItem lineItem)
    {
        if (string.IsNullOrWhiteSpace(lineItem.Sok))
            return; // SOK not provided

        var isConsistent = await _codeLookupService.ValidateSokVatRateAsync(lineItem.Sok, lineItem.VatRate);

        if (!isConsistent)
        {
            var sokCode = await _codeLookupService.GetSpecialCodeAsync(lineItem.Sok);
            if (sokCode?.VatRate.HasValue == true)
            {
                var expectedVatPercentage = sokCode.GetVatPercentage();
                var actualVatPercentage = lineItem.VatRate switch
                {
                    0 => 0,
                    1 => 7,
                    2 => 19,
                    _ => -1
                };

                context.AddError(
                    code: "GEN-008",
                    message: $"VAT rate mismatch for SOK code '{lineItem.Sok}'. Expected: {sokCode.VatRate} ({expectedVatPercentage}%), Got: {lineItem.VatRate} ({actualVatPercentage}%)",
                    lineNumber: lineItem.LineNumber,
                    field: "VatRate",
                    suggestion: $"Use VAT rate {sokCode.VatRate} ({expectedVatPercentage}%) as specified in TA1 reference data for this SOK code"
                );
            }
        }
    }
}
