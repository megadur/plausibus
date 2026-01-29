# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Plausibus: eRezept-Validator** is a .NET 8 (LTS) REST API for validating electronic prescriptions (E-Rezept) against German healthcare regulations including TA1 Version 039, BtMG (controlled substances), and SGB V. The validator helps pharmacy billing centers (ASW) detect compliance errors before submitting to health insurance companies, reducing rejection rates and associated costs.

## Development Commands

### Build and Run
```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API (starts on https://localhost:7001)
dotnet run --project ErezeptValidator

# Run with specific environment
dotnet run --project ErezeptValidator --environment Development
```

### Testing
```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity detailed

# Run specific test project
dotnet test ErezeptValidator.Tests
```

### Database Operations
```bash
# Apply EF Core migrations (TA1 reference database - PostgreSQL)
dotnet ef database update --project ErezeptValidator

# Create new migration
dotnet ef migrations add <MigrationName> --project ErezeptValidator
```

### Docker
```bash
# Build Docker image
docker build -t erezept-validator ./ErezeptValidator

# Run container
docker run -p 8080:8080 erezept-validator
```

## Architecture

### Layered Architecture Pattern

The application follows a clean separation of concerns with three main layers:

**1. Presentation Layer** (`Controllers/`)
- `ValidationController`: Main API endpoint for FHIR Bundle validation (`POST /api/validation/e-rezept`)
- `PznTestController`: Development/testing endpoint for direct PZN lookups (should not be in production)

**2. Data Access Layer** (`Data/`)
- **Dual Database Strategy**:
  - **ABDATA (SQL Server)**: External reference database for pharmaceutical data (PZN lookups, pricing, BTM flags)
    - Uses **Dapper** for high-performance read-only queries
    - 24-hour in-memory caching via `IMemoryCache`
    - Repository: `PznRepository` → `IPznRepository`
  - **TA1 Reference (PostgreSQL)**: Internal application database for validation rules (special codes, factors, price codes)
    - Uses **Entity Framework Core** with migrations
    - Repository: `Ta1Repository` → `ITa1Repository`
    - Context: `Ta1DbContext`

**3. Service Layer** (`Services/`)
- `DataSeeding/DatabaseInitializer`: Seeds TA1 reference tables on startup

### Key Data Models

**ABDATA Models** (`Models/Abdata/`)
- `PacApoArticle`: Maps to ABDATA `PAC_APO` table (pharmaceutical products with PZN, pricing, BTM/Cannabis flags)

**TA1 Reference Models** (`Models/Ta1Reference/`)
- `SpecialCode`: SOK codes (Sonderkennzeichen) from TA3 Anhang 1
- `FactorCode`: Factor identifiers from TA3 Table 8.2.25
- `PriceCode`: Price identifiers from TA3 Table 8.2.26
- `ValidationLog`: Audit trail for validation operations

### FHIR Integration

- Uses **Hl7.Fhir.R4** library for parsing FHIR R4 Bundles
- Main validation flow:
  1. Parse incoming JSON as FHIR Bundle
  2. Extract `MedicationRequest` resources
  3. Validate PZN codes, SOK codes, dosing rules, BTM compliance
  4. Return structured validation results with error/warning lists

### Configuration

Connection strings are in `appsettings.json`:
- `AbdataDatabase`: SQL Server connection to ABDATA (external reference DB)
- `Ta1ReferenceDatabase`: PostgreSQL connection for internal TA1 rules

## Development Status (Per ARCHITECTURE.md Review)

**Implemented:**
- ✅ PZN format and checksum validation (Modulo 11)
- ✅ ABDATA integration with Dapper + caching
- ✅ TA1 reference database structure (EF Core + PostgreSQL)
- ✅ Basic FHIR Bundle parsing
- ✅ API scaffolding with Swagger/OpenAPI

**Missing/In Progress:**
- ⚠️ Validation Engine: Core business logic orchestration (Pipeline/Chain of Responsibility pattern)
- ⚠️ Validators: `BtmValidator`, `CannabisValidator`, `PriceValidator`, `CompoundingValidator`
- ⚠️ Format validators: Timestamp validation (FMT-003, GEN-001)
- ⚠️ Price calculation rules: CALC-001 to CALC-007
- ⚠️ Compounding rules: REZ-xxx series
- ⚠️ Error response format: Standardized error codes per TA1 spec (Section 12.2)

## Important Implementation Notes

### Data Access Patterns

1. **Use Dapper for ABDATA**: The ABDATA database is a large, external, read-only reference database. Continue using Dapper (not EF Core) for performance. Queries are hardcoded in `PznRepository.cs`.

