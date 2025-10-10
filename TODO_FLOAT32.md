# TODO: Float32 Code Generator (Roslyn Source Generator)

**Projekt:** Vollständige Float32-Unterstützung für GeometricAlgebraFulcrumLib
**Typ:** Roslyn Incremental Source Generator
**Ziel:** Auto-Generierung aller Float32-Module aus Float64 beim Build
**Scope:** 1108 Dateien in 28 Modulen
**Status:** Planning Complete, Ready for Implementation

---

## 📊 Executive Summary

### Was wird generiert?

| Modul | Files | Priority | Dependencies |
|-------|-------|----------|--------------|
| **Scalars.Float32** | 10 | 🔴 P0 | None |
| **LinearAlgebra.Float32** | 203 | 🔴 P0 | Scalars |
| **GeometricAlgebra.Float32** | 129 | 🔴 P0 | Scalars, LinearAlgebra |
| **CGa.Float32** | 83 | 🟠 P1 | GA, LA |
| **PGa.Float32** | 7 | 🟠 P1 | GA, LA |
| **VGa.Float32** | 4 | 🟠 P1 | GA, LA |
| **Trajectories.Float32** | ~30 | 🟡 P2 | GA, LA |
| **BasicShapes.Float32** | ~50 | 🟡 P2 | Geometry |
| **Calculus.Float32** | 23 | 🟡 P2 | GA |
| **Others** | ~569 | ⚪ P3 | Various |
| **Total** | **1108** | | |

**Key Insight:** Die ersten 3 Module (P0) sind **kritisch** für Arc-Spline Projekt!

---

## 🎯 Project Goals

### Primary Goals
1. ✅ **CGa.Float32** funktionsfähig für Arc-Spline Project
2. ✅ **Roslyn Source Generator** als Build-Time Code-Gen
3. ✅ **Wartbarkeit** - Auto-Regen bei Float64 Updates
4. ✅ **Testbarkeit** - Generierter Code muss kompilieren + Tests laufen

### Secondary Goals
5. ✅ **Vollständige Float32-Unterstützung** für gesamte GA-FuL
6. ✅ **Upstream-fähig** - PR zu GA-FuL Repository
7. ✅ **Performance** - Sub-second generation time

---

## 🏗️ Architecture

### Generator Structure

```
GeometricAlgebraFulcrumLib.CodeGeneration/
├── Float32SourceGenerator.cs           ← Main Incremental Generator
├── Float32SyntaxRewriter.cs            ← Roslyn Rewriter (Pattern-Ersetzung)
├── Float32GeneratorOptions.cs          ← Configuration
├── ModuleDependencyGraph.cs            ← Dependency-Auflösung
├── GeneratedCodeCache.cs               ← Incremental Cache
└── Diagnostics/
    ├── Float32Diagnostics.cs           ← Error/Warning Codes
    └── Float32DiagnosticDescriptors.cs

GeometricAlgebraFulcrumLib.Algebra/
├── Scalars/Float32/                    ← Generated at build time
├── LinearAlgebra/Float32/              ← Generated at build time
└── GeometricAlgebra/Float32/           ← Generated at build time

GeometricAlgebraFulcrumLib.Modeling/
└── Geometry/
    ├── CGa/Float32/                    ← Generated at build time
    ├── PGa/Float32/                    ← Generated at build time
    └── VGa/Float32/                    ← Generated at build time
```

### Generator Pipeline

```
Build Trigger
    ↓
Roslyn Incremental Generator
    ↓
Read All Float64 Source Files (via AdditionalFiles)
    ↓
Parse to Syntax Trees (Roslyn CSharpSyntaxTree)
    ↓
Apply Float32SyntaxRewriter (Pattern Substitution)
    ↓
Generate Float32 Sources (AddSource)
    ↓
Compilation (msbuild compiles generated code)
```

---

## 📋 Implementation Tasks

### Phase 0: Setup & Infrastructure (2-3 hours)

**Task 0.1: Create Generator Project**
```bash
cd GeometricAlgebraFulcrumLib
dotnet new classlib -n GeometricAlgebraFulcrumLib.CodeGeneration
cd GeometricAlgebraFulcrumLib.CodeGeneration
dotnet add package Microsoft.CodeAnalysis.CSharp --version 4.8.0
dotnet add package Microsoft.CodeAnalysis.Analyzers --version 3.3.4
```

**Task 0.2: Configure .csproj**

