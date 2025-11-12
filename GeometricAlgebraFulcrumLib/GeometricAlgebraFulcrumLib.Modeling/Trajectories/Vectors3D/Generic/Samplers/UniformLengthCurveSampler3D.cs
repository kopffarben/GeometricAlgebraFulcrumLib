using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Samplers;

/// <summary>
/// Samples a parametric curve at uniform arc-length intervals.
/// Uses an arc-length parameterization to ensure equal spacing along the curve's actual path.
/// </summary>
/// <typeparam name="T">The scalar type (double, float, etc.)</typeparam>
public class UniformLengthCurveSampler3D<T> :
    IParametricCurveSampler3D<T>
{
    public ArcLengthPath3D<T> ArcLengthCurve { get; private set; }

    public ParametricPath3D<T> Curve
        => ArcLengthCurve;

    public ScalarRange<T> ParameterRange { get; private set; }

    public ScalarRange<T> LengthRange { get; private set; }

    public bool IsPeriodic { get; private set; }

    public Scalar<T> CurveSectionLength { get; private set; }

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
            var length = scalarProcessor.Add(
                LengthRange.MinValue.ScalarValue,
                scalarProcessor.Times(index, CurveSectionLength.ScalarValue).ScalarValue
            );

            return Curve.GetFrame(
                ArcLengthCurve.LengthToTime(length)
            );
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UniformLengthCurveSampler3D(ArcLengthPath3D<T> curve, ScalarRange<T> parameterRange, int count, bool isPeriodic = false)
    {
        if (isPeriodic && count < 1 || !isPeriodic && count < 2)
            throw new ArgumentOutOfRangeException(nameof(count));

        Count = count;
        IsPeriodic = isPeriodic;
        ArcLengthCurve = curve;
        ParameterRange = parameterRange;

        var scalarProcessor = curve.TimeRange.ScalarProcessor;
        LengthRange = ScalarRange<T>.Create(
            curve.TimeToLength(parameterRange.MinValue),
            curve.TimeToLength(parameterRange.MaxValue)
        );

        var divisor = isPeriodic ? count : count - 1;
        CurveSectionLength = scalarProcessor.Divide(
            LengthRange.Length.ScalarValue,
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
    public UniformLengthCurveSampler3D<T> SetCurve(ArcLengthPath3D<T> curve, ScalarRange<T> parameterRange, int count, bool isPeriodic)
    {
        if (isPeriodic && count < 1 || !isPeriodic && count < 2)
            throw new ArgumentOutOfRangeException(nameof(count));

        ArcLengthCurve = curve;
        ParameterRange = parameterRange;
        Count = count;
        IsPeriodic = isPeriodic;

        var scalarProcessor = curve.TimeRange.ScalarProcessor;
        LengthRange = ScalarRange<T>.Create(
            curve.TimeToLength(parameterRange.MinValue),
            curve.TimeToLength(parameterRange.MaxValue)
        );

        var divisor = isPeriodic ? count : count - 1;
        CurveSectionLength = scalarProcessor.Divide(
            LengthRange.Length.ScalarValue,
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
            {
                var length = scalarProcessor.Add(
                    LengthRange.MinValue.ScalarValue,
                    scalarProcessor.Times(i, CurveSectionLength.ScalarValue).ScalarValue
                );
                return ArcLengthCurve.LengthToTime(length);
            });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ScalarRange<T>> GetParameterSections()
    {
        var scalarProcessor = Curve.TimeRange.ScalarProcessor;
        return Enumerable
            .Range(0, Count)
            .Select(i =>
            {
                var length1 = scalarProcessor.Add(
                    LengthRange.MinValue.ScalarValue,
                    scalarProcessor.Times(i, CurveSectionLength.ScalarValue).ScalarValue
                );
                var length2 = scalarProcessor.Add(
                    LengthRange.MinValue.ScalarValue,
                    scalarProcessor.Times(i + 1, CurveSectionLength.ScalarValue).ScalarValue
                );
                return ScalarRange<T>.Create(
                    ArcLengthCurve.LengthToTime(length1),
                    ArcLengthCurve.LengthToTime(length2)
                );
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
                var length = scalarProcessor.Add(
                    LengthRange.MinValue.ScalarValue,
                    scalarProcessor.Times(i, CurveSectionLength.ScalarValue).ScalarValue
                );
                return Curve.GetValue(
                    ArcLengthCurve.LengthToTime(length)
                );
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
                var length = scalarProcessor.Add(
                    LengthRange.MinValue.ScalarValue,
                    scalarProcessor.Times(i, CurveSectionLength.ScalarValue).ScalarValue
                );
                return Curve.GetDerivative1Value(
                    ArcLengthCurve.LengthToTime(length)
                );
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
                var length = scalarProcessor.Add(
                    LengthRange.MinValue.ScalarValue,
                    scalarProcessor.Times(i, CurveSectionLength.ScalarValue).ScalarValue
                );
                return Curve.GetFrame(
                    ArcLengthCurve.LengthToTime(length)
                );
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
