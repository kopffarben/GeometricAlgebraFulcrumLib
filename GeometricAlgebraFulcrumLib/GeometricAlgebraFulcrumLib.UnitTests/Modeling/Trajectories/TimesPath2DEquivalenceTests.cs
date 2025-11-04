using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Mapped;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Tests für Generic TimesPath2D&lt;T&gt;
/// Phase 3 Module 6B - Component-wise Path Multiplication
/// Tests: Component-wise multiplication, flattening
/// </summary>
[TestFixture]
public class TimesPath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static ScalarRange<double> TimeRange => ScalarRange<double>.Create(
        ScalarProcessor.Scalar(0),
        ScalarProcessor.Scalar(1)
    );

    #endregion

    #region Basic Component-wise Multiplication Tests (3 tests)

    [Test]
    public void TimesPath2D_TwoPaths_ShouldMultiplyComponentWise()
    {
        // Arrange - Zwei konstante Pfade: (2,3) ⊙ (4,5) = (8,15)
        var path1 = ConstantPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 2, 3)
        );

        var path2 = ConstantPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 4, 5)
        );

        // Act
        var timesPath = TimesPath2D<double>.Finite(path1, path2);

        // Assert - Component-wise multiplication: (2,3) * (4,5) = (2*4, 3*5) = (8,15)
        var result = timesPath.GetValue(ScalarProcessor.Scalar(0.5));
        Assert.That(result.X.ScalarValue, Is.EqualTo(8.0).Within(Tolerance), "X should be 2*4=8");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(15.0).Within(Tolerance), "Y should be 3*5=15");
    }

    [Test]
    public void TimesPath2D_ThreePaths_ShouldMultiplyCorrectly()
    {
        // Arrange - Drei Pfade: (2,1) ⊙ (3,4) ⊙ (1,2) = (6,8)
        var path1 = ConstantPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 2, 1)
        );

        var path2 = ConstantPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 3, 4)
        );

        var path3 = ConstantPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 1, 2)
        );

        // Act
        var timesPath = TimesPath2D<double>.Finite(path1, path2, path3);

        // Assert - (2,1) * (3,4) * (1,2) = (2*3*1, 1*4*2) = (6, 8)
        var result = timesPath.GetValue(ScalarProcessor.Scalar(0.5));
        Assert.That(result.X.ScalarValue, Is.EqualTo(6.0).Within(Tolerance), "X should be 2*3*1=6");
        Assert.That(result.Y.ScalarValue, Is.EqualTo(8.0).Within(Tolerance), "Y should be 1*4*2=8");
    }

    [Test]
    public void TimesPath2D_CountProperty_ShouldBeCorrect()
    {
        // Arrange
        var path1 = ConstantPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 2, 3)
        );

        var path2 = ConstantPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 4, 5)
        );

        var path3 = ConstantPath2D<double>.Create(
            TimeRange,
            false,
            LinVector2D<double>.Create(ScalarProcessor, 6, 7)
        );

        // Act
        var timesPath = TimesPath2D<double>.Finite(path1, path2, path3);

        // Assert
        Assert.That(timesPath.Count, Is.EqualTo(3), "Should have 3 base paths");
        Assert.That(timesPath[0], Is.SameAs(path1), "First path");
        Assert.That(timesPath[1], Is.SameAs(path2), "Second path");
        Assert.That(timesPath[2], Is.SameAs(path3), "Third path");
    }

    #endregion
}
