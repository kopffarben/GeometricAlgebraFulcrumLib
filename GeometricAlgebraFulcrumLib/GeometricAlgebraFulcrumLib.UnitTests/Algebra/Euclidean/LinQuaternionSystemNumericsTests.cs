using System;
using System.Numerics;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Tests for generic LinQuaternion<T> against System.Numerics.Quaternion
/// using Float32 scalar processor for compatibility
/// </summary>
[TestFixture]
public class LinQuaternionSystemNumericsTests
{
    private const double Tolerance = 1e-6; // Float32 precision
    private ScalarProcessorOfFloat32 ScalarProcessor { get; set; } = null!;

    [SetUp]
    public void Setup()
    {
        ScalarProcessor = ScalarProcessorOfFloat32.Instance;
    }

    #region Helper Methods

    private LinQuaternion<float> CreateQuaternion(float x, float y, float z, float w)
    {
        return LinQuaternion<float>.Create(ScalarProcessor, x, y, z, w);
    }

    private LinQuaternion<float> FromSystemNumerics(Quaternion q)
    {
        return CreateQuaternion(q.X, q.Y, q.Z, q.W);
    }

    private Quaternion ToSystemNumerics(LinQuaternion<float> q)
    {
        return new Quaternion(
            q.ScalarI.ScalarValue,
            q.ScalarJ.ScalarValue,
            q.ScalarK.ScalarValue,
            q.Scalar.ScalarValue
        );
    }

    #endregion

    [Test]
    public void TestConstructionEquivalence()
    {
        var testCases = new[]
        {
            (x: 0.0f, y: 0.0f, z: 0.0f, w: 1.0f), // Identity
            (x: 1.0f, y: 0.0f, z: 0.0f, w: 0.0f), // Pure i
            (x: 0.0f, y: 1.0f, z: 0.0f, w: 0.0f), // Pure j
            (x: 0.0f, y: 0.0f, z: 1.0f, w: 0.0f), // Pure k
            (x: 1.0f, y: 2.0f, z: 3.0f, w: 4.0f), // General case
        };

        foreach (var (x, y, z, w) in testCases)
        {
            var linQuat = CreateQuaternion(x, y, z, w);
            var sysQuat = new Quaternion(x, y, z, w);

            Assert.That(Math.Abs(linQuat.ScalarI.ScalarValue - sysQuat.X), Is.LessThan(Tolerance),
                $"X component mismatch for ({x},{y},{z},{w})");
            Assert.That(Math.Abs(linQuat.ScalarJ.ScalarValue - sysQuat.Y), Is.LessThan(Tolerance),
                $"Y component mismatch");
            Assert.That(Math.Abs(linQuat.ScalarK.ScalarValue - sysQuat.Z), Is.LessThan(Tolerance),
                $"Z component mismatch");
            Assert.That(Math.Abs(linQuat.Scalar.ScalarValue - sysQuat.W), Is.LessThan(Tolerance),
                $"W component mismatch");
        }
    }

    [Test]
    public void TestConversionRoundTrip()
    {
        var testCases = new[]
        {
            new Quaternion(0f, 0f, 0f, 1f), // Identity
            new Quaternion(1f, 2f, 3f, 4f), // General case
            new Quaternion(-1f, -2f, -3f, -4f) // Negative values
        };

        foreach (var sysQuat in testCases)
        {
            var linQuat = FromSystemNumerics(sysQuat);
            var backToSys = ToSystemNumerics(linQuat);

            // Round-trip conversion should preserve values
            Assert.That(Math.Abs(backToSys.X - sysQuat.X), Is.LessThan(Tolerance),
                "X component mismatch in round-trip");
            Assert.That(Math.Abs(backToSys.Y - sysQuat.Y), Is.LessThan(Tolerance),
                "Y component mismatch in round-trip");
            Assert.That(Math.Abs(backToSys.Z - sysQuat.Z), Is.LessThan(Tolerance),
                "Z component mismatch in round-trip");
            Assert.That(Math.Abs(backToSys.W - sysQuat.W), Is.LessThan(Tolerance),
                "W component mismatch in round-trip");
        }
    }

