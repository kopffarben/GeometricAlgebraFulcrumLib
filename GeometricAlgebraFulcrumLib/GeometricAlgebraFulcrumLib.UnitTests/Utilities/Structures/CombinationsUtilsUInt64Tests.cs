using System;
using System.Linq;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Combinations;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Utilities.Structures;

/// <summary>
/// Tests for CombinationsUtilsUInt64 - UInt64 bit-pattern based combinations
/// Includes binomial coefficients, combinadic encoding, and Pascal triangle operations
/// </summary>
[TestFixture]
public class CombinationsUtilsUInt64Tests
{
    #region Binomial Coefficient Tests

    [Test]
    public void ComputeBinomialCoefficient_C5_3_ShouldReturn10()
    {
        Assert.That(5.ComputeBinomialCoefficient(3), Is.EqualTo(10UL));
    }

    [Test]
    public void ComputeBinomialCoefficient_C10_5_ShouldReturn252()
    {
        Assert.That(10.ComputeBinomialCoefficient(5), Is.EqualTo(252UL));
    }

    [Test]
    public void ComputeBinomialCoefficient_C4_2_ShouldReturn6()
    {
        Assert.That(4.ComputeBinomialCoefficient(2), Is.EqualTo(6UL));
    }

    [Test]
    public void ComputeBinomialCoefficient_NEqualsK_ShouldReturn1()
    {
        Assert.That(5.ComputeBinomialCoefficient(5), Is.EqualTo(1UL));
        Assert.That(10.ComputeBinomialCoefficient(10), Is.EqualTo(1UL));
    }

    [Test]
    public void ComputeBinomialCoefficient_KEqualsZero_ShouldReturn1()
    {
        Assert.That(5.ComputeBinomialCoefficient(0), Is.EqualTo(1UL));
        Assert.That(100.ComputeBinomialCoefficient(0), Is.EqualTo(1UL));
    }

    [Test]
    public void ComputeBinomialCoefficient_NLessThanK_ShouldReturn0()
    {
        Assert.That(3.ComputeBinomialCoefficient(5), Is.EqualTo(0UL));
    }

