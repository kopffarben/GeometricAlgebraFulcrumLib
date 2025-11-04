using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Equivalence Tests for Generic ConstantPath2D vs Float64ConstantPath2D
/// Phase 3 Module 6B - 2D Constant Trajectories
/// Tests: Generic double vs Float64 Specialized for ConstantPath2D
/// </summary>
[TestFixture]
public class ConstantPath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static LinVector2D<double> CreateGenericVector(double x, double y)
    {
        return LinVector2D<double>.Create(ScalarProcessor, x, y);
    }

    #endregion

    #region ConstantPath2D Equivalence Tests (11 tests)

    [Test]
    public void ConstantPath2D_GetValue_ShouldMatchFloat64()
    {
        // Arrange
        var pointFloat64 = LinFloat64Vector2D.Create(3.0, 4.0);
        var pointGeneric = CreateGenericVector(3.0, 4.0);

        var pathFloat64 = Float64ConstantPath2D.Finite(pointFloat64);
        var pathGeneric = ConstantPath2D<double>.Create(ScalarProcessor, pointGeneric);

        // Act & Assert at various times
        for (var t = -1.0; t <= 1.0; t += 0.5)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = pathFloat64.GetValue(t);
            var valueGeneric = pathGeneric.GetValue(tScalar);

            Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance), $"X at t={t}");
            Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance), $"Y at t={t}");
        }
    }

    [Test]
    public void ConstantPath2D_GetDerivative1Value_ShouldBeZero()
    {
        // Arrange - constant path with zero tangent
        var pointFloat64 = LinFloat64Vector2D.Create(5.0, -3.0);
        var pointGeneric = CreateGenericVector(5.0, -3.0);

        var pathFloat64 = Float64ConstantPath2D.Finite(pointFloat64);
        var pathGeneric = ConstantPath2D<double>.Create(ScalarProcessor, pointGeneric);

        // Act
        var derivative1Float64 = pathFloat64.GetDerivative1Value(0.0);
        var derivative1Generic = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(0.0));

        // Assert - Derivative of constant path should be zero
        Assert.That(derivative1Generic.X.ScalarValue, Is.EqualTo(derivative1Float64.X.ScalarValue).Within(Tolerance), "Derivative1 X should be 0");
        Assert.That(derivative1Generic.Y.ScalarValue, Is.EqualTo(derivative1Float64.Y.ScalarValue).Within(Tolerance), "Derivative1 Y should be 0");

        Assert.That(derivative1Float64.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Float64 Derivative1 should be zero");
        Assert.That(derivative1Generic.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Generic Derivative1 should be zero");
    }

    [Test]
    public void ConstantPath2D_GetDerivative2Value_ShouldBeZero()
    {
        // Arrange
        var pointFloat64 = LinFloat64Vector2D.Create(1.0, 2.0);
        var pointGeneric = CreateGenericVector(1.0, 2.0);

        var pathFloat64 = Float64ConstantPath2D.Finite(pointFloat64);
        var pathGeneric = ConstantPath2D<double>.Create(ScalarProcessor, pointGeneric);

        // Act
        var derivative2Float64 = pathFloat64.GetDerivative2Value(0.5);
        var derivative2Generic = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(0.5));

        // Assert - Second derivative of constant path should be zero
        Assert.That(derivative2Generic.X.ScalarValue, Is.EqualTo(derivative2Float64.X.ScalarValue).Within(Tolerance), "Derivative2 X should be 0");
        Assert.That(derivative2Generic.Y.ScalarValue, Is.EqualTo(derivative2Float64.Y.ScalarValue).Within(Tolerance), "Derivative2 Y should be 0");

        Assert.That(derivative2Float64.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Float64 Derivative2 should be zero");
        Assert.That(derivative2Generic.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Generic Derivative2 should be zero");
    }

    [Test]
    public void ConstantPath2D_GetFrame_ShouldMatchFloat64()
    {
        // Arrange
        var pointFloat64 = LinFloat64Vector2D.Create(2.0, 3.0);
        var tangentFloat64 = LinFloat64Vector2D.Create(1.0, 0.0);

        var pointGeneric = CreateGenericVector(2.0, 3.0);
        var tangentGeneric = CreateGenericVector(1.0, 0.0);

        var pathFloat64 = Float64ConstantPath2D.Finite(pointFloat64, tangentFloat64);
        var pathGeneric = ConstantPath2D<double>.Create(
            ScalarRange<double>.Create(ScalarProcessor.Scalar(-1.0), ScalarProcessor.Scalar(1.0)),
            false,
            pointGeneric,
            tangentGeneric
        );

        // Act
        var frameFloat64 = pathFloat64.GetFrame(0.5);
        var frameGeneric = pathGeneric.GetFrame(ScalarProcessor.Scalar(0.5));

        // Assert - Point should match
        Assert.That(frameGeneric.Point.X.ScalarValue, Is.EqualTo(frameFloat64.Point.X.ScalarValue).Within(Tolerance), "Frame Point X");
        Assert.That(frameGeneric.Point.Y.ScalarValue, Is.EqualTo(frameFloat64.Point.Y.ScalarValue).Within(Tolerance), "Frame Point Y");

        // Tangent should match
        Assert.That(frameGeneric.Tangent.X.ScalarValue, Is.EqualTo(frameFloat64.Tangent.X.ScalarValue).Within(Tolerance), "Frame Tangent X");
        Assert.That(frameGeneric.Tangent.Y.ScalarValue, Is.EqualTo(frameFloat64.Tangent.Y.ScalarValue).Within(Tolerance), "Frame Tangent Y");
    }

    [Test]
    public void ConstantPath2D_IsValid_ShouldMatchFloat64()
    {
        // Arrange
        var pointFloat64 = LinFloat64Vector2D.Create(7.0, -2.0);
        var pointGeneric = CreateGenericVector(7.0, -2.0);

        var pathFloat64 = Float64ConstantPath2D.Finite(pointFloat64);
        var pathGeneric = ConstantPath2D<double>.Create(ScalarProcessor, pointGeneric);

        // Act
        var isValidFloat64 = pathFloat64.IsValid();
        var isValidGeneric = pathGeneric.IsValid();

        // Assert
        Assert.That(isValidGeneric, Is.EqualTo(isValidFloat64), "IsValid should match");
        Assert.That(isValidFloat64, Is.True, "Float64 path should be valid");
        Assert.That(isValidGeneric, Is.True, "Generic path should be valid");
    }

    [Test]
    public void ConstantPath2D_ToFinitePath_ShouldBeIdempotent()
    {
        // Arrange - Start with finite (non-periodic) path
        var pointGeneric = CreateGenericVector(4.0, 5.0);
        var pathGeneric = ConstantPath2D<double>.Create(ScalarProcessor, pointGeneric);

        // Act
        var finitePathGeneric = pathGeneric.ToFinitePath();

        // Assert - Should return same instance if already finite
        Assert.That(ReferenceEquals(pathGeneric, finitePathGeneric), Is.True, "Generic: ToFinitePath should return same instance when already finite");
        Assert.That(finitePathGeneric.IsFinite, Is.True, "IsFinite should be true after conversion");
    }

    [Test]
    public void ConstantPath2D_ToPeriodicPath_ShouldConvertCorrectly()
    {
        // Arrange - Start with finite (non-periodic) path
        var pointGeneric = CreateGenericVector(6.0, 7.0);
        var pathGeneric = ConstantPath2D<double>.Create(ScalarProcessor, pointGeneric);

        // Act
        var periodicPathGeneric = pathGeneric.ToPeriodicPath();

        // Assert - Should create new periodic instance
        Assert.That(ReferenceEquals(pathGeneric, periodicPathGeneric), Is.False, "Generic: ToPeriodicPath should create new instance");
        Assert.That(periodicPathGeneric.IsPeriodic, Is.True, "Generic converted path should be periodic");
        Assert.That(pathGeneric.IsFinite, Is.True, "Original path should remain finite");
    }

    [Test]
    public void ConstantPath2D_TimeRange_ShouldMatchFloat64()
    {
        // Arrange
        var rangeFloat64 = Float64ScalarRange.Create(0, Math.PI);
        var rangeGeneric = ScalarRange<double>.Create(ScalarProcessor.Scalar(0), ScalarProcessor.Scalar(Math.PI));

        var pointFloat64 = LinFloat64Vector2D.Create(1.0, 1.0);
        var pointGeneric = CreateGenericVector(1.0, 1.0);

        var pathFloat64 = Float64ConstantPath2D.Finite(rangeFloat64, pointFloat64);
        var pathGeneric = ConstantPath2D<double>.Create(rangeGeneric, false, pointGeneric);

        // Assert
        Assert.That(pathGeneric.TimeRange.MinValue.ScalarValue, Is.EqualTo(pathFloat64.TimeRange.MinValue).Within(Tolerance), "TimeRange MinValue");
        Assert.That(pathGeneric.TimeRange.MaxValue.ScalarValue, Is.EqualTo(pathFloat64.TimeRange.MaxValue).Within(Tolerance), "TimeRange MaxValue");
    }

    [Test]
    public void ConstantPath2D_WithNonZeroTangent_ShouldReturnConstantTangent()
    {
        // Arrange
        var pointFloat64 = LinFloat64Vector2D.Create(0.0, 0.0);
        var tangentFloat64 = LinFloat64Vector2D.Create(3.0, 4.0);

        var pointGeneric = CreateGenericVector(0.0, 0.0);
        var tangentGeneric = CreateGenericVector(3.0, 4.0);

        var pathFloat64 = Float64ConstantPath2D.Finite(pointFloat64, tangentFloat64);
        var pathGeneric = ConstantPath2D<double>.Create(
            ScalarRange<double>.Create(ScalarProcessor.Scalar(-1.0), ScalarProcessor.Scalar(1.0)),
            false,
            pointGeneric,
            tangentGeneric
        );

        // Act - Test at multiple times
        for (var t = -1.0; t <= 1.0; t += 0.5)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var derivative1Float64 = pathFloat64.GetDerivative1Value(t);
            var derivative1Generic = pathGeneric.GetDerivative1Value(tScalar);

            // Assert - Tangent should be constant
            Assert.That(derivative1Generic.X.ScalarValue, Is.EqualTo(derivative1Float64.X.ScalarValue).Within(Tolerance), $"Tangent X at t={t}");
            Assert.That(derivative1Generic.Y.ScalarValue, Is.EqualTo(derivative1Float64.Y.ScalarValue).Within(Tolerance), $"Tangent Y at t={t}");

            Assert.That(derivative1Float64.X.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), $"Float64 Tangent X should be 3.0 at t={t}");
            Assert.That(derivative1Float64.Y.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), $"Float64 Tangent Y should be 4.0 at t={t}");
        }
    }

    [Test]
    public void ConstantPath2D_Normal_ShouldBePerpendicular()
    {
        // Arrange
        var pointGeneric = CreateGenericVector(0.0, 0.0);
        var tangentGeneric = CreateGenericVector(1.0, 0.0); // X-axis

        var pathGeneric = ConstantPath2D<double>.Create(
            ScalarRange<double>.Create(ScalarProcessor.Scalar(-1.0), ScalarProcessor.Scalar(1.0)),
            false,
            pointGeneric,
            tangentGeneric
        );

        // Act
        var frame = pathGeneric.GetFrame(ScalarProcessor.Scalar(0.0));

        // Assert - Normal should be perpendicular to tangent (Y-axis for X-axis tangent)
        var dotProduct = frame.Tangent.X * frame.Normal.X + frame.Tangent.Y * frame.Normal.Y;
        Assert.That(dotProduct.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Normal should be perpendicular to Tangent (dot product = 0)");

        // For tangent (1, 0), normal should be (0, 1) or (0, -1)
        Assert.That(Math.Abs(frame.Normal.Y.ScalarValue), Is.EqualTo(1.0).Within(Tolerance), "Normal Y component magnitude should be 1.0");
    }

    [Test]
    public void ConstantPath2D_FrameTangent_ShouldBeNormalized()
    {
        // Arrange
        var pointGeneric = CreateGenericVector(1.0, 2.0);
        var tangentGeneric = CreateGenericVector(3.0, 4.0); // Not normalized

        var pathGeneric = ConstantPath2D<double>.Create(
            ScalarRange<double>.Create(ScalarProcessor.Scalar(-1.0), ScalarProcessor.Scalar(1.0)),
            false,
            pointGeneric,
            tangentGeneric
        );

        // Act
        var frame = pathGeneric.GetFrame(ScalarProcessor.Scalar(0.5));

        // Assert - Frame tangent should be normalized (unit vector)
        var tangentNormSquared = frame.Tangent.X * frame.Tangent.X + frame.Tangent.Y * frame.Tangent.Y;
        Assert.That(tangentNormSquared.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Frame Tangent should be normalized (norm = 1)");
    }

    #endregion
}
