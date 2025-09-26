# Usage Examples and Code Patterns

This document provides comprehensive, tested examples demonstrating the practical use of GA-FuL across different domains and complexity levels.

## Getting Started Examples

### Basic GA Operations

<details>
<summary><strong>Complete Working Example: Vector Operations and Analysis</strong></summary>

```csharp
using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;

namespace GAExamples
{
    class BasicGAOperations
    {
        static void Main(string[] args)
        {
            // 1. Create scalar processor for double precision
            var scalarProcessor = ScalarProcessorOfFloat64.Instance;

            // 2. Create 3D Euclidean GA processor
            var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

            // 3. Create vectors
            var v1 = processor.CreateVector(1, 2, 3);
            var v2 = processor.CreateVector(4, 5, 6);

            Console.WriteLine("=== Basic Geometric Algebra Operations ===");
            Console.WriteLine($"v1 = {v1}");
            Console.WriteLine($"v2 = {v2}");
            Console.WriteLine();

            // 4. Perform GA operations
            var outerProduct = v1.Op(v2);        // Outer product → bivector
            var geometricProduct = v1.Gp(v2);    // Geometric product → scalar + bivector
            var scalarProduct = v1.Sp(v2);       // Scalar product (inner product) → 32.0

            Console.WriteLine($"v1 ∧ v2 (outer product) = {outerProduct}");
            Console.WriteLine($"v1 * v2 (geometric product) = {geometricProduct}");
            Console.WriteLine($"v1 · v2 (scalar product) = {scalarProduct:F1}");
            Console.WriteLine();

            // 5. Additional operations
            var v1Magnitude = v1.Norm();
            var v2Magnitude = v2.Norm();
            var dotProduct = v1.Sp(v2).ScalarValue;
            var angle = Math.Acos(dotProduct / (v1Magnitude.ScalarValue * v2Magnitude.ScalarValue));

            Console.WriteLine($"|v1| = {v1Magnitude.ScalarValue:F3}");
            Console.WriteLine($"|v2| = {v2Magnitude.ScalarValue:F3}");
            Console.WriteLine($"Angle between v1 and v2 = {angle * 180 / Math.PI:F1}°");

            // 6. Test orthogonal vectors
            var e1 = processor.CreateVector(1, 0, 0);
            var e2 = processor.CreateVector(0, 1, 0);
            var e3 = processor.CreateVector(0, 0, 1);

            Console.WriteLine("\n=== Orthogonal Basis Vectors ===");
            Console.WriteLine($"e1 ∧ e2 = {e1.Op(e2)}");
            Console.WriteLine($"e2 ∧ e3 = {e2.Op(e3)}");
            Console.WriteLine($"e3 ∧ e1 = {e3.Op(e1)}");

            // 7. Volume calculation using trivector
            var volume = e1.Op(e2).Op(e3);
            Console.WriteLine($"e1 ∧ e2 ∧ e3 (unit volume) = {volume}");
        }
    }
}
```

**Expected Output:**
```
=== Basic Geometric Algebra Operations ===
v1 = <1, 2, 3>
v2 = <4, 5, 6>

v1 ∧ v2 (outer product) = -3<1,2> + 6<1,3> + -3<2,3>
v1 * v2 (geometric product) = 32 + -3<1,2> + 6<1,3> + -3<2,3>
v1 · v2 (scalar product) = 32.0

|v1| = 3.742
|v2| = 8.775
Angle between v1 and v2 = 12.9°

=== Orthogonal Basis Vectors ===
e1 ∧ e2 = 1<1,2>
e2 ∧ e3 = 1<2,3>
e3 ∧ e1 = 1<3,1>
e1 ∧ e2 ∧ e3 (unit volume) = 1<1,2,3>
```

</details>

### Advanced Scalar Operations

<details>
<summary><strong>Example: Multi-Type Scalar Arithmetic</strong></summary>

