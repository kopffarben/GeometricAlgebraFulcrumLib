using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;

public static class Path3DUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<Scalar<T>> GetTimeValues<T>(this IEnumerable<ParametricPath3DLocalFrame<T>> path)
    {
        return path.Select(f => f.TimeValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<LinVector3D<T>> GetPoints<T>(this IEnumerable<ParametricPath3DLocalFrame<T>> path)
    {
        return path.Select(f => f.Point);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<LinVector3D<T>> GetPoints<T>(this ParametricPath3D<T> path, IEnumerable<Scalar<T>> tList)
    {
        return tList.Select(path.GetValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinVector3D<T>[] GetPoints<T>(this ParametricPath3D<T> path, params Scalar<T>[] tList)
    {
        return tList.Select(path.GetValue).ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinVector3D<T> GetTangent<T>(this ParametricPath3D<T> path, Scalar<T> t)
    {
        return path.GetDerivative1Value(t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<LinVector3D<T>> GetTangents<T>(this IEnumerable<ParametricPath3DLocalFrame<T>> path)
    {
        return path.Select(f => f.Tangent);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<LinVector3D<T>> GetTangents<T>(this ParametricPath3D<T> path, IEnumerable<Scalar<T>> tList)
    {
        return tList.Select(path.GetDerivative1Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinVector3D<T>[] GetTangents<T>(this ParametricPath3D<T> path, params Scalar<T>[] tList)
    {
        return tList.Select(path.GetDerivative1Value).ToArray();
    }
}
