# Abrechnungs‑Validierungsbeispiele und Workflows

## Überblick

Dieses Dokument bietet praxisnahe Beispiele für Abrechnungsszenarien und deren Validierungsabläufe. Jedes Beispiel enthält die Verordnungsdaten, die Validierungsschritte und die erwarteten Ergebnisse.

---

## Beispiel 1: Standard‑Arzneimittel mit PZN

### Szenario
Reguläres verordnetes Arzneimittel mit Standardpreis.

### Eingabedaten
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

### Validierungsschritte
1. ✅ PZN gültig (8 Stellen)
2. ✅ Kein SOK erforderlich (PZN vorhanden)
3. ✅ Kein Faktorkennzeichen nötig (volle Packung)
4. ✅ Preiskennzeichen 11 gültig (standardmäßiger Apothekeneinkaufspreis)
5. ✅ USt‑Satz 2 (19%) passend für Standard‑Arzneimittel
6. ✅ E‑Rezept kompatibel

### Ergebnis
**PASS** – Standardverordnung, alle Prüfungen bestanden.

---

## Beispiel 2: Rezeptur

### Szenario
Apotheke stellt ein individuelles Arzneimittel nach Verordnung her.

### Eingabedaten
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

### Validierungsschritte
1. ✅ Keine PZN (Rezeptur)
2. ✅ SOK 09999011 gültig (Rezepturen nach § 5 Abs. 3 AMPreisV)
3. ✅ SOK nicht abgelaufen
4. ✅ Preiskennzeichen 14 korrekt (Hilfstaxe für Rezepturen)
5. ✅ USt‑Satz 2 (19%) entspricht SOK‑Spezifikation
6. ✅ E‑Rezept kompatibel (SOK.e_rezept = 1)
7. ✅ Zusatzdaten vorhanden (Zusammensetzung erforderlich gemäß Zusatzdaten = 1)

### Ergebnis
**PASS** – Rezeptur korrekt angegeben.

---

## Beispiel 3: Opioid‑Substitution – Take‑Home‑Verordnung

### Szenario
Methadon wird zur Mitgabe mit Einzeldosierung abgegeben.

### Eingabedaten
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

### Validierungsschritte
1. ✅ Keine PZN (Teilmengen nach Anlage 4)
2. ✅ SOK 09999086 gültig (Methadon‑Teilmengen Anlage 4)
3. ✅ Faktorkennzeichen 55 korrekt (Einzeldosis in mg für Take‑Home)
4. ✅ Faktorwert entspricht Dosis (15 mg)
5. ✅ Preiskennzeichen 14 korrekt (Hilfstaxe)
6. ✅ USt‑Satz 2 (19%) korrekt
7. ✅ E‑Rezept = false korrekt (SOK.e_rezept = 0, Papierverordnung erforderlich)
8. ✅ Zusatzdaten vorhanden (Dosisangaben erforderlich gemäß Zusatzdaten = 3)

### Ergebnis
**PASS** – Opioid‑Substitution korrekt dokumentiert.

---

## Beispiel 4: Opioid‑Substitution – Beaufsichtigte Abgabe

### Szenario
Methadon wird zur beaufsichtigten Abgabe (Sichtvergabe) in der Apotheke ausgegeben.

### Eingabedaten
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

### Validierungsschritte
1. ✅ SOK 09999086 gültig
2. ✅ Faktorkennzeichen 57 korrekt (Einzeldosis in mg für beaufsichtigte Abgabe)
3. ✅ Faktorwert entspricht Dosis
4. ✅ Zusatzdaten enthalten Bestätigung der Beaufsichtigung

### Ergebnis
**PASS** – Beaufsichtigte Abgabe korrekt angegeben.

---

## Beispiel 5: Cannabisblüten – Unverändert

### Szenario
Medizinische Cannabisblüten werden unverarbeitet abgegeben.

### Eingabedaten
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

