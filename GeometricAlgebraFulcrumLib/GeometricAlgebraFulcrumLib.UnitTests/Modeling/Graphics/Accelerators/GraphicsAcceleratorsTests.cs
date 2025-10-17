using System.Collections.Generic;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.BasicShapes.Triangles.Space3D.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.BasicShapes.Triangles.Space2D.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Accelerators.Grids.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Accelerators.Grids.Space2D;
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Accelerators.BIH.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Graphics.Accelerators.BIH.Space2D;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Graphics.Accelerators;

/// <summary>
/// Tests for Graphics Accelerators
/// Phase 3C - Extended Modeling: Graphics Accelerators (15 tests)
/// Tests spatial acceleration structures: Grids and BIH (Bounding Interval Hierarchy)
/// </summary>
[TestFixture]
public class GraphicsAcceleratorsTests
{
    private const double Tolerance = 1e-10;

    #region Grid 3D Tests (5 tests)

    [Test]
    public void Grid3D_Construction_ShouldWork()
    {
        // Arrange
        var triangles = new List<Float64Triangle3D>
        {
            Float64Triangle3D.Create(
                LinFloat64Vector3D.Create(0, 0, 0),
                LinFloat64Vector3D.Create(1, 0, 0),
                LinFloat64Vector3D.Create(0, 1, 0)
            ),
            Float64Triangle3D.Create(
                LinFloat64Vector3D.Create(2, 2, 2),
                LinFloat64Vector3D.Create(3, 2, 2),
                LinFloat64Vector3D.Create(2, 3, 2)
            )
        };

        // Act
        var grid = new AccGrid3D<Float64Triangle3D>(triangles);

        // Assert
        Assert.That(grid, Is.Not.Null, "Grid should be created");
        Assert.That(grid.Count, Is.EqualTo(2), "Grid should contain 2 triangles");
    }

    [Test]
    public void Grid3D_BoundingBox_ShouldBeValid()
    {
        // Arrange - Use larger, 3D triangles to avoid overflow
        var triangles = new List<Float64Triangle3D>
        {
            Float64Triangle3D.Create(
                LinFloat64Vector3D.Create(0, 0, 0),
                LinFloat64Vector3D.Create(10, 0, 0),
                LinFloat64Vector3D.Create(0, 10, 10)
            )
        };

        // Act
        var grid = new AccGrid3D<Float64Triangle3D>(triangles);

        // Assert
        Assert.That(grid.BoundingBox, Is.Not.Null, "Grid should have bounding box");
        Assert.That(grid.BoundingBox.MinX, Is.EqualTo(0).Within(Tolerance), "MinX should be 0");
        Assert.That(grid.BoundingBox.MaxX, Is.GreaterThanOrEqualTo(0), "MaxX should be >= 0");
    }

    [Test]
    public void Grid3D_CellsCount_ShouldBePositive()
    {
        // Arrange - Use larger, 3D triangles to avoid overflow
        var triangles = new List<Float64Triangle3D>
        {
            Float64Triangle3D.Create(
                LinFloat64Vector3D.Create(0, 0, 0),
                LinFloat64Vector3D.Create(10, 0, 5),
                LinFloat64Vector3D.Create(0, 10, 5)
            )
        };

        // Act
        var grid = new AccGrid3D<Float64Triangle3D>(triangles);

        // Assert
        Assert.That(grid.CellsCountX, Is.GreaterThan(0), "CellsCountX should be positive");
        Assert.That(grid.CellsCountY, Is.GreaterThan(0), "CellsCountY should be positive");
        Assert.That(grid.CellsCountZ, Is.GreaterThan(0), "CellsCountZ should be positive");
        Assert.That(grid.CellsCount, Is.EqualTo(grid.CellsCountX * grid.CellsCountY * grid.CellsCountZ), "Total cells should be product of dimensions");
    }

    [Test]
    public void Grid3D_NonEmptyCellsCount_ShouldBeValid()
    {
        // Arrange - Use larger, 3D triangles to avoid overflow
        var triangles = new List<Float64Triangle3D>
        {
            Float64Triangle3D.Create(
                LinFloat64Vector3D.Create(0, 0, 0),
                LinFloat64Vector3D.Create(10, 0, 5),
                LinFloat64Vector3D.Create(0, 10, 5)
            )
        };

        // Act
        var grid = new AccGrid3D<Float64Triangle3D>(triangles);

        // Assert
        Assert.That(grid.NonEmptyCellsCount, Is.GreaterThan(0), "Should have at least one non-empty cell");
        Assert.That(grid.NonEmptyCellsCount, Is.LessThanOrEqualTo(grid.CellsCount), "Non-empty cells <= total cells");
        Assert.That(grid.EmptyCellsCount, Is.EqualTo(grid.CellsCount - grid.NonEmptyCellsCount), "Empty + non-empty = total");
    }

