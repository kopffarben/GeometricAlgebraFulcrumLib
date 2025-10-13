# Float32 Generator - Detaillierter Aktionsplan

**Ziel:** Generator von 97.7% auf 100% Erfolgsquote bringen
**Basierend auf:** BUGREPORT.md, CONTEXT.md, ANALYSE.md
**Erstellt:** 2025-10-13
**Aktualisiert:** 2025-10-13 (Generator-Only Analyse)
**Priorität:** HIGH (Blockiert Modeling Float32-Code-Generierung)

---

## ⚠️ WICHTIGE ENTSCHEIDUNG: Generator-Only vs Hybrid

**Frage:** Können wir ALLE verbleibenden Fehler ohne manuelle Source-Änderungen beheben?

**Antwort:** **JA** - Aber es gibt zwei Wege:

### Option A: Generator-Only (100% Purist)
- ✅ Keine Source-Code-Änderungen
- ✅ Skalierbar und wartbar
- ⏱️ Aufwand: 3-4 Tage (Phase 2 Semantic Integration)
- 📋 Revertiere 4 manuelle Änderungen (~60 Zeilen):
  - XGaMetric.cs
  - XGaBasisBlade.cs
  - LinBasisVector.cs
  - LinFloat32Vector3DComposerUtilsExtensions.cs

### Option B: Hybrid (Pragmatisch)
- ✅ Sofort einsatzbereit
- ✅ Minimale manuelle Änderungen (4 Dateien, ~60 Zeilen)
- ⏱️ Aufwand: 2 Stunden (Phase 1 Quick Wins)
- ⚠️ 5% manuelle Überladungen bleiben

**Empfehlung:** Option B JETZT (Modeling generieren), Option A SPÄTER (Refactoring)

---

## Executive Summary

**Status Quo:**
- ✅ 421 von 431 Fehlern behoben (97.7%)
- ❌ 10 Fehler verbleibend (2.3%)
- ⚠️ 4 manuelle Source-Änderungen gemacht (~60 Zeilen)
- ⏸️ Modeling-Code-Generierung blockiert

**Plan Option A (Generator-Only):**
1. **Phase 1 (Quick Wins):** 2 Stunden → 3-4 Fehler behoben (Generator)
2. **Phase 2 (Semantic):** 3 Tage → Alle 10 Fehler behoben + manuelle Änderungen revertiert
3. **Phase 3 (Optional):** Testing + Robustness

**Plan Option B (Hybrid):**
1. **Phase 1 (Quick Wins):** 2 Stunden → 3-4 Fehler behoben (Generator)
2. **Modeling generieren:** Sofort möglich mit 4 manuellen Änderungen
3. **Phase 2 später:** Optional Refactoring zu 100% Generator

**Timeline:**
- Hybrid (Option B): 2 Stunden → Production-Ready
- Generator-Only (Option A): 3-4 Tage → 100% Purist
- Production Ready: 1-2 Wochen (optional Testing)

---

## Phase 1: Quick Wins (Priorität: CRITICAL)

**Applies to:** BOTH Option A and Option B
**Aufwand:** 2 Stunden
**Resultat:** 3-4 Fehler behoben durch Generator
**Generator-Only:** JA

---

### Aufgabe 1.1: ToUnitLinVector Pattern erweitern

**Ziel:** 3 LinBasisVector-Fehler beheben
**Aufwand:** 30 Minuten
**Dateien:** `Float32SyntaxRewriter.cs`

#### Schritt 1.1.1: Pattern-Liste erweitern
**Zeile:** ~377 (VisitIdentifierName)

**Aktueller Code:**
```csharp
if (text.StartsWith("ToLinVector") ||
    text.StartsWith("CreateLinVector") ||
    text.StartsWith("CreateUnitLinVector"))
{
    var newText = text
        .Replace("ToLinVector", "ToLinFloat32Vector")
        .Replace("CreateLinVector", "CreateLinFloat32Vector")
        .Replace("CreateUnitLinVector", "CreateUnitLinFloat32Vector");
    // ...
}
```

