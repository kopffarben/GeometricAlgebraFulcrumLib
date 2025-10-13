# Float32 Generator - Context Documentation

**Zweck:** Dokumentation des aktuellen Float32-Generators zur Planung von Verbesserungen
**Datum:** 2025-10-13
**Aktualisiert:** 2025-10-13 (Generator-Only Analyse)
**Version:** v1.0.0

---

## ⚠️ Generator-Only Solution - Status Update

**Frage:** Ist der Generator vollständig ohne manuelle Source-Änderungen?

**Antwort:** **NEIN** - Aktuell existieren 4 manuelle Source-Änderungen (~60 Zeilen):

1. **XGaMetric.cs** (+20 Zeilen)
   - Manuelle Overload: `IsValidMultivectorDictionary(IReadOnlyDictionary<int, XGaFloat32KVector>)`
   - Generator-Only möglich: JA (mit Semantic Model, Phase 2)

2. **XGaBasisBlade.cs** (+8 Zeilen)
   - Manuelle Overload: `ToKVector(XGaFloat32Processor processor)`
   - Generator-Only möglich: JA (mit Semantic Model, Phase 2 Task 2.2.3)

3. **LinBasisVector.cs** (+28 Zeilen)
   - 4 Float32 utility methods: ToVectorTerm32, ToZeroVectorTerm32, etc.
   - Generator-Only möglich: JA (mit Semantic Model context detection)

4. **LinFloat32Vector3DComposerUtilsExtensions.cs** (neue Datei)
   - MathNet.Numerics interop extensions
   - Generator-Only möglich: JA (Generator handled bereits Vector<double>.ToArray())

**Schlussfolgerung:** Alle manuellen Änderungen können durch Phase 2 Semantic Integration eliminiert werden.

**Entscheidung für Benutzer:** Siehe TODO.md für Option A (Generator-Only) vs Option B (Hybrid)

---

## Übersicht

Der Float32-Generator ist ein Roslyn-basierter Source Generator, der automatisch Float64 (double) C#-Code zu Float32 (float) transformiert.

**Aktueller Status:** 97.7% Generator-basiert, 4 manuelle Änderungen für verbleibende 2.3% Fehler

### Architektur

```
Float32Generator
├── F32Gen.cs                          (Entry Point, IIncrementalGenerator)
├── Float32SyntaxRewriter.cs          (Core Transformation Logic)
├── FloatLibraryIdentifierFinder.cs   (Helper: Findet transformierbare Files)
└── Template.sbntxt                   (Scriban Template für Headers)
```

### Statistiken

| Metrik | Wert |
|--------|------|
| Zeilen Code | 1,164 |
| Visit-Methoden | 83 |
| Conditionals | 171 |
| Erfolgsquote | 97.7% (421/431 Fehler behoben) |

---

## 1. Entry Point: F32Gen.cs

### Zweck
Implementiert `IIncrementalGenerator` für .NET 6+ Incremental Source Generators.

### Workflow

```csharp
public void Initialize(IncrementalGeneratorInitializationContext context)
{
    1. Finde alle Float64-Dateien
       ├── Nutze FloatLibraryIdentifierFinder
       ├── Sucht nach "Float64" im Dateinamen oder Namespace
       └── Filtert bereits Float32-Dateien aus

    2. Für jede Datei:
       ├── Parse mit CSharpSyntaxTree.ParseText()
       ├── Transformiere mit Float32SyntaxRewriter
       ├── Generate Header aus Template
       └── Registriere Output mit AddSource()

    3. Output-Struktur:
       └── obj/Generated/GAF.Gen/GAF.Gen.F32Gen/
           ├── [Namespace]/
           │   └── [FileName].g.cs
           └── Meta-Informationen
}
```

### Key Features
- **Incremental:** Regeneriert nur geänderte Dateien
- **Parallel:** Nutzt IncrementalValueProvider für Parallelisierung
- **Template-basiert:** Header aus Scriban-Template

---

## 2. Core: Float32SyntaxRewriter.cs

### Architektur-Prinzip

Erbt von `CSharpSyntaxRewriter` und überschreibt Visit-Methoden für verschiedene Syntax-Node-Typen.

