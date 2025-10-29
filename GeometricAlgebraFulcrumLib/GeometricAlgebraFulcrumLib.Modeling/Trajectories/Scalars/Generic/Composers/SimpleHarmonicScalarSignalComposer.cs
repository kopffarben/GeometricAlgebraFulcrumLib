using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Composers;

/// <summary>
/// Composer for building sum of simple harmonic signals with different harmonic factors
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public class SimpleHarmonicScalarSignalComposer<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleHarmonicScalarSignalComposer<T> Create(IScalarProcessor<T> scalarProcessor)
    {
        return new SimpleHarmonicScalarSignalComposer<T>(scalarProcessor);
    }


    private readonly IScalarProcessor<T> _scalarProcessor;

    private readonly Dictionary<int, SimpleHarmonicScalarSignal<T>> _harmonicTerms
        = new Dictionary<int, SimpleHarmonicScalarSignal<T>>();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SimpleHarmonicScalarSignalComposer(IScalarProcessor<T> scalarProcessor)
    {
        _scalarProcessor = scalarProcessor;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        return _harmonicTerms.Values.All(term => term.IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleHarmonicScalarSignalComposer<T> Clear()
    {
        _harmonicTerms.Clear();

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleHarmonicScalarSignalComposer<T> RemoveHarmonic(int harmonicFactor)
    {
        _harmonicTerms.Remove(harmonicFactor);

        return this;
    }

    public SimpleHarmonicScalarSignalComposer<T> SetHarmonic(int harmonicFactor, Scalar<T> magnitude, Scalar<T> timeShift)
    {
        var term = SimpleHarmonicScalarSignal<T>.Create(
            _scalarProcessor,
            true,
            harmonicFactor,
            magnitude,
            timeShift
        );

        if (_harmonicTerms.ContainsKey(harmonicFactor))
            _harmonicTerms[harmonicFactor] = term;
        else
            _harmonicTerms.Add(harmonicFactor, term);

        return this;
    }

    public SimpleHarmonicScalarSignalComposer<T> SetHarmonic(int harmonicFactor, Scalar<T> magnitude)
    {
        return SetHarmonic(harmonicFactor, magnitude, _scalarProcessor.Zero.ToScalar());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignal<T> GetSignal(bool isPeriodic)
    {
        return isPeriodic
            ? ScalarPlusSignal<T>.Periodic(_harmonicTerms.Values)
            : ScalarPlusSignal<T>.Finite(_harmonicTerms.Values);
    }
}
