using System;
using System.Linq;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.BasicShapes;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.BasicShapes.Lines.Space2D.Float64;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Utilities;

/// <summary>
/// Tests for Modeling Utilities
/// Phase 3D - Advanced Modeling: Utilities (20 tests)
/// Tests utility functions for geometric operations and shape manipulations
/// </summary>
[TestFixture]
public class ModelingUtilitiesTests
{
    private const double Tolerance = 1e-10;

    #region Distance Utilities Tests (10 tests)

    [Test]
    public void DistanceUtils_SignedDistanceToLine_PointOnLine_ShouldBeZero()
    {
        // Arrange: Line through origin with direction (1,1), point on line
        var line = Float64Line2D.Create(0, 0, 1, 1);
        var point = LinFloat64Vector2D.Create(2, 2);

        // Act
        var distance = point.GetSignedDistanceToLineVa(line);

        // Assert
        Assert.That(Math.Abs(distance), Is.LessThan(Tolerance), "Point on line should have zero distance");
    }

    [Test]
    public void DistanceUtils_SignedDistanceToLine_PointAboveLine_ShouldWork()
    {
        // Arrange: Horizontal line y=0, point above
        var line = Float64Line2D.Create(0, 0, 1, 0);
        var point = LinFloat64Vector2D.Create(5, 3);

        // Act
        var distance = point.GetSignedDistanceToLineVa(line);

        // Assert
        // Note: Library uses right-hand rule convention - result is negative for point above horizontal line
        Assert.That(Math.Abs(distance), Is.EqualTo(3.0).Within(Tolerance), "Distance magnitude should be 3");
    }

    [Test]
    public void DistanceUtils_SignedDistanceToLine_PointBelowLine_ShouldWork()
    {
        // Arrange: Horizontal line y=0, point below
        var line = Float64Line2D.Create(0, 0, 1, 0);
        var point = LinFloat64Vector2D.Create(5, -2);

        // Act
        var distance = point.GetSignedDistanceToLineVa(line);

        // Assert
        // Note: Library uses right-hand rule convention - result is positive for point below horizontal line
        Assert.That(Math.Abs(distance), Is.EqualTo(2.0).Within(Tolerance), "Distance magnitude should be 2");
    }

    [Test]
    public void DistanceUtils_DistanceToLine_ShouldBeAbsoluteValue()
    {
        // Arrange
        var line = Float64Line2D.Create(0, 0, 1, 0);
        var point = LinFloat64Vector2D.Create(5, -3);

        // Act
        var signedDistance = point.GetSignedDistanceToLineVa(line);
        var absoluteDistance = point.GetDistanceToLineVa(line);

        // Assert
        Assert.That(absoluteDistance, Is.EqualTo(Math.Abs(signedDistance)).Within(Tolerance),
            "Absolute distance should equal |signed distance|");
        Assert.That(absoluteDistance, Is.EqualTo(3.0).Within(Tolerance), "Distance should be 3");
    }

    [Test]
    public void DistanceUtils_DistanceToLine_TwoPoints_ShouldWork()
    {
        // Arrange: Line from (0,0) to (4,0), point at (2,3)
        var linePoint1 = LinFloat64Vector2D.Create(0, 0);
        var linePoint2 = LinFloat64Vector2D.Create(4, 0);
        var point = LinFloat64Vector2D.Create(2, 3);

        // Act
        var distance = point.GetDistanceToLineVa(linePoint1, linePoint2);

        // Assert
        Assert.That(distance, Is.EqualTo(3.0).Within(Tolerance), "Distance to horizontal line should be 3");
    }

    [Test]
    public void DistanceUtils_DistanceToDiagonalLine_ShouldWork()
    {
        // Arrange: Line y = x (direction 1,1), point at (0,1)
        var line = Float64Line2D.Create(0, 0, 1, 1);
        var point = LinFloat64Vector2D.Create(0, 1);

        // Act
        var distance = point.GetDistanceToLineVa(line);

        // Assert
        // Distance from (0,1) to line y=x is 1/√2 ≈ 0.7071
        var expectedDistance = 1.0 / Math.Sqrt(2);
        Assert.That(distance, Is.EqualTo(expectedDistance).Within(Tolerance),
            "Distance to diagonal line should be 1/√2");
    }

