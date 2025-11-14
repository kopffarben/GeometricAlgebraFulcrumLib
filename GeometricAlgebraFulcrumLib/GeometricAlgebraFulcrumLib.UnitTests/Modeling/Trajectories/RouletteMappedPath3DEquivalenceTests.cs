using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.AffineMaps.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Circles;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Mapped;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Circles;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public sealed class RouletteMappedPath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static readonly double[] SampleParameters = { 0.0, 0.25, 0.5, 0.75, 1.0 };

    private static LinVector3D<double> CreateVector(double x, double y, double z)
    {
        return LinVector3D<double>.Create(ScalarProcessor, x, y, z);
    }

    private static void AssertVectorsAreEqual(LinFloat64Vector3D expected, LinVector3D<double> actual, string context)
    {
        Assert.That(actual.X.ScalarValue, Is.EqualTo(expected.X).Within(Tolerance), $"{context} X");
        Assert.That(actual.Y.ScalarValue, Is.EqualTo(expected.Y).Within(Tolerance), $"{context} Y");
        Assert.That(actual.Z.ScalarValue, Is.EqualTo(expected.Z).Within(Tolerance), $"{context} Z");
    }

    private static void AssertFramesAreEqual(Float64Path3DLocalFrame expected, ParametricPath3DLocalFrame<double> actual, string context)
    {
        AssertVectorsAreEqual(expected.Point, actual.Point, $"{context} Point");
        AssertVectorsAreEqual(expected.Tangent, actual.Tangent, $"{context} Tangent");
        AssertVectorsAreEqual(expected.Normal1, actual.Normal1, $"{context} Normal1");
        AssertVectorsAreEqual(expected.Normal2, actual.Normal2, $"{context} Normal2");
    }

    [Test]
    public void RouletteMappedPath3D_ShouldMatchFloat64()
    {
        var floatBase = new Float64CirclePath3D(
            LinFloat64Vector3D.Create(0.5, -0.25, 1.0),
            LinFloat64Vector3D.Create(0.0, 0.0, 1.0),
            2.0,
            rotationCount: 1);

        var genericBase = CirclePath3D<double>.Create(
            ScalarProcessor,
            CreateVector(0.5, -0.25, 1.0),
            CreateVector(0.0, 0.0, 1.0),
            2.0,
            rotationCount: 1);

        var fixedOriginFloat = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var movingOriginFloat = LinFloat64Vector3D.Create(-0.5, 0.25, -1.0);
        var rotationFloat = LinFloat64Quaternion.CreateFromAxisAngle(
            LinFloat64Vector3D.Create(0.0, 1.0, 0.0),
            LinFloat64PolarAngle.Angle30);

        var fixedOriginGeneric = CreateVector(1.0, 2.0, 3.0);
        var movingOriginGeneric = CreateVector(-0.5, 0.25, -1.0);
        var rotationGeneric = LinQuaternion<double>.CreateFromAxisAngle(
            CreateVector(0.0, 1.0, 0.0),
            LinPolarAngle<double>.Angle30(ScalarProcessor));

        var floatMap = new Float64RouletteAffineMap3D(fixedOriginFloat, movingOriginFloat, rotationFloat);
        var genericMap = new RouletteAffineMap3D<double>(fixedOriginGeneric, movingOriginGeneric, rotationGeneric);

        var floatPath = new Float64RouletteMappedPath3D(floatBase, floatMap);
        var genericPath = new RouletteMappedPath3D<double>(genericBase, genericMap);

        foreach (var t in SampleParameters)
        {
            var scalarT = ScalarProcessor.Scalar(t);
            AssertVectorsAreEqual(floatPath.GetValue(t), genericPath.GetValue(scalarT), $"Value t={t}");
            AssertVectorsAreEqual(floatPath.GetDerivative1Value(t), genericPath.GetDerivative1Value(scalarT), $"Derivative1 t={t}");
            AssertVectorsAreEqual(floatPath.GetDerivative2Value(t), genericPath.GetDerivative2Value(scalarT), $"Derivative2 t={t}");
            AssertFramesAreEqual(floatPath.GetFrame(t), genericPath.GetFrame(scalarT), $"Frame t={t}");

            var floatLength = floatPath.TimeToLength(t);
            var genericLength = genericPath.TimeToLength(scalarT).ScalarValue;
            Assert.That(genericLength, Is.EqualTo(floatLength).Within(Tolerance), $"TimeToLength t={t}");
        }

        var lengths = new[] { 0.0, floatPath.GetLength() / 2.0, floatPath.GetLength() };
        foreach (var length in lengths)
        {
            var floatTime = floatPath.LengthToTime(length);
            var genericTime = genericPath.LengthToTime(ScalarProcessor.Scalar(length)).ScalarValue;
            Assert.That(genericTime, Is.EqualTo(floatTime).Within(Tolerance), $"LengthToTime length={length}");
        }
    }
}
