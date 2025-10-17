using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Rendering.Visuals.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Rendering.Visuals.Space3D.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Rendering.Visuals.Space3D.Curves;
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Rendering.Visuals.Space3D.Surfaces;
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Rendering.Visuals.Space3D.Styles;
using NUnit.Framework;
using SixLabors.ImageSharp;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Graphics.Primitives;

/// <summary>
/// Tests for Graphics Primitives Construction
/// Phase 3C - Extended Modeling: Graphics Primitives Construction (20 tests)
/// Tests creation of visual primitives: Points, Lines, Spheres, Circles, Surfaces
/// </summary>
[TestFixture]
public class GrVisualPrimitivesConstructionTests
{
    private const double Tolerance = 1e-10;

    // Helper to create a simple material mock
    private class SimpleMaterial : IGrVisualElementMaterial3D
    {
        public string MaterialName { get; }
        public SimpleMaterial(string name) => MaterialName = name;
    }

    #region Point Construction Tests (4 tests)

    [Test]
    public void Point_CreateStatic_ShouldWork()
    {
        // Arrange
        var name = "TestPoint";
        var material = new SimpleMaterial("TestMaterial");
        var style = material.CreateThickSurfaceStyle(0.1);
        var position = LinFloat64Vector3D.Create(1, 2, 3);

        // Act
        var point = GrVisualPoint3D.CreateStatic(name, style, position);

        // Assert
        Assert.That(point, Is.Not.Null, "Point should be created");
        Assert.That(point.Name, Is.EqualTo(name), "Name should match");
        Assert.That(point.X.ScalarValue, Is.EqualTo(1).Within(Tolerance), "X should match");
        Assert.That(point.Y.ScalarValue, Is.EqualTo(2).Within(Tolerance), "Y should match");
        Assert.That(point.Z.ScalarValue, Is.EqualTo(3).Within(Tolerance), "Z should match");
    }

    [Test]
    public void Point_CreateAtOrigin_ShouldWork()
    {
        // Arrange
        var name = "OriginPoint";
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThickSurfaceStyle(0.1);
        var position = LinFloat64Vector3D.Zero;

        // Act
        var point = GrVisualPoint3D.CreateStatic(name, style, position);

        // Assert
        Assert.That(point, Is.Not.Null, "Point should be created");
        Assert.That(point.X.ScalarValue, Is.EqualTo(0).Within(Tolerance), "X should be 0");
        Assert.That(point.Y.ScalarValue, Is.EqualTo(0).Within(Tolerance), "Y should be 0");
        Assert.That(point.Z.ScalarValue, Is.EqualTo(0).Within(Tolerance), "Z should be 0");
    }

    [Test]
    public void Point_Style_ShouldBeAccessible()
    {
        // Arrange
        var name = "StyledPoint";
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThickSurfaceStyle(0.5);
        var position = LinFloat64Vector3D.E1;

        // Act
        var point = GrVisualPoint3D.CreateStatic(name, style, position);

        // Assert
        Assert.That(point.Style, Is.Not.Null, "Style should be accessible");
        Assert.That(point.Style.Thickness, Is.EqualTo(0.5).Within(Tolerance), "Thickness should match");
    }

    [Test]
    public void Point_MultiplePoints_ShouldHaveIndependentState()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThickSurfaceStyle(0.1);
        var point1 = GrVisualPoint3D.CreateStatic("Point1", style, LinFloat64Vector3D.Create(1, 0, 0));
        var point2 = GrVisualPoint3D.CreateStatic("Point2", style, LinFloat64Vector3D.Create(0, 1, 0));

