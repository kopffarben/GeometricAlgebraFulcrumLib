using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;

/// <summary>
/// A 3D parametric path composed of three harmonic (sinusoidal) scalar signals,
/// one for each coordinate (X, Y, Z).
/// Perfect for creating Lissajous figures and complex harmonic motion patterns.
/// Formula: (X, Y, Z) = (MagX*cos(FreqX*t), MagY*cos(FreqY*t), MagZ*cos(FreqZ*t))
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
public sealed class HarmonicPath3D<T> :
    ParametricPath3D<T>
{
    /// <summary>
    /// Creates a harmonic path from three harmonic scalar signals.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HarmonicPath3D<T> Create(
        HarmonicScalarSignal<T> xCurve,
        HarmonicScalarSignal<T> yCurve,
        HarmonicScalarSignal<T> zCurve)
    {
        return new HarmonicPath3D<T>(xCurve, yCurve, zCurve);
    }


    /// <summary>
    /// Harmonic signal for X component
    /// </summary>
    public HarmonicScalarSignal<T> XCurve { get; }

    /// <summary>
    /// Harmonic signal for Y component
    /// </summary>
    public HarmonicScalarSignal<T> YCurve { get; }

    /// <summary>
    /// Harmonic signal for Z component
    /// </summary>
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

    /// <summary>
    /// Gets the 3D point at parameter t by evaluating each harmonic component.
    /// Result: (XCurve(t), YCurve(t), ZCurve(t))
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Algebra.Scalars.Generic.Scalar<T> t)
    {
        return LinVector3D<T>.Create(
            XCurve.GetValue(t),
            YCurve.GetValue(t),
            ZCurve.GetValue(t)
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

    /// <summary>
    /// Gets the first derivative (velocity) at parameter t.
    /// Result: (XCurve'(t), YCurve'(t), ZCurve'(t))
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Algebra.Scalars.Generic.Scalar<T> t)
    {
        return LinVector3D<T>.Create(
            XCurve.GetDerivative1Value(t),
            YCurve.GetDerivative1Value(t),
            ZCurve.GetDerivative1Value(t)
        );
    }

    /// <summary>
    /// Gets the second derivative (acceleration) at parameter t.
    /// Result: (XCurve''(t), YCurve''(t), ZCurve''(t))
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Algebra.Scalars.Generic.Scalar<T> t)
    {
        return LinVector3D<T>.Create(
            XCurve.GetDerivative2Value(t),
            YCurve.GetDerivative2Value(t),
            ZCurve.GetDerivative2Value(t)
        );
    }
}
