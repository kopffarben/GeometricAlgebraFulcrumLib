---
layout: default
title: "Erste Schritte"
lang: de
---

# Getting Started mit GA-FuL

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

**Typen:**

```csharp
// Generic GA-Prozessor (beliebige Metrik)
var processor = XGaProcessor<T>.Create(scalarProcessor);

// Euclidean GA (alle e_i^2 = +1)
var euclidean = XGaProcessor<T>.CreateEuclidean(scalarProcessor);

// Conformal GA
var conformal = XGaConformalSpace5D<T>.Create(scalarProcessor);

// Projective GA
var projective = XGaProjectiveSpace<T>.Create(scalarProcessor, dimension);
```

**Mit Metrik:**
```csharp
// Metrik (p, q, r)
// p: Anzahl +1 Quadrate
// q: Anzahl -1 Quadrate
// r: Anzahl  0 Quadrate

// 3D Euclidean: (3, 0, 0)
var ga3d = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// Spacetime (Minkowski): (3, 1, 0)
var spacetime = XGaProcessor<double>.Create(scalarProcessor, 1, 0); // Minkowski spacetime (3,1)

// 5D Conformal: (4, 1, 0)
var cga5d = XGaProcessor<double>.Create(scalarProcessor, 4, 1, 0);
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
var mv = processor.CreateMultivector(...);

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
var sphere = cga.Encode.IpnsRound.Sphere(
    centerX: 1,
    centerY: 1,
    centerZ: 1,
    radius: 2
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
var processor = XGaProcessor<Expr>.Create(scalarProcessor);

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
var sp1 = ScalarProcessorOfFloat64.Instance;
var proc = XGaProcessor<double>.Create(sp1);
var v = proc.CreateVector(1, 2, 3);

// FEHLER: Falscher Skalartyp
var sp2 = ScalarProcessorOfFloat32.Instance;
var other = XGaProcessor<float>.Create(sp2);
var result = v.Gp(other.CreateVector(4, 5, 6));  // Kompilierungsfehler!
```

**Lösung:** Verwenden Sie konsistente Skalartypen:
```csharp
var sp = ScalarProcessorOfFloat64.Instance;
var proc = XGaProcessor<double>.Create(sp);
var v1 = proc.CreateVector(1, 2, 3);
var v2 = proc.CreateVector(4, 5, 6);
var result = v1.Gp(v2);  // OK!
```

---

### Fehler 2: Falsche Metrik

**Problem:**
```csharp
// 3D Euclidean
var proc = XGaProcessor<double>.CreateEuclidean(sp);

// Versuch, 4D-Vektor zu erstellen
var v = proc.CreateVector(1, 2, 3, 4);  // Funktioniert, aber...
// ... der Prozessor erwartet 3D-Metrik!
```

**Lösung:** Metrik explizit definieren:
```csharp
// Für 4D
var proc = XGaProcessor<double>.Create(sp, 4, 0, 0);
var v = proc.CreateVector(1, 2, 3, 4);  // OK!
```

---

### Fehler 3: Vergessen zu Normalisieren

**Problem:**
```csharp
var v = processor.CreateVector(1, 2, 3);
// v ist NICHT normalisiert!
```

**Lösung:**
```csharp
var v = processor.CreateVector(1, 2, 3).Normalize();
// oder
var v = processor.CreateVector(1, 2, 3);
v = v.DivideByNorm();
```

---

## Tipps und Best Practices

1. **Verwenden Sie `var`**: Der Code wird lesbarer
   ```csharp
   var processor = XGaProcessor<double>.Create(...);
   // statt
   XGaProcessor<double> processor = XGaProcessor<double>.Create(...);
   ```

2. **Wiederverwendung von Prozessoren**: Erstellen Sie Prozessoren nur einmal
   ```csharp
   // Gut
   var processor = XGaProcessor<double>.CreateEuclidean(sp);
   var v1 = processor.CreateVector(...);
   var v2 = processor.CreateVector(...);

   // Schlecht
   var v1 = XGaProcessor<double>.CreateEuclidean(sp).CreateVector(...);
   var v2 = XGaProcessor<double>.CreateEuclidean(sp).CreateVector(...);
   ```

3. **Verwenden Sie Composer für komplexe Multivektoren**
   ```csharp
   var composer = processor.CreateComposer();
   composer.SetTerm(...);
   composer.AddTerm(...);
   var mv = composer.GetMultivector();
   ```

4. **Überprüfen Sie Ihre Ergebnisse**: Nutzen Sie `.ToString()` für Debugging
   ```csharp
   Console.WriteLine($"Result: {result}");
   ```

---

[← Zurück zur Hauptdokumentation](README.en.md)
