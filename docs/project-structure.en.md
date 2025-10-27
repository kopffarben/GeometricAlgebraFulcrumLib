---
layout: default
title: "Project Structure"
lang: en
---

# GA-FuL Project Structure

**[🇩🇪 Deutsche Version](project-structure.de.md)**

---

## Table of Contents

1. [Overview](#overview)
2. [Main Modules](#main-modules)
3. [Directory Structure](#directory-structure)
4. [Dependencies](#dependencies)
5. [Build Configuration](#build-configuration)

---

## Overview

The GA-FuL project is divided into several modules, each providing specific functionalities. The structure follows the [layered design](architecture.en.md) of the library.

```
GeometricAlgebraFulcrumLib/
├── GeometricAlgebraFulcrumLib/          # Main folder with all modules
│   ├── GeometricAlgebraFulcrumLib.Algebra/
│   ├── GeometricAlgebraFulcrumLib.Modeling/
│   ├── GeometricAlgebraFulcrumLib.MetaProgramming/
│   ├── GeometricAlgebraFulcrumLib.Utilities.*/
│   ├── GeometricAlgebraFulcrumLib.Applications/
│   ├── GeometricAlgebraFulcrumLib.Mathematica/
│   ├── GeometricAlgebraFulcrumLib.Matlab/
│   └── ...
├── GeometricAlgebraFulcrumLib.Documentation/
├── GeometricAlgebraFulcrumLib.Visualizations/
├── docs/                                 # This documentation
├── assets/
├── README.adoc
└── LICENSE
```

---

## Main Modules

### Core Modules (Algebra Layer)

#### 1. **GeometricAlgebraFulcrumLib.Algebra**

**Purpose:** Core algebra implementation

**Main Components:**
- `GeometricAlgebra/`: GA operations and multivectors
  - `Basis/`: Basis blades and index sets
  - `Dense/`: Optimized dense implementation
  - `Extended/`: Extended GA (XGa)
  - `Restricted/`: Restricted GA (RGa)
- `ComplexAlgebra/`: Complex numbers
- `LinearAlgebra/`: Vectors, matrices, tensors
- `Scalars/`: Scalar processors
- `GeometricFrequency/`: Spectral analysis

**Dependencies:**
- MathNet.Numerics
- PeterO.Numbers
- AngouriMath
- NumpyDotNet

**Important Classes:**
```
GeometricAlgebra/
├── XGaProcessor<T>              # Main GA processor
├── XGaMultivector<T>            # Multivector
├── XGaKVector<T>                # k-vector
├── XGaBasisBlade               # Basis blade
├── XGaMetric                   # Metric definition
└── Composers/
    └── XGaMultivectorComposer<T>  # Multivector builder
```

---

#### 2. **GeometricAlgebraFulcrumLib.Utilities.Structures**

**Purpose:** Basic data structures

**Main Components:**
- `IndexSets/`: IIndexSet implementations
- `Dictionaries/`: Optimized dictionary implementations
- `Collections/`: Specialized collections
- `BitManipulation/`: Bit operations for efficient index sets

**Important Classes:**
```
IndexSets/
├── IIndexSet                    # Interface
├── EmptyIndexSet               # Empty set
├── SingleIndexSet              # One element
├── UInt64IndexSet              # For dimension < 64
├── DenseIndexSet               # Dense array
└── SparseIndexSet              # Sparse hash set
```

---

#### 3. **GeometricAlgebraFulcrumLib.Utilities.Text**

**Purpose:** Text and LaTeX generation

**Main Components:**
- `Text/`: Formatted text generation
- `LaTeX/`: LaTeX code composition
- `Parametric/`: Template-based text generation
- `Files/`: File and folder hierarchy creation

---

#### 4. **GeometricAlgebraFulcrumLib.Utilities.Code**

**Purpose:** Code generation utilities

**Main Components:**
- `SyntaxTree/`: Abstract Syntax Trees (AST)
- `Languages/`: Language-specific code generators
- `Expressions/`: Expression representations

---

#### 5. **GeometricAlgebraFulcrumLib.Utilities.Web**

**Purpose:** Web graphics utilities

**Main Components:**
- `Svg/`: SVG generation
- `Html/`: HTML generation
- `JavaScript/`: JavaScript code generation
- `BabylonJs/`: Babylon.js integration

---

### Modeling Modules

#### 6. **GeometricAlgebraFulcrumLib.Modeling**

**Purpose:** Geometric modeling

**Main Components:**
- `Geometry/`: Geometric objects
  - `CGa/`: Conformal Geometric Algebra
  - `PGa/`: Projective Geometric Algebra
  - `Euclidean/`: Euclidean geometry
- `Graphics/`: Graphics primitives
- `Calculus/`: Geometric calculus (in development)

**Important Classes:**
```
Geometry/
└── CGa/
    ├── CGaGeometricSpace5D<T>      # 5D CGA for 3D geometry
    ├── CGaGeometricSpace4D<T>      # 4D CGA for 2D geometry
    └── Encoding/
        ├── EncodeIpnsRound         # IPNS round objects
        ├── EncodeOpnsFlat          # OPNS flat objects
        └── Decode                  # Decoding
```

---

### Metaprogramming Modules

#### 7. **GeometricAlgebraFulcrumLib.MetaProgramming**

**Purpose:** Code generation and optimization

**Main Components:**
- `Context/`: Meta context
- `Expressions/`: Meta expressions
- `Composers/`: Code composers
- `Optimizers/`: Code optimizers
- `Applications/`: Application examples

**Important Classes:**
```
MetaProgramming/
├── MetaContext                  # Main context
├── IMetaExpression              # Expression interface
├── MetaExpressionComposer       # Expression builder
├── CodeComposer                 # Code generator
└── Optimizers/
    ├── ConstantPropagation
    ├── CommonSubexpressionElimination
    └── GeneticProgramming
```

---

### Integration Modules

#### 8. **GeometricAlgebraFulcrumLib.Mathematica**

**Purpose:** Wolfram Mathematica integration

**Main Components:**
- `Processors/`: Mathematica scalar processor
- `Expressions/`: Mathematica expression wrapper
- `Utilities/`: Helper functions

**Dependencies:**
- Wolfram.NETLink

---

#### 9. **GeometricAlgebraFulcrumLib.Matlab**

**Purpose:** MATLAB integration and toolbox

**Main Components:**
- `GA-FuL MATLAB Toolbox/`: Complete MATLAB toolbox
- `Processors/`: MATLAB scalar processor
- `CodeGeneration/`: MATLAB code generation

**Features:**
- MATLAB function generation
- GA operations in MATLAB
- Visualization

---

### Application Modules

#### 10. **GeometricAlgebraFulcrumLib.Applications**

**Purpose:** General application examples

**Main Components:**
- Various application examples for GA-FuL
- Demos and tutorials

---

#### 11. **GeometricAlgebraFulcrumLib.Applications.Symbolic**

**Purpose:** Symbolic applications

**Main Components:**
- Symbolic GA computations
- Mathematica-based applications

---

#### 12. **GeometricAlgebraFulcrumLib.Samples.Generations**

**Purpose:** Code generation examples

**Main Components:**
- Code generation samples
- Template examples

---

### Additional Modules

#### 13. **GeometricAlgebraFulcrumLib.MonoGame**

**Purpose:** MonoGame integration for games and graphics

---

#### 14. **GeometricAlgebraFulcrumLib.Stride**

**Purpose:** Stride Game Engine integration

---

#### 15. **GeometricAlgebraFulcrumLib.Optimization**

**Purpose:** Optimization algorithms

---

#### 16. **GeometricAlgebraFulcrumLib.Benchmarks**

**Purpose:** Performance benchmarks

**Tools:**
- BenchmarkDotNet
- Performance tests

---

#### 17. **GeometricAlgebraFulcrumLib.UnitTests**

**Purpose:** Unit tests

**Framework:**
- xUnit or NUnit

---

## Directory Structure

### Typical Module Structure

```
GeometricAlgebraFulcrumLib.ModuleName/
├── GeometricAlgebraFulcrumLib.ModuleName.csproj
├── Namespace1/
│   ├── Class1.cs
│   ├── Class2.cs
│   └── Subfolder/
│       └── Class3.cs
├── Namespace2/
│   └── ...
└── Properties/
    └── AssemblyInfo.cs (optional)
```

### Example: Algebra Module

```
GeometricAlgebraFulcrumLib.Algebra/
├── GeometricAlgebraFulcrumLib.Algebra.csproj
├── Scalars/
│   ├── IScalarProcessor.cs
│   ├── ScalarProcessorOfFloat64.cs
│   ├── ScalarProcessorOfFloat32.cs
│   └── ...
├── GeometricAlgebra/
│   ├── Basis/
│   │   ├── IIndexSet.cs
│   │   ├── XGaBasisBlade.cs
│   │   └── ...
│   ├── Extended/
│   │   ├── XGaProcessor.cs
│   │   ├── XGaMultivector.cs
│   │   ├── XGaKVector.cs
│   │   └── Composers/
│   │       └── XGaMultivectorComposer.cs
│   └── Restricted/
│       ├── RGaProcessor.cs
│       └── ...
├── LinearAlgebra/
│   ├── Vectors/
│   ├── Matrices/
│   └── Tensors/
└── ComplexAlgebra/
    └── ...
```

---

## Dependencies

### Dependency Graph

```
┌─────────────────────────────────────┐
│   Applications, Samples, Tests      │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│  MetaProgramming, Mathematica       │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│         Modeling                    │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│         Algebra                     │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│      Utilities (Text, Code,         │
│      Structures, Web)               │
└─────────────────────────────────────┘
```

### NuGet Packages

**Algebra Module:**
```xml
<PackageReference Include="AngouriMath" Version="1.3.0" />
<PackageReference Include="MathNet.Numerics" Version="5.0.0" />
<PackageReference Include="PeterO.Numbers" Version="1.8.2" />
<PackageReference Include="NumpyDotNet" Version="0.9.87.2" />
<PackageReference Include="OxyPlot.Core" Version="2.2.0" />
<PackageReference Include="SixLabors.ImageSharp" Version="3.1.10" />
<PackageReference Include="EPPlus" Version="8.0.7" />
<PackageReference Include="Dew.Math" Version="6.2.3" />
```

**Mathematica Module:**
```xml
<Reference Include="Wolfram.NETLink" />
```

---

## Build Configuration

### .NET Configuration

All projects use:
```xml
<PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

### Build Commands

**Build entire solution:**
```bash
cd GeometricAlgebraFulcrumLib
dotnet build GeometricAlgebraFulcrumLib.sln
```

**Build specific project:**
```bash
cd GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Algebra
dotnet build
```

**Run tests:**
```bash
cd GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests
dotnet test
```

**Release build:**
```bash
dotnet build -c Release
```

---

## Naming Conventions

### Assembly Names

Format: `GeometricAlgebraFulcrumLib.<ModuleName>`

Examples:
- `GeometricAlgebraFulcrumLib.Algebra`
- `GeometricAlgebraFulcrumLib.Modeling`
- `GeometricAlgebraFulcrumLib.MetaProgramming`

### Namespace Conventions

Format: `GeometricAlgebraFulcrumLib.<ModuleName>.<SubNamespace>`

Examples:
- `GeometricAlgebraFulcrumLib.Algebra.Scalars`
- `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Basis`
- `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa`

### Class Prefixes

| Prefix | Meaning | Example |
|--------|---------|---------|
| `XGa` | Extended GA | `XGaProcessor<T>` |
| `RGa` | Restricted GA | `RGaProcessor<T>` |
| `CGa` | Conformal GA | `CGaGeometricSpace5D<T>` |
| `PGa` | Projective GA | `PGaGeometricSpace<T>` |
| `Float64` | 64-bit float optimized | `RGaFloat64Multivector` |

---

## Summary

The GA-FuL project structure:

✓ **Modular**: Clear separation of responsibilities
✓ **Layered**: From Utilities to Applications
✓ **Extensible**: Easy to add new modules
✓ **Well organized**: Consistent naming conventions
✓ **Documented**: Clear structure and dependencies

---

**Last Updated**: 2025-10-27 | **Structure**: Current | **Performance**: Generic<T> 1.24-2.31x faster 🚀

[← Back to Main Documentation](README.en.md)