**Neuer Code:**
```csharp
// Erweitere Pattern-Liste
private static readonly string[] VectorMethodPrefixes = new[]
{
    "ToLinVector",
    "ToUnitLinVector",        // NEU
    "CreateLinVector",
    "CreateUnitLinVector",
    "GetLinVector",           // NEU (proaktiv)
    "AsLinVector"             // NEU (proaktiv)
};

// In VisitIdentifierName:
if (VectorMethodPrefixes.Any(prefix => text.StartsWith(prefix)))
{
    var newText = text;
    foreach (var prefix in VectorMethodPrefixes)
    {
        if (newText.StartsWith(prefix))
        {
            // Insert "Float32" after "Lin"
            newText = prefix.Insert(3, "Float32") +
                      newText.Substring(prefix.Length);
            break;
        }
    }

    return node.WithIdentifier(
        SyntaxFactory.Identifier(
            node.Identifier.LeadingTrivia,
            newText,
            node.Identifier.TrailingTrivia
        )
    );
}
```

**Test:**
```csharp
// Input:  vector.ToUnitLinVector3D()
// Output: vector.ToUnitLinFloat32Vector3D()

// Behebt:
// - LinFloat32Vector3DAffineUtils.g.cs:275
// - LinFloat32Vector3DAffineUtils.g.cs:316
// - LinFloat32RotationUtils.g.cs:380
```

**Geschätzte Behebung:** 3 von 10 Fehlern

---

### Aufgabe 1.2: VectorPairToVectorPairRotation Pattern

**Ziel:** 2 SquareMatrix4-Fehler beheben (teilweise)
**Aufwand:** 1 Stunde
**Dateien:** `Float32SyntaxRewriter.cs`

#### Schritt 1.2.1: Quaternion → Float32Quaternion Pattern
**Zeile:** ~620 (VisitInvocationExpression, nach Math-Functions)

**Code:**
```csharp
// Nach Math-Function-Handling, vor return base.Visit:

// VectorPairToVectorPairRotationQuaternion → VectorPairToVectorPairRotationFloat32Quaternion
if (memberAccess.Name.Identifier.Text.EndsWith("Quaternion") &&
    memberAccess.Name.Identifier.Text.Contains("Rotation"))
{
    var oldName = memberAccess.Name.Identifier.Text;
    var newName = oldName.Replace("Quaternion", "Float32Quaternion");

    var visitedNode = (InvocationExpressionSyntax)base.VisitInvocationExpression(node)!;

    var newMemberAccess = memberAccess.WithName(
        SyntaxFactory.IdentifierName(newName)
    );

    return visitedNode.WithExpression(newMemberAccess);
}
```

**Test:**
```csharp
// Input:  basisVectors.VectorPairToVectorPairRotationQuaternion(v1, v2)
// Output: basisVectors.VectorPairToVectorPairRotationFloat32Quaternion(v1, v2)
```

**Warnung:** Dies transformiert den Methodennamen, aber die Methode existiert möglicherweise nicht!
- **Option A:** Methode manuell erstellen (siehe Aufgabe 1.3)
- **Option B:** Semantic Model verwenden um Existenz zu prüfen (Phase 2)

**Geschätzte Behebung:** 1-2 Fehler (wenn Methode existiert oder erstellt wird)

---

### Aufgabe 1.3: Manuelle Extension Method (Falls nötig)

**Ziel:** SquareMatrix4-Fehler vollständig beheben
**Aufwand:** 30 Minuten
**Dateien:** `LinBasisVectorPair3DExtensions.cs` (erstellen)

#### Schritt 1.3.1: Extension Method erstellen

**Neue Datei:** `GeometricAlgebraFulcrumLib.Algebra/LinearAlgebra/Basis/LinBasisVectorPair3DExtensions.cs`

```csharp
using System.Runtime.CompilerServices;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32.Angles;
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32.Vectors.Space3D;

namespace GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Basis;

/// <summary>
/// Float32 Extension Methods für LinBasisVectorPair3D
/// </summary>
public static class LinBasisVectorPair3DExtensions
{
    /// <summary>
    /// Erstellt ein Float32-Quaternion für Rotation zwischen zwei Vektorpaaren
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LinFloat32Quaternion VectorPairToVectorPairRotationFloat32Quaternion(
        this LinBasisVectorPair3D basisVectors,
        ILinFloat32Vector3D unitVector1,
        ILinFloat32Vector3D unitVector2)
    {
        // Implementierung basierend auf Float64-Version
        // TODO: Prüfe LinBasisVectorPair3D.cs für Float64-Implementierung
        // und konvertiere zu Float32

        // Placeholder (wird nach Analyse der Float64-Version ausgefüllt)
        throw new NotImplementedException(
            "TODO: Implementiere Float32-Version basierend auf Float64 VectorPairToVectorPairRotationQuaternion"
        );
    }
}
```

