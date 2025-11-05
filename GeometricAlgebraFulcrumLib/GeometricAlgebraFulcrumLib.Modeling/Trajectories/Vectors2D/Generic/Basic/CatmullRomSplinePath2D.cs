using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.Parametric.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;

/// <summary>
/// Implementation of the Centripetal Catmull-Rom spline for 2D vectors
/// https://en.wikipedia.org/wiki/Centripetal_Catmull%E2%80%93Rom_spline
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class CatmullRomSplinePath2D<T> :
    ParametricPath2D<T>
{
    public sealed record SplineSegmentData(int KnotIndex1, int KnotIndex2, Scalar<T> ParameterValue);


    private readonly Scalar<T>[] _knotList;
    private readonly List<ILinVector2D<T>> _pointList;

    public CatmullRomSplineType CurveType { get; }

    public bool IsClosed { get; }

    public IEnumerable<ILinVector2D<T>> ControlPoints
        => _pointList;

    public int ControlPointCount
        => _pointList.Count;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static LinVector2D<T> ToLinVector(IScalarProcessor<T> processor, ILinVector2D<T> point)
    {
        return LinVector2D<T>.Create(processor, point.X.ScalarValue, point.Y.ScalarValue);
    }


    public CatmullRomSplinePath2D(bool isPeriodic, IEnumerable<ILinVector2D<T>> inputPointList, CatmullRomSplineType curveType, bool isClosed)
        : base(ScalarRange<T>.ZeroToOne(inputPointList.First().ScalarProcessor), isPeriodic)
    {
        CurveType = curveType;
        IsClosed = isClosed;
        _pointList = new List<ILinVector2D<T>>(inputPointList);

        var scalarProcessor = _pointList[0].ScalarProcessor;
        ILinVector2D<T> endPoint1, endPoint2;

        if (isClosed)
        {
            // Make sure the first and last points are the same.
            var distanceSquared = _pointList[0].GetDistanceSquaredToPoint(_pointList[^1]);
            if (distanceSquared.IsNearZero())
                _pointList.RemoveAt(_pointList.Count - 1);

            _pointList.Add(_pointList[0]);

            // Use the second and second from last points as control points.
            endPoint1 = _pointList[^2];
            endPoint2 = _pointList[1];
        }
        else
        {
            // Extend the curve by two control points
            var two = scalarProcessor.ScalarFromNumber(2);
            var p0 = _pointList[0];
            var p1 = _pointList[1];
            var pLast = _pointList[^1];
            var pSecondLast = _pointList[^2];

            endPoint1 = LinVector2D<T>.Create(
                scalarProcessor,
                (two * p0.X - p1.X).ScalarValue,
                (two * p0.Y - p1.Y).ScalarValue
            );
            endPoint2 = LinVector2D<T>.Create(
                scalarProcessor,
                (two * pLast.X - pSecondLast.X).ScalarValue,
                (two * pLast.Y - pSecondLast.Y).ScalarValue
            );
        }

        // Insert control points at both ends.
        _pointList.Insert(0, endPoint1);
        _pointList.Add(endPoint2);

        _knotList = new Scalar<T>[_pointList.Count];
        _knotList[0] = scalarProcessor.Zero;  // Initialize first knot to zero

        var total = scalarProcessor.Zero;
        for (var i = 1; i < _pointList.Count; i++)
        {
            var p1 = _pointList[i];
            var p0 = _pointList[i - 1];
            var dx = p1.X - p0.X;
            var dy = p1.Y - p0.Y;
            var ds = dx * dx + dy * dy;

            // Compute power based on curve type
            Scalar<T> dsPower;
            if (curveType == CatmullRomSplineType.Centripetal)
            {
                // power = 0.25
                var dsSqrt = ds.Sqrt();  // sqrt(ds) = ds^0.5
                dsPower = dsSqrt.Sqrt();  // sqrt(sqrt(ds)) = ds^0.25
            }
            else
            {
                // Chordal: power = 0.5
                dsPower = ds.Sqrt();  // sqrt(ds) = ds^0.5
            }

            total += dsPower;

            _knotList[i] = total;
        }

        var tMin = _knotList[1];
        var tMax = _knotList[^2];
        var tRange = tMax - tMin;
        var tRangeInv = scalarProcessor.One / tRange;

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
            var one = t.ScalarProcessor.One;

            return (one - t) * _pointList[0].X + t * _pointList[1].X;
        }

        if (index1 == _knotList.Length - 2 && index2 == _knotList.Length - 1)
        {
            var t = (parameterValue - _knotList[^2]) / (_knotList[^1] - _knotList[^2]);
            var one = t.ScalarProcessor.One;

            return (one - t) * _pointList[^2].X + t * _pointList[^1].X;
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
            var one = t.ScalarProcessor.One;

            return (one - t) * _pointList[0].Y + t * _pointList[1].Y;
        }

        if (index1 == _knotList.Length - 2 && index2 == _knotList.Length - 1)
        {
            var t = (parameterValue - _knotList[^2]) / (_knotList[^1] - _knotList[^2]);
            var one = t.ScalarProcessor.One;

            return (one - t) * _pointList[^2].Y + t * _pointList[^1].Y;
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

    public override LinVector2D<T> GetValue(Scalar<T> parameterValue)
    {
        var scalarProcessor = parameterValue.ScalarProcessor;

        // Handle edge cases
        if (parameterValue <= _knotList[0])
            return LinVector2D<T>.Create(scalarProcessor, _pointList[0].X.ScalarValue, _pointList[0].Y.ScalarValue);

        if (parameterValue >= _knotList[^1])
            return LinVector2D<T>.Create(scalarProcessor, _pointList[^1].X.ScalarValue, _pointList[^1].Y.ScalarValue);

        var (index1, index2) =
            GetKnotIndexContaining(parameterValue, 0, _knotList.Length - 1);

        if (index1 == index2)
            return LinVector2D<T>.Create(scalarProcessor, _pointList[index1].X.ScalarValue, _pointList[index1].Y.ScalarValue);

        if (index1 == 0 && index2 == 1)
        {
            var t = (parameterValue - _knotList[0]) / (_knotList[1] - _knotList[0]);
            var one = scalarProcessor.One;

            var p0 = _pointList[0];
            var p1 = _pointList[1];

            return LinVector2D<T>.Create(
                scalarProcessor,
                ((one - t) * p0.X + t * p1.X).ScalarValue,
                ((one - t) * p0.Y + t * p1.Y).ScalarValue
            );
        }

        if (index1 == _knotList.Length - 2 && index2 == _knotList.Length - 1)
        {
            var t = (parameterValue - _knotList[^2]) / (_knotList[^1] - _knotList[^2]);
            var one = scalarProcessor.One;

            var p0 = _pointList[^2];
            var p1 = _pointList[^1];

            return LinVector2D<T>.Create(
                scalarProcessor,
                ((one - t) * p0.X + t * p1.X).ScalarValue,
                ((one - t) * p0.Y + t * p1.Y).ScalarValue
            );
        }

        // General case
        Debug.Assert(
            index2 == index1 + 1 &&
            index1 >= 1 &&
            index2 <= _knotList.Length - 2
        );

        var tQuad = _knotList.GetItemQuad(index1 - 1);

        // Create Quad<LinVector2D<T>> from 4 consecutive points
        var pQuad = new Quad<LinVector2D<T>>(
            ToLinVector(scalarProcessor, _pointList[index1 - 1]),
            ToLinVector(scalarProcessor, _pointList[index1]),
            ToLinVector(scalarProcessor, _pointList[index1 + 1]),
            ToLinVector(scalarProcessor, _pointList[index1 + 2])
        );

        return parameterValue.GetCatmullRomValue(tQuad, pQuad);
    }

    public override ParametricPath2D<T> ToFinitePath()
    {
        if (IsFinite)
            return this;

        throw new NotImplementedException();
    }

    public override ParametricPath2D<T> ToPeriodicPath()
    {
        if (IsPeriodic)
            return this;

        throw new NotImplementedException();
    }

    public override LinVector2D<T> GetDerivative1Value(Scalar<T> parameterValue)
    {
        var scalarProcessor = parameterValue.ScalarProcessor;

        if (parameterValue <= _knotList[0] || parameterValue >= _knotList[^1])
        {
            // Numerical differentiation not available for Generic<T>
            throw new NotImplementedException(
                "Numerical differentiation at boundary is not available for Generic<T>. " +
                "CatmullRom derivatives require values within the spline range."
            );
        }

        var (index1, index2) =
            GetKnotIndexContaining(parameterValue, 0, _knotList.Length - 1);

        if (index1 == index2)
        {
            throw new NotImplementedException(
                "Numerical differentiation at exact knot points is not available for Generic<T>."
            );
        }

        Debug.Assert(
            index2 == index1 + 1 &&
            index1 >= 1 &&
            index2 <= _knotList.Length - 2
        );

        var tQuad = _knotList.GetItemQuad(index1 - 1);

        var p0 = _pointList[index1 - 1];
        var p1 = _pointList[index1];
        var p2 = _pointList[index1 + 1];
        var p3 = _pointList[index1 + 2];

        var pQuad = new Quad<LinVector2D<T>>(
            ToLinVector(scalarProcessor, p0),
            ToLinVector(scalarProcessor, p1),
            ToLinVector(scalarProcessor, p2),
            ToLinVector(scalarProcessor, p3)
        );

        return parameterValue.GetCatmullRomDerivativeValue(tQuad, pQuad);
    }

    public override LinVector2D<T> GetDerivative2Value(Scalar<T> parameterValue)
    {
        var scalarProcessor = parameterValue.ScalarProcessor;

        if (parameterValue <= _knotList[0] || parameterValue >= _knotList[^1])
        {
            throw new NotImplementedException(
                "Numerical differentiation at boundary is not available for Generic<T>. " +
                "CatmullRom second derivatives require values within the spline range."
            );
        }

        var (index1, index2) =
            GetKnotIndexContaining(parameterValue, 0, _knotList.Length - 1);

        if (index1 == index2)
        {
            throw new NotImplementedException(
                "Numerical differentiation at exact knot points is not available for Generic<T>."
            );
        }

        Debug.Assert(
            index2 == index1 + 1 &&
            index1 >= 1 &&
            index2 <= _knotList.Length - 2
        );

        var tQuad = _knotList.GetItemQuad(index1 - 1);

        var p0 = _pointList[index1 - 1];
        var p1 = _pointList[index1];
        var p2 = _pointList[index1 + 1];
        var p3 = _pointList[index1 + 2];

        var pQuad = new Quad<LinVector2D<T>>(
            ToLinVector(scalarProcessor, p0),
            ToLinVector(scalarProcessor, p1),
            ToLinVector(scalarProcessor, p2),
            ToLinVector(scalarProcessor, p3)
        );

        return parameterValue.GetCatmullRomDerivative2Value(tQuad, pQuad);
    }

}
