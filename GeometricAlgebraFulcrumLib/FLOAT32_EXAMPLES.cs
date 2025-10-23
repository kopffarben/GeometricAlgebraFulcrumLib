// Float32 Usage Examples
// These examples demonstrate common use cases for Float32 geometric algebra operations

using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float32;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float32;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.PGa.Float32;

namespace GeometricAlgebraFulcrumLib.Examples;

/// <summary>
/// Basic examples for XGa (Extended Geometric Algebra) with Float32
/// </summary>
public static class XGaFloat32Examples
{
    /// <summary>
    /// Example 1: Vector operations in 3D Euclidean space
    /// </summary>
    public static void Example1_BasicVectorOperations()
    {
        // Create a Euclidean processor
        var processor = XGaFloat32Processor.Euclidean;

        // Create two 3D vectors
        var v1 = processor.CreateVectorComposer()
            .SetVectorTerm(0, 1f)  // x component
            .SetVectorTerm(1, 0f)  // y component
            .SetVectorTerm(2, 0f)  // z component
            .GetVector();

        var v2 = processor.CreateVectorComposer()
            .SetVectorTerm(0, 0f)
            .SetVectorTerm(1, 1f)
            .SetVectorTerm(2, 0f)
            .GetVector();

        // Geometric product: v1 * v2
        var geometricProduct = v1.Gp(v2);
        // Result: scalar part (dot product) + bivector part (wedge product)
        // For orthogonal vectors: gp = bivector only

        // Outer product (wedge): v1 ∧ v2
        var outerProduct = v1.Op(v2);
        // Result: bivector representing the oriented plane containing v1 and v2

        // Scalar product (inner): v1 · v2
        var scalarProduct = v1.Sp(v2);
        // Result: 0 (orthogonal vectors)

        Console.WriteLine($"Geometric Product Grade: {geometricProduct.Grade}");  // Mixed grades
        Console.WriteLine($"Outer Product Grade: {outerProduct.Grade}");          // 2 (bivector)
        Console.WriteLine($"Scalar Product: {scalarProduct.ScalarValue}");        // 0
    }

    /// <summary>
    /// Example 2: Rotations using rotors in 3D
    /// </summary>
    public static void Example2_RotationsWithRotors()
    {
        var processor = XGaFloat32Processor.Euclidean;

        // Create a vector to rotate
        var vector = processor.CreateVectorComposer()
            .SetVectorTerm(0, 1f)
            .SetVectorTerm(1, 0f)
            .SetVectorTerm(2, 0f)
            .GetVector();

        // Create a target vector (45 degrees in xy-plane)
        var targetVector = processor.CreateVectorComposer()
            .SetVectorTerm(0, MathF.Cos(MathF.PI / 4f))
            .SetVectorTerm(1, MathF.Sin(MathF.PI / 4f))
            .SetVectorTerm(2, 0f)
            .GetVector();

        // Create a rotor that rotates from vector to targetVector
        var rotor = vector.CreatePureRotor(targetVector);

        // Apply rotation: R * v * R†
        var rotatedVector = rotor.OmMap(vector);

        // Verify rotation
        var dotProduct = rotatedVector.ESp(targetVector);  // Should be ≈ 1
        Console.WriteLine($"Rotation verification (dot product): {dotProduct}");
    }

    /// <summary>
    /// Example 3: Custom metric signatures (Minkowski spacetime)
    /// </summary>
    public static void Example3_MinkowskiSpacetime()
    {
        // Create Minkowski metric: (+, -, -, -) for spacetime
        // Convention: (time, x, y, z) with metric signature (1, 3, 0) = (p, q, r)
        var processor = XGaFloat32Processor.Create(negativeCount: 3, zeroCount: 0);

        // Create a spacetime event (ct, x, y, z)
        var event1 = processor.CreateVectorComposer()
            .SetVectorTerm(0, 5f)   // ct (time coordinate)
            .SetVectorTerm(1, 3f)   // x
            .SetVectorTerm(2, 0f)   // y
            .SetVectorTerm(3, 0f)   // z
            .GetVector();

        var event2 = processor.CreateVectorComposer()
            .SetVectorTerm(0, 5f)
            .SetVectorTerm(1, 4f)
            .SetVectorTerm(2, 0f)
            .SetVectorTerm(3, 0f)
            .GetVector();

        // Spacetime interval: (event1 - event2)²
        var displacement = event1.Subtract(event2);
        var interval = displacement.NormSquared();

        // interval > 0: timelike separation
        // interval = 0: lightlike separation
        // interval < 0: spacelike separation
        Console.WriteLine($"Spacetime interval²: {interval}");
    }
}

