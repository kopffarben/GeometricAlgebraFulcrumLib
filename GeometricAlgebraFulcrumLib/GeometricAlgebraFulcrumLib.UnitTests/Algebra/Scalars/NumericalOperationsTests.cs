using System;
using AngouriMath;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float32;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.MetaProgramming.Context.Processors;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Scalars;

[TestFixture]
public class NumericalOperationsTests
{
    private const double Float64Tolerance = 1e-6;
    private const float Float32Tolerance = 1e-4f;

    #region Float64 Tests (MathNet Backend)

    [Test]
    public void Float64_Differentiate_Polynomial_ReturnsCorrectDerivative()
    {
        // Test: f(x) = x^2 + 2x + 1
        // Expected: f'(x) = 2x + 2
        // At x = 3: f'(3) = 8

        var processor = ScalarProcessorOfFloat64.Instance;
        var ops = processor.NumericalOperations;

        Scalar<double> TestFunction(Scalar<double> x)
        {
            return processor.Times(x.ScalarValue, x.ScalarValue)
                .Add(processor.Times(2.0, x.ScalarValue))
                .Add(processor.ScalarFromNumber(1.0));
        }

        var point = processor.ScalarFromNumber(3.0);
        var derivative = ops.Differentiate(TestFunction, point);

        Assert.That(derivative.ScalarValue, Is.EqualTo(8.0).Within(Float64Tolerance),
            $"Expected f'(3) = 8.0, got {derivative.ScalarValue}");
    }

    [Test]
    public void Float64_Differentiate_TrigFunction_ReturnsCorrectDerivative()
    {
        // Test: f(x) = sin(x)
        // Expected: f'(x) = cos(x)
        // At x = π/2: f'(π/2) = cos(π/2) ≈ 0

        var processor = ScalarProcessorOfFloat64.Instance;
        var ops = processor.NumericalOperations;

        Scalar<double> TestFunction(Scalar<double> x)
        {
            return processor.Sin(x.ScalarValue);
        }

        var point = processor.PiOver2;
        var derivative = ops.Differentiate(TestFunction, point);

        Assert.That(derivative.ScalarValue, Is.EqualTo(0.0).Within(Float64Tolerance),
            $"Expected f'(π/2) ≈ 0.0, got {derivative.ScalarValue}");
    }

    [Test]
    public void Float64_Differentiate_ExponentialFunction_ReturnsCorrectDerivative()
    {
        // Test: f(x) = e^x
        // Expected: f'(x) = e^x
        // At x = 1: f'(1) = e ≈ 2.71828

        var processor = ScalarProcessorOfFloat64.Instance;
        var ops = processor.NumericalOperations;

        Scalar<double> TestFunction(Scalar<double> x)
        {
            return processor.Exp(x.ScalarValue);
        }

        var point = processor.ScalarFromNumber(1.0);
        var derivative = ops.Differentiate(TestFunction, point);
        var expected = Math.E;

        Assert.That(derivative.ScalarValue, Is.EqualTo(expected).Within(Float64Tolerance),
            $"Expected f'(1) = e ≈ {expected}, got {derivative.ScalarValue}");
    }

    [Test]
    public void Float64_Differentiate2_Polynomial_ReturnsCorrectSecondDerivative()
    {
        // Test: f(x) = x^3
        // Expected: f''(x) = 6x
        // At x = 2: f''(2) = 12

        var processor = ScalarProcessorOfFloat64.Instance;
        var ops = processor.NumericalOperations;

        Scalar<double> TestFunction(Scalar<double> x)
        {
            return processor.Times(
                processor.Times(x.ScalarValue, x.ScalarValue).ScalarValue,
                x.ScalarValue
            );
        }

        var point = processor.ScalarFromNumber(2.0);
        var derivative2 = ops.Differentiate2(TestFunction, point);

        Assert.That(derivative2.ScalarValue, Is.EqualTo(12.0).Within(Float64Tolerance),
            $"Expected f''(2) = 12.0, got {derivative2.ScalarValue}");
    }

