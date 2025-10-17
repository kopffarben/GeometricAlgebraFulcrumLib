using System.Collections.Generic;
using GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Utilities.Structures;

/// <summary>
/// Tests for IndexSet - fundamental data structure for basis blade indexing in GA
/// IndexSet represents a sorted set of non-negative integers, optimized for small sets (≤64 elements as bit pattern)
/// </summary>
[TestFixture]
public class IndexSetTests
{
    #region Creation Tests

    [Test]
    public void EmptySet_ShouldHaveZeroCount()
    {
        var emptySet = IndexSet.EmptySet;

        Assert.That(emptySet.Count, Is.EqualTo(0));
        Assert.That(emptySet.IsEmptySet, Is.True);
    }

    [Test]
    public void CreateUnit_ShouldCreateSingletonSet()
    {
        var set0 = IndexSet.CreateUnit(0);
        var set5 = IndexSet.CreateUnit(5);

        Assert.That(set0.Count, Is.EqualTo(1));
        Assert.That(set0[0], Is.EqualTo(0));
        Assert.That(set0.IsUnitSet, Is.True);

        Assert.That(set5.Count, Is.EqualTo(1));
        Assert.That(set5[0], Is.EqualTo(5));
        Assert.That(set5.IsUnitSet, Is.True);
    }

    [Test]
    public void CreatePair_ShouldCreateTwoElementSet()
    {
        var pair = IndexSet.CreatePair(2, 5);

        Assert.That(pair.Count, Is.EqualTo(2));
        Assert.That(pair[0], Is.EqualTo(2));
        Assert.That(pair[1], Is.EqualTo(5));
        Assert.That(pair.IsPairSet, Is.True);
    }

    [Test]
    public void CreatePair_RequiresPreSortedParameters()
    {
        // Note: CreatePair does NOT sort automatically - it expects index1 < index2
        // This is enforced by Debug.Assert in the implementation
        var pair = IndexSet.CreatePair(2, 5); // Must be in sorted order

        Assert.That(pair[0], Is.EqualTo(2));
        Assert.That(pair[1], Is.EqualTo(5));
    }

    [Test]
    public void CreateTriplet_ShouldCreateThreeElementSet()
    {
        var triplet = IndexSet.CreateTriplet(1, 3, 5);

        Assert.That(triplet.Count, Is.EqualTo(3));
        Assert.That(triplet[0], Is.EqualTo(1));
        Assert.That(triplet[1], Is.EqualTo(3));
        Assert.That(triplet[2], Is.EqualTo(5));
        Assert.That(triplet.IsTripletSet, Is.True);
    }

    [Test]
    public void CreateFromArray_ShouldCreateSet()
    {
        var set = IndexSet.Create(1, 3, 5, 7);

        Assert.That(set.Count, Is.EqualTo(4));
        Assert.That(set[0], Is.EqualTo(1));
        Assert.That(set[3], Is.EqualTo(7));
    }

    [Test]
    public void CreateFromArray_ShouldRemoveDuplicates()
    {
        var set = IndexSet.Create(1, 3, 3, 5, 5, 7);

        Assert.That(set.Count, Is.EqualTo(4), "Duplicates should be removed");
        Assert.That(set[0], Is.EqualTo(1));
        Assert.That(set[1], Is.EqualTo(3));
        Assert.That(set[2], Is.EqualTo(5));
        Assert.That(set[3], Is.EqualTo(7));
    }

    [Test]
    public void CreateFromArray_ShouldSortElements()
    {
        var set = IndexSet.Create(7, 3, 1, 5); // Unsorted

        Assert.That(set[0], Is.EqualTo(1), "Should be sorted");
        Assert.That(set[1], Is.EqualTo(3));
        Assert.That(set[2], Is.EqualTo(5));
        Assert.That(set[3], Is.EqualTo(7));
    }

    [Test]
    public void CreateDense_ShouldCreateConsecutiveIndices()
    {
        var dense = IndexSet.CreateDense(5); // [0, 1, 2, 3, 4]

        Assert.That(dense.Count, Is.EqualTo(5));
        Assert.That(dense.IsDenseSet, Is.True);
        Assert.That(dense[0], Is.EqualTo(0));
        Assert.That(dense[4], Is.EqualTo(4));
    }

    [Test]
    public void CreateDenseWithStart_ShouldCreateConsecutiveIndices()
    {
        var dense = IndexSet.CreateDense(3, 4); // [3, 4, 5, 6]

        Assert.That(dense.Count, Is.EqualTo(4));
        Assert.That(dense.IsDenseSet, Is.True);
        Assert.That(dense[0], Is.EqualTo(3));
        Assert.That(dense[3], Is.EqualTo(6));
    }

