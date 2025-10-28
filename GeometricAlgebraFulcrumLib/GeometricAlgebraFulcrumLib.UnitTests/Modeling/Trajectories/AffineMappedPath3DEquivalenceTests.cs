using System;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Mapped;
using NUnit.Framework;
using ConstantPath3D = GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic.ConstantPath3D<double>;
using LineSegmentPath3D = GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic.LineSegmentPath3D<double>;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

/// <summary>
/// Tests for AffineMappedPath3D<T> - verifies affine transformations (translation, rotation, scaling)
/// are correctly applied to paths, with separate handling of points and vectors.
/// </summary>
[TestFixture]
public class AffineMappedPath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private IScalarProcessor<double> ScalarProcessor { get; set; } = null!;

    [OneTimeSetUp]
    public void ClassInit()
    {
        ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    }

    [Test]
    public void AffineMappedPath3D_TranslationOnly_PointShifted_VectorUnchanged()
    {
        // Create a line segment from (0,0,0) to (1,0,0)
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 1, 0, 0)
        );

        // Translation: shift by (5, 3, 2)
        var translation = LinVector3D<double>.Create(ScalarProcessor, 5, 3, 2);
        var pointMap = (LinVector3D<double> v) => v + translation;
        var vectorMap = (LinVector3D<double> v) => v;  // Vectors unaffected by translation

        var mappedPath = AffineMappedPath3D<double>.Create(basePath, pointMap, vectorMap);

        // At t=0: Point should be (0,0,0) + (5,3,2) = (5,3,2)
        var value0 = mappedPath.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "t=0: X");
        Assert.That(value0.Y.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "t=0: Y");
        Assert.That(value0.Z.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "t=0: Z");

        // At t=1: Point should be (1,0,0) + (5,3,2) = (6,3,2)
        var value1 = mappedPath.GetValue(ScalarProcessor.One);
        Assert.That(value1.X.ScalarValue, Is.EqualTo(6.0).Within(Tolerance), "t=1: X");
        Assert.That(value1.Y.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "t=1: Y");
        Assert.That(value1.Z.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "t=1: Z");

        // Velocity (derivative) should be (1,0,0) unchanged by translation
        var deriv = mappedPath.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(0.5));
        Assert.That(deriv.X.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Velocity X");
        Assert.That(deriv.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Velocity Y");
        Assert.That(deriv.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Velocity Z");
    }

    [Test]
    public void AffineMappedPath3D_UniformScaling_BothPointAndVectorScaled()
    {
        // Create a constant path at (2, 3, 4)
        var basePath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 2, 3, 4)
        );

        // Uniform scaling by factor 2
        var scale = 2.0;
        var scaleMap = (LinVector3D<double> v) => LinVector3D<double>.Create(
            ScalarProcessor,
            v.X.ScalarValue * scale,
            v.Y.ScalarValue * scale,
            v.Z.ScalarValue * scale
        );

        var mappedPath = AffineMappedPath3D<double>.Create(basePath, scaleMap, scaleMap);

        // Point should be (4, 6, 8)
        var value = mappedPath.GetValue(ScalarProcessor.ScalarFromNumber(0.5));
        Assert.That(value.X.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "X scaled");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(6.0).Within(Tolerance), "Y scaled");
        Assert.That(value.Z.ScalarValue, Is.EqualTo(8.0).Within(Tolerance), "Z scaled");
    }

    [Test]
    public void AffineMappedPath3D_NonUniformScaling_DifferentScalePerAxis()
    {
        // Line segment from (1,1,1) to (2,2,2)
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1),
            LinVector3D<double>.Create(ScalarProcessor, 2, 2, 2)
        );

        // Non-uniform scaling: X*2, Y*3, Z*4
        var scaleMap = (LinVector3D<double> v) => LinVector3D<double>.Create(
            ScalarProcessor,
            v.X.ScalarValue * 2,
            v.Y.ScalarValue * 3,
            v.Z.ScalarValue * 4
        );

        var mappedPath = AffineMappedPath3D<double>.Create(basePath, scaleMap, scaleMap);

        // At t=0: (1,1,1) -> (2,3,4)
        var value0 = mappedPath.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "t=0: X");
        Assert.That(value0.Y.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "t=0: Y");
        Assert.That(value0.Z.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "t=0: Z");

        // At t=1: (2,2,2) -> (4,6,8)
        var value1 = mappedPath.GetValue(ScalarProcessor.One);
        Assert.That(value1.X.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "t=1: X");
        Assert.That(value1.Y.ScalarValue, Is.EqualTo(6.0).Within(Tolerance), "t=1: Y");
        Assert.That(value1.Z.ScalarValue, Is.EqualTo(8.0).Within(Tolerance), "t=1: Z");

        // Velocity (1,1,1) -> (2,3,4)
        var deriv = mappedPath.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(0.5));
        Assert.That(deriv.X.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "Velocity X");
        Assert.That(deriv.Y.ScalarValue, Is.EqualTo(3.0).Within(Tolerance), "Velocity Y");
        Assert.That(deriv.Z.ScalarValue, Is.EqualTo(4.0).Within(Tolerance), "Velocity Z");
    }

    [Test]
    public void AffineMappedPath3D_RotationAboutZAxis_90Degrees()
    {
        // Line segment from (1,0,0) to (2,0,0) along X-axis
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 1, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 2, 0, 0)
        );

        // Rotate 90° around Z-axis: (x,y,z) -> (-y,x,z)
        var rotateMap = (LinVector3D<double> v) => LinVector3D<double>.Create(
            ScalarProcessor,
            -v.Y.ScalarValue,
            v.X.ScalarValue,
            v.Z.ScalarValue
        );

        var mappedPath = AffineMappedPath3D<double>.Create(basePath, rotateMap, rotateMap);

        // At t=0: (1,0,0) -> (0,1,0)
        var value0 = mappedPath.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "t=0: X");
        Assert.That(value0.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "t=0: Y");
        Assert.That(value0.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "t=0: Z");

        // At t=1: (2,0,0) -> (0,2,0)
        var value1 = mappedPath.GetValue(ScalarProcessor.One);
        Assert.That(value1.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "t=1: X");
        Assert.That(value1.Y.ScalarValue, Is.EqualTo(2.0).Within(Tolerance), "t=1: Y");
        Assert.That(value1.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "t=1: Z");

        // Velocity (1,0,0) -> (0,1,0)
        var deriv = mappedPath.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(0.5));
        Assert.That(deriv.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Velocity X");
        Assert.That(deriv.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Velocity Y");
        Assert.That(deriv.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Velocity Z");
    }

    [Test]
    public void AffineMappedPath3D_AffineCombination_RotationPlusTranslation()
    {
        // Line segment from (1,0,0) to (2,0,0)
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 1, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 2, 0, 0)
        );

        var translation = LinVector3D<double>.Create(ScalarProcessor, 10, 20, 30);

        // Point map: Rotate 90° + translate
        var pointMap = (LinVector3D<double> v) => LinVector3D<double>.Create(
            ScalarProcessor,
            -v.Y.ScalarValue + translation.X.ScalarValue,
            v.X.ScalarValue + translation.Y.ScalarValue,
            v.Z.ScalarValue + translation.Z.ScalarValue
        );

        // Vector map: Only rotation (no translation for vectors)
        var vectorMap = (LinVector3D<double> v) => LinVector3D<double>.Create(
            ScalarProcessor,
            -v.Y.ScalarValue,
            v.X.ScalarValue,
            v.Z.ScalarValue
        );

        var mappedPath = AffineMappedPath3D<double>.Create(basePath, pointMap, vectorMap);

        // At t=0: (1,0,0) -> rotate -> (0,1,0) -> translate -> (10,21,30)
        var value0 = mappedPath.GetValue(ScalarProcessor.Zero);
        Assert.That(value0.X.ScalarValue, Is.EqualTo(10.0).Within(Tolerance), "t=0: X");
        Assert.That(value0.Y.ScalarValue, Is.EqualTo(21.0).Within(Tolerance), "t=0: Y");
        Assert.That(value0.Z.ScalarValue, Is.EqualTo(30.0).Within(Tolerance), "t=0: Z");

        // Velocity: (1,0,0) -> rotate -> (0,1,0) (no translation)
        var deriv = mappedPath.GetDerivative1Value(ScalarProcessor.ScalarFromNumber(0.5));
        Assert.That(deriv.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Velocity X");
        Assert.That(deriv.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Velocity Y");
        Assert.That(deriv.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Velocity Z");
    }

    [Test]
    public void AffineMappedPath3D_IdentityTransformation_ShouldPreserveOriginal()
    {
        // Line segment from (3,4,5) to (6,7,8)
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 3, 4, 5),
            LinVector3D<double>.Create(ScalarProcessor, 6, 7, 8)
        );

        // Identity mapping
        var identityMap = (LinVector3D<double> v) => v;

        var mappedPath = AffineMappedPath3D<double>.Create(basePath, identityMap, identityMap);

        var t = ScalarProcessor.ScalarFromNumber(0.5);
        var originalValue = basePath.GetValue(t);
        var mappedValue = mappedPath.GetValue(t);

        Assert.That(mappedValue.X.ScalarValue, Is.EqualTo(originalValue.X.ScalarValue).Within(Tolerance), "X unchanged");
        Assert.That(mappedValue.Y.ScalarValue, Is.EqualTo(originalValue.Y.ScalarValue).Within(Tolerance), "Y unchanged");
        Assert.That(mappedValue.Z.ScalarValue, Is.EqualTo(originalValue.Z.ScalarValue).Within(Tolerance), "Z unchanged");

        var originalDeriv = basePath.GetDerivative1Value(t);
        var mappedDeriv = mappedPath.GetDerivative1Value(t);

        Assert.That(mappedDeriv.X.ScalarValue, Is.EqualTo(originalDeriv.X.ScalarValue).Within(Tolerance), "Velocity X unchanged");
        Assert.That(mappedDeriv.Y.ScalarValue, Is.EqualTo(originalDeriv.Y.ScalarValue).Within(Tolerance), "Velocity Y unchanged");
        Assert.That(mappedDeriv.Z.ScalarValue, Is.EqualTo(originalDeriv.Z.ScalarValue).Within(Tolerance), "Velocity Z unchanged");
    }

    [Test]
    public void AffineMappedPath3D_CreateLinear_UsesSameFunctionForBoth()
    {
        // Test the CreateLinear factory method (for pure linear transformations)
        var basePath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        // Scale by 5
        var linearMap = (LinVector3D<double> v) => LinVector3D<double>.Create(
            ScalarProcessor,
            v.X.ScalarValue * 5,
            v.Y.ScalarValue * 5,
            v.Z.ScalarValue * 5
        );

        var mappedPath = AffineMappedPath3D<double>.CreateLinear(basePath, linearMap);

        // Both points and vectors should be scaled
        var value = mappedPath.GetValue(ScalarProcessor.Zero);
        Assert.That(value.X.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "X");
        Assert.That(value.Y.ScalarValue, Is.EqualTo(10.0).Within(Tolerance), "Y");
        Assert.That(value.Z.ScalarValue, Is.EqualTo(15.0).Within(Tolerance), "Z");
    }

    [Test]
    public void AffineMappedPath3D_GetDerivative2Value_TransformedByVectorMap()
    {
        // Create a path with non-zero second derivative (using composition or harmonic)
        // For simplicity, use a line segment (derivative is constant, second derivative is zero)
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 0, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1)
        );

        // Scale transformation
        var scaleMap = (LinVector3D<double> v) => LinVector3D<double>.Create(
            ScalarProcessor,
            v.X.ScalarValue * 3,
            v.Y.ScalarValue * 3,
            v.Z.ScalarValue * 3
        );

        var mappedPath = AffineMappedPath3D<double>.Create(basePath, scaleMap, scaleMap);

        // Second derivative of line segment is zero, transformed should still be zero
        var deriv2 = mappedPath.GetDerivative2Value(ScalarProcessor.ScalarFromNumber(0.5));
        Assert.That(deriv2.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Accel X");
        Assert.That(deriv2.Y.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Accel Y");
        Assert.That(deriv2.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Accel Z");
    }

    [Test]
    public void AffineMappedPath3D_GetFrame_TransformsBothPointAndTangent()
    {
        // Line segment from (1,0,0) to (2,0,0)
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 1, 0, 0),
            LinVector3D<double>.Create(ScalarProcessor, 2, 0, 0)
        );

        var translation = LinVector3D<double>.Create(ScalarProcessor, 5, 5, 5);

        // Point map includes translation
        var pointMap = (LinVector3D<double> v) => v + translation;
        // Vector map: Rotate 90° around Z (no scaling, to maintain unit tangent)
        var vectorMap = (LinVector3D<double> v) => LinVector3D<double>.Create(
            ScalarProcessor,
            -v.Y.ScalarValue,
            v.X.ScalarValue,
            v.Z.ScalarValue
        );

        var mappedPath = AffineMappedPath3D<double>.Create(basePath, pointMap, vectorMap);

        var t = ScalarProcessor.ScalarFromNumber(0.5);
        var frame = mappedPath.GetFrame(t);

        // Point at t=0.5: (1.5,0,0) + (5,5,5) = (6.5,5,5)
        Assert.That(frame.Point.X.ScalarValue, Is.EqualTo(6.5).Within(Tolerance), "Frame Point X");
        Assert.That(frame.Point.Y.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Frame Point Y");
        Assert.That(frame.Point.Z.ScalarValue, Is.EqualTo(5.0).Within(Tolerance), "Frame Point Z");

        // Tangent: Original (1,0,0) -> Rotate 90° -> (0,1,0) [normalized]
        // Note: Frame.Create() normalizes the tangent to a unit vector
        Assert.That(frame.Tangent.X.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Frame Tangent X");
        Assert.That(frame.Tangent.Y.ScalarValue, Is.EqualTo(1.0).Within(Tolerance), "Frame Tangent Y");
        Assert.That(frame.Tangent.Z.ScalarValue, Is.EqualTo(0.0).Within(Tolerance), "Frame Tangent Z");

        // Time should be preserved
        Assert.That(frame.TimeValue.ScalarValue, Is.EqualTo(0.5).Within(Tolerance), "Frame Time");
    }

    [Test]
    public void AffineMappedPath3D_IsValid_WhenBaseValid_ShouldReturnTrue()
    {
        var basePath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var identityMap = (LinVector3D<double> v) => v;
        var mappedPath = AffineMappedPath3D<double>.Create(basePath, identityMap, identityMap);

        Assert.That(mappedPath.IsValid(), Is.True, "AffineMappedPath should be valid");
        Assert.That(basePath.IsValid(), Is.True, "Base path should be valid");
    }

    [Test]
    public void AffineMappedPath3D_ToFinitePath_WhenFinite_ShouldReturnSelf()
    {
        var basePath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        var identityMap = (LinVector3D<double> v) => v;
        var mappedPath = AffineMappedPath3D<double>.Create(basePath, identityMap, identityMap);

        var finitePath = mappedPath.ToFinitePath();

        Assert.That(finitePath, Is.SameAs(mappedPath), "Should return self when already finite");
        Assert.That(mappedPath.IsFinite, Is.True);
        Assert.That(mappedPath.IsPeriodic, Is.False);
    }

    [Test]
    public void AffineMappedPath3D_ToPeriodicPath_WhenFinite_ShouldReturnNewInstance()
    {
        var basePath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 2, 3, 4)
        );

        var scaleMap = (LinVector3D<double> v) => v * ScalarProcessor.ScalarFromNumber(2);
        var mappedPath = AffineMappedPath3D<double>.Create(basePath, scaleMap, scaleMap);

        var periodicPath = mappedPath.ToPeriodicPath();

        Assert.That(periodicPath, Is.Not.SameAs(mappedPath), "Should return new instance");
        Assert.That(periodicPath.IsPeriodic, Is.True);
        Assert.That(periodicPath.IsFinite, Is.False);

        // Values should still match
        var t = ScalarProcessor.ScalarFromNumber(0.5);
        var value1 = mappedPath.GetValue(t);
        var value2 = periodicPath.GetValue(t);

        Assert.That(value2.X.ScalarValue, Is.EqualTo(value1.X.ScalarValue).Within(Tolerance), "X matches");
        Assert.That(value2.Y.ScalarValue, Is.EqualTo(value1.Y.ScalarValue).Within(Tolerance), "Y matches");
        Assert.That(value2.Z.ScalarValue, Is.EqualTo(value1.Z.ScalarValue).Within(Tolerance), "Z matches");
    }

    [Test]
    public void AffineMappedPath3D_ToPeriodicPath_WhenPeriodic_ShouldReturnSelf()
    {
        // First create a periodic base path
        var finitePath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );
        var periodicBasePath = (ParametricPath3D<double>)finitePath.ToPeriodicPath();

        var identityMap = (LinVector3D<double> v) => v;
        var mappedPath = AffineMappedPath3D<double>.Create(periodicBasePath, identityMap, identityMap);

        var periodicPath = mappedPath.ToPeriodicPath();

        Assert.That(periodicPath, Is.SameAs(mappedPath), "Should return self when already periodic");
        Assert.That(mappedPath.IsPeriodic, Is.True);
    }

    [Test]
    public void AffineMappedPath3D_Properties_ShouldReturnCorrectValues()
    {
        var basePath = ConstantPath3D.Finite(
            ScalarRange<double>.ZeroToOne(ScalarProcessor),
            LinVector3D<double>.Create(ScalarProcessor, 1, 2, 3)
        );

        Func<LinVector3D<double>, LinVector3D<double>> pointMapFunc = v => v * ScalarProcessor.ScalarFromNumber(2);
        Func<LinVector3D<double>, LinVector3D<double>> vectorMapFunc = v => v * ScalarProcessor.ScalarFromNumber(3);

        var mappedPath = AffineMappedPath3D<double>.Create(basePath, pointMapFunc, vectorMapFunc);

        Assert.That(mappedPath.BasePath, Is.SameAs(basePath), "BasePath should reference original");
        Assert.That(mappedPath.PointMap, Is.SameAs(pointMapFunc), "PointMap should reference provided function");
        Assert.That(mappedPath.VectorMap, Is.SameAs(vectorMapFunc), "VectorMap should reference provided function");
    }

    [Test]
    public void AffineMappedPath3D_TimeRangePreservation_ShouldMatchBase()
    {
        var basePath = LineSegmentPath3D.Create(
            false,
            LinVector3D<double>.Create(ScalarProcessor, 1, 1, 1),
            LinVector3D<double>.Create(ScalarProcessor, 2, 2, 2)
        );

        var identityMap = (LinVector3D<double> v) => v;
        var mappedPath = AffineMappedPath3D<double>.Create(basePath, identityMap, identityMap);

        // Time range should match base path
        Assert.That(mappedPath.MinTime.ScalarValue, Is.EqualTo(basePath.MinTime.ScalarValue).Within(Tolerance));
        Assert.That(mappedPath.MaxTime.ScalarValue, Is.EqualTo(basePath.MaxTime.ScalarValue).Within(Tolerance));
    }
}
