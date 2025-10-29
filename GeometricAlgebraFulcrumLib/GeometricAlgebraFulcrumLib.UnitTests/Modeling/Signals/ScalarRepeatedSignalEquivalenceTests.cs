using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Signals;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Mapped;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public class ScalarRepeatedSignalEquivalenceTests
{
    private const double Tolerance = 1e-13;

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor =
        ScalarProcessorOfFloat64.Instance;


    [Test]
    public void Test_RepeatedSinSignal_Count2_GetValue()
    {
        // Arrange
        var float64Signal = Float64ScalarRepeatedSignal.Create(
            Float64ScalarSinSignal.FiniteInstance,
            2
        );
        var genericSignal = ScalarRepeatedSignal<double>.Create(
            SinScalarSignal<double>.Finite(ScalarProcessor),
            2
        );

        // Act & Assert - Test over two repetitions
        for (var t = -Math.PI; t <= Math.PI; t += Math.PI / 4)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;

            Debug.Assert(
                Math.Abs(float64Value - genericValue) < Tolerance,
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}"
            );

            Assert.That(
                Math.Abs(float64Value - genericValue),
                Is.LessThan(Tolerance),
                $"GetValue mismatch at t={t}"
            );
        }
    }

    [Test]
    public void Test_RepeatedCosSignal_Count3_GetValue()
    {
        // Arrange
        var float64Signal = Float64ScalarRepeatedSignal.Create(
            Float64ScalarCosSignal.FiniteInstance,
            3
        );
        var genericSignal = ScalarRepeatedSignal<double>.Create(
            CosScalarSignal<double>.Finite(ScalarProcessor),
            3
        );

        // Act & Assert
        for (var t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;

            Debug.Assert(
                Math.Abs(float64Value - genericValue) < Tolerance,
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}"
            );

            Assert.That(
                Math.Abs(float64Value - genericValue),
                Is.LessThan(Tolerance),
                $"GetValue mismatch at t={t}"
            );
        }
    }

    [Test]
    public void Test_RepeatedCosSignal_Count4_Derivatives()
    {
        // Arrange
        var float64Signal = Float64ScalarRepeatedSignal.Create(
            Float64ScalarCosSignal.FiniteInstance,
            4
        );
        var genericSignal = ScalarRepeatedSignal<double>.Create(
            CosScalarSignal<double>.Finite(ScalarProcessor),
            4
        );

        // Act & Assert
        for (var t = -Math.PI; t <= Math.PI; t += Math.PI / 3)
        {
            // First derivative
            var float64Der1 = float64Signal.GetDerivative1Value(t);
            var genericDer1 = genericSignal.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;

            Debug.Assert(
                Math.Abs(float64Der1 - genericDer1) < Tolerance,
                $"Derivative1 mismatch at t={t}: Float64={float64Der1}, Generic={genericDer1}"
            );

            Assert.That(
                Math.Abs(float64Der1 - genericDer1),
                Is.LessThan(Tolerance),
                $"GetDerivative1Value mismatch at t={t}"
            );

            // Second derivative
            var float64Der2 = float64Signal.GetDerivative2Value(t);
            var genericDer2 = genericSignal.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;

            Debug.Assert(
                Math.Abs(float64Der2 - genericDer2) < Tolerance,
                $"Derivative2 mismatch at t={t}: Float64={float64Der2}, Generic={genericDer2}"
            );

            Assert.That(
                Math.Abs(float64Der2 - genericDer2),
                Is.LessThan(Tolerance),
                $"GetDerivative2Value mismatch at t={t}"
            );
        }
    }

    [Test]
    public void Test_RepeatedSignal_TimeRangeExtension()
    {
        // Arrange - Base signal has time range [-π, π], repeated 3 times gives range [-3π, 3π]
        var float64Signal = Float64ScalarRepeatedSignal.Create(
            Float64ScalarSinSignal.FiniteInstance,
            3
        );
        var genericSignal = ScalarRepeatedSignal<double>.Create(
            SinScalarSignal<double>.Finite(ScalarProcessor),
            3
        );

        // Act & Assert
        Assert.That(
            Math.Abs(float64Signal.MinTime - genericSignal.MinTime.ScalarValue),
            Is.LessThan(Tolerance),
            "MinTime mismatch"
        );

        Assert.That(
            Math.Abs(float64Signal.MaxTime - genericSignal.MaxTime.ScalarValue),
            Is.LessThan(Tolerance),
            "MaxTime mismatch"
        );

        Assert.That(
            Math.Abs(float64Signal.TimeRangeLength - genericSignal.TimeRangeLength.ScalarValue),
            Is.LessThan(Tolerance),
            "TimeRangeLength mismatch"
        );
    }

    [Test]
    public void Test_RepeatedSignal_IsFinite()
    {
        // Arrange
        var float64Signal = Float64ScalarRepeatedSignal.Create(
            Float64ScalarSinSignal.FiniteInstance,
            3
        );
        var genericSignal = ScalarRepeatedSignal<double>.Create(
            SinScalarSignal<double>.Finite(ScalarProcessor),
            3
        );

        // Act & Assert - Repeated signal is always finite
        Debug.Assert(
            float64Signal.IsFinite,
            "Float64 signal should be finite"
        );

        Assert.That(float64Signal.IsFinite, Is.True, "Float64 signal should be finite");
        Assert.That(genericSignal.IsFinite, Is.True, "Generic signal should be finite");
        Assert.That(float64Signal.IsFinite, Is.EqualTo(genericSignal.IsFinite), "IsFinite mismatch");
    }

    [Test]
    public void Test_RepeatedSignal_IsNotPeriodic()
    {
        // Arrange
        var float64Signal = Float64ScalarRepeatedSignal.Create(
            Float64ScalarSinSignal.FiniteInstance,
            2
        );
        var genericSignal = ScalarRepeatedSignal<double>.Create(
            SinScalarSignal<double>.Finite(ScalarProcessor),
            2
        );

        // Act & Assert - Repeated signal is NOT periodic (it has a defined end)
        Debug.Assert(
            !float64Signal.IsPeriodic,
            "Float64 signal should NOT be periodic"
        );

        Assert.That(float64Signal.IsPeriodic, Is.False, "Float64 signal should NOT be periodic");
        Assert.That(genericSignal.IsPeriodic, Is.False, "Generic signal should NOT be periodic");
        Assert.That(float64Signal.IsPeriodic, Is.EqualTo(genericSignal.IsPeriodic), "IsPeriodic mismatch");
    }

    [Test]
    public void Test_RepeatedSignal_ToFiniteSignal_ReturnsItself()
    {
        // Arrange
        var float64Signal = Float64ScalarRepeatedSignal.Create(
            Float64ScalarSinSignal.FiniteInstance,
            2
        );
        var genericSignal = ScalarRepeatedSignal<double>.Create(
            SinScalarSignal<double>.Finite(ScalarProcessor),
            2
        );

        // Act
        var float64Finite = float64Signal.ToFiniteSignal();
        var genericFinite = genericSignal.ToFiniteSignal();

        // Assert - ToFiniteSignal() should return the same instance (already finite)
        Debug.Assert(
            ReferenceEquals(float64Signal, float64Finite),
            "Float64 ToFiniteSignal should return itself"
        );

        Assert.That(
            ReferenceEquals(float64Signal, float64Finite),
            Is.True,
            "Float64 ToFiniteSignal should return itself"
        );
        Assert.That(
            ReferenceEquals(genericSignal, genericFinite),
            Is.True,
            "Generic ToFiniteSignal should return itself"
        );
    }

    [Test]
    public void Test_RepeatedSignal_ToPeriodicSignal_ReturnsBaseSignalPeriodic()
    {
        // Arrange
        var float64BaseSignal = Float64ScalarSinSignal.FiniteInstance;
        var float64Signal = Float64ScalarRepeatedSignal.Create(float64BaseSignal, 3);

        var genericBaseSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericSignal = ScalarRepeatedSignal<double>.Create(genericBaseSignal, 3);

        // Act
        var float64Periodic = float64Signal.ToPeriodicSignal();
        var genericPeriodic = genericSignal.ToPeriodicSignal();

        // Assert - ToPeriodicSignal() should return the base signal's periodic version
        Debug.Assert(
            float64Periodic.IsPeriodic,
            "Float64 ToPeriodicSignal should be periodic"
        );

        Assert.That(float64Periodic.IsPeriodic, Is.True, "Float64 ToPeriodicSignal should be periodic");
        Assert.That(genericPeriodic.IsPeriodic, Is.True, "Generic ToPeriodicSignal should be periodic");

        // Verify that the periodic signal matches the base signal's periodic version
        var t = Math.PI / 2;
        var float64Value = float64Periodic.GetValue(t);
        var genericValue = genericPeriodic.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;

        Assert.That(
            Math.Abs(float64Value - genericValue),
            Is.LessThan(Tolerance),
            "ToPeriodicSignal values should match"
        );
    }

    [Test]
    public void Test_RepeatedSignal_Count_Property()
    {
        // Arrange
        const int count = 7;
        var float64Signal = Float64ScalarRepeatedSignal.Create(
            Float64ScalarSinSignal.FiniteInstance,
            count
        );
        var genericSignal = ScalarRepeatedSignal<double>.Create(
            SinScalarSignal<double>.Finite(ScalarProcessor),
            count
        );

        // Act & Assert
        Debug.Assert(
            float64Signal.Count == count,
            $"Float64 Count={float64Signal.Count}, expected {count}"
        );

        Assert.That(float64Signal.Count, Is.EqualTo(count), "Float64 Count mismatch");
        Assert.That(genericSignal.Count, Is.EqualTo(count), "Generic Count mismatch");
        Assert.That(float64Signal.Count, Is.EqualTo(genericSignal.Count), "Count property mismatch");
    }

    [Test]
    public void Test_RepeatedSignal_BaseSignal_Property()
    {
        // Arrange
        var float64BaseSignal = Float64ScalarSinSignal.FiniteInstance;
        var float64Signal = Float64ScalarRepeatedSignal.Create(float64BaseSignal, 2);

        var genericBaseSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericSignal = ScalarRepeatedSignal<double>.Create(genericBaseSignal, 2);

        // Act & Assert - Verify BaseSignal property is accessible
        Debug.Assert(
            ReferenceEquals(float64Signal.BaseSignal, float64BaseSignal),
            "Float64 BaseSignal should reference the original base signal"
        );

        Assert.That(
            ReferenceEquals(float64Signal.BaseSignal, float64BaseSignal),
            Is.True,
            "Float64 BaseSignal should reference the original base signal"
        );
        Assert.That(
            ReferenceEquals(genericSignal.BaseSignal, genericBaseSignal),
            Is.True,
            "Generic BaseSignal should reference the original base signal"
        );

        // Verify base signal values match
        var t = 0.5;
        var float64BaseValue = float64Signal.BaseSignal.GetValue(t);
        var genericBaseValue = genericSignal.BaseSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;

        Assert.That(
            Math.Abs(float64BaseValue - genericBaseValue),
            Is.LessThan(Tolerance),
            "BaseSignal values should match"
        );
    }
}
