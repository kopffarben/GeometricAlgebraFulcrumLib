using System.Collections.Generic;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Outermorphisms;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.SpaceND;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.LinearMaps.SpaceND;
using GeometricAlgebraFulcrumLib.UnitTests.Utilities;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.LinearMaps;

/// <summary>
/// Comprehensive tests for outermorphisms in Geometric Algebra
/// An outermorphism is a linear map that preserves the outer product
/// Tests identity, diagonal, and general outermorphisms
/// </summary>
[TestFixture]
public class OutermorphismsTests
{
    private const double Tolerance = 1e-10;

    #region Identity Outermorphism Tests

    [TestFixture]
    public class IdentityOutermorphismTests
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

        [Test]
        public void IdentityOutermorphism_PreservesVectors()
        {
            // Identity outermorphism should not change vectors
            var identity = new XGaFloat64IdentityOutermorphism(_processor);
            var v = _random.GetVector();

            var mapped = identity.OmMap(v);

            TestUtils.AssertMultivectorEquals(v, mapped, Tolerance,
                "Identity outermorphism should preserve vectors");
        }

        [Test]
        public void IdentityOutermorphism_PreservesBivectors()
        {
            // Identity outermorphism should not change bivectors
            var identity = new XGaFloat64IdentityOutermorphism(_processor);
            var bivector = _random.GetBivector();

            var mapped = identity.OmMap(bivector);

            TestUtils.AssertMultivectorEquals(bivector, mapped, Tolerance,
                "Identity outermorphism should preserve bivectors");
        }

        [Test]
        public void IdentityOutermorphism_PreservesKVectors()
        {
            // Identity outermorphism should not change k-vectors
            var identity = new XGaFloat64IdentityOutermorphism(_processor);
            const int grade = 3;
            var kVector = _random.GetKVector(grade);

            var mapped = identity.OmMap(kVector);

            TestUtils.AssertMultivectorEquals(kVector, mapped, Tolerance,
                $"Identity outermorphism should preserve grade-{grade} k-vectors");
        }

