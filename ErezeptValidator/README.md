# E-Rezept Validator

API for validating E-Rezept prescriptions according to German TA1 (Technische Anlage 1) specifications.

## Features

- ✅ **ABDATA Integration**: Direct access to ABDATA ARTIKELSTAMM production database
- ✅ **PZN Validation**: Format and checksum validation using Modulo 11 algorithm
- ✅ **BTM/Cannabis Classification**: Automatic detection of controlled substances and cannabis medications
- ✅ **Pricing Data**: Real-time pharmacy purchase/sales prices from ABDATA
- ✅ **Caching**: 24-hour in-memory cache for performance optimization
- ✅ **Swagger UI**: Interactive API documentation and testing

## Quick Start

### Prerequisites

- .NET 8.0 SDK
- Access to ABDATA database (already configured)

### Run the Application

```bash
cd ErezeptValidator
dotnet restore
dotnet run
```

The API will be available at:
- **Swagger UI**: https://localhost:7001 (or http://localhost:5000)
- **API Base**: https://localhost:7001/api

## API Endpoints

### Health Check
```http
GET /health
```
Check if the service and database connection are healthy.

### Database Health Check
```http
GET /api/PznTest/health
```
Test ABDATA database connectivity.

### Get Article by PZN
```http
GET /api/PznTest/{pzn}
```
Get complete article information from ABDATA.

**Example:**
```bash
curl https://localhost:7001/api/PznTest/00123456
```

**Response:**
```json
{
  "pzn": "00123456",
  "name": "Example Medication",
  "longName": "Example Medication 500mg Tablets",
  "pricing": {
    "aekEuros": 5.50,
    "avkEuros": 8.35,
    "festbetragEuros": 6.50,
    "vatRate": "19%"
  },
  "classification": {
    "isBtm": false,
    "isBtmExempt": false,
    "isTRezept": false,
    "isCannabis": false,
    "isLifestyleMedication": false,
    "requiresPrescription": true,
    "requiresBtmPrescription": false
  },
  "marketStatus": {
    "code": "01",
    "isAvailable": true
  },
  "validation": {
    "formatValid": true,
    "checksumValid": true
  }
}
```

### Validate PZN
```http
GET /api/PznTest/validate/{pzn}
```
Validate PZN format and checksum without database lookup.

**Example:**
```bash
curl https://localhost:7001/api/PznTest/validate/00123456
```

### Search Articles by Name
```http
GET /api/PznTest/search?q={searchTerm}&limit={maxResults}
```
Search for articles by name (min 3 characters).

**Example:**
```bash
curl "https://localhost:7001/api/PznTest/search?q=Aspirin&limit=5"
```

### Batch PZN Lookup
```http
POST /api/PznTest/batch
Content-Type: application/json

["00123456", "00234567", "00345678"]
```
Get multiple articles in a single request (max 100 PZNs).

## Database Configuration

The ABDATA database connection is configured in [appsettings.json](appsettings.json):

```json
{
  "ConnectionStrings": {
    "AbdataDatabase": "Data Source=127.0.0.1;Initial Catalog=ABDATA1225A;..."
  },
  "AbdataSettings": {
    "DatabaseName": "ABDATA1225A",
    "Version": "December 2025 A",
    "CacheExpirationHours": 24
  }
}
```

## ABDATA Tables

The validator uses the following ABDATA tables:

### PAC_APO (Primary)
Article base information including:
- PZN (primary key)
- Name and long name
- BTM/Cannabis classification
- Pricing (AEK, AVK, Festbetrag)
- Market status
- Prescription requirements

### Planned for Future
- **VOV_APO**: Prescription requirements
- **VPV_APO**: Article-prescription mapping
- **GRU_APO**: Rebate groups

## Architecture

```
┌─────────────────────────────────────┐
│  PznTestController (API Layer)      │
└─────────────┬───────────────────────┘
              │
              ▼
┌─────────────────────────────────────┐
│  IPznRepository (Data Access)       │
│  - PznRepository (Implementation)   │
└─────────────┬───────────────────────┘
              │
              ├──→ IMemoryCache (24h TTL)
              │
              ▼
┌─────────────────────────────────────┐
│  ABDATA ARTIKELSTAMM Database       │
│  (SQL Server - PAC_APO table)       │
└─────────────────────────────────────┘
```

## Development

### Project Structure
```
ErezeptValidator/
├── Program.cs                   # Application entry point
├── appsettings.json            # Configuration
├── Controllers/
│   └── PznTestController.cs    # PZN test API endpoints
├── Data/
│   ├── IPznRepository.cs       # Repository interface
│   └── PznRepository.cs        # Repository implementation (Dapper)
└── Models/
    └── Abdata/
        └── PacApoArticle.cs    # ABDATA PAC_APO entity model
```

### Key Technologies
- **ASP.NET Core 8.0** - Web API framework
- **Dapper** - Micro-ORM for high-performance SQL queries
- **Microsoft.Data.SqlClient** - SQL Server connectivity
- **IMemoryCache** - In-memory caching
- **Swagger/OpenAPI** - API documentation

## Testing

### Test Database Connection
```bash
curl https://localhost:7001/api/PznTest/health
```

Expected response:
```json
{
  "status": "healthy",
  "database": "ABDATA connected",
  "timestamp": "2026-01-24T12:00:00Z",
  "testQuery": "Success"
}
```

### Test PZN Lookup
Find a valid PZN first:
```bash
curl "https://localhost:7001/api/PznTest/search?q=Paracetamol&limit=1"
```

Then lookup that PZN:
```bash
curl https://localhost:7001/api/PznTest/{pzn-from-search}
```

## Next Steps

- [ ] Implement full TA1 validation rules (72 rules documented)
- [ ] Add FHIR R4 E-Rezept parsing
- [ ] Implement BTM validation (BTM-001 through BTM-004)
- [ ] Implement Cannabis validation (CAN-001 through CAN-005)
- [ ] Add compounded preparations validation (REZ-001 through REZ-020)
- [ ] Integrate with gematik FHIR profiles
- [ ] Create comprehensive test suite

## Documentation

- **[TA1 Technical Specification](../docs/TA1-Validation-Rules-Technical-Specification.md)** - Complete validation rules
- **[ABDATA Integration Plan](../docs/ABDATA-Database-Integration-Plan.md)** - Database integration details
- **[Data Requirements](../docs/Data-Requirements-for-Implementation.md)** - Data needs analysis

## License

Proprietary - ASW Genossenschaft

## Contact

For questions or issues, contact the development team.
