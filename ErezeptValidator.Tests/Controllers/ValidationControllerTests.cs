using System.Text;
using ErezeptValidator.Controllers;
using ErezeptValidator.Models.Validation;
using ErezeptValidator.Services.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ErezeptValidator.Tests.Controllers;

/// <summary>
/// Unit tests for ValidationController
/// </summary>
public class ValidationControllerTests
{
    private readonly Mock<ValidationPipeline> _mockPipeline;
    private readonly Mock<ILogger<ValidationController>> _mockLogger;
    private readonly ValidationController _controller;

    public ValidationControllerTests()
    {
        _mockPipeline = new Mock<ValidationPipeline>(
            Mock.Of<IEnumerable<IValidator>>(),
            Mock.Of<ILogger<ValidationPipeline>>()
        );
        _mockLogger = new Mock<ILogger<ValidationController>>();
        _controller = new ValidationController(_mockPipeline.Object, _mockLogger.Object);

        // Setup HttpContext for controller
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task ValidateERezept_EmptyBody_ReturnsBadRequest()
    {
        // Arrange
        var emptyBody = new MemoryStream(Encoding.UTF8.GetBytes(""));
        _controller.ControllerContext.HttpContext.Request.Body = emptyBody;
        _controller.ControllerContext.HttpContext.Request.ContentType = "application/json";

        // Act
        var result = await _controller.ValidateERezept();

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidateERezept_WhitespaceOnly_ReturnsBadRequest()
    {
        // Arrange
        var whitespaceBody = new MemoryStream(Encoding.UTF8.GetBytes("   \n\t  "));
        _controller.ControllerContext.HttpContext.Request.Body = whitespaceBody;
        _controller.ControllerContext.HttpContext.Request.ContentType = "application/json";

        // Act
        var result = await _controller.ValidateERezept();

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ValidateERezept_InvalidJson_ReturnsBadRequest()
    {
        // Arrange
        var invalidJson = new MemoryStream(Encoding.UTF8.GetBytes("{invalid json"));
        _controller.ControllerContext.HttpContext.Request.Body = invalidJson;
        _controller.ControllerContext.HttpContext.Request.ContentType = "application/json";

        // Act
        var result = await _controller.ValidateERezept();

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ValidateERezept_NonBundleResource_ReturnsBadRequest()
    {
        // Arrange - FHIR Patient resource instead of Bundle
        var patientJson = @"{
            ""resourceType"": ""Patient"",
            ""id"": ""test-patient""
        }";
        var body = new MemoryStream(Encoding.UTF8.GetBytes(patientJson));
        _controller.ControllerContext.HttpContext.Request.Body = body;
        _controller.ControllerContext.HttpContext.Request.ContentType = "application/json";

        // Act
        var result = await _controller.ValidateERezept();

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.Should().NotBeNull();
        var valueStr = System.Text.Json.JsonSerializer.Serialize(badRequest.Value);
        valueStr.Should().Contain("Invalid resource type");
    }

    [Fact]
    public async Task ValidateERezept_ValidBundle_NoErrors_ReturnsPassStatus()
    {
        // Arrange
        var bundleJson = CreateSampleBundleJson("00285949");
        var body = new MemoryStream(Encoding.UTF8.GetBytes(bundleJson));
        _controller.ControllerContext.HttpContext.Request.Body = body;
        _controller.ControllerContext.HttpContext.Request.ContentType = "application/json";

        // Mock pipeline to return successful validation
        var validationResults = new List<ValidationResult>
        {
            new ValidationResult { ValidatorName = "TestValidator" }
        };
        _mockPipeline.Setup(x => x.ValidateAsync(It.IsAny<Hl7.Fhir.Model.Bundle>()))
            .ReturnsAsync(validationResults);

        // Act
        var result = await _controller.ValidateERezept();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        // Verify the response structure
        var value = okResult!.Value;
        value.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidateERezept_ValidBundle_WithErrors_ReturnsErrorStatus()
    {
        // Arrange
        var bundleJson = CreateSampleBundleJson("12345678");
        var body = new MemoryStream(Encoding.UTF8.GetBytes(bundleJson));
        _controller.ControllerContext.HttpContext.Request.Body = body;
        _controller.ControllerContext.HttpContext.Request.ContentType = "application/json";

        // Mock pipeline to return validation errors
        var validationResult = new ValidationResult { ValidatorName = "PznFormatValidator" };
        validationResult.AddError("FMT-001", "Invalid PZN format", pzn: "12345678");

        var validationResults = new List<ValidationResult> { validationResult };
        _mockPipeline.Setup(x => x.ValidateAsync(It.IsAny<Hl7.Fhir.Model.Bundle>()))
            .ReturnsAsync(validationResults);

        // Act
        var result = await _controller.ValidateERezept();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
        var valueStr = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
        valueStr.Should().Contain("\"status\":\"ERROR\"");
    }

    [Fact]
    public async Task ValidateERezept_ValidBundle_WithWarnings_ReturnsPassStatus()
    {
        // Arrange
        var bundleJson = CreateSampleBundleJson("00285949");
        var body = new MemoryStream(Encoding.UTF8.GetBytes(bundleJson));
        _controller.ControllerContext.HttpContext.Request.Body = body;
        _controller.ControllerContext.HttpContext.Request.ContentType = "application/json";

        // Mock pipeline to return warnings only
        var validationResult = new ValidationResult { ValidatorName = "BtmDetectionValidator" };
        validationResult.AddWarning("BTM-003", "BTM prescription validity warning");

        var validationResults = new List<ValidationResult> { validationResult };
        _mockPipeline.Setup(x => x.ValidateAsync(It.IsAny<Hl7.Fhir.Model.Bundle>()))
            .ReturnsAsync(validationResults);

        // Act
        var result = await _controller.ValidateERezept();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
        var valueStr = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
        valueStr.Should().Contain("\"status\":\"PASS\"");
    }

    [Fact]
    public async Task ValidateERezept_XmlBundle_ParsesSuccessfully()
    {
        // Arrange
        var bundleXml = CreateSampleBundleXml("00285949");
        var body = new MemoryStream(Encoding.UTF8.GetBytes(bundleXml));
        _controller.ControllerContext.HttpContext.Request.Body = body;
        _controller.ControllerContext.HttpContext.Request.ContentType = "application/xml";

        // Mock pipeline
        var validationResults = new List<ValidationResult>
        {
            new ValidationResult { ValidatorName = "TestValidator" }
        };
        _mockPipeline.Setup(x => x.ValidateAsync(It.IsAny<Hl7.Fhir.Model.Bundle>()))
            .ReturnsAsync(validationResults);

        // Act
        var result = await _controller.ValidateERezept();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockPipeline.Verify(x => x.ValidateAsync(It.IsAny<Hl7.Fhir.Model.Bundle>()), Times.Once);
    }

    /// <summary>
    /// Helper method to create a minimal FHIR Bundle JSON for testing
    /// </summary>
    private string CreateSampleBundleJson(string pzn)
    {
        return $@"{{
            ""resourceType"": ""Bundle"",
            ""id"": ""test-bundle-123"",
            ""type"": ""document"",
            ""entry"": [
                {{
                    ""resource"": {{
                        ""resourceType"": ""MedicationRequest"",
                        ""id"": ""test-med-request"",
                        ""status"": ""active"",
                        ""intent"": ""order"",
                        ""medicationCodeableConcept"": {{
                            ""coding"": [
                                {{
                                    ""system"": ""http://fhir.de/CodeSystem/ifa/pzn"",
                                    ""code"": ""{pzn}""
                                }}
                            ]
                        }},
                        ""subject"": {{
                            ""reference"": ""Patient/test-patient""
                        }}
                    }}
                }}
            ]
        }}";
    }

    /// <summary>
    /// Helper method to create a minimal FHIR Bundle XML for testing
    /// </summary>
    private string CreateSampleBundleXml(string pzn)
    {
        return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Bundle xmlns=""http://hl7.org/fhir"">
    <id value=""test-bundle-123""/>
    <type value=""document""/>
    <entry>
        <resource>
            <MedicationRequest>
                <id value=""test-med-request""/>
                <status value=""active""/>
                <intent value=""order""/>
                <medicationCodeableConcept>
                    <coding>
                        <system value=""http://fhir.de/CodeSystem/ifa/pzn""/>
                        <code value=""{pzn}""/>
                    </coding>
                </medicationCodeableConcept>
                <subject>
                    <reference value=""Patient/test-patient""/>
                </subject>
            </MedicationRequest>
        </resource>
    </entry>
</Bundle>";
    }
}
