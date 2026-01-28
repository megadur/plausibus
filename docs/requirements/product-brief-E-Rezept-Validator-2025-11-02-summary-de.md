# Zusammenfassung: Product Brief E-Rezept-Validator (2025-11-02)

- **Zweck:** API-basierte E-Rezept-Validierung für Apotheken-Rechenzentren (ASW), um Plausibilitätsfehler vor der Kassenabrechnung zu erkennen und Ablehnungen zu reduzieren.
- **Problem:** Hohe Kosten durch abgelehnte Abrechnungen (Zeitverlust, Cashflow-Verzögerungen, Compliance-Risiken).
- **Lösung:** .NET 8 Web API mit FHIR R4-Parsing, spezialisierter Rule-Engine und ABDA/Lauer-Taxe-Integration. Liefert detaillierte Fehlermeldungen und Korrekturhinweise.
- **Zielgruppen:** Primär ASW-Rechenzentren; sekundär deren Software-Entwickler.
- **MVP-Scope:** PZN-Validierung, Dosierungs-Compliance, BtM-Regeln, Grunddaten-Checks, REST-API, strukturierte JSON-Antworten, DSGVO-konformes Logging.
- **Außerhalb MVP:** Batch-Validierung, Dashboard, historische Analysen, Interaktions-Checks, Multi-Tenancy.
- **Erfolgskriterien:** <2% Ablehnungsrate, <500 ms Antwortzeit, >95% Fehlererkennung, Integration <2 Wochen, Pilot bei mind. 2 Rechenzentren.
- **Technik/Architektur:** Stateless API, Rule-Engine-Pattern, HTTP Client Factory, Redis-Caching, Monitoring, TLS/API-Key-Auth, perspektivisch TI-Zertifizierung.
- **Risiken/Offene Punkte:** gematik-Zertifizierung (6–12 Monate, hohe Kosten), ABDA-API-Abhängigkeit, regulatorische Änderungen, Legacy-Integration, Skalierung, Wettbewerb.
- **Vision:** Ausbau zu Standard-Compliance-Tool im ASW-Ökosystem, spätere Batch- und Analytics-Funktionen, mögliche White-Label-Lizenzierung.
