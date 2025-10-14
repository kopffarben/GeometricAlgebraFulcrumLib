using GeometricAlgebraFulcrumLib.Modeling.Graphics.Structures.Vertices;

namespace GeometricAlgebraFulcrumLib.Modeling.Graphics.Primitives.Vertices;

/// <summary>
/// This interface represents the information of a single vertex
/// like position, normal, color, or texture coordinates
/// </summary>
public interface IGraphicsFloat32Vertex3D :
    IGraphicsFloat32SurfaceLocalFrame3D
{
    bool HasParameterValue { get; }

    bool HasNormal { get; }

    bool HasColor { get; }

    GraphicsVertexDataKind3D DataKind { get; }
}
