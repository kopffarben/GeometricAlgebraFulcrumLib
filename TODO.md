# TODO – Arc-Splines & VR-Sketch System in 3D CGA (GA-FuL, C#)

**Projekt:** VR-basiertes Zeichnungssystem mit echtzeitfähigem Arc-Spline Fitting
**Framework:** GeometricAlgebraFulcrumLib (GA-FuL)
**Algebra:** 3D Conformal Geometric Algebra (CGA)
**Sprache:** C# (.NET 8.0)
**Entwicklung:** Hobby-Projekt mit Claude Code Unterstützung
**Stand:** Oktober 2025

---

## 🎯 Vision & Ziele

### Endprodukt
Ein **VR-Sketch-System** das Benutzereingaben (VR-Controller) in Echtzeit zu Arc-Splines (Kreisbögen + Geraden) vereinfacht, mit **minimaler Segmentanzahl** bei kontrollierbarem Fehler. Gezeichnete Bögen können mit **Conformal-Manipulatoren** transformiert werden.

### Kern-Features
- ✅ **Echtzeitfähiges Arc-Fitting** - PushPoint → optimierte Segmente
- ✅ **Hybrid-Algorithmus** (Phase 1: PCA + CGA Circle-Fit, Phase 2: Pure CGA)
- ✅ **VR-Integration** - Controller Input (Position, Orientation, Pressure)
- ✅ **Manipulator-System** - Conformal, Rotor-Field, IK-Chain
- ✅ **BabylonJS Visualization** - Web-basiertes Rendering
- ⚠️ **VVVV VL Nodes** (Optional, Phase 2.5+)

### Design-Prinzipien
1. **Separation of Concerns** - Core Library (wiederverwendbar) vs. VR Application
2. **CGA-First** - Maximale Nutzung von Conformal GA (koordinatenfrei wo möglich)
3. **Incremental Development** - Kleine, testbare Schritte
4. **Claude Code Optimized** - Klare Tasks, gut dokumentiert

---

## 📐 Architektur-Entscheidungen

### 1. Fitting-Algorithmus: **Hybrid 2-Phasen-Ansatz**

**Phase 1 (MVP):** Euklid-PCA + CGA Circle-Fit
```
3D Punkte
  → PCA (Euklid, 3×3 Eigenzerlegung) → Beste Ebene {μ, n, u, v}
  → Projektion auf Ebene (2D)
  → CGA Circle-Fit (Dorst-Methode statt Pratt/Taubin)
  → 3D Lift + Motor-Konstruktion (Pure CGA)
```

**Phase 2 (Research/Optional):** Pure CGA (Dorst 2014)
```
3D Punkte
  → Encode to CGA
  → k-Sphere Fit (Eigenproblem in CGA, 5×5)
  → Extract Motor
```

**Begründung Phase 1:**
- PCA ist extrem schnell & robust (GA-FuL hat Jacobi-Eigenzerlegung)
- CGA ab Circle-Level = philosophisch "rein genug"
- Praktisch & sofort umsetzbar

### 2. Projektstruktur: **Zwei-Schichten (Option C)**

```
GeometricAlgebraFulcrumLib/
├── Modeling/
│   └── Geometry/
│       └── ArcSplines/                           ← LAYER 1: CORE
│           ├── Fitting/
│           │   ├── IArcSplineSolver.cs
│           │   ├── HybridCircleFitter.cs        ← Phase 1 Algorithmus
│           │   ├── PureCGACircleFitter.cs       ← Phase 2 (optional)
│           │   ├── OnlinePCA.cs
│           │   └── CircleFitCGA.cs
│           ├── Geometry/
│           │   ├── ArcSegment.cs
│           │   ├── ArcSpline.cs
│           │   └── ProjectionUtils.cs
│           ├── PostProcessing/
│           │   ├── BiarcSmoother.cs             ← G¹-Glättung (optional)
│           │   └── MergePass.cs                 ← Segmentzahl reduzieren
│           └── API/
│               ├── ArcSplineFitter.cs           ← Public API
│               └── FitSettings.cs
│
└── Applications/
    └── VRSketch/                                 ← LAYER 2: VR APPLICATION
        ├── Input/
        │   ├── VRControllerSample.cs
        │   └── VRArcSplineBuilder.cs            ← Wrapper um ArcSplineFitter
        ├── Manipulation/
        │   ├── IManipulator.cs
        │   ├── ConformalManipulator.cs
        │   ├── RotorFieldManipulator.cs
        │   └── IKChainManipulator.cs
        ├── Visualization/
        │   └── BabylonJs/
        │       ├── ArcSplineRenderer.cs
        │       ├── ManipulatorRenderer.cs
        │       └── SceneBuilder.cs
        └── VVVV/                                 ← Phase 2.5+ (nice-to-have)
            └── Nodes/
```

