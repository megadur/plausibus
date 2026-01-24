# Documentation Gap Analysis

**Date:** 2026-01-24
**Purpose:** Identify overlaps, gaps, and required updates between existing and new documentation

---

## Documents Analyzed

1. **TA1-Validation-Rules-Technical-Specification.md** (1,512 lines)
   - Focus: E-Rezept validation business logic rules
   - Scope: Format validations, BTM, Cannabis, Compounding, Calculations

2. **CODE_STRUCTURES.md** (NEW - 600+ lines)
   - Focus: Code definitions and reference catalogs
   - Scope: Factor codes, Price codes, SOK1/SOK2 catalogs, Cross-code rules

3. **VALIDATION_EXAMPLES.md** (NEW - 600+ lines)
   - Focus: Practical validation scenarios
   - Scope: 16 detailed examples with inputs/outputs, workflow diagrams

---

## Overlap Analysis

### 1. Faktorkennzeichen (Factor Codes)

| Document | Coverage | Details |
|----------|----------|---------|
| **TA1** | Format validation only | FMT-007: Format rules (2 digits)<br>Appendix A.2: Lists codes 11, 55, 57 |
| **CODE_STRUCTURES** | Complete definitions | Section 1: All 4 codes (11, 55, 57, **99**)<br>Full descriptions and use cases |

**Overlap:** PARTIAL - Different aspects (format vs. definitions)
**Gap:** ⚠️ TA1 missing Factor code **99** (waste/disposal)

---

### 2. Preiskennzeichen (Price Codes)

| Document | Coverage | Details |
|----------|----------|---------|
| **TA1** | Format validation only | FMT-009: Format rules (2 digits)<br>Appendix A.3: Lists codes 11, 13, 14, 15, 90 |
| **CODE_STRUCTURES** | Complete definitions | Section 2: All 8 codes (11-17, 21)<br>Full descriptions with tax status |

**Overlap:** PARTIAL - Different aspects (format vs. definitions)
**Gap:** ⚠️ TA1 missing Price codes **12, 16, 17, 21**

---

### 3. Sonderkennzeichen (Special Codes)

| Document | Coverage | Details |
|----------|----------|---------|
| **TA1** | Usage rules, scattered references | Specific codes mentioned in context:<br>- BTM-001: 02567001<br>- Cannabis: 06461446, 06461423, etc.<br>Appendix A.1: ~25 codes listed |
| **CODE_STRUCTURES** | Complete catalog | Section 3: 172 SOK1 codes<br>Section 4: 109 SOK2 codes<br>Organized by category with all fields |

**Overlap:** PARTIAL - TA1 has business logic, CODE_STRUCTURES has catalog
**Gap:** ⚠️⚠️⚠️ **MAJOR** - TA1 has incomplete special code catalog (25 vs 281 codes)

---

### 4. Validation Rules

| Rule Type | TA1 Coverage | CODE_STRUCTURES Coverage | Overlap |
|-----------|--------------|--------------------------|---------|
| **Format Validations** | ✅ Complete (FMT-xxx) | ❌ Not covered | NONE - Complementary |
| **Temporal Validation** | ⚠️ Basic (GEN-002) | ✅ Detailed (Section 6.1) | PARTIAL - Needs enhancement |
| **Authorization** | ⚠️ IK only (SPC-007) | ✅ SOK2 authorization (Section 6.2) | PARTIAL - Missing contract validation |
| **Cross-Code Rules** | ❌ Not explicitly defined | ✅ Complete (Section 5) | NONE - New content |
| **VAT Consistency** | ⚠️ Scattered mentions | ✅ Systematic (Section 5.3) | PARTIAL - Needs consolidation |
| **E-Rezept Compatibility** | ❌ Not defined | ✅ Defined (Section 5.4) | NONE - New rule |
| **Business Logic** | ✅ Complete (BTM, CAN, REZ, etc.) | ❌ Not covered | NONE - Complementary |

---

### 5. Error Messages

| Document | Coverage | Error Categories |
|----------|----------|------------------|
| **TA1** | Section 12 | FMT-xxx, GEN-xxx, BTM-xxx, REZ-xxx, CAN-xxx, CALC-xxx, SPC-xxx, FEE-xxx |
| **CODE_STRUCTURES** | Section 7 | SOK_xxx, FACTOR_xxx, PRICE_xxx, VAT_xxx, EREZEPT_xxx |

