---
layout: default
title: "Architecture"
lang: en
---

# GA-FuL Architecture

**[🇩🇪 Deutsche Version](architecture.de.md)**

## Table of Contents

1. [Overview](#overview)
2. [Component Layers](#component-layers)
3. [Algebra Layer](#algebra-layer)
4. [Modeling Layer](#modeling-layer)
5. [Metaprogramming Layer](#metaprogramming-layer)
6. [System Utilities Layer](#system-utilities-layer)
7. [Data Flow](#data-flow)

---

## Overview

The architecture of GA-FuL follows a **layered design**, where each layer takes on specific responsibilities and builds upon the functionalities of the underlying layers. This architecture allows users with different backgrounds to choose the appropriate level of abstraction for their needs.

```
┌─────────────────────────────────────────────┐
│      Metaprogramming Layer                  │
│  (Code Generation & Optimization)           │
├─────────────────────────────────────────────┤
│         Modeling Layer                      │
│  (Geometry, Visualization, Calculus)        │
├─────────────────────────────────────────────┤
│           Algebra Layer                     │
│  (GA Operations, Scalars, Multivectors)     │
├─────────────────────────────────────────────┤
│       System Utilities Layer                │
│  (Data Structures, Text, Code, Web)         │
└─────────────────────────────────────────────┘
```

---

## Component Layers

### 1. Algebra Layer

**Purpose:** Fundamental GA operations and data structures

**Main Components:**
- Scalar Processors
- Basis Blades
- Multivectors
- Linear Maps
- Transformations

**Details:** See [Algebra Layer](#algebra-layer)

---

### 2. Modeling Layer

**Purpose:** Geometric modeling and visualization

**Main Components:**
- Geometry Subsystem
- Visualization Subsystem
- Calculus Subsystem (in development)

**Details:** See [Modeling Layer](#modeling-layer)

---

### 3. Metaprogramming Layer

**Purpose:** Code generation and optimization

**Main Components:**
- Meta Context
- Expression Trees
- Code Optimizer
- Code Composer

**Details:** See [Metaprogramming Layer](#metaprogramming-layer)

---

### 4. System Utilities Layer

**Purpose:** Basic services for all other layers

**Main Components:**
- Data Structures
- Text Utilities
- Code Utilities
- Web Graphics Utilities

**Details:** See [System Utilities Layer](#system-utilities-layer)

---

## Algebra Layer

The Algebra Layer is the foundation of GA-FuL and fulfills the core design intentions CDI-1 and CDI-2.

### Scalar Representation

```
IScalarProcessor<T>
├── INumericScalarProcessor<T>
│   ├── ScalarProcessorOfFloat32
│   ├── ScalarProcessorOfFloat64
│   ├── ScalarProcessorOfDecimal
│   ├── ScalarProcessorOfRational
│   └── ScalarProcessorOfComplex
└── ISymbolicScalarProcessor<T>
    ├── MathematicaScalarProcessor
    └── [Future CAS Integrations]
```

**Important Classes:**

#### `IScalarProcessor<T>`
Generic interface for scalar operations:
- Arithmetic (addition, subtraction, multiplication, division)
- Transcendental functions (trigonometry, exponential, logarithm)
- Comparison operations

#### `Scalar<T>`
Thin wrapper class for simpler API:
```csharp
// Without wrapper (complicated):
w = scalarProcessor.Add(x, scalarProcessor.Times(y, z));

// With wrapper (simple):
w = x + y * z;
```

---

### Basis Blades

**Representation:**

Basis blades $e_{i_1, i_2, \ldots, i_k}$ are internally represented by a sorted set of indices.

```
IIndexSet
├── EmptyIndexSet (empty set)
├── SingleIndexSet (one element)
├── UInt64IndexSet (for dimensions < 64)
├── DenseIndexSet (arbitrary size, array-based)
└── SparseIndexSet (arbitrary size, HashSet-based)
```

**Important Classes:**

#### `XGaBasisBlade`
Thin wrapper around `IIndexSet` with methods for:
- Geometric Product
- Outer Product
- Inner Product
- Reverse Operation
- Additional bilinear products

#### `XGaSignedBasisBlade`
Extends `XGaBasisBlade` with a sign (±1 or 0)

#### `XGaMetric`
Processes basis blades with specific metric signature $(p, q, r)$:
- $p$: Number of basis vectors with $e_i^2 = +1$
- $q$: Number of basis vectors with $e_i^2 = -1$
- $r$: Number of basis vectors with $e_i^2 = 0$

---

### Multivectors

**Data Structure:**

```
Multivector
└── IReadOnlyDictionary<int, XGaKVector<T>>
    └── Grade k → k-Vector
        └── IReadOnlyDictionary<IIndexSet, T>
            └── Basis Blade → Scalar
```

**Advantages of this Structure:**
- Highly sparse representation
- Efficient memory usage
- Flexible dimensionality

**Important Classes:**

#### `XGaMultivector<T>`
Generic multivector with arbitrary scalar type `T`

#### `XGaKVector<T>`
Represents a k-vector (all components have the same grade)

#### `XGaProcessor<T>`
Main processor for multivector operations:
- Algebraic operations
- Geometric products
- Grade extraction
- Normalization

#### `XGaMultivectorComposer<T>`
Composer class for creating multivectors (DOP Principle 3)

---

### Transformations

GA-FuL provides classes for common GA transformations:

| Transformation | Class | Description |
|----------------|--------|--------------|
| **Outermorphism** | `XGaOutermorphism<T>` | General linear transformation |
| **Versor** | `XGaVersor<T>` | Orthogonal operators |
| **Rotor** | `XGaRotor<T>` | Rotations |
| **Reflector** | `XGaReflector<T>` | Reflections |
| **Projector** | `XGaProjector<T>` | Projections |

---

### Optimized Float64 Implementation

For performance-critical applications:

**`RGaFloat64Multivector`**
- Optimized for 64-bit floating-point numbers
- Supports up to 64 dimensions
- Uses lookup tables for dimensions ≤ 12
- Faster operations than generic version

---

## Modeling Layer

The Modeling Layer fulfills CDI-5 and provides specialized abstractions for various application domains.

### Geometry Subsystem

**Supported GA Types:**

#### 1. Directional GA (Vector Algebra)
Standard vector algebra with outer product

#### 2. Conformal GA (CGA)
Modeling of 3D geometry in 5D CGA space

**Classes:**
- `XGaConformalSpace<T>`: Base class
- `XGaConformalSpace4D<T>`: 4D CGA (for 2D geometry)
- `XGaConformalSpace5D<T>`: 5D CGA (for 3D geometry)

**Functionalities:**
- Encoding geometric objects (points, lines, planes, circles, spheres)
- Decoding CGA blades
- Geometric operations (intersections, projections, reflections)
- Transformations (rotation, translation, inversion)

**Example:**
```csharp
var cga = CGaGeometricSpace5D<double>.Create(scalarProcessor);

// Encode a point
var point = cga.EncodeIpnsRound.Point(x, y, z);

// Encode a sphere
var sphere = cga.EncodeIpnsRound.Sphere(centerX, centerY, centerZ, radius);

// Intersect two objects
var intersection = sphere.Op(plane);
```

#### 3. Projective GA (PGA)
Projective geometry modeling

**Special Feature in GA-FuL:**
CGA and PGA are implemented within the same algebraic space (based on the DPGA concept), allowing both representations to be freely mixed.

---

### Visualization Subsystem

**Supported Backends:**

#### Babylon.js
- JavaScript code generation for 3D visualization
- Support for static and animated objects
- Interactive scenes in browser

#### Selenium WebDriver
- Browser automation for video creation
- Combines individual frames into videos

**Examples:**
In the repository under `GeometricAlgebraFulcrumLib.Visualizations`

---

### Calculus Subsystem

**Status:** In development

**Planned Functionalities:**
- Geometric calculus on multivectors
- Derivatives
- Integrals
- Differential forms

---

## Metaprogramming Layer

The Metaprogramming Layer fulfills CDI-3 and enables automatic code generation.

### Workflow

```
1. Initialize MCO
      ↓
2. Define input parameters
      ↓
3. Describe GA operations
      ↓
4. Select output variables
      ↓
5. Set variable names
      ↓
6. Optimize code
      ↓
7. Initialize CCO
      ↓
8. Generate final code
```

---

### Meta Context

**`MetaContext`**: Main class for metaprogramming

**Functionalities:**
- Management of meta expressions
- Automatic conversion of GA operations to scalar operations
- Optimizations

**Meta Expression Hierarchy:**

```
IMetaExpression
├── IMetaExpressionAtomic
│   ├── MetaExpressionNumber (Constant)
│   └── MetaExpressionVariable (Parameter)
└── IMetaExpressionComposite
    ├── MetaExpressionNegative
    ├── MetaExpressionAdd
    ├── MetaExpressionSubtract
    ├── MetaExpressionTimes
    ├── MetaExpressionDivide
    ├── MetaExpressionPower
    └── MetaExpressionFunction (sin, cos, exp, etc.)
```

---

### Optimizations

The MCO performs the following optimizations:

1. **Constant Propagation**
   - Evaluates constant expressions at compile time

2. **Common Subexpression Elimination (CSE)**
   - Identifies repeated subexpressions
   - Extracts them into intermediate variables

3. **Symbolic Simplification**
   - Optional: Use of external CAS (e.g., Mathematica)
   - Simplifies complex expressions

4. **Variable Pruning**
   - Removes unused intermediate variables
   - Removes redundant computations

5. **Reduction of Computation Steps**
   - Optional: Genetic programming
   - Evolution strategy (4+1)

---

### Code Composer

**`CodeComposer`**: Converts meta expressions to target language code

**Supported Languages:**
- C/C++
- C#
- Java
- JavaScript
- Python
- MATLAB

**Template-based Code Generation:**
- Single code files
- Multi-file projects
- Complete libraries with folder structure

**Example Workflow:**
```csharp
// 1. Create MCO
var context = new MetaContext();

// 2. Input parameters
var x = context.CreateParameter("x");
var y = context.CreateParameter("y");

// 3. GA operations
var multivector1 = processor.CreateVector(x, y, 0);
var multivector2 = processor.CreateVector(1, 1, 1);
var result = multivector1.Gp(multivector2);

// 4. Define output
context.SetOutput("result", result);

// 5. Optimize
context.Optimize();

// 6. Generate code
var codeComposer = new CSharpCodeComposer();
var code = codeComposer.Generate(context);
```

---

## System Utilities Layer

The Utilities Layer provides basic services for all other layers.

### Data Structures Subsystem

**Important Classes:**

#### `IReadOnlyDictionary<TKey, TValue>` Implementations
- `EmptyDictionary<TKey, TValue>`: Empty dictionary
- `SingleItemDictionary<TKey, TValue>`: One element
- `Dictionary<TKey, TValue>`: Standard dictionary

**Usage:**
Efficient storage of sparse data (multivectors, k-vectors)

---

### Text Utilities Subsystem

**Functionalities:**
- Formatted text generation
- LaTeX code composition
- Parametric text templates
- Hierarchical folder/file creation

**Important Classes:**
- `LinearTextComposer`: Linear text composition
- `ParametricTextComposer`: Template-based text generation
- `LaTeXComposer`: LaTeX document creation

---

### Code Utilities Subsystem

**Functionalities:**
- Language-independent Abstract Syntax Trees (ASTs)
- Code generation from ASTs
- Code formatting

**Usage:**
Supports the Metaprogramming Layer

---

### Web Graphics Utilities Subsystem

**Functionalities:**
- Web-based graphics code generation
- Support for Babylon.js
- SVG generation
- HTML/CSS/JavaScript utilities

**Usage:**
Supports the Visualization Subsystem of the Modeling Layer

---

## Data Flow

### Typical Workflow for Numerical Computations

```
1. Choose scalar processor
   ↓
2. Create GA processor
   ↓
3. Define multivectors
   ↓
4. Perform GA operations
   ↓
5. Extract results
```

### Typical Workflow for Code Generation

```
1. Choose symbolic scalar processor
   ↓
2. Create meta context
   ↓
3. Define input parameters
   ↓
4. Describe GA operations
   ↓
5. Optimize
   ↓
6. Generate code
```

### Typical Workflow for Visualization

```
1. Model geometric objects
   ↓
2. Create visualization context
   ↓
3. Generate rendering code
   ↓
4. Render in browser/framework
```

---

## Design Decisions

### Why Layered Architecture?

1. **Separation of Concerns**: Each layer has clear responsibilities
2. **Reusability**: Lower layers can be used independently
3. **Extensibility**: New features can be added without breaking existing ones
4. **Testability**: Each layer can be tested in isolation

### Why Data-Oriented Programming?

1. **Reduced Complexity**: Separation of data and behavior
2. **Better Maintainability**: Simpler code structure
3. **Higher Flexibility**: Generic data structures
4. **Immutability**: Thread safety and predictability

---

## Performance Considerations

### Optimized Implementations

| Scenario | Implementation | Performance |
|----------|-----------------|-------------|
| **Small Dimensions (< 12)** | Lookup tables | Very fast |
| **Medium Dimensions (< 64)** | UInt64-based indices | Fast |
| **Large Dimensions** | Sparse dictionary | Moderate performance |
| **Float64-specific** | `RGaFloat64Multivector` | Optimized |
| **High-Performance** | Code generation | Maximum |

---

## Extensibility

### Adding New Scalar Types

1. Implement `IScalarProcessor<T>` or `INumericScalarProcessor<T>`
2. Register the processor
3. Use it with `XGaProcessor<T>`

### Adding New Geometries

1. Extend `XGaConformalSpace<T>` or create new base class
2. Implement encoding/decoding methods
3. Add specific operations

### Adding New Target Languages for Code Generation

1. Extend `CodeComposer`
2. Implement language-specific syntax
3. Register the composer

---

## Summary

The GA-FuL architecture provides:

✓ **Flexibility** through generic scalars and multivectors
✓ **Efficiency** through optimized data structures
✓ **Extensibility** through layered design
✓ **Maintainability** through Data-Oriented Programming
✓ **Performance** through multiple optimization levels
✓ **User-friendliness** through different abstraction levels

---

[← Back to Main Documentation](README.en.md)
