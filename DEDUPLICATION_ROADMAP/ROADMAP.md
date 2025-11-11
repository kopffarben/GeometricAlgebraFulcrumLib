# Project Roadmap

**Strategy:** Generic-First - Implement Generic<T>, deprecate Float64 specialized
**Goal:** 100% Generic coverage, eliminate ~78,500 LOC of duplicate code
**Status:** Phase 1 Complete ✅ | Phase 3 In Progress 🚀 (60%)

---

## 📋 Phase Overview

| Phase | Status | Duration | Description |
|-------|--------|----------|-------------|
| **Phase 1** | ✅ COMPLETE | 3 weeks (actual: ~20h!) | XGa Core, ComplexAlgebra, VGA |
| **Phase 1 Optimizations** | ✅ COMPLETE | 1 week | Performance breakthrough |
| **Infrastructure** | ✅ COMPLETE | 1 week (actual: ~9h!) | INumericalOperations<T> |
| **Phase 2** | ⏭️ SKIPPED | - | Thin Wrapper (no longer needed) |
| **Phase 3A** | 🚀 60% COMPLETE | ~5-6 weeks | Trajectories (57/151 done) |
| **Phase 3B** | 📋 PLANNED | ~3 weeks | Calculus Core |
| **Phase 3C** | 📋 PLANNED | ~2 weeks | Signals |
| **Phase 3D** | 📋 PLANNED | Variable | Advanced/Optional modules |

**Total Remaining:** ~10-12 weeks for Phase 3A-C (core functionality)

---

## 🎯 Phase 3: Modeling Layer (IN PROGRESS)

**Overall Goal:** Migrate 151 Float64 Modeling classes to Generic<T>
**Current Progress:** 57/151 classes complete (38%)
**Remaining Work:** ~94 classes (~10-12 weeks)

### Why Phase 2 Was Skipped

**Original Plan:** Create "Thin Wrapper" - Float64 calls Generic<double>
**Reality:** Generic<T> is **1.39-2.31x FASTER** than Float64!
**Decision:** Skip thin wrapper, directly implement remaining Generic<T> classes
**Benefit:** Cleaner architecture, no wrapper overhead

---

## 🚀 Phase 3A: Trajectories (IN PROGRESS - 60% Complete)

**Status:** 57/151 classes implemented
**Estimated Remaining:** ~5-6 weeks

### Module 6A: Trajectories Vectors3D

**Progress:** 33/~52 classes (63% complete)
**Remaining:** ~19 classes (~3-4 weeks)

#### ✅ Completed Categories

**Basic Paths (8 classes):**
- `ConstantPath3D<T>` ✅
- `LineSegmentPath3D<T>` ✅
- `HarmonicPath3D<T>` ✅
- `SphericalPath3D<T>` ✅
- `ScalarTripletPath3D<T>` ✅
- `SimpleHarmonicPath3D<T>` ✅
- `ComputedPath3D<T>` ✅ (with INumericalOperations)
- `CatmullRomSplinePath3D<T>` ✅ (with INumericalOperations)

**Bezier Paths (6 classes):**
- `Bezier0Path3D<T>` ✅ (constant)
- `Bezier1Path3D<T>` ✅ (linear)
- `Bezier2Path3D<T>` ✅ (quadratic)
- `Bezier3Path3D<T>` ✅ (cubic)
- `BezierNPath3D<T>` ✅ (N-degree)
- `BezierPath3DUtils<T>` ✅ (utilities)

**Circle Paths (5 classes):**
- `CirclePath3D<T>` ✅
- `XyCirclePath3D<T>` ✅ (XY plane)
- `YzCirclePath3D<T>` ✅ (YZ plane)
- `ZxCirclePath3D<T>` ✅ (ZX plane)
- `AxisAlignedCirclePath3D<T>` ✅

**Mapped Paths (5 classes):**
- `AffineMappedPath3D<T>` ✅
- `AffineMappedTimePath3D<T>` ✅
- `MappedTrajectoryPath3D<T>` ✅
- `PlusPath3D<T>` ✅
- `TimesPath3D<T>` ✅

**Composers (1 class):**
- `SimpleHarmonicPath3DComposer<T>` ✅

**Base Classes (3 classes):**
- `ParametricPath3D<T>` ✅ (abstract base)
- `ParametricPath3DLocalFrame<T>` ✅ (frame computations)
- `ArcLengthPath3D<T>` ✅ (arc-length parameterization)

#### ⏳ Remaining Classes (~19 classes)

