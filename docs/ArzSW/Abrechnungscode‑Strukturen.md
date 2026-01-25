# Abrechnungscode‑Strukturen und Validierungsregeln

## Überblick

Das deutsche Apothekenabrechnungssystem verwendet mehrere Codetypen zur Klassifizierung und Validierung von Verordnungen. Dieses Dokument beschreibt Struktur und Validierungsregeln für jeden Codetyp.

---

## 1. Faktorkennzeichen – TA3 Abschnitt 8.2.25

**Zweck:** Kennzeichnet Faktoren für Preisberechnungen bei Teilmengen, Opioid‑Substitution und Verwurf.

**Struktur:**
- **Länge:** 2 Stellen
- **Format:** Numerische Zeichenfolge (z. B. „11“, „55“, „57“, „99“)

**Gültige Codes:**

| Code | Inhalt | Beschreibung | Anwendungsfall |
|------|--------|--------------|----------------|
| `11` | Anteil in Promille | Anteil in Promille | Verarbeitete Packungen, Teilmengen, Zuschläge |
| `55` | Einzeldosis in Milligramm | Einzeldosis in Milligramm | Opioid‑Substitution Take‑Home‑Verordnungen |
| `57` | Einzeldosis in Milligramm | Einzeldosis in Milligramm | Opioid‑Substitution unter Aufsicht |
| `99` | Anteil einer Packung in Promille | Packungsanteil in Promille | Verwurf/Entsorgung |

**Validierungsregeln:**
1. Muss exakt 2 Stellen haben
2. Muss einer der Werte 11, 55, 57, 99 sein
3. Code 55 und 57 sind ausschließlich für Opioid‑Substitution
4. Code 99 kennzeichnet Verwurf und erfordert Sonderbehandlung

---

## 2. Preiskennzeichen – TA3 Abschnitt 8.2.25

**Zweck:** Kennzeichnet die Preisart für Abrechnungsberechnungen.

**Struktur:**
- **Länge:** 2 Stellen
- **Format:** Numerische Zeichenfolge (z. B. „11“, „12“, „21“)

**Gültige Codes:**

| Code | Inhalt | Steuerstatus | Beschreibung |
|------|--------|-------------|--------------|
| `11` | Apothekeneinkaufspreis nach AMPreisV | o. USt. | Apothekeneinkaufspreis nach Arzneimittelpreisverordnung |
| `12` | Von Apotheke mit pharma. Unternehmer vereinbarter Preis | o. USt. | Zwischen Apotheke und Hersteller vereinbarter Preis |
| `13` | Von Apotheke tatsächlich geleisteter Einkaufspreis | o. USt. | Tatsächlicher Einkaufspreis der Apotheke |
| `14` | Abrechnungspreis nach § 4 und 5 AMPreisV – „Hilfstaxe“ | o. USt. | Abrechnungspreis nach Hilfstaxe (Stoffe) |
| `15` | Zwischen Apotheke und Krankenkasse vereinbarter Preis § 129 Abs. 5 SGB V | o. USt. | Vertragspreis zwischen Apotheke und Krankenkasse |
| `16` | Vertragspreise auf Grundlage von § 129a SGB V | o. USt. | Vertragspreise gemäß § 129a SGB V |
| `17` | Abrechnungspreis „Preis 2“ nach Verzeichnis mg‑Preise | o. USt. | Abrechnungspreis „Preis 2“ gemäß mg‑Preisverzeichnis |
| `21` | Abrechnungspreis bei Rabattvertrag § 130a Abs. 8c SGB V „Preis 1“ | o. USt. | Abrechnungspreis bei Rabattvertrag „Preis 1“ |

**Validierungsregeln:**
1. Muss exakt 2 Stellen haben
2. Muss einer der Werte 11–17, 21 sein
3. Alle Preise sind ohne Umsatzsteuer (USt.)
4. Code 21 gilt speziell für Rabattvertragsfälle (§ 130a Abs. 8c)
5. Code 14 gilt für Rezepturen („Hilfstaxe“)
6. Code 15 erfordert einen Vertrag zwischen Apotheke und bestimmter Krankenkasse

---

## 3. Sonderkennzeichen Standard (SOK1) – TA1 Anhang 1

**Zweck:** Standard‑Sonderkennzeichen für Arzneimittel und Leistungen ohne PZN.

