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
/// Tests for Graphics Primitives Operations
/// Phase 3C - Extended Modeling: Graphics Primitives Operations (20 tests)
/// Tests operations on visual primitives: Visibility, Validation, Properties
/// </summary>
[TestFixture]
public class GrVisualPrimitivesOperationsTests
{
    private const double Tolerance = 1e-10;

    private class SimpleMaterial : IGrVisualElementMaterial3D
    {
        public string MaterialName { get; }
        public SimpleMaterial(string name) => MaterialName = name;
    }

    #region Visibility Tests (5 tests)

    [Test]
    public void Visibility_DefaultValue_ShouldBeOne()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThickSurfaceStyle(0.1);
        var point = GrVisualPoint3D.CreateStatic("P", style, LinFloat64Vector3D.Zero);

        // Act & Assert
        Assert.That(point.Visibility, Is.EqualTo(1.0).Within(Tolerance), "Default visibility should be 1.0");
    }

    [Test]
    public void Visibility_SetValue_ShouldWork()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThickSurfaceStyle(0.1);
        var point = GrVisualPoint3D.CreateStatic("P", style, LinFloat64Vector3D.Zero);

        // Act
        point.Visibility = 0.5;

        // Assert
        Assert.That(point.Visibility, Is.EqualTo(0.5).Within(Tolerance), "Visibility should be settable");
    }

    [Test]
    public void Visibility_Clamping_ShouldClampToZero()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThickSurfaceStyle(0.1);
        var point = GrVisualPoint3D.CreateStatic("P", style, LinFloat64Vector3D.Zero);

        // Act
        point.Visibility = -0.5;

        // Assert
        Assert.That(point.Visibility, Is.EqualTo(0.0).Within(Tolerance), "Negative visibility should clamp to 0");
    }

    [Test]
    public void Visibility_Clamping_ShouldClampToOne()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThickSurfaceStyle(0.1);
        var point = GrVisualPoint3D.CreateStatic("P", style, LinFloat64Vector3D.Zero);

        // Act
        point.Visibility = 1.5;

        // Assert
        Assert.That(point.Visibility, Is.EqualTo(1.0).Within(Tolerance), "Visibility > 1 should clamp to 1");
    }

    [Test]
    public void Visibility_AllPrimitives_ShouldSupportVisibility()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var pointStyle = material.CreateThickSurfaceStyle(0.1);
        var lineStyle = Color.Red.CreateSolidLineCurveStyle();
        var surfaceStyle = material.CreateThinSurfaceStyle();

        var point = GrVisualPoint3D.CreateStatic("P", pointStyle, LinFloat64Vector3D.Zero);
        var line = GrVisualLineSegment3D.CreateStatic("L", lineStyle, LinFloat64Vector3D.E1, LinFloat64Vector3D.E2);
        var sphere = GrVisualSphereSurface3D.CreateStatic("S", surfaceStyle, 1.0);

        // Act
        point.Visibility = 0.3;
        line.Visibility = 0.6;
        sphere.Visibility = 0.9;

        // Assert
        Assert.That(point.Visibility, Is.EqualTo(0.3).Within(Tolerance), "Point visibility");
        Assert.That(line.Visibility, Is.EqualTo(0.6).Within(Tolerance), "Line visibility");
        Assert.That(sphere.Visibility, Is.EqualTo(0.9).Within(Tolerance), "Sphere visibility");
    }

    #endregion

    #region Validation Tests (5 tests)

    [Test]
    public void IsValid_Point_ShouldBeValid()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThickSurfaceStyle(0.1);
        var point = GrVisualPoint3D.CreateStatic("P", style, LinFloat64Vector3D.Create(1, 2, 3));

        // Act
        var isValid = point.IsValid();

        // Assert
        Assert.That(isValid, Is.True, "Point should be valid");
    }

    [Test]
    public void IsValid_LineSegment_ShouldBeValid()
    {
        // Arrange
        var style = Color.Blue.CreateSolidLineCurveStyle();
        var line = GrVisualLineSegment3D.CreateStatic("L", style, LinFloat64Vector3D.Zero, LinFloat64Vector3D.E1);

        // Act
        var isValid = line.IsValid();

        // Assert
        Assert.That(isValid, Is.True, "LineSegment should be valid");
    }

    [Test]
    public void IsValid_Sphere_ShouldBeValid()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThinSurfaceStyle();
        var sphere = GrVisualSphereSurface3D.CreateStatic("S", style, LinFloat64Vector3D.Zero, 5.0);

        // Act
        var isValid = sphere.IsValid();

        // Assert
        Assert.That(isValid, Is.True, "Sphere should be valid");
    }

    [Test]
    public void IsValid_Circle_ShouldBeValid()
    {
        // Arrange
        var style = Color.Green.CreateSolidLineCurveStyle();
        var circle = GrVisualCircleCurve3D.CreateStatic("C", style, LinFloat64Vector3D.Zero, LinFloat64Vector3D.E3, 2.0);

        // Act
        var isValid = circle.IsValid();

        // Assert
        Assert.That(isValid, Is.True, "Circle should be valid");
    }

    [Test]
    public void IsValid_AllPrimitives_ShouldBeValid()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var point = GrVisualPoint3D.CreateStatic("P", material.CreateThickSurfaceStyle(0.1), LinFloat64Vector3D.E1);
        var line = GrVisualLineSegment3D.CreateStatic("L", Color.Red.CreateSolidLineCurveStyle(), LinFloat64Vector3D.E1, LinFloat64Vector3D.E2);
        var sphere = GrVisualSphereSurface3D.CreateStatic("S", material.CreateThinSurfaceStyle(), 1.0);
        var circle = GrVisualCircleCurve3D.CreateStatic("C", Color.Blue.CreateSolidLineCurveStyle(), LinFloat64Vector3D.E3, 1.0);

        // Act & Assert
        Assert.That(point.IsValid(), Is.True, "Point should be valid");
        Assert.That(line.IsValid(), Is.True, "Line should be valid");
        Assert.That(sphere.IsValid(), Is.True, "Sphere should be valid");
        Assert.That(circle.IsValid(), Is.True, "Circle should be valid");
    }

    #endregion

    #region Name Property Tests (4 tests)

    [Test]
    public void Name_Point_ShouldBeAccessible()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThickSurfaceStyle(0.1);
        var point = GrVisualPoint3D.CreateStatic("TestPoint", style, LinFloat64Vector3D.Zero);

        // Act & Assert
        Assert.That(point.Name, Is.EqualTo("TestPoint"), "Point name should match");
    }

    [Test]
    public void Name_Line_ShouldBeAccessible()
    {
        // Arrange
        var style = Color.Red.CreateSolidLineCurveStyle();
        var line = GrVisualLineSegment3D.CreateStatic("TestLine", style, LinFloat64Vector3D.E1, LinFloat64Vector3D.E2);

        // Act & Assert
        Assert.That(line.Name, Is.EqualTo("TestLine"), "Line name should match");
    }

    [Test]
    public void Name_Sphere_ShouldBeAccessible()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThinSurfaceStyle();
        var sphere = GrVisualSphereSurface3D.CreateStatic("TestSphere", style, 1.0);

        // Act & Assert
        Assert.That(sphere.Name, Is.EqualTo("TestSphere"), "Sphere name should match");
    }

    [Test]
    public void Name_UniqueName_ForEachPrimitive()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var point = GrVisualPoint3D.CreateStatic("P1", material.CreateThickSurfaceStyle(0.1), LinFloat64Vector3D.Zero);
        var line = GrVisualLineSegment3D.CreateStatic("L1", Color.Red.CreateSolidLineCurveStyle(), LinFloat64Vector3D.E1, LinFloat64Vector3D.E2);
        var sphere = GrVisualSphereSurface3D.CreateStatic("S1", material.CreateThinSurfaceStyle(), 1.0);

        // Act & Assert
        Assert.That(point.Name, Is.Not.EqualTo(line.Name), "Point and line should have different names");
        Assert.That(line.Name, Is.Not.EqualTo(sphere.Name), "Line and sphere should have different names");
        Assert.That(point.Name, Is.Not.EqualTo(sphere.Name), "Point and sphere should have different names");
    }

    #endregion

    #region Style Property Tests (3 tests)

    [Test]
    public void Style_Point_ShouldBeAccessible()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThickSurfaceStyle(0.5);
        var point = GrVisualPoint3D.CreateStatic("P", style, LinFloat64Vector3D.Zero);

        // Act & Assert
        Assert.That(point.Style, Is.Not.Null, "Style should be accessible");
        Assert.That(point.Style.Thickness, Is.EqualTo(0.5).Within(Tolerance), "Thickness should match");
    }

    [Test]
    public void Style_Line_ShouldBeAccessible()
    {
        // Arrange
        var style = Color.Red.CreateSolidLineCurveStyle();
        var line = GrVisualLineSegment3D.CreateStatic("L", style, LinFloat64Vector3D.E1, LinFloat64Vector3D.E2);

        // Act & Assert
        Assert.That(line.Style, Is.Not.Null, "Style should be accessible");
        Assert.That(line.Style, Is.SameAs(style), "Style reference should match");
    }

    [Test]
    public void Style_Sphere_ShouldBeAccessible()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThinSurfaceStyle();
        var sphere = GrVisualSphereSurface3D.CreateStatic("S", style, 1.0);

        // Act & Assert
        Assert.That(sphere.Style, Is.Not.Null, "Style should be accessible");
        Assert.That(sphere.Style, Is.SameAs(style), "Style reference should match");
    }

    #endregion

    #region Geometry Property Tests (3 tests)

    [Test]
    public void Geometry_Point_PositionAccessible()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThickSurfaceStyle(0.1);
        var position = LinFloat64Vector3D.Create(1, 2, 3);
        var point = GrVisualPoint3D.CreateStatic("P", style, position);

        // Act & Assert
        Assert.That(point.X.ScalarValue, Is.EqualTo(1).Within(Tolerance), "X should be accessible");
        Assert.That(point.Y.ScalarValue, Is.EqualTo(2).Within(Tolerance), "Y should be accessible");
        Assert.That(point.Z.ScalarValue, Is.EqualTo(3).Within(Tolerance), "Z should be accessible");
    }

    [Test]
    public void Geometry_Line_EndpointsAccessible()
    {
        // Arrange
        var style = Color.Blue.CreateSolidLineCurveStyle();
        var pos1 = LinFloat64Vector3D.Create(1, 0, 0);
        var pos2 = LinFloat64Vector3D.Create(0, 1, 0);
        var line = GrVisualLineSegment3D.CreateStatic("L", style, pos1, pos2);

        // Act & Assert
        Assert.That(line.Position1.X.ScalarValue, Is.EqualTo(1).Within(Tolerance), "Position1.X accessible");
        Assert.That(line.Position2.Y.ScalarValue, Is.EqualTo(1).Within(Tolerance), "Position2.Y accessible");
    }

    [Test]
    public void Geometry_Sphere_CenterRadiusAccessible()
    {
        // Arrange
        var material = new SimpleMaterial("Mat");
        var style = material.CreateThinSurfaceStyle();
        var center = LinFloat64Vector3D.Create(5, 6, 7);
        var radius = 3.5;
        var sphere = GrVisualSphereSurface3D.CreateStatic("S", style, center, radius);

        // Act & Assert
        Assert.That(sphere.Center.X.ScalarValue, Is.EqualTo(5).Within(Tolerance), "Center.X accessible");
        Assert.That(sphere.Center.Y.ScalarValue, Is.EqualTo(6).Within(Tolerance), "Center.Y accessible");
        Assert.That(sphere.Center.Z.ScalarValue, Is.EqualTo(7).Within(Tolerance), "Center.Z accessible");
        Assert.That(sphere.Radius, Is.EqualTo(3.5).Within(Tolerance), "Radius accessible");
    }

    #endregion
}
