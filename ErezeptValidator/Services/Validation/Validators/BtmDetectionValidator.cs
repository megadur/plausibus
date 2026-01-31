using ErezeptValidator.Data;
using ErezeptValidator.Models.Abdata;
using ErezeptValidator.Models.Validation;

namespace ErezeptValidator.Services.Validation.Validators;

/// <summary>
/// Detects BTM (Betäubungsmittel/narcotics) medications and adds appropriate warnings
/// </summary>
public class BtmDetectionValidator : IValidator
{
    private readonly ILogger<BtmDetectionValidator> _logger;

    public string Name => "BTM Detection Validator";

    public BtmDetectionValidator(ILogger<BtmDetectionValidator> logger)
    {
        _logger = logger;
    }

    public async Task<ValidationResult> ValidateAsync(ValidationContext context)
    {
        var result = ValidationResult.Success(Name);

        foreach (var pzn in context.PznCodes)
        {
            // Get article from context (populated by PznExistsValidator)
            if (context.Metadata.TryGetValue($"article_{pzn}", out var articleObj) &&
                articleObj is PacApoArticle article)
            {
                // Check BTM status (field 08)
                // 0 = keine Angabe, 1 = nein, 2 = BTM, 3 = Exempt preparation, 4 = T-Rezept
                if (article.Btm == 2)
                {
                    result.AddInfo("BTM-INFO",
                        $"BTM medication detected: {pzn} - {article.Name}. Additional BTM validation rules will apply.",
                        "PZN", pzn);

                    // Store BTM flag for other validators
                    context.Metadata[$"btm_{pzn}"] = true;
                }
                else if (article.Btm == 3)
                {
                    result.AddInfo("BTM-EXEMPT-INFO",
                        $"BTM exempt preparation detected: {pzn} - {article.Name}.",
                        "PZN", pzn);
                }
                else if (article.Btm == 4)
                {
                    result.AddInfo("T-REZEPT-INFO",
                        $"T-Rezept medication detected: {pzn} - {article.Name}. T-Rezept validation rules will apply.",
                        "PZN", pzn);

                    context.Metadata[$"trezept_{pzn}"] = true;
                }
            }
        }

        return await Task.FromResult(result);
    }
}
