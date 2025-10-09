# VR Arc-Spline Sketch System - TODO

## Projektziel

Entwicklung eines **VR-basierten Arc-Spline Zeichnungssystems** in Conformal Geometric Algebra (CGA) als **VVVV VL Node-Set** in C#.

### Kernkonzepte

- **Koordinatenfrei**: Alle Operationen in CGA (Conformal Geometric Algebra)
- **Real-time VR**: Kontinuierliche Eingabe von Position, Orientation, Pressure
- **Arc-Splines**: Rotor-basierte Kreisbogen-Splines (adaptiert von Ganja.js 2D PGA → 3D CGA)
- **Manipulator-System**: Verkettbare Deformer (Conformal, Rotor-Field, IK-Chain)
- **Flexible Visualisierung**: Multi-Backend Rendering-Pipeline
- **VVVV VL Ready**: Architektur für Node-Set-Integration

---

## Phase 0: Grundlagen & Repository-Analyse ✓

### 0.1 CGA-Funktionalität in GA-Ful verstehen ✓

**Bereits verfügbar in GeometricAlgebraFulcrumLib:**

- ✅ `CGaFloat64GeometricSpace5D` - 5D CGA für 3D Euklidischen Raum
- ✅ `CGaFloat64Round` - Kreise, Sphären (OPNS/IPNS)
- ✅ `CGaFloat64RealRoundComposerUtils` - Kreis-Konstruktion
- ✅ `CGaFloat64TranslationUtils`, `CGaFloat64RotationUtils`, `CGaFloat64ScalingUtils`
- ✅ `CGaFloat64LerpRoundUtils` - Kreis-Interpolation
- ✅ `CGaFloat64ParametricElement` - Zeitabhängige Geometrie
- ✅ `CGaFloat64Visualizer` - BabylonJS Visualisierung

**Wichtige Klassen-Pfade:**
```
GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Float64/
  - CGaFloat64GeometricSpace5D.cs
  - Elements/CGaFloat64Round.cs
  - Operations/CGaFloat64RotationUtils.cs
  - Encoding/CGaFloat64OpnsRoundEncoder.cs
  - Decoding/CGaFloat64OpnsRoundDecoder.cs
  - Visualizer/CGaFloat64Visualizer.cs
```

### 0.2 MetaProgramming-Möglichkeiten erkunden ✓

**Verfügbar für Performance-Optimierung:**

