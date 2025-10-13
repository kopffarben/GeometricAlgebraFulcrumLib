using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Projectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.UnitTests.Utilities;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.LinearMaps;

/// <summary>
/// Comprehensive tests for projectors in Geometric Algebra
/// A projector is a linear map that projects onto a subspace defined by a blade
/// Tests idempotence, orthogonal projections, and subspace properties
///
/// NOTE: These tests are currently DISABLED because there is no XGaFloat64Projector creation API.
/// The library provides ProjectOn() method directly on multivectors, not a Projector class.
/// These tests need to be rewritten to use ProjectOn() instead of creating Projector objects.
/// </summary>
[TestFixture]
[Ignore("No Projector creation API - tests need rewrite to use ProjectOn() method")]
public class ProjectorsTests
{
#if false
    private const double Tolerance = 1e-10;

    #region Basic Projector Tests

    [TestFixture]
    public class BasicProjectorTests
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
        public void Projector_IdempotenceProperty()
        {
            // P(P(v)) = P(v) - Fundamental property of projectors
            var blade = _random.GetKVector(2);
            var projector = blade.CreateProjector();

            var v = _random.GetVector();
            var projected = projector.OmMap(v);
            var doubleProjected = projector.OmMap(projected);

            TestUtils.AssertMultivectorEquals(projected, doubleProjected, Tolerance,
                "Projector should be idempotent: P(P(v)) = P(v)");
        }

        [Test]
        public void Projector_IsLinearMap()
        {
            // P(a + b) = P(a) + P(b)
            var blade = _random.GetKVector(2);
            var projector = blade.CreateProjector();

            var a = _random.GetVector();
            var b = _random.GetVector();

            var projectedSum = projector.OmMap(a + b);
            var sumOfProjected = projector.OmMap(a) + projector.OmMap(b);

            TestUtils.AssertMultivectorEquals(projectedSum, sumOfProjected, Tolerance,
                "Projector should be linear: P(a+b) = P(a) + P(b)");
        }

        [Test]
        public void Projector_ScalarMultiplicationCommutes()
        {
            // P(c·v) = c·P(v) for scalar c
            var blade = _random.GetKVector(2);
            var projector = blade.CreateProjector();

            var v = _random.GetVector();
            var scalar = 3.5;

            var projectedScaled = projector.OmMap(v * scalar);
            var scaledProjected = projector.OmMap(v) * scalar;

            TestUtils.AssertMultivectorEquals(projectedScaled, scaledProjected, Tolerance,
                "Projector should commute with scalar multiplication");
        }

        [Test]
        public void Projector_MapsZeroToZero()
        {
            // P(0) = 0
            var blade = _random.GetKVector(2);
            var projector = blade.CreateProjector();

            var zero = _processor.ScalarZero;
            var projected = projector.OmMap(zero);

            TestUtils.AssertNearZero(projected, Tolerance,
                "Projector should map zero to zero");
        }

