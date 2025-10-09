using System.IO;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Rotors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Blades;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Operations;

namespace GeometricAlgebraFulcrumLib.Applications.VRSketch.Prototypes;

/// <summary>
/// Prototyp: Direkter 3D CGA Ansatz für Arc-Spline Konstruktion
/// Basiert auf Controller Position + Orientation (Option B4)
///
/// HINWEIS: Dies ist eine vereinfachte Version, die die grundlegende
/// Circle-Konstruktion demonstriert. Rotor-basierte Parametrisierung
/// wird in zukünftigen Iterationen hinzugefügt.
/// </summary>
public static class ArcSplinePrototype
{
    private static readonly CGaFloat64GeometricSpace5D Cga = CGaFloat64GeometricSpace5D.Instance;

    /// <summary>
    /// Konstruiere Kreis-Arc durch zwei Punkte mit gegebener Ebene
    /// Dies ist die Kern-Methode für Arc-Segment Konstruktion
    /// </summary>
    /// <param name="p1">Startpunkt</param>
    /// <param name="p2">Endpunkt</param>
    /// <param name="planeBivector">Bivector der Kreisebene (definiert Orientation)</param>
    /// <param name="previousCircleCenter">Optional: Zentrum des vorherigen Kreises (für C¹-Kontinuität)</param>
    /// <returns>CGA Blade (Kreis) durch p1, p2 in gegebener Ebene</returns>
    public static CGaFloat64Blade ConstructCircleArc(
        LinFloat64Vector3D p1,
        LinFloat64Vector3D p2,
        LinFloat64Bivector3D planeBivector,
        LinFloat64Vector3D? previousCircleCenter = null)
    {
        // 1. Berechne Normale der Ebene (aus Bivector)
        var normal = BivectorToNormal(planeBivector);

        // 2. Berechne Mittelpunkt der Chord (p1 → p2)
        var chordMidpoint = (p1 + p2) * 0.5;

        // 3. Berechne Chord-Vektor
        var chordVector = p2 - p1;
        var chordLength = chordVector.VectorENorm();

        if (chordLength < 1e-10)
        {
            // Punkte zu nah beieinander - Fallback: kleiner Kreis
            return Cga.Encode.IpnsRound.RealCircle(
                radius: 0.01,
                egaCenter: p1,
                egaNormalVector: normal
            );
        }

        // 4. Berechne Richtung vom Chord-Midpoint zum Kreiszentrum
        // Diese Richtung muss senkrecht zur Chord sein und in der Ebene liegen
        var chordDirection = LinFloat64Vector3D.CreateUnitVector(chordVector.X, chordVector.Y, chordVector.Z);
        var centerDirection = LinFloat64Vector3D.CreateUnitVector(
            chordDirection.VectorCross(normal).X,
            chordDirection.VectorCross(normal).Y,
            chordDirection.VectorCross(normal).Z
        );

        // 5. Bestimme Radius basierend auf vorherigem Kreis (für Kontinuität)
        // Oder verwende Heuristik: Radius = Chord-Länge (ergibt ~120° Arc)
        double radius;
        LinFloat64Vector3D center;

        if (previousCircleCenter is not null)
        {
            // Versuche C¹-Kontinuität zu erreichen
            center = EstimateCenterForContinuity(
                p1, p2, chordMidpoint, centerDirection, previousCircleCenter, out radius);
        }
        else
        {
            // Heuristik: Radius = Chord-Länge (ergibt ca. 120° Bogen)
            radius = chordLength;

            // Berechne Center-Offset: Für Chord c und Radius r:
            // offset = sqrt(r² - (c/2)²)
            var halfChord = chordLength / 2.0;
            var centerOffset = Math.Sqrt(Math.Max(0, radius * radius - halfChord * halfChord));

            center = chordMidpoint + centerDirection * centerOffset;
        }

        // 6. HINWEIS: IpnsRound.RealCircle() mit normalVector funktioniert nicht korrekt
        // Stattdessen verwenden wir OPNS Encoding durch 3 Punkte:
        // - p1, p2 (gegeben)
        // - p_third: ein dritter Punkt auf dem Kreis in der gewünschten Ebene

        // Berechne p_third als Punkt auf dem Kreis, orthogonal zum Chord
        var p_third = center + LinFloat64Vector3D.CreateUnitVector(
            chordDirection.X,
            chordDirection.Y,
            chordDirection.Z
        ) * radius;

        // Encode via OPNS (3 Punkte) und konvertiere zu IPNS
        var opnsCircle = Cga.Encode.OpnsRound.Circle(p1, p2, p_third);
        return opnsCircle.OpnsToIpns();
    }

    /// <summary>
    /// Konstruiere Kreis aus Controller-Pose (Position + Orientation)
    /// Dies ist die Hauptmethode für VR-Sketch: Nutzer zeichnet mit Controller,
    /// und wir erstellen Kreise basierend auf Controller-Orientation
    /// </summary>
    /// <param name="p1">Startpunkt des Arc-Segments</param>
    /// <param name="p2">Endpunkt des Arc-Segments</param>
    /// <param name="controllerNormal">Normalenvektor der Kreisebene (aus Controller-Orientation)</param>
    /// <param name="curvatureScale">Skalierung der Krümmung (1.0 = natürlich, >1.0 = stärker gekrümmt)</param>
    public static CGaFloat64Blade ConstructCircleFromControllerPose(
        LinFloat64Vector3D p1,
        LinFloat64Vector3D p2,
        LinFloat64Vector3D controllerNormal,
        double curvatureScale = 1.0)
    {
        // 1. Berechne Chord (Sehne)
        var chord = p2 - p1;
        var chordLength = chord.VectorENorm();
        var chordMidpoint = (p1 + p2) * 0.5;

        if (chordLength < 1e-6)
        {
            // Punkte zu nah - Fallback: verwende Punkt-Geometrie
            return Cga.Encode.IpnsRound.Point(p1);
        }

        // 2. Normalisiere Controller-Normal
        var normal = LinFloat64Vector3D.CreateUnitVector(
            controllerNormal.X,
            controllerNormal.Y,
            controllerNormal.Z
        );

        // 3. Berechne dritten Punkt für OPNS-Encoding
        // Strategie: Wähle Punkt auf Chord-Midpoint + Offset in Normalenrichtung
        // Dies definiert die "Höhe" des Bogens über der Chord

        // Höhe basierend auf Chord-Länge und Curvature-Scale
        // curvatureScale = 1.0 ergibt moderate Krümmung (~60° Bogen)
        var arcHeight = chordLength * 0.5 * curvatureScale;

        var p3 = chordMidpoint + normal * arcHeight;

        // 4. Konstruiere Kreis via OPNS (3 Punkte)
        var opnsCircle = Cga.Encode.OpnsRound.Circle(p1, p2, p3);
        return opnsCircle.OpnsToIpns();
    }