File: `GeometricAlgebraFulcrumLib.CodeGeneration/GeometricAlgebraFulcrumLib.CodeGeneration.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <IsRoslynComponent>true</IsRoslynComponent>
    <EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.8.0" PrivateAssets="all" />
    <PackageReference Include="Microsoft.CodeAnalysis.Analyzers" Version="3.3.4" PrivateAssets="all" />
  </ItemGroup>

  <!-- Hinweis: netstandard2.0 ist required für Source Generators -->
</Project>
```

**Task 0.3: Configure Consumer Projects**

Files: `GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebraFulcrumLib.Algebra.csproj`
       `GeometricAlgebraFulcrumLib.Modeling/GeometricAlgebraFulcrumLib.Modeling.csproj`

```xml
<ItemGroup>
  <!-- Reference Source Generator -->
  <ProjectReference Include="..\GeometricAlgebraFulcrumLib.CodeGeneration\GeometricAlgebraFulcrumLib.CodeGeneration.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />

  <!-- Mark Float64 sources as AdditionalFiles for generator -->
  <AdditionalFiles Include="Scalars\Float64\**\*.cs" />
  <AdditionalFiles Include="LinearAlgebra\Float64\**\*.cs" />
  <AdditionalFiles Include="GeometricAlgebra\Float64\**\*.cs" />
</ItemGroup>

<!-- Configure Generator -->
<ItemGroup>
  <CompilerVisibleProperty Include="Float32GeneratorEnabled" />
  <CompilerVisibleProperty Include="Float32GeneratorOutputPath" />
</ItemGroup>

<PropertyGroup>
  <Float32GeneratorEnabled>true</Float32GeneratorEnabled>
  <Float32GeneratorOutputPath>$(MSBuildProjectDirectory)</Float32GeneratorOutputPath>

  <!-- Include generated files in compilation -->
  <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
  <CompilerGeneratedFilesOutputPath>$(BaseIntermediateOutputPath)Generated</CompilerGeneratedFilesOutputPath>
</PropertyGroup>

<!-- Add generated files to project (for IDE support) -->
<ItemGroup>
  <Compile Include="$(CompilerGeneratedFilesOutputPath)\**\*.cs" />
</ItemGroup>
```

**✅ Verification:**
```bash
dotnet build GeometricAlgebraFulcrumLib.CodeGeneration
# Should succeed without errors
```

---

### Phase 1: Core Roslyn Rewriter (4-6 hours)

**Task 1.1: Implement Float32SyntaxRewriter**

File: `GeometricAlgebraFulcrumLib.CodeGeneration/Float32SyntaxRewriter.cs`

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GeometricAlgebraFulcrumLib.CodeGeneration;

/// <summary>
/// Roslyn Syntax Rewriter that transforms Float64 code to Float32
/// </summary>
public class Float32SyntaxRewriter : CSharpSyntaxRewriter
{
    private readonly SemanticModel _semanticModel;

    public Float32SyntaxRewriter(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
    }

    // ============================================================
    // 1. NAMESPACE: Float64 → Float32
    // ============================================================

    public override SyntaxNode? VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
    {
        var newName = ReplaceFloat64ToFloat32(node.Name.ToString());
        var newNameSyntax = SyntaxFactory.ParseName(newName);

        return base.VisitNamespaceDeclaration(
            node.WithName(newNameSyntax)
        );
    }

    public override SyntaxNode? VisitFileScopedNamespaceDeclaration(
        FileScopedNamespaceDeclarationSyntax node)
    {
        var newName = ReplaceFloat64ToFloat32(node.Name.ToString());
        var newNameSyntax = SyntaxFactory.ParseName(newName);

        return base.VisitFileScopedNamespaceDeclaration(
            node.WithName(newNameSyntax)
        );
    }

