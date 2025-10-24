using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;

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
}
