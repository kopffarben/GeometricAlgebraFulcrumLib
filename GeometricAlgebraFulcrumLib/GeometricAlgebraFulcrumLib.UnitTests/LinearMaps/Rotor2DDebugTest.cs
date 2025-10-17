using System;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Rotors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.LinearMaps;

/// <summary>
/// Debug test to understand 2D rotor construction issues
/// </summary>
[TestFixture]
public class Rotor2DDebugTest
{
    [Test]
    public void Debug_90DegreeRotationConstruction()
    {
        var processor = XGaFloat64Processor.Euclidean;

        Console.WriteLine("=== Debug: 90° Rotation Construction ===");
        Console.WriteLine();

        // Get basis vectors
        var e1 = processor.VectorTerm(0);
        var e2 = processor.VectorTerm(1);

        // Method 1: Manual construction (from test)
        Console.WriteLine("Method 1: Manual Construction");
        var angle = Math.PI / 2; // 90 degrees
        var halfAngle = angle / 2;

        var bivector = e1.Op(e2).GetBivectorPart();
        Console.WriteLine($"  Bivector e₁∧e₂: {bivector}");

        var scalarPart = Math.Cos(halfAngle);
        var bivectorPart = bivector * Math.Sin(halfAngle);

        Console.WriteLine($"  Angle: {angle} rad (90°)");
        Console.WriteLine($"  Half angle: {halfAngle} rad (45°)");
        Console.WriteLine($"  Scalar part (cos(45°)): {scalarPart}");
        Console.WriteLine($"  Bivector part (sin(45°) * B): {bivectorPart}");

        var rotorManual = XGaFloat64PureRotor.Create(scalarPart, bivectorPart);
        Console.WriteLine($"  Rotor multivector: {rotorManual.Multivector}");
        Console.WriteLine($"  Rotor reverse: {rotorManual.MultivectorReverse}");
        Console.WriteLine($"  IsValid: {rotorManual.IsValid()}");
        Console.WriteLine();

        // Test rotation of e1
        Console.WriteLine("Testing: Rotate e₁");
        var e1RotatedManual = rotorManual.OmMap(e1);
        Console.WriteLine($"  Result: {e1RotatedManual}");
        Console.WriteLine($"  Expected: {e2}");
        var diff1 = (e2 - e1RotatedManual).ENormSquared();
        Console.WriteLine($"  Difference norm squared: {diff1}");
        Console.WriteLine();

        // Method 2: Using CreatePureRotor
        Console.WriteLine("Method 2: Using CreatePureRotor(e1 → e2)");
        var rotorAuto = e1.CreatePureRotor(e2);
        Console.WriteLine($"  Rotor multivector: {rotorAuto.Multivector}");
        Console.WriteLine($"  Rotor reverse: {rotorAuto.MultivectorReverse}");
        Console.WriteLine($"  IsValid: {rotorAuto.IsValid()}");
        Console.WriteLine();

        var e1RotatedAuto = rotorAuto.OmMap(e1);
        Console.WriteLine($"  Result: {e1RotatedAuto}");
        Console.WriteLine($"  Expected: {e2}");
        var diff2 = (e2 - e1RotatedAuto).ENormSquared();
        Console.WriteLine($"  Difference norm squared: {diff2}");
        Console.WriteLine();

        // Analyze the OmMap operation
        Console.WriteLine("=== OmMap Operation Analysis ===");
        Console.WriteLine("Formula: OmMap(v) = R * v * R̃");
        Console.WriteLine();

        var step1 = rotorManual.Multivector.Gp(e1);
        Console.WriteLine($"Step 1: R * e₁ = {step1}");

        var step2 = step1.Gp(rotorManual.MultivectorReverse);
        Console.WriteLine($"Step 2: (R * e₁) * R̃ = {step2}");

        var vectorPart = step2.GetVectorPart();
        Console.WriteLine($"Step 3: Extract vector part = {vectorPart}");
        Console.WriteLine();

        // Check rotor condition
        Console.WriteLine("=== Rotor Condition Check ===");
        var gpCondition = rotorManual.Multivector.Gp(rotorManual.MultivectorReverse);
        Console.WriteLine($"R * R̃ = {gpCondition}");
        Console.WriteLine($"Scalar part: {gpCondition.GetScalarPart()}");
        Console.WriteLine($"Expected: 1.0");
        Console.WriteLine($"Difference: {Math.Abs(gpCondition.GetScalarPart() - 1.0)}");
    }

    [Test]
    public void Debug_RotorVsReflection()
    {
        var processor = XGaFloat64Processor.Euclidean;

        Console.WriteLine("=== Debug: Rotor vs Reflection ===");
        Console.WriteLine();

        var e1 = processor.VectorTerm(0);
        var e2 = processor.VectorTerm(1);

        // Create rotor: e1 → e2 (90° rotation)
        var rotor = e1.CreatePureRotor(e2);
        Console.WriteLine("Rotor (e₁ → e₂):");
        Console.WriteLine($"  Multivector: {rotor.Multivector}");
        Console.WriteLine($"  IsValid: {rotor.IsValid()}");
        Console.WriteLine();

        // Compare with expected 45° rotor formula
        var angle = Math.PI / 2;
        var halfAngle = angle / 2;
        Console.WriteLine($"Expected 45° rotor:");
        Console.WriteLine($"  Scalar: cos(45°) = {Math.Cos(halfAngle)}");
        Console.WriteLine($"  Bivector: sin(45°) * e₁e₂ = {Math.Sin(halfAngle)}");
        Console.WriteLine();

        // Extract rotor components
        var actualScalar = rotor.Multivector.GetScalarPart();
        var actualBivector = rotor.Multivector.GetBivectorPart();
        Console.WriteLine($"Actual rotor components:");
        Console.WriteLine($"  Scalar: {actualScalar}");
        Console.WriteLine($"  Bivector: {actualBivector}");
        Console.WriteLine();

        // Test on both e1 and e2
        Console.WriteLine("Testing rotor:");
        var e1Rotated = rotor.OmMap(e1);
        var e2Rotated = rotor.OmMap(e2);
        Console.WriteLine($"  R(e₁) = {e1Rotated}");
        Console.WriteLine($"  Expected: {e2}");
        Console.WriteLine($"  R(e₂) = {e2Rotated}");
        Console.WriteLine($"  Expected: -e₁ (for 90° rotation)");
    }
}
