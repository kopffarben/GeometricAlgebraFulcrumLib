using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Applications.VRSketch.CurveFitting;

Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║     Arc Fitting Algorithm Tests - Synthetic Data Analysis     ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// Test 1: Circle Fitting Genauigkeit
Console.WriteLine("═══ TEST 1: Circle Fitting Precision ═══");
Console.WriteLine();

var expectedCenter = LinFloat64Vector3D.Create(2, 3, 1);
var expectedRadius = 1.5;
var expectedNormal = LinFloat64Vector3D.E3;

var arcPoints = GenerateArcPoints(expectedCenter, expectedRadius, expectedNormal, 0, Math.PI, 20);

Console.WriteLine($"Generated: {arcPoints.Count} points on 180° arc");
Console.WriteLine($"Expected:  Center=({expectedCenter.X:F2}, {expectedCenter.Y:F2}, {expectedCenter.Z:F2}), Radius={expectedRadius:F3}");

var fitResult = CircleFitter.FitCircle(arcPoints);

if (fitResult != null)
{
    Console.WriteLine($"Fitted:    Center=({fitResult.Center.X:F2}, {fitResult.Center.Y:F2}, {fitResult.Center.Z:F2}), Radius={fitResult.Radius:F3}");
    Console.WriteLine($"RMS Error: {fitResult.RmsError:F6}");

    var centerError = (fitResult.Center - expectedCenter).VectorENorm();
    var radiusError = Math.Abs(fitResult.Radius - expectedRadius);

    Console.WriteLine($"Center Error: {centerError:F6}");
    Console.WriteLine($"Radius Error: {radiusError:F6}");

    if (fitResult.RmsError < 0.001)
        Console.WriteLine("✓ PASS: Circle fitting is very accurate!\n");
    else
        Console.WriteLine($"✗ FAIL: RMS Error too high: {fitResult.RmsError:F6}\n");
}
else
{
    Console.WriteLine("✗ FAIL: Circle fitting returned null!\n");
}

// Test 2: Single Perfect Arc - sollte nur 2 Punkte (Start/End) ergeben
Console.WriteLine("═══ TEST 2: Single 90° Arc Segmentation ═══");
Console.WriteLine();

var singleArc = GenerateArcPoints(
    LinFloat64Vector3D.Create(1, 1, 0),
    2.0,
    LinFloat64Vector3D.E3,
    0,
    Math.PI / 2,
    50
);

Console.WriteLine($"Generated: {singleArc.Count} points on perfect 90° arc");

// Teste verschiedene maxError Werte
var maxErrors = new[] { 0.001, 0.01, 0.05, 0.1 };
foreach (var maxError in maxErrors)
{
    var segmented = ArcSegmentationFitter.SegmentIntoArcs(
        singleArc,
        maxError: maxError,
        minPointsPerArc: 3,
        maxPointsPerArc: 30
    );

    Console.WriteLine($"  maxError={maxError:F3}: {singleArc.Count} → {segmented.Count} points = {segmented.Count - 1} segments");
}

Console.WriteLine("\nAdaptive:");
var adaptiveSingle = ArcSegmentationFitter.SegmentIntoArcsAdaptive(singleArc);
Console.WriteLine($"  {singleArc.Count} → {adaptiveSingle.Count} points = {adaptiveSingle.Count - 1} segments");

if (adaptiveSingle.Count == 2)
    Console.WriteLine("✓ PASS: Perfect! Recognized as single arc.\n");
else if (adaptiveSingle.Count <= 5)
    Console.WriteLine($"○ OK: {adaptiveSingle.Count - 1} segments for single arc (acceptable)\n");
else
    Console.WriteLine($"✗ FAIL: Too many segments ({adaptiveSingle.Count - 1}) for a perfect arc!\n");

// Test 3: S-Kurve (2 Bögen) - sollte 2-3 Segmente ergeben
Console.WriteLine("═══ TEST 3: S-Curve (2 Arcs) Segmentation ═══");
Console.WriteLine();

