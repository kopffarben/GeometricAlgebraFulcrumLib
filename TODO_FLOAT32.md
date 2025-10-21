# Float32 Implementation - Design Document

**Goal:** Create a robust Roslyn-based code generator to automatically convert the entire Float64 implementation to Float32, with full semantic validation and no performance-degrading casts.

**Status:** Ready for Implementation

---

## Executive Summary

### Scope

**Total Files to Convert: ~845 files**

1. **Algebra Layer**: XGa/RGa Float64 → Float32 (**329 files**)
2. **Modeling Layer**: XGa/RGa Float64 → Float32 (**374 files**)
3. **LinearAlgebra**: LinFloat64* → LinFloat32* (**138 files**)
   - LinFloat64Quaternion (147 references in Modeling layer)
   - LinFloat64Vector3D, LinFloat64Bivector3D, LinFloat64Angle, etc.
4. **Utilities.Structures**: Float64Sparse* → Float32Sparse* (**2 files**)
   - Float64SparseVector
   - Float64SparseArray

**New Code to Create: ~5-10 files**

5. **MetaProgramming Code Generation Support** (NEW)
   - MetaExpressionToCSharpFloat32Converter.cs
   - MetaExpressionToCppFloat32Converter.cs (optional)
   - MetaExpressionToMatlabFloat32Converter.cs (optional)
   - GaFuLCSharpServer.CSharpFloat32() factory method

**Code to Refactor: ~4 files**

6. **Utilities.Code Parameterization**
   - CclCSharpCodeGenerator.cs (parameterize ScalarTypeName)
   - CclCppCodeGenerator.cs (parameterize ScalarTypeName)
   - CclMatlabCodeGenerator.cs (parameterize ScalarTypeName)
   - CclExcelCodeGenerator.cs (parameterize ScalarTypeName)

### Timeline Estimates

| Phase | Description | Conservative | Aggressive |
|-------|-------------|--------------|-----------|
| **Phase 0** | Interface Refactoring | 51h (6.5 days) | 29h (3.5 days) |
| **Phase 1** | Generator Development | 39h (5 days) | 28h (3.5 days) |
| **Phase 1A** | MetaProgramming Support | 9h (1 day) | 6h (0.75 days) |
| **Phase 2** | Layer-by-Layer Conversion | 19h (2.5 days) | 11h (1.5 days) |
| **Phase 3** | Testing & Validation | 8h (1 day) | 4h (0.5 days) |
| **TOTAL** | | **126h ≈ 16 days** | **78h ≈ 10 days** |

**Recommended Planning:** 135h (17 days) with 10% buffer

### Key Design Decisions

1. **Breaking Interface Change:** `IScalarProcessor<T>` → `IScalarProcessor<T, TPrecision>`
   - Requires .NET 7+ for `INumberBase<TPrecision>` constraint
   - Enables clean Float32 code generation without casts
   - All processors, multivectors, and composers must be updated

2. **Roslyn-Based Code Generation:**
   - Semantic-aware transformations (not regex)
   - 5-phase validation pipeline
   - Preserves comments and formatting

3. **Category-Based Literal Conversion:**
   - Ultra-small epsilons (<1e-10) → Clamped to 1e-7f
   - Normal values → Direct conversion
   - Maintains semantic meaning where possible

4. **Comprehensive Testing:**
   - 30% of total time dedicated to testing
   - Unit, Component, Integration, E2E, Smoke, Differential tests
   - Performance benchmarks (Float32 vs Float64)

---

## Architecture & Design Decisions

### Decision 1: IScalarProcessor<T, TPrecision> Interface

**Current Problem:**
```csharp
public interface IScalarProcessor<T>
{
    double ZeroEpsilon { get; set; }  // ❌ Always double, even for float!
    double ToFloat64(T scalar);        // ❌ Forces double conversion
    Scalar<T> ScalarFromRandom(Random gen, double min, double max);  // ❌ double params
}
```

**Solution:**
```csharp
using System.Numerics;

public interface IScalarProcessor<T, TPrecision>
    where TPrecision : struct, INumberBase<TPrecision>
{
    TPrecision ZeroEpsilon { get; set; }
    TPrecision ToPrecision(T scalar);
    T GetScalarFromPrecision(TPrecision number);
    Scalar<T> ScalarFromRandom(Random gen, TPrecision min, TPrecision max);

    // ... all other methods
}
```

**Implementations:**
```csharp
// Float64
IScalarProcessor<double, double>

// Float32
IScalarProcessor<float, float>

// Complex (precision = magnitude precision)
IScalarProcessor<Complex, double>

// Symbolic (precision = evaluation precision)
IScalarProcessor<IMetaExpression, double>
```

