# Tech Stack

## Core Technologies
- **.NET 8.0**: Target framework for all projects
- **C# 12**: Latest language version with modern features
- **ImplicitUsings**: Enabled in most projects
- **Nullable Reference Types**: Enabled for better type safety

## Testing Framework
- **NUnit 4.3.2**: Primary testing framework
  - `[TestFixture]` attribute for test classes
  - `[Test]` attribute for test methods
  - NUnit3TestAdapter for test execution
- **Microsoft.NET.Test.Sdk 17.14.1**: Test SDK for running tests
- **xunit.core 2.9.3**: Also available in test projects

## Key NuGet Dependencies

### Mathematical Libraries
- **AngouriMath 1.3.0**: Symbolic mathematics
- **MathNet.Numerics 5.0.0**: Numerical computing
- **PeterO.Numbers 1.8.2**: Arbitrary precision arithmetic
- **NumpyDotNet 0.9.87.2**: NumPy-like functionality

### Performance & GPU Computing
- **ILGPU 1.5.3**: GPU-accelerated parallel algorithms
- **ILGPU.Algorithms 1.5.3**: Algorithm library for ILGPU
- **ILGPU.Lightning 0.3.0**: High-level ILGPU utilities
- **HonkPerf.NET.Core 1.0.2**: Performance monitoring

### Visualization & Graphics
- **OxyPlot.Core 2.2.0**: 2D plotting library
- **OxyPlot.SkiaSharp 2.2.0**: SkiaSharp rendering backend
- **SixLabors.ImageSharp 3.1.10**: Image processing

### Game Engines (for specific projects)
- **Stride Engine**: 3D game engine integration
- **MonoGame**: Cross-platform game framework

### Data & Integration
- **CsvHelper 33.1.0**: CSV file handling
- **EPPlus 8.0.7**: Excel file manipulation
- **Dew.Lab.Studio**: Signal processing and analysis (multiple packages)

### External Tool Integration
- **Wolfram Mathematica**: Optional for symbolic computations
- **MATLAB**: Optional for MATLAB integration and toolbox

## Development Environment
- **Windows**: Primary development platform
- **Visual Studio 2022**: Recommended IDE (Version 17+)
- **Visual Studio Code**: Alternative with .vscode configuration
- **Git**: Version control
