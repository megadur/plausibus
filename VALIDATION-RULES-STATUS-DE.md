# E-Rezept Validator - Status der Validierungsregeln

**Letzte Aktualisierung:** 01.02.2026
**Gesamtzahl Regeln in TA1-Spezifikation:** 67 Regeln
**Implementiert:** 18 Regeln (27%)
**In Bearbeitung:** 0 Regeln
**Ausstehend:** 49 Regeln (73%)

---

## Zusammenfassung nach Kategorie

| Kategorie | Gesamt | Implementiert | Ausstehend | Status |
|----------|--------|---------------|------------|--------|
| **Format (FMT)** | 10 | 10 | 0 | ✅ Vollständig |
| **Allgemein (GEN)** | 8 | 8 | 0 | ✅ Vollständig |
| **Berechnung (CALC)** | 7 | 7 | 0 | ✅ Vollständig |
| **BTM** | 4 | 1 | 3 | 🟡 25% |
| **Cannabis (CAN)** | 5 | 0 | 5 | ⭕ 0% |
| **Rezeptur (REZ)** | 21 | 0 | 21 | ⭕ 0% |
| **Gebühren (FEE)** | 3 | 0 | 3 | ⭕ 0% |
| **Sonderfälle (SPC)** | 8 | 0 | 8 | ⭕ 0% |
| **Wirtschaftl. Einzelmenge (ESQ)** | 4 | 0 | 4 | ⭕ 0% |

---

## Detaillierter Status

### ✅ Formatvalidierung (FMT) - 10/10 Vollständig

**Validator:** `FhirFormatValidator.cs`, `PznFormatValidator.cs`

| Regel | Beschreibung | Status | Implementierung |
|-------|--------------|--------|-----------------|
| FMT-001 | PZN-Formatvalidierung (8 Ziffern) | ✅ Vollständig | `PznFormatValidator` |
| FMT-002 | PZN-Prüfziffernvalidierung (Modulo 11) | ✅ Vollständig | `PznFormatValidator` |
| FMT-003 | ISO 8601 DateTime-Format | ✅ Vollständig | `FhirFormatValidator` |
| FMT-004 | Herstellerkennzeichenformat | ✅ Vollständig | `FhirFormatValidator` |
| FMT-005 | Zählerfeld-Formate | ✅ Vollständig | `FhirFormatValidator` |
| FMT-006 | Chargenbezeichnungsformat | ✅ Vollständig | `FhirFormatValidator` |
| FMT-007 | Faktorkennzeichenformat | ✅ Vollständig | `FhirFormatValidator` |
| FMT-008 | Faktorwertformat | ✅ Vollständig | `FhirFormatValidator` |
| FMT-009 | Preiskennzeichenformat | ✅ Vollständig | `FhirFormatValidator` |
| FMT-010 | Preiswertformat | ✅ Vollständig | `FhirFormatValidator` |

---

### ✅ Allgemeine Regeln (GEN) - 8/8 Vollständig

**Validator:** `FhirAbgabedatenValidator.cs`

| Regel | Beschreibung | Status | Implementierung |
|-------|--------------|--------|-----------------|
| GEN-001 | Deutsche Zeitzone (MEZ/MESZ) | ✅ Vollständig | `FhirAbgabedatenValidator` |
| GEN-002 | Gültigkeitsdatum für Feldänderungen | ✅ Vollständig | `FhirAbgabedatenValidator` |
| GEN-003 | Bruttopreis-Zusammensetzung | ✅ Vollständig | `FhirAbgabedatenValidator` |
| GEN-004 | MwSt-Berechnung für gesetzliche Gebühren | ✅ Vollständig | `FhirAbgabedatenValidator` |
| GEN-005 | Sonderkennzeichen-Übermittlung | ✅ Vollständig | `FhirAbgabedatenValidator` |
| GEN-006 | SOK-Gültigkeitszeitraum-Prüfung | ✅ Vollständig | `FhirAbgabedatenValidator` |
| GEN-007 | E-Rezept SOK-Kompatibilität | ✅ Vollständig | `FhirAbgabedatenValidator` |
| GEN-008 | MwSt-Satz-Konsistenz | ✅ Vollständig | `FhirAbgabedatenValidator` |

