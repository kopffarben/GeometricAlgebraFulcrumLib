# Float32 Generator - Aktionsplan für 100% Coverage

**Projekt:** GeometricAlgebraFulcrumLib.Modeling
**Aktueller Status:** 99.1% (18/~2000 Fehler)
**Ziel:** 99.5% → 100% Float32 Coverage
**Datum:** 2025-10-14
**Basierend auf:** BUGREPORT.md, CONTEXT.md, ANALYSE.md

---

## ⚡ IMMEDIATE ACTION REQUIRED (30 Minutes)

**🐛 Generator Bug Found:** 9 duplicate method errors (CS0111) due to missing float parameter check

**Quick Fix Available:**
- File: `Float32SyntaxRewriter.cs` line ~370
- Add: 3 lines of code (`HasFloatParameter` check)
- Impact: 18 → 9 errors (50% reduction)
- Time: 30 minutes

**See section below for exact code and test commands!**

---

## Executive Summary

**Aktueller Stand:**
- ✅ Algebra-Projekt: 100% (431 → 0 Fehler)
- ✅ Modeling-Projekt: 99.1% (~2000 → 18 Fehler)
- ✅ Generator-Features: AST-Transformation, Enum Support, Path Hashing
- ❌ 3 Quelldateien mit 18 Fehler (9 Generator-Bug + 9 Architektur)

**Error Breakdown:**
| Category | Count | Type | Fix Effort |
|----------|-------|------|------------|
| Duplicate Methods (CS0111) | 9 | **Generator Bug** | **30 min** ← DO THIS! |
| Interface Mismatches | 5 | Architecture | 60 min (optional) |
| Abstract Method Signature | 4 | Architecture | 45 min (optional) |

**Lösungswege:**
1. **IMMEDIATE (Recommended):** Fix generator bug (30min) → 18 → 9 errors (99.5%)
2. **Option B (Optional):** Architecture changes (3h) → 9 → 0 errors (100%)
3. **Option C (Not Recommended):** Semantic Model (2-3d) → Too much effort

**Empfehlung:** ✅ **Fix generator bug immediately, evaluate Float32 Signal usage before doing Option B**

---

## IMMEDIATE FIX: Generator Bug (30 Minutes) - DO THIS FIRST!

**Konzept:** Fix duplicate method generation by skipping methods with float parameters

### Quick Fix: Add HasFloatParameter Check

**Status:** ⬜ Not started (HIGHEST PRIORITY)

**File to Modify:** `Float32SyntaxRewriter.cs`
**Location:** Line ~370 (inside `VisitMethodDeclaration`, after `HasFloatThisParameter` check)

**Exact Code to Add:**
```csharp
// SKIP: Methods with float parameters (likely have double overloads)
// This prevents duplicates like ScalarFromNumber(float) + ScalarFromNumber(double) → both becoming float
if (HasFloatParameter(node.ParameterList))
{
    return null;  // Skip float overload, keep double overload (transforms to float)
}
```

**Test Commands:**
```bash
# 1. Rebuild generator
cd GeometricAlgebraFulcrumLib/GeometricAlgebraFulcrumLib.CodeGeneration
dotnet build

# 2. Clean generated files
cd ../GeometricAlgebraFulcrumLib.Modeling
rm -rf obj/Generated

# 3. Regenerate with clean build
dotnet build --no-incremental

# 4. Verify fix - Count CS0111 errors (should be 0)
dotnet build 2>&1 | grep "error CS0111" | wc -l

# 5. Count total errors (should be 9, down from 18)
dotnet build 2>&1 | grep "error CS" | wc -l
```

**Expected Results:**
- Before: 18 errors total (9 CS0111 + 9 architecture)
- After: 9 errors total (0 CS0111 + 9 architecture)
- **Success:** 50% error reduction in 30 minutes!

**Why This Works:**
- Operators already skip float parameters (line 291: `HasFloatParameter`)
- Extension methods skip `this float` (line 367: `HasFloatThisParameter`)
- Regular methods DON'T skip float parameters → This causes duplicates
- Adding same check for regular methods fixes the issue