### Validierungsschritte
1. ✅ SOK 06460694 gültig (Cannabisblüten unverändert)
2. ✅ Preiskennzeichen 13 passend (tatsächlicher Einkaufspreis)
3. ✅ USt‑Satz 2 (19%) korrekt
4. ✅ E‑Rezept kompatibel
5. ✅ Zusatzdaten vorhanden (Sorte und Gehaltsangaben erforderlich)

### Ergebnis
**PASS** – Cannabisverordnung korrekt dokumentiert.

---

## Beispiel 6: Teilpackung (Stückelung)

### Szenario
Arzneimittel wird teilweise abgegeben (z. B. 500 Tabletten aus 1000er‑Packung).

### Eingabedaten
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

### Validierungsschritte
1. ✅ PZN vorhanden (Originalprodukt)
2. ✅ SOK 09999057 gültig (Teilmenge verschreibungspflichtiger Arzneimittel)
3. ✅ Faktorkennzeichen 11 erforderlich und vorhanden (Promilleanteil)
4. ✅ Faktorwert 500‰ = 50% der Packung
5. ✅ E‑Rezept = false korrekt (SOK.e_rezept = 0 für Teilmengen)

### Ergebnis
**PASS** – Teilpackung korrekt angegeben.

---

## Beispiel 7: BTM‑Gebühr

### Szenario
Zusätzliche Gebühr für die Abgabe eines Betäubungsmittels.

### Eingabedaten
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

### Validierungsschritte
1. ✅ SOK 02567001 gültig (BTM‑Gebühr nach Ziffer 4.1)
2. ✅ Keine PZN (Gebühr‑Code, kein Produkt)
3. ✅ Kein Faktorkennzeichen (für Gebühren nicht anwendbar)
4. ✅ USt‑Satz 2 (19%) korrekt
5. ✅ E‑Rezept = false korrekt (SOK.e_rezept = 0)
6. ✅ Zusatzdaten vorhanden (Gebührenbetrag erforderlich gemäß Zusatzdaten = 4)

### Ergebnis
**PASS** – BTM‑Gebühr korrekt erfasst.

---

## Beispiel 8: Grippeschutz‑Impfleistung (GKV)

### Szenario
Apotheke führt eine Grippeschutz‑Impfleistung durch, gedeckt durch die GKV.

### Eingabedaten
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

### Validierungsschritte
1. ✅ PZN 18774529 gültig (Grippeimpfstoff)
2. ✅ SOK 17716926 gültig (Impfleistungsgebühr)
3. ✅ SOK 17716955 gültig (Impffremdleistungen)
4. ✅ SOK 18774512 gültig (Beschaffungskosten)
5. ✅ USt‑Satz 0 (steuerfrei) korrekt für alle Service‑SOK
6. ✅ E‑Rezept kompatibel
7. ✅ Impfdaten vollständig

### Ergebnis
**PASS** – Impfleistung korrekt dokumentiert.

**Hinweis:** Ab dem 01.04.2025 muss die Abrechnung ausschließlich elektronisch erfolgen (Anhang 5 TA1).

---

## Beispiel 9: Krankenhausapotheke – Zytostatika‑Zubereitung (steuerfrei)

### Szenario
Krankenhausapotheke stellt eine zytostatische Lösung für einen Patienten her (steuerfrei).

### Eingabedaten
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

### Validierungsschritte
1. ✅ SOK 06460872 gültig (Zytostatika, 0% USt.)
2. ✅ USt‑Satz 0 entspricht SOK‑Spezifikation
3. ✅ Apothekentyp = Krankenhaus (0%‑USt‑Codes nur für Krankenhausapotheken)
4. ✅ Zusatzdaten vorhanden
5. ✅ Preiskennzeichen 14 passend

### Ergebnis
**PASS** – Krankenhaus‑Zytostatika korrekt dokumentiert.

---

## Beispiel 10: ERROR – Abgelaufenes SOK

### Szenario
Verwendung eines abgelaufenen Sonderkennzeichens.

### Eingabedaten
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

### Validierungsschritte
1. ✅ SOK 17717104 existiert (VAXIGRIP Tetra 2022/2023)
2. ❌ SOK abgelaufen: gültig bis 01.08.2024
3. ❌ Abgabedatum 2025-01-15 > Ablaufdatum