**Overlap:** PARTIAL - Different error categories
**Gap:** ⚠️ Error codes should be consolidated into a single catalog

---

### 6. Practical Examples

| Document | Coverage |
|----------|----------|
| **TA1** | ❌ No practical examples |
| **VALIDATION_EXAMPLES** | ✅ 16 detailed scenarios |

**Overlap:** NONE - VALIDATION_EXAMPLES is entirely new content

---

## Critical Gaps Identified

### 🔴 CRITICAL - TA1 Document Needs Updates

#### 1. Missing Factor Code 99
**Location:** TA1 Appendix A.2
**Issue:** Factor code 99 (waste/disposal) not documented
**Impact:** Validation logic incomplete for waste scenarios
**Fix Required:**
```diff
### A.2 Factor Identifier Codes (TA3 8.2.25)

| Code | Description |
|------|-------------|
| 11 | Standard factor (Promilleanteil) |
| 55 | Dose in milligrams (for substitution) |
| 57 | Alternative dose specification |
+ | 99 | Package share in promille (waste/disposal) |
```

#### 2. Missing Price Codes 12, 16, 17, 21
**Location:** TA1 Appendix A.3
**Issue:** Incomplete price code catalog
**Impact:** Cannot validate certain pricing scenarios (contract prices, mg-prices)
**Fix Required:**
```diff
### A.3 Price Identifier Codes (TA3 8.2.26)

| Code | Description |
|------|-------------|
| 11 | Pharmacy purchase price per AMPreisV |
+ | 12 | Price agreed between pharmacy and manufacturer |
| 13 | Actual pharmacy purchase price |
| 14 | Billing price per AMPreisV §§4,5 (with surcharges) |
| 15 | Contracted billing price (pharmacy-insurance agreement) |
+ | 16 | Contract prices per § 129a SGB V |
+ | 17 | Billing price "Preis 2" per mg-price directory |
+ | 21 | Discount contract price "Preis 1" per § 130a Abs. 8c |
| 90 | Special price (e.g., 0.00 for markers) |
```

#### 3. Incomplete Special Code Catalog
**Location:** TA1 Appendix A.1
**Issue:** Only ~25 codes listed, missing 256+ codes
**Impact:** Cannot validate most SOK scenarios
**Fix Required:**
```diff
### A.1 Special Codes by Category

+ **NOTE:** This is a summary of frequently used codes.
+ For the complete catalog of 172 SOK1 codes and 109 SOK2 codes,
+ see [CODE_STRUCTURES.md](./CODE_STRUCTURES.md) Sections 3 and 4.

| Category | Code | Description |
|----------|------|-------------|
...existing content...
```

#### 4. Missing SOK Temporal Validation Rules
**Location:** TA1 Section 2.2 or new section
**Issue:** No validation for SOK validity dates
**Impact:** May accept expired or not-yet-valid SOKs
**Fix Required:** Add new validation rules
```
#### Rule GEN-006: SOK Validity Period Check
```
Severity: ERROR
Condition: SOK must be valid at dispensing date
Fields: SOK, Dispensing date
Implementation:
- Retrieve SOK from reference table
- Check: dispensing_date >= SOK.valid_from
- Check: SOK.valid_until IS NULL OR dispensing_date <= SOK.valid_until
- Error if outside validity period
Reference: CODE_STRUCTURES.md Section 6.1
```
```

#### 5. Missing SOK Authorization Validation
**Location:** TA1 Section 9 (Special Case Validations)
**Issue:** No validation for contract-specific SOK authorization
**Impact:** Pharmacies may use SOK codes they're not authorized for
**Fix Required:** Add new validation rules
```
#### Rule SPC-008: Contract-Specific SOK Authorization
```
Severity: ERROR
Condition: Pharmacy must be authorized to use SOK2 codes
Fields: SOK, Pharmacy association
Implementation:
- If SOK is in SOK2 range (contract-specific)
- Retrieve SOK.assigned_to from reference table
- Verify pharmacy association matches
- Error if unauthorized
Reference: CODE_STRUCTURES.md Section 6.2
```
```