**Hinweise:**
- Verwendet TA1-Referenzdatenbank für SOK-Code-Validierung
- Temporale Validierung mit Abgabedatum
- E-Rezept-Kompatibilitätsprüfung

---

### ✅ Berechnungsregeln (CALC) - 7/7 Vollständig

**Validator:** `CalculationValidator.cs`

| Regel | Beschreibung | Status | Implementierung |
|-------|--------------|--------|-----------------|
| CALC-001 | Standard-Promilleanteil-Formel | ✅ Vollständig | `CalculationValidator` |
| CALC-002 | Sonderkennzeichen-Faktor-Ausnahme | ✅ Vollständig | `CalculationValidator` |
| CALC-003 | Künstliche Befruchtung Sonderkennzeichen | ✅ Vollständig | `CalculationValidator` |
| CALC-004 | Grundlegende Preisberechnung | ✅ Vollständig | `CalculationValidator` ⭐ NEU |
| CALC-005 | MwSt-Ausschluss im Preisfeld | ✅ Vollständig | `CalculationValidator` ⭐ NEU |
| CALC-006 | Preiskennzeichen-Nachschlagen | ✅ Vollständig | `PriceIdentifier` Value Object |
| CALC-007 | Flexible nachgestellte Nullen | ✅ Vollständig | `PromilleFactor` Value Object |

**Funktionen:**
- Value Object Pattern (Money, PromilleFactor, Pzn, SokCode, PriceIdentifier)
- ABDATA-Integration für Preisberechnungen
- Toleranzbasierter Dezimalvergleich (0,000001 für Faktoren, 0,01 EUR für Preise)
- Formel: `Preis = (Faktor / 1000) × Basispreis`

---

### 🟡 BTM-Validierung - 1/4 (25%)

**Validator:** `BtmDetectionValidator.cs` (nur grundlegende Erkennung)

| Regel | Beschreibung | Status | Priorität |
|-------|--------------|--------|-----------|
| BTM-001 | E-BTM Gebühren-Sonderkennzeichen | ⭕ Ausstehend | Hoch |
| BTM-002 | Alle Arzneimittel müssen aufgeführt sein | ⭕ Ausstehend | Hoch |
| BTM-003 | BTM Sieben-Tage-Gültigkeitsregel | ⭕ Ausstehend | Hoch |
| BTM-004 | BTM Diagnose-Anforderung | ⭕ Ausstehend | Mittel |

**Aktuelle Implementierung:**
- ✅ BTM-Erkennung über ABDATA (Btm-Flag = 2)
- ✅ Grundlegende Klassifizierung (BTM, Ausnahme, T-Rezept)
- ⭕ Geschäftslogik-Validierung ausstehend

**Nächste Schritte:**
- Implementierung BTM-001: E-BTM Gebühr validieren (SOK-Code-Validierung)
- Implementierung BTM-002: Sicherstellen, dass alle Artikel PZN/SOK-Codes haben
- Implementierung BTM-003: Verordnungsdatum ≤ 7 Tage alt prüfen
- Implementierung BTM-004: Vorhandensein des Diagnosecodes validieren

---

### ⭕ Cannabis-Validierung (CAN) - 0/5 (0%)

**Validator:** Noch nicht implementiert

| Regel | Beschreibung | Status | Priorität |
|-------|--------------|--------|-----------|
| CAN-001 | Cannabis-Sonderkennzeichen | ⭕ Ausstehend | Hoch |
| CAN-002 | Keine BTM/T-Rezept-Substanzen | ⭕ Ausstehend | Hoch |
| CAN-003 | Faktorfeld-Wert | ⭕ Ausstehend | Hoch |
| CAN-004 | Bruttopreis-Berechnung | ⭕ Ausstehend | Mittel |
| CAN-005 | Herstellungsdaten erforderlich | ⭕ Ausstehend | Mittel |

**Anforderungen:**
- Cannabis-Erkennung über ABDATA (Cannabis-Flag = 2 oder 3)
- Sonderkennzeichen-Validierung (SOK-Codes für Cannabis)
- Herstellungsdaten-Extraktion aus FHIR
- Cannabis-spezifische Preisberechnung

**Verfügbare Daten:**
- ✅ Cannabis-Flag in ABDATA PAC_APO-Tabelle
- ✅ Cannabis-Erkennung in `PacApoArticle.IsCannabis`

---

### ⭕ Rezeptur (REZ) - 0/21 (0%)

