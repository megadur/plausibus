using ErezeptValidator.Data;
using ErezeptValidator.Tests.Fixtures;
using FluentAssertions;

namespace ErezeptValidator.Tests.Integration;

/// <summary>
/// Integration tests for Ta1Repository with real database context
/// </summary>
public class Ta1RepositoryIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly ITa1Repository _repository;

    public Ta1RepositoryIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new Ta1Repository(_fixture.Context);
    }

    [Fact]
    public async Task GetSpecialCodeAsync_ExistingCode_ReturnsCode()
    {
        // Arrange
        var sokCode = "09999005";

        // Act
        var result = await _repository.GetSpecialCodeAsync(sokCode);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be(sokCode);
        result.ERezept.Should().Be(1);
        result.IsErezeptCompatible.Should().BeTrue();
    }

    [Fact]
    public async Task GetSpecialCodeAsync_NonExistingCode_ReturnsNull()
    {
        // Arrange
        var nonExistingSokCode = "99999999";

        // Act
        var result = await _repository.GetSpecialCodeAsync(nonExistingSokCode);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetSpecialCodeAsync_NotErezeptCompatible_ReturnsCodeWithERezeptZero()
    {
        // Arrange
        var sokCode = "09999006";

        // Act
        var result = await _repository.GetSpecialCodeAsync(sokCode);

        // Assert
        result.Should().NotBeNull();
        result!.ERezept.Should().Be(0);
        result.IsErezeptCompatible.Should().BeFalse();
    }
}