### 3. Phasen & Prioritäten

**Phase 1: Core MVP** (~3-4 Wochen mit Claude Code)
- Layer 1 (Core) komplett funktionsfähig
- Keine VR, keine Manipulatoren, keine Visualisierung
- **G¹-Glättung optional am Ende** (wenn Zeit/Lust)

**Phase 1.5: VR Integration** (~2 Wochen)
- Layer 2 (VR) Basis-Funktionalität
- Zeichnen in VR möglich
- BabylonJS Renderer (nur Bögen)

**Phase 2: Interaktion** (~3-4 Wochen)
- Erster Manipulator (Conformal)
- G¹-Glättung falls nicht in Phase 1

**Phase 2.5+: Erweiterte Features** (nach Bedarf)
- Weitere Manipulatoren
- Pure CGA Fitter (Phase 2 Algorithmus)
- VVVV VL Integration
- Performance-Optimierung

---

## 🔬 Mathematische Grundlagen (CGA Essentials)

### CGA-Modell (ℝ^{4,1})

**Basis-Vektoren:**
- Euklidischer Unterraum: `e₁, e₂, e₃`
- Null-Vektoren: `n_o` (origin), `n_∞` (infinity)
- Metrik: `n_o · n_o = 0`, `n_∞ · n_∞ = 0`, `n_o · n_∞ = -1`

**Point Encoding (Dorst-Konvention):**
```
up(x) = n_o + x + ½‖x‖² n_∞
down(X) = [decode via GA-FuL or Dorst formula]
```

**Primitives:**
```csharp
// Linie (entarteter Kreis durch n_∞)
Line = up(A) ∧ up(B) ∧ n_∞

// Kreis (3 Punkte)
Circle = up(A) ∧ up(B) ∧ up(C)
```

**Versoren/Motoren:**
```csharp
// Translator
T(t) = exp(-½ n_∞ ∧ t)

// Rotor in Ebene {u,v} um Winkel θ
R(θ) = exp(-½ θ P̂)  wobei P̂ = normalize(u ∧ v)

// Motor: Rotation um Punkt C
M = T(C) · R(θ) · T(-C)

// Anwendung: Sandwich
X' = M X M̃
```

**Orbit-Kurven:**
- Kreisbogen = Bahn eines Punktes unter konstantem Rotor
- `x(t) = exp(-½Bt) x(0) exp(½Bt)` für t ∈ [0,1]

---

## 📦 Phase 1: Core MVP (LAYER 1)

**Ziel:** Funktionsfähiger, getesteter Arc-Spline Fitter ohne VR/GUI

### 1.1 Projekt-Setup

**Tasks:**
- [ ] Ordner erstellen: `GeometricAlgebraFulcrumLib/Modeling/Geometry/ArcSplines/`
- [ ] Projekt-Referenzen setzen (zu `GeometricAlgebraFulcrumLib.Algebra`)
- [ ] Test-Projekt: `GeometricAlgebraFulcrumLib.Modeling.Tests/Geometry/ArcSplines/`
- [ ] .csproj konfigurieren:
  ```xml
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  </PropertyGroup>
  ```

### 1.2 Datenstrukturen

