**Technische Dokumentation ABDA-Artikelstamm** 

**für**   
**Apotheken-Softwarehäuser** 

Ausgabe: 01\. Januar 2025 

Bearbeitungsstand: 28.10.2024  
**Inhaltsverzeichnis** 

**1 Einleitung ................................................................................................ 5 2 Artikel-Basisinformationen (PAC\_APO) ............................................................. 6** 2.1 Attributdefinitionen ................................................................................. 6 2.2 Gruppierung der Attribute nach Themenbereichen .............................................. 31 **3 Packungsgrößenkennzeichnungen (PGR\_APO) ................................................... 34 4 Quantitative Packungsgrößenangaben (PGR2\_APO) ............................................. 35 5 Textinformationen zu Packungen (PAT\_APO)....................................................... 37 6 Klinikbausteine (KLB\_APO) ........................................................................... 38 7 Verknüpfung von Klinikpackungen mit Klinikbausteinen (VPK\_APO) .......................... 39 8 Adressen (ADR\_APO) .................................................................................. 40 9 Service-Lines (SER\_APO) ............................................................................. 42 10 Darreichungsformen (DAR\_APO)..................................................................... 43 11 Warengruppen (WAR\_APO) ........................................................................... 44 12 Indikationsbereiche (INB\_APO)....................................................................... 45 13 Indikationsbereiche von Artikeln (VPI\_APO) ....................................................... 46 14 Gruppen betr. die Rabattinformationen nach § 130a (8) SGB V (GRU\_APO)................... 47 15 Institutionskennzeichen (IKZ\_APO) .................................................................. 48 16 Zuordnung von Institutionskennzeichen zu Gruppen (IZG\_APO) ............................... 49 17 Zuordnung von Artikeln zu Gruppen (PZG\_APO) .................................................. 50 18 Daten zum Entfall des Impfstoffabschlags nach § 130a (2) SGB V (IAE\_APO) ................ 51 19 Verordnungsvorgaben (VOV\_APO) ................................................................... 52 20 Verknüpfung von Artikeln mit Verordnungsvorgaben (VPV\_APO) .............................. 55 21 Dokumente (DOK\_APO)................................................................................ 56 22 Zusammenfassung von Key-Value-Tabellen (KVP\_APO) ......................................... 57 23 ER-Modell ................................................................................................ 59** 23.1 Beschreibung der Dateibeziehungen ............................................................. 60 **24 Arzneimittelabgabepreise gemäß AMPreisV........................................................ 61 25 Arzneimittelabgabepreise gemäß § 129 (5a) SGB V ............................................... 64 26 Berechnung der Abgabepreise für Fertigarzneimittel ............................................. 66 27 Dokumenthistorie ....................................................................................... 67** 

3  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. Einleitung** 

**1 Einleitung** 

Der ABDA-Artikelstamm besteht aus den in den Abschnitten 2 bis 22 beschriebenen Dateien. 

Die Dateien werden im K2-Format zur Verfügung gestellt, dessen allgemeine Festlegungen in einem separaten Dokument erläutert werden (K2FORMAT.pdf). Hauptaspekte der vorliegenden ABDA-Artikel stamm-Dokumentation sind die Beschreibung der Datenfelder und der Beziehungen zwischen den Da teien. Erstgenannte erfolgt jeweils in Form einer dreispaltigen Tabelle: 

• 1\. Spalte: Identifier 

• 2\. Spalte: Kurzbeschreibung des Dateninhalts, Identifier im Klartext in eckigen Klammern • 3\. Spalte: 

**–** Weitere formale Angaben aus den F-Sätzen (Primärschlüsselattribut, Pflicht- oder optionale Angabe, Feldlängentyp/-länge/-datentyp) 

**–** ggf. die Kennzeichnung als Fremdschlüsselattribut, d. h. der Feldinhalt entstammt einem Feld einer anderen oder derselben Datei. Die Angabe erfolgt immer mit Verweis auf den zugehörigen Eintrag im Abschnitt 23.1 (*Beschreibung der Dateibeziehungen*). Die in Da tei KVP\_APO zusammengefassten Key/Value-Tabellen werden in diesem Zusammenhang 

*nicht* berücksichtigt. 

**–** ggf. der Wertebereich, aus Platzgründen auch auszugsweise, erkennbar an drei senkrecht angeordneten Punkten. Wertebereiche sind in KVP\_APO vollständig abgelegt. 

**–** optionale redaktionelle Erläuterung und/oder Verweise auf spezielle Textabschnitte. 

In der Tabelle werden zuerst die Primärschlüsselattribute in der Reihenfolge ihrer Identifier aufgelistet. Dahinter folgen die Nichtschlüsselattribute, sortiert nach ihrer Kurzbeschreibung, wobei die Abkürzung *Kz.* (*Kennzeichen*) nicht berücksichtigt wird. 

Das den Daten des ABDA-Artikelstamms zu Grunde liegende Entity-Relationship-Modell („ER-Modell“, jede Datei entspricht einer Tabelle) wird im Abschnitt 23 veranschaulicht. Aus technischen Gründen verbinden die Beziehungslinien nicht in jedem Fall korrespondierende Attribute. Diesbezügliche Aussa gen sind der bereits erwähnten *Beschreibung der Dateibeziehungen* zu entnehmen\! 

In dieser Dokumentation sind ausschließlich Querverweise farbig dargestellt. 

Mehrfach verwendete Abkürzungen: 

AMG Arzneimittelgesetz 

AMPreisV Arzneimittelpreisverordnung 

AM-RL Arzneimittel-Richtlinie 

AMVV Arzneimittelverschreibungsverordnung 

BtMG Betäubungsmittelgesetz 

G-BA Gemeinsamer Bundesausschuss 

GKV Gesetzliche Krankenversicherung 

IFA Informationsstelle für Arzneispezialitäten GmbH 

KVA Krankenhaus versorgende Apotheken 

MPAV Medizinprodukte-Abgabeverordnung 

MwSt Mehrwertsteuer 

SGB V Sozialgesetzbuch Fünftes Buch 

Die Bedeutung anderer Abkürzungen ergibt sich aus Fußnoten oder dem Kontext. **Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 5 von 74**  
**PAC\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**2 Artikel-Basisinformationen** 

Dateiname: PAC\_APO 

Dateilangname: Packungsinfos 

ABDATA-Dateinummer: 1005 

PAC\_APO bildet in Verbindung mit PGR\_APO, PGR2\_APO, KLB\_APO, VPK\_APO und VPV\_APO die vorwiegend wirtschaftlichen Informationen zu den Handelsformen von Fertigarzneimitteln und anderen apothekenüblichen Waren ab. 

Informationen zur Umsetzung von 

• Aut idem siehe Ende der Attributdefinitionen 

• Rabattverträgen siehe GRU\_APO 

• Impfstoffabschlägen siehe IAE\_APO 

**2.1 Attributdefinitionen** 

01 **Pharmazentralnummer** \[PZN\] 

18 **Abgabepreis des** 

**pharmazeutischen** 

**Unternehmers** 

(in Cent, ohne MwSt.) 

\[ApU\] 

C0 **Abgabepreis des** 

**pharmazeutischen** 

**Unternehmers gemäß** 

**§ 78 (3a) Satz 1 AMG** 

(in Cent) 

\[ApU\_78\_3a\_1\_AMG\] 

E3 **Ablösung Abschlag § 130a SGB V** 

\[Abloesung\_130a\_1\_8\]   
Primärschlüsselattribut, Pflichtangabe, Format: F/8/PZ8 PZN aus den Nummernkreisen (ohne Prüfziffer) 0800000 bis 0839999 sowie 8000000 bis 8399999 sind im ABDA Artikelstamm nicht enthalten, da diese ausschließlich der internen Nutzung in den Anwendungssystemen vorbehalten sind. 

Pflichtangabe, Format: V/10/NU1 

Die Feldwerte sind in allen Fällen Basis der Berechnung des Abgabepreises, in denen eine Anwendung der gülti gen AMPreisV oder der AMPreisV in der Fassung vom 31.12.2003 (ID 07 bzw. 76) verpflichtend ist. Sofern ein Er stattungsbetrag nach § 130b SGB V (s. u. C0) existiert, ist gemäß § 78 (3a) Satz 1 AMG ein Wert gleich oder kleiner als dieser abgelegt. 

Der Feldinhalt *0* ist gleichbedeutend mit *keine Angabe*. Pflichtangabe, Format: V/10/NU1   
Die Attributwerte sind nur zur Anzeige vorgesehen. Ange geben ist der unveränderte nach § 130b SGB V vereinbarte oder festgesetzte Erstattungsbetrag. 

Der Feldinhalt *0* ist gleichbedeutend mit *keine Angabe*. 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja, Ablösung Abschlag § 130a Abs. 

1/8 vereinbart 

3 *→* ja, Ablösung Abschlag § 130a Abs. 

1b Satz 1 vereinbart 

4 *→* ja, Ablösung Abschlag § 130a Abs. 

1b Satz 2 vereinbart 

*ja* kennzeichnet Artikel, deren Erstattungsbetrag nach § 130a Abs. 1 Satz 4 in Verbindung mit § 130a Abs. 8 Satz 6 SGB V den Abschlag nach § 130a Abs. 1 SGB V ablöst. Dieser Ab schlag ist somit in den Erstattungsbetrag (*Abgabepreis des pharmazeutischen Unternehmers gemäß § 78 Abs. 3a Satz 1 AMG*, siehe ID C0) eingerechnet, das Feld *Rabattwert zu Lasten des Anbieters* (ID 74) bleibt daher leer. Die Werte 3 und 4 sind seit 01.01.2024 obsolet. 

**. . .** 

**Seite 6 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PAC\_APO** 

G3 **Kz. AMNOG-Verfahren (§ 35a SGB V)** 

\[AMNOG\] 

02 **Apothekeneinkaufspreis** (in Cent, ohne MwSt.) 

\[Apo\_Ek\] 

C3 **Apothekeneinkaufspreis auf Basis des Preises des pharmazeutischen** 

**Unternehmers** (in Cent) 

\[Apo\_Ek\_PpU\] 

03 **Kz. Apothekenpflicht** \[Apopflicht\] 

04 **Apothekenverkaufspreis** (in Cent, inkl. MwSt.) 

\[Apo\_Vk\] 

C4 **Apothekenverkaufspreis auf Basis des Preises des pharmazeutischen** 

**Unternehmers** (in Cent) 

\[Apo\_Vk\_PpU\]   
Pflichtangabe, Format: F/1/NU1 

Kennzeichen, ob der Gemeinsame Bundesausschuss eine Nutzenbewertung gemäß § 35a SGB V durchführt oder be reits abgeschlossen hat. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja, befindet sich im Nutzenbewer 

tungsverfahren 

3 *→* ja, Nutzenbewertungsverfahren ab 

geschlossen 

Pflichtangabe, Format: V/10/NU1 

Bei einer Belegung der ID 07 oder 76 mit dem Wert *2* basiert der Feldwert auf dem Abgabepreis des pharmazeutischen Unternehmers (s. o. ID 18). 

Der Feldinhalt *0* ist gleichbedeutend mit *keine Angabe*. Pflichtangabe, Format: V/10/NU1   
Die Attributwerte sind nur zur Anzeige vorgesehen. Die Prei sangabe ist optional; auch bei vorhandenem Preis des phar mazeutischen Unternehmers (ID C2) ist sie nicht zwingend. 

Die Nichtangabe wird mit dem Feldinhalt *0* entsprechend *kei ne Angabe* ausgedrückt. 

Pflichtangabe, Format: F/1/NU1 

Information zur Anwendbarkeit des Begriffs Apothekenpflicht gemäß AMG und abgeleiteten Verordnungen bzw. MPAV. Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

3 *→* nein/Ausnahmeregel 

Wert *3* ist wie folgt zu interpretieren: „Nicht apothekenpflichti ges Medizinprodukt gemäß § 3 (1) Satz 2 MPAV.“ Pflichtangabe, Format: V/10/NU1   
Der hier abgebildete Preis ist grundsätzlich maßgeblich bei der Abgabe von verschreibungspflichtigen Fertigarzneimittel packungen; er ist auch maßgeblich bei der Abgabe von apo thekenpflichtigen, nicht verschreibungspflichtigen Fertig arzneimittelpackungen zu Lasten der GKV. In den vorstehend genannten Fällen basiert der Feldwert auf dem Apotheken einkaufspreis (ID 02) oder Abgabepreis des pharmazeuti schen Unternehmers (ID 18). 

Der Feldinhalt *0* ist gleichbedeutend mit *keine Angabe*. Pflichtangabe, Format: V/10/NU1   
Die Attributwerte sind nur zur Anzeige vorgesehen. Die Prei sangabe ist optional; auch bei vorhandenem Preis des phar mazeutischen Unternehmers (ID C2) ist sie nicht zwingend. 

Die Nichtangabe wird mit dem Feldinhalt *0* entsprechend *kei ne Angabe* ausgedrückt. 

**. . .** 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 7 von 74**  
**PAC\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

A5 **Kz. Artikel gemäß § 15 (1) Satz 2 ApBetrO** \[ApBetrO\_15\_1\] 

A6 **Kz. Artikel gemäß § 15 (2) ApBetrO** 

\[ApBetrO\_15\_2\] 

05 **Artikelnummer des Anbieters** 

\[Artikelnr\]   
Pflichtangabe, Format: V/2/NU1 

Die einzelnen Werte fassen Gruppen von Arzneimitteln oder weiteren Produkten zusammen, aus denen Präparate gemäß § 15 (1) Satz 2 der Apothekenbetriebsordnung 2012 in der Apotheke vorrätig zu halten sind. 

Wertebereich: 0 *→* nicht betroffen 

1 *→* Analgetika 

2 *→* Betäubungsmittel 

3 *→* Glucocorticosteroide zur Injektion 

4 *→* Antihistaminika zur Injektion 

5 *→* Glucocorticoide zur Inhalation 

zur Behandlung von Rauchgas 

Intoxikationen 

6 *→* Antischaum-Mittel zur Behandlung 

von Tensid-Intoxikationen 

7 *→* medizinische Kohle, 50 Gramm 

Pulver zur Herstellung einer Sus 

pension 

8 *→* Tetanus-Impfstoff 

9 *→* Tetanus-Hyperimmun-Globulin 

250 I. E. 

10 *→* Epinephrin zur Injektion 

11 *→* 0,9% Kochsalzlösung zur Injektion 

12 *→* Verbandstoffe, Einwegspritzen und 

\-kanülen, Katheter, Überleitungsge 

räte für Infusionen sowie Produkte 

zur Blutzuckerbestimmung 

Pflichtangabe, Format: V/2/NU1 

Mit einem Wert ungleich Null sind Artikel gekennzeichnet, die gemäß § 15 (2) der Apothekenbetriebsordnung entweder in der Apotheke vorrätig zu halten sind oder kurzfristig beschafft werden können. 

Wertebereich: 0 *→* nicht betroffen 

1 *→* Botulismus-Antitoxin vom Pferd 

2 *→* Diphtherie-Antitoxin vom Pferd 

3 *→* Schlangengift-Immunserum, poly 

valent, Europa 

4 *→* Tollwut-Impfstoff 

5 *→* Tollwut-Immunglobulin 

6 *→* Varizella-Zoster-Immunglobulin 

7 *→* C1-Esterase-Inhibitor 

8 *→* Hepatitis-B-Immunglobulin 

9 *→* Hepatitis-B-Impfstoff 

10 *→* Digitalis-Antitoxin 

11 *→* Opioide in transdermaler und trans 

mucosaler Darreichungsform 

Optionale Angabe, Format: V/18/AN1 

**. . .** 

**Seite 8 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PAC\_APO** 

64 **Artikeltyp** 

\[Artikeltyp\] 

06 **Kz. Arzneimittel** 

\[Arzneimittel\] 

G0 **Kz. Arzneimittel für seltenes Leiden** 

\[Orphan\_Drug\] 

H4 **Kz. Arzneimittel mit alters gerechter Darreichungsform oder Wirkstärke für Kinder** 

\[AM\_mit\_altersg\_Dafo\]   
Pflichtangabe, Format: V/2/NU1 

Das Attribut dient der Unterscheidung von ansonsten in den Attributen *Kurzname*, *Schlüssel der Darreichungsform*, *Schlüssel der Adresse des Anbieters* und Packungsgröße (die in PGR2\_APO mit Typ *Anbieter* angegegebene Pa ckungsgröße) identischen Artikeln. 

Wertebereich: 1 *→* Standard 

2 *→* Klinikpackung 

3 *→* Pandemieartikel 

5 *→* Schüttware 

6 *→* Muster gemäß § 47 (3) und (4) 

AMG 

7 *→* Marketingbedarf 

Reine Klinikware wird, sofern der Anbieter es wünscht, mit dem Wert *2* belegt. 

Ein Pandemieartikel stellt eine spezielle Ware dar, die für die Versorgung im Pandemiefall vorgesehen ist. 

Wert *5* betrifft zugelassene Dosenware für eine patientenindi viduelle Arzneimittelversorgung. 

Mit dem Wert *6* gekennzeichnete Artikel sollten in Anwen dungsprogrammen nur auf Anforderung angezeigt oder bei Recherchen berücksichtigt werden. 

Mit Artikeltyp *7* werden PZN gekennzeichnet, die nicht als Endverbrauchereinheit verkauft werden. Beispiele: Waren proben (z. B. Kosmetika), Werbeware (z. B. Papiertaschen 

tücher), Displays, Großpackungen (z. B. Traubenzucker Bonbons in 5 kg-Einheiten). 

Pflichtangabe, Format: F/1/NU1 

Information zur Anwendbarkeit des Begriffs Arzneimittel ge mäß AMG. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: F/1/NU1 

Kennzeichen für Arzneimittel zur Behandlung eines seltenen Leidens nach der Verordnung (EG) Nr. 141/2000 des Euro päischen Parlaments und des Rates vom 16.12.1999 über Arzneimittel für seltene Leiden (ABl. L 18 vom 22.1.2000, S. 1\) 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja, Arzneimittel mit fiktiv festgesetz 

tem Festbetrag 

Bei Arzneimitteln wird angegeben, ob für sie gemäß § 35 Abs. 1a SGB V ein fiktiv festgesetzter Festbetrag gilt. **. . .** 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 9 von 74**  
**PAC\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

H5 **Kz. Arzneimittel mit aufgehobenem Festbetrag** \[AM\_mit\_aufg\_Festb\] 

C1 **Kz. Arzneimittel mit Erstattungsbetrag nach** 

**§ 130b SGB V** 

\[AM\_mit\_Erstatt\_130b\] 

H6 **Kz. Arzneimittel mit versorgungskritischem** 

**Wirkstoff nach BfArM-Liste** \[AM\_BfArM\_Versorgung\] 

H7 **Kz. Arzneimittel zur Behandlung von Kin** 

**dern nach BfArM-Liste** 

\[AM\_BfArM\_Kinder\] 

07 **Kz. Arzneimittelpreis verordnung AMG** 

\[AMPreisV\_AMG\]   
Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja, Arzneimittel mit aufgehobenem 

Festbetrag 

Bei Arzneimitteln wird angegeben, ob für sie gemäß § 35 Abs. 5 Satz 8 SGB V der Festbetrag aufgehoben wurde. Pflichtangabe, Format: F/1/NU1   
Die Attributwerte sind nur zur Anzeige vorgesehen. Wertebereich: 0 *→* nein 

1 *→* ja, Erstattungsbetrag gilt 

2 *→* ja, Erstattungsbetrag gilt fort 

3 *→* ja, Artikel ist ein Reserveantibioti 

kum 

*ja, Erstattungsbetrag gilt* bedeutet, dass der betroffene Artikel ein Arzneimittel ist, für das ein Erstattungsbetrag gilt (§ 130b Abs. 1 oder Abs. 3 oder Abs. 3a oder Abs. 4 SGB V). *ja, Erstattungsbetrag gilt fort* bedeutet, dass der betroffene Artikel ein Arzneimittel ist, für das ein Erstattungsbetrag als höchstens zulässiger Abgabepreis ungeachtet des Wegfalls des Unterlagenschutzes fortgilt (§ 130b Abs. 8a SGB V). *ja, Artikel ist ein Reserveantibiotikum* bedeutet, dass es sich um ein Reserveantibiotikum gemäß § 130b Absatz 3b SGB V handelt. 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja, Festbetrag wurde angehoben 

3 *→* ja, vormals kein Festbetrag 

Bei Arzneimitteln wird angegeben, ob sie gemäß § 35 Abs. 5b SGB V einen versorgungskritischen Wirkstoff enthalten und auf der Liste des Bundesinstituts für Arzneimittel und Medizinprodukte (BfArM) aufgeführt sind. 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja, Festbetrag wurde aufgehoben 

3 *→* ja, vormals kein Festbetrag 

Bei Arzneimitteln wird angegeben, ob sie gemäß § 35 Abs. 5a SGB V auf der Liste des Bundesinstituts für Arzneimit tel und Medizinprodukte (BfArM) aufgeführt sind. Die Liste nennt Arzneimittel, die auf Grund der zugelassenen Darrei chungsformen und Wirkstärken zur Behandlung von Kindern notwendig sind. 

Pflichtangabe, Format: F/1/NU1 

Information zur Anwendung(spflicht) derRAMPreisV ge mäß § 78 AMG. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

**. . .** 

**Seite 10 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PAC\_APO** 

F5 **Kz. ATMP** 

\[ATMP\] 

76 **Kz. Arzneimittelpreis verordnung SGB V** 

\[AMPreisV\_SGB\] 

83 **Kz. Ausnahmeregelung gemäß § 51 AMG** 

\[Ausnahme\_51AMG\] 

A3 **Kz. Ausnahmeregelung ge mäß § 52b (2) Satz 3 AMG** 

\[Ausnahme\_52b\_2\_AMG\]   
Pflichtangabe, Format: F/1/NU1 

Neues Kennzeichen von Arzneimitteln für neuartige Therapi en (ATMP \- Advanced Therapy Medicinal Products) Wertebereich: 0 *→* keine Angabe 

1 *→* Nein: kein ATMP 

2 *→* ja, Gentherapeutikum 

3 *→* ja, somatisches Zelltherapeutikum 

4 *→* ja, Tumorimpfstoff 

5 *→* ja, biotechnologisch bearbeitetes 

Gewebeprodukt 

6 *→* ja, kombiniertes ATMP 

7 *→* ja, sonstiges ATMP 

Pflichtangabe, Format: F/1/NU1 

Information zur Anwendung(spflicht) der vonR§ 129 (5a) SGB V referenzierten AMPreisV in der am 31.12.2003 gülti gen Fassung. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

*ja* bedeutet, dass der Artikel den Anforderungen des § 51 (1) AMG gerecht wird und damit einer Ausnahmeregelung hin sichtlich der Erlaubnispflicht für das Betreiben eines Groß handels mit Arzneimitteln, Testsera oder Testantigenen ge mäß § 52a AMG unterliegt. D. h. das Betreiben eines Groß handels mit diesem Artikel bedarf nicht der Erteilung einer Großhandelserlaubnis gemäß § 52a AMG. 

*nein* bedeutet, dass der Artikel nicht dieser Ausnahmerege lung unterliegt. 

Im Falle von Nichtarzneimitteln ist *nein* bzw. *keine Angabe* im Sinne von *nicht betroffen* zu interpretieren. 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

*ja* bedeutet, dass der Artikel nicht der Regelung des § 52b (2) Satz 1 AMG unterliegt, nach der pharmazeutische Unterneh mer im Rahmen ihrer Verantwortlichkeit eine bedarfsgerechte und kontinuierliche Belieferung vollversorgender Arzneimittel großhandlungen gewährleisten müssen. 

*nein* bedeutet, dass der Artikel der Regelung des § 52b (2) Satz 1 AMG unterliegt. 

**. . .** 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 11 von 74**  
**PAC\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

B3 **Kz. Ausnahme von der Ersetzung** 

