using GeometricAlgebraFulcrumLib.Applications.VRSketch.Prototypes;

namespace GeometricAlgebraFulcrumLib.Applications.VRSketch;

class Program
{
    static void Main(string[] args)
    {
        // Führe beide Tests aus
        Console.WriteLine("╔════════════════════════════════════════════════╗");
        Console.WriteLine("║     VR Arc-Spline Prototype Test Suite        ║");
        Console.WriteLine("╚════════════════════════════════════════════════╝\n");

        // Test 1: Minimale CGA Funktionalität
        MinimalTest.Run();

        Console.WriteLine("\n\n" + new string('═', 50) + "\n\n");

        // Test 2: Arc-Spline Konstruktion
        ArcSplinePrototype.TestThreePointArcSpline();

        Console.WriteLine("\n\n" + new string('═', 50) + "\n\n");

        // Test 3: Controller-basierte Arc-Konstruktion
        ArcSplinePrototype.TestControllerBasedArcConstruction();

        Console.WriteLine("\n\n" + new string('═', 50) + "\n\n");

        // Test 4: Multi-Segment Spline mit C¹-Kontinuität
        ArcSplinePrototype.TestMultiSegmentSpline();

        Console.WriteLine("\n\n=== Alle Tests abgeschlossen ===");
    }
}
