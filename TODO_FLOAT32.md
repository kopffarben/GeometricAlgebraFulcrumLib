# Float32 Code Generator - Implementation Plan (FINAL)

**Goal:** Create a robust Roslyn-based code generator to automatically convert the entire Float64 implementation to Float32, with full semantic validation and no performance-degrading casts.

**Status:** ✅ All design decisions finalized, ready for implementation

**Last Updated:** 2025-10-20 (Version 2.6 - Test Parallelism: Float64 → Float32 Test Duplication)

---

## Executive Summary

### Final Scope Decision

**Total Files to Convert: ~755-805 files**

1. **Algebra Layer**: XGa/RGa Float64 → Float32 (329 files)
2. **Modeling Layer**: XGa/RGa Float64 → Float32 (374 files)
3. **LinearAlgebra**: LinFloat64* → LinFloat32* (~50-100 files)
   - LinFloat64Quaternion (heavily used: 190 references in Modeling)
   - LinFloat64Vector3D, LinFloat64Bivector3D, LinFloat64Angle, etc.
4. **Utilities.Structures**: Float64Sparse* → Float32Sparse* (2 files)
   - Float64SparseVector
   - Float64SparseArray

**New Code to Create: ~5-10 files**

5. **MetaProgramming Code Generation Support** (NEW, not conversion)
   - MetaExpressionToCSharpFloat32Converter.cs
   - MetaExpressionToCppFloat32Converter.cs (optional)
   - MetaExpressionToMatlabFloat32Converter.cs (optional)
   - GaFuLCSharpServer.CSharpFloat32() factory method

**Code to Refactor: ~4 files**

6. **Utilities.Code Parameterization** (REFACTORING, not conversion)
   - CclCSharpCodeGenerator.cs (parameterize ScalarTypeName)
   - CclCppCodeGenerator.cs (parameterize ScalarTypeName)
   - CclMatlabCodeGenerator.cs (parameterize ScalarTypeName)
   - CclExcelCodeGenerator.cs (parameterize ScalarTypeName)

### Key Findings from Analysis

1. **Interface Design Issue (CRITICAL - DECISION MADE):**
   - `IScalarProcessor<T>` has hardcoded `double ZeroEpsilon` property
   - Existing `ScalarProcessorOfFloat32` incorrectly uses `double` internally
   - **SOLUTION:** Option A - Breaking change to `IScalarProcessor<T, TPrecision>`
   - **CONSTRAINT:** `where TPrecision : struct, INumberBase<TPrecision>` (requires .NET 7+)
   - This enables clean Float32 code generation without casts
   - Consistent with Finding 1 (ScalarProcessorNumberUtils.cs) generic extension methods

2. **MetaProgramming Requirements (NEW DISCOVERY):**
   - Current: Only `MetaExpressionToCSharpFloat64Converter` exists
   - Uses `Math.*` methods and `double` scalar type
   - **REQUIRED:** Create `MetaExpressionToCSharpFloat32Converter` using `MathF.*` and `float`
   - **REQUIRED:** Parameterize base code generators to accept scalar type

3. **Quaternion Integration (CRITICAL):**
   - LinFloat64Quaternion used 190 times in Modeling layer
   - System.Numerics.Quaternion is ALREADY float-based
   - LinFloat64Quaternion with double is suboptimal (implicit casts)
   - **DECISION:** Include in Float32 conversion scope

4. **Technical Challenges:**
   - Windows path length limitations (260 chars)
   - Precision constants need semantic adjustment (1e-12 → 1e-7f)
   - Math class methods (Math.Sin → MathF.Sin)
   - Literal suffixes (1d → 1f)
   - Partial classes spread across multiple files

---

## Critical Findings (Post-Planning Discovery)

During detailed code review, **3 CRITICAL issues** were discovered that were not accounted for in the original plan (v2.0-v2.3). These findings significantly impact **Phase 0** and **Phase 1** timelines.

### Finding 1: ScalarProcessorNumberUtils.cs - Missing Scope ⚠️ CRITICAL

**File:** `GeometricAlgebraFulcrumLib.Algebra/Scalars/Generic/ScalarProcessorNumberUtils.cs`

**Problem:**
- Contains **90+ extension methods** for `IScalarProcessor<T>`
- **ALL methods** use hardcoded `scalarProcessor.ToFloat64()` and `double.*` static methods
- Examples: `IsNumber()`, `IsNearZero()`, `IsFinite()`, `IsInfinite()`, `GetScaledNumber()`, etc.
- This file was **NOT mentioned** in the original Phase 0 scope!

**Current Code (PROBLEMATIC):**
```csharp
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static bool IsNumber<T>(this IScalarProcessor<T> scalarProcessor, T scalar)
{
    var number = scalarProcessor.ToFloat64(scalar);  // ❌ Hardcoded double
    return !double.IsNaN(number);                    // ❌ double.IsNaN
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static bool IsNearZero<T>(this IScalarProcessor<T> scalarProcessor, T scalar)
{
    var number = scalarProcessor.ToFloat64(scalar);  // ❌ double
    return !double.IsNaN(number) &&
           number.IsNearZero(scalarProcessor.ZeroEpsilon);  // ❌ double ZeroEpsilon
}
```

**Solution: Option A - Generic Extension Methods with TPrecision (APPROVED)**

Refactor all 90+ methods to use generic `TPrecision` parameter with `INumberBase<TPrecision>` constraint:

```csharp
public static bool IsNumber<T, TPrecision>(
    this IScalarProcessor<T, TPrecision> scalarProcessor,
    T scalar)
    where TPrecision : struct, INumberBase<TPrecision>
{
    var number = scalarProcessor.ToPrecision(scalar);  // ✅ Generic!
    return !TPrecision.IsNaN(number);                  // ✅ TPrecision.IsNaN
}

public static bool IsNearZero<T, TPrecision>(
    this IScalarProcessor<T, TPrecision> scalarProcessor,
    T scalar)
    where TPrecision : struct, INumberBase<TPrecision>
{
    var number = scalarProcessor.ToPrecision(scalar);
    return !TPrecision.IsNaN(number) &&
           Float64Utils.IsNearZero(number, scalarProcessor.ZeroEpsilon);  // ✅ Generic
}
```

**Benefits:**
- ✅ Works for Float32, Float64, decimal, Half, and any future numeric types
- ✅ Type-safe at compile-time
- ✅ Zero runtime overhead (JIT optimizes generic constraints)
- ✅ Requires .NET 7+ for `INumberBase<T>` (acceptable)

**Impact:**
- **Phase 0 Extended:** +8-10 hours
  - Transform 90+ methods: 6-8h
  - Unit tests: 2h
  - Code review: 0.5-1h

---

### Finding 2: Default Parameter Precision Values ⚠️ MEDIUM

**Files:** `Float64Utils.cs`, `Float64ArrayUtils.cs`, and many utility classes

**Problem:**
- Many methods have **default parameters** with hardcoded precision values:

```csharp
// ❌ PROBLEMATIC
public static bool IsNearZero(this double value, double epsilon = 1e-13)
{
    return !double.IsNaN(value) && Math.Abs(value) < epsilon;
}

public static bool IsInRange(
    this double value,
    double min = 0.0,
    double max = 1.0,
    double epsilon = 1e-13)
{
    return value >= min - epsilon && value <= max + epsilon;
}
```

**Why this is a problem:**
- Naive type conversion → `epsilon = 1e-13` → `epsilon = 1e-13f` ❌
- **1e-13f is too small for float** (float epsilon: ~1e-7f)
- Compiler converts `1e-13` to float → **denormalized or 0.0f**
- Tests fail because `IsNearZero(1e-8f)` returns `false` (1e-8f < 0.0f)

**Solution: Option B - Extend LiteralRewriter with Default Parameter Handling (APPROVED)**

Add semantic default value conversion to LiteralRewriter:

```csharp
public class LiteralRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitParameter(ParameterSyntax node)
    {
        var newNode = (ParameterSyntax)base.VisitParameter(node);

        if (newNode.Default != null)
        {
            newNode = newNode.WithDefault(
                RewriteDefaultValue(newNode.Default, newNode.Type)
            );
        }

        return newNode;
    }

    private float ConvertDefaultParameterValue(double value)
    {
        double absValue = Math.Abs(value);

        // Category 1: Ultra-small epsilons (< 1e-10) → Float32 epsilon
        if (absValue > 0 && absValue < 1e-10)
            return (float)(Math.Sign(value) * 1e-7);  // Clamp to float epsilon

        // Category 2-4: Normal values → Direct conversion
        return (float)value;
    }
}
```

**Example Conversions:**
```csharp
// ✅ CORRECTED
public static bool IsNearZero(this float value, float epsilon = 1e-7f)  // ✅ Clamped!
{
    return !float.IsNaN(value) && Math.Abs(value) < epsilon;
}

public static bool IsInRange(
    this float value,
    float min = 0.0f,
    float max = 1.0f,
    float epsilon = 1e-7f)  // ✅ Clamped!
{
    return value >= min - epsilon && value <= max + epsilon;
}
```

**Impact:**
- **Phase 1 Extended:** +4.5-5.5 hours
  - LiteralRewriter extension: 2-3h
  - Unit tests: 1.5h
  - Integration testing: 0.5h
  - Code review: 0.5h

---

### Finding 3: LiteralRewriter Precision Scaling Heuristic is WRONG ⚠️ CRITICAL

**Original Plan (v2.2):**
> "For numeric literals, scale exponents appropriately (e.g., 1e-16 → 1e-11f)"

**Why this is WRONG:**

**Problem 1: Float Precision Limits**
```csharp
// ❌ NAIVE SCALING (Original Plan)
double original = 1e-16;
float scaled = 1e-11f;  // "Scale exponent by ~5"

// PROBLEM: 1e-11f is STILL too small for float!
float value = 1e-11f;
Console.WriteLine(value * value);  // 0.0f (Underflow!)
```

**IEEE 754 Single Precision Limits:**
- Practical epsilon: ~**1e-7f** (6-9 significant digits)
- Smallest normal: ~1.175494e-38f
- Machine epsilon: ~1.19209e-07f

**Problem 2: Not All Literals Are Epsilons!**
```csharp
// Float64Utils.cs contains MANY different literal types:

// Category A: Epsilons (tolerances)
const double ZeroEpsilon = 1e-13;            // → 1e-7f ✅

// Category B: Normal constants
const double Half = 0.5;                     // → 0.5f ✅ (NOT scale!)
const double Pi = 3.14159265358979323846;   // → 3.14159265f ✅

// Category C: Large numbers
const double MaxIterations = 1e6;            // → 1e6f ✅ (NOT scale!)

// If we "scale all exponents by 5":
const double Pi = 3.14159265358979323846;  // ~3.14159e0
// → 3.14159e5f = 314159.0f  ❌❌❌ COMPLETELY WRONG!
```

**Solution: Option A - Category-Based Scaling with Clamping (APPROVED)**

Classify literals by **magnitude**, apply semantic conversion rules:

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
        return (float)(sign * Float32Epsilon);  // ✅ Clamped!

    // Category 2: Small epsilons (1e-10 to 1e-6) → Direct conversion
    if (absValue >= 1e-10 && absValue < 1e-6)
        return (float)value;

    // Category 3: Normal values (1e-6 to 1e6) → Direct conversion
    if (absValue >= 1e-6 && absValue <= 1e6)
        return (float)value;

    // Category 4: Large values (1e6 to float.MaxValue) → Direct conversion
    if (absValue > 1e6 && absValue <= Float32Max)
        return (float)value;

    // Category 5: Out of range (> float.MaxValue) → Clamp
    if (absValue > Float32Max)
        return (float)(sign * Float32Max);  // ✅ Clamped!

    // Fallback
    return (float)value;
}
```

**Conversion Examples:**

| Original (Float64) | Category | Converted (Float32) | Reason |
|-------------------|----------|---------------------|--------|
| `1e-20` | 1 | `1e-7f` | Ultra-small → Clamped |
| `1e-13` | 1 | `1e-7f` | Ultra-small → Clamped |
| `1e-8` | 2 | `1e-8f` | Small epsilon → Direct |
| `0.5` | 3 | `0.5f` | Normal → Direct |
| `3.14159265...` | 3 | `3.14159265f` | Normal → Direct |
| `1e6` | 4 | `1e6f` | Large → Direct |
| `1e30` | 4 | `1e30f` | Large → Direct |
| `1e100` | 5 | `3.4028235e38f` | Out of range → Clamped! |

**Impact:**
- **Phase 1 Extended:** +6 hours
  - `ConvertLiteralToFloat32Semantic()`: 2h
  - `VisitLiteralExpression()` integration: 1h
  - Unit tests (15+ cases): 2h
  - Integration testing: 0.5h
  - Code review: 0.5h

**Note:** Finding 2 and Finding 3 **share the same conversion logic** (`ConvertLiteralToFloat32Semantic()`), so actual implementation overlap reduces total time!

---

### Combined Impact Summary

| Finding | Problem | Solution | Phase | Time Impact |
|---------|---------|----------|-------|-------------|
| **1** | ScalarProcessorNumberUtils.cs (90+ methods) not in scope | Generic extension methods with `<T, TPrecision>` | Phase 0 | +8-10h |
| **2** | Default parameter epsilon values need semantic handling | Extend LiteralRewriter with default parameter conversion | Phase 1 | +4.5-5.5h |
| **3** | Naive "scale by 5" breaks normal constants | Category-based scaling with clamping | Phase 1 | +6h |
| **Overlap** | Finding 2 & 3 share `ConvertLiteralToFloat32Semantic()` | Shared implementation | - | **-0.5h** |

**Total Time Impact:**
- **Phase 0:** 46h → **54-56h** (Conservative), 25h → **32-34h** (Aggressive)
- **Phase 1:** 28h → **38-39h** (Conservative), 18h → **27-28h** (Aggressive)
- **Total Project:** 108h → **124-127h** (Conservative), 63h → **76-79h** (Aggressive)

**New Implementation Requirements:**
- ✅ .NET 7+ required for `INumberBase<T>` interface (user approved)
  - Used in `IScalarProcessor<T, TPrecision>` constraint
  - Used in all 90+ extension methods in ScalarProcessorNumberUtils.cs
  - Required for generic numeric operations (TPrecision.IsNaN, TPrecision.Abs, etc.)
- All 90+ extension methods in ScalarProcessorNumberUtils.cs must be refactored
- LiteralRewriter must handle both default parameters AND literal expressions semantically
- Category-based conversion logic must respect IEEE 754 single precision limits

---

## Phase 0: Interface Refactoring (PREREQUISITE - BREAKING CHANGE)

### Decision: Option A - IScalarProcessor<T, TPrecision>

**Why this approach:**
- Clean architecture for MetaProgramming code generation
- No float↔double casts in generated code
- Type-safe precision handling
- Future-proof for other numeric types
- Breaking change is acceptable for this major refactoring

### 0.1 Refactor IScalarProcessor Interface

**File:** `GeometricAlgebraFulcrumLib.Algebra/Scalars/Generic/IScalarProcessor.cs`

**BEFORE:**
```csharp
public interface IScalarProcessor<T>
{
    double ZeroEpsilon { get; set; }  // ❌ Always double, even for float!

    bool IsNumeric { get; }
    bool IsSymbolic { get; }

    Scalar<T> Zero { get; }
    Scalar<T> One { get; }

    double ToFloat64(T scalar);  // ❌ Forces double conversion
    Scalar<T> ScalarFromRandom(Random randomGenerator, double minValue, double maxValue);  // ❌ double params
}
```

**AFTER (Option A - Breaking Change with INumberBase<TPrecision> Constraint):**
```csharp
using System.Numerics;  // Required for INumberBase<T>

public interface IScalarProcessor<T, TPrecision>
    where TPrecision : struct, INumberBase<TPrecision>  // ✅ Generic numeric constraint!
{
    // Precision type matches use case: float for Float32, double for Float64
    TPrecision ZeroEpsilon { get; set; }

    bool IsNumeric { get; }
    bool IsSymbolic { get; }

    Scalar<T> Zero { get; }
    Scalar<T> One { get; }

    // Generic conversion instead of hard-coded Float64
    TPrecision ToPrecision(T scalar);
    T GetScalarFromPrecision(TPrecision number);

    // Type-safe random generation
    Scalar<T> ScalarFromRandom(Random randomGenerator, TPrecision minValue, TPrecision maxValue);

    // ... all other methods (arithmetic, transcendental functions, etc.)
}

// Specialized interfaces remain generic over both T and TPrecision
public interface INumericScalarProcessor<T, TPrecision> : IScalarProcessor<T, TPrecision>
    where TPrecision : struct, INumberBase<TPrecision>
{
    // ... numeric-specific operations
}

public interface ISymbolicScalarProcessor<T, TPrecision> : IScalarProcessor<T, TPrecision>
    where TPrecision : struct, INumberBase<TPrecision>
{
    // ... symbolic-specific operations
}
```

**CRITICAL DESIGN DECISION: Why `INumberBase<TPrecision>` Constraint?**

**Analysis Result:** ✅ **YES, use `where TPrecision : struct, INumberBase<TPrecision>` on the interface!**

**Reasoning:**

1. **Type Safety:**
   - Prevents nonsensical types like `IScalarProcessor<double, string>` ❌
   - Guarantees that `ZeroEpsilon`, `ToPrecision()`, etc. work with numeric types
   - Constraint validates at compile-time, not runtime

2. **Consistency with Finding 1 (ScalarProcessorNumberUtils.cs):**
   - All 90+ extension methods in ScalarProcessorNumberUtils.cs use the same constraint
   - Interface and extension methods have matching contracts ✅
   - Example from Finding 1:
   ```csharp
   public static bool IsNearZero<T, TPrecision>(
       this IScalarProcessor<T, TPrecision> processor,
       T scalar)
       where TPrecision : struct, INumberBase<TPrecision>
   {
       var number = processor.ToPrecision(scalar);
       return !TPrecision.IsNaN(number) &&
              TPrecision.Abs(number) < processor.ZeroEpsilon;
   }
   ```

3. **Enables Generic Numeric Operations:**
   ```csharp
   // Inside interface or extension methods:
   TPrecision epsilon = processor.ZeroEpsilon;
   if (TPrecision.IsNaN(epsilon)) { ... }        // Generic static methods!
   if (TPrecision.IsFinite(epsilon)) { ... }     // Works for float, double, decimal
   TPrecision abs = TPrecision.Abs(epsilon);     // No casts needed
   ```

4. **Future-Proof:**
   - Supports `float`, `double`, `decimal`, `Half` (Float16)
   - Extensible for future .NET numeric types
   - Custom numeric types can implement `INumberBase<T>` if needed

5. **No False Restrictions:**
   - ✅ `T` remains unconstrained (can be `IMetaExpression`, `ERational`, `Complex`, etc.)
   - ✅ `TPrecision` correctly constrained (must be numeric type)
   - ❌ NOT constraining `T` - symbolic processors need non-numeric scalar types!

**Why NOT constrain `T`:**
```csharp
// ❌ WRONG - would break symbolic processors!
public interface IScalarProcessor<T, TPrecision>
    where T : struct, INumberBase<T>           // ❌ Would break IMetaExpression!
    where TPrecision : struct, INumberBase<TPrecision>
{
    // This would prevent:
    // - IScalarProcessor<IMetaExpression, double>  ❌
    // - IScalarProcessor<ERational, double>        ❌
    // - IScalarProcessor<Entity, double>           ❌ (AngouriMath)
}
```

**Verified Implementations (all work with constraint):**

| Implementation | T Type | TPrecision | Constraint Valid? |
|---------------|--------|------------|-------------------|
| ScalarProcessorOfFloat64 | `double` | `double` | ✅ double : INumberBase<double> |
| ScalarProcessorOfFloat32 | `float` | `float` | ✅ float : INumberBase<float> |
| ScalarProcessorOfERational | `ERational` | `double` | ✅ ERational (any), double : INumberBase<double> |
| ScalarProcessorOfEDecimal | `EDecimal` | `double` | ✅ EDecimal (any), double : INumberBase<double> |
| ScalarProcessorOfMetaExpression | `IMetaExpression` | `double` | ✅ IMetaExpression (any), double : INumberBase<double> |
| ScalarProcessorOfAngouriMathEntity | `Entity` | `double` | ✅ Entity (any), double : INumberBase<double> |
| ScalarProcessorOfComplex | `Complex` | `double` | ✅ Complex (any), double : INumberBase<double> |

**Key Insight:** `T` can be anything (symbolic, exact rational, etc.), but `TPrecision` MUST be a numeric type for conversions and epsilon comparisons.

**Requirements:**
- ✅ .NET 7+ for `INumberBase<T>` interface (already approved for Finding 1)
- ✅ `using System.Numerics;` in interface file
- ✅ All implementations must use numeric types for `TPrecision` (already the case)

**No Downsides:**
- All realistic `TPrecision` types already implement `INumberBase<T>`
- No existing code uses non-numeric types for precision parameter
- Constraint prevents future bugs from invalid types

**Migration pattern for all implementations:**
```csharp
// Float64: TPrecision = double
public sealed class ScalarProcessorOfFloat64
    : INumericScalarProcessor<double, double>
{
    private double _zeroEpsilon = 1e-12;
    public double ZeroEpsilon { get => _zeroEpsilon; set => _zeroEpsilon = value; }

    public double ToPrecision(double scalar) => scalar;
}

// Float32: TPrecision = float
public sealed class ScalarProcessorOfFloat32
    : INumericScalarProcessor<float, float>
{
    private float _zeroEpsilon = 1e-7f;  // ✅ Correct precision for float
    public float ZeroEpsilon { get => _zeroEpsilon; set => _zeroEpsilon = value; }

    public float ToPrecision(float scalar) => scalar;
}

// Complex: TPrecision = double (precision of Real/Imaginary parts)
public sealed class ScalarProcessorOfComplex
    : INumericScalarProcessor<Complex, double>
{
    private double _zeroEpsilon = 1e-12;  // ✅ Correct - Complex uses double internally
    public double ZeroEpsilon { get => _zeroEpsilon; set => _zeroEpsilon = value; }

    public double ToPrecision(Complex scalar) => scalar.Magnitude;
}

