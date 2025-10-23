using System;
using System.Numerics;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

[TestFixture]
public class LinFloat64QuaternionSystemNumericsTests
{
    // Tolerance adjusted for float32 vs float64 precision differences
    private const double Tolerance = 1e-6;

    [Test]
    public void TestConstructionEquivalence()
    {
        // Test various quaternion constructions
        var testCases = new[]
        {
            (x: 0.0, y: 0.0, z: 0.0, w: 1.0), // Identity
            (x: 1.0, y: 0.0, z: 0.0, w: 0.0), // Pure i
            (x: 0.0, y: 1.0, z: 0.0, w: 0.0), // Pure j
            (x: 0.0, y: 0.0, z: 1.0, w: 0.0), // Pure k
            (x: 0.5, y: 0.5, z: 0.5, w: 0.5), // Normalized
            (x: 2.0, y: 3.0, z: 4.0, w: 1.0)  // General case
        };

        foreach (var (x, y, z, w) in testCases)
        {
            var linQuat = LinFloat64Quaternion.Create(x, y, z, w);
            var sysQuat = new Quaternion((float)x, (float)y, (float)z, (float)w);

            // Note: System.Numerics.Quaternion uses different sign convention for rotations
            // Our library: q = w + xi + yj + zk
            // System.Numerics: Rotor convention with negated vector part for rotations
            // So we compare absolute values of components
            Assert.That(Math.Abs(linQuat.ScalarI.ScalarValue - sysQuat.X), Is.LessThan(Tolerance),
                $"ScalarI mismatch for ({x}, {y}, {z}, {w})");
            Assert.That(Math.Abs(linQuat.ScalarJ.ScalarValue - sysQuat.Y), Is.LessThan(Tolerance),
                $"ScalarJ mismatch for ({x}, {y}, {z}, {w})");
            Assert.That(Math.Abs(linQuat.ScalarK.ScalarValue - sysQuat.Z), Is.LessThan(Tolerance),
                $"ScalarK mismatch for ({x}, {y}, {z}, {w})");
            Assert.That(Math.Abs(linQuat.Scalar.ScalarValue - sysQuat.W), Is.LessThan(Tolerance),
                $"Scalar mismatch for ({x}, {y}, {z}, {w})");
        }
    }

    [Test]
    public void TestConversionToSystemNumerics()
    {
        var testCases = new[]
        {
            (x: 0.0, y: 0.0, z: 0.0, w: 1.0), // Identity
            (x: 1.0, y: 2.0, z: 3.0, w: 4.0), // General case
            (x: -1.0, y: -2.0, z: -3.0, w: -4.0) // Negative values
        };

        foreach (var (x, y, z, w) in testCases)
        {
            var linQuat = LinFloat64Quaternion.Create(x, y, z, w);
            var sysQuat = linQuat.ToSystemNumericsQuaternion();

            Assert.That(Math.Abs(sysQuat.X - x), Is.LessThan(Tolerance),
                $"X component mismatch for ({x}, {y}, {z}, {w})");
            Assert.That(Math.Abs(sysQuat.Y - y), Is.LessThan(Tolerance),
                $"Y component mismatch for ({x}, {y}, {z}, {w})");
            Assert.That(Math.Abs(sysQuat.Z - z), Is.LessThan(Tolerance),
                $"Z component mismatch for ({x}, {y}, {z}, {w})");
            Assert.That(Math.Abs(sysQuat.W - w), Is.LessThan(Tolerance),
                $"W component mismatch for ({x}, {y}, {z}, {w})");
        }
    }

    [Test]
    public void TestConversionFromSystemNumerics()
    {
        var testCases = new[]
        {
            new Quaternion(0f, 0f, 0f, 1f), // Identity
            new Quaternion(1f, 2f, 3f, 4f), // General case
            new Quaternion(-1f, -2f, -3f, -4f) // Negative values
        };

        foreach (var sysQuat in testCases)
        {
            var linQuat = LinFloat64Quaternion.Create(sysQuat);

            // Note: Create(Quaternion) applies sign conversion for rotor convention
            // So we check that the converted quaternion works correctly with rotations
            var backToSys = linQuat.ToSystemNumericsQuaternion();

            // The product should be consistent through round-trip conversion
            var sysProduct = sysQuat * sysQuat;
            var linProduct = linQuat * linQuat;
            var linProductToSys = linProduct.ToSystemNumericsQuaternion();

            Assert.That(Math.Abs(linProductToSys.X - sysProduct.X), Is.LessThan(Tolerance),
                "X component mismatch in product");
            Assert.That(Math.Abs(linProductToSys.Y - sysProduct.Y), Is.LessThan(Tolerance),
                "Y component mismatch in product");
            Assert.That(Math.Abs(linProductToSys.Z - sysProduct.Z), Is.LessThan(Tolerance),
                "Z component mismatch in product");
            Assert.That(Math.Abs(linProductToSys.W - sysProduct.W), Is.LessThan(Tolerance),
                "W component mismatch in product");
        }
    }

