# Billing Validation Examples and Workflows

## Overview

This document provides practical examples of billing scenarios and their validation workflows. Each example includes the prescription data, validation steps, and expected outcomes.

---

## Example 1: Standard Prescription Drug with PZN

### Scenario
Regular prescription medication dispensed with standard pricing.

### Input Data
```json
{
  "pzn": "01234567",
  "sok": null,
  "faktorkennzeichen": null,
  "preiskennzeichen": "11",
  "vat_rate": 2,
  "dispensing_date": "2026-01-15",
  "e_rezept": true
}
```

### Validation Steps
1. ✅ PZN valid (8 digits)
2. ✅ No SOK required (PZN present)
3. ✅ No Faktorkennzeichen needed (full package)
4. ✅ Preiskennzeichen 11 valid (standard pharmacy purchase price)
5. ✅ VAT rate 2 (19%) matches standard medication
6. ✅ E-Rezept compatible

### Result
**PASS** - Standard prescription, all validations passed.

---

## Example 2: Compounded Preparation (Rezeptur)

### Scenario
Pharmacy prepares a custom medication according to prescription.

### Input Data
```json
{
  "pzn": null,
  "sok": "09999011",
  "faktorkennzeichen": null,
  "preiskennzeichen": "14",
  "vat_rate": 2,
  "dispensing_date": "2026-01-15",
  "e_rezept": true,
  "additional_data": {
    "composition": [
      {"substance": "Salicylic acid", "amount": "5g"},
      {"substance": "Zinc oxide", "amount": "25g"}
    ]
  }
}
```

### Validation Steps
1. ✅ No PZN (compounded preparation)
2. ✅ SOK 09999011 valid (compounded preparations per § 5 Abs. 3 AMPreisV)
3. ✅ SOK not expired
4. ✅ Preiskennzeichen 14 correct (Hilfstaxe for compounded preparations)
5. ✅ VAT rate 2 (19%) matches SOK specification
6. ✅ E-Rezept compatible (SOK.e_rezept = 1)
7. ✅ Additional data present (composition required per Zusatzdaten = 1)

### Result
**PASS** - Compounded preparation correctly specified.

---

## Example 3: Opioid Substitution - Take-Home Prescription

### Scenario
Methadone dispensed for take-home use with individual dosing.

### Input Data
```json
{
  "pzn": null,
  "sok": "09999086",
  "faktorkennzeichen": "55",
  "factor_value": 15,
  "preiskennzeichen": "14",
  "vat_rate": 2,
  "dispensing_date": "2026-01-15",
  "e_rezept": false,
  "additional_data": {
    "dose_mg": 15,
    "doses_count": 7,
    "substance": "Methadone"
  }
}
```

### Validation Steps
1. ✅ No PZN (partial quantities from Anlage 4)
2. ✅ SOK 09999086 valid (Methadone partial quantities Anlage 4)
3. ✅ Faktorkennzeichen 55 correct (single dose in mg for take-home)
4. ✅ Factor value matches dose (15 mg)
5. ✅ Preiskennzeichen 14 correct (Hilfstaxe applies)
6. ✅ VAT rate 2 (19%) matches
7. ✅ E-Rezept = false correct (SOK.e_rezept = 0, paper prescription required)
8. ✅ Additional data present (dose information required per Zusatzdaten = 3)

### Result
**PASS** - Opioid substitution correctly documented.

---

## Example 4: Opioid Substitution - Supervised Administration

### Scenario
Methadone dispensed for supervised administration (Sichtvergabe) in pharmacy.

### Input Data
```json
{
  "pzn": null,
  "sok": "09999086",
  "faktorkennzeichen": "57",
  "factor_value": 80,
  "preiskennzeichen": "14",
  "vat_rate": 2,
  "dispensing_date": "2026-01-15",
  "e_rezept": false,
  "additional_data": {
    "dose_mg": 80,
    "administration": "supervised",
    "substance": "Methadone"
  }
}
```

### Validation Steps
1. ✅ SOK 09999086 valid
2. ✅ Faktorkennzeichen 57 correct (single dose in mg for supervised administration)
3. ✅ Factor value matches dose
4. ✅ Additional data includes supervision confirmation

### Result
**PASS** - Supervised administration correctly specified.

---

## Example 5: Cannabis Flowers - Unchanged State

### Scenario
Medical cannabis flowers dispensed without processing.

