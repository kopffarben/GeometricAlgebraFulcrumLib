using System;
using System.Linq;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Basis;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Composers;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Composers;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public sealed class Path3DComposerUtilsEquivalenceTests
{
    private const double Tolerance = 1e-12;

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static readonly double[] SampleParameters = { 0.0, 0.25, 0.5, 0.75, 1.0 };

    private static LinVector3D<double> CreateVector(double x, double y, double z)
    {
        return LinVector3D<double>.Create(ScalarProcessor, x, y, z);
    }

    private static LinVector2D<double> CreateVector2D(double x, double y)
    {
        return LinVector2D<double>.Create(ScalarProcessor, x, y);
    }

    private static Scalar<double> CreateScalar(double value)
    {
        return ScalarProcessor.Scalar(value);
    }

    private static void AssertVectorsAreEqual(ILinFloat64Vector3D expected, LinVector3D<double> actual, string context)
    {
        Assert.That(actual.X.ScalarValue, Is.EqualTo(expected.X.ScalarValue).Within(Tolerance), $"{context} X");
        Assert.That(actual.Y.ScalarValue, Is.EqualTo(expected.Y.ScalarValue).Within(Tolerance), $"{context} Y");
        Assert.That(actual.Z.ScalarValue, Is.EqualTo(expected.Z.ScalarValue).Within(Tolerance), $"{context} Z");
    }

    private static void AssertVectorsAreEqual(LinFloat64Vector2D expected, LinVector2D<double> actual, string context)
    {
        Assert.That(actual.X.ScalarValue, Is.EqualTo(expected.X.ScalarValue).Within(Tolerance), $"{context} X");
        Assert.That(actual.Y.ScalarValue, Is.EqualTo(expected.Y.ScalarValue).Within(Tolerance), $"{context} Y");
    }

    private static void AssertScalarsAreEqual(double expected, Scalar<double> actual, string context)
    {
        Assert.That(actual.ScalarValue, Is.EqualTo(expected).Within(Tolerance), context);
    }

    [Test]
    public void ScalarSignal_ToParametricCurve3D_ShouldMatchFloat64()
    {
        var floatSignal = Float64ScalarComputedSignal.Finite(
            Float64ScalarRange.ZeroToOne,
            t => Math.Sin(Math.PI * t)
        );

        var genericSignal = ComputedScalarSignal<double>.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            t => ScalarProcessor.Scalar(Math.Sin(Math.PI * t.ScalarValue))
        );

        var floatCurve = Float64Path3DComposerUtils.ToParametricCurve3D(
            floatSignal,
            value => LinFloat64Vector3D.Create(value, 2 * value, value * value)
        );

        var genericCurve = Path3DComposerUtils.ToParametricCurve3D(
            genericSignal,
            value => CreateVector(value.ScalarValue, 2 * value.ScalarValue, value.ScalarValue * value.ScalarValue)
        );

        foreach (var t in SampleParameters)
        {
            var floatValue = floatCurve.GetValue(t);
            var genericValue = genericCurve.GetValue(CreateScalar(t));

            AssertVectorsAreEqual(floatValue, genericValue, $"Scalar signal map at t={t}");
        }
    }

    [Test]
    public void Path2D_ToParametricCurve3D_ShouldMatchFloat64()
    {
        var point1Float = LinFloat64Vector2D.Create(0.0, 0.0);
        var point2Float = LinFloat64Vector2D.Create(2.0, 3.0);

        var point1Generic = CreateVector2D(0.0, 0.0);
        var point2Generic = CreateVector2D(2.0, 3.0);

        var floatPath = Float64LineSegmentPath2D.Create(false, point1Float, point2Float);
        var genericPath = LineSegmentPath2D<double>.Create(false, point1Generic, point2Generic);

        var floatCurve = Float64Path3DComposerUtils.ToParametricCurve3D(
            floatPath,
            v => LinFloat64Vector3D.Create(v.X.ScalarValue, v.Y.ScalarValue, 1.0)
        );

        var genericCurve = Path3DComposerUtils.ToParametricCurve3D(
            genericPath,
            v => CreateVector(v.X.ScalarValue, v.Y.ScalarValue, 1.0)
        );

        foreach (var t in SampleParameters)
        {
            var floatValue = floatCurve.GetValue(t);
            var genericValue = genericCurve.GetValue(CreateScalar(t));

            AssertVectorsAreEqual(floatValue, genericValue, $"2D mapping at t={t}");
        }
    }

    [Test]
    public void Path2D_ToXyParametricCurve3D_ShouldMatchFloat64()
    {
        var point1Float = LinFloat64Vector2D.Create(1.0, -1.0);
        var point2Float = LinFloat64Vector2D.Create(4.0, 2.0);

        var point1Generic = CreateVector2D(1.0, -1.0);
        var point2Generic = CreateVector2D(4.0, 2.0);

        var floatPath = Float64LineSegmentPath2D.Create(false, point1Float, point2Float);
        var genericPath = LineSegmentPath2D<double>.Create(false, point1Generic, point2Generic);

        var floatCurve = Float64Path3DComposerUtils.ToXyParametricCurve3D(floatPath);
        var genericCurve = Path3DComposerUtils.ToXyParametricCurve3D(genericPath);

        foreach (var t in SampleParameters)
        {
            var floatValue = floatCurve.GetValue(t);
            var genericValue = genericCurve.GetValue(CreateScalar(t));

            AssertVectorsAreEqual(floatValue, genericValue, $"XY lift at t={t}");
        }
    }

    [Test]
    public void Path3D_ToParametricCurve3D_ShouldMatchFloat64()
    {
        var point1Float = LinFloat64Vector3D.Create(0.0, 1.0, 2.0);
        var point2Float = LinFloat64Vector3D.Create(4.0, -1.0, 3.0);

        var point1Generic = CreateVector(0.0, 1.0, 2.0);
        var point2Generic = CreateVector(4.0, -1.0, 3.0);

        var floatPath = new Float64LineSegmentPath3D(false, point1Float, point2Float);
        var genericPath = LineSegmentPath3D<double>.Create(false, point1Generic, point2Generic);

        var floatCurve = Float64Path3DComposerUtils.ToParametricCurve3D(
            floatPath,
            v => LinFloat64Vector3D.Create(
                2 * v.X.ScalarValue + 1,
                3 * v.Y.ScalarValue - 2,
                v.Z.ScalarValue + 5
            )
        );

        var genericCurve = Path3DComposerUtils.ToParametricCurve3D(
            genericPath,
            v => CreateVector(2 * v.X.ScalarValue + 1, 3 * v.Y.ScalarValue - 2, v.Z.ScalarValue + 5)
        );

        foreach (var t in SampleParameters)
        {
            var floatValue = floatCurve.GetValue(t);
            var genericValue = genericCurve.GetValue(CreateScalar(t));

            AssertVectorsAreEqual(floatValue, genericValue, $"3D mapping at t={t}");
        }
    }

    [Test]
    public void CreateBezier3D_ShouldMatchFloat64()
    {
        var p1Float = LinFloat64Vector3D.Create(0.0, 0.0, 0.0);
        var p2Float = LinFloat64Vector3D.Create(1.0, 2.0, 0.0);
        var p3Float = LinFloat64Vector3D.Create(2.0, -1.0, 1.0);
        var p4Float = LinFloat64Vector3D.Create(3.0, 0.0, 2.0);

        var p1Generic = CreateVector(0.0, 0.0, 0.0);
        var p2Generic = CreateVector(1.0, 2.0, 0.0);
        var p3Generic = CreateVector(2.0, -1.0, 1.0);
        var p4Generic = CreateVector(3.0, 0.0, 2.0);

        var floatCurve = Float64Path3DComposerUtils.CreateBezier3D(p1Float, p2Float, p3Float, p4Float);
        var genericCurve = Path3DComposerUtils.CreateBezier3D(p1Generic, p2Generic, p3Generic, p4Generic);

        foreach (var t in SampleParameters)
        {
            var floatValue = floatCurve.GetValue(t);
            var genericValue = genericCurve.GetValue(CreateScalar(t));

            AssertVectorsAreEqual(floatValue, genericValue, $"Bezier3 at t={t}");
        }
    }

    [Test]
    public void CreateCircle3D_AxisAligned_ShouldMatchFloat64()
    {
        var floatCircle = LinBasisVector.Pz.CreateCircle3D(2.0, rotationCount: 2);
        var genericCircle = LinBasisVector.Pz.CreateCircle3D(ScalarProcessor, 2.0, rotationCount: 2);

        foreach (var t in SampleParameters)
        {
            var floatValue = floatCircle.GetValue(t);
            var genericValue = genericCircle.GetValue(CreateScalar(t));

            AssertVectorsAreEqual(floatValue, genericValue, $"Axis-aligned circle at t={t}");
        }
    }

    [Test]
    public void CreateCircle3D_WithCenter_ShouldMatchFloat64()
    {
        var unitNormalFloat = LinFloat64Vector3D.Create(1.0, 1.0, 1.0).ToUnitLinVector3D();
        var centerFloat = LinFloat64Vector3D.Create(1.0, -1.0, 0.5);

        var unitNormalGeneric = CreateVector(1.0, 1.0, 1.0).ToUnitLinVector3D();
        var centerGeneric = CreateVector(1.0, -1.0, 0.5);

        var floatCircle = unitNormalFloat.CreateCircle3D(centerFloat, 1.5, rotationCount: 1);
        var genericCircle = unitNormalGeneric.CreateCircle3D(centerGeneric, 1.5, rotationCount: 1);

        foreach (var t in SampleParameters)
        {
            var floatValue = floatCircle.GetValue(t);
            var genericValue = genericCircle.GetValue(CreateScalar(t));

            AssertVectorsAreEqual(floatValue, genericValue, $"General circle at t={t}");
        }
    }

    [Test]
    public void CreateMathCurve2D_ShouldMatchFloat64()
    {
        Func<double, double> funcFloat = x => x * x + 1;
        Func<Scalar<double>, Scalar<double>> funcGeneric = x => ScalarProcessor.Scalar(x.ScalarValue * x.ScalarValue + 1);

        var floatCurve = Float64Path3DComposerUtils.CreateMathCurve2D(Float64ScalarRange.ZeroToOne, funcFloat);
        var genericCurve = Path3DComposerUtils.CreateMathCurve2D(ScalarRange<double>.ZeroToOne(ScalarProcessor), funcGeneric);

        foreach (var t in SampleParameters)
        {
            var floatValue = floatCurve.GetValue(t);
            var genericValue = genericCurve.GetValue(CreateScalar(t));

            AssertVectorsAreEqual(floatValue, genericValue, $"Math curve at t={t}");
        }
    }

    [Test]
    public void GetOffsetCurve_WithConstantVector_ShouldMatchFloat64()
    {
        var baseFloat = new Float64LineSegmentPath3D(
            false,
            LinFloat64Vector3D.Create(0.0, 0.0, 0.0),
            LinFloat64Vector3D.Create(1.0, 1.0, 1.0)
        );

        var baseGeneric = LineSegmentPath3D<double>.Create(
            false,
            CreateVector(0.0, 0.0, 0.0),
            CreateVector(1.0, 1.0, 1.0)
        );

        var offsetFloat = Float64Path3DComposerUtils.GetOffsetCurve(baseFloat, 1.0, -2.0, 0.5);
        var offsetGeneric = Path3DComposerUtils.GetOffsetCurve(baseGeneric, 1.0, -2.0, 0.5);

        foreach (var t in SampleParameters)
        {
            var floatValue = offsetFloat.GetValue(t);
            var genericValue = offsetGeneric.GetValue(CreateScalar(t));

            AssertVectorsAreEqual(floatValue, genericValue, $"Offset curve at t={t}");
        }
    }

    [Test]
    public void GetDistanceCurve_BetweenCurves_ShouldMatchFloat64()
    {
        var curve1Float = new Float64LineSegmentPath3D(
            false,
            LinFloat64Vector3D.Create(0.0, 0.0, 0.0),
            LinFloat64Vector3D.Create(2.0, 0.0, 0.0)
        );

        var curve2Float = new Float64LineSegmentPath3D(
            false,
            LinFloat64Vector3D.Create(0.0, 1.0, 0.0),
            LinFloat64Vector3D.Create(2.0, 1.0, 0.0)
        );

        var curve1Generic = LineSegmentPath3D<double>.Create(
            false,
            CreateVector(0.0, 0.0, 0.0),
            CreateVector(2.0, 0.0, 0.0)
        );

        var curve2Generic = LineSegmentPath3D<double>.Create(
            false,
            CreateVector(0.0, 1.0, 0.0),
            CreateVector(2.0, 1.0, 0.0)
        );

        var floatSignal = Float64Path3DComposerUtils.GetDistanceCurve(curve1Float, curve2Float);
        var genericSignal = Path3DComposerUtils.GetDistanceCurve(curve1Generic, curve2Generic);

        foreach (var t in SampleParameters)
        {
            var floatValue = floatSignal.GetValue(t);
            var genericValue = genericSignal.GetValue(CreateScalar(t));

            AssertScalarsAreEqual(floatValue, genericValue, $"Distance curve at t={t}");
        }
    }

    [Test]
    public void GetMidPointCurve_ShouldMatchFloat64()
    {
        var baseFloat = new Float64LineSegmentPath3D(
            false,
            LinFloat64Vector3D.Create(0.0, 0.0, 0.0),
            LinFloat64Vector3D.Create(2.0, 2.0, 2.0)
        );

        var baseGeneric = LineSegmentPath3D<double>.Create(
            false,
            CreateVector(0.0, 0.0, 0.0),
            CreateVector(2.0, 2.0, 2.0)
        );

        var floatCurve = Float64Path3DComposerUtils.GetMidPointCurve(baseFloat, LinFloat64Vector3D.Create(2.0, -2.0, 0.0));
        var genericCurve = Path3DComposerUtils.GetMidPointCurve(baseGeneric, CreateVector(2.0, -2.0, 0.0));

        foreach (var t in SampleParameters)
        {
            var floatValue = floatCurve.GetValue(t);
            var genericValue = genericCurve.GetValue(CreateScalar(t));

            AssertVectorsAreEqual(floatValue, genericValue, $"Midpoint curve at t={t}");
        }
    }

    [Test]
    public void GetMedianPointCurve_ShouldMatchFloat64()
    {
        var curve1Float = new Float64LineSegmentPath3D(
            false,
            LinFloat64Vector3D.Create(0.0, 0.0, 0.0),
            LinFloat64Vector3D.Create(3.0, 0.0, 0.0)
        );

        var curve2Float = new Float64LineSegmentPath3D(
            false,
            LinFloat64Vector3D.Create(0.0, 3.0, 0.0),
            LinFloat64Vector3D.Create(3.0, 3.0, 0.0)
        );

        var curve3Float = new Float64LineSegmentPath3D(
            false,
            LinFloat64Vector3D.Create(0.0, 0.0, 3.0),
            LinFloat64Vector3D.Create(3.0, 0.0, 3.0)
        );

        var curve1Generic = LineSegmentPath3D<double>.Create(false, CreateVector(0.0, 0.0, 0.0), CreateVector(3.0, 0.0, 0.0));
        var curve2Generic = LineSegmentPath3D<double>.Create(false, CreateVector(0.0, 3.0, 0.0), CreateVector(3.0, 3.0, 0.0));
        var curve3Generic = LineSegmentPath3D<double>.Create(false, CreateVector(0.0, 0.0, 3.0), CreateVector(3.0, 0.0, 3.0));

        var floatCurve = Float64Path3DComposerUtils.GetMedianPointCurve(curve1Float, curve2Float, curve3Float);
        var genericCurve = Path3DComposerUtils.GetMedianPointCurve(curve1Generic, curve2Generic, curve3Generic);

        foreach (var t in SampleParameters)
        {
            var floatValue = floatCurve.GetValue(t);
            var genericValue = genericCurve.GetValue(CreateScalar(t));

            AssertVectorsAreEqual(floatValue, genericValue, $"Median curve at t={t}");
        }
    }

    [Test]
    public void GetPlaneNormalCurve_ShouldMatchFloat64()
    {
        var curve1Float = new Float64LineSegmentPath3D(false, LinFloat64Vector3D.Create(0.0, 0.0, 0.0), LinFloat64Vector3D.Create(1.0, 0.0, 0.0));
        var curve2Float = new Float64LineSegmentPath3D(false, LinFloat64Vector3D.Create(0.0, 1.0, 0.0), LinFloat64Vector3D.Create(1.0, 1.0, 0.0));
        var curve3Float = new Float64LineSegmentPath3D(false, LinFloat64Vector3D.Create(0.0, 0.0, 1.0), LinFloat64Vector3D.Create(1.0, 0.0, 1.0));

        var curve1Generic = LineSegmentPath3D<double>.Create(false, CreateVector(0.0, 0.0, 0.0), CreateVector(1.0, 0.0, 0.0));
        var curve2Generic = LineSegmentPath3D<double>.Create(false, CreateVector(0.0, 1.0, 0.0), CreateVector(1.0, 1.0, 0.0));
        var curve3Generic = LineSegmentPath3D<double>.Create(false, CreateVector(0.0, 0.0, 1.0), CreateVector(1.0, 0.0, 1.0));

        var floatCurve = Float64Path3DComposerUtils.GetPlaneNormalCurve(curve1Float, curve2Float, curve3Float);
        var genericCurve = Path3DComposerUtils.GetPlaneNormalCurve(curve1Generic, curve2Generic, curve3Generic);

        foreach (var t in SampleParameters)
        {
            var floatValue = floatCurve.GetValue(t);
            var genericValue = genericCurve.GetValue(CreateScalar(t));

            AssertVectorsAreEqual(floatValue, genericValue, $"Plane normal curve at t={t}");
        }
    }
}
