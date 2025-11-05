using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.Parametric.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;

/// <summary>
/// Implementation of the Centripetal Catmull-Rom spline
/// https://en.wikipedia.org/wiki/Centripetal_Catmull%E2%80%93Rom_spline
/// </summary>
public sealed class CatmullRomSplinePath3D<T> :
    ParametricPath3D<T>
{
    public sealed record SplineSegmentData(int KnotIndex1, int KnotIndex2, Scalar<T> ParameterValue);


    private readonly Scalar<T>[] _knotList;
    private readonly List<LinVector3D<T>> _pointList;

    public CatmullRomSplineType CurveType { get; }

    public bool IsClosed { get; }

    public IEnumerable<LinVector3D<T>> ControlPoints
        => _pointList;

    public int ControlPointCount
        => _pointList.Count;


    public CatmullRomSplinePath3D(bool isPeriodic, IEnumerable<LinVector3D<T>> inputPointList, CatmullRomSplineType curveType, bool isClosed, IScalarProcessor<T> scalarProcessor)
        : base(ScalarRange<T>.Create(scalarProcessor.Zero, scalarProcessor.One), isPeriodic)
    {
        CurveType = curveType;
        IsClosed = isClosed;
        _pointList = new List<LinVector3D<T>>(inputPointList);

        var processor = scalarProcessor;

        LinVector3D<T> endPoint1, endPoint2;

        if (isClosed)
        {
            // Make sure the first and last points are the same.
            var distanceSquared = (_pointList[0] - _pointList[^1]).VectorENormSquared();
            var tolerance = processor.ScalarFromNumber(1e-12);
            if (distanceSquared < tolerance * tolerance)
                _pointList.RemoveAt(_pointList.Count - 1);

            _pointList.Add(_pointList[0]);

            // Use the second and second from last points as control points.
            endPoint1 = _pointList[^2];
            endPoint2 = _pointList[1];
        }
        else
        {
            // Extend the curve by two control points
            var two = processor.ScalarFromNumber(2);
            endPoint1 = two * _pointList[0] - _pointList[1];
            endPoint2 = two * _pointList[^1] - _pointList[^2];
        }

        // Insert control points at both ends.
        _pointList.Insert(0, endPoint1);
        _pointList.Add(endPoint2);

        _knotList = new Scalar<T>[_pointList.Count];
        _knotList[0] = processor.Zero;

        var total = processor.Zero;
        for (var i = 1; i < _pointList.Count; i++)
        {
            var vector = _pointList[i] - _pointList[i - 1];
            var ds = vector.VectorENormSquared();

            var power =
                curveType == CatmullRomSplineType.Centripetal
                    ? 0.25 : 0.5;

            // Math.Pow equivalent for Generic<T>
            // For Centripetal: ds^0.25 = sqrt(sqrt(ds))
            // For Chordal: ds^0.5 = sqrt(ds)
            Scalar<T> dsRoot;
            if (processor is IScalarProcessor<double> doubleProc)
            {
                var dsDouble = Convert.ToDouble(ds.ScalarValue);
                var rootDouble = Math.Pow(dsDouble, power);
                dsRoot = processor.ScalarFromValue((T)(object)rootDouble);
            }
            else
            {
                // Fallback: use repeated square root for other types
                dsRoot = ds; // For Uniform type or when Math.Pow not available
                if (curveType != CatmullRomSplineType.Uniform)
                {
                    // This is a limitation - Math.Pow not available for all Generic<T>
                    throw new NotSupportedException(
                        $"Catmull-Rom spline with {curveType} type requires Math.Pow, " +
                        "which is only available for Generic<double>. " +
                        "Use Generic<double> or CatmullRomSplineType.Uniform.");
                }
            }

            total += dsRoot;

            _knotList[i] = total;
        }

        var tMin = _knotList[1];
        var tMax = _knotList[^2];
        var tRangeInv = processor.One / (tMax - tMin);

        for (var i = 0; i < _knotList.Length; i++)
            _knotList[i] = (_knotList[i] - tMin) * tRangeInv;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsValid()
    {
        return _pointList.All(p => p.IsValid());
    }


    public Pair<int> GetKnotIndexContaining(Scalar<T> parameterValue)
    {
        return GetKnotIndexContaining(parameterValue, 0, _knotList.Length - 1);
    }

    private Pair<int> GetKnotIndexContaining(Scalar<T> parameterValue, int index1, int index2)
    {
        while (true)
        {
            if (index2 == index1 || index2 == index1 + 1)
                return new Pair<int>(index1, index2);

            var indexMid = (index1 + index2) / 2;
            var tMid = _knotList[indexMid];

            if (parameterValue < tMid)
            {
                index2 = indexMid;
                continue;
            }

            if (parameterValue > tMid)
            {
                index1 = indexMid;
                continue;
            }

            return new Pair<int>(indexMid, indexMid);
        }
    }

    public Scalar<T> GetPointX(Scalar<T> parameterValue)
    {
        // Handle edge cases
        if (parameterValue <= _knotList[0])
            return _pointList[0].X;

        if (parameterValue >= _knotList[^1])
            return _pointList[^1].X;

        var (index1, index2) =
            GetKnotIndexContaining(parameterValue, 0, _knotList.Length - 1);

        if (index1 == index2)
            return _pointList[index1].X;

        if (index1 == 0 && index2 == 1)
        {
            var t = (parameterValue - _knotList[0]) / (_knotList[1] - _knotList[0]);

            // Linear interpolation: (1-t)*p0 + t*p1
            return (parameterValue.ScalarProcessor.One - t) * _pointList[0].X + t * _pointList[1].X;
        }

        if (index1 == _knotList.Length - 2 && index2 == _knotList.Length - 1)
        {
            var t = (parameterValue - _knotList[^2]) / (_knotList[^1] - _knotList[^2]);

            return (parameterValue.ScalarProcessor.One - t) * _pointList[^2].X + t * _pointList[^1].X;
        }

        // General case
        Debug.Assert(
            index2 == index1 + 1 &&
            index1 >= 1 &&
            index2 <= _knotList.Length - 2
        );

        var tQuad = _knotList.GetItemQuad(index1 - 1);
        var xQuad = _pointList.GetTupleXQuad(index1 - 1);

        return parameterValue.GetCatmullRomValue(tQuad, xQuad);
    }

    public Scalar<T> GetPointY(Scalar<T> parameterValue)
    {
        // Handle edge cases
        if (parameterValue <= _knotList[0])
            return _pointList[0].Y;

        if (parameterValue >= _knotList[^1])
            return _pointList[^1].Y;

        var (index1, index2) =
            GetKnotIndexContaining(parameterValue, 0, _knotList.Length - 1);

        if (index1 == index2)
            return _pointList[index1].Y;

        if (index1 == 0 && index2 == 1)
        {
            var t = (parameterValue - _knotList[0]) / (_knotList[1] - _knotList[0]);

            return (parameterValue.ScalarProcessor.One - t) * _pointList[0].Y + t * _pointList[1].Y;
        }

        if (index1 == _knotList.Length - 2 && index2 == _knotList.Length - 1)
        {
            var t = (parameterValue - _knotList[^2]) / (_knotList[^1] - _knotList[^2]);

            return (parameterValue.ScalarProcessor.One - t) * _pointList[^2].Y + t * _pointList[^1].Y;
        }

        // General case
        Debug.Assert(
            index2 == index1 + 1 &&
            index1 >= 1 &&
            index2 <= _knotList.Length - 2
        );

        var tQuad = _knotList.GetItemQuad(index1 - 1);
        var yQuad = _pointList.GetTupleYQuad(index1 - 1);

        return parameterValue.GetCatmullRomValue(tQuad, yQuad);
    }

    public Scalar<T> GetPointZ(Scalar<T> parameterValue)
    {
        // Handle edge cases
        if (parameterValue <= _knotList[0])
            return _pointList[0].Z;

        if (parameterValue >= _knotList[^1])
            return _pointList[^1].Z;

        var (index1, index2) =
            GetKnotIndexContaining(parameterValue, 0, _knotList.Length - 1);

        if (index1 == index2)
            return _pointList[index1].Z;

        if (index1 == 0 && index2 == 1)
        {
            var t = (parameterValue - _knotList[0]) / (_knotList[1] - _knotList[0]);

            return (parameterValue.ScalarProcessor.One - t) * _pointList[0].Z + t * _pointList[1].Z;
        }

        if (index1 == _knotList.Length - 2 && index2 == _knotList.Length - 1)
        {
            var t = (parameterValue - _knotList[^2]) / (_knotList[^1] - _knotList[^2]);

            return (parameterValue.ScalarProcessor.One - t) * _pointList[^2].Z + t * _pointList[^1].Z;
        }

        // General case
        Debug.Assert(
            index2 == index1 + 1 &&
            index1 >= 1 &&
            index2 <= _knotList.Length - 2
        );

        var tQuad = _knotList.GetItemQuad(index1 - 1);
        var zQuad = _pointList.GetTupleZQuad(index1 - 1);

        return parameterValue.GetCatmullRomValue(tQuad, zQuad);
    }

    public override LinVector3D<T> GetValue(Scalar<T> parameterValue)
    {
        // Handle edge cases
        if (parameterValue <= _knotList[0])
            return _pointList[0];

        if (parameterValue >= _knotList[^1])
            return _pointList[^1];

        var (index1, index2) =
            GetKnotIndexContaining(parameterValue, 0, _knotList.Length - 1);

        if (index1 == index2)
            return _pointList[index1];

        if (index1 == 0 && index2 == 1)
        {
            var t = (parameterValue - _knotList[0]) / (_knotList[1] - _knotList[0]);
            var processor = parameterValue.ScalarProcessor;
            var oneMinusT = processor.One - t;

            return LinVector3D<T>.Create(
                oneMinusT * _pointList[0].X + t * _pointList[1].X,
                oneMinusT * _pointList[0].Y + t * _pointList[1].Y,
                oneMinusT * _pointList[0].Z + t * _pointList[1].Z
            );
        }

        if (index1 == _knotList.Length - 2 && index2 == _knotList.Length - 1)
        {
            var t = (parameterValue - _knotList[^2]) / (_knotList[^1] - _knotList[^2]);
            var processor = parameterValue.ScalarProcessor;
            var oneMinusT = processor.One - t;

            return LinVector3D<T>.Create(
                oneMinusT * _pointList[^2].X + t * _pointList[^1].X,
                oneMinusT * _pointList[^2].Y + t * _pointList[^1].Y,
                oneMinusT * _pointList[^2].Z + t * _pointList[^1].Z
            );
        }

        // General case
        Debug.Assert(
            index2 == index1 + 1 &&
            index1 >= 1 &&
            index2 <= _knotList.Length - 2
        );

        var tQuad = _knotList.GetItemQuad(index1 - 1);
        var xQuad = _pointList.GetTupleXQuad(index1 - 1);
        var yQuad = _pointList.GetTupleYQuad(index1 - 1);
        var zQuad = _pointList.GetTupleZQuad(index1 - 1);

        var x = parameterValue.GetCatmullRomValue(tQuad, xQuad);
        var y = parameterValue.GetCatmullRomValue(tQuad, yQuad);
        var z = parameterValue.GetCatmullRomValue(tQuad, zQuad);

        return LinVector3D<T>.Create(x, y, z);
    }

    public override ParametricPath3D<T> ToFinitePath()
    {
        throw new NotImplementedException();
    }

    public override ParametricPath3D<T> ToPeriodicPath()
    {
        throw new NotImplementedException();
    }

    public override LinVector3D<T> GetDerivative1Value(Scalar<T> parameterValue)
    {
        if (parameterValue <= TimeRange.MinValue || parameterValue >= TimeRange.MaxValue)
        {
            // Edge cases - use numerical differentiation via INumericalOperations<T>
            var ops = parameterValue.ScalarProcessor.NumericalOperations;
            if (ops is null)
                throw new NotSupportedException(
                    "Numerical differentiation at endpoints requires INumericalOperations<T>, " +
                    "which is not available for this scalar type.");

            return LinVector3D<T>.Create(
                ops.Differentiate(GetPointX, parameterValue),
                ops.Differentiate(GetPointY, parameterValue),
                ops.Differentiate(GetPointZ, parameterValue)
            );
        }

        var (index1, index2) =
            GetKnotIndexContaining(parameterValue, 0, _knotList.Length - 1);

        if (index1 == index2)
        {
            // Single point - use numerical differentiation via INumericalOperations<T>
            var ops = parameterValue.ScalarProcessor.NumericalOperations;
            if (ops is null)
                throw new NotSupportedException(
                    "Derivative at a single knot point requires INumericalOperations<T>, " +
                    "which is not available for this scalar type.");

            return LinVector3D<T>.Create(
                ops.Differentiate(GetPointX, parameterValue),
                ops.Differentiate(GetPointY, parameterValue),
                ops.Differentiate(GetPointZ, parameterValue)
            );
        }

        Debug.Assert(
            index2 == index1 + 1 &&
            index1 >= 1 &&
            index2 <= _knotList.Length - 2
        );

        var tQuad = _knotList.GetItemQuad(index1 - 1);
        var xQuad = _pointList.GetTupleXQuad(index1 - 1);
        var yQuad = _pointList.GetTupleYQuad(index1 - 1);
        var zQuad = _pointList.GetTupleZQuad(index1 - 1);

        var x = parameterValue.GetCatmullRomDerivativeValue(tQuad, xQuad);
        var y = parameterValue.GetCatmullRomDerivativeValue(tQuad, yQuad);
        var z = parameterValue.GetCatmullRomDerivativeValue(tQuad, zQuad);

        return LinVector3D<T>.Create(x, y, z);
    }

    public override LinVector3D<T> GetDerivative2Value(Scalar<T> parameterValue)
    {
        if (parameterValue <= TimeRange.MinValue || parameterValue >= TimeRange.MaxValue)
        {
            // Edge cases - use numerical differentiation via INumericalOperations<T>
            var ops = parameterValue.ScalarProcessor.NumericalOperations;
            if (ops is null)
                throw new NotSupportedException(
                    "Numerical differentiation at endpoints requires INumericalOperations<T>, " +
                    "which is not available for this scalar type.");

            return LinVector3D<T>.Create(
                ops.Differentiate2(GetPointX, parameterValue),
                ops.Differentiate2(GetPointY, parameterValue),
                ops.Differentiate2(GetPointZ, parameterValue)
            );
        }

        var (index1, index2) =
            GetKnotIndexContaining(parameterValue, 0, _knotList.Length - 1);

        if (index1 == index2)
        {
            // Single point - use numerical differentiation via INumericalOperations<T>
            var ops = parameterValue.ScalarProcessor.NumericalOperations;
            if (ops is null)
                throw new NotSupportedException(
                    "Second derivative at a single knot point requires INumericalOperations<T>, " +
                    "which is not available for this scalar type.");

            return LinVector3D<T>.Create(
                ops.Differentiate2(GetPointX, parameterValue),
                ops.Differentiate2(GetPointY, parameterValue),
                ops.Differentiate2(GetPointZ, parameterValue)
            );
        }

        Debug.Assert(
            index2 == index1 + 1 &&
            index1 >= 1 &&
            index2 <= _knotList.Length - 2
        );

        var tQuad = _knotList.GetItemQuad(index1 - 1);
        var xQuad = _pointList.GetTupleXQuad(index1 - 1);
        var yQuad = _pointList.GetTupleYQuad(index1 - 1);
        var zQuad = _pointList.GetTupleZQuad(index1 - 1);

        var x = parameterValue.GetCatmullRomDerivative2Value(tQuad, xQuad);
        var y = parameterValue.GetCatmullRomDerivative2Value(tQuad, yQuad);
        var z = parameterValue.GetCatmullRomDerivative2Value(tQuad, zQuad);

        return LinVector3D<T>.Create(x, y, z);
    }
}
