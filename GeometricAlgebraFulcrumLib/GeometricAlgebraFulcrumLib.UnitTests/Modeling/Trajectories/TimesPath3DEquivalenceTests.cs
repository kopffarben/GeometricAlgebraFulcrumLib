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
public class TimesPath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [OneTimeSetUp]
    public void ClassInit()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void TimesPath3D_TwoConstantPaths_ShouldMultiplyCorrectly()
    {
        // Path1: constant at (2, 3, 4)
        var path1 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 2, 3, 4)
        );

        // Path2: constant at (5, 6, 7)
        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 5, 6, 7)
        );

        var timesPath = TimesPath3D<double>.Finite(path1, path2);

        // Component-wise product should be (2*5, 3*6, 4*7) = (10, 18, 28)
        var value = timesPath.GetValue(ScalarProcessor.Zero);

        Assert.That(value.X.ScalarValue, Is.EqualTo(10.0).Within(Tolerance), "X component");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(18.0).Within(Tolerance), "Y component");
        Assert.That(value.Z.ScalarValue, Is.EqualTo(28.0).Within(Tolerance), "Z component");
    }

    [Test]
    public void TimesPath3D_TwoLineSegments_ShouldMultiplyCorrectly()
    {
        // Path1: (1,1,1) → (2,2,2)
        var path1 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1),
            LinVector3D<double>.Create(ScalarProcessor, 2, 2, 2)
        );

        // Path2: (2,3,4) → (4,6,8)
        var path2 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 2, 3, 4),
            LinVector3D<double>.Create(ScalarProcessor, 4, 6, 8)
        );

        var timesPath = TimesPath3D<double>.Finite(path1, path2);

        // At t=0: (1*2, 1*3, 1*4) = (2, 3, 4)
        var value0 = timesPath.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance));
        Assert.That(value0.Y.ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
        Assert.That(value0.Z.ScalarValue, Is.EqualTo(4.0).Within(Tolerance));

        // At t=0.5: (1.5*3, 1.5*4.5, 1.5*6) = (4.5, 6.75, 9)
        var value05 = timesPath.GetValue(ScalarProcessor.ScalarFromNumber(0.5));
        Assert.That(value05.X.ScalarValue, Is.EqualTo(4.5).Within(Tolerance));
        Assert.That(value05.Y.ScalarValue, Is.EqualTo(6.75).Within(Tolerance));
        Assert.That(value05.Z.ScalarValue, Is.EqualTo(9.0).Within(Tolerance));

        // At t=1: (2*4, 2*6, 2*8) = (8, 12, 16)
        var value1 = timesPath.GetValue(ScalarProcessor.One);
        Assert.That(value1.X.ScalarValue, Is.EqualTo(8.0).Within(Tolerance));
        Assert.That(value1.Y.ScalarValue, Is.EqualTo(12.0).Within(Tolerance));
        Assert.That(value1.Z.ScalarValue, Is.EqualTo(16.0).Within(Tolerance));
    }

    [Test]
    public void TimesPath3D_ThreePaths_ShouldMultiplyCorrectly()
    {
        var path1 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 2, 1, 1)
        );

        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 3, 1)
        );

        var path3 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 5)
        );

        var timesPath = TimesPath3D<double>.Finite(path1, path2, path3);

        Assert.That(timesPath.Count, Is.EqualTo(3), "Should have 3 base paths");

        // Product: (2*1*1, 1*3*1, 1*1*5) = (2, 3, 5)
        var value = timesPath.GetValue(ScalarProcessor.Zero);
        Assert.That(value.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance));
        Assert.That(value.Y.ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
        Assert.That(value.Z.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
    }

    [Test]
    public void TimesPath3D_NestedTimesPath_ShouldFlattenCorrectly()
    {
        // Create (A*B) * C and verify it flattens to [A,B,C]
        var pathA = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 2, 1, 1)
        );

        var pathB = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 3, 1)
        );

        var pathC = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 5)
        );

        var pathAB = TimesPath3D<double>.Finite(pathA, pathB);
        var pathABC = TimesPath3D<double>.Finite(pathAB, pathC);

        // Should have 3 base paths (A, B, C), not nested structure
        Assert.That(pathABC.Count, Is.EqualTo(3), "Should flatten to 3 paths");

        var value = pathABC.GetValue(ScalarProcessor.Zero);
        Assert.That(value.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance));
        Assert.That(value.Y.ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
        Assert.That(value.Z.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
    }

    [Test]
    public void TimesPath3D_GetDerivative1Value_ShouldUseProductRule()
    {
        // Path1: f(t) = (t, 1, 1) has derivative f'(t) = (1, 0, 0)
        var path1 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 1, 1),
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1)
        );

        // Path2: g(t) = (1, t, 1) has derivative g'(t) = (0, 1, 0)
        var path2 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 1, 0, 1),
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1)
        );

        var timesPath = TimesPath3D<double>.Finite(path1, path2);

        // Product rule: d/dt[f⊗g] = f'⊗g + f⊗g'
        // At t=0.5: f=(0.5,1,1), g=(1,0.5,1), f'=(1,0,0), g'=(0,1,0)
        // Result: (1,0,0)⊗(1,0.5,1) + (0.5,1,1)⊗(0,1,0) = (1,0,0) + (0,1,0) = (1,1,0)
        var deriv = timesPath.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(deriv.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X derivative");
        Assert.That(deriv.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Y derivative");
        Assert.That(deriv.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Z derivative");
    }

    [Test]
    public void TimesPath3D_GetDerivative2Value_ConstantVelocity_ShouldBeZero()
    {
        // For constant velocity (linear) paths, second derivative of product has specific form
        var path1 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1),
            LinVector3D<double>.Create(ScalarProcessor, 2, 2, 2)
        );

        var path2 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1),
            LinVector3D<double>.Create(ScalarProcessor, 3, 3, 3)
        );

        var timesPath = TimesPath3D<double>.Finite(path1, path2);

        // For linear paths: f''=0, g''=0, so d²/dt²[f⊗g] = 2(f'⊗g')
        // f'=(1,1,1), g'=(2,2,2), so 2(f'⊗g') = 2(2,2,2) = (4,4,4)
        var deriv2 = timesPath.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(deriv2.X.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "Second derivative X");
        Assert.That(deriv2.Y.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "Second derivative Y");
        Assert.That(deriv2.Z.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "Second derivative Z");
    }

    [Test]
    public void TimesPath3D_IsValid_ShouldReturnTrue()
    {
        var path1 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 4, 5, 6)
        );

        var timesPath = TimesPath3D<double>.Finite(path1, path2);

        Assert.That(timesPath.IsValid(), Is.True);
    }

    [Test]
    public void TimesPath3D_ToFinitePath_WhenFinite_ShouldReturnSelf()
    {
        var path1 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 4, 5, 6)
        );

        var timesPath = TimesPath3D<double>.Finite(path1, path2);

        var finitePath = timesPath.ToFinitePath();

        Assert.That(finitePath, Is.SameAs(timesPath));
        Assert.That(timesPath.IsFinite, Is.True);
        Assert.That(timesPath.IsPeriodic, Is.False);
    }

    [Test]
    public void TimesPath3D_ToPeriodicPath_WhenFinite_ShouldReturnNewInstance()
    {
        var path1 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 2, 3, 4)
        );

        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 5, 6, 7)
        );

        var timesPath = TimesPath3D<double>.Finite(path1, path2);

        var periodicPath = timesPath.ToPeriodicPath();

        Assert.That(periodicPath, Is.Not.SameAs(timesPath));
        Assert.That(periodicPath.IsPeriodic, Is.True);
        Assert.That(periodicPath.IsFinite, Is.False);

        // Values should still match
        var t = ScalarProcessor.ScalarFromNumber(0.5);
        var value1 = timesPath.GetValue(t);
        var value2 = periodicPath.GetValue(t);

        Assert.That(value2.X.ScalarValue, Is.EqualTo(value1.X.ScalarValue).Within(Tolerance));
        Assert.That(value2.Y.ScalarValue, Is.EqualTo(value1.Y.ScalarValue).Within(Tolerance));
        Assert.That(value2.Z.ScalarValue, Is.EqualTo(value1.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void TimesPath3D_IReadOnlyList_ShouldProvideAccess()
    {
        var path1 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 2, 1, 1)
        );

        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 3, 1)
        );

        var timesPath = TimesPath3D<double>.Finite(path1, path2);

        // Test IReadOnlyList interface
        Assert.That(timesPath.Count, Is.EqualTo(2));
        Assert.That(timesPath[0], Is.SameAs(path1));
        Assert.That(timesPath[1], Is.SameAs(path2));

        // Test enumeration
        var list = timesPath.ToList();
        Assert.That(list.Count, Is.EqualTo(2));
        Assert.That(list[0], Is.SameAs(path1));
        Assert.That(list[1], Is.SameAs(path2));
    }

    [Test]
    public void TimesPath3D_TimeRange_ShouldBeMinMaxOfComponents()
    {
        // Path1: time range [0, 1]
        var path1 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1),
            LinVector3D<double>.Create(ScalarProcessor, 2, 2, 2)
        );

        // Path2: time range [0, 1] (same)
        var path2 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1),
            LinVector3D<double>.Create(ScalarProcessor, 3, 3, 3)
        );

        var timesPath = TimesPath3D<double>.Finite(path1, path2);

        // Time range should be [0, 1] (min of mins, max of maxes)
        Assert.That(timesPath.MinTime.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(timesPath.MaxTime.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
    }

    [Test]
    public void TimesPath3D_Periodic_ShouldCreatePeriodicPath()
    {
        var path1 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 4, 5, 6)
        );

        var timesPath = TimesPath3D<double>.Periodic(path1, path2);

        Assert.That(timesPath.IsPeriodic, Is.True);
        Assert.That(timesPath.IsFinite, Is.False);
    }

    [Test]
    public void TimesPath3D_WithSimpleHarmonic_ShouldMultiplyCorrectly()
    {
        // Path1: simple harmonic varying only in X direction (2, 0, 0)
        var path1 = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessor,
            1,  // harmonicFactor
            LinVector3D<double>.Create(ScalarProcessor, 2, 0, 0)
        );

        // Path2: constant scaling at (3, 5, 7)
        var path2 = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 3, 5, 7)
        );

        var timesPath = TimesPath3D<double>.Finite(path1, path2);

        // At t=0: cos(0)*(2,0,0) ⊗ (3,5,7) = (2,0,0) ⊗ (3,5,7) = (6, 0, 0)
        var value0 = timesPath.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(6.0).Within(Tolerance));
        Assert.That(value0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(value0.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
    }

    [Test]
    public void TimesPath3D_IdentityMultiplication_ShouldPreserveOriginal()
    {
        // Multiplying by (1,1,1) should preserve the original path
        var path1 = LineSegmentPath3D<double>.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 2, 3, 4),
            LinVector3D<double>.Create(ScalarProcessor, 5, 6, 7)
        );

        var identityPath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1)
        );

        var timesPath = TimesPath3D<double>.Finite(path1, identityPath);

        // At t=0: (2,3,4) ⊗ (1,1,1) = (2,3,4)
        var value0 = timesPath.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance));
        Assert.That(value0.Y.ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
        Assert.That(value0.Z.ScalarValue, Is.EqualTo(4.0).Within(Tolerance));

        // At t=1: (5,6,7) ⊗ (1,1,1) = (5,6,7)
        var value1 = timesPath.GetValue(ScalarProcessor.One);
        Assert.That(value1.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(value1.Y.ScalarValue, Is.EqualTo(6.0).Within(Tolerance));
        Assert.That(value1.Z.ScalarValue, Is.EqualTo(7.0).Within(Tolerance));
    }
}
