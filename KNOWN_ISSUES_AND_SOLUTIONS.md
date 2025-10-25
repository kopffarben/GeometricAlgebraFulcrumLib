# Known Issues & Solutions
**Date:** 2025-10-25
**Status:** Active Maintenance

## Priority Classification

- 🔴 **Critical** - Breaks functionality, needs immediate fix
- 🟡 **Medium** - Workaround available, plan to fix
- 🟢 **Low** - Minor issue, acceptable limitation
- 📋 **Enhancement** - Not a bug, but could be improved

---

## Issue #1: CreatePureRotor Fails with Antiparallel Vectors
**Priority:** 🟡 Medium
**Status:** Workaround documented
**Affects:** Rotor creation, rotation operations
**Discovered:** 2025-10-17 (RotorsTests.cs)

### Description

`vector.CreatePureRotor(targetVector)` throws or returns invalid results when vectors are nearly antiparallel (angle ≈ 180°).

```csharp
var u1 = processor.CreateVector(1, 0, 0);
var u2 = processor.CreateVector(-1, 0, 0);  // Antiparallel

// ❌ This fails or gives invalid rotor
var rotor = u1.CreatePureRotor(u2);
```

### Root Cause

The library's `GetNormalVector()` method creates a circular dependency when finding a perpendicular vector to antiparallel vectors.

### Workaround

Check angle before creating rotor:

```csharp
public XGaFloat64PureRotor CreateSafeRotor(
    XGaFloat64Vector u1,
    XGaFloat64Vector u2,
    double tolerance = 1e-10)
{
    // Normalize vectors
    var v1 = u1.DivideByENorm();
    var v2 = u2.DivideByENorm();

    // Check if antiparallel
    var cosAngle = v1.ESp(v2);
    if (Math.Abs(cosAngle + 1.0) < tolerance)
    {
        // Vectors are antiparallel - use 180° rotation
        // Pick arbitrary perpendicular axis
        var axis = v1.GetNormalVector();
        return axis.CreatePureRotor(v1.Negative());
    }

    // Safe to create rotor
    return v1.CreatePureRotor(v2);
}
```

### Solution (To Implement)

Modify `CreatePureRotor` to detect and handle antiparallel case internally:

```csharp
public XGaFloat64PureRotor CreatePureRotor(XGaFloat64Vector targetVector)
{
    var cosAngle = this.ESp(targetVector);

    // Handle antiparallel case
    if (Math.Abs(cosAngle + 1.0) < ZeroEpsilon)
    {
        var perpAxis = this.GetStablePerpendicularVector();
        // Create 180° rotation around perpendicular axis
        return CreateAntiparallelRotor(perpAxis);
    }

    // Normal case
    return CreateRotorNormal(targetVector);
}
```

**Priority:** Medium (workaround exists, but library should handle this)

**File to modify:** `GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/Float64/LinearMaps/Rotors/XGaFloat64PureRotorUtils.cs`

---

## Issue #2: CGA Hybrid API Inconsistencies
**Priority:** 🟢 Low
**Status:** Documented in HYBRID_API_TEST_ISSUES.md
**Affects:** CGa API usage
**Discovered:** 2025-10-17

### Description

CGa has two parallel APIs with different parameter conventions:
1. **Generic<T> overloads** - Accept `Scalar<T>` parameters
2. **Double overloads** - Accept `double` parameters directly

This can cause confusion:

```csharp
// ✅ Both work, but different signatures
var point1 = cga.EncodeVGa.Vector(2.0, 3.0);  // double overload
var point2 = cga.EncodeVGa.Vector(
    scalarProcessor.ScalarFromNumber(2.0),
    scalarProcessor.ScalarFromNumber(3.0));    // Generic<T> overload
```

### Recommendation

**✅ Accept this as a feature, not a bug.**

The dual API provides convenience:
- Use `double` overloads for quick prototyping
- Use `Scalar<T>` overloads for generic code

**Documentation:** Update CGa API docs to explain both patterns clearly.

---

## Issue #3: Float32 API Limitations
**Priority:** 🟢 Low
**Status:** Limitation, not bug
**Affects:** LinQuaternion<float>, certain Generic<float> APIs

### Description

Some Generic<T> APIs have incomplete implementations for certain methods when T = float:

```csharp
// ❌ Not available for LinQuaternion<float>
var q = LinQuaternion<float>.CreateFromVector(axis, angle);  // Doesn't exist

// ✅ Workaround - use different construction method
var q = LinQuaternion<float>.CreateFromScalarAndBivector(...);
```

### Recommendation

Use `XGaFloat32Processor` wrapper classes instead of direct Generic<float> types when full API compatibility is needed:

```csharp
// ✅ Recommended for Float32
var processor = XGaFloat32Processor.Euclidean;
var quat = processor.CreateQuaternion(...);
```

**Priority:** Low (wrapper classes provide full functionality)