**Validator:** Noch nicht implementiert

| Regel | Beschreibung | Status | Priorität |
|-------|--------------|--------|-----------|
| REZ-001 | Identifizierung von Rezepturarzneimitteln | ⭕ Ausstehend | Hoch |
| REZ-002 | Parenteral - Herstellerkennzeichen | ⭕ Ausstehend | Mittel |
| REZ-003 | Parenteral - Zeitstempel-Validierung | ⭕ Ausstehend | Mittel |
| REZ-004 | Parenteral - Zählersequenz | ⭕ Ausstehend | Niedrig |
| REZ-005 | Parenteral - Faktor als Promilleanteil | ⭕ Ausstehend | Mittel |
| REZ-006 | Parenteral - Wochenvorrat-Limit | ⭕ Ausstehend | Mittel |
| REZ-007 | ESQ - Herstellerkennzeichen-Typ | ⭕ Ausstehend | Niedrig |
| REZ-008 | ESQ - Zeitstempel-Validierung | ⭕ Ausstehend | Niedrig |
| REZ-009 | ESQ - Zähler für 02567053 | ⭕ Ausstehend | Niedrig |
| REZ-010 | ESQ - Zähler für 02566993 | ⭕ Ausstehend | Niedrig |
| REZ-011 | ESQ - Faktorkennzeichen | ⭕ Ausstehend | Niedrig |
| REZ-012 | ESQ - Teilmengen-Faktor | ⭕ Ausstehend | Mittel |
| REZ-013 | Cannabis/Rezeptur - Sonderkennzeichen | ⭕ Ausstehend | Hoch |
| REZ-014 | Cannabis/Rezeptur - Herstellerkennzeichen | ⭕ Ausstehend | Mittel |
| REZ-015 | Cannabis/Rezeptur - Herstellungszeitstempel | ⭕ Ausstehend | Mittel |
| REZ-016 | Cannabis/Rezeptur - Zählerwerte | ⭕ Ausstehend | Niedrig |
| REZ-017 | Cannabis/Rezeptur - Faktorkennzeichen | ⭕ Ausstehend | Mittel |
| REZ-018 | Cannabis/Rezeptur - Faktor als Promilleanteil | ⭕ Ausstehend | Hoch |
| REZ-019 | Cannabis/Rezeptur - Preiskennzeichen | ⭕ Ausstehend | Hoch |
| REZ-020 | Cannabis/Rezeptur - Preisanpassung große Mengen | ⭕ Ausstehend | Mittel |
| REZ-021 | Validierung zusätzlicher Datenanforderungen | ⭕ Ausstehend | Hoch |

**Teilimplementierung:**
- ✅ CALC-005: Grundlegende MwSt-Ausschluss-Prüfung für Rezeptur
- ✅ `SokCode.IsCompounding` Eigenschaft (SOK 06460702, 09999011)

**Nächste Schritte:**
- Erstellen von `CompoundingValidator.cs`
- Implementierung REZ-001, REZ-013, REZ-018, REZ-019, REZ-021 (hohe Priorität)
- Vollständige Rezeptur-Preisberechnungen

---

### ⭕ Gebührenvalidierung (FEE) - 0/3 (0%)

**Validator:** Noch nicht implementiert

| Regel | Beschreibung | Status | Priorität |
|-------|--------------|--------|-----------|
| FEE-001 | Botendienstgebühr-Validierung | ⭕ Ausstehend | Mittel |
| FEE-002 | Noctu (Nachtdienstgebühr) | ⭕ Ausstehend | Mittel |
| FEE-003 | Wiederbeschaffungsgebühr | ⭕ Ausstehend | Niedrig |

**Anforderungen:**
- Gebührenerkennung über SOK-Codes
- Validierung gesetzlicher Gebührenbeträge
- MwSt-Anpassungsberechnungen
- Zeitbasierte Validierung (Noctu: 20:00-06:00 Uhr)

---

### ⭕ Sonderfälle (SPC) - 0/8 (0%)

**Validator:** Noch nicht implementiert

