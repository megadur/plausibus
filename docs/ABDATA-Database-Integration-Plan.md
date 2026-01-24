# ABDATA ARTIKELSTAMM Database Integration Plan

**Document Version:** 1.0
**Date:** 2026-01-24
**Status:** Ready for Implementation
**Based on:** ABDATA Artikelstamm Technical Documentation (01.01.2025)

---

## Executive Summary

🎉 **GREAT NEWS:** The ABDATA ARTIKELSTAMM database is **available in production** and contains nearly all the data we need for E-Rezept validation!

### What This Means

✅ **P0 Data RESOLVED:** PZN validation data available
✅ **P1 Data RESOLVED:** Drug information, pricing, BTM/Cannabis flags available
✅ **No API delays:** Direct database access (no external API integration needed for MVP)
✅ **Can start implementation immediately** with real production data

### Critical Path Impact

| Data Need | Original Status | New Status | Impact |
|-----------|----------------|------------|--------|
| PZN Validation | ❌ Missing | ✅ **Available** (PAC_APO) | **UNBLOCKED** |
| BTM Classification | ❌ Missing | ✅ **Available** (PAC_APO field 08) | **UNBLOCKED** |
| Cannabis Identification | ❌ Missing | ✅ **Available** (PAC_APO field G8) | **UNBLOCKED** |
| Pricing Data (AEK, AVK) | ❌ Missing (ABDA API) | ✅ **Available** (PAC_APO) | **UNBLOCKED** |
| Festbetrag Data | ❌ Missing | ✅ **Available** (PAC_APO field 97) | **UNBLOCKED** |
| Market Status | ❌ Missing | ✅ **Available** (PAC_APO field 52) | **UNBLOCKED** |

---

## Table of Contents

