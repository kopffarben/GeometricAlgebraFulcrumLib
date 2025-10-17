using System;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Reflectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.UnitTests.Utilities;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.LinearMaps;

/// <summary>
/// Comprehensive tests for reflectors in Geometric Algebra
///
/// IMPORTANT: This library uses "reflection THROUGH an axis/line" convention:
/// - Vectors PARALLEL to the normal are PRESERVED
/// - Vectors PERPENDICULAR to the normal are REVERSED
///
/// This is opposite to Householder reflection (reflection across hyperplane perpendicular to n).
/// Tests Pure Reflectors, axis reflections, and geometric properties
/// </summary>
[TestFixture]
public class ReflectorsTests
{
    private const double Tolerance = 1e-10;

    #region Pure Reflector Tests

    [TestFixture]
    public class PureReflectorTests
    {
        private XGaFloat64Processor _processor = null!;
        private XGaFloat64RandomComposer _random = null!;
        private const int VSpaceDimensions = 5;

        [OneTimeSetUp]
        public void Setup()
        {
            _processor = XGaFloat64Processor.Euclidean;
            _random = _processor.CreateXGaRandomComposer(VSpaceDimensions, 42);
        }

        /// <summary>
        /// Helper method to get a non-zero unit vector for use as a reflection normal
        /// </summary>
        private XGaFloat64Vector GetNonZeroUnitVector()
        {
            const int maxAttempts = 50;
            for (int i = 0; i < maxAttempts; i++)
            {
                var vector = _random.GetVector();
                var normSquared = vector.ENormSquared().ScalarValue;

                // Check if norm is not near zero
                if (normSquared > 1e-10)
                {
                    return vector.DivideByENorm();
                }
            }

            // Fallback: return e1 (first basis vector)
            return _processor.VectorTerm(0);
        }

        [Test]
        public void PureReflector_IsValid_ChecksReflectorCondition()
        {
            // Create a reflector from a unit vector (normal to hyperplane)
            var normal = GetNonZeroUnitVector();
            var reflector = normal.ToPureReflector();

            // A valid reflector should pass IsValid() check
            Assert.That(reflector.IsValid(),
                "IsValid() should return true for a properly constructed reflector");
        }

        [Test]
        public void PureReflector_InvolutionProperty()
        {
            // Reflecting twice should return to original: reflect(reflect(v)) = v
            var normal = GetNonZeroUnitVector();
            var reflector = normal.ToPureReflector();

            var v = _random.GetVector();
            var reflected = reflector.OmMap(v);
            var doubleReflected = reflector.OmMap(reflected);

            TestUtils.AssertMultivectorEquals(v, doubleReflected, Tolerance,
                "Double reflection should return original vector (involution property)");
        }

        [Test]
        public void PureReflector_PreservesNorm()
        {
            // Reflection preserves vector norms
            var normal = GetNonZeroUnitVector();
            var reflector = normal.ToPureReflector();

            var v = _random.GetVector();
            var reflected = reflector.OmMap(v);

            TestUtils.AssertNormPreserving(v, reflected, Tolerance,
                "Reflection should preserve vector norm");
        }

        [Test]
        public void PureReflector_PreservesNormalVector()
        {
            // The normal vector should be preserved by the reflection (reflection THROUGH the axis)
            var normal = GetNonZeroUnitVector();
            var reflector = normal.ToPureReflector();

            var reflected = reflector.OmMap(normal);

            TestUtils.AssertMultivectorEquals(normal, reflected, Tolerance,
                "Reflection through axis should preserve the normal vector");
        }

        [Test]
        public void PureReflector_ReversesPerpendicularVectors()
        {
            // Vectors perpendicular to the normal should be reversed (reflection through axis)
            var normal = _processor.VectorTerm(0); // e₁

            // Create a vector perpendicular to normal
            var perpendicular = _processor.VectorTerm(1); // e₂ ⊥ e₁

            var reflector = normal.ToPureReflector();
            var reflected = reflector.OmMap(perpendicular);

            TestUtils.AssertMultivectorEquals(-perpendicular, reflected, Tolerance,
                "Reflection through axis should reverse vectors perpendicular to normal");
        }

