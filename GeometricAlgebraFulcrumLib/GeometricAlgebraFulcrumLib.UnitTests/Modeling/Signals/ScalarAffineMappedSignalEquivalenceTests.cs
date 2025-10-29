using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.AffineMaps.Space1D;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Mapped;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Mapped;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public class ScalarAffineMappedSignalEquivalenceTests
{
    private const double Tolerance = 1e-13;

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor =
        ScalarProcessorOfFloat64.Instance;


    [Test]
    public void Test_AffineMappedSignal_ScaleOnly_GetValue()
    {
        // Arrange - Scale signal values by 2.0
        var float64BaseSignal = Float64ScalarSinSignal.FiniteInstance;
        var float64AffineMap = Float64AffineMap1D.CreateScale(2.0);
        var float64Signal = Float64ScalarAffineMappedSignal.Create(float64BaseSignal, float64AffineMap);

        var genericBaseSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericAffineMap = AffineMap1D<double>.CreateScale(ScalarProcessor, ScalarProcessor.ScalarFromNumber(2.0));
        var genericSignal = ScalarAffineMappedSignal<double>.Create(genericBaseSignal, genericAffineMap);

        // Act & Assert
        for (var t = -Math.PI; t <= Math.PI; t += Math.PI / 8)
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

            // Verify it's actually 2*sin(t)
            var expected = 2.0 * Math.Sin(t);
            Assert.That(Math.Abs(float64Value - expected), Is.LessThan(Tolerance));
        }
    }

    [Test]
    public void Test_AffineMappedSignal_TranslateOnly_GetValue()
    {
        // Arrange - Translate signal values by +3.0
        var float64BaseSignal = Float64ScalarCosSignal.FiniteInstance;
        var float64AffineMap = Float64AffineMap1D.CreateTranslate(3.0);
        var float64Signal = Float64ScalarAffineMappedSignal.Create(float64BaseSignal, float64AffineMap);

        var genericBaseSignal = CosScalarSignal<double>.Finite(ScalarProcessor);
        var genericAffineMap = AffineMap1D<double>.CreateTranslate(ScalarProcessor, ScalarProcessor.ScalarFromNumber(3.0));
        var genericSignal = ScalarAffineMappedSignal<double>.Create(genericBaseSignal, genericAffineMap);

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

            // Verify it's actually cos(t) + 3
            var expected = Math.Cos(t) + 3.0;
            Assert.That(Math.Abs(float64Value - expected), Is.LessThan(Tolerance));
        }
    }

    [Test]
    public void Test_AffineMappedSignal_ScaleAndTranslate_GetValue()
    {
        // Arrange - Affine map: f(x) = 2.5*x + 1.0
        var float64BaseSignal = Float64ScalarSinSignal.FiniteInstance;
        var float64AffineMap = Float64AffineMap1D.Create(2.5, 1.0);
        var float64Signal = Float64ScalarAffineMappedSignal.Create(float64BaseSignal, float64AffineMap);

        var genericBaseSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericAffineMap = AffineMap1D<double>.Create(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(2.5),
            ScalarProcessor.ScalarFromNumber(1.0)
        );
        var genericSignal = ScalarAffineMappedSignal<double>.Create(genericBaseSignal, genericAffineMap);

        // Act & Assert
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

            // Verify it's actually 2.5*sin(t) + 1.0
            var expected = 2.5 * Math.Sin(t) + 1.0;
            Assert.That(Math.Abs(float64Value - expected), Is.LessThan(Tolerance));
        }
    }

    [Test]
    public void Test_AffineMappedSignal_Identity_GetValue()
    {
        // Arrange - Identity map: f(x) = x (scaling=1, offset=0)
        var float64BaseSignal = Float64ScalarSinSignal.FiniteInstance;
        var float64AffineMap = Float64AffineMap1D.Identity;
        var float64Signal = Float64ScalarAffineMappedSignal.Create(float64BaseSignal, float64AffineMap);

        var genericBaseSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericAffineMap = AffineMap1D<double>.Identity(ScalarProcessor);
        var genericSignal = ScalarAffineMappedSignal<double>.Create(genericBaseSignal, genericAffineMap);

        // Act & Assert
        for (var t = -Math.PI; t <= Math.PI; t += Math.PI / 6)
        {
            var float64Value = float64Signal.GetValue(t);
            var genericValue = genericSignal.GetValue(ScalarProcessor.ScalarFromNumber(t)).ScalarValue;
            var baseValue = float64BaseSignal.GetValue(t);

            Debug.Assert(
                Math.Abs(float64Value - genericValue) < Tolerance,
                $"Mismatch at t={t}: Float64={float64Value}, Generic={genericValue}"
            );

            Assert.That(
                Math.Abs(float64Value - genericValue),
                Is.LessThan(Tolerance),
                $"GetValue mismatch at t={t}"
            );

            // Identity map should not change the value
            Assert.That(Math.Abs(float64Value - baseValue), Is.LessThan(Tolerance));
        }
    }

    [Test]
    public void Test_AffineMappedSignal_Reflection_GetValue()
    {
        // Arrange - Reflection: f(x) = -x (scaling=-1, offset=0)
        var float64BaseSignal = Float64ScalarSinSignal.FiniteInstance;
        var float64AffineMap = Float64AffineMap1D.Reflection;
        var float64Signal = Float64ScalarAffineMappedSignal.Create(float64BaseSignal, float64AffineMap);

        var genericBaseSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericAffineMap = AffineMap1D<double>.Reflection(ScalarProcessor);
        var genericSignal = ScalarAffineMappedSignal<double>.Create(genericBaseSignal, genericAffineMap);

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

            // Verify it's actually -sin(t)
            var expected = -Math.Sin(t);
            Assert.That(Math.Abs(float64Value - expected), Is.LessThan(Tolerance));
        }
    }

    [Test]
    public void Test_AffineMappedSignal_Derivative1()
    {
        // Arrange - Affine map: f(x) = 3*x + 2
        // d/dt[3*sin(t) + 2] = 3*cos(t)
        var float64BaseSignal = Float64ScalarSinSignal.FiniteInstance;
        var float64AffineMap = Float64AffineMap1D.Create(3.0, 2.0);
        var float64Signal = Float64ScalarAffineMappedSignal.Create(float64BaseSignal, float64AffineMap);

        var genericBaseSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericAffineMap = AffineMap1D<double>.Create(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(3.0),
            ScalarProcessor.ScalarFromNumber(2.0)
        );
        var genericSignal = ScalarAffineMappedSignal<double>.Create(genericBaseSignal, genericAffineMap);

        // Act & Assert
        for (var t = -Math.PI; t <= Math.PI; t += Math.PI / 4)
        {
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

            // Verify it's actually 3*cos(t)
            var expected = 3.0 * Math.Cos(t);
            Assert.That(Math.Abs(float64Der1 - expected), Is.LessThan(Tolerance));
        }
    }

    [Test]
    public void Test_AffineMappedSignal_Derivative2()
    {
        // Arrange - Affine map: f(x) = 2.5*x + 1.5
        // d²/dt²[2.5*sin(t) + 1.5] = -2.5*sin(t)
        var float64BaseSignal = Float64ScalarSinSignal.FiniteInstance;
        var float64AffineMap = Float64AffineMap1D.Create(2.5, 1.5);
        var float64Signal = Float64ScalarAffineMappedSignal.Create(float64BaseSignal, float64AffineMap);

        var genericBaseSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericAffineMap = AffineMap1D<double>.Create(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(2.5),
            ScalarProcessor.ScalarFromNumber(1.5)
        );
        var genericSignal = ScalarAffineMappedSignal<double>.Create(genericBaseSignal, genericAffineMap);

        // Act & Assert
        for (var t = -Math.PI; t <= Math.PI; t += Math.PI / 3)
        {
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

            // Verify it's actually -2.5*sin(t)
            var expected = -2.5 * Math.Sin(t);
            Assert.That(Math.Abs(float64Der2 - expected), Is.LessThan(Tolerance));
        }
    }

    [Test]
    public void Test_AffineMappedSignal_IsFinite()
    {
        // Arrange
        var float64BaseSignal = Float64ScalarSinSignal.FiniteInstance;
        var float64AffineMap = Float64AffineMap1D.Create(2.0, 1.0);
        var float64Signal = Float64ScalarAffineMappedSignal.Create(float64BaseSignal, float64AffineMap);

        var genericBaseSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericAffineMap = AffineMap1D<double>.Create(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(2.0),
            ScalarProcessor.ScalarFromNumber(1.0)
        );
        var genericSignal = ScalarAffineMappedSignal<double>.Create(genericBaseSignal, genericAffineMap);

        // Act & Assert
        Debug.Assert(
            float64Signal.IsFinite,
            "Float64 signal should be finite"
        );

        Assert.That(float64Signal.IsFinite, Is.True, "Float64 signal should be finite");
        Assert.That(genericSignal.IsFinite, Is.True, "Generic signal should be finite");
        Assert.That(float64Signal.IsFinite, Is.EqualTo(genericSignal.IsFinite), "IsFinite mismatch");
    }

    [Test]
    public void Test_AffineMappedSignal_TimeRangePreserved()
    {
        // Arrange - Affine mapping preserves time range of base signal
        var float64BaseSignal = Float64ScalarSinSignal.FiniteInstance;
        var float64AffineMap = Float64AffineMap1D.Create(2.0, 1.0);
        var float64Signal = Float64ScalarAffineMappedSignal.Create(float64BaseSignal, float64AffineMap);

        var genericBaseSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericAffineMap = AffineMap1D<double>.Create(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(2.0),
            ScalarProcessor.ScalarFromNumber(1.0)
        );
        var genericSignal = ScalarAffineMappedSignal<double>.Create(genericBaseSignal, genericAffineMap);

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

        // Time range should match base signal
        Assert.That(
            Math.Abs(float64Signal.MinTime - float64BaseSignal.MinTime),
            Is.LessThan(Tolerance),
            "MinTime should match base signal"
        );

        Assert.That(
            Math.Abs(float64Signal.MaxTime - float64BaseSignal.MaxTime),
            Is.LessThan(Tolerance),
            "MaxTime should match base signal"
        );
    }

    [Test]
    public void Test_AffineMappedSignal_ToFiniteSignal_ReturnsItself()
    {
        // Arrange
        var float64BaseSignal = Float64ScalarSinSignal.FiniteInstance;
        var float64AffineMap = Float64AffineMap1D.Create(2.0, 1.0);
        var float64Signal = Float64ScalarAffineMappedSignal.Create(float64BaseSignal, float64AffineMap);

        var genericBaseSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericAffineMap = AffineMap1D<double>.Create(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(2.0),
            ScalarProcessor.ScalarFromNumber(1.0)
        );
        var genericSignal = ScalarAffineMappedSignal<double>.Create(genericBaseSignal, genericAffineMap);

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
    public void Test_AffineMappedSignal_BaseSignalProperty()
    {
        // Arrange
        var float64BaseSignal = Float64ScalarSinSignal.FiniteInstance;
        var float64AffineMap = Float64AffineMap1D.Create(2.0, 1.0);
        var float64Signal = Float64ScalarAffineMappedSignal.Create(float64BaseSignal, float64AffineMap);

        var genericBaseSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericAffineMap = AffineMap1D<double>.Create(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(2.0),
            ScalarProcessor.ScalarFromNumber(1.0)
        );
        var genericSignal = ScalarAffineMappedSignal<double>.Create(genericBaseSignal, genericAffineMap);

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

    [Test]
    public void Test_AffineMappedSignal_AffineMapProperty()
    {
        // Arrange
        var float64BaseSignal = Float64ScalarSinSignal.FiniteInstance;
        var float64AffineMap = Float64AffineMap1D.Create(3.5, 2.5);
        var float64Signal = Float64ScalarAffineMappedSignal.Create(float64BaseSignal, float64AffineMap);

        var genericBaseSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericAffineMap = AffineMap1D<double>.Create(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(3.5),
            ScalarProcessor.ScalarFromNumber(2.5)
        );
        var genericSignal = ScalarAffineMappedSignal<double>.Create(genericBaseSignal, genericAffineMap);

        // Act & Assert - Verify AffineMap properties
        Assert.That(
            Math.Abs(float64Signal.AffineMap.Scaling.ScalarValue - genericSignal.AffineMap.Scaling.ScalarValue),
            Is.LessThan(Tolerance),
            "AffineMap Scaling should match"
        );

        Assert.That(
            Math.Abs(float64Signal.AffineMap.Offset.ScalarValue - genericSignal.AffineMap.Offset.ScalarValue),
            Is.LessThan(Tolerance),
            "AffineMap Offset should match"
        );
    }

    [Test]
    public void Test_AffineMappedSignal_IsValid()
    {
        // Arrange
        var genericBaseSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        var genericAffineMap = AffineMap1D<double>.Create(
            ScalarProcessor,
            ScalarProcessor.ScalarFromNumber(2.0),
            ScalarProcessor.ScalarFromNumber(1.0)
        );
        var signal = ScalarAffineMappedSignal<double>.Create(genericBaseSignal, genericAffineMap);

        // Act & Assert
        Assert.That(signal.IsValid(), Is.True);
    }
}
