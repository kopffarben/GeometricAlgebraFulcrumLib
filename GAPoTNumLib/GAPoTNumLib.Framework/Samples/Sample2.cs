using System;
using System.Globalization;
using GAPoTNumLib.GAPoT;

namespace GAPoTNumLib.Framework.Samples
{
    /// <summary>
    /// Demonstrates multivector operations and display formatting in GAPoT
    /// including LaTeX output, text display, and basic operations.
    /// </summary>
    public static class MultivectorOperationsSample
    {
        /// <summary>
        /// Demonstrates multivector display formats, operations, and computations
        /// including LaTeX formatting, inverse operations, and geometric products.
        /// </summary>
        public static void Execute()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            
            var mvU = "1<1>, 1<2>, 1<3>, 2<4>, 1<5>, 1<7>, -1<8>"
                .GaPoTNumParseVector();

            var mvI = "1<1>, 1<2>, 1<3>, 3<4>, 0.5<5>, 1<7>"
                .GaPoTNumParseVector();
           
            Console.WriteLine(@"Display multivectors in LaTeX form");
            Console.WriteLine($@"U = {mvU.TermsToLaTeX()}");
            Console.WriteLine($@"I = {mvI.TermsToLaTeX()}");
            Console.WriteLine();
           
            Console.WriteLine(@"Display multivectors in text form");
            Console.WriteLine($@"U = {mvU}");
            Console.WriteLine($@"I = {mvI}");
            Console.WriteLine();

            Console.WriteLine(@"Compute and display the inverse");
            Console.WriteLine($@"inv(U) = {mvU.Inverse()}");
            Console.WriteLine($@"inv(I) = {mvI.Inverse()}");
            Console.WriteLine();

            Console.WriteLine(@"Compute and display geometric product of multivectors U * inv(U)");
            Console.WriteLine($@"U * inv(U) = {mvU * mvU.Inverse()}");
            Console.WriteLine($@"I * inv(I) = {mvI * mvI.Inverse()}");
            Console.WriteLine();

            Console.WriteLine(@"Compute and display geometric product of multivectors");
            Console.WriteLine($@"U * I = {mvU * mvI}");
            Console.WriteLine($@"U * I = {(mvU * mvI).TermsToLaTeX()}");
            Console.WriteLine();
        }
    }
}