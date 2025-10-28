using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Bezier;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Bezier;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public class Bezier3Path3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [SetUp]
    public void Setup()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    private LinVector3D<double> CreateGenericVector(double x, double y, double z)
    {
        return LinVector3D<double>.Create(
            ScalarProcessor.Scalar(x),
            ScalarProcessor.Scalar(y),
            ScalarProcessor.Scalar(z)
        );
    }

    [Test]
    public void Bezier3Path3D_Constructor_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(0.0, 0.0, 0.0);
        var p2Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 0.0);
        var p3Float64 = LinFloat64Vector3D.Create(2.0, 2.0, 1.0);
        var p4Float64 = LinFloat64Vector3D.Create(3.0, 0.0, 1.0);

        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(1.0, 2.0, 0.0);
        var p3Generic = CreateGenericVector(2.0, 2.0, 1.0);
        var p4Generic = CreateGenericVector(3.0, 0.0, 1.0);

        // Act
        var pathFloat64 = new Float64Bezier3Path3D(false, p1Float64, p2Float64, p3Float64, p4Float64);
        var pathGeneric = Bezier3Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic, p4Generic);

        // Assert
        Assert.That(pathGeneric.Point1.X.ScalarValue, Is.EqualTo(pathFloat64.Point1.X.ScalarValue).Within(Tolerance));
        Assert.That(pathGeneric.Point2.Y.ScalarValue, Is.EqualTo(pathFloat64.Point2.Y.ScalarValue).Within(Tolerance));
        Assert.That(pathGeneric.Point3.Z.ScalarValue, Is.EqualTo(pathFloat64.Point3.Z.ScalarValue).Within(Tolerance));
        Assert.That(pathGeneric.Point4.X.ScalarValue, Is.EqualTo(pathFloat64.Point4.X.ScalarValue).Within(Tolerance));
        Assert.That(pathGeneric.IsPeriodic, Is.EqualTo(pathFloat64.IsPeriodic));
    }

    [Test]
    public void Bezier3Path3D_GetValue_AtStart_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var p2Float64 = LinFloat64Vector3D.Create(4.0, 5.0, 6.0);
        var p3Float64 = LinFloat64Vector3D.Create(7.0, 8.0, 9.0);
        var p4Float64 = LinFloat64Vector3D.Create(10.0, 11.0, 12.0);

        var p1Generic = CreateGenericVector(1.0, 2.0, 3.0);
        var p2Generic = CreateGenericVector(4.0, 5.0, 6.0);
        var p3Generic = CreateGenericVector(7.0, 8.0, 9.0);
        var p4Generic = CreateGenericVector(10.0, 11.0, 12.0);

        var pathFloat64 = new Float64Bezier3Path3D(false, p1Float64, p2Float64, p3Float64, p4Float64);
        var pathGeneric = Bezier3Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic, p4Generic);

        // Act
        var valueFloat64 = pathFloat64.GetValue(0.0);
        var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(0.0));

        // Assert - At t=0: B(0) = P₁
        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z.ScalarValue).Within(Tolerance));

        Assert.That(valueFloat64.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(valueFloat64.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance));
        Assert.That(valueFloat64.Z.ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
    }

    [Test]
    public void Bezier3Path3D_GetValue_AtEnd_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var p2Float64 = LinFloat64Vector3D.Create(4.0, 5.0, 6.0);
        var p3Float64 = LinFloat64Vector3D.Create(7.0, 8.0, 9.0);
        var p4Float64 = LinFloat64Vector3D.Create(10.0, 11.0, 12.0);

        var p1Generic = CreateGenericVector(1.0, 2.0, 3.0);
        var p2Generic = CreateGenericVector(4.0, 5.0, 6.0);
        var p3Generic = CreateGenericVector(7.0, 8.0, 9.0);
        var p4Generic = CreateGenericVector(10.0, 11.0, 12.0);

        var pathFloat64 = new Float64Bezier3Path3D(false, p1Float64, p2Float64, p3Float64, p4Float64);
        var pathGeneric = Bezier3Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic, p4Generic);

        // Act
        var valueFloat64 = pathFloat64.GetValue(1.0);
        var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(1.0));

        // Assert - At t=1: B(1) = P₄
        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z.ScalarValue).Within(Tolerance));

        Assert.That(valueFloat64.X.ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
        Assert.That(valueFloat64.Y.ScalarValue, Is.EqualTo(11.0).Within(Tolerance));
        Assert.That(valueFloat64.Z.ScalarValue, Is.EqualTo(12.0).Within(Tolerance));
    }

    [Test]
    public void Bezier3Path3D_GetValue_AtMidpoint_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(0.0, 0.0, 0.0);
        var p2Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 0.0);
        var p3Float64 = LinFloat64Vector3D.Create(2.0, 2.0, 1.0);
        var p4Float64 = LinFloat64Vector3D.Create(3.0, 0.0, 1.0);

        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(1.0, 2.0, 0.0);
        var p3Generic = CreateGenericVector(2.0, 2.0, 1.0);
        var p4Generic = CreateGenericVector(3.0, 0.0, 1.0);

        var pathFloat64 = new Float64Bezier3Path3D(false, p1Float64, p2Float64, p3Float64, p4Float64);
        var pathGeneric = Bezier3Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic, p4Generic);

        // Act
        var valueFloat64 = pathFloat64.GetValue(0.5);
        var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(0.5));

        // Assert - At t=0.5: B(0.5) = 0.125*P₁ + 0.375*P₂ + 0.375*P₃ + 0.125*P₄
        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void Bezier3Path3D_GetValue_AtVariousT_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(0.0, 0.0, 0.0);
        var p2Float64 = LinFloat64Vector3D.Create(1.0, 3.0, 1.0);
        var p3Float64 = LinFloat64Vector3D.Create(3.0, 3.0, 2.0);
        var p4Float64 = LinFloat64Vector3D.Create(4.0, 0.0, 3.0);

        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(1.0, 3.0, 1.0);
        var p3Generic = CreateGenericVector(3.0, 3.0, 2.0);
        var p4Generic = CreateGenericVector(4.0, 0.0, 3.0);

        var pathFloat64 = new Float64Bezier3Path3D(false, p1Float64, p2Float64, p3Float64, p4Float64);
        var pathGeneric = Bezier3Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic, p4Generic);

        // Test at multiple t values
        var tValues = new[] { 0.0, 0.25, 0.5, 0.75, 1.0 };

        foreach (var t in tValues)
        {
            var valueFloat64 = pathFloat64.GetValue(t);
            var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(t));

            Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance), $"X failed at t={t}");
            Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance), $"Y failed at t={t}");
            Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z.ScalarValue).Within(Tolerance), $"Z failed at t={t}");
        }
    }

    [Test]
    public void Bezier3Path3D_GetDerivative1Value_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(0.0, 0.0, 0.0);
        var p2Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 0.0);
        var p3Float64 = LinFloat64Vector3D.Create(2.0, 2.0, 1.0);
        var p4Float64 = LinFloat64Vector3D.Create(3.0, 0.0, 1.0);

        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(1.0, 2.0, 0.0);
        var p3Generic = CreateGenericVector(2.0, 2.0, 1.0);
        var p4Generic = CreateGenericVector(3.0, 0.0, 1.0);

        var pathFloat64 = new Float64Bezier3Path3D(false, p1Float64, p2Float64, p3Float64, p4Float64);
        var pathGeneric = Bezier3Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic, p4Generic);

        // Test at multiple t values
        var tValues = new[] { 0.0, 0.5, 1.0 };

        foreach (var t in tValues)
        {
            var deriv1Float64 = pathFloat64.GetDerivative1Value(t);
            var deriv1Generic = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(t));

            Assert.That(deriv1Generic.X.ScalarValue, Is.EqualTo(deriv1Float64.X.ScalarValue).Within(Tolerance), $"X failed at t={t}");
            Assert.That(deriv1Generic.Y.ScalarValue, Is.EqualTo(deriv1Float64.Y.ScalarValue).Within(Tolerance), $"Y failed at t={t}");
            Assert.That(deriv1Generic.Z.ScalarValue, Is.EqualTo(deriv1Float64.Z.ScalarValue).Within(Tolerance), $"Z failed at t={t}");
        }
    }

    [Test]
    public void Bezier3Path3D_GetDerivative1Value_AtStart_ShouldBe3TimesFirstSegment()
    {
        // Arrange
        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(1.0, 2.0, 3.0);
        var p3Generic = CreateGenericVector(4.0, 5.0, 6.0);
        var p4Generic = CreateGenericVector(7.0, 8.0, 9.0);

        var pathGeneric = Bezier3Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic, p4Generic);

        // Act
        var deriv1 = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(0.0));

        // Assert - At t=0: B'(0) = 3(P₂ - P₁)
        Assert.That(deriv1.X.ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
        Assert.That(deriv1.Y.ScalarValue, Is.EqualTo(6.0).Within(Tolerance));
        Assert.That(deriv1.Z.ScalarValue, Is.EqualTo(9.0).Within(Tolerance));
    }

    [Test]
    public void Bezier3Path3D_GetDerivative1Value_AtEnd_ShouldBe3TimesLastSegment()
    {
        // Arrange
        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(1.0, 2.0, 3.0);
        var p3Generic = CreateGenericVector(4.0, 5.0, 6.0);
        var p4Generic = CreateGenericVector(7.0, 9.0, 12.0);

        var pathGeneric = Bezier3Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic, p4Generic);

        // Act
        var deriv1 = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(1.0));

        // Assert - At t=1: B'(1) = 3(P₄ - P₃)
        Assert.That(deriv1.X.ScalarValue, Is.EqualTo(9.0).Within(Tolerance));  // 3 * (7-4)
        Assert.That(deriv1.Y.ScalarValue, Is.EqualTo(12.0).Within(Tolerance)); // 3 * (9-5)
        Assert.That(deriv1.Z.ScalarValue, Is.EqualTo(18.0).Within(Tolerance)); // 3 * (12-6)
    }

    [Test]
    public void Bezier3Path3D_GetDerivative2Value_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(0.0, 0.0, 0.0);
        var p2Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 0.0);
        var p3Float64 = LinFloat64Vector3D.Create(2.0, 2.0, 1.0);
        var p4Float64 = LinFloat64Vector3D.Create(3.0, 0.0, 1.0);

        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(1.0, 2.0, 0.0);
        var p3Generic = CreateGenericVector(2.0, 2.0, 1.0);
        var p4Generic = CreateGenericVector(3.0, 0.0, 1.0);

        var pathFloat64 = new Float64Bezier3Path3D(false, p1Float64, p2Float64, p3Float64, p4Float64);
        var pathGeneric = Bezier3Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic, p4Generic);

        // Test at multiple t values
        var tValues = new[] { 0.0, 0.5, 1.0 };

        foreach (var t in tValues)
        {
            var deriv2Float64 = pathFloat64.GetDerivative2Value(t);
            var deriv2Generic = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(t));

            Assert.That(deriv2Generic.X.ScalarValue, Is.EqualTo(deriv2Float64.X.ScalarValue).Within(Tolerance), $"X failed at t={t}");
            Assert.That(deriv2Generic.Y.ScalarValue, Is.EqualTo(deriv2Float64.Y.ScalarValue).Within(Tolerance), $"Y failed at t={t}");
            Assert.That(deriv2Generic.Z.ScalarValue, Is.EqualTo(deriv2Float64.Z.ScalarValue).Within(Tolerance), $"Z failed at t={t}");
        }
    }

    [Test]
    public void Bezier3Path3D_GetDerivativeCurve_ShouldReturnBezier2()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(0.0, 0.0, 0.0);
        var p2Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 0.0);
        var p3Float64 = LinFloat64Vector3D.Create(2.0, 2.0, 1.0);
        var p4Float64 = LinFloat64Vector3D.Create(3.0, 0.0, 1.0);

        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(1.0, 2.0, 0.0);
        var p3Generic = CreateGenericVector(2.0, 2.0, 1.0);
        var p4Generic = CreateGenericVector(3.0, 0.0, 1.0);

        var pathFloat64 = new Float64Bezier3Path3D(false, p1Float64, p2Float64, p3Float64, p4Float64);
        var pathGeneric = Bezier3Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic, p4Generic);

        // Act
        var derivCurveFloat64 = pathFloat64.GetDerivativeCurve();
        var derivCurveGeneric = pathGeneric.GetDerivativeCurve();

        // Assert - Derivative curve should be Bezier2 with control points: 3(P₂-P₁), 3(P₃-P₂), 3(P₄-P₃)
        Assert.That(derivCurveGeneric.Point1.X.ScalarValue, Is.EqualTo(derivCurveFloat64.Point1.X.ScalarValue).Within(Tolerance));
        Assert.That(derivCurveGeneric.Point1.Y.ScalarValue, Is.EqualTo(derivCurveFloat64.Point1.Y.ScalarValue).Within(Tolerance));
        Assert.That(derivCurveGeneric.Point1.Z.ScalarValue, Is.EqualTo(derivCurveFloat64.Point1.Z.ScalarValue).Within(Tolerance));

        Assert.That(derivCurveGeneric.Point2.X.ScalarValue, Is.EqualTo(derivCurveFloat64.Point2.X.ScalarValue).Within(Tolerance));
        Assert.That(derivCurveGeneric.Point3.Z.ScalarValue, Is.EqualTo(derivCurveFloat64.Point3.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void Bezier3Path3D_ToFinitePath_ShouldPreserveGeometry()
    {
        // Arrange
        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(1.0, 2.0, 0.0);
        var p3Generic = CreateGenericVector(2.0, 2.0, 1.0);
        var p4Generic = CreateGenericVector(3.0, 0.0, 1.0);

        var pathPeriodic = Bezier3Path3D<double>.Create(ScalarProcessor, true, p1Generic, p2Generic, p3Generic, p4Generic);
        var pathFinite = Bezier3Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic, p4Generic);

        // Act
        var convertedFromPeriodic = pathPeriodic.ToFinitePath();
        var convertedFromFinite = pathFinite.ToFinitePath();

        // Assert
        Assert.That(convertedFromPeriodic.IsPeriodic, Is.False);
        Assert.That(convertedFromFinite.IsPeriodic, Is.False);

        // Verify geometry is preserved
        var value1 = convertedFromPeriodic.GetValue(ScalarProcessor.Scalar(0.5));
        var value2 = pathPeriodic.GetValue(ScalarProcessor.Scalar(0.5));

        Assert.That(value1.X.ScalarValue, Is.EqualTo(value2.X.ScalarValue).Within(Tolerance));
        Assert.That(value1.Y.ScalarValue, Is.EqualTo(value2.Y.ScalarValue).Within(Tolerance));
        Assert.That(value1.Z.ScalarValue, Is.EqualTo(value2.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void Bezier3Path3D_ToPeriodicPath_ShouldPreserveGeometry()
    {
        // Arrange
        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(1.0, 2.0, 0.0);
        var p3Generic = CreateGenericVector(2.0, 2.0, 1.0);
        var p4Generic = CreateGenericVector(3.0, 0.0, 1.0);

        var pathPeriodic = Bezier3Path3D<double>.Create(ScalarProcessor, true, p1Generic, p2Generic, p3Generic, p4Generic);
        var pathFinite = Bezier3Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic, p4Generic);

        // Act
        var convertedFromPeriodic = pathPeriodic.ToPeriodicPath();
        var convertedFromFinite = pathFinite.ToPeriodicPath();

        // Assert
        Assert.That(convertedFromPeriodic.IsPeriodic, Is.True);
        Assert.That(convertedFromFinite.IsPeriodic, Is.True);

        // Verify geometry is preserved
        var value1 = convertedFromFinite.GetValue(ScalarProcessor.Scalar(0.5));
        var value2 = pathFinite.GetValue(ScalarProcessor.Scalar(0.5));

        Assert.That(value1.X.ScalarValue, Is.EqualTo(value2.X.ScalarValue).Within(Tolerance));
        Assert.That(value1.Y.ScalarValue, Is.EqualTo(value2.Y.ScalarValue).Within(Tolerance));
        Assert.That(value1.Z.ScalarValue, Is.EqualTo(value2.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void Bezier3Path3D_IsValid_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var p2Float64 = LinFloat64Vector3D.Create(4.0, 5.0, 6.0);
        var p3Float64 = LinFloat64Vector3D.Create(7.0, 8.0, 9.0);
        var p4Float64 = LinFloat64Vector3D.Create(10.0, 11.0, 12.0);

        var p1Generic = CreateGenericVector(1.0, 2.0, 3.0);
        var p2Generic = CreateGenericVector(4.0, 5.0, 6.0);
        var p3Generic = CreateGenericVector(7.0, 8.0, 9.0);
        var p4Generic = CreateGenericVector(10.0, 11.0, 12.0);

        var pathFloat64 = new Float64Bezier3Path3D(false, p1Float64, p2Float64, p3Float64, p4Float64);
        var pathGeneric = Bezier3Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic, p4Generic);

        // Act & Assert
        Assert.That(pathGeneric.IsValid(), Is.EqualTo(pathFloat64.IsValid()));
        Assert.That(pathGeneric.IsValid(), Is.True);
    }
}
