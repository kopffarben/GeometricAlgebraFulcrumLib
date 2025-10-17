using System;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Combinations;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Utilities.Structures;

/// <summary>
/// Tests for Combination class - lexicographic combination generation
/// Based on: https://www.developertyrone.com/blog/generating-the-mth-lexicographical-element-of-a-mathematical-combination/
/// </summary>
[TestFixture]
public class CombinationTests
{
    #region Constructor Tests

    [Test]
    public void Constructor_ValidParameters_ShouldInitialize()
    {
        var comb = new Combination(5, 3);

        Assert.That(comb.IsValid(), Is.True);
        Assert.That(comb.ToString(), Is.EqualTo("{ 0 1 2 }"));
    }

    [Test]
    public void Constructor_NegativeN_ShouldThrow()
    {
        Assert.Throws<Exception>(() => new Combination(-5, 3));
    }

    [Test]
    public void Constructor_NegativeK_ShouldThrow()
    {
        Assert.Throws<Exception>(() => new Combination(5, -3));
    }

    [Test]
    public void Constructor_WithArray_ValidData_ShouldInitialize()
    {
        var data = new long[] { 1, 3, 5 };
        var comb = new Combination(10, 3, data);

        Assert.That(comb.IsValid(), Is.True);
        Assert.That(comb.ToString(), Is.EqualTo("{ 1 3 5 }"));
    }

    [Test]
    public void Constructor_WithArray_WrongLength_ShouldThrow()
    {
        var data = new long[] { 1, 3 }; // Length 2, but k=3
        Assert.Throws<Exception>(() => new Combination(10, 3, data));
    }

    [Test]
    public void Constructor_WithArray_InvalidData_ShouldThrow()
    {
        var data = new long[] { 3, 1, 5 }; // Not lexicographic order
        Assert.Throws<Exception>(() => new Combination(10, 3, data));
    }

    [Test]
    public void Constructor_EdgeCase_NEqualsK_ShouldWork()
    {
        var comb = new Combination(5, 5);

        Assert.That(comb.IsValid(), Is.True);
        Assert.That(comb.ToString(), Is.EqualTo("{ 0 1 2 3 4 }"));
    }

    [Test]
    public void Constructor_EdgeCase_KEqualsZero_ShouldWork()
    {
        var comb = new Combination(5, 0);

        Assert.That(comb.IsValid(), Is.True);
        Assert.That(comb.ToString(), Is.EqualTo("{ }"));
    }

    #endregion

    #region IsValid Tests

    [Test]
    public void IsValid_ValidCombination_ShouldReturnTrue()
    {
        var data = new long[] { 0, 2, 4, 6 };
        var comb = new Combination(10, 4, data);

        Assert.That(comb.IsValid(), Is.True);
    }

    [Test]
    public void IsValid_ValueOutOfRange_ShouldReturnFalse()
    {
        var data = new long[] { 0, 2, 10 }; // 10 >= n (10)
        Assert.Throws<Exception>(() => new Combination(10, 3, data));
    }

    [Test]
    public void IsValid_NotLexicographic_ShouldReturnFalse()
    {
        var data = new long[] { 2, 1, 3 }; // Not sorted
        Assert.Throws<Exception>(() => new Combination(10, 3, data));
    }

    [Test]
    public void IsValid_Duplicate_ShouldReturnFalse()
    {
        var data = new long[] { 1, 2, 2 }; // Duplicate
        Assert.Throws<Exception>(() => new Combination(10, 3, data));
    }

    #endregion

    #region Successor Tests

    [Test]
    public void Successor_SimpleCombination_ShouldReturnNext()
    {
        var comb = new Combination(5, 3); // { 0 1 2 }
        var next = comb.Successor();

        Assert.That(next, Is.Not.Null);
        Assert.That(next.ToString(), Is.EqualTo("{ 0 1 3 }"));
    }

    [Test]
    public void Successor_MiddleCombination_ShouldReturnNext()
    {
        var data = new long[] { 0, 2, 4 };
        var comb = new Combination(5, 3, data);
        var next = comb.Successor();

        Assert.That(next, Is.Not.Null);
        Assert.That(next.ToString(), Is.EqualTo("{ 0 3 4 }"));
    }

    [Test]
    public void Successor_LastCombination_ShouldReturnNull()
    {
        var data = new long[] { 2, 3, 4 }; // Last combination for C(5,3)
        var comb = new Combination(5, 3, data);
        var next = comb.Successor();

        Assert.That(next, Is.Null);
    }

