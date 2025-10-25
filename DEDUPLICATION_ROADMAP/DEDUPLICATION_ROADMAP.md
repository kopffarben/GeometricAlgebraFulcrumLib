# Code Deduplication Roadmap - Generic-First Strategy

**Ziel:** Generic-Implementierung auf 100% Float64-Kompatibilität bringen, dann Float64 → Thin Wrapper migrieren.

**Status:** Phase 1.1 COMPLETE ✅ (Module 1: XGa Core - All Tasks Complete)
**Erstellt:** 2025-10-23 (Komplette Neustrukturierung basierend auf aktuellen API-Daten)
**Letzte Aktualisierung:** 2025-10-25
**Geschätzte Dauer:** 8-11 Wochen (6-8 Wochen Phase 1 + 2-3 Wochen Phase 2)
**LOC-Reduktion (erwartet):** ~78,500 Zeilen

---

## ⚠️ WICHTIG: Dokumentationspflege

**Diese Dateien müssen synchron gehalten werden:**
1. **`DEDUPLICATION_ROADMAP.md`** (dieses Dokument) - Gesamt-Roadmap und Strategie
2. **`NEXT_STEPS_ROADMAP.md`** - Nächste konkrete Schritte
3. **`DEDUPLICATION_TASKS.md`** - Detaillierte Tasks pro Modul

Nach jedem Meilenstein: Alle drei Dokumente aktualisieren!

**Quick Check:** Siehe `_Status.md` für Kurzübersicht.

---

## 🎯 Strategie: Generic-First (Option A/B Hybrid)

### Grundprinzip

**Float64 ist DEPRECATED** und wird **NICHT erweitert**.

**Nur Generic wird erweitert**, um 100% Float64-Kompatibilität zu erreichen.

### Phasen pro Modul

```
Phase 1 (pro Modul): Generic erweitern
├── Alle fehlenden Features aus Float64 in Generic implementieren
├── Generic wird Superset: Generic ⊇ Float64
└── Bugs nur fixen wenn wir an diesem Modul arbeiten

Phase 2 (pro Modul): Thin Wrapper Migration
├── Float64 als dünner Wrapper um Generic<double> neu schreiben
├── 100% kompatibel zu bestehendem Float64-Code
└── Kein Breaking Change für Nutzer
```

### Warum dieser Ansatz funktioniert

1. **Generic ⊇ Float64** nach Phase 1 → Thin Wrapper ist möglich
2. **Float64 bleibt funktionsfähig** → Kein Breaking Change
3. **Modulweise Migration** → Schrittweise, testbar, sicher
4. **Bewährtes Muster** → Float32 zeigt: Es funktioniert!

---

## 📊 Datenquellen

**Alle Informationen basieren auf:**
- `API_COMPARISON_FLOAT64_VS_GENERIC_WITH_NAMESPACES.md`
- `API_COMPARISON_REPORT.md`

**Spalte 2** = Was fehlt in Generic (RELEVANT)
**Spalte 3** = Was fehlt in Float64 (IGNORIEREN - Float64 ist deprecated)

---

## 🗺️ Module nach Top-Down Priorität (Option B)

### Module 1: XGa Core ✅ COMPLETE
**Priorität:** P0 (Fundament)
**Status:** 100% complete - Alle 5 Klassen implementiert und getestet
**Aufwand:** ~14 Stunden tatsächlich (18-24 Stunden geschätzt) - 5 Klassen, 39 Tests, 496 LOC

#### Was fehlt in Generic (aus Spalte 2):

1. ~~**XGaComputedOutermorphism<T>** class~~ ✅ COMPLETE (2025-10-24)
   - Float64: `XGaFloat64ComputedOutermorphism` existiert
   - Generic: **IMPLEMENTIERT** in Task 1.1
   - Zeile 12 in API_COMPARISON
   - Includes: 6 Equivalence Tests (all passing)
   - **BONUS:** Fixed critical IndexSet.GetSubsets() bug (EmptySet singleton issue)

