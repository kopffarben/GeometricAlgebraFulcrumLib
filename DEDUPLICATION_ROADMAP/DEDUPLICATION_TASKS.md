# Deduplication Tasks - Detaillierte Checkliste

**Erstellt:** 2025-10-23
**Letzte Aktualisierung:** 2025-10-25
**Status:** Phase 1.1 In Progress (Module 1: XGa Core - Tasks 1.1, 1.2 & 1.3 Complete)
**Prinzip:** Generic-First - NUR Generic wird erweitert

---

## ⚠️ Task Execution Rules

1. **Ein Modul nach dem anderen** - Phase 1 für Modul komplett, dann nächstes Modul
2. **Generic-Only** - Float64 wird NICHT erweitert (deprecated)
3. **Bugs pro Modul** - Nur Bugs für aktuelles Modul fixen
4. **Tests schreiben** - Jede neue Implementierung braucht Tests
5. **Dokumentation** - Nach jedem Modul alle 3 Roadmap-Docs aktualisieren

---

## MODULE 1: XGa Core 🔄 STARTING

**Priorität:** P0 (Fundament)
**Geschätzter Aufwand:** 3-5 Tage (18-24 Stunden)
**Datenquelle:** API_COMPARISON Zeilen 12, 24, 25, 27, 30

---

### Task 1.1: XGaComputedOutermorphism<T> ✅ COMPLETE

- [x] **Implementierung: XGaComputedOutermorphism<T>**
  - **Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.LinearMaps.Outermorphisms`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Outermorphisms.XGaFloat64ComputedOutermorphism`
  - **Datei:** `GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/Generic/LinearMaps/Outermorphisms/XGaComputedOutermorphism.cs`
  - **Estimated:** 4-6 Stunden
  - **Actual:** ~3 Stunden

- [x] Float64-Datei lesen und analysieren
- [x] Generic-Datei erstellen mit Basis-Struktur
- [x] Constructor implementieren (akzeptiert `XGaProcessor<T>`)
- [x] Properties implementieren (Processor, Metric, ScalarProcessor)
- [x] OmMap-Methods implementieren (mit `IScalarProcessor<T>`)
- [x] Weitere Methods portieren (alle public methods)
- [x] Unit-Tests schreiben (Debug-Tests in `XGaComputedOutermorphismDebugTests.cs`)
- [x] Equivalence-Tests schreiben (`XGaComputedOutermorphismEquivalenceTests.cs` - 6 Tests)
- [x] Alle Tests laufen lassen → 6/6 passing ✅
- [x] **BONUS:** IndexSet.GetSubsets() Bug gefunden und gefixt (EmptySet singleton issue)
- [x] Git commit mit Message: "feat(Generic): Add XGaComputedOutermorphism<T> + fix IndexSet bug"

---

### Task 1.2: XGaStoredOutermorphism<T> ✅ COMPLETE

- [x] **Implementierung: XGaStoredOutermorphism<T>**
  - **Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.LinearMaps.Outermorphisms`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Outermorphisms.XGaFloat64StoredOutermorphism`
  - **Datei:** `GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/Generic/LinearMaps/Outermorphisms/XGaStoredOutermorphism.cs`
  - **Estimated:** 4-6 Stunden
  - **Actual:** ~4 Stunden (inkl. Bug-Fixes)

- [x] Float64-Datei lesen und analysieren
- [x] Generic-Datei erstellen
- [x] Constructor implementieren
- [x] Storage-Logic implementieren (multivector storage mit `Dictionary<IndexSet, XGaKVector<T>>`)
- [x] OmMap-Methods implementieren
- [x] Factory-Methoden hinzugefügt (`CreateStoredOutermorphism()` für Generic & Float64)
- [x] Unit-Tests + Equivalence-Tests schreiben (`XGaStoredOutermorphismEquivalenceTests.cs` - 9 Tests)
- [x] Tests laufen lassen → 9/9 passing ✅
- [x] **BONUS:** 2 CRITICAL Float64 bugs gefunden und gefixt:
  - OmMapBasisBlade() returned kVector.GetVectorPart() instead of kVector
  - OmMapBasisBivector() had inverted index-order logic (index1 > index2 → index1 < index2)
- [x] Dokumentation aktualisiert (alle 3 Roadmap-Docs)
- [x] Git commit mit Message: "feat(Generic): Add XGaStoredOutermorphism<T> + fix critical Float64 bugs"

---

### Task 1.3: XGaOutermorphismComposerUtils<T> ✅ COMPLETE

- [x] **Implementierung: XGaOutermorphismComposerUtils<T>**
  - **Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.LinearMaps.Outermorphisms`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.LinearMaps.Outermorphisms.XGaFloat64OutermorphismComposerUtils`
  - **Datei:** `GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/Generic/LinearMaps/Outermorphisms/XGaOutermorphismComposerUtils.cs`
  - **Estimated:** 3-4 Stunden
  - **Actual:** ~2 Stunden

