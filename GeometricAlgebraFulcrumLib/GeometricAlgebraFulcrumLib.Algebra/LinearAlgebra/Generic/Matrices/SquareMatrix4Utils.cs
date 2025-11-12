using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Matrices;

public static class SquareMatrix4Utils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinVector3D<T> MapAffineVector<T>(this SquareMatrix4<T> matrix, ILinVector3D<T> vector)
    {
        var sp = matrix.ScalarProcessor;

        // Affine transformation: rotate/scale without translation
        var term00 = sp.Times(matrix.Scalar00.ScalarValue, vector.X.ScalarValue);
        var term01 = sp.Times(matrix.Scalar01.ScalarValue, vector.Y.ScalarValue);
        var term02 = sp.Times(matrix.Scalar02.ScalarValue, vector.Z.ScalarValue);
        var x = sp.Add(sp.Add(term00.ScalarValue, term01.ScalarValue).ScalarValue, term02.ScalarValue);

        var term10 = sp.Times(matrix.Scalar10.ScalarValue, vector.X.ScalarValue);
        var term11 = sp.Times(matrix.Scalar11.ScalarValue, vector.Y.ScalarValue);
        var term12 = sp.Times(matrix.Scalar12.ScalarValue, vector.Z.ScalarValue);
        var y = sp.Add(sp.Add(term10.ScalarValue, term11.ScalarValue).ScalarValue, term12.ScalarValue);

        var term20 = sp.Times(matrix.Scalar20.ScalarValue, vector.X.ScalarValue);
        var term21 = sp.Times(matrix.Scalar21.ScalarValue, vector.Y.ScalarValue);
        var term22 = sp.Times(matrix.Scalar22.ScalarValue, vector.Z.ScalarValue);
        var z = sp.Add(sp.Add(term20.ScalarValue, term21.ScalarValue).ScalarValue, term22.ScalarValue);

        return LinVector3D<T>.Create(x, y, z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Pair<LinVector3D<T>> MapAffineVectors<T>(this SquareMatrix4<T> matrix, ILinVector3D<T> vector1, ILinVector3D<T> vector2)
    {
        return new Pair<LinVector3D<T>>(
            matrix.MapAffineVector(vector1),
            matrix.MapAffineVector(vector2)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Triplet<LinVector3D<T>> MapAffineVectors<T>(this SquareMatrix4<T> matrix, ILinVector3D<T> vector1, ILinVector3D<T> vector2, ILinVector3D<T> vector3)
    {
        return new Triplet<LinVector3D<T>>(
            matrix.MapAffineVector(vector1),
            matrix.MapAffineVector(vector2),
            matrix.MapAffineVector(vector3)
        );
    }
}
