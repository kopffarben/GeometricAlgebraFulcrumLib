# Next Steps Roadmap

**Last Updated:** 2025-10-23
**Current Status:** Phase 0 Complete ✅ → Phase 1 Starting (Module 1: XGa Multivectors)
**Branch:** Feature/ScalarFloat32

---

## ⚠️ DOCUMENTATION MAINTENANCE

**CRITICAL:** The following files must be kept synchronized and up to date after any significant changes:
1. **`DEDUPLICATION_ROADMAP.md`** - Overall roadmap and detailed phase descriptions
2. **`NEXT_STEPS_ROADMAP.md`** (this file) - Immediate next steps and priorities
3. **`_Status.md`** - Quick bullet-list overview (for fast status checks)
4. **`DEDUPLICATION_TASKS.md`** - Detailed task list per module

After completing milestones, fixing bugs, or making architectural changes, update all four files to reflect current status.

**Quick Status Check:** See [`_Status.md`](_Status.md) for a concise bullet-list overview.
**Detailed Task List:** See [`DEDUPLICATION_TASKS.md`](DEDUPLICATION_TASKS.md) for complete task breakdown.

---

## 🎯 Reality Check: Where We Actually Are

### ✅ Phase 0 COMPLETE
- [x] 102/102 equivalence tests passing
- [x] Performance validated (Generic 1.27x FASTER)
- [x] Comprehensive API analysis (20 agents, 700+ files)
- [x] Task list created

### ⏸️ Phase 1 NOT STARTED
**Current API Parity:** ~60-80% (varies by module)
**Estimated Effort:** 30-68 hours (4-9 work days)
**NOT "ready for Phase 2"** - must complete API synchronization first

---

## 🚀 Week 1-2: Module 1 - XGa Multivectors Phase 1

### Current Status
- ✅ Tests: 8/8 composer tests passing
- ❌ API Parity: ~70% (significant gaps on both sides)
- ❌ MapScalars: Float64 0%, Generic 100%
- ❌ Composers: Float64 minimal, Generic extended
- ❌ Utils: Float64 100%, Generic 0%

### 🎯 Start Here: Task 1.1.1 - Implement MapScalars in Float64

**Location:** `DEDUPLICATION_TASKS.md` → Module 1 → Task 1.1.1

**Goal:** Add MapScalars() family to all Float64 multivector types

**Steps:**
1. **Day 1:** Implement MapScalars in XGaFloat64Scalar/Vector (Subtasks 1.1.1a-b)
   ```bash
   # Edit files:
   # - GeometricAlgebra/Float64/Multivectors/XGaFloat64Scalar.cs
   # - GeometricAlgebra/Float64/Multivectors/XGaFloat64Vector.cs
   ```

2. **Day 2:** Implement MapScalars in Bivector/KVector/Multivector (Subtasks 1.1.1c-f)
   ```bash
   # Edit files:
   # - XGaFloat64Bivector.cs
   # - XGaFloat64HigherKVector.cs
   # - XGaFloat64KVector.cs
   # - XGaFloat64*Multivector.cs
   ```

3. **Day 2-3:** Write equivalence tests (Subtask 1.1.1g)
   ```bash
   # Create: UnitTests/Algebra/XGaMapScalarsEquivalenceTests.cs
   # Expected: 15 test methods
   ```

4. **Day 3:** Run tests and verify (Subtask 1.1.1h)
   ```bash
   cd GeometricAlgebraFulcrumLib
   dotnet test --filter "XGaMapScalarsEquivalenceTests"
   # Expected: 15/15 passing
   ```

**Success Criteria:** ✅ MapScalars in all Float64 types, 15/15 tests passing

---

### Days 3-4: Task 1.1.2-1.1.3 - MapBasisVectors & MapTerms

See `DEDUPLICATION_TASKS.md` Module 1 → Tasks 1.1.2-1.1.3

**Expected Result:** 25/25 total tests passing

---

### Days 5-7: Task 1.1.4 - Extend Composer API in Float64