    [Test]
    public void ComputeBinomialCoefficient_NegativeN_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => (-5).ComputeBinomialCoefficient(3));
    }

    [Test]
    public void ComputeBinomialCoefficient_NegativeK_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 5.ComputeBinomialCoefficient(-3));
    }

    [Test]
    public void ComputeBinomialCoefficient_Symmetry_ShouldHold()
    {
        // C(n, k) = C(n, n-k)
        Assert.That(10.ComputeBinomialCoefficient(3), Is.EqualTo(10.ComputeBinomialCoefficient(7)));
        Assert.That(20.ComputeBinomialCoefficient(5), Is.EqualTo(20.ComputeBinomialCoefficient(15)));
    }

    [Test]
    public void ComputeTake2BinomialCoefficient_ShouldComputeCorrectly()
    {
        // C(n, 2) = n * (n-1) / 2
        Assert.That(CombinationsUtilsUInt64.ComputeTake2BinomialCoefficient(5), Is.EqualTo(10UL));
        Assert.That(CombinationsUtilsUInt64.ComputeTake2BinomialCoefficient(10), Is.EqualTo(45UL));
        Assert.That(CombinationsUtilsUInt64.ComputeTake2BinomialCoefficient(4), Is.EqualTo(6UL));
    }

    [Test]
    public void ComputeTake2BinomialCoefficient_EdgeCases()
    {
        Assert.That(CombinationsUtilsUInt64.ComputeTake2BinomialCoefficient(0), Is.EqualTo(0UL));
        Assert.That(CombinationsUtilsUInt64.ComputeTake2BinomialCoefficient(1), Is.EqualTo(0UL));
        Assert.That(CombinationsUtilsUInt64.ComputeTake2BinomialCoefficient(2), Is.EqualTo(1UL));
    }

    [Test]
    public void ComputeTake2BinomialCoefficient_NegativeN_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CombinationsUtilsUInt64.ComputeTake2BinomialCoefficient(-5));
    }

    #endregion

    #region GetBinomialCoefficient Tests (with Pascal Triangle)

    [Test]
    public void GetBinomialCoefficient_UsePascalTriangle_SmallValues()
    {
        // For small values, should use cached Pascal Triangle
        Assert.That(5.GetBinomialCoefficient(3), Is.EqualTo(10UL));
        Assert.That(10.GetBinomialCoefficient(5), Is.EqualTo(252UL));
    }

    [Test]
    public void GetBinomialCoefficient_ConsistentWithComputedValues()
    {
        // Should give same results as ComputeBinomialCoefficient
        for (int n = 0; n <= 20; n++)
        {
            for (int k = 0; k <= n; k++)
            {
                var computed = n.ComputeBinomialCoefficient(k);
                var cached = n.GetBinomialCoefficient(k);
                Assert.That(cached, Is.EqualTo(computed),
                    $"C({n},{k}) should be consistent");
            }
        }
    }

    [Test]
    public void GetBinomialCoefficient_NegativeParameters_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => (-5).GetBinomialCoefficient(3));
        Assert.Throws<ArgumentOutOfRangeException>(() => 5.GetBinomialCoefficient(-3));
    }

    #endregion

    #region GetMaxBinomialCoefficient Tests

    [Test]
    public void GetMaxBinomialCoefficient_EvenN_ShouldReturnCenterValue()
    {
        // For n=10, max is C(10, 5) = 252
        Assert.That(10.GetMaxBinomialCoefficient(), Is.EqualTo(252UL));
    }

    [Test]
    public void GetMaxBinomialCoefficient_OddN_ShouldReturnCenterValue()
    {
        // For n=9, max is C(9, 4) = C(9, 5) = 126
        Assert.That(9.GetMaxBinomialCoefficient(), Is.EqualTo(126UL));
    }

    [Test]
    public void GetMaxBinomialCoefficient_SmallValues()
    {
        Assert.That(0.GetMaxBinomialCoefficient(), Is.EqualTo(1UL));
        Assert.That(1.GetMaxBinomialCoefficient(), Is.EqualTo(1UL));
        Assert.That(2.GetMaxBinomialCoefficient(), Is.EqualTo(2UL));
        Assert.That(3.GetMaxBinomialCoefficient(), Is.EqualTo(3UL));
        Assert.That(4.GetMaxBinomialCoefficient(), Is.EqualTo(6UL));
    }

    #endregion

    #region GetBinomialCoefficients Tests

    [Test]
    public void GetBinomialCoefficients_ShouldReturnFullRow()
    {
        // Row 4: 1, 4, 6, 4, 1
        var row = 4.GetBinomialCoefficients();

        Assert.That(row.Count, Is.EqualTo(5), "Row n has n+1 elements");
        Assert.That(row[0], Is.EqualTo(1UL));
        Assert.That(row[1], Is.EqualTo(4UL));
        Assert.That(row[2], Is.EqualTo(6UL));
        Assert.That(row[3], Is.EqualTo(4UL));
        Assert.That(row[4], Is.EqualTo(1UL));
    }

    [Test]
    public void GetBinomialCoefficients_SymmetricRow()
    {
        var row = 10.GetBinomialCoefficients();

        // Row should be symmetric
        for (int i = 0; i <= 10; i++)
        {
            Assert.That(row[i], Is.EqualTo(row[10 - i]),
                $"Row should be symmetric at position {i}");
        }
    }

    [Test]
    public void GetBinomialCoefficients_Row0_ShouldReturn1()
    {
        var row = 0.GetBinomialCoefficients();

        Assert.That(row.Count, Is.EqualTo(1));
        Assert.That(row[0], Is.EqualTo(1UL));
    }

    #endregion

    #region Combinadic Encoding Tests

    [Test]
    public void IndexToCombinadic_Simple_ShouldReturnCorrectCombination()
    {
        // For C(5, 3), index 0 should give {0, 1, 2}
        var combinadic = 0UL.IndexToCombinadic(3).ToArray();

        Assert.That(combinadic.Length, Is.EqualTo(3));
        Assert.That(combinadic, Is.EqualTo(new[] { 2, 1, 0 })); // Reverse lexicographic order
    }

    [Test]
    public void IndexToCombinadic_Index1_ShouldReturnNext()
    {
        // For C(5, 3), index 1 should give {0, 1, 3}
        var combinadic = 1UL.IndexToCombinadic(3).ToArray();

        Assert.That(combinadic, Is.EqualTo(new[] { 3, 1, 0 }));
    }

    [Test]
    public void IndexToCombinadic_ZeroDigits_ShouldReturnEmpty()
    {
        var combinadic = 0UL.IndexToCombinadic(0).ToArray();

        Assert.That(combinadic.Length, Is.EqualTo(0));
    }

    [Test]
    public void IndexToCombinadic_MaxDigits_ShouldWork()
    {
        var maxDigits = CombinationsUtilsUInt64.MaxSetSize;
        var combinadic = 0UL.IndexToCombinadic(maxDigits).ToArray();

        Assert.That(combinadic.Length, Is.EqualTo(maxDigits));
    }

    [Test]
    public void IndexToCombinadic_InvalidDigitsCount_ShouldThrow()
    {
        Assert.Throws<InvalidOperationException>(() =>
            0UL.IndexToCombinadic(-1).ToArray());

        Assert.Throws<InvalidOperationException>(() =>
            0UL.IndexToCombinadic(65).ToArray());
    }

    #endregion

    #region Combinadic Pattern Tests

    [Test]
    public void IndexToCombinadicPattern_Simple_ShouldReturnBitPattern()
    {
        // For C(5, 3), index 0 should give bits 0, 1, 2 set
        var pattern = 0UL.IndexToCombinadicPattern(3);

        // Pattern should have bits 0, 1, 2 set = 0b111 = 7
        Assert.That(pattern, Is.EqualTo(0b111UL));
    }

    [Test]
    public void IndexToCombinadicPattern_Index1_ShouldReturnCorrectPattern()
    {
        // For C(5, 3), index 1 should give bits 0, 1, 3 set
        var pattern = 1UL.IndexToCombinadicPattern(3);

        // Pattern should have bits 0, 1, 3 set = 0b1011 = 11
        Assert.That(pattern, Is.EqualTo(0b1011UL));
    }

    [Test]
    public void IndexToCombinadicPattern_ZeroDigits_ShouldReturn0()
    {
        var pattern = 0UL.IndexToCombinadicPattern(0);

        Assert.That(pattern, Is.EqualTo(0UL));
    }

    [Test]
    public void IndexToCombinadicPattern_MaxDigits_ShouldReturnAllBitsSet()
    {
        var maxDigits = CombinationsUtilsUInt64.MaxSetSize;
        var pattern = 0UL.IndexToCombinadicPattern(maxDigits);

        Assert.That(pattern, Is.EqualTo(ulong.MaxValue));
    }

    #endregion

    #region CombinadicPatternToIndex Tests

    [Test]
    public void CombinadicPatternToIndex_Simple_ShouldReturnIndex()
    {
        // Pattern 0b111 (bits 0, 1, 2) should give index 0 for C(5, 3)
        var index = 0b111UL.CombinadicPatternToIndex();

        Assert.That(index, Is.EqualTo(0UL));
    }

    [Test]
    public void CombinadicPatternToIndex_Pattern11_ShouldReturnIndex1()
    {
        // Pattern 0b1011 (bits 0, 1, 3) should give index 1
        var index = 0b1011UL.CombinadicPatternToIndex();

        Assert.That(index, Is.EqualTo(1UL));
    }

    [Test]
    public void CombinadicPatternToIndex_ZeroPattern_ShouldReturn0()
    {
        Assert.That(0UL.CombinadicPatternToIndex(), Is.EqualTo(0UL));
    }

    [Test]
    public void CombinadicPatternToIndex_WithOnesCount_ShouldReturnBoth()
    {
        var pattern = 0b1011UL;
        pattern.CombinadicPatternToIndex(out int onesCount, out ulong index);

        Assert.That(onesCount, Is.EqualTo(3), "Should count 3 bits");
        Assert.That(index, Is.EqualTo(1UL));
    }

    #endregion

    #region Round-Trip Tests

    [Test]
    public void CombinadicRoundTrip_IndexToPatternToIndex_ShouldBeIdentity()
    {
        // Test round-trip for several combinations
        for (int digits = 1; digits <= 5; digits++)
        {
            var maxIndex = (int)CombinationsUtilsUInt64.MaxSetSize.GetBinomialCoefficient(digits);
            var testCount = Math.Min(10, maxIndex);

            for (ulong index = 0; index < (ulong)testCount; index++)
            {
                var pattern = index.IndexToCombinadicPattern(digits);
                var recovered = pattern.CombinadicPatternToIndex();

                Assert.That(recovered, Is.EqualTo(index),
                    $"Round-trip failed for index={index}, digits={digits}");
            }
        }
    }

    [Test]
    public void CombinadicRoundTrip_PatternToIndexToPattern_ShouldBeIdentity()
    {
        // Test patterns with different numbers of bits set
        var patterns = new ulong[]
        {
            0b111UL,      // 3 bits
            0b1011UL,     // 3 bits
            0b10101UL,    // 3 bits
            0b111000UL,   // 3 bits
            0b1111UL,     // 4 bits
        };

        foreach (var pattern in patterns)
        {
            var index = pattern.CombinadicPatternToIndex();
            pattern.CombinadicPatternToIndex(out int onesCount, out _);

            var recovered = index.IndexToCombinadicPattern(onesCount);

            Assert.That(recovered, Is.EqualTo(pattern),
                $"Round-trip failed for pattern={pattern:X}");
        }
    }

    #endregion

    #region Pascal Identity Tests

    [Test]
    public void PascalIdentity_ShouldHold()
    {
        // C(n, k) = C(n-1, k-1) + C(n-1, k)
        for (int n = 2; n <= 20; n++)
        {
            for (int k = 1; k < n; k++)
            {
                var left = n.GetBinomialCoefficient(k);
                var right = (n - 1).GetBinomialCoefficient(k - 1) + (n - 1).GetBinomialCoefficient(k);

                Assert.That(left, Is.EqualTo(right),
                    $"Pascal identity failed for C({n},{k})");
            }
        }
    }

    #endregion

    #region GetMaximalRowIndex Tests

    [Test]
    public void GetMaximalRowIndex_ShouldFindLargestRow()
    {
        // For value=10, columnIndex=3, should find largest n where C(n,3) <= 10
        // C(5,3) = 10, C(6,3) = 20, so max is 5
        var maxRow = CombinationsUtilsUInt64.GetMaximalRowIndex(10, 3);

        Assert.That(maxRow, Is.EqualTo(5));
    }

    [Test]
    public void GetMaximalRowIndex_EdgeCase_ExactMatch()
    {
        // C(5,3) = 10
        var maxRow = CombinationsUtilsUInt64.GetMaximalRowIndex(10, 3);

        Assert.That(maxRow, Is.EqualTo(5));
    }

    #endregion

    #region MaxSetSize Test

    [Test]
    public void MaxSetSize_ShouldBe64()
    {
        // MaxSetSize should be 64 (PascalTriangle has 65 rows, indexed 0-64)
        // This allows for all 64 bits of ulong to be used
        Assert.That(CombinationsUtilsUInt64.MaxSetSize, Is.EqualTo(64));
    }

    #endregion
}
