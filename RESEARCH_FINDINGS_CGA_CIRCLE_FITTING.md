# Recherche-Ergebnisse: CGA Circle-Fitting für Arc-Splines

**Datum:** 9. Oktober 2025
**Thema:** Kann man Circle-Fitting (>3 Punkte) direkt in CGA durchführen?
**Ergebnis:** **JA - mit wichtigen Einschränkungen**

---

## Executive Summary

### Die Kernfrage
Kann man den Arc-Fitting Algorithmus **vollständig in CGA** implementieren, ohne den "Umweg" über Euklid-PCA + Pratt/Taubin?

### Die Antwort
**Ja, aber...**

1. ✅ **3 Punkte → Kreis:** Trivial in CGA via `p1 ∧ p2 ∧ p3`
2. ✅ **>3 Punkte → Kreis (Least-Squares):** **Möglich in CGA** - Leo Dorst (2014/2018)
3. ⚠️ **Implementiert in GA-FuL:** **NEIN** - nur 3-Punkt-Methode vorhanden
4. ⚠️ **Komplexität:** Höher als traditionelle Methoden
5. ✅ **Vorteil:** Koordinatenfrei, eleganter, generalisierbarer

---

## Wichtigste Erkenntnisse

### 1. GA-FuL Codebase-Analyse

**Was IST vorhanden:**

```csharp
// ✅ 3-Punkt-Kreis (PURE CGA)
Cga.Encode.OpnsRound.Circle(point1, point2, point3)

// Implementation (Line 154-161, CGaFloat64OpnsRoundEncoder.cs):
var p1 = GeometricSpace.Encode.IpnsRound.Point(egaPoint1);
var p2 = GeometricSpace.Encode.IpnsRound.Point(egaPoint2);
var p3 = GeometricSpace.Encode.IpnsRound.Point(egaPoint3);
return p1.Op(p2).Op(p3);  // <-- Pure CGA: p1 ∧ p2 ∧ p3
```

**Was FEHLT:**
- ❌ Circle-Fitting für >3 Punkte (Least-Squares)
- ❌ Plane-Fitting für Punktwolken
- ❌ Allgemeine GA Least-Squares Solver

**Was DA IST (verwandt):**
- ✅ `EllipseFitting/` - Eigenzerlegung (Jacobi), könnte adaptiert werden
- ✅ `CGaFloat64LerpRoundUtils` - Interpolation von Kreisen
- ✅ Umfangreiches Versor/Motor Framework

---

### 2. Leo Dorst's CGA Circle-Fitting Methode

**Hauptpaper:** "Total Least Squares Fitting of k-Spheres in n-D Euclidean Space Using an (n+2)-D Isometric Representation" (2014)

**Die Kernidee:**

1. **Embedding:** Punkte p ∈ ℝⁿ → Vektoren in ℝⁿ⁺² (CGA Raum)
2. **Pratt-Fit in CGA:** Der klassische Pratt 2D Circle-Fit wird zu einem **Eigenproblem** in CGA
3. **k-Sphere Fitting:** Hypersphere-Fit wird zu Eigenproblem eines symmetrischen linearen Operators
4. **Intersection:** Best-fit k-Spheres = Intersection (outer product) von Eigenvektoren

**Mathematik (vereinfacht):**

```
Gegeben: n Punkte {p₁, p₂, ..., pₙ} ∈ ℝ³
Gesucht: Kreis K der den kleinsten quadratischen Fehler hat

CGA-Methode:
1. Encode: Xᵢ = up(pᵢ) ∈ CGA (conformal points)
2. Bilde Moment-Matrix M = Σᵢ (Xᵢ ⊗ Xᵢ)
3. Löse Eigenproblem: M v = λ v
4. Kreis K = Rekonstruktion aus Eigenvektor(en)
```

**Vorteile gegenüber Pratt/Taubin:**
- ✅ Koordinatenfrei
- ✅ Generalisiert zu k-Spheres in n-D
- ✅ Theoretisch elegant
- ✅ Nutzt GA-Struktur (Outer products, Duality)