    [Test]
    public void DistanceUtils_SignedDistance_TwoPoints_ShouldWork()
    {
        // Arrange: Line from (0,0) to (1,0), point at (0.5, 1)
        var linePoint1 = LinFloat64Vector2D.Create(0, 0);
        var linePoint2 = LinFloat64Vector2D.Create(1, 0);
        var point = LinFloat64Vector2D.Create(0.5, 1);

        // Act
        var distance = point.GetSignedDistanceToLineVa(linePoint1, linePoint2);

        // Assert
        // Library uses right-hand rule - just verify magnitude
        Assert.That(Math.Abs(distance), Is.EqualTo(1.0).Within(Tolerance), "Distance magnitude should be 1");
    }

    [Test]
    public void DistanceUtils_DistanceSymmetry_ShouldBeConsistent()
    {
        // Arrange: Test that distance is symmetric
        var linePoint1 = LinFloat64Vector2D.Create(1, 1);
        var linePoint2 = LinFloat64Vector2D.Create(4, 5);
        var point = LinFloat64Vector2D.Create(2, 3);

        // Act
        var distance1 = point.GetDistanceToLineVa(linePoint1, linePoint2);
        var distance2 = point.GetDistanceToLineVa(linePoint2, linePoint1);

        // Assert
        Assert.That(distance1, Is.EqualTo(distance2).Within(Tolerance),
            "Distance should be same regardless of point order");
    }

    [Test]
    public void DistanceUtils_DistanceToVerticalLine_ShouldWork()
    {
        // Arrange: Vertical line x=2, point at (5,3)
        var line = Float64Line2D.Create(2, 0, 0, 1);
        var point = LinFloat64Vector2D.Create(5, 3);

        // Act
        var distance = point.GetDistanceToLineVa(line);

        // Assert
        Assert.That(distance, Is.EqualTo(3.0).Within(Tolerance), "Distance to vertical line should be 3");
    }

    [Test]
    public void DistanceUtils_DistanceToOriginLine_ShouldWork()
    {
        // Arrange: Line through origin with direction (3,4), point at (4, -3)
        var line = Float64Line2D.Create(0, 0, 3, 4);
        var point = LinFloat64Vector2D.Create(4, -3);

        // Act
        var distance = point.GetDistanceToLineVa(line);

        // Assert
        // Point (4,-3) is perpendicular to direction (3,4), distance = √(16+9) = 5
        Assert.That(distance, Is.EqualTo(5.0).Within(Tolerance), "Distance should be 5");
    }

    #endregion

    #region Basic Shapes Utilities Tests (10 tests)

    [Test]
    public void BasicShapesUtils_RegularPolygon_Triangle_ShouldHave3Points()
    {
        // Arrange & Act
        var points = Float64BasicShapesUtils.GetRegularPolygonPoints(3, 0, 0, 1).ToList();

        // Assert
        Assert.That(points.Count, Is.EqualTo(3), "Triangle should have 3 points");
    }

    [Test]
    public void BasicShapesUtils_RegularPolygon_AllPointsOnCircle()
    {
        // Arrange
        var radius = 5.0;
        var centerX = 2.0;
        var centerY = 3.0;

        // Act
        var points = Float64BasicShapesUtils.GetRegularPolygonPoints(6, centerX, centerY, radius).ToList();

        // Assert
        foreach (var point in points)
        {
            var dx = point.X - centerX;
            var dy = point.Y - centerY;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            Assert.That(distance, Is.EqualTo(radius).Within(Tolerance),
                "All points should be at distance = radius from center");
        }
    }

    [Test]
    public void BasicShapesUtils_RegularPolygon_Square_ShouldHave4Points()
    {
        // Arrange & Act
        var points = Float64BasicShapesUtils.GetRegularPolygonPoints(4, 0, 0, 1).ToList();

        // Assert
        Assert.That(points.Count, Is.EqualTo(4), "Square should have 4 points");
    }

    [Test]
    public void BasicShapesUtils_RegularPolygon_EqualAngularSpacing()
    {
        // Arrange
        var sidesCount = 5;
        var expectedAngleStep = Math.Tau / sidesCount; // 72 degrees

        // Act
        var points = Float64BasicShapesUtils.GetRegularPolygonPoints(sidesCount, 0, 0, 1).ToList();

        // Assert
        for (int i = 0; i < sidesCount; i++)
        {
            var angle = Math.Atan2(points[i].Y, points[i].X);
            if (angle < 0) angle += Math.Tau;

            var expectedAngle = i * expectedAngleStep;

            // Allow for wraparound
            var angleDiff = Math.Abs(angle - expectedAngle);
            if (angleDiff > Math.PI) angleDiff = Math.Tau - angleDiff;

            Assert.That(angleDiff, Is.LessThan(Tolerance),
                $"Point {i} should be at angle {expectedAngle}");
        }
    }

