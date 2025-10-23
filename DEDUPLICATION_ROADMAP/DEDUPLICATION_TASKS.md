# Deduplication Tasks - Module-by-Module

**Created:** 2025-10-23
**Status:** Phase 0 Complete ✅ → Phase 1 Starting
**Principle:** For EACH module: API Sync → Equivalence Tests → Thin Wrapper

---

## Task Execution Rules

1. **One module at a time** - Complete Phase 1 for module before moving to next
2. **API change = Equivalence Test** - Every API addition requires test
3. **Bugs fixed per module** - Only fix bugs when working on that module
4. **100% API Parity required** - Module must reach 100% before Phase 2
5. **Tests must pass** - All equivalence tests passing before marking complete

---

## Module 1: XGa Multivectors Core ⏸️ NOT STARTED

**Current Status:**
- Core Products: 100% equivalent ✅ (8/8 tests passing)
- MapScalars: Float64 0%, Generic 100% ❌
- Composers: Float64 minimal, Generic extended ⚠️
- Utils: Float64 100%, Generic 0% ❌
- **Overall API Parity: ~70%**

### Phase 1.1: Float64 API Extensions 🎯 START HERE

#### Task 1.1.1: Implement MapScalars in Float64
**Goal:** Add missing MapScalars() family to XGaFloat64Vector/Bivector/Scalar/KVector/Multivector

**Subtasks:**
- [ ] 1.1.1a: Add `XGaFloat64Scalar.MapScalars(Func<double, double>)`
  - Location: `GeometricAlgebra/Float64/Multivectors/XGaFloat64Scalar.cs`
  - Pattern: Copy from Generic `XGaScalar<T>.MapScalars`
  - Estimated LOC: ~5 lines

- [ ] 1.1.1b: Add `XGaFloat64Vector.MapScalars` family (3 overloads)
  - `MapScalars(Func<double, double>)`
  - `MapScalars(Func<int, double, double>)`
  - `MapScalars(Func<IndexSet, double, double>)`
  - Location: `GeometricAlgebra/Float64/Multivectors/XGaFloat64Vector.cs`
  - Estimated LOC: ~30 lines

- [ ] 1.1.1c: Add `XGaFloat64Bivector.MapScalars` family (3 overloads)
  - Same pattern as Vector
  - Location: `GeometricAlgebra/Float64/Multivectors/XGaFloat64Bivector.cs`
  - Estimated LOC: ~30 lines

- [ ] 1.1.1d: Add `XGaFloat64HigherKVector.MapScalars` family (3 overloads)
  - Same pattern
  - Location: `GeometricAlgebra/Float64/Multivectors/XGaFloat64HigherKVector.cs`
  - Estimated LOC: ~30 lines

- [ ] 1.1.1e: Add `XGaFloat64KVector.MapScalars` family (3 overloads)
  - Same pattern
  - Location: `GeometricAlgebra/Float64/Multivectors/XGaFloat64KVector.cs`
  - Estimated LOC: ~30 lines

- [ ] 1.1.1f: Add `XGaFloat64Multivector.MapScalars` family (3 overloads)
  - For GradedMultivector, UniformMultivector, base Multivector
  - Location: `GeometricAlgebra/Float64/Multivectors/XGaFloat64Multivector*.cs`
  - Estimated LOC: ~60 lines

**Test Task:**
- [ ] 1.1.1g: Create `XGaMapScalarsEquivalenceTests.cs`
  - Test Vector.MapScalars() Float64 vs Generic<double>
  - Test Bivector.MapScalars() Float64 vs Generic<double>
  - Test Multivector.MapScalars() Float64 vs Generic<double>
  - Expected tests: ~15 test methods
  - Expected result: 15/15 passing
  - Location: `UnitTests/Algebra/XGaMapScalarsEquivalenceTests.cs`

- [ ] 1.1.1h: Run `dotnet test --filter "XGaMapScalarsEquivalenceTests"`
  - Verify: 15/15 passing
  - If fails: Debug and fix

**Estimated Effort:** 4-6 hours
**Completion Criteria:** ✅ All MapScalars methods in Float64, 15/15 tests passing

---

