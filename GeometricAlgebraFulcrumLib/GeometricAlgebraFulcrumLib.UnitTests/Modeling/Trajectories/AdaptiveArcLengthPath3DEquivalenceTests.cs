using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Circles;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Mapped;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Adaptive;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Adaptive;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Circles;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public sealed class AdaptiveArcLengthPath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static readonly double[] SampleParameters = { 0.0, 0.25, 0.5, 0.75, 1.0 };

    private static LinVector3D<double> CreateVector(double x, double y, double z)
    {
        return LinVector3D<double>.Create(ScalarProcessor, x, y, z);
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

    private static void AssertFramesAreEqual(Float64Path3DLocalFrame expected, ParametricPath3DLocalFrame<double> actual, string context)
    {
        AssertVectorsAreEqual(expected.Point, actual.Point, $"{context} Point");
        AssertVectorsAreEqual(expected.Tangent, actual.Tangent, $"{context} Tangent");
        AssertVectorsAreEqual(expected.Normal1, actual.Normal1, $"{context} Normal1");
        AssertVectorsAreEqual(expected.Normal2, actual.Normal2, $"{context} Normal2");
    }

    [Test]
    public void AdaptiveArcLengthPath3D_DefaultOptions_ShouldMatchFloat64()
    {
        var point1Float = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var point2Float = LinFloat64Vector3D.Create(4.5, -1.0, 2.0);

        var point1Generic = CreateVector(1.0, 2.0, 3.0);
        var point2Generic = CreateVector(4.5, -1.0, 2.0);

        var floatBase = new Float64LineSegmentPath3D(false, point1Float, point2Float);
        var genericBase = LineSegmentPath3D<double>.Create(false, point1Generic, point2Generic);

        var floatArc = Float64AdaptiveArcLengthPath3D.Create(floatBase);
        var genericArc = AdaptiveArcLengthPath3D<double>.Create(genericBase);

        Assert.That(genericArc.GetLength().ScalarValue, Is.EqualTo(floatArc.GetLength().ScalarValue).Within(Tolerance), "Length");

        foreach (var t in SampleParameters)
        {
            var scalarT = CreateScalar(t);
            AssertVectorsAreEqual(floatArc.GetValue(t), genericArc.GetValue(scalarT), $"Value t={t}");
            AssertVectorsAreEqual(floatArc.GetDerivative1Value(t), genericArc.GetDerivative1Value(scalarT), $"Derivative1 t={t}");
            AssertVectorsAreEqual(floatArc.GetDerivative2Value(t), genericArc.GetDerivative2Value(scalarT), $"Derivative2 t={t}");
            AssertFramesAreEqual(floatArc.GetFrame(t), genericArc.GetFrame(scalarT), $"Frame t={t}");

            var floatLength = floatArc.TimeToLength(t).ScalarValue;
            var genericLength = genericArc.TimeToLength(scalarT).ScalarValue;
            Assert.That(genericLength, Is.EqualTo(floatLength).Within(Tolerance), $"TimeToLength t={t}");
        }

        var floatArcLength = floatArc.GetLength().ScalarValue;
        var lengthsToTest = new[] { 0.0, floatArcLength / 2.0, floatArcLength };

        foreach (var length in lengthsToTest)
        {
            var floatTime = floatArc.LengthToTime(length).ScalarValue;
            var genericTime = genericArc.LengthToTime(CreateScalar(length)).ScalarValue;
            Assert.That(genericTime, Is.EqualTo(floatTime).Within(Tolerance), $"LengthToTime length={length}");
        }
    }

    [Test]
    public void AdaptiveArcLengthPath3D_CustomOptions_ShouldMatchFloat64()
    {
        var floatOptions = new Float64AdaptivePath3DSamplingOptions(10.DegreesToDirectedAngle(), 2, 12)
        {
            MaxEdgeFramesDistance = 1e-4,
            MaxEdgeFramesParameterDistance = 0.05
        };

        var genericAngle = LinDirectedAngle<double>.CreateFromDegrees(ScalarProcessor, 10);
        var genericOptions = new AdaptivePath3DSamplingOptions<double>(ScalarProcessor, genericAngle, 2, 12)
        {
            MaxEdgeFramesDistance = ScalarProcessor.ScalarFromNumber(1e-4),
            MaxEdgeFramesParameterDistance = ScalarProcessor.ScalarFromNumber(0.05)
        };

        var centerFloat = LinFloat64Vector3D.Create(1.0, -2.0, 0.5);
        var unitNormalFloat = LinFloat64Vector3D.Create(0.0, 0.0, 1.0);
        var floatCircle = new Float64CirclePath3D(centerFloat, unitNormalFloat, 2.5, rotationCount: 2);

        var centerGeneric = CreateVector(1.0, -2.0, 0.5);
        var unitNormalGeneric = CreateVector(0.0, 0.0, 1.0);
        var genericCircle = CirclePath3D<double>.Create(ScalarProcessor, centerGeneric, unitNormalGeneric, 2.5, rotationCount: 2);

        var floatArc = Float64AdaptiveArcLengthPath3D.Create(floatCircle, floatOptions);
        var genericArc = AdaptiveArcLengthPath3D<double>.Create(genericCircle, genericOptions);

        Assert.That(genericArc.GetLength().ScalarValue, Is.EqualTo(floatArc.GetLength().ScalarValue).Within(Tolerance), "Length");

        foreach (var t in SampleParameters)
        {
            var scalarT = CreateScalar(t);
            AssertVectorsAreEqual(floatArc.GetValue(t), genericArc.GetValue(scalarT), $"Value t={t}");
            AssertVectorsAreEqual(floatArc.GetDerivative1Value(t), genericArc.GetDerivative1Value(scalarT), $"Derivative1 t={t}");
            AssertVectorsAreEqual(floatArc.GetDerivative2Value(t), genericArc.GetDerivative2Value(scalarT), $"Derivative2 t={t}");
            AssertFramesAreEqual(floatArc.GetFrame(t), genericArc.GetFrame(scalarT), $"Frame t={t}");

            var floatLength = floatArc.TimeToLength(t).ScalarValue;
            var genericLength = genericArc.TimeToLength(scalarT).ScalarValue;
            Assert.That(genericLength, Is.EqualTo(floatLength).Within(Tolerance), $"TimeToLength t={t}");
        }

        var floatArcLength = floatArc.GetLength().ScalarValue;
        var lengthsToTest = new[] { 0.0, floatArcLength / 3.0, floatArcLength };

        foreach (var length in lengthsToTest)
        {
            var floatTime = floatArc.LengthToTime(length).ScalarValue;
            var genericTime = genericArc.LengthToTime(CreateScalar(length)).ScalarValue;
            Assert.That(genericTime, Is.EqualTo(floatTime).Within(Tolerance), $"LengthToTime length={length}");
        }
    }
}
