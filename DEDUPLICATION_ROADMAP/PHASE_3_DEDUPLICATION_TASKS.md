# Phase 3 Deduplication Tasks - Detaillierte Checkliste

**Erstellt:** 2025-10-28 (Nach vollständiger Verifikation aller 257 Klassen)
**Status:** 📋 PLANNED - Bereit zum Start nach Phase 2
**Prinzip:** Generic-First - NUR Generic wird implementiert
**Scope:** 257 Klassen in 10 Modulen (6A-6D, 7A-7B, 8, 9, 10)

---

## ⚠️ WICHTIG: Execution Rules für Phase 3

1. **Ein Modul nach dem anderen** - Phase 3A komplett, dann 3B, dann 3C, dann 3D
2. **Generic-Only** - Float64 wird NICHT erweitert (deprecated)
3. **Tests schreiben** - Jede neue Implementierung braucht Equivalence-Tests
4. **Dokumentation** - Nach jedem Modul alle 3 Roadmap-Docs aktualisieren
5. **Performance messen** - Benchmarks nach jedem Modul

---

## 🔴 KRITISCHER WORKFLOW: Equivalence Test Pattern (ZWINGEND!)

**Für JEDE Klasse gilt diese Reihenfolge:**

```
1. ✅ IMPLEMENTIERE Generic<T> Klasse
   ↓
2. ✅ SCHREIBE Equivalence Tests (Generic<double> vs Float64)
   ↓
3. ✅ STELLE SICHER alle Tests passing (100% Pass Rate!)
   ↓
4. ✅ NUR DANN Git Commit
   ↓
5. ✅ Weiter zur nächsten Klasse
```

**❌ NIEMALS committen wenn:**
- Tests fehlen
- Tests failing
- Equivalence nicht nachgewiesen

**✅ Bewährtes Pattern aus Phase 1:**
- XGaComputedOutermorphism<T>: 9 Tests → 100% passing → Commit ✅
- XGaGramSchmidtFrame<T>: 9 Tests → 100% passing → Commit ✅
- ComplexNumber<T>: 30 Tests → 100% passing → Commit ✅

**Minimum-Anforderung pro Klasse:**
- **10+ Equivalence Tests** (Generic<double> vs Float64)
- **100% Pass Rate** BEFORE Commit
- Test-Coverage für alle Public Methods

---

## 🗺️ MODULE 6A: Trajectories Vectors3D (60 Klassen)

**Priorität:** P1 (Critical)
**Geschätzter Aufwand:** 8 Wochen (320 Stunden)
**Start:** Nach Phase 2 Complete

### Architektur-Überlegungen

**Float64-Hierarchy:**
```
Float64Path3D (Basis)
├── Float64BasicPath3D
├── Float64BezierPath3D
│   ├── Float64CatmullRomSplinePath3D
│   ├── Float64HermiteSplinePath3D
│   └── ...
├── Float64AdaptivePath3D
├── Float64MappedPath3D
└── ...
```

**Generic<T> Hierarchy:**
```
ParametricPath3D<T> (Basis)
├── BasicPath3D<T>
├── BezierPath3D<T>
│   ├── CatmullRomSplinePath3D<T>
│   ├── HermiteSplinePath3D<T>
│   └── ...
├── AdaptivePath3D<T>
├── MappedPath3D<T>
└── ...
```

### Task 6A.1: Basis-Klassen (Woche 1-2)