```
CSharpSyntaxRewriter
    └── Float32SyntaxRewriter
            ├── Visit[NodeType] Methoden (83)
            ├── Helper Methods (Transformation Logic)
            └── State Tracking (Context)
```

### State Management

```csharp
private string? _currentClassName;           // Für Blacklist-Checking
private bool _insideVectorComplexMethod;     // Vector<Complex> Context
```

**Problem:** Minimales State Tracking
- Keine Semantic Model Integration
- Keine Type Inference
- Keine Scope-Analyse

---

## 3. Transformations-Übersicht

### 3.1 Namespace & Type Names

#### VisitNamespaceDeclaration / VisitFileScopedNamespaceDeclaration
```csharp
// Float64 → Float32
GeometricAlgebraFulcrumLib.Algebra.Float64.Vectors
    → GeometricAlgebraFulcrumLib.Algebra.Float32.Vectors
```

**Methode:** String-Replacement (`ReplaceFloat64ToFloat32`)

#### VisitClassDeclaration / VisitStructDeclaration / VisitInterfaceDeclaration
```csharp
// Typen: LinFloat64Vector → LinFloat32Vector
// XGaFloat64Processor → XGaFloat32Processor
```

**State Tracking:** Setzt `_currentClassName` für Kontext

#### VisitIdentifierName / VisitGenericName
```csharp
// Identifiers transformieren
// Generics: Vector<double> → Vector<float>
//           IReadOnlyDictionary<int, XGaFloat64KVector>
//               → IReadOnlyDictionary<int, XGaFloat32KVector>
```

**Spezialfall:** Vector<Complex> wird NICHT transformiert (Complex ist immer double-basiert)

---

### 3.2 Using Directives

#### VisitUsingDirective
```csharp
// Filtert Float32-Namespaces heraus (Konflikt-Vermeidung)
// Transformiert Float64 → Float32 in using statements
```

**Float32NamespacesToFilter:**
- `GeometricAlgebraFulcrumLib.Algebra.Scalars.Float32`
- `GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32`
- `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float32`
- usw.

---

### 3.3 Literals & Keywords

#### VisitLiteralExpression
```csharp
// 1.0d → 1.0f
// 1.0 → 1.0f
// Erhält wissenschaftliche Notation (1.5e-10)
// Spezialfall: default(double) → default(float)
```

**Strategie:** Suffix-Replacement mit Präzisions-Erhaltung

#### VisitPredefinedType
```csharp
// double → float
// Keyword-Level Transformation
```

---

### 3.4 Method Invocations (Komplex!)

#### VisitInvocationExpression - Math.* Methoden

**Pattern 1: Standard Math → MathF**
```csharp
Math.Sin(x)    → MathF.Sin(x)
Math.Cos(x)    → MathF.Cos(x)
// 50+ Math-Methoden
```

**Pattern 2: Methoden ohne MathF-Äquivalent**
```csharp
Math.BitDecrement(x)       → (float)Math.BitDecrement(x)
Math.BitIncrement(x)       → (float)Math.BitIncrement(x)
Math.FusedMultiplyAdd(...) → (float)Math.FusedMultiplyAdd(...)
```

**Pattern 3: Chaining-Problematik**
```csharp
// Problem: Cast-Präzedenz
eigenVector.L2Norm().IsNearZero()

// Falsch: (float)eigenVector.L2Norm().IsNearZero()
//         └─────────────────┬────────────────────┘
//           cast bool → float (ERROR!)

// Lösung: Parent-Check
if (parent is MemberAccessExpression && parent.Expression == node)
    return base.Visit(node);  // Don't cast if chained
```

**Pattern 4: MathNet.Numerics Special Cases**
```csharp
// L2Norm() gibt double zurück, aber chained usage ist ok
v.L2Norm()             → (float)v.L2Norm()  (standalone)
v.L2Norm().IsNearZero → v.L2Norm().IsNearZero()  (chained, kein cast)

// Vector<Complex>.ToArray() → float[] benötigt
eigenVector.Real().ToArray()
    → eigenVector.Real().ToArray().Select(x => (float)x).ToArray()
```

---

#### VisitInvocationExpression - Standalone Functions

