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
/// Equivalence Tests for Generic SimpleHarmonicPath2D vs Float64 SimpleHarmonicPath2D
/// Phase 3 Module 6B - 2D Simple Harmonic Motion
/// Tests: Generic double vs Float64 Specialized
/// </summary>
[TestFixture]
public class SimpleHarmonicPath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static LinVector2D<double> CreateGenericVector(double x, double y)
    {
        return LinVector2D<double>.Create(ScalarProcessor, x, y);
    }

    #endregion

    #region Harmonic Factor 1 Tests (5 tests)

    [Test]
    public void SimpleHarmonicPath2D_GetValue_ShouldMatchFloat64()
    {
        // Arrange - Simple harmonic motion with harmonic factor 1
        var magnitudeFloat64 = LinFloat64Vector2D.Create(2.0, 3.0);
        var timeOffsetFloat64 = LinFloat64Vector2D.Create(0.0, 0.0);

        var magnitudeGeneric = CreateGenericVector(2.0, 3.0);
        var timeOffsetGeneric = CreateGenericVector(0.0, 0.0);

        var pathFloat64 = Float64SimpleHarmonicPath2D.Finite(1, magnitudeFloat64, timeOffsetFloat64);
        var pathGeneric = SimpleHarmonicPath2D<double>.Create(ScalarProcessor, false, 1, magnitudeGeneric, timeOffsetGeneric);

        // Act & Assert - Compare at multiple t values
        for (var t = -Math.PI; t <= Math.PI; t += Math.PI / 4)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = pathFloat64.GetValue(t);
            var valueGeneric = pathGeneric.GetValue(tScalar);

            Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance), $"X at t={t}");
            Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance), $"Y at t={t}");
        }
    }

    [Test]
    public void SimpleHarmonicPath2D_GetValue_AtT0_ShouldBeMagnitude()
    {
        // Arrange - At t=0 with zero offset, should equal magnitude
        var magnitudeGeneric = CreateGenericVector(5.0, 7.0);
        var timeOffsetGeneric = CreateGenericVector(0.0, 0.0);

        var pathGeneric = SimpleHarmonicPath2D<double>.Create(ScalarProcessor, false, 1, magnitudeGeneric, timeOffsetGeneric);

        // Act
        var value = pathGeneric.GetValue(ScalarProcessor.Scalar(0.0));

        // Assert - cos(0) = 1, so value should equal magnitude
        Assert.That(value.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "X at t=0");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(7.0).Within(Tolerance), "Y at t=0");
    }

    [Test]
    public void SimpleHarmonicPath2D_GetDerivative1Value_ShouldMatchFloat64()
    {
        // Arrange
        var magnitudeFloat64 = LinFloat64Vector2D.Create(1.5, 2.5);
        var timeOffsetFloat64 = LinFloat64Vector2D.Create(0.5, -0.5);

        var magnitudeGeneric = CreateGenericVector(1.5, 2.5);
        var timeOffsetGeneric = CreateGenericVector(0.5, -0.5);

        var pathFloat64 = Float64SimpleHarmonicPath2D.Finite(1, magnitudeFloat64, timeOffsetFloat64);
        var pathGeneric = SimpleHarmonicPath2D<double>.Create(ScalarProcessor, false, 1, magnitudeGeneric, timeOffsetGeneric);

        // Act & Assert - Compare first derivative at multiple t values
        for (var t = -Math.PI; t <= Math.PI; t += Math.PI / 4)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var derivFloat64 = pathFloat64.GetDerivative1Value(t);
            var derivGeneric = pathGeneric.GetDerivative1Value(tScalar);

            Assert.That(derivGeneric.X.ScalarValue, Is.EqualTo(derivFloat64.X.ScalarValue).Within(Tolerance), $"Derivative1 X at t={t}");
            Assert.That(derivGeneric.Y.ScalarValue, Is.EqualTo(derivFloat64.Y.ScalarValue).Within(Tolerance), $"Derivative1 Y at t={t}");
        }
    }

    [Test]
    public void SimpleHarmonicPath2D_GetDerivative2Value_ShouldMatchFloat64()
    {
        // Arrange
        var magnitudeFloat64 = LinFloat64Vector2D.Create(3.0, 4.0);
        var timeOffsetFloat64 = LinFloat64Vector2D.Create(0.0, 0.0);

        var magnitudeGeneric = CreateGenericVector(3.0, 4.0);
        var timeOffsetGeneric = CreateGenericVector(0.0, 0.0);

        var pathFloat64 = Float64SimpleHarmonicPath2D.Finite(1, magnitudeFloat64, timeOffsetFloat64);
        var pathGeneric = SimpleHarmonicPath2D<double>.Create(ScalarProcessor, false, 1, magnitudeGeneric, timeOffsetGeneric);

        // Act & Assert - Compare second derivative at multiple t values
        for (var t = -Math.PI; t <= Math.PI; t += Math.PI / 4)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var deriv2Float64 = pathFloat64.GetDerivative2Value(t);
            var deriv2Generic = pathGeneric.GetDerivative2Value(tScalar);

            Assert.That(deriv2Generic.X.ScalarValue, Is.EqualTo(deriv2Float64.X.ScalarValue).Within(Tolerance), $"Derivative2 X at t={t}");
            Assert.That(deriv2Generic.Y.ScalarValue, Is.EqualTo(deriv2Float64.Y.ScalarValue).Within(Tolerance), $"Derivative2 Y at t={t}");
        }
    }

    [Test]
    public void SimpleHarmonicPath2D_GetFrame_TangentShouldBeNormalized()
    {
        // Arrange
        var magnitudeGeneric = CreateGenericVector(2.0, 3.0);
        var timeOffsetGeneric = CreateGenericVector(0.0, 0.0);

        var pathGeneric = SimpleHarmonicPath2D<double>.Create(ScalarProcessor, false, 1, magnitudeGeneric, timeOffsetGeneric);

        // Act & Assert - Check tangent normalization at multiple points
        for (var t = -Math.PI; t <= Math.PI; t += Math.PI / 4)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var frame = pathGeneric.GetFrame(tScalar);

            var tangentNormSq = frame.Tangent.X.ScalarValue * frame.Tangent.X.ScalarValue +
                                frame.Tangent.Y.ScalarValue * frame.Tangent.Y.ScalarValue;

            Assert.That(tangentNormSq, Is.EqualTo(1.0).Within(Tolerance), $"Frame tangent should be normalized at t={t}");
        }
    }

    #endregion

    #region Harmonic Factor > 1 Tests (3 tests)

    [Test]
    public void SimpleHarmonicPath2D_HarmonicFactor2_ShouldMatchFloat64()
    {
        // Arrange - Harmonic factor 2 means double frequency
        var magnitudeFloat64 = LinFloat64Vector2D.Create(1.0, 1.0);
        var timeOffsetFloat64 = LinFloat64Vector2D.Create(0.0, 0.0);

        var magnitudeGeneric = CreateGenericVector(1.0, 1.0);
        var timeOffsetGeneric = CreateGenericVector(0.0, 0.0);

        var pathFloat64 = Float64SimpleHarmonicPath2D.Finite(2, magnitudeFloat64, timeOffsetFloat64);
        var pathGeneric = SimpleHarmonicPath2D<double>.Create(ScalarProcessor, false, 2, magnitudeGeneric, timeOffsetGeneric);

        // Act & Assert - Compare at multiple t values
        for (var t = -Math.PI; t <= Math.PI; t += Math.PI / 8)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = pathFloat64.GetValue(t);
            var valueGeneric = pathGeneric.GetValue(tScalar);

            Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance), $"X at t={t}, harmonic factor 2");
            Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance), $"Y at t={t}, harmonic factor 2");
        }
    }

    [Test]
    public void SimpleHarmonicPath2D_HarmonicFactor3_DerivativesShouldMatch()
    {
        // Arrange - Harmonic factor 3
        var magnitudeFloat64 = LinFloat64Vector2D.Create(2.5, 1.5);
        var timeOffsetFloat64 = LinFloat64Vector2D.Create(0.25, -0.25);

        var magnitudeGeneric = CreateGenericVector(2.5, 1.5);
        var timeOffsetGeneric = CreateGenericVector(0.25, -0.25);

        var pathFloat64 = Float64SimpleHarmonicPath2D.Finite(3, magnitudeFloat64, timeOffsetFloat64);
        var pathGeneric = SimpleHarmonicPath2D<double>.Create(ScalarProcessor, false, 3, magnitudeGeneric, timeOffsetGeneric);

        // Act & Assert - Compare derivatives
        for (var t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var tScalar = ScalarProcessor.Scalar(t);

            var deriv1Float64 = pathFloat64.GetDerivative1Value(t);
            var deriv1Generic = pathGeneric.GetDerivative1Value(tScalar);

            Assert.That(deriv1Generic.X.ScalarValue, Is.EqualTo(deriv1Float64.X.ScalarValue).Within(Tolerance), $"Derivative1 X at t={t}");
            Assert.That(deriv1Generic.Y.ScalarValue, Is.EqualTo(deriv1Float64.Y.ScalarValue).Within(Tolerance), $"Derivative1 Y at t={t}");

            var deriv2Float64 = pathFloat64.GetDerivative2Value(t);
            var deriv2Generic = pathGeneric.GetDerivative2Value(tScalar);

            Assert.That(deriv2Generic.X.ScalarValue, Is.EqualTo(deriv2Float64.X.ScalarValue).Within(Tolerance), $"Derivative2 X at t={t}");
            Assert.That(deriv2Generic.Y.ScalarValue, Is.EqualTo(deriv2Float64.Y.ScalarValue).Within(Tolerance), $"Derivative2 Y at t={t}");
        }
    }

    [Test]
    public void SimpleHarmonicPath2D_WithTimeOffset_ShouldMatchFloat64()
    {
        // Arrange - Test time offset effect
        var magnitudeFloat64 = LinFloat64Vector2D.Create(3.0, 2.0);
        var timeOffsetFloat64 = LinFloat64Vector2D.Create(0.5, -0.3);

        var magnitudeGeneric = CreateGenericVector(3.0, 2.0);
        var timeOffsetGeneric = CreateGenericVector(0.5, -0.3);

        var pathFloat64 = Float64SimpleHarmonicPath2D.Finite(1, magnitudeFloat64, timeOffsetFloat64);
        var pathGeneric = SimpleHarmonicPath2D<double>.Create(ScalarProcessor, false, 1, magnitudeGeneric, timeOffsetGeneric);

        // Act & Assert - Time offset shifts the phase
        for (var t = -Math.PI; t <= Math.PI; t += Math.PI / 4)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = pathFloat64.GetValue(t);
            var valueGeneric = pathGeneric.GetValue(tScalar);

            Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance), $"X at t={t} with time offset");
            Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance), $"Y at t={t} with time offset");
        }
    }

    #endregion

    #region Conversion Tests (2 tests)

    [Test]
    public void SimpleHarmonicPath2D_ToFinitePath_ShouldBeIdempotent()
    {
        // Arrange
        var magnitudeGeneric = CreateGenericVector(1.0, 1.0);
        var timeOffsetGeneric = CreateGenericVector(0.0, 0.0);

        var pathGeneric = SimpleHarmonicPath2D<double>.Create(ScalarProcessor, false, 1, magnitudeGeneric, timeOffsetGeneric);

        // Act
        var finitePath = pathGeneric.ToFinitePath();

        // Assert - Should return same instance if already finite
        Assert.That(ReferenceEquals(pathGeneric, finitePath), Is.True, "ToFinitePath should return same instance when already finite");
        Assert.That(finitePath.IsFinite, Is.True, "IsFinite should be true");
    }

    [Test]
    public void SimpleHarmonicPath2D_ToPeriodicPath_ShouldConvert()
    {
        // Arrange
        var magnitudeGeneric = CreateGenericVector(2.0, 3.0);
        var timeOffsetGeneric = CreateGenericVector(0.0, 0.0);

        var pathGeneric = SimpleHarmonicPath2D<double>.Create(ScalarProcessor, false, 1, magnitudeGeneric, timeOffsetGeneric);

        // Act
        var periodicPath = pathGeneric.ToPeriodicPath();

        // Assert - Should create new periodic instance
        Assert.That(ReferenceEquals(pathGeneric, periodicPath), Is.False, "ToPeriodicPath should create new instance");
        Assert.That(periodicPath.IsPeriodic, Is.True, "Converted path should be periodic");
        Assert.That(pathGeneric.IsFinite, Is.True, "Original path should remain finite");
    }

    #endregion
}
