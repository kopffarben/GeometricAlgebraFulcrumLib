using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Basis;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Adaptive;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Bezier;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Circles;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.Parametric.Float64.Space4D.Curves.CatmullRom;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.Parametric.Float64;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Composers;

public static class Path3DComposerUtils
{
    private static ComputedPath3D<T> CreateComputedPath<T>(
        ParametricPath3D<T> curve,
        Func<Scalar<T>, LinVector3D<T>> getValueFunc,
        Func<Scalar<T>, LinVector3D<T>>? getDerivative1ValueFunc = null,
        Func<Scalar<T>, LinVector3D<T>>? getDerivative2ValueFunc = null)
    {
        return ComputedPath3D<T>.Create(
            curve.TimeRange,
            curve.IsPeriodic,
            getValueFunc,
            getDerivative1ValueFunc ?? curve.GetDerivative1Value,
            getDerivative2ValueFunc ?? curve.GetDerivative2Value
        );
    }

    private static ComputedScalarSignal<T> CreateScalarSignal<T>(
        ScalarRange<T> timeRange,
        bool isPeriodic,
        Func<Scalar<T>, Scalar<T>> getValueFunc)
    {
        return isPeriodic
            ? ComputedScalarSignal<T>.Periodic(timeRange, getValueFunc)
            : ComputedScalarSignal<T>.Finite(timeRange, getValueFunc);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> ToParametricCurve3D<T>(
        this ScalarSignal<T> curve,
        Func<Scalar<T>, LinVector3D<T>> vectorMapping)
    {
        return ComputedPath3D<T>.Create(
            curve.TimeRange,
            curve.IsPeriodic,
            t => vectorMapping(curve.GetValue(t))
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> ToParametricCurve3D<T>(
        this ParametricPath3D<T> curve,
        Func<LinVector3D<T>, LinVector3D<T>> vectorMapping)
    {
        return curve.MapCurve3D(vectorMapping);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArcLengthPath3D<T> ToArcLengthPointPath3D<T>(this ParametricPath3D<T> curve)
    {
        return curve as ArcLengthPath3D<T>
               ?? AdaptiveArcLengthPath3D<T>.Create(curve);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArcLengthPath3D<T> ToArcLengthPointPath3D<T>(this ParametricPath3D<T> curve, AdaptivePath3DSamplingOptions<T> options)
    {
        return curve as ArcLengthPath3D<T>
               ?? AdaptiveArcLengthPath3D<T>.Create(curve, options);
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> FiniteAdaptiveCurve3D<T>(this ParametricPath3D<T> curve, AdaptivePath3DSamplingOptions<T> options)
    {
        return AdaptivePath3D<T>
            .Finite(curve)
            .GenerateTree(options);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> FiniteAdaptiveCurve3D<T>(this ParametricPath3D<T> curve, ScalarRange<T> timeRange, AdaptivePath3DSamplingOptions<T> options)
    {
        return AdaptivePath3D<T>
            .Finite(timeRange, curve)
            .GenerateTree(options);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> PeriodicAdaptiveCurve3D<T>(this ParametricPath3D<T> curve, AdaptivePath3DSamplingOptions<T> options)
    {
        return AdaptivePath3D<T>
            .Periodic(curve)
            .GenerateTree(options);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdaptivePath3D<T> PeriodicAdaptiveCurve3D<T>(this ParametricPath3D<T> curve, ScalarRange<T> timeRange, AdaptivePath3DSamplingOptions<T> options)
    {
        return AdaptivePath3D<T>
            .Periodic(timeRange, curve)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bezier0Path3D<T> CreateBezier3D<T>(ILinVector3D<T> point1)
    {
        var scalarProcessor = point1.X.ScalarProcessor;
        return Bezier0Path3D<T>.Create(scalarProcessor, false, point1.ToVector3D());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bezier1Path3D<T> CreateBezier3D<T>(ILinVector3D<T> point1, ILinVector3D<T> point2)
    {
        var scalarProcessor = point1.X.ScalarProcessor;
        return Bezier1Path3D<T>.Create(scalarProcessor, false, point1.ToVector3D(), point2.ToVector3D());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bezier2Path3D<T> CreateBezier3D<T>(ILinVector3D<T> point1, ILinVector3D<T> point2, ILinVector3D<T> point3)
    {
        var scalarProcessor = point1.X.ScalarProcessor;
        return Bezier2Path3D<T>.Create(
            scalarProcessor,
            false,
            point1.ToVector3D(),
            point2.ToVector3D(),
            point3.ToVector3D()
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bezier3Path3D<T> CreateBezier3D<T>(ILinVector3D<T> point1, ILinVector3D<T> point2, ILinVector3D<T> point3, ILinVector3D<T> point4)
    {
        var scalarProcessor = point1.X.ScalarProcessor;
        return Bezier3Path3D<T>.Create(
            scalarProcessor,
            false,
            point1.ToVector3D(),
            point2.ToVector3D(),
            point3.ToVector3D(),
            point4.ToVector3D()
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CatmullRomSplinePath3D<T> CreateCatmullRomSpline3D<T>(
        this IEnumerable<ILinVector3D<T>> pointList,
        CatmullRomSplineType curveType,
        bool isClosed)
    {
        var points = pointList.Select(p => p.ToVector3D()).ToList();
        if (points.Count < 2)
            throw new ArgumentException("At least two control points are required", nameof(pointList));

        var scalarProcessor = points[0].ScalarProcessor;
        return new CatmullRomSplinePath3D<T>(false, points, curveType, isClosed, scalarProcessor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AxisAlignedCirclePath3D<T> CreateCircle3D<T>(
        this LinBasisVector normalAxis,
        IScalarProcessor<T> scalarProcessor,
        T radius,
        int rotationCount = 1)
    {
        return (normalAxis.Index, normalAxis.IsNegative) switch
        {
            (0, false) => YzCirclePath3D<T>.Create(scalarProcessor, radius, rotationCount),
            (0, true) => YzCirclePath3D<T>.Create(scalarProcessor, radius, -rotationCount),
            (1, false) => ZxCirclePath3D<T>.Create(scalarProcessor, radius, rotationCount),
            (1, true) => ZxCirclePath3D<T>.Create(scalarProcessor, radius, -rotationCount),
            (2, false) => XyCirclePath3D<T>.Create(scalarProcessor, radius, rotationCount),
            (2, true) => XyCirclePath3D<T>.Create(scalarProcessor, radius, -rotationCount),
            _ => throw new ArgumentOutOfRangeException(nameof(normalAxis))
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CirclePath3D<T> CreateCircle3D<T>(this ILinVector3D<T> unitNormal, T radius, int rotationCount = 1)
    {
        var scalarProcessor = unitNormal.X.ScalarProcessor;
        var center = LinVector3D<T>.Zero(scalarProcessor);
        return CirclePath3D<T>.Create(scalarProcessor, center, unitNormal, radius, rotationCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CirclePath3D<T> CreateCircle3D<T>(this ILinVector3D<T> unitNormal, ILinVector3D<T> center, T radius, int rotationCount = 1)
    {
        var scalarProcessor = unitNormal.X.ScalarProcessor;
        return CirclePath3D<T>.Create(scalarProcessor, center, unitNormal, radius, rotationCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath2D<T> CreateMathCurve2D<T>(ScalarRange<T> xRange, Func<Scalar<T>, Scalar<T>> mathFunction)
    {
        return ComputedPath2D<T>.Finite(
            xRange,
            t => LinVector2D<T>.Create(t, mathFunction(t))
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AffineMappedPath3D<T> CreateMappedCurve3D<T>(
        this ParametricPath3D<T> curve,
        Func<LinVector3D<T>, LinVector3D<T>> pointMap,
        Func<LinVector3D<T>, LinVector3D<T>>? vectorMap = null)
    {
        return AffineMappedPath3D<T>.Create(
            curve,
            pointMap,
            vectorMap ?? pointMap
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedScalarSignal<T> GetDistanceCurve<T>(this ParametricPath3D<T> curve, ILinVector3D<T> point)
    {
        var fixedPoint = point.ToVector3D();
        return CreateScalarSignal(
            curve.TimeRange,
            curve.IsPeriodic,
            t => curve.GetValue(t).GetDistanceToPoint(fixedPoint)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedScalarSignal<T> GetDistanceCurve<T>(this ParametricPath2D<T> curve1, ParametricPath2D<T> curve2)
    {
        return CreateScalarSignal(
            curve1.TimeRange,
            curve1.IsPeriodic,
            t => curve1.GetValue(t).GetDistanceToPoint(curve2.GetValue(t))
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedScalarSignal<T> GetDistanceCurve<T>(this ParametricPath3D<T> curve1, ParametricPath3D<T> curve2)
    {
        return CreateScalarSignal(
            curve1.TimeRange,
            curve1.IsPeriodic,
            t => curve1.GetValue(t).GetDistanceToPoint(curve2.GetValue(t))
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> GetOffsetCurve<T>(this ParametricPath3D<T> curve, T offsetVectorX, T offsetVectorY, T offsetVectorZ)
    {
        var scalarProcessor = curve.TimeRange.ScalarProcessor;
        var offsetVector = LinVector3D<T>.Create(scalarProcessor, offsetVectorX, offsetVectorY, offsetVectorZ);

        return CreateComputedPath(
            curve,
            t => curve.GetValue(t) + offsetVector
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> GetOffsetCurve<T>(this ParametricPath3D<T> curve, ILinVector3D<T> offsetVector)
    {
        var vector = offsetVector.ToVector3D();
        return CreateComputedPath(
            curve,
            t => curve.GetValue(t) + vector
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> GetOffsetCurve<T>(this ParametricPath3D<T> curve, ParametricPath3D<T> offsetVectorCurve)
    {
        return CreateComputedPath(
            curve,
            t => curve.GetValue(t) + offsetVectorCurve.GetValue(t),
            t => curve.GetDerivative1Value(t) + offsetVectorCurve.GetDerivative1Value(t),
            t => curve.GetDerivative2Value(t) + offsetVectorCurve.GetDerivative2Value(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> GetTangentCurve<T>(this ParametricPath3D<T> curve)
    {
        return ComputedPath3D<T>.Create(
            curve.TimeRange,
            curve.IsPeriodic,
            curve.GetDerivative1Value,
            curve.GetDerivative2Value
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> GetPlaneNormalCurve<T>(this ParametricPath3D<T> curve1, ParametricPath3D<T> curve2, ParametricPath3D<T> curve3)
    {
        return ComputedPath3D<T>.Create(
            curve1.TimeRange,
            curve1.IsPeriodic,
            t =>
            {
                var p1 = curve1.GetValue(t);
                var p2 = curve2.GetValue(t);
                var p3 = curve3.GetValue(t);

                return (p2 - p1).VectorUnitCross(p3 - p2);
            }
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> GetMidPointCurve<T>(this ParametricPath3D<T> curve, ILinVector3D<T> point)
    {
        var scalarProcessor = curve.TimeRange.ScalarProcessor;
        var half = scalarProcessor.ScalarFromNumber(0.5d);
        var fixedPoint = point.ToVector3D();

        return CreateComputedPath(
            curve,
            t => half * (curve.GetValue(t) + fixedPoint),
            t => half * curve.GetDerivative1Value(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> GetMidPointCurve<T>(this ParametricPath3D<T> curve1, ParametricPath3D<T> curve2)
    {
        var scalarProcessor = curve1.TimeRange.ScalarProcessor;
        var half = scalarProcessor.ScalarFromNumber(0.5d);

        return CreateComputedPath(
            curve1,
            t => half * (curve1.GetValue(t) + curve2.GetValue(t)),
            t => half * (curve1.GetDerivative1Value(t) + curve2.GetDerivative1Value(t)),
            t => half * (curve1.GetDerivative2Value(t) + curve2.GetDerivative2Value(t))
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> GetMedianPointCurve<T>(this ParametricPath3D<T> curve1, ParametricPath3D<T> curve2, ILinVector3D<T> point)
    {
        var scalarProcessor = curve1.TimeRange.ScalarProcessor;
        var third = scalarProcessor.ScalarFromNumber(1d / 3d);
        var fixedPoint = point.ToVector3D();

        return CreateComputedPath(
            curve1,
            t => third * (curve1.GetValue(t) + curve2.GetValue(t) + fixedPoint),
            t => third * (curve1.GetDerivative1Value(t) + curve2.GetDerivative1Value(t)),
            t => third * (curve1.GetDerivative2Value(t) + curve2.GetDerivative2Value(t))
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> GetMedianPointCurve<T>(this ParametricPath3D<T> curve, ILinVector3D<T> point1, ILinVector3D<T> point2)
    {
        var scalarProcessor = curve.TimeRange.ScalarProcessor;
        var third = scalarProcessor.ScalarFromNumber(1d / 3d);
        var fixedPoint = point1.ToVector3D() + point2.ToVector3D();

        return CreateComputedPath(
            curve,
            t => third * (curve.GetValue(t) + fixedPoint),
            t => third * curve.GetDerivative1Value(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedPath3D<T> GetMedianPointCurve<T>(this ParametricPath3D<T> curve1, ParametricPath3D<T> curve2, ParametricPath3D<T> curve3)
    {
        var scalarProcessor = curve1.TimeRange.ScalarProcessor;
        var third = scalarProcessor.ScalarFromNumber(1d / 3d);

        return CreateComputedPath(
            curve1,
            t => third * (curve1.GetValue(t) + curve2.GetValue(t) + curve3.GetValue(t)),
            t => third * (curve1.GetDerivative1Value(t) + curve2.GetDerivative1Value(t) + curve3.GetDerivative1Value(t)),
            t => third * (curve1.GetDerivative2Value(t) + curve2.GetDerivative2Value(t) + curve3.GetDerivative2Value(t))
        );
    }
}
