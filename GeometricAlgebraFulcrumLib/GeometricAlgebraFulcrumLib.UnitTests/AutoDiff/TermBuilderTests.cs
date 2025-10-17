using System;
using System.Collections.Generic;
using GeometricAlgebraFulcrumLib.Modeling.Calculus.AutoDiff;
using NUnit.Framework;
using static GeometricAlgebraFulcrumLib.UnitTests.AutoDiff.Utils;

namespace GeometricAlgebraFulcrumLib.UnitTests.AutoDiff;

[TestFixture]
public class TermBuilderContractTests
{
    private static readonly Variable x = new Variable();
    private static readonly Variable y = new Variable();

    [Test]
    public void ConstantContract()
    {
        Assert.That(TermBuilder.Constant(1), Is.InstanceOf<Constant>());
        Assert.That(TermBuilder.Constant(0), Is.InstanceOf<Zero>());
    }

    [Test]
    public void SumValidationContract()
    {
        Assert.Throws<ArgumentNullException>(() => TermBuilder.Sum(null));
        Assert.Throws<ArgumentNullException>(() => TermBuilder.Sum(x, null));
        Assert.Throws<ArgumentNullException>(() => TermBuilder.Sum(null, x));
        Assert.Throws<ArgumentNullException>(() => TermBuilder.Sum(x, y, null));
        Assert.Throws<ArgumentException>(() => TermBuilder.Sum(x, y, Vec(x, null)));
        Assert.Throws<ArgumentException>(() => TermBuilder.Sum(Vec(x, null)));
    }

    [Test]
    public void ProductContract()
    {
        Assert.Throws<ArgumentNullException>(() => TermBuilder.Product(x, null));
        Assert.Throws<ArgumentNullException>(() => TermBuilder.Product(null, x));
        Assert.Throws<ArgumentNullException>(() => TermBuilder.Product(x, y, null));
        Assert.Throws<ArgumentException>(() => TermBuilder.Product(x, y, Vec(x, null)));
        Assert.That(TermBuilder.Product(x, y), Is.InstanceOf<Product>());
    }

    [Test]
    public void PowerContract()
    {
        Assert.Throws<ArgumentNullException>(() => TermBuilder.Power(null, 1));
        Assert.Throws<ArgumentNullException>(() => TermBuilder.Power(x, null));
        Assert.Throws<ArgumentNullException>(() => TermBuilder.Power(null, x));
        Assert.That(TermBuilder.Power(x, 1), Is.InstanceOf<ConstPower>());

        Assert.Throws<ArgumentException>(() => TermBuilder.Power(x, double.NaN));
        Assert.Throws<ArgumentException>(() => TermBuilder.Power(x, double.PositiveInfinity));
        Assert.That(TermBuilder.Power(x, y), Is.InstanceOf<TermPower>());
        Assert.That(TermBuilder.Power(1, y), Is.InstanceOf<TermPower>());
    }

    [Test]
    public void ExpContract()
    {
        Assert.Throws<ArgumentNullException>(() => TermBuilder.Exp(null));
        Assert.That(TermBuilder.Exp(x), Is.InstanceOf<Exp>());
    }

    [Test]
    public void LogContract()
    {
        Assert.Throws<ArgumentNullException>(() => TermBuilder.Log(null));
        Assert.That(TermBuilder.Log(x), Is.InstanceOf<Log>());
    }

    [Test]
    [TestCaseSource(nameof(QuadFormContractData))]
    public void QuadFormContract(Term x, Term y, Term a11, Term a21, Term a12, Term a22)
    {
        Assert.Throws<ArgumentNullException>(() => TermBuilder.QuadForm(x, y, a11, a21, a12, a22));
    }

    public static IEnumerable<object[]> QuadFormContractData() =>
        new[]
        {
            new Term[] { null, y, 1, 2, 3, 4 },
            new Term[] { x, null, 1, 2, 3, 4 },
            new Term[] { x, y, null, 2, 3, 4 },
            new Term[] { x, y, 1, null, 3, 4 },
            new Term[] { x, y, 1, 2, null, 4 },
            new Term[] { x, y, 1, 2, 3, null },
        };
}