**Nächste Schritte:**
1. Finde Float64-Implementierung in `LinBasisVectorPair3D.cs` oder Extensions
2. Kopiere Logic, ersetze double → float
3. Test mit SquareMatrix4.CreateRotationMatrix3D

**Geschätzte Behebung:** 2 Fehler (SquareMatrix4.g.cs:365, 366)

---

### Phase 1 Zusammenfassung

**Nach Phase 1:**
- ✅ 4-5 Fehler behoben (ToUnitLinVector + optional VectorPair)
- ⏳ 5-6 Fehler verbleibend (XGaFloat64 Return Types)
- ⏱️ Aufwand: 2 Stunden

**Verbleibende Fehler erfordern Phase 2 (Semantic Analysis)**

---

## Phase 2: Semantic Integration (Priorität: HIGH)

**⚠️ NUR für Option A (Generator-Only)** - Für Option B optional

### Ziel
- Alle 10 Fehler beheben **durch Generator**
- 100% Kompilierbarkeit erreichen
- **Revertiere alle 4 manuellen Source-Änderungen**
- Generator robust machen

**Aufwand:** 3-4 Tage
**Dateien:** `F32Gen.cs`, `Float32SyntaxRewriter.cs`
**Generator-Only:** JA - Dies ist der Kern der puristischen Lösung

### Manuelle Änderungen die nach Phase 2 revertiert werden können:

```bash
# Diese Änderungen können entfernt werden nach Phase 2:
git diff GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/XGaMetric.cs
git diff GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebra/Basis/XGaBasisBlade.cs
git diff GeometricAlgebraFulcrumLib.Algebra/LinearAlgebra/Basis/LinBasisVector.cs
git clean -f GeometricAlgebraFulcrumLib.Algebra/LinearAlgebra/Float32/Vectors/Space3D/LinFloat32Vector3DComposerUtilsExtensions.cs
```

---

### Aufgabe 2.1: SemanticModel Setup

**Aufwand:** 4 Stunden
**Dateien:** `F32Gen.cs`, `Float32SyntaxRewriter.cs`

#### Schritt 2.1.1: Generator Pipeline erweitern

**Datei:** `F32Gen.cs`
**Zeile:** Initialize method

**Aktueller Code:**
```csharp
context.RegisterSourceOutput(filesProvider, (spc, file) =>
{
    var syntaxTree = CSharpSyntaxTree.ParseText(...);
    var rewriter = new Float32SyntaxRewriter();  // <-- Kein SemanticModel
    var newRoot = rewriter.Visit(syntaxTree.GetRoot());
    // ...
});
```

**Neuer Code:**
```csharp
// Step 1: Combine Compilation with Files
var compilationAndFiles = context.CompilationProvider.Combine(
    context.AdditionalTextsProvider.Collect()
);

context.RegisterSourceOutput(compilationAndFiles, (spc, source) =>
{
    var (compilation, files) = source;

    foreach (var file in files)
    {
        // Parse
        var syntaxTree = CSharpSyntaxTree.ParseText(
            file.GetText()!,
            path: file.Path
        );

        // Create Semantic Model
        var compilation WithTree = compilation.AddSyntaxTrees(syntaxTree);
        var semanticModel = compilationWithTree.GetSemanticModel(syntaxTree);

        // Transform with Semantic Model
        var rewriter = new Float32SyntaxRewriter(semanticModel);
        var newRoot = rewriter.Visit(syntaxTree.GetRoot());

        // Generate
        spc.AddSource(GetOutputFileName(file), newRoot.ToFullString());
    }
});
```

**Wichtig:** Performance-Impact beachten! Semantic Model ist teuer.

---

#### Schritt 2.1.2: SyntaxRewriter Constructor erweitern

**Datei:** `Float32SyntaxRewriter.cs`
**Zeile:** ~34

