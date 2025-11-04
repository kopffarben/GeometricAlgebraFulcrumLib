using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Tests for Generic PolarPath2D<T>
/// Phase 3 Module 6B - 2D Polar Parametric Curves
/// Tests: Mathematical correctness of polar coordinate transformations and derivatives
/// </summary>
[TestFixture]
public class PolarPath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    #endregion

    #region Circular Motion Tests (3 tests)

    [Test]
    public void PolarPath2D_CircularMotion_ShouldProduceCircle()
    {
        // Arrange - Circle: r(t) = 1, theta(t) = 2πt for t in [0,1]
        var timeRange = ScalarRange<double>.Create(ScalarProcessor.Scalar(0), ScalarProcessor.Scalar(1));

        var rPath = ConstantScalarSignal<double>.Finite(timeRange, ScalarProcessor.Scalar(1.0));

        var thetaPath = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => ScalarProcessor.PiTimes2 * t,  // theta = 2πt
            t => ScalarProcessor.PiTimes2        // dtheta/dt = 2π
        );

        var path = PolarPath2D<double>.Finite(timeRange, rPath, thetaPath);

        // Act & Assert - At t=0, should be at (1, 0)
        var p0 = path.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(p0.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X at t=0");
        Assert.That(p0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=0");

        // At t=0.25, should be at (0, 1)
        var p1 = path.GetValue(ScalarProcessor.Scalar(0.25));
        Assert.That(p1.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=0.25");
        Assert.That(p1.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Y at t=0.25");

        // At t=0.5, should be at (-1, 0)
        var p2 = path.GetValue(ScalarProcessor.Scalar(0.5));
        Assert.That(p2.X.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), "X at t=0.5");
        Assert.That(p2.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=0.5");

        // At t=0.75, should be at (0, -1)
        var p3 = path.GetValue(ScalarProcessor.Scalar(0.75));
        Assert.That(p3.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=0.75");
        Assert.That(p3.Y.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), "Y at t=0.75");
    }

    [Test]
    public void PolarPath2D_CircularMotion_DerivativeShouldBeTangent()
    {
        // Arrange - Circle: r = 2, theta = 2πt
        var timeRange = ScalarRange<double>.Create(ScalarProcessor.Scalar(0), ScalarProcessor.Scalar(1));

        var rPath = ConstantScalarSignal<double>.Finite(timeRange, ScalarProcessor.Scalar(2.0));

        var thetaPath = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => ScalarProcessor.PiTimes2 * t,
            t => ScalarProcessor.PiTimes2
        );

        var path = PolarPath2D<double>.Finite(timeRange, rPath, thetaPath);

        // Act & Assert - First derivative should be perpendicular to radius
        // For a circle, tangent velocity has constant magnitude = radius * angular_velocity
        // Magnitude = 2 * 2π = 4π
        var expectedMagnitude = 2.0 * 2.0 * Math.PI;

        for (var t = 0.0; t <= 1.0; t += 0.125)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var deriv = path.GetDerivative1Value(tScalar);
            var derivMagnitude = Math.Sqrt(deriv.X.ScalarValue * deriv.X.ScalarValue +
                                            deriv.Y.ScalarValue * deriv.Y.ScalarValue);

            Assert.That(derivMagnitude, Is.EqualTo(expectedMagnitude).Within(Tolerance),
                $"Derivative magnitude at t={t} should be constant");
        }
    }

    [Test]
    public void PolarPath2D_CircularMotion_SecondDerivativeShouldPointToCenter()
    {
        // Arrange - Circle: r = 1.5, theta = 2πt
        var timeRange = ScalarRange<double>.Create(ScalarProcessor.Scalar(0), ScalarProcessor.Scalar(1));

        var rPath = ConstantScalarSignal<double>.Finite(timeRange, ScalarProcessor.Scalar(1.5));

        var thetaPath = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => ScalarProcessor.PiTimes2 * t,
            t => ScalarProcessor.PiTimes2,
            t => ScalarProcessor.Zero  // d²theta/dt² = 0
        );

        var path = PolarPath2D<double>.Finite(timeRange, rPath, thetaPath);

        // Act & Assert - Second derivative should point toward center (centripetal acceleration)
        // For circular motion: a = -ω²r where ω = 2π
        var omega = 2.0 * Math.PI;
        var expectedAccelMagnitude = omega * omega * 1.5;

        for (var t = 0.0; t <= 1.0; t += 0.125)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var pos = path.GetValue(tScalar);
            var deriv2 = path.GetDerivative2Value(tScalar);

            // Acceleration should point opposite to position (toward center)
            var dotProduct = pos.X.ScalarValue * deriv2.X.ScalarValue +
                            pos.Y.ScalarValue * deriv2.Y.ScalarValue;

            // Should be negative (pointing inward) with magnitude = ω²r²
            Assert.That(dotProduct, Is.LessThan(0), $"Acceleration should point inward at t={t}");
            Assert.That(Math.Abs(dotProduct), Is.EqualTo(expectedAccelMagnitude * 1.5).Within(Tolerance),
                $"Dot product magnitude at t={t}");
        }
    }

    #endregion

    #region Spiral Tests (2 tests)

    [Test]
    public void PolarPath2D_ArchimedeanSpiral_RadiusShouldIncreaseLinearly()
    {
        // Arrange - Archimedean spiral: r(t) = t, theta(t) = 4πt (2 turns)
        var timeRange = ScalarRange<double>.Create(ScalarProcessor.Scalar(0), ScalarProcessor.Scalar(1));

        var rPath = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => t,  // r = t
            t => ScalarProcessor.One  // dr/dt = 1
        );

        var thetaPath = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => ScalarProcessor.PiTimes2 * ScalarProcessor.Scalar(2.0) * t,  // theta = 4πt
            t => ScalarProcessor.PiTimes2 * ScalarProcessor.Scalar(2.0)       // dtheta/dt = 4π
        );

        var path = PolarPath2D<double>.Finite(timeRange, rPath, thetaPath);

        // Act & Assert - Radius from origin should equal t
        for (var t = 0.125; t <= 1.0; t += 0.125) // Skip t=0
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var pos = path.GetValue(tScalar);
            var radius = Math.Sqrt(pos.X.ScalarValue * pos.X.ScalarValue +
                                    pos.Y.ScalarValue * pos.Y.ScalarValue);

            Assert.That(radius, Is.EqualTo(t).Within(Tolerance), $"Radius should equal t at t={t}");
        }
    }

    [Test]
    public void PolarPath2D_Cardioid_ShouldHaveCuspAtOrigin()
    {
        // Arrange - Cardioid: r(t) = 1 + cos(theta), theta(t) = 2πt
        var timeRange = ScalarRange<double>.Create(ScalarProcessor.Scalar(0), ScalarProcessor.Scalar(1));

        var thetaPath = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => ScalarProcessor.PiTimes2 * t,
            t => ScalarProcessor.PiTimes2
        );

        var rPath = ComputedScalarSignal<double>.Finite(
            timeRange,
            t =>
            {
                var theta = (ScalarProcessor.PiTimes2 * t).ScalarValue;
                return ScalarProcessor.Scalar(1.0) + ScalarProcessor.Cos(theta);
            },
            t =>
            {
                var theta = (ScalarProcessor.PiTimes2 * t).ScalarValue;
                var thetaDt = ScalarProcessor.PiTimes2.ScalarValue;
                return -ScalarProcessor.Sin(theta) * thetaDt;  // dr/dt = -sin(theta) * dtheta/dt
            }
        );

        var path = PolarPath2D<double>.Finite(timeRange, rPath, thetaPath);

        // Act & Assert - At t=0.5 (theta=π), r should be 0 (cusp at origin)
        var p = path.GetValue(ScalarProcessor.Scalar(0.5));
        var radius = Math.Sqrt(p.X.ScalarValue * p.X.ScalarValue + p.Y.ScalarValue * p.Y.ScalarValue);

        Assert.That(radius, Is.EqualTo(0.0).Within(Tolerance), "Cardioid should touch origin at theta=π");
    }

    #endregion

    #region Frame and Conversion Tests (2 tests)

    [Test]
    public void PolarPath2D_GetFrame_TangentShouldBeNormalized()
    {
        // Arrange
        var timeRange = ScalarRange<double>.Create(ScalarProcessor.Scalar(0), ScalarProcessor.Scalar(1));

        var rPath = ConstantScalarSignal<double>.Finite(timeRange, ScalarProcessor.Scalar(2.0));

        var thetaPath = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => ScalarProcessor.PiTimes2 * t,
            t => ScalarProcessor.PiTimes2
        );

        var path = PolarPath2D<double>.Finite(timeRange, rPath, thetaPath);

        // Act & Assert - Check tangent normalization
        for (var t = 0.125; t <= 1.0; t += 0.125)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var frame = path.GetFrame(tScalar);

            var tangentNormSq = frame.Tangent.X.ScalarValue * frame.Tangent.X.ScalarValue +
                                frame.Tangent.Y.ScalarValue * frame.Tangent.Y.ScalarValue;

            Assert.That(tangentNormSq, Is.EqualTo(1.0).Within(Tolerance),
                $"Frame tangent should be normalized at t={t}");
        }
    }

    [Test]
    public void PolarPath2D_Conversion_FiniteToPeriodicShouldCreateNewInstance()
    {
        // Arrange
        var timeRange = ScalarRange<double>.Create(ScalarProcessor.Scalar(0), ScalarProcessor.Scalar(1));

        var rPath = ConstantScalarSignal<double>.Finite(timeRange, ScalarProcessor.Scalar(1.0));

        var thetaPath = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => ScalarProcessor.PiTimes2 * t,
            t => ScalarProcessor.PiTimes2
        );

        var path = PolarPath2D<double>.Finite(timeRange, rPath, thetaPath);

        // Act
        var periodicPath = path.ToPeriodicPath();

        // Assert
        Assert.That(ReferenceEquals(path, periodicPath), Is.False, "ToPeriodicPath should create new instance");
        Assert.That(periodicPath.IsPeriodic, Is.True, "Converted path should be periodic");
        Assert.That(path.IsFinite, Is.True, "Original path should remain finite");
    }

    #endregion
}