    /// <summary>
    /// Konstruiere Kreis mit Tangenten-Constraint (für C¹-Kontinuität)
    /// Gegeben: Zwei Punkte p1, p2 und Tangente t1 bei p1
    /// Findet Circle, der durch p1, p2 geht mit Tangent t1 bei p1
    /// </summary>
    /// <param name="p1">Startpunkt</param>
    /// <param name="p2">Endpunkt</param>
    /// <param name="tangent1">Tangente bei p1 (muss normalisiert sein)</param>
    /// <param name="normalHint">Controller-Normal als Hint für Ebenen-Wahl</param>
    /// <returns>Circle (IPNS)</returns>
    public static CGaFloat64Blade ConstructCircleWithTangentConstraint(
        LinFloat64Vector3D p1,
        LinFloat64Vector3D p2,
        LinFloat64Vector3D tangent1,
        LinFloat64Vector3D normalHint,
        double curvatureScale = 1.0)
    {
        var chord = p2 - p1;
        var chordLength = chord.VectorENorm();

        // Degenerate Fall: Punkte zu nah
        if (chordLength < 1e-6)
        {
            return Cga.Encode.IpnsRound.Point(p1);
        }

        // Normalisiere Inputs
        var t1 = LinFloat64Vector3D.CreateUnitVector(tangent1.X, tangent1.Y, tangent1.Z);

        // Prüfe ob tangent1 und chord parallel sind
        var chordNorm = LinFloat64Vector3D.CreateUnitVector(chord.X, chord.Y, chord.Z);
        var dotProduct = Math.Abs(t1.VectorESp(chordNorm));

        if (dotProduct > 0.95) // Fast parallel (gerade Bewegung)
        {
            // SPEZIALFALL: Gerade Bewegung → Arc mit Controller-Normal
            // curvatureScale steuert die Krümmungsstärke:
            // curvatureScale < 1.0 → stärkere Krümmung
            // curvatureScale > 1.0 → schwächere Krümmung

            var parallelness = (dotProduct - 0.95) / 0.05; // 0..1 range
            // Base arc height: 20% statt 5% für sichtbarere Krümmung
            var baseHeightFactor = 0.2 * (1.0 - parallelness * 0.9); // 20% bis 2%
            var arcHeightFactor = baseHeightFactor / curvatureScale; // curvatureScale anwenden

            var normalNorm = LinFloat64Vector3D.CreateUnitVector(normalHint.X, normalHint.Y, normalHint.Z);
            var midpoint = (p1 + p2) * 0.5;
            var arcHeight = chordLength * arcHeightFactor;
            var p3Fallback = midpoint + normalNorm * arcHeight;
            var opnsCircle = Cga.Encode.OpnsRound.Circle(p1, p2, p3Fallback);
            return opnsCircle.OpnsToIpns();
        }

        // Berechne Ebene aufgespannt von tangent1 und chord
        // Normal dieser Ebene:
        var planeNormal = t1.VectorCross(chord);
        var planeNormalNorm = planeNormal.VectorENorm();

        if (planeNormalNorm < 1e-10)
        {
            // Fallback wenn t1 || chord
            var normalNorm2 = LinFloat64Vector3D.CreateUnitVector(normalHint.X, normalHint.Y, normalHint.Z);
            var midpoint2 = (p1 + p2) * 0.5;
            var arcHeight2 = chordLength * 0.5;
            var p3Fallback2 = midpoint2 + normalNorm2 * arcHeight2;
            var opnsCircle2 = Cga.Encode.OpnsRound.Circle(p1, p2, p3Fallback2);
            return opnsCircle2.OpnsToIpns();
        }

        var planeNormalUnit = LinFloat64Vector3D.CreateUnitVector(
            planeNormal.X / planeNormalNorm,
            planeNormal.Y / planeNormalNorm,
            planeNormal.Z / planeNormalNorm
        );

        // Radiale Richtung: senkrecht zu tangent1, in der Ebene {t1, chord}
        // r_dir = plane_normal × t1
        var radialDir = planeNormalUnit.VectorCross(t1);
        var radialDirUnit = LinFloat64Vector3D.CreateUnitVector(radialDir.X, radialDir.Y, radialDir.Z);

        // Löse für λ: center = p1 + λ * radialDir
        // Bedingung: |center - p1| = |center - p2|
        // => λ² = |chord + λ*radialDir|²
        // => λ² = |chord|² + 2λ*(chord·radialDir) + λ²
        // => 0 = |chord|² + 2λ*(chord·radialDir)
        // => λ = -|chord|² / (2*(chord·radialDir))

        var chordDotRadial = chord.VectorESp(radialDirUnit);

        if (Math.Abs(chordDotRadial) < 1e-10)
        {
            // Sonderfall: chord ⊥ radialDir
            // Das sollte nicht passieren, aber Fallback
            var normalNorm3 = LinFloat64Vector3D.CreateUnitVector(normalHint.X, normalHint.Y, normalHint.Z);
            var midpoint3 = (p1 + p2) * 0.5;
            var arcHeight3 = chordLength * 0.5;
            var p3Fallback3 = midpoint3 + normalNorm3 * arcHeight3;
            var opnsCircle3 = Cga.Encode.OpnsRound.Circle(p1, p2, p3Fallback3);
            return opnsCircle3.OpnsToIpns();
        }

        var chordLengthSquared = chordLength * chordLength;
        var lambda = -chordLengthSquared / (2.0 * chordDotRadial);

        // WICHTIG: Prüfe ob der berechnete Radius zu klein ist
        // Für flache Kurven brauchen wir einen großen Radius
        var calculatedRadius = Math.Abs(lambda);

        // Minimaler Radius = 0.5× Chord-Länge (verhindert zu enge Kurven, aber erlaubt C¹-Kontinuität)
        // HINWEIS: Zu großer minRadius zerstört C¹-Kontinuität durch OPNS-Fallback!
        var minRadius = chordLength * 0.5;

        if (calculatedRadius < minRadius)
        {
            // Zu kleiner Radius → Verwende flachen Arc via OPNS
            // Arc-Höhe (Sagitta) berechnen: s = r - sqrt(r² - (c/2)²)
            // Für minRadius und gegebene Chord:
            var halfChord = chordLength / 2.0;
            var sagitta = minRadius - Math.Sqrt(minRadius * minRadius - halfChord * halfChord);

            // p3 = Midpoint + Normale * Sagitta
            var midpoint = (p1 + p2) * 0.5;

            // Finde Normale in der Ebene (t1, chord)
            // Das ist die planeNormal, die wir schon haben
            var sagittaDirection = LinFloat64Vector3D.CreateUnitVector(
                planeNormalUnit.X,
                planeNormalUnit.Y,
                planeNormalUnit.Z
            );

            // Prüfe Richtung: sollte grob zu normalHint passen
            if (sagittaDirection.VectorESp(normalHint) < 0)
            {
                sagittaDirection = sagittaDirection * -1.0;
            }

            var p3Flat = midpoint + sagittaDirection * sagitta;
            var opnsCircleFlat = Cga.Encode.OpnsRound.Circle(p1, p2, p3Flat);

            Console.WriteLine($"Radius zu klein ({calculatedRadius:F3}), verwende flachen OPNS-Arc (minRadius: {minRadius:F3}, sagitta: {sagitta:F3})");

            return opnsCircleFlat.OpnsToIpns();
        }

        // Zwei mögliche Center:
        var center1 = p1 + radialDirUnit * lambda;
        var center2 = p1 - radialDirUnit * lambda;

        // Wähle Center das den KÜRZESTEN Arc produziert
        // Berechne Arc-Winkel für beide Centers
        var v1_center1 = LinFloat64Vector3D.CreateUnitVector((p1 - center1).X, (p1 - center1).Y, (p1 - center1).Z);
        var v2_center1 = LinFloat64Vector3D.CreateUnitVector((p2 - center1).X, (p2 - center1).Y, (p2 - center1).Z);
        var angle1 = Math.Acos(Math.Clamp(v1_center1.VectorESp(v2_center1), -1.0, 1.0));

        var v1_center2 = LinFloat64Vector3D.CreateUnitVector((p1 - center2).X, (p1 - center2).Y, (p1 - center2).Z);
        var v2_center2 = LinFloat64Vector3D.CreateUnitVector((p2 - center2).X, (p2 - center2).Y, (p2 - center2).Z);
        var angle2 = Math.Acos(Math.Clamp(v1_center2.VectorESp(v2_center2), -1.0, 1.0));

        // Zusätzlich: Check ob die Tangente am Ende in richtige Richtung zeigt
        // Tangente at p2 sollte grob in Richtung chord zeigen
        var normal1 = planeNormalUnit;
        var normal2 = planeNormalUnit * -1.0;

        // Tangente at p2 = normal × (p2 - center)
        var tangent1_at_p2 = normal1.VectorCross(v2_center1);
        var tangent2_at_p2 = normal2.VectorCross(v2_center2);

        var chordDirection = LinFloat64Vector3D.CreateUnitVector(chord.X, chord.Y, chord.Z);
        var tangent1_dot = tangent1_at_p2.VectorESp(chordDirection);
        var tangent2_dot = tangent2_at_p2.VectorESp(chordDirection);

        // Wähle den Kreis mit:
        // 1. Kleinerer Arc-Winkel (bevorzugt kurze Arcs)
        // 2. Falls ähnlich: Tangente die besser mit chord aligned ist
        LinFloat64Vector3D chosenCenter;

        if (Math.Abs(angle1 - angle2) < 0.1) // Ähnliche Winkel
        {
            // Wähle basierend auf Tangenten-Alignment
            chosenCenter = tangent1_dot > tangent2_dot ? center1 : center2;
        }
        else
        {
            // Wähle den mit kleinerem Winkel
            chosenCenter = angle1 < angle2 ? center1 : center2;
        }

        // Konstruiere Circle via 3 Punkte (OPNS)
        // p1, p2, und ein dritter Punkt p3 auf dem Circle
        // WICHTIG: p3 muss in Richtung der Tangente t1 liegen, um den kurzen Arc zu garantieren

        var radius = (p1 - chosenCenter).VectorENorm();
        var v1 = p1 - chosenCenter;
        var v2 = p2 - chosenCenter;

        // Berechne Winkel zwischen v1 und v2
        var v1_unit = LinFloat64Vector3D.CreateUnitVector(v1.X, v1.Y, v1.Z);
        var v2_unit = LinFloat64Vector3D.CreateUnitVector(v2.X, v2.Y, v2.Z);
        var totalAngle = Math.Acos(Math.Clamp(v1_unit.VectorESp(v2_unit), -1.0, 1.0));

        // Wähle p3 als Punkt auf dem Kreis bei 1/3 des Winkels von p1 zu p2
        // Rotiere v1 um die Normal-Achse um 1/3 des Winkels
        var rotationAngle = totalAngle / 3.0;

        // Bestimme Rotationsrichtung: Soll in Richtung der Tangente sein
        // Tangente = normal × (p1 - center)
        var tangentAtP1 = LinFloat64Vector3D.CreateUnitVector(
            planeNormalUnit.VectorCross(v1_unit).X,
            planeNormalUnit.VectorCross(v1_unit).Y,
            planeNormalUnit.VectorCross(v1_unit).Z
        );

        // Check: Zeigt v2 in grob gleiche Richtung wie tangentAtP1?
        // (Das sagt uns die Rotationsrichtung)
        var crossProduct = v1_unit.VectorCross(v2_unit);
        var directionCheck = crossProduct.VectorESp(planeNormalUnit);

        // Wenn negativ, rotiere in andere Richtung
        if (directionCheck < 0)
        {
            rotationAngle = -rotationAngle;
        }

        // Rotiere v1 um rotationAngle um planeNormalUnit-Achse
        var cos = Math.Cos(rotationAngle);
        var sin = Math.Sin(rotationAngle);

        // Rodrigues rotation formula
        var k = planeNormalUnit;
        var v = v1_unit;
        var vRot = v * cos + k.VectorCross(v) * sin + k * (k.VectorESp(v)) * (1 - cos);

        var p3 = chosenCenter + vRot * radius;

        var circle = Cga.Encode.OpnsRound.Circle(p1, p2, p3);
        return circle.OpnsToIpns();
    }