- [x] Float64-Datei analysieren (static utility class)
- [x] Generic static class erweitern (existierte bereits mit 1 Methode)
- [x] Extension method ColumnsToOutermorphism<T> portieren
- [x] Factory methods waren bereits vorhanden (CreateComputedOutermorphism, CreateStoredOutermorphism)
- [x] Tests schreiben (`XGaOutermorphismComposerUtilsEquivalenceTests.cs` - 6 Tests)
- [x] Tests laufen lassen → 6/6 passing ✅
- [x] Dokumentation aktualisiert (alle 3 Roadmap-Docs)
- [x] Git commit mit Message: "feat(Generic): Complete XGaOutermorphismComposerUtils<T> with ColumnsToOutermorphism"

---

### Task 1.4: XGaGramSchmidtFrame<T>

- [ ] **Implementierung: XGaGramSchmidtFrame<T>**
  - **Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Frames`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Frames.XGaFloat64GramSchmidtFrame`
  - **Datei:** `GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/Generic/Frames/XGaGramSchmidtFrame.cs`
  - **Estimated:** 4-5 Stunden

- [ ] Float64-Datei analysieren
- [ ] Generic-Datei erstellen
- [ ] Frame storage implementieren (List oder Array von Vektoren)
- [ ] Gram-Schmidt orthogonalization implementieren
  - ⚠️ **WICHTIG:** Numerisch sensibel! Korrekte `ScalarProcessor` Usage
  - Verwende `processor.ScalarProcessor.Sqrt()`, `Divide()`, etc.
- [ ] Frame-Methods implementieren (GetVector, GetBasisVector, etc.)
- [ ] Tests schreiben (inkl. Orthonormalität prüfen: `v[i].Sp(v[j]) == 0` für i≠j)
- [ ] Tests laufen lassen → passing
- [ ] Git commit mit Message: "feat(Generic): Add XGaGramSchmidtFrame<T>"

---

### Task 1.5: XGaConformalComposerUtils<T>

- [ ] **Implementierung: XGaConformalComposerUtils<T>**
  - **Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Spaces.Conformal`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Spaces.Conformal.XGaFloat64ConformalComposerUtils`
  - **Datei:** `GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/Generic/Spaces/Conformal/XGaConformalComposerUtils.cs`
  - **Estimated:** 2-3 Stunden

- [ ] Float64-Datei analysieren
- [ ] Generic static class erstellen
- [ ] Conformal utility methods portieren (Composers für CGA-Objekte)
- [ ] Tests schreiben
- [ ] Tests laufen lassen → passing
- [ ] Git commit mit Message: "feat(Generic): Add XGaConformalComposerUtils<T>"

---

### Task 1.6: ToTuple() Extensions (Optional - P2)

- [ ] **Implementierung: ToTuple() extension methods** (Optional - kann übersprungen werden)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Multivectors`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors` (diverse ToTuple methods)
  - **Dateien:** Diverse Generic Multivector-Dateien
  - **Estimated:** 1-2 Stunden

- [ ] ToTuple methods in Float64 identifizieren (Scalar, Vector, Bivector, etc.)
- [ ] Generic-Äquivalente implementieren
- [ ] Tests schreiben
- [ ] Tests laufen lassen → passing
- [ ] Git commit mit Message: "feat(Generic): Add ToTuple() extensions"

**Entscheidung:** Kann übersprungen werden wenn Zeit knapp (P2 - niedrige Priorität)

---

### Task 1.7: Dokumentation & Abschluss Module 1

- [ ] **Dokumentation aktualisieren**
- [ ] `DEDUPLICATION_ROADMAP.md` aktualisieren → Module 1 Status: "Phase 1 Complete ✅"
- [ ] `NEXT_STEPS_ROADMAP.md` aktualisieren → Nächstes Modul: Module 2 (ComplexAlgebra)
- [ ] `DEDUPLICATION_TASKS.md` aktualisieren → Alle Checkboxen für Module 1 als checked markieren
- [ ] `_Status.md` aktualisieren
- [ ] Alle Tests für Module 1 nochmal laufen lassen
- [ ] Git push all commits

---

### ✅ MODULE 1 COMPLETE WHEN:

- [ ] Tasks 1.1-1.5 alle complete (✅✅✅✅✅)
- [ ] Task 1.6 complete ODER bewusst übersprungen
- [ ] Alle neuen Tests passing
- [ ] Generic hat nun ALLE Features die Float64 hat (für XGa Core)
- [ ] Dokumentation aktualisiert
- [ ] Git commits gepusht

**TOTAL Module 1:** 18-24 Stunden (3-5 Arbeitstage)

---

## MODULE 2: ComplexAlgebra ⏸️ NOT STARTED

**Priorität:** P0 (Wichtig für Algebra)
**Status:** 🚨 GESAMTES Modul fehlt in Generic
**Geschätzter Aufwand:** 1-2 Wochen (40-80 Stunden)
**Datenquelle:** API_COMPARISON Zeile 63