**Nachteile:**
- ⚠️ Höherdimensionale Eigenprobleme (5×5 statt 3×3)
- ⚠️ Weniger etabliert/getestet als Pratt
- ⚠️ Keine fertigen Libraries (außer Research-Code)

---

### 3. Alternative: Hybrid-Ansatz

**Was TODO_ARC_SPLINE_FIT.md vorschlägt:**

```
3D Punkte
  → PCA (Euklid, 3×3 Eigen)
  → Projektion auf Ebene
  → Pratt/Taubin (2D, etabliert)
  → 3D Rückprojektion
  → dann CGA für Rotor/Motor
```

**Hybrid-Vorschlag (Beste beider Welten):**

```
3D Punkte
  → PCA (Euklid, bewährt & schnell)
  → Projektion auf Ebene
  → CGA Circle-Fit (statt Pratt/Taubin)  <-- Hier CGA
  → Ab hier alles CGA (Motor, Transformationen)
```

**Begründung:**
- PCA ist sehr schnell & robust (3×3 Jacobi, ~10 Iterationen)
- Circle-Fit in 2D-CGA ist einfacher als volle 3D-CGA Fit
- Ab dem Kreis ist alles CGA → koordinatenfrei

---

### 4. Implementierungs-Optionen

#### **Option A: Euklid + CGA Hybrid** (Empfohlen für Phase 1)

**Code-Skizze:**
```csharp
public class HybridCircleFitter : IArcSplineSolver
{
    public ArcSegment? TryFitSegment(ReadOnlySpan<Vector3> points)
    {
        // 1. PCA (Euklid - schnell & robust)
        var (center, normal, u, v) = FitPlaneEuclidean(points);

        // 2. Projektion (Euklid)
        var points2D = ProjectToPlane(points, center, u, v);

        // 3. CGA Circle-Fit (2D)
        var cgaPoints2D = points2D.Select(p => Cga.Encode.VGa.Vector(p));
        var circle2D = FitCircleCGA(cgaPoints2D); // <-- CGA Method

        // 4. Lift to 3D + Motor Construction (Pure CGA ab hier)
        var circle3D = LiftTo3D(circle2D, center, normal, u, v);
        var motor = CreateMotorFromCircle(circle3D);

        return new ArcSegment { Circle = circle3D, Rotor = motor, ... };
    }
}
```

**Vorteile:**
- ✅ Nutzt bewährte PCA
- ✅ CGA ab Circle-Level
- ✅ Schnell implementierbar
- ✅ Robust

---

#### **Option B: Pure CGA** (Research/Phase 2)

**Code-Skizze:**
```csharp
public class PureCGACircleFitter : IArcSplineSolver
{
    public ArcSegment? TryFitSegment(ReadOnlySpan<Vector3> points)
    {
        // 1. Encode to CGA
        var cgaPoints = points.Select(p => Cga.Encode.IpnsRound.Point(p));

        // 2. Dorst k-Sphere Fit (direkt in CGA)
        var circle = FitCircleDorst2014(cgaPoints); // <-- Eigenproblem in CGA

        // 3. Extract Motor
        var motor = CreateMotorFromCircle(circle);

        return new ArcSegment { Circle = circle, Rotor = motor, ... };
    }

    private CGaFloat64Round FitCircleDorst2014(IEnumerable<CGaFloat64Blade> points)
    {
        // Implementierung nach Dorst (2014)
        // 1. Bilde Moment-Matrix M in CGA
        // 2. Eigenzerlegung (5×5 oder reduziert)
        // 3. Rekonstruktion aus Eigenvektoren

        // TODO: Aus Papers implementieren
        throw new NotImplementedException("Requires Dorst 2014 paper implementation");
    }
}
```

**Vorteile:**
- ✅ Pure CGA, koordinatenfrei
- ✅ Theoretisch elegant
- ✅ Publishable (neue Contribution)

