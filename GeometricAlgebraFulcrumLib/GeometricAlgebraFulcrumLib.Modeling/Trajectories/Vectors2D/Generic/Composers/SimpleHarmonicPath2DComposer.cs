using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Mapped;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Composers;

/// <summary>
/// Composer for building complex harmonic paths by combining simple harmonic motions
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class SimpleHarmonicPath2DComposer<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleHarmonicPath2DComposer<T> Create(IScalarProcessor<T> scalarProcessor)
    {
        return new SimpleHarmonicPath2DComposer<T>(scalarProcessor);
    }


    private readonly Dictionary<int, SimpleHarmonicPath2D<T>> _harmonicTerms
        = new Dictionary<int, SimpleHarmonicPath2D<T>>();

    public IScalarProcessor<T> ScalarProcessor { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SimpleHarmonicPath2DComposer(IScalarProcessor<T> scalarProcessor)
    {
        ScalarProcessor = scalarProcessor;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleHarmonicPath2DComposer<T> Clear()
    {
        _harmonicTerms.Clear();

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleHarmonicPath2DComposer<T> RemoveHarmonic(int harmonicFactor)
    {
        _harmonicTerms.Remove(harmonicFactor);

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleHarmonicPath2DComposer<T> SetHarmonic(int harmonicFactor, LinVector2D<T> magnitude, LinVector2D<T> timeOffset)
    {
        var term =
            SimpleHarmonicPath2D<T>.Create(
                ScalarProcessor,
                true, // isPeriodic - harmonics are always periodic
                harmonicFactor,
                magnitude,
                timeOffset
            );

        if (_harmonicTerms.ContainsKey(harmonicFactor))
            _harmonicTerms[harmonicFactor] = term;
        else
            _harmonicTerms.Add(harmonicFactor, term);

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ParametricPath2D<T> GetPath(bool isPeriodic)
    {
        return isPeriodic
            ? PlusPath2D<T>.Periodic(_harmonicTerms.Values)
            : PlusPath2D<T>.Finite(_harmonicTerms.Values);
    }

}
