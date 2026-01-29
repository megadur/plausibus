using ErezeptValidator.Models.Ta1Reference;

namespace ErezeptValidator.Data;

/// <summary>
/// Repository interface for TA1 reference data
/// </summary>
public interface ITa1Repository
{
    Task<SpecialCode?> GetSpecialCodeAsync(string code);
    Task<FactorCode?> GetFactorCodeAsync(string code);
    Task<PriceCode?> GetPriceCodeAsync(string code);
}
