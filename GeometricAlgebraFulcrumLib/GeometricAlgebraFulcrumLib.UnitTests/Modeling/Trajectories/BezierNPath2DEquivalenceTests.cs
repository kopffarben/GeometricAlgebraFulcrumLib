using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Bezier;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Tests für Generic BezierNPath2D&lt;T&gt;
/// Phase 3 Module 6B - 2D Bezier Curves of Arbitrary Degree
/// Tests: De Casteljau Algorithmus, Ableitungskurven, hohe Grade
/// </summary>
[TestFixture]
public class BezierNPath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    #endregion

    #region Degree 4 Curve Tests (2 tests)

    [Test]
    public void BezierNPath2D_Degree4Curve_ShouldInterpolateCorrectly()
    {
        // Arrange - Grad-4 Bezier-Kurve (5 Kontrollpunkte)
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var curve = new BezierNPath2D<double>(timeRange, false);

        // Definiere 5 Kontrollpunkte
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 0, 0));   // P0
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 1, 2));   // P1
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 3, 3));   // P2
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 4, 1));   // P3
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 5, 0));   // P4

        // Act & Assert - Endpunkte
        var p0 = curve.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(p0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Start X");
        Assert.That(p0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Start Y");

        var p1 = curve.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(p1.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "End X");
        Assert.That(p1.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "End Y");

        // Mittelpunkt sollte existieren
        var pMid = curve.GetValue(ScalarProcessor.Scalar(0.5));
        Assert.That(pMid.X.ScalarValue, Is.GreaterThan(0) & Is.LessThan(5), "Mid X in range");
        Assert.That(pMid.Y.ScalarValue, Is.GreaterThan(0), "Mid Y positive");
    }

    [Test]
    public void BezierNPath2D_Degree4Curve_DegreePropertyShouldBeCorrect()
    {
        // Arrange
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var curve = new BezierNPath2D<double>(timeRange, false);

        // 5 Kontrollpunkte → Grad 4
        for (int i = 0; i < 5; i++)
            curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, i, i));

        // Assert
        Assert.That(curve.Degree, Is.EqualTo(4), "Degree should be 4 for 5 control points");
        Assert.That(curve.ControlPoints.Count, Is.EqualTo(5), "Should have 5 control points");
    }

    #endregion

    #region Degree 5 Curve Tests (2 tests)

    [Test]
    public void BezierNPath2D_Degree5Curve_ShouldPassThroughEndpoints()
    {
        // Arrange - Grad-5 Bezier-Kurve (6 Kontrollpunkte)
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var curve = new BezierNPath2D<double>(timeRange, false);

        // 6 zufällige Kontrollpunkte
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 0, 0));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 1, 3));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 2, 5));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 3, 4));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 4, 2));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 6, 1));

        // Act - Bezier-Kurven gehen immer durch erste und letzte Kontrollpunkte
        var start = curve.GetValue(ScalarProcessor.Scalar(0.0));
        var end = curve.GetValue(ScalarProcessor.Scalar(1.0));

        // Assert
        Assert.That(start.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Start matches first control point X");
        Assert.That(start.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Start matches first control point Y");

        Assert.That(end.X.ScalarValue, Is.EqualTo(6.0).Within(Tolerance), "End matches last control point X");
        Assert.That(end.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "End matches last control point Y");
    }

    [Test]
    public void BezierNPath2D_Degree5Curve_ShouldLieWithinConvexHull()
    {
        // Arrange - Einfache konvexe Hülle: Quadrat [0,4] x [0,4]
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var curve = new BezierNPath2D<double>(timeRange, false);

        // Alle Kontrollpunkte im Quadrat [0,4] x [0,4]
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 0, 0));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 1, 2));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 2, 4));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 3, 3));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 4, 1));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 4, 0));

        // Act & Assert - Kurve sollte innerhalb konvexer Hülle liegen
        for (var t = 0.0; t <= 1.0; t += 0.1)
        {
            var point = curve.GetValue(ScalarProcessor.Scalar(t));

            Assert.That(point.X.ScalarValue, Is.GreaterThanOrEqualTo(0.0).Within(Tolerance), $"X >= 0 at t={t}");
            Assert.That(point.X.ScalarValue, Is.LessThanOrEqualTo(4.0).Within(Tolerance), $"X <= 4 at t={t}");
            Assert.That(point.Y.ScalarValue, Is.GreaterThanOrEqualTo(0.0).Within(Tolerance), $"Y >= 0 at t={t}");
            Assert.That(point.Y.ScalarValue, Is.LessThanOrEqualTo(4.0).Within(Tolerance), $"Y <= 4 at t={t}");
        }
    }

    #endregion

    #region Derivative Curve Tests (3 tests)

    [Test]
    public void BezierNPath2D_GetDerivativeCurve_ShouldReduceDegreeByOne()
    {
        // Arrange - Grad-4 Kurve
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var curve = new BezierNPath2D<double>(timeRange, false);
        for (int i = 0; i < 5; i++)
            curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, i, i * i));

        // Act
        var derivCurve = curve.GetDerivativeCurve();

        // Assert
        Assert.That(curve.Degree, Is.EqualTo(4), "Original curve degree");
        Assert.That(derivCurve.Degree, Is.EqualTo(3), "Derivative curve degree should be 3");
    }

    [Test]
    public void BezierNPath2D_GetDerivative1Value_ShouldMatchDerivativeCurve()
    {
        // Arrange
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var curve = new BezierNPath2D<double>(timeRange, false);
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 0, 0));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 1, 1));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 2, 4));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 3, 2));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 4, 0));

        var derivCurve = curve.GetDerivativeCurve();

        // Act & Assert - GetDerivative1Value sollte GetDerivativeCurve().GetValue() entsprechen
        for (var t = 0.0; t <= 1.0; t += 0.25)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var deriv1 = curve.GetDerivative1Value(tScalar);
            var derivFromCurve = derivCurve.GetValue(tScalar);

            Assert.That(deriv1.X.ScalarValue, Is.EqualTo(derivFromCurve.X.ScalarValue).Within(Tolerance),
                $"Derivative X at t={t}");
            Assert.That(deriv1.Y.ScalarValue, Is.EqualTo(derivFromCurve.Y.ScalarValue).Within(Tolerance),
                $"Derivative Y at t={t}");
        }
    }

    [Test]
    public void BezierNPath2D_GetDerivative2Value_ShouldMatchSecondDerivativeCurve()
    {
        // Arrange
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var curve = new BezierNPath2D<double>(timeRange, false);
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 0, 0));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 1, 2));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 3, 3));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 4, 1));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 5, 0));

        var deriv1Curve = curve.GetDerivativeCurve();
        var deriv2Curve = deriv1Curve.GetDerivativeCurve();

        // Act & Assert
        for (var t = 0.0; t <= 1.0; t += 0.25)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var deriv2 = curve.GetDerivative2Value(tScalar);
            var deriv2FromCurve = deriv2Curve.GetValue(tScalar);

            Assert.That(deriv2.X.ScalarValue, Is.EqualTo(deriv2FromCurve.X.ScalarValue).Within(Tolerance),
                $"Second derivative X at t={t}");
            Assert.That(deriv2.Y.ScalarValue, Is.EqualTo(deriv2FromCurve.Y.ScalarValue).Within(Tolerance),
                $"Second derivative Y at t={t}");
        }
    }

    #endregion

    #region Edge Cases and Conversion Tests (3 tests)

    [Test]
    public void BezierNPath2D_SingleControlPoint_ShouldBeConstant()
    {
        // Arrange - Grad-0 Kurve (konstant)
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var curve = new BezierNPath2D<double>(timeRange, false);
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 3, 5));

        // Act & Assert - Sollte konstant sein
        for (var t = 0.0; t <= 1.0; t += 0.2)
        {
            var point = curve.GetValue(ScalarProcessor.Scalar(t));
            Assert.That(point.X.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), $"Constant X at t={t}");
            Assert.That(point.Y.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), $"Constant Y at t={t}");
        }

        // Ableitung sollte Null sein
        var deriv = curve.GetDerivative1Value(ScalarProcessor.Scalar(0.5));
        Assert.That(deriv.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Derivative X should be zero");
        Assert.That(deriv.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Derivative Y should be zero");
    }

    [Test]
    public void BezierNPath2D_TwoControlPoints_ShouldBeLinear()
    {
        // Arrange - Grad-1 Kurve (linear)
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var curve = new BezierNPath2D<double>(timeRange, false);
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 0, 0));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 10, 5));

        // Act & Assert - Sollte linear interpolieren
        var p0 = curve.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(p0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(p0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));

        var pMid = curve.GetValue(ScalarProcessor.Scalar(0.5));
        Assert.That(pMid.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Mid X");
        Assert.That(pMid.Y.ScalarValue, Is.EqualTo(2.5).Within(Tolerance), "Mid Y");

        var p1 = curve.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(p1.X.ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
        Assert.That(p1.Y.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
    }

    [Test]
    public void BezierNPath2D_Conversion_FiniteToPeriodicShouldPreserveControlPoints()
    {
        // Arrange
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var curve = new BezierNPath2D<double>(timeRange, false);
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 0, 0));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 1, 1));
        curve.ControlPoints.Add(LinVector2D<double>.Create(ScalarProcessor, 2, 0));

        // Act
        var periodicCurve = (BezierNPath2D<double>)curve.ToPeriodicPath();

        // Assert
        Assert.That(ReferenceEquals(curve, periodicCurve), Is.False, "Should create new instance");
        Assert.That(periodicCurve.IsPeriodic, Is.True, "Should be periodic");
        Assert.That(periodicCurve.Degree, Is.EqualTo(curve.Degree), "Degree should be preserved");

        for (int i = 0; i < curve.ControlPoints.Count; i++)
        {
            Assert.That(periodicCurve.ControlPoints[i].X.ScalarValue,
                Is.EqualTo(curve.ControlPoints[i].X.ScalarValue).Within(Tolerance), $"Control point {i} X");
            Assert.That(periodicCurve.ControlPoints[i].Y.ScalarValue,
                Is.EqualTo(curve.ControlPoints[i].Y.ScalarValue).Within(Tolerance), $"Control point {i} Y");
        }
    }

    #endregion
}