- [ ] **ParametricPath3D<T>** - Basis-Klasse
  - **Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Trajectories.Vectors3D.Generic`
  - **Referenz:** `Float64Path3D` (~200 LOC)
  - **Estimated:** 8-12 Stunden

**Implementation (WORKFLOW ZWINGEND EINHALTEN!):**
- [ ] 1️⃣ **IMPLEMENTIERUNG:** Float64Path3D.cs analysieren und Generic<T> erstellen
  - [ ] Generic<T> Basis-Klasse erstellen
  - [ ] Properties implementieren:
    - [ ] `IScalarProcessor<T> ScalarProcessor`
    - [ ] `ScalarRange<T> TimeRange`
    - [ ] `bool IsPeriodic`
  - [ ] Methods implementieren:
    - [ ] `GetPoint(T time)` → `LinVector3D<T>`
    - [ ] `GetTangent(T time)` → `LinVector3D<T>`
    - [ ] `GetUnitTangent(T time)` → `LinVector3D<T>`
    - [ ] `GetLength()` → `T`
    - [ ] `GetLength(T t1, T t2)` → `T`

- [ ] 2️⃣ **EQUIVALENCE TESTS:** Tests schreiben (Generic<double> vs Float64)
  - [ ] Mindestens 10+ Tests schreiben
  - [ ] Alle Public Methods testen
  - [ ] Test-Pattern: Float64-Klasse vs Generic<double> vergleichen

- [ ] 3️⃣ **VERIFICATION:** Alle Tests passing (100% Pass Rate)
  - [ ] `dotnet test --filter "ParametricPath3DEquivalenceTests"`
  - [ ] ALLE Tests grün ✅

- [ ] 4️⃣ **COMMIT:** NUR wenn 100% Tests passing!
  - [ ] Git add + commit mit klarer Message
  - [ ] Message-Format: "feat(Generic): Add ParametricPath3D<T> + 10 Equivalence Tests ✅"

---

- [ ] **BasicPath3D<T>** - Einfache parametrische Kurve
  - **Referenz:** `Float64BasicPath3D` (~150 LOC)
  - **Estimated:** 6-8 Stunden

**Implementation:**
- [ ] Constructor mit `Func<T, LinVector3D<T>>`
- [ ] GetPoint, GetTangent Implementierung
- [ ] Tests schreiben
- [ ] Git commit

---

- [ ] **ConstantPath3D<T>** - Konstanter Punkt
  - **Referenz:** `Float64ConstantPath3D` (~80 LOC)
  - **Estimated:** 3-4 Stunden

---

- [ ] **LinearPath3D<T>** - Linear interpolated path
  - **Referenz:** `Float64LinearPath3D` (~100 LOC)
  - **Estimated:** 4-6 Stunden

---

- [ ] **ComposedPath3D<T>** - Composition mehrerer Pfade
  - **Referenz:** `Float64ComposedPath3D` (~180 LOC)
  - **Estimated:** 8-10 Stunden

### Task 6A.2: Bezier Curves (Woche 3-5)

- [ ] **BezierPath3D<T>** - N-degree Bezier
  - **Referenz:** `Float64BezierPath3D` (~400 LOC)
  - **Estimated:** 16-20 Stunden
  - **KOMPLEX:** Bernstein polynomials, recursive evaluation

**Implementation:**
- [ ] Bezier basis functions mit IScalarProcessor<T>
- [ ] DeCasteljau algorithm
- [ ] Control point management
- [ ] Tangent/Curvature calculation
- [ ] Tests (20+ Equivalence Tests)
- [ ] Git commit

---

- [ ] **CatmullRomSplinePath3D<T>** - Catmull-Rom Spline
  - **Referenz:** `Float64CatmullRomSplinePath3D` (~300 LOC)
  - **Estimated:** 12-16 Stunden

---

- [ ] **HermiteSplinePath3D<T>** - Hermite Spline
  - **Referenz:** `Float64HermiteSplinePath3D` (~250 LOC)
  - **Estimated:** 10-14 Stunden

---

- [ ] **BSplinePath3D<T>** - B-Spline
  - **Referenz:** `Float64BSplinePath3D` (~350 LOC)
  - **Estimated:** 14-18 Stunden

---

- [ ] **NurbsPath3D<T>** - NURBS
  - **Referenz:** `Float64NurbsPath3D` (~450 LOC)
  - **Estimated:** 18-24 Stunden
  - **SEHR KOMPLEX:** Rational basis functions, knot vectors

---

- [ ] **BezierDegree2Path3D<T>** - Quadratic Bezier (optimiert)
  - **Referenz:** `Float64BezierDegree2Path3D` (~120 LOC)
  - **Estimated:** 6-8 Stunden

---

- [ ] **BezierDegree3Path3D<T>** - Cubic Bezier (optimiert)
  - **Referenz:** `Float64BezierDegree3Path3D` (~150 LOC)
  - **Estimated:** 8-10 Stunden

---

### Task 6A.3: Adaptive & Special Curves (Woche 6-7)

- [ ] **AdaptivePath3D<T>** - Adaptive sampling based on curvature
  - **Referenz:** `Float64AdaptivePath3D` (~250 LOC)
  - **Estimated:** 12-16 Stunden
  - **KOMPLEX:** Curvature-based sampling, adaptive refinement

---

- [ ] **ArcLengthPath3D<T>** - Arc-length parameterization
  - **Referenz:** `Float64ArcLengthPath3D` (~220 LOC)
  - **Estimated:** 10-14 Stunden
  - **KOMPLEX:** Numerical integration for arc length

---

- [ ] **CirclePath3D<T>** - Circle in 3D space
  - **Referenz:** `Float64CirclePath3D` (~150 LOC)
  - **Estimated:** 6-8 Stunden

---

- [ ] **EllipsePath3D<T>** - Ellipse in 3D space
  - **Referenz:** `Float64EllipsePath3D` (~180 LOC)
  - **Estimated:** 8-10 Stunden

---

- [ ] **HelixPath3D<T>** - Helix/Spiral
  - **Referenz:** `Float64HelixPath3D` (~130 LOC)
  - **Estimated:** 6-8 Stunden

---

- [ ] **LissajousPath3D<T>** - Lissajous curve
  - **Referenz:** `Float64LissajousPath3D` (~140 LOC)
  - **Estimated:** 6-8 Stunden

---

### Task 6A.4: Mapped & Transformed (Woche 6-7)

- [ ] **MappedPath3D<T>** - Transformed path
  - **Referenz:** `Float64MappedPath3D` (~180 LOC)
  - **Estimated:** 8-10 Stunden

---

- [ ] **RotatedPath3D<T>** - Rotation-transformed
  - **Referenz:** `Float64RotatedPath3D` (~150 LOC)
  - **Estimated:** 6-8 Stunden

---

- [ ] **ScaledPath3D<T>** - Scaled path
  - **Referenz:** `Float64ScaledPath3D` (~120 LOC)
  - **Estimated:** 5-7 Stunden

---

- [ ] **TranslatedPath3D<T>** - Translated path
  - **Referenz:** `Float64TranslatedPath3D` (~110 LOC)
  - **Estimated:** 5-7 Stunden

---

### Task 6A.5: Composers & Builders (Woche 7-8)

- [ ] **Path3DComposer<T>** - Builder pattern
  - **Referenz:** `Float64Path3DComposer` (~200 LOC)
  - **Estimated:** 10-12 Stunden

**Methods:**
- [ ] `SetBasicPath(Func<T, LinVector3D<T>>)`
- [ ] `SetBezierPath(List<LinVector3D<T>> controlPoints)`
- [ ] `SetCatmullRomPath(List<LinVector3D<T>> points)`
- [ ] `SetCirclePath(center, radius, normal)`
- [ ] `GetPath()` → `ParametricPath3D<T>`

---

- [ ] **Path3DComposerUtils<T>** - Static utilities
  - **Referenz:** `Float64Path3DComposerUtils` (~150 LOC)
  - **Estimated:** 6-8 Stunden

---

### Task 6A.6: Samplers (Woche 8)

- [ ] **Path3DSampler<T>** - Uniform sampling
  - **Referenz:** `Float64Path3DSampler` (~150 LOC)
  - **Estimated:** 6-8 Stunden

---

- [ ] **AdaptivePath3DSampler<T>** - Adaptive sampling
  - **Referenz:** `Float64AdaptivePath3DSampler` (~180 LOC)
  - **Estimated:** 8-10 Stunden

---

- [ ] **ArcLengthPath3DSampler<T>** - Arc-length sampling
  - **Referenz:** `Float64ArcLengthPath3DSampler` (~170 LOC)
  - **Estimated:** 8-10 Stunden

---

### Task 6A.7: Testing & Documentation (Woche 8)

- [ ] **Equivalence Tests für alle 60 Klassen**
  - **Estimated:** 20-30 Stunden
  - **Pattern:** Generic<double> vs Float64

- [ ] **Performance Benchmarks**
  - Basic paths
  - Bezier curves
  - Adaptive sampling

- [ ] **Dokumentation aktualisieren**
  - [ ] DEDUPLICATION_ROADMAP.md → Module 6A: Complete ✅
  - [ ] NEXT_STEPS_ROADMAP.md → Module 6B: Next
  - [ ] Git push all commits

---

## 🗺️ MODULE 6B: Trajectories Vectors2D (40 Klassen)

**Priorität:** P1 (Critical)
**Geschätzter Aufwand:** 5 Wochen (200 Stunden)
**Start:** Nach Module 6A

**ÄHNLICH zu 6A, aber 2D statt 3D:**
- Alle Klassen analog zu 6A
- `LinVector2D<T>` statt `LinVector3D<T>`
- Einfachere Geometrie (keine Normal-Vektoren nötig)

### Klassen-Liste (40 total):
- [ ] ParametricPath2D<T>, BasicPath2D<T>, ConstantPath2D<T>, LinearPath2D<T>
- [ ] BezierPath2D<T>, CatmullRomSplinePath2D<T>, HermiteSplinePath2D<T>, BSplinePath2D<T>
- [ ] AdaptivePath2D<T>, ArcLengthPath2D<T>
- [ ] CirclePath2D<T>, EllipsePath2D<T>, CircularArcPath2D<T>
- [ ] MappedPath2D<T>, RotatedPath2D<T>, ScaledPath2D<T>, TranslatedPath2D<T>
- [ ] Path2DComposer<T>, Path2DSampler<T>, AdaptivePath2DSampler<T>
- [ ] + 20 weitere Spezialkurven

---

## 🗺️ MODULE 6C: Trajectories Scalars (40 Klassen)

**Priorität:** P1 (Important)
**Geschätzter Aufwand:** 5 Wochen (200 Stunden)
**Start:** Nach Module 6B (oder parallel zu 8)

**Scalar Trajectories:** `time → scalar`

### Klassen-Liste (40 total):
- [ ] ParametricScalar<T>, BasicScalarPath<T>, ConstantScalar<T>, LinearScalar<T>
- [ ] AnglePath<T>, NormalizedAngle<T>, PolarAngle<T>
- [ ] ParametricAngle<T>, AngleComposer<T>
- [ ] ScalarComposer<T>, MappedScalar<T>, NormalizedScalar<T>
- [ ] ScalarSignal<T>, PeriodicScalar<T>
- [ ] + 25 weitere Scalar-Funktionen

---

## 🗺️ MODULE 6D: Trajectories Others (11 Klassen)

**Priorität:** P2 (Optional)
**Geschätzter Aufwand:** 2 Wochen (80 Stunden)
**Start:** Phase 3D (optional)

### Bivectors (5 Klassen):
- [ ] ParametricBivector2D<T>, BivectorPath2D<T>
- [ ] ParametricBivector3D<T>, BivectorPath3D<T>
- [ ] BivectorComposer<T>

### Quaternions (4 Klassen):
- [ ] ParametricQuaternion<T>, QuaternionPath<T>
- [ ] SlerpPath<T> (Spherical Linear Interpolation)
- [ ] SquadPath<T> (Spherical Quadrangle Interpolation)

### Trivectors (2 Klassen):
- [ ] ParametricTrivector3D<T>, TrivectorPath3D<T>

---

## 🗺️ MODULE 7A: Calculus Core DifferentialFunction (35 Klassen)

**Priorität:** P1 (Critical)
**Geschätzter Aufwand:** 7 Wochen (280 Stunden)
**Start:** Nach Module 6A (parallel zu 6B möglich)

### Task 7A.1: DifferentialFunction Hierarchy (Woche 1-2)

- [ ] **DifferentialFunction<T>** - Basis-Klasse
  - **Referenz:** `DifferentialFunction` (~250 LOC)
  - **Estimated:** 12-16 Stunden
  - **KOMPLEX:** Expression tree, derivative rules

**Implementation:**
- [ ] Expression tree representation
- [ ] `Differentiate()` method (returns DifferentialFunction<T>)
- [ ] `Evaluate(T x)` method
- [ ] `Simplify()` method
- [ ] Operator overloads (+, -, *, /)

---

- [ ] **DifferentialBasicFunction<T>** - Variable & Constant
  - **Referenz:** `DifferentialBasicFunction` (~80 LOC)
  - **Estimated:** 6-8 Stunden

---

- [ ] **DifferentialUnaryFunction<T>** - Sin, Cos, Exp, etc.
  - **Referenz:** `DifferentialUnaryFunction` (~150 LOC)
  - **Estimated:** 8-10 Stunden

---

- [ ] **DifferentialBinaryFunction<T>** - Plus, Times, Power
  - **Referenz:** `DifferentialBinaryFunction` (~180 LOC)
  - **Estimated:** 10-12 Stunden

---

- [ ] **DifferentialNaryFunction<T>** - Sum, Product
  - **Referenz:** `DifferentialNaryFunction` (~120 LOC)
  - **Estimated:** 8-10 Stunden

---

- [ ] **DifferentialCompositeFunction<T>** - f(g(x))
  - **Referenz:** `DifferentialCompositeFunction` (~200 LOC)
  - **Estimated:** 12-16 Stunden
  - **KOMPLEX:** Chain rule

---

- [ ] **DifferentialCustomFunction<T>** - User-defined
  - **Referenz:** `DifferentialCustomFunction` (~100 LOC)
  - **Estimated:** 6-8 Stunden

---

### Task 7A.2: Concrete Functions (Woche 3-4)

- [ ] **DfVar<T>** - Variable
  - **Referenz:** `DfVar` (~60 LOC)
  - **Estimated:** 4-6 Stunden

---

- [ ] **DfConstant<T>** - Constant
  - **Referenz:** `DfConstant` (~80 LOC)
  - **Estimated:** 4-6 Stunden

---

- [ ] **DfCos<T>**, **DfSin<T>** - Trigonometric
  - **Referenz:** `DfCos`, `DfSin` (~100 LOC each)
  - **Estimated:** 6-8 Stunden each

---

- [ ] **DfExp<T>** - Exponential
  - **Referenz:** `DfExp` (~90 LOC)
  - **Estimated:** 6-8 Stunden

---

- [ ] **DfPlus<T>**, **DfTimes<T>** - Arithmetic
  - **Referenz:** `DfPlus`, `DfTimes` (~120 LOC each)
  - **Estimated:** 6-8 Stunden each

---

- [ ] **DfPowerScalar<T>** - Power function
  - **Referenz:** `DfPowerScalar` (~130 LOC)
  - **Estimated:** 8-10 Stunden

---

- [ ] **DfSmoothBlend<T>** - Smooth blending
  - **Referenz:** `DfSmoothBlend` (~110 LOC)
  - **Estimated:** 6-8 Stunden

---

- [ ] **DfFiniteSupport<T>** - Finite support
  - **Referenz:** `DfFiniteSupport` (~90 LOC)
  - **Estimated:** 6-8 Stunden

---

### Task 7A.3: Constant Values (Woche 5)

- [ ] **DfConstantValue<T>** - Base
  - **Estimated:** 4-6 Stunden

- [ ] **DfConstantValueE<T>**, **DfConstantValuePi<T>**
  - **Estimated:** 3-4 Stunden each

- [ ] **DfConstantValueInteger<T>**, **DfConstantValueRational<T>**
  - **Estimated:** 3-4 Stunden each

- [ ] **DfConstantValueFloat<T>**, **DfConstantValueDecimal<T>**
  - **Estimated:** 3-4 Stunden each

- [ ] **DfConstantValuePlus<T>**, **DfConstantValueTimes<T>**
  - **Estimated:** 4-6 Stunden each

- [ ] **DfConstantValueUtils<T>**
  - **Estimated:** 4-6 Stunden

---

### Task 7A.4: Testing & Integration (Woche 6-7)

- [ ] **Equivalence Tests** (100+ Tests)
  - Derivative rules
  - Chain rule
  - Expression simplification
  - Operator overloads

- [ ] **Performance Benchmarks**
  - Expression evaluation
  - Differentiation speed

- [ ] **Documentation**
  - [ ] DEDUPLICATION_ROADMAP.md update
  - [ ] Git push

---

## 🗺️ MODULE 7B: Calculus Advanced (35+ Klassen)

**Priorität:** P2-P3 (Optional)
**Geschätzter Aufwand:** 8 Wochen (320 Stunden)
**Start:** Phase 3D (optional)

### Interpolators (14 Klassen) - 3 Wochen
- [ ] DfAkimaSplineInterpolator<T>
- [ ] DfBarycentricInterpolator<T>
- [ ] DfCatmullRomSplineInterpolator<T>
- [ ] DfChebyshevSignalInterpolator<T>
- [ ] DfFourierSignalInterpolator<T>
- [ ] DfLinearSplineSignalInterpolator<T>
- [ ] + Options classes

### Polynomials (9 Klassen) - 2 Wochen
- [ ] DfPolynomial<T>
- [ ] DfBernsteinBasis<T>, DfBernsteinPolynomial<T>
- [ ] DfChebyshevBasis<T>, DfChebyshevPolynomial<T>
- [ ] DfMonomialBasis<T>, DfMonomialPolynomial<T>
- [ ] DfAffinePolynomial<T>

### Differential Curves (6 Klassen) - 2 Wochen
- [ ] DifferentialPath3D<T> (800 LOC!) - 40-60 Stunden allein!
- [ ] PowerSignal3D<T>
- [ ] PowerSignal3DAnalyzer<T>
- [ ] DifferentialCurve<T>
- [ ] DifferentialCurveFrame3D<T>
- [ ] TorusKnotCurve3D<T>

### Fourier (4 Klassen) - 1 Woche
- [ ] VectorFourierCurve<T>
- [ ] VectorFourierCurveTerm<T>
- [ ] MultivectorFourierCurve<T>
- [ ] MultivectorFourierCurveTerm<T>

### AutoDiff (~40 Klassen) - OPTIONAL (SEHR SCHWIERIG)
**WARNUNG:** AutoDiff ist hardcoded `double`!
- Evtl. NICHT generifizierbar ohne komplette Neu-Implementation
- **ENTSCHEIDUNG:** Erst implementieren wenn explizit benötigt (P3)

---

## 🗺️ MODULE 8: Signals (11 Klassen)

**Priorität:** P1 (Important)
**Geschätzter Aufwand:** 3 Wochen (100 Stunden)
**Start:** Nach Module 6C

### Task 8.1: Core Signal Processing (Woche 1-2)

- [ ] **SampledTimeSignal<T>** - GRÖSSTE KLASSE (1,655 LOC!)
  - **Referenz:** `Float64SampledTimeSignal`
  - **Estimated:** 40-60 Stunden
  - **SEHR KOMPLEX:** FFT, IFFT, Integration, Energy, Operators

**Implementation:**
- [ ] SamplingSpecs<T> integration
- [ ] Sample storage (IReadOnlyList<T>)
- [ ] FFT via IScalarProcessor<T>.Fft() (wenn verfügbar)
- [ ] IFFT via IScalarProcessor<T>.Ifft()
- [ ] IntegrateTrapezoidal() - Numerical integration
- [ ] Energy(), EnergyAc(), EnergyDc()
- [ ] GetFourierSpectrum() → SignalSpectrum<T>
- [ ] CreateFourierInterpolator() → DifferentialFunction<T>
- [ ] Operators: +, -, *, /
- [ ] 100+ Methods!
- [ ] Tests (50+ Equivalence Tests)
- [ ] Git commit

---

- [ ] **SamplingSpecs<T>** - Sampling specifications
  - **Referenz:** `Float64SamplingSpecs` (~50 LOC)
  - **Estimated:** 3-4 Stunden

---

- [ ] **ComplexSignalSpectrum<T>** - Complex frequency spectrum
  - **Referenz:** `Float64ComplexSignalSpectrum` (~200 LOC)
  - **Estimated:** 10-12 Stunden

---

### Task 8.2: Analysis (Woche 2)

- [ ] **SignalHistogram<T>** - Signal histogram
  - **Referenz:** `Float64SignalHistogram` (~150 LOC)
  - **Estimated:** 8-10 Stunden

---

- [ ] **SignalLog2Histogram<T>** - Log2 histogram
  - **Referenz:** `Float64SignalLog2Histogram` (~120 LOC)
  - **Estimated:** 6-8 Stunden

---

- [ ] **SignalSpectrum<T>** - Signal spectrum (bereits teilweise vorhanden)
  - **Referenz:** `Float64SignalSpectrum` (~180 LOC)
  - **Estimated:** 8-10 Stunden (Erweiterung von Scalar SignalSpectrum<T>)

---

### Task 8.3: Composers & Utils (Woche 3)

- [ ] **SampledTimeSignalComposer<T>** - Builder
  - **Referenz:** `Float64SampledTimeSignalComposer` (~100 LOC)
  - **Estimated:** 6-8 Stunden

---

- [ ] **SignalComposerUtils<T>** - Static utilities
  - **Referenz:** `Float64SignalComposerUtils` (~80 LOC)
  - **Estimated:** 4-6 Stunden

---

- [ ] **SignalInterpolatorComposerUtils<T>** - Interpolator utilities
  - **Referenz:** `Float64SignalInterpolatorComposerUtils` (~70 LOC)
  - **Estimated:** 4-6 Stunden

---

- [ ] **SignalUtils<T>** - General utilities
  - **Referenz:** `Float64SignalUtils` (~150 LOC)
  - **Estimated:** 8-10 Stunden

---

- [ ] **VectorSignalUtils<T>** - Vector signal utilities
  - **Referenz:** `Float64VectorSignalUtils` (~120 LOC)
  - **Estimated:** 6-8 Stunden

---

### Task 8.4: Testing & Documentation (Woche 3)

- [ ] **Equivalence Tests** (80+ Tests)
  - FFT/IFFT
  - Signal operations
  - Spectrum analysis

- [ ] **Performance Benchmarks**
  - FFT performance
  - Signal processing operations

- [ ] **Documentation**
  - [ ] DEDUPLICATION_ROADMAP.md update
  - [ ] Git push

---

## 🗺️ MODULE 9: Statistics (15 Klassen)

**Priorität:** P2 (Nice-to-Have)
**Geschätzter Aufwand:** 1.5 Wochen (60 Stunden)
**Start:** Phase 3C

### Continuous (8 Klassen) - 1 Woche
- [ ] HistogramBinData<T>
- [ ] PiecewiseAffineFunction<T>
- [ ] ProbabilityDistributionFunction<T>
- [ ] QuantizedHistogram<T>
- [ ] QuantizedHistogramBinData<T>
- [ ] QuantizedHistogramPdf<T>
- [ ] SparseIrregularHistogram<T>
- [ ] SparseRegularHistogram<T>

### Discrete (3 Klassen) - 0.5 Wochen
- [ ] DiscreteProbabilityFunction<T>
- [ ] DiscreteProbabilityMassFunction<T>
- [ ] PmfRandomGenerator<T>

### Random (3 Klassen) - 0.5 Wochen
- [ ] RandomEuclideanVectorsComposer<T>
- [ ] RandomGaMultivectorComposer<T>
- [ ] RandomUtils<T>

### Base (1 Klasse) - 0.5 Wochen
- [ ] CumulativeDistributionFunction<T>

---

## 🗺️ MODULE 10: PropagatorNetworks (10 Klassen)

**Priorität:** P2-P3 (Specialized)
**Geschätzter Aufwand:** 1.25 Wochen (50 Stunden)
**Start:** Phase 3C

### Core (3 Klassen) - 0.5 Wochen
- [ ] PnCell<T> (191 LOC)
- [ ] PnValue<T>
- [ ] PnPropagator<T>

### Operations (6 Klassen) - 0.5 Wochen
- [ ] PnPropagatorPlus<T>
- [ ] PnPropagatorMinus<T>
- [ ] PnPropagatorTimes<T>
- [ ] PnPropagatorDivide<T>
- [ ] PnPropagatorSquare<T>
- [ ] PnPropagatorSquareRoot<T>

### Utils (1 Klasse) - 0.25 Wochen
- [ ] PnComputationUtils<T>

---

## 📅 Workflow für jeden Tag (Phase 3)

### Morgen:
1. [ ] Dieses Dokument öffnen
2. [ ] Nächstes unkomplettiertes Task identifizieren
3. [ ] Float64-Implementierung analysieren
4. [ ] Test-Strategie definieren

### Während Arbeit:
1. [ ] Generic<T> Klasse implementieren
2. [ ] IScalarProcessor<T> für alle Operationen verwenden
3. [ ] Tests schreiben parallel zur Implementierung
4. [ ] Tests laufen lassen nach jeder Änderung
5. [ ] Iterieren bis alle Tests passing
6. [ ] **Checkbox abhaken in diesem Dokument** ✓

### Abend:
1. [ ] Git commit mit klarer Message
2. [ ] Fortschritt dokumentieren (Checkboxen setzen)
3. [ ] Nächsten Tag planen

### Wöchentlich:
1. [ ] Alle 3 Roadmap-Dokumente aktualisieren
2. [ ] Fortschritt vs. Schätzung prüfen
3. [ ] Zeitplan anpassen wenn nötig
4. [ ] Performance-Benchmarks laufen lassen

---

## 📊 Erfolgsmetriken (Nach jedem Modul tracken)

**Module 6A: Trajectories Vectors3D**
- [ ] 60 Generic<T> Klassen implementiert
- [ ] 200+ Equivalence-Tests passing
- [ ] Performance: Generic ≥ 95% von Float64
- [ ] LOC hinzugefügt: ~8,000 LOC
- [ ] Zeitaufwand: X Stunden (vs. 320 Stunden geschätzt)

**Module 6B: Trajectories Vectors2D**
- [ ] 40 Generic<T> Klassen implementiert
- [ ] 150+ Equivalence-Tests passing
- [ ] Zeitaufwand: X Stunden (vs. 200 Stunden geschätzt)

**Module 6C: Trajectories Scalars**
- [ ] 40 Generic<T> Klassen implementiert
- [ ] 120+ Equivalence-Tests passing
- [ ] Zeitaufwand: X Stunden (vs. 200 Stunden geschätzt)

**Module 7A: Calculus Core**
- [ ] 35 Generic<T> Klassen implementiert
- [ ] 150+ Equivalence-Tests passing
- [ ] DifferentialFunction<T> Hierarchy funktional
- [ ] Zeitaufwand: X Stunden (vs. 280 Stunden geschätzt)

**Module 8: Signals**
- [ ] 11 Generic<T> Klassen implementiert
- [ ] 100+ Equivalence-Tests passing
- [ ] FFT/IFFT funktional
- [ ] Zeitaufwand: X Stunden (vs. 100 Stunden geschätzt)

**Module 9: Statistics**
- [ ] 15 Generic<T> Klassen implementiert
- [ ] 50+ Equivalence-Tests passing
- [ ] Zeitaufwand: X Stunden (vs. 60 Stunden geschätzt)

**Module 10: PropagatorNetworks**
- [ ] 10 Generic<T> Klassen implementiert
- [ ] 40+ Equivalence-Tests passing
- [ ] Zeitaufwand: X Stunden (vs. 50 Stunden geschätzt)

---

## 🚨 Bekannte Herausforderungen

### 1. AutoDiff System (Calculus)
**Problem:** Hardcoded `double` in Tape-based differentiation
**Lösung:** Als P3 (OPTIONAL) markieren, erst bei Bedarf

### 2. FFT (Signals)
**Problem:** MathNet.Numerics FFT ist double-only
**Lösung:**
- IScalarProcessor<T>.Fft() Interface definieren
- Für double: MathNet.Numerics
- Für andere T: DFT Fallback oder Error

### 3. NURBS Curves (Trajectories)
**Problem:** Sehr komplex, 450 LOC
**Lösung:** Viel Zeit einplanen (18-24h), evtl. P2

### 4. Float64SampledTimeSignal (Signals)
**Problem:** MASSIV (1,655 LOC!), 100+ Methods
**Lösung:** 40-60 Stunden einplanen, in Sub-Tasks aufteilen

---

**Dokument Version:** 1.0
**Letzte Aktualisierung:** 2025-10-28
**Status:** PLANNED - Bereit für Implementation nach Phase 2
**Nächste Aktion:** Phase 2 abschließen, dann Module 6A starten