**Goal:** Add int/IFloat64Scalar/Float64Scalar overloads + SetTrivectorTerm methods

**Critical:** SetTrivectorTerm is COMPLETELY MISSING in Float64!

**Expected Result:** 18/18 composer tests passing (8 existing + 10 new)

---

### Days 8-9: Tasks 1.2.1-1.2.2 - Generic Utils/Conversions

**Goal:** Add ToXGaVector<T>() extension methods for LinVector2D/3D/4D

**Expected Result:** 13/13 conversion tests passing

---

### Day 10: Task 1.3.1 - Bug Fix: XGaPureRotor<T>.IsValid()

**Location:** `GeometricAlgebra/Generic/LinearMaps/Rotors/XGaPureRotor.cs:78`

**Bug:** Inverted logic
```csharp
// WRONG:
public bool IsValid() => Multivector.IsEven(2);

// CORRECT:
public bool IsValid() => !Multivector.IsEven(2);
```

**Expected Result:** 2/2 bug fix tests passing

---

### Day 11: Verification & Documentation

**Task 1.4.1:** Run ALL XGa equivalence tests
```bash
dotnet test --filter "FullyQualifiedName~XGa*EquivalenceTests"
```

**Expected Result:** 58/58 tests passing (8 existing + 50 new)

**Task 1.5.1:** Update documentation
- [ ] Update `_Status.md` - Mark Module 1 complete
- [ ] Update `DEDUPLICATION_ROADMAP.md` - Update milestone status
- [ ] Update `NEXT_STEPS_ROADMAP.md` - Move to Module 2
- [ ] Check off tasks in `DEDUPLICATION_TASKS.md`

**Success Criteria:** ✅ Module 1 Phase 1 Complete (100% API Parity, 58/58 tests)

---

## 🚀 Week 3: Module 2 - LinearAlgebra Phase 1

### Current Status
- ✅ Tests: 28/28 equivalence tests passing
- ❌ API Parity: ~80%
- ❌ Generic missing: IsNearZero(epsilon) in 15+ types
- ❌ Generic missing: LinBivector2D<T>.Rcp()
- ❌ Generic missing: LinQuaternion<T> System.Numerics interop

### Tasks

**Task 2.1.1:** Add IsNearZero(epsilon) to 15+ types (4-6 hours)
**Task 2.1.2:** Implement LinBivector2D<T>.Rcp() (1 hour)
**Task 2.1.3:** Restore LinQuaternion<T> System.Numerics interop (2-3 hours)

**Expected Result:** 49/49 tests passing (28 existing + 21 new)

**See:** `DEDUPLICATION_TASKS.md` Module 2 for complete breakdown

---

## 🚀 Week 4: Module 3 & 4 - CGA & Polynomials Phase 1

### Module 3: CGA Encoders (0-30 hours depending on decision)

**Current Status:**
- ✅ Tests: 66/66 passing
- ✅ Functionally equivalent (proven)
- ⚠️ Different API signatures (Float64 double-only, Generic Hybrid)

**DECISION NEEDED:**
- **Option A (Recommended):** Keep as-is → 0 hours, proceed to Phase 2
- **Option B:** Synchronize APIs → 20-30 hours

**See:** `DEDUPLICATION_TASKS.md` Module 3

---

### Module 4: Polynomials (2-3 hours)

**Task 4.1.1:** Fix BSplineKnotVector<T>.AppendKnot() validation bug

**Expected Result:** 2/2 tests passing

**See:** `DEDUPLICATION_TASKS.md` Module 4

---

## Phase 1 Complete When:

- [ ] Module 1: 58/58 tests, 100% API Parity ✅
- [ ] Module 2: 49/49 tests, 100% API Parity ✅
- [ ] Module 3: Decision made, 66/66 tests ✅
- [ ] Module 4: 2/2 tests, bug fixed ✅
- [ ] **TOTAL: 175/175 equivalence tests passing**

**Then:** Begin Phase 2 (Thin Wrapper Migration)

