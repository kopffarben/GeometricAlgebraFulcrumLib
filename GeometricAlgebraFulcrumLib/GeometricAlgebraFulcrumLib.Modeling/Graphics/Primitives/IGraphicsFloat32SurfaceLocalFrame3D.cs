using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32.Frames.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32.Vectors.Space3D;
using SixLabors.ImageSharp;

namespace GeometricAlgebraFulcrumLib.Modeling.Graphics.Primitives;

public interface IGraphicsFloat32SurfaceLocalFrame3D :
    ILinFloat32Vector3D
{
    int Index { get; }

    LinFloat32Vector3D Point { get; }

    Color Color { get; set; }

    LinFloat32Vector2D ParameterValue { get; }

    LinFloat32Normal3D Normal { get; }
}