// Symbolic: TPrecision = double (evaluation precision for optimization)
public sealed class ScalarProcessorOfMetaExpression
    : ISymbolicScalarProcessor<IMetaExpressionAtomic, double>
{
    private double _zeroEpsilon = 1e-12;
    public double ZeroEpsilon { get => _zeroEpsilon; set => _zeroEpsilon = value; }

    public double ToPrecision(IMetaExpressionAtomic scalar)
        => /* evaluate to double */;
}
```

**CRITICAL DECISION: XGaProcessor and RGaProcessor also get TPrecision parameter!**

```csharp
// Processors also become generic over TPrecision
public sealed class XGaProcessor<T, TPrecision>
    where TPrecision : struct, INumberBase<TPrecision>  // ✅ Same constraint!
{
    public IScalarProcessor<T, TPrecision> ScalarProcessor { get; }

    // All precision-dependent operations use TPrecision
    public bool IsNearZero(T scalar)
    {
        var magnitude = ScalarProcessor.ToPrecision(scalar);

        // Can use generic numeric operations thanks to constraint:
        return !TPrecision.IsNaN(magnitude) &&
               magnitude < ScalarProcessor.ZeroEpsilon;
    }
}

// Float64 specialization
public sealed class XGaFloat64Processor : XGaProcessor<double, double>
{
    // Constraint automatically satisfied: double : INumberBase<double> ✅
}

// Float32 specialization
public sealed class XGaFloat32Processor : XGaProcessor<float, float>
{
    // Constraint automatically satisfied: float : INumberBase<float> ✅
}

// Symbolic processor with double precision
public sealed class XGaMetaExpressionProcessor : XGaProcessor<IMetaExpression, double>
{
    // IMetaExpression can be anything, but TPrecision=double satisfies constraint ✅
}
```

**Extended Impact Analysis:**
- **ALL** scalar processor implementations must be updated
- **ALL** processor classes: XGaProcessor, RGaProcessor → add TPrecision parameter
- **ALL** multivector base classes must propagate TPrecision
- **ALL** composer classes must propagate TPrecision
- **ALL** types using `IScalarProcessor<T>` must add TPrecision parameter
- **ALL** generic constraints with processors must be updated
- Estimated: **~50-75 files** need manual updates (not 20-30!)

**Testing Requirements:**
- Update all unit tests for ScalarProcessor classes
- Update all unit tests for Processors (XGa, RGa)
- Verify Float32 processor uses `float` epsilon
- Verify Float64 processor uses `double` epsilon
- Verify Complex processor uses `double` epsilon
- Verify symbolic processor uses `double` epsilon
- Smoke tests for XGaFloat64Processor, RGaFloat64Processor

### 0.2 Fix ScalarProcessorOfFloat32 Implementation

**File:** `GeometricAlgebraFulcrumLib.Algebra/Scalars/Generic/ScalarProcessorOfFloat32.cs`

**Critical bugs to fix:**
```csharp
// Line 435 - BEFORE (WRONG!)
public Scalar<float> VectorToRadians(float scalarX, float scalarY)
{
    var value = Math.Atan2(scalarY, scalarX);  // ❌ Uses Math instead of MathF
    if (value < 0) value += Math.Tau;          // ❌ Math.Tau
    return ScalarFromNumber(value);
}

// AFTER (CORRECT)
public Scalar<float> VectorToRadians(float scalarX, float scalarY)
{
    var value = MathF.Atan2(scalarY, scalarX);  // ✅ MathF
    if (value < 0) value += 2f * MathF.PI;      // ✅ MathF.PI (no Tau in MathF)
    return ScalarFromNumber(value);
}
```

**Additional fixes:**
- Replace ALL `Math.*` → `MathF.*`
- Replace ALL `double` literals → `float` literals
- Use `float` for internal precision calculations

### 0.3 Phase 0 Timeline & Breakdown

**Updated Estimate (based on ~50-75 files):**

| Sub-Phase | Task | Conservative | Aggressive |
|-----------|------|--------------|-----------|
| **0.1** | Interface Changes | 2h | 1h |
| **0.2** | Core ScalarProcessor Implementations (6 files) | 10h | 6h |
| **0.3** | Processor Layer (XGa, RGa - 4 files) | 10h | 6h |
| **0.4** | Multivectors & Composers (~16 files) | 12h | 7h |
| **0.5** | Generic Constraints & Dependencies (~20 files) | 6h | 3h |
| **0.6** | **ScalarProcessorNumberUtils.cs (Finding 1)** | **10h** | **8h** |
| **0.7** | Unit Tests & Verification | 6h | 2h |
| **Total** | | **56h ≈ 7 Arbeitstage** | **33h ≈ 4 Arbeitstage** |

**Why more than original 12h estimate?**
- Design decision: XGaProcessor<T, TPrecision> (not just IScalarProcessor)
- All multivector types must propagate TPrecision
- All composer types must propagate TPrecision
- More generic constraints to update
- ~50-75 files total (not 20-30)

**Strategy:**
1. Use C# compiler to find ALL errors after interface change
2. Systematically fix each error group
3. Run tests after each sub-phase
4. Commit after each successful sub-phase

**Risk Mitigation:**
- Compiler finds ALL issues automatically (no guessing needed)
- Breaking change is one-time only
- Tests ensure correctness at each step

---

## Phasen-Abhängigkeiten (WICHTIG!)

### Sequenzielle Ausführung Erforderlich

```
┌──────────────────────────────────────────────────────────┐
│ Phase 0: IScalarProcessor<T, TPrecision> Refactoring     │
│ (56h Conservative / 33h Aggressive)                      │
│                                                          │
│ - Interface-Änderungen                                   │
│ - ScalarProcessor Implementierungen                      │
│ - XGaProcessor/RGaProcessor → <T, TPrecision>           │
│ - Multivector & Composer Anpassungen                     │
│ - ScalarProcessorNumberUtils.cs (90+ methods) [NEW!]    │
│ - Tests                                                  │
│                                                          │
│ ✅ MUSS KOMPLETT FERTIG SEIN!                            │
└──────────────────────────────────────────────────────────┘
                            ↓
┌──────────────────────────────────────────────────────────┐
│ Phase 1: Generator Development                           │
│ (39h Conservative / 28h Aggressive)                      │
│                                                          │
│ KANN ERST NACH PHASE 0 STARTEN!                         │
│                                                          │
│ Grund: Generator muss NEUES Interface kennen:           │
│ - GenericParameterRewriter braucht <T, TPrecision>      │
│ - Discovery muss neue Signaturen erkennen               │
│ - LiteralRewriter mit Default Parameters [NEW!]         │
│ - Category-based precision scaling [NEW!]               │
└──────────────────────────────────────────────────────────┘
                            ↓
┌──────────────────────────────────────────────────────────┐
│ Phase 1A: MetaProgramming + Generic Evaluation          │
│ (9h Conservative / 6h Aggressive)                        │
│                                                          │
│ Parallel zu Phase 1 möglich (unabhängig)                │
└──────────────────────────────────────────────────────────┘
                            ↓
┌──────────────────────────────────────────────────────────┐
│ Phase 2: Layer-by-Layer Conversion                      │
│ (17h Conservative / 10h Aggressive)                      │
│                                                          │
│ NACH Phase 1 komplett fertig!                           │
│                                                          │
│ 2.1: Algebra (329 files)                                │
│ 2.2: LinearAlgebra (50-100 files)                       │
│ 2.3: Modeling (374 files)                               │
│ 2.4: Utilities (2 files)                                │
└──────────────────────────────────────────────────────────┘
                            ↓
┌──────────────────────────────────────────────────────────┐
│ Phase 3: Testing & Validation                           │
│ (8h Conservative / 4h Aggressive)                        │
└──────────────────────────────────────────────────────────┘
```

**KRITISCHER PUNKT:**
Phase 0 blockiert alles! Generator kann NICHT entwickelt werden,
bevor Phase 0 abgeschlossen ist, da GenericParameterRewriter
das neue Interface `IScalarProcessor<T, TPrecision>` verstehen muss.

**Alternative (nicht empfohlen):**
Generator-Framework parallel entwickeln, aber GenericParameterRewriter
erst nach Phase 0. Spart ~8-10h, aber komplexere Koordination.

---

## Phase 1: Generator Development

### 1.1 Project Structure

**New Solution:** `GeometricAlgebraFulcrumLib.Float32.sln` (short name for path length)

**Projects:**
```
GA.Float32.CodeGenerator/          (Console app, main generator)
├── Discovery/                     (Roslyn discovery logic)
├── Analysis/                      (Dependency analysis)
├── Transformation/                (Syntax rewriters)
├── Validation/                    (Compilation + semantic validation)
└── Output/                        (File writing)

GA.Float32.Tests/                  (Unit tests for generator)
└── TestData/                      (Small test files)
```

**Generated Output Structure:**
```
Generated/
├── GA.Algebra/                    (Short project name)
│   ├── Scalars/
│   ├── GeometricAlgebra/
│   │   └── Float32/               (Parallel to Float64)
│   │       ├── Multivectors/
│   │       ├── Processors/
│   │       └── ...
│   └── LinearAlgebra/
│       └── Float32/               (NEW - converted from Float64)
│           ├── Quaternions/
│           ├── Vectors/
│           └── ...
│
├── GA.Modeling/
│   └── Geometry/
│       └── [Similar structure]
│
└── GA.Utilities/                  (NEW - for converted utility types)
    └── Float32SparseVector.cs
    └── Float32SparseArray.cs
```

### 1.2 Generator Architecture - 5-Stage Pipeline

```
┌─────────────────────────────────────────────────────────────┐
│ Stage 1: DISCOVERY                                           │
│ - Load Roslyn workspace                                      │
│ - Parse all .cs files in target projects                    │
│ - Build semantic models                                      │
│ - Identify Float64 types via semantic analysis              │
│ Output: List<TypeToConvert> with metadata                   │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ Stage 2: DEPENDENCY ANALYSIS                                 │
│ - Build dependency graph between types                       │
│ - Topological sort for generation order                     │
│ - Identify external dependencies                            │
│ Output: Sorted list + Dependency map                        │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ Stage 3: TRANSFORMATION                                      │
│ For each type (in dependency order):                        │
│ - Clone SyntaxTree                                          │
│ - Apply Syntax Rewriters (see 1.3)                         │
│ - Preserve formatting/comments                              │
│ Output: Transformed SyntaxTree per file                     │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ Stage 4: VALIDATION                                          │
│ - Parse all generated syntax trees                          │
│ - Create CSharpCompilation                                  │
│ - Check for errors (syntax + semantic)                      │
│ - Generate validation report                                │
│ Output: ValidationReport (success/errors)                   │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ Stage 5: OUTPUT                                              │
│ - Write validated files to disk                             │
│ - Preserve directory structure (with adjustments)           │
│ - Generate .csproj files                                    │
│ Output: Complete Float32 project structure                  │
└─────────────────────────────────────────────────────────────┘
```

### 1.3 Syntax Rewriter Components

Each rewriter is a `CSharpSyntaxRewriter` subclass, applied in sequence:

**1.3.1 TypeNameRewriter**
```csharp
// Transforms type identifiers
XGaFloat64Processor → XGaFloat32Processor
RGaFloat64Multivector → RGaFloat32Multivector
LinFloat64Quaternion → LinFloat32Quaternion
LinFloat64Vector3D → LinFloat32Vector3D
Float64SparseVector → Float32SparseVector

// Handles:
- Class names
- Interface names
- Struct names
- Generic type arguments
- Base class references
```

**1.3.2 TypeKeywordRewriter**
```csharp
// Transforms type keywords
double → float

// Context-aware via SemanticModel
// Does NOT transform in:
- String literals: "double precision" (unchanged)
- Comments: // Uses double (unchanged)
- Method names: ToFloat64() (see MethodNameRewriter)
```

**1.3.3 GenericParameterRewriter (HYBRID APPROACH)**

**Strategy:** Semantische Analyse für kritische Types, String-Ersetzung für einfache Generics.

```csharp
// Kritische Types (Semantische Analyse):
IScalarProcessor<double, double> → IScalarProcessor<float, float>
INumericScalarProcessor<double, double> → INumericScalarProcessor<float, float>
XGaProcessor<double, double> → XGaProcessor<float, float>
RGaProcessor<double, double> → RGaProcessor<float, float>

// Einfache Generics (String-Ersetzung):
Dictionary<int, double> → Dictionary<int, float>
Func<double, double> → Func<float, float>
List<double> → List<float>

// Generic Constraints:
where T : IScalarProcessor<double, double> → where T : IScalarProcessor<float, float>
where T : XGaProcessor<double, double> → where T : XGaProcessor<float, float>
```

**Implementierung:**

```csharp
private TypeSyntax ConvertGenericArgument(
    TypeSyntax typeArg,
    SemanticModel semanticModel)
{
    // 1. Versuche Semantic Model (für kritische Types)
    var typeSymbol = semanticModel.GetTypeInfo(typeArg).Type;
    if (typeSymbol != null)
    {
        // Prüfe ob Float64-numerischer Typ
        if (IsCriticalProcessorType(typeSymbol))
        {
            // Semantische Analyse: Konvertiere nur wenn T=double UND TPrecision=double
            return AnalyzeAndConvert(typeSymbol);
        }
    }

    // 2. Fallback: String-basierte Ersetzung
    var typeString = typeArg.ToString();
    if (typeString == "double")
    {
        return SyntaxFactory.ParseTypeName("float");
    }

    return typeArg;  // Unverändert
}

private bool IsCriticalProcessorType(ITypeSymbol type)
{
    var typeName = type.ToDisplayString();
    return typeName.Contains("IScalarProcessor") ||
           typeName.Contains("XGaProcessor") ||
           typeName.Contains("RGaProcessor");
}
```

**Vorteile:**
- ✅ Semantische Sicherheit für kritische Types
- ✅ Einfache String-Ersetzung für unkritische Generics
- ✅ Kein Risiko bei IScalarProcessor<Complex, double> (wird erkannt als nicht-Float64)
- ✅ Wartbar - keine Whitelist nötig

**1.3.4 MethodCallRewriter**
```csharp
// Math → MathF
Math.Sin(x) → MathF.Sin(x)
Math.Cos(x) → MathF.Cos(x)
Math.PI → MathF.PI
Math.E → MathF.E
Math.Tau → (2f * MathF.PI)  // ⚠️ MathF has no Tau!

// double methods → float methods
double.IsNaN(x) → float.IsNaN(x)
double.IsFinite(x) → float.IsFinite(x)
double.PositiveInfinity → float.PositiveInfinity
```

**1.3.5 LiteralRewriter**
```csharp
// Numeric literals
1d → 1f
2.5d → 2.5f
1.0 → 1.0f (add suffix if ambiguous)
0d → 0f

// Special precision values (SEMANTIC CHANGE!)
1e-12 → 1e-7f   (epsilon for double → epsilon for float)
1e-13 → 1e-8f
1e-14 → 1e-8f
1e-15 → 1e-9f
// Heuristic: Scale exponent by ~5 for float precision
```

**1.3.6 MethodNameRewriter**
```csharp
// Float64-specific method names
ToFloat64() → ToFloat32()
FromFloat64() → FromFloat32()
GetFloat64() → GetFloat32()

// Property names
Float64Value → Float32Value
```

**1.3.7 NamespaceRewriter**
```csharp
// Namespace adjustments
.Float64 → .Float32

// Examples:
GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64
→ GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float32
```

### 1.4 Discovery Strategy (Semantic Analysis)

**Why Semantic Model over Pattern Matching:**

❌ **Pattern Matching (Fragile):**
```csharp
if (fileName.Contains("Float64"))  // Misses nested types, false positives
```

✅ **Semantic Analysis (Robust):**
```csharp
var typeSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

// Check if type actually uses double as scalar
var usesDouble = typeSymbol.Interfaces.Any(i =>
    i.Name == "IScalarProcessor" &&
    i.TypeArguments.Any(t => t.SpecialType == SpecialType.System_Double)
);

// Check inheritance from Float64 types
var inheritsFloat64 = baseType?.Name.Contains("Float64") == true;
```

**Discovery Criteria (ANY of these):**
1. Type name contains "Float64"
2. Type implements `IScalarProcessor<double, double>`
3. Type inherits from a Float64 base type
4. Type is in `.Float64` namespace
5. Type is in LinearAlgebra with "LinFloat64" prefix
6. Type is Float64SparseVector or Float64SparseArray

### 1.5 Validation Strategy (5-Phase Pipeline)

Die Validation erfolgt in **5 Phasen** direkt nach der Transformation. Jede Phase baut auf der vorherigen auf und prüft unterschiedliche Aspekte der Code-Korrektheit.

```
┌────────────────────────────────────────────────────────────┐
│ Stage 3: TRANSFORMATION (7 Rewriters)                     │
└────────────────────────────────────────────────────────────┘
                          ↓
┌────────────────────────────────────────────────────────────┐
│ Stage 4: VALIDATION (5 Phasen)                            │
├────────────────────────────────────────────────────────────┤
│ Phase 1: Syntax Validation                                │
│ Phase 2: Transformation Completeness (NEW!)               │
│ Phase 3: Compilation Validation (FIXED!)                  │
│ Phase 4: Semantic Validation (DETAILED!)                  │
│ Phase 5: Cross-Reference Validation (DETAILED!)           │
└────────────────────────────────────────────────────────────┘
                          ↓
┌────────────────────────────────────────────────────────────┐
│ Stage 5: OUTPUT                                            │
└────────────────────────────────────────────────────────────┘
```

---

#### Phase 1: Syntax Validation

**Zweck:** Prüft, ob der generierte Code syntaktisch korrekt ist.

**Implementation:**
```csharp
public class SyntaxValidator
{
    public ValidationReport Validate(SyntaxTree generatedTree)
    {
        Console.WriteLine("Phase 1: Syntax Validation...");

        var diagnostics = generatedTree.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        if (diagnostics.Any())
        {
            Console.WriteLine($"❌ Syntax errors found: {diagnostics.Count}");
            foreach (var diag in diagnostics.Take(10))
            {
                var location = diag.Location.GetLineSpan();
                Console.WriteLine($"   {location.Path}:{location.StartLinePosition.Line}");
                Console.WriteLine($"   {diag.GetMessage()}");
            }
            return ValidationReport.Failed(diagnostics);
        }

        Console.WriteLine($"✅ Syntax valid");
        return ValidationReport.Success();
    }
}
```

**Was wird geprüft:**
- Keine Syntax-Fehler (Parser-Errors)
- Alle Klammern geschlossen
- Keine ungültigen Token

---

#### Phase 2: Transformation Completeness Validation (NEW!)

**Zweck:** Prüft, ob **alle** Transformationen vollständig durchgeführt wurden. Diese Phase erkennt Rewriter-Bugs **vor** der Compilation.

**Warum wichtig:** Ein Rewriter könnte bestimmte SyntaxKinds vergessen (z.B. `double*`, `ref double`, `double?`), und dieser Bug würde erst bei Compilation auffallen. Mit Phase 2 erkennen wir solche Probleme sofort!

**Implementation:**
```csharp
public class TransformationCompletenessValidator
{
    public ValidationReport Validate(
        SyntaxTree originalTree,
        SyntaxTree generatedTree)
    {
        Console.WriteLine("Phase 2: Transformation Completeness Validation...");

        var errors = new List<ValidationError>();
        var warnings = new List<ValidationWarning>();

        // Check 1: Node Count (strukturelle Integrität)
        var originalNodeCount = originalTree.GetRoot().DescendantNodes().Count();
        var generatedNodeCount = generatedTree.GetRoot().DescendantNodes().Count();

        if (Math.Abs(originalNodeCount - generatedNodeCount) > 5)  // Toleranz: 5 Nodes
        {
            warnings.Add(new ValidationWarning
            {
                File = generatedTree.FilePath,
                Message = $"Significant node count change: {originalNodeCount} → {generatedNodeCount}",
                Severity = ValidationSeverity.Warning
            });
        }

        // Check 2: Trivia Preservation (Comments, Whitespace)
        var originalTriviaCount = originalTree.GetRoot()
            .DescendantTrivia()
            .Count(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
                        t.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
                        t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));

