using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public class ComputedPath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [SetUp]
    public void Setup()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void ComputedPath3D_FiniteConstructor_ShouldMatchFloat64()
    {
        // Create a simple quadratic path: (t², 2t², 3t²)
        var timeRange = Float64ScalarRange.Create(0, 1);
        var timeRangeGeneric = ScalarRange<double>.Create(
            ScalarProcessor.Zero,
            ScalarProcessor.One
        );

        var pathFloat64 = Float64ComputedPath3D.Finite(
            timeRange,
            t => LinFloat64Vector3D.Create(t * t, 2 * t * t, 3 * t * t)
        );

        var pathGeneric = ComputedPath3D<double>.Finite(
            timeRangeGeneric,
            t => LinVector3D<double>.Create(
                t * t,
                ScalarProcessor.ScalarFromNumber(2) * t * t,
                ScalarProcessor.ScalarFromNumber(3) * t * t
            )
        );

        Assert.That(pathGeneric.TimeRange.MinValue.ScalarValue, Is.EqualTo(pathFloat64.TimeRange.MinValue).Within(Tolerance));
        Assert.That(pathGeneric.TimeRange.MaxValue.ScalarValue, Is.EqualTo(pathFloat64.TimeRange.MaxValue).Within(Tolerance));
        Assert.That(pathGeneric.IsFinite, Is.EqualTo(pathFloat64.IsFinite));
        Assert.That(pathGeneric.IsPeriodic, Is.EqualTo(pathFloat64.IsPeriodic));
    }

    [Test]
    public void ComputedPath3D_GetValue_AtStart_ShouldMatchFloat64()
    {
        var timeRange = Float64ScalarRange.Create(0, 1);
        var timeRangeGeneric = ScalarRange<double>.Create(
            ScalarProcessor.Zero,
            ScalarProcessor.One
        );

        var pathFloat64 = Float64ComputedPath3D.Finite(
            timeRange,
            t => LinFloat64Vector3D.Create(t * t, 2 * t * t, 3 * t * t)
        );

        var pathGeneric = ComputedPath3D<double>.Finite(
            timeRangeGeneric,
            t => LinVector3D<double>.Create(
                t * t,
                ScalarProcessor.ScalarFromNumber(2) * t * t,
                ScalarProcessor.ScalarFromNumber(3) * t * t
            )
        );

        // At t=0: (0, 0, 0)
        var valueFloat64 = pathFloat64.GetValue(0.0);
        var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(0.0));

        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void ComputedPath3D_GetValue_AtEnd_ShouldMatchFloat64()
    {
        var timeRange = Float64ScalarRange.Create(0, 1);
        var timeRangeGeneric = ScalarRange<double>.Create(
            ScalarProcessor.Zero,
            ScalarProcessor.One
        );

        var pathFloat64 = Float64ComputedPath3D.Finite(
            timeRange,
            t => LinFloat64Vector3D.Create(t * t, 2 * t * t, 3 * t * t)
        );

        var pathGeneric = ComputedPath3D<double>.Finite(
            timeRangeGeneric,
            t => LinVector3D<double>.Create(
                t * t,
                ScalarProcessor.ScalarFromNumber(2) * t * t,
                ScalarProcessor.ScalarFromNumber(3) * t * t
            )
        );

        // At t=1: (1, 2, 3)
        var valueFloat64 = pathFloat64.GetValue(1.0);
        var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(1.0));

        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void ComputedPath3D_GetValue_AtMidpoint_ShouldMatchFloat64()
    {
        var timeRange = Float64ScalarRange.Create(0, 1);
        var timeRangeGeneric = ScalarRange<double>.Create(
            ScalarProcessor.Zero,
            ScalarProcessor.One
        );

        var pathFloat64 = Float64ComputedPath3D.Finite(
            timeRange,
            t => LinFloat64Vector3D.Create(t * t, 2 * t * t, 3 * t * t)
        );

        var pathGeneric = ComputedPath3D<double>.Finite(
            timeRangeGeneric,
            t => LinVector3D<double>.Create(
                t * t,
                ScalarProcessor.ScalarFromNumber(2) * t * t,
                ScalarProcessor.ScalarFromNumber(3) * t * t
            )
        );

        // At t=0.5: (0.25, 0.5, 0.75)
        var valueFloat64 = pathFloat64.GetValue(0.5);
        var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(0.5));

        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void ComputedPath3D_GetDerivative1Value_WithProvidedFunction_ShouldMatchFloat64()
    {
        var timeRange = Float64ScalarRange.Create(0, 1);
        var timeRangeGeneric = ScalarRange<double>.Create(
            ScalarProcessor.Zero,
            ScalarProcessor.One
        );

        // Path: (t², 2t², 3t²), Derivative: (2t, 4t, 6t)
        var pathFloat64 = Float64ComputedPath3D.Finite(
            timeRange,
            t => LinFloat64Vector3D.Create(t * t, 2 * t * t, 3 * t * t),
            t => LinFloat64Vector3D.Create(2 * t, 4 * t, 6 * t)
        );

        var pathGeneric = ComputedPath3D<double>.Finite(
            timeRangeGeneric,
            t => LinVector3D<double>.Create(
                t * t,
                ScalarProcessor.ScalarFromNumber(2) * t * t,
                ScalarProcessor.ScalarFromNumber(3) * t * t
            ),
            t => LinVector3D<double>.Create(
                ScalarProcessor.ScalarFromNumber(2) * t,
                ScalarProcessor.ScalarFromNumber(4) * t,
                ScalarProcessor.ScalarFromNumber(6) * t
            )
        );

        // At t=0.5: derivative = (1, 2, 3)
        var deriv1Float64 = pathFloat64.GetDerivative1Value(0.5);
        var deriv1Generic = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(0.5));

        Assert.That(deriv1Generic.X.ScalarValue, Is.EqualTo(deriv1Float64.X.ScalarValue).Within(Tolerance));
        Assert.That(deriv1Generic.Y.ScalarValue, Is.EqualTo(deriv1Float64.Y.ScalarValue).Within(Tolerance));
        Assert.That(deriv1Generic.Z.ScalarValue, Is.EqualTo(deriv1Float64.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void ComputedPath3D_GetDerivative1Value_WithoutFunction_ShouldThrowNotImplementedException()
    {
        var timeRangeGeneric = ScalarRange<double>.Create(
            ScalarProcessor.Zero,
            ScalarProcessor.One
        );

        // Path: (t², 2t², 3t²) without explicit derivative
        var pathGeneric = ComputedPath3D<double>.Finite(
            timeRangeGeneric,
            t => LinVector3D<double>.Create(
                t * t,
                ScalarProcessor.ScalarFromNumber(2) * t * t,
                ScalarProcessor.ScalarFromNumber(3) * t * t
            )
        );

        // Numerical differentiation is not available for Generic<T>
        Assert.Throws<NotImplementedException>(() =>
        {
            var deriv1Generic = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(0.5));
        });
    }

    [Test]
    public void ComputedPath3D_GetDerivative2Value_WithProvidedFunction_ShouldMatchFloat64()
    {
        var timeRange = Float64ScalarRange.Create(0, 1);
        var timeRangeGeneric = ScalarRange<double>.Create(
            ScalarProcessor.Zero,
            ScalarProcessor.One
        );

        // Path: (t², 2t², 3t²), 1st Deriv: (2t, 4t, 6t), 2nd Deriv: (2, 4, 6)
        var pathFloat64 = Float64ComputedPath3D.Finite(
            timeRange,
            t => LinFloat64Vector3D.Create(t * t, 2 * t * t, 3 * t * t),
            t => LinFloat64Vector3D.Create(2 * t, 4 * t, 6 * t),
            t => LinFloat64Vector3D.Create(2, 4, 6)
        );

        var pathGeneric = ComputedPath3D<double>.Finite(
            timeRangeGeneric,
            t => LinVector3D<double>.Create(
                t * t,
                ScalarProcessor.ScalarFromNumber(2) * t * t,
                ScalarProcessor.ScalarFromNumber(3) * t * t
            ),
            t => LinVector3D<double>.Create(
                ScalarProcessor.ScalarFromNumber(2) * t,
                ScalarProcessor.ScalarFromNumber(4) * t,
                ScalarProcessor.ScalarFromNumber(6) * t
            ),
            t => LinVector3D<double>.Create(
                ScalarProcessor.ScalarFromNumber(2),
                ScalarProcessor.ScalarFromNumber(4),
                ScalarProcessor.ScalarFromNumber(6)
            )
        );

        // Second derivative is constant: (2, 4, 6)
        var deriv2Float64 = pathFloat64.GetDerivative2Value(0.5);
        var deriv2Generic = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(0.5));

        Assert.That(deriv2Generic.X.ScalarValue, Is.EqualTo(deriv2Float64.X.ScalarValue).Within(Tolerance));
        Assert.That(deriv2Generic.Y.ScalarValue, Is.EqualTo(deriv2Float64.Y.ScalarValue).Within(Tolerance));
        Assert.That(deriv2Generic.Z.ScalarValue, Is.EqualTo(deriv2Float64.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void ComputedPath3D_Periodic_Constructor_ShouldMatchFloat64()
    {
        var timeRange = Float64ScalarRange.Create(0, 1);
        var timeRangeGeneric = ScalarRange<double>.Create(
            ScalarProcessor.Zero,
            ScalarProcessor.One
        );

        var pathFloat64 = Float64ComputedPath3D.Periodic(
            timeRange,
            t => LinFloat64Vector3D.Create(t * t, 2 * t * t, 3 * t * t)
        );

        var pathGeneric = ComputedPath3D<double>.Periodic(
            timeRangeGeneric,
            t => LinVector3D<double>.Create(
                t * t,
                ScalarProcessor.ScalarFromNumber(2) * t * t,
                ScalarProcessor.ScalarFromNumber(3) * t * t
            )
        );

        Assert.That(pathGeneric.IsPeriodic, Is.EqualTo(pathFloat64.IsPeriodic));
        Assert.That(pathGeneric.IsFinite, Is.EqualTo(pathFloat64.IsFinite));
    }

    [Test]
    public void ComputedPath3D_ToFinitePath_ShouldPreserveGeometry()
    {
        var timeRangeGeneric = ScalarRange<double>.Create(
            ScalarProcessor.Zero,
            ScalarProcessor.One
        );

        var pathPeriodic = ComputedPath3D<double>.Periodic(
            timeRangeGeneric,
            t => LinVector3D<double>.Create(t * t, t * t, t * t)
        );

        var pathFinite = pathPeriodic.ToFinitePath();

        Assert.That(pathFinite.IsFinite, Is.True);

        // Geometry should be preserved
        var t = ScalarProcessor.Scalar(0.5);
        var valuePeriodic = pathPeriodic.GetValue(t);
        var valueFinite = pathFinite.GetValue(t);

        Assert.That(valueFinite.X.ScalarValue, Is.EqualTo(valuePeriodic.X.ScalarValue).Within(Tolerance));
        Assert.That(valueFinite.Y.ScalarValue, Is.EqualTo(valuePeriodic.Y.ScalarValue).Within(Tolerance));
        Assert.That(valueFinite.Z.ScalarValue, Is.EqualTo(valuePeriodic.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void ComputedPath3D_ToPeriodicPath_ShouldPreserveGeometry()
    {
        var timeRangeGeneric = ScalarRange<double>.Create(
            ScalarProcessor.Zero,
            ScalarProcessor.One
        );

        var pathFinite = ComputedPath3D<double>.Finite(
            timeRangeGeneric,
            t => LinVector3D<double>.Create(t * t, t * t, t * t)
        );

        var pathPeriodic = pathFinite.ToPeriodicPath();

        Assert.That(pathPeriodic.IsPeriodic, Is.True);

        // Geometry should be preserved
        var t = ScalarProcessor.Scalar(0.5);
        var valueFinite = pathFinite.GetValue(t);
        var valuePeriodic = pathPeriodic.GetValue(t);

        Assert.That(valuePeriodic.X.ScalarValue, Is.EqualTo(valueFinite.X.ScalarValue).Within(Tolerance));
        Assert.That(valuePeriodic.Y.ScalarValue, Is.EqualTo(valueFinite.Y.ScalarValue).Within(Tolerance));
        Assert.That(valuePeriodic.Z.ScalarValue, Is.EqualTo(valueFinite.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void ComputedPath3D_IsValid_ShouldAlwaysReturnTrue()
    {
        var timeRangeGeneric = ScalarRange<double>.Create(
            ScalarProcessor.Zero,
            ScalarProcessor.One
        );

        var pathGeneric = ComputedPath3D<double>.Finite(
            timeRangeGeneric,
            t => LinVector3D<double>.Create(t, t, t)
        );

        Assert.That(pathGeneric.IsValid(), Is.True);
    }

    [Test]
    public void ComputedPath3D_CreateWithXYZFunctions_ShouldMatchFloat64()
    {
        var timeRange = Float64ScalarRange.Create(0, 1);
        var timeRangeGeneric = ScalarRange<double>.Create(
            ScalarProcessor.Zero,
            ScalarProcessor.One
        );

        // Create path with separate X, Y, Z functions
        var pathFloat64 = Float64ComputedPath3D.Create(
            timeRange,
            false,
            t => t * t,       // x(t) = t²
            t => 2 * t,       // y(t) = 2t
            t => 3.0          // z(t) = 3
        );

        var pathGeneric = ComputedPath3D<double>.Create(
            timeRangeGeneric,
            false,
            t => t.ScalarValue * t.ScalarValue,
            t => 2.0 * t.ScalarValue,
            t => 3.0
        );

        // Test at t=0.5
        var valueFloat64 = pathFloat64.GetValue(0.5);
        var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(0.5));

        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void ComputedPath3D_SymmetricOneRange_ShouldMatchFloat64()
    {
        var pathFloat64 = Float64ComputedPath3D.Finite(
            t => LinFloat64Vector3D.Create(t, t * t, t * t * t)
        );

        var pathGeneric = ComputedPath3D<double>.Finite(
            ScalarProcessor,
            t => LinVector3D<double>.Create(t, t * t, t * t * t)
        );

        Assert.That(pathGeneric.TimeRange.MinValue.ScalarValue, Is.EqualTo(pathFloat64.TimeRange.MinValue).Within(Tolerance));
        Assert.That(pathGeneric.TimeRange.MaxValue.ScalarValue, Is.EqualTo(pathFloat64.TimeRange.MaxValue).Within(Tolerance));

        // Test at t=0.5
        var valueFloat64 = pathFloat64.GetValue(0.5);
        var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(0.5));

        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z.ScalarValue).Within(Tolerance));
    }
}
