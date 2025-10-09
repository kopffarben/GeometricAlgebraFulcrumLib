# Arc-Spline Prototype - Direct 3D CGA Approach

## Übersicht

Dieser Prototyp implementiert Arc-Spline Konstruktion **direkt in 3D CGA** (Conformal Geometric Algebra) ohne den Umweg über 2D PGA.

### Hauptunterschiede zum Ganja.js Ansatz:

| Aspekt | Ganja.js (2D PGA) | Dieser Prototyp (3D CGA) |
|--------|-------------------|--------------------------|
| **Dimensionen** | 2D (x, y) | 3D (x, y, z) |
| **Algebra** | PGA (Projective GA) | CGA (Conformal GA) |
| **Punkt-Encoding** | `!(1e0 + x*e1 + y*e2)` | `Eo + vector` |
| **Kreis-Konstruktion** | Rotor aus Linien | Direkt aus Punkten + Ebene |
| **Controller-Input** | Position only | Position + Orientation |
| **Ebenen-Definition** | Implizit (2D) | Explizit via Bivector |

---

## Kern-Konzepte

### 1. Circle-Plane aus Controller (Option B4)

```csharp
// Controller liefert:
// - Position (3D Vektor)
// - Orientation (Quaternion)
// - Pressure (float)

// Berechne Bewegungsrichtung:
var tangent = (currentPos - previousPos).Normalized();

// Extrahiere "Forward" aus Controller-Orientation:
var forward = orientation.RotateVector(E3);

// Projiziere Forward auf Ebene ⊥ tangent:
var normal = (forward - tangent * (forward · tangent)).Normalized();

// Kreisebene = tangent ∧ normal (Bivector):
var planeBivector = tangent.Op(normal);
```

**Mathematik:** Die Kreisebene wird durch zwei Vektoren aufgespannt:
- **Tangente** (Bewegungsrichtung)
- **Normal** (aus Controller-Orientation, projiziert auf ⊥tangent)

Dies gibt dem User volle Kontrolle über die "Twist" der Kurve um die Bewegungsrichtung.

---

### 2. Circle Construction durch 2 Punkte in Ebene

```csharp
public static CGaFloat64Round ConstructCircleArc(
    LinFloat64Vector3D p1,           // Startpunkt
    LinFloat64Vector3D p2,           // Endpunkt
    LinFloat64Bivector3D plane,      // Kreisebene (Bivector)
    LinFloat64Vector3D? prevCenter)  // Optional: vorheriges Zentrum
```

**Algorithmus:**

1. **Chord berechnen:** `chord = p2 - p1`
2. **Chord-Midpoint:** `mid = (p1 + p2) / 2`
3. **Center-Direction:** `centerDir = chordDir × normal` (senkrecht zu Chord, in Ebene)
4. **Radius wählen:**
   - Wenn kein vorheriges Segment: Heuristik `radius = chord_length` (≈120° Arc)
   - Wenn vorheriges Segment: Schätze Radius für C¹-Kontinuität
5. **Center berechnen:** `center = mid + centerDir * offset`
   - `offset = sqrt(radius² - (chord/2)²)`
6. **Kreis konstruieren:** `DefineRealRoundCircle(radius, center, plane)`

**Geometrie:**

```
        center
          ●
         /│\
    r  /  │  \ r
      /   │h  \
     /    │    \
   p1─────●─────p2
      chord/2
```

Wo: `h = sqrt(r² - (chord/2)²)` = Offset vom Midpoint zu Center

---

### 3. Rotor Extraction für Arc-Parametrisierung

```csharp
var rotor = ExtractRotorFromCircle(circle, p1, p2);
```

**Rotor** beschreibt die Rotation entlang des Arc:
- **Rotationsachse:** Normale der Kreisebene
- **Rotationswinkel:** Öffnungswinkel des Arc
- **Rotationszentrum:** Kreiszentrum

**Mathematik in CGA:**

```
Rotor R = e^(B * angle/2)

wobei:
  B = Bivector der Rotationsebene um center
  angle = arccos((p1-center) · (p2-center) / r²)
```

---

### 4. Arc Sampling via Rotor-Orbit