**`FitSettings.cs`**
```csharp
public sealed class FitSettings
{
    public double EpsilonRadial { get; init; } = 1e-3;     // max radialer Fehler [m]
    public double EpsilonPlanar { get; init; } = 2e-3;     // max Abstand zur PCA-Ebene [m]
    public double EpsilonAngle { get; init; } = 0.087;     // ~5° Tangenten-Winkel [rad]
    public double EpsilonLine { get; init; } = 1e-3;       // max Linienabstand [m]
    public double RMax { get; init; } = 1e6;               // "praktisch gerade" Schwelle
    public double MinAngleRad { get; init; } = 0.087;      // min. Bogenwinkel [rad] (~5°)
    public double MinChordLength { get; init; } = 1e-2;    // min. Sehnenlänge [m]
    public int MinPointsSegment { get; init; } = 3;        // min. Punkte für Segment
    public int MaxWindow { get; init; } = 256;             // Ringpuffer-Größe
    public int AllowOutliers { get; init; } = 1;           // erlaubte Ausreißer in Folge
    public int RefitStride { get; init; } = 32;            // alle K Punkte Voll-Refit
    public bool PreferArcs { get; init; } = true;          // Bögen bevorzugen
}
```

**`ArcSegment.cs`**
```csharp
public enum SegmentType { Line, Arc }

public sealed class ArcSegment
{
    public SegmentType Type { get; init; }

    // Endpunkte (beide Typen)
    public Vector3 P0 { get; init; }
    public Vector3 P1 { get; init; }

    // Linie
    public Vector3 Direction { get; init; }  // normiert

    // Bogen
    public Vector3 Center { get; init; }
    public Vector3 PlaneNormal { get; init; }  // normiert
    public double Radius { get; init; }
    public double Theta { get; init; }         // |Winkel| [rad]
    public int Sign { get; init; }             // ±1 Drehsinn

    // CGA (optional, für VR-Layer)
    public CGaFloat64Round? Circle { get; init; }

    // Timestamps (optional, für VR)
    public double StartTime { get; init; }
    public double EndTime { get; init; }

    // Computed properties
    public double ArcLength => Type == SegmentType.Arc
        ? Radius * Theta
        : (P1 - P0).Length();
}
```

**`ArcSpline.cs`**
```csharp
public sealed class ArcSpline
{
    public List<ArcSegment> Segments { get; } = new();

    public double TotalLength => Segments.Sum(s => s.ArcLength);
    public int SegmentCount => Segments.Count;
    public int ArcCount => Segments.Count(s => s.Type == SegmentType.Arc);
    public int LineCount => Segments.Count(s => s.Type == SegmentType.Line);
}
```

### 1.3 Online PCA (Planarität)

**`OnlinePCA.cs`**

**Quelle:** Welford-Algorithmus für inkrementelle Mittelwert/Kovarianz

```csharp
public sealed class OnlinePCA
{
    private int _n;
    private Vector3 _mean;
    private Matrix3x3 _covariance;  // Symmetric

    public void AddPoint(Vector3 p)
    {
        _n++;
        var delta = p - _mean;
        _mean += delta / _n;
        var delta2 = p - _mean;
        _covariance += OuterProduct(delta, delta2);
    }

    public (Vector3 center, Vector3 normal, Vector3 u, Vector3 v) GetPlane()
    {
        // Eigenzerlegung (nutze GA-FuL Jacobi)
        var (eigenvalues, eigenvectors) = JacobiEigendecompose(_covariance);

        // Normal = kleinster Eigenvektor
        var normal = eigenvectors[indexOfSmallest(eigenvalues)];

        // {u,v} = die zwei größeren
        var u = eigenvectors[indexOfLargest(eigenvalues)];
        var v = eigenvectors[indexOfMiddle(eigenvalues)];

        return (_mean, normal, u, v);
    }

    public double PlanarError(ReadOnlySpan<Vector3> points, Vector3 normal, Vector3 center)
    {
        return points.Max(p => Math.Abs(Vector3.Dot(p - center, normal)));
    }
}
```

**GA-FuL Integration:**
- Nutze: `GeometricAlgebraFulcrumLib.Applications.Symbolic/EllipseFitting/JacobiSymmetricEigenDecomposer.cs`
- Adaptiere für 3×3 Matrix

### 1.4 Circle-Fit in CGA

**`CircleFitCGA.cs`**

**Methode 1: 3-Punkt (exakt)**
```csharp
public static CGaFloat64Round FitCircleThreePoints(
    CGaFloat64GeometricSpace cga,
    Vector3 p1, Vector3 p2, Vector3 p3)
{
    // Direkt via GA-FuL vorhanden!
    return cga.DefineRealRoundCircleFromPoints(p1, p2, p3);
}
```

