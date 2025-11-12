# Adaptive System Implementation Roadmap
**Date:** 2025-11-12
**Session:** Continuation from Samplers implementation

## 🎯 Goal
Implement complete Generic<T> Adaptive sampling subsystem to enable curvature-based adaptive refinement for parametric curves.

## 📊 Progress: 4/9 Classes Complete (44%)

### ✅ COMPLETED (4/9 classes)

1. **AdaptivePath3DCornerPosition** ✅ (NON-GENERIC)
   - File: `AdaptivePath3DCornerPosition.cs`
   - Type: Record (struct-like)
   - Dependencies: None
   - Lines: ~70
   - Description: Hierarchical tree position index using level and segment count
   - Key methods: `GetGridIndex()`, `GetInterpolationValue()`, `CompareTo()`

2. **AdaptivePath3DSamplingOptions<T>** ✅
   - File: `AdaptivePath3DSamplingOptions.cs`
   - Type: Configuration class
   - Dependencies: `LinAngle<T>`, `IScalarProcessor<T>`
   - Lines: ~110
   - Description: Sampling refinement criteria (distance, angle thresholds, level limits)
   - Properties:
     - `MaxEdgeFramesDistance` (spatial tolerance)
     - `MaxEdgeFramesAngle` (angular tolerance)
     - `MinLevelCount` / `MaxLevelCount` (tree depth limits)

3. **ParametricCurveLocalFrameInterpolationMethod** ✅ (NON-GENERIC)
   - File: `ParametricCurveLocalFrameInterpolationMethod.cs`
   - Type: Enum
   - Values:
     - `TangentLinearInterpolation = 0` (fast, less accurate)
     - `SphericalLinearInterpolation = 1` (slow, more accurate)

4. **AdaptivePath3DCorner<T>** ✅
   - File: `AdaptivePath3DCorner.cs`
   - Type: Record
   - Dependencies: `AdaptivePath3D<T>`, `AdaptivePath3DCornerPosition`, `ParametricPath3DLocalFrame<T>`
   - Lines: ~45
   - Description: Control point in adaptive tree, shared between adjacent nodes
   - Properties: `ParentTree`, `Position`, `Index`, `Frame`, `GridIndex`

---

## ❌ REMAINING (5/9 classes - ~56%)

### Priority 1: Tree Nodes (Abstract → Concrete)

#### 5. **AdaptivePath3DNode<T>** - ABSTRACT BASE CLASS
**Complexity:** HIGH
**Estimated Time:** 3-4 hours
**Lines:** ~200+ (Float64 version: 7KB)

**Dependencies:**
- `AdaptivePath3D<T>` (forward reference)
- `AdaptivePath3DBranch<T>` (circular dependency)
- `AdaptivePath3DCorner<T>` ✅
- `LinPolarAngle<T>` (for angle calculations)

**Key Properties:**
- `ParentTree: AdaptivePath3D<T>`
- `ParentBranch: AdaptivePath3DBranch<T>?`
- `Corner0, Corner1: AdaptivePath3DCorner<T>`
- `Frame0, Frame1: ParametricPath3DLocalFrame<T>`
- `Level, CellIndex: int`
- `Length0, Length1: Scalar<T>`

**Key Methods:**
- `UpdateLengthData(length0)` - Abstract, computes arc lengths
- `Contains(parameterValue)` - Check if parameter is in node range
- `ContainsLength(length)` - Check if length is in node range
- `EdgeFrameDistance()` - Euclidean distance between frames
- `EdgeFrameMaxAngle()` - Maximum angle between frame vectors
- `HasNearEdgeFrames(options)` - Refinement termination check
- `GetEdgeFramePair()` - Returns (Frame0, Frame1) tuple

**Implementation Notes:**
- Abstract class with 2 constructors (root node, child node)
- Implements `IReadOnlyCollection<AdaptivePath3DNode<T>>`
- Property `LeafNodes` yields all leaf descendants via DFS traversal

---

#### 6. **AdaptivePath3DBranch<T>** - EXTENDS NODE
**Complexity:** MEDIUM-HIGH
**Estimated Time:** 2-3 hours
**Lines:** ~120 (Float64 version: 3.6KB)

**Dependencies:**
- Extends `AdaptivePath3DNode<T>`
- Uses `AdaptivePath3DLeaf<T>`
- Uses `AdaptivePath3DSamplingOptions<T>` ✅
- Uses `ParametricCurveLocalFrameSamplingMethod` (enum - needs porting!)

**Key Properties:**
- `Child0, Child1: AdaptivePath3DNode<T>` (can be Branch or Leaf)
- `Count: int` (always 2)

**Key Methods:**
- `GenerateTree(options)` - Recursive tree generation with adaptive subdivision
- `CreateBranchChildren(options)` - Split into 2 sub-branches
- `GetChildContaining(parameterValue)` - Traversal helper
- `GetChildContainingLength(length)` - Arc-length traversal helper
- `UpdateLengthData(length0)` - Recursively update arc lengths

