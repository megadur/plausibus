# E-Rezept Validator - Validation Rules Implementation Status

**Last Updated:** 2026-02-01
**Total Rules in TA1 Spec:** 67 rules
**Implemented:** 18 rules (27%)
**In Progress:** 0 rules
**Pending:** 49 rules (73%)

---

## Summary by Category

| Category | Total | Implemented | Pending | Status |
|----------|-------|-------------|---------|--------|
| **Format (FMT)** | 10 | 10 | 0 | ✅ Complete |
| **General (GEN)** | 8 | 8 | 0 | ✅ Complete |
| **Calculation (CALC)** | 7 | 7 | 0 | ✅ Complete |
| **BTM** | 4 | 1 | 3 | 🟡 25% |
| **Cannabis (CAN)** | 5 | 0 | 5 | ⭕ 0% |
| **Compounding (REZ)** | 21 | 0 | 21 | ⭕ 0% |
| **Fees (FEE)** | 3 | 0 | 3 | ⭕ 0% |
| **Special Cases (SPC)** | 8 | 0 | 8 | ⭕ 0% |
| **Economic Single Qty (ESQ)** | 4 | 0 | 4 | ⭕ 0% |

---

## Detailed Status

### ✅ Format Validation (FMT) - 10/10 Complete

**Validator:** `FhirFormatValidator.cs`, `PznFormatValidator.cs`

| Rule | Description | Status | Implementation |
|------|-------------|--------|----------------|
| FMT-001 | PZN format validation (8 digits) | ✅ Complete | `PznFormatValidator` |
| FMT-002 | PZN checksum validation (Modulo 11) | ✅ Complete | `PznFormatValidator` |
| FMT-003 | ISO 8601 DateTime format | ✅ Complete | `FhirFormatValidator` |
| FMT-004 | Manufacturer identifier format | ✅ Complete | `FhirFormatValidator` |
| FMT-005 | Counter field formats | ✅ Complete | `FhirFormatValidator` |
| FMT-006 | Batch designation format | ✅ Complete | `FhirFormatValidator` |
| FMT-007 | Factor identifier format | ✅ Complete | `FhirFormatValidator` |
| FMT-008 | Factor value format | ✅ Complete | `FhirFormatValidator` |
| FMT-009 | Price identifier format | ✅ Complete | `FhirFormatValidator` |
| FMT-010 | Price value format | ✅ Complete | `FhirFormatValidator` |

---

### ✅ General Rules (GEN) - 8/8 Complete

**Validator:** `FhirAbgabedatenValidator.cs`

| Rule | Description | Status | Implementation |
|------|-------------|--------|----------------|
| GEN-001 | German time zone (CET/CEST) | ✅ Complete | `FhirAbgabedatenValidator` |
| GEN-002 | Effective date for field changes | ✅ Complete | `FhirAbgabedatenValidator` |
| GEN-003 | Gross price composition | ✅ Complete | `FhirAbgabedatenValidator` |
| GEN-004 | VAT calculation for statutory fees | ✅ Complete | `FhirAbgabedatenValidator` |
| GEN-005 | Special code transmission | ✅ Complete | `FhirAbgabedatenValidator` |
| GEN-006 | SOK validity period check | ✅ Complete | `FhirAbgabedatenValidator` |
| GEN-007 | E-Rezept SOK compatibility | ✅ Complete | `FhirAbgabedatenValidator` |
| GEN-008 | VAT rate consistency | ✅ Complete | `FhirAbgabedatenValidator` |

**Notes:**
- Uses TA1 reference database for SOK code validation
- Temporal validation with dispensing date
- E-Rezept compatibility checking

---

### ✅ Calculation Rules (CALC) - 7/7 Complete

**Validator:** `CalculationValidator.cs`

| Rule | Description | Status | Implementation |
|------|-------------|--------|----------------|
| CALC-001 | Standard Promilleanteil formula | ✅ Complete | `CalculationValidator` |
| CALC-002 | Special code factor exception | ✅ Complete | `CalculationValidator` |
| CALC-003 | Artificial insemination special code | ✅ Complete | `CalculationValidator` |
| CALC-004 | Basic price calculation | ✅ Complete | `CalculationValidator` ⭐ NEW |
| CALC-005 | VAT exclusion in price field | ✅ Complete | `CalculationValidator` ⭐ NEW |
| CALC-006 | Price identifier lookup | ✅ Complete | `PriceIdentifier` value object |
| CALC-007 | Flexible trailing zeros | ✅ Complete | `PromilleFactor` value object |

