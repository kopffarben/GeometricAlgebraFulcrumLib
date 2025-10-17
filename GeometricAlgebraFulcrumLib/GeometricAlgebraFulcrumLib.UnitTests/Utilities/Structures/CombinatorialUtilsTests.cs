using System.Collections.Generic;
using System.Linq;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Combinations;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Utilities.Structures;

/// <summary>
/// Tests for CombinatorialUtils - Generic permutations and combinations generation
/// </summary>
[TestFixture]
public class CombinatorialUtilsTests
{
    #region GetPermutationsRepeated Tests

    [Test]
    public void GetPermutationsRepeated_Length1_ShouldReturnElements()
    {
        var list = new[] { 1, 2, 3 };
        var perms = list.GetPermutationsRepeated(1).ToList();

        Assert.That(perms.Count, Is.EqualTo(3));
        Assert.That(perms[0], Is.EqualTo(new[] { 1 }));
        Assert.That(perms[1], Is.EqualTo(new[] { 2 }));
        Assert.That(perms[2], Is.EqualTo(new[] { 3 }));
    }

    [Test]
    public void GetPermutationsRepeated_Length2_ShouldGenerate9Permutations()
    {
        // Input: {1,2,3}, 2
        // Output: {1,1} {1,2} {1,3} {2,1} {2,2} {2,3} {3,1} {3,2} {3,3}
        var list = new[] { 1, 2, 3 };
        var perms = list.GetPermutationsRepeated(2).ToList();

        Assert.That(perms.Count, Is.EqualTo(9), "3^2 = 9 permutations with repetition");

        // Check first few
        Assert.That(perms[0], Is.EqualTo(new[] { 1, 1 }));
        Assert.That(perms[1], Is.EqualTo(new[] { 1, 2 }));
        Assert.That(perms[2], Is.EqualTo(new[] { 1, 3 }));
        Assert.That(perms[3], Is.EqualTo(new[] { 2, 1 }));
    }

    [Test]
    public void GetPermutationsRepeated_ExampleFromDocumentation()
    {
        // Input:  {1,2,3,4}, 2
        // Output: {1,1} {1,2} {1,3} {1,4} {2,1} {2,2} {2,3} {2,4} {3,1} {3,2} {3,3} {3,4} {4,1} {4,2} {4,3} {4,4}
        var list = new[] { 1, 2, 3, 4 };
        var perms = list.GetPermutationsRepeated(2).ToList();

        Assert.That(perms.Count, Is.EqualTo(16), "4^2 = 16");
    }

    #endregion

    #region GetPermutationsDistinct Tests

    [Test]
    public void GetPermutationsDistinct_Length1_ShouldReturnElements()
    {
        var list = new[] { 1, 2, 3 };
        var perms = list.GetPermutationsDistinct(1).ToList();

        Assert.That(perms.Count, Is.EqualTo(3));
    }

    [Test]
    public void GetPermutationsDistinct_Length2_ShouldGenerate6Permutations()
    {
        // Input: {1,2,3}, 2
        // Output: {1,2} {1,3} {2,1} {2,3} {3,1} {3,2}
        var list = new[] { 1, 2, 3 };
        var perms = list.GetPermutationsDistinct(2).ToList();

        Assert.That(perms.Count, Is.EqualTo(6), "P(3,2) = 3!/(3-2)! = 6");
    }

    [Test]
    public void GetPermutationsDistinct_ExampleFromDocumentation()
    {
        // Input:  {1,2,3,4}, 2
        // Output: {1,2} {1,3} {1,4} {2,1} {2,3} {2,4} {3,1} {3,2} {3,4} {4,1} {4,2} {4,3}
        var list = new[] { 1, 2, 3, 4 };
        var perms = list.GetPermutationsDistinct(2).ToList();

        Assert.That(perms.Count, Is.EqualTo(12), "P(4,2) = 4!/(4-2)! = 12");

        // Verify no element appears twice in same permutation
        foreach (var perm in perms)
        {
            var permList = perm.ToList();
            Assert.That(permList.Distinct().Count(), Is.EqualTo(permList.Count),
                "No duplicates in distinct permutations");
        }
    }

    [Test]
    public void GetPermutationsDistinct_FullLength_ShouldReturnFactorialPermutations()
    {
        var list = new[] { 1, 2, 3 };
        var perms = list.GetPermutationsDistinct(3).ToList();

        Assert.That(perms.Count, Is.EqualTo(6), "3! = 6");
    }

    #endregion

    #region GetCombinationsRepeated Tests

    [Test]
    public void GetCombinationsRepeated_Length1_ShouldReturnElements()
    {
        var list = new[] { 1, 2, 3 };
        var combs = list.GetCombinationsRepeated(1).ToList();

        Assert.That(combs.Count, Is.EqualTo(3));
    }

    [Test]
    public void GetCombinationsRepeated_Length2_ShouldGenerate6Combinations()
    {
        // Input: {1,2,3}, 2
        // Output: {1,1} {1,2} {1,3} {2,2} {2,3} {3,3}
        var list = new[] { 1, 2, 3 };
        var combs = list.GetCombinationsRepeated(2).ToList();

        Assert.That(combs.Count, Is.EqualTo(6));
    }