        [Test]
        public void IdentityOutermorphism_PreservesMultivectors()
        {
            // Identity outermorphism should not change multivectors
            var identity = new XGaFloat64IdentityOutermorphism(_processor);
            var multivector = _random.GetMultivector();

            var mapped = identity.OmMap(multivector);

            TestUtils.AssertMultivectorEquals(multivector, mapped, Tolerance,
                "Identity outermorphism should preserve multivectors");
        }
    }

    #endregion

    #region Diagonal Outermorphism Tests

    [TestFixture]
    public class DiagonalOutermorphismTests
    {
        private XGaFloat64Processor _processor = null!;
        private const int VSpaceDimensions = 5;

        [OneTimeSetUp]
        public void Setup()
        {
            _processor = XGaFloat64Processor.Euclidean;
        }

        private XGaFloat64DiagonalOutermorphism CreateDiagonalOutermorphism(double[] scalars)
        {
            // Create a diagonal outermorphism using the correct API
            // Build a vector with the diagonal scalars
            var diagonalVector = _processor.VectorZero;
            for (int i = 0; i < scalars.Length && i < VSpaceDimensions; i++)
            {
                diagonalVector += _processor.VectorTerm(i, scalars[i]);
            }
            return diagonalVector.ToDiagonalAutomorphism();
        }

        [Test]
        public void DiagonalOutermorphism_ScalesBasisVectors()
        {
            // Diagonal outermorphism scales each basis vector independently
            var scalars = new[] { 2.0, 3.0, 4.0, 5.0, 6.0 };
            var diagonalMap = CreateDiagonalOutermorphism(scalars);

            // Test each basis vector
            for (int i = 0; i < VSpaceDimensions; i++)
            {
                var basisVector = _processor.VectorTerm(i);
                var mapped = diagonalMap.OmMap(basisVector);

                var expected = basisVector * scalars[i];

                TestUtils.AssertMultivectorEquals(expected, mapped, Tolerance,
                    $"Diagonal outermorphism should scale basis vector e_{i} by {scalars[i]}");
            }
        }

        [Test]
        public void DiagonalOutermorphism_IsLinear()
        {
            // Outermorphism is linear: f(a + b) = f(a) + f(b)
            var scalars = new[] { 2.0, 3.0, 4.0, 5.0, 6.0 };
            var diagonalMap = CreateDiagonalOutermorphism(scalars);

            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);

            var a = e0 * 2 + e1 * 3;
            var b = e0 * 5 + e1 * 7;

            var mappedSum = diagonalMap.OmMap(a + b);
            var sumOfMapped = diagonalMap.OmMap(a) + diagonalMap.OmMap(b);

            TestUtils.AssertMultivectorEquals(mappedSum, sumOfMapped, Tolerance,
                "Outermorphism should be linear: f(a+b) = f(a) + f(b)");
        }

        [Test]
        public void DiagonalOutermorphism_ScalesOuterProduct()
        {
            // For diagonal outermorphism: f(a ∧ b) = f(a) ∧ f(b)
            var scalars = new[] { 2.0, 3.0, 4.0, 5.0, 6.0 };
            var diagonalMap = CreateDiagonalOutermorphism(scalars);

            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);

            var a = e0;
            var b = e1;

            // Map the outer product
            var outerProductMapped = diagonalMap.OmMap(a.Op(b).GetBivectorPart());

            // Outer product of mapped vectors
            var aMapped = diagonalMap.OmMap(a);
            var bMapped = diagonalMap.OmMap(b);
            var mappedOuterProduct = aMapped.Op(bMapped);

            TestUtils.AssertMultivectorEquals(outerProductMapped, mappedOuterProduct, Tolerance,
                "Outermorphism should preserve outer product: f(a∧b) = f(a)∧f(b)");
        }
    }

    #endregion

    #region General Outermorphism Properties Tests

    [TestFixture]
    public class OutermorphismPropertiesTests
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

        private XGaFloat64DiagonalOutermorphism CreateDiagonalOutermorphism(double[] scalars)
        {
            // Create a diagonal outermorphism using the correct API
            // Build a vector with the diagonal scalars
            var diagonalVector = _processor.VectorZero;
            for (int i = 0; i < scalars.Length && i < VSpaceDimensions; i++)
            {
                diagonalVector += _processor.VectorTerm(i, scalars[i]);
            }
            return diagonalVector.ToDiagonalAutomorphism();
        }

        [Test]
        public void Outermorphism_PreservesGrade()
        {
            // Outermorphisms preserve the grade of k-vectors
            var scalars = new[] { 2.0, 3.0, 4.0, 5.0, 6.0 };
            var outermorphism = CreateDiagonalOutermorphism(scalars);

            // Use deterministic k-vector instead of random to avoid numerical precision issues
            const int grade = 3;
            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);
            var e2 = _processor.VectorTerm(2);
            var kVector = e0.Op(e1).Op(e2).GetKVectorPart(grade);

            var mapped = outermorphism.OmMap(kVector);

            TestUtils.AssertGrade(mapped, grade,
                $"Outermorphism should preserve grade of k-vectors");
        }

        [Test]
        public void Outermorphism_Linearity_ScalarMultiplication()
        {
            // f(c·a) = c·f(a) for scalar c
            var scalars = new[] { 2.0, 3.0, 4.0, 5.0, 6.0 };
            var outermorphism = CreateDiagonalOutermorphism(scalars);

            var v = _random.GetVector();
            var scalar = 3.7;

            var mappedScaled = outermorphism.OmMap(v * scalar);
            var scaledMapped = outermorphism.OmMap(v) * scalar;

            TestUtils.AssertMultivectorEquals(mappedScaled, scaledMapped, Tolerance,
                "Outermorphism should commute with scalar multiplication");
        }

        [Test]
        public void Outermorphism_PreservesOuterProduct_Vectors()
        {
            // For any outermorphism f: f(a ∧ b) = f(a) ∧ f(b)
            var scalars = new[] { 2.0, 3.0, 4.0, 5.0, 6.0 };
            var outermorphism = CreateDiagonalOutermorphism(scalars);

            var a = _random.GetVector();
            var b = _random.GetVector();

            var outerProductMapped = outermorphism.OmMap(a.Op(b).GetBivectorPart());

            var aMapped = outermorphism.OmMap(a);
            var bMapped = outermorphism.OmMap(b);
            var mappedOuterProduct = aMapped.Op(bMapped);

            TestUtils.AssertMultivectorEquals(outerProductMapped, mappedOuterProduct, Tolerance,
                "Outermorphism should preserve outer product: f(a∧b) = f(a)∧f(b)");
        }

        [Test]
        public void Outermorphism_PreservesOuterProduct_ThreeVectors()
        {
            // For three vectors: f(a ∧ b ∧ c) = f(a) ∧ f(b) ∧ f(c)
            var scalars = new[] { 2.0, 3.0, 4.0, 5.0, 6.0 };
            var outermorphism = CreateDiagonalOutermorphism(scalars);

            var a = _processor.VectorTerm(0);
            var b = _processor.VectorTerm(1);
            var c = _processor.VectorTerm(2);

            var trivector = a.Op(b).Op(c).GetKVectorPart(3);
            var trivectorMapped = outermorphism.OmMap(trivector);

            var aMapped = outermorphism.OmMap(a);
            var bMapped = outermorphism.OmMap(b);
            var cMapped = outermorphism.OmMap(c);
            var mappedTrivector = aMapped.Op(bMapped).Op(cMapped);

            TestUtils.AssertMultivectorEquals(trivectorMapped, mappedTrivector, Tolerance,
                "Outermorphism should preserve triple outer product");
        }

        [Test]
        public void Outermorphism_BasisBlade_Mapping()
        {
            // Test mapping of basis blades
            var scalars = new[] { 2.0, 3.0, 4.0, 5.0, 6.0 };
            var outermorphism = CreateDiagonalOutermorphism(scalars);

            // Map basis bivector e₀ ∧ e₁
            var basisBivector = outermorphism.OmMapBasisBivector(0, 1);

            // Expected: (2·e₀) ∧ (3·e₁) = 6·(e₀∧e₁)
            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);
            var expected = e0.Op(e1) * (scalars[0] * scalars[1]);

            TestUtils.AssertMultivectorEquals(expected, basisBivector, Tolerance,
                "Basis bivector should be scaled by product of corresponding scalars");
        }

        [Test]
        public void Outermorphism_MapsZeroToZero()
        {
            // Any outermorphism maps zero to zero
            var scalars = new[] { 2.0, 3.0, 4.0, 5.0, 6.0 };
            var outermorphism = CreateDiagonalOutermorphism(scalars);

            var zero = _processor.ScalarZero;
            var mapped = outermorphism.OmMap(zero);

            TestUtils.AssertNearZero(mapped, Tolerance,
                "Outermorphism should map zero to zero");
        }
    }

    #endregion

    #region Scaling Outermorphism Tests

    [TestFixture]
    public class ScalingOutermorphismTests
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

        private XGaFloat64DiagonalOutermorphism CreateDiagonalOutermorphism(double[] scalars)
        {
            // Create a diagonal outermorphism using the correct API
            // Build a vector with the diagonal scalars
            var diagonalVector = _processor.VectorZero;
            for (int i = 0; i < scalars.Length && i < VSpaceDimensions; i++)
            {
                diagonalVector += _processor.VectorTerm(i, scalars[i]);
            }
            return diagonalVector.ToDiagonalAutomorphism();
        }

        [Test]
        public void ScalingOutermorphism_UniformScaling()
        {
            // Uniform scaling by factor k
            var scaleFactor = 2.5;
            var scalars = new double[VSpaceDimensions];
            for (int i = 0; i < VSpaceDimensions; i++)
                scalars[i] = scaleFactor;

            var scaling = CreateDiagonalOutermorphism(scalars);

            var v = _random.GetVector();
            var mapped = scaling.OmMap(v);

            var expected = v * scaleFactor;

            TestUtils.AssertMultivectorEquals(expected, mapped, Tolerance,
                "Uniform scaling should multiply vector by scale factor");
        }

        [Test]
        public void ScalingOutermorphism_ScalesBivectorByProduct()
        {
            // Scaling a bivector: f(a∧b) = (k₁·a) ∧ (k₂·b) = k₁·k₂·(a∧b)
            var scaleFactor1 = 2.0;
            var scaleFactor2 = 3.0;
            var scalars = new[] { scaleFactor1, scaleFactor2, 1.0, 1.0, 1.0 };

            var scaling = CreateDiagonalOutermorphism(scalars);

            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);
            var bivector = e0.Op(e1).GetBivectorPart();

            var mapped = scaling.OmMap(bivector);

            var expected = bivector * (scaleFactor1 * scaleFactor2);

            TestUtils.AssertMultivectorEquals(expected, mapped, Tolerance,
                "Scaling should multiply bivector by product of scale factors");
        }

        [Test]
        public void ScalingOutermorphism_DeterminantRelation()
        {
            // For a k-blade: f(B) is scaled by product of k eigenvalues
            // This is related to the determinant of the linear map
            var scalars = new[] { 2.0, 3.0, 4.0, 5.0, 6.0 };
            var scaling = CreateDiagonalOutermorphism(scalars);

            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);
            var e2 = _processor.VectorTerm(2);

            var trivector = e0.Op(e1).Op(e2).GetKVectorPart(3);
            var mapped = scaling.OmMap(trivector);

            // Expected scale factor: 2 * 3 * 4 = 24
            var expectedScaleFactor = scalars[0] * scalars[1] * scalars[2];
            var expected = trivector * expectedScaleFactor;

            TestUtils.AssertMultivectorEquals(expected, mapped, Tolerance,
                "Trivector should be scaled by product of three scale factors");
        }
    }

    #endregion
}