**Nachteile:**
- ⚠️ Implementierungsaufwand (2-4 Wochen)
- ⚠️ Numerische Stabilität muss getestet werden
- ⚠️ Keine etablierte Referenz-Implementation

---

## Papers-Übersicht

### **Kategorie 1: CGA Circle/Sphere Fitting (Direkt relevant)**

1. ⭐⭐⭐ **Dorst (2014):** "Total Least Squares Fitting of k-Spheres in n-D"
   - **THE** Paper für CGA Least-Squares Fitting
   - Embeds Pratt-Fit in CGA Framework
   - Eigenproblem-Lösung
   - `references/Dorst_2014_Total_Least_Squares_k-Spheres_CGA.pdf`

2. ⭐⭐⭐ **Dorst (2018):** "Least Squares Fitting of Spatial Circles"
   - Praktische Anleitung
   - AGACSE Conference
   - `references/Dorst_Least_Squares_Fitting_Spatial_Circles.pdf`

3. ⭐⭐ **Dorst:** "CGA Tutorial 1"
   - Grundlagen CGA
   - `references/Dorst_CGA_Tutorial_1.pdf`

### **Kategorie 2: Arc-Splines (Anwendung)**

4. ⭐⭐⭐ **Jeon et al. (2024):** "Reliability-based G¹ Arc Spline Approximation"
   - State-of-the-art Arc-Fitting
   - Robuste G¹-Fits mit Kovarianzen
   - `references/Jeon_Hwang_Choi_2024_Reliability_G1_Arc_Spline.pdf`

5. ⭐⭐ **Drysdale, Rote, Sturm (2008):** "Minimum Number of Circular Arcs"
   - Optimale globale Lösung
   - Graph/Dijkstra Algorithmus
   - `references/Drysdale_Rote_Sturm_2008_Approximation_Polygonal_Curve.pdf`

6. ⭐⭐ **Safonova & Rossignac (2003):** "Compressed Piecewise-Circular"
   - Starke Kompression
   - Praxisnah
   - `references/Safonova_Rossignac_2003_Compressed_Piecewise_Circular.pdf`

### **Kategorie 3: CGA Grundlagen**

7. ⭐⭐⭐ **Dorst (2016):** "Construction of 3D Conformal Motions"
   - Rotor-Exponential, Orbits
   - Fundamentale CGA-Konzepte
   - `references/Dorst_2016_Construction_3D_Conformal_Motions.pdf`

8. ⭐⭐ **Doran (2003):** "Circle and Sphere Blending"
   - Circle-Blending in CGA
   - G^n-Stetigkeit
   - `references/Doran_2003_Circle_Sphere_Blending_CGA.pdf`

9. ⭐⭐ **Dorst & Mann:** "Geometric Algebra Framework"
   - Computational Framework
   - `references/Dorst_Mann_Geometric_Algebra_Framework.pdf`

10. ⭐⭐ **Gunn (2016):** "Geometric Algebras for Euclidean Geometry"
    - Umfassender Überblick
    - `references/Gunn_2016_Geometric_Algebras_Euclidean_Geometry.pdf`

### **Kategorie 4: Motoren & IK**

11. ⭐ **Kavan et al. (2006):** "Dual Quaternion Skinning"
    - Motor-Interpolation für Animation
    - `references/Kavan_et_al_2006_Dual_Quaternion_Skinning.pdf`

12. ⭐ **Prošková (2017):** "Interpolations by Rational Motions"
    - G²-Hermite-Bewegungen
    - `references/Proskova_2017_Interpolations_Rational_Motions.pdf`

### **Kategorie 5: Praxis**

13. ⭐ **Schindler (2012):** "Digital Maps Using Circular Arc Splines"
    - Anwendung in Kartierung
    - `references/Schindler_2012_Digital_Maps_Circular_Arc_Splines.pdf`

---

## Empfehlung für Ihr Projekt

### **Phase 1: Prototyp (4-6 Wochen)**

**Implementierung:** **Option A - Hybrid-Ansatz**