**Impact:**
- All processor types must add TPrecision parameter
- All multivector/composer types must propagate TPrecision
- ~50-60 files affected in Phase 0
- **Requires .NET 7+** for `INumberBase<T>`

---

### Decision 2: ScalarProcessorNumberUtils Refactoring

**File:** `GeometricAlgebraFulcrumLib.Algebra/Scalars/Generic/ScalarProcessorNumberUtils.cs`

**Current Problem:**
- Contains **48 extension methods** for `IScalarProcessor<T>`
- ALL use hardcoded `scalarProcessor.ToFloat64()` and `double.*` static methods

**Current Code:**
```csharp
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static bool IsNumber<T>(this IScalarProcessor<T> scalarProcessor, T scalar)
{
    var number = scalarProcessor.ToFloat64(scalar);  // ❌ Hardcoded double
    return !double.IsNaN(number);                    // ❌ double.IsNaN
}
```

**Solution:**
```csharp
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static bool IsNumber<T, TPrecision>(
    this IScalarProcessor<T, TPrecision> scalarProcessor,
    T scalar)
    where TPrecision : struct, INumberBase<TPrecision>
{
    var number = scalarProcessor.ToPrecision(scalar);  // ✅ Generic!
    return !TPrecision.IsNaN(number);                  // ✅ TPrecision.IsNaN
}
```

**Impact:**
- All 48 methods must be refactored
- Estimated time: 4-5h (Conservative), 3-4h (Aggressive)

---

### Decision 3: Literal Conversion Strategy

**Challenge:** Converting double precision constants to float requires semantic understanding.

**Category-Based Conversion:**

```csharp
private float ConvertLiteralToFloat32Semantic(double value)
{
    const double Float32Epsilon = 1e-7;
    const double Float32Max = 3.4028235e38;

    // Special cases
    if (double.IsNaN(value)) return float.NaN;
    if (double.IsPositiveInfinity(value)) return float.PositiveInfinity;
    if (value == 0.0) return 0.0f;

    double absValue = Math.Abs(value);
    int sign = Math.Sign(value);

    // Category 1: Ultra-small epsilons (< 1e-10) → Clamp to Float32 epsilon
    if (absValue > 0 && absValue < 1e-10)
        return (float)(sign * Float32Epsilon);

    // Category 2: Small epsilons (1e-10 to 1e-6) → Direct conversion
    if (absValue >= 1e-10 && absValue < 1e-6)
        return (float)value;

    // Category 3: Normal values (1e-6 to 1e6) → Direct conversion
    if (absValue >= 1e-6 && absValue <= 1e6)
        return (float)value;

    // Category 4: Large values (> 1e6) → Direct conversion
    if (absValue > 1e6 && absValue <= Float32Max)
        return (float)value;

    // Category 5: Out of range → Clamp
    if (absValue > Float32Max)
        return (float)(sign * Float32Max);

    return (float)value;
}
```

**Conversion Examples:**

| Original (Float64) | Category | Converted (Float32) | Reason |
|-------------------|----------|---------------------|--------|
| `1e-20` | 1 | `1e-7f` | Ultra-small → Clamped |
| `1e-12` | 1 | `1e-7f` | Ultra-small → Clamped |
| `1e-8` | 2 | `1e-8f` | Small epsilon → Direct |
| `0.5` | 3 | `0.5f` | Normal → Direct |
| `3.14159265...` | 3 | `3.14159265f` | Normal → Direct |
| `1e6` | 4 | `1e6f` | Large → Direct |
| `1e30` | 4 | `1e30f` | Large → Direct |
| `1e100` | 5 | `3.4028235e38f` | Out of range → Clamped! |

---

## Phase 0: Interface Refactoring (PREREQUISITE)

**Duration:** 51h (Conservative) / 29h (Aggressive)

**Goal:** Refactor core interfaces to support generic precision type parameter.

### 0.1 Refactor IScalarProcessor Interface

**File:** `GeometricAlgebraFulcrumLib.Algebra/Scalars/Generic/IScalarProcessor.cs`

**Changes:**
```csharp
public interface IScalarProcessor<T, TPrecision>
    where TPrecision : struct, INumberBase<TPrecision>
{
    TPrecision ZeroEpsilon { get; set; }

    bool IsNumeric { get; }
    bool IsSymbolic { get; }

    Scalar<T> Zero { get; }
    Scalar<T> One { get; }

    // Generic conversion instead of hard-coded Float64
    TPrecision ToPrecision(T scalar);
    T GetScalarFromPrecision(TPrecision number);

    // Type-safe random generation
    Scalar<T> ScalarFromRandom(Random gen, TPrecision min, TPrecision max);

    // ... all other methods (arithmetic, transcendental, etc.)
}
```

**Time:** 2h (Conservative) / 1h (Aggressive)