    [Test]
    public void Successor_Chain_ShouldGenerateAllCombinations()
    {
        // C(5, 3) has 10 combinations
        var comb = new Combination(5, 3);
        var count = 1;

        while ((comb = comb.Successor()) != null)
        {
            count++;
            Assert.That(comb.IsValid(), Is.True);
        }

        Assert.That(count, Is.EqualTo(10), "C(5,3) = 10");
    }

    [Test]
    public void Successor_C4_2_ShouldGenerate6Combinations()
    {
        // C(4, 2) has 6 combinations: {0,1}, {0,2}, {0,3}, {1,2}, {1,3}, {2,3}
        var comb = new Combination(4, 2);
        var combinations = new System.Collections.Generic.List<string> { comb.ToString() };

        while ((comb = comb.Successor()) != null)
        {
            combinations.Add(comb.ToString());
        }

        Assert.That(combinations.Count, Is.EqualTo(6));
        Assert.That(combinations[0], Is.EqualTo("{ 0 1 }"));
        Assert.That(combinations[1], Is.EqualTo("{ 0 2 }"));
        Assert.That(combinations[2], Is.EqualTo("{ 0 3 }"));
        Assert.That(combinations[3], Is.EqualTo("{ 1 2 }"));
        Assert.That(combinations[4], Is.EqualTo("{ 1 3 }"));
        Assert.That(combinations[5], Is.EqualTo("{ 2 3 }"));
    }

    #endregion

    #region Choose Tests

    [Test]
    public void Choose_C5_3_ShouldReturn10()
    {
        Assert.That(Combination.Choose(5, 3), Is.EqualTo(10));
    }

    [Test]
    public void Choose_C10_5_ShouldReturn252()
    {
        Assert.That(Combination.Choose(10, 5), Is.EqualTo(252));
    }

    [Test]
    public void Choose_C4_2_ShouldReturn6()
    {
        Assert.That(Combination.Choose(4, 2), Is.EqualTo(6));
    }

    [Test]
    public void Choose_NEqualsK_ShouldReturn1()
    {
        Assert.That(Combination.Choose(5, 5), Is.EqualTo(1));
        Assert.That(Combination.Choose(10, 10), Is.EqualTo(1));
    }

    [Test]
    public void Choose_KEqualsZero_ShouldReturn1()
    {
        Assert.That(Combination.Choose(5, 0), Is.EqualTo(1));
        Assert.That(Combination.Choose(100, 0), Is.EqualTo(1));
    }

    [Test]
    public void Choose_NLessThanK_ShouldReturn0()
    {
        Assert.That(Combination.Choose(3, 5), Is.EqualTo(0));
    }

    [Test]
    public void Choose_NegativeN_ShouldThrow()
    {
        Assert.Throws<Exception>(() => Combination.Choose(-5, 3));
    }

    [Test]
    public void Choose_NegativeK_ShouldThrow()
    {
        Assert.Throws<Exception>(() => Combination.Choose(5, -3));
    }

    [Test]
    public void Choose_LargeValues_ShouldWork()
    {
        // C(20, 10) = 184,756
        Assert.That(Combination.Choose(20, 10), Is.EqualTo(184756));
    }

    [Test]
    public void Choose_Symmetry_ShouldHold()
    {
        // C(n, k) = C(n, n-k)
        Assert.That(Combination.Choose(10, 3), Is.EqualTo(Combination.Choose(10, 7)));
        Assert.That(Combination.Choose(20, 5), Is.EqualTo(Combination.Choose(20, 15)));
    }

    [Test]
    public void Choose_PascalIdentity_ShouldHold()
    {
        // C(n, k) = C(n-1, k-1) + C(n-1, k)
        var n = 10;
        var k = 5;
        var left = Combination.Choose(n, k);
        var right = Combination.Choose(n - 1, k - 1) + Combination.Choose(n - 1, k);

        Assert.That(left, Is.EqualTo(right), "Pascal's identity should hold");
    }

    #endregion

    #region ToString Tests

    [Test]
    public void ToString_EmptySet_ShouldReturnEmptyBraces()
    {
        var comb = new Combination(5, 0);
        Assert.That(comb.ToString(), Is.EqualTo("{ }"));
    }

    [Test]
    public void ToString_SingleElement_ShouldFormatCorrectly()
    {
        var data = new long[] { 3 };
        var comb = new Combination(5, 1, data);
        Assert.That(comb.ToString(), Is.EqualTo("{ 3 }"));
    }

    [Test]
    public void ToString_MultipleElements_ShouldFormatCorrectly()
    {
        var data = new long[] { 1, 3, 5, 7 };
        var comb = new Combination(10, 4, data);
        Assert.That(comb.ToString(), Is.EqualTo("{ 1 3 5 7 }"));
    }

    #endregion
}
