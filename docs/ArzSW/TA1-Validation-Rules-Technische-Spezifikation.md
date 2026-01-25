# TA1 Validierungsregeln – Technische Spezifikation

**Dokumentversion:** 1.0
**Basierend auf:** TA1 Version 039, Stand 31.03.2025
**Gültig ab:** Abrechnungsmonat 10/2025
**Autor:** E‑Rezept‑Validator Team
**Datum:** 2026-01-24
**Referenz:** § 300 SGB V – Technische Anlage 1 zur Arzneimittelabrechnungsvereinbarung

---

## Inhaltsverzeichnis

1. [Dokumentübersicht](#1-dokumentübersicht)
2. [Allgemeine Validierungsregeln](#2-allgemeine-validierungsregeln)
3. [Datenformat-Validierungen](#3-datenformat-validierungen)
4. [BTM- & T‑Rezept‑Validierungen](#4-btm--t-rezept-validierungen)
5. [Validierungen von Sondergebühren](#5-validierungen-von-sondergebühren)
6. [Rezepturen (Herstellungen)](#6-rezepturen-herstellungen)
7. [Cannabis-spezifische Validierungen](#7-cannabis-spezifische-validierungen)
8. [Wirtschaftliche Einzelmengen](#8-wirtschaftliche-einzelmengen)
9. [Validierungen von Sonderfällen](#9-validierungen-von-sonderfällen)
10. [Preis- und Faktorberechnungen](#10-preis--und-faktorberechnungen)
11. [Prioritätsmatrix der Validierungsregeln](#11-prioritätsmatrix-der-validierungsregeln)
12. [Fehlercodes und Meldungen](#12-fehlercodes-und-meldungen)

---

## 1. Dokumentübersicht

### 1.1 Zweck

Diese technische Spezifikation definiert den vollständigen Satz an Validierungsregeln für E‑Rezept‑Abrechnungsdaten gemäß TA1 Version 039. Diese Regeln stellen die Einhaltung der deutschen Arzneimittelabrechnungsregelungen (§ 300 SGB V) sicher und verhindern Ablehnungen durch Krankenkassen.

### 1.2 Zugehörige Dokumentation

Dieses Dokument ist Teil einer umfassenden Validierungsdokumentation:

- **Dieses Dokument (TA1‑Validation‑Rules):** Validierungslogik, Regeln und Algorithmen (WIE validiert wird)
- **[CODE_STRUCTURES.md](./Abrechnung/CODE_STRUCTURES.md):** Vollständige Codekataloge und Referenzdaten (WORAUF validiert wird)
  - 172 SOK1‑Codes (standardisierte Sonderkennzeichen)
  - 109 SOK2‑Codes (vertragsbezogene Sonderkennzeichen)
  - Faktor- und Preiskennzeichen‑Definitionen
  - Codeübergreifende Validierungsregeln
- **[VALIDATION_EXAMPLES.md](./Abrechnung/VALIDATION_EXAMPLES.md):** Praktische Validierungsszenarien (WIE es aussieht)
  - 16 detaillierte Beispiele mit Ein- und Ausgaben
  - Pass/Fail‑Szenarien
  - Workflow‑Diagramme

### 1.3 Geltungsbereich

Diese Spezifikation umfasst:
- Validierung von E‑Rezept‑Abgabedaten
- Format- und Strukturvalidierungen
- Geschäftslogik‑Validierungen
- Sonderfallbehandlung (BTM, Cannabis, Rezepturen usw.)
- Regeln zur Preis‑ und Faktorberechnung

### 1.3 Wichtige Änderungen in TA1 Version 039

**Stichtag:** Oktober 2025

- **Abschnitt 4.14:** Vollständige Überarbeitung der Cannabis‑Regelungen
- **Abschnitt 4.14.2:** Neue Regeln zur Dezimalstellenbehandlung bei Faktoren
- **Abschnitt 4.5.2:** Überarbeitete Regeln für Verordnungen nach § 3 Abs. 4
- **Abschnitt 4.10:** Aktualisierungen für BTM‑, Noctu‑ und T‑Rezept‑Gebühren
- **Abschnitt 5:** Ergänzungen der technischen Beschreibung für Wiederbeschaffung, Noctu‑ und BTM‑Gebühren

### 1.4 Referenzdokumente

- TA1 (Technische Anlage 1) Version 039, Stand 31.03.2025
- TA3 (Technische Anlage 3) – Codetabellen und Segmentdefinitionen
- TA7 (Technische Anlage 7) – Abgabedatenstruktur
- AMPreisV (Arzneimittelpreisverordnung)
- BtMG (Betäubungsmittelgesetz)
- SGB V § 300 – Rechtsrahmen für die Datenübermittlung
- FHIR R4 Standard – Standard für Gesundheitsdatenaustausch
- gematik Spezifikationen – E‑Rezept technische Spezifikationen

---

## 2. Allgemeine Validierungsregeln

### 2.1 Zeitzonen- und Zeitstempelregeln

**Referenz:** TA1 Abschnitt 1, Seite 5

#### Regel GEN-001: Deutsche Zeitzone
```
Schweregrad: ERROR
Bedingung: Alle Zeitstempel müssen in deutscher Zeit (CET/CEST) vorliegen
Felder: Alle Datums-/Zeitfelder in Abgabedaten
Implementierung:
- Zeitzone muss Europe/Berlin sein
- Offset +01:00 (CET) oder +02:00 (CEST) je nach Datum akzeptieren
- Zeitstempel ohne Zeitzonenangabe ablehnen
```

#### Regel GEN-002: Stichtag für Feldänderungen
```
Schweregrad: ERROR
Bedingung: Feldänderungen basieren auf Abgabedatum, nicht auf Abrechnungsmonat
Felder: Referenziert durch Feld ID 5 (TA7) und ZUP-11 (TA3)
Implementierung:
- Abgabedatum als Referenz für anwendbare Regeln verwenden
- Nicht den Abrechnungsmonat verwenden
```

### 2.2 Bruttopreis‑Regeln

**Referenz:** TA1 Abschnitt 1, Seite 5

#### Regel GEN-003: Zusammensetzung des Bruttopreises
```
Schweregrad: ERROR
Bedingung: Bruttopreis muss immer den Apothekenabgabepreis nach AMPreisV abbilden
Felder: Bruttopreis (ID 23 in TA7)
Implementierung:
- Bruttopreis = Apothekenabgabepreis gemäß AMPreisV oder vertraglicher Regelung
- Zuzahlung darf NICHT abgezogen werden
- Mehrkosten dürfen NICHT abgezogen werden
- Eigenbeteiligung darf NICHT abgezogen werden
Validierung:
- Bruttopreis > 0 prüfen
- Sicherstellen, dass Zuzahlungen in separaten Feldern stehen
```

#### Regel GEN-004: USt‑Berechnung für gesetzliche Gebühren
```
Schweregrad: WARNING
Bedingung: Gesetzliche Gebühren (BTM‑Gebühr, Noctu, T‑Rezept) müssen USt‑bereinigt sein
Felder: Bruttopreis bei Gebühr‑Sonderkennzeichen
Implementierung:
- Gesetzliche Gebühren um den USt‑Anteil reduzieren
- Berechnete Bruttopreise müssen den gesetzlichen Preisen exakt entsprechen
- TA3‑Rundungsregeln gelten hier NICHT
Formel:
- Netto‑Gebühr = Brutto‑Gebühr / (1 + USt‑Satz)
- USt‑Satz in Deutschland typischerweise 19%
```

### 2.3 Validierung von Sonderkennzeichen (SOK)

**Referenz:** TA1 Abschnitt 4.14.1, 4.14.2; [CODE_STRUCTURES.md](./Abrechnung/CODE_STRUCTURES.md) Abschnitte 3–4

#### Regel GEN-005: Übermittlung von Sonderkennzeichen
```
Schweregrad: ERROR
Bedingung: Sonderkennzeichen für elektronische Zusatzdaten müssen in ZDP‑Segmenten stehen
Felder: Sonderkennzeichen in elektronischen Daten
Implementierung:
- Papierrezepte: auf Formular gedruckt
- E‑Rezept: in der Abgabedatenstruktur übertragen
- Jedes Sonderkennzeichen maximal einmal pro Verordnung
- Mehrfachgebühren über Faktor‑Feld (Vielfache von 1000.000000) abbilden
```

#### Regel GEN-006: SOK‑Gültigkeitsprüfung
```
Schweregrad: ERROR
Bedingung: Sonderkennzeichen muss am Abgabedatum gültig sein
Felder: Sonderkennzeichen, Abgabedatum
Referenz: CODE_STRUCTURES.md Abschnitt 6.1
Implementierung:
- SOK aus Referenztabelle (SOK1 oder SOK2) ermitteln
- Prüfen: Abgabedatum >= SOK.valid_from
- Prüfen: SOK.valid_until IST NULL ODER Abgabedatum <= SOK.valid_until
- Fehler, falls außerhalb der Gültigkeit
Fehlermeldung:
- "SOK {code} ist am {valid_until} abgelaufen. Abgabedatum {dispensing_date} liegt außerhalb der Gültigkeit."
- "SOK {code} ist noch nicht gültig. Gültig ab {valid_from}, Abgabedatum {dispensing_date}."
Beispiel:
  SOK 17717104 (VAXIGRIP 2022/2023) abgelaufen am 2024-08-01
  Abgabedatum 2025-01-15 → ERROR
```

#### Regel GEN-007: E‑Rezept‑Kompatibilität von SOK
```
Schweregrad: ERROR
Bedingung: SOK muss E‑Rezept unterstützen, wenn Verordnung E‑Rezept ist
Felder: E‑Rezept‑Flag, Sonderkennzeichen
Referenz: CODE_STRUCTURES.md Abschnitt 5.4
Implementierung:
- Wenn prescription.is_e_rezept == true
- SOK.e_rezept aus Referenztabelle ermitteln
- Prüfen: SOK.e_rezept IN [1, 2] (1=kompatibel, 2=Sonderbehandlung)
- Fehler, wenn SOK.e_rezept == 0 (nicht kompatibel)
Fehlermeldung:
- "SOK {code} ({description}) ist nicht mit E‑Rezept kompatibel. E‑Rezept‑Flag muss false sein."
Beispiel:
  SOK 09999057 (Teilmenge) hat e_rezept=0
  Nicht mit E‑Rezept nutzbar → Papierverordnung erforderlich
```

#### Regel GEN-008: USt‑Satz‑Konsistenz
```
Schweregrad: ERROR
Bedingung: USt‑Satz muss der SOK‑Spezifikation entsprechen
Felder: USt‑Satz‑Code, Sonderkennzeichen
Referenz: CODE_STRUCTURES.md Abschnitt 5.3
Implementierung:
- SOK.vat_rate aus Referenztabelle ermitteln
- USt‑Codes zuordnen: 0=0% (steuerfrei), 1=7% (ermäßigt), 2=19% (voll), -=N/A
- Prüfen: prescription.vat_code == SOK.vat_rate
- Fehler bei Abweichung
Fehlermeldung:
- "USt‑Satz stimmt nicht für SOK {code} ({description}). Erwartet: {expected}%, gefunden: {actual}%."
Beispiel:
  SOK 02567515 (Granulozyten) hat vat_rate=0 (steuerfrei)
  Bei vat_code=2 (19%) → ERROR
```

---

## 3. Datenformat‑Validierungen

### 3.1 PZN (Pharmazentralnummer)‑Format

**Referenz:** TA1 Abschnitt 4.14.2, Seite 39

#### Regel FMT-001: PZN‑Formatvalidierung
```
Schweregrad: ERROR
Bedingung: PZN muss genau 8 Ziffern haben, alphanumerisch mit führenden Nullen
Feld: PZN_Sonderkennzeichen
Format: 8‑stellig alphanumerisch
Beispiele:
  Gültig:   "01234567", "00123456"
  Ungültig: "1234567" (zu kurz), "123456789" (zu lang)
Implementierung:
- Regex: ^[0-9]{8}$
- Falls nötig links mit Nullen auffüllen
- Sonderkennzeichen als Alternative zulassen
```

#### Regel FMT-002: PZN‑Prüfziffernvalidierung
```
Schweregrad: WARNING
Bedingung: Prüfziffernvalidierung für PZN (wenn PZN, kein Sonderkennzeichen)
Feld: PZN
Implementierung:
- PZN‑Prüfziffernalgorithmus (Modulo 11) anwenden
- Letzte Ziffer ist Prüfziffer
- Nur Warnung (kein Fehler), da manche Sonderkennzeichen keine Prüfziffer haben
```

### 3.2 Zeitstempelformat‑Validierung

**Referenz:** TA1 Abschnitt 4.14.2, Seite 39

#### Regel FMT-003: ISO‑8601‑Datums-/Zeitformat
```
Schweregrad: ERROR
Bedingung: Herstellungszeitstempel muss ISO‑8601‑konform sein
Feld: Herstellungsdatum und Zeitpunkt der Herstellung
Akzeptierte Formate:
  - YYYY-MM-DDTHH:MM:00Z (UTC)
  - YYYY-MM-DDThh:mm:ss+zz:zz (mit Zeitzonenoffset)
Beispiele:
  Gültig:   "2025-10-15T14:30:00Z", "2025-10-15T16:30:00+02:00"
  Ungültig: "2025-10-15 14:30:00", "15.10.2025 14:30"
Implementierung:
- ISO‑8601‑Parser verwenden
- Zeitzonenanteil prüfen
- Für Vergleiche in UTC konvertieren
```

### 3.3 Numerische Feldformate

**Referenz:** TA1 Abschnitt 4.14.2, Seite 39

#### Regel FMT-004: Herstellerkennzeichen‑Format
```
Schweregrad: ERROR
Feld: Kennzeichen des Herstellenden
Format: 9‑stellige numerische Zeichenfolge
Beispiele:
  Gültig:   "123456789", "001234567"
  Ungültig: "12345678" (zu kurz), "ABCD12345"
Implementierung:
- Regex: ^[0-9]{9}$
- Apotheker‑IK oder Avoxa/ABDATA‑Herstellerkennzeichen
```

#### Regel FMT-005: Zähler‑Feldformate
```
Schweregrad: ERROR
Felder:
  - Zähler Herstellungssegment: 1–2 Ziffern numerisch
  - Zähler Einheit: 1–n Ziffern numerisch
  - Zähler Abrechnungsposition: 1–n Ziffern numerisch
Implementierung:
- Zähler Herstellung: ^[0-9]{1,2}$
- Zähler Einheit: ^[0-9]+$
- Zähler Abrechnungsposition: ^[0-9]+$
- Muss bei "1" beginnen und fortlaufend sein
```

#### Regel FMT-006: Chargenbezeichnung‑Format
```
Schweregrad: WARNING
Feld: Chargenbezeichnung
Format: 1–20 alphanumerische Zeichen
Implementierung:
- Regex: ^[A-Za-z0-9]{1,20}$
- Optionales Feld
```

#### Regel FMT-007: Faktorkennzeichen‑Format
```
Schweregrad: ERROR
Feld: Faktorkennzeichen
Format: 2‑stellig alphanumerisch
Referenz: TA3 Tabelle 8.2.25
Beispiele: "11", "55", "57"
Implementierung:
- Regex: ^[0-9A-Za-z]{2}$
- Abgleich mit TA3‑Codetabelle 8.2.25
```

#### Regel FMT-008: Faktor‑Wertformat
```
Schweregrad: ERROR
Feld: Faktor
Format: 1–13 Ziffern (max. 6 vor dem Dezimalpunkt + 6 nach dem Dezimalpunkt)
Beispiele:
  Gültig:   "1000.000000", "250.500000", "1.0", "3000"
  Ungültig: "1234567.123456" (zu viele Vorkommaziffern), "1.1234567" (zu viele Nachkommastellen)
Implementierung:
- Regex: ^[0-9]{1,6}(\.[0-9]{1,6})?$
- Gesamtlänge ≤ 13 Zeichen inkl. Dezimalpunkt
- NEU in Version 039: Flexible Nachkommennullen (1.0 = 1.000000)
```

#### Regel FMT-009: Preiskennzeichen‑Format
```
Schweregrad: ERROR
Feld: Preiskennzeichen
Format: 2‑stellig alphanumerisch
Referenz: TA3 Tabelle 8.2.26
Beispiele: "11", "13", "14", "15", "90"
Implementierung:
- Regex: ^[0-9A-Za-z]{2}$
- Abgleich mit TA3‑Codetabelle 8.2.26
```

#### Regel FMT-010: Preiswert‑Format
```
Schweregrad: ERROR
Feld: Preis
Format: 1–12 Ziffern in EUR (max. 9 vor dem Dezimalpunkt + 2 nach dem Dezimalpunkt)
Beispiele:
  Gültig:   "123.45", "1234567.99", "0.50"
  Ungültig: "1234567890.12" (zu viele Vorkommaziffern), "10.123" (zu viele Nachkommastellen)
Implementierung:
- Regex: ^[0-9]{1,9}(\.[0-9]{1,2})?$
- Muss ein gültiger Eurobetrag sein
- Für Währungswerte genau 2 Nachkommastellen
```

---

## 4. BTM‑ & T‑Rezept‑Validierungen

### 4.1 BTM‑Gebührenberechnung

**Referenz:** TA1 Abschnitt 4.1.1 b, Seite 8

#### Regel BTM-001: E‑BTM‑Gebühr‑Sonderkennzeichen
```
Schweregrad: ERROR
Bedingung: E‑BTM‑ und E‑T‑Rezept‑Verordnungen erfordern Gebühr‑Sonderkennzeichen
Sonderkennzeichen:
  - BTM‑Gebühr: 02567001
  - T‑Rezept‑Gebühr: [Code aus Anhang 1/2]
Felder:
  - PZN: Sonderkennzeichen
  - Faktor: Anzahl der BTM/T‑Rezept‑Zeilen
  - Bruttopreis: Summe der BTM/T‑Rezept‑Gebühren
Implementierung:
- E‑BTM oder E‑T‑Rezept‑Flag in Verordnung erkennen
- Sonderkennzeichen prüfen
- Faktor = Anzahl der BTM/T‑Rezept‑Zeilen
- Preis = Gebühr × Faktor
Hinweis: Implementierung erst bei vollständigem Roll‑out von E‑BTM/E‑T‑Rezept
```

#### Regel BTM-002: Alle Arzneimittel müssen aufgeführt sein
```
Schweregrad: ERROR
Bedingung: Alle abgegebenen Betäubungsmittel mit PZN, Menge und Preis
Implementierung:
- Jeder BTM/T‑Rezept‑Artikel muss enthalten:
  - Gültige 8‑stellige PZN
  - Menge (Menge)
  - Preis (Bruttopreis)
- Keine Auslassungen erlaubt
```

#### Regel BTM-003: 7‑Tage‑Gültigkeit
```
Schweregrad: WARNING
Bedingung: BTM‑Verordnungen sind 7 Tage gültig (BtMG)
Implementierung:
- Tage zwischen Verordnungsdatum und Abgabedatum berechnen
- Warnung bei > 7 Tagen
- Kann auf abgelaufene Verordnung hindeuten
Referenz: BtMG §3
```

#### Regel BTM-004: BTM‑Diagnosepflicht
```
Schweregrad: WARNING
Bedingung: BTM‑Verordnungen erfordern Diagnosecode (ICD‑10)
Feld: Diagnosecode in Verordnung
Implementierung:
- Vorhandensein eines ICD‑10‑Diagnosecodes prüfen
- Warnung bei fehlender Angabe (Regelung nach BtMG §3)
```

---

## 5. Validierungen von Sondergebühren

### 5.1 Botendienstgebühr

**Referenz:** TA1 Abschnitt 4.1.4 b, Seite 9

#### Regel FEE-001: Botendienstgebühr validieren
```
Schweregrad: ERROR
Bedingung: Botendienst nach § 129 Abs. 5g SGB V
Sonderkennzeichen: Botendienstgebühr (aus Anhang 1/2)
Felder:
  - Sonderkennzeichen: Botendienst‑SOK
  - Faktor: "1"
  - Bruttopreis: Betrag der Botendienstgebühr
Geltung: Pro Lieferort und Tag für verschreibungspflichtige Arzneimittel
Implementierung:
- Sonderkennzeichen für Botendienst validieren
- Faktor muss exakt "1" oder "1.000000" sein
- Preis muss gültiger Botendienst‑Betrag sein
- Nur eine Gebühr pro Lieferort und Tag
```

### 5.2 Noctu‑Gebühr

**Referenz:** TA1 Abschnitt 4.1.2 b, Seite 8

#### Regel FEE-002: Noctu (Nacht‑/Notdienst)‑Gebühr
```
Schweregrad: ERROR
Bedingung: Nacht‑/Notdienst zwischen 20:00–06:00 oder Wochenende/Feiertag
Sonderkennzeichen: Noctu‑SOK (aus Anhang 1/2)
Felder:
  - Sonderkennzeichen: Noctu‑Code
  - Faktor: "1" oder mehrfach
  - Bruttopreis: Noctu‑Gebühr
Implementierung:
- Abgabezeit zwischen 20:00–06:00 erkennen
- Oder Wochenende (Samstag/Sonntag) erkennen
- Oder Feiertag erkennen
- Noctu‑Sonderkennzeichen anwenden
- USt‑bereinigte Gebühr berechnen (gemäß GEN‑004)
```

### 5.3 Wiederbeschaffungsgebühr

**Referenz:** TA1 Abschnitt 4.1.3 b, Seite 9

#### Regel FEE-003: Wiederbeschaffungsgebühr
```
Schweregrad: ERROR
Bedingung: Notfallbeschaffung nicht verfügbarer Arzneimittel
Sonderkennzeichen: Wiederbeschaffung (aus Anhang 1/2)
Implementierung:
- Sonderkennzeichen für Wiederbeschaffung validieren
- Grund der Nichtverfügbarkeit dokumentieren
- Wiederbeschaffungsgebühr anwenden
```

---

## 6. Rezepturen (Herstellungen)

### 6.1 Allgemeine Rezeptur‑Regeln

**Referenz:** TA1 Abschnitt 4.14.2, Seiten 38–45

#### Regel REZ-001: Rezeptur‑Kennzeichnung
```
Schweregrad: ERROR
Bedingung: Rezepturen müssen die korrekten Sonderkennzeichen verwenden
Sonderkennzeichen:
  - Parenteralia: Bereich 1.7.1 – 1.7.24
  - Wirtschaftliche Einzelmengen: 02567053, 02566993
  - Cannabis‑Zubereitungen: 06461446, 06461423, 06460665, 06460694, 06460748, 06460754
  - Allgemeine Rezeptur: 06460702, 09999011
Implementierung:
- Rezeptur anhand des Sonderkennzeichens erkennen
- In passende Unterregeln routen
- BTM/T‑Rezept‑Substanzen ausschließen (sofern nicht explizit erlaubt)
```

### 6.2 Parenterale Zubereitungen (4.14.2 a)

**Referenz:** TA1 Abschnitt 4.14.2 a, Seiten 40–42

#### Regel REZ-002: Parenteralia – Herstellerkennzeichen
```
Schweregrad: ERROR
Bedingung: Herstellerkennzeichen muss Avoxa/ABDATA‑Code sein
Feld: Kennzeichen des Herstellenden
Implementierung:
- 9‑stelliger numerischer Code
- Von Avoxa/ABDATA im Auftrag des DAV vergeben
- NICHT das Apotheken‑IK
```

#### Regel REZ-003: Parenteralia – Zeitstempelvalidierung
```
Schweregrad: ERROR
Bedingung: Herstellungszeitstempel ≤ Signaturzeitstempel
Felder:
  - Herstellungsdatum und Zeitpunkt
  - Signaturzeitstempel
Implementierung:
- Beide Zeitstempel nach UTC parsen
- Prüfen: manufacturing_timestamp <= signature_timestamp
Fehlermeldung: "Herstellungszeitstempel darf nicht nach dem Signaturzeitstempel liegen"
```

#### Regel REZ-004: Parenteralia – Zählersequenz
```
Schweregrad: ERROR
Bedingung: Fortlaufende Zählernummerierung ab "1"
Felder:
  - Zähler Herstellung
  - Zähler Einheit
  - Zähler Abrechnungsposition
Implementierung:
- Herstellungszähler: beginnt bei 1, erhöht sich pro Herstellungsvorgang
- Einheitenzähler: beginnt bei 1 je Herstellung, erhöht sich pro Einheit
- Abrechnungspositionszähler: beginnt bei 1 je Einheit, erhöht sich pro Position
- Lückenlos (keine fehlenden Nummern)
```

#### Regel REZ-005: Parenteralia – Faktor als Promilleanteil
```
Schweregrad: ERROR
Bedingung: Faktor als Promillewert
Feld: Faktor
Implementierung:
- 1 ganze Packung = 1000.000000
- 3 ganze Packungen = 3000.000000
- Teilpackung anteilig berechnen
- Ausnahme: Sonderkennzeichen ohne Mengenbezug = 1.000000
Beispiele:
  - Ganze Beutel = "1000.000000"
  - 3 Beutel = "3000.000000"
  - 1/2 Beutel = "500.000000"
```

#### Regel REZ-006: Parenteralia – Wochenbedarfsgrenze
```
Schweregrad: WARNING
Bedingung: Maximal 1‑Wochen‑Versorgung identischer Zubereitungen
Implementierung:
- Anzahl identischer Einheiten zählen
- Warnung bei > 7 Einheiten (Hinweis auf Überversorgung)
- Nur informativ (kein Fehler)
```

### 6.3 Wirtschaftliche Einzelmengen (4.14.2 b)

**Referenz:** TA1 Abschnitt 4.14.2 b, Seiten 43–44

#### Regel REZ-007: Einzelmengen – Herstellertyp
```
Schweregrad: ERROR
Bedingung: Herstellerkennzeichen ist Apotheken‑IK, NICHT Avoxa/ABDATA‑Code
Sonderkennzeichen: 02567053 (Auseinzelung), 02566993 (Wochenblister)
Feld: Kennzeichen des Herstellenden
Implementierung:
- 9‑stelliges Apotheken‑IK
- Ausnahme: Wenn Apotheke auch Parenteralia herstellt, kann Avoxa/ABDATA‑Code verwendet werden
```

#### Regel REZ-008: Einzelmengen – Zeitstempelvalidierung
```
Schweregrad: ERROR
Bedingung: Herstellungszeitstempel ≤ Signaturzeitstempel
Sonderkennzeichen: 02567053, 02566993
Implementierung:
- Wie REZ‑003
- Prüfen: manufacturing_timestamp <= signature_timestamp
```

#### Regel REZ-009: Einzelmengen – Zähler für 02567053
```
Schweregrad: ERROR
Bedingung: Auseinzelung immer Zähler = "1"
Sonderkennzeichen: 02567053
Felder:
  - Zähler Herstellung: "1"
  - Zähler Einheit: "1"
Implementierung:
- Beide Zähler müssen exakt "1" sein
- Ein einzelner Abgabevorgang
```

#### Regel REZ-010: Einzelmengen – Zähler für 02566993
```
Schweregrad: ERROR
Bedingung: Wochenblister mit fortlaufenden Zählern
Sonderkennzeichen: 02566993
Felder:
  - Zähler Herstellung: fortlaufend ab "1"
  - Zähler Einheit: fortlaufend je Herstellung ab "1"
Implementierung:
- Herstellungszähler: steigt je Batch
- Einheitenzähler: steigt je Blister/Einheit im Batch
- Lückenlos
```

#### Regel REZ-011: Einzelmengen – Faktorkennzeichen
```
Schweregrad: ERROR
Bedingung: Faktorkennzeichen muss immer "11" sein
Sonderkennzeichen: 02567053, 02566993
Feld: Faktorkennzeichen
Implementierung:
- Fester Wert: "11"
- Referenz: TA3 Tabelle 8.2.25
```

#### Regel REZ-012: Einzelmengen – Teilmengenfaktor
```
Schweregrad: ERROR
Bedingung: Faktorberechnung für Teilmengen
Feld: Faktor
Implementierung:
- Ganze Packung = 1000.000000
- Teilmenge = (abgegebene_Menge / Packungsmenge) × 1000.000000
Beispiel:
  - 7 Tabletten aus 28er‑Packung = (7/28) × 1000 = 250.000000
  - 3 volle Packungen = 3000.000000
```

### 6.4 Cannabis & allgemeine Rezeptur (4.14.2 c)

**Referenz:** TA1 Abschnitt 4.14.2 c, Seiten 45–46

#### Regel REZ-013: Cannabis/ Rezeptur – Sonderkennzeichen
```
Schweregrad: ERROR
Bedingung: Korrektes Sonderkennzeichen je Zubereitungsart
Sonderkennzeichen:
  Cannabis (Anlage 10):
    - 06461446: Cannabisblüten (getrocknet)
    - 06461423: Cannabis‑Extrakte
    - 06460665: Dronabinol‑Zubereitung Typ 1
    - 06460694: Dronabinol‑Zubereitung Typ 2
    - 06460748: Cannabis‑Zubereitung Typ 3
    - 06460754: Cannabis‑Zubereitung Typ 4
  Allgemeine Rezeptur (§§ 4,5 AMPreisV):
    - 06460702: Standardrezeptur
    - 09999011: Alternative Rezeptur
Implementierung:
- Sonderkennzeichen aus zulässiger Liste validieren
- Keine BTM/T‑Rezept‑Substanzen (außer Cannabis ausdrücklich)
```

#### Regel REZ-014: Cannabis/ Rezeptur – Herstellerkennzeichen
```
Schweregrad: ERROR
Bedingung: Herstellerkennzeichen ist Apotheken‑IK
Feld: Kennzeichen des Herstellenden
Implementierung:
- 9‑stelliges Apotheken‑IK
- Ausnahme: Wenn Apotheke Parenteralia herstellt, kann Avoxa/ABDATA‑Code verwendet werden
```

#### Regel REZ-015: Cannabis/ Rezeptur – Herstellungszeitstempel
```
Schweregrad: ERROR
Bedingung: Zeitstempel = Abgabedatum + 00:00
Feld: Herstellungsdatum und Zeitpunkt
Implementierung:
- Abgabedatum (Abgabedatum) ermitteln
- Zeitanteil auf "00:00" setzen
- Format: YYYY-MM-DDT00:00:00+zz:zz
Beispiel: Abgabe 2025-10-15 um 14:30 → "2025-10-15T00:00:00+02:00"
```

#### Regel REZ-016: Cannabis/ Rezeptur – Zählerwerte
```
Schweregrad: ERROR
Bedingung: Alle Zähler müssen "1" sein (eine Herstellung pro Verordnung)
Felder:
  - Zähler Herstellung: "1"
  - Zähler Einheit: "1"
Implementierung:
- Festwert: beide Zähler = "1"
- Nur eine Rezeptur pro Verordnung zulässig
```

#### Regel REZ-017: Cannabis/ Rezeptur – Faktorkennzeichen
```
Schweregrad: ERROR
Bedingung: Faktorkennzeichen muss "11" sein
Feld: Faktorkennzeichen
Implementierung:
- Festwert: "11"
- Referenz: TA3 Tabelle 8.2.25
```

#### Regel REZ-018: Cannabis/ Rezeptur – Faktor als Promilleanteil
```
Schweregrad: ERROR
Bedingung: Faktor als Promillewert
Feld: Faktor
Implementierung:
- Ganze Packung = 1000.000000
- Teilpackung proportional
- Sonderkennzeichen (1.1.1–1.2.2, 1.3.1, 1.3.2, 1.6.5, 1.10.2, 1.10.3) = 1.000000
Beispiel:
  - Ganze 50g‑Packung = "1000.000000"
  - 2g aus 50g = (2/50) × 1000 = "40.000000"
```

#### Regel REZ-019: Cannabis/ Rezeptur – Preiskennzeichen
```
Schweregrad: ERROR
Bedingung: Preiskennzeichen abhängig von der Rezepturart
Feld: Preiskennzeichen
Referenz: TA3 Tabelle 8.2.26
Implementierung:
  Anlage‑10‑Zubereitungen:
    - "14" = Preis nach AMPreisV §§ 4,5 (inkl. Fest-/Prozentschlägen)
    - "14" = falls tatsächlicher Apothekeneinkaufspreis gilt

  Allgemeine Rezeptur (06460702, 09999011):
    Stoffe/Behältnisse in Anlage 1/2:
      - "14" = Preis nach Anlage 1/2 + prozentuale Zuschläge §§ 4,5 Abs.1 Nr.1 AMPreisV
    Stoffe/Behältnisse NICHT in Anlage 1/2:
      - "13" = tatsächlicher Apothekeneinkaufspreis + prozentuale Zuschläge
    Teilmengen von Fertigarzneimitteln:
      - "11" = Apothekeneinkaufspreis nach AMPreisV + prozentuale Zuschläge
```

#### Regel REZ-020: Cannabis/ Rezeptur – Preisanpassung bei großen Mengen
```
Schweregrad: WARNING
Bedingung: Preisanpassung bei Herstellmengen über Basismenge
Referenz: AMPreisV § 5 Abs. 3
Implementierung:
- Basismenge gemäß AMPreisV (z. B. 300g)
- Menge ≤ Basismenge: Faktor = 1000.000000, Preis = Basispreis
- Menge > Basismenge und ≤ 2× Basismenge: Faktor = 1500.000000, Preis = 1,5× Basispreis
Beispiel:
  - Basis: 300g = 6€ → Faktor "1000.000000", Preis "6.00"
  - 100g ≤ 300g → Faktor "1000.000000", Preis "6.00"
  - 500g (> 300g, ≤ 600g) → Faktor "1500.000000", Preis "9.00"
```

#### Regel REZ-021: Validierung der Zusatzdatenpflicht
```
Schweregrad: ERROR
Bedingung: SOK verlangt Zusatzdaten gemäß Zusatzdaten‑Feldspezifikation
Felder: Sonderkennzeichen, Zusatzdatenstruktur
Referenz: CODE_STRUCTURES.md Abschnitt 3.6 (SOK1‑Validierungsregeln), Abschnitt 6.3
Implementierung:
- SOK.zusatzdaten aus Referenztabelle ermitteln
- Wenn zusatzdaten > 0: Zusatzdaten sind ERFORDERLICH
- Vollständigkeit abhängig vom zusatzdaten‑Wert prüfen:
  - 0: Keine Zusatzdaten erforderlich
  - 1: Zusammensetzungsdaten erforderlich (z. B. Zutatenliste)
  - 2: Faktor/Preis‑Zusatzdaten erforderlich (z. B. Behältnis, Stoffdetails)
  - 3: Substitutionsdosen‑Daten erforderlich (dosis_mg, Substanz, Applikationsart)
  - 4: Gebühren-/Leistungsdaten erforderlich (Gebührenbetrag, Begründung)
- Fehler, wenn erforderliche Daten fehlen oder unvollständig sind
Fehlermeldungen:
- "SOK {code} erfordert Zusatzdaten (Zusatzdaten={value}). Zusatzdaten fehlen oder sind unvollständig."
- "SOK {code} erfordert Zusammensetzungsdaten. Zutatenliste fehlt."
- "SOK {code} erfordert Substitutionsdosis‑Daten. Dosis, Substanz und Applikationsart erforderlich."
Beispiele:
  SOK 09999011 (Rezeptur): zusatzdaten=1 → Zusammensetzung erforderlich
  SOK 09999086 (Methadon): zusatzdaten=3 → Dosisdaten erforderlich
  SOK 02567001 (BTM‑Gebühr): zusatzdaten=4 → Gebührenbetrag erforderlich
```

---

## 7. Cannabis‑spezifische Validierungen

**Referenz:** TA1 Abschnitt 4.14, Version 039 – vollständige Überarbeitung der Cannabis‑Regelungen

### 7.1 Identifikation von Cannabis‑Zubereitungen

#### Regel CAN-001: Cannabis‑Sonderkennzeichen
```
Schweregrad: ERROR
Bedingung: Cannabis‑Zubereitungen nach § 31 Abs. 6 SGB V
Sonderkennzeichen (Anlage 10):
  - 06461446: Getrocknete Cannabisblüten
  - 06461423: Cannabis‑Extrakte
  - 06460665: Dronabinol‑Zubereitung Typ 1
  - 06460694: Dronabinol‑Zubereitung Typ 2
  - 06460748: Cannabis‑Zubereitung Typ 3
  - 06460754: Cannabis‑Zubereitung Typ 4
Implementierung:
- Cannabis‑Zubereitung anhand Sonderkennzeichen erkennen
- Cannabis‑spezifische Regeln anwenden
- Einhaltung von § 31 Abs. 6 SGB V sicherstellen
```

#### Regel CAN-002: Cannabis – Keine BTM/T‑Rezept‑Substanzen
```
Schweregrad: ERROR
Bedingung: Cannabis‑Zubereitungen dürfen keine BTM‑ oder T‑Rezept‑Substanzen enthalten
Referenz: TA1 Abschnitt 4.14.2 Allgemeine Regeln
Implementierung:
- Alle Inhaltsstoffe auf BTM‑Einstufung prüfen
- Fehler, wenn BTM/T‑Rezept‑Substanz vorhanden
- Separate Abrechnung für BTM‑Cannabis
```

#### Regel CAN-003: Cannabis – Faktorwert im Sonderkennzeichen‑Satz
```
Schweregrad: ERROR
Bedingung: Faktor muss "1" im Sonderkennzeichen‑Satz der Abgabedaten sein
Feld: Faktor in Abgabedaten
Implementierung:
- Haupt‑Sonderkennzeichen‑Satz: Faktor = "1" oder "1.000000"
- Detaillierte Herstellungsdaten: gemäß REZ‑018 berechnen
```

#### Regel CAN-004: Cannabis – Bruttopreisberechnung
```
Schweregrad: ERROR
Bedingung: Bruttopreis = Gesamtbetrag der Abrechnung
Feld: Bruttopreis in Abgabedaten
Implementierung:
- Gesamtbetrag aus allen Zutaten + Arbeit + Zuschlägen berechnen
- Alle anwendbaren Gebühren einbeziehen
- AMPreisV‑Regeln anwenden
- Abgleich mit Anlage‑10‑Preislisten
```

#### Regel CAN-005: Cannabis – Herstellungsdaten erforderlich
```
Schweregrad: ERROR
Bedingung: Alle Cannabis‑Zubereitungen benötigen detaillierte Herstellungsdaten
Felder: Vollständige Herstellungssegment‑Struktur
Implementierung:
- Herstellerkennzeichen (Apotheken‑IK)
- Herstellungszeitstempel (Abgabedatum + 00:00)
- Zähler (alle "1")
- Vollständige Zutatenliste mit PZN, Faktoren, Preisen
- Zuschläge und Gebühren
```

---

## 8. Wirtschaftliche Einzelmengen

### 8.1 Auseinzelung (02567053)

**Referenz:** TA1 Abschnitt 4.11, 4.14.2 b

#### Regel ESQ-001: Auseinzelung – Sonderkennzeichen
```
Schweregrad: ERROR
Sonderkennzeichen: 02567053
Bedingung: Auseinzelung aus größerer Packung
Referenz: Rahmenvertrag § 129 SGB V
Implementierung:
- Sonderkennzeichen 02567053 validieren
- Regeln für Auseinzelung anwenden
- Ursprungs‑PZN der Packung dokumentieren
```

#### Regel ESQ-002: Auseinzelung – Einzelne Einheit
```
Schweregrad: ERROR
Bedingung: Immer exakt eine Einheit
Zähler:
  - Zähler Herstellung: "1"
  - Zähler Einheit: "1"
Implementierung:
- Beide Zähler müssen "1" sein
- Nur ein Abgabevorgang erlaubt
```

### 8.2 Patientenindividuelle Teilmengen (02566993)

**Referenz:** TA1 Abschnitt 4.13, 4.14.2 b

#### Regel ESQ-003: Patientenindividuelle Teilmengen – Sonderkennzeichen
```
Schweregrad: ERROR
Sonderkennzeichen: 02566993
Bedingung: Wochenblister oder ähnliche patientenspezifische Verpackung
Implementierung:
- Sonderkennzeichen 02566993 validieren
- Mehrfach‑Einheiten fortlaufend nummerieren
- Alle Ursprungspackungen dokumentieren
```

#### Regel ESQ-004: Wochenblister – Mehrfache Einheiten
```
Schweregrad: ERROR
Bedingung: Fortlaufende Nummerierung bei mehreren Einheiten
Zähler:
  - Zähler Herstellung: fortlaufend ab "1" je Batch
  - Zähler Einheit: fortlaufend ab "1" je Batch
Implementierung:
- Herstellungszähler steigt je Batch
- Einheitenzähler steigt je Blister im Batch
- Lückenlose Sequenz
Beispiel:
  - Batch 1, Blister 1: Herstellung=1, Einheit=1
  - Batch 1, Blister 2: Herstellung=1, Einheit=2
  - Batch 1, Blister 3: Herstellung=1, Einheit=3
  - Batch 2, Blister 1: Herstellung=2, Einheit=1
```

---

## 9. Validierungen von Sonderfällen

### 9.1 § 3 Abs. 4‑Verordnungen

**Referenz:** TA1 Abschnitt 4.5.2, Seite 11

#### Regel SPC-001: Behandlung von Niedrigpreis‑Arzneimitteln
```
Schweregrad: ERROR
Bedingung: Bruttopreis ≤ Zuzahlungsbetrag
Felder:
  - Bruttopreis (ID 23): Apothekenabgabepreis
  - Zuzahlung (ID 27, gesteuert durch ID26=0): Zuzahlungsbetrag
Implementierung:
- Prüfen: Bruttopreis <= Zuzahlungsbetrag
- Beide Felder korrekt befüllen
- Bei Mehrkosten: separates Mehrkostenfeld (ID 27, ID26=1)
- In GesamtBrutto (ID 7) und GesamtZuzahlung (ID 6) einbeziehen
```

#### Regel SPC-002: Mehrkosten bei § 3 Abs. 4
```
Schweregrad: WARNING
Bedingung: Patient zahlt Mehrkosten über die Zuzahlung hinaus
Felder:
  - Bruttopreis (ID 23): Apothekenabgabepreis
  - Zuzahlung (ID 27, ID26=0): Zuzahlung
  - Mehrkosten (ID 27, ID26=1): Mehrkosten
Implementierung:
- Alle drei Felder erforderlich, wenn Mehrkosten > 0
- In Summen (GesamtBrutto, GesamtZuzahlung) einbeziehen
```

### 9.2 Verordnungen zur künstlichen Befruchtung

**Referenz:** TA1 Abschnitt 4.9.2, Seiten 14–15

#### Regel SPC-003: Kennzeichen künstliche Befruchtung
```
Schweregrad: ERROR
Bedingung: Verordnung ist für künstliche Befruchtung gekennzeichnet
Feld: Zuzahlungsstatus
Implementierung:
- Kennzeichen in E‑Rezept prüfen
- Spezielle Abrechnungsregeln anwenden
```

#### Regel SPC-004: 50%‑Eigenbeteiligung
```
Schweregrad: ERROR
Bedingung: Patient zahlt 50% des Apothekenabgabepreises oder 50% des Festbetrags
Feld: Kostenbetrag Kategorie "2"
Implementierung:
- Wenn AVK ≤ Festbetrag: Beitrag = 50% × AVK
- Wenn AVK > Festbetrag: Beitrag = 50% × Festbetrag
- Mehrkosten (AVK - Festbetrag) in Kategorie "1"
- Zuzahlung in Kategorie "0" = "0.00"
Formel:
  PatientContribution = min(AVK, Festbetrag) × 0.5
  Mehrkosten = max(0, AVK - Festbetrag)
```

#### Regel SPC-005: Künstliche Befruchtung – Rezeptur
```
Schweregrad: ERROR
Bedingung: Rezeptur oder wirtschaftliche Einzelmengen bei künstlicher Befruchtung
Sonderkennzeichen: siehe REZ‑013 + 09999643
Implementierung:
- Standard‑Rezepturregeln anwenden (Abschnitt 4.14.2 b/c)
- Zusätzlich Sonderkennzeichen 09999643 erforderlich
- 50%‑Preisberechnung anwenden
```

### 9.3 Abweichung von der Standardabgabe

**Referenz:** TA1 Abschnitt 4.10, Seiten 16–19

#### Regel SPC-006: Abweichungs‑Sonderkennzeichen
```
Schweregrad: ERROR
Sonderkennzeichen: 02567024
Bedingung: Abweichung von der Standardabgabe nach § 129 SGB V
Feld: Faktor (3‑stelliger Code für Begründung)
Implementierung:
- Position 1: Erstes Arzneimittel
- Position 2: Zweites Arzneimittel
- Position 3: Drittes Arzneimittel
Werte je Position:
  "1" = Standardabgabe nach § 129 oder Leerzeile
  "2" = Nichtverfügbarkeit Rabattarzneimittel (alle Auswahlbereiche)
  "3" = Nichtverfügbarkeit preisgünstiges Arzneimittel (Generikamarkt)
  "4" = Rabatt + preisgünstig nicht verfügbar
  "5" = Notfall (dringender Fall)
  "6" = Notfall + Nichtverfügbarkeit Kombination
  "7" = Wunscharzneimittel
  "8" = Pharmazeutische Bedenken nach § 17 Abs. 5 S. 2 ApBetrO
  "9" = Bedenken gegen Rabatt und preisgünstig
Beispiel:
  Faktor "243" = Med1: Rabatt nicht verfügbar (2), Med2: preisgünstig nicht verfügbar (4), Med3: preisgünstig nicht verfügbar (3)
```

### 9.4 Institutionskennzeichen (IK)

**Referenz:** TA1 Abschnitt 4.6.2, Seite 12

#### Regel SPC-007: IK‑Format für E‑Rezept
```
Schweregrad: ERROR
Bedingung: Vollständiges 9‑stelliges IK erforderlich
Feld: Institutionskennzeichen
Implementierung:
- Muss exakt 9 Ziffern haben
- Für öffentliche Apotheken: enthält Klassifikationscode "30"
- Für sonstige Leistungserbringer: vollständiges 9‑stelliges IK
Format: ^[0-9]{9}$
```

#### Regel SPC-008: Vertragsbezogene SOK‑Autorisierung
```
Schweregrad: ERROR
Bedingung: Apotheke muss zur Nutzung vertragsbezogener SOK berechtigt sein
Felder: Sonderkennzeichen, Apothekenverband
Referenz: CODE_STRUCTURES.md Abschnitt 6.2
Implementierung:
- Wenn SOK im SOK2‑Bereich (vertragsbezogen)
- SOK.assigned_to aus Referenztabelle ermitteln
- Prüfen, ob Apothekenverband autorisiert ist
- Fehler, wenn Apotheke nicht in der autorisierten Liste ist
Fehlermeldung:
- "SOK {code} ist ein vertragsbezogenes Sonderkennzeichen für {assigned_to}. Apothekenverband {pharmacy_assoc} ist nicht berechtigt."
Beispiel:
  SOK 06460501 (AOK BW‑Vertrag) zugeordnet zu "LAV Baden‑Württemberg"
  Apothekenverband "LAV Bayern" → ERROR
Hinweis:
- SOK1 (Standard): für alle Apotheken verfügbar
- SOK2 (vertragsbezogen): auf bestimmte Verbände/Verträge beschränkt
```

---

## 10. Preis‑ und Faktorberechnungen

### 10.1 Promilleanteil‑Berechnungen

**Referenz:** TA1 Abschnitte 4.14.1 a/b/c/d, 4.14.2 a/b/c

#### Regel CALC-001: Standard‑Promilleanteil‑Formel
```
Schweregrad: ERROR
Bedingung: Faktor als Promilleanteil
Formel:
  Faktor = (Abgegebene_Menge / Packungsmenge) × 1000
Beispiele:
  - 1 volle Packung: (1/1) × 1000 = 1000.000000
  - 3 volle Packungen: (3/1) × 1000 = 3000.000000
  - 7 Tabletten aus 28: (7/28) × 1000 = 250.000000
  - 2g aus 50g: (2/50) × 1000 = 40.000000
  - 1 Einheit aus 10: (1/10) × 1000 = 100.000000
Implementierung:
- Präzisen Dezimalwert berechnen
- Bis zu 6 Nachkommastellen
- Nachkommennullen dürfen entfallen (siehe FMT‑008)
```

#### Regel CALC-002: Ausnahmefaktor für Sonderkennzeichen
```
Schweregrad: ERROR
Bedingung: Sonderkennzeichen ohne Mengenbezug verwenden immer Faktor 1.000000
Sonderkennzeichen: 1.1.1–1.2.2, 1.3.1, 1.3.2, 1.6.5, 1.10.2, 1.10.3
Implementierung:
- Faktor fest auf 1.000000 setzen (oder 1.0, 1)
- Begründung: Keine eindeutige Mengenreferenz
```

#### Regel CALC-003: Faktor für künstliche Befruchtung
```
Schweregrad: ERROR
Sonderkennzeichen: 09999643 (Marker künstliche Befruchtung)
Bedingung: Faktor immer 1000.000000, Preis immer 0.00
Implementierung:
- Faktor = "1000.000000"
- Preis = "0.00" oder ",00"
- Preiskennzeichen = "90"
```

### 10.2 Preisberechnungen

#### Regel CALC-004: Grundpreisberechnung
```
Schweregrad: ERROR
Bedingung: Preis aus Faktor und Preiskennzeichen abgeleitet
Formel:
  Wenn Faktor mengenbezogen:
    Preis = (Faktor / 1000) × Basispreis je Preiskennzeichen
  Wenn Faktor = 1.000000 bei Sonderkennzeichen:
    Preis = Tatsächlicher Betrag für abgegebene Menge
Implementierung:
- Basispreis anhand Preiskennzeichen (TA3 8.2.26) ermitteln
- Faktor anwenden
- Auf 2 Dezimalstellen runden (EUR)
Beispiele:
  - Basispreis 100€, Faktor 1000.000000 → 100.00€
  - Basispreis 100€, Faktor 250.000000 → 25.00€
  - Basispreis 100€, Faktor 3000.000000 → 300.00€
```

#### Regel CALC-005: USt‑Ausschluss im Preisfeld
```
Schweregrad: ERROR
Bedingung: Preise in ZDP/Herstellungsdaten ohne USt.
Feld: Preis (ZDP‑06)
Implementierung:
- Alle Preise in Herstellungsdaten ohne USt.
- USt. wird später in der Abrechnung hinzugefügt
- Preis muss Netto‑Betrag sein (ohne USt.)
```

#### Regel CALC-006: Preiskennzeichen‑Lookup
```
Schweregrad: ERROR
Feld: Preiskennzeichen (TA3 Tabelle 8.2.26)
Häufige Codes:
  "11" = Apothekeneinkaufspreis nach AMPreisV
  "13" = Tatsächlicher Apothekeneinkaufspreis
  "14" = Abrechnungspreis nach AMPreisV §§ 4,5 (inkl. Zuschläge)
  "15" = Vertraglich vereinbarter Abrechnungspreis
  "90" = Sonderpreis (z. B. Marker künstliche Befruchtung = 0.00)
Implementierung:
- Abgleich mit TA3 Tabelle 8.2.26
- Code muss existieren
- Entsprechende Preisregel anwenden
```

### 10.3 Dezimalstellen‑Behandlung

**Referenz:** TA1 Abschnitt 4.14.2, Seite 38 (NEU in Version 039)

#### Regel CALC-007: Flexible Nachkommennullen
```
Schweregrad: INFO
Bedingung: Anzahl der Nachkommennullen in Faktoren ist unerheblich
Feld: Faktor
Beispiele (alle gleichwertig):
  - "1"
  - "1.0"
  - "1.000000"
  - "1000"
  - "1000.0"
  - "1000.000000"
Implementierung:
- Jede Darstellung innerhalb der maximalen Dezimalstellen akzeptieren
- Intern normalisieren
- FHIR kann Dezimalwerte ohne Nachkommennullen liefern
- Maximal 6 Nachkommastellen weiterhin erzwingen
```

---

## 11. Prioritätsmatrix der Validierungsregeln

### 11.1 Schweregrade

| Schweregrad | Beschreibung | Aktion |
|------------|--------------|--------|
| **ERROR** | Kritischer Validierungsfehler, Verordnung nicht abrechenbar | Übermittlung blockieren, Korrektur erforderlich |
| **WARNING** | Möglicher Fehler, kann zu Ablehnung führen | Übermittlung zulassen, Warnung anzeigen |
| **INFO** | Informative Meldung, Best Practice | Keine Blockierung, nur Hinweis |

### 11.2 Ausführungsreihenfolge

1. **Phase 1: Format‑Validierungen** (FMT‑xxx)
   - Zuerst ausführen, bei fehlerhaftem Format sofort abbrechen
   - PZN‑Format, Zeitstempel‑Format, numerische Formate
   - Bei ERROR: Verarbeitung stoppen

2. **Phase 2: Allgemeine Regeln** (GEN‑xxx)
   - Zeitzone, Bruttopreis‑Zusammensetzung, Sonderkennzeichen‑Ort
   - Bei ERROR: Verarbeitung stoppen

3. **Phase 3: Verordnungstyp erkennen**
   - Erkennen: BTM, Cannabis, Rezeptur, Standard, Sonderfälle
   - An passende Spezial‑Validatoren routen

4. **Phase 4: Typ‑spezifische Validierungen**
   - BTM (BTM‑xxx)
   - Cannabis (CAN‑xxx)
   - Rezeptur (REZ‑xxx)
   - Wirtschaftliche Einzelmengen (ESQ‑xxx)
   - Sonderfälle (SPC‑xxx)
   - Bei ERROR: Fehler sammeln

5. **Phase 5: Berechnungen** (CALC‑xxx)
   - Preis‑ und Faktorberechnungen
   - Feldübergreifende Validierungen
   - Summenprüfung

6. **Phase 6: Gebührenvalidierungen** (FEE‑xxx)
   - Gesetzliche Gebühren (BTM, Noctu, Botendienst)
   - USt‑Berechnungen
   - WARNUNG sammeln

### 11.3 Kritische Pfadregeln (müssen bestehen)

Folgende Regeln sind **kritisch** und müssen bestanden werden:

- **GEN-001**: Deutsche Zeitzone (alle Zeitstempel)
- **GEN-003**: Bruttopreis‑Zusammensetzung
- **FMT-001**: PZN‑Format
- **FMT-003**: ISO‑8601‑Zeitstempel‑Format
- **FMT-008**: Faktorformat
- **FMT-010**: Preisformat
- **REZ-003**: Herstellungszeitstempel ≤ Signaturzeitstempel
- **REZ-004**: Zählersequenz
- **REZ-015**: Cannabis‑Herstellungszeitstempel (falls Cannabis)
- **CAN-002**: Keine BTM‑Substanzen in Cannabis (falls Cannabis)
- **CALC-001**: Promilleanteil‑Berechnung

---

## 12. Fehlercodes und Meldungen

### 12.1 Fehlercode‑Struktur

Format: `[Kategorie]-[Nummer]-[Schweregrad]`

Beispiele:
- `FMT-001-E`: Formatvalidierungsfehler #001
- `BTM-003-W`: BTM‑Warnung #003
- `CAN-004-E`: Cannabis‑Fehler #004

### 12.2 Standard‑Antwortformat

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
      "message": "PZN muss genau 8 Stellen haben. Gefunden: 6 Stellen.",
      "suggestion": "PZN mit führenden Nullen auffüllen: '00123456'",
      "reference": "TA1 Abschnitt 4.14.2, Seite 39"
    }
  ],
  "warnings": [
    {
      "code": "BTM-003-W",
      "severity": "WARNING",
      "field": "Abgabedatum",
      "message": "BTM‑Verordnung 9 Tage nach Verordnungsdatum abgegeben. Maximale Gültigkeit 7 Tage gemäß BtMG §3.",
      "reference": "BtMG §3"
    }
  ],
  "info": []
}
```

### 12.3 Häufige Fehlermeldungen

#### Formatfehler (FMT‑xxx)

```
FMT-001-E: "PZN muss genau 8 Stellen haben. Gefunden: {actual_length} Stellen."
FMT-003-E: "Herstellungszeitstempel muss ISO‑8601‑konform sein. Gefunden: '{actual_value}'."
FMT-008-E: "Faktor überschreitet maximale Dezimalstellen. Max. 6 vor und 6 nach dem Dezimalpunkt. Gefunden: '{actual_value}'."
FMT-010-E: "Preis überschreitet maximale Dezimalstellen. Max. 9 vor und 2 nach dem Dezimalpunkt. Gefunden: '{actual_value}'."
```

#### Allgemeine Fehler (GEN‑xxx)

```
GEN-001-E: "Zeitstempel muss in deutscher Zeit (CET/CEST) sein. Gefundene Zeitzone: '{actual_timezone}'."
GEN-003-E: "Bruttopreis darf keine Zuzahlung abziehen. Erwartet: Apothekenabgabepreis nach AMPreisV."
GEN-004-W: "Gesetzliche Gebühr nicht korrekt USt‑bereinigt. Erwartet: {expected}, gefunden: {actual}."
GEN-006-E: "SOK {code} ist am {valid_until} abgelaufen. Abgabedatum {dispensing_date} liegt außerhalb der Gültigkeit."
GEN-006-E: "SOK {code} ist noch nicht gültig. Gültig ab {valid_from}, Abgabedatum {dispensing_date}."
GEN-007-E: "SOK {code} ({description}) ist nicht E‑Rezept‑kompatibel. E‑Rezept‑Flag muss false sein oder Papierverordnung verwenden."
GEN-008-E: "USt‑Satz stimmt nicht für SOK {code} ({description}). Erwartet: {expected}%, gefunden: {actual}%."
```

#### BTM‑Fehler (BTM‑xxx)

```
BTM-001-E: "E‑BTM‑Verordnung ohne BTM‑Gebühr‑Sonderkennzeichen (02567001)."
BTM-002-E: "BTM‑Zeile ohne Pflichtfelder (PZN, Menge, Preis)."
BTM-003-W: "BTM‑Verordnung {days} Tage nach Verordnungsdatum abgegeben. Maximal 7 Tage gemäß BtMG §3."
BTM-004-W: "BTM‑Verordnung ohne ICD‑10‑Diagnosecode (erforderlich gemäß BtMG §3)."
```

#### Rezeptur‑Fehler (REZ‑xxx)

```
REZ-003-E: "Herstellungszeitstempel ({manufacturing_ts}) darf nicht nach dem Signaturzeitstempel ({signature_ts}) liegen."
REZ-004-E: "Zählersequenz enthält eine Lücke. Erwartet: {expected}, gefunden: {actual}."
REZ-005-E: "Faktor muss als Promilleanteil angegeben werden (z. B. 1 Packung = 1000.000000)."
REZ-015-E: "Cannabis‑Herstellungszeitstempel muss Abgabedatum + 00:00 sein. Erwartet: '{expected}', gefunden: '{actual}'."
REZ-016-E: "Cannabis‑Zähler müssen alle '1' sein. Gefunden: Herstellung={h}, Einheit={e}."
REZ-021-E: "SOK {code} erfordert Zusatzdaten (Zusatzdaten={value}). Zusatzdaten fehlen oder sind unvollständig."
REZ-021-E: "SOK {code} erfordert Zusammensetzungsdaten. Zutatenliste fehlt."
REZ-021-E: "SOK {code} erfordert Substitutionsdosis‑Daten. Dosis, Substanz und Applikationsart erforderlich."
```

#### Cannabis‑Fehler (CAN‑xxx)

```
CAN-001-E: "Ungültiges Cannabis‑Sonderkennzeichen. Erwartet: 06461446, 06461423, 06460665, 06460694, 06460748, 06460754. Gefunden: '{actual}'."
CAN-002-E: "Cannabis‑Zubereitung enthält BTM‑ oder T‑Rezept‑Substanzen. Das ist gemäß TA1 Abschnitt 4.14.2 nicht zulässig."
CAN-003-E: "Cannabis‑Sonderkennzeichen‑Satz muss Faktor = '1' haben. Gefunden: '{actual}'."
CAN-005-E: "Cannabis‑Zubereitung ohne erforderliche Herstellungsdaten (Herstellungssegment)."
```

#### Berechnungsfehler (CALC‑xxx)

```
CALC-001-E: "Faktorberechnung (Promilleanteil) falsch. Erwartet: {expected}, gefunden: {actual}."
CALC-004-E: "Preisberechnung falsch. Formel: (Faktor / 1000) × Basispreis. Erwartet: {expected}, gefunden: {actual}."
CALC-006-E: "Ungültiges Preiskennzeichen. Code '{actual}' nicht in TA3 Tabelle 8.2.26 gefunden."
```

#### Sonderfall‑Fehler (SPC‑xxx)

```
SPC-001-E: "Bei § 3 Abs. 4‑Verordnungen muss Bruttopreis ≤ Zuzahlung sein. Gefunden: Bruttopreis={bruttopreis}, Zuzahlung={zuzahlung}."
SPC-004-E: "Berechnung der Eigenbeteiligung bei künstlicher Befruchtung falsch. Erwartet 50% von min(AVK, Festbetrag). Erwartet: {expected}, gefunden: {actual}."
SPC-006-E: "Abweichungs‑Sonderkennzeichen (02567024) hat ungültigen Faktorcode. Erwartet 3‑stelligen Code mit Werten 1‑9. Gefunden: '{actual}'."
SPC-007-E: "Institutionskennzeichen (IK) muss genau 9‑stellig sein. Gefunden: {actual_length} Stellen."
SPC-008-E: "SOK {code} ist vertragsbezogen und zu {assigned_to} zugeordnet. Apothekenverband {pharmacy_assoc} ist nicht berechtigt."
```

### 12.4 Korrekturvorschläge

Fügen Sie zu Fehlern konkrete Korrekturhinweise hinzu:

```json
{
  "code": "FMT-001-E",
  "suggestion": "PZN mit führenden Nullen auffüllen. Beispiel: '123456' → '00123456'"
}
```

```json
{
  "code": "REZ-003-E",
  "suggestion": "Herstellungszeitstempel auf den Signaturzeitstempel oder früher setzen."
}
```

```json
{
  "code": "CAN-002-E",
  "suggestion": "BTM‑Substanzen aus der Cannabis‑Zubereitung entfernen oder separaten BTM‑Abrechnungsprozess verwenden."
}
```

---

## 13. Implementierungsrichtlinien

### 13.1 Validierungsarchitektur

Empfohlene Validator‑Architektur:

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
│   ├── SokTemporalValidator (GEN-006)
│   ├── SokErezeptCompatibilityValidator (GEN-007)
│   ├── SokVatConsistencyValidator (GEN-008)
│   └── ...
├── PrescriptionTypeDetector (Phase 3)
│   ├── DetectBTM()
│   ├── DetectCannabis()
│   ├── DetectCompounding()
│   └── DetectSpecialCase()
├── SpecializedValidators (Phase 4)
│   ├── BtmValidator (BTM-xxx)
│   ├── CannabisValidator (CAN-xxx)
│   ├── CompoundingValidator (REZ-xxx, REZ-021)
│   ├── EconomicSingleQuantityValidator (ESQ-xxx)
│   ├── SokAuthorizationValidator (SPC-008)
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

### 13.2 Externe Datenabhängigkeiten

Der Validator benötigt Zugriff auf:

1. **Code‑Referenzdaten** (aus [CODE_STRUCTURES.md](./Abrechnung/CODE_STRUCTURES.md))
   - Faktorkennzeichen (4 Codes): Abschnitt 1
   - Preiskennzeichen (8 Codes): Abschnitt 2
   - SOK1‑Codes (172 Codes): Abschnitt 3
   - SOK2‑Codes (109 Codes): Abschnitt 4
   - Codeübergreifende Validierungsregeln: Abschnitt 5

2. **TA3‑Codetabellen**
   - Tabelle 8.2.25: Faktorkennzeichen
   - Tabelle 8.2.26: Preiskennzeichen
   - Weitere relevante Codetabellen

3. **ABDA‑Datenbank**
   - PZN‑Validierung und Lookup
   - Zulassungsstatus von Arzneimitteln
   - Aktuelle Preise (für Berechnungsvalidierung)

4. **Anlagen‑Tabellen**
   - Anhang 1: Bundesweite Sonderkennzeichen
   - Anhang 2: Krankenkassen‑Apotheken‑Vertragskennzeichen
   - Hilfstaxe Anlagen 1, 2, 4, 5, 6, 7, 10

5. **Lauer‑Taxe API** (optional)
   - Echtzeit‑PZN‑Validierung
   - Aktuelle Arzneimittelinformationen
   - Preisdaten

**Hinweis:** Das Dokument CODE_STRUCTURES.md dient als primäre Referenz für SOK‑Codes und sollte mit offiziellen TA1‑Anlagen aktuell gehalten werden.

### 13.3 Performance‑Überlegungen

- **Validierungsgeschwindigkeit Ziel:** < 500ms je E‑Rezept (laut Produkt‑Brief)
- **Caching‑Strategie:**
  - TA3‑Codetabellen cachen (täglich aktualisieren)
  - ABDA‑Daten cachen (stündlich aktualisieren)
  - Sonderkennzeichenlisten cachen (bei TA‑Version‑Update)
- **Fail‑Fast‑Prinzip:**
  - Formatvalidierungen zuerst ausführen
  - Bei kritischen Fehlern abbrechen (z. B. ungültige PZN)
- **Parallelisierung:**
  - Unabhängige Validatoren parallel ausführen
  - Ergebnisse am Ende aggregieren

### 13.4 Teststrategie

#### Unit‑Tests
- Jede Regel einzeln testen
- TA1‑Beispiele als Testfälle verwenden
- Alle Fehlerpfade abdecken

#### Integrationstests
- Vollständige Verordnungsflüsse testen
- BTM‑, Cannabis‑, Rezeptur‑Szenarien testen
- Sonderfälle testen (künstliche Befruchtung usw.)

#### Regressionstests
- Test‑Suite je TA‑Version pflegen
- Rückwärtskompatibilität sicherstellen
- TA‑Versionsübergänge testen

#### Testdaten
- Anonymisierte echte E‑Rezept‑Daten verwenden
- Synthetische Tests für Grenzfälle erstellen
- Alle Sonderkennzeichenkombinationen abdecken
- Beispiele aus [VALIDATION_EXAMPLES.md](./Abrechnung/VALIDATION_EXAMPLES.md) verwenden

---

## 14. Versionshistorie und Change‑Management

### 14.1 TA1‑Versionsverfolgung

| TA1‑Version | Gültig ab | Wesentliche Änderungen | Auswirkung auf Validator |
|------------|-----------|------------------------|--------------------------|
| 039 | 2025-10-01 | Cannabis‑Regeln (§4.14), Dezimalstellen (§4.14.2), BTM‑Gebühren (§5) | HOCH – größere Regeländerungen |
| 038 | 2022-11-16 | Mehrkosten‑Klarstellung (§4.5.2), Anlagen ausgelagert | MITTEL – Regelpräzisierungen |
| 037 | 2022-06-27 | Künstliche Befruchtung (§4.9), Cannabis‑Ergänzungen (§4.14.1) | HOCH – neue Sonderfälle |

### 14.2 Validator‑Versionsroadmap

**Version 1.0 (MVP)**
- Kern‑Formatvalidierungen (FMT‑xxx)
- Allgemeine Regeln (GEN‑xxx)
- Grundlegende BTM‑Validierung (BTM‑001, BTM‑002)
- Standard‑Rezeptur (REZ‑001 bis REZ‑006)

**Version 1.1**
- Vollständige BTM‑Validierung (BTM‑003, BTM‑004)
- Cannabis‑Validierungen (CAN‑xxx)
- Wirtschaftliche Einzelmengen (ESQ‑xxx)
- Erweiterte Rezeptur (REZ‑007 bis REZ‑020)

**Version 1.2**
- Sonderfall‑Validierungen (SPC‑xxx)
- Gebühren‑Validierungen (FEE‑xxx)
- Vollständige Berechnungsvalidierungen (CALC‑xxx)
- Performance‑Optimierungen

**Version 2.0**
- ABDA‑API‑Integration
- Echtzeit‑PZN‑Lookup
- Lauer‑Taxe‑Integration
- Erweiterte Preisvalidierung

---

## Anhang A: Schnellreferenztabellen

### A.1 Sonderkennzeichen nach Kategorie

**Hinweis:** Dieser Anhang enthält häufig verwendete Sonderkennzeichen. Für den **vollständigen Katalog** von:
- **172 SOK1‑Codes** (standardisierte Sonderkennzeichen)
- **109 SOK2‑Codes** (vertragsbezogene Sonderkennzeichen)

siehe [CODE_STRUCTURES.md](./Abrechnung/CODE_STRUCTURES.md) Abschnitte 3 und 4.

Für **praktische Validierungsbeispiele** siehe [VALIDATION_EXAMPLES.md](./Abrechnung/VALIDATION_EXAMPLES.md).

| Kategorie | Code | Beschreibung |
|----------|------|--------------|
| **BTM‑Gebühren** | 02567001 | BTM‑Gebühr |
| **T‑Rezept‑Gebühr** | 06460688 | T‑Rezept‑Gebühr (Thalidomid‑Gebühr) |
| **Cannabis** | 06461446 | Cannabisblüten (getrocknet) |
| | 06461423 | Cannabis‑Extrakte |
| | 06460665 | Dronabinol‑Zubereitung Typ 1 |
| | 06460694 | Dronabinol‑Zubereitung Typ 2 |
| | 06460748 | Cannabis‑Zubereitung Typ 3 |
| | 06460754 | Cannabis‑Zubereitung Typ 4 |
| **Rezeptur** | 06460702 | Standardrezeptur (§§4,5 AMPreisV) |
| | 09999011 | Alternative Rezeptur |
| **Wirtschaftliche Einzelmengen** | 02567053 | Auseinzelung |
| | 02566993 | Wochenblister (patientenspezifische Teilmengen) |
| **Opioid‑Substitution** | 09999086 | Methadon‑Teilmengen (Anlage 4) |
| | 02567107 | Levomethadon‑Teilmengen (Anlage 5) |
| | 02567113 | Buprenorphin/Subutex‑Einzeldosen |
| | 02567136 | Buprenorphin/Naloxon‑Einzeldosen |
| | 06461506 | Methadon‑Zubereitungen (Anlage 4) |
| | 06461512 | Levomethadon‑Zubereitungen (Anlage 5) |
| **Abweichungen** | 02567024 | Abweichung von der Standardabgabe |
| **Künstl. Befruchtung** | 09999643 | Marker künstliche Befruchtung |
| **Zuschläge** | 06460518 | Allgemeiner Zuschlag |
| **Gebühren/Services** | 02567018 | Noctu‑Gebühr (Nacht‑/Notdienst) |
| | 06461110 | Botendienst |
| **Impfungen** | 17716926 | Grippeschutz‑Impfleistung |
| | 17716955 | Grippe‑Impffremdleistung |
| | 17717400 | COVID‑Impfleistung |

### A.2 Faktorkennzeichen (TA3 8.2.25)

**Referenz:** Für vollständige Definitionen siehe [CODE_STRUCTURES.md](./Abrechnung/CODE_STRUCTURES.md) Abschnitt 1

| Code | Beschreibung |
|------|--------------|
| 11 | Standardfaktor (Promilleanteil) – Anteil in Promille |
| 55 | Dosis in Milligramm (Substitution Take‑Home) |
| 57 | Dosis in Milligramm (Substitution unter Aufsicht) |
| 99 | Packungsanteil in Promille (Entsorgung) |

### A.3 Preiskennzeichen (TA3 8.2.26)

**Referenz:** Für vollständige Definitionen und Steuerstatus siehe [CODE_STRUCTURES.md](./Abrechnung/CODE_STRUCTURES.md) Abschnitt 2

| Code | Beschreibung |
|------|--------------|
| 11 | Apothekeneinkaufspreis nach AMPreisV |
| 12 | Preis zwischen Apotheke und Hersteller vereinbart |
| 13 | Tatsächlicher Apothekeneinkaufspreis |
| 14 | Abrechnungspreis nach AMPreisV §§4,5 (Hilfstaxe – inkl. Zuschläge) |
| 15 | Vertraglich vereinbarter Abrechnungspreis nach § 129 Abs. 5 SGB V |
| 16 | Vertragspreise nach § 129a SGB V |
| 17 | Abrechnungspreis "Preis 2" gemäß mg‑Preisliste |
| 21 | Rabattvertrags‑Abrechnungspreis "Preis 1" nach § 130a Abs. 8c SGB V |
| 90 | Sonderpreis (z. B. 0.00 für Marker) |

### A.4 Validierungsregel‑Schnellübersicht

| Szenario | Wichtige Regeln |
|----------|----------------|
| **Standard‑E‑Rezept** | GEN-001, GEN-003, GEN-006, GEN-007, GEN-008, FMT-001, FMT-003, FMT-008, FMT-010 |
| **BTM‑Verordnung** | BTM-001, BTM-002, BTM-003, BTM-004, GEN-004, GEN-006, GEN-008 |
| **Cannabis‑Zubereitung** | CAN-001 bis CAN-005, REZ-013 bis REZ-021, GEN-006, GEN-008 |
| **Wochenblister** | ESQ-003, ESQ-004, REZ-007 bis REZ-012, REZ-021, GEN-006 |
| **Künstliche Befruchtung** | SPC-003, SPC-004, SPC-005, CALC-003, GEN-006 |
| **Rezeptur** | REZ-001 bis REZ-021, CALC-001, CALC-004, GEN-006, GEN-008 |
| **Vertrags‑SOK** | SPC-008, GEN-006, GEN-007, GEN-008 |
| **SOK mit E‑Rezept** | GEN-007, GEN-006, GEN-008 |

---

## Anhang B: Glossar

| Begriff | Deutsch | Definition |
|--------|--------|------------|
| **AMPreisV** | Arzneimittelpreisverordnung | Preisverordnung für Arzneimittel |
| **ABDA** | Bundesvereinigung Deutscher Apothekerverbände | Dachverband der Apothekerverbände |
| **Abgabedaten** | Abgabedaten | Datenstruktur für E‑Rezept‑Abgaben |
| **BtM** | Betäubungsmittel | Kontrollierte Substanzen/Narkotika |
| **BtMG** | Betäubungsmittelgesetz | Gesetz zu Betäubungsmitteln |
| **Bruttopreis** | Bruttopreis | Apothekenabgabepreis vor Abzug der Zuzahlung |
| **Eigenbeteiligung** | Eigenbeteiligung | Anteil des Patienten (z. B. 50% bei künstlicher Befruchtung) |
| **Faktor** | Faktor | Mengenmultiplikator (oft als Promilleanteil) |
| **Faktorkennzeichen** | Faktorkennzeichen | Code für Faktor‑Typ (TA3 8.2.25) |
| **IK** | Institutionskennzeichen | 9‑stelliges Institutionskennzeichen |
| **Mehrkosten** | Mehrkosten | Kosten über Festbetrag, vom Patienten getragen |
| **Noctu** | Nacht‑/Notdienst | Apothekennotdienst (20:00–06:00) |
| **Preiskennzeichen** | Preiskennzeichen | Code für Preisart (TA3 8.2.26) |
| **Promilleanteil** | Promilleanteil | Faktor als Promille (‰) |
| **PZN** | Pharmazentralnummer | 8‑stellige Arzneimittel‑ID |
| **Rezeptur** | Rezeptur | In der Apotheke hergestellte Zubereitung |
| **Sonderkennzeichen** | Sonderkennzeichen | Code für Gebühren, Services, Zubereitungen |
| **TA1/TA3/TA7** | Technische Anlagen | Technische Anlagen der Abrechnungsvereinbarung |
| **T‑Rezept** | Thalidomid‑Verordnung | Sonderverordnung für Thalidomid‑Arzneimittel |
| **Zuzahlung** | Zuzahlung | Gesetzliche Zuzahlung des Patienten |

---

## Dokumentkontrolle

**Freigabe:**
- [ ] Produktmanager‑Review
- [ ] Technischer Lead‑Review
- [ ] Compliance‑Review
- [ ] ASW Genossenschaft‑Freigabe

**Verteilung:**
- Entwicklungsteam
- QA‑Team
- Compliance‑Team
- ASW Genossenschaft Stakeholder

**Nächster Review‑Termin:** Bei Veröffentlichung von TA1 Version 040

**Kontakt:**
- Technische Fragen: dev-team@erezept-validator.local
- Compliance‑Fragen: compliance@erezept-validator.local

---

*Ende der technischen Spezifikation*# TA1 Validation Rules - Technical Specification

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

### 1.2 Related Documentation

This document is part of a comprehensive validation documentation suite:

- **This Document (TA1-Validation-Rules)**: Validation business logic, rules, and algorithms (HOW to validate)
- **[CODE_STRUCTURES.md](./Abrechnung/CODE_STRUCTURES.md)**: Complete code catalogs and reference data (WHAT to validate against)
  - 172 SOK1 codes (standard special codes)
  - 109 SOK2 codes (contract-specific special codes)
  - Factor and price code definitions
  - Cross-code validation rules
- **[VALIDATION_EXAMPLES.md](./Abrechnung/VALIDATION_EXAMPLES.md)**: Practical validation scenarios (WHAT it looks like)
  - 16 detailed examples with inputs and outputs
  - Pass/fail scenarios
  - Workflow diagrams

### 1.3 Scope

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

### 2.3 Special Codes (Sonderkennzeichen) Validation

**Reference:** TA1 Section 4.14.1, 4.14.2; [CODE_STRUCTURES.md](./Abrechnung/CODE_STRUCTURES.md) Sections 3-4

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

#### Rule GEN-006: SOK Validity Period Check
```
Severity: ERROR
Condition: Special code must be valid at dispensing date
Fields: Sonderkennzeichen, Abgabedatum (dispensing date)
Reference: CODE_STRUCTURES.md Section 6.1
Implementation:
- Retrieve SOK from reference table (SOK1 or SOK2)
- Check: dispensing_date >= SOK.valid_from
- Check: SOK.valid_until IS NULL OR dispensing_date <= SOK.valid_until
- Error if outside validity period
Error Message:
- "SOK {code} expired on {valid_until}. Dispensing date {dispensing_date} is not within validity period."
- "SOK {code} not yet valid. Valid from {valid_from}, dispensing date {dispensing_date}."
Example:
  SOK 17717104 (VAXIGRIP 2022/2023) expired 2024-08-01
  If dispensing date 2025-01-15 → ERROR
```

#### Rule GEN-007: E-Rezept SOK Compatibility
```
Severity: ERROR
Condition: SOK must support E-Rezept if prescription is E-Rezept
Fields: E-Rezept flag, Sonderkennzeichen
Reference: CODE_STRUCTURES.md Section 5.4
Implementation:
- If prescription.is_e_rezept == true
- Retrieve SOK.e_rezept from reference table
- Check: SOK.e_rezept IN [1, 2] (1=compatible, 2=special handling)
- Error if SOK.e_rezept == 0 (not compatible)
Error Message:
- "SOK {code} ({description}) is not compatible with E-Rezept. E-Rezept flag must be false."
Example:
  SOK 09999057 (partial quantity) has e_rezept=0
  Cannot be used with E-Rezept → must use paper prescription
```

#### Rule GEN-008: VAT Rate Consistency
```
Severity: ERROR
Condition: VAT rate must match SOK specification
Fields: VAT rate code, Sonderkennzeichen
Reference: CODE_STRUCTURES.md Section 5.3
Implementation:
- Retrieve SOK.vat_rate from reference table
- Map VAT codes: 0=0% (tax-free), 1=7% (reduced), 2=19% (standard), -=N/A
- Verify: prescription.vat_code == SOK.vat_rate
- Error if mismatch
Error Message:
- "VAT rate mismatch for SOK {code} ({description}). Expected: {expected}%, Found: {actual}%"
Example:
  SOK 02567515 (Granulocytes) specifies vat_rate=0 (tax-free)
  If prescription has vat_code=2 (19%) → ERROR
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

#### Rule REZ-021: Additional Data Requirement Validation
```
Severity: ERROR
Condition: SOK requires additional data per Zusatzdaten field specification
Fields: Sonderkennzeichen, Additional data structure
Reference: CODE_STRUCTURES.md Section 3.6 (SOK1 validation rules), Section 6.3
Implementation:
- Retrieve SOK.zusatzdaten from reference table
- If zusatzdaten > 0: additional data is REQUIRED
- Verify completeness based on zusatzdaten value:
  - 0: No additional data required
  - 1: Composition data required (e.g., ingredient list for compounded preparations)
  - 2: Factor/price supplementary data required (e.g., container, substance details)
  - 3: Opioid substitution dose data required (dose_mg, substance, administration type)
  - 4: Fee/service data required (fee amount, justification)
- Error if required data missing or incomplete
Error Messages:
- "SOK {code} requires additional data (Zusatzdaten={value}). Additional data missing or incomplete."
- "SOK {code} requires composition data. Ingredient list missing."
- "SOK {code} requires opioid dose data. Dose, substance, and administration type required."
Examples:
  SOK 09999011 (compounded preparation): zusatzdaten=1 → composition required
  SOK 09999086 (Methadone): zusatzdaten=3 → dose information required
  SOK 02567001 (BTM fee): zusatzdaten=4 → fee amount required
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

#### Rule SPC-008: Contract-Specific SOK Authorization
```
Severity: ERROR
Condition: Pharmacy must be authorized to use contract-specific SOK codes
Fields: Sonderkennzeichen, Pharmacy association
Reference: CODE_STRUCTURES.md Section 6.2
Implementation:
- If SOK is in SOK2 range (contract-specific codes)
- Retrieve SOK.assigned_to from reference table
- Verify pharmacy association matches authorized organizations
- Error if pharmacy not in authorized list
Error Message:
- "SOK {code} is a contract-specific code assigned to {assigned_to}. Pharmacy association {pharmacy_assoc} is not authorized to use this code."
Example:
  SOK 06460501 (AOK BW contract) assigned to "LAV Baden-Württemberg"
  If pharmacy association is "LAV Bayern" → ERROR
Note:
- SOK1 codes (standard): available to all pharmacies
- SOK2 codes (contract-specific): restricted to specific associations/contracts
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
GEN-006-E: "SOK {code} expired on {valid_until}. Dispensing date {dispensing_date} is not within validity period."
GEN-006-E: "SOK {code} not yet valid. Valid from {valid_from}, dispensing date {dispensing_date}."
GEN-007-E: "SOK {code} ({description}) is not compatible with E-Rezept. E-Rezept flag must be false or use paper prescription."
GEN-008-E: "VAT rate mismatch for SOK {code} ({description}). Expected: {expected}%, Found: {actual}%."
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
REZ-021-E: "SOK {code} requires additional data (Zusatzdaten={value}). Additional data missing or incomplete."
REZ-021-E: "SOK {code} requires composition data. Ingredient list missing."
REZ-021-E: "SOK {code} requires opioid dose data. Dose, substance, and administration type required."
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
SPC-008-E: "SOK {code} is a contract-specific code assigned to {assigned_to}. Pharmacy association {pharmacy_assoc} is not authorized to use this code."
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
│   ├── SokTemporalValidator (GEN-006)
│   ├── SokErezeptCompatibilityValidator (GEN-007)
│   ├── SokVatConsistencyValidator (GEN-008)
│   └── ...
├── PrescriptionTypeDetector (Phase 3)
│   ├── DetectBTM()
│   ├── DetectCannabis()
│   ├── DetectCompounding()
│   └── DetectSpecialCase()
├── SpecializedValidators (Phase 4)
│   ├── BtmValidator (BTM-xxx)
│   ├── CannabisValidator (CAN-xxx)
│   ├── CompoundingValidator (REZ-xxx, REZ-021)
│   ├── EconomicSingleQuantityValidator (ESQ-xxx)
│   ├── SokAuthorizationValidator (SPC-008)
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

1. **Code Reference Data** (from [CODE_STRUCTURES.md](./Abrechnung/CODE_STRUCTURES.md))
   - Factor codes (4 codes): Section 1
   - Price codes (8 codes): Section 2
   - SOK1 codes (172 codes): Section 3
   - SOK2 codes (109 codes): Section 4
   - Cross-code validation rules: Section 5

2. **TA3 Code Tables**
   - Table 8.2.25: Faktorkennzeichen (Factor identifiers)
   - Table 8.2.26: Preiskennzeichen (Price identifiers)
   - Other relevant code tables

3. **ABDA Database**
   - PZN validation and lookup
   - Drug authorization status
   - Current prices (for calculation validation)

4. **Annex Tables**
   - Anhang 1: Federal special codes (Sonderkennzeichen)
   - Anhang 2: Insurance-pharmacy contracted special codes
   - Hilfstaxe Annexes 1, 2, 4, 5, 6, 7, 10

5. **Lauer-Taxe API** (optional)
   - Real-time PZN validation
   - Current drug information
   - Pricing data

**Note:** The CODE_STRUCTURES.md document serves as the primary reference for SOK codes and should be kept updated with official TA1 annexes.

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
- Use examples from [VALIDATION_EXAMPLES.md](./Abrechnung/VALIDATION_EXAMPLES.md) as test scenarios

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

**NOTE:** This appendix contains frequently used special codes. For the **complete catalog** of:
- **172 SOK1 codes** (standard special codes)
- **109 SOK2 codes** (contract-specific special codes)

See [CODE_STRUCTURES.md](./Abrechnung/CODE_STRUCTURES.md) Sections 3 and 4.

For **practical validation examples** using these codes, see [VALIDATION_EXAMPLES.md](./Abrechnung/VALIDATION_EXAMPLES.md).

| Category | Code | Description |
|----------|------|-------------|
| **BTM Fees** | 02567001 | BTM-Gebühr (Controlled substance fee) |
| **T-Rezept Fee** | 06460688 | T-Rezept-Gebühr (Thalidomide prescription fee) |
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
| **Opioid Subst.** | 09999086 | Methadone partial quantities (Anlage 4) |
| | 02567107 | Levomethadone partial quantities (Anlage 5) |
| | 02567113 | Buprenorphine/Subutex single doses |
| | 02567136 | Buprenorphine/Naloxone single doses |
| | 06461506 | Methadone preparations (Anlage 4) |
| | 06461512 | Levomethadone preparations (Anlage 5) |
| **Deviations** | 02567024 | Deviation from standard dispensing |
| **Art. Insem.** | 09999643 | Artificial insemination marker |
| **Surcharges** | 06460518 | General surcharge |
| **Fees/Services** | 02567018 | Noctu fee (night service) |
| | 06461110 | Botendienst (delivery service) |
| **Vaccinations** | 17716926 | Flu vaccination service fee |
| | 17716955 | Flu vaccination auxiliary services |
| | 17717400 | COVID vaccination service |

### A.2 Factor Identifier Codes (TA3 8.2.25)

**Reference:** For complete definitions and use cases, see [CODE_STRUCTURES.md](./Abrechnung/CODE_STRUCTURES.md) Section 1

| Code | Description |
|------|-------------|
| 11 | Standard factor (Promilleanteil) - Share in promille |
| 55 | Dose in milligrams (for opioid substitution take-home) |
| 57 | Dose in milligrams (for opioid substitution supervised administration) |
| 99 | Package share in promille (waste/disposal) |

### A.3 Price Identifier Codes (TA3 8.2.26)

**Reference:** For complete definitions and tax status, see [CODE_STRUCTURES.md](./Abrechnung/CODE_STRUCTURES.md) Section 2

| Code | Description |
|------|-------------|
| 11 | Pharmacy purchase price per AMPreisV |
| 12 | Price agreed between pharmacy and pharmaceutical manufacturer |
| 13 | Actual pharmacy purchase price |
| 14 | Billing price per AMPreisV §§4,5 (Hilfstaxe - with surcharges) |
| 15 | Contracted billing price per § 129 Abs. 5 SGB V (pharmacy-insurance agreement) |
| 16 | Contract prices per § 129a SGB V |
| 17 | Billing price "Preis 2" per mg-price directory |
| 21 | Discount contract billing price "Preis 1" per § 130a Abs. 8c SGB V |
| 90 | Special price (e.g., 0.00 for markers) |

### A.4 Validation Rule Quick Lookup

| Scenario | Key Rules to Check |
|----------|-------------------|
| **Standard E-Rezept** | GEN-001, GEN-003, GEN-006, GEN-007, GEN-008, FMT-001, FMT-003, FMT-008, FMT-010 |
| **BTM Prescription** | BTM-001, BTM-002, BTM-003, BTM-004, GEN-004, GEN-006, GEN-008 |
| **Cannabis Preparation** | CAN-001 through CAN-005, REZ-013 through REZ-021, GEN-006, GEN-008 |
| **Weekly Blister** | ESQ-003, ESQ-004, REZ-007 through REZ-012, REZ-021, GEN-006 |
| **Artificial Insemination** | SPC-003, SPC-004, SPC-005, CALC-003, GEN-006 |
| **Compounding** | REZ-001 through REZ-021, CALC-001, CALC-004, GEN-006, GEN-008 |
| **Contract-Specific SOK** | SPC-008, GEN-006, GEN-007, GEN-008 |
| **SOK with E-Rezept** | GEN-007, GEN-006, GEN-008 |

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
