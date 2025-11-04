using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;

/// <summary>
/// A 2D parametric path composed of two harmonic (sinusoidal) signals for X and Y components
/// Each component follows: Magnitude * Cos(2π * FrequencyHz * (t + TimeOffset))
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class HarmonicPath2D<T> :
    ParametricPath2D<T>,
    IPair<HarmonicScalarSignal<T>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HarmonicPath2D<T> Create(HarmonicScalarSignal<T> xCurve, HarmonicScalarSignal<T> yCurve)
    {
        return new HarmonicPath2D<T>(xCurve, yCurve);
    }


    public HarmonicScalarSignal<T> Item1
        => XCurve;

    public HarmonicScalarSignal<T> Item2
        => YCurve;

    public HarmonicScalarSignal<T> XCurve { get; }

    public HarmonicScalarSignal<T> YCurve { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private HarmonicPath2D(HarmonicScalarSignal<T> xCurve, HarmonicScalarSignal<T> yCurve)
        : base(xCurve.TimeRange, xCurve.IsPeriodic)
    {
        XCurve = xCurve;
        YCurve = yCurve;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return XCurve.IsValid() &&
               YCurve.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new HarmonicPath2D<T>(
                (HarmonicScalarSignal<T>)XCurve.ToFiniteSignal(),
                (HarmonicScalarSignal<T>)YCurve.ToFiniteSignal()
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new HarmonicPath2D<T>(
                (HarmonicScalarSignal<T>)XCurve.ToPeriodicSignal(),
                (HarmonicScalarSignal<T>)YCurve.ToPeriodicSignal()
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetValue(Scalar<T> t)
    {
        return LinVector2D<T>.Create(
            XCurve.GetValue(t),
            YCurve.GetValue(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative1Value(Scalar<T> t)
    {
        return LinVector2D<T>.Create(
            XCurve.GetDerivative1Value(t),
            YCurve.GetDerivative1Value(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative2Value(Scalar<T> t)
    {
        return LinVector2D<T>.Create(
            XCurve.GetDerivative2Value(t),
            YCurve.GetDerivative2Value(t)
        );
    }

    /// <summary>
    /// Override GetScalarComponents to return the harmonic signals directly
    /// instead of creating computed signals that wrap the path methods.
    /// This is more efficient for HarmonicPath2D.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Pair<ScalarSignal<T>> GetScalarComponents()
    {
        return new Pair<ScalarSignal<T>>(XCurve, YCurve);
    }
}
