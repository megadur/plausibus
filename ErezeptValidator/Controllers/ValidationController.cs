using System.Text.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using ErezeptValidator.Data;
using ErezeptValidator.Services.Validation;
using ErezeptValidator.Models.Validation;
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
    private readonly ValidationPipeline _validationPipeline;
    private readonly ILogger<ValidationController> _logger;

    public ValidationController(ValidationPipeline validationPipeline, ILogger<ValidationController> logger)
    {
        _validationPipeline = validationPipeline;
        _logger = logger;
    }

    /// <summary>
    /// Validate an E-Rezept FHIR R4 Bundle according to TA1 rules
    /// </summary>
    /// <returns>Detailed validation results</returns>
    [HttpPost("e-rezept")]
    [Consumes("application/json", "application/fhir+json", "application/xml", "application/fhir+xml", "text/xml")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateERezept()
    {
        string bundleContent;

        // Read raw request body
        using (var reader = new StreamReader(Request.Body))
        {
            bundleContent = await reader.ReadToEndAsync();
        }

        if (string.IsNullOrWhiteSpace(bundleContent))
        {
            return BadRequest(new { error = "Invalid payload", message = "Request body must contain a FHIR Bundle document (JSON or XML)" });
        }

        try
        {
            var contentType = Request.ContentType?.ToLowerInvariant() ?? "application/json";
            var isXml = contentType.Contains("xml");

            var parserSettings = new ParserSettings
            {
                AcceptUnknownMembers = true,
                AllowUnrecognizedEnums = true
            };

            Resource resource;
            if (isXml)
            {
                var xmlParser = new Hl7.Fhir.Serialization.FhirXmlParser(parserSettings);
                resource = xmlParser.Parse<Resource>(bundleContent);
            }
            else
            {
                var jsonParser = new FhirJsonParser(parserSettings);
                resource = jsonParser.Parse<Resource>(bundleContent);
            }

            if (resource is not Bundle bundle)
            {
                return BadRequest(new { error = "Invalid resource type", message = "FHIR resource must be of type Bundle", resourceType = resource.TypeName });
            }

            _logger.LogInformation("Validating E-Rezept bundle: {BundleId} with {EntryCount} entries", bundle.Id, bundle.Entry?.Count ?? 0);

            // Execute validation pipeline
            var validationResults = await _validationPipeline.ValidateAsync(bundle);

            // Aggregate issues from all validators
            var allIssues = validationResults.SelectMany(r => r.Issues).ToList();
            var errors = allIssues.Where(i => i.Severity == ValidationSeverity.Error).ToList();
            var warnings = allIssues.Where(i => i.Severity == ValidationSeverity.Warning).ToList();
            var infos = allIssues.Where(i => i.Severity == ValidationSeverity.Info).ToList();

            var overallStatus = errors.Any() ? "ERROR" : "PASS";

            return Ok(new
            {
                status = overallStatus,
                bundleId = bundle.Id,
                timestamp = DateTime.UtcNow,
                validationResults = validationResults.Select(r => new
                {
                    validator = r.ValidatorName,
                    isValid = r.IsValid,
                    errorCount = r.ErrorCount,
                    warningCount = r.WarningCount,
                    infoCount = r.InfoCount,
                    issues = r.Issues.Select(i => new
                    {
                        code = i.Code,
                        severity = i.Severity.ToString(),
                        message = i.Message,
                        field = i.Field,
                        pzn = i.Pzn,
                        details = i.Details
                    })
                }),
                summary = new
                {
                    errors = errors.Count,
                    warnings = warnings.Count,
                    infos = infos.Count,
                    validatorsExecuted = validationResults.Count
                },
                errors = errors.Select(e => new { code = e.Code, message = e.Message, pzn = e.Pzn }),
                warnings = warnings.Select(w => new { code = w.Code, message = w.Message, pzn = w.Pzn }),
                infos = infos.Select(i => new { code = i.Code, message = i.Message, pzn = i.Pzn })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate E-Rezept bundle");
            return BadRequest(new { error = "Validation failed", message = ex.Message });
        }
    }
}

