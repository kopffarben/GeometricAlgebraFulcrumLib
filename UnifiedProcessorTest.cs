using System;
using System.Numerics;

namespace GeometricAlgebraFulcrumLib.Tests;

/// <summary>
/// CRITICAL TEST: Can INumber&lt;T&gt; unify EVERYTHING?
/// </summary>
public class UnifiedProcessorTest
{
    // ✅ INumber<T> supports:
    // - float, double, Half (IFloatingPointIeee754<T>)
    // - int, long, BigInteger (IIntegerBinary<T>)
    // - Complex (IFloatingPointIeee754<Complex>? NO!)
    // - decimal

    public class TestINumber<T> where T : INumber<T>
    {
        // ✅ Direct operators!
        public T Add(T a, T b) => a + b;
        public T Multiply(T a, T b) => a * b;
        public T Divide(T a, T b) => a / b;
        public T Negate(T a) => -a;

        // ✅ Static methods!
        public T Abs(T x) => T.Abs(x);

        // ❌ PROBLEM: No Sqrt, Sin, Cos in INumber<T>!
        // These are in IFloatingPointIeee754<T> and Complex separately

        // ❌ PROBLEM: ZeroEpsilon type?
        // Complex.Magnitude is double, not Complex!
        // So ZeroEpsilon must be double for Complex
        // But T for float/double
    }

    public class TestComplex
    {
        public void TestComplexAsINumber()
        {
            // ✅ Complex IS INumber<Complex>!
            var test = new TestINumber<Complex>();

            Complex c1 = new Complex(1, 2);
            Complex c2 = new Complex(3, 4);

            Complex sum = test.Add(c1, c2);  // ✅ Works!
            Complex product = test.Multiply(c1, c2);  // ✅ Works!

            // ❌ But: No Sqrt, Sin, Cos in INumber<T>!
            // Complex sqrtResult = test.Sqrt(c1);  // ❌ Doesn't compile!

            // ❌ And: Magnitude comparison issue
            // Complex.Magnitude returns double, not Complex
            // How to handle ZeroEpsilon?
        }
    }

    // CRITICAL QUESTION: Can we have polymorphic ZeroEpsilon?

    public class UnifiedProcessor<T> where T : INumber<T>
    {
        // ❌ PROBLEM 1: What type is ZeroEpsilon?
        // For float/double: Should be T
        // For Complex: Should be double (for magnitude)
        // For symbolic: Should be double (for evaluation precision)

        public object ZeroEpsilon { get; set; }  // ❌ Ugly! Type-unsafe!

        // Or:
        // public T ZeroEpsilon { get; set; }  // ❌ Wrong for Complex!

        // ❌ PROBLEM 2: Math functions not in INumber<T>
        // public T Sqrt(T x) => T.Sqrt(x);  // ❌ Doesn't compile!

        // Need to use runtime type checking:
        public T Sqrt(T x)
        {
            if (typeof(T) == typeof(float))
                return (T)(object)MathF.Sqrt((float)(object)x!);  // ❌ Ugly!

            if (typeof(T) == typeof(double))
                return (T)(object)Math.Sqrt((double)(object)x!);  // ❌ Ugly!

            if (typeof(T) == typeof(Complex))
                return (T)(object)Complex.Sqrt((Complex)(object)x!);  // ❌ Ugly!

            throw new NotSupportedException();  // ❌ Runtime error!
        }
    }

    // ALTERNATIVE: What about separate interfaces for capabilities?

    public interface IScalarOperations<T>
    {
        T Add(T a, T b);
        T Multiply(T a, T b);
        T Sqrt(T a);
        // ... all operations
    }

    // But this brings us back to IScalarProcessor<T>!
    // We're going in circles!
}

/// <summary>
/// KEY INSIGHT: The problem is NOT just operators!
///
/// The real issues:
/// 1. ZeroEpsilon type (T for float/double, double for Complex/symbolic)
/// 2. Math functions availability (Sqrt, Sin, Cos not in INumber&lt;T&gt;)
/// 3. Symbolic types don't compute, they build expression trees
///
/// INumber&lt;T&gt; helps with operators, but doesn't solve everything.
/// </summary>
public class KeyInsight
{
    /*
     * CORE PROBLEM: Types fall into THREE categories:
     *
     * 1. FLOATING-POINT NUMERIC: float, double, Half
     *    - Have operators (INumber<T>)
     *    - Have math functions (IFloatingPointIeee754<T>)
     *    - ZeroEpsilon type: T
     *    - Direct computation
     *
     * 2. OTHER NUMERIC: Complex, decimal, BigInteger
     *    - Have operators (INumber<T>)
     *    - Math functions: type-specific (Complex.Sqrt vs no Sqrt for BigInteger)
     *    - ZeroEpsilon type: varies (double for Complex magnitude)
     *    - Direct computation
     *
     * 3. SYMBOLIC: IMetaExpression, WolframExpr, AngouriMath
     *    - NO operators (build AST instead)
     *    - NO math functions (build AST instead)
     *    - ZeroEpsilon type: double (for evaluation)
     *    - Build expression trees, not compute
     *
     * Can ONE implementation handle all three?
     */
}
