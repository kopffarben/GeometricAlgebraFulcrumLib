using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
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
        // TODO: Implement SetMinimizedRotationNormals and SetSimpleRotationNormals for Generic<T>
        // These methods update Frame1 normals based on Frame0 to minimize rotation
        // For now, skip this optimization - frames use their original normals
        // if (ParentTree.FrameSamplingMethod == ParametricCurveLocalFrameSamplingMethod.MinimizedRotation)
        //     Frame1.SetMinimizedRotationNormals(Frame0);
        // else
        //     Frame1.SetSimpleRotationNormals(Frame0);


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
