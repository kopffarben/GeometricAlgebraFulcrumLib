using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;

namespace GeometricAlgebraFulcrumLib.Applications.VRSketch.CurveFitting;

/// <summary>
/// Test-Programm für Arc Fitting mit synthetischen Daten
/// </summary>
public static class ArcFittingTests
{
    /// <summary>
    /// Generiere Punkte auf einem Kreisbogen
    /// </summary>
    public static List<LinFloat64Vector3D> GenerateArcPoints(
        LinFloat64Vector3D center,
        double radius,
        LinFloat64Vector3D normal,
        double startAngle,
        double endAngle,
        int numPoints)
    {
        var points = new List<LinFloat64Vector3D>();

        // Finde zwei orthogonale Vektoren in der Ebene
        var arbitrary = Math.Abs(normal.Z) < 0.9
            ? LinFloat64Vector3D.E3
            : LinFloat64Vector3D.E1;

        var u = normal.VectorCross(arbitrary);
        u = LinFloat64Vector3D.CreateUnitVector(u.X, u.Y, u.Z);

        var v = normal.VectorCross(u);
        v = LinFloat64Vector3D.CreateUnitVector(v.X, v.Y, v.Z);

        // Generiere Punkte
        for (int i = 0; i < numPoints; i++)
        {
            var t = i / (numPoints - 1.0);
            var angle = startAngle + t * (endAngle - startAngle);

            var point = center +
                        u * (radius * Math.Cos(angle)) +
                        v * (radius * Math.Sin(angle));

            points.Add(point);
        }

        return points;
    }

    /// <summary>
    /// Generiere Punkte auf mehreren Kreisbögen (S-Kurve)
    /// </summary>
    public static List<LinFloat64Vector3D> GenerateSCurve(int pointsPerArc = 20)
    {
        var allPoints = new List<LinFloat64Vector3D>();

        // Erster Bogen: 90° nach rechts
        var arc1 = GenerateArcPoints(
            center: LinFloat64Vector3D.Create(1, 0, 0),
            radius: 1.0,
            normal: LinFloat64Vector3D.E3,
            startAngle: Math.PI,
            endAngle: Math.PI / 2,
            numPoints: pointsPerArc
        );
        allPoints.AddRange(arc1);

        // Zweiter Bogen: 90° nach links
        var arc2 = GenerateArcPoints(
            center: LinFloat64Vector3D.Create(1, 2, 0),
            radius: 1.0,
            normal: LinFloat64Vector3D.E3,
            startAngle: -Math.PI / 2,
            endAngle: 0,
            numPoints: pointsPerArc
        );
        allPoints.AddRange(arc2.Skip(1)); // Skip first point (duplicate)

        return allPoints;
    }

    /// <summary>
    /// Test Circle Fitting
    /// </summary>
    public static void TestCircleFitting()
    {
        Console.WriteLine("\n=== TEST: Circle Fitting ===");

        // Generiere Punkte auf einem bekannten Kreis
        var expectedCenter = LinFloat64Vector3D.Create(2, 3, 1);
        var expectedRadius = 1.5;
        var expectedNormal = LinFloat64Vector3D.E3;

        var points = GenerateArcPoints(
            expectedCenter,
            expectedRadius,
            expectedNormal,
            startAngle: 0,
            endAngle: Math.PI, // 180° Bogen
            numPoints: 20
        );

        Console.WriteLine($"Generated {points.Count} points on arc");
        Console.WriteLine($"Expected: Center={expectedCenter}, Radius={expectedRadius:F3}, Normal={expectedNormal}");

        // Fitte Kreis
        var result = CircleFitter.FitCircle(points);

        if (result == null)
        {
            Console.WriteLine("ERROR: Circle fitting failed!");
            return;
        }

        Console.WriteLine($"Fitted:   Center={result.Center}, Radius={result.Radius:F3}, Normal={result.Normal}");
        Console.WriteLine($"RMS Error: {result.RmsError:F6}");

        // Prüfe Genauigkeit
        var centerError = (result.Center - expectedCenter).VectorENorm();
        var radiusError = Math.Abs(result.Radius - expectedRadius);

        Console.WriteLine($"Center Error: {centerError:F6}");
        Console.WriteLine($"Radius Error: {radiusError:F6}");

        if (result.RmsError < 0.001 && centerError < 0.01 && radiusError < 0.01)
        {
            Console.WriteLine("✓ Circle fitting SUCCESS!");
        }
        else
        {
            Console.WriteLine("✗ Circle fitting FAILED - too much error!");
        }
    }