var sCurve = GenerateSCurve(20);
Console.WriteLine($"Generated: {sCurve.Count} points on S-curve (2 arcs, 90° each)");

foreach (var maxError in maxErrors)
{
    var segmented = ArcSegmentationFitter.SegmentIntoArcs(
        sCurve,
        maxError: maxError,
        minPointsPerArc: 3,
        maxPointsPerArc: 30
    );

    Console.WriteLine($"  maxError={maxError:F3}: {sCurve.Count} → {segmented.Count} points = {segmented.Count - 1} segments");
}

Console.WriteLine("\nAdaptive:");
var adaptiveS = ArcSegmentationFitter.SegmentIntoArcsAdaptive(sCurve);
Console.WriteLine($"  {sCurve.Count} → {adaptiveS.Count} points = {adaptiveS.Count - 1} segments");

if (adaptiveS.Count >= 3 && adaptiveS.Count <= 4)
    Console.WriteLine("✓ PASS: Recognized 2-3 segments for 2 arcs!\n");
else
    Console.WriteLine($"✗ FAIL: Expected 2-3 segments, got {adaptiveS.Count - 1}\n");

// Test 4: Langer Bogen mit vielen Punkten - "Abkürzungs"-Problem
Console.WriteLine("═══ TEST 4: Long Arc (180°) - Shortcut Problem ═══");
Console.WriteLine();

var longArc = GenerateArcPoints(
    LinFloat64Vector3D.Create(0, 0, 0),
    3.0,
    LinFloat64Vector3D.E3,
    0,
    Math.PI, // 180°
    100 // viele Punkte
);

Console.WriteLine($"Generated: {longArc.Count} points on 180° arc");
Console.WriteLine("Testing if algorithm takes shortcuts...\n");

// Test mit verschiedenen Parametern
var testConfigs = new[]
{
    (maxError: 0.01, minPts: 3, maxPts: 30, name: "Conservative"),
    (maxError: 0.05, minPts: 3, maxPts: 30, name: "Default"),
    (maxError: 0.1, minPts: 5, maxPts: 50, name: "Aggressive")
};

foreach (var (maxError, minPts, maxPts, name) in testConfigs)
{
    var segmented = ArcSegmentationFitter.SegmentIntoArcs(
        longArc,
        maxError: maxError,
        minPointsPerArc: minPts,
        maxPointsPerArc: maxPts
    );

    Console.WriteLine($"  {name} (maxErr={maxError:F2}, minPts={minPts}, maxPts={maxPts}):");
    Console.WriteLine($"    {longArc.Count} → {segmented.Count} points = {segmented.Count - 1} segments");

    // Prüfe ob es "Abkürzungen" nimmt
    if (segmented.Count == 2)
    {
        // Nur Start + End = direkte Linie! Das ist schlecht für einen Bogen
        var chordLength = (segmented[1] - segmented[0]).VectorENorm();
        var arcLength = Math.PI * 3.0; // r * π für 180°
        Console.WriteLine($"    ⚠ WARNING: Only 2 points! Chord={chordLength:F2}, Arc={arcLength:F2}");
        Console.WriteLine($"    This is a shortcut! Should follow the curve.\n");
    }
    else
    {
        Console.WriteLine($"    ✓ Follows curve with {segmented.Count - 1} segments\n");
    }
}

// Test 5: Verschiedene Bogenwinkel
Console.WriteLine("═══ TEST 5: Various Arc Angles ═══");
Console.WriteLine();

