using System.Data;
using System.Text.RegularExpressions;
using Dapper;
using ErezeptValidator.Models.Abdata;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;

namespace ErezeptValidator.Data;

/// <summary>
/// Repository implementation for PZN data access using Dapper for performance
/// Includes in-memory caching with 24-hour TTL
/// </summary>
public partial class PznRepository : IPznRepository
{
    private readonly string _connectionString;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PznRepository> _logger;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(24);

    [GeneratedRegex(@"^\d{8}$")]
    private static partial Regex PznFormatRegex();

    public PznRepository(IConfiguration configuration, IMemoryCache cache, ILogger<PznRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("AbdataDatabase")
            ?? throw new InvalidOperationException("AbdataDatabase connection string not found");
        _cache = cache;
        _logger = logger;
    }

    public async Task<PacApoArticle?> GetByPznAsync(string pzn)
    {
        // Normalize PZN (trim, pad with zeros if needed)
        pzn = NormalizePzn(pzn);

        if (!ValidatePznFormat(pzn))
        {
            _logger.LogWarning("Invalid PZN format: {Pzn}", pzn);
            return null;
        }

        // Check cache first
        var cacheKey = $"pzn:{pzn}";
        if (_cache.TryGetValue<PacApoArticle>(cacheKey, out var cachedArticle))
        {
            _logger.LogDebug("Cache hit for PZN: {Pzn}", pzn);
            return cachedArticle;
        }

        // Query database
        try
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
                SELECT
                    PZN as Pzn,
                    Name,
                    Langname_ungekuerzt as LangnameUngekuerzt,
                    BTM as Btm,
                    Cannabis,
                    Apo_Ek as ApoEk,
                    Apo_Vk as ApoVk,
                    ApU,
                    ApU_78_3a_1_AMG as ApU_78_3a_1_Amg,
                    Festbetrag,
                    Verkehrsstatus,
                    Rezeptpflicht,
                    MwSt,
                    Lifestyle,
                    TFG as Tfg,
                    Apopflicht
                FROM PAC_APO
                WHERE PZN = @Pzn";

            var article = await connection.QuerySingleOrDefaultAsync<PacApoArticle>(sql, new { Pzn = pzn });

            if (article != null)
            {
                // Cache the result
                _cache.Set(cacheKey, article, _cacheExpiration);
                _logger.LogInformation("Retrieved PZN {Pzn} from database: {Name}", pzn, article.Name);
            }
            else
            {
                _logger.LogWarning("PZN not found in database: {Pzn}", pzn);
            }

            return article;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving PZN {Pzn} from database", pzn);
            throw;
        }
    }

    public async Task<Dictionary<string, PacApoArticle>> GetByPznBatchAsync(string[] pzns)
    {
        var normalizedPzns = pzns.Select(NormalizePzn).Where(ValidatePznFormat).Distinct().ToArray();

        if (normalizedPzns.Length == 0)
        {
            return new Dictionary<string, PacApoArticle>();
        }

        var result = new Dictionary<string, PacApoArticle>();

        // Check cache for each PZN
        var uncachedPzns = new List<string>();
        foreach (var pzn in normalizedPzns)
        {
            var cacheKey = $"pzn:{pzn}";
            if (_cache.TryGetValue<PacApoArticle>(cacheKey, out var cachedArticle))
            {
                result[pzn] = cachedArticle;
            }
            else
            {
                uncachedPzns.Add(pzn);
            }
        }

        // Fetch uncached PZNs from database
        if (uncachedPzns.Count > 0)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var sql = @"
                    SELECT
                        PZN as Pzn,
                        Name,
                        Langname_ungekuerzt as LangnameUngekuerzt,
                        BTM as Btm,
                        Cannabis,
                        Apo_Ek as ApoEk,
                        Apo_Vk as ApoVk,
                        ApU,
                        ApU_78_3a_1_AMG as ApU_78_3a_1_Amg,
                        Festbetrag,
                        Verkehrsstatus,
                        Rezeptpflicht,
                        MwSt,
                        Lifestyle,
                        TFG as Tfg,
                        Apopflicht
                    FROM PAC_APO
                    WHERE PZN IN @Pzns";

                var articles = await connection.QueryAsync<PacApoArticle>(sql, new { Pzns = uncachedPzns });

                foreach (var article in articles)
                {
                    result[article.Pzn] = article;

                    // Cache each article
                    var cacheKey = $"pzn:{article.Pzn}";
                    _cache.Set(cacheKey, article, _cacheExpiration);
                }

                _logger.LogInformation("Retrieved {Count} PZNs from database (batch)", articles.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving batch PZNs from database");
                throw;
            }
        }

        return result;
    }

    public bool ValidatePznFormat(string pzn)
    {
        if (string.IsNullOrWhiteSpace(pzn))
            return false;

        // Must be exactly 8 digits
        if (!PznFormatRegex().IsMatch(pzn))
            return false;

        // Check reserved ranges (internal use only per ABDATA documentation)
        if (int.TryParse(pzn[..7], out int pznNumber))
        {
            // Ranges 0800000-0839999 and 8000000-8399999 are reserved
            if ((pznNumber >= 80000 && pznNumber <= 83999) ||
                (pznNumber >= 800000 && pznNumber <= 839999))
            {
                return false;
            }
        }

        return true;
    }

    public bool ValidatePznChecksum(string pzn)
    {
        if (!ValidatePznFormat(pzn))
            return false;

        // PZN Checksum: Modulo 11 algorithm
        // Weights: 2, 3, 4, 5, 6, 7, 8 for first 7 digits
        int[] weights = { 2, 3, 4, 5, 6, 7, 8 };
        int sum = 0;

        for (int i = 0; i < 7; i++)
        {
            sum += (pzn[i] - '0') * weights[i];
        }

        int checksum = sum % 11;
        int expectedCheckDigit = pzn[7] - '0';

        return checksum == expectedCheckDigit;
    }

    public async Task<bool> PznExistsAsync(string pzn)
    {
        var article = await GetByPznAsync(pzn);
        return article != null;
    }

    public async Task<byte?> GetBtmStatusAsync(string pzn)
    {
        var article = await GetByPznAsync(pzn);
        return article?.Btm;
    }

    public async Task<byte?> GetCannabisStatusAsync(string pzn)
    {
        var article = await GetByPznAsync(pzn);
        return article?.Cannabis;
    }

    public async Task<bool> IsAvailableOnMarketAsync(string pzn)
    {
        var article = await GetByPznAsync(pzn);
        return article?.IsAvailableOnMarket ?? false;
    }

    public async Task<List<PacApoArticle>> SearchByNameAsync(string searchTerm, int maxResults = 10)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new List<PacApoArticle>();

        try
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"
                SELECT TOP (@MaxResults)
                    PZN as Pzn,
                    Name,
                    Langname_ungekuerzt as LangnameUngekuerzt,
                    BTM as Btm,
                    Cannabis,
                    Apo_Ek as ApoEk,
                    Apo_Vk as ApoVk,
                    ApU,
                    ApU_78_3a_1_AMG as ApU_78_3a_1_Amg,
                    Festbetrag,
                    Verkehrsstatus,
                    Rezeptpflicht,
                    MwSt,
                    Lifestyle,
                    TFG as Tfg,
                    Apopflicht
                FROM PAC_APO
                WHERE Name LIKE '%' + @SearchTerm + '%'
                   OR Langname_ungekuerzt LIKE '%' + @SearchTerm + '%'
                ORDER BY Name";

            var articles = await connection.QueryAsync<PacApoArticle>(sql, new { SearchTerm = searchTerm, MaxResults = maxResults });

            return articles.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching articles by name: {SearchTerm}", searchTerm);
            throw;
        }
    }

    private static string NormalizePzn(string pzn)
    {
        if (string.IsNullOrWhiteSpace(pzn))
            return string.Empty;

        // Remove whitespace and leading zeros, then pad to 8 digits
        pzn = pzn.Trim();

        if (pzn.Length < 8 && int.TryParse(pzn, out _))
        {
            pzn = pzn.PadLeft(8, '0');
        }

        return pzn;
    }
}
