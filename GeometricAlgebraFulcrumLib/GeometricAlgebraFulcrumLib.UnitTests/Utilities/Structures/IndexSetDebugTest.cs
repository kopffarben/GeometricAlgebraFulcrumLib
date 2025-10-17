using GeometricAlgebraFulcrumLib.Utilities.Structures.IndexSets;
using NUnit.Framework;
using System;

namespace GeometricAlgebraFulcrumLib.UnitTests.Utilities.Structures;

/// <summary>
/// Debug tests to understand IndexSet behavior
/// </summary>
[TestFixture]
public class IndexSetDebugTest
{
    [Test]
    public void Debug_CreatePair_BitPattern()
    {
        // Test CreatePair with simple values
        var pair = IndexSet.CreatePair(2, 5);

        Console.WriteLine($"CreatePair(2, 5):");
        Console.WriteLine($"  Count: {pair.Count}");
        Console.WriteLine($"  IsUInt64Set: {pair.IsUInt64Set}");
        Console.WriteLine($"  IsPairSet: {pair.IsPairSet}");
        Console.WriteLine($"  BitPattern (cast to ulong): {(ulong)pair:X}");

        Console.WriteLine($"  Indices:");
        for (int i = 0; i < pair.Count; i++)
        {
            Console.WriteLine($"    pair[{i}] = {pair[i]}");
        }
    }

    [Test]
    public void Debug_GetNthSetBitPosition()
    {
        // Manually test GetNthSetBitPosition
        ulong bitPattern = (1UL << 2) | (1UL << 5); // Bits 2 and 5 set = 0b100100 = 0x24
        Console.WriteLine($"BitPattern: 0x{bitPattern:X} (binary: {Convert.ToString((long)bitPattern, 2).PadLeft(8, '0')})");

        // Use extension method
        var pos0 = GeometricAlgebraFulcrumLib.Utilities.Structures.BitManipulation.UInt64BitUtils.GetNthSetBitPosition(bitPattern, 0);
        var pos1 = GeometricAlgebraFulcrumLib.Utilities.Structures.BitManipulation.UInt64BitUtils.GetNthSetBitPosition(bitPattern, 1);

        Console.WriteLine($"GetNthSetBitPosition(0) = {pos0} (expected: 2)");
        Console.WriteLine($"GetNthSetBitPosition(1) = {pos1} (expected: 5)");

        Assert.That(pos0, Is.EqualTo(2), "First set bit should be at position 2");
        Assert.That(pos1, Is.EqualTo(5), "Second set bit should be at position 5");
    }

    [Test]
    public void Debug_CreatePair_ReversedOrder()
    {
        // This should fail because CreatePair expects index1 < index2
        try
        {
            var pair = IndexSet.CreatePair(5, 2);
            Console.WriteLine($"CreatePair(5, 2) succeeded: pair[0]={pair[0]}, pair[1]={pair[1]}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CreatePair(5, 2) failed with: {ex.GetType().Name}: {ex.Message}");
        }
    }

    [Test]
    public void Debug_Create_WithArray()
    {
        // Test Create with unsorted array
        var set = IndexSet.Create(5, 2, 8);

        Console.WriteLine($"Create(5, 2, 8):");
        Console.WriteLine($"  Count: {set.Count}");
        for (int i = 0; i < set.Count; i++)
        {
            Console.WriteLine($"    set[{i}] = {set[i]}");
        }
    }
}