**Acceptance Criteria:**
- [ ] No CS0111 duplicate method errors
- [ ] Total errors reduced from 18 to 9
- [ ] Build completes successfully
- [ ] Generated code compiles (with 9 architecture errors remaining)

**Time Estimate:** 30 minutes (5 min code + 5 min rebuild + 10 min test + 10 min validation)

---

## Option B: Architecture Changes (2-3 Stunden) - OPTIONAL

**Konzept:** Erstelle fehlende Float32-Interfaces und passe Architektur an (mehr Generics)

**Note:** Only do these if Float32 signals are actually used! Do Quick Fix first, then evaluate need.

### Checkliste

#### B.1 - ILinFloat32Vector3D Interface ⏱️ 30min

**Status:** ⬜ Nicht begonnen

**Zu erstellen:**
- Datei: `Algebra/LinearAlgebra/Float32/Vectors/Space3D/ILinFloat32Vector3D.cs`
- Analog: `ILinFloat32Vector2D.cs`
- Zeilen: ~30 (beide Interfaces)

**Code-Template:**
```csharp
namespace GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32.Vectors.Space3D;

public interface ILinFloat32Vector3D :
    ILinFloat32Vector,
    ITriplet<Float32Scalar>
{
    Float32Scalar X { get; }
    Float32Scalar Y { get; }
    Float32Scalar Z { get; }

    LinFloat32Vector3D ToLinFloat32Vector3D();
    LinFloat32Vector3D ToUnitLinFloat32Vector3D();
    Float32Scalar VectorENorm();
    Float32Scalar VectorENormSquared();
}
```

**Behebt:**
- 9 Fehler in `GrParametricSurfaceLocalFrame3D_E15E4BCA.g.cs`
- Interface Return Type Mismatch (CS0738 Fehler)

**Testing:**
```bash
dotnet build GeometricAlgebraFulcrumLib.Modeling 2>&1 | grep "CS0738" | wc -l
# Erwartung: 0 (vorher 9)
```

**Dependencies:**
- Benötigt: `ILinFloat32Vector` (sollte bereits existieren)
- Benötigt: `Float32Scalar` (bereits existiert)

**Akzeptanzkriterien:**
- [ ] ILinFloat32Vector3D.cs kompiliert
- [ ] ILinFloat32Vector2D.cs kompiliert
- [ ] `GrParametricSurfaceLocalFrame3D` kompiliert
- [ ] Keine CS0738 Fehler mehr

---

#### B.2 - IGraphicsFloat32SurfaceLocalFrame3D Interface ⏱️ 20min

**Status:** ⬜ Nicht begonnen

**Zu erstellen:**
- Datei: `Modeling/Graphics/Rendering/Surfaces/IGraphicsFloat32SurfaceLocalFrame3D.cs`
- Zeilen: ~20

**Code-Template:**
```csharp
namespace GeometricAlgebraFulcrumLib.Modeling.Graphics.Rendering.Surfaces;

public interface IGraphicsFloat32SurfaceLocalFrame3D
{
    LinFloat32Vector3D Point { get; }
    LinFloat32Vector2D ParameterValue { get; }
    LinFloat32Normal3D Normal { get; }

    bool IsValid();
}
```

**Behebt:**
- Interface-Constraint für Parametric Surface Classes
- Ergänzt B.1 für vollständige Surface-Support

**Testing:**
```bash
dotnet build GeometricAlgebraFulcrumLib.Modeling 2>&1 | grep "IGraphicsSurfaceLocalFrame3D"
# Erwartung: Keine Fehler mehr mit Float64-Version
```

**Akzeptanzkriterien:**
- [ ] Interface kompiliert
- [ ] `GrParametricSurfaceLocalFrame3D` kann Interface implementieren

---

#### B.3 - ScalarProcessorOfFloat32 Unsealed ⏱️ 10min

**Status:** ⬜ Nicht begonnen

