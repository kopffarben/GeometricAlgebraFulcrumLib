namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.Parametric.Generic;

/// <summary>
/// Specifies the method for computing normal vectors when sampling local frames along a parametric curve.
/// </summary>
public enum ParametricCurveLocalFrameSamplingMethod
{
    /// <summary>
    /// Use simple rotation to align normals between consecutive frames.
    /// Faster but may accumulate error over long paths.
    /// </summary>
    SimpleRotation = 0,

    /// <summary>
    /// Use rotation-minimizing frames to minimize normal rotation.
    /// Slower but more stable for long paths with high curvature.
    /// </summary>
    MinimizedRotation = 1
}
