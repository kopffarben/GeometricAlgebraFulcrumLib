using System.Runtime.CompilerServices;
using System.Text;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Matrices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;

/// <summary>
/// Represents a local frame (position, tangent, normal1, normal2) at a point on a parametric 3D path
/// Includes helpers to rotate normals similar to Float64Path3DLocalFrame.
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class ParametricPath3DLocalFrame<T>
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
    public LinVector3D<T> Normal1 { get; private set; }

    /// <summary>
    /// The second unit vector orthogonal to the tangent at the given curve point
    /// </summary>
    public LinVector3D<T> Normal2 { get; private set; }


    private ParametricPath3DLocalFrame(Scalar<T> timeValue, LinVector3D<T> point, LinVector3D<T> tangent, LinVector3D<T> normal1, LinVector3D<T> normal2)
    {
        TimeValue = timeValue;
        Point = point;
        Tangent = tangent;
        Normal1 = normal1;
        Normal2 = normal2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ParametricPath3DLocalFrame<T> UpdateNormals(LinVector3D<T> normal1, LinVector3D<T> normal2)
    {
        Normal1 = normal1;
        Normal2 = normal2;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ParametricPath3DLocalFrame<T> SetSimpleRotationNormals(ParametricPath3DLocalFrame<T> sourceFrame)
    {
        var (normal1, normal2) = sourceFrame.RotateNormalsByTangent(Tangent);
        return UpdateNormals(normal1, normal2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ParametricPath3DLocalFrame<T> SetMinimizedRotationNormals(ParametricPath3DLocalFrame<T> sourceFrame)
    {
        var planeNormal1 = Point - sourceFrame.Point;

        var (normal1L, normal2L, tangentL) =
            planeNormal1.ReflectVectorsOnVector(
                sourceFrame.Normal1,
                sourceFrame.Normal2,
                sourceFrame.Tangent
            );

        var planeNormal2 = Tangent - tangentL;

        var (normal1, normal2) =
            planeNormal2.ReflectVectorsOnVector(normal1L, normal2L);

        return UpdateNormals(normal1, normal2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Pair<LinVector3D<T>> RotateNormalsByTangent(LinVector3D<T> newTangent)
    {
        var matrix = SquareMatrix3<T>.CreateVectorToVectorRotationMatrix3D(Tangent, newTangent);

        var newNormal1 = matrix * Normal1;
        var newNormal2 = matrix * Normal2;

        return new Pair<LinVector3D<T>>(newNormal1, newNormal2);
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
