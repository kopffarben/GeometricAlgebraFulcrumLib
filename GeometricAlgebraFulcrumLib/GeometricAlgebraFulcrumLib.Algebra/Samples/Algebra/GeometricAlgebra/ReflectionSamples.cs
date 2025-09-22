using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.LinearMaps.SpaceND;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.LinearMaps.SpaceND.Reflection;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Matrices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.SpaceND;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Utilities.Text;
using MathNet.Numerics.LinearAlgebra;

namespace GeometricAlgebraFulcrumLib.Algebra.Samples.Algebra.GeometricAlgebra;

/// <summary>
/// Provides examples and tests for reflection operations in geometric algebra
/// </summary>
public static class ReflectionSamples
{
    private const double DefaultTolerance = 1e-7;
    private const int TestIterations = 100;

    /// <summary>
    /// Creates and initializes common geometric algebra components
    /// </summary>
    private static (XGaFloat64Processor processor, TextComposerFloat64 textComposer, LaTeXComposerFloat64 laTeXComposer, Random random) 
        CreateCommonComponents(int seed = 10)
    {
        var processor = XGaFloat64Processor.Euclidean;
        var textComposer = TextComposerFloat64.DefaultComposer;
        var laTeXComposer = LaTeXComposerFloat64.DefaultComposer;
        var random = new Random(seed);

        return (processor, textComposer, laTeXComposer, random);
    }

    /// <summary>
    /// Validates that two vectors are approximately equal within a tolerance
    /// </summary>
    private static void AssertVectorsEqual(LinFloat64Vector v1, LinFloat64Vector v2, double tolerance = DefaultTolerance)
    {
        Debug.Assert((v1 - v2).GetVectorNormSquared().IsNearZero(tolerance), 
            "Vectors should be approximately equal");
    }

    /// <summary>
    /// Validates that a matrix is a reflection matrix (determinant ±1)
    /// </summary>
    private static void AssertIsReflectionMatrix(Matrix<double> matrix, double tolerance = DefaultTolerance)
    {
        Debug.Assert(Math.Abs(matrix.Determinant()).IsNearOne(tolerance), 
            "Matrix should be a reflection matrix with |determinant| = 1");
    }

    /// <summary>
    /// Validates that two matrices are approximately equal
    /// </summary>
    private static void AssertMatricesEqual(Matrix<double> m1, Matrix<double> m2, double tolerance = DefaultTolerance)
    {
        Debug.Assert((m1 - m2).L2Norm() < tolerance,
            "Matrices should be approximately equal");
    }