---

### 0.2 Update Core ScalarProcessor Implementations

**Files (~6 files):**
1. `ScalarProcessorOfFloat64.cs`
2. `ScalarProcessorOfFloat32.cs`
3. `ScalarProcessorOfERational.cs`
4. `ScalarProcessorOfEDecimal.cs`
5. `ScalarProcessorOfMetaExpression.cs`
6. `ScalarProcessorOfComplex.cs`

**Migration Pattern:**
```csharp
// Float64: TPrecision = double
public sealed class ScalarProcessorOfFloat64
    : INumericScalarProcessor<double, double>
{
    private double _zeroEpsilon = 1e-12;
    public double ZeroEpsilon { get => _zeroEpsilon; set => _zeroEpsilon = value; }

    public double ToPrecision(double scalar) => scalar;
    public double GetScalarFromPrecision(double number) => number;
}

// Float32: TPrecision = float
public sealed class ScalarProcessorOfFloat32
    : INumericScalarProcessor<float, float>
{
    private float _zeroEpsilon = 1e-7f;  // ✅ Correct precision for float
    public float ZeroEpsilon { get => _zeroEpsilon; set => _zeroEpsilon = value; }

    public float ToPrecision(float scalar) => scalar;
    public float GetScalarFromPrecision(float number) => number;
}

// Symbolic: TPrecision = double (evaluation precision)
public sealed class ScalarProcessorOfMetaExpression
    : ISymbolicScalarProcessor<IMetaExpressionAtomic, double>
{
    private double _zeroEpsilon = 1e-12;
    public double ZeroEpsilon { get => _zeroEpsilon; set => _zeroEpsilon = value; }

    public double ToPrecision(IMetaExpressionAtomic scalar)
        => /* evaluate to double */;
    public IMetaExpressionAtomic GetScalarFromPrecision(double number)
        => /* create expression from double */;
}
```

**Critical:** Fix ScalarProcessorOfFloat32 to use `MathF.*` instead of `Math.*`

**Time:** 10h (Conservative) / 6h (Aggressive)

---

### 0.3 Update Processor Layer (XGa, RGa)

**Files (~4 files):**
1. `XGaProcessor.cs`
2. `RGaProcessor.cs`
3. `XGaFloat64Processor.cs`
4. `RGaFloat64Processor.cs`

**Changes:**
```csharp
// Generic processors get TPrecision parameter
public sealed class XGaProcessor<T, TPrecision>
    where TPrecision : struct, INumberBase<TPrecision>
{
    public IScalarProcessor<T, TPrecision> ScalarProcessor { get; }

    // All precision-dependent operations use TPrecision
    public bool IsNearZero(T scalar)
    {
        var magnitude = ScalarProcessor.ToPrecision(scalar);
        return !TPrecision.IsNaN(magnitude) &&
               TPrecision.Abs(magnitude) < ScalarProcessor.ZeroEpsilon;
    }
}

// Float64 specialization
public sealed class XGaFloat64Processor : XGaProcessor<double, double>
{
    // Constraint automatically satisfied: double : INumberBase<double>
}

// Float32 specialization (to be generated)
public sealed class XGaFloat32Processor : XGaProcessor<float, float>
{
    // Constraint automatically satisfied: float : INumberBase<float>
}
```

**Time:** 10h (Conservative) / 6h (Aggressive)

---

### 0.4 Update Multivectors & Composers

**Files (~16 files):**
- `XGaMultivector.cs`, `XGaKVector.cs`, `XGaScalar.cs`, `XGaVector.cs`, `XGaBivector.cs`, etc.
- Composer classes

**Changes:** Propagate `TPrecision` parameter to all generic types

**Time:** 12h (Conservative) / 7h (Aggressive)

---

### 0.5 Update Generic Constraints & Dependencies

**Files (~20 files):**
- All types using `IScalarProcessor<T>` must add TPrecision parameter
- All generic constraints must be updated

**Time:** 6h (Conservative) / 3h (Aggressive)

---

### 0.6 Refactor ScalarProcessorNumberUtils.cs

**File:** `GeometricAlgebraFulcrumLib.Algebra/Scalars/Generic/ScalarProcessorNumberUtils.cs`

**Task:** Transform all 48 extension methods to use generic `TPrecision` parameter

**Example transformation (repeated 48 times):**
```csharp
// BEFORE
public static bool IsZero<T>(this IScalarProcessor<T> processor, T scalar)
{
    var number = processor.ToFloat64(scalar);
    return !double.IsNaN(number) && number.IsZero();
}

// AFTER
public static bool IsZero<T, TPrecision>(
    this IScalarProcessor<T, TPrecision> processor,
    T scalar)
    where TPrecision : struct, INumberBase<TPrecision>
{
    var number = processor.ToPrecision(scalar);
    return !TPrecision.IsNaN(number) && /* zero check */;
}
```

