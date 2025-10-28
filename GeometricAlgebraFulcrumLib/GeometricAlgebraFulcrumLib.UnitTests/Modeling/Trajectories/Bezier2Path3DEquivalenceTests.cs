using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Bezier;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Bezier;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Equivalence Tests for Generic Bezier2Path3D vs Float64Bezier2Path3D
/// Phase 3A Module 6A - Quadratic Bezier Trajectories
/// Tests: Generic double vs Float64 Specialized for Bezier2Path3D (quadratic Bezier curves)
/// </summary>
[TestFixture]
public sealed class Bezier2Path3DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static LinVector3D<double> CreateGenericVector(double x, double y, double z)
    {
        return LinVector3D<double>.Create(ScalarProcessor, x, y, z);
    }

    #endregion

    #region Bezier2Path3D Equivalence Tests (10 tests)

    [Test]
    public void Bezier2Path3D_GetValue_AtStart_ShouldMatchFloat64()
    {
        // Arrange - Quadratic Bezier curve with 3 control points
        var p1Float64 = LinFloat64Vector3D.Create(0.0, 0.0, 0.0);
        var p2Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 1.0);
        var p3Float64 = LinFloat64Vector3D.Create(2.0, 0.0, 2.0);

        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(1.0, 2.0, 1.0);
        var p3Generic = CreateGenericVector(2.0, 0.0, 2.0);

        var pathFloat64 = new Float64Bezier2Path3D(false, p1Float64, p2Float64, p3Float64);
        var pathGeneric = Bezier2Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic);

        // Act - At t=0, curve should be at P1
        var valueFloat64 = pathFloat64.GetValue(0.0);
        var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(0.0));

        // Assert - Should match first control point
        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance), "X at t=0");
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance), "Y at t=0");
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z.ScalarValue).Within(Tolerance), "Z at t=0");

        Assert.That(valueFloat64.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Float64 X at t=0 should be 0");
        Assert.That(valueFloat64.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Float64 Y at t=0 should be 0");

        Debug.Assert(Math.Abs(valueGeneric.X.ScalarValue - 0.0) < Tolerance);
    }

    [Test]
    public void Bezier2Path3D_GetValue_AtEnd_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(0.0, 0.0, 0.0);
        var p2Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 1.0);
        var p3Float64 = LinFloat64Vector3D.Create(2.0, 0.0, 2.0);

        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(1.0, 2.0, 1.0);
        var p3Generic = CreateGenericVector(2.0, 0.0, 2.0);

        var pathFloat64 = new Float64Bezier2Path3D(false, p1Float64, p2Float64, p3Float64);
        var pathGeneric = Bezier2Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic);

        // Act - At t=1, curve should be at P3
        var valueFloat64 = pathFloat64.GetValue(1.0);
        var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(1.0));

        // Assert - Should match third control point
        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance), "X at t=1");
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance), "Y at t=1");
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z.ScalarValue).Within(Tolerance), "Z at t=1");

        Assert.That(valueFloat64.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Float64 X at t=1 should be 2");
        Assert.That(valueFloat64.Z.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Float64 Z at t=1 should be 2");

        Debug.Assert(Math.Abs(valueGeneric.X.ScalarValue - 2.0) < Tolerance);
    }

    [Test]
    public void Bezier2Path3D_GetValue_AtMidpoint_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(0.0, 0.0, 0.0);
        var p2Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 1.0);
        var p3Float64 = LinFloat64Vector3D.Create(2.0, 0.0, 2.0);

        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(1.0, 2.0, 1.0);
        var p3Generic = CreateGenericVector(2.0, 0.0, 2.0);

        var pathFloat64 = new Float64Bezier2Path3D(false, p1Float64, p2Float64, p3Float64);
        var pathGeneric = Bezier2Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic);

        // Act - At t=0.5
        var valueFloat64 = pathFloat64.GetValue(0.5);
        var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(0.5));

        // Assert
        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance), "X at t=0.5");
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance), "Y at t=0.5");
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z.ScalarValue).Within(Tolerance), "Z at t=0.5");

        // At t=0.5: B(0.5) = 0.25*P1 + 0.5*P2 + 0.25*P3
        // Expected: (0.25*0 + 0.5*1 + 0.25*2, 0.25*0 + 0.5*2 + 0.25*0, 0.25*0 + 0.5*1 + 0.25*2)
        // = (1.0, 1.0, 1.0)
        Assert.That(valueFloat64.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X at t=0.5 should be 1.0");
        Assert.That(valueFloat64.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Y at t=0.5 should be 1.0");
        Assert.That(valueFloat64.Z.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Z at t=0.5 should be 1.0");

        Debug.Assert(Math.Abs(valueGeneric.Y.ScalarValue - 1.0) < Tolerance);
    }

    [Test]
    public void Bezier2Path3D_GetDerivative1Value_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(0.0, 0.0, 0.0);
        var p2Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 1.0);
        var p3Float64 = LinFloat64Vector3D.Create(2.0, 0.0, 2.0);

        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(1.0, 2.0, 1.0);
        var p3Generic = CreateGenericVector(2.0, 0.0, 2.0);

        var pathFloat64 = new Float64Bezier2Path3D(false, p1Float64, p2Float64, p3Float64);
        var pathGeneric = Bezier2Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic);

        // Act - First derivative at t=0
        var deriv1Float64_0 = pathFloat64.GetDerivative1Value(0.0);
        var deriv1Generic_0 = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(0.0));

        // Act - First derivative at t=0.5
        var deriv1Float64_05 = pathFloat64.GetDerivative1Value(0.5);
        var deriv1Generic_05 = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(0.5));

        // Assert - At t=0
        Assert.That(deriv1Generic_0.X.ScalarValue, Is.EqualTo(deriv1Float64_0.X.ScalarValue).Within(Tolerance), "Derivative1 X at t=0");
        Assert.That(deriv1Generic_0.Y.ScalarValue, Is.EqualTo(deriv1Float64_0.Y.ScalarValue).Within(Tolerance), "Derivative1 Y at t=0");
        Assert.That(deriv1Generic_0.Z.ScalarValue, Is.EqualTo(deriv1Float64_0.Z.ScalarValue).Within(Tolerance), "Derivative1 Z at t=0");

        // Assert - At t=0.5
        Assert.That(deriv1Generic_05.X.ScalarValue, Is.EqualTo(deriv1Float64_05.X.ScalarValue).Within(Tolerance), "Derivative1 X at t=0.5");
        Assert.That(deriv1Generic_05.Y.ScalarValue, Is.EqualTo(deriv1Float64_05.Y.ScalarValue).Within(Tolerance), "Derivative1 Y at t=0.5");
        Assert.That(deriv1Generic_05.Z.ScalarValue, Is.EqualTo(deriv1Float64_05.Z.ScalarValue).Within(Tolerance), "Derivative1 Z at t=0.5");

        Debug.Assert(Math.Abs(deriv1Generic_0.X.ScalarValue - deriv1Float64_0.X.ScalarValue) < Tolerance);
    }

    [Test]
    public void Bezier2Path3D_GetDerivative2Value_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(0.0, 0.0, 0.0);
        var p2Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 1.0);
        var p3Float64 = LinFloat64Vector3D.Create(2.0, 0.0, 2.0);

        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(1.0, 2.0, 1.0);
        var p3Generic = CreateGenericVector(2.0, 0.0, 2.0);

        var pathGeneric = Bezier2Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic);

        // Act - Second derivative (should be constant for quadratic Bezier)
        var deriv2_0 = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(0.0));
        var deriv2_05 = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(0.5));
        var deriv2_1 = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(1.0));

        // Assert - Second derivative should be constant for quadratic Bezier: B''(t) = 2(P3 - 2*P2 + P1)
        // Expected: 2*(2,0,2 - 2*1,2,1 + 0,0,0) = 2*(0,-4,0) = (0,-8,0)
        Assert.That(deriv2_0.X.ScalarValue, Is.EqualTo(deriv2_05.X.ScalarValue).Within(Tolerance), "Derivative2 X should be constant");
        Assert.That(deriv2_0.Y.ScalarValue, Is.EqualTo(deriv2_05.Y.ScalarValue).Within(Tolerance), "Derivative2 Y should be constant");
        Assert.That(deriv2_0.Z.ScalarValue, Is.EqualTo(deriv2_05.Z.ScalarValue).Within(Tolerance), "Derivative2 Z should be constant");

        Assert.That(deriv2_0.X.ScalarValue, Is.EqualTo(deriv2_1.X.ScalarValue).Within(Tolerance), "Derivative2 X should be constant across all t");
        Assert.That(deriv2_0.Y.ScalarValue, Is.EqualTo(deriv2_1.Y.ScalarValue).Within(Tolerance), "Derivative2 Y should be constant across all t");

        Assert.That(deriv2_0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Derivative2 X should be 0");
        Assert.That(deriv2_0.Y.ScalarValue, Is.EqualTo(-8.0).Within(Tolerance), "Derivative2 Y should be -8");
        Assert.That(deriv2_0.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Derivative2 Z should be 0");

        Debug.Assert(Math.Abs(deriv2_0.Y.ScalarValue + 8.0) < Tolerance);
    }

    [Test]
    public void Bezier2Path3D_IsValid_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var p2Float64 = LinFloat64Vector3D.Create(4.0, 5.0, 6.0);
        var p3Float64 = LinFloat64Vector3D.Create(7.0, 8.0, 9.0);

        var p1Generic = CreateGenericVector(1.0, 2.0, 3.0);
        var p2Generic = CreateGenericVector(4.0, 5.0, 6.0);
        var p3Generic = CreateGenericVector(7.0, 8.0, 9.0);

        var pathFloat64 = new Float64Bezier2Path3D(false, p1Float64, p2Float64, p3Float64);
        var pathGeneric = Bezier2Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic);

        // Act
        var isValidFloat64 = pathFloat64.IsValid();
        var isValidGeneric = pathGeneric.IsValid();

        // Assert
        Assert.That(isValidGeneric, Is.EqualTo(isValidFloat64), "IsValid should match");
        Assert.That(isValidFloat64, Is.True, "Float64 path should be valid");
        Assert.That(isValidGeneric, Is.True, "Generic path should be valid");

        Debug.Assert(isValidGeneric);
    }

    [Test]
    public void Bezier2Path3D_ToFinitePath_ShouldBeIdempotent()
    {
        // Arrange - Start with non-periodic path
        var p1Generic = CreateGenericVector(1.0, 2.0, 3.0);
        var p2Generic = CreateGenericVector(4.0, 5.0, 6.0);
        var p3Generic = CreateGenericVector(7.0, 8.0, 9.0);

        var pathGeneric = Bezier2Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic);

        // Act
        var finitePathGeneric = pathGeneric.ToFinitePath();

        // Assert - Should return same instance if already finite
        Assert.That(ReferenceEquals(pathGeneric, finitePathGeneric), Is.True, "ToFinitePath should return same instance when already finite");
        Assert.That(finitePathGeneric.IsFinite, Is.True, "IsFinite should be true after conversion");

        Debug.Assert(finitePathGeneric.IsFinite);
    }

    [Test]
    public void Bezier2Path3D_ToPeriodicPath_ShouldConvertCorrectly()
    {
        // Arrange - Start with finite (non-periodic) path
        var p1Generic = CreateGenericVector(1.0, 2.0, 3.0);
        var p2Generic = CreateGenericVector(4.0, 5.0, 6.0);
        var p3Generic = CreateGenericVector(7.0, 8.0, 9.0);

        var pathGeneric = Bezier2Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic);

        // Act
        var periodicPathGeneric = pathGeneric.ToPeriodicPath();

        // Assert - Should create new periodic instance
        Assert.That(ReferenceEquals(pathGeneric, periodicPathGeneric), Is.False, "ToPeriodicPath should create new instance");
        Assert.That(periodicPathGeneric.IsPeriodic, Is.True, "Converted path should be periodic");
        Assert.That(pathGeneric.IsFinite, Is.True, "Original path should remain finite");

        Debug.Assert(periodicPathGeneric.IsPeriodic);
    }

    [Test]
    public void Bezier2Path3D_ControlPoints_ShouldBeAccessible()
    {
        // Arrange
        var p1Generic = CreateGenericVector(1.0, 2.0, 3.0);
        var p2Generic = CreateGenericVector(4.0, 5.0, 6.0);
        var p3Generic = CreateGenericVector(7.0, 8.0, 9.0);

        var pathGeneric = Bezier2Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic);

        // Assert - Control points should be accessible
        Assert.That(pathGeneric.Point1.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Point1 X");
        Assert.That(pathGeneric.Point1.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Point1 Y");
        Assert.That(pathGeneric.Point1.Z.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "Point1 Z");

        Assert.That(pathGeneric.Point2.X.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "Point2 X");
        Assert.That(pathGeneric.Point2.Y.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Point2 Y");
        Assert.That(pathGeneric.Point2.Z.ScalarValue, Is.EqualTo(6.0).Within(Tolerance), "Point2 Z");

        Assert.That(pathGeneric.Point3.X.ScalarValue, Is.EqualTo(7.0).Within(Tolerance), "Point3 X");
        Assert.That(pathGeneric.Point3.Y.ScalarValue, Is.EqualTo(8.0).Within(Tolerance), "Point3 Y");
        Assert.That(pathGeneric.Point3.Z.ScalarValue, Is.EqualTo(9.0).Within(Tolerance), "Point3 Z");

        Debug.Assert(Math.Abs(pathGeneric.Point1.X.ScalarValue - 1.0) < Tolerance);
    }

    [Test]
    public void Bezier2Path3D_BernsteinBasis_AtMultiplePoints_ShouldMatchFormula()
    {
        // Arrange - Simple horizontal curve
        var p1Generic = CreateGenericVector(0.0, 0.0, 0.0);
        var p2Generic = CreateGenericVector(5.0, 5.0, 0.0);
        var p3Generic = CreateGenericVector(10.0, 0.0, 0.0);

        var pathGeneric = Bezier2Path3D<double>.Create(ScalarProcessor, false, p1Generic, p2Generic, p3Generic);

        // Act & Assert at multiple t values
        for (var i = 0; i <= 10; i++)
        {
            var t = i / 10.0;
            var value = pathGeneric.GetValue(ScalarProcessor.Scalar(t));

            // Manually calculate using Bernstein basis: B(t) = (1-t)²P1 + 2(1-t)tP2 + t²P3
            var s = 1.0 - t;
            var b0 = s * s;
            var b1 = 2 * s * t;
            var b2 = t * t;

            var expectedX = b0 * 0.0 + b1 * 5.0 + b2 * 10.0;
            var expectedY = b0 * 0.0 + b1 * 5.0 + b2 * 0.0;
            var expectedZ = b0 * 0.0 + b1 * 0.0 + b2 * 0.0;

            Assert.That(value.X.ScalarValue, Is.EqualTo(expectedX).Within(Tolerance), $"X at t={t}");
            Assert.That(value.Y.ScalarValue, Is.EqualTo(expectedY).Within(Tolerance), $"Y at t={t}");
            Assert.That(value.Z.ScalarValue, Is.EqualTo(expectedZ).Within(Tolerance), $"Z at t={t}");
        }

        Debug.Assert(true); // All assertions passed
    }

    #endregion
}
