using System;
using System.Diagnostics;
using System.Linq;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Adaptive;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Adaptive;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Tuples;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public sealed class AdaptivePath3DEquivalenceTests
{
    private const double Tolerance = 1e-12;
    private readonly ScalarProcessorOfFloat64 _sp = ScalarProcessorOfFloat64.Instance;

    #region AdaptivePath3DSamplingOptions Tests

    [Test]
    public void TestSamplingOptions_DefaultValues()
    {
        var float64Options = Float64AdaptivePath3DSamplingOptions.Default;
        var genericOptions = AdaptivePath3DSamplingOptions<double>.Default(_sp);

        Assert.That(genericOptions.MinLevelCount, Is.EqualTo(float64Options.MinLevelCount));
        Assert.That(genericOptions.MaxLevelCount, Is.EqualTo(float64Options.MaxLevelCount));
        Assert.That(genericOptions.MaxEdgeFramesAngle.ScalarValue, Is.EqualTo(float64Options.MaxEdgeFramesAngle).Within(Tolerance));

        Debug.Assert(genericOptions.MinLevelCount == float64Options.MinLevelCount);
    }

    [Test]
    public void TestSamplingOptions_CustomValues()
    {
        const int minLevel = 3;
        const int maxLevel = 8;
        const double maxAngle = 5.0;
        const double maxDistance = 0.05;
        const double maxParamDistance = 0.1;

        var float64Options = new Float64AdaptivePath3DSamplingOptions
        {
            MinLevelCount = minLevel,
            MaxLevelCount = maxLevel,
            MaxEdgeFramesAngle = maxAngle,
            MaxEdgeFramesDistance = maxDistance,
            MaxEdgeFramesParameterDistance = maxParamDistance
        };

        var genericOptions = new AdaptivePath3DSamplingOptions<double>(_sp)
        {
            MinLevelCount = minLevel,
            MaxLevelCount = maxLevel,
            MaxEdgeFramesAngle = _sp.ScalarFromValue(maxAngle),
            MaxEdgeFramesDistance = _sp.ScalarFromValue(maxDistance),
            MaxEdgeFramesParameterDistance = _sp.ScalarFromValue(maxParamDistance)
        };

        Assert.That(genericOptions.MinLevelCount, Is.EqualTo(float64Options.MinLevelCount));
        Assert.That(genericOptions.MaxLevelCount, Is.EqualTo(float64Options.MaxLevelCount));
        Assert.That(genericOptions.MaxEdgeFramesAngle.ScalarValue, Is.EqualTo(float64Options.MaxEdgeFramesAngle).Within(Tolerance));
        Assert.That(genericOptions.MaxEdgeFramesDistance.ScalarValue, Is.EqualTo(float64Options.MaxEdgeFramesDistance).Within(Tolerance));
        Assert.That(genericOptions.MaxEdgeFramesParameterDistance.ScalarValue, Is.EqualTo(float64Options.MaxEdgeFramesParameterDistance).Within(Tolerance));

        Debug.Assert(genericOptions.MaxLevelCount == float64Options.MaxLevelCount);
    }

    #endregion

    #region AdaptivePath3D Basic Tests

    [Test]
    public void TestAdaptivePath_Creation()
    {
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(1, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 1,
            LinVector3D<double>.Create(_sp.One, _sp.ScalarFromValue(2.0), _sp.ScalarFromValue(3.0)));

        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Options = Float64AdaptivePath3DSamplingOptions.Default;
        var genericOptions = AdaptivePath3DSamplingOptions<double>.Default(_sp);

        var float64Adaptive = float64Curve.CreateAdaptiveCurve3D(float64Range, float64Options);
        var genericAdaptive = genericCurve.CreateAdaptiveCurve3D(genericRange, genericOptions);

        Assert.That(genericAdaptive.Count, Is.GreaterThan(0));
        Assert.That(genericAdaptive.IsValid(), Is.True);
        Assert.That(float64Adaptive.IsValid(), Is.True);

        Debug.Assert(genericAdaptive.IsValid());
    }

    [Test]
    public void TestAdaptivePath_LeafCount()
    {
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(2, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 2,
            LinVector3D<double>.Create(_sp.One, _sp.ScalarFromValue(2.0), _sp.ScalarFromValue(3.0)));

        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Options = Float64AdaptivePath3DSamplingOptions.Default;
        var genericOptions = AdaptivePath3DSamplingOptions<double>.Default(_sp);

        var float64Adaptive = float64Curve.CreateAdaptiveCurve3D(float64Range, float64Options);
        var genericAdaptive = genericCurve.CreateAdaptiveCurve3D(genericRange, genericOptions);

        // Both should generate same number of leaves with same options
        Assert.That(genericAdaptive.LeafNodeCount, Is.EqualTo(float64Adaptive.LeafNodeCount));
        Assert.That(genericAdaptive.Count, Is.EqualTo(float64Adaptive.Count));

        Debug.Assert(genericAdaptive.LeafNodeCount == float64Adaptive.LeafNodeCount);
    }

    [Test]
    public void TestAdaptivePath_GetPoints()
    {
        var magnitude = LinFloat64Vector3D.Create(1.0, 1.0, 1.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(1, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 1,
            LinVector3D<double>.Create(_sp.One, _sp.One, _sp.One));

        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Options = Float64AdaptivePath3DSamplingOptions.Default;
        var genericOptions = AdaptivePath3DSamplingOptions<double>.Default(_sp);

        var float64Adaptive = float64Curve.CreateAdaptiveCurve3D(float64Range, float64Options);
        var genericAdaptive = genericCurve.CreateAdaptiveCurve3D(genericRange, genericOptions);

        var float64Points = float64Adaptive.GetPoints().ToArray();
        var genericPoints = genericAdaptive.GetPoints().ToArray();

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

    [Test]
    public void TestAdaptivePath_GetTangents()
    {
        var magnitude = LinFloat64Vector3D.Create(2.0, 1.5, 1.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(2, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 2,
            LinVector3D<double>.Create(_sp.ScalarFromValue(2.0), _sp.ScalarFromValue(1.5), _sp.One));

        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Options = Float64AdaptivePath3DSamplingOptions.Default;
        var genericOptions = AdaptivePath3DSamplingOptions<double>.Default(_sp);

        var float64Adaptive = float64Curve.CreateAdaptiveCurve3D(float64Range, float64Options);
        var genericAdaptive = genericCurve.CreateAdaptiveCurve3D(genericRange, genericOptions);

        var float64Tangents = float64Adaptive.GetTangents().ToArray();
        var genericTangents = genericAdaptive.GetTangents().ToArray();

        Assert.That(genericTangents.Length, Is.EqualTo(float64Tangents.Length));

        for (var i = 0; i < float64Tangents.Length; i++)
        {
            Assert.That(genericTangents[i].X.ScalarValue, Is.EqualTo(float64Tangents[i].X).Within(Tolerance),
                $"Mismatch at tangent {i}, X component");
            Assert.That(genericTangents[i].Y.ScalarValue, Is.EqualTo(float64Tangents[i].Y).Within(Tolerance),
                $"Mismatch at tangent {i}, Y component");
            Assert.That(genericTangents[i].Z.ScalarValue, Is.EqualTo(float64Tangents[i].Z).Within(Tolerance),
                $"Mismatch at tangent {i}, Z component");
        }

        Debug.Assert(genericTangents.Length == float64Tangents.Length);
    }

    #endregion

    #region AdaptivePath3D Arc-Length Tests

    [Test]
    public void TestAdaptivePath_ArcLength()
    {
        var magnitude = LinFloat64Vector3D.Create(1.0, 1.0, 1.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(1, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 1,
            LinVector3D<double>.Create(_sp.One, _sp.One, _sp.One));

        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Options = Float64AdaptivePath3DSamplingOptions.Default;
        var genericOptions = AdaptivePath3DSamplingOptions<double>.Default(_sp);

        var float64Adaptive = float64Curve.CreateAdaptiveCurve3D(float64Range, float64Options);
        var genericAdaptive = genericCurve.CreateAdaptiveCurve3D(genericRange, genericOptions);

        // Test total arc length
        var float64Length = float64Adaptive.Length;
        var genericLength = genericAdaptive.Length.ScalarValue;

        Assert.That(genericLength, Is.EqualTo(float64Length).Within(Tolerance * 10), // Higher tolerance for arc length
            "Total arc length should match");

        Debug.Assert(Math.Abs(genericLength - float64Length) < Tolerance * 10);
    }

    [Test]
    public void TestAdaptivePath_TimeToLength()
    {
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(1, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 1,
            LinVector3D<double>.Create(_sp.One, _sp.ScalarFromValue(2.0), _sp.ScalarFromValue(3.0)));

        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Options = Float64AdaptivePath3DSamplingOptions.Default;
        var genericOptions = AdaptivePath3DSamplingOptions<double>.Default(_sp);

        var float64Adaptive = float64Curve.CreateAdaptiveCurve3D(float64Range, float64Options);
        var genericAdaptive = genericCurve.CreateAdaptiveCurve3D(genericRange, genericOptions);

        var testTimes = new[] { -Math.PI, -Math.PI / 2, 0.0, Math.PI / 2, Math.PI };

        foreach (var t in testTimes)
        {
            var float64Length = float64Adaptive.TimeToLength(t);
            var genericLength = genericAdaptive.TimeToLength(_sp.ScalarFromValue(t));

            Assert.That(genericLength.ScalarValue, Is.EqualTo(float64Length).Within(Tolerance * 10),
                $"Arc length at t={t} should match");
        }

        Debug.Assert(true);
    }

    [Test]
    public void TestAdaptivePath_LengthToTime()
    {
        var magnitude = LinFloat64Vector3D.Create(1.0, 1.0, 1.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(1, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 1,
            LinVector3D<double>.Create(_sp.One, _sp.One, _sp.One));

        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Options = Float64AdaptivePath3DSamplingOptions.Default;
        var genericOptions = AdaptivePath3DSamplingOptions<double>.Default(_sp);

        var float64Adaptive = float64Curve.CreateAdaptiveCurve3D(float64Range, float64Options);
        var genericAdaptive = genericCurve.CreateAdaptiveCurve3D(genericRange, genericOptions);

        var float64TotalLength = float64Adaptive.Length;
        var genericTotalLength = genericAdaptive.Length.ScalarValue;

        // Test at 0%, 25%, 50%, 75%, 100% of arc length
        var testLengths = new[] { 0.0, 0.25 * float64TotalLength, 0.5 * float64TotalLength, 0.75 * float64TotalLength, float64TotalLength };

        foreach (var length in testLengths)
        {
            var float64Time = float64Adaptive.LengthToTime(length);
            var genericTime = genericAdaptive.LengthToTime(_sp.ScalarFromValue(length));

            Assert.That(genericTime.ScalarValue, Is.EqualTo(float64Time).Within(Tolerance * 10),
                $"Time at length={length} should match");
        }

        Debug.Assert(true);
    }

    #endregion

    #region AdaptivePath3D Refinement Tests

    [Test]
    public void TestAdaptivePath_RefinementLevels()
    {
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 3, // High harmonic = more curvature
            LinVector3D<double>.Create(_sp.One, _sp.ScalarFromValue(2.0), _sp.ScalarFromValue(3.0)));

        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        // Test with different refinement levels
        var options1 = new AdaptivePath3DSamplingOptions<double>(_sp)
        {
            MinLevelCount = 2,
            MaxLevelCount = 4
        };

        var options2 = new AdaptivePath3DSamplingOptions<double>(_sp)
        {
            MinLevelCount = 4,
            MaxLevelCount = 6
        };

        var adaptive1 = genericCurve.CreateAdaptiveCurve3D(genericRange, options1);
        var adaptive2 = genericCurve.CreateAdaptiveCurve3D(genericRange, options2);

        // Higher refinement should produce more leaves
        Assert.That(adaptive2.LeafNodeCount, Is.GreaterThan(adaptive1.LeafNodeCount),
            "Higher refinement level should produce more leaf nodes");

        Debug.Assert(adaptive2.LeafNodeCount > adaptive1.LeafNodeCount);
    }

    [Test]
    public void TestAdaptivePath_CurvatureAdaptation()
    {
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 5, // Very high curvature
            LinVector3D<double>.Create(_sp.One, _sp.One, _sp.One));

        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var options = new AdaptivePath3DSamplingOptions<double>(_sp)
        {
            MinLevelCount = 2,
            MaxLevelCount = 8,
            MaxEdgeFramesAngle = _sp.ScalarFromValue(5.0) // Small angle threshold = more refinement
        };

        var adaptive = genericCurve.CreateAdaptiveCurve3D(genericRange, options);

        // High curvature with small angle threshold should trigger significant refinement
        Assert.That(adaptive.LeafNodeCount, Is.GreaterThan(16),
            "High curvature curve should trigger adaptive refinement");

        Debug.Assert(adaptive.LeafNodeCount > 16);
    }

    #endregion

    #region AdaptiveCurveSampler3D Tests

    [Test]
    public void TestAdaptiveSampler_Properties()
    {
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(1, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 1,
            LinVector3D<double>.Create(_sp.One, _sp.ScalarFromValue(2.0), _sp.ScalarFromValue(3.0)));

        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Options = Float64AdaptivePath3DSamplingOptions.Default;
        var genericOptions = AdaptivePath3DSamplingOptions<double>.Default(_sp);

        var float64Sampler = new AdaptiveCurveSampler3D(float64Curve, float64Range, float64Options);
        var genericSampler = new AdaptiveCurveSampler3D<double>(genericCurve, genericRange, genericOptions);

        Assert.That(genericSampler.Count, Is.EqualTo(float64Sampler.Count));
        Assert.That(genericSampler.IsValid(), Is.True);
        Assert.That(float64Sampler.IsValid(), Is.True);

        Debug.Assert(genericSampler.Count == float64Sampler.Count);
    }

    [Test]
    public void TestAdaptiveSampler_GetPoints()
    {
        var magnitude = LinFloat64Vector3D.Create(1.0, 1.0, 1.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(2, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 2,
            LinVector3D<double>.Create(_sp.One, _sp.One, _sp.One));

        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Options = Float64AdaptivePath3DSamplingOptions.Default;
        var genericOptions = AdaptivePath3DSamplingOptions<double>.Default(_sp);

        var float64Sampler = new AdaptiveCurveSampler3D(float64Curve, float64Range, float64Options);
        var genericSampler = new AdaptiveCurveSampler3D<double>(genericCurve, genericRange, genericOptions);

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

    [Test]
    public void TestAdaptiveSampler_GetParameterValues()
    {
        var magnitude = LinFloat64Vector3D.Create(2.0, 1.5, 1.0);
        var float64Curve = Float64SimpleHarmonicPath3D.FiniteSymmetric(1, magnitude);
        var genericCurve = SimpleHarmonicPath3D<double>.FiniteSymmetric(_sp, 1,
            LinVector3D<double>.Create(_sp.ScalarFromValue(2.0), _sp.ScalarFromValue(1.5), _sp.One));

        var float64Range = Float64ScalarRange.SymmetricPi;
        var genericRange = ScalarRange<double>.SymmetricPi(_sp);

        var float64Options = Float64AdaptivePath3DSamplingOptions.Default;
        var genericOptions = AdaptivePath3DSamplingOptions<double>.Default(_sp);

        var float64Sampler = new AdaptiveCurveSampler3D(float64Curve, float64Range, float64Options);
        var genericSampler = new AdaptiveCurveSampler3D<double>(genericCurve, genericRange, genericOptions);

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

    #endregion
}