**Struktur:**
- **Länge:** 8 Stellen
- **Format:** Numerische Zeichenfolge (z. B. „09999005“, „02567001“)
- **Bereich:** Typisch beginnend mit 02, 06, 09 oder 17–18

**Kategorien:**

### 3.1 Arzneimittel ohne PZN
| SOK | Beschreibung | USt. | E‑Rezept | Apothekenabschlag |
|-----|--------------|------|---------|-------------------|
| `09999005` | Verschreibungspflichtige Arzneimittel ohne PZN | 19% | Ja | Nach § 130 Abs. 1/1a SGB V |
| `09999175` | Nicht‑verschreibungspflichtige Arzneimittel ohne PZN | 19% | Ja | Nach § 130 Abs. 1 SGB V |
| `09999117` | Einzelimportierte verschreibungspflichtige Arzneimittel (§ 73 Abs. 3 AMG) | 19% | Ja | Nach § 130 Abs. 1/1a SGB V |

### 3.2 Rezepturen
| SOK | Beschreibung | USt. | E‑Rezept | Apothekenabschlag |
|-----|--------------|------|---------|-------------------|
| `09999011` | Rezepturen nach § 5 Abs. 3 AMPreisV | 19% | Ja | Nach § 130 Abs. 1/1a SGB V |
| `06460702` | Unverarbeitete Stoffe nach Ziffer 4.4 | 19% | Ja | Nach § 130 Abs. 1 SGB V |

### 3.3 Cannabis‑Produkte
| SOK | Beschreibung | USt. | E‑Rezept |
|-----|--------------|------|---------|
| `06460694` | Cannabisblüten unverändert | 19% | Ja |
| `06460665` | Cannabisblüten in Zubereitungen | 19% | Ja |
| `06460754` | Cannabinoid‑Stoffe unverändert | 19% | Ja |
| `06460748` | Cannabinoid‑Stoffe in Zubereitungen | 19% | Ja |

### 3.4 Spezialzubereitungen (variable USt.)
| SOK | Beschreibung | USt. | E‑Rezept |
|-----|--------------|------|---------|
| `09999092` | Zytostatika‑Zubereitungen | 19% | Ja |
| `06460866` | Zytostatika‑Zubereitungen | 7% | Ja |
| `06460872` | Zytostatika‑Zubereitungen | 0% | Ja |
| `09999100` | Parenterale Ernährungslösungen | 19% | Ja |
| `06460889` | Parenterale Ernährungslösungen | 7% | Ja |
| `06460895` | Parenterale Ernährungslösungen | 0% | Ja |

### 3.5 Opioid‑Substitution
| SOK | Beschreibung | USt. | E‑Rezept | Zusatzdaten |
|-----|--------------|------|---------|------------|
| `09999086` | Methadon‑Teilmengen (Anlage 4) | 19% | Nein | Erforderlich |
| `02567107` | Levomethadon‑Teilmengen (Anlage 5) | 19% | Nein | Erforderlich |
| `02567113` | Buprenorphin/Subutex‑Einzeldosen | 19% | Nein | Erforderlich |
| `02567136` | Buprenorphin/Naloxon‑Einzeldosen | 19% | Nein | Erforderlich |

### 3.6 Blutprodukte (typisch steuerfrei)
| SOK | Beschreibung | USt. | E‑Rezept |
|-----|--------------|------|---------|
| `02567515` | Granulozyten ohne PZN | 0% | Ja |
| `02567521` | Vollblut ohne PZN | 0% | Ja |
| `02567484` | Erythrozytenkonzentrate ohne PZN | 0% | Ja |
| `02567490` | Thrombozytenkonzentrate ohne PZN | 0% | Ja |

### 3.7 Gebühren und Dienstleistungen
| SOK | Beschreibung | USt. | E‑Rezept | Zusatzdaten |
|-----|--------------|------|---------|------------|
| `02567001` | BTM‑Gebühr nach Ziffer 4.1 | 19% | Nein | Erforderlich |
| `06460688` | T‑Rezept‑Gebühr nach Ziffer 4.1 | 19% | Nein | Erforderlich |
| `02567018` | Noctu‑Gebühr nach Ziffer 4.2 | 19% | Ja | Erforderlich |
| `06461110` | Botendienst | 19% | Ja | Erforderlich |

