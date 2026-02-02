using ErezeptValidator.Data;
using ErezeptValidator.Models.Abdata;
using ErezeptValidator.Models.Validation;
using ErezeptValidator.Services.Validation.Validators;
using FluentAssertions;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Task = System.Threading.Tasks.Task;

namespace ErezeptValidator.Tests.Validators;

/// <summary>
/// Unit tests for CannabisValidator (CAN-001 to CAN-005)
/// </summary>
public class CannabisValidatorTests
{
    private readonly Mock<IPznRepository> _mockPznRepository;
    private readonly Mock<ILogger<CannabisValidator>> _mockLogger;
    private readonly CannabisValidator _validator;

    // Valid Cannabis SOK codes from TA1 Annex 10
    private readonly string[] _validCannabisSokCodes =
        { "06461446", "06461423", "06460665", "06460694", "06460748", "06460754" };

    public CannabisValidatorTests()
    {
        _mockPznRepository = new Mock<IPznRepository>();
        _mockLogger = new Mock<ILogger<CannabisValidator>>();
        _validator = new CannabisValidator(_mockPznRepository.Object, _mockLogger.Object);
    }

    #region CAN-001: Cannabis special codes

    [Fact]
    public async Task CAN001_ValidCannabisSokCode_NoError()
    {
        // TODO: Arrange
        // - Create context with Cannabis medication (Cannabis flag = 2 or 3)
        // - Include valid SOK code (one of: 06461446, 06461423, etc.)

        // TODO: Act
        // var result = await _validator.ValidateAsync(context);

        // TODO: Assert
        // result.IsValid.Should().BeTrue();
        // result.Errors.Should().NotContain(e => e.Code == "CAN-001");
    }

    [Theory]
    [InlineData("06461446")]
    [InlineData("06461423")]
    [InlineData("06460665")]
    [InlineData("06460694")]
    [InlineData("06460748")]
    [InlineData("06460754")]
    public async Task CAN001_AllValidCannabisSokCodes_NoError(string sokCode)
    {
        // TODO: Test each valid Cannabis SOK code individually
        // Expected: No CAN-001 error for any valid code
    }

    [Fact]
    public async Task CAN001_InvalidCannabisSokCode_ReturnsError()
    {
        // TODO: Test Cannabis medication with invalid SOK code
        // Expected: Error CAN-001 "Ungültiger Cannabis-Sonderkennzeichen"
    }

    [Fact]
    public async Task CAN001_MissingCannabisSokCode_ReturnsError()
    {
        // TODO: Test Cannabis medication without any SOK code
        // Expected: Error CAN-001
    }

    #endregion

    #region CAN-002: No BTM/T-Rezept substances

    [Fact]
    public async Task CAN002_CannabisWithoutBtmSubstances_NoError()
    {
        // TODO: Arrange
        // - Cannabis preparation with Cannabis flag = 2 or 3
        // - All ingredients have Btm flag = 0 (no BTM)

        // TODO: Act & Assert
        // result.Errors.Should().NotContain(e => e.Code == "CAN-002");
    }

    [Fact]
    public async Task CAN002_CannabisWithBtmSubstance_ReturnsError()
    {
        // TODO: Test Cannabis preparation containing BTM substance (Btm = 2)
        // Expected: Error CAN-002 "BTM-Substanz in Cannabis-Zubereitung"
    }

    [Fact]
    public async Task CAN002_CannabisWithTRezeptSubstance_ReturnsError()
    {
        // TODO: Test Cannabis preparation containing T-Rezept substance (Btm = 4)
        // Expected: Error CAN-002 "T-Rezept-Substanz in Cannabis-Zubereitung"
    }

    [Fact]
    public async Task CAN002_CannabisBtmMutualExclusion_ReturnsError()
    {
        // TODO: Test that Cannabis and BTM are mutually exclusive
        // A preparation cannot be both Cannabis and BTM simultaneously
    }

    #endregion

    #region CAN-003: Faktor field value