        var generatedTriviaCount = generatedTree.GetRoot()
            .DescendantTrivia()
            .Count(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
                        t.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
                        t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));

        if (originalTriviaCount > generatedTriviaCount)
        {
            warnings.Add(new ValidationWarning
            {
                File = generatedTree.FilePath,
                Message = $"Comments lost: {originalTriviaCount} → {generatedTriviaCount}",
                Severity = ValidationSeverity.Warning
            });
        }

        // Check 3: Unbehandelte 'double' Keywords (KRITISCH!)
        var doubleKeywords = generatedTree.GetRoot()
            .DescendantNodes()
            .OfType<PredefinedTypeSyntax>()
            .Where(t => t.Keyword.IsKind(SyntaxKind.DoubleKeyword))
            .ToList();

        foreach (var keyword in doubleKeywords)
        {
            var location = keyword.GetLocation().GetLineSpan();
            errors.Add(new ValidationError
            {
                File = generatedTree.FilePath,
                Line = location.StartLinePosition.Line,
                Column = location.StartLinePosition.Character,
                Message = $"Unconverted 'double' keyword found (Rewriter bug!)",
                CodeSnippet = keyword.Parent?.ToString(),
                Severity = ValidationSeverity.Error
            });
        }

        // Check 4: Unbehandelte 'Float64' in Identifiern (KRITISCH!)
        var float64Identifiers = generatedTree.GetRoot()
            .DescendantTokens()
            .Where(t => t.IsKind(SyntaxKind.IdentifierToken))
            .Where(t => t.Text.Contains("Float64"))
            .ToList();

        foreach (var token in float64Identifiers)
        {
            var location = token.GetLocation().GetLineSpan();
            errors.Add(new ValidationError
            {
                File = generatedTree.FilePath,
                Line = location.StartLinePosition.Line,
                Column = location.StartLinePosition.Character,
                Message = $"Identifier still contains 'Float64': {token.Text}",
                Severity = ValidationSeverity.Error
            });
        }

        // Check 5: Unbehandelte 'Math.' References
        var mathReferences = generatedTree.GetRoot()
            .DescendantNodes()
            .OfType<MemberAccessExpressionSyntax>()
            .Where(ma => ma.Expression is IdentifierNameSyntax id &&
                         id.Identifier.Text == "Math")
            .ToList();

        foreach (var mathRef in mathReferences)
        {
            var location = mathRef.GetLocation().GetLineSpan();
            warnings.Add(new ValidationWarning
            {
                File = generatedTree.FilePath,
                Line = location.StartLinePosition.Line,
                Message = $"'Math' reference found (should be 'MathF'): {mathRef}",
                Severity = ValidationSeverity.Warning
            });
        }

        // Report
        if (errors.Any())
        {
            Console.WriteLine($"❌ Transformation incomplete: {errors.Count} errors");
            foreach (var error in errors.Take(5))
            {
                Console.WriteLine($"   {error.File}:{error.Line} - {error.Message}");
            }
        }

        if (warnings.Any())
        {
            Console.WriteLine($"⚠️  {warnings.Count} warnings");
        }

        if (!errors.Any() && !warnings.Any())
        {
            Console.WriteLine($"✅ Transformation complete");
        }

        return new ValidationReport { Errors = errors, Warnings = warnings };
    }
}
```

**Was wird geprüft:**
- ✅ Keine unbehandelten `double` Keywords (inkl. Pointer, Ref, Nullable)
- ✅ Keine unbehandelten `Float64` Identifier
- ✅ Trivia (Comments) erhalten
- ✅ Node Count stabil (~gleich)
- ✅ Keine `Math.*` References (sollte `MathF.*` sein)

---

#### Phase 3: Compilation Validation (FIXED!)

**Zweck:** Prüft, ob der generierte Code kompiliert - **OHNE** Float64 DLLs als References!

**KRITISCHE ÄNDERUNG:** Die vorherige Version hätte Float64 DLLs als References hinzugefügt. Das würde **False Positives** erlauben (generierter Code könnte Float64-Types nutzen und trotzdem kompilieren).

**❌ FALSCH (alte Version):**
```csharp
var compilation = CSharpCompilation.Create("Float32Validation")
    .AddReferences(
        systemReferences,
        existingFloat64Assemblies  // ❌ GEFAHR! False Positives möglich!
    );
```

**✅ RICHTIG (neue Version):**
```csharp
var compilation = CSharpCompilation.Create("Float32Validation")
    .AddReferences(
        // NUR System-References, KEINE Float64 DLLs!
        systemReferences
    );
```

**Implementation:**
```csharp
public class CompilationValidator
{
    private static readonly string[] RequiredSystemAssemblies = new[]
    {
        typeof(object).Assembly.Location,                           // System.Private.CoreLib
        typeof(Console).Assembly.Location,                          // System.Console
        typeof(Enumerable).Assembly.Location,                       // System.Linq
        typeof(System.Collections.Generic.List<>).Assembly.Location, // System.Collections
        typeof(System.Numerics.Quaternion).Assembly.Location,       // System.Numerics
        typeof(System.Diagnostics.Debug).Assembly.Location,         // System.Diagnostics
        typeof(System.Runtime.CompilerServices.MethodImplAttribute).Assembly.Location, // System.Runtime
    };

    public ValidationReport Validate(List<SyntaxTree> generatedTrees)
    {
        Console.WriteLine("Phase 3: Compilation Validation (without Float64 references)...");

        // Create compilation with ONLY System references (NO Float64 DLLs!)
        var compilation = CSharpCompilation.Create(
            "Float32Validation",
            syntaxTrees: generatedTrees,
            references: RequiredSystemAssemblies.Select(loc =>
                MetadataReference.CreateFromFile(loc)),
            options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                allowUnsafe: true  // For pointer types
            )
        );

        var diagnostics = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        if (diagnostics.Any())
        {
            Console.WriteLine($"❌ Compilation failed: {diagnostics.Count} errors");

            // Group errors by type for better reporting
            var errorGroups = diagnostics
                .GroupBy(d => d.Id)
                .OrderByDescending(g => g.Count());

            foreach (var group in errorGroups.Take(5))
            {
                Console.WriteLine($"   {group.Key}: {group.Count()} occurrences");
                var firstError = group.First();
                var location = firstError.Location.GetLineSpan();
                Console.WriteLine($"      Example: {location.Path}:{location.StartLinePosition.Line}");
                Console.WriteLine($"      {firstError.GetMessage()}");
            }

            return ValidationReport.Failed(diagnostics);
        }

        Console.WriteLine($"✅ Compilation successful ({generatedTrees.Count} files)");
        return ValidationReport.Success();
    }
}
```

**Was wird geprüft:**
- ✅ Code kompiliert ohne Float64 DLLs
- ✅ Keine Float64-Type-References (würden zu CS0246 Errors führen)
- ✅ Alle Float32-Types korrekt definiert
- ✅ Alle System-Types korrekt verwendet

**Warum diese Änderung kritisch ist:**

Beispiel-Szenario mit BUG im Generator:
```csharp
// Generierter Code mit Bug:
public class XGaFloat32Processor
{
    private XGaFloat64Processor _processor;  // ❌ Float64-Reference!
}
```

- **Mit Float64 DLLs:** Kompiliert ✅ (False Positive!)
- **Ohne Float64 DLLs:** CS0246 Error ❌ (Korrekt erkannt!)

---

#### Phase 4: Semantic Validation (DETAILED!)

**Zweck:** Prüft die semantische Korrektheit via Roslyn SemanticModel: Base Classes, Interfaces, Generic Constraints, Member-Preservation.

**4.1 Base Class Validation**

Stellt sicher, dass Base Classes korrekt konvertiert wurden (Float32, nicht Float64).

```csharp
public class BaseClassValidator
{
    public ValidationReport Validate(
        Compilation compilation,
        IEnumerable<SyntaxTree> generatedTrees)
    {
        var errors = new List<ValidationError>();

        foreach (var tree in generatedTrees)
        {
            var semanticModel = compilation.GetSemanticModel(tree);
            var root = tree.GetRoot();

            // Find all class/struct declarations
            var typeDeclarations = root.DescendantNodes()
                .OfType<BaseTypeDeclarationSyntax>();

            foreach (var typeDecl in typeDeclarations)
            {
                var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl) as INamedTypeSymbol;
                if (typeSymbol == null) continue;

                // Check Base Type
                if (typeSymbol.BaseType != null &&
                    typeSymbol.BaseType.SpecialType != SpecialType.System_Object &&
                    typeSymbol.BaseType.SpecialType != SpecialType.System_ValueType)
                {
                    if (typeSymbol.BaseType.Name.Contains("Float64"))
                    {
                        var location = typeDecl.Identifier.GetLocation().GetLineSpan();
                        errors.Add(new ValidationError
                        {
                            File = tree.FilePath,
                            Line = location.StartLinePosition.Line,
                            TypeName = typeSymbol.Name,
                            Message = $"Base class still uses Float64: {typeSymbol.BaseType.Name}",
                            Severity = ValidationSeverity.Error
                        });
                    }
                }
            }
        }

        if (errors.Any())
        {
            Console.WriteLine($"   ❌ Base class errors: {errors.Count}");
        }

        return new ValidationReport { Errors = errors };
    }
}
```

**4.2 Interface Implementation Validation**

Prüft, dass Interface Generic Arguments korrekt konvertiert wurden.

```csharp
public class InterfaceValidator
{
    public ValidationReport Validate(
        Compilation compilation,
        IEnumerable<SyntaxTree> generatedTrees)
    {
        var errors = new List<ValidationError>();

        foreach (var tree in generatedTrees)
        {
            var semanticModel = compilation.GetSemanticModel(tree);
            var root = tree.GetRoot();

            var typeDeclarations = root.DescendantNodes()
                .OfType<BaseTypeDeclarationSyntax>();

            foreach (var typeDecl in typeDeclarations)
            {
                var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl) as INamedTypeSymbol;
                if (typeSymbol == null) continue;

                // Check all implemented interfaces
                foreach (var iface in typeSymbol.Interfaces)
                {
                    // Check Generic Arguments
                    foreach (var typeArg in iface.TypeArguments)
                    {
                        if (IsFloat64Type(typeArg))
                        {
                            var location = typeDecl.Identifier.GetLocation().GetLineSpan();
                            errors.Add(new ValidationError
                            {
                                File = tree.FilePath,
                                Line = location.StartLinePosition.Line,
                                TypeName = typeSymbol.Name,
                                Message = $"Interface uses Float64: {iface.ToDisplayString()}",
                                Severity = ValidationSeverity.Error
                            });
                        }
                    }
                }
            }
        }

        if (errors.Any())
        {
            Console.WriteLine($"   ❌ Interface errors: {errors.Count}");
        }

        return new ValidationReport { Errors = errors };
    }

    private bool IsFloat64Type(ITypeSymbol typeSymbol)
    {
        // Check 1: System.Double
        if (typeSymbol.SpecialType == SpecialType.System_Double)
            return true;

        // Check 2: Name contains "Float64"
        if (typeSymbol.Name.Contains("Float64"))
            return true;

        // Check 3: Generic Type with Float64 Arguments
        if (typeSymbol is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            return namedType.TypeArguments.Any(IsFloat64Type);
        }

        return false;
    }
}
```

**4.3 Member Preservation Validation**

Vergleicht Original- und Generated-Type: Sind alle Members vorhanden?

**Hinweis:** Dieser Check benötigt Zugriff auf die Original-Types. In der Praxis kann das optional sein (nice-to-have, kein MUST-HAVE).

```csharp
public class MemberPreservationValidator
{
    public ValidationReport Validate(
        Compilation originalCompilation,
        Compilation generatedCompilation,
        Dictionary<string, string> typeNameMapping)  // Float64TypeName → Float32TypeName
    {
        var warnings = new List<ValidationWarning>();

        foreach (var mapping in typeNameMapping)
        {
            var originalType = FindType(originalCompilation, mapping.Key);
            var generatedType = FindType(generatedCompilation, mapping.Value);

            if (originalType == null || generatedType == null)
                continue;

            var originalMembers = originalType.GetMembers()
                .Where(m => !m.IsImplicitlyDeclared)
                .ToList();

            var generatedMembers = generatedType.GetMembers()
                .Where(m => !m.IsImplicitlyDeclared)
                .ToList();

            // Check 1: Member Count
            if (originalMembers.Count != generatedMembers.Count)
            {
                warnings.Add(new ValidationWarning
                {
                    TypeName = generatedType.Name,
                    Message = $"Member count changed: {originalMembers.Count} → {generatedMembers.Count}",
                    Severity = ValidationSeverity.Warning
                });
            }

            // Check 2: Missing Members
            var originalMemberNames = originalMembers.Select(m => m.Name).ToHashSet();
            var generatedMemberNames = generatedMembers.Select(m => m.Name).ToHashSet();

            var missingMembers = originalMemberNames.Except(generatedMemberNames).ToList();

            if (missingMembers.Any())
            {
                warnings.Add(new ValidationWarning
                {
                    TypeName = generatedType.Name,
                    Message = $"Missing members: {string.Join(", ", missingMembers)}",
                    Severity = ValidationSeverity.Error  // Error, weil Members fehlen!
                });
            }
        }

        if (warnings.Any(w => w.Severity == ValidationSeverity.Error))
        {
            Console.WriteLine($"   ❌ Member preservation errors: {warnings.Count(w => w.Severity == ValidationSeverity.Error)}");
        }

        return new ValidationReport { Warnings = warnings };
    }

    private INamedTypeSymbol FindType(Compilation compilation, string fullTypeName)
    {
        return compilation.GetTypeByMetadataName(fullTypeName);
    }
}
```

**4.4 Generic Constraints Validation**

Prüft, dass Generic Constraints korrekt konvertiert wurden.

```csharp
public class GenericConstraintsValidator
{
    public ValidationReport Validate(
        Compilation compilation,
        IEnumerable<SyntaxTree> generatedTrees)
    {
        var errors = new List<ValidationError>();

        foreach (var tree in generatedTrees)
        {
            var semanticModel = compilation.GetSemanticModel(tree);
            var root = tree.GetRoot();

            var typeDeclarations = root.DescendantNodes()
                .OfType<BaseTypeDeclarationSyntax>();

            foreach (var typeDecl in typeDeclarations)
            {
                var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl) as INamedTypeSymbol;
                if (typeSymbol == null || !typeSymbol.IsGenericType)
                    continue;

                // Check each type parameter
                foreach (var typeParam in typeSymbol.TypeParameters)
                {
                    foreach (var constraint in typeParam.ConstraintTypes)
                    {
                        if (IsFloat64Type(constraint))
                        {
                            var location = typeDecl.Identifier.GetLocation().GetLineSpan();
                            errors.Add(new ValidationError
                            {
                                File = tree.FilePath,
                                Line = location.StartLinePosition.Line,
                                TypeName = typeSymbol.Name,
                                TypeParameter = typeParam.Name,
                                Message = $"Generic constraint uses Float64: where {typeParam.Name} : {constraint.ToDisplayString()}",
                                Severity = ValidationSeverity.Error
                            });
                        }
                    }
                }
            }
        }

        if (errors.Any())
        {
            Console.WriteLine($"   ❌ Generic constraint errors: {errors.Count}");
        }

        return new ValidationReport { Errors = errors };
    }

    private bool IsFloat64Type(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.SpecialType == SpecialType.System_Double)
            return true;

        if (typeSymbol.Name.Contains("Float64"))
            return true;

        if (typeSymbol is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            return namedType.TypeArguments.Any(IsFloat64Type);
        }

        return false;
    }
}
```

**Phase 4 Orchestrator:**
```csharp
public class SemanticValidator
{
    public ValidationReport Validate(
        Compilation compilation,
        IEnumerable<SyntaxTree> generatedTrees)
    {
        Console.WriteLine("Phase 4: Semantic Validation...");

        var allErrors = new List<ValidationError>();
        var allWarnings = new List<ValidationWarning>();

        // 4.1 Base Class Validation
        var baseClassReport = new BaseClassValidator().Validate(compilation, generatedTrees);
        allErrors.AddRange(baseClassReport.Errors);

        // 4.2 Interface Validation
        var interfaceReport = new InterfaceValidator().Validate(compilation, generatedTrees);
        allErrors.AddRange(interfaceReport.Errors);

        // 4.3 Generic Constraints Validation
        var constraintsReport = new GenericConstraintsValidator().Validate(compilation, generatedTrees);
        allErrors.AddRange(constraintsReport.Errors);

        // 4.4 Member Preservation (optional - needs original compilation)
        // var memberReport = new MemberPreservationValidator().Validate(...);

        if (allErrors.Any())
        {
            Console.WriteLine($"❌ Semantic validation failed: {allErrors.Count} errors");
        }
        else
        {
            Console.WriteLine($"✅ Semantic validation passed");
        }

        return new ValidationReport { Errors = allErrors, Warnings = allWarnings };
    }
}
```

**Was wird geprüft:**
- ✅ Base Classes sind Float32 (nicht Float64)
- ✅ Interface Generic Arguments sind Float32
- ✅ Generic Constraints sind Float32
- ✅ Alle Members vorhanden (optional)

---

#### Phase 5: Cross-Reference Validation (DETAILED!)

**Zweck:** Prüft, dass **KEINE** Float64-References im generierten Code verbleiben. Dies ist die letzte und umfassendste Prüfung!

**5.1 Identifier Scan für Float64-References**

```csharp
public class IdentifierScanValidator
{
    public ValidationReport Validate(
        Compilation compilation,
        IEnumerable<SyntaxTree> generatedTrees)
    {
        var errors = new List<ValidationError>();

        foreach (var tree in generatedTrees)
        {
            var semanticModel = compilation.GetSemanticModel(tree);
            var root = tree.GetRoot();

            // Scan all identifiers
            var identifiers = root.DescendantNodes()
                .OfType<IdentifierNameSyntax>();

            foreach (var identifier in identifiers)
            {
                var symbolInfo = semanticModel.GetSymbolInfo(identifier);
                var typeSymbol = symbolInfo.Symbol as ITypeSymbol;

                if (typeSymbol != null && IsFloat64Type(typeSymbol))
                {
                    var location = identifier.GetLocation().GetLineSpan();
                    errors.Add(new ValidationError
                    {
                        File = tree.FilePath,
                        Line = location.StartLinePosition.Line,
                        Message = $"Float64 reference found: {typeSymbol.ToDisplayString()}",
                        CodeSnippet = identifier.Parent?.ToString(),
                        Severity = ValidationSeverity.Error
                    });
                }
            }
        }

        return new ValidationReport { Errors = errors };
    }

    private bool IsFloat64Type(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.SpecialType == SpecialType.System_Double)
            return true;

        var fullName = typeSymbol.ToDisplayString();
        if (fullName.Contains("Float64"))
            return true;

        if (typeSymbol is INamedTypeSymbol namedType)
        {
            if (namedType.Name.Contains("ScalarProcessor") && namedType.IsGenericType)
            {
                return namedType.TypeArguments.Any(IsFloat64Type);
            }
        }

        return false;
    }
}
```

**5.2 Generic Arguments Check für 'double'**

```csharp
public class GenericArgumentsValidator
{
    public ValidationReport Validate(
        Compilation compilation,
        IEnumerable<SyntaxTree> generatedTrees)
    {
        var errors = new List<ValidationError>();

        foreach (var tree in generatedTrees)
        {
            var semanticModel = compilation.GetSemanticModel(tree);
            var root = tree.GetRoot();

            // Scan all generic names
            var genericNames = root.DescendantNodes()
                .OfType<GenericNameSyntax>();

            foreach (var genericName in genericNames)
            {
                var typeInfo = semanticModel.GetTypeInfo(genericName);
                var namedType = typeInfo.Type as INamedTypeSymbol;

                if (namedType != null)
                {
                    foreach (var typeArg in namedType.TypeArguments)
                    {
                        if (typeArg.SpecialType == SpecialType.System_Double)
                        {
                            var location = genericName.GetLocation().GetLineSpan();
                            errors.Add(new ValidationError
                            {
                                File = tree.FilePath,
                                Line = location.StartLinePosition.Line,
                                Message = $"Generic argument uses 'double': {namedType.ToDisplayString()}",
                                CodeSnippet = genericName.Parent?.ToString(),
                                Severity = ValidationSeverity.Error
                            });
                        }
                    }
                }
            }
        }

        return new ValidationReport { Errors = errors };
    }
}
```

**5.3 Math vs MathF Usage Check**

```csharp
public class MathUsageValidator
{
    public ValidationReport Validate(
        Compilation compilation,
        IEnumerable<SyntaxTree> generatedTrees)
    {
        var errors = new List<ValidationError>();

        foreach (var tree in generatedTrees)
        {
            var semanticModel = compilation.GetSemanticModel(tree);
            var root = tree.GetRoot();

            // Scan all member access
            var memberAccess = root.DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>();

            foreach (var access in memberAccess)
            {
                var symbolInfo = semanticModel.GetSymbolInfo(access);
                var memberSymbol = symbolInfo.Symbol;

                if (memberSymbol?.ContainingType != null)
                {
                    var containingTypeName = memberSymbol.ContainingType.Name;
                    var containingNamespace = memberSymbol.ContainingType.ContainingNamespace.ToDisplayString();

                    // Check: Math instead of MathF?
                    if (containingTypeName == "Math" && containingNamespace == "System")
                    {
                        var location = access.GetLocation().GetLineSpan();
                        errors.Add(new ValidationError
                        {
                            File = tree.FilePath,
                            Line = location.StartLinePosition.Line,
                            Message = $"Using Math.{memberSymbol.Name} instead of MathF.{memberSymbol.Name}",
                            CodeSnippet = access.ToString(),
                            Severity = ValidationSeverity.Error
                        });
                    }
                }
            }
        }

        return new ValidationReport { Errors = errors };
    }
}
```

**5.4 Literal Suffix Check (d → f)**

```csharp
public class LiteralSuffixValidator
{
    public ValidationReport Validate(IEnumerable<SyntaxTree> generatedTrees)
    {
        var errors = new List<ValidationError>();

        foreach (var tree in generatedTrees)
        {
            var root = tree.GetRoot();

            // Scan all numeric literals
            var literals = root.DescendantNodes()
                .OfType<LiteralExpressionSyntax>()
                .Where(lit => lit.IsKind(SyntaxKind.NumericLiteralExpression));

            foreach (var literal in literals)
            {
                var literalText = literal.Token.Text;

                // Check: Has double suffix (d/D)?
                if (literalText.EndsWith("d", StringComparison.OrdinalIgnoreCase))
                {
                    var location = literal.GetLocation().GetLineSpan();
                    errors.Add(new ValidationError
                    {
                        File = tree.FilePath,
                        Line = location.StartLinePosition.Line,
                        Message = $"Double literal found: {literalText} (should use float suffix 'f')",
                        CodeSnippet = literal.Parent?.ToString(),
                        Severity = ValidationSeverity.Error
                    });
                }
            }
        }

        return new ValidationReport { Errors = errors };
    }
}
```

**Phase 5 Orchestrator:**
```csharp
public class CrossReferenceValidator
{
    public ValidationReport Validate(
        Compilation compilation,
        IEnumerable<SyntaxTree> generatedTrees)
    {
        Console.WriteLine("Phase 5: Cross-Reference Validation...");

        var allErrors = new List<ValidationError>();

        // 5.1 Identifier Scan
        var identifierReport = new IdentifierScanValidator().Validate(compilation, generatedTrees);
        allErrors.AddRange(identifierReport.Errors);
        if (identifierReport.Errors.Any())
        {
            Console.WriteLine($"   ❌ Float64 identifiers found: {identifierReport.Errors.Count}");
        }

        // 5.2 Generic Arguments Check
        var genericReport = new GenericArgumentsValidator().Validate(compilation, generatedTrees);
        allErrors.AddRange(genericReport.Errors);
        if (genericReport.Errors.Any())
        {
            Console.WriteLine($"   ❌ Generic 'double' arguments found: {genericReport.Errors.Count}");
        }

        // 5.3 Math vs MathF Check
        var mathReport = new MathUsageValidator().Validate(compilation, generatedTrees);
        allErrors.AddRange(mathReport.Errors);
        if (mathReport.Errors.Any())
        {
            Console.WriteLine($"   ❌ Math (not MathF) usage found: {mathReport.Errors.Count}");
        }

        // 5.4 Literal Suffix Check
        var literalReport = new LiteralSuffixValidator().Validate(generatedTrees);
        allErrors.AddRange(literalReport.Errors);
        if (literalReport.Errors.Any())
        {
            Console.WriteLine($"   ❌ Double literals (d suffix) found: {literalReport.Errors.Count}");
        }

        if (allErrors.Any())
        {
            Console.WriteLine($"❌ Cross-reference validation failed: {allErrors.Count} errors");
        }
        else
        {
            Console.WriteLine($"✅ Cross-reference validation passed - NO Float64 references!");
        }

        return new ValidationReport { Errors = allErrors };
    }
}
```

**Was wird geprüft:**
- ✅ Keine Float64-Type-References in Identifiern
- ✅ Keine `double` in Generic Arguments
- ✅ Keine `Math.*` (sollte `MathF.*` sein)
- ✅ Keine double-Literals (`1.0d` → sollte `1.0f` sein)

---

#### Validation Pipeline Orchestrator

```csharp
public class ValidationPipeline
{
    public ValidationReport RunAll(
        List<(SyntaxTree Original, SyntaxTree Generated)> treePairs,
        Compilation generatedCompilation)
    {
        Console.WriteLine("\n=== VALIDATION PIPELINE (5 Phases) ===\n");

        var allErrors = new List<ValidationError>();
        var allWarnings = new List<ValidationWarning>();

        var generatedTrees = treePairs.Select(p => p.Generated).ToList();

        // Phase 1: Syntax
        foreach (var pair in treePairs)
        {
            var report = new SyntaxValidator().Validate(pair.Generated);
            allErrors.AddRange(report.Errors);
            if (report.HasErrors) return report;  // Stop on syntax errors
        }

        // Phase 2: Transformation Completeness
        foreach (var pair in treePairs)
        {
            var report = new TransformationCompletenessValidator().Validate(pair.Original, pair.Generated);
            allErrors.AddRange(report.Errors);
            allWarnings.AddRange(report.Warnings);
            if (report.HasErrors) return report;  // Stop on transformation errors
        }

        // Phase 3: Compilation
        var compilationReport = new CompilationValidator().Validate(generatedTrees);
        allErrors.AddRange(compilationReport.Errors);
        if (compilationReport.HasErrors) return compilationReport;  // Stop on compilation errors

        // Phase 4: Semantic
        var semanticReport = new SemanticValidator().Validate(generatedCompilation, generatedTrees);
        allErrors.AddRange(semanticReport.Errors);
        allWarnings.AddRange(semanticReport.Warnings);

        // Phase 5: Cross-Reference
        var crossRefReport = new CrossReferenceValidator().Validate(generatedCompilation, generatedTrees);
        allErrors.AddRange(crossRefReport.Errors);

        // Final Report
        Console.WriteLine("\n=== VALIDATION SUMMARY ===");
        Console.WriteLine($"Files validated: {treePairs.Count}");
        Console.WriteLine($"Errors: {allErrors.Count}");
        Console.WriteLine($"Warnings: {allWarnings.Count}");

        if (!allErrors.Any())
        {
            Console.WriteLine("\n✅ ALL VALIDATION PASSED - Code is ready for output!");
        }
        else
        {
            Console.WriteLine("\n❌ VALIDATION FAILED - Fix errors before proceeding!");
        }

        return new ValidationReport
        {
            Errors = allErrors,
            Warnings = allWarnings
        };
    }
}
```

---

#### Validation Report Model

```csharp
public class ValidationReport
{
    public List<ValidationError> Errors { get; set; } = new();
    public List<ValidationWarning> Warnings { get; set; } = new();