```csharp
Arc(t) = e^(t·log(R)) ⊲ p1

für t ∈ [0, 1]
```

**Implementation:**

```csharp
public static LinFloat64Vector3D SampleArcPoint(
    CGaFloat64Versor rotor,
    LinFloat64Vector3D startPoint,
    double t)
{
    var cgaStart = Cga.Encode.VGa.VectorAsRound(startPoint);
    var logRotor = rotor.Log();                    // Bivector
    var scaledLog = logRotor.GradeMultiply(t);     // t * log(R)
    var interpolatedRotor = scaledLog.Exp();       // e^(t*log(R))
    var rotatedPoint = interpolatedRotor.OmMap(cgaStart);  // R(t) ⊲ p
    return rotatedPoint.DecodeOpnsRound.PositionToVector3D();
}
```

**Eigenschaften:**
- ✅ Gleichmäßige Bewegung entlang Arc (konstante Winkelgeschwindigkeit)
- ✅ Koordinatenfrei (reine CGA-Operationen)
- ✅ Numerisch stabil

---

### 5. C¹-Kontinuität

**Natürliche Kontinuität:** Die Rotor-basierte Konstruktion garantiert **nicht automatisch** C¹-Kontinuität zwischen Segmenten.

**Strategie für Kontinuität:**

1. **C⁰ (Positions-Kontinuität):** Automatisch erfüllt (p[i+1] ist End von Segment i und Start von Segment i+1)

2. **C¹ (Tangentiale Kontinuität):** Erfordert dass Tangentenvektoren an Verbindungspunkt identisch sind.

**Aktueller Ansatz:** `EstimateCenterForContinuity()`
- Verwendet vorheriges Kreiszentrum als "Hint"
- Projiziert vorheriges Zentrum auf mögliche Center-Direction
- Wählt Radius der smooth Übergang ermöglicht

**Verbesserungspotential:**
- Explizite Tangenten-Matching-Constraint
- Least-Squares Fit über mehrere Punkte
- Adaptive Radius-Wahl basierend auf Krümmungsänderung

---

## Verwendung

### Einfaches Beispiel:

```csharp
using GeometricAlgebraFulcrumLib.Applications.VRSketch.Prototypes;

// 1. Definiere Kontrollpunkte
var p1 = LinFloat64Vector3D.Create(0, 0, 0);
var p2 = LinFloat64Vector3D.Create(1, 0.5, 0);
var p3 = LinFloat64Vector3D.Create(2, 0, 0);

// 2. Simuliere Controller-Orientations
var q1 = LinFloat64Quaternion.Create(1, 0, 0, 0); // Identity

// 3. Konstruiere erstes Segment
var tangent = (p2 - p1).ToUnitVector();
var plane = ArcSplinePrototype.CalculateCirclePlane(p2, q1, tangent);
var circle = ArcSplinePrototype.ConstructCircleArc(p1, p2, plane);

// 4. Extrahiere Rotor
var rotor = ArcSplinePrototype.ExtractRotorFromCircle(circle, p1, p2);

// 5. Sample Arc
for (double t = 0; t <= 1.0; t += 0.1)
{
    var point = ArcSplinePrototype.SampleArcPoint(rotor, p1, t);
    Console.WriteLine($"t={t:F1}: {point}");
}

// 6. Konstruiere nächstes Segment (mit Kontinuität)
var tangent2 = (p3 - p2).ToUnitVector();
var plane2 = ArcSplinePrototype.CalculateCirclePlane(p3, q1, tangent2);
var circle2 = ArcSplinePrototype.ConstructCircleArc(
    p2, p3, plane2,
    previousCircleCenter: circle.CenterToVector3D()  // Für Kontinuität
);
```

### Tests ausführen:

```bash
cd GeometricAlgebraFulcrumLib.Applications.VRSketch/Prototypes
dotnet run
```

**Ausgabe:**
- Test 1: Three-Point Arc-Spline (Grundlegend)
- Test 2: S-Curve (Kontinuität)
- Test 3: Circle-Plane mit verschiedenen Orientations

---

## Mathematische Grundlagen

### CGA 5D für 3D Geometrie

