# Billing Code Structures and Validation Rules

## Overview

The German pharmacy billing system uses several types of codes to classify and validate prescriptions. This document describes the structure and validation rules for each code type.

---

## 1. Faktorkennzeichen (Factor Codes) - TA3 Section 8.2.25

**Purpose:** Identifies factors used in pricing calculations for partial quantities, opioid substitution, and waste.

**Structure:**
- **Length:** 2 digits
- **Format:** Numeric string (e.g., "11", "55", "57", "99")

**Valid Codes:**

| Code | Content | Description | Use Case |
|------|---------|-------------|----------|
| `11` | Anteil in Promille | Share in promille | Processed packages, partial quantities, supplements |
| `55` | Einzeldosis in Milligramm | Single dose in milligrams | Opioid substitution Take-Home prescriptions |
| `57` | Einzeldosis in Milligramm | Single dose in milligrams | Opioid substitution supervised administration |
| `99` | Anteil einer Packung in Promille | Package share in promille | Waste/disposal |

**Validation Rules:**
1. Must be exactly 2 digits
2. Must be one of the values: 11, 55, 57, 99
3. Code 55 and 57 are specifically for opioid substitution scenarios
4. Code 99 indicates waste and requires special handling

---

## 2. Preiskennzeichen (Price Codes) - TA3 Section 8.2.25

**Purpose:** Identifies the type of price used in billing calculations.

**Structure:**
- **Length:** 2 digits
- **Format:** Numeric string (e.g., "11", "12", "21")

**Valid Codes:**

| Code | Content | Tax Status | Description |
|------|---------|------------|-------------|
| `11` | Apothekeneinkaufspreis nach AMPreisV | excl. VAT | Pharmacy purchase price per Drug Price Regulation |
| `12` | Von Apotheke mit pharma. Unternehmer vereinbarter Preis | excl. VAT | Price agreed between pharmacy and manufacturer |
| `13` | Von Apotheke tatsächlich geleisteter Einkaufspreis | excl. VAT | Actual purchase price paid by pharmacy |
| `14` | Abrechnungspreis nach § 4 und 5 AMPreisV - "Hilfstaxe" | excl. VAT | Billing price per auxiliary price list (substances) |
| `15` | Zwischen Apotheke und Krankenkasse vereinbarter Preis § 129 Abs. 5 SGB V | excl. VAT | Price agreed between pharmacy and health insurer |
| `16` | Vertragspreise auf Grundlage von § 129a SGB V | excl. VAT | Contract prices based on § 129a SGB V |
| `17` | Abrechnungspreis "Preis 2" nach Verzeichnis mg-Preise | excl. VAT | Billing price "Price 2" per mg-price directory |
| `21` | Abrechnungspreis bei Rabattvertrag § 130a Abs. 8c SGB V "Preis 1" | excl. VAT | Billing price for discount contracts, "Price 1" |

**Validation Rules:**
1. Must be exactly 2 digits
2. Must be one of the values: 11-17, 21
3. All prices are without VAT (USt. excl.)
4. Code 21 applies specifically to discount contract scenarios (§ 130a Abs. 8c)
5. Code 14 applies to compounded preparations ("Hilfstaxe")
6. Code 15 requires a contract between pharmacy and specific health insurer

---

## 3. Sonderkennzeichen Standard (SOK1) - TA1 Anhang 1

**Purpose:** Standard special codes for medications and services that don't have standard identifiers (PZN).

**Structure:**
- **Length:** 8 digits
- **Format:** Numeric string (e.g., "09999005", "02567001")
- **Range:** Typically starts with 02, 06, 09, or 17-18

**Categories:**

### 3.1 Medications without PZN
| SOK | Description | VAT | E-Rezept | Pharmacy Discount |
|-----|-------------|-----|----------|-------------------|
| `09999005` | Prescription drugs without PZN | 19% | Yes | Per § 130 Abs. 1/1a SGB V |
| `09999175` | Non-prescription drugs without PZN | 19% | Yes | Per § 130 Abs. 1 SGB V |
| `09999117` | Individually imported prescription drugs (§73 Abs. 3 AMG) | 19% | Yes | Per § 130 Abs. 1/1a SGB V |

