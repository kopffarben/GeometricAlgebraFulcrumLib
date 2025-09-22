using GeometricAlgebraFulcrumLib.MetaProgramming.Samples.Algebra.AngouriMath;

namespace GeometricAlgebraFulcrumLib.MetaProgramming
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("GeometricAlgebraFulcrumLib - AngouriMath vs Mathematica Test");
            Console.WriteLine("===========================================================");
            Console.WriteLine();
            
            try 
            {
                SymbolicComparisonTest.RunBasicTest();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