2. **Use EF Core for TA1 Reference**: The internal PostgreSQL database uses EF Core for structured schema management and migrations.

3. **Caching Strategy**: PZN lookups are cached for 24 hours (aligned with ABDATA daily update cycle). Cache key pattern: `"pzn:{normalizedPzn}"`.

4. **Connection String Names**:
   - `"AbdataDatabase"` for SQL Server
   - `"Ta1ReferenceDatabase"` for PostgreSQL

### Security Considerations

- Connection strings contain credentials in `appsettings.json` (use environment variables or Azure Key Vault in production)
- API currently has no authentication (plan: TLS + API keys, future: gematik TI certification)
- DSGVO-compliant logging required (no sensitive patient data in logs)

### Validation Rule Implementation Roadmap

When implementing the Validation Engine, follow this pattern:

1. **Create validator interfaces**: Each validator should implement a common `IValidator` interface
2. **Use Pipeline Pattern**: Chain validators in order: Format → General → Specific → Calculation
3. **Reference TA1 spec**: All rule codes (FMT-001, BTM-001, CALC-001, etc.) are defined in `docs/requirements/Data-Requirements-for-Implementation.md`
4. **Error format**: Return structured errors with rule code, severity (ERROR/WARNING), message, and suggested corrections

### PZN Validation

PZN (Pharmazentralnummer) is an 8-digit pharmaceutical product identifier:
- Format: `^\d{8}$`
- Checksum: Modulo 11 algorithm (implemented in `PznRepository.ValidatePznChecksum()`)
- Normalization: Trim and left-pad with zeros to 8 digits

### Test Data Location

- FHIR E-Rezept examples: `docs/eRezept-Beispiele/`
- TA1 specifications: `docs/requirements/`
- ABDATA integration docs: `docs/ABDATA-Database-Integration-Plan.md`

## Common Pitfalls

1. **Don't use EF Core for ABDATA queries**: This will kill performance. ABDATA is optimized for Dapper.
2. **Cache invalidation**: PZN cache is time-based (24h). Don't try to manually invalidate unless ABDATA is updated mid-day.
3. **FHIR parsing**: Always use `ParserSettings` with `AcceptUnknownMembers = true` and `AllowUnrecognizedEnums = true` to handle variations in E-Rezept formats.
4. **Remove PznTestController**: This is a development endpoint and should not be deployed to production. Move to test project or remove entirely.

## API Endpoints

### Production Endpoints

- `POST /api/validation/e-rezept`: Validate FHIR R4 Bundle (primary endpoint)
  - Accepts: `application/json` (FHIR Bundle)
  - Returns: Validation results with errors/warnings
- `GET /health`: Health check endpoint

### Development Endpoints (Remove in Production)

- `GET /api/pzntest/{pzn}`: Direct PZN lookup (debugging only)

### Swagger UI

- Available at `https://localhost:7001` in Development mode
- Documents all endpoints with request/response schemas

## CI/CD Workflows

The project includes GitHub Actions workflows (in `.github/workflows/`):

- **Main CI/CD Pipeline** (`ci-cd.yml`): Build, test, security scan (CodeQL), Docker publish, deployment
- **PR Validation** (`pr-validation.yml`): Code formatting, security checks, PR size validation, SonarCloud
- **Dependency Updates** (`dependency-update.yml`): Weekly NuGet updates (Mondays 9 AM UTC)
- **Release Management** (`release.yml`): Multi-platform builds, GitHub releases, changelog generation

## External Dependencies

### Required External Systems

1. **ABDATA Database** (SQL Server): Pharmaceutical reference data
   - Version: December 2025 A (ABDATA1225A)
   - Update frequency: Daily (justifies 24h cache)
   - Tables used: `PAC_APO`, `VOV_APO`, `VPV_APO`

2. **TA1 Reference Data**: Code tables from GKV-Spitzenverband/DAV
   - TA3 Table 8.2.25: Factor identifiers (~20 codes, quarterly updates)
   - TA3 Table 8.2.26: Price identifiers (~30 codes, quarterly updates)
   - Anhang 1 & 2: Special codes (SOK)

### Optional Integrations (Future)

- **Lauer-Taxe API**: Alternative pricing source
- **gematik TI**: Telematikinfrastruktur integration (requires certification, 6-12 months)

## Documentation References

- `ARCHITECTURE.md`: Detailed architectural review and gap analysis
- `docs/requirements/Data-Requirements-for-Implementation.md`: Complete data requirements, reference tables, external APIs
- `docs/requirements/product-brief-E-Rezept-Validator-2025-11-02.md`: Product vision and requirements
- `docs/ABDATA-Database-Integration-Plan.md`: ABDATA integration strategy
- `README.md`: Project overview and quick start guide
