using System.Runtime.CompilerServices;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Adaptive;

/// <summary>
/// Represents a corner (control point) in an adaptive sampling tree.
/// Corners are shared between adjacent tree nodes.
/// </summary>
/// <typeparam name="T">The scalar type (double, float, etc.)</typeparam>
public sealed record AdaptivePath3DCorner<T>
{
    public AdaptivePath3D<T> ParentTree { get; }

    public AdaptivePath3DCornerPosition Position { get; }

    public int Index { get; }

    public ParametricPath3DLocalFrame<T> Frame { get; }

    public int GridIndex
        => Position.GetGridIndex(ParentTree.TreeLevelCount);

    public double InterpolationValue
        => Position.GetInterpolationValue();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal AdaptivePath3DCorner(AdaptivePath3D<T> parentTree, int index, ParametricPath3DLocalFrame<T> frame, AdaptivePath3DCornerPosition position)
    {
        ParentTree = parentTree;
        Index = index;
        Frame = frame;
        Position = position;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
        return $"({GridIndex})";
    }
}
