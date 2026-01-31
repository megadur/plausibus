using ErezeptValidator.Services.CodeLookup;
using Microsoft.AspNetCore.Mvc;

namespace ErezeptValidator.Controllers;

/// <summary>
/// API controller for TA1 reference code lookups (SOK, factor, and price codes)
/// </summary>
[ApiController]
[Route("api/v1/codes")]
[Produces("application/json")]
public class CodeReferenceController : ControllerBase
{
    private readonly ICodeLookupService _codeLookupService;
    private readonly ILogger<CodeReferenceController> _logger;

    public CodeReferenceController(ICodeLookupService codeLookupService, ILogger<CodeReferenceController> logger)
    {
        _codeLookupService = codeLookupService;
        _logger = logger;
    }

    /// <summary>
    /// Get details for a specific SOK (Sonderkennzeichen) code
    /// </summary>
    /// <param name="code">8-digit SOK code (e.g., 02566958)</param>
    /// <returns>SOK code details including description, VAT rate, E-Rezept compatibility, and validity dates</returns>
    /// <response code="200">SOK code found and returned successfully</response>
    /// <response code="404">SOK code not found in database</response>
    [HttpGet("sok/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSokCode(string code)
    {
        _logger.LogDebug("Looking up SOK code: {Code}", code);

        var sokCode = await _codeLookupService.GetSpecialCodeAsync(code);

        if (sokCode == null)
        {
            _logger.LogWarning("SOK code not found: {Code}", code);
            return NotFound(new
            {
                error = "SOK code not found",
                code = code,
                message = $"SOK code '{code}' does not exist in the TA1 reference database"
            });
        }

        _logger.LogInformation("SOK code found: {Code} - {Description}", code, sokCode.Description);

        return Ok(new
        {
            code = sokCode.Code,
            codeType = sokCode.CodeType,
            description = sokCode.Description,
            category = sokCode.Category,
            vatRate = sokCode.VatRate,
            vatPercentage = sokCode.GetVatPercentage(),
            eRezept = sokCode.ERezept,
            eRezeptCompatible = sokCode.IsErezeptCompatible,
            pharmacyDiscount = sokCode.PharmacyDiscount,
            requiresAdditionalData = sokCode.RequiresAdditionalData,
            validity = new
            {
                validFromBillingMonth = sokCode.ValidFromBillingMonth,
                validFromDispensingDate = sokCode.ValidFromDispensingDate,
                expiredBillingMonth = sokCode.ExpiredBillingMonth,
                expiredDispensingDate = sokCode.ExpiredDispensingDate,
                isCurrentlyValid = sokCode.IsValidOnDate(DateOnly.FromDateTime(DateTime.Now))
            },
            issuance = new
            {
                issuedTo = sokCode.IssuedTo,
                issuedDate = sokCode.IssuedDate
            },
            notes = sokCode.Notes,
            metadata = new
            {
                createdAt = sokCode.CreatedAt,
                updatedAt = sokCode.UpdatedAt
            }
        });
    }

    /// <summary>
    /// Get all factor codes (Faktor-Kennzeichen)
    /// </summary>
    /// <returns>List of all factor codes with descriptions and use cases</returns>
    /// <response code="200">Factor codes returned successfully (typically 4 codes)</response>
    [HttpGet("factors")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllFactorCodes()
    {
        _logger.LogDebug("Retrieving all factor codes");

        var factorCodes = await _codeLookupService.GetAllFactorCodesAsync();
        var factorList = factorCodes.ToList();

        _logger.LogInformation("Retrieved {Count} factor codes", factorList.Count);

        return Ok(new
        {
            count = factorList.Count,
            codes = factorList.Select(f => new
            {
                code = f.Code,
                content = f.Content,
                description = f.Description,
                useCase = f.UseCase
            })
        });
    }

    /// <summary>
    /// Get all price codes (Preis-Kennzeichen)
    /// </summary>
    /// <returns>List of all price codes with descriptions and tax status</returns>
    /// <response code="200">Price codes returned successfully (typically 9 codes)</response>
    [HttpGet("prices")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPriceCodes()
    {
        _logger.LogDebug("Retrieving all price codes");

        var priceCodes = await _codeLookupService.GetAllPriceCodesAsync();
        var priceList = priceCodes.ToList();

        _logger.LogInformation("Retrieved {Count} price codes", priceList.Count);

        return Ok(new
        {
            count = priceList.Count,
            codes = priceList.Select(p => new
            {
                code = p.Code,
                content = p.Content,
                description = p.Description,
                taxStatus = p.TaxStatus
            })
        });
    }

    /// <summary>
    /// Get statistics about loaded reference codes
    /// </summary>
    /// <returns>Count of SOK, factor, and price codes in the database</returns>
    /// <response code="200">Statistics returned successfully</response>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCodeStatistics()
    {
        _logger.LogDebug("Retrieving code statistics");

        var sokCount = await _codeLookupService.GetSpecialCodeCountAsync();
        var factorCodes = await _codeLookupService.GetAllFactorCodesAsync();
        var priceCodes = await _codeLookupService.GetAllPriceCodesAsync();

        var sok1Codes = await _codeLookupService.GetSpecialCodesByTypeAsync("SOK1");
        var sok2Codes = await _codeLookupService.GetSpecialCodesByTypeAsync("SOK2");

        var stats = new
        {
            totalCodes = sokCount + factorCodes.Count() + priceCodes.Count(),
            specialCodes = new
            {
                total = sokCount,
                sok1 = sok1Codes.Count(),
                sok2 = sok2Codes.Count()
            },
            factorCodes = factorCodes.Count(),
            priceCodes = priceCodes.Count(),
            databaseVersion = "TA1-039-2025",
            lastUpdated = DateTime.UtcNow.ToString("yyyy-MM-dd")
        };

        _logger.LogInformation("Code statistics: {TotalCodes} total codes", stats.totalCodes);

        return Ok(stats);
    }
}