    [Test]
    public void Float64_Differentiate2_TrigFunction_ReturnsCorrectSecondDerivative()
    {
        // Test: f(x) = sin(x)
        // Expected: f''(x) = -sin(x)
        // At x = π/2: f''(π/2) = -1

        var processor = ScalarProcessorOfFloat64.Instance;
        var ops = processor.NumericalOperations;

        Scalar<double> TestFunction(Scalar<double> x)
        {
            return processor.Sin(x.ScalarValue);
        }

        var point = processor.PiOver2;
        var derivative2 = ops.Differentiate2(TestFunction, point);

        Assert.That(derivative2.ScalarValue, Is.EqualTo(-1.0).Within(Float64Tolerance),
            $"Expected f''(π/2) = -1.0, got {derivative2.ScalarValue}");
    }

    [Test]
    public void Float64_Differentiate_AtZero_ReturnsCorrectDerivative()
    {
        // Test: f(x) = x^2
        // Expected: f'(x) = 2x
        // At x = 0: f'(0) = 0

        var processor = ScalarProcessorOfFloat64.Instance;
        var ops = processor.NumericalOperations;

        Scalar<double> TestFunction(Scalar<double> x)
        {
            return processor.Times(x.ScalarValue, x.ScalarValue);
        }

        var point = processor.Zero;
        var derivative = ops.Differentiate(TestFunction, point);

        Assert.That(derivative.ScalarValue, Is.EqualTo(0.0).Within(Float64Tolerance),
            $"Expected f'(0) = 0.0, got {derivative.ScalarValue}");
    }

    [Test]
    public void Float64_Differentiate_ConstantFunction_ReturnsZero()
    {
        // Test: f(x) = 5
        // Expected: f'(x) = 0

        var processor = ScalarProcessorOfFloat64.Instance;
        var ops = processor.NumericalOperations;

        Scalar<double> TestFunction(Scalar<double> x)
        {
            return processor.ScalarFromNumber(5.0);
        }

        var point = processor.ScalarFromNumber(3.14);
        var derivative = ops.Differentiate(TestFunction, point);

        Assert.That(derivative.ScalarValue, Is.EqualTo(0.0).Within(Float64Tolerance),
            $"Expected constant function derivative = 0.0, got {derivative.ScalarValue}");
    }

    [Test]
    public void Float64_Differentiate_LogFunction_ReturnsCorrectDerivative()
    {
        // Test: f(x) = ln(x)
        // Expected: f'(x) = 1/x
        // At x = 2: f'(2) = 0.5

        var processor = ScalarProcessorOfFloat64.Instance;
        var ops = processor.NumericalOperations;

        Scalar<double> TestFunction(Scalar<double> x)
        {
            return processor.LogE(x.ScalarValue);
        }

        var point = processor.ScalarFromNumber(2.0);
        var derivative = ops.Differentiate(TestFunction, point);

        Assert.That(derivative.ScalarValue, Is.EqualTo(0.5).Within(Float64Tolerance),
            $"Expected f'(2) = 0.5, got {derivative.ScalarValue}");
    }

    #endregion

    #region Float32 Tests (MathNet Backend)

    [Test]
    public void Float32_Differentiate_Polynomial_ReturnsCorrectDerivative()
    {
        // Test: f(x) = x^2 + 2x + 1
        // At x = 3: f'(3) = 8

        var processor = ScalarProcessorOfFloat32.Instance;
        var ops = processor.NumericalOperations;

        Scalar<float> TestFunction(Scalar<float> x)
        {
            return processor.Times(x.ScalarValue, x.ScalarValue)
                .Add(processor.Times(2.0f, x.ScalarValue))
                .Add(processor.ScalarFromNumber(1.0f));
        }

        var point = processor.ScalarFromNumber(3.0f);
        var derivative = ops.Differentiate(TestFunction, point);

        Assert.That(derivative.ScalarValue, Is.EqualTo(8.0f).Within(Float32Tolerance),
            $"Expected f'(3) = 8.0, got {derivative.ScalarValue}");
    }

    [Test]
    public void Float32_Differentiate_TrigFunction_ReturnsCorrectDerivative()
    {
        // Test: f(x) = cos(x)
        // Expected: f'(x) = -sin(x)
        // At x = 0: f'(0) = 0

        var processor = ScalarProcessorOfFloat32.Instance;
        var ops = processor.NumericalOperations;

        Scalar<float> TestFunction(Scalar<float> x)
        {
            return processor.Cos(x.ScalarValue);
        }

        var point = processor.Zero;
        var derivative = ops.Differentiate(TestFunction, point);

        Assert.That(derivative.ScalarValue, Is.EqualTo(0.0f).Within(Float32Tolerance),
            $"Expected f'(0) ≈ 0.0, got {derivative.ScalarValue}");
    }