\[Ausnahme\_Ersetzung\] 

H1 **Batterie-Registriernummer** \[Batt\_Regnr\] 

78 **Kz. Bedingte Erstattung bestimmter** 

**Fertigarzneimittel** 

\[Bedingte\_Erstatt\_FAM\]   
Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* nein 

1 *→* ja 

2 *→* bedingt 

*ja* bedeutet, dass der betroffene Artikel aufgrund der Anla ge VII AM-RL, Teil B, bei Einzelverordnungen von der Erset zung gemäß § 9 (2) des Rahmenvertrages nach § 129 (2) SGB V ausgenommen ist und damit nicht durch ein Ra battarzneimittel oder ein preisgünstigeres Präparat substi tuiert werden darf. Unberührt bleibt die Abgabe von Importen nach § 9 (1) des Rahmenvertrages. 

*bedingt* bedeutet, dass kein genereller Substitutionsaus schluss gemäß AM-RL Anlage VII Teil B vorliegt, sondern die Substitution an Bedingungen geknüpft ist, z. B. das Appli kationsintervall. PZN mit dem Wert 2 und identischem Wert in ID 69 der PAC\_APO (Schlüssel der Auswahltabelle) sind – vorbehaltlich der Übereinstimmung in mindestens einer Indi kation (vergleiche Erläuterungen zu VPI\_APO und INB\_APO) – substituierbar. 

Optionale Angabe, Format: F/8/NU3 

In dem Datenfeld wird die Registriernummer der stiftung ear (siehe ID H0) gemäß BattG abgebildet. 

Optionale Angabe, Format: F/1/NU1 

Das Attribut bezieht sich auf die Ausnahmeregelungen für die GKV-Erstattung apothekenpflichtiger, nicht verschreibungs pflichtiger Arzneimittel gemäß AM-RL (*OTC-Übersicht*). Das Attribut berücksichtigt nicht, dass für Kinder apotheken pflichtige, nicht verschreibungspflichtige Arzneimittel grund sätzlich zu Lasten der GKV abgegeben werden dürfen. Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Mit *ja* sind apothekenpflichtige, nicht verschreibungspflichtige Fertigarzneimittelpackungen gekennzeichnet, die unter be stimmten Umständen erstattungsfähig sind. In diesen Fällen wird via VPV\_APO auf die zutreffende(n) Position(en) der *OTC-Übersicht* (VOV\_APO, Typ 7) verwiesen. 

Mit *nein* sind apothekenpflichtige, nicht verschreibungspflich tige Fertigarzneimittelpackungen gekennzeichnet, die nicht der *OTC-Übersicht* zugeordnet werden können. Sofern keine anderweitige Regelung zur Erstattungsfähigkeit zutrifft (z. B. Notfallkontrazeptiva, siehe VOV\_APO, Typ 21), ist von der Nicht-Erstattungsfähigkeit dieses Arzneimittels für Erwachse ne auszugehen. 

Nichtarzneimittel (z. B. Verband- und Hilfsmittel), verschrei bungspflichtige sowie nicht apothekenpflichtige Fertigarznei mittel sind nicht Gegenstand der o. g. AM-RL. Daher sind die entsprechenden PZN mit *keine Angabe* belegt. **. . .** 

**Seite 12 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PAC\_APO** 

G1 **Kz. bedingte Zulassung** \[Bedingte\_Zulassung\] 

I4 **Kz. Bestimmung nach § 130b Abs. 1c SGB V** 

\[Bestimmung\_130b\_1c\] 

08 **Kz. Betäubungsmittel** \[BTM\] 

92 **Kz. Bezugnehmende Zulassung als** 

**Generikum** 

\[Generikum\] 

G6 **Biosimilargruppe** \[Biosimilargruppe\]   
Pflichtangabe, Format: F/1/NU1 

Kennzeichen für Arzneimittel mit einer Genehmigung nach Artikel 14 Absatz 7 der Verordnung (EG) Nr. 726/2004 des Europäischen Parlaments und des Rates vom 31.03.2004 zur Festlegung von Gemeinschaftsverfahren für die Geneh 

migung und Überwachung von Human- und Tierarzneimitteln und zur Errichtung einer Europäischen Arzneimittel-Agentur (ABl. L 136 vom 30.4.2004, S. 1\) (bedingte Zulassung) Arzneimittel mit bedingter Zulassung können noch vor Ab schluss der vollständigen klinischen Prüfung auf den Markt gebracht werden 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: F/1/NU1 

Kennzeichen, ob für das Arzneimittel eine Bestimmung nach § 130b (1c) SGB V zutrifft (siehe dazu I5, I6, 74). Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: F/1/NU1 

Information zur Anwendbarkeit des Begriffs Betäubungsmittel gemäß BtMG. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

3 *→* ja, ausgenommene Zubereitung 

Artikel mit dem Wert *2* sind in VPV\_APO mit einer Verord nungsvorgabe des Typs 5 verknüpft. 

Wert *3* ist wie folgt zu interpretieren: „Ausgenommene Zu bereitung nach BtMG. Zur Verordnung und Abgabe ist kein BtM-Rezept erforderlich. Unterliegt bei Einfuhr, Ausfuhr und Durchfuhr den betäubungsmittelrechtlichen Vorschriften, aus genommen Codein und Dihydrocodein. Die Sonderregeln des Rahmenvertrages nach § 129 (2) SGB V, § 9 (3) Buchstabe f, finden keine Anwendung.“ Die Artikel sind mit einer Verord nungsvorgabe des Typs 8 verknüpft. 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Mit *ja* sind Arzneimittelpackungen belegt, die laut Anbieter angabe eine bezugnehmende Zulassung als Generikum im Sinne von § 24a AMG in der bis 5\. September 2005 gelten 

den Fassung bzw. im Sinne von § 24b (2) und § 24b (5) AMG in der Fassung der 14\. AMG-Novelle beanspruchen. Optionale Angabe, Format: V/4/NU1   
Alle Artikel mit der gleichen Biosimilargruppe gehören zur gleichen Position der Anlage VIIa der Arzneimittel-Richtlinie, da sie auf ein gemeinsames Referenzarzneimittel zugelas sen sind bzw. selbst dieses Referenzarzneimittel sind (s. G7). Diese Artikel unterliegen den Regelungen des § 40b der Arzneimittel-Richtlinie. 

**. . .** 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 13 von 74**  
**PAC\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

93 **Kz. Biotechnisch** 

**hergestelltes** 

**Fertigarzneimittel** 

\[Biotech\_FAM\] 

C7 **Kz. Biotechnologisch hergestelltes** 

**Arzneimittel** 

\[Biotech\_AM\] 

09 **Breite** (in Millimeter) \[Breite\] 

E1 **Kz. CMR-Gefahrstoff** \[CMR\_Stoff\] 

H8 **Datum, ab dem der** 

**Abgabepreis des pharma** 

**zeutischen Unternehmers** 

**nach § 78 Abs. 3a Satz 1** 

**AMG gilt** 

\[Gdat\_ApU\_78\_3a\_1\] 

I5 **Datum, ab dem der** 

**Abgabepreis des pharma** 

**zeutischen Unternehmers** 

**nach § 78 Abs. 3a Satz 1** 

**AMG mit Bestimmung nach § 130b Abs. 1c SGB V gilt** 

\[Gdat\_ApU78\_3a\_130b1c\]   
Pflichtangabe, Format: F/1/FLA 

Wertebereich: 0 *→* nein 

1 *→* ja 

Das Attribut wird von ABDATA Pharma-Daten-Service ge pflegt. 

Den Wert *0* nehmen auch Artikel an, die keine Arzneimittel darstellen. 

Artikel mit Wert *1* unterliegen nicht dem Substitutionsgebot nach Rahmenvertrag gemäß § 129 (2) SGB V. Diesbezügli che Ausnahmen haben einen Eintrag in *Schlüssel der Aus wahltabelle* (ID 69). 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja, biotechnologisch hergestelltes 

Original-Arzneimittel 

3 *→* ja, Biosimilar 

4 *→* ja, biotechnologisch hergestelltes 

Arzneimittel, das zu einem weite 

ren biotechnologisch hergestellten 

Arzneimittel in Ausgangsstoffen und 

Herstellungsprozess identisch ist 

Derzeit hat das IFA-Kennzeichen rein informativen Charak ter. Unplausible Konstellationen zu ID 93 sind auf Grund der abweichenden Datenherkunft möglich. 

Optionale Angabe, Format: V/6/NU1 

Angabe gilt bei verpackten Artikeln inklusive der äußeren Umhüllung; bei nicht rechtwinkligen Artikeln ist das max. Maß angegeben; welches der Außenmaße als Breite bezeichnet wird, entscheidet sich nach dem „Gesicht“ der Packung. Die Breite der Packung ist die Tiefe. 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Mit *ja* gekennzeichnet sind Artikel (auch Fertigarzneimittel) mit einem oder mehreren Inhaltsstoffen (auch Wirkstoffe), die einzeln oder in Kombination karzinogen, keimzellmuta 

gen oder reproduktionstoxisch sind. Für Fertigarzneimittel mit CMR-Stoffen gilt eine Berücksichtigungsgrenze nach TRGS (*Technische Regeln für Gefahrstoffe*) 525\. 

Optionale Angabe, Format: F/8/DT8 

Wird ein Erstattungsbetrag nach § 130b SGB V vereinbart oder festgesetzt (ID C0), ist das Datum angegeben, ab dem dieser Preis nach § 130b Abs. 3a SGB V gilt. 

Optionale Angabe, Format: F/8/DT8 

Für Arzneimittel, für welche eine Bestimmung nach § 130b (1c) SGB V zutrifft (ID I4), ist das Datum angegeben, ab dem dieser Preis gilt. 

**. . .** 

**Seite 14 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PAC\_APO** 

I0 **Datum, ab dem die** 

**Ablösung des Abschlags** 

**§ 130a SGB V gilt** 

\[Gdat\_Abloesung\_130a\] 

89 **Kz. Diätetikum** 

\[Diaetetikum\] 

C5 **Differenz zwischen dem Preis des pharmazeutischen Unternehmers und dem** 

**Abgabepreis gemäß** 

**§ 78 (3a) Satz 1 AMG** 

(in Cent) 

\[Diff\_PpU\_ApU\_78\_3a\_1\] 

11 **Kz. Droge/Chemikalie** \[Droge\_Chemikalie\] 

12 **Kz. Eichung** 

\[Eichung\] 

B8 **Kz. Elektro- und** 

**Elektronikgeräte** 

**Stoff-Verordnung** 

\[ElektroStoffV\] 

A8 **Kz. EU-Bio-Logo nach EG-Öko-Basisverordnung** 

\[EU\_Bio\_Logo\]   
Optionale Angabe, Format: F/8/DT8 

Ablösung nach ID E3 

Pflichtangabe, Format: F/1/NU1 

Information zur Anwendbarkeit des Begriffs *Diätetikum* ge mäß der *Verordnung über diätetische Lebensmittel (Diätver ordnung)*. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja, gemäß § 31 SGB V 

3 *→* sonstiges Diätetikum 

Der Wert *2* kennzeichnet Diätetika, die von § 31 SGB V be troffen sind. 

Pflichtangabe, Format: V/10/NU2 

Die Attributwerte sind nur zur Anzeige vorgesehen. Abge legt wird das Resultat der Berechnung *Wert aus ID C2* minus *Wert aus ID C0*, sofern beide definiert sind. Negative Werte sind möglich. 

Der Feldinhalt *0* ist gleichbedeutend mit *keine Angabe*. 

Pflichtangabe, Format: F/1/NU1 

Kz. für Stoffe, die aufgrund ihrer Zweckbestimmung als Aus gangsstoff für die Herstellung von Arzneimitteln oder anderen Produkten (wie Kosmetika, Lebensmittel etc.) in Apotheken dienen oder dienen können, sowie Laborsubstanzen. Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: F/1/NU1 

Kz. gemäß § 2 Eichgesetz (Eichpflicht); besteht Eichpflicht, so kann die Laufzeit dem Feld *Laufzeit der Eichung* entnom men werden. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Mit *ja* werden von der *Verordnung zur Beschränkung der Ver wendung gefährlicher Stoffen in Elektro- und Elektronikge räten* (ElektroStoffV) betroffene Artikel gekennzeichnet. Pflichtangabe, Format: F/1/NU1   
Kennzeichnung von Artikeln im Sinne der *Verordnung (EG) Nr. 834/2007*. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

**. . .** 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 15 von 74**  
**PAC\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

F6 **Kz. EU-Novel Food Verordnung** 

\[Novel\_Food\] 

C8 **Kz. Explosivgrundstoff** \[Explosivgrundstoff\] 

97 **Festbetrag** 

(in Cent, inkl. MwSt.) 

\[Festbetrag\]   
Pflichtangabe, Format: F/1/NU1 

Kennzeichen gemäß der Verordnung (EU) 2015/2283 über neuartige Lebensmittel (Novel Food-Verordnung). Wertebereich: 0 *→* keine Angabe 

1 *→* nicht betroffen, da kein Lebensmit 

tel 

2 *→* kein Novel Food gemäß Novel 

Food-VO 

3 *→* Betroffenheit fraglich, aufgrund 

Übergangsvorschriften in Verkehr 

4 *→* ja 

Wert *1* ist wie folgt zu interpretieren: „Artikel ist kein Lebens mittel und daher nicht betroffen“ 

Wert *2* ist wie folgt zu interpretieren: „Artikel ist ein Lebens mittel oder Lebensmittelinhalts- oder \-zusatzstoff und ist kein Novel Food gemäß Novel Food-Verordnung (EU) 2015/2283.“ Wert *3* ist wie folgt zu interpretieren: „Artikel ist hinsichtlich einer Betroffenheit von der Novel Food-Verordnung (EU) 2015/2283 noch fraglich, darf aber bis auf Weiteres als Le bensmittel oder Lebensmittelinhalts- oder \-zusatzstoff gemäß den gültigen Übergangsvorschriften vermarktet und konsu miert werden.“ 

Wert *4* ist wie folgt zu interpretieren: „Artikel ist ein Novel Food gemäß Novel Food-Verordnung (EU) 2015/2283“ Pflichtangabe, Format: F/1/NU1   
Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

*ja* bedeutet, dass der Artikel Stoffe der Anhänge der *Verord nung (EU) Nr. 98/2013* enthält. Betroffene Artikel können bei spielsweise Chemikalien, Biozide, Medizinprodukte, Kosme tika, Pflanzenschutzmittel oder sonstige Artikel des Neben sortimentes sein. Arzneimittel sind von diesem Kennzeichen nicht betroffen. 

Optionale Angabe, Format: V/10/NU1 

Angesprochen sind Festbeträge für Arzneimittel gemäß § 35 SGB V. Grundsätzlich werden festbetragsgeregelte Arznei mittelpackungen von der GKV zum Apothekenverkaufspreis (ID 04) erstattet, wenn dieser nicht höher ist als der zugeord nete Festbetrag. Ist der Apothekenverkaufspreis einer PZN höher als der zugeordnete Festbetrag, ist die Differenz als sog. Mehrkosten durch den GKV-Versicherten zu tragen (zu möglichen Ausnahmeregelungen siehe GRU\_APO\!). Siehe auch Erläuterung zu *Festbetragsstufe* (s. u.) und *Schlüssel der Festbetragstabelle* (ID 46)\! 

**. . .** 

**Seite 16 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PAC\_APO** 

98 **Festbetragsstufe** 

\[Festbetragsstufe\] 

16 **Kz. feuchteempfindlich** \[Feuchteempf\] 

17 **Gewicht** (brutto, in Gramm) \[Gewicht\] 

15 **Global Trade Item Number** \[GTIN\] 

I1 **Gültigkeitsdatum bei Änderung der** 

**Vertriebsinformation** 

\[Gdat\_Vertriebsinfo\] 

21 **Gültigkeitsdatum bei Änderung eines Preises** 

\[Gdat\_Preise\] 

77 **Kz. Hilfsmittel** 

**zum Verbrauch** 

\[Hm\_zum\_Verbrauch\] 

1European Article Number   
2Universal Product Code   
Optionale Angabe, Format: F/1/NU1 

Die Festbetragsstufen strukturieren gemäß § 35 SGB V die Arzneimittelfestbeträge. 

Wertebereich: 1 *→* Festbetrag der Stufe 1 

2 *→* Festbetrag der Stufe 2 

3 *→* Festbetrag der Stufe 3 

*Stufe 1*: Arzneimittel mit denselben Wirkstoffen *Stufe 2*: Arzneimittel mit pharmakologisch-therapeutisch ver gleichbaren Wirkstoffen, insbesondere mit chemisch ver wandten Stoffen 

*Stufe 3*: Arzneimittel mit therapeutisch vergleichbarer Wir kung, insbesondere Arzneimittelkombinationen. Siehe auch Erläuterung zu *Festbetrag* (s. o.) und *Schlüssel der Festbetragstabelle* (ID 46)\! 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* trocken lagern 

Optionale Angabe, Format: V/8/NU1 

Optionale Angabe, Format: F/14/NU3 

Die GTIN ermöglicht eine weltweit eindeutige Artikel identifizierung; das Konzept integriert die EAN1\- und UPC2\- Codeschemata. 

Optionale Angabe, Format: F/8/DT8 

Siehe Erläuterung zuR*Gültigkeitsdatum bei Änderung ei nes Preises*\! 

Optionale Angabe, Format: F/8/DT8 

Das Attribut bezieht sich auf alle vom Anbieter gemeldeten preisbezogenen Informationen, dazu gehören ID 02, 04, 07, 18, 24, 37, 68, 76, A0, B0, C0, C2, C3, C4, C5, F4. 

**Gültigkeitsdaten** (gilt auch für ID I1): Bei Neuaufnahmen werden generell die Gültigkeitsdaten gesetzt. Sowohl die Änderung eines Gültigkeitsdatums ohne Änderung in dem zugrunde liegenden Attribut als auch der umgekehrte Fall sind möglich (somit auch rückwirkende Änderungen). Pflichtangabe, Format: F/1/NU1   
Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

*ja* bedeutet, dass bei GKV-Verordnungen die Zuzah lungshöhe unabhängig von der Verordnungshäufigkeit pro Monat nicht größer als 10 Euro ist. 

**. . .** 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 17 von 74**  
**PAC\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

E5 **Hochladedatum der Verifizierungsinformationen für den Pflichtbetrieb** 

\[Hochladedatum\] 

22 **Höhe** (in Millimeter) 

\[Hoehe\] 

B9 **Importgruppennummer** \[Importgruppennr\] 

E6 **Kz. In-vitro-Diagnostika Klasse** 

\[IVD\_Klasse\] 

A9 **Kz. Kosmetikum nach EG-Verordnung** 

\[Kosmetikum\_EG\_VO\] 

F4 **Krankenhausapotheken einkaufspreis des pharma zeutischen Unternehmers** 

(in Cent) 

\[KHAEP\_PPU\] 

24 **Krankenhauseinkaufspreis** (in Cent, ohne MwSt.) 

\[Krankenhaus\_Ek\]   
Optionale Angabe, Format: F/8/DT8 

Angegeben ist das Datum, ab dem individuelle Daten von Pa ckungen abrufbar sind, die der Verifizierungspflicht im Pflicht betrieb unterliegen. 

Zur Umsetzung dieser Information wird auf die Implemen tierungsvorgaben der NGDA – Netzgesellschaft Deutscher Apotheker mbH verwiesen (*securPharm-Abgabeserver – Im plementierungsleitfaden Client-Implementierungen*, “secILF\_Client“). 

Optionale Angabe, Format: V/6/NU1 

Siehe Erläuterung zuR*Breite*\! Die Höhe ist die rechte senkrechte Seite. 

Optionale Angabe, Format: V/5/NU1 

Artikel mit identischer Importgruppennummer gelten unterein ander als „gleich“ (evtl. abweichende Hilfsstoffzusammenset zung). Im Allgemeinen besteht eine Importgruppe aus Origi nal und Importen. Da aber das Original nicht zwingend exis tieren muss, ist eine solche Gruppierung mittels ID 62 nicht immer möglich. Dies führt zu Problemen bei der Abgabe im portierter Arzneimittel gemäß § 9 (1) des Rahmenvertrages nach § 129 (2) SGB V, weshalb in diesem Zusammenhang die Gruppierung ausschließlich mit der Importgruppennum mer erfolgen sollte. 

Pflichtangabe, Format: F/1/NU1 

Artikel 47 (1) der *Verordnung (EU) 2017/746* schreibt die Ein stufung von In-vitro-Diagnostika unter Berücksichtigung ihrer Zweckbestimmung und der damit verbundenen Risiken vor. Die Klassifizierung erfolgt gemäß Anhang VIII der Verord nung. 

Wertebereich: 0 *→* keine Angabe 

1 *→* Klasse A 

2 *→* Klasse B 

3 *→* Klasse C 

4 *→* Klasse D 

Pflichtangabe, Format: F/1/NU1 

Kennzeichnung von Artikeln im Sinne der *Verordnung (EG) Nr. 1223/2009*. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: V/10/NU1 

Angabe des Krankenhausapothekeneinkaufspreises des pharmazeutischen Unternehmers in EURO, dargestellt als Cent-Betrag, der bei Arzneimitteln, für die ein Erstattungsbe trag nach § 130b SGB V gilt, vom Krankenhauseinkaufspreis (ID 24) abweichen kann. 

Der Feldinhalt *0* ist gleichbedeutend mit *keine Angabe*. Pflichtangabe, Format: V/10/NU1   
Auch im Sinne von KVA-Preis. Der Feldinhalt *0* ist gleichbe deutend mit *keine Angabe*. 

**. . .** 

**Seite 18 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PAC\_APO** 

25 **Kz. Kühlkette** 

\[Kuehlkette\] 

26 **Kurzname** 

\[Kurzname\] 

27 **Länge** (in Millimeter) \[Laenge\] 

28 **Kz. lageempfindlich** \[Lageempf\] 

I3 **Kz. Lagertemperatur beachten** 

\[Lagertemperatur\] 

29 **Lagertemperatur, maximal** (in Grad Celsius) 

\[Lagertemperatur\_max\] 

30 **Lagertemperatur, minimal** (in Grad Celsius) 

\[Lagertemperatur\_min\] 

31 **Langname** 

\[Langname\] 

67 **Langname, ungekürzt** \[Langname\_ungekuerzt\] 

33 **Laufzeit** (in Monaten) \[Laufzeit\_Verfall\] 

32 **Laufzeit der Eichung** (in Monaten) 

\[Laufzeit\_Eichung\]   
Pflichtangabe, Format: F/1/NU1 

Kz. für temperaturempfindliche Artikel, die bei Lagerung und Transport gekühlt werden müssen. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: V/26/AN1 

Optionale Angabe, Format: V/6/NU1 

Siehe Erläuterung zuR*Breite*\! Die Länge ist die vordere untere Seitenkante. 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* liegend lagern 

3 *→* aufrecht lagern 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Mit diesem Kennzeichen wird die Information abgebildet, ob für den Artikel die Lagertemperatur zu beachten ist. Ist das Kennzeichen mit dem Wert *ja* belegt, so sind die an gegebenen Lagertemperaturen (ID 29 und 30) verbindlich. Ist das Kennzeichen mit *nein* belegt, sind beide Lagertem peraturen leer und für die Lagerung des Artikels gibt es hin sichtlich der Temperatur keine Einschränkungen. Ist für das Kennzeichen der Wert *keine Angabe* gemeldet, darf aus dem Fehlen der Lagertemperaturen nicht geschlossen werden, dass eine Lagerung ohne Einschränkungen möglich ist. 

Optionale Angabe, Format: V/4/NU2 

Optionale Angabe, Format: V/4/NU2 

Optionale Angabe, Format: V/50/AN1 

Optionale Angabe, Format: U/AN1 

Optionale Angabe, Format: V/3/NU1 

Angegeben ist die garantierte Haltbarkeitsdauer des Artikels in Monaten, gerechnet ab dem Herstellungszeitpunkt; bei entsprechenden Artikeln bezieht sich die Laufzeit auf den ungeöffneten Zustand. 

