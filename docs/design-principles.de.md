---
layout: default
title: "Design-Prinzipien"
lang: de
---

# GA-FuL Design-Prinzipien

**[🇬🇧 English Version](design-principles.en.md)**

## Inhaltsverzeichnis

1. [Übersicht](#übersicht)
2. [Kern-Design-Intentionen (CDIs)](#kern-design-intentionen-cdis)
3. [Data-Oriented Programming (DOP)](#data-oriented-programming-dop)
4. [Implementierung der DOP-Prinzipien](#implementierung-der-dop-prinzipien)
5. [Praktische Beispiele](#praktische-beispiele)

---

## Übersicht

Das Design von GA-FuL basiert auf zwei Hauptkonzepten:

1. **Core Design Intentions (CDIs)**: Fünf Hauptziele, die das System erfüllen soll
2. **Data-Oriented Programming (DOP)**: Vier Prinzipien für die praktische Umsetzung

Diese Prinzipien entstanden aus der Erfahrung mit dem Vorgängersystem [GMac](https://github.com/ga-explorer/GMac) und adressieren dessen Schwächen.

---

## Kern-Design-Intentionen (CDIs)

### CDI-1: Abstraktion von Multivektor-Operationen

**Ziel:** Trennung von Multivektor-Operationen und konkreten Skalar-Repräsentationen

**Problem:**
- Skalare können auf viele Arten repräsentiert werden:
  - IEEE 754 Gleitkommazahlen (32-bit, 64-bit)
  - Beliebig genaue Dezimalzahlen
  - Rationale Zahlen (Brüche)
  - Symbolische Ausdrücke (für CAS)
  - Mehrdimensionale Arrays (für ML/AI)
  - Abgetastete Signale (für Signalverarbeitung)

**Lösung:**
- Generische Implementierung über `IScalarProcessor<T>`
- Alle GA-Operationen unabhängig vom Skalartyp
- Einfache Integration neuer Skalartypen

**Beispiel:**
```csharp
// Derselbe Code funktioniert mit allen Skalartypen
var processor = XGaProcessor<T>.CreateEuclidean(scalarProcessor);
var v1 = processor.CreateComposer()
    .SetVectorTerm(0, a)
    .SetVectorTerm(1, b)
    .SetVectorTerm(2, c)
    .GetVector();
var v2 = processor.CreateComposer()
    .SetVectorTerm(0, x)
    .SetVectorTerm(1, y)
    .SetVectorTerm(2, z)
    .GetVector();
var result = v1.Gp(v2); // Geometrisches Produkt

// T kann sein: double, float, decimal, BigDecimal, Expr, etc.
```

**Vorteile:**
- ✓ Ein Code für alle Skalartypen
- ✓ Einfaches Hinzufügen neuer Skalartypen
- ✓ Maximale Wiederverwendbarkeit

---

### CDI-2: Reduzierung der Speicheranforderungen

**Ziel:** Effiziente Speicherung von sparse Multivektoren in hochdimensionalen GAs

**Problem:**
- Ein voller Multivektor in $n$-dimensionaler GA benötigt $2^n$ Skalare
- Ein voller $k$-Vektor benötigt $\binom{n}{k}$ Skalare
- Beispiele (bei 32-bit floats):
  - 30D GA, voller Multivektor: 8 GB
  - 30D GA, voller 15-Vektor: 1.16 GB
  - Dies ist für die meisten Anwendungen unpraktisch

**Realität:**
- Praktische GA-Anwendungen nutzen meist **sparse Multivektoren**
- Beispiel: 5D-CGA kann 3D-Geometrie mit nur 5 von 32 Skalaren repräsentieren

**Lösung:**
- Dictionary-basierte Speicherung: `IReadOnlyDictionary<IIndexSet, T>`
- Nur nicht-null Komponenten werden gespeichert
- Automatische Auswahl der effizientesten Datenstruktur

**Speicher-Hierarchie:**

| Multivektor-Typ | Datenstruktur | Speicher-Effizienz |
|-----------------|---------------|-------------------|
| Zero-Multivektor | `EmptyDictionary` | 0 Skalare |
| Single-Komponente | `SingleItemDictionary` | 1 Skalar + Index |
| Sparse | `Dictionary<IIndexSet, T>` | n Skalare + Indices |
| Dense (klein) | Lookup-Tabellen | Optimiert für kleine n |

**Index-Set-Hierarchie:**

| Dimensions-Bereich | IIndexSet-Implementierung | Speicher |
|-------------------|---------------------------|----------|
| Leere Menge | `EmptyIndexSet` | 0 Bytes |
| 1 Element | `SingleIndexSet` | 4 Bytes |
| < 64 Dimensionen | `UInt64IndexSet` | 8 Bytes |
| Beliebig (dicht) | `DenseIndexSet` | Array von ints |
| Beliebig (sparse) | `SparseIndexSet` | HashSet |

**Beispiel:**
```csharp
// 100D-GA, sparse Multivektor mit 10 Komponenten
var composer = processor.CreateMultivectorComposer();
composer.SetTerm(indexSet1, scalar1);
composer.SetTerm(indexSet2, scalar2);
// ... 10 Terme total

var mv = composer.GetMultivector();

// Speicher: ~10 Skalare + 10 Index-Sets
// Voller Multivektor würde 2^100 Skalare benötigen!
```

**Vorteile:**
- ✓ Hochdimensionale GAs werden praktikabel
- ✓ Automatische Optimierung
- ✓ Flexible Dimensionalität

---

### CDI-3: Metaprogrammierung-Fähigkeiten

**Ziel:** Code-Generierung aus GA-Ausdrücken für optimierte Performance

**Problem:**
- Generische Implementierung ist flexibel aber manchmal langsam
- High-Performance-Anwendungen brauchen optimierten Code
- Manuelles Schreiben von optimiertem Code ist fehleranfällig

**Lösung:**
- Automatische Code-Generierung aus GA-Ausdrücken
- Optimierung durch:
  - Konstantenpropagierung
  - Common Subexpression Elimination
  - Symbolische Vereinfachung
  - Genetische Programmierung
- Unterstützung für mehrere Zielsprachen

**Workflow:**
```
High-Level GA-Operationen
        ↓
Expression Tree
        ↓
Optimierung
        ↓
Zielsprachen-Code (C++, C#, CUDA, etc.)
```

**Beispiel:**
```csharp
// 1. Definiere Berechnungen in GA-FuL
var context = new MetaContext();
var x = context.CreateParameter("x");
var y = context.CreateParameter("y");
var scalarProcessor = context.ScalarProcessor;
var processor = XGaProcessor<IMetaExpression>.CreateEuclidean(scalarProcessor);

var v1 = processor.CreateComposer()
    .SetVectorTerm(0, x)
    .SetVectorTerm(1, y)
    .SetVectorTerm(2, 0)
    .GetVector();
var v2 = processor.CreateComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 1)
    .SetVectorTerm(2, 1)
    .GetVector();
var result = v1.Gp(v2);

// 2. Optimiere
context.Optimize();

// 3. Generiere C++-Code
var cppCode = new CppCodeComposer().Generate(context);

// Resultat: Hochoptimierter C++-Code
```

**Anwendungsfälle:**
- CUDA-Code für GPU-Beschleunigung
- Embedded Systems (C/C++)
- Web-Anwendungen (JavaScript)
- Wissenschaftliche Berechnungen (MATLAB/Python)

**Vorteile:**
- ✓ Beste Performance für Production-Code
- ✓ Fehlerfreie Code-Generierung
- ✓ Automatische Optimierung
- ✓ Mehrere Zielsprachen

---

### CDI-4: Geschichtetes System-Design

**Ziel:** Organisation der Komplexität durch Schichten für verschiedene Benutzergruppen

**Problem:**
- CDI-1 bis CDI-3 führen zu hoher Komplexität
- Verschiedene Benutzer haben verschiedene Bedürfnisse
- Manche brauchen abstrakte High-Level-API
- Andere brauchen Low-Level-Kontrolle für Performance

**Lösung:**
Vier Schichten mit zunehmender Abstraktion:

```
Abstraktionsebene
       ↑
       │    ┌────────────────────────────┐
 Hoch  │    │  Metaprogrammierungs-Layer │
       │    ├────────────────────────────┤
       │    │     Modellierungs-Layer     │
       │    ├────────────────────────────┤
       │    │       Algebra-Layer         │
       │    ├────────────────────────────┤
Niedrig│    │   System-Utilities-Layer    │
       │    └────────────────────────────┘
```

**Benutzer-Profile:**

| Benutzertyp | Bevorzugte Schicht | Anwendungsfall |
|-------------|-------------------|----------------|
| **Forscher** | Modellierung | Prototyping geometrischer Algorithmen |
| **Softwareentwickler** | Metaprogrammierung | Code-Generierung für Production |
| **Bibliotheksentwickler** | Algebra | Erweiterung der Funktionalität |
| **Performance-Ingenieur** | Algebra + Utilities | Optimierung kritischer Pfade |

**Beispiele:**

*High-Level (Modellierung):*
```csharp
// Koordinatenfreie, intuitive API
var point = cga.Encode.IpnsRound.Point(x, y, z);
var sphere = cga.Encode.IpnsRound.RealSphere(radius, cx, cy, cz);
var intersection = point.Op(sphere);
```

*Low-Level (Algebra):*
```csharp
// Direkte Kontrolle über Skalare und Basis-Blades
var blade = processor.CreateBasisBlade(indexSet);
var scalar = scalarProcessor.Times(a, b);
var term = processor.CreateTerm(blade, scalar);
```

**Vorteile:**
- ✓ Jeder Benutzer findet passende Abstraktionsebene
- ✓ Klare Trennung der Verantwortlichkeiten
- ✓ Einfache Wartung und Erweiterung

---

### CDI-5: Unified, Generic, Extensible API

**Ziel:** Einheitliche API für verschiedene Anwendungsklassen

**Anforderungen:**
- **Unified:** Konsistente Namenskonventionen und Muster
- **Generic:** Funktioniert mit allen Skalartypen und GA-Metriken
- **Extensible:** Einfaches Hinzufügen neuer Features

**Implementierung:**

**1. Konsistente Namenskonventionen:**
```csharp
// Alle Prozessoren folgen demselben Muster
IScalarProcessor<T>
XGaProcessor<T>
XGaConformalSpace<T>

// Alle Composer folgen demselben Muster
XGaMultivectorComposer<T>
XGaKVectorComposer<T>
```

**2. Generische Typen:**
```csharp
// Ein Template für alle Typen
XGaMultivector<double>
XGaMultivector<float>
XGaMultivector<Expr>  // Symbolisch
XGaMultivector<BigDecimal>
```

**3. Extension Methods:**
```csharp
// Einheitliche API durch Extension Methods
multivector.Gp(other)      // Geometric Product
multivector.Op(other)      // Outer Product
multivector.Lcp(other)     // Left Contraction Product
multivector.Rcp(other)     // Right Contraction Product
multivector.Sp(other)      // Scalar Product
multivector.Reverse()      // Reverse
multivector.GradeInvolution() // Grade Involution
```

**4. Builder-Pattern:**
```csharp
// Fluent API für Konstruktion
var mv = processor.CreateMultivectorComposer()
    .SetTerm(index1, scalar1)
    .SetTerm(index2, scalar2)
    .AddTerm(index3, scalar3)
    .GetMultivector();
```

**Unterstützte Anwendungsklassen:**
- Numerische Berechnungen
- Symbolische Mathematik
- Signalverarbeitung
- Computer Vision
- Robotik
- Visualisierung
- Code-Generierung

**Vorteile:**
- ✓ Leicht zu lernen
- ✓ Konsistent über alle Module
- ✓ IntelliSense-freundlich
- ✓ Einfach zu erweitern

---

## Data-Oriented Programming (DOP)

Traditionelles OOP führte zu zu hoher Komplexität in GA-FuL. DOP bietet eine bessere Alternative.

### Warum DOP statt klassischem OOP?

**Probleme mit klassischem OOP:**
- ❌ Tiefe Kopplung von Daten und Verhalten
- ❌ Komplexe Vererbungshierarchien
- ❌ Schwierige Wartung bei großen Systemen
- ❌ Einkapselung kann zu Inflexibilität führen

**Vorteile von DOP:**
- ✓ Lesbarerer Code
- ✓ Wartbarerer Code
- ✓ Erweiterbarer Code
- ✓ Bessere Testbarkeit

---

### DOP-Prinzip 1: Trennung von Verhalten und Daten

**Regel:** Verhaltenscode und Daten getrennt halten

**In OOP:**
```csharp
// Klassisches OOP: Daten und Verhalten gekoppelt
class Multivector {
    private Dictionary<IIndexSet, T> data;

    public Multivector Gp(Multivector other) {
        // Verhalten im selben Objekt
    }
}
```

**In GA-FuL (DOP):**
```csharp
// DOP: Daten in thin wrapper
class XGaMultivector<T> {
    public IReadOnlyDictionary<int, XGaKVector<T>> Data { get; }
}

// Verhalten in statischen Extension Methods
static class XGaMultivectorExtensions {
    public static XGaMultivector<T> Gp<T>(
        this XGaMultivector<T> mv1,
        XGaMultivector<T> mv2
    ) {
        // Verhalten getrennt von Daten
    }
}
```

**Vorteile:**
- ✓ Daten können von mehreren Verhaltens-Modulen verwendet werden
- ✓ Verhalten kann unabhängig getestet werden
- ✓ Einfacher Code-Reviews

---

### DOP-Prinzip 2: Generische Datenstrukturen

**Regel:** Verwende generelle Datenstrukturen statt spezielle Klassen

**Bevorzugte Strukturen:**
- Arrays/Listen für sequentielle Daten
- Dictionaries/Maps für Key-Value-Paare
- Sets für eindeutige Sammlungen
- Queues für FIFO
- Trees für hierarchische Daten

**In GA-FuL:**

**Multivektoren:**
```csharp
// NICHT eine spezielle Klasse mit interner Array-Struktur
// SONDERN generisches Dictionary
IReadOnlyDictionary<int, XGaKVector<T>>
```

**k-Vektoren:**
```csharp
IReadOnlyDictionary<IIndexSet, T>
```

**Beispiel:**
```csharp
// Zugriff ist einfach und transparent
var grade2Part = multivector[2];  // Dictionary-Zugriff
var scalar = kVector[indexSet];   // Dictionary-Zugriff

// Keine versteckten internen Strukturen
```

**Vorteile:**
- ✓ Transparente Datenstrukturen
- ✓ Standard-Operationen verfügbar
- ✓ Einfach zu debuggen
- ✓ Vertraut für alle Entwickler

---

### DOP-Prinzip 3: Unveränderliche Daten

**Regel:** Daten niemals direkt ändern, sondern neue Versionen erstellen

**Warum Unveränderlichkeit?**
- Thread-Sicherheit
- Vorhersagbares Verhalten
- Einfacheres Debugging
- Funktionale Programmierung-Stil

**Implementierung in GA-FuL:**

**Problem:** Wie erstellt man neue Multivektoren ohne Mutation?

**Lösung:** Composer-Klassen

```csharp
// 1. Composer für Konstruktion (Mutable während Build)
var composer = processor.CreateMultivectorComposer();
composer.SetTerm(index1, scalar1);
composer.SetTerm(index2, scalar2);
composer.AddTerm(index3, scalar3);

// 2. Finaler Multivektor (Immutable)
var multivector = composer.GetMultivector();

// 3. Multivector selbst ist read-only
// multivector.Data ist IReadOnlyDictionary<...>
```

**Pattern:**
```
Mutable Composer → Build → Immutable Data Structure
```

**Klassen:**
- `XGaMultivectorComposer<T>`: Erstellt Multivektoren
- `XGaKVectorComposer<T>`: Erstellt k-Vektoren
- `XGaScalarComposer<T>`: Erstellt Skalare

**Vorteile:**
- ✓ Keine unerwarteten Seiteneffekte
- ✓ Thread-sicher
- ✓ Einfacher zu verstehen
- ✓ Optimierungsmöglichkeiten durch Compiler

---

### DOP-Prinzip 4: Trennung von Datenrepräsentation und Schema

**Regel:** Datenschema getrennt von tatsächlichen Daten speichern

**Implementierung:**

**Durch Interfaces und abstrakte Klassen:**

```csharp
// Schema: Interface definiert die Form
interface IIndexSet {
    int Count { get; }
    bool Contains(int index);
    IEnumerator<int> GetEnumerator();
}

// Repräsentation: Verschiedene Implementierungen
class EmptyIndexSet : IIndexSet { }
class SingleIndexSet : IIndexSet { }
class UInt64IndexSet : IIndexSet { }
class DenseIndexSet : IIndexSet { }
class SparseIndexSet : IIndexSet { }
```

**Verwendung:**
```csharp
// Code verwendet nur das Schema (Interface)
void ProcessBlade(IIndexSet indexSet) {
    // Funktioniert mit allen Implementierungen
    foreach (var index in indexSet) {
        // ...
    }
}

// Automatische Auswahl der besten Implementierung
var indexSet = IndexSetFactory.Create(indices);
// Gibt EmptyIndexSet, SingleIndexSet, UInt64IndexSet, etc. zurück
```

**Weitere Beispiele:**

**Multivektoren:**
```csharp
// Schema
IReadOnlyDictionary<IIndexSet, T>

// Implementierungen
class EmptyDictionary<K, V> : IReadOnlyDictionary<K, V>
class SingleItemDictionary<K, V> : IReadOnlyDictionary<K, V>
class Dictionary<K, V> : IReadOnlyDictionary<K, V>
```

**Vorteile:**
- ✓ Flexibilität bei der Implementierung
- ✓ Optimierung zur Laufzeit
- ✓ Einfaches Austauschen von Implementierungen
- ✓ Interface-basierte Tests

---

## Implementierung der DOP-Prinzipien

### Beispiel: Index-Sets

**Vollständige Implementierung der vier DOP-Prinzipien:**

**DOP-4 (Schema):**
```csharp
// Interface als Schema
interface IIndexSet {
    int Count { get; }
    bool Contains(int index);
    IEnumerable<int> GetIndices();
}
```

**DOP-2 (Generische Strukturen) + DOP-3 (Unveränderlich):**
```csharp
// Implementierung mit read-only internen Strukturen
class UInt64IndexSet : IIndexSet {
    private readonly ulong _bitmap;  // Immutable

    public UInt64IndexSet(ulong bitmap) {
        _bitmap = bitmap;  // Set once, never changed
    }
}

class SparseIndexSet : IIndexSet {
    private readonly ImmutableHashSet<int> _indices;  // Immutable

    public SparseIndexSet(IEnumerable<int> indices) {
        _indices = indices.ToImmutableHashSet();
    }
}
```

**DOP-1 (Trennung von Verhalten):**
```csharp
// Thin wrapper
class XGaBasisBlade {
    public IIndexSet IndexSet { get; }  // Just holds data

    public XGaBasisBlade(IIndexSet indexSet) {
        IndexSet = indexSet;
    }
}

// Verhalten in Extension Methods
static class XGaBasisBladeExtensions {
    public static XGaBasisBlade Op(
        this XGaBasisBlade blade1,
        XGaBasisBlade blade2,
        XGaMetric metric
    ) {
        // Outer product logic here
    }
}
```

---

### Beispiel: Multivektoren

**Vollständige DOP-Implementierung:**

**DOP-4 + DOP-2:**
```csharp
// Schema: Generic Dictionary
IReadOnlyDictionary<IIndexSet, T>

// Thin Wrapper mit Schema
class XGaKVector<T> {
    public IReadOnlyDictionary<IIndexSet, T> ScalarDictionary { get; }
    public XGaProcessor<T> Processor { get; }
}

class XGaMultivector<T> {
    public IReadOnlyDictionary<int, XGaKVector<T>> KVectorDictionary { get; }
    public XGaProcessor<T> Processor { get; }
}
```

**DOP-3 (Immutability via Composer):**
```csharp
// Mutable Builder
class XGaMultivectorComposer<T> {
    private Dictionary<IIndexSet, T> _terms = new();

    public XGaMultivectorComposer<T> SetTerm(IIndexSet id, T scalar) {
        _terms[id] = scalar;
        return this;
    }

    public XGaMultivector<T> GetMultivector() {
        // Select most efficient implementation
        if (_terms.Count == 0)
            return new ZeroMultivector<T>();
        if (_terms.Count == 1)
            return new SingleTermMultivector<T>(_terms.First());
        return new SparseMultivector<T>(_terms.ToImmutableDictionary());
    }
}
```

**DOP-1 (Verhalten getrennt):**
```csharp
// Extension Methods für Operationen
static class XGaMultivectorExtensions {
    public static XGaMultivector<T> Gp<T>(
        this XGaMultivector<T> mv1,
        XGaMultivector<T> mv2
    ) {
        var composer = mv1.Processor.CreateMultivectorComposer();

        // Geometric product logic
        foreach (var (id1, scalar1) in mv1.Terms)
        foreach (var (id2, scalar2) in mv2.Terms) {
            var product = id1.Gp(id2, mv1.Processor.Metric);
            composer.AddTerm(product.IndexSet,
                mv1.Processor.ScalarProcessor.Times(
                    scalar1,
                    scalar2,
                    product.Sign
                )
            );
        }

        return composer.GetMultivector();
    }
}
```

---

## Praktische Beispiele

### Beispiel 1: Neuen Skalartyp hinzufügen

Dank DOP-Prinzipien ist dies einfach:

```csharp
// 1. Implementiere IScalarProcessor<T> (DOP-1: Verhalten)
public class MyCustomScalarProcessor : INumericScalarProcessor<MyScalar> {
    public MyScalar Add(MyScalar a, MyScalar b) => a + b;
    public MyScalar Times(MyScalar a, MyScalar b) => a * b;
    // ... weitere Operationen
}

// 2. Verwende es mit GA-FuL (DOP-4: Schema-Kompatibilität)
var scalarProcessor = new MyCustomScalarProcessor();
var processor = XGaProcessor<MyScalar>.CreateEuclidean(scalarProcessor);

// 3. Alle GA-Operationen funktionieren automatisch!
var v1 = processor.CreateComposer()
    .SetVectorTerm(0, a)
    .SetVectorTerm(1, b)
    .SetVectorTerm(2, c)
    .GetVector();
var v2 = processor.CreateComposer()
    .SetVectorTerm(0, x)
    .SetVectorTerm(1, y)
    .SetVectorTerm(2, z)
    .GetVector();
var result = v1.Gp(v2);
```

---

### Beispiel 2: Neue Index-Set-Implementierung

```csharp
// 1. Implementiere IIndexSet (DOP-4: Schema)
public class BitVectorIndexSet : IIndexSet {
    private readonly BitVector32 _bits;  // DOP-3: Immutable

    public int Count => _bits.Data.CountBits();
    public bool Contains(int index) => _bits[1 << index];
    // ...
}

// 2. Verwende es (DOP-2: Generic Structure)
IIndexSet indexSet = new BitVectorIndexSet(bits);
var blade = new XGaBasisBlade(indexSet);

// 3. Alle Operationen funktionieren (DOP-1: Verhalten getrennt)
var result = blade.Op(otherBlade, metric);
```

---

### Beispiel 3: Neue geometrische Operation

```csharp
// DOP-1: Verhalten als Extension Method hinzufügen
public static class MyCustomOperations {
    public static XGaMultivector<T> MyCustomProduct<T>(
        this XGaMultivector<T> mv1,
        XGaMultivector<T> mv2
    ) {
        // DOP-2: Arbeite mit generischen Datenstrukturen
        var composer = mv1.Processor.CreateMultivectorComposer();

        foreach (var (id1, s1) in mv1.Terms)
        foreach (var (id2, s2) in mv2.Terms) {
            // Nutze bestehende Strukturen
            var id = ComputeResultingId(id1, id2);
            var scalar = ComputeResultingScalar(s1, s2);
            composer.AddTerm(id, scalar);
        }

        // DOP-3: Returne immutable Objekt
        return composer.GetMultivector();
    }
}

// Verwendung
var result = multivector1.MyCustomProduct(multivector2);
```

---

## Zusammenfassung

### Kern-Design-Intentionen

| CDI | Ziel | Nutzen |
|-----|------|--------|
| **CDI-1** | Skalar-Abstraktion | Flexibilität bei Skalartypen |
| **CDI-2** | Sparse-Speicherung | Hochdimensionale GAs praktikabel |
| **CDI-3** | Metaprogrammierung | Optimierte Code-Generierung |
| **CDI-4** | Geschichtetes Design | Verschiedene Abstraktionsebenen |
| **CDI-5** | Unified API | Konsistente, erweiterbare Schnittstelle |

### DOP-Prinzipien

| DOP | Prinzip | Implementierung in GA-FuL |
|-----|---------|--------------------------|
| **DOP-1** | Trennung von Verhalten und Daten | Extension Methods + Thin Wrappers |
| **DOP-2** | Generische Datenstrukturen | Dictionary, Array, HashSet |
| **DOP-3** | Unveränderliche Daten | Composer-Pattern für Konstruktion |
| **DOP-4** | Schema-Trennung | Interfaces + Multiple Implementierungen |

### Vorteile der Kombination

✓ **Lesbar:** Klare Trennung, einfache Strukturen
✓ **Wartbar:** Geringe Kopplung, hohe Kohäsion
✓ **Erweiterbar:** Neue Features leicht hinzuzufügen
✓ **Performant:** Optimierungsmöglichkeiten auf allen Ebenen
✓ **Flexibel:** Unterstützt verschiedene Anwendungsfälle
✓ **Testbar:** Jede Komponente isoliert testbar

---

[← Zurück zur Hauptdokumentation](README.en.md)
