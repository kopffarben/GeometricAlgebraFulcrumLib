using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;
using NUnit.Framework;
using LineSegmentPath3D = GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic.LineSegmentPath3D<double>;
using ConstantPath3D = GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic.ConstantPath3D<double>;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Tests for AffineMappedTimePath3D<T> - verifies time remapping with affine transformations.
/// Time transform: t_new = scaling * t_old + offset
/// </summary>
[TestFixture]
public class AffineMappedTimePath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [OneTimeSetUp]
    public void ClassInit()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void AffineMappedTimePath3D_IdentityTransform_PreservesOriginal()
    {
        // Line from (0,0,0) to (1,1,1), time [0,1]
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1)
        );

        // Identity: t_new = 1*t_old + 0 = t_old
        var mappedPath = AffineMappedTimePath3D<double>.Create(
            basePath,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        // Values should match exactly
        var t = ScalarProcessor.ScalarFromNumber(0.5);
        var baseValue = basePath.GetValue(t);
        var mappedValue = mappedPath.GetValue(t);

        Assert.That(mappedValue.X.ScalarValue, Is.EqualTo(baseValue.X.ScalarValue).Within(Tolerance), "X at t=0.5");
        Assert.That(mappedValue.Y.ScalarValue, Is.EqualTo(baseValue.Y.ScalarValue).Within(Tolerance), "Y at t=0.5");
        Assert.That(mappedValue.Z.ScalarValue, Is.EqualTo(baseValue.Z.ScalarValue).Within(Tolerance), "Z at t=0.5");
    }

    [Test]
    public void AffineMappedTimePath3D_ScalingOnly_DoublesSpeed()
    {
        // Line from (0,0,0) to (10,0,0), time [0,1]
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 10, 0, 0)
        );

        // Scale by 2: t_new = 2*t_old (covers [0,1] in base at new time [0,2])
        // Inverse: t_old = t_new / 2
        var mappedPath = AffineMappedTimePath3D<double>.CreateScaling(
            basePath,
            ScalarProcessor.ScalarFromNumber(2.0)
        );

        // At new t=1: base t=0.5, point=(5,0,0)
        var value = mappedPath.GetValue(ScalarProcessor.One);
        Assert.That(value.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "X at t=1");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=1");
        Assert.That(value.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Z at t=1");

        // Derivative should be halved (chain rule: d/dt_new = d/dt_old * dt_old/dt_new = deriv * 0.5)
        var deriv = mappedPath.GetDerivative1Value(ScalarProcessor.One);
        Assert.That(deriv.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Derivative X");
    }

    [Test]
    public void AffineMappedTimePath3D_OffsetOnly_ShiftsTime()
    {
        // Line from (0,0,0) to (1,1,1), time [0,1]
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1)
        );

        // Offset by 2: t_new = t_old + 2, so new time range [2,3]
        // Inverse: t_old = t_new - 2
        var mappedPath = AffineMappedTimePath3D<double>.CreateTranslation(
            basePath,
            ScalarProcessor.ScalarFromNumber(2.0)
        );

        // At new t=2.5: base t=0.5, point=(0.5,0.5,0.5)
        var value = mappedPath.GetValue(ScalarProcessor.ScalarFromNumber(2.5));
        Assert.That(value.X.ScalarValue, Is.EqualTo(0.5).Within(Tolerance), "X at t=2.5");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(0.5).Within(Tolerance), "Y at t=2.5");
        Assert.That(value.Z.ScalarValue, Is.EqualTo(0.5).Within(Tolerance), "Z at t=2.5");

        // Derivative unchanged (offset doesn't affect derivative)
        var deriv = mappedPath.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(2.5));
        Assert.That(deriv.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Derivative X");
    }

    [Test]
    public void AffineMappedTimePath3D_CombinedScalingAndOffset()
    {
        // Line from (0,0,0) to (6,0,0), time [0,1]
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 6, 0, 0)
        );

        // Transform: t_new = 3*t_old + 1, so new time [1,4]
        // Inverse: t_old = (t_new - 1) / 3
        var mappedPath = AffineMappedTimePath3D<double>.Create(
            basePath,
            ScalarProcessor.ScalarFromNumber(3.0),
            ScalarProcessor.One
        );

        // At new t=2.5: base t=(2.5-1)/3=0.5, point=(3,0,0)
        var value = mappedPath.GetValue(ScalarProcessor.ScalarFromNumber(2.5));
        Assert.That(value.X.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "X at t=2.5");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=2.5");

        // Derivative: base deriv=6, scaled by 1/3 (inverse scaling)
        var deriv = mappedPath.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(2.5));
        Assert.That(deriv.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Derivative X (6 * 1/3)");
    }

    [Test]
    public void AffineMappedTimePath3D_NegativeScaling_ReversesTime()
    {
        // Line from (0,0,0) to (4,0,0), time [0,1]
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 4, 0, 0)
        );

        // Reverse: t_new = -1*t_old + 0, new time [0,-1] or effectively [-1,0]
        // Inverse: t_old = -t_new
        var mappedPath = AffineMappedTimePath3D<double>.Create(
            basePath,
            ScalarProcessor.ScalarFromNumber(-1.0),
            ScalarProcessor.Zero
        );

        // At new t=-0.5: base t=0.5, point=(2,0,0)
        var value = mappedPath.GetValue(ScalarProcessor.ScalarFromNumber(-0.5));
        Assert.That(value.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "X at t=-0.5");

        // Derivative: base deriv=4, scaled by -1 (inverse of -1 scaling)
        var deriv = mappedPath.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(-0.5));
        Assert.That(deriv.X.ScalarValue, Is.EqualTo(-4.0).Within(Tolerance), "Derivative X (reversed)");
    }

    [Test]
    public void AffineMappedTimePath3D_CreateFromRanges_RemapsTimeRange()
    {
        // Line from (0,0,0) to (10,0,0), base time [0,1]
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 10, 0, 0)
        );

        // Remap: input [0,1] -> output [5,15]
        // This means: t_out = 10*t_in + 5
        // Inverse: t_in = (t_out - 5) / 10
        var mappedPath = AffineMappedTimePath3D<double>.CreateFromRanges(
            basePath,
            ScalarProcessor.Zero,
            ScalarProcessor.One,
            ScalarProcessor.ScalarFromNumber(5.0),
            ScalarProcessor.ScalarFromNumber(15.0)
        );

        // At new t=10: base t=(10-5)/10=0.5, point=(5,0,0)
        var value = mappedPath.GetValue(ScalarProcessor.ScalarFromNumber(10.0));
        Assert.That(value.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "X at t=10");

        // At new t=5: base t=0, point=(0,0,0)
        var valueStart = mappedPath.GetValue(ScalarProcessor.ScalarFromNumber(5.0));
        Assert.That(valueStart.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=5 (start)");

        // At new t=15: base t=1, point=(10,0,0)
        var valueEnd = mappedPath.GetValue(ScalarProcessor.ScalarFromNumber(15.0));
        Assert.That(valueEnd.X.ScalarValue, Is.EqualTo(10.0).Within(Tolerance), "X at t=15 (end)");
    }

    [Test]
    public void AffineMappedTimePath3D_GetDerivative1Value_ChainRule()
    {
        // Line from (0,0,0) to (12,0,0), time [0,1], velocity=12
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 12, 0, 0)
        );

        // Scale by 4: t_new = 4*t_old
        // Inverse: t_old = t_new / 4
        // Chain rule: d/dt_new = d/dt_old * dt_old/dt_new = 12 * (1/4) = 3
        var mappedPath = AffineMappedTimePath3D<double>.CreateScaling(
            basePath,
            ScalarProcessor.ScalarFromNumber(4.0)
        );

        var deriv = mappedPath.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(2.0));
        Assert.That(deriv.X.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "Derivative X (12 * 1/4)");
        Assert.That(deriv.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Derivative Y");
        Assert.That(deriv.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Derivative Z");
    }

    [Test]
    public void AffineMappedTimePath3D_GetDerivative2Value_ChainRuleSquared()
    {
        // For constant velocity path, second derivative is zero
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1)
        );

        // Scale by 2
        var mappedPath = AffineMappedTimePath3D<double>.CreateScaling(
            basePath,
            ScalarProcessor.ScalarFromNumber(2.0)
        );

        // Second derivative of line segment is zero, scaled is still zero
        var deriv2 = mappedPath.GetDerivative2Value(ScalarProcessor.One);
        Assert.That(deriv2.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative X");
        Assert.That(deriv2.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative Y");
        Assert.That(deriv2.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative Z");
    }

    [Test]
    public void AffineMappedTimePath3D_GetFrame_RemapsTime()
    {
        // Line from (1,0,0) to (3,0,0), time [0,1]
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 1, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 3, 0, 0)
        );

        // Scale by 2: new time [0,2]
        var mappedPath = AffineMappedTimePath3D<double>.CreateScaling(
            basePath,
            ScalarProcessor.ScalarFromNumber(2.0)
        );

        // At new t=1: base t=0.5, point=(2,0,0)
        var frame = mappedPath.GetFrame(ScalarProcessor.One);

        Assert.That(frame.Point.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Frame Point X");
        Assert.That(frame.Point.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Frame Point Y");
        Assert.That(frame.Point.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Frame Point Z");

        // Tangent should be normalized unit vector (1,0,0)
        Assert.That(frame.Tangent.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Frame Tangent X");
        Assert.That(frame.Tangent.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Frame Tangent Y");
    }

    [Test]
    public void AffineMappedTimePath3D_IsValid_WhenBaseValid()
    {
        var basePath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var mappedPath = AffineMappedTimePath3D<double>.Create(
            basePath,
            ScalarProcessor.ScalarFromNumber(2.0),
            ScalarProcessor.One
        );

        Assert.That(mappedPath.IsValid(), Is.True, "Mapped path should be valid");
        Assert.That(basePath.IsValid(), Is.True, "Base path should be valid");
    }

    [Test]
    public void AffineMappedTimePath3D_ToFinitePath_WhenFinite_ReturnsSelf()
    {
        var basePath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var mappedPath = AffineMappedTimePath3D<double>.CreateScaling(
            basePath,
            ScalarProcessor.ScalarFromNumber(2.0)
        );

        var finitePath = mappedPath.ToFinitePath();

        Assert.That(finitePath, Is.SameAs(mappedPath), "Should return self when already finite");
        Assert.That(mappedPath.IsFinite, Is.True);
        Assert.That(mappedPath.IsPeriodic, Is.False);
    }

    [Test]
    public void AffineMappedTimePath3D_ToPeriodicPath_WhenFinite_ReturnsNewInstance()
    {
        var basePath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var mappedPath = AffineMappedTimePath3D<double>.CreateScaling(
            basePath,
            ScalarProcessor.ScalarFromNumber(3.0)
        );

        var periodicPath = mappedPath.ToPeriodicPath();

        Assert.That(periodicPath, Is.Not.SameAs(mappedPath), "Should return new instance");
        Assert.That(periodicPath.IsPeriodic, Is.True);
        Assert.That(periodicPath.IsFinite, Is.False);

        // Values should still match
        var t = ScalarProcessor.ScalarFromNumber(1.5);
        var value1 = mappedPath.GetValue(t);
        var value2 = periodicPath.GetValue(t);

        Assert.That(value2.X.ScalarValue, Is.EqualTo(value1.X.ScalarValue).Within(Tolerance));
        Assert.That(value2.Y.ScalarValue, Is.EqualTo(value1.Y.ScalarValue).Within(Tolerance));
        Assert.That(value2.Z.ScalarValue, Is.EqualTo(value1.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void AffineMappedTimePath3D_ToPeriodicPath_WhenPeriodic_ReturnsSelf()
    {
        var finitePath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );
        var periodicBasePath = (ParametricPath3D<double>)finitePath.ToPeriodicPath();

        var mappedPath = AffineMappedTimePath3D<double>.CreateScaling(
            periodicBasePath,
            ScalarProcessor.ScalarFromNumber(2.0)
        );

        var periodicPath = mappedPath.ToPeriodicPath();

        Assert.That(periodicPath, Is.SameAs(mappedPath), "Should return self when already periodic");
        Assert.That(mappedPath.IsPeriodic, Is.True);
    }

    [Test]
    public void AffineMappedTimePath3D_Properties_StoreCorrectValues()
    {
        var basePath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var scaling = ScalarProcessor.ScalarFromNumber(3.0);
        var offset = ScalarProcessor.ScalarFromNumber(5.0);

        var mappedPath = AffineMappedTimePath3D<double>.Create(
            basePath,
            scaling,
            offset
        );

        Assert.That(mappedPath.BasePath, Is.SameAs(basePath), "BasePath should reference original");
        Assert.That(mappedPath.TimeMapScaling.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "TimeMapScaling");
        Assert.That(mappedPath.TimeMapOffset.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "TimeMapOffset");

        // Check inverse values: inverse_scaling = 1/3, inverse_offset = -5/3
        Assert.That(mappedPath.InverseTimeMapScaling.ScalarValue, Is.EqualTo(1.0 / 3.0).Within(Tolerance), "InverseScaling");
        Assert.That(mappedPath.InverseTimeMapOffset.ScalarValue, Is.EqualTo(-5.0 / 3.0).Within(Tolerance), "InverseOffset");
    }

    [Test]
    public void AffineMappedTimePath3D_TimeRange_TransformedCorrectly()
    {
        // Base path: time [0,1]
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1)
        );

        // Transform: t_new = 2*t_old + 3, so new time [3,5]
        var mappedPath = AffineMappedTimePath3D<double>.Create(
            basePath,
            ScalarProcessor.ScalarFromNumber(2.0),
            ScalarProcessor.ScalarFromNumber(3.0)
        );

        // TimeRange should be [3,5]
        Assert.That(mappedPath.MinTime.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "MinTime");
        Assert.That(mappedPath.MaxTime.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "MaxTime");
    }
}
