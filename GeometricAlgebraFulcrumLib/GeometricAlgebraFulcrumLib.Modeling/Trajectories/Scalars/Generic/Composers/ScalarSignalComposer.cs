using System.Collections;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Composers;

/// <summary>
/// Mutable composer for building sequential signal lists
/// Implements IReadOnlyList for compatibility with ScalarListSignal
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class ScalarSignalComposer<T> :
    IReadOnlyList<ScalarSignal<T>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScalarSignalComposer<T> Create(IScalarProcessor<T> scalarProcessor)
    {
        return new ScalarSignalComposer<T>(scalarProcessor);
    }


    private readonly IScalarProcessor<T> _scalarProcessor;
    private readonly List<ScalarSignal<T>> _scalarList = new List<ScalarSignal<T>>();


    public int Count
        => _scalarList.Count;

    public ScalarSignal<T> this[int index]
    {
        get => _scalarList[index];
        set => _scalarList[index] = value;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarSignalComposer(IScalarProcessor<T> scalarProcessor)
    {
        _scalarProcessor = scalarProcessor;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalComposer<T> Clear()
    {
        _scalarList.Clear();

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarSignalComposer<T> RemoveAt(int index)
    {
        _scalarList.RemoveAt(index);

        return this;
    }


    public ScalarSignalComposer<T> AppendSignal(ScalarSignal<T> scalar)
    {
        if (scalar is ScalarListSignal<T> scalarList)
        {
            foreach (var s in scalarList)
                AppendSignal(s);

            return this;
        }

        _scalarList.Add(scalar);

        return this;
    }

    public ScalarSignalComposer<T> PrependSignal(ScalarSignal<T> scalar)
    {
        if (scalar is ScalarListSignal<T> scalarList)
        {
            foreach (var s in scalarList.Reverse())
                PrependSignal(s);

            return this;
        }

        _scalarList.Insert(0, scalar);

        return this;
    }

    public ScalarSignalComposer<T> InsertSignal(int index, ScalarSignal<T> scalar)
    {
        if (scalar is ScalarListSignal<T> scalarList)
        {
            foreach (var s in scalarList.Reverse())
                InsertSignal(index, s);

            return this;
        }

        _scalarList.Insert(index, scalar);

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarListSignal<T> GetFiniteSignal()
    {
        return ScalarListSignal<T>.Finite(_scalarList);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScalarListSignal<T> GetPeriodicSignal()
    {
        return ScalarListSignal<T>.Periodic(_scalarList);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<ScalarSignal<T>> GetEnumerator()
    {
        return _scalarList.GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
