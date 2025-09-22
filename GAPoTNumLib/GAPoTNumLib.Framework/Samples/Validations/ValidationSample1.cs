using System;
using System.Linq;
using GAPoTNumLib.GAPoT;
using GAPoTNumLib.Structures;

namespace GAPoTNumLib.Framework.Samples.Validations
{
    public static class ValidationSample1
    {
        /// <summary>
        /// Validates biversor to multivector conversion and norm calculations.
        /// </summary>
        public static void Execute1()
        {
            Console.WriteLine("Biversor to Multivector Conversion Validation");
            Console.WriteLine("==============================================");
            
            var bv =
                "3.1009536313039 <>, -1.28445705037617 <1,3>, -0.985598559653489 <2,3>, 0.408248290463863 <1,2>"
                    .GaPoTNumParseBiversor();

            Console.WriteLine($"Biversor Squared Norm: {bv.Norm2():G}");
            Console.WriteLine();

            var mv = bv.ToMultivector();

            Console.WriteLine($"Multivector Squared Norm: {mv.Norm2():G}");
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates bit reversal operations and bit counting functionality.
        /// </summary>
        public static void Execute2()
        {
            Console.WriteLine("Bit Manipulation Validation");
            Console.WriteLine("===========================");
            
            var bitsCount = 4;

            for (var i = 0; i < (1 << bitsCount); i++)
            {
                var patternText1 = 
                    Convert.ToString(i, 2).PadLeft(bitsCount, '0');

                var j = i.ReverseBits(bitsCount);

                var patternText2 = 
                    Convert.ToString(j, 2).PadLeft(bitsCount, '0');

                Console.WriteLine($"{patternText1} <=> {patternText2}, ({i.CountOnes()} bits)");
            }
        }

        /// <summary>
        /// Validates rotor operations including inverse and reverse calculations.
        /// </summary>
        /// <summary>
        /// Validates rotor operations including inverse and reverse calculations.
        /// </summary>
        public static void Execute3()
        {
            Console.WriteLine("Multivector Construction Validation");
            Console.WriteLine("===================================");
            
            var mv = GaPoTNumMultivector
                .CreateZero()
                .AddTerm(3, 1)
                .AddTerm(5, 2)
                .AddTerm(0, -2);

            Console.WriteLine($"Constructed Multivector: {mv.ToText()}");
        }

        /// <summary>
        /// Demonstrates rotor inverse and reverse operations for validation.
        /// </summary>
        public static void Execute()
        {
            Console.WriteLine("Rotor Operations Validation");
            Console.WriteLine("===========================");
            
            var rotor =
                "0.880476239217149 <>, 0.115916895959295 <1,2>, -0.364705199631001 <1,3>, -0.279848142333121 <2,3>"
                    .GaPoTNumParseBiversor().ToMultivector();

            Console.WriteLine($"Rotor Inverse: {rotor.Inverse()}");
            Console.WriteLine();

            Console.WriteLine($"Rotor Reverse: {rotor.Reverse()}");
            Console.WriteLine();
        }
    }
}