    /// <summary>
    /// Schätze Kreiszentrum für C¹-Kontinuität mit vorherigem Segment
    /// </summary>
    private static LinFloat64Vector3D EstimateCenterForContinuity(
        LinFloat64Vector3D p1,
        LinFloat64Vector3D p2,
        LinFloat64Vector3D chordMidpoint,
        LinFloat64Vector3D centerDirection,
        LinFloat64Vector3D previousCenter,
        out double radius)
    {
        var chordLength = (p2 - p1).VectorENorm();

        // Berechne Radius basierend auf vorherigem Zentrum
        // Idee: Projiziere vorheriges Zentrum auf Center-Richtung
        var toPreviousCenter = previousCenter - chordMidpoint;
        var projectedDistance = toPreviousCenter.VectorESp(centerDirection);

        // Clamp zu sinnvollen Werten (Radius sollte mindestens halbe Chord sein)
        var minRadius = chordLength / 2.0;
        var maxRadius = chordLength * 3.0; // Arbitrary limit

        var estimatedOffset = Math.Clamp(Math.Abs(projectedDistance), minRadius * 0.5, maxRadius);

        // Berechne Radius aus Offset
        var halfChord = chordLength / 2.0;
        radius = Math.Sqrt(estimatedOffset * estimatedOffset + halfChord * halfChord);

        // Zentrum in Richtung des vorherigen Zentrums (für Smooth Continuation)
        var sign = projectedDistance >= 0 ? 1.0 : -1.0;
        return chordMidpoint + centerDirection * (sign * estimatedOffset);
    }

    /// <summary>
    /// Berechne Kreisebene aus Controller-Orientation und Bewegungsrichtung (Option B4)
    /// </summary>
    /// <param name="position">Aktuelle Position</param>
    /// <param name="orientation">Controller Quaternion</param>
    /// <param name="tangent">Bewegungsrichtung (normalisiert)</param>
    /// <returns>Bivector der Kreisebene</returns>
    public static LinFloat64Bivector3D CalculateCirclePlane(
        LinFloat64Vector3D position,
        LinFloat64Quaternion orientation,
        LinFloat64Vector3D tangent)
    {
        // 1. Extrahiere "Forward" Vektor aus Quaternion
        // Der Controller "zeigt" in eine Richtung
        var forward = orientation.RotateVector(LinFloat64Vector3D.E3); // Z-axis als Default

        // 2. Projiziere Forward auf Ebene senkrecht zu Tangente
        // Dies gibt die "Twist" um die Bewegungsrichtung
        var normalToTangent = forward - tangent * forward.VectorESp(tangent);

        // 3. Falls Forward parallel zu Tangent ist, verwende Up-Vektor
        if (normalToTangent.VectorENorm() < 1e-6)
        {
            var up = orientation.RotateVector(LinFloat64Vector3D.E2);
            normalToTangent = up - tangent * up.VectorESp(tangent);
        }

        normalToTangent = LinFloat64Vector3D.CreateUnitVector(
            normalToTangent.X,
            normalToTangent.Y,
            normalToTangent.Z
        );

        // 4. Kreisebene = Tangent ∧ Normal
        return tangent.Op(normalToTangent);
    }

    /// <summary>
    /// Hilfsfunktion: Konvertiere Bivector zu Normal-Vektor
    /// </summary>
    private static LinFloat64Vector3D BivectorToNormal(LinFloat64Bivector3D bivector)
    {
        // In 3D: Bivector entspricht Dual des Normal-Vektors
        // Bivector e₁∧e₂ entspricht Normal e₃, etc.
        // Hodge Dual: ⋆(e₁∧e₂) = e₃

        // Extrahiere Komponenten: B = xy*e₁∧e₂ + xz*e₁∧e₃ + yz*e₂∧e₃
        var xy = bivector.Xy;
        var xz = bivector.Xz;
        var yz = bivector.Yz;

        // Dual: normal = (yz, -xz, xy)
        var normal = LinFloat64Vector3D.Create(yz, -xz, xy);

        // Normalisiere
        var norm = normal.VectorENorm();
        if (norm < 1e-10) return LinFloat64Vector3D.E3; // Fallback

        return LinFloat64Vector3D.CreateUnitVector(normal.X, normal.Y, normal.Z);
    }

    /// <summary>
    /// Berechne Arc-Länge zwischen zwei Punkten auf einem Kreis
    /// </summary>
    public static double CalculateArcLength(
        CGaFloat64Blade circle,
        LinFloat64Vector3D p1,
        LinFloat64Vector3D p2)
    {
        var decoded = circle.DecodeIpnsRound.Element();
        var center = decoded.CenterToVector3D();
        var radius = decoded.RealRadius;

        var v1 = LinFloat64Vector3D.CreateUnitVector(
            (p1 - center).X,
            (p1 - center).Y,
            (p1 - center).Z
        );
        var v2 = LinFloat64Vector3D.CreateUnitVector(
            (p2 - center).X,
            (p2 - center).Y,
            (p2 - center).Z
        );

        var angle = Math.Acos(Math.Clamp(v1.VectorESp(v2), -1.0, 1.0));

        return radius * angle;
    }

    /// <summary>
    /// Prüfe C¹-Kontinuität zwischen zwei Kreisen an Verbindungspunkt
    /// </summary>
    public static bool CheckC1Continuity(
        CGaFloat64Blade circle1,
        CGaFloat64Blade circle2,
        LinFloat64Vector3D connectionPoint,
        double tolerance = 1e-6)
    {
        // Berechne Tangenten an Verbindungspunkt
        var tangent1 = CalculateTangentAtPoint(circle1, connectionPoint);
        var tangent2 = CalculateTangentAtPoint(circle2, connectionPoint);

        // Prüfe ob parallel (gleiche Richtung)
        var dotProduct = Math.Abs(tangent1.VectorESp(tangent2));
        return Math.Abs(dotProduct - 1.0) < tolerance;
    }