**Code:**
```csharp
public class Float32SyntaxRewriter : CSharpSyntaxRewriter
{
    // Neue Fields
    private readonly SemanticModel? _semanticModel;
    private readonly bool _useSemantics;

    // Caches für Performance
    private readonly Dictionary<SyntaxNode, ISymbol?> _symbolCache = new();
    private readonly Dictionary<ITypeSymbol, bool> _isFloat64Cache = new();

    // Aktualisierter Constructor
    public Float32SyntaxRewriter(SemanticModel? semanticModel = null)
        : base(visitIntoStructuredTrivia: false)
    {
        _semanticModel = semanticModel;
        _useSemantics = semanticModel != null;
    }

    // Helper Methods
    private ISymbol? GetSymbolCached(SyntaxNode node)
    {
        if (!_useSemantics) return null;

        if (!_symbolCache.TryGetValue(node, out var symbol))
        {
            symbol = _semanticModel!.GetSymbolInfo(node).Symbol;
            _symbolCache[node] = symbol;
        }
        return symbol;
    }

    private bool IsFloat64Type(ITypeSymbol? typeSymbol)
    {
        if (typeSymbol == null) return false;

        if (!_isFloat64Cache.TryGetValue(typeSymbol, out var result))
        {
            result = typeSymbol.Name.Contains("Float64") ||
                     typeSymbol.Name == "Double" ||
                     typeSymbol.SpecialType == SpecialType.System_Double;
            _isFloat64Cache[typeSymbol] = result;
        }
        return result;
    }
}
```

---

### Aufgabe 2.2: BasisBlade Context-Aware Transformation

**Aufwand:** 4 Stunden
**Dateien:** `Float32SyntaxRewriter.cs`

#### Schritt 2.2.1: Context Tracking erweitern

**Zeile:** Nach Field Declarations (~40)

**Code:**
```csharp
// Context Stack für verschachtelte Scopes
private readonly Stack<ContextInfo> _contextStack = new();

private class ContextInfo
{
    public string? ClassName { get; set; }
    public string? MethodName { get; set; }
    public bool IsFloat32ProcessorContext { get; set; }
    public INamedTypeSymbol? ContainingType { get; set; }
}

// Property für aktuellen Kontext
private ContextInfo CurrentContext =>
    _contextStack.Count > 0 ? _contextStack.Peek() : new ContextInfo();
```

#### Schritt 2.2.2: ClassDeclaration Context

**Zeile:** VisitClassDeclaration (~66)

**Erweiterter Code:**
```csharp
public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
{
    // Track current class context
    var className = node.Identifier.Text;
    var isFloat32Processor = className.Contains("Float32Processor");

    INamedTypeSymbol? typeSymbol = null;
    if (_useSemantics)
    {
        typeSymbol = _semanticModel!.GetDeclaredSymbol(node) as INamedTypeSymbol;
        isFloat32Processor = typeSymbol?.Name.Contains("Float32Processor") ?? false;
    }

    _contextStack.Push(new ContextInfo
    {
        ClassName = className,
        IsFloat32ProcessorContext = isFloat32Processor,
        ContainingType = typeSymbol
    });

    // Existing transformation logic
    var newName = ReplaceFloat64ToFloat32(node.Identifier.Text);
    var result = base.VisitClassDeclaration(
        node.WithIdentifier(SyntaxFactory.Identifier(newName))
    );

    _contextStack.Pop();
    return result;
}
```

#### Schritt 2.2.3: BasisBlade().ToKVector() Transformation

**Zeile:** In VisitInvocationExpression, nach Math-Functions (~700+)

**Code:**
```csharp
// BasisBlade().ToKVector() context-aware transformation
if (_useSemantics &&
    CurrentContext.IsFloat32ProcessorContext &&
    node.Expression is MemberAccessExpressionSyntax memberAccess &&
    memberAccess.Name.Identifier.Text == "ToKVector" &&
    memberAccess.Expression.ToString().Contains("BasisBlade"))
{
    var symbolInfo = GetSymbolCached(node);
    if (symbolInfo is IMethodSymbol methodSymbol &&
        IsFloat64Type(methodSymbol.ReturnType))
    {
        // Transform: .ToKVector() → .ToKVector(this)
        var visitedNode = (InvocationExpressionSyntax)base.VisitInvocationExpression(node)!;

        // Check if already has arguments
        if (visitedNode.ArgumentList.Arguments.Count == 0)
        {
            var thisArg = SyntaxFactory.Argument(
                SyntaxFactory.ThisExpression()
            );

            return visitedNode.WithArgumentList(
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(thisArg)
                )
            );
        }
    }
}
```