---

### Task 2.1: ComplexScalar<T>

- [ ] **Implementierung: ComplexScalar<T>** (Basis-Klasse für komplexe Skalare)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Algebra.ComplexAlgebra`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Algebra.ComplexAlgebra.Float64ComplexScalar`
  - **Datei:** `GeometricAlgebraFulcrumLib.Algebra/ComplexAlgebra/ComplexScalar.cs` (NEUE DATEI)
  - **Estimated:** 2-3 Tage

- [ ] Float64-Implementierung analysieren (komplexe Logik!)
- [ ] Generic-Klasse erstellen mit `IScalarProcessor<T>`
- [ ] Properties implementieren:
  - [ ] `Real` (T)
  - [ ] `Imaginary` (T)
  - [ ] `Magnitude` (T)
  - [ ] `Phase` (T)
  - [ ] `ScalarProcessor` (IScalarProcessor<T>)
- [ ] Constructor implementieren:
  - [ ] `ComplexScalar(T real, T imaginary)`
  - [ ] `ComplexScalar(IScalarProcessor<T>, T real, T imaginary)`
  - [ ] `FromPolar(T magnitude, T phase)` factory
- [ ] Arithmetic operations implementieren:
  - [ ] `Add(ComplexScalar<T>)`
  - [ ] `Subtract(ComplexScalar<T>)`
  - [ ] `Multiply(ComplexScalar<T>)`
  - [ ] `Divide(ComplexScalar<T>)`
  - [ ] `Negate()`
- [ ] Spezielle Operationen:
  - [ ] `Conjugate()` - Konjugation (real, -imaginary)
  - [ ] `Reciprocal()` - 1/z
  - [ ] `Sqrt()` - Quadratwurzel
  - [ ] `Exp()` - e^z
  - [ ] `Log()` - ln(z)
- [ ] Conversions implementieren:
  - [ ] `ToPolar()` - Kartesisch → Polar
  - [ ] `ToCartesian()` - Polar → Kartesisch
  - [ ] `ToString()` override
- [ ] Operator overloads:
  - [ ] `operator +`, `-`, `*`, `/`
  - [ ] `operator ==`, `!=`
- [ ] Tests schreiben (UMFANGREICH - komplexe Algebra!):
  - [ ] Unit-Tests für alle Operationen
  - [ ] Edge-Cases (0, infinity, NaN)
  - [ ] Equivalence-Tests (Generic<double> vs Float64)
- [ ] Tests laufen lassen → passing
- [ ] Git commit mit Message: "feat(ComplexAlgebra): Add ComplexScalar<T>"

---

### Task 2.2: ComplexUtils<T>

- [ ] **Implementierung: ComplexUtils<T>** (Utility-Methods)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Algebra.ComplexAlgebra`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Algebra.ComplexAlgebra.Float64ComplexUtils`
  - **Datei:** `GeometricAlgebraFulcrumLib.Algebra/ComplexAlgebra/ComplexUtils.cs` (NEUE DATEI)
  - **Estimated:** 1 Tag

- [ ] Float64-Utils analysieren
- [ ] Generic static class erstellen
- [ ] Utility methods portieren:
  - [ ] `Create(T real, T imaginary)`
  - [ ] `CreatePolar(T magnitude, T phase)`
  - [ ] `Zero(IScalarProcessor<T>)`
  - [ ] `One(IScalarProcessor<T>)`
  - [ ] `I(IScalarProcessor<T>)` - imaginäre Einheit
  - [ ] Trigonometrische Functions (Sin, Cos, Tan mit komplexen Argumenten)
  - [ ] Hyperbolische Functions (Sinh, Cosh, Tanh)
- [ ] Tests schreiben
- [ ] Tests laufen lassen → passing
- [ ] Git commit mit Message: "feat(ComplexAlgebra): Add ComplexUtils<T>"

---

### Task 2.3: ComplexAlgebraUtils<T>

- [ ] **Implementierung: ComplexAlgebraUtils<T>** (Algebra-Operations)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Algebra.ComplexAlgebra`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Algebra.ComplexAlgebra.ComplexAlgebraUtils`
  - **Datei:** `GeometricAlgebraFulcrumLib.Algebra/ComplexAlgebra/ComplexAlgebraUtils.cs` (erweitern für Generic)
  - **Estimated:** 1-2 Tage

- [ ] Float64-Implementierung analysieren
- [ ] Generic static class erstellen (oder bestehende erweitern)
- [ ] Algebra-Methods portieren:
  - [ ] Matrix operations mit komplexen Einträgen
  - [ ] Determinante (komplex)
  - [ ] Eigenwerte/Eigenvektoren (falls vorhanden)
  - [ ] Weitere algebraische Operationen
- [ ] Tests schreiben
- [ ] Tests laufen lassen → passing
- [ ] Git commit mit Message: "feat(ComplexAlgebra): Add ComplexAlgebraUtils<T>"

