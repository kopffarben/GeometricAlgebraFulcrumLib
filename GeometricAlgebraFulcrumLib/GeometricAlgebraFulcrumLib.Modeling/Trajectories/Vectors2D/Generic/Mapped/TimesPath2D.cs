using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Mapped;

/// <summary>
/// A 2D path that is the component-wise product of multiple base paths.
/// At any time t, the result is: path1(t) ⊙ path2(t) ⊙ ... ⊙ pathN(t)
/// where ⊙ denotes component-wise multiplication: (x1,y1) ⊙ (x2,y2) = (x1*x2, y1*y2)
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class TimesPath2D<T> :
    ParametricPath2D<T>,
    IReadOnlyList<ParametricPath2D<T>>
{
    private static void Add(ICollection<ParametricPath2D<T>> basePaths, ParametricPath2D<T> path)
    {
        // Flatten nested TimesPath2D structures
        if (path is not TimesPath2D<T> timesPath)
        {
            basePaths.Add(path);
            return;
        }

        foreach (var p in timesPath)
            Add(basePaths, p);
    }


    #region Static Factory Methods - Finite

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimesPath2D<T> Finite(ParametricPath2D<T> path1, ParametricPath2D<T> path2)
    {
        return Finite(new[] { path1, path2 });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimesPath2D<T> Finite(ParametricPath2D<T> path1, ParametricPath2D<T> path2, params ParametricPath2D<T>[] pathList)
    {
        var paths = new List<ParametricPath2D<T>>(pathList.Length + 2)
        {
            path1,
            path2
        };

        paths.AddRange(pathList);

        return Finite(paths);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimesPath2D<T> Finite(IEnumerable<ParametricPath2D<T>> pathList)
    {
        var basePaths = new List<ParametricPath2D<T>>();

        foreach (var path in pathList)
            Add(basePaths, path);

        return new TimesPath2D<T>(false, basePaths);
    }

    #endregion

    #region Static Factory Methods - Periodic

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimesPath2D<T> Periodic(ParametricPath2D<T> path1, ParametricPath2D<T> path2)
    {
        return Periodic(new[] { path1, path2 });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimesPath2D<T> Periodic(ParametricPath2D<T> path1, ParametricPath2D<T> path2, params ParametricPath2D<T>[] pathList)
    {
        var paths = new List<ParametricPath2D<T>>(pathList.Length + 2)
        {
            path1,
            path2
        };

        paths.AddRange(pathList);

        return Periodic(paths);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimesPath2D<T> Periodic(IEnumerable<ParametricPath2D<T>> pathList)
    {
        var basePaths = new List<ParametricPath2D<T>>();

        foreach (var path in pathList)
            Add(basePaths, path);

        return new TimesPath2D<T>(true, basePaths);
    }

    #endregion


    public IReadOnlyList<ParametricPath2D<T>> BasePaths { get; }

    public int Count
        => BasePaths.Count;

    public ParametricPath2D<T> this[int index]
        => BasePaths[index];


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private TimesPath2D(bool isPeriodic, IReadOnlyList<ParametricPath2D<T>> basePaths)
        : base(
            ScalarRange<T>.Create(
                basePaths.Select(p => p.TimeRange.MinValue)
                    .Aggregate((min, current) => current < min ? current : min),
                basePaths.Select(p => p.TimeRange.MaxValue)
                    .Aggregate((max, current) => current > max ? current : max)
            ),
            isPeriodic
        )
    {
        BasePaths = basePaths;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return BasePaths.Count >= 2 &&
               BasePaths.All(p => p.IsValid()) &&
               TimeRange.IsFinite;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new TimesPath2D<T>(
                false,
                BasePaths
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath2D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new TimesPath2D<T>(
                true,
                BasePaths
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetValue(Scalar<T> t)
    {
        t = TimeRange.Clamp(t);

        // Start with identity (1, 1) for component-wise multiplication
        return BasePaths.Aggregate(
            LinVector2D<T>.Symmetric(t.ScalarProcessor),
            (accumulator, path) =>
            {
                var pathValue = path.GetValue(t);
                // Component-wise multiplication: (x1, y1) * (x2, y2) = (x1*x2, y1*y2)
                return LinVector2D<T>.Create(
                    accumulator.Item1 * pathValue.Item1,
                    accumulator.Item2 * pathValue.Item2
                );
            }
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative1Value(Scalar<T> t)
    {
        // Component-wise product derivative requires product rule: (f*g)' = f'*g + f*g'
        // For multiple factors, this becomes complex - simplified implementation returns zero
        // TODO: Implement proper derivative for component-wise product
        return LinVector2D<T>.Zero(t.ScalarProcessor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative not implemented for component-wise product
        return LinVector2D<T>.Zero(t.ScalarProcessor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<ParametricPath2D<T>> GetEnumerator()
    {
        return BasePaths.GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
