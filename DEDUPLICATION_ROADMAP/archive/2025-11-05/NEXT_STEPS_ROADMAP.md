# Next Steps Roadmap - Konkrete Aktionen

**Letzte Aktualisierung:** 2025-11-05 (🧪 Test Quality: +16 tests fixed, 98.44% pass rate achieved)
**Aktueller Status:** Phase 1 ✅ COMPLETE | Phase 2 ⏭️ SKIPPED | Phase 3 🚀 IN PROGRESS | Tests 🧪 IMPROVING
**Nächster Schritt:** Fix remaining 14 test failures, then continue Phase 3A Module 6A
**Branch:** Feature/ScalarFloat32

⚠️ **DECISION (2025-10-28):** Phase 2 (Thin Wrapper) ÜBERSPRUNGEN → Direkt zu Phase 3 (Modeling Layer)
- **Grund:** Modeling Layer hat höhere Priorität für produktive Nutzung
- **Phase 2 kann später** erfolgen (Thin Wrapper ist nur Code-Reduktion, keine neue Funktionalität)

---

## ⚠️ DOKUMENTATIONSPFLEGE

**Diese Dateien synchron halten:**
1. **`DEDUPLICATION_ROADMAP.md`** - Gesamt-Roadmap
2. **`NEXT_STEPS_ROADMAP.md`** (dieses Dokument) - Nächste Schritte
3. **`DEDUPLICATION_TASKS.md`** - Detaillierte Tasks

Nach jedem Meilenstein alle drei aktualisieren!

---

## 🧪 Test Quality Improvement - ✅ IN PROGRESS (2025-11-05)

**Ziel:** 100% test pass rate vor Phase 2/3 Fortsetzung

**Session Progress:**
- **Start:** 2438 passing, 30 failing
- **Jetzt:** 2454 passing, 14 failing
- **Fortschritt:** +16 tests fixed ✅
- **Pass Rate:** 97.92% → 98.44% (+0.52%)

**Fixed Issues (3 Commits):**
1. ✅ aec0f601 - AngouriMath parse errors (9 tests) - Variable naming fix
2. ✅ 658aea25 - VGa2D pseudoscalar type bug (5 tests) - KVector type hierarchy fix
3. ✅ 6e4a3bc5 - CatmullRom2D edge cases (2 tests) - Zero-range handling

**Remaining:** 14 failures (CatmullRom2D, Float32, Signals, Trajectories)

**Details:** Siehe `DEDUPLICATION_ROADMAP.md` Section "Test Quality & Bug Fixes"

---

## 🎯 Wo sind wir?

**Phase 0:** ✅ COMPLETE (2025-10-26)
- API-Analyse komplett
- Performance validiert (CGa: Generic 1.27x schneller | XGa: Generic 1.88x langsamer)
- **XGa Performance Root Cause identifiziert** (Aggregate+Lambda, Interface-Indirection)
- 102 Equivalence-Tests passing
- Roadmap erstellt

**Phase 1 Quick Win Optimierungen:** ✅ COMPLETE (2025-10-27)
- Optimierung 1.1: Lambda-Overhead eliminiert (`ScalarProcessorAddUtils.cs`) ✅
- Optimierung 1.2: Type-spezifische Fast-Paths (`XGaMultivectorUnaryBinaryOps.cs`) ✅
- **Ergebnis:** Generic<double> ist jetzt **1.39-2.31x SCHNELLER** als Float64 Specialized!
- **Übertrifft Erwartungen um das 7-fache!** (Erwartet: 10% schneller, Erreicht: 74-131% schneller)
- **Alle 20/20 MultivectorStoragesTests passing** ✅
- **Benchmarks validiert** ✅
- **Phase 2 Migration vollständig UNBLOCKED** ✅

**Weitere Performance-Optimierungen:** ✅ COMPLETE (2025-10-27)
- **Sp Phase 1** (Scalar Product): K-Vector Sp optimiert (`ScalarComposerOperations.cs`)
  - Conformal Sp: 33% → 14% Overhead (19pp Verbesserung) ✅
  - Phase 2B: GradedMultivector Sp-Versuch verursachte 30% Regression → korrekt revertiert
- **Phase 2D** (Lcp/Rcp): Contraction Products optimiert (`ProductGp.cs`)
  - Lcp: 9% → 5.2% Overhead ✅
  - Rcp: ~9% → 6.0% Overhead ✅
- **Bewährtes Pattern validiert:** Type-spezifische Fast-Paths mit lokalem Akkumulator
- **Details:** SP_OPTIMIZATION_ANALYSIS.md, LCP_OPTIMIZATION_ANALYSIS.md

**Phase 1.1:** ✅ COMPLETE (Module 1: XGa Core)
- Task 1.1 (XGaComputedOutermorphism<T>) ✅ COMPLETE
- Task 1.2 (XGaStoredOutermorphism<T>) ✅ COMPLETE
- Task 1.3 (XGaOutermorphismComposerUtils<T>) ✅ COMPLETE
- Task 1.4 (XGaGramSchmidtFrame<T>) ✅ COMPLETE
- Task 1.5 (XGaConformalComposerUtils<T>) ✅ COMPLETE
- **Alle Klassen in Generic** ✅