2. ~~**XGaStoredOutermorphism<T>** class~~ ✅ COMPLETE (2025-10-25)
   - Float64: `XGaFloat64StoredOutermorphism` existiert
   - Generic: **IMPLEMENTIERT** in Task 1.2
   - Zeile 24 in API_COMPARISON
   - Includes: 9 Equivalence Tests (all passing)
   - **BONUS:** Fixed 2 critical Float64 bugs (OmMapBasisBlade, OmMapBasisBivector)

3. ~~**XGaOutermorphismComposerUtils<T>** static class~~ ✅ COMPLETE (2025-10-25)
   - Float64: `XGaFloat64OutermorphismComposerUtils` existiert
   - Generic: **ERWEITERT** in Task 1.3 (ColumnsToOutermorphism<T> hinzugefügt)
   - Zeile 25 in API_COMPARISON
   - Includes: 6 Equivalence Tests for ColumnsToOutermorphism (all passing)

4. ~~**XGaGramSchmidtFrame<T>** class~~ ✅ COMPLETE (2025-10-25)
   - Float64: `XGaFloat64GramSchmidtFrame` existiert (uses MathNet.Numerics QR)
   - Generic: **IMPLEMENTIERT** in Task 1.4 (classical modified Gram-Schmidt algorithm)
   - Zeile 27 in API_COMPARISON
   - Includes: 9 Equivalence Tests (all passing)
   - **Key:** Fully generic implementation with no external dependencies

5. ~~**XGaConformalComposerUtils<T>** static class~~ ✅ COMPLETE (2025-10-25)
   - Float64: `XGaFloat64ConformalComposerUtils` existiert (empty placeholder)
   - Generic: **IMPLEMENTIERT** in Task 1.5 (matching empty placeholder)
   - Zeile 30 in API_COMPARISON
   - Includes: 5 Structural Equivalence Tests (all passing)
   - **Note:** Both classes are empty placeholders for future CGA composition utilities

6. **ToTuple()** extension methods (SKIPPED - P2 niedrige Priorität)
   - Zeile 13 in API_COMPARISON
   - **Decision:** Optional convenience feature, skipped to focus on higher priority modules

#### Bugs gefunden & gefixt in Task 1.2:
- ✅ **CRITICAL**: XGaFloat64StoredOutermorphism.OmMapBasisBlade() returned kVector.GetVectorPart() instead of kVector
- ✅ **CRITICAL**: XGaFloat64StoredOutermorphism.OmMapBasisBivector() had inverted index-order logic

#### ✅ Module 1 COMPLETE:
- **5 Klassen implementiert:** XGaComputedOutermorphism<T>, XGaStoredOutermorphism<T>, XGaOutermorphismComposerUtils<T>, XGaGramSchmidtFrame<T>, XGaConformalComposerUtils<T>
- **39 Tests:** 10 + 9 + 6 + 9 + 5 = 39 tests (100% passing)
- **496 LOC:** 126 + 157 + 13 + 189 + 11 = 496 LOC
- **Bugs gefixt:** 3 critical bugs (1 IndexSet, 2 Float64)
- **Aufwand:** ~14 Stunden (58% der geschätzten 18-24 Stunden)

**Next:** Phase 1.2 - Module 2 (ComplexAlgebra)

---

### Module 2: ComplexAlgebra ⏸️ NOT STARTED
**Priorität:** P0 (Wichtig für Algebra)
**Status:** 🚨 0% Generic - **GESAMTES MODUL FEHLT**
**Aufwand:** 1-2 Wochen

#### Was zu implementieren (Zeile 63):

**GESAMTES Generic-Modul erstellen:**

1. **ComplexScalar<T>** generic class
   - Basis-Klasse für komplexe Skalare
   - Unterstützung für IScalarProcessor<T>

2. **ComplexUtils<T>** generic utility class
   - Hilfsfunktionen für komplexe Zahlen

3. **ComplexAlgebraUtils<T>** generic algebra utilities
   - Algebraische Operationen