    [Test]
    public void Float32_Differentiate2_Polynomial_ReturnsCorrectSecondDerivative()
    {
        // Test: f(x) = x^4
        // Expected: f''(x) = 12x^2
        // At x = 2: f''(2) = 48

        var processor = ScalarProcessorOfFloat32.Instance;
        var ops = processor.NumericalOperations;

        Scalar<float> TestFunction(Scalar<float> x)
        {
            var x2 = processor.Times(x.ScalarValue, x.ScalarValue);
            return processor.Times(x2.ScalarValue, x2.ScalarValue);
        }

        var point = processor.ScalarFromNumber(2.0f);
        var derivative2 = ops.Differentiate2(TestFunction, point);

        Assert.That(derivative2.ScalarValue, Is.EqualTo(48.0f).Within(Float32Tolerance),
            $"Expected f''(2) = 48.0, got {derivative2.ScalarValue}");
    }

    [Test]
    public void Float32_Differentiate_ExponentialFunction_ReturnsCorrectDerivative()
    {
        // Test: f(x) = e^x
        // At x = 0: f'(0) = 1

        var processor = ScalarProcessorOfFloat32.Instance;
        var ops = processor.NumericalOperations;

        Scalar<float> TestFunction(Scalar<float> x)
        {
            return processor.Exp(x.ScalarValue);
        }

        var point = processor.Zero;
        var derivative = ops.Differentiate(TestFunction, point);

        Assert.That(derivative.ScalarValue, Is.EqualTo(1.0f).Within(Float32Tolerance),
            $"Expected f'(0) = 1.0, got {derivative.ScalarValue}");
    }

    [Test]
    public void Float32_Differentiate_SquareRoot_ReturnsCorrectDerivative()
    {
        // Test: f(x) = sqrt(x)
        // Expected: f'(x) = 1/(2*sqrt(x))
        // At x = 4: f'(4) = 0.25

        var processor = ScalarProcessorOfFloat32.Instance;
        var ops = processor.NumericalOperations;

        Scalar<float> TestFunction(Scalar<float> x)
        {
            return processor.Sqrt(x.ScalarValue);
        }

        var point = processor.ScalarFromNumber(4.0f);
        var derivative = ops.Differentiate(TestFunction, point);

        Assert.That(derivative.ScalarValue, Is.EqualTo(0.25f).Within(Float32Tolerance),
            $"Expected f'(4) = 0.25, got {derivative.ScalarValue}");
    }

    #endregion

    #region AngouriMath Symbolic Tests

    [Test]
    public void AngouriMath_Differentiate_Polynomial_ReturnsExactSymbolicDerivative()
    {
        // Test: f(x) = x^2
        // Expected: f'(x) = 2*x
        // At x = 3: f'(3) = 6 (EXACT)

        var processor = ScalarProcessorOfAngouriMathEntity.Instance;
        var ops = processor.NumericalOperations;

        Scalar<Entity> TestFunction(Scalar<Entity> x)
        {
            return processor.Times(x.ScalarValue, x.ScalarValue);
        }

        var point = processor.ScalarFromNumber(3.0);
        var derivative = ops.Differentiate(TestFunction, point);

        // AngouriMath gives EXACT results
        var expected = 6.0;
        var actual = processor.ToFloat64(derivative.ScalarValue);

        Assert.That(actual, Is.EqualTo(expected).Within(1e-12),
            $"Expected EXACT f'(3) = {expected}, got {actual}");
    }

    [Test]
    public void AngouriMath_Differentiate_TrigFunction_ReturnsExactSymbolicDerivative()
    {
        // Test: f(x) = sin(x)
        // Expected: f'(x) = cos(x)
        // At x = 0: f'(0) = cos(0) = 1 (EXACT)

        var processor = ScalarProcessorOfAngouriMathEntity.Instance;
        var ops = processor.NumericalOperations;

        Scalar<Entity> TestFunction(Scalar<Entity> x)
        {
            return processor.Sin(x.ScalarValue);
        }

        var point = processor.Zero;
        var derivative = ops.Differentiate(TestFunction, point);

        var expected = 1.0;
        var actual = processor.ToFloat64(derivative.ScalarValue);

        Assert.That(actual, Is.EqualTo(expected).Within(1e-12),
            $"Expected EXACT f'(0) = {expected}, got {actual}");
    }