**Methode 2: n-Punkt (Least-Squares, Phase 1 - Simpel)**
```csharp
public static (Vector2 center, double radius) FitCircle2D(
    ReadOnlySpan<Vector2> points)
{
    // Initial: 3-Punkt-Kreis der letzten 3 Punkte
    var (c0, r0) = CircumcircleFromThreePoints(
        points[^3], points[^2], points[^1]
    );

    // Refinement: Gauss-Newton auf algebraische Distanz
    // (vereinfacht gegenüber Pratt/Taubin, aber für Phase 1 ausreichend)

    for (int iter = 0; iter < 5; iter++)
    {
        // Linearisiere um (c0, r0)
        // Löse Normalgleichungen
        // Update c0, r0
    }

    return (c0, r0);
}
```

**Methode 3: Pure CGA Least-Squares (Phase 2 - Dorst 2014)**
```csharp
// TODO Phase 2: Implementierung nach Dorst (2014)
// - Bilde Moment-Matrix in CGA
// - Eigenzerlegung (5×5)
// - Rekonstruktion aus Eigenvektoren
```

### 1.5 Hybrid Circle Fitter

**`HybridCircleFitter.cs`**

```csharp
public sealed class HybridCircleFitter : IArcSplineSolver
{
    private readonly OnlinePCA _pca = new();
    private readonly FitSettings _settings;

    public ArcSegment? TryFitSegment(ReadOnlySpan<Vector3> points)
    {
        if (points.Length < _settings.MinPointsSegment)
            return null;

        // 1. PCA → Ebene
        _pca.Reset();
        foreach (var p in points)
            _pca.AddPoint(p);

        var (center, normal, u, v) = _pca.GetPlane();

        // 2. Planarität prüfen
        var planarError = _pca.PlanarError(points, normal, center);
        if (planarError > _settings.EpsilonPlanar)
            return null;  // Nicht planar genug

        // 3. Projektion auf Ebene
        var points2D = ProjectToPlane(points, center, u, v);

        // 4. Circle-Fit in 2D
        var (c2D, radius) = CircleFitCGA.FitCircle2D(points2D);

        // 5. Prüfe ob Kreis oder Linie
        if (radius > _settings.RMax || !IsValidArc(points2D, c2D, radius))
        {
            return FitLine(points);  // Fallback zu Linie
        }

        // 6. Lift zu 3D
        var center3D = center + c2D.X * u + c2D.Y * v;

        // 7. Compute Arc-Winkel
        var theta = ComputeArcAngle(points2D, c2D);
        if (Math.Abs(theta) < _settings.MinAngleRad)
            return FitLine(points);

        // 8. CGA Circle (optional für VR-Layer)
        var cgaCircle = CreateCGACircle(center3D, radius, normal);

        return new ArcSegment
        {
            Type = SegmentType.Arc,
            P0 = points[0],
            P1 = points[^1],
            Center = center3D,
            PlaneNormal = normal,
            Radius = radius,
            Theta = Math.Abs(theta),
            Sign = Math.Sign(theta),
            Circle = cgaCircle
        };
    }

    private ArcSegment FitLine(ReadOnlySpan<Vector3> points)
    {
        // Richtung = normalize(P1 - P0)
        var dir = Vector3.Normalize(points[^1] - points[0]);

        return new ArcSegment
        {
            Type = SegmentType.Line,
            P0 = points[0],
            P1 = points[^1],
            Direction = dir
        };
    }
}
```

### 1.6 Arc-Spline Fitter (Public API)

**`ArcSplineFitter.cs`**

