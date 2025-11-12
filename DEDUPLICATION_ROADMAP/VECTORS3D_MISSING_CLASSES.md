# Vectors3D Missing Generic Classes Analysis

**Date:** 2025-11-11
**Status:** Phase 3A - Trajectories Vectors3D
**Current Coverage:** 33/53 classes (62%)
**Missing:** 20 classes

---

## Summary

| Category | Float64 | Generic | Missing | Priority |
|----------|---------|---------|---------|----------|
| **Basic** | 9 | 7 | 2 | P1 |
| **Bezier** | 6 | 6 | 0 | ✅ COMPLETE |
| **Circles** | 5 | 5 | 0 | ✅ COMPLETE |
| **Composers** | 2 | 1 | 1 | P1 |
| **Mapped** | 9 | 5 | 4 | P2 |
| **Adaptive** | 9 | 0 | 9 | P2 |
| **Samplers** | 6 | 0 | 6 | P2 |
| **Base Classes** | 6 | 9 | -3 | ✅ COMPLETE |

**Total:** 53 Float64 files, 33 Generic files, **20 missing**

---

## ✅ Complete Categories

### Bezier Paths (100% coverage)
All Bezier classes have Generic<T> equivalents:
- ✅ Bezier0Path3D<T>
- ✅ Bezier1Path3D<T>
- ✅ Bezier2Path3D<T>
- ✅ Bezier3Path3D<T>
- ✅ BezierNPath3D<T>
- ✅ BezierPath3DUtils<T>

### Circle Paths (100% coverage)
All circle classes implemented:
- ✅ CirclePath3D<T>
- ✅ XyCirclePath3D<T>
- ✅ YzCirclePath3D<T>
- ✅ ZxCirclePath3D<T>
- ✅ AxisAlignedCirclePath3D<T>

**Note:** IFloat64CirclePath3D is Float64-specific interface, no Generic equivalent needed.

### Base Classes (100% coverage + extras)
Generic has MORE base classes than Float64:
- ✅ ParametricPath3D<T>
- ✅ ParametricPath3DLocalFrame<T>
- ✅ ArcLengthPath3D<T>
- ✅ CatmullRomSplinePath3D<T>
- ✅ ComputedPath3D<T>
- ✅ ConstantPath3D<T>
- ✅ HarmonicPath3D<T>
- ✅ ScalarTripletPath3D<T>
- ✅ SphericalPath3D<T>

---

## ⏳ Missing Classes (Detailed)

### 1. Basic Paths (2 classes missing) - P1

**Missing:**
1. **RoulettePath3D** (Float64RoulettePath3D.cs)
   - Priority: P2
   - Description: Roulette curve generator
   - Complexity: MEDIUM
   - Estimated Time: 4-6 hours

2. **LineSegmentPath3D** (?? appears in Generic list but need to verify)
   - VERIFY: Generic list shows LineSegmentPath3D.cs exists
   - Status: MIGHT BE COMPLETE

**Already in Generic:**
- ✅ ConstantPath3D
- ✅ LineSegmentPath3D (needs verification)
- ✅ HarmonicPath3D
- ✅ SimpleHarmonicPath3D
- ✅ ScalarTripletPath3D
- ✅ SphericalPath3D
- ✅ CatmullRomSplinePath3D
- ✅ ComputedPath3D (with INumericalOperations support!)

---

### 2. Composers (1 class missing) - P1

**Missing:**
1. **Path3DComposerUtils** (Float64Path3DComposerUtils.cs)
   - Priority: P1
   - Description: Utility methods for path composition
   - Complexity: LOW-MEDIUM
   - Estimated Time: 3-5 hours

**Already in Generic:**
- ✅ SimpleHarmonicPath3DComposer

---

### 3. Mapped Paths (4 classes missing) - P2

**Missing:**
1. **AdaptiveArcLengthPath3D** (Float64AdaptiveArcLengthPath3D.cs)
   - Priority: P2
   - Description: Arc-length parameterized path with adaptive sampling
   - Complexity: HIGH
   - Dependencies: Adaptive system
   - Estimated Time: 6-8 hours

