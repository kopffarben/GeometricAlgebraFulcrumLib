using System;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors2D.Generic.Basic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Tests für Generic HarmonicPath2D&lt;T&gt;
/// Phase 3 Module 6B - 2D Harmonic Parametric Paths
/// Tests: 2D-Pfade aus harmonischen Signalen, Lissajous-Figuren
/// </summary>
[TestFixture]
public class HarmonicPath2DEquivalenceTests
{
    private const double Tolerance = 1e-12;

    #region Test Setup

    private static readonly ScalarProcessorOfFloat64 ScalarProcessor = ScalarProcessorOfFloat64.Instance;

    #endregion

    #region Circular Motion Tests (2 tests)

    [Test]
    public void HarmonicPath2D_CircularMotion_ShouldProduceCircle()
    {
        // Arrange - Kreis: x = cos(2πt), y = sin(2πt) = cos(2πt - π/2)
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        // x = cos(2πt) → frequency = 1 Hz, timeOffset = 0
        var xSignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.One,        // 1 Hz
            ScalarProcessor.One,        // magnitude = 1
            ScalarProcessor.Zero        // timeOffset = 0
        );

        // y = sin(2πt) = cos(2πt - π/2) → timeOffset = -1/4 period = -0.25
        var ySignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.One,        // 1 Hz
            ScalarProcessor.One,        // magnitude = 1
            ScalarProcessor.Scalar(-0.25)  // timeOffset = -π/(2π) = -0.25
        );

        var path = HarmonicPath2D<double>.Create(xSignal, ySignal);

        // Act & Assert - Prüfe Kardinalspunkte
        var p0 = path.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(p0.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X at t=0");
        Assert.That(p0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=0");

        var p1 = path.GetValue(ScalarProcessor.Scalar(0.25));
        Assert.That(p1.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=0.25");
        Assert.That(p1.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Y at t=0.25");

        var p2 = path.GetValue(ScalarProcessor.Scalar(0.5));
        Assert.That(p2.X.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), "X at t=0.5");
        Assert.That(p2.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Y at t=0.5");

        var p3 = path.GetValue(ScalarProcessor.Scalar(0.75));
        Assert.That(p3.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=0.75");
        Assert.That(p3.Y.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), "Y at t=0.75");
    }

    [Test]
    public void HarmonicPath2D_CircularMotion_RadiusShouldBeConstant()
    {
        // Arrange - Kreis mit Radius 2
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var radius = 2.0;

        var xSignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.One,
            ScalarProcessor.Scalar(radius),
            ScalarProcessor.Zero
        );

        var ySignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.One,
            ScalarProcessor.Scalar(radius),
            ScalarProcessor.Scalar(-0.25)
        );

        var path = HarmonicPath2D<double>.Create(xSignal, ySignal);

        // Act & Assert - Radius sollte konstant 2 sein
        for (var t = 0.0; t <= 1.0; t += 0.125)
        {
            var pos = path.GetValue(ScalarProcessor.Scalar(t));
            var actualRadius = Math.Sqrt(pos.X.ScalarValue * pos.X.ScalarValue +
                                         pos.Y.ScalarValue * pos.Y.ScalarValue);

            Assert.That(actualRadius, Is.EqualTo(radius).Within(Tolerance), $"Radius should be {radius} at t={t}");
        }
    }

    #endregion

    #region Lissajous Curves Tests (3 tests)

    [Test]
    public void HarmonicPath2D_LissajousCurve_OneToTwo_ShouldProduceEightShape()
    {
        // Arrange - Lissajous 1:2 Kurve: x = cos(2πt), y = cos(4πt)
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var xSignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.One,        // 1 Hz
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var ySignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.Scalar(2.0),  // 2 Hz
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var path = HarmonicPath2D<double>.Create(xSignal, ySignal);

        // Act & Assert - Prüfe charakteristische Punkte
        var p0 = path.GetValue(ScalarProcessor.Scalar(0.0));
        Assert.That(p0.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "X at t=0");
        Assert.That(p0.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Y at t=0");

        var p1 = path.GetValue(ScalarProcessor.Scalar(0.25));
        Assert.That(p1.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "X at t=0.25");
        Assert.That(p1.Y.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), "Y at t=0.25");

        var p2 = path.GetValue(ScalarProcessor.Scalar(0.5));
        Assert.That(p2.X.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), "X at t=0.5");
        Assert.That(p2.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Y at t=0.5");
    }

    [Test]
    public void HarmonicPath2D_LissajousCurve_ThreeToTwo_ShouldBePeriodic()
    {
        // Arrange - Lissajous 3:2 Kurve
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var xSignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.Scalar(3.0),  // 3 Hz
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var ySignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.Scalar(2.0),  // 2 Hz
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var path = HarmonicPath2D<double>.Create(xSignal, ySignal);

        // Act & Assert - Werte bei t=0 und t=1 sollten gleich sein (Periode = 1)
        var p0 = path.GetValue(ScalarProcessor.Scalar(0.0));
        var p1 = path.GetValue(ScalarProcessor.Scalar(1.0));

        Assert.That(p1.X.ScalarValue, Is.EqualTo(p0.X.ScalarValue).Within(Tolerance), "X should be periodic");
        Assert.That(p1.Y.ScalarValue, Is.EqualTo(p0.Y.ScalarValue).Within(Tolerance), "Y should be periodic");
    }

    [Test]
    public void HarmonicPath2D_LissajousWithPhaseShift_ShouldProduceEllipse()
    {
        // Arrange - Ellipse durch 90° Phasenverschiebung: x = cos(2πt), y = cos(2πt - π/2) = sin(2πt)
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var xSignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.One,
            ScalarProcessor.Scalar(3.0),  // magnitude = 3 (horizontale Halbachse)
            ScalarProcessor.Zero
        );

        var ySignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.One,
            ScalarProcessor.Scalar(2.0),  // magnitude = 2 (vertikale Halbachse)
            ScalarProcessor.Scalar(-0.25)  // 90° phase shift
        );

        var path = HarmonicPath2D<double>.Create(xSignal, ySignal);

        // Act & Assert - Prüfe Ellipsenform: (x/3)² + (y/2)² = 1
        for (var t = 0.0; t <= 1.0; t += 0.125)
        {
            var pos = path.GetValue(ScalarProcessor.Scalar(t));
            var ellipseValue = (pos.X.ScalarValue / 3.0) * (pos.X.ScalarValue / 3.0) +
                               (pos.Y.ScalarValue / 2.0) * (pos.Y.ScalarValue / 2.0);

            Assert.That(ellipseValue, Is.EqualTo(1.0).Within(Tolerance), $"Point should lie on ellipse at t={t}");
        }
    }

    #endregion

    #region Derivative Tests (2 tests)

    [Test]
    public void HarmonicPath2D_FirstDerivative_ShouldMatchAnalyticalFormula()
    {
        // Arrange - x = cos(2πt), y = cos(4πt)
        // dx/dt = -2π sin(2πt), dy/dt = -4π sin(4πt)
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var xSignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.One,        // 1 Hz
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var ySignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.Scalar(2.0),  // 2 Hz
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var path = HarmonicPath2D<double>.Create(xSignal, ySignal);

        // Act & Assert - Prüfe Ableitungen
        var t = 0.125;  // t = 1/8
        var deriv = path.GetDerivative1Value(ScalarProcessor.Scalar(t));

        var omega1 = 2.0 * Math.PI * 1.0;  // w = 2π * 1 Hz
        var omega2 = 2.0 * Math.PI * 2.0;  // w = 2π * 2 Hz

        var expectedDx = -omega1 * Math.Sin(omega1 * t);
        var expectedDy = -omega2 * Math.Sin(omega2 * t);

        Assert.That(deriv.X.ScalarValue, Is.EqualTo(expectedDx).Within(Tolerance), "First derivative X");
        Assert.That(deriv.Y.ScalarValue, Is.EqualTo(expectedDy).Within(Tolerance), "First derivative Y");
    }

    [Test]
    public void HarmonicPath2D_SecondDerivative_ShouldPointTowardsCenter()
    {
        // Arrange - Kreisbewegung: Beschleunigung zeigt zum Zentrum
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var radius = 1.5;

        var xSignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.One,
            ScalarProcessor.Scalar(radius),
            ScalarProcessor.Zero
        );

        var ySignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.One,
            ScalarProcessor.Scalar(radius),
            ScalarProcessor.Scalar(-0.25)
        );

        var path = HarmonicPath2D<double>.Create(xSignal, ySignal);

        // Act & Assert - Zweite Ableitung sollte zum Zentrum zeigen (Zentripetalbeschleunigung)
        var omega = 2.0 * Math.PI;
        var expectedAccelMagnitude = omega * omega * radius;

        for (var t = 0.0; t <= 1.0; t += 0.125)
        {
            var tScalar = ScalarProcessor.Scalar(t);
            var pos = path.GetValue(tScalar);
            var deriv2 = path.GetDerivative2Value(tScalar);

            // Beschleunigung sollte entgegen der Position zeigen (zum Zentrum)
            var dotProduct = pos.X.ScalarValue * deriv2.X.ScalarValue +
                            pos.Y.ScalarValue * deriv2.Y.ScalarValue;

            // Sollte negativ sein (zeigt nach innen)
            Assert.That(dotProduct, Is.LessThan(0), $"Acceleration should point inward at t={t}");
            Assert.That(Math.Abs(dotProduct), Is.EqualTo(expectedAccelMagnitude * radius).Within(Tolerance),
                $"Acceleration magnitude at t={t}");
        }
    }

    #endregion

    #region Component and Conversion Tests (3 tests)

    [Test]
    public void HarmonicPath2D_GetScalarComponents_ShouldReturnOriginalSignals()
    {
        // Arrange
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var xSignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var ySignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.Scalar(2.0),
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var path = HarmonicPath2D<double>.Create(xSignal, ySignal);

        // Act
        var (retrievedXSignal, retrievedYSignal) = path.GetScalarComponents();

        // Assert - Die Signale sollten dieselben Referenzen sein (effiziente Überschreibung)
        Assert.That(ReferenceEquals(retrievedXSignal, xSignal), Is.True, "X signal should be same reference");
        Assert.That(ReferenceEquals(retrievedYSignal, ySignal), Is.True, "Y signal should be same reference");
    }

    [Test]
    public void HarmonicPath2D_GetFrame_TangentShouldBeNormalized()
    {
        // Arrange
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var xSignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var ySignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Scalar(-0.25)
        );

        var path = HarmonicPath2D<double>.Create(xSignal, ySignal);

        // Act & Assert - Prüfe Tangentennormalisierung
        for (var t = 0.0; t <= 1.0; t += 0.125)
        {
            var frame = path.GetFrame(ScalarProcessor.Scalar(t));

            var tangentNormSq = frame.Tangent.X.ScalarValue * frame.Tangent.X.ScalarValue +
                                frame.Tangent.Y.ScalarValue * frame.Tangent.Y.ScalarValue;

            Assert.That(tangentNormSq, Is.EqualTo(1.0).Within(Tolerance),
                $"Frame tangent should be normalized at t={t}");
        }
    }

    [Test]
    public void HarmonicPath2D_Conversion_FiniteToPeriodicShouldCreateNewInstance()
    {
        // Arrange
        var timeRange = ScalarRange<double>.Create(
            ScalarProcessor.Scalar(0),
            ScalarProcessor.Scalar(1)
        );

        var xSignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var ySignal = HarmonicScalarSignal<double>.Finite(
            timeRange,
            ScalarProcessor.One,
            ScalarProcessor.One,
            ScalarProcessor.Zero
        );

        var path = HarmonicPath2D<double>.Create(xSignal, ySignal);

        // Act
        var periodicPath = path.ToPeriodicPath();

        // Assert
        Assert.That(ReferenceEquals(path, periodicPath), Is.False, "ToPeriodicPath should create new instance");
        Assert.That(periodicPath.IsPeriodic, Is.True, "Converted path should be periodic");
        Assert.That(path.IsFinite, Is.True, "Original path should remain finite");
    }

    #endregion
}