#### Task 1.1.2: Implement MapBasisVectors in Float64
**Goal:** Add MapBasisVectors() to remap basis vector indices

**Subtasks:**
- [ ] 1.1.2a: Add `XGaFloat64Vector.MapBasisVectors(Func<int, int>)`
  - Location: `XGaFloat64Vector.cs`
  - Estimated LOC: ~10 lines

- [ ] 1.1.2b: Add same for Bivector, KVector, Multivector
  - Estimated LOC: ~30 lines total

**Test Task:**
- [ ] 1.1.2c: Add tests to `XGaMapScalarsEquivalenceTests.cs` (reuse same file)
  - Test Vector.MapBasisVectors()
  - Expected: 5 new test methods
  - Expected result: 5/5 passing

- [ ] 1.1.2d: Run tests, verify 20/20 total passing

**Estimated Effort:** 2 hours
**Completion Criteria:** ✅ MapBasisVectors in Float64, 20/20 tests passing

---

#### Task 1.1.3: Implement MapTerms in Float64
**Goal:** Add MapTerms() for combined index+scalar mapping

**Subtasks:**
- [ ] 1.1.3a: Add `XGaFloat64Vector.MapTerms(Func<int, double, KeyValuePair<int, double>>)`
- [ ] 1.1.3b: Add same for Bivector, KVector, Multivector

**Test Task:**
- [ ] 1.1.3c: Add tests to equivalence test file
  - Expected: 5 new test methods
  - Expected result: 25/25 total passing

**Estimated Effort:** 2 hours
**Completion Criteria:** ✅ MapTerms in Float64, 25/25 tests passing

---

#### Task 1.1.4: Extend Composer API in Float64
**Goal:** Add int/IFloat64Scalar/Float64Scalar overloads to Composers

**Subtasks:**
- [ ] 1.1.4a: Add `SetVectorTerm(int index, int scalar)`
- [ ] 1.1.4b: Add `SetVectorTerm(int index, Float64Scalar scalar)`
- [ ] 1.1.4c: Add `SetVectorTerm(int index, IFloat64Scalar scalar)`
- [ ] 1.1.4d: Same for SetBivectorTerm (3 overloads)
- [ ] 1.1.4e: Add SetTrivectorTerm methods (MISSING in Float64!)
  - `SetTrivectorTerm(int i1, int i2, int i3, double scalar)`
  - `SetTrivectorTerm(int i1, int i2, int i3, int scalar)`
  - `SetTrivectorTerm(int i1, int i2, int i3, Float64Scalar scalar)`
  - `SetTrivectorTerm(int i1, int i2, int i3, IFloat64Scalar scalar)`
- [ ] 1.1.4f: Same for AddVectorTerm, AddBivectorTerm, AddTrivectorTerm
- [ ] 1.1.4g: Same for SubtractVectorTerm, SubtractBivectorTerm, SubtractTrivectorTerm

**Test Task:**
- [ ] 1.1.4h: Extend `XGaComposerEquivalenceTests.cs`
  - Test int overloads
  - Test IFloat64Scalar overloads
  - Test Trivector methods
  - Expected: +10 test methods (18 total in file)
  - Expected result: 18/18 passing

- [ ] 1.1.4i: Run tests, verify 18/18 passing

**Estimated Effort:** 6-8 hours (many overloads)
**Completion Criteria:** ✅ Extended Composer API, 18/18 tests passing

---

#### Task 1.1.5: Add Times/Divide overloads in Float64
**Goal:** Add IFloat64Scalar/Float64Scalar overloads

**Subtasks:**
- [ ] 1.1.5a: Add `XGaFloat64Scalar.Times(Float64Scalar)`
- [ ] 1.1.5b: Add `XGaFloat64Scalar.Times(IFloat64Scalar)`
- [ ] 1.1.5c: Add `XGaFloat64Scalar.Divide(Float64Scalar)`
- [ ] 1.1.5d: Add `XGaFloat64Scalar.Divide(IFloat64Scalar)`
- [ ] 1.1.5e: Same for Vector, Bivector, KVector, Multivector

