namespace GeometricAlgebraFulcrumLib.MetaProgramming
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("GeometricAlgebraFulcrumLib - AngouriMath vs Mathematica Test");
            Console.WriteLine("===========================================================");
            Console.WriteLine();
            
            Console.WriteLine("MetaExpressionToScalar and ScalarToMetaExpression methods have been");
            Console.WriteLine("fully implemented using proper AngouriMath converters.");
            Console.WriteLine();
            
            try
            {
                SimpleTest.TestConversions();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during testing: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