**Adaptive Sampling (~8 classes):**
- `AdaptivePath3D<T>` - Adaptive sampling based on curvature
- `AdaptivePath3DComposer<T>` - Builder for adaptive paths
- `AdaptivePath3DSampler<T>` - Sampling strategies
- ~5 more adaptive-related classes

**Path Composers (~5 classes):**
- `Path3DComposer<T>` - General path builder
- `Path3DComposerUtils<T>` - Utility methods
- ~3 more composer classes

**Path Samplers (~5 classes):**
- `Path3DSampler<T>` - Sampling interface
- `UniformPath3DSampler<T>` - Uniform sampling
- `AdaptiveGradingPath3DSampler<T>` - Grade-based sampling
- ~2 more sampler classes

**Others (~1 class):**
- Any remaining utility or specialized path classes

**Estimated Time:** 3-4 weeks (assuming same pace as Phase 1: ~5h per class)

---

### Module 6B: Trajectories Vectors2D

**Progress:** 24/~40 classes (60% complete)
**Remaining:** ~16 classes (~2-3 weeks)

#### ✅ Completed Categories

**Basic Paths (10 classes):**
- `ConstantPath2D<T>` ✅
- `LineSegmentPath2D<T>` ✅
- `CirclePath2D<T>` ✅
- `HarmonicPath2D<T>` ✅
- `PolarPath2D<T>` ✅
- `ScalarPairPath2D<T>` ✅
- `SimpleHarmonicPath2D<T>` ✅
- `ComputedPath2D<T>` ✅ (with INumericalOperations)
- `CatmullRomSplinePath2D<T>` ✅ (with INumericalOperations)

**Bezier Paths (6 classes):**
- `Bezier0Path2D<T>` ✅
- `Bezier1Path2D<T>` ✅
- `Bezier2Path2D<T>` ✅
- `Bezier3Path2D<T>` ✅
- `BezierNPath2D<T>` ✅
- `BezierPath2DUtils<T>` ✅

**Mapped Paths (5 classes):**
- `AffineMappedPath2D<T>` ✅
- `AffineMappedTimePath2D<T>` ✅
- `MappedTrajectoryPath2D<T>` ✅
- `PlusPath2D<T>` ✅
- `TimesPath2D<T>` ✅

**Composers (1 class):**
- `SimpleHarmonicPath2DComposer<T>` ✅

**Base Classes (2 classes):**
- `ParametricPath2D<T>` ✅
- `ParametricPath2DLocalFrame<T>` ✅
- `ArcLengthPath2D<T>` ✅

#### ⏳ Remaining Classes (~16 classes)

**Adaptive Sampling (~6 classes):**
- `AdaptivePath2D<T>`
- `AdaptivePath2DComposer<T>`
- ~4 more adaptive classes

**Path Composers (~5 classes):**
- `Path2DComposer<T>`
- ~4 more composer classes

**Path Samplers (~4 classes):**
- `Path2DSampler<T>`
- `UniformPath2DSampler<T>`
- ~2 more sampler classes

**Others (~1 class):**
- Remaining utility classes

**Estimated Time:** 2-3 weeks

---

### Module 6C: Trajectories Scalars (FUTURE)

**Progress:** Unknown (need to verify existing Generic classes)
**Estimated:** ~40 Float64 classes total
**Priority:** P1 (needed for complete trajectory functionality)
**Estimated Time:** ~3-4 weeks

**Categories:**
- Angles (5 classes)
- Basic (8 classes)
- Composers (7 classes)
- Mapped (5 classes)
- Normalized (5 classes)
- Parametric (8 classes)
- Plots (2 classes)

**Dependency:** Can start after Module 6A/6B completion

---

### Module 6D: Trajectories Others (FUTURE)

**Progress:** 0/11 classes
**Estimated Time:** ~2 weeks
**Priority:** P2 (nice-to-have, less commonly used)

**Categories:**
- Bivectors2D (3 classes)
- Bivectors3D (2 classes)
- Quaternions (4 classes) - SLERP, SQUAD
- Trivectors3D (2 classes)

**Dependency:** Can start after Module 6A/6B/6C

---

## 📚 Phase 3B: Calculus Core (PLANNED)

**Status:** Not started
**Estimated:** ~3 weeks
**Priority:** P1 (critical for differential geometry)

### Module 7A: DifferentialFunction Hierarchy

**Scope:** ~35 classes
**Complexity:** HIGH (automatic differentiation, symbolic operations)

#### Core Classes (~20 classes)

