using GeometricAlgebraFulcrumLib.Algebra.ComplexAlgebra;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Geometry.VGa.Generic;

public sealed class XGaEuclideanGeometrySpace2D<T> :
    XGaEuclideanGeometrySpace<T>
{
    public XGaEuclideanGeometrySpace2D(IScalarProcessor<T> scalarProcessor)
        : base(scalarProcessor, 2)
    {
    }


    public XGaVector<T> EncodeVector(T x, T y)
    {
        return EuclideanProcessor.Vector(x, y);
    }

    public XGaBivector<T> EncodeBivector(T xyScalar)
    {
        return Processor.BivectorTerm(0, 1, xyScalar);
    }

    public XGaMultivector<T> EncodeComplex(T scalar, T iScalar)
    {
        return Processor
            .CreateMultivectorComposer()
            .SetScalarTerm(scalar)
            .SetBivectorTerm(0, 1, ScalarProcessor.Negative(iScalar).ScalarValue)
            .GetMultivector();
    }

    public ComplexNumber<T> DecodeComplex(XGaMultivector<T> mv)
    {
        return ScalarProcessor.CreateComplexNumber(
            mv.Scalar().ScalarValue,
            ScalarProcessor.Negative(mv[0, 1].ScalarValue).ScalarValue
        );
    }
}