### Input Data
```json
{
  "pzn": null,
  "sok": "06460694",
  "faktorkennzeichen": null,
  "preiskennzeichen": "13",
  "vat_rate": 2,
  "dispensing_date": "2026-01-15",
  "e_rezept": true,
  "additional_data": {
    "strain": "Bedrocan",
    "thc_content": "22%",
    "cbd_content": "<1%",
    "amount_g": 15
  }
}
```

### Validation Steps
1. ✅ SOK 06460694 valid (Cannabis flowers unchanged)
2. ✅ Preiskennzeichen 13 appropriate (actual purchase price)
3. ✅ VAT rate 2 (19%) correct
4. ✅ E-Rezept compatible
5. ✅ Additional data present (strain and content info required)

### Result
**PASS** - Cannabis prescription correctly documented.

---

## Example 6: Partial Package (Stückelung)

### Scenario
Prescription drug partially dispensed (e.g., 500 tablets from 1000-tablet package).

### Input Data
```json
{
  "pzn": "01234567",
  "sok": "09999057",
  "faktorkennzeichen": "11",
  "factor_value": 500,
  "preiskennzeichen": "11",
  "vat_rate": 2,
  "dispensing_date": "2026-01-15",
  "e_rezept": false
}
```

### Validation Steps
1. ✅ PZN present (original product)
2. ✅ SOK 09999057 valid (partial quantity of prescription drug)
3. ✅ Faktorkennzeichen 11 required and present (share in promille)
4. ✅ Factor value 500‰ = 50% of package
5. ✅ E-Rezept = false correct (SOK.e_rezept = 0 for partial quantities)

### Result
**PASS** - Partial package correctly specified.

---

## Example 7: BTM Fee (Controlled Substance Fee)

### Scenario
Additional fee for dispensing controlled substance (Betäubungsmittel).

### Input Data
```json
{
  "pzn": null,
  "sok": "02567001",
  "faktorkennzeichen": null,
  "preiskennzeichen": null,
  "vat_rate": 2,
  "dispensing_date": "2026-01-15",
  "e_rezept": false,
  "fee_amount": 2.50
}
```

### Validation Steps
1. ✅ SOK 02567001 valid (BTM fee per Ziffer 4.1)
2. ✅ No PZN (fee code, not a product)
3. ✅ No Faktorkennzeichen (not applicable to fees)
4. ✅ VAT rate 2 (19%) correct
5. ✅ E-Rezept = false correct (SOK.e_rezept = 0)
6. ✅ Additional data present (fee amount required per Zusatzdaten = 4)

### Result
**PASS** - BTM fee correctly recorded.

---

## Example 8: Flu Vaccination Service (GKV)

### Scenario
Pharmacy performs flu vaccination service covered by statutory health insurance.

### Input Data
```json
{
  "pzn": "18774529",
  "sok_service": "17716926",
  "sok_materials": "17716955",
  "sok_procurement": "18774512",
  "vat_rate": 0,
  "dispensing_date": "2024-10-15",
  "e_rezept": true,
  "vaccination_data": {
    "vaccine_pzn": "18774529",
    "vaccine_name": "FLUAD Tetra 2024/2025",
    "batch_number": "ABC123",
    "administration_date": "2024-10-15"
  }
}
```

### Validation Steps
1. ✅ PZN 18774529 valid (flu vaccine product)
2. ✅ SOK 17716926 valid (vaccination service fee)
3. ✅ SOK 17716955 valid (auxiliary services)
4. ✅ SOK 18774512 valid (procurement costs)
5. ✅ VAT rate 0 (tax-free) correct for all service SOKs
6. ✅ E-Rezept compatible
7. ✅ Vaccination data complete

### Result
**PASS** - Vaccination service correctly documented.

**Note:** After April 1, 2025, billing must be done exclusively electronically per Anhang 5 TA1.

---

## Example 9: Hospital Pharmacy - Cytostatic Preparation (Tax-Free)

### Scenario
Hospital pharmacy prepares cytostatic solution for patient (tax-free).

### Input Data
```json
{
  "pzn": null,
  "sok": "06460872",
  "faktorkennzeichen": null,
  "preiskennzeichen": "14",
  "vat_rate": 0,
  "dispensing_date": "2026-01-15",
  "e_rezept": true,
  "pharmacy_type": "hospital",
  "additional_data": {
    "preparation_type": "cytostatic",
    "active_substance": "5-Fluorouracil",
    "dose": "500mg in 100ml NaCl 0.9%"
  }
}
```

