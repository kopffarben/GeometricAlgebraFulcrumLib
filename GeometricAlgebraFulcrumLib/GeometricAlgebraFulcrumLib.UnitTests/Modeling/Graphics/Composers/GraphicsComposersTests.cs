using System.Linq;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Composers;
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Primitives;
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Primitives.Lines;
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Primitives.Triangles;
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Primitives.Vertices;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Collections.PeriodicLists2D;
using NUnit.Framework;
using SixLabors.ImageSharp;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Graphics.Composers;

/// <summary>
/// Tests for Graphics Composers
/// Phase 3C - Extended Modeling: Graphics Export/Composers (25 tests)
/// Tests geometry composition for graphics export: Lines, Triangles, Grids
/// </summary>
[TestFixture]
public class GraphicsComposersTests
{
    private const double Tolerance = 1e-10;

    #region LineGeometryComposer Tests (6 tests)

    [Test]
    public void LineComposer_Construction_ShouldWork()
    {
        // Arrange & Act
        var composer = new GrLineGeometryComposer3D();

        // Assert
        Assert.That(composer, Is.Not.Null, "Composer should be created");
        Assert.That(composer.Count, Is.EqualTo(0), "Should start with 0 lines");
        Assert.That(composer.VertexCount, Is.EqualTo(0), "Should start with 0 vertices");
    }

    [Test]
    public void LineComposer_AddLine_ShouldIncreaseCount()
    {
        // Arrange
        var composer = new GrLineGeometryComposer3D();
        var p1 = LinFloat64Vector3D.Create(0, 0, 0);
        var p2 = LinFloat64Vector3D.Create(1, 0, 0);

        // Act
        composer.AddLine(p1, p2);

        // Assert
        Assert.That(composer.Count, Is.EqualTo(1), "Should have 1 line");
        Assert.That(composer.VertexCount, Is.GreaterThan(0), "Should have vertices");
    }

    [Test]
    public void LineComposer_PrimitiveType_ShouldBeLineList()
    {
        // Arrange & Act
        var composer = new GrLineGeometryComposer3D();

        // Assert
        Assert.That(composer.PrimitiveType, Is.EqualTo(GraphicsPrimitiveType3D.LineList), "Should be LineList type");
    }

    [Test]
    public void LineComposer_GeometryIndices_ShouldBeAccessible()
    {
        // Arrange
        var composer = new GrLineGeometryComposer3D();
        composer.AddLine(LinFloat64Vector3D.E1, LinFloat64Vector3D.E2);

        // Act
        var indices = composer.GeometryIndices.ToList();

        // Assert
        Assert.That(indices, Is.Not.Null, "Indices should be accessible");
        Assert.That(indices.Count, Is.GreaterThan(0), "Should have indices");
    }

    [Test]
    public void LineComposer_GeometryVertices_ShouldBeAccessible()
    {
        // Arrange
        var composer = new GrLineGeometryComposer3D();
        composer.AddLine(LinFloat64Vector3D.E1, LinFloat64Vector3D.E2);

        // Act
        var vertices = composer.GeometryVertices.ToList();

        // Assert
        Assert.That(vertices, Is.Not.Null, "Vertices should be accessible");
        Assert.That(vertices.Count, Is.GreaterThan(0), "Should have vertices");
    }

    [Test]
    public void LineComposer_DistanceEpsilon_ShouldBeConfigurable()
    {
        // Arrange
        var composer = new GrLineGeometryComposer3D();

        // Act
        composer.DistanceEpsilon = 1e-5;

        // Assert
        Assert.That(composer.DistanceEpsilon, Is.EqualTo(1e-5).Within(Tolerance), "Distance epsilon should be configurable");
    }

    #endregion

    #region TriangleGeometryComposer Tests (7 tests)

    [Test]
    public void TriangleComposer_Construction_ShouldWork()
    {
        // Arrange & Act
        var composer = new GrTriangleGeometryComposer3D();

        // Assert
        Assert.That(composer, Is.Not.Null, "Composer should be created");
        Assert.That(composer.Count, Is.EqualTo(0), "Should start with 0 triangles");
        Assert.That(composer.VertexCount, Is.EqualTo(0), "Should start with 0 vertices");
    }

    [Test]
    public void TriangleComposer_AddTriangle_ShouldIncreaseCount()
    {
        // Arrange
        var composer = new GrTriangleGeometryComposer3D();
        var p1 = LinFloat64Vector3D.Create(0, 0, 0);
        var p2 = LinFloat64Vector3D.Create(1, 0, 0);
        var p3 = LinFloat64Vector3D.Create(0, 1, 0);

        // Act
        composer.AddTriangle(p1, p2, p3);

        // Assert
        Assert.That(composer.Count, Is.EqualTo(1), "Should have 1 triangle");
        Assert.That(composer.VertexCount, Is.GreaterThan(0), "Should have vertices");
    }