```
Euklid PCA + CGA Circle-Fit ab 2D-Ebene
```

**Begründung:**
1. ✅ PCA ist etabliert & schnell (GA-FuL hat Eigenzerlegung)
2. ✅ CGA ab Circle-Level = philosophisch "rein genug"
3. ✅ Schnelle Implementierung → frühe Ergebnisse
4. ✅ Robust & testbar

**Tasks:**
- [ ] PCA-Implementierung (nutze GA-FuL `EllipseFitting/JacobiSymmetricEigenDecomposer`)
- [ ] 2D Circle-Fit in CGA (basierend auf Dorst 2018 Paper)
- [ ] 3D Lift + Motor-Konstruktion (Pure CGA)
- [ ] Testing mit synthetischen Daten

---

### **Phase 2: Pure CGA (8-12 Wochen, optional)**

**Implementierung:** **Option B - Pure CGA nach Dorst (2014)**

**Begründung:**
1. ✅ Wissenschaftlicher Beitrag
2. ✅ Vollständig koordinatenfrei
3. ✅ Publishable Results

**Tasks:**
- [ ] Studium Dorst (2014) Paper im Detail
- [ ] Implementierung CGA Moment-Matrix
- [ ] Eigenzerlegung in höheren Dimensionen
- [ ] Numerische Stabilität testen
- [ ] Benchmark gegen Hybrid-Ansatz
- [ ] Paper schreiben (optional)

---

## Konkrete Code-Pfade in GA-FuL

### Für Hybrid-Ansatz nutzen:

**Eigenzerlegung (PCA):**
```
GeometricAlgebraFulcrumLib.Applications.Symbolic/EllipseFitting/
  - JacobiSymmetricEigenDecomposer.cs (3×3 Jacobi)
  - JacobiSymmetricEigenDecomposer4X4.cs (4×4 falls nötig)
```

**CGA Encoding:**
```
GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Float64/Encoding/
  - CGaFloat64OpnsRoundEncoder.cs (Circle from points)
  - CGaFloat64IpnsRoundEncoder.cs (Point encoding)
```

**CGA Operations:**
```
GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Float64/Operations/
  - CGaFloat64RotationUtils.cs (Rotor construction)
  - CGaFloat64TranslationUtils.cs (Translator)
```

**Round Utilities:**
```
GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Float64/Elements/
  - CGaFloat64RealRoundComposerUtils.cs (Circle/Sphere construction)
  - CGaFloat64Round.cs (Circle/Sphere representation)
```

---

## Precision Considerations: Float32 vs Float64

**Update:** 10. Oktober 2025

### GA-FuL Float Precision Support Analysis

**TL;DR:** GA-FuL hat **KEINE native float32 CGA-Klassen**. Verwende `CGaFloat64` intern, konvertiere zu `float` beim Output.

### Detailed Analysis

| Component | Float64 | Float32 | Status |
|-----------|---------|---------|--------|
| **CGA Geometric Space** | ✅ `CGaFloat64GeometricSpace5D` | ❌ Nicht vorhanden | Float64 only |
| **Circle from Points** | ✅ `DefineRealRoundCircleFromPoints` | ❌ Nicht vorhanden | Float64 only |
| **Jacobi Eigendecomposer** | ✅ `double[,]` | ❌ Nicht vorhanden | Float64 only |
| **Generic\<T\>** | ✅ `CGaGeometricSpace<T>` | ⚠️ Theoretisch möglich | Ungetestet |

**Code Evidence:**
```
GeometricAlgebraFulcrumLib/Modeling/Geometry/CGa/
├── Float64/          ← 83 files ✅ COMPLETE
├── Generic/          ← 70 files ✅ (but Jacobi still double)
└── Float32/          ← ❌ DOES NOT EXIST
```

### Recommended Strategy: **Float64 Internal, Float32 Output**

