using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Basis;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Utilities.Structures.BitManipulation;
using GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets;

namespace GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.LinearMaps.Outermorphisms;

public sealed class XGaStoredOutermorphism<T> :
    XGaOutermorphismBase<T>
{
    private readonly IReadOnlyDictionary<IndexSet, XGaKVector<T>> _basisMapDictionary;

    public override XGaProcessor<T> Processor { get; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal XGaStoredOutermorphism(IReadOnlyDictionary<IndexSet, XGaKVector<T>> basisMapDictionary, XGaProcessor<T> processor)
    {
        _basisMapDictionary = basisMapDictionary;
        Processor = processor;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return _basisMapDictionary.Values.All(
            d => d.IsValid()
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override IEnumerable<KeyValuePair<IndexSet, XGaMultivector<T>>> GetMappedBasisBlades(int vSpaceDimensions)
    {
        return _basisMapDictionary
            .Where(p =>
                p.Key.VSpaceDimensions() <= vSpaceDimensions
            ).Select(p =>
                new KeyValuePair<IndexSet, XGaMultivector<T>>(p.Key, p.Value)
            );
    }

    public override IXGaOutermorphism<T> GetOmAdjoint()
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override XGaVector<T> OmMapBasisVector(int index)
    {
        var id = index.ToUnitIndexSet();

        return _basisMapDictionary.TryGetValue(id, out var kVector)
            ? kVector.GetVectorPart()
            : Processor.VectorZero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override XGaBivector<T> OmMapBasisBivector(int index1, int index2)
    {
        if (index1 == index2) return Processor.BivectorZero;

        if (index1 < index2)
        {
            // Indices in ascending order - use directly
            var id = IndexSet.CreatePair(index1, index2);

            return _basisMapDictionary.TryGetValue(id, out var kVector)
                ? kVector.GetBivectorPart()
                : Processor.BivectorZero;
        }
        else
        {
            // Indices in descending order - swap and negate
            var id = IndexSet.CreatePair(index2, index1);

            return _basisMapDictionary.TryGetValue(id, out var kVector)
                ? kVector.GetBivectorPart().Negative()
                : Processor.BivectorZero;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override XGaKVector<T> OmMapBasisBlade(IndexSet id)
    {
        return _basisMapDictionary.TryGetValue(id, out var kVector)
            ? kVector
            : Processor.KVectorZero(id.Grade());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override XGaVector<T> OmMap(XGaVector<T> vector)
    {
        var composer = Processor.CreateVectorComposer();

        foreach (var (id, scalar) in vector)
            composer.AddKVectorScaled(OmMapBasisBlade(id), scalar);

        return composer.GetVector();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override XGaBivector<T> OmMap(XGaBivector<T> bivector)
    {
        var composer = Processor.CreateBivectorComposer();

        foreach (var (id, scalar) in bivector)
            composer.AddKVectorScaled(OmMapBasisBlade(id), scalar);

        return composer.GetBivector();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override XGaHigherKVector<T> OmMap(XGaHigherKVector<T> kVector)
    {
        var composer = Processor.CreateKVectorComposer(kVector.Grade);

        foreach (var (id, scalar) in kVector)
            composer.AddKVectorScaled(OmMapBasisBlade(id), scalar);

        return composer.GetHigherKVector();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override XGaMultivector<T> OmMap(XGaMultivector<T> multivector)
    {
        var composer = Processor.CreateMultivectorComposer();

        foreach (var (id, scalar) in multivector)
            composer.AddMultivectorScaled(OmMapBasisBlade(id), scalar);

        return composer.GetMultivector();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override IEnumerable<KeyValuePair<IndexSet, XGaVector<T>>> GetOmMappedBasisVectors(int vSpaceDimensions)
    {
        return vSpaceDimensions.GetRange(index =>
            new KeyValuePair<int, XGaVector<T>>(
                index,
                OmMapBasisVector(index)
            )
        ).Where(p => !p.Value.IsZero)
        .Select(p =>
            new KeyValuePair<IndexSet, XGaVector<T>>(
                p.Key.ToUnitIndexSet(),
                p.Value
            )
        );
    }

}
