using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public class ConstantPath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [OneTimeSetUp]
    public void ClassInit()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void ConstantPath_Finite_GetValue_ShouldReturnSamePoint()
    {
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var pathFloat64 = Float64ConstantPath3D.Finite(point);

        var pointGeneric = LinVector3D<double>.Create(
            ScalarProcessor.ScalarFromNumber(1),
            ScalarProcessor.ScalarFromNumber(2),
            ScalarProcessor.ScalarFromNumber(3)
        );
        var pathGeneric = ConstantPath3D<double>.Finite(ScalarProcessor, pointGeneric);

        // Test at different parameter values
        var t1Float64 = pathFloat64.GetValue(0.0);
        var t1Generic = pathGeneric.GetValue(ScalarProcessor.Zero);

        Assert.That(t1Generic.X.ScalarValue, Is.EqualTo(t1Float64.X.ScalarValue).Within(Tolerance));
        Assert.That(t1Generic.Y.ScalarValue, Is.EqualTo(t1Float64.Y.ScalarValue).Within(Tolerance));
        Assert.That(t1Generic.Z.ScalarValue, Is.EqualTo(t1Float64.Z.ScalarValue).Within(Tolerance));

        var t2Float64 = pathFloat64.GetValue(0.5);
        var t2Generic = pathGeneric.GetValue(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(t2Generic.X.ScalarValue, Is.EqualTo(t2Float64.X.ScalarValue).Within(Tolerance));
        Assert.That(t2Generic.Y.ScalarValue, Is.EqualTo(t2Float64.Y.ScalarValue).Within(Tolerance));
        Assert.That(t2Generic.Z.ScalarValue, Is.EqualTo(t2Float64.Z.ScalarValue).Within(Tolerance));

        var t3Float64 = pathFloat64.GetValue(1.0);
        var t3Generic = pathGeneric.GetValue(ScalarProcessor.One);

        Assert.That(t3Generic.X.ScalarValue, Is.EqualTo(t3Float64.X.ScalarValue).Within(Tolerance));
        Assert.That(t3Generic.Y.ScalarValue, Is.EqualTo(t3Float64.Y.ScalarValue).Within(Tolerance));
        Assert.That(t3Generic.Z.ScalarValue, Is.EqualTo(t3Float64.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void ConstantPath_Finite_GetDerivative1Value_ShouldReturnZero()
    {
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var pathFloat64 = Float64ConstantPath3D.Finite(point);

        var pointGeneric = LinVector3D<double>.Create(
            ScalarProcessor.ScalarFromNumber(1),
            ScalarProcessor.ScalarFromNumber(2),
            ScalarProcessor.ScalarFromNumber(3)
        );
        var pathGeneric = ConstantPath3D<double>.Finite(ScalarProcessor, pointGeneric);

        var deriv1Float64 = pathFloat64.GetDerivative1Value(0.5);
        var deriv1Generic = pathGeneric.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(deriv1Generic.X.ScalarValue, Is.EqualTo(deriv1Float64.X.ScalarValue).Within(Tolerance));
        Assert.That(deriv1Generic.Y.ScalarValue, Is.EqualTo(deriv1Float64.Y.ScalarValue).Within(Tolerance));
        Assert.That(deriv1Generic.Z.ScalarValue, Is.EqualTo(deriv1Float64.Z.ScalarValue).Within(Tolerance));

        // Should be zero
        Assert.That(deriv1Generic.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(deriv1Generic.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(deriv1Generic.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
    }

    [Test]
    public void ConstantPath_WithTangent_GetDerivative1Value_ShouldReturnTangent()
    {
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var tangent = LinFloat64Vector3D.Create(0.5, 1.0, 1.5);
        var pathFloat64 = Float64ConstantPath3D.Finite(point, tangent);

        var pointGeneric = LinVector3D<double>.Create(
            ScalarProcessor.ScalarFromNumber(1),
            ScalarProcessor.ScalarFromNumber(2),
            ScalarProcessor.ScalarFromNumber(3)
        );
        var tangentGeneric = LinVector3D<double>.Create(
            ScalarProcessor.ScalarFromNumber(0.5),
            ScalarProcessor.ScalarFromNumber(1.0),
            ScalarProcessor.ScalarFromNumber(1.5)
        );
        var pathGeneric = ConstantPath3D<double>.Finite(ScalarProcessor, pointGeneric, tangentGeneric);

        var deriv1Float64 = pathFloat64.GetDerivative1Value(0.5);
        var deriv1Generic = pathGeneric.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(deriv1Generic.X.ScalarValue, Is.EqualTo(deriv1Float64.X.ScalarValue).Within(Tolerance));
        Assert.That(deriv1Generic.Y.ScalarValue, Is.EqualTo(deriv1Float64.Y.ScalarValue).Within(Tolerance));
        Assert.That(deriv1Generic.Z.ScalarValue, Is.EqualTo(deriv1Float64.Z.ScalarValue).Within(Tolerance));

        // Should be tangent
        Assert.That(deriv1Generic.X.ScalarValue, Is.EqualTo(0.5).Within(Tolerance));
        Assert.That(deriv1Generic.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(deriv1Generic.Z.ScalarValue, Is.EqualTo(1.5).Within(Tolerance));
    }

    [Test]
    public void ConstantPath_GetDerivative2Value_ShouldReturnZero()
    {
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var tangent = LinFloat64Vector3D.Create(0.5, 1.0, 1.5);
        var pathFloat64 = Float64ConstantPath3D.Finite(point, tangent);

        var pointGeneric = LinVector3D<double>.Create(
            ScalarProcessor.ScalarFromNumber(1),
            ScalarProcessor.ScalarFromNumber(2),
            ScalarProcessor.ScalarFromNumber(3)
        );
        var tangentGeneric = LinVector3D<double>.Create(
            ScalarProcessor.ScalarFromNumber(0.5),
            ScalarProcessor.ScalarFromNumber(1.0),
            ScalarProcessor.ScalarFromNumber(1.5)
        );
        var pathGeneric = ConstantPath3D<double>.Finite(ScalarProcessor, pointGeneric, tangentGeneric);

        var deriv2Float64 = pathFloat64.GetDerivative2Value(0.5);
        var deriv2Generic = pathGeneric.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(0.5));

        Assert.That(deriv2Generic.X.ScalarValue, Is.EqualTo(deriv2Float64.X.ScalarValue).Within(Tolerance));
        Assert.That(deriv2Generic.Y.ScalarValue, Is.EqualTo(deriv2Float64.Y.ScalarValue).Within(Tolerance));
        Assert.That(deriv2Generic.Z.ScalarValue, Is.EqualTo(deriv2Float64.Z.ScalarValue).Within(Tolerance));

        // Should be zero
        Assert.That(deriv2Generic.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(deriv2Generic.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(deriv2Generic.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
    }

    [Test]
    public void ConstantPath_IsValid_ShouldReturnTrue()
    {
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var pathFloat64 = Float64ConstantPath3D.Finite(point);

        var pointGeneric = LinVector3D<double>.Create(
            ScalarProcessor.ScalarFromNumber(1),
            ScalarProcessor.ScalarFromNumber(2),
            ScalarProcessor.ScalarFromNumber(3)
        );
        var pathGeneric = ConstantPath3D<double>.Finite(ScalarProcessor, pointGeneric);

        Assert.That(pathFloat64.IsValid(), Is.True);
        Assert.That(pathGeneric.IsValid(), Is.True);
    }

    [Test]
    public void ConstantPath_ToFinitePath_ShouldReturnSelf()
    {
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var pathFloat64 = Float64ConstantPath3D.Finite(point);

        var pointGeneric = LinVector3D<double>.Create(
            ScalarProcessor.ScalarFromNumber(1),
            ScalarProcessor.ScalarFromNumber(2),
            ScalarProcessor.ScalarFromNumber(3)
        );
        var pathGeneric = ConstantPath3D<double>.Finite(ScalarProcessor, pointGeneric);

        var finiteFloat64 = pathFloat64.ToFinitePath();
        var finiteGeneric = pathGeneric.ToFinitePath();

        Assert.That(finiteFloat64, Is.SameAs(pathFloat64));
        Assert.That(finiteGeneric, Is.SameAs(pathGeneric));
    }

    [Test]
    public void ConstantPath_ToPeriodicPath_ShouldReturnNewInstance()
    {
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var pathFloat64 = Float64ConstantPath3D.Finite(point);

        var pointGeneric = LinVector3D<double>.Create(
            ScalarProcessor.ScalarFromNumber(1),
            ScalarProcessor.ScalarFromNumber(2),
            ScalarProcessor.ScalarFromNumber(3)
        );
        var pathGeneric = ConstantPath3D<double>.Finite(ScalarProcessor, pointGeneric);

        var periodicFloat64 = pathFloat64.ToPeriodicPath();
        var periodicGeneric = pathGeneric.ToPeriodicPath();

        Assert.That(periodicFloat64, Is.Not.SameAs(pathFloat64));
        Assert.That(periodicGeneric, Is.Not.SameAs(pathGeneric));

        Assert.That(periodicFloat64.IsPeriodic, Is.True);
        Assert.That(periodicGeneric.IsPeriodic, Is.True);
    }

    [Test]
    public void ConstantPath_Point_ShouldMatchConstructorValue()
    {
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var pathFloat64 = Float64ConstantPath3D.Finite(point);

        var pointGeneric = LinVector3D<double>.Create(
            ScalarProcessor.ScalarFromNumber(1),
            ScalarProcessor.ScalarFromNumber(2),
            ScalarProcessor.ScalarFromNumber(3)
        );
        var pathGeneric = ConstantPath3D<double>.Finite(ScalarProcessor, pointGeneric);

        Assert.That(pathGeneric.Point.X.ScalarValue, Is.EqualTo(pathFloat64.Point.X.ScalarValue).Within(Tolerance));
        Assert.That(pathGeneric.Point.Y.ScalarValue, Is.EqualTo(pathFloat64.Point.Y.ScalarValue).Within(Tolerance));
        Assert.That(pathGeneric.Point.Z.ScalarValue, Is.EqualTo(pathFloat64.Point.Z.ScalarValue).Within(Tolerance));
    }

    [Test]
    public void ConstantPath_Tangent_ShouldMatchConstructorValue()
    {
        var point = LinFloat64Vector3D.Create(1, 2, 3);
        var tangent = LinFloat64Vector3D.Create(0.5, 1.0, 1.5);
        var pathFloat64 = Float64ConstantPath3D.Finite(point, tangent);

        var pointGeneric = LinVector3D<double>.Create(
            ScalarProcessor.ScalarFromNumber(1),
            ScalarProcessor.ScalarFromNumber(2),
            ScalarProcessor.ScalarFromNumber(3)
        );
        var tangentGeneric = LinVector3D<double>.Create(
            ScalarProcessor.ScalarFromNumber(0.5),
            ScalarProcessor.ScalarFromNumber(1.0),
            ScalarProcessor.ScalarFromNumber(1.5)
        );
        var pathGeneric = ConstantPath3D<double>.Finite(ScalarProcessor, pointGeneric, tangentGeneric);

        Assert.That(pathGeneric.Tangent.X.ScalarValue, Is.EqualTo(pathFloat64.Tangent.X.ScalarValue).Within(Tolerance));
        Assert.That(pathGeneric.Tangent.Y.ScalarValue, Is.EqualTo(pathFloat64.Tangent.Y.ScalarValue).Within(Tolerance));
        Assert.That(pathGeneric.Tangent.Z.ScalarValue, Is.EqualTo(pathFloat64.Tangent.Z.ScalarValue).Within(Tolerance));
    }
}
