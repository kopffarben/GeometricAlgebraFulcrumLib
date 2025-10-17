# Known Issues in GeometricAlgebraFulcrumLib

This document tracks known bugs and issues discovered during unit testing.

---

## Bug: CreatePureRotor fails with antiparallel vectors

**Severity**: Medium
**Status**: Unresolved
**Discovered**: 2025-01-13
**Location**: `GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/Float64/Multivectors/SubspaceOps.cs:712`

### Description

The `XGaFloat64Vector.CreatePureRotor(XGaFloat64Vector targetVector, bool assumeUnitVectors)` method fails with a `DebugAssertException` when the source and target vectors are nearly antiparallel (cosine angle ≈ -1).

### Root Cause

When two unit vectors are antiparallel, the code attempts to find a normal vector using `GetNormalVector()`:

```csharp
var rotationBlade =
    cosAngle.IsMinusOne
        ? GetNormalVector().Op(this)  // <-- FAILS HERE
        : targetVector.Op(this);
```

The `GetNormalVector()` method internally calls:
- `ToLinVector().GetUnitNormal()`
- Which creates a rotation from e₀ to the normalized vector
- If the normalized vector is also antiparallel to e₀, a circular dependency occurs
- A `Debug.Assert` fails deep in the call chain

### Stack Trace

```
Microsoft.VisualStudio.TestPlatform.TestHost.DebugAssertException
  at System.Diagnostics.Debug.Fail(String message, String detailMessage)
  at GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors.XGaFloat64Vector.CreatePureRotor(XGaFloat64Vector targetVector, Boolean assumeUnitVectors)
    in SubspaceOps.cs:line 712
```

### Reproduction

```csharp
var processor = XGaFloat64Processor.Euclidean;
var random = processor.CreateXGaRandomComposer(vSpaceDimensions, seed);

// Generate random unit vectors - occasionally they will be antiparallel
var u1 = random.GetVector().DivideByENorm();
var u2 = random.GetVector().DivideByENorm();

// This may fail if u1 and u2 are nearly antiparallel
var rotor = u1.CreatePureRotor(u2);  // DebugAssertException
```

### Workaround

1. **Catch the exception** and retry with different vectors:
   ```csharp
   try
   {
       var rotor = u1.CreatePureRotor(u2);
   }
   catch (DebugAssertException)
   {
       // Vectors are antiparallel, skip or retry
   }
   ```

2. **Check angle before creating rotor**:
   ```csharp
   var cosAngle = u1.ESp(u2);
   if (Math.Abs(cosAngle + 1.0) < 1e-10)
   {
       // Vectors are antiparallel, handle specially
   }
   else
   {
       var rotor = u1.CreatePureRotor(u2);
   }
   ```

### Proposed Fix

The library should handle the antiparallel case explicitly:

1. Detect when vectors are antiparallel before calling `GetNormalVector()`
2. Use a deterministic method to find a normal vector (e.g., choose the first non-parallel basis vector)
3. Replace `Debug.Assert` with proper exception handling

### Affected Tests

- `ProcessorSpecificTests.EuclideanProcessorTests.Rotation_PreservesNorm` (flaky - fails intermittently)

### Related Code Locations

- `SubspaceOps.cs:712` - `CreatePureRotor` method
- `LinFloat64Vector.cs:1207-1218` - `GetUnitNormal` method
- `LinFloat64AxisToVectorRotation.cs:61-91` - `CreateFromRotatedVector` method

---

## Notes

- This is a **flaky bug** - it only occurs with specific random vector combinations
- The bug is in the **original library code**, not in the unit tests
- Production code using `CreatePureRotor` should include error handling for this edge case