**Refinement Logic:**
```csharp
var continueSubdivision =
    IsRoot ||
    Level < options.MinLevelCount ||
    (Level < options.MaxLevelCount && !HasNearEdgeFrames(options));
```

**Missing Dependency:** `ParametricCurveLocalFrameSamplingMethod` enum
- Values: `MinimizedRotation`, `SimpleRotation`
- Used to update frame normals: `Frame1.SetMinimizedRotationNormals(Frame0)`

---

#### 7. **AdaptivePath3DLeaf<T>** - EXTENDS NODE (Terminal)
**Complexity:** LOW
**Estimated Time:** 1-2 hours
**Lines:** ~50 (Float64 version: 1.6KB)

**Dependencies:**
- Extends `AdaptivePath3DNode<T>`
- Uses `AdaptivePath3DBranch<T>` (parent reference)
- Uses `ILineSegment3D<T>` (geometry interface)

**Key Properties:**
- `LeafListIndex: int` - Index in parent tree's leaf array
- `PrevLeafNode, NextLeafNode: AdaptivePath3DLeaf<T>?` - Linked list navigation
- `Count: int` (always 0 - no children)

**Key Methods:**
- `GetLineSegment()` - Returns line segment between Frame0 and Frame1 points
- `UpdateLengthData(length0)` - Computes Length1 = Length0 + distance(Frame0, Frame1)
- `GetEnumerator()` - Returns empty (terminal node)

---

#### 8. **AdaptivePath3DSample<T>** - INTERPOLATION
**Complexity:** MEDIUM
**Estimated Time:** 2-3 hours
**Lines:** ~130 (Float64 version: 4.3KB)

**Dependencies:**
- `AdaptivePath3DLeaf<T>`
- `ParametricCurveLocalFrameInterpolationMethod` ✅
- `SquareMatrix4<T>` (rotation matrices)
- `LinVector3D<T>.Lerp()` (linear interpolation)
- `LinVector3D<T>.VectorToVectorRotationAxisAngle()` (spherical interpolation)

**Key Properties:**
- `LeafNode: AdaptivePath3DLeaf<T>`
- `ParameterValue: Scalar<T>` - Evaluation parameter
- `InterpolationValue: Scalar<T>` - Normalized t ∈ [0,1] within leaf segment
- `FrameInterpolationMethod` - Forwarded from parent tree

**Key Methods:**
- `GetPoint()` - Interpolate position (always linear)
- `GetTangent()` - Interpolate tangent (linear OR spherical)
- `GetFrame()` - Interpolate complete frame (position + tangent + normals)

**Interpolation Modes:**
1. **TangentLinearInterpolation:**
   ```csharp
   tangent = Lerp(Frame0.Tangent, Frame1.Tangent).Normalize()
   normals = RotateNormals(Frame0.Tangent → tangent)
   ```

2. **SphericalLinearInterpolation:**
   ```csharp
   (axis, angle) = Frame0.Tangent.RotationAxisAngle(Frame1.Tangent)
   (tangent, normals) = RotationMatrix(axis, angle * t).Transform(Frame0)
   ```

**Edge Cases:**
- If `ParameterValue == LeafNode.MinParameterValue` → return `Frame0` directly
- If `ParameterValue == LeafNode.MaxParameterValue` → return `Frame1` directly

---

### Priority 2: Main Tree Class

#### 9. **AdaptivePath3D<T>** - MAIN ADAPTIVE TREE
**Complexity:** VERY HIGH
**Estimated Time:** 5-8 hours
**Lines:** ~500+ (Float64 version: 18.9KB - largest class!)

**Dependencies:**
- `ParametricPath3D<T>` (base curve)
- `AdaptivePath3DSamplingOptions<T>` ✅
- `AdaptivePath3DNode<T>`
- `AdaptivePath3DBranch<T>`
- `AdaptivePath3DLeaf<T>`
- `AdaptivePath3DCorner<T>` ✅
- `AdaptivePath3DSample<T>`
- `ParametricCurveLocalFrameInterpolationMethod` ✅
- `ParametricCurveLocalFrameSamplingMethod` (enum - needs porting!)

**Key Properties:**
- `BaseCurve: ParametricPath3D<T>` - Source curve
- `ParameterRange: ScalarRange<T>` - Sampling interval
- `RootNode: AdaptivePath3DBranch<T>` - Tree root
- `LeafNodesList: IReadOnlyList<AdaptivePath3DLeaf<T>>` - All leaf nodes (flat array)
- `CornersList: IReadOnlyList<AdaptivePath3DCorner<T>>` - All corner points
- `TreeLevelCount: int` - Maximum tree depth
- `FrameInterpolationMethod` - How to interpolate frames
- `FrameSamplingMethod` - How to compute normals
- `TotalLength: Scalar<T>` - Total arc length

**Key Methods:**
- **Construction & Generation:**
  - `GenerateTree()` - Build adaptive tree recursively
  - `GetOrAddCorner(position)` - Lazy corner creation with deduplication
  - `AddLeafNode(leaf)` - Register leaf in flat list