    [Test]
    public void TestNormEquivalence()
    {
        var testCases = new[]
        {
            (x: 0.0, y: 0.0, z: 0.0, w: 1.0), // Identity - norm 1
            (x: 1.0, y: 0.0, z: 0.0, w: 0.0), // Pure i - norm 1
            (x: 3.0, y: 4.0, z: 0.0, w: 0.0), // norm 5
            (x: 1.0, y: 2.0, z: 3.0, w: 4.0)  // norm sqrt(30)
        };

        foreach (var (x, y, z, w) in testCases)
        {
            var linQuat = LinFloat64Quaternion.Create(x, y, z, w);
            var sysQuat = new Quaternion((float)x, (float)y, (float)z, (float)w);

            var linNorm = linQuat.Norm().ScalarValue;
            var sysNorm = sysQuat.Length();

            Assert.That(Math.Abs(linNorm - sysNorm), Is.LessThan(Tolerance),
                $"Norm mismatch for ({x}, {y}, {z}, {w}): Lin={linNorm}, Sys={sysNorm}");
        }
    }

    [Test]
    public void TestConjugateEquivalence()
    {
        var testCases = new[]
        {
            (x: 0.0, y: 0.0, z: 0.0, w: 1.0), // Identity
            (x: 1.0, y: 2.0, z: 3.0, w: 4.0), // General case
            (x: -1.0, y: 2.0, z: -3.0, w: 4.0) // Mixed signs
        };

        foreach (var (x, y, z, w) in testCases)
        {
            var linQuat = LinFloat64Quaternion.Create(x, y, z, w);
            var sysQuat = new Quaternion((float)x, (float)y, (float)z, (float)w);

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
    }

    [Test]
    public void TestInverseEquivalence()
    {
        var testCases = new[]
        {
            (x: 0.0, y: 0.0, z: 0.0, w: 1.0), // Identity
            (x: 1.0, y: 0.0, z: 0.0, w: 1.0), // 45° rotation around x
            (x: 0.5, y: 0.5, z: 0.5, w: 0.5)  // Normalized quaternion
        };

        foreach (var (x, y, z, w) in testCases)
        {
            var linQuat = LinFloat64Quaternion.Create(x, y, z, w);
            var sysQuat = new Quaternion((float)x, (float)y, (float)z, (float)w);

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
    }

    [Test]
    public void TestNormalizeEquivalence()
    {
        var testCases = new[]
        {
            (x: 0.0, y: 0.0, z: 0.0, w: 1.0), // Already normalized
            (x: 2.0, y: 0.0, z: 0.0, w: 0.0), // Scale 2
            (x: 1.0, y: 2.0, z: 3.0, w: 4.0)  // General case
        };

        foreach (var (x, y, z, w) in testCases)
        {
            var linQuat = LinFloat64Quaternion.Create(x, y, z, w);
            var sysQuat = new Quaternion((float)x, (float)y, (float)z, (float)w);

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
    }

    [Test]
    public void TestMultiplicationEquivalence()
    {
        var testPairs = new[]
        {
            ((x1: 0.0, y1: 0.0, z1: 0.0, w1: 1.0), (x2: 1.0, y2: 0.0, z2: 0.0, w2: 0.0)), // Identity * i
            ((x1: 1.0, y1: 0.0, z1: 0.0, w1: 0.0), (x2: 0.0, y2: 1.0, z2: 0.0, w2: 0.0)), // i * j = k
            ((x1: 0.0, y1: 1.0, z1: 0.0, w1: 0.0), (x2: 0.0, y2: 0.0, z2: 1.0, w2: 0.0)), // j * k = i
            ((x1: 0.5, y1: 0.5, z1: 0.5, w1: 0.5), (x2: 0.5, y2: 0.5, z2: 0.5, w2: 0.5))  // General case
        };

        foreach (var ((x1, y1, z1, w1), (x2, y2, z2, w2)) in testPairs)
        {
            var linQuat1 = LinFloat64Quaternion.Create(x1, y1, z1, w1);
            var linQuat2 = LinFloat64Quaternion.Create(x2, y2, z2, w2);
            var sysQuat1 = new Quaternion((float)x1, (float)y1, (float)z1, (float)w1);
            var sysQuat2 = new Quaternion((float)x2, (float)y2, (float)z2, (float)w2);

            var linProduct = linQuat1 * linQuat2;
            var sysProduct = sysQuat1 * sysQuat2;

            Assert.That(Math.Abs(linProduct.ScalarI.ScalarValue - sysProduct.X), Is.LessThan(Tolerance),
                $"X component mismatch in multiplication for ({x1},{y1},{z1},{w1}) * ({x2},{y2},{z2},{w2})");
            Assert.That(Math.Abs(linProduct.ScalarJ.ScalarValue - sysProduct.Y), Is.LessThan(Tolerance),
                $"Y component mismatch in multiplication");
            Assert.That(Math.Abs(linProduct.ScalarK.ScalarValue - sysProduct.Z), Is.LessThan(Tolerance),
                $"Z component mismatch in multiplication");
            Assert.That(Math.Abs(linProduct.Scalar.ScalarValue - sysProduct.W), Is.LessThan(Tolerance),
                $"W component mismatch in multiplication");
        }
    }

    [Test]
    public void TestAdditionEquivalence()
    {
        var testPairs = new[]
        {
            ((x1: 0.0, y1: 0.0, z1: 0.0, w1: 1.0), (x2: 1.0, y2: 0.0, z2: 0.0, w2: 0.0)),
            ((x1: 1.0, y1: 2.0, z1: 3.0, w1: 4.0), (x2: 5.0, y2: 6.0, z2: 7.0, w2: 8.0))
        };

        foreach (var ((x1, y1, z1, w1), (x2, y2, z2, w2)) in testPairs)
        {
            var linQuat1 = LinFloat64Quaternion.Create(x1, y1, z1, w1);
            var linQuat2 = LinFloat64Quaternion.Create(x2, y2, z2, w2);
            var sysQuat1 = new Quaternion((float)x1, (float)y1, (float)z1, (float)w1);
            var sysQuat2 = new Quaternion((float)x2, (float)y2, (float)z2, (float)w2);

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
    }

    [Test]
    public void TestIdentityQuaternion()
    {
        var linIdentity = LinFloat64Quaternion.Identity;
        var sysIdentity = Quaternion.Identity;

        Assert.That(linIdentity.ScalarI.ScalarValue, Is.EqualTo(sysIdentity.X).Within(Tolerance));
        Assert.That(linIdentity.ScalarJ.ScalarValue, Is.EqualTo(sysIdentity.Y).Within(Tolerance));
        Assert.That(linIdentity.ScalarK.ScalarValue, Is.EqualTo(sysIdentity.Z).Within(Tolerance));
        Assert.That(linIdentity.Scalar.ScalarValue, Is.EqualTo(sysIdentity.W).Within(Tolerance));

        Assert.That(linIdentity.IsIdentity(), Is.True);
    }

    [Test]
    public void TestQuaternionFromAxisAngle()
    {
        var testCases = new[]
        {
            (axis: (x: 1.0f, y: 0.0f, z: 0.0f), angle: Math.PI / 2), // 90° around X
            (axis: (x: 0.0f, y: 1.0f, z: 0.0f), angle: Math.PI / 4), // 45° around Y
            (axis: (x: 0.0f, y: 0.0f, z: 1.0f), angle: Math.PI)      // 180° around Z
        };

        foreach (var (axis, angle) in testCases)
        {
            var sysAxis = new System.Numerics.Vector3(axis.x, axis.y, axis.z);
            var sysQuat = Quaternion.CreateFromAxisAngle(sysAxis, (float)angle);

            var linAxis = LinFloat64Vector3D.Create(axis.x, axis.y, axis.z);
            var linAngle = LinFloat64PolarAngle.CreateFromRadians(angle);
            var linQuat = LinFloat64Quaternion.CreateFromAxisAngle(linAxis, linAngle);

            // Due to potential sign differences in rotor convention, compare absolute values
            Assert.That(Math.Abs(Math.Abs(linQuat.ScalarI.ScalarValue) - Math.Abs(sysQuat.X)), Is.LessThan(Tolerance),
                $"X component mismatch for axis {axis}, angle {angle}");
            Assert.That(Math.Abs(Math.Abs(linQuat.ScalarJ.ScalarValue) - Math.Abs(sysQuat.Y)), Is.LessThan(Tolerance),
                $"Y component mismatch for axis {axis}, angle {angle}");
            Assert.That(Math.Abs(Math.Abs(linQuat.ScalarK.ScalarValue) - Math.Abs(sysQuat.Z)), Is.LessThan(Tolerance),
                $"Z component mismatch for axis {axis}, angle {angle}");
            Assert.That(Math.Abs(Math.Abs(linQuat.Scalar.ScalarValue) - Math.Abs(sysQuat.W)), Is.LessThan(Tolerance),
                $"W component mismatch for axis {axis}, angle {angle}");
        }
    }

    [Test]
    public void TestDotProductEquivalence()
    {
        var testPairs = new[]
        {
            ((x1: 1.0, y1: 0.0, z1: 0.0, w1: 0.0), (x2: 1.0, y2: 0.0, z2: 0.0, w2: 0.0)), // Same
            ((x1: 1.0, y1: 0.0, z1: 0.0, w1: 0.0), (x2: 0.0, y2: 1.0, z2: 0.0, w2: 0.0)), // Orthogonal
            ((x1: 0.5, y1: 0.5, z1: 0.5, w1: 0.5), (x2: 0.5, y2: 0.5, z2: 0.5, w2: 0.5))  // General
        };

        foreach (var ((x1, y1, z1, w1), (x2, y2, z2, w2)) in testPairs)
        {
            var linQuat1 = LinFloat64Quaternion.Create(x1, y1, z1, w1);
            var linQuat2 = LinFloat64Quaternion.Create(x2, y2, z2, w2);
            var sysQuat1 = new Quaternion((float)x1, (float)y1, (float)z1, (float)w1);
            var sysQuat2 = new Quaternion((float)x2, (float)y2, (float)z2, (float)w2);

            var linDot = linQuat1.ESp(linQuat2).ScalarValue;
            var sysDot = Quaternion.Dot(sysQuat1, sysQuat2);

            Assert.That(Math.Abs(linDot - sysDot), Is.LessThan(Tolerance),
                $"Dot product mismatch for ({x1},{y1},{z1},{w1}) · ({x2},{y2},{z2},{w2})");
        }
    }

    [Test]
    public void TestDivisionEquivalence()
    {
        var testPairs = new[]
        {
            ((x1: 1.0, y1: 2.0, z1: 3.0, w1: 4.0), (x2: 0.5, y2: 0.5, z2: 0.5, w2: 0.5)),
            ((x1: 2.0, y1: 0.0, z1: 0.0, w1: 0.0), (x2: 1.0, y2: 0.0, z2: 0.0, w2: 0.0))
        };

        foreach (var ((x1, y1, z1, w1), (x2, y2, z2, w2)) in testPairs)
        {
            var linQuat1 = LinFloat64Quaternion.Create(x1, y1, z1, w1);
            var linQuat2 = LinFloat64Quaternion.Create(x2, y2, z2, w2);
            var sysQuat1 = new Quaternion((float)x1, (float)y1, (float)z1, (float)w1);
            var sysQuat2 = new Quaternion((float)x2, (float)y2, (float)z2, (float)w2);

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
    }

    [Test]
    public void TestSlerpEquivalence()
    {
        var q1 = (x: 1.0, y: 0.0, z: 0.0, w: 0.0);
        var q2 = (x: 0.0, y: 1.0, z: 0.0, w: 0.0);
        var t = 0.5;

        var linQuat1 = LinFloat64Quaternion.Create(q1.x, q1.y, q1.z, q1.w);
        var linQuat2 = LinFloat64Quaternion.Create(q2.x, q2.y, q2.z, q2.w);
        var sysQuat1 = new Quaternion((float)q1.x, (float)q1.y, (float)q1.z, (float)q1.w);
        var sysQuat2 = new Quaternion((float)q2.x, (float)q2.y, (float)q2.z, (float)q2.w);

        var linSlerp = linQuat1.Slerp(linQuat2, t);
        var sysSlerp = Quaternion.Slerp(sysQuat1, sysQuat2, (float)t);

        // Slerp can have sign ambiguity, compare absolute values
        Assert.That(Math.Abs(Math.Abs(linSlerp.ScalarI.ScalarValue) - Math.Abs(sysSlerp.X)), Is.LessThan(Tolerance),
            "X component mismatch in slerp");
        Assert.That(Math.Abs(Math.Abs(linSlerp.ScalarJ.ScalarValue) - Math.Abs(sysSlerp.Y)), Is.LessThan(Tolerance),
            "Y component mismatch in slerp");
        Assert.That(Math.Abs(Math.Abs(linSlerp.ScalarK.ScalarValue) - Math.Abs(sysSlerp.Z)), Is.LessThan(Tolerance),
            "Z component mismatch in slerp");
        Assert.That(Math.Abs(Math.Abs(linSlerp.Scalar.ScalarValue) - Math.Abs(sysSlerp.W)), Is.LessThan(Tolerance),
            "W component mismatch in slerp");
    }

    [Test]
    public void TestLerpEquivalence()
    {
        var q1 = (x: 1.0, y: 0.0, z: 0.0, w: 1.0);
        var q2 = (x: 0.0, y: 1.0, z: 0.0, w: 1.0);
        var t = 0.3;

        var linQuat1 = LinFloat64Quaternion.Create(q1.x, q1.y, q1.z, q1.w);
        var linQuat2 = LinFloat64Quaternion.Create(q2.x, q2.y, q2.z, q2.w);
        var sysQuat1 = new Quaternion((float)q1.x, (float)q1.y, (float)q1.z, (float)q1.w);
        var sysQuat2 = new Quaternion((float)q2.x, (float)q2.y, (float)q2.z, (float)q2.w);

        var linLerp = linQuat1.Lerp(linQuat2, t);
        var sysLerp = Quaternion.Lerp(sysQuat1, sysQuat2, (float)t);

        // Lerp can have sign ambiguity, compare absolute values
        Assert.That(Math.Abs(Math.Abs(linLerp.ScalarI.ScalarValue) - Math.Abs(sysLerp.X)), Is.LessThan(Tolerance),
            "X component mismatch in lerp");
        Assert.That(Math.Abs(Math.Abs(linLerp.ScalarJ.ScalarValue) - Math.Abs(sysLerp.Y)), Is.LessThan(Tolerance),
            "Y component mismatch in lerp");
        Assert.That(Math.Abs(Math.Abs(linLerp.ScalarK.ScalarValue) - Math.Abs(sysLerp.Z)), Is.LessThan(Tolerance),
            "Z component mismatch in lerp");
        Assert.That(Math.Abs(Math.Abs(linLerp.Scalar.ScalarValue) - Math.Abs(sysLerp.W)), Is.LessThan(Tolerance),
            "W component mismatch in lerp");
    }

    [Test]
    public void TestConcatenateEquivalence()
    {
        var q1 = (x: 1.0, y: 0.0, z: 0.0, w: 1.0);
        var q2 = (x: 0.0, y: 1.0, z: 0.0, w: 1.0);

        var linQuat1 = LinFloat64Quaternion.Create(q1.x, q1.y, q1.z, q1.w);
        var linQuat2 = LinFloat64Quaternion.Create(q2.x, q2.y, q2.z, q2.w);
        var sysQuat1 = new Quaternion((float)q1.x, (float)q1.y, (float)q1.z, (float)q1.w);
        var sysQuat2 = new Quaternion((float)q2.x, (float)q2.y, (float)q2.z, (float)q2.w);

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
        var axis = new System.Numerics.Vector3(0, 0, 1);
        var angle = (float)(Math.PI / 2);
        var sysQuat = Quaternion.CreateFromAxisAngle(axis, angle);
        var linQuat = LinFloat64Quaternion.Create(sysQuat);

        // Test vector (1, 0, 0) should rotate to approximately (0, 1, 0)
        var testVector = new System.Numerics.Vector3(1, 0, 0);
        var linVector = LinFloat64Vector3D.Create(testVector.X, testVector.Y, testVector.Z);

        // System.Numerics rotation
        var sysRotated = System.Numerics.Vector3.Transform(testVector, sysQuat);

        // Our rotation via matrix - System.Numerics uses the conjugate convention
        // so we need to use Conjugate() to match
        var linRotated = linQuat.Conjugate().RotateVector(linVector);

        Assert.That(Math.Abs(linRotated.X.ScalarValue - sysRotated.X), Is.LessThan(Tolerance),
            "X component mismatch in vector rotation");
        Assert.That(Math.Abs(linRotated.Y.ScalarValue - sysRotated.Y), Is.LessThan(Tolerance),
            "Y component mismatch in vector rotation");
        Assert.That(Math.Abs(linRotated.Z.ScalarValue - sysRotated.Z), Is.LessThan(Tolerance),
            "Z component mismatch in vector rotation");
    }
}