### 3.2 Compounded Preparations
| SOK | Description | VAT | E-Rezept | Pharmacy Discount |
|-----|-------------|-----|----------|-------------------|
| `09999011` | Compounded preparations per § 5 Abs. 3 AMPreisV | 19% | Yes | Per § 130 Abs. 1/1a SGB V |
| `06460702` | Unprocessed substances per Ziffer 4.4 | 19% | Yes | Per § 130 Abs. 1 SGB V |

### 3.3 Cannabis Products
| SOK | Description | VAT | E-Rezept |
|-----|-------------|-----|----------|
| `06460694` | Cannabis flowers in unchanged state | 19% | Yes |
| `06460665` | Cannabis flowers in preparations | 19% | Yes |
| `06460754` | Cannabinoid substances unchanged | 19% | Yes |
| `06460748` | Cannabinoid substances in preparations | 19% | Yes |

### 3.4 Specialized Preparations (with variable VAT)
| SOK | Description | VAT | E-Rezept |
|-----|-------------|-----|----------|
| `09999092` | Cytostatic preparations | 19% | Yes |
| `06460866` | Cytostatic preparations | 7% | Yes |
| `06460872` | Cytostatic preparations | 0% | Yes |
| `09999100` | Parenteral nutrition solutions | 19% | Yes |
| `06460889` | Parenteral nutrition solutions | 7% | Yes |
| `06460895` | Parenteral nutrition solutions | 0% | Yes |

### 3.5 Opioid Substitution
| SOK | Description | VAT | E-Rezept | Additional Data |
|-----|-------------|-----|----------|-----------------|
| `09999086` | Methadone partial quantities (Anlage 4) | 19% | No | Required |
| `02567107` | Levomethadone partial quantities (Anlage 5) | 19% | No | Required |
| `02567113` | Buprenorphine/Subutex single doses | 19% | No | Required |
| `02567136` | Buprenorphine/Naloxone single doses | 19% | No | Required |

### 3.6 Blood Products (typically tax-free)
| SOK | Description | VAT | E-Rezept |
|-----|-------------|-----|----------|
| `02567515` | Granulocytes without PZN | 0% | Yes |
| `02567521` | Whole blood without PZN | 0% | Yes |
| `02567484` | Erythrocyte concentrates without PZN | 0% | Yes |
| `02567490` | Platelet concentrates without PZN | 0% | Yes |

### 3.7 Fees and Services
| SOK | Description | VAT | E-Rezept | Additional Data |
|-----|-------------|-----|----------|-----------------|
| `02567001` | BTM fee per Ziffer 4.1 | 19% | No | Required |
| `06460688` | T-Rezept fee per Ziffer 4.1 | 19% | No | Required |
| `02567018` | Noctu fee per Ziffer 4.2 | 19% | Yes | Required |
| `06461110` | Delivery service (Botendienst) | 19% | Yes | Required |

### 3.8 Flu Vaccinations (GKV)
| SOK | Description | VAT | E-Rezept |
|-----|-------------|-----|----------|
| `17716926` | Agreed price for flu vaccination service | 0% | Yes |
| `17716955` | Agreed price for auxiliary services | 0% | Yes |
| `18774512` | Legal procurement costs § 132e Abs. 1a | 0% | Yes |

### 3.9 COVID-19 Vaccinations
| SOK | Description | VAT | E-Rezept | Valid Until |
|-----|-------------|-----|----------|-------------|
| `17717400` | COVID vaccination service (updated) | 0% | Yes | Active |
| `18774908` | COVID vaccination materials | 0% | Yes | Active |

### 3.10 Pharmacy Services (Pharmaceutical Care)
| SOK | Description | VAT | E-Rezept |
|-----|-------------|-----|----------|
| `17716783` | Extended instruction on inhalation technique | N/A | Yes |
| `17716808` | Extended medication counseling for polymedication | N/A | Yes |
| `17716820` | Pharmaceutical care for oral antitumor therapy | N/A | Yes |
| `17716843` | Pharmaceutical care for organ transplant patients | N/A | Yes |

**Validation Rules:**
1. Must be exactly 8 digits
2. Must exist in the SOK1 reference table
3. Check if code is still valid (not expired based on "Außerkrafttreten" dates)
4. VAT rate must match the code specification
5. E-Rezept compatibility must be checked
6. "Zusatzdaten" (additional data) field indicates required supplementary information:
   - `0` = No additional data required
   - `1` = Additional data required (e.g., composition for compounded preparations)
   - `2` = Additional data for factors/prices
   - `3` = Additional data for opioid substitution doses
   - `4` = Additional data for fees/services

---