    /// <summary>
    /// Berechne Tangentenvektor auf Kreis an gegebenem Punkt
    /// </summary>
    private static LinFloat64Vector3D CalculateTangentAtPoint(
        CGaFloat64Blade circle,
        LinFloat64Vector3D point)
    {
        var decoded = circle.DecodeIpnsRound.Element();
        var center = decoded.CenterToVector3D();
        var normal = decoded.NormalDirectionToVector3D();

        // Radialvektor vom Zentrum zum Punkt
        var radial = LinFloat64Vector3D.CreateUnitVector(
            (point - center).X,
            (point - center).Y,
            (point - center).Z
        );

        // Tangente = Normal × Radial
        var tangent = normal.VectorCross(radial);
        return LinFloat64Vector3D.CreateUnitVector(tangent.X, tangent.Y, tangent.Z);
    }

    /// <summary>
    /// Extrahiere Rotor für Arc-Rotation von p1 nach p2 um Center
    /// Verwendet Euclidean Rotor (VGa) statt CGA Versor
    /// </summary>
    public static XGaFloat64Rotor ExtractRotorFromArc(
        CGaFloat64Blade circle,
        LinFloat64Vector3D p1,
        LinFloat64Vector3D p2)
    {
        var decoded = circle.DecodeIpnsRound.Element();
        var center = decoded.CenterToVector3D();

        // Vektoren vom Center zu den Punkten (in VGa)
        var v1 = p1 - center;
        var v2 = p2 - center;

        var processor = Cga.EuclideanProcessor;

        // Prüfe ob v1 oder v2 zu kurz sind (Punkt zu nah am Center)
        var v1Norm = v1.VectorENorm();
        var v2Norm = v2.VectorENorm();

        if (v1Norm < 1e-10 || v2Norm < 1e-10)
        {
            // Punkte sind zu nah am Center - Identity Rotor
            return XGaFloat64Rotor.CreateIdentity(processor);
        }

        // Normalisiere Vektoren
        var v1Unit = LinFloat64Vector3D.CreateUnitVector(v1.X, v1.Y, v1.Z);
        var v2Unit = LinFloat64Vector3D.CreateUnitVector(v2.X, v2.Y, v2.Z);

        // Prüfe ob Vektoren parallel sind (dot product ≈ ±1)
        var dotProduct = v1Unit.VectorESp(v2Unit);

        if (Math.Abs(dotProduct - 1.0) < 1e-10)
        {
            // Vektoren sind gleich - Identity Rotor
            return XGaFloat64Rotor.CreateIdentity(processor);
        }

        if (Math.Abs(dotProduct + 1.0) < 1e-10)
        {
            // Vektoren sind entgegengesetzt - 180° Rotation
            // Finde einen Vektor senkrecht zu v1 in der Circle-Ebene
            var normal = decoded.NormalDirectionToVector3D();

            // perp = normal × v1 (Vektor in Circle-Ebene, senkrecht zu v1)
            var perp = normal.VectorCross(v1Unit);
            perp = LinFloat64Vector3D.CreateUnitVector(perp.X, perp.Y, perp.Z);

            // Erstelle Rotor der v1 180° um die Achse parallel zu normal rotiert
            // Das entspricht Rotation von v1 zu -v1 in der Ebene aufgespannt von v1 und perp
            var xgaV1Unit = processor.Vector(v1Unit.X, v1Unit.Y, v1Unit.Z);
            var xgaPerp = processor.Vector(perp.X, perp.Y, perp.Z);

            // Bivector = v1 ∧ perp (normalisiert)
            var bivector = xgaV1Unit.Op(xgaPerp).GetBivectorPart();
            var bivectorNorm = bivector.Norm().ScalarValue;

            if (bivectorNorm < 1e-10)
            {
                // Bivector ist zu klein - Identity Rotor
                return XGaFloat64Rotor.CreateIdentity(processor);
            }

            var unitBivector = bivector / bivectorNorm;

            return XGaFloat64Rotor.CreateEuclideanPureRotor(
                LinFloat64PolarAngle.CreateFromDegrees(180),
                unitBivector
            );
        }

        // Encode als XGa Vectors (Euclidean GA)
        var xgaV1 = processor.Vector(v1.X, v1.Y, v1.Z);
        var xgaV2 = processor.Vector(v2.X, v2.Y, v2.Z);

        // Erstelle Euclidean Rotor der v1 nach v2 rotiert
        return XGaFloat64Rotor.CreateEuclideanPureRotor(xgaV1, xgaV2);
    }

    /// <summary>
    /// Sample Arc-Punkt via Rotor Power
    /// Arc(t) = R^t ⊲ p1 für t ∈ [0,1]
    /// Verwendet Rotor Power via GP (Geometric Product)
    /// WICHTIG: Garantiert exakte Start/End-Punkte bei t=0 und t=1
    /// </summary>
    public static LinFloat64Vector3D SampleArcWithRotor(
        XGaFloat64Rotor rotor,
        LinFloat64Vector3D center,
        LinFloat64Vector3D p1,
        LinFloat64Vector3D p2,
        double t)
    {
        // EXAKTE Endpunkte garantieren: Bei t=0 oder t=1 DIREKT die Punkte zurückgeben
        // Dies verhindert numerische Fehler durch Rotor-Interpolation
        if (Math.Abs(t) < 1e-10)
        {
            return p1;
        }
        if (Math.Abs(t - 1.0) < 1e-10)
        {
            return p2;
        }

        // Vektor vom Center zum Startpunkt
        var v1 = p1 - center;
        var processor = rotor.Processor;
        var xgaV1 = processor.Vector(v1.X, v1.Y, v1.Z);

        // Rotor Power: R^t für t ∈ (0, 1)
        // Interpolation: Extrahiere Angle und Bivector
        // R = e^(B*angle/2) = cos(angle/2) + sin(angle/2)*B_unit
        var rotorMv = rotor.Multivector;
        var scalarPart = rotorMv.Scalar();
        var bivectorPart = rotorMv.GetBivectorPart();

        // Berechne Winkel: cos(angle/2) = scalar part
        var halfAngle = Math.Acos(Math.Clamp(scalarPart, -1.0, 1.0));

        // Normiere Bivector: sin(angle/2)*B_unit = bivectorPart
        // => B_unit = bivectorPart / sin(angle/2)
        var sinHalfAngle = Math.Sin(halfAngle);

        XGaFloat64Rotor rotorT;
        if (Math.Abs(sinHalfAngle) < 1e-10)
        {
            // Fast identity rotor - linearer Interpolation
            var interpolated = p1 + (p2 - p1) * t;
            return interpolated;
        }
        else
        {
            var unitBivector = bivectorPart / sinHalfAngle;

            // R^t = e^(B_unit * t*angle/2)
            var tHalfAngle = t * halfAngle;
            rotorT = XGaFloat64Rotor.CreateEuclideanPureRotor(
                LinFloat64PolarAngle.CreateFromRadians(2.0 * tHalfAngle),
                unitBivector
            );
        }

        // Wende Rotor an: vRotated = R^t * v1 * (R^t)^{-1}
        var vRotated = rotorT.OmMap(xgaV1);

        // Zurück zu 3D + Center addieren
        return center + LinFloat64Vector3D.Create(
            vRotated[0],
            vRotated[1],
            vRotated[2]
        );
    }

