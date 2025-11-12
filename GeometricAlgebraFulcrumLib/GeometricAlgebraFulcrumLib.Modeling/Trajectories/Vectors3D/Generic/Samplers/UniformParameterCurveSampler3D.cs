using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Samplers;

/// <summary>
/// Samples a parametric curve at uniformly spaced parameter values.
/// </summary>
/// <typeparam name="T">The scalar type (double, float, etc.)</typeparam>
public class UniformParameterCurveSampler3D<T> :
    IParametricCurveSampler3D<T>
{
    public ParametricPath3D<T> Curve { get; private set; }

    public ScalarRange<T> ParameterRange { get; private set; }

    public bool IsPeriodic { get; private set; }

    public Scalar<T> ParameterSectionLength { get; private set; }

    public int Count { get; private set; }

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

            var scalarProcessor = Curve.TimeRange.ScalarProcessor;
            var parameter = scalarProcessor.Add(
                ParameterRange.MinValue,
                scalarProcessor.Times(
                    scalarProcessor.ScalarFromNumber(index),
                    ParameterSectionLength
                )
            );

            return Curve.GetFrame(parameter);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UniformParameterCurveSampler3D(ParametricPath3D<T> curve, ScalarRange<T> parameterRange, int count, bool isPeriodic = false)
    {
        if (isPeriodic && count < 1 || !isPeriodic && count < 2)
            throw new ArgumentOutOfRangeException(nameof(count));

        Count = count;
        IsPeriodic = isPeriodic;
        Curve = curve;
        ParameterRange = parameterRange;

        var scalarProcessor = curve.TimeRange.ScalarProcessor;
        var divisor = isPeriodic ? count : count - 1;
        ParameterSectionLength = scalarProcessor.Divide(
            parameterRange.Length.ScalarValue,
            scalarProcessor.ScalarFromNumber(divisor).ScalarValue
        );

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        return ParameterRange.IsValid() &&
               Curve.IsValid() &&
               (IsPeriodic && Count > 0 || !IsPeriodic && Count > 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UniformParameterCurveSampler3D<T> SetCurve(ParametricPath3D<T> curve, ScalarRange<T> parameterRange, int count, bool isPeriodic)
    {
        if (isPeriodic && count < 1 || !isPeriodic && count < 2)
            throw new ArgumentOutOfRangeException(nameof(count));

        Curve = curve;
        ParameterRange = parameterRange;
        Count = count;
        IsPeriodic = isPeriodic;

        var scalarProcessor = curve.TimeRange.ScalarProcessor;
        var divisor = isPeriodic ? count : count - 1;
        ParameterSectionLength = scalarProcessor.Divide(
            ParameterRange.Length.ScalarValue,
            scalarProcessor.ScalarFromNumber(divisor).ScalarValue
        );

        Debug.Assert(IsValid());

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<Scalar<T>> GetParameterValues()
    {
        var scalarProcessor = Curve.TimeRange.ScalarProcessor;

        return Enumerable
            .Range(0, Count)
            .Select(i =>
                scalarProcessor.Add(
                    ParameterRange.MinValue,
                    scalarProcessor.Times(
                        scalarProcessor.ScalarFromNumber(i),
                        ParameterSectionLength
                    )
                )
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ScalarRange<T>> GetParameterSections()
    {
        var scalarProcessor = Curve.TimeRange.ScalarProcessor;

        return Enumerable
            .Range(0, Count)
            .Select(i =>
            {
                var start = scalarProcessor.Add(
                    ParameterRange.MinValue,
                    scalarProcessor.Times(
                        scalarProcessor.ScalarFromNumber(i),
                        ParameterSectionLength
                    )
                );
                var end = scalarProcessor.Add(
                    ParameterRange.MinValue,
                    scalarProcessor.Times(
                        scalarProcessor.ScalarFromNumber(i + 1),
                        ParameterSectionLength
                    )
                );

                return ScalarRange<T>.Create(start, end);
            });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<LinVector3D<T>> GetPoints()
    {
        var scalarProcessor = Curve.TimeRange.ScalarProcessor;

        return Enumerable
            .Range(0, Count)
            .Select(i =>
            {
                var parameter = scalarProcessor.Add(
                    ParameterRange.MinValue,
                    scalarProcessor.Times(
                        scalarProcessor.ScalarFromNumber(i),
                        ParameterSectionLength
                    )
                );

                return Curve.GetValue(parameter);
            });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<LinVector3D<T>> GetTangents()
    {
        var scalarProcessor = Curve.TimeRange.ScalarProcessor;

        return Enumerable
            .Range(0, Count)
            .Select(i =>
            {
                var parameter = scalarProcessor.Add(
                    ParameterRange.MinValue,
                    scalarProcessor.Times(
                        scalarProcessor.ScalarFromNumber(i),
                        ParameterSectionLength
                    )
                );

                return Curve.GetDerivative1Value(parameter);
            });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ParametricPath3DLocalFrame<T>> GetFrames()
    {
        var scalarProcessor = Curve.TimeRange.ScalarProcessor;

        return Enumerable
            .Range(0, Count)
            .Select(i =>
            {
                var parameter = scalarProcessor.Add(
                    ParameterRange.MinValue,
                    scalarProcessor.Times(
                        scalarProcessor.ScalarFromNumber(i),
                        ParameterSectionLength
                    )
                );

                return Curve.GetFrame(parameter);
            });
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