**Time:** 5h (Conservative) / 4h (Aggressive)

---

### 0.7 Unit Tests & Verification

**Test all refactored implementations:**
```csharp
[Test]
public void AllImplementationsCompile()
{
    Assert.DoesNotThrow(() => new ScalarProcessorOfFloat64());
    Assert.DoesNotThrow(() => new ScalarProcessorOfFloat32());
    Assert.DoesNotThrow(() => new ScalarProcessorOfERational());
}

[Test]
public void Float32ProcessorUsesCorrectEpsilon()
{
    var processor = ScalarProcessorOfFloat32.Instance;
    Assert.That(processor.ZeroEpsilon, Is.TypeOf<float>());
    Assert.That(processor.ZeroEpsilon, Is.EqualTo(1e-7f));
}
```

**Time:** 6h (Conservative) / 2h (Aggressive)

---

## Phase 1: Generator Development

**Duration:** 39h (Conservative) / 28h (Aggressive)

**Goal:** Create Roslyn-based code generator with 5-phase validation pipeline.

### 1.1 Project Structure

**New Project:** `GeometricAlgebraFulcrumLib.Float32.CodeGenerator` (Console app)

```
GA.Float32.CodeGenerator/
├── Discovery/              (Roslyn discovery logic)
├── Analysis/               (Dependency analysis)
├── Transformation/         (Syntax rewriters)
├── Validation/            (5-phase pipeline)
└── Output/                (File writing)
```

**Generated Output:**
```
Generated/
├── GA.Algebra/
│   ├── Scalars/
│   ├── GeometricAlgebra/Float32/
│   └── LinearAlgebra/Float32/
├── GA.Modeling/
│   └── Geometry/[Similar structure]
└── GA.Utilities/
    ├── Float32SparseVector.cs
    └── Float32SparseArray.cs
```

---

### 1.2 Generator Architecture - 5-Stage Pipeline

```
Stage 1: DISCOVERY
└─> Load Roslyn workspace, identify Float64 types
    Output: List<TypeToConvert>

Stage 2: DEPENDENCY ANALYSIS
└─> Build dependency graph, topological sort
    Output: Sorted list + Dependency map

Stage 3: TRANSFORMATION
└─> Apply 7 Syntax Rewriters in sequence
    Output: Transformed SyntaxTree per file

Stage 4: VALIDATION (5-Phase Pipeline)
└─> Phase 1: Syntax Validation
    Phase 2: Transformation Completeness
    Phase 3: Compilation Validation (without Float64 DLLs!)
    Phase 4: Semantic Validation
    Phase 5: Cross-Reference Validation
    Output: ValidationReport

Stage 5: OUTPUT
└─> Write validated files, generate .csproj
    Output: Complete Float32 project
```

---

### 1.3 Syntax Rewriters (7 Components)

#### 1.3.1 TypeNameRewriter
```csharp
// Transforms type identifiers
XGaFloat64Processor → XGaFloat32Processor
RGaFloat64Multivector → RGaFloat32Multivector
LinFloat64Quaternion → LinFloat32Quaternion
Float64SparseVector → Float32SparseVector
```

#### 1.3.2 TypeKeywordRewriter
```csharp
// Context-aware type keyword transformation
double → float

// Does NOT transform in:
// - String literals: "double precision"
// - Comments: // Uses double
```

#### 1.3.3 GenericParameterRewriter
```csharp
// Hybrid: Semantic analysis + String replacement

// Critical types (semantic analysis):
IScalarProcessor<double, double> → IScalarProcessor<float, float>
XGaProcessor<double, double> → XGaProcessor<float, float>

// Simple generics (string replacement):
Dictionary<int, double> → Dictionary<int, float>
Func<double, double> → Func<float, float>
```

#### 1.3.4 MethodCallRewriter
```csharp
// Math → MathF
Math.Sin(x) → MathF.Sin(x)
Math.PI → MathF.PI
Math.Tau → (2f * MathF.PI)  // MathF has no Tau!

// double methods → float methods
double.IsNaN(x) → float.IsNaN(x)
double.IsFinite(x) → float.IsFinite(x)
```

#### 1.3.5 LiteralRewriter
```csharp
// Numeric literals with semantic conversion
1d → 1f
2.5d → 2.5f
1e-12 → 1e-7f  // Category-based scaling!

// Default parameters
public static bool IsNearZero(this float value, float epsilon = 1e-7f)
```

#### 1.3.6 MethodNameRewriter
```csharp
// Float64-specific method names
ToFloat64() → ToFloat32()
FromFloat64() → FromFloat32()
Float64Value → Float32Value
```