- ✅ `MetaContext` - Symbolische Expression-DAG
- ✅ Common Subexpression Elimination (CSE)
- ✅ Konstantenpropagation
- ✅ Code-Generierung (C#, C++, MATLAB)
- ✅ Genetische Optimierung (`MetaContextGeneticOptimizer`)

**Anwendung:** Später für optimierte Arc-Fitting-Algorithmen Code generieren

### 0.3 Stride3D/Rendering-Situation klären ✓

**Entscheidung:**
- Kein Stride3D vorerst (nur Platzhalter im Repo)
- Ziel: **VVVV VL Node-Set**
- Prototyping: BabylonJS HTML-Viewer (bereits in GA-Ful verfügbar)
- CPU-Implementation fokussiert

---

## Phase 1: Core Data Structures

### 1.1 VR Controller Input Model

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch/Input/VRControllerSample.cs`

```csharp
/// <summary>
/// Einzelner Sample-Punkt vom VR-Controller
/// </summary>
public sealed class VRControllerSample
{
    /// <summary>
    /// Position im Raum (3D Vektor)
    /// </summary>
    public LinFloat64Vector3D Position { get; init; }

    /// <summary>
    /// Orientation im Raum (Quaternion)
    /// </summary>
    public LinFloat64Quaternion Orientation { get; init; }

    /// <summary>
    /// Pressure/Trigger-Wert [0.0, 1.0]
    /// </summary>
    public double Pressure { get; init; }

    /// <summary>
    /// Zeitstempel in Millisekunden
    /// </summary>
    public double Timestamp { get; init; }

    /// <summary>
    /// Berechne Tangente zum nächsten Sample (Bewegungsrichtung)
    /// </summary>
    public LinFloat64Vector3D GetTangentTo(VRControllerSample next)
    {
        return (next.Position - Position).ToUnitVector();
    }

    /// <summary>
    /// Berechne Kreisebene aus Orientation und Bewegungsrichtung
    /// </summary>
    /// <param name="tangent">Bewegungsrichtung</param>
    /// <returns>Bivector der Kreisebene in CGA</returns>
    public LinFloat64Bivector3D GetCirclePlane(LinFloat64Vector3D tangent)
    {
        // Option B4: Controller Orientation + Bewegungsrichtung
        // Forward-Vektor aus Quaternion extrahieren
        var forward = Orientation.RotateVector(LinFloat64Vector3D.E3);

        // Kreisebene = Tangente ∧ (Forward projiziert auf Ebene ⊥ Tangente)
        var normalToTangent = forward - tangent * forward.VectorDot(tangent);

        return tangent.Op(normalToTangent.ToUnitVector());
    }
}
```

**Tasks:**
- [ ] Implementiere `VRControllerSample` Klasse
- [ ] Implementiere `GetTangentTo()` für Bewegungsrichtung
- [ ] Implementiere `GetCirclePlane()` für Option B4 (Orientation + Tangente)
- [ ] Unit-Tests für Plane-Berechnung mit verschiedenen Orientations

**Mathematische Grundlage:**
```
Gegeben:
  - Position p(t) ∈ ℝ³
  - Orientation q(t) ∈ SO(3) (Quaternion)
  - Pressure pr(t) ∈ [0, 1]

Berechne Kreisebene:
  - Tangente t = dp/dt (normalisiert)
  - Forward f = q · e₃ · q̄  (Controller-Forward)
  - Normal n = f - (f·t)t  (projiziert auf ⊥t)
  - Bivector B = t ∧ n  (Kreisebene)
```

---

### 1.2 Arc-Segment (einzelner Kreisbogen)

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch/Geometry/ArcSegment.cs`

```csharp
/// <summary>
/// Einzelnes Arc-Segment des Splines (Kreisbogen in CGA)
/// </summary>
public sealed class ArcSegment
{
    /// <summary>
    /// CGA Geometric Space
    /// </summary>
    private static readonly CGaFloat64GeometricSpace5D Cga =
        CGaFloat64GeometricSpace5D.Instance;

    /// <summary>
    /// Startpunkt des Arcs (CGA Point)
    /// </summary>
    public CGaFloat64Blade StartPoint { get; init; }

    /// <summary>
    /// Endpunkt des Arcs (CGA Point)
    /// </summary>
    public CGaFloat64Blade EndPoint { get; init; }

    /// <summary>
    /// Kreis-Element in CGA (definiert Arc-Geometrie)
    /// </summary>
    public CGaFloat64Round Circle { get; init; }

    /// <summary>
    /// Rotor der diesen Arc definiert (aus Ganja.js Methode)
    /// </summary>
    public CGaFloat64Versor Rotor { get; init; }

    /// <summary>
    /// Start-Timestamp (für Replay)
    /// </summary>
    public double StartTime { get; init; }

    /// <summary>
    /// End-Timestamp (für Replay)
    /// </summary>
    public double EndTime { get; init; }

    /// <summary>
    /// Start-Pressure (für Dicke-Visualisierung)
    /// </summary>
    public double StartPressure { get; init; }

    /// <summary>
    /// End-Pressure (für Dicke-Visualisierung)
    /// </summary>
    public double EndPressure { get; init; }

    /// <summary>
    /// Ist dieses Segment "frozen" (fixiert) oder noch "live" (wird noch bearbeitet)?
    /// </summary>
    public bool IsFrozen { get; set; }

    /// <summary>
    /// Sample Punkte entlang des Arcs (für Visualisierung)
    /// </summary>
    /// <param name="resolution">Anzahl Samples</param>
    /// <returns>Array von CGA Points entlang Arc</returns>
    public CGaFloat64Blade[] SamplePoints(int resolution = 32)
    {
        var points = new CGaFloat64Blade[resolution];

        for (int i = 0; i < resolution; i++)
        {
            double t = i / (resolution - 1.0);

            // Orbit: e^(t·log(R)) ⊲ StartPoint
            var logRotor = Rotor.Log();
            var scaledLog = logRotor.GradeMultiply(t);
            var interpolatedRotor = scaledLog.Exp();

            points[i] = interpolatedRotor.OmMap(StartPoint);
        }

        return points;
    }

    /// <summary>
    /// Interpoliere Pressure entlang des Arcs
    /// </summary>
    public double GetPressureAt(double t)
    {
        return (1 - t) * StartPressure + t * EndPressure;
    }
}
```

**Tasks:**
- [ ] Implementiere `ArcSegment` Klasse
- [ ] Implementiere `SamplePoints()` mit Rotor-Orbit (aus Ganja.js)
- [ ] Implementiere `GetPressureAt()` für Pressure-Interpolation
- [ ] Validiere dass Circle-Geometrie mit Rotor konsistent ist
- [ ] Unit-Tests für verschiedene Arc-Konfigurationen

**Mathematische Grundlage (Rotor Orbit):**
```
Gegeben Rotor R, Startpunkt p:

Arc(t) = e^(t·log(R)) ⊲ p    für t ∈ [0, 1]

wobei:
  - log(R) = Bivector (definiert Rotationsebene + Winkel)
  - e^(B) = Rotor exponential
  - ⊲ = Outermorphism (Sandwich Product)
```

---

### 1.3 Arc-Spline Builder (Real-time Fitting)

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch/Geometry/ArcSplineBuilder.cs`

```csharp
/// <summary>
/// Baut Arc-Spline inkrementell während VR-Zeichnung
/// Basiert auf Ganja.js Algorithmus (2D PGA → 3D CGA)
/// </summary>
public sealed class ArcSplineBuilder
{
    private static readonly CGaFloat64GeometricSpace5D Cga =
        CGaFloat64GeometricSpace5D.Instance;

    /// <summary>
    /// Gefrorene (fixierte) Segmente
    /// </summary>
    private readonly List<ArcSegment> _frozenSegments = new();

    /// <summary>
    /// Gefrorene Stützpunkte (p[0], p[1], ..., p[n])
    /// </summary>
    private readonly List<VRControllerSample> _frozenPoints = new();

    /// <summary>
    /// Gefrorene Kreiszentren (für Rotor-Berechnung)
    /// </summary>
    private readonly List<CGaFloat64Blade> _frozenCenters = new();

    /// <summary>
    /// Aktueller "working point" (noch nicht gefroren)
    /// </summary>
    private VRControllerSample? _workingPoint;

    /// <summary>
    /// Letzter berechneter Punkt (für updateThreshold)
    /// </summary>
    private VRControllerSample? _lastCalculatedPoint;

    /// <summary>
    /// Live Arc-Segment (noch nicht gefroren)
    /// </summary>
    private ArcSegment? _liveSegment;

    /// <summary>
    /// Thresholds für adaptive Sampling
    /// </summary>
    public ArcSplineThresholds Thresholds { get; set; } = new();

    /// <summary>
    /// Initialisierung bei Pressure > 0
    /// </summary>
    public void BeginDrawing(VRControllerSample firstSample)
    {
        _frozenPoints.Clear();
        _frozenSegments.Clear();
        _frozenCenters.Clear();

        _frozenPoints.Add(firstSample);

        // Initiales Zentrum (wie im Ganja.js: c0 = p[0] - offset)
        var initialCenter = CalculateInitialCenter(firstSample);
        _frozenCenters.Add(initialCenter);

        _workingPoint = null;
        _liveSegment = null;
    }

    /// <summary>
    /// Update mit neuem Controller-Sample
    /// </summary>
    public void UpdateDrawing(VRControllerSample newSample)
    {
        _workingPoint = newSample;

        // Prüfe ob Update nötig (updateThreshold)
        if (_lastCalculatedPoint != null)
        {
            var distance = (newSample.Position - _lastCalculatedPoint.Position).Norm();
            if (distance < Thresholds.UpdateThreshold)
                return; // Zu wenig Bewegung, skip Update
        }

        _lastCalculatedPoint = newSample;

        // Berechne Live-Segment
        _liveSegment = CalculateLiveSegment(newSample);

        // Prüfe ob Segment gefroren werden soll
        if (ShouldFreezeSegment(newSample))
        {
            FreezeSegment(newSample);
        }
    }

    /// <summary>
    /// Beende Zeichnung bei Pressure = 0
    /// </summary>
    public ArcSpline EndDrawing()
    {
        // Finales Segment hinzufügen falls vorhanden
        if (_liveSegment != null && _workingPoint != null)
        {
            FreezeSegment(_workingPoint);
        }

        return new ArcSpline(_frozenSegments.ToArray());
    }

    /// <summary>
    /// Berechne Live-Segment (adaptiert von Ganja.js)
    /// </summary>
    private ArcSegment CalculateLiveSegment(VRControllerSample current)
    {
        int n = _frozenPoints.Count - 1; // Index des letzten gefrorenen Punkts
        var pn = _frozenPoints[n];

        // Encode gefrorenen Punkt als CGA Point
        var cgaPn = Cga.Encode.VGa.VectorAsRound(pn.Position);
        var cgaCurrent = Cga.Encode.VGa.VectorAsRound(current.Position);

        // Ganja.js Algorithmus:
        // L1 = Linie durch vorheriges Zentrum und aktuellen Punkt p[n]
        // In 3D CGA: L1 = (previousCenter ∧ p[n])

        var previousCenter = _frozenCenters.Count > n
            ? _frozenCenters[n]
            : _frozenCenters[_frozenCenters.Count - 1];

        // TODO: Implementiere korrekte CGA-Konstruktion
        // Dies ist eine Vereinfachung - muss noch verfeinert werden

        // Berechne Tangente und Kreisebene
        var tangent = pn.GetTangentTo(current);
        var circlePlane = current.GetCirclePlane(tangent);

        // Konstruiere Rotor (vereinfacht - braucht Verfeinerung)
        var rotor = CalculateRotor(pn, current, previousCenter);

        // Konstruiere Kreis aus Rotor und Punkten
        var circle = CalculateCircleFromRotor(rotor, cgaPn, cgaCurrent);

        return new ArcSegment
        {
            StartPoint = cgaPn,
            EndPoint = cgaCurrent,
            Circle = circle,
            Rotor = rotor,
            StartTime = pn.Timestamp,
            EndTime = current.Timestamp,
            StartPressure = pn.Pressure,
            EndPressure = current.Pressure,
            IsFrozen = false
        };
    }

    /// <summary>
    /// Prüfe ob Segment gefroren werden soll (Multi-Threshold)
    /// </summary>
    private bool ShouldFreezeSegment(VRControllerSample current)
    {
        if (_frozenPoints.Count == 0) return false;

        var last = _frozenPoints[^1];

        // Threshold 1: Distanz
        var distance = (current.Position - last.Position).Norm();
        if (distance > Thresholds.DistanceThreshold)
            return true;

        // Threshold 2: Winkel (wenn mindestens 2 Punkte vorhanden)
        if (_frozenPoints.Count >= 2)
        {
            var prevLast = _frozenPoints[^2];
            var v1 = (last.Position - prevLast.Position).ToUnitVector();
            var v2 = (current.Position - last.Position).ToUnitVector();
            var angle = Math.Acos(Math.Clamp(v1.VectorDot(v2), -1, 1));

            if (angle > Thresholds.AngleThreshold)
                return true;
        }

        // Threshold 3: Zeit
        var timeDiff = current.Timestamp - last.Timestamp;
        if (timeDiff > Thresholds.TimeThreshold)
            return true;

        // Threshold 4: Krümmungsänderung (optional - komplexer)
        // TODO: Implementierung

        return false;
    }

    /// <summary>
    /// Friere aktuelles Segment ein
    /// </summary>
    private void FreezeSegment(VRControllerSample newPoint)
    {
        if (_liveSegment == null) return;

        _liveSegment.IsFrozen = true;
        _frozenSegments.Add(_liveSegment);
        _frozenPoints.Add(newPoint);

        // Berechne und speichere neues Zentrum für nächstes Segment
        var newCenter = CalculateNextCenter(_liveSegment);
        _frozenCenters.Add(newCenter);

        _liveSegment = null;
    }

    // TODO: Implementiere Helper-Methoden
    private CGaFloat64Blade CalculateInitialCenter(VRControllerSample first)
    {
        // Ganja.js: c0 = p[0] - offset
        // Offset kann aus Orientation oder Default-Wert kommen
        var offset = first.Orientation.RotateVector(LinFloat64Vector3D.E2) * 0.1;
        return Cga.Encode.VGa.VectorAsRound(first.Position - offset);
    }

    private CGaFloat64Versor CalculateRotor(
        VRControllerSample p1,
        VRControllerSample p2,
        CGaFloat64Blade previousCenter)
    {
        // TODO: Implementiere Ganja.js Rotor-Berechnung in CGA
        // R = (L2 * L1).Normalized
        throw new NotImplementedException();
    }

    private CGaFloat64Round CalculateCircleFromRotor(
        CGaFloat64Versor rotor,
        CGaFloat64Blade start,
        CGaFloat64Blade end)
    {
        // TODO: Extrahiere Kreis-Parameter aus Rotor
        throw new NotImplementedException();
    }

    private CGaFloat64Blade CalculateNextCenter(ArcSegment segment)
    {
        // Zentrum aus Kreis extrahieren
        return Cga.Encode.VGa.VectorAsRound(segment.Circle.CenterToVector3D());
    }
}

/// <summary>
/// Konfigurierbare Thresholds für adaptives Sampling
/// </summary>
public sealed class ArcSplineThresholds
{
    /// <summary>
    /// Minimale Distanz zwischen Stützpunkten (Meter)
    /// </summary>
    public double DistanceThreshold { get; set; } = 0.05; // 5cm

    /// <summary>
    /// Minimale Winkeländerung (Radians)
    /// </summary>
    public double AngleThreshold { get; set; } = 0.26; // ~15°

    /// <summary>
    /// Maximale Zeit zwischen Punkten (Millisekunden)
    /// </summary>
    public double TimeThreshold { get; set; } = 100;

    /// <summary>
    /// Minimale Bewegung für Live-Update (Meter)
    /// </summary>
    public double UpdateThreshold { get; set; } = 0.002; // 2mm

    /// <summary>
    /// Krümmungsänderungs-Threshold (optional)
    /// </summary>
    public double CurvatureThreshold { get; set; } = 0.1;
}
```

**Tasks:**
- [ ] Implementiere `ArcSplineBuilder` Grundstruktur
- [ ] Implementiere `BeginDrawing()`, `UpdateDrawing()`, `EndDrawing()`
- [ ] Implementiere Multi-Threshold `ShouldFreezeSegment()`
- [ ] **KRITISCH:** Implementiere `CalculateRotor()` - Ganja.js → CGA Übersetzung
- [ ] **KRITISCH:** Implementiere `CalculateCircleFromRotor()`
- [ ] Implementiere Curvature-Threshold (optional)
- [ ] Unit-Tests mit simulierten Controller-Daten
- [ ] Performance-Tests (sollte O(1) pro Frame sein)

**Mathematische Challenge:**

Die Ganja.js Rotor-Berechnung in 2D PGA:
```javascript
var L1 = ((R[i-1]??(p[0]*c0.Conjugate)).Log() & ~p[i]);
var L2 = ((p[i] + p[i+1]) | (p[i] & p[i+1]));
R[i] = (L2 * L1).Normalized;
```

Muss übersetzt werden nach 3D CGA. Das erfordert:
1. Verständnis von PGA Point Konstruktion (`!(.25e2+.25e1)`)
2. Verständnis von PGA Line (`&` = meet)
3. Übersetzung zu CGA Kreisen/Ebenen

**TODO für Architekt (DU):**
- [ ] Mathematische Übersetzung Ganja.js PGA → GA-Ful CGA dokumentieren
- [ ] Prototyp der Rotor-Berechnung testen

---

### 1.4 Arc-Spline (vollständige Kurve)

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch/Geometry/ArcSpline.cs`

```csharp
/// <summary>
/// Vollständiger Arc-Spline (Liste von Arc-Segmenten)
/// C¹-kontinuierlich durch natürliche Rotor-Konstruktion
/// </summary>
public sealed class ArcSpline
{
    private readonly ArcSegment[] _segments;

    public IReadOnlyList<ArcSegment> Segments => _segments;

    /// <summary>
    /// Gesamtdauer der Zeichnung (für Replay)
    /// </summary>
    public double TotalDuration =>
        _segments.Length > 0
            ? _segments[^1].EndTime - _segments[0].StartTime
            : 0;

    /// <summary>
    /// Start-Zeit
    /// </summary>
    public double StartTime =>
        _segments.Length > 0 ? _segments[0].StartTime : 0;

    public ArcSpline(ArcSegment[] segments)
    {
        _segments = segments;
    }

    /// <summary>
    /// Sample Punkt auf Spline zu gegebenem Zeitpunkt (für Replay)
    /// </summary>
    public (CGaFloat64Blade Point, double Pressure)? SampleAtTime(double time)
    {
        // Finde Segment das time enthält
        var segment = FindSegmentAtTime(time);
        if (segment == null) return null;

        // Lokaler Parameter t innerhalb Segment
        var t = (time - segment.StartTime) / (segment.EndTime - segment.StartTime);
        t = Math.Clamp(t, 0, 1);

        // Sample Punkt via Rotor-Orbit
        var logRotor = segment.Rotor.Log();
        var scaledLog = logRotor.GradeMultiply(t);
        var interpolatedRotor = scaledLog.Exp();
        var point = interpolatedRotor.OmMap(segment.StartPoint);

        // Interpoliere Pressure
        var pressure = segment.GetPressureAt(t);

        return (point, pressure);
    }

    /// <summary>
    /// Sample Punkt auf Spline zu gegebenem Arc-Length Parameter s ∈ [0,1]
    /// </summary>
    public (CGaFloat64Blade Point, double Pressure)? SampleAtArcLength(double s)
    {
        // TODO: Arc-length Parametrisierung
        // Braucht Vor-Berechnung von Arc-Längen pro Segment
        throw new NotImplementedException();
    }

    /// <summary>
    /// Berechne Gesamtlänge des Splines
    /// </summary>
    public double CalculateTotalLength()
    {
        double totalLength = 0;

        foreach (var segment in _segments)
        {
            // Arc-Länge = radius * angle
            var radius = segment.Circle.RealRadius;
            var angle = CalculateArcAngle(segment);
            totalLength += radius * angle;
        }

        return totalLength;
    }

    /// <summary>
    /// Finde Segment das gegebene Zeit enthält
    /// </summary>
    private ArcSegment? FindSegmentAtTime(double time)
    {
        foreach (var segment in _segments)
        {
            if (time >= segment.StartTime && time <= segment.EndTime)
                return segment;
        }
        return null;
    }

    /// <summary>
    /// Berechne Öffnungswinkel eines Arc-Segments
    /// </summary>
    private double CalculateArcAngle(ArcSegment segment)
    {
        // Angle = 2 * arcsin(chord / (2*radius))
        var chord = (segment.EndPoint.DecodeOpnsRound.PositionToVector3D() -
                     segment.StartPoint.DecodeOpnsRound.PositionToVector3D()).Norm();
        var radius = segment.Circle.RealRadius;

        return 2 * Math.Asin(Math.Clamp(chord / (2 * radius), -1, 1));
    }
}
```

**Tasks:**
- [ ] Implementiere `ArcSpline` Klasse
- [ ] Implementiere `SampleAtTime()` für Time-basiertes Replay
- [ ] Implementiere `SampleAtArcLength()` für Arc-Length Parametrisierung
- [ ] Implementiere `CalculateTotalLength()`
- [ ] Unit-Tests für verschiedene Spline-Konfigurationen

---

### 1.5 Sketch (Liste von Arc-Splines)

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch/Geometry/Sketch.cs`

```csharp
/// <summary>
/// Sketch = Sammlung von Arc-Splines
/// Kann als Ganzes transformiert und deformiert werden
/// </summary>
public sealed class Sketch
{
    private readonly List<ArcSpline> _splines = new();

    public IReadOnlyList<ArcSpline> Splines => _splines;

    /// <summary>
    /// Gesamtdauer des Sketches (für Replay)
    /// </summary>
    public double TotalDuration
    {
        get
        {
            if (_splines.Count == 0) return 0;
            var start = _splines.Min(s => s.StartTime);
            var end = _splines.Max(s => s.StartTime + s.TotalDuration);
            return end - start;
        }
    }

    /// <summary>
    /// Füge neuen Arc-Spline hinzu
    /// </summary>
    public void AddSpline(ArcSpline spline)
    {
        _splines.Add(spline);
    }

    /// <summary>
    /// Sample alle Splines zu gegebenem Zeitpunkt (für Replay)
    /// </summary>
    public IEnumerable<(CGaFloat64Blade Point, double Pressure)> SampleAtTime(double time)
    {
        foreach (var spline in _splines)
        {
            var sample = spline.SampleAtTime(time);
            if (sample.HasValue)
                yield return sample.Value;
        }
    }

    /// <summary>
    /// Berechne Bounding Sphere des Sketches (für Deformation-Referenz)
    /// </summary>
    public CGaFloat64Round CalculateBoundingSphere()
    {
        // Sammle alle Punkte
        var allPoints = new List<LinFloat64Vector3D>();

        foreach (var spline in _splines)
        {
            foreach (var segment in spline.Segments)
            {
                allPoints.Add(segment.StartPoint.DecodeOpnsRound.PositionToVector3D());
                allPoints.Add(segment.EndPoint.DecodeOpnsRound.PositionToVector3D());
            }
        }

        // Berechne Zentrum (Schwerpunkt)
        var center = allPoints.Aggregate(LinFloat64Vector3D.Zero, (sum, p) => sum + p)
                     / allPoints.Count;

        // Berechne Radius (max Distanz zu Zentrum)
        var radius = allPoints.Max(p => (p - center).Norm());

        return CGaFloat64GeometricSpace5D.Instance.DefineRealRoundSphere(radius, center);
    }
}
```

**Tasks:**
- [ ] Implementiere `Sketch` Klasse
- [ ] Implementiere `AddSpline()`
- [ ] Implementiere `SampleAtTime()` für Multi-Spline Replay
- [ ] Implementiere `CalculateBoundingSphere()` (für spätere Deformation)
- [ ] Unit-Tests

---

## Phase 2: Manipulator-System (Prototyp B → E → D)

### 2.1 Interface Design

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch/Manipulation/ISketchManipulator.cs`

```csharp
/// <summary>
/// Interface für alle Sketch-Manipulatoren
/// Gewährleistet koordinatenfreie Verkettbarkeit
/// </summary>
public interface ISketchManipulator
{
    /// <summary>
    /// Name des Manipulators (für UI/Debugging)
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Ist Manipulator aktiv?
    /// </summary>
    bool IsActive { get; set; }

    /// <summary>
    /// Transformiere einzelnen Punkt (koordinatenfrei in CGA)
    /// </summary>
    CGaFloat64Blade TransformPoint(CGaFloat64Blade point);

    /// <summary>
    /// Transformiere Arc-Segment
    /// </summary>
    ArcSegment TransformSegment(ArcSegment segment);

    /// <summary>
    /// Transformiere Arc-Spline
    /// </summary>
    ArcSpline TransformSpline(ArcSpline spline);

    /// <summary>
    /// Transformiere kompletten Sketch
    /// </summary>
    Sketch TransformSketch(Sketch sketch);

    /// <summary>
    /// Update Manipulator (z.B. für Animationen, IK-Updates)
    /// </summary>
    void Update(double deltaTime);
}

/// <summary>
/// Basis-Implementierung mit default Transform-Hierarchie
/// </summary>
public abstract class SketchManipulatorBase : ISketchManipulator
{
    public abstract string Name { get; }
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Implementiere nur Punkt-Transformation (default für alle Manipulatoren)
    /// </summary>
    public abstract CGaFloat64Blade TransformPoint(CGaFloat64Blade point);

    public virtual ArcSegment TransformSegment(ArcSegment segment)
    {
        if (!IsActive) return segment;

        // Transform Start/End Points
        var newStart = TransformPoint(segment.StartPoint);
        var newEnd = TransformPoint(segment.EndPoint);

        // Transform Circle
        var newCenter = TransformPoint(
            CGaFloat64GeometricSpace5D.Instance.Encode.VGa.VectorAsRound(
                segment.Circle.CenterToVector3D()));

        // Rebuild Circle mit neuen Parametern
        // TODO: Rotor muss auch transformiert werden!

        return new ArcSegment
        {
            StartPoint = newStart,
            EndPoint = newEnd,
            Circle = segment.Circle, // TODO: Transform Circle properly
            Rotor = segment.Rotor,   // TODO: Transform Rotor
            StartTime = segment.StartTime,
            EndTime = segment.EndTime,
            StartPressure = segment.StartPressure,
            EndPressure = segment.EndPressure,
            IsFrozen = segment.IsFrozen
        };
    }

    public virtual ArcSpline TransformSpline(ArcSpline spline)
    {
        if (!IsActive) return spline;

        var transformedSegments = spline.Segments
            .Select(TransformSegment)
            .ToArray();

        return new ArcSpline(transformedSegments);
    }

    public virtual Sketch TransformSketch(Sketch sketch)
    {
        if (!IsActive) return sketch;

        var transformedSketch = new Sketch();

        foreach (var spline in sketch.Splines)
        {
            transformedSketch.AddSpline(TransformSpline(spline));
        }

        return transformedSketch;
    }

    public virtual void Update(double deltaTime) { }
}
```

**Tasks:**
- [ ] Implementiere `ISketchManipulator` Interface
- [ ] Implementiere `SketchManipulatorBase` mit Default-Hierarchie
- [ ] **WICHTIG:** Klären wie Rotor/Circle richtig transformiert werden (koordinatenfrei!)

---

### 2.2 Manipulator Stack (Verkettung)

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch/Manipulation/ManipulatorStack.cs`

```csharp
/// <summary>
/// Stack von verketteten Manipulatoren
/// Transform-Pipeline: Sketch → M1 → M2 → M3 → ... → Final
/// </summary>
public sealed class ManipulatorStack : ISketchManipulator
{
    private readonly List<ISketchManipulator> _manipulators = new();

    public string Name => "Manipulator Stack";
    public bool IsActive { get; set; } = true;

    public IReadOnlyList<ISketchManipulator> Manipulators => _manipulators;

    /// <summary>
    /// Füge Manipulator hinzu (ans Ende der Pipeline)
    /// </summary>
    public void AddManipulator(ISketchManipulator manipulator)
    {
        _manipulators.Add(manipulator);
    }

    /// <summary>
    /// Entferne Manipulator
    /// </summary>
    public void RemoveManipulator(ISketchManipulator manipulator)
    {
        _manipulators.Remove(manipulator);
    }

    /// <summary>
    /// Verkettete Punkt-Transformation
    /// </summary>
    public CGaFloat64Blade TransformPoint(CGaFloat64Blade point)
    {
        if (!IsActive) return point;

        var result = point;
        foreach (var manipulator in _manipulators)
        {
            if (manipulator.IsActive)
                result = manipulator.TransformPoint(result);
        }
        return result;
    }

    public ArcSegment TransformSegment(ArcSegment segment)
    {
        if (!IsActive) return segment;

        var result = segment;
        foreach (var manipulator in _manipulators)
        {
            if (manipulator.IsActive)
                result = manipulator.TransformSegment(result);
        }
        return result;
    }

    public ArcSpline TransformSpline(ArcSpline spline)
    {
        if (!IsActive) return spline;

        var result = spline;
        foreach (var manipulator in _manipulators)
        {
            if (manipulator.IsActive)
                result = manipulator.TransformSpline(result);
        }
        return result;
    }

    public Sketch TransformSketch(Sketch sketch)
    {
        if (!IsActive) return sketch;

        var result = sketch;
        foreach (var manipulator in _manipulators)
        {
            if (manipulator.IsActive)
                result = manipulator.TransformSketch(result);
        }
        return result;
    }

    public void Update(double deltaTime)
    {
        foreach (var manipulator in _manipulators)
        {
            manipulator.Update(deltaTime);
        }
    }
}
```

**Tasks:**
- [ ] Implementiere `ManipulatorStack`
- [ ] Unit-Tests für Verkettung mehrerer Manipulatoren
- [ ] Performance-Tests (verkettete Transformationen sollten effizient sein)

---

### 2.3 Prototyp B: Conformal Manipulator

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch/Manipulation/ConformalManipulator.cs`

```csharp
/// <summary>
/// Konforme Transformationen (Möbius-Transformationen in CGA)
/// Erhält Winkel, Kreise bleiben Kreise
/// </summary>
public sealed class ConformalManipulator : SketchManipulatorBase
{
    private static readonly CGaFloat64GeometricSpace5D Cga =
        CGaFloat64GeometricSpace5D.Instance;

    public override string Name => "Conformal Transform";

    /// <summary>
    /// Versor (Produkt von Reflektionen) definiert konforme Transformation
    /// V = S1 * S2 * ... * Sn
    /// </summary>
    public CGaFloat64Versor Versor { get; set; }

    public ConformalManipulator()
    {
        // Identity Versor
        Versor = Cga.ScalarProcessor.CreateScalar(1).CreateVersor();
    }

    public override CGaFloat64Blade TransformPoint(CGaFloat64Blade point)
    {
        // Sandwich Product: p' = V ⊲ p ⊲ Ṽ
        return Versor.OmMap(point);
    }

    public override ArcSegment TransformSegment(ArcSegment segment)
    {
        // Konforme Transformationen erhalten Kreise!
        // Arc bleibt Arc, nur transformiert

        var newStart = TransformPoint(segment.StartPoint);
        var newEnd = TransformPoint(segment.EndPoint);

        // Transform Circle (bleibt Kreis!)
        var circleCenter = Cga.Encode.VGa.VectorAsRound(segment.Circle.CenterToVector3D());
        var newCenter = TransformPoint(circleCenter);

        // Radius bleibt gleich bei reinen Rotationen/Translationen
        // Bei Inversionen ändert sich Radius - muss berechnet werden
        var newRadius = CalculateTransformedRadius(segment.Circle);

        // Normal-Bivector transformieren
        var normalVector = segment.Circle.NormalDirectionToVector3D();
        var normalAsRound = Cga.Encode.VGa.VectorAsRound(normalVector);
        var transformedNormal = TransformPoint(normalAsRound);

        // Rebuild Circle
        var newCircle = Cga.DefineRealRoundCircle(
            weight: segment.Circle.Weight,
            radiusSquared: newRadius * newRadius,
            center: newCenter.DecodeOpnsRound.PositionToVector3D(),
            directionBivector: LinFloat64Bivector3D.Create(0, 0, 1) // TODO: from transformed normal
        );

        // Rotor transformieren (TODO: wie?)
        var newRotor = TransformRotor(segment.Rotor);

        return new ArcSegment
        {
            StartPoint = newStart,
            EndPoint = newEnd,
            Circle = newCircle,
            Rotor = newRotor,
            StartTime = segment.StartTime,
            EndTime = segment.EndTime,
            StartPressure = segment.StartPressure,
            EndPressure = segment.EndPressure,
            IsFrozen = segment.IsFrozen
        };
    }

    // Helper: Berechne transformierten Radius
    private double CalculateTransformedRadius(CGaFloat64Round circle)
    {
        // Bei Translation/Rotation bleibt Radius gleich
        // Bei Inversion: r' = r / (distance_to_inversion_center)²
        // TODO: Implementierung basierend auf Versor-Typ
        return circle.RealRadius;
    }

    // Helper: Transformiere Rotor
    private CGaFloat64Versor TransformRotor(CGaFloat64Versor rotor)
    {
        // Konjugation: R' = V * R * Ṽ
        // TODO: Verifizieren dass das korrekt ist
        return (Versor.Multivector * rotor.Multivector * Versor.Multivector.Reverse())
            .CreateVersor();
    }

    // === Factory Methods für verschiedene konforme Transformationen ===

    /// <summary>
    /// Translation
    /// </summary>
    public static ConformalManipulator CreateTranslation(LinFloat64Vector3D translation)
    {
        var cga = CGaFloat64GeometricSpace5D.Instance;

        // Translator: T = 1 + (e_inf ∧ v)/2
        var translator = cga.CreateCGaTranslator(translation);

        return new ConformalManipulator { Versor = translator };
    }

    /// <summary>
    /// Rotation um Achse
    /// </summary>
    public static ConformalManipulator CreateRotation(
        LinFloat64Vector3D axisPoint,
        LinFloat64Vector3D axisDirection,
        double angleRadians)
    {
        var cga = CGaFloat64GeometricSpace5D.Instance;

        // Rotor: R = e^(B*angle/2)
        var rotor = cga.CreateCGaRotor(axisPoint, axisDirection, angleRadians);

        return new ConformalManipulator { Versor = rotor };
    }

    /// <summary>
    /// Uniform Scaling
    /// </summary>
    public static ConformalManipulator CreateScaling(
        LinFloat64Vector3D center,
        double scaleFactor)
    {
        var cga = CGaFloat64GeometricSpace5D.Instance;

        // Scaling via Dilation: D = e^(log(s) * e_inf ∧ c)
        var scaler = cga.CreateCGaScaler(center, scaleFactor);

        return new ConformalManipulator { Versor = scaler };
    }

    /// <summary>
    /// Inversion an Sphäre
    /// </summary>
    public static ConformalManipulator CreateInversion(
        LinFloat64Vector3D sphereCenter,
        double sphereRadius)
    {
        var cga = CGaFloat64GeometricSpace5D.Instance;

        // Inversion: S = 2*c - 1  (vereinfacht)
        // TODO: Korrekte CGA Inversion
        throw new NotImplementedException();
    }

    /// <summary>
    /// Verkette mehrere konforme Transformationen
    /// </summary>
    public static ConformalManipulator Compose(params ConformalManipulator[] manipulators)
    {
        var composed = new ConformalManipulator();

        // Versors multiplizieren
        var result = manipulators[0].Versor;
        for (int i = 1; i < manipulators.Length; i++)
        {
            result = (result.Multivector * manipulators[i].Versor.Multivector).CreateVersor();
        }

        composed.Versor = result;
        return composed;
    }
}
```

**Tasks (Prototyp B - Priorität 1):**
- [ ] Implementiere `ConformalManipulator` Basis
- [ ] Implementiere `TransformPoint()` mit Sandwich Product
- [ ] Implementiere `CreateTranslation()` Factory
- [ ] Implementiere `CreateRotation()` Factory
- [ ] Implementiere `CreateScaling()` Factory
- [ ] **Optional:** Implementiere `CreateInversion()` (komplex!)
- [ ] Implementiere `Compose()` für Versor-Verkettung
- [ ] **WICHTIG:** Klären wie Circle/Rotor korrekt transformiert werden
- [ ] Unit-Tests für alle Transformationen
- [ ] Prototyp-Viewer Integration (Gizmos kommen später)

**Mathematische Referenzen:**
- GA-Ful: `CGaFloat64TranslationUtils.cs`, `CGaFloat64RotationUtils.cs`
- Konforme Transformationen erhalten: Winkel, Kreise → Kreise/Linien

---

### 2.4 Prototyp E: Rotor-Field Manipulator (Guide-Spline)

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch/Manipulation/RotorFieldManipulator.cs`

```csharp
/// <summary>
/// Deformiert Sketch entlang eines Guide-Splines
/// Verwendet Rotor-Feld für koordinatenfreie Deformation
/// </summary>
public sealed class RotorFieldManipulator : SketchManipulatorBase
{
    private static readonly CGaFloat64GeometricSpace5D Cga =
        CGaFloat64GeometricSpace5D.Instance;

    public override string Name => "Rotor Field (Guide Spline)";

    /// <summary>
    /// Original "Spine" des Sketches (implizite zentrale Kurve)
    /// </summary>
    public ArcSpline? OriginalSpine { get; set; }

    /// <summary>
    /// Guide-Spline (neue Spine)
    /// </summary>
    public ArcSpline? GuideSpline { get; set; }

    /// <summary>
    /// Falloff-Radius für Influence (Meter)
    /// </summary>
    public double FalloffRadius { get; set; } = 1.0;

    /// <summary>
    /// Falloff-Kurve (0 = linear, 1 = smooth, 2 = smoother)
    /// </summary>
    public double FalloffExponent { get; set; } = 1.0;

    /// <summary>
    /// Rotor-Cache für Performance
    /// Key = Arc-Length Parameter s
    /// </summary>
    private readonly Dictionary<double, CGaFloat64Versor> _rotorCache = new();

    public override CGaFloat64Blade TransformPoint(CGaFloat64Blade point)
    {
        if (OriginalSpine == null || GuideSpline == null)
            return point;

        var p = point.DecodeOpnsRound.PositionToVector3D();

        // 1. Finde closest point auf Original Spine → Arc-Length Parameter s
        var (s, closestPoint) = FindClosestPointOnSpine(p, OriginalSpine);

        // 2. Berechne Distanz zu Spine
        var distance = (p - closestPoint).Norm();

        // 3. Berechne Influence (Falloff)
        var influence = CalculateFalloff(distance);

        if (influence < 0.001) // Zu weit weg, keine Deformation
            return point;

        // 4. Hole Rotor für Parameter s (oder berechne + cache)
        var rotor = GetOrComputeRotorAt(s);

        // 5. Interpoliere zwischen Identity und Rotor basierend auf Influence
        var interpolatedRotor = InterpolateRotor(rotor, influence);

        // 6. Transformiere Punkt
        var transformed = interpolatedRotor.OmMap(point);

        return transformed;
    }

    /// <summary>
    /// Finde closest point auf Spline und Arc-Length Parameter
    /// </summary>
    private (double s, LinFloat64Vector3D closestPoint) FindClosestPointOnSpine(
        LinFloat64Vector3D point,
        ArcSpline spine)
    {
        // TODO: Effiziente Implementierung
        // Brute-Force für Prototyp: Sample Spline und finde closest

        double minDistance = double.MaxValue;
        double bestS = 0;
        LinFloat64Vector3D bestPoint = LinFloat64Vector3D.Zero;

        int samples = 100;
        for (int i = 0; i < samples; i++)
        {
            double s = i / (samples - 1.0);
            var sample = SampleSplineAtArcLength(spine, s);
            var distance = (point - sample).Norm();

            if (distance < minDistance)
            {
                minDistance = distance;
                bestS = s;
                bestPoint = sample;
            }
        }

        return (bestS, bestPoint);
    }

    /// <summary>
    /// Sample Spline bei Arc-Length Parameter s ∈ [0,1]
    /// </summary>
    private LinFloat64Vector3D SampleSplineAtArcLength(ArcSpline spline, double s)
    {
        // TODO: Echte Arc-Length Parametrisierung
        // Für Prototyp: Lineare Interpolation über Segmente

        var totalLength = spline.CalculateTotalLength();
        var targetLength = s * totalLength;

        double accumulatedLength = 0;

        foreach (var segment in spline.Segments)
        {
            var segmentLength = segment.Circle.RealRadius * CalculateArcAngle(segment);

            if (accumulatedLength + segmentLength >= targetLength)
            {
                // Dieser Segment enthält den target point
                var localT = (targetLength - accumulatedLength) / segmentLength;
                return SampleSegmentAt(segment, localT);
            }

            accumulatedLength += segmentLength;
        }

        // Fallback: letzter Punkt
        return spline.Segments[^1].EndPoint.DecodeOpnsRound.PositionToVector3D();
    }

    private LinFloat64Vector3D SampleSegmentAt(ArcSegment segment, double t)
    {
        var logRotor = segment.Rotor.Log();
        var scaledLog = logRotor.GradeMultiply(t);
        var interpolatedRotor = scaledLog.Exp();
        var point = interpolatedRotor.OmMap(segment.StartPoint);
        return point.DecodeOpnsRound.PositionToVector3D();
    }

    private double CalculateArcAngle(ArcSegment segment)
    {
        var chord = (segment.EndPoint.DecodeOpnsRound.PositionToVector3D() -
                     segment.StartPoint.DecodeOpnsRound.PositionToVector3D()).Norm();
        var radius = segment.Circle.RealRadius;
        return 2 * Math.Asin(Math.Clamp(chord / (2 * radius), -1, 1));
    }

    /// <summary>
    /// Berechne Falloff basierend auf Distanz
    /// </summary>
    private double CalculateFalloff(double distance)
    {
        if (distance > FalloffRadius) return 0;

        var normalized = distance / FalloffRadius;
        return Math.Pow(1 - normalized, FalloffExponent);
    }

    /// <summary>
    /// Hole oder berechne Rotor für Arc-Length Parameter s
    /// </summary>
    private CGaFloat64Versor GetOrComputeRotorAt(double s)
    {
        // Runde s für Cache (z.B. auf 0.01 Schritte)
        var cacheKey = Math.Round(s * 100) / 100.0;

        if (_rotorCache.TryGetValue(cacheKey, out var cached))
            return cached;

        // Berechne Rotor
        var rotor = ComputeRotorAt(s);
        _rotorCache[cacheKey] = rotor;

        return rotor;
    }

    /// <summary>
    /// Berechne Rotor der Original → Guide Transformation bei Parameter s
    /// </summary>
    private CGaFloat64Versor ComputeRotorAt(double s)
    {
        // Sample Original Spine und Guide Spine bei s
        var origPos = SampleSplineAtArcLength(OriginalSpine!, s);
        var guidePos = SampleSplineAtArcLength(GuideSpline!, s);

        // Berechne Tangenten (numerisch)
        var ds = 0.01;
        var origTangent = (SampleSplineAtArcLength(OriginalSpine!, Math.Min(s + ds, 1)) - origPos).ToUnitVector();
        var guideTangent = (SampleSplineAtArcLength(GuideSpline!, Math.Min(s + ds, 1)) - guidePos).ToUnitVector();

        // Konstruiere Rotor der:
        // 1. origPos → guidePos bewegt (Translation)
        // 2. origTangent → guideTangent dreht (Rotation)

        // Translation
        var translation = guidePos - origPos;
        var translator = Cga.CreateCGaTranslator(translation);

        // Rotation (von origTangent zu guideTangent)
        var rotationAxis = origTangent.VectorCross(guideTangent);
        var rotationAngle = Math.Acos(Math.Clamp(origTangent.VectorDot(guideTangent), -1, 1));

        CGaFloat64Versor rotator;
        if (rotationAxis.Norm() > 0.001)
        {
            rotator = Cga.CreateCGaRotor(origPos, rotationAxis, rotationAngle);
        }
        else
        {
            // Tangenten parallel, keine Rotation
            rotator = Cga.ScalarProcessor.CreateScalar(1).CreateVersor();
        }

        // Kombiniere: Erst drehen, dann bewegen
        var combined = (translator.Multivector * rotator.Multivector).CreateVersor();

        return combined;
    }

    /// <summary>
    /// Interpoliere zwischen Identity und Rotor
    /// </summary>
    private CGaFloat64Versor InterpolateRotor(CGaFloat64Versor rotor, double t)
    {
        if (t <= 0) return Cga.ScalarProcessor.CreateScalar(1).CreateVersor();
        if (t >= 1) return rotor;

        // Logarithmic interpolation: e^(t * log(R))
        var logRotor = rotor.Log();
        var scaledLog = logRotor.GradeMultiply(t);
        return scaledLog.Exp().CreateVersor();
    }

    /// <summary>
    /// Clear Rotor-Cache (z.B. wenn Guide-Spline sich ändert)
    /// </summary>
    public void InvalidateCache()
    {
        _rotorCache.Clear();
    }
}
```

**Tasks (Prototyp E - Priorität 2):**
- [ ] Implementiere `RotorFieldManipulator` Basis
- [ ] Implementiere `TransformPoint()` mit Closest-Point-Search
- [ ] Implementiere `FindClosestPointOnSpine()` (Brute-Force für Prototyp OK)
- [ ] Implementiere `ComputeRotorAt()` mit Translation + Rotation
- [ ] Implementiere Falloff-System
- [ ] Implementiere Rotor-Interpolation
- [ ] Implementiere Rotor-Cache für Performance
- [ ] **TODO:** Original-Spine automatisch aus Sketch extrahieren
- [ ] Unit-Tests mit einfachen Splines
- [ ] Performance-Tests (sollte real-time fähig sein)

**Optimierungen für später:**
- [ ] Spatial Hashing für Closest-Point-Search
- [ ] Adaptive Sampling basierend auf Krümmung
- [ ] Parallele Rotor-Berechnung

---

### 2.5 Prototyp D: IK-Chain Manipulator

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch/Manipulation/IKChainManipulator.cs`

```csharp
/// <summary>
/// IK-Joint in CGA (koordinatenfrei)
/// </summary>
public sealed class IKJoint
{
    /// <summary>
    /// Position des Joints
    /// </summary>
    public LinFloat64Vector3D Position { get; set; }

    /// <summary>
    /// Rotor definiert Orientation des Joints
    /// </summary>
    public CGaFloat64Versor Rotor { get; set; }

    /// <summary>
    /// Parent Joint (null für Root)
    /// </summary>
    public IKJoint? Parent { get; set; }

    /// <summary>
    /// Bone Length zum Parent
    /// </summary>
    public double BoneLength { get; set; }

    /// <summary>
    /// Gewichtung für Skinning
    /// </summary>
    public double Weight { get; set; } = 1.0;
}

/// <summary>
/// IK-Chain Manipulator für "Character-like" Sketch-Deformation
/// </summary>
public sealed class IKChainManipulator : SketchManipulatorBase
{
    private static readonly CGaFloat64GeometricSpace5D Cga =
        CGaFloat64GeometricSpace5D.Instance;

    public override string Name => "IK Chain";

    /// <summary>
    /// Joints der IK-Chain
    /// </summary>
    public List<IKJoint> Joints { get; } = new();

    /// <summary>
    /// IK Target (für End-Effector)
    /// </summary>
    public LinFloat64Vector3D? Target { get; set; }

    /// <summary>
    /// IK-Solver Iterations
    /// </summary>
    public int SolverIterations { get; set; } = 10;

    /// <summary>
    /// Threshold für IK-Konvergenz
    /// </summary>
    public double ConvergenceThreshold { get; set; } = 0.001;

    public override CGaFloat64Blade TransformPoint(CGaFloat64Blade point)
    {
        if (Joints.Count == 0)
            return point;

        var p = point.DecodeOpnsRound.PositionToVector3D();

        // Gewichtete Summe aller Joint-Transformationen (Skinning)
        var result = LinFloat64Vector3D.Zero;
        double totalWeight = 0;

        foreach (var joint in Joints)
        {
            var weight = CalculateJointWeight(p, joint);
            if (weight < 0.001) continue;

            // Transformiere Punkt durch Joint-Rotor
            var transformed = joint.Rotor.OmMap(point);
            var transformedPos = transformed.DecodeOpnsRound.PositionToVector3D();

            result += weight * transformedPos;
            totalWeight += weight;
        }

        if (totalWeight > 0)
        {
            result /= totalWeight;
            return Cga.Encode.VGa.VectorAsRound(result);
        }

        return point;
    }

    /// <summary>
    /// Berechne Gewichtung eines Punktes zu einem Joint
    /// </summary>
    private double CalculateJointWeight(LinFloat64Vector3D point, IKJoint joint)
    {
        // Distanz-basierte Gewichtung
        var distance = (point - joint.Position).Norm();

        // Falloff basierend auf Bone-Length
        var falloffRadius = joint.BoneLength * 2; // Konfigurierbar

        if (distance > falloffRadius) return 0;

        var normalized = distance / falloffRadius;
        return Math.Pow(1 - normalized, 2); // Quadratischer Falloff
    }

    /// <summary>
    /// Solve IK (FABRIK-Algorithmus)
    /// Forward And Backward Reaching Inverse Kinematics
    /// </summary>
    public void SolveIK()
    {
        if (Joints.Count < 2 || Target == null)
            return;

        var target = Target.Value;

        // Speichere Original-Root Position
        var rootPosition = Joints[0].Position;

        for (int iter = 0; iter < SolverIterations; iter++)
        {
            // Forward Pass: Reach toward target
            Joints[^1].Position = target;

            for (int i = Joints.Count - 2; i >= 0; i--)
            {
                var child = Joints[i + 1];
                var current = Joints[i];

                // Richtung von child zu current
                var direction = (current.Position - child.Position).ToUnitVector();

                // Neue Position: child.Position + direction * boneLength
                current.Position = child.Position + direction * child.BoneLength;

                // Update Rotor basierend auf neuer Orientation
                if (i > 0)
                {
                    var originalDirection = (Joints[i + 1].Position - Joints[i - 1].Position).ToUnitVector();
                    current.Rotor = CalculateRotor(originalDirection, direction);
                }
            }

            // Backward Pass: Satisfy root constraint
            Joints[0].Position = rootPosition;

            for (int i = 1; i < Joints.Count; i++)
            {
                var parent = Joints[i - 1];
                var current = Joints[i];

                // Richtung von parent zu current
                var direction = (current.Position - parent.Position).ToUnitVector();

                // Neue Position
                current.Position = parent.Position + direction * current.BoneLength;

                // Update Rotor
                if (i < Joints.Count - 1)
                {
                    var originalDirection = (Joints[i + 1].Position - Joints[i - 1].Position).ToUnitVector();
                    current.Rotor = CalculateRotor(originalDirection, direction);
                }
            }

            // Check Konvergenz
            var endEffectorDistance = (Joints[^1].Position - target).Norm();
            if (endEffectorDistance < ConvergenceThreshold)
                break;
        }
    }

    /// <summary>
    /// Berechne Rotor zwischen zwei Richtungen (in CGA)
    /// </summary>
    private CGaFloat64Versor CalculateRotor(
        LinFloat64Vector3D fromDirection,
        LinFloat64Vector3D toDirection)
    {
        var axis = fromDirection.VectorCross(toDirection);
        var angle = Math.Acos(Math.Clamp(fromDirection.VectorDot(toDirection), -1, 1));

        if (axis.Norm() < 0.001) // Parallel
            return Cga.ScalarProcessor.CreateScalar(1).CreateVersor();

        return Cga.CreateCGaRotor(LinFloat64Vector3D.Zero, axis, angle);
    }

    /// <summary>
    /// Update (löst IK wenn Target gesetzt ist)
    /// </summary>
    public override void Update(double deltaTime)
    {
        if (Target.HasValue)
        {
            SolveIK();
        }
    }

    /// <summary>
    /// Extrahiere IK-Rig aus Sketch (automatisch)
    /// </summary>
    public static IKChainManipulator ExtractRigFromSketch(
        Sketch sketch,
        int numberOfJoints = 5)
    {
        // TODO: Intelligentes Rig-Extraction
        // Für Prototyp: Gleichmäßig verteilte Joints entlang Bounding-Box

        var manipulator = new IKChainManipulator();

        // Placeholder: Erstelle einfache Chain
        // Echte Implementierung würde Sketch-Geometrie analysieren

        return manipulator;
    }
}
```

**Tasks (Prototyp D - Priorität 3):**
- [ ] Implementiere `IKJoint` Klasse
- [ ] Implementiere `IKChainManipulator` Basis
- [ ] Implementiere `TransformPoint()` mit Joint-Skinning
- [ ] Implementiere `CalculateJointWeight()` mit Falloff
- [ ] Implementiere `SolveIK()` mit FABRIK-Algorithmus
- [ ] Implementiere `CalculateRotor()` zwischen Richtungen
- [ ] **Optional:** Implementiere `ExtractRigFromSketch()` (automatisches Rigging)
- [ ] Unit-Tests mit einfachen IK-Chains
- [ ] Performance-Tests

**Erweiterungen für später:**
- [ ] CCD (Cyclic Coordinate Descent) als alternativer IK-Solver
- [ ] Joint Constraints (Angle Limits)
- [ ] Multiple End-Effectors

---

## Phase 3: Visualization Pipeline

### 3.1 Visualization Interface

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch/Visualization/ISketchRenderer.cs`

```csharp
/// <summary>
/// Interface für verschiedene Rendering-Backends
/// </summary>
public interface ISketchRenderer
{
    /// <summary>
    /// Initialisiere Renderer
    /// </summary>
    void Initialize();

    /// <summary>
    /// Render Sketch
    /// </summary>
    void RenderSketch(Sketch sketch);

    /// <summary>
    /// Render einzelnen Arc-Spline
    /// </summary>
    void RenderSpline(ArcSpline spline);

    /// <summary>
    /// Render einzelnes Arc-Segment
    /// </summary>
    void RenderSegment(ArcSegment segment);

    /// <summary>
    /// Clear Scene
    /// </summary>
    void Clear();

    /// <summary>
    /// Finalize und Export (z.B. HTML für BabylonJS)
    /// </summary>
    void Finalize(string outputPath);
}
```

**Tasks:**
- [ ] Implementiere `ISketchRenderer` Interface

---

### 3.2 Prototyp Renderer: Option A (Polylinie + Pressure-Kreise)

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch/Visualization/SimplePolylineRenderer.cs`

```csharp
/// <summary>
/// Einfachster Renderer: Polylinie mit Kreisen für Pressure
/// Nutzt BabylonJS über GA-Ful Visualizer
/// </summary>
public sealed class SimplePolylineRenderer : ISketchRenderer
{
    private static readonly CGaFloat64GeometricSpace5D Cga =
        CGaFloat64GeometricSpace5D.Instance;

    private string _outputPath = string.Empty;

    /// <summary>
    /// Auflösung (Samples pro Segment)
    /// </summary>
    public int SegmentResolution { get; set; } = 16;

    /// <summary>
    /// Pressure-Kreis Skalierung (Pressure 1.0 = PressureScale Meter Radius)
    /// </summary>
    public double PressureScale { get; set; } = 0.02; // 2cm

    public void Initialize()
    {
        Cga.Visualizer.BeginDrawing3D("VR Sketch Prototype", "");
    }

    public void RenderSketch(Sketch sketch)
    {
        foreach (var spline in sketch.Splines)
        {
            RenderSpline(spline);
        }
    }

    public void RenderSpline(ArcSpline spline)
    {
        foreach (var segment in spline.Segments)
        {
            RenderSegment(segment);
        }
    }

    public void RenderSegment(ArcSegment segment)
    {
        // 1. Sample Punkte entlang Arc
        var points = segment.SamplePoints(SegmentResolution);

        // 2. Zeichne Polylinie
        for (int i = 0; i < points.Length - 1; i++)
        {
            var p1 = points[i].DecodeOpnsRound.PositionToVector3D();
            var p2 = points[i + 1].DecodeOpnsRound.PositionToVector3D();

            Cga.Visualizer.DrawLineTo(p1, p2, Color.Blue);
        }

        // 3. Zeichne Pressure-Kreise an Endpunkten
        DrawPressureCircle(
            segment.StartPoint.DecodeOpnsRound.PositionToVector3D(),
            segment.StartPressure
        );

        DrawPressureCircle(
            segment.EndPoint.DecodeOpnsRound.PositionToVector3D(),
            segment.EndPressure
        );
    }

    private void DrawPressureCircle(LinFloat64Vector3D position, double pressure)
    {
        var radius = pressure * PressureScale;

        // Zeichne Kreis als Disc
        var circle = Cga.DefineRealRoundCircle(
            radius: radius,
            position: position,
            direction: LinFloat64Bivector3D.Create(0, 0, 1) // XY-Ebene
        );

        Cga.Visualizer.DrawCircleSurface3D(
            circle,
            Color.FromArgb(128, 255, 100, 0) // Orange, semi-transparent
        );
    }

    public void Clear()
    {
        // TODO: GA-Ful Visualizer hat kein Clear - neu initialisieren?
    }

    public void Finalize(string outputPath)
    {
        _outputPath = outputPath;
        Cga.Visualizer.EndDrawing3D();

        // GA-Ful schreibt HTML automatisch
        Console.WriteLine($"Rendered sketch to: {outputPath}");
    }
}
```

**Tasks (Visualisierung Prototyp A - Priorität 1):**
- [ ] Implementiere `SimplePolylineRenderer`
- [ ] Teste mit GA-Ful BabylonJS Visualizer
- [ ] Erstelle Beispiel-Ausgabe HTML

---

### 3.3 Renderer Option B: Tube/Cylinder

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch/Visualization/TubeRenderer.cs`

```csharp
/// <summary>
/// Renderer mit 3D-Röhre (Tube/Cylinder)
/// </summary>
public sealed class TubeRenderer : ISketchRenderer
{
    // TODO: Implementierung nach Prototyp A
    // Nutzt BabylonJS Tube/Cylinder Meshes
    // Pressure → Tube Radius
}
```

**Tasks (Visualisierung Prototyp B - Priorität 2):**
- [ ] Implementiere nach Simple Renderer funktioniert
- [ ] Tube-Mesh-Generierung mit variablem Radius

---

### 3.4 Renderer Option C: Ribbon mit Pressure-Breite

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch/Visualization/RibbonRenderer.cs`

```csharp
/// <summary>
/// Renderer mit Ribbon/Band (wie Pinselstrich)
/// </summary>
public sealed class RibbonRenderer : ISketchRenderer
{
    // TODO: Implementierung nach Tube Renderer
    // Ribbon mit Breite ∝ Pressure
}
```

**Tasks (Visualisierung Prototyp C - Priorität 3):**
- [ ] Implementiere nach Tube Renderer
- [ ] Ribbon-Mesh-Generierung

---

### 3.5 Renderer Option D: Volumetrisches Mesh

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch/Visualization/VolumetricMeshRenderer.cs`

```csharp
/// <summary>
/// Renderer mit vollständigem 3D-Körper
/// </summary>
public sealed class VolumetricMeshRenderer : ISketchRenderer
{
    // TODO: Implementierung für Zukunft
    // Für Boolean-Operationen etc.
}
```

**Tasks (Visualisierung Prototyp D - Priorität 4/Future):**
- [ ] Später implementieren wenn nötig

---

## Phase 4: VVVV VL Integration Vorbereitung

### 4.1 Node-freundliche Architektur

**Design-Prinzipien für VL Node-Set:**

1. **Immutable Data Structures** wo möglich
   ```csharp
   // Statt:
   public void AddSpline(ArcSpline spline) { }

   // Besser für VL:
   public Sketch WithSpline(ArcSpline spline) => ...;
   ```

2. **Klare Input/Output Separation**
   ```csharp
   // Jede Klasse sollte klare "Ports" haben
   [Input] public ArcSpline Input { get; set; }
   [Output] public ArcSpline Output => Transform(Input);
   ```

3. **Observable/Reactive wo sinnvoll**
   ```csharp
   public IObservable<Sketch> SketchUpdates { get; }
   ```

**Tasks:**
- [ ] Review alle Klassen auf VL-Kompatibilität
- [ ] Refactor zu Immutability wo sinnvoll
- [ ] Dokumentiere Input/Output Ports

---

### 4.2 Node Categories

**Geplante VL Node-Kategorien:**

```
VRSketch/
├── Input/
│   ├── VRControllerSample (Create)
│   ├── VRControllerStream (Observable)
│   └── SimulateController (für Testing)
├── Geometry/
│   ├── ArcSegment (Create, Sample, Query)
│   ├── ArcSpline (Create, Sample, Query)
│   ├── Sketch (Create, Add, Query)
│   └── ArcSplineBuilder (Begin, Update, End)
├── Manipulation/
│   ├── ConformalManipulator
│   │   ├── Translation
│   │   ├── Rotation
│   │   ├── Scaling
│   │   └── Compose
│   ├── RotorFieldManipulator
│   │   └── WithGuideSpline
│   ├── IKChainManipulator
│   │   ├── CreateChain
│   │   ├── SetTarget
│   │   └── SolveIK
│   └── ManipulatorStack
│       ├── Add
│       └── Remove
├── Visualization/
│   ├── SimplePolylineRenderer
│   ├── TubeRenderer
│   └── RibbonRenderer
└── Utils/
    ├── Thresholds (Configure)
    └── Export (HTML, OBJ, etc.)
```

**Tasks:**
- [ ] Dokumentiere Node-Kategorien
- [ ] Erstelle Node-Attribute/Metadata
- [ ] Erstelle VL Help Patches (später)

---

## Phase 5: Testing & Prototyping

### 5.1 Unit Tests

**Datei:** `GeometricAlgebraFulcrumLib.Applications.VRSketch.Tests/`

**Test-Kategorien:**
- [ ] `VRControllerSampleTests` - Input Model
- [ ] `ArcSegmentTests` - Arc Geometrie
- [ ] `ArcSplineBuilderTests` - Fitting-Algorithmus
- [ ] `ConformalManipulatorTests` - Konforme Transformationen
- [ ] `RotorFieldManipulatorTests` - Guide-Spline Deformation
- [ ] `IKChainManipulatorTests` - IK-Solver
- [ ] `ManipulatorStackTests` - Verkettung

---

### 5.2 Integration Tests

- [ ] Vollständiger Workflow: VR Input → Arc Fitting → Rendering
- [ ] Manipulator-Stack mit allen 3 Typen
- [ ] Replay-Funktionalität
- [ ] Performance-Tests (sollte Real-time sein)

---

### 5.3 Prototyp-Szenarien

**Prototyp 1: Basic Drawing**
- [ ] Simuliere VR Controller Input (einfache Linie)
- [ ] Fitte Arc-Spline
- [ ] Rendere als Polylinie + Pressure-Kreise
- [ ] Export HTML, visuell verifizieren

**Prototyp 2: Conformal Manipulation**
- [ ] Zeichne einfachen Sketch
- [ ] Wende Translation an → verifiziere
- [ ] Wende Rotation an → verifiziere
- [ ] Wende Scaling an → verifiziere

**Prototyp 3: Guide-Spline Deformation**
- [ ] Zeichne geraden Sketch
- [ ] Zeichne gebogenen Guide-Spline
- [ ] Deformiere Sketch → verifiziere Biegung

**Prototyp 4: IK Manipulation**
- [ ] Zeichne Sketch
- [ ] Erstelle IK-Chain
- [ ] Setze Target → verifiziere Deformation

---

## Phase 6: Optimierung & Verfeinerung

### 6.1 Performance-Optimierungen

- [ ] Profiling des Arc-Fitting (sollte <1ms pro Frame sein)
- [ ] Profiling der Manipulatoren
- [ ] Spatial Hashing für Closest-Point-Search
- [ ] Parallele Verarbeitung wo sinnvoll
- [ ] Cache-Strategien

### 6.2 Mathematische Verfeinerung

- [ ] **KRITISCH:** Ganja.js Rotor-Berechnung korrekt in CGA übersetzen
- [ ] Arc-Length Parametrisierung optimieren
- [ ] Numerische Stabilität testen
- [ ] Edge-Cases behandeln (z.B. fast-gerade Arcs)

### 6.3 Code-Generierung (MetaProgramming)

- [ ] Nutze GA-Ful MetaContext für optimierten Arc-Fitting Code
- [ ] Generiere spezialisierte Rotor-Berechnungen
- [ ] CSE für Manipulator-Verkettungen

---

## Phase 7: Dokumentation & Finalisierung

### 7.1 Code-Dokumentation

- [ ] XML-Kommentare für alle public APIs
- [ ] Mathematische Grundlagen dokumentieren
- [ ] Architektur-Diagramme
- [ ] Usage-Beispiele

### 7.2 VL-spezifische Dokumentation

- [ ] Node-Set Dokumentation
- [ ] Help-Patches erstellen
- [ ] Tutorial-Patches
- [ ] Beispiel-Projekte

---

## Kritische offene Fragen (für Architekt/DU)

### Mathematik:

1. **Ganja.js → CGA Übersetzung:**
   ```javascript
   // 2D PGA (Ganja.js):
   var L1 = ((R[i-1]??(p[0]*c0.Conjugate)).Log() & ~p[i]);
   var L2 = ((p[i] + p[i+1]) | (p[i] & p[i+1]));
   R[i] = (L2 * L1).Normalized;
   ```

   **Frage:** Wie übersetzt sich das exakt nach 3D CGA?
   - Was ist das Äquivalent von PGA Line in CGA?
   - Wie konstruiert man L1 (Linie durch Zentrum und Punkt)?
   - Wie konstruiert man L2 (Mittelsenkrechte in 3D)?

2. **Rotor-Transformation unter Versor:**
   ```csharp
   // Ist das korrekt für Rotor-Transformation?
   R' = V * R * Ṽ  // Konjugation
   ```

   **Frage:** Verifizieren dass Rotoren so transformiert werden?

3. **Circle-Transformation unter Versor:**
   - Wie transformiert man CGA Round (Circle) korrekt?
   - Radius-Berechnung bei Inversionen?

### Architektur:

4. **Immutability vs. Performance:**
   - VL bevorzugt Immutability (functional style)
   - Aber Real-time VR braucht Performance
   - **Frage:** Trade-off? Hybrid-Ansatz?

5. **Original-Spine Extraction:**
   - Wie extrahiert man automatisch "zentrale Kurve" aus Sketch?
   - Schwerpunkt-Linie? Haupt-Komponente? User-definiert?

---

## Projektstruktur (Vorschlag)

```
GeometricAlgebraFulcrumLib/
├── GeometricAlgebraFulcrumLib.Applications.VRSketch/
│   ├── Input/
│   │   └── VRControllerSample.cs
│   ├── Geometry/
│   │   ├── ArcSegment.cs
│   │   ├── ArcSplineBuilder.cs
│   │   ├── ArcSpline.cs
│   │   └── Sketch.cs
│   ├── Manipulation/
│   │   ├── ISketchManipulator.cs
│   │   ├── ManipulatorStack.cs
│   │   ├── ConformalManipulator.cs
│   │   ├── RotorFieldManipulator.cs
│   │   └── IKChainManipulator.cs
│   ├── Visualization/
│   │   ├── ISketchRenderer.cs
│   │   ├── SimplePolylineRenderer.cs
│   │   ├── TubeRenderer.cs
│   │   └── RibbonRenderer.cs
│   └── Utils/
│       └── ArcSplineThresholds.cs
├── GeometricAlgebraFulcrumLib.Applications.VRSketch.Tests/
│   ├── GeometryTests/
│   ├── ManipulationTests/
│   └── IntegrationTests/
└── GeometricAlgebraFulcrumLib.Applications.VRSketch.Prototypes/
    ├── Prototype1_BasicDrawing.cs
    ├── Prototype2_ConformalManipulation.cs
    ├── Prototype3_GuideSplineDeformation.cs
    └── Prototype4_IKManipulation.cs
```

---

## Nächste Schritte (Priorität)

### Sofort:

1. **Mathematische Klärung:**
   - [ ] Ganja.js Rotor-Berechnung in CGA übersetzen
   - [ ] Mit einfachem 2D-Beispiel verifizieren
   - [ ] Auf 3D erweitern

2. **Prototyp 1 starten:**
   - [ ] `VRControllerSample` implementieren
   - [ ] `ArcSegment` implementieren
   - [ ] Einfaches Test-Szenario (3 Punkte → 1 Arc)

3. **Rendering-Test:**
   - [ ] `SimplePolylineRenderer` implementieren
   - [ ] Ein Arc visuell im Browser verifizieren

### Dann:

4. **Arc-Spline Builder:**
   - [ ] Rotor-Berechnung implementieren
   - [ ] Adaptive Threshold-System
   - [ ] Live-Update-Logic

5. **Prototyp B (Conformal):**
   - [ ] Translation/Rotation Factory-Methoden
   - [ ] Test mit einfachem Sketch

---

## Erfolgs-Kriterien

### Prototyp-Phase 1 (Basic):
- ✅ Arc-Spline kann aus VR-Input gefittet werden
- ✅ Live-Update funktioniert (<16ms pro Frame)
- ✅ Adaptive Thresholds produzieren gute Reduktion
- ✅ Rendering im Browser sichtbar

### Prototyp-Phase 2 (Manipulation B):
- ✅ Konforme Transformationen funktionieren
- ✅ Kreise bleiben Kreise
- ✅ Verkettung mehrerer Transformationen

### Prototyp-Phase 3 (Manipulation E):
- ✅ Guide-Spline deformiert Sketch
- ✅ Deformation ist smooth und intuitiv
- ✅ Performance real-time fähig

### Prototyp-Phase 4 (Manipulation D):
- ✅ IK-Solver konvergiert zuverlässig
- ✅ Skinning produziert smooth Deformation

### Final (VVVV VL):
- ✅ Alle Funktionen als VL Nodes verfügbar
- ✅ Dokumentation und Beispiele
- ✅ Performance-optimiert

---

## Ressourcen & Referenzen

### GA-Ful Klassen (wichtigste):
- `CGaFloat64GeometricSpace5D` - 5D CGA
- `CGaFloat64Round` - Kreise/Sphären
- `CGaFloat64Versor` - Konforme Transformationen
- `CGaFloat64Visualizer` - BabylonJS Rendering
- `MetaContext` - Code-Generierung

### Externe Referenzen:
- Ganja.js Circular Spline: [Original Code]
- FABRIK IK: Aristidou & Lasenby (2011)
- CGA: "Geometric Algebra for Computer Science" - Dorst, Fontijne, Mann

### VVVV VL:
- https://visualprogramming.net
- VL Node Development Guide

---

**Letzte Aktualisierung:** 2025-10-03
**Status:** Planning / Ready for Implementation
**Nächster Meilenstein:** Prototyp 1 - Basic Arc Fitting
