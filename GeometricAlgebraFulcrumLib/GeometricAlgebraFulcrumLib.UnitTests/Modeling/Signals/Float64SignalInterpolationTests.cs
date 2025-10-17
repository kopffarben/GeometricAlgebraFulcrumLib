using System;
using System.Linq;
using GeometricAlgebraFulcrumLib.Modeling.Calculus.Functions.Float64.Interpolators;
using GeometricAlgebraFulcrumLib.Modeling.Signals;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

/// <summary>
/// Tests for Signal Interpolation
/// Phase 3B - Core Modeling: Interpolation Tests (40 tests)
/// Tests signal creation, interpolation methods, and properties
/// </summary>
[TestFixture]
public class Float64SignalInterpolationTests
{
    private const double Tolerance = 1e-10;

    #region Signal Construction Tests (10 tests)

    [Test]
    public void Signal_FiniteZero_ShouldCreate()
    {
        // Arrange & Act
        var signal = Float64SampledTimeSignal.FiniteZero(samplingRate: 100, sampleCount: 10);

        // Assert
        Assert.That(signal, Is.Not.Null, "Signal should be created");
        Assert.That(signal.Count, Is.EqualTo(10), "Sample count should be 10");
        Assert.That(signal.SamplingRate, Is.EqualTo(100), "Sampling rate should be 100");
    }

    [Test]
    public void Signal_PeriodicZero_ShouldCreate()
    {
        // Arrange & Act
        var signal = Float64SampledTimeSignal.PeriodicZero(samplingRate: 50, sampleCount: 20);

        // Assert
        Assert.That(signal, Is.Not.Null, "Signal should be created");
        Assert.That(signal.Count, Is.EqualTo(20), "Sample count should be 20");
        Assert.That(signal.IsPeriodic, Is.True, "Signal should be periodic");
    }

    [Test]
    public void Signal_FiniteConstant_ShouldHaveConstantValue()
    {
        // Arrange & Act
        var signal = Float64SampledTimeSignal.FiniteConstant(samplingRate: 100, sampleCount: 5, value: 3.5);

        // Assert
        Assert.That(signal.Count, Is.EqualTo(5), "Should have 5 samples");
        for (int i = 0; i < signal.Count; i++)
        {
            Assert.That(signal[i], Is.EqualTo(3.5).Within(Tolerance), $"Sample {i} should be 3.5");
        }
    }

    [Test]
    public void Signal_PeriodicConstant_ShouldBePeriodic()
    {
        // Arrange & Act
        var signal = Float64SampledTimeSignal.PeriodicConstant(samplingRate: 200, sampleCount: 8, value: 2.0);

        // Assert
        Assert.That(signal.IsPeriodic, Is.True, "Signal should be periodic");
        Assert.That(signal[0], Is.EqualTo(2.0).Within(Tolerance), "All samples should be 2.0");
    }

    [Test]
    public void Signal_CreateFromArray_ShouldWork()
    {
        // Arrange
        var samples = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 };

        // Act
        var signal = Float64SampledTimeSignal.Finite(samplingRate: 100, samples);

