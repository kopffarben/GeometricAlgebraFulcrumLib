---
layout: default
title: "Design Principles"
lang: en
---

# GA-FuL Design Principles

**[🇩🇪 Deutsche Version](design-principles.de.md)**

## Table of Contents

1. [Overview](#overview)
2. [Core Design Intentions (CDIs)](#core-design-intentions-cdis)
3. [Data-Oriented Programming (DOP)](#data-oriented-programming-dop)
4. [Implementation of DOP Principles](#implementation-of-dop-principles)
5. [Practical Examples](#practical-examples)

---

## Overview

The design of GA-FuL is based on two main concepts:

1. **Core Design Intentions (CDIs)**: Five main goals that the system should fulfill
2. **Data-Oriented Programming (DOP)**: Four principles for practical implementation

These principles emerged from experience with the predecessor system [GMac](https://github.com/ga-explorer/GMac) and address its weaknesses.

---

## Core Design Intentions (CDIs)

### CDI-1: Abstraction of Multivector Operations

**Goal:** Separation of multivector operations and concrete scalar representations

**Problem:**
- Scalars can be represented in many ways:
  - IEEE 754 floating point numbers (32-bit, 64-bit)
  - Arbitrary precision decimal numbers
  - Rational numbers (fractions)
  - Symbolic expressions (for CAS)
  - Multidimensional arrays (for ML/AI)
  - Sampled signals (for signal processing)

**Solution:**
- Generic implementation via `IScalarProcessor<T>`
- All GA operations independent of scalar type
- Easy integration of new scalar types

**Example:**
```csharp
// The same code works with all scalar types
var processor = XGaProcessor<T>.CreateEuclidean(scalarProcessor);
var v1 = processor.CreateVectorComposer()
    .SetVectorTerm(0, a)
    .SetVectorTerm(1, b)
    .SetVectorTerm(2, c)
    .GetVector();
var v2 = processor.CreateVectorComposer()
    .SetVectorTerm(0, x)
    .SetVectorTerm(1, y)
    .SetVectorTerm(2, z)
    .GetVector();
var result = v1.Gp(v2); // Geometric Product

// T can be: double, float, decimal, BigDecimal, Expr, etc.
```

**Benefits:**
- ✓ One code for all scalar types
- ✓ Easy addition of new scalar types
- ✓ Maximum reusability

---

### CDI-2: Reduction of Memory Requirements

**Goal:** Efficient storage of sparse multivectors in high-dimensional GAs

**Problem:**
- A full multivector in $n$-dimensional GA requires $2^n$ scalars
- A full $k$-vector requires $\binom{n}{k}$ scalars
- Examples (with 32-bit floats):
  - 30D GA, full multivector: 8 GB
  - 30D GA, full 15-vector: 1.16 GB
  - This is impractical for most applications

**Reality:**
- Practical GA applications mostly use **sparse multivectors**
- Example: 5D-CGA can represent 3D geometry with only 5 out of 32 scalars

**Solution:**
- Dictionary-based storage: `IReadOnlyDictionary<IIndexSet, T>`
- Only non-null components are stored
- Automatic selection of the most efficient data structure

**Memory Hierarchy:**

| Multivector Type | Data Structure | Memory Efficiency |
|-----------------|---------------|-------------------|
| Zero-Multivector | `EmptyDictionary` | 0 scalars |
| Single-Component | `SingleItemDictionary` | 1 scalar + index |
| Sparse | `Dictionary<IIndexSet, T>` | n scalars + indices |
| Dense (small) | Lookup tables | Optimized for small n |

**Index-Set Hierarchy:**

| Dimension Range | IIndexSet Implementation | Memory |
|-------------------|---------------------------|----------|
| Empty set | `EmptyIndexSet` | 0 bytes |
| 1 element | `SingleIndexSet` | 4 bytes |
| < 64 dimensions | `UInt64IndexSet` | 8 bytes |
| Arbitrary (dense) | `DenseIndexSet` | Array of ints |
| Arbitrary (sparse) | `SparseIndexSet` | HashSet |

**Example:**
```csharp
// 100D-GA, sparse multivector with 10 components
var composer = processor.CreateMultivectorComposer();
composer.SetTerm(indexSet1, scalar1);
composer.SetTerm(indexSet2, scalar2);
// ... 10 terms total

var mv = composer.GetMultivector();

// Memory: ~10 scalars + 10 index-sets
// Full multivector would require 2^100 scalars!
```

**Benefits:**
- ✓ High-dimensional GAs become practical
- ✓ Automatic optimization
- ✓ Flexible dimensionality

---

### CDI-3: Metaprogramming Capabilities

**Goal:** Code generation from GA expressions for maximum performance and cross-platform deployment

**Context:**
- Generic<T> implementation already provides **1.24-2.31x faster** performance than Float64 Specialized
- However, for **extreme performance** or **specific target platforms** (embedded systems, GPUs, web), metaprogramming enables further optimization

**Use Cases:**
- Target platforms without .NET runtime (C/C++, JavaScript, CUDA)
- Domain-specific optimizations beyond generic implementation
- Compile-time constant folding for known geometry
- Custom optimization for specific hardware

**Solution:**
- Automatic code generation from GA expressions
- Optimization through:
  - Constant propagation
  - Common subexpression elimination
  - Symbolic simplification
  - Genetic programming
- Support for multiple target languages

**Workflow:**
```
High-Level GA Operations
        ↓
Expression Tree
        ↓
Optimization
        ↓
Target Language Code (C++, C#, CUDA, etc.)
```

**Example:**
```csharp
// 1. Define computations in GA-FuL
var context = new MetaContext();
var x = context.CreateParameter("x");
var y = context.CreateParameter("y");
var scalarProcessor = context.ScalarProcessor;
var processor = XGaProcessor<IMetaExpression>.CreateEuclidean(scalarProcessor);

var v1 = processor.CreateVectorComposer()
    .SetVectorTerm(0, x)
    .SetVectorTerm(1, y)
    .SetVectorTerm(2, 0)
    .GetVector();
var v2 = processor.CreateVectorComposer()
    .SetVectorTerm(0, 1)
    .SetVectorTerm(1, 1)
    .SetVectorTerm(2, 1)
    .GetVector();
var result = v1.Gp(v2);

// 2. Optimize
context.Optimize();

// 3. Generate C++ code
var cppCode = new CppCodeComposer().Generate(context);

// Result: Highly optimized C++ code
```

**Use Cases:**
- CUDA code for GPU acceleration
- Embedded systems (C/C++)
- Web applications (JavaScript)
- Scientific computing (MATLAB/Python)

**Benefits:**
- ✓ Best performance for production code
- ✓ Error-free code generation
- ✓ Automatic optimization
- ✓ Multiple target languages

---

### CDI-4: Layered System Design

**Goal:** Organization of complexity through layers for different user groups

**Problem:**
- CDI-1 to CDI-3 lead to high complexity
- Different users have different needs
- Some need abstract high-level API
- Others need low-level control for performance

**Solution:**
Four layers with increasing abstraction:

```
Abstraction Level
       ↑
       │    ┌────────────────────────────┐
 High  │    │  Metaprogramming Layer     │
       │    ├────────────────────────────┤
       │    │     Modeling Layer         │
       │    ├────────────────────────────┤
       │    │       Algebra Layer        │
       │    ├────────────────────────────┤
  Low  │    │   System Utilities Layer   │
       │    └────────────────────────────┘
```

**User Profiles:**

| User Type | Preferred Layer | Use Case |
|-------------|-------------------|----------------|
| **Researcher** | Modeling | Prototyping geometric algorithms |
| **Software Developer** | Metaprogramming | Code generation for production |
| **Library Developer** | Algebra | Extending functionality |
| **Performance Engineer** | Algebra + Utilities | Optimizing critical paths |

**Examples:**

*High-Level (Modeling):*
```csharp
// Coordinate-free, intuitive API
var point = cga.Encode.IpnsRound.Point(x, y, z);
var sphere = cga.Encode.IpnsRound.RealSphere(radius, cx, cy, cz);
var intersection = point.Op(sphere);
```

*Low-Level (Algebra):*
```csharp
// Direct control over scalars and basis blades
var blade = processor.CreateBasisBlade(indexSet);
var scalar = scalarProcessor.Times(a, b);
var term = processor.CreateTerm(blade, scalar);
```

**Benefits:**
- ✓ Every user finds suitable abstraction level
- ✓ Clear separation of responsibilities
- ✓ Easy maintenance and extension

---

### CDI-5: Unified, Generic, Extensible API

**Goal:** Unified API for different application classes

**Requirements:**
- **Unified:** Consistent naming conventions and patterns
- **Generic:** Works with all scalar types and GA metrics
- **Extensible:** Easy addition of new features

**Implementation:**

**1. Consistent Naming Conventions:**
```csharp
// All processors follow the same pattern
IScalarProcessor<T>
XGaProcessor<T>
XGaConformalSpace<T>

// All composers follow the same pattern
XGaMultivectorComposer<T>
XGaKVectorComposer<T>
```

**2. Generic Types:**
```csharp
// One template for all types
XGaMultivector<double>
XGaMultivector<float>
XGaMultivector<Expr>  // Symbolic
XGaMultivector<BigDecimal>
```

**3. Extension Methods:**
```csharp
// Unified API through extension methods
multivector.Gp(other)      // Geometric Product
multivector.Op(other)      // Outer Product
multivector.Lcp(other)     // Left Contraction Product
multivector.Rcp(other)     // Right Contraction Product
multivector.Sp(other)      // Scalar Product
multivector.Reverse()      // Reverse
multivector.GradeInvolution() // Grade Involution
```

**4. Builder Pattern:**
```csharp
// Fluent API for construction
var mv = processor.CreateMultivectorComposer()
    .SetTerm(index1, scalar1)
    .SetTerm(index2, scalar2)
    .AddTerm(index3, scalar3)
    .GetMultivector();
```

**Supported Application Classes:**
- Numerical computations
- Symbolic mathematics
- Signal processing
- Computer vision
- Robotics
- Visualization
- Code generation

**Benefits:**
- ✓ Easy to learn
- ✓ Consistent across all modules
- ✓ IntelliSense-friendly
- ✓ Easy to extend

---

## Data-Oriented Programming (DOP)

Traditional OOP led to excessive complexity in GA-FuL. DOP offers a better alternative.

### Why DOP instead of classic OOP?

**Problems with classic OOP:**
- ❌ Deep coupling of data and behavior
- ❌ Complex inheritance hierarchies
- ❌ Difficult maintenance in large systems
- ❌ Encapsulation can lead to inflexibility

**Benefits of DOP:**
- ✓ More readable code
- ✓ More maintainable code
- ✓ More extensible code
- ✓ Better testability

---

### DOP Principle 1: Separation of Behavior and Data

**Rule:** Keep behavior code and data separate

**In OOP:**
```csharp
// Classic OOP: Data and behavior coupled
class Multivector {
    private Dictionary<IIndexSet, T> data;

    public Multivector Gp(Multivector other) {
        // Behavior in the same object
    }
}
```

**In GA-FuL (DOP):**
```csharp
// DOP: Data in thin wrapper
class XGaMultivector<T> {
    public IReadOnlyDictionary<int, XGaKVector<T>> Data { get; }
}

// Behavior in static extension methods
static class XGaMultivectorExtensions {
    public static XGaMultivector<T> Gp<T>(
        this XGaMultivector<T> mv1,
        XGaMultivector<T> mv2
    ) {
        // Behavior separate from data
    }
}
```

**Benefits:**
- ✓ Data can be used by multiple behavior modules
- ✓ Behavior can be tested independently
- ✓ Easier code reviews

---

### DOP Principle 2: Generic Data Structures

**Rule:** Use general data structures instead of special classes

**Preferred Structures:**
- Arrays/Lists for sequential data
- Dictionaries/Maps for key-value pairs
- Sets for unique collections
- Queues for FIFO
- Trees for hierarchical data

**In GA-FuL:**

**Multivectors:**
```csharp
// NOT a special class with internal array structure
// BUT generic dictionary
IReadOnlyDictionary<int, XGaKVector<T>>
```

**k-Vectors:**
```csharp
IReadOnlyDictionary<IIndexSet, T>
```

**Example:**
```csharp
// Access is simple and transparent
var grade2Part = multivector[2];  // Dictionary access
var scalar = kVector[indexSet];   // Dictionary access

// No hidden internal structures
```

**Benefits:**
- ✓ Transparent data structures
- ✓ Standard operations available
- ✓ Easy to debug
- ✓ Familiar to all developers

---

### DOP Principle 3: Immutable Data

**Rule:** Never modify data directly, but create new versions

**Why Immutability?**
- Thread safety
- Predictable behavior
- Easier debugging
- Functional programming style

**Implementation in GA-FuL:**

**Problem:** How to create new multivectors without mutation?

**Solution:** Composer classes

```csharp
// 1. Composer for construction (Mutable during build)
var composer = processor.CreateMultivectorComposer();
composer.SetTerm(index1, scalar1);
composer.SetTerm(index2, scalar2);
composer.AddTerm(index3, scalar3);

// 2. Final multivector (Immutable)
var multivector = composer.GetMultivector();

// 3. Multivector itself is read-only
// multivector.Data is IReadOnlyDictionary<...>
```

**Pattern:**
```
Mutable Composer → Build → Immutable Data Structure
```

**Classes:**
- `XGaMultivectorComposer<T>`: Creates multivectors
- `XGaKVectorComposer<T>`: Creates k-vectors
- `XGaScalarComposer<T>`: Creates scalars

**Benefits:**
- ✓ No unexpected side effects
- ✓ Thread-safe
- ✓ Easier to understand
- ✓ Optimization opportunities by compiler

---

### DOP Principle 4: Separation of Data Representation and Schema

**Rule:** Store data schema separately from actual data

**Implementation:**

**Through Interfaces and Abstract Classes:**

```csharp
// Schema: Interface defines the shape
interface IIndexSet {
    int Count { get; }
    bool Contains(int index);
    IEnumerator<int> GetEnumerator();
}

// Representation: Different implementations
class EmptyIndexSet : IIndexSet { }
class SingleIndexSet : IIndexSet { }
class UInt64IndexSet : IIndexSet { }
class DenseIndexSet : IIndexSet { }
class SparseIndexSet : IIndexSet { }
```

**Usage:**
```csharp
// Code uses only the schema (interface)
void ProcessBlade(IIndexSet indexSet) {
    // Works with all implementations
    foreach (var index in indexSet) {
        // ...
    }
}

// Automatic selection of the best implementation
var indexSet = IndexSetFactory.Create(indices);
// Returns EmptyIndexSet, SingleIndexSet, UInt64IndexSet, etc.
```

**Additional Examples:**

**Multivectors:**
```csharp
// Schema
IReadOnlyDictionary<IIndexSet, T>

// Implementations
class EmptyDictionary<K, V> : IReadOnlyDictionary<K, V>
class SingleItemDictionary<K, V> : IReadOnlyDictionary<K, V>
class Dictionary<K, V> : IReadOnlyDictionary<K, V>
```

**Benefits:**
- ✓ Flexibility in implementation
- ✓ Runtime optimization
- ✓ Easy swapping of implementations
- ✓ Interface-based testing

---

## Implementation of DOP Principles

### Example: Index-Sets

**Complete implementation of the four DOP principles:**

**DOP-4 (Schema):**
```csharp
// Interface as schema
interface IIndexSet {
    int Count { get; }
    bool Contains(int index);
    IEnumerable<int> GetIndices();
}
```

**DOP-2 (Generic Structures) + DOP-3 (Immutable):**
```csharp
// Implementation with read-only internal structures
class UInt64IndexSet : IIndexSet {
    private readonly ulong _bitmap;  // Immutable

    public UInt64IndexSet(ulong bitmap) {
        _bitmap = bitmap;  // Set once, never changed
    }
}

class SparseIndexSet : IIndexSet {
    private readonly ImmutableHashSet<int> _indices;  // Immutable

    public SparseIndexSet(IEnumerable<int> indices) {
        _indices = indices.ToImmutableHashSet();
    }
}
```

**DOP-1 (Separation of Behavior):**
```csharp
// Thin wrapper
class XGaBasisBlade {
    public IIndexSet IndexSet { get; }  // Just holds data

    public XGaBasisBlade(IIndexSet indexSet) {
        IndexSet = indexSet;
    }
}

// Behavior in extension methods
static class XGaBasisBladeExtensions {
    public static XGaBasisBlade Op(
        this XGaBasisBlade blade1,
        XGaBasisBlade blade2,
        XGaMetric metric
    ) {
        // Outer product logic here
    }
}
```

---

### Example: Multivectors

**Complete DOP Implementation:**

**DOP-4 + DOP-2:**
```csharp
// Schema: Generic Dictionary
IReadOnlyDictionary<IIndexSet, T>

// Thin wrapper with schema
class XGaKVector<T> {
    public IReadOnlyDictionary<IIndexSet, T> ScalarDictionary { get; }
    public XGaProcessor<T> Processor { get; }
}

class XGaMultivector<T> {
    public IReadOnlyDictionary<int, XGaKVector<T>> KVectorDictionary { get; }
    public XGaProcessor<T> Processor { get; }
}
```

**DOP-3 (Immutability via Composer):**
```csharp
// Mutable builder
class XGaMultivectorComposer<T> {
    private Dictionary<IIndexSet, T> _terms = new();

    public XGaMultivectorComposer<T> SetTerm(IIndexSet id, T scalar) {
        _terms[id] = scalar;
        return this;
    }

    public XGaMultivector<T> GetMultivector() {
        // Select most efficient implementation
        if (_terms.Count == 0)
            return new ZeroMultivector<T>();
        if (_terms.Count == 1)
            return new SingleTermMultivector<T>(_terms.First());
        return new SparseMultivector<T>(_terms.ToImmutableDictionary());
    }
}
```

**DOP-1 (Behavior separate):**
```csharp
// Extension methods for operations
static class XGaMultivectorExtensions {
    public static XGaMultivector<T> Gp<T>(
        this XGaMultivector<T> mv1,
        XGaMultivector<T> mv2
    ) {
        var composer = mv1.Processor.CreateMultivectorComposer();

        // Geometric product logic
        foreach (var (id1, scalar1) in mv1.Terms)
        foreach (var (id2, scalar2) in mv2.Terms) {
            var product = id1.Gp(id2, mv1.Processor.Metric);
            composer.AddTerm(product.IndexSet,
                mv1.Processor.ScalarProcessor.Times(
                    scalar1,
                    scalar2,
                    product.Sign
                )
            );
        }

        return composer.GetMultivector();
    }
}
```

---

## Practical Examples

### Example 1: Adding a New Scalar Type

Thanks to DOP principles, this is simple:

```csharp
// 1. Implement IScalarProcessor<T> (DOP-1: Behavior)
public class MyCustomScalarProcessor : INumericScalarProcessor<MyScalar> {
    public MyScalar Add(MyScalar a, MyScalar b) => a + b;
    public MyScalar Times(MyScalar a, MyScalar b) => a * b;
    // ... additional operations
}

// 2. Use it with GA-FuL (DOP-4: Schema compatibility)
var scalarProcessor = new MyCustomScalarProcessor();
var processor = XGaProcessor<MyScalar>.CreateEuclidean(scalarProcessor);

// 3. All GA operations work automatically!
var v1 = processor.CreateVectorComposer()
    .SetVectorTerm(0, a)
    .SetVectorTerm(1, b)
    .SetVectorTerm(2, c)
    .GetVector();
var v2 = processor.CreateVectorComposer()
    .SetVectorTerm(0, x)
    .SetVectorTerm(1, y)
    .SetVectorTerm(2, z)
    .GetVector();
var result = v1.Gp(v2);
```

---

### Example 2: New Index-Set Implementation

```csharp
// 1. Implement IIndexSet (DOP-4: Schema)
public class BitVectorIndexSet : IIndexSet {
    private readonly BitVector32 _bits;  // DOP-3: Immutable

    public int Count => _bits.Data.CountBits();
    public bool Contains(int index) => _bits[1 << index];
    // ...
}

// 2. Use it (DOP-2: Generic structure)
IIndexSet indexSet = new BitVectorIndexSet(bits);
var blade = new XGaBasisBlade(indexSet);

// 3. All operations work (DOP-1: Behavior separate)
var result = blade.Op(otherBlade, metric);
```

---

### Example 3: New Geometric Operation

```csharp
// DOP-1: Add behavior as extension method
public static class MyCustomOperations {
    public static XGaMultivector<T> MyCustomProduct<T>(
        this XGaMultivector<T> mv1,
        XGaMultivector<T> mv2
    ) {
        // DOP-2: Work with generic data structures
        var composer = mv1.Processor.CreateMultivectorComposer();

        foreach (var (id1, s1) in mv1.Terms)
        foreach (var (id2, s2) in mv2.Terms) {
            // Use existing structures
            var id = ComputeResultingId(id1, id2);
            var scalar = ComputeResultingScalar(s1, s2);
            composer.AddTerm(id, scalar);
        }

        // DOP-3: Return immutable object
        return composer.GetMultivector();
    }
}

// Usage
var result = multivector1.MyCustomProduct(multivector2);
```

---

## Summary

### Core Design Intentions

| CDI | Goal | Benefit |
|-----|------|--------|
| **CDI-1** | Scalar abstraction | Flexibility in scalar types |
| **CDI-2** | Sparse storage | High-dimensional GAs practical |
| **CDI-3** | Metaprogramming | Optimized code generation |
| **CDI-4** | Layered design | Different abstraction levels |
| **CDI-5** | Unified API | Consistent, extensible interface |

### DOP Principles

| DOP | Principle | Implementation in GA-FuL |
|-----|---------|--------------------------|
| **DOP-1** | Separation of behavior and data | Extension Methods + Thin Wrappers |
| **DOP-2** | Generic data structures | Dictionary, Array, HashSet |
| **DOP-3** | Immutable data | Composer pattern for construction |
| **DOP-4** | Schema separation | Interfaces + Multiple implementations |

### Benefits of the Combination

✓ **Readable:** Clear separation, simple structures
✓ **Maintainable:** Low coupling, high cohesion
✓ **Extensible:** New features easy to add
✓ **Performant:** Optimization possibilities at all levels
✓ **Flexible:** Supports different use cases
✓ **Testable:** Every component testable in isolation

---

**Last Updated**: 2025-10-27 | **Design**: DOP + 5 CDIs | **Performance**: Generic<T> 1.24-2.31x faster 🚀

**Note:** These design principles enabled the performance improvements documented in [GENERIC_VS_SPECIALIZED_PERFORMANCE.md](../GENERIC_VS_SPECIALIZED_PERFORMANCE.md).

[← Back to Main Documentation](README.en.md)
