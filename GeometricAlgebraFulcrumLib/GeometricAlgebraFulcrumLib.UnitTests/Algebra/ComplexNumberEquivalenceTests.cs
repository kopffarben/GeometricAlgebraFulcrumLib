using System;
using System.Numerics;
using GeometricAlgebraFulcrumLib.Algebra.ComplexAlgebra;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Unit tests for ComplexNumber&lt;T&gt; equivalence - Module 2, Task 2.1 of deduplication roadmap.
/// Tests ensure Generic&lt;double&gt; ComplexNumber produces equivalent results to System.Numerics.Complex.
/// ComplexNumber&lt;T&gt; is fully generic and works with any scalar type through IScalarProcessor&lt;T&gt;.
/// </summary>
[TestFixture]
public class ComplexNumberEquivalenceTests
{
    private IScalarProcessor<double> _scalarProcessor = null!;
    private const double Tolerance = 1e-12;

    [SetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    /// <summary>
    /// Helper method to assert two complex numbers are equivalent within tolerance.
    /// </summary>
    private void AssertComplexEquivalent(Complex expected, ComplexNumber<double> actual, string message)
    {
        Assert.That(actual.RealValue, Is.EqualTo(expected.Real).Within(Tolerance),
            $"{message}: Real part should match");
        Assert.That(actual.ImaginaryValue, Is.EqualTo(expected.Imaginary).Within(Tolerance),
            $"{message}: Imaginary part should match");
    }

    [Test]
    public void ComplexNumber_ZeroConstant_ShouldMatchSystemComplex()
    {
        // Arrange & Act
        var genericZero = _scalarProcessor.CreateComplexNumberZero();
        var systemZero = Complex.Zero;

        // Assert
        AssertComplexEquivalent(systemZero, genericZero, "Zero constant");
    }

    [Test]
    public void ComplexNumber_OneConstant_ShouldMatchSystemComplex()
    {
        // Arrange & Act
        var genericOne = _scalarProcessor.CreateComplexNumberOne();
        var systemOne = Complex.One;

        // Assert
        AssertComplexEquivalent(systemOne, genericOne, "One constant");
    }

    [Test]
    public void ComplexNumber_ImaginaryUnitConstant_ShouldMatchSystemComplex()
    {
        // Arrange & Act
        var genericI = _scalarProcessor.CreateComplexNumberI();
        var systemI = Complex.ImaginaryOne;

        // Assert
        AssertComplexEquivalent(systemI, genericI, "Imaginary unit");
    }

    [Test]
    public void ComplexNumber_Addition_ShouldProduceEquivalentResults()
    {
        // Arrange
        var c1Generic = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var c2Generic = _scalarProcessor.CreateComplexNumber(1.0, 2.0);

        var c1System = new Complex(3.0, 4.0);
        var c2System = new Complex(1.0, 2.0);

        // Act
        var resultGeneric = c1Generic + c2Generic;
        var resultSystem = c1System + c2System;

        // Assert
        AssertComplexEquivalent(resultSystem, resultGeneric, "Addition");
    }

    [Test]
    public void ComplexNumber_Subtraction_ShouldProduceEquivalentResults()
    {
        // Arrange
        var c1Generic = _scalarProcessor.CreateComplexNumber(5.0, 7.0);
        var c2Generic = _scalarProcessor.CreateComplexNumber(2.0, 3.0);

        var c1System = new Complex(5.0, 7.0);
        var c2System = new Complex(2.0, 3.0);

        // Act
        var resultGeneric = c1Generic - c2Generic;
        var resultSystem = c1System - c2System;

        // Assert
        AssertComplexEquivalent(resultSystem, resultGeneric, "Subtraction");
    }

    [Test]
    public void ComplexNumber_Multiplication_ShouldProduceEquivalentResults()
    {
        // Arrange
        var c1Generic = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var c2Generic = _scalarProcessor.CreateComplexNumber(1.0, 2.0);

        var c1System = new Complex(3.0, 4.0);
        var c2System = new Complex(1.0, 2.0);

        // Act
        var resultGeneric = c1Generic * c2Generic;
        var resultSystem = c1System * c2System;

        // Assert
        AssertComplexEquivalent(resultSystem, resultGeneric, "Multiplication");
    }

    [Test]
    public void ComplexNumber_Division_ShouldProduceEquivalentResults()
    {
        // Arrange
        var c1Generic = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var c2Generic = _scalarProcessor.CreateComplexNumber(1.0, 2.0);

        var c1System = new Complex(3.0, 4.0);
        var c2System = new Complex(1.0, 2.0);

        // Act
        var resultGeneric = c1Generic / c2Generic;
        var resultSystem = c1System / c2System;

        // Assert
        AssertComplexEquivalent(resultSystem, resultGeneric, "Division");
    }

    [Test]
    public void ComplexNumber_Negation_ShouldProduceEquivalentResults()
    {
        // Arrange
        var cGeneric = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var cSystem = new Complex(3.0, 4.0);

        // Act
        var resultGeneric = -cGeneric;
        var resultSystem = -cSystem;

        // Assert
        AssertComplexEquivalent(resultSystem, resultGeneric, "Negation");
    }

    [Test]
    public void ComplexNumber_Conjugate_ShouldProduceEquivalentResults()
    {
        // Arrange
        var cGeneric = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var cSystem = new Complex(3.0, 4.0);

        // Act
        var resultGeneric = cGeneric.Conjugate();
        var resultSystem = Complex.Conjugate(cSystem);

        // Assert
        AssertComplexEquivalent(resultSystem, resultGeneric, "Conjugate");
    }

    [Test]
    public void ComplexNumber_Magnitude_ShouldMatchSystemComplex()
    {
        // Arrange
        var cGeneric = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var cSystem = new Complex(3.0, 4.0);

        // Act
        var magnitudeGeneric = cGeneric.MagnitudeValue;
        var magnitudeSystem = cSystem.Magnitude;

        // Assert
        Assert.That(magnitudeGeneric, Is.EqualTo(magnitudeSystem).Within(Tolerance),
            "Magnitude should match");
    }

    [Test]
    public void ComplexNumber_MagnitudeSquared_ShouldMatchSystemComplex()
    {
        // Arrange
        var cGeneric = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var cSystem = new Complex(3.0, 4.0);

        // Act
        var magSquaredGeneric = cGeneric.MagnitudeSquaredValue;
        var magSquaredSystem = cSystem.Magnitude * cSystem.Magnitude;

        // Assert
        Assert.That(magSquaredGeneric, Is.EqualTo(magSquaredSystem).Within(Tolerance),
            "Magnitude squared should match");
    }

    [Test]
    public void ComplexNumber_Phase_ShouldMatchSystemComplex()
    {
        // Arrange
        var cGeneric = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var cSystem = new Complex(3.0, 4.0);

        // Act
        var phaseGeneric = cGeneric.Phase.Radians.ScalarValue;
        var phaseSystem = cSystem.Phase;

        // Assert
        Assert.That(phaseGeneric, Is.EqualTo(phaseSystem).Within(Tolerance),
            "Phase should match");
    }

    [Test]
    public void ComplexNumber_Inverse_ShouldProduceEquivalentResults()
    {
        // Arrange
        var cGeneric = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var cSystem = new Complex(3.0, 4.0);

        // Act
        var resultGeneric = cGeneric.Inverse();
        var resultSystem = 1.0 / cSystem;

        // Assert
        AssertComplexEquivalent(resultSystem, resultGeneric, "Inverse");
    }

    [Test]
    public void ComplexNumber_Square_ShouldProduceEquivalentResults()
    {
        // Arrange
        var cGeneric = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var cSystem = new Complex(3.0, 4.0);

        // Act
        var resultGeneric = cGeneric.Square();
        var resultSystem = cSystem * cSystem;

        // Assert
        AssertComplexEquivalent(resultSystem, resultGeneric, "Square");
    }

    [Test]
    public void ComplexNumber_AddScalar_ShouldProduceEquivalentResults()
    {
        // Arrange
        var cGeneric = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var cSystem = new Complex(3.0, 4.0);
        var scalar = 5.0;

        // Act
        var resultGeneric = cGeneric + scalar;
        var resultSystem = cSystem + scalar;

        // Assert
        AssertComplexEquivalent(resultSystem, resultGeneric, "Add scalar");
    }

    [Test]
    public void ComplexNumber_SubtractScalar_ShouldProduceEquivalentResults()
    {
        // Arrange
        var cGeneric = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var cSystem = new Complex(3.0, 4.0);
        var scalar = 5.0;

        // Act
        var resultGeneric = cGeneric - scalar;
        var resultSystem = cSystem - scalar;

        // Assert
        AssertComplexEquivalent(resultSystem, resultGeneric, "Subtract scalar");
    }

    [Test]
    public void ComplexNumber_MultiplyByScalar_ShouldProduceEquivalentResults()
    {
        // Arrange
        var cGeneric = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var cSystem = new Complex(3.0, 4.0);
        var scalar = 2.5;

        // Act
        var resultGeneric = cGeneric * scalar;
        var resultSystem = cSystem * scalar;

        // Assert
        AssertComplexEquivalent(resultSystem, resultGeneric, "Multiply by scalar");
    }

    [Test]
    public void ComplexNumber_DivideByScalar_ShouldProduceEquivalentResults()
    {
        // Arrange
        var cGeneric = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var cSystem = new Complex(3.0, 4.0);
        var scalar = 2.0;

        // Act
        var resultGeneric = cGeneric / scalar;
        var resultSystem = cSystem / scalar;

        // Assert
        AssertComplexEquivalent(resultSystem, resultGeneric, "Divide by scalar");
    }

    [Test]
    public void ComplexNumber_CreateFromPolar_ShouldProduceEquivalentResults()
    {
        // Arrange
        var magnitude = 5.0;
        var phase = Math.PI / 3.0;

        // Act
        var resultGeneric = _scalarProcessor.CreateComplexNumberPolar(magnitude, phase);
        var resultSystem = Complex.FromPolarCoordinates(magnitude, phase);

        // Assert
        AssertComplexEquivalent(resultSystem, resultGeneric, "Polar coordinates");
    }

    [Test]
    public void ComplexNumber_IsZero_ShouldDetectZero()
    {
        // Arrange
        var zero = _scalarProcessor.CreateComplexNumberZero();
        var nonZero = _scalarProcessor.CreateComplexNumber(1.0, 0.0);

        // Act & Assert
        Assert.That(zero.IsZero(), Is.True, "Zero should be detected as zero");
        Assert.That(nonZero.IsZero(), Is.False, "Non-zero should not be detected as zero");
    }

    [Test]
    public void ComplexNumber_RealNumber_ShouldHaveZeroImaginaryPart()
    {
        // Arrange & Act
        var realGeneric = _scalarProcessor.CreateComplexNumberReal(7.5);
        var realSystem = new Complex(7.5, 0.0);

        // Assert
        AssertComplexEquivalent(realSystem, realGeneric, "Real number");
        Assert.That(realGeneric.ImaginaryValue, Is.EqualTo(0.0).Within(Tolerance),
            "Real number should have zero imaginary part");
    }

    [Test]
    public void ComplexNumber_ImaginaryNumber_ShouldHaveZeroRealPart()
    {
        // Arrange & Act
        var imaginaryGeneric = _scalarProcessor.CreateComplexNumberImaginary(3.5);
        var imaginarySystem = new Complex(0.0, 3.5);

        // Assert
        AssertComplexEquivalent(imaginarySystem, imaginaryGeneric, "Imaginary number");
        Assert.That(imaginaryGeneric.RealValue, Is.EqualTo(0.0).Within(Tolerance),
            "Imaginary number should have zero real part");
    }

    [Test]
    public void ComplexNumber_LogE_ShouldProduceEquivalentResults()
    {
        // Arrange
        var cGeneric = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var cSystem = new Complex(3.0, 4.0);

        // Act
        var resultGeneric = cGeneric.LogE();
        var resultSystem = Complex.Log(cSystem);

        // Assert
        AssertComplexEquivalent(resultSystem, resultGeneric, "Natural logarithm");
    }

    [Test]
    public void ComplexNumber_MultiplicationIsCommutative_ShouldBeTrue()
    {
        // Arrange
        var c1 = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var c2 = _scalarProcessor.CreateComplexNumber(1.0, 2.0);

        // Act
        var result1 = c1 * c2;
        var result2 = c2 * c1;

        // Assert
        AssertComplexEquivalent(
            new Complex(result1.RealValue, result1.ImaginaryValue),
            result2,
            "Multiplication should be commutative"
        );
    }

    [Test]
    public void ComplexNumber_AdditionIsCommutative_ShouldBeTrue()
    {
        // Arrange
        var c1 = _scalarProcessor.CreateComplexNumber(3.0, 4.0);
        var c2 = _scalarProcessor.CreateComplexNumber(1.0, 2.0);

        // Act
        var result1 = c1 + c2;
        var result2 = c2 + c1;

        // Assert
        AssertComplexEquivalent(
            new Complex(result1.RealValue, result1.ImaginaryValue),
            result2,
            "Addition should be commutative"
        );
    }

    [Test]
    public void ComplexNumber_MultiplicationByConjugate_ShouldBeReal()
    {
        // Arrange
        var c = _scalarProcessor.CreateComplexNumber(3.0, 4.0);

        // Act
        var result = c * c.Conjugate();

        // Assert
        Assert.That(result.ImaginaryValue, Is.EqualTo(0.0).Within(Tolerance),
            "Multiplication by conjugate should produce real number");
        Assert.That(result.RealValue, Is.EqualTo(c.MagnitudeSquaredValue).Within(Tolerance),
            "Multiplication by conjugate should equal magnitude squared");
    }

    [Test]
    public void ComplexNumber_DivisionByItself_ShouldBeOne()
    {
        // Arrange
        var c = _scalarProcessor.CreateComplexNumber(3.0, 4.0);

        // Act
        var result = c / c;

        // Assert
        AssertComplexEquivalent(Complex.One, result, "Division by itself should be one");
    }

    [Test]
    public void ComplexNumber_NegativeOfNegative_ShouldBeOriginal()
    {
        // Arrange
        var c = _scalarProcessor.CreateComplexNumber(3.0, 4.0);

        // Act
        var result = -(-c);

        // Assert
        AssertComplexEquivalent(new Complex(3.0, 4.0), result, "Negative of negative");
    }

    [Test]
    public void ComplexNumber_ConjugateOfConjugate_ShouldBeOriginal()
    {
        // Arrange
        var c = _scalarProcessor.CreateComplexNumber(3.0, 4.0);

        // Act
        var result = c.Conjugate().Conjugate();

        // Assert
        AssertComplexEquivalent(new Complex(3.0, 4.0), result, "Conjugate of conjugate");
    }

    [Test]
    public void ComplexNumber_InverseOfInverse_ShouldBeOriginal()
    {
        // Arrange
        var c = _scalarProcessor.CreateComplexNumber(3.0, 4.0);

        // Act
        var result = c.Inverse().Inverse();

        // Assert
        AssertComplexEquivalent(new Complex(3.0, 4.0), result, "Inverse of inverse");
    }
}
