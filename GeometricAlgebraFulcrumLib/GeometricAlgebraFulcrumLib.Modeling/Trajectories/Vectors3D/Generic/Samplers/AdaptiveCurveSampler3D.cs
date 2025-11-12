using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Adaptive;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Composers;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Samplers;

/// <summary>
/// Samples a parametric curve using adaptive refinement based on curvature.
/// Automatically refines sampling density in regions of high curvature.
/// </summary>
/// <typeparam name="T">The scalar type (double, float, etc.)</typeparam>
public class AdaptiveCurveSampler3D<T> :
    IParametricCurveSampler3D<T>
{
    public ParametricPath3D<T> Curve { get; private set; }

    public ScalarRange<T> ParameterRange { get; private set; }

    public bool IsPeriodic { get; private set; }

    public AdaptivePath3DSamplingOptions<T> SamplingOptions { get; private set; }

    public AdaptivePath3D<T> SampledCurve { get; private set; }

    public int Count
        => SampledCurve.Count;

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

            return SampledCurve[index];
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AdaptiveCurveSampler3D(ParametricPath3D<T> curve, ScalarRange<T> parameterRange, AdaptivePath3DSamplingOptions<T> samplingOptions, bool isPeriodic = false)
    {
        SamplingOptions = samplingOptions;
        IsPeriodic = isPeriodic;
        Curve = curve;
        ParameterRange = parameterRange;
        SampledCurve = curve.CreateAdaptiveCurve3D(parameterRange, samplingOptions);

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        return ParameterRange.IsValid() &&
               Curve.IsValid() &&
               SampledCurve.IsValid() &&
               (IsPeriodic && Count > 0 || !IsPeriodic && Count > 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AdaptiveCurveSampler3D<T> SetCurve(ParametricPath3D<T> curve, ScalarRange<T> parameterRange, AdaptivePath3DSamplingOptions<T> samplingOptions, bool isPeriodic)
    {
        Curve = curve;
        ParameterRange = parameterRange;
        SamplingOptions = samplingOptions;
        IsPeriodic = isPeriodic;
        SampledCurve = curve.CreateAdaptiveCurve3D(parameterRange, samplingOptions);

        Debug.Assert(IsValid());

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<Scalar<T>> GetParameterValues()
    {
        return SampledCurve.GetTimeValues();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ScalarRange<T>> GetParameterSections()
    {
        var tValues = SampledCurve.GetTimeValues().ToImmutableArray();

        var t1 = tValues[0];
        for (var i = 1; i < tValues.Length; i++)
        {
            var t2 = tValues[i];

            yield return ScalarRange<T>.Create(t1, t2);

            t1 = t2;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<LinVector3D<T>> GetPoints()
    {
        return SampledCurve.Select(f => f.Point);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<LinVector3D<T>> GetTangents()
    {
        return SampledCurve.Select(f => f.Tangent);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ParametricPath3DLocalFrame<T>> GetFrames()
    {
        return SampledCurve;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<ParametricPath3DLocalFrame<T>> GetEnumerator()
    {
        return SampledCurve.GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