4. **All complex number operations**
   - Addition, Subtraktion, Multiplikation, Division
   - Konjugation, Betrag, Argument
   - Polar ↔ Kartesisch Konversion

**Referenz:** Float64-Modul komplett (4 Dateien):
- `Float64ComplexScalar`
- `Float64ComplexUtils`
- `ComplexAlgebraUtils`
- `ComplexNumber<T>` (generic wrapper exists)

#### Bugs zu fixen: Keine für ComplexAlgebra Generic (neue Implementierung)

#### Geschätzter Aufwand: 1-2 Wochen (4 Dateien, komplexe Algebra-Logik)

---

### Module 3: VGA (Vector GA) ⏸️ NOT STARTED
**Priorität:** P0 (Wichtig für Modeling)
**Status:** 🚨 0% Generic - **GESAMTES MODUL FEHLT**
**Aufwand:** 1 Woche

#### Was zu implementieren (Zeile 103):

**GESAMTES Generic-Modul erstellen:**

1. **RGaEuclideanGeometrySpace<T>** base class
   - Basis-Klasse für euklidische Geometrie-Spaces

2. **RGaEuclideanGeometrySpace2D<T>** 2D specialization
   - 2D-spezifische Geometrie-Operationen

3. **RGaEuclideanGeometrySpace3D<T>** 3D specialization
   - 3D-spezifische Geometrie-Operationen

4. **EuclideanGeometryUtils<T>** generic utilities
   - Hilfsfunktionen für euklidische Geometrie

**Referenz:** Float64-Modul komplett (4 Dateien):
- `RGaEuclideanGeometrySpace`
- `RGaEuclideanGeometrySpace2D`
- `RGaEuclideanGeometrySpace3D`
- `EuclideanGeometryUtils`

#### Bugs zu fixen: Keine für VGA Generic (neue Implementierung)

#### Geschätzter Aufwand: 1 Woche (4 Dateien, Geometrie-Operationen)

---

### Module 4: CGA Visualizers ⏸️ NOT STARTED
**Priorität:** P1 (Visualization)
**Status:** 🚨 0% Generic - **GESAMTES MODUL FEHLT**
**Aufwand:** 2-3 Wochen

#### Was zu implementieren (Zeile 83):

**GESAMTES Generic-Visualizer-Modul erstellen:**

1. **CGaVisualizer<T>** generic visualizer
   - Haupt-Visualizer-Klasse

2. **CGaVisualizerDirectionStyle<T>** class
   - Styling für Richtungen

3. **CGaVisualizerElementStyle<T>** class
   - Styling für Elemente

4. **CGaVisualizerFlatStyle<T>** class
   - Styling für flache Objekte (Punkte, Linien, Ebenen)

5. **CGaVisualizerRoundStyle<T>** class
   - Styling für runde Objekte (Kreise, Sphären)

6. **CGaVisualizerTangentStyle<T>** class
   - Styling für tangentiale Objekte

7. **CGaVisualizerUtils<T>** utilities
   - Hilfsfunktionen für Visualisierung

**Integration (Zeile 67, 78):**
- **CGaGeometricSpace5D<T>** muss erweitert werden:
  - `Visualizer` property
  - `VisualizerAnimationComposer` property
  - `VisualizerKaTeXComposer` property
  - `VisualizerSceneComposer` property

- **CGaBlade<T>** muss erweitert werden:
  - `Visualizer` property (returns `CGaVisualizer<T>`)

**Referenz:** Float64-Modul komplett (7 Dateien):
- `CGaFloat64Visualizer`
- `CGaFloat64VisualizerDirectionStyle`
- `CGaFloat64VisualizerElementStyle`
- `CGaFloat64VisualizerFlatStyle`
- `CGaFloat64VisualizerRoundStyle`
- `CGaFloat64VisualizerTangentStyle`
- `CGaFloat64VisualizerUtils`

#### Bugs zu fixen: Keine für CGA Visualizers Generic (neue Implementierung)