    [Test]
    public void AngouriMath_Differentiate2_Polynomial_ReturnsExactSecondDerivative()
    {
        // Test: f(x) = x^3
        // Expected: f''(x) = 6x
        // At x = 2: f''(2) = 12 (EXACT)

        var processor = ScalarProcessorOfAngouriMathEntity.Instance;
        var ops = processor.NumericalOperations;

        Scalar<Entity> TestFunction(Scalar<Entity> x)
        {
            return processor.Times(
                processor.Times(x.ScalarValue, x.ScalarValue).ScalarValue,
                x.ScalarValue
            );
        }

        var point = processor.ScalarFromNumber(2.0);
        var derivative2 = ops.Differentiate2(TestFunction, point);

        var expected = 12.0;
        var actual = processor.ToFloat64(derivative2.ScalarValue);

        Assert.That(actual, Is.EqualTo(expected).Within(1e-12),
            $"Expected EXACT f''(2) = {expected}, got {actual}");
    }

    [Test]
    public void AngouriMath_Differentiate_ExponentialFunction_ReturnsSymbolicDerivative()
    {
        // Test: f(x) = e^x
        // Expected: f'(x) = e^x
        // At x = 1: f'(1) = e

        var processor = ScalarProcessorOfAngouriMathEntity.Instance;
        var ops = processor.NumericalOperations;

        Scalar<Entity> TestFunction(Scalar<Entity> x)
        {
            return processor.Exp(x.ScalarValue);
        }

        var point = processor.ScalarFromNumber(1.0);
        var derivative = ops.Differentiate(TestFunction, point);

        var expected = Math.E;
        var actual = processor.ToFloat64(derivative.ScalarValue);

        Assert.That(actual, Is.EqualTo(expected).Within(1e-10),
            $"Expected f'(1) = e ≈ {expected}, got {actual}");
    }

    [Test]
    public void AngouriMath_Differentiate_ComplexPolynomial_ReturnsExactDerivative()
    {
        // Test: f(x) = 3x^4 - 2x^3 + 5x^2 - 7x + 11
        // Expected: f'(x) = 12x^3 - 6x^2 + 10x - 7
        // At x = 1: f'(1) = 12 - 6 + 10 - 7 = 9 (EXACT)

        var processor = ScalarProcessorOfAngouriMathEntity.Instance;
        var ops = processor.NumericalOperations;

        Scalar<Entity> TestFunction(Scalar<Entity> x)
        {
            var x2 = processor.Times(x.ScalarValue, x.ScalarValue);
            var x3 = processor.Times(x2.ScalarValue, x.ScalarValue);
            var x4 = processor.Times(x3.ScalarValue, x.ScalarValue);

            return processor.Add(
                processor.Times(3.0, x4.ScalarValue).ScalarValue,
                processor.Negative(processor.Times(2.0, x3.ScalarValue).ScalarValue).ScalarValue
            ).Add(
                processor.Times(5.0, x2.ScalarValue)
            ).Subtract(
                processor.Times(7.0, x.ScalarValue)
            ).Add(
                processor.ScalarFromNumber(11.0)
            );
        }

        var point = processor.ScalarFromNumber(1.0);
        var derivative = ops.Differentiate(TestFunction, point);

        var expected = 9.0;
        var actual = processor.ToFloat64(derivative.ScalarValue);

        Assert.That(actual, Is.EqualTo(expected).Within(1e-12),
            $"Expected EXACT f'(1) = {expected}, got {actual}");
    }

    [Test]
    public void AngouriMath_Integrate_Polynomial_ReturnsExactIntegral()
    {
        // Test: ∫[0,1] x dx
        // Expected: [x^2/2] from 0 to 1 = 0.5 (EXACT)

        var processor = ScalarProcessorOfAngouriMathEntity.Instance;
        var ops = processor.NumericalOperations;

        Scalar<Entity> TestFunction(Scalar<Entity> x)
        {
            return x;
        }

        var a = processor.Zero;
        var b = processor.One;
        var integral = ops.Integrate(TestFunction, a, b);

        Assert.That(integral, Is.Not.Null, "Integration should succeed for polynomials");

        var expected = 0.5;
        var actual = processor.ToFloat64(integral.Value.ScalarValue);

        Assert.That(actual, Is.EqualTo(expected).Within(1e-12),
            $"Expected EXACT ∫x dx = {expected}, got {actual}");
    }

