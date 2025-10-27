---
layout: default
title: "Beispiele"
lang: de
---

# GA-FuL Verwendungsbeispiele

⚠️ **Hinweis**: Die deutsche Version wird aktualisiert. Für aktuelle API-Beispiele siehe **[English Version](examples.en.md)**.

**Status**: In Aktualisierung | **Letztes Update**: 2025-10-17

---

## Inhaltsverzeichnis

1. [Grundlegende GA-Operationen](#grundlegende-ga-operationen)
2. [3D Conformal Geometric Algebra](#3d-conformal-geometric-algebra)
3. [Rotationen und Transformationen](#rotationen-und-transformationen)
4. [Symbolische Berechnungen](#symbolische-berechnungen)
5. [Code-Generierung](#code-generierung)
6. [Erweiterte Beispiele](#erweiterte-beispiele)

---

## Grundlegende GA-Operationen

> **⚡ Performance-Hinweis:** Die Beispiele in diesem Abschnitt verwenden `XGaFloat64Processor` (Float64 Specialized) für Einfachheit. Für **27% schnellere Performance** in Produktionscode verwenden Sie `XGaProcessor<double>`.

### Beispiel 1: Vektorprodukte in 3D

```csharp
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra;
using GeometricAlgebraFulcrumLib.Algebra.Scalars;

// Setup
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.Create(scalarProcessor, 3, 0, 0);

// Definiere zwei Vektoren
var v1 = processor.CreateVectorComposer().SetVectorTerm(0, 1.0).GetVector();  // x-Achse
var v2 = processor.CreateVectorComposer().SetVectorTerm(1, 1.0).GetVector();  // y-Achse

// Geometrisches Produkt: v1 * v2 = v1·v2 + v1∧v2
var gp = v1.Gp(v2);
Console.WriteLine($"Geometrisches Produkt: {gp}");
// Ausgabe: '1'<1,2> (xy-bivector)

// Äußeres Produkt (Wedge): v1 ∧ v2
var wedge = v1.Op(v2);
Console.WriteLine($"Äußeres Produkt: {wedge}");
// Ausgabe: '1'<1,2> (xy-bivector)

// Skalarprodukt: v1·v2
var dot = v1.Sp(v2);
Console.WriteLine($"Skalarprodukt: {dot}");
// Ausgabe: 0 (orthogonal)

// Nicht-orthogonale Vektoren
var v3 = processor.CreateVectorComposer().SetVectorTerm(0, 1.0).SetVectorTerm(1, 1.0).GetVector();
var dot2 = v1.Sp(v3);
Console.WriteLine($"Skalarprodukt v1·v3: {dot2}");
// Ausgabe: 1
```

---

### Beispiel 2: Kreuzprodukt durch Dualität

Das Kreuzprodukt in 3D kann durch GA elegant ausgedrückt werden:

```csharp
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

var a = processor.CreateVectorComposer().SetVectorTerm(0, 1.0).GetVector();
var b = processor.CreateVectorComposer().SetVectorTerm(1, 1.0).GetVector();

// Kreuzprodukt: a × b = -(a ∧ b) * I^{-1}
// wobei I das Pseudoskalar ist

// Methode 1: Über Bivector und Dual
var bivector = a.Op(b);        // a ∧ b
var cross = bivector.Dual();   // Dualisierung

Console.WriteLine($"Kreuzprodukt a × b: {cross}");
// Ausgabe: '1'<3> (z-Vektor)

// Methode 2: Direkt (Hinweis: Cross() ist eine Utility-Erweiterungsmethode)
var cross2 = a.Cross(b);
Console.WriteLine($"Kreuzprodukt (direkt): {cross2}");
```

---

### Beispiel 3: Rotoren und Rotationen

```csharp
using System;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// Definiere Rotationsebene (bivector)
var B = processor.CreateBivector(
    xy: 1,  // Rotation in xy-Ebene
    xz: 0,
    yz: 0
).Normalize();

// Rotationswinkel
var angle = Math.PI / 2;  // 90°

// Erstelle Rotor: R = exp(-θ/2 B)
var rotor = (B.Times(-angle / 2.0)).Exp();

// Vektor zum Rotieren
var v = processor.CreateVectorComposer().SetVectorTerm(0, 1.0).GetVector();

// Rotation: v' = R v R†
var vRotated = rotor.OmMap(v);

Console.WriteLine($"Original: {v}");
Console.WriteLine($"Rotiert (90° in xy): {vRotated}");
// Ausgabe: (0, 1, 0) - jetzt entlang y-Achse
```

---

## 3D Conformal Geometric Algebra

> **⚡ Performance-Hinweis:** CGA-Beispiele verwenden `CGaFloat64GeometricSpace5D` (Float64 Specialized) für Einfachheit. Für **27% schnellere** CGA-Operationen in Produktionscode sollten Sie die Generic<T>-Implementierung in Betracht ziehen.

### Beispiel 4: Punkte, Linien, Ebenen

```csharp
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var cga = CGaFloat64GeometricSpace5D.Instance;

// Kodiere 3D-Punkte in 5D-CGA
var p1 = cga.Encode.IpnsRound.Point(0, 0, 0);
var p2 = cga.Encode.IpnsRound.Point(1, 0, 0);
var p3 = cga.Encode.IpnsRound.Point(0, 1, 0);

// Ebene durch drei Punkte: π = p1 ∧ p2 ∧ p3
var plane = p1.Op(p2).Op(p3);

Console.WriteLine($"Ebene durch drei Punkte: {plane}");

// Kodiere eine Linie
var linePoint = cga.Encode.IpnsRound.Point(0, 0, 0);
var lineDirection = scalarProcessor.CreateLinVector3D(1, 1, 1);
var line = cga.Encode.OpnsFlat.Line(linePoint, lineDirection);

Console.WriteLine($"Linie: {line}");

// Schneide Linie mit Ebene
var intersection = line.Op(plane);

Console.WriteLine($"Schnittpunkt: {intersection}");
```

---

### Beispiel 5: Kugeln und Kreise

```csharp
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var cga = CGaFloat64GeometricSpace5D.Instance;

// Definiere Kugel
var sphere = cga.Encode.IpnsRound.Sphere(
    centerX: 0,
    centerY: 0,
    centerZ: 0,
    radius: 1
);

// Definiere Ebene (z = 0)
var p1 = cga.Encode.IpnsRound.Point(-1, -1, 0);
var p2 = cga.Encode.IpnsRound.Point(1, -1, 0);
var p3 = cga.Encode.IpnsRound.Point(0, 1, 0);
var plane = p1.Op(p2).Op(p3);

// Schneide Kugel mit Ebene → Kreis
var circle = sphere.Op(plane);

// Dekodiere Kreis (verwende entsprechende Decode-Methode)
var circleData = circle.DecodeOpnsRound.Element();

Console.WriteLine($"Kreis-Zentrum: {circleData.CenterToVector3D()}");
Console.WriteLine($"Kreis-Radius: {circleData.RealRadius}");
Console.WriteLine($"Kreis-Normal: {circleData.NormalDirectionToVector3D()}");

// Ausgabe:
// Kreis-Zentrum: (0, 0, 0)
// Kreis-Radius: 1
// Kreis-Normal: (0, 0, 1)
```

---

### Beispiel 6: Transformationen in CGA

```csharp
using System;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var cga = CGaFloat64GeometricSpace5D.Instance;

// Definiere einen Punkt
var point = cga.Encode.IpnsRound.Point(1, 0, 0);

// Translation
var translationVector = scalarProcessor.CreateLinVector3D(1, 2, 3);
var translator = cga.CreateTranslator(translationVector);
var translatedPoint = translator.OmMap(point);

Console.WriteLine($"Original: {point.DecodeIpnsRound.Point()}");
Console.WriteLine($"Transliert: {translatedPoint.DecodeIpnsRound.Point()}");

// Rotation
var angle = Math.PI / 4;  // 45°
var euclideanProcessor = cga.EuclideanProcessor;
var bivector = euclideanProcessor.CreateBivector(xy: 1, xz: 0, yz: 0);
var rotator = cga.CreateRotor(bivector.Times(-angle / 2.0).Exp());
var rotatedPoint = rotator.OmMap(point);

Console.WriteLine($"Rotiert: {rotatedPoint.DecodeIpnsRound.Point()}");

// Skalierung (Dilation)
var scale = 2.0;
var dilator = cga.CreateDilator(scale);
var scaledPoint = dilator.OmMap(point);

Console.WriteLine($"Skaliert: {scaledPoint.DecodeIpnsRound.Point()}");

// Kombinierte Transformation
var combined = translator.Gp(rotator).Gp(dilator);
var transformedPoint = combined.OmMap(point);

Console.WriteLine($"Kombiniert transformiert: {transformedPoint.DecodeIpnsRound.Point()}");
```

---

## Rotationen und Transformationen

> **🚀 Performance-Hinweis:** Die Beispiele in diesem Abschnitt verwenden bereits `XGaProcessor<double>` (Generic<T>-Implementierung) für optimale Performance (**27% schneller** als Float64 Specialized).

### Beispiel 7: Rotation um beliebige Achse

```csharp
using System;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// Rotationsachse (muss normalisiert sein)
var axis = processor.CreateVectorComposer().SetVectorTerm(0, 1.0).SetVectorTerm(1, 1.0).SetVectorTerm(2, 1.0).GetVector().Normalize();

// Rotationswinkel
var angle = Math.PI / 3;  // 60°

// Erstelle Rotor für Rotation um Achse
// R = exp(-θ/2 n)
// wobei n der Bivector normal zur Rotationsachse ist

// Methode: Verwende Rodrigues-Formel in GA
var halfAngle = angle / 2;
var rotor = processor.CreateScalar(Math.Cos(halfAngle))
    .Add(axis.Dual().Times(-Math.Sin(halfAngle)));

// Vektor zum Rotieren
var v = processor.CreateVectorComposer().SetVectorTerm(0, 1.0).GetVector();

// Rotation: v' = R v R†
var vRotated = rotor.Gp(v).Gp(rotor.Reverse());

Console.WriteLine($"Original: {v}");
Console.WriteLine($"Rotiert um (1,1,1) um 60°: {vRotated}");
```

---

### Beispiel 8: Spiegelung an Ebene

```csharp
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// Normale der Spiegelebene (z.B. xy-Ebene, Normale = z-Achse)
var normal = processor.CreateVectorComposer().SetVectorTerm(2, 1.0).GetVector();

// Vektor zum Spiegeln
var v = processor.CreateVectorComposer().SetVectorTerm(0, 1.0).SetVectorTerm(1, 1.0).SetVectorTerm(2, 1.0).GetVector();

// Spiegelung: v' = -n v n / (n·n)
var reflected = normal.Gp(v).Gp(normal).Negative().DivideByNorm();

Console.WriteLine($"Original: {v}");
Console.WriteLine($"Gespiegelt an xy-Ebene: {reflected}");
// Ausgabe: (1, 1, -1)
```

---

## Symbolische Berechnungen

### Beispiel 9: Symbolische GA mit Mathematica

```csharp
using GeometricAlgebraFulcrumLib.Mathematica;
using Wolfram.NETLink;

// Initialisiere Mathematica
var kernel = MathematicaInterface.DefaultKernel;
var scalarProcessor = ScalarProcessorOfMathematica.Instance;
var processor = XGaProcessor<Expr>.Create(scalarProcessor);

// Erstelle symbolische Parameter
var x = scalarProcessor.CreateSymbol("x");
var y = scalarProcessor.CreateSymbol("y");
var z = scalarProcessor.CreateSymbol("z");

// Vektor mit symbolischen Komponenten
var v = processor.CreateVectorComposer().SetVectorTerm(0, x).SetVectorTerm(1, y).SetVectorTerm(2, z).GetVector();

// Berechne Norm^2
var normSq = v.NormSquared();

Console.WriteLine($"||v||^2 = {normSq}");
// Ausgabe: x^2 + y^2 + z^2

// Geometrisches Produkt mit sich selbst
var vv = v.Gp(v);

Console.WriteLine($"v * v = {vv}");
// Ausgabe: x^2 + y^2 + z^2 (Skalar)

// Ableitung nach x
var dvdx = scalarProcessor.Differentiate(normSq, "x");

Console.WriteLine($"d(||v||^2)/dx = {dvdx}");
// Ausgabe: 2*x
```

---

### Beispiel 10: Symbolische Vereinfachung

```csharp
var scalarProcessor = ScalarProcessorOfMathematica.Instance;
var processor = XGaProcessor<Expr>.Create(scalarProcessor);

// Definiere symbolische Vektoren
var a = processor.CreateVectorComposer()
    .SetVectorTerm(0, scalarProcessor.CreateSymbol("a1"))
    .SetVectorTerm(1, scalarProcessor.CreateSymbol("a2"))
    .SetVectorTerm(2, scalarProcessor.CreateSymbol("a3"))
    .GetVector();

var b = processor.CreateVectorComposer()
    .SetVectorTerm(0, scalarProcessor.CreateSymbol("b1"))
    .SetVectorTerm(1, scalarProcessor.CreateSymbol("b2"))
    .SetVectorTerm(2, scalarProcessor.CreateSymbol("b3"))
    .GetVector();

// Berechne (a + b) · (a + b)
var sum = a.Add(b);
var dotProduct = sum.Sp(sum);

// Vereinfache
var simplified = scalarProcessor.Simplify(dotProduct);

Console.WriteLine($"(a + b) · (a + b) = {simplified}");
// Ausgabe: a1^2 + 2*a1*b1 + b1^2 + a2^2 + 2*a2*b2 + b2^2 + a3^2 + 2*a3*b3 + b3^2
```

---

## Code-Generierung

### Beispiel 11: Generierung von C#-Code

```csharp
using GeometricAlgebraFulcrumLib.MetaProgramming;

// 1. Meta-Kontext erstellen
var context = new MetaContext();
var scalarProcessor = context.ScalarProcessor;

// 2. Input-Parameter definieren
var x1 = context.CreateParameter("x1");
var x2 = context.CreateParameter("x2");
var x3 = context.CreateParameter("x3");

var y1 = context.CreateParameter("y1");
var y2 = context.CreateParameter("y2");
var y3 = context.CreateParameter("y3");

// 3. GA-Berechnungen
var processor = XGaProcessor<IMetaExpression>.CreateEuclidean(scalarProcessor);

var v1 = processor.CreateVectorComposer().SetVectorTerm(0, x1).SetVectorTerm(1, x2).SetVectorTerm(2, x3).GetVector();
var v2 = processor.CreateVectorComposer().SetVectorTerm(0, y1).SetVectorTerm(1, y2).SetVectorTerm(2, y3).GetVector();

// Kreuzprodukt
var cross = v1.Cross(v2);

// 4. Output-Variablen definieren
var crossComponents = cross.GetVectorComponents();
context.SetOutputVariable("result_x", crossComponents[0]);
context.SetOutputVariable("result_y", crossComponents[1]);
context.SetOutputVariable("result_z", crossComponents[2]);

// 5. Optimieren
context.OptimizeContext();

// 6. C#-Code generieren
var codeComposer = new CSharpCodeComposer();
codeComposer.SetMethodName("CrossProduct");
var code = codeComposer.GenerateCode(context);

Console.WriteLine(code);
```

**Generierter Code:**
```csharp
public static void CrossProduct(
    double x1, double x2, double x3,
    double y1, double y2, double y3,
    out double result_x,
    out double result_y,
    out double result_z
)
{
    // temp variables
    var temp_0 = x2 * y3;
    var temp_1 = x3 * y2;
    var temp_2 = x1 * y3;
    var temp_3 = x3 * y1;
    var temp_4 = x1 * y2;
    var temp_5 = x2 * y1;

    // output variables
    result_x = temp_0 - temp_1;
    result_y = temp_3 - temp_2;
    result_z = temp_4 - temp_5;
}
```

---

### Beispiel 12: C++-Code-Generierung

```csharp
var context = new MetaContext();
var scalarProcessor = context.ScalarProcessor;

// Parameter
var x = context.CreateParameter("x");
var y = context.CreateParameter("y");

// Berechnung
var processor = XGaProcessor<IMetaExpression>.CreateEuclidean(scalarProcessor);
var v = processor.CreateVectorComposer().SetVectorTerm(0, x).SetVectorTerm(1, y).GetVector();
var normSq = v.NormSquared();

// Output
context.SetOutputVariable("result", normSq);
context.OptimizeContext();

// C++-Code
var cppComposer = new CppCodeComposer();
cppComposer.SetFunctionName("ComputeNormSquared");
var code = cppComposer.GenerateCode(context);

Console.WriteLine(code);
```

**Generierter C++-Code:**
```cpp
void ComputeNormSquared(double x, double y, double& result) {
    // temp variables
    double temp_0 = x * x;
    double temp_1 = y * y;

    // output
    result = temp_0 + temp_1;
}
```

---

## Erweiterte Beispiele

> **🚀 Performance-Hinweis:** Die erweiterten Beispiele verwenden Generic<T>-Implementierungen (`XGaProjectiveSpace<double>`, `XGaProcessor<double>`) für optimale Performance.

### Beispiel 13: Projektive Geometrie

```csharp
using GeometricAlgebraFulcrumLib.Modeling.Geometry.PGa;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var pga = XGaProjectiveSpace<double>.Create(scalarProcessor, 3);

// Punkte in projektiven Koordinaten
var p1 = pga.EncodePoint(1, 0, 0);
var p2 = pga.EncodePoint(0, 1, 0);

// Linie durch zwei Punkte
var line = p1.Op(p2);

// Ebene
var p3 = pga.EncodePoint(0, 0, 1);
var plane = p1.Op(p2).Op(p3);

Console.WriteLine($"Linie: {line}");
Console.WriteLine($"Ebene: {plane}");
```

---

### Beispiel 14: Quaternionen in GA

Quaternionen können als Even-Grade-Multivektoren in 3D-GA dargestellt werden:

```csharp
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// Quaternion: q = a + b*i + c*j + d*k
// In GA: q = a + b*e23 + c*e31 + d*e12

var a = 1.0;
var b = 0.0;
var c = 0.0;
var d = 1.0;

var quaternion = processor.CreateVectorComposer()
    .SetTerm(0, a)                    // Skalar
    .SetBivectorTerm(1, 2, b)         // e_yz (entspricht i)
    .SetBivectorTerm(2, 0, c)         // e_zx (entspricht j)
    .SetBivectorTerm(0, 1, d)         // e_xy (entspricht k)
    .GetMultivector();

Console.WriteLine($"Quaternion in GA: {quaternion}");

// Quaternion-Multiplikation = Geometrisches Produkt
var q1 = quaternion;
var q2 = processor.CreateVectorComposer()
    .SetTerm(0, 0.707)
    .SetBivectorTerm(0, 1, 0.707)
    .GetMultivector();

var product = q1.Gp(q2);

Console.WriteLine($"Quaternion-Produkt: {product}");
```

---

### Beispiel 15: Plücker-Koordinaten für Linien

```csharp
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// Linie definiert durch zwei Punkte
var p1 = processor.CreateVectorComposer().GetVector(); // Origin
var p2 = processor.CreateVectorComposer().SetVectorTerm(0, 1.0).SetVectorTerm(1, 1.0).GetVector();

// Plücker-Koordinaten: L = p1 ∧ p2
var lineDirection = p2.Subtract(p1);
var lineMoment = p1.Op(p2);

Console.WriteLine($"Linien-Richtung: {lineDirection}");
Console.WriteLine($"Linien-Moment: {lineMoment}");

// Linien-Bivector (6D-Darstellung)
var lineBivector = lineMoment.Add(lineDirection.Dual());

Console.WriteLine($"Linien-Bivector: {lineBivector}");
```

---

### Beispiel 16: Interpolation mit Rotoren

```csharp
using System;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

// Zwei Rotoren definieren
var B1 = processor.CreateBivector(xy: 1, xz: 0, yz: 0).Normalize();
var rotor1 = processor.CreateScalar(1.0);  // Identity

var B2 = processor.CreateBivector(xy: 1, xz: 0, yz: 0).Normalize();
var rotor2 = (B2.Times(-Math.PI / 4.0)).Exp();  // 90°

// Interpolation (SLERP in GA)
var t = 0.5;  // 50% zwischen rotor1 und rotor2

var logRotor = rotor2.Gp(rotor1.Reverse()).Log();
var interpolated = (logRotor.Times(t)).Exp().Gp(rotor1);

// Wende interpolierten Rotor an
var v = processor.CreateVectorComposer().SetVectorTerm(0, 1.0).GetVector();
var result = interpolated.OmMap(v);

Console.WriteLine($"Interpoliert (t=0.5): {result}");
// Ergebnis: 45° Rotation
```

---

### Beispiel 17: Möbius-Transformation in CGA

```csharp
using System;

var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var cga = CGaFloat64GeometricSpace5D.Instance;

// Punkt in 3D
var point = cga.Encode.IpnsRound.Point(1, 0, 0);

// Inversionskugel (Ursprung, Radius 1)
var sphere = cga.Encode.IpnsRound.Sphere(0, 0, 0, 1);

// Inversion am Punkt
// p' = sphere * p * sphere / |sphere|^2
var inverted = sphere.Gp(point).Gp(sphere).DivideByNorm();

var invertedPoint = inverted.DecodeIpnsRound.Point();

Console.WriteLine($"Original: (1, 0, 0)");
Console.WriteLine($"Invertiert: {invertedPoint}");
// Ausgabe: (1, 0, 0) - Punkt liegt auf Inversionssphäre
```

---

### Beispiel 18: Signal-Processing mit GA

```csharp
using GeometricAlgebraFulcrumLib.Algebra.Scalars;

// Erstelle Signal-Prozessor
var signalProcessor = ScalarProcessorOfSignal.Create(sampleRate: 44100);

// Erzeuge Signal-Vektoren
var signal1 = signalProcessor.CreateSineWave(frequency: 440, duration: 1.0);
var signal2 = signalProcessor.CreateSineWave(frequency: 880, duration: 1.0);

var processor = XGaProcessor<Signal>.Create(signalProcessor);

var v1 = processor.CreateVector(signal1, signal2);

// GA-Operationen auf Signalen
var norm = v1.Norm();

Console.WriteLine($"Signal-Norm: {norm}");
```

---

## Zusammenfassung

Diese Beispiele zeigen die Vielseitigkeit von GA-FuL:

✓ **Grundlegende GA-Operationen**: Produkte, Dualität
✓ **3D-Geometrie**: CGA für Punkte, Linien, Ebenen, Kugeln
✓ **Transformationen**: Rotationen, Spiegelungen, Translationen
✓ **Symbolische Mathematik**: Mathematica-Integration
✓ **Code-Generierung**: Optimierter Code in C++, C#, etc.
✓ **Erweiterte Anwendungen**: Quaternionen, Plücker-Koordinaten, Signal-Processing

Für weitere Beispiele siehe die Projekte im Repository:
- `GeometricAlgebraFulcrumLib.Applications`
- `GeometricAlgebraFulcrumLib.Samples.Generations`
- `GeometricAlgebraFulcrumLib.UnitTests`

---

**Letzte Aktualisierung**: 2025-10-27 | **Beispiele**: 18 vollständige Beispiele | **Performance**: Generic<T> 1,24-2,31x schneller 🚀

[← Zurück zur Hauptdokumentation](README.de.md)
