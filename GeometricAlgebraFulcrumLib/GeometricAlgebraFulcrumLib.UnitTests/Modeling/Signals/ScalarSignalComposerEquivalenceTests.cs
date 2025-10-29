using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Float64.Composers;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Basic;
using GeometricAlgebraFulcrumLib.Modeling.Trajectories.Scalars.Generic.Composers;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.Signals;

[TestFixture]
public sealed class ScalarSignalComposerEquivalenceTests
{
    private const double Tolerance = 1e-12;

    private IScalarProcessor<double> ScalarProcessor { get; }
        = ScalarProcessorOfFloat64.Instance;

    [Test]
    public void TestAppendSignal()
    {
        // Float64 version
        var composerFloat64 = new Float64ScalarSignalComposer();
        composerFloat64.AppendSignal(Float64ScalarSinSignal.FiniteInstance);
        composerFloat64.AppendSignal(Float64ScalarCosSignal.FiniteInstance);

        // Generic version
        var composerGeneric = ScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.AppendSignal(SinScalarSignal<double>.Finite(ScalarProcessor));
        composerGeneric.AppendSignal(CosScalarSignal<double>.Finite(ScalarProcessor));

        // Both should have 2 signals
        Assert.That(composerGeneric.Count, Is.EqualTo(composerFloat64.Count));
        Assert.That(composerGeneric.Count, Is.EqualTo(2));
    }

    [Test]
    public void TestPrependSignal()
    {
        // Float64 version
        var composerFloat64 = new Float64ScalarSignalComposer();
        composerFloat64.AppendSignal(Float64ScalarSinSignal.FiniteInstance);
        composerFloat64.PrependSignal(Float64ScalarCosSignal.FiniteInstance);

        // Generic version
        var composerGeneric = ScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.AppendSignal(SinScalarSignal<double>.Finite(ScalarProcessor));
        composerGeneric.PrependSignal(CosScalarSignal<double>.Finite(ScalarProcessor));

        // Should have 2 signals, with cos first
        Assert.That(composerGeneric.Count, Is.EqualTo(2));

        // First signal should be cos (type check via value at t=0: cos(0)=1, sin(0)=0)
        var firstValue = composerGeneric[0].GetValue(ScalarProcessor.Scalar(0.0)).ScalarValue;
        Assert.That(firstValue, Is.EqualTo(1.0).Within(Tolerance));
    }

    [Test]
    public void TestInsertSignal()
    {
        // Float64 version
        var composerFloat64 = new Float64ScalarSignalComposer();
        composerFloat64.AppendSignal(Float64ScalarSinSignal.FiniteInstance);
        composerFloat64.AppendSignal(Float64ScalarCosSignal.FiniteInstance);
        composerFloat64.InsertSignal(1, Float64ScalarConstantOneSignal.FiniteInstance);

        // Generic version
        var composerGeneric = ScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.AppendSignal(SinScalarSignal<double>.Finite(ScalarProcessor));
        composerGeneric.AppendSignal(CosScalarSignal<double>.Finite(ScalarProcessor));
        composerGeneric.InsertSignal(1, ConstantOneScalarSignal<double>.Finite(ScalarProcessor));

        // Should have 3 signals
        Assert.That(composerGeneric.Count, Is.EqualTo(3));

        // Middle signal should be constant one
        var middleValue = composerGeneric[1].GetValue(ScalarProcessor.Scalar(0.5)).ScalarValue;
        Assert.That(middleValue, Is.EqualTo(1.0).Within(Tolerance));
    }

    [Test]
    public void TestClear()
    {
        var composerGeneric = ScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.AppendSignal(SinScalarSignal<double>.Finite(ScalarProcessor));
        composerGeneric.AppendSignal(CosScalarSignal<double>.Finite(ScalarProcessor));

        composerGeneric.Clear();

        Assert.That(composerGeneric.Count, Is.EqualTo(0));
    }

