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

    #region Special Codes (SOK)

    public async Task<SpecialCode?> GetSpecialCodeAsync(string code)
    {
        return await _context.SpecialCodes
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Code == code);
    }

    public async Task<IEnumerable<SpecialCode>> GetSpecialCodesByTypeAsync(string codeType)
    {
        return await _context.SpecialCodes
            .AsNoTracking()
            .Where(s => s.CodeType == codeType)
            .OrderBy(s => s.Code)
            .ToListAsync();
    }

    public async Task<IEnumerable<SpecialCode>> GetValidSpecialCodesAsync(DateOnly date)
    {
        return await _context.SpecialCodes
            .AsNoTracking()
            .Where(s =>
                // Code must be valid from dispensing date
                (!s.ValidFromDispensingDate.HasValue || s.ValidFromDispensingDate.Value <= date) &&
                // Code must not be expired
                (!s.ExpiredDispensingDate.HasValue || s.ExpiredDispensingDate.Value >= date))
            .OrderBy(s => s.Code)
            .ToListAsync();
    }

    public async Task<int> GetSpecialCodeCountAsync()
    {
        return await _context.SpecialCodes.CountAsync();
    }

    #endregion

    #region Factor Codes

    public async Task<FactorCode?> GetFactorCodeAsync(string code)
    {
        return await _context.FactorCodes
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Code == code);
    }

    public async Task<IEnumerable<FactorCode>> GetAllFactorCodesAsync()
    {
        return await _context.FactorCodes
            .AsNoTracking()
            .OrderBy(f => f.Code)
            .ToListAsync();
    }

    #endregion

    #region Price Codes

    public async Task<PriceCode?> GetPriceCodeAsync(string code)
    {
        return await _context.PriceCodes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Code == code);
    }

    public async Task<IEnumerable<PriceCode>> GetAllPriceCodesAsync()
    {
        return await _context.PriceCodes
            .AsNoTracking()
            .OrderBy(p => p.Code)
            .ToListAsync();
    }

    #endregion

    #region Validation Logs

    public async Task AddValidationLogAsync(ValidationLog log)
    {
        _context.ValidationLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    #endregion
}