**Features:**
- Value object pattern (Money, PromilleFactor, Pzn, SokCode, PriceIdentifier)
- ABDATA integration for price calculations
- Tolerance-based decimal comparison (0.000001 for factors, 0.01 EUR for prices)
- Formula: `Price = (Factor / 1000) × Base_Price`

---

### 🟡 BTM Validation - 1/4 (25%)

**Validator:** `BtmDetectionValidator.cs` (basic detection only)

| Rule | Description | Status | Priority |
|------|-------------|--------|----------|
| BTM-001 | E-BTM fee special code | ⭕ Pending | High |
| BTM-002 | All pharmaceuticals must be listed | ⭕ Pending | High |
| BTM-003 | BTM seven-day validity rule | ⭕ Pending | High |
| BTM-004 | BTM diagnosis requirement | ⭕ Pending | Medium |

**Current Implementation:**
- ✅ BTM detection via ABDATA (Btm flag = 2)
- ✅ Basic classification (BTM, Exempt, T-Rezept)
- ⭕ Business logic validation pending

**Next Steps:**
- Implement BTM-001: Validate E-BTM fee (SOK code validation)
- Implement BTM-002: Ensure all items have PZN/SOK codes
- Implement BTM-003: Check prescription date ≤ 7 days old
- Implement BTM-004: Validate diagnosis code presence

---

### ⭕ Cannabis Validation (CAN) - 0/5 (0%)

**Validator:** Not yet implemented

| Rule | Description | Status | Priority |
|------|-------------|--------|----------|
| CAN-001 | Cannabis special codes | ⭕ Pending | High |
| CAN-002 | No BTM/T-Rezept substances | ⭕ Pending | High |
| CAN-003 | Faktor field value | ⭕ Pending | High |
| CAN-004 | Bruttopreis calculation | ⭕ Pending | Medium |
| CAN-005 | Manufacturing data required | ⭕ Pending | Medium |

**Requirements:**
- Cannabis detection via ABDATA (Cannabis flag = 2 or 3)
- Special code validation (SOK codes for cannabis)
- Manufacturing data extraction from FHIR
- Price calculation specific to cannabis

**Data Available:**
- ✅ Cannabis flag in ABDATA PAC_APO table
- ✅ Cannabis detection in `PacApoArticle.IsCannabis`

---

### ⭕ Compounding (REZ) - 0/21 (0%)

**Validator:** Not yet implemented

| Rule | Description | Status | Priority |
|------|-------------|--------|----------|
| REZ-001 | Compounded preparation identification | ⭕ Pending | High |
| REZ-002 | Parenteral - Manufacturer ID | ⭕ Pending | Medium |
| REZ-003 | Parenteral - Timestamp validation | ⭕ Pending | Medium |
| REZ-004 | Parenteral - Counter sequence | ⭕ Pending | Low |
| REZ-005 | Parenteral - Factor as Promilleanteil | ⭕ Pending | Medium |
| REZ-006 | Parenteral - Week supply limit | ⭕ Pending | Medium |
| REZ-007 | ESQ - Manufacturer ID type | ⭕ Pending | Low |
| REZ-008 | ESQ - Timestamp validation | ⭕ Pending | Low |
| REZ-009 | ESQ - Counter for 02567053 | ⭕ Pending | Low |
| REZ-010 | ESQ - Counter for 02566993 | ⭕ Pending | Low |
| REZ-011 | ESQ - Factor identifier | ⭕ Pending | Low |
| REZ-012 | ESQ - Partial quantity factor | ⭕ Pending | Medium |
| REZ-013 | Cannabis/Compounding - Special codes | ⭕ Pending | High |
| REZ-014 | Cannabis/Compounding - Manufacturer ID | ⭕ Pending | Medium |
| REZ-015 | Cannabis/Compounding - Timestamp | ⭕ Pending | Medium |
| REZ-016 | Cannabis/Compounding - Counter values | ⭕ Pending | Low |
| REZ-017 | Cannabis/Compounding - Factor identifier | ⭕ Pending | Medium |
| REZ-018 | Cannabis/Compounding - Factor as Promilleanteil | ⭕ Pending | High |
| REZ-019 | Cannabis/Compounding - Price identifier | ⭕ Pending | High |
| REZ-020 | Cannabis/Compounding - Price adjustment | ⭕ Pending | Medium |
| REZ-021 | Additional data requirement validation | ⭕ Pending | High |

