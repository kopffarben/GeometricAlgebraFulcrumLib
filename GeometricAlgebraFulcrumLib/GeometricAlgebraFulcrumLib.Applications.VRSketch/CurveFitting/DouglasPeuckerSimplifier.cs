using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;

namespace GeometricAlgebraFulcrumLib.Applications.VRSketch.CurveFitting;

/// <summary>
/// Douglas-Peucker Algorithmus für Kurven-Vereinfachung
/// Reduziert Anzahl der Punkte während die Form erhalten bleibt
/// </summary>
public static class DouglasPeuckerSimplifier
{
    /// <summary>
    /// Vereinfache eine Punktfolge mit Douglas-Peucker
    /// </summary>
    /// <param name="points">Input Punkte</param>
    /// <param name="epsilon">Toleranz (maximale Abweichung)</param>
    /// <returns>Vereinfachte Punktfolge</returns>
    public static List<LinFloat64Vector3D> Simplify(List<LinFloat64Vector3D> points, double epsilon)
    {
        if (points.Count < 3)
            return new List<LinFloat64Vector3D>(points);

        var result = new List<LinFloat64Vector3D>();
        SimplifyRecursive(points, 0, points.Count - 1, epsilon, result);
        return result;
    }

    private static void SimplifyRecursive(
        List<LinFloat64Vector3D> points,
        int startIndex,
        int endIndex,
        double epsilon,
        List<LinFloat64Vector3D> result)
    {
        // Füge Startpunkt hinzu
        if (result.Count == 0)
            result.Add(points[startIndex]);

        // Basis Fall: Nur 2 Punkte
        if (endIndex - startIndex <= 1)
        {
            result.Add(points[endIndex]);
            return;
        }

        // Finde Punkt mit maximaler Distanz zur Linie start->end
        var maxDistance = 0.0;
        var maxIndex = startIndex;

        var lineStart = points[startIndex];
        var lineEnd = points[endIndex];

        for (int i = startIndex + 1; i < endIndex; i++)
        {
            var distance = PerpendicularDistance(points[i], lineStart, lineEnd);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                maxIndex = i;
            }
        }

        // Wenn max Distanz größer als epsilon, teile rekursiv
        if (maxDistance > epsilon)
        {
            // Rekursiv: start -> maxIndex
            SimplifyRecursive(points, startIndex, maxIndex, epsilon, result);

            // Rekursiv: maxIndex -> end (maxIndex wurde schon hinzugefügt)
            SimplifyRecursive(points, maxIndex, endIndex, epsilon, result);
        }
        else
        {
            // Alle Punkte dazwischen ignorieren, füge nur Endpunkt hinzu
            result.Add(points[endIndex]);
        }
    }

    /// <summary>
    /// Berechne senkrechte Distanz von Punkt zu Linie
    /// </summary>
    private static double PerpendicularDistance(
        LinFloat64Vector3D point,
        LinFloat64Vector3D lineStart,
        LinFloat64Vector3D lineEnd)
    {
        var lineVec = lineEnd - lineStart;
        var lineLength = lineVec.VectorENorm();

        if (lineLength < 1e-10)
            return (point - lineStart).VectorENorm();

        // Projiziere point auf Linie
        var pointVec = point - lineStart;
        var t = pointVec.VectorESp(lineVec) / (lineLength * lineLength);

        // Clamp t auf [0, 1] für Liniensegment
        t = Math.Clamp(t, 0.0, 1.0);

        var projection = lineStart + lineVec * t;
        var distance = (point - projection).VectorENorm();

        return distance;
    }

    /// <summary>
    /// Schätze guten Epsilon-Wert basierend auf Punktdichte
    /// </summary>
    public static double EstimateEpsilon(List<LinFloat64Vector3D> points, double targetReductionFactor = 0.1)
    {
        if (points.Count < 2)
            return 0.01;

        // Berechne durchschnittliche Punkt-zu-Punkt Distanz
        var totalDistance = 0.0;
        for (int i = 1; i < points.Count; i++)
        {
            totalDistance += (points[i] - points[i - 1]).VectorENorm();
        }

        var avgDistance = totalDistance / (points.Count - 1);

        // ANGEPASST: Kleinerer Epsilon-Faktor = weniger aggressive Vereinfachung
        // Das bewahrt mehr Details, besonders für gerade Strecken
        var epsilon = avgDistance * targetReductionFactor * 0.5; // Halbiere den Faktor

        return epsilon;
    }
}