    public bool HasErrors => Errors.Any();
    public bool IsSuccess => !HasErrors;

    public static ValidationReport Success() => new ValidationReport();
    public static ValidationReport Failed(IEnumerable<Diagnostic> diagnostics)
    {
        return new ValidationReport
        {
            Errors = diagnostics.Select(d => new ValidationError
            {
                Message = d.GetMessage(),
                Severity = ValidationSeverity.Error
            }).ToList()
        };
    }
}

public class ValidationError
{
    public string File { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
    public string TypeName { get; set; }
    public string TypeParameter { get; set; }
    public string Message { get; set; }
    public string CodeSnippet { get; set; }
    public ValidationSeverity Severity { get; set; }
}

public class ValidationWarning
{
    public string File { get; set; }
    public int Line { get; set; }
    public string TypeName { get; set; }
    public string Message { get; set; }
    public ValidationSeverity Severity { get; set; }
}

public enum ValidationSeverity
{
    Warning,
    Error
}
```

---

#### Estimated Implementation Time

| Phase | Estimated Time |
|-------|----------------|
| Phase 1: Syntax Validation | 30 min (trivial) |
| Phase 2: Transformation Completeness | 3-4 hours |
| Phase 3: Compilation Validation | 1-2 hours |
| Phase 4: Semantic Validation | 4-5 hours |
| Phase 5: Cross-Reference Validation | 4-5 hours |
| Pipeline Orchestrator | 1-2 hours |
| Validation Models & Reporting | 1 hour |
| **Total** | **15-20 hours** |

Diese detaillierte Validation-Strategie garantiert, dass der generierte Float32-Code:
- ✅ Syntaktisch korrekt ist
- ✅ Vollständig transformiert wurde
- ✅ Ohne Float64-Dependencies kompiliert
- ✅ Semantisch korrekt ist
- ✅ Keine Float64-References enthält

### 1.6 Discovery Caching Strategy (Conservative)

**Zweck:** Beschleunigung von Generator-Entwicklungsiterationen während der Entwicklung.

**Problem:** Die Discovery Phase (3 Stages: String-Filter → Semantic-Verification → Dependency-Analysis) dauert ~5 Minuten pro Durchlauf. Bei iterativer Generator-Entwicklung muss Discovery nicht bei jeder Code-Änderung neu laufen.

**Lösung: Ansatz 2 - Conservative Caching**

Cache Discovery-Ergebnisse nur wenn **ALLE Float64 Source-Dateien unverändert** sind. Bei jeder Änderung → Full Discovery.

**Vorteile:**
- ✅ Sicher: Keine Inkonsistenzen, da Cache bei jeder Source-Änderung invalidiert wird
- ✅ Einfach: Keine komplexe Dependency-Analyse für Caching nötig
- ✅ Schnell: Spart ~5 Minuten pro Generator-Iteration
- ✅ Wartbar: Klare Invalidierungslogik

**Implementation:**

```csharp
public class DiscoveryCache
{
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string> FileHashes { get; set; }  // FilePath → SHA256
    public List<string> FilesToConvert { get; set; }            // Discovered files
}

public class FileDiscoveryEngine
{
    private const string CacheFileName = ".discovery-cache.json";

    public List<Document> DiscoverFilesToConvert(Solution solution, bool useCache = true)
    {
        // Try to load cache
        if (useCache && File.Exists(CacheFileName))
        {
            var cache = LoadCache(CacheFileName);

            // Validate: Are all Float64 sources unchanged?
            if (IsSourceUnchanged(cache, solution))
            {
                Console.WriteLine($"✅ Using cached discovery ({cache.FilesToConvert.Count} files)");
                Console.WriteLine($"   Cache from: {cache.Timestamp}");
                Console.WriteLine($"   Run with --no-cache to force full discovery");

                return cache.FilesToConvert
                    .Select(name => solution.Projects
                        .SelectMany(p => p.Documents)
                        .First(d => d.FilePath.EndsWith(name)))
                    .ToList();
            }
            else
            {
                Console.WriteLine("⚠️ Float64 source changed - running full discovery...");
            }
        }

        // Cache miss or invalid → Run full discovery
        Console.WriteLine("🔍 Running full discovery (3 stages)...");
        Console.WriteLine("   Stage 1: String-based filtering...");
        var candidateFiles = StageOneStringFilter(solution);  // 30s

        Console.WriteLine("   Stage 2: Semantic verification...");
        var verifiedFiles = StageTwoSemanticVerification(candidateFiles);  // 2 min

        Console.WriteLine("   Stage 3: Dependency analysis...");
        var sortedFiles = StageThreeDependencyAnalysis(verifiedFiles);  // 2.5 min

        Console.WriteLine($"✅ Discovery complete: {sortedFiles.Count} files to convert");

        // Save cache
        SaveCache(CacheFileName, new DiscoveryCache
        {
            Timestamp = DateTime.Now,
            FileHashes = ComputeAllHashes(solution),
            FilesToConvert = sortedFiles.Select(d => d.Name).ToList()
        });

        return sortedFiles;
    }

    private bool IsSourceUnchanged(DiscoveryCache cache, Solution solution)
    {
        var currentHashes = ComputeAllHashes(solution);

        // Check: Same number of files?
        if (cache.FileHashes.Count != currentHashes.Count)
            return false;

        // Check: All hashes match?
        foreach (var kvp in cache.FileHashes)
        {
            if (!currentHashes.TryGetValue(kvp.Key, out var currentHash))
                return false;  // File deleted

            if (currentHash != kvp.Value)
                return false;  // File modified
        }

        // Check: No new files?
        foreach (var path in currentHashes.Keys)
        {
            if (!cache.FileHashes.ContainsKey(path))
                return false;  // New file added
        }

        return true;  // All files unchanged
    }

    private Dictionary<string, string> ComputeAllHashes(Solution solution)
    {
        var hashes = new Dictionary<string, string>();

        foreach (var project in solution.Projects)
        {
            foreach (var document in project.Documents)
            {
                // Only hash Float64 source files
                if (document.Name.Contains("Float64") ||
                    document.Folders.Any(f => f.Contains("Float64")))
                {
                    var content = File.ReadAllText(document.FilePath);
                    var hash = ComputeSHA256(content);
                    hashes[document.FilePath] = hash;
                }
            }
        }

        return hashes;
    }

    private string ComputeSHA256(string content)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes);
    }

    private DiscoveryCache LoadCache(string fileName)
    {
        var json = File.ReadAllText(fileName);
        return System.Text.Json.JsonSerializer.Deserialize<DiscoveryCache>(json);
    }

    private void SaveCache(string fileName, DiscoveryCache cache)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(cache, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(fileName, json);
    }
}
```

**CLI Integration:**

```bash
# Development: Use cache (default)
dotnet run --project GA.Float32.CodeGenerator -- \
  --source "GeometricAlgebraFulcrumLib.Algebra" \
  --output "Generated/GA.Algebra"

# CI/Production: Force full discovery
dotnet run --project GA.Float32.CodeGenerator -- \
  --source "GeometricAlgebraFulcrumLib.Algebra" \
  --output "Generated/GA.Algebra" \
  --no-cache

# Clear cache manually
rm .discovery-cache.json
```

**Use Cases:**

1. **Generator-Entwicklung (Rewriter-Logik ändern):**
   - Float64 Source unverändert
   - Nur Generator-Code geändert
   - ✅ Cache valid → 5 Minuten gespart pro Iteration

2. **Float64 Source geändert:**
   - Neue Klasse hinzugefügt oder bestehende geändert
   - ❌ Cache invalid → Full Discovery (5 Minuten)

3. **CI/Production:**
   - Immer `--no-cache` verwenden
   - Garantiert konsistente Ergebnisse

**Risiken & Mitigation:**

| Risiko | Wahrscheinlichkeit | Mitigation |
|--------|-------------------|------------|
| Cache mit falschen Ergebnissen | Sehr gering | Validation invalidiert bei jeder Source-Änderung |
| Neue Dateien werden nicht erkannt | Keine | Hash-Vergleich prüft Anzahl der Dateien |
| Gelöschte Dateien bleiben im Cache | Keine | Hash-Vergleich prüft fehlende Dateien |
| Unterschiedliche Ergebnisse Dev vs CI | Keine | CI verwendet `--no-cache` |

**Performance:**

```
┌─────────────────────────────────────────────────┐
│ Mit Cache (Float64 Source unverändert)         │
│ - Cache laden: <1s                             │
│ - Hash-Validation: ~10s                        │
│ Total: ~10s                                    │
└─────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────┐
│ Ohne Cache (Full Discovery)                    │
│ - Stage 1 (String Filter): 30s                │
│ - Stage 2 (Semantic): 2 min                   │
│ - Stage 3 (Dependencies): 2.5 min             │
│ Total: ~5 min                                  │
└─────────────────────────────────────────────────┘

Zeitersparnis: ~4:50 min pro Generator-Iteration
```

**Estimated Implementation Time:** 2 hours

---

## Phase 1A: MetaProgramming Code Generation Support (NEW)

### Goal
Enable MetaProgramming layer to generate clean Float32 code without double↔float casts.

### 1A.1 Create MetaExpressionToCSharpFloat32Converter

**File:** `GeometricAlgebraFulcrumLib.MetaProgramming/Utilities/Code/CSharp/MetaExpressionToCSharpFloat32Converter.cs`

**Template:** Copy from `MetaExpressionToCSharpFloat64Converter.cs`

**Key Changes:**
```csharp
public sealed class MetaExpressionToCSharpFloat32Converter
    : MetaExpressionToLanguageConverterBase
{
    public static MetaExpressionToCSharpFloat32Converter DefaultConverter { get; }
        = new MetaExpressionToCSharpFloat32Converter();

    private MetaExpressionToCSharpFloat32Converter()
        : base(CclCSharpUtils.CSharp4Info)
    {
    }

    public override SteExpression Visit(IMetaExpressionFunction functionExpr)
    {
        var functionName = functionExpr.HeadSpecs.HeadText;
        var argumentsArray = functionExpr.Arguments.Select(Convert).ToArray();

        switch (functionName)
        {
            case "Power":
                return argumentsArray[1].ToString() switch
                {
                    "0.5" => SteExpression.CreateFunction("MathF.Sqrt", argumentsArray[0]),  // ✅ MathF
                    "-0.5" => SteExpression.CreateOperator(
                        CclCSharpUtils.Operators.Divide,
                        SteExpression.CreateLiteralNumber(1f),  // ✅ 1f
                        SteExpression.CreateFunction("MathF.Sqrt", argumentsArray[0])
                    ),
                    "-1" => SteExpression.CreateOperator(
                        CclCSharpUtils.Operators.Divide,
                        SteExpression.CreateLiteralNumber(1f),
                        argumentsArray[0]
                    ),
                    _ => SteExpression.CreateFunction("MathF.Pow", argumentsArray)  // ✅ MathF
                };

            case "Abs":
                return SteExpression.CreateFunction("MathF.Abs", argumentsArray);  // ✅ MathF

            case "Sin":
                return SteExpression.CreateFunction("MathF.Sin", argumentsArray);  // ✅ MathF

            // ... all other Math.* → MathF.*
        }
    }

    public override SteExpression Visit(IMetaExpressionNumber numberExpr)
    {
        return numberExpr.HeadSpecs switch
        {
            MetaExpressionHeadSpecsNumberSymbolic symbolicHeadSpecs =>
                symbolicHeadSpecs.HeadText switch
                {
                    "Pi" => SteExpression.CreateSymbolicNumber("MathF.PI"),  // ✅ MathF
                    "E" => SteExpression.CreateSymbolicNumber("MathF.E"),    // ✅ MathF
                    _ => numberExpr.ToSimpleTextExpression()
                },

            MetaExpressionHeadSpecsNumberRational rationalHeadSpecs =>
                SteExpression.CreateLiteralNumber((float)rationalHeadSpecs.NumberFloat64Value),  // ✅ Cast to float

            MetaExpressionHeadSpecsNumberFloat32 float32HeadSpecs =>
                SteExpression.CreateLiteralNumber(float32HeadSpecs.NumberFloat32Value),  // ✅ Use Float32

            _ =>
                numberExpr.ToSimpleTextExpression()
        };
    }
}
```

**Estimated Time:** 2-3 hours (mostly copy-paste with systematic replacements)

### 1A.2 Add CSharpFloat32() Factory Method

**File:** `GeometricAlgebraFulcrumLib.MetaProgramming/Utilities/Code/GaFuLLanguageServerBase.cs`

**Add new factory method:**
```csharp
public static GaFuLCSharpServer CSharpFloat32()
{
    return new GaFuLCSharpServer(
        CclCSharpUtils.CSharp4CodeComposer("float"),  // ✅ Parameterized scalar type
        CclCSharpUtils.CSharp4SyntaxFactory(),
        MetaExpressionToCSharpFloat32Converter.DefaultConverter
    );
}
```

**Estimated Time:** 30 minutes

### 1A.3 Parameterize Base Code Generators (REFACTORING)

**Files to refactor (4 files):**
1. `GeometricAlgebraFulcrumLib.Utilities.Code/Languages/CSharp/CclCSharpCodeGenerator.cs`
2. `GeometricAlgebraFulcrumLib.Utilities.Code/Languages/Cpp/CclCppCodeGenerator.cs`
3. `GeometricAlgebraFulcrumLib.Utilities.Code/Languages/Matlab/CclMatlabCodeGenerator.cs`
4. `GeometricAlgebraFulcrumLib.Utilities.Code/Languages/Excel/CclExcelCodeGenerator.cs`

**BEFORE (CclCSharpCodeGenerator.cs):**
```csharp
internal CclCSharpCodeGenerator()
{
    LanguageInfo = CclCSharpUtils.CSharp4Info;
    ScalarTypeName = "double";   // ❌ HARDCODED
    ScalarZero = "0.0D";         // ❌ HARDCODED
}
```

**AFTER (Parameterized):**
```csharp
internal CclCSharpCodeGenerator(string scalarTypeName = "double")
{
    LanguageInfo = CclCSharpUtils.CSharp4Info;

    ScalarTypeName = scalarTypeName;
    ScalarZero = scalarTypeName switch
    {
        "float" => "0.0f",
        "double" => "0.0D",
        _ => "0"
    };
}
```

**Update factory methods in CclCSharpUtils:**
```csharp
public static CclCSharpCodeGenerator CSharp4CodeComposer(string scalarTypeName = "double")
{
    return new CclCSharpCodeGenerator(scalarTypeName);
}
```

**Repeat for Cpp, Matlab, Excel generators.**

**Estimated Time:** 2 hours (4 generators + factory updates + testing)

### 1A.4 Generic Evaluation Support (NEW)

**Goal:** Enable genetic optimization to work with Float32 out-of-the-box using a generic, type-safe approach.

**Problem:** Current implementation has hardcoded `double` types:
- `MetaContextEvaluationData` stores `Dictionary<string, double>`
- `IMetaExpressionEvaluator.EvaluateToFloat64()` only supports Float64
- Genetic optimization cannot directly evaluate to Float32

**Solution:** Add generic precision type parameter (`TPrecision`) throughout the evaluation pipeline.

#### 1A.4.1 Extend IMetaExpressionEvaluator Interface

**File:** `GeometricAlgebraFulcrumLib.MetaProgramming/Context/Expressions/Evaluators/IMetaExpressionEvaluator.cs`

**Add generic evaluation methods:**
```csharp
public interface IMetaExpressionEvaluator
{
    // === EXISTING (backward compatibility) ===
    double EvaluateToFloat64(IMetaExpression expr);
    double EvaluateToFloat64(string exprText);

    // === NEW: Generic evaluation methods ===
    /// <summary>
    /// Evaluates a symbolic expression to a specific numeric precision type.
    /// </summary>
    /// <typeparam name="TPrecision">The numeric precision type (float, double, decimal, etc.)</typeparam>
    TPrecision EvaluateToPrecision<TPrecision>(IMetaExpression expr)
        where TPrecision : struct, IConvertible;

    /// <summary>
    /// Evaluates a symbolic expression string to a specific numeric precision type.
    /// </summary>
    TPrecision EvaluateToPrecision<TPrecision>(string exprText)
        where TPrecision : struct, IConvertible;
}
```

**Design Notes:**
- `where TPrecision : struct` - ensures value types only
- `IConvertible` - enables type-safe conversion between numeric types
- Existing methods preserved for backward compatibility

#### 1A.4.2 Implement Generic Evaluation in AngouriMathMetaExpressionEvaluator

**File:** `GeometricAlgebraFulcrumLib.MetaProgramming/Context/Expressions/Evaluators/AngouriMathMetaExpressionEvaluator.cs`

**Add implementation:**
```csharp
public sealed class AngouriMathMetaExpressionEvaluator : IMetaExpressionEvaluator
{
    // === EXISTING (unchanged) ===
    public double EvaluateToFloat64(IMetaExpression expr)
    {
        var expr1 = Convert(expr);
        return expr1.EvaluableNumerical
            ? (double)expr1.EvalNumerical()
            : throw new InvalidOperationException($"Expression is not numerically evaluable: {expr}");
    }

    // === NEW: Generic evaluation ===
    public TPrecision EvaluateToPrecision<TPrecision>(IMetaExpression expr)
        where TPrecision : struct, IConvertible
    {
        var expr1 = Convert(expr);

        if (!expr1.EvaluableNumerical)
            throw new InvalidOperationException($"Expression is not numerically evaluable: {expr}");

        var numericResult = expr1.EvalNumerical();

        // Type-safe conversion using IConvertible
        return (TPrecision)System.Convert.ChangeType(
            (double)numericResult,  // AngouriMath returns Complex, cast to double first
            typeof(TPrecision),
            System.Globalization.CultureInfo.InvariantCulture
        );
    }

