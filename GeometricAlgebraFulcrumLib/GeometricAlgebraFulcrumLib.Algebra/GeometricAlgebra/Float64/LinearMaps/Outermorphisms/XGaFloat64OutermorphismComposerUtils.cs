using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.LinearMaps.SpaceND;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.SpaceND;
using GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets;

namespace GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Outermorphisms;

public static class XGaFloat64OutermorphismComposerUtils
{
    /// <summary>
    /// Create a computed outermorphism from a function that maps basis vectors.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static XGaFloat64ComputedOutermorphism CreateComputedOutermorphism(
        this XGaFloat64Processor processor,
        Func<int, XGaFloat64Vector> basisMapFunc)
    {
        return new XGaFloat64ComputedOutermorphism(basisMapFunc, processor);
    }

    /// <summary>
    /// Create a stored outermorphism from a dictionary that maps basis blade IDs to k-vectors.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static XGaFloat64StoredOutermorphism CreateStoredOutermorphism(
        this XGaFloat64Processor processor,
        IReadOnlyDictionary<IndexSet, XGaFloat64KVector> basisMapDictionary)
    {
        return new XGaFloat64StoredOutermorphism(basisMapDictionary, processor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static XGaFloat64LinearMapOutermorphism ColumnsToOutermorphism(this double[,] vectorMapMatrix, XGaFloat64Processor processor)
    {
        var linearMap = vectorMapMatrix.ColumnsToLinVectors().ToLinUnilinearMap();

        return new XGaFloat64LinearMapOutermorphism(processor, linearMap);
    }
}