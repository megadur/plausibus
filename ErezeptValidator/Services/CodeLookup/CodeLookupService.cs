using ErezeptValidator.Data;
using ErezeptValidator.Models.Ta1Reference;
using Microsoft.Extensions.Caching.Memory;

namespace ErezeptValidator.Services.CodeLookup;

/// <summary>
/// Service for looking up and validating TA1 reference codes with intelligent caching
/// </summary>
public class CodeLookupService : ICodeLookupService
{
    private readonly ITa1Repository _repository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CodeLookupService> _logger;

    // Cache keys
    private const string AllFactorCodesCacheKey = "all_factor_codes";
    private const string AllPriceCodesCacheKey = "all_price_codes";
    private const string SpecialCodeCacheKeyPrefix = "sok_";

    // Cache expiration (24 hours - aligned with ABDATA update cycle)
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24);

    public CodeLookupService(
        ITa1Repository repository,
        IMemoryCache cache,
        ILogger<CodeLookupService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    #region Special Code (SOK) Lookups

    public async Task<SpecialCode?> GetSpecialCodeAsync(string code)
    {
        var cacheKey = $"{SpecialCodeCacheKeyPrefix}{code}";

        if (_cache.TryGetValue<SpecialCode>(cacheKey, out var cachedCode))
        {
            _logger.LogDebug("Special code {Code} retrieved from cache", code);
            return cachedCode;
        }

        var specialCode = await _repository.GetSpecialCodeAsync(code);

        if (specialCode != null)
        {
            _cache.Set(cacheKey, specialCode, CacheExpiration);
            _logger.LogDebug("Special code {Code} cached for 24 hours", code);
        }

        return specialCode;
    }

    public async Task<IEnumerable<SpecialCode>> GetSpecialCodesByTypeAsync(string codeType)
    {
        return await _repository.GetSpecialCodesByTypeAsync(codeType);
    }

    public async Task<IEnumerable<SpecialCode>> GetValidSpecialCodesAsync(DateOnly date)
    {
        return await _repository.GetValidSpecialCodesAsync(date);
    }

    public async Task<int> GetSpecialCodeCountAsync()
    {
        return await _repository.GetSpecialCodeCountAsync();
    }

    #endregion

    #region Special Code Validations

    /// <summary>
    /// Validates that a SOK code is valid on the given dispensing date (not expired, not future)
    /// </summary>
    public async Task<bool> ValidateSokTemporalAsync(string code, DateOnly dispensingDate)
    {
        var sok = await GetSpecialCodeAsync(code);
        if (sok == null)
        {
            _logger.LogWarning("SOK code {Code} not found for temporal validation", code);
            return false;
        }

        // Check if code is valid from dispensing date
        if (sok.ValidFromDispensingDate.HasValue && dispensingDate < sok.ValidFromDispensingDate.Value)
        {
            _logger.LogDebug("SOK {Code} not yet valid on {Date}. Valid from {ValidFrom}",
                code, dispensingDate, sok.ValidFromDispensingDate.Value);
            return false;
        }

        // Check if code has expired
        if (sok.ExpiredDispensingDate.HasValue && dispensingDate > sok.ExpiredDispensingDate.Value)
        {
            _logger.LogDebug("SOK {Code} expired on {Date}. Expired {ExpiredDate}",
                code, dispensingDate, sok.ExpiredDispensingDate.Value);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates that a SOK code is compatible with E-Rezept requirements
    /// E-Rezept field: 0=not compatible, 1=compatible, 2=mandatory for E-Rezept
    /// </summary>
    public async Task<bool> ValidateSokErezeptCompatibilityAsync(string code, bool isErezept)
    {
        var sok = await GetSpecialCodeAsync(code);
        if (sok == null)
        {
            _logger.LogWarning("SOK code {Code} not found for E-Rezept compatibility check", code);
            return false;
        }

        // If this is an E-Rezept, SOK must be compatible (e_rezept >= 1)
        if (isErezept && sok.ERezept == 0)
        {
            _logger.LogDebug("SOK {Code} is not compatible with E-Rezept (e_rezept={ERezept})",
                code, sok.ERezept);
            return false;
        }

        // If this is NOT an E-Rezept but SOK is mandatory for E-Rezept (e_rezept = 2), it's invalid
        if (!isErezept && sok.ERezept == 2)
        {
            _logger.LogDebug("SOK {Code} is mandatory for E-Rezept and cannot be used on paper prescriptions",
                code);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates that the VAT rate matches the SOK code's expected VAT rate
    /// VAT rates: 0=0%, 1=7%, 2=19%
    /// </summary>
    public async Task<bool> ValidateSokVatRateAsync(string code, short vatRate)
    {
        var sok = await GetSpecialCodeAsync(code);
        if (sok == null)
        {
            _logger.LogWarning("SOK code {Code} not found for VAT rate validation", code);
            return false;
        }

        // If SOK doesn't specify a VAT rate, any rate is acceptable
        if (!sok.VatRate.HasValue)
        {
            return true;
        }

        // VAT rate must match SOK specification
        if (sok.VatRate.Value != vatRate)
        {
            _logger.LogDebug("SOK {Code} VAT rate mismatch. Expected {Expected}, got {Actual}",
                code, sok.VatRate.Value, vatRate);
            return false;
        }

        return true;
    }

    #endregion

    #region Factor Code Lookups (Cached)

    public async Task<IEnumerable<FactorCode>> GetAllFactorCodesAsync()
    {
        if (_cache.TryGetValue<IEnumerable<FactorCode>>(AllFactorCodesCacheKey, out var cachedFactorCodes))
        {
            _logger.LogDebug("All factor codes retrieved from cache");
            return cachedFactorCodes!;
        }

        var factorCodes = await _repository.GetAllFactorCodesAsync();
        _cache.Set(AllFactorCodesCacheKey, factorCodes, CacheExpiration);
        _logger.LogInformation("Cached {Count} factor codes for 24 hours", factorCodes.Count());

        return factorCodes;
    }

    public async Task<FactorCode?> GetFactorCodeAsync(string code)
    {
        var allFactorCodes = await GetAllFactorCodesAsync();
        return allFactorCodes.FirstOrDefault(f => f.Code == code);
    }

    #endregion

    #region Price Code Lookups (Cached)

    public async Task<IEnumerable<PriceCode>> GetAllPriceCodesAsync()
    {
        if (_cache.TryGetValue<IEnumerable<PriceCode>>(AllPriceCodesCacheKey, out var cachedPriceCodes))
        {
            _logger.LogDebug("All price codes retrieved from cache");
            return cachedPriceCodes!;
        }

        var priceCodes = await _repository.GetAllPriceCodesAsync();
        _cache.Set(AllPriceCodesCacheKey, priceCodes, CacheExpiration);
        _logger.LogInformation("Cached {Count} price codes for 24 hours", priceCodes.Count());

        return priceCodes;
    }

    public async Task<PriceCode?> GetPriceCodeAsync(string code)
    {
        var allPriceCodes = await GetAllPriceCodesAsync();
        return allPriceCodes.FirstOrDefault(p => p.Code == code);
    }

    #endregion
}
