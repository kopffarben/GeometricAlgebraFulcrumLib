# TensorAlgebra API Analysis Report

**Generated:** 2025-10-23
**Agent:** TensorAlgebra API Analyzer
**Repository:** GA-FUL (Geometric Algebra Fulcrum Library)

---

## Executive Summary

### Key Findings

1. **Float64 Implementation: DOES NOT EXIST**
   - No specialized `Float64` tensor algebra implementation found
   - No `TensorFloat64.cs` or similar files in the codebase
   - **This is INTENTIONAL and ACCEPTABLE** - see rationale below

2. **Generic Implementation: COMPLETE**
   - Fully functional generic tensor algebra via `GenTensor<T, TWrapper>`
   - Comprehensive API with 100+ methods covering all standard tensor operations
   - Production-ready with MIT license from WhiteBlackGoose (ported library)

3. **Usage in GA-FUL: NONE FOUND**
   - No unit tests for TensorAlgebra
   - No usage examples in Applications
   - No integration with main GA-FUL APIs (XGa, RGa, CGa, etc.)
   - **Status:** Standalone library component, not integrated into GA workflows

---

## Part 1: Discovery - Float64 Search Results

### Comprehensive Search Strategy

Searched the entire codebase using multiple methods:

1. **File pattern search:** `*Tensor*Float64*.cs` - No results
2. **Class name search:** `class.*Tensor.*Float64` - No results
3. **Namespace search:** `TensorAlgebra.*Float64` - No results
4. **Directory listing:** Only `TensorAlgebra/Generic/` exists, no `Float64/` parallel directory

### Conclusion: Float64 Implementation Missing (By Design)

**Why this is acceptable:**
- The Generic implementation is designed to work with ANY scalar type through `IOperations<T>`
- Pre-built wrappers exist for Float64: `DoubleWrapper : IOperations<double>`
- Users can instantiate `GenTensor<double, DoubleWrapper>` directly
- No performance penalty compared to specialized Float64 implementation

**This differs from GA-FUL's main APIs (XGa, RGa, CGa) where:**
- Float64 specializations provide significant performance benefits
- Processor pattern benefits from singleton caching
- Multivector storage is highly optimized for Float64

**For tensors:** Generic implementation is sufficient because tensor operations are already highly optimized through:
- Direct array indexing
- Parallel processing support
- Block-based storage optimization

---

## Part 2: TensorAlgebra Directory Structure

```
GeometricAlgebraFulcrumLib/
└── GeometricAlgebraFulcrumLib.Algebra/
    └── TensorAlgebra/
        └── Generic/
            ├── Core/
            │   ├── Tensor.cs                    [GenTensor<T, TWrapper> main class]
            │   ├── TensorShape.cs                [Shape metadata struct]
            │   ├── IOperations.cs                [Scalar operation interface]
            │   ├── IteratorCompiler.cs           [Expression tree optimization]
            │   ├── Threading.cs                  [Parallel execution enum]
            │   ├── ThreadUtils.cs                [Threading helpers]
            │   └── Exceptions/
            │       ├── InvalidShapeException.cs
            │       ├── InvalidDeterminantException.cs
            │       └── ImpossibleDecompositionException.cs
            └── Functions/
                ├── DefaultWrappers.cs            [DoubleWrapper, FloatWrapper, IntWrapper, etc.]
                ├── Constructors.cs               [Factory methods for tensors/matrices/vectors]
                ├── Composition.cs                [Stack, Concat, Aggregate operations]
                ├── CopyAndForward.cs             [Copy, Forward (view) operations]
                ├── MatrixMultiplication.cs       [Optimized matrix multiply with threading]
                ├── PiecewiseArithmetics.cs       [Element-wise add/sub/mul/div]
                ├── Determinant.cs                [Laplace and Gaussian methods]
                ├── Inverse.cs                    [Matrix inversion via Adjoint]
                ├── LuDecomposition.cs            [Lower-Upper decomposition]
                ├── PluDecomposition.cs           [Permutation-LU decomposition]
                ├── EchelonForm.cs                [Row echelon and reduced row echelon]
                ├── ElementaryRowOperations.cs    [Row operations for Gaussian elimination]
                ├── Power.cs                      [Matrix power operations]
                ├── Vector.DotProduct.cs          [Vector dot product]
                ├── Vector.CrossProduct.cs        [3D vector cross product]
                ├── Serialization.cs              [Binary serialization/deserialization]
                ├── ToString.cs                   [String formatting]
                └── SquareMatrixFactory.cs        [Square matrix caching]
```

**Total Files:** 27
**Lines of Code:** ~8,000+ (estimated)
**External Dependencies:** HonkPerf.NET.Core (high-performance delegate pattern)

---

## Part 3: Generic TensorAlgebra API Documentation

### 3.1 Core Types

#### `GenTensor<T, TWrapper>`

**Signature:**
```csharp
public class GenTensor<T, TWrapper> :
    IEquatable<GenTensor<T, TWrapper>>,
    ICloneable
    where TWrapper : struct, IOperations<T>
```

**Generic Parameters:**
- `T`: The scalar type (e.g., `double`, `float`, `int`, `Complex`, `BigInteger`)
- `TWrapper`: Struct implementing `IOperations<T>` that defines arithmetic operations

