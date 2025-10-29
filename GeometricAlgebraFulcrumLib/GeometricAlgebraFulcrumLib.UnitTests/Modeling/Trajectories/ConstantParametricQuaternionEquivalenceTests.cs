using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Quaternions.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Quaternions.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public sealed class ConstantParametricQuaternionEquivalenceTests
{
    private const double Tolerance = 1e-12;

    private IScalarProcessor<double> ScalarProcessor { get; }
        = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestCreateWithPoint()
    {
        // Float64 version
        var pointFloat64 = LinFloat64Quaternion.Create(1, 2, 3, 4);
        var curveFloat64 = ConstantParametricQuaternion.Create(pointFloat64);

        // Generic version
        var pointGeneric = LinQuaternion<double>.Create(ScalarProcessor, 1, 2, 3, 4);
        var curveGeneric = ConstantParametricQuaternion<double>.Create(ScalarProcessor, pointGeneric);

        // Both should return constant point
        for (var t = -5.0; t <= 5.0; t += 1.0)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var qFloat64 = curveFloat64.GetQuaternion(t);
            var qGeneric = curveGeneric.GetQuaternion(tScalar);

            Assert.That(qGeneric.Scalar.ScalarValue, Is.EqualTo(qFloat64.Scalar.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarI.ScalarValue, Is.EqualTo(qFloat64.ScalarI.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarJ.ScalarValue, Is.EqualTo(qFloat64.ScalarJ.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarK.ScalarValue, Is.EqualTo(qFloat64.ScalarK.ScalarValue).Within(Tolerance));
        }
    }

    [Test]
    public void TestCreateWithPointAndTangent()
    {
        // Float64 version
        var pointFloat64 = LinFloat64Quaternion.Create(1, 2, 3, 4);
        var tangentFloat64 = LinFloat64Quaternion.Create(0.1, 0.2, 0.3, 0.4);
        var curveFloat64 = ConstantParametricQuaternion.Create(pointFloat64, tangentFloat64);

        // Generic version
        var pointGeneric = LinQuaternion<double>.Create(ScalarProcessor, 1, 2, 3, 4);
        var tangentGeneric = LinQuaternion<double>.Create(ScalarProcessor, 0.1, 0.2, 0.3, 0.4);
        var curveGeneric = ConstantParametricQuaternion<double>.Create(ScalarProcessor, pointGeneric, tangentGeneric);

        // Test point
        var qFloat64 = curveFloat64.GetQuaternion(0.5);
        var qGeneric = curveGeneric.GetQuaternion(ScalarProcessor.Scalar(0.5));

        Assert.That(qGeneric.Scalar.ScalarValue, Is.EqualTo(qFloat64.Scalar.ScalarValue).Within(Tolerance));
        Assert.That(qGeneric.ScalarI.ScalarValue, Is.EqualTo(qFloat64.ScalarI.ScalarValue).Within(Tolerance));
        Assert.That(qGeneric.ScalarJ.ScalarValue, Is.EqualTo(qFloat64.ScalarJ.ScalarValue).Within(Tolerance));
        Assert.That(qGeneric.ScalarK.ScalarValue, Is.EqualTo(qFloat64.ScalarK.ScalarValue).Within(Tolerance));

        // Test tangent
        var tFloat64 = curveFloat64.GetDerivative1Quaternion(0.5);
        var tGeneric = curveGeneric.GetDerivative1Quaternion(ScalarProcessor.Scalar(0.5));

        Assert.That(tGeneric.Scalar.ScalarValue, Is.EqualTo(tFloat64.Scalar.ScalarValue).Within(Tolerance));
        Assert.That(tGeneric.ScalarI.ScalarValue, Is.EqualTo(tFloat64.ScalarI.ScalarValue).Within(Tolerance));
        Assert.That(tGeneric.ScalarJ.ScalarValue, Is.EqualTo(tFloat64.ScalarJ.ScalarValue).Within(Tolerance));
        Assert.That(tGeneric.ScalarK.ScalarValue, Is.EqualTo(tFloat64.ScalarK.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void TestDefaultTangentIsIdentity()
    {
        // Float64 version (default tangent is Identity)
        var pointFloat64 = LinFloat64Quaternion.Create(2, 3, 4, 5);
        var curveFloat64 = ConstantParametricQuaternion.Create(pointFloat64);

        // Generic version
        var pointGeneric = LinQuaternion<double>.Create(ScalarProcessor, 2, 3, 4, 5);
        var curveGeneric = ConstantParametricQuaternion<double>.Create(ScalarProcessor, pointGeneric);

        // Derivative should be Identity (1, 0, 0, 0)
        var tFloat64 = curveFloat64.GetDerivative1Quaternion(1.0);
        var tGeneric = curveGeneric.GetDerivative1Quaternion(ScalarProcessor.Scalar(1.0));

        Assert.That(tGeneric.Scalar.ScalarValue, Is.EqualTo(tFloat64.Scalar.ScalarValue).Within(Tolerance));
        Assert.That(tGeneric.ScalarI.ScalarValue, Is.EqualTo(tFloat64.ScalarI.ScalarValue).Within(Tolerance));
        Assert.That(tGeneric.ScalarJ.ScalarValue, Is.EqualTo(tFloat64.ScalarJ.ScalarValue).Within(Tolerance));
        Assert.That(tGeneric.ScalarK.ScalarValue, Is.EqualTo(tFloat64.ScalarK.ScalarValue).Within(Tolerance));

        // Identity quaternion check
        Assert.That(tFloat64.Scalar.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(tFloat64.ScalarI.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(tFloat64.ScalarJ.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(tFloat64.ScalarK.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
    }

    [Test]
    public void TestConstantAcrossParameterValues()
    {
        // Float64 version
        var pointFloat64 = LinFloat64Quaternion.Create(5, 6, 7, 8);
        var curveFloat64 = ConstantParametricQuaternion.Create(pointFloat64);

        // Generic version
        var pointGeneric = LinQuaternion<double>.Create(ScalarProcessor, 5, 6, 7, 8);
        var curveGeneric = ConstantParametricQuaternion<double>.Create(ScalarProcessor, pointGeneric);

        // All parameter values should return same quaternion
        var testValues = new[] { -10.0, -1.0, 0.0, 0.5, 1.0, 10.0, 100.0 };

        foreach (var t in testValues)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var qFloat64 = curveFloat64.GetQuaternion(t);
            var qGeneric = curveGeneric.GetQuaternion(tScalar);

            Assert.That(qGeneric.ScalarI.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
            Assert.That(qGeneric.ScalarJ.ScalarValue, Is.EqualTo(6.0).Within(Tolerance));
            Assert.That(qGeneric.ScalarK.ScalarValue, Is.EqualTo(7.0).Within(Tolerance));
            Assert.That(qGeneric.Scalar.ScalarValue, Is.EqualTo(8.0).Within(Tolerance));

            Assert.That(qFloat64.ScalarI.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
            Assert.That(qFloat64.ScalarJ.ScalarValue, Is.EqualTo(6.0).Within(Tolerance));
            Assert.That(qFloat64.ScalarK.ScalarValue, Is.EqualTo(7.0).Within(Tolerance));
            Assert.That(qFloat64.Scalar.ScalarValue, Is.EqualTo(8.0).Within(Tolerance));
        }
    }

    [Test]
    public void TestParameterRangeIsInfinite()
    {
        // Float64 version
        var pointFloat64 = LinFloat64Quaternion.Create(1, 0, 0, 0);
        var curveFloat64 = ConstantParametricQuaternion.Create(pointFloat64);

        // Generic version
        var pointGeneric = LinQuaternion<double>.Create(ScalarProcessor, 1, 0, 0, 0);
        var curveGeneric = ConstantParametricQuaternion<double>.Create(ScalarProcessor, pointGeneric);

        // Both should have infinite ranges
        Assert.That(curveFloat64.TimeRange.IsInfinite);
        Assert.That(curveGeneric.ParameterRange.IsFinite, Is.False);
    }

    [Test]
    public void TestIsValid()
    {
        // Generic version
        var pointGeneric = LinQuaternion<double>.Create(ScalarProcessor, 1, 2, 3, 4);
        var tangentGeneric = LinQuaternion<double>.Create(ScalarProcessor, 0.1, 0.2, 0.3, 0.4);
        var curveGeneric = ConstantParametricQuaternion<double>.Create(ScalarProcessor, pointGeneric, tangentGeneric);

        Assert.That(curveGeneric.IsValid(), Is.True);
    }

    [Test]
    public void TestProperties()
    {
        // Generic version
        var pointGeneric = LinQuaternion<double>.Create(ScalarProcessor, 7, 8, 9, 10);
        var tangentGeneric = LinQuaternion<double>.Create(ScalarProcessor, 0.7, 0.8, 0.9, 1.0);
        var curveGeneric = ConstantParametricQuaternion<double>.Create(ScalarProcessor, pointGeneric, tangentGeneric);

        // Test Point property (Create parameter order: i, j, k, scalar)
        Assert.That(curveGeneric.Point.ScalarI.ScalarValue, Is.EqualTo(7.0).Within(Tolerance));
        Assert.That(curveGeneric.Point.ScalarJ.ScalarValue, Is.EqualTo(8.0).Within(Tolerance));
        Assert.That(curveGeneric.Point.ScalarK.ScalarValue, Is.EqualTo(9.0).Within(Tolerance));
        Assert.That(curveGeneric.Point.Scalar.ScalarValue, Is.EqualTo(10.0).Within(Tolerance));

        // Test Tangent property (Create parameter order: i, j, k, scalar)
        Assert.That(curveGeneric.Tangent.ScalarI.ScalarValue, Is.EqualTo(0.7).Within(Tolerance));
        Assert.That(curveGeneric.Tangent.ScalarJ.ScalarValue, Is.EqualTo(0.8).Within(Tolerance));
        Assert.That(curveGeneric.Tangent.ScalarK.ScalarValue, Is.EqualTo(0.9).Within(Tolerance));
        Assert.That(curveGeneric.Tangent.Scalar.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
    }

    [Test]
    public void TestIdentityQuaternion()
    {
        // Float64 identity quaternion
        var identityFloat64 = LinFloat64Quaternion.Identity;
        var curveFloat64 = ConstantParametricQuaternion.Create(identityFloat64);

        // Generic identity quaternion
        var identityGeneric = LinQuaternion<double>.Identity(ScalarProcessor);
        var curveGeneric = ConstantParametricQuaternion<double>.Create(ScalarProcessor, identityGeneric);

        var qFloat64 = curveFloat64.GetQuaternion(0.0);
        var qGeneric = curveGeneric.GetQuaternion(ScalarProcessor.Scalar(0.0));

        Assert.That(qGeneric.Scalar.ScalarValue, Is.EqualTo(qFloat64.Scalar.ScalarValue).Within(Tolerance));
        Assert.That(qGeneric.ScalarI.ScalarValue, Is.EqualTo(qFloat64.ScalarI.ScalarValue).Within(Tolerance));
        Assert.That(qGeneric.ScalarJ.ScalarValue, Is.EqualTo(qFloat64.ScalarJ.ScalarValue).Within(Tolerance));
        Assert.That(qGeneric.ScalarK.ScalarValue, Is.EqualTo(qFloat64.ScalarK.ScalarValue).Within(Tolerance));

        // Identity: (1, 0, 0, 0)
        Assert.That(qFloat64.Scalar.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(qFloat64.ScalarI.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(qFloat64.ScalarJ.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(qFloat64.ScalarK.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
    }

    [Test]
    public void TestNegativeParameterValues()
    {
        // Float64 version
        var pointFloat64 = LinFloat64Quaternion.Create(3, 4, 5, 6);
        var curveFloat64 = ConstantParametricQuaternion.Create(pointFloat64);

        // Generic version
        var pointGeneric = LinQuaternion<double>.Create(ScalarProcessor, 3, 4, 5, 6);
        var curveGeneric = ConstantParametricQuaternion<double>.Create(ScalarProcessor, pointGeneric);

        // Test negative parameter values
        for (var t = -100.0; t <= -10.0; t += 10.0)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var qFloat64 = curveFloat64.GetQuaternion(t);
            var qGeneric = curveGeneric.GetQuaternion(tScalar);

            Assert.That(qGeneric.Scalar.ScalarValue, Is.EqualTo(qFloat64.Scalar.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarI.ScalarValue, Is.EqualTo(qFloat64.ScalarI.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarJ.ScalarValue, Is.EqualTo(qFloat64.ScalarJ.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarK.ScalarValue, Is.EqualTo(qFloat64.ScalarK.ScalarValue).Within(Tolerance));
        }
    }

    [Test]
    public void TestLargeParameterValues()
    {
        // Float64 version
        var pointFloat64 = LinFloat64Quaternion.Create(10, 20, 30, 40);
        var curveFloat64 = ConstantParametricQuaternion.Create(pointFloat64);

        // Generic version
        var pointGeneric = LinQuaternion<double>.Create(ScalarProcessor, 10, 20, 30, 40);
        var curveGeneric = ConstantParametricQuaternion<double>.Create(ScalarProcessor, pointGeneric);

        // Test large parameter values
        var testValues = new[] { 1000.0, 10000.0, 1000000.0 };

        foreach (var t in testValues)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var qFloat64 = curveFloat64.GetQuaternion(t);
            var qGeneric = curveGeneric.GetQuaternion(tScalar);

            Assert.That(qGeneric.Scalar.ScalarValue, Is.EqualTo(qFloat64.Scalar.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarI.ScalarValue, Is.EqualTo(qFloat64.ScalarI.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarJ.ScalarValue, Is.EqualTo(qFloat64.ScalarJ.ScalarValue).Within(Tolerance));
            Assert.That(qGeneric.ScalarK.ScalarValue, Is.EqualTo(qFloat64.ScalarK.ScalarValue).Within(Tolerance));
        }
    }
}
