using System;
using GeometricAlgebraFulcrumLib.Utilities.Structures.BitManipulation;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Utilities.Structures;

/// <summary>
/// Tests for UInt64BitUtils - fundamental bit manipulation utilities
/// Critical for Geometric Algebra basis blade operations and ID calculations
/// </summary>
[TestFixture]
public class UInt64BitUtilsTests
{
    #region Basic Properties

    [Test]
    public void BitPositionConstants_ShouldHaveCorrectValues()
    {
        Assert.That(UInt64BitUtils.MinBitPosition, Is.EqualTo(0));
        Assert.That(UInt64BitUtils.MaxBitPosition, Is.EqualTo(63));
        Assert.That(UInt64BitUtils.MaxBitPatternSize, Is.EqualTo(64));
    }

    #endregion

    #region IsOdd / IsEven Tests

    [Test]
    public void IsOdd_OddNumbers_ShouldReturnTrue()
    {
        Assert.That(1UL.IsOdd(), Is.True);
        Assert.That(3UL.IsOdd(), Is.True);
        Assert.That(5UL.IsOdd(), Is.True);
        Assert.That(127UL.IsOdd(), Is.True);
        Assert.That(ulong.MaxValue.IsOdd(), Is.True); // All bits set
    }

    [Test]
    public void IsOdd_EvenNumbers_ShouldReturnFalse()
    {
        Assert.That(0UL.IsOdd(), Is.False);
        Assert.That(2UL.IsOdd(), Is.False);
        Assert.That(4UL.IsOdd(), Is.False);
        Assert.That(128UL.IsOdd(), Is.False);
    }

    [Test]
    public void IsEven_EvenNumbers_ShouldReturnTrue()
    {
        Assert.That(0UL.IsEven(), Is.True);
        Assert.That(2UL.IsEven(), Is.True);
        Assert.That(4UL.IsEven(), Is.True);
        Assert.That(128UL.IsEven(), Is.True);
    }

    [Test]
    public void IsEven_OddNumbers_ShouldReturnFalse()
    {
        Assert.That(1UL.IsEven(), Is.False);
        Assert.That(3UL.IsEven(), Is.False);
        Assert.That(127UL.IsEven(), Is.False);
    }

    #endregion

    #region IsBasicPattern Tests (Power of 2)

    [Test]
    public void IsBasicPattern_PowerOfTwo_ShouldReturnTrue()
    {
        Assert.That(1UL.IsBasicPattern(), Is.True);      // 2^0
        Assert.That(2UL.IsBasicPattern(), Is.True);      // 2^1
        Assert.That(4UL.IsBasicPattern(), Is.True);      // 2^2
        Assert.That(8UL.IsBasicPattern(), Is.True);      // 2^3
        Assert.That(16UL.IsBasicPattern(), Is.True);     // 2^4
        Assert.That(1024UL.IsBasicPattern(), Is.True);   // 2^10
        Assert.That((1UL << 63).IsBasicPattern(), Is.True); // 2^63
    }

    [Test]
    public void IsBasicPattern_NonPowerOfTwo_ShouldReturnFalse()
    {
        Assert.That(0UL.IsBasicPattern(), Is.False);   // Zero
        Assert.That(3UL.IsBasicPattern(), Is.False);   // 11 binary
        Assert.That(5UL.IsBasicPattern(), Is.False);   // 101 binary
        Assert.That(6UL.IsBasicPattern(), Is.False);   // 110 binary
        Assert.That(7UL.IsBasicPattern(), Is.False);   // 111 binary
        Assert.That(15UL.IsBasicPattern(), Is.False);  // 1111 binary
    }

    [Test]
    public void IsZeroOrBasicPattern_ShouldIncludeZero()
    {
        Assert.That(0UL.IsZeroOrBasicPattern(), Is.True);
        Assert.That(1UL.IsZeroOrBasicPattern(), Is.True);
        Assert.That(2UL.IsZeroOrBasicPattern(), Is.True);
        Assert.That(3UL.IsZeroOrBasicPattern(), Is.False);
    }

    #endregion

    #region FirstOneBitPosition Tests

    [Test]
    public void FirstOneBitPosition_SingleBit_ShouldReturnCorrectPosition()
    {
        Assert.That(1UL.FirstOneBitPosition(), Is.EqualTo(0));
        Assert.That(2UL.FirstOneBitPosition(), Is.EqualTo(1));
        Assert.That(4UL.FirstOneBitPosition(), Is.EqualTo(2));
        Assert.That(8UL.FirstOneBitPosition(), Is.EqualTo(3));
        Assert.That((1UL << 10).FirstOneBitPosition(), Is.EqualTo(10));
        Assert.That((1UL << 63).FirstOneBitPosition(), Is.EqualTo(63));
    }

