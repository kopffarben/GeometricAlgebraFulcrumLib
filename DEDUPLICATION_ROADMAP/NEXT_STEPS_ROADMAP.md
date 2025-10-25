# Next Steps Roadmap - Konkrete Aktionen

**Letzte Aktualisierung:** 2025-10-25
**Aktueller Status:** Phase 1.1 In Progress (Module 1: XGa Core - Tasks 1.1, 1.2 & 1.3 Complete)
**Branch:** Feature/ScalarFloat32

---

## ⚠️ DOKUMENTATIONSPFLEGE

**Diese Dateien synchron halten:**
1. **`DEDUPLICATION_ROADMAP.md`** - Gesamt-Roadmap
2. **`NEXT_STEPS_ROADMAP.md`** (dieses Dokument) - Nächste Schritte
3. **`DEDUPLICATION_TASKS.md`** - Detaillierte Tasks

Nach jedem Meilenstein alle drei aktualisieren!

---

## 🎯 Wo sind wir?

**Phase 0:** ✅ COMPLETE
- API-Analyse komplett
- Performance validiert (Generic 1.27x schneller)
- 102 Equivalence-Tests passing
- Roadmap erstellt

**Phase 1.1:** 🔄 IN PROGRESS (Module 1: XGa Core)
- Task 1.1 (XGaComputedOutermorphism<T>) ✅ COMPLETE
- Task 1.2 (XGaStoredOutermorphism<T>) ✅ COMPLETE
- Task 1.3 (XGaOutermorphismComposerUtils<T>) ✅ COMPLETE
- 2 Klassen verbleibend in Generic
- Geschätzter Aufwand: 1-2 Tage verbleibend
- Nach Completion: Phase 1.2 (ComplexAlgebra)

---

## 📋 Module 1: XGa Core - Erste Schritte

### Was fehlt in Generic (Priorität):

1. ~~**XGaComputedOutermorphism<T>**~~ ✅ COMPLETE (2025-10-24)
2. ~~**XGaStoredOutermorphism<T>**~~ ✅ COMPLETE (2025-10-25)
3. ~~**XGaOutermorphismComposerUtils<T>**~~ ✅ COMPLETE (2025-10-25)
4. **XGaGramSchmidtFrame<T>** ← START HIER (Task 1.4)
5. **XGaConformalComposerUtils<T>**
6. **ToTuple()** extensions (optional)

---

## 🚀 Tag 1: XGaComputedOutermorphism<T>

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

## 🚀 Tag 4: XGaGramSchmidtFrame<T>

**Frame-Klasse**

```
Float64: GeometricAlgebra/Float64/Frames/XGaFloat64GramSchmidtFrame.cs
Generic: GeometricAlgebra/Generic/Frames/XGaGramSchmidtFrame.cs (NEU)
```

**Wichtig:** Gram-Schmidt Orthogonalisierung
- Numerisch sensibel
- Korrekte Verwendung von ScalarProcessor für Norm/Dot-Produkt

**Geschätzter Aufwand:** 4-5 Stunden

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
- [ ] XGaGramSchmidtFrame<T> implementiert und getestet ← NEXT
- [ ] XGaConformalComposerUtils<T> implementiert und getestet
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

## 🎯 Nach Module 1: Phase 1.2

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

*Dokument maintained by: Claude Code*
*Last verified: 2025-10-25*
*Branch: Feature/ScalarFloat32*
