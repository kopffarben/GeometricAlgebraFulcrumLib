using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Tests for Generic ComputedPath2D<T>
/// Phase 3 Module 6B - 2D Function-Based Parametric Paths
/// Tests: Function-based path definition and derivative handling
/// </summary>
[TestFixture]
public class ComputedPath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    #endregion

    #region Linear Path Tests (2 tests)

    [Test]
    public void ComputedPath2D_LinearPath_ShouldProduceCorrectValues()
    {
        // Arrange - Linear path: (t, 2t) for t in [0,1]
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var path = ComputedPath2D<double>.Finite(
            timeRange,
            t => LinVector2D<double>.Create(
                t.ScalarProcessor,
                t.ScalarValue,
                2.0 * t.ScalarValue
            ),
            t => LinVector2D<double>.Create(
                t.ScalarProcessor,
                1.0,
                2.0
            )  // Derivative is constant (1, 2)
        );

        // Act & Assert
        var p0 = path.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(p0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=0");
        Assert.That(p0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=0");

        var p1 = path.GetValue(ScalarProcessor.Scalar(0.5));
        Assert.That(p1.X.ScalarValue, Is.EqualTo(0.5).Within(Tolerance), "X at t=0.5");
        Assert.That(p1.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Y at t=0.5");

        var p2 = path.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(p2.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X at t=1");
        Assert.That(p2.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Y at t=1");
    }

    [Test]
    public void ComputedPath2D_LinearPath_DerivativeShouldBeConstant()
    {
        // Arrange - Linear path with constant derivative
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var path = ComputedPath2D<double>.Finite(
            timeRange,
            t => LinVector2D<double>.Create(
                t.ScalarProcessor,
                3.0 * t.ScalarValue,
                4.0 * t.ScalarValue
            ),
            t => LinVector2D<double>.Create(
                t.ScalarProcessor,
                3.0,
                4.0
            )
        );

        // Act & Assert - Derivative should be constant (3, 4)
        for (var t = 0.0; t <= 1.0; t += 0.25)
        {
            var deriv = path.GetDerivative1Value(ScalarProcessor.Scalar(t));
            Assert.That(deriv.X.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), $"Derivative X at t={t}");
            Assert.That(deriv.Y.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), $"Derivative Y at t={t}");
        }
    }

    #endregion

    #region Circular Path Tests (2 tests)

    [Test]
    public void ComputedPath2D_CircularPath_ShouldProduceCircle()
    {
        // Arrange - Parametric circle: (cos(2πt), sin(2πt))
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var path = ComputedPath2D<double>.Finite(
            timeRange,
            t =>
            {
                var angle = (ScalarProcessor.PiTimes2 * t).ScalarValue;
                return LinVector2D<double>.Create(
                    t.ScalarProcessor,
                    t.ScalarProcessor.Cos(angle).ScalarValue,
                    t.ScalarProcessor.Sin(angle).ScalarValue
                );
            },
            t =>
            {
                var angle = (ScalarProcessor.PiTimes2 * t).ScalarValue;
                var omega = ScalarProcessor.PiTimes2.ScalarValue;
                return LinVector2D<double>.Create(
                    t.ScalarProcessor,
                    -omega * t.ScalarProcessor.Sin(angle).ScalarValue,
                    omega * t.ScalarProcessor.Cos(angle).ScalarValue
                );
            }
        );

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
    }

    [Test]
    public void ComputedPath2D_CircularPath_RadiusShouldBeOne()
    {
        // Arrange - Circle with radius 1
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var path = ComputedPath2D<double>.Finite(
            timeRange,
            t =>
            {
                var angle = (ScalarProcessor.PiTimes2 * t).ScalarValue;
                return LinVector2D<double>.Create(
                    t.ScalarProcessor,
                    t.ScalarProcessor.Cos(angle).ScalarValue,
                    t.ScalarProcessor.Sin(angle).ScalarValue
                );
            }
        );

        // Act & Assert - Radius should be 1 at all times
        for (var t = 0.0; t <= 1.0; t += 0.125)
        {
            var pos = path.GetValue(ScalarProcessor.Scalar(t));
            var radius = Math.Sqrt(pos.X.ScalarValue * pos.X.ScalarValue +
                                    pos.Y.ScalarValue * pos.Y.ScalarValue);

            Assert.That(radius, Is.EqualTo(1.0).Within(Tolerance), $"Radius should be 1 at t={t}");
        }
    }

    #endregion

    #region Lissajous Curve Tests (2 tests)

    [Test]
    public void ComputedPath2D_LissajousCurve_ShouldProduceCorrectValues()
    {
        // Arrange - Lissajous curve: (sin(2πt), sin(4πt))
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var path = ComputedPath2D<double>.Finite(
            timeRange,
            t =>
            {
                var angle1 = (ScalarProcessor.PiTimes2 * t).ScalarValue;
                var angle2 = (ScalarProcessor.PiTimes2 * ScalarProcessor.Scalar(2.0) * t).ScalarValue;
                return LinVector2D<double>.Create(
                    t.ScalarProcessor,
                    t.ScalarProcessor.Sin(angle1).ScalarValue,
                    t.ScalarProcessor.Sin(angle2).ScalarValue
                );
            }
        );

        // Act & Assert - Check specific points
        var p0 = path.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(p0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=0");
        Assert.That(p0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=0");

        var p1 = path.GetValue(ScalarProcessor.Scalar(0.25));
        Assert.That(p1.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X at t=0.25");
        Assert.That(p1.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=0.25");

        var p2 = path.GetValue(ScalarProcessor.Scalar(0.5));
        Assert.That(p2.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=0.5");
        Assert.That(p2.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=0.5");
    }

    [Test]
    public void ComputedPath2D_ParametricParabola_ShouldProduceCorrectShape()
    {
        // Arrange - Parametric parabola: (t, t²)
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(-1),
            ScalarProcessor.Scalar(1)
        );

        var path = ComputedPath2D<double>.Finite(
            timeRange,
            t => LinVector2D<double>.Create(
                t.ScalarProcessor,
                t.ScalarValue,
                t.ScalarValue * t.ScalarValue
            ),
            t => LinVector2D<double>.Create(
                t.ScalarProcessor,
                1.0,
                2.0 * t.ScalarValue
            ),
            t => LinVector2D<double>.Create(
                t.ScalarProcessor,
                0.0,
                2.0
            )
        );

        // Act & Assert - Verify parabola points
        var p0 = path.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(p0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=0");
        Assert.That(p0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=0 (vertex)");

        var p1 = path.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(p1.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X at t=1");
        Assert.That(p1.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Y at t=1");

        var p2 = path.GetValue(ScalarProcessor.Scalar(-1.0));
        Assert.That(p2.X.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), "X at t=-1");
        Assert.That(p2.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Y at t=-1");

        // Check second derivative is constant (0, 2)
        var deriv2 = path.GetDerivative2Value(ScalarProcessor.Scalar(0.5));
        Assert.That(deriv2.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative X");
        Assert.That(deriv2.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Second derivative Y");
    }

    #endregion

    #region Frame and Conversion Tests (2 tests)

    [Test]
    public void ComputedPath2D_GetFrame_TangentShouldBeNormalized()
    {
        // Arrange - Simple linear path
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var path = ComputedPath2D<double>.Finite(
            timeRange,
            t => LinVector2D<double>.Create(
                t.ScalarProcessor,
                t.ScalarValue,
                t.ScalarValue
            ),
            t => LinVector2D<double>.Create(
                t.ScalarProcessor,
                1.0,
                1.0
            )
        );

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
    public void ComputedPath2D_Conversion_FiniteToPeriodicShouldCreateNewInstance()
    {
        // Arrange
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var path = ComputedPath2D<double>.Finite(
            timeRange,
            t => LinVector2D<double>.Create(
                t.ScalarProcessor,
                t.ScalarValue,
                t.ScalarValue
            )
        );

        // Act
        var periodicPath = path.ToPeriodicPath();

        // Assert
        Assert.That(ReferenceEquals(path, periodicPath), Is.False, "ToPeriodicPath should create new instance");
        Assert.That(periodicPath.IsPeriodic, Is.True, "Converted path should be periodic");
        Assert.That(path.IsFinite, Is.True, "Original path should remain finite");
    }

    #endregion

    #region Exception Tests (1 test)

    [Test]
    public void ComputedPath2D_WithoutDerivativeFunc_ShouldThrowOnDerivativeCall()
    {
        // Arrange - Path without derivative function
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var path = ComputedPath2D<double>.Finite(
            timeRange,
            t => LinVector2D<double>.Create(
                t.ScalarProcessor,
                t.ScalarValue,
                t.ScalarValue
            )
        );

        // Act & Assert - NOW WORKS with INumericalOperations<T> fallback!
        var deriv1 = path.GetDerivative1Value(ScalarProcessor.Scalar(0.5));
        Assert.That(deriv1, Is.Not.Null, "Derivative should be computed via numerical differentiation");

        var deriv2 = path.GetDerivative2Value(ScalarProcessor.Scalar(0.5));
        Assert.That(deriv2, Is.Not.Null, "Second derivative should be computed via numerical differentiation");
    }

    #endregion

    #region Phase 1: INumericalOperations<T> Fallback Tests

    [Test]
    public void ComputedPath2D_GetDerivative1Value_WithoutFunction_UsesNumericalDifferentiation()
    {
        // Test INumericalOperations<T> fallback - simple smoke test
        var timeRangeGeneric = ScalarRange<double>.Create(ScalarProcessor.Zero, ScalarProcessor.One);

        var pathGeneric = ComputedPath2D<double>.Finite(
            timeRangeGeneric,
            t => LinVector2D<double>.Create(t * t, ScalarProcessor.ScalarFromNumber(2) * t * t)
        );

        // Should not throw - uses INumericalOperations<T> fallback
        var deriv1Generic = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(0.5));

        Assert.That(deriv1Generic, Is.Not.Null, "Derivative should be computed via numerical differentiation");
        Assert.That(deriv1Generic.X.ScalarValue, Is.Not.NaN, "X derivative should be valid");
        Assert.That(deriv1Generic.Y.ScalarValue, Is.Not.NaN, "Y derivative should be valid");

        // Expected derivative at t=0.5: (2*0.5, 4*0.5) = (1, 2)
        Assert.That(deriv1Generic.X.ScalarValue, Is.EqualTo(1.0).Within(1e-6), "X derivative should be ~1.0");
        Assert.That(deriv1Generic.Y.ScalarValue, Is.EqualTo(2.0).Within(1e-6), "Y derivative should be ~2.0");
    }

    [Test]
    public void ComputedPath2D_GetDerivative2Value_WithoutFunction_UsesNumericalDifferentiation()
    {
        // Test INumericalOperations<T> fallback for second derivative - simple smoke test
        var timeRangeGeneric = ScalarRange<double>.Create(ScalarProcessor.Zero, ScalarProcessor.One);

        var pathGeneric = ComputedPath2D<double>.Finite(
            timeRangeGeneric,
            t => LinVector2D<double>.Create(t * t, ScalarProcessor.ScalarFromNumber(2) * t * t)
        );

        // Should not throw - uses INumericalOperations<T> fallback
        var deriv2Generic = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(0.5));

        Assert.That(deriv2Generic, Is.Not.Null, "Second derivative should be computed via numerical differentiation");
        Assert.That(deriv2Generic.X.ScalarValue, Is.Not.NaN, "X second derivative should be valid");
        Assert.That(deriv2Generic.Y.ScalarValue, Is.Not.NaN, "Y second derivative should be valid");

        // Expected second derivative: constant (2, 4)
        Assert.That(deriv2Generic.X.ScalarValue, Is.EqualTo(2.0).Within(1e-4), "X second derivative should be ~2.0");
        Assert.That(deriv2Generic.Y.ScalarValue, Is.EqualTo(4.0).Within(1e-4), "Y second derivative should be ~4.0");
    }

    [Test]
    public void ComputedPath2D_GetDerivative1Value_ComplexFunction_UsesNumericalDifferentiation()
    {
        // Test with complex function: (sin(t), cos(t))
        var timeRangeGeneric = ScalarRange<double>.Create(
            ScalarProcessor.Zero,
            ScalarProcessor.ScalarFromNumber(Math.PI)
        );

        var pathGeneric = ComputedPath2D<double>.Finite(
            timeRangeGeneric,
            t =>
            {
                var tVal = t.ScalarValue;
                return LinVector2D<double>.Create(
                    ScalarProcessor.ScalarFromNumber(Math.Sin(tVal)),
                    ScalarProcessor.ScalarFromNumber(Math.Cos(tVal))
                );
            }
        );

        var tTest = Math.PI / 4.0;

        // Should not throw - uses INumericalOperations<T> fallback
        var deriv1Generic = pathGeneric.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(tTest));

        Assert.That(deriv1Generic, Is.Not.Null, "Derivative should be computed via numerical differentiation");
        Assert.That(deriv1Generic.X.ScalarValue, Is.Not.NaN, "X derivative should be valid");
        Assert.That(deriv1Generic.Y.ScalarValue, Is.Not.NaN, "Y derivative should be valid");

        // Expected derivative at t=π/4: (cos(π/4), -sin(π/4)) ≈ (0.707, -0.707)
        const double tolerance = 1e-6;
        Assert.That(deriv1Generic.X.ScalarValue, Is.EqualTo(Math.Cos(tTest)).Within(tolerance));
        Assert.That(deriv1Generic.Y.ScalarValue, Is.EqualTo(-Math.Sin(tTest)).Within(tolerance));
    }

    #endregion
}