    [Test]
    public void TriangleComposer_PrimitiveType_ShouldBeTriangleList()
    {
        // Arrange & Act
        var composer = new GrTriangleGeometryComposer3D();

        // Assert
        Assert.That(composer.PrimitiveType, Is.EqualTo(GraphicsPrimitiveType3D.TriangleList), "Should be TriangleList type");
    }

    [Test]
    public void TriangleComposer_VertexNormalsEnabled_ShouldBeConfigurable()
    {
        // Arrange
        var composer = new GrTriangleGeometryComposer3D();

        // Act
        composer.VertexNormalsEnabled = true;

        // Assert
        Assert.That(composer.VertexNormalsEnabled, Is.True, "Vertex normals should be enabled");
    }

    [Test]
    public void TriangleComposer_VertexColorsEnabled_ShouldBeConfigurable()
    {
        // Arrange
        var composer = new GrTriangleGeometryComposer3D();

        // Act
        composer.VertexColorsEnabled = true;

        // Assert
        Assert.That(composer.VertexColorsEnabled, Is.True, "Vertex colors should be enabled");
    }

    [Test]
    public void TriangleComposer_VertexTextureUVsEnabled_ShouldBeConfigurable()
    {
        // Arrange
        var composer = new GrTriangleGeometryComposer3D();

        // Act
        composer.VertexTextureUVsEnabled = true;

        // Assert
        Assert.That(composer.VertexTextureUVsEnabled, Is.True, "Vertex texture UVs should be enabled");
    }

    [Test]
    public void TriangleComposer_ReverseNormals_ShouldBeConfigurable()
    {
        // Arrange
        var composer = new GrTriangleGeometryComposer3D();

        // Act
        composer.ReverseNormals = true;

        // Assert
        Assert.That(composer.ReverseNormals, Is.True, "Reverse normals should be enabled");
    }

    #endregion

    #region GridMeshComposer Tests (4 tests)

    [Test]
    public void GridMeshComposer_Construction_ShouldWork()
    {
        // Arrange
        var positions = CreateSimpleGrid(3, 3);
        var colors = CreateSimpleGrid(3, 3);

        // Act
        var composer = new GridMeshComposer(positions, colors);

        // Assert
        Assert.That(composer, Is.Not.Null, "Composer should be created");
        Assert.That(composer.VerticesCount, Is.EqualTo(9), "Should have 3x3=9 vertices");
    }

    [Test]
    public void GridMeshComposer_Count1Count2_ShouldMatchInput()
    {
        // Arrange
        var positions = CreateSimpleGrid(4, 5);
        var colors = CreateSimpleGrid(4, 5);

        // Act
        var composer = new GridMeshComposer(positions, colors);

        // Assert
        Assert.That(composer.Count1, Is.EqualTo(4), "Count1 should match");
        Assert.That(composer.Count2, Is.EqualTo(5), "Count2 should match");
    }

    [Test]
    public void GridMeshComposer_VertexPositions_ShouldBeAccessible()
    {
        // Arrange
        var positions = CreateSimpleGrid(3, 3);
        var colors = CreateSimpleGrid(3, 3);
        var composer = new GridMeshComposer(positions, colors);

        // Act
        var vertexPositions = composer.VertexPositions.ToList();

        // Assert
        Assert.That(vertexPositions, Is.Not.Null, "Vertex positions should be accessible");
        Assert.That(vertexPositions.Count, Is.EqualTo(9), "Should have 9 positions");
    }

    [Test]
    public void GridMeshComposer_VertexNormals_ShouldBeAccessible()
    {
        // Arrange
        var positions = CreateSimpleGrid(3, 3);
        var colors = CreateSimpleGrid(3, 3);
        var composer = new GridMeshComposer(positions, colors);

        // Act
        var normals = composer.VertexNormals.ToList();

        // Assert
        Assert.That(normals, Is.Not.Null, "Vertex normals should be accessible");
        Assert.That(normals.Count, Is.EqualTo(9), "Should have 9 normals");
    }

    #endregion

    #region XyGridComposer Tests (4 tests)

    [Test]
    public void XyGridComposer_Construction_ShouldWork()
    {
        // Arrange & Act
        var composer = new XyGridComposer();

        // Assert
        Assert.That(composer, Is.Not.Null, "Composer should be created");
        Assert.That(composer.XUnitSize, Is.EqualTo(1.0).Within(Tolerance), "Default X unit size should be 1.0");
        Assert.That(composer.YUnitSize, Is.EqualTo(1.0).Within(Tolerance), "Default Y unit size should be 1.0");
    }

