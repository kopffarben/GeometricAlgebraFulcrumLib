# Integration Guide

This document explains how to integrate GA-FuL with various platforms and development environments.

## Platform Integrations

### Unity 3D
```csharp
// Unity integration example
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using UnityEngine;

public class GAUnityBridge : MonoBehaviour
{
    private XGaProcessor<float> gaProcessor;
    
    void Start()
    {
        gaProcessor = XGaProcessor<float>.CreateEuclidean(ScalarProcessorOfFloat32.Instance);
    }
    
    // Convert Unity Vector3 to GA Vector
    public XGaVector<float> ToGAVector(Vector3 unityVector)
    {
        return gaProcessor.CreateVector(unityVector.x, unityVector.y, unityVector.z);
    }
    
    // Convert GA Vector to Unity Vector3
    public Vector3 ToUnityVector(XGaVector<float> gaVector)
    {
        return new Vector3(gaVector[0], gaVector[1], gaVector[2]);
    }
}
```

### MonoGame/XNA
```csharp
// MonoGame integration
using GeometricAlgebraFulcrumLib.MonoGame;
using Microsoft.Xna.Framework;

public class GAMonoGameRenderer
{
    private GraphicsDevice graphicsDevice;
    private XGaProcessor<float> gaProcessor;
    
    public void DrawGAObject(XGaMultivector<float> gaObject)
    {
        // Convert GA object to MonoGame primitives
        var meshes = GAToMeshConverter.Convert(gaObject);
        
        foreach (var mesh in meshes)
        {
            DrawMesh(mesh);
        }
    }
}
```

## Development Environment Integration

### Visual Studio
- **IntelliSense**: Full code completion for GA operations
- **Debugging**: Step through GA computations with visualizers
- **Testing**: Integrated unit testing with MSTest/NUnit/xUnit

### JetBrains Rider
- **Code Analysis**: Static analysis for GA expression optimization
- **Refactoring**: Safe refactoring of GA code
- **Performance Profiling**: Memory and CPU profiling for GA operations

## CI/CD Integration

### GitHub Actions
```yaml
name: GA-FuL Build and Test

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '7.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal
    
    - name: Benchmark
      run: dotnet run --project Benchmarks
```

### Docker Integration
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["GeometricAlgebraFulcrumLib.sln", "."]
COPY ["GeometricAlgebraFulcrumLib/", "GeometricAlgebraFulcrumLib/"]

RUN dotnet restore
RUN dotnet build -c Release

FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build /src/bin/Release/ .

ENTRYPOINT ["dotnet", "GeometricAlgebraFulcrumLib.dll"]
```

## Package Management

### NuGet Integration
```xml
<PackageReference Include="GeometricAlgebraFulcrumLib" Version="1.0.0" />
<PackageReference Include="GeometricAlgebraFulcrumLib.Mathematica" Version="1.0.0" />
<PackageReference Include="GeometricAlgebraFulcrumLib.Modeling" Version="1.0.0" />
```

### Dependency Management
```csharp
// Minimal dependency setup
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;

// Create lightweight GA environment
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var gaProcessor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);
```

## Web Integration

### Blazor WebAssembly
```csharp
@page "/ga-demo"
@using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors

<h3>GA-FuL Blazor Demo</h3>

<input @bind="vectorX" placeholder="X" />
<input @bind="vectorY" placeholder="Y" />
<input @bind="vectorZ" placeholder="Z" />
<button @onclick="ComputeGA">Compute</button>

<p>Result: @result</p>

@code {
    private double vectorX = 1.0;
    private double vectorY = 2.0;
    private double vectorZ = 3.0;
    private string result = "";
    
    private XGaProcessor<double> gaProcessor = 
        XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);
    
    private void ComputeGA()
    {
        var vector = gaProcessor.CreateVector(vectorX, vectorY, vectorZ);
        var norm = vector.Norm();
        result = $"Vector: {vector}, Magnitude: {norm.ScalarValue:F3}";
    }
}
```

### ASP.NET Core API
```csharp
[ApiController]
[Route("api/[controller]")]
public class GeometricAlgebraController : ControllerBase
{
    private readonly XGaProcessor<double> _gaProcessor;
    
    public GeometricAlgebraController()
    {
        _gaProcessor = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);
    }
    
    [HttpPost("multiply")]
    public IActionResult MultiplyVectors([FromBody] VectorMultiplyRequest request)
    {
        var v1 = _gaProcessor.CreateVector(request.Vector1.X, request.Vector1.Y, request.Vector1.Z);
        var v2 = _gaProcessor.CreateVector(request.Vector2.X, request.Vector2.Y, request.Vector2.Z);
        
        var result = v1.Gp(v2);
        
        return Ok(new { Result = result.ToString() });
    }
}
```

## Database Integration

### Entity Framework
```csharp
// Store GA results in database
public class GAComputationResult
{
    public int Id { get; set; }
    public string InputVector1 { get; set; }
    public string InputVector2 { get; set; }
    public string GeometricProduct { get; set; }
    public string OuterProduct { get; set; }
    public double ScalarProduct { get; set; }
    public DateTime ComputedAt { get; set; }
}

public class GADbContext : DbContext
{
    public DbSet<GAComputationResult> ComputationResults { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GAComputationResult>()
            .HasKey(e => e.Id);
    }
}
```

## External Tool Integration

### MATLAB Integration
```csharp
// Generate MATLAB code from GA expressions
var context = new MetaContext();
var processor = context.CreateEuclideanXGaProcessor();

var v1 = processor.CreateParameterVector("v1x", "v1y", "v1z");
var v2 = processor.CreateParameterVector("v2x", "v2y", "v2z");
var result = v1.Op(v2);

var matlabComposer = context.CreateMatlabCodeComposer();
string matlabCode = matlabComposer.Generate();

// Save to .m file for MATLAB use
File.WriteAllText("ga_operations.m", matlabCode);
```

### Python Integration
```csharp
// Generate Python/NumPy code
var pythonComposer = context.CreatePythonCodeComposer();
pythonComposer.ComposerOptions.UseNumpyArrays = true;

string pythonCode = pythonComposer.Generate();
File.WriteAllText("ga_operations.py", pythonCode);
```

## Performance Integration

### Profiling Tools
```csharp
// Integration with performance profilers
using GeometricAlgebraFulcrumLib.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net70)]
public class GAPerformanceBenchmark
{
    private XGaProcessor<double> processor;
    
    [GlobalSetup]
    public void Setup()
    {
        processor = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);
    }
    
    [Benchmark]
    public XGaMultivector<double> GeometricProduct()
    {
        var v1 = processor.CreateVector(1, 2, 3);
        var v2 = processor.CreateVector(4, 5, 6);
        return v1.Gp(v2);
    }
}
```

### GPU Integration (ILGPU)
```csharp
// GPU-accelerated GA computations
using ILGPU;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.ILGPU;

using var context = new Context();
using var accelerator = new CudaAccelerator(context);

var gpuProcessor = new XGaGpuProcessor<float>(accelerator);

// Process large arrays on GPU
var vectors1 = CreateLargeVectorArray(1000000);
var vectors2 = CreateLargeVectorArray(1000000);
var results = gpuProcessor.ComputeGeometricProductParallel(vectors1, vectors2);
```

---

**[← Previous: Applications](applications.md) | [Next: Performance →](performance.md)**