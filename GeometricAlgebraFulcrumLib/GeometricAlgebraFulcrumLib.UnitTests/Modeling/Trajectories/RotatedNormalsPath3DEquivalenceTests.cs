using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Angles;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Angles;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Mapped;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public sealed class RotatedNormalsPath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static readonly double[] SampleParameters = { 0.0, 0.25, 0.5, 0.75, 1.0 };

    private static LinVector3D<double> CreateVector(double x, double y, double z)
    {
        return LinVector3D<double>.Create(ScalarProcessor, x, y, z);
    }

    private static void AssertVectorsAreEqual(ILinFloat64Vector3D expected, LinVector3D<double> actual, string context)
    {
        Assert.That(actual.X.ScalarValue, Is.EqualTo(expected.X.ScalarValue).Within(Tolerance), $"{context} X");
        Assert.That(actual.Y.ScalarValue, Is.EqualTo(expected.Y.ScalarValue).Within(Tolerance), $"{context} Y");
        Assert.That(actual.Z.ScalarValue, Is.EqualTo(expected.Z.ScalarValue).Within(Tolerance), $"{context} Z");
    }

    private static void AssertFramesAreEqual(Float64Path3DLocalFrame expected, ParametricPath3DLocalFrame<double> actual, string context)
    {
        AssertVectorsAreEqual(expected.Point, actual.Point, $"{context} Point");
        AssertVectorsAreEqual(expected.Tangent, actual.Tangent, $"{context} Tangent");
        AssertVectorsAreEqual(expected.Normal1, actual.Normal1, $"{context} Normal1");
        AssertVectorsAreEqual(expected.Normal2, actual.Normal2, $"{context} Normal2");
    }

    [Test]
    public void RotatedNormalsPath3D_ConstantAngle_ShouldMatchFloat64()
    {
        var baseFloat = new Float64LineSegmentPath3D(
            false,
            LinFloat64Vector3D.Create(0.0, 0.0, 0.0),
            LinFloat64Vector3D.Create(1.0, 2.0, 3.0)
        );

        var baseGeneric = LineSegmentPath3D<double>.Create(
            false,
            CreateVector(0.0, 0.0, 0.0),
            CreateVector(1.0, 2.0, 3.0)
        );

        var floatAngle = LinFloat64PolarAngleTimeSignal.CreateConstant(
            baseFloat.TimeRange,
            baseFloat.IsPeriodic,
            LinFloat64PolarAngle.Angle90
        );

        var genericAngle = LinPolarAngleTimeSignal<double>.CreateConstant(
            baseGeneric.TimeRange,
            baseGeneric.IsPeriodic,
            LinPolarAngle<double>.Angle90(ScalarProcessor)
        );

        var floatPath = new Float64RotatedNormalsPath3D(baseFloat, floatAngle);
        var genericPath = new RotatedNormalsPath3D<double>(baseGeneric, genericAngle);

        foreach (var t in SampleParameters)
        {
            var scalarT = ScalarProcessor.Scalar(t);
            AssertVectorsAreEqual(floatPath.GetValue(t), genericPath.GetValue(scalarT), $"Value t={t}");
            AssertVectorsAreEqual(floatPath.GetDerivative1Value(t), genericPath.GetDerivative1Value(scalarT), $"Derivative1 t={t}");
            AssertVectorsAreEqual(floatPath.GetDerivative2Value(t), genericPath.GetDerivative2Value(scalarT), $"Derivative2 t={t}");
            AssertFramesAreEqual(floatPath.GetFrame(t), genericPath.GetFrame(scalarT), $"Frame t={t}");
        }
    }

    [Test]
    public void RotatedNormalsPath3D_FunctionAngle_ShouldMatchFloat64()
    {
        var baseFloat = new Float64LineSegmentPath3D(
            false,
            LinFloat64Vector3D.Create(1.0, 0.0, 0.0),
            LinFloat64Vector3D.Create(1.0, 1.0, 1.0)
        );

        var baseGeneric = LineSegmentPath3D<double>.Create(
            false,
            CreateVector(1.0, 0.0, 0.0),
            CreateVector(1.0, 1.0, 1.0)
        );

        Func<double, LinFloat64Angle> floatAngleFunc =
            t => LinFloat64PolarAngle.CreateFromRadians(Math.PI / 4 * Math.Sin(Math.PI * t));

        var floatPath = baseFloat.RotateNormals(floatAngleFunc);

        Func<Scalar<double>, LinPolarAngle<double>> genericAngleFunc =
            t =>
            {
                var radians = ScalarProcessor.Scalar(Math.PI / 4 * Math.Sin(Math.PI * t.ScalarValue));
                return LinPolarAngle<double>.CreateFromRadians(radians);
            };

        var genericPath = baseGeneric.RotateNormals(genericAngleFunc);

        foreach (var t in SampleParameters)
        {
            var scalarT = ScalarProcessor.Scalar(t);
            AssertVectorsAreEqual(floatPath.GetValue(t), genericPath.GetValue(scalarT), $"Value t={t}");
            AssertVectorsAreEqual(floatPath.GetDerivative1Value(t), genericPath.GetDerivative1Value(scalarT), $"Derivative1 t={t}");
            AssertVectorsAreEqual(floatPath.GetDerivative2Value(t), genericPath.GetDerivative2Value(scalarT), $"Derivative2 t={t}");
            AssertFramesAreEqual(floatPath.GetFrame(t), genericPath.GetFrame(scalarT), $"Frame t={t}");
        }
    }
}
