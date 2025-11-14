using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Adaptive;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Float64.Samplers;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Adaptive;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic.Samplers;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Trajectories;

[TestFixture]
public sealed class CurveSamplers3DEquivalenceTests
{
    private const double Tolerance = 1e-10;
    private static readonly IScalarProcessor<double> ScalarProcessor = ScalarProcessorOfFloat64.Instance;
    private static readonly IComparer<Scalar<double>> ScalarValueComparer =
        Comparer<Scalar<double>>.Create((a, b) => a.ScalarValue.CompareTo(b.ScalarValue));

    #region Helper Methods

    private static LinVector3D<double> ToGenericVector(LinFloat64Vector3D v)
    {
        return LinVector3D<double>.Create(
            ScalarProcessor.ScalarFromValue(v.X.ScalarValue),
            ScalarProcessor.ScalarFromValue(v.Y.ScalarValue),
            ScalarProcessor.ScalarFromValue(v.Z.ScalarValue)
        );
    }

    private static void AssertVectorsEqual(LinVector3D<double> generic, LinFloat64Vector3D float64, string message = "")
    {
        Assert.That(generic.X.ScalarValue, Is.EqualTo(float64.X.ScalarValue).Within(Tolerance), $"{message} - X component");
        Assert.That(generic.Y.ScalarValue, Is.EqualTo(float64.Y.ScalarValue).Within(Tolerance), $"{message} - Y component");
        Assert.That(generic.Z.ScalarValue, Is.EqualTo(float64.Z.ScalarValue).Within(Tolerance), $"{message} - Z component");
    }


    #endregion

    #region AdaptiveCurveSampler3D Tests

    [Test]
    public void TestAdaptiveCurveSampler_Count()
    {
        const int harmonicFactor = 1;
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var timeRange = Float64ScalarRange.Create(0, 2 * Math.PI);

        var float64Path = Float64SimpleHarmonicPath3D.FiniteSymmetric(harmonicFactor, magnitude);
        var float64Options = new Float64AdaptivePath3DSamplingOptions(LinFloat64PolarAngle.Angle30, 2, 10);
        var float64Sampler = new AdaptiveCurveSampler3D(float64Path, timeRange, float64Options, false);

        var genericPath = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessor,
            harmonicFactor,
            ToGenericVector(magnitude)
        );
        var genericTimeRange = ScalarRange<double>.Create(
            ScalarProcessor.ScalarFromValue(0),
            ScalarProcessor.ScalarFromValue(2 * Math.PI)
        );
        var genericOptions = new AdaptivePath3DSamplingOptions<double>(
            ScalarProcessor,
            LinPolarAngle<double>.CreateFromDegrees(ScalarProcessor, 30),
            2,
            10
        );
        var genericSampler = new AdaptiveCurveSampler3D<double>(genericPath, genericTimeRange, genericOptions, false);