#### 1.3.7 NamespaceRewriter
```csharp
// Namespace adjustments
.Float64 → .Float32
```

---

### 1.4 Discovery Strategy (Semantic Analysis)

**Discovery Criteria (ANY of these):**
1. Type name contains "Float64"
2. Type implements `IScalarProcessor<double, double>`
3. Type inherits from a Float64 base type
4. Type is in `.Float64` namespace
5. Type is in LinearAlgebra with "LinFloat64" prefix
6. Type is Float64SparseVector or Float64SparseArray

**Implementation:**
```csharp
var typeSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

var usesDouble = typeSymbol.Interfaces.Any(i =>
    i.Name == "IScalarProcessor" &&
    i.TypeArguments.Any(t => t.SpecialType == SpecialType.System_Double)
);
```

---

### 1.5 Validation Strategy (5-Phase Pipeline)

#### Phase 1: Syntax Validation
```csharp
var diagnostics = generatedTree.GetDiagnostics()
    .Where(d => d.Severity == DiagnosticSeverity.Error)
    .ToList();
```
**Checks:** Parse errors, invalid tokens

---

#### Phase 2: Transformation Completeness
```csharp
// Check for unconverted 'double' keywords
var doubleKeywords = generatedTree.GetRoot()
    .DescendantNodes()
    .OfType<PredefinedTypeSyntax>()
    .Where(t => t.Keyword.IsKind(SyntaxKind.DoubleKeyword))
    .ToList();

// Check for unconverted 'Float64' identifiers
var float64Identifiers = generatedTree.GetRoot()
    .DescendantTokens()
    .Where(t => t.Text.Contains("Float64"))
    .ToList();

// Check for unconverted 'Math.' references
var mathReferences = generatedTree.GetRoot()
    .DescendantNodes()
    .OfType<MemberAccessExpressionSyntax>()
    .Where(ma => ma.Expression is IdentifierNameSyntax id &&
                 id.Identifier.Text == "Math")
    .ToList();
```
**Checks:** All transformations complete, no remnants

---

#### Phase 3: Compilation Validation
```csharp
// CRITICAL: NO Float64 DLLs as references!
var compilation = CSharpCompilation.Create("Float32Validation")
    .AddReferences(
        // ONLY System DLLs, NO Float64 assemblies
        systemReferences
    );

var diagnostics = compilation.GetDiagnostics()
    .Where(d => d.Severity == DiagnosticSeverity.Error)
    .ToList();
```
**Checks:** Compiles without Float64 dependencies

---

#### Phase 4: Semantic Validation
```csharp
// Check base classes don't use Float64
if (typeSymbol.BaseType != null &&
    typeSymbol.BaseType.Name.Contains("Float64"))
{
    errors.Add("Base class still uses Float64");
}

// Check interface generic arguments
foreach (var iface in typeSymbol.Interfaces)
{
    foreach (var typeArg in iface.TypeArguments)
    {
        if (IsFloat64Type(typeArg))
        {
            errors.Add($"Interface uses Float64: {iface}");
        }
    }
}
```
**Checks:** Base classes, interfaces, member preservation

---

#### Phase 5: Cross-Reference Validation
```csharp
// Verify no references to Float64 types remain
var allTypeReferences = compilation.GetTypeByMetadataName("...");
foreach (var typeRef in allTypeReferences)
{
    if (typeRef.Name.Contains("Float64"))
    {
        errors.Add($"Float64 type reference found: {typeRef}");
    }
}
```
**Checks:** No Float64 type references in generated code

---

### 1.6 Discovery Caching (Optional)

**Conservative Caching:**
- SHA256 hash validation of all Float64 source files
- Invalidate cache if ANY file changes
- CLI flag `--no-cache` for CI/CD
- Saves ~5 minutes per iteration during development

```bash
# Development: Use cache
dotnet run --project GA.Float32.CodeGenerator

# CI/Production: Force full discovery
dotnet run --project GA.Float32.CodeGenerator --no-cache
```

---

## Phase 1A: MetaProgramming Support

**Duration:** 9h (Conservative) / 6h (Aggressive)

**Goal:** Enable MetaProgramming layer to generate Float32 code.

### 1A.1 Create MetaExpressionToCSharpFloat32Converter

**File:** `GeometricAlgebraFulcrumLib.MetaProgramming/Utilities/Code/CSharp/MetaExpressionToCSharpFloat32Converter.cs`