    public TPrecision EvaluateToPrecision<TPrecision>(string exprText)
        where TPrecision : struct, IConvertible
    {
        return EvaluateToPrecision<TPrecision>(
            MetaExpressionFromTextConverter.Convert(exprText)
        );
    }
}
```

**Why this works:**
- AngouriMath evaluation is precision-agnostic (symbolic → Complex)
- `Convert.ChangeType()` handles float/double/decimal conversions type-safely
- No casts needed in user code

#### 1A.4.3 Make MetaContextEvaluationData Generic

**File:** `GeometricAlgebraFulcrumLib.MetaProgramming/Context/Evaluation/MetaContextEvaluationData.cs`

**Strategy:** Create generic version + keep non-generic for backward compatibility

```csharp
// === NEW: Generic version ===
public sealed class MetaContextEvaluationData<TPrecision> :
    IReadOnlyDictionary<string, TPrecision>
    where TPrecision : struct, IConvertible
{
    private readonly Dictionary<string, TPrecision> _variablesValues
        = new Dictionary<string, TPrecision>();

    public string EvaluationTitle { get; private set; }
    public MetaContext CodeBlock { get; }
    public int Count => _variablesValues.Count;

    public TPrecision this[string varName]
    {
        get =>
            _variablesValues.TryGetValue(varName, out var value)
                ? value
                : default;  // Generic zero (0f for float, 0d for double)
        set
        {
            if (_variablesValues.ContainsKey(varName))
                _variablesValues[varName] = value;
            else
                _variablesValues.Add(varName, value);
        }
    }

    public IEnumerable<string> Keys => _variablesValues.Keys;
    public IEnumerable<TPrecision> Values => _variablesValues.Values;

    public Dictionary<string, TPrecision> OutputVariablesValues
    {
        get
        {
            return CodeBlock.GetOutputVariables()
                .Select(item => item.InternalName)
                .ToDictionary(
                    outputVarName => outputVarName,
                    outputVarName => this[outputVarName]
                );
        }
    }

    public MetaContextEvaluationData(MetaContext codeBlock, string evalTitle)
    {
        EvaluationTitle = evalTitle;
        CodeBlock = codeBlock;
    }

    // IReadOnlyDictionary implementation...
    public bool ContainsKey(string varName) => _variablesValues.ContainsKey(varName);
    public bool TryGetValue(string varName, out TPrecision value) => _variablesValues.TryGetValue(varName, out value);
    public IEnumerator<KeyValuePair<string, TPrecision>> GetEnumerator() => _variablesValues.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

// === KEEP: Non-generic version (backward compatibility via inheritance) ===
public sealed class MetaContextEvaluationData : MetaContextEvaluationData<double>
{
    public MetaContextEvaluationData(MetaContext codeBlock, string evalTitle)
        : base(codeBlock, evalTitle)
    {
    }
}
```

**Design rationale:**
- Generic `MetaContextEvaluationData<TPrecision>` for all numeric types
- Non-generic version inherits from `<double>` - **ZERO breaking changes!**
- `default` keyword provides type-safe zero value

#### 1A.4.4 Make McOptEvaluateCodeBlock Generic

**File:** `GeometricAlgebraFulcrumLib.MetaProgramming/Context/Optimizer/McOptEvaluateCodeBlock.cs`

**Refactor to generic processor:**
```csharp
internal sealed class McOptEvaluateCodeBlock<TPrecision> :
    MetaContextProcessorBase
    where TPrecision : struct, IConvertible
{
    // === EXISTING: Backward-compatible factory (Float64) ===
    internal static void Process(MetaContext context, MetaContextEvaluationData evaluationData)
    {
        Process<double>(context, (MetaContextEvaluationData<double>)evaluationData);
    }

    // === NEW: Generic factory method ===
    internal static void Process<TPrecision>(
        MetaContext context,
        MetaContextEvaluationData<TPrecision> evaluationData)
        where TPrecision : struct, IConvertible
    {
        var processor = new McOptEvaluateCodeBlock<TPrecision>(context, evaluationData);
        processor.BeginProcessing();
    }

    private readonly MetaContextEvaluationData<TPrecision> _evaluationData;

    private McOptEvaluateCodeBlock(
        MetaContext context,
        MetaContextEvaluationData<TPrecision> evaluationData)
        : base(context)
    {
        _evaluationData = evaluationData;
    }

    private string ExpressionToString(IMetaExpression expr)
    {
        // ... unchanged ...
        if (expr.IsAtomic)
            return expr.IsVariable
                ? _evaluationData[expr.HeadText].ToString("G")  // ✅ Generic ToString()
                : expr.HeadText;

        // ... rest unchanged ...
    }

    protected override void BeginProcessing()
    {
        foreach (var computedVar in Context.GetComputedVariables())
        {
            // ✅ Type-safe generic evaluation!
            _evaluationData[computedVar.InternalName] =
                Context.SymbolicEvaluator.EvaluateToPrecision<TPrecision>(
                    ExpressionToString(computedVar.RhsExpression)
                );
        }
    }
}
```

**Key improvements:**
- Fully generic with `TPrecision` parameter
- Backward-compatible factory for existing Float64 code
- Uses new `EvaluateToPrecision<TPrecision>()` method
- Type-safe, no casts!

#### 1A.4.5 Update MetaContextEvaluationDataHistory (Generic)

**File:** `GeometricAlgebraFulcrumLib.MetaProgramming/Context/Evaluation/MetaContextEvaluationDataHistory.cs`

**Add generic version:**
```csharp
// === NEW: Generic version ===
internal sealed class MetaContextEvaluationDataHistory<TPrecision>
    where TPrecision : struct, IConvertible
{
    private readonly List<MetaContextEvaluationData<TPrecision>> _evaluations
        = new List<MetaContextEvaluationData<TPrecision>>();

    public MetaContext Context { get; }
    public int Count => _evaluations.Count;
    public MetaContextEvaluationData<TPrecision> this[int index] => _evaluations[index];

    internal MetaContextEvaluationDataHistory(
        MetaContext context,
        TPrecision minValue,
        TPrecision maxValue)
    {
        Context = context;
        // Initialize parameter variables with random test values in [minValue, maxValue]
        // ... implementation ...
    }

    public MetaContextEvaluationData<TPrecision> AddEvaluation(string evalTitle)
    {
        var evaluationData = new MetaContextEvaluationData<TPrecision>(Context, evalTitle);

        // Process evaluation using generic optimizer
        McOptEvaluateCodeBlock<TPrecision>.Process(Context, evaluationData);

        _evaluations.Add(evaluationData);
        return evaluationData;
    }
}

// === KEEP: Non-generic version (backward compatibility) ===
internal sealed class MetaContextEvaluationDataHistory
    : MetaContextEvaluationDataHistory<double>
{
    internal MetaContextEvaluationDataHistory(MetaContext context, double minValue, double maxValue)
        : base(context, minValue, maxValue)
    {
    }
}
```

#### 1A.4.6 Update MetaContextOptimizer (Optional)

**File:** `GeometricAlgebraFulcrumLib.MetaProgramming/Context/Optimizer/MetaContextOptimizer.cs`

**Add generic support if needed:**
```csharp
public class MetaContextOptimizer
{
    // Existing property (backward compatibility)
    public MetaContextEvaluationDataHistory EvaluationDataHistory { get; private set; }

    // NEW: Generic initialization
    public void InitializeEvaluationHistory<TPrecision>(TPrecision minValue, TPrecision maxValue)
        where TPrecision : struct, IConvertible
    {
        EvaluationDataHistory = new MetaContextEvaluationDataHistory<TPrecision>(
            Context,
            minValue,
            maxValue
        );
    }
}
```

#### Usage Examples

**Example 1: Float64 Genetic Optimization (Existing - Unchanged!)**
```csharp
var context = new MetaContext();
var processor = XGaProcessor<IMetaExpressionAtomic>.CreateEuclidean(context);

// ... define computations ...

// ✅ Existing code works without changes!
var history = new MetaContextEvaluationDataHistory(context, -5.0, 5.0);
var evaluation = history.AddEvaluation("test1");  // Uses double internally
```

**Example 2: Float32 Genetic Optimization (NEW!)**
```csharp
var context = new MetaContext();
var processor = XGaProcessor<IMetaExpressionAtomic>.CreateEuclidean(context);

// ... define computations ...

// ✅ Float32 optimization with explicit type parameter
var history = new MetaContextEvaluationDataHistory<float>(context, -5.0f, 5.0f);
var evaluation = history.AddEvaluation("test1");  // Uses float internally

// Access Float32 results type-safely
float result = evaluation["outputVar"];  // No cast needed!
```

**Example 3: Direct Generic Evaluation**
```csharp
var context = new MetaContext();
var evaluator = context.SymbolicEvaluator;

// Symbolic expression
var expr = context.Add(context.One, context.Pi);

// Evaluate to different precisions
float resultFloat32 = evaluator.EvaluateToPrecision<float>(expr);
double resultFloat64 = evaluator.EvaluateToPrecision<double>(expr);
decimal resultDecimal = evaluator.EvaluateToPrecision<decimal>(expr);

// Or use existing method (backward compatible)
double legacyResult = evaluator.EvaluateToFloat64(expr);
```

**Benefits:**
- ✅ **Type-safe**: No casts, compile-time type checking
- ✅ **Generic**: Works with float, double, decimal, or any `IConvertible` struct
- ✅ **Zero breaking changes**: Existing code continues to work via inheritance
- ✅ **Out-of-the-box Float32**: Just use `<float>` type parameter
- ✅ **Clean architecture**: Single implementation for all numeric types
- ✅ **Consistent**: Same pattern across evaluation pipeline

**AngouriMath Compatibility:**
- ✅ Simplification is precision-agnostic (symbolic operations)
- ✅ Float32HeadSpecs already supported in converters
- ✅ No changes needed to core AngouriMath integration
- ✅ Only evaluation layer needs generic support

**Files to modify (6 files):**
1. `IMetaExpressionEvaluator.cs` - Add generic evaluation methods
2. `AngouriMathMetaExpressionEvaluator.cs` - Implement generic evaluation
3. `MetaContextEvaluationData.cs` - Make generic, keep non-generic for compatibility
4. `McOptEvaluateCodeBlock.cs` - Make generic evaluation processor
5. `MetaContextEvaluationDataHistory.cs` - Add generic version
6. `MetaContextOptimizer.cs` - Support generic precision type parameter (optional)

**Breaking changes:** NONE (inheritance-based backward compatibility)

**Estimated Time:** 4-5 hours (6 files + testing + documentation)

---

## Phase 2: Code Conversion - Layer by Layer

### Phase 2.1: Algebra Layer (329 files)

**Input:** `GeometricAlgebraFulcrumLib.Algebra` project

**Includes:**
- XGa/RGa Float64 processors and multivectors
- IScalarProcessor implementations

**Command:**
```bash
dotnet run --project GA.Float32.CodeGenerator -- \
  --source "GeometricAlgebraFulcrumLib.Algebra" \
  --output "Generated/GA.Algebra" \
  --validate \
  --report "algebra-generation-report.json"
```

**Expected:** 329 converted files, 0 errors

### Phase 2.2: LinearAlgebra Layer (50-100 files)

**Input:** `GeometricAlgebraFulcrumLib.Algebra/LinearAlgebra/Float64`

**Includes:**
- LinFloat64Quaternion (CRITICAL - 190 usages)
- LinFloat64Vector3D, LinFloat64Bivector3D
- LinFloat64Angle, LinFloat64PlanarAngle
- All dependent types

**Command:**
```bash
dotnet run --project GA.Float32.CodeGenerator -- \
  --source "GeometricAlgebraFulcrumLib.Algebra/LinearAlgebra/Float64" \
  --output "Generated/GA.Algebra/LinearAlgebra/Float32" \
  --validate \
  --report "linearalgebra-generation-report.json"
```

**Expected:** 50-100 converted files, 0 errors

### Phase 2.3: Modeling Layer (374 files)

**Input:** `GeometricAlgebraFulcrumLib.Modeling` project

**Dependencies:** Generated GA.Algebra + GA.LinearAlgebra from Phase 2.1/2.2

**Command:**
```bash
dotnet run --project GA.Float32.CodeGenerator -- \
  --source "GeometricAlgebraFulcrumLib.Modeling" \
  --output "Generated/GA.Modeling" \
  --reference "Generated/GA.Algebra/GA.Algebra.csproj" \
  --validate \
  --report "modeling-generation-report.json"
```

**Expected:** 374 converted files, 0 errors

### Phase 2.4: Utilities.Structures (2 files)

**Input:** Float64SparseVector.cs, Float64SparseArray.cs

**Trivial conversion:**
- `Dictionary<int, double>` → `Dictionary<int, float>`
- `IReadOnlyList<double>` → `IReadOnlyList<float>`
- `0d` → `0f`

**Command:**
```bash
dotnet run --project GA.Float32.CodeGenerator -- \
  --source "GeometricAlgebraFulcrumLib.Utilities.Structures/Dictionary/Float64Sparse*.cs" \
  --output "Generated/GA.Utilities" \
  --validate \
  --report "utilities-generation-report.json"
```

**Expected:** 2 converted files, 0 errors

---

## Critical Edge Cases & Solutions

### Edge Case 1: Partial Classes
**Problem:** Types like `XGaFloat64Multivector` span multiple files.

**Solution:** Discovery phase groups partial classes by full type name, transforms all parts together with consistency checks.

### Edge Case 2: Math.Tau Missing in MathF
**Problem:** `MathF` doesn't have `Tau` constant

**Solution:** `Math.Tau` → `(2f * MathF.PI)`

### Edge Case 3: Generic Constraints with IScalarProcessor
**Problem:**
```csharp
where T : IScalarProcessor<double>
```

**Solution (with Option A):**
```csharp
where T : IScalarProcessor<double> → where T : IScalarProcessor<float, float>
```

### Edge Case 4: Precision Constants
**Problem:** `1e-12` is too precise for float

**Solution:** Heuristic scaling:
- `1e-12` → `1e-7f` (scale exponent by ~5)
- `1e-13` → `1e-8f`
- `1e-14` → `1e-8f`

### Edge Case 5: ToFloat64() Method Names
**Problem:** Method names contain type names

**Solution:** Semantic analysis ensures we only rename type-related methods:
- `ToFloat64()` → `ToFloat32()`
- `ToFloat64Precision()` → unchanged (not a type conversion method)

### Verifikation: Extension Methods

**Extension Methods sind durch existierende Rewriters abgedeckt!**

Extension Methods sind syntaktisch normale Methods mit `this` Parameter.
Alle relevanten Aspekte werden automatisch behandelt:

```csharp
// Float64 Extension Method
public static class XGaFloat64ProcessorUtils
{
    public static XGaFloat64Scalar ScalarFromValue(
        this XGaFloat64Processor processor,
        double value)
    {
        return processor.Scalar(value);
    }
}

// Wird automatisch konvertiert zu Float32:
public static class XGaFloat32ProcessorUtils
{
    public static XGaFloat32Scalar ScalarFromValue(
        this XGaFloat32Processor processor,
        float value)
    {
        return processor.Scalar(value);
    }
}
```

**Behandelt durch:**
- **TypeNameRewriter:** `XGaFloat64ProcessorUtils` → `XGaFloat32ProcessorUtils`
- **TypeKeywordRewriter:** `double value` → `float value`
- **GenericParameterRewriter:** Generic Parameters in Extension Methods
- **MethodCallRewriter:** `Math.*` → `MathF.*` in Method Bodies

**Kein separater Edge Case oder Rewriter nötig.** ✅

---

## Testing Strategy

### Level 1: Compilation Testing (MUST HAVE)
- All generated code must compile without errors
- All Float64 references must be removed
- All Float32 types must resolve

### Level 2: Smoke Testing (REQUIRED)
```csharp
// Algebra smoke test
var processor = XGaFloat32Processor.Euclidean;
var v1 = processor.Vector(1f, 2f, 3f);
var v2 = processor.Vector(4f, 5f, 6f);
var result = v1.Gp(v2);
Assert.IsNotNull(result);

// Quaternion smoke test
var q1 = LinFloat32Quaternion.Create(1f, 2f, 3f, 4f);
var q2 = LinFloat32Quaternion.Create(5f, 6f, 7f, 8f);
var result = q1.Multiply(q2);
Assert.IsNotNull(result);

// CGA smoke test
var cga = CGaFloat32GeometricSpace5D.Instance;
var point = cga.Encode.IpnsRound.Point(1f, 2f, 3f);
Assert.IsNotNull(point);
```

### Level 3: Differential Testing (NICE TO HAVE)
- Run same operations on Float64 and Float32
- Verify results within tolerance (1e-6)
- Proves semantic correctness

---

## Implementation Checklist

### Phase 0: Interface Refactoring (PREREQUISITE)
- [ ] Refactor `IScalarProcessor<T>` → `IScalarProcessor<T, TPrecision>`
- [ ] Update ALL scalar processor implementations (~20 files)
  - [ ] ScalarProcessorOfFloat32 (fix bugs!)
  - [ ] ScalarProcessorOfFloat64
  - [ ] ScalarProcessorOfComplex
  - [ ] ScalarProcessorOfMetaExpression
  - [ ] ScalarProcessorOfERational
  - [ ] ScalarProcessorOfEDecimal
- [ ] Update ALL types using IScalarProcessor
  - [ ] XGaProcessor<T> → XGaProcessor<T, TPrecision>
  - [ ] RGaProcessor<T> → RGaProcessor<T, TPrecision>
  - [ ] MetaContext
  - [ ] All multivector types
- [ ] Update ALL unit tests
- [ ] Commit: "Phase 0: IScalarProcessor<T, TPrecision> refactoring"

### Phase 1: Generator Development
- [ ] Create `GA.Float32.CodeGenerator` solution
- [ ] Implement Discovery stage
- [ ] Implement Dependency Analysis
- [ ] Implement Transformation stage (7 rewriters)
  - [ ] TypeNameRewriter
  - [ ] TypeKeywordRewriter
  - [ ] GenericParameterRewriter
  - [ ] MethodCallRewriter
  - [ ] LiteralRewriter
  - [ ] MethodNameRewriter
  - [ ] NamespaceRewriter
- [ ] Implement Validation stage (5 phases)
- [ ] Implement Output stage
- [ ] Unit tests for each component
- [ ] Integration test on mini project (5-10 files)

### Phase 1A: MetaProgramming Code Generation (NEW)
- [ ] Create `MetaExpressionToCSharpFloat32Converter.cs`
- [ ] Add `GaFuLCSharpServer.CSharpFloat32()` factory
- [ ] Parameterize base code generators (4 files):
  - [ ] CclCSharpCodeGenerator.cs
  - [ ] CclCppCodeGenerator.cs (optional)
  - [ ] CclMatlabCodeGenerator.cs (optional)
  - [ ] CclExcelCodeGenerator.cs (optional)
- [ ] Test Float32 code generation from MetaProgramming
- [ ] Commit: "Phase 1A: Float32 code generation support"

### Phase 2: Layer-by-Layer Conversion
- [ ] **2.1 Algebra Layer** (329 files)
  - [ ] Run generator
  - [ ] Review validation report
  - [ ] Fix any issues
  - [ ] Commit generated code
  - [ ] Compile and test

- [ ] **2.2 LinearAlgebra Layer** (50-100 files)
  - [ ] Run generator
  - [ ] Review validation report
  - [ ] Fix any issues
  - [ ] Verify LinFloat32Quaternion correct
  - [ ] Commit generated code
  - [ ] Compile and test

- [ ] **2.3 Modeling Layer** (374 files)
  - [ ] Run generator (with Algebra + LinearAlgebra refs)
  - [ ] Review validation report
  - [ ] Fix any issues
  - [ ] Commit generated code
  - [ ] Compile and test

- [ ] **2.4 Utilities.Structures** (2 files)
  - [ ] Run generator
  - [ ] Review (trivial conversion)
  - [ ] Commit

### Phase 3: Testing & Validation
- [ ] Compile all generated projects
- [ ] Run smoke tests (Algebra, LinearAlgebra, Modeling)
- [ ] Performance benchmarks (Float32 vs Float64)
- [ ] Optional: Differential testing
- [ ] Documentation updates

---

## Estimated Timelines (UPDATED v2.3)

### Conservative Estimate (with thorough testing)

| Phase | Task | Estimated Time |
|-------|------|----------------|
| **0** | **IScalarProcessor<T, TPrecision> refactoring (~50-75 files)** | **46 hours** |
| **1** | **Generator development (inkl. Hybrid-Rewriter + 5-Phase Validation)** | **30 hours** |
| **1A** | **MetaProgramming + Generic Evaluation Support** | **9 hours** |
| **2.1** | Algebra generation + fixes | 6 hours |
| **2.2** | LinearAlgebra generation + fixes | 4 hours |
| **2.3** | Modeling generation + fixes | 6 hours |
| **2.4** | Utilities generation | 1 hour |
| **3** | Testing & validation | 8 hours |
| **Total** | | **110 hours ≈ 13-14 Arbeitstage** |

**Änderungen gegenüber v2.2:**
- Phase 1: 28h → **30h** (+2h für erweiterte 5-Phase Validation)
- Total: 108h → **110h**

**Änderungen gegenüber v2.0:**
- Phase 0: 12h → **46h** (XGaProcessor<T, TPrecision> + mehr Dateien)
- Phase 1: 24h → **30h** (Hybrid GenericParameterRewriter + 5-Phase Validation)
- Phase 1A: 4h → **9h** (Generic Evaluation umfangreicher)

### Aggressive Estimate (minimal testing)

| Phase | Task | Estimated Time |
|-------|------|----------------|
| **0** | **IScalarProcessor<T, TPrecision> refactoring** | **25 hours** |
| **1** | **Generator (Hybrid-Rewriter + 5-Phase Validation)** | **20 hours** |
| **1A** | **MetaProgramming + Generic Evaluation** | **6 hours** |
| **2** | All conversions | 10 hours |
| **3** | Basic testing | 4 hours |
| **Total** | | **65 hours ≈ 8 Arbeitstage** |

**Änderungen gegenüber v2.2:**
- Phase 1: 18h → **20h** (+2h für erweiterte Validation)
- Total: 63h → **65h**

**Änderungen gegenüber v2.0:**
- Phase 0: 8h → **25h** (größerer Scope)
- Phase 1: 16h → **20h** (Semantic Analysis + 5-Phase Validation)
- Phase 1A: 2h → **6h** (Generic Evaluation)

---

## Comprehensive Testing Strategy

### Test Philosophy

**Principle:** "Test Early, Test Often, Test Smart"

- **Test Early:** Unit tests written DURING implementation, not after
- **Test Often:** Tests run after EVERY sub-phase completion
- **Test Smart:** Focus on high-risk areas (precision, generics, edge cases)

**Test Pyramid for Float32 Generator:**

```
                    ╱╲
                   ╱  ╲
                  ╱ E2E ╲           Level 4: End-to-End Tests
                 ╱────────╲         - Full layer conversions
                ╱          ╲        - Time: 10% of total test time
               ╱────────────╲
              ╱ Integration  ╲      Level 3: Integration Tests
             ╱────────────────╲     - Generator pipeline tests
            ╱                  ╲    - Time: 20% of total test time
           ╱────────────────────╲
          ╱   Component Tests    ╲  Level 2: Component Tests
         ╱────────────────────────╲ - Rewriter tests
        ╱                          ╲- Validation tests
       ╱────────────────────────────╲ Time: 40% of total test time
      ╱        Unit Tests            ╲
     ╱──────────────────────────────────╲ Level 1: Unit Tests
    ╱                                    ╲ - Individual functions
   ╱──────────────────────────────────────╲ - Time: 30% of total test time
```

---

### Test Categories

#### 1. Unit Tests (Fine-Grained)

**Purpose:** Test individual functions/methods in isolation

**Scope:**
- Individual Rewriter methods (VisitLiteralExpression, VisitParameter, etc.)
- Conversion functions (ConvertLiteralToFloat32Semantic, etc.)
- Discovery filter functions
- Validation predicates

**Example:**
```csharp
[TestFixture]
public class LiteralRewriterTests
{
    [Test]
    public void TestUltraSmallEpsilonClamping()
    {
        // Arrange
        var rewriter = new LiteralRewriter();
        double input = 1e-15;  // Ultra-small

        // Act
        float result = rewriter.ConvertLiteralToFloat32Semantic(input);

        // Assert
        Assert.That(result, Is.EqualTo(1e-7f));  // Clamped to Float32 epsilon
    }

    [Test]
    public void TestNormalValueDirectConversion()
    {
        var rewriter = new LiteralRewriter();
        double input = 3.14159265358979;

        float result = rewriter.ConvertLiteralToFloat32Semantic(input);

        Assert.That(result, Is.EqualTo(3.14159265f).Within(1e-7f));
    }

    [Test]
    public void TestOutOfRangeClamping()
    {
        var rewriter = new LiteralRewriter();
        double input = 1e100;  // > float.MaxValue

        float result = rewriter.ConvertLiteralToFloat32Semantic(input);

        Assert.That(result, Is.EqualTo(float.MaxValue));
    }
}
```

**When to Run:** After implementing each method/function

**Time Allocation:** ~30% of total test time

---

#### 2. Component Tests (Medium-Grained)

**Purpose:** Test entire components/modules with dependencies

**Scope:**
- Complete Rewriter classes (with real SyntaxTrees)
- Discovery stage (with real .csproj files)
- Validation stage (with real generated code)

**Example:**
```csharp
[TestFixture]
public class TypeNameRewriterComponentTests
{
    [Test]
    public void TestCompleteFileTransformation()
    {
        // Arrange: Real C# source code
        var source = @"
namespace Test
{
    public class XGaFloat64Processor
    {
        public XGaFloat64Multivector CreateVector(double x, double y)
        {
            return new XGaFloat64Multivector();
        }
    }
}";

        var tree = CSharpSyntaxTree.ParseText(source);
        var rewriter = new TypeNameRewriter();

        // Act
        var newTree = rewriter.Visit(tree.GetRoot());

        // Assert
        var newSource = newTree.ToFullString();
        Assert.That(newSource, Does.Contain("XGaFloat32Processor"));
        Assert.That(newSource, Does.Contain("XGaFloat32Multivector"));
        Assert.That(newSource, Does.Not.Contain("Float64"));
    }
}
```

**When to Run:** After completing each component implementation

**Time Allocation:** ~40% of total test time

---

#### 3. Integration Tests (Coarse-Grained)

**Purpose:** Test multiple components working together

**Scope:**
- Full generator pipeline (Discovery → Transformation → Validation → Output)
- Mini project conversions (5-10 files)

**Example:**
```csharp
[TestFixture]
public class GeneratorPipelineIntegrationTests
{
    [Test]
    public void TestFullPipelineOnMiniProject()
    {
        // Arrange: Create mini test project
        var testProject = CreateMiniProject(new[]
        {
            "XGaFloat64Processor.cs",
            "XGaFloat64Multivector.cs",
            "XGaFloat64Vector.cs"
        });

        var generator = new Float32CodeGenerator();

        // Act
        var result = generator.Run(testProject);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.GeneratedFiles.Count, Is.EqualTo(3));
        Assert.That(result.ValidationErrors.Count, Is.EqualTo(0));

        // Verify compilation
        var compilation = CompileGeneratedFiles(result.GeneratedFiles);
        Assert.That(compilation.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error), Is.False);
    }
}
```

**When to Run:** After completing Phase 1 (Generator Development)

**Time Allocation:** ~20% of total test time

---

#### 4. End-to-End Tests (Full-Stack)

**Purpose:** Test complete layer conversions in realistic scenarios

**Scope:**
- Full Algebra layer conversion (329 files)
- Full LinearAlgebra layer conversion
- Full Modeling layer conversion

**Example:**
```csharp
[TestFixture]
public class AlgebraLayerE2ETests
{
    [Test]
    [Explicit]  // Only run when needed (expensive!)
    public void TestCompleteAlgebraLayerConversion()
    {
        // Arrange
        var algebraProject = LoadProject("GeometricAlgebraFulcrumLib.Algebra");
        var generator = new Float32CodeGenerator();

        // Act
        var result = generator.Run(algebraProject);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.GeneratedFiles.Count, Is.EqualTo(329));

