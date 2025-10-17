using System;
using GeometricAlgebraFulcrumLib.Modeling.PropagatorNetworks;
using GeometricAlgebraFulcrumLib.Modeling.PropagatorNetworks.Float64;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Modeling.PropagatorNetworks;

/// <summary>
/// Tests for Propagator Networks
/// Phase 3D - Advanced Modeling: PropagatorNetworks (10 tests)
/// Tests constraint propagation networks for numeric computations
/// </summary>
[TestFixture]
public class PropagatorNetworksTests
{
    private const double Tolerance = 1e-10;

    #region Basic Network Tests (5 tests)

    [Test]
    public void PropagatorNetwork_Construction_ShouldWork()
    {
        // Arrange & Act
        var network = new PropagatorNetwork();

        // Assert
        Assert.That(network, Is.Not.Null, "Network should be created");
        Assert.That(network.Count, Is.EqualTo(0), "Empty network should have 0 cells");
    }

    [Test]
    public void PropagatorNetwork_DefineCell_ShouldAddCell()
    {
        // Arrange
        var network = new PropagatorNetwork();
        network.BeginModify();

        // Act
        var cell = network.DefineFloat64Cell("test_cell");
        network.EndModify();

        // Assert
        Assert.That(cell, Is.Not.Null, "Cell should be created");
        Assert.That(cell.Name, Is.EqualTo("test_cell"), "Cell should have correct name");
        Assert.That(network.Count, Is.EqualTo(1), "Network should have 1 cell");
    }

    [Test]
    public void PropagatorNetwork_UpdateCell_ShouldStoreValue()
    {
        // Arrange
        var network = new PropagatorNetwork();
        network.BeginModify();
        var cell = network.DefineFloat64Cell("a");
        network.EndModify();

        // Act
        cell.Update(42.0);

        // Assert
        var value = (PnValueFloat64)cell.Value;
        Assert.That(value.Value, Is.EqualTo(42.0).Within(Tolerance), "Cell should store updated value");
    }

    [Test]
    public void PropagatorNetwork_PlusOperation_ShouldPropagate()
    {
        // Arrange
        var network = new PropagatorNetwork();
        network.BeginModify();
        var a = network.DefineFloat64Cell("a");
        var b = network.DefineFloat64Cell("b");
        var c = network.DefineFloat64Cell("c");

        // c = a + b
        PnPropagatorFloat64Plus.Register(a, b, c);
        network.EndModify();

        // Act
        a.Update(10.0);
        b.Update(20.0);

        // Assert
        var cValue = (PnValueFloat64)c.Value;
        Assert.That(cValue.Value, Is.EqualTo(30.0).Within(Tolerance), "c should be a + b = 30");
    }

    [Test]
    public void PropagatorNetwork_MinusOperation_ShouldPropagate()
    {
        // Arrange
        var network = new PropagatorNetwork();
        network.BeginModify();
        var a = network.DefineFloat64Cell("a");
        var b = network.DefineFloat64Cell("b");
        var c = network.DefineFloat64Cell("c");

        // c = a - b
        PnPropagatorFloat64Minus.Register(a, b, c);
        network.EndModify();

        // Act
        a.Update(50.0);
        b.Update(20.0);

        // Assert
        var cValue = (PnValueFloat64)c.Value;
        Assert.That(cValue.Value, Is.EqualTo(30.0).Within(Tolerance), "c should be a - b = 30");
    }

    #endregion

    #region Advanced Propagation Tests (5 tests)

    [Test]
    public void PropagatorNetwork_TimesOperation_ShouldPropagate()
    {
        // Arrange
        var network = new PropagatorNetwork();
        network.BeginModify();
        var a = network.DefineFloat64Cell("a");
        var b = network.DefineFloat64Cell("b");
        var c = network.DefineFloat64Cell("c");

        // c = a * b
        PnPropagatorFloat64Times.Register(a, b, c);
        network.EndModify();

        // Act
        a.Update(6.0);
        b.Update(7.0);

        // Assert
        var cValue = (PnValueFloat64)c.Value;
        Assert.That(cValue.Value, Is.EqualTo(42.0).Within(Tolerance), "c should be a * b = 42");
    }

    [Test]
    public void PropagatorNetwork_DivideOperation_ShouldPropagate()
    {
        // Arrange
        var network = new PropagatorNetwork();
        network.BeginModify();
        var a = network.DefineFloat64Cell("a");
        var b = network.DefineFloat64Cell("b");
        var c = network.DefineFloat64Cell("c");

        // c = a / b
        PnPropagatorFloat64Divide.Register(a, b, c);
        network.EndModify();

        // Act
        a.Update(42.0);
        b.Update(7.0);

        // Assert
        var cValue = (PnValueFloat64)c.Value;
        Assert.That(cValue.Value, Is.EqualTo(6.0).Within(Tolerance), "c should be a / b = 6");
    }

    [Test]
    public void PropagatorNetwork_SquareOperation_ShouldPropagate()
    {
        // Arrange
        var network = new PropagatorNetwork();
        network.BeginModify();
        var a = network.DefineFloat64Cell("a");
        var b = network.DefineFloat64Cell("b");

        // b = a²
        PnPropagatorFloat64Square.Register(a, b);
        network.EndModify();

        // Act
        a.Update(5.0);

        // Assert
        var bValue = (PnValueFloat64)b.Value;
        Assert.That(bValue.Value, Is.EqualTo(25.0).Within(Tolerance), "b should be a² = 25");
    }

    [Test]
    public void PropagatorNetwork_AssignSum_ShouldPropagate()
    {
        // Arrange
        var network = new PropagatorNetwork();
        network.BeginModify();
        var a = network.DefineFloat64Cell("a");
        var b = network.DefineFloat64Cell("b");
        var c = network.DefineFloat64Cell("c");

        // c = a + b (with bidirectional propagation)
        network.AssignFloat64Sum("c", "a", "b");
        network.EndModify();

        // Act
        a.Update(15.0);
        b.Update(27.0);

        // Assert
        var cValue = (PnValueFloat64)c.Value;
        Assert.That(cValue.Value, Is.EqualTo(42.0).Within(Tolerance), "c should be a + b = 42");
    }

    [Test]
    public void PropagatorNetwork_PythagoreanSum_ShouldPropagate()
    {
        // Arrange: c² = a² + b²
        var network = new PropagatorNetwork();
        network.BeginModify();
        var a = network.DefineFloat64Cell("a");
        var b = network.DefineFloat64Cell("b");
        var c = network.DefineFloat64Cell("c");

        network.AssignFloat64PythagoreanSum("c", "a", "b");
        network.EndModify();

        // Act: Set a=3, b=4
        a.Update(3.0);
        b.Update(4.0);

        // Assert: c should be 5
        var cValue = (PnValueFloat64)c.Value;
        Assert.That(cValue.Value, Is.EqualTo(5.0).Within(Tolerance), "c should be √(a² + b²) = √(9 + 16) = 5");
    }

    #endregion
}