```csharp
public sealed class ArcSplineFitter
{
    private readonly IArcSplineSolver _solver;
    private readonly FitSettings _settings;
    private readonly RingBuffer<Vector3> _window;
    private readonly Queue<ArcSegment> _closedSegments = new();

    public ArcSplineFitter(FitSettings settings, IArcSplineSolver? solver = null)
    {
        _settings = settings;
        _solver = solver ?? new HybridCircleFitter(settings);
        _window = new RingBuffer<Vector3>(settings.MaxWindow);
    }

    public void PushPoint(Vector3 point, double timestamp = 0)
    {
        _window.Add(point);

        if (_window.Count < _settings.MinPointsSegment)
            return;

        // Versuche Segment zu fitten
        var segment = _solver.TryFitSegment(_window.AsSpan());

        if (segment == null)
        {
            // Fehler zu groß → finalisiere bisheriges Segment
            FinalizeCurrentSegment();
            return;
        }

        // Update cached segment (noch nicht finalisiert)
        _currentSegmentCandidate = segment;
    }

    public bool HasClosedSegment => _closedSegments.Count > 0;

    public ArcSegment PopClosedSegment()
    {
        if (!HasClosedSegment)
            throw new InvalidOperationException("No closed segments available");

        return _closedSegments.Dequeue();
    }

    public void Flush()
    {
        if (_currentSegmentCandidate != null)
        {
            _closedSegments.Enqueue(_currentSegmentCandidate);
            _currentSegmentCandidate = null;
        }
    }

    public ArcSpline Snapshot()
    {
        var spline = new ArcSpline();
        spline.Segments.AddRange(_closedSegments);
        if (_currentSegmentCandidate != null)
            spline.Segments.Add(_currentSegmentCandidate);
        return spline;
    }

    private void FinalizeCurrentSegment()
    {
        if (_currentSegmentCandidate != null)
        {
            _closedSegments.Enqueue(_currentSegmentCandidate);
            _currentSegmentCandidate = null;
        }

        // Restart mit Überlappung (letzter Punkt)
        var lastPoint = _window[^1];
        _window.Clear();
        _window.Add(lastPoint);
    }
}
```

### 1.7 Unit-Tests

**`ArcSplineFitterTests.cs`**

```csharp
[Fact]
public void PerfectCircle_ProducesOneArcSegment()
{
    var fitter = new ArcSplineFitter(new FitSettings
    {
        EpsilonRadial = 1e-6,
        EpsilonPlanar = 1e-6
    });

    // Perfekter Kreis: Radius=1, 100 Punkte
    for (int i = 0; i < 100; i++)
    {
        var angle = i * Math.PI * 2 / 100;
        var p = new Vector3(Math.Cos(angle), Math.Sin(angle), 0);
        fitter.PushPoint(p);
    }

    fitter.Flush();
    var spline = fitter.Snapshot();

    Assert.Equal(1, spline.SegmentCount);
    Assert.Equal(SegmentType.Arc, spline.Segments[0].Type);
    Assert.InRange(spline.Segments[0].Radius, 0.99, 1.01);
}

[Fact]
public void StraightLine_ProducesOneLineSegment()
{
    var fitter = new ArcSplineFitter(new FitSettings());

    for (int i = 0; i < 50; i++)
    {
        fitter.PushPoint(new Vector3(i * 0.1, 0, 0));
    }

    fitter.Flush();
    var spline = fitter.Snapshot();

    Assert.Equal(1, spline.SegmentCount);
    Assert.Equal(SegmentType.Line, spline.Segments[0].Type);
}

[Fact]
public void CircleThenLine_ProducesTwoSegments()
{
    // TODO: Implementiere
}
```

### 1.8 G¹-Glättung (Optional am Ende Phase 1)

**`BiarcSmoother.cs`**

Nur wenn Zeit & Lust am Ende von Phase 1!

```csharp
public sealed class BiarcSmoother
{
    public static ArcSpline SmoothG1(ArcSpline input, FitSettings settings)
    {
        // Für jede Naht zwischen Segmenten:
        // 1. Prüfe ob Tangenten bereits stetig (innerhalb Epsilon)
        // 2. Falls nicht: konstruiere Biarc nach Meek & Walton (1992)
        // 3. Validiere neue Bögen (Fehler, minAngle)
        // 4. Falls OK: ersetze Nahtstelle

        // TODO: Implementierung nach Meek & Walton Paper
        throw new NotImplementedException("Phase 1 optional");
    }
}
```

---

## 🎮 Phase 1.5: VR Integration (LAYER 2)

**Ziel:** Zeichnen in VR möglich, erste visuelle Demo

### 2.1 VR Controller Input

**`VRControllerSample.cs`**

