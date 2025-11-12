using System.Collections;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.Parametric.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Adaptive;

/// <summary>
/// Represents an adaptive sampling tree for a parametric curve with arc-length parameterization.
/// Automatically refines the sampling based on curvature and distance criteria.
/// </summary>
/// <typeparam name="T">The scalar type (double, float, etc.)</typeparam>
public sealed class AdaptivePath3D<T> :
    ArcLengthPath3D<T>,
    IReadOnlyList<ParametricPath3DLocalFrame<T>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> Finite(ParametricPath3D<T> basePath)
    {
        return new AdaptivePath3D<T>(
            basePath.TimeRange,
            false,
            basePath
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> Finite(ScalarRange<T> timeRange, ParametricPath3D<T> basePath)
    {
        return new AdaptivePath3D<T>(timeRange, false, basePath);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> Periodic(ParametricPath3D<T> basePath)
    {
        return new AdaptivePath3D<T>(
            basePath.TimeRange,
            true,
            basePath
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> Periodic(ScalarRange<T> timeRange, ParametricPath3D<T> basePath)
    {
        return new AdaptivePath3D<T>(timeRange, true, basePath);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> Create(ParametricPath3D<T> basePath)
    {
        return new AdaptivePath3D<T>(
            basePath.TimeRange,
            basePath.IsPeriodic,
            basePath
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> Create(ScalarRange<T> timeRange, ParametricPath3D<T> basePath)
    {
        return new AdaptivePath3D<T>(timeRange, basePath.IsPeriodic, basePath);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> Create(bool isPeriodic, ParametricPath3D<T> basePath)
    {
        return new AdaptivePath3D<T>(basePath.TimeRange, isPeriodic, basePath);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, ParametricPath3D<T> basePath)
    {
        return new AdaptivePath3D<T>(timeRange, isPeriodic, basePath);
    }


    private readonly Dictionary<AdaptivePath3DCornerPosition, int> _cornerDictionary
        = new Dictionary<AdaptivePath3DCornerPosition, int>();

    private readonly List<AdaptivePath3DCorner<T>> _cornerList
        = new List<AdaptivePath3DCorner<T>>();

    private readonly List<AdaptivePath3DLeaf<T>> _leafNodeList
        = new List<AdaptivePath3DLeaf<T>>();


    public int Count
        => 1 + _leafNodeList.Count;

    public ParametricPath3DLocalFrame<T> this[int index]
        => index == _leafNodeList.Count
            ? _leafNodeList[^1].Frame1
            : _leafNodeList[index].Frame0;

    public ParametricPath3D<T> Curve { get; }

    public AdaptivePath3DBranch<T>? RootNode { get; private set; }

    public int TreeLevelCount { get; internal set; }

    /// <summary>
    /// The number of segments per grid side for this tree
    /// </summary>
    public int GridSegmentCount
        => 1 << TreeLevelCount;

    public IEnumerable<AdaptivePath3DNode<T>> Nodes
    {
        get
        {
            var stack = new Stack<AdaptivePath3DNode<T>>();

            stack.Push(RootNode!);

            while (stack.Count > 0)
            {
                var node = stack.Pop();

                yield return node;

                if (node is not AdaptivePath3DBranch<T> branchNode)
                    continue;

                stack.Push(branchNode.Child1);
                stack.Push(branchNode.Child0);
            }
        }
    }

    public IEnumerable<AdaptivePath3DBranch<T>> BranchNodes
    {
        get
        {
            var stack = new Stack<AdaptivePath3DBranch<T>>();

            stack.Push(RootNode!);

            while (stack.Count > 0)
            {
                var branchNode = stack.Pop();

                yield return branchNode;

                if (branchNode.Child1 is AdaptivePath3DBranch<T> childBranchNode1)
                    stack.Push(childBranchNode1);

                if (branchNode.Child0 is AdaptivePath3DBranch<T> childBranchNode0)
                    stack.Push(childBranchNode0);
            }
        }
    }

    public IEnumerable<AdaptivePath3DLeaf<T>> LeafNodes
    {
        get
        {
            var stack = new Stack<AdaptivePath3DNode<T>>();

            stack.Push(RootNode!);

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

    public int LeafNodeCount
        => _leafNodeList.Count;

    public IReadOnlyList<AdaptivePath3DLeaf<T>> LeafNodesList
        => _leafNodeList;

    public Scalar<T> Length
        => RootNode!.Length1;

    public override Scalar<T> GetLength()
    {
        var sp = TimeRange.ScalarProcessor;
        var arcLength = sp.Zero;

        ParametricPath3DLocalFrame<T>? frame1 = null;
        var firstFrame = true;
        foreach (var frame2 in this)
        {
            if (firstFrame)
            {
                frame1 = frame2;
                firstFrame = false;
                continue;
            }

            var distance = frame2.Point.GetDistanceToPoint(frame1!.Point);
            arcLength = sp.Add(arcLength.ScalarValue, distance.ScalarValue);

            frame1 = frame2;
        }

        return arcLength;
    }

    public ParametricCurveLocalFrameInterpolationMethod FrameInterpolationMethod { get; set; }
        = ParametricCurveLocalFrameInterpolationMethod.TangentLinearInterpolation;

    public ParametricCurveLocalFrameSamplingMethod FrameSamplingMethod { get; set; }
        = ParametricCurveLocalFrameSamplingMethod.SimpleRotation;

    public int CornerCount
        => _cornerList.Count;

    internal IReadOnlyList<AdaptivePath3DCorner<T>> CornerList
        => _cornerList;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AdaptivePath3D(ScalarRange<T> timeRange, bool isPeriodic, ParametricPath3D<T> surface)
        : base(timeRange, isPeriodic)
    {
        Curve = surface;
        RootNode = null;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AdaptivePath3D<T> Clear()
    {
        RootNode = null;
        _cornerList.Clear();
        _leafNodeList.Clear();
        _cornerDictionary.Clear();

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> GetParameterValue(AdaptivePath3DCornerPosition cornerPosition)
    {
        var t = cornerPosition.GetInterpolationValue();
        var sp = TimeRange.ScalarProcessor;
        var tScalar = sp.ScalarFromNumber(t);

        // Manual Lerp: result = (1 - t) * a + t * b
        var oneMinusT = sp.Subtract(sp.OneValue, tScalar.ScalarValue);
        var term1 = sp.Times(oneMinusT.ScalarValue, TimeRange.MinValue.ScalarValue);
        var term2 = sp.Times(tScalar.ScalarValue, TimeRange.MaxValue.ScalarValue);
        return sp.Add(term1.ScalarValue, term2.ScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AdaptivePath3D<T> GenerateTree(AdaptivePath3DSamplingOptions<T> options)
    {
        Clear();

        RootNode = new AdaptivePath3DBranch<T>(this);

        RootNode.GenerateTree(options);

        var sp = TimeRange.ScalarProcessor;
        RootNode.UpdateLengthData(sp.Zero);

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return RootNode is not null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> parameterValue)
    {
        return GetSample(parameterValue).GetPoint();
    }

    public override ArcLengthPath3D<T> ToFiniteArcLengthPath()
    {
        throw new NotImplementedException();
    }

    public override ArcLengthPath3D<T> ToPeriodicArcLengthPath()
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> parameterValue)
    {
        return GetSample(parameterValue).GetTangent();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> parameterValue)
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3DLocalFrame<T> GetFrame(Scalar<T> parameterValue)
    {
        return GetSample(parameterValue).GetFrame();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsCorner(AdaptivePath3DCornerPosition cornerPosition)
    {
        return _cornerDictionary.ContainsKey(cornerPosition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AdaptivePath3DCorner<T> GetCorner(int cornerIndex)
    {
        return _cornerList[cornerIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AdaptivePath3DCorner<T> GetCorner(AdaptivePath3DCornerPosition cornerPosition)
    {
        var cornerIndex = _cornerDictionary[cornerPosition];

        return _cornerList[cornerIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetCornerIndex(AdaptivePath3DCornerPosition cornerPosition)
    {
        return _cornerDictionary[cornerPosition];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetCorner(AdaptivePath3DCornerPosition cornerPosition, out AdaptivePath3DCorner<T>? corner)
    {
        if (_cornerDictionary.TryGetValue(cornerPosition, out var cornerIndex))
        {
            corner = _cornerList[cornerIndex];
            return true;
        }

        corner = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetCornerIndex(AdaptivePath3DCornerPosition cornerPosition, out int cornerIndex)
    {
        return _cornerDictionary.TryGetValue(cornerPosition, out cornerIndex);
    }

    internal AdaptivePath3DCorner<T> GetOrAddCorner(AdaptivePath3DCornerPosition cornerPosition)
    {
        if (_cornerDictionary.TryGetValue(cornerPosition, out var index))
            return _cornerList[index];

        var parameterValue =
            GetParameterValue(cornerPosition);

        index = _cornerList.Count;
        var frame = Curve.GetFrame(parameterValue);
        var corner = new AdaptivePath3DCorner<T>(this, index, frame, cornerPosition);

        _cornerList.Add(corner);
        _cornerDictionary.Add(cornerPosition, index);

        return corner;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal AdaptivePath3DLeaf<T> AddLeafNode(AdaptivePath3DLeaf<T> leafNode)
    {
        _leafNodeList.Add(leafNode);

        return leafNode;
    }

    public AdaptivePath3DSample<T> GetSample(Scalar<T> parameterValue)
    {
        if (!RootNode!.Contains(parameterValue))
            throw new ArgumentOutOfRangeException();

        var branchNode = RootNode;

        while (true)
        {
            var childNode =
                branchNode.GetChildContaining(parameterValue);

            if (childNode is AdaptivePath3DLeaf<T> leafNode)
                return new AdaptivePath3DSample<T>(
                    leafNode,
                    parameterValue
                );

            branchNode = (AdaptivePath3DBranch<T>)childNode;
        }
    }

    public AdaptivePath3DSample<T> GetSampleByLength(Scalar<T> length)
    {
        if (!RootNode!.ContainsLength(length))
            throw new ArgumentOutOfRangeException();

        var branchNode = RootNode;

        while (true)
        {
            var childNode =
                branchNode.GetChildContainingLength(length);

            if (childNode is AdaptivePath3DLeaf<T> leafNode)
            {
                var sp = length.ScalarProcessor;
                var numerator = sp.Subtract(length.ScalarValue, childNode.Length0.ScalarValue);
                var denominator = childNode.Length.ScalarValue;
                var t = sp.Divide(numerator.ScalarValue, denominator);

                // Manual Lerp: result = (1 - t) * a + t * b
                var oneMinusT = sp.Subtract(sp.OneValue, t.ScalarValue);
                var term1 = sp.Times(oneMinusT.ScalarValue, childNode.MinParameterValue.ScalarValue);
                var term2 = sp.Times(t.ScalarValue, childNode.MaxParameterValue.ScalarValue);
                var parameterValue = sp.Add(term1.ScalarValue, term2.ScalarValue);

                return new AdaptivePath3DSample<T>(leafNode, parameterValue);
            }

            branchNode = (AdaptivePath3DBranch<T>)childNode;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<LinVector3D<T>> GetPoints(Scalar<T> parameterValue)
    {
        return GetPoints(TimeRange.MinValue, parameterValue);
    }

    public IEnumerable<LinVector3D<T>> GetPoints(Scalar<T> parameterValue1, Scalar<T> parameterValue2)
    {
        var sp = parameterValue1.ScalarProcessor;
        if (sp.IsPositive<T>(sp.Subtract(parameterValue1.ScalarValue, parameterValue2.ScalarValue).ScalarValue))
            (parameterValue1, parameterValue2) = (parameterValue2, parameterValue1);

        var sample1 = GetSample(parameterValue1);
        var sample2 = GetSample(parameterValue2);

        yield return sample1.GetPoint();

        var index1 = sample1.LeafNodeIndex + 1;
        var index2 = sample2.LeafNodeIndex;
        for (var index = index1; index <= index2; index++)
        {
            var leafNode = _leafNodeList[index];

            yield return leafNode.Frame0.Point;
        }

        if (sp.IsPositive<T>(sp.Subtract(parameterValue2.ScalarValue, sample2.LeafNode.MinParameterValue.ScalarValue).ScalarValue))
            yield return sample2.GetPoint();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ParametricPath3DLocalFrame<T>> GetFrames(Scalar<T> parameterValue)
    {
        return GetFrames(TimeRange.MinValue, parameterValue);
    }

    public IEnumerable<ParametricPath3DLocalFrame<T>> GetFrames(Scalar<T> parameterValue1, Scalar<T> parameterValue2)
    {
        var sp = parameterValue1.ScalarProcessor;
        if (sp.IsPositive<T>(sp.Subtract(parameterValue1.ScalarValue, parameterValue2.ScalarValue).ScalarValue))
            (parameterValue1, parameterValue2) = (parameterValue2, parameterValue1);

        var sample1 = GetSample(parameterValue1);
        var sample2 = GetSample(parameterValue2);

        yield return sample1.GetFrame();

        var index1 = sample1.LeafNodeIndex + 1;
        var index2 = sample2.LeafNodeIndex;
        for (var index = index1; index <= index2; index++)
        {
            var leafNode = _leafNodeList[index];

            yield return leafNode.Frame0;
        }

        if (sp.IsPositive<T>(sp.Subtract(parameterValue2.ScalarValue, sample2.LeafNode.MinParameterValue.ScalarValue).ScalarValue))
            yield return sample2.GetFrame();
    }

    public override Scalar<T> TimeToLength(Scalar<T> parameterValue)
    {
        var sp = parameterValue.ScalarProcessor;

        // TODO: Implement ClampPeriodic for Generic<T> if periodic wrapping is needed
        // For now, rely on Contains check below

        if (!RootNode!.Contains(parameterValue))
            throw new ArgumentOutOfRangeException();

        var branchNode = RootNode;

        while (true)
        {
            var childNode =
                branchNode.GetChildContaining(parameterValue);

            if (childNode is AdaptivePath3DLeaf<T>)
            {
                var numerator = sp.Subtract(parameterValue.ScalarValue, childNode.MinParameterValue.ScalarValue);
                var denominator = sp.Subtract(childNode.MaxParameterValue.ScalarValue, childNode.MinParameterValue.ScalarValue);
                var t = sp.Divide(numerator.ScalarValue, denominator.ScalarValue);

                // Manual Lerp: result = (1 - t) * a + t * b
                var oneMinusT = sp.Subtract(sp.OneValue, t.ScalarValue);
                var term1 = sp.Times(oneMinusT.ScalarValue, childNode.Length0.ScalarValue);
                var term2 = sp.Times(t.ScalarValue, childNode.Length1.ScalarValue);
                var length = sp.Add(term1.ScalarValue, term2.ScalarValue);

                return length;
            }

            branchNode = (AdaptivePath3DBranch<T>)childNode;
        }
    }

    public override Scalar<T> LengthToTime(Scalar<T> length)
    {
        if (!RootNode!.ContainsLength(length))
            throw new ArgumentOutOfRangeException();

        var branchNode = RootNode;

        while (true)
        {
            var childNode =
                branchNode.GetChildContainingLength(length);

            if (childNode is AdaptivePath3DLeaf<T>)
            {
                var sp = length.ScalarProcessor;
                var numerator = sp.Subtract(length.ScalarValue, childNode.Length0.ScalarValue);
                var denominator = childNode.Length.ScalarValue;
                var t = sp.Divide(numerator.ScalarValue, denominator);

                // Manual Lerp: result = (1 - t) * a + t * b
                var oneMinusT = sp.Subtract(sp.OneValue, t.ScalarValue);
                var term1 = sp.Times(oneMinusT.ScalarValue, childNode.MinParameterValue.ScalarValue);
                var term2 = sp.Times(t.ScalarValue, childNode.MaxParameterValue.ScalarValue);
                var parameterValue = sp.Add(term1.ScalarValue, term2.ScalarValue);

                return parameterValue;
            }

            branchNode = (AdaptivePath3DBranch<T>)childNode;
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AdaptivePath3D<T> GetSubCurve(Scalar<T> parameterValue1, Scalar<T> parameterValue2, AdaptivePath3DSamplingOptions<T> options)
    {
        var sp = parameterValue1.ScalarProcessor;
        if (sp.IsPositive<T>(sp.Subtract(parameterValue1.ScalarValue, parameterValue2.ScalarValue).ScalarValue))
            (parameterValue1, parameterValue2) = (parameterValue2, parameterValue1);

        var curve = new AdaptivePath3D<T>(
            ScalarRange<T>.Create(parameterValue1, parameterValue2),
            false,
            this
        );

        return curve.GenerateTree(options);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AdaptivePath3D<T> GetSubCurveByLength(Scalar<T> length1, Scalar<T> length2, AdaptivePath3DSamplingOptions<T> options)
    {
        var sp = length1.ScalarProcessor;
        if (sp.IsPositive<T>(sp.Subtract(length1.ScalarValue, length2.ScalarValue).ScalarValue))
            (length1, length2) = (length2, length1);

        return GetSubCurve(
            LengthToTime(length1),
            LengthToTime(length2),
            options
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<ParametricPath3DLocalFrame<T>> GetEnumerator()
    {
        if (_leafNodeList.Count == 0)
            yield break;

        foreach (var leafNode in _leafNodeList)
            yield return leafNode.Frame0;

        yield return _leafNodeList[^1].Frame1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
