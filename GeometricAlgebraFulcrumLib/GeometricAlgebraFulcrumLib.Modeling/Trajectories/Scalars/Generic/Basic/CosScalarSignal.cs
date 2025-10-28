using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;

/// <summary>
/// A scalar signal representing the cosine function: cos(t)
/// Time range: [-π, π]
/// Value range: [-1, 1]
/// Derivative: -sin(t)
/// Second derivative: -cos(t)
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
public sealed class CosScalarSignal<T> :
    ScalarSignal<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CosScalarSignal<T> Finite(IScalarProcessor<T> scalarProcessor)
    {
        return new CosScalarSignal<T>(scalarProcessor, false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CosScalarSignal<T> Periodic(IScalarProcessor<T> scalarProcessor)
    {
        return new CosScalarSignal<T>(scalarProcessor, true);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private CosScalarSignal(IScalarProcessor<T> scalarProcessor, bool isPeriodic)
        : base(ScalarRange<T>.SymmetricPi(scalarProcessor), isPeriodic)
    {
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToFiniteSignal()
    {
        return Finite(ScalarProcessor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ScalarSignal<T> ToPeriodicSignal()
    {
        return Periodic(ScalarProcessor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetValue(Scalar<T> t)
    {
        return ScalarProcessor.Cos(t.ScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative1Value(Scalar<T> t)
    {
        return ScalarProcessor.Negative(ScalarProcessor.Sin(t.ScalarValue).ScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Scalar<T> GetDerivative2Value(Scalar<T> t)
    {
        return ScalarProcessor.Negative(ScalarProcessor.Cos(t.ScalarValue).ScalarValue);
    }
}