```csharp
public readonly record struct VRControllerSample
{
    public Vector3 Position { get; init; }
    public Quaternion Orientation { get; init; }
    public double Pressure { get; init; }      // 0..1 (Trigger)
    public double Timestamp { get; init; }
    public bool IsDrawing { get; init; }       // Trigger gedrückt
}
```

### 2.2 VR Arc-Spline Builder

**`VRArcSplineBuilder.cs`**

```csharp
public sealed class VRArcSplineBuilder
{
    private readonly ArcSplineFitter _coreFitter;
    private readonly FitSettings _baseSettings;
    private readonly List<ArcSegment> _frozenSegments = new();

    public IReadOnlyList<ArcSegment> FrozenSegments => _frozenSegments;

    public VRArcSplineBuilder(FitSettings settings)
    {
        _baseSettings = settings;
        _coreFitter = new ArcSplineFitter(settings);
    }

    public void BeginDrawing()
    {
        _coreFitter.Reset();
    }

    public void UpdateDrawing(VRControllerSample sample)
    {
        if (!sample.IsDrawing)
            return;

        // VR-spezifisch: Pressure → Threshold Anpassung (optional)
        var dynamicSettings = _baseSettings with
        {
            EpsilonRadial = _baseSettings.EpsilonRadial * (0.5 + sample.Pressure * 0.5)
        };

        _coreFitter.PushPoint(sample.Position, sample.Timestamp);

        // Frozen Segments aktualisieren
        while (_coreFitter.HasClosedSegment)
        {
            _frozenSegments.Add(_coreFitter.PopClosedSegment());
        }
    }

    public void EndDrawing()
    {
        _coreFitter.Flush();

        while (_coreFitter.HasClosedSegment)
        {
            _frozenSegments.Add(_coreFitter.PopClosedSegment());
        }
    }

    public ArcSegment? LiveSegment => _coreFitter.Snapshot().Segments.LastOrDefault();
}
```

### 2.3 BabylonJS Renderer (Basic)

**`ArcSplineRenderer.cs`**

```csharp
public sealed class ArcSplineRenderer
{
    public void RenderSegment(ArcSegment segment, Scene scene)
    {
        if (segment.Type == SegmentType.Line)
        {
            RenderLine(segment.P0, segment.P1, scene);
        }
        else
        {
            RenderArc(segment, scene);
        }
    }

    private void RenderLine(Vector3 p0, Vector3 p1, Scene scene)
    {
        // BabylonJS Line
        var line = Lines.CreateLines("line", new[]
        {
            new Vector3JS(p0.X, p0.Y, p0.Z),
            new Vector3JS(p1.X, p1.Y, p1.Z)
        }, scene);
        line.Color = new Color3(1, 1, 1);
    }

    private void RenderArc(ArcSegment arc, Scene scene)
    {
        // Sample Arc zu Polyline
        var samples = SampleArc(arc, numSamples: 32);

        var line = Lines.CreateLines("arc", samples.Select(p =>
            new Vector3JS(p.X, p.Y, p.Z)
        ).ToArray(), scene);

        line.Color = new Color3(0, 1, 1);  // Cyan für Bögen
    }

    private Vector3[] SampleArc(ArcSegment arc, int numSamples)
    {
        var samples = new Vector3[numSamples];

        for (int i = 0; i < numSamples; i++)
        {
            var t = i / (double)(numSamples - 1);
            var angle = arc.Sign * t * arc.Theta;

            // Rotation um Zentrum
            var relPos = arc.P0 - arc.Center;
            var rotated = RotateAroundAxis(relPos, arc.PlaneNormal, angle);
            samples[i] = arc.Center + rotated;
        }

        return samples;
    }
}
```

---

## 🎨 Phase 2: Interaktion & Manipulatoren

**Ziel:** Gezeichnete Bögen können gegriffen und transformiert werden

### 3.1 Manipulator-Interface

**`IManipulator.cs`**

```csharp
public interface IManipulator
{
    bool CanGrab(ArcSegment segment, Vector3 grabPoint);
    void StartGrab(ArcSegment segment, Vector3 grabPoint);
    ArcSegment UpdateGrab(Vector3 currentPoint);
    void EndGrab();
}
```