**Test Task:**
- [ ] 1.1.5f: Add test methods
  - Expected: 5 test methods
  - Expected result: 5/5 passing

**Estimated Effort:** 2 hours
**Completion Criteria:** ✅ Times/Divide overloads, 5/5 tests passing

---

### Phase 1.2: Generic API Extensions

#### Task 1.2.1: Implement Utils/Conversions in Generic
**Goal:** Add ToXGaVector<T>() extension methods for LinVector2D/3D/4D

**Subtasks:**
- [ ] 1.2.1a: Create `XGaVectorUtils.cs` (or extend existing)
  - Location: `GeometricAlgebra/Generic/Multivectors/XGaVectorUtils.cs`

- [ ] 1.2.1b: Add `LinVector2D<T>.ToXGaVector<T>(XGaProcessor<T>)`
  - Extension method pattern
  - Estimated LOC: ~5 lines

- [ ] 1.2.1c: Add `LinVector3D<T>.ToXGaVector<T>(XGaProcessor<T>)`
- [ ] 1.2.1d: Add `LinVector4D<T>.ToXGaVector<T>(XGaProcessor<T>)`
- [ ] 1.2.1e: Add `LinVector<T>.ToXGaVector<T>(XGaProcessor<T>)`

**Test Task:**
- [ ] 1.2.1f: Create `XGaVectorConversionEquivalenceTests.cs`
  - Test LinVector2D → XGaVector conversion (Float64 vs Generic)
  - Test LinVector3D → XGaVector conversion
  - Test LinVector4D → XGaVector conversion
  - Expected: 10 test methods
  - Expected result: 10/10 passing

**Estimated Effort:** 3 hours
**Completion Criteria:** ✅ Utils/Conversions in Generic, 10/10 tests passing

---

#### Task 1.2.2: Add CreateUnitVector/CreatePhasor in Generic
**Goal:** Add geometric constructor extension methods

**Subtasks:**
- [ ] 1.2.2a: Add `LinAngle<T>.CreateUnitVector<T>(int index1, int index2, XGaProcessor<T>)`
- [ ] 1.2.2b: Add `LinAngle<T>.CreatePhasor<T>(T magnitude, int index1, int index2, XGaProcessor<T>)`

**Test Task:**
- [ ] 1.2.2c: Add tests to conversion test file
  - Expected: 3 test methods
  - Expected result: 13/13 total passing

**Estimated Effort:** 1 hour
**Completion Criteria:** ✅ Geometric constructors, 13/13 tests passing

---

### Phase 1.3: Bug Fixes for XGa

#### Task 1.3.1: Fix XGaPureRotor<T>.IsValid() inverted logic
**Location:** `GeometricAlgebra/Generic/LinearMaps/Rotors/XGaPureRotor.cs`
**Bug:** Line 78 has inverted logic: `return Multivector.IsEven(2)` should be `!IsEven(2)`

**Subtasks:**
- [ ] 1.3.1a: Read file and locate bug
  ```bash
  Read GeometricAlgebra/Generic/LinearMaps/Rotors/XGaPureRotor.cs
  ```

- [ ] 1.3.1b: Fix the bug
  ```csharp
  // BEFORE (WRONG):
  public bool IsValid() => Multivector.IsEven(2);

  // AFTER (CORRECT):
  public bool IsValid() => !Multivector.IsEven(2);
  ```

- [ ] 1.3.1c: Write test for IsValid()
  - Location: `UnitTests/LinearMaps/XGaPureRotorTests.cs`
  - Test that pure rotors are correctly validated
  - Expected: 2 test methods
  - Expected result: 2/2 passing

- [ ] 1.3.1d: Run test and verify

**Estimated Effort:** 30 minutes
**Completion Criteria:** ✅ Bug fixed, 2/2 tests passing

---

### Phase 1.4: Verification

#### Task 1.4.1: Run ALL XGa Equivalence Tests
**Command:**
```bash
dotnet test --filter "FullyQualifiedName~XGa*EquivalenceTests"
```

**Expected Results:**
- XGaComposerEquivalenceTests: 18/18 ✅
- XGaMapScalarsEquivalenceTests: 25/25 ✅
- XGaVectorConversionEquivalenceTests: 13/13 ✅
- XGaPureRotorTests: 2/2 ✅
- **TOTAL: 58/58 tests passing**

