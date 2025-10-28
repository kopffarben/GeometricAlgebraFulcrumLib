using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;

/// <summary>
/// A parametric 3D path expressed in spherical coordinates (r(t), theta(t), phi(t))
/// where:
/// - r is the radial distance
/// - theta is the polar angle (angle from z-axis)
/// - phi is the azimuthal angle (angle in xy-plane from x-axis)
///
/// Cartesian conversion:
/// - x = r * cos(theta) * cos(phi)
/// - y = r * cos(theta) * sin(phi)
/// - z = r * sin(theta)
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
public sealed class SphericalPath3D<T> :
    ParametricPath3D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SphericalPath3D<T> Finite(
        ScalarRange<T> timeRange,
        ScalarSignal<T> rCurve,
        ScalarSignal<T> thetaCurve,
        ScalarSignal<T> phiCurve)
    {
        return new SphericalPath3D<T>(
            timeRange,
            false,
            rCurve,
            thetaCurve,
            phiCurve
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SphericalPath3D<T> Periodic(
        ScalarRange<T> timeRange,
        ScalarSignal<T> rCurve,
        ScalarSignal<T> thetaCurve,
        ScalarSignal<T> phiCurve)
    {
        return new SphericalPath3D<T>(
            timeRange,
            true,
            rCurve,
            thetaCurve,
            phiCurve
        );
    }


    public ScalarSignal<T> RCurve { get; }

    public ScalarSignal<T> ThetaCurve { get; }

    public ScalarSignal<T> PhiCurve { get; }

    private IScalarProcessor<T> ScalarProcessor
        => TimeRange.MinValue.ScalarProcessor;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SphericalPath3D(
        ScalarRange<T> timeRange,
        bool isPeriodic,
        ScalarSignal<T> rCurve,
        ScalarSignal<T> thetaCurve,
        ScalarSignal<T> phiCurve)
        : base(timeRange, isPeriodic)
    {
        RCurve = rCurve;
        ThetaCurve = thetaCurve;
        PhiCurve = phiCurve;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return TimeRange.IsValid() &&
               RCurve.IsValid() &&
               ThetaCurve.IsValid() &&
               PhiCurve.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        var r = RCurve.GetValue(t);
        var theta = ThetaCurve.GetValue(t);
        var phi = PhiCurve.GetValue(t);

        var thetaCos = ScalarProcessor.Cos(theta.ScalarValue);
        var thetaSin = ScalarProcessor.Sin(theta.ScalarValue);

        var phiCos = ScalarProcessor.Cos(phi.ScalarValue);
        var phiSin = ScalarProcessor.Sin(phi.ScalarValue);

        var x = r * thetaCos * phiCos;
        var y = r * thetaCos * phiSin;
        var z = r * thetaSin;

        return LinVector3D<T>.Create(x, y, z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        var r = RCurve.GetValue(t);
        var theta = ThetaCurve.GetValue(t);
        var phi = PhiCurve.GetValue(t);

        var thetaCos = ScalarProcessor.Cos(theta.ScalarValue);
        var thetaSin = ScalarProcessor.Sin(theta.ScalarValue);

        var phiCos = ScalarProcessor.Cos(phi.ScalarValue);
        var phiSin = ScalarProcessor.Sin(phi.ScalarValue);

        var rDt1 = RCurve.GetDerivative1Value(t);
        var thetaDt1 = ThetaCurve.GetDerivative1Value(t);
        var phiDt1 = PhiCurve.GetDerivative1Value(t);

        // x = r * thetaCos * phiCos;
        // dx/dt = dr/dt * cos(theta) * cos(phi) - r * sin(theta) * dtheta/dt * cos(phi) - r * cos(theta) * sin(phi) * dphi/dt
        var x =
            rDt1 * thetaCos * phiCos -
            r * thetaSin * thetaDt1 * phiCos -
            r * thetaCos * phiSin * phiDt1;

        // y = r * thetaCos * phiSin;
        // dy/dt = dr/dt * cos(theta) * sin(phi) - r * sin(theta) * dtheta/dt * sin(phi) + r * cos(theta) * cos(phi) * dphi/dt
        var y =
            rDt1 * thetaCos * phiSin -
            r * thetaSin * thetaDt1 * phiSin +
            r * thetaCos * phiCos * phiDt1;

        // z = r * thetaSin;
        // dz/dt = dr/dt * sin(theta) + r * cos(theta) * dtheta/dt
        var z =
            rDt1 * thetaSin +
            r * thetaCos * thetaDt1;

        return LinVector3D<T>.Create(x, y, z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        var r = RCurve.GetValue(t);
        var theta = ThetaCurve.GetValue(t);
        var phi = PhiCurve.GetValue(t);

        var thetaCos = ScalarProcessor.Cos(theta.ScalarValue);
        var thetaSin = ScalarProcessor.Sin(theta.ScalarValue);

        var phiCos = ScalarProcessor.Cos(phi.ScalarValue);
        var phiSin = ScalarProcessor.Sin(phi.ScalarValue);

        var rDt1 = RCurve.GetDerivative1Value(t);
        var thetaDt1 = ThetaCurve.GetDerivative1Value(t);
        var phiDt1 = PhiCurve.GetDerivative1Value(t);

        var rDt2 = RCurve.GetDerivative2Value(t);
        var thetaDt2 = ThetaCurve.GetDerivative2Value(t);
        var phiDt2 = PhiCurve.GetDerivative2Value(t);

        // Second derivative formulas (from product rule and chain rule)
        var x =
            ScalarProcessor.Negative((phiCos * thetaCos * r * phiDt1 * phiDt1).ScalarValue) -
            ScalarProcessor.Times(ScalarProcessor.TwoValue, (thetaCos * phiSin * phiDt1 * rDt1).ScalarValue) +
            ScalarProcessor.Times(ScalarProcessor.TwoValue, (r * phiSin * thetaSin * phiDt1 * thetaDt1).ScalarValue) -
            ScalarProcessor.Times(ScalarProcessor.TwoValue, (phiCos * thetaSin * rDt1 * thetaDt1).ScalarValue) -
            phiCos * thetaCos * r * thetaDt1 * thetaDt1 -
            thetaCos * r * phiSin * phiDt2 +
            phiCos * thetaCos * rDt2 -
            phiCos * r * thetaSin * thetaDt2;

        var y =
            ScalarProcessor.Negative((thetaCos * r * phiSin * phiDt1 * phiDt1).ScalarValue) +
            ScalarProcessor.Times(ScalarProcessor.TwoValue, (phiCos * thetaCos * phiDt1 * rDt1).ScalarValue) -
            ScalarProcessor.Times(ScalarProcessor.TwoValue, (phiCos * r * thetaSin * phiDt1 * thetaDt1).ScalarValue) -
            ScalarProcessor.Times(ScalarProcessor.TwoValue, (phiSin * thetaSin * rDt1 * thetaDt1).ScalarValue) -
            thetaCos * r * phiSin * thetaDt1 * thetaDt1 +
            phiCos * thetaCos * r * phiDt2 +
            thetaCos * phiSin * rDt2 -
            r * phiSin * thetaSin * thetaDt2;

        var z =
            ScalarProcessor.Times(ScalarProcessor.TwoValue, (thetaCos * rDt1 * thetaDt1).ScalarValue) -
            r * thetaSin * thetaDt1 * thetaDt1 +
            thetaSin * rDt2 +
            thetaCos * r * thetaDt2;

        return LinVector3D<T>.Create(x, y, z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new SphericalPath3D<T>(
                TimeRange,
                false,
                RCurve.ToFiniteSignal(),
                ThetaCurve.ToFiniteSignal(),
                PhiCurve.ToFiniteSignal()
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new SphericalPath3D<T>(
                TimeRange,
                true,
                RCurve.ToPeriodicSignal(),
                ThetaCurve.ToPeriodicSignal(),
                PhiCurve.ToPeriodicSignal()
            );
    }
}
