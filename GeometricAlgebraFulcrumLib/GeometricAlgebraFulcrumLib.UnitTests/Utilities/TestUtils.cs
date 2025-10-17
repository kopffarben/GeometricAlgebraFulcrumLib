using System;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Utilities;

/// <summary>
/// Utility methods for testing Geometric Algebra operations
/// </summary>
public static class TestUtils
{
    /// <summary>
    /// Default tolerance for floating point comparisons
    /// </summary>
    public const double DefaultTolerance = 1e-10;

    /// <summary>
    /// Asserts that two multivectors are equal within a specified tolerance
    /// </summary>
    public static void AssertMultivectorEquals(
        XGaFloat64Multivector expected,
        XGaFloat64Multivector actual,
        double tolerance = DefaultTolerance,
        string? message = null)
    {
        if (expected is null && actual is null)
            return;

        if (expected is null || actual is null)
        {
            Assert.Fail(message ?? "One multivector is null while the other is not");
            return;
        }

        // Check if the difference is near zero
        var difference = expected - actual;
        var isNearZero = difference.IsNearZero(tolerance);

        if (!isNearZero)
        {
            var diffNorm = difference.Norm().ScalarValue;
            var errorMessage = message ??
                $"Multivectors are not equal.\nExpected: {expected}\nActual: {actual}\nDifference Norm: {diffNorm}";
            Assert.Fail(errorMessage);
        }
    }

    /// <summary>
    /// Asserts that two scalars are equal within a specified tolerance
    /// </summary>
    public static void AssertScalarEquals(
        XGaFloat64Scalar expected,
        XGaFloat64Scalar actual,
        double tolerance = DefaultTolerance,
        string? message = null)
    {
        var diff = Math.Abs(expected.ScalarValue - actual.ScalarValue);

        if (diff > tolerance)
        {
            var errorMessage = message ??
                $"Scalars are not equal.\nExpected: {expected.ScalarValue}\nActual: {actual.ScalarValue}\nDifference: {diff}";
            Assert.Fail(errorMessage);
        }
    }

    /// <summary>
    /// Asserts that two double values are equal within a specified tolerance
    /// </summary>
    public static void AssertDoubleEquals(
        double expected,
        double actual,
        double tolerance = DefaultTolerance,
        string? message = null)
    {
        var diff = Math.Abs(expected - actual);

        if (diff > tolerance)
        {
            var errorMessage = message ??
                $"Values are not equal.\nExpected: {expected}\nActual: {actual}\nDifference: {diff}";
            Assert.Fail(errorMessage);
        }
    }

    /// <summary>
    /// Asserts that a multivector is near zero
    /// </summary>
    public static void AssertNearZero(
        XGaFloat64Multivector multivector,
        double tolerance = DefaultTolerance,
        string? message = null)
    {
        if (!multivector.IsNearZero(tolerance))
        {
            var norm = multivector.Norm().ScalarValue;
            var errorMessage = message ??
                $"Multivector is not near zero.\nNorm: {norm}\nMultivector: {multivector}";
            Assert.Fail(errorMessage);
        }
    }

    /// <summary>
    /// Asserts that a scalar is near zero
    /// </summary>
    public static void AssertNearZero(
        double value,
        double tolerance = DefaultTolerance,
        string? message = null)
    {
        if (Math.Abs(value) > tolerance)
        {
            var errorMessage = message ??
                $"Value is not near zero.\nValue: {value}";
            Assert.Fail(errorMessage);
        }
    }

    /// <summary>
    /// Asserts that a multivector has a specific grade
    /// </summary>
    public static void AssertGrade(
        XGaFloat64Multivector multivector,
        int expectedGrade,
        string? message = null)
    {
        // Check if it's a k-vector of the expected grade
        if (!multivector.IsKVector(expectedGrade))
        {
            var grades = string.Join(", ", multivector.KVectorGrades);
            var errorMessage = message ??
                $"Multivector is not a k-vector of grade {expectedGrade}.\nActual grades: {grades}";
            Assert.Fail(errorMessage);
        }
    }

    /// <summary>
    /// Creates a random multivector with controlled properties
    /// </summary>
    public static XGaFloat64Multivector CreateRandomMultivector(
        XGaFloat64Processor processor,
        int vSpaceDimensions,
        int seed,
        int minTerms = 1,
        int maxTerms = 10)
    {
        var random = processor.CreateXGaRandomComposer(vSpaceDimensions, seed);
        var termCount = new Random(seed).Next(minTerms, maxTerms + 1);
        return random.GetUniformMultivector(termCount);
    }

    /// <summary>
    /// Creates a random vector
    /// </summary>
    public static XGaFloat64Vector CreateRandomVector(
        XGaFloat64Processor processor,
        int vSpaceDimensions,
        int seed)
    {
        var random = processor.CreateXGaRandomComposer(vSpaceDimensions, seed);
        return random.GetVector();
    }