var angles = new[] { (30.0, "30°"), (60.0, "60°"), (120.0, "120°"), (270.0, "270°") };
foreach (var (angleDeg, label) in angles)
{
    var arc = GenerateArcPoints(
        LinFloat64Vector3D.Create(0, 0, 0),
        2.0,
        LinFloat64Vector3D.E3,
        0,
        angleDeg * Math.PI / 180.0,
        30
    );

    var segmented = ArcSegmentationFitter.SegmentIntoArcsAdaptive(arc);
    Console.WriteLine($"{label} arc: {arc.Count} → {segmented.Count} points = {segmented.Count - 1} segments");

    if (segmented.Count <= 3)
        Console.WriteLine($"  ✓ Good: {segmented.Count - 1} segments for {label} arc");
    else
        Console.WriteLine($"  ✗ Too many segments: {segmented.Count - 1} for {label} arc");
}

// Test 6: Verschiedene Radien
Console.WriteLine("\n═══ TEST 6: Various Radii ═══");
Console.WriteLine();

var radii = new[] { (0.5, "small"), (2.0, "medium"), (10.0, "large"), (50.0, "very large") };
foreach (var (radius, label) in radii)
{
    var arc = GenerateArcPoints(
        LinFloat64Vector3D.Create(0, 0, 0),
        radius,
        LinFloat64Vector3D.E3,
        0,
        Math.PI / 2,
        30
    );

    var segmented = ArcSegmentationFitter.SegmentIntoArcsAdaptive(arc);
    Console.WriteLine($"{label} radius ({radius}): {arc.Count} → {segmented.Count} points = {segmented.Count - 1} segments");

    if (segmented.Count <= 3)
        Console.WriteLine($"  ✓ Good: {segmented.Count - 1} segments");
    else
        Console.WriteLine($"  ✗ Too many segments: {segmented.Count - 1}");
}

// Test 7: Verschiedene Punktdichten
Console.WriteLine("\n═══ TEST 7: Various Point Densities ═══");
Console.WriteLine();

var densities = new[] { (5, "very sparse"), (10, "sparse"), (30, "normal"), (100, "dense"), (200, "very dense") };
foreach (var (numPoints, label) in densities)
{
    var arc = GenerateArcPoints(
        LinFloat64Vector3D.Create(0, 0, 0),
        2.0,
        LinFloat64Vector3D.E3,
        0,
        Math.PI / 2,
        numPoints
    );

    var segmented = ArcSegmentationFitter.SegmentIntoArcsAdaptive(arc);
    Console.WriteLine($"{label} ({numPoints} pts): {arc.Count} → {segmented.Count} points = {segmented.Count - 1} segments");

    if (segmented.Count <= 3)
        Console.WriteLine($"  ✓ Good: {segmented.Count - 1} segments");
    else
        Console.WriteLine($"  ✗ Too many segments: {segmented.Count - 1}");
}

// Test 8: Fast-gerade Linie (sehr großer Radius)
Console.WriteLine("\n═══ TEST 8: Nearly Straight Line (Large Radius) ═══");
Console.WriteLine();

var straightArc = GenerateArcPoints(
    LinFloat64Vector3D.Create(0, 0, 0),
    100.0,  // sehr großer Radius
    LinFloat64Vector3D.E3,
    0,
    Math.PI / 20,  // nur 9°
    30
);

var straightSegmented = ArcSegmentationFitter.SegmentIntoArcsAdaptive(straightArc);
Console.WriteLine($"Nearly straight: {straightArc.Count} → {straightSegmented.Count} points = {straightSegmented.Count - 1} segments");

if (straightSegmented.Count == 2)
    Console.WriteLine("  ✓ Perfect: Recognized as single segment");
else if (straightSegmented.Count <= 3)
    Console.WriteLine($"  ○ OK: {straightSegmented.Count - 1} segments for nearly straight line");
else
    Console.WriteLine($"  ✗ FAIL: Too many segments ({straightSegmented.Count - 1}) for nearly straight line");

// Test 9: Enger Bogen (sehr kleiner Radius)
Console.WriteLine("\n═══ TEST 9: Tight Arc (Small Radius) ═══");
Console.WriteLine();

var tightArc = GenerateArcPoints(
    LinFloat64Vector3D.Create(0, 0, 0),
    0.2,  // sehr kleiner Radius
    LinFloat64Vector3D.E3,
    0,
    Math.PI,  // 180°
    30
);

