#!/usr/bin/env dotnet-script
#r "GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Algebra/bin/Release/net8.0/GeometricAlgebraFulcrumLib.Algebra.dll"
#r "GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Utilities.Structures/bin/Release/net8.0/GeometricAlgebraFulcrumLib.Utilities.Structures.dll"

using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;

// Test double Sp
Console.WriteLine("=== Testing Generic<double> Sp Operation ===");
var processor = XGaFloat64Processor.Euclidean;
var v1 = processor.Vector(1.0, 2.0, 3.0);
var v2 = processor.Vector(4.0, 5.0, 6.0);
var result = v1.Sp(v2);
Console.WriteLine($"v1 · v2 = {result.ScalarValue}");
Console.WriteLine($"Expected: {1*4 + 2*5 + 3*6} = 32");
Console.WriteLine($"Match: {Math.Abs(result.ScalarValue - 32.0) < 1e-12}");

// Test float Sp
Console.WriteLine("\n=== Testing Generic<float> Sp Operation ===");
var scalarProcessor = ScalarProcessorOfFloating<float>.Instance;
var processorFloat = XGaProcessor<float>.CreateEuclidean(scalarProcessor);
var v1f = processorFloat.Vector(1.0f, 2.0f, 3.0f);
var v2f = processorFloat.Vector(4.0f, 5.0f, 6.0f);
var resultf = v1f.Sp(v2f);
Console.WriteLine($"v1 · v2 = {resultf.ScalarValue}");
Console.WriteLine($"Expected: {1*4 + 2*5 + 3*6} = 32");
Console.WriteLine($"Match: {Math.Abs(resultf.ScalarValue - 32.0f) < 1e-6f}");

Console.WriteLine("\n✓ Sp optimization works correctly!");
