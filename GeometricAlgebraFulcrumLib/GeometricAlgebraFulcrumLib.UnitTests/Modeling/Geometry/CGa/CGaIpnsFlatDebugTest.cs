using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Debug-Test um Line- und Plane-Encoding zu untersuchen
/// </summary>
[TestFixture]
public class CGaIpnsFlatDebugTest
{
    [Test]
    public void Debug_Line_2D_Structure()
    {
        // Arrange
        var float64Space = CGaFloat64GeometricSpace4D.Instance;
        var genericSpace = CGaGeometricSpace4D<double>.Create(ScalarProcessorOfFloat64.Instance);

        double distance = 2.0;
        double normalX = 1.0, normalY = 0.0;

        // Act
        var float64Blade = float64Space.Encode.IpnsFlat.Line(distance, normalX, normalY);
        var genericBlade = genericSpace.Encode.IpnsFlat.Line(distance, normalX, normalY);

        // Assert - Print structure
        Console.WriteLine("=== Float64 Line ===");
        var float64Vector = float64Blade.InternalVector;
        Console.WriteLine($"Grade: {float64Vector.Grade}");
        Console.WriteLine($"Count: {float64Vector.Count}");
        Console.WriteLine("IdScalarPairs:");
        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Console.WriteLine($"  IndexSet: {pair.Key}, Scalar: {pair.Value}");
        }

        Console.WriteLine("\n=== Generic Line ===");
        var genericVector = genericBlade.InternalVector;
        Console.WriteLine($"Grade: {genericVector.Grade}");
        Console.WriteLine($"Count: {genericVector.Count}");
        Console.WriteLine("IdScalarPairs:");
        foreach (var pair in genericVector.IdScalarPairs)
        {
            Console.WriteLine($"  IndexSet: {pair.Key}, Scalar: {pair.Value}");
        }
    }

    [Test]
    public void Debug_Plane_3D_Structure()
    {
        // Arrange
        var float64Space = CGaFloat64GeometricSpace5D.Instance;
        var genericSpace = CGaGeometricSpace5D<double>.Create(ScalarProcessorOfFloat64.Instance);

        double distance = 2.0;
        double nx = 0.0, ny = 0.0, nz = 1.0;

        // Act
        var float64Blade = float64Space.Encode.IpnsFlat.Plane(distance, nx, ny, nz);
        var genericBlade = genericSpace.Encode.IpnsFlat.Plane(distance, nx, ny, nz);

        // Assert - Print structure
        Console.WriteLine("=== Float64 Plane ===");
        var float64Vector = float64Blade.InternalVector;
        Console.WriteLine($"Grade: {float64Vector.Grade}");
        Console.WriteLine($"Count: {float64Vector.Count}");
        Console.WriteLine("IdScalarPairs:");
        foreach (var pair in float64Vector.IdScalarPairs)
        {
            Console.WriteLine($"  IndexSet: {pair.Key}, Scalar: {pair.Value}");
        }

        Console.WriteLine("\n=== Generic Plane ===");
        var genericVector = genericBlade.InternalVector;
        Console.WriteLine($"Grade: {genericVector.Grade}");
        Console.WriteLine($"Count: {genericVector.Count}");
        Console.WriteLine("IdScalarPairs:");
        foreach (var pair in genericVector.IdScalarPairs)
        {
            Console.WriteLine($"  IndexSet: {pair.Key}, Scalar: {pair.Value}");
        }
    }
}