    [Test]
    public void FirstOneBitPosition_MultipleBits_ShouldReturnLowestPosition()
    {
        Assert.That(3UL.FirstOneBitPosition(), Is.EqualTo(0));    // 11 binary
        Assert.That(5UL.FirstOneBitPosition(), Is.EqualTo(0));    // 101 binary
        Assert.That(6UL.FirstOneBitPosition(), Is.EqualTo(1));    // 110 binary
        Assert.That(12UL.FirstOneBitPosition(), Is.EqualTo(2));   // 1100 binary
        Assert.That(0xF0UL.FirstOneBitPosition(), Is.EqualTo(4)); // 11110000 binary
    }

    [Test]
    public void FirstOneBitPosition_Zero_ShouldReturnMinusOne()
    {
        Assert.That(0UL.FirstOneBitPosition(), Is.EqualTo(-1));
    }

    [Test]
    public void FirstOneBitPosition_Naive_ShouldMatchOptimized()
    {
        Assert.That(UInt64BitUtils.Naive.FirstOneBitPosition(5UL), Is.EqualTo(5UL.FirstOneBitPosition()));
        Assert.That(UInt64BitUtils.Naive.FirstOneBitPosition(0xF0UL), Is.EqualTo(0xF0UL.FirstOneBitPosition()));
        Assert.That(UInt64BitUtils.Naive.FirstOneBitPosition(0UL), Is.EqualTo(0UL.FirstOneBitPosition()));
    }

    #endregion

    #region LastOneBitPosition Tests

    [Test]
    public void LastOneBitPosition_SingleBit_ShouldReturnCorrectPosition()
    {
        Assert.That(1UL.LastOneBitPosition(), Is.EqualTo(0));
        Assert.That(2UL.LastOneBitPosition(), Is.EqualTo(1));
        Assert.That(4UL.LastOneBitPosition(), Is.EqualTo(2));
        Assert.That(8UL.LastOneBitPosition(), Is.EqualTo(3));
        Assert.That((1UL << 10).LastOneBitPosition(), Is.EqualTo(10));
        Assert.That((1UL << 63).LastOneBitPosition(), Is.EqualTo(63));
    }

    [Test]
    public void LastOneBitPosition_MultipleBits_ShouldReturnHighestPosition()
    {
        Assert.That(3UL.LastOneBitPosition(), Is.EqualTo(1));     // 11 binary
        Assert.That(5UL.LastOneBitPosition(), Is.EqualTo(2));     // 101 binary
        Assert.That(7UL.LastOneBitPosition(), Is.EqualTo(2));     // 111 binary
        Assert.That(0xFUL.LastOneBitPosition(), Is.EqualTo(3));   // 1111 binary
        Assert.That(0xF0UL.LastOneBitPosition(), Is.EqualTo(7));  // 11110000 binary
    }

    [Test]
    public void LastOneBitPosition_Zero_ShouldReturnMinusOne()
    {
        Assert.That(0UL.LastOneBitPosition(), Is.EqualTo(-1));
    }

    [Test]
    public void LastOneBitPosition_Naive_ShouldMatchOptimized()
    {
        Assert.That(UInt64BitUtils.Naive.LastOneBitPosition(5UL), Is.EqualTo(5UL.LastOneBitPosition()));
        Assert.That(UInt64BitUtils.Naive.LastOneBitPosition(0xF0UL), Is.EqualTo(0xF0UL.LastOneBitPosition()));
        Assert.That(UInt64BitUtils.Naive.LastOneBitPosition(0UL), Is.EqualTo(0UL.LastOneBitPosition()));
    }

    #endregion

    #region FirstLastOneBitPosition Tests

    [Test]
    public void FirstLastOneBitPosition_ShouldReturnBoth()
    {
        var result1 = 5UL.FirstLastOneBitPosition(); // 101 binary
        Assert.That(result1.Item1, Is.EqualTo(0), "First bit should be at position 0");
        Assert.That(result1.Item2, Is.EqualTo(2), "Last bit should be at position 2");

        var result2 = 0xF0UL.FirstLastOneBitPosition(); // 11110000 binary
        Assert.That(result2.Item1, Is.EqualTo(4), "First bit should be at position 4");
        Assert.That(result2.Item2, Is.EqualTo(7), "Last bit should be at position 7");
    }

