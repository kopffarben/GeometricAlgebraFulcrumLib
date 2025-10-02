# Geometric Algebra Fulcrum Library (GA-FuL)

A unified, generic C# library for geometric algebra computations using any kind of scalars (floating point, rational, symbolic, etc.).

**Author:** Ahmad H. Eid
**Email:** ga.computing.eg@gmail.com

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12-green.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-Check%20LICENSE%20file-yellow.svg)](LICENSE)

## 📖 Documentation

**Complete documentation is available at:**
**[https://kopffarben.github.io/GeometricAlgebraFulcrumLib/](https://kopffarben.github.io/GeometricAlgebraFulcrumLib/)**

The documentation is available in both **English** and **German** (Deutsch).

---

## What is Geometric Algebra?

**Geometric Algebra (GA)** is a powerful mathematical language that unifies many algebraic tools under the same framework:

- Real vectors
- Complex numbers
- Quaternions
- Octonions
- Spinors
- Matrices
- and many more

GA unifies the **geometric reasoning** process among many seemingly diverse application domains.

---

## Key Features

### 1. **Generic Scalar Abstraction**
Support for various scalar types:
- Floating point (32-bit, 64-bit)
- Arbitrary precision decimals
- Rational numbers
- **Symbolic expressions** (Mathematica, SymPy, etc.)
- Multi-dimensional arrays and tensors

### 2. **Memory-Efficient Sparse Multivectors**
- Optimized data structures for high-dimensional GAs
- Up to 64 dimensions (optimized)
- Arbitrary dimensions (generic)

### 3. **Metaprogramming & Code Generation**
- Automatic code generation from GA expressions
- Optimization through CSE, constant propagation, symbolic simplification
- Target languages: **C/C++, C#, Java, JavaScript, Python, MATLAB**

### 4. **Layered Architecture**
- **Algebra Layer:** Core GA operations
- **Modeling Layer:** Geometric modeling (CGA, PGA) and visualization
- **Metaprogramming Layer:** Code generation and optimization
- **System Utilities Layer:** Text, code, and web graphics utilities

### 5. **Data-Oriented Programming (DOP)**
- Separation of behavior and data
- Generic, immutable data structures
- Composer pattern for object construction

---

## Quick Start

### Installation

```bash
# Clone the repository
git clone https://github.com/ga-explorer/GeometricAlgebraFulcrumLib.git
cd GeometricAlgebraFulcrumLib

# Build the solution
dotnet build
```

### Simple Example

```csharp
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Extended.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;

// Create a CGA space for 3D geometry
var cga = CGaFloat64GeometricSpace5D.Instance;

// Encode points as CGA null vectors
var point1 = cga.Encode.IpnsRound.Point(3.5, 4.3, 2.6);
var point2 = cga.Encode.IpnsRound.Point(-2.1, 3.4, 5.0);

// Encode a sphere
var sphere = cga.Encode.IpnsRound.Sphere(0, 0, 0, 5.0);

// Perform GA operations
var pointPair = point1.Op(point2);
var intersection = sphere.Op(pointPair);
```

For more examples, visit the [Examples Documentation](https://kopffarben.github.io/GeometricAlgebraFulcrumLib/examples.en.html).

---

## Application Domains

- Computer Graphics and Visualization
- Robotics and Motion Control
- Physics Simulations
- Signal and Image Processing
- Machine Learning and AI
- Computer Vision
- Mathematical Modeling
- High-Performance Computing

---

## Project Components

| Module | Description |
|--------|-------------|
| **GeometricAlgebraFulcrumLib.Algebra** | Core algebra layer with GA operations |
| **GeometricAlgebraFulcrumLib.Modeling** | Geometric modeling and visualization |
| **GeometricAlgebraFulcrumLib.MetaProgramming** | Code generation and optimization |
| **GeometricAlgebraFulcrumLib.Mathematica** | Wolfram Mathematica integration |
| **GeometricAlgebraFulcrumLib.Matlab** | MATLAB integration and toolbox |
| **GeometricAlgebraFulcrumLib.Applications** | Application examples |
| **GeometricAlgebraFulcrumLib.Utilities** | Text, code, and data structure utilities |

---

## System Requirements

- **.NET 8.0** or higher
- **C# 12** (latest)
- Optional:
  - Wolfram Mathematica (for symbolic computations)
  - MATLAB (for MATLAB integration)

---

## Documentation Sections

Visit [https://kopffarben.github.io/GeometricAlgebraFulcrumLib/](https://kopffarben.github.io/GeometricAlgebraFulcrumLib/) for:

- **[Getting Started](https://kopffarben.github.io/GeometricAlgebraFulcrumLib/getting-started.en.html)** - Installation and first steps
- **[Architecture](https://kopffarben.github.io/GeometricAlgebraFulcrumLib/architecture.en.html)** - System design and layers
- **[Design Principles](https://kopffarben.github.io/GeometricAlgebraFulcrumLib/design-principles.en.html)** - Core design intentions
- **[Examples](https://kopffarben.github.io/GeometricAlgebraFulcrumLib/examples.en.html)** - Comprehensive code examples
- **[API Reference](https://kopffarben.github.io/GeometricAlgebraFulcrumLib/api-reference.en.html)** - Detailed API documentation
- **[Project Structure](https://kopffarben.github.io/GeometricAlgebraFulcrumLib/project-structure.en.html)** - Module organization

**Available in English and German (Deutsch).**

---

## Citation

If you use GA-FuL in your research or project, please cite:

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

## Resources

- **Documentation:** [https://kopffarben.github.io/GeometricAlgebraFulcrumLib/](https://kopffarben.github.io/GeometricAlgebraFulcrumLib/)
- **Main Repository:** [GA-FuL on GitHub](https://github.com/ga-explorer/GeometricAlgebraFulcrumLib)
- **Publication:** [MDPI Mathematics - GA-FuL Article](https://www.mdpi.com/2227-7390/12/14/2272)
- **Predecessor Project:** [GMac](https://github.com/ga-explorer/GMac)

---

## Contact

For questions, suggestions, or collaboration:

**Ahmad H. Eid**
Email: ga.computing.eg@gmail.com

---

## License

See the [LICENSE](LICENSE) file for details.