        // Act & Assert
        Assert.That(point1.X.ScalarValue, Is.EqualTo(1).Within(Tolerance), "Point1.X should be 1");
        Assert.That(point2.Y.ScalarValue, Is.EqualTo(1).Within(Tolerance), "Point2.Y should be 1");
        Assert.That(point1.Name, Is.Not.EqualTo(point2.Name), "Names should be different");
    }

    #endregion

    #region LineSegment Construction Tests (4 tests)

    [Test]
    public void LineSegment_CreateStatic_ShouldWork()
    {
        // Arrange
        var name = "TestLine";
        var style = Color.Red.CreateSolidLineCurveStyle();
        var position1 = LinFloat64Vector3D.Create(0, 0, 0);
        var position2 = LinFloat64Vector3D.Create(1, 1, 1);

        // Act
        var line = GrVisualLineSegment3D.CreateStatic(name, style, position1, position2);

        // Assert
        Assert.That(line, Is.Not.Null, "LineSegment should be created");
        Assert.That(line.Name, Is.EqualTo(name), "Name should match");
        Assert.That(line.Position1.X.ScalarValue, Is.EqualTo(0).Within(Tolerance), "Position1.X should match");
        Assert.That(line.Position2.X.ScalarValue, Is.EqualTo(1).Within(Tolerance), "Position2.X should match");
    }

    [Test]
    public void LineSegment_FromOrigin_ShouldWork()
    {
        // Arrange
        var name = "OriginLine";
        var style = Color.Blue.CreateSolidLineCurveStyle();
        var position2 = LinFloat64Vector3D.Create(5, 0, 0);

        // Act
        var line = GrVisualLineSegment3D.CreateStatic(name, style, position2);

        // Assert
        Assert.That(line, Is.Not.Null, "LineSegment should be created");
        Assert.That(line.Position1.X.ScalarValue, Is.EqualTo(0).Within(Tolerance), "Should start at origin");
        Assert.That(line.Position2.X.ScalarValue, Is.EqualTo(5).Within(Tolerance), "Should end at (5,0,0)");
    }

    [Test]
    public void LineSegment_Style_ShouldBeAccessible()
    {
        // Arrange
        var name = "StyledLine";
        var style = Color.Green.CreateSolidLineCurveStyle();
        var line = GrVisualLineSegment3D.CreateStatic(name, style, LinFloat64Vector3D.E1, LinFloat64Vector3D.E2);

        // Act & Assert
        Assert.That(line.Style, Is.Not.Null, "Style should be accessible");
    }

    [Test]
    public void LineSegment_DifferentEndpoints_ShouldWork()
    {
        // Arrange
        var style = Color.Yellow.CreateSolidLineCurveStyle();
        var line1 = GrVisualLineSegment3D.CreateStatic("Line1", style, LinFloat64Vector3D.E1, LinFloat64Vector3D.E2);
        var line2 = GrVisualLineSegment3D.CreateStatic("Line2", style, LinFloat64Vector3D.E2, LinFloat64Vector3D.E3);

        // Act & Assert
        Assert.That(line1.Position2.Y.ScalarValue, Is.EqualTo(1).Within(Tolerance), "Line1 ends at E2");
        Assert.That(line2.Position2.Z.ScalarValue, Is.EqualTo(1).Within(Tolerance), "Line2 ends at E3");
    }

    #endregion

    #region Sphere Construction Tests (4 tests)

    [Test]
    public void Sphere_CreateStatic_ShouldWork()
    {
        // Arrange
        var name = "TestSphere";
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThinSurfaceStyle();
        var center = LinFloat64Vector3D.Create(1, 2, 3);
        var radius = 5.0;

        // Act
        var sphere = GrVisualSphereSurface3D.CreateStatic(name, style, center, radius);

        // Assert
        Assert.That(sphere, Is.Not.Null, "Sphere should be created");
        Assert.That(sphere.Name, Is.EqualTo(name), "Name should match");
        Assert.That(sphere.Center.X.ScalarValue, Is.EqualTo(1).Within(Tolerance), "Center.X should match");
        Assert.That(sphere.Radius, Is.EqualTo(5).Within(Tolerance), "Radius should match");
    }

    [Test]
    public void Sphere_AtOrigin_ShouldWork()
    {
        // Arrange
        var name = "OriginSphere";
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThinSurfaceStyle();
        var radius = 2.5;

        // Act
        var sphere = GrVisualSphereSurface3D.CreateStatic(name, style, radius);

        // Assert
        Assert.That(sphere, Is.Not.Null, "Sphere should be created");
        Assert.That(sphere.Center.X.ScalarValue, Is.EqualTo(0).Within(Tolerance), "Should be at origin");
        Assert.That(sphere.Radius, Is.EqualTo(2.5).Within(Tolerance), "Radius should match");
    }

    [Test]
    public void Sphere_Style_ShouldBeAccessible()
    {
        // Arrange
        var name = "StyledSphere";
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThinSurfaceStyle();
        var sphere = GrVisualSphereSurface3D.CreateStatic(name, style, 1.0);

        // Act & Assert
        Assert.That(sphere.Style, Is.Not.Null, "Style should be accessible");
    }

    [Test]
    public void Sphere_DifferentRadii_ShouldWork()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThinSurfaceStyle();
        var sphere1 = GrVisualSphereSurface3D.CreateStatic("Small", style, LinFloat64Vector3D.Zero, 1.0);
        var sphere2 = GrVisualSphereSurface3D.CreateStatic("Large", style, LinFloat64Vector3D.Zero, 10.0);

        // Act & Assert
        Assert.That(sphere1.Radius, Is.EqualTo(1.0).Within(Tolerance), "Small sphere radius");
        Assert.That(sphere2.Radius, Is.EqualTo(10.0).Within(Tolerance), "Large sphere radius");
    }

    #endregion

    #region Circle Construction Tests (4 tests)

    [Test]
    public void Circle_CreateStatic_ShouldWork()
    {
        // Arrange
        var name = "TestCircle";
        var style = Color.Red.CreateSolidLineCurveStyle();
        var center = LinFloat64Vector3D.Create(1, 2, 3);
        var normal = LinFloat64Vector3D.E3;
        var radius = 4.0;

        // Act
        var circle = GrVisualCircleCurve3D.CreateStatic(name, style, center, normal, radius);

        // Assert
        Assert.That(circle, Is.Not.Null, "Circle should be created");
        Assert.That(circle.Name, Is.EqualTo(name), "Name should match");
        Assert.That(circle.Center.Z.ScalarValue, Is.EqualTo(3).Within(Tolerance), "Center.Z should match");
        Assert.That(circle.Radius, Is.EqualTo(4).Within(Tolerance), "Radius should match");
    }

    [Test]
    public void Circle_AtOrigin_ShouldWork()
    {
        // Arrange
        var name = "OriginCircle";
        var style = Color.Blue.CreateSolidLineCurveStyle();
        var normal = LinFloat64Vector3D.E2;
        var radius = 3.0;

        // Act
        var circle = GrVisualCircleCurve3D.CreateStatic(name, style, normal, radius);

        // Assert
        Assert.That(circle, Is.Not.Null, "Circle should be created");
        Assert.That(circle.Center.X.ScalarValue, Is.EqualTo(0).Within(Tolerance), "Should be at origin");
        Assert.That(circle.Radius, Is.EqualTo(3).Within(Tolerance), "Radius should match");
    }

    [Test]
    public void Circle_Normal_ShouldBeAccessible()
    {
        // Arrange
        var name = "NormalCircle";
        var style = Color.Green.CreateSolidLineCurveStyle();
        var normal = LinFloat64Vector3D.E1;
        var circle = GrVisualCircleCurve3D.CreateStatic(name, style, normal, 1.0);

        // Act & Assert
        Assert.That(circle.Normal, Is.Not.Null, "Normal should be accessible");
        Assert.That(circle.Normal.X.ScalarValue, Is.EqualTo(1).Within(Tolerance), "Normal should be E1");
    }

    [Test]
    public void Circle_DifferentNormals_ShouldWork()
    {
        // Arrange
        var style = Color.Yellow.CreateSolidLineCurveStyle();
        var circleXY = GrVisualCircleCurve3D.CreateStatic("XY", style, LinFloat64Vector3D.E3, 1.0);
        var circleXZ = GrVisualCircleCurve3D.CreateStatic("XZ", style, LinFloat64Vector3D.E2, 1.0);

        // Act & Assert
        Assert.That(circleXY.Normal.Z.ScalarValue, Is.EqualTo(1).Within(Tolerance), "XY plane normal");
        Assert.That(circleXZ.Normal.Y.ScalarValue, Is.EqualTo(1).Within(Tolerance), "XZ plane normal");
    }

    #endregion

    #region Surface Construction Tests (4 tests)

    [Test]
    public void Surface_CircleSurface_ShouldCreate()
    {
        // Arrange
        var name = "CircleSurface";
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThinSurfaceStyle();
        var center = LinFloat64Vector3D.Zero;
        var normal = LinFloat64Vector3D.E3;
        var radius = 2.0;

        // Act
        var surface = GrVisualCircleSurface3D.CreateStatic(name, style, center, normal, radius, false);

        // Assert
        Assert.That(surface, Is.Not.Null, "Circle surface should be created");
        Assert.That(surface.Name, Is.EqualTo(name), "Name should match");
    }

    [Test]
    public void Surface_TriangleSurface_ShouldCreate()
    {
        // Arrange
        var name = "Triangle";
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThinSurfaceStyle();
        var p1 = LinFloat64Vector3D.Create(0, 0, 0);
        var p2 = LinFloat64Vector3D.Create(1, 0, 0);
        var p3 = LinFloat64Vector3D.Create(0, 1, 0);

        // Act
        var surface = GrVisualTriangleSurface3D.CreateStatic(name, style, p1, p2, p3);

        // Assert
        Assert.That(surface, Is.Not.Null, "Triangle surface should be created");
        Assert.That(surface.Name, Is.EqualTo(name), "Name should match");
    }

    [Test]
    public void Surface_ParallelogramSurface_ShouldCreate()
    {
        // Arrange
        var name = "Parallelogram";
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThinSurfaceStyle();
        var position = LinFloat64Vector3D.Zero;
        var direction1 = LinFloat64Vector3D.E1;
        var direction2 = LinFloat64Vector3D.E2;

        // Act
        var surface = GrVisualParallelogramSurface3D.CreateStatic(name, style, position, direction1, direction2);

        // Assert
        Assert.That(surface, Is.Not.Null, "Parallelogram surface should be created");
        Assert.That(surface.Name, Is.EqualTo(name), "Name should match");
    }

    [Test]
    public void Surface_AllTypes_ShouldSupportCommonAPI()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThinSurfaceStyle();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var circle = GrVisualCircleSurface3D.CreateStatic("C", style, LinFloat64Vector3D.Zero, LinFloat64Vector3D.E3, 1.0, false);
            var triangle = GrVisualTriangleSurface3D.CreateStatic("T", style, LinFloat64Vector3D.E1, LinFloat64Vector3D.E2, LinFloat64Vector3D.E3);
            var parallelogram = GrVisualParallelogramSurface3D.CreateStatic("P", style, LinFloat64Vector3D.Zero, LinFloat64Vector3D.E1, LinFloat64Vector3D.E2);

            Assert.That(circle.Style, Is.Not.Null);
            Assert.That(triangle.Style, Is.Not.Null);
            Assert.That(parallelogram.Style, Is.Not.Null);
        }, "All surface types should support common API");
    }

    #endregion
}
