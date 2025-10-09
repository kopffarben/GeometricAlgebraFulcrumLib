using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;

namespace GeometricAlgebraFulcrumLib.Applications.VRSketch.CurveFitting;

/// <summary>
/// Resampling von Kurven auf gleichmäßige Abstände
/// </summary>
public static class CurveResampler
{
    /// <summary>
    /// Resample curve zu gleichmäßigen Abständen
    /// </summary>
    /// <param name="points">Original Punkte</param>
    /// <param name="targetDistance">Ziel-Abstand zwischen Punkten (0 = auto)</param>
    /// <returns>Resampled Punkte mit gleichmäßigen Abständen</returns>
    public static List<LinFloat64Vector3D> ResampleToUniformDistance(
        List<LinFloat64Vector3D> points,
        double targetDistance = 0.0)
    {
        if (points.Count < 2)
            return points;

        // Auto-detect target distance als Median der Abstände
        if (targetDistance <= 0)
        {
            var distances = new List<double>();
            for (int i = 1; i < points.Count; i++)
            {
                distances.Add((points[i] - points[i - 1]).VectorENorm());
            }
            distances.Sort();
            targetDistance = distances[distances.Count / 2]; // Median
        }

        var result = new List<LinFloat64Vector3D>();
        result.Add(points[0]);

        double accumulatedDistance = 0.0;
        int sourceIndex = 0;

        while (sourceIndex < points.Count - 1)
        {
            var currentPoint = result[^1];
            var nextPoint = points[sourceIndex + 1];
            var segmentVector = nextPoint - currentPoint;
            var segmentLength = segmentVector.VectorENorm();

            if (segmentLength < 1e-10)
            {
                // Skip duplicate points
                sourceIndex++;
                continue;
            }

            var remainingDistance = targetDistance - accumulatedDistance;

            if (segmentLength >= remainingDistance)
            {
                // Interpolate point at target distance
                var t = remainingDistance / segmentLength;
                var newPoint = currentPoint + segmentVector * t;
                result.Add(newPoint);
                accumulatedDistance = 0.0;

                // Don't advance sourceIndex - continue from newPoint
            }
            else
            {
                // Not enough distance in this segment
                accumulatedDistance += segmentLength;
                sourceIndex++;

                // Add last point if we're at the end
                if (sourceIndex == points.Count - 1)
                {
                    result.Add(points[^1]);
                }
            }
        }

        // Ensure last point is included
        if ((result[^1] - points[^1]).VectorENorm() > targetDistance * 0.1)
        {
            result.Add(points[^1]);
        }

        return result;
    }

    /// <summary>
    /// Berechne Kurvenlänge
    /// </summary>
    public static double ComputeCurveLength(List<LinFloat64Vector3D> points)
    {
        if (points.Count < 2)
            return 0.0;

        double length = 0.0;
        for (int i = 1; i < points.Count; i++)
        {
            length += (points[i] - points[i - 1]).VectorENorm();
        }
        return length;
    }

    /// <summary>
    /// Resample auf eine bestimmte Anzahl Punkte
    /// </summary>
    public static List<LinFloat64Vector3D> ResampleToPointCount(
        List<LinFloat64Vector3D> points,
        int targetPointCount)
    {
        if (points.Count < 2 || targetPointCount < 2)
            return points;

        var curveLength = ComputeCurveLength(points);
        var targetDistance = curveLength / (targetPointCount - 1);

        return ResampleToUniformDistance(points, targetDistance);
    }

    /// <summary>
    /// Adaptive Resampling basierend auf Kurvatur
    /// Mehr Punkte bei hoher Kurvatur, weniger bei geraden Abschnitten
    /// </summary>
    public static List<LinFloat64Vector3D> ResampleAdaptive(
        List<LinFloat64Vector3D> points,
        double minDistance = 0.01,
        double maxDistance = 0.1)
    {
        if (points.Count < 3)
            return points;

        var result = new List<LinFloat64Vector3D>();
        result.Add(points[0]);

        double accumulatedDistance = 0.0;

        for (int i = 1; i < points.Count - 1; i++)
        {
            var prev = points[i - 1];
            var curr = points[i];
            var next = points[i + 1];

            // Berechne Kurvatur (Winkel zwischen Segmenten)
            var v1 = curr - prev;
            var v2 = next - curr;

            var len1 = v1.VectorENorm();
            var len2 = v2.VectorENorm();

            if (len1 < 1e-10 || len2 < 1e-10)
                continue;

            var v1Norm = v1 / len1;
            var v2Norm = v2 / len2;

            var cosAngle = Math.Max(-1.0, Math.Min(1.0, v1Norm.VectorESp(v2Norm)));
            var angle = Math.Acos(cosAngle);

            // Kurvatur: 0 = gerade, π = Umkehrung
            var curvature = angle;

            // Adaptive target distance: weniger bei hoher Kurvatur
            var targetDistance = maxDistance - (maxDistance - minDistance) * (curvature / Math.PI);

            var distanceToPrev = (curr - result[^1]).VectorENorm();
            accumulatedDistance += distanceToPrev;

            if (accumulatedDistance >= targetDistance)
            {
                result.Add(curr);
                accumulatedDistance = 0.0;
            }
        }

        // Add last point
        result.Add(points[^1]);

        return result;
    }
}
