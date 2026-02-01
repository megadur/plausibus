# CALC-004 to CALC-007 Implementation Summary

**Date:** 2026-02-01
**Status:** ✅ Completed
**Build:** ✅ Successful (0 errors, 3 pre-existing warnings)

---

## Overview

All calculation validation rules (CALC-004 to CALC-007) have been successfully implemented and integrated into the validation pipeline. The implementation follows the value object pattern for type safety and leverages ABDATA integration for price calculations.

---

## Implemented Rules

### ✅ CALC-004: Basic Price Calculation

**Reference:** TA1 Version 039, Section 10.2
**Severity:** ERROR
**Formula:** `Price = (Factor / 1000) × Base_Price_per_PriceIdentifier`

#### Implementation Details

**Files Modified:**
- `Models/ValueObjects/PriceIdentifier.cs`
- `Services/Validation/Validators/CalculationValidator.cs`
- `Models/ValueObjects/Money.cs`

**Key Features:**
1. **Price Identifier Mapping** - Maps TA3 price codes to ABDATA fields:
   ```csharp
   "11" → AEK  // Pharmacy purchase price per AMPreisV
   "13" → AEK  // Actual pharmacy purchase price
   "14" → AVK  // Billing price with surcharges
   ```

2. **ABDATA Integration:**
   - Looks up PZN in ABDATA to retrieve base prices
   - Extracts AEK (ApoEk) or AVK (ApoVk) in cents
   - Converts to Money value object

3. **Calculation:**
   ```csharp
   multiplier = (Factor / 1000)
   expectedPrice = basePrice × multiplier
   ```

4. **Validation:**
   - Compares actual price vs. expected price
   - Tolerance: 0.01 EUR (for rounding differences)
   - Error code: `CALC-004-E` if difference exceeds tolerance

#### Example

```
Base Price (AEK): 100.00 EUR
Factor: 250.000000
Expected: (250 / 1000) × 100 = 25.00 EUR
Actual: 25.01 EUR
Result: ❌ ERROR (difference 0.01 EUR exceeds tolerance)
```

#### Code Location
`CalculationValidator.cs:259-313` - `ValidatePriceCalculationAsync()`

---

### ✅ CALC-005: VAT Exclusion in Price Field

**Reference:** TA1 Version 039, Section 10.2
**Severity:** ERROR
**Applies To:** Compounding preparations (Rezepturen)

#### Implementation Details

**Files Modified:**
- `Models/ValueObjects/SokCode.cs`
- `Services/Validation/Validators/CalculationValidator.cs`

**Key Features:**
1. **Compounding Detection:**
   ```csharp
   public bool IsCompounding => _code is "06460702" or "09999011";
   ```
   - 06460702: Standard compounding
   - 09999011: Alternative compounding

2. **Tax Status Validation:**
   - Checks if price identifier has tax status "excl. VAT"
   - Looks up price code in TA1 reference database
   - Validates compounding prices exclude VAT

3. **Error Code:** `CALC-005-E` if VAT appears to be included

#### Notes
- Basic implementation for MVP
- Full VAT calculation validation deferred to REZ-xxx rules (compounding validation)
- Detects obvious cases where tax status is marked "incl. VAT"

#### Code Location
`CalculationValidator.cs:323-358` - `ValidateVatExclusionAsync()`

---

### ✅ CALC-006: Price Identifier Lookup

**Reference:** TA1 Version 039, Section 10.2
**Severity:** ERROR
**Status:** Already Implemented

#### Verification

**Existing Implementation:**
1. **PriceIdentifier Value Object** (`Models/ValueObjects/PriceIdentifier.cs:28-39`)
   - Validates price codes on creation
   - Valid codes: 11-17, 21, 90
   - Throws exception for invalid codes

2. **GEN-004 Validation** (`FhirAbgabedatenValidator.cs:160-178`)
   - Validates price codes against TA1 database
   - Returns error if code not found in reference data

#### Mapping
- CALC-006 specification requirement → Implemented as GEN-004 + PriceIdentifier validation
- Error handling ensures invalid price codes are caught before calculation

---