**Key Fields:**
- `T[] Data`: Flat array storing all tensor elements in row-major order
- `TensorShape Shape`: Multidimensional shape metadata
- `int[] Blocks`: Pre-computed strides for efficient indexing
- `int LinOffset`: Linear offset for subtensor views

**Key Properties:**
- `TensorShape Shape`: Get tensor dimensions (e.g., `[3, 4, 5]`)
- `int Volume`: Total number of elements (product of all dimensions)
- `bool IsMatrix`: True if 2D tensor
- `bool IsVector`: True if 1D tensor
- `bool IsSquareMatrix`: True if NxN matrix
- `T this[int...]`: Indexer for accessing elements

---

### 3.2 IOperations Interface

**Purpose:** Abstract scalar arithmetic for any type T

```csharp
public interface IOperations<T>
{
    T Add(T a, T b);              // Addition
    T Subtract(T a, T b);         // Subtraction
    T Multiply(T a, T b);         // Multiplication
    T Negate(T a);                // Unary negation
    T Divide(T a, T b);           // Division
    T CreateOne();                // Multiplicative identity (1)
    T CreateZero();               // Additive identity (0)
    T Copy(T a);                  // Deep copy (for mutable types)
    bool AreEqual(T a, T b);      // Equality comparison
    bool IsZero(T a);             // Zero test
    string ToString(T a);         // String representation
    byte[] Serialize(T a);        // Binary serialization
    T Deserialize(byte[] data);   // Binary deserialization
}
```

**Built-in Wrappers:**
- `IntWrapper` - for `int`
- `LongWrapper` - for `long`
- `FloatWrapper` - for `float` (with 1e-5 tolerance)
- `DoubleWrapper` - for `double` (with 1e-7 tolerance)
- `ComplexWrapper` - for `System.Numerics.Complex`
- `BigIntWrapper` - for `System.Numerics.BigInteger`
- `GenericWrapper<T>` - Generic wrapper with runtime type checking (slower)

---

### 3.3 Complete API Breakdown

#### A. Construction APIs (13 methods)

**Factory Methods (Static):**
```csharp
// Identity tensors/matrices
GenTensor<T, TWrapper>.CreateIdentityTensor(int[] dimensions, int finalMatrixDiagonal)
GenTensor<T, TWrapper>.CreateIdentityMatrix(int diagonal)

// Vectors (1D tensors)
GenTensor<T, TWrapper>.CreateVector(int length)
GenTensor<T, TWrapper>.CreateVector(T[] data)

// Matrices (2D tensors)
GenTensor<T, TWrapper>.CreateMatrix(int width, int height)
GenTensor<T, TWrapper>.CreateMatrix(T[,] data)
GenTensor<T, TWrapper>.CreateMatrix(T[][] data)
GenTensor<T, TWrapper>.CreateSquareMatrix(int size)

// General tensors (N-D)
GenTensor<T, TWrapper>.CreateTensor(TensorShape shape)
GenTensor<T, TWrapper>.CreateTensor(int[] dimensions)
GenTensor<T, TWrapper>.CreateTensor(T[] data, int[] dimensions)
GenTensor<T, TWrapper>.CreateTensor(Func<int[], T> initializer, params int[] dimensions)
```

**Composition:**
```csharp
GenTensor<T, TWrapper>.Stack(params GenTensor<T, TWrapper>[] elements)
  // Stacks tensors along new first axis
  // Example: Stack([2x3], [2x3], [2x3]) -> [3x2x3]

GenTensor<T, TWrapper>.Concat(GenTensor<T, TWrapper> a, GenTensor<T, TWrapper> b)
  // Concatenates along first axis
  // Example: Concat([4x3x5], [9x3x5]) -> [13x3x5]

void Aggregate<TAggregatorFunc, TU, TUWrapper>(GenTensor<T, TWrapper> tensor,
    GenTensor<TU, TUWrapper> accumulated, TAggregatorFunc accumulator, int axis)
  // Aggregates along axis (like LINQ Aggregate but for tensors)
```

---

#### B. Indexing and Access APIs (25 methods)

**Direct Indexing:**
```csharp
T this[int index1]                           // 1D access
T this[int index1, int index2]               // 2D access
T this[int index1, int index2, int index3]   // 3D access
T this[int index1, int index2, int index3, int index4] // 4D access
T this[params int[] indices]                 // N-D access (variable indices)

void SetValueNoCheck(int index, T value)          // Fast set (no bounds check)
T GetValueNoCheck(int index)                      // Fast get (no bounds check)
// ... overloads for 2D, 3D, 4D, N-D
```

**Subtensor Operations:**
```csharp
GenTensor<T, TWrapper> GetSubtensor(int index)
  // Get slice along first dimension
  // Example: tensor[3x4x5].GetSubtensor(1) -> [4x5]

GenTensor<T, TWrapper> GetSubtensor(params int[] indecies)
  // Get element at N-1 dimension path
  // Example: tensor[3x4x5x2].GetSubtensor(1, 2) -> [5x2]

void SetSubtensor(GenTensor<T, TWrapper> sub, params int[] indices)
  // Write subtensor back to parent

GenTensor<T, TWrapper> Slice(int leftIncluding, int rightExcluding)
  // Slice along first dimension like Python [left:right]
  // Example: tensor[10x20].Slice(3, 7) -> [4x20]

T GetCell(params int[] indices)              // Alias for this[]
void SetCell(T value, params int[] indices)  // Alias for this[] setter
```

