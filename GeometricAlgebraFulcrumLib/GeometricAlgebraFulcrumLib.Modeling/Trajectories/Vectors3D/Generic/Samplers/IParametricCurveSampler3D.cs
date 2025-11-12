using GeometricAlgebraFulcrumLib.Algebra;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Samplers;

/// <summary>
/// Interface for sampling parametric curves in 3D space.
/// Provides methods to extract points, tangents, and local frames from a curve.
/// </summary>
/// <typeparam name="T">The scalar type (double, float, etc.)</typeparam>
public interface IParametricCurveSampler3D<T> :
    IAlgebraicElement,
    IReadOnlyList<ParametricPath3DLocalFrame<T>>
{
    /// <summary>
    /// The curve being sampled
    /// </summary>
    ParametricPath3D<T> Curve { get; }

    /// <summary>
    /// The parameter range for sampling
    /// </summary>
    ScalarRange<T> ParameterRange { get; }

    /// <summary>
    /// True if the curve is periodic (closed loop)
    /// </summary>
    bool IsPeriodic { get; }

    /// <summary>
    /// Get the parameter values at which the curve is sampled
    /// </summary>
    IEnumerable<Scalar<T>> GetParameterValues();

    /// <summary>
    /// Get the parameter sections (ranges between consecutive samples)
    /// </summary>
    IEnumerable<ScalarRange<T>> GetParameterSections();

    /// <summary>
    /// Get the 3D points along the curve at the sample parameters
    /// </summary>
    IEnumerable<LinVector3D<T>> GetPoints();

    /// <summary>
    /// Get the tangent vectors along the curve at the sample parameters
    /// </summary>
    IEnumerable<LinVector3D<T>> GetTangents();

    /// <summary>
    /// Get the complete local frames (Tangent, Normal, Binormal) at the sample parameters
    /// </summary>
    IEnumerable<ParametricPath3DLocalFrame<T>> GetFrames();
}