    // ============================================================
    // 2. CLASS/STRUCT/INTERFACE NAMES: *Float64* → *Float32*
    // ============================================================

    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var newName = ReplaceFloat64ToFloat32(node.Identifier.Text);
        return base.VisitClassDeclaration(
            node.WithIdentifier(SyntaxFactory.Identifier(newName))
        );
    }

    public override SyntaxNode? VisitStructDeclaration(StructDeclarationSyntax node)
    {
        var newName = ReplaceFloat64ToFloat32(node.Identifier.Text);
        return base.VisitStructDeclaration(
            node.WithIdentifier(SyntaxFactory.Identifier(newName))
        );
    }

    public override SyntaxNode? VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        var newName = ReplaceFloat64ToFloat32(node.Identifier.Text);
        return base.VisitInterfaceDeclaration(
            node.WithIdentifier(SyntaxFactory.Identifier(newName))
        );
    }

    public override SyntaxNode? VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        var newName = ReplaceFloat64ToFloat32(node.Identifier.Text);
        return base.VisitRecordDeclaration(
            node.WithIdentifier(SyntaxFactory.Identifier(newName))
        );
    }

    // ============================================================
    // 3. TYPE REFERENCES: double → float, Float64 → Float32
    // ============================================================

    public override SyntaxNode? VisitPredefinedType(PredefinedTypeSyntax node)
    {
        // double → float
        if (node.Keyword.IsKind(SyntaxKind.DoubleKeyword))
        {
            return node.WithKeyword(
                SyntaxFactory.Token(SyntaxKind.FloatKeyword)
            );
        }

        return base.VisitPredefinedType(node);
    }

    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        var text = node.Identifier.Text;

        // Float64 → Float32 (in type names)
        if (text.Contains("Float64"))
        {
            var newText = ReplaceFloat64ToFloat32(text);
            return node.WithIdentifier(SyntaxFactory.Identifier(newText));
        }

        // Math → MathF (for math functions)
        if (text == "Math")
        {
            return node.WithIdentifier(SyntaxFactory.Identifier("MathF"));
        }

        return base.VisitIdentifierName(node);
    }

    public override SyntaxNode? VisitGenericName(GenericNameSyntax node)
    {
        var text = node.Identifier.Text;

        // Generic types with Float64 in name
        if (text.Contains("Float64"))
        {
            var newText = ReplaceFloat64ToFloat32(text);
            return base.VisitGenericName(
                node.WithIdentifier(SyntaxFactory.Identifier(newText))
            );
        }

        return base.VisitGenericName(node);
    }

    // ============================================================
    // 4. USING DIRECTIVES: Float64 → Float32 in imports
    // ============================================================

    public override SyntaxNode? VisitUsingDirective(UsingDirectiveSyntax node)
    {
        var nameText = node.Name?.ToString();
        if (nameText != null && nameText.Contains("Float64"))
        {
            var newName = ReplaceFloat64ToFloat32(nameText);
            var newNameSyntax = SyntaxFactory.ParseName(newName);

            return node.WithName(newNameSyntax);
        }

        return base.VisitUsingDirective(node);
    }

    // ============================================================
    // 5. NUMERIC LITERALS: Add 'f' suffix if missing
    // ============================================================

    public override SyntaxNode? VisitLiteralExpression(LiteralExpressionSyntax node)
    {
        // Check if it's a numeric literal that should be float
        if (node.IsKind(SyntaxKind.NumericLiteralExpression))
        {
            var text = node.Token.Text;

            // If it has decimal point but no 'f' suffix, add it
            if (text.Contains(".") && !text.EndsWith("f") && !text.EndsWith("F") &&
                !text.EndsWith("d") && !text.EndsWith("D") && !text.EndsWith("m"))
            {
                var newText = text + "f";
                var newToken = SyntaxFactory.Literal(newText, float.Parse(text));
                return node.WithToken(newToken);
            }

            // If it has 'd' or 'D' suffix, replace with 'f'
            if (text.EndsWith("d") || text.EndsWith("D"))
            {
                var newText = text.Substring(0, text.Length - 1) + "f";
                var value = float.Parse(text.TrimEnd('d', 'D'));
                var newToken = SyntaxFactory.Literal(newText, value);
                return node.WithToken(newToken);
            }
        }

        return base.VisitLiteralExpression(node);
    }

    // ============================================================
    // 6. MEMBER ACCESS: Math.Sin → MathF.Sin
    // ============================================================

    public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        // Math.XXX → MathF.XXX
        if (node.Expression is IdentifierNameSyntax identifier &&
            identifier.Identifier.Text == "Math")
        {
            var newExpression = SyntaxFactory.IdentifierName("MathF");
            return base.VisitMemberAccessExpression(
                node.WithExpression(newExpression)
            );
        }

        return base.VisitMemberAccessExpression(node);
    }

    // ============================================================
    // HELPER METHODS
    // ============================================================

    private static string ReplaceFloat64ToFloat32(string text)
    {
        return text.Replace("Float64", "Float32")
                   .Replace("float64", "float32")
                   .Replace("FLOAT64", "FLOAT32");
    }
}
```

**✅ Verification:**
Write unit test for each Visit* method with example syntax trees.

---

### Phase 2: Incremental Source Generator (6-8 hours)

**Task 2.1: Implement Main Generator**

File: `GeometricAlgebraFulcrumLib.CodeGeneration/Float32SourceGenerator.cs`

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace GeometricAlgebraFulcrumLib.CodeGeneration;

[Generator]
public class Float32SourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register post-initialization output (runs once)
        context.RegisterPostInitializationOutput(ctx =>
        {
            // Optional: Add global attributes or marker files
        });

        // Define incremental pipeline
        var float64Files = context.AdditionalFilesProvider
            .Where(file => file.Path.Contains("Float64") && file.Path.EndsWith(".cs"))
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Combine(context.CompilationProvider);

        // Register source output
        context.RegisterSourceOutput(float64Files, (ctx, source) =>
        {
            var (fileAndOptions, compilation) = source;
            var (file, options) = fileAndOptions;

            GenerateFloat32Source(ctx, file, options, compilation);
        });
    }

    private static void GenerateFloat32Source(
        SourceProductionContext context,
        AdditionalText float64File,
        AnalyzerConfigOptionsProvider options,
        Compilation compilation)
    {
        try
        {
            // Read source text
            var sourceText = float64File.GetText(context.CancellationToken);
            if (sourceText == null)
                return;

            // Parse to syntax tree
            var syntaxTree = CSharpSyntaxTree.ParseText(
                sourceText,
                cancellationToken: context.CancellationToken
            );

            // Get semantic model
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            // Apply rewriter
            var rewriter = new Float32SyntaxRewriter(semanticModel);
            var newRoot = rewriter.Visit(syntaxTree.GetRoot(context.CancellationToken));

            // Generate output
            var float32Code = newRoot?.ToFullString();
            if (string.IsNullOrEmpty(float32Code))
                return;

            // Generate file name
            var fileName = GetFloat32FileName(float64File.Path);
            var hintName = $"Float32.{fileName}";

            // Add source to compilation
            context.AddSource(
                hintName,
                SourceText.From(float32Code, Encoding.UTF8)
            );
        }
        catch (Exception ex)
        {
            // Report diagnostic
            var descriptor = new DiagnosticDescriptor(
                id: "FLOAT32GEN001",
                title: "Float32 Generation Error",
                messageFormat: "Error generating Float32 from {0}: {1}",
                category: "Float32Generator",
                DiagnosticSeverity.Error,
                isEnabledByDefault: true
            );

            context.ReportDiagnostic(Diagnostic.Create(
                descriptor,
                Location.None,
                float64File.Path,
                ex.Message
            ));
        }
    }

    private static string GetFloat32FileName(string float64Path)
    {
        // Extract file name and convert path
        // e.g., "Scalars/Float64/Float64Scalar.cs"
        //    → "Scalars_Float32_Float32Scalar.cs"

        var fileName = Path.GetFileNameWithoutExtension(float64Path)
            .Replace("Float64", "Float32");

        var relativePath = float64Path
            .Replace("Float64", "Float32")
            .Replace(Path.DirectorySeparatorChar, '_')
            .Replace(Path.AltDirectorySeparatorChar, '_');

        return $"{relativePath}_{fileName}.g.cs";
    }
}
```