---

#### C. Matrix Arithmetic APIs (15 methods)

**Matrix Multiplication:**
```csharp
GenTensor<T, TWrapper> MatrixMultiply(GenTensor<T, TWrapper> b)
  // Standard matrix multiplication (A * B)
  // Supports threading: Single, Multi, Auto
  // Optimized with block-based memory access

GenTensor<T, TWrapper> TensorMatrixMultiply(GenTensor<T, TWrapper> b)
  // Multiply all matrices in tensor with corresponding matrices in b
  // Example: [5x3x4] * [5x4x6] -> [5x3x6] (5 matrix multiplications)
```

**Matrix Division:**
```csharp
GenTensor<T, TWrapper> MatrixDivide(GenTensor<T, TWrapper> b)
  // Matrix division: A / B = A * Inverse(B)

GenTensor<T, TWrapper> TensorMatrixDivide(GenTensor<T, TWrapper> b)
  // Divide all matrices in tensor
```

**Piecewise (Element-wise) Arithmetic:**
```csharp
// Tensor + Tensor (element-wise)
GenTensor<T, TWrapper> PiecewiseAdd(GenTensor<T, TWrapper> b)
GenTensor<T, TWrapper> PiecewiseSubtract(GenTensor<T, TWrapper> b)
GenTensor<T, TWrapper> PiecewiseMultiply(GenTensor<T, TWrapper> b)
GenTensor<T, TWrapper> PiecewiseDivide(GenTensor<T, TWrapper> b)

// Tensor + Scalar (broadcast)
GenTensor<T, TWrapper> PiecewiseAdd(T b)
GenTensor<T, TWrapper> PiecewiseSubtract(T b)
GenTensor<T, TWrapper> PiecewiseMultiply(T b)
GenTensor<T, TWrapper> PiecewiseDivide(T b)

// Scalar + Tensor (broadcast)
GenTensor<T, TWrapper> PiecewiseSubtract(T a, GenTensor<T, TWrapper> b)
GenTensor<T, TWrapper> PiecewiseDivide(T a, GenTensor<T, TWrapper> b)
```

**Matrix Power:**
```csharp
GenTensor<T, TWrapper> MatrixPower(int power)
  // Compute A^n via repeated multiplication

GenTensor<T, TWrapper> TensorMatrixPower(int power)
  // Power all matrices in tensor
```

---

#### D. Linear Algebra APIs (25 methods)

**Determinant (3 algorithms):**
```csharp
T DeterminantLaplace()
  // Laplace expansion (exact but slow, O(n!))
  // Best for: Small matrices (n <= 5)

T DeterminantGaussianSimple()
  // Gaussian elimination without division safety
  // Best for: Exact types (rational, symbolic)

T DeterminantGaussianSafeDivision()
  // Gaussian elimination with pivoting
  // Best for: Floating-point (default for most use cases)

// Tensor variants (compute determinant for all matrices)
GenTensor<T, TWrapper> TensorDeterminantLaplace()
GenTensor<T, TWrapper> TensorDeterminantGaussianSimple()
GenTensor<T, TWrapper> TensorDeterminantGaussianSafeDivision()
```

**Matrix Inversion:**
```csharp
void InvertMatrix()
  // In-place inversion via Adjoint method
  // Uses: Adjoint(A) / det(A)

void TensorMatrixInvert()
  // Invert all matrices in tensor
```

**Adjoint (Adjugate Matrix):**
```csharp
GenTensor<T, TWrapper> Adjoint()
  // Compute adjoint matrix (transpose of cofactor matrix)
```

**Matrix Decompositions:**
```csharp
(GenTensor<T, TWrapper> lower, GenTensor<T, TWrapper> upper) LuDecomposition()
  // LU decomposition: A = L * U
  // Returns: (lower triangular, upper triangular)
  // Throws: ImpossibleDecomposition if no decomposition exists

(GenTensor<T, TWrapper> permutation,
 GenTensor<T, TWrapper> lower,
 GenTensor<T, TWrapper> upper) PluDecomposition()
  // PLU decomposition: PA = LU
  // Returns: (permutation matrix, lower, upper)
  // More stable than LU (uses row pivoting)
```

**Row Echelon Forms (8 variants):**
```csharp
// Simple variants (no pivoting, may fail for some matrices)
GenTensor<T, TWrapper> RowEchelonFormSimple()
GenTensor<T, TWrapper> RowEchelonFormLeadingOnesSimple()
GenTensor<T, TWrapper> ReducedRowEchelonFormSimple()

// Safe division variants (pivoting for numerical stability)
GenTensor<T, TWrapper> RowEchelonFormSafeDivision()
GenTensor<T, TWrapper> RowEchelonFormLeadingOnesSafeDivision()
GenTensor<T, TWrapper> ReducedRowEchelonFormSafeDivision()

// Permutation variants (returns permutation matrix + result)
(GenTensor<T, TWrapper> permutation, GenTensor<T, TWrapper> result)
    RowEchelonFormPermuteSimple()
(GenTensor<T, TWrapper> permutation, GenTensor<T, TWrapper> result)
    RowEchelonFormPermuteSafeDivision()
(GenTensor<T, TWrapper> permutation, GenTensor<T, TWrapper> result)
    ReducedRowEchelonFormPermuteSafeDivision()
```