    [Test]
    public void TestIdentityQuaternion()
    {
        var linIdentity = LinQuaternion<float>.Identity(ScalarProcessor);
        var sysIdentity = Quaternion.Identity;

        Assert.That(Math.Abs(linIdentity.ScalarI.ScalarValue - sysIdentity.X), Is.LessThan(Tolerance),
            "Identity X component mismatch");
        Assert.That(Math.Abs(linIdentity.ScalarJ.ScalarValue - sysIdentity.Y), Is.LessThan(Tolerance),
            "Identity Y component mismatch");
        Assert.That(Math.Abs(linIdentity.ScalarK.ScalarValue - sysIdentity.Z), Is.LessThan(Tolerance),
            "Identity Z component mismatch");
        Assert.That(Math.Abs(linIdentity.Scalar.ScalarValue - sysIdentity.W), Is.LessThan(Tolerance),
            "Identity W component mismatch");
    }

    [Test]
    public void TestAdditionEquivalence()
    {
        var linQuat1 = CreateQuaternion(1f, 2f, 3f, 4f);
        var linQuat2 = CreateQuaternion(5f, 6f, 7f, 8f);
        var sysQuat1 = new Quaternion(1f, 2f, 3f, 4f);
        var sysQuat2 = new Quaternion(5f, 6f, 7f, 8f);

        var linSum = linQuat1 + linQuat2;
        var sysSum = sysQuat1 + sysQuat2;

        Assert.That(Math.Abs(linSum.ScalarI.ScalarValue - sysSum.X), Is.LessThan(Tolerance),
            "X component mismatch in addition");
        Assert.That(Math.Abs(linSum.ScalarJ.ScalarValue - sysSum.Y), Is.LessThan(Tolerance),
            "Y component mismatch in addition");
        Assert.That(Math.Abs(linSum.ScalarK.ScalarValue - sysSum.Z), Is.LessThan(Tolerance),
            "Z component mismatch in addition");
        Assert.That(Math.Abs(linSum.Scalar.ScalarValue - sysSum.W), Is.LessThan(Tolerance),
            "W component mismatch in addition");
    }

    [Test]
    public void TestMultiplicationEquivalence()
    {
        var linQuat1 = CreateQuaternion(1f, 0f, 0f, 0f);
        var linQuat2 = CreateQuaternion(0f, 1f, 0f, 0f);
        var sysQuat1 = new Quaternion(1f, 0f, 0f, 0f);
        var sysQuat2 = new Quaternion(0f, 1f, 0f, 0f);

        var linProduct = linQuat1 * linQuat2;
        var sysProduct = sysQuat1 * sysQuat2;

        Assert.That(Math.Abs(linProduct.ScalarI.ScalarValue - sysProduct.X), Is.LessThan(Tolerance),
            "X component mismatch in multiplication");
        Assert.That(Math.Abs(linProduct.ScalarJ.ScalarValue - sysProduct.Y), Is.LessThan(Tolerance),
            "Y component mismatch in multiplication");
        Assert.That(Math.Abs(linProduct.ScalarK.ScalarValue - sysProduct.Z), Is.LessThan(Tolerance),
            "Z component mismatch in multiplication");
        Assert.That(Math.Abs(linProduct.Scalar.ScalarValue - sysProduct.W), Is.LessThan(Tolerance),
            "W component mismatch in multiplication");
    }

