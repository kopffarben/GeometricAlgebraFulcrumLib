---
layout: default
title: "Architektur"
lang: de
---

# GA-FuL Architektur

**[🇬🇧 English Version](architecture.en.md)**

## Inhaltsverzeichnis

1. [Übersicht](#übersicht)
2. [Komponenten-Schichten](#komponenten-schichten)
3. [Algebra-Schicht](#algebra-schicht)
4. [Modellierungs-Schicht](#modellierungs-schicht)
5. [Metaprogrammierungs-Schicht](#metaprogrammierungs-schicht)
6. [System-Utilities-Schicht](#system-utilities-schicht)
7. [Datenfluss](#datenfluss)

---

## Übersicht

Die Architektur von GA-FuL folgt einem **geschichteten Design**, bei dem jede Schicht spezifische Verantwortlichkeiten übernimmt und auf den Funktionalitäten der darunterliegenden Schichten aufbaut. Diese Architektur ermöglicht es Benutzern mit unterschiedlichem Hintergrund, die passende Abstraktionsebene für ihre Bedürfnisse zu wählen.

```
┌─────────────────────────────────────────────┐
│      Metaprogrammierungs-Schicht            │
│  (Code-Generierung & Optimierung)           │
├─────────────────────────────────────────────┤
│         Modellierungs-Schicht               │
│  (Geometrie, Visualisierung, Kalkül)        │
├─────────────────────────────────────────────┤
│           Algebra-Schicht                   │
│  (GA-Operationen, Skalare, Multivektoren)   │
├─────────────────────────────────────────────┤
│       System-Utilities-Schicht              │
│  (Datenstrukturen, Text, Code, Web)         │
└─────────────────────────────────────────────┘
```

---

## Komponenten-Schichten

### 1. Algebra-Schicht

**Zweck:** Grundlegende GA-Operationen und Datenstrukturen

**Hauptkomponenten:**
- Skalar-Prozessoren
- Basis-Blades
- Multivektoren
- Lineare Abbildungen
- Transformationen

**Details:** Siehe [Algebra-Schicht](#algebra-schicht)

---

### 2. Modellierungs-Schicht

**Zweck:** Geometrische Modellierung und Visualisierung

**Hauptkomponenten:**
- Geometrie-Subsystem
- Visualisierungs-Subsystem
- Kalkül-Subsystem (in Entwicklung)

**Details:** Siehe [Modellierungs-Schicht](#modellierungs-schicht)

---

### 3. Metaprogrammierungs-Schicht

**Zweck:** Code-Generierung und Optimierung

**Hauptkomponenten:**
- Meta-Kontext
- Expression Trees
- Code-Optimierer
- Code-Composer

**Details:** Siehe [Metaprogrammierungs-Schicht](#metaprogrammierungs-schicht)

---

### 4. System-Utilities-Schicht

**Zweck:** Grundlegende Dienste für alle anderen Schichten

**Hauptkomponenten:**
- Datenstrukturen
- Text-Utilities
- Code-Utilities
- Web-Grafik-Utilities

**Details:** Siehe [System-Utilities-Schicht](#system-utilities-schicht)

---

## Algebra-Schicht

Die Algebra-Schicht ist die Grundlage von GA-FuL und erfüllt die Kern-Design-Intentionen CDI-1 und CDI-2.

### Skalar-Repräsentation

```
IScalarProcessor<T>
├── INumericScalarProcessor<T>
│   ├── ScalarProcessorOfFloat32
│   ├── ScalarProcessorOfFloat64
│   ├── ScalarProcessorOfDecimal
│   ├── ScalarProcessorOfRational
│   └── ScalarProcessorOfComplex
└── ISymbolicScalarProcessor<T>
    ├── MathematicaScalarProcessor
    └── [Zukünftige CAS-Integrationen]
```

**Wichtige Klassen:**

#### `IScalarProcessor<T>`
Generische Schnittstelle für Skalar-Operationen:
- Arithmetik (Addition, Subtraktion, Multiplikation, Division)
- Transzendentale Funktionen (Trigonometrie, Exponential, Logarithmus)
- Vergleichsoperationen

#### `Scalar<T>`
Thin-Wrapper-Klasse für einfachere API:
```csharp
// Ohne Wrapper (kompliziert):
w = scalarProcessor.Add(x, scalarProcessor.Times(y, z));

// Mit Wrapper (einfach):
w = x + y * z;
```

---

### Basis-Blades

**Repräsentation:**

Basis-Blades $e_{i_1, i_2, \ldots, i_k}$ werden intern durch eine sortierte Menge von Indizes dargestellt.

```
IIndexSet
├── EmptyIndexSet (leere Menge)
├── SingleIndexSet (ein Element)
├── UInt64IndexSet (für Dimensionen < 64)
├── DenseIndexSet (beliebige Größe, Array-basiert)
└── SparseIndexSet (beliebige Größe, HashSet-basiert)
```

**Wichtige Klassen:**

#### `XGaBasisBlade`
Thin-Wrapper um `IIndexSet` mit Methoden für:
- Geometrisches Produkt
- Äußeres Produkt
- Inneres Produkt
- Reverse-Operation
- Weitere bilineare Produkte

#### `XGaSignedBasisBlade`
Erweitert `XGaBasisBlade` um ein Vorzeichen (±1 oder 0)

#### `XGaMetric`
Verarbeitet Basis-Blades mit spezifischer Metrik-Signatur $(p, q, r)$:
- $p$: Anzahl Basisvektoren mit $e_i^2 = +1$
- $q$: Anzahl Basisvektoren mit $e_i^2 = -1$
- $r$: Anzahl Basisvektoren mit $e_i^2 = 0$

---

### Multivektoren

**Datenstruktur:**

```
Multivektor
└── IReadOnlyDictionary<int, XGaKVector<T>>
    └── Grade k → k-Vektor
        └── IReadOnlyDictionary<IIndexSet, T>
            └── Basis-Blade → Skalar
```

**Vorteile dieser Struktur:**
- Hochgradig sparse Repräsentation
- Effiziente Speichernutzung
- Flexible Dimensionalität

**Wichtige Klassen:**

#### `XGaMultivector<T>`
Generischer Multivektor mit beliebigem Skalartyp `T`

#### `XGaKVector<T>`
Repräsentiert einen k-Vektor (alle Komponenten haben denselben Grad)

#### `XGaProcessor<T>`
Hauptprozessor für Multivektor-Operationen:
- Algebraische Operationen
- Geometrische Produkte
- Grades-Extraktion
- Normalisierung

#### `XGaMultivectorComposer<T>`
Composer-Klasse für das Erstellen von Multivektoren (DOP-Prinzip 3)

---

### Transformationen

GA-FuL bietet Klassen für gängige GA-Transformationen:

| Transformation | Klasse | Beschreibung |
|----------------|--------|--------------|
| **Outermorphism** | `XGaOutermorphism<T>` | Allgemeine lineare Transformation |
| **Versor** | `XGaVersor<T>` | Orthogonale Operatoren |
| **Rotor** | `XGaRotor<T>` | Rotationen |
| **Reflektor** | `XGaReflector<T>` | Spiegelungen |
| **Projektor** | `XGaProjector<T>` | Projektionen |

---

### Implementierungs-Architekturen: Float64 Specialized vs Generic<T>

GA-FuL bietet **zwei parallele Implementierungs-Architekturen** für Gleitkomma-Berechnungen:

#### 1. **Generic<T> Implementierung** (Empfohlen für Performance) 🚀

**`XGaProcessor<double>` / `XGaProcessor<float>`**
- **1,24-2,31x SCHNELLER** als Float64 Specialized
- Funktioniert mit jedem Skalar-Typ T (double, float, rational, symbolisch, etc.)
- Typ-spezifische Fast-Paths mit JIT-Devirtualisierung
- 16-33% weniger Speicherverbrauch
- Moderne .NET-Optimierungen (Span<T>, Value-Semantik)

**Performance:**
- High-Level CGA-Operationen: **1,27x schneller**
- Norm-Operationen: **1,74-2,31x schneller**
- Scalar Product Overhead: **14%** (verbessert von 33%)
- Left Contraction Overhead: **5,2%** (verbessert von 9%)

#### 2. **Float64 Specialized Implementierung** (Einfachere API)

**`XGaFloat64Processor` / `RGaFloat64Multivector`**
- Spezialisiert nur für 64-Bit-Gleitkommazahlen
- Einfachere API, gut für Lernen und Prototyping
- Singleton-Pattern für gängige Metriken (Euclidean, Conformal, Projective)
- Baseline-Performance (27% langsamer als Generic<double>)

**Wann Float64 Specialized verwenden:**
- Lernen und Bildungszwecke
- Schnelles Prototyping mit einfacherer Syntax
- Legacy-Code-Kompatibilität

**Wann Generic<T> verwenden:**
- Produktionscode mit optimaler Performance
- Speicher-kritische Anwendungen
- Jeder andere Skalar-Typ außer double

> **💡 Performance-Empfehlung:** Verwenden Sie **Generic<double>** für Produktionscode. Float64 Specialized wird für API-Einfachheit und Rückwärtskompatibilität beibehalten.

---

## Modellierungs-Schicht

Die Modellierungs-Schicht erfüllt CDI-5 und bietet spezialisierte Abstraktionen für verschiedene Anwendungsbereiche.

### Geometrie-Subsystem

**Unterstützte GA-Typen:**

#### 1. Directional GA (Vektor-Algebra)
Standard-Vektor-Algebra mit äußerem Produkt

#### 2. Conformal GA (CGA)
Modellierung von 3D-Geometrie in 5D-CGA-Raum

**Klassen:**
- `XGaConformalSpace<T>`: Basis-Klasse
- `XGaConformalSpace4D<T>`: 4D-CGA (für 2D-Geometrie)
- `XGaConformalSpace5D<T>`: 5D-CGA (für 3D-Geometrie)

**Funktionalitäten:**
- Kodierung geometrischer Objekte (Punkte, Linien, Ebenen, Kreise, Sphären)
- Dekodierung von CGA-Blades
- Geometrische Operationen (Schnitte, Projektionen, Spiegelungen)
- Transformationen (Rotation, Translation, Inversion)

**Beispiel:**
```csharp
var cga = CGaFloat64GeometricSpace5D.Instance;

// Kodiere einen Punkt
var point = cga.Encode.IpnsRound.Point(x, y, z);

// Kodiere eine Sphäre
var sphere = cga.Encode.IpnsRound.RealSphere(radius, centerX, centerY, centerZ);

// Schneide zwei Objekte
var intersection = sphere.Op(plane);
```

> **⚡ Performance-Hinweis:** Dieses Beispiel verwendet `CGaFloat64GeometricSpace5D` (Float64 Specialized) für Einfachheit. Für **27% schnellere** CGA-Operationen in Produktionscode sollten Sie die Generic<T>-Implementierung in Betracht ziehen.

#### 3. Projective GA (PGA)
Projektive Geometrie-Modellierung

**Besonderheit in GA-FuL:**
CGA und PGA sind innerhalb desselben algebraischen Raums implementiert (basierend auf dem DPGA-Konzept), was es ermöglicht, beide Repräsentationen frei zu mischen.

---

### Visualisierungs-Subsystem

**Unterstützte Backends:**

#### Babylon.js
- JavaScript-Code-Generierung für 3D-Visualisierung
- Unterstützung für statische und animierte Objekte
- Interaktive Szenen im Browser

#### Selenium WebDriver
- Browser-Automatisierung für Video-Erstellung
- Kombiniert einzelne Frames zu Videos

**Beispiele:**
Im Repository unter `GeometricAlgebraFulcrumLib.Visualizations`

---

### Kalkül-Subsystem

**Status:** In Entwicklung

**Geplante Funktionalitäten:**
- Geometrischer Kalkül auf Multivektoren
- Ableitungen
- Integrale
- Differentialformen

---

## Metaprogrammierungs-Schicht

Die Metaprogrammierungs-Schicht erfüllt CDI-3 und ermöglicht automatische Code-Generierung.

### Workflow

```
1. MCO initialisieren
      ↓
2. Input-Parameter definieren
      ↓
3. GA-Operationen beschreiben
      ↓
4. Output-Variablen auswählen
      ↓
5. Variablennamen setzen
      ↓
6. Code optimieren
      ↓
7. CCO initialisieren
      ↓
8. Finalen Code generieren
```

---

### Meta-Kontext

**`MetaContext`**: Hauptklasse für Metaprogrammierung

**Funktionalitäten:**
- Verwaltung von Meta-Expressions
- Automatische Konvertierung von GA-Operationen zu Skalar-Operationen
- Optimierungen

**Meta-Expression-Hierarchie:**

```
IMetaExpression
├── IMetaExpressionAtomic
│   ├── MetaExpressionNumber (Konstante)
│   └── MetaExpressionVariable (Parameter)
└── IMetaExpressionComposite
    ├── MetaExpressionNegative
    ├── MetaExpressionAdd
    ├── MetaExpressionSubtract
    ├── MetaExpressionTimes
    ├── MetaExpressionDivide
    ├── MetaExpressionPower
    └── MetaExpressionFunction (sin, cos, exp, etc.)
```

---

### Optimierungen

Der MCO führt folgende Optimierungen durch:

1. **Konstanten-Propagierung**
   - Berechnet konstante Ausdrücke zur Kompilierzeit

2. **Common Subexpression Elimination (CSE)**
   - Identifiziert wiederholte Teilausdrücke
   - Extrahiert sie in Zwischenvariablen

3. **Symbolische Vereinfachung**
   - Optional: Verwendung externer CAS (z.B. Mathematica)
   - Vereinfacht komplexe Ausdrücke

4. **Variable Pruning**
   - Entfernt ungenutzte Zwischenvariablen
   - Entfernt redundante Berechnungen

5. **Reduktion der Berechnungsschritte**
   - Optional: Genetische Programmierung
   - Evolutionsstrategie (4+1)

---

### Code-Composer

**`CodeComposer`**: Konvertiert Meta-Expressions in Zielsprachen-Code

**Unterstützte Sprachen:**
- C/C++
- C#
- Java
- JavaScript
- Python
- MATLAB

**Template-basierte Code-Generierung:**
- Einzelne Code-Dateien
- Multi-File-Projekte
- Vollständige Bibliotheken mit Ordnerstruktur

**Beispiel-Workflow:**
```csharp
// 1. MCO erstellen
var context = new MetaContext();
var scalarProcessor = context.ScalarProcessor;

// 2. Prozessor für Metaprogrammierung erstellen
var processor = XGaProcessor<IMetaExpression>.CreateEuclidean(scalarProcessor);

// 3. Input-Parameter
var x = context.CreateParameter("x");
var y = context.CreateParameter("y");

// 4. GA-Operationen (composer pattern)
var multivector1 = processor.CreateMultivectorComposer()
    .SetVectorTerm(0, x)
    .SetVectorTerm(1, y)
    .SetVectorTerm(2, 0)
    .GetMultivector();

var multivector2 = processor.CreateMultivectorComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 1)
    .SetVectorTerm(2, 1)
    .GetMultivector();

var result = multivector1.Gp(multivector2);

// 5. Output definieren
context.SetOutput("result", result);

// 6. Optimieren
context.Optimize();

// 7. Code generieren
var codeComposer = new CSharpCodeComposer();
var code = codeComposer.Generate(context);
```

---

## System-Utilities-Schicht

Die Utilities-Schicht bietet grundlegende Dienste für alle anderen Schichten.

### Datenstrukturen-Subsystem

**Wichtige Klassen:**

#### `IReadOnlyDictionary<TKey, TValue>` Implementierungen
- `EmptyDictionary<TKey, TValue>`: Leeres Dictionary
- `SingleItemDictionary<TKey, TValue>`: Ein Element
- `Dictionary<TKey, TValue>`: Standard-Dictionary

**Verwendung:**
Effiziente Speicherung von sparse Daten (Multivektoren, k-Vektoren)

---

### Text-Utilities-Subsystem

**Funktionalitäten:**
- Formatierte Text-Generierung
- LaTeX-Code-Composition
- Parametrische Text-Templates
- Hierarchische Ordner/Datei-Erstellung

**Wichtige Klassen:**
- `LinearTextComposer`: Lineare Text-Zusammenstellung
- `ParametricTextComposer`: Template-basierte Text-Generierung
- `LaTeXComposer`: LaTeX-Dokument-Erstellung

---

### Code-Utilities-Subsystem

**Funktionalitäten:**
- Sprachunabhängige Abstract Syntax Trees (ASTs)
- Code-Generierung aus ASTs
- Code-Formatierung

**Verwendung:**
Unterstützt die Metaprogrammierungs-Schicht

---

### Web-Grafik-Utilities-Subsystem

**Funktionalitäten:**
- Web-basierter Grafik-Code-Generierung
- Unterstützung für Babylon.js
- SVG-Generierung
- HTML/CSS/JavaScript-Utilities

**Verwendung:**
Unterstützt die Visualisierungs-Subsystem der Modellierungs-Schicht

---

## Datenfluss

### Typischer Workflow für numerische Berechnungen

```
1. Skalar-Prozessor wählen
   ↓
2. GA-Prozessor erstellen
   ↓
3. Multivektoren definieren
   ↓
4. GA-Operationen durchführen
   ↓
5. Ergebnisse extrahieren
```

### Typischer Workflow für Code-Generierung

```
1. Symbolischer Skalar-Prozessor wählen
   ↓
2. Meta-Kontext erstellen
   ↓
3. Input-Parameter definieren
   ↓
4. GA-Operationen beschreiben
   ↓
5. Optimieren
   ↓
6. Code generieren
```

### Typischer Workflow für Visualisierung

```
1. Geometrische Objekte modellieren
   ↓
2. Visualisierungs-Kontext erstellen
   ↓
3. Rendering-Code generieren
   ↓
4. In Browser/Framework rendern
```

---

## Design-Entscheidungen

### Warum geschichtete Architektur?

1. **Separation of Concerns**: Jede Schicht hat klare Verantwortlichkeiten
2. **Wiederverwendbarkeit**: Untere Schichten können unabhängig genutzt werden
3. **Erweiterbarkeit**: Neue Funktionen können hinzugefügt werden ohne bestehende zu brechen
4. **Testbarkeit**: Jede Schicht kann isoliert getestet werden

### Warum Data-Oriented Programming?

1. **Reduzierte Komplexität**: Trennung von Daten und Verhalten
2. **Bessere Wartbarkeit**: Einfachere Code-Struktur
3. **Höhere Flexibilität**: Generische Datenstrukturen
4. **Unveränderlichkeit**: Thread-Sicherheit und Vorhersagbarkeit

---

## Performance-Überlegungen

### Optimierte Implementierungen

| Szenario | Implementierung | Relative Performance | Hinweise |
|----------|-----------------|----------------------|----------|
| **Generic<T> Produktion** | `XGaProcessor<double>` | **1,24-2,31x schneller** 🚀 | Empfohlen für Produktion |
| **Float64 Lernen/Prototyping** | `XGaFloat64Processor` | 1,00x (Baseline) | Einfachere API |
| **Kleine Dimensionen (< 12)** | Lookup-Tabellen | Sehr schnell | Beide Architekturen profitieren |
| **Mittlere Dimensionen (< 64)** | UInt64-basierte Indices | Schnell | Beide Architekturen profitieren |
| **Große Dimensionen** | Sparse Dictionary | Moderat | Beide Architekturen profitieren |
| **Code-Generierung** | MetaProgramming Layer | Maximale Performance | Sprachunabhängige Optimierung |

**Performance-Aufschlüsselung (Generic<double> vs Float64 Specialized):**
- High-Level CGA-Operationen: **1,27x schneller**
- Norm-Operationen: **1,74-2,31x schneller**
- Speicherverbrauch: **16-33% weniger**
- Scalar Product: 14% Overhead (reduziert von 33%)
- Left Contraction: 5,2% Overhead (reduziert von 9%)

> **📊 Detaillierte Benchmarks:** Siehe [GENERIC_VS_SPECIALIZED_PERFORMANCE.md](../GENERIC_VS_SPECIALIZED_PERFORMANCE.md) für vollständige Analyse.

---

## Erweiterbarkeit

### Neue Skalar-Typen hinzufügen

1. Implementiere `IScalarProcessor<T>` oder `INumericScalarProcessor<T>`
2. Registriere den Prozessor
3. Verwende ihn mit `XGaProcessor<T>`

### Neue Geometrien hinzufügen

1. Erweitere `XGaConformalSpace<T>` oder erstelle neue Basis-Klasse
2. Implementiere Encoding/Decoding-Methoden
3. Füge spezifische Operationen hinzu

### Neue Zielsprachen für Code-Generierung

1. Erweitere `CodeComposer`
2. Implementiere sprach-spezifische Syntax
3. Registriere den Composer

---

## Zusammenfassung

Die GA-FuL-Architektur bietet:

✓ **Flexibilität** durch generische Skalare und Multivektoren
✓ **Effizienz** durch optimierte Datenstrukturen
✓ **Erweiterbarkeit** durch geschichtetes Design
✓ **Wartbarkeit** durch Data-Oriented Programming
✓ **Leistung** durch mehrere Optimierungsstufen
✓ **Benutzerfreundlichkeit** durch verschiedene Abstraktionsebenen

---

**Letzte Aktualisierung**: 2025-10-27 | **Architektur**: Parallele Float64 & Generic<T> | **Performance**: Generic<T> 1,24-2,31x schneller 🚀

[← Zurück zur Hauptdokumentation](README.de.md)