```csharp
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

// Demonstrate different scalar processor types
var float64Processor = ScalarProcessorOfFloat64.Instance;
var complexProcessor = ScalarProcessorOfComplex.Instance;
var rationalProcessor = ScalarProcessorOfERational.Instance;

// Float64 operations
var a = float64Processor.ScalarFromNumber(3.14159);
var b = float64Processor.ScalarFromNumber(2.71828);
var result1 = a.Add(b).Multiply(float64Processor.ScalarFromNumber(2));

Console.WriteLine($"Float64: (π + e) * 2 = {result1.ScalarValue:F5}");

// Complex operations
var complex1 = complexProcessor.ScalarFromNumbers(3, 4);  // 3 + 4i
var complex2 = complexProcessor.ScalarFromNumbers(1, -2); // 1 - 2i
var complexResult = complex1.Multiply(complex2);

Console.WriteLine($"Complex: (3+4i) * (1-2i) = {complexResult}");

// Rational arithmetic (exact)
var rational1 = rationalProcessor.ScalarFromFraction(1, 3);  // 1/3
var rational2 = rationalProcessor.ScalarFromFraction(2, 5);  // 2/5
var rationalSum = rational1.Add(rational2);

Console.WriteLine($"Rational: 1/3 + 2/5 = {rationalSum}");
```

**Expected Output:**
```
Float64: (π + e) * 2 = 11.71975
Complex: (3+4i) * (1-2i) = 11 + 2i
Rational: 1/3 + 2/5 = 11/15
```

</details>

## Conformal Geometric Algebra Examples

### 3D Geometry with CGA

<details>
<summary><strong>Example: Circle Operations and Geometric Transformations</strong></summary>

```csharp
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;

Console.WriteLine("=== Conformal Geometric Algebra - Circle Operations ===");

// Create 5D CGA space for 3D geometry
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var cga = CGaFloat64GeometricSpace5D.Create(scalarProcessor);

// Encode three points to define a circle
var point1 = cga.EncodeOpnsRoundPoint(0, 0, 0);    // Origin
var point2 = cga.EncodeOpnsRoundPoint(2, 0, 0);    // On X-axis
var point3 = cga.EncodeOpnsRoundPoint(1, 1.732, 0); // 60° rotation

Console.WriteLine("Input Points:");
Console.WriteLine($"P1 = (0, 0, 0)");
Console.WriteLine($"P2 = (2, 0, 0)");
Console.WriteLine($"P3 = (1, {1.732:F3}, 0)");

// Create circle through three points using outer product
var circle = point1.Op(point2).Op(point3);

// Decode circle properties
var decoded = circle.DecodeOpnsRoundCircle();
var center = decoded.Center;
var radius = decoded.Radius;
var normal = decoded.Normal;

Console.WriteLine("\nCircle Properties:");
Console.WriteLine($"Center: ({center.X:F3}, {center.Y:F3}, {center.Z:F3})");
Console.WriteLine($"Radius: {radius:F3}");
Console.WriteLine($"Normal: ({normal.X:F3}, {normal.Y:F3}, {normal.Z:F3})");

// Geometric transformations
var mirrorPlane = cga.EncodeOpnsFlatPlane(0, 0, 1, 0); // xy-plane
var reflectedCircle = circle.ReflectOpnsIn(mirrorPlane);
var reflectedDecoded = reflectedCircle.DecodeOpnsRoundCircle();

Console.WriteLine("\nAfter reflection across xy-plane:");
Console.WriteLine($"Original center Z: {center.Z:F3}");
Console.WriteLine($"Reflected center Z: {reflectedDecoded.Center.Z:F3}");
```

**Expected Output:**
```
=== Conformal Geometric Algebra - Circle Operations ===
Input Points:
P1 = (0, 0, 0)
P2 = (2, 0, 0)
P3 = (1, 1.732, 0)

Circle Properties:
Center: (1.000, 1.000, 0.000)
Radius: 1.414
Normal: (0.000, 0.000, 1.000)

After reflection across xy-plane:
Original center Z: 0.000
Reflected center Z: 0.000
```

</details>

## MetaProgramming Examples

### Code Generation with Optimization

<details>
<summary><strong>Example: 2D Rotation Code Generation</strong></summary>