### 3.2 Conformal Manipulator

**`ConformalManipulator.cs`**

```csharp
public sealed class ConformalManipulator : IManipulator
{
    private ArcSegment? _originalSegment;
    private Vector3 _grabOffset;

    public bool CanGrab(ArcSegment segment, Vector3 grabPoint)
    {
        // Prüfe ob grabPoint nahe am Segment ist
        var dist = DistanceToSegment(segment, grabPoint);
        return dist < GrabThreshold;
    }

    public void StartGrab(ArcSegment segment, Vector3 grabPoint)
    {
        _originalSegment = segment;

        // Berechne Offset: grabPoint relativ zu segment.Center (für Arc)
        _grabOffset = segment.Type == SegmentType.Arc
            ? grabPoint - segment.Center
            : Vector3.Zero;
    }

    public ArcSegment UpdateGrab(Vector3 currentPoint)
    {
        if (_originalSegment == null)
            throw new InvalidOperationException();

        if (_originalSegment.Type == SegmentType.Arc)
        {
            return TransformArcConformal(_originalSegment, currentPoint);
        }
        else
        {
            return TransformLineConformal(_originalSegment, currentPoint);
        }
    }

    private ArcSegment TransformArcConformal(ArcSegment arc, Vector3 newGrabPoint)
    {
        // Neue Center-Position
        var newCenter = newGrabPoint - _grabOffset;

        // Conformal Transform: Motor konstruieren
        // Translation von arc.Center → newCenter

        // TODO: Nutze GA-FuL Motor-Utilities
        var cga = CGaFloat64GeometricSpace5D.Instance;
        var translator = cga.CreateTranslator(newCenter - arc.Center);

        // Transformiere P0, P1
        var newP0 = ApplyMotor(translator, arc.P0);
        var newP1 = ApplyMotor(translator, arc.P1);

        return arc with
        {
            P0 = newP0,
            P1 = newP1,
            Center = newCenter
        };
    }
}
```

---

## 📚 Referenzen & GA-FuL Code-Pfade

### Papers (in `references/`)

**Essentiell für Phase 1:**
1. ⭐⭐⭐ Dorst (2016): Construction of 3D Conformal Motions
2. ⭐⭐⭐ Dorst (2018): Least Squares Fitting of Spatial Circles
3. ⭐⭐ Jeon et al. (2024): Reliability-based G¹ Arc Spline

**Für G¹-Glättung:**
4. Meek & Walton (1992): G¹ Arc Splines (siehe TODO_ARC_SPLINE_FIT.md)

**Für Phase 2 (Pure CGA):**
5. ⭐⭐⭐ Dorst (2014): Total Least Squares k-Spheres

### GA-FuL Code-Pfade

**Eigenzerlegung (für PCA):**
```
GeometricAlgebraFulcrumLib.Applications.Symbolic/EllipseFitting/
  - JacobiSymmetricEigenDecomposer.cs (3×3 Jacobi)
```

**CGA Encoding:**
```
GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Float64/Encoding/
  - CGaFloat64OpnsRoundEncoder.cs (Circle from points)
  - CGaFloat64IpnsRoundEncoder.cs (Point encoding)
```

**CGA Circle-Konstruktion:**
```
GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Float64/Elements/
  - CGaFloat64RealRoundComposerUtils.cs
      → DefineRealRoundCircleFromPoints(point1, point2, point3)
```

**Motoren/Versoren:**
```
GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Float64/Operations/
  - (TODO: Finde Translator/Rotor Utils)
```

---

## 🛠️ Implementierungs-Strategie (für Claude Code)

### Prinzipien
1. **Ein Feature pro Session** - z.B. "Implementiere OnlinePCA.cs"
2. **Test-Driven** - Erst Test schreiben, dann Implementation
3. **Inkrementell** - Klein anfangen, dann erweitern
4. **Dokumentiert** - Jede Klasse hat XML-Comments

### Empfohlene Reihenfolge

**Woche 1-2: Foundations**
1. Projekt-Setup
2. Datenstrukturen (FitSettings, ArcSegment, ArcSpline)
3. OnlinePCA (mit Unit-Tests)
4. Projection-Utils

