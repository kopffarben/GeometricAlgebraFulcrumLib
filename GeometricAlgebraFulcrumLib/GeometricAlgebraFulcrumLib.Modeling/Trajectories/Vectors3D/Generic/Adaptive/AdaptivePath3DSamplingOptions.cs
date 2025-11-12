using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Adaptive;

/// <summary>
/// Configuration options for adaptive path sampling.
/// Controls refinement criteria based on distance and angle thresholds.
/// </summary>
/// <typeparam name="T">The scalar type (double, float, etc.)</typeparam>
public sealed class AdaptivePath3DSamplingOptions<T>
{
    private Scalar<T> _maxEdgeFramesParameterDistance;
    public Scalar<T> MaxEdgeFramesParameterDistance
    {
        get => _maxEdgeFramesParameterDistance;
        set
        {
            var sp = value.ScalarProcessor;
            if (sp.IsNegative(value.ScalarValue))
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be >= 0");

            _maxEdgeFramesParameterDistance = value;
        }
    }

    private Scalar<T> _maxEdgeFramesDistance;
    public Scalar<T> MaxEdgeFramesDistance
    {
        get => _maxEdgeFramesDistance;
        set
        {
            var sp = value.ScalarProcessor;
            if (sp.IsZero(value.ScalarValue) || sp.IsNegative(value.ScalarValue))
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be > 0");

            _maxEdgeFramesDistance = value;
        }
    }

    private LinAngle<T> _maxEdgeFramesAngle;
    public LinAngle<T> MaxEdgeFramesAngle
    {
        get => _maxEdgeFramesAngle;
        set
        {
            var sp = value.ScalarProcessor;
            var degreesValue = value.DegreesValue;

            if (sp.IsZero(degreesValue) || sp.IsNegative(degreesValue))
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be > 0 degrees");

            var degrees180Value = sp.ScalarFromNumber(180).ScalarValue;
            var diff = sp.Subtract(degreesValue, degrees180Value).ScalarValue;
            if (sp.IsPositive(diff))
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be <= 180 degrees");

            _maxEdgeFramesAngle = value;
        }
    }

    private int _maxLevelCount = 10;
    public int MaxLevelCount
    {
        get => _maxLevelCount;
        set
        {
            if (value is < 2 or > 30)
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be >= 2 and <= 30");

            _maxLevelCount = value;
        }
    }

    private int _minLevelCount = 3;
    public int MinLevelCount
    {
        get => _minLevelCount;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be >= 0");

            _minLevelCount = value;
        }
    }


    public AdaptivePath3DSamplingOptions(IScalarProcessor<T> scalarProcessor, LinAngle<T> maxAngleError, int minLevelCount, int maxLevelCount)
    {
        _maxEdgeFramesDistance = scalarProcessor.ScalarFromNumber(1e-5);
        _maxEdgeFramesParameterDistance = scalarProcessor.Zero;
        _maxEdgeFramesAngle = maxAngleError;

        MaxEdgeFramesAngle = maxAngleError;
        MinLevelCount = minLevelCount;
        MaxLevelCount = maxLevelCount;
    }
}