| Regel | Beschreibung | Status | Priorität |
|-------|--------------|--------|-----------|
| SPC-001 | Behandlung von Niedrigpreis-Arzneimitteln | ⭕ Ausstehend | Mittel |
| SPC-002 | Mehrkosten für § 3 Abs. 4 | ⭕ Ausstehend | Mittel |
| SPC-003 | Künstliche Befruchtung Kennzeichen | ✅ Teilweise | Hoch |
| SPC-004 | 50% Patientenbeteiligung | ⭕ Ausstehend | Mittel |
| SPC-005 | Künstliche Befruchtung - Rezeptur | ⭕ Ausstehend | Hoch |
| SPC-006 | Abweichungs-Sonderkennzeichen | ⭕ Ausstehend | Niedrig |
| SPC-007 | IK-Format für E-Rezept | ⭕ Ausstehend | Mittel |
| SPC-008 | Vertragsabhängige SOK-Autorisierung | ⭕ Ausstehend | Niedrig |

**Teilimplementierung:**
- ✅ SPC-003: Künstliche Befruchtung Marker (SOK 09999643) in CALC-003 validiert

---

### ⭕ Wirtschaftliche Einzelmenge (ESQ) - 0/4 (0%)

**Validator:** Noch nicht implementiert

| Regel | Beschreibung | Status | Priorität |
|-------|--------------|--------|-----------|
| ESQ-001 | Individuelle Abgabe - Sonderkennzeichen | ⭕ Ausstehend | Niedrig |
| ESQ-002 | Individuelle Abgabe - Einzelne Einheit | ⭕ Ausstehend | Niedrig |
| ESQ-003 | Patientenindividuelle Teilmengen | ⭕ Ausstehend | Niedrig |
| ESQ-004 | Wochenblister - Mehrere Einheiten | ⭕ Ausstehend | Niedrig |

**Anforderungen:**
- ESQ-spezifische SOK-Codes
- Einheitsmengen-Validierung
- Herstellerdaten-Validierung

---

## Implementierungs-Roadmap

### Phase 1: Kern-Validierung ✅ ABGESCHLOSSEN
- [x] Formatvalidierung (FMT-001 bis FMT-010)
- [x] Allgemeine Regeln (GEN-001 bis GEN-008)
- [x] Berechnungsregeln (CALC-001 bis CALC-007)
- [x] ABDATA-Integration
- [x] TA1-Referenzdatenbank
- [x] Value Objects (Money, PromilleFactor, Pzn, SokCode, PriceIdentifier)

### Phase 2: BTM-Validierung 🔄 ALS NÄCHSTES
**Priorität:** Hoch
**Geschätzter Aufwand:** 2-3 Tage

- [ ] BTM-001: E-BTM Gebühren-Sonderkennzeichen
- [ ] BTM-002: Alle Arzneimittel müssen aufgeführt sein
- [ ] BTM-003: Sieben-Tage-Gültigkeitsregel
- [ ] BTM-004: Diagnose-Anforderung

**Voraussetzungen:**
- ✅ ABDATA BTM-Erkennung verfügbar
- ✅ Datumsverarbeitungs-Infrastruktur
- ⭕ Diagnosecode-Extraktion aus FHIR

### Phase 3: Cannabis-Validierung 📅 GEPLANT
**Priorität:** Hoch
**Geschätzter Aufwand:** 2-3 Tage

- [ ] CAN-001: Cannabis-Sonderkennzeichen
- [ ] CAN-002: Keine BTM/T-Rezept-Substanzen
- [ ] CAN-003: Faktorfeld-Wert
- [ ] CAN-004: Bruttopreis-Berechnung
- [ ] CAN-005: Herstellungsdaten erforderlich

**Voraussetzungen:**
- ✅ ABDATA Cannabis-Erkennung verfügbar
- ⭕ Cannabis-spezifische SOK-Codes in Datenbank
- ⭕ Herstellungsdaten-Extraktion

### Phase 4: Rezeptur-Validierung 📅 GEPLANT
**Priorität:** Mittel-Hoch
**Geschätzter Aufwand:** 5-7 Tage

**Hohe Priorität (REZ-001, 013, 018, 019, 021):**
- [ ] REZ-001: Identifizierung von Rezepturarzneimitteln
- [ ] REZ-013: Sonderkennzeichen
- [ ] REZ-018: Faktor als Promilleanteil
- [ ] REZ-019: Preiskennzeichen
- [ ] REZ-021: Zusätzliche Datenvalidierung