**Phase 1.2:** ✅ COMPLETE (Module 2: ComplexAlgebra)
- ComplexNumber<T> ✅ ALREADY IMPLEMENTED (947 LOC)
- ComplexAlgebraUtils ✅ ALREADY IMPLEMENTED (243 LOC)
- Float64ComplexUtils ✅ ALREADY IMPLEMENTED (90 LOC)
- **Bonus:** Fixed critical Phase-property bug in ComplexNumber<T>
- **30 Equivalence Tests** ✅ (100% passing)

**Phase 1.3:** ✅ COMPLETE (Module 3: VGA)
- XGaEuclideanGeometrySpace<T> ✅ IMPLEMENTED (41 LOC)
- XGaEuclideanGeometrySpace2D<T> ✅ IMPLEMENTED (42 LOC)
- XGaEuclideanGeometrySpace3D<T> ✅ IMPLEMENTED (67 LOC)
- EuclideanGeometryUtils<T> ⏭️ SKIPPED (P2 - not needed for Generic)
- **11 Equivalence Tests** ✅ (6/6 3D tests passing, 2D blocked by Float64 bug)
- **150 LOC** total
- **Aufwand:** ~2 Stunden (vs. 35-45 Stunden geschätzt)
- **Next:** Phase 1.4 (Module 4: CGA Visualizers)

**Phase 2:** ⏭️ SKIPPED (Thin Wrapper Migration)
- Thin Wrapper Pattern für Float64 → Generic<double>
- ~78,500 LOC Reduktion möglich
- Module 2 (ComplexAlgebra), Module 3 (VGA), Module 5 (LinearAlgebra) mit Performance-Validierung
- CGa/PGa Thin Wrapper empfohlen (1.24-1.27x schneller validiert)
- **Status:** ÜBERSPRUNGEN - Kann später erfolgen (nur Code-Reduktion)
- **Geschätzter Aufwand:** 1-2 Wochen (wenn später durchgeführt)

**Phase 3:** 🚀 IN PROGRESS (Modeling Layer Generic Implementation)
- **KRITISCHER FUND:** 257 Float64-Klassen mit fast KEINEN Generic<T> Äquivalenten
- 5 Module: Trajectories (151), Calculus (~107), Signals (11), Statistics (15), PropagatorNetworks (10)
- 4 Sub-Phasen (3A-3D): Priorität-basiert (P0/P1/P2/P3)
- **Geschätzte Dauer:** 28 Wochen für Core (3A+3B), 41 Wochen für ALLES
- **Details:** [PHASE_3_MODELING_LAYER.md](PHASE_3_MODELING_LAYER.md)
- **Tasks:** [PHASE_3_DEDUPLICATION_TASKS.md](PHASE_3_DEDUPLICATION_TASKS.md)
- **Started:** 2025-10-28 (Module 6A: Trajectories)
- **Optimal Module Order (verified):** 6A (simplified) → 7A → 8 → 7B → 6A (extended)

---

## 🚀 Phase 3A - Module 6A: Trajectories (CURRENT FOCUS)

### 📊 Dependency Analysis Result (2025-10-28)

**VERIFIED:** Die vereinfachte Implementation ist NOTWENDIG wegen zirkulärer Dependencies:

```
ParametricPath3D<T>.GetScalarComponents() → ScalarSignal<T> (Module 8)
                                               ↓
                           ScalarSignal<T> inherits from Trajectory<T> (Module 6A)
                                               ↓
                           ParametricPath3D<T> inherits from Trajectory<...> (Module 6A)
```

**DEADLOCK:** Beide brauchen einander! Lösung = Phased Implementation.

### 🎯 Was implementieren wir JETZT? (Phase 3A Module 6A - Simplified)

**Basis-Klassen (Woche 1):**
1. ✅ **Trajectory<T>** - Generic base class
   - Properties: `TimeRange`, `IsPeriodic`, `MinTime`, `MaxTime`
   - Abstract: `GetValue(T t)`, `IsValid()`, `ToFinite()`, `ToPeriodic()`

2. ✅ **ParametricPath3D<T>** - Generic 3D trajectory (SIMPLIFIED)
   - Inherits: `Trajectory<LinVector3D<T>>`
   - Core methods: `GetValue()`, `GetDerivative1Value()`, `GetDerivative2Value()`, `GetFrame()`
   - ❌ **SKIP:** `GetScalarComponents()`, `FindValueRange()` (brauchen ScalarSignal<T>)

**Einfache Subklasse für Tests (Woche 1):**
3. ✅ **ConstantPath3D<T>** - Constant trajectory
   - Simplest possible subclass (verified in Float64ConstantPath3D.cs)
   - Alle Core-Methods implementierbar

**Tests (Woche 1):**
4. ✅ **8+ Equivalence Tests**
   - Generic<double> vs Float64 für GetValue(), GetDerivative1Value(), GetDerivative2Value(), GetFrame()
   - 100% Pass Rate REQUIRED

**Alle weiteren 59 Subklassen können DANACH implementiert werden:**
- LineSegmentPath3D<T>, CirclePath3D<T>, BezierPath3D<T>, etc.
- Verified: KEINE verwenden GetScalarComponents()!

### 🔄 Was kommt SPÄTER? (Nach Module 8 Signals)

**Phase 3B Module 8 → dann Phase 3A Extended:**
- ScalarSignal<T> implementieren
- GetScalarComponents() zu ParametricPath3D<T> hinzufügen
- FindValueRange() hinzufügen

