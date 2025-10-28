using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;

/// <summary>
/// Maps a generic trajectory of type TIn to a 3D parametric path using a mapping function.
/// This allows converting scalar trajectories or other trajectory types to 3D paths.
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
/// <typeparam name="TIn">Input value type from the base trajectory</typeparam>
public sealed class MappedTrajectoryPath3D<T, TIn> :
    ParametricPath3D<T>
{
    /// <summary>
    /// Creates a mapped trajectory path from a base trajectory and mapping function.
    /// </summary>
    /// <param name="baseTrajectory">The source trajectory to map from</param>
    /// <param name="valueMap">Function that maps TIn values to 3D vectors</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MappedTrajectoryPath3D<T, TIn> Create(Trajectory<T, TIn> baseTrajectory, Func<TIn, LinVector3D<T>> valueMap)
    {
        return new MappedTrajectoryPath3D<T, TIn>(baseTrajectory, valueMap);
    }


    /// <summary>
    /// The source trajectory that produces TIn values
    /// </summary>
    public Trajectory<T, TIn> BaseTrajectory { get; }

    /// <summary>
    /// The mapping function that converts TIn values to 3D vectors
    /// </summary>
    public Func<TIn, LinVector3D<T>> ValueMap { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MappedTrajectoryPath3D(Trajectory<T, TIn> baseTrajectory, Func<TIn, LinVector3D<T>> valueMap)
        : base(baseTrajectory.TimeRange, baseTrajectory.IsPeriodic)
    {
        BaseTrajectory = baseTrajectory;
        ValueMap = valueMap;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BaseTrajectory.IsValid();
    }

    /// <summary>
    /// Evaluates the mapped path at time t by first evaluating the base trajectory,
    /// then applying the mapping function to the result.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        return ValueMap(BaseTrajectory.GetValue(t));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new MappedTrajectoryPath3D<T, TIn>(
                (Trajectory<T, TIn>)BaseTrajectory.ToFinite(),
                ValueMap
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new MappedTrajectoryPath3D<T, TIn>(
                (Trajectory<T, TIn>)BaseTrajectory.ToPeriodic(),
                ValueMap
            );
    }

    /// <summary>
    /// First derivative is not automatically computable from the mapping function alone.
    /// Returns zero vector. Override if analytical derivative is available.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        // Cannot automatically compute derivative through arbitrary mapping function
        // Would require chain rule and derivative of the mapping function
        return LinVector3D<T>.Zero(TimeRange.MinValue.ScalarProcessor);
    }

    /// <summary>
    /// Second derivative is not automatically computable from the mapping function alone.
    /// Returns zero vector. Override if analytical derivative is available.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        // Cannot automatically compute second derivative through arbitrary mapping function
        return LinVector3D<T>.Zero(TimeRange.MinValue.ScalarProcessor);
    }
}