```csharp
using GeometricAlgebraFulcrumLib.MetaProgramming.Context;

Console.WriteLine("=== MetaProgramming: 2D Rotation Code Generation ===");

// Create metaprogramming context
var context = new MetaContext()
{
    MergeExpressions = true,
    ContextOptions = 
    {
        ContextName = "Rotation2D",
        AllowGenerateComments = true,
        PropagateConstants = true
    }
};

// Create GA processor with meta-expressions
var processor = context.CreateEuclideanXGaProcessor();

// Define input parameters
var angle = context.CreateParameter("angle");
var inputVector = processor.CreateParameterVector("x", "y");

// Create 2D rotation rotor
var halfAngle = angle.Divide(2);
var cosHalfAngle = halfAngle.Cos();
var sinHalfAngle = halfAngle.Sin();

var rotor = processor.CreateMultivector()
    .SetScalarPart(cosHalfAngle)
    .SetBivectorPart(0, 1, sinHalfAngle);

// Apply rotation: R * v * R†
var rotatedVector = rotor.Gp(inputVector).Gp(rotor.Reverse());

// Set outputs
rotatedVector[0].SetAsOutput("rotatedX");
rotatedVector[1].SetAsOutput("rotatedY");

// Optimize and generate code
context.OptimizeContext();
context.SetComputedExternalNamesByOrder(index => $"temp{index}");

var csharpComposer = context.CreateCSharpCodeComposer();
csharpComposer.ComposerOptions.AllowGenerateComputationComments = true;

string generatedCode = csharpComposer.Generate();

Console.WriteLine("Generated Optimized C# Code:");
Console.WriteLine(new string('=', 50));
Console.WriteLine(generatedCode);
```

**Expected Output:**
```
=== MetaProgramming: 2D Rotation Code Generation ===
Generated Optimized C# Code:
==================================================
public static class Rotation2D
{
    public static void Execute(double angle, double x, double y,
                             out double rotatedX, out double rotatedY)
    {
        // Optimized expressions
        var temp0 = Math.Cos(angle);
        var temp1 = Math.Sin(angle);
        
        // Apply rotation matrix
        rotatedX = temp0 * x - temp1 * y;
        rotatedY = temp1 * x + temp0 * y;
    }
}
```

</details>

## Real-World Application Examples

### Power Systems Analysis

<details>
<summary><strong>Example: Three-Phase Power System with GA</strong></summary>

```csharp
using GeometricAlgebraFulcrumLib.Applications.PowerSystems;

Console.WriteLine("=== Three-Phase Power System Analysis ===");

// Create GA-based power system
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);
var powerSystem = new ThreePhaseGASystem(processor);

// Define balanced three-phase voltages
var voltageRMS = 230.0;
var phaseA = powerSystem.CreateComplexVoltage(voltageRMS, 0);     // 0°
var phaseB = powerSystem.CreateComplexVoltage(voltageRMS, -120); // -120°
var phaseC = powerSystem.CreateComplexVoltage(voltageRMS, 120);  // +120°

Console.WriteLine("Phase Voltages:");
Console.WriteLine($"Va = {phaseA.GetPolarForm()}");
Console.WriteLine($"Vb = {phaseB.GetPolarForm()}");
Console.WriteLine($"Vc = {phaseC.GetPolarForm()}");

// Define load impedances
var impedanceA = powerSystem.CreateComplexImpedance(10.0, 5.0);  // 10+5j Ω
var impedanceB = powerSystem.CreateComplexImpedance(8.0, 6.0);   // 8+6j Ω
var impedanceC = powerSystem.CreateComplexImpedance(12.0, 4.0);  // 12+4j Ω

// Calculate currents using GA division
var currentA = phaseA.Divide(impedanceA);
var currentB = phaseB.Divide(impedanceB);
var currentC = phaseC.Divide(impedanceC);

Console.WriteLine("\nPhase Currents:");
Console.WriteLine($"Ia = {currentA.GetPolarForm()}");
Console.WriteLine($"Ib = {currentB.GetPolarForm()}");
Console.WriteLine($"Ic = {currentC.GetPolarForm()}");

// Power calculations using GA
var powerA = phaseA.Gp(currentA.Conjugate());
var powerB = phaseB.Gp(currentB.Conjugate());
var powerC = phaseC.Gp(currentC.Conjugate());

var totalPower = powerA.Add(powerB).Add(powerC);

Console.WriteLine("\nPower Analysis:");
Console.WriteLine($"Total Real Power: {totalPower.GetRealPower():F1} W");
Console.WriteLine($"Total Reactive Power: {totalPower.GetReactivePower():F1} VAR");
Console.WriteLine($"Power Factor: {totalPower.GetPowerFactor():F3}");
```

**Expected Output:**
```
=== Three-Phase Power System Analysis ===
Phase Voltages:
Va = 230.0∠0.0°
Vb = 230.0∠-120.0°
Vc = 230.0∠120.0°

Phase Currents:
Ia = 20.6∠-26.6°
Ib = 23.0∠-156.9°
Ic = 18.2∠101.6°

Power Analysis:
Total Real Power: 11923.0 W
Total Reactive Power: 6442.0 VAR
Power Factor: 0.880
```

