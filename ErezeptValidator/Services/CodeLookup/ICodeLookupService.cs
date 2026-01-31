using ErezeptValidator.Models.Ta1Reference;

namespace ErezeptValidator.Services.CodeLookup;

/// <summary>
/// Service interface for looking up and validating TA1 reference codes with caching
/// </summary>
public interface ICodeLookupService
{
    // Special Code (SOK) Lookups
    Task<SpecialCode?> GetSpecialCodeAsync(string code);
    Task<IEnumerable<SpecialCode>> GetSpecialCodesByTypeAsync(string codeType);
    Task<IEnumerable<SpecialCode>> GetValidSpecialCodesAsync(DateOnly date);

    // Special Code Validations
    Task<bool> ValidateSokTemporalAsync(string code, DateOnly dispensingDate);
    Task<bool> ValidateSokErezeptCompatibilityAsync(string code, bool isErezept);
    Task<bool> ValidateSokVatRateAsync(string code, short vatRate);

    // Factor Code Lookups (Cached)
    Task<IEnumerable<FactorCode>> GetAllFactorCodesAsync();
    Task<FactorCode?> GetFactorCodeAsync(string code);

    // Price Code Lookups (Cached)
    Task<IEnumerable<PriceCode>> GetAllPriceCodesAsync();
    Task<PriceCode?> GetPriceCodeAsync(string code);

    // Statistics
    Task<int> GetSpecialCodeCountAsync();
}