    /// <summary>
    /// Einfacher Test-Case: Konstruiere 3-Punkt Arc-Spline
    /// </summary>
    public static void TestThreePointArcSpline()
    {
        Console.WriteLine("=== Three-Point Arc-Spline Test (Simplified) ===\n");

        // Vereinfachter Test: Verwende OPNS Encoding mit 3 Punkten
        // Dies ist geometrisch eindeutig und vermeidet Controller-Orientation
        var p1 = LinFloat64Vector3D.Create(1, 0, 0);
        var p2 = LinFloat64Vector3D.Create(0, 1, 0);
        var p3 = LinFloat64Vector3D.Create(-1, 0, 0);

        // Verwende OPNS-Encoding (durch 3 Punkte definiert)
        // Circle 1 durch p1, p2, und einen dritten Punkt zwischen ihnen leicht verschoben
        var p1_mid = (p1 + p2) * 0.5 + LinFloat64Vector3D.E3 * 0.5; // Leicht aus XY-Ebene

        Console.WriteLine("Circle 1: durch p1, p2, p1_mid");
        var opnsCircle1 = Cga.Encode.OpnsRound.Circle(p1, p2, p1_mid);
        var ipnsCircle1 = opnsCircle1.OpnsToIpns();
        var circle1 = ipnsCircle1.DecodeIpnsRound.Element();

        Console.WriteLine($"  Center: {circle1.CenterToVector3D()}");
        Console.WriteLine($"  Radius: {circle1.RealRadius:F4}\n");

        // Circle 2 durch p2, p3, und einen dritten Punkt
        var p2_mid = (p2 + p3) * 0.5 + LinFloat64Vector3D.E3 * 0.5;

        Console.WriteLine("Circle 2: durch p2, p3, p2_mid");
        var opnsCircle2 = Cga.Encode.OpnsRound.Circle(p2, p3, p2_mid);
        var ipnsCircle2 = opnsCircle2.OpnsToIpns();
        var circle2 = ipnsCircle2.DecodeIpnsRound.Element();

        Console.WriteLine($"  Center: {circle2.CenterToVector3D()}");
        Console.WriteLine($"  Radius: {circle2.RealRadius:F4}\n");

        // Arc-Längen
        var arcLength1 = CalculateArcLength(ipnsCircle1, p1, p2);
        var arcLength2 = CalculateArcLength(ipnsCircle2, p2, p3);
        Console.WriteLine($"Arc Length 1: {arcLength1:F4}");
        Console.WriteLine($"Arc Length 2: {arcLength2:F4}\n");

        // === Rotor-basierte Arc-Parametrisierung ===
        Console.WriteLine("=== Rotor-based Arc Sampling ===\n");

        // Debug: Zeige Circle 1 Details
        Console.WriteLine($"Debug - p1: {p1}");
        Console.WriteLine($"Debug - p2: {p2}");
        Console.WriteLine($"Debug - Circle 1 Center: {circle1.CenterToVector3D()}");
        Console.WriteLine($"Debug - v1 (p1-center): {p1 - circle1.CenterToVector3D()}");
        Console.WriteLine($"Debug - v2 (p2-center): {p2 - circle1.CenterToVector3D()}\n");

        // Extrahiere Rotor für Circle 1
        var rotor1 = ExtractRotorFromArc(ipnsCircle1, p1, p2);
        Console.WriteLine($"Debug - Rotor1: {rotor1.Multivector}");
        Console.WriteLine($"Debug - Rotor1 Scalar: {rotor1.Multivector.Scalar()}");
        Console.WriteLine($"Debug - Rotor1 Bivector: {rotor1.Multivector.GetBivectorPart()}\n");

        Console.WriteLine("Circle 1 - Rotor-Sampling:");

        // Sample 5 Punkte auf dem Arc
        for (int i = 0; i <= 4; i++)
        {
            var t = i / 4.0;
            var samplePoint = SampleArcWithRotor(rotor1, circle1.CenterToVector3D(), p1, p2, t);
            Console.WriteLine($"  t={t:F2}: ({samplePoint.X:F3}, {samplePoint.Y:F3}, {samplePoint.Z:F3})");
        }

        // Verifiziere: Sollte bei t=1 genau p2 sein
        var finalPoint = SampleArcWithRotor(rotor1, circle1.CenterToVector3D(), p1, p2, 1.0);
        var error = (finalPoint - p2).VectorENorm();
        Console.WriteLine($"\nError at t=1 (should be p2): {error:E3}");

        Console.WriteLine("\n=== Test Complete ===");
    }

    /// <summary>
    /// Repräsentiert ein Arc-Segment in der Spline
    /// </summary>
    public class ArcSegment
    {
        public CGaFloat64Blade Circle { get; set; }
        public LinFloat64Vector3D StartPoint { get; set; }
        public LinFloat64Vector3D EndPoint { get; set; }
        public XGaFloat64Rotor Rotor { get; set; }
        public double ArcLength { get; set; }

        public ArcSegment(CGaFloat64Blade circle, LinFloat64Vector3D startPoint, LinFloat64Vector3D endPoint)
        {
            Circle = circle;
            StartPoint = startPoint;
            EndPoint = endPoint;
            Rotor = ExtractRotorFromArc(circle, startPoint, endPoint);
            ArcLength = CalculateArcLength(circle, startPoint, endPoint);

            // Debug: Verify rotor produces correct endpoint
            var decoded = circle.DecodeIpnsRound.Element();
            var center = decoded.CenterToVector3D();
            var sampledEnd = SampleArcWithRotor(Rotor, center, startPoint, endPoint, 1.0);
            var endError = (sampledEnd - endPoint).VectorENorm();
            if (endError > 0.01)
            {
                Console.WriteLine($"⚠️ ArcSegment: End point mismatch! Error={endError:F4}, Start={startPoint}, Expected End={endPoint}, Sampled End={sampledEnd}");
            }
        }
    }

    /// <summary>
    /// Multi-Segment Arc-Spline mit automatischer C¹-Kontinuität
    /// </summary>
    public class ArcSpline
    {
        public List<ArcSegment> Segments { get; } = new List<ArcSegment>();
        public double TotalLength { get; private set; }

        /// <summary>
        /// Exportiere Spline als JSON für Babylon.js Visualisierung
        /// </summary>
        public string ExportToJson(int samplesPerSegment = 20)
        {
            // WICHTIG: Verwende invariante Kultur für Punkt-Dezimaltrennzeichen
            var culture = System.Globalization.CultureInfo.InvariantCulture;

            var json = new System.Text.StringBuilder();
            json.AppendLine("{");
            json.AppendLine("  \"segments\": [");

            // Export Segments
            for (int i = 0; i < Segments.Count; i++)
            {
                var seg = Segments[i];
                var decoded = seg.Circle.DecodeIpnsRound.Element();
                var center = decoded.CenterToVector3D();

                json.AppendLine("    {");
                json.AppendLine($"      \"index\": {i},");
                json.AppendLine($"      \"startPoint\": [{seg.StartPoint.X.ToString("F6", culture)}, {seg.StartPoint.Y.ToString("F6", culture)}, {seg.StartPoint.Z.ToString("F6", culture)}],");
                json.AppendLine($"      \"endPoint\": [{seg.EndPoint.X.ToString("F6", culture)}, {seg.EndPoint.Y.ToString("F6", culture)}, {seg.EndPoint.Z.ToString("F6", culture)}],");
                json.AppendLine($"      \"center\": [{center.X.ToString("F6", culture)}, {center.Y.ToString("F6", culture)}, {center.Z.ToString("F6", culture)}],");
                json.AppendLine($"      \"radius\": {decoded.RealRadius.ToString("F6", culture)},");
                json.AppendLine($"      \"arcLength\": {seg.ArcLength.ToString("F6", culture)},");

                // Sample points along arc
                json.AppendLine("      \"samples\": [");
                for (int j = 0; j <= samplesPerSegment; j++)
                {
                    var t = j / (double)samplesPerSegment;
                    var point = SampleArcWithRotor(seg.Rotor, center, seg.StartPoint, seg.EndPoint, t);
                    json.Append($"        [{point.X.ToString("F6", culture)}, {point.Y.ToString("F6", culture)}, {point.Z.ToString("F6", culture)}]");
                    if (j < samplesPerSegment) json.Append(",");
                    json.AppendLine();
                }
                json.Append("      ]");
                json.AppendLine();
                json.Append("    }");
                if (i < Segments.Count - 1) json.Append(",");
                json.AppendLine();
            }

            json.AppendLine("  ],");

            // Export Connection Points with Tangents
            json.AppendLine("  \"connections\": [");
            var tangents = GetConnectionPointTangents();
            for (int i = 0; i < tangents.Count; i++)
            {
                var (point, t1, t2, angle) = tangents[i];
                json.AppendLine("    {");
                json.AppendLine($"      \"point\": [{point.X.ToString("F6", culture)}, {point.Y.ToString("F6", culture)}, {point.Z.ToString("F6", culture)}],");
                json.AppendLine($"      \"tangent1\": [{t1.X.ToString("F6", culture)}, {t1.Y.ToString("F6", culture)}, {t1.Z.ToString("F6", culture)}],");
                json.AppendLine($"      \"tangent2\": [{t2.X.ToString("F6", culture)}, {t2.Y.ToString("F6", culture)}, {t2.Z.ToString("F6", culture)}],");
                json.AppendLine($"      \"angleDegrees\": {(angle * 180.0 / Math.PI).ToString("F6", culture)}");
                json.Append("    }");
                if (i < tangents.Count - 1) json.Append(",");
                json.AppendLine();
            }
            json.AppendLine("  ],");

            json.AppendLine($"  \"totalLength\": {TotalLength.ToString("F6", culture)},");
            json.AppendLine($"  \"segmentCount\": {Segments.Count}");
            json.AppendLine("}");

            return json.ToString();
        }