#### Geschätzter Aufwand: 2-3 Wochen (7 Dateien + Integration in 2 Klassen)

---

### Module 5: LinearAlgebra Details ⏸️ NOT STARTED
**Priorität:** P2 (Polishing - Convenience-Features)
**Status:** ~85% complete, diverse kleine Lücken
**Aufwand:** 2-3 Tage

#### Was fehlt in Generic:

**LinVector2D<T> (Zeile 32):**
- ⚠️ `Rcp()` method - Right Contraction Product

**LinVector3D<T> (Zeile 37):**
- ⚠️ `ToVector3D()` conversion method
- ⚠️ `BasisVectors` als property (Generic hat aktuell method)

**LinQuaternion<T> (Zeile 46, 48):**
- ⚠️ `CreateFromRotationMatrix()` factory method
- ⚠️ `ToSquareMatrix4()` conversion
- ⚠️ `ToSystemNumericsQuaternion()` interop
- ⚠️ 6 static properties:
  - `XyToXz`, `XyToYx`, `XyToYz`
  - `XyToZx`, `XyToZy`, `ZxToXy`

**LinBivector2D<T> (Zeile 50):**
- ⚠️ `ToXGaBivector()` second overload variant
- ⚠️ `ToXyBivector3D()` conversion

**LinBivector3D<T> (Zeile 53):**
- ⚠️ `ToXyBivector3D()` method

**LinAngle<T> (Zeile 56-61):**
- ⚠️ 14 static constants:
  - `Angle0Radians`, `Angle30Radians`, `Angle45Radians`
  - `Angle60Radians`, `Angle90Radians`, `Angle120Radians`
  - `Angle135Radians`, `Angle150Radians`, `Angle180Radians`
  - `Angle210Radians`, `Angle225Radians`, `Angle270Radians`
  - `Angle315Radians`, `Angle360Radians`
  - `Pi`, `PiOver2`, `PiTimes2`, `PiTimes4`
  - `DegreeToRadianFactor`, `RadianToDegreeFactor`
- ⚠️ `ToPolarAngleInPeriodicRange()` method
- ⚠️ `ToSquareMatrix2()` method

#### Bugs zu fixen: Keine P0-Bugs für LinearAlgebra Details

#### Geschätzter Aufwand: 2-3 Tage (viele kleine Features, meist Konversionen und Constants)

---

## 📅 Zeitplan

### Optimistisch (6 Wochen)

| Phase | Modul | Dauer | Abschluss |
|-------|-------|-------|-----------|
| Phase 1.1 | XGa Core | 3 Tage | Woche 1 |
| Phase 1.2 | ComplexAlgebra | 1 Woche | Woche 2 |
| Phase 1.3 | VGA | 1 Woche | Woche 3 |
| Phase 1.4 | CGA Visualizers | 2 Wochen | Woche 5 |
| Phase 1.5 | LinearAlgebra Details | 2 Tage | Woche 5 |
| Phase 2 | Alle 5 Module: Thin Wrapper | 1 Woche | Woche 6 |

### Realistisch (8-9 Wochen)

| Phase | Modul | Dauer | Abschluss |
|-------|-------|-------|-----------|
| Phase 1.1 | XGa Core | 5 Tage | Woche 1 |
| Phase 1.2 | ComplexAlgebra | 2 Wochen | Woche 3 |
| Phase 1.3 | VGA | 1 Woche | Woche 4 |
| Phase 1.4 | CGA Visualizers | 3 Wochen | Woche 7 |
| Phase 1.5 | LinearAlgebra Details | 3 Tage | Woche 8 |
| Phase 2 | Alle 5 Module: Thin Wrapper | 1-2 Wochen | Woche 9 |

### Konservativ (11 Wochen)

Inkludiert Buffer, Testing, Code Review, und unerwartete Probleme.

---

## 🗺️ PHASE 1: Generic auf 100% Float64-Kompatibilität bringen

**Für jedes Modul:**