---

## 📋 Phase 1 Module 1: XGa Core - Historie (COMPLETE)

### Was wurde implementiert:

1. ~~**XGaComputedOutermorphism<T>**~~ ✅ COMPLETE (2025-10-24)
2. ~~**XGaStoredOutermorphism<T>**~~ ✅ COMPLETE (2025-10-25)
3. ~~**XGaOutermorphismComposerUtils<T>**~~ ✅ COMPLETE (2025-10-25)
4. ~~**XGaGramSchmidtFrame<T>**~~ ✅ COMPLETE (2025-10-25)
5. ~~**XGaConformalComposerUtils<T>**~~ ✅ COMPLETE (2025-10-26)

---

## 🚀 OLD: Tag 1: XGaComputedOutermorphism<T> (ARCHIV - COMPLETE)

### Schritt 1: Float64-Implementierung analysieren

**Datei zu lesen:**
```
GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Algebra/
  GeometricAlgebra/Float64/LinearMaps/Outermorphisms/
    XGaFloat64ComputedOutermorphism.cs
```

**Was zu verstehen:**
- Basis-Klasse/Interface
- Constructor(s)
- Properties (Processor, VSpaceDimensions, etc.)
- Methods (OmMap, GetMultivector, etc.)
- Abhängigkeiten (welche anderen Klassen werden verwendet?)

**Kommando:**
```bash
# Datei öffnen und analysieren
code GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/Float64/LinearMaps/Outermorphisms/XGaFloat64ComputedOutermorphism.cs
```

### Schritt 2: Generic-Struktur prüfen

**Ziel-Location für neue Datei:**
```
GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Algebra/
  GeometricAlgebra/Generic/LinearMaps/Outermorphisms/
    XGaComputedOutermorphism.cs  ← NEUE DATEI
```

**Prüfen ob Ordner existiert:**
```bash
ls GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/Generic/LinearMaps/Outermorphisms/
```

### Schritt 3: Implementierung

**Pattern:**
1. Float64-Datei als Basis nehmen
2. `double` → `T` ersetzen
3. `XGaFloat64*` → `Xga*<T>` ersetzen
4. Hardcoded double-Operations → `ScalarProcessor.Method()` verwenden
5. Generic-Interface `IScalarProcessor<T>` integrieren

**Wichtig:**
- Constructor muss `XGaProcessor<T>` akzeptieren
- Alle arithmetischen Operationen über `ScalarProcessor`
- Properties und Methods analog zu Float64

**Beispiel (vereinfacht):**
```csharp
// Float64 Version:
public sealed class XGaFloat64ComputedOutermorphism
{
    public XGaFloat64Processor Processor { get; }

    public XGaFloat64ComputedOutermorphism(XGaFloat64Processor processor)
    {
        Processor = processor;
    }

    public XGaFloat64Vector OmMap(XGaFloat64Vector vector)
    {
        // ... Implementation mit double
    }
}

// Generic Version:
public sealed class XGaComputedOutermorphism<T>
{
    public XGaProcessor<T> Processor { get; }

    public XGaComputedOutermorphism(XGaProcessor<T> processor)
    {
        Processor = processor;
    }

    public XGaVector<T> OmMap(XGaVector<T> vector)
    {
        // ... Implementation mit T und ScalarProcessor
    }
}
```

### Schritt 4: Testing (optional aber empfohlen)

**Test-Datei erstellen:**
```
GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests/
  Algebra/XGaComputedOutermorphismEquivalenceTests.cs  ← NEUE DATEI
```

**Test-Pattern:**
```csharp
[Test]
public void ComputedOutermorphism_OmMapVector_ShouldProduceIdenticalResults()
{
    // Float64
    var processorF64 = XGaFloat64Processor.Euclidean;
    var omF64 = new XGaFloat64ComputedOutermorphism(processorF64);
    var vectorF64 = processorF64.Vector(1, 2, 3);
    var resultF64 = omF64.OmMap(vectorF64);

    // Generic<double>
    var processorGen = XGaProcessor<double>.CreateEuclidean(ScalarProcessorOfFloat64.Instance);
    var omGen = new XGaComputedOutermorphism<double>(processorGen);
    var vectorGen = processorGen.Vector(1.0, 2.0, 3.0);
    var resultGen = omGen.OmMap(vectorGen);

    // Compare
    AssertEquivalent(resultF64, resultGen, 1e-12);
}
```

### Schritt 5: Commit

**Nach erfolgreicher Implementierung:**
```bash
git add GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/Generic/LinearMaps/Outermorphisms/XGaComputedOutermorphism.cs
git add GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests/Algebra/XGaComputedOutermorphismEquivalenceTests.cs
git commit -m "feat(Generic): Add XGaComputedOutermorphism<T> to match Float64 API

- Implement XGaComputedOutermorphism<T> based on XGaFloat64ComputedOutermorphism
- All operations use IScalarProcessor<T> for scalar abstraction
- Add equivalence tests (Generic<double> vs Float64)
- Module 1 (XGa Core) - Task 1/5 complete

Refs: API_COMPARISON line 12"
```

---

## ✅ Tag 2: XGaStoredOutermorphism<T> - COMPLETE (2025-10-25)

**Implementiert:**
- XGaStoredOutermorphism<T> in Generic
- Factory-Methoden für Generic und Float64
- 9 Equivalence Tests (100% passing)