- **Query & Traversal:**
  - `GetLeafNodeContaining(parameterValue)` - Find leaf for parameter
  - `GetLeafNodeContainingLength(length)` - Find leaf for arc length
  - `GetValue(parameter)` - Interpolated position
  - `GetTangent(parameter)` - Interpolated tangent
  - `GetFrame(parameter)` - Interpolated local frame
  - `GetTimeValues()` - All corner parameters (sorted)
  - `GetTangents()` - All corner tangents

- **Arc Length Parameterization:**
  - `TimeToLength(parameter)` - Parameter → arc length
  - `LengthToTime(length)` - Arc length → parameter
  - `UpdateLengthData()` - Compute cumulative arc lengths

**Internal Data Structures:**
- `Dictionary<AdaptivePath3DCornerPosition, AdaptivePath3DCorner<T>>` - Corner cache
- `List<AdaptivePath3DLeaf<T>>` - Leaf registry
- Stack-based DFS traversal for queries

**Critical Patterns:**
1. **Corner Deduplication:** Adjacent nodes share corners → use dictionary cache
2. **Lazy Evaluation:** Tree generated once, then queried multiple times
3. **Hybrid Storage:** Tree structure + flat leaf array for efficient iteration

---

## 🔗 Dependency Graph

```
CornerPosition (struct) ──┐
                          ├──> Corner<T> ──┐
AdaptivePath3D<T> ────────┘                ├──> Node<T> (abstract)
                                           │         ├──> Branch<T>
                                           │         │       └──> GenerateTree()
SamplingOptions<T> ────────────────────────┘         └──> Leaf<T>
                                                                └──> Sample<T>
```

**Circular Dependencies:**
- `Node ↔ AdaptivePath3D` (parent reference)
- `Node ↔ Branch` (parent-child)
- `Branch → Leaf` (child type)

---

## 📋 Implementation Order (Recommended)

### Batch 1: Foundation (DONE ✅)
1. CornerPosition (struct)
2. SamplingOptions<T>
3. InterpolationMethod (enum)
4. Corner<T>

### Batch 2: Tree Nodes (~5-9 hours)
5. **Node<T>** (abstract base) - Start here!
6. **Branch<T>** (extends Node)
7. **Leaf<T>** (extends Node)

**Dependencies to port:**
- `ParametricCurveLocalFrameSamplingMethod` enum (MinimizedRotation, SimpleRotation)
- `ILineSegment3D<T>` interface (may already exist in Generic)

### Batch 3: Interpolation (~2-3 hours)
8. **Sample<T>**

### Batch 4: Main Tree Class (~5-8 hours)
9. **AdaptivePath3D<T>**

---

## 🎯 Total Estimated Time

- **Remaining:** 12-18 hours (~2-3 weeks at 1h/day)
- **Node:** 3-4 hours
- **Branch:** 2-3 hours
- **Leaf:** 1-2 hours
- **Sample:** 2-3 hours
- **AdaptivePath3D:** 5-8 hours

---

## 🚧 Known Challenges

1. **SquareMatrix4<T> Methods:**
   - `CreateRotationMatrix3D(axis, angle)` - Check if exists in Generic
   - `MapAffineVector(vector)` - Matrix-vector transformation
   - `MapAffineVectors(...)` - Batch transformation

2. **LinVector3D<T> Methods:**
   - `VectorToVectorRotationAxisAngle()` - Rodrigues formula
   - `GetAngle(other)` - Angle between vectors
   - `Lerp(v1, v2, t)` - Linear interpolation
   - `ToUnitLinVector3D()` - Normalization

3. **LocalFrame Methods:**
   - `SetMinimizedRotationNormals(otherFrame)` - Minimize rotation between frames
   - `SetSimpleRotationNormals(otherFrame)` - Simple parallel transport

4. **Type-Specific Fast-Paths:**
   - Adaptive system is performance-critical (recursive tree generation)
   - Should apply "Phase 1 Optimization" patterns (typeof(T) checks, local accumulators)

---

## ✅ Success Criteria

**Milestone 1:** All 9 classes compile
**Milestone 2:** AdaptiveCurveSampler3D<T> works
**Milestone 3:** Equivalence tests pass vs Float64 baseline
**Milestone 4:** Performance benchmarks (should be 1.3-2x faster than Float64!)

---

## 📚 Next Session Starting Point

```csharp
// Start with AdaptivePath3DNode<T> abstract base class
// Location: GeometricAlgebraFulcrumLib.Modeling/Trajectories/Vectors3D/Generic/Adaptive/

public abstract class AdaptivePath3DNode<T> :
    IReadOnlyCollection<AdaptivePath3DNode<T>>
{
    // Copy from Float64AdaptivePath3DNode.cs
    // Replace Float64-specific types with Generic<T> equivalents
    // Apply scalar processor patterns learned from Samplers
}
```

**First task:** Port `ParametricCurveLocalFrameSamplingMethod` enum
**Second task:** Implement `AdaptivePath3DNode<T>` abstract class (~200 LOC)
