using System.Globalization;
using System.Text;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Applications.VRSketch.CurveFitting;
using static GeometricAlgebraFulcrumLib.Applications.VRSketch.Prototypes.ArcSplinePrototype;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddCors();

var app = builder.Build();

// Enable static files
app.UseStaticFiles();

// Enable CORS for local development
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// API: Calculate Arc-Spline with Curve Fitting
app.MapPost("/api/arcspline/fit", (ArcSplineFitRequest request) =>
{
    try
    {
        if (request.RawPoints.Count < 2)
            return Results.BadRequest("Need at least 2 points");

        // Apply Curve Fitting based on method
        List<Point3D> fittedPoints;

        switch (request.FittingMethod)
        {
            case "douglas-peucker":
                var rawVectors = request.RawPoints
                    .Select(p => LinFloat64Vector3D.Create(p.X, p.Y, p.Z))
                    .ToList();

                // WICHTIG: Erst Resampling auf gleichmäßige Abstände!
                var resampledDP = CurveResampler.ResampleToUniformDistance(rawVectors);
                Console.WriteLine($"Douglas-Peucker: Resampling {rawVectors.Count} → {resampledDP.Count} points");

                var epsilon = request.Epsilon > 0
                    ? request.Epsilon
                    : DouglasPeuckerSimplifier.EstimateEpsilon(resampledDP, targetReductionFactor: 0.15);

                var simplified = DouglasPeuckerSimplifier.Simplify(resampledDP, epsilon);

                fittedPoints = simplified
                    .Select(v => new Point3D(v.X, v.Y, v.Z))
                    .ToList();
                break;

            case "none":
                fittedPoints = request.RawPoints;
                break;

            case "arc-fit":
                var arcRawVectors = request.RawPoints
                    .Select(p => LinFloat64Vector3D.Create(p.X, p.Y, p.Z))
                    .ToList();

                // WICHTIG: Erst Resampling auf gleichmäßige Abstände!
                var resampledArc = CurveResampler.ResampleToUniformDistance(arcRawVectors);
                Console.WriteLine($"Arc-Fit: Resampling {arcRawVectors.Count} → {resampledArc.Count} points");

                // Verwende Epsilon als targetReductionFactor (0.0 - 1.0)
                var targetReduction = request.Epsilon > 0 ? request.Epsilon : 0.15;

                // Controller Normal für korrekte Krümmungsrichtung
                var controllerNormalArc = LinFloat64Vector3D.Create(
                    request.ControllerNormal.X,
                    request.ControllerNormal.Y,
                    request.ControllerNormal.Z
                );

                // NEUE METHODE: Hole Circle-Fit Ergebnisse direkt!
                var arcSegmentsWithFit = ArcSegmentationFitter.SegmentIntoArcsWithFit(
                    resampledArc,
                    targetReductionFactor: targetReduction,
                    minPointsPerArc: 3,
                    preferredNormal: controllerNormalArc);

                // Baue Arc-Spline DIREKT aus gefitteten Kreisen
                if (arcSegmentsWithFit.Count > 0)
                {
                    var splineArcFit = new ArcSpline();

                    foreach (var seg in arcSegmentsWithFit)
                    {
                        splineArcFit.AddArcFromCircleFit(
                            seg.StartPoint,
                            seg.EndPoint,
                            seg.CircleFit.Center,
                            seg.CircleFit.Normal,
                            seg.CircleFit.Radius
                        );
                    }

                    var jsonStringArc = splineArcFit.ExportToJson(samplesPerSegment: 30);
                    var jsonObjectArc = System.Text.Json.JsonSerializer.Deserialize<object>(jsonStringArc);

                    Console.WriteLine($"\n=== Arc-Fit: {request.RawPoints.Count} → {arcSegmentsWithFit.Count} fitted arcs ===");
                    for (int i = 0; i < splineArcFit.Segments.Count; i++)
                    {
                        var s = splineArcFit.Segments[i];
                        var decoded = s.Circle.DecodeIpnsRound.Element();
                        Console.WriteLine($"  Arc {i}: Radius={decoded.RealRadius:F3}, ArcLength={s.ArcLength:F3}");
                    }

                    return Results.Ok(jsonObjectArc);
                }

                // Fallback wenn kein Arc gefunden
                fittedPoints = request.RawPoints;
                break;

            default:
                return Results.BadRequest($"Unknown fitting method: {request.FittingMethod}");
        }

        // Build Arc-Spline from fitted points
        if (fittedPoints.Count < 2)
            return Results.BadRequest("Fitting resulted in too few points");

        var spline = new ArcSpline();
        var normal = LinFloat64Vector3D.Create(
            request.ControllerNormal.X,
            request.ControllerNormal.Y,
            request.ControllerNormal.Z
        );

        var p1 = LinFloat64Vector3D.Create(fittedPoints[0].X, fittedPoints[0].Y, fittedPoints[0].Z);
        var p2 = LinFloat64Vector3D.Create(fittedPoints[1].X, fittedPoints[1].Y, fittedPoints[1].Z);
        spline.AddFirstSegment(p1, p2, normal, curvatureScale: 0.5);

        for (int i = 2; i < fittedPoints.Count; i++)
        {
            var nextPoint = LinFloat64Vector3D.Create(
                fittedPoints[i].X,
                fittedPoints[i].Y,
                fittedPoints[i].Z
            );
            spline.AddSegmentFromController(nextPoint, normal, curvatureScale: 0.5);
        }

        var jsonString = spline.ExportToJson(samplesPerSegment: 30);
        var jsonObject = System.Text.Json.JsonSerializer.Deserialize<object>(jsonString);

        // Debug: Zeige Arc-Radien und Krümmung
        Console.WriteLine($"\n=== Curve Fit: {request.RawPoints.Count} → {fittedPoints.Count} points ===");
        Console.WriteLine($"Arc-Spline with {spline.Segments.Count} segments:");
        for (int i = 0; i < spline.Segments.Count; i++)
        {
            var seg = spline.Segments[i];
            var decoded = seg.Circle.DecodeIpnsRound.Element();
            var radius = decoded.RealRadius;
            var arcLength = seg.ArcLength;
            var curvature = 1.0 / radius;
            Console.WriteLine($"  Segment {i}: Radius={radius:F3}, ArcLength={arcLength:F3}, Curvature={curvature:F4}");
        }

        return Results.Ok(jsonObject);
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error: {ex.Message}");
    }
});

