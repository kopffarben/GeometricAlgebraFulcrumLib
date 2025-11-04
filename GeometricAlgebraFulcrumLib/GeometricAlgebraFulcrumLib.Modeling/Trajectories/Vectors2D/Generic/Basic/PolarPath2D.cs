using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;

/// <summary>
/// A parametric curve expressed in polar coordinates (r(t), theta(t))
/// Converts to Cartesian: x = r * cos(theta), y = r * sin(theta)
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class PolarPath2D<T> :
    ParametricPath2D<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PolarPath2D<T> Create(ScalarRange<T> timeRange, bool isPeriodic, ScalarSignal<T> rPath, ScalarSignal<T> thetaPath)
    {
        return new PolarPath2D<T>(timeRange, isPeriodic, rPath, thetaPath);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PolarPath2D<T> Finite(ScalarRange<T> timeRange, ScalarSignal<T> rPath, ScalarSignal<T> thetaPath)
    {
        return new PolarPath2D<T>(timeRange, false, rPath, thetaPath);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PolarPath2D<T> Periodic(ScalarRange<T> timeRange, ScalarSignal<T> rPath, ScalarSignal<T> thetaPath)
    {
        return new PolarPath2D<T>(timeRange, true, rPath, thetaPath);
    }


    public ScalarSignal<T> RPath { get; }

    public ScalarSignal<T> ThetaPath { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private PolarPath2D(ScalarRange<T> timeRange, bool isPeriodic, ScalarSignal<T> rPath, ScalarSignal<T> thetaPath)
        : base(timeRange, isPeriodic)
    {
        RPath = rPath;
        ThetaPath = thetaPath;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return RPath.IsValid() &&
               ThetaPath.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new PolarPath2D<T>(
                TimeRange,
                false,
                RPath.ToFiniteSignal(),
                ThetaPath.ToFiniteSignal()
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new PolarPath2D<T>(
                TimeRange,
                true,
                RPath.ToPeriodicSignal(),
                ThetaPath.ToPeriodicSignal()
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetValue(Scalar<T> t)
    {
        var r = RPath.GetValue(t);
        var theta = ThetaPath.GetValue(t);

        // Convert polar (r, theta) to Cartesian (x, y)
        // x = r * cos(theta), y = r * sin(theta)
        var scalarProcessor = r.ScalarProcessor;
        var angle = scalarProcessor.CreatePolarAngleFromRadians(theta.ScalarValue);

        var polarVec = new LinPolarVector2D<T>(r, angle);

        return LinVector2D<T>.Create(polarVec.X, polarVec.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative1Value(Scalar<T> t)
    {
        var r = RPath.GetValue(t);
        var theta = ThetaPath.GetValue(t);

        var scalarProcessor = r.ScalarProcessor;

        var thetaCos = scalarProcessor.Cos(theta.ScalarValue);
        var thetaSin = scalarProcessor.Sin(theta.ScalarValue);

        var rDt1 = RPath.GetDerivative1Value(t);
        var thetaDt1 = ThetaPath.GetDerivative1Value(t);

        // x = r * cos(theta)
        // dx/dt = dr/dt * cos(theta) - r * sin(theta) * dtheta/dt
        var xDt1 = rDt1 * thetaCos - r * thetaSin * thetaDt1;

        // y = r * sin(theta)
        // dy/dt = dr/dt * sin(theta) + r * cos(theta) * dtheta/dt
        var yDt1 = rDt1 * thetaSin + r * thetaCos * thetaDt1;

        return LinVector2D<T>.Create(xDt1, yDt1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative2Value(Scalar<T> t)
    {
        var r = RPath.GetValue(t);
        var theta = ThetaPath.GetValue(t);

        var scalarProcessor = r.ScalarProcessor;

        var thetaCos = scalarProcessor.Cos(theta.ScalarValue);
        var thetaSin = scalarProcessor.Sin(theta.ScalarValue);

        var rDt1 = RPath.GetDerivative1Value(t);
        var thetaDt1 = ThetaPath.GetDerivative1Value(t);

        var rDt2 = RPath.GetDerivative2Value(t);
        var thetaDt2 = ThetaPath.GetDerivative2Value(t);

        // xDt1 = rDt1 * cos(theta) - r * sin(theta) * thetaDt1
        // d²x/dt² = d/dt[rDt1 * cos(theta) - r * sin(theta) * thetaDt1]
        //         = rDt2 * cos(theta) - rDt1 * sin(theta) * thetaDt1
        //           - rDt1 * sin(theta) * thetaDt1 - r * cos(theta) * thetaDt1²
        //           - r * sin(theta) * thetaDt2
        var xDt2 =
            rDt2 * thetaCos -
            rDt1 * thetaSin * thetaDt1 -
            rDt1 * thetaSin * thetaDt1 -
            r * thetaCos * thetaDt1 * thetaDt1 -
            r * thetaSin * thetaDt2;

        // yDt1 = rDt1 * sin(theta) + r * cos(theta) * thetaDt1
        // d²y/dt² = d/dt[rDt1 * sin(theta) + r * cos(theta) * thetaDt1]
        //         = rDt2 * sin(theta) + rDt1 * cos(theta) * thetaDt1
        //           + rDt1 * cos(theta) * thetaDt1 - r * sin(theta) * thetaDt1²
        //           + r * cos(theta) * thetaDt2
        var yDt2 =
            rDt2 * thetaSin +
            rDt1 * thetaCos * thetaDt1 +
            rDt1 * thetaCos * thetaDt1 -
            r * thetaSin * thetaDt1 * thetaDt1 +
            r * thetaCos * thetaDt2;

        return LinVector2D<T>.Create(xDt2, yDt2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2DLocalFrame<T> GetFrame(Scalar<T> t)
    {
        // Get tangent vector from first derivative
        var tangent = GetDerivative1Value(t);
        var tangentNorm = tangent.Norm();

        var normalizedTangent = tangent.ScalarProcessor.IsZero(tangentNorm.ScalarValue)
            ? LinVector2D<T>.UnitSymmetric(tangent.ScalarProcessor)
            : tangent / tangentNorm;

        return ParametricPath2DLocalFrame<T>.Create(
            t,
            GetValue(t),
            normalizedTangent
        );
    }
}
