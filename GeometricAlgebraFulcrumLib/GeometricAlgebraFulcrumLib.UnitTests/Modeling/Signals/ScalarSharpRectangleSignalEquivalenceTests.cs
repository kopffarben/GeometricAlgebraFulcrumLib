using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Normalized;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Normalized;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public sealed class ScalarSharpRectangleSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestFiniteSharpRectangleSignal_BoundaryValues()
    {
        var float64Signal = Float64ScalarSharpRectangleSignal.FiniteInstance;
        var genericSignal = ScalarSharpRectangleSignal<double>.Finite(ScalarProcessor);

        // Test at t = -1 (left boundary, should be -1)
        var t1 = -1.0;
        var float64Value1 = float64Signal.GetValue(t1);
        var genericValue1 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t1)).ScalarValue;
        Assert.That(Math.Abs(float64Value1 - genericValue1), Is.LessThan(Tolerance),
            $"Mismatch at t={t1}: Float64={float64Value1}, Generic={genericValue1}");
        Assert.That(float64Value1, Is.EqualTo(-1.0).Within(Tolerance));

        // Test at t = 0 (center, should be 1)
        var t2 = 0.0;
        var float64Value2 = float64Signal.GetValue(t2);
        var genericValue2 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t2)).ScalarValue;
        Assert.That(Math.Abs(float64Value2 - genericValue2), Is.LessThan(Tolerance),
            $"Mismatch at t={t2}: Float64={float64Value2}, Generic={genericValue2}");
        Assert.That(float64Value2, Is.EqualTo(1.0).Within(Tolerance));

        // Test at t = 1 (right boundary, should be -1)
        var t3 = 1.0;
        var float64Value3 = float64Signal.GetValue(t3);
        var genericValue3 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t3)).ScalarValue;
        Assert.That(Math.Abs(float64Value3 - genericValue3), Is.LessThan(Tolerance),
            $"Mismatch at t={t3}: Float64={float64Value3}, Generic={genericValue3}");
        Assert.That(float64Value3, Is.EqualTo(-1.0).Within(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSharpRectangleSignal_DiscontinuityPoints()
    {
        var float64Signal = Float64ScalarSharpRectangleSignal.FiniteInstance;
        var genericSignal = ScalarSharpRectangleSignal<double>.Finite(ScalarProcessor);

        // Test at t = -0.5 (discontinuity, should be 0)
        var t1 = -0.5;
        var float64Value1 = float64Signal.GetValue(t1);
        var genericValue1 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t1)).ScalarValue;
        Assert.That(Math.Abs(float64Value1 - genericValue1), Is.LessThan(Tolerance),
            $"Mismatch at t={t1}: Float64={float64Value1}, Generic={genericValue1}");
        Assert.That(Math.Abs(float64Value1), Is.LessThan(Tolerance)); // Should be 0

        // Test at t = 0.5 (discontinuity, should be 0)
        var t2 = 0.5;
        var float64Value2 = float64Signal.GetValue(t2);
        var genericValue2 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t2)).ScalarValue;
        Assert.That(Math.Abs(float64Value2 - genericValue2), Is.LessThan(Tolerance),
            $"Mismatch at t={t2}: Float64={float64Value2}, Generic={genericValue2}");
        Assert.That(Math.Abs(float64Value2), Is.LessThan(Tolerance)); // Should be 0

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSharpRectangleSignal_InsideRectangle()
    {
        var float64Signal = Float64ScalarSharpRectangleSignal.FiniteInstance;
        var genericSignal = ScalarSharpRectangleSignal<double>.Finite(ScalarProcessor);

        // Test inside rectangle region (|t| < 0.5, should be 1)
        for (double t = -0.4; t <= 0.4; t += 0.1)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
            Assert.That(float64Value, Is.EqualTo(1.0).Within(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSharpRectangleSignal_OutsideRectangle()
    {
        var float64Signal = Float64ScalarSharpRectangleSignal.FiniteInstance;
        var genericSignal = ScalarSharpRectangleSignal<double>.Finite(ScalarProcessor);

        // Test outside rectangle region (|t| > 0.5, should be -1)
        var testPoints = new[] { -0.9, -0.7, -0.6, 0.6, 0.7, 0.9 };
        foreach (var t in testPoints)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Value - genericValue), Is.LessThan(Tolerance),
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}");
            Assert.That(float64Value, Is.EqualTo(-1.0).Within(Tolerance));
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSharpRectangleSignal_Derivative1()
    {
        var float64Signal = Float64ScalarSharpRectangleSignal.FiniteInstance;
        var genericSignal = ScalarSharpRectangleSignal<double>.Finite(ScalarProcessor);

        // Test first derivative at various points (should always be 0)
        for (double t = -1.0; t <= 1.0; t += 0.2)
        {
            var float64Deriv = float64Signal.GetDerivative1Value(t);
            var genericDeriv = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Derivative mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");
            Assert.That(Math.Abs(float64Deriv), Is.LessThan(Tolerance)); // Should be 0
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestFiniteSharpRectangleSignal_Derivative2()
    {
        var float64Signal = Float64ScalarSharpRectangleSignal.FiniteInstance;
        var genericSignal = ScalarSharpRectangleSignal<double>.Finite(ScalarProcessor);

        // Test second derivative at various points (should always be 0)
        for (double t = -1.0; t <= 1.0; t += 0.2)
        {
            var float64Deriv = float64Signal.GetDerivative2Value(t);
            var genericDeriv = genericSignal.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            Assert.That(Math.Abs(float64Deriv - genericDeriv), Is.LessThan(Tolerance),
                $"Second derivative mismatch at t={t}: Float64={float64Deriv}, Generic={genericDeriv}");
            Assert.That(Math.Abs(float64Deriv), Is.LessThan(Tolerance)); // Should be 0
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestPeriodicSharpRectangleSignal()
    {
        var float64Signal = Float64ScalarSharpRectangleSignal.PeriodicInstance;
        var genericSignal = ScalarSharpRectangleSignal<double>.Periodic(ScalarProcessor);

        for (double t = -1.0; t <= 1.0; t += 0.2)
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
        var finiteSignal = ScalarSharpRectangleSignal<double>.Finite(ScalarProcessor);
        var periodicSignal = finiteSignal.ToPeriodicSignal() as ScalarSharpRectangleSignal<double>;

        Assert.That(periodicSignal, Is.Not.Null);
        Assert.That(periodicSignal!.IsPeriodic, Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestPeriodicToFiniteConversion()
    {
        var periodicSignal = ScalarSharpRectangleSignal<double>.Periodic(ScalarProcessor);
        var finiteSignal = periodicSignal.ToFiniteSignal() as ScalarSharpRectangleSignal<double>;

        Assert.That(finiteSignal, Is.Not.Null);
        Assert.That(finiteSignal!.IsFinite, Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestIsValid()
    {
        var finiteSignal = ScalarSharpRectangleSignal<double>.Finite(ScalarProcessor);
        var periodicSignal = ScalarSharpRectangleSignal<double>.Periodic(ScalarProcessor);

        Assert.That(finiteSignal.IsValid(), Is.True);
        Assert.That(periodicSignal.IsValid(), Is.True);

        Debug.Assert(true);
    }

    [Test]
    public void TestOutOfRangeClamping()
    {
        var float64Signal = Float64ScalarSharpRectangleSignal.FiniteInstance;
        var genericSignal = ScalarSharpRectangleSignal<double>.Finite(ScalarProcessor);

        // Test below minimum
        var t1 = -2.0;
        var float64Value1 = float64Signal.GetValue(t1);
        var genericValue1 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t1)).ScalarValue;
        Assert.That(Math.Abs(float64Value1 - genericValue1), Is.LessThan(Tolerance),
            $"Clamping mismatch at t={t1}: Float64={float64Value1}, Generic={genericValue1}");
        Assert.That(float64Value1, Is.EqualTo(-1.0).Within(Tolerance));

        // Test above maximum
        var t2 = 2.0;
        var float64Value2 = float64Signal.GetValue(t2);
        var genericValue2 = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t2)).ScalarValue;
        Assert.That(Math.Abs(float64Value2 - genericValue2), Is.LessThan(Tolerance),
            $"Clamping mismatch at t={t2}: Float64={float64Value2}, Generic={genericValue2}");
        Assert.That(float64Value2, Is.EqualTo(-1.0).Within(Tolerance));

        Debug.Assert(true);
    }

    [Test]
    public void TestTimeRangeProperties()
    {
        var signal = ScalarSharpRectangleSignal<double>.Finite(ScalarProcessor);

        Assert.That(signal.TimeRange.MinValue.ScalarValue, Is.EqualTo(-1.0));
        Assert.That(signal.TimeRange.MaxValue.ScalarValue, Is.EqualTo(1.0));

        Debug.Assert(true);
    }
}
