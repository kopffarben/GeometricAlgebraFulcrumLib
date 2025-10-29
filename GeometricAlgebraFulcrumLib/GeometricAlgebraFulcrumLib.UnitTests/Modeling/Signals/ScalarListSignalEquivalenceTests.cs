using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Mapped;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public sealed class ScalarListSignalEquivalenceTests
{
    private static readonly ScalarProcessorOfFloat64 ScalarProcessor =
        ScalarProcessorOfFloat64.Instance;

    private const double Tolerance = 1e-12;

    [Test]
    public void TestFiniteList_TwoSignals()
    {
        // Float64 version
        var signal1Float64 = Float64ScalarSinSignal.FiniteInstance;
        var signal2Float64 = Float64ScalarCosSignal.FiniteInstance;
        var listFloat64 = Float64ScalarListSignal.Finite(signal1Float64, signal2Float64);

        // Generic version
        var signal1Generic = SinScalarSignal<double>.Finite(ScalarProcessor);
        var signal2Generic = CosScalarSignal<double>.Finite(ScalarProcessor);
        var listGeneric = ScalarListSignal<double>.Finite(signal1Generic, signal2Generic);

        // Verify time ranges match
        Assert.That(listGeneric.MinTime.ScalarValue, Is.EqualTo(listFloat64.MinTime).Within(Tolerance));
        Assert.That(listGeneric.MaxTime.ScalarValue, Is.EqualTo(listFloat64.MaxTime).Within(Tolerance));
        Assert.That(listGeneric.Count, Is.EqualTo(listFloat64.Count));

        // Test values at various times
        for (var t = listFloat64.MinTime; t <= listFloat64.MaxTime; t += 0.5)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = listFloat64.GetValue(t);
            var valueGeneric = listGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }

        Debug.WriteLine($"TestFiniteList_TwoSignals passed");
    }

    [Test]
    public void TestPeriodicList_ThreeSignals()
    {
        // Float64 version
        var signal1Float64 = Float64ScalarSinSignal.FiniteInstance;
        var signal2Float64 = Float64ScalarCosSignal.FiniteInstance;
        var signal3Float64 = Float64ScalarConstantOneSignal.FiniteInstance;
        var listFloat64 = Float64ScalarListSignal.Periodic(signal1Float64, signal2Float64, signal3Float64);

        // Generic version
        var signal1Generic = SinScalarSignal<double>.Finite(ScalarProcessor);
        var signal2Generic = CosScalarSignal<double>.Finite(ScalarProcessor);
        var signal3Generic = ConstantScalarSignal<double>.Finite(ScalarProcessor, ScalarProcessor.Scalar(1.0));
        var listGeneric = ScalarListSignal<double>.Periodic(signal1Generic, signal2Generic, signal3Generic);

        // Verify periodic flag
        Assert.That(listGeneric.IsPeriodic, Is.True);
        Assert.That(listGeneric.Count, Is.EqualTo(listFloat64.Count));

        // Test values at various times
        for (var t = listFloat64.MinTime; t <= listFloat64.MaxTime; t += 0.5)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = listFloat64.GetValue(t);
            var valueGeneric = listGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }

        Debug.WriteLine($"TestPeriodicList_ThreeSignals passed");
    }

    [Test]
    public void TestDerivativeValues()
    {
        // Float64 version
        var signal1Float64 = Float64ScalarSinSignal.FiniteInstance;
        var signal2Float64 = Float64ScalarCosSignal.FiniteInstance;
        var listFloat64 = Float64ScalarListSignal.Finite(signal1Float64, signal2Float64);

        // Generic version
        var signal1Generic = SinScalarSignal<double>.Finite(ScalarProcessor);
        var signal2Generic = CosScalarSignal<double>.Finite(ScalarProcessor);
        var listGeneric = ScalarListSignal<double>.Finite(signal1Generic, signal2Generic);

        // Test derivative values at various times
        for (var t = listFloat64.MinTime; t <= listFloat64.MaxTime; t += 0.5)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var derivFloat64 = listFloat64.GetDerivative1Value(t);
            var derivGeneric = listGeneric.GetDerivative1Value(tScalar).ScalarValue;

            Assert.That(derivGeneric, Is.EqualTo(derivFloat64).Within(Tolerance),
                $"Derivative mismatch at t={t}");
        }

        Debug.WriteLine($"TestDerivativeValues passed");
    }

    [Test]
    public void TestToFiniteSignal()
    {
        // Float64 version
        var signal1Float64 = Float64ScalarSinSignal.FiniteInstance;
        var signal2Float64 = Float64ScalarCosSignal.FiniteInstance;
        var periodicFloat64 = Float64ScalarListSignal.Periodic(signal1Float64, signal2Float64);
        var finiteFloat64 = periodicFloat64.ToFiniteSignal();

        // Generic version
        var signal1Generic = SinScalarSignal<double>.Finite(ScalarProcessor);
        var signal2Generic = CosScalarSignal<double>.Finite(ScalarProcessor);
        var periodicGeneric = ScalarListSignal<double>.Periodic(signal1Generic, signal2Generic);
        var finiteGeneric = periodicGeneric.ToFiniteSignal();

        // Verify periodic/finite flags
        Assert.That(periodicGeneric.IsPeriodic, Is.True);
        Assert.That(finiteGeneric.IsPeriodic, Is.False);

        Debug.WriteLine($"TestToFiniteSignal passed");
    }

    [Test]
    public void TestToPeriodicSignal()
    {
        // Float64 version
        var signal1Float64 = Float64ScalarSinSignal.FiniteInstance;
        var signal2Float64 = Float64ScalarCosSignal.FiniteInstance;
        var finiteFloat64 = Float64ScalarListSignal.Finite(signal1Float64, signal2Float64);
        var periodicFloat64 = finiteFloat64.ToPeriodicSignal();

        // Generic version
        var signal1Generic = SinScalarSignal<double>.Finite(ScalarProcessor);
        var signal2Generic = CosScalarSignal<double>.Finite(ScalarProcessor);
        var finiteGeneric = ScalarListSignal<double>.Finite(signal1Generic, signal2Generic);
        var periodicGeneric = finiteGeneric.ToPeriodicSignal();

        // Verify periodic/finite flags
        Assert.That(finiteGeneric.IsFinite, Is.True);
        Assert.That(periodicGeneric.IsPeriodic, Is.True);

        Debug.WriteLine($"TestToPeriodicSignal passed");
    }

    [Test]
    public void TestBaseSignalsAccess()
    {
        // Float64 version
        var signal1Float64 = Float64ScalarSinSignal.FiniteInstance;
        var signal2Float64 = Float64ScalarCosSignal.FiniteInstance;
        var listFloat64 = Float64ScalarListSignal.Finite(signal1Float64, signal2Float64);

        // Generic version
        var signal1Generic = SinScalarSignal<double>.Finite(ScalarProcessor);
        var signal2Generic = CosScalarSignal<double>.Finite(ScalarProcessor);
        var listGeneric = ScalarListSignal<double>.Finite(signal1Generic, signal2Generic);

        // Verify IReadOnlyList<> interface
        Assert.That(listGeneric.Count, Is.EqualTo(listFloat64.Count));
        Assert.That(listGeneric[0], Is.Not.Null);
        Assert.That(listGeneric[1], Is.Not.Null);

        Debug.WriteLine($"TestBaseSignalsAccess passed");
    }
}