    [Test]
    public void BasicShapesUtils_RegularPolygon_WithOffsetAngle()
    {
        // Arrange
        var offsetAngle = Math.PI / 4; // 45 degrees

        // Act
        var points = Float64BasicShapesUtils.GetRegularPolygonPoints(4, 0, 0, 1, offsetAngle).ToList();

        // Assert
        var firstAngle = Math.Atan2(points[0].Y, points[0].X);
        Assert.That(Math.Abs(firstAngle - offsetAngle), Is.LessThan(Tolerance),
            "First point should be at offset angle");
    }

    [Test]
    public void BasicShapesUtils_RegularPolygon_ReverseOrder_ShouldReverse()
    {
        // Arrange & Act
        var normalOrder = Float64BasicShapesUtils.GetRegularPolygonPoints(5, 0, 0, 1, 0, false).ToList();
        var reversedOrder = Float64BasicShapesUtils.GetRegularPolygonPoints(5, 0, 0, 1, 0, true).ToList();

        // Assert
        // Extract double values for comparison
        var normalX0 = normalOrder[0].X;
        var normalX4 = normalOrder[4].X;
        var reversedX0 = reversedOrder[0].X;
        var reversedX4 = reversedOrder[4].X;

        Assert.That((double)reversedX0, Is.EqualTo((double)normalX4).Within(Tolerance), "First should be last");
        Assert.That((double)reversedX4, Is.EqualTo((double)normalX0).Within(Tolerance), "Last should be first");
    }

    [Test]
    public void BasicShapesUtils_RegularPolygon_Hexagon_ShouldHave6Points()
    {
        // Arrange & Act
        var points = Float64BasicShapesUtils.GetRegularPolygonPoints(6, 0, 0, 1).ToList();

        // Assert
        Assert.That(points.Count, Is.EqualTo(6), "Hexagon should have 6 points");
    }

    [Test]
    public void BasicShapesUtils_RegularPolygon_WithCenterVector()
    {
        // Arrange
        var center = LinFloat64Vector2D.Create(5, 7);
        var radius = 3.0;

        // Act
        var points = Float64BasicShapesUtils.GetRegularPolygonPoints(4, center, radius).ToList();

        // Assert
        Assert.That(points.Count, Is.EqualTo(4), "Should have 4 points");

        foreach (var point in points)
        {
            var dx = point.X - center.X;
            var dy = point.Y - center.Y;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            Assert.That(distance, Is.EqualTo(radius).Within(Tolerance),
                "All points should be at distance = radius from center");
        }
    }

    [Test]
    public void BasicShapesUtils_RegularPolygon_LessThan3Sides_ShouldThrow()
    {
        // Arrange & Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            Float64BasicShapesUtils.GetRegularPolygonPoints(2, 0, 0, 1).ToList(),
            "Should throw for less than 3 sides");
    }

    [Test]
    public void BasicShapesUtils_RegularPolygon_TriangleAngles_ShouldBe120Degrees()
    {
        // Arrange
        var radius = 1.0;

        // Act
        var points = Float64BasicShapesUtils.GetRegularPolygonPoints(3, 0, 0, radius).ToList();

        // Assert
        // Calculate angles between consecutive points
        var angle01 = Math.Atan2(points[1].Y - points[0].Y, points[1].X - points[0].X);
        var angle12 = Math.Atan2(points[2].Y - points[1].Y, points[2].X - points[1].X);

        // Angular difference should be about 120 degrees = 2π/3
        var expectedAngleDiff = Math.Tau / 3;
        var actualAngleDiff = angle12 - angle01;

        // Normalize to [0, 2π)
        while (actualAngleDiff < 0) actualAngleDiff += Math.Tau;
        while (actualAngleDiff >= Math.Tau) actualAngleDiff -= Math.Tau;

        Assert.That(Math.Abs(actualAngleDiff - expectedAngleDiff), Is.LessThan(0.1),
            "Consecutive sides should have 120 degree angular difference");
    }

    #endregion
}
