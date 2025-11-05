using System.Diagnostics;
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.Parametric.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;

/// <summary>
/// Implementation of the Centripetal Catmull-Rom spline
/// https://en.wikipedia.org/wiki/Centripetal_Catmull%E2%80%93Rom_spline
/// </summary>
/// <typeparam name="T">Scalar type</typeparam>
public sealed class CatmullRomSplinePath3D<T> :
    ParametricPath3D<T>
{
    public sealed record SplineSegmentData(int KnotIndex1, int KnotIndex2, Scalar<T> ParameterValue);


    private readonly Scalar<T>[] _knotList;
    private readonly List<ILinVector3D<T>> _pointList;

    public CatmullRomSplineType CurveType { get; }

    public bool IsClosed { get; }

    public IEnumerable<ILinVector3D<T>> ControlPoints
        => _pointList;

    public int ControlPointCount
        => _pointList.Count;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static LinVector3D<T> ToLinVector(IScalarProcessor<T> processor, ILinVector3D<T> point)
    {
        return LinVector3D<T>.Create(processor, point.X.ScalarValue, point.Y.ScalarValue, point.Z.ScalarValue);
    }


    public CatmullRomSplinePath3D(bool isPeriodic, IEnumerable<ILinVector3D<T>> inputPointList, CatmullRomSplineType curveType, bool isClosed)
        : base(ScalarRange<T>.ZeroToOne(inputPointList.First().ScalarProcessor), isPeriodic)
    {
        CurveType = curveType;
        IsClosed = isClosed;
        _pointList = new List<ILinVector3D<T>>(inputPointList);

        var scalarProcessor = _pointList[0].ScalarProcessor;
        ILinVector3D<T> endPoint1, endPoint2;

        // Handle single-point spline as a degenerate case (constant path)
        if (_pointList.Count == 1)
        {
            var singlePoint = _pointList[0];
            endPoint1 = singlePoint;
            endPoint2 = singlePoint;
        }
        else if (isClosed)
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

            endPoint1 = LinVector3D<T>.Create(
                scalarProcessor,
                (two * p0.X - p1.X).ScalarValue,
                (two * p0.Y - p1.Y).ScalarValue,
                (two * p0.Z - p1.Z).ScalarValue
            );
            endPoint2 = LinVector3D<T>.Create(
                scalarProcessor,
                (two * pLast.X - pSecondLast.X).ScalarValue,
                (two * pLast.Y - pSecondLast.Y).ScalarValue,
                (two * pLast.Z - pSecondLast.Z).ScalarValue
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
            var dz = p1.Z - p0.Z;
            var ds = dx * dx + dy * dy + dz * dz;

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

        // Normalize knot list to [0, 1] range
        // For single-point splines (tRange == 0), skip normalization
        var tRangeValue = scalarProcessor.ToFloat64(tRange.ScalarValue);
        if (tRangeValue > scalarProcessor.ZeroEpsilon)
        {
            var tRangeInv = scalarProcessor.One / tRange;

            for (var i = 0; i < _knotList.Length; i++)
                _knotList[i] = (_knotList[i] - tMin) * tRangeInv;
        }
        else
        {
            // Degenerate case: all knots are the same, just set to 0
            for (var i = 0; i < _knotList.Length; i++)
                _knotList[i] = scalarProcessor.Zero;
        }
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

        // Check if indices are in valid range for Catmull-Rom interpolation
        // If not, return constant X value (e.g., degenerate/single-point splines)
        if (!(index2 == index1 + 1 && index1 >= 1 && index2 <= _knotList.Length - 2))
        {
            var dataPointIndex = Math.Max(0, Math.Min(1, _pointList.Count - 2));
            return _pointList[dataPointIndex].X;
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

        // Check if indices are in valid range for Catmull-Rom interpolation
        // If not, return constant Y value (e.g., degenerate/single-point splines)
        if (!(index2 == index1 + 1 && index1 >= 1 && index2 <= _knotList.Length - 2))
        {
            var dataPointIndex = Math.Max(0, Math.Min(1, _pointList.Count - 2));
            return _pointList[dataPointIndex].Y;
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
            var one = t.ScalarProcessor.One;

            return (one - t) * _pointList[0].Z + t * _pointList[1].Z;
        }

        if (index1 == _knotList.Length - 2 && index2 == _knotList.Length - 1)
        {
            var t = (parameterValue - _knotList[^2]) / (_knotList[^1] - _knotList[^2]);
            var one = t.ScalarProcessor.One;

            return (one - t) * _pointList[^2].Z + t * _pointList[^1].Z;
        }

        // Check if indices are in valid range for Catmull-Rom interpolation
        // If not, return constant Z value (e.g., degenerate/single-point splines)
        if (!(index2 == index1 + 1 && index1 >= 1 && index2 <= _knotList.Length - 2))
        {
            var dataPointIndex = Math.Max(0, Math.Min(1, _pointList.Count - 2));
            return _pointList[dataPointIndex].Z;
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
        var scalarProcessor = parameterValue.ScalarProcessor;

        // Handle edge cases
        if (parameterValue <= _knotList[0])
            return LinVector3D<T>.Create(scalarProcessor, _pointList[0].X.ScalarValue, _pointList[0].Y.ScalarValue, _pointList[0].Z.ScalarValue);

        if (parameterValue >= _knotList[^1])
            return LinVector3D<T>.Create(scalarProcessor, _pointList[^1].X.ScalarValue, _pointList[^1].Y.ScalarValue, _pointList[^1].Z.ScalarValue);

        var (index1, index2) =
            GetKnotIndexContaining(parameterValue, 0, _knotList.Length - 1);

        if (index1 == index2)
            return LinVector3D<T>.Create(scalarProcessor, _pointList[index1].X.ScalarValue, _pointList[index1].Y.ScalarValue, _pointList[index1].Z.ScalarValue);

        if (index1 == 0 && index2 == 1)
        {
            var t = (parameterValue - _knotList[0]) / (_knotList[1] - _knotList[0]);
            var one = scalarProcessor.One;

            var p0 = _pointList[0];
            var p1 = _pointList[1];

            return LinVector3D<T>.Create(
                scalarProcessor,
                ((one - t) * p0.X + t * p1.X).ScalarValue,
                ((one - t) * p0.Y + t * p1.Y).ScalarValue,
                ((one - t) * p0.Z + t * p1.Z).ScalarValue
            );
        }

        if (index1 == _knotList.Length - 2 && index2 == _knotList.Length - 1)
        {
            var t = (parameterValue - _knotList[^2]) / (_knotList[^1] - _knotList[^2]);
            var one = scalarProcessor.One;

            var p0 = _pointList[^2];
            var p1 = _pointList[^1];

            return LinVector3D<T>.Create(
                scalarProcessor,
                ((one - t) * p0.X + t * p1.X).ScalarValue,
                ((one - t) * p0.Y + t * p1.Y).ScalarValue,
                ((one - t) * p0.Z + t * p1.Z).ScalarValue
            );
        }

        // Check if indices are in valid range for Catmull-Rom interpolation
        // If not, return constant point (e.g., degenerate/single-point splines)
        if (!(index2 == index1 + 1 && index1 >= 1 && index2 <= _knotList.Length - 2))
        {
            // Degenerate case - return first data point (original input point, not control point)
            var dataPointIndex = Math.Max(0, Math.Min(1, _pointList.Count - 2));
            return LinVector3D<T>.Create(
                scalarProcessor,
                _pointList[dataPointIndex].X.ScalarValue,
                _pointList[dataPointIndex].Y.ScalarValue,
                _pointList[dataPointIndex].Z.ScalarValue
            );
        }

        // General case
        Debug.Assert(
            index2 == index1 + 1 &&
            index1 >= 1 &&
            index2 <= _knotList.Length - 2
        );

        var tQuad = _knotList.GetItemQuad(index1 - 1);

        // Create Quad<LinVector3D<T>> from 4 consecutive points
        var pQuad = new Quad<LinVector3D<T>>(
            ToLinVector(scalarProcessor, _pointList[index1 - 1]),
            ToLinVector(scalarProcessor, _pointList[index1]),
            ToLinVector(scalarProcessor, _pointList[index1 + 1]),
            ToLinVector(scalarProcessor, _pointList[index1 + 2])
        );

        return parameterValue.GetCatmullRomValue(tQuad, pQuad);
    }

    public override ParametricPath3D<T> ToFinitePath()
    {
        if (IsFinite)
            return this;

        throw new NotImplementedException();
    }

    public override ParametricPath3D<T> ToPeriodicPath()
    {
        if (IsPeriodic)
            return this;

        throw new NotImplementedException();
    }

    public override LinVector3D<T> GetDerivative1Value(Scalar<T> parameterValue)
    {
        var scalarProcessor = parameterValue.ScalarProcessor;

        if (parameterValue <= _knotList[0] || parameterValue >= _knotList[^1])
        {
            // Edge cases - use numerical differentiation via INumericalOperations<T>
            var ops = scalarProcessor.NumericalOperations;
            if (ops is null)
                throw new NotSupportedException(
                    "Numerical differentiation at boundary requires INumericalOperations<T>, " +
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
            var ops = scalarProcessor.NumericalOperations;
            if (ops is null)
                throw new NotSupportedException(
                    "Derivative at exact knot point requires INumericalOperations<T>, " +
                    "which is not available for this scalar type.");

            return LinVector3D<T>.Create(
                ops.Differentiate(GetPointX, parameterValue),
                ops.Differentiate(GetPointY, parameterValue),
                ops.Differentiate(GetPointZ, parameterValue)
            );
        }

        // Check if indices are in valid range for Catmull-Rom interpolation
        // If not, fall back to numerical differentiation (e.g., degenerate/single-point splines)
        if (!(index2 == index1 + 1 && index1 >= 1 && index2 <= _knotList.Length - 2))
        {
            var ops = scalarProcessor.NumericalOperations;
            if (ops is null)
                throw new NotSupportedException(
                    "Derivative in degenerate case requires INumericalOperations<T>, " +
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

        var p0 = _pointList[index1 - 1];
        var p1 = _pointList[index1];
        var p2 = _pointList[index1 + 1];
        var p3 = _pointList[index1 + 2];

        var pQuad = new Quad<LinVector3D<T>>(
            ToLinVector(scalarProcessor, p0),
            ToLinVector(scalarProcessor, p1),
            ToLinVector(scalarProcessor, p2),
            ToLinVector(scalarProcessor, p3)
        );

        return parameterValue.GetCatmullRomDerivativeValue(tQuad, pQuad);
    }

    public override LinVector3D<T> GetDerivative2Value(Scalar<T> parameterValue)
    {
        var scalarProcessor = parameterValue.ScalarProcessor;

        if (parameterValue <= _knotList[0] || parameterValue >= _knotList[^1])
        {
            // Edge cases - use numerical differentiation via INumericalOperations<T>
            var ops = scalarProcessor.NumericalOperations;
            if (ops is null)
                throw new NotSupportedException(
                    "Numerical second derivative at boundary requires INumericalOperations<T>, " +
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
            var ops = scalarProcessor.NumericalOperations;
            if (ops is null)
                throw new NotSupportedException(
                    "Second derivative at exact knot point requires INumericalOperations<T>, " +
                    "which is not available for this scalar type.");

            return LinVector3D<T>.Create(
                ops.Differentiate2(GetPointX, parameterValue),
                ops.Differentiate2(GetPointY, parameterValue),
                ops.Differentiate2(GetPointZ, parameterValue)
            );
        }

        // Check if indices are in valid range for Catmull-Rom interpolation
        // If not, fall back to numerical differentiation (e.g., degenerate/single-point splines)
        if (!(index2 == index1 + 1 && index1 >= 1 && index2 <= _knotList.Length - 2))
        {
            var ops = scalarProcessor.NumericalOperations;
            if (ops is null)
                throw new NotSupportedException(
                    "Second derivative in degenerate case requires INumericalOperations<T>, " +
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

        var p0 = _pointList[index1 - 1];
        var p1 = _pointList[index1];
        var p2 = _pointList[index1 + 1];
        var p3 = _pointList[index1 + 2];

        var pQuad = new Quad<LinVector3D<T>>(
            ToLinVector(scalarProcessor, p0),
            ToLinVector(scalarProcessor, p1),
            ToLinVector(scalarProcessor, p2),
            ToLinVector(scalarProcessor, p3)
        );

        return parameterValue.GetCatmullRomDerivative2Value(tQuad, pQuad);
    }

}
