using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Equivalence Tests for Generic CirclePath2D vs Float64CirclePath2D
/// Phase 3 Module 6B - 2D Circle Trajectories
/// Tests: Generic double vs Float64 Specialized for CirclePath2D
/// </summary>
/// <remarks>
/// KNOWN ISSUE: Float64CirclePath2D has a bug where it doesn't add Center to positions.
/// These tests verify the CORRECT Generic<T> behavior. Float64CirclePath2D needs fixing.
/// </remarks>
[TestFixture]
public class CirclePath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static LinVector2D<double> CreateGenericVector(double x, double y)
    {
        return LinVector2D<double>.Create(ScalarProcessor, x, y);
    }

    #endregion

    #region CirclePath2D Tests (12 tests)

    [Test]
    public void CirclePath2D_AtOrigin_GetValue_ShouldBeOnCircle()
    {
        // Arrange - Circle at origin with radius 2
        var centerGeneric = CreateGenericVector(0.0, 0.0);
        var radius = 2.0;

        var pathGeneric = CirclePath2D<double>.Create(ScalarProcessor, centerGeneric, radius);

        // Act & Assert - Test at key angles
        var testTimes = new[] { 0.0, 0.25, 0.5, 0.75, 1.0 }; // 0°, 90°, 180°, 270°, 360°

        foreach (var t in testTimes)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var point = pathGeneric.GetValue(tScalar);

            // Distance from origin should equal radius
            var distance = Math.Sqrt(point.X.ScalarValue * point.X.ScalarValue + point.Y.ScalarValue * point.Y.ScalarValue);
            Assert.That(distance, Is.EqualTo(radius).Within(Tolerance), $"Point at t={t} should be on circle");
        }
    }

    [Test]
    public void CirclePath2D_WithCenter_GetValue_ShouldBeRelativeToCenter()
    {
        // Arrange - Circle at (3, 4) with radius 5
        var centerGeneric = CreateGenericVector(3.0, 4.0);
        var radius = 5.0;

        var pathGeneric = CirclePath2D<double>.Create(ScalarProcessor, centerGeneric, radius);

        // Act & Assert - Points should be at distance radius from center
        for (var t = 0.0; t <= 1.0; t += 0.1)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var point = pathGeneric.GetValue(tScalar);

            // Distance from center should equal radius
            var dx = point.X.ScalarValue - centerGeneric.X.ScalarValue;
            var dy = point.Y.ScalarValue - centerGeneric.Y.ScalarValue;
            var distance = Math.Sqrt(dx * dx + dy * dy);

            Assert.That(distance, Is.EqualTo(radius).Within(Tolerance), $"Distance from center at t={t}");
        }
    }

    [Test]
    public void CirclePath2D_GetValue_AtT0_ShouldBeAtStartPosition()
    {
        // Arrange
        var centerGeneric = CreateGenericVector(1.0, 2.0);
        var radius = 3.0;
        var pathGeneric = CirclePath2D<double>.Create(ScalarProcessor, centerGeneric, radius);

        // Act - At t=0, angle=0, so point should be (radius, 0) + center = (4, 2)
        var point = pathGeneric.GetValue(ScalarProcessor.Scalar(0.0));

        // Assert
        Assert.That(point.X.ScalarValue, Is.EqualTo(1.0 + radius).Within(Tolerance), "X at t=0");
        Assert.That(point.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Y at t=0");
    }

    [Test]
    public void CirclePath2D_GetValue_AtT025_ShouldBeAtTopOfCircle()
    {
        // Arrange - t=0.25 means angle = π/2 (90°), so point is at (0, radius) + center
        var centerGeneric = CreateGenericVector(0.0, 0.0);
        var radius = 1.0;
        var pathGeneric = CirclePath2D<double>.Create(ScalarProcessor, centerGeneric, radius);

        // Act
        var point = pathGeneric.GetValue(ScalarProcessor.Scalar(0.25));

        // Assert - At 90°: cos(π/2)=0, sin(π/2)=1
        Assert.That(point.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=0.25 (90°)");
        Assert.That(point.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Y at t=0.25 (90°)");
    }

    [Test]
    public void CirclePath2D_ReverseDirection_ShouldRotateClockwise()
    {
        // Arrange - Normal circle rotates counter-clockwise, reversed rotates clockwise
        var centerGeneric = CreateGenericVector(0.0, 0.0);
        var radius = 1.0;

        var pathNormal = CirclePath2D<double>.Create(ScalarProcessor, centerGeneric, radius, false);
        var pathReversed = CirclePath2D<double>.Create(ScalarProcessor, centerGeneric, radius, true);

        // Act - At t=0.25 (should be 90°)
        var tScalar = ScalarProcessor.Scalar(0.25);
        var pointNormal = pathNormal.GetValue(tScalar);
        var pointReversed = pathReversed.GetValue(tScalar);

        // Assert - Normal: (0, 1), Reversed: (0, -1)
        Assert.That(pointNormal.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Normal Y at 90°");
        Assert.That(pointReversed.Y.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), "Reversed Y at -90°");
    }

    [Test]
    public void CirclePath2D_GetDerivative1Value_ShouldBeTangentToCircle()
    {
        // Arrange
        var centerGeneric = CreateGenericVector(2.0, 3.0);
        var radius = 4.0;
        var pathGeneric = CirclePath2D<double>.Create(ScalarProcessor, centerGeneric, radius);

        // Act & Assert - Tangent should be perpendicular to radius vector
        for (var t = 0.0; t <= 1.0; t += 0.25)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var point = pathGeneric.GetValue(tScalar);
            var tangent = pathGeneric.GetDerivative1Value(tScalar);

            // Radius vector from center to point
            var radiusVecX = point.X.ScalarValue - centerGeneric.X.ScalarValue;
            var radiusVecY = point.Y.ScalarValue - centerGeneric.Y.ScalarValue;

            // Dot product of radius and tangent should be near zero (perpendicular)
            var dotProduct = radiusVecX * tangent.X.ScalarValue + radiusVecY * tangent.Y.ScalarValue;

            Assert.That(dotProduct, Is.EqualTo(0.0).Within(Tolerance), $"Tangent perpendicular to radius at t={t}");
        }
    }

    [Test]
    public void CirclePath2D_GetDerivative2Value_ShouldPointTowardCenter()
    {
        // Arrange - Second derivative (acceleration) for circular motion points toward center
        var centerGeneric = CreateGenericVector(5.0, 6.0);
        var radius = 2.0;
        var pathGeneric = CirclePath2D<double>.Create(ScalarProcessor, centerGeneric, radius);

        // Act & Assert
        for (var t = 0.0; t <= 1.0; t += 0.25)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var point = pathGeneric.GetValue(tScalar);
            var accel = pathGeneric.GetDerivative2Value(tScalar);

            // Vector from point to center
            var toCenterX = centerGeneric.X.ScalarValue - point.X.ScalarValue;
            var toCenterY = centerGeneric.Y.ScalarValue - point.Y.ScalarValue;

            // Normalize both vectors
            var toCenterNorm = Math.Sqrt(toCenterX * toCenterX + toCenterY * toCenterY);
            var accelNorm = Math.Sqrt(accel.X.ScalarValue * accel.X.ScalarValue + accel.Y.ScalarValue * accel.Y.ScalarValue);

            if (accelNorm > Tolerance && toCenterNorm > Tolerance)
            {
                var toCenterUnitX = toCenterX / toCenterNorm;
                var toCenterUnitY = toCenterY / toCenterNorm;
                var accelUnitX = accel.X.ScalarValue / accelNorm;
                var accelUnitY = accel.Y.ScalarValue / accelNorm;

                // Acceleration should be parallel to vector toward center
                Assert.That(accelUnitX, Is.EqualTo(toCenterUnitX).Within(Tolerance), $"Accel X direction at t={t}");
                Assert.That(accelUnitY, Is.EqualTo(toCenterUnitY).Within(Tolerance), $"Accel Y direction at t={t}");
            }
        }
    }

    [Test]
    public void CirclePath2D_GetLength_ShouldBeCircumference()
    {
        // Arrange
        var centerGeneric = CreateGenericVector(0.0, 0.0);
        var radius = 3.0;
        var pathGeneric = CirclePath2D<double>.Create(ScalarProcessor, centerGeneric, radius);

        // Act
        var length = pathGeneric.GetLength();

        // Assert - Circumference = 2πr
        var expectedLength = 2.0 * Math.PI * radius;
        Assert.That(length.ScalarValue, Is.EqualTo(expectedLength).Within(Tolerance), "Circumference");
    }

    [Test]
    public void CirclePath2D_TimeToLength_ShouldBeProportional()
    {
        // Arrange
        var centerGeneric = CreateGenericVector(1.0, 1.0);
        var radius = 5.0;
        var pathGeneric = CirclePath2D<double>.Create(ScalarProcessor, centerGeneric, radius);

        var totalLength = pathGeneric.GetLength().ScalarValue;

        // Act & Assert - Arc length should be proportional to time parameter
        for (var t = 0.0; t <= 1.0; t += 0.2)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var arcLength = pathGeneric.TimeToLength(tScalar);

            var expectedLength = t * totalLength;
            Assert.That(arcLength.ScalarValue, Is.EqualTo(expectedLength).Within(Tolerance), $"Arc length at t={t}");
        }
    }

    [Test]
    public void CirclePath2D_LengthToTime_ShouldBeInverseOfTimeToLength()
    {
        // Arrange
        var centerGeneric = CreateGenericVector(0.0, 0.0);
        var radius = 2.0;
        var pathGeneric = CirclePath2D<double>.Create(ScalarProcessor, centerGeneric, radius);

        // Act & Assert - Round-trip conversion should be identity
        for (var t = 0.0; t <= 1.0; t += 0.1)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var length = pathGeneric.TimeToLength(tScalar);
            var tBack = pathGeneric.LengthToTime(length);

            Assert.That(tBack.ScalarValue, Is.EqualTo(t).Within(Tolerance), $"Round-trip at t={t}");
        }
    }

    [Test]
    public void CirclePath2D_GetFrame_TangentShouldBeNormalized()
    {
        // Arrange
        var centerGeneric = CreateGenericVector(3.0, 4.0);
        var radius = 1.5;
        var pathGeneric = CirclePath2D<double>.Create(ScalarProcessor, centerGeneric, radius);

        // Act & Assert - Frame tangent should be unit vector
        for (var t = 0.0; t <= 1.0; t += 0.25)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var frame = pathGeneric.GetFrame(tScalar);

            var tangentNormSq = frame.Tangent.X.ScalarValue * frame.Tangent.X.ScalarValue +
                                 frame.Tangent.Y.ScalarValue * frame.Tangent.Y.ScalarValue;

            Assert.That(tangentNormSq, Is.EqualTo(1.0).Within(Tolerance), $"Tangent normalized at t={t}");
        }
    }

    [Test]
    public void CirclePath2D_GetFrame_NormalShouldBePerpendicularToTangent()
    {
        // Arrange
        var centerGeneric = CreateGenericVector(0.0, 0.0);
        var radius = 1.0;
        var pathGeneric = CirclePath2D<double>.Create(ScalarProcessor, centerGeneric, radius);

        // Act & Assert
        for (var t = 0.0; t <= 1.0; t += 0.2)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var frame = pathGeneric.GetFrame(tScalar);

            // Dot product of tangent and normal should be zero
            var dotProduct = frame.Tangent.X.ScalarValue * frame.Normal.X.ScalarValue +
                              frame.Tangent.Y.ScalarValue * frame.Normal.Y.ScalarValue;

            Assert.That(dotProduct, Is.EqualTo(0.0).Within(Tolerance), $"Normal perpendicular to tangent at t={t}");
        }
    }

    [Test]
    public void CirclePath2D_IsValid_WithPositiveRadius_ShouldBeTrue()
    {
        // Arrange
        var centerGeneric = CreateGenericVector(1.0, 2.0);
        var radius = 3.0;
        var pathGeneric = CirclePath2D<double>.Create(ScalarProcessor, centerGeneric, radius);

        // Act
        var isValid = pathGeneric.IsValid();

        // Assert
        Assert.That(isValid, Is.True, "Circle with positive radius should be valid");
    }

    [Test]
    public void CirclePath2D_WithNegativeRadius_ShouldThrowException()
    {
        // Arrange
        var centerGeneric = CreateGenericVector(0.0, 0.0);
        var negativeRadius = -1.0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
        {
            CirclePath2D<double>.Create(ScalarProcessor, centerGeneric, negativeRadius);
        }, "Should throw for negative radius");
    }

    #endregion
}
