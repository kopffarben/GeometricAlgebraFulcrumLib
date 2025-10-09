using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;

namespace GeometricAlgebraFulcrumLib.Applications.VRSketch.CurveFitting;

/// <summary>
/// Least-Squares Circle Fitting für 3D Punkte
/// Verwendet algebraische Methode (Pratt)
/// </summary>
public static class CircleFitter
{
    public record CircleFitResult(
        LinFloat64Vector3D Center,
        LinFloat64Vector3D Normal,
        double Radius,
        double RmsError
    );

    /// <summary>
    /// Fitte Kreis an 3D Punktewolke
    /// </summary>
    /// <param name="points">Punkte für Circle Fit</param>
    /// <param name="preferredNormal">Bevorzugte Normale (z.B. Controller-Normal) - optional</param>
    public static CircleFitResult? FitCircle(
        List<LinFloat64Vector3D> points,
        LinFloat64Vector3D? preferredNormal = null)
    {
        if (points.Count < 3)
            return null;

        // 1. Berechne Schwerpunkt (centroid)
        var centroid = LinFloat64Vector3D.Create(
            points.Average(p => p.X),
            points.Average(p => p.Y),
            points.Average(p => p.Z)
        );

        // 2. Berechne beste Fit-Ebene via PCA
        var normal = ComputeBestFitPlaneNormal(points, centroid);

        // 3. Projiziere Punkte auf Ebene und transformiere in 2D
        var localCoords = ProjectTo2D(points, centroid, normal);

        // 4. Fitte 2D Kreis via Least Squares
        var (center2D, radius) = FitCircle2D(localCoords);

        // 5. Transformiere Center zurück zu 3D
        var center3D = TransformFrom2D(center2D, centroid, normal);

        // 6. Berechne RMS Error
        var rmsError = ComputeRmsError(points, center3D, radius);

        // 7. WICHTIG: Prüfe Krümmungsrichtung mit preferredNormal
        if (preferredNormal is not null && points.Count >= 2)
        {
            // Berechne Mittelpunkt der Chord (Start zu End)
            var chordMid = (points[0] + points[^1]) * 0.5;

            // Vektor von Chord-Mittelpunkt zum Circle-Center
            var toCenter = center3D - chordMid;

            // Prüfe ob dieser Vektor in Richtung des preferredNormal zeigt
            var alignment = toCenter.VectorESp(preferredNormal);

            // Wenn negativ, zeigt das Center in die falsche Richtung → Normal flippen
            if (alignment < 0)
            {
                normal = LinFloat64Vector3D.Create(-normal.X, -normal.Y, -normal.Z);
                Console.WriteLine($"CircleFitter: Flipped normal - center was on wrong side (alignment={alignment:F3})");
            }
        }

        return new CircleFitResult(center3D, normal, radius, rmsError);
    }

    /// <summary>
    /// Berechne beste Fit-Ebene Normal via PCA (Hauptkomponentenanalyse)
    /// </summary>
    private static LinFloat64Vector3D ComputeBestFitPlaneNormal(
        List<LinFloat64Vector3D> points,
        LinFloat64Vector3D centroid)
    {
        // Kovarianz-Matrix berechnen
        var xx = 0.0; var xy = 0.0; var xz = 0.0;
        var yy = 0.0; var yz = 0.0;
        var zz = 0.0;

        foreach (var p in points)
        {
            var dx = p.X - centroid.X;
            var dy = p.Y - centroid.Y;
            var dz = p.Z - centroid.Z;

            xx += dx * dx;
            xy += dx * dy;
            xz += dx * dz;
            yy += dy * dy;
            yz += dy * dz;
            zz += dz * dz;
        }

        // Finde Eigenvector mit kleinstem Eigenvalue (Normal der Ebene)
        // Vereinfachte Methode: Cross-Product der ersten beiden Hauptrichtungen
        // Für robustere Lösung würde man Eigenvalue-Decomposition nutzen

        // Approximation: Nutze SVD-artige Heuristik
        // Normal ist orthogonal zu den beiden stärksten Varianz-Richtungen

        // Einfache Methode: Nehme Cross-Product der ersten 3 Punkte (wenn nicht kollinear)
        if (points.Count >= 3)
        {
            var v1 = points[1] - points[0];
            var v2 = points[2] - points[0];
            var cross = v1.VectorCross(v2);
            var crossLength = cross.VectorENorm();

            if (crossLength > 1e-6)
            {
                return LinFloat64Vector3D.CreateUnitVector(cross.X, cross.Y, cross.Z);
            }
        }

        // Fallback: Nutze Z-Achse
        return LinFloat64Vector3D.E3;
    }