**Bugs gefixt:**
- CRITICAL: OmMapBasisBlade() returned vector part instead of full kVector
- CRITICAL: OmMapBasisBivector() had inverted index-order logic

**Tatsächlicher Aufwand:** ~4 Stunden (inkl. Bug-Fixes)

---

## ✅ Tag 3: XGaOutermorphismComposerUtils<T> - COMPLETE (2025-10-25)

**Implementiert:**
- Extended XGaOutermorphismComposerUtils<T> with ColumnsToOutermorphism<T>
- Adds matrix-to-outermorphism conversion functionality
- 6 Equivalence Tests (100% passing)

**Methods:**
- CreateComputedOutermorphism<T> (already existed)
- CreateStoredOutermorphism<T> (added in Task 1.2)
- ColumnsToOutermorphism<T> (added in Task 1.3) ← NEW

**Tatsächlicher Aufwand:** ~2 Stunden

---

## ✅ Tag 4: XGaGramSchmidtFrame<T> - COMPLETE (2025-10-25)

**Implementiert:**
- XGaGramSchmidtFrame<T> using classical modified Gram-Schmidt algorithm
- Fully generic implementation with IScalarProcessor<T> (no MathNet.Numerics dependency)
- Works with ANY scalar type (double, float, rational, symbolic, etc.)
- 9 Equivalence Tests (100% passing)

**Key Design Decisions:**
- **Algorithm:** Classical modified Gram-Schmidt (not QR decomposition)
  - Float64 uses MathNet.Numerics QR (only works with double)
  - Generic uses direct orthogonalization (works with all T)
- **Universality:** No external dependencies, pure Generic<T> implementation
- **Methods:** Create(), GetDirection(), GetCurvature(), GetDarbouxBlade(), GetDarbouxBivector(), CleanNorms()

**Tatsächlicher Aufwand:** ~4 Stunden (inkl. Algorithm-Implementierung)

**Bugs fixed during implementation:**
- None - clean implementation on first try after fixing Scalar<T> conversion issues

---

## 🚀 Tag 5: XGaConformalComposerUtils<T>

**Conformal GA Utilities**

```
Float64: GeometricAlgebra/Float64/Spaces/Conformal/XGaFloat64ConformalComposerUtils.cs
Generic: GeometricAlgebra/Generic/Spaces/Conformal/XGaConformalComposerUtils.cs (NEU)
```

**Geschätzter Aufwand:** 2-3 Stunden

---

## 🚀 Tag 5-6 (Optional): ToTuple() Extensions

**Convenience Feature (niedrige Priorität)**

```
Float64: GeometricAlgebra/Float64/Multivectors/XGaFloat64Scalar.cs (ToTuple methods)
Generic: GeometricAlgebra/Generic/Multivectors/XGaScalar.cs (NEU: ToTuple methods)
```

**Was:** Extension methods für Tuple-Konversionen
- `XGaScalar<T>.ToTuple()` → `Tuple<T>`
- `XGaVector<T>.ToTuple()` → `Tuple<T, T, ...>`
- etc.

**Geschätzter Aufwand:** 1-2 Stunden

**Entscheidung:** Kann übersprungen werden wenn Zeit knapp ist (P2 Feature)

---

## ✅ Module 1 Complete Wenn:

- [x] XGaComputedOutermorphism<T> implementiert und getestet ✅
- [x] XGaStoredOutermorphism<T> implementiert und getestet ✅
- [x] XGaOutermorphismComposerUtils<T> implementiert und getestet ✅
- [x] XGaGramSchmidtFrame<T> implementiert und getestet ✅
- [ ] XGaConformalComposerUtils<T> implementiert und getestet ← NEXT
- [ ] ToTuple() extensions (optional)
- [x] Alle Tests passing ✅
- [x] Dokumentation aktualisiert ✅

**Dann:** Modul 1 als "Phase 1 Complete ✅" markieren

**Weiter zu:** Module 2 (ComplexAlgebra)

---

## 📅 Workflow für jeden Tag

### Morgen:
1. Roadmap-Dokumente lesen
2. Aktuelles Task in `DEDUPLICATION_TASKS.md` checken
3. Float64-Implementierung analysieren

### Während Arbeit:
1. Code implementieren (Generic)
2. Tests schreiben
3. Tests laufen lassen
4. Iterieren bis alle Tests passing

### Abend:
1. Commit mit klarer Message
2. Task in `DEDUPLICATION_TASKS.md` abhaken
3. Nächsten Tag planen

### Wöchentlich:
1. Alle 3 Roadmap-Dokumente aktualisieren
2. Fortschritt vs. Schätzung prüfen
3. Zeitplan anpassen wenn nötig

---

## ⏭️ Module 4: CGA Visualizers - SKIPPED (P3 - Optional)

**Status:** Übersprungen - Nicht benötigt für Generic-First Strategy

**Begründung (Analyse 2025-10-25):**
- ❌ **0 Verwendungen** in Applications/Samples
- ❌ **0 Tests** (Unit oder Integration)
- ✅ **5,459 LOC** in 7 Dateien (Dead Code)
- ⏱️ **80-120 Stunden** Aufwand für ungenutzten Code

**Entscheidung:** Module 4 wird **NICHT** implementiert.