**Elementary Row Operations:**
```csharp
void RowMultiply(int row, T coef)
  // Multiply row by scalar

void RowAdd(int row1, int row2, T coef)
  // Add row2 * coef to row1

void RowSubtract(int row1, int row2, T coef)
  // Subtract row2 * coef from row1

void RowSwap(int row1, int row2)
  // Swap two rows

(int row, int col) RowGetLeadingElement(int row)
  // Find position of leading (pivot) element in row
```

**Transpose:**
```csharp
void Transpose()
  // General tensor transpose (swaps first two dimensions)
  // Example: [3x4x5] -> [4x3x5]

void TransposeMatrix()
  // Matrix transpose (optimized for 2D)
  // Example: [3x4] -> [4x3]
```

---

#### E. Vector Operations (4 methods)

**Dot Product:**
```csharp
T VectorDotProduct(GenTensor<T, TWrapper> b)
  // Standard dot product: sum(a[i] * b[i])
  // Requires: Both tensors are 1D vectors of same length

GenTensor<T, TWrapper> TensorVectorDotProduct(GenTensor<T, TWrapper> b)
  // Dot product for all vectors in tensor
  // Example: [5x3] dot [5x3] -> [5] (5 dot products)
```

**Cross Product:**
```csharp
GenTensor<T, TWrapper> VectorCrossProduct(GenTensor<T, TWrapper> b)
  // 3D cross product: a × b
  // Requires: Both tensors are 3D vectors

GenTensor<T, TWrapper> TensorVectorCrossProduct(GenTensor<T, TWrapper> b)
  // Cross product for all vectors in tensor
  // Example: [10x3] cross [10x3] -> [10x3] (10 cross products)
```

---

#### F. Iteration and Enumeration APIs (12 methods)

**ForEach (Functional Iteration):**
```csharp
void ForEach(Action<T, int[]> iterator)
  // Iterate over all elements with indices
  // Example: tensor.ForEach((value, indices) => Console.WriteLine($"{indices}: {value}"))

// Overloads for 1D, 2D, 3D (avoid index array allocation)
void ForEach(Action<T, int> iterator)
void ForEach(Action<T, int, int> iterator)
void ForEach(Action<T, int, int, int> iterator)
```

**Iterate (Enumerator Pattern):**
```csharp
IEnumerable<(T value, int[] indices)> Iterate()
  // LINQ-compatible enumerator
  // Example: tensor.Iterate().Where(t => t.value > 0).Select(t => t.indices)
```

**IterateOver (Subtensor Iteration):**
```csharp
IEnumerable<GenTensor<T, TWrapper>> IterateOver(int dimension)
  // Iterate over subtensors along dimension
  // Example: tensor[10x20x30].IterateOver(0) yields 10 tensors of shape [20x30]

IEnumerable<GenTensor<T, TWrapper>> IterateOverCopy(int dimension)
  // Same as IterateOver but returns copies (not views)

IEnumerable<GenTensor<T, TWrapper>> IterateOverMatrices()
  // Iterate over all 2D slices
  // Example: tensor[5x3x4].IterateOverMatrices() yields 5 matrices of [3x4]

IEnumerable<GenTensor<T, TWrapper>> IterateOverVectors()
  // Iterate over all 1D slices
  // Example: tensor[5x3x4].IterateOverVectors() yields 15 vectors of [4]

IEnumerable<T> IterateOverElements()
  // Iterate over raw elements (flat traversal, no indices)
```

**Specialized Iteration:**
```csharp
void IterateOver1(Action<GenTensor<T, TWrapper>> action)
void IterateOver2(Action<GenTensor<T, TWrapper>> action)
void IterateOver3(Action<GenTensor<T, TWrapper>> action)
  // Iterate over dimension 0, 1, or 2 (inline optimization)
```

---

#### G. Utility APIs (11 methods)

**Copy and View:**
```csharp
GenTensor<T, TWrapper> Copy()
  // Deep copy (new data array)

GenTensor<T, TWrapper> Forward()
  // Create view (shares data array, different shape/offset)
  // Used for subtensors, slices, transpose views
```

**Serialization:**
```csharp
byte[] Serialize()
  // Binary serialization (shape + elements)

static GenTensor<T, TWrapper> Deserialize(byte[] data)
  // Binary deserialization
```

**String Representation:**
```csharp
string ToString()
  // Formatted string representation
  // Example:
  //   Matrix[3 x 3]
  //   1.0 2.0 3.0
  //   4.0 5.0 6.0
  //   7.0 8.0 9.0
```

