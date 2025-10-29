using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Mapped;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public sealed class ScalarMappedTrajectorySignalEquivalenceTests
{
    private const double Tolerance = 1e-12;

    private IScalarProcessor<double> ScalarProcessor { get; }
        = ScalarProcessorOfFloat64.Instance;

    // Helper: Simple Float64 trajectory for testing
    private sealed class SimpleFloat64VectorTrajectory : Float64Trajectory<(double x, double y, double z)>
    {
        public SimpleFloat64VectorTrajectory()
            : base(Float64ScalarRange.Create(-1, 1), false)
        {
        }

        public override bool IsValid() => true;

        public override IFloat64Trajectory ToFinite() => this;

        public override IFloat64Trajectory ToPeriodic()
            => new SimpleFloat64VectorTrajectory { IsPeriodic = true };

        private bool IsPeriodic { get; init; }

        public override (double x, double y, double z) GetValue(double t)
        {
            // Simple trajectory: (t, t^2, sin(t))
            return (t, t * t, Math.Sin(t));
        }
    }

    // Helper: Simple Generic trajectory for testing
    private sealed class SimpleGenericVectorTrajectory<T> : Trajectory<T, (Scalar<T> x, Scalar<T> y, Scalar<T> z)>
    {
        private readonly IScalarProcessor<T> _processor;

        public SimpleGenericVectorTrajectory(IScalarProcessor<T> processor)
            : base(ScalarRange<T>.NegativeOneToOne(processor), false)
        {
            _processor = processor;
        }

        public override bool IsValid() => true;

        public override ITrajectory<T> ToFinite() => this;

        public override ITrajectory<T> ToPeriodic()
            => new SimpleGenericVectorTrajectory<T>(_processor) { IsPeriodic = true };

        private bool IsPeriodic { get; init; }

        public override (Scalar<T> x, Scalar<T> y, Scalar<T> z) GetValue(Scalar<T> t)
        {
            // Simple trajectory: (t, t^2, sin(t))
            var x = t;
            var y = t * t;
            var z = _processor.Sin(t.ScalarValue).ToScalar();
            return (x, y, z);
        }
    }

    [Test]
    public void TestMapToX_Component()
    {
        // Float64 version - extract X component
        var trajectoryFloat64 = new SimpleFloat64VectorTrajectory();
        var signalFloat64 = Float64ScalarMappedTrajectorySignal<(double, double, double)>.Create(
            trajectoryFloat64,
            v => v.Item1  // Extract X
        );

        // Generic version
        var trajectoryGeneric = new SimpleGenericVectorTrajectory<double>(ScalarProcessor);
        var signalGeneric = ScalarMappedTrajectorySignal<double, (Scalar<double>, Scalar<double>, Scalar<double>)>.Create(
            trajectoryGeneric,
            v => v.Item1  // Extract X
        );

        // Test values at various times
        for (var t = -1.0; t <= 1.0; t += 0.25)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = signalFloat64.GetValue(t);
            var valueGeneric = signalGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }
    }

    [Test]
    public void TestMapToY_Component()
    {
        // Float64 version - extract Y component (t^2)
        var trajectoryFloat64 = new SimpleFloat64VectorTrajectory();
        var signalFloat64 = Float64ScalarMappedTrajectorySignal<(double, double, double)>.Create(
            trajectoryFloat64,
            v => v.Item2  // Extract Y
        );

        // Generic version
        var trajectoryGeneric = new SimpleGenericVectorTrajectory<double>(ScalarProcessor);
        var signalGeneric = ScalarMappedTrajectorySignal<double, (Scalar<double>, Scalar<double>, Scalar<double>)>.Create(
            trajectoryGeneric,
            v => v.Item2  // Extract Y
        );

        // Test values at various times
        for (var t = -1.0; t <= 1.0; t += 0.25)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = signalFloat64.GetValue(t);
            var valueGeneric = signalGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }
    }

    [Test]
    public void TestMapToZ_Component()
    {
        // Float64 version - extract Z component (sin(t))
        var trajectoryFloat64 = new SimpleFloat64VectorTrajectory();
        var signalFloat64 = Float64ScalarMappedTrajectorySignal<(double, double, double)>.Create(
            trajectoryFloat64,
            v => v.Item3  // Extract Z
        );

        // Generic version
        var trajectoryGeneric = new SimpleGenericVectorTrajectory<double>(ScalarProcessor);
        var signalGeneric = ScalarMappedTrajectorySignal<double, (Scalar<double>, Scalar<double>, Scalar<double>)>.Create(
            trajectoryGeneric,
            v => v.Item3  // Extract Z
        );

        // Test values at various times
        for (var t = -1.0; t <= 1.0; t += 0.25)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = signalFloat64.GetValue(t);
            var valueGeneric = signalGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }
    }

    [Test]
    public void TestMapWithCustomFunction_Magnitude()
    {
        // Float64 version - compute magnitude
        var trajectoryFloat64 = new SimpleFloat64VectorTrajectory();
        var signalFloat64 = Float64ScalarMappedTrajectorySignal<(double, double, double)>.Create(
            trajectoryFloat64,
            v => Math.Sqrt(v.Item1 * v.Item1 + v.Item2 * v.Item2 + v.Item3 * v.Item3)
        );

        // Generic version
        var trajectoryGeneric = new SimpleGenericVectorTrajectory<double>(ScalarProcessor);
        var signalGeneric = ScalarMappedTrajectorySignal<double, (Scalar<double>, Scalar<double>, Scalar<double>)>.Create(
            trajectoryGeneric,
            v =>
            {
                var mag2 = v.Item1 * v.Item1 + v.Item2 * v.Item2 + v.Item3 * v.Item3;
                return ScalarProcessor.Sqrt(mag2.ScalarValue).ToScalar();
            }
        );

        // Test values at various times
        for (var t = -1.0; t <= 1.0; t += 0.25)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var valueFloat64 = signalFloat64.GetValue(t);
            var valueGeneric = signalGeneric.GetValue(tScalar).ScalarValue;

            Assert.That(valueGeneric, Is.EqualTo(valueFloat64).Within(Tolerance),
                $"Value mismatch at t={t}");
        }
    }

    [Test]
    public void TestTimeRange()
    {
        var trajectoryFloat64 = new SimpleFloat64VectorTrajectory();
        var signalFloat64 = Float64ScalarMappedTrajectorySignal<(double, double, double)>.Create(
            trajectoryFloat64,
            v => v.Item1
        );

        var trajectoryGeneric = new SimpleGenericVectorTrajectory<double>(ScalarProcessor);
        var signalGeneric = ScalarMappedTrajectorySignal<double, (Scalar<double>, Scalar<double>, Scalar<double>)>.Create(
            trajectoryGeneric,
            v => v.Item1
        );

        // Verify time ranges match
        Assert.That(signalGeneric.MinTime.ScalarValue, Is.EqualTo(signalFloat64.MinTime).Within(Tolerance));
        Assert.That(signalGeneric.MaxTime.ScalarValue, Is.EqualTo(signalFloat64.MaxTime).Within(Tolerance));
        Assert.That(signalGeneric.IsPeriodic, Is.EqualTo(signalFloat64.IsPeriodic));
    }

    [Test]
    public void TestIsValid()
    {
        var trajectoryGeneric = new SimpleGenericVectorTrajectory<double>(ScalarProcessor);
        var signalGeneric = ScalarMappedTrajectorySignal<double, (Scalar<double>, Scalar<double>, Scalar<double>)>.Create(
            trajectoryGeneric,
            v => v.Item1
        );

        Assert.That(signalGeneric.IsValid(), Is.True);
    }

    [Test]
    public void TestToFiniteSignal()
    {
        var trajectoryGeneric = new SimpleGenericVectorTrajectory<double>(ScalarProcessor);
        var signalGeneric = ScalarMappedTrajectorySignal<double, (Scalar<double>, Scalar<double>, Scalar<double>)>.Create(
            trajectoryGeneric,
            v => v.Item1
        );

        var finiteSignal = signalGeneric.ToFiniteSignal();

        Assert.That(finiteSignal.IsFinite, Is.True);
        Assert.That(finiteSignal.IsPeriodic, Is.False);

        // Values should match
        for (var t = -1.0; t <= 1.0; t += 0.3)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var originalValue = signalGeneric.GetValue(tScalar).ScalarValue;
            var finiteValue = finiteSignal.GetValue(tScalar).ScalarValue;

            Assert.That(finiteValue, Is.EqualTo(originalValue).Within(Tolerance));
        }
    }

    [Test]
    public void TestToPeriodicSignal()
    {
        var trajectoryGeneric = new SimpleGenericVectorTrajectory<double>(ScalarProcessor);
        var signalGeneric = ScalarMappedTrajectorySignal<double, (Scalar<double>, Scalar<double>, Scalar<double>)>.Create(
            trajectoryGeneric,
            v => v.Item1
        );

        var periodicSignal = signalGeneric.ToPeriodicSignal();

        Assert.That(periodicSignal.IsFinite, Is.False);
        Assert.That(periodicSignal.IsPeriodic, Is.True);

        // Values should match
        for (var t = -1.0; t <= 1.0; t += 0.3)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var originalValue = signalGeneric.GetValue(tScalar).ScalarValue;
            var periodicValue = periodicSignal.GetValue(tScalar).ScalarValue;

            Assert.That(periodicValue, Is.EqualTo(originalValue).Within(Tolerance));
        }
    }

    [Test]
    public void TestBaseTrajectoryProperty()
    {
        var trajectoryGeneric = new SimpleGenericVectorTrajectory<double>(ScalarProcessor);
        var signalGeneric = ScalarMappedTrajectorySignal<double, (Scalar<double>, Scalar<double>, Scalar<double>)>.Create(
            trajectoryGeneric,
            v => v.Item1
        );

        Assert.That(signalGeneric.BaseTrajectory, Is.SameAs(trajectoryGeneric));
    }

    [Test]
    public void TestValueMapProperty()
    {
        var trajectoryGeneric = new SimpleGenericVectorTrajectory<double>(ScalarProcessor);
        Func<(Scalar<double>, Scalar<double>, Scalar<double>), Scalar<double>> mapFunc = v => v.y;

        var signalGeneric = ScalarMappedTrajectorySignal<double, (Scalar<double>, Scalar<double>, Scalar<double>)>.Create(
            trajectoryGeneric,
            mapFunc
        );

        Assert.That(signalGeneric.ValueMap, Is.SameAs(mapFunc));
    }

    [Test]
    public void TestEdgeCases_MinMaxTime()
    {
        var trajectoryFloat64 = new SimpleFloat64VectorTrajectory();
        var signalFloat64 = Float64ScalarMappedTrajectorySignal<(double, double, double)>.Create(
            trajectoryFloat64,
            v => v.Item1
        );

        var trajectoryGeneric = new SimpleGenericVectorTrajectory<double>(ScalarProcessor);
        var signalGeneric = ScalarMappedTrajectorySignal<double, (Scalar<double>, Scalar<double>, Scalar<double>)>.Create(
            trajectoryGeneric,
            v => v.Item1
        );

        // Test at exact MinTime
        var minValueFloat64 = signalFloat64.GetValue(signalFloat64.MinTime);
        var minValueGeneric = signalGeneric.GetValue(signalGeneric.MinTime).ScalarValue;
        Assert.That(minValueGeneric, Is.EqualTo(minValueFloat64).Within(Tolerance));

        // Test at exact MaxTime
        var maxValueFloat64 = signalFloat64.GetValue(signalFloat64.MaxTime);
        var maxValueGeneric = signalGeneric.GetValue(signalGeneric.MaxTime).ScalarValue;
        Assert.That(maxValueGeneric, Is.EqualTo(maxValueFloat64).Within(Tolerance));
    }
}