Optionale Angabe, Format: V/3/NU1 

Gültigkeitsdauer der Eichung in Monaten 

**. . .** 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 19 von 74**  
**PAC\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

90 **Kz. Lebensmittel** 

\[Lebensmittel\] 

34 **Kz. lichtempfindlich** \[Lichtempf\] 

81 **Kz. Lifestyle-Medikament** \[Lifestyle\] 

I2 **Kz. MedCanG** 

\[MedCanG\] 

36 **Kz. Medizinprodukt** \[Medizinprodukt\]   
Pflichtangabe, Format: F/1/NU1 

Information zur Anwendbarkeit des Begriffs *Lebensmittel* ge mäß *Lebensmittel- und Futtermittelgesetzbuch – LFGB*. Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* vor Licht schützen 

3 *→* vor Sonne schützen 

Pflichtangabe, Format: F/1/NU1 

Das Attribut bezieht sich auf Anlage II der AM-RL in Verbin dung mit dem GKV-Erstattungsausschluss gemäß § 34 (1) Satz 7 SGB V. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja, ohne Ausnahmen 

3 *→* ja, mit Ausnahmen 

*ja, ohne Ausnahmen* bedeutet, der Artikel ist vom GKV Erstattungsausschluss betroffen. 

*ja, mit Ausnahmen* bedeutet, der Artikel ist in bestimm ten Fällen (indikationsabhängig) vom GKV-Erstattungs ausschluss betroffen. Artikel mit diesem Wert sind in VPV\_APO mit einer Verordnungsvorgabe des Typs 5 verk nüpft. 

*nein* bzw. *keine Angabe* bedeutet, der Artikel ist vom GKV Erstattungsausschluss gemäß § 34 (1) Satz 7 SGB V nicht betroffen. Damit ist aber keine übergeordnete Aussage zur GKV-Erstattungsfähigkeit verbunden\! 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja, Cannabis zu medizinischen 

Zwecken nach § 2 Nr. 1 MedCanG 

3 *→* ja, Cannabis zu medizinisch 

wissenschaftlichen Zwecken nach 

§ 2 Nr. 2 MedCanG 

Bei Artikeln wird angegeben, ob sie Cannabis nach § 2 Nr. 1 oder Nr. 2 MedCanG sind. 

Pflichtangabe, Format: F/1/NU1 

Information zur Anwendbarkeit des Begriffs Medizinprodukt gemäß der Verordnung (EU) 2017/745 des Europäischen Parlaments und des Rates über Medizinprodukte, zur Ände rung der Richtlinie 2001/83/EG, der Verordnung (EG) Nr. 178/2002 und der Verordnung (EG) Nr. 1223/2009 und zur Aufhebung der Richtlinien 90/385/EWG und 93/42/EWG des Rates. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

**. . .** 

**Seite 20 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PAC\_APO** 

E7 **Kz. Medizinprodukte-Klasse** \[MP\_Klasse\] 

D9 **Kz. Medizinprodukt gemäß § 31 (1) Satz 2 SGB V** 

\[Medizinprodukt\_AMRL\] 

F1 **Mehrfachvertriebsgruppe** \[MV\_Gruppe\] 

37 **Kz. Mehrwertsteuersatz** \[MwSt\] 

65 **Mindestbestellmenge** \[Mindestbestellmenge\]   
Pflichtangabe, Format: F/1/NU1 

Artikel 51 (1) der *Verordnung (EU) 2017/745* schreibt die Ein stufung von Medizinprodukten unter Berücksichtigung ihrer Zweckbestimmung und der damit verbundenen Risiken vor. Die Klassifizierung erfolgt gemäß Anhang VIII der Verord nung. 

Wertebereich: 0 *→* keine Angabe 

1 *→* Klasse I 

2 *→* Klasse IIa 

3 *→* Klasse IIb 

4 *→* Klasse III 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Es handelt sich um eine vom Anbieter gemeldete Information; unplausible Konstellationen zu den Verordnungsvorgaben des Typs 1 sind nicht auszuschließen. 

Optionale Angabe, Format: V/4/NU1 

Alle Artikel mit der gleichen Mehrfachvertriebsgruppe befin den sich zueinander im Mehrfachvertrieb nach § 2 (15) des Rahmenvertrages nach § 129 (2) SGB V. Eine Mehrfach vertriebsgruppe besteht im Allgemeinen aus Parallelarznei mitteln und ihren Importen. Sie kann zur Gruppierung von Artikeln bei einer Abgabe nach § 9 (2) S. 2 des Rahmenver trages nach § 129 (2) SGB V herangezogen werden. Arti kel in einer Mehrfachvertriebsgruppe können verschiede nen Importgruppen angehören oder keinen Eintrag in B9 in PAC\_APO besitzen. Falls der Austausch gegen ein wirk stoffgleiches Arzneimittel ausgeschlossen ist (gesetztes Aut idem-Kreuz), ist bei der Abgabe von Arzneimitteln nach Rah menvertrag dagegen die Importgruppe des verordneten Arz neimittels zu berücksichtigen. 

Pflichtangabe, Format: F/1/NU1 

Angabe gemäß § 12 *Umsatzsteuergesetz (UStG)*. Wertebereich: 0 *→* keine Angabe 

1 *→* voll (derzeit 19%) 

2 *→* ermäßigt (derzeit 7%) 

3 *→* ohne 

Optionale Angabe, Format: V/6/NU1 

Kleinste Abgabeeinheit beim Direktbezug vom Anbieter, be zogen auf die Artikelanzahl (nicht auf die Packungsgröße). Beispielsweise bedeutet die Angabe „100“, dass mindestens 100 Artikel oder ein Vielfaches dieser Menge bestellt werden können. 

**. . .** 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 21 von 74**  
**PAC\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

94 **Kz. Tierarzneimittel Abgabemengen-Register** 

**(TAR)** 

\[Mitteilung\_47\_1cAMG\] 

91 **Kz. Nahrungsergänzungs mittel** 

\[NEM\] 

E8 **National Trade Item Number** \[NTIN\] 

40 **Kz. Negativliste** 

\[Negativliste\] 

E9 **Pharmacy Product Number** \[PPN\] 

C6 **„Pharmazentralnummer“ der Bundesopiumstelle** 

\[BOPSTNr\] 

68 **Kz. Preisangaben** 

**verordnung** 

\[PAngV\]   
Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja, Meldepflicht nach § 45 Abs. 6 

Nr. 1 TAMG 

3 *→* ja, Meldepflicht nach § 45 Abs. 6 

Nr. 2 TAMG 

4 *→* ja, Meldepflicht nach § 45 Abs. 6 

Nr. 1 und 2 TAMG 

Das Kennzeichen bildet die Information ab, ob der Arti kel gemäß § 45 Abs. 6 TAMG an das Tierarzneimittel Abgabemengen-Register (TAR) gemeldet werden muss. Anmerkung zum technischen Namen: Die Meldepflicht wurde früher über § 47 (1c) AMG geregelt. 

Pflichtangabe, Format: F/1/NU1 

Information zur Anwendbarkeit des Begriffs *Nahrungser gänzungsmittel* gemäß der *Verordnung über Nahrungser gänzungsmittel*. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Optionale Angabe, Format: F/14/NU3 

Aus der 8-stelligen Pharmazentralnummer (ID 01) entsteht durch Voranstellen der Ziffernfolge *04150* und Anhängen ei ner Prüfziffer die 14-stellige NTIN. 

Pflichtangabe, Format: F/1/NU1 

Angabe im Sinne der bereits existierenden Negativliste Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja, gemäß Arzneimittel-Richtlinie, 

ohne Ausnahmen 

3 *→* ja, gemäß Arzneimittel-Richtlinie, 

mit Ausnahmen 

Optionale Angabe, Format: F/12/NU3 

Aus der 8-stelligen Pharmazentralnummer (ID 01) entsteht durch Voranstellen der Ziffernfolge *11* und Anhängen einer 2-stelligen Prüfsumme die 12-stellige PPN. 

Optionale Angabe, Format: F/8/NU3 

Bei den Attributwerten handelt es sich um von der Bundes opiumstelle des BfArM (Bundesinstitut für Arzneimittel und Medizinprodukte) vergebene und im Bundesanzeiger veröffentlichte „Pharmazentralnummern“ für den *Außenhan del* mit Betäubungsmitteln. Eine Verwechslung mit den unter ID 01 abgelegten Werten ist zu vermeiden\! 

Pflichtangabe, Format: F/1/NU1 

Information zur Anwendungspflicht der *Preisangabenverord nung*. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

**. . .** 

**Seite 22 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PAC\_APO** 

C2 **Preis des** 

**pharmazeutischen** 

**Unternehmers** (in Cent) 

\[PpU\] 

G9 **Preisstrukturmodell** 

\[Preisstrukturmodell\] 

A0 **Rabattwert nach § 130a (2) SGB V** (in Cent, ohne MwSt.) \[Rabwert\_130a\_2\_SGB\] 

74 **Rabattwert zu Lasten des Anbieters** 

(in Cent, ohne MwSt.) 

\[Rabwert\_Anbieter\] 

85 **Rabattwert zu Lasten des Anbieters für ein Generikum** (in Cent, ohne MwSt.) 

\[Rabwert\_Generikum\] 

86 **Rabattwert zu Lasten des Anbieters** 

**wegen Preismoratoriums** 

(in Cent, ohne MwSt.) 

\[Rabwert\_Preismora\] 

72 **Kz. Rabatt zu Lasten der Apotheke** 

\[Rab\_Apo\]   
Pflichtangabe, Format: V/10/NU1 

Die Attributwerte sind nur zur Anzeige vorgesehen. Die An gabe ist obligatorisch, wenn für einen Artikel ein Erstattungs betrag nach § 130b SGB V vereinbart oder festgesetzt wurde (ID C0 und C1); es handelt sich hierbei um den nach § 78 (3) AMG zu meldenden Preis. 

Der Feldinhalt *0* ist gleichbedeutend mit *keine Angabe*. Pflichtangabe, Format: F/1/NU1   
Wertebereich: 0 *→* keine Angabe 

1 *→* nicht betroffen 

2 *→* lineares Preisstrukturmodell 

3 *→* flaches Preisstrukturmodell 

4 *→* komplexes Preisstrukturmodell 

Für Arzneimittel, deren Erstattungsbetrag aufgrund von § 130b Abs. 8a SGB V als höchstens zulässiger Abgabe preis ungeachtet des Wegfalls des Unterlagenschutzes fort gilt, wird hier eine Information zum Preisstrukturmodell ange geben. 

Beim linearen Preisstrukturmodell ("lineares pricing") steigt der Preis je Packung mit jeder weiteren Einheit der Wirkstoff gesamtmenge in einer Packung. 

Beim flachen Preisstrukturmodell ("flat pricing") haben un terschiedliche Wirkstärken je Einheit einer Darreichungsform den gleichen Preis. 

Beim komplexen Preisstrukturmodell ist das Modell zur Preis bestimmung des Arzneimittels nicht eindeutig linear oder flach. 

Pflichtangabe, Format: V/10/NU1 

Der Abschlag betrifft Impfstoffe für Schutzimpfungen nach § 20d (1) SGB V; siehe auch IAE\_APO\! 

Der Feldinhalt *0* ist gleichbedeutend mit *keine Angabe*. Optionale Angabe, Format: V/10/NU1   
Dieser Rabattwert ergibt sich aufgrund des § 130a (1)/(1a) SGB V oder § 130b (1c) SGB V, wonach ein Anbieter für be stimmte Fertigarzneimittelpackungen gegenüber der GKV einen Rabatt zu gewähren hat. 

Optionale Angabe, Format: V/10/NU1 

Dieser Rabattwert ergibt sich aufgrund § 130a (3b) SGB V, wonach ein Anbieter für bestimmte (patentfreie, wirkstoffglei che) Fertigarzneimittelpackungen („Generika“) gegenüber der GKV einen Rabatt zu gewähren hat. Die tatsächliche Ra batthöhe wird nach einem komplexen Rechenverfahren ermit telt und kann auch den Wert „0“ annehmen. 

Optionale Angabe, Format: V/10/NU1 

Dieser Rabattwert ergibt sich aufgrund § 130a (3a) SGB V. Er wird nach einem komplexen Rechenverfahren ermittelt und kann aufgrund von Ausnahmeregelungen auch den Wert „0“ annehmen. 

Pflichtangabe, Format: F/1/FLA 

Apothekenrabatt gemäß § 130 SGB V 

Wertebereich: 0 *→* nein 

1 *→* ja 

**. . .** 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 23 von 74**  
**PAC\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

G7 **Kz. Referenzarzneimittel für Biosimilars** 

\[Ref\_Biosimilar\] 

D6 **Registrierungsnummer der stiftung elektro-altgeräte**   
**register® (ear)**   
\[Regnr\_stiftung\_ear\] 

23 **Kz. Re-/Parallelimport** \[Import\_Reimport\] 

44 **Schlüssel der Adresse des Anbieters** 

\[Key\_ADR\_Anbieter\] 

66 **Schlüssel der Adresse des Herstellers** 

\[Key\_ADR\_Hersteller\] 

F9 **Schlüssel der Adresse des örtlichen Vertreters** 

\[Key\_ADR\_Vertreter\] 

F8 **Schlüssel der Adresse des Zulassungsinhabers** 

\[Key\_ADR\_Zulassung\] 

69 **Schlüssel der** 

**Auswahltabelle** 

\[Key\_AUS\] 

45 **Schlüssel der** 

**Darreichungsform** 

\[Key\_DAR\] 

46 **Schlüssel der** 

**Festbetragstabelle** 

\[Key\_FES\] 

47 **Schlüssel der** 

**Warengruppe** 

\[Key\_WAR\]   
Pflichtangabe, Format: F/1/FLA 

Kennzeichen, ob ein Artikel gemäß § 40b der Arzneimittel Richtlinie in der zugehörigen Anlage VIIa als Referenzarznei mittel gekennzeichnet ist. 

Wertebereich: 0 *→* nein 

1 *→* ja 

Optionale Angabe, Format: F/8/NU3 

Das Attribut enthält die in Zusammenhang mit der Registrie rung gemäß *Elektro- und Elektronikgerätegesetz – ElektroG* (siehe ID H0) durch die stiftung ear an den Hersteller (i. S. d. ElektroG) in der Form *WEEE-Reg.-Nr. DE \<Registrierungs* 

*nummer\>* vergebene Nummer. 

Pflichtangabe, Format: F/1/FLA 

Die Angabe bezieht sich ausschließlich auf Fertigarzneimittel. Siehe auch ID 62\! 

Wertebereich: 0 *→* nein 

1 *→* ja 

RFremdschlüsselattribut, Pflichtangabe, 

Format: V/5/NU1 

„Anbieter“ steht im Sinne von „Inverkehrbringer“, „PZN Anmelder“. 

RFremdschlüsselattribut, optionale Angabe, 

Format: V/5/NU1 

Das Attribut dient in bestimmten Fällen der Differenzierung von Hersteller und Anbieter. 

RFremdschlüsselattribut, optionale Angabe, 

Format: V/5/NU1 

RFremdschlüsselattribut, optionale Angabe, 

Format: V/5/NU1 

Optionale Angabe, Format: V/9/NU1 

Erläuterungen siehe Ende der Attributdefinition 

RFremdschlüsselattribut, optionale Angabe, 

Format: F/3/AL1 

Optionale Angabe, Format: V/6/NU1 

Pro PZN sind die Attribute *Schlüssel der Festbetragstabelle*, *Festbetrag* (ID 97) und *Festbetragsstufe* (ID 98) belegt, wenn die PZN einem Arzneimittelfestbetrag gemäß § 35 SGB V unterliegt. Andernfalls, wenn die PZN von keinem Festbe trag betroffen ist oder kein Arzneimittel darstellt, sind die se drei Attribute leer. Im *Schlüssel der Festbetragstabelle* übereinstimmende PZN unterliegen einheitlichen Kriterien der Festbetragszuordnung. Die Inhalte der ID 46, 97 und 98 sind nicht zur Umsetzung der Aut idem-Regelung nach § 129 SGB V geeignet\! 

RFremdschlüsselattribut, optionale Angabe, 

Format: V/8/WGS 

**. . .** 

**Seite 24 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PAC\_APO** 

E4 **Kz. securPharm Pilot** \[securPharm\_Pilot\] 

87 **Kz. Sicherheitsdatenblatt für Gefahrstoffe erforderlich** 

\[SDB\_erforderlich\] 

F3 **Kz. Sonstiges Produkt zur Wundbehandlung gemäß** 

**§ 31 (1a) SGB V** 

\[Wundbehand\_31\_1a\_SGB\] 

48 **Sortierbegriff, basierend auf *Langname*** (ID 31) 

\[Sortierbegriff\] 

B1 **Kz. steril** 

\[Steril\] 

H0 **Kz. stiftung ear** 

\[Stiftung\_Ear\] 

49 **Kz. Tierarzneimittelgesetz** \[Tierarzneimittel\]   
Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Zur Umsetzung dieser Information siehe ID E5\! Pflichtangabe, Format: F/1/NU1   
Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

*ja* bedeutet, dass für den Artikel das Vorhalten eines Sicher heitsdatenblattes für Gefahrstoffe erforderlich ist. *nein* und *keine Angabe* sind entsprechend zu interpretieren. Pflichtangabe, Format: F/1/NU1   
Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

*ja* bedeutet, dass es sich um ein sonstiges Produkt zur Wundbehandlung nach § 31 (1a) SGB V handelt. Optionale Angabe, Format: V/30/AN2 

Pflichtangabe, Format: F/1/NU1 

Information zur Sterilität eines Artikels. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja, unterliegt der Registrierungs 

pflicht 

Die stiftung ear ist zuständig für die Registrierungspflichten der Anbieter von Batterien (nach BattG) und von Elektro- und Elektronikgeräten (nach ElektroG). Ohne vorherige Registrie rung bei der stiftung ear darf ein Anbieter in Deutschland we der Batterien noch Elektro- und Elektronikgeräte in Verkehr bringen. 

Mit dem Wert *nein* werden Artikel gekennzeichnet, die nicht betroffen sind. 

Mit dem Wert *ja, unterliegt der Registrierungspflicht* werden Artikel gekennzeichnet, die der Registrierungspflicht gemäß Batteriegesetz (BattG) oder Elektro- und Elektronikgeräte gesetz (ElektroG) unterliegen und ordnungsgemäß bei der stiftung ear registriert wurden. 

Pflichtangabe, Format: F/1/NU1 

Wertebereich : 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja, Tierarzneimittel 

3 *→* ja, veterinärmedizintechnisches 

Produkt (VMTP) 

**. . .** 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 25 von 74**  
**PAC\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

50 **Kz. Transfusionsgesetz** \[TFG\] 

A2 **Kz. T-Rezept** 

\[T\_Rezept\] 

F2 **UDI-DI gemäß MDR** \[UDIDI\_MDR\] 

E2 **UN-Nummer** 

\[UNNr\] 

B0 **Unverbindliche** 

**Preisempfehlung** 

(in Cent, inkl. MwSt.) 

\[UVP\] 

D8 **Kz. Verbandmittel gemäß § 31 (1a) SGB V** 

\[Verbandmittel\] 

51 **Kz. Verfalldatum** 

\[Verfalldatum\] 

D2 **Verfalldatum der Charge, ab der im Pflichtbetrieb** 

**verifiziert wird** 

\[Veribeginn\_Pflicht\]   
Pflichtangabe, Format: F/1/NU1 

Information zur Anwendbarkeit des Begriffs Blutprodukt ge mäß *Transfusionsgesetz – TFG*; betroffene Artikel unterlie gen der Dokumentationspflicht. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Mit *ja* gekennzeichnete Artikel müssen gemäß § 3a (1) AMVV auf einem speziellen amtlichen Vordruck (*T-Rezept*) verschrieben werden. Diese Artikel können via VPV\_APO mit Verordnungsvorgaben (VOV\_APO) der Typen *3*, *4* und *5* verknüpft sein. 

Optionale Angabe, Format: V/120/AN1 

Der UDI-DI ist ein eindeutiger nummerischer oder alphanum merischer Code, der im Zuge der MDR (VO (EU) 2017/745) sowie der IVDR (VO (EU) 2017/746 vom 05.04.2017) der Identifikation von Medizinprodukten auf Artikelebene dient. Optionale Angabe, Format: F/4/NU3   
Jedes Sicherheitsdatenblatt (siehe auch ID 87) ist mit einer sogenannten UN-Nr. versehen. Die UN-Nr. beschreibt das Gut, von dem ein Gefährdungspotential ausgeht; z. B. 1170: „Ethanol (Ethylalkohol) oder Ethanol-Lösung“. 

Pflichtangabe, Format: V/10/NU1 

Es handelt sich um eine Preisempfehlung für den Verkauf an Endverbraucher. 

Der Feldinhalt *0* ist gleichbedeutend mit *keine Angabe*. Ist bei verschreibungspflichtigen Arzneimitteln mit *0 (keine Angabe)* belegt. Bei apothekenpflichtigen, nicht verschrei bungspflichtigen Arzneimitteln ist eine Angabe zusätzlich zum Apothekenverkaufspreis ID 04 möglich. 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Es handelt sich um eine vom Anbieter gemeldete Information; unplausible Konstellationen zu den Informationen im Artikel stamm Plus V sind möglich. 

Pflichtangabe, Format: F/1/NU1 

Information über die Aufbringung eines Verfalldatums auf den Artikel; ist ein Verfalldatum gegeben, kann die entsprechende Laufzeit dem Feld *Laufzeit in Monaten* entnommen werden. Wertebereich: 0 *→* keine Angabe 

1 *→* ohne Verfalldatum 

2 *→* mit Verfalldatum 

Optionale Angabe, Format: F/6/DTV 

Packungen mit einem Verfalldatum gleich oder größer als das in diesem Attribut angegebene nehmen am Pflichtbetrieb der Verifizierung teil. 

Zur Umsetzung dieser Information siehe ID E5\! **. . .** 

**Seite 26 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PAC\_APO** 

52 **Kz. Verkehrsfähigkeitsstatus** \[Verkehrsstatus\] 

53 **Kz. Verpackungsart** 

\[Verpackungsart\] 

54 **Kz. Verschreibungspflicht** \[Rezeptpflicht\] 

55 **Kz. Vertriebsstatus** 

\[Vertriebsstatus\]   
Pflichtangabe, Format: F/1/NU1 

Das Attribut trifft keine Aussage über eine Verpflichtung/Be reitschaft des Anbieters zur Rücknahme der Ware; nicht ver kehrsfähige Ware darf nicht abverkauft werden\! Wertebereich: 0 *→* keine Angabe 

1 *→* nicht verkehrsfähig 

2 *→* verkehrsfähig 

3 *→* Verkehrsfähigkeit in Prüfung 

Pflichtangabe, Format: V/2/NU1 

Packmitteltyp gemäß DIN 55 405: 2014-12 

Wertebereich: 0 *→* keine Angabe 

1 *→* unverpackt 

2 *→* Ballon 

3 *→* Becher   
... 

23 *→* Ampulle 

24 *→* Behälter 

25 *→* Box 

26 *→* Container 

27 *→* Gefäß 

28 *→* Karton 

30 *→* Mehrfachdosenbehälter 

Pflichtangabe, Format: F/1/NU1 

Information zur Anwendbarkeit des Begriffs Verschreibungs pflicht gemäß AMG und abgeleiteten Verordnungen bzw. MPAV. 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

3 *→* ja/Ausnahmeregel 

4 *→* nein/Ausnahmeregel 

Wert *3* ist wie folgt zu interpretieren: „Verschreibungspflichti ges Arzneimittel mit Ausnahmeregelung gemäß AMVV.“ Die Artikel sind mit einer Verordnungsvorgabe des Typs 8 verk nüpft. 

