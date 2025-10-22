# GA-FUL Architektur-Spezifikation
## Scalar Abstraction System - Detaillierte Architektur

**Teil von:** [SCALAR_ABSTRACTION_DESIGN.md](./SCALAR_ABSTRACTION_DESIGN.md)
**Version:** 3.0
**Datum:** 2025-01-22

---

## Inhaltsverzeichnis

1. [Layer-Architektur Übersicht](#layer-architektur-übersicht)
2. [Algebra Layer Analyse](#algebra-layer-analyse)
3. [Modeling Layer Analyse](#modeling-layer-analyse)
4. [Utilities Layer Analyse](#utilities-layer-analyse)
5. [Kritische Architektur-Befunde](#kritische-architektur-befunde)
6. [Komponenten-Diagramme](#komponenten-diagramme)

---

## Layer-Architektur Übersicht

### Bestehende 4-Layer Architektur

```
┌─────────────────────────────────────────┐
│     Applications Layer                  │
│  (Robotics, Geometry Apps, etc.)        │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│     Modeling Layer                      │
│  (CGa, PGa, VGa, HGa, Euclidean)       │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│     Algebra Layer                       │
│  (XGa, Scalars, GeometricAlgebra)      │
└─────────────────┬───────────────────────┘
                  │
     ┌────────────┼────────────┐
     │                         │
┌────▼──────────┐    ┌────────▼──────────┐
│ MetaProgramming│    │   Utilities       │
│  (Symbolic,    │    │ (IndexSets, etc.) │
│   CodeGen)     │    │                   │
└────────────────┘    └───────────────────┘
```

### Scalar Abstraction Dependencies

```
IScalarProcessor<T> (Interface)
        ▲
        │ implements
        ├─────────────────┬──────────────┬─────────────────┐
        │                 │              │                 │
ScalarProcessorOfFloat64  MetaContext   ScalarProcessorOfFloating<T>
    (bestehend)        (bestehend)           (NEU!)
        │                 │              │
        │                 │              ├── float
        │                 │              ├── double
        │                 │              └── Half
        │                 │
        └─────────┬───────┴──────────────┘
                  │
                  ▼
         XGaProcessor<T>
                  │
                  ▼
         CGaGeometricSpace<T>
```

---

## Algebra Layer Analyse

### ✅ IScalarProcessor<T> - PERFEKT

**Datei:** `GeometricAlgebraFulcrumLib.Algebra/Scalars/Generic/IScalarProcessor.cs`

**Status:** Architektonisch sauber, benötigt nur eine Erweiterung

**Bestehende API:**
```csharp
public interface IScalarProcessor<T>
{
    // Dual API Pattern - sowohl Scalar<T> als auch raw T
    Scalar<T> Zero { get; }
    Scalar<T> One { get; }
    T ZeroValue { get; }
    T OneValue { get; }

    // Operations return Scalar<T>, accept raw T
    Scalar<T> Add(T a, T b);
    Scalar<T> Subtract(T a, T b);
    Scalar<T> Times(T a, T b);
    Scalar<T> Divide(T a, T b);
    // ... weitere Operationen
}
```

**Benötigte Erweiterung:**
```csharp
public interface IScalarProcessor<T>
{
    // NEU: Wrapping-Methode für Konsistenz
    Scalar<T> Scalar(T value);

    // Optional: Convenience für häufige Use-Cases
    Scalar<T> ScalarFromNumber(int number);
    Scalar<T> ScalarFromNumber(float number);
    Scalar<T> ScalarFromNumber(double number);
}
```

**Rationale:** Einheitliches Pattern zum Wrappen von raw T in Scalar<T>.

### ✅ Scalar<T> - PERFEKT

**Datei:** `GeometricAlgebraFulcrumLib.Algebra/Scalars/Generic/Scalar.cs`

**Status:** Hat bereits ALLE benötigten Operatoren!

**Bestehende Operatoren (Lines 32-450):**
```csharp
public readonly struct Scalar<T> : IScalar<T>
{
    public IScalarProcessor<T> ScalarProcessor { get; }
    public T ScalarValue { get; }

    // Arithmetische Operatoren
    public static Scalar<T> operator +(Scalar<T> s1, Scalar<T> s2);
    public static Scalar<T> operator -(Scalar<T> s1, Scalar<T> s2);
    public static Scalar<T> operator *(Scalar<T> s1, Scalar<T> s2);
    public static Scalar<T> operator /(Scalar<T> s1, Scalar<T> s2);

    // Overloads für primitive Typen
    public static Scalar<T> operator +(Scalar<T> s, int n);
    public static Scalar<T> operator +(Scalar<T> s, float n);
    public static Scalar<T> operator +(Scalar<T> s, double n);
    public static Scalar<T> operator +(Scalar<T> s, T value);
    // ... symmetrisch für -, *, /

    // Vergleichs-Operatoren
    public static bool operator ==(Scalar<T> s1, Scalar<T> s2);
    public static bool operator !=(Scalar<T> s1, Scalar<T> s2);
    // ...
}
```

**Kritische Erkenntnis:** Keine Änderungen nötig! Operatoren bereits komplett implementiert.

### ✅ XGaProcessor<T> und XGaMultivector<T> - PERFEKT

**Datei:** `GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/Generic/`

**Status:** Bereits vollständige Hybrid-API implementiert!

**XGaMultivector Hybrid Pattern:**
```csharp
public abstract class XGaMultivector<T>
{
    // Hybrid API: Akzeptiert T, Scalar<T>, UND IScalar<T>
    public abstract XGaMultivector<T> Times(T scalarValue);
    public abstract XGaMultivector<T> Times(Scalar<T> scalarValue);
    public abstract XGaMultivector<T> Times(IScalar<T> scalarValue);

    public abstract XGaMultivector<T> Divide(T scalarValue);
    public abstract XGaMultivector<T> Divide(Scalar<T> scalarValue);
    public abstract XGaMultivector<T> Divide(IScalar<T> scalarValue);

    // Interne Speicherung: Dictionary<IndexSet, T> - raw T!
    // Beispiel: XGaUniformMultivector<T>
    protected Dictionary<IndexSet, T> _idScalarDictionary;
}
```

**Kritische Erkenntnis:** Phase 2 der ursprünglichen Roadmap ENTFÄLLT! XGa ist bereits perfekt.

### ✅ ScalarProcessorOfFloat64 - REFERENZ

**Datei:** `GeometricAlgebraFulcrumLib.Algebra/Scalars/Float64/ScalarProcessorOfFloat64.cs`

**Status:** Bestehende Implementation als Referenz

**Pattern:**
```csharp
public sealed class ScalarProcessorOfFloat64 : IScalarProcessor<double>
{
    public static ScalarProcessorOfFloat64 Instance { get; } = new();

    public double ZeroValue => 0.0d;
    public double OneValue => 1.0d;
    public Scalar<double> Zero => new Scalar<double>(this, 0.0d);
    public Scalar<double> One => new Scalar<double>(this, 1.0d);

    public Scalar<double> Add(double a, double b)
        => new Scalar<double>(this, a + b);
    // ... weitere Operationen
}
```

**Zu implementieren:** ScalarProcessorOfFloating<T> als Generalisierung!

---

## Modeling Layer Analyse

### Geometrie-Systeme Status-Übersicht

| System | Generic Version | Float64 Version | IST-Status (Problem) | SOLL-Status (Lösung) | Priorität |
|--------|----------------|-----------------|----------------------|----------------------|-----------|
| **CGa** (Conformal) | ❌ Circle/Point NUR IScalar<T> (Phase 2 Ziel: Hybrid API) | ❌ **24,026 LOC separate Implementation** | **PROBLEM: ~19.6k LOC Duplikation** | **LÖSUNG: 11-14k LOC thin wrapper** | **P0** |
| **PGa** (Projective) | ✅ Hybrid API | ✅ Deprecated (commented out) | **ERFOLGSGESCHICHTE** (bereits thin wrapper) | - | - |
| **VGa** (Vector/Euclidean) | ❌ **FEHLT KOMPLETT** | ✅ Float64-only (~200 LOC) | **BLOCKIERT Float32 Workflows** | **Generic implementieren** | **P0** |
| **HGa** (Hyperbolic) | ✅ Generic-only (2 files) | ❌ Kein Float64 | OK (Nischen-Use-Case) | Optional: Float64 wrapper | P2 |
| **Euclidean** (E3D, E2D) | ✅ Komplett generisch | - | PERFEKT | - | - |

### ❌ CGa (Conformal Geometric Algebra) - HAUPTPROBLEM

**IST-Zustand (AKTUELLES PROBLEM):**

```
Modeling/Geometry/CGa/
├── Generic/      19,608 LOC (77 files) ← Separate Implementation
│   ├── Blades/ CGaBlade.cs (✅ Hat bereits Operatoren!)
│   ├── Encoding/ (❌ Circle/Point haben NUR IScalar<T> - Hybrid API ist Phase 2 Ziel)
│   │   ├── CGaIpnsRoundEncoder.cs
│   │   ├── CGaIpnsFlatEncoder.cs
│   │   ├── CGaOpnsRoundEncoder.cs
│   │   └── CGaOpnsFlatEncoder.cs
│   ├── Decoding/
│   └── CGaGeometricSpace.cs
│
└── Float64/      24,026 LOC (83 files) ← ❌ EIGENSTÄNDIGE IMPLEMENTATION!
    ├── Blades/ CGaFloat64Blade.cs
    ├── Encoding/
    │   ├── CGaFloat64IpnsRoundEncoder.cs (❌ Komplett separate Logik!)
    │   └── ... (ALLES DUPLIZIERT!)
    └── CGaFloat64GeometricSpace.cs

PROBLEM: ~19,608 LOC CODE-DUPLIKATION (~100% von Generic)
```

**Problem-Analyse (IST):**

1. **❌ Massive Code-Duplikation:** Generic und Float64 haben separate, duplizierte Implementations
2. **❌ Hybrid API fehlt** für Circle/Point - nur IScalar<T> vorhanden (Phase 2 Ziel)
3. **❌ Float64 wird aktiv genutzt** in Production (Refactoring blockiert ohne Tests)
4. **❌ Tests fehlen:** IST: 8 Tests, BENÖTIGT: 162 Baseline + 190 neue = 352 total

**SOLL-Zustand (LÖSUNG NACH PHASE 3):**

```
Modeling/Geometry/CGa/
├── Generic/      19,608 LOC (unverändert) ← CORE IMPLEMENTATION (alle Logik)
│   └── ... (vollständige Hybrid API für alle Encoder/Decoder)
│
└── Float64/      11,000-14,000 LOC ← THIN WRAPPER!
    ├── CGaFloat64GeometricSpace.cs (delegiert zu Generic<double>)
    ├── CGaFloat64Blade.cs (Wrapper)
    ├── CGaFloat64IpnsRoundEncoder.cs (delegiert zu Generic)
    └── ... (NUR Wrapping-Code)

LÖSUNG: ~10,000-13,000 LOC EINGESPART!
```

**CGa Generic Encoder (Current):**
```csharp
// Datei: CGa/Generic/Encoding/CGaIpnsRoundEncoder.cs
public class CGaIpnsRoundEncoder<T> : CGaEncoderBase<T>
{
    // Generic API using IScalar<T>
    public CGaBlade<T> Circle(IScalar<T> radiusSquared, IScalar<T> centerX, IScalar<T> centerY)
    {
        return HyperSphere(
            radiusSquared,
            LinVector2D<T>.Create(centerX, centerY).ToXGaVector(GeometricSpace.EuclideanProcessor)
        );
    }

    // Keine T, Scalar<T>, double, float Überladungen!
}
```

**CGa Float64 Encoder (Current - DUPLIZIERT!):**
```csharp
// Datei: CGa/Float64/Encoding/CGaFloat64IpnsRoundEncoder.cs
public class CGaFloat64IpnsRoundEncoder : CGaFloat64EncoderBase
{
    public CGaFloat64Blade Circle(double radiusSquared, double centerX, double centerY)
    {
        return HyperSphere(
            radiusSquared,
            LinFloat64Vector2D.Create(centerX, centerY).ToXGaFloat64Vector()
        );
    }

    // Komplett separate Implementation!
}
```

**Refactoring-Strategie:**

1. Generic erweitern mit T + Scalar<T> + double/float Überladungen
2. Float64 zu dünnem Wrapper über Generic<double> umbauen
3. Public API von Float64 100% beibehalten (Zero Breaking Changes!)

### ✅ PGa (Projective Geometric Algebra) - ERFOLGSGESCHICHTE

**Status:** Bereits erfolgreich zu Generic-only migriert!

**Verzeichnisstruktur:**
```
Modeling/Geometry/PGa/
├── Generic/  (✅ AKTIV)
│   ├── Blades/
│   │   └── PGaBlade.cs (✅ Hybrid Operatoren!)
│   ├── Encoding/
│   │   └── PGaEncodePGaElementUtils.cs (✅ Hybrid API!)
│   └── ...
└── Float64/  (✅ DEPRECATED - alles auskommentiert!)
    └── ... (Kommentiert als Referenz erhalten)
```

**PGa Generic Encoder (SUCCESS PATTERN):**
```csharp
// Datei: PGa/Generic/Encoding/PGaEncodePGaElementUtils.cs
public static class PGaEncodePGaElementUtils
{
    // Convenience Überladung (double)
    public static PGaBlade<T> EncodePGaPoint<T>(
        this PGaGeometricSpace<T> pgaGeometricSpace,
        double pointX, double pointY)
    {
        return pgaGeometricSpace.EncodePGaPoint(
            pgaGeometricSpace.EncodeVGaVectorAsXGaVector(pointX, pointY)
        );
    }

    // Generic Überladung (IScalar<T>)
    public static PGaBlade<T> EncodePGaPoint<T>(
        this PGaGeometricSpace<T> pgaGeometricSpace,
        IScalar<T> pointX, IScalar<T> pointY)
    {
        return pgaGeometricSpace.EncodePGaPoint(
            pgaGeometricSpace.EncodeVGaVectorAsXGaVector(pointX, pointY)
        );
    }

    // Structured Überladung (Scalar<T> in Tuple)
    public static PGaBlade<T> EncodePGaPoint<T>(
        this PGaGeometricSpace<T> pgaGeometricSpace,
        IPair<Scalar<T>> point)
    {
        return pgaGeometricSpace.EncodePGaPoint(
            pgaGeometricSpace.EncodeVGaVectorAsXGaVector(point)
        );
    }

    // LinVector Überladung
    public static PGaBlade<T> EncodePGaPoint<T>(
        this PGaGeometricSpace<T> pgaGeometricSpace,
        LinVector<T> point)
    {
        return pgaGeometricSpace.EncodePGaPoint(
            pgaGeometricSpace.EncodeVGaVectorAsXGaVector(point)
        );
    }

    // XGaVector Überladung (Core)
    public static PGaBlade<T> EncodePGaPoint<T>(
        this PGaGeometricSpace<T> pgaGeometricSpace,
        XGaVector<T> vgaPoint)
    {
        // Core implementation
    }
}
```

**PGaBlade Operatoren (Hybrid):**
```csharp
// Datei: PGa/Generic/Blades/PGaBlade.cs
public sealed record PGaBlade<T>
{
    // Raw T operator
    public static PGaBlade<T> operator *(T scalar, PGaBlade<T> blade)
    {
        return blade.Times(scalar);
    }

    // Scalar<T> operator
    public static PGaBlade<T> operator *(Scalar<T> scalar, PGaBlade<T> blade)
    {
        return blade.Times(scalar);
    }

    // IScalar<T> operator
    public static PGaBlade<T> operator *(IScalar<T> scalar, PGaBlade<T> blade)
    {
        return blade.Times(scalar);
    }

    // Convenience operators (int, float, double)
    public static PGaBlade<T> operator *(int scalar, PGaBlade<T> blade)
    {
        return blade.Times(blade.ScalarProcessor.ScalarFromNumber(scalar));
    }
    // ... etc.
}
```

**Lernen von PGa:**
1. Hybrid API Pattern funktioniert perfekt
2. Float64 kann vollständig deprecated werden (oder als Wrapper erhalten)
3. Convenience Überladungen (double/float) in Generic sind wichtig für Usability

### ✅ VGa (Vector Geometric Algebra) - OK

**Status:** Nur Float64, aber einfache Implementation

**Verzeichnisstruktur:**
```
Modeling/Geometry/VGa/
└── Float64/
    ├── RGaEuclideanGeometrySpace.cs
    ├── RGaEuclideanGeometrySpace2D.cs
    └── RGaEuclideanGeometrySpace3D.cs
```

**Implementation:**
```csharp
public abstract class XGaEuclideanGeometrySpace : GaFloat64GeometricSpace
{
    public XGaFloat64EuclideanProcessor EuclideanProcessor
        => XGaFloat64EuclideanProcessor.Instance;

    public XGaFloat64Vector E1 { get; }
    public XGaFloat64Vector E2 { get; }
    public XGaFloat64Bivector E12 { get; }
    // Simple wrapper um XGa
}
```

**Bewertung:** Nur ~200 Zeilen Code, 4 Dateien. Niedriger Nutzen für Generifizierung. Priority P2.

### ✅ HGa (Hyperbolic Geometric Algebra) - OK

**Status:** Nur Generic, minimale Implementation

**Verzeichnisstruktur:**
```
Modeling/Geometry/HGa/
└── Generic/
    ├── HGaGeometricSpace3D.cs
    └── HGaGeometricSpace4D.cs
```

**Implementation:**
```csharp
public sealed class HGaGeometricSpace4D<T>
{
    public IScalarProcessor<T> ScalarProcessor
        => GeometricProcessor.ScalarProcessor;

    public XGaEuclideanProcessor<T> GeometricProcessor { get; }

    public HGaGeometricSpace4D(IScalarProcessor<T> scalarProcessor)
    {
        GeometricProcessor = scalarProcessor.CreateEuclideanXGaProcessor();
    }
    // ...
}
```

**Bewertung:** Nur 2 Dateien, Nischen-Use-Case. Keine Änderungen nötig.

### ✅ Euclidean (E3D, E2D) - PERFEKT

**Verzeichnisstruktur:**
```
Modeling/Geometry/Euclidean/
├── Space2D/
│   └── Objects/
│       ├── E2DPoint.cs
│       └── E2DVector.cs
└── Space3D/
    └── Objects/
        ├── E3DPoint.cs
        └── E3DVector.cs
```

**Implementation:**
```csharp
// Datei: Euclidean/Space3D/Objects/E3DPoint.cs
public sealed record E3DPoint<T>
{
    public IScalarProcessor<T> ScalarProcessor { get; }
    public T X { get; }
    public T Y { get; }
    public T Z { get; }

    // Operatoren nutzen ScalarProcessor
    public static E3DPoint<T> operator +(E3DPoint<T> v1, E3DVector<T> v2)
    {
        var processor = v1.ScalarProcessor;
        return new E3DPoint<T>(
            processor.Add(v1.X, v2.X),
            processor.Add(v1.Y, v2.Y),
            processor.Add(v1.Z, v2.Z)
        );
    }
    // ...
}
```

**Bewertung:** Bereits vollständig generisch und sauber implementiert. Keine Änderungen nötig.

---

## Utilities Layer Analyse

### ✅ Utilities.Structures - PERFEKT

**Komponenten:**
- IndexSets: Generisch, keine Scalar-Abhängigkeiten ✅
- Combinations: Generisch ✅
- BitManipulation: Generisch ✅
- Collections, Dictionary, etc.: Generisch ✅

**Float64-spezifische Utilities (isoliert):**
```
Utilities.Structures/Dictionary/
├── Float64SparseArray.cs  (Standalone utility)
└── Float64SparseVector.cs (Standalone utility)
```

**Bewertung:** Nur 2 Float64-spezifische Dateien, beide standalone. Keine Abhängigkeiten in kritischen Utilities. Keine Änderungen nötig.

### ✅ Utilities.Code - PERFEKT

**Zweck:** Code-Manipulation, Text-Processing für MetaProgramming

**Bewertung:** Keine Scalar-Abhängigkeiten. Keine Änderungen nötig.

### ✅ Utilities.Text - PERFEKT

**Zweck:** Text-Processing, String-Manipulation

**Bewertung:** Keine Scalar-Abhängigkeiten. Keine Änderungen nötig.

---

## Kritische Architektur-Befunde

### 🎯 Zusammenfassung der Validierung

| Layer | Status | Probleme | Aktion |
|-------|--------|----------|--------|
| **Algebra** | ✅ PERFEKT | Keine | Minimale Erweiterung (IScalarProcessor.Scalar(T)) |
| **Modeling - CGa** | ❌ PROBLEM | Code-Duplikation, Unvollständige API | **3-Phasen Refactoring** |
| **Modeling - PGa** | ✅ ERFOLG | Keine (bereits migriert) | Referenzmuster nutzen |
| **Modeling - VGa** | ✅ OK | Nur Float64 | Optional generifizieren (P2) |
| **Modeling - HGa** | ✅ OK | Keine | Keine Änderungen |
| **Modeling - Euclidean** | ✅ PERFEKT | Keine | Keine Änderungen |
| **Utilities** | ✅ PERFEKT | Keine | Keine Änderungen |

### 🔑 Kernerkenntnisse

1. **XGa ist bereits perfekt** → Kein Refactoring nötig, nur Referenz nutzen
2. **PGa ist Erfolgsgeschichte** → Pattern für CGa Refactoring übernehmen
3. **Nur CGa braucht Arbeit** → Fokussiertes Refactoring auf ein System
4. **Utilities sind sauber** → Keine Blocker für Generic-Implementierungen
5. **Float64 wird aktiv genutzt** → Backward Compatibility ist kritisch

### 📊 Scope-Reduktion

**Ursprüngliche Annahme:** Mehrere Systeme brauchen Refactoring
**Realität:** Nur CGa braucht Refactoring

**Zeitersparnis:**
- Ursprüngliche Schätzung: 8-13 Wochen
- Revidierte Schätzung: **19-25 Wochen** (korrigiert nach Architekten-Review)

---

## Komponenten-Diagramme

### Scalar Processor Hierarchie

```
                    IScalarProcessor<T>
                            │
        ┌───────────────────┼───────────────────────┐
        │                   │                       │
ScalarProcessorOfFloat64    │            ScalarProcessorOfFloating<T>
    (bestehend)             │                   (NEU!)
                            │                       │
                    MetaContext               IFloatingPointIeee754<T>
                    (bestehend)                     │
                            │               ┌───────┼──────┐
                            │               │       │      │
                            │             float  double  Half
                            │
        ┌───────────────────┴───────────────────┐
        │                                       │
ScalarProcessorOfERational          ScalarProcessorOfEDecimal
    (bestehend)                         (bestehend)
```

### CGa Architektur (Vor vs. Nach Refactoring)

**VOR dem Refactoring (IST - PROBLEM):**
```
CGaGeometricSpace<T>                CGaFloat64GeometricSpace
    (Generic - 23k LOC)                  (Float64 - 28k LOC)
        │                                    │
        │                                    │
    Hybrid API (Circle/Point)            double API
    (teilweise)                              │
        │                                    │
        │                                    │
    Wenig genutzt                    Production-genutzt
        │                                    │
        └────────────┬───────────────────────┘
                     │
            Separate Implementationen
           (Code-Duplikation ~25,000 LOC!)
                     │
            ❌ KEINE Tests zum Validieren
            (IST: 8, BENÖTIGT: 162 Baseline)
```

**NACH dem Refactoring (SOLL - LÖSUNG):**
```
        CGaGeometricSpace<T>
             (Generic)
                  │
        T + Scalar<T> + IScalar<T>
       + double/float convenience
                  │
        Interne raw T Performance
                  │
        ┌─────────┴──────────┐
        │                    │
Float32 Workflow      Symbolic Workflow
        │                    │
    float, Half      IMetaExpressionAtomic
        │                    │
        │                    │
        └────────┬───────────┘
                 │
    CGaFloat64GeometricSpace
         (Thin Wrapper)
                 │
         Public API 100%
           kompatibel
                 │
        162 Tests + Apps
        (Keine Änderungen!)
```

---

## Nächste Schritte

1. **Review dieser Architektur-Spezifikation**
2. **Weiter zu:** [API_DESIGN_PATTERNS.md](./API_DESIGN_PATTERNS.md)
3. **Dann:** [IMPLEMENTATION_ROADMAP.md](./IMPLEMENTATION_ROADMAP.md)

---

[← Zurück zu SCALAR_ABSTRACTION_DESIGN.md](./SCALAR_ABSTRACTION_DESIGN.md)