    [Test]
    public void TestConjugateEquivalence()
    {
        var linQuat = CreateQuaternion(1f, 2f, 3f, 4f);
        var sysQuat = new Quaternion(1f, 2f, 3f, 4f);

        var linConj = linQuat.Conjugate();
        var sysConj = Quaternion.Conjugate(sysQuat);

        Assert.That(Math.Abs(linConj.ScalarI.ScalarValue - sysConj.X), Is.LessThan(Tolerance),
            "X component mismatch in conjugate");
        Assert.That(Math.Abs(linConj.ScalarJ.ScalarValue - sysConj.Y), Is.LessThan(Tolerance),
            "Y component mismatch in conjugate");
        Assert.That(Math.Abs(linConj.ScalarK.ScalarValue - sysConj.Z), Is.LessThan(Tolerance),
            "Z component mismatch in conjugate");
        Assert.That(Math.Abs(linConj.Scalar.ScalarValue - sysConj.W), Is.LessThan(Tolerance),
            "W component mismatch in conjugate");
    }

    [Test]
    public void TestNormEquivalence()
    {
        var linQuat = CreateQuaternion(1f, 2f, 3f, 4f);
        var sysQuat = new Quaternion(1f, 2f, 3f, 4f);

        var linNorm = linQuat.Norm().ScalarValue;
        var sysNorm = sysQuat.Length();

        Assert.That(Math.Abs(linNorm - sysNorm), Is.LessThan(Tolerance),
            "Norm mismatch");
    }

    [Test]
    public void TestNormalizeEquivalence()
    {
        var linQuat = CreateQuaternion(1f, 2f, 3f, 4f);
        var sysQuat = new Quaternion(1f, 2f, 3f, 4f);

        var linNorm = linQuat.Normalize();
        var sysNorm = Quaternion.Normalize(sysQuat);

        Assert.That(Math.Abs(linNorm.ScalarI.ScalarValue - sysNorm.X), Is.LessThan(Tolerance),
            "X component mismatch in normalize");
        Assert.That(Math.Abs(linNorm.ScalarJ.ScalarValue - sysNorm.Y), Is.LessThan(Tolerance),
            "Y component mismatch in normalize");
        Assert.That(Math.Abs(linNorm.ScalarK.ScalarValue - sysNorm.Z), Is.LessThan(Tolerance),
            "Z component mismatch in normalize");
        Assert.That(Math.Abs(linNorm.Scalar.ScalarValue - sysNorm.W), Is.LessThan(Tolerance),
            "W component mismatch in normalize");
    }

    [Test]
    public void TestInverseEquivalence()
    {
        var linQuat = CreateQuaternion(1f, 2f, 3f, 4f);
        var sysQuat = new Quaternion(1f, 2f, 3f, 4f);

        var linInv = linQuat.Inverse();
        var sysInv = Quaternion.Inverse(sysQuat);

        Assert.That(Math.Abs(linInv.ScalarI.ScalarValue - sysInv.X), Is.LessThan(Tolerance),
            "X component mismatch in inverse");
        Assert.That(Math.Abs(linInv.ScalarJ.ScalarValue - sysInv.Y), Is.LessThan(Tolerance),
            "Y component mismatch in inverse");
        Assert.That(Math.Abs(linInv.ScalarK.ScalarValue - sysInv.Z), Is.LessThan(Tolerance),
            "Z component mismatch in inverse");
        Assert.That(Math.Abs(linInv.Scalar.ScalarValue - sysInv.W), Is.LessThan(Tolerance),
            "W component mismatch in inverse");
    }

    [Test]
    public void TestDotProductEquivalence()
    {
        var linQuat1 = CreateQuaternion(1f, 2f, 3f, 4f);
        var linQuat2 = CreateQuaternion(5f, 6f, 7f, 8f);
        var sysQuat1 = new Quaternion(1f, 2f, 3f, 4f);
        var sysQuat2 = new Quaternion(5f, 6f, 7f, 8f);

        var linDot = linQuat1.ESp(linQuat2).ScalarValue;
        var sysDot = Quaternion.Dot(sysQuat1, sysQuat2);

        Assert.That(Math.Abs(linDot - sysDot), Is.LessThan(Tolerance),
            "Dot product mismatch");
    }