**✅ Verification:**
```bash
dotnet build GeometricAlgebraFulcrumLib.Algebra
# Check: obj/Debug/netX.X/generated/ should contain Float32 files
```

---

### Phase 3: Module-Specific Configuration (2-3 hours)

**Task 3.1: Dependency-Aware Generation**

File: `GeometricAlgebraFulcrumLib.CodeGeneration/ModuleDependencyGraph.cs`

```csharp
namespace GeometricAlgebraFulcrumLib.CodeGeneration;

public class ModuleDependencyGraph
{
    // Define generation order based on dependencies
    public static readonly List<ModuleInfo> GenerationOrder = new()
    {
        // Layer 0: No dependencies
        new ModuleInfo("Scalars", 0, Array.Empty<string>()),

        // Layer 1: Depends on Scalars
        new ModuleInfo("LinearAlgebra", 1, new[] { "Scalars" }),
        new ModuleInfo("Polynomials", 1, new[] { "Scalars" }),

        // Layer 2: Depends on LinearAlgebra
        new ModuleInfo("GeometricAlgebra", 2, new[] { "Scalars", "LinearAlgebra" }),

        // Layer 3: Depends on GA + LA
        new ModuleInfo("CGa", 3, new[] { "GeometricAlgebra", "LinearAlgebra" }),
        new ModuleInfo("PGa", 3, new[] { "GeometricAlgebra", "LinearAlgebra" }),
        new ModuleInfo("VGa", 3, new[] { "GeometricAlgebra", "LinearAlgebra" }),
        new ModuleInfo("BasicShapes", 3, new[] { "GeometricAlgebra", "LinearAlgebra" }),

        // Layer 4: High-level
        new ModuleInfo("Trajectories", 4, new[] { "CGa", "VGa" }),
        new ModuleInfo("Calculus", 4, new[] { "GeometricAlgebra" }),
    };
}

public record ModuleInfo(string Name, int Layer, string[] Dependencies);
```

