using ErezeptValidator.Data.Contexts;
using ErezeptValidator.Models.Ta1Reference;
using Microsoft.EntityFrameworkCore;

namespace ErezeptValidator.Data;

/// <summary>
/// Repository for TA1 reference data using EF Core
/// </summary>
public class Ta1Repository : ITa1Repository
{
    private readonly Ta1DbContext _context;

    public Ta1Repository(Ta1DbContext context)
    {
        _context = context;
    }

    public async Task<SpecialCode?> GetSpecialCodeAsync(string code)
    {
        return await _context.SpecialCodes.FirstOrDefaultAsync(s => s.Code == code);
    }

    public async Task<FactorCode?> GetFactorCodeAsync(string code)
    {
        return await _context.FactorCodes.FirstOrDefaultAsync(f => f.Code == code);
    }

    public async Task<PriceCode?> GetPriceCodeAsync(string code)
    {
        return await _context.PriceCodes.FirstOrDefaultAsync(p => p.Code == code);
    }
}
