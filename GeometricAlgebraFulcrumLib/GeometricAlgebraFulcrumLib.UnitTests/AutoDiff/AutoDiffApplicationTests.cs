using System;
using GeometricAlgebraFulcrumLib.Modeling.Calculus.AutoDiff;
using NUnit.Framework;
using static GeometricAlgebraFulcrumLib.UnitTests.AutoDiff.Utils;
using static GeometricAlgebraFulcrumLib.Modeling.Calculus.AutoDiff.TermBuilder;

namespace GeometricAlgebraFulcrumLib.UnitTests.AutoDiff;

/// <summary>
/// Tests for practical AutoDiff applications based on AutoDiffSamples.cs
/// These tests verify real-world use cases like optimization and root finding
/// </summary>
[TestFixture]
public class AutoDiffApplicationTests
{
    #region Helper Methods (from AutoDiffSamples.cs)

    /// <summary>
    /// Newton-Raphson method for finding roots of f(x) = 0
    /// </summary>
    private static double NewtonRaphson(Term func, Variable x, double initGuess, int maxIterations = 10)
    {
        var guess = initGuess;
        var compiledFunc = func.Compile(x);
        for (var i = 0; i < maxIterations; ++i)
        {
            var diffResult = compiledFunc.Differentiate(guess);
            var dfx = diffResult.Item1[0];  // derivative
            var fx = diffResult.Item2;       // function value
            guess = guess - fx / dfx;        // newton-raphson iteration
        }
        return guess;
    }

    /// <summary>
    /// Gradient descent optimization
    /// </summary>
    private static double[] GradientDescent(ICompiledTerm func, double[] init, double stepSize, int iterations)
    {
        var x = (double[])init.Clone();
        var gradient = new double[x.Length];

        for (var i = 0; i < iterations; ++i)
        {
            func.Differentiate(x, gradient);
            for (var j = 0; j < x.Length; ++j)
                x[j] -= stepSize * gradient[j];
        }
        return x;
    }

    #endregion

    #region Example 1: Basic Function Evaluation and Gradient

    [Test]
    public void BasicFunctionEvaluationAndGradient()
    {
        // func(x, y) = (x + y) * exp(x - y)
        var x = new Variable();
        var y = new Variable();
        var func = (x + y) * Exp(x - y);

        Variable[] vars = { x, y };
        double[] point = { 1, 2 };  // Changed from (1, -2) to (1, 2) for non-zero gradient

        var eval = func.Evaluate(vars, point);
        var gradient = func.Differentiate(vars, point);

        // Verify function value
        // f(1, 2) = (1 + 2) * exp(1 - 2) = 3 * exp(-1) = 3 / e
        var expected = 3 * Math.Exp(-1);
        Assert.That(Math.Abs(eval - expected), Is.LessThan(1e-10));

        // Verify gradient is computed (should be finite and valid)
        Assert.That(double.IsFinite(gradient[0]), Is.True);
        Assert.That(double.IsFinite(gradient[1]), Is.True);

        // The gradient should be non-trivial (not both zero)
        var gradientNorm = Math.Sqrt(gradient[0] * gradient[0] + gradient[1] * gradient[1]);
        Assert.That(gradientNorm, Is.GreaterThan(0.0), "Gradient should be non-zero");
    }

    #endregion

    #region Example 2: Compiled Term with Multiple Evaluations

    [Test]
    public void CompiledTermMultipleEvaluations()
    {
        // func(x, y) = (x + y) * exp(x - y)
        var x = new Variable();
        var y = new Variable();
        var func = (x + y) * Exp(x - y);

        var compiledFunc = func.Compile(x, y);

        // Perform multiple evaluations efficiently
        for (var i = 0; i < 10; ++i)
        {
            var xVal = i / 10.0;
            var yVal = 1 + i / 20.0;

            var diffResult = compiledFunc.Differentiate(xVal, yVal);
            var gradient = diffResult.Item1;
            var value = diffResult.Item2;

            // Verify results are finite
            Assert.That(double.IsFinite(value), Is.True);
            Assert.That(double.IsFinite(gradient[0]), Is.True);
            Assert.That(double.IsFinite(gradient[1]), Is.True);
        }
    }

    [Test]
    public void CompiledTermConsistentWithUncompiled()
    {
        var x = new Variable();
        var y = new Variable();
        var func = (x + y) * Exp(x - y);

        var compiledFunc = func.Compile(x, y);

        // Test that compiled and uncompiled give same results
        var testPoint = Vec(1.5, -0.5);

        var uncompiledValue = func.Evaluate(Vec(x, y), testPoint);
        var uncompiledGrad = func.Differentiate(Vec(x, y), testPoint);

        var compiledResult = compiledFunc.Differentiate(testPoint[0], testPoint[1]);
        var compiledValue = compiledResult.Item2;
        var compiledGrad = compiledResult.Item1;

        Assert.That(Math.Abs(uncompiledValue - compiledValue), Is.LessThan(1e-10));
        Assert.That(Math.Abs(uncompiledGrad[0] - compiledGrad[0]), Is.LessThan(1e-10));
        Assert.That(Math.Abs(uncompiledGrad[1] - compiledGrad[1]), Is.LessThan(1e-10));
    }

