using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Adaptive;

/// <summary>
/// Abstract base class for nodes in an adaptive sampling tree.
/// Nodes can be either branches (internal nodes) or leaves (terminal nodes).
/// </summary>
/// <typeparam name="T">The scalar type (double, float, etc.)</typeparam>
public abstract class AdaptivePath3DNode<T> :
    IReadOnlyCollection<AdaptivePath3DNode<T>>
{
    public AdaptivePath3D<T> ParentTree { get; }

    public AdaptivePath3DBranch<T>? ParentBranch { get; }

    public IEnumerable<AdaptivePath3DBranch<T>> ParentBranches
    {
        get
        {
            for (var node = ParentBranch; node is not null; node = node.ParentBranch)
                yield return node;
        }
    }

    public bool IsRoot
        => ParentBranch is null;

    public bool IsLeaf
        => this is AdaptivePath3DLeaf<T>;

    public bool IsBranch
        => this is AdaptivePath3DBranch<T>;

    public bool IsChild
        => ParentBranch is not null;

    public bool IsLeftChild
        => (CellIndex & 1) == 0;

    public bool IsRightChild
        => (CellIndex & 1) == 1;

    public abstract int Count { get; }

    public int Level { get; }

    public int CellIndex { get; }

    public int FrameIndex0
        => Corner0.Index;

    public int FrameIndex1
        => Corner1.Index;

    public AdaptivePath3DCorner<T> Corner0 { get; }

    public AdaptivePath3DCorner<T> Corner1 { get; }

    public int GridIndex0
        => Corner0.GridIndex;

    public int GridIndex1
        => Corner1.GridIndex;

    public ParametricPath3DLocalFrame<T> Frame0
        => Corner0.Frame;

    public ParametricPath3DLocalFrame<T> Frame1
        => Corner1.Frame;

    public Scalar<T> MinParameterValue
        => Frame0.TimeValue;

    public Scalar<T> MaxParameterValue
        => Frame1.TimeValue;

    public Scalar<T> Length0 { get; internal set; }

    public Scalar<T> Length1 { get; internal set; }

    public Scalar<T> Length
    {
        get
        {
            var sp = Frame0.TimeValue.ScalarProcessor;
            return sp.Subtract(Length1.ScalarValue, Length0.ScalarValue);
        }
    }

    public IEnumerable<AdaptivePath3DLeaf<T>> LeafNodes
    {
        get
        {
            var stack = new Stack<AdaptivePath3DNode<T>>();

            stack.Push(this);

            while (stack.Count > 0)
            {
                var node = stack.Pop();

                if (node is not AdaptivePath3DBranch<T> branchNode)
                {
                    yield return (AdaptivePath3DLeaf<T>)node;
                    continue;
                }

                stack.Push(branchNode.Child1);
                stack.Push(branchNode.Child0);
            }
        }
    }


    /// <summary>
    /// Construct root node of tree
    /// </summary>
    /// <param name="parentTree"></param>
    protected AdaptivePath3DNode(AdaptivePath3D<T> parentTree)
    {
        ParentTree = parentTree;
        ParentBranch = null;
        Level = 0;
        CellIndex = 0;

        ParentTree.TreeLevelCount = 0;

        var position0 = new AdaptivePath3DCornerPosition(0, 0);
        var position1 = new AdaptivePath3DCornerPosition(0, 1);

        Corner0 = ParentTree.GetOrAddCorner(position0);
        Corner1 = ParentTree.GetOrAddCorner(position1);

        var sp = Corner0.Frame.TimeValue.ScalarProcessor;
        Length0 = sp.Zero;
        Length1 = sp.Zero;
    }

    /// <summary>
    /// Construct sub-node of tree
    /// </summary>
    /// <param name="parentBranch"></param>
    /// <param name="isRightChild"></param>
    protected AdaptivePath3DNode(AdaptivePath3DBranch<T> parentBranch, bool isRightChild)
    {
        Debug.Assert(parentBranch.Level < 30);

        ParentTree = parentBranch.ParentTree;
        ParentBranch = parentBranch;
        Level = parentBranch.Level + 1;
        CellIndex = parentBranch.CellIndex << 1 | (isRightChild ? 1 : 0);

        if (ParentTree.TreeLevelCount < Level)
            ParentTree.TreeLevelCount = Level;

        var position0 = new AdaptivePath3DCornerPosition(Level, CellIndex);
        var position1 = new AdaptivePath3DCornerPosition(Level, CellIndex + 1);

        Corner0 = ParentTree.GetOrAddCorner(position0);
        Corner1 = ParentTree.GetOrAddCorner(position1);

        var sp = Corner0.Frame.TimeValue.ScalarProcessor;
        Length0 = sp.Zero;
        Length1 = sp.Zero;
    }


    internal abstract Scalar<T> UpdateLengthData(Scalar<T> length0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(Scalar<T> parameterValue)
    {
        var sp = parameterValue.ScalarProcessor;
        return sp.IsZeroOrPositive<T>(sp.Subtract(parameterValue.ScalarValue, MinParameterValue.ScalarValue).ScalarValue) &&
               sp.IsZeroOrPositive<T>(sp.Subtract(MaxParameterValue.ScalarValue, parameterValue.ScalarValue).ScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsLength(Scalar<T> length)
    {
        var sp = length.ScalarProcessor;
        return sp.IsZeroOrPositive<T>(sp.Subtract(length.ScalarValue, Length0.ScalarValue).ScalarValue) &&
               sp.IsZeroOrPositive<T>(sp.Subtract(Length1.ScalarValue, length.ScalarValue).ScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Pair<ParametricPath3DLocalFrame<T>> GetEdgeFramePair()
    {
        return new Pair<ParametricPath3DLocalFrame<T>>(Frame0, Frame1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> EdgeFrameDistance()
    {
        return Frame0.Point.GetDistanceToPoint(Frame1.Point);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LinPolarAngle<T> EdgeFrameMaxAngle()
    {
        var sp = Frame0.Point.ScalarProcessor;
        var maxAngle = LinPolarAngle<T>.Angle0(sp);

        var angle = Frame0.Normal1.GetAngle(Frame1.Normal1);
        if (sp.Subtract(angle.RadiansValue, maxAngle.RadiansValue) is var diff1 &&
            sp.IsPositive<T>(diff1.ScalarValue))
            maxAngle = angle;

        angle = Frame0.Normal2.GetAngle(Frame1.Normal2);
        if (sp.Subtract(angle.RadiansValue, maxAngle.RadiansValue) is var diff2 &&
            sp.IsPositive<T>(diff2.ScalarValue))
            maxAngle = angle;

        angle = Frame0.Tangent.GetAngle(Frame1.Tangent);
        if (sp.Subtract(angle.RadiansValue, maxAngle.RadiansValue) is var diff3 &&
            sp.IsPositive<T>(diff3.ScalarValue))
            maxAngle = angle;

        return maxAngle;
    }

    public bool HasNearEdgeFrames(AdaptivePath3DSamplingOptions<T> options)
    {
        var sp = Frame0.TimeValue.ScalarProcessor;

        var parameterDistanceMax = options.MaxEdgeFramesParameterDistance;
        var parameterDistance = sp.Subtract(Frame1.TimeValue.ScalarValue, Frame0.TimeValue.ScalarValue);
        var absParameterDistance = sp.IsNegative<T>(parameterDistance.ScalarValue)
            ? sp.Negative(parameterDistance.ScalarValue)
            : parameterDistance;

        if (sp.IsZeroOrPositive<T>(sp.Subtract(parameterDistanceMax.ScalarValue, absParameterDistance.ScalarValue).ScalarValue))
            return true;

        var pointDistanceMax = options.MaxEdgeFramesDistance;
        var pointDistance = (Frame1.Point - Frame0.Point).Norm();
        if (sp.IsZeroOrPositive<T>(sp.Subtract(pointDistanceMax.ScalarValue, pointDistance.ScalarValue).ScalarValue))
            return true;

        var angleMax = options.MaxEdgeFramesAngle.RadiansValue;

        var angle = Frame0.Normal1.GetAngle(Frame1.Normal1).RadiansValue;
        if (sp.IsPositive<T>(sp.Subtract(angle, angleMax).ScalarValue))
            return false;

        angle = Frame0.Normal2.GetAngle(Frame1.Normal2).RadiansValue;
        if (sp.IsPositive<T>(sp.Subtract(angle, angleMax).ScalarValue))
            return false;

        angle = Frame0.Tangent.GetAngle(Frame1.Tangent).RadiansValue;
        if (sp.IsPositive<T>(sp.Subtract(angle, angleMax).ScalarValue))
            return false;

        return true;
    }

    public abstract IEnumerator<AdaptivePath3DNode<T>> GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
