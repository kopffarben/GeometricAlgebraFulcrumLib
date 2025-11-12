using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Matrices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.Parametric.Generic;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Adaptive;

/// <summary>
/// Represents an interpolated sample point within a leaf segment of an adaptive tree.
/// Handles interpolation of position, tangent, and complete local frames.
/// </summary>
/// <typeparam name="T">The scalar type (double, float, etc.)</typeparam>
public sealed record AdaptivePath3DSample<T>
{
    public AdaptivePath3DLeaf<T> LeafNode { get; }

    public int LeafNodeIndex
        => LeafNode.LeafListIndex;

    public Scalar<T> ParameterValue { get; }

    public Scalar<T> InterpolationValue { get; }

    public ParametricCurveLocalFrameInterpolationMethod FrameInterpolationMethod
        => LeafNode.ParentTree.FrameInterpolationMethod;


    internal AdaptivePath3DSample(AdaptivePath3DLeaf<T> leafNode, Scalar<T> parameterValue)
    {
        LeafNode = leafNode;
        ParameterValue = parameterValue;

        var sp = parameterValue.ScalarProcessor;
        var numerator = sp.Subtract(parameterValue.ScalarValue, leafNode.MinParameterValue.ScalarValue);
        var denominator = sp.Subtract(leafNode.MaxParameterValue.ScalarValue, leafNode.MinParameterValue.ScalarValue);
        InterpolationValue = sp.Divide(numerator.ScalarValue, denominator.ScalarValue);
    }


    public LinVector3D<T> GetPoint()
    {
        var sp = ParameterValue.ScalarProcessor;

        // Check if at edge (avoid interpolation if exact match)
        if (sp.IsZero(sp.Subtract(ParameterValue.ScalarValue, LeafNode.MinParameterValue.ScalarValue).ScalarValue))
            return LeafNode.Frame0.Point;

        if (sp.IsZero(sp.Subtract(ParameterValue.ScalarValue, LeafNode.MaxParameterValue.ScalarValue).ScalarValue))
            return LeafNode.Frame1.Point;

        return InterpolationValue.Lerp(LeafNode.Frame0.Point, LeafNode.Frame1.Point);
    }

    public LinVector3D<T> GetTangent()
    {
        var sp = ParameterValue.ScalarProcessor;

        // Check if at edge
        if (sp.IsZero(sp.Subtract(ParameterValue.ScalarValue, LeafNode.MinParameterValue.ScalarValue).ScalarValue))
            return LeafNode.Frame0.Tangent;

        if (sp.IsZero(sp.Subtract(ParameterValue.ScalarValue, LeafNode.MaxParameterValue.ScalarValue).ScalarValue))
            return LeafNode.Frame1.Tangent;

        if (FrameInterpolationMethod == ParametricCurveLocalFrameInterpolationMethod.TangentLinearInterpolation)
            return InterpolationValue.Lerp(
                LeafNode.Frame0.Tangent,
                LeafNode.Frame1.Tangent
            ).ToUnitLinVector3D();

        var (axis, angle) =
            LeafNode.Frame0.Tangent.CreateVectorToVectorRotationAxisAngle(
                LeafNode.Frame1.Tangent
            );

        return SquareMatrix4<T>
            .CreateRotationMatrix3D(axis, angle.AngleTimes(InterpolationValue.ScalarValue))
            .MapAffineVector(LeafNode.Frame0.Tangent);
    }

    public ParametricPath3DLocalFrame<T> GetFrame()
    {
        var sp = ParameterValue.ScalarProcessor;

        // Check if at edge
        if (sp.IsZero(sp.Subtract(ParameterValue.ScalarValue, LeafNode.MinParameterValue.ScalarValue).ScalarValue))
            return LeafNode.Frame0;

        if (sp.IsZero(sp.Subtract(ParameterValue.ScalarValue, LeafNode.MaxParameterValue.ScalarValue).ScalarValue))
            return LeafNode.Frame1;

        var point =
            InterpolationValue.Lerp(LeafNode.Frame0.Point, LeafNode.Frame1.Point);

        LinVector3D<T> normal1, normal2, tangent;

        if (FrameInterpolationMethod == ParametricCurveLocalFrameInterpolationMethod.TangentLinearInterpolation)
        {
            // Use linear interpolation to find the new tangent
            tangent = InterpolationValue.Lerp(LeafNode.Frame0.Tangent, LeafNode.Frame1.Tangent).ToUnitLinVector3D();

            // Use simple rotation to rotate the normals
            (normal1, normal2) =
                SquareMatrix4<T>
                    .CreateRotationMatrix3D(LeafNode.Frame0.Tangent, tangent)
                    .MapAffineVectors(
                        LeafNode.Frame0.Normal1,
                        LeafNode.Frame0.Normal2
                    );
        }
        else
        {
            // Use spherical linear interpolation on the whole frame
            var (axis, angle) =
                LeafNode.Frame0.Tangent.CreateVectorToVectorRotationAxisAngle(
                    LeafNode.Frame1.Tangent
                );

            (normal1, normal2, tangent) =
                SquareMatrix4<T>
                    .CreateRotationMatrix3D(axis, angle.AngleTimes(InterpolationValue.ScalarValue))
                    .MapAffineVectors(
                        LeafNode.Frame0.Normal1,
                        LeafNode.Frame0.Normal2,
                        LeafNode.Frame0.Tangent
                    );
        }

        return ParametricPath3DLocalFrame<T>.Create(
            ParameterValue,
            point,
            tangent,
            normal1,
            normal2
        );
    }
}