### Validation Steps
1. ✅ SOK 06460872 valid (cytostatic, tax-free 0%)
2. ✅ VAT rate 0 matches SOK specification
3. ✅ Pharmacy type = hospital (only hospitals can use 0% VAT codes)
4. ✅ Additional data present
5. ✅ Preiskennzeichen 14 appropriate

### Result
**PASS** - Hospital cytostatic preparation correctly documented.

---

## Example 10: ERROR - Expired SOK Code

### Scenario
Attempting to use an expired special code.

### Input Data
```json
{
  "pzn": null,
  "sok": "17717104",
  "faktorkennzeichen": null,
  "preiskennzeichen": "11",
  "vat_rate": 0,
  "dispensing_date": "2025-01-15",
  "e_rezept": false
}
```

### Validation Steps
1. ✅ SOK 17717104 exists (VAXIGRIP Tetra 2022/2023)
2. ❌ SOK expired: valid until 01.08.2024
3. ❌ Dispensing date 2025-01-15 > expiration date

### Result
**FAIL - SOK_EXPIRED**

**Error Message:**
```
SOK code 17717104 expired on 2024-08-01.
Dispensing date 2025-01-15 is not within validity period.
Use updated seasonal flu vaccine SOK code.
```

---

## Example 11: ERROR - Missing Faktorkennzeichen

### Scenario
Opioid substitution without required dose factor.

### Input Data
```json
{
  "pzn": null,
  "sok": "09999086",
  "faktorkennzeichen": null,
  "preiskennzeichen": "14",
  "vat_rate": 2,
  "dispensing_date": "2026-01-15",
  "e_rezept": false
}
```

### Validation Steps
1. ✅ SOK 09999086 valid (Methadone)
2. ❌ Faktorkennzeichen required but missing
3. ❌ SOK requires Zusatzdaten = 3 (dose information)

### Result
**FAIL - FACTOR_REQUIRED**

**Error Message:**
```
SOK 09999086 (Methadone partial quantities) requires Faktorkennzeichen 55 or 57
to specify dose in milligrams.
Missing required dose information.
```

---

## Example 12: ERROR - VAT Rate Mismatch

### Scenario
Blood product with incorrect VAT rate.

### Input Data
```json
{
  "pzn": null,
  "sok": "02567515",
  "faktorkennzeichen": null,
  "preiskennzeichen": "12",
  "vat_rate": 2,
  "dispensing_date": "2026-01-15",
  "e_rezept": true
}
```

### Validation Steps
1. ✅ SOK 02567515 valid (Granulocytes)
2. ❌ VAT rate mismatch: SOK specifies 0% (tax-free), but input has 2% (19%)

### Result
**FAIL - VAT_MISMATCH**

**Error Message:**
```
VAT rate mismatch for SOK 02567515 (Granulocytes).
Expected: 0% (tax-free blood product)
Provided: 2% (19% standard rate)
Blood products are tax-exempt.
```

---

## Example 13: ERROR - E-Rezept Incompatibility

### Scenario
Attempting to use E-Rezept for a code that requires paper prescription.

### Input Data
```json
{
  "pzn": null,
  "sok": "09999057",
  "faktorkennzeichen": "11",
  "factor_value": 500,
  "preiskennzeichen": "11",
  "vat_rate": 2,
  "dispensing_date": "2026-01-15",
  "e_rezept": true
}
```

### Validation Steps
1. ✅ SOK 09999057 valid (partial quantity)
2. ❌ E-Rezept incompatible: SOK.e_rezept = 0

### Result
**FAIL - EREZEPT_INCOMPATIBLE**

**Error Message:**
```
SOK 09999057 (partial quantity prescription drug) is not compatible with E-Rezept.
E-Rezept field must be false or 0.
Partial quantities require paper prescription documentation.
```

---

## Example 14: WARNING - Unusual Price Code

### Scenario
Compounded preparation with non-standard price code.

### Input Data
```json
{
  "pzn": null,
  "sok": "09999011",
  "faktorkennzeichen": null,
  "preiskennzeichen": "11",
  "vat_rate": 2,
  "dispensing_date": "2026-01-15",
  "e_rezept": true,
  "additional_data": {
    "composition": [{"substance": "Water", "amount": "100ml"}]
  }
}
```