    [Test]
    public void CreateFromUInt64Pattern_ShouldConvertBitPattern()
    {
        var pattern = 0b1010UL; // Bits 1 and 3 set
        var set = IndexSet.CreateFromUInt64Pattern(pattern);

        Assert.That(set.Count, Is.EqualTo(2));
        Assert.That(set[0], Is.EqualTo(1));
        Assert.That(set[1], Is.EqualTo(3));
    }

    #endregion

    #region Property Tests

    [Test]
    public void FirstIndex_ShouldReturnLowestIndex()
    {
        var set = IndexSet.Create(5, 2, 8);
        Assert.That(set.FirstIndex, Is.EqualTo(2));
    }

    [Test]
    public void LastIndex_ShouldReturnHighestIndex()
    {
        var set = IndexSet.Create(5, 2, 8);
        Assert.That(set.LastIndex, Is.EqualTo(8));
    }

    [Test]
    public void IsUInt64Set_SmallSet_ShouldBeTrue()
    {
        var smallSet = IndexSet.Create(1, 5, 10);
        Assert.That(smallSet.IsUInt64Set, Is.True);
    }

    [Test]
    public void IsSparseSet_ShouldDetectGaps()
    {
        var sparse = IndexSet.Create(1, 5, 10); // Gaps between elements
        Assert.That(sparse.IsSparseSet, Is.True);
        Assert.That(sparse.IsDenseSet, Is.False);
    }

    [Test]
    public void IsEmptySet_ShouldDetectEmpty()
    {
        Assert.That(IndexSet.EmptySet.IsEmptySet, Is.True);
        Assert.That(IndexSet.CreateUnit(0).IsEmptySet, Is.False);
    }

    #endregion

    #region Operators Tests

    [Test]
    public void UnionOperator_ShouldCombineSets()
    {
        var set1 = IndexSet.Create(1, 2, 3);
        var set2 = IndexSet.Create(3, 4, 5);

        var union = set1 | set2;

        Assert.That(union.Count, Is.EqualTo(5));
        Assert.That(union.SetContains(1), Is.True);
        Assert.That(union.SetContains(5), Is.True);
    }

    [Test]
    public void IntersectionOperator_ShouldFindCommonElements()
    {
        var set1 = IndexSet.Create(1, 2, 3, 4);
        var set2 = IndexSet.Create(3, 4, 5, 6);

        var intersection = set1 & set2;

        Assert.That(intersection.Count, Is.EqualTo(2));
        Assert.That(intersection.SetContains(3), Is.True);
        Assert.That(intersection.SetContains(4), Is.True);
        Assert.That(intersection.SetContains(1), Is.False);
    }

    [Test]
    public void XorOperator_ShouldComputeSymmetricDifference()
    {
        var set1 = IndexSet.Create(1, 2, 3);
        var set2 = IndexSet.Create(2, 3, 4);

        var xor = set1 ^ set2;

        Assert.That(xor.Count, Is.EqualTo(2));
        Assert.That(xor.SetContains(1), Is.True);
        Assert.That(xor.SetContains(4), Is.True);
        Assert.That(xor.SetContains(2), Is.False); // Common elements removed
        Assert.That(xor.SetContains(3), Is.False);
    }

    [Test]
    public void DifferenceOperator_ShouldRemoveElements()
    {
        var set1 = IndexSet.Create(1, 2, 3, 4);
        var set2 = IndexSet.Create(2, 3);

        var difference = set1 - set2;

        Assert.That(difference.Count, Is.EqualTo(2));
        Assert.That(difference.SetContains(1), Is.True);
        Assert.That(difference.SetContains(4), Is.True);
        Assert.That(difference.SetContains(2), Is.False);
    }

    [Test]
    public void ShiftLeftOperator_ShouldIncreaseAllIndices()
    {
        var set = IndexSet.Create(1, 2, 3);

        var shifted = set << 2; // Shift left by 2

        Assert.That(shifted.Count, Is.EqualTo(3));
        Assert.That(shifted[0], Is.EqualTo(3)); // 1 + 2
        Assert.That(shifted[1], Is.EqualTo(4)); // 2 + 2
        Assert.That(shifted[2], Is.EqualTo(5)); // 3 + 2
    }

    [Test]
    public void ShiftRightOperator_ShouldDecreaseAllIndices()
    {
        var set = IndexSet.Create(5, 6, 7);

        var shifted = set >> 2; // Shift right by 2

        Assert.That(shifted.Count, Is.EqualTo(3));
        Assert.That(shifted[0], Is.EqualTo(3)); // 5 - 2
        Assert.That(shifted[1], Is.EqualTo(4)); // 6 - 2
        Assert.That(shifted[2], Is.EqualTo(5)); // 7 - 2
    }

