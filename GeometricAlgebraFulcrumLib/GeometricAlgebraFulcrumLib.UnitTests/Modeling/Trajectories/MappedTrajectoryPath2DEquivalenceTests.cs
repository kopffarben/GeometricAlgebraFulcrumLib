using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Mapped;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Tests für Generic MappedTrajectoryPath2D&lt;TScalar, TValue&gt;
/// Phase 3 Module 6B - Trajectory-to-Path Mapping
/// Tests: Scalar-to-vector mapping, constant trajectory, computed trajectory
/// </summary>
[TestFixture]
public class MappedTrajectoryPath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static ScalarRange<double> TimeRange => ScalarRange<double>.Create(
        ScalarProcessor.Scalar(0),
        ScalarProcessor.Scalar(1)
    );

    #endregion

    #region Mapping Tests (4 tests)

    [Test]
    public void MappedTrajectoryPath2D_ConstantScalarToCircle_ShouldMapCorrectly()
    {
        // Arrange - Constant scalar trajectory with value π/4
        var angle = ScalarProcessor.PiOver4;
        var scalarTrajectory = ConstantScalarSignal<double>.Finite(TimeRange, angle);

        // Map scalar angle to unit circle point: angle → (cos(angle), sin(angle))
        Func<Scalar<double>, LinVector2D<double>> circleMap = a =>
        {
            var angleValue = a.ScalarValue;
            return LinVector2D<double>.Create(
                ScalarProcessor,
                Math.Cos(angleValue),
                Math.Sin(angleValue)
            );
        };

        // Act
        var path = MappedTrajectoryPath2D<double, Scalar<double>>.Create(
            scalarTrajectory,
            circleMap
        );

        // Assert - Point should be at 45 degrees: (√2/2, √2/2)
        var expectedX = Math.Sqrt(2) / 2;
        var expectedY = Math.Sqrt(2) / 2;

        var result = path.GetValue(ScalarProcessor.Scalar(0.5));
        Assert.That(result.X.ScalarValue, Is.EqualTo(expectedX).Within(Tolerance), "X coordinate");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(expectedY).Within(Tolerance), "Y coordinate");
    }

    [Test]
    public void MappedTrajectoryPath2D_ScalarToScaledVector_ShouldScaleCorrectly()
    {
        // Arrange - Linear scalar trajectory: value = 2*t
        ITrajectory<double, Scalar<double>> scalarTrajectory = new TestLinearScalarTrajectory(
            TimeRange,
            false,
            ScalarProcessor
        );

        // Map scalar to vector: s → (s, s*2)
        Func<Scalar<double>, LinVector2D<double>> vectorMap = s =>
            LinVector2D<double>.Create(ScalarProcessor, s.ScalarValue, s.ScalarValue * 2);

        // Act
        var path = MappedTrajectoryPath2D<double, Scalar<double>>.Create(
            scalarTrajectory,
            vectorMap
        );

        // Assert - At t=0.5: value=1.0 → (1, 2)
        var result = path.GetValue(ScalarProcessor.Scalar(0.5));
        Assert.That(result.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X at t=0.5");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Y at t=0.5");

        // At t=1.0: value=2.0 → (2, 4)
        var resultEnd = path.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(resultEnd.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "X at t=1.0");
        Assert.That(resultEnd.Y.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "Y at t=1.0");
    }

    [Test]
    public void MappedTrajectoryPath2D_VectorToProjection_ShouldProjectCorrectly()
    {
        // Arrange - Vector trajectory that produces 3D-like vectors (as scalars: x, y, z components)
        var vectorTrajectory = new TestVectorComponentTrajectory(
            TimeRange,
            false,
            ScalarProcessor
        );

        // Map 3 components to 2D projection: (x,y,z) → (x, y)
        Func<TestVector3Components, LinVector2D<double>> projectionMap = v =>
            LinVector2D<double>.Create(ScalarProcessor, v.X, v.Y);

        // Act
        var path = MappedTrajectoryPath2D<double, TestVector3Components>.Create(
            vectorTrajectory,
            projectionMap
        );

        // Assert - At t=0: (0, 0, 0) → (0, 0)
        var resultStart = path.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(resultStart.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at start");
        Assert.That(resultStart.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at start");

        // At t=1: (1, 2, 3) → (1, 2)
        var resultEnd = path.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(resultEnd.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X at end");
        Assert.That(resultEnd.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Y at end");
    }

    [Test]
    public void MappedTrajectoryPath2D_TimeRangeAndPeriodicity_ShouldPreserve()
    {
        // Arrange - Custom time range and periodic
        var customRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(2),
            ScalarProcessor.Scalar(5)
        );
        var scalarTrajectory = ConstantScalarSignal<double>.Periodic(customRange, ScalarProcessor.Scalar(1.0));

        Func<Scalar<double>, LinVector2D<double>> identityMap = s =>
            LinVector2D<double>.Create(ScalarProcessor, s.ScalarValue, s.ScalarValue);

        // Act
        var path = MappedTrajectoryPath2D<double, Scalar<double>>.Create(
            scalarTrajectory,
            identityMap
        );

        // Assert
        Assert.That(path.TimeRange.MinValue.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Min time");
        Assert.That(path.TimeRange.MaxValue.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Max time");
        Assert.That(path.IsPeriodic, Is.True, "Should be periodic");
    }

    #endregion

    #region Test Helper Classes

    /// <summary>
    /// Simple linear scalar trajectory for testing: value = 2*t
    /// </summary>
    private class TestLinearScalarTrajectory : ITrajectory<double, Scalar<double>>
    {
        public ScalarRange<double> TimeRange { get; }
        public bool IsPeriodic { get; }
        public Scalar<double> MinTime => TimeRange.MinValue;
        public Scalar<double> MaxTime => TimeRange.MaxValue;
        public Scalar<double> MidTime => TimeRange.MidValue;
        public Scalar<double> TimeRangeLength => TimeRange.Length;
        public Scalar<double> ValueAtMinTime => GetValue(MinTime);
        public Scalar<double> ValueAtMidTime => GetValue(MidTime);
        public Scalar<double> ValueAtMaxTime => GetValue(MaxTime);

        private readonly IScalarProcessor<double> _processor;

        public TestLinearScalarTrajectory(ScalarRange<double> timeRange, bool isPeriodic, IScalarProcessor<double> processor)
        {
            TimeRange = timeRange;
            IsPeriodic = isPeriodic;
            _processor = processor;
        }

        public Scalar<double> GetValue(Scalar<double> t)
        {
            return _processor.Scalar(t.ScalarValue * 2);
        }

        public bool IsValid() => true;
        public ITrajectory<double> ToFinite() => this;
        public ITrajectory<double> ToPeriodic() => new TestLinearScalarTrajectory(TimeRange, true, _processor);
    }

    /// <summary>
    /// Test vector component structure
    /// </summary>
    private record TestVector3Components(double X, double Y, double Z);

    /// <summary>
    /// Test trajectory that produces 3-component vectors
    /// </summary>
    private class TestVectorComponentTrajectory : ITrajectory<double, TestVector3Components>
    {
        public ScalarRange<double> TimeRange { get; }
        public bool IsPeriodic { get; }
        public Scalar<double> MinTime => TimeRange.MinValue;
        public Scalar<double> MaxTime => TimeRange.MaxValue;
        public Scalar<double> MidTime => TimeRange.MidValue;
        public Scalar<double> TimeRangeLength => TimeRange.Length;
        public TestVector3Components ValueAtMinTime => GetValue(MinTime);
        public TestVector3Components ValueAtMidTime => GetValue(MidTime);
        public TestVector3Components ValueAtMaxTime => GetValue(MaxTime);

        private readonly IScalarProcessor<double> _processor;

        public TestVectorComponentTrajectory(ScalarRange<double> timeRange, bool isPeriodic, IScalarProcessor<double> processor)
        {
            TimeRange = timeRange;
            IsPeriodic = isPeriodic;
            _processor = processor;
        }

        public TestVector3Components GetValue(Scalar<double> t)
        {
            var tValue = t.ScalarValue;
            return new TestVector3Components(tValue, tValue * 2, tValue * 3);
        }

        public bool IsValid() => true;
        public ITrajectory<double> ToFinite() => this;
        public ITrajectory<double> ToPeriodic() => new TestVectorComponentTrajectory(TimeRange, true, _processor);
    }

    #endregion
}