### 3.8 Grippeimpfungen (GKV)
| SOK | Beschreibung | USt. | E‑Rezept |
|-----|--------------|------|---------|
| `17716926` | Vereinbarter Preis für Impfleistung | 0% | Ja |
| `17716955` | Vereinbarter Preis für Impffremdleistungen | 0% | Ja |
| `18774512` | Gesetzliche Beschaffungskosten § 132e Abs. 1a | 0% | Ja |

### 3.9 COVID‑19‑Impfungen
| SOK | Beschreibung | USt. | E‑Rezept | Gültig bis |
|-----|--------------|------|---------|-----------|
| `17717400` | COVID‑Impfleistung (aktualisiert) | 0% | Ja | Aktiv |
| `18774908` | COVID‑Impfmaterialien | 0% | Ja | Aktiv |

### 3.10 Pharmazeutische Dienstleistungen
| SOK | Beschreibung | USt. | E‑Rezept |
|-----|--------------|------|---------|
| `17716783` | Erweiterte Einweisung in die Inhalationstechnik | n. a. | Ja |
| `17716808` | Erweiterte Medikationsberatung bei Polymedikation | n. a. | Ja |
| `17716820` | Pharmazeutische Betreuung bei oraler Antitumortherapie | n. a. | Ja |
| `17716843` | Pharmazeutische Betreuung bei Organtransplantation | n. a. | Ja |

**Validierungsregeln:**
1. Muss exakt 8 Stellen haben
2. Muss in der SOK1‑Referenztabelle vorhanden sein
3. Gültigkeit prüfen (nicht abgelaufen laut „Außerkrafttreten“‑Datum)
4. USt‑Satz muss der Spezifikation entsprechen
5. E‑Rezept‑Kompatibilität muss geprüft werden
6. Feld „Zusatzdaten“ zeigt erforderliche Zusatzinformationen an:
   - `0` = Keine Zusatzdaten erforderlich
   - `1` = Zusatzdaten erforderlich (z. B. Zusammensetzung für Rezepturen)
   - `2` = Zusatzdaten für Faktoren/Preise
   - `3` = Zusatzdaten für Substitutionsdosen
   - `4` = Zusatzdaten für Gebühren/Leistungen

---

## 4. Sonderkennzeichen Vertragsbezogen (SOK2) – TA1 Anhang 2

**Zweck:** Sonderkennzeichen zwischen bestimmten Krankenkassen und Apothekenverbänden (regional/vertragsbezogen) vereinbart.

**Struktur:**
- **Länge:** 8 Stellen
- **Format:** Numerische Zeichenfolge (z. B. „02566740“, „17716518“)
- **Zuordnung:** Spezifischen Organisationen zugewiesen (DAV, Landesverbände)

**Beispiele:**

| SOK | Beschreibung | Zugeordnet zu | Gültig ab | Gültig bis |
|-----|--------------|--------------|----------|-----------|
| `02566740` | Homöopathie‑Vertrag DAV‑mhplusBKK | DAV | 01.09.2005 | Aktiv |
| `06460501` | Ergänzung Rahmenvertrag AOK BW | LAV Baden‑Württemberg | 01.04.2015 | 01.01.2025 |
| `17716518` | Rekonstitution Risdiplam (Evrysdi) | DAV/vdek | 01.02.2022 | 30.04.2025 |
| `17717392` | Import wegen Lieferengpass | BAV | 03.05.2023 | Aktiv |
| `18774506` | Vergütung beaufsichtigte Opioid‑Abgabe | DAV | 01.05.2024 | Aktiv |

**Validierungsregeln:**
1. Muss exakt 8 Stellen haben
2. Muss in der SOK2‑Referenztabelle vorhanden sein
3. Gültigkeitszeitraum prüfen:
   - „Gültig ab Abrechnungsmonat“ / „Gültig ab Abgabedatum“ = Startdatum
   - „Außerkrafttreten Abrechnungsmonat“ / „Außerkrafttreten Abgabedatum“ = Enddatum
4. Prüfen, ob Apotheke zur Nutzung berechtigt ist (Verband/Vertrag)
5. Einige Codes sind auf spezifische Organisationen begrenzt (z. B. LAV Niedersachsen, SAV)

---

## 5. Codeübergreifende Validierungsregeln

### 5.1 Kombinationen SOK + Faktorkennzeichen