    [Test]
    public void FirstLastOneBitPosition_SingleBit_ShouldReturnSamePosition()
    {
        var result = 16UL.FirstLastOneBitPosition(); // Single bit at position 4
        Assert.That(result.Item1, Is.EqualTo(4));
        Assert.That(result.Item2, Is.EqualTo(4));
    }

    #endregion

    #region CountOnes Tests

    [Test]
    public void CountOnes_ShouldReturnCorrectCount()
    {
        Assert.That(0UL.CountOnes(), Is.EqualTo(0));
        Assert.That(1UL.CountOnes(), Is.EqualTo(1));
        Assert.That(3UL.CountOnes(), Is.EqualTo(2));  // 11 binary
        Assert.That(7UL.CountOnes(), Is.EqualTo(3));  // 111 binary
        Assert.That(0xFUL.CountOnes(), Is.EqualTo(4)); // 1111 binary
        Assert.That(0xFFUL.CountOnes(), Is.EqualTo(8)); // 11111111 binary
    }

    [Test]
    public void CountOnes_AllBitsSet_ShouldReturn64()
    {
        Assert.That(ulong.MaxValue.CountOnes(), Is.EqualTo(64));
    }

    [Test]
    public void CountOnes_AlternatingBits_ShouldReturn32()
    {
        // 0xAAAAAAAAAAAAAAAA = 1010101010...10 (32 ones)
        Assert.That(0xAAAAAAAAAAAAAAAAUL.CountOnes(), Is.EqualTo(32));
        // 0x5555555555555555 = 0101010101...01 (32 ones)
        Assert.That(0x5555555555555555UL.CountOnes(), Is.EqualTo(32));
    }

    [Test]
    public void CountOnesEquals_ShouldCheckCount()
    {
        Assert.That(0UL.CountOnesEquals(0), Is.True);
        Assert.That(7UL.CountOnesEquals(3), Is.True);
        Assert.That(7UL.CountOnesEquals(2), Is.False);
        Assert.That(0xFUL.CountOnesEquals(4), Is.True);
    }

    #endregion

    #region IsOneAt / IsZeroAt Tests

    [Test]
    public void IsOneAt_ShouldDetectSetBits()
    {
        var pattern = 5UL; // 101 binary

        Assert.That(pattern.IsOneAt(0), Is.True);
        Assert.That(pattern.IsOneAt(1), Is.False);
        Assert.That(pattern.IsOneAt(2), Is.True);
        Assert.That(pattern.IsOneAt(3), Is.False);
    }

    [Test]
    public void IsZeroAt_ShouldDetectUnsetBits()
    {
        var pattern = 5UL; // 101 binary

        Assert.That(pattern.IsZeroAt(0), Is.False);
        Assert.That(pattern.IsZeroAt(1), Is.True);
        Assert.That(pattern.IsZeroAt(2), Is.False);
        Assert.That(pattern.IsZeroAt(3), Is.True);
    }

    #endregion

    #region PatternToMask Tests

    [Test]
    public void PatternToMask_ShouldCreateMaskWithAllLowerBitsSet()
    {
        // PatternToMask creates a mask with all bits set up to the highest bit in pattern
        Assert.That(1UL.PatternToMask(), Is.EqualTo(1UL));     // 1 -> 1
        Assert.That(2UL.PatternToMask(), Is.EqualTo(3UL));     // 10 -> 11
        Assert.That(4UL.PatternToMask(), Is.EqualTo(7UL));     // 100 -> 111
        Assert.That(8UL.PatternToMask(), Is.EqualTo(15UL));    // 1000 -> 1111
        Assert.That(0xFUL.PatternToMask(), Is.EqualTo(0xFUL)); // 1111 -> 1111
    }

    [Test]
    public void PatternToMask_Naive_ShouldMatchOptimized()
    {
        Assert.That(UInt64BitUtils.Naive.PatternToMask(5UL), Is.EqualTo(5UL.PatternToMask()));
        Assert.That(UInt64BitUtils.Naive.PatternToMask(8UL), Is.EqualTo(8UL.PatternToMask()));
    }

    #endregion

    #region SetBit Operations Tests

    [Test]
    public void SetBitToOneAt_ShouldSetSpecificBit()
    {
        Assert.That(0UL.SetBitToOneAt(0), Is.EqualTo(1UL));
        Assert.That(0UL.SetBitToOneAt(1), Is.EqualTo(2UL));
        Assert.That(0UL.SetBitToOneAt(2), Is.EqualTo(4UL));
        Assert.That(1UL.SetBitToOneAt(1), Is.EqualTo(3UL)); // 01 -> 11
    }

