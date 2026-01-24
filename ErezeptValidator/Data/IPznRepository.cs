using ErezeptValidator.Models.Abdata;

namespace ErezeptValidator.Data;

/// <summary>
/// Repository interface for PZN (Pharmazentralnummer) data access from ABDATA database
/// </summary>
public interface IPznRepository
{
    /// <summary>
    /// Get article information by PZN
    /// </summary>
    /// <param name="pzn">8-digit PZN</param>
    /// <returns>Article information or null if not found</returns>
    Task<PacApoArticle?> GetByPznAsync(string pzn);

    /// <summary>
    /// Get multiple articles by PZNs (batch operation for performance)
    /// </summary>
    /// <param name="pzns">Array of PZNs</param>
    /// <returns>Dictionary of PZN to article information</returns>
    Task<Dictionary<string, PacApoArticle>> GetByPznBatchAsync(string[] pzns);

    /// <summary>
    /// Validate PZN format (8 digits)
    /// </summary>
    /// <param name="pzn">PZN to validate</param>
    /// <returns>True if format is valid</returns>
    bool ValidatePznFormat(string pzn);

    /// <summary>
    /// Validate PZN checksum using Modulo 11 algorithm
    /// </summary>
    /// <param name="pzn">PZN to validate</param>
    /// <returns>True if checksum is valid</returns>
    bool ValidatePznChecksum(string pzn);

    /// <summary>
    /// Check if PZN exists in database
    /// </summary>
    /// <param name="pzn">PZN to check</param>
    /// <returns>True if PZN exists</returns>
    Task<bool> PznExistsAsync(string pzn);

    /// <summary>
    /// Get BTM (controlled substance) status for a PZN
    /// </summary>
    /// <param name="pzn">PZN to check</param>
    /// <returns>BTM indicator value (0=None, 2=BTM, 3=Exempt, 4=T-Rezept)</returns>
    Task<byte?> GetBtmStatusAsync(string pzn);

    /// <summary>
    /// Get Cannabis status for a PZN
    /// </summary>
    /// <param name="pzn">PZN to check</param>
    /// <returns>Cannabis indicator value (0=None, 2=MedCanG §2 Nr.1, 3=MedCanG §2 Nr.2)</returns>
    Task<byte?> GetCannabisStatusAsync(string pzn);

    /// <summary>
    /// Check if article is available on market
    /// </summary>
    /// <param name="pzn">PZN to check</param>
    /// <returns>True if market status is "01" (available)</returns>
    Task<bool> IsAvailableOnMarketAsync(string pzn);

    /// <summary>
    /// Search articles by name (for testing/debugging)
    /// </summary>
    /// <param name="searchTerm">Search term (case-insensitive)</param>
    /// <param name="maxResults">Maximum number of results (default 10)</param>
    /// <returns>List of matching articles</returns>
    Task<List<PacApoArticle>> SearchByNameAsync(string searchTerm, int maxResults = 10);
}