| SOK‑Muster | Erforderliches Faktorkennzeichen | Begründung |
|-----------|----------------------------------|-----------|
| Opioid‑Substitutions‑SOK (09999086, 02567107, usw.) | `55` oder `57` | Dosis in Milligramm erforderlich |
| Teilmengen‑SOK (09999057, 09999198) | `11` | Anteil in Promille erforderlich |
| Verwurf‑Kennzeichen | `99` | Verwurfmenge in Promille erforderlich |

### 5.2 Kombinationen SOK + Preiskennzeichen

| SOK‑Muster | Typisches Preiskennzeichen | Begründung |
|-----------|----------------------------|-----------|
| Standard‑Arzneimittel | `11`, `12`, `13` | Reguläre Apothekeneinkaufspreise |
| Rezepturen (09999011) | `14` | Hilfstaxe |
| Rabattvertrags‑Arzneimittel | `21` | Rabattvertrag „Preis 1“ |
| Vertragsbezogene Arzneimittel (SOK2) | `15`, `16` | Vereinbarte Vertragspreise |

### 5.3 USt‑Konsistenz

**Regel:** Der in den Abrechnungsdaten angegebene USt‑Satz muss der SOK‑Definition entsprechen.

| USt‑Code | Beschreibung | Anwendbare SOK |
|---------|--------------|----------------|
| `0` | Steuerfrei (0%) | Blutprodukte, Impfungen, Krankenhaus‑Apothekenleistungen |
| `1` | Ermäßigter Satz (7%) | Bestimmte Krankenhaus‑Zubereitungen |
| `2` | Regulärer Satz (19%) | Die meisten Arzneimittel und Apothekenleistungen |
| `-` | Nicht anwendbar | Service‑Codes ohne direkte USt. (z. B. Gebührenkennzeichen) |

### 5.4 E‑Rezept‑Kompatibilität

**Regel:** Wenn eine Verordnung als E‑Rezept übermittelt wird, muss das SOK E‑Rezept‑fähig sein.

- Spalte „E‑Rezept“ in den SOK‑Tabellen prüfen
- Werte: `0` = nicht kompatibel, `1` = kompatibel, `2` = Sonderbehandlung erforderlich

---

## 6. Datenqualitätsprüfungen

### 6.1 Zeitliche Validierung
```
IF prescription.dispensing_date < SOK.valid_from THEN
  ERROR "SOK am Abgabedatum noch nicht gültig"

IF SOK.valid_until IS NOT NULL AND prescription.dispensing_date > SOK.valid_until THEN
  ERROR "SOK am Abgabedatum abgelaufen"
```

### 6.2 Organisatorische Berechtigung
```
IF SOK.assigned_to IS NOT NULL THEN
  IF pharmacy.association NOT IN SOK.assigned_to THEN
    ERROR "Apotheke nicht berechtigt, dieses vertragsbezogene SOK zu verwenden"
```

### 6.3 Erforderliche Zusatzdaten
```
IF SOK.zusatzdaten > 0 THEN
  IF prescription.additional_data IS NULL THEN
    ERROR "Zusatzdaten erforderlich, aber nicht vorhanden"
```

### 6.4 Logik für Codekombinationen
```
IF SOK IN opioid_substitution_list THEN
  IF faktorkennzeichen NOT IN ['55', '57'] THEN
    ERROR "Opioid‑Substitution erfordert Dosisfaktor (55 oder 57)"

IF SOK.category = 'compounded_preparation' THEN
  IF preiskennzeichen != '14' THEN
    WARNING "Rezepturen verwenden typischerweise Preiskennzeichen 14 (Hilfstaxe)"
```

---

## 7. Fehlermeldungen

### Schweregrade

| Stufe | Beschreibung | Aktion |
|-------|--------------|--------|
| `ERROR` | Kritischer Fehler, Abrechnung wird abgelehnt | Vor Übermittlung beheben |
| `WARNING` | Auffälligkeit, wahrscheinlich falsch | Prüfen, ggf. fortfahren |
| `INFO` | Hinweis | Keine Aktion erforderlich |

### Häufige Fehlercodes

