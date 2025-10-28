using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Equivalence Tests for Generic ParametricPath3D vs Float64Path3D
/// Phase 3A Module 6A - Generic Trajectories (Simplified Version)
/// Tests: Generic double vs Float64 Specialized for ConstantPath3D
/// </summary>
[TestFixture]
public class ParametricPath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static LinVector3D<double> CreateGenericVector(double x, double y, double z)
    {
        return LinVector3D<double>.Create(ScalarProcessor, x, y, z);
    }

    private static ScalarRange<double> CreateGenericRange(double min, double max)
    {
        return ScalarRange<double>.Create(
            ScalarProcessor.Scalar(min),
            ScalarProcessor.Scalar(max)
        );
    }

    #endregion

    #region ConstantPath3D Equivalence Tests (10 tests)

    [Test]
    public void ConstantPath3D_GetValue_ShouldMatchFloat64()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(3.0, 4.0, 5.0);
        var pointGeneric = CreateGenericVector(3.0, 4.0, 5.0);

        var pathFloat64 = Float64ConstantPath3D.Finite(
            Float64ScalarRange.Create(-1, 1),
            point
        );

        var pathGeneric = ConstantPath3D<double>.Finite(
            CreateGenericRange(-1, 1),
            pointGeneric
        );

        // Act
        var valueFloat64_0 = pathFloat64.GetValue(0.0);
        var valueGeneric_0 = pathGeneric.GetValue(ScalarProcessor.Scalar(0.0));

        var valueFloat64_05 = pathFloat64.GetValue(0.5);
        var valueGeneric_05 = pathGeneric.GetValue(ScalarProcessor.Scalar(0.5));

        // Assert
        Assert.That(valueGeneric_0.X.ScalarValue, Is.EqualTo(valueFloat64_0.X.ScalarValue).Within(Tolerance), "X at t=0");
        Assert.That(valueGeneric_0.Y.ScalarValue, Is.EqualTo(valueFloat64_0.Y.ScalarValue).Within(Tolerance), "Y at t=0");
        Assert.That(valueGeneric_0.Z.ScalarValue, Is.EqualTo(valueFloat64_0.Z.ScalarValue).Within(Tolerance), "Z at t=0");

        Assert.That(valueGeneric_05.X.ScalarValue, Is.EqualTo(valueFloat64_05.X.ScalarValue).Within(Tolerance), "X at t=0.5");
        Assert.That(valueGeneric_05.Y.ScalarValue, Is.EqualTo(valueFloat64_05.Y.ScalarValue).Within(Tolerance), "Y at t=0.5");
        Assert.That(valueGeneric_05.Z.ScalarValue, Is.EqualTo(valueFloat64_05.Z.ScalarValue).Within(Tolerance), "Z at t=0.5");
    }

    [Test]
    public void ConstantPath3D_GetDerivative1Value_ShouldMatchFloat64()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var tangent = LinFloat64Vector3D.Create(0.5, 0.5, 0.0);

        var pointGeneric = CreateGenericVector(1.0, 2.0, 3.0);
        var tangentGeneric = CreateGenericVector(0.5, 0.5, 0.0);

        var pathFloat64 = Float64ConstantPath3D.Finite(
            Float64ScalarRange.Create(-1, 1),
            point,
            tangent
        );

        var pathGeneric = ConstantPath3D<double>.Finite(
            CreateGenericRange(-1, 1),
            pointGeneric,
            tangentGeneric
        );

        // Act
        var derivative1Float64 = pathFloat64.GetDerivative1Value(0.0);
        var derivative1Generic = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(0.0));

        // Assert
        Assert.That(derivative1Generic.X.ScalarValue, Is.EqualTo(derivative1Float64.X.ScalarValue).Within(Tolerance), "Derivative1 X");
        Assert.That(derivative1Generic.Y.ScalarValue, Is.EqualTo(derivative1Float64.Y.ScalarValue).Within(Tolerance), "Derivative1 Y");
        Assert.That(derivative1Generic.Z.ScalarValue, Is.EqualTo(derivative1Float64.Z.ScalarValue).Within(Tolerance), "Derivative1 Z");
    }

    [Test]
    public void ConstantPath3D_GetDerivative2Value_ShouldMatchFloat64()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var pointGeneric = CreateGenericVector(1.0, 2.0, 3.0);

        var pathFloat64 = Float64ConstantPath3D.Finite(
            Float64ScalarRange.Create(-1, 1),
            point
        );

        var pathGeneric = ConstantPath3D<double>.Finite(
            CreateGenericRange(-1, 1),
            pointGeneric
        );

        // Act
        var derivative2Float64 = pathFloat64.GetDerivative2Value(0.0);
        var derivative2Generic = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(0.0));

        // Assert - Second derivative of constant path should be zero
        Assert.That(derivative2Generic.X.ScalarValue, Is.EqualTo(derivative2Float64.X.ScalarValue).Within(Tolerance), "Derivative2 X should be 0");
        Assert.That(derivative2Generic.Y.ScalarValue, Is.EqualTo(derivative2Float64.Y.ScalarValue).Within(Tolerance), "Derivative2 Y should be 0");
        Assert.That(derivative2Generic.Z.ScalarValue, Is.EqualTo(derivative2Float64.Z.ScalarValue).Within(Tolerance), "Derivative2 Z should be 0");

        Assert.That(derivative2Float64.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Float64 Derivative2 should be zero");
        Assert.That(derivative2Generic.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Generic Derivative2 should be zero");
    }

    [Test]
    public void ConstantPath3D_GetFrame_ShouldMatchFloat64()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1.0, 0.0, 0.0);
        var tangent = LinFloat64Vector3D.Create(0.0, 1.0, 0.0);

        var pointGeneric = CreateGenericVector(1.0, 0.0, 0.0);
        var tangentGeneric = CreateGenericVector(0.0, 1.0, 0.0);

        var pathFloat64 = Float64ConstantPath3D.Finite(
            Float64ScalarRange.Create(-1, 1),
            point,
            tangent
        );

        var pathGeneric = ConstantPath3D<double>.Finite(
            CreateGenericRange(-1, 1),
            pointGeneric,
            tangentGeneric
        );

        // Act
        var frameFloat64 = pathFloat64.GetFrame(0.5);
        var frameGeneric = pathGeneric.GetFrame(ScalarProcessor.Scalar(0.5));

        // Assert - Point should match
        Assert.That(frameGeneric.Point.X.ScalarValue, Is.EqualTo(frameFloat64.Point.X.ScalarValue).Within(Tolerance), "Frame Point X");
        Assert.That(frameGeneric.Point.Y.ScalarValue, Is.EqualTo(frameFloat64.Point.Y.ScalarValue).Within(Tolerance), "Frame Point Y");
        Assert.That(frameGeneric.Point.Z.ScalarValue, Is.EqualTo(frameFloat64.Point.Z.ScalarValue).Within(Tolerance), "Frame Point Z");

        // Tangent should match (normalized)
        Assert.That(frameGeneric.Tangent.X.ScalarValue, Is.EqualTo(frameFloat64.Tangent.X.ScalarValue).Within(Tolerance), "Frame Tangent X");
        Assert.That(frameGeneric.Tangent.Y.ScalarValue, Is.EqualTo(frameFloat64.Tangent.Y.ScalarValue).Within(Tolerance), "Frame Tangent Y");
        Assert.That(frameGeneric.Tangent.Z.ScalarValue, Is.EqualTo(frameFloat64.Tangent.Z.ScalarValue).Within(Tolerance), "Frame Tangent Z");
    }

    [Test]
    public void ConstantPath3D_TimeRange_MinTime_ShouldMatchFloat64()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var pointGeneric = CreateGenericVector(1.0, 2.0, 3.0);

        var pathFloat64 = Float64ConstantPath3D.Finite(
            Float64ScalarRange.Create(5.0, 10.0),
            point
        );

        var pathGeneric = ConstantPath3D<double>.Finite(
            CreateGenericRange(5.0, 10.0),
            pointGeneric
        );

        // Assert
        Assert.That(pathGeneric.MinTime.ScalarValue, Is.EqualTo(pathFloat64.MinTime).Within(Tolerance), "MinTime should match");
    }

    [Test]
    public void ConstantPath3D_TimeRange_MaxTime_ShouldMatchFloat64()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var pointGeneric = CreateGenericVector(1.0, 2.0, 3.0);

        var pathFloat64 = Float64ConstantPath3D.Finite(
            Float64ScalarRange.Create(5.0, 10.0),
            point
        );

        var pathGeneric = ConstantPath3D<double>.Finite(
            CreateGenericRange(5.0, 10.0),
            pointGeneric
        );

        // Assert
        Assert.That(pathGeneric.MaxTime.ScalarValue, Is.EqualTo(pathFloat64.MaxTime).Within(Tolerance), "MaxTime should match");
    }

    [Test]
    public void ConstantPath3D_TimeRange_MidTime_ShouldMatchFloat64()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var pointGeneric = CreateGenericVector(1.0, 2.0, 3.0);

        var pathFloat64 = Float64ConstantPath3D.Finite(
            Float64ScalarRange.Create(0.0, 10.0),
            point
        );

        var pathGeneric = ConstantPath3D<double>.Finite(
            CreateGenericRange(0.0, 10.0),
            pointGeneric
        );

        // Assert
        Assert.That(pathGeneric.MidTime.ScalarValue, Is.EqualTo(pathFloat64.MidTime).Within(Tolerance), "MidTime should match");
        Assert.That(pathFloat64.MidTime, Is.EqualTo(5.0).Within(Tolerance), "Float64 MidTime should be 5.0");
        Assert.That(pathGeneric.MidTime.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Generic MidTime should be 5.0");
    }

    [Test]
    public void ConstantPath3D_IsPeriodic_IsFinite_ShouldMatchFloat64()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var pointGeneric = CreateGenericVector(1.0, 2.0, 3.0);

        var pathFloat64 = Float64ConstantPath3D.Finite(
            Float64ScalarRange.Create(-1, 1),
            point
        );

        var pathGeneric = ConstantPath3D<double>.Finite(
            CreateGenericRange(-1, 1),
            pointGeneric
        );

        // Assert
        Assert.That(pathGeneric.IsPeriodic, Is.EqualTo(pathFloat64.IsPeriodic), "IsPeriodic should match");
        Assert.That(pathGeneric.IsFinite, Is.EqualTo(pathFloat64.IsFinite), "IsFinite should match");
        Assert.That(pathFloat64.IsPeriodic, Is.False, "Float64 should be finite (not periodic)");
        Assert.That(pathGeneric.IsPeriodic, Is.False, "Generic should be finite (not periodic)");
    }

    [Test]
    public void ConstantPath3D_ToFinitePath_ShouldMatchFloat64Behavior()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var pointGeneric = CreateGenericVector(1.0, 2.0, 3.0);

        var pathFloat64 = Float64ConstantPath3D.Finite(
            Float64ScalarRange.Create(-1, 1),
            point
        );

        var pathGeneric = ConstantPath3D<double>.Finite(
            CreateGenericRange(-1, 1),
            pointGeneric
        );

        // Act
        var finitePathFloat64 = pathFloat64.ToFinitePath();
        var finitePathGeneric = pathGeneric.ToFinitePath();

        // Assert - Should return same instance if already finite
        Assert.That(ReferenceEquals(pathFloat64, finitePathFloat64), Is.True, "Float64: ToFinitePath should return same instance when already finite");
        Assert.That(ReferenceEquals(pathGeneric, finitePathGeneric), Is.True, "Generic: ToFinitePath should return same instance when already finite");

        Assert.That(finitePathGeneric.IsFinite, Is.EqualTo(finitePathFloat64.IsFinite), "IsFinite should match after conversion");
    }

    [Test]
    public void ConstantPath3D_IsValid_ShouldMatchFloat64()
    {
        // Arrange
        var point = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var pointGeneric = CreateGenericVector(1.0, 2.0, 3.0);

        var pathFloat64 = Float64ConstantPath3D.Finite(
            Float64ScalarRange.Create(-1, 1),
            point
        );

        var pathGeneric = ConstantPath3D<double>.Finite(
            CreateGenericRange(-1, 1),
            pointGeneric
        );

        // Act
        var isValidFloat64 = pathFloat64.IsValid();
        var isValidGeneric = pathGeneric.IsValid();

        // Assert
        Assert.That(isValidGeneric, Is.EqualTo(isValidFloat64), "IsValid should match");
        Assert.That(isValidFloat64, Is.True, "Float64 path should be valid");
        Assert.That(isValidGeneric, Is.True, "Generic path should be valid");
    }

    #endregion
}