**Task 3.2: Configure Each Project**

For each project that needs Float32 generation, add to `.csproj`:

```xml
<!-- GeometricAlgebraFulcrumLib.Algebra.csproj -->
<ItemGroup>
  <CompilerVisibleProperty Include="Float32_Module" />
  <CompilerVisibleProperty Include="Float32_Layer" />
</ItemGroup>

<PropertyGroup>
  <Float32_Module>Algebra</Float32_Module>
  <Float32_Layer>0-2</Float32_Layer>
</PropertyGroup>
```

```xml
<!-- GeometricAlgebraFulcrumLib.Modeling.csproj -->
<ItemGroup>
  <CompilerVisibleProperty Include="Float32_Module" />
  <CompilerVisibleProperty Include="Float32_Layer" />
</ItemGroup>

<PropertyGroup>
  <Float32_Module>Modeling</Float32_Module>
  <Float32_Layer>3-4</Float32_Layer>
</PropertyGroup>
```

---

### Phase 4: Testing & Validation (4-6 hours)

**Task 4.1: Unit Tests for Rewriter**

File: `GeometricAlgebraFulcrumLib.CodeGeneration.Tests/Float32SyntaxRewriterTests.cs`

```csharp
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace GeometricAlgebraFulcrumLib.CodeGeneration.Tests;

public class Float32SyntaxRewriterTests
{
    [Fact]
    public void RewriteClassName_Float64ToFloat32()
    {
        var source = @"
            public class CGaFloat64GeometricSpace { }
        ";

        var expected = @"
            public class CGaFloat32GeometricSpace { }
        ";

        var result = ApplyRewriter(source);

        Assert.Equal(NormalizeWhitespace(expected), NormalizeWhitespace(result));
    }

    [Fact]
    public void RewriteTypeReference_DoubleToFloat()
    {
        var source = @"
            public double Radius { get; set; }
        ";

        var expected = @"
            public float Radius { get; set; }
        ";

        var result = ApplyRewriter(source);

        Assert.Equal(NormalizeWhitespace(expected), NormalizeWhitespace(result));
    }

    [Fact]
    public void RewriteMathCalls_MathToMathF()
    {
        var source = @"
            var x = Math.Sin(angle);
            var y = Math.Sqrt(value);
        ";

        var expected = @"
            var x = MathF.Sin(angle);
            var y = MathF.Sqrt(value);
        ";

        var result = ApplyRewriter(source);

        Assert.Equal(NormalizeWhitespace(expected), NormalizeWhitespace(result));
    }

    [Fact]
    public void RewriteNamespace_Float64ToFloat32()
    {
        var source = @"
            namespace GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64;
        ";

        var expected = @"
            namespace GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32;
        ";

        var result = ApplyRewriter(source);

        Assert.Contains("Float32", result);
        Assert.DoesNotContain("Float64", result);
    }

    [Fact]
    public void RewriteUsingDirective_Float64ToFloat32()
    {
        var source = @"
            using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors;
        ";

        var expected = @"
            using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32.Vectors;
        ";

        var result = ApplyRewriter(source);

        Assert.Contains("Float32", result);
    }

    [Fact]
    public void RewriteLiteral_AddFloatSuffix()
    {
        var source = @"
            var x = 3.14;
            var y = 2.5d;
        ";

        var expected = @"
            var x = 3.14f;
            var y = 2.5f;
        ";

        var result = ApplyRewriter(source);

        Assert.Contains("3.14f", result);
        Assert.Contains("2.5f", result);
    }

    // Helper methods
    private static string ApplyRewriter(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(
            "TestCompilation",
            new[] { tree }
        );
        var semanticModel = compilation.GetSemanticModel(tree);

        var rewriter = new Float32SyntaxRewriter(semanticModel);
        var newRoot = rewriter.Visit(tree.GetRoot());

        return newRoot.ToFullString();
    }

    private static string NormalizeWhitespace(string code)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        return tree.GetRoot().NormalizeWhitespace().ToFullString();
    }
}
```

**Task 4.2: Integration Tests**

File: `GeometricAlgebraFulcrumLib.CodeGeneration.Tests/GeneratorIntegrationTests.cs`

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace GeometricAlgebraFulcrumLib.CodeGeneration.Tests;