| Code | Meldung | Schweregrad |
|------|--------|------------|
| `SOK_INVALID` | SOK‑Code nicht in Referenztabellen gefunden | ERROR |
| `SOK_EXPIRED` | SOK‑Code am Abgabedatum abgelaufen | ERROR |
| `SOK_NOT_YET_VALID` | SOK‑Code am Abgabedatum noch nicht gültig | ERROR |
| `SOK_UNAUTHORIZED` | Apotheke nicht berechtigt für dieses SOK2 | ERROR |
| `FACTOR_INVALID` | Ungültiges Faktorkennzeichen | ERROR |
| `FACTOR_REQUIRED` | Faktorkennzeichen erforderlich, aber fehlt | ERROR |
| `FACTOR_MISMATCH` | Faktorkennzeichen passt nicht zu SOK | ERROR |
| `PRICE_INVALID` | Ungültiges Preiskennzeichen | ERROR |
| `PRICE_MISMATCH` | Preiskennzeichen passt nicht zum SOK‑Typ | WARNING |
| `VAT_MISMATCH` | USt‑Satz passt nicht zur SOK‑Spezifikation | ERROR |
| `EREZEPT_INCOMPATIBLE` | SOK nicht E‑Rezept‑kompatibel | ERROR |
| `ADDITIONAL_DATA_REQUIRED` | Zusatzdaten erforderlich, fehlen aber | ERROR |

---

## 8. Implementierungshinweise

### 8.1 Referenzdaten‑Management
- SOK1‑ und SOK2‑Tabellen regelmäßig aus offiziellen Quellen aktualisieren
- Effektivdatierung pflegen: Historie für rückwirkende Validierung vorhalten
- Änderungen in Codedefinitionen (USt‑Sätze, Gültigkeitszeiträume) nachverfolgen

### 8.2 Validierungsstrategie
1. **Statische Validierung:** Format und Existenz prüfen
2. **Zeitliche Validierung:** Gültigkeitsdaten gegen Abgabedatum prüfen
3. **Berechtigungsprüfung:** Apotheke für Vertrags‑SOK prüfen
4. **Querverweise:** Codekombinationen (SOK + Faktor + Preis) prüfen
5. **Geschäftslogik:** Fachregeln anwenden

### 8.3 Performance‑Überlegungen
- SOK‑Codes für schnellen Zugriff indizieren
- Häufig verwendete Regeln cachen
- Gültige SOK‑Listen pro Zeitraum vorkomputieren
- Bulk‑Validierung für Batch‑Verarbeitung

---

## 9. Referenzen

### Offizielle Dokumente
- **TA1 (Technische Anlage 1):** Technische Spezifikation für den Austausch von Abrechnungsdaten
- **TA3 (Technische Anlage 3):** Datenträger‑Spezifikation und Formatdefinitionen
- **Anhang 1:** Standard‑Sonderkennzeichen (SOK1)
- **Anhang 2:** Vertragsbezogene Sonderkennzeichen (SOK2)
- **AMPreisV:** Arzneimittelpreisverordnung
- **SGB V:** Sozialgesetzbuch V

### Relevante Abschnitte
- TA1 Abschnitt 4: Abrechnungsdatenstrukturen
- TA1 Anhang 1: SOK1‑Definitionen
- TA1 Anhang 2: SOK2‑Definitionen
- TA3 Abschnitt 8.2.25: Faktorkennzeichen und Preiskennzeichen

---

## 10. Glossar

| Begriff | Deutsch | Beschreibung |
|--------|--------|--------------|
| **SOK** | Sonderkennzeichen | Sonderkennzeichen |
| **PZN** | Pharmazentralnummer | Pharmazentralnummer (Standard‑Arzneimittel‑ID) |
| **Faktorkennzeichen** | Faktorkennzeichen | Code zur Kennzeichnung von Berechnungsfaktoren |
| **Preiskennzeichen** | Preiskennzeichen | Code zur Kennzeichnung der Preisart |
| **AMPreisV** | Arzneimittelpreisverordnung | Arzneimittelpreisverordnung |
| **Hilfstaxe** | Hilfstaxe | Hilfstaxe für Rezepturen |
| **BTM** | Betäubungsmittel | Betäubungsmittel
| **Noctu** | Nacht/Notdienst | Notdienst außerhalb der Öffnungszeiten |
| **Zusatzdaten** | Zusatzdaten | Erforderliche Zusatzinformationen |
| **DAV** | Deutscher Apothekerverband | Deutscher Apothekerverband |
| **vdek** | Verband der Ersatzkassen | Verband der Ersatzkassen |
| **LAV** | Landesapothekerverband | Landesapothekerverband |

---

**Dokumentversion:** 1.0
**Letzte Aktualisierung:** 2026-01-24
**Basierend auf:** TA1_039_20250331, TA3_042_20240906