---

### Task 2.4: Integration & Testing

- [ ] **Integration-Tests für ComplexAlgebra**
  - **Estimated:** 1 Tag

- [ ] Alle ComplexAlgebra-Tests zusammen laufen lassen
- [ ] Integration-Tests schreiben:
  - [ ] ComplexScalar<T> mit XGa (komplexe Multivektoren)
  - [ ] ComplexScalar<T> mit LinAlgebra (komplexe Vektoren/Matrizen)
- [ ] Equivalence-Tests (Generic<double> vs Float64ComplexScalar)
- [ ] Performance-Tests (optional - benchmarks)
- [ ] Alle Tests passing
- [ ] Git commit mit Message: "test(ComplexAlgebra): Add integration tests"

---

### Task 2.5: Dokumentation & Abschluss Module 2

- [ ] **Dokumentation aktualisieren**
- [ ] Alle 3 Roadmap-Dokumente aktualisieren
- [ ] Module 2 als "Phase 1 Complete ✅" markieren
- [ ] `_Status.md` aktualisieren
- [ ] Git push all commits

---

### ✅ MODULE 2 COMPLETE WHEN:

- [ ] ComplexScalar<T> implementiert und getestet (✅)
- [ ] ComplexUtils<T> implementiert und getestet (✅)
- [ ] ComplexAlgebraUtils<T> implementiert und getestet (✅)
- [ ] Integration-Tests passing (✅)
- [ ] Dokumentation aktualisiert (✅)

**TOTAL Module 2:** 40-80 Stunden (1-2 Wochen)

---

## MODULE 3: VGA (Vector GA) ⏸️ NOT STARTED

**Priorität:** P0 (Wichtig für Modeling)
**Status:** 🚨 GESAMTES Modul fehlt in Generic
**Geschätzter Aufwand:** 1 Woche (35-45 Stunden)
**Datenquelle:** API_COMPARISON Zeile 103

---

### Task 3.1: RGaEuclideanGeometrySpace<T>

- [ ] **Implementierung: RGaEuclideanGeometrySpace<T>** (Basis-Klasse)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.VGa.Generic`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.VGa.Float64.RGaEuclideanGeometrySpace`
  - **Datei:** `GeometricAlgebraFulcrumLib.Modeling/Geometry/VGa/Generic/RGaEuclideanGeometrySpace.cs` (NEUE DATEI)
  - **Estimated:** 2 Tage

- [ ] Float64-Implementierung analysieren
- [ ] Generic base class erstellen
- [ ] Constructor, Properties implementieren (Processor, VSpaceDimensions, etc.)
- [ ] Methods implementieren (geometry operations)
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(VGA): Add RGaEuclideanGeometrySpace<T>"

---

### Task 3.2: RGaEuclideanGeometrySpace2D<T>

- [ ] **Implementierung: RGaEuclideanGeometrySpace2D<T>** (2D-Spezialisierung)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.VGa.Generic`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.VGa.Float64.RGaEuclideanGeometrySpace2D`
  - **Datei:** `GeometricAlgebraFulcrumLib.Modeling/Geometry/VGa/Generic/RGaEuclideanGeometrySpace2D.cs` (NEUE DATEI)
  - **Estimated:** 1 Tag

- [ ] Float64-Implementierung analysieren
- [ ] Generic 2D-Spezialisierung erstellen (erbt von RGaEuclideanGeometrySpace<T>)
- [ ] 2D-spezifische Methods implementieren (Rotation2D, etc.)
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(VGA): Add RGaEuclideanGeometrySpace2D<T>"

---

### Task 3.3: RGaEuclideanGeometrySpace3D<T>

- [ ] **Implementierung: RGaEuclideanGeometrySpace3D<T>** (3D-Spezialisierung)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.VGa.Generic`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.VGa.Float64.RGaEuclideanGeometrySpace3D`
  - **Datei:** `GeometricAlgebraFulcrumLib.Modeling/Geometry/VGa/Generic/RGaEuclideanGeometrySpace3D.cs` (NEUE DATEI)
  - **Estimated:** 1 Tag

- [ ] Float64-Implementierung analysieren
- [ ] Generic 3D-Spezialisierung erstellen
- [ ] 3D-spezifische Methods implementieren (Cross product, Rotation3D, etc.)
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(VGA): Add RGaEuclideanGeometrySpace3D<T>"

---

### Task 3.4: EuclideanGeometryUtils<T>

- [ ] **Implementierung: EuclideanGeometryUtils<T>** (Utility-Methods)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.VGa.Generic`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.VGa.Float64.EuclideanGeometryUtils`
  - **Datei:** `GeometricAlgebraFulcrumLib.Modeling/Geometry/VGa/Generic/EuclideanGeometryUtils.cs` (NEUE DATEI)
  - **Estimated:** 1 Tag