### ✅ CALC-007: Flexible Trailing Zeros

**Reference:** TA1 Version 039, Section 10.3
**Severity:** INFO
**Status:** Already Implemented

#### Verification

**Existing Implementation:**
1. **PromilleFactor.EqualsWithinTolerance()** (`Models/ValueObjects/PromilleFactor.cs:76-82`)
   ```csharp
   Tolerance = 0.000001m  // 6 decimal precision
   ```

2. **Normalization Examples:**
   - `1` = `1.0` = `1.000000` ✅ Equal
   - `1000` = `1000.0` = `1000.000000` ✅ Equal

3. **TryParse Method:**
   - Automatically normalizes decimal inputs
   - Stores as micro-units (long) for precision

#### Code Location
`PromilleFactor.cs:73-82` - `EqualsWithinTolerance()`

---

## Enhanced Value Objects

### Money

**New Methods:**
- `Subtract(Money other)` - Subtraction with same currency check
- `Abs(Money value)` - Absolute value for difference calculation
- `Round()` - Already precise to cents (2 decimals)
- Operator `-` for natural subtraction syntax

**Usage in CALC-004:**
```csharp
var difference = Money.Abs(actualPrice - expectedPrice);
if (difference > tolerance) { /* error */ }
```

### PriceIdentifier

**New Properties/Methods:**
- `UsesAbdataBasePrice` - Returns true for codes 11, 13, 14
- `GetAbdataPriceField()` - Maps code to ABDATA column ("AEK" or "AVK")

**Usage:**
```csharp
if (priceId.UsesAbdataBasePrice) {
    var field = priceId.GetAbdataPriceField(); // "AEK" or "AVK"
    var basePrice = GetPriceFromAbdata(field);
}
```

### SokCode

**New Properties:**
- `IsCompounding` - Detects compounding SOK codes (06460702, 09999011)
- Private constant for artificial insemination code

---

## Integration

### Validation Pipeline

The CALC validators are executed as part of `CalculationValidator` in the validation pipeline:

1. **CALC-001** - Standard Promilleanteil formula (existing)
2. **CALC-002** - Special codes without quantity reference (existing)
3. **CALC-003** - Artificial insemination validation (existing)
4. **CALC-004** - **NEW** Basic price calculation
5. **CALC-005** - **NEW** VAT exclusion for compounding
6. **CALC-006** - Already covered by existing validation
7. **CALC-007** - Already covered by tolerance-based comparison

### Execution Order

```
ValidateLineItemCalculationsAsync()
├── Extract PZN, SOK, Factor, Price from FHIR
├── Create value objects (Pzn, SokCode, PromilleFactor, Money, PriceIdentifier)
├── CALC-003: Artificial insemination?
│   └── Yes → Validate and skip other rules
├── CALC-002: Special code without quantity ref?
│   └── Yes → Validate factor = 1.0
├── CALC-001: Standard factor calculation
│   └── Validate (Factor / Package) × 1000
├── CALC-004: Price calculation
│   └── If PZN + PriceId uses ABDATA → Validate price
└── CALC-005: VAT exclusion
    └── If compounding SOK → Validate tax status
```

---

## Testing

### Test Data Available

**Location:** `docs/eRezept-Beispiele/`

1. **Standard PZN Prescription** (CALC-004 test)
   - File: `PZN-Verordnung_Nr_1/PZN_Nr1_eAbgabedaten.xml`
   - Tests: Price calculation with factor and base price

2. **Artificial Insemination** (CALC-003 test)
   - File: `PZN-Verordnung_Künstliche_Befruchtung/PZN-Verordnung_Künstliche_Befruchtung_V1/PZN_KB_V1_eAbgabedaten.xml`
   - Tests: Factor=1000, Price=0.00, PriceId=90

3. **Compounding Prescription** (CALC-005 test)
   - File: `Rezeptur-Verordnung_Nr_1/Rez_Nr1_eAbgabedaten.xml`
   - Tests: VAT exclusion for compounding

### Test Script

**PowerShell Script:** `test-calc-validation.ps1`

