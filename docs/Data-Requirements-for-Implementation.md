# Data Requirements for E-Rezept Validator Implementation

**Document Version:** 1.0
**Date:** 2026-01-24
**Status:** Draft
**Purpose:** Identify all data sources, reference tables, and external dependencies required to implement the TA1 validation rules

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Critical Path Data (Must Have for MVP)](#2-critical-path-data-must-have-for-mvp)
3. [Reference Data Tables](#3-reference-data-tables)
4. [External API Integrations](#4-external-api-integrations)
5. [Test Data Requirements](#5-test-data-requirements)
6. [Documentation Requirements](#6-documentation-requirements)
7. [Data Acquisition Plan](#7-data-acquisition-plan)
8. [Data Management Strategy](#8-data-management-strategy)

---

## 1. Executive Summary

### 1.1 Overview

To implement the E-Rezept validator according to TA1 specifications, we need:
- **11 reference data tables** (code tables, special codes, pricing tables)
- **3 external API integrations** (ABDA, Lauer-Taxe, optional gematik)
- **6 types of test data** (sample E-Rezepts, edge cases, BTM, Cannabis, etc.)
- **5 technical documentation sources** (FHIR profiles, TA3, gematik specs)

### 1.2 Priority Levels

| Priority | Description | Timeline |
|----------|-------------|----------|
| **P0** | Blocking - Cannot start implementation without this | Week 1 |
| **P1** | Critical - Needed for MVP completion | Week 2-3 |
| **P2** | Important - Needed for v1.1 release | Week 4-6 |
| **P3** | Optional - Nice to have for v2.0 | Future |

### 1.3 Current Status

✅ **Available:**
- TA1 Version 039 document (complete)
- Technical specification (derived from TA1)
- Product brief
- .NET 8 project structure

❌ **Missing (Critical):**
- TA3 code tables
- Special codes (Anhang 1, 2)
- FHIR R4 E-Rezept profiles
- Sample E-Rezept test data

---

## 2. Critical Path Data (Must Have for MVP)

### 2.1 TA3 Code Tables

**Priority:** P0 (Blocking)
**Source:** GKV-Spitzenverband / DAV (Deutscher Apothekerverband)
**Format:** Excel/CSV or API access

#### Required Tables

| Table | Description | Usage | Approx. Rows | Update Frequency |
|-------|-------------|-------|--------------|------------------|
| **TA3 Table 8.2.25** | Faktorkennzeichen (Factor identifiers) | Validation rules FMT-007, REZ-005, REZ-011, REZ-017 | ~20 codes | Quarterly |
| **TA3 Table 8.2.26** | Preiskennzeichen (Price identifiers) | Validation rules FMT-009, REZ-019, CALC-006 | ~30 codes | Quarterly |

#### Sample Structure Needed

```json
// TA3 Table 8.2.25 - Faktorkennzeichen
{
  "code": "11",
  "description": "Promilleanteil (Per mille value)",
  "validFrom": "2020-01-01",
  "validUntil": null,
  "applicableTo": ["compounding", "economic_quantities"]
}

// TA3 Table 8.2.26 - Preiskennzeichen
{
  "code": "14",
  "description": "Abrechnungspreis nach AMPreisV §§ 4,5",
  "validFrom": "2020-01-01",
  "validUntil": null,
  "calculationBase": "purchase_price_with_surcharges"
}
```

#### Action Items

- [ ] **Request TA3 tables from ASW Genossenschaft** (likely already have access)
- [ ] **Alternative:** Download from DAV website (if publicly available)
- [ ] **Fallback:** Parse from official TA3 PDF document
- [ ] Convert to structured format (JSON/SQL)
- [ ] Create seeding mechanism for database

---

### 2.2 Special Codes (Sonderkennzeichen)

**Priority:** P0 (Blocking)
**Source:** TA1 Anhang 1 (federal) and Anhang 2 (insurance-specific)
**Format:** Excel tables (mentioned as externalized in TA1 v038)

#### Required Annexes

| Annex | Description | Usage | Critical Codes |
|-------|-------------|-------|----------------|
| **Anhang 1** | Bundesweit vereinbarte Sonderkennzeichen (Federal special codes) | BTM fees, Cannabis codes, Compounding codes | ~50-100 codes |
| **Anhang 2** | Krankenkassen-Apotheken vereinbarte Sonderkennzeichen (Insurance-pharmacy codes) | Messenger service, Noctu fees | ~30-50 codes |

#### Critical Special Codes for MVP

```json
// High-priority special codes
{
  "btm_codes": [
    {"code": "02567001", "description": "BTM-Gebühr", "category": "btm", "priority": "P0"}
  ],
  "cannabis_codes": [
    {"code": "06461446", "description": "Cannabis dried flowers", "category": "cannabis", "priority": "P0"},
    {"code": "06461423", "description": "Cannabis extracts", "category": "cannabis", "priority": "P0"},
    {"code": "06460665", "description": "Dronabinol preparation type 1", "category": "cannabis", "priority": "P0"},
    {"code": "06460694", "description": "Dronabinol preparation type 2", "category": "cannabis", "priority": "P0"},
    {"code": "06460748", "description": "Cannabis preparation type 3", "category": "cannabis", "priority": "P0"},
    {"code": "06460754", "description": "Cannabis preparation type 4", "category": "cannabis", "priority": "P0"}
  ],
  "compounding_codes": [
    {"code": "06460702", "description": "Standard compounding", "category": "compounding", "priority": "P0"},
    {"code": "09999011", "description": "Alternative compounding", "category": "compounding", "priority": "P0"}
  ],
  "economic_quantity_codes": [
    {"code": "02567053", "description": "Individual dispensing", "category": "economic_qty", "priority": "P0"},
    {"code": "02566993", "description": "Weekly blister", "category": "economic_qty", "priority": "P0"}
  ],
  "deviation_codes": [
    {"code": "02567024", "description": "Deviation from standard dispensing", "category": "deviation", "priority": "P1"}
  ],
  "artificial_insemination": [
    {"code": "09999643", "description": "Artificial insemination marker", "category": "special_case", "priority": "P1"}
  ]
}
```

#### Action Items

- [ ] **Extract Anhang 1 and 2 from official TA1 documentation** (mentioned as Excel files in TA1 v038)
- [ ] **Request from ASW Genossenschaft** (they likely have these tables)
- [ ] **Alternative:** Contact DAV for official special code lists
- [ ] Create database schema for special codes
- [ ] Implement versioning (codes can become inactive over time)

---

### 2.3 FHIR R4 E-Rezept Structure Definitions

**Priority:** P0 (Blocking)
**Source:** gematik Simplifier repository
**Format:** FHIR StructureDefinition (JSON/XML)

#### Required FHIR Profiles

| Profile | URL | Purpose |
|---------|-----|---------|
| **GEM_ERP_PR_Bundle** | https://simplifier.net/erezept-workflow | Main E-Rezept bundle structure |
| **GEM_ERP_PR_MedicationDispense** | https://simplifier.net/erezept-workflow | Dispensing information (Abgabedaten) |
| **GEM_ERP_PR_Medication** | https://simplifier.net/erezept-workflow | Medication details, PZN |
| **GEM_ERP_PR_Composition** | https://simplifier.net/erezept-workflow | Manufacturing data for compounding |

#### Example FHIR Bundle Structure Needed

```json
{
  "resourceType": "Bundle",
  "type": "document",
  "entry": [
    {
      "resource": {
        "resourceType": "MedicationDispense",
        "id": "example-dispense",
        "status": "completed",
        "medicationReference": {
          "reference": "Medication/example-med"
        },
        "dosageInstruction": [
          {
            "text": "1-0-1",
            "timing": {...}
          }
        ],
        "extension": [
          {
            "url": "https://gematik.de/fhir/StructureDefinition/AbgabedatenExtension",
            "valueString": "..."
          }
        ]
      }
    }
  ]
}
```

#### Action Items

- [ ] **Download gematik E-Rezept FHIR profiles** from Simplifier.net
  - URL: https://simplifier.net/erezept-workflow
  - URL: https://simplifier.net/erezept
- [ ] **Study extension definitions** for Abgabedaten (dispensing data)
- [ ] **Identify how manufacturing data (Herstellungssegment)** is encoded
- [ ] Map FHIR structure to TA7 Abgabedaten fields
- [ ] Install Hl7.Fhir.R4 NuGet package (already in project)
- [ ] Create C# model classes from FHIR profiles

---

### 2.4 PZN Validation Data

**Priority:** P0 (Blocking)
**Source:** ABDA or IFA (Informationsstelle für Arzneispezialitäten)

#### Required Data

1. **PZN Checksum Algorithm**
   - Modulo 11 checksum calculation
   - Rules for validation
   - Edge cases (special codes vs. real PZNs)

2. **Sample PZN List** (for testing)
   - Valid PZNs (at least 100 examples)
   - Invalid PZNs (for negative testing)
   - Special codes that look like PZNs

#### PZN Checksum Algorithm

```csharp
// PZN Checksum Algorithm (Modulo 11)
public static bool ValidatePznChecksum(string pzn)
{
    if (pzn.Length != 8 || !pzn.All(char.IsDigit))
        return false;

    int[] weights = { 2, 3, 4, 5, 6, 7, 8 };
    int sum = 0;

    for (int i = 0; i < 7; i++)
    {
        sum += int.Parse(pzn[i].ToString()) * weights[i];
    }

    int checksum = sum % 11;
    int expectedCheckDigit = int.Parse(pzn[7].ToString());

    return checksum == expectedCheckDigit;
}
```

#### Action Items

- [ ] **Confirm PZN checksum algorithm** (Modulo 11 is standard)
- [ ] **Create test PZN list** (ask ASW for sample data)
- [ ] **Alternative:** Generate synthetic valid PZNs for testing
- [ ] Implement PZN format validator (FMT-001)
- [ ] Implement PZN checksum validator (FMT-002)

---

## 3. Reference Data Tables

### 3.1 Hilfstaxe Annexes

**Priority:** P1 (Critical for MVP)
**Source:** DAV / Lauer-Taxe
**Purpose:** Pricing for compounding ingredients, containers, labor

#### Required Hilfstaxe Annexes

| Annex | Description | Usage in TA1 |
|-------|-------------|--------------|
| **Annex 1** | Substances (Stoffe) - PZN, prices | Compounding preparations (§4.14.1 a, c, d) |
| **Annex 2** | Containers (Gefäße) - PZN, prices | Compounding preparations (§4.14.1 b, c) |
| **Annex 4-7** | Substitution preparations pricing | Substitution medications (§4.14.1 d) |
| **Annex 10** | Cannabis preparation pricing | Cannabis regulations (§4.14.2 c) |

#### Sample Structure

```json
{
  "annex": 1,
  "category": "substances",
  "items": [
    {
      "pzn": "00123456",
      "name": "Aqua purificata",
      "unit": "1000ml",
      "basePrice": 5.50,
      "pricePerUnit": 0.0055,
      "validFrom": "2025-01-01",
      "validUntil": null
    }
  ]
}
```

#### Action Items

- [ ] **Request Hilfstaxe annexes from ASW Genossenschaft**
- [ ] **Alternative:** Subscribe to Lauer-Taxe (commercial service)
- [ ] **Fallback:** Use static pricing tables for MVP (less accurate)
- [ ] Create database tables for Hilfstaxe data
- [ ] Implement price lookup service

---

### 3.2 AMPreisV Pricing Rules

**Priority:** P1 (Critical for price calculations)
**Source:** Bundesgesetzblatt (Federal Law Gazette)
**Format:** Legal text + calculation formulas

#### Required Information

1. **Base Pricing Formulas** (§§ 4, 5 AMPreisV)
   - Purchase price markup percentages
   - Fixed surcharges
   - Labor prices for compounding

2. **Surcharge Tables**
   - Percentage surcharges based on base quantity
   - Fixed fees
   - Cannabis-specific surcharges (if any)

3. **VAT Rates**
   - Current VAT rate (19% standard, 7% reduced)
   - Applicability to pharmaceutical products

#### Example Pricing Formula

```csharp
// AMPreisV § 5 Abs. 1 - Compounding surcharge calculation
public decimal CalculateCompoundingSurcharge(decimal purchasePrice, decimal quantity, decimal baseQuantity)
{
    // Percentage surcharge per AMPreisV
    decimal percentageSurcharge = purchasePrice * 0.10m; // 10% example

    // Fixed surcharge
    decimal fixedSurcharge = 8.35m; // Example fixed fee

    // Adjust for quantity exceeding base quantity
    if (quantity > baseQuantity)
    {
        decimal factor = quantity / baseQuantity;
        return (percentageSurcharge + fixedSurcharge) * factor;
    }

    return percentageSurcharge + fixedSurcharge;
}
```

#### Action Items

- [ ] **Download AMPreisV from official sources** (gesetze-im-internet.de)
- [ ] Extract pricing formulas from legal text
- [ ] Create calculation service for AMPreisV pricing
- [ ] Validate against real-world examples from ASW

---

### 3.3 BTM Classifications

**Priority:** P2 (Important for BTM validation)
**Source:** BfArM (Bundesinstitut für Arzneimittel und Medizinprodukte)
**Format:** BtM-Liste (controlled substances list)

#### Required Data

1. **BtM Substance List**
   - Active ingredients classified as controlled substances
   - PZNs of BTM medications
   - BTM categories (Anlage I, II, III)

2. **T-Rezept Substance List**
   - Thalidomide and related substances
   - PZNs of T-Rezept medications

#### Action Items

- [ ] **Download BtM-Liste from BfArM** (bfarm.de)
- [ ] Map substances to PZNs
- [ ] **Alternative:** Use ABDA database BTM flags
- [ ] Create BTM lookup service

---

## 4. External API Integrations

### 4.1 ABDA Database API

**Priority:** P1 (Critical for PZN validation)
**Provider:** ABDA (Bundesvereinigung Deutscher Apothekerverbände)
**Cost:** Commercial license required

#### API Capabilities Needed

1. **PZN Lookup**
   - Validate PZN exists
   - Get medication name, strength, form
   - Check market authorization status

2. **Drug Information**
   - Active ingredients
   - BTM classification
   - Festbetrag (fixed price) data
   - AVK (pharmacy sales price)

3. **Pricing Data**
   - Current AEK (pharmacy purchase price)
   - AVK (pharmacy sales price)
   - Festbetrag if applicable

#### API Request Example

```http
GET https://api.abdata.de/v1/pzn/00123456
Authorization: Bearer {API_KEY}

Response:
{
  "pzn": "00123456",
  "name": "Paracetamol 500mg Tabletten",
  "manufacturer": "Example Pharma",
  "activeIngredients": ["Paracetamol"],
  "strength": "500mg",
  "packageSize": "20 tablets",
  "btm": false,
  "tRezept": false,
  "marketAuthorizationStatus": "active",
  "pricing": {
    "aek": 5.50,
    "avk": 8.35,
    "festbetrag": 6.50
  }
}
```

#### Action Items

- [ ] **Contact ASW Genossenschaft** - they likely have ABDA access
- [ ] **Alternative:** Request ABDA trial access for development
- [ ] **Fallback:** Use static PZN data dump for MVP
- [ ] Implement ABDA API client with circuit breaker
- [ ] Create caching layer (cache for 24 hours)

---

### 4.2 Lauer-Taxe API (Optional)

**Priority:** P2 (Important for enhanced validation)
**Provider:** Lauer-Fischer GmbH
**Cost:** Commercial license

#### API Capabilities

- Real-time PZN validation
- Current pricing (updated monthly)
- Drug interaction data (future enhancement)
- Dosage information

#### Action Items

- [ ] Evaluate Lauer-Taxe as alternative/supplement to ABDA
- [ ] **Decision:** ABDA vs. Lauer-Taxe vs. both
- [ ] Negotiate commercial terms with ASW

---

### 4.3 gematik E-Rezept API (Optional)

**Priority:** P3 (Optional for v2.0)
**Provider:** gematik
**Purpose:** Validate against live E-Rezept system

#### Capabilities

- Validate E-Rezept structure against official schema
- Check prescription authenticity
- Verify digital signatures

#### Action Items

- [ ] Research gematik test environment access
- [ ] Plan for v2.0 integration
- [ ] Not blocking for MVP

---

## 5. Test Data Requirements

### 5.1 Sample E-Rezept Test Data

**Priority:** P0 (Blocking)
**Source:** ASW Genossenschaft or gematik test data
**Format:** FHIR R4 JSON bundles

#### Required Test Cases

| Category | Count | Purpose |
|----------|-------|---------|
| **Standard E-Rezept** | 10 | Basic validation testing |
| **BTM E-Rezept** | 5 | BTM-specific rules (when available) |
| **Cannabis E-Rezept** | 10 | Cannabis regulations (new in v039) |
| **Compounded Preparations** | 15 | Parenteral, economic quantities, general |
| **Special Cases** | 10 | §3 Abs.4, artificial insemination, deviations |
| **Invalid/Error Cases** | 20 | Negative testing (missing fields, wrong formats) |

#### Test Data Structure

```json
{
  "testCaseId": "TC-001-Standard-ERezept",
  "description": "Valid standard E-Rezept with single PZN",
  "expectedResult": "PASSED",
  "fhirBundle": {
    "resourceType": "Bundle",
    "..." : "..."
  },
  "validationRules": ["GEN-001", "GEN-003", "FMT-001", "FMT-003"]
}
```

#### Action Items

- [ ] **Request sample E-Rezept data from ASW Genossenschaft**
- [ ] **Alternative:** Download gematik test data repository
  - URL: https://github.com/gematik/eRezept-Examples
- [ ] **Create synthetic test cases** for edge conditions
- [ ] Anonymize real E-Rezept data if available
- [ ] Organize test data by validation category

---

### 5.2 Edge Case Test Data

**Priority:** P1 (Critical for robust validation)

#### Edge Cases to Cover

1. **Timestamp Edge Cases**
   - Exactly at signature timestamp boundary
   - Timezone conversions (CET/CEST transitions)
   - Leap year dates
   - Date in past/future

2. **Numeric Precision Edge Cases**
   - Maximum decimal places (6.6 for factors)
   - Trailing zeros handling (1.0 vs 1.000000)
   - Very small values (0.000001)
   - Very large values (999999.999999)

3. **Counter Sequence Edge Cases**
   - Single unit (counter = 1)
   - Maximum units (100+ units)
   - Non-sequential counters (error case)

4. **Special Code Combinations**
   - Multiple special codes per prescription
   - Conflicting special codes
   - Deprecated special codes

#### Action Items

- [ ] Create edge case test suite
- [ ] Document expected behavior for each edge case
- [ ] Use in automated testing

---

### 5.3 Performance Test Data

**Priority:** P2 (Important for performance validation)

#### Requirements

- 1,000 E-Rezept samples for load testing
- Target: <500ms per validation (product brief requirement)
- Simulate concurrent validations

#### Action Items

- [ ] Generate or collect large test dataset
- [ ] Set up performance testing framework
- [ ] Measure baseline performance
- [ ] Optimize based on results

---

## 6. Documentation Requirements

### 6.1 Technical Annexes

**Priority:** P0 (Blocking)

| Document | Description | Source | Status |
|----------|-------------|--------|--------|
| **TA1 v039** | Billing rules | GKV/DAV | ✅ Available |
| **TA3** | Segment definitions, code tables | GKV/DAV | ❌ Need full document |
| **TA7** | Dispensing data structure (Abgabedaten) | GKV/DAV | ❌ Need full document |

#### Action Items

- [ ] **Request complete TA3 from ASW** (code table definitions)
- [ ] **Request complete TA7 from ASW** (Abgabedaten field specs)
- [ ] Parse and extract relevant sections
- [ ] Create cross-reference mapping (TA1 ↔ TA3 ↔ TA7)

---

### 6.2 gematik Specifications

**Priority:** P1 (Critical for FHIR understanding)

| Document | Description | URL |
|----------|-------------|-----|
| **E-Rezept FHIR Profiles** | FHIR StructureDefinitions | https://simplifier.net/erezept-workflow |
| **E-Rezept Implementation Guide** | Technical implementation guide | https://gemspec.gematik.de/ |
| **Abgabedaten Specification** | How dispensing data is encoded in FHIR | Part of E-Rezept spec |

#### Action Items

- [ ] Download all relevant gematik specs
- [ ] Study FHIR extension definitions
- [ ] Map gematik extensions to TA1 requirements

---

### 6.3 Legal/Regulatory Documents

**Priority:** P2 (Important for compliance understanding)

| Document | Description | Source |
|----------|-------------|--------|
| **AMPreisV** | Pharmaceutical pricing regulation | gesetze-im-internet.de |
| **BtMG** | Controlled substances act | gesetze-im-internet.de |
| **SGB V § 300** | Data transmission legal framework | gesetze-im-internet.de |

#### Action Items

- [ ] Download and archive relevant legal texts
- [ ] Extract pricing formulas from AMPreisV
- [ ] Document BTM regulations from BtMG
- [ ] Not blocking for implementation start

---

## 7. Data Acquisition Plan

### 7.1 Week 1 - Critical Path Data

**Goal:** Unblock implementation start

| Data Item | Action | Owner | Status |
|-----------|--------|-------|--------|
| TA3 Tables 8.2.25, 8.2.26 | Request from ASW | Team Lead | ⏳ Pending |
| Anhang 1, 2 (Special Codes) | Request from ASW | Team Lead | ⏳ Pending |
| FHIR Profiles | Download from Simplifier | Developer | ⏳ Pending |
| Sample E-Rezept Data | Request from ASW + download gematik | Developer | ⏳ Pending |
| PZN Checksum Algorithm | Research + implement | Developer | ⏳ Pending |

---

### 7.2 Week 2-3 - MVP Completion Data

**Goal:** Complete MVP validator

| Data Item | Action | Owner | Status |
|-----------|--------|-------|--------|
| Hilfstaxe Annexes 1, 2 | Request from ASW/Lauer-Taxe | Team Lead | ⏳ Pending |
| AMPreisV Formulas | Extract from legal text | Developer | ⏳ Pending |
| ABDA API Access | Coordinate with ASW | Team Lead | ⏳ Pending |
| Complete TA7 Document | Request from ASW | Team Lead | ⏳ Pending |
| Edge Case Test Data | Create synthetic | QA Team | ⏳ Pending |

---

### 7.3 Week 4-6 - Enhanced Features Data

**Goal:** Implement v1.1 features

| Data Item | Action | Owner | Status |
|-----------|--------|-------|--------|
| BTM Classification List | Download from BfArM | Developer | ⏳ Pending |
| Hilfstaxe Annexes 4-7, 10 | Request from ASW/Lauer-Taxe | Team Lead | ⏳ Pending |
| Lauer-Taxe API (optional) | Evaluate and negotiate | Team Lead | ⏳ Pending |
| Performance Test Dataset | Generate 1000+ samples | QA Team | ⏳ Pending |

---

## 8. Data Management Strategy

### 8.1 Data Storage

#### Database Schema

```sql
-- Code tables
CREATE TABLE FactorIdentifiers (
    Code VARCHAR(2) PRIMARY KEY,
    Description NVARCHAR(200),
    ValidFrom DATE,
    ValidUntil DATE,
    ApplicableTo VARCHAR(100)
);

CREATE TABLE PriceIdentifiers (
    Code VARCHAR(2) PRIMARY KEY,
    Description NVARCHAR(200),
    ValidFrom DATE,
    ValidUntil DATE,
    CalculationBase VARCHAR(100)
);

-- Special codes
CREATE TABLE SpecialCodes (
    Code VARCHAR(8) PRIMARY KEY,
    Description NVARCHAR(200),
    Category VARCHAR(50),
    ValidFrom DATE,
    ValidUntil DATE,
    Priority VARCHAR(10),
    Annex INT -- 1 or 2
);

-- Hilfstaxe data
CREATE TABLE HilfstaxeItems (
    PZN VARCHAR(8) PRIMARY KEY,
    Name NVARCHAR(200),
    Annex INT,
    Unit VARCHAR(50),
    BasePrice DECIMAL(10,2),
    PricePerUnit DECIMAL(10,6),
    ValidFrom DATE,
    ValidUntil DATE
);

-- PZN cache (from ABDA API)
CREATE TABLE PznCache (
    PZN VARCHAR(8) PRIMARY KEY,
    Name NVARCHAR(200),
    Manufacturer NVARCHAR(200),
    IsBTM BIT,
    IsTRezept BIT,
    MarketStatus VARCHAR(50),
    AEK DECIMAL(10,2), -- Pharmacy purchase price
    AVK DECIMAL(10,2), -- Pharmacy sales price
    Festbetrag DECIMAL(10,2),
    CachedAt DATETIME,
    ExpiresAt DATETIME
);
```

---

### 8.2 Data Update Strategy

#### Update Frequencies

| Data Type | Update Frequency | Method | Responsibility |
|-----------|------------------|--------|----------------|
| TA3 Code Tables | Quarterly (with TA versions) | Manual import | DevOps |
| Special Codes | Quarterly (with TA versions) | Manual import | DevOps |
| Hilfstaxe Data | Monthly | API sync or manual | DevOps |
| PZN Cache | Real-time (24h cache) | API with caching | Application |
| AMPreisV Formulas | Annually | Code update | Developer |

---

### 8.3 Data Versioning

#### Version Control Strategy

```json
{
  "dataVersion": {
    "ta1Version": "039",
    "ta3Version": "039",
    "effectiveDate": "2025-10-01",
    "codeTablesVersion": "2025-Q4",
    "specialCodesVersion": "2025-Q4",
    "hilfstaxeVersion": "2025-11"
  }
}
```

#### Action Items

- [ ] Implement data versioning in database
- [ ] Create migration scripts for version updates
- [ ] Log which data version was used for each validation

---

### 8.4 Fallback Strategies

#### If Data Not Available

| Data Type | Fallback Strategy | Impact |
|-----------|------------------|--------|
| ABDA API Down | Use cached data (24h max) | Slight staleness acceptable |
| TA3 Tables Missing | Manual entry of critical codes (~50 codes) | Reduced coverage |
| Hilfstaxe Missing | Skip price validation, warn only | Price validation disabled |
| Sample Test Data Missing | Generate synthetic test cases | Less realistic testing |

---

## 9. Data Acquisition Checklist

### Immediate Actions (This Week)

- [ ] **Contact ASW Genossenschaft** to request:
  - [ ] Complete TA3 document (especially tables 8.2.25, 8.2.26)
  - [ ] Anhang 1 and 2 Excel files (special codes)
  - [ ] TA7 complete document (Abgabedaten structure)
  - [ ] Sample anonymized E-Rezept data (10-20 examples)
  - [ ] ABDA database access credentials (if available)
  - [ ] Hilfstaxe annexes 1, 2 (at minimum)

- [ ] **Download from public sources:**
  - [ ] gematik FHIR profiles from Simplifier.net
  - [ ] gematik E-Rezept examples from GitHub
  - [ ] AMPreisV from gesetze-im-internet.de
  - [ ] BtMG from gesetze-im-internet.de

- [ ] **Research and document:**
  - [ ] PZN checksum algorithm (Modulo 11)
  - [ ] FHIR extension definitions for Abgabedaten
  - [ ] AMPreisV pricing formulas

### Short-Term Actions (Next 2 Weeks)

- [ ] **Set up data infrastructure:**
  - [ ] Create database schema for reference data
  - [ ] Implement data seeding mechanism
  - [ ] Create API client for ABDA (if access granted)
  - [ ] Implement PZN caching layer

- [ ] **Create test data:**
  - [ ] Generate synthetic E-Rezept test cases
  - [ ] Create edge case test suite
  - [ ] Document test case coverage

### Medium-Term Actions (Next 4-6 Weeks)

- [ ] **Enhanced data:**
  - [ ] Negotiate Lauer-Taxe access (if needed)
  - [ ] Obtain BTM classification list
  - [ ] Get Hilfstaxe annexes 4-7, 10
  - [ ] Generate performance test dataset

---

## 10. Questions for ASW Genossenschaft

### Critical Questions (Block Implementation)

1. **TA3 Access:**
   - Do you have the complete TA3 document with code tables 8.2.25 and 8.2.26?
   - Can we get this in machine-readable format (Excel/CSV/JSON)?

2. **Special Codes:**
   - Do you have Anhang 1 and 2 Excel files externalized from TA1?
   - What format are these available in?
   - How often do these update?

3. **ABDA Access:**
   - Do you have ABDA database API access we can use?
   - If yes, can we get credentials and documentation?
   - If no, do you have a PZN data dump we can use?

4. **Sample Data:**
   - Can you provide anonymized E-Rezept FHIR bundles for testing?
   - Especially: Cannabis E-Rezept, compounded preparations, BTM (when available)?

### Important Questions (Affect MVP Scope)

5. **Hilfstaxe:**
   - Do you have access to Hilfstaxe annexes 1, 2?
   - Are you using Lauer-Taxe or another pricing service?
   - Can we integrate with your existing pricing data?

6. **Validation Priorities:**
   - Which validation rules are most critical to your business?
   - Are there specific error types you see frequently from pharmacies?
   - Should we prioritize certain prescription types (Cannabis, BTM, compounding)?

7. **Integration:**
   - How will you integrate this validator into your existing systems?
   - API-based integration? Library? Batch processing?
   - What response format do you need?

---

## 11. Summary and Next Steps

### Data Status Summary

| Category | Status | Blocking? |
|----------|--------|-----------|
| TA1 Document | ✅ Complete | No |
| TA3 Code Tables | ❌ Missing | **YES - Blocking** |
| Special Codes (Anhang 1, 2) | ❌ Missing | **YES - Blocking** |
| FHIR Profiles | ⏳ Downloadable | No - can get immediately |
| Sample E-Rezept Data | ❌ Missing | **YES - Blocking** |
| PZN Validation | ⏳ Algorithm available, data needed | Partial blocking |
| Hilfstaxe Annexes | ❌ Missing | Partial blocking (price validation) |
| ABDA API Access | ❌ Unknown | Partial blocking (can use fallback) |

### Critical Path to Start Implementation

**Minimum Required to Start (P0):**
1. ✅ TA1 document - **HAVE IT**
2. ❌ TA3 Tables 8.2.25, 8.2.26 - **NEED FROM ASW**
3. ❌ Special codes (Anhang 1, 2) - **NEED FROM ASW**
4. ⏳ FHIR profiles - **CAN DOWNLOAD NOW**
5. ❌ 5-10 sample E-Rezept JSON files - **NEED FROM ASW**

**Can Start With Fallback:**
- Implement format validators (FMT-xxx) without code tables (use hardcoded critical codes)
- Implement general rules (GEN-xxx)
- Set up project architecture
- Create data models

**Recommended Action:**
**Schedule meeting with ASW Genossenschaft this week** to request P0 data items and clarify data access strategy.

---

## Appendix A: Contact Information

### Data Providers

| Provider | Contact | Purpose |
|----------|---------|---------|
| **ASW Genossenschaft** | [Client contact] | TA3, Anhang 1/2, sample data, ABDA access |
| **DAV** | https://www.deutscher-apotheker-verband.de/ | TA documents, Hilfstaxe |
| **gematik** | https://www.gematik.de/ | FHIR profiles, E-Rezept specs |
| **ABDA** | https://www.abdata.de/ | PZN database API |
| **Lauer-Fischer** | https://www.lauer-fischer.de/ | Lauer-Taxe pricing data |
| **BfArM** | https://www.bfarm.de/ | BTM classification list |

---

## Document Control

**Next Actions:**
- [ ] Review with development team
- [ ] Schedule ASW meeting
- [ ] Begin P0 data acquisition
- [ ] Update status as data is obtained

**Review Date:** Weekly until P0 data acquired

---

*End of Data Requirements Document*
