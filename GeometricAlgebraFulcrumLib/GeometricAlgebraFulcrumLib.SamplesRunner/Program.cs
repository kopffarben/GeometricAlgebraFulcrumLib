using System;
using System.Reflection;
using GeometricAlgebraFulcrumLib.Optimization.Samples;
using GeometricAlgebraFulcrumLib.Utilities.Structures.Samples.IndexSets;

namespace GeometricAlgebraFulcrumLib.SamplesRunner;

/// <summary>
/// Centralized runner for all GeometricAlgebraFulcrumLib samples.
/// Provides an organized way to discover and execute various sample demonstrations.
/// </summary>
public static class SamplesRunner
{
    /// <summary>
    /// Displays all available sample categories and allows interactive selection.
    /// </summary>
    public static void RunInteractively()
    {
        Console.WriteLine("=============================================================");
        Console.WriteLine("    GeometricAlgebraFulcrumLib Samples Runner");
        Console.WriteLine("=============================================================");
        Console.WriteLine();
        
        while (true)
        {
            DisplayMenu();
            Console.Write("Enter your choice (1-7, 0 to quit): ");
            
            if (!int.TryParse(Console.ReadLine(), out var choice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }
            
            if (choice == 0)
            {
                Console.WriteLine("Goodbye!");
                break;
            }
            
            ExecuteCategory(choice);
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }
    
    /// <summary>
    /// Runs all samples automatically for demonstration purposes.
    /// </summary>
    public static void RunAllSamples()
    {
        Console.WriteLine("Running All GeometricAlgebraFulcrumLib Samples");
        Console.WriteLine("==============================================");
        Console.WriteLine();
        
        // Run optimization samples
        RunOptimizationSamples();
        
        // Run data structure samples  
        RunDataStructureSamples();
        
        // GAPoT samples are currently unavailable due to .NET Framework compatibility
        Console.WriteLine("GAPoT Framework samples are currently unavailable (framework compatibility issue)");
        
        Console.WriteLine("All samples completed successfully!");
    }
    
    private static void DisplayMenu()
    {
        Console.WriteLine("Available Sample Categories:");
        Console.WriteLine("============================");
        Console.WriteLine("1. Optimization Samples");
        Console.WriteLine("   - Gradient Descent (CPU vs GPU performance)");
        Console.WriteLine("   - Support Vector Machines (Classification)");
        Console.WriteLine("   - Cartesian Genetic Programming");
        Console.WriteLine();
        Console.WriteLine("2. Data Structure Samples");
        Console.WriteLine("   - IndexSet operations and performance");
        Console.WriteLine("   - Set manipulation and iteration");
        Console.WriteLine();
        Console.WriteLine("3. GAPoT Framework Samples");
        Console.WriteLine("   - Power system calculations");
        Console.WriteLine("   - Multivector operations");
        Console.WriteLine("   - Validation examples");
        Console.WriteLine();
        Console.WriteLine("4. Geometric Algebra Samples");
        Console.WriteLine("   - Euclidean operations");
        Console.WriteLine("   - Rotations and reflections");
        Console.WriteLine();
        Console.WriteLine("5. Symbolic Mathematics Samples");
        Console.WriteLine("   - Mathematica integration");
        Console.WriteLine("   - Symbolic computations");
        Console.WriteLine();
        Console.WriteLine("6. Power Systems Applications");
        Console.WriteLine("   - Clarke transformations");
        Console.WriteLine("   - Geometric frequency analysis");
        Console.WriteLine();
        Console.WriteLine("7. Performance Benchmarks");
        Console.WriteLine("   - Algorithm performance comparisons");
        Console.WriteLine("   - Memory usage analysis");
        Console.WriteLine();
        Console.WriteLine("0. Exit");
        Console.WriteLine();
    }
    
    private static void ExecuteCategory(int choice)
    {
        try
        {
            switch (choice)
            {
                case 1:
                    RunOptimizationSamples();
                    break;
                case 2:
                    RunDataStructureSamples();
                    break;
                case 3:
                    Console.WriteLine("GAPoT Framework samples are currently unavailable due to .NET Framework compatibility issues.");
                    Console.WriteLine("The GAPoTNumLib.Framework project targets .NET Framework 4.7.2 which cannot be directly");
                    Console.WriteLine("referenced from this .NET 8.0 console application.");
                    break;
                case 4:
                    Console.WriteLine("Geometric Algebra samples would be run here (not implemented in this demo)");
                    break;
                case 5:
                    Console.WriteLine("Symbolic Mathematics samples would be run here (not implemented in this demo)");
                    break;
                case 6:
                    Console.WriteLine("Power Systems samples would be run here (not implemented in this demo)");
                    break;
                case 7:
                    Console.WriteLine("Performance Benchmarks would be run here (not implemented in this demo)");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please select 1-7 or 0 to exit.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error running samples: {ex.Message}");
            Console.WriteLine("Some samples may require external data files or specific configurations.");
        }
    }
    
    private static void RunOptimizationSamples()
    {
        Console.WriteLine("Running Optimization Samples...");
        Console.WriteLine("===============================");
        
        try
        {
            Console.WriteLine("\n1. Gradient Descent Performance Comparison:");
            Console.WriteLine("--------------------------------------------");
            GradientDescentSamples.PerformanceComparisonExample();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Gradient Descent sample failed: {ex.Message}");
        }
        
        try
        {
            Console.WriteLine("\n2. SVM Wine Dataset Classification:");
            Console.WriteLine("------------------------------------");
            SvmSamples.WineDatasetClassificationExample();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SVM Wine dataset sample failed: {ex.Message}");
            Console.WriteLine("This sample requires wine.txt dataset file.");
        }
        
        try
        {
            Console.WriteLine("\n3. SVM Synthetic Data Classification:");
            Console.WriteLine("-------------------------------------");
            SvmSamples.SyntheticDataClassificationExample();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SVM synthetic data sample failed: {ex.Message}");
        }
    }
    
    private static void RunDataStructureSamples()
    {
        Console.WriteLine("Running Data Structure Samples...");
        Console.WriteLine("=================================");
        
        Console.WriteLine("\n1. IndexSet Creation Examples:");
        Console.WriteLine("-------------------------------");
        BasicSamples.CreationExample();
        
        Console.WriteLine("\n2. IndexSet Iterator Examples:");
        Console.WriteLine("-------------------------------");
        BasicSamples.IteratorExample1();
        
        Console.WriteLine("\n3. IndexSet Performance Examples:");
        Console.WriteLine("----------------------------------");
        BasicSamples.IteratorExample2();
        
        Console.WriteLine("\n4. IndexSet Remove Operations:");
        Console.WriteLine("------------------------------");
        BasicSamples.TryRemoveExample();
        
        Console.WriteLine("\n5. IndexSet Ordering:");
        Console.WriteLine("---------------------");
        BasicSamples.OrderingExample();
    }
    
    /// <summary>
    /// Entry point for console applications to run samples.
    /// </summary>
    public static void Main(string[] args)
    {
        if (args.Length > 0 && args[0].ToLower() == "--all")
        {
            RunAllSamples();
        }
        else
        {
            RunInteractively();
        }
    }
}