    [Test]
    public void SetBitToZeroAt_ShouldClearSpecificBit()
    {
        Assert.That(1UL.SetBitToZeroAt(0), Is.EqualTo(0UL));
        Assert.That(3UL.SetBitToZeroAt(0), Is.EqualTo(2UL)); // 11 -> 10
        Assert.That(3UL.SetBitToZeroAt(1), Is.EqualTo(1UL)); // 11 -> 01
        Assert.That(7UL.SetBitToZeroAt(1), Is.EqualTo(5UL)); // 111 -> 101
    }

    [Test]
    public void SetBitsToZeroAt_MultiplePositions_ShouldClearAllSpecified()
    {
        var pattern = 0xFUL; // 1111 binary
        var result = pattern.SetBitsToZeroAt(0, 2);

        // Clear bits at positions 0 and 2: 1111 -> 1010 = 10
        Assert.That(result, Is.EqualTo(10UL));
    }

    #endregion

    #region Log2Ceiling Tests

    [Test]
    public void Log2Ceiling_PowerOfTwo_ShouldReturnExactLog()
    {
        Assert.That(1UL.Log2Ceiling(), Is.EqualTo(0));   // log2(1) = 0
        Assert.That(2UL.Log2Ceiling(), Is.EqualTo(1));   // log2(2) = 1
        Assert.That(4UL.Log2Ceiling(), Is.EqualTo(2));   // log2(4) = 2
        Assert.That(8UL.Log2Ceiling(), Is.EqualTo(3));   // log2(8) = 3
        Assert.That(16UL.Log2Ceiling(), Is.EqualTo(4));  // log2(16) = 4
        Assert.That(1024UL.Log2Ceiling(), Is.EqualTo(10)); // log2(1024) = 10
    }

    [Test]
    public void Log2Ceiling_NonPowerOfTwo_ShouldRoundUp()
    {
        Assert.That(3UL.Log2Ceiling(), Is.EqualTo(2));   // ceil(log2(3)) = 2
        Assert.That(5UL.Log2Ceiling(), Is.EqualTo(3));   // ceil(log2(5)) = 3
        Assert.That(6UL.Log2Ceiling(), Is.EqualTo(3));   // ceil(log2(6)) = 3
        Assert.That(7UL.Log2Ceiling(), Is.EqualTo(3));   // ceil(log2(7)) = 3
        Assert.That(9UL.Log2Ceiling(), Is.EqualTo(4));   // ceil(log2(9)) = 4
    }

    #endregion

    #region Power2 Limits Tests

    [Test]
    public void Power2LowerLimit_ShouldReturnLargestPowerOfTwo()
    {
        Assert.That(1UL.Power2LowerLimit(), Is.EqualTo(1UL));
        Assert.That(3UL.Power2LowerLimit(), Is.EqualTo(2UL));   // Largest 2^n ≤ 3 is 2
        Assert.That(5UL.Power2LowerLimit(), Is.EqualTo(4UL));   // Largest 2^n ≤ 5 is 4
        Assert.That(7UL.Power2LowerLimit(), Is.EqualTo(4UL));   // Largest 2^n ≤ 7 is 4
        Assert.That(15UL.Power2LowerLimit(), Is.EqualTo(8UL));  // Largest 2^n ≤ 15 is 8
    }

    [Test]
    public void Power2UpperLimit_ShouldReturnSmallestPowerOfTwo()
    {
        Assert.That(1UL.Power2UpperLimit(), Is.EqualTo(1UL));
        Assert.That(3UL.Power2UpperLimit(), Is.EqualTo(4UL));   // Smallest 2^n ≥ 3 is 4
        Assert.That(5UL.Power2UpperLimit(), Is.EqualTo(8UL));   // Smallest 2^n ≥ 5 is 8
        Assert.That(7UL.Power2UpperLimit(), Is.EqualTo(8UL));   // Smallest 2^n ≥ 7 is 8
        Assert.That(9UL.Power2UpperLimit(), Is.EqualTo(16UL));  // Smallest 2^n ≥ 9 is 16
    }

    #endregion

    #region ShiftOnes Tests

    [Test]
    public void ShiftOnes_ShouldPreserveOnesCount()
    {
        var pattern = 7UL; // 111 binary
        var shifted = pattern.ShiftOnes(2);

        Assert.That(shifted.CountOnes(), Is.EqualTo(pattern.CountOnes()),
            "Shifting should preserve the number of ones");
    }

    [Test]
    public void ShiftOnes_PositiveOffset_ShouldShiftLeft()
    {
        var pattern = 3UL; // 11 binary
        var shifted = pattern.ShiftOnes(2);

        // 11 shifted left by 2 becomes 1100 = 12
        Assert.That(shifted, Is.EqualTo(12UL));
    }

