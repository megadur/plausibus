# ErezeptValidator.Tests

Comprehensive test suite for the E-Rezept Validator API.

## Test Statistics

- **Total Tests**: 29
- **Status**: ✅ All Passing
- **Coverage**: Unit tests, Integration tests, and Helpers

## Test Structure

### Unit Tests

#### `Data/PznRepositoryTests.cs`
- PZN format validation
- PZN checksum validation (Modulo 11 algorithm)
- Tests for valid/invalid PZN formats
- Tests for reserved PZN ranges

#### `Models/PznValidationTests.cs`
- Standalone PZN checksum validation logic
- Test data with known valid/invalid PZNs
- Algorithm explanation and documentation

#### `Controllers/ValidationControllerTests.cs`
- API endpoint validation tests
- FHIR Bundle parsing tests
- PZN validation integration
- SOK code validation
- BTM (controlled substances) detection
- Error handling and response formats

### Integration Tests

#### `Integration/Ta1RepositoryIntegrationTests.cs`
- Real database context tests using in-memory EF Core
- Special code (SOK) lookup
- E-Rezept compatibility checks
- Data seeding validation

### Test Helpers

#### `Helpers/FhirBundleBuilder.cs`
- Fluent builder for creating test FHIR Bundles
- Supports MedicationRequest, Patient, Practitioner resources
- Simplifies test data creation

#### `Fixtures/DatabaseFixture.cs`
- xUnit class fixture for database tests
- Seeds test data (Special Codes, Price Codes, Factor Codes)
- Uses in-memory database for fast, isolated tests

### Test Data

#### `TestData/sample-erezept-bundle.json`
- Complete E-Rezept FHIR R4 Bundle example
- Includes Composition, MedicationRequest, Patient, Practitioner, Organization
- Can be used for manual testing and integration tests

## Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity detailed

# Run specific test class
dotnet test --filter "FullyQualifiedName~PznRepositoryTests"

# Run tests with code coverage
dotnet test /p:CollectCoverage=true
```

## Test Dependencies

- **xUnit**: Test framework
- **FluentAssertions**: Fluent assertion library for readable tests
- **Moq**: Mocking framework for dependencies
- **Microsoft.AspNetCore.Mvc.Testing**: For integration testing
- **Microsoft.EntityFrameworkCore.InMemory**: In-memory database for fast tests

## Writing New Tests

### Unit Test Example

```csharp
[Fact]
public void MyMethod_ValidInput_ReturnsExpectedResult()
{
    // Arrange
    var service = new MyService();
    var input = "test";

    // Act
    var result = service.MyMethod(input);

    // Assert
    result.Should().Be("expected");
}
```

### Integration Test Example

```csharp
public class MyIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public MyIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task MyMethod_WithDatabase_ReturnsData()
    {
        // Arrange
        var repository = new MyRepository(_fixture.Context);

        // Act
        var result = await repository.GetDataAsync();

        // Assert
        result.Should().NotBeNull();
    }
}
```

## Test Coverage Goals

- **Unit Tests**: Cover all business logic and validation rules
- **Integration Tests**: Cover database operations and repository patterns
- **Controller Tests**: Cover API endpoints and HTTP responses
- **Edge Cases**: Invalid inputs, null checks, boundary conditions

## Continuous Integration

Tests are automatically run in the GitHub Actions CI/CD pipeline:
- On every push to main/develop
- On every pull request
- Before deployment

## Future Test Additions

- [ ] Validation Engine tests (when implemented)
- [ ] BTM-specific validation tests
- [ ] Price calculation tests (CALC-001 to CALC-007)
- [ ] Compounding prescription tests (Rezeptur)
- [ ] Performance tests for batch PZN lookups
- [ ] End-to-end API tests with TestServer
