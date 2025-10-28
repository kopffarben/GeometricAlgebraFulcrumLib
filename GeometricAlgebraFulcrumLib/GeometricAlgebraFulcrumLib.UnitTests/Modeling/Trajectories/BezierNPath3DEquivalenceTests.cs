using System;
using System.Linq;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Bezier;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Bezier;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public class BezierNPath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [OneTimeSetUp]
    public void ClassInit()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void BezierNPath3D_Degree0_ShouldMatchConstantPoint()
    {
        // Degree 0 = constant point (1 control point)
        var p0 = LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3);

        var bezierN = BezierNPath3D<double>.Finite(ScalarProcessor, p0);

        Assert.That(bezierN.Degree, Is.EqualTo(0), "Degree should be 0");

        // Test at various t values - should always return p0
        var t0 = ScalarProcessor.Zero;
        var t05 = ScalarProcessor.ScalarFromNumber(0.5);
        var t1 = ScalarProcessor.One;

        var value0 = bezierN.GetValue(t0);
        var value05 = bezierN.GetValue(t05);
        var value1 = bezierN.GetValue(t1);

        Assert.That(value0.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(value0.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance));
        Assert.That(value0.Z.ScalarValue, Is.EqualTo(3.0).Within(Tolerance));

        Assert.That(value05.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(value1.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
    }

    [Test]
    public void BezierNPath3D_Degree1_ShouldMatchLinearInterpolation()
    {
        // Degree 1 = linear (2 control points)
        var p0 = LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0);
        var p1 = LinVector3D<double>.Create(ScalarProcessor, 10, 10, 10);

        var bezierN = BezierNPath3D<double>.Finite(ScalarProcessor, p0, p1);

        Assert.That(bezierN.Degree, Is.EqualTo(1), "Degree should be 1");

        // At t=0: should be p0
        var value0 = bezierN.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));

        // At t=0.5: should be (5, 5, 5)
        var value05 = bezierN.GetValue(ScalarProcessor.ScalarFromNumber(0.5));
        Assert.That(value05.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(value05.Y.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(value05.Z.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));

        // At t=1: should be p1
        var value1 = bezierN.GetValue(ScalarProcessor.One);
        Assert.That(value1.X.ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
        Assert.That(value1.Y.ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
        Assert.That(value1.Z.ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
    }

    [Test]
    public void BezierNPath3D_Degree2_ShouldMatchBezier2()
    {
        // Degree 2 = quadratic (3 control points)
        var p0 = LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0);
        var p1 = LinVector3D<double>.Create(ScalarProcessor, 5, 10, 5);
        var p2 = LinVector3D<double>.Create(ScalarProcessor, 10, 0, 10);

        var bezierN = BezierNPath3D<double>.Finite(ScalarProcessor, p0, p1, p2);
        var bezier2 = Bezier2Path3D<double>.Create(ScalarProcessor, false, p0, p1, p2);

        Assert.That(bezierN.Degree, Is.EqualTo(2), "Degree should be 2");

        // Test at multiple t values
        var tValues = new[] { 0.0, 0.25, 0.5, 0.75, 1.0 };

        foreach (var tVal in tValues)
        {
            var t = ScalarProcessor.ScalarFromNumber(tVal);
            var valueN = bezierN.GetValue(t);
            var value2 = bezier2.GetValue(t);

            Assert.That(valueN.X.ScalarValue, Is.EqualTo(value2.X.ScalarValue).Within(Tolerance), $"X mismatch at t={tVal}");
            Assert.That(valueN.Y.ScalarValue, Is.EqualTo(value2.Y.ScalarValue).Within(Tolerance), $"Y mismatch at t={tVal}");
            Assert.That(valueN.Z.ScalarValue, Is.EqualTo(value2.Z.ScalarValue).Within(Tolerance), $"Z mismatch at t={tVal}");
        }
    }

    [Test]
    public void BezierNPath3D_Degree3_ShouldMatchBezier3()
    {
        // Degree 3 = cubic (4 control points)
        var p0 = LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0);
        var p1 = LinVector3D<double>.Create(ScalarProcessor, 3, 6, 1);
        var p2 = LinVector3D<double>.Create(ScalarProcessor, 7, 4, 8);
        var p3 = LinVector3D<double>.Create(ScalarProcessor, 10, 0, 10);

        var bezierN = BezierNPath3D<double>.Finite(ScalarProcessor, p0, p1, p2, p3);
        var bezier3 = Bezier3Path3D<double>.Create(ScalarProcessor, false, p0, p1, p2, p3);

        Assert.That(bezierN.Degree, Is.EqualTo(3), "Degree should be 3");

        // Test at multiple t values
        var tValues = new[] { 0.0, 0.2, 0.4, 0.6, 0.8, 1.0 };

        foreach (var tVal in tValues)
        {
            var t = ScalarProcessor.ScalarFromNumber(tVal);
            var valueN = bezierN.GetValue(t);
            var value3 = bezier3.GetValue(t);

            Assert.That(valueN.X.ScalarValue, Is.EqualTo(value3.X.ScalarValue).Within(Tolerance), $"X mismatch at t={tVal}");
            Assert.That(valueN.Y.ScalarValue, Is.EqualTo(value3.Y.ScalarValue).Within(Tolerance), $"Y mismatch at t={tVal}");
            Assert.That(valueN.Z.ScalarValue, Is.EqualTo(value3.Z.ScalarValue).Within(Tolerance), $"Z mismatch at t={tVal}");
        }
    }

    [Test]
    public void BezierNPath3D_Degree5_ShouldEvaluateCorrectly()
    {
        // Degree 5 = quintic (6 control points) - tests the general algorithm
        var controlPoints = new[]
        {
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 2, 4, 1),
            LinVector3D<double>.Create(ScalarProcessor, 4, 6, 3),
            LinVector3D<double>.Create(ScalarProcessor, 6, 5, 5),
            LinVector3D<double>.Create(ScalarProcessor, 8, 2, 7),
            LinVector3D<double>.Create(ScalarProcessor, 10, 0, 10)
        };

        var bezierN = BezierNPath3D<double>.Finite(ScalarProcessor, controlPoints);

        Assert.That(bezierN.Degree, Is.EqualTo(5), "Degree should be 5");

        // At t=0: should be first control point
        var value0 = bezierN.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(value0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(value0.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));

        // At t=1: should be last control point
        var value1 = bezierN.GetValue(ScalarProcessor.One);
        Assert.That(value1.X.ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
        Assert.That(value1.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(value1.Z.ScalarValue, Is.EqualTo(10.0).Within(Tolerance));

        // At t=0.5: should be somewhere in the middle (no exact value, just checking it runs)
        var value05 = bezierN.GetValue(ScalarProcessor.ScalarFromNumber(0.5));
        Assert.That(value05.IsValid(), Is.True);
        Assert.That(value05.X.ScalarValue, Is.GreaterThan(0.0));
        Assert.That(value05.X.ScalarValue, Is.LessThan(10.0));
    }

    [Test]
    public void BezierNPath3D_GetDerivativeCurve_ShouldReduceDegreeByOne()
    {
        // Cubic curve (degree 3)
        var p0 = LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0);
        var p1 = LinVector3D<double>.Create(ScalarProcessor, 3, 6, 1);
        var p2 = LinVector3D<double>.Create(ScalarProcessor, 7, 4, 8);
        var p3 = LinVector3D<double>.Create(ScalarProcessor, 10, 0, 10);

        var bezierN = BezierNPath3D<double>.Finite(ScalarProcessor, p0, p1, p2, p3);

        // First derivative should have degree 2 (3-1)
        var deriv1Curve = bezierN.GetDerivativeCurve();
        Assert.That(deriv1Curve.Degree, Is.EqualTo(2), "First derivative should have degree 2");

        // Second derivative should have degree 1 (2-1)
        var deriv2Curve = deriv1Curve.GetDerivativeCurve();
        Assert.That(deriv2Curve.Degree, Is.EqualTo(1), "Second derivative should have degree 1");

        // Third derivative should have degree 0 (1-1)
        var deriv3Curve = deriv2Curve.GetDerivativeCurve();
        Assert.That(deriv3Curve.Degree, Is.EqualTo(0), "Third derivative should have degree 0");
    }

    [Test]
    public void BezierNPath3D_GetDerivative1Value_ShouldMatchDerivativeCurve()
    {
        // Quadratic curve
        var p0 = LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0);
        var p1 = LinVector3D<double>.Create(ScalarProcessor, 5, 10, 5);
        var p2 = LinVector3D<double>.Create(ScalarProcessor, 10, 0, 10);

        var bezierN = BezierNPath3D<double>.Finite(ScalarProcessor, p0, p1, p2);

        var tValues = new[] { 0.0, 0.25, 0.5, 0.75, 1.0 };

        foreach (var tVal in tValues)
        {
            var t = ScalarProcessor.ScalarFromNumber(tVal);
            var deriv1 = bezierN.GetDerivative1Value(t);
            var derivCurve = bezierN.GetDerivativeCurve();
            var derivCurveValue = derivCurve.GetValue(t);

            Assert.That(deriv1.X.ScalarValue, Is.EqualTo(derivCurveValue.X.ScalarValue).Within(Tolerance), $"Derivative1 X mismatch at t={tVal}");
            Assert.That(deriv1.Y.ScalarValue, Is.EqualTo(derivCurveValue.Y.ScalarValue).Within(Tolerance), $"Derivative1 Y mismatch at t={tVal}");
            Assert.That(deriv1.Z.ScalarValue, Is.EqualTo(derivCurveValue.Z.ScalarValue).Within(Tolerance), $"Derivative1 Z mismatch at t={tVal}");
        }
    }

    [Test]
    public void BezierNPath3D_IsValid_ShouldReturnTrue()
    {
        var p0 = LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0);
        var p1 = LinVector3D<double>.Create(ScalarProcessor, 10, 10, 10);

        var bezierN = BezierNPath3D<double>.Finite(ScalarProcessor, p0, p1);

        Assert.That(bezierN.IsValid(), Is.True);
    }

    [Test]
    public void BezierNPath3D_ToFinitePath_WhenFinite_ShouldReturnSelf()
    {
        var p0 = LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0);
        var p1 = LinVector3D<double>.Create(ScalarProcessor, 10, 10, 10);

        var bezierN = BezierNPath3D<double>.Finite(ScalarProcessor, p0, p1);

        var finitePath = bezierN.ToFinitePath();

        Assert.That(finitePath, Is.SameAs(bezierN));
        Assert.That(bezierN.IsFinite, Is.True);
        Assert.That(bezierN.IsPeriodic, Is.False);
    }

    [Test]
    public void BezierNPath3D_ToPeriodicPath_WhenFinite_ShouldReturnNewInstance()
    {
        var p0 = LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0);
        var p1 = LinVector3D<double>.Create(ScalarProcessor, 10, 10, 10);

        var bezierN = BezierNPath3D<double>.Finite(ScalarProcessor, p0, p1);

        var periodicPath = bezierN.ToPeriodicPath();

        Assert.That(periodicPath, Is.Not.SameAs(bezierN));
        Assert.That(periodicPath.IsPeriodic, Is.True);
        Assert.That(periodicPath.IsFinite, Is.False);

        // Values should still match
        var t = ScalarProcessor.ScalarFromNumber(0.5);
        var value1 = bezierN.GetValue(t);
        var value2 = periodicPath.GetValue(t);

        Assert.That(value2.X.ScalarValue, Is.EqualTo(value1.X.ScalarValue).Within(Tolerance));
        Assert.That(value2.Y.ScalarValue, Is.EqualTo(value1.Y.ScalarValue).Within(Tolerance));
        Assert.That(value2.Z.ScalarValue, Is.EqualTo(value1.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void BezierNPath3D_EmptyControlPoints_ShouldReturnZero()
    {
        var bezierN = new BezierNPath3D<double>(ScalarProcessor, false);

        var value = bezierN.GetValue(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(value.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(value.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(value.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
    }

    [Test]
    public void BezierNPath3D_ControlPointsCanBeModified()
    {
        var p0 = LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0);
        var bezierN = BezierNPath3D<double>.Finite(ScalarProcessor, p0);

        // Add more control points dynamically
        bezierN.ControlPoints.Add(LinVector3D<double>.Create(ScalarProcessor, 10, 10, 10));

        Assert.That(bezierN.Degree, Is.EqualTo(1), "Degree should now be 1 after adding second point");

        var value1 = bezierN.GetValue(ScalarProcessor.One);
        Assert.That(value1.X.ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
    }

    [Test]
    public void BezierNPath3D_Degree7_ShouldEvaluateCorrectly()
    {
        // Degree 7 (8 control points) - tests higher-degree general algorithm
        var controlPoints = Enumerable.Range(0, 8)
            .Select(i => LinVector3D<double>.Create(ScalarProcessor, i * 1.0, Math.Sin(i), i * 0.5))
            .ToArray();

        var bezierN = BezierNPath3D<double>.Finite(ScalarProcessor, controlPoints);

        Assert.That(bezierN.Degree, Is.EqualTo(7), "Degree should be 7");

        // Test endpoints
        var value0 = bezierN.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));

        var value1 = bezierN.GetValue(ScalarProcessor.One);
        Assert.That(value1.X.ScalarValue, Is.EqualTo(7.0).Within(Tolerance));

        // Test midpoint
        var value05 = bezierN.GetValue(ScalarProcessor.ScalarFromNumber(0.5));
        Assert.That(value05.IsValid(), Is.True);
    }
}