**Zu ändern:**
- Datei: `Algebra/Scalars/Float32/ScalarProcessorOfFloat32.cs`
- Zeilen: 1 (sealed entfernen)

**Change:**
```diff
- public sealed class ScalarProcessorOfFloat32 : IScalarProcessor<Float32Scalar>
+ public class ScalarProcessorOfFloat32 : IScalarProcessor<Float32Scalar>
```

**Behebt:**
- 1 Fehler in `ScalarFunctionProcessorOfFloat32_AFF941D4.g.cs`
- Error CS0509 (Cannot inherit from sealed type)

**Testing:**
```bash
dotnet build GeometricAlgebraFulcrumLib.Modeling 2>&1 | grep "CS0509"
# Erwartung: 0 Fehler
```

**Breaking Changes:** ⚠️ Minor
- Erlaubt Subclassing, was vorher verboten war
- Keine Breaking Changes für bestehenden Code

**Akzeptanzkriterien:**
- [ ] Algebra-Projekt kompiliert weiterhin
- [ ] `ScalarFunctionProcessorOfFloat32` kompiliert in Modeling

---

#### B.4 - ScalarSignalSpectrum<T, TSampling> Generic Refactoring ⏱️ 45min

**Status:** ⬜ Nicht begonnen

**Zu ändern:**
- Datei: `Modeling/Signals/ScalarSignalSpectrum.cs` (Base Class)
- Zeilen: ~30 (Generic Parameter hinzufügen)

**Vorher:**
```csharp
public abstract class ScalarSignalSpectrum<T>
{
    protected abstract ScalarSignalSpectrum<T> CreateSignalSpectrum(
        Float64SamplingSpecs samplingSpecs,  // ❌ Hardcodiert
        Dictionary<int, SignalSpectrumSample> dict
    );
}
```

**Nachher:**
```csharp
public abstract class ScalarSignalSpectrum<T, TSamplingSpecs>
    where TSamplingSpecs : ISamplingSpecs
{
    protected abstract ScalarSignalSpectrum<T, TSamplingSpecs> CreateSignalSpectrum(
        TSamplingSpecs samplingSpecs,  // ✅ Generisch
        Dictionary<int, SignalSpectrumSample> dict
    );
}
```

**Zusätzlich zu erstellen:**
```csharp
// Modeling/Signals/ISamplingSpecs.cs
public interface ISamplingSpecs
{
    float SamplingRate { get; }
    int SampleCount { get; }
}

// Float32SamplingSpecs.cs implementiert ISamplingSpecs
// Float64SamplingSpecs.cs implementiert ISamplingSpecs
```

**Behebt:**
- 2 Fehler in `Float32SignalSpectrum_CD7A20A8.g.cs` (CS0115, CS0534)
- 2 Fehler in `Float32ComplexSignalSpectrum_8CDE8F0E.g.cs` (CS0115, CS0534)

**Testing:**
```bash
dotnet build GeometricAlgebraFulcrumLib.Modeling 2>&1 | grep "Float32SignalSpectrum"
# Erwartung: 0 Fehler
```

**Breaking Changes:** ⚠️ Major
- Alle `ScalarSignalSpectrum<T>` → `ScalarSignalSpectrum<T, TSamplingSpecs>`
- Migration nötig: ~10 Dateien updaten

**Migration-Plan:**
1. Suche: `ScalarSignalSpectrum<` in Modeling-Projekt
2. Ersetze: `ScalarSignalSpectrum<Complex>` → `ScalarSignalSpectrum<Complex, Float64SamplingSpecs>`
3. Teste: Algebra-Projekt (falls betroffen)

**Akzeptanzkriterien:**
- [ ] ISamplingSpecs Interface definiert
- [ ] Float32/Float64SamplingSpecs implementieren Interface
- [ ] ScalarSignalSpectrum<T, TSamplingSpecs> kompiliert
- [ ] Alle abhängigen Klassen migriert
- [ ] Keine CS0115/CS0534 Fehler mehr

---