## 4. Sonderkennzeichen Contract-Specific (SOK2) - TA1 Anhang 2

**Purpose:** Special codes agreed between specific health insurers and pharmacy associations (regional or contract-specific).

**Structure:**
- **Length:** 8 digits
- **Format:** Numeric string (e.g., "02566740", "17716518")
- **Assignment:** Given to specific organizations (DAV, regional associations)

**Examples:**

| SOK | Description | Assigned To | Valid From | Valid Until |
|-----|-------------|-------------|------------|-------------|
| `02566740` | Homeopathy contract DAV-mhplusBKK | DAV | 01.09.2005 | Active |
| `06460501` | Supplement to framework contract AOK BW | LAV Baden-Württemberg | 01.04.2015 | 01.01.2025 |
| `17716518` | Reconstitution Risdiplam (Evrysdi) | DAV/vdek | 01.02.2022 | 30.04.2025 |
| `17717392` | Imported drugs due to supply shortage | BAV | 03.05.2023 | Active |
| `18774506` | Remuneration for supervised opioid administration | DAV | 01.05.2024 | Active |

**Validation Rules:**
1. Must be exactly 8 digits
2. Must exist in the SOK2 reference table
3. Check validity period:
   - "Gültig ab Abrechnungsmonat" / "Gültig ab Abgabedatum" = Start date
   - "Außerkrafttreten Abrechnungsmonat" / "Außerkrafttreten Abgabedatum" = End date
4. Verify the pharmacy is authorized to use the code (based on regional association or contract)
5. Some codes are limited to specific organizations (e.g., LAV Niedersachsen, SAV)

---

## 5. Cross-Code Validation Rules

### 5.1 SOK + Faktorkennzeichen Combinations

| SOK Pattern | Required Faktorkennzeichen | Reason |
|-------------|---------------------------|---------|
| Opioid substitution SOKs (09999086, 02567107, etc.) | `55` or `57` | Must specify dose in milligrams |
| Partial quantity SOKs (09999057, 09999198) | `11` | Must specify share in promille |
| Waste indication | `99` | Must specify waste amount in promille |

### 5.2 SOK + Preiskennzeichen Combinations

| SOK Pattern | Typical Preiskennzeichen | Reason |
|-------------|-------------------------|---------|
| Standard prescription drugs | `11`, `12`, `13` | Regular pharmacy purchase prices |
| Compounded preparations (09999011) | `14` | Uses Hilfstaxe (auxiliary price list) |
| Discount contract medications | `21` | Applies discount contract "Preis 1" |
| Contract-specific drugs (SOK2) | `15`, `16` | Uses agreed contract prices |

### 5.3 VAT Consistency

**Rule:** The VAT rate specified in the billing data must match the SOK definition.

| VAT Code | Description | Applicable SOKs |
|----------|-------------|-----------------|
| `0` | Tax-free (0%) | Blood products, vaccinations, hospital pharmacy services |
| `1` | Reduced rate (7%) | Specific hospital pharmacy preparations |
| `2` | Standard rate (19%) | Most medications and pharmacy services |
| `-` | Not applicable | Service codes without direct VAT (e.g., fee indicators) |

### 5.4 E-Rezept Compatibility

**Rule:** If the prescription is submitted as E-Rezept, the SOK must support E-Rezept.

- Check the "E-Rezept" column in SOK tables
- Values: `0` = not compatible, `1` = compatible, `2` = special handling required

---

## 6. Data Quality Checks

### 6.1 Temporal Validation
```
IF prescription.dispensing_date < SOK.valid_from THEN
  ERROR "SOK not yet valid at dispensing date"

IF SOK.valid_until IS NOT NULL AND prescription.dispensing_date > SOK.valid_until THEN
  ERROR "SOK expired at dispensing date"
```

### 6.2 Organizational Authorization
```
IF SOK.assigned_to IS NOT NULL THEN
  IF pharmacy.association NOT IN SOK.assigned_to THEN
    ERROR "Pharmacy not authorized to use this contract-specific SOK"
```

### 6.3 Required Additional Data
```
IF SOK.zusatzdaten > 0 THEN
  IF prescription.additional_data IS NULL THEN
    ERROR "Additional data required but not provided"
```

### 6.4 Code Combination Logic
```
IF SOK IN opioid_substitution_list THEN
  IF faktorkennzeichen NOT IN ['55', '57'] THEN
    ERROR "Opioid substitution requires dose factor (55 or 57)"

IF SOK.category = 'compounded_preparation' THEN
  IF preiskennzeichen != '14' THEN
    WARNING "Compounded preparations typically use price code 14 (Hilfstaxe)"
```