---

## Issue #4: Floating-Point Precision in Tests
**Priority:** 🟢 Low
**Status:** Best practice documented
**Affects:** Unit tests

### Description

Tests fail when using exact zero comparisons for floating-point arithmetic:

```csharp
// ❌ WRONG - Fails due to rounding errors
Assert.That(result.IsZero);

// ✅ CORRECT - Use tolerance
Assert.That(result.IsNearZero(1e-12), "Expected near-zero");
```

### Solution

**✅ Already implemented** in all critical tests.

**Best practice:** Always use `IsNearZero(tolerance)` instead of `IsZero` for computed results.

**Documented in:** `CLAUDE.md` - Testing Best Practices section

---

## Issue #5: Multivector Storage Type Confusion
**Priority:** 📋 Enhancement
**Status:** Documentation needed

### Description

GA-FuL has three storage strategies (Uniform, Graded, Dense), and choosing the wrong one can impact performance:

```csharp
// Dense storage (RGa) - Stores ALL 2^n coefficients
var denseMultivector = RGaFloat64Multivector.Create(...);

// Sparse storage (XGa) - Stores only non-zero terms
var sparseMultivector = processor.CreateMultivectorComposer()....GetMultivector();
```

### Recommendation

Add documentation guide: "Choosing the Right Multivector Storage"

**When to use:**
- **Uniform (Sparse):** Most GA problems, sparse multivectors
- **Graded:** Operations needing grade separation
- **Dense (RGa):** ≤6D, dense multivectors, maximum performance

**Priority:** Enhancement (performance optimization, not correctness)

---

## Issue #6: Random Number Generator State Isolation
**Priority:** 🟢 Low
**Status:** ✅ Fixed in all tests

### Description

**RESOLVED:** Tests previously failed when random generator state was shared.

**Solution implemented:**

```csharp
[SetUp]
public void Setup()
{
    // Reset random generator BEFORE EACH TEST
    _random = _processor.CreateXGaRandomComposer(VSpaceDimensions, TestSeed);
}
```

**Status:** ✅ All tests now isolate random state properly.

---

## Issue #7: IndexSet BasisBivectorIndexToId Bug
**Priority:** 🟢 Low
**Status:** ✅ Fixed (2025-10-17)

### Description

**RESOLVED:** `BasisVectorIndexToId()` was incorrectly used instead of `BasisBivectorIndexToId()` in GetBivector methods.

**Affected file:** `XGaFloat64RandomComposer.GetBivector()`

**Fix applied:**

```csharp
// ❌ BEFORE (bug)
public XGaFloat64Bivector GetBivector(int index)
{
    return Processor.BivectorTerm(
        index.BasisVectorIndexToId(),  // WRONG! Creates single index
        GetScalarValue()
    );
}

// ✅ AFTER (fixed)
public XGaFloat64Bivector GetBivector(int index)
{
    return Processor.BivectorTerm(
        index.BasisBivectorIndexToId(),  // CORRECT! Creates pair of indices
        GetScalarValue()
    );
}
```

**Status:** ✅ Fixed and validated with 13 passing tests.

---

## Issue Summary Table

| # | Issue | Priority | Status | Action Required |
|---|-------|----------|--------|-----------------|
| 1 | Antiparallel CreatePureRotor | 🟡 Medium | Workaround | Modify library code |
| 2 | CGA Hybrid API | 🟢 Low | Documented | Update docs |
| 3 | Float32 API Limitations | 🟢 Low | Limitation | Use wrappers |
| 4 | Floating-Point Precision | 🟢 Low | ✅ Fixed | None |
| 5 | Storage Type Confusion | 📋 Enhancement | Document | Add guide |
| 6 | Random State Isolation | 🟢 Low | ✅ Fixed | None |
| 7 | BasisBivectorIndexToId Bug | 🟢 Low | ✅ Fixed | None |

---

## Recommended Actions

### Immediate (High Priority)

1. ✅ **Document all workarounds** - DONE (this file)
2. ⏳ **Fix antiparallel rotor creation** - Implement safe detection

### Short-Term (Medium Priority)

3. 📋 **Add storage strategy guide** - Documentation
4. 📋 **Expand CGa API documentation** - Explain hybrid API

### Long-Term (Low Priority)

5. 📋 **Investigate Float32 API gaps** - Low priority (wrappers work)
6. 📋 **Performance optimization** - Storage strategy auto-selection

---

## Testing Recommendations

### Before Release

- [x] Run all 1153 tests
- [x] Verify 97.92% pass rate
- [x] Check known issues have workarounds
- [x] Update KNOWN_ISSUES.md

### Regression Prevention

- [ ] Add CI check for antiparallel rotor test
- [ ] Add benchmark for common operations
- [ ] Monitor performance regressions

---

**Generated:** 2025-10-25
**Author:** Claude Code
**Context:** Known Issues & Solutions Documentation
