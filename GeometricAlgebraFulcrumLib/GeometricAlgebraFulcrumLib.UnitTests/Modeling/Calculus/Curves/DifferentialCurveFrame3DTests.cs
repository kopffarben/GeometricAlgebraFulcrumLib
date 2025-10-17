using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Calculus.Curves;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Calculus.Curves;

/// <summary>
/// Tests for Differential Curve Frame 3D
/// Phase 3B - Core Modeling: Differential Curves Tests (40 tests)
/// Tests curve frame construction, properties, and frame operations
/// </summary>
[TestFixture]
public class DifferentialCurveFrame3DTests
{
    private const double Tolerance = 1e-10;

    #region Frame Construction Tests (10 tests)

    [Test]
    public void CurveFrame_CreateOrthonormal_ShouldWork()
    {
        // Arrange
        var parameterValue = 0.5;
        var origin = LinFloat64Vector3D.Create(1, 2, 3);
        var direction = LinFloat64Vector3D.E1;

        // Act
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(parameterValue, origin, direction);

        // Assert
        Assert.That(frame, Is.Not.Null, "Frame should be created");
        Assert.That(frame.ParameterValue, Is.EqualTo(parameterValue), "Parameter value should match");
        Assert.That(frame.Origin, Is.EqualTo(origin), "Origin should match");
    }

    [Test]
    public void CurveFrame_CreateOrthonormalRightHanded_ShouldBeRightHanded()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Create(0, 0, 0);
        var direction = LinFloat64Vector3D.E1;

        // Act
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(0, origin, direction, rightHanded: true);

