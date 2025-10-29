using GeometricAlgebraFulcrumLib.Algebra;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Quaternions.Generic;

/// <summary>
/// A parametric 4D quaternion curve with continuous first derivative
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public interface IParametricQuaternion<T> :
    IAlgebraicElement
{
    IScalarProcessor<T> ScalarProcessor { get; }

    ScalarRange<T> ParameterRange { get; }

    LinQuaternion<T> GetQuaternion(Scalar<T> parameterValue);

    LinQuaternion<T> GetDerivative1Quaternion(Scalar<T> parameterValue);
}
