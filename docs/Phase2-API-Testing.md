# Phase 2: Code Reference API - Testing Guide

**Date:** 2026-01-31
**Endpoints Created:** 4 (SOK lookup, factors, prices, stats)

---

## Quick Start

### 1. Start the Application

```bash
cd ErezeptValidator
dotnet run
```

The API will start on `https://localhost:7001`

### 2. Open Swagger UI

Navigate to: **https://localhost:7001**

The Swagger UI provides interactive API documentation and testing.

---

## Manual Testing

### Option 1: Swagger UI (Recommended)

1. Open https://localhost:7001 in your browser
2. Expand the **Codes** section
3. Test each endpoint using the "Try it out" button

### Option 2: PowerShell Script

Run the provided test script:

```powershell
.\test-api.ps1
```

This will automatically test all 5 scenarios.

### Option 3: cURL Commands

```bash
# Get statistics
curl -k https://localhost:7001/api/v1/codes/stats

# Get all factor codes
curl -k https://localhost:7001/api/v1/codes/factors

# Get all price codes
curl -k https://localhost:7001/api/v1/codes/prices

# Get specific SOK code
curl -k https://localhost:7001/api/v1/codes/sok/02566958

# Test 404 error handling
curl -k https://localhost:7001/api/v1/codes/sok/99999999
```

---

## API Endpoints

### 1. GET /api/v1/codes/stats

**Description:** Get statistics about loaded reference codes

**Response:**
```json
{
  "totalCodes": 285,
  "specialCodes": {
    "total": 272,
    "sok1": 165,
    "sok2": 107
  },
  "factorCodes": 4,
  "priceCodes": 9,
  "databaseVersion": "TA1-039-2025",
  "lastUpdated": "2026-01-31"
}
```

### 2. GET /api/v1/codes/factors

**Description:** Get all factor codes (Faktor-Kennzeichen)

**Response:**
```json
{
  "count": 4,
  "codes": [
    {
      "code": "11",
      "content": "Anteil in Promille",
      "description": "Share in promille (parts per thousand)",
      "useCase": "Discrete units"
    },
    ...
  ]
}
```

### 3. GET /api/v1/codes/prices

**Description:** Get all price codes (Preis-Kennzeichen)

**Response:**
```json
{
  "count": 9,
  "codes": [
    {
      "code": "11",
      "content": "Apothekeneinkaufspreis nach AMPreisV",
      "description": "Pharmacy purchase price per Drug Price Regulation",
      "taxStatus": "excl. VAT"
    },
    ...
  ]
}
```

### 4. GET /api/v1/codes/sok/{code}

**Description:** Get details for a specific SOK code

**Parameters:**
- `code` (path): 8-digit SOK code (e.g., 02566958)

**Success Response (200 OK):**
```json
{
  "code": "02566958",
  "codeType": "SOK1",
  "description": "Hilfsmittel, die im Zusammenhang mit einer individuell hergestellten Rezeptur abgegeben werden",
  "category": "other",
  "vatRate": 2,
  "vatPercentage": 19,
  "eRezept": 0,
  "eRezeptCompatible": false,
  "pharmacyDiscount": null,
  "requiresAdditionalData": false,
  "validity": {
    "validFromBillingMonth": null,
    "validFromDispensingDate": null,
    "expiredBillingMonth": null,
    "expiredDispensingDate": null,
    "isCurrentlyValid": true
  },
  "issuance": {
    "issuedTo": null,
    "issuedDate": null
  },
  "notes": null,
  "metadata": {
    "createdAt": "2026-01-31T12:00:00Z",
    "updatedAt": "2026-01-31T12:00:00Z"
  }
}
```

**Error Response (404 Not Found):**
```json
{
  "error": "SOK code not found",
  "code": "99999999",
  "message": "SOK code '99999999' does not exist in the TA1 reference database"
}
```

---

## Test Scenarios