    [Test]
    public void TestDivisionEquivalence()
    {
        var linQuat1 = CreateQuaternion(1f, 2f, 3f, 4f);
        var linQuat2 = CreateQuaternion(5f, 6f, 7f, 8f);
        var sysQuat1 = new Quaternion(1f, 2f, 3f, 4f);
        var sysQuat2 = new Quaternion(5f, 6f, 7f, 8f);

        var linDiv = linQuat1 / linQuat2;
        var sysDiv = sysQuat1 / sysQuat2;

        Assert.That(Math.Abs(linDiv.ScalarI.ScalarValue - sysDiv.X), Is.LessThan(Tolerance),
            "X component mismatch in division");
        Assert.That(Math.Abs(linDiv.ScalarJ.ScalarValue - sysDiv.Y), Is.LessThan(Tolerance),
            "Y component mismatch in division");
        Assert.That(Math.Abs(linDiv.ScalarK.ScalarValue - sysDiv.Z), Is.LessThan(Tolerance),
            "Z component mismatch in division");
        Assert.That(Math.Abs(linDiv.Scalar.ScalarValue - sysDiv.W), Is.LessThan(Tolerance),
            "W component mismatch in division");
    }

    [Test]
    public void TestConcatenateEquivalence()
    {
        var linQuat1 = CreateQuaternion(1f, 0f, 0f, 0f);
        var linQuat2 = CreateQuaternion(0f, 1f, 0f, 0f);
        var sysQuat1 = new Quaternion(1f, 0f, 0f, 0f);
        var sysQuat2 = new Quaternion(0f, 1f, 0f, 0f);

        // Concatenate is multiplication
        // System.Numerics.Quaternion.Concatenate(q1, q2) == q2 * q1
        var linConcat = linQuat1.Concatenate(linQuat2);
        var sysConcat = Quaternion.Concatenate(sysQuat1, sysQuat2);

        Assert.That(Math.Abs(linConcat.ScalarI.ScalarValue - sysConcat.X), Is.LessThan(Tolerance),
            "X component mismatch in concatenate");
        Assert.That(Math.Abs(linConcat.ScalarJ.ScalarValue - sysConcat.Y), Is.LessThan(Tolerance),
            "Y component mismatch in concatenate");
        Assert.That(Math.Abs(linConcat.ScalarK.ScalarValue - sysConcat.Z), Is.LessThan(Tolerance),
            "Z component mismatch in concatenate");
        Assert.That(Math.Abs(linConcat.Scalar.ScalarValue - sysConcat.W), Is.LessThan(Tolerance),
            "W component mismatch in concatenate");
    }

    [Test]
    public void TestRotateVectorEquivalence()
    {
        // Create a 90° rotation around Z axis
        var axis = new Vector3(0, 0, 1);
        var angle = MathF.PI / 2;
        var sysQuat = Quaternion.CreateFromAxisAngle(axis, angle);
        var linQuat = FromSystemNumerics(sysQuat);

        // Test vector (1, 0, 0) should rotate to approximately (0, 1, 0)
        var testVector = new Vector3(1, 0, 0);
        var linVector = LinVector3D<float>.Create(ScalarProcessor, testVector.X, testVector.Y, testVector.Z);

        // System.Numerics rotation
        var sysRotated = Vector3.Transform(testVector, sysQuat);

        // Our rotation via matrix - System.Numerics uses the conjugate convention
        // so we need to use Conjugate() to match
        var linRotated = linQuat.Conjugate().RotateVector(linVector);

        Assert.That(Math.Abs(linRotated.Scalar1.ScalarValue - sysRotated.X), Is.LessThan(Tolerance),
            "X component mismatch in vector rotation");
        Assert.That(Math.Abs(linRotated.Scalar2.ScalarValue - sysRotated.Y), Is.LessThan(Tolerance),
            "Y component mismatch in vector rotation");
        Assert.That(Math.Abs(linRotated.Scalar3.ScalarValue - sysRotated.Z), Is.LessThan(Tolerance),
            "Z component mismatch in vector rotation");
    }
}
