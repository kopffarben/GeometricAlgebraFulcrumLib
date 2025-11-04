using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Float64.Bezier;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Bezier;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Equivalence Tests for Generic Bezier Paths vs Float64 Bezier Paths
/// Phase 3 Module 6B - 2D Bezier Curves
/// Tests: Generic double vs Float64 Specialized for Bezier0, Bezier1, Bezier2Path2D
/// </summary>
[TestFixture]
public class BezierPath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static LinVector2D<double> CreateGenericVector(double x, double y)
    {
        return LinVector2D<double>.Create(ScalarProcessor, x, y);
    }

    #endregion

    #region Bezier0Path2D Tests (5 tests)

    [Test]
    public void Bezier0Path2D_GetValue_ShouldBeConstant()
    {
        // Arrange - Constant Bezier (degree 0)
        var pointFloat64 = LinFloat64Vector2D.Create(3.0, 4.0);
        var pointGeneric = CreateGenericVector(3.0, 4.0);

        var pathFloat64 = Float64Bezier0Path2D.Finite(pointFloat64);
        var pathGeneric = Bezier0Path2D<double>.Create(ScalarProcessor, false, pointGeneric);

        // Act & Assert - Value should be constant for all t
        for (var t = 0.0; t <= 1.0; t += 0.25)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = pathFloat64.GetValue(t);
            var valueGeneric = pathGeneric.GetValue(tScalar);

            Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance), $"X at t={t}");
            Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance), $"Y at t={t}");
        }
    }

    [Test]
    public void Bezier0Path2D_GetDerivative1_ShouldBeZero()
    {
        // Arrange
        var pointGeneric = CreateGenericVector(1.0, 2.0);
        var pathGeneric = Bezier0Path2D<double>.Create(ScalarProcessor, false, pointGeneric);

        // Act
        var derivative = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(0.5));

        // Assert - Derivative of constant is zero
        Assert.That(derivative.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Derivative X should be 0");
        Assert.That(derivative.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Derivative Y should be 0");
    }

    [Test]
    public void Bezier0Path2D_GetDerivative2_ShouldBeZero()
    {
        // Arrange
        var pointGeneric = CreateGenericVector(5.0, -3.0);
        var pathGeneric = Bezier0Path2D<double>.Create(ScalarProcessor, false, pointGeneric);

        // Act
        var derivative2 = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(0.5));

        // Assert - Second derivative of constant is zero
        Assert.That(derivative2.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative X should be 0");
        Assert.That(derivative2.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative Y should be 0");
    }

    [Test]
    public void Bezier0Path2D_ToFinitePath_ShouldBeIdempotent()
    {
        // Arrange
        var pointGeneric = CreateGenericVector(2.0, 3.0);
        var pathGeneric = Bezier0Path2D<double>.Create(ScalarProcessor, false, pointGeneric);

        // Act
        var finitePath = pathGeneric.ToFinitePath();

        // Assert - Should return same instance if already finite
        Assert.That(ReferenceEquals(pathGeneric, finitePath), Is.True, "ToFinitePath should return same instance when already finite");
        Assert.That(finitePath.IsFinite, Is.True, "IsFinite should be true");
    }

    [Test]
    public void Bezier0Path2D_ToPeriodicPath_ShouldConvert()
    {
        // Arrange
        var pointGeneric = CreateGenericVector(4.0, 5.0);
        var pathGeneric = Bezier0Path2D<double>.Create(ScalarProcessor, false, pointGeneric);

        // Act
        var periodicPath = pathGeneric.ToPeriodicPath();

        // Assert - Should create new periodic instance
        Assert.That(ReferenceEquals(pathGeneric, periodicPath), Is.False, "ToPeriodicPath should create new instance");
        Assert.That(periodicPath.IsPeriodic, Is.True, "Converted path should be periodic");
        Assert.That(pathGeneric.IsFinite, Is.True, "Original path should remain finite");
    }

    #endregion

    #region Bezier1Path2D Tests (8 tests)

    [Test]
    public void Bezier1Path2D_GetValue_AtT0_ShouldBePoint1()
    {
        // Arrange
        var point1Float64 = LinFloat64Vector2D.Create(1.0, 2.0);
        var point2Float64 = LinFloat64Vector2D.Create(5.0, 6.0);

        var point1Generic = CreateGenericVector(1.0, 2.0);
        var point2Generic = CreateGenericVector(5.0, 6.0);

        var pathFloat64 = new Float64Bezier1Path2D(false, point1Float64, point2Float64);
        var pathGeneric = Bezier1Path2D<double>.Create(ScalarProcessor, false, point1Generic, point2Generic);

        // Act
        var valueFloat64 = pathFloat64.GetValue(0.0);
        var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(0.0));

        // Assert
        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance), "X at t=0");
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance), "Y at t=0");
        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Should be Point1 at t=0");
    }

    [Test]
    public void Bezier1Path2D_GetValue_AtT1_ShouldBePoint2()
    {
        // Arrange
        var point1Generic = CreateGenericVector(1.0, 2.0);
        var point2Generic = CreateGenericVector(5.0, 6.0);

        var pathGeneric = Bezier1Path2D<double>.Create(ScalarProcessor, false, point1Generic, point2Generic);

        // Act
        var value = pathGeneric.GetValue(ScalarProcessor.Scalar(1.0));

        // Assert
        Assert.That(value.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "X at t=1");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(6.0).Within(Tolerance), "Y at t=1");
    }

    [Test]
    public void Bezier1Path2D_GetValue_AtT05_ShouldBeMidpoint()
    {
        // Arrange - Linear interpolation at t=0.5 should be midpoint
        var point1Generic = CreateGenericVector(0.0, 0.0);
        var point2Generic = CreateGenericVector(10.0, 20.0);

        var pathGeneric = Bezier1Path2D<double>.Create(ScalarProcessor, false, point1Generic, point2Generic);

        // Act
        var value = pathGeneric.GetValue(ScalarProcessor.Scalar(0.5));

        // Assert - Midpoint
        Assert.That(value.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "X at t=0.5 should be midpoint");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(10.0).Within(Tolerance), "Y at t=0.5 should be midpoint");
    }

    [Test]
    public void Bezier1Path2D_GetDerivative1_ShouldBeConstant()
    {
        // Arrange
        var point1Generic = CreateGenericVector(1.0, 2.0);
        var point2Generic = CreateGenericVector(5.0, 6.0);

        var pathGeneric = Bezier1Path2D<double>.Create(ScalarProcessor, false, point1Generic, point2Generic);

        // Expected: Point2 - Point1 = (4, 4)
        var expectedX = 4.0;
        var expectedY = 4.0;

        // Act & Assert - Derivative should be constant for all t
        for (var t = 0.0; t <= 1.0; t += 0.25)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var derivative = pathGeneric.GetDerivative1Value(tScalar);

            Assert.That(derivative.X.ScalarValue, Is.EqualTo(expectedX).Within(Tolerance), $"Derivative X at t={t}");
            Assert.That(derivative.Y.ScalarValue, Is.EqualTo(expectedY).Within(Tolerance), $"Derivative Y at t={t}");
        }
    }

    [Test]
    public void Bezier1Path2D_GetDerivative2_ShouldBeZero()
    {
        // Arrange
        var point1Generic = CreateGenericVector(0.0, 0.0);
        var point2Generic = CreateGenericVector(10.0, 20.0);

        var pathGeneric = Bezier1Path2D<double>.Create(ScalarProcessor, false, point1Generic, point2Generic);

        // Act
        var derivative2 = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(0.5));

        // Assert - Linear curve has zero acceleration
        Assert.That(derivative2.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative X should be 0");
        Assert.That(derivative2.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative Y should be 0");
    }

    [Test]
    public void Bezier1Path2D_GetDerivativeCurve_ShouldBeBezier0()
    {
        // Arrange
        var point1Generic = CreateGenericVector(1.0, 2.0);
        var point2Generic = CreateGenericVector(5.0, 6.0);

        var pathGeneric = Bezier1Path2D<double>.Create(ScalarProcessor, false, point1Generic, point2Generic);

        // Act
        var derivativeCurve = pathGeneric.GetDerivativeCurve();

        // Assert - Derivative of Bezier1 is Bezier0 (constant)
        Assert.That(derivativeCurve, Is.InstanceOf<Bezier0Path2D<double>>(), "Derivative curve should be Bezier0");

        // Derivative curve should have value Point2 - Point1
        var expectedPoint = point2Generic - point1Generic;
        var derivValue = derivativeCurve.GetValue(ScalarProcessor.Scalar(0.5));

        Assert.That(derivValue.X.ScalarValue, Is.EqualTo(expectedPoint.X.ScalarValue).Within(Tolerance), "Derivative curve X");
        Assert.That(derivValue.Y.ScalarValue, Is.EqualTo(expectedPoint.Y.ScalarValue).Within(Tolerance), "Derivative curve Y");
    }

    [Test]
    public void Bezier1Path2D_GetFrame_TangentShouldBeNormalized()
    {
        // Arrange
        var point1Generic = CreateGenericVector(0.0, 0.0);
        var point2Generic = CreateGenericVector(3.0, 4.0); // Length 5

        var pathGeneric = Bezier1Path2D<double>.Create(ScalarProcessor, false, point1Generic, point2Generic);

        // Act
        var frame = pathGeneric.GetFrame(ScalarProcessor.Scalar(0.5));

        // Assert - Tangent should be normalized (unit vector)
        var tangentNormSq = frame.Tangent.X.ScalarValue * frame.Tangent.X.ScalarValue +
                             frame.Tangent.Y.ScalarValue * frame.Tangent.Y.ScalarValue;

        Assert.That(tangentNormSq, Is.EqualTo(1.0).Within(Tolerance), "Frame tangent should be normalized");
    }

    [Test]
    public void Bezier1Path2D_ShouldMatchFloat64()
    {
        // Arrange
        var point1Float64 = LinFloat64Vector2D.Create(2.0, 3.0);
        var point2Float64 = LinFloat64Vector2D.Create(7.0, 11.0);

        var point1Generic = CreateGenericVector(2.0, 3.0);
        var point2Generic = CreateGenericVector(7.0, 11.0);

        var pathFloat64 = new Float64Bezier1Path2D(false, point1Float64, point2Float64);
        var pathGeneric = Bezier1Path2D<double>.Create(ScalarProcessor, false, point1Generic, point2Generic);

        // Act & Assert - Compare at multiple t values
        for (var t = 0.0; t <= 1.0; t += 0.1)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = pathFloat64.GetValue(t);
            var valueGeneric = pathGeneric.GetValue(tScalar);

            Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance), $"X at t={t}");
            Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance), $"Y at t={t}");
        }
    }

    #endregion

    #region Bezier2Path2D Tests (5 tests)

    [Test]
    public void Bezier2Path2D_GetValue_AtT0_ShouldBePoint1()
    {
        // Arrange
        var point1Generic = CreateGenericVector(1.0, 2.0);
        var point2Generic = CreateGenericVector(5.0, 10.0);
        var point3Generic = CreateGenericVector(9.0, 12.0);

        var pathGeneric = Bezier2Path2D<double>.Create(ScalarProcessor, false, point1Generic, point2Generic, point3Generic);

        // Act
        var value = pathGeneric.GetValue(ScalarProcessor.Scalar(0.0));

        // Assert - At t=0, curve should be at Point1
        Assert.That(value.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X at t=0");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Y at t=0");
    }

    [Test]
    public void Bezier2Path2D_GetValue_AtT1_ShouldBePoint3()
    {
        // Arrange
        var point1Generic = CreateGenericVector(1.0, 2.0);
        var point2Generic = CreateGenericVector(5.0, 10.0);
        var point3Generic = CreateGenericVector(9.0, 12.0);

        var pathGeneric = Bezier2Path2D<double>.Create(ScalarProcessor, false, point1Generic, point2Generic, point3Generic);

        // Act
        var value = pathGeneric.GetValue(ScalarProcessor.Scalar(1.0));

        // Assert - At t=1, curve should be at Point3
        Assert.That(value.X.ScalarValue, Is.EqualTo(9.0).Within(Tolerance), "X at t=1");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(12.0).Within(Tolerance), "Y at t=1");
    }

    [Test]
    public void Bezier2Path2D_GetDerivative2_ShouldBeConstant()
    {
        // Arrange - Quadratic curve has constant second derivative
        var point1Generic = CreateGenericVector(0.0, 0.0);
        var point2Generic = CreateGenericVector(5.0, 5.0);
        var point3Generic = CreateGenericVector(10.0, 0.0);

        var pathGeneric = Bezier2Path2D<double>.Create(ScalarProcessor, false, point1Generic, point2Generic, point3Generic);

        // Act - Test at multiple t values
        var deriv2_t0 = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(0.0));
        var deriv2_t05 = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(0.5));
        var deriv2_t1 = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(1.0));

        // Assert - Second derivative should be constant
        Assert.That(deriv2_t05.X.ScalarValue, Is.EqualTo(deriv2_t0.X.ScalarValue).Within(Tolerance), "Second derivative X should be constant");
        Assert.That(deriv2_t05.Y.ScalarValue, Is.EqualTo(deriv2_t0.Y.ScalarValue).Within(Tolerance), "Second derivative Y should be constant");
        Assert.That(deriv2_t1.X.ScalarValue, Is.EqualTo(deriv2_t0.X.ScalarValue).Within(Tolerance), "Second derivative X should be constant");
        Assert.That(deriv2_t1.Y.ScalarValue, Is.EqualTo(deriv2_t0.Y.ScalarValue).Within(Tolerance), "Second derivative Y should be constant");
    }

    [Test]
    public void Bezier2Path2D_GetDerivativeCurve_ShouldBeBezier1()
    {
        // Arrange
        var point1Generic = CreateGenericVector(1.0, 2.0);
        var point2Generic = CreateGenericVector(5.0, 6.0);
        var point3Generic = CreateGenericVector(9.0, 10.0);

        var pathGeneric = Bezier2Path2D<double>.Create(ScalarProcessor, false, point1Generic, point2Generic, point3Generic);

        // Act
        var derivativeCurve = pathGeneric.GetDerivativeCurve();

        // Assert - Derivative of Bezier2 is Bezier1
        Assert.That(derivativeCurve, Is.InstanceOf<Bezier1Path2D<double>>(), "Derivative curve should be Bezier1");
    }

    [Test]
    public void Bezier2Path2D_GetFrame_TangentShouldBeNormalized()
    {
        // Arrange
        var point1Generic = CreateGenericVector(0.0, 0.0);
        var point2Generic = CreateGenericVector(5.0, 10.0);
        var point3Generic = CreateGenericVector(10.0, 0.0);

        var pathGeneric = Bezier2Path2D<double>.Create(ScalarProcessor, false, point1Generic, point2Generic, point3Generic);

        // Act
        var frame = pathGeneric.GetFrame(ScalarProcessor.Scalar(0.5));

        // Assert - Tangent should be normalized
        var tangentNormSq = frame.Tangent.X.ScalarValue * frame.Tangent.X.ScalarValue +
                             frame.Tangent.Y.ScalarValue * frame.Tangent.Y.ScalarValue;

        Assert.That(tangentNormSq, Is.EqualTo(1.0).Within(Tolerance), "Frame tangent should be normalized");
    }

    #endregion
}