### Schritt 1.1: Analyse
1. Float64-Implementierung lesen und verstehen
2. API-Unterschiede aus `API_COMPARISON` extrahieren
3. Test-Strategie definieren

### Schritt 1.2: Implementierung
1. Fehlende Klassen in Generic implementieren
2. IScalarProcessor<T> statt hardcoded double verwenden
3. Generic-Patterns befolgen (Composer, Factory-Methods, etc.)

### Schritt 1.3: Testing
1. Unit-Tests für neue Generic-Features schreiben
2. Equivalence-Tests: Generic<double> ≡ Float64 (optional, aber empfohlen)
3. Alle Tests müssen passieren

### Schritt 1.4: Bugs fixen (wenn vorhanden)
1. **NUR** Bugs für aktuelles Modul fixen
2. Bugs in anderen Modulen werden später gefixt

### Schritt 1.5: Dokumentation
1. Alle 3 Roadmap-Dokumente aktualisieren
2. Code-Kommentare hinzufügen
3. Modul als "Phase 1 Complete ✅" markieren

---

## 🗺️ PHASE 2: Thin Wrapper Migration

**Für jedes Modul (nach Phase 1 Complete):**

### Schritt 2.1: Thin Wrapper erstellen
1. Float64-Klasse als static class mit Properties neu schreiben
2. Alle Properties/Methods forwarden zu Generic<double>
3. Pattern folgen: `XGaFloat32Processor` als Referenz

### Schritt 2.2: Testing
1. Bestehende Float64-Tests müssen weiterhin passieren
2. Performance-Benchmarks (sollte ≥95% sein, aber Generic ist oft schneller!)

### Schritt 2.3: Code-Deletion
1. Alte Float64-Implementierung löschen (nach Test-Success)
2. Nur Thin Wrapper behalten (~100-200 LOC statt ~20,000 LOC)

### Schritt 2.4: Dokumentation
1. Migration dokumentieren
2. LOC-Reduktion tracken
3. Modul als "Phase 2 Complete ✅" markieren

---

## 📊 Erfolgsmetriken

### Quantitativ

| Metrik | Vorher | Nachher | Verbesserung |
|--------|--------|---------|--------------|
| **Total Files** | 390 Float64 + 231 Generic | ~246 Generic + 5 Wrappers | **-370 files (-64%)** |
| **Total LOC** | ~150,000 | ~92,500 | **-57,500 LOC (-38%)** |
| **XGa Files** | 129 Float64 | 1 Wrapper | **-128 files** |
| **CGA Files** | 83 Float64 | 1 Wrapper | **-82 files** |
| **Maintenance** | 100% (baseline) | ~55% | **-45% effort** |
| **Performance** | 100% (Float64) | **127%** | **+27% schneller!** ✅ |

### Qualitativ

- ✅ **Single Source of Truth:** Generic ist die einzige Implementierung
- ✅ **Type Safety:** Compile-time checks für alle Scalar-Typen
- ✅ **Extensibility:** Neue Scalar-Typen (Complex, Quaternion, etc.) einfach hinzufügbar
- ✅ **Maintainability:** Bug-Fixes gelten automatisch für alle Scalar-Typen
- ✅ **Performance:** Generic ist nachweislich schneller als Specialized (1.27x)

---

## 🎯 Nächste Schritte

### ✅ Phase 0 COMPLETE
- [x] 102/102 Equivalence-Tests passing
- [x] Performance validiert (Generic 1.27x schneller)
- [x] API-Analyse komplett (700+ Dateien analysiert)
- [x] Roadmap erstellt

### 🔄 Phase 1.1: Module 1 - XGa Core (STARTING)

**Start:** Nächster Arbeitstag
**Siehe:** `NEXT_STEPS_ROADMAP.md` für konkrete erste Tasks
**Siehe:** `DEDUPLICATION_TASKS.md` für detaillierte Task-Liste

**Erste Aufgabe:** XGaComputedOutermorphism<T> implementieren

---

## 🚨 Risikomanagement

