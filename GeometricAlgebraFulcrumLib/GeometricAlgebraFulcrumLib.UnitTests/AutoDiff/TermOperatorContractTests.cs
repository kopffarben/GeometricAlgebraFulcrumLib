using System;
using GeometricAlgebraFulcrumLib.Modeling.Calculus.AutoDiff;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.AutoDiff;

[TestFixture]
public class TermOperatorContractTests
{
    Term x = new Variable();

    [Test]
    public void PlusContract()
    {
        Assert.Throws<ArgumentNullException>(() => { var _ = x + null; });
        Assert.Throws<ArgumentNullException>(() => { var _ = null + x; });
    }

    [Test]
    public void StarContract()
    {
        Assert.Throws<ArgumentNullException>(() => { var _ = x * null; });
        Assert.Throws<ArgumentNullException>(() => { var _ = null * x; });
    }

    [Test]
    public void UnaryMinusContract()
    {
        Term trm = null;
        Assert.Throws<ArgumentNullException>(() => { var _ = -trm; });
    }

    [Test]
    public void BinaryMinusContract()
    {
        Assert.Throws<ArgumentNullException>(() => { var _ = x - null; });
        Assert.Throws<ArgumentNullException>(() => { var _ = null - x; });
    }

    [Test]
    public void SlashContract()
    {
        Assert.Throws<ArgumentNullException>(() => { var _ = null / x; });
        Assert.Throws<ArgumentNullException>(() => { var _ = x / null; });
    }
}