**Woche 3: Circle-Fitting**
5. CircleFitCGA (3-Punkt, dann n-Punkt simpel)
6. HybridCircleFitter
7. Tests mit perfekten Kreisen/Linien

**Woche 4: Integration**
8. ArcSplineFitter (Public API)
9. RingBuffer & Streaming-Logik
10. End-to-End Tests
11. Console-Demo

**Woche 5-6: VR Layer**
12. VRControllerSample
13. VRArcSplineBuilder
14. BabylonJS Renderer
15. Erste VR-Demo

---

## 📋 Checkliste Phase 1 (Core MVP)

### Setup
- [ ] Ordnerstruktur erstellt
- [ ] Projekt-Referenzen gesetzt
- [ ] Test-Projekt angelegt
- [ ] Build erfolgreich (Debug + Release)

### Datenstrukturen
- [ ] `FitSettings.cs` implementiert
- [ ] `ArcSegment.cs` implementiert
- [ ] `ArcSpline.cs` implementiert
- [ ] Unit-Tests für Datenstrukturen

### Core Algorithmen
- [ ] `OnlinePCA.cs` implementiert
- [ ] Jacobi-Eigenzerlegung integriert (aus GA-FuL)
- [ ] Projektion auf Ebene implementiert
- [ ] Tests: PCA auf bekannten Daten

### Circle-Fitting
- [ ] 3-Punkt-Kreis (via GA-FuL)
- [ ] n-Punkt Circle-Fit (2D, simpel)
- [ ] Tests: perfekte Kreise verschiedener Radien
- [ ] Tests: entartete Fälle (kollinear → Linie)

### Integration
- [ ] `HybridCircleFitter.cs` implementiert
- [ ] `ArcSplineFitter.cs` implementiert
- [ ] RingBuffer-Logik
- [ ] Segment-Finalisierung
- [ ] Überlappungs-Politik

### Testing & Validation
- [ ] Test: Perfekter Kreis → 1 Segment
- [ ] Test: Gerade Linie → 1 Segment
- [ ] Test: Kreis + Linie → 2 Segmente
- [ ] Test: Verrauschte Daten
- [ ] Console-Demo funktioniert

### Optional (G¹-Glättung)
- [ ] Biarc-Konstruktion (Meek & Walton)
- [ ] Toggle On/Off
- [ ] Tests mit/ohne G¹

---

## 🎯 Success-Kriterien

**Phase 1 Complete:**
- ✅ Perfekte Kreise werden als 1 Segment erkannt
- ✅ Geraden werden als Line-Segment erkannt
- ✅ Mix aus Kreisen/Geraden wird korrekt segmentiert
- ✅ RMS-Fehler unter EpsilonRadial
- ✅ Segmentzahl ist minimal (für gegebene Thresholds)
- ✅ Alle Unit-Tests laufen durch
- ✅ Console-Demo zeigt Ergebnisse

**Phase 1.5 Complete:**
- ✅ VR-Zeichnen funktioniert
- ✅ Frozen Segments werden gerendert
- ✅ Live-Segment wird visualisiert
- ✅ Performance: >60 FPS in VR

**Phase 2 Complete:**
- ✅ Erster Manipulator funktioniert
- ✅ Grab/Move/Release Interaktion
- ✅ Conformal Transform korrekt

---

## 📖 Weitere Ressourcen

**Siehe auch:**
- `RESEARCH_FINDINGS_CGA_CIRCLE_FITTING.md` - Detaillierte Recherche-Ergebnisse
- `references/README.md` - Übersicht aller Papers
- `TODO_ARC_SPLINE_FIT.md` - Ursprüngliche detaillierte Fitting-Spezifikation (Referenz)
- `TODO.md` - Ursprüngliche VR-Vision (Referenz)

**Online:**
- GA-FuL GitHub: https://github.com/ga-explorer/GeometricAlgebraFulcrumLib
- GA-FuL Paper (2024): https://www.mdpi.com/2227-7390/12/14/2272
- Leo Dorst's Homepage: https://staff.fnwi.uva.nl/l.dorst/

---

**Stand:** Oktober 2025
**Status:** Architecture Defined, Ready for Implementation
**Next Step:** Phase 1 - Woche 1 (Projekt-Setup & Datenstrukturen)