    #endregion

    #region Equality and Comparison Tests

    [Test]
    public void Equals_IdenticalSets_ShouldBeEqual()
    {
        var set1 = IndexSet.Create(1, 2, 3);
        var set2 = IndexSet.Create(1, 2, 3);

        Assert.That(set1.Equals(set2), Is.True);
        Assert.That(set1 == set2, Is.True);
    }

    [Test]
    public void Equals_DifferentSets_ShouldNotBeEqual()
    {
        var set1 = IndexSet.Create(1, 2, 3);
        var set2 = IndexSet.Create(1, 2, 4);

        Assert.That(set1.Equals(set2), Is.False);
        Assert.That(set1 != set2, Is.True);
    }

    [Test]
    public void CompareTo_ShouldCompareLexicographically()
    {
        var set1 = IndexSet.Create(1, 2, 3);
        var set2 = IndexSet.Create(1, 2, 4);

        Assert.That(set1.CompareTo(set2), Is.LessThan(0), "set1 should be less than set2");
        Assert.That(set2.CompareTo(set1), Is.GreaterThan(0), "set2 should be greater than set1");
    }

    [Test]
    public void ComparisonOperators_ShouldWork()
    {
        var set1 = IndexSet.Create(1, 2, 3);
        var set2 = IndexSet.Create(1, 2, 4);

        Assert.That(set1 < set2, Is.True);
        Assert.That(set2 > set1, Is.True);
        Assert.That(set1 <= set2, Is.True);
        Assert.That(set1 <= set1, Is.True);
    }

    #endregion

    #region Contains and Indexing Tests

    [Test]
    public void Contains_ExistingElement_ShouldReturnTrue()
    {
        var set = IndexSet.Create(1, 3, 5, 7);

        Assert.That(set.SetContains(3), Is.True);
        Assert.That(set.SetContains(7), Is.True);
    }

    [Test]
    public void Contains_NonExistingElement_ShouldReturnFalse()
    {
        var set = IndexSet.Create(1, 3, 5, 7);

        Assert.That(set.SetContains(2), Is.False);
        Assert.That(set.SetContains(4), Is.False);
        Assert.That(set.SetContains(10), Is.False);
    }

    [Test]
    public void Indexer_ShouldAccessElementsByPosition()
    {
        var set = IndexSet.Create(5, 10, 15);

        Assert.That(set[0], Is.EqualTo(5));
        Assert.That(set[1], Is.EqualTo(10));
        Assert.That(set[2], Is.EqualTo(15));
    }

    [Test]
    public void Count_ShouldReturnNumberOfElements()
    {
        Assert.That(IndexSet.EmptySet.Count, Is.EqualTo(0));
        Assert.That(IndexSet.CreateUnit(5).Count, Is.EqualTo(1));
        Assert.That(IndexSet.Create(1, 2, 3, 4, 5).Count, Is.EqualTo(5));
    }

    #endregion

    #region Cast Operations Tests

    [Test]
    public void CastToUInt64_SmallSet_ShouldConvertToBitPattern()
    {
        var set = IndexSet.Create(1, 3, 5);
        var pattern = (ulong)set;

        // Pattern should have bits 1, 3, 5 set: 0b101010 = 42
        Assert.That(pattern, Is.EqualTo(0b101010UL));
    }

    [Test]
    public void CastFromUInt64_ShouldCreateSet()
    {
        var pattern = 0b101UL; // Bits 0 and 2 set
        var set = (IndexSet)pattern;

        Assert.That(set.Count, Is.EqualTo(2));
        Assert.That(set[0], Is.EqualTo(0));
        Assert.That(set[1], Is.EqualTo(2));
    }

    #endregion

    #region Enumeration Tests

    [Test]
    public void GetEnumerator_ShouldIterateAllElements()
    {
        var set = IndexSet.Create(2, 4, 6, 8);
        var elements = new List<int>();

        foreach (var element in set)
            elements.Add(element);

        Assert.That(elements.Count, Is.EqualTo(4));
        Assert.That(elements[0], Is.EqualTo(2));
        Assert.That(elements[3], Is.EqualTo(8));
    }

    [Test]
    public void ToList_ShouldReturnSortedList()
    {
        var set = IndexSet.Create(7, 3, 1, 5);
        var list = new List<int>();

        foreach (var item in set)
            list.Add(item);

        Assert.That(list, Is.EqualTo(new[] { 1, 3, 5, 7 }));
    }

    #endregion
}