// API: Calculate Arc-Spline from points (legacy, direct mode)
app.MapPost("/api/arcspline", (ArcSplineRequest request) =>
{
    try
    {
        var spline = new ArcSpline();

        if (request.Points.Count < 2)
            return Results.BadRequest("Need at least 2 points");

        // Add first segment
        var p1 = LinFloat64Vector3D.Create(request.Points[0].X, request.Points[0].Y, request.Points[0].Z);
        var p2 = LinFloat64Vector3D.Create(request.Points[1].X, request.Points[1].Y, request.Points[1].Z);
        var normal = LinFloat64Vector3D.Create(
            request.ControllerNormal.X,
            request.ControllerNormal.Y,
            request.ControllerNormal.Z
        );

        spline.AddFirstSegment(p1, p2, normal, curvatureScale: 1.0);

        // Add remaining segments
        for (int i = 2; i < request.Points.Count; i++)
        {
            var nextPoint = LinFloat64Vector3D.Create(
                request.Points[i].X,
                request.Points[i].Y,
                request.Points[i].Z
            );
            spline.AddSegmentFromController(nextPoint, normal, curvatureScale: 1.0);
        }

        // Export to JSON string
        var jsonString = spline.ExportToJson(samplesPerSegment: 30);

        // Parse JSON string to object so it can be properly serialized by ASP.NET
        var jsonObject = System.Text.Json.JsonSerializer.Deserialize<object>(jsonString);

        // Debug logging
        Console.WriteLine($"\n=== Spline computed with {spline.Segments.Count} segments ===");

        return Results.Ok(jsonObject);
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error: {ex.Message}");
    }
});

// Test endpoint for arc fitting
app.MapGet("/api/test/arcfitting", () =>
{
    try
    {
        Console.WriteLine("\n" + new string('=', 80));
        ArcFittingTests.RunAllTests();
        Console.WriteLine(new string('=', 80) + "\n");

        return Results.Ok(new { message = "Tests completed - check console output" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Test error: {ex.Message}\n{ex.StackTrace}");
    }
});

// Root endpoint redirects to index.html
app.MapGet("/", () => Results.Redirect("/index.html"));

app.Run();

// Request/Response models
public record Point3D(double X, double Y, double Z);
public record ArcSplineRequest(List<Point3D> Points, Point3D ControllerNormal);
public record ArcSplineFitRequest(
    List<Point3D> RawPoints,
    Point3D ControllerNormal,
    string FittingMethod = "douglas-peucker",
    double Epsilon = 0.0
);
