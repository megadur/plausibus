using ErezeptValidator.Data;
using ErezeptValidator.Models.Validation;

namespace ErezeptValidator.Services.Validation.Validators;

/// <summary>
/// Validates that PZN exists in ABDATA database
/// </summary>
public class PznExistsValidator : IValidator
{
    private readonly IPznRepository _pznRepository;
    private readonly ILogger<PznExistsValidator> _logger;

    public string Name => "PZN Exists Validator";

    public PznExistsValidator(IPznRepository pznRepository, ILogger<PznExistsValidator> logger)
    {
        _pznRepository = pznRepository;
        _logger = logger;
    }

    public async Task<ValidationResult> ValidateAsync(ValidationContext context)
    {
        var result = ValidationResult.Success(Name);

        // Batch lookup all PZNs
        var articles = await _pznRepository.GetByPznBatchAsync(context.PznCodes.ToArray());

        foreach (var pzn in context.PznCodes)
        {
            if (!articles.ContainsKey(pzn))
            {
                result.AddWarning("DATA-001-W", $"PZN {pzn} not found in ABDATA database. Verify PZN is correct.", "PZN", pzn);
            }
            else
            {
                // Store article in context for use by other validators
                context.Metadata[$"article_{pzn}"] = articles[pzn];
            }
        }

        return result;
    }
}