        // Compile generated code
        var compilation = CompileGeneratedProject(result);
        var errors = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        Assert.That(errors, Is.Empty,
            $"Compilation failed with {errors.Count} errors:\n{string.Join("\n", errors)}");
    }
}
```

**When to Run:** Before committing each layer in Phase 2

**Time Allocation:** ~10% of total test time

---

### Phase-Specific Test Strategy

#### Phase 0: Interface Refactoring - Test Strategy

**Testing Frequency:** After EACH sub-phase (0.1 → 0.7)

| Sub-Phase | Test Type | What to Test | Acceptance Criteria | Time |
|-----------|-----------|--------------|---------------------|------|
| **0.1** Interface Changes | Unit | Interface compiles | No compile errors | 15 min |
| **0.2** ScalarProcessor Impls | Unit + Component | - All 6 implementations compile<br>- ZeroEpsilon correct type<br>- ToPrecision() works | All tests pass | 1.5h |
| **0.3** Processor Layer | Unit + Component | - XGaProcessor<T, TPrecision> compiles<br>- RGaProcessor<T, TPrecision> compiles | All tests pass | 1.5h |
| **0.4** Multivectors & Composers | Unit + Component | - Generic constraints propagated<br>- Composer pattern works | All tests pass | 2h |
| **0.5** Generic Constraints | Unit | - All constraints compile<br>- No constraint violations | Compiler happy | 1h |
| **0.6** ScalarProcessorNumberUtils | Unit | - All 90+ methods compile<br>- Generic operations work | All tests pass | 2h |
| **0.7** Full Verification | Integration | - All existing unit tests pass<br>- Float64 still works<br>- Float32 compiles | **ALL 1153 tests pass!** | 2h |

**Total Phase 0 Test Time: 10h (included in 56h estimate)**

**Phase 0 Acceptance Criteria:**
```csharp
[TestFixture]
public class Phase0AcceptanceTests
{
    [Test]
    public void AllExistingTestsStillPass()
    {
        // Run entire test suite (1153 tests)
        var results = TestRunner.RunAll("GeometricAlgebraFulcrumLib.UnitTests");

        Assert.That(results.TotalTests, Is.EqualTo(1153));
        Assert.That(results.Passed, Is.EqualTo(1129));
        Assert.That(results.Skipped, Is.EqualTo(24));
        Assert.That(results.Failed, Is.EqualTo(0));  // ✅ NO REGRESSIONS!
    }

    [Test]
    public void IScalarProcessorHasCorrectConstraint()
    {
        var interfaceType = typeof(IScalarProcessor<,>);
        var constraints = interfaceType.GetGenericArguments()[1].GetGenericParameterConstraints();

        Assert.That(constraints, Does.Contain(typeof(INumberBase<>)));
    }

    [Test]
    public void AllScalarProcessorsCompile()
    {
        // Verify all implementations compile with new interface
        Assert.DoesNotThrow(() => new ScalarProcessorOfFloat64());
        Assert.DoesNotThrow(() => new ScalarProcessorOfFloat32());
        Assert.DoesNotThrow(() => new ScalarProcessorOfERational());
        Assert.DoesNotThrow(() => new ScalarProcessorOfMetaExpression(new MetaContext()));
    }

    [Test]
    public void Float32ProcessorUsesCorrectEpsilon()
    {
        var processor = ScalarProcessorOfFloat32.Instance;

        Assert.That(processor.ZeroEpsilon, Is.TypeOf<float>());
        Assert.That(processor.ZeroEpsilon, Is.EqualTo(1e-7f));  // Correct precision!
    }
}
```

---

#### Phase 1: Generator Development - Test Strategy

**Testing Frequency:** After EACH component + Integration tests at end

| Component | Test Type | What to Test | Acceptance Criteria | Time |
|-----------|-----------|--------------|---------------------|------|
| **1.1** Discovery Stage | Unit + Component | - Float64 type detection<br>- Semantic verification<br>- Dependency analysis | 100% correct type detection | 1h |
| **1.2** TypeNameRewriter | Unit + Component | Float64→Float32 conversions | All variants covered | 1h |
| **1.3** LiteralRewriter | Unit | - Category-based scaling<br>- Default parameters<br>- Edge cases | All 5 categories + edge cases | 2h |
| **1.4** GenericParameterRewriter | Unit + Component | <T>→<T, TPrecision> | All generic cases | 1h |
| **1.5** Other Rewriters | Unit | Math→MathF, etc. | All conversions correct | 1h |
| **1.6** Validation Pipeline | Component | 5-phase validation | Each phase catches errors | 2h |
| **1.7** Integration | Integration | Full pipeline on mini project | 5-10 files convert correctly | 2h |

**Total Phase 1 Test Time: 10h (included in 39h estimate)**

**Phase 1 Acceptance Criteria:**
```csharp
[TestFixture]
public class Phase1AcceptanceTests
{
    [Test]
    public void GeneratorConvertsSimpleClassCorrectly()
    {
        var source = @"
public class XGaFloat64Processor
{
    private double _epsilon = 1e-13;
    public double GetEpsilon() => _epsilon;
}";

        var generator = new Float32CodeGenerator();
        var result = generator.Transform(source);

        Assert.That(result, Does.Contain("XGaFloat32Processor"));
        Assert.That(result, Does.Contain("float _epsilon = 1e-7f"));  // Clamped!
        Assert.That(result, Does.Contain("float GetEpsilon()"));
    }

    [Test]
    public void LiteralRewriterHandlesAllCategories()
    {
        var rewriter = new LiteralRewriter();

        // Category 1: Ultra-small
        Assert.That(rewriter.ConvertLiteralToFloat32Semantic(1e-15), Is.EqualTo(1e-7f));

        // Category 2: Small epsilon
        Assert.That(rewriter.ConvertLiteralToFloat32Semantic(1e-8), Is.EqualTo(1e-8f));

        // Category 3: Normal
        Assert.That(rewriter.ConvertLiteralToFloat32Semantic(3.14159), Is.EqualTo(3.14159f).Within(1e-7f));

        // Category 4: Large
        Assert.That(rewriter.ConvertLiteralToFloat32Semantic(1e30), Is.EqualTo(1e30f));

        // Category 5: Out of range
        Assert.That(rewriter.ConvertLiteralToFloat32Semantic(1e100), Is.EqualTo(float.MaxValue));
    }

    [Test]
    public void ValidationCatchesUnconvertedDouble()
    {
        var badCode = @"
public class XGaFloat32Processor
{
    private double _epsilon = 1e-13;  // ❌ Unconverted!
}";

        var validator = new TransformationCompletenessValidator();
        var report = validator.Validate(badCode);

        Assert.That(report.Errors.Count, Is.GreaterThan(0));
        Assert.That(report.Errors[0].Message, Does.Contain("Unconverted 'double' keyword"));
    }
}
```

---

#### Phase 1A: MetaProgramming - Test Strategy

**Testing Focus:** Code generation produces Float32-compatible code

| Component | Test Type | What to Test | Time |
|-----------|-----------|--------------|------|
| Float32Converter | Unit | All Math→MathF conversions | 1h |
| Generic Evaluation | Unit | Float32 evaluation works | 1h |
| Integration | Component | Generate + compile code | 1h |

**Total Phase 1A Test Time: 3h (included in 9h estimate)**

**Phase 1A Acceptance Criteria:**
```csharp
[TestFixture]
public class Phase1AAcceptanceTests
{
    [Test]
    public void Float32ConverterGeneratesCorrectCode()
    {
        var context = new MetaContext();
        var processor = XGaProcessor<IMetaExpressionAtomic, float>
            .CreateEuclidean(context.ScalarProcessor);

        var x = context.GetOrDefineParameterVariable("x");
        var expr = context.Sin(x);  // Symbolic sin(x)

        var converter = MetaExpressionToCSharpFloat32Converter.DefaultConverter;
        var steExpr = converter.Visit(expr);

        var code = GenerateCode(steExpr);
        Assert.That(code, Does.Contain("MathF.Sin"));  // ✅ Not Math.Sin!
    }

    [Test]
    public void GenericEvaluationSupportsFloat32()
    {
        var history = new GaFuLMetaContextCodeComposerOptions();
        var composer = history.CreateContextCodeComposer(
            GaFuLLanguageServerBase.CSharpFloat32()  // ✅ Float32!
        );

        composer.TextComposer.ScalarTypeName = "float";
        Assert.That(composer.ScalarTypeName, Is.EqualTo("float"));
    }
}
```

---

#### Phase 2: Layer Conversion - Test Strategy

**Testing Pattern for EACH Layer:**

```
1. Run Generator
2. Review Validation Report (automated)
3. Run Compilation Test
4. Run Smoke Tests (quick sanity checks)
5. Run Differential Tests (optional, for critical layers)
```

| Layer | Files | Compilation | Smoke Tests | Differential | Total Time |
|-------|-------|-------------|-------------|--------------|------------|
| **2.1** Algebra | 329 | ✅ Required | ✅ Required | ⭕ Optional | 6h |
| **2.2** LinearAlgebra | 50-100 | ✅ Required | ✅ Required | ⭕ Optional | 4h |
| **2.3** Modeling | 374 | ✅ Required | ✅ Required | ✅ Required (CGA!) | 6h |
| **2.4** Utilities | 2 | ✅ Required | ✅ Required | ❌ Not needed | 1h |

**Smoke Tests per Layer:**

```csharp
[TestFixture]
public class Phase2SmokeTests
{
    [Test]
    [Category("Smoke")]
    public void AlgebraLayer_BasicOperations()
    {
        // Arrange
        var processor = XGaFloat32Processor.Euclidean;

        // Act
        var v1 = processor.Vector(1f, 2f, 3f);
        var v2 = processor.Vector(4f, 5f, 6f);
        var gp = v1.Gp(v2);        // Geometric product
        var op = v1.Op(v2);        // Outer product
        var sp = v1.Sp(v2);        // Scalar product
        var norm = v1.ENorm();     // Euclidean norm

        // Assert
        Assert.That(gp, Is.Not.Null);
        Assert.That(op, Is.Not.Null);
        Assert.That(sp.ScalarValue, Is.GreaterThan(0));
        Assert.That(norm.ScalarValue, Is.GreaterThan(0));
    }

    [Test]
    [Category("Smoke")]
    public void LinearAlgebraLayer_QuaternionOperations()
    {
        // Arrange
        var q1 = LinFloat32Quaternion.Create(1f, 0f, 0f, 0f);  // Identity
        var q2 = LinFloat32Quaternion.Create(0f, 1f, 0f, 0f);  // i

        // Act
        var product = q1.Multiply(q2);
        var conjugate = q1.Conjugate();
        var norm = q1.Norm();

        // Assert
        Assert.That(product, Is.Not.Null);
        Assert.That(norm, Is.EqualTo(1f).Within(1e-6f));
    }

    [Test]
    [Category("Smoke")]
    public void ModelingLayer_CGAOperations()
    {
        // Arrange
        var cga = CGaFloat32GeometricSpace5D.Instance;

        // Act
        var point = cga.Encode.IpnsRound.Point(1f, 2f, 3f);
        var sphere = cga.Encode.IpnsRound.Sphere(0f, 0f, 0f, 1f);
        var plane = cga.Encode.Opns.Plane(0f, 0f, 1f, 1f);

        // Assert
        Assert.That(point, Is.Not.Null);
        Assert.That(sphere, Is.Not.Null);
        Assert.That(plane, Is.Not.Null);
    }
}
```

**Differential Tests (Critical for Modeling Layer):**

```csharp
[TestFixture]
public class Phase2DifferentialTests
{
    [Test]
    [Category("Differential")]
    public void CGA_PointEncodingMatchesWithinTolerance()
    {
        // Arrange
        var cga64 = CGaFloat64GeometricSpace5D.Instance;
        var cga32 = CGaFloat32GeometricSpace5D.Instance;

        double x = 1.234567890123456;
        double y = 2.345678901234567;
        double z = 3.456789012345678;

        // Act
        var point64 = cga64.Encode.IpnsRound.Point(x, y, z);
        var point32 = cga32.Encode.IpnsRound.Point((float)x, (float)y, (float)z);

        // Assert: Compare scalar values with tolerance
        const float tolerance = 1e-6f;
        foreach (var (id, scalar64) in point64.IdScalarPairs)
        {
            if (point32.TryGetBasisBladeScalarValue(id, out var scalar32))
            {
                Assert.That((float)scalar64, Is.EqualTo(scalar32).Within(tolerance),
                    $"Mismatch at basis blade {id}");
            }
        }
    }
}
```

**Phase 2 Acceptance Criteria:**
- ✅ All layers compile without errors
- ✅ All smoke tests pass
- ✅ Differential tests pass for Modeling layer (tolerance: 1e-6f)
- ✅ No Float64 references in generated code
- ✅ Validation reports: 0 errors

---

#### Phase 3: Final Testing & Validation - Test Strategy

**Comprehensive Test Suite:**

| Test Type | Scope | Time |
|-----------|-------|------|
| Full Compilation | All 755-805 files | 1h |
| Smoke Tests | All 3 layers | 1h |
| Performance Benchmarks | Float32 vs Float64 | 2h |
| Differential Tests | Critical paths | 2h |
| Regression Tests | Existing 1153 tests | 2h |

**Total Phase 3 Test Time: 8h**

**Performance Benchmark Example:**

```csharp
[TestFixture]
public class Phase3PerformanceBenchmarks
{
    [Test]
    [Explicit]
    public void Benchmark_GeometricProduct_Float32vsFloat64()
    {
        // Arrange
        var processor64 = XGaFloat64Processor.Euclidean;
        var processor32 = XGaFloat32Processor.Euclidean;

        var v1_64 = processor64.Vector(1.0, 2.0, 3.0);
        var v2_64 = processor64.Vector(4.0, 5.0, 6.0);

        var v1_32 = processor32.Vector(1f, 2f, 3f);
        var v2_32 = processor32.Vector(4f, 5f, 6f);

        const int iterations = 1000000;

        // Act: Benchmark Float64
        var sw64 = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var _ = v1_64.Gp(v2_64);
        }
        sw64.Stop();

        // Act: Benchmark Float32
        var sw32 = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var _ = v1_32.Gp(v2_32);
        }
        sw32.Stop();

        // Assert: Float32 should be faster (or at least not slower)
        Console.WriteLine($"Float64: {sw64.ElapsedMilliseconds}ms");
        Console.WriteLine($"Float32: {sw32.ElapsedMilliseconds}ms");
        Console.WriteLine($"Speedup: {(double)sw64.ElapsedMilliseconds / sw32.ElapsedMilliseconds:F2}x");

        Assert.That(sw32.ElapsedMilliseconds, Is.LessThanOrEqualTo(sw64.ElapsedMilliseconds * 1.1),
            "Float32 should not be significantly slower than Float64");
    }
}
```

**Phase 3 Acceptance Criteria:**
- ✅ All generated code compiles (755-805 files)
- ✅ All smoke tests pass (3 layers)
- ✅ Performance: Float32 not significantly slower than Float64
- ✅ Differential tests: Max error < 1e-6f for all critical operations
- ✅ All existing 1153 unit tests still pass (no regressions!)
- ✅ Documentation updated

---

### Test Time Allocation Summary

| Phase | Development | Testing | Total | Test % |
|-------|-------------|---------|-------|--------|
| **Phase 0** | 46h | 10h | **56h** | 18% |
| **Phase 1** | 29h | 10h | **39h** | 26% |
| **Phase 1A** | 6h | 3h | **9h** | 33% |
| **Phase 2** | 10h | 7h | **17h** | 41% |
| **Phase 3** | 0h | 8h | **8h** | 100% |
| **Total** | 91h | 38h | **129h** | 29% |

**Key Insight:** ~30% of total time is dedicated to testing! This ensures quality and prevents regressions.

---

### Test Infrastructure Requirements

**Test Project Structure:**
```
GA.Float32.CodeGenerator.Tests/
├── Unit/
│   ├── Rewriters/
│   │   ├── LiteralRewriterTests.cs
│   │   ├── TypeNameRewriterTests.cs
│   │   └── GenericParameterRewriterTests.cs
│   ├── Discovery/
│   │   └── TypeDiscoveryTests.cs
│   └── Validation/
│       └── ValidationPipelineTests.cs
├── Component/
│   ├── DiscoveryStageTests.cs
│   ├── TransformationStageTests.cs
│   └── ValidationStageTests.cs
├── Integration/
│   └── GeneratorPipelineTests.cs
├── E2E/
│   ├── AlgebraLayerTests.cs
│   ├── LinearAlgebraLayerTests.cs
│   └── ModelingLayerTests.cs
└── TestData/
    ├── MiniProject/
    │   ├── XGaFloat64Processor.cs
    │   └── XGaFloat64Multivector.cs
    └── ExpectedOutput/
        ├── XGaFloat32Processor.cs
        └── XGaFloat32Multivector.cs