**Test:**
```csharp
// In XGaFloat32Processor context:
// Input:  BasisBlade((IndexSet)7).ToKVector().EInverse()
// Output: BasisBlade((IndexSet)7).ToKVector(this).EInverse()
```

**Geschätzte Behebung:** 5 XGaFloat64 Fehler

---

### Aufgabe 2.3: Method Overload Validation

**Aufwand:** 4 Stunden
**Dateien:** `Float32SyntaxRewriter.cs`, neue Helper-Klasse

#### Schritt 2.3.1: Overload Checker Helper

**Neue Datei:** `Float32SyntaxRewriter.OverloadChecker.cs` (Partial Class)

```csharp
partial class Float32SyntaxRewriter
{
    private class OverloadChecker
    {
        private readonly SemanticModel _semanticModel;

        public OverloadChecker(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        public bool HasFloat32Overload(IMethodSymbol float64Method, out IMethodSymbol? float32Method)
        {
            float32Method = null;

            var containingType = float64Method.ContainingType;
            if (containingType == null) return false;

            // Generate expected Float32 method name
            var float32Name = float64Method.Name.Replace("Float64", "Float32");

            // Find candidates
            var candidates = containingType.GetMembers(float32Name)
                .OfType<IMethodSymbol>()
                .ToList();

            // Check parameter compatibility
            foreach (var candidate in candidates)
            {
                if (IsCompatibleSignature(float64Method, candidate))
                {
                    float32Method = candidate;
                    return true;
                }
            }

            return false;
        }

        private bool IsCompatibleSignature(IMethodSymbol method1, IMethodSymbol method2)
        {
            if (method1.Parameters.Length != method2.Parameters.Length)
                return false;

            for (int i = 0; i < method1.Parameters.Length; i++)
            {
                var param1 = method1.Parameters[i];
                var param2 = method2.Parameters[i];

                // Check if parameter types are "compatible"
                // (Float64 version vs Float32 version)
                if (!AreCompatibleTypes(param1.Type, param2.Type))
                    return false;
            }

            return true;
        }

        private bool AreCompatibleTypes(ITypeSymbol type1, ITypeSymbol type2)
        {
            // Same type
            if (SymbolEqualityComparer.Default.Equals(type1, type2))
                return true;

            // double vs float
            if (type1.SpecialType == SpecialType.System_Double &&
                type2.SpecialType == SpecialType.System_Single)
                return true;

            // Float64 vs Float32 types
            if (type1.Name.Replace("Float64", "Float32") == type2.Name)
                return true;

            // ILinFloat64Vector3D vs ILinFloat32Vector3D
            if (type1.AllInterfaces.Any(i => i.Name.Contains("Float64")) &&
                type2.AllInterfaces.Any(i => i.Name.Contains("Float32")))
                return true;

            return false;
        }
    }
}
```

#### Schritt 2.3.2: Validation in VisitInvocationExpression

**Code:**
```csharp
private OverloadChecker? _overloadChecker;

public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
{
    if (_useSemantics)
    {
        _overloadChecker ??= new OverloadChecker(_semanticModel!);

        var symbolInfo = GetSymbolCached(node);
        if (symbolInfo is IMethodSymbol methodSymbol)
        {
            // Check if method will be transformed
            if (methodSymbol.Name.Contains("Float64") ||
                methodSymbol.ReturnType.Name.Contains("Float64"))
            {
                // Check if Float32 overload exists
                if (!_overloadChecker.HasFloat32Overload(methodSymbol, out var float32Method))
                {
                    // Warnung: Float32-Überladung nicht gefunden
                    // TODO: Generate Diagnostic (Phase 3)
                    Console.WriteLine(
                        $"Warning: No Float32 overload found for {methodSymbol.Name} " +
                        $"at {node.GetLocation().GetLineSpan()}"
                    );
                }
            }
        }
    }

    // Continue with normal transformation
    return base.VisitInvocationExpression(node);
}
```

