using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public sealed class RoulettePath3DEquivalenceTests
{
    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    private const double Tolerance = 1e-10;

    [Test]
    public void RoulettePathMatchesFloat64Baseline()
    {
        var fixedCurveGeneric = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0d, 0d, 0d),
            LinVector3D<double>.Create(ScalarProcessor, 0d, 1d, 0d)
        );

        var movingCurveGeneric = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0d, 0d, 0d),
            LinVector3D<double>.Create(ScalarProcessor, 1d, 0d, 0d)
        );

        var parameterMax = ScalarProcessor.ScalarFromNumber(1d);

        var rouletteGeneric = new RoulettePath3D<double>(
            false,
            fixedCurveGeneric,
            movingCurveGeneric,
            parameterMax
        );

        var fixedCurveFloat = new Float64LineSegmentPath3D(
            false,
            LinFloat64Vector3D.Create(0d, 0d, 0d),
            LinFloat64Vector3D.Create(0d, 1d, 0d)
        );

        var movingCurveFloat = new Float64LineSegmentPath3D(
            false,
            LinFloat64Vector3D.Create(0d, 0d, 0d),
            LinFloat64Vector3D.Create(1d, 0d, 0d)
        );

        var rouletteFloat = new Float64RoulettePath3D(
            false,
            fixedCurveFloat,
            movingCurveFloat,
            parameterValueMax: 1d
        );

        foreach (var sample in new[] { 0d, 0.25d, 0.5d, 0.75d, 1d })
        {
            var genericPoint = rouletteGeneric.GetValue(ScalarProcessor.ScalarFromNumber(sample));
            var floatPoint = rouletteFloat.GetValue(sample);

            Assert.That(genericPoint.X.ScalarValue, Is.EqualTo(floatPoint.X.ScalarValue).Within(Tolerance));
            Assert.That(genericPoint.Y.ScalarValue, Is.EqualTo(floatPoint.Y.ScalarValue).Within(Tolerance));
            Assert.That(genericPoint.Z.ScalarValue, Is.EqualTo(floatPoint.Z.ScalarValue).Within(Tolerance));
        }
    }
}
