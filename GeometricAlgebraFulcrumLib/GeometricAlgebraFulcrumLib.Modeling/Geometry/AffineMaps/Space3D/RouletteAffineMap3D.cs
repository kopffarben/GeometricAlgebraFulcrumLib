using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Matrices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.AffineMaps.Space3D;

/// <summary>
/// Generic counterpart to Float64RouletteAffineMap3D. Maps points/vectors/normals via translation+rotation.
/// </summary>
public sealed record RouletteAffineMap3D<T>
{
    public LinVector3D<T> FixedFrameOrigin { get; }

    public LinVector3D<T> MovingFrameOrigin { get; }

    public LinQuaternion<T> RotationQuaternion { get; }

    public IScalarProcessor<T> ScalarProcessor => FixedFrameOrigin.ScalarProcessor;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RouletteAffineMap3D(
        LinVector3D<T> fixedFrameOrigin,
        LinVector3D<T> movingFrameOrigin,
        LinQuaternion<T> rotationQuaternion)
    {
        FixedFrameOrigin = fixedFrameOrigin;
        MovingFrameOrigin = movingFrameOrigin;
        RotationQuaternion = rotationQuaternion;

        Debug.Assert(IsValid());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        return FixedFrameOrigin.IsValid() &&
               MovingFrameOrigin.IsValid() &&
               RotationQuaternion.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsIdentity()
    {
        return FixedFrameOrigin.IsZero() &&
               MovingFrameOrigin.IsZero() &&
               RotationQuaternion.IsIdentity();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LinVector3D<T> MapPoint(LinVector3D<T> point)
    {
        return FixedFrameOrigin +
               RotationQuaternion.RotateVector(point - MovingFrameOrigin);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LinVector3D<T> MapVector(LinVector3D<T> vector)
    {
        return RotationQuaternion.RotateVector(vector);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LinVector3D<T> MapNormal(LinVector3D<T> normal)
    {
        return RotationQuaternion.RotateVector(normal);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RouletteAffineMap3D<T> GetInverse()
    {
        return new RouletteAffineMap3D<T>(
            MovingFrameOrigin,
            FixedFrameOrigin,
            RotationQuaternion.Inverse()
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SquareMatrix4<T> GetSquareMatrix4()
    {
        var (c1, c2, c3) = RotationQuaternion.RotateBasisVectors();
        var c4 = MapPoint(LinVector3D<T>.Zero(ScalarProcessor));

        return new SquareMatrix4<T>(ScalarProcessor)
        {
            Scalar00 = c1.X,
            Scalar10 = c1.Y,
            Scalar20 = c1.Z,
            Scalar01 = c2.X,
            Scalar11 = c2.Y,
            Scalar21 = c2.Z,
            Scalar02 = c3.X,
            Scalar12 = c3.Y,
            Scalar22 = c3.Z,
            Scalar03 = c4.X,
            Scalar13 = c4.Y,
            Scalar23 = c4.Z,
            Scalar33 = ScalarProcessor.One
        };
    }
}
