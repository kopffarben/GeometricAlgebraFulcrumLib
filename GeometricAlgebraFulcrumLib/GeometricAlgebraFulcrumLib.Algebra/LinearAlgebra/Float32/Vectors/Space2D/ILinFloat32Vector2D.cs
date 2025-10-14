using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float32;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32.Vectors.Space2D;

public interface ILinFloat32Vector2D :
    IFloat32LinearAlgebraElement,
    IPair<Float32Scalar>
{
    Float32Scalar X { get; }

    Float32Scalar Y { get; }
}
