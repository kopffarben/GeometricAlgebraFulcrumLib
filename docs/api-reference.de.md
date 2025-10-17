---
layout: default
title: "API-Referenz"
lang: de
---

# GA-FuL API-Referenz

**[🇬🇧 English Version](api-reference.en.md)**

## Inhaltsverzeichnis

1. [Übersicht](#übersicht)
2. [Skalar-Prozessoren](#skalar-prozessoren)
3. [GA-Prozessoren](#ga-prozessoren)
4. [Multivektoren](#multivektoren)
5. [Basis-Blades](#basis-blades)
6. [Conformal GA](#conformal-ga)
7. [Metaprogrammierung](#metaprogrammierung)
8. [Utilities](#utilities)

---

## Übersicht

Diese API-Referenz bietet eine Übersicht über die wichtigsten Klassen und Methoden in GA-FuL. Für vollständige Details siehe den Source-Code und IntelliSense-Dokumentation.

---

## Skalar-Prozessoren

### `IScalarProcessor<T>`

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.Scalars`

**Beschreibung:** Interface für Skalar-Operationen

**Wichtige Methoden:**

```csharp
public interface IScalarProcessor<T>
{
    // Arithmetik
    T Add(T a, T b);
    T Subtract(T a, T b);
    T Times(T a, T b);
    T Divide(T a, T b);
    T Negative(T a);
    T Power(T a, T b);

    // Trigonometrie
    T Sin(T a);
    T Cos(T a);
    T Tan(T a);
    T ArcSin(T a);
    T ArcCos(T a);
    T ArcTan(T a);
    T ArcTan2(T y, T x);

    // Exponential und Logarithmus
    T Exp(T a);
    T Log(T a);
    T Log10(T a);
    T Sqrt(T a);

    // Vergleiche
    bool IsZero(T a);
    bool IsOne(T a);
    bool IsMinusOne(T a);
    bool IsPositive(T a);
    bool IsNegative(T a);

    // Konstanten
    T Zero { get; }
    T One { get; }
    T MinusOne { get; }
}
```

---

### `ScalarProcessorOfFloat64`

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.Scalars`

**Beschreibung:** Skalar-Prozessor für 64-bit Gleitkommazahlen

**Verwendung:**

```csharp
// Singleton-Instanz
var sp = ScalarProcessorOfFloat64.Instance;

// Operationen
var result = sp.Add(1.0, 2.0);  // 3.0
var sin = sp.Sin(Math.PI / 2);   // 1.0
```

**Weitere Prozessoren:**
- `ScalarProcessorOfFloat32`: 32-bit floats
- `ScalarProcessorOfDecimal`: Decimal
- `ScalarProcessorOfRational`: Rationale Zahlen
- `ScalarProcessorOfComplex`: Komplexe Zahlen
- `ScalarProcessorOfMathematica`: Symbolische Ausdrücke

---

### `Scalar<T>`

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.Scalars`

**Beschreibung:** Thin-Wrapper für einfachere Skalar-Operationen

**Eigenschaften:**

```csharp
public class Scalar<T>
{
    public IScalarProcessor<T> Processor { get; }
    public T Value { get; }
}
```

**Operatoren:**

```csharp
var sp = ScalarProcessorOfFloat64.Instance;
var a = new Scalar<double>(sp, 2.0);
var b = new Scalar<double>(sp, 3.0);

// Arithmetische Operatoren
var sum = a + b;         // 5.0
var diff = a - b;        // -1.0
var product = a * b;     // 6.0
var quotient = a / b;    // 0.666...

// Vergleiche
bool eq = a == b;        // false
bool lt = a < b;         // true
```

---

## GA-Prozessoren

### `XGaFloat64Processor` (Vereinfachte Float64 API)

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors`

**Beschreibung:** Vereinfachter Float64-spezifischer GA-Prozessor (empfohlen für die meisten Benutzer)

**Konstruktion:**

```csharp
// Euklidisch (alle +1) - Am häufigsten
var processor = XGaFloat64Processor.Euclidean;

// Konformal (1 negative Signatur)
var conformal = XGaFloat64Processor.Conformal;

// Projektiv (1 Null-Signatur)
var projective = XGaFloat64Processor.Projective;

// Benutzerdefinierte Metrik
var custom = XGaFloat64Processor.Create(
    negativeCount: 1,  // Anzahl -1 Quadrate
    zeroCount: 0       // Anzahl  0 Quadrate
);
```

---

### `XGaProcessor<T>` (Generische API)

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended`

**Beschreibung:** Generischer GA-Prozessor für benutzerdefinierte Skalartypen (Rational, Symbolic, etc.)

**Konstruktion:**

```csharp
// Für nicht-Float64 Typen (z.B. Rational, Symbolic)
var scalarProcessor = ScalarProcessorOfRational.Instance;
var processor = XGaProcessor<Rational>.CreateEuclidean(scalarProcessor);
```

**Wichtige Eigenschaften:**

```csharp
public class XGaProcessor<T>
{
    public IScalarProcessor<T> ScalarProcessor { get; }
    public XGaMetric Metric { get; }
    public int VSpaceDimensions { get; }
    public int GaSpaceDimensions { get; }  // 2^VSpaceDimensions
}
```

**Factory-Methoden:**

```csharp
// Skalare
XGaScalar<T> CreateScalar(T value);

// Vektoren (über Composer)
var vector = processor.CreateComposer()
    .SetVectorTerm(0, x)
    .SetVectorTerm(1, y)
    .SetVectorTerm(2, z)
    .GetVector();

// Basis-Vektoren
XGaVector<T> CreateBasisVector(int index);

// Bivektoren (über Composer)
var bivector = processor.CreateBivectorComposer()
    .SetBivectorTerm(0, 1, xy)
    .SetBivectorTerm(0, 2, xz)
    .SetBivectorTerm(1, 2, yz)
    .GetBivector();

// Composer
XGaMultivectorComposer<T> CreateComposer();
```

---

## Multivektoren

### `XGaMultivector<T>`

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended`

**Beschreibung:** Generischer Multivektor

**Eigenschaften:**

```csharp
public class XGaMultivector<T>
{
    public XGaProcessor<T> Processor { get; }
    public IReadOnlyDictionary<int, XGaKVector<T>> KVectors { get; }

    // Grades
    public IEnumerable<int> Grades { get; }
    public int MinGrade { get; }
    public int MaxGrade { get; }

    // Tests
    public bool IsZero { get; }
    public bool IsScalar { get; }
    public bool IsVector { get; }
    public bool IsBivector { get; }
    public bool IsKVector { get; }
}
```

**Produkte:**

```csharp
// Geometrisches Produkt
XGaMultivector<T> Gp(XGaMultivector<T> other);

// Äußeres Produkt (Outer/Wedge)
XGaMultivector<T> Op(XGaMultivector<T> other);

// Linke Kontraktion (Left Contraction)
XGaMultivector<T> Lcp(XGaMultivector<T> other);

// Rechte Kontraktion (Right Contraction)
XGaMultivector<T> Rcp(XGaMultivector<T> other);

// Skalarprodukt
Scalar<T> Sp(XGaMultivector<T> other);

// Fat Dot Product
XGaMultivector<T> Fdp(XGaMultivector<T> other);

// Hestenes Inner Product
XGaMultivector<T> Hip(XGaMultivector<T> other);

// Commutator Product
XGaMultivector<T> Cp(XGaMultivector<T> other);

// Anti-Commutator Product
XGaMultivector<T> Acp(XGaMultivector<T> other);
```

**Unäre Operationen:**

```csharp
// Reverse (Umkehrung)
XGaMultivector<T> Reverse();

// Grade Involution
XGaMultivector<T> GradeInvolution();

// Clifford Conjugate
XGaMultivector<T> CliffordConjugate();

// Dual
XGaMultivector<T> Dual();
XGaMultivector<T> UnDual();

// Negation
XGaMultivector<T> Negative();

// Magnitude
Scalar<T> Norm();
Scalar<T> NormSquared();

// Normalisierung
XGaMultivector<T> Normalize();
XGaMultivector<T> DivideByNorm();

// Inverse
XGaMultivector<T> Inverse();

// Exponential und Logarithmus
XGaMultivector<T> Exp();
XGaMultivector<T> Log();
```

**Grade-Extraktion:**

```csharp
// Spezifischer Grade
XGaKVector<T> GetKVectorPart(int grade);

// Skalarteil
XGaScalar<T> GetScalarPart();

// Vektorteil
XGaVector<T> GetVectorPart();

// Bivektorteil
XGaBivector<T> GetBivectorPart();

// Nur gerade/ungerade Grades
XGaMultivector<T> GetEvenPart();
XGaMultivector<T> GetOddPart();
```

**Arithmetik:**

```csharp
// Addition
XGaMultivector<T> Add(XGaMultivector<T> other);

// Subtraktion
XGaMultivector<T> Subtract(XGaMultivector<T> other);

// Skalare Multiplikation
XGaMultivector<T> Times(T scalar);
XGaMultivector<T> Divide(T scalar);

// Operatoren
var sum = mv1 + mv2;
var diff = mv1 - mv2;
var scaled = 2.0 * mv1;
```

---

### `XGaMultivectorComposer<T>`

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended.Composers`

**Beschreibung:** Builder für Multivektoren

**Verwendung:**

```csharp
var composer = processor.CreateComposer();

// Terme hinzufügen
composer.SetTerm(indexSet, scalar);
composer.AddTerm(indexSet, scalar);

// Basis-Blades hinzufügen
composer.SetBasisBladeScalar(id, scalar);
composer.AddBasisBladeScalar(id, scalar);

// Spezielle Terme
composer.SetScalarTerm(scalar);
composer.SetVectorTerm(index, scalar);
composer.SetBivectorTerm(index1, index2, scalar);

// Multivektor erstellen
var multivector = composer.GetMultivector();

// Oder spezifischen Typ
var scalar = composer.GetScalar();
var vector = composer.GetVector();
var bivector = composer.GetBivector();
var kVector = composer.GetKVector(grade);
```

---

## Basis-Blades

### `XGaBasisBlade`

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Basis`

**Beschreibung:** Repräsentiert ein Basis-Blade

**Eigenschaften:**

```csharp
public class XGaBasisBlade
{
    public IIndexSet IndexSet { get; }
    public int Grade { get; }
    public ulong Id { get; }  // Für Grade < 64
}
```

**Konstruktion:**

```csharp
// Über Processor
var e1 = processor.CreateBasisVector(0);
var e2 = processor.CreateBasisVector(1);
var e12 = processor.CreateBasisBivector(0, 1);

// Direkt
var blade = new XGaBasisBlade(indexSet);
```

**Operationen:**

```csharp
// Produkte
XGaSignedBasisBlade Gp(XGaBasisBlade other, XGaMetric metric);
XGaSignedBasisBlade Op(XGaBasisBlade other);
XGaSignedBasisBlade Lcp(XGaBasisBlade other, XGaMetric metric);

// Unäre Operationen
XGaBasisBlade Reverse();
XGaBasisBlade GradeInvolution();
```

---

### `IIndexSet`

**Namespace:** `GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets`

**Beschreibung:** Interface für Index-Mengen

**Methoden:**

```csharp
public interface IIndexSet
{
    int Count { get; }
    bool Contains(int index);
    IEnumerable<int> GetIndices();
    bool IsSubsetOf(IIndexSet other);
    bool Overlaps(IIndexSet other);

    IIndexSet Union(IIndexSet other);
    IIndexSet Intersect(IIndexSet other);
    IIndexSet Except(IIndexSet other);
}
```

---

## Conformal GA

### `CGaGeometricSpace5D<T>`

**Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa`

**Beschreibung:** 5D Conformal GA für 3D-Geometrie

**Konstruktion:**

```csharp
// Für Float64 (empfohlen)
var cga = CGaFloat64GeometricSpace5D.Instance;

// Für generische Typen
var cga = CGaGeometricSpace5D<T>.Create(scalarProcessor);
```

**Encoding:**

```csharp
// IPNS Round Objects (Inner Product Null Space)
var point = cga.Encode.IpnsRound.Point(x, y, z);
var sphere = cga.Encode.IpnsRound.RealSphere(radius, cx, cy, cz);
var pointPair = cga.Encode.IpnsRound.PointPair(p1, p2);
var circle = cga.Encode.IpnsRound.Circle(center, radius, bivector);

// OPNS Flat Objects (Outer Product Null Space)
var line = cga.Encode.OpnsFlat.Line(point, direction);
var plane = cga.Encode.OpnsFlat.Plane(point, bivector);
var flatPoint = cga.Encode.OpnsFlat.Point(x, y, z);

// IPNS Flat Objects
var ipnsLine = cga.Encode.IpnsFlat.Line(point, direction);
var ipnsPlane = cga.Encode.IpnsFlat.Plane(point, normal);
```

**Decoding:**

```csharp
// Dekodiere IPNS Round
var decoded = blade.DecodeIpnsRound.Element();

// Extrahiere Komponenten
var center = decoded.CenterToVector3D();
var radius = decoded.RealRadius;
var direction = decoded.DirectionToVector3D();
var normal = decoded.NormalDirectionToVector3D();
var bivector = decoded.DirectionToBivector3D();

// Dekodiere OPNS Flat
var decodedFlat = blade.DecodeOpnsFlat.Element();
var point = decodedFlat.PositionToVector3D();
var direction = decodedFlat.DirectionToVector3D();
```

**Transformationen:**

```csharp
// Translation
var translator = cga.CreateTranslator(translationVector);
var translated = translator.OmMap(object);

// Rotation
var rotor = cga.CreateRotor(bivector, angle);
var rotated = rotor.OmMap(object);

// Dilation (Skalierung)
var dilator = cga.CreateDilator(scale);
var scaled = dilator.OmMap(object);

// Inversion
var inverter = cga.CreateInverter(sphere);
var inverted = inverter.OmMap(object);

// Kombinierte Transformation
var combined = translator.Gp(rotor).Gp(dilator);
var transformed = combined.OmMap(object);
```

**Geometrische Operationen:**

```csharp
// Schnitt
var intersection = object1.Op(object2);

// Projektion
var projected = object1.ProjectOn(object2);

// Spiegelung
var reflected = object.ReflectOn(plane);

// Dualisierung
var dual = object.Dual();
```

---

## Metaprogrammierung

### `MetaContext`

**Namespace:** `GeometricAlgebraFulcrumLib.MetaProgramming.Context`

**Beschreibung:** Kontext für Code-Generierung

**Verwendung:**

```csharp
// 1. Kontext erstellen
var context = new MetaContext();

// 2. Parameter definieren
var x = context.CreateParameter("x");
var y = context.CreateParameter("y");

// 3. Konstanten
var pi = context.CreateConstant("pi", Math.PI);

// 4. Berechnungen
var scalarProcessor = context.ScalarProcessor;
var processor = XGaProcessor<IMetaExpression>.CreateEuclidean(scalarProcessor);
var v = processor.CreateComposer()
    .SetVectorTerm(0, x)
    .SetVectorTerm(1, y)
    .SetVectorTerm(2, 0)
    .GetVector();
var result = v.NormSquared();

// 5. Output definieren
context.SetOutputVariable("result", result);

// 6. Optimieren
context.OptimizeContext();

// 7. Code generieren
var codeComposer = new CSharpCodeComposer();
var code = codeComposer.GenerateCode(context);
```

**Optimierungen:**

```csharp
// Konstanten-Propagation
context.EnableConstantPropagation();

// Common Subexpression Elimination
context.EnableCommonSubexpressionElimination();

// Symbolische Vereinfachung (benötigt CAS)
context.EnableSymbolicSimplification();

// Genetische Programmierung
context.EnableGeneticOptimization(
    populationSize: 100,
    generations: 50
);

// Alle Optimierungen
context.OptimizeContext();
```

---

### `IMetaExpression`

**Namespace:** `GeometricAlgebraFulcrumLib.MetaProgramming.Expressions`

**Beschreibung:** Interface für Meta-Expressions

**Hierarchie:**

```csharp
IMetaExpression
├── IMetaExpressionAtomic
│   ├── MetaExpressionNumber        // Konstante
│   └── MetaExpressionVariable      // Variable/Parameter
└── IMetaExpressionComposite
    ├── MetaExpressionNegative      // -a
    ├── MetaExpressionAdd           // a + b
    ├── MetaExpressionSubtract      // a - b
    ├── MetaExpressionTimes         // a * b
    ├── MetaExpressionDivide        // a / b
    ├── MetaExpressionPower         // a^b
    └── MetaExpressionFunction      // sin(a), cos(a), etc.
```

---

### Code-Composer

**Namespace:** `GeometricAlgebraFulcrumLib.MetaProgramming.Composers`

**Verfügbare Composer:**

```csharp
// C#
var csharpComposer = new CSharpCodeComposer();

// C++
var cppComposer = new CppCodeComposer();

// JavaScript
var jsComposer = new JavaScriptCodeComposer();

// Python
var pythonComposer = new PythonCodeComposer();

// MATLAB
var matlabComposer = new MatlabCodeComposer();
```

**Konfiguration:**

```csharp
composer.SetMethodName("ComputeResult");
composer.SetClassName("MyClass");
composer.SetNamespace("MyNamespace");

composer.SetIndentation("    ");  // 4 Spaces
composer.SetLineEnding("\n");

var code = composer.GenerateCode(context);
```

---

## Utilities

### Linear Algebra Utilities

**Namespace:** `GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra`

**Vektoren (Float64):**

```csharp
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space4D;

// 2D Vektor
var v2d = LinFloat64Vector2D.Create(x, y);

// 3D Vektor
var v3d = LinFloat64Vector3D.Create(x, y, z);

// 4D Vektor
var v4d = LinFloat64Vector4D.Create(x, y, z, w);

// n-D Vektor (generisch Float64)
var vnd = Float64Vector.Create(component1, component2, component3, ...);
```

**Vektoren (Generisch):**

```csharp
// Für nicht-Float64 Typen (Rational, Symbolic, etc.)
var v2d = scalarProcessor.CreateVector2D(x, y);
var v3d = scalarProcessor.CreateVector3D(x, y, z);
var v4d = scalarProcessor.CreateVector4D(x, y, z, w);
```

**Matrizen:**

```csharp
// Matrix erstellen
var matrix = scalarProcessor.CreateMatrix(rows, columns);

// Matrix-Operationen
var product = matrix1.Multiply(matrix2);
var transpose = matrix.Transpose();
var inverse = matrix.Inverse();
var determinant = matrix.Determinant();
```

---

### Text Utilities

**Namespace:** `GeometricAlgebraFulcrumLib.Utilities.Text`

**Linear Text Composer:**

```csharp
var composer = new LinearTextComposer();

composer.AppendLine("Hello");
composer.AppendLine("World");
composer.AppendAtNewLine("New line");

var text = composer.ToString();
```

**Parametric Text Composer:**

```csharp
var template = "Hello #name#, you are #age# years old.";
var composer = new ParametricTextComposer("#", "#", template);

composer["name"] = "Alice";
composer["age"] = "30";

var result = composer.GenerateText();
// "Hello Alice, you are 30 years old."
```

---

## Zusammenfassung

Diese API-Referenz bietet eine Übersicht über die wichtigsten Klassen und Methoden. Für:

- **Vollständige API-Details**: Siehe IntelliSense und XML-Kommentare im Code
- **Verwendungsbeispiele**: Siehe [examples.md](examples.en.md)
- **Architektur**: Siehe [architecture.md](architecture.en.md)
- **Getting Started**: Siehe [getting-started.md](getting-started.en.md)

---

[← Zurück zur Hauptdokumentation](README.en.md)
