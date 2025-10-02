---
layout: default
title: "GA-FuL Documentation"
lang: en
---

# Geometric Algebra Fulcrum Library (GA-FuL) - Documentation

**Author:** Ahmad H. Eid
**Email:** ga.computing.eg@gmail.com

**[🇩🇪 Deutsche Version](README.de.md)**

---

## Table of Contents

1. [Introduction](#introduction)
2. [Installation and Getting Started](getting-started.en.md)
3. [Architecture and Design](architecture.en.md)
4. [Design Principles](design-principles.en.md)
5. [API Reference](api-reference.en.md)
6. [Usage Examples](examples.en.md)
7. [Project Structure](project-structure.en.md)
8. [Citation](#citation)

---

## Introduction

The **Geometric Algebra Fulcrum Library (GA-FuL)** is a unified, generic C# library for geometric algebra computations using any kind of scalars (floating point, rational, symbolic, etc.). GA-FuL can be used for prototyping geometric algorithms based on the powerful mathematics of geometric algebra. The GA-FuL codebase enables numeric computations, symbolic manipulations, and optimized code generation.

### What is Geometric Algebra?

**Geometric Algebra (GA)** is a powerful mathematical language that unifies many algebraic tools under the same framework of mathematical operations. Such tools include, for example:

- Real vectors
- Complex numbers
- Quaternions
- Octonions
- Spinors
- Matrices
- and many more

The most important feature of GA is, however, that it unifies the **geometric reasoning** process among many seemingly diverse application domains.

### Why the Name "Geometric Algebra Fulcrum"?

The name has two levels of meaning:

1. **Mathematical Fulcrum:** Geometric Algebra acts as a **mathematical lever** for geometric thinking across scientific and engineering domains. We can use this "Geometric Algebra Fulcrum" to seamlessly balance the abstract ideal needs of geometric reasoning with the concrete tools of algebraic manipulation under a unifying mathematical framework.

2. **Computational Fulcrum:** There is a need for software tools that act as a pivot point, i.e. a **Fulcrum**, for prototyping and implementing several kinds of computations on GA's multivectors. GA-FuL is intended to play the role of a **Computational Fulcrum** for prototyping algorithms and implementing software based on **Geometric Algebra**.

---

## Key Features

### 1. **Generic Scalar Abstraction**
- Support for various scalar representations:
  - Floating point numbers (32-bit, 64-bit)
  - Arbitrary precision decimals
  - Rational numbers
  - Symbolic expressions (Mathematica, SymPy, etc.)
  - Multi-dimensional arrays and tensors
  - Sampled signals for signal processing

### 2. **Memory-Efficient Sparse Multivectors**
- Optimized data structures for sparse multivectors in high-dimensional GAs
- Support for spaces up to 64 dimensions (optimized)
- Support for arbitrary dimensions (generic)

### 3. **Metaprogramming Capabilities**
- Automatic code generation from GA expressions
- Optimization through:
  - Constant propagation
  - Common subexpression elimination
  - Symbolic simplification
  - Genetic programming for further optimization
- Target languages:
  - C/C++
  - C#
  - Java
  - JavaScript
  - Python
  - MATLAB
  - CUDA (planned)

### 4. **Layered System Design**
- **Algebra Layer:** Fundamental GA operations and data structures
- **Modeling Layer:** Geometric modeling and visualization
- **Metaprogramming Layer:** Code generation and optimization
- **System Utilities Layer:** Support services for text, code, and web graphics

### 5. **Data-Oriented Programming (DOP)**
- Separation of behavior and data
- Use of generic data structures
- Immutable data
- Separation of data representation and data schema

---

## Application Domains

GA-FuL can be used in various domains:

- **Computer Graphics and Visualization**
- **Robotics and Motion Control**
- **Physics Simulations**
- **Signal and Image Processing**
- **Machine Learning and AI**
- **Computer Vision**
- **Mathematical Modeling**
- **Code Generation for High-Performance Computing**

---

## Project Components

The GA-FuL project consists of several modules:

| Module | Description |
|--------|-------------|
| **GeometricAlgebraFulcrumLib.Algebra** | Core algebra layer with GA operations |
| **GeometricAlgebraFulcrumLib.Modeling** | Geometric modeling and visualization |
| **GeometricAlgebraFulcrumLib.MetaProgramming** | Code generation and optimization |
| **GeometricAlgebraFulcrumLib.Mathematica** | Integration with Wolfram Mathematica |
| **GeometricAlgebraFulcrumLib.Matlab** | MATLAB integration and toolbox |
| **GeometricAlgebraFulcrumLib.Applications** | Application examples |
| **GeometricAlgebraFulcrumLib.Utilities** | Utility libraries for text, code, and structures |
| **GeometricAlgebraFulcrumLib.Benchmarks** | Performance benchmarks |
| **GeometricAlgebraFulcrumLib.UnitTests** | Unit tests |

For more details, see the [Project Structure Documentation](project-structure.en.md).

---

## System Requirements

- **.NET 8.0** or higher
- **C# 12** (latest)
- Optional:
  - Wolfram Mathematica (for symbolic computations)
  - MATLAB (for MATLAB integration)

---

## Getting Started

For a quick introduction to GA-FuL, please read the [Getting Started Guide](getting-started.en.md).

A simple example:

```csharp
// Define scalar processor for 64-bit floating point numbers
var scalarProcessor = ScalarProcessorOfFloat64.Instance;

// Create a CGA space
var cga = CGaGeometricSpace5D<double>.Create(scalarProcessor);

// Encode points as CGA null vectors
var point1 = cga.EncodeIpnsRound.Point(3.5, 4.3, 2.6);
var point2 = cga.EncodeIpnsRound.Point(-2.1, 3.4, 5.0);

// Perform GA operations
var result = point1.Op(point2);
```

For more examples, see the [Examples Documentation](examples.en.md).

---

## Citation

If you use GA-FuL in your research or project, please cite the following article:

**Eid, A.H.; Montoya, F.G.** "Developing GA-FuL: A Generic Wide-Purpose Library for Computing with Geometric Algebra." *Mathematics* 2024, 12, 2272.
DOI: [10.3390/math12142272](https://doi.org/10.3390/math12142272)

```bibtex
@Article{Eid2024,
  author    = {Eid, Ahmad Hosny and Montoya, Francisco G.},
  journal   = {Mathematics},
  title     = {Developing GA-FuL: A Generic Wide-Purpose Library for Computing with Geometric Algebra},
  year      = {2024},
  issn      = {2227-7390},
  month     = jul,
  number    = {14},
  pages     = {2272},
  volume    = {12},
  doi       = {10.3390/math12142272},
  publisher = {MDPI AG},
}
```

---

## License

For more information about the license, see the [LICENSE](../LICENSE) file in the project's root directory.

---

## Resources

- **Main Repository:** [GA-FuL on GitHub](https://github.com/ga-explorer/GeometricAlgebraFulcrumLib)
- **Publication:** [MDPI Mathematics - GA-FuL Article](https://www.mdpi.com/2227-7390/12/14/2272)
- **Predecessor Project:** [GMac](https://github.com/ga-explorer/GMac)

---

## Contact

For questions, suggestions, or collaboration, please contact:

**Ahmad H. Eid**
Email: ga.computing.eg@gmail.com

---

## Documentation Structure

This documentation is divided into several thematic sections:

- **[Getting Started](getting-started.en.md)** - Installation and first steps
- **[Architecture](architecture.en.md)** - System design and component layers
- **[Design Principles](design-principles.en.md)** - Core design intentions and DOP principles
- **[Examples](examples.en.md)** - Comprehensive code examples
- **[API Reference](api-reference.en.md)** - Detailed API documentation
- **[Project Structure](project-structure.en.md)** - Module organization and dependencies

**Language Options:**
- **English:** `.md` files
- **Deutsch:** `.de.md` files

---

**Note:** This documentation provides comprehensive information about GA-FuL. Use the table of contents to navigate to desired topics.
