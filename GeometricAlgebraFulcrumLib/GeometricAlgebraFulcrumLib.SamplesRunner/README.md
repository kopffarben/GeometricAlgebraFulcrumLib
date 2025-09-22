# GeometricAlgebraFulcrumLib Samples Runner

A console application that provides an organized way to discover and execute various sample demonstrations from the GeometricAlgebraFulcrumLib project.

## Features

- **Interactive Menu**: Browse and run samples by category
- **Batch Execution**: Run all samples with a single command
- **Error Handling**: Graceful handling of missing dependencies
- **Progress Tracking**: Clear output and progress indication

## Usage

### Interactive Mode (Default)
Run the application without any arguments to access the interactive menu:

```bash
dotnet run --project GeometricAlgebraFulcrumLib.SamplesRunner
```

### Batch Mode
Run all samples automatically:

```bash
dotnet run --project GeometricAlgebraFulcrumLib.SamplesRunner -- --all
```

## Available Sample Categories

1. **Optimization Samples**
   - Gradient Descent (CPU vs GPU performance)
   - Support Vector Machines (Classification)
   - Cartesian Genetic Programming

2. **Data Structure Samples**
   - IndexSet operations and performance
   - Set manipulation and iteration

3. **GAPoT Framework Samples** *(Currently unavailable due to .NET Framework compatibility)*
   - Power system calculations
   - Multivector operations
   - Validation examples

4. **Geometric Algebra Samples** *(Placeholder for future implementation)*

5. **Symbolic Mathematics Samples** *(Placeholder for future implementation)*

6. **Power Systems Applications** *(Placeholder for future implementation)*

7. **Performance Benchmarks** *(Placeholder for future implementation)*

## Dependencies

The console application references:
- `GeometricAlgebraFulcrumLib.Optimization`
- `GeometricAlgebraFulcrumLib.Utilities.Structures`

## Note on GAPoT Samples

The GAPoT Framework samples are currently unavailable because they depend on the `GAPoTNumLib.Framework` project which targets .NET Framework 4.7.2. This console application targets .NET 8.0 and cannot directly reference .NET Framework projects.

To use GAPoT samples, you would need to either:
- Upgrade the GAPoTNumLib.Framework project to .NET 8.0
- Create a separate console application targeting .NET Framework
- Use a compatibility shim or wrapper

## Building

```bash
dotnet build GeometricAlgebraFulcrumLib.SamplesRunner
```

## Running

From the solution directory:
```bash
dotnet run --project GeometricAlgebraFulcrumLib.SamplesRunner
```

From the project directory:
```bash
cd GeometricAlgebraFulcrumLib.SamplesRunner
dotnet run
```