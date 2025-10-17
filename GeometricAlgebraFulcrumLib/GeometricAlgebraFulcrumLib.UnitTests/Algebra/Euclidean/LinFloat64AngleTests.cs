using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Tests for Angle Operations
/// Phase 3B - Core Modeling: Euclidean Geometry (6 tests)
/// Tests PolarAngle and DirectedAngle types
/// </summary>
[TestFixture]
public class LinFloat64AngleTests
{
    private const double Tolerance = 1e-10;

    #region PolarAngle Tests (3 tests)

    [Test]
    public void PolarAngle_CreateFromDegrees_ShouldHaveCorrectRadians()
    {
        // Arrange & Act
        var angle90 = LinFloat64PolarAngle.CreateFromDegrees(90);
        var angle180 = LinFloat64PolarAngle.CreateFromDegrees(180);
        var angle270 = LinFloat64PolarAngle.CreateFromDegrees(270);

        // Assert
        Assert.That(angle90.Radians.ScalarValue, Is.EqualTo(Math.PI / 2).Within(Tolerance),
            "90° should be π/2 radians");
        Assert.That(angle180.Radians.ScalarValue, Is.EqualTo(Math.PI).Within(Tolerance),
            "180° should be π radians");
        Assert.That(angle270.Radians.ScalarValue, Is.EqualTo(3 * Math.PI / 2).Within(Tolerance),
            "270° should be 3π/2 radians");
    }

    [Test]
    public void PolarAngle_CosSinValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var angle0 = LinFloat64PolarAngle.Angle0;
        var angle90 = LinFloat64PolarAngle.Angle90;
        var angle180 = LinFloat64PolarAngle.Angle180;
        var angle270 = LinFloat64PolarAngle.Angle270;

        // Assert
        // 0°: cos = 1, sin = 0
        Assert.That(angle0.Cos().ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "cos(0°) = 1");
        Assert.That(angle0.Sin().ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "sin(0°) = 0");

        // 90°: cos = 0, sin = 1
        Assert.That(angle90.Cos().ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "cos(90°) = 0");
        Assert.That(angle90.Sin().ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "sin(90°) = 1");

        // 180°: cos = -1, sin = 0
        Assert.That(angle180.Cos().ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), "cos(180°) = -1");
        Assert.That(angle180.Sin().ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "sin(180°) = 0");

        // 270°: cos = 0, sin = -1
        Assert.That(angle270.Cos().ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "cos(270°) = 0");
        Assert.That(angle270.Sin().ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), "sin(270°) = -1");
    }

    [Test]
    public void PolarAngle_45DegreeAngle_ShouldHaveCorrectValues()
    {
        // Arrange & Act
        var angle45 = LinFloat64PolarAngle.Angle45;
        var sqrt2Over2 = Math.Sqrt(2) / 2;

        // Assert
        // 45°: cos = sin = √2/2
        Assert.That(angle45.Cos().ScalarValue, Is.EqualTo(sqrt2Over2).Within(Tolerance),
            "cos(45°) = √2/2");
        Assert.That(angle45.Sin().ScalarValue, Is.EqualTo(sqrt2Over2).Within(Tolerance),
            "sin(45°) = √2/2");
        Assert.That(angle45.Radians.ScalarValue, Is.EqualTo(Math.PI / 4).Within(Tolerance),
            "45° = π/4 radians");
    }

    #endregion

    #region DirectedAngle Tests (3 tests)

    [Test]
    public void DirectedAngle_CreatePositiveAndNegative_ShouldHaveCorrectSigns()
    {
        // Arrange & Act
        var anglePlus90 = LinFloat64DirectedAngle.Angle90;
        var angleMinus90 = LinFloat64DirectedAngle.AngleMinus90;

        // Assert
        Assert.That(anglePlus90.Radians.ScalarValue, Is.EqualTo(Math.PI / 2).Within(Tolerance),
            "+90° should be positive π/2");
        Assert.That(angleMinus90.Radians.ScalarValue, Is.EqualTo(-Math.PI / 2).Within(Tolerance),
            "-90° should be negative π/2");

        // Cos should be same for ±90°, sin should be opposite
        Assert.That(anglePlus90.Cos().ScalarValue, Is.EqualTo(angleMinus90.Cos().ScalarValue).Within(Tolerance),
            "cos(90°) = cos(-90°)");
        Assert.That(anglePlus90.Sin().ScalarValue, Is.EqualTo(-angleMinus90.Sin().ScalarValue).Within(Tolerance),
            "sin(90°) = -sin(-90°)");
    }

    [Test]
    public void DirectedAngle_CreateFromDegrees_ShouldNormalizeToRange()
    {
        // Arrange & Act
        var angle450 = LinFloat64DirectedAngle.CreateFromDegrees(450);  // 450° = 90°
        var angleMinus450 = LinFloat64DirectedAngle.CreateFromDegrees(-450);  // -450° = -90°

        // Assert
        // DirectedAngle normalizes to [-360°, 360°] or similar range
        Assert.That(angle450.Radians.ScalarValue, Is.EqualTo(Math.PI / 2).Within(Tolerance),
            "450° should normalize to 90°");
        Assert.That(angleMinus450.Radians.ScalarValue, Is.EqualTo(-Math.PI / 2).Within(Tolerance),
            "-450° should normalize to -90°");
    }

    [Test]
    public void DirectedAngle_SpecialAngles_ShouldHaveCorrectValues()
    {
        // Arrange
        var angle0 = LinFloat64DirectedAngle.Angle0;
        var angle180 = LinFloat64DirectedAngle.Angle180;
        var angleMinus180 = LinFloat64DirectedAngle.AngleMinus180;

        // Assert
        Assert.That(angle0.Radians.ScalarValue, Is.EqualTo(0.0).Within(Tolerance),
            "0° = 0 radians");
        Assert.That(angle180.Radians.ScalarValue, Is.EqualTo(Math.PI).Within(Tolerance),
            "180° = π radians");
        Assert.That(angleMinus180.Radians.ScalarValue, Is.EqualTo(-Math.PI).Within(Tolerance),
            "-180° = -π radians");

        // Both ±180° should have same cos and sin
        Assert.That(angle180.Cos().ScalarValue, Is.EqualTo(angleMinus180.Cos().ScalarValue).Within(Tolerance),
            "cos(180°) = cos(-180°) = -1");
        Assert.That(Math.Abs(angle180.Sin().ScalarValue), Is.EqualTo(0.0).Within(Tolerance),
            "sin(±180°) ≈ 0");
    }

    #endregion
}