**Base:**
- `DifferentialFunction<T>` - Abstract base
- `DifferentialBasicFunction<T>` - Variables, constants
- `DifferentialUnaryFunction<T>` - Single-argument functions
- `DifferentialBinaryFunction<T>` - Two-argument functions
- `DifferentialNaryFunction<T>` - N-argument functions
- `DifferentialCompositeFunction<T>` - Function composition
- `DifferentialCustomFunction<T>` - User-defined

**Concrete Functions (15 classes):**
- `DfVar<T>` - Variable
- `DfConstant<T>` - Constant value
- `DfCos<T>`, `DfSin<T>` - Trigonometric
- `DfExp<T>` - Exponential
- `DfPlus<T>`, `DfTimes<T>` - Arithmetic
- `DfPowerScalar<T>` - Power function
- `DfSmoothBlend<T>` - Smooth blending
- `DfFiniteSupport<T>` - Finite support function
- ~5 more concrete functions

#### Constant Value Hierarchy (~10 classes)

- `DfConstantValue<T>`
- `DfConstantValueE<T>`, `DfConstantValuePi<T>`
- `DfConstantValueInteger<T>`, `DfConstantValueRational<T>`
- `DfConstantValueFloat<T>`, `DfConstantValueDecimal<T>`
- `DfConstantValuePlus<T>`, `DfConstantValueTimes<T>`

#### Utilities (~5 classes)

- `MathDf<T>` - Math utilities for differential functions
- `DifferentialUtils<T>` - Helper methods
- `ScalarFunctionProcessorOfT<T>` - Function processing

**Key Features:**
- AutoDiff-compatible (automatic differentiation)
- Symbolische Differentiation
- Expression tree management

**Estimated Time:** 3 weeks (high complexity, foundational for other modules)

---

## 🔊 Phase 3C: Signals (PLANNED)

**Status:** Not started
**Estimated:** ~2 weeks
**Priority:** P1 (important for signal processing)

### Module 8: Signal Processing

**Scope:** ~11 classes
**Current Generic:** 3 classes (Processor, Spectrum, HarmonicComposer)
**Remaining:** 8 classes

#### ⏳ To Implement

**Core Signal Processing (3 classes):**
- `SampledTimeSignal<T>` - **LARGEST CLASS** (~1,655 LOC!)
  - FFT, IFFT (using INumericalOperations?)
  - Integration, Energy computation
  - Fourier spectrum, interpolation
  - Operators: +, -, *, /
  - **Estimated:** 1-1.5 weeks alone!
- `SamplingSpecs<T>` - Sampling specifications
- `ComplexSignalSpectrum<T>` - Complex spectrum analysis

**Analysis (3 classes):**
- `SignalHistogram<T>` - Histogram analysis
- `SignalLog2Histogram<T>` - Log-scale histogram
- `SignalSpectrum<T>` - Spectrum analysis

**Composers & Utils (2 classes):**
- `SampledTimeSignalComposer<T>` - Signal builder
- `SignalUtils<T>` - Utility methods

**Challenges:**
- FFT/IFFT requires numerical library (MathNet.Numerics for double/float)
- May need to provide fallback or error for non-numeric types

**Estimated Time:** 2 weeks

---

## 📐 Phase 3D: Advanced/Optional (FUTURE)

**Priority:** P2-P3 (nice-to-have, not critical)
**Estimated:** Variable (6-10 weeks total)

### Module 7B: Calculus Advanced (~35+ classes)

**Scope:** Advanced calculus features
**Complexity:** VERY HIGH
**Estimated:** ~5-8 weeks

**Categories:**
- **AutoDiff System (~40 classes)** - HARDCODED double, difficult to generify
- **Interpolators (14 classes)** - Akima, Catmull-Rom, Chebyshev, Fourier
- **Polynomials (9 classes)** - Bernstein, Chebyshev, Monomial basis
- **Differential Curves (6 classes)** - DifferentialPath3D (800 LOC!)
- **Fourier (4 classes)** - Vector/Multivector Fourier curves

**Decision:** May skip AutoDiff (too difficult to generify, limited value)

### Module 9: Statistics (~15 classes)

**Scope:** Statistical analysis and random generation
**Complexity:** LOW-MEDIUM
**Estimated:** ~1.5 weeks

**Categories:**
- Continuous Distributions (8 classes)
- Discrete Distributions (3 classes)
- Random Generators (3 classes)

### Module 10: PropagatorNetworks (~10 classes)

**Scope:** Constraint propagation system
**Complexity:** MEDIUM
**Estimated:** ~1.25 weeks
**Priority:** P3 (specialized domain, not widely used)

