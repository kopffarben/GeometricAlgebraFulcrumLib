using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;
using NUnit.Framework;
using ConstantPath3D = GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic.ConstantPath3D<double>;
using ConstantScalarSignal = GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic.ConstantScalarSignal<double>;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public class MappedTrajectoryPath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [OneTimeSetUp]
    public void ClassInit()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void MappedTrajectoryPath3D_MapScalarToXComponent_ShouldWorkCorrectly()
    {
        // Create a constant scalar signal returning 5.0
        var scalarSignal = ConstantScalarSignal.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            ScalarProcessor.ScalarFromNumber(5.0)
        );

        // Map scalar to (scalar, 0, 0)
        var mappedPath = MappedTrajectoryPath3D<double, Scalar<double>>.Create(
            scalarSignal,
            scalar => LinVector3D<double>.Create(ScalarProcessor, scalar.ScalarValue, 0, 0)
        );

        // At any time, should return (5, 0, 0)
        var value = mappedPath.GetValue(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(value.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "X component");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y component");
        Assert.That(value.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Z component");
    }

    [Test]
    public void MappedTrajectoryPath3D_MapScalarToSymmetricVector_ShouldWorkCorrectly()
    {
        // Create a constant scalar signal returning 3.0
        var scalarSignal = ConstantScalarSignal.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            ScalarProcessor.ScalarFromNumber(3.0)
        );

        // Map scalar to (scalar, scalar, scalar)
        var mappedPath = MappedTrajectoryPath3D<double, Scalar<double>>.Create(
            scalarSignal,
            scalar => LinVector3D<double>.Create(ScalarProcessor, scalar.ScalarValue, scalar.ScalarValue, scalar.ScalarValue)
        );

        // At any time, should return (3, 3, 3)
        var value = mappedPath.GetValue(ScalarProcessor.Zero);

        Assert.That(value.X.ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
        Assert.That(value.Y.ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
        Assert.That(value.Z.ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
    }

    [Test]
    public void MappedTrajectoryPath3D_MapVectorToScaled_ShouldWorkCorrectly()
    {
        // Create a constant vector path at (2, 3, 4)
        var vectorPath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 2, 3, 4)
        );

        // Map vector to 2 * vector
        var mappedPath = MappedTrajectoryPath3D<double, LinVector3D<double>>.Create(
            vectorPath,
            vec => LinVector3D<double>.Create(ScalarProcessor, vec.X.ScalarValue * 2, vec.Y.ScalarValue * 2, vec.Z.ScalarValue * 2)
        );

        // At any time, should return (4, 6, 8)
        var value = mappedPath.GetValue(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(value.X.ScalarValue, Is.EqualTo(4.0).Within(Tolerance));
        Assert.That(value.Y.ScalarValue, Is.EqualTo(6.0).Within(Tolerance));
        Assert.That(value.Z.ScalarValue, Is.EqualTo(8.0).Within(Tolerance));
    }

    [Test]
    public void MappedTrajectoryPath3D_MapVectorToPermuted_ShouldWorkCorrectly()
    {
        // Create a constant vector path at (1, 2, 3)
        var vectorPath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        // Map vector (x,y,z) to (z,x,y) - cyclic permutation
        var mappedPath = MappedTrajectoryPath3D<double, LinVector3D<double>>.Create(
            vectorPath,
            vec => LinVector3D<double>.Create(ScalarProcessor, vec.Z.ScalarValue, vec.X.ScalarValue, vec.Y.ScalarValue)
        );

        // Should return (3, 1, 2)
        var value = mappedPath.GetValue(ScalarProcessor.Zero);

        Assert.That(value.X.ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
        Assert.That(value.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(value.Z.ScalarValue, Is.EqualTo(2.0).Within(Tolerance));
    }

    [Test]
    public void MappedTrajectoryPath3D_MapLineSegmentThroughFunction_ShouldWorkCorrectly()
    {
        // Create a line segment from (0,0,0) to (1,1,1)
        var linePath = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1)
        );

        // Map by doubling each component
        var mappedPath = MappedTrajectoryPath3D<double, LinVector3D<double>>.Create(
            linePath,
            vec => LinVector3D<double>.Create(ScalarProcessor, vec.X.ScalarValue * 2, vec.Y.ScalarValue * 2, vec.Z.ScalarValue * 2)
        );

        // At t=0: (0,0,0) * 2 = (0,0,0)
        var value0 = mappedPath.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(value0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(value0.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));

        // At t=0.5: (0.5,0.5,0.5) * 2 = (1,1,1)
        var value05 = mappedPath.GetValue(ScalarProcessor.ScalarFromNumber(0.5));
        Assert.That(value05.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(value05.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(value05.Z.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));

        // At t=1: (1,1,1) * 2 = (2,2,2)
        var value1 = mappedPath.GetValue(ScalarProcessor.One);
        Assert.That(value1.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance));
        Assert.That(value1.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance));
        Assert.That(value1.Z.ScalarValue, Is.EqualTo(2.0).Within(Tolerance));
    }

    [Test]
    public void MappedTrajectoryPath3D_IdentityMapping_ShouldPreserveOriginal()
    {
        // Create a line segment
        var linePath = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 2, 3, 4),
            LinVector3D<double>.Create(ScalarProcessor, 5, 6, 7)
        );

        // Identity mapping (no transformation)
        var mappedPath = MappedTrajectoryPath3D<double, LinVector3D<double>>.Create(
            linePath,
            vec => vec
        );

        // Values should match original path exactly
        var t = ScalarProcessor.ScalarFromNumber(0.5);
        var originalValue = linePath.GetValue(t);
        var mappedValue = mappedPath.GetValue(t);

        Assert.That(mappedValue.X.ScalarValue, Is.EqualTo(originalValue.X.ScalarValue).Within(Tolerance));
        Assert.That(mappedValue.Y.ScalarValue, Is.EqualTo(originalValue.Y.ScalarValue).Within(Tolerance));
        Assert.That(mappedValue.Z.ScalarValue, Is.EqualTo(originalValue.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void MappedTrajectoryPath3D_IsValid_WhenBaseValid_ShouldReturnTrue()
    {
        var vectorPath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var mappedPath = MappedTrajectoryPath3D<double, LinVector3D<double>>.Create(
            vectorPath,
            vec => LinVector3D<double>.Create(ScalarProcessor, vec.X.ScalarValue * 2, vec.Y.ScalarValue * 2, vec.Z.ScalarValue * 2)
        );

        Assert.That(mappedPath.IsValid(), Is.True);
        Assert.That(vectorPath.IsValid(), Is.True);
    }

    [Test]
    public void MappedTrajectoryPath3D_ToFinitePath_WhenFinite_ShouldReturnSelf()
    {
        var vectorPath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var mappedPath = MappedTrajectoryPath3D<double, LinVector3D<double>>.Create(
            vectorPath,
            vec => vec
        );

        var finitePath = mappedPath.ToFinitePath();

        Assert.That(finitePath, Is.SameAs(mappedPath), "Should return self when already finite");
        Assert.That(mappedPath.IsFinite, Is.True);
        Assert.That(mappedPath.IsPeriodic, Is.False);
    }

    [Test]
    public void MappedTrajectoryPath3D_ToPeriodicPath_WhenFinite_ShouldReturnNewInstance()
    {
        var vectorPath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 2, 3, 4)
        );

        var mappedPath = MappedTrajectoryPath3D<double, LinVector3D<double>>.Create(
            vectorPath,
            vec => vec * ScalarProcessor.ScalarFromNumber(2)
        );

        var periodicPath = mappedPath.ToPeriodicPath();

        Assert.That(periodicPath, Is.Not.SameAs(mappedPath), "Should return new instance");
        Assert.That(periodicPath.IsPeriodic, Is.True);
        Assert.That(periodicPath.IsFinite, Is.False);

        // Values should still match through mapping
        var t = ScalarProcessor.ScalarFromNumber(0.5);
        var value1 = mappedPath.GetValue(t);
        var value2 = periodicPath.GetValue(t);

        Assert.That(value2.X.ScalarValue, Is.EqualTo(value1.X.ScalarValue).Within(Tolerance));
        Assert.That(value2.Y.ScalarValue, Is.EqualTo(value1.Y.ScalarValue).Within(Tolerance));
        Assert.That(value2.Z.ScalarValue, Is.EqualTo(value1.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void MappedTrajectoryPath3D_ToPeriodicPath_WhenPeriodic_ShouldReturnSelf()
    {
        // First create a finite path, then convert to periodic
        var finitePath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var periodicVectorPath = (ParametricPath3D<double>)finitePath.ToPeriodicPath();

        var mappedPath = MappedTrajectoryPath3D<double, LinVector3D<double>>.Create(
            periodicVectorPath,
            vec => vec
        );

        var periodicPath = mappedPath.ToPeriodicPath();

        Assert.That(periodicPath, Is.SameAs(mappedPath), "Should return self when already periodic");
        Assert.That(mappedPath.IsPeriodic, Is.True);
    }

    [Test]
    public void MappedTrajectoryPath3D_GetDerivative1Value_ShouldReturnZero()
    {
        // Derivatives cannot be computed through arbitrary mapping function
        var linePath = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1)
        );

        var mappedPath = MappedTrajectoryPath3D<double, LinVector3D<double>>.Create(
            linePath,
            vec => vec * ScalarProcessor.ScalarFromNumber(2)
        );

        var deriv = mappedPath.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(0.5));

        // Should return zero vector since derivative cannot be computed
        Assert.That(deriv.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "First derivative X should be zero");
        Assert.That(deriv.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "First derivative Y should be zero");
        Assert.That(deriv.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "First derivative Z should be zero");
    }

    [Test]
    public void MappedTrajectoryPath3D_GetDerivative2Value_ShouldReturnZero()
    {
        // Derivatives cannot be computed through arbitrary mapping function
        var linePath = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1)
        );

        var mappedPath = MappedTrajectoryPath3D<double, LinVector3D<double>>.Create(
            linePath,
            vec => LinVector3D<double>.Create(ScalarProcessor, vec.X.ScalarValue * 3, vec.Y.ScalarValue * 3, vec.Z.ScalarValue * 3)
        );

        var deriv2 = mappedPath.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(0.5));

        // Should return zero vector since second derivative cannot be computed
        Assert.That(deriv2.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative X should be zero");
        Assert.That(deriv2.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative Y should be zero");
        Assert.That(deriv2.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative Z should be zero");
    }

    [Test]
    public void MappedTrajectoryPath3D_TimeRangePreservation_ShouldMatchBase()
    {
        // Create a path with specific time range
        var vectorPath = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1),
            LinVector3D<double>.Create(ScalarProcessor, 2, 2, 2)
        );

        var mappedPath = MappedTrajectoryPath3D<double, LinVector3D<double>>.Create(
            vectorPath,
            vec => vec
        );

        // Time range should match base trajectory
        Assert.That(mappedPath.MinTime.ScalarValue, Is.EqualTo(vectorPath.MinTime.ScalarValue).Within(Tolerance));
        Assert.That(mappedPath.MaxTime.ScalarValue, Is.EqualTo(vectorPath.MaxTime.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void MappedTrajectoryPath3D_ComplexMapping_ShouldWorkCorrectly()
    {
        // Map scalar to circular path: scalar -> (cos(scalar), sin(scalar), 0)
        var scalarSignal = ConstantScalarSignal.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            ScalarProcessor.ScalarFromNumber(Math.PI / 4)  // 45 degrees
        );

        var mappedPath = MappedTrajectoryPath3D<double, Scalar<double>>.Create(
            scalarSignal,
            scalar =>
            {
                var angle = scalar.ScalarValue;
                return LinVector3D<double>.Create(
                    ScalarProcessor,
                    Math.Cos(angle),
                    Math.Sin(angle),
                    0
                );
            }
        );

        var value = mappedPath.GetValue(ScalarProcessor.Zero);

        // At π/4: cos(π/4) ≈ 0.707, sin(π/4) ≈ 0.707
        Assert.That(value.X.ScalarValue, Is.EqualTo(Math.Cos(Math.PI / 4)).Within(Tolerance), "X should be cos(π/4)");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(Math.Sin(Math.PI / 4)).Within(Tolerance), "Y should be sin(π/4)");
        Assert.That(value.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Z should be 0");
    }

    [Test]
    public void MappedTrajectoryPath3D_BaseTrajectoryProperty_ShouldReturnOriginal()
    {
        var vectorPath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var mappedPath = MappedTrajectoryPath3D<double, LinVector3D<double>>.Create(
            vectorPath,
            vec => vec
        );

        Assert.That(mappedPath.BaseTrajectory, Is.SameAs(vectorPath), "BaseTrajectory should reference original");
    }

    [Test]
    public void MappedTrajectoryPath3D_ValueMapProperty_ShouldNotBeNull()
    {
        var vectorPath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        Func<LinVector3D<double>, LinVector3D<double>> mapFunc = vec => LinVector3D<double>.Create(ScalarProcessor, vec.X.ScalarValue * 2, vec.Y.ScalarValue * 2, vec.Z.ScalarValue * 2);

        var mappedPath = MappedTrajectoryPath3D<double, LinVector3D<double>>.Create(
            vectorPath,
            mapFunc
        );

        Assert.That(mappedPath.ValueMap, Is.Not.Null, "ValueMap should not be null");
        Assert.That(mappedPath.ValueMap, Is.SameAs(mapFunc), "ValueMap should reference the provided mapping function");
    }
}
