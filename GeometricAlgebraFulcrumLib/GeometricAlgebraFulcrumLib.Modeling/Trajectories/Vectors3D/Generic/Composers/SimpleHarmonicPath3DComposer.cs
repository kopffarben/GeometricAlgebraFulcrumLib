using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Composers;

/// <summary>
/// Composer for creating Fourier series-like paths using SimpleHarmonicPath3D components.
/// Combines multiple harmonic terms with different frequencies (integer multiples).
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class SimpleHarmonicPath3DComposer<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SimpleHarmonicPath3DComposer<T> Create(IScalarProcessor<T> scalarProcessor)
    {
        return new SimpleHarmonicPath3DComposer<T>(scalarProcessor);
    }


    public IScalarProcessor<T> ScalarProcessor { get; }

    private readonly Dictionary<int, SimpleHarmonicPath3D<T>> _harmonicTerms
        = new Dictionary<int, SimpleHarmonicPath3D<T>>();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SimpleHarmonicPath3DComposer(IScalarProcessor<T> scalarProcessor)
    {
        ScalarProcessor = scalarProcessor;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleHarmonicPath3DComposer<T> Clear()
    {
        _harmonicTerms.Clear();

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleHarmonicPath3DComposer<T> RemoveHarmonic(int harmonicFactor)
    {
        _harmonicTerms.Remove(harmonicFactor);

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleHarmonicPath3DComposer<T> SetHarmonic(int harmonicFactor, T magnitudeX, T magnitudeY, T magnitudeZ)
    {
        return SetHarmonic(
            harmonicFactor,
            LinVector3D<T>.Create(ScalarProcessor, magnitudeX, magnitudeY, magnitudeZ)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleHarmonicPath3DComposer<T> SetHarmonic(int harmonicFactor, LinVector3D<T> magnitude)
    {
        var oneThird = ScalarProcessor.Divide(
            ScalarProcessor.One.ScalarValue,
            ScalarProcessor.ScalarFromNumber(3).ScalarValue
        );

        return SetHarmonic(
            harmonicFactor,
            magnitude,
            LinVector3D<T>.Create(
                ScalarProcessor,
                ScalarProcessor.Zero.ScalarValue,
                oneThird.ScalarValue,
                ScalarProcessor.Negative(oneThird.ScalarValue).ScalarValue
            )
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleHarmonicPath3DComposer<T> SetHarmonic(int harmonicFactor, LinVector3D<T> magnitude, LinVector3D<T> timeOffset)
    {
        var term =
            SimpleHarmonicPath3D<T>.Periodic(
                ScalarRange<T>.SymmetricPi(ScalarProcessor),
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
    public ParametricPath3D<T> GetSignal(bool isPeriodic)
    {
        return isPeriodic
            ? PlusPath3D<T>.Periodic(_harmonicTerms.Values)
            : PlusPath3D<T>.Finite(_harmonicTerms.Values);
    }
}
