using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Frames;

/// <summary>
/// Generic Gram-Schmidt orthogonalization frame for any scalar type T.
/// Uses the classical Gram-Schmidt algorithm with IScalarProcessor&lt;T&gt; for scalar operations.
/// </summary>
/// <typeparam name="T">Scalar type (double, float, rational, symbolic, etc.)</typeparam>
public class XGaGramSchmidtFrame<T>
{
    /// <summary>
    /// Create a Gram-Schmidt orthonormal frame from a set of input vectors.
    /// Uses the modified Gram-Schmidt algorithm for better numerical stability.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static XGaGramSchmidtFrame<T> Create(params XGaVector<T>[] vArray)
    {
        if (vArray.Length == 0)
            throw new ArgumentException("Input vector array cannot be empty", nameof(vArray));

        var processor = vArray[0].Processor;
        var scalarProcessor = processor.ScalarProcessor;
        var n = vArray.Length;

        var directionNorms = new T[n];
        var unitDirections = new XGaVector<T>[n];

        // Modified Gram-Schmidt algorithm
        // For each input vector, orthogonalize against all previous unit vectors
        for (var i = 0; i < n; i++)
        {
            var u = vArray[i];

            // Orthogonalize against all previous unit vectors
            for (var j = 0; j < i; j++)
            {
                // projection = (v · e_j) * e_j
                var dotProduct = u.ESp(unitDirections[j]).ScalarValue;
                var projection = unitDirections[j].Times(dotProduct);
                u = u.Subtract(projection);
            }

            // Compute norm of orthogonalized vector
            var normSquared = u.ENormSquared().ScalarValue;
            var norm = scalarProcessor.Sqrt(normSquared).ScalarValue;

            // Store norm
            directionNorms[i] = norm;

            // Normalize to get unit vector
            // Handle near-zero norm case
            if (scalarProcessor.IsZero(norm))
            {
                // Create a zero vector as unit direction (degenerate case)
                unitDirections[i] = processor.VectorZero;
            }
            else
            {
                unitDirections[i] = u.Divide(norm);
            }
        }

        return new XGaGramSchmidtFrame<T>(directionNorms, unitDirections, processor);
    }


    private readonly T[] _directionNormsArray;

    public XGaProcessor<T> Processor { get; }

    public IScalarProcessor<T> ScalarProcessor => Processor.ScalarProcessor;

    public IReadOnlyList<T> DirectionNorms => _directionNormsArray;

    public IReadOnlyList<XGaVector<T>> UnitDirections { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private XGaGramSchmidtFrame(T[] directionNorms, IReadOnlyList<XGaVector<T>> unitDirections, XGaProcessor<T> processor)
    {
        _directionNormsArray = directionNorms;
        UnitDirections = unitDirections;
        Processor = processor;
    }


    /// <summary>
    /// Clean near-zero norms by setting them to exact zero.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XGaGramSchmidtFrame<T> CleanNorms()
    {
        var scalarProcessor = ScalarProcessor;

        for (var i = 0; i < _directionNormsArray.Length; i++)
        {
            if (scalarProcessor.IsZero(_directionNormsArray[i]))
                _directionNormsArray[i] = scalarProcessor.ZeroValue;
        }

        return this;
    }

    /// <summary>
    /// Get the direction vector at the specified index (non-normalized).
    /// direction = norm * unitDirection
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XGaVector<T> GetDirection(int index)
    {
        return UnitDirections[index].Times(_directionNormsArray[index]);
    }

    /// <summary>
    /// Get all direction vectors (non-normalized).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<XGaVector<T>> GetDirections()
    {
        return _directionNormsArray.Select(
            (norm, i) => UnitDirections[i].Times(norm)
        );
    }

    /// <summary>
    /// Get the curvature at index i: k_i = ||d_{i+1}|| / ||d_i||
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetCurvature(int index)
    {
        var scalarProcessor = ScalarProcessor;
        return scalarProcessor.Divide(
            _directionNormsArray[index + 1],
            _directionNormsArray[index]
        ).ScalarValue;
    }

    /// <summary>
    /// Get all curvatures.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<T> GetCurvatures()
    {
        var scalarProcessor = ScalarProcessor;

        for (var i = 0; i < _directionNormsArray.Length - 1; i++)
            yield return scalarProcessor.Divide(
                _directionNormsArray[i + 1],
                _directionNormsArray[i]
            ).ScalarValue;
    }

    /// <summary>
    /// Get the Darboux blade (bivector) at index i: k_i * (e_i ∧ e_{i+1})
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XGaBivector<T> GetDarbouxBlade(int index)
    {
        var curvature = GetCurvature(index);
        var e1 = UnitDirections[index];
        var e2 = UnitDirections[index + 1];

        return e1.Op(e2).Times(curvature).GetBivectorPart();
    }

    /// <summary>
    /// Get all Darboux blades.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<XGaBivector<T>> GetDarbouxBlades()
    {
        for (var i = 0; i < _directionNormsArray.Length - 1; i++)
            yield return GetDarbouxBlade(i);
    }

    /// <summary>
    /// Get the Darboux bivector (sum of all Darboux blades).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XGaBivector<T> GetDarbouxBivector()
    {
        return GetDarbouxBlades().Aggregate(
            Processor.BivectorZero,
            (a, b) => a.Add(b)
        );
    }
}