#### B.5 - IScalarProcessor<T, TScalar> Generic Parameter ⏱️ 60min

**Status:** ⬜ Nicht begonnen

**Zu ändern:**
- Datei: `Algebra/Scalars/IScalarProcessor.cs`
- Zeilen: ~40

**Vorher:**
```csharp
public interface IScalarProcessor<T>
{
    double ZeroEpsilon { get; }              // ❌ Hardcodiert
    T ScalarFromNumber(double value);        // ❌ Hardcodiert
    double ToFloat64(T scalar);
    T ScalarFromRandom(Random rnd, double min, double max);  // ❌
}
```

**Nachher:**
```csharp
public interface IScalarProcessor<T, TScalar = double>  // Default für Backward-Compatibility
    where TScalar : struct, IConvertible
{
    TScalar ZeroEpsilon { get; }             // ✅ Generisch
    T ScalarFromNumber(TScalar value);       // ✅ Generisch
    double ToFloat64(T scalar);              // Bleibt double (Konvertierung)
    T ScalarFromRandom(Random rnd, TScalar min, TScalar max);  // ✅
}
```

**Behebt:**
- 5 Fehler in `ScalarProcessorOfFloat32Signal_1340A8DA.g.cs`
  - CS0535: Missing ScalarFromNumber(double)
  - CS0535: Missing ToFloat64()
  - CS0535: Missing ScalarFromRandom()
  - CS0738: ZeroEpsilon wrong type
  - CS0111: Duplicate ScalarFromNumber

**Testing:**
```bash
dotnet build GeometricAlgebraFulcrumLib.Modeling 2>&1 | grep "ScalarProcessorOfFloat32Signal"
# Erwartung: 0 Fehler
```

**Breaking Changes:** ⚠️ Minor (mit Default Parameter)
- Alte `IScalarProcessor<T>` nutzt automatisch `TScalar = double`
- Neue Verwendung: `IScalarProcessor<Float32SampledTimeSignal, float>`

**Migration-Plan:**
1. Suche: `IScalarProcessor<` im gesamten Solution
2. Prüfe: Welche explizit `double` erwarten (bleiben unverändert)
3. Update: Float32-spezifische Implementierungen zu `<T, float>`

**Akzeptanzkriterien:**
- [ ] IScalarProcessor<T, TScalar> kompiliert
- [ ] Default Parameter `= double` funktioniert
- [ ] ScalarProcessorOfFloat32Signal nutzt `<Float32SampledTimeSignal, float>`
- [ ] Algebra-Projekt weiterhin kompatibel
- [ ] Keine CS0535/CS0738/CS0111 Fehler mehr

---

### Option B - Zusammenfassung & Timeline

| Task | Zeit | Fehler behoben | Priorität |
|------|------|----------------|-----------|
| B.1 - ILinFloat32Vector3D | 30min | 9 | HIGH |
| B.2 - IGraphicsFloat32Surface | 20min | (ergänzend) | MEDIUM |
| B.3 - Unsealed ScalarProcessor | 10min | 1 | LOW |
| B.4 - SignalSpectrum Generic | 45min | 4 | HIGH |
| B.5 - IScalarProcessor Generic | 60min | 5 | HIGH |
| **Gesamt** | **2h 45min** | **19** | - |

**Empfohlene Reihenfolge:**
1. **B.1** (9 Fehler) + **B.2** (ergänzend) = **1h**
2. **B.3** (1 Fehler, trivial) = **10min**
3. **B.4** (4 Fehler) = **45min**
4. **B.5** (5 Fehler) = **60min**

**Total:** ~3 Stunden → **100% Modeling Coverage**

---

## Option C: Semantic Model Integration (2-3 Tage)

**Konzept:** Erweitere Generator um Roslyn Semantic Model für automatische Interface/Base Class Transformation.

### Checkliste

#### C.1 - Semantic Model Setup ⏱️ 2h

**Status:** ⬜ Nicht begonnen

**Zu implementieren:**
- Datei: `Float32SourceGenerator.cs` (erweitern)
- Zeilen: ~50