```

**CI/CD Integration:**
```yaml
# .github/workflows/float32-tests.yml
name: Float32 Generator Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET 7+
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '7.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

      - name: Run Unit Tests
        run: dotnet test --no-build --filter "Category=Unit"

      - name: Run Component Tests
        run: dotnet test --no-build --filter "Category=Component"

      - name: Run Integration Tests
        run: dotnet test --no-build --filter "Category=Integration"

      - name: Run Smoke Tests
        run: dotnet test --no-build --filter "Category=Smoke"
```

---

### Continuous Testing Guidelines

**During Development:**
- ✅ Write tests BEFORE implementing features (TDD)
- ✅ Run tests after EVERY code change
- ✅ Use test-driven refactoring

**Before Committing:**
- ✅ All unit tests pass
- ✅ All component tests pass
- ✅ Code coverage > 80% for new code

**Before Phase Completion:**
- ✅ All tests for phase pass
- ✅ Integration tests pass
- ✅ Acceptance criteria met

**Before Release:**
- ✅ ALL tests pass (Unit + Component + Integration + E2E)
- ✅ Performance benchmarks acceptable
- ✅ No regressions in existing 1153 tests

---

## Test Parallelism: Float64 → Float32 Test Duplication

### Overview

**CRITICAL REQUIREMENT:** All existing Float64-specific tests MUST have corresponding Float32 counterparts to ensure complete test coverage for the new Float32 implementation.

**Current Test Suite Status:**
- Total existing tests: **1153 tests**
- Pass rate: **97.92%** (1129 passing, 24 skipped, 0 failing)
- Float64-specific tests identified: **~129 tests** (across 9 test files)
- Generic processor tests: **~105+ tests** (use XGaFloat64Processor)

**Post-Float32 Expected Tests:** ~**2300+ tests** (nearly doubled test suite)

---

### Float64-Specific Test Files Requiring Float32 Counterparts

#### Category 1: Algebra Layer Tests (Algebra/Euclidean) - 44 Tests

| Float64 Test File | Test Count | Float32 Counterpart | Priority |
|-------------------|------------|---------------------|----------|
| `LinFloat64QuaternionTests.cs` | 10 tests | `LinFloat32QuaternionTests.cs` | **P0 - CRITICAL** |
| `LinFloat64Vector3DTests.cs` | 10 tests | `LinFloat32Vector3DTests.cs` | **P0 - CRITICAL** |
| `LinFloat64Vector2DTests.cs` | 10 tests | `LinFloat32Vector2DTests.cs` | **P0 - CRITICAL** |
| `LinFloat64BivectorTests.cs` | 8 tests | `LinFloat32BivectorTests.cs` | P1 |
| `LinFloat64AngleTests.cs` | 6 tests | `LinFloat32AngleTests.cs` | P1 |

**Test Coverage:**
- **Quaternion** (10 tests): Construction, norm, conjugate, inverse, multiplication, rotation
- **Vector3D** (10 tests): Construction, basis vectors, arithmetic, norm, dot product, cross product, normalization
- **Vector2D** (10 tests): Construction, polar coordinates, arithmetic, norm, dot product, orthogonality, normalization
- **Bivector** (8 tests): 2D and 3D bivector construction, basis bivectors, dual, scalar product
- **Angle** (6 tests): PolarAngle and DirectedAngle creation, trigonometric functions, special angles

#### Category 2: Modeling Layer Tests (Modeling/Geometry/Euclidean) - 45 Tests

| Float64 Test File | Test Count | Float32 Counterpart | Priority |
|-------------------|------------|---------------------|----------|
| `LinFloat64QuaternionTests.cs` | 15 tests | `LinFloat32QuaternionTests.cs` | **P0 - CRITICAL** |
| `LinFloat64VectorTests.cs` | 20 tests | `LinFloat32VectorTests.cs` | **P0 - CRITICAL** |
| `LinFloat64BivectorTests.cs` | 10 tests | `LinFloat32BivectorTests.cs` | P1 |

**Test Coverage:**
- **Quaternion** (15 tests): More comprehensive quaternion tests including axis-angle construction, arithmetic operations, multiplication non-commutativity
- **Vector** (20 tests): 2D, 3D, 4D vector operations, chained operations, operator consistency
- **Bivector** (10 tests): More comprehensive 2D and 3D bivector tests including basis bivectors, arithmetic

#### Category 3: Modeling Layer Tests (Modeling/Signals) - 40 Tests

| Float64 Test File | Test Count | Float32 Counterpart | Priority |
|-------------------|------------|---------------------|----------|
| `Float64SignalInterpolationTests.cs` | 40 tests | `Float32SignalInterpolationTests.cs` | P2 |

**Test Coverage:**
- **Signal Construction** (10 tests): FiniteZero, PeriodicZero, FiniteConstant, arrays, indexing
- **Linear Spline Interpolation** (10 tests): Data point accuracy, interpolation, constant signals, derivatives
- **Catmull-Rom Spline Interpolation** (10 tests): Creation, data point accuracy, smoothness, derivatives
- **Signal Properties** (10 tests): Enumeration, sampling specs, periodic vs finite, edge cases

#### Category 4: Generic Processor Tests - ~105+ Tests

| Float64 Test File | Test Count | Float32 Adaptation | Priority |
|-------------------|------------|-------------------|----------|
| `ProcessorSpecificTests.cs` | ~30+ tests | Use `XGaFloat32Processor` | **P0 - CRITICAL** |
| `ProductOperationsTests.cs` | ~40+ tests | Use `XGaFloat32Processor` | **P0 - CRITICAL** |
| `UnaryOperationsTests.cs` | ~35+ tests | Use `XGaFloat32Processor` | **P0 - CRITICAL** |

**Test Coverage:**
- **Processor-Specific** (~30 tests): Euclidean, Conformal, Projective processors, basis vectors, metrics, custom signatures
- **Product Operations** (~40 tests): Outer product, geometric product, scalar product, contractions, commutator, anti-commutator, fat dot, Hestenes inner product
- **Unary Operations** (~35 tests): Reverse, grade involution, Clifford conjugate, norm, inverse, negative

**Important Note:** These tests currently use `XGaFloat64Processor` but test algebraic properties that should work identically with `XGaFloat32Processor`. They need to be adapted, not duplicated.

---

### Test Duplication Strategy

#### Phase 3A: Float32 Test Suite Creation (New Phase)

**Goal:** Create complete Float32 test suite mirroring all Float64 tests

**Approach:** **Automated Test Generation** (Preferred)

**Why Automated?**
- **129+ test files** to duplicate manually is error-prone
- **Naming consistency** critical (LinFloat64* → LinFloat32*)
- **Tolerance adjustments** must be systematic (1e-10 → 1e-7f)
- **Type name replacements** must be complete (Float64 → Float32, double → float)

**Automated Test Generator Workflow:**

```
1. Discovery Phase:
   - Scan GeometricAlgebraFulcrumLib.UnitTests/ for *Float64*Tests.cs
   - Identify generic processor tests using XGaFloat64Processor
   - Parse test file structure (test fixtures, test methods, assertions)

2. Transformation Phase:
   - Copy test file structure
   - Replace: LinFloat64* → LinFloat32*
   - Replace: Float64 → Float32
   - Replace: double → float (in literals and type names)
   - Replace: XGaFloat64Processor → XGaFloat32Processor
   - Adjust: Tolerance constants (1e-10 → 1e-7f, 1e-12 → 1e-7f)
   - Adjust: Literal suffixes (1d → 1f, 0.0D → 0.0f)
   - Adjust: Math.* → MathF.* (Sin, Cos, Sqrt, etc.)

3. Validation Phase:
   - Verify all Float32 test files compile
   - Verify test structure matches Float64 counterparts
   - Check that no Float64 references remain

4. Output:
   - New test files in parallel directory structure:
     GeometricAlgebraFulcrumLib.UnitTests/
     ├── Algebra/Euclidean/
     │   ├── LinFloat64QuaternionTests.cs (existing)
     │   ├── LinFloat32QuaternionTests.cs (NEW)
     │   ├── LinFloat64Vector3DTests.cs (existing)
     │   ├── LinFloat32Vector3DTests.cs (NEW)
     │   └── ... (all Float32 counterparts)
     ├── Modeling/Geometry/Euclidean/
     │   ├── LinFloat64QuaternionTests.cs (existing)
     │   ├── LinFloat32QuaternionTests.cs (NEW)
     │   └── ... (all Float32 counterparts)
     └── ... (rest of test directories)
```

**Test Generator Roslyn Rewriters:**

```csharp
// TestFileRewriter: Main rewriter for test files
public class TestFileRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
    {
        // Replace type names
        var newName = node.Identifier.Text
            .Replace("Float64", "Float32")
            .Replace("XGaFloat64", "XGaFloat32")
            .Replace("LinFloat64", "LinFloat32");

        if (newName != node.Identifier.Text)
            return node.WithIdentifier(SyntaxFactory.Identifier(newName));

        return base.VisitIdentifierName(node);
    }

    public override SyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
    {
        if (node.IsKind(SyntaxKind.NumericLiteralExpression))
        {
            // Replace double literals with float literals
            var text = node.Token.Text;
            if (text.EndsWith("d", StringComparison.OrdinalIgnoreCase) ||
                text.EndsWith("D") ||
                text.Contains(".") && !text.EndsWith("f", StringComparison.OrdinalIgnoreCase))
            {
                var newText = text.TrimEnd('d', 'D') + "f";
                return SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(newText, float.Parse(newText.TrimEnd('f')))
                );
            }
        }
        return base.VisitLiteralExpression(node);
    }

    public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
    {
        // Replace Math.* with MathF.*
        if (node.Identifier.Text == "Math")
            return node.WithIdentifier(SyntaxFactory.Identifier("MathF"));

        return base.VisitIdentifierName(node);
    }
}

// ToleranceRewriter: Adjust tolerance constants
public class ToleranceRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        // Find: private const double Tolerance = 1e-10;
        // Replace: private const float Tolerance = 1e-7f;

        if (IsToleranceField(node))
        {
            var newDeclaration = node.Declaration
                .WithType(SyntaxFactory.ParseTypeName("float"))
                .WithVariables(SyntaxFactory.SeparatedList(
                    node.Declaration.Variables.Select(v =>
                        v.WithInitializer(SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.NumericLiteralExpression,
                                SyntaxFactory.Literal("1e-7f", 1e-7f)
                            )
                        ))
                    )
                ));

            return node.WithDeclaration(newDeclaration);
        }

        return base.VisitFieldDeclaration(node);
    }

    private bool IsToleranceField(FieldDeclarationSyntax node)
    {
        return node.Declaration.Variables.Any(v =>
            v.Identifier.Text.Contains("Tolerance", StringComparison.OrdinalIgnoreCase));
    }
}
```

---

### Test Naming Conventions

**Strict Naming Rules:**

| Float64 Test | Float32 Test | Rationale |
|-------------|-------------|-----------|
| `LinFloat64QuaternionTests.cs` | `LinFloat32QuaternionTests.cs` | Direct 1:1 mapping |
| `LinFloat64Vector3DTests.cs` | `LinFloat32Vector3DTests.cs` | Preserves dimensionality suffix |
| `Float64SignalInterpolationTests.cs` | `Float32SignalInterpolationTests.cs` | Direct 1:1 mapping |
| `ProcessorSpecificTests.cs` | `ProcessorSpecificTests.cs` (adapt) | Generic test, modify processor type |

**Test Method Naming:**
- Keep test method names IDENTICAL across Float64 and Float32 versions
- Example: `Quaternion_Conjugate_ShouldNegateVectorPart()` → Same name in Float32 version
- Rationale: Easy to compare test results, understand parallel structure

---

### Tolerance Adjustment Strategy

**Float64 vs Float32 Precision:**

| Context | Float64 Tolerance | Float32 Tolerance | Adjustment Factor |
|---------|------------------|-------------------|-------------------|
| **Standard Tests** | `1e-10` | `1e-7f` | 1000× looser |
| **Strict Tests** | `1e-12` | `1e-7f` | 100000× looser |
| **Relaxed Tests** | `1e-8` | `1e-6f` | 100× looser |
| **Compound Operations** | `1e-10 * 10` | `1e-6f` | Slightly looser |

**Automated Tolerance Replacement Rules:**

```csharp
// Tolerance replacement map
var toleranceMap = new Dictionary<string, string>
{
    { "1e-10", "1e-7f" },
    { "1e-12", "1e-7f" },
    { "1e-13", "1e-7f" },
    { "1e-15", "1e-7f" },
    { "1e-8", "1e-6f" },
    { "1e-9", "1e-7f" },
};

// Context-specific adjustments
if (IsCompoundOperation(testMethod))
{
    tolerance = "1e-6f";  // Slightly looser for compound ops
}
```

**Special Cases:**

1. **Norm Preservation Tests:**
   - Float64: `Assert.That(norm, Is.EqualTo(1.0).Within(1e-10))`
   - Float32: `Assert.That(norm, Is.EqualTo(1.0f).Within(1e-6f))` (looser due to accumulated error)

2. **Rotation Tests:**
   - Float64: `Assert.That(rotated.X, Is.EqualTo(expected).Within(1e-10))`
   - Float32: `Assert.That(rotated.X, Is.EqualTo(expected).Within(1e-6f))`

3. **Interpolation Tests:**
   - Float64: `Assert.That(value, Is.EqualTo(expected).Within(1e-10))`
   - Float32: `Assert.That(value, Is.EqualTo(expected).Within(1e-6f))`

---

### Differential Testing: Float64 vs Float32

**Purpose:** Ensure Float32 implementation produces results within acceptable tolerance of Float64 implementation.

**Approach:** For critical operations, run BOTH Float64 and Float32 versions and compare results.

**Example Differential Test:**

```csharp
[TestFixture]
public class DifferentialTests
{
    private const float DifferentialTolerance = 1e-6f;

    [Test]
    public void QuaternionMultiplication_Float32VsFloat64_ShouldMatchWithinTolerance()
    {
        // Arrange: Create same quaternion in both precisions
        var q1_64 = LinFloat64Quaternion.Create(1.0, 2.0, 3.0, 4.0);
        var q2_64 = LinFloat64Quaternion.Create(5.0, 6.0, 7.0, 8.0);

        var q1_32 = LinFloat32Quaternion.Create(1.0f, 2.0f, 3.0f, 4.0f);
        var q2_32 = LinFloat32Quaternion.Create(5.0f, 6.0f, 7.0f, 8.0f);

        // Act: Perform same operation
        var result_64 = q1_64 * q2_64;
        var result_32 = q1_32 * q2_32;

        // Assert: Results should match within tolerance
        Assert.That(
            (float)result_64.Scalar.ScalarValue,
            Is.EqualTo(result_32.Scalar.ScalarValue).Within(DifferentialTolerance),
            "Scalar components should match"
        );
        Assert.That(
            (float)result_64.ScalarI.ScalarValue,
            Is.EqualTo(result_32.ScalarI.ScalarValue).Within(DifferentialTolerance),
            "I components should match"
        );
        Assert.That(
            (float)result_64.ScalarJ.ScalarValue,
            Is.EqualTo(result_32.ScalarJ.ScalarValue).Within(DifferentialTolerance),
            "J components should match"
        );
        Assert.That(
            (float)result_64.ScalarK.ScalarValue,
            Is.EqualTo(result_32.ScalarK.ScalarValue).Within(DifferentialTolerance),
            "K components should match"
        );
    }

