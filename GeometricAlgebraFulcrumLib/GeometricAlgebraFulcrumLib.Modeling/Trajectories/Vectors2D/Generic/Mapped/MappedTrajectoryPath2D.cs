using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Mapped;

/// <summary>
/// A 2D path that maps values from a generic trajectory to 2D vectors.
/// Allows transformation from any value type TValue to 2D vectors.
/// </summary>
/// <typeparam name="TScalar">Scalar type for time and coordinates</typeparam>
/// <typeparam name="TValue">Value type produced by the base trajectory</typeparam>
public sealed class MappedTrajectoryPath2D<TScalar, TValue> :
    ParametricPath2D<TScalar>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MappedTrajectoryPath2D<TScalar, TValue> Create(
        ITrajectory<TScalar, TValue> baseTrajectory,
        Func<TValue, LinVector2D<TScalar>> valueMap)
    {
        return new MappedTrajectoryPath2D<TScalar, TValue>(baseTrajectory, valueMap);
    }


    /// <summary>
    /// The base trajectory that produces values of type TValue
    /// </summary>
    public ITrajectory<TScalar, TValue> BaseTrajectory { get; }

    /// <summary>
    /// Function that maps trajectory values (type TValue) to 2D vectors
    /// </summary>
    public Func<TValue, LinVector2D<TScalar>> ValueMap { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MappedTrajectoryPath2D(
        ITrajectory<TScalar, TValue> baseTrajectory,
        Func<TValue, LinVector2D<TScalar>> valueMap)
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
    /// Gets the 2D vector at time t by evaluating the base trajectory and mapping the result
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<TScalar> GetValue(Scalar<TScalar> t)
    {
        return ValueMap(BaseTrajectory.GetValue(t));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<TScalar> ToFinitePath()
    {
        return IsFinite
            ? this
            : new MappedTrajectoryPath2D<TScalar, TValue>(
                (ITrajectory<TScalar, TValue>)BaseTrajectory.ToFinite(),
                ValueMap
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<TScalar> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new MappedTrajectoryPath2D<TScalar, TValue>(
                (ITrajectory<TScalar, TValue>)BaseTrajectory.ToPeriodic(),
                ValueMap
            );
    }

    /// <summary>
    /// First derivative is not available for mapped trajectories.
    /// Returns zero vector as mapping function derivatives are unknown.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<TScalar> GetDerivative1Value(Scalar<TScalar> t)
    {
        return LinVector2D<TScalar>.Zero(t.ScalarProcessor);
    }

    /// <summary>
    /// Second derivative is not available for mapped trajectories.
    /// Returns zero vector as mapping function derivatives are unknown.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<TScalar> GetDerivative2Value(Scalar<TScalar> t)
    {
        return LinVector2D<TScalar>.Zero(t.ScalarProcessor);
    }
}