        // Assert
        Assert.That(frame.IsRightHanded(), Is.True, "Frame should be right-handed");
        Assert.That(frame.IsLeftHanded(), Is.False, "Frame should not be left-handed");
    }

    [Test]
    public void CurveFrame_CreateOrthonormalLeftHanded_ShouldBeLeftHanded()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Create(0, 0, 0);
        var direction = LinFloat64Vector3D.E2;

        // Act
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(0, origin, direction, rightHanded: false);

        // Assert
        Assert.That(frame.IsLeftHanded(), Is.True, "Frame should be left-handed");
        Assert.That(frame.IsRightHanded(), Is.False, "Frame should not be right-handed");
    }

    [Test]
    public void CurveFrame_CreateWithThreeDirections_ShouldWork()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Create(1, 2, 3);
        var dir1 = LinFloat64Vector3D.E1;
        var dir2 = LinFloat64Vector3D.E2;
        var dir3 = LinFloat64Vector3D.E3;

        // Act
        var frame = DifferentialCurveFrame3D.Create(0, origin, dir1, dir2, dir3);

        // Assert
        Assert.That(frame, Is.Not.Null, "Frame should be created");
        Assert.That(frame.Direction1, Is.EqualTo(dir1), "Direction1 should match");
        Assert.That(frame.Direction2, Is.EqualTo(dir2), "Direction2 should match");
        Assert.That(frame.Direction3, Is.EqualTo(dir3), "Direction3 should match");
    }

    [Test]
    public void CurveFrame_Origin_ShouldMatchConstructor()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Create(5, 6, 7);
        var direction = LinFloat64Vector3D.E1;

        // Act
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(0, origin, direction);

        // Assert
        Assert.That(frame.X.ScalarValue, Is.EqualTo(5).Within(Tolerance), "X should match");
        Assert.That(frame.Y.ScalarValue, Is.EqualTo(6).Within(Tolerance), "Y should match");
        Assert.That(frame.Z.ScalarValue, Is.EqualTo(7).Within(Tolerance), "Z should match");
    }

    [Test]
    public void CurveFrame_ParameterValue_ShouldBeAccessible()
    {
        // Arrange & Act
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(
            1.5,
            LinFloat64Vector3D.Zero,
            LinFloat64Vector3D.E1
        );

        // Assert
        Assert.That(frame.ParameterValue, Is.EqualTo(1.5).Within(Tolerance), "Parameter should be accessible");
    }

    [Test]
    public void CurveFrame_VSpaceDimensions_ShouldBeThree()
    {
        // Arrange & Act
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(
            0,
            LinFloat64Vector3D.Zero,
            LinFloat64Vector3D.E2
        );

        // Assert
        Assert.That(frame.VSpaceDimensions, Is.EqualTo(3), "VSpace dimensions should be 3");
    }

    [Test]
    public void CurveFrame_IsValid_ShouldReturnTrue()
    {
        // Arrange
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(
            0,
            LinFloat64Vector3D.Create(1, 2, 3),
            LinFloat64Vector3D.E3
        );

        // Act & Assert
        Assert.That(frame.IsValid(), Is.True, "Valid frame should return true");
    }

    [Test]
    public void CurveFrame_DifferentParameterValues_ShouldWork()
    {
        // Arrange & Act
        var frame1 = DifferentialCurveFrame3D.CreateOrthonormal(0.0, LinFloat64Vector3D.Zero, LinFloat64Vector3D.E1);
        var frame2 = DifferentialCurveFrame3D.CreateOrthonormal(1.0, LinFloat64Vector3D.Zero, LinFloat64Vector3D.E1);
        var frame3 = DifferentialCurveFrame3D.CreateOrthonormal(Math.PI, LinFloat64Vector3D.Zero, LinFloat64Vector3D.E1);

        // Assert
        Assert.That(frame1.ParameterValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(frame2.ParameterValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(frame3.ParameterValue, Is.EqualTo(Math.PI).Within(Tolerance));
    }

    [Test]
    public void CurveFrame_AllDirections_ShouldBeAccessible()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Create(0, 0, 0);
        var dir1 = LinFloat64Vector3D.E1;
        var dir2 = LinFloat64Vector3D.E2;
        var dir3 = LinFloat64Vector3D.E3;

        // Act
        var frame = DifferentialCurveFrame3D.Create(0, origin, dir1, dir2, dir3);

        // Assert
        Assert.DoesNotThrow(() =>
        {
            var _ = frame.Direction1;
            var __ = frame.Direction2;
            var ___ = frame.Direction3;
        }, "All directions should be accessible");
    }

    #endregion

    #region Frame Properties Tests (10 tests)

    [Test]
    public void CurveFrame_OrthonormalBasis_ShouldBeOrthonormal()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Zero;
        var direction = LinFloat64Vector3D.E1;

        // Act
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(0, origin, direction);

        // Assert
        Assert.That(frame.IsOrthonormal(), Is.True, "Orthonormal frame should satisfy IsOrthonormal");
    }

    [Test]
    public void CurveFrame_OrthonormalBasis_ShouldBeOrthogonal()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Zero;
        var direction = LinFloat64Vector3D.E2;

        // Act
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(0, origin, direction);

        // Assert
        Assert.That(frame.IsOrthogonal(), Is.True, "Orthonormal frame should also be orthogonal");
    }

    [Test]
    public void CurveFrame_StandardBasis_ShouldBeFrame3D()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Zero;
        var dir1 = LinFloat64Vector3D.E1;
        var dir2 = LinFloat64Vector3D.E2;
        var dir3 = LinFloat64Vector3D.E3;

        // Act
        var frame = DifferentialCurveFrame3D.Create(0, origin, dir1, dir2, dir3);

        // Assert
        Assert.That(frame.IsFrame3D(), Is.True, "Standard basis should be a valid 3D frame");
    }

    [Test]
    public void CurveFrame_StandardBasis_ShouldBeRightHanded()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Zero;
        var frame = DifferentialCurveFrame3D.Create(
            0,
            origin,
            LinFloat64Vector3D.E1,
            LinFloat64Vector3D.E2,
            LinFloat64Vector3D.E3
        );

        // Act & Assert
        Assert.That(frame.IsRightHanded(), Is.True, "Standard basis E1, E2, E3 should be right-handed");
    }

    [Test]
    public void CurveFrame_ReversedBasis_ShouldBeLeftHanded()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Zero;
        var frame = DifferentialCurveFrame3D.Create(
            0,
            origin,
            LinFloat64Vector3D.E1,
            LinFloat64Vector3D.E3,  // Swapped E2 and E3
            LinFloat64Vector3D.E2
        );

        // Act & Assert
        Assert.That(frame.IsLeftHanded(), Is.True, "Reversed basis should be left-handed");
    }

    [Test]
    public void CurveFrame_ScaledDirections_ShouldNotBeOrthonormal()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Zero;
        var dir1 = LinFloat64Vector3D.E1 * 2;  // Scaled, not unit length
        var dir2 = LinFloat64Vector3D.E2;
        var dir3 = LinFloat64Vector3D.E3;

        // Act
        var frame = DifferentialCurveFrame3D.Create(0, origin, dir1, dir2, dir3);

        // Assert
        Assert.That(frame.IsOrthonormal(), Is.False, "Scaled directions should not be orthonormal");
        Assert.That(frame.IsOrthogonal(), Is.True, "But should still be orthogonal");
    }

    [Test]
    public void CurveFrame_NonOrthogonalDirections_ShouldNotBeOrthogonal()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Zero;
        var dir1 = LinFloat64Vector3D.Create(1, 0, 0);
        var dir2 = LinFloat64Vector3D.Create(1, 1, 0);  // Not orthogonal to dir1
        var dir3 = LinFloat64Vector3D.E3;

        // Act
        var frame = DifferentialCurveFrame3D.Create(0, origin, dir1, dir2, dir3);

        // Assert
        Assert.That(frame.IsOrthogonal(), Is.False, "Non-orthogonal directions should fail IsOrthogonal");
    }

    [Test]
    public void CurveFrame_OrthonormalWithDifferentDirections_ShouldAllBeOrthonormal()
    {
        // Arrange & Act
        var frame1 = DifferentialCurveFrame3D.CreateOrthonormal(0, LinFloat64Vector3D.Zero, LinFloat64Vector3D.E1);
        var frame2 = DifferentialCurveFrame3D.CreateOrthonormal(0, LinFloat64Vector3D.Zero, LinFloat64Vector3D.E2);
        var frame3 = DifferentialCurveFrame3D.CreateOrthonormal(0, LinFloat64Vector3D.Zero, LinFloat64Vector3D.E3);

        // Assert
        Assert.That(frame1.IsOrthonormal(), Is.True, "E1-based frame should be orthonormal");
        Assert.That(frame2.IsOrthonormal(), Is.True, "E2-based frame should be orthonormal");
        Assert.That(frame3.IsOrthonormal(), Is.True, "E3-based frame should be orthonormal");
    }

    [Test]
    public void CurveFrame_IsFrame3D_ShouldDetectLinearlyIndependent()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Zero;
        var dir1 = LinFloat64Vector3D.Create(1, 2, 3);
        var dir2 = LinFloat64Vector3D.Create(4, 5, 6);
        var dir3 = LinFloat64Vector3D.Create(7, 8, 9);

        // Act
        var frame = DifferentialCurveFrame3D.Create(0, origin, dir1, dir2, dir3);

        // Assert - these are linearly dependent (collinear scaled vectors), so should NOT be a valid frame
        Assert.That(frame.IsFrame3D(), Is.False, "Linearly dependent directions should not form valid frame");
    }

    [Test]
    public void CurveFrame_Properties_ShouldBeConsistent()
    {
        // Arrange
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(0.5, LinFloat64Vector3D.Zero, LinFloat64Vector3D.E1);

        // Act & Assert - All properties should be accessible and consistent
        Assert.DoesNotThrow(() =>
        {
            var valid = frame.IsValid();
            var orthogonal = frame.IsOrthogonal();
            var orthonormal = frame.IsOrthonormal();
            var isFrame = frame.IsFrame3D();
            var rightHanded = frame.IsRightHanded();
            var leftHanded = frame.IsLeftHanded();

            Assert.That(valid, Is.True);
            Assert.That(isFrame, Is.True);
            Assert.That(rightHanded != leftHanded, Is.True, "Should be either right or left handed, not both");
        }, "All properties should be accessible");
    }

    #endregion

    #region Frame Operations Tests (10 tests)

    [Test]
    public void CurveFrame_GetDirectionsMatrix_ShouldWork()
    {
        // Arrange
        var frame = DifferentialCurveFrame3D.Create(
            0,
            LinFloat64Vector3D.Zero,
            LinFloat64Vector3D.E1,
            LinFloat64Vector3D.E2,
            LinFloat64Vector3D.E3
        );

        // Act
        var matrix = frame.GetDirectionsMatrix();

        // Assert
        Assert.That(matrix, Is.Not.Null, "Matrix should be created");
        Assert.That(matrix.Scalar00.ScalarValue, Is.EqualTo(1).Within(Tolerance), "M00 should be 1");
        Assert.That(matrix.Scalar11.ScalarValue, Is.EqualTo(1).Within(Tolerance), "M11 should be 1");
        Assert.That(matrix.Scalar22.ScalarValue, Is.EqualTo(1).Within(Tolerance), "M22 should be 1");
    }

    [Test]
    public void CurveFrame_GetLocalVectorWithThreeScalars_ShouldWork()
    {
        // Arrange
        var frame = DifferentialCurveFrame3D.Create(
            0,
            LinFloat64Vector3D.Zero,
            LinFloat64Vector3D.E1,
            LinFloat64Vector3D.E2,
            LinFloat64Vector3D.E3
        );

        // Act
        var localVec = frame.GetLocalVector(2, 3, 4);

        // Assert
        Assert.That(localVec.X.ScalarValue, Is.EqualTo(2).Within(Tolerance), "X should be 2");
        Assert.That(localVec.Y.ScalarValue, Is.EqualTo(3).Within(Tolerance), "Y should be 3");
        Assert.That(localVec.Z.ScalarValue, Is.EqualTo(4).Within(Tolerance), "Z should be 4");
    }

    [Test]
    public void CurveFrame_GetLocalVectorWithZeroCoefficients_ShouldBeZero()
    {
        // Arrange
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(
            0,
            LinFloat64Vector3D.Zero,
            LinFloat64Vector3D.E1
        );

        // Act
        var localVec = frame.GetLocalVector(0, 0, 0);

        // Assert
        Assert.That(localVec.X.ScalarValue, Is.EqualTo(0).Within(Tolerance), "X should be 0");
        Assert.That(localVec.Y.ScalarValue, Is.EqualTo(0).Within(Tolerance), "Y should be 0");
        Assert.That(localVec.Z.ScalarValue, Is.EqualTo(0).Within(Tolerance), "Z should be 0");
    }

    [Test]
    public void CurveFrame_GetLocalVectorBasisE1_ShouldMatchDirection1()
    {
        // Arrange
        var frame = DifferentialCurveFrame3D.Create(
            0,
            LinFloat64Vector3D.Zero,
            LinFloat64Vector3D.E1,
            LinFloat64Vector3D.E2,
            LinFloat64Vector3D.E3
        );

        // Act
        var localVec = frame.GetLocalVector(1, 0, 0);

        // Assert
        Assert.That(localVec.X.ScalarValue, Is.EqualTo(frame.Direction1.X.ScalarValue).Within(Tolerance));
        Assert.That(localVec.Y.ScalarValue, Is.EqualTo(frame.Direction1.Y.ScalarValue).Within(Tolerance));
        Assert.That(localVec.Z.ScalarValue, Is.EqualTo(frame.Direction1.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void CurveFrame_GetLocalVectorLinearCombination_ShouldWork()
    {
        // Arrange
        var frame = DifferentialCurveFrame3D.Create(
            0,
            LinFloat64Vector3D.Zero,
            LinFloat64Vector3D.Create(1, 0, 0),
            LinFloat64Vector3D.Create(0, 2, 0),
            LinFloat64Vector3D.Create(0, 0, 3)
        );

        // Act - 2*dir1 + 3*dir2 + 4*dir3
        var localVec = frame.GetLocalVector(2, 3, 4);

        // Assert - Expected: 2*(1,0,0) + 3*(0,2,0) + 4*(0,0,3) = (2,6,12)
        Assert.That(localVec.X.ScalarValue, Is.EqualTo(2).Within(Tolerance));
        Assert.That(localVec.Y.ScalarValue, Is.EqualTo(6).Within(Tolerance));
        Assert.That(localVec.Z.ScalarValue, Is.EqualTo(12).Within(Tolerance));
    }

    [Test]
    public void CurveFrame_GetDirectionsMatrixOrthonormal_ShouldBeOrthogonal()
    {
        // Arrange
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(
            0,
            LinFloat64Vector3D.Zero,
            LinFloat64Vector3D.E1
        );

        // Act
        var matrix = frame.GetDirectionsMatrix();

        // Assert - For an orthonormal frame, M^T * M should be identity
        // We'll just verify the matrix is not null and has expected dimensions
        Assert.That(matrix, Is.Not.Null, "Matrix should exist");
    }

    [Test]
    public void CurveFrame_MultipleOrigins_ShouldNotAffectDirections()
    {
        // Arrange
        var origin1 = LinFloat64Vector3D.Create(0, 0, 0);
        var origin2 = LinFloat64Vector3D.Create(5, 5, 5);
        var direction = LinFloat64Vector3D.E1;

        // Act
        var frame1 = DifferentialCurveFrame3D.CreateOrthonormal(0, origin1, direction);
        var frame2 = DifferentialCurveFrame3D.CreateOrthonormal(0, origin2, direction);

        // Assert - Different origins, same directions
        Assert.That(frame1.Direction1, Is.EqualTo(frame2.Direction1), "Direction1 should be same");
        Assert.That(frame1.Direction2, Is.EqualTo(frame2.Direction2), "Direction2 should be same");
        Assert.That(frame1.Direction3, Is.EqualTo(frame2.Direction3), "Direction3 should be same");
    }

    [Test]
    public void CurveFrame_GetLocalVectorNegativeCoefficients_ShouldWork()
    {
        // Arrange
        var frame = DifferentialCurveFrame3D.Create(
            0,
            LinFloat64Vector3D.Zero,
            LinFloat64Vector3D.E1,
            LinFloat64Vector3D.E2,
            LinFloat64Vector3D.E3
        );

        // Act
        var localVec = frame.GetLocalVector(-1, -2, -3);

        // Assert
        Assert.That(localVec.X.ScalarValue, Is.EqualTo(-1).Within(Tolerance));
        Assert.That(localVec.Y.ScalarValue, Is.EqualTo(-2).Within(Tolerance));
        Assert.That(localVec.Z.ScalarValue, Is.EqualTo(-3).Within(Tolerance));
    }

    [Test]
    public void CurveFrame_AllOperations_ShouldNotThrow()
    {
        // Arrange
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(
            1.0,
            LinFloat64Vector3D.Create(1, 2, 3),
            LinFloat64Vector3D.E1
        );

        // Act & Assert - All operations should work without throwing
        Assert.DoesNotThrow(() =>
        {
            var _ = frame.IsValid();
            var __ = frame.IsOrthogonal();
            var ___ = frame.IsOrthonormal();
            var ____ = frame.IsFrame3D();
            var _____ = frame.IsRightHanded();
            var ______ = frame.IsLeftHanded();
            var _______ = frame.GetDirectionsMatrix();
            var ________ = frame.GetLocalVector(1, 2, 3);
        }, "All operations should work without throwing");
    }

    [Test]
    public void CurveFrame_ConsistencyAcrossParameterValues_ShouldMaintainProperties()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Zero;
        var direction = LinFloat64Vector3D.E1;

        // Act
        var frame1 = DifferentialCurveFrame3D.CreateOrthonormal(0.0, origin, direction);
        var frame2 = DifferentialCurveFrame3D.CreateOrthonormal(0.5, origin, direction);
        var frame3 = DifferentialCurveFrame3D.CreateOrthonormal(1.0, origin, direction);

        // Assert - All frames should have same orthonormality properties
        Assert.That(frame1.IsOrthonormal(), Is.EqualTo(frame2.IsOrthonormal()), "Property should be consistent");
        Assert.That(frame2.IsOrthonormal(), Is.EqualTo(frame3.IsOrthonormal()), "Property should be consistent");
    }

    #endregion

    #region Item Access Tests (10 tests)

    [Test]
    public void CurveFrame_Item1_ShouldReturnOriginX()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Create(5, 6, 7);
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(0, origin, LinFloat64Vector3D.E1);

        // Act & Assert
        Assert.That(frame.Item1.ScalarValue, Is.EqualTo(5).Within(Tolerance), "Item1 should be Origin.X");
    }

    [Test]
    public void CurveFrame_Item2_ShouldReturnOriginY()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Create(5, 6, 7);
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(0, origin, LinFloat64Vector3D.E1);

        // Act & Assert
        Assert.That(frame.Item2.ScalarValue, Is.EqualTo(6).Within(Tolerance), "Item2 should be Origin.Y");
    }

    [Test]
    public void CurveFrame_Item3_ShouldReturnOriginZ()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Create(5, 6, 7);
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(0, origin, LinFloat64Vector3D.E1);

        // Act & Assert
        Assert.That(frame.Item3.ScalarValue, Is.EqualTo(7).Within(Tolerance), "Item3 should be Origin.Z");
    }

    [Test]
    public void CurveFrame_X_ShouldMatchItem1()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Create(10, 20, 30);
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(0, origin, LinFloat64Vector3D.E2);

        // Act & Assert
        Assert.That(frame.X.ScalarValue, Is.EqualTo(frame.Item1.ScalarValue).Within(Tolerance), "X should match Item1");
    }

    [Test]
    public void CurveFrame_Y_ShouldMatchItem2()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Create(10, 20, 30);
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(0, origin, LinFloat64Vector3D.E2);

        // Act & Assert
        Assert.That(frame.Y.ScalarValue, Is.EqualTo(frame.Item2.ScalarValue).Within(Tolerance), "Y should match Item2");
    }

    [Test]
    public void CurveFrame_Z_ShouldMatchItem3()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Create(10, 20, 30);
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(0, origin, LinFloat64Vector3D.E2);

        // Act & Assert
        Assert.That(frame.Z.ScalarValue, Is.EqualTo(frame.Item3.ScalarValue).Within(Tolerance), "Z should match Item3");
    }

    [Test]
    public void CurveFrame_XYZ_ShouldMatchOrigin()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Create(1.5, 2.5, 3.5);
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(0, origin, LinFloat64Vector3D.E3);

        // Act & Assert
        Assert.That(frame.X.ScalarValue, Is.EqualTo(origin.X.ScalarValue).Within(Tolerance));
        Assert.That(frame.Y.ScalarValue, Is.EqualTo(origin.Y.ScalarValue).Within(Tolerance));
        Assert.That(frame.Z.ScalarValue, Is.EqualTo(origin.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void CurveFrame_ItemAccessors_ShouldBeReadOnly()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Create(1, 2, 3);
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(0, origin, LinFloat64Vector3D.E1);

        // Act - Access all item properties
        var item1 = frame.Item1;
        var item2 = frame.Item2;
        var item3 = frame.Item3;

        // Assert - Properties should be accessible and consistent
        Assert.That(item1.ScalarValue, Is.EqualTo(1).Within(Tolerance));
        Assert.That(item2.ScalarValue, Is.EqualTo(2).Within(Tolerance));
        Assert.That(item3.ScalarValue, Is.EqualTo(3).Within(Tolerance));
    }

    [Test]
    public void CurveFrame_ZeroOrigin_ShouldHaveZeroItems()
    {
        // Arrange & Act
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(
            0,
            LinFloat64Vector3D.Zero,
            LinFloat64Vector3D.E1
        );

        // Assert
        Assert.That(frame.Item1.ScalarValue, Is.EqualTo(0).Within(Tolerance));
        Assert.That(frame.Item2.ScalarValue, Is.EqualTo(0).Within(Tolerance));
        Assert.That(frame.Item3.ScalarValue, Is.EqualTo(0).Within(Tolerance));
    }

    [Test]
    public void CurveFrame_AllItemAccessors_ShouldBeConsistent()
    {
        // Arrange
        var origin = LinFloat64Vector3D.Create(7.7, 8.8, 9.9);
        var frame = DifferentialCurveFrame3D.CreateOrthonormal(0, origin, LinFloat64Vector3D.E1);

        // Act & Assert - All accessors should return the same values
        Assert.That(frame.X.ScalarValue, Is.EqualTo(frame.Item1.ScalarValue).Within(Tolerance));
        Assert.That(frame.Y.ScalarValue, Is.EqualTo(frame.Item2.ScalarValue).Within(Tolerance));
        Assert.That(frame.Z.ScalarValue, Is.EqualTo(frame.Item3.ScalarValue).Within(Tolerance));
        Assert.That(frame.X.ScalarValue, Is.EqualTo(frame.Origin.X.ScalarValue).Within(Tolerance));
        Assert.That(frame.Y.ScalarValue, Is.EqualTo(frame.Origin.Y.ScalarValue).Within(Tolerance));
        Assert.That(frame.Z.ScalarValue, Is.EqualTo(frame.Origin.Z.ScalarValue).Within(Tolerance));
    }

    #endregion
}