**Code-Skelett:**
```csharp
public override void Initialize(IncrementalGeneratorInitializationContext context)
{
    // NEU: Kombination mit Compilation
    var compilationProvider = context.CompilationProvider;

    var semanticFiles = context.AdditionalTextsProvider
        .Combine(compilationProvider);

    context.RegisterSourceOutput(semanticFiles, (ctx, combined) =>
    {
        var (file, compilation) = combined;
        var syntaxTree = CSharpSyntaxTree.ParseText(file.GetText());
        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        // Nutze Semantic Model für Transformationen
        var rewriter = new SemanticFloat32SyntaxRewriter(semanticModel);
        var newRoot = rewriter.Visit(syntaxTree.GetRoot());
        // ...
    });
}
```

**Herausforderungen:**
- Performance: Semantic Model ~10x langsamer als AST-only
- Memory: Compilation Context im RAM (~150 MB)
- Null-Handling: Compilation kann bei Parse-Errors null sein

**Akzeptanzkriterien:**
- [ ] Compilation Provider funktioniert
- [ ] Semantic Model wird korrekt erstellt
- [ ] Performance akzeptabel (<5s für 476 Dateien)

---

#### C.2 - Interface Detection & Transformation ⏱️ 4h

**Status:** ⬜ Nicht begonnen

**Zu implementieren:**
- Datei: `SemanticFloat32SyntaxRewriter.cs` (neu)
- Zeilen: ~100

**Funktionalität:**
```csharp
public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
{
    var classSymbol = _semanticModel.GetDeclaredSymbol(node);
    if (classSymbol == null) return base.VisitClassDeclaration(node);

    // Analysiere Interfaces
    var transformedInterfaces = new List<BaseTypeSyntax>();
    foreach (var iface in classSymbol.Interfaces)
    {
        if (iface.Name.Contains("Float64"))
        {
            var float32Name = iface.Name.Replace("Float64", "Float32");
            // Transformiere Interface-Referenz
            transformedInterfaces.Add(...);
        }
    }

    // Update BaseList
    var newNode = node.WithBaseList(...);
    return base.VisitClassDeclaration(newNode);
}
```

**Problem:** **Henne-Ei-Dilemma**
- Interface I_Float64 → I_Float32 transformieren
- ABER: I_Float32 existiert noch nicht (wird erst später generiert)
- **Lösung:** Multi-Pass Generator (2 Durchläufe)

**Akzeptanzkriterien:**
- [ ] Interface-Referenzen werden erkannt
- [ ] Float64 → Float32 Transformation funktioniert
- [ ] Henne-Ei-Problem gelöst (Multi-Pass oder Pre-Generation)

---

#### C.3 - Generic Type Argument Resolution ⏱️ 3h

**Status:** ⬜ Nicht begonnen

**Zu implementieren:**
- Datei: `SemanticFloat32SyntaxRewriter.cs` (erweitern)
- Zeilen: ~80

**Funktionalität:**
```csharp
public override SyntaxNode? VisitGenericName(GenericNameSyntax node)
{
    var typeArgs = node.TypeArgumentList.Arguments;
    var transformedArgs = new List<TypeSyntax>();

    foreach (var typeArg in typeArgs)
    {
        var typeSymbol = _semanticModel.GetSymbolInfo(typeArg).Symbol as ITypeSymbol;
        if (typeSymbol?.Name.Contains("Float64") == true)
        {
            var float32Name = typeSymbol.Name.Replace("Float64", "Float32");
            transformedArgs.Add(SyntaxFactory.ParseTypeName(float32Name));
        }
        else
        {
            transformedArgs.Add(typeArg);
        }
    }

    return node.WithTypeArgumentList(
        SyntaxFactory.TypeArgumentList(
            SyntaxFactory.SeparatedList(transformedArgs)
        )
    );
}
```

**Herausforderungen:**
- Nested Generics: `Dictionary<int, List<Float64Scalar>>`
- Qualified Names: `GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64.Float64Scalar`