    /// <summary>
    /// Prints subspace information to console
    /// </summary>
    private static void PrintSubspaces(IEnumerable<object> subspaces, string prefix = "Subspace")
    {
        var index = 1;
        foreach (var subspace in subspaces)
        {
            Console.WriteLine($"{prefix} {index++}:");
            Console.WriteLine(subspace);
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Sample demonstrating conversion between reflection matrix and hyperplane reflections
    /// </summary>
    /// <param name="n">Dimension of the space</param>
    /// <param name="reflectionCount">Number of reflections to generate</param>
    public static void ReflectionMatrixToHyperPlaneReflectionsSample(int n, int reflectionCount)
    {
        var (processor, _, _, _) = CreateCommonComponents();
        var randomComposer = processor.CreateXGaRandomComposer(n, 10);
        var random = randomComposer.RandomGenerator;

        // Create initial reflection sequence
        var reflectionSequence = LinFloat64HyperPlaneNormalReflectionSequence.Create(n);
        
        for (var i = 0; i < reflectionCount; i++)
        {
            reflectionSequence.AppendMap(random.GetHyperPlaneNormalReflection(n));
        }

        // Convert to matrix and back to sequence
        var matrix = reflectionSequence.ToMatrix(n, n);
        var reconstructedSequence = LinFloat64HyperPlaneNormalReflectionSequence.CreateFromReflectionMatrix(matrix);

        // Validate reconstruction accuracy
        AssertMatricesEqual(reconstructedSequence.ToMatrix(n, n), matrix);
    }

    /// <summary>
    /// Example demonstrating hyperplane normal reflection sequence operations
    /// </summary>
    public static void HyperPlaneReflectionExample()
    {
        const int n = 9;
        var (_, _, _, random) = CreateCommonComponents();

        // Generate orthogonal matrix and convert to reflection sequence
        var matrix = random.GetMathNetOrthogonalMatrix(n);
        var reflectionSequence = matrix.GetHyperPlaneNormalReflectionSequence();
        
        // Validate round-trip conversion
        var reconstructedMatrix = reflectionSequence.ToMatrix(n, n)
            .GetHyperPlaneNormalReflectionSequence()
            .ToMatrix(n, n);

        AssertMatricesEqual(reconstructedMatrix, reflectionSequence.ToMatrix(n, n));
        AssertMatricesEqual(matrix, reflectionSequence.ToMatrix(n, n));

        // Display reflection information
        PrintReflectionSequenceInfo(reflectionSequence);

        // Validate reflection properties
        ValidateReflectionProperties(reflectionSequence, n);

        // Display eigensubspaces
        var reflectionMatrix = reflectionSequence.ToMatrix(n, n);
        var subspaceList = reflectionMatrix.GetSimpleEigenSubspaces();
        PrintSubspaces(subspaceList);
    }

    /// <summary>
    /// Prints information about a reflection sequence
    /// </summary>
    private static void PrintReflectionSequenceInfo(LinFloat64HyperPlaneNormalReflectionSequence sequence)
    {
        for (var k = 0; k < sequence.Count; k++)
        {
            var reflection = sequence[k];
            var normalVector = reflection.ReflectionNormal;

            Console.WriteLine($"Reflection {k + 1}:");
            Console.WriteLine($"Normal Vector: {normalVector}");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Validates that reflection properties hold for the given sequence
    /// </summary>
    private static void ValidateReflectionProperties(LinFloat64HyperPlaneNormalReflectionSequence sequence, int n)
    {
        var reflectionMatrix = sequence.ToMatrix(n, n);

        foreach (var reflection in sequence)
        {
            var normalVector = reflection.ReflectionNormal;
            var expectedReflected = -normalVector;

            var reflected1 = sequence.MapVector(normalVector);
            var reflected2 = (reflectionMatrix * MathNetNumericsUtils.ToMathNetVector(normalVector, n)).CreateLinVector();

            AssertVectorsEqual(expectedReflected, reflected1);
            AssertVectorsEqual(expectedReflected, reflected2);
            AssertVectorsEqual(reflected1, reflected2);
        }
    }


    /// <summary>
    /// Comprehensive validation of reflection properties for arbitrary reflection sequences
    /// </summary>
    public static void ValidateReflectionSequenceProperties()
    {
        const int n = 9;
        var (_, _, _, random) = CreateCommonComponents();

        // Test orthogonal reflections
        ValidateOrthogonalReflections(n, random);

        // Test general reflections
        ValidateGeneralReflections(n, random);
    }

    /// <summary>
    /// Validates properties of orthogonal reflection sequences
    /// </summary>
    private static void ValidateOrthogonalReflections(int n, Random random)
    {
        for (var reflectionCount = 1; reflectionCount <= n; reflectionCount++)
        {
            var reflectionSequence = LinFloat64HyperPlaneNormalReflectionSequence.CreateRandomOrthogonal(
                random, n, reflectionCount);

            var reflectionMatrix = reflectionSequence.ToMatrix(n, n);

            // Validate reflection matrix properties
            AssertIsReflectionMatrix(reflectionMatrix);
            Debug.Assert(reflectionSequence.IsNearOrthogonalReflectionsSequence(),
                "Sequence should contain only orthogonal reflections");

            // Display results
            var subspaceList = reflectionMatrix.GetSimpleEigenSubspaces();
            Console.WriteLine($"Orthogonal reflections number: {reflectionCount}");
            PrintSubspaces(subspaceList);

            // Validate individual reflections
            ValidateIndividualReflections(reflectionSequence, reflectionMatrix, n);

            // Validate sequence-matrix consistency
            ValidateSequenceMatrixConsistency(reflectionSequence, reflectionMatrix, random, n);
        }
    }

    /// <summary>
    /// Validates properties of general reflection sequences
    /// </summary>
    private static void ValidateGeneralReflections(int n, Random random)
    {
        for (var reflectionCount = 1; reflectionCount <= 2 * n; reflectionCount++)
        {
            var reflectionSequence = LinFloat64HyperPlaneNormalReflectionSequence.CreateRandom(
                random, n, reflectionCount);

            var reflectionMatrix = reflectionSequence.ToMatrix(n, n);

            // Validate reflection matrix properties
            AssertIsReflectionMatrix(reflectionMatrix);

            // Display results
            var subspaceList = reflectionMatrix.GetSimpleEigenSubspaces();
            Console.WriteLine($"General reflections number: {reflectionCount}");
            PrintSubspaces(subspaceList);

            // Validate sequence-matrix consistency
            ValidateSequenceMatrixConsistency(reflectionSequence, reflectionMatrix, random, n);
        }
    }

    /// <summary>
    /// Validates that individual reflections work correctly
    /// </summary>
    private static void ValidateIndividualReflections(
        LinFloat64HyperPlaneNormalReflectionSequence sequence,
        Matrix<double> matrix,
        int n)
    {
        foreach (var reflection in sequence)
        {
            var normalVector = reflection.ReflectionNormal;
            var expectedReflected = -normalVector;

            var reflected1 = sequence.MapVector(normalVector);
            var reflected2 = (matrix * MathNetNumericsUtils.ToMathNetVector(normalVector, n)).CreateLinVector();

            AssertVectorsEqual(expectedReflected, reflected1);
            AssertVectorsEqual(expectedReflected, reflected2);
            AssertVectorsEqual(reflected1, reflected2);
        }
    }

    /// <summary>
    /// Validates consistency between sequence operations and matrix multiplication
    /// </summary>
    private static void ValidateSequenceMatrixConsistency(
        LinFloat64HyperPlaneNormalReflectionSequence sequence,
        Matrix<double> matrix,
        Random random,
        int n)
    {
        for (var i = 0; i < TestIterations; i++)
        {
            var testVector = random.GetLinVector(n).CreateLinVector();

            var result1 = sequence.MapVector(testVector);
            var result2 = (matrix * MathNetNumericsUtils.ToMathNetVector(testVector, n)).CreateLinVector();

            AssertVectorsEqual(result1, result2);
        }
    }
}