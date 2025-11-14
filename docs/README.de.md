---
layout: default
title: "GA-FuL Documentation"
lang: de
---

# Geometric Algebra Fulcrum Library (GA-FuL) - Dokumentation

**Autor:** Ahmad H. Eid
**E-Mail:** ga.computing.eg@gmail.com

**[🇬🇧 English Version](README.en.md)**

---

## Inhaltsverzeichnis

1. [Einführung](#einführung)
2. [Installation und Erste Schritte](getting-started.en.md)
3. [Architektur und Design](architecture.en.md)
4. [Design-Prinzipien](design-principles.en.md)
5. [API-Referenz](api-reference.en.md)
6. [Verwendungsbeispiele](examples.en.md)
7. [Projektstruktur](project-structure.en.md)
8. [Zitation](#zitation)

---

## Einführung

Die **Geometric Algebra Fulcrum Library (GA-FuL)** ist eine vereinheitlichte, generische C#-Bibliothek für Berechnungen mit geometrischer Algebra unter Verwendung beliebiger Skalartypen (Gleitkommazahlen, rationale Zahlen, symbolische Ausdrücke usw.). GA-FuL kann für das Prototyping geometrischer Algorithmen auf Basis der leistungsstarken Mathematik der geometrischen Algebra verwendet werden. Die GA-FuL-Codebasis ermöglicht numerische Berechnungen, symbolische Manipulationen und optimierte Code-Generierung.

### Was ist Geometrische Algebra?

**Geometrische Algebra (GA)** ist eine mächtige mathematische Sprache, die viele algebraische Werkzeuge unter demselben Rahmenwerk mathematischer Operationen vereint. Solche Werkzeuge umfassen beispielsweise:

- Reelle Vektoren
- Komplexe Zahlen
- Quaternionen
- Oktonionen
- Spinoren
- Matrizen
- und viele mehr

Das wichtigste Merkmal von GA ist jedoch, dass sie den **geometrischen Denkprozess** über viele scheinbar unterschiedliche Anwendungsbereiche hinweg vereint.

### Warum der Name "Geometric Algebra Fulcrum"?

Der Name hat zwei Bedeutungsebenen:

1. **Mathematischer Fulcrum:** Geometrische Algebra fungiert als **mathematischer Hebelarm** für geometrisches Denken über wissenschaftliche und technische Bereiche hinweg. Wir können diesen "Geometric Algebra Fulcrum" verwenden, um die abstrakten idealen Bedürfnisse des geometrischen Denkens nahtlos mit den konkreten Werkzeugen der algebraischen Manipulation unter einem vereinheitlichenden mathematischen Rahmenwerk auszubalancieren.

2. **Computationaler Fulcrum:** Es besteht Bedarf an Software-Werkzeugen, die als Drehpunkt fungieren, d.h. ein **Fulcrum**, für das Prototyping und die Implementierung verschiedener Arten von Berechnungen auf GAs Multivektoren. GA-FuL soll die Rolle eines **Computational Fulcrum** für das Prototyping von Algorithmen und die Implementierung von Software auf Basis von **Geometrischer Algebra** spielen.

---

## Hauptmerkmale

### 1. **Generische Skalar-Abstraktion**
- Unterstützung für verschiedene Skalar-Darstellungen:
  - Gleitkommazahlen (32-bit, 64-bit)
  - Beliebig genaue Dezimalzahlen
  - Rationale Zahlen
  - Symbolische Ausdrücke (Mathematica, SymPy, etc.)
  - Mehrdimensionale Arrays und Tensoren
  - Abgetastete Signale für Signalverarbeitung

### 2. **Hochleistungs-Generic-Implementierung** 🚀
- **Generic<double> ist 1,24-2,31x SCHNELLER** als Float64-Spezialisierung
- Aktuelle Optimierungen (2025-10):
  - Norm-Operationen: **1,74-2,31x Beschleunigung**
  - Skalarprodukt (Sp): Overhead reduziert von 33% → 14%
  - Linke Kontraktion (Lcp): Overhead reduziert von 9% → 5,2%
  - Rechte Kontraktion (Rcp): Overhead reduziert von 9% → 6,0%
- **16-33% weniger Speicher** bei Verwendung von Generic<T>
- Typ-spezifische Fast-Paths mit JIT-Devirtualisierung
- Siehe [Performance-Dokumentation](../GENERIC_VS_SPECIALIZED_PERFORMANCE.md) für vollständige Analyse

### 3. **Speichereffiziente Sparse-Multivektoren**
- Optimierte Datenstrukturen für dünnbesetzte Multivektoren in hochdimensionalen GAs
- Unterstützung für Räume mit bis zu 64 Dimensionen (optimiert)
- Unterstützung für beliebige Dimensionen (generisch)

### 4. **Metaprogrammierung-Fähigkeiten**
- Automatische Code-Generierung aus GA-Ausdrücken
- Optimierung durch:
  - Konstantenpropagierung
  - Common Subexpression Elimination
  - Symbolische Vereinfachung
  - Genetische Programmierung für weitere Optimierung
- Zielsprachen:
  - C/C++
  - C#
  - Java
  - JavaScript
  - Python
  - MATLAB
  - CUDA (geplant)

### 5. **Geschichtetes System-Design**
- **Algebra-Schicht:** Grundlegende GA-Operationen und Datenstrukturen
- **Modellierungs-Schicht:** Geometrische Modellierung und Visualisierung
- **Metaprogrammierungs-Schicht:** Code-Generierung und Optimierung
- **System-Utilities-Schicht:** Hilfsdienste für Text, Code und Web-Grafiken

### 6. **Data-Oriented Programming (DOP)**
- Trennung von Verhalten und Daten
- Verwendung generischer Datenstrukturen
- Unveränderliche Daten
- Trennung von Datenrepräsentation und Datenschema

---

## Anwendungsbereiche

GA-FuL kann in verschiedenen Bereichen eingesetzt werden:

- **Computergrafik und Visualisierung**
- **Robotik und Bewegungssteuerung**
- **Physik-Simulationen**
- **Signal- und Bildverarbeitung**
- **Maschinelles Lernen und KI**
- **Computer Vision**
- **Mathematische Modellierung**
- **Code-Generierung für High-Performance Computing**

---

## Projektkomponenten

Das GA-FuL-Projekt besteht aus mehreren Modulen:

| Modul | Beschreibung |
|-------|--------------|
| **GeometricAlgebraFulcrumLib.Algebra** | Kern-Algebra-Schicht mit GA-Operationen |
| **GeometricAlgebraFulcrumLib.Modeling** | Geometrische Modellierung und Visualisierung |
| **GeometricAlgebraFulcrumLib.MetaProgramming** | Code-Generierung und Optimierung |
| **GeometricAlgebraFulcrumLib.Mathematica** | Integration mit Wolfram Mathematica |
| **GeometricAlgebraFulcrumLib.Matlab** | MATLAB-Integration und Toolbox |
| **GeometricAlgebraFulcrumLib.Applications** | Anwendungsbeispiele |
| **GeometricAlgebraFulcrumLib.Utilities** | Hilfsbibliotheken für Text, Code und Strukturen |
| **GeometricAlgebraFulcrumLib.Benchmarks** | Performance-Benchmarks |
| **GeometricAlgebraFulcrumLib.UnitTests** | Unit-Tests |

Weitere Details finden Sie in der [Projektstruktur-Dokumentation](project-structure.en.md).

---

## Systemanforderungen

- **.NET 8.0** oder höher
- **C# 12** (latest)
- Optional:
  - Wolfram Mathematica (für symbolische Berechnungen)
  - MATLAB (für MATLAB-Integration)

---

## Testing & Qualität

**Status (2025-10-27)**: 🎉 **97,92% Tests bestehen!** 🎉

- **Gesamt Tests**: 1.153
- **Bestanden**: 1.129 (97,92%) ✅
- **Fehlgeschlagen**: 0
- **Übersprungen**: 24
- **Code-Abdeckung**: ~52%

**Test-Suites**:
| Komponente | Tests | Erfolgsquote |
|-----------|-------|--------------|
| Algebra-Operationen | 133 | 100% 🎯 |
| Lineare Abbildungen | 121 | 100% |
| AutoDiff | 69 | 100% |
| Modellierung (CGa/PGA) | 507 | 91% |
| Utilities | 295 | 99,7% |

Siehe [ISSUES_TO_FIX.md](https://github.com/ga-explorer/GeometricAlgebraFulcrumLib/blob/main/docs/status/ISSUES_TO_FIX.md) für Details.

---

## Erste Schritte

Für eine schnelle Einführung in GA-FuL lesen Sie bitte die [Getting Started Guide](getting-started.en.md).

Ein einfaches Beispiel:

```csharp
// Definiere den Skalarprozessor für 64-Bit-Gleitkommazahlen
var scalarProcessor = ScalarProcessorOfFloat64.Instance;

// Erstelle einen CGA-Raum
var cga = CGaFloat64GeometricSpace5D.Instance;

// Kodiere Punkte als CGA-Nullvektoren
var point1 = cga.Encode.IpnsRound.Point(3.5, 4.3, 2.6);
var point2 = cga.Encode.IpnsRound.Point(-2.1, 3.4, 5.0);

// Führe GA-Operationen aus (vereinfachtes Beispiel)
var result = point1.Op(point2);
```

> **Performance-Hinweis:** Dieses Beispiel verwendet die Float64-Spezialisierungs-API aus Gründen der Einfachheit. Für bessere Performance (**27% schneller** im Durchschnitt, bis zu **2,31x schneller** für Norm-Operationen) sollten Sie die Generic<T>-Implementierung in Betracht ziehen. Siehe [Getting Started Guide](getting-started.en.md) für Generic<T>-Beispiele und [Performance-Dokumentation](../GENERIC_VS_SPECIALIZED_PERFORMANCE.md) für detaillierte Benchmarks.

Weitere Beispiele finden Sie in der [Beispiele-Dokumentation](examples.en.md).

---

## Zitation

Wenn Sie GA-FuL in Ihrer Forschung oder Ihrem Projekt verwenden, zitieren Sie bitte folgenden Artikel:

**Eid, A.H.; Montoya, F.G.** "Developing GA-FuL: A Generic Wide-Purpose Library for Computing with Geometric Algebra." *Mathematics* 2024, 12, 2272.
DOI: [10.3390/math12142272](https://doi.org/10.3390/math12142272)

```bibtex
@Article{Eid2024,
  author    = {Eid, Ahmad Hosny and Montoya, Francisco G.},
  journal   = {Mathematics},
  title     = {Developing GA-FuL: A Generic Wide-Purpose Library for Computing with Geometric Algebra},
  year      = {2024},
  issn      = {2227-7390},
  month     = jul,
  number    = {14},
  pages     = {2272},
  volume    = {12},
  doi       = {10.3390/math12142272},
  publisher = {MDPI AG},
}
```

---

## Lizenz

Weitere Informationen zur Lizenz finden Sie in der [LICENSE](../LICENSE)-Datei im Hauptverzeichnis des Projekts.

---

## Ressourcen

- **Haupt-Repository:** [GA-FuL auf GitHub](https://github.com/ga-explorer/GeometricAlgebraFulcrumLib)
- **Publikation:** [MDPI Mathematics - GA-FuL Artikel](https://www.mdpi.com/2227-7390/12/14/2272)
- **Vorgänger-Projekt:** [GMac](https://github.com/ga-explorer/GMac)

---

## Kontakt

Für Fragen, Vorschläge oder Zusammenarbeit kontaktieren Sie bitte:

**Ahmad H. Eid**
E-Mail: ga.computing.eg@gmail.com

---

## Dokumentationsstruktur

Diese Dokumentation ist in mehrere thematische Abschnitte unterteilt:

- **[Getting Started](getting-started.en.md)** - Installation und erste Schritte
- **[Architektur](architecture.en.md)** - System-Design und Komponentenschichten
- **[Design-Prinzipien](design-principles.en.md)** - Kern-Design-Absichten und DOP-Prinzipien
- **[Beispiele](examples.en.md)** - Umfassende Code-Beispiele
- **[API-Referenz](api-reference.en.md)** - Detaillierte API-Dokumentation
- **[Projektstruktur](project-structure.en.md)** - Modul-Organisation und Abhängigkeiten

**Sprachoptionen:**
- **English:** `.en.md` Dateien
- **Deutsch:** `.de.md` Dateien

---

**Letzte Aktualisierung**: 2025-10-27 | **Status**: Aktiv | **Test-Erfolgsquote**: 97,92% ✅ | **Performance**: Generic<T> 1,24-2,31x schneller 🚀

**Hinweis:** Diese Dokumentation bietet umfassende Informationen über GA-FuL. Nutzen Sie das Inhaltsverzeichnis, um zu den gewünschten Themen zu navigieren.
