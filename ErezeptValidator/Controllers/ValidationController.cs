using System.Text.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using ErezeptValidator.Data;
using Microsoft.AspNetCore.Mvc;

namespace ErezeptValidator.Controllers;

/// <summary>
/// Validation controller for E-Rezept FHIR bundles
/// </summary>
[ApiController]
[Route("api/validation")]
[Produces("application/json")]
public class ValidationController : ControllerBase
{
    private readonly IPznRepository _pznRepository;
    private readonly ITa1Repository _ta1Repository;
    private readonly ILogger<ValidationController> _logger;

    public ValidationController(IPznRepository pznRepository, ITa1Repository ta1Repository, ILogger<ValidationController> logger)
    {
        _pznRepository = pznRepository;
        _ta1Repository = ta1Repository;
        _logger = logger;
    }

    /// <summary>
    /// Validate an E-Rezept FHIR R4 Bundle according to TA1 rules
    /// </summary>
    /// <param name="bundleJson">FHIR Bundle JSON payload</param>
    /// <returns>Detailed validation results</returns>
    [HttpPost("e-rezept")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateERezept([FromBody] JsonElement bundleJson)
    {
        if (bundleJson.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            return BadRequest(new { error = "Invalid payload", message = "Request body must contain a FHIR Bundle JSON document" });
        }

        var rawJson = bundleJson.GetRawText();

        try
        {
            var parser = new FhirJsonParser(new ParserSettings
            {
                AcceptUnknownMembers = true,
                AllowUnrecognizedEnums = true
            });

            var resource = parser.Parse<Resource>(rawJson);

            if (resource is not Bundle bundle)
            {
                return BadRequest(new { error = "Invalid resource type", message = "FHIR resource must be of type Bundle", resourceType = resource.TypeName });
            }

            _logger.LogInformation("Validating E-Rezept bundle: {BundleId} with {EntryCount} entries", bundle.Id, bundle.Entry?.Count ?? 0);

            var validationResults = new List<Dictionary<string, object>>();
            var errors = new List<string>();
            var warnings = new List<string>();

            // Example: Validate MedicationRequests in the bundle
            foreach (var entry in bundle.Entry ?? Enumerable.Empty<Bundle.EntryComponent>())
            {
                if (entry.Resource is MedicationRequest medRequest)
                {
                    var result = await ValidateMedicationRequest(medRequest);
                    validationResults.Add(result);

                    if (result.TryGetValue("errors", out var resultErrorsObj) && resultErrorsObj is List<string> resultErrors && resultErrors.Count > 0)
                    {
                        errors.AddRange(resultErrors);
                    }
                }
            }

            var overallStatus = errors.Any() ? "ERROR" : "PASS";

            return Ok(new
            {
                status = overallStatus,
                bundleId = bundle.Id,
                timestamp = DateTime.UtcNow,
                results = validationResults,
                summary = new
                {
                    errors = errors.Count,
                    warnings = warnings.Count,
                    rulesEvaluated = validationResults.Count
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate E-Rezept bundle");
            return BadRequest(new { error = "Validation failed", message = ex.Message });
        }
    }

    private async Task<Dictionary<string, object>> ValidateMedicationRequest(MedicationRequest medRequest)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        // Extract PZN from medication.code.coding.code (assume first coding is PZN)
        var pzn = (medRequest.Medication as CodeableConcept)?.Coding?.FirstOrDefault()?.Code;
        if (!string.IsNullOrEmpty(pzn))
        {
            // PZN format validation
            if (!_pznRepository.ValidatePznFormat(pzn))
            {
                errors.Add($"FMT-001: Invalid PZN format: {pzn}");
            }
            else
            {
                // PZN lookup
                var article = await _pznRepository.GetByPznAsync(pzn);
                if (article == null)
                {
                    errors.Add($"DATA-001: PZN not found in ABDATA: {pzn}");
                }
                else
                {
                    // BtM validation
                    if (article.IsBtm)
                    {
                        warnings.Add($"BTM-001: BtM product detected: {article.Name}");
                    }
                }
            }
        }

        // Extract SOK code from extensions or notes (placeholder - adjust based on actual FHIR structure)
        var sokCode = "09999005"; // Placeholder - extract from actual FHIR extension
        var specialCode = await _ta1Repository.GetSpecialCodeAsync(sokCode);
        if (specialCode == null)
        {
            errors.Add($"SOK-001: Invalid SOK code: {sokCode}");
        }
        else
        {
            if (specialCode.ERezept == 0)
            {
                errors.Add($"SOK-002: SOK code not E-Rezept compatible: {sokCode}");
            }
        }

        return new Dictionary<string, object>
        {
            ["medicationRequestId"] = medRequest.Id,
            ["pzn"] = pzn,
            ["sokCode"] = sokCode,
            ["errors"] = errors,
            ["warnings"] = warnings
        };
    }
}