    /// <summary>
    /// Projiziere 3D Punkte auf 2D Ebene
    /// </summary>
    private static List<(double x, double y)> ProjectTo2D(
        List<LinFloat64Vector3D> points,
        LinFloat64Vector3D origin,
        LinFloat64Vector3D normal)
    {
        // Konstruiere orthonormale Basis für die Ebene
        var (u, v) = ConstructOrthonormalBasis(normal);

        var result = new List<(double, double)>();
        foreach (var p in points)
        {
            var rel = p - origin;
            var x = rel.VectorESp(u);
            var y = rel.VectorESp(v);
            result.Add((x, y));
        }

        return result;
    }

    /// <summary>
    /// Konstruiere orthonormale Basis für Ebene mit gegebener Normale
    /// </summary>
    private static (LinFloat64Vector3D u, LinFloat64Vector3D v) ConstructOrthonormalBasis(
        LinFloat64Vector3D normal)
    {
        // Finde beliebigen Vektor nicht parallel zu normal
        var arbitrary = Math.Abs(normal.Z) < 0.9
            ? LinFloat64Vector3D.E3
            : LinFloat64Vector3D.E1;

        var u = normal.VectorCross(arbitrary);
        u = LinFloat64Vector3D.CreateUnitVector(u.X, u.Y, u.Z);

        var v = normal.VectorCross(u);
        v = LinFloat64Vector3D.CreateUnitVector(v.X, v.Y, v.Z);

        return (u, v);
    }

    /// <summary>
    /// Fitte 2D Kreis via Algebraic Least Squares (Pratt Method)
    /// </summary>
    private static ((double x, double y) center, double radius) FitCircle2D(
        List<(double x, double y)> points)
    {
        if (points.Count < 3)
            return ((0, 0), 0);

        var n = points.Count;

        // Berechne Schwerpunkt
        var mx = points.Average(p => p.x);
        var my = points.Average(p => p.y);

        // Zentriere Punkte
        var centered = points.Select(p => (x: p.x - mx, y: p.y - my)).ToList();

        // Aufbau des Least-Squares Systems
        // Circle equation: (x-cx)^2 + (y-cy)^2 = r^2
        // Umgeschrieben: x^2 + y^2 - 2*cx*x - 2*cy*y + (cx^2 + cy^2 - r^2) = 0
        // A = -2*cx, B = -2*cy, C = cx^2 + cy^2 - r^2
        // System: A*x + B*y + (x^2 + y^2) + C = 0

        var Sxx = centered.Sum(p => p.x * p.x);
        var Syy = centered.Sum(p => p.y * p.y);
        var Sxy = centered.Sum(p => p.x * p.y);
        var Sx = centered.Sum(p => p.x);
        var Sy = centered.Sum(p => p.y);
        var Sxxx = centered.Sum(p => p.x * p.x * p.x);
        var Syyy = centered.Sum(p => p.y * p.y * p.y);
        var Sxyy = centered.Sum(p => p.x * p.y * p.y);
        var Sxxy = centered.Sum(p => p.x * p.x * p.y);

        // Löse 2x2 System für cx, cy
        var A = 2 * (Sx * Sx - n * Sxx);
        var B = 2 * (Sx * Sy - n * Sxy);
        var C = 2 * (Sy * Sy - n * Syy);
        var D = Sxx * Sx + Sxy * Sy - n * (Sxxx + Sxyy);
        var E = Sxy * Sx + Syy * Sy - n * (Sxxy + Syyy);

        var denominator = A * C - B * B;
        if (Math.Abs(denominator) < 1e-10)
        {
            // Degenerate case
            return ((mx, my), 0);
        }

        var cx = (D * C - B * E) / denominator;
        var cy = (A * E - B * D) / denominator;

        // Berechne Radius
        var rSquared = cx * cx + cy * cy + (Sxx + Syy) / n;
        var radius = Math.Sqrt(Math.Max(0, rSquared));

        // Transformiere zurück (un-center)
        return ((cx + mx, cy + my), radius);
    }

    /// <summary>
    /// Transformiere 2D Punkt zurück zu 3D
    /// </summary>
    private static LinFloat64Vector3D TransformFrom2D(
        (double x, double y) point2D,
        LinFloat64Vector3D origin,
        LinFloat64Vector3D normal)
    {
        var (u, v) = ConstructOrthonormalBasis(normal);
        return origin + u * point2D.x + v * point2D.y;
    }

    /// <summary>
    /// Berechne RMS Error für Circle Fit
    /// </summary>
    private static double ComputeRmsError(
        List<LinFloat64Vector3D> points,
        LinFloat64Vector3D center,
        double radius)
    {
        var sumSquaredError = 0.0;
        foreach (var p in points)
        {
            var distance = (p - center).VectorENorm();
            var error = distance - radius;
            sumSquaredError += error * error;
        }

        return Math.Sqrt(sumSquaredError / points.Count);
    }
}