**Implementierung nur wenn:**
- Konkrete Nutzungsszenarien entstehen
- Tests geschrieben werden
- Explizite Anforderung vom Maintainer

---

## 🎯 Phase 1 ✅ COMPLETE (2025-10-25)

**Abgeschlossene Module:**
- ✅ Module 1: XGa Core (5 Klassen, 39 Tests, ~14h)
- ✅ Module 2: ComplexAlgebra (bereits vorhanden, Bug-Fix, ~1h)
- ✅ Module 3: VGA (3 Klassen, 11 Tests, ~2h)
- ⏭️ Module 4: CGA Visualizers (SKIPPED - P3 Optional)
- ✅ Module 5: LinearAlgebra Details (6 Properties, 6 Tests, ~3h)

**Gesamt-Aufwand Phase 1:** ~20 Stunden (vs. 6-8 Wochen geschätzt!)

**Ergebnis:** Generic<T> hat jetzt 100% API-Parität mit Float64 für alle P0/P1/P2 Features.

---

## ⚠️ KRITISCHE WARNUNG: XGa Performance-Widerspruch (2025-10-26)

**Status:** 🚨 **Phase 2 XGa Migration BLOCKIERT - Erfordert weitere Untersuchung**

**Benchmark-Ergebnisse widersprechen CGa-Performance:**

| Level | Float64 Spec | Generic<float> | Generic<double> | Fazit |
|-------|--------------|----------------|-----------------|-------|
| **CGa (High-Level)** | Baseline | **1.24x schneller** ✅ | **1.27x schneller** ✅ | Generic gewinnt! |
| **XGa (Low-Level)** | Baseline | **1.85x langsamer** ⚠️ | **1.88x langsamer** ⚠️ | Float64 gewinnt! |

**Kernaussage:**
- CGa-Benchmarks zeigten: Generic ist **schneller** → Thin Wrapper sollte funktionieren
- XGa-Benchmarks zeigen: Generic ist **1.15-2.62x LANGSAMER** → Thin Wrapper würde Performance verschlechtern!

**Hypothese:** Low-Level XGa-Operationen leiden unter `IScalarProcessor<T>` Indirection-Overhead, während High-Level CGa-Operationen von besserer JIT-Optimierung profitieren.

**Auswirkungen auf Phase 2:**
1. ❌ **XGa Core (Module 1):** Thin Wrapper Migration NICHT empfohlen (1.15-2.62x Regression!)
2. ✅ **CGa/PGa:** Thin Wrapper Migration weiterhin empfohlen (1.24-1.27x Speedup validiert)
3. ⚠️ **ComplexAlgebra/VGA:** Performance-Validierung erforderlich vor Migration

**Nächste Schritte (ZWINGEND vor Phase 2.1):**
- [ ] XGa Float64 vs Generic<double> profilen (exakte Bottlenecks identifizieren)
- [ ] Float64 XGa Source auf SIMD/AVX2-Usage prüfen
- [ ] `IScalarProcessor<T>` Call-Overhead messen
- [ ] Aggressive Inlining-Hints testen

**Dokumentation:** Siehe `XGA_NORMALIZATION_BENCHMARK_RESULTS.md` für vollständige Analyse

---

## 🚀 Phase 2: Thin Wrapper Migration - NÄCHSTER SCHRITT

**⚠️ STRATEGIE-ANPASSUNG ERFORDERLICH (siehe Warnung oben)**

**Ursprüngliches Ziel:** Float64-Klassen als dünne Wrapper um Generic<double> neu schreiben

**Vorteile (CGa/PGa validiert):**
- ~78,500 LOC Reduktion
- 100% Rückwärtskompatibilität
- Bewährtes Pattern (siehe Float32)
- Einfacher zu warten
- **1.24-1.27x Performance-Vorteil** (CGa-Level)

**Neue Strategie - Hybride Migration:**
- Module 1: XGa Core → **SKIP (Performance-Regression)** ⏭️
- Module 2: ComplexAlgebra → Performance-Validierung erforderlich ⚠️
- Module 3: VGA → Performance-Validierung erforderlich ⚠️
- Module 5: LinearAlgebra → Performance-Validierung erforderlich ⚠️
- **CGa/PGa (nicht in Modules):** Thin Wrapper empfohlen ✅

**Geschätzter Aufwand:** 1-2 Wochen (angepasst nach Performance-Validierung)

**Start:** NACH Performance-Untersuchungen für XGa

---

## 📋 Phase 3: Modeling Layer - Erste Schritte

**Status:** 📋 PLANNED (Start nach Phase 2 Complete)

**Umfang:** 257 verifizierte Float64-Klassen mit fast KEINEN Generic<T> Äquivalenten

### Übersicht

**Vollständige Dokumentation:**
- **[PHASE_3_MODELING_LAYER.md](PHASE_3_MODELING_LAYER.md)** - Komplette Planung mit Timeline (870+ Zeilen)
- **[PHASE_3_DEDUPLICATION_TASKS.md](PHASE_3_DEDUPLICATION_TASKS.md)** - Task-by-Task Checkliste (1000+ Zeilen)

**Module und Prioritäten:**

