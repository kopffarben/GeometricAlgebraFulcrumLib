# Float32 Generator - Architektur & Kontext

**Version:** v1.0.0  
**Datum:** 2025-10-14  
**Projekte:** Algebra (100%) + Modeling (96%)  
**Typ:** Roslyn Incremental Source Generator

## Executive Summary

**Erfolgsrate:** 97.8% (840 Dateien, 19/~2000 Fehler verbleibend)

Der Generator transformiert Float64→Float32 via AST-Manipulation mit `CSharpSyntaxRewriter`.  
**Limitationen:** Keine Semantic Analysis → Interface/Base Class Dependencies nicht aufgelöst.

---

## Architektur

### Float32SourceGenerator.cs (Entry Point)

```
AdditionalTextsProvider (476 *.cs Dateien)
    ↓ Filter: /Float64/ in Path, exclude /obj//bin/
RegisterSourceOutput
    ↓ Parse: CSharpSyntaxTree.ParseText()
Float32SyntaxRewriter.Visit(syntaxTree)
    ↓ Transform: Namespaces, Types, Methods, Expressions
AddSource(hintName, transformedCode)
    ↓ Output: obj/Generated/GAF.Gen/GAF.Gen.F32Gen/*.g.cs
```

### Float32SyntaxRewriter.cs (AST Visitor)

**Transformations:**
- `double` → `float`
- `*Float64*` → `*Float32*` (Class/Struct/Enum Namen)
- `Math.*` → `MathF.*`
- `1.5` → `1.5f` (Literals)
- `Vector<double>` → `Vector<float>` (MathNet.Numerics)
- `BitConverter.*Double*` → `*Single*`

**Special Handling:**
- Method Chaining: `.L2Norm()` cast injection
- `ToLinVector()` → `ToLinFloat32Vector()`
- Enum Declarations (✅ v1.0.0 Feature)

---

## Erfolge

### Algebra-Projekt: 431 → 0 Fehler (100%)
- 364 Float64 → 375 Float32 Dateien generiert
- Alle Transformationen funktionieren

### Modeling-Projekt: ~2000 → 19 Fehler (96.0%)
- 476 Float64 → 476 Float32 Dateien generiert
- 19 Fehler aus 5 Quelldateien (Interface-Abhängigkeiten)

---

## Limitationen (AST-Only, ohne Semantic Model)

**1. Interface Transformation fehlt**
```csharp
// Float64:
public class MyClass : ILinFloat64Vector3D { }

// Generiert:
public class MyClass : ILinFloat64Vector3D { }  // ❌ NICHT transformiert!
```

**2. Generic Type Arguments**
```csharp
// Float64:
public class MyClass : ITriplet<Float64Scalar> { }

// Generiert:
public class MyClass : ITriplet<Float64Scalar> { }  // ❌ NICHT transformiert!
```

**3. Sealed Base Classes**
```csharp
// ScalarProcessorOfFloat32 ist sealed (Algebra-Projekt)
public class ScalarFunctionProcessorOfFloat32 : ScalarProcessorOfFloat32 { }
// ❌ Error CS0509: Cannot inherit from sealed type
```

**4. Abstract Method Signatures**
```csharp
// Base class erwartet Float64SamplingSpecs
protected override Float32SignalSpectrum CreateSignalSpectrum(
    Float32SamplingSpecs specs, ...  // ❌ Base class Signatur: Float64SamplingSpecs
)
```

---

## Performance

| Metrik | Algebra | Modeling | Gesamt |
|--------|---------|----------|--------|
| Dateien | 375 | 476 | 851 |
| Gen-Zeit | ~2s | ~3s | ~5s |
| Build | ~10s | ~20s | ~30s |
| Overhead | 20% | 15% | 17% |

**Incremental Compilation:** ✅ Nur geänderte Dateien werden regeneriert

---

## Known Issues (19 Fehler in 5 Dateien)

Siehe **BUGREPORT.md** für Details.

**Kategorien:**
1. Interface Return Type Mismatch (9 Fehler)
2. Sealed Base Class (1 Fehler)
3. Abstract Method Signature (4 Fehler)
4. Interface Member Missing (5 Fehler)

**Lösung:** Option B (Manuelle Float32-Interfaces) oder Option C (Semantic Model)
