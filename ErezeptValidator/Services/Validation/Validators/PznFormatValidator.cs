using ErezeptValidator.Data;
using ErezeptValidator.Models.Validation;

namespace ErezeptValidator.Services.Validation.Validators;

/// <summary>
/// Validates PZN format and checksum (FMT-001, FMT-002)
/// </summary>
public class PznFormatValidator : IValidator
{
    private readonly IPznRepository _pznRepository;
    private readonly ILogger<PznFormatValidator> _logger;

    public string Name => "PZN Format Validator";

    public PznFormatValidator(IPznRepository pznRepository, ILogger<PznFormatValidator> logger)
    {
        _pznRepository = pznRepository;
        _logger = logger;
    }

    public async Task<ValidationResult> ValidateAsync(ValidationContext context)
    {
        var result = ValidationResult.Success(Name);

        foreach (var pzn in context.PznCodes)
        {
            // FMT-001: Format validation
            if (!_pznRepository.ValidatePznFormat(pzn))
            {
                result.AddError("FMT-001-E", $"Invalid PZN format: {pzn}. PZN must be 8 digits.", "PZN", pzn);
                continue; // Skip checksum validation if format is invalid
            }

            // FMT-002: Checksum validation
            if (!_pznRepository.ValidatePznChecksum(pzn))
            {
                result.AddWarning("FMT-002-W", $"PZN checksum invalid: {pzn}. Verify PZN is correct.", "PZN", pzn);
            }
        }

        return await Task.FromResult(result);
    }
}