**Partial Implementation:**
- ✅ CALC-005: Basic VAT exclusion check for compounding
- ✅ `SokCode.IsCompounding` property (SOK 06460702, 09999011)

**Next Steps:**
- Create `CompoundingValidator.cs`
- Implement REZ-001, REZ-013, REZ-018, REZ-019, REZ-021 (high priority)
- Full compounding price calculations

---

### ⭕ Fee Validation (FEE) - 0/3 (0%)

**Validator:** Not yet implemented

| Rule | Description | Status | Priority |
|------|-------------|--------|----------|
| FEE-001 | Messenger service fee validation | ⭕ Pending | Medium |
| FEE-002 | Noctu (night service) fee | ⭕ Pending | Medium |
| FEE-003 | Re-procurement fee | ⭕ Pending | Low |

**Requirements:**
- Fee detection via SOK codes
- Statutory fee amounts validation
- VAT adjustment calculations
- Time-based validation (Noctu: 20:00-06:00)

---

### ⭕ Special Cases (SPC) - 0/8 (0%)

**Validator:** Not yet implemented

| Rule | Description | Status | Priority |
|------|-------------|--------|----------|
| SPC-001 | Low-price medication handling | ⭕ Pending | Medium |
| SPC-002 | Additional costs for § 3 Abs. 4 | ⭕ Pending | Medium |
| SPC-003 | Artificial insemination flag | ✅ Partial | High |
| SPC-004 | 50% patient contribution | ⭕ Pending | Medium |
| SPC-005 | Artificial insemination - Compounding | ⭕ Pending | High |
| SPC-006 | Deviation special code | ⭕ Pending | Low |
| SPC-007 | IK format for E-Rezept | ⭕ Pending | Medium |
| SPC-008 | Contract-specific SOK authorization | ⭕ Pending | Low |

**Partial Implementation:**
- ✅ SPC-003: Artificial insemination marker (SOK 09999643) validated in CALC-003

---

### ⭕ Economic Single Quantity (ESQ) - 0/4 (0%)

**Validator:** Not yet implemented

| Rule | Description | Status | Priority |
|------|-------------|--------|----------|
| ESQ-001 | Individual dispensing - Special code | ⭕ Pending | Low |
| ESQ-002 | Individual dispensing - Single unit | ⭕ Pending | Low |
| ESQ-003 | Patient-specific partial quantities | ⭕ Pending | Low |
| ESQ-004 | Weekly blister - Multiple units | ⭕ Pending | Low |

**Requirements:**
- ESQ-specific SOK codes
- Unit quantity validation
- Manufacturer data validation

---

## Implementation Roadmap

### Phase 1: Core Validation ✅ COMPLETE
- [x] Format validation (FMT-001 to FMT-010)
- [x] General rules (GEN-001 to GEN-008)
- [x] Calculation rules (CALC-001 to CALC-007)
- [x] ABDATA integration
- [x] TA1 reference database
- [x] Value objects (Money, PromilleFactor, Pzn, SokCode, PriceIdentifier)

### Phase 2: BTM Validation 🔄 NEXT
**Priority:** High
**Estimated Effort:** 2-3 days

- [ ] BTM-001: E-BTM fee special code
- [ ] BTM-002: All pharmaceuticals must be listed
- [ ] BTM-003: Seven-day validity rule
- [ ] BTM-004: Diagnosis requirement

**Prerequisites:**
- ✅ ABDATA BTM detection available
- ✅ Date handling infrastructure
- ⭕ Diagnosis code extraction from FHIR

### Phase 3: Cannabis Validation 📅 PLANNED
**Priority:** High
**Estimated Effort:** 2-3 days