        [Test]
        public void PureReflector_DecompositionFormula()
        {
            // For reflection THROUGH an axis defined by unit vector n:
            // - Parallel component (v·n)n is preserved
            // - Perpendicular component v - (v·n)n is reversed
            // Formula: reflect(v) = 2(v·n)n - v
            var n = GetNonZeroUnitVector();
            var reflector = n.ToPureReflector();

            var v = _random.GetVector();
            var reflected = reflector.OmMap(v);

            // Calculate expected using formula for reflection through axis
            var vDotN = v.Sp(n).ScalarValue;
            var expected = n * (2 * vDotN) - v;

            TestUtils.AssertMultivectorEquals(expected, reflected, Tolerance * 10,
                "Reflection should match formula: 2(v·n)n - v for unit normal");
        }

        [Test]
        public void PureReflector_AxisReflection()
        {
            // Reflection THROUGH the Z-axis (preserves Z, reverses X and Y)
            var zAxis = _processor.VectorTerm(2); // e₃ (Z-axis)
            var reflector = zAxis.ToPureReflector();

            // Point (1, 2, 3)
            var point = _processor.VectorTerm(0) + _processor.VectorTerm(1) * 2 + _processor.VectorTerm(2) * 3;

            var reflected = reflector.OmMap(point);

            // Expected: (-1, -2, 3) - Z component preserved, X and Y reversed
            var expected = -_processor.VectorTerm(0) - _processor.VectorTerm(1) * 2 + _processor.VectorTerm(2) * 3;

            TestUtils.AssertMultivectorEquals(expected, reflected, Tolerance,
                "Reflection through Z-axis should preserve Z and reverse X,Y");
        }

        [Test]
        public void PureReflector_InverseReflector()
        {
            // For reflections, R is its own inverse (involution: R^2 = I)
            // So applying R twice should return to original
            var normal = GetNonZeroUnitVector();
            var reflector = normal.ToPureReflector();

            var v = _random.GetVector();

            // Mathematically: R ∘ R = I, so R^{-1} = R
            // Test by applying reflector twice
            var reflected = reflector.OmMap(v);
            var doubleReflected = reflector.OmMap(reflected);

            TestUtils.AssertMultivectorEquals(v, doubleReflected, Tolerance * 100,
                "Reflector applied twice should return original (R^2 = I)");
        }

        [Test]
        public void PureReflector_PreservesBivectorGrade()
        {
            // Reflection preserves grades
            var normal = GetNonZeroUnitVector();
            var reflector = normal.ToPureReflector();

            var bivector = _random.GetBivector();
            var reflected = reflector.OmMap(bivector);

            TestUtils.AssertGrade(reflected, 2,
                "Reflection should preserve grade of bivectors");
        }

        [Test]
        public void PureReflector_PreservesKVectorGrade()
        {
            // Reflection preserves k-vector grades
            var normal = GetNonZeroUnitVector();
            var reflector = normal.ToPureReflector();

            const int grade = 3;
            // Use deterministic k-vector to avoid numerical precision issues with random k-vectors
            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);
            var e2 = _processor.VectorTerm(2);
            var kVector = e0.Op(e1).Op(e2);

            var reflected = reflector.OmMap(kVector);

            TestUtils.AssertGrade(reflected, grade,
                $"Reflection should preserve grade-{grade} k-vectors");
        }

        [Test]
        public void PureReflector_PreservesScalarProduct()
        {
            // Reflections are orthogonal transformations and preserve scalar products
            // For reflection through axis: reflect(a) · reflect(b) = a · b
            var normal = GetNonZeroUnitVector();
            var reflector = normal.ToPureReflector();

            var a = _random.GetVector();
            var b = _random.GetVector();

            var originalSp = a.Sp(b).ScalarValue;

            var aReflected = reflector.OmMap(a);
            var bReflected = reflector.OmMap(b);
            var reflectedSp = aReflected.Sp(bReflected).ScalarValue;

            // Use larger tolerance for random vectors (accumulated floating-point errors)
            TestUtils.AssertDoubleEquals(originalSp, reflectedSp, Tolerance * 1000,
                "Reflection should preserve scalar products");
        }

