using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace GeometricAlgebraFulcrumLib.Algebra.Scalars.Float32;

public readonly struct Float32ScalarRange :
    IAlgebraicElement
{
    public static Float32ScalarRange Infinite { get; }
        = new Float32ScalarRange(
            float.NegativeInfinity,
            float.PositiveInfinity
        );

    public static Float32ScalarRange ZeroToInfinity { get; }
        = new Float32ScalarRange(
            0f,
            float.PositiveInfinity
        );

    public static Float32ScalarRange InfinityToZero { get; }
        = new Float32ScalarRange(
            float.NegativeInfinity,
            0f
        );
    
    public static Float32ScalarRange ZeroToOne { get; }
        = new Float32ScalarRange(0f, 1f);

    public static Float32ScalarRange ZeroToPi { get; }
        = new Float32ScalarRange(0f, MathF.PI);

    public static Float32ScalarRange ZeroToTwoPi { get; }
        = new Float32ScalarRange(0f, MathF.Tau);

    public static Float32ScalarRange NegativeOneToZero { get; }
        = new Float32ScalarRange(-1f, 0f);

    public static Float32ScalarRange SymmetricOne { get; }
        = new Float32ScalarRange(-1f, 1f);
    
    public static Float32ScalarRange SymmetricHalfPi { get; }
        = new Float32ScalarRange(-MathF.PI / 2f, MathF.PI / 2f);

    public static Float32ScalarRange SymmetricPi { get; }
        = new Float32ScalarRange(-MathF.PI, MathF.PI);
    
    public static Float32ScalarRange SymmetricTwoPi { get; }
        = new Float32ScalarRange(-MathF.Tau, MathF.Tau);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Float32ScalarRange CreateAroundZero(float delta)
    {
        return delta >= 0
            ? new Float32ScalarRange(-delta, delta)
            : new Float32ScalarRange(delta, -delta);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Float32ScalarRange CreateAround(float center, float delta)
    {
        return delta >= 0
            ? new Float32ScalarRange(center - delta, center + delta)
            : new Float32ScalarRange(center + delta, center - delta);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Float32ScalarRange Create(float value)
    {
        return value > 0f
            ? new Float32ScalarRange(0f, value)
            : new Float32ScalarRange(value, 0f);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Float32ScalarRange Create(int value1, int value2)
    {
        return value1 <= value2
            ? new Float32ScalarRange(value1, value2)
            : new Float32ScalarRange(value2, value1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Float32ScalarRange Create(float value1, float value2)
    {
        return value1 <= value2
            ? new Float32ScalarRange(value1, value2)
            : new Float32ScalarRange(value2, value1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Float32ScalarRange Create(float value1, float value2, float value3)
    {
        var minValue = value1;
        var maxValue = value1;

        if (minValue > value2) minValue = value2;
        if (minValue > value3) minValue = value3;

        if (maxValue < value2) maxValue = value2;
        if (maxValue < value3) maxValue = value3;

        return new Float32ScalarRange(minValue, maxValue);
    }

    public static Float32ScalarRange Create(params float[] valuesList)
    {
        var minValue = 0.0f;
        var maxValue = 0.0f;

        var flag = false;
        foreach (var value in valuesList)
        {
            if (!flag)
            {
                minValue = value;
                maxValue = value;

                flag = true;
                continue;
            }

            if (minValue > value) minValue = value;
            if (maxValue < value) maxValue = value;
        }

        return new Float32ScalarRange(minValue, maxValue);
    }
    
    public static Float32ScalarRange Create(IEnumerable<float> valuesList)
    {
        var minValue = 0.0f;
        var maxValue = 0.0f;

        var flag = false;
        foreach (var value in valuesList)
        {
            if (!flag)
            {
                minValue = value;
                maxValue = value;

                flag = true;
                continue;
            }

            if (minValue > value) minValue = value;
            if (maxValue < value) maxValue = value;
        }

        return new Float32ScalarRange(minValue, maxValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Float32ScalarRange Create(Float32ScalarRange b1, Float32ScalarRange b2)
    {
        return new Float32ScalarRange(
            MathF.Min(b1.MinValue, b2.MinValue),
            MathF.Max(b1.MaxValue, b2.MaxValue)
        );
    }


    public float MinValue { get; }

    public float MaxValue { get; }

    public float MidValue
        => 0.5f * (MinValue + MaxValue);

    public float Length
        => MaxValue - MinValue;

    public bool IsInvalid
        => float.IsNaN(MinValue) ||
           float.IsNaN(MaxValue) ||
           MinValue > MaxValue;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        return !float.IsNaN(MinValue) &&
               !float.IsNaN(MaxValue) &&
               MinValue <= MaxValue;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarRange(float value)
    {
        Debug.Assert(!float.IsNaN(value));

        MinValue = value;
        MaxValue = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Float32ScalarRange(float minValue, float maxValue)
    {
        Debug.Assert(
            !float.IsNaN(minValue) &&
            !float.IsNaN(maxValue) &&
            minValue <= maxValue
        );

        MinValue = minValue;
        MaxValue = maxValue;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out float minValue, out float maxValue)
    {
        minValue = MinValue;
        maxValue = MaxValue;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(float value)
    {
        return value >= MinValue && 
               value <= MaxValue;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(float value, bool excludeMinValue, bool excludeMaxValue)
    {
        if (excludeMinValue && value <= MinValue)
            return false;

        if (!excludeMinValue && value < MinValue)
            return false;

        if (excludeMaxValue && value >= MaxValue)
            return false;

        if (!excludeMaxValue && value > MaxValue)
            return false;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ClampValue(float value)
    {
        if (value < MinValue) return MinValue;
        if (value > MaxValue) return MaxValue;

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ClampPeriodic(float value)
    {
        var length = Length;

        if (length == 0f) 
            return MinValue;

        //Map value to a periodic range
        var valueRelative = (value - MinValue) % length;

        if (valueRelative < 0f) 
            valueRelative += length;

        return MinValue + valueRelative;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float MapValue(float value, Float32ScalarRange targetRange)
    {
        var t = (value - MinValue) / Length;

        return targetRange.MinValue + t * targetRange.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
        return $"[{MinValue:G}, {MaxValue:G}]";
    }
}