public class GeneratorIntegrationTests
{
    [Fact]
    public void Generator_ProducesValidFloat32Code()
    {
        // Arrange
        var float64Source = @"
            using System;
            using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;

            namespace GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float64.Vectors.Space3D;

            public sealed record LinFloat64Vector3D
            {
                public double X { get; init; }
                public double Y { get; init; }
                public double Z { get; init; }

                public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        ";

        var additionalText = new TestAdditionalText("LinFloat64Vector3D.cs", float64Source);
        var compilation = CreateCompilation();

        // Act
        var generator = new Float32SourceGenerator();
        var driver = CSharpGeneratorDriver.Create(generator)
            .AddAdditionalTexts(ImmutableArray.Create<AdditionalText>(additionalText));

        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out var diagnostics
        );

        // Assert
        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));

        var generatedTrees = outputCompilation.SyntaxTrees.Skip(1); // Skip original
        Assert.NotEmpty(generatedTrees);

        var generatedCode = generatedTrees.First().ToString();

        // Verify transformations
        Assert.Contains("namespace GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32", generatedCode);
        Assert.Contains("public sealed record LinFloat32Vector3D", generatedCode);
        Assert.Contains("public float X", generatedCode);
        Assert.Contains("MathF.Sqrt", generatedCode);
        Assert.DoesNotContain("double", generatedCode);
        Assert.DoesNotContain("Float64", generatedCode);
    }

    [Fact]
    public void Generator_CompilesWithoutErrors()
    {
        // Same setup as above
        // ...

        // Verify compilation
        var errors = outputCompilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error);

        Assert.Empty(errors);
    }

    private static Compilation CreateCompilation()
    {
        return CSharpCompilation.Create(
            "TestCompilation",
            references: new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            }
        );
    }
}

// Helper class
internal class TestAdditionalText : AdditionalText
{
    private readonly SourceText _text;

    public TestAdditionalText(string path, string text)
    {
        Path = path;
        _text = SourceText.From(text, Encoding.UTF8);
    }

    public override string Path { get; }

    public override SourceText? GetText(CancellationToken cancellationToken = default)
        => _text;
}
```

**✅ Verification:**
```bash
dotnet test GeometricAlgebraFulcrumLib.CodeGeneration.Tests
# All tests should pass
```

---

### Phase 5: Full Build Integration (2-3 hours)

**Task 5.1: Configure Multi-Project Build**

Root `.sln` or `Directory.Build.props`:

```xml
<!-- Directory.Build.props at repository root -->
<Project>
  <PropertyGroup>
    <!-- Enable Float32 Generation globally -->
    <Float32GeneratorEnabled>true</Float32GeneratorEnabled>

    <!-- Output path for generated files -->
    <Float32GeneratorOutputPath>$(MSBuildProjectDirectory)</Float32GeneratorOutputPath>

    <!-- Emit generated files to disk (for debugging) -->
    <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
    <CompilerGeneratedFilesOutputPath>$(BaseIntermediateOutputPath)Generated</CompilerGeneratedFilesOutputPath>
  </PropertyGroup>
</Project>
```

**Task 5.2: Build Order Script**

File: `build-float32.ps1` (PowerShell) or `build-float32.sh` (Bash)

```powershell
#!/usr/bin/env pwsh

# Build Float32 modules in dependency order

Write-Host "Building Float32 Code Generator..." -ForegroundColor Cyan
dotnet build GeometricAlgebraFulcrumLib.CodeGeneration/GeometricAlgebraFulcrumLib.CodeGeneration.csproj

Write-Host "`nGenerating Float32 modules in dependency order..." -ForegroundColor Cyan

# Layer 0: Scalars
Write-Host "Layer 0: Scalars..." -ForegroundColor Yellow
dotnet build GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebraFulcrumLib.Algebra.csproj

# Layer 1-2: LinearAlgebra, GeometricAlgebra
Write-Host "Layer 1-2: LinearAlgebra, GeometricAlgebra..." -ForegroundColor Yellow
dotnet build GeometricAlgebraFulcrumLib.Algebra/GeometricAlgebraFulcrumLib.Algebra.csproj

# Layer 3: CGa, PGa, VGa
Write-Host "Layer 3: CGa, PGa, VGa..." -ForegroundColor Yellow
dotnet build GeometricAlgebraFulcrumLib.Modeling/GeometricAlgebraFulcrumLib.Modeling.csproj

# Layer 4: Trajectories, Calculus, etc.
Write-Host "Layer 4: High-level modules..." -ForegroundColor Yellow
dotnet build GeometricAlgebraFulcrumLib.Modeling/GeometricAlgebraFulcrumLib.Modeling.csproj

