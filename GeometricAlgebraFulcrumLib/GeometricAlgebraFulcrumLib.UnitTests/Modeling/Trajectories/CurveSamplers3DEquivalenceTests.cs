using System;
using System.Diagnostics;
using System.Linq;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Samplers;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Samplers;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public sealed class CurveSamplers3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private readonly ScalarProcessorOfFloat64 _sp = ScalarProcessorOfFloat64.Instance;

    #region ConstantCurveSampler3D Tests

    [Test]
    public void TestConstantCurveSampler_Properties()
    {
        var point = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var tangent = LinFloat64Vector3D.Create(1.0, 0.0, 0.0);

        var float64Sampler = new ConstantCurveSampler3D(point, tangent);
        var genericSampler = new ConstantCurveSampler3D<double>(
            LinVector3D<double>.Create(_sp.ScalarFromValue(1.0), _sp.ScalarFromValue(2.0), _sp.ScalarFromValue(3.0)),
            LinVector3D<double>.Create(_sp.One, _sp.Zero, _sp.Zero)
        );

        Assert.That(genericSampler.Count, Is.EqualTo(float64Sampler.Count));
        Assert.That(genericSampler.IsPeriodic, Is.EqualTo(float64Sampler.IsPeriodic));

        Debug.Assert(genericSampler.Count == float64Sampler.Count);
    }

    [Test]
    public void TestConstantCurveSampler_GetPoints()
    {
        var point = LinFloat64Vector3D.Create(1.5, 2.5, 3.5);
        var tangent = LinFloat64Vector3D.Create(0.0, 1.0, 0.0);

        var float64Sampler = new ConstantCurveSampler3D(point, tangent);
        var genericSampler = new ConstantCurveSampler3D<double>(
            LinVector3D<double>.Create(_sp.ScalarFromValue(1.5), _sp.ScalarFromValue(2.5), _sp.ScalarFromValue(3.5)),
            LinVector3D<double>.Create(_sp.Zero, _sp.One, _sp.Zero)
        );

        var float64Points = float64Sampler.GetPoints().ToArray();
        var genericPoints = genericSampler.GetPoints().ToArray();

        Assert.That(genericPoints.Length, Is.EqualTo(float64Points.Length));

        for (var i = 0; i < float64Points.Length; i++)
        {
            Assert.That(genericPoints[i].X.ScalarValue, Is.EqualTo(float64Points[i].X).Within(Tolerance));
            Assert.That(genericPoints[i].Y.ScalarValue, Is.EqualTo(float64Points[i].Y).Within(Tolerance));
            Assert.That(genericPoints[i].Z.ScalarValue, Is.EqualTo(float64Points[i].Z).Within(Tolerance));
        }

        Debug.Assert(genericPoints.Length == float64Points.Length);
    }

    #endregion

    #region UniformParameterCurveSampler3D Tests

    [Test]
    public void TestUniformParameterSampler_Properties()
    {
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(1, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            _sp,
            1,
            LinVector3D<double>.Create(_sp.One, _sp.ScalarFromValue(2.0), _sp.ScalarFromValue(3.0))
        );

        const int sampleCount = 10;
        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Sampler = new UniformParameterCurveSampler3D(float64Curve, float64Range, sampleCount);
        var genericSampler = new UniformParameterCurveSampler3D<double>(genericCurve, genericRange, sampleCount);

        Assert.That(genericSampler.Count, Is.EqualTo(float64Sampler.Count));
        Assert.That(genericSampler.IsPeriodic, Is.EqualTo(float64Sampler.IsPeriodic));

        Debug.Assert(genericSampler.Count == float64Sampler.Count);
    }

    [Test]
    public void TestUniformParameterSampler_ParameterValues()
    {
        var magnitude = LinFloat64Vector3D.Create(1.0, 1.0, 1.0);
        var float64Curve = Float64SimpleHarmonicPath3D.PeriodicSymmetric(1, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.PeriodicSymmetric(_sp, 1,
            LinVector3D<double>.Create(_sp.One, _sp.One, _sp.One));

        const int sampleCount = 20;
        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Sampler = new UniformParameterCurveSampler3D(float64Curve, float64Range, sampleCount);
        var genericSampler = new UniformParameterCurveSampler3D<double>(genericCurve, genericRange, sampleCount);

        var float64Params = float64Sampler.GetParameterValues().ToArray();
        var genericParams = genericSampler.GetParameterValues().ToArray();

        Assert.That(genericParams.Length, Is.EqualTo(float64Params.Length));

        for (var i = 0; i < float64Params.Length; i++)
        {
            Assert.That(genericParams[i].ScalarValue, Is.EqualTo(float64Params[i]).Within(Tolerance),
                $"Mismatch at parameter index {i}");
        }

        Debug.Assert(genericParams.Length == float64Params.Length);
    }

    [Test]
    public void TestUniformParameterSampler_Points()
    {
        var magnitude = LinFloat64Vector3D.Create(2.0, 1.5, 1.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(2, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 2,
            LinVector3D<double>.Create(_sp.ScalarFromValue(2.0), _sp.ScalarFromValue(1.5), _sp.One));

        const int sampleCount = 15;
        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Sampler = new UniformParameterCurveSampler3D(float64Curve, float64Range, sampleCount);
        var genericSampler = new UniformParameterCurveSampler3D<double>(genericCurve, genericRange, sampleCount);

        var float64Points = float64Sampler.GetPoints().ToArray();
        var genericPoints = genericSampler.GetPoints().ToArray();

        Assert.That(genericPoints.Length, Is.EqualTo(float64Points.Length));

        for (var i = 0; i < float64Points.Length; i++)
        {
            Assert.That(genericPoints[i].X.ScalarValue, Is.EqualTo(float64Points[i].X).Within(Tolerance),
                $"Mismatch at point {i}, X component");
            Assert.That(genericPoints[i].Y.ScalarValue, Is.EqualTo(float64Points[i].Y).Within(Tolerance),
                $"Mismatch at point {i}, Y component");
            Assert.That(genericPoints[i].Z.ScalarValue, Is.EqualTo(float64Points[i].Z).Within(Tolerance),
                $"Mismatch at point {i}, Z component");
        }

        Debug.Assert(genericPoints.Length == float64Points.Length);
    }

    #endregion

    #region ParameterListCurveSampler3D Tests

    [Test]
    public void TestParameterListSampler_Properties()
    {
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(1, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 1,
            LinVector3D<double>.Create(_sp.One, _sp.ScalarFromValue(2.0), _sp.ScalarFromValue(3.0)));

        var parameterList = new[] { -Math.PI, -Math.PI / 2, 0.0, Math.PI / 2, Math.PI };
        var float64Sampler = new ParameterListCurveSampler3D(float64Curve, parameterList);
        var genericSampler = new ParameterListCurveSampler3D<double>(genericCurve,
            parameterList.Select(p => _sp.ScalarFromValue(p)).ToArray());

        Assert.That(genericSampler.Count, Is.EqualTo(float64Sampler.Count));
        Assert.That(genericSampler.Count, Is.EqualTo(parameterList.Length));

        Debug.Assert(genericSampler.Count == float64Sampler.Count);
    }

    [Test]
    public void TestParameterListSampler_CustomParameters()
    {
        var magnitude = LinFloat64Vector3D.Create(1.0, 1.0, 1.0);
        var float64Curve = Float64SimpleHarmonicPath3D.PeriodicSymmetric(1, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.PeriodicSymmetric(_sp, 1,
            LinVector3D<double>.Create(_sp.One, _sp.One, _sp.One));

        var parameterList = new[] { -2.5, -1.0, -0.5, 0.0, 0.5, 1.0, 2.5 };
        var float64Sampler = new ParameterListCurveSampler3D(float64Curve, parameterList);
        var genericSampler = new ParameterListCurveSampler3D<double>(genericCurve,
            parameterList.Select(p => _sp.ScalarFromValue(p)).ToArray());

        var float64Points = float64Sampler.GetPoints().ToArray();
        var genericPoints = genericSampler.GetPoints().ToArray();

        Assert.That(genericPoints.Length, Is.EqualTo(float64Points.Length));

        for (var i = 0; i < float64Points.Length; i++)
        {
            Assert.That(genericPoints[i].X.ScalarValue, Is.EqualTo(float64Points[i].X).Within(Tolerance),
                $"Mismatch at parameter {parameterList[i]}, X component");
            Assert.That(genericPoints[i].Y.ScalarValue, Is.EqualTo(float64Points[i].Y).Within(Tolerance),
                $"Mismatch at parameter {parameterList[i]}, Y component");
            Assert.That(genericPoints[i].Z.ScalarValue, Is.EqualTo(float64Points[i].Z).Within(Tolerance),
                $"Mismatch at parameter {parameterList[i]}, Z component");
        }

        Debug.Assert(genericPoints.Length == float64Points.Length);
    }

    #endregion

    #region UniformLengthCurveSampler3D Tests

    [Test]
    public void TestUniformLengthSampler_Properties()
    {
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(1, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 1,
            LinVector3D<double>.Create(_sp.One, _sp.ScalarFromValue(2.0), _sp.ScalarFromValue(3.0)));

        const int sampleCount = 25;
        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Sampler = new UniformLengthCurveSampler3D(float64Curve, float64Range, sampleCount);
        var genericSampler = new UniformLengthCurveSampler3D<double>(genericCurve, genericRange, sampleCount);

        Assert.That(genericSampler.Count, Is.EqualTo(float64Sampler.Count));
        Assert.That(genericSampler.IsPeriodic, Is.EqualTo(float64Sampler.IsPeriodic));

        Debug.Assert(genericSampler.Count == float64Sampler.Count);
    }

    [Test]
    public void TestUniformLengthSampler_ArcLengthDistribution()
    {
        var magnitude = LinFloat64Vector3D.Create(2.0, 1.5, 1.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(2, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 2,
            LinVector3D<double>.Create(_sp.ScalarFromValue(2.0), _sp.ScalarFromValue(1.5), _sp.One));

        const int sampleCount = 30;
        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Sampler = new UniformLengthCurveSampler3D(float64Curve, float64Range, sampleCount);
        var genericSampler = new UniformLengthCurveSampler3D<double>(genericCurve, genericRange, sampleCount);

        var float64Points = float64Sampler.GetPoints().ToArray();
        var genericPoints = genericSampler.GetPoints().ToArray();

        Assert.That(genericPoints.Length, Is.EqualTo(float64Points.Length));

        // Verify points match
        for (var i = 0; i < float64Points.Length; i++)
        {
            Assert.That(genericPoints[i].X.ScalarValue, Is.EqualTo(float64Points[i].X).Within(Tolerance),
                $"Mismatch at point {i}, X component");
            Assert.That(genericPoints[i].Y.ScalarValue, Is.EqualTo(float64Points[i].Y).Within(Tolerance),
                $"Mismatch at point {i}, Y component");
            Assert.That(genericPoints[i].Z.ScalarValue, Is.EqualTo(float64Points[i].Z).Within(Tolerance),
                $"Mismatch at point {i}, Z component");
        }

        // Verify uniform arc-length spacing (distances between consecutive points should be approximately equal)
        if (genericPoints.Length > 2)
        {
            var distances = new double[genericPoints.Length - 1];
            for (var i = 0; i < distances.Length; i++)
            {
                var dx = genericPoints[i + 1].X.ScalarValue - genericPoints[i].X.ScalarValue;
                var dy = genericPoints[i + 1].Y.ScalarValue - genericPoints[i].Y.ScalarValue;
                var dz = genericPoints[i + 1].Z.ScalarValue - genericPoints[i].Z.ScalarValue;
                distances[i] = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            }

            var avgDistance = distances.Average();
            foreach (var distance in distances)
            {
                // Arc-length sampling should produce similar distances (within 20% tolerance for approximate methods)
                Assert.That(distance, Is.EqualTo(avgDistance).Within(0.2 * avgDistance),
                    "Arc-length distances should be approximately uniform");
            }
        }

        Debug.Assert(genericPoints.Length == float64Points.Length);
    }

    #endregion

    #region Sampler Validation Tests

    [Test]
    public void TestAllSamplers_IsValid()
    {
        var magnitude = LinVector3D<double>.Create(_sp.One, _sp.One, _sp.One);
        var curve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 1, magnitude);
        var range = ScalarRange<double>.SymmetricPi(_sp);

        var constantSampler = new ConstantCurveSampler3D<double>(
            LinVector3D<double>.Create(_sp.One, _sp.One, _sp.One),
            LinVector3D<double>.Create(_sp.One, _sp.Zero, _sp.Zero)
        );
        var uniformParamSampler = new UniformParameterCurveSampler3D<double>(curve, range, 10);
        var paramListSampler = new ParameterListCurveSampler3D<double>(curve,
            new[] { _sp.Zero, _sp.One, _sp.ScalarFromValue(2.0) });
        var uniformLengthSampler = new UniformLengthCurveSampler3D<double>(curve, range, 15);

        Assert.That(constantSampler.IsValid(), Is.True);
        Assert.That(uniformParamSampler.IsValid(), Is.True);
        Assert.That(paramListSampler.IsValid(), Is.True);
        Assert.That(uniformLengthSampler.IsValid(), Is.True);

        Debug.Assert(constantSampler.IsValid());
    }

    [Test]
    public void TestSamplers_GetFrames()
    {
        var magnitude = LinFloat64Vector3D.Create(1.0, 1.0, 1.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(1, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 1,
            LinVector3D<double>.Create(_sp.One, _sp.One, _sp.One));

        const int sampleCount = 10;
        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Sampler = new UniformParameterCurveSampler3D(float64Curve, float64Range, sampleCount);
        var genericSampler = new UniformParameterCurveSampler3D<double>(genericCurve, genericRange, sampleCount);

        var float64Frames = float64Sampler.GetFrames().ToArray();
        var genericFrames = genericSampler.GetFrames().ToArray();

        Assert.That(genericFrames.Length, Is.EqualTo(float64Frames.Length));

        for (var i = 0; i < float64Frames.Length; i++)
        {
            // Test frame point
            Assert.That(genericFrames[i].Point.X.ScalarValue, Is.EqualTo(float64Frames[i].Point.X).Within(Tolerance));
            Assert.That(genericFrames[i].Point.Y.ScalarValue, Is.EqualTo(float64Frames[i].Point.Y).Within(Tolerance));
            Assert.That(genericFrames[i].Point.Z.ScalarValue, Is.EqualTo(float64Frames[i].Point.Z).Within(Tolerance));

            // Test frame tangent
            Assert.That(genericFrames[i].Tangent.X.ScalarValue, Is.EqualTo(float64Frames[i].Tangent.X).Within(Tolerance));
            Assert.That(genericFrames[i].Tangent.Y.ScalarValue, Is.EqualTo(float64Frames[i].Tangent.Y).Within(Tolerance));
            Assert.That(genericFrames[i].Tangent.Z.ScalarValue, Is.EqualTo(float64Frames[i].Tangent.Z).Within(Tolerance));
        }

        Debug.Assert(genericFrames.Length == float64Frames.Length);
    }

    #endregion
}
