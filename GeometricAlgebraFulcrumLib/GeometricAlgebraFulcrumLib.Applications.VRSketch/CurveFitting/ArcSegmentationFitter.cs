using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;

namespace GeometricAlgebraFulcrumLib.Applications.VRSketch.CurveFitting;

/// <summary>
/// Segmentiert eine Punktfolge in kreisbogenförmige Abschnitte
/// Verwendet Circle Fitting + Sliding Window Approach
/// </summary>
public static class ArcSegmentationFitter
{
    /// <summary>
    /// Segmentiere Punktfolge in Arc-Segmente mit Krümmungs-Analyse
    /// </summary>
    /// <param name="points">Input Punkte (z.B. aus Maus-Drawing)</param>
    /// <param name="maxError">Maximaler RMS Error für Arc-Fit (default: 0.05)</param>
    /// <param name="minPointsPerArc">Minimale Punkte pro Arc (default: 3)</param>
    /// <param name="maxPointsPerArc">Maximale Punkte pro Arc (default: 100)</param>
    /// <param name="preferredNormal">Bevorzugte Normal-Richtung (z.B. Controller-Normal)</param>
    /// <returns>Liste der Segmentierungs-Punkte (Start/End jedes Arc)</returns>
    public record ArcSegmentInfo(
        LinFloat64Vector3D StartPoint,
        LinFloat64Vector3D EndPoint,
        CircleFitter.CircleFitResult CircleFit
    );

    public static List<LinFloat64Vector3D> SegmentIntoArcs(
        List<LinFloat64Vector3D> points,
        double maxError = 0.05,
        int minPointsPerArc = 3,
        int maxPointsPerArc = 100,
        LinFloat64Vector3D? preferredNormal = null)
    {
        if (points.Count < minPointsPerArc)
            return points;

        var keyPoints = new List<LinFloat64Vector3D>();
        keyPoints.Add(points[0]);

        var currentIndex = 0;

        while (currentIndex < points.Count - 1)
        {
            var bestSegmentEnd = currentIndex + minPointsPerArc;
            CircleFitter.CircleFitResult? bestFit = null;
            double bestScore = double.MaxValue;

            // STRATEGIE: Finde das LÄNGSTE Segment mit akzeptablem Fit
            // Teste alle möglichen Längen und wähle das beste Score/Length Verhältnis
            for (int endIndex = currentIndex + minPointsPerArc;
                 endIndex <= Math.Min(currentIndex + maxPointsPerArc, points.Count);
                 endIndex++)
            {
                var segmentPoints = points.GetRange(currentIndex, endIndex - currentIndex);
                var fit = CircleFitter.FitCircle(segmentPoints, preferredNormal);

                if (fit == null)
                {
                    // Debug: Warum schlägt Circle Fit fehl?
                    if (endIndex == currentIndex + minPointsPerArc)
                    {
                        Console.WriteLine($"⚠️ CircleFitter.FitCircle returned null for {segmentPoints.Count} points at index {currentIndex}");
                    }
                    continue; // Überspringe, aber versuche längere Segmente
                }

                // WICHTIG: Reject zu große Radien (fast-gerade Linien)
                // Berechne typische Segment-Länge
                var chordLength = (segmentPoints[^1] - segmentPoints[0]).VectorENorm();
                var maxReasonableRadius = chordLength * 10.0; // Max 10x der Chord-Länge

                if (fit.Radius > maxReasonableRadius)
                {
                    if (endIndex == currentIndex + minPointsPerArc)
                    {
                        Console.WriteLine($"⚠️ Rejected arc with huge radius: {fit.Radius:F2} (chord={chordLength:F2}, max={maxReasonableRadius:F2}) at index {currentIndex}");
                    }
                    continue; // Zu großer Radius → fast gerade Linie → reject
                }

                var segmentLength = endIndex - currentIndex;

                // Score: RmsError - LengthBonus
                // Bei gleichem Error werden längere Segmente bevorzugt
                // LengthBonus sollte signifikant, aber nicht zu dominant sein
                // Adaptive: Je besser der Fit, desto größer der Bonus (aber mindestens 30%)
                var qualityFactor = 1.0 - (fit.RmsError / maxError);  // 1.0 bei perfektem Fit, 0.0 bei maxError
                var lengthBonus = segmentLength * (maxError / 15.0) * Math.Max(0.3, qualityFactor);
                var score = fit.RmsError - lengthBonus;

                // Akzeptiere wenn Error unter Threshold UND Score besser als vorheriges
                if (fit.RmsError <= maxError && score < bestScore)
                {
                    bestSegmentEnd = endIndex;
                    bestFit = fit;
                    bestScore = score;

                    // Debug: Log accepted fit
                    if (endIndex == currentIndex + minPointsPerArc)
                    {
                        Console.WriteLine($"✓ Arc fit accepted: points={segmentLength}, radius={fit.Radius:F2}, rmsError={fit.RmsError:F4}, score={score:F4}");
                    }

                    // KEIN break! Versuche auch längere Segmente!
                }
            }

            // Füge Endpunkt hinzu
            if (bestFit != null && bestSegmentEnd > currentIndex)
            {
                keyPoints.Add(points[bestSegmentEnd - 1]);
                currentIndex = bestSegmentEnd - 1;
            }
            else
            {
                // Fallback: Füge nächsten Punkt hinzu
                Console.WriteLine($"⚠️ Arc Fit FALLBACK at index {currentIndex}: No valid arc found! (maxError={maxError:F4})");
                currentIndex += minPointsPerArc;
                if (currentIndex < points.Count)
                    keyPoints.Add(points[currentIndex]);
            }
        }

        // Stelle sicher dass Endpunkt dabei ist
        if (keyPoints[^1] != points[^1])
        {
            keyPoints.Add(points[^1]);
        }

        Console.WriteLine($"Arc Segmentation: {points.Count} raw points → {keyPoints.Count} arc control points");

        return keyPoints;
    }

