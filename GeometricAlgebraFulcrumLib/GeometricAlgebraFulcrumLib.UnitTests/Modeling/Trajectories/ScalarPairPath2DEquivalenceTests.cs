using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Tests for Generic ScalarPairPath2D&lt;T&gt;
/// Phase 3 Module 6B - 2D Scalar Pair Parametric Paths
/// Tests: Composing 2D paths from separate X and Y scalar signals
/// </summary>
[TestFixture]
public class ScalarPairPath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    #endregion

    #region Linear Path Tests (2 tests)

    [Test]
    public void ScalarPairPath2D_LinearComponents_ShouldProduceLinearPath()
    {
        // Arrange - Linear path: x = 2t, y = 3t
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var xSignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => ScalarProcessor.Scalar(2.0) * t,
            t => ScalarProcessor.Scalar(2.0)  // dx/dt = 2
        );

        var ySignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => ScalarProcessor.Scalar(3.0) * t,
            t => ScalarProcessor.Scalar(3.0)  // dy/dt = 3
        );

        var path = ScalarPairPath2D<double>.Finite(timeRange, xSignal, ySignal);

        // Act & Assert
        var p0 = path.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(p0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=0");
        Assert.That(p0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=0");

        var p1 = path.GetValue(ScalarProcessor.Scalar(0.5));
        Assert.That(p1.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X at t=0.5");
        Assert.That(p1.Y.ScalarValue, Is.EqualTo(1.5).Within(Tolerance), "Y at t=0.5");

        var p2 = path.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(p2.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "X at t=1");
        Assert.That(p2.Y.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "Y at t=1");
    }

    [Test]
    public void ScalarPairPath2D_LinearComponents_DerivativeShouldBeConstant()
    {
        // Arrange - Linear path with constant derivatives
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var xSignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => ScalarProcessor.Scalar(5.0) * t,
            t => ScalarProcessor.Scalar(5.0),
            t => ScalarProcessor.Zero  // d²x/dt² = 0
        );

        var ySignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => ScalarProcessor.Scalar(7.0) * t,
            t => ScalarProcessor.Scalar(7.0),
            t => ScalarProcessor.Zero  // d²y/dt² = 0
        );

        var path = ScalarPairPath2D<double>.Finite(timeRange, xSignal, ySignal);

        // Act & Assert - Derivative should be constant (5, 7)
        for (var t = 0.0; t <= 1.0; t += 0.25)
        {
            var deriv = path.GetDerivative1Value(ScalarProcessor.Scalar(t));
            Assert.That(deriv.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), $"Derivative X at t={t}");
            Assert.That(deriv.Y.ScalarValue, Is.EqualTo(7.0).Within(Tolerance), $"Derivative Y at t={t}");
        }

        // Second derivative should be zero
        for (var t = 0.0; t <= 1.0; t += 0.25)
        {
            var deriv2 = path.GetDerivative2Value(ScalarProcessor.Scalar(t));
            Assert.That(deriv2.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), $"Second derivative X at t={t}");
            Assert.That(deriv2.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), $"Second derivative Y at t={t}");
        }
    }

    #endregion

    #region Circular Motion Tests (2 tests)

    [Test]
    public void ScalarPairPath2D_CircularMotion_ShouldProduceCircle()
    {
        // Arrange - Circle: x = cos(2πt), y = sin(2πt)
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var xSignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t =>
            {
                var angle = (ScalarProcessor.PiTimes2 * t).ScalarValue;
                return ScalarProcessor.Cos(angle);
            },
            t =>
            {
                var angle = (ScalarProcessor.PiTimes2 * t).ScalarValue;
                var omega = ScalarProcessor.PiTimes2.ScalarValue;
                return -ScalarProcessor.Sin(angle) * omega;
            }
        );

        var ySignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t =>
            {
                var angle = (ScalarProcessor.PiTimes2 * t).ScalarValue;
                return ScalarProcessor.Sin(angle);
            },
            t =>
            {
                var angle = (ScalarProcessor.PiTimes2 * t).ScalarValue;
                var omega = ScalarProcessor.PiTimes2.ScalarValue;
                return ScalarProcessor.Cos(angle) * omega;
            }
        );

        var path = ScalarPairPath2D<double>.Finite(timeRange, xSignal, ySignal);

        // Act & Assert - Check cardinal points
        var p0 = path.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(p0.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X at t=0");
        Assert.That(p0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=0");

        var p1 = path.GetValue(ScalarProcessor.Scalar(0.25));
        Assert.That(p1.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=0.25");
        Assert.That(p1.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Y at t=0.25");

        var p2 = path.GetValue(ScalarProcessor.Scalar(0.5));
        Assert.That(p2.X.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), "X at t=0.5");
        Assert.That(p2.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=0.5");

        var p3 = path.GetValue(ScalarProcessor.Scalar(0.75));
        Assert.That(p3.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=0.75");
        Assert.That(p3.Y.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), "Y at t=0.75");
    }

    [Test]
    public void ScalarPairPath2D_CircularMotion_RadiusShouldBeConstant()
    {
        // Arrange - Circle with radius 2
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var radius = 2.0;

        var xSignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t =>
            {
                var angle = (ScalarProcessor.PiTimes2 * t).ScalarValue;
                return ScalarProcessor.Cos(angle) * radius;
            }
        );

        var ySignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t =>
            {
                var angle = (ScalarProcessor.PiTimes2 * t).ScalarValue;
                return ScalarProcessor.Sin(angle) * radius;
            }
        );

        var path = ScalarPairPath2D<double>.Finite(timeRange, xSignal, ySignal);

        // Act & Assert - Radius should be 2 at all times
        for (var t = 0.0; t <= 1.0; t += 0.125)
        {
            var pos = path.GetValue(ScalarProcessor.Scalar(t));
            var actualRadius = Math.Sqrt(pos.X.ScalarValue * pos.X.ScalarValue +
                                         pos.Y.ScalarValue * pos.Y.ScalarValue);

            Assert.That(actualRadius, Is.EqualTo(radius).Within(Tolerance), $"Radius should be {radius} at t={t}");
        }
    }

    #endregion

    #region Parabolic Motion Tests (2 tests)

    [Test]
    public void ScalarPairPath2D_ParabolicMotion_ShouldProduceCorrectTrajectory()
    {
        // Arrange - Parabolic motion: x = t, y = t²
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var xSignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => t,
            t => ScalarProcessor.One,
            t => ScalarProcessor.Zero
        );

        var ySignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => t * t,
            t => ScalarProcessor.Scalar(2.0) * t,
            t => ScalarProcessor.Scalar(2.0)
        );

        var path = ScalarPairPath2D<double>.Finite(timeRange, xSignal, ySignal);

        // Act & Assert - Verify parabola points
        var p0 = path.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(p0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=0");
        Assert.That(p0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=0");

        var p1 = path.GetValue(ScalarProcessor.Scalar(0.5));
        Assert.That(p1.X.ScalarValue, Is.EqualTo(0.5).Within(Tolerance), "X at t=0.5");
        Assert.That(p1.Y.ScalarValue, Is.EqualTo(0.25).Within(Tolerance), "Y at t=0.5");

        var p2 = path.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(p2.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X at t=1");
        Assert.That(p2.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Y at t=1");

        // Verify second derivative (constant vertical acceleration)
        var deriv2 = path.GetDerivative2Value(ScalarProcessor.Scalar(0.5));
        Assert.That(deriv2.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "No horizontal acceleration");
        Assert.That(deriv2.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Constant vertical acceleration");
    }

    [Test]
    public void ScalarPairPath2D_GetScalarComponents_ShouldReturnOriginalSignals()
    {
        // Arrange
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var xSignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => t,
            t => ScalarProcessor.One
        );

        var ySignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => t * t,
            t => ScalarProcessor.Scalar(2.0) * t
        );

        var path = ScalarPairPath2D<double>.Finite(timeRange, xSignal, ySignal);

        // Act
        var (retrievedXSignal, retrievedYSignal) = path.GetScalarComponents();

        // Assert - The signals should be the same references (efficient override)
        Assert.That(ReferenceEquals(retrievedXSignal, xSignal), Is.True, "X signal should be same reference");
        Assert.That(ReferenceEquals(retrievedYSignal, ySignal), Is.True, "Y signal should be same reference");

        // Verify values match
        for (var t = 0.0; t <= 1.0; t += 0.25)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            Assert.That(retrievedXSignal.GetValue(tScalar).ScalarValue,
                Is.EqualTo(xSignal.GetValue(tScalar).ScalarValue).Within(Tolerance),
                $"Retrieved X signal value at t={t}");
            Assert.That(retrievedYSignal.GetValue(tScalar).ScalarValue,
                Is.EqualTo(ySignal.GetValue(tScalar).ScalarValue).Within(Tolerance),
                $"Retrieved Y signal value at t={t}");
        }
    }

    #endregion

    #region Frame and Conversion Tests (2 tests)

    [Test]
    public void ScalarPairPath2D_GetFrame_TangentShouldBeNormalized()
    {
        // Arrange
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var xSignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => ScalarProcessor.Scalar(3.0) * t,
            t => ScalarProcessor.Scalar(3.0)
        );

        var ySignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => ScalarProcessor.Scalar(4.0) * t,
            t => ScalarProcessor.Scalar(4.0)
        );

        var path = ScalarPairPath2D<double>.Finite(timeRange, xSignal, ySignal);

        // Act & Assert - Check tangent normalization
        for (var t = 0.0; t <= 1.0; t += 0.25)
        {
            var frame = path.GetFrame(ScalarProcessor.Scalar(t));

            var tangentNormSq = frame.Tangent.X.ScalarValue * frame.Tangent.X.ScalarValue +
                                frame.Tangent.Y.ScalarValue * frame.Tangent.Y.ScalarValue;

            Assert.That(tangentNormSq, Is.EqualTo(1.0).Within(Tolerance),
                $"Frame tangent should be normalized at t={t}");
        }
    }

    [Test]
    public void ScalarPairPath2D_Conversion_FiniteToPeriodicShouldCreateNewInstance()
    {
        // Arrange
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var xSignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => t,
            t => ScalarProcessor.One
        );

        var ySignal = ComputedScalarSignal<double>.Finite(
            timeRange,
            t => t,
            t => ScalarProcessor.One
        );

        var path = ScalarPairPath2D<double>.Finite(timeRange, xSignal, ySignal);

        // Act
        var periodicPath = path.ToPeriodicPath();

        // Assert
        Assert.That(ReferenceEquals(path, periodicPath), Is.False, "ToPeriodicPath should create new instance");
        Assert.That(periodicPath.IsPeriodic, Is.True, "Converted path should be periodic");
        Assert.That(path.IsFinite, Is.True, "Original path should remain finite");
    }

    #endregion

    #region Time Range Tests (1 test)

    [Test]
    public void ScalarPairPath2D_TimeRangeIntersection_ShouldUseCommonInterval()
    {
        // Arrange - Signals with different time ranges
        var xTimeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(2)
        );

        var yTimeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0.5),
            ScalarProcessor.Scalar(1.5)
        );

        var xSignal = ComputedScalarSignal<double>.Finite(
            xTimeRange,
            t => t,
            t => ScalarProcessor.One
        );

        var ySignal = ComputedScalarSignal<double>.Finite(
            yTimeRange,
            t => t,
            t => ScalarProcessor.One
        );

        // Act - Create path (should use intersection of time ranges)
        var path = ScalarPairPath2D<double>.Finite(xSignal, ySignal);

        // Assert - Time range should be [0.5, 1.5] (intersection)
        Assert.That(path.TimeRange.MinValue.ScalarValue, Is.EqualTo(0.5).Within(Tolerance), "Min time should be 0.5");
        Assert.That(path.TimeRange.MaxValue.ScalarValue, Is.EqualTo(1.5).Within(Tolerance), "Max time should be 1.5");

        // Verify path works within the intersection
        var p = path.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(p.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X at t=1.0");
        Assert.That(p.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Y at t=1.0");
    }

    #endregion
}
