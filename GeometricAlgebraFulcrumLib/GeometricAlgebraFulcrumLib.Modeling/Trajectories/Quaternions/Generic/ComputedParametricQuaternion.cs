using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Quaternions.Generic;

/// <summary>
/// Parametric quaternion trajectory computed from user-provided functions
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public class ComputedParametricQuaternion<T> :
    IParametricQuaternion<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedParametricQuaternion<T> Create(IScalarProcessor<T> scalarProcessor, ScalarRange<T> parameterRange, Func<Scalar<T>, LinQuaternion<T>> getPointFunc)
    {
        return new ComputedParametricQuaternion<T>(
            scalarProcessor,
            parameterRange,
            getPointFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedParametricQuaternion<T> Create(IScalarProcessor<T> scalarProcessor, Func<Scalar<T>, LinQuaternion<T>> getPointFunc)
    {
        return new ComputedParametricQuaternion<T>(
            scalarProcessor,
            ScalarRange<T>.Infinite(scalarProcessor),
            getPointFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedParametricQuaternion<T> Create(IScalarProcessor<T> scalarProcessor, ScalarRange<T> parameterRange, Func<Scalar<T>, LinQuaternion<T>> getPointFunc, Func<Scalar<T>, LinQuaternion<T>> getTangentFunc)
    {
        return new ComputedParametricQuaternion<T>(
            scalarProcessor,
            parameterRange,
            getPointFunc,
            getTangentFunc
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComputedParametricQuaternion<T> Create(IScalarProcessor<T> scalarProcessor, Func<Scalar<T>, LinQuaternion<T>> getPointFunc, Func<Scalar<T>, LinQuaternion<T>> getTangentFunc)
    {
        return new ComputedParametricQuaternion<T>(
            scalarProcessor,
            ScalarRange<T>.Infinite(scalarProcessor),
            getPointFunc,
            getTangentFunc
        );
    }

    // Note: DifferentialFunction<T> and component-wise numerical differentiation factory methods omitted
    // - DifferentialFunction<T> doesn't exist yet (Float64-only infrastructure)
    // - Component-wise numerical diff requires MathNet.Numerics which doesn't support generic T
    // Use Create(getPointFunc, getTangentFunc) instead to provide explicit derivative function


    public IScalarProcessor<T> ScalarProcessor { get; }

    public Func<Scalar<T>, LinQuaternion<T>> GetPointFunc { get; }

    public Func<Scalar<T>, LinQuaternion<T>>? GetTangentFunc { get; }

    public ScalarRange<T> ParameterRange { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ComputedParametricQuaternion(IScalarProcessor<T> scalarProcessor, ScalarRange<T> parameterRange, Func<Scalar<T>, LinQuaternion<T>> getPointFunc)
    {
        ScalarProcessor = scalarProcessor;
        ParameterRange = parameterRange;
        GetPointFunc = getPointFunc;
        GetTangentFunc = null;

        Debug.Assert(IsValid());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ComputedParametricQuaternion(IScalarProcessor<T> scalarProcessor, ScalarRange<T> parameterRange, Func<Scalar<T>, LinQuaternion<T>> getPointFunc, Func<Scalar<T>, LinQuaternion<T>> getTangentFunc)
    {
        ScalarProcessor = scalarProcessor;
        ParameterRange = parameterRange;
        GetPointFunc = getPointFunc;
        GetTangentFunc = getTangentFunc;

        Debug.Assert(IsValid());
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LinQuaternion<T> GetQuaternion(Scalar<T> parameterValue)
    {
        return GetPointFunc(parameterValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LinQuaternion<T> GetDerivative1Quaternion(Scalar<T> parameterValue)
    {
        if (GetTangentFunc is not null)
            return GetTangentFunc(parameterValue);

        const double zeroEpsilon = 1e-7;
        var epsilon = ScalarProcessor.ScalarFromNumber(zeroEpsilon);

        var p1 = GetPointFunc(ScalarProcessor.Subtract(parameterValue.ScalarValue, epsilon.ScalarValue));
        var p2 = GetPointFunc(ScalarProcessor.Add(parameterValue.ScalarValue, epsilon.ScalarValue));

        return (p2 - p1) / (2 * zeroEpsilon);
    }
}
