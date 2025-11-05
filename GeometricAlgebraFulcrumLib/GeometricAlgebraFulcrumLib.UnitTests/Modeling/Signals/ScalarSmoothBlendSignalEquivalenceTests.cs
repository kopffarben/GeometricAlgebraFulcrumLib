using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Mapped;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Normalized;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Normalized;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public sealed class ScalarSmoothBlendSignalEquivalenceTests
{
    private const double Tolerance = 1e-12;

    private IScalarProcessor<double> ScalarProcessor { get; }
        = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestFiniteSmoothBlend_SinCos()
    {
        // Float64 version
        var signal1Float64 = Float64ScalarSinSignal.FiniteInstance;
        var signal2Float64 = Float64ScalarCosSignal.FiniteInstance;
        var blendFloat64 = Float64ScalarSmoothBlendSignal.Finite(-0.5, 0.5, signal1Float64, signal2Float64);

        // Generic version
        var signal1Generic = SinScalarSignal<double>.Finite(ScalarProcessor);
        var signal2Generic = CosScalarSignal<double>.Finite(ScalarProcessor);
        var blendGeneric = ScalarSmoothBlendSignal<double>.Finite(
            ScalarProcessor.Scalar(-0.5),
            ScalarProcessor.Scalar(0.5),
            signal1Generic,
            signal2Generic
        );

        // Verify time range
        Assert.That(blendGeneric.MinTime.ScalarValue, Is.EqualTo(blendFloat64.MinTime).Within(Tolerance));
        Assert.That(blendGeneric.MaxTime.ScalarValue, Is.EqualTo(blendFloat64.MaxTime).Within(Tolerance));
        Assert.That(blendGeneric.IsPeriodic, Is.EqualTo(blendFloat64.IsPeriodic));

        // Test values at various times within blend range
        for (var t = -0.5; t <= 0.5; t += 0.1)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = blendFloat64.GetValue(t);
            var valueGeneric = blendGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }

        // Test at edges of blend range
        var minValue = blendGeneric.GetValue(ScalarProcessor.Scalar(-0.5)).ScalarValue;
        Assert.That(minValue, Is.EqualTo(blendFloat64.GetValue(-0.5)).Within(Tolerance));

        var maxValue = blendGeneric.GetValue(ScalarProcessor.Scalar(0.5)).ScalarValue;
        Assert.That(maxValue, Is.EqualTo(blendFloat64.GetValue(0.5)).Within(Tolerance));
    }

    [Test]
    public void TestPeriodicSmoothBlend_RampSignals()
    {
        // Float64 version - blend from linear ramp to constant signal
        var signal1Float64 = Float64ScalarRampSignal.FiniteInstance;
        var signal2Float64 = Float64ScalarConstantOneSignal.FiniteInstance;
        var blendFloat64 = Float64ScalarSmoothBlendSignal.Periodic(-1, 1, signal1Float64, signal2Float64);

        // Generic version
        var signal1Generic = ScalarRampSignal<double>.Finite(ScalarProcessor);
        var signal2Generic = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var blendGeneric = ScalarSmoothBlendSignal<double>.Periodic(
            ScalarProcessor.Scalar(-1),
            ScalarProcessor.One,
            signal1Generic,
            signal2Generic
        );

        // Verify periodic flag
        Assert.That(blendGeneric.IsPeriodic, Is.True);
        Assert.That(blendFloat64.IsPeriodic, Is.True);

        // Test values at various times
        for (var t = -1.0; t <= 1.0; t += 0.25)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = blendFloat64.GetValue(t);
            var valueGeneric = blendGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }
    }

    [Test]
    public void TestBlendingBehavior_MidpointCheck()
    {
        // At midpoint of blend range, should be approximately 50% blend
        var signal1Float64 = Float64ScalarConstantZeroSignal.FiniteInstance;
        var signal2Float64 = Float64ScalarConstantOneSignal.FiniteInstance;
        var blendFloat64 = Float64ScalarSmoothBlendSignal.Finite(0.0, 1.0, signal1Float64, signal2Float64);

        var signal1Generic = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.Zero);
        var signal2Generic = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.One);
        var blendGeneric = ScalarSmoothBlendSignal<double>.Finite(
            ScalarProcessor.Zero,
            ScalarProcessor.One,
            signal1Generic,
            signal2Generic
        );

        // At t=0.5 (midpoint), value should be near 0.5
        var midValue = blendGeneric.GetValue(ScalarProcessor.Scalar(0.5)).ScalarValue;
        Assert.That(midValue, Is.EqualTo(blendFloat64.GetValue(0.5)).Within(Tolerance));
        Assert.That(midValue, Is.InRange(0.4, 0.6), "Midpoint value should be near 0.5");

        // At t=0 (start), value should be near 0
        var startValue = blendGeneric.GetValue(ScalarProcessor.Scalar(0.0)).ScalarValue;
        Assert.That(startValue, Is.EqualTo(blendFloat64.GetValue(0.0)).Within(Tolerance));

        // At t=1 (end), value should be near 1
        var endValue = blendGeneric.GetValue(ScalarProcessor.Scalar(1.0)).ScalarValue;
        Assert.That(endValue, Is.EqualTo(blendFloat64.GetValue(1.0)).Within(Tolerance));
    }

    [Test]
    public void TestToFiniteSignal()
    {
        var signal1Generic = SinScalarSignal<double>.Finite(ScalarProcessor);
        var signal2Generic = CosScalarSignal<double>.Finite(ScalarProcessor);
        var blendGeneric = ScalarSmoothBlendSignal<double>.Periodic(
            ScalarProcessor.Scalar(-1),
            ScalarProcessor.Scalar(1),
            signal1Generic,
            signal2Generic
        );

        var finiteBlend = blendGeneric.ToFiniteSignal();

        Assert.That(finiteBlend.IsFinite, Is.True);
        Assert.That(finiteBlend.IsPeriodic, Is.False);

        // Values should match
        for (var t = -1.0; t <= 1.0; t += 0.3)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var originalValue = blendGeneric.GetValue(tScalar).ScalarValue;
            var finiteValue = finiteBlend.GetValue(tScalar).ScalarValue;

            Assert.That(finiteValue, Is.EqualTo(originalValue).Within(Tolerance));
        }
    }

    [Test]
    public void TestToPeriodicSignal()
    {
        var signal1Generic = SinScalarSignal<double>.Finite(ScalarProcessor);
        var signal2Generic = CosScalarSignal<double>.Finite(ScalarProcessor);
        var blendGeneric = ScalarSmoothBlendSignal<double>.Finite(
            ScalarProcessor.Scalar(-1),
            ScalarProcessor.Scalar(1),
            signal1Generic,
            signal2Generic
        );

        var periodicBlend = blendGeneric.ToPeriodicSignal();

        Assert.That(periodicBlend.IsFinite, Is.False);
        Assert.That(periodicBlend.IsPeriodic, Is.True);

        // Values should match
        for (var t = -1.0; t <= 1.0; t += 0.3)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var originalValue = blendGeneric.GetValue(tScalar).ScalarValue;
            var periodicValue = periodicBlend.GetValue(tScalar).ScalarValue;

            Assert.That(periodicValue, Is.EqualTo(originalValue).Within(Tolerance));
        }
    }

    [Test]
    public void TestBaseSignalProperties()
    {
        var signal1Generic = SinScalarSignal<double>.Finite(ScalarProcessor);
        var signal2Generic = CosScalarSignal<double>.Finite(ScalarProcessor);
        var blendGeneric = ScalarSmoothBlendSignal<double>.Finite(
            ScalarProcessor.Scalar(-1),
            ScalarProcessor.Scalar(1),
            signal1Generic,
            signal2Generic
        );

        Assert.That(blendGeneric.BaseSignal1, Is.SameAs(signal1Generic));
        Assert.That(blendGeneric.BaseSignal2, Is.SameAs(signal2Generic));
    }

    [Test]
    public void TestIsValid()
    {
        var signal1Generic = SinScalarSignal<double>.Finite(ScalarProcessor);
        var signal2Generic = CosScalarSignal<double>.Finite(ScalarProcessor);
        var blendGeneric = ScalarSmoothBlendSignal<double>.Finite(
            ScalarRange<double>.NegativeOneToOne(ScalarProcessor),
            signal1Generic,
            signal2Generic
        );

        Assert.That(blendGeneric.IsValid(), Is.True);
    }

    [Test]
    public void TestFactoryMethods_WithRange()
    {
        var timeRange = ScalarRange<double>.Create(ScalarProcessor.Scalar(0.0), ScalarProcessor.Scalar(2.0));
        var signal1Generic = SinScalarSignal<double>.Finite(ScalarProcessor);
        var signal2Generic = CosScalarSignal<double>.Finite(ScalarProcessor);

        var blendFinite = ScalarSmoothBlendSignal<double>.Finite(timeRange, signal1Generic, signal2Generic);
        var blendPeriodic = ScalarSmoothBlendSignal<double>.Periodic(timeRange, signal1Generic, signal2Generic);

        Assert.That(blendFinite.TimeRange, Is.EqualTo(timeRange));
        Assert.That(blendFinite.IsFinite, Is.True);

        Assert.That(blendPeriodic.TimeRange, Is.EqualTo(timeRange));
        Assert.That(blendPeriodic.IsPeriodic, Is.True);
    }

    [Test]
    public void TestSmoothTransition_Monotonicity()
    {
        // Blend from 0 to 1 - should be monotonically increasing
        var signal1Generic = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.Scalar(0.0));
        var signal2Generic = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.Scalar(1.0));
        var blendGeneric = ScalarSmoothBlendSignal<double>.Finite(
            ScalarProcessor.Scalar(0.0),
            ScalarProcessor.Scalar(1.0),
            signal1Generic,
            signal2Generic
        );

        var previousValue = blendGeneric.GetValue(ScalarProcessor.Scalar(0.0)).ScalarValue;

        for (var t = 0.1; t <= 1.0; t += 0.1)
        {
            var currentValue = blendGeneric.GetValue(ScalarProcessor.Scalar(t)).ScalarValue;
            Assert.That(currentValue, Is.GreaterThanOrEqualTo(previousValue),
                $"Blend should be monotonically increasing, but decreased from {previousValue} to {currentValue} at t={t}");
            previousValue = currentValue;
        }
    }
}
