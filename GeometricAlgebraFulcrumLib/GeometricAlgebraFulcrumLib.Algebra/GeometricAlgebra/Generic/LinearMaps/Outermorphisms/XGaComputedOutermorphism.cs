using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Basis;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.LinearMaps;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.BitManipulation;
using GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets;

namespace GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.LinearMaps.Outermorphisms;

/// <summary>
/// This class represents an outermorphism defined by a function that computes
/// the mapping of basis vectors on demand.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class XGaComputedOutermorphism<T> :
    IXGaOutermorphism<T>
{
    public Func<int, XGaVector<T>> BasisMapFunc { get; }

    public XGaProcessor<T> Processor { get; }

    public XGaMetric Metric
        => Processor;

    public IScalarProcessor<T> ScalarProcessor
        => Processor.ScalarProcessor;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal XGaComputedOutermorphism(Func<int, XGaVector<T>> basisMapFunc, XGaProcessor<T> processor)
    {
        BasisMapFunc = basisMapFunc;
        Processor = processor;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid()
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IXGaUnilinearMap<T> GetAdjoint()
    {
        return GetOmAdjoint();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XGaMultivector<T> MapBasisBlade(IndexSet id)
    {
        return OmMapBasisBlade(id);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XGaMultivector<T> Map(XGaMultivector<T> multivector)
    {
        return OmMap(multivector);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<KeyValuePair<IndexSet, XGaMultivector<T>>> GetMappedBasisBlades(int vSpaceDimensions)
    {
        return Processor
            .GetBasisBladeIds(vSpaceDimensions)
            .Select(id =>
                new KeyValuePair<IndexSet, XGaMultivector<T>>(
                    id,
                    OmMapBasisBlade(id)
                )
            );
    }


    public IXGaOutermorphism<T> GetOmAdjoint()
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XGaVector<T> OmMapBasisVector(int index)
    {
        return BasisMapFunc(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XGaBivector<T> OmMapBasisBivector(int index1, int index2)
    {
        if (index1 == index2) return Processor.BivectorZero;

        var v1 = OmMapBasisVector(index1);
        if (v1.IsZero) return Processor.BivectorZero;

        var v2 = OmMapBasisVector(index2);

        return index1 < index2 ? v1.Op(v2) : v2.Op(v1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XGaKVector<T> OmMapBasisBlade(IndexSet id)
    {
        if (id.IsEmptySet)
            return Processor.ScalarOne;

        return id.IsBasisVector()
            ? OmMapBasisVector(id.FirstIndex)
            : id.Select(OmMapBasisVector).Op(Processor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XGaVector<T> OmMap(XGaVector<T> vector)
    {
        var composer = Processor.CreateVectorComposer();

        foreach (var (id, scalar) in vector.IdScalarPairs)
            composer.AddKVectorScaled(
                OmMapBasisVector(id.FirstIndex),
                scalar
            );

        return composer.GetVector();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XGaBivector<T> OmMap(XGaBivector<T> bivector)
    {
        var composer = Processor.CreateBivectorComposer();

        foreach (var (id, scalar) in bivector.IdScalarPairs)
            composer.AddKVectorScaled(
                OmMapBasisBivector(id.FirstIndex, id.LastIndex),
                scalar
            );

        return composer.GetBivector();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XGaHigherKVector<T> OmMap(XGaHigherKVector<T> kVector)
    {
        var composer = Processor.CreateKVectorComposer(kVector.Grade);

        foreach (var (id, scalar) in kVector.IdScalarPairs)
            composer.AddKVectorScaled(
                OmMapBasisBlade(id),
                scalar
            );

        return composer.GetHigherKVector();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XGaKVector<T> OmMap(XGaKVector<T> kVector)
    {
        return kVector switch
        {
            XGaScalar<T> s => s,
            XGaVector<T> v => OmMap(v),
            XGaBivector<T> bv => OmMap(bv),
            XGaHigherKVector<T> kv => OmMap(kv),
            _ => throw new InvalidOperationException()
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public XGaMultivector<T> OmMap(XGaMultivector<T> multivector)
    {
        var composer = Processor.CreateMultivectorComposer();

        foreach (var (id, scalar) in multivector.IdScalarPairs)
            composer.AddKVectorScaled(
                OmMapBasisBlade(id),
                scalar
            );

        return composer.GetMultivector();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<KeyValuePair<IndexSet, XGaVector<T>>> GetOmMappedBasisVectors(int vSpaceDimensions)
    {
        return vSpaceDimensions
            .GetRange()
            .Select(index =>
                new KeyValuePair<IndexSet, XGaVector<T>>(
                    index.ToUnitIndexSet(),
                    OmMapBasisVector(index)
                )
            );
    }

    public LinUnilinearMap<T> GetVectorMapPart(int vSpaceDimensions)
    {
        var indexVectorDictionary = vSpaceDimensions.GetRange(
                index =>
                    new KeyValuePair<int, XGaVector<T>>(
                        index,
                        OmMapBasisVector(index)
                    )
            ).Where(p => !p.Value.IsZero)
            .ToDictionary(
                p => p.Key,
                p => p.Value.ToLinVector()
            );

        return indexVectorDictionary.ToLinUnilinearMap(ScalarProcessor);
    }
}