**Akzeptanzkriterien:**
- [ ] Generic Type Arguments werden transformiert
- [ ] `ITriplet<Float64Scalar>` → `ITriplet<Float32Scalar>`
- [ ] Nested Generics funktionieren

---

#### C.4 - Dependency Graph Builder ⏱️ 4h

**Status:** ⬜ Nicht begonnen

**Zu implementieren:**
- Datei: `DependencyGraphBuilder.cs` (neu)
- Zeilen: ~120

**Funktionalität:**
1. Scan alle AdditionalFiles
2. Build Dependency Graph (Interface → Class Dependencies)
3. Topological Sort für Generierungs-Reihenfolge
4. Multi-Pass Generation

**Pseudo-Code:**
```csharp
public class DependencyGraphBuilder
{
    public List<AdditionalText> OrderByDependencies(IEnumerable<AdditionalText> files)
    {
        var graph = new Dictionary<string, List<string>>();

        // Build Graph
        foreach (var file in files)
        {
            var dependencies = ExtractDependencies(file);
            graph[file.Path] = dependencies;
        }

        // Topological Sort
        return TopologicalSort(graph);
    }
}
```

**Problem:** Roslyn Generators sind **file-by-file**, kein Multi-Pass API

**Workaround:**
- Pass 1: Generiere alle Interfaces
- Pass 2: Generiere alle Classes
- **Implementierung:** Zwei separate `RegisterSourceOutput` Calls

**Akzeptanzkriterien:**
- [ ] Dependency Graph wird korrekt gebaut
- [ ] Topological Sort funktioniert
- [ ] Multi-Pass Generation funktioniert

---

#### C.5 - Circular Dependency Detection ⏱️ 2h

**Status:** ⬜ Nicht begonnen

**Zu implementieren:**
- Datei: `DependencyGraphBuilder.cs` (erweitern)
- Zeilen: ~60

**Funktionalität:**
```csharp
private void DetectCycles(Dictionary<string, List<string>> graph)
{
    var visited = new HashSet<string>();
    var recursionStack = new HashSet<string>();

    foreach (var node in graph.Keys)
    {
        if (HasCycle(node, graph, visited, recursionStack))
        {
            // Report Diagnostic
            context.ReportDiagnostic(Diagnostic.Create(
                "FLOAT32GEN003",
                "Circular Interface Dependency detected",
                ...
            ));
        }
    }
}
```

**Akzeptanzkriterien:**
- [ ] Circular Dependencies werden erkannt
- [ ] Diagnostics werden gemeldet
- [ ] Generator bricht nicht ab

---

#### C.6 - Testing & Debugging ⏱️ 4h

**Status:** ⬜ Nicht begonnen

**Tasks:**
1. Unit Tests für Semantic Model Integration
2. Integration Tests (Algebra + Modeling)
3. Performance-Tests (Benchmark vs AST-only)
4. Edge Case Testing (Nested Generics, Circular Deps)

**Test-Framework:**
- Roslyn SourceGenerator Testing Package
- Snapshot-based Testing (Verify)
- BenchmarkDotNet für Performance

**Akzeptanzkriterien:**
- [ ] Alle Tests grün
- [ ] Performance <10s für 476 Dateien
- [ ] Edge Cases abgedeckt

---

### Option C - Zusammenfassung & Timeline

| Phase | Tasks | Zeit | Komplexität |
|-------|-------|------|-------------|
| **Setup** | C.1 Semantic Model | 2h | Hoch |
| **Core** | C.2 Interface + C.3 Generics | 7h | Sehr Hoch |
| **Advanced** | C.4 Dependency Graph + C.5 Cycles | 6h | Extrem Hoch |
| **QA** | C.6 Testing & Debugging | 4h | Hoch |
| **Gesamt** | | **19h** | **Sehr Hoch** |

**Realistische Schätzung:** 2-3 Arbeitstage (mit Pausen, Debugging)