#### 6. Missing E-Rezept Compatibility Validation
**Location:** TA1 Section 2 or 9
**Issue:** No validation for SOK E-Rezept compatibility
**Impact:** May submit E-Rezept with incompatible SOK codes
**Fix Required:** Add new validation rules
```
#### Rule GEN-007: E-Rezept SOK Compatibility
```
Severity: ERROR
Condition: SOK must support E-Rezept if prescription is E-Rezept
Fields: E-Rezept flag, SOK
Implementation:
- If prescription.is_e_rezept == true
- Check: SOK.e_rezept_compatible IN [1, 2]
- Error if SOK.e_rezept_compatible == 0
Reference: CODE_STRUCTURES.md Section 5.4
```
```

#### 7. Missing VAT Consistency Validation
**Location:** TA1 Section 2 or 3
**Issue:** No systematic VAT rate validation against SOK
**Impact:** May submit incorrect VAT rates
**Fix Required:** Add new validation rules
```
#### Rule GEN-008: VAT Rate Consistency
```
Severity: ERROR
Condition: VAT rate must match SOK specification
Fields: VAT rate, SOK
Implementation:
- Retrieve SOK.vat_rate from reference table
- Map VAT codes: 0=0%, 1=7%, 2=19%, -=N/A
- Verify: prescription.vat_code == SOK.vat_rate
- Error if mismatch
Reference: CODE_STRUCTURES.md Section 5.3
```
```

#### 8. Missing Additional Data Requirement Validation
**Location:** TA1 Section 6 (Compounded Preparations)
**Issue:** No validation for Zusatzdaten requirement
**Impact:** May submit SOKs without required additional data
**Fix Required:** Enhance existing rules or add new
```
#### Rule REZ-021: Additional Data Requirement
```
Severity: ERROR
Condition: SOK requires additional data per Zusatzdaten field
Fields: SOK, Additional data structure
Implementation:
- Retrieve SOK.zusatzdaten from reference table
- If zusatzdaten > 0:
  - Verify additional_data is present
  - Verify completeness based on zusatzdaten value:
    - 1: Composition data required
    - 2: Factor/price data required
    - 3: Opioid dose data required
    - 4: Fee/service data required
- Error if missing
Reference: CODE_STRUCTURES.md Section 3, validation rule note
```
```

---

## Document Relationship

The three documents serve **complementary** purposes:

```
┌─────────────────────────────────────────────────────────────┐
│                  E-Rezept Validation System                  │
└─────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
        ▼                     ▼                     ▼
┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐
│  TA1-Validation  │  │ CODE_STRUCTURES  │  │    VALIDATION    │
│  -Rules-Tech     │  │                  │  │    _EXAMPLES     │
│  -Spec.md        │  │                  │  │                  │
├──────────────────┤  ├──────────────────┤  ├──────────────────┤
│ HOW to validate  │  │ WHAT to validate │  │ WHAT it looks    │
│                  │  │ against          │  │ like in practice │
├──────────────────┤  ├──────────────────┤  ├──────────────────┤
│ • Business logic │  │ • Factor codes   │  │ • 16 scenarios   │
│ • Validation     │  │ • Price codes    │  │ • Inputs/outputs │
│   rules (FMT,    │  │ • SOK1 catalog   │  │ • Pass/fail      │
│   GEN, BTM, CAN, │  │   (172 codes)    │  │   examples       │
│   REZ, CALC,     │  │ • SOK2 catalog   │  │ • Workflow       │
│   SPC, FEE)      │  │   (109 codes)    │  │   diagrams       │
│ • Error codes    │  │ • Cross-code     │  │ • Best practices │
│ • Implementation │  │   rules          │  │                  │
│   architecture   │  │ • Data quality   │  │                  │
│                  │  │   checks         │  │                  │
└──────────────────┘  └──────────────────┘  └──────────────────┘
        │                     │                     │
        └─────────────────────┴─────────────────────┘
                              │
                              ▼
                    Validator Implementation
```

### Purpose of Each Document

1. **TA1-Validation-Rules-Technical-Specification.md**
   - **Role:** Business logic and validation algorithms
   - **Target:** Developers implementing validators
   - **Content:** HOW to validate (rules, formulas, logic)
   - **Should reference:** CODE_STRUCTURES.md for code definitions

2. **CODE_STRUCTURES.md**
   - **Role:** Reference data and code catalogs
   - **Target:** Developers and compliance officers
   - **Content:** WHAT codes exist and their definitions
   - **Should reference:** TA1 document for validation rules