**Pattern: ToLinVector* Transformationen**
```csharp
// Problem: Methoden ohne Objektpräfix
return ToLinVector4D(vector)

// Lösung in VisitIdentifierName:
if (text.StartsWith("ToLinVector"))
    text = text.Replace("ToLinVector", "ToLinFloat32Vector")
```

**Erfolgreich transformiert:**
- `ToLinVector` → `ToLinFloat32Vector`
- `CreateLinVector` → `CreateLinFloat32Vector`
- `CreateUnitLinVector` → `CreateUnitLinFloat32Vector`

**FEHLT (siehe BUGREPORT.md):**
- `ToUnitLinVector3D` → `ToUnitLinFloat32Vector3D`
- `VectorToVectorRotationQuaternion` → `VectorToVectorRotationFloat32Quaternion`

---

### 3.5 Member Access

#### VisitMemberAccessExpression

**Pattern 1: Property Transformationen**
```csharp
// NICHT transformiert (extern):
double.IsFinite(x)  → float.IsFinite(x)  (separate Behandlung)
```

**Pattern 2: Extension Methods**
```csharp
// .Abs() wird NICHT transformiert (Float32Utils.Abs() Extension existiert)
cosAngle.Abs().IsNearOne()  // bleibt unverändert
```

**Limitierung:** Keine Semantic-Analysis von Extension Methods

---

### 3.6 BitConverter Calls

#### Spezial-Handling für BitConverter
```csharp
BitConverter.DoubleToInt64Bits(x)  → BitConverter.SingleToInt32Bits((float)x)
BitConverter.Int64BitsToDouble(x)  → BitConverter.Int32BitsToSingle((int)x)
```

**Zweck:** Binary-Representation-Konvertierung

---

### 3.7 Cast Expressions

#### VisitCastExpression
```csharp
(double)x → (float)x

// Spezialfall: verschachtelte Casts
(double)(double)x → (float)x  (äußerer Cast wird ersetzt)
```

---

### 3.8 Object Creation

#### VisitObjectCreationExpression
```csharp
new LinFloat64Vector(...)  → new LinFloat32Vector(...)
```

**Transformiert:** Type Name in `new`-Expressions

---

## 4. Helper Methods

### ReplaceFloat64ToFloat32(string text)

**Zentrale Transformation:**
```csharp
text = text
    .Replace("Float64", "Float32")
    .Replace("float64", "float32")
    .Replace("FLOAT64", "FLOAT32")
    .Replace("double", "float")
    .Replace("Double", "Float");
```

**Kontext-Unabhängig:** Pure string replacement (Vor- und Nachteile!)

---

### Spezial-Patterns

#### IsVectorComplexContext
```csharp
// Erkennt Vector<Complex> Member Access
if (memberAccess.Expression.ToString().Contains("Vector<Complex>"))
    _insideVectorComplexMethod = true;
```

**Problem:** String-basiert, nicht Semantic-basiert

---

## 5. Was der Generator NICHT kann

### 5.1 Type Inference / Semantic Analysis

**Problem:**
```csharp
var result = BasisBlade((IndexSet)7).ToKVector();
// Generator weiß NICHT:
// - ToKVector() gibt XGaFloat64KVector zurück
// - result sollte XGaFloat32KVector sein
// - ToKVector() braucht (this) Parameter in Float32
```

**Ursache:** Kein `SemanticModel` verwendet

### 5.2 Method Overload Resolution

**Problem:**
```csharp
// Float64 Version:
basisVectors.VectorPairToVectorPairRotationQuaternion(v1, v2)

// Float32 Version braucht:
basisVectors.VectorPairToVectorPairRotationFloat32Quaternion(v1, v2)

// Generator macht:
basisVectors.VectorPairToVectorPairRotationFloat32Quaternion(v1, v2)

// ABER: Diese Überladung existiert nicht für Float32-Typen!
```

**Ursache:** Generator prüft nicht ob transformierte Signatur existiert

### 5.3 Context-Aware Transformations

**Problem:**
```csharp
// In XGaFloat32Processor-Kontext:
BasisBlade(7).ToKVector()  // sollte: .ToKVector(this)

// In XGaFloat64Processor-Kontext:
BasisBlade(7).ToKVector()  // sollte: .ToKVector() (unverändert)
```