    [Test]
    public void TestRemoveAt()
    {
        // Float64 version
        var composerFloat64 = new Float64ScalarSignalComposer();
        composerFloat64.AppendSignal(Float64ScalarSinSignal.FiniteInstance);
        composerFloat64.AppendSignal(Float64ScalarCosSignal.FiniteInstance);
        composerFloat64.AppendSignal(Float64ScalarConstantOneSignal.FiniteInstance);
        composerFloat64.RemoveAt(1);  // Remove cos

        // Generic version
        var composerGeneric = ScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.AppendSignal(SinScalarSignal<double>.Finite(ScalarProcessor));
        composerGeneric.AppendSignal(CosScalarSignal<double>.Finite(ScalarProcessor));
        composerGeneric.AppendSignal(ConstantOneScalarSignal<double>.Finite(ScalarProcessor));
        composerGeneric.RemoveAt(1);  // Remove cos

        Assert.That(composerGeneric.Count, Is.EqualTo(2));

        // Second signal should be constant one (sin at 0 = 0, constant = 1)
        var secondValue = composerGeneric[1].GetValue(ScalarProcessor.Scalar(0.0)).ScalarValue;
        Assert.That(secondValue, Is.EqualTo(1.0).Within(Tolerance));
    }

    [Test]
    public void TestIndexerGet()
    {
        var composerGeneric = ScalarSignalComposer<double>.Create(ScalarProcessor);
        var sinSignal = SinScalarSignal<double>.Finite(ScalarProcessor);
        composerGeneric.AppendSignal(sinSignal);

        var retrieved = composerGeneric[0];
        Assert.That(retrieved, Is.SameAs(sinSignal));
    }

    [Test]
    public void TestIndexerSet()
    {
        var composerGeneric = ScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.AppendSignal(SinScalarSignal<double>.Finite(ScalarProcessor));

        var cosSignal = CosScalarSignal<double>.Finite(ScalarProcessor);
        composerGeneric[0] = cosSignal;

        var retrieved = composerGeneric[0];
        Assert.That(retrieved, Is.SameAs(cosSignal));
    }

    [Test]
    public void TestGetFiniteSignal()
    {
        var composerGeneric = ScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.AppendSignal(SinScalarSignal<double>.Finite(ScalarProcessor));
        composerGeneric.AppendSignal(CosScalarSignal<double>.Finite(ScalarProcessor));

        var listSignal = composerGeneric.GetFiniteSignal();

        Assert.That(listSignal.IsFinite, Is.True);
        Assert.That(listSignal.Count, Is.EqualTo(2));
    }

    [Test]
    public void TestGetPeriodicSignal()
    {
        var composerGeneric = ScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.AppendSignal(SinScalarSignal<double>.Finite(ScalarProcessor));
        composerGeneric.AppendSignal(CosScalarSignal<double>.Finite(ScalarProcessor));

        var listSignal = composerGeneric.GetPeriodicSignal();

        Assert.That(listSignal.IsPeriodic, Is.True);
        Assert.That(listSignal.Count, Is.EqualTo(2));
    }

    [Test]
    public void TestFluentAPI()
    {
        // Test method chaining
        var composerGeneric = ScalarSignalComposer<double>.Create(ScalarProcessor)
            .AppendSignal(SinScalarSignal<double>.Finite(ScalarProcessor))
            .AppendSignal(CosScalarSignal<double>.Finite(ScalarProcessor))
            .PrependSignal(ConstantOneScalarSignal<double>.Finite(ScalarProcessor));

        Assert.That(composerGeneric.Count, Is.EqualTo(3));

        // First should be constant one
        var firstValue = composerGeneric[0].GetValue(ScalarProcessor.Scalar(0.5)).ScalarValue;
        Assert.That(firstValue, Is.EqualTo(1.0).Within(Tolerance));
    }

    [Test]
    public void TestEnumerator()
    {
        var composerGeneric = ScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.AppendSignal(SinScalarSignal<double>.Finite(ScalarProcessor));
        composerGeneric.AppendSignal(CosScalarSignal<double>.Finite(ScalarProcessor));

        var count = 0;
        foreach (var signal in composerGeneric)
        {
            Assert.That(signal, Is.Not.Null);
            count++;
        }

        Assert.That(count, Is.EqualTo(2));
    }

    [Test]
    public void TestAppendListSignal_Flattening()
    {
        // Create a list signal
        var listSignal = ScalarListSignal<double>.Finite(
            SinScalarSignal<double>.Finite(ScalarProcessor),
            CosScalarSignal<double>.Finite(ScalarProcessor)
        );

        // Append it to composer - should flatten
        var composerGeneric = ScalarSignalComposer<double>.Create(ScalarProcessor);
        composerGeneric.AppendSignal(listSignal);

        // Should have 2 signals, not 1 list signal
        Assert.That(composerGeneric.Count, Is.EqualTo(2));
    }
}