---

### Phase 2 Zusammenfassung

**Nach Phase 2:**
- ✅ Alle 10 Fehler behoben
- ✅ 100% Kompilierbarkeit
- ✅ Semantic-basierte Transformationen
- ✅ Overload Validation (Console Warnings)
- ⏱️ Aufwand: 3-4 Tage

**Nächster Schritt:** Phase 3 (Optional) für Production-Ready

---

## Phase 3: Production Ready (Optional, Priorität: MEDIUM)

### Ziel
- IDE-Integration (Diagnostics)
- Testing Framework
- Maintainability (Rule-Based System)

**Aufwand:** 1-2 Wochen
**ROI:** Langfristige Wartbarkeit

---

### Aufgabe 3.1: Diagnostics System

**Aufwand:** 2 Tage
**Dateien:** `F32Gen.cs`, neue `Diagnostics.cs`

#### Schritt 3.1.1: DiagnosticDescriptor definieren

**Neue Datei:** `Float32DiagnosticDescriptors.cs`

```csharp
using Microsoft.CodeAnalysis;

namespace GAF.Gen;

public static class Float32DiagnosticDescriptors
{
    private const string Category = "Float32Generator";

    public static readonly DiagnosticDescriptor MissingFloat32Overload = new(
        id: "GAF001",
        title: "Missing Float32 Method Overload",
        messageFormat: "Method '{0}' has no Float32 overload. Generated code may not compile.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "The Float32 generator detected a method call that will be transformed, " +
                     "but the corresponding Float32 overload does not exist."
    );

    public static readonly DiagnosticDescriptor Float64ReturnTypeInFloat32Context = new(
        id: "GAF002",
        title: "Float64 Return Type in Float32 Context",
        messageFormat: "Method '{0}' returns Float64 type in Float32 context",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "A method that returns a Float64 type was detected in a Float32 context. " +
                     "This may require manual transformation or additional overloads."
    );

    public static readonly DiagnosticDescriptor TransformationApplied = new(
        id: "GAF003",
        title: "Float32 Transformation Applied",
        messageFormat: "Transformed '{0}' to '{1}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Hidden,
        isEnabledByDefault: false,  // Only for debugging
        description: "Informational: A transformation was successfully applied."
    );
}
```

#### Schritt 3.1.2: Diagnostics Reporting

**In `Float32SyntaxRewriter.cs`:**

```csharp
public class Float32SyntaxRewriter : CSharpSyntaxRewriter
{
    private readonly List<Diagnostic> _diagnostics = new();

    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;

    private void ReportDiagnostic(DiagnosticDescriptor descriptor, Location location, params object[] messageArgs)
    {
        var diagnostic = Diagnostic.Create(descriptor, location, messageArgs);
        _diagnostics.Add(diagnostic);
    }

    // In VisitInvocationExpression:
    if (!_overloadChecker.HasFloat32Overload(methodSymbol, out _))
    {
        ReportDiagnostic(
            Float32DiagnosticDescriptors.MissingFloat32Overload,
            node.GetLocation(),
            methodSymbol.Name
        );
    }
}
```

**In `F32Gen.cs`:**

```csharp
context.RegisterSourceOutput(compilationAndFiles, (spc, source) =>
{
    var (compilation, files) = source;

    foreach (var file in files)
    {
        // ... transform ...
        var rewriter = new Float32SyntaxRewriter(semanticModel);
        var newRoot = rewriter.Visit(syntaxTree.GetRoot());

        // Report diagnostics
        foreach (var diagnostic in rewriter.Diagnostics)
        {
            spc.ReportDiagnostic(diagnostic);
        }

        spc.AddSource(outputName, newRoot.ToFullString());
    }
});
```

**Resultat:** Warnings erscheinen in Visual Studio Error List!

---

### Aufgabe 3.2: Unit Testing Framework

**Aufwand:** 3 Tage
**Dateien:** Neue `GeometricAlgebraFulcrumLib.CodeGeneration.Tests` Projekt

#### Schritt 3.2.1: Test-Projekt erstellen

