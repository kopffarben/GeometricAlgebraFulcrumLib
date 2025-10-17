---
layout: default
title: "Erste Schritte"
lang: de
---

# Getting Started mit GA-FuL

**[🇬🇧 English Version](getting-started.en.md)**

---

## Inhaltsverzeichnis

1. [Installation](#installation)
2. [Systemanforderungen](#systemanforderungen)
3. [Erstes Beispiel](#erstes-beispiel)
4. [Grundkonzepte](#grundkonzepte)
5. [Häufige Workflows](#häufige-workflows)
6. [Nächste Schritte](#nächste-schritte)

---

## Installation

### Voraussetzungen

- **.NET 8.0 SDK** oder höher
- **Visual Studio 2022** oder **JetBrains Rider** oder **VS Code** mit C# Extension
- Optional: **Wolfram Mathematica** (für symbolische Berechnungen)
- Optional: **MATLAB** (für MATLAB-Integration)

### Klonen des Repositories

```bash
git clone https://github.com/ga-explorer/GeometricAlgebraFulcrumLib.git
cd GeometricAlgebraFulcrumLib
```

### Build

```bash
cd GeometricAlgebraFulcrumLib
dotnet build GeometricAlgebraFulcrumLib.sln
```

### Als NuGet-Paket (wenn verfügbar)

```bash
dotnet add package GeometricAlgebraFulcrumLib.Algebra
dotnet add package GeometricAlgebraFulcrumLib.Modeling
dotnet add package GeometricAlgebraFulcrumLib.MetaProgramming
```

---

## Systemanforderungen

### Minimale Anforderungen

| Komponente | Anforderung |
|------------|-------------|
| **OS** | Windows 10/11, Linux, macOS |
| **.NET Version** | .NET 8.0+ |
| **RAM** | 4 GB (minimum), 8 GB (empfohlen) |
| **IDE** | Visual Studio 2022, Rider, VS Code |

### Abhängigkeiten

Die wichtigsten NuGet-Pakete sind bereits in den Projektdateien enthalten:

- `MathNet.Numerics`: Numerische Berechnungen
- `AngouriMath`: Symbolische Mathematik
- `PeterO.Numbers`: Arbitrary-precision Zahlen
- `OxyPlot`: Plotting und Visualisierung
- Weitere siehe `.csproj` Dateien

---

## Erstes Beispiel

### Einfache Vektor-Operationen

```csharp
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra;
using GeometricAlgebraFulcrumLib.Algebra.Scalars;

// 1. Skalar-Prozessor auswählen (64-bit floats)
var scalarProcessor = ScalarProcessorOfFloat64.Instance;

// 2. GA-Prozessor erstellen (3D Euclidean GA)
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// 3. Vektoren erstellen
var composer = processor.CreateComposer();
composer.SetVectorTerm(0, 1);
composer.SetVectorTerm(1, 0);
composer.SetVectorTerm(2, 0);
var v1 = composer.GetVector();  // x-Achse

composer = processor.CreateComposer();
composer.SetVectorTerm(0, 0);
composer.SetVectorTerm(1, 1);
composer.SetVectorTerm(2, 0);
var v2 = composer.GetVector();  // y-Achse

// 4. Geometrisches Produkt
var gp = v1.Gp(v2);  // = xy bivector

// 5. Äußeres Produkt
var op = v1.Op(v2);  // = xy bivector

// 6. Inneres Produkt
var ip = v1.Lcp(v2);  // = 0 (orthogonal)

// 7. Ausgabe
Console.WriteLine($"Geometric Product: {gp}");
Console.WriteLine($"Outer Product: {op}");
Console.WriteLine($"Inner Product: {ip}");
```

**Ausgabe:**
```
Geometric Product: '1'<1,2>
Outer Product: '1'<1,2>
Inner Product: 0
```

---

## Grundkonzepte

### 1. Skalar-Prozessoren

Ein **Skalar-Prozessor** definiert, wie Skalare (Zahlen) behandelt werden.

**Verfügbare Prozessoren:**

```csharp
// Float64 (Standard double)
var sp1 = ScalarProcessorOfFloat64.Instance;

// Float32 (Standard float)
var sp2 = ScalarProcessorOfFloat32.Instance;

// Arbitrary Precision Decimal
var sp3 = ScalarProcessorOfDecimal.Instance;

// Rational Numbers (Brüche)
var sp4 = ScalarProcessorOfRational.Instance;

// Complex Numbers
var sp5 = ScalarProcessorOfComplex.Instance;

// Symbolic (Mathematica)
var sp6 = ScalarProcessorOfMathematica.Instance;
```

**Beispiel:**
```csharp
// Mit rationalen Zahlen
var scalarProc = ScalarProcessorOfRational.Instance;
var processor = XGaProcessor<Rational>.CreateEuclidean(scalarProc);

var composer = processor.CreateComposer();
composer.SetVectorTerm(0, new Rational(1, 2));  // 1/2
composer.SetVectorTerm(1, new Rational(1, 3));  // 1/3
composer.SetVectorTerm(2, new Rational(1, 4));  // 1/4
var v = composer.GetVector();
```

---

### 2. GA-Prozessoren

Ein **GA-Prozessor** verwaltet Multivektoren und GA-Operationen.

**Typen (Float64):**

```csharp
// Euclidean (alle e_i^2 = +1) - Am häufigsten
var euclidean = XGaFloat64Processor.Euclidean;

// Conformal (beinhaltet 1 negative Signatur)
var conformal = XGaFloat64Processor.Conformal;

// Projective (beinhaltet 1 Null-Signatur)
var projective = XGaFloat64Processor.Projective;

// Benutzerdefinierte Metrik
var custom = XGaFloat64Processor.Create(
    negativeCount: 1,
    zeroCount: 0
);
```

**Typen (Generic für nicht-Float64):**

```csharp
// Generic GA-Prozessor (für Rational, Symbolic, etc.)
var processor = XGaProcessor<T>.CreateEuclidean(scalarProcessor);

// Benutzerdefinierte Metrik mit generischem Typ
var custom = XGaProcessor<T>.Create(
    scalarProcessor,
    negativeCount: 1,
    zeroCount: 0
);
```

**Gängige Metriken:**
```csharp
// 3D Euclidean: Alle positive Signaturen
var ga3d = XGaFloat64Processor.Euclidean;

// Spacetime (Minkowski): (3,1,0) - 3 positiv, 1 negativ
var spacetime = XGaFloat64Processor.Create(negativeCount: 1, zeroCount: 0);

// 5D Conformal: (4,1,0) - Für 3D-Geometrie
var cga = CGaFloat64GeometricSpace5D.Instance;  // Spezialisiertes CGA
```

---

### 3. Multivektoren

**Multivektoren** sind die grundlegenden Objekte in GA.

**Erstellen:**

```csharp
// Skalare (Grade 0)
var scalar = processor.CreateScalar(5.0);

// Vektoren (Grade 1)
var composer = processor.CreateComposer();
composer.SetVectorTerm(0, 1);
composer.SetVectorTerm(1, 2);
composer.SetVectorTerm(2, 3);
var vector = composer.GetVector();

// Bivektoren (Grade 2)
composer = processor.CreateComposer();
composer.SetBivectorTerm(0, 1, 1.0); // xy
composer.SetBivectorTerm(0, 2, 2.0); // xz
composer.SetBivectorTerm(1, 2, 3.0); // yz
var bivector = composer.GetBivector();

// Allgemeine Multivektoren
composer = processor.CreateComposer();
composer.SetTerm(0, 1.0);           // Skalar-Teil
composer.SetTerm(1, 2.0);           // e_1
composer.SetTerm(2, 3.0);           // e_2
composer.SetBivectorTerm(0, 1, 4.0); // e_1 ∧ e_2
var mv = composer.GetMultivector();
```

---

### 4. GA-Operationen

**Grundlegende Produkte:**

```csharp
var composer = processor.CreateComposer();
composer.SetVectorTerm(0, 1);
composer.SetVectorTerm(1, 0);
composer.SetVectorTerm(2, 0);
var v1 = composer.GetVector();

composer = processor.CreateComposer();
composer.SetVectorTerm(0, 0);
composer.SetVectorTerm(1, 1);
composer.SetVectorTerm(2, 0);
var v2 = composer.GetVector();

// Geometrisches Produkt
var gp = v1.Gp(v2);

// Äußeres Produkt (Outer/Wedge Product)
var op = v1.Op(v2);

// Inneres Produkt (Left Contraction)
var lcp = v1.Lcp(v2);

// Right Contraction
var rcp = v1.Rcp(v2);

// Skalarprodukt
var sp = v1.Sp(v2);

// Fettes Punkt-Produkt (Fat Dot)
var fdp = v1.Fdp(v2);
```

**Unäre Operationen:**

```csharp
// Multivektor erstellen (Beispiel)
var composer = processor.CreateComposer();
composer.SetTerm(0, 1.0);  // Skalar
composer.SetVectorTerm(0, 2.0);  // e_1
composer.SetBivectorTerm(0, 1, 3.0);  // e_1 ∧ e_2
var mv = composer.GetMultivector();

// Reverse (Umkehrung)
var rev = mv.Reverse();

// Grade Involution
var gi = mv.GradeInvolution();

// Clifford Conjugate
var cc = mv.CliffordConjugate();

// Dual
var dual = mv.Dual();

// Magnitude (Norm)
var norm = mv.Norm();
var normSquared = mv.NormSquared();

// Normalisierung
var normalized = mv.Normalize();
```

---

### 5. Basis-Blades

**Basis-Blades** sind die Basiselemente der GA.

```csharp
// Basis-Vektoren
var e1 = processor.CreateBasisVector(0);  // e_1
var e2 = processor.CreateBasisVector(1);  // e_2
var e3 = processor.CreateBasisVector(2);  // e_3

// Basis-Bivektoren
var e12 = processor.CreateBasisBivector(0, 1);  // e_1 ∧ e_2
var e23 = processor.CreateBasisBivector(1, 2);  // e_2 ∧ e_3

// Allgemeine Basis-Blades
var e123 = processor.CreateBasisBlade(0, 1, 2);  // e_1 ∧ e_2 ∧ e_3

// Produkte auf Basis-Blades
var product = e1.Gp(e2);  // e_1 * e_2
```

---

## Häufige Workflows

### Workflow 1: Numerische 3D-Geometrie

```csharp
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa;

// Setup
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var cga = CGaFloat64GeometricSpace5D.Instance;

// Punkte definieren
var p1 = cga.Encode.IpnsRound.Point(0, 0, 0);
var p2 = cga.Encode.IpnsRound.Point(1, 0, 0);
var p3 = cga.Encode.IpnsRound.Point(0, 1, 0);

// Ebene durch drei Punkte
var plane = p1.Op(p2).Op(p3);

// Kugel definieren
var sphere = cga.Encode.IpnsRound.RealSphere(
    radius: 2,
    centerX: 1,
    centerY: 1,
    centerZ: 1
);

// Schnitt von Ebene und Kugel (ergibt Kreis)
var circle = plane.Op(sphere);

// Dekodieren
var circleData = circle.Decode.OpnsRound.Element();
var center = circleData.CenterToVector3D();
var radius = circleData.RealRadius;

Console.WriteLine($"Kreis-Zentrum: {center}");
Console.WriteLine($"Kreis-Radius: {radius}");
```

---

### Workflow 2: Symbolische Berechnungen

```csharp
using GeometricAlgebraFulcrumLib.Mathematica;

// Mathematica Skalar-Prozessor
var scalarProcessor = ScalarProcessorOfMathematica.Instance;
var processor = XGaProcessor<Expr>.CreateEuclidean(scalarProcessor);

// Symbolische Parameter
var x = scalarProcessor.CreateSymbol("x");
var y = scalarProcessor.CreateSymbol("y");
var z = scalarProcessor.CreateSymbol("z");

// Vektor mit symbolischen Komponenten
var composer = processor.CreateComposer();
composer.SetVectorTerm(0, x);
composer.SetVectorTerm(1, y);
composer.SetVectorTerm(2, z);
var v = composer.GetVector();

// Berechnung
var normSquared = v.NormSquared();

// Ausgabe: normSquared ist symbolischer Ausdruck
// x^2 + y^2 + z^2
Console.WriteLine($"||v||^2 = {normSquared}");
```

---

### Workflow 3: Code-Generierung

```csharp
using GeometricAlgebraFulcrumLib.MetaProgramming;

// 1. Meta-Kontext erstellen
var context = new MetaContext();

// 2. Symbolische Parameter definieren
var x = context.CreateParameter("x");
var y = context.CreateParameter("y");
var z = context.CreateParameter("z");

// 3. GA-Berechnungen
var scalarProcessor = context.ScalarProcessor;
var processor = XGaProcessor<IMetaExpression>.CreateEuclidean(scalarProcessor);

var composer = processor.CreateComposer();
composer.SetVectorTerm(0, x);
composer.SetVectorTerm(1, y);
composer.SetVectorTerm(2, z);
var v1 = composer.GetVector();

composer = processor.CreateComposer();
composer.SetVectorTerm(0, 1);
composer.SetVectorTerm(1, 0);
composer.SetVectorTerm(2, 0);
var v2 = composer.GetVector();

var result = v1.Gp(v2);

// 4. Output definieren
context.SetOutputVariable("result", result);

// 5. Optimieren
context.OptimizeContext();

// 6. C#-Code generieren
var codeComposer = new CSharpCodeComposer();
var code = codeComposer.GenerateCode(context);

// 7. Ausgabe
Console.WriteLine(code);
```

**Generierter Code:**
```csharp
public static void ComputeGeometricProduct(
    double x, double y, double z,
    out double result_scalar,
    out double result_e1,
    out double result_e2,
    out double result_e3
) {
    result_scalar = x;
    result_e1 = 0;
    result_e2 = z;
    result_e3 = -y;
}
```

---

### Workflow 4: Rotationen

```csharp
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// Rotations-Bivector (Rotationsebene)
var composer = processor.CreateComposer();
composer.SetBivectorTerm(0, 1, 1.0); // xy
composer.SetBivectorTerm(0, 2, 0.0); // xz
composer.SetBivectorTerm(1, 2, 0.0); // yz
var B = composer.GetBivector();

// Winkel
var angle = Math.PI / 4;  // 45°

// Rotor - Erstellen mit Exponentialfunktion
var rotor = (-angle / 2 * B).Exp();

// Vektor zum Rotieren
composer = processor.CreateComposer();
composer.SetVectorTerm(0, 1);
composer.SetVectorTerm(1, 0);
composer.SetVectorTerm(2, 0);
var v = composer.GetVector();

// Rotation: v' = R v R^†
var rotated = rotor.Gp(v).Gp(rotor.Reverse());

Console.WriteLine($"Original: {v}");
Console.WriteLine($"Rotiert: {rotated}");
```

---

## Nächste Schritte

### Weitere Dokumentation

1. **[Architektur](architecture.en.md)**: Verstehen Sie das System-Design
2. **[Design-Prinzipien](design-principles.en.md)**: Lernen Sie die Designphilosophie
3. **[API-Referenz](api-reference.en.md)**: Detaillierte API-Dokumentation
4. **[Beispiele](examples.en.md)**: Umfangreiche Code-Beispiele

### Beispiel-Projekte

Schauen Sie sich die Beispiel-Projekte im Repository an:

```
GeometricAlgebraFulcrumLib/
├── GeometricAlgebraFulcrumLib.Applications/
├── GeometricAlgebraFulcrumLib.Applications.Symbolic/
├── GeometricAlgebraFulcrumLib.Samples.Generations/
└── GeometricAlgebraFulcrumLib.UnitTests/
```

### Tutorials

1. **3D-Geometrie mit CGA**
   - Punkt, Linie, Ebene, Kreis, Kugel
   - Transformationen
   - Schnitte und Projektionen

2. **Rotoren und Versoren**
   - Rotationen
   - Spiegelungen
   - Kombinierte Transformationen

3. **Code-Generierung**
   - Optimierung
   - Multi-Language-Support
   - Template-basierte Generierung

4. **Symbolische Mathematik**
   - Mathematica-Integration
   - Vereinfachungen
   - Ableitungen

### Community und Support

- **GitHub Issues**: Bug-Reports und Feature-Requests
- **Discussions**: Fragen und Diskussionen
- **Email**: ga.computing.eg@gmail.com

### Weiterführende Ressourcen

**Bücher über Geometrische Algebra:**
- *Geometric Algebra for Computer Science* - Dorst, Fontijne, Mann
- *Geometric Algebra for Physicists* - Doran, Lasenby
- *New Foundations for Classical Mechanics* - Hestenes

**Online-Ressourcen:**
- [bivector.net](https://bivector.net)
- [GA-FuL Paper auf MDPI](https://www.mdpi.com/2227-7390/12/14/2272)

---

## Häufige Fehler und Lösungen

### Fehler 1: Skalartyp-Mismatch

**Problem:**
```csharp
// Float64-Prozessor
var proc1 = XGaFloat64Processor.Euclidean;
var v1 = proc1.CreateComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 2)
    .SetVectorTerm(2, 3)
    .GetVector();

// FEHLER: Anderer Skalartyp (Float32)
var sp2 = ScalarProcessorOfFloat32.Instance;
var proc2 = XGaProcessor<float>.CreateEuclidean(sp2);
var v2 = proc2.CreateComposer()
    .SetVectorTerm(0, 4f)
    .SetVectorTerm(1, 5f)
    .SetVectorTerm(2, 6f)
    .GetVector();

var result = v1.Gp(v2);  // Kompilierungsfehler! Unterschiedliche Typen
```

**Lösung:** Verwenden Sie konsistente Skalartypen:
```csharp
var processor = XGaFloat64Processor.Euclidean;
var v1 = processor.CreateComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 2)
    .SetVectorTerm(2, 3)
    .GetVector();
var v2 = processor.CreateComposer()
    .SetVectorTerm(0, 4)
    .SetVectorTerm(1, 5)
    .SetVectorTerm(2, 6)
    .GetVector();
var result = v1.Gp(v2);  // OK!
```

---

### Fehler 2: Dimensions-Bewusstsein

**Hinweis:** GA-FuL-Prozessoren haben keine feste Dimension. Sie können Vektoren beliebiger Dimension erstellen:

```csharp
var processor = XGaFloat64Processor.Euclidean;

// 3D-Vektor erstellen
var v3d = processor.CreateComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 2)
    .SetVectorTerm(2, 3)
    .GetVector();

// 4D-Vektor mit demselben Prozessor
var v4d = processor.CreateComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 2)
    .SetVectorTerm(2, 3)
    .SetVectorTerm(3, 4)
    .GetVector();

// Beide funktionieren einwandfrei!
```

**Benutzerdefinierte Metrik Beispiel:**
```csharp
// Für nicht-euklidische Metriken (z.B. Minkowski Spacetime)
var spacetime = XGaFloat64Processor.Create(
    negativeCount: 1,  // Zeit-Dimension
    zeroCount: 0
);
```

---

### Fehler 3: Vergessen zu Normalisieren

**Problem:**
```csharp
var processor = XGaFloat64Processor.Euclidean;
var v = processor.CreateComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 2)
    .SetVectorTerm(2, 3)
    .GetVector();
// v ist NICHT normalisiert! Magnitude = sqrt(1² + 2² + 3²) = sqrt(14) ≈ 3.74
```

**Lösung:**
```csharp
var v = processor.CreateComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 2)
    .SetVectorTerm(2, 3)
    .GetVector()
    .Normalize();  // Jetzt Magnitude = 1

// Oder separat:
var v = processor.CreateComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 2)
    .SetVectorTerm(2, 3)
    .GetVector();
v = v.DivideByNorm();  // Identisch mit Normalize()
```

---

## Tipps und Best Practices

1. **Verwenden Sie `var`**: Der Code wird lesbarer
   ```csharp
   var processor = XGaFloat64Processor.Euclidean;
   // statt
   XGaFloat64Processor processor = XGaFloat64Processor.Euclidean;
   ```

2. **Wiederverwendung von Prozessoren**: Erstellen Sie Prozessoren nur einmal
   ```csharp
   // Gut: Prozessor wiederverwenden
   var processor = XGaFloat64Processor.Euclidean;
   var v1 = processor.CreateComposer().SetVectorTerm(0, 1).GetVector();
   var v2 = processor.CreateComposer().SetVectorTerm(1, 1).GetVector();

   // Schlecht: Prozessor mehrfach erstellen
   var v1 = XGaFloat64Processor.Euclidean.CreateComposer().SetVectorTerm(0, 1).GetVector();
   var v2 = XGaFloat64Processor.Euclidean.CreateComposer().SetVectorTerm(1, 1).GetVector();
   ```

3. **Composer wiederverwenden**: Für mehrere ähnliche Multivektoren
   ```csharp
   var processor = XGaFloat64Processor.Euclidean;
   var composer = processor.CreateComposer();

   // Erster Vektor
   composer.Clear();
   composer.SetVectorTerm(0, 1).SetVectorTerm(1, 0);
   var v1 = composer.GetVector();

   // Zweiter Vektor (Composer wiederverwenden)
   composer.Clear();
   composer.SetVectorTerm(0, 0).SetVectorTerm(1, 1);
   var v2 = composer.GetVector();
   ```

4. **Method Chaining für einfache Fälle**
   ```csharp
   // Einfache Vektor-Erstellung mit Chaining
   var v = processor.CreateComposer()
       .SetVectorTerm(0, 1)
       .SetVectorTerm(1, 2)
       .SetVectorTerm(2, 3)
       .GetVector();
   ```

5. **Überprüfen Sie Ihre Ergebnisse**: Nutzen Sie `.ToString()` für Debugging
   ```csharp
   Console.WriteLine($"Result: {result}");
   Console.WriteLine($"Norm: {result.Norm()}");
   ```

---

**Zuletzt aktualisiert**: 2025-10-17 | **API**: Vollständig aktualisiert mit moderner XGaFloat64Processor API

[← Zurück zur Hauptdokumentation](README.de.md)
