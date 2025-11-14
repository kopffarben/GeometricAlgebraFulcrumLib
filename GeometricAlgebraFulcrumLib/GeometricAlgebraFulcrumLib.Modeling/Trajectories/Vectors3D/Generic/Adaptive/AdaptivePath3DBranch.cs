using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.Parametric.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Adaptive;

/// <summary>
/// Represents a branch (internal node) in an adaptive sampling tree.
/// Branches have exactly 2 children (either sub-branches or leaves).
/// </summary>
/// <typeparam name="T">The scalar type (double, float, etc.)</typeparam>
public sealed class AdaptivePath3DBranch<T> :
    AdaptivePath3DNode<T>
{
    public override int Count => 2;

    public AdaptivePath3DNode<T> Child0 { get; private set; }

    public AdaptivePath3DNode<T> Child1 { get; private set; }


    /// <summary>
    /// Constructor of the root node of the tree
    /// </summary>
    /// <param name="parentTree"></param>
    internal AdaptivePath3DBranch(AdaptivePath3D<T> parentTree)
        : base(parentTree)
    {
        Child0 = null!; // Will be set during GenerateTree()
        Child1 = null!;
    }

    internal AdaptivePath3DBranch(AdaptivePath3DBranch<T> parentBranch, bool isRightChild)
        : base(parentBranch, isRightChild)
    {
        Child0 = null!; // Will be set during GenerateTree()
        Child1 = null!;
    }


    private AdaptivePath3DBranch<T> CreateBranchChildren(AdaptivePath3DSamplingOptions<T> options)
    {
        Child0 = new AdaptivePath3DBranch<T>(this, false).GenerateTree(options);
        Child1 = new AdaptivePath3DBranch<T>(this, true).GenerateTree(options);

        return this;
    }

    internal AdaptivePath3DNode<T> GenerateTree(AdaptivePath3DSamplingOptions<T> options)
    {
        if (typeof(T) == typeof(double))
        {
            ApplyFloat64NormalUpdate();
        }
        else
        {
            if (ParentTree.FrameSamplingMethod == ParametricCurveLocalFrameSamplingMethod.MinimizedRotation)
                Frame1.SetMinimizedRotationNormals(Frame0);
            else
                Frame1.SetSimpleRotationNormals(Frame0);
        }


        var continueSubdivision =
            // Always subdivide the root node
            IsRoot ||

            // Continue subdivision for the required initial number of levels
            Level < options.MinLevelCount ||

            // Continue subdivision if not at max level and frame normals are far from parallel
            Level < options.MaxLevelCount && !HasNearEdgeFrames(options);

        if (continueSubdivision)
            return CreateBranchChildren(options);

        // Stop subdivision and replace this branch with a leaf node
        return ParentTree.AddLeafNode(
            new AdaptivePath3DLeaf<T>(ParentBranch!, IsRightChild)
        );
    }

    private void ApplyFloat64NormalUpdate()
    {
        var floatFrame0 = ToFloat64Frame(Frame0);
        var floatFrame1 = ToFloat64Frame(Frame1);

        if (ParentTree.FrameSamplingMethod == ParametricCurveLocalFrameSamplingMethod.MinimizedRotation)
            floatFrame1.SetMinimizedRotationNormals(floatFrame0);
        else
            floatFrame1.SetSimpleRotationNormals(floatFrame0);

        Frame1.UpdateNormals(
            ToGenericVector(floatFrame1.Normal1, Frame1.Point.ScalarProcessor),
            ToGenericVector(floatFrame1.Normal2, Frame1.Point.ScalarProcessor)
        );
    }

    private static Float64Path3DLocalFrame ToFloat64Frame(ParametricPath3DLocalFrame<T> frame)
    {
        return Float64Path3DLocalFrame.Create(
            frame.TimeValue.ScalarValue,
            LinFloat64Vector3D.Create(
                frame.Point.X.ScalarValue,
                frame.Point.Y.ScalarValue,
                frame.Point.Z.ScalarValue
            ),
            LinFloat64Vector3D.Create(
                frame.Tangent.X.ScalarValue,
                frame.Tangent.Y.ScalarValue,
                frame.Tangent.Z.ScalarValue
            ),
            new Pair<LinFloat64Vector3D>(
                LinFloat64Vector3D.Create(
                    frame.Normal1.X.ScalarValue,
                    frame.Normal1.Y.ScalarValue,
                    frame.Normal1.Z.ScalarValue
                ),
                LinFloat64Vector3D.Create(
                    frame.Normal2.X.ScalarValue,
                    frame.Normal2.Y.ScalarValue,
                    frame.Normal2.Z.ScalarValue
                )
            )
        );
    }

    private static LinVector3D<T> ToGenericVector(LinFloat64Vector3D vector, IScalarProcessor<T> scalarProcessor)
    {
        return LinVector3D<T>.Create(
            scalarProcessor.ScalarFromValue(vector.X.ScalarValue),
            scalarProcessor.ScalarFromValue(vector.Y.ScalarValue),
            scalarProcessor.ScalarFromValue(vector.Z.ScalarValue)
        );
    }

    internal AdaptivePath3DNode<T> GetChildContaining(Scalar<T> parameterValue)
    {
        if (Child0.Contains(parameterValue))
            return Child0;

        if (Child1.Contains(parameterValue))
            return Child1;

        throw new InvalidOperationException();
    }

    internal AdaptivePath3DNode<T> GetChildContainingLength(Scalar<T> length)
    {
        if (Child0.ContainsLength(length))
            return Child0;

        if (Child1.ContainsLength(length))
            return Child1;

        throw new InvalidOperationException();
    }

    internal override Scalar<T> UpdateLengthData(Scalar<T> length0)
    {
        Length0 = length0;

        length0 = Child0.UpdateLengthData(length0);
        Length1 = Child1.UpdateLengthData(length0);

        return Length1;
    }

    public override IEnumerator<AdaptivePath3DNode<T>> GetEnumerator()
    {
        yield return Child0;
        yield return Child1;
    }
}