- [ ] Float64-Utils analysieren
- [ ] Generic static class erstellen
- [ ] Utility methods implementieren (Distance, Angle, Projection, etc.)
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(VGA): Add EuclideanGeometryUtils<T>"

---

### Task 3.5: Integration, Testing & Dokumentation

- [ ] **Integration & Abschluss Module 3**
  - **Estimated:** 1 Tag

- [ ] Alle VGA-Tests zusammen laufen lassen
- [ ] Integration-Tests (VGA mit XGa)
- [ ] Alle Tests passing
- [ ] Dokumentation aktualisieren (3 Roadmap-Docs)
- [ ] Module 3 als "Phase 1 Complete ✅" markieren
- [ ] Git push all commits

---

### ✅ MODULE 3 COMPLETE WHEN:

- [ ] Alle 4 VGA-Klassen implementiert (✅✅✅✅)
- [ ] Alle Tests passing (✅)
- [ ] Dokumentation aktualisiert (✅)

**TOTAL Module 3:** 35-45 Stunden (5-7 Tage, ~1 Woche)

---

## MODULE 4: CGA Visualizers ⏸️ NOT STARTED

**Priorität:** P1 (Visualization)
**Status:** 🚨 GESAMTES Modul fehlt in Generic
**Geschätzter Aufwand:** 2-3 Wochen (80-120 Stunden)
**Datenquelle:** API_COMPARISON Zeilen 67, 78, 83

---

### Task 4.1: CGaVisualizer<T>

- [ ] **Implementierung: CGaVisualizer<T>** (Haupt-Visualizer-Klasse)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Visualizers`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Visualizers.CGaFloat64Visualizer`
  - **Datei:** `GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Generic/Visualizers/CGaVisualizer.cs` (NEUE DATEI)
  - **Estimated:** 3-4 Tage

- [ ] Float64-Implementierung analysieren (KOMPLEX - große Klasse!)
- [ ] Generic-Hauptklasse erstellen
- [ ] Visualization-Pipeline implementieren
- [ ] Rendering-Methods implementieren
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(CGA): Add CGaVisualizer<T>"

---

### Task 4.2: CGaVisualizerDirectionStyle<T>

- [ ] **Implementierung: CGaVisualizerDirectionStyle<T>**
  - **Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Visualizers`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Visualizers.CGaFloat64VisualizerDirectionStyle`
  - **Datei:** `GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Generic/Visualizers/CGaVisualizerDirectionStyle.cs` (NEUE DATEI)
  - **Estimated:** 1 Tag

- [ ] Float64-Implementierung analysieren
- [ ] Generic-Klasse erstellen
- [ ] Style-Properties implementieren
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(CGA): Add CGaVisualizerDirectionStyle<T>"

---

### Task 4.3: CGaVisualizerElementStyle<T>

- [ ] **Implementierung: CGaVisualizerElementStyle<T>**
  - **Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Visualizers`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Visualizers.CGaFloat64VisualizerElementStyle`
  - **Datei:** `GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Generic/Visualizers/CGaVisualizerElementStyle.cs` (NEUE DATEI)
  - **Estimated:** 1 Tag

- [ ] Float64-Implementierung analysieren
- [ ] Generic-Klasse erstellen
- [ ] Style-Properties implementieren
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(CGA): Add CGaVisualizerElementStyle<T>"

---

### Task 4.4: CGaVisualizerFlatStyle<T>

- [ ] **Implementierung: CGaVisualizerFlatStyle<T>**
  - **Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Visualizers`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Visualizers.CGaFloat64VisualizerFlatStyle`
  - **Datei:** `GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Generic/Visualizers/CGaVisualizerFlatStyle.cs` (NEUE DATEI)
  - **Estimated:** 1 Tag

- [ ] Float64-Implementierung analysieren
- [ ] Generic-Klasse erstellen (Styling für Punkte, Linien, Ebenen)
- [ ] Style-Properties implementieren
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(CGA): Add CGaVisualizerFlatStyle<T>"

---

### Task 4.5: CGaVisualizerRoundStyle<T>

- [ ] **Implementierung: CGaVisualizerRoundStyle<T>**
  - **Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Visualizers`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Visualizers.CGaFloat64VisualizerRoundStyle`
  - **Datei:** `GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Generic/Visualizers/CGaVisualizerRoundStyle.cs` (NEUE DATEI)
  - **Estimated:** 1 Tag

- [ ] Float64-Implementierung analysieren
- [ ] Generic-Klasse erstellen (Styling für Kreise, Sphären)
- [ ] Style-Properties implementieren
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(CGA): Add CGaVisualizerRoundStyle<T>"

---

### Task 4.6: CGaVisualizerTangentStyle<T>

- [ ] **Implementierung: CGaVisualizerTangentStyle<T>**
  - **Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Visualizers`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Visualizers.CGaFloat64VisualizerTangentStyle`
  - **Datei:** `GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Generic/Visualizers/CGaVisualizerTangentStyle.cs` (NEUE DATEI)
  - **Estimated:** 1 Tag