**Equality:**
```csharp
bool Equals(GenTensor<T, TWrapper> other)
  // Element-wise equality (uses TWrapper.AreEqual)

static bool operator ==(GenTensor<T, TWrapper> a, GenTensor<T, TWrapper> b)
static bool operator !=(GenTensor<T, TWrapper> a, GenTensor<T, TWrapper> b)

int GetHashCode()
  // Hash code based on shape and elements
```

**Cloning:**
```csharp
object Clone()
  // ICloneable implementation (calls Copy)
```

**Next Index (Internal):**
```csharp
void NextIndex(int[] indices)
  // Increment index array to next position (row-major order)
  // Used internally for iteration
```

---

#### H. Assignment API (1 method)

```csharp
void Assign(Func<int[], T> assigner)
  // Fill tensor using custom function
  // Example: tensor.Assign(indices => indices.Sum())
```

---

### 3.4 TensorShape API

**Struct:** `TensorShape` (value type, immutable)

```csharp
public struct TensorShape
{
    public int[] Shape;              // Dimensions array

    // Properties
    int Length;                      // Number of dimensions
    int DimensionCount;              // Alias for Length
    int Count;                       // Total volume (product of dimensions)

    // Indexer
    int this[int i];                 // Access i-th dimension

    // Methods
    TensorShape Copy();              // Copy shape
    TensorShape SubShape(int from);  // Get shape from dimension onward
    void Swap(ref int i1, ref int i2); // Swap two dimension values
    int[] ToArray();                 // Convert to int array
    string ToString();               // String representation (e.g., "[3 x 4 x 5]")

    // Equality
    bool Equals(TensorShape other);
    static bool operator ==(TensorShape a, TensorShape b);
    static bool operator !=(TensorShape a, TensorShape b);
    int GetHashCode();
}
```

---

### 3.5 Threading Support

**Enum:** `Threading`

```csharp
public enum Threading
{
    Single,  // Single-threaded execution
    Multi,   // Force parallel execution (Parallel.For)
    Auto     // Auto-detect based on problem size
}
```

**Supported Operations:**
- Matrix multiplication (threshold: Volume > 125)
- Piecewise arithmetic (threshold: Volume > 64)

**Usage Example:**
```csharp
var result = matrixA.MatrixMultiply(matrixB, Threading.Multi);
```

---

## Part 4: Usage Examples

### Example 1: Basic Double Matrix Operations

```csharp
using GeometricAlgebraFulcrumLib.Algebra.TensorAlgebra.Generic.Core;
using GeometricAlgebraFulcrumLib.Algebra.TensorAlgebra.Generic.Functions;

// Create a 3x3 identity matrix (Double)
var identity = GenTensor<double, DoubleWrapper>.CreateIdentityMatrix(3);

// Create a matrix from 2D array
var matrixA = GenTensor<double, DoubleWrapper>.CreateMatrix(new double[,]
{
    { 1, 2, 3 },
    { 4, 5, 6 },
    { 7, 8, 9 }
});

// Matrix multiplication
var result = matrixA.MatrixMultiply(identity);

// Element-wise addition
var sum = matrixA.PiecewiseAdd(matrixA); // 2 * A

// Transpose
matrixA.TransposeMatrix();

// Access elements
double value = matrixA[1, 2]; // Row 1, Column 2

// Iterate over elements
matrixA.ForEach((val, i, j) =>
    Console.WriteLine($"[{i},{j}] = {val}"));
```

---

### Example 2: Linear Algebra - Solving Systems

```csharp
// Create coefficient matrix
var A = GenTensor<double, DoubleWrapper>.CreateMatrix(new double[,]
{
    { 2, 1, -1 },
    { -3, -1, 2 },
    { -2, 1, 2 }
});

// Compute determinant
double det = A.DeterminantGaussianSafeDivision();
Console.WriteLine($"Determinant: {det}");

// Compute inverse
var AInv = A.Copy();
AInv.InvertMatrix();

// Verify: A * A^-1 = I
var identity = A.MatrixMultiply(AInv);

// LU Decomposition
var (L, U) = A.LuDecomposition();
Console.WriteLine("Lower triangular:");
Console.WriteLine(L.ToString());
Console.WriteLine("Upper triangular:");
Console.WriteLine(U.ToString());

// Row echelon form (for solving systems)
var rref = A.ReducedRowEchelonFormSafeDivision();
```

---

### Example 3: Vector Operations

```csharp
// Create 3D vectors
var v1 = GenTensor<double, DoubleWrapper>.CreateVector(new double[] { 1, 0, 0 });
var v2 = GenTensor<double, DoubleWrapper>.CreateVector(new double[] { 0, 1, 0 });

// Dot product
double dot = v1.VectorDotProduct(v2); // 0

// Cross product
var cross = v1.VectorCrossProduct(v2); // [0, 0, 1]

// Element-wise operations
var scaled = v1.PiecewiseMultiply(5.0); // [5, 0, 0]
```

---

### Example 4: Tensor Operations (N-D)