**Ursache:** Keine Scope-Analyse, kein Kontext-Tracking

### 5.4 Return Type Transformations in Chains

**Problem:**
```csharp
// Implizite Return Types werden nicht transformiert:
BasisBlade(7)        // returns XGaBasisBlade
    .ToKVector()     // returns XGaFloat64KVector (PROBLEM!)
    .EInverse()      // expects XGaFloat32KVector

// Generator sieht nur Syntactic Nodes, nicht implizite Typen
```

**Ursache:** Roslyn Type Symbol Resolution fehlt

---

## 6. Generator-Stärken

### ✅ Was funktioniert gut

1. **Namespaces & Type Declarations**
   - 100% Erfolgsquote bei Typ-Namen
   - Zuverlässige String-Replacement

2. **Literals**
   - Robuste Numeric-Literal-Transformation
   - Erhält Präzision und wissenschaftliche Notation

3. **Math Functions**
   - 50+ Math → MathF Transformationen
   - Spezial-Handling für Methoden ohne MathF-Äquivalent

4. **Generics**
   - `Vector<double>` → `Vector<float>`
   - Intelligente Complex-Erkennung

5. **Chaining-Awareness (teilweise)**
   - L2Norm() Chaining korrekt
   - .Abs() Extension Method korrekt

6. **Performance**
   - Incremental Generator: Nur geänderte Files
   - Parallel Processing

---

## 7. Generator-Schwächen

### ❌ Was fehlt / limitiert

1. **Semantic Model Integration**
   - Keine Type Resolution
   - Keine Symbol Information
   - Pure Syntax-Transformation

2. **Context Tracking**
   - Minimales State (_currentClassName, _insideVectorComplexMethod)
   - Keine Scope-Hierarchie
   - Keine Method-Kontext-Erkennung

3. **Validation**
   - Keine Prüfung ob transformierte Signatur existiert
   - Keine Warnung bei fehlenden Überladungen
   - Generator generiert Code der nicht kompiliert

4. **Pattern Coverage**
   - Viele if/else-Chains (171 Conditionals!)
   - Hardcoded Patterns (nicht extensible)
   - Fehlende Patterns (ToUnitLinVector, etc.)

5. **Diagnostics**
   - Keine Generator-Warnings
   - Keine Error-Reporting an IDE
   - Stille Failures

6. **Testability**
   - Keine Unit-Tests sichtbar
   - Schwer zu debuggen (Incremental Generator)

---

## 8. Roslyn-Features NICHT genutzt

### Semantic Model
```csharp
// MÖGLICH aber nicht implementiert:
var semanticModel = compilation.GetSemanticModel(syntaxTree);
var symbolInfo = semanticModel.GetSymbolInfo(node);

// Würde ermöglichen:
// - Type Inference
// - Method Resolution
// - Overload Selection
```

### Symbol Information
```csharp
// MÖGLICH:
var typeSymbol = semanticModel.GetTypeInfo(expression).Type;

// Würde lösen:
// - Return Type Analysis
// - Method Chain Type Tracking
```

### Diagnostics API
```csharp
// MÖGLICH:
context.ReportDiagnostic(Diagnostic.Create(
    descriptor,
    location,
    "Warning: Method overload not found for Float32"
));
```

### Control Flow Analysis
```csharp
// MÖGLICH:
var dataFlowAnalysis = semanticModel.AnalyzeDataFlow(statement);

// Würde ermöglichen:
// - Variable Typ-Tracking
// - Context-Aware Transformations
```

---

## 9. Dependencies & Environment

### NuGet Packages
```xml
<PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.x" />
<PackageReference Include="Scriban" Version="5.x" />
```

### Build Integration
```xml
<!-- In .csproj -->
<ProjectReference Include="..\GeometricAlgebraFulcrumLib.CodeGeneration\..."
                  OutputItemType="Analyzer"
                  ReferenceOutputAssembly="false" />
```

### Output Location
```
obj/Generated/
    GAF.Gen/
        GAF.Gen.F32Gen/
            [Namespace]/
                [FileName].g.cs
```

---

## 10. Code Patterns & Idioms

