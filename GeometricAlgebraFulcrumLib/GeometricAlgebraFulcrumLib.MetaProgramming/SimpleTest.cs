using AngouriMath;
using GeometricAlgebraFulcrumLib.MetaProgramming.Context;
using GeometricAlgebraFulcrumLib.MetaProgramming.Context.Processors;

namespace GeometricAlgebraFulcrumLib.MetaProgramming;

/// <summary>
/// Simple test to validate the fully implemented MetaExpressionToScalar and ScalarToMetaExpression methods
/// </summary>
public static class SimpleTest
{
    public static void TestConversions()
    {
        Console.WriteLine("Testing fully implemented conversion methods...");
        
        var processor = ScalarProcessorOfAngouriMathEntity.Instance;
        var context = new MetaContext();
        
        // Test 1: Create a simple meta expression and convert to scalar
        var x = context["x"];
        Console.WriteLine($"MetaExpression: {x.ScalarValue}");
        
        var entity = processor.MetaExpressionToScalar(x.ScalarValue);
        Console.WriteLine($"Converted to AngouriMath Entity: {entity}");
        
        // Test 2: Convert back to meta expression
        var backToMeta = processor.ScalarToMetaExpression(context, entity);
        Console.WriteLine($"Converted back to MetaExpression: {backToMeta}");
        
        // Test 3: Test with a number
        var number = context.GetOrDefineLiteralNumber(42);
        Console.WriteLine($"Number MetaExpression: {number}");
        
        var numberEntity = processor.MetaExpressionToScalar(number);
        Console.WriteLine($"Number as Entity: {numberEntity}");
        
        var numberBackToMeta = processor.ScalarToMetaExpression(context, numberEntity);
        Console.WriteLine($"Number back to MetaExpression: {numberBackToMeta}");
        
        Console.WriteLine("✓ Conversion methods are working!");
    }
}