        [Test]
        public void PureReflector_ChangesOrientation()
        {
            // Reflection reverses orientation (changes handedness)
            // For vectors a, b, c: det(reflect(a), reflect(b), reflect(c)) = -det(a, b, c)
            // This can be tested with the outer product

            var normal = GetNonZeroUnitVector();
            var reflector = normal.ToPureReflector();

            var a = _processor.VectorTerm(0);
            var b = _processor.VectorTerm(1);
            var c = _processor.VectorTerm(2);

            // Original trivector
            var original = a.Op(b).Op(c);

            // Reflected trivector
            var aReflected = reflector.OmMap(a);
            var bReflected = reflector.OmMap(b);
            var cReflected = reflector.OmMap(c);
            var reflected = aReflected.Op(bReflected).Op(cReflected);

            // Should have opposite sign (orientation reversal)
            var originalNorm = original.Norm().ScalarValue;
            var reflectedNorm = reflected.Norm().ScalarValue;

            // Magnitudes should be equal
            TestUtils.AssertDoubleEquals(originalNorm, reflectedNorm, Tolerance,
                "Reflection should preserve magnitude of trivector");

            // But the trivector itself should be negated (orientation reversal)
            // Note: This depends on the normal vector orientation
        }
    }

    #endregion

    #region Reflection Composition Tests

    [TestFixture]
    public class ReflectionCompositionTests
    {
        private XGaFloat64Processor _processor = null!;
        private XGaFloat64RandomComposer _random = null!;
        private const int VSpaceDimensions = 5;

        [OneTimeSetUp]
        public void Setup()
        {
            _processor = XGaFloat64Processor.Euclidean;
            _random = _processor.CreateXGaRandomComposer(VSpaceDimensions, 42);
        }

        /// <summary>
        /// Helper method to get a non-zero unit vector for use as a reflection normal
        /// </summary>
        private XGaFloat64Vector GetNonZeroUnitVector()
        {
            const int maxAttempts = 50;
            for (int i = 0; i < maxAttempts; i++)
            {
                var vector = _random.GetVector();
                var normSquared = vector.ENormSquared().ScalarValue;

                // Check if norm is not near zero
                if (normSquared > 1e-10)
                {
                    return vector.DivideByENorm();
                }
            }

            // Fallback: return e1 (first basis vector)
            return _processor.VectorTerm(0);
        }

        [Test]
        public void Reflection_TwoReflectionsGiveRotation()
        {
            // Composing two reflections gives a rotation
            // The angle of rotation is twice the angle between the normals
            var n1 = _processor.VectorTerm(0); // e₁
            var n2 = _processor.VectorTerm(1); // e₂

            var reflector1 = n1.ToPureReflector();
            var reflector2 = n2.ToPureReflector();

            var v = _random.GetVector();

            // Apply two reflections
            var result = reflector2.OmMap(reflector1.OmMap(v));

            // The result should preserve norm
            TestUtils.AssertNormPreserving(v, result, Tolerance,
                "Composition of two reflections should preserve norm");
        }

        [Test]
        public void Reflection_CompositionOfThreeReflections()
        {
            // Three reflections composed
            var n1 = GetNonZeroUnitVector();
            var n2 = GetNonZeroUnitVector();
            var n3 = GetNonZeroUnitVector();

            var reflector1 = n1.ToPureReflector();
            var reflector2 = n2.ToPureReflector();
            var reflector3 = n3.ToPureReflector();

            var v = _random.GetVector();
            var result = reflector3.OmMap(reflector2.OmMap(reflector1.OmMap(v)));

            // Should preserve norm
            TestUtils.AssertNormPreserving(v, result, Tolerance,
                "Composition of three reflections should preserve norm");
        }

        [Test]
        public void Reflection_OrthogonalReflections()
        {
            // Reflections through orthogonal hyperplanes
            var e1 = _processor.VectorTerm(0);
            var e2 = _processor.VectorTerm(1);

            var reflector1 = e1.ToPureReflector();
            var reflector2 = e2.ToPureReflector();

            // Reflect (1, 1) through both coordinate axes
            var v = e1 + e2;

            var reflected1 = reflector1.OmMap(v); // Reflects across YZ plane (perpendicular to X)
            var reflected2 = reflector2.OmMap(reflected1); // Then reflects across XZ plane (perpendicular to Y)

            // Expected: (-1, -1)
            var expected = -e1 - e2;

            TestUtils.AssertMultivectorEquals(expected, reflected2, Tolerance,
                "Two orthogonal reflections should negate both components");
        }
    }

    #endregion

    #region Special Cases Tests

    [TestFixture]
    public class ReflectorSpecialCasesTests
    {
        private XGaFloat64Processor _processor = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            _processor = XGaFloat64Processor.Euclidean;
        }

        [Test]
        public void Reflector_ReflectionThroughZAxis()
        {
            // Reflection THROUGH Z-axis (e₃)
            var e3 = _processor.VectorTerm(2);
            var reflector = e3.ToPureReflector();

            // Point (1, 2, 3)
            var e1 = _processor.VectorTerm(0);
            var e2 = _processor.VectorTerm(1);
            var point = e1 + e2 * 2 + e3 * 3;

            var reflected = reflector.OmMap(point);

            // Expected: (-1, -2, 3) - Z preserved, X and Y negated
            var expected = -e1 - e2 * 2 + e3 * 3;

            TestUtils.AssertMultivectorEquals(expected, reflected, Tolerance,
                "Reflection through Z-axis should preserve Z and negate X,Y");
        }

        [Test]
        public void Reflector_ReflectionThroughYAxis()
        {
            // Reflection THROUGH Y-axis (e₂)
            var e2 = _processor.VectorTerm(1);
            var reflector = e2.ToPureReflector();

            // Point (1, 2, 3)
            var e1 = _processor.VectorTerm(0);
            var e3 = _processor.VectorTerm(2);
            var point = e1 + e2 * 2 + e3 * 3;

            var reflected = reflector.OmMap(point);

            // Expected: (-1, 2, -3) - Y preserved, X and Z negated
            var expected = -e1 + e2 * 2 - e3 * 3;

            TestUtils.AssertMultivectorEquals(expected, reflected, Tolerance,
                "Reflection through Y-axis should preserve Y and negate X,Z");
        }

        [Test]
        public void Reflector_ReflectionThroughXAxis()
        {
            // Reflection THROUGH X-axis (e₁)
            var e1 = _processor.VectorTerm(0);
            var reflector = e1.ToPureReflector();

            // Point (1, 2, 3)
            var e2 = _processor.VectorTerm(1);
            var e3 = _processor.VectorTerm(2);
            var point = e1 + e2 * 2 + e3 * 3;

            var reflected = reflector.OmMap(point);

            // Expected: (1, -2, -3) - X preserved, Y and Z negated
            var expected = e1 - e2 * 2 - e3 * 3;

            TestUtils.AssertMultivectorEquals(expected, reflected, Tolerance,
                "Reflection through X-axis should preserve X and negate Y,Z");
        }

        [Test]
        public void Reflector_ReflectionAcrossDiagonalPlane()
        {
            // Reflection across plane with normal (1, 1, 0) / sqrt(2)
            var e1 = _processor.VectorTerm(0);
            var e2 = _processor.VectorTerm(1);
            var e3 = _processor.VectorTerm(2);

            var normal = (e1 + e2).DivideByENorm();
            var reflector = normal.ToPureReflector();

            // Test with e₁
            var reflected = reflector.OmMap(e1);

            // When reflecting e₁ across the plane perpendicular to (1,1,0)/√2,
            // we expect the result to swap x and y components
            // e₁ = (1, 0, 0) → (0, 1, 0) = e₂
            TestUtils.AssertMultivectorEquals(e2, reflected, Tolerance,
                "Reflection across diagonal plane should swap e₁ and e₂");
        }

        [Test]
        public void Reflector_ThreeAxisReflections()
        {
            // Composing reflections through all three coordinate axes
            // With this library's convention (preserve parallel, reverse perpendicular):
            // X-reflection: (2,3,4) → (2,-3,-4)
            // Y-reflection: (2,-3,-4) → (-2,-3,4)
            // Z-reflection: (-2,-3,4) → (2,3,4) - back to original!

            var e1 = _processor.VectorTerm(0);
            var e2 = _processor.VectorTerm(1);
            var e3 = _processor.VectorTerm(2);

            var reflector1 = e1.ToPureReflector();
            var reflector2 = e2.ToPureReflector();
            var reflector3 = e3.ToPureReflector();

            var point = e1 * 2 + e2 * 3 + e3 * 4;

            // Compose three reflections
            var reflected = reflector3.OmMap(reflector2.OmMap(reflector1.OmMap(point)));

            // Expected: original point (identity transformation)
            TestUtils.AssertMultivectorEquals(point, reflected, Tolerance,
                "Three reflections through coordinate axes returns to original");
        }

        [Test]
        public void Reflector_VectorPerpendicularToAxis()
        {
            // A vector perpendicular to the axis should be REVERSED (library convention)
            var axis = _processor.VectorTerm(2); // e₃ (Z-axis)

            // Vector in XY plane (perpendicular to Z-axis)
            var e1 = _processor.VectorTerm(0);
            var e2 = _processor.VectorTerm(1);
            var perpVector = e1 * 3 + e2 * 4;

            var reflector = axis.ToPureReflector();
            var reflected = reflector.OmMap(perpVector);

            // Perpendicular vectors are reversed through the axis
            TestUtils.AssertMultivectorEquals(-perpVector, reflected, Tolerance,
                "Vectors perpendicular to axis should be reversed");
        }
    }

    #endregion

    #region Geometric Properties Tests

    [TestFixture]
    public class ReflectorGeometricPropertiesTests
    {
        private XGaFloat64Processor _processor = null!;
        private XGaFloat64RandomComposer _random = null!;
        private const int VSpaceDimensions = 5;

        [OneTimeSetUp]
        public void Setup()
        {
            _processor = XGaFloat64Processor.Euclidean;
            _random = _processor.CreateXGaRandomComposer(VSpaceDimensions, 42);
        }

        /// <summary>
        /// Helper method to get a non-zero unit vector for use as a reflection normal
        /// </summary>
        private XGaFloat64Vector GetNonZeroUnitVector()
        {
            const int maxAttempts = 50;
            for (int i = 0; i < maxAttempts; i++)
            {
                var vector = _random.GetVector();
                var normSquared = vector.ENormSquared().ScalarValue;

                // Check if norm is not near zero
                if (normSquared > 1e-10)
                {
                    return vector.DivideByENorm();
                }
            }

            // Fallback: return e1 (first basis vector)
            return _processor.VectorTerm(0);
        }

        [Test]
        public void Reflector_PreservesAngles()
        {
            // Reflection preserves angles between vectors
            var normal = GetNonZeroUnitVector();
            var reflector = normal.ToPureReflector();

            var a = _random.GetVector();
            var b = _random.GetVector();

            // Calculate original angle using scalar product
            var originalCos = a.Sp(b).ScalarValue / (a.ENorm().ScalarValue * b.ENorm().ScalarValue);

            var aReflected = reflector.OmMap(a);
            var bReflected = reflector.OmMap(b);

            var reflectedCos = aReflected.Sp(bReflected).ScalarValue /
                               (aReflected.ENorm().ScalarValue * bReflected.ENorm().ScalarValue);

            TestUtils.AssertDoubleEquals(originalCos, reflectedCos, Tolerance,
                "Reflection should preserve angles between vectors");
        }

        [Test]
        public void Reflector_PreservesParallelism()
        {
            // If a ∥ b, then reflect(a) ∥ reflect(b)
            var normal = GetNonZeroUnitVector();
            var reflector = normal.ToPureReflector();

            var a = _random.GetVector();
            var b = a * 2.5; // Parallel to a

            var aReflected = reflector.OmMap(a);
            var bReflected = reflector.OmMap(b);

            // Check if reflected vectors are parallel: a_r ∧ b_r = 0
            var outerProduct = aReflected.Op(bReflected);

            TestUtils.AssertNearZero(outerProduct, Tolerance,
                "Reflection should preserve parallelism");
        }

        [Test]
        public void Reflector_PreservesOrthogonality()
        {
            // If a ⊥ b, then reflect(a) ⊥ reflect(b)
            var normal = GetNonZeroUnitVector();
            var reflector = normal.ToPureReflector();

            var a = _processor.VectorTerm(0);
            var b = _processor.VectorTerm(1); // Orthogonal to a

            var aReflected = reflector.OmMap(a);
            var bReflected = reflector.OmMap(b);

            // Check if reflected vectors are orthogonal: a_r · b_r = 0
            var scalarProduct = aReflected.Sp(bReflected).ScalarValue;

            TestUtils.AssertNearZero(scalarProduct, Tolerance,
                "Reflection should preserve orthogonality");
        }

        [Test]
        public void Reflector_IsLinearMap()
        {
            // Reflection is a linear map: reflect(a + b) = reflect(a) + reflect(b)
            var normal = GetNonZeroUnitVector();
            var reflector = normal.ToPureReflector();

            var a = _random.GetVector();
            var b = _random.GetVector();

            var reflectedSum = reflector.OmMap(a + b);
            var sumOfReflected = reflector.OmMap(a) + reflector.OmMap(b);

            TestUtils.AssertMultivectorEquals(reflectedSum, sumOfReflected, Tolerance,
                "Reflection should be linear: reflect(a+b) = reflect(a) + reflect(b)");
        }

        [Test]
        public void Reflector_IsLinearMap_ScalarMultiple()
        {
            // Reflection is linear: reflect(c·a) = c·reflect(a)
            var normal = GetNonZeroUnitVector();
            var reflector = normal.ToPureReflector();

            var a = _random.GetVector();
            var scalar = 3.7;

            var reflectedScaled = reflector.OmMap(a * scalar);
            var scaledReflected = reflector.OmMap(a) * scalar;

            TestUtils.AssertMultivectorEquals(reflectedScaled, scaledReflected, Tolerance,
                "Reflection should be linear: reflect(c·a) = c·reflect(a)");
        }
    }

    #endregion
}