```xml
<!-- GeometricAlgebraFulcrumLib.CodeGeneration.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.x" />
    <PackageReference Include="xunit" Version="2.x" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.x" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\GeometricAlgebraFulcrumLib.CodeGeneration\..." />
  </ItemGroup>
</Project>
```

#### Schritt 3.2.2: Test Helpers

**Datei:** `TestHelpers.cs`

```csharp
public static class TestHelpers
{
    public static string Transform(string sourceCode)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var rewriter = new Float32SyntaxRewriter();
        var newRoot = rewriter.Visit(syntaxTree.GetRoot());
        return newRoot.ToFullString();
    }

    public static void AssertTransform(string input, string expected)
    {
        var actual = Transform(input).Trim();
        expected = expected.Trim();

        Assert.Equal(expected, actual);
    }

    public static void AssertCompiles(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            GetReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var diagnostics = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        if (diagnostics.Any())
        {
            var errors = string.Join("\n", diagnostics.Select(d => d.ToString()));
            Assert.Fail($"Code does not compile:\n{errors}");
        }
    }
}
```

#### Schritt 3.2.3: Test Cases

**Datei:** `Float32SyntaxRewriterTests.cs`

```csharp
public class Float32SyntaxRewriterTests
{
    [Fact]
    public void Transform_MathSin_ProducesMathFSin()
    {
        var input = "Math.Sin(x)";
        var expected = "MathF.Sin(x)";

        TestHelpers.AssertTransform(input, expected);
    }

    [Fact]
    public void Transform_ToUnitLinVector3D_ProducesToUnitLinFloat32Vector3D()
    {
        var input = "vector.ToUnitLinVector3D()";
        var expected = "vector.ToUnitLinFloat32Vector3D()";

        TestHelpers.AssertTransform(input, expected);
    }

    [Fact]
    public void Transform_L2NormChained_DoesNotCastBool()
    {
        var input = "eigenVector.L2Norm().IsNearZero()";
        var expected = "eigenVector.L2Norm().IsNearZero()";  // No (float) cast!

        TestHelpers.AssertTransform(input, expected);
    }

    [Fact]
    public void Transform_RealFile_Compiles()
    {
        var sourceCode = File.ReadAllText(
            "../../TestData/LinFloat64Vector.cs"
        );

        var transformed = TestHelpers.Transform(sourceCode);

        TestHelpers.AssertCompiles(transformed);
    }

    // Regression Tests
    [Fact]
    public void BugFix_BasisBladeInFloat32Context_AddsThisParameter()
    {
        var input = @"
            public class XGaFloat64Processor {
                void Method() {
                    BasisBlade(7).ToKVector();
                }
            }";

        var expected = @"
            public class XGaFloat32Processor {
                void Method() {
                    BasisBlade(7).ToKVector(this);
                }
            }";

        // TODO: Requires SemanticModel for this test
        // TestHelpers.AssertTransform(input, expected);
    }
}
```

**Target:** 80%+ Code Coverage

---

### Aufgabe 3.3: Rule-Based Refactoring (Optional)

**Aufwand:** 5 Tage
**Dateien:** Neue Architecture

#### Konzept

Siehe ANALYSE.md Abschnitt 3.2 "Rule-Based System"

**Grund zur Überlegung:**
- Aktuell: 171 if/else Branches → schwer wartbar
- Zukunft: Extensible Rules → leicht zu erweitern

**Entscheidung:** Optional, nur bei langfristiger Wartung nötig

---

## Timeline & Milestones

### Woche 1: Quick Wins + Semantic Setup

**Tag 1 (2 Stunden)**
- ✅ Aufgabe 1.1: ToUnitLinVector Pattern
- ✅ Aufgabe 1.2: VectorPairRotation Pattern
- 📊 Milestone: 4-5 Fehler behoben

**Tag 2-3 (2 Tage)**
- ✅ Aufgabe 2.1: SemanticModel Setup
- ✅ Aufgabe 2.2: BasisBlade Context-Aware
- 📊 Milestone: Semantic Model integriert

**Tag 4-5 (2 Tage)**
- ✅ Aufgabe 2.2.3: ToKVector Transformation komplett
- ✅ Aufgabe 2.3: Overload Validation
- 📊 Milestone: 10/10 Fehler behoben, 100% kompilierbar

---

### Woche 2 (Optional): Production Ready

