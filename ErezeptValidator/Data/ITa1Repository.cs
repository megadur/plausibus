using ErezeptValidator.Models.Ta1Reference;

namespace ErezeptValidator.Data;

/// <summary>
/// Repository interface for TA1 reference data
/// </summary>
public interface ITa1Repository
{
    // Special Codes (SOK)
    Task<SpecialCode?> GetSpecialCodeAsync(string code);
    Task<IEnumerable<SpecialCode>> GetSpecialCodesByTypeAsync(string codeType);
    Task<IEnumerable<SpecialCode>> GetValidSpecialCodesAsync(DateOnly date);
    Task<int> GetSpecialCodeCountAsync();

    // Factor Codes
    Task<FactorCode?> GetFactorCodeAsync(string code);
    Task<IEnumerable<FactorCode>> GetAllFactorCodesAsync();

    // Price Codes
    Task<PriceCode?> GetPriceCodeAsync(string code);
    Task<IEnumerable<PriceCode>> GetAllPriceCodesAsync();

    // Validation Logs
    Task AddValidationLogAsync(ValidationLog log);
}
