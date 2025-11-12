namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.Parametric.Generic;

/// <summary>
/// Specifies the interpolation method for local frames along a parametric curve.
/// </summary>
public enum ParametricCurveLocalFrameInterpolationMethod
{
    /// <summary>
    /// Use linear interpolation for tangent vectors (faster, less accurate).
    /// </summary>
    TangentLinearInterpolation = 0,

    /// <summary>
    /// Use spherical linear interpolation for the entire frame (slower, more accurate).
    /// </summary>
    SphericalLinearInterpolation = 1
}
