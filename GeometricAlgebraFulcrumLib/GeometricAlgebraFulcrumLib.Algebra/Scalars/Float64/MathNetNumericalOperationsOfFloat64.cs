using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using MathNet.Numerics;

namespace GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;

/// <summary>
/// Math.NET-based numerical operations for double precision floating point.
/// Uses central finite differences for numerical differentiation.
/// </summary>
public sealed class MathNetNumericalOperationsOfFloat64 : INumericalOperations<double>
{
    /// <summary>
    /// Singleton instance for Float64 numerical operations.
    /// </summary>
    public static MathNetNumericalOperationsOfFloat64 Instance { get; }
        = new(ScalarProcessorOfFloat64.Instance);

    public IScalarProcessor<double> ScalarProcessor { get; }

    private MathNetNumericalOperationsOfFloat64(IScalarProcessor<double> processor)
    {
        ScalarProcessor = processor ?? throw new ArgumentNullException(nameof(processor));
    }

    /// <summary>
    /// Compute first derivative using Math.NET central finite differences.
    /// Accuracy: ~1e-8, Performance: ~100ns per call.
    /// </summary>
    public Scalar<double> Differentiate(
        Func<Scalar<double>, Scalar<double>> function,
        Scalar<double> point)
    {
        ArgumentNullException.ThrowIfNull(function);
        ArgumentNullException.ThrowIfNull(point);

        // Convert Scalar<double> function to raw double function for Math.NET
        double RawFunction(double x)
        {
            var scalarX = ScalarProcessor.Scalar(x);
            var result = function(scalarX);
            return result.ScalarValue;
        }

        // Use Math.NET central finite differences
        var derivativeValue = MathNet.Numerics.Differentiate.FirstDerivative(
            RawFunction,
            point.ScalarValue
        );

        return ScalarProcessor.Scalar(derivativeValue);
    }

    /// <summary>
    /// Compute second derivative using Math.NET central finite differences.
    /// </summary>
    public Scalar<double> Differentiate2(
        Func<Scalar<double>, Scalar<double>> function,
        Scalar<double> point)
    {
        ArgumentNullException.ThrowIfNull(function);
        ArgumentNullException.ThrowIfNull(point);

        double RawFunction(double x)
        {
            var scalarX = ScalarProcessor.Scalar(x);
            var result = function(scalarX);
            return result.ScalarValue;
        }

        var derivativeValue = MathNet.Numerics.Differentiate.SecondDerivative(
            RawFunction,
            point.ScalarValue
        );

        return ScalarProcessor.Scalar(derivativeValue);
    }

    /// <summary>
    /// Numerical integration - not yet implemented.
    /// TODO Phase 3: Implement using Math.NET.Numerics.Integration.
    /// </summary>
    public Scalar<double>? Integrate(
        Func<Scalar<double>, Scalar<double>> function,
        Scalar<double> a,
        Scalar<double> b)
    {
        // TODO: Implement using Math.NET.Numerics.Integration
        // Example: DoubleExponentialTransformation, GaussLegendreRule, etc.
        return null;
    }

    /// <summary>
    /// Root finding - not yet implemented.
    /// TODO Phase 3: Implement using Math.NET.Numerics.RootFinding.
    /// </summary>
    public Scalar<double>? FindRoot(
        Func<Scalar<double>, Scalar<double>> function,
        Scalar<double> initialGuess,
        Scalar<double>? tolerance = null)
    {
        // TODO: Implement using Math.NET.Numerics.RootFinding
        // Example: Brent, Newton-Raphson, Bisection, etc.
        return null;
    }
}