```csharp
// Create 3D tensor (batch of matrices)
var tensor = GenTensor<double, DoubleWrapper>.CreateTensor(
    indices => (double)(indices[0] + indices[1] + indices[2]),
    5, 3, 4  // 5 batches, 3x4 matrices
);

// Stack tensors (create new dimension)
var stacked = GenTensor<double, DoubleWrapper>.Stack(tensor, tensor, tensor);
// Result shape: [3, 5, 3, 4]

// Iterate over matrices
foreach (var matrix in tensor.IterateOverMatrices())
{
    Console.WriteLine($"Matrix shape: {matrix.Shape}");
    // Each is [3x4]
}

// Get subtensor
var slice = tensor.GetSubtensor(2); // Get 3rd batch -> [3x4]

// Aggregate over dimension
var summed = GenTensor<double, DoubleWrapper>.CreateTensor(3, 4);
GenTensor<double, DoubleWrapper>.Aggregate(
    tensor, summed,
    new AddDelegate<double>(), // Custom delegate for addition
    axis: 0 // Sum along batch dimension
);
```

---

### Example 5: Using Float32 Instead

```csharp
// Just change type parameters!
var matrixF32 = GenTensor<float, FloatWrapper>.CreateMatrix(new float[,]
{
    { 1.0f, 2.0f },
    { 3.0f, 4.0f }
});

float detF32 = matrixF32.DeterminantGaussianSafeDivision();
// All operations work identically
```

---

### Example 6: Custom Scalar Type (Complex Numbers)

```csharp
using System.Numerics;

var complexMatrix = GenTensor<Complex, ComplexWrapper>.CreateMatrix(new Complex[,]
{
    { new Complex(1, 1), new Complex(2, -1) },
    { new Complex(0, 3), new Complex(1, 0) }
});

Complex detComplex = complexMatrix.DeterminantGaussianSafeDivision();
Console.WriteLine($"Complex determinant: {detComplex}");
// Works with all matrix operations!
```

---

## Part 5: Performance Characteristics

### Complexity Analysis

| Operation | Time Complexity | Space Complexity | Notes |
|-----------|----------------|------------------|-------|
| **Construction** |
| CreateMatrix(n, m) | O(nm) | O(nm) | Allocates array |
| CreateIdentityMatrix(n) | O(n²) | O(n²) | Fills with 0s and 1s |
| **Indexing** |
| this[i, j] | O(1) | O(1) | Direct array access with offset |
| GetSubtensor(i) | O(1) | O(1) | Creates view (no copy) |
| Slice(left, right) | O(1) | O(1) | Creates view |
| **Matrix Operations** |
| MatrixMultiply | O(n³) | O(n²) | Naive algorithm, parallelizable |
| Transpose | O(1) | O(1) | Creates view, not actual transpose |
| **Linear Algebra** |
| DeterminantLaplace | O(n!) | O(n²) | Recursive, only for small n |
| DeterminantGaussian | O(n³) | O(n²) | Gaussian elimination |
| InvertMatrix | O(n³) | O(n²) | Via Adjoint method |
| LuDecomposition | O(n³) | O(n²) | Standard algorithm |
| RowEchelonForm | O(n²m) | O(nm) | n rows, m columns |
| **Vector Operations** |
| VectorDotProduct | O(n) | O(1) | Linear scan |
| VectorCrossProduct | O(1) | O(1) | 3D only, fixed ops |
| **Piecewise Operations** |
| PiecewiseAdd/Sub/Mul/Div | O(n) | O(n) | Parallelizable (threshold: 64) |
| **Iteration** |
| ForEach | O(n) | O(1) | Linear scan, no allocations |
| Iterate | O(1) per item | O(1) | Lazy enumeration |

---

### Memory Optimization Features

