using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public sealed class ComputedScalarSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestComputedSignal_QuadraticFunction()
    {
        // Test with quadratic function: f(t) = t²
        Func<double, double> float64Func = t => t * t;
        Func<Scalar<double>, Scalar<double>> genericFunc = t => t * t;

        var float64Signal = Float64ScalarComputedSignal.Finite(Float64ScalarRange.SymmetricOne, float64Func);
        var genericSignal = ComputedScalarSignal<double>.Finite(ScalarProcessor, genericFunc);

        // Test at various points
        for (double t = -1.0; t <= 1.0; t += 0.2)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
            Assert.That(float64Value, Is.EqualTo(t * t).Within(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestComputedSignal_SineFunction()
    {
        // Test with sine function: f(t) = sin(t)
        Func<double, double> float64Func = Math.Sin;
        Func<Scalar<double>, Scalar<double>> genericFunc = t => ScalarProcessor.Sin(t.ScalarValue);

        var float64Signal = Float64ScalarComputedSignal.Finite(Float64ScalarRange.SymmetricPi, float64Func);
        var genericSignal = ComputedScalarSignal<double>.Finite(
            ScalarRange<double>.SymmetricPi(ScalarProcessor),
            genericFunc
        );

        // Test at various points
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestComputedSignal_WithDerivative1()
    {
        // Test with f(t) = t², f'(t) = 2t
        Func<double, double> float64Func = t => t * t;
        Func<double, double> float64Deriv1 = t => 2 * t;

        Func<Scalar<double>, Scalar<double>> genericFunc = t => t * t;
        Func<Scalar<double>, Scalar<double>> genericDeriv1 = t => ScalarProcessor.ScalarFromNumber(2) * t;

        var float64Signal = Float64ScalarComputedSignal.Finite(
            Float64ScalarRange.SymmetricOne,
            float64Func,
            float64Deriv1
        );
        var genericSignal = ComputedScalarSignal<double>.Finite(
            ScalarProcessor,
            genericFunc,
            genericDeriv1
        );

        // Test values and derivatives
        for (double t = -1.0; t <= 1.0; t += 0.25)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Value mismatch at t={t}");

            var float64Deriv = float64Signal.GetDerivative1Value(t);
            var genericDeriv = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Derivative1 mismatch at t={t}");
            Assert.That(float64Deriv, Is.EqualTo(2 * t).Within(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestComputedSignal_WithDerivative2()
    {
        // Test with f(t) = t³, f'(t) = 3t², f''(t) = 6t
        Func<double, double> float64Func = t => t * t * t;
        Func<double, double> float64Deriv1 = t => 3 * t * t;
        Func<double, double> float64Deriv2 = t => 6 * t;

        Func<Scalar<double>, Scalar<double>> genericFunc = t => t * t * t;
        Func<Scalar<double>, Scalar<double>> genericDeriv1 = t => ScalarProcessor.ScalarFromNumber(3) * t * t;
        Func<Scalar<double>, Scalar<double>> genericDeriv2 = t => ScalarProcessor.ScalarFromNumber(6) * t;

        var float64Signal = Float64ScalarComputedSignal.Finite(
            Float64ScalarRange.SymmetricOne,
            float64Func,
            float64Deriv1,
            float64Deriv2
        );
        var genericSignal = ComputedScalarSignal<double>.Finite(
            ScalarProcessor,
            genericFunc,
            genericDeriv1,
            genericDeriv2
        );

        // Test values and both derivatives
        for (double t = -1.0; t <= 1.0; t += 0.25)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Value mismatch at t={t}");

            var float64Deriv1Val = float64Signal.GetDerivative1Value(t);
            var genericDeriv1Val = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv1Val - genericDeriv1Val), Is.LessThan(Tolerance),
                $"Derivative1 mismatch at t={t}");

            var float64Deriv2Val = float64Signal.GetDerivative2Value(t);
            var genericDeriv2Val = genericSignal.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv2Val - genericDeriv2Val), Is.LessThan(Tolerance),
                $"Derivative2 mismatch at t={t}");
            Assert.That(float64Deriv2Val, Is.EqualTo(6 * t).Within(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestComputedSignal_ExponentialFunction()
    {
        // Test with f(t) = exp(t), f'(t) = exp(t), f''(t) = exp(t)
        Func<double, double> float64Func = Math.Exp;
        Func<double, double> float64Deriv1 = Math.Exp;
        Func<double, double> float64Deriv2 = Math.Exp;

        Func<Scalar<double>, Scalar<double>> genericFunc = t => ScalarProcessor.Exp(t.ScalarValue);
        Func<Scalar<double>, Scalar<double>> genericDeriv1 = t => ScalarProcessor.Exp(t.ScalarValue);
        Func<Scalar<double>, Scalar<double>> genericDeriv2 = t => ScalarProcessor.Exp(t.ScalarValue);

        var float64Signal = Float64ScalarComputedSignal.Finite(
            Float64ScalarRange.SymmetricOne,
            float64Func,
            float64Deriv1,
            float64Deriv2
        );
        var genericSignal = ComputedScalarSignal<double>.Finite(
            ScalarProcessor,
            genericFunc,
            genericDeriv1,
            genericDeriv2
        );

        // Test at various points
        for (double t = -1.0; t <= 1.0; t += 0.2)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Value mismatch at t={t}");

            var float64Deriv1Val = float64Signal.GetDerivative1Value(t);
            var genericDeriv1Val = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv1Val - genericDeriv1Val), Is.LessThan(Tolerance),
                $"Derivative1 mismatch at t={t}");

            var float64Deriv2Val = float64Signal.GetDerivative2Value(t);
            var genericDeriv2Val = genericSignal.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv2Val - genericDeriv2Val), Is.LessThan(Tolerance),
                $"Derivative2 mismatch at t={t}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestComputedSignal_PeriodicSignal()
    {
        Func<double, double> float64Func = t => t * t;
        Func<Scalar<double>, Scalar<double>> genericFunc = t => t * t;

        var float64Signal = Float64ScalarComputedSignal.Periodic(Float64ScalarRange.SymmetricOne, float64Func);
        var genericSignal = ComputedScalarSignal<double>.Periodic(ScalarProcessor, genericFunc);

        for (double t = -1.0; t <= 1.0; t += 0.25)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestComputedSignal_FiniteToPeriodicConversion()
    {
        Func<Scalar<double>, Scalar<double>> genericFunc = t => t * t;

        var finiteSignal = ComputedScalarSignal<double>.Finite(ScalarProcessor, genericFunc);
        var periodicSignal = finiteSignal.ToPeriodicSignal() as ComputedScalarSignal<double>;

        Assert.That(periodicSignal, Is.Not.Null);
        Assert.That(periodicSignal!.IsPeriodic, Is.True);

        // Test that values are the same
        for (double t = -1.0; t <= 1.0; t += 0.5)
        {
            var finiteValue = finiteSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            var periodicValue = periodicSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(finiteValue - periodicValue), Is.LessThan(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestComputedSignal_PeriodicToFiniteConversion()
    {
        Func<Scalar<double>, Scalar<double>> genericFunc = t => t * t;

        var periodicSignal = ComputedScalarSignal<double>.Periodic(ScalarProcessor, genericFunc);
        var finiteSignal = periodicSignal.ToFiniteSignal() as ComputedScalarSignal<double>;

        Assert.That(finiteSignal, Is.Not.Null);
        Assert.That(finiteSignal!.IsFinite, Is.True);

        // Test that values are the same
        for (double t = -1.0; t <= 1.0; t += 0.5)
        {
            var periodicValue = periodicSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            var finiteValue = finiteSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(periodicValue - finiteValue), Is.LessThan(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestComputedSignal_IsValid()
    {
        Func<Scalar<double>, Scalar<double>> genericFunc = t => t * t;

        var signal1 = ComputedScalarSignal<double>.Finite(ScalarProcessor, genericFunc);
        var signal2 = ComputedScalarSignal<double>.Periodic(ScalarProcessor, genericFunc);

        Assert.That(signal1.IsValid(), Is.True);
        Assert.That(signal2.IsValid(), Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestComputedSignal_TimeRangeProperties()
    {
        Func<Scalar<double>, Scalar<double>> genericFunc = t => t * t;

        var signal = ComputedScalarSignal<double>.Finite(ScalarProcessor, genericFunc);

        Assert.That(signal.TimeRange.MinValue.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance));
        Assert.That(signal.TimeRange.MaxValue.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestComputedSignal_CustomTimeRange()
    {
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.ScalarFromNumber(0.0),
            ScalarProcessor.ScalarFromNumber(2.0 * Math.PI)
        );
        Func<Scalar<double>, Scalar<double>> genericFunc = t => ScalarProcessor.Sin(t.ScalarValue);

        var signal = ComputedScalarSignal<double>.Finite(timeRange, genericFunc);

        Assert.That(signal.TimeRange.MinValue.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(signal.TimeRange.MaxValue.ScalarValue, Is.EqualTo(2.0 * Math.PI).Within(Tolerance));

        // Test values across custom range
        for (double t = 0.0; t <= 2.0 * Math.PI; t += Math.PI / 4)
        {
            var value = signal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(value, Is.EqualTo(Math.Sin(t)).Within(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestComputedSignal_PolynomialWithAllDerivatives()
    {
        // Test with f(t) = t⁴ - 2t² + 1
        // f'(t) = 4t³ - 4t
        // f''(t) = 12t² - 4
        Func<double, double> float64Func = t => t * t * t * t - 2 * t * t + 1;
        Func<double, double> float64Deriv1 = t => 4 * t * t * t - 4 * t;
        Func<double, double> float64Deriv2 = t => 12 * t * t - 4;

        Func<Scalar<double>, Scalar<double>> genericFunc = t =>
        {
            var t2 = t * t;
            var t4 = t2 * t2;
            return t4 - ScalarProcessor.ScalarFromNumber(2) * t2 + ScalarProcessor.One;
        };
        Func<Scalar<double>, Scalar<double>> genericDeriv1 = t =>
        {
            var t2 = t * t;
            var t3 = t2 * t;
            return ScalarProcessor.ScalarFromNumber(4) * t3 - ScalarProcessor.ScalarFromNumber(4) * t;
        };
        Func<Scalar<double>, Scalar<double>> genericDeriv2 = t =>
        {
            var t2 = t * t;
            return ScalarProcessor.ScalarFromNumber(12) * t2 - ScalarProcessor.ScalarFromNumber(4);
        };

        var float64Signal = Float64ScalarComputedSignal.Finite(
            Float64ScalarRange.SymmetricOne,
            float64Func,
            float64Deriv1,
            float64Deriv2
        );
        var genericSignal = ComputedScalarSignal<double>.Finite(
            ScalarProcessor,
            genericFunc,
            genericDeriv1,
            genericDeriv2
        );

        // Test at specific points
        double[] testPoints = { -1.0, -0.5, 0.0, 0.5, 1.0 };
        foreach (var t in testPoints)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Value mismatch at t={t}");

            var float64Deriv1Val = float64Signal.GetDerivative1Value(t);
            var genericDeriv1Val = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv1Val - genericDeriv1Val), Is.LessThan(Tolerance),
                $"Derivative1 mismatch at t={t}");

            var float64Deriv2Val = float64Signal.GetDerivative2Value(t);
            var genericDeriv2Val = genericSignal.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv2Val - genericDeriv2Val), Is.LessThan(Tolerance),
                $"Derivative2 mismatch at t={t}");
        }

        Debug.Assert(true);
    }
}
