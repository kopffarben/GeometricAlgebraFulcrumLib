using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra.Euclidean;

/// <summary>
/// Tests for Angle Operations
/// EXTENDED: Tests both Float64 AND Generic&lt;double&gt; implementations
/// Phase 2 - Validates API parity between Float64 and Generic&lt;double&gt;
/// </summary>
[TestFixture]
public class LinAngleTests
{
    private const double Tolerance = 1e-10;
    private IScalarProcessor<double> _scalarProcessor = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _scalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    #region Helper Methods

    private object CreatePolarAngleFromDegrees(double degrees, bool useGeneric)
    {
        if (useGeneric)
        {
            return LinPolarAngle<double>.CreateFromDegrees(
                _scalarProcessor.ScalarFromNumber(degrees)
            );
        }
        else
        {
            return LinFloat64PolarAngle.CreateFromDegrees(degrees);
        }
    }

    private object CreateDirectedAngleFromDegrees(double degrees, bool useGeneric)
    {
        if (useGeneric)
        {
            return LinDirectedAngle<double>.CreateFromDegrees(
                _scalarProcessor.ScalarFromNumber(degrees)
            );
        }
        else
        {
            return LinFloat64DirectedAngle.CreateFromDegrees(degrees);
        }
    }

    private object GetPolarAngle0(bool useGeneric)
    {
        return useGeneric
            ? LinPolarAngle<double>.Angle0(_scalarProcessor)
            : LinFloat64PolarAngle.Angle0;
    }

    private object GetPolarAngle45(bool useGeneric)
    {
        return useGeneric
            ? LinPolarAngle<double>.Angle45(_scalarProcessor)
            : LinFloat64PolarAngle.Angle45;
    }

    private object GetPolarAngle90(bool useGeneric)
    {
        return useGeneric
            ? LinPolarAngle<double>.Angle90(_scalarProcessor)
            : LinFloat64PolarAngle.Angle90;
    }

    private object GetPolarAngle180(bool useGeneric)
    {
        return useGeneric
            ? LinPolarAngle<double>.Angle180(_scalarProcessor)
            : LinFloat64PolarAngle.Angle180;
    }

    private object GetPolarAngle270(bool useGeneric)
    {
        return useGeneric
            ? LinPolarAngle<double>.Angle270(_scalarProcessor)
            : LinFloat64PolarAngle.Angle270;
    }

    private object GetDirectedAngle0(bool useGeneric)
    {
        return useGeneric
            ? LinDirectedAngle<double>.Angle0(_scalarProcessor)
            : LinFloat64DirectedAngle.Angle0;
    }

    private object GetDirectedAngle90(bool useGeneric)
    {
        return useGeneric
            ? LinDirectedAngle<double>.Angle90(_scalarProcessor)
            : LinFloat64DirectedAngle.Angle90;
    }

    private object GetDirectedAngleMinus90(bool useGeneric)
    {
        return useGeneric
            ? LinDirectedAngle<double>.AngleMinus90(_scalarProcessor)
            : LinFloat64DirectedAngle.AngleMinus90;
    }

    private object GetDirectedAngle180(bool useGeneric)
    {
        return useGeneric
            ? LinDirectedAngle<double>.Angle180(_scalarProcessor)
            : LinFloat64DirectedAngle.Angle180;
    }

    private object GetDirectedAngleMinus180(bool useGeneric)
    {
        return useGeneric
            ? LinDirectedAngle<double>.AngleMinus180(_scalarProcessor)
            : LinFloat64DirectedAngle.AngleMinus180;
    }

    private double GetRadians(object angle)
    {
        return angle switch
        {
            LinFloat64PolarAngle f64p => f64p.Radians.ScalarValue,
            LinPolarAngle<double> genp => genp.Radians.ScalarValue,
            LinFloat64DirectedAngle f64d => f64d.Radians.ScalarValue,
            LinDirectedAngle<double> gend => gend.Radians.ScalarValue,
            _ => throw new ArgumentException($"Unexpected angle type: {angle.GetType()}")
        };
    }

    private double GetCos(object angle)
    {
        return angle switch
        {
            LinFloat64PolarAngle f64p => f64p.Cos().ScalarValue,
            LinPolarAngle<double> genp => genp.Cos().ScalarValue,
            LinFloat64DirectedAngle f64d => f64d.Cos().ScalarValue,
            LinDirectedAngle<double> gend => gend.Cos().ScalarValue,
            _ => throw new ArgumentException($"Unexpected angle type: {angle.GetType()}")
        };
    }

    private double GetSin(object angle)
    {
        return angle switch
        {
            LinFloat64PolarAngle f64p => f64p.Sin().ScalarValue,
            LinPolarAngle<double> genp => genp.Sin().ScalarValue,
            LinFloat64DirectedAngle f64d => f64d.Sin().ScalarValue,
            LinDirectedAngle<double> gend => gend.Sin().ScalarValue,
            _ => throw new ArgumentException($"Unexpected angle type: {angle.GetType()}")
        };
    }

    #endregion

