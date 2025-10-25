using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.LinearMaps;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.SpaceND;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets;

namespace GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.LinearMaps.Outermorphisms;

public static class XGaOutermorphismComposerUtils
{
    /// <summary>
    /// Create a computed outermorphism from a function that maps basis vectors.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static XGaComputedOutermorphism<T> CreateComputedOutermorphism<T>(
        this XGaProcessor<T> processor,
        Func<int, XGaVector<T>> basisMapFunc)
    {
        return new XGaComputedOutermorphism<T>(basisMapFunc, processor);
    }

    /// <summary>
    /// Create a stored outermorphism from a dictionary that maps basis blade IDs to k-vectors.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static XGaStoredOutermorphism<T> CreateStoredOutermorphism<T>(
        this XGaProcessor<T> processor,
        IReadOnlyDictionary<IndexSet, XGaKVector<T>> basisMapDictionary)
    {
        return new XGaStoredOutermorphism<T>(basisMapDictionary, processor);
    }

    /// <summary>
    /// Create a linear map outermorphism from a matrix where columns represent mapped basis vectors.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static XGaLinearMapOutermorphism<T> ColumnsToOutermorphism<T>(
        this T[,] vectorMapMatrix,
        XGaProcessor<T> processor)
    {
        var linearMap = vectorMapMatrix.ColumnsToLinVectors(processor.ScalarProcessor).ToLinUnilinearMap(processor.ScalarProcessor);

        return new XGaLinearMapOutermorphism<T>(processor, linearMap);
    }
}