    /// <summary>
    /// Creates a random blade of specified grade
    /// </summary>
    public static XGaFloat64KVector CreateRandomBlade(
        XGaFloat64Processor processor,
        int vSpaceDimensions,
        int grade,
        int seed)
    {
        var random = processor.CreateXGaRandomComposer(vSpaceDimensions, seed);
        return random.GetBlade(grade);
    }

    /// <summary>
    /// Creates a random rotor (verified to be a valid rotor)
    /// </summary>
    public static XGaFloat64Multivector CreateRandomRotor(
        XGaFloat64Processor processor,
        int vSpaceDimensions,
        int seed)
    {
        var random = processor.CreateXGaRandomComposer(vSpaceDimensions, seed);

        // Create two random vectors and construct a rotor from them
        var u = random.GetVector().DivideByENorm();
        var v = random.GetVector().DivideByENorm();

        var rotor = u.CreatePureRotor(v);
        return rotor.Multivector;
    }

    /// <summary>
    /// Verifies that a multivector is a valid rotor (R * reverse(R) = 1)
    /// </summary>
    public static void AssertIsValidRotor(
        XGaFloat64Multivector rotor,
        double tolerance = DefaultTolerance,
        string? message = null)
    {
        var product = rotor.Gp(rotor.Reverse());
        var scalar = product.GetScalarPart();

        AssertDoubleEquals(
            1.0,
            scalar,
            tolerance,
            message ?? "Rotor condition failed: R * reverse(R) should equal 1"
        );
    }

    /// <summary>
    /// Creates an array of random test multivectors
    /// </summary>
    public static XGaFloat64Multivector[] CreateRandomMultivectorArray(
        XGaFloat64Processor processor,
        int vSpaceDimensions,
        int count,
        int baseSeed)
    {
        var result = new XGaFloat64Multivector[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = CreateRandomMultivector(processor, vSpaceDimensions, baseSeed + i);
        }
        return result;
    }

    /// <summary>
    /// Prints detailed information about a multivector for debugging
    /// </summary>
    public static string GetMultivectorInfo(XGaFloat64Multivector mv)
    {
        var info = $"Multivector Info:\n";
        info += $"  Term Count: {mv.Count}\n";
        info += $"  Grades: {string.Join(", ", mv.KVectorGrades)}\n";
        info += $"  Norm: {mv.Norm().ScalarValue}\n";
        info += $"  Is Zero: {mv.IsZero}\n";
        info += $"  Terms:\n";

        foreach (var (id, scalar) in mv.IdScalarPairs)
        {
            info += $"    [{id}]: {scalar}\n";
        }

        return info;
    }

    /// <summary>
    /// Asserts that two multivectors are approximately proportional (differ by a scalar factor)
    /// </summary>
    public static void AssertProportional(
        XGaFloat64Multivector a,
        XGaFloat64Multivector b,
        double tolerance = DefaultTolerance,
        string? message = null)
    {
        // Check if either is zero
        if (a.IsNearZero(tolerance) || b.IsNearZero(tolerance))
        {
            Assert.Fail(message ?? "Cannot check proportionality when one multivector is near zero");
            return;
        }

        // Compute the ratio using scalar product
        var aNorm = a.Norm().ScalarValue;
        var bNorm = b.Norm().ScalarValue;

        if (Math.Abs(aNorm) < tolerance || Math.Abs(bNorm) < tolerance)
        {
            Assert.Fail(message ?? "Cannot check proportionality when one multivector has near-zero norm");
            return;
        }

        var ratio = aNorm / bNorm;
        var scaled = b * ratio;

        AssertMultivectorEquals(a, scaled, tolerance,
            message ?? "Multivectors are not proportional");
    }

    /// <summary>
    /// Gets a readable string representation of a grade
    /// </summary>
    public static string GetGradeName(int grade)
    {
        return grade switch
        {
            0 => "Scalar",
            1 => "Vector",
            2 => "Bivector",
            3 => "Trivector",
            4 => "Quadvector",
            _ => $"Grade-{grade}"
        };
    }

    /// <summary>
    /// Asserts that an operation preserves the norm (for orthogonal transformations)
    /// </summary>
    public static void AssertNormPreserving(
        XGaFloat64Multivector original,
        XGaFloat64Multivector transformed,
        double tolerance = DefaultTolerance,
        string? message = null)
    {
        var originalNorm = original.Norm().ScalarValue;
        var transformedNorm = transformed.Norm().ScalarValue;

        AssertDoubleEquals(
            originalNorm,
            transformedNorm,
            tolerance,
            message ?? $"Norm not preserved.\nOriginal: {originalNorm}\nTransformed: {transformedNorm}"
        );
    }
}