2. **RotatedNormalsArcLengthPath3D** (Float64RotatedNormalsArcLengthPath3D.cs)
   - Priority: P3
   - Description: Arc-length path with rotated normal frames
   - Complexity: MEDIUM-HIGH
   - Estimated Time: 5-7 hours

3. **RotatedNormalsPath3D** (Float64RotatedNormalsPath3D.cs)
   - Priority: P3
   - Description: Path with rotated normal frames
   - Complexity: MEDIUM
   - Estimated Time: 4-6 hours

4. **RouletteMappedPath3D** (Float64RouletteMappedPath3D.cs)
   - Priority: P3
   - Description: Roulette curve with mapping transformations
   - Complexity: MEDIUM-HIGH
   - Dependencies: RoulettePath3D
   - Estimated Time: 5-7 hours

**Already in Generic:**
- ✅ AffineMappedPath3D
- ✅ AffineMappedTimePath3D
- ✅ MappedTrajectoryPath3D
- ✅ PlusPath3D
- ✅ TimesPath3D

---

### 4. Adaptive Sampling System (9 classes missing) - P2

**Description:** Complete adaptive sampling subsystem for curvature-based refinement.

**Missing:**
1. **AdaptiveCurveTreeCornerIndex** (AdaptiveCurveTreeCornerIndex.cs)
   - Priority: P2
   - Description: Indexing system for adaptive tree corners
   - Complexity: LOW
   - Estimated Time: 2-3 hours

2. **AdaptivePath3D** (Float64AdaptivePath3D.cs)
   - Priority: P2
   - Description: Main adaptive path class
   - Complexity: HIGH
   - Estimated Time: 8-10 hours
   - **Core class - implement first**

3. **AdaptivePath3DBranch** (Float64AdaptivePath3DBranch.cs)
   - Priority: P2
   - Description: Branch node in adaptive tree
   - Complexity: MEDIUM
   - Estimated Time: 4-5 hours

4. **AdaptivePath3DCorner** (Float64AdaptivePath3DCorner.cs)
   - Priority: P2
   - Description: Corner/vertex in adaptive mesh
   - Complexity: MEDIUM
   - Estimated Time: 3-4 hours

5. **AdaptivePath3DCornerPosition** (Float64AdaptivePath3DCornerPosition.cs)
   - Priority: P2
   - Description: Position tracking for corners
   - Complexity: LOW
   - Estimated Time: 2-3 hours

6. **AdaptivePath3DLeaf** (Float64AdaptivePath3DLeaf.cs)
   - Priority: P2
   - Description: Leaf node in adaptive tree
   - Complexity: MEDIUM
   - Estimated Time: 3-4 hours

7. **AdaptivePath3DNode** (Float64AdaptivePath3DNode.cs)
   - Priority: P2
   - Description: Generic node in adaptive tree
   - Complexity: MEDIUM
   - Estimated Time: 4-5 hours

8. **AdaptivePath3DSample** (Float64AdaptivePath3DSample.cs)
   - Priority: P2
   - Description: Sample point in adaptive mesh
   - Complexity: LOW
   - Estimated Time: 2-3 hours

9. **AdaptivePath3DSamplingOptions** (Float64AdaptivePath3DSamplingOptions.cs)
   - Priority: P2
   - Description: Configuration for adaptive sampling
   - Complexity: LOW
   - Estimated Time: 2-3 hours

**Total Adaptive System Time:** ~32-42 hours (~5-6 weeks at 1h/day)

---

### 5. Samplers (6 classes missing) - P2

**Description:** Path sampling strategies for extracting point sequences.

**Missing:**
1. **IParametricCurveSampler3D** (IParametricCurveSampler3D.cs)
   - Priority: P2
   - Description: Interface for curve samplers
   - Complexity: LOW (interface only)
   - Estimated Time: 1-2 hours
   - **Implement first (base interface)**

2. **UniformParameterCurveSampler3D** (UniformParameterCurveSampler3D.cs)
   - Priority: P2
   - Description: Uniform parameter-space sampling
   - Complexity: LOW
   - Estimated Time: 2-3 hours

