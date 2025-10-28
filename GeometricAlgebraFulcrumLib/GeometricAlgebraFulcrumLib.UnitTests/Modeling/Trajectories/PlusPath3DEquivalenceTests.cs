using System.Linq;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;
using NUnit.Framework;
using ConstantPath3D = GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic.ConstantPath3D<double>;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public class PlusPath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [OneTimeSetUp]
    public void ClassInit()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void PlusPath3D_TwoConstantPaths_ShouldSumCorrectly()
    {
        // Path1: constant at (1, 2, 3)
        var path1 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        // Path2: constant at (4, 5, 6)
        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 4, 5, 6)
        );

        var plusPath = PlusPath3D<double>.Finite(path1, path2);

        // Sum should be (5, 7, 9)
        var value = plusPath.GetValue(ScalarProcessor.Zero);

        Assert.That(value.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "X component");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(7.0).Within(Tolerance), "Y component");
        Assert.That(value.Z.ScalarValue, Is.EqualTo(9.0).Within(Tolerance), "Z component");
    }

    [Test]
    public void PlusPath3D_TwoLineSegments_ShouldSumCorrectly()
    {
        // Path1: (0,0,0) → (10,0,0)
        var path1 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 10, 0, 0)
        );

        // Path2: (0,0,0) → (0,10,0)
        var path2 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 0, 10, 0)
        );

        var plusPath = PlusPath3D<double>.Finite(path1, path2);

        // At t=0: (0+0, 0+0, 0+0) = (0, 0, 0)
        var value0 = plusPath.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(value0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(value0.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));

        // At t=0.5: (5+0, 0+5, 0+0) = (5, 5, 0)
        var value05 = plusPath.GetValue(ScalarProcessor.ScalarFromNumber(0.5));
        Assert.That(value05.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(value05.Y.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(value05.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));

        // At t=1: (10+0, 0+10, 0+0) = (10, 10, 0)
        var value1 = plusPath.GetValue(ScalarProcessor.One);
        Assert.That(value1.X.ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
        Assert.That(value1.Y.ScalarValue, Is.EqualTo(10.0).Within(Tolerance));
        Assert.That(value1.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
    }

    [Test]
    public void PlusPath3D_ThreePaths_ShouldSumCorrectly()
    {
        var path1 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 0, 0)
        );

        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 0, 2, 0)
        );

        var path3 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 3)
        );

        var plusPath = PlusPath3D<double>.Finite(path1, path2, path3);

        Assert.That(plusPath.Count, Is.EqualTo(3), "Should have 3 base paths");

        var value = plusPath.GetValue(ScalarProcessor.Zero);
        Assert.That(value.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(value.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance));
        Assert.That(value.Z.ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
    }

    [Test]
    public void PlusPath3D_NestedPlusPath_ShouldFlattenCorrectly()
    {
        // Create (A+B) + C and verify it flattens to [A,B,C]
        var pathA = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 0, 0)
        );

        var pathB = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 0, 2, 0)
        );

        var pathC = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 3)
        );

        var pathAB = PlusPath3D<double>.Finite(pathA, pathB);
        var pathABC = PlusPath3D<double>.Finite(pathAB, pathC);

        // Should have 3 base paths (A, B, C), not nested structure
        Assert.That(pathABC.Count, Is.EqualTo(3), "Should flatten to 3 paths");

        var value = pathABC.GetValue(ScalarProcessor.Zero);
        Assert.That(value.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(value.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance));
        Assert.That(value.Z.ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
    }

    [Test]
    public void PlusPath3D_GetDerivative1Value_ShouldSumDerivatives()
    {
        // Path1: line from (0,0,0) to (10,0,0) has derivative (10,0,0)
        var path1 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 10, 0, 0)
        );

        // Path2: line from (0,0,0) to (0,20,0) has derivative (0,20,0)
        var path2 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 0, 20, 0)
        );

        var plusPath = PlusPath3D<double>.Finite(path1, path2);

        // Derivative should be (10, 20, 0)
        var deriv = plusPath.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(deriv.X.ScalarValue, Is.EqualTo(10.0).Within(Tolerance), "X derivative");
        Assert.That(deriv.Y.ScalarValue, Is.EqualTo(20.0).Within(Tolerance), "Y derivative");
        Assert.That(deriv.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Z derivative");
    }

    [Test]
    public void PlusPath3D_GetDerivative2Value_ShouldSumSecondDerivatives()
    {
        // For constant velocity paths, second derivative should be zero
        var path1 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 10, 0, 0)
        );

        var path2 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 0, 10, 0)
        );

        var plusPath = PlusPath3D<double>.Finite(path1, path2);

        var deriv2 = plusPath.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(deriv2.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative X");
        Assert.That(deriv2.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative Y");
        Assert.That(deriv2.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Second derivative Z");
    }

    [Test]
    public void PlusPath3D_IsValid_ShouldReturnTrue()
    {
        var path1 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 4, 5, 6)
        );

        var plusPath = PlusPath3D<double>.Finite(path1, path2);

        Assert.That(plusPath.IsValid(), Is.True);
    }

    [Test]
    public void PlusPath3D_ToFinitePath_WhenFinite_ShouldReturnSelf()
    {
        var path1 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 4, 5, 6)
        );

        var plusPath = PlusPath3D<double>.Finite(path1, path2);

        var finitePath = plusPath.ToFinitePath();

        Assert.That(finitePath, Is.SameAs(plusPath));
        Assert.That(plusPath.IsFinite, Is.True);
        Assert.That(plusPath.IsPeriodic, Is.False);
    }

    [Test]
    public void PlusPath3D_ToPeriodicPath_WhenFinite_ShouldReturnNewInstance()
    {
        var path1 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 4, 5, 6)
        );

        var plusPath = PlusPath3D<double>.Finite(path1, path2);

        var periodicPath = plusPath.ToPeriodicPath();

        Assert.That(periodicPath, Is.Not.SameAs(plusPath));
        Assert.That(periodicPath.IsPeriodic, Is.True);
        Assert.That(periodicPath.IsFinite, Is.False);

        // Values should still match
        var t = ScalarProcessor.ScalarFromNumber(0.5);
        var value1 = plusPath.GetValue(t);
        var value2 = periodicPath.GetValue(t);

        Assert.That(value2.X.ScalarValue, Is.EqualTo(value1.X.ScalarValue).Within(Tolerance));
        Assert.That(value2.Y.ScalarValue, Is.EqualTo(value1.Y.ScalarValue).Within(Tolerance));
        Assert.That(value2.Z.ScalarValue, Is.EqualTo(value1.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void PlusPath3D_IReadOnlyList_ShouldProvideAccess()
    {
        var path1 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 0, 0)
        );

        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 0, 2, 0)
        );

        var plusPath = PlusPath3D<double>.Finite(path1, path2);

        // Test IReadOnlyList interface
        Assert.That(plusPath.Count, Is.EqualTo(2));
        Assert.That(plusPath[0], Is.SameAs(path1));
        Assert.That(plusPath[1], Is.SameAs(path2));

        // Test enumeration
        var list = plusPath.ToList();
        Assert.That(list.Count, Is.EqualTo(2));
        Assert.That(list[0], Is.SameAs(path1));
        Assert.That(list[1], Is.SameAs(path2));
    }

    [Test]
    public void PlusPath3D_TimeRange_ShouldBeMinMaxOfComponents()
    {
        // Path1: time range [0, 1]
        var path1 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 10, 0, 0)
        );

        // Path2: time range [0, 1] (same)
        var path2 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 0, 10, 0)
        );

        var plusPath = PlusPath3D<double>.Finite(path1, path2);

        // Time range should be [0, 1] (min of mins, max of maxes)
        Assert.That(plusPath.MinTime.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(plusPath.MaxTime.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
    }

    [Test]
    public void PlusPath3D_Periodic_ShouldCreatePeriodicPath()
    {
        var path1 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 4, 5, 6)
        );

        var plusPath = PlusPath3D<double>.Periodic(path1, path2);

        Assert.That(plusPath.IsPeriodic, Is.True);
        Assert.That(plusPath.IsFinite, Is.False);
    }

    [Test]
    public void PlusPath3D_WithSimpleHarmonic_ShouldSumCorrectly()
    {
        // Path1: simple harmonic at (1, 0, 0)
        var path1 = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessor,
            1,  // harmonicFactor
            LinVector3D<double>.Create(ScalarProcessor, 1, 0, 0)
        );

        // Path2: constant offset at (5, 5, 5)
        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 5, 5, 5)
        );

        var plusPath = PlusPath3D<double>.Finite(path1, path2);

        // At t=0: cos(0) * (1,0,0) + (5,5,5) = (1,0,0) + (5,5,5) = (6, 5, 5)
        var value0 = plusPath.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(6.0).Within(Tolerance));
        Assert.That(value0.Y.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(value0.Z.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
    }
}