    #endregion

    #region GetNthSetBitPosition Tests

    [Test]
    public void GetNthSetBitPosition_SingleBit_ShouldReturnCorrectPosition()
    {
        Assert.That(1UL.GetNthSetBitPosition(0), Is.EqualTo(0));   // Bit 0
        Assert.That(2UL.GetNthSetBitPosition(0), Is.EqualTo(1));   // Bit 1
        Assert.That(4UL.GetNthSetBitPosition(0), Is.EqualTo(2));   // Bit 2
        Assert.That(8UL.GetNthSetBitPosition(0), Is.EqualTo(3));   // Bit 3
        Assert.That((1UL << 63).GetNthSetBitPosition(0), Is.EqualTo(63)); // Bit 63
    }

    [Test]
    public void GetNthSetBitPosition_MultipleBits_ShouldReturnAbsolutePositions()
    {
        // Pattern: 0b00100100 (bits 2 and 5 set)
        var pattern = 0x24UL;

        Assert.That(pattern.GetNthSetBitPosition(0), Is.EqualTo(2), "First set bit at position 2");
        Assert.That(pattern.GetNthSetBitPosition(1), Is.EqualTo(5), "Second set bit at position 5");
    }

    [Test]
    public void GetNthSetBitPosition_ComplexPattern_ShouldReturnAllPositions()
    {
        // Pattern: 0b10101010 (bits 1, 3, 5, 7 set)
        var pattern = 0xAAUL;

        Assert.That(pattern.GetNthSetBitPosition(0), Is.EqualTo(1));
        Assert.That(pattern.GetNthSetBitPosition(1), Is.EqualTo(3));
        Assert.That(pattern.GetNthSetBitPosition(2), Is.EqualTo(5));
        Assert.That(pattern.GetNthSetBitPosition(3), Is.EqualTo(7));
    }

    [Test]
    public void GetNthSetBitPosition_NonContiguousBits_ShouldReturnCorrectPositions()
    {
        // Pattern: bits 0, 10, 20, 30 set
        var pattern = (1UL << 0) | (1UL << 10) | (1UL << 20) | (1UL << 30);

        Assert.That(pattern.GetNthSetBitPosition(0), Is.EqualTo(0));
        Assert.That(pattern.GetNthSetBitPosition(1), Is.EqualTo(10));
        Assert.That(pattern.GetNthSetBitPosition(2), Is.EqualTo(20));
        Assert.That(pattern.GetNthSetBitPosition(3), Is.EqualTo(30));
    }

    [Test]
    public void GetNthSetBitPosition_IndexOutOfRange_ShouldThrow()
    {
        var pattern = 0b101UL; // Only 2 bits set

        Assert.Throws<IndexOutOfRangeException>(() => pattern.GetNthSetBitPosition(2));
        Assert.Throws<IndexOutOfRangeException>(() => pattern.GetNthSetBitPosition(3));
    }

    [Test]
    public void GetNthSetBitPosition_ZeroPattern_ShouldThrow()
    {
        Assert.Throws<IndexOutOfRangeException>(() => 0UL.GetNthSetBitPosition(0));
    }

    [Test]
    public void TryGetNthSetBitPosition_ValidIndex_ShouldReturnPosition()
    {
        var pattern = 0x24UL; // Bits 2 and 5 set

        Assert.That(pattern.TryGetNthSetBitPosition(0), Is.EqualTo(2));
        Assert.That(pattern.TryGetNthSetBitPosition(1), Is.EqualTo(5));
    }

    [Test]
    public void TryGetNthSetBitPosition_IndexOutOfRange_ShouldReturnMinusOne()
    {
        var pattern = 0b101UL; // Only 2 bits set

        Assert.That(pattern.TryGetNthSetBitPosition(2), Is.EqualTo(-1));
        Assert.That(pattern.TryGetNthSetBitPosition(10), Is.EqualTo(-1));
    }

    [Test]
    public void TryGetNthSetBitPosition_ZeroPattern_ShouldReturnMinusOne()
    {
        Assert.That(0UL.TryGetNthSetBitPosition(0), Is.EqualTo(-1));
    }

    [Test]
    public void GetNthSetBitPosition_AllBitsSet_ShouldReturnSequentialPositions()
    {
        var pattern = 0xFFUL; // First 8 bits set

        for (int i = 0; i < 8; i++)
        {
            Assert.That(pattern.GetNthSetBitPosition(i), Is.EqualTo(i),
                $"Bit {i} should be at position {i}");
        }
    }

    #endregion
}
