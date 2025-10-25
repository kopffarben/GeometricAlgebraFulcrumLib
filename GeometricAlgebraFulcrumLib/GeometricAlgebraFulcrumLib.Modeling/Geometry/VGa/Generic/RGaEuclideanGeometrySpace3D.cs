using System.Collections.Generic;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.VGa.Generic;

public sealed class XGaEuclideanGeometrySpace3D<T> :
    XGaEuclideanGeometrySpace<T>
{
    public XGaVector<T> E3 { get; }

    public XGaBivector<T> E13 { get; }

    public XGaBivector<T> E23 { get; }


    public XGaEuclideanGeometrySpace3D(IScalarProcessor<T> scalarProcessor)
        : base(scalarProcessor, 3)
    {
        E3 = EuclideanProcessor.VectorTerm(2);

        E13 = EuclideanProcessor.BivectorTerm(0, 2);
        E23 = EuclideanProcessor.BivectorTerm(1, 2);
    }


    public XGaVector<T> EncodeVector(T x, T y, T z)
    {
        return Processor.Vector(x, y, z);
    }

    public XGaBivector<T> EncodeBivector(T xy, T xz, T yz)
    {
        return Processor.Bivector(
            new Dictionary<IndexSet, T>
            {
                {(IndexSet)3UL, xy},
                {(IndexSet)5UL, xz},
                {(IndexSet)6UL, yz}
            }
        );
    }

    public XGaMultivector<T> EncodeQuaternion(T scalar, T iScalar, T jScalar, T kScalar)
    {
        return Processor
            .CreateMultivectorComposer()
            .SetScalarTerm(scalar)
            .SetBivectorTerm(0, 1, ScalarProcessor.Negative(kScalar).ScalarValue)
            .SetBivectorTerm(0, 2, jScalar)
            .SetBivectorTerm(1, 2, ScalarProcessor.Negative(iScalar).ScalarValue)
            .GetMultivector();
    }

    public LinQuaternion<T> DecodeQuaternion(XGaMultivector<T> mv)
    {
        return LinQuaternion<T>.Create(
            ScalarProcessor,
            ScalarProcessor.Negative(mv[1, 2].ScalarValue).ScalarValue,
            mv[0, 2].ScalarValue,
            ScalarProcessor.Negative(mv[0, 1].ScalarValue).ScalarValue,
            mv.Scalar().ScalarValue
        );
    }
}