    /// <summary>
    /// Alternative: Adaptive Segmentierung MIT Circle-Fit Ergebnissen
    /// </summary>
    public static List<ArcSegmentInfo> SegmentIntoArcsWithFit(
        List<LinFloat64Vector3D> points,
        double targetReductionFactor = 0.1,
        int minPointsPerArc = 3,
        LinFloat64Vector3D? preferredNormal = null)
    {
        if (points.Count < minPointsPerArc)
            return new List<ArcSegmentInfo>();

        var avgDistance = EstimateAveragePointDistance(points);
        var baseError = avgDistance * targetReductionFactor;
        var scaleFactor = 1.0 + (targetReductionFactor * 9.0);
        var estimatedError = baseError * scaleFactor;
        var maxError = Math.Max(0.001, Math.Min(0.1, estimatedError));

        Console.WriteLine($"Arc Fitting WITH FIT DATA: maxError = {maxError:F4}, avgDistance = {avgDistance:F4}");

        var segments = new List<ArcSegmentInfo>();
        var currentIndex = 0;

        while (currentIndex < points.Count - 1)
        {
            var bestSegmentEnd = currentIndex + minPointsPerArc;
            CircleFitter.CircleFitResult? bestFit = null;
            double bestScore = double.MaxValue;

            for (int endIndex = currentIndex + minPointsPerArc;
                 endIndex <= Math.Min(currentIndex + 100, points.Count);
                 endIndex++)
            {
                var segmentPoints = points.GetRange(currentIndex, endIndex - currentIndex);
                var fit = CircleFitter.FitCircle(segmentPoints, preferredNormal);

                if (fit == null) continue;

                var chordLength = (segmentPoints[^1] - segmentPoints[0]).VectorENorm();
                if (fit.Radius > chordLength * 10.0) continue;

                var segmentLength = endIndex - currentIndex;
                var qualityFactor = 1.0 - (fit.RmsError / maxError);
                var lengthBonus = segmentLength * (maxError / 15.0) * Math.Max(0.3, qualityFactor);
                var score = fit.RmsError - lengthBonus;

                if (fit.RmsError <= maxError && score < bestScore)
                {
                    bestSegmentEnd = endIndex;
                    bestFit = fit;
                    bestScore = score;
                }
            }

            if (bestFit != null && bestSegmentEnd > currentIndex)
            {
                segments.Add(new ArcSegmentInfo(
                    points[currentIndex],
                    points[bestSegmentEnd - 1],
                    bestFit
                ));
                currentIndex = bestSegmentEnd - 1;
            }
            else
            {
                currentIndex += minPointsPerArc;
            }
        }

        Console.WriteLine($"Arc Fitting Result: {points.Count} points → {segments.Count} arc segments");
        return segments;
    }

    /// <summary>
    /// Alternative: Adaptive Segmentierung mit dynamischem Error-Threshold
    /// </summary>
    public static List<LinFloat64Vector3D> SegmentIntoArcsAdaptive(
        List<LinFloat64Vector3D> points,
        double targetReductionFactor = 0.1,
        int minPointsPerArc = 3,
        LinFloat64Vector3D? preferredNormal = null)
    {
        if (points.Count < minPointsPerArc)
            return points;

        // Schätze automatisch guten maxError basierend auf Punktdichte
        var avgDistance = EstimateAveragePointDistance(points);

        // Berechne maxError basierend auf Punktdichte und targetReductionFactor
        // targetReductionFactor: 0.0 = keine Reduktion, 1.0 = maximale Reduktion
        // Formel: Je höher targetReductionFactor, desto größer maxError, desto weniger Segmente

        // Basis-Error: proportional zur Punktdichte
        var baseError = avgDistance * targetReductionFactor;

        // Scale-Faktor: Erhöht die Toleranz bei höheren targetReductionFactors
        // Bei 0.15: Faktor ~2.5, bei 0.5: Faktor ~5, bei 1.0: Faktor ~10
        var scaleFactor = 1.0 + (targetReductionFactor * 9.0);

        var estimatedError = baseError * scaleFactor;

        // Begrenze maxError auf sinnvollen Bereich:
        // Min: 0.001 (sehr präzise)
        // Max: 0.1 (sehr tolerant)
        var maxError = Math.Max(0.001, Math.Min(0.1, estimatedError));

        Console.WriteLine($"Arc Fitting: Auto-estimated maxError = {maxError:F4}, avgDistance = {avgDistance:F4}, targetReduction={targetReductionFactor:F2}");

        var result = SegmentIntoArcs(points, maxError, minPointsPerArc, maxPointsPerArc: 100, preferredNormal);

        Console.WriteLine($"Arc Fitting Result: {points.Count} → {result.Count} points ({result.Count - 1} arcs)");

        return result;
    }

    /// <summary>
    /// Berechne durchschnittliche Punkt-zu-Punkt Distanz
    /// </summary>
    private static double EstimateAveragePointDistance(List<LinFloat64Vector3D> points)
    {
        if (points.Count < 2)
            return 0.01;

        var totalDistance = 0.0;
        for (int i = 1; i < points.Count; i++)
        {
            totalDistance += (points[i] - points[i - 1]).VectorENorm();
        }

        return totalDistance / (points.Count - 1);
    }
}