**Key Changes:**
```csharp
public sealed class MetaExpressionToCSharpFloat32Converter
    : MetaExpressionToLanguageConverterBase
{
    public override SteExpression Visit(IMetaExpressionFunction functionExpr)
    {
        switch (functionName)
        {
            case "Power":
                return argumentsArray[1].ToString() switch
                {
                    "0.5" => SteExpression.CreateFunction("MathF.Sqrt", argumentsArray[0]),
                    "-1" => SteExpression.CreateOperator(
                        CclCSharpUtils.Operators.Divide,
                        SteExpression.CreateLiteralNumber(1f),
                        argumentsArray[0]
                    ),
                    _ => SteExpression.CreateFunction("MathF.Pow", argumentsArray)
                };

            case "Sin":
                return SteExpression.CreateFunction("MathF.Sin", argumentsArray);

            // ... all Math.* → MathF.*
        }
    }

    public override SteExpression Visit(IMetaExpressionNumber numberExpr)
    {
        return numberExpr.HeadSpecs switch
        {
            MetaExpressionHeadSpecsNumberSymbolic symbolicHeadSpecs =>
                symbolicHeadSpecs.HeadText switch
                {
                    "Pi" => SteExpression.CreateSymbolicNumber("MathF.PI"),
                    "E" => SteExpression.CreateSymbolicNumber("MathF.E"),
                    _ => numberExpr.ToSimpleTextExpression()
                },

            MetaExpressionHeadSpecsNumberRational rationalHeadSpecs =>
                SteExpression.CreateLiteralNumber((float)rationalHeadSpecs.NumberFloat64Value),

            _ => numberExpr.ToSimpleTextExpression()
        };
    }
}
```

**Time:** 2-3h

---

### 1A.2 Add CSharpFloat32() Factory Method

**File:** `GeometricAlgebraFulcrumLib.MetaProgramming/Utilities/Code/GaFuLLanguageServerBase.cs`

```csharp
public static GaFuLCSharpServer CSharpFloat32()
{
    return new GaFuLCSharpServer(
        CclCSharpUtils.CSharp4CodeComposer("float"),
        CclCSharpUtils.CSharp4SyntaxFactory(),
        MetaExpressionToCSharpFloat32Converter.DefaultConverter
    );
}
```

**Time:** 30min

---

### 1A.3 Parameterize Base Code Generators

**Files (4 files):**
1. `CclCSharpCodeGenerator.cs`
2. `CclCppCodeGenerator.cs`
3. `CclMatlabCodeGenerator.cs`
4. `CclExcelCodeGenerator.cs`

**Change:**
```csharp
// BEFORE
internal CclCSharpCodeGenerator()
{
    ScalarTypeName = "double";  // ❌ HARDCODED
}

// AFTER
internal CclCSharpCodeGenerator(string scalarTypeName = "double")
{
    ScalarTypeName = scalarTypeName;
    ScalarZero = scalarTypeName switch
    {
        "float" => "0.0f",
        "double" => "0.0D",
        _ => "0"
    };
}
```

**Time:** 2h

---

### 1A.4 Generic Evaluation Support

Enable Float32 evaluation in MetaProgramming contexts:
```csharp
var processor = XGaProcessor<IMetaExpressionAtomic, float>
    .CreateEuclidean(context.ScalarProcessor);
```

**Time:** 2-3h

---

## Phase 2: Layer-by-Layer Conversion

**Duration:** 19h (Conservative) / 11h (Aggressive)

**Strategy:** Process layers sequentially with validation after each.

### 2.1 Algebra Layer (329 files)

**Command:**
```bash
dotnet run --project GA.Float32.CodeGenerator -- \
  --source "GeometricAlgebraFulcrumLib.Algebra" \
  --output "Generated/GA.Algebra" \
  --validate
```

**Validation:**
- Compilation test
- Smoke tests (basic operations)
- Zero errors required

**Time:** 8h (Conservative) / 5h (Aggressive)

---

### 2.2 LinearAlgebra Layer (138 files)

**Includes:**
- LinFloat64Quaternion → LinFloat32Quaternion
- LinFloat64Vector3D → LinFloat32Vector3D
- LinFloat64Bivector3D → LinFloat32Bivector3D
- LinFloat64Angle → LinFloat32Angle
- etc.

**Time:** 5h (Conservative) / 3h (Aggressive)

---

### 2.3 Modeling Layer (374 files)

**Includes:**
- CGA (Conformal Geometric Algebra)
- PGA (Projective Geometric Algebra)
- VGa (Vector Geometric Algebra)
- HGa (Hyperbolic Geometric Algebra)

**Validation:**
- Differential tests (Float64 vs Float32 tolerance checking)
- Max error < 1e-6f for critical operations

**Time:** 5h (Conservative) / 2h (Aggressive)

---

### 2.4 Utilities Layer (2 files)

**Files:**
- Float64SparseVector → Float32SparseVector
- Float64SparseArray → Float32SparseArray

**Time:** 1h (Conservative) / 1h (Aggressive)

