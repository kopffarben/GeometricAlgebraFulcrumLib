# Project Structure

## Solution Organization
The project is organized as a single Visual Studio solution containing multiple projects:
- **Main Solution**: `GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.sln`

## Core Library Projects

### Algebra Layer
- **GeometricAlgebraFulcrumLib.Algebra**: Core geometric algebra operations and processors
  - Contains fundamental GA operations
  - Implements various scalar types (Float64, Rational, Symbolic)
  - Multivector implementations and operations

### Modeling Layer
- **GeometricAlgebraFulcrumLib.Modeling**: Geometric modeling and visualization
  - Conformal Geometric Algebra (CGA)
  - Projective Geometric Algebra (PGA)
  - Geometric modeling tools

### MetaProgramming Layer
- **GeometricAlgebraFulcrumLib.MetaProgramming**: Code generation and optimization
  - Automatic code generation from GA expressions
  - Common Subexpression Elimination (CSE)
  - Target multiple languages (C/C++, C#, Java, JavaScript, Python, MATLAB)

### Utilities Layer
- **GeometricAlgebraFulcrumLib.Utilities.Structures**: Data structures and collections
- **GeometricAlgebraFulcrumLib.Utilities.Text**: Text processing utilities
- **GeometricAlgebraFulcrumLib.Utilities.Code**: Code manipulation utilities
- **GeometricAlgebraFulcrumLib.Utilities.Web**: Web graphics utilities

## Integration Projects
- **GeometricAlgebraFulcrumLib.Mathematica**: Wolfram Mathematica integration
- **GeometricAlgebraFulcrumLib.Matlab**: MATLAB integration and toolbox

## Visualization Projects
- **GeometricAlgebraFulcrumLib.Stride**: Stride game engine integration
- **GeometricAlgebraFulcrumLib.MonoGame**: MonoGame framework integration

## Application Projects
- **GeometricAlgebraFulcrumLib.Applications**: General application examples
- **GeometricAlgebraFulcrumLib.Applications.Symbolic**: Symbolic computation examples
- **GeometricAlgebraFulcrumLib.Samples.Generations**: Code generation samples

## Testing and Performance
- **GeometricAlgebraFulcrumLib.UnitTests**: Unit test suite
  - Algebra tests
  - AutoDiff tests
  - Geometry tests
  - Linear maps tests (Outermorphisms, Reflectors, Rotors, Versors, Projectors)
  - Processing tests
  - Storage tests
- **GeometricAlgebraFulcrumLib.Benchmarks**: Performance benchmarks
- **GeometricAlgebraFulcrumLib.Optimization**: Optimization algorithms

## Other Projects
- **GA-FuL MATLAB Toolbox**: MATLAB toolbox for GA-FuL

## Directory Layout
```
GA-FUL-main/
├── .claude/                    # Claude configuration
├── .serena/                    # Serena MCP configuration
├── assets/                     # Project assets
├── GeometricAlgebraFulcrumLib/ # Main solution folder
│   ├── .vscode/               # VS Code configuration
│   ├── GeometricAlgebraFulcrumLib.Algebra/
│   ├── GeometricAlgebraFulcrumLib.Modeling/
│   ├── GeometricAlgebraFulcrumLib.MetaProgramming/
│   ├── GeometricAlgebraFulcrumLib.Mathematica/
│   ├── GeometricAlgebraFulcrumLib.Utilities.*/
│   ├── GeometricAlgebraFulcrumLib.Applications/
│   ├── GeometricAlgebraFulcrumLib.UnitTests/
│   ├── GeometricAlgebraFulcrumLib.Benchmarks/
│   └── GeometricAlgebraFulcrumLib.sln
├── GeometricAlgebraFulcrumLib.Documentation/
├── GeometricAlgebraFulcrumLib.Visualizations/
├── README.md
├── LICENSE
└── docs/guides/DEVELOPMENT_GUIDE.md    # Condensed architecture + agent guide
```

## Build Configurations
- Debug (Any CPU, x64, ARM32)
- Release (Any CPU, x64, ARM32)
