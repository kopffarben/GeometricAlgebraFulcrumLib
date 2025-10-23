using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Debug test to isolate the Trivector encoding bug
/// </summary>
[TestFixture]
public class CGaTrivectorDebugTest
{
    [Test]
    public void Debug_Float64_Ie_Structure()
    {
        // Arrange
        var space = CGaFloat64GeometricSpace5D.Instance;

        // Act - Check Ie pseudoscalar
        var ie = space.Ie;
        var ieKVector = ie.InternalKVector;

        // Assert - Print structure
        Console.WriteLine($"Float64 Ie Grade: {ieKVector.Grade}");
        Console.WriteLine($"Float64 Ie Count: {ieKVector.Count}");
        Console.WriteLine($"Float64 Ie IdScalarPairs:");
        foreach (var pair in ieKVector.IdScalarPairs)
        {
            Console.WriteLine($"  IndexSet: {pair.Key}, Scalar: {pair.Value}");
        }
    }

    [Test]
    public void Debug_Generic_Ie_Structure()
    {
        // Arrange
        var space = CGaGeometricSpace5D<double>.Create(ScalarProcessorOfFloat64.Instance);

        // Act - Check Ie pseudoscalar
        var ie = space.Ie;
        var ieKVector = ie.InternalKVector;

        // Assert - Print structure
        Console.WriteLine($"Generic Ie Grade: {ieKVector.Grade}");
        Console.WriteLine($"Generic Ie Count: {ieKVector.Count}");
        Console.WriteLine($"Generic Ie IdScalarPairs:");
        foreach (var pair in ieKVector.IdScalarPairs)
        {
            Console.WriteLine($"  IndexSet: {pair.Key}, Scalar: {pair.Value}");
        }
    }

    [Test]
    public void Debug_Float64_Trivector_Encoding()
    {
        // Arrange
        var space = CGaFloat64GeometricSpace5D.Instance;
        double xyz = 2.5;

        // Act
        var blade = space.Encode.VGa.Trivector(xyz);
        var kVector = blade.InternalKVector;

        // Assert
        Console.WriteLine($"Float64 Trivector Grade: {kVector.Grade}");
        Console.WriteLine($"Float64 Trivector Count: {kVector.Count}");
        Console.WriteLine($"Float64 Trivector IdScalarPairs:");
        foreach (var pair in kVector.IdScalarPairs)
        {
            Console.WriteLine($"  IndexSet: {pair.Key}, Scalar: {pair.Value}");
        }
    }

    [Test]
    public void Debug_Generic_Trivector_Encoding()
    {
        // Arrange
        var space = CGaGeometricSpace5D<double>.Create(ScalarProcessorOfFloat64.Instance);
        double xyz = 2.5;

        // Act
        var scalarProcessor = space.ScalarProcessor;
        var blade = space.Encode.VGa.Trivector(scalarProcessor.ScalarFromValue(xyz));
        var kVector = blade.InternalKVector;

        // Assert
        Console.WriteLine($"Generic Trivector Grade: {kVector.Grade}");
        Console.WriteLine($"Generic Trivector Count: {kVector.Count}");
        Console.WriteLine($"Generic Trivector IdScalarPairs:");
        foreach (var pair in kVector.IdScalarPairs)
        {
            Console.WriteLine($"  IndexSet: {pair.Key}, Scalar: {pair.Value}");
        }
    }

    [Test]
    public void Debug_Scalar_Multiplication()
    {
        // Arrange
        var space = CGaGeometricSpace5D<double>.Create(ScalarProcessorOfFloat64.Instance);
        double xyz = 2.5;
        var scalarProcessor = space.ScalarProcessor;
        var xyzScalar = scalarProcessor.ScalarFromValue(xyz);

        // Act - Directly multiply Ie by scalar
        var ie = space.Ie.InternalKVector;
        var result = ie.Times(xyzScalar);

        // Assert
        Console.WriteLine($"Direct multiplication result Grade: {result.Grade}");
        Console.WriteLine($"Direct multiplication result Count: {result.Count}");
        Console.WriteLine($"Direct multiplication IdScalarPairs:");
        foreach (var pair in result.IdScalarPairs)
        {
            Console.WriteLine($"  IndexSet: {pair.Key}, Scalar: {pair.Value}");
        }
    }
}
