using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public sealed class SinScalarSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestFiniteSinSignal_BoundaryValues()
    {
        var float64Signal = Float64ScalarSinSignal.FiniteInstance;
        var genericSignal = SinScalarSignal<double>.Finite(ScalarProcessor);

        // Test at t = -π (left boundary, should be ~0)
        var t1 = -Math.PI;
        var float64Value1 = float64Signal.GetValue(t1);
        var genericValue1 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t1)).ScalarValue;
        Assert.That(Math.Abs(float64Value1 - genericValue1), Is.LessThan(Tolerance),
            $"Mismatch at t={t1}: Float64={float64Value1}, Generic={genericValue1}");
        Assert.That(Math.Abs(float64Value1), Is.LessThan(Tolerance)); // sin(-π) ≈ 0

        // Test at t = 0 (center, should be 0)
        var t2 = 0.0;
        var float64Value2 = float64Signal.GetValue(t2);
        var genericValue2 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t2)).ScalarValue;
        Assert.That(Math.Abs(float64Value2 - genericValue2), Is.LessThan(Tolerance),
            $"Mismatch at t={t2}: Float64={float64Value2}, Generic={genericValue2}");
        Assert.That(Math.Abs(float64Value2), Is.LessThan(Tolerance)); // sin(0) = 0

        // Test at t = π (right boundary, should be ~0)
        var t3 = Math.PI;
        var float64Value3 = float64Signal.GetValue(t3);
        var genericValue3 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t3)).ScalarValue;
        Assert.That(Math.Abs(float64Value3 - genericValue3), Is.LessThan(Tolerance),
            $"Mismatch at t={t3}: Float64={float64Value3}, Generic={genericValue3}");
        Assert.That(Math.Abs(float64Value3), Is.LessThan(Tolerance)); // sin(π) ≈ 0

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSinSignal_KeyPoints()
    {
        var float64Signal = Float64ScalarSinSignal.FiniteInstance;
        var genericSignal = SinScalarSignal<double>.Finite(ScalarProcessor);

        // Test at t = π/2 (maximum, should be 1)
        var t1 = Math.PI / 2;
        var float64Value1 = float64Signal.GetValue(t1);
        var genericValue1 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t1)).ScalarValue;
        Assert.That(Math.Abs(float64Value1 - genericValue1), Is.LessThan(Tolerance),
            $"Mismatch at t={t1}: Float64={float64Value1}, Generic={genericValue1}");
        Assert.That(float64Value1, Is.EqualTo(1.0).Within(Tolerance)); // sin(π/2) = 1

        // Test at t = -π/2 (minimum, should be -1)
        var t2 = -Math.PI / 2;
        var float64Value2 = float64Signal.GetValue(t2);
        var genericValue2 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t2)).ScalarValue;
        Assert.That(Math.Abs(float64Value2 - genericValue2), Is.LessThan(Tolerance),
            $"Mismatch at t={t2}: Float64={float64Value2}, Generic={genericValue2}");
        Assert.That(float64Value2, Is.EqualTo(-1.0).Within(Tolerance)); // sin(-π/2) = -1

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSinSignal_VariousPoints()
    {
        var float64Signal = Float64ScalarSinSignal.FiniteInstance;
        var genericSignal = SinScalarSignal<double>.Finite(ScalarProcessor);

        // Test at various points in the range
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 8)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSinSignal_Derivative1()
    {
        var float64Signal = Float64ScalarSinSignal.FiniteInstance;
        var genericSignal = SinScalarSignal<double>.Finite(ScalarProcessor);

        // Test first derivative at various points (should be cos(t))
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Deriv = float64Signal.GetDerivative1Value(t);
            var genericDeriv = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Derivative mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");

            // Verify derivative is cos(t)
            var expectedDeriv = Math.Cos(t);
            Assert.That(Math.Abs(float64Deriv - expectedDeriv), Is.LessThan(Tolerance),
                $"Derivative should be cos(t) at t={t}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSinSignal_Derivative1AtKeyPoints()
    {
        var float64Signal = Float64ScalarSinSignal.FiniteInstance;
        var genericSignal = SinScalarSignal<double>.Finite(ScalarProcessor);

        // Derivative at t=0: cos(0) = 1
        var t1 = 0.0;
        var float64Deriv1 = float64Signal.GetDerivative1Value(t1);
        var genericDeriv1 = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t1)).ScalarValue;
        Assert.That(Math.Abs(float64Deriv1 - genericDeriv1), Is.LessThan(Tolerance));
        Assert.That(float64Deriv1, Is.EqualTo(1.0).Within(Tolerance));

        // Derivative at t=π/2: cos(π/2) ≈ 0
        var t2 = Math.PI / 2;
        var float64Deriv2 = float64Signal.GetDerivative1Value(t2);
        var genericDeriv2 = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t2)).ScalarValue;
        Assert.That(Math.Abs(float64Deriv2 - genericDeriv2), Is.LessThan(Tolerance));
        Assert.That(Math.Abs(float64Deriv2), Is.LessThan(Tolerance));

        // Derivative at t=-π/2: cos(-π/2) ≈ 0
        var t3 = -Math.PI / 2;
        var float64Deriv3 = float64Signal.GetDerivative1Value(t3);
        var genericDeriv3 = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t3)).ScalarValue;
        Assert.That(Math.Abs(float64Deriv3 - genericDeriv3), Is.LessThan(Tolerance));
        Assert.That(Math.Abs(float64Deriv3), Is.LessThan(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSinSignal_Derivative2()
    {
        var float64Signal = Float64ScalarSinSignal.FiniteInstance;
        var genericSignal = SinScalarSignal<double>.Finite(ScalarProcessor);

        // Test second derivative at various points (should be -sin(t))
        for (double t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Deriv = float64Signal.GetDerivative2Value(t);
            var genericDeriv = genericSignal.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Second derivative mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");

            // Verify derivative is -sin(t)
            var expectedDeriv = -Math.Sin(t);
            Assert.That(Math.Abs(float64Deriv - expectedDeriv), Is.LessThan(Tolerance),
                $"Second derivative should be -sin(t) at t={t}");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestPeriodicSinSignal()
    {
        var float64Signal = Float64ScalarSinSignal.PeriodicInstance;
        var genericSignal = SinScalarSignal<double>.Periodic(ScalarProcessor);

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
    public void TestFiniteToPeriodicConversion()
    {
        var finiteSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var periodicSignal = finiteSignal.ToPeriodicSignal() as SinScalarSignal<double>;

        Assert.That(periodicSignal, Is.Not.Null);
        Assert.That(periodicSignal!.IsPeriodic, Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestPeriodicToFiniteConversion()
    {
        var periodicSignal = SinScalarSignal<double>.Periodic(ScalarProcessor);
        var finiteSignal = periodicSignal.ToFiniteSignal() as SinScalarSignal<double>;

        Assert.That(finiteSignal, Is.Not.Null);
        Assert.That(finiteSignal!.IsFinite, Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestIsValid()
    {
        var finiteSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var periodicSignal = SinScalarSignal<double>.Periodic(ScalarProcessor);

        Assert.That(finiteSignal.IsValid(), Is.True);
        Assert.That(periodicSignal.IsValid(), Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestTimeRangeProperties()
    {
        var signal = SinScalarSignal<double>.Finite(ScalarProcessor);

        Assert.That(signal.TimeRange.MinValue.ScalarValue, Is.EqualTo(-Math.PI).Within(Tolerance));
        Assert.That(signal.TimeRange.MaxValue.ScalarValue, Is.EqualTo(Math.PI).Within(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestOddFunctionProperty()
    {
        var genericSignal = SinScalarSignal<double>.Finite(ScalarProcessor);

        // sin(-t) = -sin(t)
        for (double t = 0.1; t <= Math.PI; t += Math.PI / 10)
        {
            var valuePlus = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            var valueMinus = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(-t)).ScalarValue;
            Assert.That(Math.Abs(valuePlus + valueMinus), Is.LessThan(Tolerance),
                $"Odd function property broken at t=±{t}: sin({t})={valuePlus}, sin({-t})={valueMinus}");
        }

        Debug.Assert(true);
    }
}