Wert *4* ist wie folgt zu interpretieren: „Nicht verschreibungs pflichtiges Medizinprodukt gemäß § 3 Absatz 1 Satz 2 MPAV. Das Medizinprodukt darf nur an Fachkreise nach § 3 Num mer 2 des Medizinprodukterecht-Durchführungsgesetzes ab gegeben werden, es sei denn, es wird eine ärztliche oder zahnärztliche Verschreibung vorgelegt.“ 

Pflichtangabe, Format: F/1/NU1 

Aussage über die Verfügbarkeit des Artikels beim Anbieter; keine Aussage über die Verfügbarkeit beim Großhandel; kei ne Aussage über die Verkehrsfähigkeit. 

Wertebereich: 0 *→* keine Angabe 

1 *→* außer Vertrieb 

2 *→* im Vertrieb 

3 *→* zurückgezogen 

Mit dem Wert *1* gekennzeichnete Artikel dürfen abverkauft werden, sofern dem nichts anderes entgegen steht (z. B. Ver kehrsfähigkeit). 

Wert *3* bedeutet, dass Lagerware nicht zur Abgabe und zum Abverkauf bestimmt ist. 

**. . .** 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 27 von 74**  
**PAC\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

56 **Kz. Vertriebsweg Apotheke** 

\[Vw\_Apo\] 

57 **Kz. Vertriebsweg Großhandel** 

\[Vw\_Grosshandel\] 

58 **Kz. Vertriebsweg Krankenhausapotheke** 

(im Sinne von KVA) 

\[Vw\_Krankenhausapo\] 

59 **Kz. Vertriebsweg sonstiger Einzelhandel** 

\[Vw\_sonstEinzelhandel\] 

60 **Verweis auf** 

**Nachfolge-PZN** 

\[PZN\_Nachfolger\] 

62 **Verweis auf** 

**PZN des Originals** 

\[PZN\_Original\] 

B7 **Kz. Wirkstoff nach EG-Richtlinie** 

\[Wirkstoff\_EG\_RL\] 

63 **Kz. zerbrechlich** 

\[Bruchgefahr\] 

95 **Kz. Zugelassenes Biozid** \[Biozid\]   
Pflichtangabe, Format: F/1/NU1 

Siehe Erläuterung zuR*Kz. Vertriebsweg sonstiger Einzel handel*\! 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: F/1/NU1 

Siehe Erläuterung zuR*Kz. Vertriebsweg sonstiger Einzel handel*\! 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: F/1/NU1 

Siehe Erläuterung zuR*Kz. Vertriebsweg sonstiger Einzel handel*\! 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

**Vertriebswege** (gilt auch für ID 56, 57 und 58): Die jeweilige Kennzeichnung gibt keinen Hinweis auf den ausschließlichen Vertrieb/Nichtvertrieb gegenüber anderen Kanälen\! RFremdschlüsselattribut, optionale Angabe,   
Format: F/8/PZ8 

Der referenzierte Artikel ist Nachfolger des aktuell ausge wählten Artikels; der Nachfolger ist eine sinnvolle Alternative mit gleicher oder ähnlicher Zweckbestimmung; Anbieter von Arzneimitteln oder Medizinprodukten sind gehalten, dies so eng auszulegen, dass die Arzneimittel-/Produktsicherheit ge währleistet ist. 

RFremdschlüsselattribut, optionale Angabe, 

Format: F/8/PZ8 

Verweis von Re-/Parallelimport auf Original; nur bezogen auf Fertigarzneimittel. Siehe auch ID 23\! 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Mit *ja* werden nach Artikel 1 Nr. 3a der *Richtlinie 2001/83/EG des Europäischen Parlaments und des Rates* Drogen und Chemikalien (ID 11) gekennzeichnet, die bei der Arzneimittel herstellung als Wirkstoffe verwendet werden. 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

**. . .** 

**Seite 28 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PAC\_APO** 

96 **Kz. Zugelassenes** 

**Pflanzenschutzmittel** 

\[Pflanzenschutzmittel\] 

G2 **Kz. Zulassung in Ausnahme fällen** 

\[Zulassung\_Ausnahme\] 

I6 **Zuzahlung für AM entspre chend § 130b Abs. 1c, nach § 61 Satz 1 SGB V (in Cent)** 

\[Zuz\_130b1c\_61\_1\] 

88 **Kz. Zuzahlungsbefreiung für preisgünstige** 

**Festbetragsartikel** 

\[Zuzfrei\_31SGB\_Feb\] 

80 **Kz. Zuzahlungsbefreiung gemäß § 31 (3) SGB V für** 

**Blut- und Harnteststreifen** 

\[Zuzfrei\_31SGB\_Tstr\]   
Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Pflichtangabe, Format: F/1/NU1 

Kennzeichen für Arzneimittel mit einer Genehmigung nach Artikel 14 Absatz 8 der Verordnung (EG) Nr. 726/2004 des Europäischen Parlaments und des Rates vom 31.03.2004 zur Festlegung von Gemeinschaftsverfahren für die Geneh 

migung und Überwachung von Human- und Tierarzneimitteln und zur Errichtung einer Europäischen Arzneimittel-Agentur (ABl. L 136 vom 30.4.2004, S. 1\) (Zulassung in Ausnahme fällen) 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

Optional, Format: V/10/NU1 

Für Arzneimittel, für welche eine Bestimmung nach § 130b (1c) SGB V zutrifft (ID I4), ist die auf Grundlage des vertraulichen Erstattungsbetrags berechnete Höhe der Zu zahlung nach § 61 Satz 1 SGB V angegeben. 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

*ja* wird bei Festbetragsartikeln (siehe *Schlüssel der Festbe tragstabelle* \[ID 46\] und Folgeverweise) gesetzt, deren Apo thekenverkaufspreis (ID 04) eine gemäß § 31 (3) SGB V fest gelegte Zuzahlungsbefreiungsgrenze nicht überschreitet. Ein GKV-Versicherter hat bei Verordnung dieses Artikels keine Zuzahlung zu leisten. 

Festbetragsartikel, für die zwar eine Zuzahlungsbefreiungs grenze existiert, deren Apothekenverkaufspreis diese aber überschreitet, werden mit *nein* gekennzeichnet. *keine Angabe* erhalten Nicht-Festbetragsartikel und Festbe tragsartikel, für die keine Zuzahlungsbefreiungsgrenze exis tiert. 

Das Attribut bezieht sich nicht auf die Zuzahlungsbefreiung gemäß § 62 SGB V. Das Attribut trifft auch keine Aussage zur Erstattungsfähigkeit. 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

*ja* bedeutet, dass gemäß § 31 (3) SGB V bei einer Abgabe von Blut- und Harnteststreifen zu Lasten der GKV der Versi cherte keine Zuzahlung zu leisten hat. Das Attribut bezieht sich nicht auf die Zuzahlungsbefreiung gemäß § 62 SGB V. Das Attribut trifft auch keine Aussage zur Erstattungsfähig keit. 

Dieses Kennzeichen darf nur ausgewertet werden, wenn sich die (vertragsabhängige) Information aus dem Modul ABDA Artikelstamm Plus V nicht ableiten lässt. Die dort hinterlegte Information ist vorrangig auszuwerten und anzuwenden. 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 29 von 74**  
**PAC\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**Erläuterungen zu ID 69** 

Eine Auswahltabelle fasst auf Basis der Anlage VII der AM-RL oder nach Anlage 1 des Rahmenvertra ges gemäß § 129 (2) SGB V eine Gruppe von Artikeln zusammen, die in Wirkstoff, Wirkstärke und Pa ckungsgröße gleich und in der Darreichungsform therapeutisch vergleichbar sind (*gleiche Packungs größe* bedeutet in diesem Fall grundsätzlich die Übereinstimmung in der N-Kennzeichnung; s. ID 03 in Datei PGR\_APO). Innerhalb einer Gruppe kann ein Austausch nach o. g. Rahmenvertrag vorgenom men werden, sofern dem keine anderweitigen Regelungen oder Vorgaben oder individuelle Bedenken entgegenstehen. Der in diesem Zusammenhang notwendige Vergleich der Indikationsbereiche von Ar tikeln wird durch Datei VPI\_APO in Verbindung mit INB\_APO ermöglicht. 

Hinweis: Der in den Auswahltabellen bereits berücksichtigte Teilaspekt der Austauschbarkeit von Dar reichungsformen ist für die in Anlage VII der AM-RL erfassten Fälle in VOV\_APO für Gruppierungs zwecke separat abgebildet (Datensätze mit dem Wert *10* in ID 03).Wird in Anlage VII der AM-RL keine Festlegung getroffen, gilt § 9 (3) Buchstabe d) Spiegelstrich eins nach o. g. Rahmenvertrag. 

**Seite 30 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PAC\_APO** 

**2.2 Gruppierung der Attribute nach Themenbereichen** 

Nachfolgende Übersicht dient der Orientierung, stellt aber keine verbindliche Vorgabe dar\! Die jeweils in Klammern nachgestellten Angaben verweisen entweder auf die entsprechende ID in PAC\_APO oder auf die Datei(en), in der/denen die betreffenden Inhalte abgelegt sind. 

• **Artikelgrunddaten** 

Artikelnummer des Anbieters (05) 

Artikeltyp (64) 

Global Trade Item Number (15) 

Kurzname (26) 

Langname (31) 

Langname, ungekürzt (67) 

National Trade Item Number (E8) 

Packungsgröße (PGR2\_APO, Typ *Anbieter*) 

Pharmacy Product Number (E9) 

Pharmazentralnummer (01) 

Schlüssel der Adresse des Anbieters (44) 

Schlüssel der Adresse des Herstellers (66) 

Schlüssel der Adresse des örtlichen Vertreters (F9) 

Schlüssel der Adresse des Zulassungsinhabers (F8) 

Schlüssel der Darreichungsform (45) 

Schlüssel der Warengruppe (47) 

Sortierbegriff zum Langname (48) 

UDI-DI gemäß MDR (F2) 

• **Preisinformationen** 

Abgabepreis des pharmazeutischen Unternehmers (18) 

Abgabepreis des pharmazeutischen Unternehmers gemäß § 78 (3a) Satz 1 AMG (C0) Kz. Ablösung des Abschlags nach § 130a (1)/(8) SGB V (E3) 

Apothekeneinkaufspreis (02) 

Apothekeneinkaufspreis auf Basis des Preises des pharmazeutischen Unternehmers (C3) Apothekenverkaufspreis (04) 

Apothekenverkaufspreis auf Basis des Preises des pharmazeutischen Unternehmers (C4) Kz. Arzneimittel mit altersgerechter Darreichungsform (H4) 

Kz. Arzneimittel mit aufgehobenem Festbetrag (H5) 

Kz. Arzneimittel mit Erstattungsbetrag nach § 130b SGB V (C1) 

Kz. Arzneimittel mit versorgungskritischem Wirkstoff nach BfArM-Liste (H6) 

Kz. Arzneimittel zur Behandlung von Kindern nach BfArM-Liste (H7) 

Kz. Bestimmung nach § 130b Abs. 1c SGB V (I4) 

Datum, ab dem der Abgabepreis des pharmazeutischen Unternehmers nach § 78 Abs. 3a Satz 1 AMG gilt (H8) 

Datum, ab dem der Abgabepreis des pharmazeutischen Unternehmers nach § 78 Abs. 3a Satz 1 AMG mit Bestimmung nach § 130b Abs. 1c SGB V gilt (I5) 

Datum, ab dem die Ablösung des Abschlags § 130a SGB V gilt (I0) 

Differenz zwischen dem Preis des pharmazeutischen Unternehmers und dem Abgabepreis ge mäß § 78 (3a) Satz 1 AMG (C5) 

Festbetrag (97) 

Festbetragsstufe (98) 

Kz. Hilfsmittel zum Verbrauch (77) 

Krankenhausapothekeneinkaufspreis des pharmazeutischen Unternehmers (F4) Krankenhauseinkaufspreis (24) 

Kz. Mehrwertsteuersatz (37) 

Packungsgrößenkennzeichnung (PGR\_APO) 

Preis des pharmazeutischen Unternehmers (C2) 

Preisstrukturmodell (G9) 

Rabattwert nach § 130a (2) SGB V (A0) 

Rabattwert zu Lasten des Anbieters (74) 

Rabattwert zu Lasten des Anbieters für ein Generikum (85) 

Rabattwert zu Lasten des Anbieters wegen Preismoratoriums (86) 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 31 von 74**  
**PAC\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

Kz. Rabatt zu Lasten der Apotheke (72) 

Schlüssel der Festbetragstabelle (46) 

Unverbindliche Preisempfehlung (B0) 

Zuzahlung für AM entsprechend § 130b Abs. 1c, nach § 61 Satz 1 SGB V (in Cent) (I6) Kz. Zuzahlungsbefreiung für Blut- und Harnteststreifen (80) 

Kz. Zuzahlungsbefreiung für preisgünstige Festbetragsartikel (88) 

• **Vertriebsinformationen** 

Kz. Vertriebsstatus (55) 

Kz. Verkehrsfähigkeitsstatus (52) 

Kz. Vertriebsweg Apotheke (56) 

Kz. Vertriebsweg Großhandel (57) 

Kz. Vertriebsweg Krankenhausapotheke (58) 

Kz. Vertriebsweg sonstiger Einzelhandel (59) 

• **Verweisinformationen** 

Verweis auf Nachfolge-PZN (60) 

Verweis auf PZN des Originals (62) 

• **Gültigkeitsdaten** 

. . . bei Änderung der Vertriebsinformation (I1) 

. . . bei Änderung eines Preises (21) 

• **Packungs- und Lagerungsinformationen** 

Breite (09) 

Kz. Eichung (12) 

Kz. feuchteempfindlich (16) 

Gewicht (17) 

Höhe (22) 

Kz. Kühlkette (25) 

Länge (27) 

Kz. lageempfindlich (28) 

Kz. Lagertemperatur beachten (I3) 

Lagertemperatur max. (29) 

Lagertemperatur min. (30) 

Laufzeit der Eichung (32) 

Laufzeit in Monaten (33) 

Kz. lichtempfindlich (34) 

Mindestbestellmenge (65) 

Kz. steril (B1) 

Kz. Verfalldatum (51) 

Kz. Verpackungsart (53) 

Kz. zerbrechlich (63) 

• **Verifizierungsinformationen** 

Hochladedatum der Verifizierungsinformationen für den Pflichtbetrieb (E5) 

Kz. securPharm Pilot (E4) 

Verfalldatum der Charge, ab der im Pflichtbetrieb verifiziert wird (D2) 

• **Rechtsinformationen** 

Kz. AMNOG-Verfahren (§ 35a SGB V) (G3) 

Kz. Apothekenpflicht (03) 

Kz. Artikel gemäß § 15 (1) Satz 2 ApBetrO (A5) 

Kz. Artikel gemäß § 15 (2) ApBetrO (A6) 

Kz. Arzneimittel (06) 

Kz. Arzneimittel für seltenes Leiden (G0) 

Kz. Arzneimittelpreisverordnung AMG (07) 

Kz. Arzneimittelpreisverordnung SGB V (76) 

Kz. ATMP (F5) 

Kz. Ausnahmeregelung gemäß § 51 AMG (83) 

Kz. Ausnahmeregelung gemäß § 52b (2) Satz 3 AMG (A3) 

Kz. Ausnahme von der Ersetzung (B3) 

**Seite 32 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PAC\_APO** 

Batterie-Registriernummer(H1) 

Kz. Bedingte Erstattung best. Fertigarzneimittel (78) sowie Verknüpfung mit Positionen der *OTC-Übersicht* (VPV\_APO) 

Kz. bedingte Zulassung (G1) 

Kz. Betäubungsmittel (08) 

Kz. Bezugnehmende Zulassung als Generikum (92) 

Biosimilargruppe (G6) 

Kz. Biotechnisch hergestelltes Fertigarzneimittel (93) 

Kz. Biotechnologisch hergestelltes Arzneimittel (C7) 

Kz. CMR-Gefahrstoff (E1) 

Kz. Diätetikum (89) 

Kz. Droge/Chemikalie (11) 

Kz. Elektro- und Elektronikgeräte-Stoff-Verordnung (B8) 

Kz. EU-Bio-Logo nach EG-Öko-Basisverordnung (A8) 

Kz. EU-Novel Food-Verordnung (F6) 

Kz. Explosivgrundstoff (C8) 

Importgruppennummer (B9) 

Indikationsbereiche (VPI\_APO/INB\_APO) 

Informationen zu Rabattverträgen nach § 130a (8) SGB V (PZG\_APO/GRU\_APO) Kz. In-vitro-Diagnostika-Klasse (E6) 

Kz. Kosmetikum nach EG-Verordnung (A9) 

Kz. Lebensmittel (90) 

Kz. Lifestyle-Medikament (81) 

Kz. MedCanG (I2) 

Kz. Medizinprodukt (36) 

Kz. Medizinprodukte-Klasse (E7) 

Kz. Medizinprodukt gemäß § 31 (1) Satz 2 SGB V (D9) 

Mehrfachvertriebsgruppe (F1) 

Kz. Tierarzneimittel-Abgabemengen-Register (TAR) (94) 

Kz. Nahrungsergänzungsmittel (91) 

Kz. Negativliste (40) 

„Pharmazentralnummer“ der Bundesopiumstelle (C6) 

Kz. Preisangabenverordnung (68) 

Ref\_Biosimilar (G7)   
Registrierungsnummer der stiftung elektro-altgeräte register® (ear) (D6)   
Kz. Re-/Parallelimport (23) 

Schlüssel der Auswahltabelle (69) 

Kz. Sicherheitsdatenblatt für Gefahrstoffe erforderlich (87) 

Kz. Sonstiges Produkt zur Wundbehandlung gemäß § 31 (1a) SGB V (F3) 

Kz. stiftung ear (H0) 

Kz. Tierarzneimittelgesetz (49) 

Kz. Transfusionsgesetz (50) 

Kz. T-Rezept (A2) 

UN-Nummer (E2) 

Kz. Verbandmittel gemäß § 31 (1a) SGB V (D8) 

Kz. Verkehrsfähigkeitsstatus (52) 

Verknüpfung mit Verordnungsvorgaben (VPV\_APO) 

Kz. Verschreibungspflicht (54) 

Kz. Wirkstoff nach EG-Richtlinie (B7) 

Kz. Zugelassenes Biozid (95) 

Kz. Zugelassenes Pflanzenschutzmittel (96) 

Kz. Zulassung in Ausnahmefällen (G2) 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 33 von 74**  
**PGR\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**3 Packungsgrößenkennzeichnungen** 

Dateiname: PGR\_APO 

Dateilangname: Packungsgroessen 

ABDATA-Dateinummer: 1044 

Die Datei beschreibt die Einstufung von Packungsgrößen gemäß Rechtsverordnung nach § 31 (4) Satz 1 SGB V (*Packungsgrößenverordnung – PackungsV*). 

**Attributdefinitionen** 

01 **Pharmazentralnummer** \[PZN\] 

02 **Packungskomponente** \[Packungskomponente\] 

03 **Einstufung** 

\[Einstufung\]   
Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: F/8/PZ8 

Primärschlüsselattribut, Pflichtangabe, Format: V/2/NU1 Bei Kombipackungen werden die Einzelkomponenten will kürlich durchnummeriert. 

Pflichtangabe, Format: V/2/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nicht betroffen 

2 *→* keine therapiegerechte 

Packungsgröße 

3 *→* N1 

4 *→* N2 

5 *→* N3 

*keine Angabe* bedeutet, dass die Packungsgröße kleiner als die größte vorgegebene Messzahl ist und gleichzeitig keiner der vorgegebenen Messzahlen entspricht 

*nicht betroffen* bedeutet, dass der Artikel nicht von der Pa ckungsV betroffen ist 

*keine therapiegerechte Packungsgröße* bedeutet, dass die Packungsgröße größer als die größte vorgegebene Messzahl ist 

**Seite 34 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PGR2\_APO** 

**4 Quantitative Packungsgrößenangaben** 

Dateiname: PGR2\_APO 

Dateilangname: Packungsgroessen\_2 

ABDATA-Dateinummer: 1223 

Neben der vom Anbieter gemeldeten Packungsgröße enthält die Datei bestimmten Verwendungs zwecken dienende alternative Packungsgrößen. 

**Attributdefinitionen** 

01 **Pharmazentralnummer** \[PZN\] 

02 **Zähler** 

\[Zaehler\] 

03 **Einheit der** 

**Packungsgröße** 

\[Einheit\] 

04 **Komponentennummer** \[Komponentennr\] 

05 **Typ der** 

**Packungsgrößenangabe** \[Typ\]   
Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: F/8/PZ8 

Primärschlüsselattribut, Pflichtangabe, Format: V/2/NU1 Das Attribut hat keine fachliche Bedeutung. 

Pflichtangabe, Format: V/2/AN1 

Wertebereich: cm *→* Zentimeter 

Fl *→* Flasche 

g *→* Gramm 

IE *→* Internationale Einheiten 

kg *→* Kilogramm 

l *→* Liter 

m *→* Meter 

mg *→* Milligramm 

ml *→* Milliliter 

mm *→* Millimeter 

P *→* Packung 

Sp *→* Sprühstöße 

St *→* Stück   
µg *→* Mikrogramm 

Siehe auch unten *Zahlenwert der Packungsgröße* (ID 06)\! Optionale Angabe, Format: V/2/NU1   
Das Attribut ermöglicht die Angabe von Packungsgrößen zu den Komponenten von Kombipackungen. Der zugehöri ge Datensatz in FAK\_DB (ABDA-Datenbank) kann mit der Kombination des mit der PZN verknüpften Fertigarzneimit telschlüssels (via PAE\_DB) und der Komponentennummer selektiert werden. 

Das Attribut ist bis auf Weiteres nicht belegt. 

Pflichtangabe, Format: V/2/NU1 

Wertebereich: 1 *→* Anbieter 

2 *→* FAM-Vergleich 

Zu jeder PZN existiert genau eine Packungsgröße des Typs *1*; es handelt sich um die vom Anbieter gemeldete In formation. 

Zu jeder PZN kann maximal eine Packungsgröße des Typs *2* existieren; sie erleichtert die Vergleichbarkeit von Artikeln beispielsweise im Rahmen von Aut idem. 

**. . .** 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 35 von 74**  
**PGR2\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

06 **Zahlenwert der Packungsgröße** 

\[Zahl\]   
Pflichtangabe, Format: V/20/MPG 

Zusammen mit *Einheit der Packungsgröße* (s. o. ID 03\) wird die Packungsgröße gebildet; bei gebündelten Packungen (z. B. Klinikware) kann auch die Anzahl der gebündelten Ein heiten angegeben sein; dabei bezieht sich der in ID 03 abge legte Wert immer auf den letztgenannten Faktor (z. B. *20* bei *5x20 ml*). 

**Seite 36 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. PAT\_APO** 

**5 Textinformationen zu Packungen** 

Dateiname: PAT\_APO 

Datei-Langname: Packungstexte 

ABDATA-Dateinummer: 1328 

PAT\_APO enthält Volltextinformationen zur Zweckbestimmung und Zusammensetzung von Medizinpro dukten bzw. In-vitro-Diagnostika sowie Diätetika, entsprechend den Angaben des Anbieters. 

**Attributdefinitionen** 

01 **Pharmazentralnummer** \[PZN\] 

02 **Textfeldtyp** 

\[Textfeld\] 

03 **Text** 

\[Text\] 

04 **Dateiname** 

\[Dateiname\] 

05 **Dateiendung** 

\[Dateiendung\]   
Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: F/8/PZ8 

Primärschlüsselattribut, Pflichtangabe, Format: V/2/NU1 Wertebereich: 1 *→* Zweckbestimmung für ein Medizinprodukt/In-vitro 

Diagnostikum laut Anbieter 

2 *→* Zusammensetzung eines 

Medizinproduktes/In-vitro 

Diagnostikums nach Art und Menge 

laut Anbieter 

Pflichtfeld, Format: U/-/B64 