    #endregion

    #region Example 3: Gradient Descent Optimization

    [Test]
    public void GradientDescentFindsMinimum()
    {
        var x = new Variable();
        var y = new Variable();
        var z = new Variable();

        // f(x, y, z) = (x-2)² + (y+4)² + (z-1)²
        // The minimum should be at (x, y, z) = (2, -4, 1)
        var func = Power(x - 2, 2) + Power(y + 4, 2) + Power(z - 1, 2);
        var compiled = func.Compile(x, y, z);

        // Start from origin
        var vec = new double[3];
        vec = GradientDescent(compiled, vec, stepSize: 0.01, iterations: 1000);

        // Check that we're close to the minimum
        Assert.That(Math.Abs(vec[0] - 2.0), Is.LessThan(0.01), "x should be close to 2");
        Assert.That(Math.Abs(vec[1] - (-4.0)), Is.LessThan(0.01), "y should be close to -4");
        Assert.That(Math.Abs(vec[2] - 1.0), Is.LessThan(0.01), "z should be close to 1");
    }

    [Test]
    public void GradientDescentConvergesToZeroGradient()
    {
        var x = new Variable();
        var y = new Variable();

        // f(x, y) = (x-3)² + (y+1)²
        // Minimum at (3, -1)
        var func = Power(x - 3, 2) + Power(y + 1, 2);
        var compiled = func.Compile(x, y);

        var vec = GradientDescent(compiled, new double[] { 0, 0 }, stepSize: 0.02, iterations: 500);

        // At minimum, gradient should be near zero
        var gradient = new double[2];
        compiled.Differentiate(vec, gradient);

        Assert.That(Math.Abs(gradient[0]), Is.LessThan(0.01), "Gradient x-component should be near zero");
        Assert.That(Math.Abs(gradient[1]), Is.LessThan(0.01), "Gradient y-component should be near zero");
    }

    #endregion

    #region Example 4: Custom Functions (arctan, atan2)

    [Test]
    public void CustomArctanFunction()
    {
        // Create custom arctan function
        var arctan = UnaryFunc.Factory(
            x => Math.Atan(x),           // evaluate
            x => 1 / (1 + x * x));       // derivative

        var v = new Variable();
        var term = arctan(v);

        // Test at x = 1 (atan(1) = π/4)
        var value = term.Evaluate(Vec(v), NumVec(1.0));
        Assert.That(Math.Abs(value - Math.PI / 4), Is.LessThan(1e-10));

        // Test derivative at x = 1 (derivative = 1/(1+1²) = 0.5)
        var gradient = term.Differentiate(Vec(v), NumVec(1.0));
        Assert.That(Math.Abs(gradient[0] - 0.5), Is.LessThan(1e-10));
    }

    [Test]
    public void CustomAtan2Function()
    {
        // Create custom atan2 function
        var atan2 = BinaryFunc.Factory(
            (x, y) => Math.Atan2(y, x),
            (x, y) => Tuple.Create(
                -y / (x * x + y * y),    // d/dx
                x / (x * x + y * y)));   // d/dy

        var u = new Variable();
        var v = new Variable();
        var term = atan2(u, v);

        // Test at (x, y) = (1, 1) (atan2(1, 1) = π/4)
        var value = term.Evaluate(Vec(u, v), NumVec(1.0, 1.0));
        Assert.That(Math.Abs(value - Math.PI / 4), Is.LessThan(1e-10));

        // Test gradient
        var gradient = term.Differentiate(Vec(u, v), NumVec(1.0, 1.0));

        // At (1, 1): d/dx = -1/(1+1) = -0.5, d/dy = 1/(1+1) = 0.5
        Assert.That(Math.Abs(gradient[0] - (-0.5)), Is.LessThan(1e-10));
        Assert.That(Math.Abs(gradient[1] - 0.5), Is.LessThan(1e-10));
    }

    [Test]
    public void ComplexCustomFunctionComposition()
    {
        // Create function factories
        var arctan = UnaryFunc.Factory(
            x => Math.Atan(x),
            x => 1 / (1 + x * x));

        var atan2 = BinaryFunc.Factory(
            (x, y) => Math.Atan2(y, x),
            (x, y) => Tuple.Create(
                -y / (x * x + y * y),
                x / (x * x + y * y)));

        // Compose: atan2(u, v) - arctan(w) * atan2(v, w)
        var u = new Variable();
        var v = new Variable();
        var w = new Variable();
        var term = atan2(u, v) - arctan(w) * atan2(v, w);
        var compiled = term.Compile(u, v, w);

        // Compute value and gradient at (1, 2, -2)
        var diff = compiled.Differentiate(1, 2, -2);

        // Verify results are finite
        Assert.That(double.IsFinite(diff.Item2), Is.True, "Value should be finite");
        Assert.That(double.IsFinite(diff.Item1[0]), Is.True, "Gradient[0] should be finite");
        Assert.That(double.IsFinite(diff.Item1[1]), Is.True, "Gradient[1] should be finite");
        Assert.That(double.IsFinite(diff.Item1[2]), Is.True, "Gradient[2] should be finite");
    }

