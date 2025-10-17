using System.Runtime.CompilerServices;

namespace GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32.Matrices;

/// <summary>
/// Manual extensions for MatrixUtils to handle double[] to float[] conversions
/// </summary>
public static class MatrixUtilsExtensions
{
    /// <summary>
    /// Convert double[] to float[]
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ToFloatArray(this double[] array)
    {
        var result = new float[array.Length];
        for (int i = 0; i < array.Length; i++)
            result[i] = (float)array[i];
        return result;
    }

    /// <summary>
    /// Convert MathNet.Numerics.LinearAlgebra.Vector&lt;double&gt; to float[]
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ToFloatArray(this MathNet.Numerics.LinearAlgebra.Vector<double> vector)
    {
        var result = new float[vector.Count];
        for (int i = 0; i < vector.Count; i++)
            result[i] = (float)vector[i];
        return result;
    }
}
