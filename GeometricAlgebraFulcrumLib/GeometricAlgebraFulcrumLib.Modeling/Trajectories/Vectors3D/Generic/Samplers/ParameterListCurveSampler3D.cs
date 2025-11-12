using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Samplers;

/// <summary>
/// Samples a parametric curve at explicitly specified parameter values.
/// </summary>
/// <typeparam name="T">The scalar type (double, float, etc.)</typeparam>
public class ParameterListCurveSampler3D<T> :
    IParametricCurveSampler3D<T>
{
    public ParametricPath3D<T> Curve { get; private set; }

    public ScalarRange<T> ParameterRange
        => ScalarRange<T>.Create(
            ParameterValueSet[0],
            ParameterValueSet[^1]
        );

    public bool IsPeriodic { get; private set; }

    public ImmutableSortedSet<Scalar<T>> ParameterValueSet { get; private set; }

    public int Count
        => ParameterValueSet.Count;

    public ParametricPath3DLocalFrame<T> this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
            {
                if (IsPeriodic)
                    index = index.Mod(Count);

                else
                    throw new IndexOutOfRangeException();
            }

            return Curve.GetFrame(
                ParameterValueSet[index]
            );
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ParameterListCurveSampler3D(ParametricPath3D<T> curve, ImmutableSortedSet<Scalar<T>> parameterValueSet, bool isPeriodic = false)
    {
        IsPeriodic = isPeriodic;
        Curve = curve;
        ParameterValueSet = parameterValueSet;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        var scalarProcessor = Curve.TimeRange.ScalarProcessor;
        return ParameterRange.IsValid() &&
               Curve.IsValid() &&
               ParameterValueSet.Count > 0 &&
               scalarProcessor.IsZeroOrPositive(ParameterValueSet[0].ScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ParameterListCurveSampler3D<T> SetCurve(ParametricPath3D<T> curve, ImmutableSortedSet<Scalar<T>> parameterValueSet, bool isPeriodic)
    {
        IsPeriodic = isPeriodic;
        Curve = curve;
        ParameterValueSet = parameterValueSet;

        Debug.Assert(IsValid());

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<Scalar<T>> GetParameterValues()
    {
        return Enumerable
            .Range(0, Count)
            .Select(i => ParameterValueSet[i]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ScalarRange<T>> GetParameterSections()
    {
        if (IsPeriodic)
            return Enumerable
                .Range(0, Count)
                .Select(i =>
                    ScalarRange<T>.Create(
                        ParameterValueSet[i],
                        ParameterValueSet[(i + 1).Mod(Count)]
                    )
                );

        return Enumerable
            .Range(0, Count - 1)
            .Select(i =>
                ScalarRange<T>.Create(
                    ParameterValueSet[i],
                    ParameterValueSet[i + 1]
                )
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<LinVector3D<T>> GetPoints()
    {
        return Enumerable
            .Range(0, Count)
            .Select(i => Curve.GetValue(ParameterValueSet[i]));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<LinVector3D<T>> GetTangents()
    {
        return Enumerable
            .Range(0, Count)
            .Select(i => Curve.GetDerivative1Value(ParameterValueSet[i]));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ParametricPath3DLocalFrame<T>> GetFrames()
    {
        return Enumerable
            .Range(0, Count)
            .Select(i => Curve.GetFrame(ParameterValueSet[i]));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<ParametricPath3DLocalFrame<T>> GetEnumerator()
    {
        return GetFrames().GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