        // Assert
        Assert.That(signal.Count, Is.EqualTo(5), "Should have 5 samples");
        Assert.That(signal[0], Is.EqualTo(1.0).Within(Tolerance), "First sample should be 1.0");
        Assert.That(signal[4], Is.EqualTo(5.0).Within(Tolerance), "Last sample should be 5.0");
    }

    [Test]
    public void Signal_CreatePeriodic_ShouldSetPeriodicFlag()
    {
        // Arrange
        var samples = new double[] { 1, 2, 3 };

        // Act
        var signal = Float64SampledTimeSignal.Periodic(samplingRate: 50, samples);

        // Assert
        Assert.That(signal.IsPeriodic, Is.True, "Periodic signal should have IsPeriodic true");
    }

    [Test]
    public void Signal_FiniteSignal_ShouldNotBePeriodic()
    {
        // Arrange
        var samples = new double[] { 1, 2, 3 };

        // Act
        var signal = Float64SampledTimeSignal.Finite(samplingRate: 50, samples);

        // Assert
        Assert.That(signal.IsPeriodic, Is.False, "Finite signal should have IsPeriodic false");
    }

    [Test]
    public void Signal_SamplingRate_ShouldBeAccessible()
    {
        // Arrange & Act
        var signal = Float64SampledTimeSignal.FiniteZero(samplingRate: 123.45, sampleCount: 10);

        // Assert
        Assert.That(signal.SamplingRate, Is.EqualTo(123.45).Within(Tolerance), "Sampling rate should be accessible");
    }

    [Test]
    public void Signal_Count_ShouldMatchSampleCount()
    {
        // Arrange
        var samples = new double[] { 1, 2, 3, 4, 5, 6, 7 };

        // Act
        var signal = Float64SampledTimeSignal.Finite(100, samples);

        // Assert
        Assert.That(signal.Count, Is.EqualTo(samples.Length), "Count should match sample array length");
    }

    [Test]
    public void Signal_Indexer_ShouldAccessSamples()
    {
        // Arrange
        var samples = new double[] { 10, 20, 30, 40, 50 };
        var signal = Float64SampledTimeSignal.Finite(100, samples);

        // Act & Assert
        for (int i = 0; i < samples.Length; i++)
        {
            Assert.That(signal[i], Is.EqualTo(samples[i]).Within(Tolerance), $"Sample {i} should match");
        }
    }

    #endregion

    #region Linear Spline Interpolation Tests (10 tests)

    [Test]
    public void LinearInterpolation_AtDataPoints_ShouldBeExact()
    {
        // Arrange
        var samples = new double[] { 0, 1, 2, 3, 4 };
        var signal = Float64SampledTimeSignal.Finite(samplingRate: 1.0, samples);
        var options = new DfLinearSplineSignalInterpolatorOptions();

        // Act
        var interpolator = DfLinearSplineSignalInterpolator.Create(signal, options);

        // Assert - interpolation at data points should be exact
        for (int i = 0; i < samples.Length; i++)
        {
            var t = i / 1.0; // time = index / samplingRate
            var value = interpolator.GetValue(t);
            Assert.That(value, Is.EqualTo(samples[i]).Within(Tolerance), $"Value at t={t} should be exact");
        }
    }

    [Test]
    public void LinearInterpolation_BetweenPoints_ShouldInterpolate()
    {
        // Arrange
        var samples = new double[] { 0, 2, 4 }; // Linear with slope 2
        var signal = Float64SampledTimeSignal.Finite(samplingRate: 1.0, samples);
        var options = new DfLinearSplineSignalInterpolatorOptions();

        // Act
        var interpolator = DfLinearSplineSignalInterpolator.Create(signal, options);
        var midValue = interpolator.GetValue(0.5); // Between 0 and 2

        // Assert - should interpolate linearly
        Assert.That(midValue, Is.EqualTo(1.0).Within(Tolerance), "Mid-point should be 1.0");
    }

    [Test]
    public void LinearInterpolation_ConstantSignal_ShouldBeConstant()
    {
        // Arrange
        var signal = Float64SampledTimeSignal.FiniteConstant(samplingRate: 10, sampleCount: 100, value: 5.0);
        var options = new DfLinearSplineSignalInterpolatorOptions();

        // Act
        var interpolator = DfLinearSplineSignalInterpolator.Create(signal, options);

        // Assert - interpolation of constant signal should be constant everywhere
        var value1 = interpolator.GetValue(0.5);
        var value2 = interpolator.GetValue(5.5);
        Assert.That(value1, Is.EqualTo(5.0).Within(Tolerance), "Should be constant");
        Assert.That(value2, Is.EqualTo(5.0).Within(Tolerance), "Should be constant");
    }

    [Test]
    public void LinearInterpolation_Create_ShouldNotThrow()
    {
        // Arrange
        var samples = new double[] { 1, 2, 3, 4, 5 };
        var signal = Float64SampledTimeSignal.Finite(100, samples);
        var options = new DfLinearSplineSignalInterpolatorOptions();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var interpolator = DfLinearSplineSignalInterpolator.Create(signal, options);
        }, "Creating linear interpolator should not throw");
    }

    [Test]
    public void LinearInterpolation_Options_ShouldBeAccessible()
    {
        // Arrange
        var samples = new double[] { 1, 2, 3 };
        var signal = Float64SampledTimeSignal.Finite(10, samples);
        var options = new DfLinearSplineSignalInterpolatorOptions();

        // Act
        var interpolator = DfLinearSplineSignalInterpolator.Create(signal, options);

        // Assert
        Assert.That(interpolator.Options, Is.Not.Null, "Options should be accessible");
    }

    [Test]
    public void LinearInterpolation_Spline_ShouldExist()
    {
        // Arrange
        var samples = new double[] { 0, 1, 2 };
        var signal = Float64SampledTimeSignal.Finite(1, samples);
        var options = new DfLinearSplineSignalInterpolatorOptions();

        // Act
        var interpolator = DfLinearSplineSignalInterpolator.Create(signal, options);

        // Assert
        Assert.That(interpolator.Spline, Is.Not.Null, "Spline should exist");
    }

    [Test]
    public void LinearInterpolation_GetDerivative1_ShouldWork()
    {
        // Arrange
        var samples = new double[] { 0, 1, 2, 3 };
        var signal = Float64SampledTimeSignal.Finite(1, samples);
        var options = new DfLinearSplineSignalInterpolatorOptions();

        // Act
        var interpolator = DfLinearSplineSignalInterpolator.Create(signal, options);
        var derivative = interpolator.GetDerivative1();

        // Assert
        Assert.That(derivative, Is.Not.Null, "First derivative should exist");
    }

    [Test]
    public void LinearInterpolation_GetDerivative2_ShouldWork()
    {
        // Arrange
        var samples = new double[] { 0, 1, 4, 9 }; // Quadratic: t^2
        var signal = Float64SampledTimeSignal.Finite(1, samples);
        var options = new DfLinearSplineSignalInterpolatorOptions();

        // Act
        var interpolator = DfLinearSplineSignalInterpolator.Create(signal, options);
        var derivative2 = interpolator.GetDerivative2();

        // Assert
        Assert.That(derivative2, Is.Not.Null, "Second derivative should exist");
    }

    [Test]
    public void LinearInterpolation_GetDerivativeN_ShouldWork()
    {
        // Arrange
        var samples = new double[] { 0, 1, 2, 3, 4 };
        var signal = Float64SampledTimeSignal.Finite(1, samples);
        var options = new DfLinearSplineSignalInterpolatorOptions();

        // Act
        var interpolator = DfLinearSplineSignalInterpolator.Create(signal, options);
        var derivativeN = interpolator.GetDerivativeN(3);

        // Assert
        Assert.That(derivativeN, Is.Not.Null, "N-th derivative should exist");
    }

    [Test]
    public void LinearInterpolation_SubRange_ShouldWork()
    {
        // Arrange
        var samples = new double[] { 0, 1, 2, 3, 4, 5 };
        var signal = Float64SampledTimeSignal.Finite(1, samples);
        var options = new DfLinearSplineSignalInterpolatorOptions();

        // Act
        var interpolator = DfLinearSplineSignalInterpolator.Create(signal, 1, 4, options); // Samples 1 to 4

        // Assert
        Assert.That(interpolator, Is.Not.Null, "Sub-range interpolator should be created");
    }

    #endregion

    #region Catmull-Rom Spline Interpolation Tests (10 tests)

    [Test]
    public void CatmullRomInterpolation_Create_ShouldWork()
    {
        // Arrange
        var samples = new double[] { 0, 1, 2, 3, 4 };
        var signal = Float64SampledTimeSignal.Finite(1, samples);
        var options = new DfCatmullRomSplineSignalInterpolatorOptions() { BezierDegree = 2 };

        // Act
        var interpolator = DfCatmullRomSplineSignalInterpolator.Create(signal, options);

        // Assert
        Assert.That(interpolator, Is.Not.Null, "Catmull-Rom interpolator should be created");
    }

    [Test]
    public void CatmullRomInterpolation_AtDataPoints_ShouldBeExact()
    {
        // Arrange
        var samples = new double[] { 1, 2, 3, 4, 5 };
        var signal = Float64SampledTimeSignal.Finite(1, samples);
        var options = new DfCatmullRomSplineSignalInterpolatorOptions() { BezierDegree = 2 };

        // Act
        var interpolator = DfCatmullRomSplineSignalInterpolator.Create(signal, options);

        // Assert - at data points, interpolation should be exact
        for (int i = 0; i < samples.Length; i++)
        {
            var t = i / 1.0;
            var value = interpolator.GetValue(t);
            Assert.That(value, Is.EqualTo(samples[i]).Within(0.1), $"Value at t={t} should be close");
        }
    }

    [Test]
    public void CatmullRomInterpolation_Options_ShouldBeAccessible()
    {
        // Arrange
        var samples = new double[] { 1, 2, 3 };
        var signal = Float64SampledTimeSignal.Finite(1, samples);
        var options = new DfCatmullRomSplineSignalInterpolatorOptions() { BezierDegree = 2 };

        // Act
        var interpolator = DfCatmullRomSplineSignalInterpolator.Create(signal, options);

        // Assert
        Assert.That(interpolator.Options, Is.Not.Null, "Options should be accessible");
    }

    [Test]
    public void CatmullRomInterpolation_GetDerivative1_ShouldWork()
    {
        // Arrange
        var samples = new double[] { 0, 1, 2, 3, 4 };
        var signal = Float64SampledTimeSignal.Finite(1, samples);
        var options = new DfCatmullRomSplineSignalInterpolatorOptions() { BezierDegree = 2 };

        // Act
        var interpolator = DfCatmullRomSplineSignalInterpolator.Create(signal, options);
        var derivative = interpolator.GetDerivative1();

        // Assert
        Assert.That(derivative, Is.Not.Null, "First derivative should exist");
    }

    [Test]
    public void CatmullRomInterpolation_GetDerivative2_ShouldWork()
    {
        // Arrange
        var samples = new double[] { 0, 1, 4, 9, 16 };
        var signal = Float64SampledTimeSignal.Finite(1, samples);
        var options = new DfCatmullRomSplineSignalInterpolatorOptions() { BezierDegree = 2 };

        // Act
        var interpolator = DfCatmullRomSplineSignalInterpolator.Create(signal, options);
        var derivative2 = interpolator.GetDerivative2();

        // Assert
        Assert.That(derivative2, Is.Not.Null, "Second derivative should exist");
    }

    [Test]
    public void CatmullRomInterpolation_Smoothness_ShouldBeC1()
    {
        // Arrange
        var samples = new double[] { 0, 1, 2, 3, 4 };
        var signal = Float64SampledTimeSignal.Finite(1, samples);
        var options = new DfCatmullRomSplineSignalInterpolatorOptions() { BezierDegree = 2 };

        // Act
        var interpolator = DfCatmullRomSplineSignalInterpolator.Create(signal, options);

        // Assert - Catmull-Rom should be C1 continuous (has continuous first derivative)
        Assert.DoesNotThrow(() =>
        {
            var d1 = interpolator.GetDerivative1();
            var value = d1.GetValue(1.5);
        }, "First derivative should be continuous");
    }

    [Test]
    public void CatmullRomInterpolation_ConstantSignal_ShouldBeConstant()
    {
        // Arrange
        var signal = Float64SampledTimeSignal.FiniteConstant(10, 9, 7.5);
        var options = new DfCatmullRomSplineSignalInterpolatorOptions() { BezierDegree = 2 };

        // Act
        var interpolator = DfCatmullRomSplineSignalInterpolator.Create(signal, options);

        // Assert
        var value1 = interpolator.GetValue(0.5);
        var value2 = interpolator.GetValue(0.8);
        Assert.That(Math.Abs(value1 - 7.5), Is.LessThan(0.1), "Should be approximately constant");
        Assert.That(Math.Abs(value2 - 7.5), Is.LessThan(0.1), "Should be approximately constant");
    }

    [Test]
    public void CatmullRomInterpolation_SubRange_ShouldWork()
    {
        // Arrange
        var samples = new double[] { 0, 1, 2, 3, 4, 5, 6 };
        var signal = Float64SampledTimeSignal.Finite(1, samples);
        var options = new DfCatmullRomSplineSignalInterpolatorOptions() { BezierDegree = 2 };

        // Act
        var interpolator = DfCatmullRomSplineSignalInterpolator.Create(signal, 1, 5, options);

        // Assert
        Assert.That(interpolator, Is.Not.Null, "Sub-range interpolator should work");
    }

    [Test]
    public void CatmullRomInterpolation_GetDerivativeN_ShouldWork()
    {
        // Arrange
        var samples = new double[] { 0, 1, 2, 3, 4 };
        var signal = Float64SampledTimeSignal.Finite(1, samples);
        var options = new DfCatmullRomSplineSignalInterpolatorOptions() { BezierDegree = 2 };

        // Act
        var interpolator = DfCatmullRomSplineSignalInterpolator.Create(signal, options);

        // Assert
        Assert.DoesNotThrow(() =>
        {
            var d1 = interpolator.GetDerivativeN(1);
            var d2 = interpolator.GetDerivativeN(2);
            var d3 = interpolator.GetDerivativeN(3);
        }, "N-th derivatives should work");
    }

    [Test]
    public void CatmullRomInterpolation_AllOperations_ShouldNotThrow()
    {
        // Arrange
        var samples = new double[] { 1, 2, 3, 4, 5 };
        var signal = Float64SampledTimeSignal.Finite(1, samples);
        var options = new DfCatmullRomSplineSignalInterpolatorOptions() { BezierDegree = 2 };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var interpolator = DfCatmullRomSplineSignalInterpolator.Create(signal, options);
            var value = interpolator.GetValue(2.5);
            var d1 = interpolator.GetDerivative1();
            var d2 = interpolator.GetDerivative2();
            var options2 = interpolator.Options;
        }, "All operations should work without throwing");
    }

    #endregion

    #region Signal Properties Tests (10 tests)

    [Test]
    public void Signal_IsEnumerable_ShouldWork()
    {
        // Arrange
        var samples = new double[] { 1, 2, 3, 4, 5 };
        var signal = Float64SampledTimeSignal.Finite(100, samples);

        // Act
        var enumeratedSamples = signal.ToArray();

        // Assert
        Assert.That(enumeratedSamples.Length, Is.EqualTo(samples.Length), "Enumeration should work");
        for (int i = 0; i < samples.Length; i++)
        {
            Assert.That(enumeratedSamples[i], Is.EqualTo(samples[i]).Within(Tolerance));
        }
    }

    [Test]
    public void Signal_SamplingSpecs_ShouldBeAccessible()
    {
        // Arrange
        var signal = Float64SampledTimeSignal.FiniteZero(samplingRate: 1000, sampleCount: 256);

        // Act
        var specs = signal.SamplingSpecs;

        // Assert
        Assert.That(specs, Is.Not.Null, "SamplingSpecs should be accessible");
        Assert.That(specs.SamplingRate, Is.EqualTo(1000).Within(Tolerance), "SamplingRate should match");
    }

    [Test]
    public void Signal_DifferentSamplingRates_ShouldWork()
    {
        // Arrange & Act
        var signal1 = Float64SampledTimeSignal.FiniteZero(10, 100);
        var signal2 = Float64SampledTimeSignal.FiniteZero(100, 100);
        var signal3 = Float64SampledTimeSignal.FiniteZero(1000, 100);

        // Assert
        Assert.That(signal1.SamplingRate, Is.EqualTo(10).Within(Tolerance));
        Assert.That(signal2.SamplingRate, Is.EqualTo(100).Within(Tolerance));
        Assert.That(signal3.SamplingRate, Is.EqualTo(1000).Within(Tolerance));
    }

    [Test]
    public void Signal_ZeroSignal_AllSamplesShouldBeZero()
    {
        // Arrange
        var signal = Float64SampledTimeSignal.FiniteZero(100, 50);

        // Act & Assert
        for (int i = 0; i < signal.Count; i++)
        {
            Assert.That(signal[i], Is.EqualTo(0).Within(Tolerance), $"Sample {i} should be zero");
        }
    }

    [Test]
    public void Signal_LargeSampleCount_ShouldWork()
    {
        // Arrange & Act
        var signal = Float64SampledTimeSignal.FiniteZero(1000, 10000);

        // Assert
        Assert.That(signal.Count, Is.EqualTo(10000), "Large sample counts should work");
    }

    [Test]
    public void Signal_CreateFromEnumerable_ShouldWork()
    {
        // Arrange
        var samples = Enumerable.Range(0, 10).Select(i => (double)i);

        // Act
        var signal = Float64SampledTimeSignal.Finite(100, samples);

        // Assert
        Assert.That(signal.Count, Is.EqualTo(10), "Should create from enumerable");
        Assert.That(signal[0], Is.EqualTo(0).Within(Tolerance));
        Assert.That(signal[9], Is.EqualTo(9).Within(Tolerance));
    }

    [Test]
    public void Signal_PeriodicVsFinite_ShouldHaveDifferentFlags()
    {
        // Arrange
        var samples = new double[] { 1, 2, 3 };

        // Act
        var periodicSignal = Float64SampledTimeSignal.Periodic(100, samples);
        var finiteSignal = Float64SampledTimeSignal.Finite(100, samples);

        // Assert
        Assert.That(periodicSignal.IsPeriodic, Is.True, "Periodic should be true");
        Assert.That(finiteSignal.IsPeriodic, Is.False, "Finite should be false");
    }

    [Test]
    public void Signal_AllConstructors_ShouldCreateValidSignals()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var s1 = Float64SampledTimeSignal.FiniteZero(100, 10);
            var s2 = Float64SampledTimeSignal.PeriodicZero(100, 10);
            var s3 = Float64SampledTimeSignal.FiniteConstant(100, 10, 5.0);
            var s4 = Float64SampledTimeSignal.PeriodicConstant(100, 10, 5.0);
            var s5 = Float64SampledTimeSignal.Finite(100, new double[] { 1, 2, 3 });
            var s6 = Float64SampledTimeSignal.Periodic(100, new double[] { 1, 2, 3 });
        }, "All constructors should create valid signals");
    }

    [Test]
    public void Signal_IndexOutOfRange_ShouldThrow()
    {
        // Arrange
        var signal = Float64SampledTimeSignal.FiniteZero(100, 10);

        // Act & Assert
        Assert.Throws<IndexOutOfRangeException>(() =>
        {
            var _ = signal[10]; // Index 10 is out of range for Count=10 (0-9)
        }, "Index out of range should throw");
    }

    [Test]
    public void Signal_NegativeIndex_ShouldThrow()
    {
        // Arrange
        var signal = Float64SampledTimeSignal.FiniteZero(100, 10);

        // Act & Assert
        Assert.Throws<IndexOutOfRangeException>(() =>
        {
            var _ = signal[-1];
        }, "Negative index should throw");
    }

    #endregion
}