**Success Criteria:**
- [ ] 1.4.1a: All tests passing
- [ ] 1.4.1b: No warnings or errors
- [ ] 1.4.1c: API Parity documented: 100% ✅

---

### Phase 1.5: Documentation

#### Task 1.5.1: Update Status Documents
**Files to update:**
- [ ] 1.5.1a: `DEDUPLICATION_ROADMAP/_Status.md`
  - Update "Module 1: XGa Multivectors Core" to "Phase 1 Complete ✅"
  - Update test count: 58/58 passing
  - Update API Parity: 100%

- [ ] 1.5.1b: `DEDUPLICATION_ROADMAP/DEDUPLICATION_ROADMAP.md`
  - Update Phase 1.1 status

- [ ] 1.5.1c: `DEDUPLICATION_ROADMAP/NEXT_STEPS_ROADMAP.md`
  - Mark Module 1 complete
  - Next step: Module 2

**Completion Criteria:** ✅ All docs updated, Module 1 marked complete

---

### ✅ Module 1 Complete When:
- [ ] All 58 equivalence tests passing
- [ ] 100% API Parity documented
- [ ] Bug fixed (XGaPureRotor)
- [ ] Ready for Phase 2 (Thin Wrapper Migration)

**Estimated Total Effort for Module 1:** 20-25 hours

---

## Module 2: LinearAlgebra ⏸️ NOT STARTED

**Current Status:**
- Equivalence Tests: 28/28 passing ✅
- API Gaps: Generic missing IsNearZero(epsilon), Rcp(), etc.
- **Overall API Parity: ~80%**

### Phase 1.1: Generic API Extensions

#### Task 2.1.1: Add IsNearZero(epsilon) to 15+ types
**Goal:** Add tolerance-based zero checking to Generic types

**Missing in:**
- LinVector2D<T>
- LinVector3D<T>
- LinVector4D<T>
- LinVector<T>
- LinBivector2D<T>
- LinBivector3D<T>
- LinBivector<T>
- LinTrivector<T>
- LinQuaternion<T>
- Lin4x4Matrix<T>
- Lin3x3Matrix<T>
- (+ more)

**Subtasks:**
- [ ] 2.1.1a: Add `bool IsNearZero(T epsilon)` to LinVector2D<T>
  - Location: `LinearAlgebra/Generic/Vectors/Space2D/LinVector2D.cs`
  - Pattern: Check if all components < epsilon
  - Estimated LOC: ~10 lines

- [ ] 2.1.1b: Add to LinVector3D<T>
- [ ] 2.1.1c: Add to LinVector4D<T>
- [ ] 2.1.1d: Add to LinBivector2D<T>
- [ ] 2.1.1e: Add to LinBivector3D<T>
- [ ] 2.1.1f: Add to LinQuaternion<T>
- [ ] 2.1.1g: Add to remaining 9+ types

**Test Task:**
- [ ] 2.1.1h: Extend `LinVector*EquivalenceTests.cs`
  - Test IsNearZero(epsilon) for each type
  - Expected: +15 test methods
  - Expected result: 43/43 total passing (28 existing + 15 new)

- [ ] 2.1.1i: Run tests, verify 43/43 passing

**Estimated Effort:** 4-6 hours
**Completion Criteria:** ✅ IsNearZero in all types, 43/43 tests passing

---

#### Task 2.1.2: Implement LinBivector2D<T>.Rcp()
**Goal:** Add missing Right Contraction Product method

**Subtasks:**
- [ ] 2.1.2a: Analyze Float64 implementation
  ```bash
  Read LinearAlgebra/Float64/Vectors/Space2D/LinFloat64Bivector2D.cs
  ```

- [ ] 2.1.2b: Copy implementation to Generic
  - Location: `LinearAlgebra/Generic/Vectors/Space2D/LinBivector2D.cs`
  - Adapt from double to T using ScalarProcessor
  - Estimated LOC: ~15 lines

