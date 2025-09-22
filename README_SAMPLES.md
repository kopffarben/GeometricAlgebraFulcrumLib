# GeometricAlgebraFulcrumLib Samples Organization

This document provides an overview of the organized sample code structure within the GeometricAlgebraFulcrumLib repository.

## Quick Start

### Interactive Samples Runner
Run the centralized samples runner for an interactive menu:
```csharp
SamplesRunner.RunInteractively();
```

### Run All Samples
Execute all samples automatically:
```csharp
SamplesRunner.RunAllSamples();
```

### Command Line Usage
```bash
dotnet run                    # Interactive mode
dotnet run -- --all         # Run all samples
```

## Features

### 🎯 Centralized Sample Runner
- **Interactive Menu**: Browse and run samples by category
- **Batch Execution**: Run all samples with a single command
- **Error Handling**: Graceful handling of missing dependencies
- **Progress Tracking**: Clear output and progress indication

### 📁 Organized Structure

The samples have been reorganized and cleaned up to provide clear, well-documented examples of the library's capabilities. Each sample includes proper XML documentation and follows consistent naming conventions.

## Sample Categories

### 1. Optimization Samples
Location: `GeometricAlgebraFulcrumLib.Optimization/Samples/`

- **GradientDescentSamples.cs**
  - `PerformanceComparisonExample()` - Compares CPU vs GPU SGD performance on large datasets

- **SvmSamples.cs**
  - `WineDatasetClassificationExample()` - Wine dataset classification with cross-validation
  - `SyntheticDataClassificationExample()` - Synthetic data classification with per-class analysis
  - `CartesianGeneticProgrammingExample()` - Function approximation using CGP

### 2. Index Set Samples  
Location: `GeometricAlgebraFulcrumLib.Utilities.Structures/Samples/IndexSets/`

- **BasicSamples.cs**
  - `CreationExample()` - Different ways to create IndexSet instances
  - `IteratorExample1()` - IndexSet iteration using spans
  - `IteratorExample2()` - Performance comparison between configurations
  - `TryRemoveExample()` - Demonstrates element removal operations
  - `OrderingExample()` - IndexSet ordering and sorting
  - `ContainsExample2()` - Set containment operations
  - `CountSwapsExample1()` - Swap counting algorithms

### 3. GAPoT Framework Samples
Location: `GAPoTNumLib.Framework/Samples/`

- **PowerSystemCalculationsSample.cs** (formerly Sample1.cs)
  - `Execute()` - Power system calculations using GAPoT (voltage, current, power, impedance)

- **MultivectorOperationsSample.cs** (formerly Sample2.cs)  
  - `Execute()` - Multivector operations and display formatting

- **Validation Samples**
  - `ValidationSample1.cs` - Various validation examples for GAPoT operations

### 4. Geometric Algebra Samples
Location: `GeometricAlgebraFulcrumLib.Algebra/Samples/`

Multiple specialized samples covering:
- Euclidean multivector operations
- Rotation and reflection operations
- Gram-Schmidt orthogonalization
- Eigenspace decomposition
- Storage implementations

### 5. Applications Samples
Location: `GeometricAlgebraFulcrumLib.Applications/`

- Power Systems samples (Clarke transformation, geometric frequency analysis)
- Symbolic computation samples
- Modeling and visualization samples

## Improvements Made

### Code Quality
- ✅ Removed commented-out code blocks
- ✅ Added comprehensive XML documentation to all public methods
- ✅ Improved method and class naming conventions
- ✅ Fixed code formatting and consistency
- ✅ Added meaningful output messages and formatting

### Organization
- ✅ Renamed poorly named samples (Sample1, Sample2, etc.) to descriptive names
- ✅ Grouped related samples together
- ✅ Added consistent error handling and logging
- ✅ Improved code readability and maintainability

### Documentation
- ✅ Added XML documentation for all sample methods
- ✅ Created this overview document
- ✅ Added inline comments for complex operations
- ✅ Improved console output formatting for better user experience

## Usage Guidelines

1. **Running Samples**: Each sample class contains static methods that can be called directly
2. **Dependencies**: Ensure all required NuGet packages are installed
3. **Data Files**: Some samples require external data files - check the file paths in the code
4. **Performance Testing**: Performance samples include timing measurements for benchmarking

## Contributing

When adding new samples:
1. Use descriptive class and method names
2. Add comprehensive XML documentation
3. Include proper error handling
4. Follow the established coding patterns
5. Add console output formatting for user-friendly results

## Next Steps

- [ ] Add more unit tests for sample methods
- [ ] Create interactive examples for key algorithms  
- [ ] Add performance benchmarks for all major operations
- [ ] Create visual examples using the graphics capabilities
- [ ] Extend the SamplesRunner with more sample categories
- [ ] Add sample execution timing and performance metrics
- [ ] Create automated sample testing in CI/CD pipeline

## Samples Runner

The `SamplesRunner` class provides a centralized way to discover and execute all samples:

### Categories Available:
1. **Optimization Samples** - ML algorithms, gradient descent, SVM
2. **Data Structure Samples** - IndexSet operations and performance tests  
3. **GAPoT Framework Samples** - Power system calculations and validations
4. **Geometric Algebra Samples** - Vector operations and transformations
5. **Symbolic Mathematics** - Mathematica integration examples
6. **Power Systems Applications** - Real-world power system analysis
7. **Performance Benchmarks** - Algorithm comparison and profiling

### Usage Examples:
```csharp
// Run specific category
SamplesRunner.RunOptimizationSamples();
SamplesRunner.RunDataStructureSamples();
SamplesRunner.RunGAPoTSamples();

// Interactive selection
SamplesRunner.RunInteractively();
```