    [Test]
    public void VectorNormalization_Float32VsFloat64_ShouldMatchWithinTolerance()
    {
        // Test that normalized vectors match between precisions
        var v_64 = LinFloat64Vector3D.Create(3.0, 4.0, 0.0);
        var v_32 = LinFloat32Vector3D.Create(3.0f, 4.0f, 0.0f);

        var normalized_64 = v_64.ToUnitLinVector3D();
        var normalized_32 = v_32.ToUnitLinVector3D();

        Assert.That(
            (float)normalized_64.X.ScalarValue,
            Is.EqualTo(normalized_32.X.ScalarValue).Within(DifferentialTolerance)
        );
        Assert.That(
            (float)normalized_64.Y.ScalarValue,
            Is.EqualTo(normalized_32.Y.ScalarValue).Within(DifferentialTolerance)
        );
    }
}
```

---

### Test Time Estimates

**Phase 3A: Float32 Test Suite Creation**

| Task | Time (Conservative) | Time (Aggressive) |
|------|---------------------|-------------------|
| **Test Generator Development** | 8h | 5h |
| - Roslyn test rewriter | 3h | 2h |
| - Tolerance adjuster | 2h | 1h |
| - Test discovery | 2h | 1h |
| - Integration | 1h | 1h |
| **Test Generation Execution** | 4h | 2h |
| - Run generator on 9 Float64 test files | 1h | 0.5h |
| - Manual review of generated tests | 2h | 1h |
| - Fix any generation issues | 1h | 0.5h |
| **Test Validation** | 6h | 4h |
| - Verify all Float32 tests compile | 2h | 1h |
| - Run Float32 test suite | 1h | 0.5h |
| - Fix failing tests | 2h | 1.5h |
| - Differential testing setup | 1h | 1h |
| **Documentation** | 2h | 1h |
| **TOTAL Phase 3A** | **20h** | **12h** |

**Total Timeline Update (with Phase 3A):**

| Phase | Conservative | Aggressive |
|-------|-------------|-----------|
| Phase 0 | 56h | 34h |
| Phase 1 | 39h | 28h |
| Phase 1A | 9h | 6h |
| Phase 2 | 17h | 11h |
| **Phase 3A (NEW)** | **20h** | **12h** |
| Phase 3 | 8h | 5h |
| **TOTAL** | **149h** | **96h** |

---

### Success Criteria for Test Parallelism

**Phase 3A Acceptance:**
- ✅ All 129 Float64 tests have Float32 counterparts (1:1 mapping)
- ✅ All Float32 test files compile without errors
- ✅ Test naming conventions followed (LinFloat64* → LinFloat32*)
- ✅ Tolerance constants adjusted systematically (1e-10 → 1e-7f)
- ✅ All Float32 tests run successfully (no crashes)
- ✅ Pass rate for Float32 tests: > 95% (some expected failures due to known Float32 precision limits)

**Post-Phase 3A Test Suite Size:**
- Before: 1153 tests (Float64 only)
- After: ~2280+ tests (Float64 + Float32)
- New tests: ~1127 tests (129 Float32 unit tests + 998 generated tests from processor adaptation)

**CI/CD Integration:**
- Float32 test suite runs on every commit
- Differential tests validate Float32 vs Float64 consistency
- Performance tests ensure Float32 is faster than Float64 for critical operations

---

## Risk Assessment

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| IScalarProcessor breaking change causes widespread issues | High | High | Systematic refactoring with compiler guidance |
| Partial classes cause inconsistencies | Low | Medium | Group and validate together |
| Path length exceeds 260 chars | Medium | Medium | New solution with short names |
| Generated code doesn't compile | Low | Critical | 5-phase validation pipeline |
| Math.Tau missing in MathF | High | Low | Simple substitution (2*PI) |
| MetaProgramming Float32 generation has casts | Low | Medium | Proper converter implementation |
| LinearAlgebra conversions break Modeling | Medium | High | Test Quaternion conversions thoroughly |

---

## Success Criteria

### Must Have (Required for completion):
- [ ] All ~755-805 files successfully converted
- [ ] Generated code compiles without errors in Release mode
- [ ] No Float64 references remain in generated Float32 code
- [ ] No double→float casts in hot paths
- [ ] Smoke tests pass for Algebra, LinearAlgebra, Modeling
- [ ] MetaProgramming generates clean Float32 code (no casts)
- [ ] LinFloat32Quaternion works correctly with System.Numerics.Quaternion

### Nice to Have (Optional enhancements):
- [ ] Differential tests show <1e-6 difference from Float64 (scaled for precision)
- [ ] Performance benchmarks show memory improvement
- [ ] Code generation documentation
- [ ] CI/CD integration
- [ ] Complex support (Phase 2 - future work)

---

## Next Steps

1. ✅ **Review this document** - APPROVED by user
2. ✅ **Design decisions finalized** - Option A confirmed
3. **Begin Phase 0** - IScalarProcessor<T, TPrecision> refactoring
4. **Develop generator** - Core pipeline implementation
5. **Iterate** based on results

---

## Appendix: Scope Verification

### ✅ Confirmed Conversions (755-805 files total)

1. **Algebra XGa/RGa**: 329 files
   - Verified: All XGaFloat64* and RGaFloat64* types
   - Includes: Processors, Multivectors, Composers

2. **Modeling**: 374 files
   - Verified: All modeling layer Float64 types
   - Includes: CGa, PGa, VGa implementations

3. **LinearAlgebra**: ~50-100 files
   - Verified: LinFloat64Quaternion (190 usages in Modeling)
   - Includes: Vector3D, Bivector3D, Angle types
   - Critical dependency for Modeling layer

4. **Utilities.Structures**: 2 files
   - Verified: Float64SparseVector, Float64SparseArray
   - Minimal code, trivial conversion

### ✅ Confirmed New Code (5-10 files)

5. **MetaProgramming Code Generation**: ~5-10 new files
   - MetaExpressionToCSharpFloat32Converter.cs
   - Factory method additions
   - Optional: Cpp, Matlab converters

### ✅ Confirmed Refactorings (4 files)

6. **Utilities.Code Base Generators**: 4 files
   - CclCSharpCodeGenerator.cs
   - CclCppCodeGenerator.cs
   - CclMatlabCodeGenerator.cs
   - CclExcelCodeGenerator.cs

### ❌ Explicitly Out of Scope

- Complex support (deferred to Phase 2)
- MetaProgramming expression types (already generic)
- Utilities.Text, Utilities.Web (no Float64 types)
- Matlab project (not core functionality)

---

**Document Version:** 2.3 (FINAL - Validation Strategy Overhauled)
**Last Updated:** 2025-10-20
**Status:** ✅ Ready for Implementation - All Design Decisions Finalized

## Changelog v2.3

**Phase 1 Validation Strategy - Complete Overhaul:**
- **Struktur-Änderung:** 4 Phasen → **5 Phasen** (Transformation Completeness hinzugefügt)
- **Phase 2 NEU:** Transformation Completeness Validation
  - Erkennt Rewriter-Bugs VOR Compilation
  - Prüft: Unbehandelte 'double' Keywords, 'Float64' Identifier, Trivia Preservation
  - Implementation: ~3-4 Stunden
- **Phase 3 FIXED:** Compilation Validation
  - **KRITISCHE ÄNDERUNG:** KEINE Float64 DLLs als References!
  - Verhindert False Positives (Code mit Float64-Usage würde sonst kompilieren)
  - Vollständige RequiredSystemAssemblies-Liste
- **Phase 4 KONKRETISIERT:** Semantic Validation
  - 4.1 Base Class Validation (mit Code)
  - 4.2 Interface Implementation Validation (mit Code)
  - 4.3 Member Preservation Validation (mit Code)
  - 4.4 Generic Constraints Validation (mit Code)
- **Phase 5 KONKRETISIERT:** Cross-Reference Validation
  - 5.1 Identifier Scan für Float64-References
  - 5.2 Generic Arguments Check für 'double'
  - 5.3 Math vs MathF Usage Check
  - 5.4 Literal Suffix Check (d → f)
- **Pipeline-Diagramm:** Visualisierung der 5 Phasen
- **Validation Report Models:** ValidationError, ValidationWarning, ValidationSeverity
- **Estimated Implementation Time:** 15-20 Stunden für komplette Validation

**Umfang:**
- +1100 Zeilen Code-Beispiele und Dokumentation
- TODO_FLOAT32.md: ~1900 → ~2400 Zeilen
- Alle Validationen mit vollständiger Implementation dokumentiert

**Gesamt-Timeline (ANGEPASST - Including Critical Findings):**
- Phase 0: 46h → **54-56h** (+8-10h für Finding 1 - ScalarProcessorNumberUtils.cs)
- Phase 1: 28h → **38-39h** (+10-11h für Findings 2 & 3 - Default Parameters + Literal Scaling)
- Conservative: 108h → **124-127h** (≈ 15-16 Arbeitstage)
- Aggressive: 63h → **76-79h** (≈ 9-10 Arbeitstage)

---

## Changelog v2.4

**INumberBase<TPrecision> Constraint Decision:**
- **Analysis:** Evaluated whether `IScalarProcessor<T, TPrecision>` should have generic constraint
- **Result:** ✅ YES - Use `where TPrecision : struct, INumberBase<TPrecision>`
- **Rationale:**
  - Type safety: Prevents invalid types like `IScalarProcessor<double, string>`
  - Consistency: Matches Finding 1 extension methods constraint
  - Enables generic numeric operations: `TPrecision.IsNaN()`, `TPrecision.Abs()`, etc.
  - No false restrictions: `T` remains unconstrained (symbolic processors work)
  - Future-proof: Supports float, double, decimal, Half
- **Verified:** All existing implementations (Float64, Float32, ERational, MetaExpression, etc.) work with constraint
- **Requirement:** .NET 7+ (user approved)

**Critical Findings Discovery (Post-Planning):**

After detailed code review of implementation files, **3 CRITICAL issues** were discovered that were not accounted for in previous planning versions (v2.0-v2.3):

**Finding 1: ScalarProcessorNumberUtils.cs - Missing Scope ⚠️ CRITICAL**
- **File:** `ScalarProcessorNumberUtils.cs` (402 lines, 90+ extension methods)
- **Problem:** ALL methods use hardcoded `ToFloat64()` and `double.*` static methods
- **Impact:** This file was NOT mentioned in original Phase 0 scope
- **Solution:** Refactor all 90+ methods to generic `<T, TPrecision>` with `INumberBase<TPrecision>` constraint
- **Requirements:** .NET 7+ for `INumberBase<T>` interface (approved by user)
- **Time Impact:** +8-10 hours to Phase 0

**Finding 2: Default Parameter Precision Values ⚠️ MEDIUM**
- **Files:** `Float64Utils.cs`, `Float64ArrayUtils.cs`, and many utility classes
- **Problem:** Default parameters like `epsilon = 1e-13` need semantic adjustment, not just type conversion
- **Example:** `1e-13` → `1e-13f` ❌ (too small for float!) → Should be `1e-7f` ✅
- **Solution:** Extend LiteralRewriter with `VisitParameter()` to handle default values semantically
- **Time Impact:** +4.5-5.5 hours to Phase 1

**Finding 3: LiteralRewriter Precision Scaling Heuristic is WRONG ⚠️ CRITICAL**
- **Original Plan (v2.2):** "Scale exponents by ~5" (e.g., 1e-16 → 1e-11f)
- **Problem 1:** 1e-11f is STILL too small for float (practical epsilon: 1e-7f)
- **Problem 2:** Scaling ALL literals breaks normal constants (e.g., Pi = 3.14159 → 314159.0f ❌)
- **Solution:** Category-based scaling with clamping:
  - Category 1: Ultra-small epsilons (< 1e-10) → Clamp to 1e-7f
  - Category 2-4: Normal values (1e-10 to float.MaxValue) → Direct conversion
  - Category 5: Out of range (> float.MaxValue) → Clamp to float.MaxValue
- **Time Impact:** +6 hours to Phase 1

**Shared Implementation:**
- Finding 2 and Finding 3 share the same `ConvertLiteralToFloat32Semantic()` method
- Actual overlap reduces combined time by ~0.5h
- Total Finding 2 + 3 impact: +10-11h (not +10.5-11.5h separately)

**Timeline Updates:**
- **Phase 0:** 46h → **54-56h** (Conservative), 25h → **32-34h** (Aggressive)
- **Phase 1:** 28h → **38-39h** (Conservative), 18h → **27-28h** (Aggressive)
- **Total Project:** 108h → **124-127h** (Conservative), 63h → **76-79h** (Aggressive)

**Documentation:**
- New section: "## Critical Findings (Post-Planning Discovery)" added with full implementation details
- All 3 findings documented with:
  - Problem description
  - Code examples (before/after)
  - Approved solutions
  - Time impact breakdown
  - Conversion tables (Finding 3)
- INumberBase<TPrecision> constraint analysis and decision documented in Phase 0.1
- Verified all existing implementations work with constraint (table included)
- TODO_FLOAT32.md: ~2400 → ~2900 Zeilen

**Version:** 2.6 (2025-10-20) - Test Parallelism: Float64 → Float32 Test Duplication Strategy

---

## Changelog v2.6

**Test Parallelism Section Added (NEW PHASE 3A):**

After analyzing all existing Float64-specific test files, a comprehensive test duplication strategy was created to ensure complete Float32 test coverage.

**Key Findings:**
- **9 Float64-specific test files identified:**
  - 5 files in Algebra/Euclidean (44 tests total)
  - 3 files in Modeling/Geometry/Euclidean (45 tests total)
  - 1 file in Modeling/Signals (40 tests total)
- **~105+ generic processor tests** that use `XGaFloat64Processor` and need adaptation
- **Total Float64-specific tests: ~129 tests**
- **Expected post-Float32 test suite: ~2300+ tests** (nearly doubled)

**Test Files Analyzed:**
1. `LinFloat64QuaternionTests.cs` (Algebra) - 10 tests
2. `LinFloat64Vector3DTests.cs` (Algebra) - 10 tests
3. `LinFloat64Vector2DTests.cs` (Algebra) - 10 tests
4. `LinFloat64BivectorTests.cs` (Algebra) - 8 tests
5. `LinFloat64AngleTests.cs` (Algebra) - 6 tests
6. `LinFloat64QuaternionTests.cs` (Modeling) - 15 tests
7. `LinFloat64VectorTests.cs` (Modeling) - 20 tests
8. `LinFloat64BivectorTests.cs` (Modeling) - 10 tests
9. `Float64SignalInterpolationTests.cs` (Modeling/Signals) - 40 tests

**New Section Contents:**

1. **Test Duplication Strategy:**
   - **Automated Test Generation** (preferred approach)
   - Roslyn-based test generator to systematically convert Float64 tests to Float32
   - Reasons: Consistency, systematic tolerance adjustments, error-free naming

2. **Test Generator Workflow:**
   - Discovery Phase: Scan for `*Float64*Tests.cs` files
   - Transformation Phase: Type name replacements, tolerance adjustments, Math→MathF
   - Validation Phase: Compilation checks, structure verification
   - Output: Parallel directory structure with Float32 counterparts

3. **Test Generator Roslyn Rewriters:**
   - `TestFileRewriter`: Main rewriter for type name replacements
   - `ToleranceRewriter`: Adjust tolerance constants (1e-10 → 1e-7f)
   - Full code examples provided

4. **Test Naming Conventions:**
   - Strict 1:1 mapping: `LinFloat64QuaternionTests.cs` → `LinFloat32QuaternionTests.cs`
   - Test method names kept IDENTICAL for easy comparison
   - Rationale: Easy to compare test results, understand parallel structure

5. **Tolerance Adjustment Strategy:**
   - Standard tests: 1e-10 → 1e-7f (1000× looser)
   - Strict tests: 1e-12 → 1e-7f (100000× looser)
   - Relaxed tests: 1e-8 → 1e-6f (100× looser)
   - Automated tolerance replacement map provided

6. **Differential Testing:**
   - Purpose: Ensure Float32 results match Float64 within tolerance
   - Approach: Run BOTH Float64 and Float32, compare with 1e-6f tolerance
   - Full code examples for quaternion and vector differential tests

7. **Phase 3A: Float32 Test Suite Creation (NEW):**
   - Goal: Create complete Float32 test suite mirroring all Float64 tests
   - Time estimate: 20h (Conservative), 12h (Aggressive)
   - Breakdown:
     - Test Generator Development: 8h/5h
     - Test Generation Execution: 4h/2h
     - Test Validation: 6h/4h
     - Documentation: 2h/1h

**Total Timeline Update:**
- **Before:** 129h (Conservative), 84h (Aggressive)
- **After (with Phase 3A):** 149h (Conservative), 96h (Aggressive)
- **Increase:** +20h (Conservative), +12h (Aggressive)

**Success Criteria for Test Parallelism:**
- ✅ All 129 Float64 tests have Float32 counterparts (1:1 mapping)
- ✅ All Float32 test files compile without errors
- ✅ Test naming conventions followed systematically
- ✅ Tolerance constants adjusted systematically
- ✅ Pass rate for Float32 tests: > 95%
- ✅ CI/CD integration with differential tests

**Documentation Updates:**
- New section: "## Test Parallelism: Float64 → Float32 Test Duplication" (~420 lines)
- Detailed test file analysis tables
- Complete test generator implementation examples
- Differential testing code examples
- Phase 3A breakdown with time estimates
- TODO_FLOAT32.md: ~4400 → ~4820 lines

**Version:** 2.6 (2025-10-20) - Test Parallelism Strategy

---

**Version:** 2.5 (2025-10-20) - Critical Findings + INumberBase<TPrecision> Constraint + Comprehensive Testing Strategy

---

## Changelog v2.5

**Comprehensive Testing Strategy Added:**

After user request for detailed test planning, a complete testing strategy was developed covering all phases:

**Test Philosophy:**
- "Test Early, Test Often, Test Smart"
- TDD approach with tests written DURING implementation
- Test pyramid: 30% Unit, 40% Component, 20% Integration, 10% E2E

**4 Test Categories Defined:**
1. **Unit Tests (Fine-Grained):** Individual functions/methods
   - Example: LiteralRewriter conversion functions
   - Time allocation: ~30% of test time

2. **Component Tests (Medium-Grained):** Complete modules with dependencies
   - Example: Full Rewriter classes with real SyntaxTrees
   - Time allocation: ~40% of test time

3. **Integration Tests (Coarse-Grained):** Multiple components together
   - Example: Full generator pipeline on mini project
   - Time allocation: ~20% of test time

4. **End-to-End Tests (Full-Stack):** Complete layer conversions
   - Example: Full Algebra layer (329 files)
   - Time allocation: ~10% of test time

**Phase-Specific Test Strategies:**
- **Phase 0:** 10h testing (18% of 56h) - Test after EACH sub-phase (0.1-0.7)
  - Acceptance criteria: ALL 1153 existing tests still pass (no regressions!)
  - Verify INumberBase<TPrecision> constraint
  - Verify Float32 uses correct epsilon (1e-7f)

- **Phase 1:** 10h testing (26% of 39h) - Test each component + integration
  - Test all 7 Rewriters independently
  - Test 5-phase validation pipeline
  - Integration test on mini project (5-10 files)

- **Phase 1A:** 3h testing (33% of 9h) - Float32 code generation
  - Verify Math→MathF conversions
  - Test generic evaluation with Float32

- **Phase 2:** 7h testing (41% of 17h) - Layer-by-layer validation
  - Compilation tests (required for all layers)
  - Smoke tests (required for all layers)
  - Differential tests (required for Modeling/CGA only)
  - Tolerance: 1e-6f for Float32 vs Float64 comparisons

- **Phase 3:** 8h testing (100% of 8h) - Final comprehensive validation
  - Full compilation (755-805 files)
  - Performance benchmarks (Float32 vs Float64)
  - Regression tests (1153 tests)
  - Differential tests on critical paths

**Test Code Examples:**
- ~700 lines of concrete test code examples provided
- Covers all test categories with real implementation patterns
- Includes acceptance criteria tests for each phase

**Test Infrastructure:**
- Detailed test project structure
- CI/CD integration with GitHub Actions
- Test categories for filtering (Unit, Component, Integration, Smoke, Differential)

**Test Time Summary:**
- Total test time: **38h out of 129h** (29% dedicated to testing)
- Phase breakdown documented with specific time allocations
- Continuous testing guidelines (TDD, before commit, before release)

**Timeline Impact:**
- Phase 0: 56h (includes 10h testing)
- Phase 1: 39h (includes 10h testing)
- Phase 1A: 9h (includes 3h testing)
- Phase 2: 17h (includes 7h testing)
- Phase 3: 8h (100% testing)
- **Total: 129h** (conservative estimate, includes all testing)

**Documentation:**
- New section: "## Comprehensive Testing Strategy" (~750 lines)
- Test pyramid diagram
- Phase-specific test tables with acceptance criteria
- Performance benchmark examples
- Test infrastructure requirements
- CI/CD configuration
- TODO_FLOAT32.md: ~2900 → ~3700 Zeilen

**Key Insight:** ~30% of project time dedicated to testing ensures:
- Zero regressions (all 1153 tests must still pass)
- High confidence in generated code quality
- Early detection of conversion issues
- Performance validation

**Version:** 2.5 (2025-10-20)

---

## Changelog v2.4

**INumberBase<TPrecision> Constraint Decision:**
- **Analysis:** Evaluated whether `IScalarProcessor<T, TPrecision>` should have generic constraint
- **Result:** ✅ YES - Use `where TPrecision : struct, INumberBase<TPrecision>`
- **Rationale:**
  - Type safety: Prevents invalid types like `IScalarProcessor<double, string>`
  - Consistency: Matches Finding 1 extension methods constraint
  - Enables generic numeric operations: `TPrecision.IsNaN()`, `TPrecision.Abs()`, etc.
  - No false restrictions: `T` remains unconstrained (symbolic processors work)
  - Future-proof: Supports float, double, decimal, Half
- **Verified:** All existing implementations (Float64, Float32, ERational, MetaExpression, etc.) work with constraint
- **Requirement:** .NET 7+ (user approved)

**Critical Findings Discovery (Post-Planning):**

After detailed code review of implementation files, **3 CRITICAL issues** were discovered that were not accounted for in previous planning versions (v2.0-v2.3):

**Finding 1: ScalarProcessorNumberUtils.cs - Missing Scope ⚠️ CRITICAL**
- **File:** `ScalarProcessorNumberUtils.cs` (402 lines, 90+ extension methods)
- **Problem:** ALL methods use hardcoded `ToFloat64()` and `double.*` static methods
- **Impact:** This file was NOT mentioned in original Phase 0 scope
- **Solution:** Refactor all 90+ methods to generic `<T, TPrecision>` with `INumberBase<TPrecision>` constraint
- **Requirements:** .NET 7+ for `INumberBase<T>` interface (approved by user)
- **Time Impact:** +8-10 hours to Phase 0

**Finding 2: Default Parameter Precision Values ⚠️ MEDIUM**
- **Files:** `Float64Utils.cs`, `Float64ArrayUtils.cs`, and many utility classes
- **Problem:** Default parameters like `epsilon = 1e-13` need semantic adjustment, not just type conversion
- **Example:** `1e-13` → `1e-13f` ❌ (too small for float!) → Should be `1e-7f` ✅
- **Solution:** Extend LiteralRewriter with `VisitParameter()` to handle default values semantically
- **Time Impact:** +4.5-5.5 hours to Phase 1

**Finding 3: LiteralRewriter Precision Scaling Heuristic is WRONG ⚠️ CRITICAL**
- **Original Plan (v2.2):** "Scale exponents by ~5" (e.g., 1e-16 → 1e-11f)
- **Problem 1:** 1e-11f is STILL too small for float (practical epsilon: 1e-7f)
- **Problem 2:** Scaling ALL literals breaks normal constants (e.g., Pi = 3.14159 → 314159.0f ❌)
- **Solution:** Category-based scaling with clamping:
  - Category 1: Ultra-small epsilons (< 1e-10) → Clamp to 1e-7f
  - Category 2-4: Normal values (1e-10 to float.MaxValue) → Direct conversion
  - Category 5: Out of range (> float.MaxValue) → Clamp to float.MaxValue
- **Time Impact:** +6 hours to Phase 1

**Shared Implementation:**
- Finding 2 and Finding 3 share the same `ConvertLiteralToFloat32Semantic()` method
- Actual overlap reduces combined time by ~0.5h
- Total Finding 2 + 3 impact: +10-11h (not +10.5-11.5h separately)

**Timeline Updates:**
- **Phase 0:** 46h → **54-56h** (Conservative), 25h → **32-34h** (Aggressive)
- **Phase 1:** 28h → **38-39h** (Conservative), 18h → **27-28h** (Aggressive)
- **Total Project:** 108h → **124-127h** (Conservative), 63h → **76-79h** (Aggressive)

**Documentation:**
- New section: "## Critical Findings (Post-Planning Discovery)" added with full implementation details
- All 3 findings documented with:
  - Problem description
  - Code examples (before/after)
  - Approved solutions
  - Time impact breakdown
  - Conversion tables (Finding 3)
- INumberBase<TPrecision> constraint analysis and decision documented in Phase 0.1
- Verified all existing implementations work with constraint (table included)
- TODO_FLOAT32.md: ~2400 → ~2900 Zeilen

**Version:** 2.4 (2025-10-20) - Critical Findings + INumberBase<TPrecision> Constraint

---

## Changelog v2.3

**Validation Strategy Overhauled:**
- **Phase 2 HINZUGEFÜGT:** Transformation Completeness Validation (NEW!)
  - Prüft unconverted 'double' keywords, Float64 identifiers, lost trivia
  - Catches Rewriter bugs BEFORE Compilation phase
  - Vollständig implementiert mit Code-Beispielen
- **Phase 3 KORRIGIERT:** Compilation Validation (CRITICAL FIX!)
  - Original: Included Float64 DLLs as references ❌ (false positives!)
  - Fixed: ONLY System references, NO Float64 DLLs ✅
  - Prevents generated code from accidentally using Float64 types
- **Phase 4 KONKRETISIERT:** Semantic Validation
  - 4.1 Type Consistency Validation (mit Code)
  - 4.2 API Compatibility Validation (mit Code)
  - 4.3 Member Preservation Validation (mit Code)
  - 4.4 Generic Constraints Validation (mit Code)
- **Phase 5 KONKRETISIERT:** Cross-Reference Validation
  - 5.1 Identifier Scan für Float64-References
  - 5.2 Generic Arguments Check für 'double'
  - 5.3 Math vs MathF Usage Check
  - 5.4 Literal Suffix Check (d → f)
- **Pipeline-Diagramm:** Visualisierung der 5 Phasen
- **Validation Report Models:** ValidationError, ValidationWarning, ValidationSeverity
- **Estimated Implementation Time:** 15-20 Stunden für komplette Validation

**Umfang:**
- +1100 Zeilen Code-Beispiele und Dokumentation
- TODO_FLOAT32.md: ~1900 → ~2400 Zeilen
- Alle Validationen mit vollständiger Implementation dokumentiert

**Gesamt-Timeline (v2.3 - Before Critical Findings):**
- Phase 1: 28h → **30h** (+2h für erweiterte Validation)
- Conservative: 108h → **110h** (≈ 13-14 Arbeitstage)
- Aggressive: 63h → **65h** (≈ 8 Arbeitstage)

---

## Changelog v2.1

**Phase 0 Updates:**
- Extended scope: ~50-75 files (not 20-30)
- XGaProcessor<T, TPrecision> decision documented
- Timeline: 46h Conservative (not 12h), 25h Aggressive (not 8h)

**Phase 1 Updates:**
- GenericParameterRewriter: Hybrid-Ansatz (Semantische Analyse + String-Ersetzung)
- Timeline: 28h Conservative (not 24h), 18h Aggressive (not 16h)

**Phase 1A Updates:**
- Generic Evaluation Support erweitert (6 Dateien, 9h statt 4h)
- Vollständig dokumentiert mit Beispielen

**Phasen-Abhängigkeiten:**
- Explizites Diagramm hinzugefügt
- Sequenzielle Ausführung dokumentiert
- Phase 0 blockiert Phase 1 (GenericParameterRewriter braucht neues Interface)

**Edge Cases:**
- Extension Methods Verifikation hinzugefügt (durch existierende Rewriters abgedeckt)

**Gesamt-Timeline:**
- Conservative: 108h (≈ 13-14 Arbeitstage)
- Aggressive: 63h (≈ 8 Arbeitstage)
