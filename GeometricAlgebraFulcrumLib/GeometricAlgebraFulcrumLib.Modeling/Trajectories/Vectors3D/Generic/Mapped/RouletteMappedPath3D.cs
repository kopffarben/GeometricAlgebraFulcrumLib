using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.AffineMaps.Space3D;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;

/// <summary>
/// Generic equivalent of <c>Float64RouletteMappedPath3D</c>. Applies a RouletteAffineMap3D&lt;T&gt; to an arc-length path.
/// </summary>
public sealed class RouletteMappedPath3D<T> :
    ArcLengthPath3D<T>
{
    public ArcLengthPath3D<T> BaseCurve { get; }

    public RouletteAffineMap3D<T> RouletteMap { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RouletteMappedPath3D(
        ArcLengthPath3D<T> baseCurve,
        RouletteAffineMap3D<T> rouletteMap)
        : base(baseCurve.TimeRange, baseCurve.IsPeriodic)
    {
        BaseCurve = baseCurve;
        RouletteMap = rouletteMap;

        Debug.Assert(IsValid());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BaseCurve.IsValid() &&
               RouletteMap.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        return RouletteMap.MapPoint(BaseCurve.GetValue(t));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        return RouletteMap.MapVector(BaseCurve.GetDerivative1Value(t));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        return RouletteMap.MapVector(BaseCurve.GetDerivative2Value(t));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        var frame = BaseCurve.GetFrame(t);

        var point = RouletteMap.MapPoint(frame.Point);
        var (tangent, normal1, normal2) =
            RouletteMap.RotationQuaternion.RotateVectors(
                frame.Tangent,
                frame.Normal1,
                frame.Normal2
            );

        return ParametricPath3DLocalFrame<T>.Create(
            t,
            point,
            tangent,
            normal1,
            normal2
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetLength()
    {
        return BaseCurve.GetLength();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> TimeToLength(Scalar<T> t)
    {
        return BaseCurve.TimeToLength(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> LengthToTime(Scalar<T> length)
    {
        return BaseCurve.LengthToTime(length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath3D<T> ToFiniteArcLengthPath()
    {
        return IsFinite
            ? this
            : new RouletteMappedPath3D<T>(
                BaseCurve.ToFiniteArcLengthPath(),
                RouletteMap
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath3D<T> ToPeriodicArcLengthPath()
    {
        return IsPeriodic
            ? this
            : new RouletteMappedPath3D<T>(
                BaseCurve.ToPeriodicArcLengthPath(),
                RouletteMap
            );
    }
}
