# TA1 Validation Rules - Technical Specification

**Document Version:** 1.0
**Based on:** TA1 Version 039, Stand 31.03.2025
**Applicable from:** Abrechnungsmonat 10/2025
**Author:** E-Rezept-Validator Team
**Date:** 2026-01-24
**Reference:** § 300 SGB V - Technische Anlage 1 zur Arzneimittelabrechnungsvereinbarung

---

## Table of Contents

1. [Document Overview](#1-document-overview)
2. [General Validation Rules](#2-general-validation-rules)
3. [Data Format Validations](#3-data-format-validations)
4. [BTM & T-Rezept Validations](#4-btm--t-rezept-validations)
5. [Special Fee Validations](#5-special-fee-validations)
6. [Compounded Preparations (Rezepturen)](#6-compounded-preparations-rezepturen)
7. [Cannabis-Specific Validations](#7-cannabis-specific-validations)
8. [Economic Single Quantities](#8-economic-single-quantities)
9. [Special Case Validations](#9-special-case-validations)
10. [Price and Factor Calculations](#10-price-and-factor-calculations)
11. [Validation Rule Priority Matrix](#11-validation-rule-priority-matrix)
12. [Error Codes and Messages](#12-error-codes-and-messages)

---

## 1. Document Overview

### 1.1 Purpose

This technical specification defines the complete set of validation rules for E-Rezept billing data according to TA1 Version 039. These rules ensure compliance with German pharmaceutical billing regulations (§ 300 SGB V) and prevent rejection by health insurance companies (Krankenkassen).

### 1.2 Scope

This specification covers:
- E-Rezept dispensing data (Abgabedaten) validation
- Format and structural validations
- Business logic validations
- Special case handling (BTM, Cannabis, compounded preparations, etc.)
- Price and factor calculation rules

### 1.3 Key Changes in TA1 Version 039

**Effective Date:** October 2025

- **Section 4.14:** Complete revision for Cannabis regulations
- **Section 4.14.2:** New rules for decimal place handling in factors
- **Section 4.5.2:** Revised regulations for § 3 Abs. 4 prescriptions
- **Section 4.10:** Updates for BTM, Noctu, and T-Rezept fees
- **Section 5:** Technical description additions for re-dispensing, Noctu, and BTM fees

### 1.4 Reference Documents

- TA1 (Technische Anlage 1) Version 039, Stand 31.03.2025
- TA3 (Technische Anlage 3) - Code tables and segment definitions
- TA7 (Technische Anlage 7) - Dispensing data structure
- AMPreisV (Arzneimittelpreisverordnung) - Pharmaceutical pricing regulation
- BtMG (Betäubungsmittelgesetz) - Controlled substances act
- SGB V § 300 - Legal framework for data transmission
- FHIR R4 Standard - Health data interchange standard
- gematik Specifications - E-Rezept technical specifications

---

## 2. General Validation Rules

### 2.1 Timezone and Timestamp Rules

**Reference:** TA1 Section 1, Page 5

#### Rule GEN-001: German Time Zone
```
Severity: ERROR
Condition: All timestamps must be in German time (CET/CEST)
Fields: All datetime fields in Abgabedaten
Implementation:
- Validate timezone is Europe/Berlin
- Accept offset +01:00 (CET) or +02:00 (CEST) depending on date
- Reject timestamps without timezone information
```

#### Rule GEN-002: Effective Date for Field Changes
```
Severity: ERROR
Condition: Field changes based on dispensing date (Abgabedatum), not billing month
Fields: Referenced by Feld ID 5 (TA7) and ZUP-11 (TA3)
Implementation:
- Use dispensing date (Abgabedatum) as reference for applicable rules
- Not the billing month (Abrechnungsmonat)
```

### 2.2 Gross Price (Bruttopreis) Rules

**Reference:** TA1 Section 1, Page 5

#### Rule GEN-003: Gross Price Composition
```
Severity: ERROR
Condition: Bruttopreis must always reflect pharmacy dispensing price per AMPreisV
Fields: Bruttopreis (ID 23 in TA7)
Implementation:
- Gross price = pharmacy sales price according to AMPreisV or contractual regulations
- Must NOT deduct copayment (Zuzahlung)
- Must NOT deduct additional costs (Mehrkosten)
- Must NOT deduct patient contribution (Eigenbeteiligung)
Validation:
- Verify Bruttopreis > 0
- Check that copayments are in separate fields
```

#### Rule GEN-004: VAT Calculation for Statutory Fees
```
Severity: WARNING
Condition: Statutory fees (BTM-Gebühr, Noctu, T-Rezept) must be VAT-adjusted
Fields: Bruttopreis for fee special codes
Implementation:
- Reduce statutory fees by VAT proportion
- Ensure calculated gross prices match legal prices exactly
- TA3 rounding rules do NOT apply to this specific case
Formula:
- Net fee = Gross statutory fee / (1 + VAT_rate)
- VAT rate typically 19% in Germany
```

### 2.3 Special Codes (Sonderkennzeichen) Location

**Reference:** TA1 Section 4.14.1, 4.14.2

#### Rule GEN-005: Special Code Transmission
```
Severity: ERROR
Condition: Special codes for electronic additional data must be in ZDP segments
Fields: Sonderkennzeichen in electronic data
Implementation:
- For paper prescriptions: printed on form
- For E-Rezept: transmitted in Abgabedaten structure
- Each special code maximum once per prescription
- Multiple fees indicated via Factor field (multiples of 1000.000000)
```

---

## 3. Data Format Validations

### 3.1 PZN (Pharmazentralnummer) Format

**Reference:** TA1 Section 4.14.2, Page 39

#### Rule FMT-001: PZN Format Validation
```
Severity: ERROR
Condition: PZN must be exactly 8 digits, alphanumeric with leading zeros
Field: PZN_Sonderkennzeichen
Format: 8-digit alphanumeric
Examples:
  Valid:   "01234567", "00123456"
  Invalid: "1234567" (too short), "123456789" (too long)
Implementation:
- Regex: ^[0-9]{8}$
- Left-pad with zeros if necessary
- Allow special codes (Sonderkennzeichen) as alternatives
```

#### Rule FMT-002: PZN Checksum Validation
```
Severity: WARNING
Condition: PZN checksum validation (if PZN, not special code)
Field: PZN
Implementation:
- Apply PZN checksum algorithm (Modulo 11)
- Last digit is checksum
- Warning only (not error) as some special codes may not follow checksum
```

### 3.2 Timestamp Format Validation

**Reference:** TA1 Section 4.14.2, Page 39

#### Rule FMT-003: ISO 8601 DateTime Format
```
Severity: ERROR
Condition: Manufacturing timestamp must be ISO 8601 compliant
Field: Herstellungsdatum und Zeitpunkt der Herstellung
Formats Accepted:
  - YYYY-MM-DDTHH:MM:00Z (UTC)
  - YYYY-MM-DDThh:mm:ss+zz:zz (with timezone offset)
Examples:
  Valid:   "2025-10-15T14:30:00Z", "2025-10-15T16:30:00+02:00"
  Invalid: "2025-10-15 14:30:00", "15.10.2025 14:30"
Implementation:
- Use ISO 8601 parser
- Validate timezone component present
- Convert to UTC for comparisons
```

### 3.3 Numeric Field Formats

**Reference:** TA1 Section 4.14.2, Page 39

#### Rule FMT-004: Manufacturer Identifier Format
```
Severity: ERROR
Field: Kennzeichen des Herstellenden
Format: 9 digits numeric
Examples:
  Valid:   "123456789", "001234567"
  Invalid: "12345678" (too short), "ABCD12345"
Implementation:
- Regex: ^[0-9]{9}$
- Either pharmacy IK or Avoxa/ABDATA manufacturer code
```

#### Rule FMT-005: Counter Field Formats
```
Severity: ERROR
Fields:
  - Zähler Herstellungssegment: 1-2 digits numeric
  - Zähler Einheit: 1-n digits numeric
  - Zähler Abrechnungsposition: 1-n digits numeric
Implementation:
- Zähler Herstellung: ^[0-9]{1,2}$
- Zähler Einheit: ^[0-9]+$
- Zähler Abrechnungsposition: ^[0-9]+$
- Must start at "1" and be sequential
```

#### Rule FMT-006: Batch Designation Format
```
Severity: WARNING
Field: Chargenbezeichnung
Format: 1-20 alphanumeric characters
Implementation:
- Regex: ^[A-Za-z0-9]{1,20}$
- Optional field
```

#### Rule FMT-007: Factor Identifier Format
```
Severity: ERROR
Field: Faktorkennzeichen
Format: 2-digit alphanumeric
Reference: TA3 Table 8.2.25
Examples: "11", "55", "57"
Implementation:
- Regex: ^[0-9A-Za-z]{2}$
- Cross-reference with TA3 code table 8.2.25
```

#### Rule FMT-008: Factor Value Format
```
Severity: ERROR
Field: Faktor
Format: 1-13 digits (max 6 pre-decimal + 6 post-decimal places)
Examples:
  Valid:   "1000.000000", "250.500000", "1.0", "3000"
  Invalid: "1234567.123456" (too many pre-decimal), "1.1234567" (too many post-decimal)
Implementation:
- Regex: ^[0-9]{1,6}(\.[0-9]{1,6})?$
- Total length ≤ 13 digits including decimal separator
- NEW in Version 039: Trailing zeros flexible (1.0 = 1.000000)
```

#### Rule FMT-009: Price Identifier Format
```
Severity: ERROR
Field: Preiskennzeichen
Format: 2-digit alphanumeric
Reference: TA3 Table 8.2.26
Examples: "11", "13", "14", "15", "90"
Implementation:
- Regex: ^[0-9A-Za-z]{2}$
- Cross-reference with TA3 code table 8.2.26
```

#### Rule FMT-010: Price Value Format
```
Severity: ERROR
Field: Preis
Format: 1-12 digits in EUR (max 9 pre-decimal + 2 post-decimal places)
Examples:
  Valid:   "123.45", "1234567.99", "0.50"
  Invalid: "1234567890.12" (too many pre-decimal), "10.123" (too many post-decimal)
Implementation:
- Regex: ^[0-9]{1,9}(\.[0-9]{1,2})?$
- Must be valid Euro amount
- Exactly 2 decimal places for currency
```

---

## 4. BTM & T-Rezept Validations

### 4.1 BTM Fee Calculation

**Reference:** TA1 Section 4.1.1 b, Page 8

#### Rule BTM-001: E-BTM Fee Special Code
```
Severity: ERROR
Condition: E-BTM and E-T-Rezept prescriptions require fee special codes
Special Codes:
  - BTM-Gebühr: 02567001
  - T-Rezept-Gebühr: [Code from Anhang 1/2]
Fields:
  - PZN: Special code
  - Faktor: Number of controlled substance lines
  - Bruttopreis: Sum of BTM/T-Rezept fees
Implementation:
- Detect E-BTM or E-T-Rezept flag in prescription
- Validate special code present
- Factor = count of BTM/T-Rezept lines
- Price = fee × factor
Note: Implementation only when E-BTM/E-T-Rezept fully rolled out
```

#### Rule BTM-002: All Pharmaceuticals Must Be Listed
```
Severity: ERROR
Condition: All dispensed controlled substances with PZNs, quantities, and prices
Implementation:
- Every BTM/T-Rezept item must have:
  - Valid 8-digit PZN
  - Quantity (Menge)
  - Price (Bruttopreis)
- No omissions allowed
```

#### Rule BTM-003: BTM Seven-Day Validity Rule
```
Severity: WARNING
Condition: BTM prescriptions valid for 7 days (per BtMG)
Implementation:
- Calculate days between prescription date and dispensing date
- Warning if > 7 days
- May indicate expired prescription
Reference: BtMG §3
```

#### Rule BTM-004: BTM Diagnosis Requirement
```
Severity: WARNING
Condition: BTM prescriptions require diagnosis code (ICD-10)
Field: Diagnosis code in prescription
Implementation:
- Check for presence of ICD-10 diagnosis code
- Warning if missing (regulatory requirement per BtMG §3)
```

---

## 5. Special Fee Validations

### 5.1 Messenger Service Fee

**Reference:** TA1 Section 4.1.4 b, Page 9

#### Rule FEE-001: Messenger Service Fee Validation
```
Severity: ERROR
Condition: Delivery service per § 129 Abs. 5g SGB V
Special Code: Botendienstgebühr (from Anhang 1/2)
Fields:
  - Sonderkennzeichen: Botendienst special code
  - Faktor: "1"
  - Bruttopreis: Messenger service fee amount
Scope: Per delivery location and day for prescription medications
Implementation:
- Validate special code for messenger service
- Factor must be exactly "1" or "1.000000"
- Price must be valid messenger service fee
- Only one fee per delivery location per day
```

### 5.2 Noctu Fee Validation

**Reference:** TA1 Section 4.1.2 b, Page 8

#### Rule FEE-002: Noctu (Night Service) Fee
```
Severity: ERROR
Condition: Night service between 20:00-06:00 or weekends/holidays
Special Code: Noctu special code (from Anhang 1/2)
Fields:
  - Sonderkennzeichen: Noctu code
  - Faktor: "1" or multiple
  - Bruttopreis: Noctu fee
Implementation:
- Detect dispensing time between 20:00-06:00
- Or detect weekend (Saturday/Sunday)
- Or detect public holiday
- Apply Noctu special code
- VAT-adjusted fee calculation (per GEN-004)
```

### 5.3 Wiederbeschaffung (Re-procurement) Fee

**Reference:** TA1 Section 4.1.3 b, Page 9

#### Rule FEE-003: Re-procurement Fee
```
Severity: ERROR
Condition: Emergency procurement of unavailable medication
Special Code: Wiederbeschaffung code (from Anhang 1/2)
Implementation:
- Validate special code for re-procurement
- Document unavailability reason
- Apply procurement fee
```

---

## 6. Compounded Preparations (Rezepturen)

### 6.1 General Compounded Preparation Rules

**Reference:** TA1 Section 4.14.2, Pages 38-45

#### Rule REZ-001: Compounded Preparation Identification
```
Severity: ERROR
Condition: Compounded preparations must use proper special codes
Special Codes:
  - Parenteral preparations: 1.7.1 - 1.7.24 range
  - Economic single quantities: 02567053, 02566993
  - Cannabis preparations: 06461446, 06461423, 06460665, 06460694, 06460748, 06460754
  - General compounding: 06460702, 09999011
Implementation:
- Detect compounded preparation by special code
- Route to appropriate validation sub-rules
- Exclude BTM or T-Rezept substances (unless explicitly allowed)
```

### 6.2 Parenteral Preparations (4.14.2 a)

**Reference:** TA1 Section 4.14.2 a, Pages 40-42

#### Rule REZ-002: Parenteral Preparation - Manufacturer ID
```
Severity: ERROR
Condition: Manufacturer ID must be Avoxa/ABDATA code
Field: Kennzeichen des Herstellenden
Implementation:
- 9-digit numeric code
- Assigned by Avoxa/ABDATA on behalf of DAV
- NOT the pharmacy IK
```

#### Rule REZ-003: Parenteral Preparation - Timestamp Validation
```
Severity: ERROR
Condition: Manufacturing timestamp ≤ signature timestamp
Fields:
  - Herstellungsdatum und Zeitpunkt
  - Signature timestamp
Implementation:
- Parse both timestamps to UTC
- Validate: manufacturing_timestamp <= signature_timestamp
Error Message: "Manufacturing timestamp cannot be later than signature timestamp"
```

#### Rule REZ-004: Parenteral Preparation - Counter Sequence
```
Severity: ERROR
Condition: Sequential counter numbering starting from "1"
Fields:
  - Zähler Herstellung (manufacturing counter)
  - Zähler Einheit (unit counter)
  - Zähler Abrechnungsposition (billing position counter)
Implementation:
- Manufacturing counter: starts at 1, increments per manufacturing process
- Unit counter: starts at 1 per manufacturing process, increments per unit
- Billing position counter: starts at 1 per unit, increments per line item
- Must be gapless (no missing numbers)
```

#### Rule REZ-005: Parenteral Preparation - Factor as Promilleanteil
```
Severity: ERROR
Condition: Factor expressed as per mille (per thousand) value
Field: Faktor
Implementation:
- 1 whole package = 1000.000000
- 3 whole packages = 3000.000000
- Partial package: calculate proportionally
- Exception: Special codes without quantity reference = 1.000000
Examples:
  - Full bag = "1000.000000"
  - 3 bags = "3000.000000"
  - 1/2 bag = "500.000000"
```

#### Rule REZ-006: Parenteral Preparation - Week Supply Limit
```
Severity: WARNING
Condition: Maximum 1 week supply of identical preparations
Implementation:
- Count number of identical units
- Warning if > 7 units (may indicate over-prescribing)
- Informational only (not error)
```

### 6.3 Economic Single Quantities (4.14.2 b)

**Reference:** TA1 Section 4.14.2 b, Pages 43-44

#### Rule REZ-007: Economic Single Quantity - Manufacturer ID Type
```
Severity: ERROR
Condition: Manufacturer ID is pharmacy IK, NOT Avoxa/ABDATA code
Special Codes: 02567053 (individual dispensing), 02566993 (weekly blister)
Field: Kennzeichen des Herstellenden
Implementation:
- 9-digit pharmacy Institutionskennzeichen (IK)
- Exception: If pharmacy also produces parenteral preparations, may use Avoxa/ABDATA code
```

#### Rule REZ-008: Economic Single Quantity - Timestamp Validation
```
Severity: ERROR
Condition: Manufacturing timestamp ≤ signature timestamp
Special Codes: 02567053, 02566993
Implementation:
- Same as REZ-003
- Validate: manufacturing_timestamp <= signature_timestamp
```

#### Rule REZ-009: Economic Single Quantity - Counter for 02567053
```
Severity: ERROR
Condition: Individual dispensing always has counter = "1"
Special Code: 02567053
Fields:
  - Zähler Herstellung: "1"
  - Zähler Einheit: "1"
Implementation:
- Both counters must be exactly "1"
- Single dispensing event
```

#### Rule REZ-010: Economic Single Quantity - Counter for 02566993
```
Severity: ERROR
Condition: Weekly blister has sequential counters
Special Code: 02566993
Fields:
  - Zähler Herstellung: Sequential starting from "1"
  - Zähler Einheit: Sequential per manufacturing process starting from "1"
Implementation:
- Manufacturing counter: increments per manufacturing batch
- Unit counter: increments per blister/unit within batch
- Must be gapless
```

#### Rule REZ-011: Economic Single Quantity - Factor Identifier
```
Severity: ERROR
Condition: Factor identifier must always be "11"
Special Codes: 02567053, 02566993
Field: Faktorkennzeichen
Implementation:
- Hardcoded value: "11"
- Reference: TA3 Table 8.2.25
```

#### Rule REZ-012: Economic Single Quantity - Partial Quantity Factor
```
Severity: ERROR
Condition: Factor calculation for partial quantities
Field: Faktor
Implementation:
- Full package = 1000.000000
- Partial quantity = (dispensed_quantity / package_quantity) × 1000.000000
Example:
  - 7 tablets from 28-tablet package = (7/28) × 1000 = 250.000000
  - 3 full packages = 3000.000000
```

### 6.4 Cannabis & General Compounding (4.14.2 c)

**Reference:** TA1 Section 4.14.2 c, Pages 45-46

#### Rule REZ-013: Cannabis/Compounding - Special Codes
```
Severity: ERROR
Condition: Must use correct special code for preparation type
Special Codes:
  Cannabis (Annex 10):
    - 06461446: Cannabis dried flowers
    - 06461423: Cannabis extracts
    - 06460665: Dronabinol preparation type 1
    - 06460694: Dronabinol preparation type 2
    - 06460748: Cannabis preparation type 3
    - 06460754: Cannabis preparation type 4
  General compounding (§§ 4,5 AMPreisV):
    - 06460702: Standard compounding
    - 09999011: Alternative compounding
Implementation:
- Validate special code from allowed list
- Ensure NO BTM or T-Rezept substances (unless Cannabis specifically)
```

#### Rule REZ-014: Cannabis/Compounding - Manufacturer ID Type
```
Severity: ERROR
Condition: Manufacturer ID is pharmacy IK
Field: Kennzeichen des Herstellenden
Implementation:
- 9-digit pharmacy Institutionskennzeichen (IK)
- Exception: If pharmacy produces parenteral preparations, may use Avoxa/ABDATA code
```

#### Rule REZ-015: Cannabis/Compounding - Manufacturing Timestamp
```
Severity: ERROR
Condition: Timestamp = dispensing date + 00:00
Field: Herstellungsdatum und Zeitpunkt
Implementation:
- Extract dispensing date (Abgabedatum)
- Set time component to "00:00"
- Format: YYYY-MM-DDT00:00:00+zz:zz
Example: If dispensed 2025-10-15 at 14:30, timestamp = "2025-10-15T00:00:00+02:00"
```

#### Rule REZ-016: Cannabis/Compounding - Counter Values
```
Severity: ERROR
Condition: All counters must be "1" (single preparation per prescription)
Fields:
  - Zähler Herstellung: "1"
  - Zähler Einheit: "1"
Implementation:
- Hardcoded: both counters = "1"
- Only one compounded preparation per prescription allowed
```

#### Rule REZ-017: Cannabis/Compounding - Factor Identifier
```
Severity: ERROR
Condition: Factor identifier must be "11"
Field: Faktorkennzeichen
Implementation:
- Hardcoded value: "11"
- Reference: TA3 Table 8.2.25
```

#### Rule REZ-018: Cannabis/Compounding - Factor as Promilleanteil
```
Severity: ERROR
Condition: Factor calculated as per mille
Field: Faktor
Implementation:
- Full package = 1000.000000
- Partial package = proportional calculation
- Special codes (1.1.1-1.2.2, 1.3.1, 1.3.2, 1.6.5, 1.10.2, 1.10.3) = 1.000000
Example:
  - Full 50g package = "1000.000000"
  - 2g extracted from 50g = (2/50) × 1000 = "40.000000"
```

#### Rule REZ-019: Cannabis/Compounding - Price Identifier
```
Severity: ERROR
Condition: Price identifier based on compounding type
Field: Preiskennzeichen
Reference: TA3 Table 8.2.26
Implementation:
  Annex 10 preparations:
    - "14" = Price per AMPreisV §§ 4,5 (including fixed/percentage surcharges)
    - "14" = If actual pharmacy purchase price applies

  General compounding (06460702, 09999011):
    Substances/containers in Annex 1/2:
      - "14" = Price per Annex 1/2 + percentage surcharges §§ 4,5 Abs.1 Nr.1 AMPreisV
    Substances/containers NOT in Annex 1/2:
      - "13" = Actual pharmacy purchase price + percentage surcharges
    Partial quantities of finished pharmaceuticals:
      - "11" = Pharmacy purchase price per AMPreisV + percentage surcharges
```

#### Rule REZ-020: Cannabis/Compounding - Price Adjustment for Large Quantities
```
Severity: WARNING
Condition: Adjust price when compounding quantity exceeds base quantity
Reference: AMPreisV § 5 Abs. 3
Implementation:
- Base quantity per AMPreisV: e.g., 300g
- If compounding quantity ≤ base quantity: factor = 1000.000000, price = base price
- If compounding quantity > base quantity but ≤ 2× base: factor = 1500.000000, price = 1.5× base price
Example:
  - Base: 300g = 6€ → Factor "1000.000000", Price "6.00"
  - 100g ≤ 300g → Factor "1000.000000", Price "6.00"
  - 500g (> 300g, ≤ 600g) → Factor "1500.000000", Price "9.00"
```

---

## 7. Cannabis-Specific Validations

**Reference:** TA1 Section 4.14, Version 039 - Complete revision for Cannabis regulations

### 7.1 Cannabis Preparation Identification

#### Rule CAN-001: Cannabis Special Codes
```
Severity: ERROR
Condition: Cannabis preparations per § 31 Abs. 6 SGB V
Special Codes (Annex 10):
  - 06461446: Dried cannabis flowers (getrocknete Blüten)
  - 06461423: Cannabis extracts (Extrakte)
  - 06460665: Dronabinol preparation type 1
  - 06460694: Dronabinol preparation type 2
  - 06460748: Cannabis preparation type 3
  - 06460754: Cannabis preparation type 4
Implementation:
- Detect cannabis preparation by special code
- Apply cannabis-specific validation rules
- Ensure compliance with § 31 Abs. 6 SGB V
```

#### Rule CAN-002: Cannabis - No BTM/T-Rezept Substances
```
Severity: ERROR
Condition: Cannabis preparations must NOT contain BTM or T-Rezept substances
Reference: TA1 Section 4.14.2 general rules
Implementation:
- Scan all ingredients for BTM classification
- Error if BTM/T-Rezept substance detected
- Separate billing process for BTM-Cannabis
```

#### Rule CAN-003: Cannabis - Faktor Field Value
```
Severity: ERROR
Condition: Factor must be "1" in Abgabedaten special code line
Field: Faktor in dispensing data
Implementation:
- Main special code line: Faktor = "1" or "1.000000"
- Detailed manufacturing data: calculated per REZ-018
```

#### Rule CAN-004: Cannabis - Bruttopreis Calculation
```
Severity: ERROR
Condition: Gross price = total amount to be billed
Field: Bruttopreis in dispensing data
Implementation:
- Calculate total from all ingredients + labor + surcharges
- Include all applicable fees
- Apply AMPreisV pricing rules
- Verify against Annex 10 pricing tables
```

#### Rule CAN-005: Cannabis - Manufacturing Data Required
```
Severity: ERROR
Condition: All cannabis preparations require detailed manufacturing data
Fields: Complete Herstellungssegment structure
Implementation:
- Manufacturer ID (pharmacy IK)
- Manufacturing timestamp (dispensing date + 00:00)
- Counters (all "1")
- Complete ingredient list with PZN, factors, prices
- Surcharges and fees
```

---

## 8. Economic Single Quantities

### 8.1 Individual Dispensing (02567053)

**Reference:** TA1 Section 4.11, 4.14.2 b

#### Rule ESQ-001: Individual Dispensing - Special Code
```
Severity: ERROR
Special Code: 02567053
Condition: Auseinzelung (individual dispensing from larger package)
Reference: Rahmenvertrag § 129 SGB V
Implementation:
- Validate special code 02567053
- Apply individual dispensing rules
- Document source package PZN
```

#### Rule ESQ-002: Individual Dispensing - Single Unit
```
Severity: ERROR
Condition: Always exactly one unit
Counters:
  - Zähler Herstellung: "1"
  - Zähler Einheit: "1"
Implementation:
- Both counters must be "1"
- Only one dispensing event allowed
```

### 8.2 Patient-Specific Partial Quantities (02566993)

**Reference:** TA1 Section 4.13, 4.14.2 b

#### Rule ESQ-003: Patient-Specific Partial Quantities - Special Code
```
Severity: ERROR
Special Code: 02566993
Condition: Weekly blister or similar patient-specific packaging
Implementation:
- Validate special code 02566993
- Apply multi-unit sequential numbering
- Document all source packages
```

#### Rule ESQ-004: Weekly Blister - Multiple Units
```
Severity: ERROR
Condition: Sequential numbering for multiple units
Counters:
  - Zähler Herstellung: Sequential starting "1" per batch
  - Zähler Einheit: Sequential starting "1" per batch
Implementation:
- Manufacturing counter increments per batch
- Unit counter increments per blister within batch
- Must be gapless sequence
Example:
  - Batch 1, Blister 1: Herstellung=1, Einheit=1
  - Batch 1, Blister 2: Herstellung=1, Einheit=2
  - Batch 1, Blister 3: Herstellung=1, Einheit=3
  - Batch 2, Blister 1: Herstellung=2, Einheit=1
```

---

## 9. Special Case Validations

### 9.1 § 3 Abs. 4 Prescriptions

**Reference:** TA1 Section 4.5.2, Page 11

#### Rule SPC-001: Low-Price Medication Handling
```
Severity: ERROR
Condition: Gross price ≤ copayment amount
Fields:
  - Bruttopreis (ID 23): Pharmacy sales price
  - Zuzahlung (ID 27, controlled by ID26=0): Copayment amount
Implementation:
- Validate: Bruttopreis <= Zuzahlungsbetrag
- Ensure both fields populated correctly
- If additional costs: separate Mehrkosten field (ID 27, ID26=1)
- Include in GesamtBrutto (ID 7) and GesamtZuzahlung (ID 6)
```

#### Rule SPC-002: Additional Costs for § 3 Abs. 4
```
Severity: WARNING
Condition: Patient pays additional costs beyond copayment
Fields:
  - Bruttopreis (ID 23): Pharmacy sales price
  - Zuzahlung (ID 27, ID26=0): Copayment
  - Mehrkosten (ID 27, ID26=1): Additional costs
Implementation:
- All three fields required if Mehrkosten > 0
- Include all in totals (GesamtBrutto, GesamtZuzahlung)
```

### 9.2 Artificial Insemination Prescriptions

**Reference:** TA1 Section 4.9.2, Pages 14-15

#### Rule SPC-003: Artificial Insemination Flag
```
Severity: ERROR
Condition: Prescription flagged for artificial insemination
Field: Zuzahlungsstatus (copayment status field)
Implementation:
- Check for artificial insemination flag in E-Rezept
- Apply special billing rules
```

#### Rule SPC-004: 50% Patient Contribution
```
Severity: ERROR
Condition: Patient pays 50% of pharmacy sales price or 50% of fixed price
Field: Kostenbetrag Kategorie "2"
Implementation:
- If AVK ≤ Festbetrag: contribution = 50% × AVK
- If AVK > Festbetrag: contribution = 50% × Festbetrag
- Additional costs (AVK - Festbetrag) in Kategorie "1"
- Copayment in Kategorie "0" = "0.00"
Formula:
  PatientContribution = min(AVK, Festbetrag) × 0.5
  Mehrkosten = max(0, AVK - Festbetrag)
```

#### Rule SPC-005: Artificial Insemination - Compounding
```
Severity: ERROR
Condition: Compounding or economic single quantities for artificial insemination
Special Codes: See REZ-013 + 09999643
Implementation:
- Standard compounding rules apply (Section 4.14.2 b/c)
- Additional special code 09999643 required
- 50% pricing calculations
```

### 9.3 Deviation from Standard Dispensing

**Reference:** TA1 Section 4.10, Pages 16-19

#### Rule SPC-006: Deviation Special Code
```
Severity: ERROR
Special Code: 02567024
Condition: Deviation from standard dispensing per § 129 SGB V framework
Field: Faktor (3-digit code indicating reason)
Implementation:
- Position 1: First medication
- Position 2: Second medication
- Position 3: Third medication
Values per position:
  "1" = Standard dispensing per § 129 or empty line
  "2" = Unavailability of contract medication (all selection ranges)
  "3" = Unavailability of low-price medication (generic market)
  "4" = Both contract and low-price unavailable
  "5" = Emergency case (dringender Fall)
  "6" = Emergency + unavailability combination
  "7" = Patient-requested medication (Wunscharzneimittel)
  "8" = Pharmacist concerns per § 17 Abs. 5 S. 2 ApBetrO
  "9" = Concerns against both contract and low-price medication
Example:
  Factor "243" = Med1: unavailable contract (2), Med2: unavailable generic (4), Med3: unavailable generic (3)
```

### 9.4 Institution Identifier (IK)

**Reference:** TA1 Section 4.6.2, Page 12

#### Rule SPC-007: IK Format for E-Rezept
```
Severity: ERROR
Condition: Full 9-digit IK required for E-Rezept
Field: Institutionskennzeichen
Implementation:
- Must be exactly 9 digits
- For public pharmacies: includes classification code "30"
- For other authorized service providers: full 9-digit IK
Format: ^[0-9]{9}$
```

---

## 10. Price and Factor Calculations

### 10.1 Promilleanteil (Per Mille) Calculations

**Reference:** TA1 Sections 4.14.1 a/b/c/d, 4.14.2 a/b/c

#### Rule CALC-001: Standard Promilleanteil Formula
```
Severity: ERROR
Condition: Factor expressed as per thousand (Promilleanteil)
Formula:
  Factor = (Dispensed_Quantity / Package_Quantity) × 1000
Examples:
  - 1 full package: (1/1) × 1000 = 1000.000000
  - 3 full packages: (3/1) × 1000 = 3000.000000
  - 7 tablets from 28: (7/28) × 1000 = 250.000000
  - 2g from 50g package: (2/50) × 1000 = 40.000000
  - 1 unit from 10 units: (1/10) × 1000 = 100.000000
Implementation:
- Calculate precise decimal value
- Format with up to 6 decimal places
- May omit trailing zeros (per FMT-008)
```

#### Rule CALC-002: Special Code Factor Exception
```
Severity: ERROR
Condition: Special codes without quantity reference always use factor 1.000000
Special Codes: 1.1.1-1.2.2, 1.3.1, 1.3.2, 1.6.5, 1.10.2, 1.10.3
Implementation:
- Hardcode factor = 1.000000 (or 1.0, 1)
- Reason: No unambiguous quantity reference for these codes
```

#### Rule CALC-003: Artificial Insemination Special Code Factor
```
Severity: ERROR
Special Code: 09999643 (artificial insemination marker)
Condition: Factor always 1000.000000, Price always 0.00
Implementation:
- Faktor = "1000.000000"
- Preis = "0.00" or ",00"
- Preiskennzeichen = "90"
```

### 10.2 Price Calculations

#### Rule CALC-004: Basic Price Calculation
```
Severity: ERROR
Condition: Price derived from factor and price identifier
Formula:
  If factor relates to quantity:
    Preis = (Faktor / 1000) × Base_Price_per_PriceIdentifier
  If factor = 1.000000 for special code:
    Preis = Actual_amount_for_dispensed_quantity
Implementation:
- Retrieve base price based on Preiskennzeichen (TA3 8.2.26)
- Apply factor calculation
- Round to 2 decimal places (EUR)
Examples:
  - Base price 100€, factor 1000.000000 → 100.00€
  - Base price 100€, factor 250.000000 → 25.00€
  - Base price 100€, factor 3000.000000 → 300.00€
```

#### Rule CALC-005: VAT Exclusion in Price Field
```
Severity: ERROR
Condition: Prices in ZDP/manufacturing data are WITHOUT VAT
Field: Preis (ZDP-06)
Implementation:
- All prices in compounding data exclude VAT
- VAT added later in final billing
- Ensure price is net amount (ohne USt.)
```

#### Rule CALC-006: Price Identifier Lookup
```
Severity: ERROR
Field: Preiskennzeichen (TA3 Table 8.2.26)
Common Codes:
  "11" = Pharmacy purchase price per AMPreisV
  "13" = Actual pharmacy purchase price
  "14" = Billing price per AMPreisV §§ 4,5 (including surcharges)
  "15" = Contracted billing price between pharmacy and insurance
  "90" = Special price (e.g., artificial insemination marker = 0.00)
Implementation:
- Cross-reference with TA3 table 8.2.26
- Validate code exists
- Apply corresponding pricing rule
```

### 10.3 Decimal Place Handling

**Reference:** TA1 Section 4.14.2, Page 38 (NEW in Version 039)

#### Rule CALC-007: Flexible Trailing Zeros
```
Severity: INFO
Condition: Number of trailing zeros in factors does not matter
Field: Faktor
Examples (all equivalent):
  - "1"
  - "1.0"
  - "1.000000"
  - "1000"
  - "1000.0"
  - "1000.000000"
Implementation:
- Accept any representation within max decimal places
- Normalize internally for calculations
- FHIR may represent as decimal without trailing zeros
- Maximum decimal places still enforced (6 post-decimal)
```

---

## 11. Validation Rule Priority Matrix

### 11.1 Severity Levels

| Severity | Description | Action |
|----------|-------------|--------|
| **ERROR** | Critical validation failure, prescription cannot be billed | Block submission, must be corrected |
| **WARNING** | Potential issue, may cause rejection | Allow submission with warning notification |
| **INFO** | Informational message, best practice | No blocking, informational only |

### 11.2 Validation Execution Order

1. **Phase 1: Format Validations** (FMT-xxx)
   - Execute first, fail fast on malformed data
   - PZN format, timestamp format, numeric formats
   - If any ERROR: stop processing

2. **Phase 2: General Rules** (GEN-xxx)
   - Timezone, gross price composition, special code location
   - If any ERROR: stop processing

3. **Phase 3: Prescription Type Detection**
   - Identify: BTM, Cannabis, Compounding, Standard, Special Cases
   - Route to appropriate specialized validators

4. **Phase 4: Type-Specific Validations**
   - BTM (BTM-xxx)
   - Cannabis (CAN-xxx)
   - Compounding (REZ-xxx)
   - Economic Single Quantities (ESQ-xxx)
   - Special Cases (SPC-xxx)
   - If any ERROR: accumulate errors

5. **Phase 5: Calculations** (CALC-xxx)
   - Price and factor calculations
   - Cross-field validations
   - Totals verification

6. **Phase 6: Fee Validations** (FEE-xxx)
   - Statutory fees (BTM, Noctu, Messenger)
   - VAT calculations
   - If any WARNING: accumulate warnings

### 11.3 Critical Path Rules (Must Pass)

The following rules are **critical path** and must pass for billing submission:

- **GEN-001**: German timezone (all timestamps)
- **GEN-003**: Gross price composition
- **FMT-001**: PZN format
- **FMT-003**: ISO 8601 timestamp format
- **FMT-008**: Factor format
- **FMT-010**: Price format
- **REZ-003**: Manufacturing timestamp ≤ signature timestamp
- **REZ-004**: Sequential counter validation
- **REZ-015**: Cannabis manufacturing timestamp (if Cannabis)
- **CAN-002**: No BTM in Cannabis preparations (if Cannabis)
- **CALC-001**: Promilleanteil calculation

---

## 12. Error Codes and Messages

### 12.1 Error Code Structure

Format: `[Category]-[Number]-[Severity]`

Examples:
- `FMT-001-E`: Format validation error #001
- `BTM-003-W`: BTM validation warning #003
- `CAN-004-E`: Cannabis validation error #004

### 12.2 Standard Error Response Format

```json
{
  "validationResult": "FAILED" | "PASSED_WITH_WARNINGS" | "PASSED",
  "timestamp": "2025-10-15T14:30:00Z",
  "prescriptionId": "160.000.000.000.000.12",
  "errors": [
    {
      "code": "FMT-001-E",
      "severity": "ERROR",
      "field": "PZN",
      "value": "123456",
      "message": "PZN must be exactly 8 digits. Found: 6 digits.",
      "suggestion": "Pad PZN with leading zeros: '00123456'",
      "reference": "TA1 Section 4.14.2, Page 39"
    }
  ],
  "warnings": [
    {
      "code": "BTM-003-W",
      "severity": "WARNING",
      "field": "Abgabedatum",
      "message": "BTM prescription dispensed 9 days after prescription date. Maximum validity is 7 days per BtMG §3.",
      "reference": "BtMG §3"
    }
  ],
  "info": []
}
```

### 12.3 Common Error Messages

#### Format Errors (FMT-xxx)

```
FMT-001-E: "PZN must be exactly 8 digits. Found: {actual_length} digits."
FMT-003-E: "Manufacturing timestamp must be in ISO 8601 format. Found: '{actual_value}'."
FMT-008-E: "Factor exceeds maximum decimal places. Max 6 pre-decimal + 6 post-decimal. Found: '{actual_value}'."
FMT-010-E: "Price exceeds maximum decimal places. Max 9 pre-decimal + 2 post-decimal. Found: '{actual_value}'."
```

#### General Errors (GEN-xxx)

```
GEN-001-E: "Timestamp must be in German time (CET/CEST). Found timezone: '{actual_timezone}'."
GEN-003-E: "Bruttopreis must not have copayment deducted. Expected: pharmacy sales price per AMPreisV."
GEN-004-W: "Statutory fee not properly VAT-adjusted. Expected net fee: {expected}, Found: {actual}."
```

#### BTM Errors (BTM-xxx)

```
BTM-001-E: "E-BTM prescription missing BTM fee special code (02567001)."
BTM-002-E: "Controlled substance line missing required fields (PZN, quantity, price)."
BTM-003-W: "BTM prescription dispensed {days} days after prescription date. Maximum validity is 7 days per BtMG §3."
BTM-004-W: "BTM prescription missing ICD-10 diagnosis code (required per BtMG §3)."
```

#### Compounding Errors (REZ-xxx)

```
REZ-003-E: "Manufacturing timestamp ({manufacturing_ts}) cannot be later than signature timestamp ({signature_ts})."
REZ-004-E: "Counter sequence has gap. Expected: {expected}, Found: {actual}."
REZ-005-E: "Factor must be expressed as Promilleanteil (per mille). Example: 1 package = 1000.000000."
REZ-015-E: "Cannabis manufacturing timestamp must be dispensing date + 00:00. Expected: '{expected}', Found: '{actual}'."
REZ-016-E: "Cannabis preparation counters must all be '1'. Found: Herstellung={h}, Einheit={e}."
```

#### Cannabis Errors (CAN-xxx)

```
CAN-001-E: "Invalid Cannabis special code. Expected one of: 06461446, 06461423, 06460665, 06460694, 06460748, 06460754. Found: '{actual}'."
CAN-002-E: "Cannabis preparation contains BTM or T-Rezept substances. This is not allowed per TA1 Section 4.14.2."
CAN-003-E: "Cannabis special code line must have Factor = '1'. Found: '{actual}'."
CAN-005-E: "Cannabis preparation missing required manufacturing data (Herstellungssegment)."
```

#### Calculation Errors (CALC-xxx)

```
CALC-001-E: "Factor (Promilleanteil) calculation incorrect. Expected: {expected}, Found: {actual}."
CALC-004-E: "Price calculation incorrect. Formula: (Factor / 1000) × Base_Price. Expected: {expected}, Found: {actual}."
CALC-006-E: "Invalid price identifier (Preiskennzeichen). Code '{actual}' not found in TA3 Table 8.2.26."
```

#### Special Case Errors (SPC-xxx)

```
SPC-001-E: "For § 3 Abs. 4 prescriptions, Bruttopreis must be ≤ Zuzahlung. Found: Bruttopreis={bruttopreis}, Zuzahlung={zuzahlung}."
SPC-004-E: "Artificial insemination patient contribution calculation incorrect. Expected 50% of min(AVK, Festbetrag). Expected: {expected}, Found: {actual}."
SPC-006-E: "Deviation special code (02567024) has invalid factor code. Expected 3-digit code with values 1-9. Found: '{actual}'."
SPC-007-E: "Institution identifier (IK) must be exactly 9 digits. Found: {actual_length} digits."
```

### 12.4 Suggested Corrections

Include actionable suggestions with errors:

```json
{
  "code": "FMT-001-E",
  "suggestion": "Pad PZN with leading zeros. Example: '123456' → '00123456'"
}
```

```json
{
  "code": "REZ-003-E",
  "suggestion": "Adjust manufacturing timestamp to be equal to or earlier than signature timestamp."
}
```

```json
{
  "code": "CAN-002-E",
  "suggestion": "Remove BTM substances from Cannabis preparation or use separate BTM billing process."
}
```

---

## 13. Implementation Guidelines

### 13.1 Validation Architecture

Recommended validator architecture:

```
ValidationEngine
├── FormatValidators (Phase 1)
│   ├── PznFormatValidator (FMT-001, FMT-002)
│   ├── TimestampFormatValidator (FMT-003)
│   ├── NumericFormatValidator (FMT-004 through FMT-010)
│   └── ...
├── GeneralValidators (Phase 2)
│   ├── TimezoneValidator (GEN-001)
│   ├── GrossPriceValidator (GEN-003, GEN-004)
│   └── ...
├── PrescriptionTypeDetector (Phase 3)
│   ├── DetectBTM()
│   ├── DetectCannabis()
│   ├── DetectCompounding()
│   └── DetectSpecialCase()
├── SpecializedValidators (Phase 4)
│   ├── BtmValidator (BTM-xxx)
│   ├── CannabisValidator (CAN-xxx)
│   ├── CompoundingValidator (REZ-xxx)
│   ├── EconomicSingleQuantityValidator (ESQ-xxx)
│   └── SpecialCaseValidator (SPC-xxx)
├── CalculationValidators (Phase 5)
│   ├── PromilleanteilCalculator (CALC-001, CALC-002, CALC-003)
│   ├── PriceCalculator (CALC-004, CALC-005, CALC-006)
│   └── DecimalNormalizer (CALC-007)
└── FeeValidators (Phase 6)
    ├── BtmFeeValidator (FEE-xxx, BTM-001)
    ├── NoctuFeeValidator (FEE-002)
    └── MessengerFeeValidator (FEE-001)
```

### 13.2 External Data Dependencies

The validator requires access to:

1. **TA3 Code Tables**
   - Table 8.2.25: Faktorkennzeichen (Factor identifiers)
   - Table 8.2.26: Preiskennzeichen (Price identifiers)
   - Other relevant code tables

2. **ABDA Database**
   - PZN validation and lookup
   - Drug authorization status
   - Current prices (for calculation validation)

3. **Annex Tables**
   - Anhang 1: Federal special codes (Sonderkennzeichen)
   - Anhang 2: Insurance-pharmacy contracted special codes
   - Hilfstaxe Annexes 1, 2, 4, 5, 6, 7, 10

4. **Lauer-Taxe API** (optional)
   - Real-time PZN validation
   - Current drug information
   - Pricing data

### 13.3 Performance Considerations

- **Validation Speed Target:** <500ms per E-Rezept (per product brief)
- **Caching Strategy:**
  - Cache TA3 code tables (refresh daily)
  - Cache ABDA data (refresh hourly)
  - Cache special code lists (refresh on TA version update)
- **Fail-Fast Principle:**
  - Execute format validations first
  - Stop on critical errors (don't continue if PZN invalid)
- **Parallel Validation:**
  - Independent validators can run in parallel
  - Aggregate results at end

### 13.4 Testing Strategy

#### Unit Tests
- Test each validation rule individually
- Use test cases from TA1 examples
- Cover all error paths

#### Integration Tests
- Test complete prescription validation flows
- Test BTM, Cannabis, Compounding scenarios
- Test special cases (artificial insemination, etc.)

#### Regression Tests
- Maintain test suite for each TA version
- Ensure backward compatibility where applicable
- Test TA version transitions

#### Test Data
- Use anonymized real E-Rezept data
- Create synthetic test cases for edge conditions
- Cover all special code combinations

---

## 14. Version History and Change Management

### 14.1 TA1 Version Tracking

| TA1 Version | Effective Date | Key Changes | Validator Impact |
|-------------|----------------|-------------|------------------|
| 039 | 2025-10-01 | Cannabis regulations (§4.14), Decimal places (§4.14.2), BTM fees (§5) | HIGH - Major validation rule changes |
| 038 | 2022-11-16 | Mehrkosten clarification (§4.5.2), Annexes externalized | MEDIUM - Rule refinements |
| 037 | 2022-06-27 | Artificial insemination (§4.9), Cannabis additions (§4.14.1) | HIGH - New special cases |

### 14.2 Validator Version Roadmap

**Version 1.0 (MVP)**
- Core format validations (FMT-xxx)
- General rules (GEN-xxx)
- Basic BTM validation (BTM-001, BTM-002)
- Standard compounding (REZ-001 through REZ-006)

**Version 1.1**
- Complete BTM validation (BTM-003, BTM-004)
- Cannabis validations (CAN-xxx)
- Economic single quantities (ESQ-xxx)
- Advanced compounding (REZ-007 through REZ-020)

**Version 1.2**
- Special case validations (SPC-xxx)
- Fee validations (FEE-xxx)
- Complete calculation validations (CALC-xxx)
- Performance optimizations

**Version 2.0**
- ABDA API integration
- Real-time PZN lookup
- Lauer-Taxe integration
- Advanced pricing validation

---

## Appendix A: Quick Reference Tables

### A.1 Special Codes by Category

| Category | Code | Description |
|----------|------|-------------|
| **BTM Fees** | 02567001 | BTM-Gebühr (Controlled substance fee) |
| **Cannabis** | 06461446 | Cannabis dried flowers |
| | 06461423 | Cannabis extracts |
| | 06460665 | Dronabinol preparation type 1 |
| | 06460694 | Dronabinol preparation type 2 |
| | 06460748 | Cannabis preparation type 3 |
| | 06460754 | Cannabis preparation type 4 |
| **Compounding** | 06460702 | Standard compounding (§§4,5 AMPreisV) |
| | 09999011 | Alternative compounding |
| **Economic Qty** | 02567053 | Individual dispensing (Auseinzelung) |
| | 02566993 | Weekly blister (patient-specific partial quantities) |
| **Substitution** | 02567107 | Substitution preparation type 1 |
| | 02567113 | Substitution preparation type 2 |
| | 02567136 | Substitution preparation type 3 |
| | 09999086 | Substitution preparation type 4 |
| | 06461506 | Substitution preparation type 5 |
| | 06461512 | Substitution preparation type 6 |
| **Deviations** | 02567024 | Deviation from standard dispensing |
| **Art. Insem.** | 09999643 | Artificial insemination marker |
| **Surcharges** | 06460518 | General surcharge |

### A.2 Factor Identifier Codes (TA3 8.2.25)

| Code | Description |
|------|-------------|
| 11 | Standard factor (Promilleanteil) |
| 55 | Dose in milligrams (for substitution) |
| 57 | Alternative dose specification |

### A.3 Price Identifier Codes (TA3 8.2.26)

| Code | Description |
|------|-------------|
| 11 | Pharmacy purchase price per AMPreisV |
| 13 | Actual pharmacy purchase price |
| 14 | Billing price per AMPreisV §§4,5 (with surcharges) |
| 15 | Contracted billing price (pharmacy-insurance agreement) |
| 90 | Special price (e.g., 0.00 for markers) |

### A.4 Validation Rule Quick Lookup

| Scenario | Key Rules to Check |
|----------|-------------------|
| **Standard E-Rezept** | GEN-001, GEN-003, FMT-001, FMT-003, FMT-008, FMT-010 |
| **BTM Prescription** | BTM-001, BTM-002, BTM-003, BTM-004, GEN-004 |
| **Cannabis Preparation** | CAN-001 through CAN-005, REZ-013 through REZ-020 |
| **Weekly Blister** | ESQ-003, ESQ-004, REZ-007 through REZ-012 |
| **Artificial Insemination** | SPC-003, SPC-004, SPC-005, CALC-003 |
| **Compounding** | REZ-001 through REZ-020, CALC-001, CALC-004 |

---

## Appendix B: Glossary

| Term | German | Definition |
|------|--------|------------|
| **AMPreisV** | Arzneimittelpreisverordnung | Pharmaceutical Pricing Regulation |
| **ABDA** | Bundesvereinigung Deutscher Apothekerverbände | Federal Union of German Pharmacy Associations |
| **Abgabedaten** | Dispensing Data | Data structure for E-Rezept dispensing information |
| **BtM** | Betäubungsmittel | Controlled substances/narcotics |
| **BtMG** | Betäubungsmittelgesetz | Controlled Substances Act |
| **Bruttopreis** | Gross Price | Pharmacy sales price before copayment deduction |
| **Eigenbeteiligung** | Patient Contribution | Patient's share (e.g., 50% for artificial insemination) |
| **Faktor** | Factor | Quantity multiplier (often as Promilleanteil) |
| **Faktorkennzeichen** | Factor Identifier | Code indicating factor type (TA3 8.2.25) |
| **IK** | Institutionskennzeichen | Institution identifier (9-digit code) |
| **Mehrkosten** | Additional Costs | Costs beyond fixed price paid by patient |
| **Noctu** | Night Service | After-hours pharmacy service (20:00-06:00) |
| **Preiskennzeichen** | Price Identifier | Code indicating price type (TA3 8.2.26) |
| **Promilleanteil** | Per Mille Value | Factor expressed per thousand (‰) |
| **PZN** | Pharmazentralnummer | Central Pharmaceutical Number (8-digit drug ID) |
| **Rezeptur** | Compounded Preparation | Pharmacy-compounded medication |
| **Sonderkennzeichen** | Special Code | Special identifier for fees, services, preparations |
| **TA1/TA3/TA7** | Technische Anlagen | Technical Annexes to billing agreement |
| **T-Rezept** | Thalidomide Prescription | Special prescription for thalidomide-related drugs |
| **Zuzahlung** | Copayment | Patient's statutory copayment |

---

## Document Control

**Approval:**
- [ ] Product Manager Review
- [ ] Technical Lead Review
- [ ] Compliance Officer Review
- [ ] ASW Genossenschaft Approval

**Distribution:**
- Development Team
- QA Team
- Compliance Team
- ASW Genossenschaft Stakeholders

**Next Review Date:** Upon release of TA1 Version 040

**Contact:**
- Technical Questions: dev-team@erezept-validator.local
- Compliance Questions: compliance@erezept-validator.local

---

*End of Technical Specification*