**Usage:**
```powershell
# Start API
cd ErezeptValidator
dotnet run

# In another terminal
cd ..
./test-calc-validation.ps1
```

**Features:**
- Tests all 3 validation scenarios
- Displays results with color-coded output
- Shows errors, warnings, and success messages

### Manual API Testing

**Endpoint:** `POST /api/validation/e-rezept`

**Example using curl:**
```bash
curl -k -X POST https://localhost:7001/api/validation/e-rezept \
  -H "Content-Type: application/fhir+xml" \
  --data-binary @docs/eRezept-Beispiele/PZN-Verordnung_Nr_1/PZN_Nr1_eAbgabedaten.xml
```

**Example using PowerShell:**
```powershell
$xml = Get-Content "docs/eRezept-Beispiele/PZN-Verordnung_Nr_1/PZN_Nr1_eAbgabedaten.xml" -Raw
$response = Invoke-WebRequest `
    -Uri "https://localhost:7001/api/validation/e-rezept" `
    -Method POST `
    -Body $xml `
    -ContentType "application/fhir+xml" `
    -SkipCertificateCheck

$response.Content | ConvertFrom-Json | ConvertTo-Json -Depth 10
```

---

## Build Status

### Compilation

```
Build succeeded.

    3 Warning(s)
    0 Error(s)

Time Elapsed 00:00:03.80
```

**Warnings:** 3 pre-existing warnings in `PznTestController.cs` and `PznRepository.cs` (nullable reference types - not related to this implementation)

### Files Changed

| File | Lines Added | Purpose |
|------|-------------|---------|
| PriceIdentifier.cs | +25 | ABDATA price field mapping |
| Money.cs | +30 | Subtraction and absolute value |
| SokCode.cs | +10 | Compounding detection |
| CalculationValidator.cs | +110 | CALC-004 and CALC-005 validation |
| **Total** | **~175** | **4 files modified** |

---

## Next Steps

### Immediate

1. **Run Tests** - Execute `test-calc-validation.ps1` with real FHIR bundles
2. **Verify Results** - Check that CALC-004 correctly calculates prices from ABDATA
3. **Test Edge Cases** - Test with missing PZNs, invalid price identifiers, etc.

### Short-term

4. **Implement BTM Validation** - BTM-001 to BTM-004 (controlled substances)
5. **Implement Cannabis Validation** - CAN-001 to CAN-005
6. **Enhance CALC-005** - Full compounding price validation with REZ-xxx rules

### Medium-term

7. **Integration Testing** - Test with all 20+ example bundles in `docs/eRezept-Beispiele/`
8. **Performance Testing** - Ensure <500ms validation time
9. **Production Cleanup** - Remove or secure `PznTestController`

---

## Technical Notes

### Design Decisions

1. **Value Objects First** - All monetary amounts and factors use type-safe value objects
2. **ABDATA Lookup** - Direct repository access for base prices (cached 24 hours)
3. **Tolerance-Based Comparison** - 0.01 EUR tolerance for floating-point rounding
4. **Early Return Pattern** - Skip validation if required data is missing

### Known Limitations

1. **CALC-005 Basic Implementation** - Full VAT validation deferred to REZ-xxx rules
2. **Contracted Prices** - Price codes 12, 15-17, 21 skip CALC-004 (not in ABDATA)
3. **PZN Lookup Dependency** - CALC-004 requires successful PZN lookup in ABDATA

### Future Enhancements

1. **Festbetrag Support** - Add Festbetrag (fixed price) calculations
2. **Compounding Price Formulas** - Implement REZ-019 and REZ-020 for compounding
3. **Contract Price Validation** - Add support for contracted price calculations

---

## References

- **TA1 Version 039** - Technical Specification for E-Rezept billing
- **TA3 Table 8.2.25** - Factor identifier codes
- **TA3 Table 8.2.26** - Price identifier codes
- **AMPreisV** - German drug pricing regulation (Arzneimittelpreisverordnung)
- **ABDATA** - Pharmaceutical reference database

---

**Implementation Complete** ✅
**Ready for Testing** 🧪
**Build Status** 🟢
**Documentation** ✅
