using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Angles;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;

public sealed class RotatedNormalsPath3D<T> :
    ParametricPath3D<T>
{
    public ParametricPath3D<T> BaseCurve { get; }

    public LinPolarAngleTimeSignal<T> AngleFunction { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RotatedNormalsPath3D(
        ParametricPath3D<T> baseCurve,
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
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new RotatedNormalsPath3D<T>(
                BaseCurve.ToFinitePath(),
                AngleFunction
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new RotatedNormalsPath3D<T>(
                BaseCurve.ToPeriodicPath(),
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
}