        [Test]
        public void Projector_PreservesGrade()
        {
            // Projecting a k-vector should yield a k-vector
            var blade = _random.GetKVector(2);
            var projector = blade.CreateProjector();

            const int grade = 3;
            var kVector = _random.GetKVector(grade);
            var projected = projector.OmMap(kVector);

            TestUtils.AssertGrade(projected, grade,
                "Projector should preserve grade of k-vectors");
        }
    }

    #endregion

    #region Vector Projection Tests

    [TestFixture]
    public class VectorProjectionTests
    {
        private XGaFloat64Processor _processor = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            _processor = XGaFloat64Processor.Euclidean;
        }

        [Test]
        public void VectorProjector_ProjectOntoVector()
        {
            // Project a vector onto another vector (1D subspace)
            var e0 = _processor.VectorTerm(0);
            var projector = e0.CreateProjector();

            // Project (1, 2, 0) onto e₀
            var e1 = _processor.VectorTerm(1);
            var v = e0 + e1 * 2;

            var projected = projector.OmMap(v);

            // Expected: projection onto e₀ is just e0 component = (1, 0, 0)
            TestUtils.AssertMultivectorEquals(e0, projected, Tolerance,
                "Projection onto e₀ should give e₀ component only");
        }

        [Test]
        public void VectorProjector_ProjectParallelVector()
        {
            // Projecting a vector parallel to the blade should return the vector itself
            var e0 = _processor.VectorTerm(0);
            var projector = e0.CreateProjector();

            var parallel = e0 * 5;
            var projected = projector.OmMap(parallel);

            TestUtils.AssertMultivectorEquals(parallel, projected, Tolerance,
                "Projecting parallel vector should return the vector itself");
        }

        [Test]
        public void VectorProjector_ProjectPerpendicularVector()
        {
            // Projecting a vector perpendicular to the blade should return zero
            var e0 = _processor.VectorTerm(0);
            var projector = e0.CreateProjector();

            var perpendicular = _processor.VectorTerm(1); // e₁ ⊥ e₀
            var projected = projector.OmMap(perpendicular);

            TestUtils.AssertNearZero(projected, Tolerance,
                "Projecting perpendicular vector should return zero");
        }

        [Test]
        public void VectorProjector_PythagoreanDecomposition()
        {
            // v = P(v) + P⊥(v), where P⊥ is the orthogonal complement
            // ||v||² = ||P(v)||² + ||P⊥(v)||²
            var e0 = _processor.VectorTerm(0);
            var projector = e0.CreateProjector();

            var e1 = _processor.VectorTerm(1);
            var v = e0 * 3 + e1 * 4; // (3, 4)

            var projected = projector.OmMap(v);
            var orthogonal = v - projected;

            // Check orthogonality: P(v) · P⊥(v) = 0
            var dotProduct = projected.Sp(orthogonal).ScalarValue;
            TestUtils.AssertNearZero(dotProduct, Tolerance,
                "Projected and orthogonal components should be orthogonal");

            // Check Pythagorean theorem
            var vNormSq = v.NormSquared().ScalarValue;
            var projectedNormSq = projected.NormSquared().ScalarValue;
            var orthogonalNormSq = orthogonal.NormSquared().ScalarValue;

            TestUtils.AssertDoubleEquals(vNormSq, projectedNormSq + orthogonalNormSq, Tolerance,
                "Pythagorean theorem: ||v||² = ||P(v)||² + ||P⊥(v)||²");
        }
    }

    #endregion

    #region Plane Projection Tests

    [TestFixture]
    public class PlaneProjectionTests
    {
        private XGaFloat64Processor _processor = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            _processor = XGaFloat64Processor.Euclidean;
        }

        [Test]
        public void PlaneProjector_ProjectOntoXYPlane()
        {
            // Project onto XY plane (defined by e₀ ∧ e₁)
            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);
            var e2 = _processor.VectorTerm(2);

            var xyPlane = e0.Op(e1).GetBivectorPart();
            var projector = xyPlane.CreateProjector();

            // Project (1, 2, 3) onto XY plane
            var v = e0 + e1 * 2 + e2 * 3;
            var projected = projector.OmMap(v);

            // Expected: (1, 2, 0) - Z component removed
            var expected = e0 + e1 * 2;

            TestUtils.AssertMultivectorEquals(expected, projected, Tolerance,
                "Projection onto XY plane should remove Z component");
        }

        [Test]
        public void PlaneProjector_ProjectVectorInPlane()
        {
            // A vector lying in the plane should be unchanged
            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);

            var xyPlane = e0.Op(e1).GetBivectorPart();
            var projector = xyPlane.CreateProjector();

            var inPlane = e0 * 5 + e1 * 7;
            var projected = projector.OmMap(inPlane);

            TestUtils.AssertMultivectorEquals(inPlane, projected, Tolerance,
                "Vector in plane should be unchanged by projection");
        }

        [Test]
        public void PlaneProjector_ProjectVectorPerpendicularToPlane()
        {
            // A vector perpendicular to the plane should project to zero
            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);
            var e2 = _processor.VectorTerm(2);

            var xyPlane = e0.Op(e1).GetBivectorPart();
            var projector = xyPlane.CreateProjector();

            var perpendicular = e2 * 5; // Perpendicular to XY plane
            var projected = projector.OmMap(perpendicular);

            TestUtils.AssertNearZero(projected, Tolerance,
                "Vector perpendicular to plane should project to zero");
        }

        [Test]
        public void PlaneProjector_ProjectBivector()
        {
            // Project a bivector onto a plane
            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);
            var e2 = _processor.VectorTerm(2);

            var xyPlane = e0.Op(e1).GetBivectorPart();
            var projector = xyPlane.CreateProjector();

            // Bivector e₀∧e₂ (in XZ plane)
            var bivector = e0.Op(e2).GetBivectorPart();
            var projected = projector.OmMap(bivector);

            // Projection of a bivector onto a plane
            // The result should still be a bivector
            TestUtils.AssertGrade(projected, 2,
                "Projecting bivector should yield bivector");
        }
    }

    #endregion

    #region Subspace Projection Tests

    [TestFixture]
    public class SubspaceProjectionTests
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
        public void Projector_ProjectionReducesNorm()
        {
            // ||P(v)|| ≤ ||v|| - Projection cannot increase norm
            var blade = _random.GetKVector(2);
            var projector = blade.CreateProjector();

            var v = _random.GetVector();
            var projected = projector.OmMap(v);

            var originalNorm = v.Norm().ScalarValue;
            var projectedNorm = projected.Norm().ScalarValue;

            Assert.That(projectedNorm <= originalNorm + Tolerance,
                $"Projected norm ({projectedNorm}) should not exceed original norm ({originalNorm})");
        }

        [Test]
        public void Projector_ProjectionIntoSubspace()
        {
            // The projected vector lies in the subspace defined by the blade
            // For a blade B, P(v) can be expressed as a linear combination of vectors in B's subspace
            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);
            var blade = e0.Op(e1).GetBivectorPart();

            var projector = blade.CreateProjector();

            var e2 = _processor.VectorTerm(2);
            var v = e0 + e1 + e2; // (1, 1, 1)

            var projected = projector.OmMap(v);

            // Projected vector should have zero e₂ component
            var e2Component = projected.Sp(e2).ScalarValue;
            TestUtils.AssertNearZero(e2Component, Tolerance,
                "Projected vector should have no component perpendicular to the subspace");
        }

        [Test]
        public void Projector_OrthogonalComplement()
        {
            // For projector P, the orthogonal complement is Q = I - P
            // where Q(v) is orthogonal to P(v)
            var blade = _random.GetKVector(2);
            var projector = blade.CreateProjector();

            var v = _random.GetVector();
            var projected = projector.OmMap(v);
            var orthogonal = v - projected;

            // Check orthogonality
            var dotProduct = projected.Sp(orthogonal).ScalarValue;

            TestUtils.AssertNearZero(dotProduct, Tolerance,
                "Projected and orthogonal complement should be orthogonal");
        }

        [Test]
        public void Projector_IdempotenceOnBivector()
        {
            // P(P(B)) = P(B) for bivector B
            var blade = _random.GetKVector(2);
            var projector = blade.CreateProjector();

            var bivector = _random.GetBivector();
            var projected = projector.OmMap(bivector);
            var doubleProjected = projector.OmMap(projected);

            TestUtils.AssertMultivectorEquals(projected, doubleProjected, Tolerance,
                "Projector should be idempotent on bivectors");
        }

        [Test]
        public void Projector_ProjectOntoHigherGradeSubspace()
        {
            // Project onto a 3D subspace (trivector blade)
            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);
            var e2 = _processor.VectorTerm(2);

            var blade3D = e0.Op(e1).Op(e2).GetKVectorPart();
            var projector = blade3D.CreateProjector();

            var e3 = _processor.VectorTerm(3);
            var v = e0 + e1 + e2 + e3; // (1, 1, 1, 1, 0)

            var projected = projector.OmMap(v);

            // Projected should have no e₃ component
            var e3Component = projected.Sp(e3).ScalarValue;
            TestUtils.AssertNearZero(e3Component, Tolerance,
                "Projected vector should have no e₃ component");
        }

        [Test]
        public void Projector_PreservesSubspaceVectors()
        {
            // Vectors already in the subspace should be unchanged
            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);
            var blade = e0.Op(e1).GetBivectorPart();

            var projector = blade.CreateProjector();

            // Vector in the subspace
            var inSubspace = e0 * 3 + e1 * 4;
            var projected = projector.OmMap(inSubspace);

            TestUtils.AssertMultivectorEquals(inSubspace, projected, Tolerance,
                "Vectors in the subspace should be unchanged by projection");
        }
    }

    #endregion

    #region Geometric Properties Tests

    [TestFixture]
    public class ProjectorGeometricPropertiesTests
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
        public void Projector_MinimizesDistance()
        {
            // P(v) is the closest point in the subspace to v
            // ||v - P(v)|| ≤ ||v - w|| for any w in the subspace
            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);
            var blade = e0.Op(e1).GetBivectorPart();

            var projector = blade.CreateProjector();

            var e2 = _processor.VectorTerm(2);
            var v = e0 + e1 + e2 * 5; // (1, 1, 5)

            var projected = projector.OmMap(v);
            var distanceToProjection = (v - projected).Norm().ScalarValue;

            // Try another point in the subspace
            var w = e0 * 2 + e1 * 3; // (2, 3, 0) - in the XY plane
            var distanceToW = (v - w).Norm().ScalarValue;

            Assert.That(distanceToProjection <= distanceToW + Tolerance,
                "Distance to projection should be minimal");
        }

        [Test]
        public void Projector_PreservesAnglesInSubspace()
        {
            // Angles between projected vectors are preserved from original subspace angles
            // If a and b are in the subspace, angle(P(a), P(b)) = angle(a, b)
            var e0 = _processor.VectorTerm(0);
            var e1 = _processor.VectorTerm(1);
            var blade = e0.Op(e1).GetBivectorPart();

            var projector = blade.CreateProjector();

            // Vectors in the subspace
            var a = e0;
            var b = e1;

            var aProjected = projector.OmMap(a);
            var bProjected = projector.OmMap(b);

            // Since they're already in the subspace, they should be unchanged
            TestUtils.AssertMultivectorEquals(a, aProjected, Tolerance, "a should be unchanged");
            TestUtils.AssertMultivectorEquals(b, bProjected, Tolerance, "b should be unchanged");

            // Angle should be preserved (90 degrees for e₀ and e₁)
            var originalAngle = a.Sp(b).ScalarValue; // = 0
            var projectedAngle = aProjected.Sp(bProjected).ScalarValue; // = 0

            TestUtils.AssertDoubleEquals(originalAngle, projectedAngle, Tolerance,
                "Angles in subspace should be preserved");
        }

        [Test]
        public void Projector_CompositionWithItself()
        {
            // P ∘ P = P (composition with itself is itself)
            var blade = _random.GetKVector(2);
            var projector = blade.CreateProjector();

            var v = _random.GetVector();

            // Apply projector twice
            var projected1 = projector.OmMap(v);
            var projected2 = projector.OmMap(projected1);

            TestUtils.AssertMultivectorEquals(projected1, projected2, Tolerance,
                "P ∘ P = P (projector composed with itself equals itself)");
        }
    }

    #endregion
#endif
}
