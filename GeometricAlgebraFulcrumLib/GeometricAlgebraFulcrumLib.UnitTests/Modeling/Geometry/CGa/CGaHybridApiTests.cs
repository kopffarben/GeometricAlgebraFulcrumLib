using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Geometry.CGa;

/// <summary>
/// Integration tests for Hybrid API (T, double, Scalar&lt;T&gt;, IScalar&lt;T&gt; overloads)
/// Phase 2 - Validates that all scalar type overloads work correctly
/// </summary>
[TestFixture]
public class CGaHybridApiTests
{
    private CGaGeometricSpace<double> _space4D = null!;
    private CGaGeometricSpace<double> _space5D = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _space4D = CGaGeometricSpace<double>.Create4D(ScalarProcessorOfFloat64.Instance);
        _space5D = CGaGeometricSpace<double>.Create5D(ScalarProcessorOfFloat64.Instance);
    }

    #region IpnsRound Encoder Tests

    [Test]
    public void IpnsRound_Point2D_DoubleOverload_Works()
    {
        // Arrange
        double x = 1.5, y = 2.5;

        // Act
        var point = _space4D.EncodeIpnsRound.Point(x, y);

        // Assert
        Assert.That(point, Is.Not.Null, "Point should not be null");
        Assert.That(point.Norm().ScalarValue, Is.GreaterThan(0.0), "Point should have positive norm");
    }

    [Test]
    public void IpnsRound_Point2D_TOverload_Works()
    {
        // Arrange
        double x = 1.5, y = 2.5;

        // Act
        var point = _space4D.EncodeIpnsRound.Point(x, y);  // T is double in this generic context

        // Assert
        Assert.That(point, Is.Not.Null, "Point should not be null");
        Assert.That(point.Norm().ScalarValue, Is.GreaterThan(0.0), "Point should have positive norm");
    }

    [Test]
    public void IpnsRound_Point2D_IScalarOverload_Works()
    {
        // Arrange
        var x = _space4D.ScalarProcessor.ScalarFromNumber(1.5);
        var y = _space4D.ScalarProcessor.ScalarFromNumber(2.5);

        // Act
        var point = _space4D.EncodeIpnsRound.Point(x, y);

        // Assert
        Assert.That(point, Is.Not.Null, "Point should not be null");
        Assert.That(point.Norm().ScalarValue, Is.GreaterThan(0.0), "Point should have positive norm");
    }

    [Test]
    public void IpnsRound_Point3D_DoubleOverload_Works()
    {
        // Arrange
        double x = 1.0, y = 2.0, z = 3.0;

        // Act
        var point = _space5D.EncodeIpnsRound.Point(x, y, z);

        // Assert
        Assert.That(point, Is.Not.Null, "Point should not be null");
        Assert.That(point.Norm().ScalarValue, Is.GreaterThan(0.0), "Point should have positive norm");
    }

    [Test]
    public void IpnsRound_Point3D_IScalarOverload_Works()
    {
        // Arrange
        var x = _space5D.ScalarProcessor.ScalarFromNumber(1.0);
        var y = _space5D.ScalarProcessor.ScalarFromNumber(2.0);
        var z = _space5D.ScalarProcessor.ScalarFromNumber(3.0);

        // Act
        var point = _space5D.EncodeIpnsRound.Point(x, y, z);

        // Assert
        Assert.That(point, Is.Not.Null, "Point should not be null");
        Assert.That(point.Norm().ScalarValue, Is.GreaterThan(0.0), "Point should have positive norm");
    }

    [Test]
    public void IpnsRound_Circle_DoubleOverload_Works()
    {
        // Arrange
        double radiusSquared = 4.0, cx = 0.0, cy = 0.0;

        // Act
        var circle = _space4D.EncodeIpnsRound.Circle(radiusSquared, cx, cy);

        // Assert
        Assert.That(circle, Is.Not.Null, "Circle should not be null");
        Assert.That(circle.Norm().ScalarValue, Is.GreaterThan(0.0), "Circle should have positive norm");
    }

    [Test]
    public void IpnsRound_Circle_IScalarOverload_Works()
    {
        // Arrange
        var radiusSquared = _space4D.ScalarProcessor.ScalarFromNumber(4.0);
        var cx = _space4D.ScalarProcessor.ScalarFromNumber(0.0);
        var cy = _space4D.ScalarProcessor.ScalarFromNumber(0.0);

        // Act
        var circle = _space4D.EncodeIpnsRound.Circle(radiusSquared, cx, cy);

        // Assert
        Assert.That(circle, Is.Not.Null, "Circle should not be null");
        Assert.That(circle.Norm().ScalarValue, Is.GreaterThan(0.0), "Circle should have positive norm");
    }

    #endregion

    #region IpnsFlat Encoder Tests

    [Test]
    public void IpnsFlat_Point3D_DoubleOverload_Works()
    {
        // Arrange
        double x = 1.0, y = 0.0, z = -1.0;

        // Act
        var point = _space5D.EncodeIpnsFlat.Point(x, y, z);

        // Assert
        Assert.That(point, Is.Not.Null, "Point should not be null");
        Assert.That(point.Norm().ScalarValue, Is.GreaterThan(0.0), "Point should have positive norm");
    }

    [Test]
    public void IpnsFlat_Line2D_DoubleOverload_Works()
    {
        // Arrange
        double distance = 1.0, normalX = 1.0, normalY = 0.0;

        // Act
        var line = _space4D.EncodeIpnsFlat.Line(distance, normalX, normalY);

        // Assert
        Assert.That(line, Is.Not.Null, "Line should not be null");
        Assert.That(line.Norm().ScalarValue, Is.GreaterThan(0.0), "Line should have positive norm");
    }

    [Test]
    public void IpnsFlat_Plane3D_DoubleOverload_Works()
    {
        // Arrange
        double distance = 1.0, nx = 0.0, ny = 0.0, nz = 1.0;

        // Act
        var plane = _space5D.EncodeIpnsFlat.Plane(distance, nx, ny, nz);

        // Assert
        Assert.That(plane, Is.Not.Null, "Plane should not be null");
        Assert.That(plane.Norm().ScalarValue, Is.GreaterThan(0.0), "Plane should have positive norm");
    }

    #endregion

    #region OpnsRound Encoder Tests

    [Test]
    public void OpnsRound_Point3D_DoubleOverload_Works()
    {
        // Arrange
        double x = 1.0, y = 2.0, z = 3.0;

        // Act
        var point = _space5D.EncodeOpnsRound.Point(x, y, z);

        // Assert
        Assert.That(point, Is.Not.Null, "Point should not be null");
        Assert.That(point.Norm().ScalarValue, Is.GreaterThan(0.0), "Point should have positive norm");
    }

    #endregion

    #region OpnsFlat Encoder Tests

    [Test]
    public void OpnsFlat_Point3D_DoubleOverload_Works()
    {
        // Arrange
        double x = -1.0, y = 2.5, z = 3.5;

        // Act
        var point = _space5D.EncodeOpnsFlat.Point(x, y, z);

        // Assert
        Assert.That(point, Is.Not.Null, "Point should not be null");
        Assert.That(point.Norm().ScalarValue, Is.GreaterThan(0.0), "Point should have positive norm");
    }

    #endregion

    #region IpnsTangent Encoder Tests

    [Test]
    public void IpnsTangent_Point3D_DoubleOverload_Works()
    {
        // Arrange
        double x = 0.5, y = 1.5, z = 2.5;

        // Act
        var point = _space5D.EncodeIpnsTangent.Point(x, y, z);

        // Assert
        Assert.That(point, Is.Not.Null, "Point should not be null");
        Assert.That(point.Norm().ScalarValue, Is.GreaterThan(0.0), "Point should have positive norm");
    }

    [Test]
    public void IpnsTangent_Line2D_DoubleOverload_Works()
    {
        // Arrange
        double distance = 1.0, nx = 1.0, ny = 0.0;

        // Act
        var line = _space4D.EncodeIpnsTangent.Line(distance, nx, ny);

        // Assert
        Assert.That(line, Is.Not.Null, "Line should not be null");
        Assert.That(line.Norm().ScalarValue, Is.GreaterThan(0.0), "Line should have positive norm");
    }

    [Test]
    public void IpnsTangent_Plane3D_DoubleOverload_Works()
    {
        // Arrange
        double distance = 2.0, nx = 0.0, ny = 0.0, nz = 1.0;

        // Act
        var plane = _space5D.EncodeIpnsTangent.Plane(distance, nx, ny, nz);

        // Assert
        Assert.That(plane, Is.Not.Null, "Plane should not be null");
        Assert.That(plane.Norm().ScalarValue, Is.GreaterThan(0.0), "Plane should have positive norm");
    }

    #endregion

    #region OpnsTangent Encoder Tests

    [Test]
    public void OpnsTangent_Point3D_DoubleOverload_Works()
    {
        // Arrange
        double x = 1.0, y = 0.0, z = 1.0;

        // Act
        var point = _space5D.EncodeOpnsTangent.Point(x, y, z);

        // Assert
        Assert.That(point, Is.Not.Null, "Point should not be null");
        Assert.That(point.Norm().ScalarValue, Is.GreaterThan(0.0), "Point should have positive norm");
    }

    [Test]
    public void OpnsTangent_Line2D_DoubleOverload_Works()
    {
        // Arrange
        double distance = 0.5, nx = 0.0, ny = 1.0;

        // Act
        var line = _space4D.EncodeOpnsTangent.Line(distance, nx, ny);

        // Assert
        Assert.That(line, Is.Not.Null, "Line should not be null");
        Assert.That(line.Norm().ScalarValue, Is.GreaterThan(0.0), "Line should have positive norm");
    }

    [Test]
    public void OpnsTangent_Plane3D_DoubleOverload_Works()
    {
        // Arrange
        double distance = 1.5, nx = 1.0, ny = 0.0, nz = 0.0;

        // Act
        var plane = _space5D.EncodeOpnsTangent.Plane(distance, nx, ny, nz);

        // Assert
        Assert.That(plane, Is.Not.Null, "Plane should not be null");
        Assert.That(plane.Norm().ScalarValue, Is.GreaterThan(0.0), "Plane should have positive norm");
    }

    #endregion

    #region PGa Encoder Tests

    [Test]
    public void PGa_Point3D_DoubleOverload_Works()
    {
        // Arrange
        double x = 2.0, y = 3.0, z = 4.0;

        // Act
        var point = _space5D.EncodePGa.Point(x, y, z);

        // Assert
        Assert.That(point, Is.Not.Null, "Point should not be null");
        Assert.That(point.Norm().ScalarValue, Is.GreaterThan(0.0), "Point should have positive norm");
    }

    #endregion

    #region Main Encoder Translation Tests

    [Test]
    public void MainEncoder_Translation3D_DoubleOverload_Works()
    {
        // Arrange
        double vx = 3.0, vy = 4.0, vz = 5.0;

        // Act
        var translation = _space5D.Encode.Translation(vx, vy, vz);

        // Assert
        Assert.That(translation, Is.Not.Null, "Translation should not be null");
        Assert.That(translation.Norm().ScalarValue, Is.GreaterThan(0.0), "Translation should have positive norm");
    }

    [Test]
    public void MainEncoder_Translation3D_LinVector3DOverload_Works()
    {
        // Arrange
        var vector = LinVector3D<double>.Create(
            _space5D.ScalarProcessor.ScalarFromNumber(3.0),
            _space5D.ScalarProcessor.ScalarFromNumber(4.0),
            _space5D.ScalarProcessor.ScalarFromNumber(5.0)
        );

        // Act
        var translation = _space5D.Encode.Translation(vector);

        // Assert
        Assert.That(translation, Is.Not.Null, "Translation should not be null");
        Assert.That(translation.Norm().ScalarValue, Is.GreaterThan(0.0), "Translation should have positive norm");
    }

    #endregion
}
