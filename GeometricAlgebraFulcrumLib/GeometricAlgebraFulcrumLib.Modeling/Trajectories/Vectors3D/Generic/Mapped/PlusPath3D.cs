using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;

/// <summary>
/// A 3D path that represents the sum of multiple paths.
/// GetValue(t) = Σ BaseSignals[i].GetValue(t)
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
public sealed class PlusPath3D<T> :
    ParametricPath3D<T>,
    IReadOnlyList<ParametricPath3D<T>>
{
    /// <summary>
    /// Recursively flattens nested PlusPath3D instances into a flat list.
    /// This ensures that ((A+B)+C) becomes [A,B,C] instead of nested structure.
    /// </summary>
    private static void Add(ICollection<ParametricPath3D<T>> baseSignals, ParametricPath3D<T> path)
    {
        if (path is not PlusPath3D<T> plusPath)
        {
            baseSignals.Add(path);
            return;
        }

        // Recursively flatten nested PlusPath3D
        foreach (var s in plusPath)
            Add(baseSignals, s);
    }


    #region Factory Methods - Finite

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PlusPath3D<T> Finite(ParametricPath3D<T> path1, ParametricPath3D<T> path2)
    {
        return Finite(new[] { path1, path2 });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PlusPath3D<T> Finite(ParametricPath3D<T> path1, ParametricPath3D<T> path2, params ParametricPath3D<T>[] pathList)
    {
        var paths = new List<ParametricPath3D<T>>(pathList.Length + 2)
        {
            path1,
            path2
        };

        paths.AddRange(pathList);

        return Finite(paths);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PlusPath3D<T> Finite(IEnumerable<ParametricPath3D<T>> pathList)
    {
        var baseSignals = new List<ParametricPath3D<T>>();

        foreach (var path in pathList)
            Add(baseSignals, path);

        return new PlusPath3D<T>(false, baseSignals);
    }

    #endregion

    #region Factory Methods - Periodic

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PlusPath3D<T> Periodic(ParametricPath3D<T> path1, ParametricPath3D<T> path2)
    {
        return Periodic(new[] { path1, path2 });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PlusPath3D<T> Periodic(ParametricPath3D<T> path1, ParametricPath3D<T> path2, params ParametricPath3D<T>[] pathList)
    {
        var paths = new List<ParametricPath3D<T>>(pathList.Length + 2)
        {
            path1,
            path2
        };

        paths.AddRange(pathList);

        return Periodic(paths);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PlusPath3D<T> Periodic(IEnumerable<ParametricPath3D<T>> pathList)
    {
        var baseSignals = new List<ParametricPath3D<T>>();

        foreach (var path in pathList)
            Add(baseSignals, path);

        return new PlusPath3D<T>(true, baseSignals);
    }

    #endregion


    public IReadOnlyList<ParametricPath3D<T>> BaseSignals { get; }

    public int Count
        => BaseSignals.Count;

    public ParametricPath3D<T> this[int index]
        => BaseSignals[index];

    private IScalarProcessor<T> ScalarProcessor
        => TimeRange.MinValue.ScalarProcessor;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private PlusPath3D(bool isPeriodic, IReadOnlyList<ParametricPath3D<T>> baseSignals)
        : base(
            ScalarRange<T>.Create(
                baseSignals.Select(s => s.MinTime).OrderBy(t => t.ScalarValue).First(),
                baseSignals.Select(s => s.MaxTime).OrderByDescending(t => t.ScalarValue).First()
            ),
            isPeriodic
        )
    {
        BaseSignals = baseSignals;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BaseSignals.Count >= 2 &&
               BaseSignals.All(s => s.IsValid()) &&
               TimeRange.IsFinite;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new PlusPath3D<T>(
                false,
                BaseSignals
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new PlusPath3D<T>(
                true,
                BaseSignals
            );
    }

    /// <summary>
    /// Evaluates the path at parameter t by summing all base path values.
    /// GetValue(t) = Σ BaseSignals[i].GetValue(t)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        // Note: Not using ClampTime since it's not available for Generic<T>
        // The base paths are responsible for their own clamping

        return BaseSignals.Aggregate(
            LinVector3D<T>.Zero(ScalarProcessor),
            (accumulator, path) => accumulator + path.GetValue(t)
        );
    }

    /// <summary>
    /// Gets the first derivative by summing all base path derivatives.
    /// d/dt[A+B] = dA/dt + dB/dt
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        return BaseSignals.Aggregate(
            LinVector3D<T>.Zero(ScalarProcessor),
            (accumulator, path) => accumulator + path.GetDerivative1Value(t)
        );
    }

    /// <summary>
    /// Gets the second derivative by summing all base path second derivatives.
    /// d²/dt²[A+B] = d²A/dt² + d²B/dt²
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        return BaseSignals.Aggregate(
            LinVector3D<T>.Zero(ScalarProcessor),
            (accumulator, path) => accumulator + path.GetDerivative2Value(t)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<ParametricPath3D<T>> GetEnumerator()
    {
        return BaseSignals.GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
