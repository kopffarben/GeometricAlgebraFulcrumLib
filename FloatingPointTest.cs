using System;
using System.Numerics;

namespace GeometricAlgebraFulcrumLib.Tests;

/// <summary>
/// Critical test: Can IFloatingPointIeee754&lt;T&gt; enable direct operators?
/// This would eliminate the need for IScalarProcessor&lt;T&gt; indirection for float/double/Half
/// </summary>
public class FloatingPointGenericTest<T> where T : IFloatingPointIeee754<T>
{
    // Can we use direct operators? YES!
    public T Add(T a, T b) => a + b;
    public T Subtract(T a, T b) => a - b;
    public T Multiply(T a, T b) => a * b;
    public T Divide(T a, T b) => a / b;
    public T Negate(T a) => -a;

    // Can we use static abstract interface members? YES!
    public T Sin(T x) => T.Sin(x);
    public T Cos(T x) => T.Cos(x);
    public T Sqrt(T x) => T.Sqrt(x);
    public T Abs(T x) => T.Abs(x);
    public T Exp(T x) => T.Exp(x);
    public T Log(T x) => T.Log(x);
    public T Pow(T x, T y) => T.Pow(x, y);

    // Can we use constants? YES!
    public T Zero => T.Zero;
    public T One => T.One;
    public T Pi => T.Pi;
    public T E => T.E;
    public T Tau => T.Tau;

    // Can we have T ZeroEpsilon? YES!
    public T ZeroEpsilon { get; set; } = T.CreateChecked(1e-12);

    // Can we compare? YES!
    public bool IsNearZero(T value)
    {
        return T.Abs(value) < ZeroEpsilon;
    }

    // Can we convert from numbers? YES!
    public T FromInt(int value) => T.CreateChecked(value);
    public T FromDouble(double value) => T.CreateChecked(value);
}

// Test with float32
class TestFloat32
{
    public void Run()
    {
        var test = new FloatingPointGenericTest<float>();
        test.ZeroEpsilon = 1e-7f;  // Appropriate for float32

        float result = test.Add(1.0f, 2.0f);
        float sinValue = test.Sin(test.Pi);
        bool nearZero = test.IsNearZero(1e-8f);

        Console.WriteLine($"Float32 - Result: {result}, Sin(Pi): {sinValue}, NearZero: {nearZero}");
    }
}

// Test with float64
class TestFloat64
{
    public void Run()
    {
        var test = new FloatingPointGenericTest<double>();
        test.ZeroEpsilon = 1e-12;  // Appropriate for float64

        double result = test.Add(1.0, 2.0);
        double sinValue = test.Sin(test.Pi);
        bool nearZero = test.IsNearZero(1e-13);

        Console.WriteLine($"Float64 - Result: {result}, Sin(Pi): {sinValue}, NearZero: {nearZero}");
    }
}

// Test with Half (float16)
class TestFloat16
{
    public void Run()
    {
        var test = new FloatingPointGenericTest<Half>();
        test.ZeroEpsilon = Half.CreateChecked(1e-3);  // Appropriate for Half

        Half result = test.Add(Half.One, Half.CreateChecked(2));
        Half sinValue = test.Sin(test.Pi);
        bool nearZero = test.IsNearZero(Half.CreateChecked(1e-4));

        Console.WriteLine($"Half - Result: {result}, Sin(Pi): {sinValue}, NearZero: {nearZero}");
    }
}

// CRITICAL QUESTION: What about Complex?
// Complex is NOT IFloatingPointIeee754<Complex> !
//
// public class ComplexProcessor
// {
//     // This WILL NOT compile:
//     // var test = new FloatingPointGenericTest<Complex>();  // ❌ Constraint violation!
//
//     // Complex needs separate handling because:
//     // 1. Complex has double ZeroEpsilon (for magnitude precision)
//     // 2. Complex.Magnitude returns double (not Complex)
//     // 3. Complex is fundamentally different (2D number)
// }
