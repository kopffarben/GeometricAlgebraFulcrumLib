using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Bezier;

/// <summary>
/// A Bezier curve of arbitrary degree N using De Casteljau algorithm
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class BezierNPath2D<T> :
    ParametricPath2D<T>
{
    public List<LinVector2D<T>> ControlPoints { get; }
        = new List<LinVector2D<T>>();

    public int Degree
        => ControlPoints.Count - 1;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BezierNPath2D(ScalarRange<T> timeRange, bool isPeriodic)
        : base(timeRange, isPeriodic)
    {
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return ControlPoints.All(p => p.IsValid());
    }

    /// <summary>
    /// Compute the derivative curve of this Bezier curve.
    /// The derivative of a degree-n Bezier curve is a degree-(n-1) Bezier curve.
    /// </summary>
    public BezierNPath2D<T> GetDerivativeCurve()
    {
        var result = new BezierNPath2D<T>(TimeRange, IsPeriodic);

        if (Degree < 1)
            return result;

        var scalarProcessor = ControlPoints[0].ScalarProcessor;
        var degreeScalar = scalarProcessor.ScalarFromNumber(Degree);

        for (var n = 0; n < Degree; n++)
        {
            var diff = ControlPoints[n + 1] - ControlPoints[n];
            result.ControlPoints.Add(degreeScalar * diff);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetValue(Scalar<T> t)
    {
        return t.DeCasteljau(ControlPoints.ToArray());
    }

    public override ParametricPath2D<T> ToFinitePath()
    {
        if (IsFinite)
            return this;

        var result = new BezierNPath2D<T>(TimeRange, false);
        result.ControlPoints.AddRange(ControlPoints);
        return result;
    }

    public override ParametricPath2D<T> ToPeriodicPath()
    {
        if (IsPeriodic)
            return this;

        var result = new BezierNPath2D<T>(TimeRange, true);
        result.ControlPoints.AddRange(ControlPoints);
        return result;
    }

    public override LinVector2D<T> GetDerivative1Value(Scalar<T> t)
    {
        if (Degree < 1)
            return LinVector2D<T>.Zero(t.ScalarProcessor);

        var derivativeCurve = GetDerivativeCurve();
        return derivativeCurve.GetValue(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector2D<T> GetDerivative2Value(Scalar<T> t)
    {
        if (Degree < 2)
            return LinVector2D<T>.Zero(t.ScalarProcessor);

        var derivativeCurve = GetDerivativeCurve();
        var secondDerivativeCurve = derivativeCurve.GetDerivativeCurve();
        return secondDerivativeCurve.GetValue(t);
    }
}