**Test Task:**
- [ ] 2.1.2c: Add test to `LinBivectorEquivalenceTests.cs`
  - Test Rcp() Float64 vs Generic<double>
  - Expected: +1 test method
  - Expected result: 44/44 total passing

**Estimated Effort:** 1 hour
**Completion Criteria:** ✅ Rcp() implemented, 44/44 tests passing

---

#### Task 2.1.3: Restore LinQuaternion<T> System.Numerics interop
**Goal:** Uncomment and fix CreateFromRotationMatrix() and conversions

**Subtasks:**
- [ ] 2.1.3a: Uncomment System.Numerics interop code
  - Location: `LinearAlgebra/Generic/Vectors/Space4D/LinQuaternion.cs`
  - Find commented-out sections

- [ ] 2.1.3b: Adapt for Generic<T> (may require double/float constraint)
  - System.Numerics only works with float/double
  - May need: `where T : struct, IFloatingPoint<T>`

- [ ] 2.1.3c: Test with Float64 and Float32

**Test Task:**
- [ ] 2.1.3d: Add tests for System.Numerics conversions
  - Expected: +5 test methods
  - Expected result: 49/49 total passing

**Estimated Effort:** 2-3 hours (complex due to generic constraints)
**Completion Criteria:** ✅ System.Numerics interop restored, 49/49 tests passing

---

### Phase 1.2: Verification

#### Task 2.2.1: Run ALL LinearAlgebra Equivalence Tests
**Command:**
```bash
dotnet test --filter "FullyQualifiedName~Lin*EquivalenceTests"
```

**Expected Results:**
- LinVector2DEquivalenceTests: 8/8 ✅
- LinVector3DEquivalenceTests: 8/8 ✅
- LinBivectorEquivalenceTests: 11/11 ✅
- LinQuaternionEquivalenceTests: 22/22 ✅
- **TOTAL: 49/49 tests passing**

**Success Criteria:**
- [ ] 2.2.1a: All tests passing
- [ ] 2.2.1b: 100% API Parity documented

---

### ✅ Module 2 Complete When:
- [ ] All 49 equivalence tests passing
- [ ] 100% API Parity documented
- [ ] Ready for Phase 2

**Estimated Total Effort for Module 2:** 8-10 hours

---

## Module 3: CGA Encoders ⏸️ NOT STARTED

**Current Status:**
- Equivalence Tests: 66/66 passing ✅
- API Status: Float64 (double-only) vs Generic (Hybrid API with T + double + IScalar<T>)
- **Overall API Parity: ~85% (functionally equivalent, but signatures differ)**

**Note:** CGA is unique - APIs are functionally equivalent (proven by tests) but signatures differ. **Decision needed:**

**Option A:** Keep as-is (Recommended)
- Float64 stays simple (double-only)
- Generic stays Hybrid (flexible)
- No changes needed
- Thin wrapper still possible

**Option B:** Synchronize to Hybrid API in Float64
- Add IFloat64Scalar/Float64Scalar overloads to all Float64 encoders
- Estimated: 40+ methods × 6 encoder types = 240+ new overloads
- Estimated effort: 20-30 hours
- Benefit: Unclear (tests already prove equivalence)

### Phase 1.1: Decision Task

#### Task 3.1.1: Decide API strategy for CGA
**Decision Point:** Keep APIs as-is (different but equivalent) OR synchronize?

**Subtasks:**
- [ ] 3.1.1a: Review team preference
- [ ] 3.1.1b: If KEEP AS-IS:
  - Mark Module 3 as "Phase 1 Complete (functionally equivalent)" ✅
  - Proceed directly to Phase 2 (Thin Wrapper)
  - Estimated effort: 0 hours

- [ ] 3.1.1c: If SYNCHRONIZE:
  - Add Hybrid API overloads to Float64 (see Task 3.1.2)
  - Estimated effort: 20-30 hours

**Recommended:** KEEP AS-IS (proven equivalent, no benefit to synchronization)

---

#### Task 3.1.2: (Optional) Synchronize CGA Encoders
**Only if decision is to synchronize**