/// <summary>
/// Examples for CGA (Conformal Geometric Algebra) with Float32
/// </summary>
public static class CGaFloat32Examples
{
    /// <summary>
    /// Example 1: Encoding and manipulating 3D geometric objects
    /// </summary>
    public static void Example1_GeometricObjects3D()
    {
        var cga = CGaFloat32GeometricSpace.Space5D;  // 5D CGA for 3D geometry

        // Encode points
        var p1 = cga.EncodeIpnsRound.Point(0f, 0f, 0f);  // Origin
        var p2 = cga.EncodeIpnsRound.Point(1f, 0f, 0f);
        var p3 = cga.EncodeIpnsRound.Point(0f, 1f, 0f);

        // Encode sphere (center at origin, radius 5)
        var sphere = cga.EncodeIpnsRound.Sphere(0f, 0f, 0f, 5f);

        // Encode plane (normal = (0,0,1), distance from origin = 2)
        var plane = cga.EncodeOpns.Plane(0f, 0f, 1f, 2f);

        // Intersection: sphere ∧ plane = circle
        var circle = sphere.Op(plane);

        Console.WriteLine($"Sphere grade: {sphere.Grade}");
        Console.WriteLine($"Plane grade: {plane.Grade}");
        Console.WriteLine($"Circle grade: {circle.Grade}");
    }

    /// <summary>
    /// Example 2: Distance calculations in CGA
    /// </summary>
    public static void Example2_DistanceCalculations()
    {
        var cga = CGaFloat32GeometricSpace.Space5D;

        // Create two points
        var p1 = cga.EncodeIpnsRound.Point(0f, 0f, 0f);
        var p2 = cga.EncodeIpnsRound.Point(3f, 4f, 0f);

        // Distance formula in CGA: -2 * (p1 · p2)
        var distance = MathF.Sqrt(-2f * p1.Sp(p2).ScalarValue);

        Console.WriteLine($"Distance between points: {distance}");  // Should be 5
    }

    /// <summary>
    /// Example 3: Reflection and inversion (conformal transformations)
    /// </summary>
    public static void Example3_ConformalTransformations()
    {
        var cga = CGaFloat32GeometricSpace.Space5D;

        // Point to transform
        var point = cga.EncodeIpnsRound.Point(2f, 0f, 0f);

        // Reflection plane (yz-plane: normal = (1,0,0), distance = 0)
        var reflectionPlane = cga.EncodeOpns.Plane(1f, 0f, 0f, 0f);

        // Reflect point across plane: π * p * π†
        // (In CGA, reflection is: versor * object * reverse(versor))
        var reflectedPoint = reflectionPlane.Gp(point).Gp(reflectionPlane.Reverse());

        // Decode back to Euclidean coordinates
        // Result should be (-2, 0, 0)
        Console.WriteLine($"Reflected point: {reflectedPoint}");
    }

    /// <summary>
    /// Example 4: Hybrid API - mixing float and double
    /// </summary>
    public static void Example4_HybridAPI()
    {
        var cga = CGaFloat32GeometricSpace.Space4D;

        // Method 1: Native float (most efficient)
        var v1 = cga.EncodeVGa.Vector(1f, 2f);

        // Method 2: Double literals (automatically converted)
        var v2 = cga.EncodeVGa.Vector(1.0, 2.0);

        // Method 3: Explicit IScalar<float>
        var x = cga.ScalarProcessor.ScalarFromValue(1f);
        var y = cga.ScalarProcessor.ScalarFromValue(2f);
        var v3 = cga.EncodeVGa.Vector(x, y);

        // All three methods produce the same result
        var areEqual = v1.Subtract(v2).IsZero() && v2.Subtract(v3).IsZero();
        Console.WriteLine($"All methods produce identical results: {areEqual}");
    }

    /// <summary>
    /// Example 5: 2D geometry in 4D CGA
    /// </summary>
    public static void Example5_2DGeometry()
    {
        var cga = CGaFloat32GeometricSpace.Space4D;  // 4D CGA for 2D geometry

        // Encode 2D circle (center at (1,1), radius 2)
        var circle = cga.EncodeIpnsRound.Circle(1f, 1f, 2f);

        // Encode 2D line passing through (0,0) with direction (1,1)
        var line = cga.EncodeOpns.Line(0f, 0f, 1f, 1f);

        // Intersection: circle ∧ line = two points
        var intersectionPoints = circle.Op(line);

        Console.WriteLine($"Intersection points grade: {intersectionPoints.Grade}");
    }
}

/// <summary>
/// Examples for PGA (Projective Geometric Algebra) with Float32
/// </summary>
public static class PGaFloat32Examples
{
    /// <summary>
    /// Example 1: Basic PGA space creation
    /// </summary>
    public static void Example1_PGASpaceCreation()
    {
        // 4D PGA for 3D Euclidean geometry
        var pga = PGaFloat32GeometricSpace.Space4D;

        Console.WriteLine($"PGA Space Dimensions: {pga.VSpaceDimensions}");
        Console.WriteLine($"Is 3D Euclidean: {pga.Is3D}");
        Console.WriteLine($"Projective Processor: {pga.ProjectiveProcessor}");
    }

