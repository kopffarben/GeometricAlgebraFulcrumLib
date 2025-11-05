using System;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.VGa.Generic;

public abstract class XGaEuclideanGeometrySpace<T> :
    GaGeometricSpace<T>
{
    public XGaVector<T> E1 { get; }

    public XGaVector<T> E2 { get; }

    public XGaBivector<T> E12 { get; }

    public XGaKVector<T> I { get; }

    public XGaKVector<T> Iinv { get; }

    public XGaKVector<T> Irev { get; }


    protected XGaEuclideanGeometrySpace(IScalarProcessor<T> scalarProcessor, int vSpaceDimensions)
        : base(GaGeometricSpaceBasisSpecs<T>.CreateVGa(scalarProcessor, vSpaceDimensions))
    {
        if (vSpaceDimensions < 2)
            throw new ArgumentOutOfRangeException(nameof(vSpaceDimensions));

        E1 = EuclideanProcessor.VectorTerm(0);
        E2 = EuclideanProcessor.VectorTerm(1);

        E12 = EuclideanProcessor.BivectorTerm(0, 1);

        I = EuclideanProcessor.KVectorTerm((IndexSet)(GaSpaceDimensions - 1), ScalarProcessor.OneValue);
        Iinv = I.Inverse();
        Irev = I.Reverse();
    }

}