### Ergebnis
**FAIL – SOK_EXPIRED**

**Fehlermeldung:**
```
SOK 17717104 ist am 2024-08-01 abgelaufen.
Abgabedatum 2025-01-15 liegt außerhalb der Gültigkeit.
Bitte aktuelles saisonales Grippe‑SOK verwenden.
```

---

## Beispiel 11: ERROR – Fehlendes Faktorkennzeichen

### Szenario
Opioid‑Substitution ohne erforderlichen Dosis‑Faktor.

### Eingabedaten
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

### Validierungsschritte
1. ✅ SOK 09999086 gültig (Methadon)
2. ❌ Faktorkennzeichen erforderlich, fehlt aber
3. ❌ SOK erfordert Zusatzdaten = 3 (Dosisangaben)

### Ergebnis
**FAIL – FACTOR_REQUIRED**

**Fehlermeldung:**
```
SOK 09999086 (Methadon‑Teilmengen) erfordert Faktorkennzeichen 55 oder 57,
umb die Dosis in Milligramm anzugeben.
Erforderliche Dosisangaben fehlen.
```

---

## Beispiel 12: ERROR – USt‑Satz stimmt nicht

### Szenario
Blutprodukt mit falschem USt‑Satz.

### Eingabedaten
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

### Validierungsschritte
1. ✅ SOK 02567515 gültig (Granulozyten)
2. ❌ USt‑Satz‑Abweichung: SOK verlangt 0% (steuerfrei), Eingabe enthält 2% (19%)

### Ergebnis
**FAIL – VAT_MISMATCH**

**Fehlermeldung:**
```
USt‑Satz stimmt nicht für SOK 02567515 (Granulozyten).
Erwartet: 0% (steuerfreies Blutprodukt)
Angegeben: 2% (19% Standard)
Blutprodukte sind steuerfrei.
```

---

## Beispiel 13: ERROR – E‑Rezept‑Inkompatibilität

### Szenario
E‑Rezept wird für einen Code verwendet, der Papierverordnung erfordert.

### Eingabedaten
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

### Validierungsschritte
1. ✅ SOK 09999057 gültig (Teilmenge)
2. ❌ E‑Rezept nicht kompatibel: SOK.e_rezept = 0

### Ergebnis
**FAIL – EREZEPT_INCOMPATIBLE**

**Fehlermeldung:**
```
SOK 09999057 (Teilmengen‑Arzneimittel) ist nicht E‑Rezept‑kompatibel.
E‑Rezept‑Feld muss false oder 0 sein.
Teilmengen erfordern Papierverordnungs‑Dokumentation.
```

---

## Beispiel 14: WARNING – Ungewöhnliches Preiskennzeichen

### Szenario
Rezeptur mit unüblichem Preiskennzeichen.

### Eingabedaten
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

### Validierungsschritte
1. ✅ SOK 09999011 gültig
2. ✅ Zusatzdaten vorhanden
3. ⚠️ Preiskennzeichen 11 unüblich für Rezepturen (typisch 14)

### Ergebnis
**PASS mit WARNING – PRICE_MISMATCH**

**Warnmeldung:**
```
Rezeptur (SOK 09999011) verwendet typischerweise Preiskennzeichen 14 (Hilfstaxe).
Preiskennzeichen 11 (Apothekeneinkaufspreis) ist ungewöhnlich.
Bitte prüfen, ob die Preisermittlung korrekt ist.
```

---

## Beispiel 15: Vertragsbezogener Code – Autorisiert

### Szenario
Regionaler Vertragscode wird von berechtigter Apotheke verwendet.

### Eingabedaten
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

### Validierungsschritte
1. ✅ SOK 06460501 gültig (AOK BW‑Vertragszuschlag)
2. ✅ SOK zugeordnet: LAV Baden‑Württemberg
3. ✅ Apothekenverband stimmt überein
4. ✅ Abgabedatum innerhalb der Gültigkeit: 2015-04-01 bis 2025-01-01
5. ✅ Preiskennzeichen 15 korrekt (vereinbarter Preis nach § 129 Abs. 5)