**Categories:**
- Core Classes (3 classes)
- Propagator Operations (6 classes)
- Utilities (1 class)

---

## 🎯 Success Criteria

### Phase 3A (Trajectories) Complete When:
- [ ] All ~94 remaining Trajectory classes implemented in Generic<T>
- [ ] 10+ equivalence tests per new class (100% pass rate)
- [ ] Performance ≥95% of Float64 (we already exceed this!)
- [ ] Documentation updated (STATUS.md + ROADMAP.md)

### Phase 3B (Calculus Core) Complete When:
- [ ] All ~35 DifferentialFunction classes implemented
- [ ] Full integration with INumericalOperations<T>
- [ ] Symbolic differentiation working for Entity type
- [ ] 10+ tests per class

### Phase 3C (Signals) Complete When:
- [ ] SampledTimeSignal<T> fully functional (including FFT for double/float)
- [ ] All 11 Signal classes implemented
- [ ] Integration with Trajectory classes working
- [ ] Signal processing tests passing

### Overall Phase 3 Complete When:
- [ ] 100% Generic coverage for Modeling layer (all ~200+ classes)
- [ ] All tests passing (≥98% pass rate maintained)
- [ ] Float64 classes can be deprecated
- [ ] Documentation complete

---

## ⏱️ Timeline Summary

| Phase | Duration | Start | End (Estimated) |
|-------|----------|-------|-----------------|
| Phase 1 | ✅ 3 weeks | 2025-09-01 | 2025-10-25 |
| Infrastructure | ✅ 1 week | 2025-10-25 | 2025-11-05 |
| Phase 3A | 🚀 5-6 weeks | 2025-11-05 | 2025-12-20 |
| Phase 3B | 📋 3 weeks | 2025-12-20 | 2026-01-10 |
| Phase 3C | 📋 2 weeks | 2026-01-10 | 2026-01-24 |
| **Total (Core)** | **~13-14 weeks** | 2025-09-01 | **2026-01-24** |

**Optional (Phase 3D):** +6-10 weeks (if needed)

---

## 📊 Dependencies

```
Phase 1 (XGa Core)
  ↓
INumericalOperations Infrastructure
  ↓
Phase 3A (Trajectories) ← Currently Here
  ↓
Phase 3B (Calculus Core DifferentialFunction)
  ↓
Phase 3C (Signals: SampledTimeSignal<T>)
  ↓
[Optional] Phase 3D (Advanced features)
```

**Note:** Module 6C (Scalars) can run in parallel with 6A/6B completion.

---

## 🚧 Blockers & Risks

### Current Blockers: NONE ✅

All infrastructure is in place:
- ✅ INumericalOperations<T> complete
- ✅ Performance proven (Generic faster than Float64)
- ✅ Test framework established
- ✅ 57 Generic trajectory classes as examples

### Potential Risks

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| Scope creep (more classes than expected) | Medium | Medium | Verified counts via code inspection |
| Performance regression | Low | High | Type-specific fast-paths proven |
| Test failures increase | Low | Medium | 10+ tests per class, 100% pass required |
| FFT/Signal processing complexity | Medium | Medium | Fallback to error for non-numeric types |
| AutoDiff too hard to generify | High | Low | Can skip (P3 priority, limited value) |

---

## 🎉 Milestones

### Completed Milestones ✅

- [x] **2025-10-25:** Phase 1 Complete - All core GA classes in Generic<T>
- [x] **2025-10-27:** Performance Breakthrough - Generic 1.39-2.31x faster!
- [x] **2025-11-05:** INumericalOperations Infrastructure - All 3 backends working
- [x] **2025-11-05:** Existing Trajectories Updated - 4 classes edge-cases fixed

### Upcoming Milestones

- [ ] **2025-12-01:** Module 6A Complete - All Vectors3D trajectories (~3 weeks)
- [ ] **2025-12-15:** Module 6B Complete - All Vectors2D trajectories (~2 weeks)
- [ ] **2025-12-20:** Phase 3A Complete - All trajectories done
- [ ] **2026-01-10:** Phase 3B Complete - Calculus Core functional
- [ ] **2026-01-24:** Phase 3C Complete - Signal processing functional
- [ ] **2026-01-24:** **PROJECT COMPLETE** - 100% Generic coverage! 🎉

---

**For current status:** → [STATUS.md](STATUS.md)
**For architecture details:** → [ARCHITECTURE.md](ARCHITECTURE.md)
**For historical plans:** → [archive/2025-11-05/](archive/2025-11-05/)
