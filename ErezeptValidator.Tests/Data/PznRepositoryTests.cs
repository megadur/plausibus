using ErezeptValidator.Data;
using ErezeptValidator.Models.Abdata;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;

namespace ErezeptValidator.Tests.Data;

/// <summary>
/// Unit tests for PznRepository
/// </summary>
public class PznRepositoryTests
{
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<ILogger<PznRepository>> _mockLogger;

    public PznRepositoryTests()
    {
        // Create a real IConfiguration with in-memory values for testing
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:AbdataDatabase"] = "Data Source=localhost;Initial Catalog=TestDB;User ID=sa;Password=Test;"
        });
        _configuration = configBuilder.Build();

        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _mockLogger = new Mock<ILogger<PznRepository>>();
    }

    [Fact]
    public void ValidatePznFormat_ValidPzn_ReturnsTrue()
    {
        // Arrange
        var repository = new PznRepository(_configuration, _memoryCache, _mockLogger.Object);
        var validPzn = "12345678";

        // Act
        var result = repository.ValidatePznFormat(validPzn);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("1234567")]     // Too short
    [InlineData("123456789")]   // Too long
    [InlineData("1234567a")]    // Contains letter
    [InlineData("")]            // Empty
    [InlineData(null)]          // Null
    public void ValidatePznFormat_InvalidPzn_ReturnsFalse(string? invalidPzn)
    {
        // Arrange
        var repository = new PznRepository(_configuration, _memoryCache, _mockLogger.Object);

        // Act
        var result = repository.ValidatePznFormat(invalidPzn!);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("00000000", 0)]   // sum=0, 0%11=0 == 0 (valid)
    [InlineData("10000002", 2)]   // sum=(1*2)=2, 2%11=2 == 2 (valid)
    [InlineData("11111112", 2)]   // sum=(1*2)+(1*3)+(1*4)+(1*5)+(1*6)+(1*7)+(1*8)=35, 35%11=2 == 2 (valid)
    [InlineData("12345673", 3)]   // sum=1*2+2*3+3*4+4*5+5*6+6*7+7*8=168, 168%11=3 == 3 (valid)
    [InlineData("12345678", 3)]   // sum=168, 168%11=3 != 8 (invalid)
    public void ValidatePznChecksum_KnownValues_ReturnsExpected(string pzn, int expectedRemainder)
    {
        // Arrange
        var repository = new PznRepository(_configuration, _memoryCache, _mockLogger.Object);

        // Act
        var result = repository.ValidatePznChecksum(pzn);

        // Assert
        // The test is designed to show the algorithm: last digit should equal (sum % 11)
        var lastDigit = pzn[7] - '0';
        if (lastDigit == expectedRemainder)
        {
            result.Should().BeTrue($"PZN {pzn} has correct checksum: last digit {lastDigit} matches remainder {expectedRemainder}");
        }
        else
        {
            result.Should().BeFalse($"PZN {pzn} has incorrect checksum: last digit {lastDigit} doesn't match remainder {expectedRemainder}");
        }
    }

    // Note: NormalizePzn is a private method in PznRepository and cannot be tested directly
    // It is tested indirectly through GetByPznAsync which calls it internally
}