        /// <summary>
        /// Speichere JSON-Export in Datei
        /// </summary>
        public void ExportToJsonFile(string filePath, int samplesPerSegment = 20)
        {
            var json = ExportToJson(samplesPerSegment);
            System.IO.File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Berechne Tangente am Endpunkt eines Segments (numerische Ableitung)
        /// </summary>
        private static LinFloat64Vector3D ComputeTangentAtEndpoint(ArcSegment segment)
        {
            // Extrahiere Center aus dem Circle-Blade
            var decoded = segment.Circle.DecodeIpnsRound.Element();
            var center = decoded.CenterToVector3D();

            var epsilon = 0.001;
            var pointBefore = SampleArcWithRotor(
                segment.Rotor,
                center,
                segment.StartPoint,
                segment.EndPoint,
                1.0 - epsilon
            );
            var tangent = segment.EndPoint - pointBefore;
            return LinFloat64Vector3D.CreateUnitVector(tangent.X, tangent.Y, tangent.Z);
        }

        /// <summary>
        /// Füge neues Segment hinzu mit Controller-Pose
        /// WICHTIG: Nutzt END-Tangent des vorherigen Segments für C¹-Kontinuität!
        /// </summary>
        public void AddSegmentFromController(
            LinFloat64Vector3D nextPoint,
            LinFloat64Vector3D controllerNormal,
            double curvatureScale = 1.0)
        {
            if (Segments.Count == 0)
            {
                // Erstes Segment kann nicht hinzugefügt werden ohne Startpunkt
                throw new InvalidOperationException("Cannot add first segment without start point. Use AddFirstSegment().");
            }

            var lastSegment = Segments[^1];
            var startPoint = lastSegment.EndPoint;

            // C¹-CONTINUITY: Nutze END-Tangente des vorherigen Segments
            var tangentStart = ComputeTangentAtEndpoint(lastSegment);

            // WICHTIG: Prüfe ob Tangente in grob richtige Richtung zeigt
            var chord = nextPoint - startPoint;
            var chordLength = chord.VectorENorm();

            if (chordLength > 1e-6)
            {
                var chordNorm = LinFloat64Vector3D.CreateUnitVector(chord.X, chord.Y, chord.Z);
                var dotProduct = tangentStart.VectorESp(chordNorm);

                // Wenn dot product negativ, zeigt Tangente rückwärts
                if (dotProduct < 0)
                {
                    tangentStart = LinFloat64Vector3D.Create(
                        -tangentStart.X,
                        -tangentStart.Y,
                        -tangentStart.Z
                    );
                }
            }

            // Nutze Tangent-Constrained Construction für garantierte C¹-Kontinuität
            var circle = ConstructCircleWithTangentConstraint(
                startPoint,
                nextPoint,
                tangentStart,
                controllerNormal,
                curvatureScale
            );

            var segment = new ArcSegment(circle, startPoint, nextPoint);

            Segments.Add(segment);
            TotalLength += segment.ArcLength;
        }

        /// <summary>
        /// Füge Arc direkt aus CircleFit hinzu
        /// NEUE STRATEGIE: Ignoriere CircleFit-Radius, verwende nur Normal als Hint
        /// Konstruiere Arc wie bei AddSegmentFromController mit Tangent-Constraint
        /// </summary>
        public void AddArcFromCircleFit(
            LinFloat64Vector3D startPoint,
            LinFloat64Vector3D endPoint,
            LinFloat64Vector3D center,
            LinFloat64Vector3D normal,
            double radius)
        {
            // WICHTIG: CircleFit gibt uns Center/Normal/Radius für ALLE Punkte im Segment
            // Aber wir brauchen einen Arc der EXAKT durch start/end geht!

            // STRATEGIE: Verwende die selbe Methode wie AddSegmentFromController:
            // - Berechne Tangente am Startpunkt (= Bewegungsrichtung)
            // - Konstruiere Circle mit Tangent-Constraint
            // - Dies garantiert C¹-Kontinuität UND exakte Endpunkte!

            var chord = endPoint - startPoint;
            var chordLength = chord.VectorENorm();

            if (chordLength < 1e-6)
            {
                // Degenerierter Fall: Punkte zu nah
                var pointCircle = Cga.Encode.IpnsRound.Point(startPoint);
                var degenerateSegment = new ArcSegment(pointCircle, startPoint, endPoint);
                Segments.Add(degenerateSegment);
                TotalLength += degenerateSegment.ArcLength;
                return;
            }

            // Berechne Tangente: Beim ersten Segment = Chord-Richtung
            // Bei späteren Segments würden wir die END-Tangente des vorherigen nehmen
            LinFloat64Vector3D tangentStart;

            if (Segments.Count == 0)
            {
                // Erstes Segment: Tangente = Chord-Richtung
                tangentStart = LinFloat64Vector3D.CreateUnitVector(chord.X, chord.Y, chord.Z);
            }
            else
            {
                // Verwende END-Tangente des vorherigen Segments für C¹-Kontinuität
                var lastSegment = Segments[^1];
                tangentStart = ComputeTangentAtEndpoint(lastSegment);

                // Prüfe Richtung
                var chordNorm = LinFloat64Vector3D.CreateUnitVector(chord.X, chord.Y, chord.Z);
                if (tangentStart.VectorESp(chordNorm) < 0)
                {
                    tangentStart = LinFloat64Vector3D.Create(-tangentStart.X, -tangentStart.Y, -tangentStart.Z);
                }
            }

            // Konstruiere Circle mit Tangent-Constraint
            // (selbe Methode wie in AddSegmentFromController)
            var circle = ConstructCircleWithTangentConstraint(
                startPoint,
                endPoint,
                tangentStart,
                normal,  // Normal vom CircleFit als Hint
                curvatureScale: 1.0
            );

            var segment = new ArcSegment(circle, startPoint, endPoint);
            Segments.Add(segment);
            TotalLength += segment.ArcLength;
        }

        /// <summary>
        /// Projiziere Punkt auf Circle
        /// </summary>
        private static LinFloat64Vector3D ProjectPointOntoCircle(
            LinFloat64Vector3D point,
            LinFloat64Vector3D center,
            LinFloat64Vector3D normal,
            double radius)
        {
            // Vektor vom Center zum Punkt
            var toPoint = point - center;

            // Projiziere auf Circle-Ebene (entferne Normal-Komponente)
            var normalUnit = LinFloat64Vector3D.CreateUnitVector(normal.X, normal.Y, normal.Z);
            var normalComponent = toPoint.VectorESp(normalUnit) * normalUnit;
            var inPlane = toPoint - normalComponent;

            var inPlaneLength = inPlane.VectorENorm();
            if (inPlaneLength < 1e-10)
            {
                // Punkt ist auf der Circle-Achse - wähle beliebigen Punkt auf Circle
                // Finde Vektor orthogonal zu normal
                var arbitrary = Math.Abs(normalUnit.Z) < 0.9
                    ? LinFloat64Vector3D.E3
                    : LinFloat64Vector3D.E1;
                var radialDir = normalUnit.VectorCross(arbitrary);
                radialDir = LinFloat64Vector3D.CreateUnitVector(radialDir.X, radialDir.Y, radialDir.Z);
                return center + radialDir * radius;
            }

            // Skaliere auf Radius
            var radialDir2 = LinFloat64Vector3D.CreateUnitVector(inPlane.X, inPlane.Y, inPlane.Z);
            return center + radialDir2 * radius;
        }

        /// <summary>
        /// Füge erstes Segment hinzu
        /// </summary>
        public void AddFirstSegment(
            LinFloat64Vector3D startPoint,
            LinFloat64Vector3D endPoint,
            LinFloat64Vector3D controllerNormal,
            double curvatureScale = 1.0)
        {
            // WICHTIG: Auch beim ersten Segment Motion-Based Tangent nutzen!
            // Tangente = Bewegungsrichtung (Chord-Richtung)
            var chord = endPoint - startPoint;
            var chordLength = chord.VectorENorm();

            CGaFloat64Blade circle;

            if (chordLength > 1e-6)
            {
                var tangentStart = LinFloat64Vector3D.CreateUnitVector(
                    chord.X,
                    chord.Y,
                    chord.Z
                );

                // Nutze Tangent-Constrained Construction
                circle = ConstructCircleWithTangentConstraint(
                    startPoint,
                    endPoint,
                    tangentStart,
                    controllerNormal,
                    curvatureScale
                );
            }
            else
            {
                // Fallback für degenerierte Fälle
                circle = ConstructCircleFromControllerPose(startPoint, endPoint, controllerNormal, curvatureScale);
            }

            var segment = new ArcSegment(circle, startPoint, endPoint);

            Segments.Add(segment);
            TotalLength = segment.ArcLength;
        }

        /// <summary>
        /// Sample Punkt auf der gesamten Spline (t ∈ [0, 1])
        /// </summary>
        public LinFloat64Vector3D Sample(double t)
        {
            if (Segments.Count == 0)
                throw new InvalidOperationException("No segments in spline");

            t = Math.Clamp(t, 0.0, 1.0);

            // Finde entsprechendes Segment
            var targetLength = t * TotalLength;
            var accumulatedLength = 0.0;

            for (int i = 0; i < Segments.Count; i++)
            {
                var segment = Segments[i];
                var nextLength = accumulatedLength + segment.ArcLength;

                if (targetLength <= nextLength || i == Segments.Count - 1)
                {
                    // Sample in diesem Segment
                    var segmentT = segment.ArcLength > 1e-10
                        ? (targetLength - accumulatedLength) / segment.ArcLength
                        : 0.0;

                    var decoded = segment.Circle.DecodeIpnsRound.Element();
                    return SampleArcWithRotor(segment.Rotor, decoded.CenterToVector3D(), segment.StartPoint, segment.EndPoint, segmentT);
                }

                accumulatedLength = nextLength;
            }

            // Fallback: letzter Punkt
            return Segments[^1].EndPoint;
        }

        /// <summary>
        /// Prüfe C¹-Kontinuität an allen Verbindungspunkten
        /// </summary>
        public bool CheckC1Continuity(double tolerance = 1e-6)
        {
            var tangents = GetConnectionPointTangents();

            foreach (var (point, t1, t2, angle) in tangents)
            {
                var angleDegrees = angle * 180.0 / Math.PI;
                if (angleDegrees > tolerance)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gib Tangenten-Richtungen an allen Verbindungspunkten zurück
        /// </summary>
        public List<(LinFloat64Vector3D point, LinFloat64Vector3D tangent1, LinFloat64Vector3D tangent2, double angle)> GetConnectionPointTangents()
        {
            var result = new List<(LinFloat64Vector3D, LinFloat64Vector3D, LinFloat64Vector3D, double)>();

            for (int i = 0; i < Segments.Count - 1; i++)
            {
                var segment1 = Segments[i];
                var segment2 = Segments[i + 1];
                var connectionPoint = segment1.EndPoint;

                var decoded1 = segment1.Circle.DecodeIpnsRound.Element();
                var center1 = decoded1.CenterToVector3D();

                var decoded2 = segment2.Circle.DecodeIpnsRound.Element();
                var center2 = decoded2.CenterToVector3D();

                // KORREKTUR: Berechne Tangenten durch numerische Ableitung
                // Dies gibt die tatsächliche Bewegungsrichtung entlang des Arcs

                // Tangent1: Am Ende von Segment 1 (t=1)
                var epsilon = 0.001;
                var point1Before = SampleArcWithRotor(segment1.Rotor, center1, segment1.StartPoint, segment1.EndPoint, 1.0 - epsilon);
                var tangent1 = connectionPoint - point1Before;
                tangent1 = LinFloat64Vector3D.CreateUnitVector(tangent1.X, tangent1.Y, tangent1.Z);

                // Tangent2: Am Start von Segment 2 (t=0)
                var point2After = SampleArcWithRotor(segment2.Rotor, center2, segment2.StartPoint, segment2.EndPoint, epsilon);
                var tangent2 = point2After - connectionPoint;
                tangent2 = LinFloat64Vector3D.CreateUnitVector(tangent2.X, tangent2.Y, tangent2.Z);

                // Winkel zwischen Tangenten
                var dotProduct = tangent1.VectorESp(tangent2);
                var angle = Math.Acos(Math.Clamp(dotProduct, -1.0, 1.0));

                result.Add((connectionPoint, tangent1, tangent2, angle));
            }

            return result;
        }
    }

    /// <summary>
    /// Test: Controller-basierte Arc-Konstruktion
    /// Simuliert VR-Controller mit verschiedenen Orientierungen
    /// </summary>
    public static void TestControllerBasedArcConstruction()
    {
        Console.WriteLine("=== Controller-Based Arc Construction Test ===\n");

        // Simuliere VR-Controller Positionen
        var p1 = LinFloat64Vector3D.Create(0, 0, 0);
        var p2 = LinFloat64Vector3D.Create(1, 0, 0);

        Console.WriteLine($"Segment: p1 = {p1}, p2 = {p2}");
        Console.WriteLine($"Chord Length: {(p2 - p1).VectorENorm():F4}\n");

        // Test 1: Horizontale Orientierung (Normal zeigt nach oben)
        Console.WriteLine("--- Test 1: Horizontal (Normal = +Z) ---");
        var normalHorizontal = LinFloat64Vector3D.E3; // (0, 0, 1)
        var circleHorizontal = ConstructCircleFromControllerPose(p1, p2, normalHorizontal, curvatureScale: 1.0);
        var decodedH = circleHorizontal.DecodeIpnsRound.Element();

        Console.WriteLine($"  Center: {decodedH.CenterToVector3D()}");
        Console.WriteLine($"  Radius: {decodedH.RealRadius:F4}");
        Console.WriteLine($"  Normal: {decodedH.NormalDirectionToVector3D()}");

        // Verifiziere: p1 und p2 liegen auf Kreis
        var dist1H = (p1 - decodedH.CenterToVector3D()).VectorENorm();
        var dist2H = (p2 - decodedH.CenterToVector3D()).VectorENorm();
        Console.WriteLine($"  Distance p1 to center: {dist1H:F6} (should be {decodedH.RealRadius:F6})");
        Console.WriteLine($"  Distance p2 to center: {dist2H:F6} (should be {decodedH.RealRadius:F6})");
        Console.WriteLine($"  Error p1: {Math.Abs(dist1H - decodedH.RealRadius):E3}");
        Console.WriteLine($"  Error p2: {Math.Abs(dist2H - decodedH.RealRadius):E3}\n");

        // Test 2: Vertikale Orientierung (Normal zeigt nach vorne, Y)
        Console.WriteLine("--- Test 2: Vertical (Normal = +Y) ---");
        var normalVertical = LinFloat64Vector3D.E2; // (0, 1, 0)
        var circleVertical = ConstructCircleFromControllerPose(p1, p2, normalVertical, curvatureScale: 1.0);
        var decodedV = circleVertical.DecodeIpnsRound.Element();

        Console.WriteLine($"  Center: {decodedV.CenterToVector3D()}");
        Console.WriteLine($"  Radius: {decodedV.RealRadius:F4}");
        Console.WriteLine($"  Normal: {decodedV.NormalDirectionToVector3D()}");

        var dist1V = (p1 - decodedV.CenterToVector3D()).VectorENorm();
        var dist2V = (p2 - decodedV.CenterToVector3D()).VectorENorm();
        Console.WriteLine($"  Distance p1 to center: {dist1V:F6}");
        Console.WriteLine($"  Distance p2 to center: {dist2V:F6}");
        Console.WriteLine($"  Error p1: {Math.Abs(dist1V - decodedV.RealRadius):E3}");
        Console.WriteLine($"  Error p2: {Math.Abs(dist2V - decodedV.RealRadius):E3}\n");

        // Test 3: Schräge Orientierung (45° gedreht)
        Console.WriteLine("--- Test 3: Angled (Normal = (1, 1, 1) normalized) ---");
        var normalAngled = LinFloat64Vector3D.CreateUnitVector(1, 1, 1);
        var circleAngled = ConstructCircleFromControllerPose(p1, p2, normalAngled, curvatureScale: 0.5);
        var decodedA = circleAngled.DecodeIpnsRound.Element();

        Console.WriteLine($"  Center: {decodedA.CenterToVector3D()}");
        Console.WriteLine($"  Radius: {decodedA.RealRadius:F4}");
        Console.WriteLine($"  Normal: {decodedA.NormalDirectionToVector3D()}");

        var dist1A = (p1 - decodedA.CenterToVector3D()).VectorENorm();
        var dist2A = (p2 - decodedA.CenterToVector3D()).VectorENorm();
        Console.WriteLine($"  Distance p1 to center: {dist1A:F6}");
        Console.WriteLine($"  Distance p2 to center: {dist2A:F6}");
        Console.WriteLine($"  Error p1: {Math.Abs(dist1A - decodedA.RealRadius):E3}");
        Console.WriteLine($"  Error p2: {Math.Abs(dist2A - decodedA.RealRadius):E3}\n");

        // Test 4: Variable Curvature
        Console.WriteLine("--- Test 4: Variable Curvature Scales ---");
        for (int i = 1; i <= 4; i++)
        {
            var scale = i * 0.5;
            var circle = ConstructCircleFromControllerPose(p1, p2, normalHorizontal, curvatureScale: scale);
            var decoded = circle.DecodeIpnsRound.Element();

            Console.WriteLine($"  Curvature Scale {scale:F1}: Radius = {decoded.RealRadius:F4}, ArcLength = {CalculateArcLength(circle, p1, p2):F4}");
        }

        Console.WriteLine("\n=== Controller Test Complete ===");
    }

    /// <summary>
    /// Test: Multi-Segment Arc-Spline mit C¹-Kontinuität
    /// </summary>
    public static void TestMultiSegmentSpline()
    {
        Console.WriteLine("=== Multi-Segment Arc-Spline Test ===\n");

        // Erstelle Spline mit 4 Segmenten entlang einer Kurve
        var spline = new ArcSpline();

        // Segment 1: Start bei (0,0,0), Ende bei (1,0,0)
        var p1 = LinFloat64Vector3D.Create(0, 0, 0);
        var p2 = LinFloat64Vector3D.Create(1, 0, 0);
        var normal1 = LinFloat64Vector3D.E3; // Nach oben

        Console.WriteLine("Adding Segment 1: (0,0,0) → (1,0,0)");
        spline.AddFirstSegment(p1, p2, normal1, curvatureScale: 1.0);

        // Segment 2: Weiter zu (1.5, 1, 0) - Kurve nach rechts
        var p3 = LinFloat64Vector3D.Create(1.5, 1, 0);
        var normal2 = LinFloat64Vector3D.E3; // Weiter nach oben

        Console.WriteLine("Adding Segment 2: (1,0,0) → (1.5,1,0)");
        spline.AddSegmentFromController(p3, normal2, curvatureScale: 0.8);

        // Segment 3: Weiter zu (1, 2, 0) - Kurve zurück
        var p4 = LinFloat64Vector3D.Create(1, 2, 0);
        var normal3 = LinFloat64Vector3D.E3;

        Console.WriteLine("Adding Segment 3: (1.5,1,0) → (1,2,0)");
        spline.AddSegmentFromController(p4, normal3, curvatureScale: 1.2);

        // Segment 4: Ende bei (0, 2, 0)
        var p5 = LinFloat64Vector3D.Create(0, 2, 0);
        var normal4 = LinFloat64Vector3D.E3;

        Console.WriteLine("Adding Segment 4: (1,2,0) → (0,2,0)");
        spline.AddSegmentFromController(p5, normal4, curvatureScale: 1.0);

        // Debug: Check last segment rotor
        var lastSeg = spline.Segments[^1];
        var lastDecoded = lastSeg.Circle.DecodeIpnsRound.Element();
        Console.WriteLine($"\nDebug Segment 4:");
        Console.WriteLine($"  Center: {lastDecoded.CenterToVector3D()}");
        Console.WriteLine($"  v1 (start-center): {lastSeg.StartPoint - lastDecoded.CenterToVector3D()}");
        Console.WriteLine($"  v2 (end-center): {lastSeg.EndPoint - lastDecoded.CenterToVector3D()}");
        Console.WriteLine($"  Rotor: {lastSeg.Rotor.Multivector}");

        Console.WriteLine($"\nTotal Segments: {spline.Segments.Count}");
        Console.WriteLine($"Total Length: {spline.TotalLength:F4}\n");

        // Zeige Segment-Details
        Console.WriteLine("=== Segment Details ===");
        for (int i = 0; i < spline.Segments.Count; i++)
        {
            var seg = spline.Segments[i];
            var decoded = seg.Circle.DecodeIpnsRound.Element();

            Console.WriteLine($"\nSegment {i + 1}:");
            Console.WriteLine($"  Start: ({seg.StartPoint.X:F2}, {seg.StartPoint.Y:F2}, {seg.StartPoint.Z:F2})");
            Console.WriteLine($"  End:   ({seg.EndPoint.X:F2}, {seg.EndPoint.Y:F2}, {seg.EndPoint.Z:F2})");
            Console.WriteLine($"  Radius: {decoded.RealRadius:F4}");
            Console.WriteLine($"  Arc Length: {seg.ArcLength:F4}");
        }

        // Prüfe C¹-Kontinuität
        Console.WriteLine("\n\n=== C¹-Continuity Check ===");
        var isContinuous = spline.CheckC1Continuity(tolerance: 1e-3);
        Console.WriteLine($"Overall C¹-Continuous: {isContinuous}");

        // Zeige Details an Verbindungspunkten
        var tangents = spline.GetConnectionPointTangents();
        Console.WriteLine("\nConnection Point Details:");
        for (int i = 0; i < tangents.Count; i++)
        {
            var (point, t1, t2, angle) = tangents[i];
            var angleDeg = angle * 180.0 / Math.PI;

            Console.WriteLine($"\nConnection {i + 1} at ({point.X:F2}, {point.Y:F2}, {point.Z:F2}):");
            Console.WriteLine($"  Tangent 1: ({t1.X:F4}, {t1.Y:F4}, {t1.Z:F4})");
            Console.WriteLine($"  Tangent 2: ({t2.X:F4}, {t2.Y:F4}, {t2.Z:F4})");
            Console.WriteLine($"  Angle: {angleDeg:F2}° (should be ~0° for C¹)");
            Console.WriteLine($"  C¹-Continuous: {angleDeg < 1.0}");
        }

        // Sample Punkte entlang der gesamten Spline
        Console.WriteLine("\n\n=== Spline Sampling ===");
        Console.WriteLine("Sampling 10 points along entire spline:");
        for (int i = 0; i <= 10; i++)
        {
            var t = i / 10.0;
            var point = spline.Sample(t);
            Console.WriteLine($"  t={t:F2}: ({point.X:F3}, {point.Y:F3}, {point.Z:F3})");
        }

        // Verifiziere Endpunkte
        Console.WriteLine("\n=== Endpoint Verification ===");
        var startSampled = spline.Sample(0.0);
        var endSampled = spline.Sample(1.0);
        var startError = (startSampled - p1).VectorENorm();
        var endError = (endSampled - p5).VectorENorm();

        Console.WriteLine($"Start point error: {startError:E3} (should be ~0)");
        Console.WriteLine($"End point error: {endError:E3} (should be ~0)");

        // Export to JSON for Babylon.js
        Console.WriteLine("\n=== Exporting to JSON ===");
        var jsonPath = Path.Combine(Environment.CurrentDirectory, "arc-spline-data.json");
        spline.ExportToJsonFile(jsonPath, samplesPerSegment: 30);
        Console.WriteLine($"Exported to: {jsonPath}");

        Console.WriteLine("\n=== Multi-Segment Test Complete ===");
    }
}