    [Test]
    public void GetCombinationsRepeated_ExampleFromDocumentation()
    {
        // Input:  {1,2,3,4}, 2
        // Output: {1,1} {1,2} {1,3} {1,4} {2,2} {2,3} {2,4} {3,3} {3,4} {4,4}
        var list = new[] { 1, 2, 3, 4 };
        var combs = list.GetCombinationsRepeated(2).ToList();

        Assert.That(combs.Count, Is.EqualTo(10), "C(n+k-1, k) = C(4+2-1, 2) = C(5,2) = 10");

        // Verify all combinations are in non-decreasing order
        foreach (var comb in combs)
        {
            var combList = comb.ToList();
            for (int i = 0; i < combList.Count - 1; i++)
            {
                Assert.That(combList[i], Is.LessThanOrEqualTo(combList[i + 1]),
                    "Combinations should be in non-decreasing order");
            }
        }
    }

    #endregion

    #region GetCombinationsDistinct Tests

    [Test]
    public void GetCombinationsDistinct_Length1_ShouldReturnElements()
    {
        var list = new[] { 1, 2, 3 };
        var combs = list.GetCombinationsDistinct(1).ToList();

        Assert.That(combs.Count, Is.EqualTo(3));
    }

    [Test]
    public void GetCombinationsDistinct_Length2_ShouldGenerate3Combinations()
    {
        // Input: {1,2,3}, 2
        // Output: {1,2} {1,3} {2,3}
        var list = new[] { 1, 2, 3 };
        var combs = list.GetCombinationsDistinct(2).ToList();

        Assert.That(combs.Count, Is.EqualTo(3), "C(3,2) = 3");

        Assert.That(combs[0], Is.EqualTo(new[] { 1, 2 }));
        Assert.That(combs[1], Is.EqualTo(new[] { 1, 3 }));
        Assert.That(combs[2], Is.EqualTo(new[] { 2, 3 }));
    }

    [Test]
    public void GetCombinationsDistinct_ExampleFromDocumentation()
    {
        // Input:  {1,2,3,4}, 2
        // Output: {1,2} {1,3} {1,4} {2,3} {2,4} {3,4}
        var list = new[] { 1, 2, 3, 4 };
        var combs = list.GetCombinationsDistinct(2).ToList();

        Assert.That(combs.Count, Is.EqualTo(6), "C(4,2) = 6");

        // Verify all combinations are strictly increasing
        foreach (var comb in combs)
        {
            var combList = comb.ToList();
            for (int i = 0; i < combList.Count - 1; i++)
            {
                Assert.That(combList[i], Is.LessThan(combList[i + 1]),
                    "Distinct combinations should be strictly increasing");
            }
        }
    }

    [Test]
    public void GetCombinationsDistinct_C5_3_ShouldGenerate10Combinations()
    {
        var list = new[] { 1, 2, 3, 4, 5 };
        var combs = list.GetCombinationsDistinct(3).ToList();

        Assert.That(combs.Count, Is.EqualTo(10), "C(5,3) = 10");
    }

    #endregion

    #region Comparison Tests

    [Test]
    public void ComparePermutationsAndCombinations_RepeatedVsDistinct()
    {
        var list = new[] { 1, 2, 3 };

        var permsRepeated = list.GetPermutationsRepeated(2).ToList();
        var permsDistinct = list.GetPermutationsDistinct(2).ToList();
        var combsRepeated = list.GetCombinationsRepeated(2).ToList();
        var combsDistinct = list.GetCombinationsDistinct(2).ToList();

        // With repetition allows more results
        Assert.That(permsRepeated.Count, Is.GreaterThan(permsDistinct.Count));
        Assert.That(combsRepeated.Count, Is.GreaterThan(combsDistinct.Count));

        // Permutations allow more orderings than combinations
        Assert.That(permsRepeated.Count, Is.GreaterThan(combsRepeated.Count));
        Assert.That(permsDistinct.Count, Is.GreaterThan(combsDistinct.Count));
    }

    [Test]
    public void PermutationsVsCombinations_OrderMatters()
    {
        // Permutations: {1,2} and {2,1} are different
        var list = new[] { 1, 2 };
        var perms = list.GetPermutationsDistinct(2).ToList();
        Assert.That(perms.Count, Is.EqualTo(2), "P(2,2) = 2");

        // Combinations: {1,2} and {2,1} are the same
        var combs = list.GetCombinationsDistinct(2).ToList();
        Assert.That(combs.Count, Is.EqualTo(1), "C(2,2) = 1");
    }

    #endregion

    #region Edge Cases

    [Test]
    public void GetPermutationsRepeated_EmptyList_ShouldReturnEmpty()
    {
        var list = new int[] { };
        var perms = list.GetPermutationsRepeated(2).ToList();

        Assert.That(perms.Count, Is.EqualTo(0));
    }

    [Test]
    public void GetCombinationsDistinct_LengthExceedsListSize_ShouldReturnEmpty()
    {
        var list = new[] { 1, 2 };
        var combs = list.GetCombinationsDistinct(5).ToList();

        Assert.That(combs.Count, Is.EqualTo(0), "Cannot choose 5 from 2");
    }

    [Test]
    public void AllMethods_WorkWithStrings()
    {
        var list = new[] { "A", "B", "C" };

        var perms = list.GetPermutationsDistinct(2).ToList();
        Assert.That(perms.Count, Is.EqualTo(6));

        var combs = list.GetCombinationsDistinct(2).ToList();
        Assert.That(combs.Count, Is.EqualTo(3));
    }

    #endregion
}
