using System;
using System.Linq;
using System.Reflection;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Spaces.Conformal;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Spaces.Conformal;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Algebra;

/// <summary>
/// Unit tests for XGaConformalComposerUtils equivalence - Module 1, Task 1.5 of deduplication roadmap.
/// Tests ensure Float64 and Generic&lt;T&gt; conformal composer utilities are structurally equivalent.
/// Both classes are currently empty placeholders for future conformal GA composition methods.
/// </summary>
[TestFixture]
public class XGaConformalComposerUtilsEquivalenceTests
{
    [Test]
    public void Float64ConformalComposerUtils_ShouldExist()
    {
        // Arrange & Act
        var float64Type = typeof(XGaFloat64ConformalComposerUtils);

        // Assert
        Assert.That(float64Type, Is.Not.Null, "Float64 conformal composer utils class should exist");
        Assert.That(float64Type.IsClass, Is.True, "Should be a class");
        Assert.That(float64Type.IsAbstract && float64Type.IsSealed, Is.True, "Should be static class");
    }

    [Test]
    public void GenericConformalComposerUtils_ShouldExist()
    {
        // Arrange & Act
        var genericType = typeof(XGaConformalComposerUtils<>);

        // Assert
        Assert.That(genericType, Is.Not.Null, "Generic conformal composer utils class should exist");
        Assert.That(genericType.IsClass, Is.True, "Should be a class");
        Assert.That(genericType.IsAbstract && genericType.IsSealed, Is.True, "Should be static class");
        Assert.That(genericType.IsGenericTypeDefinition, Is.True, "Should be generic class");
    }

    [Test]
    public void Float64AndGeneric_ShouldBothBeEmpty()
    {
        // Arrange
        var float64Type = typeof(XGaFloat64ConformalComposerUtils);
        var genericType = typeof(XGaConformalComposerUtils<double>);

        // Act - Get public methods (excluding inherited object methods)
        var float64Methods = float64Type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        var genericMethods = genericType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        // Assert
        Assert.That(float64Methods.Length, Is.EqualTo(0),
            "Float64 class should have no public methods (empty placeholder)");
        Assert.That(genericMethods.Length, Is.EqualTo(0),
            "Generic class should have no public methods (empty placeholder)");
    }

    [Test]
    public void Float64AndGeneric_ShouldHaveEquivalentNamespaces()
    {
        // Arrange
        var float64Type = typeof(XGaFloat64ConformalComposerUtils);
        var genericType = typeof(XGaConformalComposerUtils<double>);

        // Act
        var float64Namespace = float64Type.Namespace;
        var genericNamespace = genericType.Namespace;

        // Assert
        Assert.That(float64Namespace, Is.EqualTo("GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Spaces.Conformal"),
            "Float64 should be in Float64.Spaces.Conformal namespace");
        Assert.That(genericNamespace, Is.EqualTo("GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Spaces.Conformal"),
            "Generic should be in Generic.Spaces.Conformal namespace");
    }

    [Test]
    public void GenericConformalComposerUtils_ShouldInstantiateWithDoubleType()
    {
        // Arrange & Act
        var genericType = typeof(XGaConformalComposerUtils<double>);

        // Assert
        Assert.That(genericType, Is.Not.Null, "Generic<double> type should be constructible");
        Assert.That(genericType.IsGenericType, Is.True, "Should be a generic type");
        Assert.That(genericType.GetGenericArguments()[0], Is.EqualTo(typeof(double)),
            "Type parameter should be double");
    }
}