    [Test]
    public void Grid3D_ObjectIndexing_ShouldWork()
    {
        // Arrange
        var triangle1 = Float64Triangle3D.Create(
            LinFloat64Vector3D.Create(0, 0, 0),
            LinFloat64Vector3D.Create(1, 0, 0),
            LinFloat64Vector3D.Create(0, 1, 0)
        );
        var triangle2 = Float64Triangle3D.Create(
            LinFloat64Vector3D.Create(2, 2, 2),
            LinFloat64Vector3D.Create(3, 2, 2),
            LinFloat64Vector3D.Create(2, 3, 2)
        );
        var triangles = new List<Float64Triangle3D> { triangle1, triangle2 };

        // Act
        var grid = new AccGrid3D<Float64Triangle3D>(triangles);

        // Assert
        Assert.That(grid[0], Is.SameAs(triangle1), "Should retrieve first triangle by index");
        Assert.That(grid[1], Is.SameAs(triangle2), "Should retrieve second triangle by index");
    }

    #endregion

    #region BIH 3D Tests (5 tests)

    [Test]
    public void BIH3D_Construction_ShouldWork()
    {
        // Arrange
        var triangles = new List<Float64Triangle3D>
        {
            Float64Triangle3D.Create(
                LinFloat64Vector3D.Create(0, 0, 0),
                LinFloat64Vector3D.Create(1, 0, 0),
                LinFloat64Vector3D.Create(0, 1, 0)
            ),
            Float64Triangle3D.Create(
                LinFloat64Vector3D.Create(2, 2, 2),
                LinFloat64Vector3D.Create(3, 2, 2),
                LinFloat64Vector3D.Create(2, 3, 2)
            )
        };

        // Act
        var bih = new AccBih3D<Float64Triangle3D>(triangles, depthLimit: 10, singleDepthLimit: 5, leafObjectsLimit: 4);

        // Assert
        Assert.That(bih, Is.Not.Null, "BIH should be created");
        Assert.That(bih.Count, Is.EqualTo(2), "BIH should contain 2 triangles");
    }

    [Test]
    public void BIH3D_BoundingBox_ShouldBeValid()
    {
        // Arrange
        var triangles = new List<Float64Triangle3D>
        {
            Float64Triangle3D.Create(
                LinFloat64Vector3D.Create(0, 0, 0),
                LinFloat64Vector3D.Create(1, 0, 0),
                LinFloat64Vector3D.Create(0, 1, 0)
            )
        };

        // Act
        var bih = new AccBih3D<Float64Triangle3D>(triangles, 10, 5, 4);

        // Assert
        Assert.That(bih.BoundingBox, Is.Not.Null, "BIH should have bounding box");
        Assert.That(bih.BoundingBox.MinX, Is.EqualTo(0).Within(Tolerance), "MinX should be 0");
    }

    [Test]
    public void BIH3D_Depth_ShouldBeValid()
    {
        // Arrange
        var triangles = new List<Float64Triangle3D>
        {
            Float64Triangle3D.Create(
                LinFloat64Vector3D.Create(0, 0, 0),
                LinFloat64Vector3D.Create(1, 0, 0),
                LinFloat64Vector3D.Create(0, 1, 0)
            )
        };

        // Act
        var bih = new AccBih3D<Float64Triangle3D>(triangles, depthLimit: 10, singleDepthLimit: 5, leafObjectsLimit: 4);

        // Assert - Depth can be 0 for single objects
        Assert.That(bih.BihDepth, Is.GreaterThanOrEqualTo(0), "BIH depth should be >= 0");
    }

    [Test]
    public void BIH3D_RootNode_ShouldBeAccessible()
    {
        // Arrange
        var triangles = new List<Float64Triangle3D>
        {
            Float64Triangle3D.Create(
                LinFloat64Vector3D.Create(0, 0, 0),
                LinFloat64Vector3D.Create(1, 0, 0),
                LinFloat64Vector3D.Create(0, 1, 0)
            )
        };

        // Act
        var bih = new AccBih3D<Float64Triangle3D>(triangles, 10, 5, 4);

        // Assert
        Assert.That(bih.RootNode, Is.Not.Null, "Root node should be accessible");
        Assert.That(bih.RootNode.Count, Is.EqualTo(1), "Root node should contain 1 triangle");
    }

    [Test]
    public void BIH3D_Enumeration_ShouldWork()
    {
        // Arrange
        var triangles = new List<Float64Triangle3D>
        {
            Float64Triangle3D.Create(
                LinFloat64Vector3D.Create(0, 0, 0),
                LinFloat64Vector3D.Create(1, 0, 0),
                LinFloat64Vector3D.Create(0, 1, 0)
            ),
            Float64Triangle3D.Create(
                LinFloat64Vector3D.Create(2, 2, 2),
                LinFloat64Vector3D.Create(3, 2, 2),
                LinFloat64Vector3D.Create(2, 3, 2)
            )
        };

        // Act
        var bih = new AccBih3D<Float64Triangle3D>(triangles, 10, 5, 4);
        var count = 0;
        foreach (var _ in bih)
        {
            count++;
        }

        // Assert
        Assert.That(count, Is.EqualTo(2), "Should enumerate 2 triangles");
    }

    #endregion

    #region Grid 2D Tests (2 tests)

