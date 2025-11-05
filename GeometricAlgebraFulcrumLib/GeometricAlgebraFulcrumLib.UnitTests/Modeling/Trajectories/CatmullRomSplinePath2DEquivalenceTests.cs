using System;
using System.Collections.Generic;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.Parametric.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Equivalence Tests for CatmullRomSplinePath2D&lt;T&gt;
/// Phase 3 Module 6B - Catmull-Rom Splines for 2D Paths
/// Compares Generic&lt;double&gt; implementation against Float64 reference
/// </summary>
[TestFixture]
public class CatmullRomSplinePath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static List<ILinVector2D<double>> GetTestControlPoints4()
    {
        return new List<ILinVector2D<double>>
        {
            LinVector2D<double>.Create(ScalarProcessor, 0, 0),
            LinVector2D<double>.Create(ScalarProcessor, 1, 2),
            LinVector2D<double>.Create(ScalarProcessor, 3, 3),
            LinVector2D<double>.Create(ScalarProcessor, 5, 1)
        };
    }

    private static List<ILinFloat64Vector2D> GetTestControlPointsFloat64()
    {
        return new List<ILinFloat64Vector2D>
        {
            LinFloat64Vector2D.Create(0, 0),
            LinFloat64Vector2D.Create(1, 2),
            LinFloat64Vector2D.Create(3, 3),
            LinFloat64Vector2D.Create(5, 1)
        };
    }

    #endregion

    #region Centripetal Spline Tests (5 tests)

    [Test]
    public void CatmullRomSplinePath2D_Centripetal_Open_GetValue_ShouldMatch()
    {
        // Arrange
        var controlPointsGeneric = GetTestControlPoints4();
        var controlPointsFloat64 = GetTestControlPointsFloat64();

        var genericSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: false
        );

        var float64Spline = new Float64CatmullRomSplinePath2D(
            isPeriodic: false,
            inputPointList: controlPointsFloat64,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: false
        );

        // Act & Assert - Compare GetValue at multiple points
        for (var t = 0.0; t <= 1.0; t += 0.1)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var genericValue = genericSpline.GetValue(tScalar);
            var float64Value = float64Spline.GetValue(t);

            Assert.That(genericValue.X.ScalarValue, Is.EqualTo(float64Value.X).Within(Tolerance),
                $"Centripetal Open GetValue X at t={t}");
            Assert.That(genericValue.Y.ScalarValue, Is.EqualTo(float64Value.Y).Within(Tolerance),
                $"Centripetal Open GetValue Y at t={t}");
        }
    }

    [Test]
    public void CatmullRomSplinePath2D_Centripetal_Closed_GetValue_ShouldMatch()
    {
        // Arrange
        var controlPointsGeneric = GetTestControlPoints4();
        var controlPointsFloat64 = GetTestControlPointsFloat64();

        var genericSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: true  // Closed spline
        );

        var float64Spline = new Float64CatmullRomSplinePath2D(
            isPeriodic: false,
            inputPointList: controlPointsFloat64,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: true
        );

        // Act & Assert
        for (var t = 0.0; t <= 1.0; t += 0.1)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var genericValue = genericSpline.GetValue(tScalar);
            var float64Value = float64Spline.GetValue(t);

            Assert.That(genericValue.X.ScalarValue, Is.EqualTo(float64Value.X).Within(Tolerance),
                $"Centripetal Closed GetValue X at t={t}");
            Assert.That(genericValue.Y.ScalarValue, Is.EqualTo(float64Value.Y).Within(Tolerance),
                $"Centripetal Closed GetValue Y at t={t}");
        }
    }

    [Test]
    public void CatmullRomSplinePath2D_Centripetal_GetPointX_ShouldMatch()
    {
        // Arrange
        var controlPointsGeneric = GetTestControlPoints4();
        var controlPointsFloat64 = GetTestControlPointsFloat64();

        var genericSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: false
        );

        var float64Spline = new Float64CatmullRomSplinePath2D(
            isPeriodic: false,
            inputPointList: controlPointsFloat64,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: false
        );

        // Act & Assert - Compare GetPointX at multiple points
        for (var t = 0.0; t <= 1.0; t += 0.1)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var genericX = genericSpline.GetPointX(tScalar);
            var float64X = float64Spline.GetPointX(t);

            Assert.That(genericX.ScalarValue, Is.EqualTo(float64X).Within(Tolerance),
                $"GetPointX at t={t}");
        }
    }

    [Test]
    public void CatmullRomSplinePath2D_Centripetal_GetPointY_ShouldMatch()
    {
        // Arrange
        var controlPointsGeneric = GetTestControlPoints4();
        var controlPointsFloat64 = GetTestControlPointsFloat64();

        var genericSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: false
        );

        var float64Spline = new Float64CatmullRomSplinePath2D(
            isPeriodic: false,
            inputPointList: controlPointsFloat64,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: false
        );

        // Act & Assert - Compare GetPointY at multiple points
        for (var t = 0.0; t <= 1.0; t += 0.1)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var genericY = genericSpline.GetPointY(tScalar);
            var float64Y = float64Spline.GetPointY(t);

            Assert.That(genericY.ScalarValue, Is.EqualTo(float64Y).Within(Tolerance),
                $"GetPointY at t={t}");
        }
    }

    [Test]
    public void CatmullRomSplinePath2D_Centripetal_ShouldInterpolateAtInnerKnots()
    {
        // Arrange - Centripetal splines interpolate inner control points at specific parameter values
        var controlPointsGeneric = GetTestControlPoints4();

        var genericSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: false
        );

        // Act & Assert - At t=0, should be at first interior control point
        // At t=1, should be at last interior control point
        var p0 = genericSpline.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(p0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Start X");
        Assert.That(p0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Start Y");

        var p1 = genericSpline.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(p1.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "End X");
        Assert.That(p1.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "End Y");
    }

    #endregion

    #region Chordal Spline Tests (3 tests)

    [Test]
    public void CatmullRomSplinePath2D_Chordal_Open_GetValue_ShouldMatch()
    {
        // Arrange
        var controlPointsGeneric = GetTestControlPoints4();
        var controlPointsFloat64 = GetTestControlPointsFloat64();

        var genericSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Chordal,
            isClosed: false
        );

        var float64Spline = new Float64CatmullRomSplinePath2D(
            isPeriodic: false,
            inputPointList: controlPointsFloat64,
            curveType: CatmullRomSplineType.Chordal,
            isClosed: false
        );

        // Act & Assert
        for (var t = 0.0; t <= 1.0; t += 0.1)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var genericValue = genericSpline.GetValue(tScalar);
            var float64Value = float64Spline.GetValue(t);

            Assert.That(genericValue.X.ScalarValue, Is.EqualTo(float64Value.X).Within(Tolerance),
                $"Chordal Open GetValue X at t={t}");
            Assert.That(genericValue.Y.ScalarValue, Is.EqualTo(float64Value.Y).Within(Tolerance),
                $"Chordal Open GetValue Y at t={t}");
        }
    }

    [Test]
    public void CatmullRomSplinePath2D_Chordal_Closed_GetValue_ShouldMatch()
    {
        // Arrange
        var controlPointsGeneric = GetTestControlPoints4();
        var controlPointsFloat64 = GetTestControlPointsFloat64();

        var genericSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Chordal,
            isClosed: true
        );

        var float64Spline = new Float64CatmullRomSplinePath2D(
            isPeriodic: false,
            inputPointList: controlPointsFloat64,
            curveType: CatmullRomSplineType.Chordal,
            isClosed: true
        );

        // Act & Assert
        for (var t = 0.0; t <= 1.0; t += 0.1)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var genericValue = genericSpline.GetValue(tScalar);
            var float64Value = float64Spline.GetValue(t);

            Assert.That(genericValue.X.ScalarValue, Is.EqualTo(float64Value.X).Within(Tolerance),
                $"Chordal Closed GetValue X at t={t}");
            Assert.That(genericValue.Y.ScalarValue, Is.EqualTo(float64Value.Y).Within(Tolerance),
                $"Chordal Closed GetValue Y at t={t}");
        }
    }

    [Test]
    public void CatmullRomSplinePath2D_Chordal_Endpoints_ShouldMatch()
    {
        // Arrange
        var controlPointsGeneric = GetTestControlPoints4();
        var controlPointsFloat64 = GetTestControlPointsFloat64();

        var genericSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Chordal,
            isClosed: false
        );

        var float64Spline = new Float64CatmullRomSplinePath2D(
            isPeriodic: false,
            inputPointList: controlPointsFloat64,
            curveType: CatmullRomSplineType.Chordal,
            isClosed: false
        );

        // Act
        var genericStart = genericSpline.GetValue(ScalarProcessor.Scalar(0.0));
        var float64Start = float64Spline.GetValue(0.0);
        var genericEnd = genericSpline.GetValue(ScalarProcessor.Scalar(1.0));
        var float64End = float64Spline.GetValue(1.0);

        // Assert
        Assert.That(genericStart.X.ScalarValue, Is.EqualTo(float64Start.X).Within(Tolerance), "Start X");
        Assert.That(genericStart.Y.ScalarValue, Is.EqualTo(float64Start.Y).Within(Tolerance), "Start Y");
        Assert.That(genericEnd.X.ScalarValue, Is.EqualTo(float64End.X).Within(Tolerance), "End X");
        Assert.That(genericEnd.Y.ScalarValue, Is.EqualTo(float64End.Y).Within(Tolerance), "End Y");
    }

    #endregion

    #region Derivative Tests (4 tests)

    [Test]
    public void CatmullRomSplinePath2D_GetDerivative1Value_ShouldMatch()
    {
        // Arrange
        var controlPointsGeneric = GetTestControlPoints4();
        var controlPointsFloat64 = GetTestControlPointsFloat64();

        var genericSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: false
        );

        var float64Spline = new Float64CatmullRomSplinePath2D(
            isPeriodic: false,
            inputPointList: controlPointsFloat64,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: false
        );

        // Act & Assert - Compare derivatives within valid range (not at boundaries)
        for (var t = 0.1; t <= 0.9; t += 0.2)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var genericDeriv = genericSpline.GetDerivative1Value(tScalar);
            var float64Deriv = float64Spline.GetDerivative1Value(t);

            Assert.That(genericDeriv.X.ScalarValue, Is.EqualTo(float64Deriv.X).Within(Tolerance),
                $"Derivative1 X at t={t}");
            Assert.That(genericDeriv.Y.ScalarValue, Is.EqualTo(float64Deriv.Y).Within(Tolerance),
                $"Derivative1 Y at t={t}");
        }
    }

    [Test]
    public void CatmullRomSplinePath2D_GetDerivative2Value_ShouldMatch()
    {
        // Arrange
        var controlPointsGeneric = GetTestControlPoints4();
        var controlPointsFloat64 = GetTestControlPointsFloat64();

        var genericSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: false
        );

        var float64Spline = new Float64CatmullRomSplinePath2D(
            isPeriodic: false,
            inputPointList: controlPointsFloat64,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: false
        );

        // Act & Assert - Compare second derivatives within valid range
        for (var t = 0.2; t <= 0.8; t += 0.2)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var genericDeriv2 = genericSpline.GetDerivative2Value(tScalar);
            var float64Deriv2 = float64Spline.GetDerivative2Value(t);

            Assert.That(genericDeriv2.X.ScalarValue, Is.EqualTo(float64Deriv2.X).Within(Tolerance),
                $"Derivative2 X at t={t}");
            Assert.That(genericDeriv2.Y.ScalarValue, Is.EqualTo(float64Deriv2.Y).Within(Tolerance),
                $"Derivative2 Y at t={t}");
        }
    }

    [Test]
    public void CatmullRomSplinePath2D_Chordal_GetDerivative1Value_ShouldMatch()
    {
        // Arrange
        var controlPointsGeneric = GetTestControlPoints4();
        var controlPointsFloat64 = GetTestControlPointsFloat64();

        var genericSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Chordal,
            isClosed: false
        );

        var float64Spline = new Float64CatmullRomSplinePath2D(
            isPeriodic: false,
            inputPointList: controlPointsFloat64,
            curveType: CatmullRomSplineType.Chordal,
            isClosed: false
        );

        // Act & Assert
        for (var t = 0.1; t <= 0.9; t += 0.2)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var genericDeriv = genericSpline.GetDerivative1Value(tScalar);
            var float64Deriv = float64Spline.GetDerivative1Value(t);

            Assert.That(genericDeriv.X.ScalarValue, Is.EqualTo(float64Deriv.X).Within(Tolerance),
                $"Chordal Derivative1 X at t={t}");
            Assert.That(genericDeriv.Y.ScalarValue, Is.EqualTo(float64Deriv.Y).Within(Tolerance),
                $"Chordal Derivative1 Y at t={t}");
        }
    }

    [Test]
    public void CatmullRomSplinePath2D_Chordal_GetDerivative2Value_ShouldMatch()
    {
        // Arrange
        var controlPointsGeneric = GetTestControlPoints4();
        var controlPointsFloat64 = GetTestControlPointsFloat64();

        var genericSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Chordal,
            isClosed: false
        );

        var float64Spline = new Float64CatmullRomSplinePath2D(
            isPeriodic: false,
            inputPointList: controlPointsFloat64,
            curveType: CatmullRomSplineType.Chordal,
            isClosed: false
        );

        // Act & Assert
        for (var t = 0.2; t <= 0.8; t += 0.2)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var genericDeriv2 = genericSpline.GetDerivative2Value(tScalar);
            var float64Deriv2 = float64Spline.GetDerivative2Value(t);

            Assert.That(genericDeriv2.X.ScalarValue, Is.EqualTo(float64Deriv2.X).Within(Tolerance),
                $"Chordal Derivative2 X at t={t}");
            Assert.That(genericDeriv2.Y.ScalarValue, Is.EqualTo(float64Deriv2.Y).Within(Tolerance),
                $"Chordal Derivative2 Y at t={t}");
        }
    }

    #endregion

    #region Edge Cases and Properties (4 tests)

    [Test]
    public void CatmullRomSplinePath2D_IsValid_ShouldReturnTrue()
    {
        // Arrange
        var controlPointsGeneric = GetTestControlPoints4();

        var genericSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: false
        );

        // Act & Assert
        Assert.That(genericSpline.IsValid(), Is.True, "Spline with valid control points should be valid");
    }

    [Test]
    public void CatmullRomSplinePath2D_ControlPointCount_ShouldBeCorrect()
    {
        // Arrange
        var controlPointsGeneric = GetTestControlPoints4();

        var genericSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: false
        );

        // Act & Assert
        // Open spline adds 2 extra control points at ends
        Assert.That(genericSpline.ControlPointCount, Is.EqualTo(6),
            "Open spline should have original 4 + 2 extra control points");
    }

    [Test]
    public void CatmullRomSplinePath2D_Closed_ControlPointCount_ShouldBeCorrect()
    {
        // Arrange
        var controlPointsGeneric = GetTestControlPoints4();

        var genericSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: true
        );

        // Act & Assert
        // Closed spline wraps around
        Assert.That(genericSpline.ControlPointCount, Is.EqualTo(6),
            "Closed spline should have 4 + 2 for wrapping");
        Assert.That(genericSpline.IsClosed, Is.True, "IsClosed should be true");
    }

    [Test]
    public void CatmullRomSplinePath2D_CurveType_ShouldBePreserved()
    {
        // Arrange
        var controlPointsGeneric = GetTestControlPoints4();

        var centripetalSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Centripetal,
            isClosed: false
        );

        var chordalSpline = new CatmullRomSplinePath2D<double>(
            isPeriodic: false,
            inputPointList: controlPointsGeneric,
            curveType: CatmullRomSplineType.Chordal,
            isClosed: false
        );

        // Act & Assert
        Assert.That(centripetalSpline.CurveType, Is.EqualTo(CatmullRomSplineType.Centripetal),
            "Centripetal curve type should be preserved");
        Assert.That(chordalSpline.CurveType, Is.EqualTo(CatmullRomSplineType.Chordal),
            "Chordal curve type should be preserved");
    }

    #endregion
}