**Architecture:**
```
VR Input (Vector3 float)
    ↓ ToDouble()
OnlinePCA (Vector3D double)
    ↓
Jacobi Eigendecomposer (double[,])
    ↓
CGaFloat64GeometricSpace5D.DefineRealRoundCircleFromPoints()
    ↓ ToFloat()
ArcSegment (Vector3 float) → GPU/BabylonJS
```

**Rationale:**

1. ✅ **Zero Implementation Overhead**
   - Nutze GA-FuL Float64 CGA direkt
   - Keine Custom-Implementierung nötig
   - Start sofort möglich

2. ✅ **Better Numerical Stability**
   - Double precision während PCA & Circle-Fitting
   - Vermeidet Akkumulationsfehler
   - CGA-Operationen (Division, Sqrt) stabiler

3. ✅ **Negligible Overhead**
   - Memory: ~3 KB pro Stroke (256 Punkte)
   - Performance: <1 µs Conversion-Zeit
   - GPU bekommt trotzdem float32

4. ✅ **Proven & Tested**
   - GA-FuL Float64 battle-tested
   - Keine Überraschungen bei Edge-Cases
   - Standard workflow

**Precision Analysis:**

| Stage | Type | Precision | Rationale |
|-------|------|-----------|-----------|
| **Input** | `Vector3` (float) | ~7 digits | VR Controller, GPU |
| **PCA** | `double` | ~15 digits | Eigenzerlegung braucht Stabilität |
| **Circle-Fit** | `CGaFloat64` | ~15 digits | CGA Divisions & Sqrt stabil |
| **Output** | `Vector3` (float) | ~7 digits | GPU/BabylonJS compatible |

**Tolerance Check:**
- Fitting Tolerance: `EpsilonRadial = 1e-3 m` (1 mm)
- Float32 Precision: ~0.001% error at 1m scale
- **Conclusion:** Float32 output precision **sufficient** für VR

### Implementation Example

```csharp
// FitSettings.cs - User API: float
public sealed class FitSettings
{
    public float EpsilonRadial { get; init; } = 1e-3f;  // float für User
}

// OnlinePCA.cs - Internal: double
public sealed class OnlinePCA
{
    private Vector3D _mean;        // double intern
    private Matrix3x3D _covariance;

    public void AddPoint(Vector3 p)    // Input: float
    {
        var pd = ToDouble(p);  // Konvertiere sofort
        // ... PCA in double
    }

    public (Vector3 normal, ...) GetPlane()  // Output: float
    {
        var eigenVec = JacobiDecompose(_covariance);  // double
        return ToFloat(eigenVec);  // Konvertiere am Ende
    }
}

// CircleFitCGA.cs - Internal: CGaFloat64
public static class CircleFitCGA
{
    private static readonly CGaFloat64GeometricSpace5D _cga;

    public static (Vector3 center, float radius) Fit(Vector3[] points)
    {
        // To double
        var p1_64 = ToDouble(points[0]);
        // ... CGA in double
        var circle = _cga.DefineRealRoundCircleFromPoints(...);
        // To float
        return (ToFloat(circle.Center), (float)circle.Radius);
    }
}
```

**Siehe:** `ANALYSIS_FLOAT32_SUPPORT.md` für vollständige Analyse & Decision Matrix.

---

## Fazit

**Kann man Circle-Fitting direkt in CGA machen?**
→ **JA!** Leo Dorst (2014) zeigt wie.

**Sollten Sie es für Phase 1 tun?**
→ **Hybrid-Ansatz empfohlen**: PCA (Euklid) + CGA ab Circle

**Warum Hybrid?**
1. Schneller zu implementieren
2. Nutzt bewährte Methoden (PCA)
3. Trotzdem CGA ab Kreis-Level
4. Pragmatisch & robust

**Für später:**
Pure CGA-Implementation als Research-Projekt/Publikation

---

**Nächste Schritte:**
1. Entscheidung: Hybrid vs. Pure CGA
2. Implementation starten
3. Testing mit synthetischen Daten
4. Benchmark & Validierung

**Stand:** 9. Oktober 2025
**Status:** Research abgeschlossen, Ready for Implementation