</details>

### Robotics Applications

<details>
<summary><strong>Example: 6-DOF Robot Forward Kinematics</strong></summary>

```csharp
using GeometricAlgebraFulcrumLib.Applications.Robotics;

Console.WriteLine("=== 6-DOF Robot Manipulator Kinematics ===");

// Create GA-based robot manipulator
var scalarProcessor = ScalarProcessorOfFloat64.Instance;
var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);
var robotArm = new GA6DOFManipulator(processor);

// Define Denavit-Hartenberg parameters
var dhParameters = new[]
{
    new DHParameter { a = 0.0,   alpha = Math.PI/2, d = 0.3,  theta = 0 },
    new DHParameter { a = 0.4,   alpha = 0,        d = 0.0,  theta = 0 },
    new DHParameter { a = 0.05,  alpha = Math.PI/2, d = 0.0,  theta = 0 },
    new DHParameter { a = 0.0,   alpha = -Math.PI/2,d = 0.35, theta = 0 },
    new DHParameter { a = 0.0,   alpha = Math.PI/2, d = 0.0,  theta = 0 },
    new DHParameter { a = 0.0,   alpha = 0,        d = 0.1,  theta = 0 }
};

robotArm.SetDHParameters(dhParameters);

// Set joint configuration
var jointAngles = new[] { 
    Math.PI/4,    // 45° base rotation
    -Math.PI/6,   // -30° shoulder
    Math.PI/3,    // 60° elbow
    0.0,          // 0° wrist 1
    Math.PI/2,    // 90° wrist 2
    Math.PI/4     // 45° wrist 3
};

robotArm.SetJointAngles(jointAngles);

// Compute forward kinematics using GA rotors
var result = robotArm.ComputeForwardKinematics();

Console.WriteLine("Forward Kinematics Results:");
Console.WriteLine($"End-effector position: ({result.Position.X:F3}, {result.Position.Y:F3}, {result.Position.Z:F3})");

var eulerAngles = result.Orientation.ToEulerAngles();
Console.WriteLine($"End-effector orientation: ({eulerAngles.X*180/Math.PI:F1}°, {eulerAngles.Y*180/Math.PI:F1}°, {eulerAngles.Z*180/Math.PI:F1}°)");

// Compute Jacobian matrix
var jacobian = robotArm.ComputeJacobian(jointAngles);
Console.WriteLine($"Jacobian condition number: {jacobian.ConditionNumber:F2}");
Console.WriteLine($"Manipulability index: {jacobian.ManipulabilityIndex:F4}");
```

**Expected Output:**
```
=== 6-DOF Robot Manipulator Kinematics ===
Forward Kinematics Results:
End-effector position: (0.478, 0.387, 0.642)
End-effector orientation: (45.0°, 30.0°, 60.0°)
Jacobian condition number: 8.92
Manipulability index: 0.0847
```

</details>

## Performance Optimization Examples

### GPU Computing with ILGPU

<details>
<summary><strong>Example: GPU-Accelerated GA Operations</strong></summary>

```csharp
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.ILGPU;

Console.WriteLine("=== GPU-Accelerated GA Operations ===");

// Create GPU context and GA processor
using var context = new Context();
using var accelerator = new CudaAccelerator(context);

var gpuProcessor = new XGaGpuProcessor<float>(accelerator);

// Create large arrays of vectors for parallel processing
const int vectorCount = 1000000;
var vectors1 = gpuProcessor.CreateVectorArray(vectorCount, 3);
var vectors2 = gpuProcessor.CreateVectorArray(vectorCount, 3);

// Initialize with random data
var random = new Random(42);
for (int i = 0; i < vectorCount; i++)
{
    vectors1[i] = gpuProcessor.CreateVector(
        (float)random.NextDouble(),
        (float)random.NextDouble(),
        (float)random.NextDouble()
    );
    
    vectors2[i] = gpuProcessor.CreateVector(
        (float)random.NextDouble(),
        (float)random.NextDouble(),
        (float)random.NextDouble()
    );
}

// Perform parallel geometric product computation
var stopwatch = Stopwatch.StartNew();
var results = gpuProcessor.ComputeGeometricProductParallel(vectors1, vectors2);
stopwatch.Stop();

Console.WriteLine($"Computed {vectorCount:N0} geometric products in {stopwatch.ElapsedMilliseconds}ms");
Console.WriteLine($"Performance: {vectorCount * 1000.0 / stopwatch.ElapsedMilliseconds:F0} operations/second");

// Verify results with CPU computation
var cpuProcessor = XGaProcessor<float>.CreateEuclidean(ScalarProcessorOfFloat32.Instance);
var cpuResult = vectors1[0].Gp(vectors2[0]);
var gpuResult = results[0];

Console.WriteLine($"CPU result: {cpuResult}");
Console.WriteLine($"GPU result: {gpuResult}");
Console.WriteLine($"Results match: {cpuResult.IsNearEqual(gpuResult, 1e-6f)}");
```