### Scenario 1: Valid SOK Code Lookup ✅
- **Endpoint:** GET /api/v1/codes/sok/02566958
- **Expected:** 200 OK with full SOK details
- **Validation:**
  - Response contains code, description, VAT rate
  - E-Rezept compatibility flag present
  - Validity dates (if applicable)
  - Currently valid status calculated correctly

### Scenario 2: Invalid SOK Code Lookup ✅
- **Endpoint:** GET /api/v1/codes/sok/99999999
- **Expected:** 404 Not Found with error message
- **Validation:**
  - Status code is 404
  - Error message explains the code doesn't exist

### Scenario 3: Factor Codes List ✅
- **Endpoint:** GET /api/v1/codes/factors
- **Expected:** 200 OK with 4 factor codes
- **Validation:**
  - Count = 4
  - All codes have: code, content, description, useCase
  - Codes are: 11, 55, 57, 99

### Scenario 4: Price Codes List ✅
- **Endpoint:** GET /api/v1/codes/prices
- **Expected:** 200 OK with 9 price codes
- **Validation:**
  - Count = 9
  - All codes have: code, content, description, taxStatus
  - Codes are: 11, 12, 13, 14, 15, 16, 17, 21, 90

### Scenario 5: Statistics Endpoint ✅
- **Endpoint:** GET /api/v1/codes/stats
- **Expected:** 200 OK with database statistics
- **Validation:**
  - totalCodes = 285 (272 SOK + 4 factor + 9 price)
  - specialCodes.total = 272
  - specialCodes.sok1 = 165
  - specialCodes.sok2 = 107
  - factorCodes = 4
  - priceCodes = 9

---

## Caching Behavior

The Code Reference API uses IMemoryCache with a 24-hour TTL:

- **SOK codes:** Cached individually on first lookup
- **Factor codes:** All 4 codes cached together on first request
- **Price codes:** All 9 codes cached together on first request

### Testing Cache Performance

1. First request (cold cache):
   - Factor codes: ~50-100ms (database query)
   - Price codes: ~50-100ms (database query)

2. Subsequent requests (warm cache):
   - Factor codes: ~1-5ms (memory cache)
   - Price codes: ~1-5ms (memory cache)

Check logs for cache hit/miss information:
```
dbug: ErezeptValidator.Services.CodeLookup.CodeLookupService[0]
      All factor codes retrieved from cache
```

---

## Sample SOK Codes for Testing

Here are some valid SOK codes loaded in the database:

| Code | Type | Description | E-Rezept |
|------|------|-------------|----------|
| 02566958 | SOK1 | Hilfsmittel mit Rezeptur | No (0) |
| 02566993 | SOK1 | Patientenindividuell aus Fertigarzneimittel | Yes (1) |
| 02567001 | SOK1 | BTM-Gebühr | No (0) |
| 02567018 | SOK1 | Noctu-Gebühr | Yes (1) |
| 02567047 | SOK1 | Wiederabgabe von Arzneimitteln | Yes (1) |

Use these codes to test various scenarios (E-Rezept compatible vs. not compatible, etc.)

---

## Troubleshooting

### Application won't start
- Check PostgreSQL is running: `docker ps | grep erezept-postgres`
- Check port 7001 is not in use: `netstat -ano | grep 7001`

### Endpoints return 500 Internal Server Error
- Check application logs for exceptions
- Verify database connection in appsettings.json
- Ensure SOK codes are loaded: `docker exec erezept-postgres psql -U erezept_user -d erezept_validator -c "SELECT COUNT(*) FROM ta1_reference.special_codes;"`

### Cache not working
- Check logs for "retrieved from cache" messages
- Restart application to clear cache
- Verify IMemoryCache is registered in DI container

---

## Next Steps

After Phase 2 testing is complete, proceed to **Phase 3: Validation Engine**:
- Implement 21 validation rules (FMT-001 to CALC-003)
- Create validator classes (FormatValidator, GeneralRuleValidator, CalculationValidator)
- Implement ValidationService orchestrator
- Create validation pipeline with Chain of Responsibility pattern

---

**Testing Status:** Ready for manual testing
**Last Updated:** 2026-01-31