---

## Phase 3: Testing & Validation

**Duration:** 8h (Conservative) / 4h (Aggressive)

### 3.1 Full Compilation Test

**Test:** All ~845 generated files compile without errors

**Time:** 1h

---

### 3.2 Smoke Tests

```csharp
[Test]
[Category("Smoke")]
public void AlgebraLayer_BasicOperations()
{
    var processor = XGaFloat32Processor.Euclidean;
    var v1 = processor.Vector(1f, 2f, 3f);
    var v2 = processor.Vector(4f, 5f, 6f);

    var gp = v1.Gp(v2);
    var op = v1.Op(v2);
    var sp = v1.Sp(v2);
    var norm = v1.ENorm();

    Assert.That(gp, Is.Not.Null);
    Assert.That(norm.ScalarValue, Is.GreaterThan(0));
}

[Test]
[Category("Smoke")]
public void LinearAlgebraLayer_QuaternionOperations()
{
    var q1 = LinFloat32Quaternion.Create(1f, 0f, 0f, 0f);
    var q2 = LinFloat32Quaternion.Create(0f, 1f, 0f, 0f);

    var product = q1.Multiply(q2);
    var norm = q1.Norm();

    Assert.That(norm, Is.EqualTo(1f).Within(1e-6f));
}

[Test]
[Category("Smoke")]
public void ModelingLayer_CGAOperations()
{
    var cga = CGaFloat32GeometricSpace5D.Instance;

    var point = cga.Encode.IpnsRound.Point(1f, 2f, 3f);
    var sphere = cga.Encode.IpnsRound.Sphere(0f, 0f, 0f, 1f);

    Assert.That(point, Is.Not.Null);
    Assert.That(sphere, Is.Not.Null);
}
```

**Time:** 1h

---

### 3.3 Differential Tests

```csharp
[Test]
[Category("Differential")]
public void CGA_PointEncodingMatchesWithinTolerance()
{
    var cga64 = CGaFloat64GeometricSpace5D.Instance;
    var cga32 = CGaFloat32GeometricSpace5D.Instance;

    double x = 1.234567890123456;
    double y = 2.345678901234567;
    double z = 3.456789012345678;

    var point64 = cga64.Encode.IpnsRound.Point(x, y, z);
    var point32 = cga32.Encode.IpnsRound.Point((float)x, (float)y, (float)z);

    const float tolerance = 1e-6f;
    foreach (var (id, scalar64) in point64.IdScalarPairs)
    {
        if (point32.TryGetBasisBladeScalarValue(id, out var scalar32))
        {
            Assert.That((float)scalar64, Is.EqualTo(scalar32).Within(tolerance));
        }
    }
}
```

**Time:** 2h

---

### 3.4 Performance Benchmarks

```csharp
[Test]
[Explicit]
public void Benchmark_GeometricProduct()
{
    var processor64 = XGaFloat64Processor.Euclidean;
    var processor32 = XGaFloat32Processor.Euclidean;

    var v1_64 = processor64.Vector(1.0, 2.0, 3.0);
    var v2_64 = processor64.Vector(4.0, 5.0, 6.0);

    var v1_32 = processor32.Vector(1f, 2f, 3f);
    var v2_32 = processor32.Vector(4f, 5f, 6f);

    const int iterations = 1000000;

    var sw64 = Stopwatch.StartNew();
    for (int i = 0; i < iterations; i++)
        var _ = v1_64.Gp(v2_64);
    sw64.Stop();

    var sw32 = Stopwatch.StartNew();
    for (int i = 0; i < iterations; i++)
        var _ = v1_32.Gp(v2_32);
    sw32.Stop();

    Console.WriteLine($"Float64: {sw64.ElapsedMilliseconds}ms");
    Console.WriteLine($"Float32: {sw32.ElapsedMilliseconds}ms");
    Console.WriteLine($"Speedup: {(double)sw64.ElapsedMilliseconds / sw32.ElapsedMilliseconds:F2}x");
}
```

**Time:** 2h

---

### 3.5 Regression Tests

**Run existing 1153 unit tests to ensure no regressions**

**Time:** 2h

---

## Implementation Guidelines

### Development Workflow

1. **Phase-by-Phase Execution:**
   - Complete Phase 0 entirely before starting Phase 1
   - Run tests after each sub-phase
   - Commit after each successful sub-phase

2. **Validation-Driven Development:**
   - Let validation reports guide fixes
   - All 5 validation phases must pass before proceeding

3. **Testing Frequency:**
   - Unit tests after each component
   - Integration tests after each phase
   - Acceptance criteria must be met

### Quality Gates

