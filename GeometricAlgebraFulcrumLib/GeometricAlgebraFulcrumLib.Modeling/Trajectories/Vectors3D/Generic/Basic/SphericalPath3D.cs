using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;

/// <summary>
/// A parametric 3D path expressed in spherical coordinates (r(t), theta(t), phi(t)).
/// Converts spherical coordinates to Cartesian coordinates at each time parameter.
/// Spherical convention: theta is polar angle from Z-axis, phi is azimuthal angle from X-axis.
/// Cartesian conversion:
///   x = r * cos(theta) * cos(phi)
///   y = r * cos(theta) * sin(phi)
///   z = r * sin(theta)
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
public sealed class SphericalPath3D<T> :
    ParametricPath3D<T>
{
    /// <summary>
    /// Creates a finite spherical path with the given time range and spherical coordinate signals.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SphericalPath3D<T> Finite(ScalarRange<T> timeRange, ScalarSignal<T> rCurve, ScalarSignal<T> thetaCurve, ScalarSignal<T> phiCurve)
    {
        return new SphericalPath3D<T>(
            timeRange,
            false,
            rCurve,
            thetaCurve,
            phiCurve
        );
    }

    /// <summary>
    /// Creates a periodic spherical path with the given time range and spherical coordinate signals.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SphericalPath3D<T> Periodic(ScalarRange<T> timeRange, ScalarSignal<T> rCurve, ScalarSignal<T> thetaCurve, ScalarSignal<T> phiCurve)
    {
        return new SphericalPath3D<T>(
            timeRange,
            true,
            rCurve,
            thetaCurve,
            phiCurve
        );
    }


    /// <summary>
    /// Radial distance signal r(t)
    /// </summary>
    public ScalarSignal<T> RCurve { get; }

    /// <summary>
    /// Polar angle signal theta(t) - angle from positive Z-axis
    /// </summary>
    public ScalarSignal<T> ThetaCurve { get; }

    /// <summary>
    /// Azimuthal angle signal phi(t) - angle from positive X-axis in XY-plane
    /// </summary>
    public ScalarSignal<T> PhiCurve { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SphericalPath3D(ScalarRange<T> timeRange, bool isPeriodic, ScalarSignal<T> rCurve, ScalarSignal<T> thetaCurve, ScalarSignal<T> phiCurve)
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

    /// <summary>
    /// Converts spherical coordinates (r, theta, phi) to Cartesian coordinates (x, y, z).
    /// Formula:
    ///   x = r * cos(theta) * cos(phi)
    ///   y = r * cos(theta) * sin(phi)
    ///   z = r * sin(theta)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> parameterValue)
    {
        var r = RCurve.GetValue(parameterValue);
        var theta = ThetaCurve.GetValue(parameterValue);
        var phi = PhiCurve.GetValue(parameterValue);

        var scalarProcessor = r.ScalarProcessor;

        var thetaCos = scalarProcessor.Cos(theta.ScalarValue);
        var thetaSin = scalarProcessor.Sin(theta.ScalarValue);

        var phiCos = scalarProcessor.Cos(phi.ScalarValue);
        var phiSin = scalarProcessor.Sin(phi.ScalarValue);

        // x = r * cos(theta) * cos(phi)
        var xValue = scalarProcessor.Times(
            r.ScalarValue,
            scalarProcessor.Times(thetaCos.ScalarValue, phiCos.ScalarValue).ScalarValue
        );

        // y = r * cos(theta) * sin(phi)
        var yValue = scalarProcessor.Times(
            r.ScalarValue,
            scalarProcessor.Times(thetaCos.ScalarValue, phiSin.ScalarValue).ScalarValue
        );

        // z = r * sin(theta)
        var zValue = scalarProcessor.Times(r.ScalarValue, thetaSin.ScalarValue);

        return LinVector3D<T>.Create(xValue, yValue, zValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new SphericalPath3D<T>(
                TimeRange,
                false,
                RCurve,
                ThetaCurve,
                PhiCurve
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
                RCurve,
                ThetaCurve,
                PhiCurve
            );
    }

    /// <summary>
    /// First derivative (velocity) using product rule and chain rule on spherical coordinates.
    /// dx/dt = dr/dt * cos(theta) * cos(phi) - r * sin(theta) * dtheta/dt * cos(phi) - r * cos(theta) * sin(phi) * dphi/dt
    /// dy/dt = dr/dt * cos(theta) * sin(phi) - r * sin(theta) * dtheta/dt * sin(phi) + r * cos(theta) * cos(phi) * dphi/dt
    /// dz/dt = dr/dt * sin(theta) + r * cos(theta) * dtheta/dt
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> parameterValue)
    {
        var r = RCurve.GetValue(parameterValue);
        var theta = ThetaCurve.GetValue(parameterValue);
        var phi = PhiCurve.GetValue(parameterValue);

        var scalarProcessor = r.ScalarProcessor;

        var thetaCos = scalarProcessor.Cos(theta.ScalarValue);
        var thetaSin = scalarProcessor.Sin(theta.ScalarValue);

        var phiCos = scalarProcessor.Cos(phi.ScalarValue);
        var phiSin = scalarProcessor.Sin(phi.ScalarValue);

        var rDt1 = RCurve.GetDerivative1Value(parameterValue);
        var thetaDt1 = ThetaCurve.GetDerivative1Value(parameterValue);
        var phiDt1 = PhiCurve.GetDerivative1Value(parameterValue);

        // x = r * thetaCos * phiCos;
        // dx/dt = dr/dt * cos(theta) * cos(phi) - r * sin(theta) * dtheta/dt * cos(phi) - r * cos(theta) * sin(phi) * dphi/dt
        var term1 = scalarProcessor.Times(
            scalarProcessor.Times(rDt1.ScalarValue, thetaCos.ScalarValue).ScalarValue,
            phiCos.ScalarValue
        );

        var term2 = scalarProcessor.Times(
            scalarProcessor.Times(
                scalarProcessor.Times(r.ScalarValue, thetaSin.ScalarValue).ScalarValue,
                thetaDt1.ScalarValue
            ).ScalarValue,
            phiCos.ScalarValue
        );

        var term3 = scalarProcessor.Times(
            scalarProcessor.Times(
                scalarProcessor.Times(r.ScalarValue, thetaCos.ScalarValue).ScalarValue,
                phiSin.ScalarValue
            ).ScalarValue,
            phiDt1.ScalarValue
        );

        var x = scalarProcessor.Subtract(
            scalarProcessor.Subtract(term1.ScalarValue, term2.ScalarValue).ScalarValue,
            term3.ScalarValue
        );

        // y = r * thetaCos * phiSin;
        // dy/dt = dr/dt * cos(theta) * sin(phi) - r * sin(theta) * dtheta/dt * sin(phi) + r * cos(theta) * cos(phi) * dphi/dt
        var term4 = scalarProcessor.Times(
            scalarProcessor.Times(rDt1.ScalarValue, thetaCos.ScalarValue).ScalarValue,
            phiSin.ScalarValue
        );

        var term5 = scalarProcessor.Times(
            scalarProcessor.Times(
                scalarProcessor.Times(r.ScalarValue, thetaSin.ScalarValue).ScalarValue,
                thetaDt1.ScalarValue
            ).ScalarValue,
            phiSin.ScalarValue
        );

        var term6 = scalarProcessor.Times(
            scalarProcessor.Times(
                scalarProcessor.Times(r.ScalarValue, thetaCos.ScalarValue).ScalarValue,
                phiCos.ScalarValue
            ).ScalarValue,
            phiDt1.ScalarValue
        );

        var y = scalarProcessor.Add(
            scalarProcessor.Subtract(term4.ScalarValue, term5.ScalarValue).ScalarValue,
            term6.ScalarValue
        );

        // z = r * thetaSin;
        // dz/dt = dr/dt * sin(theta) + r * cos(theta) * dtheta/dt
        var term7 = scalarProcessor.Times(rDt1.ScalarValue, thetaSin.ScalarValue);
        var term8 = scalarProcessor.Times(
            scalarProcessor.Times(r.ScalarValue, thetaCos.ScalarValue).ScalarValue,
            thetaDt1.ScalarValue
        );

        var z = scalarProcessor.Add(term7.ScalarValue, term8.ScalarValue);

        return LinVector3D<T>.Create(x, y, z);
    }

    /// <summary>
    /// Second derivative (acceleration) computed by differentiating the first derivative formulas.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> parameterValue)
    {
        var r = RCurve.GetValue(parameterValue);
        var theta = ThetaCurve.GetValue(parameterValue);
        var phi = PhiCurve.GetValue(parameterValue);

        var scalarProcessor = r.ScalarProcessor;

        var thetaCos = scalarProcessor.Cos(theta.ScalarValue);
        var thetaSin = scalarProcessor.Sin(theta.ScalarValue);

        var phiCos = scalarProcessor.Cos(phi.ScalarValue);
        var phiSin = scalarProcessor.Sin(phi.ScalarValue);

        var rDt1 = RCurve.GetDerivative1Value(parameterValue);
        var thetaDt1 = ThetaCurve.GetDerivative1Value(parameterValue);
        var phiDt1 = PhiCurve.GetDerivative1Value(parameterValue);

        var rDt2 = RCurve.GetDerivative2Value(parameterValue);
        var thetaDt2 = ThetaCurve.GetDerivative2Value(parameterValue);
        var phiDt2 = PhiCurve.GetDerivative2Value(parameterValue);

        // Precompute commonly used products
        var two = scalarProcessor.ScalarFromNumber(2);
        var phiDt1Sq = scalarProcessor.Times(phiDt1.ScalarValue, phiDt1.ScalarValue);
        var thetaDt1Sq = scalarProcessor.Times(thetaDt1.ScalarValue, thetaDt1.ScalarValue);

        // x-component second derivative terms
        var xTerm1 = scalarProcessor.Times(
            scalarProcessor.Times(phiCos.ScalarValue, thetaCos.ScalarValue).ScalarValue,
            rDt2.ScalarValue
        );

        var xTerm2 = scalarProcessor.Times(
            scalarProcessor.Times(
                scalarProcessor.Times(phiCos.ScalarValue, thetaCos.ScalarValue).ScalarValue,
                r.ScalarValue
            ).ScalarValue,
            phiDt1Sq.ScalarValue
        );

        var xTerm3 = scalarProcessor.Times(
            two.ScalarValue,
            scalarProcessor.Times(
                scalarProcessor.Times(
                    scalarProcessor.Times(thetaCos.ScalarValue, phiSin.ScalarValue).ScalarValue,
                    phiDt1.ScalarValue
                ).ScalarValue,
                rDt1.ScalarValue
            ).ScalarValue
        );

        var xTerm4 = scalarProcessor.Times(
            two.ScalarValue,
            scalarProcessor.Times(
                scalarProcessor.Times(
                    scalarProcessor.Times(phiCos.ScalarValue, thetaSin.ScalarValue).ScalarValue,
                    rDt1.ScalarValue
                ).ScalarValue,
                thetaDt1.ScalarValue
            ).ScalarValue
        );

        var xTerm5 = scalarProcessor.Times(
            two.ScalarValue,
            scalarProcessor.Times(
                scalarProcessor.Times(
                    scalarProcessor.Times(r.ScalarValue, phiSin.ScalarValue).ScalarValue,
                    scalarProcessor.Times(thetaSin.ScalarValue, phiDt1.ScalarValue).ScalarValue
                ).ScalarValue,
                thetaDt1.ScalarValue
            ).ScalarValue
        );

        var xTerm6 = scalarProcessor.Times(
            scalarProcessor.Times(
                scalarProcessor.Times(phiCos.ScalarValue, thetaCos.ScalarValue).ScalarValue,
                r.ScalarValue
            ).ScalarValue,
            thetaDt1Sq.ScalarValue
        );

        var xTerm7 = scalarProcessor.Times(
            scalarProcessor.Times(phiCos.ScalarValue, r.ScalarValue).ScalarValue,
            scalarProcessor.Times(thetaSin.ScalarValue, thetaDt2.ScalarValue).ScalarValue
        );

        var xTerm8 = scalarProcessor.Times(
            scalarProcessor.Times(thetaCos.ScalarValue, r.ScalarValue).ScalarValue,
            scalarProcessor.Times(phiSin.ScalarValue, phiDt2.ScalarValue).ScalarValue
        );

        var x = scalarProcessor.Subtract(
            scalarProcessor.Add(
                scalarProcessor.Subtract(
                    scalarProcessor.Add(
                        scalarProcessor.Subtract(
                            scalarProcessor.Subtract(
                                scalarProcessor.Subtract(xTerm1.ScalarValue, xTerm2.ScalarValue).ScalarValue,
                                xTerm3.ScalarValue
                            ).ScalarValue,
                            xTerm4.ScalarValue
                        ).ScalarValue,
                        xTerm5.ScalarValue
                    ).ScalarValue,
                    xTerm6.ScalarValue
                ).ScalarValue,
                xTerm7.ScalarValue
            ).ScalarValue,
            xTerm8.ScalarValue
        );

        // y-component second derivative terms
        var yTerm1 = scalarProcessor.Times(
            scalarProcessor.Times(thetaCos.ScalarValue, phiSin.ScalarValue).ScalarValue,
            rDt2.ScalarValue
        );

        var yTerm2 = scalarProcessor.Times(
            scalarProcessor.Times(
                scalarProcessor.Times(thetaCos.ScalarValue, r.ScalarValue).ScalarValue,
                phiSin.ScalarValue
            ).ScalarValue,
            phiDt1Sq.ScalarValue
        );

        var yTerm3 = scalarProcessor.Times(
            two.ScalarValue,
            scalarProcessor.Times(
                scalarProcessor.Times(
                    scalarProcessor.Times(phiSin.ScalarValue, thetaSin.ScalarValue).ScalarValue,
                    rDt1.ScalarValue
                ).ScalarValue,
                thetaDt1.ScalarValue
            ).ScalarValue
        );

        var yTerm4 = scalarProcessor.Times(
            two.ScalarValue,
            scalarProcessor.Times(
                scalarProcessor.Times(
                    scalarProcessor.Times(phiCos.ScalarValue, thetaCos.ScalarValue).ScalarValue,
                    phiDt1.ScalarValue
                ).ScalarValue,
                rDt1.ScalarValue
            ).ScalarValue
        );

        var yTerm5 = scalarProcessor.Times(
            two.ScalarValue,
            scalarProcessor.Times(
                scalarProcessor.Times(
                    scalarProcessor.Times(phiCos.ScalarValue, r.ScalarValue).ScalarValue,
                    scalarProcessor.Times(thetaSin.ScalarValue, phiDt1.ScalarValue).ScalarValue
                ).ScalarValue,
                thetaDt1.ScalarValue
            ).ScalarValue
        );

        var yTerm6 = scalarProcessor.Times(
            scalarProcessor.Times(
                scalarProcessor.Times(thetaCos.ScalarValue, r.ScalarValue).ScalarValue,
                phiSin.ScalarValue
            ).ScalarValue,
            thetaDt1Sq.ScalarValue
        );

        var yTerm7 = scalarProcessor.Times(
            scalarProcessor.Times(
                scalarProcessor.Times(phiCos.ScalarValue, thetaCos.ScalarValue).ScalarValue,
                r.ScalarValue
            ).ScalarValue,
            phiDt2.ScalarValue
        );

        var yTerm8 = scalarProcessor.Times(
            scalarProcessor.Times(r.ScalarValue, phiSin.ScalarValue).ScalarValue,
            scalarProcessor.Times(thetaSin.ScalarValue, thetaDt2.ScalarValue).ScalarValue
        );

        var y = scalarProcessor.Add(
            scalarProcessor.Subtract(
                scalarProcessor.Subtract(
                    scalarProcessor.Add(
                        scalarProcessor.Subtract(
                            scalarProcessor.Subtract(
                                scalarProcessor.Subtract(yTerm1.ScalarValue, yTerm2.ScalarValue).ScalarValue,
                                yTerm3.ScalarValue
                            ).ScalarValue,
                            yTerm5.ScalarValue
                        ).ScalarValue,
                        yTerm4.ScalarValue
                    ).ScalarValue,
                    yTerm6.ScalarValue
                ).ScalarValue,
                yTerm7.ScalarValue
            ).ScalarValue,
            yTerm8.ScalarValue
        );

        // z-component second derivative terms
        var zTerm1 = scalarProcessor.Times(thetaSin.ScalarValue, rDt2.ScalarValue);

        var zTerm2 = scalarProcessor.Times(
            scalarProcessor.Times(r.ScalarValue, thetaSin.ScalarValue).ScalarValue,
            thetaDt1Sq.ScalarValue
        );

        var zTerm3 = scalarProcessor.Times(
            two.ScalarValue,
            scalarProcessor.Times(
                scalarProcessor.Times(thetaCos.ScalarValue, rDt1.ScalarValue).ScalarValue,
                thetaDt1.ScalarValue
            ).ScalarValue
        );

        var zTerm4 = scalarProcessor.Times(
            scalarProcessor.Times(thetaCos.ScalarValue, r.ScalarValue).ScalarValue,
            thetaDt2.ScalarValue
        );

        var z = scalarProcessor.Add(
            scalarProcessor.Add(
                scalarProcessor.Subtract(zTerm1.ScalarValue, zTerm2.ScalarValue).ScalarValue,
                zTerm3.ScalarValue
            ).ScalarValue,
            zTerm4.ScalarValue
        );

        return LinVector3D<T>.Create(x, y, z);
    }
}