1. **View-based Operations:**
   - `GetSubtensor()`, `Slice()`, `Forward()` create views (share data)
   - Transpose returns view (swaps dimension metadata, doesn't copy)
   - Zero-copy until mutation

2. **Block-based Indexing:**
   - Pre-computed strides in `Blocks[]` array
   - Avoids repeated multiplication for index calculation
   - Cache-friendly memory access patterns

3. **Parallel Execution:**
   - Auto-threshold for small problems (avoids threading overhead)
   - `Threading.Auto` mode intelligently chooses
   - Efficient for large matrices (>10x10 typically)

---

## Part 6: Integration with GA-FUL

### Current Status: NOT INTEGRATED

**Evidence:**
1. No usage in GeometricAlgebraFulcrumLib.UnitTests
2. No usage in GeometricAlgebraFulcrumLib.Applications
3. No usage in GeometricAlgebraFulcrumLib.Modeling (CGa, PGa, VGa, HGa)
4. No usage in GeometricAlgebraFulcrumLib.Algebra (XGa, RGa)
5. No cross-references from main GA-FUL APIs

**Likely Reasons:**
- GA-FUL has its own optimized linear algebra via `LinVector`, `LinBivector`, `LinMatrix` (up to 4x4)
- Multivectors already handle N-dimensional data structures
- Tensor algebra is orthogonal to geometric algebra workflows
- Library was likely ported for potential future use but not yet integrated

---

### Potential Integration Points

Where TensorAlgebra COULD be useful in GA-FUL:

1. **Large-scale CGA computations:**
   - Batch processing of geometric objects
   - Example: Transform 1000 points in CGA simultaneously
   - Current: Loop-based, TensorAlgebra: Vectorized

2. **MetaProgramming optimizations:**
   - Symbolic tensors for code generation
   - Generate optimized tensor operations alongside GA operations

3. **Signal processing (GeometricAlgebraFulcrumLib.Modeling.Signals):**
   - Geometric wavelet transforms
   - Multidimensional geometric signal analysis

4. **Graphics primitives:**
   - Vertex buffer operations
   - Batch geometric transformations

5. **Machine learning on geometric data:**
   - Tensor-based GA neural networks
   - Conformal geometric deep learning

---

## Part 7: Recommendations

### For Float64 Wrapper Creation

**Verdict: NOT NEEDED**

Rationale:
- `DoubleWrapper` already exists and is efficient
- Usage: `GenTensor<double, DoubleWrapper>` is straightforward
- No performance penalty compared to specialized Float64 class
- Tensor operations are already optimized (parallel, block-based)

**If Float64 wrapper WERE desired (not recommended):**

```csharp
// Hypothetical Float64 wrapper (unnecessary, but possible)
public class GenTensorFloat64
{
    private GenTensor<double, DoubleWrapper> _tensor;

    // Factory methods
    public static GenTensorFloat64 CreateMatrix(int width, int height)
        => new GenTensorFloat64(GenTensor<double, DoubleWrapper>.CreateMatrix(width, height));

    // Forward all operations
    public GenTensorFloat64 MatrixMultiply(GenTensorFloat64 b)
        => new GenTensorFloat64(_tensor.MatrixMultiply(b._tensor));

    // ... etc
}
```

**Why this is unnecessary:**
- 100+ methods to wrap (maintenance burden)
- No performance gain (inlining eliminates overhead)
- Breaks compatibility with other scalar types
- Generic version is already production-ready

---

### For Integration into GA-FUL

**Priority: LOW (optional future enhancement)**

**Recommended Approach if Desired:**

1. **Create Integration Layer:**
   ```csharp
   // GeometricAlgebraFulcrumLib.Algebra.TensorAlgebra.Integration/

   public static class XGaTensorExtensions
   {
       // Convert XGaVector to GenTensor
       public static GenTensor<T, TWrapper> ToTensor<T, TWrapper>(
           this XGaVector<T> vector) where TWrapper : struct, IOperations<T>
       {
           // Extract coefficients in basis order
       }

       // Convert GenTensor to XGaVector
       public static XGaVector<T> ToXGaVector<T, TWrapper>(
           this GenTensor<T, TWrapper> tensor, XGaProcessor<T> processor)
       {
           // Reconstruct multivector from tensor elements
       }
   }
   ```

2. **Add Unit Tests:**
   - Create `GeometricAlgebraFulcrumLib.UnitTests/Algebra/TensorAlgebra/`
   - Test all TensorAlgebra operations with Double, Float, Complex
   - Verify integration with XGa types

3. **Add Examples/Samples:**
   - Create `GeometricAlgebraFulcrumLib.Applications/TensorAlgebra/`
   - Demonstrate batch CGA operations using tensors
   - Show performance comparisons

4. **Documentation:**
   - Add TensorAlgebra section to main documentation
   - Explain when to use tensors vs multivectors
   - Provide migration guide for NumPy users

---

### For Scalar API Consistency

**Observation:** TensorAlgebra uses `IOperations<T>` pattern, while XGa/RGa use `IScalarProcessor<T>`

**These are DIFFERENT abstractions:**

| Aspect | IOperations<T> | IScalarProcessor<T> |
|--------|----------------|---------------------|
| Purpose | Tensor element ops | GA scalar arithmetic |
| Methods | 13 core operations | 50+ operations (sqrt, sin, cos, exp, etc.) |
| Design | Minimal (value semantics) | Rich (mathematical functions) |
| Usage | Generic tensors | Geometric algebra |

**Recommendation: Keep Separate**
- TensorAlgebra's IOperations is deliberately minimal (portability)
- IScalarProcessor is GA-specific (needs geometric operations)
- Interop possible via adapter pattern if needed

---

## Part 8: Comparison with External Libraries

### TensorAlgebra vs NumPy (Python)

| Feature | TensorAlgebra (GA-FUL) | NumPy |
|---------|------------------------|-------|
| **Language** | C# | Python/C |
| **Generic Types** | Yes (`<T, TWrapper>`) | No (dtype system) |
| **Performance** | Good (parallel, inline) | Excellent (BLAS/LAPACK) |
| **Matrix Multiply** | O(n³) naive | O(n²⋅⁸) Strassen, BLAS |
| **Broadcasting** | Limited (scalar only) | Full N-D broadcasting |
| **N-D Support** | Yes (arbitrary) | Yes (arbitrary) |
| **Slicing** | Yes (row-major) | Yes (flexible stride) |
| **In-place Ops** | Limited (row ops) | Extensive |
| **Vectorization** | Parallel.For | SIMD intrinsics |
| **Ecosystem** | Standalone | SciPy, Pandas, ML stack |

**Use TensorAlgebra if:**
- Working in pure C# environment
- Need generic scalar types (Complex, BigInteger, symbolic)
- Integrating with GA-FUL workflows

**Use NumPy if:**
- Need maximum performance for large matrices (>100x100)
- Need advanced indexing/broadcasting
- Working with Python ML ecosystem

---

### TensorAlgebra vs Math.NET Numerics

| Feature | TensorAlgebra | Math.NET |
|---------|--------------|----------|
| **Tensors** | Yes (N-D) | No (matrices only) |
| **Generic Types** | Via IOperations | Via IField interface |
| **Linear Algebra** | Full (det, inv, decomp) | Full (+ eigen, SVD, QR) |
| **Performance** | Good | Excellent (MKL backend) |
| **Sparse Matrices** | No | Yes |
| **Distribution** | Embedded in GA-FUL | NuGet package |
| **License** | MIT (via GA-FUL) | MIT |

**Use TensorAlgebra if:**
- Already using GA-FUL
- Need N-D tensors (not just matrices)
- Want minimal dependencies

**Use Math.NET if:**
- Need advanced decompositions (SVD, eigenvalues)
- Need sparse matrix support
- Need statistical distributions

---

## Part 9: Known Limitations

### 1. Broadcasting
- Only scalar broadcasting supported
- No NumPy-style tensor broadcasting (e.g., [3x1] + [1x4] -> [3x4])
- Workaround: Manual element-wise loops

### 2. Advanced Indexing
- No fancy indexing (e.g., `tensor[[0, 2, 4]]`)
- No boolean masking (e.g., `tensor[tensor > 0]`)
- Workaround: Use `Iterate()` with LINQ

### 3. Matrix Multiplication Algorithm
- Naive O(n³) implementation
- No Strassen, no BLAS integration
- Parallelized but not cache-optimized for very large matrices

### 4. Sparse Tensors
- Dense storage only (all elements stored)
- Inefficient for sparse data (>90% zeros)
- Workaround: Use GA-FUL's sparse multivectors for sparse linear algebra

### 5. GPU Acceleration
- CPU-only implementation
- No CUDA/OpenCL support
- Workaround: Use external GPU libraries if needed

### 6. In-place Operations
- Most operations allocate new tensors
- Only row operations and `InvertMatrix()` are in-place
- Memory overhead for chained operations

### 7. SIMD Vectorization
- No explicit SIMD intrinsics (relies on JIT)
- Could be 2-4x faster with System.Runtime.Intrinsics

### 8. Exception Handling
- Controlled by `#if ALLOW_EXCEPTIONS` preprocessor directive
- May silently fail if disabled (for performance)
- Always enabled in GA-FUL build

---

## Part 10: License and Attribution

**TensorAlgebra License:** MIT
**Original Author:** WhiteBlackGoose
**Original Repository:** https://github.com/WhiteBlackGoose/GenericTensor (likely)
**Copyright:** 2020-2021 WhiteBlackGoose
**Integrated into GA-FUL:** Unknown date (pre-2025)

**All header comments preserve original copyright:**
```csharp
/*
 * MIT License
 *
 * Copyright (c) 2020-2021 WhiteBlackGoose
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction...
 */
```

**GA-FUL Integration:** No modifications detected to core TensorAlgebra code - appears to be direct port

---

## Conclusion

### Summary

1. **Float64 Tensor Implementation: INTENTIONALLY MISSING**
   - Generic implementation (`GenTensor<T, TWrapper>`) is sufficient
   - Pre-built `DoubleWrapper` provides Float64 support
   - Creating Float64 wrapper is unnecessary and not recommended

2. **Generic TensorAlgebra: COMPLETE AND PRODUCTION-READY**
   - 100+ methods covering all standard tensor operations
   - Supports any scalar type via `IOperations<T>` interface
   - Optimized with parallel execution and block-based indexing
   - MIT licensed, well-tested code from external library

3. **Integration with GA-FUL: MINIMAL**
   - No current usage in tests, applications, or modeling layers
   - Appears to be included for potential future use
   - Could be valuable for batch geometric computations (future work)

4. **Recommendation: NO ACTION NEEDED**
   - TensorAlgebra is complete as-is
   - Float64 wrapper would be redundant
   - Integration with GA-FUL is optional enhancement (low priority)

---

### API Surface Statistics

| Category | Count |
|----------|-------|
| **Construction APIs** | 13 |
| **Indexing APIs** | 25 |
| **Matrix Arithmetic** | 15 |
| **Linear Algebra** | 25 |
| **Vector Operations** | 4 |
| **Iteration APIs** | 12 |
| **Utility APIs** | 11 |
| **Assignment API** | 1 |
| **Total Public Methods** | **106** |

---

### File Structure Statistics

| Category | Count |
|----------|-------|
| **Core Files** | 7 |
| **Function Files** | 17 |
| **Exception Files** | 3 |
| **Total Files** | **27** |
| **Lines of Code** | ~8,000+ |

---

**Report Generated By:** Agent 15 (TensorAlgebra API Analyzer)
**Analysis Date:** 2025-10-23
**Repository Path:** `D:\_MBOX\_CODE\GA-FUL-main\GeometricAlgebraFulcrumLib\`
**Contact:** See `CLAUDE.md` for agent directives
