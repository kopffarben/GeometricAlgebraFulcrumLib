using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Samplers;

/// <summary>
/// Samples a constant curve (stationary point) at exactly 2 points: min and max parameter values.
/// </summary>
/// <typeparam name="T">The scalar type (double, float, etc.)</typeparam>
public class ConstantCurveSampler3D<T> :
    IParametricCurveSampler3D<T>
{
    public ConstantPath3D<T> ConstantCurve { get; private set; }

    public ParametricPath3D<T> Curve
        => ConstantCurve;

    public LinVector3D<T> Point
        => ConstantCurve.Point;

    public LinVector3D<T> Tangent
        => ConstantCurve.Tangent;

    public ScalarRange<T> ParameterRange { get; private set; }

    public bool IsPeriodic
        => true;

    public int Count
        => 2;

    public ParametricPath3DLocalFrame<T> this[int index]
    {
        get
        {
            var parameterValue =
                index % 2 == 0
                    ? ParameterRange.MinValue
                    : ParameterRange.MaxValue;

            return ConstantCurve.GetFrame(parameterValue);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConstantCurveSampler3D(LinVector3D<T> point, ScalarRange<T> timeRange)
    {
        var scalarProcessor = point.ScalarProcessor;
        ConstantCurve = ConstantPath3D<T>.Finite(
            timeRange,
            point,
            LinVector3D<T>.E1(scalarProcessor)
        );
        ParameterRange = timeRange;

        Debug.Assert(IsValid());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConstantCurveSampler3D(LinVector3D<T> point, LinVector3D<T> tangent, ScalarRange<T> timeRange)
    {
        ConstantCurve = ConstantPath3D<T>.Finite(timeRange, point, tangent);
        ParameterRange = timeRange;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        return ParameterRange.IsValid() &&
               ConstantCurve.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConstantCurveSampler3D<T> SetCurve(LinVector3D<T> point, ScalarRange<T> timeRange)
    {
        var scalarProcessor = point.ScalarProcessor;
        ConstantCurve = ConstantPath3D<T>.Finite(
            timeRange,
            point,
            LinVector3D<T>.E1(scalarProcessor)
        );
        ParameterRange = timeRange;

        Debug.Assert(IsValid());

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConstantCurveSampler3D<T> SetCurve(LinVector3D<T> point, LinVector3D<T> tangent, ScalarRange<T> timeRange)
    {
        ConstantCurve = ConstantPath3D<T>.Finite(timeRange, point, tangent);
        ParameterRange = timeRange;

        Debug.Assert(IsValid());

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<Scalar<T>> GetParameterValues()
    {
        yield return ParameterRange.MinValue;
        yield return ParameterRange.MaxValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ScalarRange<T>> GetParameterSections()
    {
        yield return ParameterRange;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<LinVector3D<T>> GetPoints()
    {
        yield return Point;
        yield return Point;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<LinVector3D<T>> GetTangents()
    {
        yield return Tangent;
        yield return Tangent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<ParametricPath3DLocalFrame<T>> GetFrames()
    {
        yield return ConstantCurve.GetFrame(ParameterRange.MinValue);
        yield return ConstantCurve.GetFrame(ParameterRange.MaxValue);
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