- [ ] Float64-Implementierung analysieren
- [ ] Generic-Klasse erstellen (Styling für tangentiale Objekte)
- [ ] Style-Properties implementieren
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(CGA): Add CGaVisualizerTangentStyle<T>"

---

### Task 4.7: CGaVisualizerUtils<T>

- [ ] **Implementierung: CGaVisualizerUtils<T>**
  - **Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Visualizers`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float64.Visualizers.CGaFloat64VisualizerUtils`
  - **Datei:** `GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Generic/Visualizers/CGaVisualizerUtils.cs` (NEUE DATEI)
  - **Estimated:** 1 Tag

- [ ] Float64-Utils analysieren
- [ ] Generic static class erstellen
- [ ] Utility methods implementieren
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(CGA): Add CGaVisualizerUtils<T>"

---

### Task 4.8: Integration in CGaGeometricSpace5D<T>

- [ ] **Erweitern: CGaGeometricSpace5D<T>** (Visualizer-Properties hinzufügen)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic`
  - **Datei:** `GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Generic/CGaGeometricSpace5D.cs` (ERWEITERN)
  - **Estimated:** 1 Tag

- [ ] Datei öffnen und analysieren
- [ ] Properties hinzufügen:
  - [ ] `Visualizer` property (returns `CGaVisualizer<T>`)
  - [ ] `VisualizerAnimationComposer` property
  - [ ] `VisualizerKaTeXComposer` property
  - [ ] `VisualizerSceneComposer` property
- [ ] Lazy initialization implementieren (erstelle Visualizer on demand)
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(CGA): Add Visualizer properties to CGaGeometricSpace5D<T>"

---

### Task 4.9: Integration in CGaBlade<T>

- [ ] **Erweitern: CGaBlade<T>** (Visualizer-Property hinzufügen)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Generic.Blades`
  - **Datei:** `GeometricAlgebraFulcrumLib.Modeling/Geometry/CGa/Generic/Blades/CGaBlade.cs` (ERWEITERN)
  - **Estimated:** 0.5 Tage

- [ ] Datei öffnen
- [ ] `Visualizer` property hinzufügen (returns `CGaVisualizer<T>`)
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(CGA): Add Visualizer property to CGaBlade<T>"

---

### Task 4.10: Testing & Dokumentation

- [ ] **Integration-Tests und Abschluss Module 4**
  - **Estimated:** 2 Tage

- [ ] Alle Visualizer-Tests zusammen laufen lassen
- [ ] Integration-Tests (Visualizers mit CGA)
- [ ] Visual-Tests (optional - renders prüfen wenn möglich)
- [ ] Alle Tests passing
- [ ] Dokumentation aktualisieren (3 Roadmap-Docs)
- [ ] Module 4 als "Phase 1 Complete ✅" markieren
- [ ] Git push all commits

---

### ✅ MODULE 4 COMPLETE WHEN:

- [ ] CGaVisualizer<T> implementiert (✅)
- [ ] 6 Style-Klassen implementiert (✅✅✅✅✅✅)
- [ ] Integration in CGaGeometricSpace5D<T> complete (✅)
- [ ] Integration in CGaBlade<T> complete (✅)
- [ ] Alle Tests passing (✅)
- [ ] Dokumentation aktualisiert (✅)

**TOTAL Module 4:** 80-120 Stunden (13-17 Tage, ~2-3 Wochen)

---

## MODULE 5: LinearAlgebra Details ⏸️ NOT STARTED

**Priorität:** P2 (Polishing - Convenience-Features)
**Status:** ~85% complete, diverse kleine Lücken
**Geschätzter Aufwand:** 2-3 Tage (13-18 Stunden)
**Datenquelle:** API_COMPARISON Zeilen 32, 37, 46, 48, 50, 53, 56-61

---

### Task 5.1: LinVector2D<T>.Rcp()