var tightSegmented = ArcSegmentationFitter.SegmentIntoArcsAdaptive(tightArc);
Console.WriteLine($"Tight arc (r=0.2): {tightArc.Count} → {tightSegmented.Count} points = {tightSegmented.Count - 1} segments");

if (tightSegmented.Count <= 3)
    Console.WriteLine($"  ✓ Good: {tightSegmented.Count - 1} segments");
else
    Console.WriteLine($"  ✗ Too many segments: {tightSegmented.Count - 1}");

// Test 10: Komplexe Kurve (3 verschiedene Bögen)
Console.WriteLine("\n═══ TEST 10: Complex Curve (3 Different Arcs) ═══");
Console.WriteLine();

var complexCurve = new List<LinFloat64Vector3D>();

// Arc 1: 90° mit Radius 1.0
var arc1 = GenerateArcPoints(
    LinFloat64Vector3D.Create(1, 0, 0),
    1.0,
    LinFloat64Vector3D.E3,
    Math.PI,
    Math.PI / 2,
    20
);
complexCurve.AddRange(arc1);

// Arc 2: 45° mit Radius 2.0 (anderer Radius!)
var arc2Start = arc1[^1];
var arc2 = GenerateArcPoints(
    LinFloat64Vector3D.Create(1, 2, 0),
    2.0,
    LinFloat64Vector3D.E3,
    -Math.PI / 2,
    -Math.PI / 4,
    15
);
complexCurve.AddRange(arc2.Skip(1));

// Arc 3: 60° mit Radius 1.5 (wieder anderer Radius!)
var arc3 = GenerateArcPoints(
    LinFloat64Vector3D.Create(2.414, 2, 0),
    1.5,
    LinFloat64Vector3D.E3,
    Math.PI,
    Math.PI - Math.PI / 3,
    20
);
complexCurve.AddRange(arc3.Skip(1));

var complexSegmented = ArcSegmentationFitter.SegmentIntoArcsAdaptive(complexCurve);
Console.WriteLine($"3 arcs (different radii): {complexCurve.Count} → {complexSegmented.Count} points = {complexSegmented.Count - 1} segments");

if (complexSegmented.Count >= 4 && complexSegmented.Count <= 6)
    Console.WriteLine($"  ✓ Good: {complexSegmented.Count - 1} segments for 3 arcs (expected 3-5)");
else
    Console.WriteLine($"  ✗ Expected 3-5 segments, got {complexSegmented.Count - 1}");

// Test 11: Bogen mit Rauschen
Console.WriteLine("\n═══ TEST 11: Arc with Noise ═══");
Console.WriteLine();

var random = new Random(42);
var noisyArc = GenerateArcPoints(
    LinFloat64Vector3D.Create(0, 0, 0),
    2.0,
    LinFloat64Vector3D.E3,
    0,
    Math.PI / 2,
    50
);

// Füge kleines Rauschen hinzu (±1% des Radius)
var noiseLevel = 0.02;
for (int i = 1; i < noisyArc.Count - 1; i++) // Nicht Start/End
{
    var noise = (random.NextDouble() - 0.5) * 2 * noiseLevel;
    noisyArc[i] = noisyArc[i] + LinFloat64Vector3D.Create(noise, noise, 0);
}

var noisySegmented = ArcSegmentationFitter.SegmentIntoArcsAdaptive(noisyArc);
Console.WriteLine($"Noisy arc (±{noiseLevel * 100}%) - Adaptive: {noisyArc.Count} → {noisySegmented.Count} points = {noisySegmented.Count - 1} segments");

// Test mit manuellem maxError
var noisyManual = ArcSegmentationFitter.SegmentIntoArcs(noisyArc, maxError: 0.1);
Console.WriteLine($"Noisy arc (±{noiseLevel * 100}%) - maxError=0.1: {noisyArc.Count} → {noisyManual.Count} points = {noisyManual.Count - 1} segments");

