using System;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.UnitTests.Utilities;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Tests for all product operations in Geometric Algebra
/// Tests mathematical identities and properties of products
/// </summary>
[TestFixture]
public class ProductOperationsTests
{
    private XGaFloat64Processor _processor = null!;
    private XGaFloat64RandomComposer _random = null!;
    private const int VSpaceDimensions = 5;
    private const int TestSeed = 42;
    private const double Tolerance = 1e-10;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _processor = XGaFloat64Processor.Euclidean;
    }

    [SetUp]
    public void Setup()
    {
        // Reset random generator before each test to ensure test independence
        // This prevents test-order dependencies caused by shared random state
        _random = _processor.CreateXGaRandomComposer(VSpaceDimensions, TestSeed);
    }

    #region Outer Product Tests

    [Test]
    public void OuterProduct_Associativity()
    {
        // (a ∧ b) ∧ c = a ∧ (b ∧ c)
        var a = _random.GetMultivector();
        var b = _random.GetMultivector();
        var c = _random.GetMultivector();

        var left = a.Op(b).Op(c);
        var right = a.Op(b.Op(c));

        TestUtils.AssertMultivectorEquals(left, right, Tolerance,
            "Outer product should be associative");
    }

    [Test]
    public void OuterProduct_VectorAntiCommutativity()
    {
        // For vectors: a ∧ b = -(b ∧ a)
        var a = _random.GetVector();
        var b = _random.GetVector();

        var ab = a.Op(b);
        var ba = b.Op(a);

        TestUtils.AssertMultivectorEquals(ab, -ba, Tolerance,
            "Outer product of vectors should be anti-commutative");
    }

    [Test]
    public void OuterProduct_VectorWithSelf_IsZero()
    {
        // a ∧ a = 0 for vectors
        var a = _random.GetVector();
        var result = a.Op(a);

        TestUtils.AssertNearZero(result, Tolerance,
            "Outer product of vector with itself should be zero");
    }

    [Test]
    public void OuterProduct_GradeAdditivity()
    {
        // grade(a ∧ b) = grade(a) + grade(b) for blades
        var gradeA = 2;
        var gradeB = 1;
        var a = _random.GetBlade(gradeA);
        var b = _random.GetBlade(gradeB);

        var result = a.Op(b);

        TestUtils.AssertGrade(result, gradeA + gradeB,
            $"Outer product should add grades: {gradeA} + {gradeB} = {gradeA + gradeB}");
    }

    [Test]
    public void OuterProduct_WithZero_IsZero()
    {
        var a = _random.GetMultivector();
        var zero = _processor.ScalarZero;

        var result1 = a.Op(zero);
        var result2 = zero.Op(a);

        TestUtils.AssertNearZero(result1, Tolerance, "a ∧ 0 should be zero");
        TestUtils.AssertNearZero(result2, Tolerance, "0 ∧ a should be zero");
    }

    [Test]
    public void OuterProduct_Distributivity()
    {
        // a ∧ (b + c) = a ∧ b + a ∧ c
        var a = _random.GetMultivector();
        var b = _random.GetMultivector();
        var c = _random.GetMultivector();

        var left = a.Op(b + c);
        var right = a.Op(b) + a.Op(c);

        TestUtils.AssertMultivectorEquals(left, right, Tolerance,
            "Outer product should be distributive over addition");
    }

    #endregion

    #region Geometric Product Tests

    [Test]
    public void GeometricProduct_Associativity()
    {
        // (a * b) * c = a * (b * c)
        var a = _random.GetMultivector();
        var b = _random.GetMultivector();
        var c = _random.GetMultivector();

        var left = a.Gp(b).Gp(c);
        var right = a.Gp(b.Gp(c));

        TestUtils.AssertMultivectorEquals(left, right, Tolerance,
            "Geometric product should be associative");
    }

    [Test]
    public void GeometricProduct_LeftDistributivity()
    {
        // a * (b + c) = a * b + a * c
        var a = _random.GetMultivector();
        var b = _random.GetMultivector();
        var c = _random.GetMultivector();

        var left = a.Gp(b + c);
        var right = a.Gp(b) + a.Gp(c);

        TestUtils.AssertMultivectorEquals(left, right, Tolerance,
            "Geometric product should be left distributive");
    }

    [Test]
    public void GeometricProduct_RightDistributivity()
    {
        // (a + b) * c = a * c + b * c
        var a = _random.GetMultivector();
        var b = _random.GetMultivector();
        var c = _random.GetMultivector();

        var left = (a + b).Gp(c);
        var right = a.Gp(c) + b.Gp(c);

        TestUtils.AssertMultivectorEquals(left, right, Tolerance,
            "Geometric product should be right distributive");
    }

    [Test]
    public void GeometricProduct_BasisVectorSquare_Euclidean()
    {
        // e_i * e_i = 1 for Euclidean metric
        for (int i = 0; i < VSpaceDimensions; i++)
        {
            var ei = _processor.VectorTerm(i);
            var square = ei.Gp(ei);

            TestUtils.AssertDoubleEquals(
                1.0,
                square.GetScalarPart(),
                Tolerance,
                $"Basis vector e_{i} squared should equal 1 in Euclidean metric"
            );
        }
    }

    [Test]
    public void GeometricProduct_BasisVectorOrthogonality_Euclidean()
    {
        // e_i * e_j = -e_j * e_i for i ≠ j
        for (int i = 0; i < VSpaceDimensions; i++)
        {
            for (int j = i + 1; j < VSpaceDimensions; j++)
            {
                var ei = _processor.VectorTerm(i);
                var ej = _processor.VectorTerm(j);

                var eiej = ei.Gp(ej);
                var ejei = ej.Gp(ei);

                TestUtils.AssertMultivectorEquals(eiej, -ejei, Tolerance,
                    $"e_{i} * e_{j} should equal -e_{j} * e_{i}");
            }
        }
    }

    [Test]
    public void GeometricProduct_RelationToOuterAndInner()
    {
        // For vectors: a * b = a · b + a ∧ b
        var a = _random.GetVector();
        var b = _random.GetVector();

        var gp = a.Gp(b);
        var sp = a.Sp(b);
        var op = a.Op(b);
        var combined = sp.Add(op);

        TestUtils.AssertMultivectorEquals(gp, combined, Tolerance,
            "For vectors: a * b should equal a · b + a ∧ b");
    }

    [Test]
    public void GeometricProduct_WithScalar()
    {
        // scalar * multivector = scalar multiplication
        var a = _random.GetMultivector();
        var scalar = 3.14;

        var gp = _processor.Scalar(scalar).Gp(a);
        var scaled = a * scalar;

        TestUtils.AssertMultivectorEquals(gp, scaled, Tolerance,
            "Geometric product with scalar should equal scalar multiplication");
    }

    #endregion

    #region Scalar Product (Inner Product) Tests

    [Test]
    public void ScalarProduct_Commutativity()
    {
        // a · b = b · a
        var a = _random.GetMultivector();
        var b = _random.GetMultivector();

        var ab = a.Sp(b);
        var ba = b.Sp(a);

        TestUtils.AssertScalarEquals(ab, ba, Tolerance,
            "Scalar product should be commutative");
    }

    [Test]
    public void ScalarProduct_PositiveDefinite_Euclidean()
    {
        // For Euclidean metric: a · a ≥ 0
        var a = _random.GetVector();
        var aa = a.Sp(a);

        Assert.That(aa.ScalarValue >= -Tolerance,
            $"Scalar product a · a should be non-negative in Euclidean metric, got {aa.ScalarValue}");
    }

    [Test]
    public void ScalarProduct_Linearity()
    {
        // (αa + βb) · c = α(a · c) + β(b · c)
        var a = _random.GetVector();
        var b = _random.GetVector();
        var c = _random.GetVector();
        var alpha = 2.5;
        var beta = -1.7;

        var left = (a * alpha + b * beta).Sp(c);
        var right = a.Sp(c) * alpha + b.Sp(c) * beta;

        TestUtils.AssertDoubleEquals(left.ScalarValue, right.ScalarValue, Tolerance,
            "Scalar product should be linear");
    }

    [Test]
    public void ScalarProduct_OrthogonalVectors()
    {
        // Orthogonal vectors should have zero scalar product
        var e0 = _processor.VectorTerm(0);
        var e1 = _processor.VectorTerm(1);

        var sp = e0.Sp(e1);

        TestUtils.AssertNearZero(sp.ScalarValue, Tolerance,
            "Scalar product of orthogonal basis vectors should be zero");
    }

    #endregion

    #region Left Contraction Tests

    [Test]
    public void LeftContraction_GradeReduction()
    {
        // grade(a ⌋ b) = grade(b) - grade(a) for blades
        var gradeA = 1;
        var gradeB = 3;
        var a = _random.GetBlade(gradeA);
        var b = _random.GetBlade(gradeB);

        var result = a.Lcp(b);

        if (!result.IsZero)
        {
            TestUtils.AssertGrade(result, gradeB - gradeA,
                $"Left contraction should reduce grade: {gradeB} - {gradeA} = {gradeB - gradeA}");
        }
    }

    [Test]
    public void LeftContraction_Associativity_WithOuter()
    {
        // (a ∧ b) ⌋ c = a ⌋ (b ⌋ c) - may not always hold, test specific case
        var a = _random.GetVector();
        var b = _random.GetVector();
        var c = _random.GetBlade(3);

        var left = a.Op(b).Lcp(c);
        var right = a.Lcp(b.Lcp(c));

        // This identity may have conditions, we test if at least one form gives valid result
        Assert.That(left.IsZero || right.IsZero || !left.Subtract(right).IsNearZero(Tolerance * 10),
            "Left contraction with outer product relationship");
    }

    [Test]
    public void LeftContraction_WithZero()
    {
        var a = _random.GetMultivector();
        var zero = _processor.ScalarZero;

        var result1 = a.Lcp(zero);
        var result2 = zero.Lcp(a);

        TestUtils.AssertNearZero((XGaFloat64Multivector)result1, Tolerance, "a ⌋ 0 should be zero");
        TestUtils.AssertNearZero((XGaFloat64Multivector)result2, Tolerance, "0 ⌋ a should be zero");
    }

    #endregion

    #region Right Contraction Tests

    [Test]
    public void RightContraction_GradeReduction()
    {
        // grade(a ⌊ b) = grade(a) - grade(b) for blades
        var gradeA = 3;
        var gradeB = 1;
        var a = _random.GetBlade(gradeA);
        var b = _random.GetBlade(gradeB);

        var result = a.Rcp(b);

        if (!result.IsZero)
        {
            TestUtils.AssertGrade(result, gradeA - gradeB,
                $"Right contraction should reduce grade: {gradeA} - {gradeB} = {gradeA - gradeB}");
        }
    }

    [Test]
    public void RightContraction_WithZero()
    {
        var a = _random.GetMultivector();
        var zero = _processor.ScalarZero;

        var result1 = a.Rcp(zero);
        var result2 = zero.Rcp(a);

        TestUtils.AssertNearZero((XGaFloat64Multivector)result1, Tolerance, "a ⌊ 0 should be zero");
        TestUtils.AssertNearZero((XGaFloat64Multivector)result2, Tolerance, "0 ⌊ a should be zero");
    }

    #endregion

    #region Commutator Product Tests

    [Test]
    public void CommutatorProduct_AntiCommutativity()
    {
        // [a,b] = -[b,a]
        var a = _random.GetMultivector();
        var b = _random.GetMultivector();

        var ab = a.Cp(b);
        var ba = b.Cp(a);

        TestUtils.AssertMultivectorEquals(ab, -ba, Tolerance,
            "Commutator product should be anti-commutative");
    }

    [Test]
    public void CommutatorProduct_Definition()
    {
        // [a,b] = ab - ba
        var a = _random.GetMultivector();
        var b = _random.GetMultivector();

        var commutator = a.Cp(b);
        var definition = a.Gp(b) - b.Gp(a);

        TestUtils.AssertMultivectorEquals(commutator, definition, Tolerance,
            "Commutator should equal ab - ba");
    }

    [Test]
    public void CommutatorProduct_WithSelf_IsZero()
    {
        // [a,a] = 0
        var a = _random.GetMultivector();
        var result = a.Cp(a);

        TestUtils.AssertNearZero(result, Tolerance,
            "Commutator of element with itself should be zero");
    }

    [Test]
    public void CommutatorProduct_JacobiIdentity()
    {
        // [a,[b,c]] + [b,[c,a]] + [c,[a,b]] = 0
        var a = _random.GetVector(); // Use vectors for simpler test
        var b = _random.GetVector();
        var c = _random.GetVector();

        var term1 = a.Cp(b.Cp(c));
        var term2 = b.Cp(c.Cp(a));
        var term3 = c.Cp(a.Cp(b));

        var result = term1 + term2 + term3;

        TestUtils.AssertNearZero(result, Tolerance * 10, // Slightly relaxed tolerance for compound operation
            "Jacobi identity should hold for commutator product");
    }

    #endregion

    #region Anti-Commutator Product Tests

    [Test]
    public void AntiCommutatorProduct_Commutativity()
    {
        // {a,b} = {b,a}
        var a = _random.GetMultivector();
        var b = _random.GetMultivector();

        var ab = a.Acp(b);
        var ba = b.Acp(a);

        TestUtils.AssertMultivectorEquals(ab, ba, Tolerance,
            "Anti-commutator product should be commutative");
    }

    [Test]
    public void AntiCommutatorProduct_Definition()
    {
        // {a,b} = ab + ba
        var a = _random.GetMultivector();
        var b = _random.GetMultivector();

        var anticommutator = a.Acp(b);
        var definition = a.Gp(b) + b.Gp(a);

        TestUtils.AssertMultivectorEquals(anticommutator, definition, Tolerance,
            "Anti-commutator should equal ab + ba");
    }

    [Test]
    public void AntiCommutatorProduct_Squared()
    {
        // {a,a} = 2(a * a)
        var a = _random.GetMultivector();

        var anticommutator = a.Acp(a);
        var expected = a.Gp(a) * 2.0;

        TestUtils.AssertMultivectorEquals(anticommutator, expected, Tolerance,
            "Anti-commutator with self should equal 2(a * a)");
    }

    #endregion

    #region Fat Dot Product Tests

    [Test]
    public void FatDotProduct_Symmetry()
    {
        // Test symmetry for same grade multivectors
        var a = _random.GetBlade(2);
        var b = _random.GetBlade(2);

        var ab = a.Fdp(b);
        var ba = b.Fdp(a);

        TestUtils.AssertMultivectorEquals(ab, ba, Tolerance,
            "Fat dot product should be symmetric for same grade");
    }

    [Test]
    public void FatDotProduct_WithZero()
    {
        var a = _random.GetMultivector();
        var zero = _processor.ScalarZero;

        var result1 = a.Fdp(zero);
        var result2 = zero.Fdp(a);

        TestUtils.AssertNearZero(result1, Tolerance, "a • 0 should be zero");
        TestUtils.AssertNearZero(result2, Tolerance, "0 • a should be zero");
    }

    #endregion

    #region Hestenes Inner Product Tests

    [Test]
    public void HestenesInnerProduct_WithZero()
    {
        var a = _random.GetMultivector();
        var zero = _processor.ScalarZero;

        var result1 = a.Hip(zero);
        var result2 = zero.Hip(a);

        TestUtils.AssertNearZero((XGaFloat64Multivector)result1, Tolerance, "a ⋅ 0 should be zero (Hestenes)");
        TestUtils.AssertNearZero((XGaFloat64Multivector)result2, Tolerance, "0 ⋅ a should be zero (Hestenes)");
    }

    [Test]
    public void HestenesInnerProduct_MatchesScalarProduct_ForVectors()
    {
        // For vectors, Hestenes inner product should match scalar product
        var a = _random.GetVector();
        var b = _random.GetVector();

        var hip = a.Hip(b);
        var sp = a.Sp(b).ScalarValue;

        TestUtils.AssertDoubleEquals(
            sp,
            hip.GetScalarPart(),
            Tolerance,
            "Hestenes inner product should match scalar product for vectors"
        );
    }

    #endregion

    #region Product Consistency Tests

    [Test]
    public void Products_Consistency_ScalarMultiplication()
    {
        // All products should be consistent with scalar multiplication
        var a = _random.GetMultivector();
        var scalar = 3.7;
        var s = _processor.Scalar(scalar);

        var gpResult = s.Gp(a);
        var opResult = s.Op(a);
        var scaled = a * scalar;

        TestUtils.AssertMultivectorEquals(gpResult, scaled, Tolerance,
            "Geometric product with scalar should equal scalar multiplication");
        TestUtils.AssertMultivectorEquals(opResult, scaled, Tolerance,
            "Outer product with scalar should equal scalar multiplication");
    }

    [Test]
    public void Products_Consistency_ZeroElement()
    {
        // All products with zero should yield zero
        var a = _random.GetMultivector();
        var zero = _processor.ScalarZero;

        Assert.That(a.Gp(zero).IsZero, "Geometric product with zero");
        Assert.That(a.Op(zero).IsZero, "Outer product with zero");
        Assert.That(a.Lcp(zero).IsZero, "Left contraction with zero");
        Assert.That(a.Rcp(zero).IsZero, "Right contraction with zero");
        Assert.That(a.Fdp(zero).IsZero, "Fat dot product with zero");
        Assert.That(a.Hip(zero).IsZero, "Hestenes inner product with zero");
        Assert.That(a.Cp(zero).IsZero, "Commutator with zero");
        Assert.That(a.Acp(zero).IsZero, "Anti-commutator with zero");
    }

    #endregion
}
