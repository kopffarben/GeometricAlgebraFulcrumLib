using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.AffineMaps.Space1D;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Mapped;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Tests für Generic AffineMappedTimePath2D&lt;T&gt;
/// Phase 3 Module 6B - Affine Time Transformation
/// Tests: Time scaling, offset, inverse mapping, derivatives
/// </summary>
[TestFixture]
public class AffineMappedTimePath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static ScalarRange<double> TimeRange => ScalarRange<double>.Create(
        ScalarProcessor.Scalar(0),
        ScalarProcessor.Scalar(1)
    );

    #endregion

    #region Time Transformation Tests (4 tests)

    [Test]
    public void AffineMappedTimePath2D_TimeScaling_ShouldDoubleSpeed()
    {
        // Arrange - Linear path from (0,0) to (10,10) over [0,1]
        var basePath = LineSegmentPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 0, 0),
            LinVector2D<double>.Create(ScalarProcessor, 10, 10)
        );

        // Time map: t' = 2*t → mapped range [0,2], speed is doubled
        var timeMap = AffineMap1D<double>.CreateScale(
            ScalarProcessor,
            ScalarProcessor.Scalar(2.0)
        );

        // Act
        var mappedPath = AffineMappedTimePath2D<double>.Create(basePath, timeMap);

        // Assert - Mapped time range should be [0, 2]
        Assert.That(mappedPath.TimeRange.MinValue.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Min time");
        Assert.That(mappedPath.TimeRange.MaxValue.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Max time");

        // At mapped t=1.0 → base t=0.5 → (5,5)
        var midResult = mappedPath.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(midResult.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "X at mid");
        Assert.That(midResult.Y.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Y at mid");

        // At mapped t=2.0 → base t=1.0 → (10,10)
        var endResult = mappedPath.GetValue(ScalarProcessor.Scalar(2.0));
        Assert.That(endResult.X.ScalarValue, Is.EqualTo(10.0).Within(Tolerance), "X at end");
        Assert.That(endResult.Y.ScalarValue, Is.EqualTo(10.0).Within(Tolerance), "Y at end");
    }

    [Test]
    public void AffineMappedTimePath2D_TimeOffset_ShouldShiftTime()
    {
        // Arrange - Constant path at (5, 7)
        var basePath = ConstantPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 5, 7)
        );

        // Time map: t' = t + 1 (shift by 1)
        var timeMap = AffineMap1D<double>.CreateTranslate(
            ScalarProcessor,
            ScalarProcessor.Scalar(1.0)
        );

        // Act
        var mappedPath = AffineMappedTimePath2D<double>.Create(basePath, timeMap);

        // Assert - Time range should be [1, 2]
        Assert.That(mappedPath.TimeRange.MinValue.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Min time");
        Assert.That(mappedPath.TimeRange.MaxValue.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Max time");

        // Value should still be constant everywhere
        var result = mappedPath.GetValue(ScalarProcessor.Scalar(1.5));
        Assert.That(result.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "X constant");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(7.0).Within(Tolerance), "Y constant");
    }

    [Test]
    public void AffineMappedTimePath2D_CombinedTransform_ShouldScaleAndShift()
    {
        // Arrange - Linear path
        var basePath = LineSegmentPath2D<double>.Create(
            ScalarRange<double>.Create(ScalarProcessor.Scalar(0), ScalarProcessor.Scalar(2)),
            false,
            LinVector2D<double>.Create(ScalarProcessor, 0, 0),
            LinVector2D<double>.Create(ScalarProcessor, 20, 30)
        );

        // Time map: t' = 2*t + 1 (scale by 2, shift by 1)
        var timeMap = AffineMap1D<double>.Create(
            ScalarProcessor,
            ScalarProcessor.Scalar(2.0),   // scaling
            ScalarProcessor.Scalar(1.0)    // offset
        );

        // Act
        var mappedPath = AffineMappedTimePath2D<double>.Create(basePath, timeMap);

        // Assert - New time range: [2*0+1, 2*2+1] = [1, 5]
        Assert.That(mappedPath.TimeRange.MinValue.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Min time");
        Assert.That(mappedPath.TimeRange.MaxValue.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Max time");

        // At t=3 in mapped path: inverse gives (3-1)/2 = 1.0 in base path
        // Base path at t=1.0: (10, 15)
        var result = mappedPath.GetValue(ScalarProcessor.Scalar(3.0));
        Assert.That(result.X.ScalarValue, Is.EqualTo(10.0).Within(Tolerance), "X at t=3");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(15.0).Within(Tolerance), "Y at t=3");
    }

    [Test]
    public void AffineMappedTimePath2D_NegativeScaling_ShouldReverseTime()
    {
        // Arrange - Linear path from (0,0) to (10,10)
        var basePath = LineSegmentPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 0, 0),
            LinVector2D<double>.Create(ScalarProcessor, 10, 10)
        );

        // Time map: t' = -t (reverse time)
        var timeMap = AffineMap1D<double>.CreateScale(
            ScalarProcessor,
            ScalarProcessor.Scalar(-1.0)
        );

        // Act
        var mappedPath = AffineMappedTimePath2D<double>.Create(basePath, timeMap);

        // Assert - Time range should be reversed: [-1, 0]
        Assert.That(mappedPath.TimeRange.MinValue.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), "Min time");
        Assert.That(mappedPath.TimeRange.MaxValue.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Max time");

        // At t=-1 in mapped path, we should be at t=1 in base path (end)
        var result = mappedPath.GetValue(ScalarProcessor.Scalar(-1.0));
        Assert.That(result.X.ScalarValue, Is.EqualTo(10.0).Within(Tolerance), "X at reversed end");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(10.0).Within(Tolerance), "Y at reversed end");
    }

    #endregion
}