    [Fact]
    public async Task CAN003_FactorEqualsOne_NoError()
    {
        // TODO: Arrange
        // - Cannabis SOK code line with Factor = 1 (or 1.000000)

        // TODO: Act & Assert
        // result.Errors.Should().NotContain(e => e.Code == "CAN-003");
    }

    [Fact]
    public async Task CAN003_FactorNotOne_ReturnsError()
    {
        // TODO: Test Cannabis SOK line with Factor != 1
        // Expected: Error CAN-003 "Faktor muss 1 sein für Cannabis-Sonderkennzeichen"
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.5)]
    [InlineData(2)]
    [InlineData(1.5)]
    public async Task CAN003_InvalidFactorValues_ReturnsError(decimal factor)
    {
        // TODO: Test various invalid factor values
        // Expected: Error CAN-003 for all non-1 values
    }

    #endregion

    #region CAN-004: Bruttopreis calculation

    [Fact]
    public async Task CAN004_ValidBruttopreisCalculation_NoError()
    {
        // TODO: Arrange
        // - Cannabis preparation with price calculation per AMPreisV
        // - Gross price includes all components from Annex 10 pricing tables

        // TODO: Act & Assert
        // result.Errors.Should().NotContain(e => e.Code == "CAN-004");
    }

    [Fact]
    public async Task CAN004_IncorrectBruttopreis_ReturnsError()
    {
        // TODO: Test Cannabis with incorrect gross price calculation
        // Expected: Error CAN-004 "Bruttopreis entspricht nicht AMPreisV"
    }

    [Fact]
    public async Task CAN004_MissingPriceComponents_ReturnsError()
    {
        // TODO: Test Cannabis missing required price components
        // Expected: Error CAN-004
    }

    #endregion

    #region CAN-005: Manufacturing data required

    [Fact]
    public async Task CAN005_CompleteManufacturingData_NoError()
    {
        // TODO: Arrange
        // - Cannabis preparation with complete Herstellungssegment:
        //   * Manufacturer ID (Herstellerkennzeichen)
        //   * Timestamp (Herstellungszeitstempel)
        //   * Counter (Zähler)
        //   * Batch designation (Chargenbezeichnung)

        // TODO: Act & Assert
        // result.Errors.Should().NotContain(e => e.Code == "CAN-005");
    }

    [Fact]
    public async Task CAN005_MissingManufacturerId_ReturnsError()
    {
        // TODO: Test Cannabis without manufacturer ID
        // Expected: Error CAN-005
    }

    [Fact]
    public async Task CAN005_MissingTimestamp_ReturnsError()
    {
        // TODO: Test Cannabis without manufacturing timestamp
        // Expected: Error CAN-005
    }

    [Fact]
    public async Task CAN005_MissingCounter_ReturnsError()
    {
        // TODO: Test Cannabis without counter value
        // Expected: Error CAN-005
    }

    [Fact]
    public async Task CAN005_MissingBatchDesignation_ReturnsError()
    {
        // TODO: Test Cannabis without batch designation
        // Expected: Error CAN-005
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Creates a test ValidationContext with Cannabis medications
    /// </summary>
    private ValidationContext CreateCannabisContext(
        string pzn,
        bool isCannabis,
        string? sokCode = null,
        decimal? factor = null,
        bool includeManufacturingData = true)
    {
        var context = new ValidationContext
        {
            Bundle = new Bundle { Id = "test-bundle" },
            BundleType = BundleType.Abgabedaten
        };

        // TODO: Add MedicationDispense, Invoice resources with Cannabis data

        return context;
    }

    /// <summary>
    /// Creates a mock Cannabis article for ABDATA lookup
    /// </summary>
    private PacApoArticle CreateCannabisArticle(string pzn, byte cannabisFlag = 2, byte btmFlag = 0)
    {
        return new PacApoArticle
        {
            Pzn = pzn,
            Name = $"Test Cannabis Product {pzn}",
            Btm = btmFlag,
            Cannabis = cannabisFlag // 2 = Cannabis § 31(6) SGB V, 3 = Cannabis preparation
        };
    }

    #endregion
}
