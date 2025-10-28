using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;

/// <summary>
/// A 3D path that represents the component-wise product of multiple paths.
/// GetValue(t) = BaseSignals[0](t) ⊗ BaseSignals[1](t) ⊗ ... ⊗ BaseSignals[n](t)
/// where ⊗ denotes component-wise (Hadamard) product.
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
public sealed class TimesPath3D<T> :
    ParametricPath3D<T>,
    IReadOnlyList<ParametricPath3D<T>>
{
    /// <summary>
    /// Recursively flattens nested TimesPath3D instances into a flat list.
    /// This ensures that ((A*B)*C) becomes [A,B,C] instead of nested structure.
    /// </summary>
    private static void Add(ICollection<ParametricPath3D<T>> baseSignals, ParametricPath3D<T> path)
    {
        if (path is not TimesPath3D<T> timesPath)
        {
            baseSignals.Add(path);
            return;
        }

        // Recursively flatten nested TimesPath3D
        foreach (var s in timesPath)
            Add(baseSignals, s);
    }


    #region Factory Methods - Finite

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimesPath3D<T> Finite(ParametricPath3D<T> path1, ParametricPath3D<T> path2)
    {
        return Finite(new[] { path1, path2 });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimesPath3D<T> Finite(ParametricPath3D<T> path1, ParametricPath3D<T> path2, params ParametricPath3D<T>[] pathList)
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
    public static TimesPath3D<T> Finite(IEnumerable<ParametricPath3D<T>> pathList)
    {
        var baseSignals = new List<ParametricPath3D<T>>();

        foreach (var path in pathList)
            Add(baseSignals, path);

        return new TimesPath3D<T>(false, baseSignals);
    }

    #endregion

    #region Factory Methods - Periodic

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimesPath3D<T> Periodic(ParametricPath3D<T> path1, ParametricPath3D<T> path2)
    {
        return Periodic(new[] { path1, path2 });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimesPath3D<T> Periodic(ParametricPath3D<T> path1, ParametricPath3D<T> path2, params ParametricPath3D<T>[] pathList)
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
    public static TimesPath3D<T> Periodic(IEnumerable<ParametricPath3D<T>> pathList)
    {
        var baseSignals = new List<ParametricPath3D<T>>();

        foreach (var path in pathList)
            Add(baseSignals, path);

        return new TimesPath3D<T>(true, baseSignals);
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
    private TimesPath3D(bool isPeriodic, IReadOnlyList<ParametricPath3D<T>> baseSignals)
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
            : new TimesPath3D<T>(
                false,
                BaseSignals
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new TimesPath3D<T>(
                true,
                BaseSignals
            );
    }

    /// <summary>
    /// Evaluates the path at parameter t by computing the component-wise product
    /// of all base path values.
    /// GetValue(t) = BaseSignals[0](t) ⊗ BaseSignals[1](t) ⊗ ... ⊗ BaseSignals[n](t)
    /// where ⊗ denotes component-wise (Hadamard) product.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        // Start with (1, 1, 1) as the multiplicative identity
        // Note: LinVector3D<T>.Symmetric creates (1, 1, 1)
        return BaseSignals.Aggregate(
            LinVector3D<T>.Symmetric(ScalarProcessor),
            (accumulator, path) => accumulator.VectorComponentTimes(path.GetValue(t))
        );
    }

    /// <summary>
    /// Gets the first derivative using the product rule for component-wise products.
    /// For product P = f₁ ⊗ f₂ ⊗ ... ⊗ fₙ:
    /// dP/dt = Σᵢ (f₁ ⊗ ... ⊗ fᵢ₋₁ ⊗ f'ᵢ ⊗ fᵢ₊₁ ⊗ ... ⊗ fₙ)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        // Product rule: Sum over each factor's derivative multiplied by all other factors
        var result = LinVector3D<T>.Zero(ScalarProcessor);

        for (var i = 0; i < BaseSignals.Count; i++)
        {
            // Start with the derivative of the i-th factor
            var term = BaseSignals[i].GetDerivative1Value(t);

            // Multiply by all other factors (excluding i-th)
            for (var j = 0; j < BaseSignals.Count; j++)
            {
                if (j != i)
                {
                    term = term.VectorComponentTimes(BaseSignals[j].GetValue(t));
                }
            }

            result += term;
        }

        return result;
    }

    /// <summary>
    /// Gets the second derivative using the product rule twice.
    /// This is a complex expression involving all combinations of derivatives.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        // Second derivative of product requires applying product rule twice
        // For simplicity, we compute: d²/dt²[P] term by term
        var result = LinVector3D<T>.Zero(ScalarProcessor);

        // First term: Sum of (f''ᵢ ⊗ ∏_{j≠i} fⱼ)
        for (var i = 0; i < BaseSignals.Count; i++)
        {
            var term = BaseSignals[i].GetDerivative2Value(t);

            for (var j = 0; j < BaseSignals.Count; j++)
            {
                if (j != i)
                {
                    term = term.VectorComponentTimes(BaseSignals[j].GetValue(t));
                }
            }

            result += term;
        }

        // Second term: Sum of cross terms (f'ᵢ ⊗ f'ⱼ ⊗ ∏_{k≠i,j} fₖ)
        for (var i = 0; i < BaseSignals.Count; i++)
        {
            for (var j = i + 1; j < BaseSignals.Count; j++)
            {
                var term = BaseSignals[i].GetDerivative1Value(t)
                    .VectorComponentTimes(BaseSignals[j].GetDerivative1Value(t));

                for (var k = 0; k < BaseSignals.Count; k++)
                {
                    if (k != i && k != j)
                    {
                        term = term.VectorComponentTimes(BaseSignals[k].GetValue(t));
                    }
                }

                // Multiply by 2 because we only iterate over i < j
                result += term + term;
            }
        }

        return result;
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
