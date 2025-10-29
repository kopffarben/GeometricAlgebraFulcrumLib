using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Quaternions.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Quaternions.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public sealed class ComputedParametricQuaternionEquivalenceTests
{
    private const double Tolerance = 1e-10;

    private IScalarProcessor<double> ScalarProcessor { get; }
        = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestCreateWithPointFunc()
    {
        // Float64 version
        var curveFloat64 = ComputedParametricQuaternion.Create(
            t => LinFloat64Quaternion.Create(
                Math.Sin(t),
                Math.Cos(t),
                t,
                1.0
            )
        );

        // Generic version
        var curveGeneric = ComputedParametricQuaternion<double>.Create(
            ScalarProcessor,
            t => LinQuaternion<double>.Create(
                ScalarProcessor,
                Math.Sin(t.ScalarValue),
                Math.Cos(t.ScalarValue),
                t.ScalarValue,
                1.0
            )
        );

        // Test at various parameter values
        for (var t = -2.0; t <= 2.0; t += 0.5)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var qFloat64 = curveFloat64.GetQuaternion(t);
            var qGeneric = curveGeneric.GetQuaternion(tScalar);

            Assert.That(qGeneric.ScalarI.ScalarValue, Is.EqualTo(qFloat64.ScalarI.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarJ.ScalarValue, Is.EqualTo(qFloat64.ScalarJ.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarK.ScalarValue, Is.EqualTo(qFloat64.ScalarK.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.Scalar.ScalarValue, Is.EqualTo(qFloat64.Scalar.ScalarValue).Within(Tolerance));
        }
    }

    [Test]
    public void TestCreateWithPointAndTangentFuncs()
    {
        // Float64 version
        var curveFloat64 = ComputedParametricQuaternion.Create(
            t => LinFloat64Quaternion.Create(t * t, t, 1.0, 0.0),
            t => LinFloat64Quaternion.Create(2 * t, 1.0, 0.0, 0.0)
        );

        // Generic version
        var curveGeneric = ComputedParametricQuaternion<double>.Create(
            ScalarProcessor,
            t => LinQuaternion<double>.Create(ScalarProcessor, t.ScalarValue * t.ScalarValue, t.ScalarValue, 1.0, 0.0),
            t => LinQuaternion<double>.Create(ScalarProcessor, 2 * t.ScalarValue, 1.0, 0.0, 0.0)
        );

        for (var t = -1.0; t <= 1.0; t += 0.25)
        {
            var tScalar = ScalarProcessor.Scalar(t);

            // Test quaternion values
            var qFloat64 = curveFloat64.GetQuaternion(t);
            var qGeneric = curveGeneric.GetQuaternion(tScalar);

            Assert.That(qGeneric.ScalarI.ScalarValue, Is.EqualTo(qFloat64.ScalarI.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarJ.ScalarValue, Is.EqualTo(qFloat64.ScalarJ.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarK.ScalarValue, Is.EqualTo(qFloat64.ScalarK.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.Scalar.ScalarValue, Is.EqualTo(qFloat64.Scalar.ScalarValue).Within(Tolerance));

            // Test tangent values
            var tFloat64 = curveFloat64.GetDerivative1Quaternion(t);
            var tGeneric = curveGeneric.GetDerivative1Quaternion(tScalar);

            Assert.That(tGeneric.ScalarI.ScalarValue, Is.EqualTo(tFloat64.ScalarI.ScalarValue).Within(Tolerance));
            Assert.That(tGeneric.ScalarJ.ScalarValue, Is.EqualTo(tFloat64.ScalarJ.ScalarValue).Within(Tolerance));
            Assert.That(tGeneric.ScalarK.ScalarValue, Is.EqualTo(tFloat64.ScalarK.ScalarValue).Within(Tolerance));
            Assert.That(tGeneric.Scalar.ScalarValue, Is.EqualTo(tFloat64.Scalar.ScalarValue).Within(Tolerance));
        }
    }

    [Test]
    public void TestCreateWithParameterRange()
    {
        var rangeFloat64 = Float64ScalarRange.Create(0, Math.PI);
        var rangeGeneric = ScalarRange<double>.Create(ScalarProcessor.Scalar(0), ScalarProcessor.Scalar(Math.PI));

        // Float64 version
        var curveFloat64 = ComputedParametricQuaternion.Create(
            rangeFloat64,
            t => LinFloat64Quaternion.Create(Math.Sin(t), Math.Cos(t), 0.0, 1.0)
        );

        // Generic version
        var curveGeneric = ComputedParametricQuaternion<double>.Create(
            ScalarProcessor,
            rangeGeneric,
            t => LinQuaternion<double>.Create(ScalarProcessor, Math.Sin(t.ScalarValue), Math.Cos(t.ScalarValue), 0.0, 1.0)
        );

        // Test parameter range
        Assert.That(curveGeneric.ParameterRange.MinValue.ScalarValue, Is.EqualTo(curveFloat64.TimeRange.MinValue).Within(Tolerance));
        Assert.That(curveGeneric.ParameterRange.MaxValue.ScalarValue, Is.EqualTo(curveFloat64.TimeRange.MaxValue).Within(Tolerance));
    }

    [Test]
    public void TestNumericalDifferentiation()
    {
        // Test that numerical differentiation works when no tangent function provided
        // Float64 version
        var curveFloat64 = ComputedParametricQuaternion.Create(
            t => LinFloat64Quaternion.Create(t * t, t * t * t, Math.Sin(t), 1.0)
        );

        // Generic version
        var curveGeneric = ComputedParametricQuaternion<double>.Create(
            ScalarProcessor,
            t => LinQuaternion<double>.Create(ScalarProcessor, t.ScalarValue * t.ScalarValue, t.ScalarValue * t.ScalarValue * t.ScalarValue, Math.Sin(t.ScalarValue), 1.0)
        );

        // Test derivative at t=1.0
        var t = 1.0;
        var tScalar = ScalarProcessor.Scalar(t);

        var derivFloat64 = curveFloat64.GetDerivative1Quaternion(t);
        var derivGeneric = curveGeneric.GetDerivative1Quaternion(tScalar);

        // Numerical derivatives should be close
        Assert.That(derivGeneric.ScalarI.ScalarValue, Is.EqualTo(derivFloat64.ScalarI.ScalarValue).Within(1e-6));
        Assert.That(derivGeneric.ScalarJ.ScalarValue, Is.EqualTo(derivFloat64.ScalarJ.ScalarValue).Within(1e-6));
        Assert.That(derivGeneric.ScalarK.ScalarValue, Is.EqualTo(derivFloat64.ScalarK.ScalarValue).Within(1e-6));
        Assert.That(derivGeneric.Scalar.ScalarValue, Is.EqualTo(derivFloat64.Scalar.ScalarValue).Within(1e-6));
    }

    // Note: TestCreateWithComponentFunctions omitted
    // Float64 version uses MathNet.Numerics.Differentiate which doesn't support generic T
    // Generic version would need explicit derivative functions or rely on numerical differentiation

    [Test]
    public void TestIsValid()
    {
        var curveGeneric = ComputedParametricQuaternion<double>.Create(
            ScalarProcessor,
            t => LinQuaternion<double>.Create(ScalarProcessor, t.ScalarValue, 0.0, 0.0, 1.0)
        );

        Assert.That(curveGeneric.IsValid(), Is.True);
    }

    [Test]
    public void TestInfiniteParameterRange()
    {
        // Float64 version
        var curveFloat64 = ComputedParametricQuaternion.Create(
            t => LinFloat64Quaternion.Create(t, 0.0, 0.0, 1.0)
        );

        // Generic version
        var curveGeneric = ComputedParametricQuaternion<double>.Create(
            ScalarProcessor,
            t => LinQuaternion<double>.Create(ScalarProcessor, t.ScalarValue, 0.0, 0.0, 1.0)
        );

        Assert.That(curveFloat64.TimeRange.IsInfinite);
        Assert.That(curveGeneric.ParameterRange.IsFinite, Is.False);
    }

    [Test]
    public void TestLinearQuaternion()
    {
        // Linear interpolation: q(t) = q0 + t * (q1 - q0)
        var q0 = LinFloat64Quaternion.Create(0.0, 0.0, 0.0, 1.0);
        var q1 = LinFloat64Quaternion.Create(1.0, 1.0, 1.0, 0.0);
        var delta = q1 - q0;

        // Float64 version
        var curveFloat64 = ComputedParametricQuaternion.Create(
            Float64ScalarRange.Create(0, 1),
            t => q0 + t * delta,
            t => delta
        );

        // Generic version
        var q0Generic = LinQuaternion<double>.Create(ScalarProcessor, 0.0, 0.0, 0.0, 1.0);
        var q1Generic = LinQuaternion<double>.Create(ScalarProcessor, 1.0, 1.0, 1.0, 0.0);
        var deltaGeneric = q1Generic - q0Generic;

        var curveGeneric = ComputedParametricQuaternion<double>.Create(
            ScalarProcessor,
            ScalarRange<double>.Create(ScalarProcessor.Scalar(0.0), ScalarProcessor.Scalar(1.0)),
            t => q0Generic + t.ScalarValue * deltaGeneric,
            t => deltaGeneric
        );

        for (var t = 0.0; t <= 1.0; t += 0.1)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var qFloat64 = curveFloat64.GetQuaternion(t);
            var qGeneric = curveGeneric.GetQuaternion(tScalar);

            Assert.That(qGeneric.ScalarI.ScalarValue, Is.EqualTo(qFloat64.ScalarI.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarJ.ScalarValue, Is.EqualTo(qFloat64.ScalarJ.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarK.ScalarValue, Is.EqualTo(qFloat64.ScalarK.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.Scalar.ScalarValue, Is.EqualTo(qFloat64.Scalar.ScalarValue).Within(Tolerance));

            var tFloat64 = curveFloat64.GetDerivative1Quaternion(t);
            var tGeneric = curveGeneric.GetDerivative1Quaternion(tScalar);

            Assert.That(tGeneric.ScalarI.ScalarValue, Is.EqualTo(tFloat64.ScalarI.ScalarValue).Within(Tolerance));
            Assert.That(tGeneric.ScalarJ.ScalarValue, Is.EqualTo(tFloat64.ScalarJ.ScalarValue).Within(Tolerance));
            Assert.That(tGeneric.ScalarK.ScalarValue, Is.EqualTo(tFloat64.ScalarK.ScalarValue).Within(Tolerance));
            Assert.That(tGeneric.Scalar.ScalarValue, Is.EqualTo(tFloat64.Scalar.ScalarValue).Within(Tolerance));
        }
    }

    [Test]
    public void TestTrigonometricQuaternion()
    {
        // Quaternion with trig functions
        // Float64 version
        var curveFloat64 = ComputedParametricQuaternion.Create(
            t => LinFloat64Quaternion.Create(Math.Sin(t), Math.Cos(t), Math.Sin(2 * t), Math.Cos(2 * t))
        );

        // Generic version
        var curveGeneric = ComputedParametricQuaternion<double>.Create(
            ScalarProcessor,
            t => LinQuaternion<double>.Create(
                ScalarProcessor,
                Math.Sin(t.ScalarValue),
                Math.Cos(t.ScalarValue),
                Math.Sin(2 * t.ScalarValue),
                Math.Cos(2 * t.ScalarValue)
            )
        );

        for (var t = 0.0; t <= Math.PI; t += Math.PI / 10)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var qFloat64 = curveFloat64.GetQuaternion(t);
            var qGeneric = curveGeneric.GetQuaternion(tScalar);

            Assert.That(qGeneric.ScalarI.ScalarValue, Is.EqualTo(qFloat64.ScalarI.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarJ.ScalarValue, Is.EqualTo(qFloat64.ScalarJ.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarK.ScalarValue, Is.EqualTo(qFloat64.ScalarK.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.Scalar.ScalarValue, Is.EqualTo(qFloat64.Scalar.ScalarValue).Within(Tolerance));
        }
    }
}