Enthält für die jeweilige PZN die in Base64 codierten Anga ben bezogen auf den entsprechenden Textfeldtypen. Die Rückübersetzung aus Base64 führt zu einem UTF-8- codierten String. 

Ist ID 05 nicht belegt, ist der Feldinhalt als reiner Textstring aufzufassen. Ansonsten ist er in eine Datei mit der in ID 05 angegebenen Endung und gegebenenfalls dem in ID 04 ab gelegten Namen zu überführen. 

Optionale Angabe, Format: V/250/AN1 

Enthält einen Dateinamen; die zugehörige Dateiendung ist in ID 05 angegeben. 

Optionale Angabe, Format: V/10/AN3 

Siehe Erläuterung zu ID 03\. 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 37 von 74**  
**KLB\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**6 Klinikbausteine** 

Dateiname: KLB\_APO 

Dateilangname: Klinikbausteine 

ABDATA-Dateinummer: 1134 

Ein Klinikbaustein ist eine Fertigarzneimittelpackung als Teileinheit einer Klinikpackung. Die Angaben zu Klinikpackungen sind in PAC\_APO integriert. Die Zuordnung von Klinikbausteinen zu Klinikpackun gen erfolgt mittels VPK\_APO; sie muss nicht zwangsläufig angegeben sein. 

**Attributdefinitionen** 

01 **Pharmazentralnummer** \[PZN\] 

02 **Einheit der** 

**Packungsgröße** 

\[Einheit\] 

03 **Kurzname** 

\[Kurzname\] 

04 **Menge der** 

**Packungsgröße** 

\[Menge\] 

05 **Schlüssel der Adresse des Anbieters** 

\[Key\_ADR\_Anbieter\] 

06 **Schlüssel der** 

**Darreichungsform** 

\[Key\_DAR\] 

07 **Kz. Verkehrsfähigkeitsstatus** \[Verkehrsstatus\] 

08 **Kz. Vertriebsstatus** 

\[Vertriebsstatus\]   
Primärschlüsselattribut, Pflichtangabe, Format: F/8/PZ8 

Pflichtangabe, Format: V/2/AN1 

Wertebereich: sieheR*Einheit der Packungsgröße* in PGR2\_APO\! 

Pflichtangabe, Format: V/26/AN1 

Pflichtangabe, Format: V/7/MPG 

Mengenangabe zur Verbrauchereinheit; zusammen mit *Ein heit der Packungsgröße* wird die Packungsgröße gebildet. RFremdschlüsselattribut, Pflichtangabe,   
Format: V/5/NU1 

„Anbieter“ steht im Sinne von „Inverkehrbringer“, „PZN Anmelder“. 

RFremdschlüsselattribut, optionale Angabe, Format: F/3/AL1 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: sieheR*Kz. Verkehrsfähigkeitsstatus* in PAC\_APO\! 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: sieheR*Kz. Vertriebsstatus* in PAC\_APO\! 

**Seite 38 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. VPK\_APO** 

**7 Verknüpfung PZN mit Klinikbausteinen** 

Dateiname: VPK\_APO 

Dateilangname: Verknuepfung\_PAC\_KLB 

ABDATA-Dateinummer: 1136 

Siehe einleitende Bemerkungen zu KLB\_APO\! 

**Attributdefinitionen** 

01 **Pharmazentralnummer der Klinikpackung** 

\[PZN\_Klinikpackung\] 

02 **Pharmazentralnummer des Klinikbausteins** 

\[PZN\_Klinikbaustein\]   
Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: F/8/PZ8 

Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: F/8/PZ8 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 39 von 74**  
**ADR\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**8 Adressen** 

Dateiname: ADR\_APO 

Dateilangname: Adressen 

ABDATA-Dateinummer: 1001 

ADR\_APO enthält in erster Linie die von der IFA im Zusammenhang mit den Artikeldaten veröffentlich ten Firmenadressen. 

**Attributdefinitionen** 

01 **Schlüssel der Adresse** \[Key\_ADR\] 

02 **Firmenname** 

\[Firmenname\] 

22 **Firmenname 2** 

\[Firmenname\_2\] 

23 **Firmenname 3** 

\[Firmenname\_3\] 

24 **Firmenname (ABDATA)** \[Firmenname\_ABDATA\] 

20 **Großhandelserlaubnis gemäß § 52a AMG** 

\[Gh\_Erlaubnis\_52aAMG\] 

05 **Hausnummer bis** \[Hausnr\_bis\] 

06 **Hausnummer bis Zusatz** \[Hausnr\_bis\_Zusatz\] 

03 **Hausnummer von** \[Hausnr\_von\] 

04 **Hausnummer von Zusatz** \[Hausnr\_von\_Zusatz\] 

21 **Herstellungserlaubnis gemäß § 13 AMG** 

\[Her\_Erlaubnis\_13AMG\]   
Primärschlüsselattribut, Pflichtangabe, Format: V/5/NU1 Pflichtangabe, Format: V/80/AN1 

Optionale Angabe, Format: V/40/AN1 

Erweiterung zum Attribut *Firmenname* (ID 02). Optionale Angabe, Format: V/40/AN1   
Erweiterung zum Attribut *Firmenname* (ID 02). Pflichtangabe, Format: V/105/AN1   
Von ABDATA Pharma-Daten-Service gepflegte Firmenbe zeichnung, in der auch Zusätze (z. B. *GmbH*) enthalten sein können. 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

3 *→* nicht erforderlich 

*ja* bedeutet, dass der Anbieter eine Erlaubnis gemäß § 52a AMG für das Betreiben eines Großhandels mit Arzneimitteln, Testsera oder Testantigenen besitzt. *nein* bzw. *nicht erforder lich* sind entsprechend zu interpretieren. 

Optionale Angabe, Format: V/4/AN1 

Optionale Angabe, Format: V/4/AN1 

Optionale Angabe, Format: V/4/AN1 

Optionale Angabe, Format: V/4/AN1 

Pflichtangabe, Format: F/1/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nein 

2 *→* ja 

3 *→* nicht erforderlich 

*ja* bedeutet, dass der Anbieter eine Erlaubnis gemäß § 13 AMG für die Herstellung von Arzneimitteln, Testsera, Testan tigenen, Wirkstoffen oder anderer zur Arzneimittelherstellung bestimmter Stoffe menschlicher Herkunft besitzt. *nein* bzw. *nicht erforderlich* sind entsprechend zu interpretieren. **. . .** 

**Seite 40 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. ADR\_APO** 

07 **Länderkennzeichen** \[Key\_LAE\] 

29 **Ländername Postfach (Kurzform)** 

\[Land\_Postfach\] 

09 **Ort Postfach** 

\[Ort\_Postfach\] 

25 **Ort Postfach (ABDATA)** \[Ort\_Postfach\_ABDATA\] 

08 **Ort Zustellung** 

\[Ort\_Zustellung\] 

26 **Ort Zustellung (ABDATA)** \[Ort\_Zustell\_ABDATA\] 

10 **Postfach** 

\[Postfach\] 

13 **Postleitzahl Großkunde** \[PLZ\_Grosskunde\] 

12 **Postleitzahl Postfach** \[PLZ\_Postfach\] 

11 **Postleitzahl Zustellung** \[PLZ\_Zustellung\] 

28 **Registrierungsnummer gemäß** 

***Verpackungsgesetz –*** 

***VerpackG*** 

\[Regnr\_VerpackG\] 

14 **Sortiername zum Firmenname** 

\[Sortiername\] 

15 **Straße** 

\[Strasse\] 

30 **Straße 2** 

\[Strasse\_2\] 

31 **Straße 3** 

\[Strasse\_3\] 

32 **Straße 4** 

\[Strasse\_4\] 

27 **Straße (ABDATA)** \[Strasse\_ABDATA\]   
Pflichtangabe, Format: V/3/AL1 

Wertebereich: A *→* Österreich 

AM *→* Armenien 

AUS *→* Australien   
... 

Optionale Angabe, Format: V/3/AL1 

Wertebereich: A *→* Österreich 

AM *→* Armenien 

AUS *→* Australien   
... 

Optionale Angabe, Format: V/40/AN1 

Optionale Angabe, Format: V/50/AN1 

Von ABDATA Pharma-Daten-Service gepflegte Ortsangabe der Postfachadresse (Abholadresse). 

Optionale Angabe, Format: V/40/AN1 

Optionale Angabe, Format: V/50/AN1 

Von ABDATA Pharma-Daten-Service gepflegte Ortsangabe der Zustelladresse. 

Optionale Angabe, Format: V/10/AN1 

Optionale Angabe, Format: V/10/AN1 

Optionale Angabe, Format: V/10/AN1 

Optionale Angabe, Format: V/10/AN1 

Optionale Angabe, Format: F/15/AN2 

Apotheken fallen als Vertreiber grundsätzlich in den Anwen dungsbereich des Verpackungsgesetzes, daher gilt auch für die Apotheke die grundsätzliche Pflicht, nur noch systembe teiligungspflichtige Verpackungen von registrierten Herstel 

lern in den Verkehr zu bringen. 

Die 15-stellige Registrierungsnummer besteht aus dem Präfix *DE*, gefolgt von dreizehn Ziffern. 

Pflichtangabe, Format: V/40/AN3 

Optionale Angabe, Format: V/40/AN1 

Optionale Angabe, Format: V/40/AN1 

Optionale Angabe, Format: V/40/AN1 

Optionale Angabe, Format: V/40/AN1 

Optionale Angabe, Format: V/200/AN1 

Von ABDATA Pharma-Daten-Service gepflegter Straßenna me, ggf. mit Hausnummer. 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 41 von 74**  
**SER\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**9 Service-Lines** 

Dateiname: SER\_APO 

Dateilangname: Service-Lines 

ABDATA-Dateinummer: 1006 

Zu den in ADR\_APO enthaltenen postalischen Firmenadressen sind in SER\_APO optional weitere Kommunikations- oder Informationswege abgelegt. 

**Attributdefinitionen** 

01 **Schlüssel der Adresse** \[Key\_ADR\] 

02 **Zähler** 

\[Zaehler\] 

03 **Adresse** 

\[Adresse\] 

04 **Adresstyp** 

\[Adresstyp\] 

05 **Service** 

\[Service\] 

06 **Zusatzinfo zum Service** \[Zusatzinfo\]   
Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: V/5/NU1 

Primärschlüsselattribut, Pflichtangabe, Format: V/2/NU1 Das Attribut hat keine fachliche Bedeutung. Pflichtangabe, Format: V/250/AN1   
Angabe der Telefon-, Telefaxnummer usw. 

Pflichtangabe, Format: V/2/NU1 

Wertebereich: 0 *→* Telefonnummer 

1 *→* Telefaxnummer 

2 *→* E-Mail-Adresse 

3 *→* URL 

Pflichtangabe, Format: V/2/NU1 

Wertebereich: 0 *→* nicht näher spezifiziert 1 *→* allgemein 

2 *→* Bestellungen 

3 *→* Med.-wiss. Information 

5 *→* Vertrieb 

6 *→* Lager, Versand, Auslieferung 

7 *→* Logistik 

8 *→* Retouren 

Optionale Angabe, Format: V/250/AN1 

**Seite 42 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. DAR\_APO** 

**10 Darreichungsformen** 

Dateiname: DAR\_APO 

Dateilangname: Darreichungsformen 

ABDATA-Dateinummer: 1002 

DAR\_APO enthält die von der IFA im Zusammenhang mit den Artikeldaten veröffentlichten Darrei chungsformen. 

**Attributdefinitionen** 

01 **Schlüssel der Darreichungsform** 

\[Key\_DAR\] 

03 **Abkürzung der Darreichungsform** 

\[Abkuerzung\] 

02 **Bezeichnung der Darreichungsform** 

\[Name\]   
Primärschlüsselattribut, Pflichtangabe, Format: F/3/AL1 Pflichtangabe, Format: V/5/AN1 

Pflichtangabe, Format: V/110/AN1 

Die Attributwerte dienen anbieterseits der Beschreibung der Handelsform pharmazeutischer Produkte. In die Begriffsbil dung können neben der Galenik weitere Aspekte eingehen, 

wodurch Recherchen nach vergleichbaren Produkten er schwert werden. Dazu besser geeignet ist die in FAP\_DB der ABDA-Datenbank erläuterte *Darreichungsformstruktur*. Die Vorgabe für die Aut idem-Recherche gemäß Rahmenvertrag nach § 129 (2) SGB V bleibt davon unberührt. 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 43 von 74**  
**WAR\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**11 Warengruppen** 

Dateiname: WAR\_APO 

Dateilangname: Warengruppen 

ABDATA-Dateinummer: 1008 

Der Warengruppenschlüssel ist strukturiert und in Anlehnung an den ATC-Code alphanumerisch auf gebaut. Die Einteilung umfaßt z. Zt. zwei Gruppen, die sich durch das erste Zeichen unterscheiden: *A* \= Sortiment der im Wesentlichen apothekenpflichtigen Fertigarzneimittel, *B* \= restliches apothe kenübliches Sortiment. Im A-Bereich entspricht die dem *A* folgende Zeichenkette einem WHO-ATC Code. 

Für die Entschlüsselung beim Einzelprodukt ist die Anzeige der Bezeichnung mit den jeweils überge ordneten Stufen notwendig. Das Verweisfeld wurde eingeführt, um Überschneidungen zwischen ATC Code und dem Schlüssel für das Randsortiment zu markieren (im Sinne von „siehe auch“). 

**Attributdefinitionen** 

01 **Schlüssel der** 

**Warengruppe** 

\[Key\_WAR\] 

02 **Name der Warengruppe** \[Name\] 

03 **Verweis auf andere Warengruppe** 

\[Key\_WAR\_Verweis\]   
Primärschlüsselattribut, Pflichtangabe, Format: V/8/WGS 

Pflichtangabe, Format: V/120/AN1 

RFremdschlüsselattribut, optionale Angabe, Format: V/8/WGS 

**Seite 44 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. INB\_APO** 

**12 Indikationsbereiche** 

Dateiname: INB\_APO 

Dateilangname: Indikationsbereiche 

ABDATA-Dateinummer: 1022 

Die in dieser Datei definierten Indikationsbereiche werden zur Umsetzung von Aut idem gemäß § 129 SGB V via VPI\_APO Artikeln zugeordnet. 

**Attributdefinitionen** 

01 **Schlüssel des Indikationsbereichs** 

\[Key\_INB\] 

02 **Beschreibung des Indikationsbereichs** 

\[Beschreibung\]   
Primärschlüsselattribut, Pflichtangabe, Format: V/5/NU1 Pflichtangabe, Format: V/200/AN1 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 45 von 74**  
**VPI\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**13 Indikationsbereiche von Artikeln** 

Dateiname: VPI\_APO 

Dateilangname: Verknuepfung\_PAC\_INB 

ABDATA-Dateinummer: 1023 

VPI\_APO ermöglicht einen Vergleich der Indikationsbereiche von Artikeln im Rahmen von Aut idem gemäß § 129 SGB V. 

Hinweis: Die Verknüpfung von Artikeln mit Indikationsbereichen ist nicht auf die Artikel beschränkt, de nen via ID 69 in PAC\_APO eine Auswahltabelle zugeordnet ist, sondern erstreckt sich auf den gesam ten generikafähigen Markt. 

**Attributdefinitionen** 

01 **Pharmazentralnummer** \[PZN\] 

02 **Schlüssel des** 

**Indikationsbereichs** 

\[Key\_INB\]   
Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: F/8/PZ8 

Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: V/5/NU1 

**Seite 46 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. GRU\_APO** 

**14 Gruppen betr. die Rabattinformationen nach § 130a (8) SGB V** 

Dateiname: GRU\_APO 

Dateilangname: Gruppen 

ABDATA-Dateinummer: 1148 

GRU\_APO bildet in Verbindung mit IKZ\_APO, IZG\_APO und PZG\_APO die Rabattinformationen nach § 130a (8) SGB V in Zusammenhang mit § 31 (2) und (3) Satz 5 SGB V sowie den Regelungen des Rahmenvertrages nach § 129 (2) SGB V ab. 

**Attributdefinitionen** 

01 **Schlüssel der Gruppe** \[Key\_GRU\] 

02 **Mehrkostenverzicht** \[Mehrkostenverzicht\] 

04 **Vorrang bei der Aut idem-Auswahl** 

\[Vorrang\_Aut\_idem\] 

03 **Zuzahlungsfaktor** (in Prozent) 

\[Zuzahlungsfaktor\]   
Primärschlüsselattribut, Pflichtangabe, Format: V/4/NU1 Ein Attributwert fasst rein formal alle IK zusammen, deren jeweils zugeordnete PZN identisch sind; ein Bezug zu real existierenden Rabattverträgen besteht nicht. Die zu einem bestimmten Gültigkeitstermin unter einem Gruppenschlüssel zusammengefassten IK und PZN können zu nachfolgenden Gültigkeitsterminen einem anderen Gruppenschlüssel zuge ordnet sein. 

Pflichtangabe, Format: V/2/NU1 

Wertebereich: 0 *→* keine Angabe 

1 *→* nicht betroffen 

2 *→* nein 

3 *→* ja 

Ist Wert *3* gesetzt und sowohl das IK der Krankenkasse des gesetzlich Versicherten als auch die verordnete PZN dem be treffenden Gruppenschlüssel zugeordnet (via IZG\_APO bzw. PZG\_APO), entfällt die Zahlung eines möglichen Mehrkos tenanteils für den Versicherten (Mehrkosten ist der Betrag, um den der Apothekenverkaufspreis \[PAC\_APO, ID 04\] ei ner Fertigarzneimittelpackung den zugeordneten Festbetrag \[PAC\_APO, ID 97\] übersteigt). In diesem Fall wird die Verord nung zum Apothekenverkaufspreis und nicht zum Festbetrag abgerechnet. Hinweis: Die Werte *0* und *1* sind bis auf Weite res nicht relevant. 

Pflichtangabe, Format: V/2/NU1 

Wertebereich: 0 *→* nein 

1 *→* ja 

Aus den derzeitigen Regelungen des Rahmenvertrages nach § 129 (2) SGB V ergibt sich ausschließlich die Vergabe des Wertes *1*. 

Optionale Angabe, Format: V/3/NU1 

Der regulär ermittelte Zuzahlungsbetrag ist mit dem Prozent wert zu multiplizieren. Beispielsweise bedeutet der Eintrag *50* bzw. *0*, dass sich die Patientenzuzahlung auf 50% ermäßigt bzw. gänzlich entfällt. Hinweis: Das Feld ist bis auf Weiteres immer belegt. 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 47 von 74**  
**IKZ\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**15 Institutionskennzeichen** 

Dateiname: IKZ\_APO 

Dateilangname: Institutionskz 

ABDATA-Dateinummer: 1146 

IKZ\_APO listet Institutionskennzeichen von Krankenkassen, die mit pharmazeutischen Unternehmen Rabattverträge gemäß § 130a (8) SGB V abgeschlossen haben (siehe GRU\_APO). Weiterhin sind die in Zusammenhang mit dem Impfstoffabschlag gemäß § 130a (2) SGB V relevanten Institutionskennzei chen integriert (siehe IAE\_APO). 

**Attributdefinitionen** 

01 **Institutionskennzeichen** \[IK\] 

02 **Name der Institution** \[Name\]   
Primärschlüsselattribut, Pflichtangabe, Format: F/9/IKZ Pflichtangabe, Format: V/50/AN1 

**Seite 48 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. IZG\_APO** 

**16 Zuordnung von Institutionskennzeichen zu Gruppen** 

Dateiname: IZG\_APO 

Dateilangname: IK\_zu\_Gruppen 

ABDATA-Dateinummer: 1147 

IZG\_APO enthält rein formale Gruppierungen von Institutionskennzeichen, denen im Zusammenhang mit Rabattverträgen gemäß § 130a (8) SGB V derselbe Pool von Artikeln zugeordnet ist. 

**Attributdefinitionen** 

01 **Schlüssel der Gruppe** \[Key\_GRU\] 

02 **Institutionskennzeichen** \[IK\]   
Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: V/4/NU1 

Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: F/9/IKZ 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 49 von 74**  
**PZG\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**17 Zuordnung von Artikeln zu Gruppen** 

Dateiname: PZG\_APO 

Dateilangname: PZN\_zu\_Gruppen 

ABDATA-Dateinummer: 1149 

PZG\_APO enthält rein formale Gruppierungen von Artikeln, die im Zusammenhang mit Rabattver trägen gemäß § 130a (8) SGB V demselben Pool von Institutionskennzeichen zugeordnet sind. 

**Attributdefinitionen** 

01 **Schlüssel der Gruppe** \[Key\_GRU\] 

02 **Pharmazentralnummer** \[PZN\]   
Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: V/4/NU1 

Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: F/8/PZ8 

**Seite 50 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. IAE\_APO** 

**18 Daten zum Entfall des Impfstoffabschlags** 

**nach § 130a (2) SGB V** 

Dateiname: IAE\_APO 

Dateilangname: Impfstoffabschl\_entf 

ABDATA-Dateinummer: 1327 

IAE\_APO enthält die *Ausnahmen*(\!) von der grundsätzlichen Regel, den Impfstoffabschlag nach § 130a (2) SGB V immer zu berücksichtigen, wenn für die betreffende PZN sowohl der Abschlagswert des Rabattes als auch der Abgabepreis des pharmazeutischen Unternehmers ungleich *0* ist (siehe auch unten stehende Erläuterung). 

**Attributdefinitionen** 

01 **Pharmazentralnummer** \[PZN\] 

02 **Institutionskennzeichen** \[IK\] 

03 **Regionalkennzeichen** \[Regionalkz\]   
Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: F/8/PZ8 

Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: F/9/IKZ 

Primärschlüsselattribut, Pflichtangabe, Format: V/2/NU1 Wertebereich: 1 *→* Baden-Württemberg 2 *→* Bayern 

3 *→* Berlin 

4 *→* Brandenburg 

5 *→* Bremen 

6 *→* Hamburg 

7 *→* Hessen 

8 *→* Mecklenburg-Vorpommern 

9 *→* Niedersachsen 

10 *→* Nordrhein 

11 *→* Rheinland-Pfalz 

12 *→* Saarland 

13 *→* Sachsen 

14 *→* Sachsen-Anhalt 

15 *→* Schleswig-Holstein 

16 *→* Thüringen 

17 *→* Westfalen-Lippe 

18 *→* bundesweit 

Wenn auf eine Verschreibung zu Lasten der GKV die Kombination aus der PZN des abzurechnenden Artikels, dem IK („Kassen-Nr.“) und dem gemäß Wertebereich der ID 03 verschlüsselten Sitz der ab rechnenden Apotheke auf einen Datensatz in IAE\_APO zutrifft, wird der in ID A0 der PAC\_APO über mittelte Wert vom Abgabepreis des pharmazeutischen Unternehmers (ID 18 in PAC\_APO) ausnahms weise *nicht* abgezogen. 

Wenn in PAC\_APO für die abzurechnende PZN in ID A0 und/oder 18 der Wert *0* angegeben ist, entfällt die Suche in IAE\_APO. 

Der Abzug entfällt unabhängig vom Sitz der Apotheke, wenn die Kombination aus der PZN des abzu rechnenden Artikels und dem IK auf einen Datensatz mit dem Regionalkennzeichen *18* zutrifft. 

Hinweis: Wenn im Anwendungsprogramm der Sitz der Apotheke bereits mit einem Wert aus Datei *LND* des Moduls *Artikelstamm Plus V* voreingestellt ist, kann dieser auch in dem hier beschriebenen Zu sammenhang verwendet werden, da der Wertebereich des Regionalkennzeichens mit dem der Datei *LND* übereinstimmt. 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 51 von 74**  
**VOV\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**19 Verordnungsvorgaben** 

Dateiname: VOV\_APO 

Datei-Langname: Verordnungsvorgaben 

ABDATA-Dateinummer: 1192 

