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
public sealed class ScalarSegmentSignalEquivalenceTests
{
    private static readonly ScalarProcessorOfFloat64 ScalarProcessor =
        ScalarProcessorOfFloat64.Instance;

    private const double Tolerance = 1e-12;

    [Test]
    public void TestFiniteSegment_SinSignal()
    {
        // Float64 version
        var baseSignalFloat64 = Float64ScalarSinSignal.FiniteInstance;
        var segmentFloat64 = Float64ScalarSegmentSignal.Finite(0.5, 2.5, baseSignalFloat64);

        // Generic version
        var baseSignalGeneric = SinScalarSignal<double>.Finite(ScalarProcessor);
        var segmentGeneric = ScalarSegmentSignal<double>.Finite(
            ScalarProcessor.Scalar(0.5),
            ScalarProcessor.Scalar(2.5),
            baseSignalGeneric
        );

        // Verify time ranges match
        Assert.That(segmentGeneric.MinTime.ScalarValue, Is.EqualTo(segmentFloat64.MinTime).Within(Tolerance));
        Assert.That(segmentGeneric.MaxTime.ScalarValue, Is.EqualTo(segmentFloat64.MaxTime).Within(Tolerance));

        // Test values at various times
        for (var t = -Math.PI; t <= Math.PI; t += 0.5)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = segmentFloat64.GetValue(t);
            var valueGeneric = segmentGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }

        Debug.WriteLine($"TestFiniteSegment_SinSignal passed");
    }

    [Test]
    public void TestPeriodicSegment_CosSignal()
    {
        // Float64 version
        var baseSignalFloat64 = Float64ScalarCosSignal.FiniteInstance;
        var segmentFloat64 = Float64ScalarSegmentSignal.Periodic(0.5, 2.5, baseSignalFloat64);

        // Generic version
        var baseSignalGeneric = CosScalarSignal<double>.Finite(ScalarProcessor);
        var segmentGeneric = ScalarSegmentSignal<double>.Periodic(
            ScalarProcessor.Scalar(0.5),
            ScalarProcessor.Scalar(2.5),
            baseSignalGeneric
        );

        // Verify time ranges match
        Assert.That(segmentGeneric.IsPeriodic, Is.EqualTo(segmentFloat64.IsPeriodic));

        // Test values at various times
        for (var t = -Math.PI; t <= Math.PI; t += 0.5)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = segmentFloat64.GetValue(t);
            var valueGeneric = segmentGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }

        Debug.WriteLine($"TestPeriodicSegment_CosSignal passed");
    }

    [Test]
    public void TestToFiniteSignal()
    {
        // Float64 version
        var baseSignalFloat64 = Float64ScalarSinSignal.FiniteInstance;
        var periodicFloat64 = Float64ScalarSegmentSignal.Periodic(0.5, 2.5, baseSignalFloat64);
        var finiteFloat64 = periodicFloat64.ToFiniteSignal();

        // Generic version
        var baseSignalGeneric = SinScalarSignal<double>.Finite(ScalarProcessor);
        var periodicGeneric = ScalarSegmentSignal<double>.Periodic(
            ScalarProcessor.Scalar(0.5),
            ScalarProcessor.Scalar(2.5),
            baseSignalGeneric
        );
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
        var baseSignalFloat64 = Float64ScalarSinSignal.FiniteInstance;
        var finiteFloat64 = Float64ScalarSegmentSignal.Finite(0.5, 2.5, baseSignalFloat64);
        var periodicFloat64 = finiteFloat64.ToPeriodicSignal();

        // Generic version
        var baseSignalGeneric = SinScalarSignal<double>.Finite(ScalarProcessor);
        var finiteGeneric = ScalarSegmentSignal<double>.Finite(
            ScalarProcessor.Scalar(0.5),
            ScalarProcessor.Scalar(2.5),
            baseSignalGeneric
        );
        var periodicGeneric = finiteGeneric.ToPeriodicSignal();

        // Verify periodic/finite flags
        Assert.That(finiteGeneric.IsFinite, Is.True);
        Assert.That(periodicGeneric.IsPeriodic, Is.True);

        Debug.WriteLine($"TestToPeriodicSignal passed");
    }
}