| Modul | Float64 | Generic | Fehlend | Geschätzt | Priorität |
|-------|---------|---------|---------|-----------|-----------|
| **6A: Trajectories Vectors3D** | 60 | 0 | **60** | 8 Wochen | **P0** ← START HIER |
| **6B: Trajectories Vectors2D** | 40 | 0 | **40** | 5 Wochen | **P1** |
| **6C: Trajectories Scalars** | 40 | 0 | **40** | 5 Wochen | **P1** |
| **6D: Trajectories Others** | 11 | 0 | **11** | 2 Wochen | **P2** |
| **7A: Calculus Core** | 35 | 3 | **32** | 7 Wochen | **P0** |
| **7B: Calculus Advanced** | 35+ | 0 | **35+** | 8 Wochen | **P2/P3** |
| **8: Signals** | 14 | 3 | **11** | 2.5 Wochen | **P1** |
| **9: Statistics** | 15 | 0 | **15** | 1.5 Wochen | **P2** |
| **10: PropagatorNetworks** | 10 | 0 | **10** | 1.25 Wochen | **P2** |

**Phase 3A (P0 - CRITICAL):** Module 6A + 7A = 16 Wochen
**Phase 3B (P1 - IMPORTANT):** Module 6B + 6C + 8 = 12 Wochen
**Phase 3C (P2 - NICE-TO-HAVE):** Module 6D + 9 + 10 = 5 Wochen
**Phase 3D (P3 - OPTIONAL):** Module 7B = 8 Wochen

---

## 🔴 KRITISCHER WORKFLOW für ALLE 257 Klassen (ZWINGEND!)

**JEDE Klasse folgt diesem Pattern:**

```
1. ✅ IMPLEMENTIERE Generic<T> basierend auf Float64
2. ✅ SCHREIBE 10+ Equivalence Tests (Generic<double> vs Float64)
3. ✅ STELLE SICHER 100% Pass Rate
4. ✅ NUR DANN Commit
5. ✅ Weiter zur nächsten Klasse
```

**❌ NIEMALS committen ohne:**
- Mindestens 10+ Equivalence Tests
- 100% Pass Rate (alle Tests grün!)
- Equivalence nachgewiesen (Generic<double> = Float64)

**✅ Bewährtes Pattern aus Phase 1:**
- 97.92% Test Pass Rate durch dieses strikte Pattern
- Jede Klasse mit Tests BEFORE Commit
- Beispiele: XGaGramSchmidtFrame<T> (9 Tests ✅), ComplexNumber<T> (30 Tests ✅)

**Siehe:**
- [PHASE_3_MODELING_LAYER.md](PHASE_3_MODELING_LAYER.md) - Workflow-Details + Test-Pattern Beispiel
- [PHASE_3_DEDUPLICATION_TASKS.md](PHASE_3_DEDUPLICATION_TASKS.md) - Task-Checklisten mit Workflow

---

## 🚀 Module 6A: Trajectories Vectors3D - Tag 1

**Start nach:** Phase 2 Complete

**Ziel:** Parametrische 3D-Kurven Generic implementieren (60 Klassen, 8 Wochen)

### Schritt 1: Float64-Basis-Klasse analysieren

**Datei zu lesen:**
```
GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Modeling/
  Trajectories/Vectors3D/Float64Path3D.cs
```

**Was zu verstehen:**
- Abstract Basis-Klasse für alle 3D-Trajektorien
- Properties: `IScalarProcessor<double>`, `ScalarRange`, `IsPeriodic`
- Methods: `GetPoint(t)`, `GetTangent(t)`, `GetLength()`, `GetArcLengthSamples()`
- Dependencies: `LinFloat64Vector3D`, `Float64ScalarRange`, `IParametricCurve3D`

**Kommando:**
```bash
# Datei öffnen und analysieren
code GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Modeling/Trajectories/Vectors3D/Float64Path3D.cs

# Unterklassen finden
find GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Modeling/Trajectories/Vectors3D -name "*Float64*.cs" | head -10
```

### Schritt 2: Generic-Struktur erstellen

**Ziel-Location für neue Datei:**
```
GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Modeling/
  Trajectories/Vectors3D/Generic/
    ParametricPath3D.cs  ← NEUE DATEI (Basis-Klasse)
```

**Prüfen ob Ordner existiert:**
```bash
# Ordner erstellen falls nicht vorhanden
mkdir -p "GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Modeling/Trajectories/Vectors3D/Generic"
ls GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Modeling/Trajectories/Vectors3D/
```

**WICHTIG:** Verzeichnisstruktur prüfen
```bash
# Aktuelle Struktur zeigen
tree GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Modeling/Trajectories/Vectors3D -L 2
```

### Schritt 3: Implementierung - ParametricPath3D<T>

**Pattern:**
1. Float64Path3D als Basis nehmen (~200 LOC)
2. `double` → `T` ersetzen
3. `LinFloat64Vector3D` → `LinVector3D<T>` ersetzen
4. `Float64ScalarRange` → `ScalarRange<T>` ersetzen
5. Hardcoded double-Operations → `ScalarProcessor.Method()` verwenden
6. Generic-Interface `IScalarProcessor<T>` integrieren