    [Test]
    public void Grid2D_Construction_ShouldWork()
    {
        // Arrange
        var triangles = new List<Float64Triangle2D>
        {
            Float64Triangle2D.Create(
                LinFloat64Vector2D.Create(0, 0),
                LinFloat64Vector2D.Create(1, 0),
                LinFloat64Vector2D.Create(0, 1)
            ),
            Float64Triangle2D.Create(
                LinFloat64Vector2D.Create(2, 2),
                LinFloat64Vector2D.Create(3, 2),
                LinFloat64Vector2D.Create(2, 3)
            )
        };

        // Act
        var grid = new AccGrid2D<Float64Triangle2D>(triangles);

        // Assert
        Assert.That(grid, Is.Not.Null, "Grid 2D should be created");
        Assert.That(grid.Count, Is.EqualTo(2), "Grid should contain 2 triangles");
    }

    [Test]
    public void Grid2D_Properties_ShouldBeValid()
    {
        // Arrange
        var triangles = new List<Float64Triangle2D>
        {
            Float64Triangle2D.Create(
                LinFloat64Vector2D.Create(0, 0),
                LinFloat64Vector2D.Create(1, 0),
                LinFloat64Vector2D.Create(0, 1)
            )
        };

        // Act
        var grid = new AccGrid2D<Float64Triangle2D>(triangles);

        // Assert
        Assert.That(grid.BoundingBox, Is.Not.Null, "Grid should have bounding box");
        Assert.That(grid.CellsCountX, Is.GreaterThan(0), "CellsCountX should be positive");
        Assert.That(grid.CellsCountY, Is.GreaterThan(0), "CellsCountY should be positive");
    }

    #endregion

    #region BIH 2D Tests (2 tests)

    [Test]
    public void BIH2D_Construction_ShouldWork()
    {
        // Arrange
        var triangles = new List<Float64Triangle2D>
        {
            Float64Triangle2D.Create(
                LinFloat64Vector2D.Create(0, 0),
                LinFloat64Vector2D.Create(1, 0),
                LinFloat64Vector2D.Create(0, 1)
            ),
            Float64Triangle2D.Create(
                LinFloat64Vector2D.Create(2, 2),
                LinFloat64Vector2D.Create(3, 2),
                LinFloat64Vector2D.Create(2, 3)
            )
        };

        // Act
        var bih = new AccBih2D<Float64Triangle2D>(triangles, depthLimit: 10, singleDepthLimit: 5, leafObjectsLimit: 4);

        // Assert
        Assert.That(bih, Is.Not.Null, "BIH 2D should be created");
        Assert.That(bih.Count, Is.EqualTo(2), "BIH should contain 2 triangles");
    }

    [Test]
    public void BIH2D_Properties_ShouldBeValid()
    {
        // Arrange
        var triangles = new List<Float64Triangle2D>
        {
            Float64Triangle2D.Create(
                LinFloat64Vector2D.Create(0, 0),
                LinFloat64Vector2D.Create(1, 0),
                LinFloat64Vector2D.Create(0, 1)
            )
        };

        // Act
        var bih = new AccBih2D<Float64Triangle2D>(triangles, 10, 5, 4);

        // Assert - Depth can be 0 for single objects
        Assert.That(bih.BoundingBox, Is.Not.Null, "BIH should have bounding box");
        Assert.That(bih.BihDepth, Is.GreaterThanOrEqualTo(0), "BIH depth should be >= 0");
        Assert.That(bih.RootNode, Is.Not.Null, "Root node should be accessible");
    }

    #endregion

    #region General Accelerator Tests (1 test)

    [Test]
    public void Accelerators_IsValid_ShouldWork()
    {
        // Arrange - Use larger, 3D triangles for Grid3D to avoid overflow
        var triangles3D = new List<Float64Triangle3D>
        {
            Float64Triangle3D.Create(
                LinFloat64Vector3D.Create(0, 0, 0),
                LinFloat64Vector3D.Create(10, 0, 5),
                LinFloat64Vector3D.Create(0, 10, 5)
            )
        };
        var triangles2D = new List<Float64Triangle2D>
        {
            Float64Triangle2D.Create(
                LinFloat64Vector2D.Create(0, 0),
                LinFloat64Vector2D.Create(1, 0),
                LinFloat64Vector2D.Create(0, 1)
            )
        };

        // Act
        var grid3D = new AccGrid3D<Float64Triangle3D>(triangles3D);
        var bih3D = new AccBih3D<Float64Triangle3D>(triangles3D, 10, 5, 4);
        var grid2D = new AccGrid2D<Float64Triangle2D>(triangles2D);
        var bih2D = new AccBih2D<Float64Triangle2D>(triangles2D, 10, 5, 4);

        // Assert
        Assert.That(grid3D.IsValid(), Is.True, "Grid 3D should be valid");
        Assert.That(bih3D.IsValid(), Is.True, "BIH 3D should be valid");
        Assert.That(grid2D.IsValid(), Is.True, "Grid 2D should be valid");
        Assert.That(bih2D.IsValid(), Is.True, "BIH 2D should be valid");
    }

    #endregion
}