In VOV\_APO werden die Inhalte von Regelungen betreffend die Verordnung von Artikeln unterschied licher Produktkategorien (Arzneimittel, Medizinprodukte usw.) abgelegt. Die Zuordnung der Verord nungsvorgaben zu konkreten Artikeln erfolgt in Datei VPV\_APO. 

**Attributdefinitionen** 

01 **Schlüssel der** 

**Verordnungsvorgabe** 

\[Key\_VOV\] 

02 **Text der** 

**Verordnungsvorgabe** 

\[Text\] 

03 **Typ der** 

**Verordnungsvorgabe** 

\[Typ\] 

04 **Dokumentschlüssel** \[Key\_DOK\]   
Primärschlüsselattribut, Pflichtangabe, Format: V/8/NU1 

Optionale Angabe, Format: U/-/AN1 

Ausgenommen Datensätze mit den Werten *9*, *10*, *19* und *23* in *Typ der Verordnungsvorgabe* (ID 03\) ist die Belegung des Attributs Pflicht. 

Pflichtangabe, Format: V/2/NU1 

Der Wertebereich ist im Anschluss an diese Tabelle aufge führt. 

RFremdschlüsselattribut, optionale Angabe, Format: V/10/NU1 

Das Attribut ist derzeit ausschließlich in Datensätzen mit den Werten 9, 19 und 23 in *Typ der Verordnungsvorgabe* (ID 03\) belegt, womit der in DOK\_APO enthaltene offizielle Text des 

Therapiehinweises bzw. des Beschlusses zur Nutzenbe wertung bzw. die jeweilige Anlage 1 zur Vereinbarung nach § 130b (1) S. 1 SGB V zwischen dem GKV-Spitzenverband und dem pharmazeutischen Unternehmer referenziert wird. Ferner kann Typ 22 mit Dokumenten verknüpft sein. 

**Der Wertebereich des Attributs *Typ der Verordnungsvorgabe* (ID 03\) Wert Bedeutung Erläuterung**   
1 Anl. V AM-RL 

(verordnungsfähige Medizin produkte)   
Betroffen sind Medizinprodukte, die gemäß Anla ge V zum Abschnitt *Verordnungsfähigkeit von Medi zinprodukten* der AM-RL zu Lasten der GKV ver ordnungsfähig sind. Es handelt sich hierbei nur Medizinprodukte, die vormals als Arzneimittel in Verkehr gebracht wurden (bzw. worden wären); d. h. es wird keine generelle Aussage zur Erstat tungsfähigkeit von Medizinprodukten getroffen. So sind z. B. Verband- und Hilfsmittel grundsätzlich nicht zugeordnet, ohne dass an dieser Stelle deren Verordnungs- und Erstattungsfähigkeit angezweifelt wird. 

**. . .** 

**Seite 52 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. VOV\_APO** 

**Wert Bedeutung Erläuterung** 

3 Verschreibungsgültigkeit Verordnungsvorgaben dieser Typen stehen im 

4 Verschreibungshöchstmengen 5 Verschreibungsbesonderheiten   
Kontext zu gesetzlichen und amtlichen Vorgaben, die bei der Verordnung und Abgabe von Arzneimitteln mit bestimmten Wirkstoffen (z. B. Thalidomid, Betäubungsmittel, Lifestyle Medikamente) zu beachten sind; siehe auch PAC\_APO, ID A2, 08 bzw. 81\! 

7 Anl. I AM-RL (OTC-Übersicht) Ausnahmeregelungen betreffend die GKV Erstattung apothekenpflichtiger, nicht verschrei 

bungspflichtiger Fertigarzneimittel gemäß Anlage I 

AM-RL. 

8 Verschreibungsausnahmen Mit Verordnungsvorgaben dieses Typs in VPV\_APO verknüpfte Artikel unterliegen in der AMVV oder im 

BtMG besonderen Ausnahmeregelungen. 

9 Anl. IV AM-RL (Therapiehinweise)   
Therapiehinweise nach § 92 (2) Satz 7 SGB V in Verbindung mit § 17 AM-RL zur wirtschaftlichen Ver ordnungsweise von Arzneimitteln; im Allgemeinen referenzieren diese Datensätze eine in DOK\_APO enthaltene Datei mit dem offiziellen Text. Zusätz lich kann *Text der Verordnungsvorgabe* (s. o. ID 02) belegt sein. 

10 Anl. VII AM-RL, Teil A (Aut idem) Hinweise zur Austauschbarkeit von Darreichungsfor men gemäß § 129 (1a) SGB V; d. h. die mit Verord 

nungsvorgaben dieses Typs in VPV\_APO verknüpf 

ten Artikel gelten bei Wirkstoffgleichheit bezüglich 

ihrer Darreichungsformen als austauschbar. In An 

wendungsprogrammen darf Typ *10* nur technisch 

in Arzneimittellisten zur Gruppierung von PZN nach 

austauschbaren Darreichungsformen verwendet 

werden; er ist nicht für die Umsetzung der Aut idem 

Regelung geeignet, weil in diesem Zusammenhang 

zusätzlich Angaben zur Wirkstärke und Packungs 

größe einfließen (siehe hierzu Erläuterungen zu 

PAC\_APO.Key\_AUS)\! *Text der Verordnungsvorga* 

*be* (s. o. ID 02) ist unter Typ *10* derzeit nicht belegt. 

11 Anl. III AM-RL: Verordnungsaus schlüsse verschreibungspfl. Arz neimittel gg. „Bagatellerkrankun gen“ 

12 Anl. III AM-RL: Verordnungsaus schlüsse („Negativliste“) 

13 Anl. III AM-RL: Verordnungsaus schlüsse nach AM-RL 

14 Anl. III AM-RL: Verordnungsein schränkungen nach AM-RL 

15 Anl. III AM-RL: Verordnung nicht verschreibungspfl. Arzneimittel (Gefährdung Kdr., Jugendl.)   
Rechtsgrundlage: § 34 (1) Satz 6 SGB V, § 13 AM RL 

Grundlage: Rechtsverordnung nach § 34 (3) SGB V 

Rechtsgrundlage: § 92 (1) Satz 1 Halbsatz 3 SGB V in Verbindung mit § 16 (1) und (2) AM-RL Rechtsgrundlage siehe Typ *13*\! 

Dieser Typ umfasst Hinweise zur Verordnungsfähig keit nicht verschreibungspflichtiger Arzneimittel für Kinder bis zum vollendeten 12\. Lebensjahr und für Jugendliche mit Entwicklungsstörungen bis zum vollendeten 18\. Lebensjahr (§ 92 (1) Satz 1 Halb satz 3 SGB V, § 16 (1) Satz 2 AM-RL) bei besonde rem Gefährdungspotential. 

**. . .** 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 53 von 74**  
**VOV\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh. Wert Bedeutung Erläuterung**   
16 Anl. III AM-RL: Unwirtschaftl. Verordnung nicht verschreibungs pfl. Arzneimittel (Kdr., Jugendl.) 

17 Anl. III AM-RL: Harn- und Blut zuckerteststreifen 

18 Anl. III AM-RL: Verordnungsein schränkungen nach AM-RL 

(Säfte) 

19 Anl. XII AM-RL (Frühe Nutzen bewertung abgeschlossen) 

20 Zusammenstellung nach § 3 Packungsgrößenverordnung (PackungsV) 

Dieser Typ umfasst Hinweise auf eine unwirtschaft liche Verordnung nicht verschreibungspflichtiger Arzneimittel bei Kindern bis zum vollendeten 12\. Lebensjahr und für Jugendliche mit Entwicklungs störungen bis zum vollendeten 18\. Lebensjahr (§ 92 (1) Satz 1 Halbsatz 3 SGB V, § 16 (1) Satz 2 AM-RL). 

Verordnungseinschränkungen nach § 92 (1) Satz 1 Halbsatz 3 SGB V in Verbindung mit § 16 (1) AM-RL Betroffen sind Saftzubereitungen für Erwachsene; Rechtsgrundlage siehe Typ *13*\! 

Dieser Typ wird Arzneimitteln mit neuen Wirkstoffen zugeordnet, für die der G-BA eine frühe Nutzenbe wertung nach § 35a SGB V abgeschlossen hat. Unter diesem Typ erfolgen Hinweise zur Zusam menstellung von Einzelpackungen zu größeren Einheiten, deren Abgabe im Sinne der PackungsV als Abgabe einer Einzelpackung einer größeren N Packungsgröße gilt. Dies ist bei der Berechnung der vom Patienten zu leistenden Zuzahlung zu berück sichtigen\! 

21 Empfängnisverhütung Nach § 24a (2) SGB V haben Versicherte bis zum vollendeten 22\. Lebensjahr Anspruch auf Versor 

gung mit verschreibungspflichtigen, empfängnis 

verhütenden Mitteln. Entsprechendes gilt für nicht 

verschreibungspflichtige Notfallkontrazeptiva, soweit 

sie ärztlich verordnet werden; in diesem Zusammen 

hang kommt der Hinweis zur fehlenden Erstattungs 

fähigkeit gemäß Anlage I AM-RL (*OTC-Übersicht*; 

PAC\_APO, ID 78) für Personen bis zum vollendeten 

22\. Lebensjahr nicht zum Tragen. 

22 Sonstige Hinweise Mit diesem Typ werden Inhalte abgebildet, wel che nicht den übrigen Typen zugeordnet werden 

können. 

23 Praxisbesonderheit nach § 130b SGB V   
Vereinbarungen nach § 130b (1) SGB V sollen vor sehen, dass Verordnungen des betreffenden Arznei mittels im Rahmen von Wirtschaftlichkeitsprüfungen als Praxisbesonderheit anerkannt werden, wenn der Arzt im Einzelfall die in der Vereinbarung festgeleg ten Anforderungen eingehalten hat. Verordnungs vorgaben dieses Typs referenzieren den Text der Vereinbarung in DOK\_APO. 

Dieser Verordnungstyp hat für Apotheken rein infor mativen Charakter. 

Im Rahmen der individuellen Verordnung ist die Anzeige von Vorgaben der Typen *15*, *16* und *18* nur sinnvoll, wenn der Patient der jeweils angegebenen Altersgruppe angehört. Typ 21 sollte altersunab hängig zur Anzeige kommen. 

Die in Spalte *Erläuterung* angegebenen Rechtsgrundlagen sind für Anzeigezwecke im gleichnamigen Attribut (ID 04) der Datei KVP\_APO abgelegt. 

**Seite 54 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. VPV\_APO** 

**20 Verknüpfung von Artikeln mit Verordnungsvorgaben** 

Dateiname: VPV\_APO 

Datei-Langname: Verknuepfung\_PAC\_VOV 

ABDATA-Dateinummer: 1195 

VPV\_APO verknüpft Artikel ggf. mit mehreren zutreffenden Verordnungsvorgaben aus VOV\_APO. 

Mit Verordnungsvorgaben desRTyps *7* verknüpfte PZN sind in ID 78 der PAC\_APO mit dem Wert *2* belegt. 

**Attributdefinitionen** 

01 **Pharmazentralnummer** \[PZN\] 

02 **Schlüssel der** 

**Verordnungsvorgabe** 

\[Key\_VOV\] 

03 **Befristungsdatum** \[Befristungsdatum\]   
Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: F/8/PZ8 

Primär- undRFremdschlüsselattribut, Pflichtangabe, Format: V/8/NU1 

Optionale Angabe, Format: F/8/DT8 

Das Attribut ist derzeit ausschließlich in Verbindung mit Ver ordnungsvorgabetyp *1* (VOV\_APO) belegt und gibt den Zeit punkt des Ablaufs der Verordnungsfähigkeit des über die PZN (s. o. ID 01\) referenzierten Artikels an. 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 55 von 74**  
**DOK\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**21 Dokumente** 

Dateiname: DOK\_APO 

Dateilangname: Dokumente 

ABDATA-Dateinummer: 1226 

DOK\_APO dient zur vollständigen Weitergabe von Dateien unterschiedlicher Herkunft und Formate. Einträge in DOK\_APO können grundsätzlich von allen Dateien des ABDA-Artikelstamms referenziert werden. Aktuell ist dies nur für VOV\_APO über ID 04 realisiert. 

**Attributdefinitionen** 

01 **Dokumentschlüssel** \[Key\_DOK\] 

02 **Dateiendung** 

\[Dateiendung\] 

03 **Dokument** 

\[Dokument\] 

06 **Dokumenttitel** 

\[Titel\] 

08 **Dokumenttyp** 

\[Typ\] 

04 **Erläuterung zum Dokument** \[Erlaeuterung\] 

05 **Stand des Dokuments** \[Stand\]   
Primärschlüsselattribut, Pflichtangabe, Format: V/10/NU1 

Pflichtangabe, Format: V/10/AN3 

Namenserweiterung der Datei, deren Inhalt in ID 03 abgelegt ist, z. B. *pdf*. 

Pflichtangabe, Format: U/-/B64 

Jeder Attributwert stellt den kompletten Inhalt einer Datei in K2-kompatibler Codierung dar. Zur Ablage in einer separaten Datei nach Decodierung kann aus den zugehörigen Werten der ID 01 und 02 ein eindeutiger Dateiname generiert wer den. 

Optionale Angabe, Format: V/200/AN1 

Das Attribut ist aktuell in allen Datensätzen belegt und er möglicht im Kontext eines konkreten Dokumenttyps (s. u. ID 08\) die direkte Auswahl von Dokumenten in Anwendungs programmen. 

Pflichtangabe, Format: V/3/NU1 

Inhaltliche Klassifizierung des in ID 03 abgelegten Doku ments. 

Wertebereich: 1 *→* Therapiehinweis \[9\] 

5 *→* Beschluss zur Nutzenbewertung 

nach § 35a SGB V \[19\] 

22 *→* Arzneimittel-Richtlinie/AM-RL 

23 *→* Anlage 1 zur Vereinbarung nach 

§ 130b (1) S. 1 SGB V \[23\] 

In eckigen Klammern wird der Typ der Verordnungsvorgabe in VOV\_APO referenziert, mit dem die Dokumente der Typen *1*, *5* und *23* verknüpft sind. 

Unter dem Dokumenttyp *22* sind die Texte der AM-RL und ihrer Anlagen I bis VII abgelegt. 

Optionale Angabe, Format: V/250/AN1 

Das Attribut enthält in Anwendungsprogrammen anzeigbare Informationen zu dem in ID 03 abgelegten Dokument, z. B. Angaben zur Herkunft. 

Optionale Angabe, Format: V/50/AN1 

Im Falle der Therapiehinweise des G-BA bezieht sich die Information auf die Veröffentlichung im Bundesanzeiger („BAnz.“). 

Im Falle der frühen Nutzenbewertung des G-BA bezieht sich die Information auf das Inkrafttreten des Beschlusses. 

**Seite 56 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. KVP\_APO** 

**22 Zusammenfassung von Key-Value-Tabellen** 

Dateiname: KVP\_APO 

Dateilangname: Key\_Value\_Paar 

ABDATA-Dateinummer: 1138 

Für eine Vielzahl von Schlüsselattributen der Dateien des ABDA-Artikelstamms müssten im Sinne ei ner korrekten ER-Modellierung „Parent“-Dateien definiert werden, die jeweils nur zwei Attribute enthiel ten („Key“ und „Value“; z. B.R*Kz. Verpackungsart* in PAC\_APO\!). Gleichzeitig würde sich die Anzahl der Dateien vervielfachen und dadurch das Datenmodell relativ unübersichtlich. Als Kompromiss zwi schen der rein schriftlichen Übermittlung in der Dokumentation und einer korrekten ER-Modellierung wurden die Wertebereiche der betroffenen Schlüsselattribute in KVP\_APO vereinigt. Die darin enthal tenen Referenzen ermöglichen eine maschinelle Verarbeitung. 

**Attributdefinitionen** 

01 **Schlüssel des** 

**Key-Value-Paares** 

\[Key\_KVP\] 

02 **ABDATA-Dateinummer** \[Dateinr\] 

03 **Entschlüsselung** \[Entschluesselung\] 

04 **Erläuterung** 

\[Erlaeuterung\] 

05 **Feldidentifier** 

\[Feldidentifier\] 

06 **Datum der Inaktivierung des Schlüsselwertes** 

\[Inaktivierungsdatum\] 

07 **Interne Notiz** 

\[Interne\_Notiz\]   
Primärschlüsselattribut, Pflichtangabe, Format: V/4/NU1 Das Attribut hat ausschließlich technische Bedeutung. Den alternativen Primärschlüssel bilden die AttributeR*ABDATA Dateinummer*,R*Feldidentifier* und 

R*Schlüsselwert*. 

Pflichtangabe, Format: V/4/NU1 

ABDATA-Dateinummer entspricht dem in ID 07 des K-Satzes der referenzierten Datei abgelegten Wert. 

Pflichtangabe, Format: V/100/AN1 

Die Attributwerte entsprechen jeweils dem „Value“ eines Key-Value-Paares, stellen also die Verbalisierung von R*Schlüsselwert* dar. Bei der Anzeige von Mengenanga ben ist es sinnvoll, bzgl. der Einheit zwischen Singular und Plural zu unterscheiden. In diesen Fällen istR*Pluralform der Entschlüsselung* belegt (auch dann, wenn der Plural sich nicht von der in *Entschlüsselung* angegebenen Singularform unterscheidet). Es gilt die Regel, dass ausschließlich für den Zahlenwert *1* der Singular verwendet wird, in allen anderen Fällen der Plural (z. B. *1 Stunde*, *1,0 Stunden*, *0,5 Stunden*, *3 Stunden*). 

Optionale Angabe, Format: V/500/AN1 

Das Attribut enthält optional eine in Anwendungsprogrammen anzeigbare Information betreffend den inR*Entschlüsselung* abgelegten Wert. 

Pflichtangabe, Format: F/2/AN2 

Feldidentifier referenziert das Feld in der durchR*ABDATA Dateinummer* festgelegten Datei, dessen Wertebereich im AttributR*Schlüsselwert* abgebildet wird. 

Optionale Angabe, Format: F/8/DT8 

Das Attribut wird belegt, wenn der betreffende Wert von ABDATA Pharma-Daten-Service offiziell nicht mehr verwen det wird, er jedoch zur Entschlüsselung archivierter Daten in Anwendungsprogrammen weiterhin zur Verfügung stehen soll. 

Optionale Angabe, Format: V/250/AN1 

Das Attribut enthält im Bedarfsfall Informationen für das Software-Unternehmen; sie sind *nicht* zur Anzeige beim En danwender gedacht. 

**. . .** 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 57 von 74**  
**KVP\_APO Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

08 **Pluralform der Entschlüsselung** 

\[Plural\] 

09 **Schlüsselwert** \[Schluesselwert\]   
Optionale Angabe, Format: V/100/AN1 

SieheR*Entschlüsselung*\! 

Pflichtangabe, Format: V/10/AN1 

Die Attributwerte entsprechen jeweils dem „Key“ eines Key Value-Paares. 

**Seite 58 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. ER-Modell 23 ER-Modell** 

PZN   
**PAC\_APO**   
**ADR\_APO**   
Key\_ADR   
Firmenname   
Firmenname\_2   
Firmenname\_3   
Firmenname\_ABDATA Gh\_Erlaubnis\_52aAMG 

**SER\_APO** Key\_ADR (FK)   
Zaehler   
Adresse   
Adresstyp   
Service   
Zusatzinfo   
Abloesung\_130a\_1\_8 AM\_BfArM\_Kinder AM\_BfArM\_Versorgung AM\_mit\_altersg\_Dafo AM\_mit\_aufg\_Festb AM\_mit\_Erstatt\_130b AMNOG   
AMPreisV\_AMG   
AMPreisV\_SGB   
ApBetrO\_15\_1   
ApBetrO\_15\_2   
Apo\_Ek   
Apo\_Ek\_PpU   
Apo\_Vk   
Apo\_Vk\_PpU   
Apopflicht   
ApU   
ApU\_78\_3a\_1\_AMG Artikelnr   
Artikeltyp   
Arzneimittel   
ATMP   
Ausnahme\_51AMG Ausnahme\_52b\_2\_AMG Ausnahme\_Ersetzung Batt\_Regnr   
Bedingte\_Erstatt\_FAM Bedingte\_Zulassung Bestimmung\_130b\_1c Biosimilargruppe   
Biotech\_AM   
Biotech\_FAM   
Biozid   
BOPSTNr   
Breite   
Bruchgefahr   
BTM   
CMR\_Stoff   
Diaetetikum   
Diff\_PpU\_ApU\_78\_3a\_1 Droge\_Chemikalie   
Eichung   
ElektroStoffV   
EU\_Bio\_Logo   
Explosivgrundstoff 

**VPK\_APO**   
PZN\_Klinikpackung (FK) PZN\_Klinikbaustein (FK)   
Hausnr\_bis   
Hausnr\_bis\_Zusatz Hausnr\_von   
Hausnr\_von\_Zusatz Her\_Erlaubnis\_13AMG Land\_Postfach   
Key\_LAE   
Ort\_Postfach   
Ort\_Postfach\_ABDATA Ort\_Zustellung   
Ort\_Zustell\_ABDATA PLZ\_Grosskunde   
PLZ\_Postfach   
PLZ\_Zustellung   
Postfach   
Regnr\_VerpackG   
Sortiername   
Strasse   
Strasse\_2   
Strasse\_3   
Strasse\_4   
Strasse\_ABDATA 

PZN   
**KLB\_APO** 

Festbetrag   
Festbetragsstufe   
Feuchteempf   
Gdat\_Abloesung\_130a Gdat\_ApU\_78\_3a\_1 Gdat\_ApU78\_3a\_130b1c Gdat\_Preise   
Gdat\_Vertriebsinfo   
Generikum   
Gewicht   
GTIN   
Hm\_zum\_Verbrauch Hochladedatum   
Hoehe   
Import\_Reimport   
Importgruppennr   
IVD\_Klasse   
Key\_ADR\_Anbieter (FK) Key\_ADR\_Hersteller (FK) Key\_ADR\_Vertreter (FK) Key\_ADR\_Zulassung (FK)   
Key\_AUS   
Key\_DAR (FK)   
Key\_FES   
Key\_WAR (FK)   
KHAEP\_PPU   
Kosmetikum\_EG\_VO Krankenhaus\_Ek   
Kuehlkette   
Kurzname   
Laenge   
Lageempf   
Lagertemperatur   
Lagertemperatur\_max Lagertemperatur\_min Langname   
Langname\_ungekuerzt Laufzeit\_Eichung   
Laufzeit\_Verfall   
Lebensmittel   
Lichtempf   
Lifestyle   
MedCanG   
Medizinprodukt   
Medizinprodukt\_AMRL Mindestbestellmenge 

**VPI\_APO**   
PZN (FK)   
Key\_INB (FK) 

**PZG\_APO**   
Key\_GRU (FK) PZN (FK) 

**INB\_APO**   
Key\_INB   
Beschreibung 

**GRU\_APO**   
Key\_GRU   
Mehrkostenverzicht Vorrang\_Aut\_idem Zuzahlungsfaktor 

Einheit   
Key\_DAR (FK)   
Key\_ADR\_Anbieter (FK) Kurzname   
Menge   
Verkehrsstatus   
Vertriebsstatus 

**DAR\_APO**   
Key\_DAR   
Name   
Abkuerzung 

**IZG\_APO**   
Key\_GRU (FK)   
IK (FK) 

**IKZ\_APO** IK   
Name 

**IAE\_APO**   
Mitteilung\_47\_1cAMG MP\_Klasse   
MV\_Gruppe   
MwSt   
Negativliste   
NEM   
Novel\_Food 

**PGR2\_APO** 

**PGR\_APO**   
PZN (FK)   
Packungskomponente 

PZN (FK) IK (FK)   
Regionalkz 

