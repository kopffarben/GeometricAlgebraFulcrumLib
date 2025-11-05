using System.Numerics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Algebra.Scalars.Floating;

/// <summary>
/// Unified scalar processor for IEEE 754 floating-point types.
/// Supports float, double, Half via IFloatingPointIeee754&lt;T&gt;.
/// </summary>
/// <typeparam name="T">Floating-point type (float, double, Half)</typeparam>
public sealed class ScalarProcessorOfFloating<T> : INumericScalarProcessor<T>
    where T : IFloatingPointIeee754<T>
{
    /// <summary>
    /// Singleton instance for performance (no allocations).
    /// </summary>
    public static ScalarProcessorOfFloating<T> Instance { get; } = new();

    private double _zeroEpsilon = 1e-12;
    public double ZeroEpsilon
    {
        get => _zeroEpsilon;
        set
        {
            if (double.IsNaN(value) || Math.Abs(value) > 1)
                throw new ArgumentException(nameof(value));

            _zeroEpsilon = Math.Abs(value);
        }
    }

    public bool IsNumeric => true;
    public bool IsSymbolic => false;

    // Scalar<T> Properties
    public Scalar<T> Zero { get; }
    public Scalar<T> PositiveInfinity { get; }
    public Scalar<T> NegativeInfinity { get; }
    public Scalar<T> One { get; }
    public Scalar<T> MinusOne { get; }
    public Scalar<T> Two { get; }
    public Scalar<T> MinusTwo { get; }
    public Scalar<T> Ten { get; }
    public Scalar<T> MinusTen { get; }
    public Scalar<T> Pi { get; }
    public Scalar<T> PiTimes2 { get; }
    public Scalar<T> PiTimes4 { get; }
    public Scalar<T> PiOver2 { get; }
    public Scalar<T> E { get; }
    public Scalar<T> DegreeToRadianFactor { get; }
    public Scalar<T> RadianToDegreeFactor { get; }

    // Raw T Value Properties
    public T ZeroValue => T.Zero;
    public T PositiveInfinityValue => T.PositiveInfinity;
    public T NegativeInfinityValue => T.NegativeInfinity;
    public T OneValue => T.One;
    public T MinusOneValue => -T.One;
    public T TwoValue => T.One + T.One;
    public T MinusTwoValue => -(T.One + T.One);
    public T TenValue => T.CreateChecked(10);
    public T MinusTenValue => T.CreateChecked(-10);
    public T PiValue => T.Pi;
    public T PiTimes2Value => T.CreateChecked(2) * T.Pi;
    public T PiTimes4Value => T.CreateChecked(4) * T.Pi;
    public T PiOver2Value => T.Pi / T.CreateChecked(2);
    public T EValue => T.E;
    public T DegreeToRadianFactorValue => T.Pi / T.CreateChecked(180);
    public T RadianToDegreeFactorValue => T.CreateChecked(180) / T.Pi;

    private ScalarProcessorOfFloating()
    {
        Zero = ScalarFromValue(ZeroValue);
        One = ScalarFromValue(OneValue);
        MinusOne = ScalarFromValue(MinusOneValue);
        Two = ScalarFromValue(TwoValue);
        MinusTwo = ScalarFromValue(MinusTwoValue);
        Ten = ScalarFromValue(TenValue);
        MinusTen = ScalarFromValue(MinusTenValue);
        Pi = ScalarFromValue(PiValue);
        E = ScalarFromValue(EValue);
        PiTimes2 = ScalarFromValue(PiTimes2Value);
        PiTimes4 = ScalarFromValue(PiTimes4Value);
        PiOver2 = ScalarFromValue(PiOver2Value);
        DegreeToRadianFactor = ScalarFromValue(DegreeToRadianFactorValue);
        RadianToDegreeFactor = ScalarFromValue(RadianToDegreeFactorValue);
        PositiveInfinity = ScalarFromValue(PositiveInfinityValue);
        NegativeInfinity = ScalarFromValue(NegativeInfinityValue);
    }

    // Wrapping method (required by design spec)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Scalar(T value) => Generic.Scalar<T>.Create(this, value);

    // Arithmetic Operations
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Add(T a, T b) => ScalarFromValue(a + b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Subtract(T a, T b) => ScalarFromValue(a - b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Times(T a, T b) => ScalarFromValue(a * b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Divide(T a, T b) => ScalarFromValue(a / b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Negative(T a) => ScalarFromValue(-a);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Positive(T a) => ScalarFromValue(+a);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Inverse(T a) => ScalarFromValue(T.One / a);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Sign(T a) => ScalarFromValue(T.CreateChecked(T.Sign(a)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> UnitStep(T a) => a > T.Zero ? One : Zero;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Abs(T a) => ScalarFromValue(T.Abs(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Power(T baseScalar, T scalar) => ScalarFromValue(T.Pow(baseScalar, scalar));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Sqrt(T a) => ScalarFromValue(T.Sqrt(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> SqrtOfAbs(T a) => ScalarFromValue(T.Sqrt(T.Abs(a)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Exp(T a) => ScalarFromValue(T.Exp(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Log(T a) => ScalarFromValue(T.Log(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> LogE(T a) => ScalarFromValue(T.Log(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Log2(T a) => ScalarFromValue(T.Log2(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Log10(T a) => ScalarFromValue(T.Log10(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Log(T baseScalar, T scalar) => ScalarFromValue(T.Log(scalar, baseScalar));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> VectorToRadians(T scalarX, T scalarY) => ScalarFromValue(T.Atan2(scalarY, scalarX));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Square(T a) => ScalarFromValue(a * a);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Cube(T a) => ScalarFromValue(a * a * a);

    // Trigonometric Functions
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Cos(T a) => ScalarFromValue(T.Cos(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Sin(T a) => ScalarFromValue(T.Sin(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Tan(T a) => ScalarFromValue(T.Tan(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Cosh(T a) => ScalarFromValue(T.Cosh(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Sinh(T a) => ScalarFromValue(T.Sinh(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> Tanh(T a) => ScalarFromValue(T.Tanh(a));

    // Inverse Trigonometric Functions
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ArcCos(T a) => ScalarFromValue(T.Acos(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ArcSin(T a) => ScalarFromValue(T.Asin(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ArcTan(T a) => ScalarFromValue(T.Atan(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ArcTan2(T y, T x) => ScalarFromValue(T.Atan2(y, x));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ArcCosh(T a) => ScalarFromValue(T.Acosh(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ArcSinh(T a) => ScalarFromValue(T.Asinh(a));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ArcTanh(T a) => ScalarFromValue(T.Atanh(a));

    // Comparisons & Tests
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid(T a) => !T.IsNaN(a) && !T.IsInfinity(a);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsFiniteNumber(T a) => T.IsFinite(a);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsZero(T a) => a == T.Zero;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsZero(T a, bool nearZeroFlag)
    {
        return nearZeroFlag
            ? T.Abs(a) < T.CreateChecked(_zeroEpsilon)
            : a == T.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNearZero(T a) => T.Abs(a) < T.CreateChecked(_zeroEpsilon);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNotZero(T a) => a != T.Zero;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNotNearZero(T a) => T.Abs(a) >= T.CreateChecked(_zeroEpsilon);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsPositive(T a) => a > T.Zero;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNegative(T a) => a < T.Zero;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNotPositive(T a) => a <= T.Zero;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNotNegative(T a) => a >= T.Zero;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNotNearPositive(T a) => a <= T.CreateChecked(_zeroEpsilon);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNotNearNegative(T a) => a >= T.CreateChecked(-_zeroEpsilon);

    // Value Conversions
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetScalarFromNumber(int number) => T.CreateChecked(number);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetScalarFromNumber(uint number) => T.CreateChecked(number);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetScalarFromNumber(long number) => T.CreateChecked(number);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetScalarFromNumber(ulong number) => T.CreateChecked(number);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetScalarFromNumber(float number) => T.CreateChecked(number);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetScalarFromNumber(double number) => T.CreateChecked(number);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetScalarFromText(string text) => T.Parse(text, null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetScalarFromRational(long numerator, long denominator)
        => T.CreateChecked(numerator) / T.CreateChecked(denominator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetScalarFromRational(ulong numerator, ulong denominator)
        => T.CreateChecked(numerator) / T.CreateChecked(denominator);

    // Scalar<T> Conversions
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ScalarFromNumber(int number) => ScalarFromValue(T.CreateChecked(number));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ScalarFromNumber(uint number) => ScalarFromValue(T.CreateChecked(number));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ScalarFromNumber(long number) => ScalarFromValue(T.CreateChecked(number));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ScalarFromNumber(ulong number) => ScalarFromValue(T.CreateChecked(number));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ScalarFromNumber(float number) => ScalarFromValue(T.CreateChecked(number));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ScalarFromNumber(double number) => ScalarFromValue(T.CreateChecked(number));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ScalarFromText(string text) => ScalarFromValue(T.Parse(text, null));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ScalarFromValue(T value) => Generic.Scalar<T>.Create(this, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ScalarFromRational(long numerator, long denominator)
        => ScalarFromValue(T.CreateChecked(numerator) / T.CreateChecked(denominator));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Scalar<T> ScalarFromRandom(Random randomGenerator, double minValue, double maxValue)
    {
        var value = minValue + (maxValue - minValue) * randomGenerator.NextDouble();
        return ScalarFromValue(T.CreateChecked(value));
    }

    // Utility Methods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToText(T scalar) => scalar.ToString() ?? string.Empty;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ToFloat64(T scalar) => double.CreateChecked(scalar);

    /// <summary>
    /// Numerical operations not yet implemented for generic floating-point types.
    /// TODO Phase 3: Implement type-specific dispatch for float/double.
    /// </summary>
    public INumericalOperations<T>? NumericalOperations => null;
}