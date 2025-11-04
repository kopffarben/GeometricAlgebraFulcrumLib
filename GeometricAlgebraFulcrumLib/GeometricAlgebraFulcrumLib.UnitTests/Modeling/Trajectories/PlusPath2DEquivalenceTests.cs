using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Mapped;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Tests für Generic PlusPath2D&lt;T&gt;
/// Phase 3 Module 6B - Path Superposition (Addition)
/// Tests: Superposition, Zeitbereich, Ableitungen, Flattening
/// </summary>
[TestFixture]
public class PlusPath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static ScalarRange<double> TimeRange => ScalarRange<double>.Create(
        ScalarProcessor.Scalar(0),
        ScalarProcessor.Scalar(1)
    );

    #endregion

    #region Basic Superposition Tests (3 tests)

    [Test]
    public void PlusPath2D_TwoPaths_ShouldSumCorrectly()
    {
        // Arrange - Zwei konstante Pfade: (1,2) + (3,4) = (4,6)
        var path1 = ConstantPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 1, 2)
        );

        var path2 = ConstantPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 3, 4)
        );

        // Act
        var plusPath = PlusPath2D<double>.Finite(path1, path2);

        // Assert
        var result = plusPath.GetValue(ScalarProcessor.Scalar(0.5));
        Assert.That(result.X.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "X should be 1+3=4");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(6.0).Within(Tolerance), "Y should be 2+4=6");
    }

    [Test]
    public void PlusPath2D_ThreePaths_ShouldSumCorrectly()
    {
        // Arrange - Drei lineare Pfade
        var path1 = LineSegmentPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 0, 0),
            LinVector2D<double>.Create(ScalarProcessor, 1, 0)
        );

        var path2 = LineSegmentPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 0, 0),
            LinVector2D<double>.Create(ScalarProcessor, 0, 1)
        );

        var path3 = ConstantPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 2, 3)
        );

        // Act
        var plusPath = PlusPath2D<double>.Finite(path1, path2, path3);

        // Assert - Bei t=0: (0,0) + (0,0) + (2,3) = (2,3)
        var result0 = plusPath.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(result0.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "X at t=0");
        Assert.That(result0.Y.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "Y at t=0");

        // Bei t=1: (1,0) + (0,1) + (2,3) = (3,4)
        var result1 = plusPath.GetValue(ScalarProcessor.Scalar(1.0));
        Assert.That(result1.X.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "X at t=1");
        Assert.That(result1.Y.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "Y at t=1");
    }

    [Test]
    public void PlusPath2D_CountProperty_ShouldBeCorrect()
    {
        // Arrange
        var path1 = ConstantPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 1, 1)
        );

        var path2 = ConstantPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 2, 2)
        );

        var path3 = ConstantPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 3, 3)
        );

        // Act
        var plusPath = PlusPath2D<double>.Finite(path1, path2, path3);

        // Assert
        Assert.That(plusPath.Count, Is.EqualTo(3), "Should have 3 base paths");
        Assert.That(plusPath[0], Is.SameAs(path1), "First path");
        Assert.That(plusPath[1], Is.SameAs(path2), "Second path");
        Assert.That(plusPath[2], Is.SameAs(path3), "Third path");
    }

    #endregion
}