---

## Phase 2 Preview (Future)

**Approach:** One module at a time, starting with Module 1

**Pattern (proven with Float32):**
1. Keep Generic implementation unchanged
2. Replace Float64 with thin wrapper
3. Run equivalence tests (should still pass)
4. Verify performance (should maintain 95%+)

**Estimated Effort per Module:**
- Module 1: 2-3 days
- Module 2: 1-2 days
- Module 3: 1 day
- Module 4: 0.5 days

**Total Phase 2:** ~1-2 weeks

---

## ⚠️ Components Deferred to Future

**Not in Phase 1 or 2:**
- **Calculus** (30% API parity) - Too large, defer
- **Trajectories** (0% Generic) - Architectural decision needed
- **BasicShapes** (100% Float64) - Decide if Generic needed
- **Statistics** (100% Float64 by design) - No action needed

---

## Bug Fix Strategy

**Rule:** Fix bugs ONLY when working on that module in Phase 1

**Bugs per Module:**
- Module 1: 1 bug (XGaPureRotor)
- Module 2: 1 bug (LinBivector2D.Rcp missing)
- Module 3: 0 bugs
- Module 4: 1 bug (BSplineKnotVector validation)

**Other bugs (NOT in Phase 1):**
- Statistics: 4 P0 bugs
- Calculus: 1 P0 bug
- Trajectories: 5 P1 bugs
- BasicShapes: 2 P1 bugs
- Signals: 1 P2 bug

**These will be fixed IF/WHEN we work on those components**

---

## Daily Workflow

### Morning:
1. Check `DEDUPLICATION_TASKS.md` for current task
2. Read task description and subtasks
3. Estimate time for today's work

### During Work:
1. Make code changes
2. Write/extend equivalence tests
3. Run tests frequently
4. Commit incrementally

### Evening:
1. Check off completed subtasks in `DEDUPLICATION_TASKS.md`
2. Update `_Status.md` if milestone reached
3. Plan next day's tasks

### Weekly:
1. Update all 4 documentation files
2. Review progress vs estimates
3. Adjust timeline if needed

---

## Key Principles

1. **100% API Parity Required** - Module must reach 100% before Phase 2
2. **Every API change = Test** - Write equivalence test for every addition
3. **One module at a time** - Complete Phase 1 fully before moving on
4. **Bugs fixed per module** - Only fix bugs when working on that module
5. **Document everything** - Update docs after every milestone

---

## Success Metrics

### Phase 1 Success:
- ✅ 175/175 equivalence tests passing
- ✅ 100% API Parity per module (4 modules)
- ✅ All module bugs fixed
- ✅ Documentation updated
- ✅ Ready for Phase 2

### Overall Project Success:
- ✅ ~132,700 LOC eliminated (59% reduction)
- ✅ Maintainability: Single source of truth (Generic)
- ✅ Performance: Maintained or improved
- ✅ Quality: All tests passing

---

## Questions & Answers

**Q: Why not start Phase 2 now?**
A: API parity is only ~60-80%, not 100%. We need to synchronize APIs first.

**Q: Why focus on 4 modules only?**
A: Core modules first (XGa, LinearAlgebra, CGA, Polynomials). Others deferred to future.

**Q: What if CGA decision is "keep as-is"?**
A: Perfect! Save 20-30 hours, proceed directly to Phase 2 for CGA.

**Q: Can I work on multiple modules in parallel?**
A: Not recommended. Complete one module fully before starting next.

**Q: How do I know I'm done with a module?**
A: When `DEDUPLICATION_TASKS.md` shows all tasks checked off and all tests passing.

---

**Next Action:** Start Module 1, Task 1.1.1 - Implement MapScalars in Float64

**First Command:**
```bash
# Open first file to edit
code GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/Float64/Multivectors/XGaFloat64Scalar.cs
```

---

*Document maintained by: Claude Code*
*Last verified against codebase: 2025-10-23*
*Branch: Feature/ScalarFloat32*
