using System.Runtime.CompilerServices;

namespace GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32.Vectors.Space3D;

/// <summary>
/// Manual extensions to complement auto-generated LinFloat32Vector3DComposerUtils
/// Provides double-to-float conversion overloads for MathNet.Numerics interoperability
/// </summary>
public static class LinFloat32Vector3DComposerUtilsExtensions
{
    /// <summary>
    /// Convert IEnumerable&lt;double&gt; to LinFloat32Vector3D
    /// This overload is needed for MathNet.Numerics Vector&lt;Complex&gt;.Real() and Imaginary()
    /// which return Vector&lt;double&gt; even in float contexts
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinFloat32Vector3D ToLinFloat32Vector3D(this IEnumerable<double> scalarList, bool makeUnit = false)
    {
        var scalarArray = new float[3];

        var i = 0;
        foreach (var scalar in scalarList)
        {
            if (i >= 3) break;
            scalarArray[i] = (float)scalar;
            i++;
        }

        if (!makeUnit)
            return LinFloat32Vector3D.Create(scalarArray[0], scalarArray[1], scalarArray[2]);

        var s = MathF.Sqrt(scalarArray[0] * scalarArray[0] + scalarArray[1] * scalarArray[1] + scalarArray[2] * scalarArray[2]);

        if (s == 0f)
            return LinFloat32Vector3D.E1;

        s = 1.0f / s;
        return LinFloat32Vector3D.Create(scalarArray[0] * s, scalarArray[1] * s, scalarArray[2] * s);
    }
}