**Beispiel (vereinfacht):**
```csharp
// Float64 Version:
public abstract class Float64Path3D : IParametricCurve3D
{
    public IScalarProcessor<double> ScalarProcessor { get; }
    public Float64ScalarRange ParameterRange { get; }

    public abstract LinFloat64Vector3D GetPoint(double t);
    public abstract LinFloat64Vector3D GetTangent(double t);

    public double GetLength()
    {
        // Hardcoded double integration
        return IntegrateLength(0.0, 1.0);
    }
}

// Generic Version:
public abstract class ParametricPath3D<T> : IParametricCurve3D<T>
{
    public IScalarProcessor<T> ScalarProcessor { get; }
    public ScalarRange<T> ParameterRange { get; }

    public abstract LinVector3D<T> GetPoint(Scalar<T> t);
    public abstract LinVector3D<T> GetTangent(Scalar<T> t);

    public Scalar<T> GetLength()
    {
        // Generic integration using ScalarProcessor
        return IntegrateLength(
            ScalarProcessor.Zero,
            ScalarProcessor.One
        );
    }
}
```

### Schritt 4: Testing (ZWINGEND vor Commit!)

**🔴 KRITISCH:** NIEMALS committen ohne 100% passing Tests!

**Test-Datei erstellen:**
```
GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests/
  Modeling/Trajectories/ParametricPath3DEquivalenceTests.cs  ← NEUE DATEI
```

**Test-Pattern (Equivalence Tests: Generic<double> vs Float64):**
```csharp
[TestFixture]
public class ParametricPath3DEquivalenceTests
{
    [Test]
    public void LinearPath3D_GetPoint_ShouldProduceIdenticalResults()
    {
        // Float64
        var pathF64 = new Float64LinearPath3D(
            new LinFloat64Vector3D(0, 0, 0),
            new LinFloat64Vector3D(1, 1, 1)
        );
        var pointF64 = pathF64.GetPoint(0.5);

        // Generic<double>
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var pathGen = new LinearPath3D<double>(
            scalarProcessor,
            new LinVector3D<double>(0, 0, 0),
            new LinVector3D<double>(1, 1, 1)
        );
        var pointGen = pathGen.GetPoint(scalarProcessor.GetScalarFromNumber(0.5));

        // Equivalence-Vergleich (WICHTIG: Toleranz für Floating-Point!)
        Assert.That(pointGen.X.ScalarValue, Is.EqualTo(pointF64.X).Within(1e-12));
        Assert.That(pointGen.Y.ScalarValue, Is.EqualTo(pointF64.Y).Within(1e-12));
        Assert.That(pointGen.Z.ScalarValue, Is.EqualTo(pointF64.Z).Within(1e-12));
    }

    [Test]
    public void BezierPath3D_GetTangent_ShouldProduceIdenticalResults()
    {
        // Test Bezier curves (most complex trajectory type)
        // ... similar pattern
    }

    // Mindestens 10+ Tests für ALLE Public Methods!
}
```

**Minimum-Anforderung:**
- **10+ Equivalence Tests** (Generic<double> vs Float64)
- **Alle Public Methods** testen
- **Floating-Point Toleranz** verwenden (1e-12)

**Tests laufen lassen:**
```bash
# Spezifische Test-Klasse
dotnet test --filter "ParametricPath3DEquivalenceTests"

# Mit verbose output
dotnet test --filter "ParametricPath3DEquivalenceTests" --verbosity normal
```

**✅ NUR weitermachen wenn:**
- ALLE Tests grün ✅
- 100% Pass Rate
- Equivalence nachgewiesen

### Schritt 5: Verification & Commit

**🔴 STOPP! Vor Commit diese Checkliste durchgehen:**

- [ ] ✅ Implementation vollständig (alle Methods implementiert)
- [ ] ✅ Mindestens 10+ Equivalence Tests geschrieben
- [ ] ✅ Tests laufen gelassen: `dotnet test --filter "ParametricPath3DEquivalenceTests"`
- [ ] ✅ **100% Pass Rate** (ALLE Tests grün!)
- [ ] ✅ Equivalence nachgewiesen (Generic<double> = Float64)
- [ ] ✅ Code reviewed (keine TODOs, keine Debug-Ausgaben)

**NUR wenn ALLE Punkte ✅ dann committen:**

```bash
# Files adden
git add GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Modeling/Trajectories/Vectors3D/Generic/ParametricPath3D.cs
git add GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.UnitTests/Modeling/Trajectories/ParametricPath3DEquivalenceTests.cs

# Commit mit klarer Message
git commit -m "feat(Generic): Add ParametricPath3D<T> + 10 Equivalence Tests ✅

- Implement ParametricPath3D<T> based on Float64Path3D (~200 LOC)
- All operations use IScalarProcessor<T> for scalar abstraction
- Support for LinVector3D<T>, ScalarRange<T>
- Add 10+ equivalence tests (Generic<double> vs Float64)
- All tests passing ✅ (100% Pass Rate)
- Phase 3 Module 6A (Trajectories Vectors3D) - Task 1/60 complete

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>"
```

**❌ NIEMALS committen wenn:**
- Tests fehlen
- Tests failing (auch nur 1!)
- Equivalence nicht nachgewiesen
- TODOs oder Debug-Code noch im Code

---

## ✅ Module 6A Complete Wenn:

- [ ] **ParametricPath3D<T>** (Basis-Klasse) implementiert ← START HIER
- [ ] **LinearPath3D<T>** implementiert
- [ ] **BezierPath3D<T>** implementiert (komplexeste Klasse!)
- [ ] **CircularPath3D<T>** implementiert
- [ ] **HelixPath3D<T>** implementiert
- [ ] ... (55 weitere Klassen) - siehe PHASE_3_DEDUPLICATION_TASKS.md
- [ ] **AdaptiveCurveSampler3D<T>** implementiert (Arc-Length Parameterization)
- [ ] Alle 60 Klassen haben 10+ Equivalence Tests
- [ ] Alle Tests passing
- [ ] Dokumentation aktualisiert

