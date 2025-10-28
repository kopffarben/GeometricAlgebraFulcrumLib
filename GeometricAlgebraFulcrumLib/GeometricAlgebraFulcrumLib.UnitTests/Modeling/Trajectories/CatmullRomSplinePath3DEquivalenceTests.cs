using System.Collections.Generic;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.Parametric.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public class CatmullRomSplinePath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [SetUp]
    public void Setup()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    private static List<LinFloat64Vector3D> CreateTestControlPoints()
    {
        return new List<LinFloat64Vector3D>
        {
            LinFloat64Vector3D.Create(0, 0, 0),
            LinFloat64Vector3D.Create(1, 2, 1),
            LinFloat64Vector3D.Create(3, 1, 2),
            LinFloat64Vector3D.Create(4, 3, 1),
            LinFloat64Vector3D.Create(5, 2, 3)
        };
    }

    private static List<LinVector3D<double>> CreateTestControlPointsGeneric(IScalarProcessor<double> processor)
    {
        return new List<LinVector3D<double>>
        {
            LinVector3D<double>.Create(processor.Zero, processor.Zero, processor.Zero),
            LinVector3D<double>.Create(
                processor.One,
                processor.ScalarFromNumber(2),
                processor.One
            ),
            LinVector3D<double>.Create(
                processor.ScalarFromNumber(3),
                processor.One,
                processor.ScalarFromNumber(2)
            ),
            LinVector3D<double>.Create(
                processor.ScalarFromNumber(4),
                processor.ScalarFromNumber(3),
                processor.One
            ),
            LinVector3D<double>.Create(
                processor.ScalarFromNumber(5),
                processor.ScalarFromNumber(2),
                processor.ScalarFromNumber(3)
            )
        };
    }

    [Test]
    public void CatmullRomSpline_OpenCurve_Centripetal_GetValue_AtStart_ShouldMatchFloat64()
    {
        var controlPointsFloat64 = CreateTestControlPoints();
        var splineFloat64 = new Float64CatmullRomSplinePath3D(
            false,
            controlPointsFloat64,
            CatmullRomSplineType.Centripetal,
            false
        );

        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            false,
            ScalarProcessor
        );

        var valueFloat64 = splineFloat64.GetValue(0.0);
        var valueGeneric = splineGeneric.GetValue(ScalarProcessor.Zero);

        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z).Within(Tolerance));
    }

    [Test]
    public void CatmullRomSpline_OpenCurve_Centripetal_GetValue_AtEnd_ShouldMatchFloat64()
    {
        var controlPointsFloat64 = CreateTestControlPoints();
        var splineFloat64 = new Float64CatmullRomSplinePath3D(
            false,
            controlPointsFloat64,
            CatmullRomSplineType.Centripetal,
            false
        );

        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            false,
            ScalarProcessor
        );

        var valueFloat64 = splineFloat64.GetValue(1.0);
        var valueGeneric = splineGeneric.GetValue(ScalarProcessor.One);

        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z).Within(Tolerance));
    }

    [Test]
    public void CatmullRomSpline_OpenCurve_Centripetal_GetValue_AtMidpoint_ShouldMatchFloat64()
    {
        var controlPointsFloat64 = CreateTestControlPoints();
        var splineFloat64 = new Float64CatmullRomSplinePath3D(
            false,
            controlPointsFloat64,
            CatmullRomSplineType.Centripetal,
            false
        );

        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            false,
            ScalarProcessor
        );

        var valueFloat64 = splineFloat64.GetValue(0.5);
        var valueGeneric = splineGeneric.GetValue(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z).Within(Tolerance));
    }

    [Test]
    public void CatmullRomSpline_OpenCurve_Centripetal_GetValue_AtQuarterPoint_ShouldMatchFloat64()
    {
        var controlPointsFloat64 = CreateTestControlPoints();
        var splineFloat64 = new Float64CatmullRomSplinePath3D(
            false,
            controlPointsFloat64,
            CatmullRomSplineType.Centripetal,
            false
        );

        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            false,
            ScalarProcessor
        );

        var valueFloat64 = splineFloat64.GetValue(0.25);
        var valueGeneric = splineGeneric.GetValue(ScalarProcessor.ScalarFromNumber(0.25));

        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z).Within(Tolerance));
    }

    [Test]
    public void CatmullRomSpline_OpenCurve_Centripetal_GetValue_AtThreeQuarterPoint_ShouldMatchFloat64()
    {
        var controlPointsFloat64 = CreateTestControlPoints();
        var splineFloat64 = new Float64CatmullRomSplinePath3D(
            false,
            controlPointsFloat64,
            CatmullRomSplineType.Centripetal,
            false
        );

        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            false,
            ScalarProcessor
        );

        var valueFloat64 = splineFloat64.GetValue(0.75);
        var valueGeneric = splineGeneric.GetValue(ScalarProcessor.ScalarFromNumber(0.75));

        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z).Within(Tolerance));
    }

    [Test]
    public void CatmullRomSpline_OpenCurve_Chordal_GetValue_AtMidpoint_ShouldMatchFloat64()
    {
        var controlPointsFloat64 = CreateTestControlPoints();
        var splineFloat64 = new Float64CatmullRomSplinePath3D(
            false,
            controlPointsFloat64,
            CatmullRomSplineType.Chordal,
            false
        );

        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Chordal,
            false,
            ScalarProcessor
        );

        var valueFloat64 = splineFloat64.GetValue(0.5);
        var valueGeneric = splineGeneric.GetValue(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z).Within(Tolerance));
    }

    [Test]
    public void CatmullRomSpline_ClosedCurve_Centripetal_GetValue_AtMidpoint_ShouldMatchFloat64()
    {
        var controlPointsFloat64 = CreateTestControlPoints();
        var splineFloat64 = new Float64CatmullRomSplinePath3D(
            false,
            controlPointsFloat64,
            CatmullRomSplineType.Centripetal,
            true
        );

        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            true,
            ScalarProcessor
        );

        var valueFloat64 = splineFloat64.GetValue(0.5);
        var valueGeneric = splineGeneric.GetValue(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z).Within(Tolerance));
    }

    [Test]
    public void CatmullRomSpline_GetDerivative1Value_AtMidpoint_ShouldMatchFloat64()
    {
        var controlPointsFloat64 = CreateTestControlPoints();
        var splineFloat64 = new Float64CatmullRomSplinePath3D(
            false,
            controlPointsFloat64,
            CatmullRomSplineType.Centripetal,
            false
        );

        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            false,
            ScalarProcessor
        );

        var deriv1Float64 = splineFloat64.GetDerivative1Value(0.5);
        var deriv1Generic = splineGeneric.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(deriv1Generic.X.ScalarValue, Is.EqualTo(deriv1Float64.X).Within(Tolerance));
        Assert.That(deriv1Generic.Y.ScalarValue, Is.EqualTo(deriv1Float64.Y).Within(Tolerance));
        Assert.That(deriv1Generic.Z.ScalarValue, Is.EqualTo(deriv1Float64.Z).Within(Tolerance));
    }

    [Test]
    public void CatmullRomSpline_GetDerivative1Value_AtQuarterPoint_ShouldMatchFloat64()
    {
        var controlPointsFloat64 = CreateTestControlPoints();
        var splineFloat64 = new Float64CatmullRomSplinePath3D(
            false,
            controlPointsFloat64,
            CatmullRomSplineType.Centripetal,
            false
        );

        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            false,
            ScalarProcessor
        );

        var deriv1Float64 = splineFloat64.GetDerivative1Value(0.25);
        var deriv1Generic = splineGeneric.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(0.25));

        Assert.That(deriv1Generic.X.ScalarValue, Is.EqualTo(deriv1Float64.X).Within(Tolerance));
        Assert.That(deriv1Generic.Y.ScalarValue, Is.EqualTo(deriv1Float64.Y).Within(Tolerance));
        Assert.That(deriv1Generic.Z.ScalarValue, Is.EqualTo(deriv1Float64.Z).Within(Tolerance));
    }

    [Test]
    public void CatmullRomSpline_GetDerivative2Value_AtMidpoint_ShouldMatchFloat64()
    {
        var controlPointsFloat64 = CreateTestControlPoints();
        var splineFloat64 = new Float64CatmullRomSplinePath3D(
            false,
            controlPointsFloat64,
            CatmullRomSplineType.Centripetal,
            false
        );

        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            false,
            ScalarProcessor
        );

        var deriv2Float64 = splineFloat64.GetDerivative2Value(0.5);
        var deriv2Generic = splineGeneric.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(deriv2Generic.X.ScalarValue, Is.EqualTo(deriv2Float64.X).Within(Tolerance));
        Assert.That(deriv2Generic.Y.ScalarValue, Is.EqualTo(deriv2Float64.Y).Within(Tolerance));
        Assert.That(deriv2Generic.Z.ScalarValue, Is.EqualTo(deriv2Float64.Z).Within(Tolerance));
    }

    [Test]
    public void CatmullRomSpline_GetPointX_AtMidpoint_ShouldMatchFloat64()
    {
        var controlPointsFloat64 = CreateTestControlPoints();
        var splineFloat64 = new Float64CatmullRomSplinePath3D(
            false,
            controlPointsFloat64,
            CatmullRomSplineType.Centripetal,
            false
        );

        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            false,
            ScalarProcessor
        );

        var xFloat64 = splineFloat64.GetPointX(0.5);
        var xGeneric = splineGeneric.GetPointX(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(xGeneric.ScalarValue, Is.EqualTo(xFloat64).Within(Tolerance));
    }

    [Test]
    public void CatmullRomSpline_GetPointY_AtMidpoint_ShouldMatchFloat64()
    {
        var controlPointsFloat64 = CreateTestControlPoints();
        var splineFloat64 = new Float64CatmullRomSplinePath3D(
            false,
            controlPointsFloat64,
            CatmullRomSplineType.Centripetal,
            false
        );

        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            false,
            ScalarProcessor
        );

        var yFloat64 = splineFloat64.GetPointY(0.5);
        var yGeneric = splineGeneric.GetPointY(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(yGeneric.ScalarValue, Is.EqualTo(yFloat64).Within(Tolerance));
    }

    [Test]
    public void CatmullRomSpline_GetPointZ_AtMidpoint_ShouldMatchFloat64()
    {
        var controlPointsFloat64 = CreateTestControlPoints();
        var splineFloat64 = new Float64CatmullRomSplinePath3D(
            false,
            controlPointsFloat64,
            CatmullRomSplineType.Centripetal,
            false
        );

        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            false,
            ScalarProcessor
        );

        var zFloat64 = splineFloat64.GetPointZ(0.5);
        var zGeneric = splineGeneric.GetPointZ(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(zGeneric.ScalarValue, Is.EqualTo(zFloat64).Within(Tolerance));
    }

    [Test]
    public void CatmullRomSpline_ControlPointCount_ShouldMatchOriginalCount()
    {
        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            false,
            ScalarProcessor
        );

        // Original 5 points + 2 control points (endPoint1 and endPoint2) = 7
        Assert.That(splineGeneric.ControlPointCount, Is.EqualTo(7));
    }

    [Test]
    public void CatmullRomSpline_IsValid_ShouldReturnTrue()
    {
        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            false,
            ScalarProcessor
        );

        Assert.That(splineGeneric.IsValid(), Is.True);
    }

    [Test]
    public void CatmullRomSpline_CurveType_ShouldMatch()
    {
        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            false,
            ScalarProcessor
        );

        Assert.That(splineGeneric.CurveType, Is.EqualTo(CatmullRomSplineType.Centripetal));
    }

    [Test]
    public void CatmullRomSpline_IsClosed_ShouldMatchConstructorParameter()
    {
        var controlPointsGeneric = CreateTestControlPointsGeneric(ScalarProcessor);
        var splineGeneric = new CatmullRomSplinePath3D<double>(
            false,
            controlPointsGeneric,
            CatmullRomSplineType.Centripetal,
            true,
            ScalarProcessor
        );

        Assert.That(splineGeneric.IsClosed, Is.True);
    }
}