    /// <summary>
    /// Test Arc Segmentation
    /// </summary>
    public static void TestArcSegmentation()
    {
        Console.WriteLine("\n=== TEST: Arc Segmentation ===");

        // Generiere S-Kurve aus 2 Bögen
        var points = GenerateSCurve(pointsPerArc: 30);

        Console.WriteLine($"Generated S-Curve with {points.Count} points (2 arcs expected)");

        // Test mit verschiedenen maxError Werten
        var maxErrors = new[] { 0.001, 0.01, 0.05, 0.1 };

        foreach (var maxError in maxErrors)
        {
            var segmented = ArcSegmentationFitter.SegmentIntoArcs(
                points,
                maxError: maxError,
                minPointsPerArc: 3,
                maxPointsPerArc: 30
            );

            Console.WriteLine($"  maxError={maxError:F3}: {points.Count} → {segmented.Count} points ({segmented.Count - 1} segments)");
        }

        // Test adaptive
        Console.WriteLine("\nAdaptive Segmentation:");
        var adaptiveSegmented = ArcSegmentationFitter.SegmentIntoArcsAdaptive(
            points,
            targetReductionFactor: 0.1
        );

        Console.WriteLine($"  Adaptive: {points.Count} → {adaptiveSegmented.Count} points ({adaptiveSegmented.Count - 1} segments)");

        // Erwartung: 2-3 Segmente für 2 Bögen
        if (adaptiveSegmented.Count >= 3 && adaptiveSegmented.Count <= 4)
        {
            Console.WriteLine("✓ Arc segmentation looks reasonable!");
        }
        else
        {
            Console.WriteLine($"✗ Arc segmentation suspicious - expected 3-4 points, got {adaptiveSegmented.Count}");
        }
    }

    /// <summary>
    /// Test mit einem einzelnen Bogen
    /// </summary>
    public static void TestSingleArc()
    {
        Console.WriteLine("\n=== TEST: Single Arc (90°) ===");

        // Generiere 90° Bogen
        var points = GenerateArcPoints(
            center: LinFloat64Vector3D.Create(1, 1, 0),
            radius: 2.0,
            normal: LinFloat64Vector3D.E3,
            startAngle: 0,
            endAngle: Math.PI / 2,
            numPoints: 50
        );

        Console.WriteLine($"Generated 90° arc with {points.Count} points");

        // Teste Segmentierung
        var segmented = ArcSegmentationFitter.SegmentIntoArcsAdaptive(points);

        Console.WriteLine($"Segmented: {points.Count} → {segmented.Count} points");

        // Für einen einzelnen perfekten Bogen sollten wir nur 2 Punkte bekommen (Start + End)
        if (segmented.Count == 2)
        {
            Console.WriteLine("✓ Perfect! Single arc recognized.");
        }
        else if (segmented.Count <= 5)
        {
            Console.WriteLine($"○ Acceptable: {segmented.Count - 1} segments for single arc");
        }
        else
        {
            Console.WriteLine($"✗ Too many segments: {segmented.Count - 1} for a single arc!");
        }

        // Zeige die gefitteten Segmente
        for (int i = 0; i < segmented.Count - 1; i++)
        {
            var start = segmented[i];
            var end = segmented[i + 1];
            var dist = (end - start).VectorENorm();
            Console.WriteLine($"  Segment {i}: distance={dist:F3}");
        }
    }

    /// <summary>
    /// Führe alle Tests aus
    /// </summary>
    public static void RunAllTests()
    {
        Console.WriteLine("╔═══════════════════════════════════════╗");
        Console.WriteLine("║  Arc Fitting Tests - Synthetic Data   ║");
        Console.WriteLine("╚═══════════════════════════════════════╝");

        TestCircleFitting();
        TestSingleArc();
        TestArcSegmentation();

        Console.WriteLine("\n=== All Tests Completed ===\n");
    }
}