**Risiken:**
- 🔴 **Hoch:** Henne-Ei-Problem schwer lösbar
- 🟡 **Mittel:** Performance-Einbußen
- 🟡 **Mittel:** Circular Dependencies
- 🟢 **Niedrig:** API-Kompatibilität

---

## Vergleich: Option B vs C

| Kriterium | Option B | Option C |
|-----------|----------|----------|
| **Aufwand** | 3 Stunden | 2-3 Tage |
| **Risiko** | Niedrig | Mittel-Hoch |
| **Wartung** | +6 Dateien (~125 Zeilen) | +Generator-Komplexität (~410 Zeilen) |
| **ROI** | ⭐⭐⭐⭐⭐ Exzellent | ⭐⭐ Fragwürdig |
| **Time-to-Market** | ✅ Sofort | ⏱️ 1 Woche |
| **Skalierbarkeit** | ⚠️ Niedrig | ✅ Hoch |
| **Code Quality** | Mix (Gen + Manual) | ✅ 100% Gen |

---

## Empfehlung & Nächste Schritte

### ✅ Sofort (Option B): Manuelle Float32-Versionen

**Begründung:**
- 96% → 100% in 3 Stunden
- Niedriges Risiko, hoher ROI
- Modeling-Projekt sofort einsatzbereit

**Start mit:**
1. **B.1 + B.2** (1 Stunde) → Behebt 9 Fehler
2. **B.3** (10 Minuten) → Behebt 1 Fehler
3. **B.4** (45 Minuten) → Behebt 4 Fehler
4. **B.5** (60 Minuten) → Behebt 5 Fehler

**Nach Completion:**
- ✅ 100% Modeling Coverage
- ✅ Alle Tests grün
- ✅ Produktiv einsatzbereit

---

### 🔮 Langfristig (Option C): Semantic Model Integration

**Erwägen wenn:**
- Weitere Projekte Float32-Support benötigen
- >50 Interface-Dependencies betroffen
- Budget für 1 Woche Entwicklung vorhanden
- Generator als Open Source oder Produkt geplant

**Vorteile:**
- Skaliert auf beliebige Projekte
- 100% Generator-Only (keine manuellen Änderungen)
- Wiederverwendbar

**Empfehlung:** Als Roadmap-Item notieren, aber nicht für aktuelle 19 Fehler

---

## Status Tracking

### Gesamt-Fortschritt

- [x] BUGREPORT.md erstellt (19 Fehler analysiert)
- [x] CONTEXT.md erstellt (Architektur dokumentiert)
- [x] ANALYSE.md erstellt (Option B vs C)
- [x] TODO.md erstellt (dieser Plan)
- [ ] Option B umgesetzt (0/5 Tasks)
- [ ] 100% Modeling Coverage erreicht

### Option B Tasks Status

- [ ] B.1 - ILinFloat32Vector3D (30min)
- [ ] B.2 - IGraphicsFloat32Surface (20min)
- [ ] B.3 - Unsealed ScalarProcessor (10min)
- [ ] B.4 - SignalSpectrum Generic (45min)
- [ ] B.5 - IScalarProcessor Generic (60min)

**Gesamtfortschritt:** 0/5 (0%)

---

## Referenzen

- **BUGREPORT.md** - Detaillierte Fehler-Analyse (19 Fehler, 5 Quelldateien)
- **CONTEXT.md** - Generator-Architektur & Limitationen
- **ANALYSE.md** - Methodische Analyse Option B vs C
- **Float32SourceGenerator.cs** - Generator Entry Point
- **Float32SyntaxRewriter.cs** - AST Transformation Logic

**Build-Commands:**
```bash
# Generator rebuilden
dotnet build GeometricAlgebraFulcrumLib.CodeGeneration/

# Modeling mit Generator testen
rm -rf GeometricAlgebraFulcrumLib.Modeling/obj/Generated
dotnet build GeometricAlgebraFulcrumLib.Modeling/ --no-incremental

# Fehler zählen
dotnet build GeometricAlgebraFulcrumLib.Modeling/ 2>&1 | grep "error CS" | wc -l
```