    /// <summary>
    /// Example 2: Working with homogeneous coordinates
    /// </summary>
    public static void Example2_HomogeneousCoordinates()
    {
        var pga = PGaFloat32GeometricSpace.Space4D;
        var processor = pga.ProjectiveProcessor;

        // Point in 3D space using homogeneous coordinates
        // Format: (x, y, z, w) where actual point = (x/w, y/w, z/w)
        var point1 = processor.CreateVectorComposer()
            .SetVectorTerm(0, 2f)   // x
            .SetVectorTerm(1, 4f)   // y
            .SetVectorTerm(2, 6f)   // z
            .SetVectorTerm(3, 2f)   // w (homogeneous coordinate)
            .GetVector();
        // This represents the point (1, 2, 3) in 3D

        // Point at infinity (direction vector)
        var directionAtInfinity = processor.CreateVectorComposer()
            .SetVectorTerm(0, 1f)
            .SetVectorTerm(1, 0f)
            .SetVectorTerm(2, 0f)
            .SetVectorTerm(3, 0f)   // w = 0 means point at infinity
            .GetVector();

        Console.WriteLine($"Point1 created: {point1}");
        Console.WriteLine($"Direction at infinity: {directionAtInfinity}");
    }

    /// <summary>
    /// Example 3: 2D geometry in 3D PGA (Space4D actually represents 3D projective, not 2D)
    /// Corrected to show 3D transformations
    /// </summary>
    public static void Example3_3DTransformations()
    {
        var pga = PGaFloat32GeometricSpace.Space4D;
        var processor = pga.ProjectiveProcessor;

        // Create a 3D point
        var point = processor.CreateVectorComposer()
            .SetVectorTerm(0, 1f)   // x
            .SetVectorTerm(1, 2f)   // y
            .SetVectorTerm(2, 3f)   // z
            .SetVectorTerm(3, 1f)   // w = 1 for finite point
            .GetVector();

        // Geometric operations in PGA
        var pointNorm = point.Norm();
        Console.WriteLine($"Point norm: {pointNorm}");
    }
}

/// <summary>
/// Performance comparison: Float32 vs Float64
/// </summary>
public static class Float32PerformanceExamples
{
    /// <summary>
    /// Demonstrates memory efficiency of Float32
    /// </summary>
    public static void ExampleMemoryUsage()
    {
        // Float32 version
        var processor32 = XGaFloat32Processor.Euclidean;
        var vectors32 = new List<XGaVector<float>>(1000);

        for (int i = 0; i < 1000; i++)
        {
            vectors32.Add(processor32.CreateVectorComposer()
                .SetVectorTerm(0, i * 1f)
                .SetVectorTerm(1, i * 2f)
                .SetVectorTerm(2, i * 3f)
                .GetVector());
        }

        // Float64 version (for comparison)
        var processor64 = XGaFloat64Processor.Euclidean;
        var vectors64 = new List<XGaVector<double>>(1000);

        for (int i = 0; i < 1000; i++)
        {
            vectors64.Add(processor64.CreateVectorComposer()
                .SetVectorTerm(0, i * 1.0)
                .SetVectorTerm(1, i * 2.0)
                .SetVectorTerm(2, i * 3.0)
                .GetVector());
        }

        // Float32 uses approximately 50% less memory for the scalar values
        // Actual memory savings depend on multivector structure overhead
        Console.WriteLine($"Float32 vectors created: {vectors32.Count}");
        Console.WriteLine($"Float64 vectors created: {vectors64.Count}");
    }

    /// <summary>
    /// Demonstrates when to use Float32 vs Float64
    /// </summary>
    public static void ExamplePrecisionTradeoffs()
    {
        var processor32 = XGaFloat32Processor.Euclidean;
        var processor64 = XGaFloat64Processor.Euclidean;

        // Small numbers: minimal precision difference
        var small32 = processor32.Scalar(0.1f);
        var small64 = processor64.Scalar(0.1);

        Console.WriteLine($"Float32: {small32.ScalarValue}");
        Console.WriteLine($"Float64: {small64.ScalarValue}");

        // Very large/small numbers: precision difference becomes significant
        var tiny32 = processor32.Scalar(1e-20f);  // May underflow
        var tiny64 = processor64.Scalar(1e-20);   // Precise

        Console.WriteLine($"Tiny Float32: {tiny32.ScalarValue}");
        Console.WriteLine($"Tiny Float64: {tiny64.ScalarValue}");
    }
}