NTIN   
**PAT\_APO** PZN (FK)   
Orphan\_Drug   
PAngV   
Pflanzenschutzmittel PPN   
PpU   
Preisstrukturmodell PZN\_Nachfolger (FK) PZN\_Original (FK) Rab\_Apo   
Rabwert\_130a\_2\_SGB Rabwert\_Anbieter Rabwert\_Generikum Rabwert\_Preismora Ref\_Biosimilar   
Regnr\_stiftung\_ear Rezeptpflicht   
SDB\_erforderlich   
securPharm\_Pilot Sortierbegriff   
Steril   
Stiftung\_Ear   
TFG   
Tierarzneimittel   
T\_Rezept   
UDDI\_MDR   
UNNr 

Zaehler   
Einheit   
Komponentennr   
Typ   
Zahl 

**VPV\_APO**   
PZN (FK)   
Key\_VOV (FK)   
Befristungsdatum 

**WAR\_APO**   
Key\_WAR   
Einstufung 

**VOV\_APO**   
Key\_VOV   
Key\_DOK (FK)   
Text   
Typ 

PZN (FK)   
Textfeld   
Dateiendung   
Dateiname   
Text 

**DOK\_APO**   
Key\_DOK   
Dateiendung   
Dokument   
Dokumenttitel   
Erlaeuterung   
Stand   
Typ   
UVP   
Verbandmittel   
Verfalldatum   
Veribeginn\_Pflicht   
Verkehrsstatus   
Verpackungsart   
Vertriebsstatus   
Vw\_Apo   
Vw\_Grosshandel   
Vw\_Krankenhausapo Vw\_sonstEinzelhandel Wirkstoff\_EG\_RL   
Wundbehand\_31\_1a\_SGB Zulassung\_Ausnahme Zuz\_130b1c\_61\_1   
Zuzfrei\_31SGB\_Feb Zuzfrei\_31SGB\_Tstr 

Key\_WAR\_Verweis (FK) Name 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 59 von 74**  
**ER-Modell Tech. Dok. ABDA-Artikelstamm Apo.-Swh. 23.1 Beschreibung der Dateibeziehungen** 

**Parent-Datei (PD)**   
**Child-Datei (CD)**   
**migrierte(s) PD-Attribut(e) Attributname in CD Beziehung (MC-Notation)** 

ADR\_APO KLB\_APO Key\_ADR Key\_ADR\_Anbieter 1:mc ADR\_APO PAC\_APO Key\_ADR Key\_ADR\_Anbieter 1:mc ADR\_APO PAC\_APO Key\_ADR Key\_ADR\_Hersteller c:mc ADR\_APO PAC\_APO Key\_ADR Key\_ADR\_Vertreter c:mc ADR\_APO PAC\_APO Key\_ADR Key\_ADR\_Zulassung c:mc ADR\_APO SER\_APO Key\_ADR Key\_ADR 1:mc DAR\_APO KLB\_APO Key\_DAR Key\_DAR c:mc DAR\_APO PAC\_APO Key\_DAR Key\_DAR c:mc DOK\_APO VOV\_APO Key\_DOK Key\_DOK c:mc GRU\_APO IZG\_APO Key\_GRU Key\_GRU 1:m 

GRU\_APO PZG\_APO Key\_GRU Key\_GRU 1:m IKZ\_APO IAE\_APO IK IK 1:mc IKZ\_APO IZG\_APO IK IK 1:mc INB\_APO VPI\_APO Key\_INB Key\_INB 1:mc KLB\_APO VPK\_APO PZN PZN\_Klinikbaustein 1:mc PAC\_APO IAE\_APO PZN PZN 1:mc PAC\_APO PAC\_APO PZN PZN\_Nachfolger c:mc PAC\_APO PAC\_APO PZN PZN\_Original c:mc PAC\_APO PAT\_APO PZN PZN 1:mc 

PAC\_APO PGR\_APO PZN PZN 1:m PAC\_APO PGR2\_APO PZN PZN 1:m PAC\_APO PZG\_APO PZN PZN 1:mc PAC\_APO VPI\_APO PZN PZN 1:mc PAC\_APO VPK\_APO PZN PZN\_Klinikpackung 1:mc PAC\_APO VPV\_APO PZN PZN 1:mc VOV\_APO VPV\_APO Key\_VOV Key\_VOV 1:mc WAR\_APO PAC\_APO Key\_WAR Key\_WAR c:mc WAR\_APO WAR\_APO Key\_WAR Key\_WAR\_Verweis c:mc 

**Dateibeziehungen in der modifizierten Chen-Notation (MC-Notation)** 

1:m Jeder Datensatz der Parent-Datei steht mit mindestens einem Datensatz der Child-Datei in Beziehung. Jeder Da tensatz der Child-Datei steht mit genau einem Datensatz der Parent-Datei in Beziehung. 

1:mc Jeder Datensatz der Parent-Datei steht mit keinem, genau einem oder mehreren Datensätzen der Child-Datei in Beziehung. Jeder Datensatz der Child-Datei steht mit genau einem Datensatz der Parent-Datei in Beziehung. c:mc Jeder Datensatz der Parent-Datei steht mit keinem, genau einem oder mehreren Datensätzen der Child-Datei in Beziehung. Jeder Datensatz der Child-Datei steht mit keinem oder genau einem Datensatz der Parent-Datei in Beziehung. 

**Seite 60 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. Arzneimittelabgabepreise gemäß AMPreisV 24 Arzneimittelabgabepreise gemäß AMPreisV** 

Abgabepreise verschreibungspflichtiger Arzneimittel gemäß AMPreisV auf Grund § 78 AMG (jeweils unter Berücksichtigung des 14\. SGB V-Änderungsgesetzes) 

**§ 78 AMG** 

(1) Das Bundesministerium für Wirtschaft und Technologie wird ermächtigt, im Einvernehmen mit dem Bundesministerium und, soweit es sich um Arzneimittel handelt, die zur Anwendung bei Tieren be stimmt sind, im Einvernehmen mit dem Bundesministerium für Ernährung, Landwirtschaft und Verbrau cherschutz durch Rechtsverordnung mit Zustimmung des Bundesrates 

1\. Preisspannen für Arzneimittel, die im Großhandel, in Apotheken oder von Tierärzten im Wieder verkauf abgegeben werden, 

2\. Preise für Arzneimittel, die in Apotheken oder von Tierärzten hergestellt und abgegeben werden, sowie für Abgabegefäße, 

3\. Preise für besondere Leistungen der Apotheken bei der Abgabe von Arzneimitteln 

festzusetzen. Abweichend von Satz 1 wird das Bundesministerium für Wirtschaft und Technologie er mächtigt, im Einvernehmen mit dem Bundesministerium durch Rechtsverordnung, die nicht der Zu stimmung des Bundesrates bedarf, den Anteil des Festzuschlags, der nicht der Förderung der Sicher stellung des Notdienstes dient, entsprechend der Kostenentwicklung der Apotheken bei wirtschaft licher Betriebsführung anzupassen. Die Preisvorschriften für den Großhandel aufgrund von Satz 1 Nummer 1 gelten auch für pharmazeutische Unternehmer oder andere natürliche oder juristische Per sonen, die eine Tätigkeit nach § 4 (22) ausüben bei der Abgabe an Apotheken, die die Arzneimittel zur Abgabe an den Verbraucher beziehen. Die Arzneimittelpreisverordnung, die auf Grund von Satz 1 erlassen worden ist, gilt auch für Arzneimittel, die gemäß § 73 (1) Satz 1 Nummer 1a in den Geltungs bereich dieses Gesetzes verbracht werden. 

(2) Die Preise und Preisspannen müssen den berechtigten Interessen der Arzneimittelverbraucher, der Tierärzte, der Apotheken und des Großhandels Rechnung tragen. Ein einheitlicher Apotheken abgabepreis für Arzneimittel, die vom Verkehr außerhalb der Apotheken ausgeschlossen sind, ist zu gewährleisten. Satz 2 gilt nicht für nicht verschreibungspflichtige Arzneimittel, die nicht zu Lasten der gesetzlichen Krankenversicherung abgegeben werden. 

(3) Für Arzneimittel nach Absatz 2 Satz 2, für die durch die Verordnung nach Absatz 1 Preise und Preisspannen bestimmt sind, haben die pharmazeutischen Unternehmer einen einheitlichen Abga bepreis sicherzustellen; für nicht verschreibungspflichtige Arzneimittel, die zu Lasten der gesetzlichen Krankenversicherung abgegeben werden, haben die pharmazeutischen Unternehmer zum Zwecke der Abrechnung der Apotheken mit den Krankenkassen ihren einheitlichen Abgabepreis anzugeben, von dem bei der Abgabe im Einzelfall abgewichen werden kann. Sozialleistungsträger, private Krankenver sicherungen sowie deren jeweilige Verbände können mit pharmazeutischen Unternehmern für die zu ihren Lasten abgegebenen verschreibungspflichtigen Arzneimittel Preisnachlässe auf den einheitlichen Abgabepreis des pharmazeutischen Unternehmers vereinbaren. 

(3a) Gilt für ein Arzneimittel ein Erstattungsbetrag nach § 130b des Fünften Buches Sozialgesetzbuch, gibt der pharmazeutische Unternehmer das Arzneimittel zum Erstattungsbetrag ab. Abweichend von Satz 1 kann der pharmazeutische Unternehmer das Arzneimittel zu einem Betrag unterhalb des Er stattungsbetrages abgeben; die Verpflichtung in Absatz 3 Satz 1 erster Halbsatz bleibt unberührt. Der Abgabepreis nach Satz 1 oder Satz 2 gilt auch für Personen, die das Arzneimittel nicht als Versicherte einer gesetzlichen Krankenkasse im Wege der Sachleistung erhalten. 

(4) Bei Arzneimitteln, die im Fall einer bedrohlichen übertragbaren Krankheit, deren Ausbreitung eine sofortige und das übliche Maß erheblich überschreitende Bereitstellung von spezifischen Arzneimit teln erforderlich macht, durch Apotheken abgegeben werden und die zu diesem Zweck nach § 47 (1) Satz 1 Nr. 3c bevorratet wurden, gilt als Grundlage für die nach Absatz 2 festzusetzenden Preise und Preisspannen der Länderabgabepreis. Entsprechendes gilt für Arzneimittel, die aus für diesen Zweck entsprechend bevorrateten Wirkstoffen in Apotheken hergestellt und in diesen Fällen abgegeben wer den. In diesen Fällen gilt Absatz 2 Satz 2 auf Länderebene. 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 61 von 74**  
**Arzneimittelabgabepreise gemäß AMPreisV Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**Auszug aus der AMPreisV** 

**§ 2 Großhandelszuschläge für Fertigarzneimittel** 

(1) Bei der Abgabe von Fertigarzneimitteln, die zur Anwendung bei Menschen bestimmt sind, durch den Großhandel an Apotheken oder Tierärzte darf auf den Abgabepreis des pharmazeutischen Unter nehmers ohne die Umsatzsteuer höchstens ein Zuschlag von 3,15 Prozent, höchstens jedoch 37,80 Euro, zuzüglich eines Festzuschlags von 73 Cent sowie die Umsatzsteuer erhoben werden. Bei der Abgabe von Fertigarzneimitteln, die zur Anwendung bei Tieren bestimmt sind, durch den Großhandel an Apotheken oder Tierärzte dürfen auf den Herstellerabgabepreis ohne die Umsatzsteuer höchstens Zuschläge nach Absatz 2 oder 3 sowie die Umsatzsteuer erhoben werden. Der Berechnung der Zu schläge nach Satz 1 ist jeweils der Betrag zugrunde zu legen, zu dem der pharmazeutische Unterneh mer das Arzneimittel nach § 78 Absatz 3 oder Absatz 3a des Arzneimittelgesetzes abgibt. 

(2) Der Höchstzuschlag nach Absatz 1 Satz 2 ist bei einem Herstellerabgabepreis 

bis 0,84 Euro 21,0 Prozent   
(Spanne 17,4 Prozent) 

von 0,89 Euro bis 1,70 Euro 20,0 Prozent   
(Spanne 16,7 Prozent) 

von 1,75 Euro bis 2,56 Euro 19,5 Prozent   
(Spanne 16,3 Prozent) 

von 2,64 Euro bis 3,65 Euro 19,0 Prozent   
(Spanne 16,0 Prozent) 

von 3,76 Euro bis 6,03 Euro 18,5 Prozent   
(Spanne 15,6 Prozent) 

von 6,21 Euro bis 9,10 Euro 18,0 Prozent   
(Spanne 15,3 Prozent) 

von 10,93 Euro bis 44,46 Euro 15,0 Prozent   
(Spanne 13,0 Prozent) 

von 55,59 Euro bis 684,76 Euro 12,0 Prozent   
(Spanne 10,7 Prozent) 

ab 684,77 Euro 3,0 Prozent   
(zuzüglich 61,63 Euro) 

(3) Der Höchstzuschlag ist nach Absatz 1 Satz 2 bei einem Herstellerabgabepreis 

von 0,85 Euro bis 0,88 Euro 0,18 Euro 

von 1,71 Euro bis 1,74 Euro 0,34 Euro 

von 2,57 Euro bis 2,63 Euro 0,50 Euro 

von 3,66 Euro bis 3,75 Euro 0,70 Euro 

von 6,04 Euro bis 6,20 Euro 1,12 Euro 

von 9,11 Euro bis 10,92 Euro 1,64 Euro 

von 44,47 Euro bis 55,58 Euro 6,67 Euro 

(Fortsetzung auf der nächsten Seite) 

**Seite 62 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. Arzneimittelabgabepreise gemäß AMPreisV** 

**§ 3 Apothekenzuschläge für Fertigarzneimittel** 

(1) Bei der Abgabe von Fertigarzneimitteln, die zur Anwendung bei Menschen bestimmt sind, durch die Apotheken sind zur Berechnung des Apothekenabgabepreises ein Festzuschlag von 3 Prozent zu züglich 8,35 Euro zuzüglich 21 Cent zur Förderung der Sicherstellung des Notdienstes zuzüglich 20 Cent zur Finanzierung zusätzlicher pharmazeutischer Dienstleistungen nach §129 Absatz 5e des Fünf ten Buches Sozialgesetzbuch sowie die Umsatzsteuer zu erheben. Soweit Fertigarzneimittel, die zur Anwendung bei Menschen bestimmt sind, durch die Apotheken zur Anwendung bei Tieren abgegeben werden, dürfen zur Berechnung des Apothekenabgabepreises abweichend von Satz 1 höchstens ein Zuschlag von 3 Prozent zuzüglich 8,10 Euro sowie die Umsatzsteuer erhoben werden. Bei der Abgabe von Fertigarzneimitteln, die zur Anwendung bei Tieren bestimmt sind, durch die Apotheken dürfen zur Berechnung des Apothekenabgabepreises höchstens die Zuschläge nach Absatz 3 oder 4 sowie die Umsatzsteuer erhoben werden. 

(2) Der Festzuschlag ist zu erheben 

1\. auf den Betrag, der sich aus der Zusammenrechnung des bei Belieferung des Großhandels gelten den Abgabepreises des pharmazeutischen Unternehmers ohne die Umsatzsteuer und des darauf ent fallenden Großhandelshöchstzuschlags nach § 2 ergibt, 

2\. bei Fertigarzneimitteln, die nach § 52b (2) Satz 3 des Arzneimittelgesetzes nur vom pharmazeuti schen Unternehmer direkt zu beziehen sind, auf den bei Belieferung der Apotheke geltenden Abgabe preis des pharmazeutischen Unternehmers ohne die Umsatzsteuer; § 2 Absatz 1 Satz 3 gilt entspre chend. 

(3) Der Höchstzuschlag ist nach Absatz 1 Satz 3 bei einem Betrag 

bis 1,22 Euro 68 Prozent   
(Spanne 40,5 Prozent) 

von 1,35 Euro bis 3,88 Euro 62 Prozent (Spanne 38,3 Prozent) 

von 4,23 Euro bis 7,30 Euro 57 Prozent (Spanne 36,3 Prozent) 

von 8,68 Euro bis 12,14 Euro 48 Prozent (Spanne 32,4 Prozent) 

von 13,56 Euro bis 19,42 Euro 43 Prozent (Spanne 30,1 Prozent) 

von 22,58 Euro bis 29,14 Euro 37 Prozent (Spanne 27,0 Prozent) 

von 35,95 Euro bis 543,91 Euro 30 Prozent (Spanne 23,1 Prozent) 

ab 543,92 Euro 8,263 Prozent (zuzüglich 118,24 Euro) 

(4) Der Höchstzuschlag ist nach Absatz 1 Satz 3 bei einem Betrag 

von 1,23 Euro bis 1,34 Euro 0,83 Euro 

von 3,89 Euro bis 4,22 Euro 2,41 Euro 

von 7,31 Euro bis 8,67 Euro 4,16 Euro 

von 12,15 Euro bis 13,55 Euro 5,83 Euro 

von 19,43 Euro bis 22,57 Euro 8,35 Euro 

von 29,15 Euro bis 35,94 Euro 10,78 Euro 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 63 von 74**  
**Arzneimittelabgabepreise gemäß § 129 (5a) SGB V Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**25 Arzneimittelabgabepreise gemäß § 129 (5a) SGB V** 

Abgabepreise nicht verschreibungspflichtiger Arzneimittel gemäß § 129 (5a) SGB V zu Lasten der GKV 

**Auszug aus § 129 SGB V** 

(5a) Bei Abgabe eines nicht verschreibungspflichtigen Arzneimittels gilt bei Abrechnung nach § 300 ein für die Versicherten maßgeblicher Arzneimittelabgabepreis in Höhe des Abgabepreises des pharma zeutischen Unternehmens zuzüglich der Zuschläge nach den §§ 2 und 3 der AMPreisV in der am 31\. Dezember 2003 gültigen Fassung. 

**Auszug aus der AMPreisV in der am 31.12.2003 gültigen Fassung** 

**§ 2 Großhandelszuschläge für Fertigarzneimittel** 

(2) Der Höchstzuschlag ist bei einem Herstellerabgabepreis 

bis 0,84 Euro 21,0 vom Hundert   
(Spanne 17,4 vom Hundert) 

von 0,89 Euro bis 1,70 Euro 20,0 vom Hundert (Spanne 16,7 vom Hundert) 

von 1,75 Euro bis 2,56 Euro 19,5 vom Hundert (Spanne 16,3 vom Hundert) 

von 2,64 Euro bis 3,65 Euro 19,0 vom Hundert (Spanne 16,0 vom Hundert) 

von 3,76 Euro bis 6,03 Euro 18,5 vom Hundert (Spanne 15,6 vom Hundert) 

von 6,21 Euro bis 9,10 Euro 18,0 vom Hundert (Spanne 15,3 vom Hundert) 

von 10,93 Euro bis 44,46 Euro 15,0 vom Hundert (Spanne 13,0 vom Hundert) 

von 55,59 Euro bis 684,76 Euro 12,0 vom Hundert (Spanne 10,7 vom Hundert) 

ab 684,77 Euro 3,0 vom Hundert   
(zuzüglich 61,63 Euro) 

(3) Der Höchstzuschlag ist bei einem Herstellerabgabepreis 

von 0,85 Euro bis 0,88 Euro 0,18 Euro 

von 1,71 Euro bis 1,74 Euro 0,34 Euro 

von 2,57 Euro bis 2,63 Euro 0,50 Euro 

von 3,66 Euro bis 3,75 Euro 0,70 Euro 

von 6,04 Euro bis 6,20 Euro 1,12 Euro 

von 9,11 Euro bis 10,92 Euro 1,64 Euro 

von 44,47 Euro bis 55,58 Euro 6,67 Euro 

(Fortsetzung auf der nächsten Seite) 

**Seite 64 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. Arzneimittelabgabepreise gemäß § 129 (5a) SGB V** 

**§ 3 Apothekenzuschläge für Fertigarzneimittel** 

(3) Der Festzuschlag ist bei einem Betrag 

bis 1,22 Euro 68 vom Hundert   
(Spanne 40,5 vom Hundert) 

von 1,35 Euro bis 3,88 Euro 62 vom Hundert (Spanne 38,3 vom Hundert) 

von 4,23 Euro bis 7,30 Euro 57 vom Hundert (Spanne 36,3 vom Hundert) 

von 8,68 Euro bis 12,14 Euro 48 vom Hundert (Spanne 32,4 vom Hundert) 

von 13,56 Euro bis 19,42 Euro 43 vom Hundert (Spanne 30,1 vom Hundert) 

von 22,58 Euro bis 29,14 Euro 37 vom Hundert (Spanne 27,0 vom Hundert) 

von 35,95 Euro bis 543,91 Euro 30 vom Hundert (Spanne 23,1 vom Hundert) 

ab 543,92 Euro 8,263 vom Hundert (zuzüglich 118,24 Euro) 

(4) Der Festzuschlag ist bei einem Betrag 

von 1,23 Euro bis 1,34 Euro 0,83 Euro 

von 3,89 Euro bis 4,22 Euro 2,41 Euro 

von 7,31 Euro bis 8,67 Euro 4,16 Euro 

von 12,15 Euro bis 13,55 Euro 5,83 Euro 

von 19,43 Euro bis 22,57 Euro 8,35 Euro 

von 29,15 Euro bis 35,94 Euro 10,78 Euro 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 65 von 74**  
**Berechnung der Abgabepreise für Fertigarzneimittel Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**26 Berechnung der Abgabepreise für Fertigarzneimittel** 

Stand: 01.01.2012 

| Zu berücksichtigendeKennzeichen aus  PAC\_APO | Kombinationen von Kennzeichenwerten |  |  |  |  |  |
| :---- | :---: | :---: | :---: | :---: | :---: | :---: |
| Arzneimittel  | ja  | ja  | ja  | ja  | ja  | ja |
| Apothekenpflicht  | ja  | ja  | ja  | ja  | ja  | ja |
| Verschreibungspflicht  | ja  | ja  | ja  | nein  | nein  | nein |
| Tierarzneimittel  | nein  | nein  | ja  | nein  | nein  | ja |
|  | *↓*  | *↓*  | *↓*  | *↓*  | *↓*  | *↓* |
| Berechnungsvorschrift  | AMPreisV  |  |  | SGB V  | keine  | keine |
| zur Anwendung am  | Menschen  | Tier  | Tier  | Menschen  | Tier  | Tier |
|  | *↓*  | *↓*  | *↓*  | *↓*  | *↓*  | *↓* |
| Großhandelszuschlag  | § 2 (1) Satz 1  | § 2 (1) Satz 1  | § 2 (2) oder (3)  | § 129 (5a)  | —  |  |
| Apothekenzuschlag  | § 3 (1) Satz 1  | § 3 (1) Satz 2  | § 3 (3) oder (4) |  |  |  |

**Seite 66 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. Dokumenthistorie** 

**27 Dokumenthistorie** 

• **Änderungen zum Bearbeitungsstand 15.05.2008, gültig ab 01.10.2008** 

**–** PAC\_APO 

\* Wegfall des Attributs *Kz. CE-Kennzeichnung* (ID 10\)   
\* Neues Attribut *Kz. Diätetikum* (ID 89)   
\* Neues Attribut *Kz. Lebensmittel* (ID 90)   
\* Neues Attribut *Kz. Nahrungsergänzungsmittel* (ID 91)   
\* Wertebereich des Attributs *Einheit der Packungsgröße* (ID 14\) um den Wert *Sprühstöße* (*Sp*) erweitert 

• **Änderungen zum Bearbeitungsstand 08.07.2008, gültig ab 01.10.2008** 

**–** Neue Dateien ab 01.10.2008: OAP\_APO und VPO\_APO 

• **Änderungen zum Bearbeitungsstand 31.10.2008, gültig ab 01.01.2009** 

**–** VOV\_APO und VPV\_APO eingefügt (Erstausgabe zum 01.01.2009) 

**–** PAC\_APO 

\* Erläuterung zum Wertebereich des Attributs *Kz. Zuzahlungsbefreiung für preisgünstige Festbetragsartikel* (ID 88) präzisiert 

• **Änderungen zum Bearbeitungsstand 26.01.2009, gültig ab 15.03.2009** 

**–** VOV\_APO 