    [Test]
    public void AngouriMath_Integrate_TrigFunction_ReturnsExactIntegral()
    {
        // Test: ∫[0,π] sin(x) dx
        // Expected: [-cos(x)] from 0 to π = -cos(π) - (-cos(0)) = 1 - (-1) = 2 (EXACT)

        var processor = ScalarProcessorOfAngouriMathEntity.Instance;
        var ops = processor.NumericalOperations;

        Scalar<Entity> TestFunction(Scalar<Entity> x)
        {
            return processor.Sin(x.ScalarValue);
        }

        var a = processor.Zero;
        var b = processor.Pi;
        var integral = ops.Integrate(TestFunction, a, b);

        Assert.That(integral, Is.Not.Null, "Integration should succeed for sin(x)");

        var expected = 2.0;
        var actual = processor.ToFloat64(integral.Value.ScalarValue);

        Assert.That(actual, Is.EqualTo(expected).Within(1e-10),
            $"Expected EXACT ∫sin(x) dx = {expected}, got {actual}");
    }

    #endregion

    #region Edge Cases and Error Handling

    [Test]
    public void Float64_Differentiate_NullFunction_ThrowsArgumentNullException()
    {
        var processor = ScalarProcessorOfFloat64.Instance;
        var ops = processor.NumericalOperations;
        var point = processor.ScalarFromNumber(1.0);

        Assert.Throws<ArgumentNullException>(() => ops.Differentiate(null!, point));
    }

    // NOTE: Test removed - Scalar<T> is a value type and cannot be null.
    // Testing null Scalar<T> is impossible and causes compilation errors.

    [Test]
    public void Float32_Differentiate2_NullFunction_ThrowsArgumentNullException()
    {
        var processor = ScalarProcessorOfFloat32.Instance;
        var ops = processor.NumericalOperations;
        var point = processor.ScalarFromNumber(1.0f);

        Assert.Throws<ArgumentNullException>(() => ops.Differentiate2(null!, point));
    }

    [Test]
    public void AngouriMath_Integrate_NullFunction_ThrowsArgumentNullException()
    {
        var processor = ScalarProcessorOfAngouriMathEntity.Instance;
        var ops = processor.NumericalOperations;
        var a = processor.Zero;
        var b = processor.One;

        Assert.Throws<ArgumentNullException>(() => ops.Integrate(null!, a, b));
    }

    #endregion

    #region Backend Comparison Tests

    [Test]
    public void CompareBackends_PolynomialDerivative_AllAgree()
    {
        // Test: f(x) = x^2 at x = 5
        // Expected: f'(5) = 10
        // All three backends should give the same result (within tolerance)

        var float64Processor = ScalarProcessorOfFloat64.Instance;
        var float32Processor = ScalarProcessorOfFloat32.Instance;
        var symbolicProcessor = ScalarProcessorOfAngouriMathEntity.Instance;

        // Float64
        Scalar<double> Float64Function(Scalar<double> x) =>
            float64Processor.Times(x.ScalarValue, x.ScalarValue);
        var float64Result = float64Processor.NumericalOperations
            .Differentiate(Float64Function, float64Processor.ScalarFromNumber(5.0));

        // Float32
        Scalar<float> Float32Function(Scalar<float> x) =>
            float32Processor.Times(x.ScalarValue, x.ScalarValue);
        var float32Result = float32Processor.NumericalOperations
            .Differentiate(Float32Function, float32Processor.ScalarFromNumber(5.0f));

        // Symbolic
        Scalar<Entity> SymbolicFunction(Scalar<Entity> x) =>
            symbolicProcessor.Times(x.ScalarValue, x.ScalarValue);
        var symbolicResult = symbolicProcessor.NumericalOperations
            .Differentiate(SymbolicFunction, symbolicProcessor.ScalarFromNumber(5.0));

        var expected = 10.0;
        var symbolicValue = symbolicProcessor.ToFloat64(symbolicResult.ScalarValue);

        Assert.That(float64Result.ScalarValue, Is.EqualTo(expected).Within(Float64Tolerance),
            $"Float64: Expected {expected}, got {float64Result.ScalarValue}");
        Assert.That((double)float32Result.ScalarValue, Is.EqualTo(expected).Within(Float32Tolerance),
            $"Float32: Expected {expected}, got {float32Result.ScalarValue}");
        Assert.That(symbolicValue, Is.EqualTo(expected).Within(1e-12),
            $"Symbolic: Expected EXACT {expected}, got {symbolicValue}");
    }

