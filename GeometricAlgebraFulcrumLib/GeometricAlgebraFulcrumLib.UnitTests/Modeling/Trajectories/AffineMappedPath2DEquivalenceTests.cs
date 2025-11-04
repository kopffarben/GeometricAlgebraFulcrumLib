using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Mapped;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Tests für Generic AffineMappedPath2D&lt;T&gt;
/// Phase 3 Module 6B - Spatial Affine Transformation
/// Tests: Translation, rotation, scaling, combined transformations
/// </summary>
[TestFixture]
public class AffineMappedPath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static ScalarRange<double> TimeRange => ScalarRange<double>.Create(
        ScalarProcessor.Scalar(0),
        ScalarProcessor.Scalar(1)
    );

    #endregion

    #region Affine Transformation Tests (5 tests)

    [Test]
    public void AffineMappedPath2D_Translation_ShouldShiftPointsNotVectors()
    {
        // Arrange - Linear path from (0,0) to (10,10)
        var basePath = LineSegmentPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 0, 0),
            LinVector2D<double>.Create(ScalarProcessor, 10, 10)
        );

        // Translation: shift by (5, 3)
        var offset = LinVector2D<double>.Create(ScalarProcessor, 5, 3);

        Func<LinVector2D<double>, LinVector2D<double>> pointMap = p => p + offset;
        Func<LinVector2D<double>, LinVector2D<double>> vectorMap = v => v; // Vectors unchanged

        // Act
        var mappedPath = AffineMappedPath2D<double>.Create(basePath, pointMap, vectorMap);

        // Assert - Points are translated
        var start = mappedPath.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(start.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Start X translated");
        Assert.That(start.Y.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "Start Y translated");

        var end = mappedPath.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(end.X.ScalarValue, Is.EqualTo(15.0).Within(Tolerance), "End X translated");
        Assert.That(end.Y.ScalarValue, Is.EqualTo(13.0).Within(Tolerance), "End Y translated");

        // Derivative should be unchanged (10, 10) because vectorMap is identity
        var derivative = mappedPath.GetDerivative1Value(ScalarProcessor.Scalar(0.5));
        Assert.That(derivative.X.ScalarValue, Is.EqualTo(10.0).Within(Tolerance), "Derivative X unchanged");
        Assert.That(derivative.Y.ScalarValue, Is.EqualTo(10.0).Within(Tolerance), "Derivative Y unchanged");
    }

    [Test]
    public void AffineMappedPath2D_Scaling_ShouldScaleBothPointsAndVectors()
    {
        // Arrange - Linear path from (1,1) to (3,3)
        var basePath = LineSegmentPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 1, 1),
            LinVector2D<double>.Create(ScalarProcessor, 3, 3)
        );

        // Scaling: 2x in both dimensions (linear transformation)
        Func<LinVector2D<double>, LinVector2D<double>> scaleMap = v =>
            LinVector2D<double>.Create(ScalarProcessor, v.X.ScalarValue * 2, v.Y.ScalarValue * 2);

        // Act
        var mappedPath = AffineMappedPath2D<double>.CreateLinear(basePath, scaleMap);

        // Assert - Points are scaled
        var start = mappedPath.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(start.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Start X scaled");
        Assert.That(start.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Start Y scaled");

        var end = mappedPath.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(end.X.ScalarValue, Is.EqualTo(6.0).Within(Tolerance), "End X scaled");
        Assert.That(end.Y.ScalarValue, Is.EqualTo(6.0).Within(Tolerance), "End Y scaled");

        // Derivative (2,2) should also be scaled to (4,4)
        var derivative = mappedPath.GetDerivative1Value(ScalarProcessor.Scalar(0.5));
        Assert.That(derivative.X.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "Derivative X scaled");
        Assert.That(derivative.Y.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "Derivative Y scaled");
    }

    [Test]
    public void AffineMappedPath2D_Rotation90Degrees_ShouldRotateCorrectly()
    {
        // Arrange - Horizontal path from (1,0) to (2,0)
        var basePath = LineSegmentPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 1, 0),
            LinVector2D<double>.Create(ScalarProcessor, 2, 0)
        );

        // 90-degree counter-clockwise rotation: (x,y) -> (-y,x)
        Func<LinVector2D<double>, LinVector2D<double>> rotationMap = v =>
            LinVector2D<double>.Create(ScalarProcessor, -v.Y.ScalarValue, v.X.ScalarValue);

        // Act
        var mappedPath = AffineMappedPath2D<double>.CreateLinear(basePath, rotationMap);

        // Assert - Path should now be vertical: (0,1) to (0,2)
        var start = mappedPath.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(start.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Start X after rotation");
        Assert.That(start.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Start Y after rotation");

        var end = mappedPath.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(end.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "End X after rotation");
        Assert.That(end.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "End Y after rotation");

        // Original derivative (1,0) should rotate to (0,1)
        var derivative = mappedPath.GetDerivative1Value(ScalarProcessor.Scalar(0.5));
        Assert.That(derivative.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Derivative X after rotation");
        Assert.That(derivative.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Derivative Y after rotation");
    }

    [Test]
    public void AffineMappedPath2D_CombinedTransform_ShouldApplyScalingAndTranslation()
    {
        // Arrange - Path from (1,1) to (2,2)
        var basePath = LineSegmentPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 1, 1),
            LinVector2D<double>.Create(ScalarProcessor, 2, 2)
        );

        // Combined: Scale by 3, then translate by (10, 20)
        var offset = LinVector2D<double>.Create(ScalarProcessor, 10, 20);
        Func<LinVector2D<double>, LinVector2D<double>> pointMap = p =>
            LinVector2D<double>.Create(
                ScalarProcessor,
                p.X.ScalarValue * 3 + 10,
                p.Y.ScalarValue * 3 + 20
            );
        Func<LinVector2D<double>, LinVector2D<double>> vectorMap = v =>
            LinVector2D<double>.Create(ScalarProcessor, v.X.ScalarValue * 3, v.Y.ScalarValue * 3);

        // Act
        var mappedPath = AffineMappedPath2D<double>.Create(basePath, pointMap, vectorMap);

        // Assert - Start point: (1,1) * 3 + (10,20) = (13, 23)
        var start = mappedPath.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(start.X.ScalarValue, Is.EqualTo(13.0).Within(Tolerance), "Start X");
        Assert.That(start.Y.ScalarValue, Is.EqualTo(23.0).Within(Tolerance), "Start Y");

        // End point: (2,2) * 3 + (10,20) = (16, 26)
        var end = mappedPath.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(end.X.ScalarValue, Is.EqualTo(16.0).Within(Tolerance), "End X");
        Assert.That(end.Y.ScalarValue, Is.EqualTo(26.0).Within(Tolerance), "End Y");

        // Derivative: (1,1) * 3 = (3,3) - translation doesn't affect vectors
        var derivative = mappedPath.GetDerivative1Value(ScalarProcessor.Scalar(0.5));
        Assert.That(derivative.X.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "Derivative X");
        Assert.That(derivative.Y.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "Derivative Y");
    }

    [Test]
    public void AffineMappedPath2D_TimeRangeAndPeriodicity_ShouldPreserveFromBasePath()
    {
        // Arrange - Periodic path with custom time range
        var customRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(2),
            ScalarProcessor.Scalar(5)
        );
        var basePath = ConstantPath2D<double>.Create(
            customRange,
            true, // periodic
            LinVector2D<double>.Create(ScalarProcessor, 1, 2)
        );

        Func<LinVector2D<double>, LinVector2D<double>> identityMap = v => v;

        // Act
        var mappedPath = AffineMappedPath2D<double>.CreateLinear(basePath, identityMap);

        // Assert - Time range and periodicity preserved
        Assert.That(mappedPath.TimeRange.MinValue.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Min time");
        Assert.That(mappedPath.TimeRange.MaxValue.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Max time");
        Assert.That(mappedPath.IsPeriodic, Is.True, "Should be periodic");
    }

    #endregion
}
