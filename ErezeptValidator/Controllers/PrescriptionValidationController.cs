using ErezeptValidator.Models.Validation;
using ErezeptValidator.Services.Validation;
using Microsoft.AspNetCore.Mvc;

namespace ErezeptValidator.Controllers;

/// <summary>
/// API controller for validating prescriptions according to TA1 Version 039 rules
/// </summary>
[ApiController]
[Route("api/v1/prescriptions")]
[Produces("application/json")]
public class PrescriptionValidationController : ControllerBase
{
    private readonly IValidationService _validationService;
    private readonly ILogger<PrescriptionValidationController> _logger;

    public PrescriptionValidationController(
        IValidationService validationService,
        ILogger<PrescriptionValidationController> logger)
    {
        _validationService = validationService;
        _logger = logger;
    }

    /// <summary>
    /// Validate a prescription according to TA1 Version 039 rules
    /// </summary>
    /// <param name="request">Prescription validation request with line items</param>
    /// <returns>Detailed validation results with errors, warnings, and metadata</returns>
    /// <response code="200">Validation completed successfully (may contain errors in response)</response>
    /// <response code="400">Invalid request format or missing required fields</response>
    /// <response code="500">Internal server error during validation</response>
    /// <remarks>
    /// This endpoint validates prescriptions against TA1 Version 039 rules including:
    /// - Format validations (FMT-001 to FMT-010): PZN format, checksums, data types
    /// - General rules (GEN-001 to GEN-008): SOK codes, factor/price codes, E-Rezept compatibility
    /// - Calculation rules (CALC-001 to CALC-003): Price and factor calculations
    ///
    /// The response will always return HTTP 200 OK if validation completes, even if the prescription
    /// contains errors. Check the `validationResult` field in the response:
    /// - "PASS": No validation errors found
    /// - "FAIL": Validation errors found (see `errors` array)
    /// - "INCOMPLETE": Validation stopped early due to critical errors
    ///
    /// Example request:
    /// ```json
    /// {
    ///   "prescriptionId": "Task/160.000.000.012.345.67",
    ///   "dispensingDate": "2026-01-31T14:30:00+01:00",
    ///   "isErezept": true,
    ///   "lineItems": [
    ///     {
    ///       "lineNumber": 1,
    ///       "pzn": "01234567",
    ///       "quantity": 1,
    ///       "grossPrice": 8.35,
    ///       "vatRate": 2,
    ///       "priceCode": "11",
    ///       "priceValue": 5.50
    ///     }
    ///   ]
    /// }
    /// ```
    /// </remarks>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(PrescriptionValidationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ValidatePrescription([FromBody] PrescriptionValidationRequest request)
    {
        _logger.LogInformation("Received validation request for prescription {PrescriptionId} with {LineItemCount} line items",
            request?.PrescriptionId ?? "unknown", request?.LineItems?.Count ?? 0);

        // Validate request model
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid request model for prescription {PrescriptionId}", request?.PrescriptionId ?? "unknown");
            return BadRequest(new
            {
                error = "Invalid request",
                message = "Request validation failed. Check required fields and data types.",
                details = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            });
        }

        if (request == null)
        {
            _logger.LogWarning("Null request received");
            return BadRequest(new
            {
                error = "Invalid request",
                message = "Request body is required"
            });
        }

        try
        {
            // Execute validation
            var response = await _validationService.ValidateAsync(request);

            // Log result summary
            _logger.LogInformation(
                "Validation completed for {PrescriptionId}: Result={Result}, Errors={ErrorCount}, Warnings={WarningCount}, Rules={RulesChecked}, Duration={DurationMs}ms",
                request.PrescriptionId,
                response.ValidationResult,
                response.Errors.Count,
                response.Warnings.Count,
                response.Summary.TotalRulesChecked,
                response.Summary.DurationMs);

            // Return response (always 200 OK, validation result in response body)
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error validating prescription {PrescriptionId}", request.PrescriptionId);

            return StatusCode(500, new
            {
                error = "Internal server error",
                message = "An unexpected error occurred during validation",
                prescriptionId = request.PrescriptionId,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Get validation service health and statistics
    /// </summary>
    /// <returns>Health status and validation engine information</returns>
    /// <response code="200">Service is healthy</response>
    [HttpGet("validation/health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetValidationHealth()
    {
        return Ok(new
        {
            status = "healthy",
            service = "Prescription Validation Service",
            version = "1.0.0-mvp",
            rulesVersion = "TA1-039-2025",
            capabilities = new
            {
                formatValidation = true,
                generalRules = true,
                calculationRules = true,
                sokCodeLookup = true,
                factorCodeLookup = true,
                priceCodeLookup = true
            },
            validators = new[]
            {
                new { name = "FormatValidator", rules = 10, order = 1 },
                new { name = "GeneralRuleValidator", rules = 8, order = 2 },
                new { name = "CalculationValidator", rules = 3, order = 3 }
            },
            totalRules = 21,
            timestamp = DateTime.UtcNow
        });
    }
}