**Subtasks:**
- [ ] Add IFloat64Scalar/Float64Scalar overloads to all 6 encoder types
  - IpnsRound (9 methods × 2 overloads = 18 new methods)
  - OpnsRound (8 methods × 2 overloads = 16 new methods)
  - IpnsFlat (6 methods × 2 overloads = 12 new methods)
  - OpnsFlat (6 methods × 2 overloads = 12 new methods)
  - IpnsTangent (6 methods × 2 overloads = 12 new methods)
  - OpnsTangent (6 methods × 2 overloads = 12 new methods)
  - **TOTAL: ~82 new overloads**

**Estimated Effort:** 20-30 hours
**Completion Criteria:** All overloads added, tests still passing

---

### ✅ Module 3 Complete When:
- [ ] Decision made (KEEP AS-IS recommended)
- [ ] 66/66 tests still passing
- [ ] Ready for Phase 2

**Estimated Total Effort for Module 3:** 0-30 hours (depending on decision)

---

## Module 4: Polynomials ⏸️ NOT STARTED

**Current Status:**
- API Consistency: ~95%
- Bug: BSplineKnotVector<T>.AppendKnot() lacks validation
- **Overall API Parity: ~95%**

### Phase 1.1: Bug Fix

#### Task 4.1.1: Fix BSplineKnotVector<T>.AppendKnot() validation
**Location:** `Modeling/Polynomials/Generic/BSplines/BSplineKnotVector.cs`

**Bug:** No validation that appended knot maintains non-decreasing order

**Subtasks:**
- [ ] 4.1.1a: Add validation check
  ```csharp
  public void AppendKnot(T value)
  {
      if (Knots.Count > 0 && ScalarProcessor.IsLessThan(value, Knots[^1].Value))
          throw new ArgumentException("Knot values must be non-decreasing");

      // existing code...
  }
  ```

**Test Task:**
- [ ] 4.1.1b: Write test for validation
  - Test that invalid knot sequence throws exception
  - Expected: 2 test methods
  - Expected result: 2/2 passing

**Estimated Effort:** 1 hour
**Completion Criteria:** ✅ Validation added, 2/2 tests passing

---

### Phase 1.2: Minor API Gaps (if any)
**TBD based on detailed comparison**

---

### ✅ Module 4 Complete When:
- [ ] Bug fixed
- [ ] Validation tests passing
- [ ] 100% API Parity documented
- [ ] Ready for Phase 2

**Estimated Total Effort for Module 4:** 2-3 hours

---

## Summary: Phase 1 Totals

### Estimated Effort by Module:
- Module 1 (XGa Multivectors): 20-25 hours
- Module 2 (LinearAlgebra): 8-10 hours
- Module 3 (CGA): 0-30 hours (depending on decision)
- Module 4 (Polynomials): 2-3 hours

**TOTAL Phase 1 Effort:** 30-68 hours (4-9 work days)

### Expected Test Results After Phase 1:
- Module 1: 58/58 tests passing ✅
- Module 2: 49/49 tests passing ✅
- Module 3: 66/66 tests passing ✅ (already passing)
- Module 4: 2/2 tests passing ✅
- **TOTAL: 175/175 equivalence tests passing**

### Phase 1 Success Criteria:
- ✅ 100% API Parity per module
- ✅ All equivalence tests passing
- ✅ All bugs fixed per module
- ✅ Ready for Phase 2 (Thin Wrapper Migration)

---

## Next Steps After Phase 1

**Module 1 Complete?** → Start Module 2
**Module 2 Complete?** → Start Module 3
**Module 3 Complete?** → Start Module 4
**Module 4 Complete?** → **Phase 1 DONE** → Begin Phase 2

**Phase 2 = Thin Wrapper Migration** (new task list will be created then)

---

## Notes

- **Components NOT included in Phase 1:**
  - Calculus (30% API parity - defer to future)
  - Trajectories (0% Generic - defer to future)
  - Statistics (100% Float64-only by design - no action needed)
  - BasicShapes (100% Float64-only - defer to future)

- **Focus:** Complete deduplication for CORE modules first (XGa, LinearAlgebra, CGA, Polynomials)

- **After Core Modules:** Reassess strategy for remaining components

---

**Last Updated:** 2025-10-23
**Next Review:** After Module 1 completion