### Ergebnis
**PASS** – Vertragsbezogener Code korrekt verwendet.

---

## Beispiel 16: ERROR – Unberechtigter Vertragscode

### Szenario
Regionaler Vertragscode wird von nicht berechtigter Apotheke verwendet.

### Eingabedaten
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

### Validierungsschritte
1. ✅ SOK 06460501 gültig
2. ❌ SOK zugeordnet: LAV Baden‑Württemberg
3. ❌ Apothekenverband: LAV Bayern (Abweichung)

### Ergebnis
**FAIL – SOK_UNAUTHORIZED**

**Fehlermeldung:**
```
SOK 06460501 ist ein vertragsbezogener Code für LAV Baden‑Württemberg.
Apothekenverband LAV Bayern ist nicht berechtigt, diesen Code zu verwenden.
Prüfen Sie, ob ein entsprechender Vertrag für Ihre Region existiert.
```

---

## Validierungs‑Workflow‑Diagramm

```
┌─────────────────────────────────────┐
│  Eingabe Abrechnungsdaten           │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  1. Formatvalidierung               │
│  - PZN: 8 Stellen oder null         │
│  - SOK: 8 Stellen, falls vorhanden  │
│  - Faktorkennzeichen: 2 Stellen     │
│  - Preiskennzeichen: 2 Stellen      │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  2. Code‑Existenzprüfung             │
│  - PZN in ABDATA‑Datenbank          │
│  - SOK in SOK1/SOK2‑Tabellen        │
│  - Faktor in gültiger Liste         │
│  - Preis in gültiger Liste          │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  3. Zeitliche Validierung            │
│  - SOK‑Gültigkeit prüfen            │
│  - Gegen Abgabedatum prüfen         │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  4. Berechtigungsprüfung             │
│  - Vertrags‑SOK?                    │
│  - Apotheke berechtigt?             │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  5. Querverweis‑Validierung          │
│  - SOK + Faktor‑Kompatibilität      │
│  - SOK + Preis‑Kompatibilität       │
│  - USt‑Satz‑Konsistenz              │
│  - E‑Rezept‑Kompatibilität          │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  6. Zusatzdaten‑Prüfung             │
│  - Erforderlich gemäß Zusatzdaten  │
│  - Vollständigkeit prüfen           │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  7. Geschäftslogik‑Validierung       │
│  - Fachliche Regeln                 │
│  - Warnbedingungen                  │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Ergebnis: PASS / FAIL / PASS+WARN  │
└─────────────────────────────────────┘
```

---

## Best Practices

### 1. Immer sequentiell validieren
Befolgen Sie die Validierungsreihenfolge, um Formatfehler vor fachlichen Fehlern zu erkennen.

### 2. Kontext in Fehlermeldungen angeben
Enthalten sein sollten:
- Was erwartet wurde
- Was angegeben wurde
- Warum es falsch ist
- Wie es zu korrigieren ist

### 3. Passende Schweregrade verwenden
- **ERROR**: Daten können nicht verarbeitet werden, werden abgelehnt
- **WARNING**: Daten können verarbeitet werden, sind aber möglicherweise falsch
- **INFO**: Hinweis, keine Aktion erforderlich

### 4. Referenzdaten aktuell halten
- SOK‑Tabellen monatlich aktualisieren (oder nach Veröffentlichung)
- Historische Daten für rückwirkende Validierung vorhalten
- Änderungen an Gültigkeitsdaten nachverfolgen

### 5. Alle Validierungsergebnisse protokollieren
Audit‑Trail pflegen:
- Validierungszeitstempel
- Eingabedaten
- Durchgeführte Validierungsschritte
- Ergebnisse und Fehlercodes
- Nutzer/System, das validiert hat

---

**Dokumentversion:** 1.0
**Letzte Aktualisierung:** 2026-01-24
**Begleitdokument:** [Kodestrukturen.md](./Kodestrukturen.md)