    #region PolarAngle Tests (3 tests × 2 implementations = 6 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void PolarAngle_CreateFromDegrees_ShouldHaveCorrectRadians(bool useGeneric)
    {
        // Arrange & Act
        var angle90 = CreatePolarAngleFromDegrees(90, useGeneric);
        var angle180 = CreatePolarAngleFromDegrees(180, useGeneric);
        var angle270 = CreatePolarAngleFromDegrees(270, useGeneric);

        // Assert
        Assert.That(GetRadians(angle90), Is.EqualTo(Math.PI / 2).Within(Tolerance),
            "90° should be π/2 radians");
        Assert.That(GetRadians(angle180), Is.EqualTo(Math.PI).Within(Tolerance),
            "180° should be π radians");
        Assert.That(GetRadians(angle270), Is.EqualTo(3 * Math.PI / 2).Within(Tolerance),
            "270° should be 3π/2 radians");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void PolarAngle_CosSinValues_ShouldBeCorrect(bool useGeneric)
    {
        // Arrange & Act
        var angle0 = GetPolarAngle0(useGeneric);
        var angle90 = GetPolarAngle90(useGeneric);
        var angle180 = GetPolarAngle180(useGeneric);
        var angle270 = GetPolarAngle270(useGeneric);

        // Assert
        // 0°: cos = 1, sin = 0
        Assert.That(GetCos(angle0), Is.EqualTo(1.0).Within(Tolerance), "cos(0°) = 1");
        Assert.That(GetSin(angle0), Is.EqualTo(0.0).Within(Tolerance), "sin(0°) = 0");

        // 90°: cos = 0, sin = 1
        Assert.That(GetCos(angle90), Is.EqualTo(0.0).Within(Tolerance), "cos(90°) = 0");
        Assert.That(GetSin(angle90), Is.EqualTo(1.0).Within(Tolerance), "sin(90°) = 1");

        // 180°: cos = -1, sin = 0
        Assert.That(GetCos(angle180), Is.EqualTo(-1.0).Within(Tolerance), "cos(180°) = -1");
        Assert.That(GetSin(angle180), Is.EqualTo(0.0).Within(Tolerance), "sin(180°) = 0");

        // 270°: cos = 0, sin = -1
        Assert.That(GetCos(angle270), Is.EqualTo(0.0).Within(Tolerance), "cos(270°) = 0");
        Assert.That(GetSin(angle270), Is.EqualTo(-1.0).Within(Tolerance), "sin(270°) = -1");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void PolarAngle_45DegreeAngle_ShouldHaveCorrectValues(bool useGeneric)
    {
        // Arrange & Act
        var angle45 = GetPolarAngle45(useGeneric);
        var sqrt2Over2 = Math.Sqrt(2) / 2;

        // Assert
        // 45°: cos = sin = √2/2
        Assert.That(GetCos(angle45), Is.EqualTo(sqrt2Over2).Within(Tolerance),
            "cos(45°) = √2/2");
        Assert.That(GetSin(angle45), Is.EqualTo(sqrt2Over2).Within(Tolerance),
            "sin(45°) = √2/2");
        Assert.That(GetRadians(angle45), Is.EqualTo(Math.PI / 4).Within(Tolerance),
            "45° = π/4 radians");
    }

    #endregion

    #region DirectedAngle Tests (3 tests × 2 implementations = 6 test cases)

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void DirectedAngle_CreatePositiveAndNegative_ShouldHaveCorrectSigns(bool useGeneric)
    {
        // Arrange & Act
        var anglePlus90 = GetDirectedAngle90(useGeneric);
        var angleMinus90 = GetDirectedAngleMinus90(useGeneric);

        // Assert
        Assert.That(GetRadians(anglePlus90), Is.EqualTo(Math.PI / 2).Within(Tolerance),
            "+90° should be positive π/2");
        Assert.That(GetRadians(angleMinus90), Is.EqualTo(-Math.PI / 2).Within(Tolerance),
            "-90° should be negative π/2");

        // Cos should be same for ±90°, sin should be opposite
        Assert.That(GetCos(anglePlus90), Is.EqualTo(GetCos(angleMinus90)).Within(Tolerance),
            "cos(90°) = cos(-90°)");
        Assert.That(GetSin(anglePlus90), Is.EqualTo(-GetSin(angleMinus90)).Within(Tolerance),
            "sin(90°) = -sin(-90°)");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void DirectedAngle_CreateFromDegrees_ShouldNormalizeToRange(bool useGeneric)
    {
        // Arrange & Act
        var angle450 = CreateDirectedAngleFromDegrees(450, useGeneric);  // 450° = 90°
        var angleMinus450 = CreateDirectedAngleFromDegrees(-450, useGeneric);  // -450° = -90°

        // Assert
        // DirectedAngle normalizes to [-360°, 360°] or similar range
        Assert.That(GetRadians(angle450), Is.EqualTo(Math.PI / 2).Within(Tolerance),
            "450° should normalize to 90°");
        Assert.That(GetRadians(angleMinus450), Is.EqualTo(-Math.PI / 2).Within(Tolerance),
            "-450° should normalize to -90°");
    }

    [TestCase(false, Description = "Float64 Implementation")]
    [TestCase(true, Description = "Generic<double> Implementation")]
    public void DirectedAngle_SpecialAngles_ShouldHaveCorrectValues(bool useGeneric)
    {
        // Arrange
        var angle0 = GetDirectedAngle0(useGeneric);
        var angle180 = GetDirectedAngle180(useGeneric);
        var angleMinus180 = GetDirectedAngleMinus180(useGeneric);

        // Assert
        Assert.That(GetRadians(angle0), Is.EqualTo(0.0).Within(Tolerance),
            "0° = 0 radians");
        Assert.That(GetRadians(angle180), Is.EqualTo(Math.PI).Within(Tolerance),
            "180° = π radians");
        Assert.That(GetRadians(angleMinus180), Is.EqualTo(-Math.PI).Within(Tolerance),
            "-180° = -π radians");

        // Both ±180° should have same cos and sin
        Assert.That(GetCos(angle180), Is.EqualTo(GetCos(angleMinus180)).Within(Tolerance),
            "cos(180°) = cos(-180°) = -1");
        Assert.That(Math.Abs(GetSin(angle180)), Is.EqualTo(0.0).Within(Tolerance),
            "sin(±180°) ≈ 0");
    }

    #endregion
}
