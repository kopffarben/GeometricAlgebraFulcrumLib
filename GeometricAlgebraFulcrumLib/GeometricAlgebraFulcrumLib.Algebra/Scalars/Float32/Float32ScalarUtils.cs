using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Algebra.Scalars.Float32;

public static class Float32ScalarUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValid(this IFloat32Scalar scalar)
    {
        return !float.IsNaN(scalar.ScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPositive(this IFloat32Scalar scalar)
    {
        return scalar.ScalarValue > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNegative(this IFloat32Scalar scalar)
    {
        return scalar.ScalarValue < 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotZeroOrNearPositive(this IFloat32Scalar scalar, float zeroEpsilon = Float32Utils.ZeroEpsilon)
    {
        return scalar.ScalarValue < -zeroEpsilon;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotZeroOrNearNegative(this IFloat32Scalar scalar, float zeroEpsilon = Float32Utils.ZeroEpsilon)
    {
        return scalar.ScalarValue > zeroEpsilon;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZeroOrPositive(this IFloat32Scalar scalar)
    {
        return scalar.ScalarValue >= 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZeroOrNegative(this IFloat32Scalar scalar)
    {
        return scalar.ScalarValue <= 0;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFinite(this IFloat32Scalar scalar)
    {
        return float.IsFinite(scalar.ScalarValue);
    }
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsZero(this IFloat32Scalar scalar)
    {
        return scalar.ScalarValue == 0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNearZero(this IFloat32Scalar scalar, float zeroEpsilon = Float32Utils.ZeroEpsilon)
    {
        return MathF.Abs(scalar.ScalarValue) < zeroEpsilon;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOne(this IFloat32Scalar scalar)
    {
        return scalar.ScalarValue - 1f == 0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNearOne(this IFloat32Scalar scalar, float zeroEpsilon = Float32Utils.ZeroEpsilon)
    {
        return MathF.Abs(scalar.ScalarValue - 1f) < zeroEpsilon;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMinusOne(this IFloat32Scalar scalar)
    {
        return scalar.ScalarValue + 1f == 0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNearMinusOne(this IFloat32Scalar scalar, float zeroEpsilon = Float32Utils.ZeroEpsilon)
    {
        return MathF.Abs(scalar.ScalarValue + 1f) < zeroEpsilon;
    }
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(this IFloat32Scalar scalar1, float scalar2)
    {
        return scalar1.ScalarValue - scalar2 == 0f;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(this IFloat32Scalar scalar1, IFloat32Scalar scalar2)
    {
        return scalar1.ScalarValue - scalar2.ScalarValue == 0f;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(this int scalar1, IFloat32Scalar scalar2)
    {
        return scalar1 - scalar2.ScalarValue == 0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(this uint scalar1, IFloat32Scalar scalar2)
    {
        return scalar1 - scalar2.ScalarValue == 0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(this long scalar1, IFloat32Scalar scalar2)
    {
        return scalar1 - scalar2.ScalarValue == 0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(this ulong scalar1, IFloat32Scalar scalar2)
    {
        return scalar1 - scalar2.ScalarValue == 0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(this float scalar1, IFloat32Scalar scalar2)
    {
        return scalar1 - scalar2.ScalarValue == 0f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEqualTo(this double scalar1, IFloat32Scalar scalar2)
    {
        return (float)(scalar1 - scalar2.ScalarValue) == 0f;
    }
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNearEqualTo(this IFloat32Scalar scalar1, float scalar2, float zeroEpsilon = Float32Utils.ZeroEpsilon)
    {
        return MathF.Abs(scalar1.ScalarValue - scalar2) < zeroEpsilon;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNearEqualTo(this IFloat32Scalar scalar1, IFloat32Scalar scalar2, float zeroEpsilon = Float32Utils.ZeroEpsilon)
    {
        return MathF.Abs(scalar1.ScalarValue - scalar2.ScalarValue) < zeroEpsilon;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Abs(this IFloat32Scalar scalar)
    {
        return MathF.Abs(scalar.ScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sqrt(this IFloat32Scalar scalar)
    {
        return MathF.Sqrt(scalar.ScalarValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SqrtOfAbs(this IFloat32Scalar scalar)
    {
        return MathF.Sqrt(MathF.Abs(scalar.ScalarValue));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Square(this IFloat32Scalar scalar)
    {
        return scalar.ScalarValue * scalar.ScalarValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cube(this IFloat32Scalar scalar)
    {
        return scalar.ScalarValue * scalar.ScalarValue * scalar.ScalarValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Power(this IFloat32Scalar scalar, float exponent)
    {
        return MathF.Pow(scalar.ScalarValue, exponent);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntegerSign Sign(this IFloat32Scalar scalar)
    {
        var value = scalar.ScalarValue;

        if (value == 0f)
            return IntegerSign.Zero;

        return value > 0f
            ? IntegerSign.Positive
            : IntegerSign.Negative;
    }
}
