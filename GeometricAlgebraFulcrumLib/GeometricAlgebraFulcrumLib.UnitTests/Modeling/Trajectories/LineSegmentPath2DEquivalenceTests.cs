using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Equivalence Tests for Generic LineSegmentPath2D vs Float64LineSegmentPath2D
/// Phase 3 Module 6B - Line Segment Trajectories
/// Tests: Generic double vs Float64 Specialized for LineSegmentPath2D
/// </summary>
[TestFixture]
public class LineSegmentPath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    private static LinVector2D<double> CreateGenericVector(double x, double y)
    {
        return LinVector2D<double>.Create(ScalarProcessor, x, y);
    }

    #endregion

    #region LineSegmentPath2D Equivalence Tests (12 tests)

    [Test]
    public void LineSegmentPath2D_GetValue_ShouldMatchFloat64()
    {
        // Arrange
        var point1Float64 = LinFloat64Vector2D.Create(1.0, 2.0);
        var point2Float64 = LinFloat64Vector2D.Create(4.0, 6.0);

        var point1Generic = CreateGenericVector(1.0, 2.0);
        var point2Generic = CreateGenericVector(4.0, 6.0);

        var pathFloat64 = Float64LineSegmentPath2D.Create(false, point1Float64, point2Float64);
        var pathGeneric = LineSegmentPath2D<double>.Create(false, point1Generic, point2Generic);

        // Act & Assert at t=0
        var valueFloat64_0 = pathFloat64.GetValue(0.0);
        var valueGeneric_0 = pathGeneric.GetValue(ScalarProcessor.Scalar(0.0));

        Assert.That(valueGeneric_0.X.ScalarValue, Is.EqualTo(valueFloat64_0.X.ScalarValue).Within(Tolerance), "X at t=0");
        Assert.That(valueGeneric_0.Y.ScalarValue, Is.EqualTo(valueFloat64_0.Y.ScalarValue).Within(Tolerance), "Y at t=0");

        // Act & Assert at t=0.5
        var valueFloat64_05 = pathFloat64.GetValue(0.5);
        var valueGeneric_05 = pathGeneric.GetValue(ScalarProcessor.Scalar(0.5));

        Assert.That(valueGeneric_05.X.ScalarValue, Is.EqualTo(valueFloat64_05.X.ScalarValue).Within(Tolerance), "X at t=0.5");
        Assert.That(valueGeneric_05.Y.ScalarValue, Is.EqualTo(valueFloat64_05.Y.ScalarValue).Within(Tolerance), "Y at t=0.5");

        // Act & Assert at t=1.0
        var valueFloat64_1 = pathFloat64.GetValue(1.0);
        var valueGeneric_1 = pathGeneric.GetValue(ScalarProcessor.Scalar(1.0));

        Assert.That(valueGeneric_1.X.ScalarValue, Is.EqualTo(valueFloat64_1.X.ScalarValue).Within(Tolerance), "X at t=1.0");
        Assert.That(valueGeneric_1.Y.ScalarValue, Is.EqualTo(valueFloat64_1.Y.ScalarValue).Within(Tolerance), "Y at t=1.0");
    }

    [Test]
    public void LineSegmentPath2D_GetDerivative1Value_ShouldMatchFloat64()
    {
        // Arrange
        var point1Float64 = LinFloat64Vector2D.Create(0.0, 0.0);
        var point2Float64 = LinFloat64Vector2D.Create(3.0, 4.0);

        var point1Generic = CreateGenericVector(0.0, 0.0);
        var point2Generic = CreateGenericVector(3.0, 4.0);

        var pathFloat64 = Float64LineSegmentPath2D.Create(false, point1Float64, point2Float64);
        var pathGeneric = LineSegmentPath2D<double>.Create(false, point1Generic, point2Generic);

        // Act - Derivative should be constant for line segment
        var derivative1Float64_0 = pathFloat64.GetDerivative1Value(0.0);
        var derivative1Generic_0 = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(0.0));

        var derivative1Float64_05 = pathFloat64.GetDerivative1Value(0.5);
        var derivative1Generic_05 = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(0.5));

        // Assert
        Assert.That(derivative1Generic_0.X.ScalarValue, Is.EqualTo(derivative1Float64_0.X.ScalarValue).Within(Tolerance), "Derivative1 X at t=0");
        Assert.That(derivative1Generic_0.Y.ScalarValue, Is.EqualTo(derivative1Float64_0.Y.ScalarValue).Within(Tolerance), "Derivative1 Y at t=0");

        Assert.That(derivative1Generic_05.X.ScalarValue, Is.EqualTo(derivative1Float64_05.X.ScalarValue).Within(Tolerance), "Derivative1 X at t=0.5");
        Assert.That(derivative1Generic_05.Y.ScalarValue, Is.EqualTo(derivative1Float64_05.Y.ScalarValue).Within(Tolerance), "Derivative1 Y at t=0.5");
    }

    [Test]
    public void LineSegmentPath2D_GetDerivative2Value_ShouldMatchFloat64()
    {
        // Arrange
        var point1Float64 = LinFloat64Vector2D.Create(1.0, 2.0);
        var point2Float64 = LinFloat64Vector2D.Create(4.0, 5.0);

        var point1Generic = CreateGenericVector(1.0, 2.0);
        var point2Generic = CreateGenericVector(4.0, 5.0);

        var pathFloat64 = Float64LineSegmentPath2D.Create(false, point1Float64, point2Float64);
        var pathGeneric = LineSegmentPath2D<double>.Create(false, point1Generic, point2Generic);

        // Act
        var derivative2Float64 = pathFloat64.GetDerivative2Value(0.0);
        var derivative2Generic = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(0.0));

        // Assert - Second derivative of line segment should be zero (no acceleration)
        Assert.That(derivative2Generic.X.ScalarValue, Is.EqualTo(derivative2Float64.X.ScalarValue).Within(Tolerance), "Derivative2 X should be 0");
        Assert.That(derivative2Generic.Y.ScalarValue, Is.EqualTo(derivative2Float64.Y.ScalarValue).Within(Tolerance), "Derivative2 Y should be 0");

        Assert.That(derivative2Float64.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Float64 Derivative2 should be zero");
        Assert.That(derivative2Generic.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Generic Derivative2 should be zero");
    }

    [Test]
    public void LineSegmentPath2D_GetLength_ShouldMatchFloat64()
    {
        // Arrange - 3-4-5 right triangle
        var point1Float64 = LinFloat64Vector2D.Create(0.0, 0.0);
        var point2Float64 = LinFloat64Vector2D.Create(3.0, 4.0);

        var point1Generic = CreateGenericVector(0.0, 0.0);
        var point2Generic = CreateGenericVector(3.0, 4.0);

        var pathFloat64 = Float64LineSegmentPath2D.Create(false, point1Float64, point2Float64);
        var pathGeneric = LineSegmentPath2D<double>.Create(false, point1Generic, point2Generic);

        // Act
        var lengthFloat64 = pathFloat64.GetLength();
        var lengthGeneric = pathGeneric.GetLength();

        // Assert - Should be 5.0 (Pythagorean theorem: sqrt(3^2 + 4^2))
        Assert.That(lengthGeneric.ScalarValue, Is.EqualTo(lengthFloat64.ScalarValue).Within(Tolerance), "Length should match");
        Assert.That(lengthFloat64.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Float64 length should be 5.0");
        Assert.That(lengthGeneric.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Generic length should be 5.0");
    }

    [Test]
    public void LineSegmentPath2D_TimeToLength_ShouldMatchFloat64()
    {
        // Arrange
        var point1Float64 = LinFloat64Vector2D.Create(0.0, 0.0);
        var point2Float64 = LinFloat64Vector2D.Create(10.0, 0.0);

        var point1Generic = CreateGenericVector(0.0, 0.0);
        var point2Generic = CreateGenericVector(10.0, 0.0);

        var pathFloat64 = Float64LineSegmentPath2D.Create(false, point1Float64, point2Float64);
        var pathGeneric = LineSegmentPath2D<double>.Create(false, point1Generic, point2Generic);

        // Act
        var lengthFloat64_0 = pathFloat64.TimeToLength(0.0);
        var lengthGeneric_0 = pathGeneric.TimeToLength(ScalarProcessor.Scalar(0.0));

        var lengthFloat64_05 = pathFloat64.TimeToLength(0.5);
        var lengthGeneric_05 = pathGeneric.TimeToLength(ScalarProcessor.Scalar(0.5));

        var lengthFloat64_1 = pathFloat64.TimeToLength(1.0);
        var lengthGeneric_1 = pathGeneric.TimeToLength(ScalarProcessor.Scalar(1.0));

        // Assert
        Assert.That(lengthGeneric_0.ScalarValue, Is.EqualTo(lengthFloat64_0.ScalarValue).Within(Tolerance), "Length at t=0");
        Assert.That(lengthGeneric_05.ScalarValue, Is.EqualTo(lengthFloat64_05.ScalarValue).Within(Tolerance), "Length at t=0.5");
        Assert.That(lengthGeneric_1.ScalarValue, Is.EqualTo(lengthFloat64_1.ScalarValue).Within(Tolerance), "Length at t=1.0");

        Assert.That(lengthFloat64_0.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Length at t=0 should be 0");
        Assert.That(lengthFloat64_05.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Length at t=0.5 should be 5");
        Assert.That(lengthFloat64_1.ScalarValue, Is.EqualTo(10.0).Within(Tolerance), "Length at t=1 should be 10");
    }

    [Test]
    public void LineSegmentPath2D_LengthToTime_ShouldMatchFloat64()
    {
        // Arrange
        var point1Float64 = LinFloat64Vector2D.Create(0.0, 0.0);
        var point2Float64 = LinFloat64Vector2D.Create(10.0, 0.0);

        var point1Generic = CreateGenericVector(0.0, 0.0);
        var point2Generic = CreateGenericVector(10.0, 0.0);

        var pathFloat64 = Float64LineSegmentPath2D.Create(false, point1Float64, point2Float64);
        var pathGeneric = LineSegmentPath2D<double>.Create(false, point1Generic, point2Generic);

        // Act
        var timeFloat64_0 = pathFloat64.LengthToTime(0.0);
        var timeGeneric_0 = pathGeneric.LengthToTime(ScalarProcessor.Scalar(0.0));

        var timeFloat64_5 = pathFloat64.LengthToTime(5.0);
        var timeGeneric_5 = pathGeneric.LengthToTime(ScalarProcessor.Scalar(5.0));

        var timeFloat64_10 = pathFloat64.LengthToTime(10.0);
        var timeGeneric_10 = pathGeneric.LengthToTime(ScalarProcessor.Scalar(10.0));

        // Assert
        Assert.That(timeGeneric_0.ScalarValue, Is.EqualTo(timeFloat64_0.ScalarValue).Within(Tolerance), "Time at length=0");
        Assert.That(timeGeneric_5.ScalarValue, Is.EqualTo(timeFloat64_5.ScalarValue).Within(Tolerance), "Time at length=5");
        Assert.That(timeGeneric_10.ScalarValue, Is.EqualTo(timeFloat64_10.ScalarValue).Within(Tolerance), "Time at length=10");

        Assert.That(timeFloat64_0.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Time at length=0 should be 0");
        Assert.That(timeFloat64_5.ScalarValue, Is.EqualTo(0.5).Within(Tolerance), "Time at length=5 should be 0.5");
        Assert.That(timeFloat64_10.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Time at length=10 should be 1.0");
    }

    [Test]
    public void LineSegmentPath2D_GetFrame_ShouldMatchFloat64()
    {
        // Arrange
        var point1Float64 = LinFloat64Vector2D.Create(0.0, 0.0);
        var point2Float64 = LinFloat64Vector2D.Create(1.0, 0.0);

        var point1Generic = CreateGenericVector(0.0, 0.0);
        var point2Generic = CreateGenericVector(1.0, 0.0);

        var pathFloat64 = Float64LineSegmentPath2D.Create(false, point1Float64, point2Float64);
        var pathGeneric = LineSegmentPath2D<double>.Create(false, point1Generic, point2Generic);

        // Act
        var frameFloat64 = pathFloat64.GetFrame(0.5);
        var frameGeneric = pathGeneric.GetFrame(ScalarProcessor.Scalar(0.5));

        // Assert - Point should match
        Assert.That(frameGeneric.Point.X.ScalarValue, Is.EqualTo(frameFloat64.Point.X.ScalarValue).Within(Tolerance), "Frame Point X");
        Assert.That(frameGeneric.Point.Y.ScalarValue, Is.EqualTo(frameFloat64.Point.Y.ScalarValue).Within(Tolerance), "Frame Point Y");

        // Tangent should match (normalized direction vector)
        Assert.That(frameGeneric.Tangent.X.ScalarValue, Is.EqualTo(frameFloat64.Tangent.X.ScalarValue).Within(Tolerance), "Frame Tangent X");
        Assert.That(frameGeneric.Tangent.Y.ScalarValue, Is.EqualTo(frameFloat64.Tangent.Y.ScalarValue).Within(Tolerance), "Frame Tangent Y");
    }

    [Test]
    public void LineSegmentPath2D_IsValid_ShouldMatchFloat64()
    {
        // Arrange
        var point1Float64 = LinFloat64Vector2D.Create(1.0, 2.0);
        var point2Float64 = LinFloat64Vector2D.Create(4.0, 5.0);

        var point1Generic = CreateGenericVector(1.0, 2.0);
        var point2Generic = CreateGenericVector(4.0, 5.0);

        var pathFloat64 = Float64LineSegmentPath2D.Create(false, point1Float64, point2Float64);
        var pathGeneric = LineSegmentPath2D<double>.Create(false, point1Generic, point2Generic);

        // Act
        var isValidFloat64 = pathFloat64.IsValid();
        var isValidGeneric = pathGeneric.IsValid();

        // Assert
        Assert.That(isValidGeneric, Is.EqualTo(isValidFloat64), "IsValid should match");
        Assert.That(isValidFloat64, Is.True, "Float64 path should be valid");
        Assert.That(isValidGeneric, Is.True, "Generic path should be valid");
    }

    [Test]
    public void LineSegmentPath2D_ToFiniteArcLengthPath_ShouldBeIdempotent()
    {
        // Arrange - Start with non-periodic path
        var point1Generic = CreateGenericVector(1.0, 2.0);
        var point2Generic = CreateGenericVector(4.0, 5.0);

        var pathGeneric = LineSegmentPath2D<double>.Create(false, point1Generic, point2Generic);

        // Act
        var finitePathGeneric = pathGeneric.ToFiniteArcLengthPath();

        // Assert - Should return same instance if already finite
        Assert.That(ReferenceEquals(pathGeneric, finitePathGeneric), Is.True, "Generic: ToFiniteArcLengthPath should return same instance when already finite");
        Assert.That(finitePathGeneric.IsFinite, Is.True, "IsFinite should be true after conversion");
    }

    [Test]
    public void LineSegmentPath2D_ToPeriodicArcLengthPath_ShouldConvertCorrectly()
    {
        // Arrange - Start with finite (non-periodic) path
        var point1Generic = CreateGenericVector(1.0, 2.0);
        var point2Generic = CreateGenericVector(4.0, 5.0);

        var pathGeneric = LineSegmentPath2D<double>.Create(false, point1Generic, point2Generic);

        // Act
        var periodicPathGeneric = pathGeneric.ToPeriodicArcLengthPath();

        // Assert - Should create new periodic instance
        Assert.That(ReferenceEquals(pathGeneric, periodicPathGeneric), Is.False, "Generic: ToPeriodicArcLengthPath should create new instance");
        Assert.That(periodicPathGeneric.IsPeriodic, Is.True, "Generic converted path should be periodic");
        Assert.That(pathGeneric.IsFinite, Is.True, "Original path should remain finite");
    }

    [Test]
    public void LineSegmentPath2D_Direction_ShouldMatchFloat64()
    {
        // Arrange
        var point1Generic = CreateGenericVector(1.0, 2.0);
        var point2Generic = CreateGenericVector(4.0, 6.0);

        var pathGeneric = LineSegmentPath2D<double>.Create(false, point1Generic, point2Generic);

        // Expected direction: Point2 - Point1 = (3, 4)
        var expectedX = 3.0;
        var expectedY = 4.0;

        // Assert
        Assert.That(pathGeneric.Direction.X.ScalarValue, Is.EqualTo(expectedX).Within(Tolerance), "Direction X");
        Assert.That(pathGeneric.Direction.Y.ScalarValue, Is.EqualTo(expectedY).Within(Tolerance), "Direction Y");
    }

    [Test]
    public void LineSegmentPath2D_LinearInterpolation_ShouldBeCorrect()
    {
        // Arrange
        var point1Generic = CreateGenericVector(0.0, 0.0);
        var point2Generic = CreateGenericVector(10.0, 20.0);

        var pathGeneric = LineSegmentPath2D<double>.Create(false, point1Generic, point2Generic);

        // Act
        var value25 = pathGeneric.GetValue(ScalarProcessor.Scalar(0.25));
        var value75 = pathGeneric.GetValue(ScalarProcessor.Scalar(0.75));

        // Assert - Linear interpolation: (1-t)*P1 + t*P2
        // At t=0.25: (0.75)*0 + (0.25)*10 = 2.5
        Assert.That(value25.X.ScalarValue, Is.EqualTo(2.5).Within(Tolerance), "X at t=0.25 should be 2.5");
        Assert.That(value25.Y.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Y at t=0.25 should be 5.0");

        // At t=0.75: (0.25)*0 + (0.75)*10 = 7.5
        Assert.That(value75.X.ScalarValue, Is.EqualTo(7.5).Within(Tolerance), "X at t=0.75 should be 7.5");
        Assert.That(value75.Y.ScalarValue, Is.EqualTo(15.0).Within(Tolerance), "Y at t=0.75 should be 15.0");
    }

    #endregion
}