    #endregion

    #region Example 5: Newton-Raphson Root Finding

    [Test]
    public void NewtonRaphsonFindsRoot()
    {
        var x = new Variable();

        // f(x) = e^(-x) + x - 2
        // Has two roots (see plot in samples)
        var func = Exp(-x) + x - 2;

        // Find the root near 2
        var root1 = NewtonRaphson(func, x, initGuess: 2, maxIterations: 10);

        // Verify it's actually a root (f(root) ≈ 0)
        var value1 = func.Evaluate(Vec(x), NumVec(root1));
        Assert.That(Math.Abs(value1), Is.LessThan(1e-6), "Root 1 should make f(x) ≈ 0");
    }

    [Test]
    public void NewtonRaphsonFindsBothRoots()
    {
        var x = new Variable();

        // f(x) = e^(-x) + x - 2
        var func = Exp(-x) + x - 2;

        // Find root near 2
        var root1 = NewtonRaphson(func, x, initGuess: 2, maxIterations: 20);

        // Find root near -1
        var root2 = NewtonRaphson(func, x, initGuess: -1, maxIterations: 20);

        // Verify both are roots
        var value1 = func.Evaluate(Vec(x), NumVec(root1));
        var value2 = func.Evaluate(Vec(x), NumVec(root2));

        Assert.That(Math.Abs(value1), Is.LessThan(1e-6), "Root 1 should make f(x) ≈ 0");
        Assert.That(Math.Abs(value2), Is.LessThan(1e-6), "Root 2 should make f(x) ≈ 0");

        // Verify they are different roots
        Assert.That(Math.Abs(root1 - root2), Is.GreaterThan(0.1), "Should find two distinct roots");
    }

    [Test]
    public void NewtonRaphsonQuadraticEquation()
    {
        var x = new Variable();

        // f(x) = x² - 5x + 6 = (x-2)(x-3)
        // Roots at x = 2 and x = 3
        var func = Power(x, 2) - 5 * x + 6;

        // Find root near 2
        var root1 = NewtonRaphson(func, x, initGuess: 2.5, maxIterations: 10);

        // Find root near 3
        var root2 = NewtonRaphson(func, x, initGuess: 3.5, maxIterations: 10);

        Assert.That(Math.Abs(root1 - 2.0), Is.LessThan(1e-6), "Should find root at x=2");
        Assert.That(Math.Abs(root2 - 3.0), Is.LessThan(1e-6), "Should find root at x=3");
    }

    #endregion

    #region Performance and Edge Cases

    [Test]
    public void CompiledTermPerformanceWithManyEvaluations()
    {
        var x = new Variable();
        var y = new Variable();
        var func = Power(x, 2) + Power(y, 2) + x * y;
        var compiled = func.Compile(x, y);

        // Perform 100 evaluations
        for (var i = 0; i < 100; ++i)
        {
            var xVal = i / 50.0 - 1.0;
            var yVal = i / 100.0;

            var result = compiled.Differentiate(xVal, yVal);

            // Just verify we get valid results
            Assert.That(double.IsFinite(result.Item2), Is.True);
            Assert.That(result.Item1.Length, Is.EqualTo(2));
        }
    }

    [Test]
    public void GradientFillsProvidedArray()
    {
        var x = new Variable();
        var y = new Variable();
        var func = x * x + y * y;
        var compiled = func.Compile(x, y);

        var input = new double[] { 3.0, 4.0 };
        var gradient = new double[2];

        var value = compiled.Differentiate(input, gradient);

        // f(x,y) = x² + y²
        // f(3,4) = 9 + 16 = 25
        Assert.That(value, Is.EqualTo(25.0));

        // Gradient = (2x, 2y) = (6, 8)
        Assert.That(gradient[0], Is.EqualTo(6.0));
        Assert.That(gradient[1], Is.EqualTo(8.0));
    }

    [Test]
    public void OptimizationWithDifferentStepSizes()
    {
        var x = new Variable();
        var func = Power(x - 5, 2);  // Minimum at x = 5
        var compiled = func.Compile(x);

        // Test with small step size (should converge slowly)
        var result1 = GradientDescent(compiled, new double[] { 0 }, stepSize: 0.01, iterations: 100);

        // Test with larger step size (should converge faster)
        var result2 = GradientDescent(compiled, new double[] { 0 }, stepSize: 0.1, iterations: 100);

        // Both should move towards 5, but larger step size should get closer
        Assert.That(result1[0], Is.GreaterThan(0.0), "Should move towards 5");
        Assert.That(result2[0], Is.GreaterThan(result1[0]), "Larger step size should get closer");
        Assert.That(Math.Abs(result2[0] - 5.0), Is.LessThan(1.0), "Should be reasonably close");
    }

    #endregion
}