**Basis-Blades:**
- `E1, E2, E3` - Euklidische Basis (3D Raum)
- `Eo` - Origin (Null vector)
- `Ei` - Infinity (Null vector)

**CGA Point Encoding:**
```
P = Eo + p + (p·p/2) * Ei

Vereinfacht für VGa→CGA:
P = Eo + p  (im Dual-Raum)
```

**CGA Circle:**
```
Circle = Center + Radius² * Ei + Normal-Bivector

In OPNS (Outer Product Null Space):
Circle = p1 ∧ p2 ∧ p3  (durch 3 Punkte)

In IPNS (Inner Product Null Space):
Circle definiert durch Center, Radius, Normal
```

### Rotor in CGA

**Definition:**
```
Rotor R = e^(B*θ/2)

wobei:
  B = Bivector (Rotationsebene)
  θ = Rotationswinkel
```

**Transformation:**
```
p' = R ⊲ p ⊲ R̃

wobei ⊲ = Outermorphism (Sandwich Product)
```

**Logarithmus:**
```
log(R) = B*θ/2

Extrahiert Bivector mit Winkel-Scaling
```

**Interpolation:**
```
R(t) = e^(t * log(R))

Spherical Linear Interpolation (SLERP) im Rotor-Raum
```

---

## Performance-Überlegungen

### O(1) pro Frame

Die aktuelle Implementation ist **O(1) pro Frame** da:
- Nur **ein** neues Segment wird berechnet
- Vorherige Segmente sind "frozen"
- Keine Neuberechnung des gesamten Splines

### Potentielle Optimierungen

1. **Rotor-Caching:**
   ```csharp
   // Cache Rotor-Logarithmus (teure Operation)
   private Dictionary<int, XGaBivector<double>> _logRotorCache;
   ```

2. **Adaptive Sampling:**
   ```csharp
   // Weniger Samples bei geringer Krümmung
   int samplesCount = (int)(curvature * baseResolution);
   ```

3. **Parallel Segment Processing:**
   ```csharp
   // Bei Batch-Operationen (z.B. Replay)
   Parallel.ForEach(segments, segment =>
   {
       // Sample segment independent
   });
   ```

---

## Nächste Schritte

### Sofort implementierbar:

1. **Adaptive Thresholds Integration:**
   - Implementiere `ShouldFreezeSegment()` Logik
   - Multi-Threshold: Distanz, Winkel, Krümmung, Zeit

2. **Pressure-Integration:**
   - Speichere Pressure an jedem Punkt
   - Interpoliere Pressure entlang Arc
   - Nutze für Visualisierungs-Dicke

3. **Verbesserte C¹-Kontinuität:**
   - Explizite Tangenten-Constraint
   - Least-Squares Circle-Fit
   - Hermite-Style Construction

### Für später:

4. **Visualization:**
   - Integration mit BabylonJS Visualizer
   - Polylinie-Renderer
   - Tube-Renderer mit Pressure

5. **Manipulator-Integration:**
   - Conformal Manipulator (Translation, Rotation)
   - Rotor-Field Manipulator (Guide-Spline)

---

## Bekannte Limitierungen

1. **C¹-Kontinuität nicht garantiert:**
   - Aktuelle Heuristik ist Näherung
   - Kann visuelle "Knicke" produzieren

2. **Keine automatische Ebenen-Anpassung:**
   - Kreisebene wird nur aus aktuellem Controller berechnet
   - Könnte smooth über mehrere Punkte interpoliert werden

3. **Singularitäten:**
   - Wenn p1 ≈ p2 (Punkte zu nah): Fallback zu kleinem Kreis
   - Wenn Tangent ∥ Forward: Fallback zu Up-Vektor

4. **Numerische Stabilität:**
   - Bei sehr großen/kleinen Radien können Rundungsfehler auftreten
   - Clipping und Epsilon-Checks sind eingebaut

---

## Lizenz & Autor

Teil des GeometricAlgebraFulcrumLib Projekts.

**Prototyp erstellt:** 2025-10-06
**Basis:** Direct 3D CGA Approach (Alternative zu Ganja.js 2D PGA)