- [ ] CAN-001: Cannabis special codes
- [ ] CAN-002: No BTM/T-Rezept substances
- [ ] CAN-003: Faktor field value
- [ ] CAN-004: Bruttopreis calculation
- [ ] CAN-005: Manufacturing data required

**Prerequisites:**
- ✅ ABDATA Cannabis detection available
- ⭕ Cannabis-specific SOK codes in database
- ⭕ Manufacturing data extraction

### Phase 4: Compounding Validation 📅 PLANNED
**Priority:** Medium-High
**Estimated Effort:** 5-7 days

**High Priority (REZ-001, 013, 018, 019, 021):**
- [ ] REZ-001: Compounded preparation identification
- [ ] REZ-013: Special codes
- [ ] REZ-018: Factor as Promilleanteil
- [ ] REZ-019: Price identifier
- [ ] REZ-021: Additional data validation

**Medium Priority (Parenteral, ESQ):**
- [ ] REZ-002 to REZ-006: Parenteral preparation rules
- [ ] REZ-007 to REZ-012: Economic single quantity rules
- [ ] REZ-014 to REZ-017: Cannabis/Compounding rules
- [ ] REZ-020: Price adjustment for large quantities

### Phase 5: Fee & Special Cases 📅 PLANNED
**Priority:** Medium
**Estimated Effort:** 2-3 days

- [ ] FEE-001 to FEE-003: Fee validation
- [ ] SPC-001 to SPC-008: Special case handling
- [ ] ESQ-001 to ESQ-004: Economic single quantity

### Phase 6: Integration & Testing 📅 PLANNED
**Priority:** High
**Estimated Effort:** 3-5 days

- [ ] Integration tests with all example bundles
- [ ] End-to-end validation scenarios
- [ ] Performance optimization (<500ms target)
- [ ] Error message refinement
- [ ] Documentation updates

---

## Test Data Coverage

### Available Test Bundles
**Location:** `docs/eRezept-Beispiele/`

| Test Case | Rules Tested | Status |
|-----------|--------------|--------|
| PZN-Verordnung_Nr_1 | FMT, GEN, CALC-001, CALC-004 | ✅ Available |
| PZN-Verordnung_Künstliche_Befruchtung | CALC-003, SPC-003 | ✅ Available |
| Rezeptur-Verordnung_Nr_1 | REZ-xxx, CALC-005 | ✅ Available |
| Rezeptur-parenterale_Zytostatika | REZ-002 to REZ-006 | ✅ Available |
| PZN-Verordnung_Noctu | FEE-002 | ✅ Available |
| Wirkstoff-Verordnung | All categories | ✅ Available |

**Total Test Bundles:** 20+ examples covering various scenarios

---

## Technical Debt & Future Enhancements

### Known Limitations

1. **CALC-005:** Basic implementation only
   - Current: Checks price identifier tax status
   - Future: Full VAT calculation validation with REZ rules

2. **BTM Detection:** Classification only
   - Current: Detects BTM via ABDATA flag
   - Future: Business logic validation (BTM-001 to BTM-004)

3. **PznTestController:** Development endpoint
   - Should be removed or secured for production

### Future Enhancements

1. **Performance Optimization**
   - Batch PZN lookups
   - Parallel validator execution
   - Advanced caching strategies

2. **Error Messages**
   - Standardized error codes per TA1 Section 12.2
   - Suggested corrections
   - Multi-language support

3. **Reporting**
   - Validation statistics
   - Rule coverage reports
   - Performance metrics

4. **Integration**
   - gematik TI integration (6-12 months)
   - Lauer-Taxe API (alternative pricing)
   - Real-time ABDATA updates

---

## References

- **TA1 Version 039:** Technical specification for E-Rezept billing
- **TA3 Tables:** 8.2.25 (Factors), 8.2.26 (Prices)
- **ABDATA:** Pharmaceutical reference database
- **Specification:** `docs/design/TA1-Validation-Rules-Technical-Specification.md`
- **Implementation:** `CALC-004-to-CALC-007-IMPLEMENTATION.md`

---

**Report Generated:** 2026-02-01
**Implementation Progress:** 27% (18/67 rules)
**Next Milestone:** BTM Validation (4 rules)
**Target Completion:** Full validation coverage by Q1 2026