    [Test]
    public void XyGridComposer_UnitSizes_ShouldBeConfigurable()
    {
        // Arrange
        var composer = new XyGridComposer();

        // Act
        composer.XUnitSize = 2.5;
        composer.YUnitSize = 3.5;

        // Assert
        Assert.That(composer.XUnitSize, Is.EqualTo(2.5).Within(Tolerance), "X unit size should be configurable");
        Assert.That(composer.YUnitSize, Is.EqualTo(3.5).Within(Tolerance), "Y unit size should be configurable");
    }

    [Test]
    public void XyGridComposer_UnitCounts_ShouldBeConfigurable()
    {
        // Arrange
        var composer = new XyGridComposer();

        // Act
        composer.XUnitsCount = 15;
        composer.YUnitsCount = 20;

        // Assert
        Assert.That(composer.XUnitsCount, Is.EqualTo(15), "X units count should be configurable");
        Assert.That(composer.YUnitsCount, Is.EqualTo(20), "Y units count should be configurable");
        Assert.That(composer.XUnitTotalCount, Is.EqualTo(30), "Total X units should be 2 * XUnitsCount");
        Assert.That(composer.YUnitTotalCount, Is.EqualTo(40), "Total Y units should be 2 * YUnitsCount");
    }

    [Test]
    public void XyGridComposer_MinMaxCoordinates_ShouldBeCorrect()
    {
        // Arrange
        var composer = new XyGridComposer
        {
            XUnitSize = 2.0,
            YUnitSize = 3.0,
            XUnitsCount = 5,
            YUnitsCount = 4
        };

        // Act
        var xMin = composer.XMin;
        var xMax = composer.XMax;

        // Assert
        Assert.That(xMin, Is.EqualTo(-10.0).Within(Tolerance), "XMin should be Center.X - XUnitSize * XUnitsCount");
        Assert.That(xMax, Is.EqualTo(10.0).Within(Tolerance), "XMax should be Center.X + XUnitSize * XUnitsCount");
    }

    #endregion

    #region YzGridComposer Tests (2 tests)

    [Test]
    public void YzGridComposer_Construction_ShouldWork()
    {
        // Arrange & Act
        var composer = new YzGridComposer();

        // Assert
        Assert.That(composer, Is.Not.Null, "YZ Grid composer should be created");
        Assert.That(composer.YUnitSize, Is.EqualTo(1.0).Within(Tolerance), "Default Y unit size should be 1.0");
        Assert.That(composer.ZUnitSize, Is.EqualTo(1.0).Within(Tolerance), "Default Z unit size should be 1.0");
    }

    [Test]
    public void YzGridComposer_UnitCounts_ShouldBeConfigurable()
    {
        // Arrange
        var composer = new YzGridComposer();

        // Act
        composer.YUnitsCount = 8;
        composer.ZUnitsCount = 12;

        // Assert
        Assert.That(composer.YUnitsCount, Is.EqualTo(8), "Y units count should be configurable");
        Assert.That(composer.ZUnitsCount, Is.EqualTo(12), "Z units count should be configurable");
    }

    #endregion

    #region ZxGridComposer Tests (2 tests)

    [Test]
    public void ZxGridComposer_Construction_ShouldWork()
    {
        // Arrange & Act
        var composer = new ZxGridComposer();

        // Assert
        Assert.That(composer, Is.Not.Null, "ZX Grid composer should be created");
        Assert.That(composer.ZUnitSize, Is.EqualTo(1.0).Within(Tolerance), "Default Z unit size should be 1.0");
        Assert.That(composer.XUnitSize, Is.EqualTo(1.0).Within(Tolerance), "Default X unit size should be 1.0");
    }

    [Test]
    public void ZxGridComposer_UnitCounts_ShouldBeConfigurable()
    {
        // Arrange
        var composer = new ZxGridComposer();

        // Act
        composer.ZUnitsCount = 6;
        composer.XUnitsCount = 10;

        // Assert
        Assert.That(composer.ZUnitsCount, Is.EqualTo(6), "Z units count should be configurable");
        Assert.That(composer.XUnitsCount, Is.EqualTo(10), "X units count should be configurable");
    }

    #endregion

    #region Helper Methods

    private static IPeriodicReadOnlyList2D<ILinFloat64Vector3D> CreateSimpleGrid(int count1, int count2)
    {
        // Use ProListConstantValues2D for simplicity in tests
        return new ProListConstantValues2D<ILinFloat64Vector3D>(
            count1,
            count2,
            LinFloat64Vector3D.Create(0, 0, 0)
        );
    }

    #endregion
}