### Validation Steps
1. ✅ SOK 09999011 valid
2. ✅ Additional data present
3. ⚠️ Preiskennzeichen 11 unusual for compounded preparations (typically 14)

### Result
**PASS with WARNING - PRICE_MISMATCH**

**Warning Message:**
```
Compounded preparation (SOK 09999011) typically uses Preiskennzeichen 14 (Hilfstaxe).
Preiskennzeichen 11 (pharmacy purchase price) is unusual.
Please verify pricing method is correct.
```

---

## Example 15: Contract-Specific Code - Authorized

### Scenario
Regional contract code used by authorized pharmacy.

### Input Data
```json
{
  "pzn": null,
  "sok": "06460501",
  "faktorkennzeichen": null,
  "preiskennzeichen": "15",
  "vat_rate": 2,
  "dispensing_date": "2024-06-15",
  "e_rezept": true,
  "pharmacy_association": "LAV Baden-Württemberg"
}
```

### Validation Steps
1. ✅ SOK 06460501 valid (AOK BW contract supplement)
2. ✅ SOK assigned to: LAV Baden-Württemberg
3. ✅ Pharmacy association matches
4. ✅ Dispensing date within validity: 2015-04-01 to 2025-01-01
5. ✅ Preiskennzeichen 15 correct (agreed price per § 129 Abs. 5)

### Result
**PASS** - Authorized use of contract-specific code.

---

## Example 16: ERROR - Unauthorized Contract Code

### Scenario
Regional contract code used by unauthorized pharmacy.

### Input Data
```json
{
  "pzn": null,
  "sok": "06460501",
  "faktorkennzeichen": null,
  "preiskennzeichen": "15",
  "vat_rate": 2,
  "dispensing_date": "2024-06-15",
  "e_rezept": true,
  "pharmacy_association": "LAV Bayern"
}
```

### Validation Steps
1. ✅ SOK 06460501 valid
2. ❌ SOK assigned to: LAV Baden-Württemberg
3. ❌ Pharmacy association: LAV Bayern (mismatch)

### Result
**FAIL - SOK_UNAUTHORIZED**

**Error Message:**
```
SOK 06460501 is a contract-specific code assigned to LAV Baden-Württemberg.
Pharmacy association LAV Bayern is not authorized to use this code.
Check if equivalent contract exists for your region.
```

---

## Validation Workflow Diagram

```
┌─────────────────────────────────────┐
│  Prescription Billing Data Input    │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  1. Format Validation                │
│  - PZN: 8 digits or null             │
│  - SOK: 8 digits if present          │
│  - Faktorkennzeichen: 2 digits       │
│  - Preiskennzeichen: 2 digits        │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  2. Code Existence Check             │
│  - PZN in ABDATA database            │
│  - SOK in SOK1/SOK2 tables           │
│  - Factor in valid list              │
│  - Price in valid list               │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  3. Temporal Validation              │
│  - Check SOK validity dates          │
│  - Verify against dispensing date    │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  4. Authorization Check              │
│  - Contract-specific SOK?            │
│  - Pharmacy authorized?              │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  5. Cross-Reference Validation       │
│  - SOK + Factor compatibility        │
│  - SOK + Price compatibility         │
│  - VAT rate consistency              │
│  - E-Rezept compatibility            │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  6. Additional Data Check            │
│  - Required per Zusatzdaten field    │
│  - Completeness validation           │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  7. Business Logic Validation        │
│  - Domain-specific rules             │
│  - Warning conditions                │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Result: PASS / FAIL / PASS+WARNING │
└─────────────────────────────────────┘
```

---

## Best Practices

### 1. Always Validate Sequentially
Follow the validation order to catch format errors before business logic errors.

### 2. Provide Context in Error Messages
Include:
- What was expected
- What was provided
- Why it's incorrect
- How to fix it

### 3. Use Appropriate Severity Levels
- **ERROR**: Data cannot be processed, will be rejected
- **WARNING**: Data can be processed but may be incorrect
- **INFO**: Informational message, no action needed

### 4. Keep Reference Data Updated
- Update SOK tables monthly (or as published)
- Maintain historical records for retroactive validation
- Track validity date changes

### 5. Log All Validation Results
Maintain audit trail:
- Validation timestamp
- Input data
- Validation steps performed
- Results and error codes
- User/system performing validation

---

**Document Version:** 1.0
**Last Updated:** 2026-01-24
**Companion Document:** [CODE_STRUCTURES.md](./CODE_STRUCTURES.md)