        Assert.That(genericSampler.Count, Is.EqualTo(float64Sampler.Count));
        Debug.Assert(genericSampler.Count == float64Sampler.Count);
    }

    [Test]
    public void TestAdaptiveCurveSampler_GetPoints()
    {
        const int harmonicFactor = 1;
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var timeRange = Float64ScalarRange.Create(0, Math.PI);

        var float64Path = Float64SimpleHarmonicPath3D.FiniteSymmetric(harmonicFactor, magnitude);
        var float64Options = new Float64AdaptivePath3DSamplingOptions(LinFloat64PolarAngle.Angle30, 2, 10);
        var float64Sampler = new AdaptiveCurveSampler3D(float64Path, timeRange, float64Options, false);

        var genericPath = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessor,
            harmonicFactor,
            ToGenericVector(magnitude)
        );
        var genericTimeRange = ScalarRange<double>.Create(
            ScalarProcessor.ScalarFromValue(0),
            ScalarProcessor.ScalarFromValue(Math.PI)
        );
        var genericOptions = new AdaptivePath3DSamplingOptions<double>(
            ScalarProcessor,
            LinPolarAngle<double>.CreateFromDegrees(ScalarProcessor, 30),
            2,
            10
        );
        var genericSampler = new AdaptiveCurveSampler3D<double>(genericPath, genericTimeRange, genericOptions, false);

        var float64Points = float64Sampler.GetFrames().Select(f => f.Point).ToArray();
        var genericPoints = genericSampler.GetFrames().Select(f => f.Point).ToArray();

        Assert.That(genericPoints.Length, Is.EqualTo(float64Points.Length));

        for (int i = 0; i < float64Points.Length; i++)
        {
            AssertVectorsEqual(genericPoints[i], float64Points[i], $"Point {i}");
        }
    }

    #endregion

    #region ConstantCurveSampler3D Tests

    [Test]
    public void TestConstantCurveSampler_Count()
    {
        var point = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var timeRange = Float64ScalarRange.Create(0, 1.0);

        var float64Sampler = new ConstantCurveSampler3D(point, timeRange);

        var genericPoint = ToGenericVector(point);
        var genericTimeRange = ScalarRange<double>.Create(
            ScalarProcessor.ScalarFromValue(0),
            ScalarProcessor.ScalarFromValue(1.0)
        );
        var genericSampler = new ConstantCurveSampler3D<double>(genericPoint, genericTimeRange);

        Assert.That(genericSampler.Count, Is.EqualTo(float64Sampler.Count));
        Assert.That(float64Sampler.Count, Is.EqualTo(2)); // Constant sampler always has 2 points
        Debug.Assert(genericSampler.Count == 2);
    }

    [Test]
    public void TestConstantCurveSampler_GetPoints()
    {
        var point = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var timeRange = Float64ScalarRange.Create(0, 1.0);

        var float64Sampler = new ConstantCurveSampler3D(point, timeRange);

        var genericPoint = ToGenericVector(point);
        var genericTimeRange = ScalarRange<double>.Create(
            ScalarProcessor.ScalarFromValue(0),
            ScalarProcessor.ScalarFromValue(1.0)
        );
        var genericSampler = new ConstantCurveSampler3D<double>(genericPoint, genericTimeRange);

        var float64Points = float64Sampler.GetPoints().ToArray();
        var genericPoints = genericSampler.GetPoints().ToArray();

        Assert.That(genericPoints.Length, Is.EqualTo(float64Points.Length));

        for (int i = 0; i < float64Points.Length; i++)
        {
            AssertVectorsEqual(genericPoints[i], float64Points[i], $"Point {i}");
        }
    }

    #endregion

    #region UniformParameterCurveSampler3D Tests

    [Test]
    public void TestUniformParameterCurveSampler_Count()
    {
        const int harmonicFactor = 1;
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var timeRange = Float64ScalarRange.Create(0, 2 * Math.PI);
        const int count = 10;

        var float64Path = Float64SimpleHarmonicPath3D.FiniteSymmetric(harmonicFactor, magnitude);
        var float64Sampler = new UniformParameterCurveSampler3D(float64Path, timeRange, count, false);

        var genericPath = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessor,
            harmonicFactor,
            ToGenericVector(magnitude)
        );
        var genericTimeRange = ScalarRange<double>.Create(
            ScalarProcessor.ScalarFromValue(0),
            ScalarProcessor.ScalarFromValue(2 * Math.PI)
        );
        var genericSampler = new UniformParameterCurveSampler3D<double>(genericPath, genericTimeRange, count, false);

        Assert.That(genericSampler.Count, Is.EqualTo(float64Sampler.Count));
        Debug.Assert(genericSampler.Count == float64Sampler.Count);
    }

    [Test]
    public void TestUniformParameterCurveSampler_GetPoints()
    {
        const int harmonicFactor = 1;
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var timeRange = Float64ScalarRange.Create(0, 2 * Math.PI);
        const int count = 5;

        var float64Path = Float64SimpleHarmonicPath3D.FiniteSymmetric(harmonicFactor, magnitude);
        var float64Sampler = new UniformParameterCurveSampler3D(float64Path, timeRange, count, false);

        var genericPath = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessor,
            harmonicFactor,
            ToGenericVector(magnitude)
        );
        var genericTimeRange = ScalarRange<double>.Create(
            ScalarProcessor.ScalarFromValue(0),
            ScalarProcessor.ScalarFromValue(2 * Math.PI)
        );
        var genericSampler = new UniformParameterCurveSampler3D<double>(genericPath, genericTimeRange, count, false);

        var float64Points = float64Sampler.GetPoints().ToArray();
        var genericPoints = genericSampler.GetPoints().ToArray();

        Assert.That(genericPoints.Length, Is.EqualTo(float64Points.Length));

        for (int i = 0; i < float64Points.Length; i++)
        {
            AssertVectorsEqual(genericPoints[i], float64Points[i], $"Point {i}");
        }
    }

    #endregion

    #region ParameterListCurveSampler3D Tests

    [Test]
    public void TestParameterListCurveSampler_Count()
    {
        const int harmonicFactor = 1;
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var parameterValues = ImmutableSortedSet.Create(
            Float64Scalar.Create(0.0),
            Float64Scalar.Create(0.5),
            Float64Scalar.Create(1.0),
            Float64Scalar.Create(1.5),
            Float64Scalar.Create(2.0)
        );

        var float64Path = Float64SimpleHarmonicPath3D.FiniteSymmetric(harmonicFactor, magnitude);
        var float64Sampler = new ParameterListCurveSampler3D(float64Path, parameterValues, false);

        var genericPath = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessor,
            harmonicFactor,
            ToGenericVector(magnitude)
        );
        var genericParameterValues = ImmutableSortedSet.Create(
            ScalarValueComparer,
            ScalarProcessor.ScalarFromValue(0.0),
            ScalarProcessor.ScalarFromValue(0.5),
            ScalarProcessor.ScalarFromValue(1.0),
            ScalarProcessor.ScalarFromValue(1.5),
            ScalarProcessor.ScalarFromValue(2.0)
        );
        var genericSampler = new ParameterListCurveSampler3D<double>(genericPath, genericParameterValues, false);

        Assert.That(genericSampler.Count, Is.EqualTo(float64Sampler.Count));
        Assert.That(float64Sampler.Count, Is.EqualTo(5));
        Debug.Assert(genericSampler.Count == 5);
    }

    [Test]
    public void TestParameterListCurveSampler_GetPoints()
    {
        const int harmonicFactor = 1;
        var magnitude = LinFloat64Vector3D.Create(1.0, 2.0, 3.0);
        var parameterValues = ImmutableSortedSet.Create(
            Float64Scalar.Create(0.0),
            Float64Scalar.Create(Math.PI / 4),
            Float64Scalar.Create(Math.PI / 2),
            Float64Scalar.Create(3 * Math.PI / 4),
            Float64Scalar.Create(Math.PI)
        );

        var float64Path = Float64SimpleHarmonicPath3D.FiniteSymmetric(harmonicFactor, magnitude);
        var float64Sampler = new ParameterListCurveSampler3D(float64Path, parameterValues, false);

        var genericPath = SimpleHarmonicPath3D<double>.FiniteSymmetric(
            ScalarProcessor,
            harmonicFactor,
            ToGenericVector(magnitude)
        );
        var genericParameterValues = ImmutableSortedSet.Create(
            ScalarValueComparer,
            ScalarProcessor.ScalarFromValue(0.0),
            ScalarProcessor.ScalarFromValue(Math.PI / 4),
            ScalarProcessor.ScalarFromValue(Math.PI / 2),
            ScalarProcessor.ScalarFromValue(3 * Math.PI / 4),
            ScalarProcessor.ScalarFromValue(Math.PI)
        );
        var genericSampler = new ParameterListCurveSampler3D<double>(genericPath, genericParameterValues, false);

        var float64Points = float64Sampler.GetPoints().ToArray();
        var genericPoints = genericSampler.GetPoints().ToArray();

        Assert.That(genericPoints.Length, Is.EqualTo(float64Points.Length));

        for (int i = 0; i < float64Points.Length; i++)
        {
            AssertVectorsEqual(genericPoints[i], float64Points[i], $"Point {i}");
        }
    }

    #endregion
}