- [ ] **Erweitern: LinVector2D<T>** (Rcp method hinzufügen)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D.LinFloat64Vector2D.Rcp()`
  - **Datei:** `GeometricAlgebraFulcrumLib.Algebra/LinearAlgebra/Generic/Vectors/Space2D/LinVector2D.cs` (ERWEITERN)
  - **Estimated:** 1 Stunde

- [ ] Float64 Rcp() analysieren (Right Contraction Product)
- [ ] Generic Rcp() implementieren mit `ScalarProcessor`
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(LinearAlgebra): Add LinVector2D<T>.Rcp()"

---

### Task 5.2: LinVector3D<T> Conversions

- [ ] **Erweitern: LinVector3D<T>** (Conversions hinzufügen)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space3D`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D.LinFloat64Vector3D`
  - **Datei:** `GeometricAlgebraFulcrumLib.Algebra/LinearAlgebra/Generic/Vectors/Space3D/LinVector3D.cs` (ERWEITERN)
  - **Estimated:** 2 Stunden

- [ ] `ToVector3D()` conversion method implementieren
- [ ] `BasisVectors` als property implementieren (Generic hat aktuell method, Float64 hat property)
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(LinearAlgebra): Add LinVector3D<T> conversions"

---

### Task 5.3: LinQuaternion<T> Features

- [ ] **Erweitern: LinQuaternion<T>** (Diverse Features hinzufügen)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space4D`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space4D.LinFloat64Quaternion`
  - **Datei:** `GeometricAlgebraFulcrumLib.Algebra/LinearAlgebra/Generic/Vectors/Space4D/LinQuaternion.cs` (ERWEITERN)
  - **Estimated:** 4-5 Stunden

- [ ] `CreateFromRotationMatrix()` factory method implementieren
- [ ] `ToSquareMatrix4()` conversion implementieren
- [ ] `ToSystemNumericsQuaternion()` interop implementieren (System.Numerics.Quaternion)
- [ ] 6 static properties implementieren:
  - [ ] `XyToXz` static property
  - [ ] `XyToYx` static property
  - [ ] `XyToYz` static property
  - [ ] `XyToZx` static property
  - [ ] `XyToZy` static property
  - [ ] `ZxToXy` static property
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(LinearAlgebra): Add LinQuaternion<T> features"

---

### Task 5.4: LinBivector Conversions

- [ ] **Erweitern: LinBivector2D<T> & LinBivector3D<T>** (Conversions hinzufügen)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Vectors.Space2D` und `.Space3D`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space2D/3D`
  - **Dateien:**
    - `GeometricAlgebraFulcrumLib.Algebra/LinearAlgebra/Generic/Vectors/Space2D/LinBivector2D.cs` (ERWEITERN)
    - `GeometricAlgebraFulcrumLib.Algebra/LinearAlgebra/Generic/Vectors/Space3D/LinBivector3D.cs` (ERWEITERN)
  - **Estimated:** 2-3 Stunden

- [ ] LinBivector2D<T>.ToXGaBivector() second overload implementieren
- [ ] LinBivector2D<T>.ToXyBivector3D() conversion implementieren
- [ ] LinBivector3D<T>.ToXyBivector3D() method implementieren
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(LinearAlgebra): Add LinBivector conversions"

---

### Task 5.5: LinAngle<T> Constants & Methods

- [ ] **Erweitern: LinAngle<T>** (Static Constants & Methods hinzufügen)
  - **Namespace:** `GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Generic.Angles`
  - **Referenz:** `GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Angles.LinFloat64Angle`
  - **Datei:** `GeometricAlgebraFulcrumLib.Algebra/LinearAlgebra/Generic/Angles/LinAngle.cs` (ERWEITERN)
  - **Estimated:** 3-4 Stunden

- [ ] 14 static constants implementieren:
  - [ ] `Angle0Radians`, `Angle30Radians`, `Angle45Radians`, `Angle60Radians`
  - [ ] `Angle90Radians`, `Angle120Radians`, `Angle135Radians`, `Angle150Radians`
  - [ ] `Angle180Radians`, `Angle210Radians`, `Angle225Radians`, `Angle270Radians`
  - [ ] `Angle315Radians`, `Angle360Radians`
  - [ ] `Pi`, `PiOver2`, `PiTimes2`, `PiTimes4`
  - [ ] `DegreeToRadianFactor`, `RadianToDegreeFactor`
- [ ] `ToPolarAngleInPeriodicRange()` method implementieren
- [ ] `ToSquareMatrix2()` method implementieren
- [ ] Tests schreiben
- [ ] Tests passing
- [ ] Git commit mit Message: "feat(LinearAlgebra): Add LinAngle<T> constants and methods"

---

### Task 5.6: Testing & Dokumentation

- [ ] **Testing und Abschluss Module 5**
  - **Estimated:** 1 Stunde

- [ ] Alle LinearAlgebra-Tests zusammen laufen lassen
- [ ] Alle Tests passing
- [ ] Dokumentation aktualisieren (3 Roadmap-Docs)
- [ ] Module 5 als "Phase 1 Complete ✅" markieren
- [ ] Git push all commits

---

### ✅ MODULE 5 COMPLETE WHEN:

- [ ] Alle LinearAlgebra-Features implementiert (✅✅✅✅✅)
- [ ] Alle Tests passing (✅)
- [ ] Dokumentation aktualisiert (✅)

**TOTAL Module 5:** 13-18 Stunden (2-3 Tage)

---

## 🎯 PHASE 1 COMPLETE WHEN:

- [ ] Module 1: XGa Core ✅
- [ ] Module 2: ComplexAlgebra ✅
- [ ] Module 3: VGA ✅
- [ ] Module 4: CGA Visualizers ✅
- [ ] Module 5: LinearAlgebra Details ✅
- [ ] **Generic ⊇ Float64** für alle 5 Module ✅
- [ ] Alle Tests passing ✅
- [ ] Dokumentation komplett aktualisiert ✅

**TOTAL Phase 1:** 186-287 Stunden (6-8 Wochen bei 35-40 Stunden/Woche)

**DANN:** Phase 2 beginnen (Thin Wrapper Migration) - separate Task-Liste wird erstellt

---

## 📅 Täglicher Workflow

### Morgen:
- [ ] Dieses Dokument öffnen
- [ ] Nächstes unkomplettiertes Task identifizieren (erstes ohne ✓)
- [ ] Subtasks für heute planen

### Während Arbeit:
- [ ] Subtask implementieren
- [ ] Tests schreiben parallel zur Implementierung
- [ ] Tests laufen lassen nach jeder Änderung
- [ ] Iterieren bis alle Tests passing
- [ ] **Checkbox abhaken in diesem Dokument** ✓

### Abend:
- [ ] Git commit mit klarer Message
- [ ] Fortschritt dokumentieren (in diesem File Checkboxen setzen)
- [ ] Nächsten Tag planen (nächstes Task anschauen)

### Wöchentlich:
- [ ] Alle 3 Roadmap-Dokumente aktualisieren
- [ ] Fortschritt vs. Schätzung prüfen
- [ ] Zeitplan anpassen wenn nötig
- [ ] Module-Completion checken

---

## 🚨 Bug-Fix Strategie

**Regel:** Bugs NUR fixen wenn wir an dem Modul arbeiten

**Bekannte Bugs pro Modul:**
- Module 1 (XGa Core): Keine bekannten P0-Bugs ✅
- Module 2 (ComplexAlgebra): Keine (neue Implementierung) ✅
- Module 3 (VGA): Keine (neue Implementierung) ✅
- Module 4 (CGA Visualizers): Keine (neue Implementierung) ✅
- Module 5 (LinearAlgebra): Keine bekannten P0-Bugs ✅

**Bugs in ANDEREN Modulen (NICHT in Phase 1 fixen):**
- Statistics: 4 P0 bugs → wird später gefixt
- Calculus: 1 P0 bug → wird später gefixt
- Trajectories: 5 P1 bugs → wird später gefixt
- BasicShapes: 2 P1 bugs → wird später gefixt
- Signals: 1 P2 bug → wird später gefixt

→ Diese werden NICHT in Phase 1 gefixt! Fokus auf die 5 Module.

---

## 📊 Erfolgsmetriken (Nach jedem Modul tracken)

**Module 1:**
- [ ] Neue Generic-Klassen: 5 Klassen
- [ ] Neue Tests: ~X Tests
- [ ] Alle Tests passing: ✅
- [ ] LOC hinzugefügt: ~X LOC
- [ ] Zeitaufwand: X Stunden (vs. 18-24 Stunden geschätzt)

**Module 2:**
- [ ] Neue Generic-Klassen: 3-4 Klassen
- [ ] Neue Tests: ~X Tests
- [ ] Alle Tests passing: ✅
- [ ] LOC hinzugefügt: ~X LOC
- [ ] Zeitaufwand: X Stunden (vs. 40-80 Stunden geschätzt)

**Module 3:**
- [ ] Neue Generic-Klassen: 4 Klassen
- [ ] Neue Tests: ~X Tests
- [ ] Alle Tests passing: ✅
- [ ] LOC hinzugefügt: ~X LOC
- [ ] Zeitaufwand: X Stunden (vs. 35-45 Stunden geschätzt)

**Module 4:**
- [ ] Neue Generic-Klassen: 7+ Klassen
- [ ] Neue Tests: ~X Tests
- [ ] Alle Tests passing: ✅
- [ ] LOC hinzugefügt: ~X LOC
- [ ] Zeitaufwand: X Stunden (vs. 80-120 Stunden geschätzt)

**Module 5:**
- [ ] Neue Features: ~10 Features
- [ ] Neue Tests: ~X Tests
- [ ] Alle Tests passing: ✅
- [ ] LOC hinzugefügt: ~X LOC
- [ ] Zeitaufwand: X Stunden (vs. 13-18 Stunden geschätzt)

---

**Nach Phase 1 complete:**
- [ ] Generic ⊇ Float64 für alle 5 Module: ✅
- [ ] Gesamt neue Klassen: ~25+ Klassen
- [ ] Gesamt neue Tests: ~X Tests
- [ ] Gesamt LOC hinzugefügt: ~X LOC
- [ ] Gesamt Zeitaufwand: X Wochen (vs. 6-8 Wochen geschätzt)

---

**Nächste Aktion:** Module 1, Task 1.1 starten (XGaComputedOutermorphism<T>)

**Erstes Kommando:**
```bash
code GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/Float64/LinearMaps/Outermorphisms/XGaFloat64ComputedOutermorphism.cs
```

---

*Dokument maintained by: Claude Code*
*Last verified: 2025-10-23*
*Branch: Feature/ScalarFloat32*