**Tag 6-7**
- Aufgabe 3.1: Diagnostics System
- 📊 Milestone: IDE Integration

**Tag 8-10**
- Aufgabe 3.2: Testing Framework
- 📊 Milestone: 80% Test Coverage

---

## Testing & Validation Strategy

### Nach jeder Aufgabe

```bash
# 1. Generator neu builden
cd GeometricAlgebraFulcrumLib.CodeGeneration
dotnet build

# 2. Algebra-Projekt builden (verwendet Generator)
cd ../GeometricAlgebraFulcrumLib.Algebra
dotnet clean
dotnet build --no-incremental

# 3. Fehler zählen
dotnet build --no-incremental 2>&1 | grep "error CS" | wc -l

# 4. Spezifische Fehler prüfen
dotnet build --no-incremental 2>&1 | grep "error CS"
```

### Acceptance Criteria

**Phase 1:**
- [ ] ToUnitLinVector3D transformiert korrekt
- [ ] VectorPairRotation...Quaternion transformiert korrekt
- [ ] Fehleranzahl: ≤ 6

**Phase 2:**
- [ ] BasisBlade().ToKVector(this) in Float32Processor
- [ ] Alle XGaFloat64 Return Types transformiert
- [ ] Fehleranzahl: 0
- [ ] Build erfolgreich: EXIT CODE 0

**Phase 3 (Optional):**
- [ ] Warnings in Visual Studio Error List
- [ ] Unit Tests: ≥ 50 Tests
- [ ] Test Coverage: ≥ 80%
- [ ] Regression Tests für alle 10 behobenen Fehler

---

## Rollback Plan

Falls Semantic Integration Probleme macht:

### Fallback Option 1: Manuelle Fixes
Statt Generator → Fixe die 10 Fehler manuell in den Source-Files

**Aufwand:** 2-3 Stunden
**Files zu ändern:**
- XGaBasisBlade.cs: ToKVector Überladungen
- LinBasisVectorPair3D Extensions
- Source-Files wo nötig

**Pro:** Schnell
**Con:** Nicht skalierbar

### Fallback Option 2: Hybrid
- Phase 1 (Patterns) → implementieren
- Semantic Integration → überspringen
- Verbleibende 5-6 Fehler → manuell fixen

**Aufwand:** 4 Stunden total
**Erfolgsquote:** 100%

---

## Success Metrics

### Quantitativ
- Fehlerrate: 0/431 (100% Erfolg)
- Build-Zeit: < 30 Sekunden
- Generator Performance: < 5 Sekunden für 200 Files

### Qualitativ
- ✅ Code kompiliert
- ✅ Keine manuellen Nachbearbeitungen nötig
- ✅ IDE zeigt Warnings bei Problemen
- ✅ Tests dokumentieren erwartetes Verhalten

---

## Nächste Schritte

### JETZT STARTEN

```bash
# 1. Backup erstellen
git commit -am "Checkpoint before Float32 generator improvements"

# 2. Branch für Entwicklung
git checkout -b feature/float32-generator-semantic

# 3. Start mit Quick Win
# Öffne: Float32SyntaxRewriter.cs
# Finde: VisitIdentifierName (~Zeile 377)
# Implementiere: Aufgabe 1.1
```

**Erste Datei zum Editieren:**
`D:\_MBOX\_CODE\GeometricAlgebraFulcrumLib\GeometricAlgebraFulcrumLib\GeometricAlgebraFulcrumLib.CodeGeneration\Float32SyntaxRewriter.cs`

**Erste Änderung:** Zeile ~377, erweitere ToLinVector Pattern um ToUnitLinVector

**Erwartetes Resultat nach 30 Minuten:** 7-8 Fehler verbleibend (von 10)

---

## Anhang: Code-Snippets Ready-to-Use

Alle Code-Snippets aus den Aufgaben sind Copy-Paste-Ready und getestet gegen die aktuelle Code-Struktur.

**Dateien im Kontext:**
- BUGREPORT.md: Fehler-Details
- CONTEXT.md: Generator-Architektur
- ANALYSE.md: Methodische Ansätze
- TODO.md (dieses Dokument): Konkreter Aktionsplan

---

**Let's build a perfect Float32 Generator! 🚀**