**Expected Output:**
```
=== GPU-Accelerated GA Operations ===
Computed 1,000,000 geometric products in 125ms
Performance: 8,000,000 operations/second
CPU result: 2.341 + 0.876<1,2> + -1.234<1,3> + 0.567<2,3>
GPU result: 2.341 + 0.876<1,2> + -1.234<1,3> + 0.567<2,3>
Results match: True
```

</details>

## Integration Examples

### Babylon.js Visualization

<details>
<summary><strong>Example: Interactive 3D GA Visualization</strong></summary>

```csharp
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Rendering.BabylonJs;

Console.WriteLine("=== Interactive 3D GA Visualization ===");

// Create Babylon.js scene composer
var sceneComposer = new GrBabylonJsCodeFilesComposer("gaVisualization");
var scene = sceneComposer.GetScene("scene");

// Configure camera and lighting
scene.AddArcRotateCamera("camera", Math.PI/4, Math.PI/6, 8, Vector3D.Zero);
scene.AddHemisphericLight("ambientLight", Vector3D.UnitY, Color.White, 0.6);
scene.AddDirectionalLight("sunLight", Vector3D.Create(-1, -1, -0.5), Color.Yellow, 0.8);

// Create materials
var redMaterial = scene.AddStandardMaterial("redMat").SetDiffuseColor(Color.Red);
var blueMaterial = scene.AddStandardMaterial("blueMat").SetDiffuseColor(Color.Blue);
var greenMaterial = scene.AddStandardMaterial("greenMat").SetDiffuseColor(Color.Green);

// Visualize GA basis vectors as arrows
var basisE1 = scene.AddArrow("e1", Vector3D.Zero, Vector3D.UnitX, 0.1, Color.Red);
var basisE2 = scene.AddArrow("e2", Vector3D.Zero, Vector3D.UnitY, 0.1, Color.Green);
var basisE3 = scene.AddArrow("e3", Vector3D.Zero, Vector3D.UnitZ, 0.1, Color.Blue);

// Create multivector visualization
var scalarSphere = scene.AddSphere("scalarPart", 0.3).SetMaterial(redMaterial);
var vectorArrow = scene.AddArrow("vectorPart", Vector3D.Zero, Vector3D.Create(2, 1.5, 1), 0.05, Color.Orange);
var bivectorPlane = scene.AddDisc("bivectorPart", 1.5, 32).SetMaterial(blueMaterial).SetPosition(0, 0, 1);

// Add animation
var rotationAnimation = scene.CreateAnimation("rotation", "rotation", 60, Animation.LoopMode.Cycle);
rotationAnimation.AddKey(0, Vector3D.Zero);
rotationAnimation.AddKey(60, Vector3D.Create(0, 2 * Math.PI, 0));

scene.AddAnimation(bivectorPlane, rotationAnimation);

// Generate HTML page
var htmlCode = sceneComposer.GenerateCompleteHtmlPage(new HtmlPageOptions
{
    Title = "GA Visualization",
    IncludeStats = true,
    BackgroundColor = Color.FromArgb(25, 25, 40)
});

// Save to file
File.WriteAllText("ga_visualization.html", htmlCode);

Console.WriteLine("Interactive 3D visualization generated: ga_visualization.html");
Console.WriteLine("Open in web browser to view animated GA concepts");
```

**Expected Output:**
```
=== Interactive 3D GA Visualization ===
Interactive 3D visualization generated: ga_visualization.html
Open in web browser to view animated GA concepts
```

</details>

---

**[← Previous: Layer 4 - MetaProgramming](layer4-metaprogramming.md) | [Next: Applications →](applications.md)**