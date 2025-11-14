using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Angles;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;

public sealed class RotatedNormalsArcLengthPath3D<T> :
    ArcLengthPath3D<T>
{
    public ArcLengthPath3D<T> BaseCurve { get; }

    public LinPolarAngleTimeSignal<T> AngleFunction { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RotatedNormalsArcLengthPath3D(
        ArcLengthPath3D<T> baseCurve,
        LinPolarAngleTimeSignal<T> angleFunction)
        : base(baseCurve.TimeRange, baseCurve.IsPeriodic)
    {
        BaseCurve = baseCurve;
        AngleFunction = angleFunction;

        Debug.Assert(IsValid());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BaseCurve.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath3D<T> ToFiniteArcLengthPath()
    {
        return IsFinite
            ? this
            : new RotatedNormalsArcLengthPath3D<T>(
                BaseCurve.ToFiniteArcLengthPath(),
                AngleFunction
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ArcLengthPath3D<T> ToPeriodicArcLengthPath()
    {
        return IsPeriodic
            ? this
            : new RotatedNormalsArcLengthPath3D<T>(
                BaseCurve.ToPeriodicArcLengthPath(),
                AngleFunction
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        return BaseCurve.GetValue(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        return BaseCurve.GetDerivative1Value(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        return BaseCurve.GetDerivative2Value(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        return BaseCurve
            .GetFrame(t)
            .RotateNormalsBy(AngleFunction.GetAngle(t));
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
}
