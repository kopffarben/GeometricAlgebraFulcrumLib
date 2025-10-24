using System;
using System.Diagnostics;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

[TestFixture]
public sealed class XGaAngleVectorEquivalenceTests
{
    private const double Tolerance = 1e-14;

    private XGaFloat64Processor _float64Processor = null!;
    private XGaProcessor<double> _genericProcessor = null!;


    [SetUp]
    public void Setup()
    {
        _float64Processor = XGaFloat64Processor.Euclidean;
        _genericProcessor = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);
    }


    [Test]
    public void LinFloat64Angle_CreateUnitVector_E01_ShouldProduceIdenticalResults()
    {
        // Arrange
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var angleFloat64 = LinFloat64PolarAngle.CreateFromRadians(Math.PI / 4); // 45 degrees
        var angleGeneric = LinPolarAngle<double>.CreateFromRadians(scalarProcessor.ScalarFromValue(Math.PI / 4));

        // Act - Create unit vectors in e0-e1 plane
        var float64Vector = angleFloat64.CreateUnitVector(0, 1, _float64Processor);
        var genericVector = angleGeneric.CreateUnitVector(0, 1, _genericProcessor);

        // Assert - Components should be identical
        var cosValue = Math.Cos(Math.PI / 4);
        var sinValue = Math.Sin(Math.PI / 4);

        Assert.That(float64Vector.Scalar(0), Is.EqualTo(cosValue).Within(Tolerance),
            "Float64 e0 component should be cos(π/4)");
        Assert.That(float64Vector.Scalar(1), Is.EqualTo(sinValue).Within(Tolerance),
            "Float64 e1 component should be sin(π/4)");

        Assert.That(genericVector.Scalar(0).ScalarValue, Is.EqualTo(cosValue).Within(Tolerance),
            "Generic e0 component should be cos(π/4)");
        Assert.That(genericVector.Scalar(1).ScalarValue, Is.EqualTo(sinValue).Within(Tolerance),
            "Generic e1 component should be sin(π/4)");

        // Float64 and Generic should match
        Assert.That(genericVector.Scalar(0).ScalarValue, Is.EqualTo(float64Vector.Scalar(0)).Within(Tolerance),
            "Float64 and Generic e0 components should match");
        Assert.That(genericVector.Scalar(1).ScalarValue, Is.EqualTo(float64Vector.Scalar(1)).Within(Tolerance),
            "Float64 and Generic e1 components should match");

        // Should be unit vector (norm = 1)
        var float64Norm = float64Vector.ENorm().ScalarValue;
        var genericNorm = genericVector.ENorm().ScalarValue;

        Debug.Assert(Math.Abs(float64Norm - 1.0) < Tolerance, "Float64 vector should be unit length");
        Debug.Assert(Math.Abs(genericNorm - 1.0) < Tolerance, "Generic vector should be unit length");
    }


    [Test]
    public void LinFloat64Angle_CreateUnitVector_E23_ShouldProduceIdenticalResults()
    {
        // Arrange
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var angleFloat64 = LinFloat64PolarAngle.CreateFromRadians(Math.PI / 3); // 60 degrees
        var angleGeneric = LinPolarAngle<double>.CreateFromRadians(scalarProcessor.ScalarFromValue(Math.PI / 3));

        // Act - Create unit vectors in e2-e3 plane
        var float64Vector = angleFloat64.CreateUnitVector(2, 3, _float64Processor);
        var genericVector = angleGeneric.CreateUnitVector(2, 3, _genericProcessor);

        // Assert
        var cosValue = Math.Cos(Math.PI / 3);
        var sinValue = Math.Sin(Math.PI / 3);

        Assert.That(float64Vector.Scalar(2), Is.EqualTo(cosValue).Within(Tolerance));
        Assert.That(float64Vector.Scalar(3), Is.EqualTo(sinValue).Within(Tolerance));

        Assert.That(genericVector.Scalar(2).ScalarValue, Is.EqualTo(cosValue).Within(Tolerance));
        Assert.That(genericVector.Scalar(3).ScalarValue, Is.EqualTo(sinValue).Within(Tolerance));

        Assert.That(genericVector.Scalar(2).ScalarValue, Is.EqualTo(float64Vector.Scalar(2)).Within(Tolerance));
        Assert.That(genericVector.Scalar(3).ScalarValue, Is.EqualTo(float64Vector.Scalar(3)).Within(Tolerance));
    }


    [Test]
    public void LinFloat64Angle_CreatePhasor_WithMagnitude2_ShouldProduceIdenticalResults()
    {
        // Arrange
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var angleFloat64 = LinFloat64PolarAngle.CreateFromRadians(Math.PI / 6); // 30 degrees
        var angleGeneric = LinPolarAngle<double>.CreateFromRadians(scalarProcessor.ScalarFromValue(Math.PI / 6));
        const double magnitude = 2.0;
        var magnitudeScalar = scalarProcessor.ScalarFromValue(magnitude);

        // Act - Create phasors (scaled vectors)
        var float64Vector = angleFloat64.CreatePhasor(magnitude, 0, 1, _float64Processor);
        var genericVector = angleGeneric.CreatePhasor(magnitudeScalar, 0, 1, _genericProcessor);

        // Assert - Should be scaled by magnitude
        var cosValue = magnitude * Math.Cos(Math.PI / 6);
        var sinValue = magnitude * Math.Sin(Math.PI / 6);

        Assert.That(float64Vector.Scalar(0), Is.EqualTo(cosValue).Within(Tolerance),
            "Float64 e0 component should be magnitude*cos(π/6)");
        Assert.That(float64Vector.Scalar(1), Is.EqualTo(sinValue).Within(Tolerance),
            "Float64 e1 component should be magnitude*sin(π/6)");

        Assert.That(genericVector.Scalar(0).ScalarValue, Is.EqualTo(cosValue).Within(Tolerance),
            "Generic e0 component should be magnitude*cos(π/6)");
        Assert.That(genericVector.Scalar(1).ScalarValue, Is.EqualTo(sinValue).Within(Tolerance),
            "Generic e1 component should be magnitude*sin(π/6)");

        Assert.That(genericVector.Scalar(0).ScalarValue, Is.EqualTo(float64Vector.Scalar(0)).Within(Tolerance));
        Assert.That(genericVector.Scalar(1).ScalarValue, Is.EqualTo(float64Vector.Scalar(1)).Within(Tolerance));

        // Should have norm = magnitude
        var float64Norm = float64Vector.ENorm().ScalarValue;
        var genericNorm = genericVector.ENorm().ScalarValue;

        Debug.Assert(Math.Abs(float64Norm - magnitude) < Tolerance, "Float64 vector should have magnitude 2.0");
        Debug.Assert(Math.Abs(genericNorm - magnitude) < Tolerance, "Generic vector should have magnitude 2.0");
    }


    [Test]
    public void LinAngle_CreateUnitVector_GenericDouble_ShouldMatchFloat64()
    {
        // Arrange
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var angleGeneric = LinPolarAngle<double>.CreateFromRadians(scalarProcessor.ScalarFromValue(Math.PI / 4));

        // Act
        var genericVector = angleGeneric.CreateUnitVector(0, 1, _genericProcessor);

        // Assert - Should match Float64 result
        var cosValue = Math.Cos(Math.PI / 4);
        var sinValue = Math.Sin(Math.PI / 4);

        Assert.That(genericVector.Scalar(0).ScalarValue, Is.EqualTo(cosValue).Within(Tolerance));
        Assert.That(genericVector.Scalar(1).ScalarValue, Is.EqualTo(sinValue).Within(Tolerance));

        var genericNorm = genericVector.ENorm().ScalarValue;
        Debug.Assert(Math.Abs(genericNorm - 1.0) < Tolerance, "Generic<double> vector should be unit length");
    }


    [Test]
    public void LinAngle_CreatePhasor_GenericDouble_ShouldMatchFloat64()
    {
        // Arrange
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var angleGeneric = LinPolarAngle<double>.CreateFromRadians(scalarProcessor.ScalarFromValue(Math.PI / 3));
        var magnitude = scalarProcessor.ScalarFromValue(3.5);

        // Act
        var genericVector = angleGeneric.CreatePhasor(magnitude, 1, 2, _genericProcessor);

        // Assert
        var cosValue = 3.5 * Math.Cos(Math.PI / 3);
        var sinValue = 3.5 * Math.Sin(Math.PI / 3);

        Assert.That(genericVector.Scalar(1).ScalarValue, Is.EqualTo(cosValue).Within(Tolerance));
        Assert.That(genericVector.Scalar(2).ScalarValue, Is.EqualTo(sinValue).Within(Tolerance));

        var genericNorm = genericVector.ENorm().ScalarValue;
        Debug.Assert(Math.Abs(genericNorm - 3.5) < Tolerance, "Generic<double> vector should have magnitude 3.5");
    }


    [Test]
    public void CreateUnitVector_ZeroAngle_ShouldProduceE1Component()
    {
        // Arrange
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var angleFloat64 = LinFloat64PolarAngle.CreateFromRadians(0); // 0 degrees
        var angleGeneric = LinPolarAngle<double>.CreateFromRadians(scalarProcessor.ScalarFromValue(0));

        // Act
        var float64Vector = angleFloat64.CreateUnitVector(0, 1, _float64Processor);
        var genericVector = angleGeneric.CreateUnitVector(0, 1, _genericProcessor);

        // Assert - cos(0)=1, sin(0)=0
        Assert.That(float64Vector.Scalar(0), Is.EqualTo(1.0).Within(Tolerance), "cos(0) = 1");
        Assert.That(float64Vector.Scalar(1), Is.EqualTo(0.0).Within(Tolerance), "sin(0) = 0");

        Assert.That(genericVector.Scalar(0).ScalarValue, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(genericVector.Scalar(1).ScalarValue, Is.EqualTo(0.0).Within(Tolerance));

        Assert.That(genericVector.Scalar(0).ScalarValue, Is.EqualTo(float64Vector.Scalar(0)).Within(Tolerance));
        Assert.That(genericVector.Scalar(1).ScalarValue, Is.EqualTo(float64Vector.Scalar(1)).Within(Tolerance));
    }


    [Test]
    public void CreateUnitVector_90Degrees_ShouldProduceE2Component()
    {
        // Arrange
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var angleFloat64 = LinFloat64PolarAngle.CreateFromRadians(Math.PI / 2); // 90 degrees
        var angleGeneric = LinPolarAngle<double>.CreateFromRadians(scalarProcessor.ScalarFromValue(Math.PI / 2));

        // Act
        var float64Vector = angleFloat64.CreateUnitVector(0, 1, _float64Processor);
        var genericVector = angleGeneric.CreateUnitVector(0, 1, _genericProcessor);

        // Assert - cos(π/2)≈0, sin(π/2)=1
        Assert.That(float64Vector.Scalar(0), Is.EqualTo(0.0).Within(Tolerance), "cos(π/2) ≈ 0");
        Assert.That(float64Vector.Scalar(1), Is.EqualTo(1.0).Within(Tolerance), "sin(π/2) = 1");

        Assert.That(genericVector.Scalar(0).ScalarValue, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(genericVector.Scalar(1).ScalarValue, Is.EqualTo(1.0).Within(Tolerance));

        Assert.That(genericVector.Scalar(0).ScalarValue, Is.EqualTo(float64Vector.Scalar(0)).Within(Tolerance));
        Assert.That(genericVector.Scalar(1).ScalarValue, Is.EqualTo(float64Vector.Scalar(1)).Within(Tolerance));
    }
}
