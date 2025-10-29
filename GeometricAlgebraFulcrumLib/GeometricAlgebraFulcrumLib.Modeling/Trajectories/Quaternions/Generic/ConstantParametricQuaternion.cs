using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Quaternions.Generic;

/// <summary>
/// Constant quaternion trajectory returning the same quaternion for all parameter values
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class ConstantParametricQuaternion<T> :
    IParametricQuaternion<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantParametricQuaternion<T> Create(IScalarProcessor<T> scalarProcessor, LinQuaternion<T> point)
    {
        return new ConstantParametricQuaternion<T>(
            scalarProcessor,
            point,
            LinQuaternion<T>.Identity(scalarProcessor)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantParametricQuaternion<T> Create(IScalarProcessor<T> scalarProcessor, LinQuaternion<T> point, LinQuaternion<T> tangent)
    {
        return new ConstantParametricQuaternion<T>(
            scalarProcessor,
            point,
            tangent
        );
    }


    public IScalarProcessor<T> ScalarProcessor { get; }

    public LinQuaternion<T> Point { get; }

    public LinQuaternion<T> Tangent { get; }

    public ScalarRange<T> ParameterRange
        => ScalarRange<T>.Infinite(ScalarProcessor);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ConstantParametricQuaternion(IScalarProcessor<T> scalarProcessor, LinQuaternion<T> point, LinQuaternion<T> tangent)
    {
        ScalarProcessor = scalarProcessor;
        Point = point;
        Tangent = tangent;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        return Point.IsValid() &&
               Tangent.IsValid();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LinQuaternion<T> GetQuaternion(Scalar<T> parameterValue)
    {
        return Point;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LinQuaternion<T> GetDerivative1Quaternion(Scalar<T> parameterValue)
    {
        return Tangent;
    }
}
