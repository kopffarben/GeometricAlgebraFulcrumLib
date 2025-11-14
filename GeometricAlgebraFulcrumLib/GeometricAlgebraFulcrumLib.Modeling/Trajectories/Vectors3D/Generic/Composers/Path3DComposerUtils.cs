using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Adaptive;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Composers;

public static class Path3DComposerUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> CreateAdaptiveCurve3D<T>(this ParametricPath3D<T> curve, AdaptivePath3DSamplingOptions<T> options)
    {
        return AdaptivePath3D<T>
            .Create(curve)
            .GenerateTree(options);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> CreateAdaptiveCurve3D<T>(this ParametricPath3D<T> curve, ScalarRange<T> timeRange, AdaptivePath3DSamplingOptions<T> options)
    {
        return AdaptivePath3D<T>
            .Create(timeRange, curve)
            .GenerateTree(options);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> CreateAdaptiveCurve3D<T>(this ParametricPath3D<T> curve, bool isPeriodic, AdaptivePath3DSamplingOptions<T> options)
    {
        return AdaptivePath3D<T>
            .Create(isPeriodic, curve)
            .GenerateTree(options);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> CreateAdaptiveCurve3D<T>(this ParametricPath3D<T> curve, ScalarRange<T> timeRange, bool isPeriodic, AdaptivePath3DSamplingOptions<T> options)
    {
        return AdaptivePath3D<T>
            .Create(timeRange, isPeriodic, curve)
            .GenerateTree(options);
    }

    /// <summary>
    /// Lift a 2D parametric curve into 3D using a custom mapping.
    /// </summary>
    public static ComputedPath3D<T> ToParametricCurve3D<T>(
        this ParametricPath2D<T> curve,
        Func<LinVector2D<T>, LinVector3D<T>> vectorMapping)
    {
        return ComputedPath3D<T>.Create(
            curve.TimeRange,
            curve.IsPeriodic,
            t => vectorMapping(curve.GetValue(t))
        );
    }

    /// <summary>
    /// Embed a 2D curve inside the XY-plane (Z=0).
    /// </summary>
    public static ComputedPath3D<T> ToXyParametricCurve3D<T>(this ParametricPath2D<T> curve)
    {
        return ComputedPath3D<T>.Create(
            curve.TimeRange,
            curve.IsPeriodic,
            t =>
            {
                var point2D = curve.GetValue(t);
                var zero = point2D.ScalarProcessor.Zero;

                return new LinVector3D<T>(
                    new Triplet<Scalar<T>>(point2D.X, point2D.Y, zero)
                );
            }
        );
    }

    /// <summary>
    /// Apply a vector mapping to a 3D curve (useful for composing transformations).
    /// </summary>
    public static ComputedPath3D<T> MapCurve3D<T>(
        this ParametricPath3D<T> curve,
        Func<LinVector3D<T>, LinVector3D<T>> vectorMapping)
    {
        return ComputedPath3D<T>.Create(
            curve.TimeRange,
            curve.IsPeriodic,
            t => vectorMapping(curve.GetValue(t))
        );
    }

    /// <summary>
    /// Create a roulette path where a moving curve rolls over a fixed curve.
    /// </summary>
    public static RoulettePath3D<T> CreateRouletteCurve3D<T>(
        this ArcLengthPath3D<T> fixedCurve,
        ArcLengthPath3D<T> movingCurve,
        Scalar<T>? parameterValueMax = null,
        bool isPeriodic = false,
        LinVector3D<T>? generatorPoint = null)
    {
        return RoulettePath3D<T>.Create(
            fixedCurve,
            movingCurve,
            parameterValueMax,
            isPeriodic,
            generatorPoint
        );
    }
}