**Phase 0 Acceptance:**
- ✅ All implementations compile with new interface
- ✅ Float32 processor uses correct epsilon (1e-7f)
- ✅ All 48 ScalarProcessorNumberUtils methods refactored
- ✅ All unit tests pass

**Phase 1 Acceptance:**
- ✅ Generator converts simple class correctly
- ✅ LiteralRewriter handles all 5 categories
- ✅ Validation catches unconverted double keywords
- ✅ All validation phases pass

**Phase 2 Acceptance:**
- ✅ All layers compile without errors
- ✅ All smoke tests pass
- ✅ Differential tests pass (tolerance: 1e-6f)
- ✅ No Float64 references in generated code

**Phase 3 Acceptance:**
- ✅ All ~845 files compile
- ✅ All smoke tests pass
- ✅ Performance: Float32 not slower than Float64
- ✅ Max differential error < 1e-6f
- ✅ No regressions in existing 1153 tests

### Git Workflow

**Branch Strategy:**
- Create feature branch: `feature/float32-implementation`
- Commit after each sub-phase completion
- Do NOT push to main until Phase 3 complete

**Commit Messages:**
```bash
git commit -m "Phase 0.1: Refactor IScalarProcessor interface to support TPrecision parameter"
git commit -m "Phase 0.6: Refactor ScalarProcessorNumberUtils.cs (48 methods)"
git commit -m "Phase 1.3: Implement LiteralRewriter with category-based conversion"
```

### Error Handling

**Validation Failures:**
1. Review validation report
2. Identify root cause (rewriter bug, edge case, etc.)
3. Fix and re-run validation
4. Do NOT proceed until all errors resolved

**Compilation Failures:**
1. Check Phase 3 validation report
2. Ensure no Float64 DLL references
3. Fix missing using statements
4. Verify all type conversions complete

### Documentation

**Update after Phase 3:**
1. README.md - Add Float32 usage examples
2. DOCUMENTATION_INDEX.md - Add Float32 section
3. CLAUDE.md - Update architecture overview

---

## Appendix: Reference Tables

### A. File Counts by Layer

| Layer | File Count | Verified |
|-------|-----------|----------|
| Algebra | 329 | ✅ |
| Modeling | 374 | ✅ |
| LinearAlgebra | 138 | ✅ |
| Utilities.Structures | 2 | ✅ |
| **Total** | **843** | ✅ |

### B. ScalarProcessorNumberUtils Methods

**Total:** 48 extension methods

**Categories:**
- Number checks (2): IsNumber, IsFiniteNumber
- Zero checks (5): IsZero, IsOne, IsMinusOne (with overloads)
- Not-zero checks (5): IsNotZero, IsNotOne, IsNotMinusOne
- Near-zero checks (3): IsNearZero, IsNearOne, IsNearMinusOne
- Not-near-zero checks (3): IsNotNearZero, IsNotNearOne, IsNotNearMinusOne
- Sign checks (4): IsPositive, IsNegative, IsNotPositive, IsNotNegative
- Zero-or-sign checks (6): IsZeroOrPositive, IsZeroOrNegative, etc.
- Comparison (2): CompareTo, HaveOppositeSign
- Multiple checks (18): AllSameSign, AllPositive, etc. (with overloads)

### C. Timeline Summary

| Phase | Conservative | Aggressive | Recommended |
|-------|--------------|-----------|-------------|
| Phase 0 | 51h (6.5d) | 29h (3.5d) | 55h (7d) |
| Phase 1 | 39h (5d) | 28h (3.5d) | 40h (5d) |
| Phase 1A | 9h (1d) | 6h (0.75d) | 9h (1d) |
| Phase 2 | 19h (2.5d) | 11h (1.5d) | 20h (2.5d) |
| Phase 3 | 8h (1d) | 4h (0.5d) | 8h (1d) |
| **Total** | **126h ≈ 16d** | **78h ≈ 10d** | **132h ≈ 17d** |

**Planning Recommendation:** 135h (17 days) with 10% buffer

### D. .NET Requirements

**Minimum .NET Version:** .NET 7.0

**Reason:** `INumberBase<T>` interface required for generic numeric constraints

**Migration Impact:**
- All projects must target .NET 7+
- Update all .csproj files
- Verify all dependencies support .NET 7+

### E. Key Constants

| Constant | Float64 | Float32 | Notes |
|----------|---------|---------|-------|
| ZeroEpsilon | 1e-12 | 1e-7f | Float32 precision limit |
| Machine Epsilon | ~2.22e-16 | ~1.19e-7f | IEEE 754 |
| Smallest Normal | ~2.23e-308 | ~1.18e-38f | |
| Max Value | ~1.79e308 | ~3.40e38f | |

---

**Document Status:** Ready for Implementation
**Version:** 1.0 (Clean)
**Last Updated:** 2025-10-21

