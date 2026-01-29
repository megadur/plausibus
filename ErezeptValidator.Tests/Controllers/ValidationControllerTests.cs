using System.Text.Json;
using ErezeptValidator.Controllers;
using ErezeptValidator.Data;
using ErezeptValidator.Models.Abdata;
using ErezeptValidator.Models.Ta1Reference;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ErezeptValidator.Tests.Controllers;

/// <summary>
/// Unit tests for ValidationController
/// </summary>
public class ValidationControllerTests
{
    private readonly Mock<IPznRepository> _mockPznRepository;
    private readonly Mock<ITa1Repository> _mockTa1Repository;
    private readonly Mock<ILogger<ValidationController>> _mockLogger;
    private readonly ValidationController _controller;

    public ValidationControllerTests()
    {
        _mockPznRepository = new Mock<IPznRepository>();
        _mockTa1Repository = new Mock<ITa1Repository>();
        _mockLogger = new Mock<ILogger<ValidationController>>();
        _controller = new ValidationController(
            _mockPznRepository.Object,
            _mockTa1Repository.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task ValidateERezept_NullPayload_ReturnsBadRequest()
    {
        // Arrange
        var nullJson = JsonDocument.Parse("null").RootElement;

        // Act
        var result = await _controller.ValidateERezept(nullJson);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidateERezept_InvalidJson_ReturnsBadRequest()
    {
        // Arrange
        var invalidJson = JsonDocument.Parse("{}").RootElement;

        // Act
        var result = await _controller.ValidateERezept(invalidJson);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ValidateERezept_NonBundleResource_ReturnsBadRequest()
    {
        // Arrange - FHIR Patient resource instead of Bundle
        var patientJson = JsonDocument.Parse(@"{
            ""resourceType"": ""Patient"",
            ""id"": ""test-patient""
        }").RootElement;

        // Act
        var result = await _controller.ValidateERezept(patientJson);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.Should().NotBeNull();
        var responseJson = JsonSerializer.Serialize(badRequest.Value);
        responseJson.Should().Contain("error");
    }

    [Fact]
    public async Task ValidateERezept_ValidBundle_InvalidPzn_ReturnsErrorStatus()
    {
        // Arrange
        _mockPznRepository.Setup(x => x.ValidatePznFormat(It.IsAny<string>())).Returns(false);

        var bundleJson = CreateSampleBundleJson("12345678");

        // Act
        var result = await _controller.ValidateERezept(bundleJson);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var responseJson = JsonSerializer.Serialize(okResult!.Value);
        responseJson.Should().Contain("ERROR");
        responseJson.Should().Contain("FMT-001");
    }

    [Fact]
    public async Task ValidateERezept_ValidBundle_ValidPzn_NotFoundInDb_ReturnsError()
    {
        // Arrange
        _mockPznRepository.Setup(x => x.ValidatePznFormat(It.IsAny<string>())).Returns(true);
        _mockPznRepository.Setup(x => x.GetByPznAsync(It.IsAny<string>())).ReturnsAsync((PacApoArticle?)null);

        var bundleJson = CreateSampleBundleJson("00285949");

        // Act
        var result = await _controller.ValidateERezept(bundleJson);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var responseJson = JsonSerializer.Serialize(okResult!.Value);
        responseJson.Should().Contain("DATA-001");
    }

    [Fact]
    public async Task ValidateERezept_ValidBundle_BtmProduct_ReturnsWarning()
    {
        // Arrange
        _mockPznRepository.Setup(x => x.ValidatePznFormat(It.IsAny<string>())).Returns(true);
        _mockPznRepository.Setup(x => x.GetByPznAsync(It.IsAny<string>())).ReturnsAsync(new PacApoArticle
        {
            Pzn = "00285949",
            Name = "Test BTM Product",
            Btm = 2,  // BTM indicator (IsBtm computed property will be true)
            Cannabis = 0
        });

        var bundleJson = CreateSampleBundleJson("00285949");

        // Act
        var result = await _controller.ValidateERezept(bundleJson);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var responseJson = JsonSerializer.Serialize(okResult!.Value);
        responseJson.Should().Contain("BTM-001");
    }

    [Fact]
    public async Task ValidateERezept_InvalidSokCode_ReturnsError()
    {
        // Arrange
        _mockPznRepository.Setup(x => x.ValidatePznFormat(It.IsAny<string>())).Returns(true);
        _mockPznRepository.Setup(x => x.GetByPznAsync(It.IsAny<string>())).ReturnsAsync(new PacApoArticle
        {
            Pzn = "00285949",
            Name = "Test Product"
        });
        _mockTa1Repository.Setup(x => x.GetSpecialCodeAsync(It.IsAny<string>())).ReturnsAsync((SpecialCode?)null);

        var bundleJson = CreateSampleBundleJson("00285949");

        // Act
        var result = await _controller.ValidateERezept(bundleJson);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var responseJson = JsonSerializer.Serialize(okResult!.Value);
        responseJson.Should().Contain("SOK-001");
    }

    [Fact]
    public async Task ValidateERezept_SokNotErezeptCompatible_ReturnsError()
    {
        // Arrange
        _mockPznRepository.Setup(x => x.ValidatePznFormat(It.IsAny<string>())).Returns(true);
        _mockPznRepository.Setup(x => x.GetByPznAsync(It.IsAny<string>())).ReturnsAsync(new PacApoArticle
        {
            Pzn = "00285949",
            Name = "Test Product"
        });
        _mockTa1Repository.Setup(x => x.GetSpecialCodeAsync(It.IsAny<string>())).ReturnsAsync(new SpecialCode
        {
            Code = "09999005",
            Description = "Test SOK",
            CodeType = "SOK1",
            ERezept = 0 // Not E-Rezept compatible
        });

        var bundleJson = CreateSampleBundleJson("00285949");

        // Act
        var result = await _controller.ValidateERezept(bundleJson);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var responseJson = JsonSerializer.Serialize(okResult!.Value);
        responseJson.Should().Contain("SOK-002");
    }

    /// <summary>
    /// Helper method to create a minimal FHIR Bundle JSON for testing
    /// </summary>
    private JsonElement CreateSampleBundleJson(string pzn)
    {
        var bundleJsonString = $@"{{
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
                        }}
                    }}
                }}
            ]
        }}";

        return JsonDocument.Parse(bundleJsonString).RootElement;
    }
}
