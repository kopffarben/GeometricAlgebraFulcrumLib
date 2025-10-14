using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float32;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32.Vectors.Space3D;

public interface ILinFloat32Vector3D :
    IFloat32LinearAlgebraElement,
    ITriplet<Float32Scalar>
{
    Float32Scalar X { get; }

    Float32Scalar Y { get; }

    Float32Scalar Z { get; }
}