\* Wertebereich des Attributs *Typ der Verordnungsvorgabe* (ID 03) um die Werte *3*, *4* und *5* erweitert 

**–** VPV\_APO 

\* Neues Attribut *Befristungsdatum* (ID 03) 

• **Änderungen zum Bearbeitungsstand 07.05.2009, gültig ab 15.05.2009** 

**–** VOV\_APO 

\* Entschlüsselung der Werte *1* und *2* des Attributs *Typ der Verordnungsvorgabe* (ID 03) an die geänderte AM-RL angepasst 

• **Änderungen zum Bearbeitungsstand 19.08.2009, gültig ab 01.10.2009** 

**–** VOV\_APO 

\* Wertebereich des Attributs *Typ der Verordnungsvorgabe* (ID 03) um den   
Wert *6* erweitert 

• **Änderungen zum Bearbeitungsstand 11.03.2010, gültig ab 01.05.2010** 

**–** PAC\_APO 

\* Attribut *Artikeltyp* (ID 64): Neuer Wert *5* 

• **Änderungen zum Bearbeitungsstand 11.03.2010, gültig ab 01.09.2010** 

**–** Wegfall von Dateien: 

\*FES\_APO und STO\_APO; die Attribute *Festbetrag* und *Festbetragsstufe* aus FES\_APO werden in PAC\_APO integriert, s. u.\! 

\* OAP\_APO und VPO\_APO; ihre Inhalte werden in die Dateien VOV\_APO (siehe auch unten) und VPV\_APO integriert. Die Attribute *Bezeichnung der Ausnahmeposition* und *Definition der Ausnahmeposition* der OAP\_APO werden im Attribut *Text der Verord nungsvorgabe* (ID 02) der VOV\_APO zusammengefasst. 

**–** Neue Datei PGR2\_APO; Auswirkung auf PAC\_APO s. u.\! 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 67 von 74**  
**Dokumenthistorie Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

**–** ADR\_APO 

\* Neue Attribute:   
· *Firmenname 2* (ID 22) 

· *Firmenname 3* (ID 23) 

· *Firmenname (ABDATA)* (ID 24) 

· *Ort Postfach (ABDATA)* (ID 25) 

· *Ort Zustellung (ABDATA)* (ID 26) 

· *Straße (ABDATA)* (ID 27) 

\* Attribut *Firmenname* (ID 02): Änderung der Feldlänge von 40 auf 80 Zeichen 

**–** PAC\_APO 

\* Wegfall von Attributen; diese werden in PGR2\_APO integriert (Hinweis: Die Packungs größe von Klinikbausteinen bleibt Bestandteil der KLB\_APO): · *Einheit der ABDATA-Packungsgröße* (ID 13\) 

· *Einheit der Packungsgröße* (ID 14\) 

· *Menge der ABDATA-Packungsgröße* (ID 38\) 

· *Menge der Packungsgröße* (ID 39\) 

\* Neue Attribute:   
· *Kz. Bezugnehmende Zulassung als Generikum* (ID 92) 

· *Kz. Biotechnisch hergestelltes Fertigarzneimittel* (ID 93) 

· *Kz. Mitteilungspflicht gemäß § 47 (1c) AMG* (ID 94) 

· *Kz. Zugelassenes Biozid* (ID 95) 

· *Kz. Zugelassenes Pflanzenschutzmittel* (ID 96) 

\* Aus der entfallenen FES\_APO übernommene Attribute:   
· *Festbetrag* (ID 97) 

· *Festbetragsstufe* (ID 98) 

**–** VOV\_APO 

\* Attribut *Text der Verordnungsvorgabe* (ID 02): Feldlängentyp von *V* in *U* geändert \* Attribut *Typ der Verordnungsvorgabe* (ID 03): 

· Entschlüsselung und Erläuterung zum Wert *6* korrigiert (*\-ausschlüsse* bzw. *oder ausgeschlossen* gestrichen) 

· Neuer Wert *7* (aufgrund der Integration der OAP\_APO) 

**–** WAR\_APO 

\* REinleitung: Definition des A-Sortiments angepasst (*im Wesentlichen* eingefügt) • **Änderungen zum Bearbeitungsstand 19.04.2011, gültig ab 01.07.2011** 

**–** PAC\_APO 

\* Neue Attribute:   
· *Kz. Großhandelsabschlag* (ID A1) 

· *Rabattwert nach § 130a (2) SGB V* (ID A0) 

· *Rabattwert nach § 130b SGB V* (ID 99\) 

\* Änderungen:   
· *Europäische Artikelnummerierung/Universal Product Code* (EAN\_UPC, ID 15) um benannt in *Global Trade Item Number* (GTIN) und Datentyp von *NU1* in *NU3* geändert. · *Großhandelseinkaufspreis* (Grosshandel\_Ek, ID 18) umbenannt in *Abgabepreis des pharmazeutischen Unternehmers* (ApU). 

· *Kz. Negativliste* (ID 40): Entschlüsselung der Werte *2* und *3* geändert. 

**–** VOV\_APO 

\* Attribut *Typ der Verordnungsvorgabe* (ID 03): Wert *2* entfallen (§ 73d SGB V wurde zum 01.01.2011 aufgehoben), Wert *8* eingefügt, Erläuterung zu Wert 6 ergänzt (*und Medi zinprodukte* eingefügt) 

**Seite 68 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. Dokumenthistorie** 

• **Änderungen zum Bearbeitungsstand 01.12.2011, gültig ab 01.01.2012** 

**–** Abschnitt *Hinweis zur Umstellung auf 8-stellige Pharmazentralnummern* eingefügt **–** Abschnitte 24 und 26 an die ab 01.01.2012 gültige Fassung der AMPreisV angepasst. **–** Abschnitte 24, 25 und 26 präzisiert. 

• **Änderungen zum Bearbeitungsstand 07.02.2012, gültig ab 01.04.2012** 

**–** Neue Datei DOK\_APO 

**–** PAC\_APO: Neues Attribut *Kz. T-Rezept* (ID A2) 

**–** VOV\_APO 

\* Neues Attribut *Dokumentschlüssel* (ID 04)   
\* Pflichtbelegung des Attributs *Text der Verordnungsvorgabe* (ID 02) aufgehoben \* Wertebereich des Attributs *Typ der Verordnungsvorgabe* (ID 03) um den Wert *9* erweitert 

• **Änderungen zum Bearbeitungsstand 25.05.2012, gültig ab 01.07.2012** 

**–** Neue Datei ISA\_APO 

**–** PAC\_APO 

\* Neue Attribute:   
· *Kz. Ausnahmeregelung gemäß § 52b (2) Satz 3 AMG* (ID A3) 

· *Kz. Verifikationspflicht* (ID A4) 

· *Kz. Artikel gemäß § 15 (1) Satz 2 ApBetrO* (ID A5) 

· *Kz. Artikel gemäß § 15 (2) ApBetrO* (ID A6) 

\* Wertebereich des Attributs *Kz. Vertriebsstatus* (ID 55) um den Wert *3* erweitert **–** VOV\_APO 

\* Attribut *Schlüssel der Verordnungsvorgabe* (ID 01): Änderung der Feldlänge von 4 auf 8 Zeichen 

\* Attribut *Typ der Verordnungsvorgabe* (ID 03): Wertebereich um den Wert *10* erweitert sowie Entschlüsselung von Wert *6* präzisiert 

**–** DOK\_APO: Neues Attribut *Dokumenttitel* (ID 06) 

**–** Beziehung zwischenRIKZ\_APO und IZG\_APO von *1:m* in *1:mc* geändert • **Änderungen zum Bearbeitungsstand 31.05.2012** (zum 01.07.2012 relevante Korrekturen) 

**–** ISA\_APO: Korrektur des Feldlängentyps der ID 03 von *F* in *V* 

**–** VPV\_APO: Korrektur der Feldlänge der ID 02 von *4* in *8* Zeichen 

• **Änderungen zum Bearbeitungsstand 06.07.2012, gültig ab 15.09.2012** 

**–** VOV\_APO: Im Wertebereich des Attributs *Typ der Verordnungsvorgabe* (ID 03) wird der Wert *6* durch die Werte *11* bis *18* ersetzt 

**–** KVP\_APO: Erhöhung der Feldlänge der ID 04 von *250* auf *500* Zeichen 

• **Änderungen zum Bearbeitungsstand 30.11.2012, gültig ab 01.01.2013** 

**–** Generelle Ersetzung des Feldformats *F/7/PZN* durch *F/8/PZ8* wegen der Umstellung auf 8-stellige Pharmazentralnummern. In diesem Zusammenhang entfällt der am 01.12.2011 eingefügte Hinweis. 

**–** Zusätzlicher Pharmazentralnummernkreis zur internen Nutzung (PAC\_APO.PZN) **–** AMPreisV, § 3 (1): Änderung des Festzuschlags von 8,10 in 8,35 Euro 

**–** PAC\_APO, ID 99: In der Feldbeschreibung wurde *Rabattwert* in *Erstattungsbetrag* korrigiert; die Angaben im F-Satz sind unverändert. 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 69 von 74**  
**Dokumenthistorie Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

• **Änderungen zum Bearbeitungsstand 20.03.2013, gültig ab 01.07.2013** 

**–** PAC\_APO 

\* Wegfall von Attributen:   
· *Apothekenverkaufspreis, empfohlener* (ID 79\) 

· *Kz. Notfalldepot der Apotheke* (ID 41\) 

· *Kz. Notfalldepot der kurzfristig beschaffbaren Arzneimittel* (ID 42\) 

· *Kz. Großhandelsabschlag* (ID A1) 

\* Neue Attribute: ID A8, A9, B0, B1, B3   
\*ID 31: Umlaute und Eszett werden korrekt durch Protypen repräsentiert (bisherige Dar stellung: *ae*, *oe*, . . . bzw. *ss*) \*ID 48: Verkürzung der Feldlänge von 120 auf 30 Zeichen (die Ausgabe erfolgt bereits seit 01.01.2012 mit dieser Feldlänge) 

\*ID 99: Hinweis eingefügt, dass in diesem Attribut Nettobeträge ausgegeben werden **–** VOV\_APO: Wertebereich der ID 03 um den Wert *19* erweitert 

• **Änderungen zum Bearbeitungsstand 30.04.2013, gültig ab 01.07.2013** 

**–** PAC\_APO 

\* Neues Attribut: ID B6 

• **Änderungen zum Bearbeitungsstand 27.05.2013, gültig ab 01.08.2013** 

**–** Neue Datei GWVO\_APO 

**–** PAC\_APO 

\* Neues Attribut: ID B2 

**–** AUS\_APO, Einleitung: Präzisierung hinsichtlich Packungsgrößengleichheit eingefügt **–** DOK\_APO, ID 05: Bedeutung des Datums im Falle der frühen Nutzenbewertung korrigiert **–** VOV\_APO, ID 02: Ausnahmeliste um den Typ *19* erweitert 

• **Änderungen zum Bearbeitungsstand 26.06.2013, gültig ab 01.08.2013** 

**–** Erläuterung zur Datei GWVO\_APO korrigiert: 

\* Differenzierung von Fall 2 bzgl. Packungsgrößenkennzeichen   
\* Beim Vergleich der Packungsgrößen in Unterfall 2.1 muss Packungsgrößentyp *2* aus PGR2\_APO herangezogen werden 

**–** AMPreisV, § 3 (1): 

\* Änderung von Satz 1 durch das ANSG   
\* Korrektur des Festzuschlags in Satz 2 von 8,35 auf 8,10 Euro; Berichtigung wird mit Veröffentlichung dieser Dokumentation gültig. 

• **Änderungen zum Bearbeitungsstand 28.01.2014, gültig ab 01.07.2014** 

**–** PAC\_APO 

\* Neue Attribute: B7, B8   
\*ID 15: Erhöhung der Feldlänge von 13 auf 14 Zeichen   
\*ID B3: Der Wert *1* verbietet jetzt eine Substitution (nach der früheren Formulierung be stand keine Verpflichtung zur Substitution); diese Änderung gilt bereits ab 01.04.2014. **–** SER\_APO 

\*ID 05: Neue Werte *10*, *11*, *12*   
\*ID 03 und 06: Jeweils Erhöhung der Feldlänge von 80 auf 250 Zeichen 

**–** GWVO\_APO: Erweiterung der Verordnungszeile um die Anzahl der verordneten Packungen **–** VOV\_APO, ID 03: Wertebereich um den Wert 20 erweitert 

**Seite 70 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. Dokumenthistorie** 

• **Änderungen zum Bearbeitungsstand 03.03.2014, gültig ab 01.04.2014/01.07.2014** 

**–** Ab 01.04.2014 gültige Änderungen 

\* GWVO\_APO   
· ID 01: Offizielle Bezeichnung *WG14-Nummer* eingefügt 

· Erläuterung, Unterfälle 2.1 und 2.2: Berücksichtigung von Betäubungsmitteln einge fügt 

\* PAC\_APO   
· Neue Attribute: C0, C1, C2, C3, C4, C5; in diesem Zusammenhang wurde die Er läuterung zu den ID 02, 04 und 18 aktualisiert 

· Inaktivierung von Attributen: ID 99 und B6 

\* AMPreisV, Anpassung infolge des 14\. SGB V-Änderungsgesetzes: § 2 (1) (Anfügen ei nes dritten Satzes) und § 3 (2) Nr. 2 (Verweis auf § 2 (1) Satz 3 angefügt), ferner § 78 AMG eingefügt. 

**–** Ab 01.07.2014 gültige Änderungen (zusätzlich zu den im Bearbeitungsstand 28.01.2014 aufgeführten Änderungen) 

\* PAC\_APO   
· Neues Attribut: B9 

• **Änderungen zum Bearbeitungsstand 21.04.2015, gültig ab 01.05.2015/01.07.2015** 

**–** Ab 01.05.2014 gültige Änderungen 

\* PAC\_APO: ID 03, 08, 54: Jeweils geänderte Interpretation des Wertes *0*, die am 01.10.2015 wieder außer Kraft gesetzt wird. 

**–** Ab 01.07.2015 gültige Änderung 

\* VOV\_APO, ID 03: Neuer Wert 21; in diesem Zusammenhang wurde die Erläuterung zum Wert *1* in ID 78 der PAC\_APO angepasst. 

• **Änderungen zum Bearbeitungsstand 21.05.2015, gültig ab 01.10.2015** 

**–** PAC\_APO 

\* Neues Attribut: C6   
\*ID 03: Neuer Wert *3*   
\*ID 08: Neuer Wert *3*   
\*ID 54: Neuer Wert *4* 

Zu ID 03, 08, 54: Die seit 01.05.2015 geänderte Interpretation des Wertes *0* wird jeweils rückgängig gemacht. 

**–** VOV\_APO, ID 03: Neuer Wert 22 

• **Änderungen zum Bearbeitungsstand 23.02.2016, gültig ab 01.07.2016** 

**–** PAC\_APO 

\* Neue Attribute: C7 und C8 

• **Änderungen zum Bearbeitungsstand 07.04.2017, gültig ab 01.10.2017** 

**–** PAC\_APO 

\* Neue Attribute: C9, D1, D2, D3, D4, D5, D6. Zu C9, D1 und D2: Neuer Datentyp, siehe auch K2FORMAT.pdf\! 

\*ID C7, Wert *4*: Entschlüsselung präzisiert 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 71 von 74**  
**Dokumenthistorie Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

• **Änderungen zum Bearbeitungsstand 20.09.2017, gültig ab 01.04.2018** 

**–** Wegfall der Datei LAE\_APO zum 01.04.2018. In diesem Zusammenhang wird der Wertebe reich von ID 07 der ADR\_APO in KVP\_APO integriert. 

**–** PAC\_APO 

\* Neue Attribute: D8, D9, E1, E2   
\*ID 46: Änderung der Feldlänge von 5 auf 6 Zeichen   
\*ID 64: Neuer Wert *6* 

• **Änderungen zum Bearbeitungsstand 26.01.2018, gültig ab 01.07.2018** 

**–** DAR\_APO, ID 02: Änderung der Feldlänge von 60 auf 110 Zeichen 

**–** PAC\_APO 

\* Neue Attribute: E4, E5 

• **Änderungen zum Bearbeitungsstand 30.08.2018, gültig ab 01.02.2019** 

**–** PAC\_APO 

\* Wegfall von Attributen:   
· *Kz. Verifikationspflicht* (ID A4) 

· *Verfalldatum der Charge, ab der im Testbetrieb verifiziert wird* (ID C9) 

· *Verfalldatum der Charge, ab der im Realbetrieb verifiziert wird* (ID D1) 

ID C9 und D1 sind bereits seit 01.02.2018 nicht mehr belegt. 

\* Neue Attribute: E6, E7, E8, E9, F0   
\*ID 53: Wertebereichserweiterung (23 bis 28, 30\) 

• **Änderungen zum Bearbeitungsstand 24.01.2019, gültig ab 01.07.2019** 

**–** Ersetzung der Datei ISA\_APO durch neue Datei IAE\_APO. 

**–** PAC\_APO, ID F0: Belegung entfällt; Ersetzung durch ID 28 in ADR\_APO. 

**–** Neue Datei: PAT\_APO 

**–** VOV\_APO, ID 03: Neuer Wert 23 

**–** DOK\_APO: Neues Attribut 08; in diesem Zusammenhang Aufnahme der Anlage 1 zu Ver einbarungen nach § 130b SGB V sowie der Arzneimittelrichtlinie und ihrer Anlagen I bis VII. 

• **Änderungen zum Bearbeitungsstand 17.12.2019, gültig ab 01.03.2020** 

**–** Anpassung der AMPreisV § 3 (1) Satz 1 Auszug aus der AMPreisV 

**–** Hinzufügen von Erläuterungen zur ID 03 in PGR\_APO und ID B0 in PAC\_APO 

**–** Korrektur der Paragraphen des Rahmenvertrages in PAC\_APO ID 08, B3, B9 und GWVO\_APO. 

**–** PAC\_APO: Neues Attribut ID F1 

**–** Wegfall der Datei ISA\_APO; Seit 01.07.2019 Ersatz durch Datei IAE\_APO. • **Änderungen zum Bearbeitungsstand 06.07.2020, gültig ab 01.12.2020** 

**–** PAC\_APO: 

\* Wegfall von Attributen:   
· *Kz. Abgabepreis des pharmazeutischen Unternehmers ist um den Erstattungsbe trag nach § 130b SGB V gemindert* (ID B6) 

· *Erstattungsbetrag nach § 130b SGB V* (ID 99\) 

· *Registrierungsnummer gemäß Verpackungsgesetz – VerpackG* (ID F0) 

\* Neue Attribute F2, F3, F4, F5, F6, F8, F9, G0, G1, G2 und G3   
\*ID B3: Veränderung des Datentypes von FL auf NU1   
\*ID B3: Neuer Wert 2 

**Seite 72 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**  
**Tech. Dok. ABDA-Artikelstamm Apo.-Swh. Dokumenthistorie** 

\*ID 80: Ergänzung der Feldbeschreibung 

**–** ADR\_APO: 

\* Neue Attribute 29, 30, 31, 32   
\*ID 27: Änderung der Feldlänge von 50 auf 200 Zeichen 

**–** SER\_APO: Anpassung der Wertebezeichnung für ID 05 

• **Änderungen zum Bearbeitungsstand 15.12.2021, gültig ab 01.12.2020** 

**–** PAC\_APO, ID 21: Erläuterung präzisiert. 

**–** Abschnitt 24: § 3 (Apothekenzuschläge für Fertigarzneimittel) präzisiert. 

**–** VOV\_APO, Typ 21 der Verordnungsvorgabe: Erläuterungstext wurde in Bezug auf das Le bensalter angepasst. 

• **Änderungen zum Bearbeitungsstand 22.06.2022, gültig ab 01.09.2022** 

**–** PAC\_APO: 

\* Neue Attribute ID G6, ID G7   
\*ID F4: Erläuterung präzisiert.   
\*ID 72: Datentyp geändert. 

• **Änderungen zum Bearbeitungsstand 02.08.2022, gültig ab 01.12.2022** 

**–** PAC\_APO: 

\*ID 07: Wert 3 entfällt   
\*ID 36: Präzisierung der Erläuterung.   
\*ID 54: Präzisierung der Erläuterung zum Wert 4\.   
\*ID 64: Neuer Wert 7   
\*ID 76: Wert 3 entfällt   
\*ID C1: Anpassung des Wertebereichs und Änderung des Datentyps von FLA auf NU1. \*ID D6: Verweis auf ID H0 eingefügt. 

\* Wegfall der Attribute: ID D3, D4 und D5   
\* Neue Attribute ID G9, ID H0, ID H1 

**–** VOV\_APO, Typ 20 der Verordnungsvorgabe: Im Erläuterungstext wurde der Hinweis ent fernt, dass dieser Typ bis auf Weiteres in VPV\_APO nicht verknüpft wird. 

• **Änderungen zum Bearbeitungsstand 10.10.2022, gültig ab 01.12.2022** 

**–** PAC\_APO: 

\*ID G9: Änderung des Wertebereichs. 

• **Änderungen zum Bearbeitungsstand 25.10.2023, gültig ab 01.02.2024** 

**–** PAC\_APO: 

\* Neues Attribut ID E3   
\* Neues Attribut ID H4   
\* Neues Attribut ID H5   
\* Neues Attribut ID H6   
\* Neues Attribut ID H7   
\* Neues Attribut ID H8   
\* Neues Attribut ID I0   
\* Neues Attribut ID I1   
\*ID C1 Erweiterung des Wertebereichs: neuer Wert 3   
\*ID 49 Beschreibung des Attributs geändert; Erweiterung des Wertebereichs: neuer Wert 3 

**Stand: 28.10.2024 ABDATA Pharma-Daten-Service Seite 73 von 74**  
**Dokumenthistorie Tech. Dok. ABDA-Artikelstamm Apo.-Swh.** 

\*ID 94 Beschreibung des Attributs geändert; Beschreibung des Werts 2 geändert; Wer tebereich um die Werte 3 und 4 erweitert \* Wegfall der Attribute ID 19, 20, 43, 61, 35   
**–** AUS\_APO: 

\* Datei entfällt. Erläuterungen zu AUT IDEM am Ende von PAC\_APO.   
Verweise in VOV\_APO angepasst 

**–** DOK\_APO: 

\* Dokumenttitel (ID 06) auf 200 Zeichen erweitert 

**–** Abschnitt 25: Anpassung des Großhandelszuschlags nach der Änderung § 2 AMPreisV vom 27.07.2023 

• **Änderungen zum Bearbeitungsstand 30.07.2024, gültig ab 01.11.2024** 

**–** PAC\_APO: 

\* Beschreibung von ID 02 (*Apothekeneinkaufspreis*) angepasst   
\* Wegfall des Attributs *Schlüssel der Artikelgruppe für die Wirkstoffverordnung nach dem ABDA/KBV-Konzept* (ID B2) 

\* Neues Attribut ID I2 *Kz. MedCanG*   
\* Neues Attribut ID I3 *Kz. Lagertemperatur beachten* 

**–** Wegfall der Datei *Gruppen betr. die Wirkstoffverordnung nach dem ABDA/KBV-Konzept* (GWVO\_APO) 

• **Änderungen zum Bearbeitungsstand 28.10.2024, gültig ab 01.01.2025** 

**–** PAC\_APO: 

\* Neues Attribut ID I4 *Kz. Bestimmung nach § 130b Abs. 1c SGB V* 

\* Neues Attribut ID I5 *Datum, ab dem der Abgabepreis des pharmazeutischen Unterneh mers nach § 78 Abs. 3a Satz 1 mit Bestimmung nach § 130b Abs. 1c SGB V gilt* 

\* Neues Attribut ID I6 *Zuzahlung für AM entsprechend § 130b Abs. 1c, nach § 61 Satz 1 SGB V (in Cent)* 

\* Beschreibung von ID 74 *Rabattwert zu Lasten des Anbieters* angepasst 

**Seite 74 von 74 ABDATA Pharma-Daten-Service Stand: 28.10.2024**