Write-Host "`n✅ Float32 generation complete!" -ForegroundColor Green
Write-Host "Generated files: obj/Debug/net8.0/generated/" -ForegroundColor Gray
```

**✅ Verification:**
```bash
./build-float32.ps1
# Should complete without errors
# Check: obj/Debug/net8.0/generated/ should contain ~1108 Float32 files
```

---

### Phase 6: Validation & Testing (4-6 hours)

**Task 6.1: Compile-Time Verification**

```bash
# Clean build to force regeneration
dotnet clean
dotnet build

# Verify no compilation errors
dotnet build --no-incremental

# Check generated file count
find obj/Debug -name "*Float32*.g.cs" | wc -l
# Should be ~1108
```

**Task 6.2: Functional Tests**

File: `GeometricAlgebraFulcrumLib.Tests/Float32FunctionalTests.cs`

```csharp
using GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32.Vectors.Space3D;
using GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float32;
using Xunit;

namespace GeometricAlgebraFulcrumLib.Tests;

public class Float32FunctionalTests
{
    [Fact]
    public void Float32_LinearAlgebra_BasicOperations()
    {
        var v1 = LinFloat32Vector3D.Create(1.0f, 2.0f, 3.0f);
        var v2 = LinFloat32Vector3D.Create(4.0f, 5.0f, 6.0f);

        var sum = v1 + v2;

        Assert.Equal(5.0f, sum.X, precision: 4);
        Assert.Equal(7.0f, sum.Y, precision: 4);
        Assert.Equal(9.0f, sum.Z, precision: 4);
    }

    [Fact]
    public void Float32_CGa_CircleFromThreePoints()
    {
        var cga = CGaFloat32GeometricSpace5D.Instance;

        var p1 = LinFloat32Vector3D.Create(1.0f, 0.0f, 0.0f);
        var p2 = LinFloat32Vector3D.Create(0.0f, 1.0f, 0.0f);
        var p3 = LinFloat32Vector3D.Create(-1.0f, 0.0f, 0.0f);

        var circle = cga.DefineRealRoundCircleFromPoints(p1, p2, p3);

        var radius = circle.RealRadius;

        Assert.InRange(radius, 0.99f, 1.01f);
    }

    [Fact]
    public void Float32_Precision_CompareWithFloat64()
    {
        // Compare Float32 vs Float64 results
        var cgaFloat32 = CGaFloat32GeometricSpace5D.Instance;
        var cgaFloat64 = CGaFloat64GeometricSpace5D.Instance;

        var p1_32 = LinFloat32Vector3D.Create(1.0f, 0.0f, 0.0f);
        var p2_32 = LinFloat32Vector3D.Create(0.0f, 1.0f, 0.0f);
        var p3_32 = LinFloat32Vector3D.Create(-1.0f, 0.0f, 0.0f);

        var p1_64 = LinFloat64Vector3D.Create(1.0, 0.0, 0.0);
        var p2_64 = LinFloat64Vector3D.Create(0.0, 1.0, 0.0);
        var p3_64 = LinFloat64Vector3D.Create(-1.0, 0.0, 0.0);

        var circle32 = cgaFloat32.DefineRealRoundCircleFromPoints(p1_32, p2_32, p3_32);
        var circle64 = cgaFloat64.DefineRealRoundCircleFromPoints(p1_64, p2_64, p3_64);

        var radius32 = circle32.RealRadius;
        var radius64 = (float)circle64.RealRadius;

        // Float32 should be within 0.01% of Float64
        Assert.InRange(radius32, radius64 * 0.9999f, radius64 * 1.0001f);
    }
}
```

**✅ Verification:**
```bash
dotnet test GeometricAlgebraFulcrumLib.Tests --filter "FullyQualifiedName~Float32"
# All Float32 tests should pass
```

---

## 🎯 Success Criteria

### Must Have (MVP)
- [ ] ✅ Generator compiles without errors
- [ ] ✅ Scalars.Float32 generated (10 files)
- [ ] ✅ LinearAlgebra.Float32 generated (203 files)
- [ ] ✅ GeometricAlgebra.Float32 generated (129 files)
- [ ] ✅ CGa.Float32 generated (83 files)
- [ ] ✅ Generated code compiles without errors
- [ ] ✅ Basic functional test passes (Circle from 3 points)

### Should Have
- [ ] ✅ PGa.Float32 + VGa.Float32 generated
- [ ] ✅ Unit tests for all rewriter methods
- [ ] ✅ Integration tests pass
- [ ] ✅ Build script for correct generation order
- [ ] ✅ Incremental build works (only changed files regenerated)

### Nice to Have
- [ ] ⭐ All 1108 files generated
- [ ] ⭐ IDE IntelliSense works for Float32 types
- [ ] ⭐ Generator performance <5 seconds
- [ ] ⭐ Documentation for generator usage

---

## 🚨 Known Issues & Edge Cases

### Issue 1: Circular Dependencies
**Problem:** CGa depends on LA, LA might reference CGa in some cases
**Solution:** Use multi-pass generation or explicit ordering

### Issue 2: Generic Constraints
**Problem:** `where T : IFloat64Scalar` → `where T : IFloat32Scalar`
**Solution:** SyntaxRewriter should handle TypeConstraintSyntax

### Issue 3: XML Documentation
**Problem:** Doc comments might reference Float64 types
**Solution:** Apply rewriter to trivia/comments too

### Issue 4: Preprocessor Directives
**Problem:** `#if FLOAT64` directives
**Solution:** Handle PreprocessorDirectiveSyntax