---

## 7. Error Messages

### Severity Levels

| Level | Description | Action |
|-------|-------------|--------|
| `ERROR` | Critical issue, billing will be rejected | Must fix before submission |
| `WARNING` | Suspicious pattern, likely incorrect | Should review, may proceed |
| `INFO` | Informational note | No action required |

### Common Error Codes

| Code | Message | Severity |
|------|---------|----------|
| `SOK_INVALID` | SOK code not found in reference tables | ERROR |
| `SOK_EXPIRED` | SOK code expired at dispensing date | ERROR |
| `SOK_NOT_YET_VALID` | SOK code not yet valid at dispensing date | ERROR |
| `SOK_UNAUTHORIZED` | Pharmacy not authorized for this contract-specific SOK | ERROR |
| `FACTOR_INVALID` | Invalid Faktorkennzeichen | ERROR |
| `FACTOR_REQUIRED` | Faktorkennzeichen required but missing | ERROR |
| `FACTOR_MISMATCH` | Faktorkennzeichen doesn't match SOK requirements | ERROR |
| `PRICE_INVALID` | Invalid Preiskennzeichen | ERROR |
| `PRICE_MISMATCH` | Preiskennzeichen doesn't match SOK type | WARNING |
| `VAT_MISMATCH` | VAT rate doesn't match SOK specification | ERROR |
| `EREZEPT_INCOMPATIBLE` | SOK not compatible with E-Rezept | ERROR |
| `ADDITIONAL_DATA_REQUIRED` | Additional data required but missing | ERROR |

---

## 8. Implementation Notes

### 8.1 Reference Data Management
- SOK1 and SOK2 tables should be updated regularly from official sources
- Maintain effective dating: keep historical records for retroactive validation
- Track changes in code definitions (VAT rates, validity periods)

### 8.2 Validation Strategy
1. **Static Validation:** Check code format and existence
2. **Temporal Validation:** Check validity dates against dispensing date
3. **Authorization Validation:** Check pharmacy eligibility for contract codes
4. **Cross-Reference Validation:** Check code combinations (SOK + Factor + Price)
5. **Business Logic Validation:** Apply domain-specific rules

### 8.3 Performance Considerations
- Index SOK codes for fast lookup
- Cache frequently used validation rules
- Precompute valid SOK lists per time period
- Use bulk validation for batch processing

---

## 9. References

### Official Documents
- **TA1 (Technische Anlage 1)**: Technical specifications for prescription billing data exchange
- **TA3 (Technische Anlage 3)**: Data carrier specifications and format definitions
- **Anhang 1**: Standard special codes (SOK1)
- **Anhang 2**: Contract-specific special codes (SOK2)
- **AMPreisV**: Arzneimittelpreisverordnung (Drug Price Regulation)
- **SGB V**: Sozialgesetzbuch V (Social Code Book V)

### Key Sections
- TA1 Section 4: Billing data structures
- TA1 Anhang 1: SOK1 definitions
- TA1 Anhang 2: SOK2 definitions
- TA3 Section 8.2.25: Factor and price code definitions

---

## 10. Glossary

| Term | German | Description |
|------|--------|-------------|
| **SOK** | Sonderkennzeichen | Special identifier code |
| **PZN** | Pharmazentralnummer | Pharmacy central number (standard drug ID) |
| **Faktorkennzeichen** | Factor code | Code identifying calculation factors |
| **Preiskennzeichen** | Price code | Code identifying price type |
| **AMPreisV** | Arzneimittelpreisverordnung | Drug Price Regulation |
| **Hilfstaxe** | Auxiliary price list | Price list for compounded preparations |
| **BTM** | Betäubungsmittel | Controlled substances (narcotics) |
| **Noctu** | Night/emergency | After-hours dispensing |
| **Zusatzdaten** | Additional data | Supplementary information required |
| **DAV** | Deutscher Apothekerverband | German Pharmacists' Association |
| **vdek** | Verband der Ersatzkassen | Association of Substitute Health Insurers |
| **LAV** | Landesapothekerverband | Regional Pharmacists' Association |

---

**Document Version:** 1.0
**Last Updated:** 2026-01-24
**Based on:** TA1_039_20250331, TA3_042_20240906