3. **UniformLengthCurveSampler3D** (UniformLengthCurveSampler3D.cs)
   - Priority: P2
   - Description: Uniform arc-length sampling
   - Complexity: MEDIUM
   - Estimated Time: 3-4 hours

4. **AdaptiveCurveSampler3D** (AdaptiveCurveSampler3D.cs)
   - Priority: P2
   - Description: Adaptive curvature-based sampling
   - Complexity: HIGH
   - Dependencies: Adaptive system
   - Estimated Time: 6-8 hours

5. **ConstantCurveSampler3D** (ConstantCurveSampler3D.cs)
   - Priority: P3
   - Description: Constant sampling rate
   - Complexity: LOW
   - Estimated Time: 2-3 hours

6. **ParameterListCurveSampler3D** (ParameterListCurveSampler3D.cs)
   - Priority: P3
   - Description: Sampling at specific parameter values
   - Complexity: LOW
   - Estimated Time: 2-3 hours

**Total Samplers Time:** ~16-23 hours (~3-4 weeks at 1h/day)

---

## 📊 Priority Breakdown

### P1 - High Priority (3 classes, ~7-11 hours)
**Essential for basic functionality:**
1. RoulettePath3D (if needed)
2. Path3DComposerUtils

**Rationale:** These complete the basic path functionality.

### P2 - Medium Priority (15 classes, ~48-65 hours)
**Important for advanced features:**
- Adaptive System (9 classes)
- Samplers (6 classes)

**Rationale:** Advanced sampling and adaptive refinement for high-quality curves.

### P3 - Low Priority (2 classes, ~9-13 hours)
**Nice-to-have specialized features:**
- RotatedNormalsPath3D
- RotatedNormalsArcLengthPath3D
- RouletteMappedPath3D

**Rationale:** Specialized use cases, not commonly needed.

---

## 🎯 Recommended Implementation Order

### Batch 1: Complete Basic Functionality (P1)
**Time:** ~1 week (7-11 hours)
1. Verify LineSegmentPath3D exists
2. Path3DComposerUtils<T>
3. RoulettePath3D<T> (if commonly used)

### Batch 2: Samplers Foundation (P2)
**Time:** ~2-3 weeks (16-23 hours)
1. IParametricCurveSampler3D (interface)
2. UniformParameterCurveSampler3D
3. UniformLengthCurveSampler3D
4. ConstantCurveSampler3D
5. ParameterListCurveSampler3D
6. AdaptiveCurveSampler3D (last, depends on Adaptive system)

### Batch 3: Adaptive System (P2)
**Time:** ~5-6 weeks (32-42 hours)
1. AdaptivePath3DSamplingOptions
2. AdaptivePath3DSample
3. AdaptiveCurveTreeCornerIndex
4. AdaptivePath3DCornerPosition
5. AdaptivePath3DCorner
6. AdaptivePath3DNode
7. AdaptivePath3DLeaf
8. AdaptivePath3DBranch
9. AdaptivePath3D (main class, last)

### Batch 4: Advanced Mapped Paths (P3)
**Time:** ~2-3 weeks (14-20 hours)
1. RotatedNormalsPath3D
2. RotatedNormalsArcLengthPath3D
3. RouletteMappedPath3D
4. AdaptiveArcLengthPath3D

---

## 📈 Adjusted Timeline

**Original Estimate:** ~19 classes, 3-4 weeks
**Actual Count:** 20 classes (after verification)

**Revised Estimates:**
- **P1 (Essential):** 1 week
- **P2 (Important):** 8-9 weeks
- **P3 (Optional):** 2-3 weeks

**Total for P1+P2:** ~9-10 weeks (~64-88 hours)
**Total if including P3:** ~11-13 weeks (~73-101 hours)

---

## 🔍 Next Steps

1. **Verify LineSegmentPath3D** - Check if Generic version exists
2. **Implement Batch 1** - Complete basic functionality (P1)
3. **Write equivalence tests** - 10+ tests per new class
4. **Update STATUS.md** - Track progress
5. **Begin Batch 2** - Start with samplers

---

**Last Updated:** 2025-11-11
**Maintained By:** GA-FUL Deduplication Team