3. **VALIDATION_EXAMPLES.md**
   - **Role:** Practical guidance and test cases
   - **Target:** Developers, QA, and users
   - **Content:** WHAT correct/incorrect data looks like
   - **Should reference:** Both TA1 and CODE_STRUCTURES

---

## Recommended Actions

### 🔥 IMMEDIATE (Critical for correctness)

1. **Update TA1 Appendix A.2** - Add missing factor code 99
2. **Update TA1 Appendix A.3** - Add missing price codes 12, 16, 17, 21
3. **Update TA1 Appendix A.1** - Add reference to CODE_STRUCTURES.md for complete catalog
4. **Add to TA1** - New validation rules:
   - GEN-006: SOK temporal validation
   - GEN-007: E-Rezept compatibility
   - GEN-008: VAT consistency
   - SPC-008: SOK authorization
   - REZ-021: Additional data requirements

### 📋 SHORT-TERM (Enhanced coverage)

5. **Add cross-references** between documents:
   ```markdown
   <!-- In TA1 -->
   For complete code catalogs, see [CODE_STRUCTURES.md](./CODE_STRUCTURES.md)
   For practical examples, see [VALIDATION_EXAMPLES.md](./VALIDATION_EXAMPLES.md)

   <!-- In CODE_STRUCTURES -->
   For validation business logic, see [TA1-Validation-Rules-Technical-Specification.md](./TA1-Validation-Rules-Technical-Specification.md)

   <!-- In VALIDATION_EXAMPLES -->
   Examples follow rules from [TA1-Validation-Rules-Technical-Specification.md](./TA1-Validation-Rules-Technical-Specification.md)
   Code definitions from [CODE_STRUCTURES.md](./CODE_STRUCTURES.md)
   ```

6. **Create mapping document** - Which TA1 rules apply to which SOKs

### 🔮 LONG-TERM (Optimization)

7. **Consolidate error catalogs** - Single source of truth for all error codes
8. **Create test data files** - Based on VALIDATION_EXAMPLES scenarios
9. **Generate TypeScript types** - From CODE_STRUCTURES definitions
10. **Build reference API** - Programmatic access to code catalogs

---

## Gaps Summary Table

| Gap | Severity | Location | Impact | Estimated Fix Time |
|-----|----------|----------|--------|-------------------|
| Missing Factor code 99 | 🔴 CRITICAL | TA1 A.2 | Cannot validate waste scenarios | 5 min |
| Missing Price codes 12,16,17,21 | 🔴 CRITICAL | TA1 A.3 | Cannot validate certain pricing | 5 min |
| Incomplete SOK catalog | 🔴 CRITICAL | TA1 A.1 | Major validation gaps | 10 min |
| No SOK temporal validation | 🔴 CRITICAL | TA1 Sec 2 | May accept expired codes | 30 min |
| No SOK authorization check | 🟠 HIGH | TA1 Sec 9 | Unauthorized code usage | 30 min |
| No E-Rezept compatibility | 🟠 HIGH | TA1 Sec 2/9 | E-Rezept submission errors | 20 min |
| No VAT consistency check | 🟠 HIGH | TA1 Sec 2/3 | Incorrect VAT rates | 20 min |
| No additional data validation | 🟡 MEDIUM | TA1 Sec 6 | Missing required data | 30 min |
| No cross-references | 🟡 MEDIUM | All docs | Navigation difficulty | 15 min |
| No mapping document | 🟢 LOW | New doc | Developer convenience | 2 hours |

**Total Estimated Fix Time:** ~5 hours

---

## Conclusion

The three documents are **complementary, not redundant**. Each serves a distinct purpose:

- ✅ **Keep all three documents**
- ⚠️ **Update TA1** with missing codes and new validation rules
- ✅ **Add cross-references** between documents
- ✅ **Maintain all three** as living documents

The new CODE_STRUCTURES.md and VALIDATION_EXAMPLES.md fill critical gaps that were missing from the TA1 document, particularly around reference data catalogs and practical examples.

---

**Next Steps:**
1. Review and approve this gap analysis
2. Create GitHub issues for each critical gap
3. Prioritize and implement fixes to TA1 document
4. Add cross-references to all documents
5. Update implementation roadmap to include new validation rules

**Document Status:** ✅ READY FOR REVIEW