**Geschätzte Dauer:** 8 Wochen (320 Stunden)

**Dann:** Weiter zu Module 6B (Trajectories Vectors2D) oder Module 7A (Calculus Core)

---

## 🎯 Nach Module 1: Phase 1.2 (ARCHIV - COMPLETE)

**Module 2: ComplexAlgebra (GESAMTES Modul neu)**

**Vorbereitung (während Module 1):**
- Float64 ComplexAlgebra-Modul analysieren (4 Dateien)
- Architektur verstehen
- Test-Strategie planen

**Start:** Nach Module 1 Complete (Tag 6-7)

**Geschätzter Aufwand:** 1-2 Wochen (komplexere Algebra-Logik)

---

## 🔧 Hilfreiche Kommandos

### Codebase durchsuchen:
```bash
# Float64-Implementierung finden
find GeometricAlgebraFulcrumLib -name "*Float64ComputedOutermorphism*"

# Generic-Äquivalent prüfen
find GeometricAlgebraFulcrumLib -path "*/Generic/*" -name "*ComputedOutermorphism*"

# Alle Outermorphism-Dateien
find GeometricAlgebraFulcrumLib -path "*/Outermorphisms/*" -name "*.cs"
```

### Tests laufen lassen:
```bash
# Alle Tests
cd GeometricAlgebraFulcrumLib
dotnet test

# Spezifische Test-Klasse
dotnet test --filter "XGaComputedOutermorphismEquivalenceTests"

# Verbose output
dotnet test --verbosity normal
```

### Build prüfen:
```bash
# Build entire solution
dotnet build GeometricAlgebraFulcrumLib.sln

# Build mit Warnings als Errors
dotnet build GeometricAlgebraFulcrumLib.sln --warnaserror
```

---

## 📚 Referenzen für Implementierung

**Float32 als Pattern-Referenz:**
- `XGaFloat32Processor.cs` - Thin Wrapper Beispiel
- Zeigt wie Generic<float> wrapped wird
- Analog für Generic<double> machbar

**Generic-Patterns:**
- `XGaProcessor<T>.cs` - Generic Processor Pattern
- `XGaVector<T>.cs` - Generic Multivector Pattern
- `XGaPureRotor<T>.cs` - Generic LinearMap Pattern

**Scalar Processor Usage:**
```csharp
// Addition
var sum = processor.ScalarProcessor.Add(a, b);

// Multiplication
var product = processor.ScalarProcessor.Times(a, b);

// Norm
var norm = processor.ScalarProcessor.Sqrt(
    processor.ScalarProcessor.Add(
        processor.ScalarProcessor.Times(x, x),
        processor.ScalarProcessor.Times(y, y)
    )
);
```

---

## ❓ Fragen während Implementierung?

**Wenn unklar:**
1. Float64-Implementierung nochmal lesen
2. Ähnliche Generic-Klassen als Referenz nehmen
3. Tests schreiben um Behavior zu verstehen
4. Dokumentation in CLAUDE.md konsultieren

**Bugs während Implementierung:**
- Nur Bugs in aktuellem Modul fixen
- Bugs in anderen Modulen notieren, später fixen

---

**Nächste Aktion:** Float64 XGaFloat64ComputedOutermorphism.cs öffnen und analysieren

**Erstes Kommando:**
```bash
code GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/Float64/LinearMaps/Outermorphisms/XGaFloat64ComputedOutermorphism.cs
```

---

## 📊 Zusammenfassung: Was kommt als nächstes?

**Sofort (Phase 2):**
- XGa Performance-Untersuchungen durchführen
- Thin Wrapper Migration für CGa/PGa (validiert schneller)
- ComplexAlgebra, VGA mit Performance-Tests
- Geschätzt: 1-2 Wochen

**Danach (Phase 3A - P0 CRITICAL):**
- Module 6A: Trajectories Vectors3D (60 Klassen, 8 Wochen)
- Module 7A: Calculus Core (32 Klassen, 7 Wochen)
- Geschätzt: 16 Wochen

**Später (Phase 3B-D):**
- Restliche Trajectories Module (6B, 6C, 6D: 91 Klassen, 12 Wochen)
- Signals (11 Klassen, 2.5 Wochen)
- Statistics (15 Klassen, 1.5 Wochen)
- PropagatorNetworks (10 Klassen, 1.25 Wochen)
- Calculus Advanced (35+ Klassen, 8 Wochen - OPTIONAL)
- Geschätzt: 25 Wochen

**GESAMT-Timeline:**
- Phase 2: 1-2 Wochen
- Phase 3A: 16 Wochen (CRITICAL)
- Phase 3B+C: 17 Wochen (IMPORTANT + NICE-TO-HAVE)
- Phase 3D: 8 Wochen (OPTIONAL)
- **Total ohne Optional:** 34-35 Wochen
- **Total mit Optional:** 42-43 Wochen

---

*Dokument maintained by: Claude Code*
*Last verified: 2025-10-28*
*Branch: Feature/ScalarFloat32*
*Phase 3 Planning: COMPLETE ✅*
