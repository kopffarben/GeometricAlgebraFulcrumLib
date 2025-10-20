using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Random;

namespace GeometricAlgebraFulcrumLib.Algebra.Scalars.Float32;

public class Float32RandomComposer
{
    public Random RandomGenerator { get; }

    public float MinScalarValue { get; private set; } = -1f;

    public float MaxScalarValue { get; private set; } = 1f;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32RandomComposer()
    {
        RandomGenerator = new Random();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32RandomComposer(int seed)
    {
        RandomGenerator = new Random(seed);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32RandomComposer(Random randomGenerator)
    {
        RandomGenerator = randomGenerator;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetScalarLimits(float minScalarValue, float maxScalarValue)
    {
        if (float.IsNaN(minScalarValue) || float.IsInfinity(minScalarValue) ||
            float.IsNaN(maxScalarValue) || float.IsInfinity(maxScalarValue))
            throw new ArgumentException();

        if (minScalarValue <= maxScalarValue)
        {
            MinScalarValue = minScalarValue;
            MaxScalarValue = maxScalarValue;
        }
        else
        {
            MinScalarValue = maxScalarValue;
            MaxScalarValue = minScalarValue;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetScalarValue()
    {
        return RandomGenerator.GetFloat32(MinScalarValue, MaxScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetScalarValue(float minValue, float maxValue)
    {
        return RandomGenerator.GetFloat32(minValue, maxValue);
    }
}
