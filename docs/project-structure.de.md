---
layout: default
title: "Projektstruktur"
lang: de
---

# GA-FuL Projektstruktur

## Inhaltsverzeichnis

1. [Übersicht](#übersicht)
2. [Hauptmodule](#hauptmodule)
3. [Verzeichnisstruktur](#verzeichnisstruktur)
4. [Abhängigkeiten](#abhängigkeiten)
5. [Build-Konfiguration](#build-konfiguration)

---

## Übersicht

Das GA-FuL-Projekt ist in mehrere Module unterteilt, die jeweils spezifische Funktionalitäten bereitstellen. Die Struktur folgt dem [geschichteten Design](architecture.en.md) der Bibliothek.

```
GeometricAlgebraFulcrumLib/
├── GeometricAlgebraFulcrumLib/          # Hauptordner mit allen Modulen
│   ├── GeometricAlgebraFulcrumLib.Algebra/
│   ├── GeometricAlgebraFulcrumLib.Modeling/
│   ├── GeometricAlgebraFulcrumLib.MetaProgramming/
│   ├── GeometricAlgebraFulcrumLib.Utilities.*/
│   ├── GeometricAlgebraFulcrumLib.Applications/
│   ├── GeometricAlgebraFulcrumLib.Mathematica/
│   ├── GeometricAlgebraFulcrumLib.Matlab/
│   └── ...
├── GeometricAlgebraFulcrumLib.Documentation/
├── GeometricAlgebraFulcrumLib.Visualizations/
├── docs/                                 # Diese Dokumentation
├── assets/
├── README.adoc
└── LICENSE
```

---

## Hauptmodule

### Kern-Module (Algebra-Schicht)

#### 1. **GeometricAlgebraFulcrumLib.Algebra**

**Zweck:** Kern-Algebra-Implementierung

**Hauptkomponenten:**
- `GeometricAlgebra/`: GA-Operationen und Multivektoren
  - `Basis/`: Basis-Blades und Index-Sets
  - `Dense/`: Optimierte Dense-Implementierung
  - `Extended/`: Erweiterte GA (XGa)
  - `Restricted/`: Eingeschränkte GA (RGa)
- `ComplexAlgebra/`: Komplexe Zahlen
- `LinearAlgebra/`: Vektoren, Matrizen, Tensoren
- `Scalars/`: Skalar-Prozessoren
- `GeometricFrequency/`: Spektralanalyse

**Abhängigkeiten:**
- MathNet.Numerics
- PeterO.Numbers
- AngouriMath
- NumpyDotNet

**Wichtige Klassen:**
```
GeometricAlgebra/
├── XGaProcessor<T>              # Haupt-GA-Prozessor
├── XGaMultivector<T>            # Multivektor
├── XGaKVector<T>                # k-Vektor
├── XGaBasisBlade               # Basis-Blade
├── XGaMetric                   # Metrik-Definition
└── Composers/
    └── XGaMultivectorComposer<T>  # Multivektor-Builder
```

---

#### 2. **GeometricAlgebraFulcrumLib.Utilities.Structures**

**Zweck:** Grundlegende Datenstrukturen

**Hauptkomponenten:**
- `IndexSets/`: Implementierungen von IIndexSet
- `Dictionaries/`: Optimierte Dictionary-Implementierungen
- `Collections/`: Spezialisierte Collections
- `BitManipulation/`: Bit-Operationen für effiziente Index-Sets

**Wichtige Klassen:**
```
IndexSets/
├── IIndexSet                    # Interface
├── EmptyIndexSet               # Leere Menge
├── SingleIndexSet              # Ein Element
├── UInt64IndexSet              # Für Dimension < 64
├── DenseIndexSet               # Dense-Array
└── SparseIndexSet              # Sparse-HashSet
```

---

#### 3. **GeometricAlgebraFulcrumLib.Utilities.Text**

**Zweck:** Text- und LaTeX-Generierung

**Hauptkomponenten:**
- `Text/`: Formatierte Text-Generierung
- `LaTeX/`: LaTeX-Code-Composition
- `Parametric/`: Template-basierte Text-Generierung
- `Files/`: Datei- und Ordner-Hierarchie-Erstellung

---

#### 4. **GeometricAlgebraFulcrumLib.Utilities.Code**

**Zweck:** Code-Generierungs-Utilities

**Hauptkomponenten:**
- `SyntaxTree/`: Abstract Syntax Trees (AST)
- `Languages/`: Sprach-spezifische Code-Generatoren
- `Expressions/`: Expression-Repräsentationen

---

#### 5. **GeometricAlgebraFulcrumLib.Utilities.Web**

**Zweck:** Web-Grafik-Utilities

**Hauptkomponenten:**
- `Svg/`: SVG-Generierung
- `Html/`: HTML-Generierung
- `JavaScript/`: JavaScript-Code-Generierung
- `BabylonJs/`: Babylon.js-Integration

---

### Modellierungs-Module

#### 6. **GeometricAlgebraFulcrumLib.Modeling**

**Zweck:** Geometrische Modellierung

**Hauptkomponenten:**
- `Geometry/`: Geometrische Objekte
  - `CGa/`: Conformal Geometric Algebra
  - `PGa/`: Projective Geometric Algebra
  - `Euclidean/`: Euklidische Geometrie
- `Graphics/`: Grafik-Primitiven
- `Calculus/`: Geometrischer Kalkül (in Entwicklung)

**Wichtige Klassen:**
```
Geometry/
└── CGa/
    ├── CGaGeometricSpace5D<T>      # 5D-CGA für 3D-Geometrie
    ├── CGaGeometricSpace4D<T>      # 4D-CGA für 2D-Geometrie
    └── Encoding/
        ├── EncodeIpnsRound         # IPNS Round-Objects
        ├── EncodeOpnsFlat          # OPNS Flat-Objects
        └── Decode                  # Dekodierung
```

---

### Metaprogrammierungs-Module

#### 7. **GeometricAlgebraFulcrumLib.MetaProgramming**

**Zweck:** Code-Generierung und Optimierung

**Hauptkomponenten:**
- `Context/`: Meta-Kontext
- `Expressions/`: Meta-Expressions
- `Composers/`: Code-Composer
- `Optimizers/`: Code-Optimierer
- `Applications/`: Anwendungsbeispiele

**Wichtige Klassen:**
```
MetaProgramming/
├── MetaContext                  # Haupt-Kontext
├── IMetaExpression              # Expression-Interface
├── MetaExpressionComposer       # Expression-Builder
├── CodeComposer                 # Code-Generator
└── Optimizers/
    ├── ConstantPropagation
    ├── CommonSubexpressionElimination
    └── GeneticProgramming
```

---

### Integrations-Module

#### 8. **GeometricAlgebraFulcrumLib.Mathematica**

**Zweck:** Wolfram Mathematica Integration

**Hauptkomponenten:**
- `Processors/`: Mathematica-Skalar-Prozessor
- `Expressions/`: Mathematica-Expression-Wrapper
- `Utilities/`: Hilfsfunktionen

**Abhängigkeiten:**
- Wolfram.NETLink

---

#### 9. **GeometricAlgebraFulcrumLib.Matlab**

**Zweck:** MATLAB Integration und Toolbox

**Hauptkomponenten:**
- `GA-FuL MATLAB Toolbox/`: Vollständige MATLAB-Toolbox
- `Processors/`: MATLAB-Skalar-Prozessor
- `CodeGeneration/`: MATLAB-Code-Generierung

**Features:**
- MATLAB-Funktions-Generierung
- GA-Operationen in MATLAB
- Visualisierung

---

### Anwendungs-Module

#### 10. **GeometricAlgebraFulcrumLib.Applications**

**Zweck:** Allgemeine Anwendungsbeispiele

**Hauptkomponenten:**
- Verschiedene Anwendungsbeispiele für GA-FuL
- Demos und Tutorials

---

#### 11. **GeometricAlgebraFulcrumLib.Applications.Symbolic**

**Zweck:** Symbolische Anwendungen

**Hauptkomponenten:**
- Symbolische GA-Berechnungen
- Mathematica-basierte Anwendungen

---

#### 12. **GeometricAlgebraFulcrumLib.Samples.Generations**

**Zweck:** Code-Generierungs-Beispiele

**Hauptkomponenten:**
- Code-Generierungs-Samples
- Template-Beispiele

---

### Zusätzliche Module

#### 13. **GeometricAlgebraFulcrumLib.MonoGame**

**Zweck:** MonoGame-Integration für Spiele und Grafik

---

#### 14. **GeometricAlgebraFulcrumLib.Stride**

**Zweck:** Stride Game Engine Integration

---

#### 15. **GeometricAlgebraFulcrumLib.Optimization**

**Zweck:** Optimierungsalgorithmen

---

#### 16. **GeometricAlgebraFulcrumLib.Benchmarks**

**Zweck:** Performance-Benchmarks

**Tools:**
- BenchmarkDotNet
- Performance-Tests

---

#### 17. **GeometricAlgebraFulcrumLib.UnitTests**

**Zweck:** Unit-Tests

**Framework:**
- xUnit oder NUnit

---

## Verzeichnisstruktur

### Typische Modul-Struktur

```
GeometricAlgebraFulcrumLib.ModuleName/
├── GeometricAlgebraFulcrumLib.ModuleName.csproj
├── Namespace1/
│   ├── Class1.cs
│   ├── Class2.cs
│   └── Subfolder/
│       └── Class3.cs
├── Namespace2/
│   └── ...
└── Properties/
    └── AssemblyInfo.cs (optional)
```

### Beispiel: Algebra-Modul

```
GeometricAlgebraFulcrumLib.Algebra/
├── GeometricAlgebraFulcrumLib.Algebra.csproj
├── Scalars/
│   ├── IScalarProcessor.cs
│   ├── ScalarProcessorOfFloat64.cs
│   ├── ScalarProcessorOfFloat32.cs
│   └── ...
├── GeometricAlgebra/
│   ├── Basis/
│   │   ├── IIndexSet.cs
│   │   ├── XGaBasisBlade.cs
│   │   └── ...
│   ├── Extended/
│   │   ├── XGaProcessor.cs
│   │   ├── XGaMultivector.cs
│   │   ├── XGaKVector.cs
│   │   └── Composers/
│   │       └── XGaMultivectorComposer.cs
│   └── Restricted/
│       ├── RGaProcessor.cs
│       └── ...
├── LinearAlgebra/
│   ├── Vectors/
│   ├── Matrices/
│   └── Tensors/
└── ComplexAlgebra/
    └── ...
```

---

## Abhängigkeiten

### Abhängigkeits-Graph

```
┌─────────────────────────────────────┐
│   Applications, Samples, Tests      │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│  MetaProgramming, Mathematica       │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│         Modeling                    │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│         Algebra                     │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│      Utilities (Text, Code,         │
│      Structures, Web)               │
└─────────────────────────────────────┘
```

### NuGet-Pakete

**Algebra-Modul:**
```xml
<PackageReference Include="AngouriMath" Version="1.3.0" />
<PackageReference Include="MathNet.Numerics" Version="5.0.0" />
<PackageReference Include="PeterO.Numbers" Version="1.8.2" />
<PackageReference Include="NumpyDotNet" Version="0.9.87.2" />
<PackageReference Include="OxyPlot.Core" Version="2.2.0" />
<PackageReference Include="SixLabors.ImageSharp" Version="3.1.10" />
<PackageReference Include="EPPlus" Version="8.0.7" />
<PackageReference Include="Dew.Math" Version="6.2.3" />
```

**Mathematica-Modul:**
```xml
<Reference Include="Wolfram.NETLink" />
```

---

## Build-Konfiguration

### .NET-Konfiguration

Alle Projekte verwenden:
```xml
<PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

### Build-Befehle

**Gesamte Solution bauen:**
```bash
cd GeometricAlgebraFulcrumLib
dotnet build GeometricAlgebraFulcrumLib.sln
```

**Spezifisches Projekt bauen:**
```bash
cd GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Algebra
dotnet build
```

**Tests ausführen:**
```bash
cd GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests
dotnet test
```

**Release-Build:**
```bash
dotnet build -c Release
```

---

## Namenskonventionen

### Assembly-Namen

Format: `GeometricAlgebraFulcrumLib.<ModuleName>`

Beispiele:
- `GeometricAlgebraFulcrumLib.Algebra`
- `GeometricAlgebraFulcrumLib.Modeling`
- `GeometricAlgebraFulcrumLib.MetaProgramming`

### Namespace-Konventionen

Format: `GeometricAlgebraFulcrumLib.<ModuleName>.<SubNamespace>`

Beispiele:
- `GeometricAlgebraFulcrumLib.Algebra.Scalars`
- `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Basis`
- `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa`

### Klassen-Präfixe

| Präfix | Bedeutung | Beispiel |
|--------|-----------|----------|
| `XGa` | Extended GA | `XGaProcessor<T>` |
| `RGa` | Restricted GA | `RGaProcessor<T>` |
| `CGa` | Conformal GA | `CGaGeometricSpace5D<T>` |
| `PGa` | Projective GA | `PGaGeometricSpace<T>` |
| `Float64` | 64-bit float optimiert | `RGaFloat64Multivector` |

---

## Zusammenfassung

Die Projektstruktur von GA-FuL:

✓ **Modular**: Klare Trennung der Verantwortlichkeiten
✓ **Geschichtet**: Von Utilities bis Applications
✓ **Erweiterbar**: Einfaches Hinzufügen neuer Module
✓ **Gut organisiert**: Konsistente Namenskonventionen
✓ **Dokumentiert**: Klare Struktur und Abhängigkeiten

---

[← Zurück zur Hauptdokumentation](README.en.md)