### Pattern 1: Visit-and-Transform
```csharp
public override SyntaxNode? VisitXXX(XXXSyntax node)
{
    1. Check if transformation needed
    2. Visit children first (base.VisitXXX)
    3. Apply transformation
    4. Return modified node
}
```

### Pattern 2: Conditional Casting
```csharp
var visitedNode = (TypeSyntax)base.VisitXXX(node)!;
// Null-forgiving operator ! überall
// Annahme: base.Visit gibt nie null zurück
```

### Pattern 3: String-Based Detection
```csharp
if (node.ToString().Contains("pattern"))
    // Transform
```
**Problem:** Fragil, nicht semantic

### Pattern 4: Parent-Checking
```csharp
var parent = node.Parent;
if (parent is MemberAccessExpression memberAccess &&
    memberAccess.Expression == node)
{
    // In chained context
}
```

---

## 11. Error-Prone Areas

### 1. Cast Precedence
```csharp
// FALSCH:
(float)x.Method().Property
// parsed as: (float)(x.Method().Property)

// RICHTIG:
((float)x).Method().Property
// oder: x.Method().Property (kein cast wenn Extension)
```

### 2. Method Name Collisions
```csharp
// Problem: Methodenname transformiert, aber Signatur existiert nicht
VectorPairToVectorPairRotationQuaternion(a, b)
    → VectorPairToVectorPairRotationFloat32Quaternion(a, b)

// Lösung fehlt: Prüfung ob Float32-Überladung existiert
```

### 3. Complex Type Chains
```csharp
// Multi-level Chaining:
node.Method1().Method2().Method3()
// Return Type von Method1 beeinflusst Method2
// Generator sieht nur Syntax, nicht Typen
```

---

## 12. Performance Charakteristiken

### Build Time Impact
- **Initial Build:** ~5-10 Sekunden für ~200 Files
- **Incremental:** <1 Sekunde für 1-5 geänderte Files
- **Memory:** ~50-100 MB für Generator Process

### Parallelization
- IncrementalValueProvider ermöglicht Parallel-Processing
- Pro File: ~20-50ms Parse + Transform Time

---

## 13. Maintenance Hinweise

### Bei Erweiterung des Generators:

1. **Neue Patterns hinzufügen:**
   - In `VisitInvocationExpression` oder `VisitIdentifierName`
   - Pattern-Konsistenz beachten
   - String-based vs. Semantic-based entscheiden

2. **State erweitern:**
   - Neue Fields für Context-Tracking
   - Reset-Logik in Visit-Methoden

3. **Semantic Model integrieren:**
   - `SemanticModel` als Constructor-Parameter
   - In F32Gen.cs: `compilation.GetSemanticModel()`
   - Performance-Impact beachten (caching!)

4. **Testing:**
   - Unit-Tests für einzelne Transformationen
   - Integration-Tests mit echtem Code
   - Regression-Tests für behobene Fehler

---

## 14. Referenz: Erfolgreiche Transformationen

### Math Functions (50+)
Sin, Cos, Tan, Asin, Acos, Atan, Atan2, Sinh, Cosh, Tanh, Asinh, Acosh, Atanh, Sqrt, Cbrt, Pow, Exp, Log, Log2, Log10, Floor, Ceiling, Round, Truncate, Abs, Sign, Min, Max, Clamp, CopySign, IEEERemainder, ...

### Type Names
LinFloat64* → LinFloat32*, XGaFloat64* → XGaFloat32*, double → float, Double → Float, Float64 → Float32

### Literals
1.0d → 1.0f, 1e-10 → 1e-10f, default(double) → default(float)

### Generics
Vector<double> → Vector<float>, IReadOnlyDictionary<int, XGaFloat64KVector> → IReadOnlyDictionary<int, XGaFloat32KVector>

---

## Zusammenfassung

**Stärken:**
- Robuste Basis-Transformationen (97.7% Erfolg)
- Gute Performance (Incremental)
- Wartbare Code-Struktur

**Hauptlimitierungen:**
- Keine Semantic Analysis
- Kein Type Inference
- Pattern-basiert statt analysiert
- Keine Validation

**Nächste Schritte:**
Siehe ANALYSE.md und TODO.md