    [Test]
    public void CompareBackends_TrigDerivative_AllAgree()
    {
        // Test: f(x) = cos(x) at x = 0
        // Expected: f'(0) = -sin(0) = 0

        var float64Processor = ScalarProcessorOfFloat64.Instance;
        var float32Processor = ScalarProcessorOfFloat32.Instance;
        var symbolicProcessor = ScalarProcessorOfAngouriMathEntity.Instance;

        // Float64
        Scalar<double> Float64Function(Scalar<double> x) =>
            float64Processor.Cos(x.ScalarValue);
        var float64Result = float64Processor.NumericalOperations
            .Differentiate(Float64Function, float64Processor.Zero);

        // Float32
        Scalar<float> Float32Function(Scalar<float> x) =>
            float32Processor.Cos(x.ScalarValue);
        var float32Result = float32Processor.NumericalOperations
            .Differentiate(Float32Function, float32Processor.Zero);

        // Symbolic
        Scalar<Entity> SymbolicFunction(Scalar<Entity> x) =>
            symbolicProcessor.Cos(x.ScalarValue);
        var symbolicResult = symbolicProcessor.NumericalOperations
            .Differentiate(SymbolicFunction, symbolicProcessor.Zero);

        var expected = 0.0;
        var symbolicValue = symbolicProcessor.ToFloat64(symbolicResult.ScalarValue);

        Assert.That(float64Result.ScalarValue, Is.EqualTo(expected).Within(Float64Tolerance));
        Assert.That((double)float32Result.ScalarValue, Is.EqualTo(expected).Within(Float32Tolerance));
        Assert.That(symbolicValue, Is.EqualTo(expected).Within(1e-12));
    }

    #endregion

    #region Integration Status Tests

    [Test]
    public void Float64_Integrate_ReturnsNull_NotYetImplemented()
    {
        // Math.NET integration is not yet implemented (Phase 3)
        var processor = ScalarProcessorOfFloat64.Instance;
        var ops = processor.NumericalOperations;

        Scalar<double> TestFunction(Scalar<double> x) => x;

        var result = ops.Integrate(TestFunction, processor.Zero, processor.One);

        Assert.That(result, Is.Null, "Float64 integration not yet implemented");
    }

    [Test]
    public void Float32_Integrate_ReturnsNull_NotYetImplemented()
    {
        // Math.NET integration is not yet implemented (Phase 3)
        var processor = ScalarProcessorOfFloat32.Instance;
        var ops = processor.NumericalOperations;

        Scalar<float> TestFunction(Scalar<float> x) => x;

        var result = ops.Integrate(TestFunction, processor.Zero, processor.One);

        Assert.That(result, Is.Null, "Float32 integration not yet implemented");
    }

    [Test]
    public void Float64_FindRoot_ReturnsNull_NotYetImplemented()
    {
        // Root finding not yet implemented (Phase 3)
        var processor = ScalarProcessorOfFloat64.Instance;
        var ops = processor.NumericalOperations;

        Scalar<double> TestFunction(Scalar<double> x) => x;

        var result = ops.FindRoot(TestFunction, processor.One);

        Assert.That(result, Is.Null, "Float64 root finding not yet implemented");
    }

    [Test]
    public void AngouriMath_FindRoot_ReturnsNull_NotYetImplemented()
    {
        // Symbolic root finding not yet implemented (Phase 3)
        var processor = ScalarProcessorOfAngouriMathEntity.Instance;
        var ops = processor.NumericalOperations;

        Scalar<Entity> TestFunction(Scalar<Entity> x) => x;

        var result = ops.FindRoot(TestFunction, processor.One);

        Assert.That(result, Is.Null, "AngouriMath root finding not yet implemented");
    }

    #endregion
}
