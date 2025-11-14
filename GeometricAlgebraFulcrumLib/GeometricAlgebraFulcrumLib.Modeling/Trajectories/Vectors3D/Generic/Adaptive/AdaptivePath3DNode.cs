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
        if (typeof(T) == typeof(double))
            return HasNearEdgeFramesFloat64(options);

        var sp = Frame0.TimeValue.ScalarProcessor;

        var parameterDistanceMax = sp.ToFloat64(options.MaxEdgeFramesParameterDistance.ScalarValue);
        var parameterDistance = Math.Abs(
            sp.ToFloat64(Frame1.TimeValue.ScalarValue) -
            sp.ToFloat64(Frame0.TimeValue.ScalarValue)
        );

        if (parameterDistance <= parameterDistanceMax)
            return true;

        var pointDistanceMax = sp.ToFloat64(options.MaxEdgeFramesDistance.ScalarValue);
        var pointDistance = sp.ToFloat64((Frame1.Point - Frame0.Point).Norm().ScalarValue);
        if (pointDistance <= pointDistanceMax)
            return true;

        var angleMax = sp.ToFloat64(options.MaxEdgeFramesAngle.RadiansValue);

        var angle = sp.ToFloat64(Frame0.Normal1.GetAngle(Frame1.Normal1).RadiansValue);
        if (angle > angleMax) return false;

        angle = sp.ToFloat64(Frame0.Normal2.GetAngle(Frame1.Normal2).RadiansValue);
        if (angle > angleMax) return false;

        angle = sp.ToFloat64(Frame0.Tangent.GetAngle(Frame1.Tangent).RadiansValue);
        if (angle > angleMax) return false;

        return true;
    }

    private bool HasNearEdgeFramesFloat64(AdaptivePath3DSamplingOptions<T> options)
    {
        static double ToFloat(Scalar<T> value)
        {
            var processor = value.ScalarProcessor;
            return processor.ToFloat64(value.ScalarValue);
        }

        static double VectorAngle(LinVector3D<T> v1, LinVector3D<T> v2)
        {
            var x1 = ToFloat(v1.X);
            var y1 = ToFloat(v1.Y);
            var z1 = ToFloat(v1.Z);

            var x2 = ToFloat(v2.X);
            var y2 = ToFloat(v2.Y);
            var z2 = ToFloat(v2.Z);

            var dot = x1 * x2 + y1 * y2 + z1 * z2;
            var len1 = Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);
            var len2 = Math.Sqrt(x2 * x2 + y2 * y2 + z2 * z2);

            var cos = Math.Clamp(dot / (len1 * len2), -1d, 1d);
            return Math.Acos(cos);
        }

        var parameterDistanceMax = ToFloat(options.MaxEdgeFramesParameterDistance);
        var parameterDistance = Math.Abs(ToFloat(Frame1.TimeValue) - ToFloat(Frame0.TimeValue));
        if (parameterDistance <= parameterDistanceMax)
            return true;

        var pointDistanceMax = ToFloat(options.MaxEdgeFramesDistance);
        var dx = ToFloat(Frame1.Point.X) - ToFloat(Frame0.Point.X);
        var dy = ToFloat(Frame1.Point.Y) - ToFloat(Frame0.Point.Y);
        var dz = ToFloat(Frame1.Point.Z) - ToFloat(Frame0.Point.Z);
        var pointDistance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
        if (pointDistance <= pointDistanceMax)
            return true;

        var angleMax = ToFloat(options.MaxEdgeFramesAngle.Radians);

        var angle = VectorAngle(Frame0.Normal1, Frame1.Normal1);
        if (angle > angleMax) return false;

        angle = VectorAngle(Frame0.Normal2, Frame1.Normal2);
        if (angle > angleMax) return false;

        angle = VectorAngle(Frame0.Tangent, Frame1.Tangent);
        if (angle > angleMax) return false;

        return true;
    }

    public abstract IEnumerator<AdaptivePath3DNode<T>> GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
