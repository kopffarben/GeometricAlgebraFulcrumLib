using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Adaptive;

/// <summary>
/// Represents a leaf (terminal node) in an adaptive sampling tree.
/// Leaves have no children and represent the finest level of refinement in their region.
/// </summary>
/// <typeparam name="T">The scalar type (double, float, etc.)</typeparam>
public sealed class AdaptivePath3DLeaf<T> :
    AdaptivePath3DNode<T>
{
    public override int Count
        => 0;

    public int LeafListIndex { get; }

    public AdaptivePath3DLeaf<T>? PrevLeafNode
        => LeafListIndex >= 0
            ? ParentTree.LeafNodesList[LeafListIndex - 1]
            : null;

    public AdaptivePath3DLeaf<T>? NextLeafNode
        => LeafListIndex < ParentTree.LeafNodeCount
            ? ParentTree.LeafNodesList[LeafListIndex + 1]
            : null;

    internal AdaptivePath3DLeaf(AdaptivePath3DBranch<T> parentBranch, bool isRightChild)
        : base(parentBranch, isRightChild)
    {
        LeafListIndex = parentBranch.ParentTree.LeafNodeCount;
    }


    internal override Scalar<T> UpdateLengthData(Scalar<T> length0)
    {
        Length0 = length0;

        var sp = length0.ScalarProcessor;
        var distance = (Frame1.Point - Frame0.Point).Norm();
        Length1 = sp.Add(length0.ScalarValue, distance.ScalarValue);

        return Length1;
    }

    public override IEnumerator<AdaptivePath3DNode<T>> GetEnumerator()
    {
        return Enumerable.Empty<AdaptivePath3DNode<T>>().GetEnumerator();
    }
}
