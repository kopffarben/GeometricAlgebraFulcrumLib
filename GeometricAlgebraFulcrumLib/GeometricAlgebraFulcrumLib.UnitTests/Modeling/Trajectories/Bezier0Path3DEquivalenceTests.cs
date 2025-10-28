using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Bezier;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Bezier;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public class Bezier0Path3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [SetUp]
    public void Setup()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    private LinVector3D<double> CreateGenericVector(double x, double y, double z)
    {
        return LinVector3D<double>.Create(
            ScalarProcessor.Scalar(x),
            ScalarProcessor.Scalar(y),
            ScalarProcessor.Scalar(z)
        );
    }

    [Test]
    public void Bezier0Path3D_Constructor_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var p1Generic = CreateGenericVector(1.0, 2.0, 3.0);

        // Act
        var pathFloat64 = new Float64Bezier0Path3D(false, p1Float64);
        var pathGeneric = Bezier0Path3D<double>.Create(ScalarProcessor, false, p1Generic);

        // Assert
        Assert.That(pathGeneric.Point1.X.ScalarValue, Is.EqualTo(pathFloat64.Point1.X.ScalarValue).Within(Tolerance));
        Assert.That(pathGeneric.Point1.Y.ScalarValue, Is.EqualTo(pathFloat64.Point1.Y.ScalarValue).Within(Tolerance));
        Assert.That(pathGeneric.Point1.Z.ScalarValue, Is.EqualTo(pathFloat64.Point1.Z.ScalarValue).Within(Tolerance));
        Assert.That(pathGeneric.IsPeriodic, Is.EqualTo(pathFloat64.IsPeriodic));
    }

    [Test]
    public void Bezier0Path3D_GetValue_AtStart_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var p1Generic = CreateGenericVector(1.0, 2.0, 3.0);

        var pathFloat64 = new Float64Bezier0Path3D(false, p1Float64);
        var pathGeneric = Bezier0Path3D<double>.Create(ScalarProcessor, false, p1Generic);

        // Act
        var valueFloat64 = pathFloat64.GetValue(0.0);
        var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(0.0));

        // Assert - Degree 0 Bezier is constant, should always return Point1
        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z.ScalarValue).Within(Tolerance));
        Assert.That(valueFloat64.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(valueFloat64.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance));
        Assert.That(valueFloat64.Z.ScalarValue, Is.EqualTo(3.0).Within(Tolerance));
    }

    [Test]
    public void Bezier0Path3D_GetValue_AtMidpoint_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(5.0, -3.0, 7.0);
        var p1Generic = CreateGenericVector(5.0, -3.0, 7.0);

        var pathFloat64 = new Float64Bezier0Path3D(false, p1Float64);
        var pathGeneric = Bezier0Path3D<double>.Create(ScalarProcessor, false, p1Generic);

        // Act
        var valueFloat64 = pathFloat64.GetValue(0.5);
        var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(0.5));

        // Assert - Degree 0 Bezier is constant at all t values
        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z.ScalarValue).Within(Tolerance));
        Assert.That(valueFloat64.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance));
        Assert.That(valueFloat64.Y.ScalarValue, Is.EqualTo(-3.0).Within(Tolerance));
        Assert.That(valueFloat64.Z.ScalarValue, Is.EqualTo(7.0).Within(Tolerance));
    }

    [Test]
    public void Bezier0Path3D_GetValue_AtEnd_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(-2.0, 4.0, -6.0);
        var p1Generic = CreateGenericVector(-2.0, 4.0, -6.0);

        var pathFloat64 = new Float64Bezier0Path3D(false, p1Float64);
        var pathGeneric = Bezier0Path3D<double>.Create(ScalarProcessor, false, p1Generic);

        // Act
        var valueFloat64 = pathFloat64.GetValue(1.0);
        var valueGeneric = pathGeneric.GetValue(ScalarProcessor.Scalar(1.0));

        // Assert - Degree 0 Bezier is constant at all t values
        Assert.That(valueGeneric.X.ScalarValue, Is.EqualTo(valueFloat64.X.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Y.ScalarValue, Is.EqualTo(valueFloat64.Y.ScalarValue).Within(Tolerance));
        Assert.That(valueGeneric.Z.ScalarValue, Is.EqualTo(valueFloat64.Z.ScalarValue).Within(Tolerance));
        Assert.That(valueFloat64.X.ScalarValue, Is.EqualTo(-2.0).Within(Tolerance));
        Assert.That(valueFloat64.Y.ScalarValue, Is.EqualTo(4.0).Within(Tolerance));
        Assert.That(valueFloat64.Z.ScalarValue, Is.EqualTo(-6.0).Within(Tolerance));
    }

    [Test]
    public void Bezier0Path3D_GetValue_IsConstant_ForAllT()
    {
        // Arrange
        var p1Generic = CreateGenericVector(3.0, 7.0, -1.0);
        var pathGeneric = Bezier0Path3D<double>.Create(ScalarProcessor, false, p1Generic);

        // Act & Assert - Verify constant for multiple t values
        var t_values = new[] { 0.0, 0.25, 0.5, 0.75, 1.0 };

        foreach (var t in t_values)
        {
            var value = pathGeneric.GetValue(ScalarProcessor.Scalar(t));
            Assert.That(value.X.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), $"Failed at t={t}");
            Assert.That(value.Y.ScalarValue, Is.EqualTo(7.0).Within(Tolerance), $"Failed at t={t}");
            Assert.That(value.Z.ScalarValue, Is.EqualTo(-1.0).Within(Tolerance), $"Failed at t={t}");
        }
    }

    [Test]
    public void Bezier0Path3D_GetDerivative1Value_ShouldBeZero()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var p1Generic = CreateGenericVector(1.0, 2.0, 3.0);

        var pathFloat64 = new Float64Bezier0Path3D(false, p1Float64);
        var pathGeneric = Bezier0Path3D<double>.Create(ScalarProcessor, false, p1Generic);

        // Act - Test at multiple t values
        var deriv1Float64_0 = pathFloat64.GetDerivative1Value(0.0);
        var deriv1Generic_0 = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(0.0));

        var deriv1Float64_05 = pathFloat64.GetDerivative1Value(0.5);
        var deriv1Generic_05 = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(0.5));

        var deriv1Float64_1 = pathFloat64.GetDerivative1Value(1.0);
        var deriv1Generic_1 = pathGeneric.GetDerivative1Value(ScalarProcessor.Scalar(1.0));

        // Assert - First derivative should be zero (no velocity) for constant curve
        Assert.That(deriv1Generic_0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(deriv1Generic_0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(deriv1Generic_0.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));

        Assert.That(deriv1Generic_05.X.ScalarValue, Is.EqualTo(deriv1Float64_05.X.ScalarValue).Within(Tolerance));
        Assert.That(deriv1Generic_05.Y.ScalarValue, Is.EqualTo(deriv1Float64_05.Y.ScalarValue).Within(Tolerance));
        Assert.That(deriv1Generic_05.Z.ScalarValue, Is.EqualTo(deriv1Float64_05.Z.ScalarValue).Within(Tolerance));

        Assert.That(deriv1Generic_1.X.ScalarValue, Is.EqualTo(deriv1Float64_1.X.ScalarValue).Within(Tolerance));
        Assert.That(deriv1Generic_1.Y.ScalarValue, Is.EqualTo(deriv1Float64_1.Y.ScalarValue).Within(Tolerance));
        Assert.That(deriv1Generic_1.Z.ScalarValue, Is.EqualTo(deriv1Float64_1.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void Bezier0Path3D_GetDerivative2Value_ShouldBeZero()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var p1Generic = CreateGenericVector(1.0, 2.0, 3.0);

        var pathFloat64 = new Float64Bezier0Path3D(false, p1Float64);
        var pathGeneric = Bezier0Path3D<double>.Create(ScalarProcessor, false, p1Generic);

        // Act - Test at multiple t values
        var deriv2Float64_0 = pathFloat64.GetDerivative2Value(0.0);
        var deriv2Generic_0 = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(0.0));

        var deriv2Float64_05 = pathFloat64.GetDerivative2Value(0.5);
        var deriv2Generic_05 = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(0.5));

        var deriv2Float64_1 = pathFloat64.GetDerivative2Value(1.0);
        var deriv2Generic_1 = pathGeneric.GetDerivative2Value(ScalarProcessor.Scalar(1.0));

        // Assert - Second derivative should be zero (no acceleration) for constant curve
        Assert.That(deriv2Generic_0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(deriv2Generic_0.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(deriv2Generic_0.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));

        Assert.That(deriv2Generic_05.X.ScalarValue, Is.EqualTo(deriv2Float64_05.X.ScalarValue).Within(Tolerance));
        Assert.That(deriv2Generic_05.Y.ScalarValue, Is.EqualTo(deriv2Float64_05.Y.ScalarValue).Within(Tolerance));
        Assert.That(deriv2Generic_05.Z.ScalarValue, Is.EqualTo(deriv2Float64_05.Z.ScalarValue).Within(Tolerance));

        Assert.That(deriv2Generic_1.X.ScalarValue, Is.EqualTo(deriv2Float64_1.X.ScalarValue).Within(Tolerance));
        Assert.That(deriv2Generic_1.Y.ScalarValue, Is.EqualTo(deriv2Float64_1.Y.ScalarValue).Within(Tolerance));
        Assert.That(deriv2Generic_1.Z.ScalarValue, Is.EqualTo(deriv2Float64_1.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void Bezier0Path3D_ToFinitePath_ShouldPreserveGeometry()
    {
        // Arrange
        var p1Generic = CreateGenericVector(2.0, 3.0, 4.0);
        var pathPeriodic = Bezier0Path3D<double>.Create(ScalarProcessor, true, p1Generic);
        var pathFinite = Bezier0Path3D<double>.Create(ScalarProcessor, false, p1Generic);

        // Act
        var convertedFromPeriodic = pathPeriodic.ToFinitePath();
        var convertedFromFinite = pathFinite.ToFinitePath();

        // Assert
        Assert.That(convertedFromPeriodic.IsPeriodic, Is.False);
        Assert.That(convertedFromFinite.IsPeriodic, Is.False);

        // Verify geometry is preserved
        var value1 = convertedFromPeriodic.GetValue(ScalarProcessor.Scalar(0.5));
        var value2 = pathPeriodic.GetValue(ScalarProcessor.Scalar(0.5));

        Assert.That(value1.X.ScalarValue, Is.EqualTo(value2.X.ScalarValue).Within(Tolerance));
        Assert.That(value1.Y.ScalarValue, Is.EqualTo(value2.Y.ScalarValue).Within(Tolerance));
        Assert.That(value1.Z.ScalarValue, Is.EqualTo(value2.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void Bezier0Path3D_ToPeriodicPath_ShouldPreserveGeometry()
    {
        // Arrange
        var p1Generic = CreateGenericVector(2.0, 3.0, 4.0);
        var pathPeriodic = Bezier0Path3D<double>.Create(ScalarProcessor, true, p1Generic);
        var pathFinite = Bezier0Path3D<double>.Create(ScalarProcessor, false, p1Generic);

        // Act
        var convertedFromPeriodic = pathPeriodic.ToPeriodicPath();
        var convertedFromFinite = pathFinite.ToPeriodicPath();

        // Assert
        Assert.That(convertedFromPeriodic.IsPeriodic, Is.True);
        Assert.That(convertedFromFinite.IsPeriodic, Is.True);

        // Verify geometry is preserved
        var value1 = convertedFromFinite.GetValue(ScalarProcessor.Scalar(0.5));
        var value2 = pathFinite.GetValue(ScalarProcessor.Scalar(0.5));

        Assert.That(value1.X.ScalarValue, Is.EqualTo(value2.X.ScalarValue).Within(Tolerance));
        Assert.That(value1.Y.ScalarValue, Is.EqualTo(value2.Y.ScalarValue).Within(Tolerance));
        Assert.That(value1.Z.ScalarValue, Is.EqualTo(value2.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void Bezier0Path3D_IsValid_ShouldMatchFloat64()
    {
        // Arrange
        var p1Float64 = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var p1Generic = CreateGenericVector(1.0, 2.0, 3.0);

        var pathFloat64 = new Float64Bezier0Path3D(false, p1Float64);
        var pathGeneric = Bezier0Path3D<double>.Create(ScalarProcessor, false, p1Generic);

        // Act & Assert
        Assert.That(pathGeneric.IsValid(), Is.EqualTo(pathFloat64.IsValid()));
        Assert.That(pathGeneric.IsValid(), Is.True);
    }
}