### ✅ Mitigierte Risiken

- ✅ **Performance:** Generic ist 1.27x schneller (empirisch validiert)
- ✅ **Korrektheit:** 102 Equivalence-Tests beweisen mathematische Äquivalenz
- ✅ **Breaking Changes:** Thin Wrapper ist 100% kompatibel zu Float64
- ✅ **Bewährtes Pattern:** Float32 zeigt: Thin Wrapper funktioniert perfekt

### ⚠️ Verbleibende Risiken

- ⚠️ **Zeitschätzung:** 6-11 Wochen (könnte länger dauern)
- ⚠️ **Komplexität:** Neue Generic-Module (ComplexAlgebra, VGA, Visualizers) sind nicht-trivial
- ⚠️ **Testing:** Neue Features brauchen umfassende Tests

### ✅ Mitigation-Strategie

- **Modulweise Vorgehen:** Ein Modul nach dem anderen (kein Parallelismus)
- **Test-First:** Tests schreiben bevor/während Implementierung
- **Incremental Commits:** Kleine, testbare Commits
- **Code Reviews:** Peer-Review für neue Generic-Module
- **Dokumentation:** Nach jedem Modul alle Docs aktualisieren

---

## 📚 Referenzen

**Erfolgreiche Thin Wrapper Beispiele:**
- `XGaFloat32Processor.cs` - ~100 LOC statt ~20,000 LOC
- `CGaFloat32GeometricSpace.cs` - ~50 LOC statt ~10,000 LOC
- `PGaFloat32GeometricSpace.cs` - ~50 LOC statt ~8,000 LOC

**Performance-Analyse:**
- `GENERIC_VS_SPECIALIZED_PERFORMANCE.md` - Generic 1.27x schneller (bewiesen!)
- `FLOAT32_PERFORMANCE_ANALYSIS.md` - Float32 Thin Wrapper 97.5% Performance

**API-Analyse:**
- `API_COMPARISON_FLOAT64_VS_GENERIC_WITH_NAMESPACES.md` - Vollständiger API-Vergleich
- `API_COMPARISON_REPORT.md` - Executive Summary

**Architektur:**
- `CLAUDE.md` - Section on Processor Pattern & Generic Scalar Abstraction

---

## 📝 Hinweise

**Warum Generic-First funktioniert:**

1. **JIT Optimization:** .NET JIT spezialisiert Generic<double> zu nativem Code (zero overhead)
2. **Empirisch bewiesen:** Generic ist 1.27x SCHNELLER (nicht langsamer!)
3. **Type Safety:** Compile-time checks verhindern Type-Fehler
4. **Single Source of Truth:** Ein Bug-Fix gilt für alle Scalar-Typen
5. **Extensibility:** Neue Scalar-Typen ohne Code-Duplication

**Warum Float64 deprecated ist:**

1. **Massive Duplication:** ~80,000 LOC dupliziert zwischen Float64 und Generic
2. **Maintenance-Albtraum:** Bug-Fixes müssen in beiden Versionen gemacht werden
3. **Performance:** Generic ist schneller (nicht langsamer!)
4. **Zukunft:** Neue Features nur in Generic (Float64 ist stagnant)

**Kritische Erfolgsfaktoren:**

1. ✅ **Modulweise vorgehen** - Ein Modul komplett fertig, dann nächstes
2. ✅ **Tests schreiben** - Neue Features MÜSSEN getestet sein
3. ✅ **Dokumentation pflegen** - Nach jedem Modul alle Docs aktualisieren
4. ✅ **Bugs pro Modul fixen** - Nicht alle Bugs auf einmal (Fokus!)
5. ✅ **Performance messen** - Benchmarks nach Phase 2 pro Modul

---

**Dokument Version:** 3.2 (Task 1.3 Complete)
**Letzte Aktualisierung:** 2025-10-25
**Status:** Phase 1.1 In Progress (Module 1: XGa Core - 3/5 tasks complete)
**Nächste Review:** Nach Completion von Module 1
