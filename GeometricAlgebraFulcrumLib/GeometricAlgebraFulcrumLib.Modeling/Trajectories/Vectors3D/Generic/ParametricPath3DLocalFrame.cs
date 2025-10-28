using System.Runtime.CompilerServices;
using System.Text;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;

/// <summary>
/// Represents a local frame (position, tangent, normal1, normal2) at a point on a parametric 3D path
/// SIMPLIFIED VERSION: This is a basic implementation with core features only.
/// Full feature set (rotation methods, etc.) can be added later if needed.
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed record ParametricPath3DLocalFrame<T>
{
    /// <summary>
    /// Create a local frame based on the tangent only.
    /// Normal1 and Normal2 are computed as an orthonormal pair perpendicular to the tangent.
    /// </summary>
    public static ParametricPath3DLocalFrame<T> Create(Scalar<T> timeValue, LinVector3D<T> point, LinVector3D<T> tangentVector)
    {
        var tangent = tangentVector.ToUnitLinVector3D();
        var (normal1, normal2) = tangent.GetUnitNormalPair();

        return new ParametricPath3DLocalFrame<T>(
            timeValue,
            point,
            tangent,
            normal1,
            normal2
        );
    }

    /// <summary>
    /// Create a local frame with explicit normal vectors
    /// </summary>
    public static ParametricPath3DLocalFrame<T> Create(Scalar<T> timeValue, LinVector3D<T> point, LinVector3D<T> tangent, LinVector3D<T> normal1, LinVector3D<T> normal2)
    {
        return new ParametricPath3DLocalFrame<T>(
            timeValue,
            point,
            tangent,
            normal1,
            normal2
        );
    }


    /// <summary>
    /// The curve parameter value at the given curve point
    /// </summary>
    public Scalar<T> TimeValue { get; }

    /// <summary>
    /// A point on the curve
    /// </summary>
    public LinVector3D<T> Point { get; }

    /// <summary>
    /// The tangent unit vector to the curve at the given curve point
    /// </summary>
    public LinVector3D<T> Tangent { get; }

    /// <summary>
    /// The first unit vector orthogonal to the tangent at the given curve point
    /// </summary>
    public LinVector3D<T> Normal1 { get; }

    /// <summary>
    /// The second unit vector orthogonal to the tangent at the given curve point
    /// </summary>
    public LinVector3D<T> Normal2 { get; }


    private ParametricPath3DLocalFrame(Scalar<T> timeValue, LinVector3D<T> point, LinVector3D<T> tangent, LinVector3D<T> normal1, LinVector3D<T> normal2)
    {
        TimeValue = timeValue;
        Point = point;
        Tangent = tangent;
        Normal1 = normal1;
        Normal2 = normal2;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
        return new StringBuilder()
            .AppendLine("Frame {")
            .AppendLine($"         t: {TimeValue}")
            .AppendLine($"     Point: {Point}")
            .AppendLine($"   Tangent: {Tangent}")
            .AppendLine($"   Normal1: {Normal1}")
            .AppendLine($"   Normal2: {Normal2}")
            .Append("}")
            .ToString();
    }
}
