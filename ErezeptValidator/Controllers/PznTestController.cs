using ErezeptValidator.Data;
using Microsoft.AspNetCore.Mvc;

namespace ErezeptValidator.Controllers;

/// <summary>
/// DEVELOPMENT ONLY: Test controller for PZN lookup and ABDATA database connectivity.
/// This controller is automatically disabled in Production environments.
/// </summary>
/// <remarks>
/// WARNING: This controller exposes internal database details and should never be deployed to production.
/// It is automatically excluded from the API in non-Development environments.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
#if !DEBUG
[ApiExplorerSettings(IgnoreApi = true)]
#endif
public class PznTestController : ControllerBase
{
    private readonly IPznRepository _pznRepository;
    private readonly ILogger<PznTestController> _logger;
    private readonly IWebHostEnvironment _environment;

    public PznTestController(
        IPznRepository pznRepository,
        ILogger<PznTestController> logger,
        IWebHostEnvironment environment)
    {
        _pznRepository = pznRepository;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Checks if the controller is available in the current environment
    /// </summary>
    private IActionResult? CheckEnvironment()
    {
        if (!_environment.IsDevelopment())
        {
            _logger.LogWarning("PznTestController accessed in non-Development environment: {Environment}", _environment.EnvironmentName);
            return NotFound(new
            {
                error = "Endpoint not available",
                message = "This endpoint is only available in Development environments",
                environment = _environment.EnvironmentName
            });
        }
        return null;
    }

    /// <summary>
    /// Get article information by PZN
    /// </summary>
    /// <param name="pzn">8-digit PZN (e.g., 00123456)</param>
    /// <returns>Article information from ABDATA</returns>
    [HttpGet("{pzn}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByPzn(string pzn)
    {
        var envCheck = CheckEnvironment();
        if (envCheck != null) return envCheck;

        _logger.LogInformation("Received request for PZN: {Pzn}", pzn);

        if (!_pznRepository.ValidatePznFormat(pzn))
        {
            return BadRequest(new
            {
                error = "Invalid PZN format",
                message = "PZN must be exactly 8 digits",
                pzn
            });
        }

        var article = await _pznRepository.GetByPznAsync(pzn);

        if (article == null)
        {
            return NotFound(new
            {
                error = "PZN not found",
                message = $"PZN {pzn} not found in ABDATA database",
                pzn
            });
        }

        var checksumValid = _pznRepository.ValidatePznChecksum(pzn);

        return Ok(new
        {
            pzn = article.Pzn,
            name = article.Name,
            longName = article.LangnameUngekuerzt,
            pricing = new
            {
                aekEuros = article.ApoEkEuros,
                avkEuros = article.ApoVkEuros,
                festbetragEuros = article.FestbetragEuros,
                vatRate = $"{article.VatRatePercentage}%"
            },
            classification = new
            {
                isBtm = article.IsBtm,
                isBtmExempt = article.IsBtmExempt,
                isTRezept = article.IsTRezept,
                isCannabis = article.IsCannabis,
                isLifestyleMedication = article.IsLifestyleMedication,
                requiresPrescription = article.RequiresPrescription,
                requiresBtmPrescription = article.RequiresBtmPrescription
            },
            marketStatus = new
            {
                code = article.Verkehrsstatus,
                isAvailable = article.IsAvailableOnMarket
            },
            validation = new
            {
                formatValid = true,
                checksumValid
            }
        });
    }

    /// <summary>
    /// Validate PZN format and checksum
    /// </summary>
    /// <param name="pzn">PZN to validate</param>
    /// <returns>Validation result</returns>
    [HttpGet("validate/{pzn}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ValidatePzn(string pzn)
    {
        var envCheck = CheckEnvironment();
        if (envCheck != null) return envCheck;

        var formatValid = _pznRepository.ValidatePznFormat(pzn);
        var checksumValid = formatValid && _pznRepository.ValidatePznChecksum(pzn);

        return Ok(new
        {
            pzn,
            formatValid,
            checksumValid,
            valid = formatValid && checksumValid,
            errors = new List<string>
            {
                !formatValid ? "Invalid format (must be 8 digits)" : null,
                formatValid && !checksumValid ? "Invalid checksum (Modulo 11 failed)" : null
            }.Where(e => e != null).ToList()
        });
    }

    /// <summary>
    /// Search articles by name
    /// </summary>
    /// <param name="q">Search query</param>
    /// <param name="limit">Maximum results (default 10)</param>
    /// <returns>List of matching articles</returns>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int limit = 10)
    {
        var envCheck = CheckEnvironment();
        if (envCheck != null) return envCheck;

        if (string.IsNullOrWhiteSpace(q) || q.Length < 3)
        {
            return BadRequest(new
            {
                error = "Search query too short",
                message = "Please provide at least 3 characters for search"
            });
        }

        var articles = await _pznRepository.SearchByNameAsync(q, Math.Min(limit, 50));

        return Ok(new
        {
            query = q,
            count = articles.Count,
            results = articles.Select(a => new
            {
                pzn = a.Pzn,
                name = a.Name,
                avkEuros = a.ApoVkEuros,
                isBtm = a.IsBtm,
                isCannabis = a.IsCannabis,
                isAvailable = a.IsAvailableOnMarket
            })
        });
    }

    /// <summary>
    /// Get batch of articles by multiple PZNs
    /// </summary>
    /// <param name="pzns">Comma-separated list of PZNs</param>
    /// <returns>Dictionary of PZN to article information</returns>
    [HttpPost("batch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBatch([FromBody] string[] pzns)
    {
        var envCheck = CheckEnvironment();
        if (envCheck != null) return envCheck;

        if (pzns == null || pzns.Length == 0)
        {
            return BadRequest(new
            {
                error = "No PZNs provided",
                message = "Please provide an array of PZNs"
            });
        }

        if (pzns.Length > 100)
        {
            return BadRequest(new
            {
                error = "Too many PZNs",
                message = "Maximum 100 PZNs per batch request"
            });
        }

        var articles = await _pznRepository.GetByPznBatchAsync(pzns);

        return Ok(new
        {
            requested = pzns.Length,
            found = articles.Count,
            notFound = pzns.Except(articles.Keys).ToList(),
            articles = articles.Select(kvp => new
            {
                pzn = kvp.Key,
                name = kvp.Value.Name,
                avkEuros = kvp.Value.ApoVkEuros,
                isBtm = kvp.Value.IsBtm,
                isCannabis = kvp.Value.IsCannabis
            })
        });
    }

    /// <summary>
    /// Test database connection
    /// </summary>
    /// <returns>Connection status</returns>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> HealthCheck()
    {
        var envCheck = CheckEnvironment();
        if (envCheck != null) return envCheck;

        try
        {
            // Try to search for a common medication (should exist in any ABDATA database)
            var testArticles = await _pznRepository.SearchByNameAsync("Aspirin", 1);

            return Ok(new
            {
                status = "healthy",
                database = "ABDATA connected",
                timestamp = DateTime.UtcNow,
                testQuery = testArticles.Count > 0 ? "Success" : "No results (database might be empty)"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");

            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "unhealthy",
                database = "ABDATA connection failed",
                error = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