var noisyManual2 = ArcSegmentationFitter.SegmentIntoArcs(noisyArc, maxError: 0.2);
Console.WriteLine($"Noisy arc (±{noiseLevel * 100}%) - maxError=0.2: {noisyArc.Count} → {noisyManual2.Count} points = {noisyManual2.Count - 1} segments");

var noisyManual3 = ArcSegmentationFitter.SegmentIntoArcs(noisyArc, maxError: 0.2, minPointsPerArc: 10);
Console.WriteLine($"Noisy arc (±{noiseLevel * 100}%) - maxError=0.2, minPts=10: {noisyArc.Count} → {noisyManual3.Count} points = {noisyManual3.Count - 1} segments");

if (noisyManual.Count <= 5)
    Console.WriteLine($"  ✓ Good: {noisyManual.Count - 1} segments with maxError=0.1");
else
    Console.WriteLine($"  ✗ Too many segments ({noisyManual.Count - 1}) even with maxError=0.1");

Console.WriteLine("\n═══ TEST SUMMARY ═══");
Console.WriteLine("Check results above to identify issues:");
Console.WriteLine("1. Circle fitting should have RMS < 0.001");
Console.WriteLine("2. Single arc should result in 1-2 segments");
Console.WriteLine("3. S-curve (2 arcs) should result in 2-3 segments");
Console.WriteLine("4. Long arc should NOT take shortcuts (need multiple segments)");
Console.WriteLine("5. Various angles should result in 1-2 segments each");
Console.WriteLine("6. Various radii should result in 1-2 segments each");
Console.WriteLine("7. Various point densities should result in 1-2 segments");
Console.WriteLine("8. Nearly straight lines should result in 1 segment");
Console.WriteLine("9. Tight arcs should result in 1-2 segments");
Console.WriteLine("10. Complex curves (3 arcs) should result in 3-5 segments");
Console.WriteLine("11. Noisy arcs should result in 1-5 segments");
Console.WriteLine();
Console.WriteLine("\nAll tests completed!");


// Helper Functions
static List<LinFloat64Vector3D> GenerateArcPoints(
    LinFloat64Vector3D center,
    double radius,
    LinFloat64Vector3D normal,
    double startAngle,
    double endAngle,
    int numPoints)
{
    var points = new List<LinFloat64Vector3D>();

    var arbitrary = Math.Abs(normal.Z) < 0.9 ? LinFloat64Vector3D.E3 : LinFloat64Vector3D.E1;
    var u = normal.VectorCross(arbitrary);
    u = LinFloat64Vector3D.CreateUnitVector(u.X, u.Y, u.Z);
    var v = normal.VectorCross(u);
    v = LinFloat64Vector3D.CreateUnitVector(v.X, v.Y, v.Z);

    for (int i = 0; i < numPoints; i++)
    {
        var t = i / (numPoints - 1.0);
        var angle = startAngle + t * (endAngle - startAngle);
        var point = center + u * (radius * Math.Cos(angle)) + v * (radius * Math.Sin(angle));
        points.Add(point);
    }

    return points;
}

static List<LinFloat64Vector3D> GenerateSCurve(int pointsPerArc)
{
    var allPoints = new List<LinFloat64Vector3D>();

    var arc1 = GenerateArcPoints(
        LinFloat64Vector3D.Create(1, 0, 0),
        1.0,
        LinFloat64Vector3D.E3,
        Math.PI,
        Math.PI / 2,
        pointsPerArc
    );
    allPoints.AddRange(arc1);

    var arc2 = GenerateArcPoints(
        LinFloat64Vector3D.Create(1, 2, 0),
        1.0,
        LinFloat64Vector3D.E3,
        -Math.PI / 2,
        0,
        pointsPerArc
    );
    allPoints.AddRange(arc2.Skip(1));

    return allPoints;
}
