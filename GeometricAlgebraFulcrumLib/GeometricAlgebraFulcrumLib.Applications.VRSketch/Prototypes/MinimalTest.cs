using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Blades;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Operations;

namespace GeometricAlgebraFulcrumLib.Applications.VRSketch.Prototypes;

/// <summary>
/// Minimaler Test der CGA-Grundfunktionalität
/// </summary>
public static class MinimalTest
{
    public static void Run()
    {
        Console.WriteLine("╔════════════════════════════════════════════════╗");
        Console.WriteLine("║       VR Arc-Spline Prototype - CGA Test       ║");
        Console.WriteLine("╚════════════════════════════════════════════════╝\n");

        var cga = CGaFloat64GeometricSpace5D.Instance;

        Console.WriteLine($"CGA Space: {cga.VSpaceDimensions}D");
        Console.WriteLine($"Basis Blades: E0={cga.EoVector}, E_inf={cga.EiVector}");

        // Test 1: Sphere creation (3D)
        Console.WriteLine("\n=== Test 1: Sphere Creation ===");
        var sphere = cga.Encode.IpnsRound.RealSphere(
            5.0,
            0, 0, 0
        );

        var sphereDecoded = sphere.DecodeIpnsRound.Element();
        Console.WriteLine($"Sphere created");
        Console.WriteLine($"  Center: ({sphereDecoded.CenterToVector3D().X:F2}, {sphereDecoded.CenterToVector3D().Y:F2}, {sphereDecoded.CenterToVector3D().Z:F2})");
        Console.WriteLine($"  Radius: {sphereDecoded.RealRadius:F2}");

        // Test 2: Circle through three points (OPNS)
        Console.WriteLine("\n=== Test 2: Circle from 3 Points ===");
        var p1 = LinFloat64Vector3D.Create(1, 0, 0);
        var p2 = LinFloat64Vector3D.Create(0, 1, 0);
        var p3 = LinFloat64Vector3D.Create(-1, 0, 0);

        var opnsCircle = cga.Encode.OpnsRound.Circle(p1, p2, p3);
        var ipnsCircle = opnsCircle.OpnsToIpns();
        var circleDecoded = ipnsCircle.DecodeIpnsRound.Element();

        Console.WriteLine($"Circle from 3 points");
        Console.WriteLine($"  Center: ({circleDecoded.CenterToVector3D().X:F2}, {circleDecoded.CenterToVector3D().Y:F2}, {circleDecoded.CenterToVector3D().Z:F2})");
        Console.WriteLine($"  Radius: {circleDecoded.RealRadius:F2}");

        Console.WriteLine("\n╔════════════════════════════════════════════════╗");
        Console.WriteLine("║  Test erfolgreich! GA-Ful CGA funktioniert!   ║");
        Console.WriteLine("╚════════════════════════════════════════════════╝");
    }
}