### Issue 5: File Path Conflicts
**Problem:** Multiple files with same name in different folders
**Solution:** Include full relative path in hint name

---

## 📊 Progress Tracking

### Phase 0: Setup ✅
- [ ] Create Generator project
- [ ] Configure NuGet packages
- [ ] Configure consumer projects

### Phase 1: Rewriter ✅
- [ ] Namespace rewriting
- [ ] Class/struct/interface names
- [ ] Type references (double/float)
- [ ] Using directives
- [ ] Numeric literals
- [ ] Math function calls

### Phase 2: Generator ✅
- [ ] Incremental generator skeleton
- [ ] AdditionalFiles pipeline
- [ ] Source output
- [ ] Error handling

### Phase 3: Configuration ✅
- [ ] Dependency graph
- [ ] Project configuration
- [ ] Build order

### Phase 4: Testing ✅
- [ ] Unit tests (rewriter)
- [ ] Integration tests (generator)
- [ ] Functional tests (generated code)

### Phase 5: Integration ✅
- [ ] Multi-project build
- [ ] Build scripts
- [ ] Incremental compilation

### Phase 6: Validation ✅
- [ ] Compile-time verification
- [ ] Runtime validation
- [ ] Performance testing

---

## 🔍 Implementation Context for Claude Code

### Files to Create (Prioritized Order)

1. **GeometricAlgebraFulcrumLib.CodeGeneration.csproj** (5 min)
2. **Float32SyntaxRewriter.cs** (2-3 hours) ← START HERE
3. **Float32SourceGenerator.cs** (3-4 hours)
4. **ModuleDependencyGraph.cs** (30 min)
5. **Float32SyntaxRewriterTests.cs** (2 hours)
6. **GeneratorIntegrationTests.cs** (2 hours)
7. **Float32FunctionalTests.cs** (1 hour)
8. **build-float32.ps1** (30 min)
9. **Directory.Build.props** updates (30 min)

### Key Context for Each Phase

**When implementing Rewriter:**
- Read example Float64 files first to understand patterns
- Test each Visit* method incrementally
- Use `SyntaxFactory` for creating new nodes
- Preserve trivia (comments, whitespace)

**When implementing Generator:**
- Start with single-file generation first
- Add error handling early
- Use cancellation tokens properly
- Test with small subset before full generation

**When implementing Tests:**
- Use minimal code examples
- Test each transformation independently
- Verify compilation of generated code
- Compare Float32 vs Float64 results for accuracy

### Roslyn Documentation References

**Essential Roslyn Docs:**
- Source Generators Overview: https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview
- Incremental Generators: https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md
- Syntax Rewriters: https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxrewriter

**NuGet Packages:**
```xml
<PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.8.0" />
<PackageReference Include="Microsoft.CodeAnalysis.Analyzers" Version="3.3.4" />
```

---

## 🎯 Final Notes for Implementation

**Start Small:**
1. Begin with Float32SyntaxRewriter
2. Test with single file (e.g., LinFloat64Vector3D.cs)
3. Verify transformations are correct
4. Then scale to full generator

**Incremental Approach:**
1. ✅ Phase 1: Rewriter (works on single file)
2. ✅ Phase 2: Generator (processes all files)
3. ✅ Phase 3: Build integration
4. ✅ Phase 4-6: Testing & validation

**When Stuck:**
- Refer to example Float64 files
- Use Roslyn Syntax Visualizer (VS extension)
- Check generated code in obj/Debug/generated/
- Add diagnostic output in generator

**Performance Tips:**
- Use Incremental Generator API (not ISourceGenerator)
- Cache semantic models
- Avoid redundant string allocations
- Batch file processing

---

**Status:** Ready for Implementation
**Estimated Total Time:** 25-35 hours
**Priority:** HIGH (Required for Arc-Spline Project)

**Next Action:** Start with Phase 1, Task 1.1 (Float32SyntaxRewriter.cs)
