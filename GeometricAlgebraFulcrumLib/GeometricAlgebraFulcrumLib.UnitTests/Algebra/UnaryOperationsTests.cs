using System;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.UnitTests.Utilities;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Tests for unary operations in Geometric Algebra
/// Tests: Reverse, Grade Involution, Clifford Conjugate, Inverse, Norm, Dual
/// </summary>
[TestFixture]
public class UnaryOperationsTests
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

    #region Reverse (Reversion) Tests

    [Test]
    public void Reverse_Involution()
    {
        // reverse(reverse(a)) = a
        var a = _random.GetMultivector();
        var reversed = a.Reverse();
        var doubleReversed = reversed.Reverse();

        TestUtils.AssertMultivectorEquals(a, doubleReversed, Tolerance,
            "Double reverse should return original multivector");
    }

    [Test]
    public void Reverse_ProductRule()
    {
        // reverse(a * b) = reverse(b) * reverse(a)
        var a = _random.GetMultivector();
        var b = _random.GetMultivector();

        var left = a.Gp(b).Reverse();
        var right = b.Reverse().Gp(a.Reverse());

        TestUtils.AssertMultivectorEquals(left, right, Tolerance,
            "Reverse should reverse product order");
    }

    [Test]
    public void Reverse_GradeDependentSign_Scalar()
    {
        // Grade 0 (scalar): no sign change
        var scalar = _processor.Scalar(3.14);
        var reversed = scalar.Reverse();

        TestUtils.AssertMultivectorEquals(scalar, reversed, Tolerance,
            "Reverse of scalar should be unchanged");
    }

    [Test]
    public void Reverse_GradeDependentSign_Vector()
    {
        // Grade 1 (vector): no sign change
        var vector = _random.GetVector();
        var reversed = vector.Reverse();

        TestUtils.AssertMultivectorEquals(vector, reversed, Tolerance,
            "Reverse of vector should be unchanged");
    }

    [Test]
    public void Reverse_GradeDependentSign_Bivector()
    {
        // Grade 2 (bivector): sign change
        var bivector = _random.GetBlade(2);
        var reversed = bivector.Reverse();

        TestUtils.AssertMultivectorEquals(bivector, -reversed, Tolerance,
            "Reverse of bivector should change sign");
    }

    [Test]
    public void Reverse_GradeDependentSign_Trivector()
    {
        // Grade 3 (trivector): sign change
        var trivector = _random.GetBlade(3);
        var reversed = trivector.Reverse();

        TestUtils.AssertMultivectorEquals(trivector, -reversed, Tolerance,
            "Reverse of trivector should change sign");
    }

    [Test]
    public void Reverse_Linearity()
    {
        // reverse(a + b) = reverse(a) + reverse(b)
        var a = _random.GetMultivector();
        var b = _random.GetMultivector();

        var left = (a + b).Reverse();
        var right = a.Reverse() + b.Reverse();

        TestUtils.AssertMultivectorEquals(left, right, Tolerance,
            "Reverse should be linear");
    }

    [Test]
    public void Reverse_ScalarMultiplication()
    {
        // reverse(k * a) = k * reverse(a)
        var a = _random.GetMultivector();
        var k = 2.5;

        var left = (a * k).Reverse();
        var right = a.Reverse() * k;

        TestUtils.AssertMultivectorEquals(left, right, Tolerance,
            "Reverse should commute with scalar multiplication");
    }

    #endregion

    #region Grade Involution Tests

    [Test]
    public void GradeInvolution_Involution()
    {
        // grade_inv(grade_inv(a)) = a
        var a = _random.GetMultivector();
        var involution = a.GradeInvolution();
        var doubleInvolution = involution.GradeInvolution();

        TestUtils.AssertMultivectorEquals(a, doubleInvolution, Tolerance,
            "Double grade involution should return original");
    }

    [Test]
    public void GradeInvolution_GradeDependentSign_Scalar()
    {
        // Grade 0: no sign change
        var scalar = _processor.Scalar(3.14);
        var involution = scalar.GradeInvolution();

        TestUtils.AssertMultivectorEquals(scalar, involution, Tolerance,
            "Grade involution of scalar should be unchanged");
    }

    [Test]
    public void GradeInvolution_GradeDependentSign_Vector()
    {
        // Grade 1: sign change
        var vector = _random.GetVector();
        var involution = vector.GradeInvolution();

        TestUtils.AssertMultivectorEquals(vector, -involution, Tolerance,
            "Grade involution of vector should change sign");
    }

    [Test]
    public void GradeInvolution_GradeDependentSign_Bivector()
    {
        // Grade 2: no sign change
        var bivector = _random.GetBlade(2);
        var involution = bivector.GradeInvolution();

        TestUtils.AssertMultivectorEquals(bivector, involution, Tolerance,
            "Grade involution of bivector should be unchanged");
    }

    [Test]
    public void GradeInvolution_Linearity()
    {
        // grade_inv(a + b) = grade_inv(a) + grade_inv(b)
        var a = _random.GetMultivector();
        var b = _random.GetMultivector();

        var left = (a + b).GradeInvolution();
        var right = a.GradeInvolution() + b.GradeInvolution();

        TestUtils.AssertMultivectorEquals(left, right, Tolerance,
            "Grade involution should be linear");
    }

    #endregion

    #region Clifford Conjugate Tests

    [Test]
    public void CliffordConjugate_Involution()
    {
        // clifford_conj(clifford_conj(a)) = a
        var a = _random.GetMultivector();
        var conjugate = a.CliffordConjugate();
        var doubleConjugate = conjugate.CliffordConjugate();

        TestUtils.AssertMultivectorEquals(a, doubleConjugate, Tolerance,
            "Double Clifford conjugate should return original");
    }

    [Test]
    public void CliffordConjugate_RelationToReverseAndInvolution()
    {
        // clifford_conj(a) = reverse(grade_inv(a)) = grade_inv(reverse(a))
        var a = _random.GetMultivector();

        var cliffordConj = a.CliffordConjugate();
        var reverseInv = a.GradeInvolution().Reverse();
        var invReverse = a.Reverse().GradeInvolution();

        TestUtils.AssertMultivectorEquals(cliffordConj, reverseInv, Tolerance,
            "Clifford conjugate should equal reverse of grade involution");
        TestUtils.AssertMultivectorEquals(cliffordConj, invReverse, Tolerance,
            "Clifford conjugate should equal grade involution of reverse");
    }

    [Test]
    public void CliffordConjugate_GradeDependentSign_Scalar()
    {
        // Grade 0: no sign change
        var scalar = _processor.Scalar(3.14);
        var conjugate = scalar.CliffordConjugate();

        TestUtils.AssertMultivectorEquals(scalar, conjugate, Tolerance,
            "Clifford conjugate of scalar should be unchanged");
    }

    [Test]
    public void CliffordConjugate_GradeDependentSign_Vector()
    {
        // Grade 1: sign change
        var vector = _random.GetVector();
        var conjugate = vector.CliffordConjugate();

        TestUtils.AssertMultivectorEquals(vector, -conjugate, Tolerance,
            "Clifford conjugate of vector should change sign");
    }

    [Test]
    public void CliffordConjugate_Linearity()
    {
        // clifford_conj(a + b) = clifford_conj(a) + clifford_conj(b)
        var a = _random.GetMultivector();
        var b = _random.GetMultivector();

        var left = (a + b).CliffordConjugate();
        var right = a.CliffordConjugate() + b.CliffordConjugate();

        TestUtils.AssertMultivectorEquals(left, right, Tolerance,
            "Clifford conjugate should be linear");
    }

    #endregion

    #region Norm Tests

    [Test]
    public void Norm_NonNegative_Euclidean()
    {
        // ||a|| ≥ 0 for Euclidean metric
        var a = _random.GetMultivector();
        var norm = a.Norm().ScalarValue;

        Assert.That(norm >= -Tolerance,
            $"Norm should be non-negative in Euclidean metric, got {norm}");
    }

    [Test]
    public void Norm_ZeroForZeroMultivector()
    {
        var zero = _processor.ScalarZero;
        var norm = zero.Norm().ScalarValue;

        TestUtils.AssertNearZero(norm, Tolerance,
            "Norm of zero multivector should be zero");
    }

    [Test]
    public void NormSquared_RelationToScalarProduct()
    {
        // ||a||² = a · a for vectors
        var a = _random.GetVector();

        var normSquared = a.NormSquared().ScalarValue;
        var scalarProduct = a.Sp(a).ScalarValue;

        TestUtils.AssertDoubleEquals(normSquared, scalarProduct, Tolerance,
            "Norm squared should equal scalar product for vectors");
    }

    [Test]
    public void NormSquared_RelationToGpReverse()
    {
        // ||a||² = <a * reverse(a)>₀ (scalar part)
        var a = _random.GetMultivector();

        var normSquared = a.NormSquared().ScalarValue;
        var gpReverse = a.Gp(a.Reverse()).GetScalarPart();

        TestUtils.AssertDoubleEquals(normSquared, gpReverse, Tolerance,
            "Norm squared should equal scalar part of a * reverse(a)");
    }

    [Test]
    public void Norm_ScalarMultiplication()
    {
        // ||k * a|| = |k| * ||a||
        var a = _random.GetMultivector();
        var k = -2.5;

        var leftNorm = (a * k).Norm().ScalarValue;
        var rightNorm = Math.Abs(k) * a.Norm().ScalarValue;

        TestUtils.AssertDoubleEquals(leftNorm, rightNorm, Tolerance,
            "Norm should scale with absolute value of scalar");
    }

    [Test]
    public void Norm_TriangleInequality()
    {
        // ||a + b|| ≤ ||a|| + ||b||
        var a = _random.GetVector(); // Use vectors for clearer test
        var b = _random.GetVector();

        var sumNorm = (a + b).Norm().ScalarValue;
        var normSum = a.Norm().ScalarValue + b.Norm().ScalarValue;

        Assert.That(sumNorm <= normSum + Tolerance,
            $"Triangle inequality violated: ||a+b|| = {sumNorm}, ||a|| + ||b|| = {normSum}");
    }

    #endregion

    #region ENorm (Euclidean Norm) Tests

    [Test]
    public void ENorm_MatchesNorm_ForEuclideanProcessor()
    {
        // For Euclidean processor, ENorm should match Norm
        var a = _random.GetVector();

        var norm = a.Norm().ScalarValue;
        var enorm = a.ENorm().ScalarValue;

        TestUtils.AssertDoubleEquals(norm, enorm, Tolerance,
            "ENorm should match Norm for Euclidean processor");
    }

    [Test]
    public void ENorm_NormalizationProducesUnitVector()
    {
        var a = _random.GetVector();
        var normalized = a.DivideByENorm();

        var norm = normalized.ENorm().ScalarValue;

        TestUtils.AssertDoubleEquals(1.0, norm, Tolerance,
            "Normalized vector should have unit norm");
    }

    #endregion

    #region Inverse Tests

    [Test]
    public void Inverse_RightInverse_Scalar()
    {
        // For scalar: a * a⁻¹ = 1
        var scalar = _processor.Scalar(3.14);
        var inverse = scalar.Inverse();

        var product = scalar.Gp(inverse);
        var expected = _processor.Scalar(1.0);

        TestUtils.AssertMultivectorEquals(product, expected, Tolerance,
            "Scalar times its inverse should equal 1");
    }

    [Test]
    public void Inverse_RightInverse_Vector()
    {
        // For vector in Euclidean space: a * a⁻¹ = 1
        var a = _random.GetVector();
        var inverse = a.Inverse();

        var product = a.Gp(inverse);
        var scalarPart = product.GetScalarPart();

        TestUtils.AssertDoubleEquals(1.0, scalarPart, Tolerance,
            "Vector times its inverse should have scalar part 1");
    }

    [Test]
    public void Inverse_LeftInverse_Vector()
    {
        // For vector: a⁻¹ * a = 1
        var a = _random.GetVector();
        var inverse = a.Inverse();

        var product = inverse.Gp(a);
        var scalarPart = product.GetScalarPart();

        TestUtils.AssertDoubleEquals(1.0, scalarPart, Tolerance,
            "Inverse times vector should have scalar part 1");
    }

    [Test]
    public void Inverse_ProductRule()
    {
        // (a * b)⁻¹ = b⁻¹ * a⁻¹ for invertible elements
        var a = _random.GetVector().DivideByENorm(); // Normalize for better numerical stability
        var b = _random.GetVector().DivideByENorm();

        var abInverse = a.Gp(b).Inverse();
        var productInverses = b.Inverse().Gp(a.Inverse());

        TestUtils.AssertMultivectorEquals(abInverse, productInverses, Tolerance * 10,
            "Inverse should reverse product order");
    }

    [Test]
    public void Inverse_DoubleInverse()
    {
        // (a⁻¹)⁻¹ = a
        var a = _random.GetVector();
        var inverse = a.Inverse();
        var doubleInverse = inverse.Inverse();

        TestUtils.AssertMultivectorEquals(a, doubleInverse, Tolerance * 10,
            "Double inverse should return original");
    }

    #endregion

    #region Dual Tests - COMMENTED OUT (Dual API needs vSpaceDimensions parameter)

    // NOTE: Dual() operation requires vSpaceDimensions parameter
    // These tests are disabled until proper API usage is determined

    // [Test]
    // public void Dual_DoubleDual_Scalar()
    // {
    //     var scalar = _processor.Scalar(3.14);
    //     var dual = scalar.Dual(VSpaceDimensions);
    //     var doubleDual = dual.Dual(VSpaceDimensions);
    //     // Test implementation depends on dual API
    // }

    #endregion

    #region Negative Tests

    [Test]
    public void Negative_Involution()
    {
        // -(-a) = a
        var a = _random.GetMultivector();
        var negative = a.Negative();
        var doubleNegative = negative.Negative();

        TestUtils.AssertMultivectorEquals(a, doubleNegative, Tolerance,
            "Double negative should return original");
    }

    [Test]
    public void Negative_Addition()
    {
        // a + (-a) = 0
        var a = _random.GetMultivector();
        var negative = a.Negative();
        var sum = a + negative;

        TestUtils.AssertNearZero(sum, Tolerance,
            "Multivector plus its negative should be zero");
    }

    [Test]
    public void Negative_DistributivityOverAddition()
    {
        // -(a + b) = (-a) + (-b)
        var a = _random.GetMultivector();
        var b = _random.GetMultivector();

        var left = (a + b).Negative();
        var right = a.Negative() + b.Negative();

        TestUtils.AssertMultivectorEquals(left, right, Tolerance,
            "Negative should distribute over addition");
    }

    #endregion

    #region Combined Operations Tests

    [Test]
    public void CombinedOperations_ReverseAndInvolution()
    {
        // Test consistency between operations
        var a = _random.GetMultivector();

        var rev_inv = a.Reverse().GradeInvolution();
        var inv_rev = a.GradeInvolution().Reverse();
        var clifford = a.CliffordConjugate();

        TestUtils.AssertMultivectorEquals(rev_inv, clifford, Tolerance,
            "Reverse then involution should equal Clifford conjugate");
        TestUtils.AssertMultivectorEquals(inv_rev, clifford, Tolerance,
            "Involution then reverse should equal Clifford conjugate");
    }

    [Test]
    public void CombinedOperations_NormalizationPreservesDirection()
    {
        // Normalized vector should be proportional to original
        var a = _random.GetVector();
        var normalized = a.DivideByENorm();

        TestUtils.AssertProportional(a, normalized, Tolerance,
            "Normalized vector should be proportional to original");
    }

    #endregion
}
