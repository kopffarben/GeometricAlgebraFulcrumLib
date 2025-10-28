using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;

/// <summary>
/// A 3D parametric path composed of three harmonic (sinusoidal) scalar signals
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
public sealed class HarmonicPath3D<T> :
    ParametricPath3D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HarmonicPath3D<T> Create(
        HarmonicScalarSignal<T> xCurve,
        HarmonicScalarSignal<T> yCurve,
        HarmonicScalarSignal<T> zCurve)
    {
        return new HarmonicPath3D<T>(xCurve, yCurve, zCurve);
    }


    public HarmonicScalarSignal<T> XCurve { get; }

    public HarmonicScalarSignal<T> YCurve { get; }

    public HarmonicScalarSignal<T> ZCurve { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private HarmonicPath3D(
        HarmonicScalarSignal<T> xCurve,
        HarmonicScalarSignal<T> yCurve,
        HarmonicScalarSignal<T> zCurve)
        : base(xCurve.TimeRange, xCurve.IsPeriodic)
    {
        XCurve = xCurve;
        YCurve = yCurve;
        ZCurve = zCurve;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return XCurve.IsValid() &&
               YCurve.IsValid() &&
               ZCurve.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        return LinVector3D<T>.Create(
            XCurve.GetValue(t),
            YCurve.GetValue(t),
            ZCurve.GetValue(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        return LinVector3D<T>.Create(
            XCurve.GetDerivative1Value(t),
            YCurve.GetDerivative1Value(t),
            ZCurve.GetDerivative1Value(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        return LinVector3D<T>.Create(
            XCurve.GetDerivative2Value(t),
            YCurve.GetDerivative2Value(t),
            ZCurve.GetDerivative2Value(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new HarmonicPath3D<T>(
                (HarmonicScalarSignal<T>)XCurve.ToFiniteSignal(),
                (HarmonicScalarSignal<T>)YCurve.ToFiniteSignal(),
                (HarmonicScalarSignal<T>)ZCurve.ToFiniteSignal()
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new HarmonicPath3D<T>(
                (HarmonicScalarSignal<T>)XCurve.ToPeriodicSignal(),
                (HarmonicScalarSignal<T>)YCurve.ToPeriodicSignal(),
                (HarmonicScalarSignal<T>)ZCurve.ToPeriodicSignal()
            );
    }
}