**Mittlere Priorität (Parenteral, ESQ):**
- [ ] REZ-002 bis REZ-006: Parenterale Zubereitungsregeln
- [ ] REZ-007 bis REZ-012: Wirtschaftliche Einzelmengen-Regeln
- [ ] REZ-014 bis REZ-017: Cannabis/Rezeptur-Regeln
- [ ] REZ-020: Preisanpassung für große Mengen

### Phase 5: Gebühren & Sonderfälle 📅 GEPLANT
**Priorität:** Mittel
**Geschätzter Aufwand:** 2-3 Tage

- [ ] FEE-001 bis FEE-003: Gebührenvalidierung
- [ ] SPC-001 bis SPC-008: Sonderfall-Behandlung
- [ ] ESQ-001 bis ESQ-004: Wirtschaftliche Einzelmenge

### Phase 6: Integration & Testen 📅 GEPLANT
**Priorität:** Hoch
**Geschätzter Aufwand:** 3-5 Tage

- [ ] Integrationstests mit allen Beispiel-Bundles
- [ ] End-to-End Validierungsszenarien
- [ ] Performance-Optimierung (<500ms Ziel)
- [ ] Verfeinerung der Fehlermeldungen
- [ ] Dokumentationsaktualisierungen

---

## Testdaten-Abdeckung

### Verfügbare Test-Bundles
**Speicherort:** `docs/eRezept-Beispiele/`

| Testfall | Getestete Regeln | Status |
|----------|------------------|--------|
| PZN-Verordnung_Nr_1 | FMT, GEN, CALC-001, CALC-004 | ✅ Verfügbar |
| PZN-Verordnung_Künstliche_Befruchtung | CALC-003, SPC-003 | ✅ Verfügbar |
| Rezeptur-Verordnung_Nr_1 | REZ-xxx, CALC-005 | ✅ Verfügbar |
| Rezeptur-parenterale_Zytostatika | REZ-002 bis REZ-006 | ✅ Verfügbar |
| PZN-Verordnung_Noctu | FEE-002 | ✅ Verfügbar |
| Wirkstoff-Verordnung | Alle Kategorien | ✅ Verfügbar |

**Gesamt Test-Bundles:** 20+ Beispiele mit verschiedenen Szenarien

---

## Technische Schulden & Zukünftige Erweiterungen

### Bekannte Einschränkungen

1. **CALC-005:** Nur grundlegende Implementierung
   - Aktuell: Prüft Preiskennzeichen-Steuerstatus
   - Zukünftig: Vollständige MwSt-Berechnungsvalidierung mit REZ-Regeln

2. **BTM-Erkennung:** Nur Klassifizierung
   - Aktuell: Erkennt BTM über ABDATA-Flag
   - Zukünftig: Geschäftslogik-Validierung (BTM-001 bis BTM-004)

3. **PznTestController:** Entwicklungs-Endpoint
   - Sollte für Produktion entfernt oder gesichert werden

### Zukünftige Erweiterungen

1. **Performance-Optimierung**
   - Batch-PZN-Abfragen
   - Parallele Validator-Ausführung
   - Erweiterte Caching-Strategien

2. **Fehlermeldungen**
   - Standardisierte Fehlercodes gemäß TA1 Abschnitt 12.2
   - Korrekturvorschläge
   - Mehrsprachige Unterstützung

3. **Berichtswesen**
   - Validierungsstatistiken
   - Regelabdeckungsberichte
   - Performance-Metriken

4. **Integration**
   - gematik TI Integration (6-12 Monate)
   - Lauer-Taxe API (alternative Preisquelle)
   - Echtzeit-ABDATA-Updates

---

## Referenzen

- **TA1 Version 039:** Technische Spezifikation für E-Rezept-Abrechnung
- **TA3 Tabellen:** 8.2.25 (Faktoren), 8.2.26 (Preise)
- **ABDATA:** Pharmazeutische Referenzdatenbank
- **Spezifikation:** `docs/design/TA1-Validation-Rules-Technical-Specification.md`
- **Implementierung:** `CALC-004-to-CALC-007-IMPLEMENTATION.md`

---

**Bericht Erstellt:** 01.02.2026
**Implementierungsfortschritt:** 27% (18/67 Regeln)
**Nächster Meilenstein:** BTM-Validierung (4 Regeln)
**Ziel-Fertigstellung:** Vollständige Validierungsabdeckung bis Q1 2026