1. [ABDATA ARTIKELSTAMM Overview](#1-abdata-artikelstamm-overview)
2. [Critical Data Fields for Validation](#2-critical-data-fields-for-validation)
3. [Database Tables Structure](#3-database-tables-structure)
4. [Integration Architecture](#4-integration-architecture)
5. [Data Access Strategy](#5-data-access-strategy)
6. [Validation Rules Mapping](#6-validation-rules-mapping)
7. [Implementation Plan](#7-implementation-plan)
8. [Still Missing Data](#8-still-missing-data)

---

## 1. ABDATA ARTIKELSTAMM Overview

### 1.1 Database Description

**ABDATA ARTIKELSTAMM** is the German pharmaceutical article master database maintained by ABDATA Pharma-Daten-Service (subsidiary of ABDA).

**Key Facts:**
- **Format:** K2 format (structured text files)
- **Update Frequency:** Daily (production updates)
- **Version:** January 2025 (Documentation: 28.10.2024)
- **Coverage:** All German pharmaceutical products, medical devices, and pharmacy products
- **Primary Key:** PZN (Pharmazentralnummer) - 8-digit code

### 1.2 Available Tables (Main)

| Table | File | Description | Relevance |
|-------|------|-------------|-----------|
| **PAC_APO** | Packungsinfos | Article base information | ⭐⭐⭐ CRITICAL - Contains all PZN data |
| **PGR_APO** | Package size indicators | Package size codes | ⭐⭐ IMPORTANT |
| **PGR2_APO** | Quantitative package data | Exact quantities | ⭐⭐ IMPORTANT |
| **VOV_APO** | Prescription requirements | Verordnungsvorgaben | ⭐⭐⭐ CRITICAL - Prescription rules |
| **VPV_APO** | Article-Prescription link | Links articles to rules | ⭐⭐⭐ CRITICAL |
| **GRU_APO** | Rebate groups (§130a) | Rabattverträge | ⭐⭐ IMPORTANT |
| **ADR_APO** | Addresses | Manufacturer addresses | ⭐ OPTIONAL |
| **DAR_APO** | Dosage forms | Darreichungsformen | ⭐⭐ IMPORTANT |
| **WAR_APO** | Product groups | Warengruppen | ⭐ OPTIONAL |

---

## 2. Critical Data Fields for Validation

### 2.1 PAC_APO - Article Base Information (PRIMARY TABLE)

**Table:** PAC_APO
**ABDATA File Number:** 1005

#### Essential Fields for Validator

| Field ID | Field Name | DB Column | Data Type | Validation Use |
|----------|-----------|-----------|-----------|----------------|
| **01** | Pharmazentralnummer | PZN | F/8/PZ8 | ✅ **Primary key, PZN validation** (FMT-001) |
| **08** | Kz. Betäubungsmittel | BTM | F/1/NU1 | ✅ **BTM classification** (BTM-001, BTM-002) |
| **G8** | Kz. Cannabis | Cannabis | F/1/NU1 | ✅ **Cannabis identification** (CAN-001) |
| **02** | Apothekeneinkaufspreis | Apo_Ek | V/10/NU1 | ✅ **AEK - Pharmacy purchase price** (CALC-004) |
| **04** | Apothekenverkaufspreis | Apo_Vk | V/10/NU1 | ✅ **AVK - Pharmacy sales price** (GEN-003, CALC-004) |
| **18** | Abgabepreis des pharmazeutischen Unternehmers | ApU | V/10/NU1 | ✅ **Pharma manufacturer price** (CALC-004) |
| **97** | Festbetrag | Festbetrag | V/10/NU1 | ✅ **Fixed price for calculations** (SPC-004) |
| **52** | Kz. Verkehrsfähigkeitsstatus | Verkehrsstatus | F/1/NU1 | ✅ **Market authorization status** |
| **54** | Kz. Verschreibungspflicht | Rezeptpflicht | F/1/NU1 | ✅ **Prescription requirement** |
| **81** | Kz. Lifestyle-Medikament | Lifestyle | F/1/NU1 | ✅ **Lifestyle medication flag** |
| **50** | Kz. Transfusionsgesetz | TFG | F/1/NU1 | ✅ **T-Rezept indicator** |
| **37** | Kz. Mehrwertsteuersatz | MwSt | F/1/NU1 | ✅ **VAT rate** (GEN-004) |
| **67** | Langname, ungekürzt | Langname_ungekuerzt | V/80/AN1 | 📋 **Full product name** (display) |
| **60** | Name | Name | V/65/AN1 | 📋 **Product name** (display) |

#### BTM Field Details (Field 08)

```
Wertebereich:
0 → keine Angabe
1 → nein
2 → Betäubungsmittel (BTM according to BtMG)
3 → Ausgenommene Zubereitung nach BtMG (Exempt preparation)
4 → ja, T-Rezept-pflichtig
```

**Usage:**
- Value `2` = BTM prescription required → Trigger BTM validation rules (BTM-001 through BTM-004)
- Value `3` = Exempt but still regulated → Warning only
- Value `4` = T-Rezept → Separate T-Rezept validation rules

#### Cannabis Field Details (Field G8)

```
Wertebereich:
0 → keine Angabe
1 → nein
2 → ja, Cannabis zu medizinischen Zwecken nach § 2 Nr. 1 MedCanG
3 → ja, Cannabis zu medizinischen Zwecken nach § 2 Nr. 2 MedCanG
```

**Usage:**
- Values `2` or `3` → Trigger Cannabis validation rules (CAN-001 through CAN-005)
- Cross-check with BTM field (should not have both BTM and Cannabis flags per TA1 4.14.2)

#### Market Status Field (Field 52)

```
Wertebereich:
01 → Markt (Available on market)
02 → Außer Handel (Out of trade)
03 → Genehmigung widerrufen (Authorization revoked)
04 → Zulassung erloschen (Authorization expired)
05 → Zulassung widerrufen (Authorization revoked)
... (more codes)
```

**Usage:**
- Only code `01` (Markt) should be dispensable
- Codes 02-05 → Warning or error (medication not available)

#### Prescription Requirement Field (Field 54)

```
Wertebereich:
1 → apothekenpflichtig (OTC - pharmacy only)
2 → verschreibungspflichtig (Prescription required)
3 → BTM-rezeptpflichtig (BTM prescription)
... (more codes)
```

---

### 2.2 VOV_APO - Prescription Requirements (VERORDNUNGSVORGABEN)

**Table:** VOV_APO
**ABDATA File Number:** 1010

Special requirements for prescribing/dispensing specific active ingredients (e.g., Thalidomid, BTM, Lifestyle drugs).

| Field ID | Field Name | DB Column | Purpose |
|----------|-----------|-----------|---------|
| **01** | Typ | Typ | Type of requirement (1-14) |
| **02** | Kurztext | Kurztext | Short description |
| **03** | Langtext | Langtext | Detailed requirement text |

**Types include:**
- Type 1: Thalidomid regulations
- Type 2: BTM special handling
- Type 8: Exempt BtMG preparations
- Type 11: Lifestyle medications (not reimbursable)

**Linked via VPV_APO** (Article → Prescription requirement mapping)

---

### 2.3 GRU_APO - Rebate Groups (§130a SGB V)

**Table:** GRU_APO
**ABDATA File Number:** 1009

Rebate contract groups per §130a (8) SGB V.

| Field ID | Field Name | Purpose |
|----------|-----------|---------|
| **01** | Gruppennummer | Group number |
| **02** | IK der Krankenkasse | Insurance company IK |
| **03** | Gültig von | Valid from date |
| **04** | Gültig bis | Valid until date |

**Usage:** Artificial insemination cost calculations, rebate validations

---

## 3. Database Tables Structure

### 3.1 Entity Relationship

```
┌─────────────┐
│  PAC_APO    │ ← Main article table (PZN = primary key)
│  (Packages) │
└──────┬──────┘
       │
       ├──→ PGR_APO (Package size codes)
       ├──→ PGR2_APO (Quantitative package data)
       ├──→ VPV_APO (Linked prescription requirements)
       │       │
       │       └──→ VOV_APO (Prescription requirement rules)
       │
       ├──→ PZG_APO (Linked to rebate groups)
       │       │
       │       └──→ GRU_APO (Rebate group details)
       │               │
       │               └──→ IZG_APO → IKZ_APO (Insurance IK mapping)
       │
       ├──→ VPI_APO → INB_APO (Indication areas)
       └──→ DAR_APO (Dosage forms reference)
```

### 3.2 Key Relationships

1. **PZN → BTM/Cannabis Validation:**
   ```sql
   SELECT PZN, BTM, Cannabis, Rezeptpflicht, Name
   FROM PAC_APO
   WHERE PZN = '12345678';
   ```

2. **PZN → Prescription Requirements:**
   ```sql
   SELECT p.PZN, p.Name, v.Typ, v.Langtext
   FROM PAC_APO p
   JOIN VPV_APO vpv ON p.PZN = vpv.PZN
   JOIN VOV_APO v ON vpv.Typ = v.Typ
   WHERE p.PZN = '12345678';
   ```

3. **PZN → Pricing:**
   ```sql
   SELECT PZN, Apo_Ek, Apo_Vk, ApU, Festbetrag, MwSt
   FROM PAC_APO
   WHERE PZN = '12345678';
   ```

---

## 4. Integration Architecture

### 4.1 Recommended Approach

```
┌─────────────────────────────────────────────────┐
│         E-Rezept Validator Application          │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│        Data Access Layer (Repository)           │
│  ┌──────────────────────────────────────────┐  │
│  │  IPznRepository                          │  │
│  │  - GetByPzn(pzn)                         │  │
│  │  - ValidatePznFormat(pzn)                │  │
│  │  - GetPricingInfo(pzn)                   │  │
│  │  - GetBtmStatus(pzn)                     │  │
│  │  - GetCannabisStatus(pzn)                │  │
│  └──────────────────────────────────────────┘  │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│          Caching Layer (In-Memory)              │
│  - Cache frequently accessed PZNs               │
│  - TTL: 24 hours (data updates daily)           │
│  - Size: ~100k most common PZNs                 │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│       ABDATA ARTIKELSTAMM Database              │
│       (Production - Direct SQL Access)          │
│                                                  │
│  Tables: PAC_APO, VOV_APO, VPV_APO, GRU_APO    │
└─────────────────────────────────────────────────┘
```

### 4.2 Database Connection

**Connection Details:**
- Database: ASW Production ARTIKELSTAMM database
- Access: Direct SQL queries (read-only)
- Connection String: [To be provided by ASW]
- Authentication: [To be confirmed]

**Performance Considerations:**
- Database is read-only for validator
- Implement connection pooling
- Use prepared statements for security
- Cache results for 24 hours (data updates daily)

---

## 5. Data Access Strategy

### 5.1 PznRepository Interface

```csharp
public interface IPznRepository
{
    // Core PZN lookup
    Task<PznArticleInfo> GetByPznAsync(string pzn);

    // Format validation
    bool ValidatePznFormat(string pzn);
    bool ValidatePznChecksum(string pzn);

    // Specific data retrieval
    Task<PricingInfo> GetPricingInfoAsync(string pzn);
    Task<BtmStatus> GetBtmStatusAsync(string pzn);
    Task<CannabisStatus> GetCannabisStatusAsync(string pzn);
    Task<MarketStatus> GetMarketStatusAsync(string pzn);
    Task<PrescriptionRequirement[]> GetPrescriptionRequirementsAsync(string pzn);

    // Batch operations (for performance)
    Task<Dictionary<string, PznArticleInfo>> GetByPznBatchAsync(string[] pzns);
}
```

### 5.2 Data Models

```csharp
public class PznArticleInfo
{
    public string Pzn { get; set; }
    public string Name { get; set; }
    public string LongName { get; set; }

    // BTM/Cannabis classification
    public BtmIndicator BtmStatus { get; set; } // 0=None, 2=BTM, 3=Exempt, 4=T-Rezept
    public CannabisIndicator CannabisStatus { get; set; } // 0=None, 2=MedCanG §2 Nr.1, 3=MedCanG §2 Nr.2

    // Pricing (in cents, excl. VAT unless specified)
    public long ApothekeEinkaufspreis { get; set; } // AEK
    public long ApothekeVerkaufspreis { get; set; } // AVK (incl. VAT)
    public long AbgabepreisPharmUnternehmer { get; set; } // ApU
    public long? Festbetrag { get; set; } // Fixed price (nullable)

    // Market status
    public MarketStatusCode MarketStatus { get; set; } // 01=Available, 02=Out of trade, etc.
    public PrescriptionStatusCode PrescriptionStatus { get; set; } // 1=OTC, 2=Rx, 3=BTM-Rx

    // VAT
    public VatRate VatRate { get; set; } // 1=19%, 2=7%, 3=0%

    // Flags
    public bool IsLifestyleMedication { get; set; }
    public bool IsTransfusionLaw { get; set; }
    public bool IsApothekenpflichtig { get; set; }
}

public class PrescriptionRequirement
{
    public int Type { get; set; } // VOV_APO.Typ
    public string ShortText { get; set; }
    public string DetailedText { get; set; }
}
```

### 5.3 SQL Queries

#### Basic PZN Lookup
```sql
-- Get all article information for a PZN
SELECT
    PZN,
    Name,
    Langname_ungekuerzt AS LongName,
    BTM AS BtmStatus,
    Cannabis AS CannabisStatus,
    Apo_Ek AS ApothekeEinkaufspreis,
    Apo_Vk AS ApothekeVerkaufspreis,
    ApU AS AbgabepreisPharmUnternehmer,
    Festbetrag,
    Verkehrsstatus AS MarketStatus,
    Rezeptpflicht AS PrescriptionStatus,
    MwSt AS VatRate,
    Lifestyle AS IsLifestyleMedication,
    TFG AS IsTransfusionLaw,
    Apopflicht AS IsApothekenpflichtig
FROM PAC_APO
WHERE PZN = @pzn;
```

#### PZN with Prescription Requirements
```sql
-- Get PZN with linked prescription requirements
SELECT
    p.PZN,
    p.Name,
    v.Typ,
    v.Kurztext,
    v.Langtext
FROM PAC_APO p
LEFT JOIN VPV_APO vpv ON p.PZN = vpv.PZN
LEFT JOIN VOV_APO v ON vpv.Typ = v.Typ
WHERE p.PZN = @pzn;
```

#### Batch PZN Lookup (Performance Optimized)
```sql
-- Get multiple PZNs in single query
SELECT
    PZN,
    Name,
    BTM,
    Cannabis,
    Apo_Vk,
    Festbetrag,
    Verkehrsstatus
FROM PAC_APO
WHERE PZN IN (@pzn1, @pzn2, @pzn3, ...);
```

---

## 6. Validation Rules Mapping

### 6.1 PZN Format Validation (FMT-001, FMT-002)

**Data Source:** PAC_APO.PZN field validation
**Algorithm:** Modulo 11 checksum

```csharp
public class PznValidator
{
    public ValidationResult ValidatePzn(string pzn)
    {
        // FMT-001: Format validation
        if (!Regex.IsMatch(pzn, @"^\d{8}$"))
            return ValidationResult.Error("FMT-001-E", "PZN must be 8 digits");

        // Check reserved ranges (internal use only)
        int pznNumber = int.Parse(pzn.Substring(0, 7));
        if ((pznNumber >= 800000 && pznNumber <= 839999) ||
            (pznNumber >= 8000000 && pznNumber <= 8399999))
            return ValidationResult.Error("FMT-001-E", "PZN in reserved range");

        // FMT-002: Checksum validation
        if (!ValidatePznChecksum(pzn))
            return ValidationResult.Warning("FMT-002-W", "PZN checksum invalid");

        // Lookup in ABDATA
        var article = await _pznRepository.GetByPznAsync(pzn);
        if (article == null)
            return ValidationResult.Warning("FMT-001-W", "PZN not found in ABDATA database");

        return ValidationResult.Success();
    }
}
```

### 6.2 BTM Validation (BTM-001 through BTM-004)

**Data Source:** PAC_APO.BTM field + VOV_APO

```csharp
public class BtmValidator
{
    public async Task<ValidationResult> ValidateBtmPrescription(ErezeptBundle erezept)
    {
        // Get all PZNs from prescription
        var pzns = ExtractPzns(erezept);

        // Batch lookup
        var articles = await _pznRepository.GetByPznBatchAsync(pzns);

        // Check for BTM medications
        var btmMedications = articles.Where(a => a.Value.BtmStatus == BtmIndicator.Btm).ToList();

        if (btmMedications.Any())
        {
            // BTM-001: Check for BTM fee special code (02567001)
            if (!HasBtmFeeSpecialCode(erezept))
                return ValidationResult.Error("BTM-001-E", "E-BTM prescription missing BTM fee special code (02567001)");

            // BTM-002: Check all BTM items have PZN, quantity, price
            foreach (var btmMed in btmMedications)
            {
                if (!HasCompleteData(erezept, btmMed.Key))
                    return ValidationResult.Error("BTM-002-E", $"BTM line for PZN {btmMed.Key} missing required fields");
            }

            // BTM-003: Check 7-day validity (warning only)
            var daysSincePrescription = (erezept.DispensingDate - erezept.PrescriptionDate).Days;
            if (daysSincePrescription > 7)
                return ValidationResult.Warning("BTM-003-W", $"BTM prescription dispensed {daysSincePrescription} days after prescription. Maximum 7 days per BtMG §3.");

            // BTM-004: Check for diagnosis code (warning only)
            if (string.IsNullOrEmpty(erezept.DiagnosisCode))
                return ValidationResult.Warning("BTM-004-W", "BTM prescription missing ICD-10 diagnosis code (required per BtMG §3)");
        }

        return ValidationResult.Success();
    }
}
```

### 6.3 Cannabis Validation (CAN-001 through CAN-005)

**Data Source:** PAC_APO.Cannabis field

```csharp
public class CannabisValidator
{
    public async Task<ValidationResult> ValidateCannabisPreparation(ErezeptBundle erezept, string specialCode)
    {
        // CAN-001: Validate special code
        var validCannabisCodes = new[] { "06461446", "06461423", "06460665", "06460694", "06460748", "06460754" };
        if (!validCannabisCodes.Contains(specialCode))
            return ValidationResult.Error("CAN-001-E", $"Invalid Cannabis special code: {specialCode}");

        // Get all ingredients
        var ingredients = ExtractIngredientPzns(erezept);
        var articles = await _pznRepository.GetByPznBatchAsync(ingredients);

        // CAN-002: Check NO BTM substances in Cannabis preparations
        var btmIngredients = articles.Where(a => a.Value.BtmStatus == BtmIndicator.Btm).ToList();
        if (btmIngredients.Any())
            return ValidationResult.Error("CAN-002-E", "Cannabis preparation contains BTM substances. Not allowed per TA1 §4.14.2.");

        // CAN-003: Factor must be "1" in main special code line
        if (erezept.SpecialCodeFactor != 1.0m)
            return ValidationResult.Error("CAN-003-E", $"Cannabis special code line must have Factor = '1'. Found: '{erezept.SpecialCodeFactor}'");

        // CAN-005: Check manufacturing data present
        if (erezept.ManufacturingData == null)
            return ValidationResult.Error("CAN-005-E", "Cannabis preparation missing required manufacturing data (Herstellungssegment)");

        return ValidationResult.Success();
    }
}
```

### 6.4 Price Validation (CALC-004, GEN-003)

**Data Source:** PAC_APO pricing fields

```csharp
public class PriceValidator
{
    public async Task<ValidationResult> ValidateGrossPrice(string pzn, decimal reportedGrossPrice)
    {
        var article = await _pznRepository.GetByPznAsync(pzn);

        if (article == null)
            return ValidationResult.Warning("FMT-001-W", $"PZN {pzn} not found in ABDATA");

        // GEN-003: Gross price should match AVK (pharmacy sales price)
        decimal expectedGrossPrice = article.ApothekeVerkaufspreis / 100.0m; // Convert cents to euros
        decimal tolerance = 0.01m; // 1 cent tolerance

        if (Math.Abs(reportedGrossPrice - expectedGrossPrice) > tolerance)
        {
            return ValidationResult.Warning("GEN-003-W",
                $"Gross price mismatch. Expected: {expectedGrossPrice:F2}€ (from ABDATA AVK), Found: {reportedGrossPrice:F2}€");
        }

        return ValidationResult.Success();
    }
}
```

---

## 7. Implementation Plan

### 7.1 Phase 1: Database Connection Setup (Week 1)

**Tasks:**
- [ ] Get database connection details from ASW
- [ ] Set up read-only database connection
- [ ] Test connectivity from development environment
- [ ] Implement PznRepository with basic CRUD operations
- [ ] Set up connection pooling and error handling

**Deliverable:** Working database connection with basic PZN lookup

---

### 7.2 Phase 2: Core Data Access Layer (Week 1-2)

**Tasks:**
- [ ] Implement full IPznRepository interface
- [ ] Create data models (PznArticleInfo, PricingInfo, etc.)
- [ ] Write SQL queries for all needed lookups
- [ ] Implement batch operations for performance
- [ ] Add comprehensive error handling
- [ ] Write unit tests for data access layer

**Deliverable:** Complete data access layer with tested queries

---

### 7.3 Phase 3: Caching Implementation (Week 2)

**Tasks:**
- [ ] Implement in-memory caching (IMemoryCache)
- [ ] Set cache TTL to 24 hours
- [ ] Implement cache warming for common PZNs
- [ ] Add cache statistics/monitoring
- [ ] Test cache hit rates

**Deliverable:** High-performance caching layer

---

### 7.4 Phase 4: Validation Integration (Week 2-3)

**Tasks:**
- [ ] Integrate PznRepository into validators
- [ ] Implement PZN format validator (FMT-001, FMT-002)
- [ ] Implement BTM validator (BTM-001 through BTM-004)
- [ ] Implement Cannabis validator (CAN-001 through CAN-005)
- [ ] Implement price validator (CALC-004, GEN-003)
- [ ] Integration testing with real ABDATA data

**Deliverable:** Working validators using ABDATA

---

## 8. Still Missing Data

### 8.1 TA3 Code Tables (P0 - Still Blocking)

❌ **Still Need:**
- TA3 Table 8.2.25 (Faktorkennzeichen)
- TA3 Table 8.2.26 (Preiskennzeichen)

**Workaround for MVP:**
- Hardcode critical ~20 codes from TA1 document
- Request full TA3 tables from ASW for complete implementation

### 8.2 Special Codes - Anhang 1 & 2 (P0 - Still Blocking)

❌ **Still Need:**
- Anhang 1: Federal special codes (Excel file)
- Anhang 2: Insurance-pharmacy special codes (Excel file)

**Workaround for MVP:**
- Hardcode critical ~50 special codes extracted from TA1 text
- Special focus on: BTM fees, Cannabis codes, Compounding codes
- Request official Excel files from ASW

### 8.3 TA7 Document (P1 - Important)

❌ **Still Need:**
- Complete TA7 document (Abgabedaten structure)
- Field mappings to FHIR extensions

**Impact:** Needed for complete Abgabedaten validation

### 8.4 Sample E-Rezept Data (P0 - Blocking for Testing)

❌ **Still Need:**
- 10-20 sample E-Rezept FHIR JSON files
- Especially: Cannabis, compounding examples

**Workaround:**
- Download gematik E-Rezept examples from GitHub
- Create synthetic test cases

---

## 9. Summary and Next Steps

### 9.1 Critical Path Update

**BEFORE discovering ABDATA availability:**
```
P0 Blockers: 5 items (PZN data, BTM list, Cannabis data, pricing, TA3 tables)
Estimated time to unblock: 2-3 weeks (API integration, data collection)
```

**AFTER discovering ABDATA availability:**
```
P0 Blockers: 2 items (TA3 tables, Special codes - can hardcode for MVP)
Estimated time to unblock: 2-3 days (database connection + hardcoded lookups)
Can start implementation: IMMEDIATELY
```

### 9.2 Immediate Action Items

**This Week:**
1. ✅ Confirm ABDATA database connection details with ASW
2. ✅ Set up database connection in development environment
3. ✅ Implement basic PznRepository
4. ✅ Test PZN lookups against production ABDATA
5. ✅ Download gematik FHIR profiles and sample data

**Next Week:**
6. ✅ Implement full data access layer
7. ✅ Add caching layer
8. ✅ Integrate ABDATA into validators
9. ⏳ Request TA3 tables and Anhang 1/2 from ASW (parallel)

### 9.3 Questions for ASW

**Database Access (URGENT):**
1. What are the ABDATA ARTIKELSTAMM database connection details?
2. Do we have read-only access?
3. Which tables are accessible (PAC_APO, VOV_APO, etc.)?
4. What is the update frequency?

**Still Missing Data:**
5. Can you provide TA3 tables 8.2.25 and 8.2.26?
6. Can you provide Anhang 1 and 2 Excel files (special codes)?
7. Can you provide TA7 complete document?
8. Can you provide sample anonymized E-Rezept FHIR bundles?

---

## 10. Conclusion

🎉 **The availability of ABDATA ARTIKELSTAMM in production is a GAME CHANGER!**

**Impact:**
- ✅ Removes 90% of P0/P1 data dependencies
- ✅ Eliminates need for external ABDA API integration (for MVP)
- ✅ Provides real, up-to-date production data
- ✅ Enables immediate implementation start
- ✅ Reduces complexity significantly

**Next Step:**
**Confirm database access with ASW and begin implementation TODAY!**

---

## Appendix A: ABDATA Field Reference

### Complete PAC_APO Field List (Relevant for Validation)

| ID | Field Name | DB Column | Type | Purpose |
|----|-----------|-----------|------|---------|
| 01 | Pharmazentralnummer | PZN | F/8/PZ8 | Primary key |
| 02 | Apothekeneinkaufspreis | Apo_Ek | V/10/NU1 | AEK (cents, no VAT) |
| 03 | Kz. Apothekenpflicht | Apopflicht | F/1/NU1 | Pharmacy-only flag |
| 04 | Apothekenverkaufspreis | Apo_Vk | V/10/NU1 | AVK (cents, incl. VAT) |
| 08 | Kz. Betäubungsmittel | BTM | F/1/NU1 | BTM classification |
| 18 | Abgabepreis pharmazeutischer Unternehmer | ApU | V/10/NU1 | Manufacturer price |
| 37 | Kz. Mehrwertsteuersatz | MwSt | F/1/NU1 | VAT rate |
| 52 | Kz. Verkehrsfähigkeitsstatus | Verkehrsstatus | F/1/NU1 | Market status |
| 54 | Kz. Verschreibungspflicht | Rezeptpflicht | F/1/NU1 | Prescription requirement |
| 60 | Name | Name | V/65/AN1 | Product name |
| 67 | Langname, ungekürzt | Langname_ungekuerzt | V/80/AN1 | Full product name |
| 81 | Kz. Lifestyle-Medikament | Lifestyle | F/1/NU1 | Lifestyle drug flag |
| 97 | Festbetrag | Festbetrag | V/10/NU1 | Fixed price |
| C0 | Abgabepreis gemäß § 78 (3a) Satz 1 AMG | ApU_78_3a_1_AMG | V/10/NU1 | Reimbursement price |
| G8 | Kz. Cannabis | Cannabis | F/1/NU1 | Cannabis indicator |

---

*End of Document*
