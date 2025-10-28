using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Bezier;

/// <summary>
/// A parametric 3D path representing an arbitrary-degree Bezier curve.
/// Degree = N-1 where N is the number of control points.
/// </summary>
/// <typeparam name="T">Scalar type for time parameter</typeparam>
public sealed class BezierNPath3D<T> :
    ParametricPath3D<T>
{
    /// <summary>
    /// Gets the list of control points defining the Bezier curve.
    /// Modifying this list will affect the curve shape.
    /// </summary>
    public List<LinVector3D<T>> ControlPoints { get; }

    /// <summary>
    /// Gets the degree of the Bezier curve (number of control points - 1).
    /// Degree 0 = constant, 1 = linear, 2 = quadratic, 3 = cubic, etc.
    /// </summary>
    public int Degree
        => ControlPoints.Count - 1;

    private IScalarProcessor<T> ScalarProcessor
        => TimeRange.MinValue.ScalarProcessor;


    /// <summary>
    /// Creates an arbitrary-degree Bezier curve with no control points initially.
    /// Control points must be added to the ControlPoints list.
    /// </summary>
    /// <param name="scalarProcessor">Scalar processor for the type T</param>
    /// <param name="isPeriodic">Whether the path is periodic</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BezierNPath3D(IScalarProcessor<T> scalarProcessor, bool isPeriodic)
        : base(ScalarRange<T>.ZeroToOne(scalarProcessor), isPeriodic)
    {
        ControlPoints = new List<LinVector3D<T>>();

        Debug.Assert(IsValid());
    }

    /// <summary>
    /// Creates an arbitrary-degree Bezier curve with the given control points.
    /// </summary>
    /// <param name="scalarProcessor">Scalar processor for the type T</param>
    /// <param name="isPeriodic">Whether the path is periodic</param>
    /// <param name="controlPoints">Control points defining the curve</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BezierNPath3D(IScalarProcessor<T> scalarProcessor, bool isPeriodic, IEnumerable<LinVector3D<T>> controlPoints)
        : base(ScalarRange<T>.ZeroToOne(scalarProcessor), isPeriodic)
    {
        ControlPoints = new List<LinVector3D<T>>(controlPoints);

        Debug.Assert(IsValid());
    }

    /// <summary>
    /// Creates a Finite BezierN curve with given control points.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BezierNPath3D<T> Finite(IScalarProcessor<T> scalarProcessor, params LinVector3D<T>[] controlPoints)
    {
        return new BezierNPath3D<T>(scalarProcessor, false, controlPoints);
    }

    /// <summary>
    /// Creates a Finite BezierN curve with given control points.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BezierNPath3D<T> Finite(IScalarProcessor<T> scalarProcessor, IEnumerable<LinVector3D<T>> controlPoints)
    {
        return new BezierNPath3D<T>(scalarProcessor, false, controlPoints);
    }

    /// <summary>
    /// Creates a Periodic BezierN curve with given control points.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BezierNPath3D<T> Periodic(IScalarProcessor<T> scalarProcessor, params LinVector3D<T>[] controlPoints)
    {
        return new BezierNPath3D<T>(scalarProcessor, true, controlPoints);
    }

    /// <summary>
    /// Creates a Periodic BezierN curve with given control points.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BezierNPath3D<T> Periodic(IScalarProcessor<T> scalarProcessor, IEnumerable<LinVector3D<T>> controlPoints)
    {
        return new BezierNPath3D<T>(scalarProcessor, true, controlPoints);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return TimeRange.IsValid() &&
               ControlPoints.All(p => p.IsValid());
    }

    /// <summary>
    /// Evaluates the Bezier curve at parameter t using De Casteljau's algorithm.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetValue(Scalar<T> t)
    {
        if (ControlPoints.Count == 0)
            return LinVector3D<T>.Zero(ScalarProcessor);

        return t.DeCasteljau(ControlPoints.ToArray());
    }

    /// <summary>
    /// Gets the derivative curve of this Bezier curve.
    /// The derivative of a degree-N Bezier is a degree-(N-1) Bezier.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BezierNPath3D<T> GetDerivativeCurve()
    {
        if (Degree == 0)
            return new BezierNPath3D<T>(ScalarProcessor, IsPeriodic, Array.Empty<LinVector3D<T>>());

        var degreeScalar = ScalarProcessor.ScalarFromNumber(Degree);
        var derivativePoints = new List<LinVector3D<T>>(Degree);

        for (var i = 0; i < Degree; i++)
        {
            var diff = ControlPoints[i + 1] - ControlPoints[i];
            derivativePoints.Add(degreeScalar * diff);
        }

        return new BezierNPath3D<T>(ScalarProcessor, IsPeriodic, derivativePoints);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative1Value(Scalar<T> t)
    {
        var derivativeCurve = GetDerivativeCurve();
        return derivativeCurve.GetValue(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override LinVector3D<T> GetDerivative2Value(Scalar<T> t)
    {
        var derivativeCurve = GetDerivativeCurve().GetDerivativeCurve();
        return derivativeCurve.GetValue(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToFinitePath()
    {
        return IsFinite
            ? this
            : new BezierNPath3D<T>(ScalarProcessor, false, ControlPoints);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override ParametricPath3D<T> ToPeriodicPath()
    {
        return IsPeriodic
            ? this
            : new BezierNPath3D<T>(ScalarProcessor, true, ControlPoints);
    }
}
