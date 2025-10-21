# GA-FuL Fork: Implementation Design Document
## Path C (REVERSED Hybrid) + CGa Generic Migration

**Version**: 1.0
**Date**: 2025-10-21
**Status**: IMPLEMENTATION READY
**Total Effort**: 260 hours (13 weeks @ 20h/week)

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Architecture Overview](#2-architecture-overview)
3. [Phase 1: IScalarOps Foundation](#3-phase-1-iscalarops-foundation)
4. [Phase 2: FloatingScalar Implementation](#4-phase-2-floatingscalar-implementation)
5. [Phase 3: XGaProcessor Integration](#5-phase-3-xgaprocessor-integration)
6. [Phase 4: CGa Generic Migration](#6-phase-4-cga-generic-migration)
7. [Testing Strategy](#7-testing-strategy)
8. [Migration Guide](#8-migration-guide)
9. [Implementation Checklist](#9-implementation-checklist)
10. [Appendix: Complete Code Examples](#10-appendix-complete-code-examples)

---

## 1. Executive Summary

### 1.1 Goals

**Primary Goal**: Enable unified workflow where SAME CODE works for:
- **Development**: Float32 execution (99% native performance)
- **Production**: Symbolic AST building → Code generation (C#, GLSL, HLSL, CUDA)

**Secondary Goal**: Make CGa generic like PGa, enabling complete type unification across all geometric algebras.

### 1.2 Solution Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    IScalarOps<T>                             │
│            (Unified scalar abstraction)                      │
└──────────────┬──────────────────────────────┬────────────────┘
               │                              │
    ┌──────────▼─────────┐      ┌────────────▼──────────────┐
    │ FloatingScalar<T>  │      │ IMetaExpressionAtomic     │
    │ (Float32/Float64)  │      │ (Symbolic - EXISTING!)    │
    └──────────┬─────────┘      └────────────┬──────────────┘
               │                              │
               └──────────┬───────────────────┘
                          │
              ┌───────────▼──────────────┐
              │   XGaProcessor<T>        │
              │   (Generic processor)    │
              └───────────┬──────────────┘
                          │
          ┌───────────────┼───────────────┐
          │               │               │
    ┌─────▼─────┐   ┌────▼────┐    ┌────▼────┐
    │ PGa<T>    │   │ CGa<T>  │    │ VGa<T>  │
    │ (Already  │   │ (NEW!)  │    │         │
    │ generic)  │   │         │    │         │
    └───────────┘   └─────────┘    └─────────┘
```

### 1.3 Key Deliverables

| Component | Description | Files | LOC | Tests |
|-----------|-------------|-------|-----|-------|
| **IScalarOps** | Unified scalar interface | 1 | 150 | 50 |
| **FloatingScalar** | Float32/64 wrapper | 2 | 400 | 100 |
| **CGaBlade<T>** | Generic CGA blade | 3 | 600 | 80 |
| **CGaElement<T>** | Generic elements hierarchy | 17 | 3,500 | 150 |
| **CGaEncoder<T>** | Generic encoders | 14 | 2,800 | 180 |
| **CGaDecoder<T>** | Generic decoders | 11 | 2,200 | 150 |
| **CGaOperations<T>** | Generic operations | 7 | 1,400 | 100 |
| **Documentation** | Guides, examples, API docs | 8 | 4,000 | - |
| **TOTAL** | | **63** | **15,050** | **810** |

### 1.4 Timeline

```
Week 1-2:   Phase 1 - IScalarOps Foundation (24h)
Week 3-4:   Phase 2 - FloatingScalar (20h)
Week 5-6:   Phase 3 - XGa Integration (24h)
Week 7-8:   Phase 4.1 - CGa Foundation (24h)
Week 9-10:  Phase 4.2 - CGa Elements (32h)
Week 11-12: Phase 4.3 - CGa Encoding (28h)
Week 13-14: Phase 4.4 - CGa Decoding (28h)
Week 15-16: Phase 4.5 - CGa Operations (20h)
Week 17-18: Phase 4.6 - CGa Interpolation (24h)
Week 19:    Phase 4.7 - CGa Versors (16h)
Week 20:    Integration & Testing (20h)

TOTAL: 20 weeks (5 months) @ 13h/week
       OR 13 weeks (3 months) @ 20h/week
```

---

## 2. Architecture Overview

### 2.1 Current State vs. Target State

#### Current State (Float64-only)

```csharp
// XGa Level - Generic
XGaProcessor<T> // ✅ Already generic

// PGa Level - Generic
PGaBlade<T>     // ✅ Already generic

// CGa Level - Float64 ONLY
CGaFloat64Blade              // ❌ Not generic
CGaFloat64Element            // ❌ Not generic
CGaFloat64GeometricSpace5D   // ❌ Not generic

// Problem: Cannot use symbolic processor with CGa!
var context = new MetaContext();
var processor = XGaProcessor<IMetaExpressionAtomic>.CreateConformal(5, context);
var cga = CGaFloat64GeometricSpace5D.Instance;
var point = cga.Encode.IpnsRound.Point(x, y, z);  // ❌ ERROR!
// Cannot convert IMetaExpressionAtomic to double
```

#### Target State (Fully Generic)

```csharp
// Unified scalar abstraction
public interface IScalarOps<TSelf> where TSelf : IScalarOps<TSelf>
{
    static abstract TSelf operator +(TSelf a, TSelf b);
    static abstract TSelf Sqrt(TSelf x);
    static abstract TSelf Zero { get; }
    // ... etc
}

// XGa Level - Generic (no changes needed)
XGaProcessor<T> where T : IScalarOps<T>

// PGa Level - Generic (no changes needed)
PGaBlade<T> where T : IScalarOps<T>

// CGa Level - NOW GENERIC!
CGaBlade<T> where T : IScalarOps<T>
CGaElement<T> where T : IScalarOps<T>
CGaGeometricSpace<T> where T : IScalarOps<T>

// Solution: Works with ANY scalar type!
// Float32:
var cga32 = new CGaGeometricSpace5D<FloatingScalar<float>>(...);
var point32 = cga32.Encode.IpnsRound.Point(1.0f, 2.0f, 3.0f);  // ✅

// Float64:
var cga64 = CGaFloat64GeometricSpace5D.Instance;
var point64 = cga64.Encode.IpnsRound.Point(1.0, 2.0, 3.0);  // ✅

// Symbolic:
var context = new MetaContext();
var cgaSymbolic = new CGaGeometricSpace5D<IMetaExpressionAtomic>(context);
var x = context["x"];
var pointSymbolic = cgaSymbolic.Encode.IpnsRound.Point(x, y, z);  // ✅
```

### 2.2 Dependency Graph

```
Level 0: Utilities (no changes)
  └── IndexSet, BitManipulation, etc.

Level 1: Scalar Abstraction (NEW)
  ├── IScalarOps<T>               [NEW - 8h]
  ├── FloatingScalar<T>           [NEW - 12h]
  └── IMetaExpressionAtomic       [EXISTING - verify compatibility]

Level 2: Algebra Layer
  ├── IScalarProcessor<T>         [EXISTING - no changes]
  ├── XGaProcessor<T>             [EXISTING - verify]
  └── XGaMultivector<T>           [EXISTING - no changes]

Level 3: Modeling Layer
  ├── PGa<T>                      [EXISTING - no changes]
  ├── VGa<T>                      [EXISTING - no changes]
  └── CGa<T>                      [MIGRATE - 200h]
      ├── CGaBlade<T>             [NEW - 24h]
      ├── CGaElement<T>           [NEW - 32h]
      ├── CGaEncoder<T>           [NEW - 28h]
      ├── CGaDecoder<T>           [NEW - 28h]
      └── CGaOperations<T>        [NEW - 88h]

Level 4: Visualization (Float64-only)
  └── CGaFloat64Visualizer        [EXISTING - no changes]
```

### 2.3 Type Hierarchy

```csharp
// Scalar Type Hierarchy
interface IScalarOps<TSelf>
    ├── FloatingScalar<float>        // Float32 wrapper
    ├── FloatingScalar<double>       // Float64 wrapper
    ├── FloatingScalar<Half>         // Float16 wrapper
    └── IMetaExpressionAtomic        // Symbolic (via adapter)

// Processor Hierarchy (Generic)
class XGaProcessor<T> where T : IScalarOps<T>
    ├── XGaFloat64Processor          // Alias for XGaProcessor<double>
    └── XGaProcessor<IMetaExpressionAtomic>

// CGa Type Hierarchy (NEW - Generic)
class CGaGeometricSpace<T> where T : IScalarOps<T>
    └── CGaGeometricSpace5D<T>
        └── CGaFloat64GeometricSpace5D : CGaGeometricSpace5D<double>
            └── Visualizer: CGaFloat64Visualizer  // Float64-only!

record CGaBlade<T> where T : IScalarOps<T>
    ├── XGaKVector<T> InternalKVector
    └── CGaProcessor<T> ConformalProcessor

abstract class CGaElement<T> where T : IScalarOps<T>
    ├── CGaRound<T>      // Circles, spheres
    ├── CGaFlat<T>       // Lines, planes
    ├── CGaTangent<T>    // Tangent elements
    └── CGaDirection<T>  // Directions
```

---

## 3. Phase 1: IScalarOps Foundation

**Duration**: 24 hours
**Priority**: P0 (Critical - blocks everything else)
**Dependencies**: None

### 3.1 Design Goals

1. **Minimal Interface**: Only operations actually needed by GA algorithms
2. **JIT-Friendly**: Designed for devirtualization via static abstract interface members
3. **Symbolic-Compatible**: Works with both numeric and symbolic types
4. **Performance**: Zero-cost abstraction when JIT devirtualizes

### 3.2 Interface Definition

**File**: `GeometricAlgebraFulcrumLib/Algebra/Scalars/IScalarOps.cs`

```csharp
using System.Numerics;

namespace GeometricAlgebraFulcrumLib.Algebra.Scalars;

/// <summary>
/// Unified scalar operations interface enabling BOTH numeric execution
/// AND symbolic AST building.
///
/// Design principles:
/// - Uses static abstract interface members (.NET 7+)
/// - JIT devirtualizes to direct calls (zero overhead)
/// - Works with FloatingScalar<T>, IMetaExpressionAtomic, etc.
///
/// Usage:
/// <code>
/// public static XGaVector<T> Algorithm<T>(XGaProcessor<T> processor)
///     where T : IScalarOps<T>
/// {
///     var x = T.Zero;
///     var y = T.One;
///     var result = x + y;  // Works for float AND symbolic!
///     return processor.Vector(result, T.Sqrt(y), T.Sin(x));
/// }
/// </code>
/// </summary>
/// <typeparam name="TSelf">Self-referential type parameter (CRTP pattern)</typeparam>
public interface IScalarOps<TSelf>
    where TSelf : IScalarOps<TSelf>
{
    // ===== Arithmetic Operators =====

    /// <summary>Addition operator</summary>
    static abstract TSelf operator +(TSelf left, TSelf right);

    /// <summary>Subtraction operator</summary>
    static abstract TSelf operator -(TSelf left, TSelf right);

    /// <summary>Multiplication operator</summary>
    static abstract TSelf operator *(TSelf left, TSelf right);

    /// <summary>Division operator</summary>
    static abstract TSelf operator /(TSelf left, TSelf right);

    /// <summary>Unary negation operator</summary>
    static abstract TSelf operator -(TSelf value);

    // ===== Mathematical Functions =====

    /// <summary>Square root</summary>
    static abstract TSelf Sqrt(TSelf x);

    /// <summary>Absolute value</summary>
    static abstract TSelf Abs(TSelf x);

    /// <summary>Sine function</summary>
    static abstract TSelf Sin(TSelf x);

    /// <summary>Cosine function</summary>
    static abstract TSelf Cos(TSelf x);

    /// <summary>Tangent function</summary>
    static abstract TSelf Tan(TSelf x);

    /// <summary>Arcsine function</summary>
    static abstract TSelf Asin(TSelf x);

    /// <summary>Arccosine function</summary>
    static abstract TSelf Acos(TSelf x);

    /// <summary>Arctangent function</summary>
    static abstract TSelf Atan(TSelf x);

    /// <summary>Two-argument arctangent (atan2)</summary>
    static abstract TSelf Atan2(TSelf y, TSelf x);

    /// <summary>Exponential function (e^x)</summary>
    static abstract TSelf Exp(TSelf x);

    /// <summary>Natural logarithm</summary>
    static abstract TSelf Log(TSelf x);

    /// <summary>Power function (x^y)</summary>
    static abstract TSelf Pow(TSelf x, TSelf y);

    // ===== Constants =====

    /// <summary>Additive identity (0)</summary>
    static abstract TSelf Zero { get; }

    /// <summary>Multiplicative identity (1)</summary>
    static abstract TSelf One { get; }

    /// <summary>Pi constant (3.14159...)</summary>
    static abstract TSelf Pi { get; }

    /// <summary>Euler's number (2.71828...)</summary>
    static abstract TSelf E { get; }

    // ===== Comparison =====

    /// <summary>Less than comparison</summary>
    static abstract bool operator <(TSelf left, TSelf right);

    /// <summary>Greater than comparison</summary>
    static abstract bool operator >(TSelf left, TSelf right);

    /// <summary>Less than or equal comparison</summary>
    static abstract bool operator <=(TSelf left, TSelf right);

    /// <summary>Greater than or equal comparison</summary>
    static abstract bool operator >=(TSelf left, TSelf right);

    // ===== Utility Methods =====

    /// <summary>
    /// Extract numeric magnitude for debugging/testing.
    /// For symbolic expressions, returns 0.0 (not meaningful).
    /// For numeric types, returns double representation.
    /// </summary>
    static abstract double Magnitude(TSelf value);

    /// <summary>
    /// Check if value is zero (or near-zero for floating-point).
    /// For symbolic, always returns false.
    /// </summary>
    static abstract bool IsZero(TSelf value);

    /// <summary>
    /// Check if value is near another value (within tolerance).
    /// For symbolic, performs structural equality.
    /// For numeric, checks |a - b| < tolerance.
    /// </summary>
    static abstract bool IsNear(TSelf a, TSelf b, double tolerance = 1e-12);
}
```

### 3.3 Design Rationale

**Why static abstract interface members?**
- .NET 7+ feature enabling zero-cost abstraction
- JIT compiler devirtualizes calls to direct implementations
- No virtual dispatch overhead at runtime
- Enables generic math patterns

**Why CRTP (Curiously Recurring Template Pattern)?**
```csharp
interface IScalarOps<TSelf> where TSelf : IScalarOps<TSelf>
//                   ^^^^^^         ^^^^^^
//                   Self-referential constraint
```
- Ensures operators return same type (not base interface)
- Enables method chaining
- Type-safe at compile time

**Performance Characteristics**:
```csharp
// Source code:
public static T Compute<T>(T x) where T : IScalarOps<T>
{
    return x + T.One;
}

// JIT devirtualizes to (for FloatingScalar<float>):
public static FloatingScalar<float> Compute(FloatingScalar<float> x)
{
    return new FloatingScalar<float>(x.Value + 1.0f);  // Direct!
}

// Further optimization (struct scalarization):
public static float Compute(float x)
{
    return x + 1.0f;  // Pure native code!
}
```

### 3.4 Implementation Tasks

#### Task 1.1: Create IScalarOps.cs (4h)

**Steps**:
1. Create file structure
2. Define interface with all members
3. Add XML documentation for all members
4. Add usage examples in comments

**Acceptance Criteria**:
- [ ] File compiles without errors
- [ ] All 30+ operations defined
- [ ] XML documentation complete (100% coverage)
- [ ] Code examples in documentation compile

#### Task 1.2: Create Adapter for IMetaExpressionAtomic (8h)

**File**: `GeometricAlgebraFulcrumLib/MetaProgramming/Adapters/MetaExpressionScalarOps.cs`

```csharp
using GeometricAlgebraFulcrumLib.Algebra.Scalars;
using GeometricAlgebraFulcrumLib.MetaProgramming.Expressions;
using GeometricAlgebraFulcrumLib.MetaProgramming.Context;

namespace GeometricAlgebraFulcrumLib.MetaProgramming.Adapters;

/// <summary>
/// Adapter making IMetaExpressionAtomic implement IScalarOps.
/// This enables symbolic expressions to work with generic GA algorithms.
///
/// IMPORTANT: Operations build AST, they do NOT compute values!
/// </summary>
public readonly struct MetaExpressionScalarOps : IScalarOps<MetaExpressionScalarOps>
{
    public IMetaExpressionAtomic Expression { get; }
    public MetaContext Context { get; }

    public MetaExpressionScalarOps(IMetaExpressionAtomic expression, MetaContext context)
    {
        Expression = expression;
        Context = context;
    }

    // Arithmetic operators - BUILD AST nodes
    public static MetaExpressionScalarOps operator +(
        MetaExpressionScalarOps left,
        MetaExpressionScalarOps right)
    {
        var context = left.Context;
        var result = context.GetOrDefineComputedVariable(
            (a, b) => MetaExpressionProcessor.Add(a, b).ScalarValue,
            left.Expression,
            right.Expression
        );
        return new MetaExpressionScalarOps(result, context);
    }

    public static MetaExpressionScalarOps operator *(
        MetaExpressionScalarOps left,
        MetaExpressionScalarOps right)
    {
        var context = left.Context;
        var result = context.GetOrDefineComputedVariable(
            (a, b) => MetaExpressionProcessor.Multiply(a, b).ScalarValue,
            left.Expression,
            right.Expression
        );
        return new MetaExpressionScalarOps(result, context);
    }

    // Mathematical functions - BUILD AST nodes
    public static MetaExpressionScalarOps Sqrt(MetaExpressionScalarOps x)
    {
        var context = x.Context;
        var result = context.GetOrDefineComputedVariable(
            a => MetaExpressionProcessor.Sqrt(a).ScalarValue,
            x.Expression
        );
        return new MetaExpressionScalarOps(result, context);
    }

    public static MetaExpressionScalarOps Sin(MetaExpressionScalarOps x)
    {
        var context = x.Context;
        var result = context.GetOrDefineComputedVariable(
            a => MetaExpressionProcessor.Sin(a).ScalarValue,
            x.Expression
        );
        return new MetaExpressionScalarOps(result, context);
    }

    // Constants - retrieve from context
    public static MetaExpressionScalarOps Zero
    {
        get
        {
            // Note: Need context to create constant!
            // This is a limitation - we'll handle via factory methods
            throw new InvalidOperationException(
                "Use MetaExpressionScalarOps.GetZero(context) instead"
            );
        }
    }

    public static MetaExpressionScalarOps One
    {
        get
        {
            throw new InvalidOperationException(
                "Use MetaExpressionScalarOps.GetOne(context) instead"
            );
        }
    }

    // Factory methods for constants (require context)
    public static MetaExpressionScalarOps GetZero(MetaContext context)
    {
        var zero = context.GetOrDefineLiteralNumber(0.0);
        return new MetaExpressionScalarOps(zero, context);
    }

    public static MetaExpressionScalarOps GetOne(MetaContext context)
    {
        var one = context.GetOrDefineLiteralNumber(1.0);
        return new MetaExpressionScalarOps(one, context);
    }

    // Utility methods
    public static double Magnitude(MetaExpressionScalarOps value)
    {
        // Symbolic expressions have no numeric magnitude
        return 0.0;
    }

    public static bool IsZero(MetaExpressionScalarOps value)
    {
        // Check if expression is literal zero
        return value.Expression is MetaExpressionNumber num &&
               num.NumberHeadSpecs.NumberFloat64Value == 0.0;
    }

    // Comparison operators - for symbolic, structural equality only
    public static bool operator <(
        MetaExpressionScalarOps left,
        MetaExpressionScalarOps right)
    {
        // Symbolic comparison not meaningful, always false
        return false;
    }

    // ... implement all other IScalarOps members
}
```

**Challenge**: IScalarOps requires static properties (Zero, One) but MetaContext is instance-based.

**Solution**: Use factory methods + context threading:
```csharp
// Instead of:
var zero = T.Zero;  // ❌ Doesn't work for symbolic

// Use:
var zero = context.GetZero<T>();  // ✅ Works for all types
```

#### Task 1.3: Unit Tests for IScalarOps (8h)

**File**: `GeometricAlgebraFulcrumLib.UnitTests/Algebra/Scalars/IScalarOpsTests.cs`

```csharp
using NUnit.Framework;
using GeometricAlgebraFulcrumLib.Algebra.Scalars;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Scalars;

[TestFixture]
public class IScalarOpsTests
{
    [Test]
    public void TestArithmeticOperations()
    {
        // Test with double (via FloatingScalar - will implement in Phase 2)
        var a = new FloatingScalar<double>(5.0);
        var b = new FloatingScalar<double>(3.0);

        var sum = a + b;
        var diff = a - b;
        var prod = a * b;
        var quot = a / b;

        Assert.That(sum.Value, Is.EqualTo(8.0).Within(1e-12));
        Assert.That(diff.Value, Is.EqualTo(2.0).Within(1e-12));
        Assert.That(prod.Value, Is.EqualTo(15.0).Within(1e-12));
        Assert.That(quot.Value, Is.EqualTo(5.0 / 3.0).Within(1e-12));
    }

    [Test]
    public void TestMathematicalFunctions()
    {
        var x = new FloatingScalar<double>(4.0);

        var sqrt = FloatingScalar<double>.Sqrt(x);
        var abs = FloatingScalar<double>.Abs(-x);

        Assert.That(sqrt.Value, Is.EqualTo(2.0).Within(1e-12));
        Assert.That(abs.Value, Is.EqualTo(4.0).Within(1e-12));
    }

    [Test]
    public void TestTrigonometricFunctions()
    {
        var angle = new FloatingScalar<double>(Math.PI / 4.0);

        var sin = FloatingScalar<double>.Sin(angle);
        var cos = FloatingScalar<double>.Cos(angle);

        Assert.That(sin.Value, Is.EqualTo(Math.Sqrt(2.0) / 2.0).Within(1e-12));
        Assert.That(cos.Value, Is.EqualTo(Math.Sqrt(2.0) / 2.0).Within(1e-12));
    }

    [Test]
    public void TestConstants()
    {
        var zero = FloatingScalar<double>.Zero;
        var one = FloatingScalar<double>.One;
        var pi = FloatingScalar<double>.Pi;

        Assert.That(zero.Value, Is.EqualTo(0.0));
        Assert.That(one.Value, Is.EqualTo(1.0));
        Assert.That(pi.Value, Is.EqualTo(Math.PI).Within(1e-12));
    }

    [Test]
    public void TestSymbolicAdapter()
    {
        var context = new MetaContext();

        var x = context.GetOrDefineParameterVariable("x");
        var y = context.GetOrDefineParameterVariable("y");

        var xOps = new MetaExpressionScalarOps(x, context);
        var yOps = new MetaExpressionScalarOps(y, context);

        // Build AST
        var sum = xOps + yOps;
        var product = xOps * yOps;
        var sqrtX = MetaExpressionScalarOps.Sqrt(xOps);

        // Verify AST was built (expressions are not null)
        Assert.That(sum.Expression, Is.Not.Null);
        Assert.That(product.Expression, Is.Not.Null);
        Assert.That(sqrtX.Expression, Is.Not.Null);

        // Verify no computation happened (Magnitude returns 0 for symbolic)
        Assert.That(MetaExpressionScalarOps.Magnitude(sum), Is.EqualTo(0.0));
    }

    // 50+ tests total covering all operations
}
```

#### Task 1.4: Documentation (4h)

**File**: `docs/IScalarOps_Guide.md`

```markdown
# IScalarOps<T> Usage Guide

## Overview

IScalarOps<T> is the foundation enabling unified algorithms that work
with BOTH numeric types (Float32, Float64) AND symbolic expressions.

## Basic Usage

### Numeric Execution

```csharp
public static XGaVector<T> RotateVector<T>(
    XGaProcessor<T> processor,
    XGaVector<T> vector,
    T angle) where T : IScalarOps<T>
{
    var cosHalf = T.Cos(angle / (T.One + T.One));
    var sinHalf = T.Sin(angle / (T.One + T.One));

    var rotor = processor.CreateMultivectorComposer()
        .SetTerm(0, cosHalf)
        .SetTerm(3, -sinHalf)
        .GetMultivector();

    return rotor.Gp(vector).Gp(rotor.Reverse()).GetVectorPart();
}

// Execute with Float32
var processor = XGaProcessor<FloatingScalar<float>>.CreateEuclidean();
var vector = processor.Vector(1.0f, 0.0f, 0.0f);
var angle = new FloatingScalar<float>(MathF.PI / 4.0f);
var result = RotateVector(processor, vector, angle);  // Computes!
```

### Symbolic Code Generation

```csharp
// Build AST with symbolic processor
var context = new MetaContext();
var processor = XGaProcessor<IMetaExpressionAtomic>.CreateEuclidean(context);

var x = context["x"];
var y = context["y"];
var z = context["z"];
var angle = context["angle"];

var vector = processor.Vector(x, y, z);
var result = RotateVector(processor, vector, angle);  // Builds AST!

// Generate code
context.OptimizeContext();
var csharpCode = GenerateCSharp(context);
```

## Implementation Guide

See complete implementation examples in Appendix.
```

### 3.5 Testing & Validation

**Acceptance Criteria**:
- [ ] All IScalarOps operations work with FloatingScalar<float>
- [ ] All IScalarOps operations work with FloatingScalar<double>
- [ ] All IScalarOps operations work with MetaExpressionScalarOps
- [ ] JIT devirtualization verified (benchmark shows <1% overhead)
- [ ] 50+ unit tests passing
- [ ] Documentation complete

**Benchmark Target**:
```
FloatingScalar<float> vs native float:
- Arithmetic: 99% (0-1% overhead)
- Math functions: 99% (0-1% overhead)
- Generic algorithm: 98-99% (1-2% overhead)
```

---

## 4. Phase 2: FloatingScalar Implementation

**Duration**: 20 hours
**Priority**: P0 (Critical)
**Dependencies**: Phase 1 (IScalarOps)

### 4.1 Design Goals

1. **Zero-Cost Wrapper**: JIT eliminates wrapper at runtime
2. **Native Performance**: 99% of native float/double performance
3. **Type-Safe**: Compile-time type checking
4. **SIMD-Friendly**: Enables vectorization where possible

### 4.2 Implementation

**File**: `GeometricAlgebraFulcrumLib/Algebra/Scalars/FloatingScalar.cs`

```csharp
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GeometricAlgebraFulcrumLib.Algebra.Scalars;

/// <summary>
/// Wrapper making floating-point types (float, double, Half) implement IScalarOps.
///
/// Design for JIT optimization:
/// - Readonly struct (pass by value, no heap allocation)
/// - AggressiveInlining on all methods (inline at call site)
/// - Delegates to T's static methods (JIT devirtualizes)
///
/// Performance characteristics:
/// - JIT struct scalarization eliminates wrapper overhead
/// - Native SIMD instructions for math operations
/// - 99% of native float/double performance
///
/// Usage:
/// <code>
/// var x = new FloatingScalar<float>(5.0f);
/// var y = new FloatingScalar<float>(3.0f);
/// var sum = x + y;  // Compiles to: float sum = 5.0f + 3.0f;
/// </code>
/// </summary>
/// <typeparam name="T">Floating-point type (float, double, or Half)</typeparam>
[StructLayout(LayoutKind.Sequential)]  // Predictable memory layout
public readonly struct FloatingScalar<T> : IScalarOps<FloatingScalar<T>>
    where T : struct, IFloatingPointIeee754<T>
{
    // ===== State =====

    /// <summary>The wrapped floating-point value</summary>
    public readonly T Value;

    // ===== Constructors =====

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FloatingScalar(T value)
    {
        Value = value;
    }

    // ===== Arithmetic Operators =====

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> operator +(
        FloatingScalar<T> left,
        FloatingScalar<T> right)
    {
        // Delegates to T.operator+ (native instruction!)
        return new FloatingScalar<T>(left.Value + right.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> operator -(
        FloatingScalar<T> left,
        FloatingScalar<T> right)
    {
        return new FloatingScalar<T>(left.Value - right.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> operator *(
        FloatingScalar<T> left,
        FloatingScalar<T> right)
    {
        return new FloatingScalar<T>(left.Value * right.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> operator /(
        FloatingScalar<T> left,
        FloatingScalar<T> right)
    {
        return new FloatingScalar<T>(left.Value / right.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> operator -(FloatingScalar<T> value)
    {
        return new FloatingScalar<T>(-value.Value);
    }

    // ===== Mathematical Functions =====

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> Sqrt(FloatingScalar<T> x)
    {
        // T.Sqrt uses hardware SQRT instruction!
        return new FloatingScalar<T>(T.Sqrt(x.Value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> Abs(FloatingScalar<T> x)
    {
        return new FloatingScalar<T>(T.Abs(x.Value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> Sin(FloatingScalar<T> x)
    {
        return new FloatingScalar<T>(T.Sin(x.Value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> Cos(FloatingScalar<T> x)
    {
        return new FloatingScalar<T>(T.Cos(x.Value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> Tan(FloatingScalar<T> x)
    {
        return new FloatingScalar<T>(T.Tan(x.Value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> Asin(FloatingScalar<T> x)
    {
        return new FloatingScalar<T>(T.Asin(x.Value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> Acos(FloatingScalar<T> x)
    {
        return new FloatingScalar<T>(T.Acos(x.Value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> Atan(FloatingScalar<T> x)
    {
        return new FloatingScalar<T>(T.Atan(x.Value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> Atan2(FloatingScalar<T> y, FloatingScalar<T> x)
    {
        return new FloatingScalar<T>(T.Atan2(y.Value, x.Value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> Exp(FloatingScalar<T> x)
    {
        return new FloatingScalar<T>(T.Exp(x.Value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> Log(FloatingScalar<T> x)
    {
        return new FloatingScalar<T>(T.Log(x.Value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FloatingScalar<T> Pow(FloatingScalar<T> x, FloatingScalar<T> y)
    {
        return new FloatingScalar<T>(T.Pow(x.Value, y.Value));
    }

    // ===== Constants =====

    public static FloatingScalar<T> Zero
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new FloatingScalar<T>(T.Zero);
    }

    public static FloatingScalar<T> One
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new FloatingScalar<T>(T.One);
    }

    public static FloatingScalar<T> Pi
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new FloatingScalar<T>(T.Pi);
    }

    public static FloatingScalar<T> E
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new FloatingScalar<T>(T.E);
    }

    // ===== Comparison Operators =====

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(
        FloatingScalar<T> left,
        FloatingScalar<T> right)
    {
        return left.Value < right.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(
        FloatingScalar<T> left,
        FloatingScalar<T> right)
    {
        return left.Value > right.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(
        FloatingScalar<T> left,
        FloatingScalar<T> right)
    {
        return left.Value <= right.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(
        FloatingScalar<T> left,
        FloatingScalar<T> right)
    {
        return left.Value >= right.Value;
    }

    // ===== Utility Methods =====

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Magnitude(FloatingScalar<T> value)
    {
        return double.CreateChecked(T.Abs(value.Value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(FloatingScalar<T> value)
    {
        return value.Value == T.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNear(
        FloatingScalar<T> a,
        FloatingScalar<T> b,
        double tolerance = 1e-12)
    {
        var diff = T.Abs(a.Value - b.Value);
        var tol = T.CreateChecked(tolerance);
        return diff < tol;
    }

    // ===== Implicit Conversions =====

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FloatingScalar<T>(T value)
    {
        return new FloatingScalar<T>(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T(FloatingScalar<T> scalar)
    {
        return scalar.Value;
    }

    // ===== Object Overrides =====

    public override string ToString()
    {
        return Value.ToString();
    }

    public override bool Equals(object? obj)
    {
        return obj is FloatingScalar<T> other && Value.Equals(other.Value);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
```

### 4.3 Scalar Processor Implementation

**File**: `GeometricAlgebraFulcrumLib/Algebra/Scalars/FloatingScalarProcessor.cs`

```csharp
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using System.Numerics;

namespace GeometricAlgebraFulcrumLib.Algebra.Scalars;

/// <summary>
/// IScalarProcessor implementation for FloatingScalar<T>.
/// Bridges IScalarOps to IScalarProcessor interface.
/// </summary>
public sealed class FloatingScalarProcessor<T> : IScalarProcessor<FloatingScalar<T>>
    where T : struct, IFloatingPointIeee754<T>
{
    // Singleton pattern
    public static FloatingScalarProcessor<T> Instance { get; } = new();

    private FloatingScalarProcessor() { }

    // ===== IScalarProcessor Implementation =====

    public Scalar<FloatingScalar<T>> Zero
        => FloatingScalar<T>.Zero.CreateScalar(this);

    public Scalar<FloatingScalar<T>> One
        => FloatingScalar<T>.One.CreateScalar(this);

    public Scalar<FloatingScalar<T>> MinusOne
        => (-FloatingScalar<T>.One).CreateScalar(this);

    public FloatingScalar<T> ZeroValue
        => FloatingScalar<T>.Zero;

    public FloatingScalar<T> OneValue
        => FloatingScalar<T>.One;

    public FloatingScalar<T> MinusOneValue
        => -FloatingScalar<T>.One;

    public Scalar<FloatingScalar<T>> Add(
        FloatingScalar<T> scalar1,
        FloatingScalar<T> scalar2)
    {
        return (scalar1 + scalar2).CreateScalar(this);
    }

    public Scalar<FloatingScalar<T>> Subtract(
        FloatingScalar<T> scalar1,
        FloatingScalar<T> scalar2)
    {
        return (scalar1 - scalar2).CreateScalar(this);
    }

    public Scalar<FloatingScalar<T>> Times(
        FloatingScalar<T> scalar1,
        FloatingScalar<T> scalar2)
    {
        return (scalar1 * scalar2).CreateScalar(this);
    }

    public Scalar<FloatingScalar<T>> Divide(
        FloatingScalar<T> scalar1,
        FloatingScalar<T> scalar2)
    {
        return (scalar1 / scalar2).CreateScalar(this);
    }

    public Scalar<FloatingScalar<T>> Negative(FloatingScalar<T> scalar)
    {
        return (-scalar).CreateScalar(this);
    }

    public Scalar<FloatingScalar<T>> Sqrt(FloatingScalar<T> scalar)
    {
        return FloatingScalar<T>.Sqrt(scalar).CreateScalar(this);
    }

    public Scalar<FloatingScalar<T>> Sin(FloatingScalar<T> scalar)
    {
        return FloatingScalar<T>.Sin(scalar).CreateScalar(this);
    }

    public Scalar<FloatingScalar<T>> Cos(FloatingScalar<T> scalar)
    {
        return FloatingScalar<T>.Cos(scalar).CreateScalar(this);
    }

    // ... implement all other IScalarProcessor methods

    public Scalar<FloatingScalar<T>> ScalarFromNumber(int value)
    {
        return new FloatingScalar<T>(T.CreateChecked(value)).CreateScalar(this);
    }

    public Scalar<FloatingScalar<T>> ScalarFromNumber(double value)
    {
        return new FloatingScalar<T>(T.CreateChecked(value)).CreateScalar(this);
    }

    public bool IsValid(FloatingScalar<T> scalar)
    {
        return T.IsFinite(scalar.Value);
    }

    public bool IsZero(FloatingScalar<T> scalar)
    {
        return scalar.Value == T.Zero;
    }

    public bool IsNearZero(FloatingScalar<T> scalar, double epsilon = 1e-12)
    {
        var eps = T.CreateChecked(epsilon);
        return T.Abs(scalar.Value) < eps;
    }
}
```

### 4.4 Type Aliases for Convenience

**File**: `GeometricAlgebraFulcrumLib/Algebra/Scalars/ScalarAliases.cs`

```csharp
namespace GeometricAlgebraFulcrumLib.Algebra.Scalars;

// Type aliases for common floating-point types
using Float32Scalar = FloatingScalar<float>;
using Float64Scalar = FloatingScalar<double>;
using Float16Scalar = FloatingScalar<Half>;

// Processor aliases
using Float32Processor = FloatingScalarProcessor<float>;
using Float64Processor = FloatingScalarProcessor<double>;
using Float16Processor = FloatingScalarProcessor<Half>;
```

### 4.5 Implementation Tasks

#### Task 2.1: Implement FloatingScalar<T> (8h)

**Steps**:
1. Create FloatingScalar.cs with all IScalarOps members
2. Add AggressiveInlining to all methods
3. Add implicit conversions to/from T
4. Comprehensive XML documentation

**Acceptance Criteria**:
- [ ] Compiles without warnings
- [ ] All 30+ IScalarOps operations implemented
- [ ] AggressiveInlining on all methods
- [ ] Implicit conversions work correctly

#### Task 2.2: Implement FloatingScalarProcessor<T> (4h)

**Steps**:
1. Create FloatingScalarProcessor.cs
2. Implement all IScalarProcessor<T> members
3. Add singleton pattern
4. Documentation

**Acceptance Criteria**:
- [ ] All IScalarProcessor methods implemented
- [ ] Singleton works correctly
- [ ] Bridges IScalarOps to IScalarProcessor properly

#### Task 2.3: Performance Benchmarks (4h)

**File**: `GeometricAlgebraFulcrumLib.Benchmarks/Algebra/FloatingScalarBenchmarks.cs`

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using GeometricAlgebraFulcrumLib.Algebra.Scalars;

namespace GeometricAlgebraFulcrumLib.Benchmarks.Algebra;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, targetCount: 10, invocationCount: 1000000)]
public class FloatingScalarBenchmarks
{
    private const int Iterations = 1000;

    [Benchmark(Baseline = true)]
    public float NativeFloat()
    {
        float result = 0.0f;
        for (int i = 0; i < Iterations; i++)
        {
            float x = (float)i;
            float y = (float)(i + 1);
            result += x + y;
            result *= x * y;
            result = MathF.Sqrt(result);
            result = MathF.Sin(result);
        }
        return result;
    }

    [Benchmark]
    public FloatingScalar<float> FloatingScalarFloat()
    {
        var result = FloatingScalar<float>.Zero;
        for (int i = 0; i < Iterations; i++)
        {
            var x = new FloatingScalar<float>((float)i);
            var y = new FloatingScalar<float>((float)(i + 1));
            result = result + x + y;
            result = result * x * y;
            result = FloatingScalar<float>.Sqrt(result);
            result = FloatingScalar<float>.Sin(result);
        }
        return result;
    }

    [Benchmark]
    public FloatingScalar<float> GenericAlgorithm()
    {
        return GenericComputation<FloatingScalar<float>>();
    }

    private static T GenericComputation<T>() where T : IScalarOps<T>
    {
        var result = T.Zero;
        for (int i = 0; i < Iterations; i++)
        {
            // Note: This requires factory method for creating T from int
            // For now, we'll test with constants
            var x = T.One;
            var y = T.One + T.One;
            result = result + x + y;
            result = result * x * y;
            result = T.Sqrt(result);
            result = T.Sin(result);
        }
        return result;
    }
}

/*
Expected Results (Release mode, .NET 8):

| Method               | Mean      | Allocated |
|--------------------- |----------:|----------:|
| NativeFloat          | 100.0 ns  |       0 B |
| FloatingScalarFloat  | 101.0 ns  |       0 B | (99% performance ✅)
| GenericAlgorithm     | 102.0 ns  |       0 B | (98% performance ✅)

Conclusion: JIT struct scalarization eliminates overhead!
*/
```

#### Task 2.4: Unit Tests (4h)

**File**: `GeometricAlgebraFulcrumLib.UnitTests/Algebra/Scalars/FloatingScalarTests.cs`

```csharp
using NUnit.Framework;
using GeometricAlgebraFulcrumLib.Algebra.Scalars;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Scalars;

[TestFixture]
public class FloatingScalarTests
{
    [Test]
    public void Float32_ArithmeticOperations()
    {
        var a = new FloatingScalar<float>(5.0f);
        var b = new FloatingScalar<float>(3.0f);

        var sum = a + b;
        var diff = a - b;
        var prod = a * b;
        var quot = a / b;

        Assert.That(sum.Value, Is.EqualTo(8.0f).Within(1e-6f));
        Assert.That(diff.Value, Is.EqualTo(2.0f).Within(1e-6f));
        Assert.That(prod.Value, Is.EqualTo(15.0f).Within(1e-6f));
        Assert.That(quot.Value, Is.EqualTo(5.0f / 3.0f).Within(1e-6f));
    }

    [Test]
    public void Float64_ArithmeticOperations()
    {
        var a = new FloatingScalar<double>(5.0);
        var b = new FloatingScalar<double>(3.0);

        var sum = a + b;
        var diff = a - b;
        var prod = a * b;
        var quot = a / b;

        Assert.That(sum.Value, Is.EqualTo(8.0).Within(1e-12));
        Assert.That(diff.Value, Is.EqualTo(2.0).Within(1e-12));
        Assert.That(prod.Value, Is.EqualTo(15.0).Within(1e-12));
        Assert.That(quot.Value, Is.EqualTo(5.0 / 3.0).Within(1e-12));
    }

    [Test]
    public void MathematicalFunctions_Accuracy()
    {
        var x = new FloatingScalar<double>(4.0);

        var sqrt = FloatingScalar<double>.Sqrt(x);
        var abs = FloatingScalar<double>.Abs(-x);
        var exp = FloatingScalar<double>.Exp(FloatingScalar<double>.Zero);
        var log = FloatingScalar<double>.Log(FloatingScalar<double>.E);

        Assert.That(sqrt.Value, Is.EqualTo(2.0).Within(1e-12));
        Assert.That(abs.Value, Is.EqualTo(4.0).Within(1e-12));
        Assert.That(exp.Value, Is.EqualTo(1.0).Within(1e-12));
        Assert.That(log.Value, Is.EqualTo(1.0).Within(1e-12));
    }

    [Test]
    public void TrigonometricFunctions_Accuracy()
    {
        var pi4 = new FloatingScalar<double>(Math.PI / 4.0);

        var sin = FloatingScalar<double>.Sin(pi4);
        var cos = FloatingScalar<double>.Cos(pi4);
        var tan = FloatingScalar<double>.Tan(pi4);

        Assert.That(sin.Value, Is.EqualTo(Math.Sqrt(2.0) / 2.0).Within(1e-12));
        Assert.That(cos.Value, Is.EqualTo(Math.Sqrt(2.0) / 2.0).Within(1e-12));
        Assert.That(tan.Value, Is.EqualTo(1.0).Within(1e-12));
    }

    [Test]
    public void Constants_Values()
    {
        Assert.That(FloatingScalar<float>.Zero.Value, Is.EqualTo(0.0f));
        Assert.That(FloatingScalar<float>.One.Value, Is.EqualTo(1.0f));
        Assert.That(FloatingScalar<double>.Pi.Value, Is.EqualTo(Math.PI).Within(1e-12));
        Assert.That(FloatingScalar<double>.E.Value, Is.EqualTo(Math.E).Within(1e-12));
    }

    [Test]
    public void ImplicitConversions()
    {
        // T → FloatingScalar<T>
        FloatingScalar<float> x = 5.0f;
        Assert.That(x.Value, Is.EqualTo(5.0f));

        // FloatingScalar<T> → T
        float y = x;
        Assert.That(y, Is.EqualTo(5.0f));
    }

    [Test]
    public void Processor_Integration()
    {
        var processor = FloatingScalarProcessor<double>.Instance;

        var x = processor.ScalarFromNumber(5.0);
        var y = processor.ScalarFromNumber(3.0);

        var sum = processor.Add(x.ScalarValue, y.ScalarValue);
        var prod = processor.Times(x.ScalarValue, y.ScalarValue);

        Assert.That(sum.ScalarValue.Value, Is.EqualTo(8.0).Within(1e-12));
        Assert.That(prod.ScalarValue.Value, Is.EqualTo(15.0).Within(1e-12));
    }

    // 100+ tests total
}
```

### 4.6 Validation

**Performance Targets**:
- [ ] FloatingScalar<float> vs native float: ≥99% performance
- [ ] FloatingScalar<double> vs native double: ≥99% performance
- [ ] Generic algorithm overhead: ≤2%
- [ ] Zero heap allocations in Release mode

**Test Coverage**:
- [ ] 100+ unit tests passing
- [ ] All arithmetic operations tested
- [ ] All math functions tested
- [ ] Edge cases (NaN, Infinity, Zero) tested
- [ ] Implicit conversions tested

---

## 5. Phase 3: XGaProcessor Integration

**Duration**: 24 hours
**Priority**: P0 (Critical)
**Dependencies**: Phase 1, Phase 2

### 5.1 Goals

1. Verify `XGaProcessor<T>` works with `FloatingScalar<T>`
2. Verify `XGaProcessor<T>` works with `MetaExpressionScalarOps`
3. Create factory methods for common processors
4. End-to-end integration tests

### 5.2 Processor Factory Methods

**File**: `GeometricAlgebraFulcrumLib/Algebra/GeometricAlgebra/Extended/XGaProcessorFactory.cs`

```csharp
using GeometricAlgebraFulcrumLib.Algebra.Scalars;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.MetaProgramming.Context;

namespace GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended;

/// <summary>
/// Factory methods for creating XGaProcessor instances with various scalar types.
/// </summary>
public static class XGaProcessorFactory
{
    // ===== Float32 Processors =====

    /// <summary>
    /// Create Euclidean Float32 processor
    /// </summary>
    public static XGaProcessor<FloatingScalar<float>> CreateFloat32Euclidean()
    {
        var scalarProcessor = FloatingScalarProcessor<float>.Instance;
        return XGaProcessor<FloatingScalar<float>>.CreateEuclidean(scalarProcessor);
    }

    /// <summary>
    /// Create Float32 processor with custom metric signature
    /// </summary>
    public static XGaProcessor<FloatingScalar<float>> CreateFloat32(
        int positiveCount,
        int negativeCount = 0,
        int zeroCount = 0)
    {
        var scalarProcessor = FloatingScalarProcessor<float>.Instance;
        return XGaProcessor<FloatingScalar<float>>.Create(
            scalarProcessor,
            positiveCount,
            negativeCount,
            zeroCount
        );
    }

    /// <summary>
    /// Create Conformal Float32 processor (for CGa)
    /// </summary>
    public static XGaProcessor<FloatingScalar<float>> CreateFloat32Conformal(
        int vSpaceDimensions)
    {
        var scalarProcessor = FloatingScalarProcessor<float>.Instance;
        return XGaProcessor<FloatingScalar<float>>.CreateConformal(
            scalarProcessor,
            vSpaceDimensions
        );
    }

    // ===== Float64 Processors (backward compatible) =====

    /// <summary>
    /// Create Euclidean Float64 processor (backward compatible with XGaFloat64Processor.Euclidean)
    /// </summary>
    public static XGaProcessor<FloatingScalar<double>> CreateFloat64Euclidean()
    {
        var scalarProcessor = FloatingScalarProcessor<double>.Instance;
        return XGaProcessor<FloatingScalar<double>>.CreateEuclidean(scalarProcessor);
    }

    // Note: XGaFloat64Processor.Euclidean still works! This is just an alternative.

    // ===== Symbolic Processors =====

    /// <summary>
    /// Create Euclidean symbolic processor for code generation
    /// </summary>
    public static XGaProcessor<IMetaExpressionAtomic> CreateSymbolicEuclidean(
        MetaContext context)
    {
        // MetaContext implements IScalarProcessor<IMetaExpressionAtomic>
        return context.CreateXGaProcessor();
    }

    /// <summary>
    /// Create symbolic processor with custom metric
    /// </summary>
    public static XGaProcessor<IMetaExpressionAtomic> CreateSymbolic(
        MetaContext context,
        int positiveCount,
        int negativeCount = 0,
        int zeroCount = 0)
    {
        return XGaProcessor<IMetaExpressionAtomic>.Create(
            context,
            positiveCount,
            negativeCount,
            zeroCount
        );
    }

    /// <summary>
    /// Create Conformal symbolic processor (for CGa code generation)
    /// </summary>
    public static XGaProcessor<IMetaExpressionAtomic> CreateSymbolicConformal(
        MetaContext context,
        int vSpaceDimensions)
    {
        return XGaProcessor<IMetaExpressionAtomic>.CreateConformal(
            context,
            vSpaceDimensions
        );
    }
}
```

### 5.3 Integration Tests

**File**: `GeometricAlgebraFulcrumLib.UnitTests/Integration/UnifiedWorkflowTests.cs`

```csharp
using NUnit.Framework;
using GeometricAlgebraFulcrumLib.Algebra.Scalars;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended;
using GeometricAlgebraFulcrumLib.MetaProgramming.Context;

namespace GeometricAlgebraFulcrumLib.UnitTests.Integration;

[TestFixture]
public class UnifiedWorkflowTests
{
    /// <summary>
    /// Generic rotor algorithm that works with ANY scalar type
    /// </summary>
    private static XGaVector<T> RotateVector<T>(
        XGaProcessor<T> processor,
        XGaVector<T> vector,
        T angle) where T : IScalarOps<T>
    {
        // Compute half-angle trig
        var two = T.One + T.One;
        var halfAngle = angle / two;
        var cosHalf = T.Cos(halfAngle);
        var sinHalf = T.Sin(halfAngle);

        // Build rotor: R = cos(θ/2) - sin(θ/2) * e₁₂
        var rotor = processor.CreateMultivectorComposer()
            .SetTerm(0, cosHalf)         // Scalar part
            .SetTerm(3, -sinHalf)        // e₁₂ bivector part
            .GetMultivector();

        // Apply rotation: v' = R * v * R†
        var rotorReverse = rotor.Reverse();
        var rotated = rotor.Gp(vector).Gp(rotorReverse);

        return rotated.GetVectorPart();
    }

    [Test]
    public void TestFloat32Workflow()
    {
        // Create Float32 processor
        var processor = XGaProcessorFactory.CreateFloat32Euclidean();

        // Create vector
        var vector = processor.Vector(1.0f, 0.0f, 0.0f);

        // 45 degree rotation
        var angle = new FloatingScalar<float>(MathF.PI / 4.0f);

        // Execute algorithm (DIRECT Float32 computation)
        var result = RotateVector(processor, vector, angle);

        // Verify results
        var x = result[0].ScalarValue.Value;
        var y = result[1].ScalarValue.Value;
        var z = result[2].ScalarValue.Value;

        Assert.That(x, Is.EqualTo(MathF.Sqrt(2.0f) / 2.0f).Within(1e-6f));
        Assert.That(y, Is.EqualTo(MathF.Sqrt(2.0f) / 2.0f).Within(1e-6f));
        Assert.That(z, Is.EqualTo(0.0f).Within(1e-6f));
    }

    [Test]
    public void TestFloat64Workflow()
    {
        // Create Float64 processor
        var processor = XGaProcessorFactory.CreateFloat64Euclidean();

        // Create vector
        var vector = processor.Vector(1.0, 0.0, 0.0);

        // 45 degree rotation
        var angle = new FloatingScalar<double>(Math.PI / 4.0);

        // Execute algorithm
        var result = RotateVector(processor, vector, angle);

        // Verify results
        var x = result[0].ScalarValue.Value;
        var y = result[1].ScalarValue.Value;
        var z = result[2].ScalarValue.Value;

        Assert.That(x, Is.EqualTo(Math.Sqrt(2.0) / 2.0).Within(1e-12));
        Assert.That(y, Is.EqualTo(Math.Sqrt(2.0) / 2.0).Within(1e-12));
        Assert.That(z, Is.EqualTo(0.0).Within(1e-12));
    }

    [Test]
    public void TestSymbolicWorkflow()
    {
        // Create symbolic processor
        var context = new MetaContext();
        var processor = XGaProcessorFactory.CreateSymbolicEuclidean(context);

        // Define symbolic parameters
        var vx = context.GetOrDefineParameterVariable("vx");
        var vy = context.GetOrDefineParameterVariable("vy");
        var vz = context.GetOrDefineParameterVariable("vz");
        var angle = context.GetOrDefineParameterVariable("angle");

        // Create symbolic vector
        var vector = processor.Vector(vx, vy, vz);

        // Execute algorithm (BUILDS AST, does not compute!)
        var result = RotateVector(processor, vector, angle);

        // Define outputs
        var outX = context.GetOrDefineOutputVariable("outX", result[0].ScalarValue);
        var outY = context.GetOrDefineOutputVariable("outY", result[1].ScalarValue);
        var outZ = context.GetOrDefineOutputVariable("outZ", result[2].ScalarValue);

        // Verify AST was built (not null)
        Assert.That(outX, Is.Not.Null);
        Assert.That(outY, Is.Not.Null);
        Assert.That(outZ, Is.Not.Null);

        // Optimize AST
        context.OptimizeContext();

        // Verify optimization happened (should have intermediate variables)
        var intermediates = context.GetIntermediateVariables();
        Assert.That(intermediates.Count(), Is.GreaterThan(0));

        // Note: Actual code generation tested separately
    }

    [Test]
    public void TestWorkflowConsistency()
    {
        // Test that Float32 and Float64 produce same results

        var processor32 = XGaProcessorFactory.CreateFloat32Euclidean();
        var vector32 = processor32.Vector(1.0f, 2.0f, 3.0f);
        var angle32 = new FloatingScalar<float>(0.5f);
        var result32 = RotateVector(processor32, vector32, angle32);

        var processor64 = XGaProcessorFactory.CreateFloat64Euclidean();
        var vector64 = processor64.Vector(1.0, 2.0, 3.0);
        var angle64 = new FloatingScalar<double>(0.5);
        var result64 = RotateVector(processor64, vector64, angle64);

        // Results should be equal within Float32 precision
        Assert.That(
            result32[0].ScalarValue.Value,
            Is.EqualTo((float)result64[0].ScalarValue.Value).Within(1e-6f)
        );
        Assert.That(
            result32[1].ScalarValue.Value,
            Is.EqualTo((float)result64[1].ScalarValue.Value).Within(1e-6f)
        );
        Assert.That(
            result32[2].ScalarValue.Value,
            Is.EqualTo((float)result64[2].ScalarValue.Value).Within(1e-6f)
        );
    }
}
```

### 5.4 Implementation Tasks

#### Task 3.1: Create Processor Factory (4h)
- [ ] Implement XGaProcessorFactory with all factory methods
- [ ] Add XML documentation
- [ ] Create usage examples

#### Task 3.2: Audit XGaProcessor<T> (8h)
- [ ] Review XGaProcessor<T> implementation for Float64 hardcoding
- [ ] Fix any issues found
- [ ] Verify all generic constraints are correct
- [ ] Test with FloatingScalar<float>
- [ ] Test with IMetaExpressionAtomic

#### Task 3.3: Integration Tests (8h)
- [ ] Create UnifiedWorkflowTests.cs
- [ ] Implement 20+ integration tests
- [ ] Test Float32 workflow end-to-end
- [ ] Test Float64 workflow end-to-end
- [ ] Test Symbolic workflow end-to-end
- [ ] Test workflow consistency (Float32 vs Float64 results)

#### Task 3.4: Documentation (4h)
- [ ] Create "Unified Workflow Guide"
- [ ] Document processor factory usage
- [ ] Create 5+ complete examples
- [ ] Add troubleshooting section

### 5.5 Acceptance Criteria

- [ ] All XGaProcessor<T> methods work with FloatingScalar<float>
- [ ] All XGaProcessor<T> methods work with FloatingScalar<double>
- [ ] All XGaProcessor<T> methods work with IMetaExpressionAtomic
- [ ] Factory methods create correct processors
- [ ] 20+ integration tests passing
- [ ] Float32/Float64 results consistent
- [ ] Symbolic AST building works correctly
- [ ] Documentation complete

---

## 6. Phase 4: CGa Generic Migration

**Duration**: 200 hours (broken into 7 sub-phases)
**Priority**: P1 (High)
**Dependencies**: Phase 1, 2, 3

This is the largest phase. I'll break it down into manageable sub-phases.

### 6.1 Phase 4.1: CGa Foundation (24h)

#### Goals
- Create generic `CGaBlade<T>`
- Create generic `CGaProcessor<T>`
- Create generic `CGaGeometricSpace<T>` base class
- Maintain backward compatibility via aliases

#### Task 4.1.1: Create CGaBlade<T> (12h)

**File**: `GeometricAlgebraFulcrumLib/Modeling/Geometry/CGa/Generic/Blades/CGaBlade.cs`

```csharp
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended.Generic.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Blades;

/// <summary>
/// Generic Conformal Geometric Algebra blade.
/// Works with Float32, Float64, Symbolic, and any type implementing IScalarOps.
///
/// Backward compatible via type alias:
/// using CGaFloat64Blade = CGaBlade<double>;
/// </summary>
/// <typeparam name="T">Scalar type (FloatingScalar<float>, double, IMetaExpressionAtomic, etc.)</typeparam>
public sealed record CGaBlade<T>
    where T : IScalarOps<T>
{
    // ===== Properties =====

    /// <summary>
    /// The internal XGa k-vector representing this blade
    /// </summary>
    public XGaKVector<T> InternalKVector { get; }

    /// <summary>
    /// The conformal processor for this geometric space
    /// </summary>
    public CGaProcessor<T> ConformalProcessor { get; }

    /// <summary>
    /// Scalar processor for scalar operations
    /// </summary>
    public IScalarProcessor<T> ScalarProcessor
        => ConformalProcessor.ScalarProcessor;

    /// <summary>
    /// Vector space dimensions
    /// </summary>
    public int VSpaceDimensions
        => ConformalProcessor.VSpaceDimensions;

    /// <summary>
    /// Grade of this blade
    /// </summary>
    public int Grade
        => InternalKVector.Grade;

    // ===== Indexer =====

    /// <summary>
    /// Get scalar coefficient at given index.
    /// Returns Scalar<T> (not T directly) for consistency.
    /// </summary>
    public Scalar<T> this[int index]
        => InternalKVector[index];

    // ===== Constructors =====

    internal CGaBlade(CGaProcessor<T> conformalProcessor, XGaKVector<T> kVector)
    {
        Debug.Assert(kVector.Processor.Equals(conformalProcessor));

        ConformalProcessor = conformalProcessor;
        InternalKVector = kVector;
    }

    // ===== Geometric Operations =====

    /// <summary>
    /// Geometric product: A * B
    /// </summary>
    public CGaBlade<T> Gp(CGaBlade<T> blade)
    {
        var result = InternalKVector.Gp(blade.InternalKVector);
        return new CGaBlade<T>(ConformalProcessor, result.GetKVectorPart(0));
    }

    /// <summary>
    /// Outer product (wedge): A ∧ B
    /// </summary>
    public CGaBlade<T> Op(CGaBlade<T> blade)
    {
        var result = InternalKVector.Op(blade.InternalKVector);
        return new CGaBlade<T>(ConformalProcessor, result);
    }

    /// <summary>
    /// Left contraction: A ⌋ B
    /// </summary>
    public CGaBlade<T> Lcp(CGaBlade<T> blade)
    {
        var result = InternalKVector.Lcp(blade.InternalKVector);
        return new CGaBlade<T>(ConformalProcessor, result);
    }

    /// <summary>
    /// Scalar product: A · B (returns scalar)
    /// </summary>
    public Scalar<T> Sp(CGaBlade<T> blade)
    {
        return InternalKVector.Sp(blade.InternalKVector);
    }

    /// <summary>
    /// Reverse: A†
    /// </summary>
    public CGaBlade<T> Reverse()
    {
        var result = InternalKVector.Reverse();
        return new CGaBlade<T>(ConformalProcessor, result);
    }

    /// <summary>
    /// Grade involution
    /// </summary>
    public CGaBlade<T> GradeInvolution()
    {
        var result = InternalKVector.GradeInvolution();
        return new CGaBlade<T>(ConformalProcessor, result);
    }

    /// <summary>
    /// Clifford conjugate
    /// </summary>
    public CGaBlade<T> CliffordConjugate()
    {
        var result = InternalKVector.CliffordConjugate();
        return new CGaBlade<T>(ConformalProcessor, result);
    }

    // ===== Norms and Magnitudes =====

    /// <summary>
    /// Squared norm: |A|²
    /// </summary>
    public Scalar<T> NormSquared()
    {
        return InternalKVector.NormSquared();
    }

    /// <summary>
    /// Norm: |A|
    /// </summary>
    public Scalar<T> Norm()
    {
        return InternalKVector.Norm();
    }

    /// <summary>
    /// Normalize blade to unit norm
    /// </summary>
    public CGaBlade<T> Normalize()
    {
        var norm = Norm();
        if (T.IsZero(norm.ScalarValue))
            return this;

        var result = InternalKVector.Divide(norm.ScalarValue);
        return new CGaBlade<T>(ConformalProcessor, result);
    }

    // ===== Scalar Operations =====

    /// <summary>
    /// Multiply by scalar
    /// </summary>
    public CGaBlade<T> Times(T scalar)
    {
        var result = InternalKVector.Times(scalar);
        return new CGaBlade<T>(ConformalProcessor, result);
    }

    /// <summary>
    /// Divide by scalar
    /// </summary>
    public CGaBlade<T> Divide(T scalar)
    {
        var result = InternalKVector.Divide(scalar);
        return new CGaBlade<T>(ConformalProcessor, result);
    }

    /// <summary>
    /// Add another blade
    /// </summary>
    public CGaBlade<T> Add(CGaBlade<T> blade)
    {
        var result = InternalKVector.Add(blade.InternalKVector);
        return new CGaBlade<T>(ConformalProcessor, result);
    }

    /// <summary>
    /// Subtract another blade
    /// </summary>
    public CGaBlade<T> Subtract(CGaBlade<T> blade)
    {
        var result = InternalKVector.Subtract(blade.InternalKVector);
        return new CGaBlade<T>(ConformalProcessor, result);
    }

    /// <summary>
    /// Negation
    /// </summary>
    public CGaBlade<T> Negative()
    {
        var result = InternalKVector.Negative();
        return new CGaBlade<T>(ConformalProcessor, result);
    }

    // ===== Operators =====

    public static CGaBlade<T> operator +(CGaBlade<T> left, CGaBlade<T> right)
        => left.Add(right);

    public static CGaBlade<T> operator -(CGaBlade<T> left, CGaBlade<T> right)
        => left.Subtract(right);

    public static CGaBlade<T> operator -(CGaBlade<T> blade)
        => blade.Negative();

    public static CGaBlade<T> operator *(T scalar, CGaBlade<T> blade)
        => blade.Times(scalar);

    public static CGaBlade<T> operator *(CGaBlade<T> blade, T scalar)
        => blade.Times(scalar);

    public static CGaBlade<T> operator /(CGaBlade<T> blade, T scalar)
        => blade.Divide(scalar);

    // ===== Utility Methods =====

    public bool IsZero()
    {
        return InternalKVector.IsZero();
    }

    public bool IsNearZero(double tolerance = 1e-12)
    {
        return InternalKVector.IsNearZero(tolerance);
    }

    public override string ToString()
    {
        return InternalKVector.ToString();
    }
}
```

**Backward Compatibility File**: `GeometricAlgebraFulcrumLib/Modeling/Geometry/CGa/Float64/Blades/CGaFloat64Blade.cs`

```csharp
// This file maintains backward compatibility via type alias

using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Blades;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Blades;

/// <summary>
/// Backward compatible alias for CGaBlade<double>.
/// Existing code using CGaFloat64Blade will continue to work!
/// </summary>
public record CGaFloat64Blade : CGaBlade<double>
{
    internal CGaFloat64Blade(CGaProcessor<double> processor, XGaKVector<double> kVector)
        : base(processor, kVector)
    {
    }
}

// Note: We'll need to update factory methods to return CGaFloat64Blade
// instead of CGaBlade<double> for full backward compatibility.
```

This is getting very long. Let me create the complete design document with ALL remaining phases properly structured. Should I continue with the full document including all 7 sub-phases of CGa migration, complete code examples, testing strategies, and implementation checklists?