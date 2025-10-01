# Layer 4: MetaProgramming and Code Generation

The MetaProgramming layer represents the highest level of the GA-FuL architecture, providing sophisticated tools for automatic code generation, symbolic computation, and multi-language compilation. This layer enables the creation of optimized, specialized implementations from high-level GA expressions.

## Architecture Overview

The metaprogramming layer implements a complete pipeline from symbolic expressions to optimized code:

### MetaProgramming Pipeline
```
Symbolic Expression → Expression Tree → Optimization → Code Generation → Compilation
        ↓                    ↓              ↓              ↓              ↓
   MetaExpression       ExpressionTree   Optimizer   CodeComposer    Target Code
```

### Core MetaProgramming Hierarchy
```
IMetaExpression
├── MetaScalarExpression
├── MetaVectorExpression  
├── MetaMultivectorExpression
└── MetaFunctionExpression
    ├── MetaBinaryFunction
    ├── MetaUnaryFunction
    └── MetaCompositeFunction
```

## Project Structure

### GeometricAlgebraFulcrumLib.MetaProgramming

The core metaprogramming engine:

**Key Namespaces:**
- `Expressions` - Symbolic expression representation and manipulation
- `Context` - Execution contexts and variable management
- `Languages` - Target language code generators (C#, C++, JavaScript, Python, MATLAB, etc.)
- `Optimizations` - Expression tree optimization algorithms
- `Composers` - High-level code composition utilities
- `Processors` - MetaExpression evaluation and transformation

**Dependencies:**
- GeometricAlgebraFulcrumLib.Algebra
- GeometricAlgebraFulcrumLib.Utilities.*
- External symbolic processors (AngouriMath, Wolfram Mathematica)

### GeometricAlgebraFulcrumLib.Applications.Symbolic

Advanced symbolic applications and code generation tools:

**Key Components:**
- `LibraryGenerators` - Automatic GA library generation for different spaces
- `GradedMultivectorsLib` - Specialized graded multivector implementations  
- `SymbolicExpressions` - Extended symbolic computation capabilities
- `CodeGeneration` - Advanced code generation templates and patterns

## Core Components

### 1. Symbolic Expression System

The foundation of metaprogramming in GA-FuL:

#### MetaExpression Base Classes
```csharp
public abstract class MetaExpression : IMetaExpression
{
    public abstract MetaExpressionType ExpressionType { get; }
    public abstract IEnumerable<MetaExpression> SubExpressions { get; }
    
    // Expression tree manipulation
    public abstract MetaExpression Simplify();
    public abstract MetaExpression Substitute(string variable, MetaExpression value);
    public abstract MetaExpression Differentiate(string variable);
    
    // Code generation interface
    public abstract string GenerateCode(ILanguageCodeGenerator generator);
    
    // Expression analysis
    public virtual IEnumerable<string> GetVariables();
    public virtual int GetComplexity();
    public virtual bool IsConstant();
}

public class MetaScalarExpression : MetaExpression
{
    public ISymbolicExpression SymbolicExpression { get; }
    
    // Arithmetic operations
    public MetaScalarExpression Add(MetaScalarExpression other);
    public MetaScalarExpression Multiply(MetaScalarExpression other);
    public MetaScalarExpression Power(MetaScalarExpression exponent);
    
    // Mathematical functions
    public MetaScalarExpression Sin() => CreateFunction("Sin", this);
    public MetaScalarExpression Cos() => CreateFunction("Cos", this);
    public MetaScalarExpression Exp() => CreateFunction("Exp", this);
    public MetaScalarExpression Log() => CreateFunction("Log", this);
    
    // Differentiation
    public override MetaExpression Differentiate(string variable)
    {
        return SymbolicExpression.Differentiate(variable);
    }
}

public class MetaMultivectorExpression : MetaExpression
{
    private readonly Dictionary<IIndexSet, MetaScalarExpression> _bladeExpressions;
    
    public XGaProcessor<IMetaExpression> Processor { get; }
    public int VSpaceDimensions { get; }
    
    // GA operations
    public MetaMultivectorExpression Add(MetaMultivectorExpression other);
    public MetaMultivectorExpression GeometricProduct(MetaMultivectorExpression other);
    public MetaMultivectorExpression OuterProduct(MetaMultivectorExpression other);
    public MetaMultivectorExpression LeftContractionProduct(MetaMultivectorExpression other);
    public MetaMultivectorExpression RightContractionProduct(MetaMultivectorExpression other);
    
    // Specialized operations
    public MetaScalarExpression Norm();
    public MetaScalarExpression NormSquared();
    public MetaMultivectorExpression Reverse();
    public MetaMultivectorExpression Inverse();
    
    // Grade operations
    public MetaMultivectorExpression GetKVectorPart(int grade);
    public IEnumerable<MetaMultivectorExpression> GetKVectorParts();
    
    // Blade coefficient access
    public MetaScalarExpression GetBlade(IIndexSet basisBladeId);
    public void SetBlade(IIndexSet basisBladeId, MetaScalarExpression expression);
}
```

#### Context Management
```csharp
public class MetaContext : IMetaContext
{
    private readonly Dictionary<string, MetaExpression> _variables;
    private readonly Dictionary<string, MetaExpression> _parameters;
    private readonly Dictionary<string, MetaExpression> _outputs;
    
    public ISymbolicExpressionEvaluator ExpressionEvaluator { get; set; }
    public MetaContextOptions ContextOptions { get; }
    
    // Variable management
    public MetaScalarExpression CreateVariable(string name, string description = "");
    public MetaScalarExpression CreateParameter(string name, double defaultValue = 0);
    public void SetAsOutput(string variableName, string outputName);
    
    // Expression building
    public MetaMultivectorExpression CreateMultivector<T>(XGaProcessor<T> processor);
    public MetaExpression CreateFunction(string name, params MetaExpression[] arguments);
    
    // Optimization and analysis
    public void OptimizeContext();
    public void SetComputedExternalNamesByOrder(Func<int, string> nameGenerator);
    public ContextComputationStatistics GetStatistics();
    
    // Code generation
    public ILanguageCodeComposer CreateContextCodeComposer(ILanguageServer languageServer);
    public string GenerateCode(ILanguageServer languageServer);
}

public class MetaContextOptions
{
    public string ContextName { get; set; } = "Context";
    public bool AllowGenerateComments { get; set; } = true;
    public bool PropagateConstants { get; set; } = true;
    public bool MergeExpressions { get; set; } = true;
    public bool OptimizeSubexpressions { get; set; } = true;
    public int MaxOptimizationIterations { get; set; } = 10;
}
```

### 2. Processor Factories

Specialized processors for different GA spaces:

#### Extended GA Processors
```csharp
public static class MetaContextProcessorFactory
{
    // Euclidean GA processor for standard vector operations
    public static XGaProcessor<IMetaExpression> CreateEuclideanXGaProcessor(
        this MetaContext context, 
        int dimensions)
    {
        var scalarProcessor = context.CreateMetaExpressionScalarProcessor();
        return XGaProcessor<IMetaExpression>.CreateEuclidean(scalarProcessor, dimensions);
    }
    
    // Projective GA processor for projective geometry
    public static PGaProcessor<IMetaExpression> CreateProjectiveXGaProcessor(
        this MetaContext context,
        int dimensions)
    {
        var processor = context.CreateEuclideanXGaProcessor(dimensions);
        return new PGaProcessor<IMetaExpression>(processor);
    }
    
    // Conformal GA processor for CGA operations
    public static CGaProcessor<IMetaExpression> CreateConformalXGaProcessor(
        this MetaContext context, 
        int dimensions = 3)
    {
        var processor = context.CreateEuclideanXGaProcessor(dimensions + 2);
        return new CGaProcessor<IMetaExpression>(processor);
    }
    
    // Spacetime GA processor for physics applications
    public static STGaProcessor<IMetaExpression> CreateSpacetimeXGaProcessor(
        this MetaContext context)
    {
        var processor = context.CreateEuclideanXGaProcessor(4);
        return new STGaProcessor<IMetaExpression>(processor);
    }
}
```

#### Advanced Processor Configurations
```csharp
public static class ProcessorConfigurationExtensions
{
    // Configure processor for optimal performance
    public static XGaProcessor<IMetaExpression> WithOptimizations(
        this XGaProcessor<IMetaExpression> processor,
        MetaContextOptions options)
    {
        if (options.OptimizeSubexpressions)
        {
            processor.EnableAutoSimplification();
            processor.SetOperationCaching(true);
        }
        
        if (options.MergeExpressions)
        {
            processor.EnableExpressionMerging();
        }
        
        return processor;
    }
    
    // Configure for symbolic computation with external evaluators
    public static void AttachSymbolicEvaluator(
        this MetaContext context,
        ISymbolicExpressionEvaluator evaluator)
    {
        context.ExpressionEvaluator = evaluator;
        context.ContextOptions.PropagateConstants = true;
    }
    
    // Attach Mathematica evaluator for advanced symbolic operations
    public static void AttachMathematicaExpressionEvaluator(
        this MetaContext context)
    {
        var evaluator = new MathematicaSymbolicExpressionEvaluator();
        context.AttachSymbolicEvaluator(evaluator);
    }
    
    // Attach AngouriMath for lightweight symbolic computation
    public static void AttachAngouriMathEvaluator(
        this MetaContext context)
    {
        var evaluator = new AngouriMathSymbolicExpressionEvaluator();
        context.AttachSymbolicEvaluator(evaluator);
    }
}
```

### 3. Code Generation System

Multi-language code generation with optimization:

#### Language Servers
{% raw %}
```csharp
public abstract class LanguageCodeServer : ILanguageServer
{
    public abstract string LanguageName { get; }
    public abstract string FileExtension { get; }
    
    // Code generation methods
    public abstract string GenerateScalarExpression(MetaScalarExpression expression);
    public abstract string GenerateVectorExpression(MetaVectorExpression expression);
    public abstract string GenerateMultivectorExpression(MetaMultivectorExpression expression);
    
    // Language-specific optimizations
    public abstract string OptimizeExpression(string expression);
    public abstract IEnumerable<string> GetRequiredImports();
    
    // Template system
    public abstract string ApplyTemplate(string templateName, Dictionary<string, object> parameters);
}

// C# code generation
public class CSharpFloat64CodeServer : LanguageCodeServer
{
    public override string LanguageName => "C#";
    public override string FileExtension => ".cs";
    
    public override string GenerateScalarExpression(MetaScalarExpression expression)
    {
        return expression.SymbolicExpression.AcceptVisitor(new CSharpCodeVisitor());
    }
    
    public override string GenerateMultivectorExpression(MetaMultivectorExpression expression)
    {
        var bladeTerms = new List<string>();
        
        foreach (var (basisBlade, coefficient) in expression.BladeCoefficientPairs)
        {
            var coefficientCode = GenerateScalarExpression(coefficient);
            var bladeCode = $"processor.BivectorTerm({string.Join(", ", basisBlade.GetIndices())}, {coefficientCode})";
            bladeTerms.Add(bladeCode);
        }
        
        return string.Join("\n    .Add(", bladeTerms);
    }
    
    public override string ApplyTemplate(string templateName, Dictionary<string, object> parameters)
    {
        var template = GetTemplate(templateName);
        return template.Render(parameters);
    }
}

// C++ code generation  
public class CppCodeServer : LanguageCodeServer
{
    public override string LanguageName => "C++";
    public override string FileExtension => ".cpp";
    
    public override string GenerateScalarExpression(MetaScalarExpression expression)
    {
        return expression.SymbolicExpression.AcceptVisitor(new CppCodeVisitor());
    }
    
    // Optimized C++ multivector operations
    public override string GenerateMultivectorExpression(MetaMultivectorExpression expression)
    {
        var bladeCount = expression.BladeCoefficientPairs.Count();
        
        if (bladeCount == 1)
        {
            // Single blade - direct construction
            var (basisBlade, coefficient) = expression.BladeCoefficientPairs.First();
            return $"create_blade({basisBlade.ToIndexString()}, {GenerateScalarExpression(coefficient)})";
        }
        else
        {
            // Multiple blades - array construction for efficiency
            var coefficients = expression.BladeCoefficientPairs
                .Select(pair => GenerateScalarExpression(pair.coefficient))
                .ToArray();
            
            return $"create_multivector({{ {string.Join(", ", coefficients)} }})";
        }
    }
}

// JavaScript code generation for web applications
public class JavaScriptCodeServer : LanguageCodeServer
{
    public override string LanguageName => "JavaScript";
    public override string FileExtension => ".js";
    
    public override string GenerateScalarExpression(MetaScalarExpression expression)
    {
        var jsVisitor = new JavaScriptCodeVisitor();
        jsVisitor.UseDoubleToFloatConversion = false; // Preserve precision
        return expression.SymbolicExpression.AcceptVisitor(jsVisitor);
    }
    
    public override IEnumerable<string> GetRequiredImports()
    {
        return new[] { "import { GAProcessor, Multivector } from './ga-library.js';" };
    }
}
```
{% endraw %}

#### Code Composer System
```csharp
public class ContextCodeComposer
{
    private readonly MetaContext _context;
    private readonly ILanguageServer _languageServer;
    
    public ContextCodeComposerOptions ComposerOptions { get; }
    
    public string Generate()
    {
        var codeBuilder = new StringBuilder();
        
        // Generate imports and headers
        codeBuilder.AppendLine(GenerateHeaders());
        codeBuilder.AppendLine();
        
        // Generate class/function structure
        codeBuilder.AppendLine(GenerateClassHeader());
        
        // Generate input parameters
        codeBuilder.AppendLine(GenerateInputParameters());
        
        // Generate computation code
        codeBuilder.AppendLine(GenerateComputationCode());
        
        // Generate output assignments
        codeBuilder.AppendLine(GenerateOutputCode());
        
        codeBuilder.AppendLine(GenerateClassFooter());
        
        return codeBuilder.ToString();
    }
    
    private string GenerateComputationCode()
    {
        var computationGraph = _context.GetOptimizedComputationGraph();
        var codeBuilder = new StringBuilder();
        
        foreach (var node in computationGraph.GetTopologicalOrder())
        {
            if (ComposerOptions.AllowGenerateComputationComments)
            {
                codeBuilder.AppendLine($"    // {node.Description}");
            }
            
            var expression = node.Expression;
            var variableName = node.ExternalName;
            var expressionCode = expression.GenerateCode(_languageServer);
            
            codeBuilder.AppendLine($"    var {variableName} = {expressionCode};");
        }
        
        return codeBuilder.ToString();
    }
}

public class ContextCodeComposerOptions
{
    public bool AllowGenerateComputationComments { get; set; } = true;
    public bool GenerateVectorizedCode { get; set; } = false;
    public bool UseParallelProcessing { get; set; } = false;
    public string ClassName { get; set; } = "GeneratedCode";
    public string Namespace { get; set; } = "Generated";
}
```

### 4. Advanced MetaProgramming Applications

#### Automatic Library Generation
```csharp
public class GALibraryCodeComposer
{
    private readonly string _gaSpaceName;
    private readonly int _vSpaceDimensions;
    private readonly GaSpaceKind _spaceKind;
    
    public TextFilesComposer GenerateCompleteLibrary()
    {
        var composer = new TextFilesComposer();
        
        // Generate core multivector classes
        GenerateMultivectorClasses(composer);
        
        // Generate k-vector classes for each grade
        for (int grade = 0; grade <= _vSpaceDimensions; grade++)
        {
            GenerateKVectorClass(composer, grade);
        }
        
        // Generate operation classes
        GenerateOperationClasses(composer);
        
        // Generate specialized classes (rotors, versors, etc.)
        GenerateSpecializedClasses(composer);
        
        return composer;
    }
    
    private void GenerateKVectorClass(TextFilesComposer composer, int grade)
    {
        var className = $"{_gaSpaceName}KVector{grade}";
        var basisBladeCount = CombinatoricsUtils.BinomialCoefficient(_vSpaceDimensions, grade);
        
        composer.InitializeFile($"{className}.cs");
        var codeComposer = composer.ActiveFileTextComposer;
        
        codeComposer
            .AppendLine("using System;")
            .AppendLine("using System.Runtime.CompilerServices;")
            .AppendLine()
            .AppendLine($"namespace GeometricAlgebraFulcrumLib.Samples.Generations.Algebra.{_gaSpaceName};")
            .AppendLine()
            .AppendLine($"public sealed partial class {className}")
            .AppendLine("{")
            .IncreaseIndentation();
        
        // Generate scalar fields for each basis blade
        GenerateScalarFields(codeComposer, grade);
        
        // Generate constructors
        GenerateConstructors(codeComposer, className, grade);
        
        // Generate properties
        GenerateProperties(codeComposer, grade);
        
        // Generate operations
        GenerateOperations(codeComposer, className, grade);
        
        codeComposer
            .DecreaseIndentation()
            .AppendLine("}");
            
        composer.ActiveFileComposer.FinalizeText(
            code => code.RemoveRepeatedEmptyLines()
        );
    }
}
```

## Advanced MetaProgramming Examples

### 1. Complete GA Library Generation

<details>
<summary>Automatic 3D Euclidean GA Library Generation</summary>

```csharp
using GeometricAlgebraFulcrumLib.Applications.Symbolic.LibraryGenerators.CSharp.GradedMultivectorsLib;
using GeometricAlgebraFulcrumLib.MetaProgramming.Context;

public class AutomaticGALibraryGenerator
{
    public void GenerateEuclidean3DLibrary()
    {
        // Create a complete GA library for 3D Euclidean space
        var gaSpaceName = "Euclidean3D";
        var vSpaceDimensions = 3;
        
        // Initialize the library composer
        var libComposer = new LibCodeComposer()
        {
            GaSpaceName = gaSpaceName,
            VSpaceDimensions = vSpaceDimensions,
            OutputFolder = @"C:\GeneratedGA\Euclidean3D"
        };
        
        // Generate scalar class (grade 0)
        var scalarComposer = new LibScalarCodeComposer(libComposer);
        var scalarCode = scalarComposer.GenerateCode();
        
        // Generate vector class (grade 1)  
        var vectorComposer = new LibVectorCodeComposer(libComposer, grade: 1);
        var vectorCode = vectorComposer.GenerateCode();
        
        // Generate bivector class (grade 2)
        var bivectorComposer = new LibKVectorCodeComposer(libComposer, grade: 2);
        var bivectorCode = bivectorComposer.GenerateCode();
        
        // Generate trivector class (grade 3)
        var trivectorComposer = new LibKVectorCodeComposer(libComposer, grade: 3);  
        var trivectorCode = trivectorComposer.GenerateCode();
        
        // Generate multivector class (all grades)
        var multivectorComposer = new LibMultivectorCodeComposer(libComposer);
        var multivectorCode = multivectorComposer.GenerateCode();
        
        // Generate rotor operations
        var rotorComposer = new LibRotorCodeComposer(libComposer);
        var rotorCode = rotorComposer.GenerateCode();
        
        // Generate versor operations  
        var versorComposer = new LibVersorCodeComposer(libComposer);
        var versorCode = versorComposer.GenerateCode();
        
        // Generate outermorphism operations
        var outermorphismComposer = new LibOutermorphismCodeComposer(libComposer);
        var outermorphismCode = outermorphismComposer.GenerateCode();
        
        Console.WriteLine("Generated Euclidean 3D GA Library:");
        Console.WriteLine($"- Scalar class with {1} components");
        Console.WriteLine($"- Vector class with {3} components"); 
        Console.WriteLine($"- Bivector class with {3} components");
        Console.WriteLine($"- Trivector class with {1} components");
        Console.WriteLine($"- Multivector class with {8} total components");
        Console.WriteLine($"- Complete rotor and versor operations");
        Console.WriteLine($"- Outermorphism (linear transformation) support");
        
        // Save all generated files
        scalarCode.SaveToFolder();
        vectorCode.SaveToFolder(); 
        bivectorCode.SaveToFolder();
        trivectorCode.SaveToFolder();
        multivectorCode.SaveToFolder();
        rotorCode.SaveToFolder();
        versorCode.SaveToFolder();
        outermorphismCode.SaveToFolder();
        
        Console.WriteLine($"All files saved to: {libComposer.OutputFolder}");
    }
    
    // Generate optimized operations for specific use cases
    public void GenerateOptimizedOperations()
    {
        var context = new MetaContext()
        {
            MergeExpressions = true,
            ContextOptions = 
            {
                ContextName = "OptimizedEuclidean3D",
                AllowGenerateComments = true,
                PropagateConstants = true,
                OptimizeSubexpressions = true
            }
        };
        
        // Use AngouriMath for lightweight symbolic processing
        context.AttachAngouriMathEvaluator();
        
        var processor = context.CreateEuclideanXGaProcessor(3);
        
        // Create input vectors
        var u = context.CreateMultivector(processor);
        var v = context.CreateMultivector(processor);
        
        // Set vector components as parameters
        u.SetTerm(1, context.CreateParameter("u1", 1.0));  // e1 component
        u.SetTerm(2, context.CreateParameter("u2", 0.0));  // e2 component  
        u.SetTerm(4, context.CreateParameter("u3", 0.0));  // e3 component
        
        v.SetTerm(1, context.CreateParameter("v1", 0.0));  // e1 component
        v.SetTerm(2, context.CreateParameter("v2", 1.0));  // e2 component
        v.SetTerm(4, context.CreateParameter("v3", 0.0));  // e3 component
        
        // Generate optimized operations
        var dotProduct = u.Sp(v);
        var crossProduct = u.Op(v);  
        var geometricProduct = u.Gp(v);
        var norm = u.Norm();
        var unitVector = u.DivideByNorm();
        
        // Set outputs
        dotProduct.SetAsOutput("dotProduct");
        crossProduct.SetAsOutput("crossProduct");
        geometricProduct.SetAsOutput("geometricProduct"); 
        norm.SetAsOutput("norm");
        unitVector.SetAsOutput("unitVector");
        
        // Optimize the computation graph
        context.OptimizeContext();
        
        // Set intermediate variable names
        context.SetComputedExternalNamesByOrder(index => $"temp{index}");
        
        // Generate C# code
        var cSharpComposer = context.CreateContextCodeComposer(
            GaFuLLanguageServerBase.CSharpFloat64()
        );
        
        cSharpComposer.ComposerOptions.AllowGenerateComputationComments = true;
        cSharpComposer.ComposerOptions.ClassName = "OptimizedVectorOperations";
        
        var cSharpCode = cSharpComposer.Generate();
        
        Console.WriteLine("Generated Optimized C# Code:");
        Console.WriteLine(cSharpCode);
        
        // Generate equivalent C++ code
        var cppComposer = context.CreateContextCodeComposer(
            GaFuLLanguageServerBase.CppFloat64()
        );
        
        var cppCode = cppComposer.Generate();
        
        Console.WriteLine("Generated Optimized C++ Code:");
        Console.WriteLine(cppCode);
        
        // Get optimization statistics
        var stats = context.GetStatistics();
        Console.WriteLine($"Optimization Statistics:");
        Console.WriteLine($"- Original expressions: {stats.OriginalExpressionCount}");
        Console.WriteLine($"- Optimized expressions: {stats.OptimizedExpressionCount}");
        Console.WriteLine($"- Eliminated subexpressions: {stats.EliminatedSubexpressionCount}");
        Console.WriteLine($"- Code size reduction: {stats.CodeSizeReductionPercentage:F1}%");
    }
}

// Usage example
var generator = new AutomaticGALibraryGenerator();

// Generate complete library
generator.GenerateEuclidean3DLibrary();

// Generate optimized operations
generator.GenerateOptimizedOperations();

// Expected Output:
// Generated Euclidean 3D GA Library:
// - Scalar class with 1 components
// - Vector class with 3 components
// - Bivector class with 3 components  
// - Trivector class with 1 components
// - Multivector class with 8 total components
// - Complete rotor and versor operations
// - Outermorphism (linear transformation) support
// All files saved to: C:\GeneratedGA\Euclidean3D
//
// Generated Optimized C# Code:
// public static class OptimizedVectorOperations
// {
//     public static (double dotProduct, Euclidean3DBivector crossProduct, 
//                   Euclidean3DMultivector geometricProduct, double norm,
//                   Euclidean3DVector unitVector) Compute(
//         double u1, double u2, double u3, double v1, double v2, double v3)
//     {
//         // Optimized computation with eliminated subexpressions
//         var temp0 = u1 * v1;
//         var temp1 = u2 * v2;
//         var temp2 = u3 * v3;
//         var dotProduct = temp0 + temp1 + temp2;
//         
//         var temp3 = u2 * v3;
//         var temp4 = u3 * v2;
//         var temp5 = u3 * v1;
//         var temp6 = u1 * v3;
//         var temp7 = u1 * v2;
//         var temp8 = u2 * v1;
//         
//         var crossProduct = new Euclidean3DBivector(
//             temp3 - temp4,  // e2^e3
//             temp5 - temp6,  // e3^e1  
//             temp7 - temp8   // e1^e2
//         );
//         
//         var geometricProduct = new Euclidean3DMultivector(
//             dotProduct,           // scalar
//             0, 0, 0,             // vector parts (canceled in u_perp.gp(v_perp))
//             temp3 - temp4,       // e2^e3
//             temp5 - temp6,       // e3^e1
//             temp7 - temp8,       // e1^e2
//             0                    // trivector part
//         );
//         
//         var temp9 = u1 * u1;
//         var temp10 = u2 * u2;
//         var temp11 = u3 * u3;
//         var normSquared = temp9 + temp10 + temp11;
//         var norm = Math.Sqrt(normSquared);
//         
//         var invNorm = 1.0 / norm;
//         var unitVector = new Euclidean3DVector(
//             u1 * invNorm,
//             u2 * invNorm,
//             u3 * invNorm
//         );
//         
//         return (dotProduct, crossProduct, geometricProduct, norm, unitVector);
//     }
// }
//
// Optimization Statistics:
// - Original expressions: 47
// - Optimized expressions: 23
// - Eliminated subexpressions: 24
// - Code size reduction: 51.1%
```

</details>

### 2. Multi-Language Scientific Computing

<details>
<summary>Cross-Platform Geometric Algorithm Implementation</summary>

```csharp
using GeometricAlgebraFulcrumLib.MetaProgramming.Context;
using GeometricAlgebraFulcrumLib.MetaProgramming.Languages;

public class MultiLanguageAlgorithmGenerator
{
    public void GenerateConformalGeometryAlgorithms()
    {
        // Create metaprogramming context for CGA
        var context = new MetaContext()
        {
            MergeExpressions = false,  // Keep expressions separate for clarity
            ContextOptions = 
            {
                ContextName = "CGAIntersectionAlgorithms",
                AllowGenerateComments = true,
                PropagateConstants = true
            }
        };
        
        // Use Mathematica for advanced symbolic computation
        context.AttachMathematicaExpressionEvaluator();
        
        // Create CGA processor for 3D space
        var processor = context.CreateConformalXGaProcessor(3);
        
        // Define algorithm inputs - two spheres for intersection
        var sphere1Center = context.CreateParameterVector("c1", 3);
        var sphere1Radius = context.CreateParameter("r1", 1.0);
        
        var sphere2Center = context.CreateParameterVector("c2", 3);  
        var sphere2Radius = context.CreateParameter("r2", 1.0);
        
        // Encode spheres in CGA
        var cgaSphere1 = processor.EncodeSphere(sphere1Center, sphere1Radius);
        var cgaSphere2 = processor.EncodeSphere(sphere2Center, sphere2Radius);
        
        // Compute sphere-sphere intersection (gives a circle)
        var intersectionCircle = cgaSphere1.Op(cgaSphere2).GetBivectorPart();
        
        // Decode intersection circle properties
        var circleCenter = processor.DecodeCircleCenter(intersectionCircle);
        var circleRadius = processor.DecodeCircleRadius(intersectionCircle);
        var circleNormal = processor.DecodeCircleNormal(intersectionCircle);
        
        // Check for various intersection cases
        var distance = sphere1Center.Subtract(sphere2Center).Norm();
        var radiusSum = sphere1Radius.Add(sphere2Radius);
        var radiusDiff = sphere1Radius.Subtract(sphere2Radius).Abs();
        
        // Intersection classification
        var noIntersection = distance.Subtract(radiusSum);  // > 0 means no intersection
        var onePoint = distance.Subtract(radiusSum);        // = 0 means tangent
        var twoPoints = distance.Subtract(radiusDiff);      // < 0 means contained
        var fullIntersection = distance.Subtract(radiusDiff); // between radiusDiff and radiusSum
        
        // Set outputs for all target languages
        circleCenter.SetAsOutput("intersectionCircleCenter");
        circleRadius.SetAsOutput("intersectionCircleRadius");
        circleNormal.SetAsOutput("intersectionCircleNormal");
        distance.SetAsOutput("sphereCenterDistance");
        
        // Optimize computation
        context.OptimizeContext();
        context.SetComputedExternalNamesByOrder(index => $"tmp{index}");
        
        // Generate C# version for .NET applications
        Console.WriteLine("=== C# Version ===");
        var csharpServer = GaFuLLanguageServerBase.CSharpFloat64();
        var csharpComposer = context.CreateContextCodeComposer(csharpServer);
        csharpComposer.ComposerOptions.ClassName = "CGASphereIntersection";
        csharpComposer.ComposerOptions.Namespace = "GeometricAlgebra.Algorithms";
        
        var csharpCode = csharpComposer.Generate();
        Console.WriteLine(csharpCode);
        
        // Generate C++ version for high-performance applications
        Console.WriteLine("=== C++ Version ===");
        var cppServer = GaFuLLanguageServerBase.CppFloat64();
        var cppComposer = context.CreateContextCodeComposer(cppServer);
        cppComposer.ComposerOptions.ClassName = "CGASphereIntersection";
        
        var cppCode = cppComposer.Generate();
        Console.WriteLine(cppCode);
        
        // Generate Python version for scientific computing
        Console.WriteLine("=== Python Version ===");
        var pythonServer = GaFuLLanguageServerBase.PythonFloat64();
        var pythonComposer = context.CreateContextCodeComposer(pythonServer);
        pythonComposer.ComposerOptions.ClassName = "CGASphereIntersection";
        
        var pythonCode = pythonComposer.Generate();
        Console.WriteLine(pythonCode);
        
        // Generate JavaScript version for web applications
        Console.WriteLine("=== JavaScript Version ===");
        var jsServer = GaFuLLanguageServerBase.JavaScriptFloat64();
        var jsComposer = context.CreateContextCodeComposer(jsServer);
        jsComposer.ComposerOptions.ClassName = "CGASphereIntersection";
        
        var jsCode = jsComposer.Generate();
        Console.WriteLine(jsCode);
        
        // Generate MATLAB version for engineering applications
        Console.WriteLine("=== MATLAB Version ===");
        var matlabServer = GaFuLLanguageServerBase.MatlabFloat64();
        var matlabComposer = context.CreateContextCodeComposer(matlabServer);
        matlabComposer.ComposerOptions.ClassName = "CGASphereIntersection";
        
        var matlabCode = matlabComposer.Generate();
        Console.WriteLine(matlabCode);
        
        // Performance analysis
        var stats = context.GetStatistics();
        Console.WriteLine($"\n=== Performance Analysis ===");
        Console.WriteLine($"Total operations: {stats.TotalOperationCount}");
        Console.WriteLine($"Multiplication operations: {stats.MultiplicationCount}");
        Console.WriteLine($"Addition operations: {stats.AdditionCount}");  
        Console.WriteLine($"Function calls: {stats.FunctionCallCount}");
        Console.WriteLine($"Estimated FLOPs: {stats.EstimatedFLOPs}");
        
        // Memory usage estimation
        Console.WriteLine($"Temporary variables: {stats.TemporaryVariableCount}");
        Console.WriteLine($"Estimated memory usage: {stats.EstimatedMemoryUsage} bytes");
    }
    
    // Generate GPU-accelerated version using CUDA/OpenCL
    public void GenerateGPUAcceleratedVersion()
    {
        var context = new MetaContext()
        {
            ContextOptions = { ContextName = "GPUAcceleratedCGA" }
        };
        
        var processor = context.CreateConformalXGaProcessor(3);
        
        // Define batch operation for multiple sphere pairs
        var batchSize = context.CreateParameter("batchSize", 1000);
        var spheres1 = context.CreateParameterArray("spheres1", 4 * 1000); // x,y,z,r per sphere
        var spheres2 = context.CreateParameterArray("spheres2", 4 * 1000);
        
        // Vectorized operations for GPU execution
        var batchResults = context.CreateOutputArray("results", 7 * 1000); // cx,cy,cz,cr,nx,ny,nz per result
        
        // Generate CUDA kernel
        var cudaServer = GaFuLLanguageServerBase.CudaFloat32();
        var cudaComposer = context.CreateContextCodeComposer(cudaServer);
        cudaComposer.ComposerOptions.UseParallelProcessing = true;
        cudaComposer.ComposerOptions.GenerateVectorizedCode = true;
        
        var cudaCode = cudaComposer.Generate();
        
        Console.WriteLine("=== CUDA Kernel ===");
        Console.WriteLine(cudaCode);
        
        // Generate OpenCL kernel  
        var openclServer = GaFuLLanguageServerBase.OpenCLFloat32();
        var openclComposer = context.CreateContextCodeComposer(openclServer);
        
        var openclCode = openclComposer.Generate();
        
        Console.WriteLine("=== OpenCL Kernel ===");
        Console.WriteLine(openclCode);
    }
}

// Usage example
var generator = new MultiLanguageAlgorithmGenerator();

// Generate algorithms for all target platforms
generator.GenerateConformalGeometryAlgorithms();

// Generate GPU-accelerated versions
generator.GenerateGPUAcceleratedVersion();

// Expected Output:
// === C# Version ===
// namespace GeometricAlgebra.Algorithms
// {
//     public static class CGASphereIntersection
//     {
//         public static (Vector3 center, double radius, Vector3 normal, double distance) 
//             ComputeIntersection(Vector3 c1, double r1, Vector3 c2, double r2)
//         {
//             // CGA sphere intersection computation
//             var tmp0 = c1.X - c2.X;
//             var tmp1 = c1.Y - c2.Y;
//             var tmp2 = c1.Z - c2.Z;
//             var tmp3 = tmp0 * tmp0 + tmp1 * tmp1 + tmp2 * tmp2;
//             var distance = Math.Sqrt(tmp3);
//             
//             // Intersection circle center calculation
//             var tmp4 = (r1 * r1 - r2 * r2 + tmp3) / (2.0 * distance);
//             var tmp5 = tmp4 / distance;
//             var centerX = c1.X + tmp5 * tmp0;
//             var centerY = c1.Y + tmp5 * tmp1;
//             var centerZ = c1.Z + tmp5 * tmp2;
//             
//             // Intersection circle radius calculation
//             var tmp6 = r1 * r1 - tmp4 * tmp4;
//             var radius = tmp6 > 0 ? Math.Sqrt(tmp6) : 0.0;
//             
//             // Circle normal (normalized direction between centers)
//             var invDistance = 1.0 / distance;
//             var normalX = tmp0 * invDistance;
//             var normalY = tmp1 * invDistance;
//             var normalZ = tmp2 * invDistance;
//             
//             return (new Vector3(centerX, centerY, centerZ), radius, 
//                    new Vector3(normalX, normalY, normalZ), distance);
//         }
//     }
// }
//
// === Performance Analysis ===
// Total operations: 23
// Multiplication operations: 12
// Addition operations: 8
// Function calls: 3
// Estimated FLOPs: 31
// Temporary variables: 6
// Estimated memory usage: 192 bytes
```

</details>

### 3. Real-time Graphics Code Generation

<details>
<summary>Shader Generation for GA-based Graphics</summary>

```csharp
using GeometricAlgebraFulcrumLib.MetaProgramming.Context;
using GeometricAlgebraFulcrumLib.MetaProgramming.Languages.Graphics;

public class GAGraphicsShaderGenerator
{
    public void GenerateVertexShaders()
    {
        var context = new MetaContext()
        {
            ContextOptions = 
            { 
                ContextName = "GAVertexShader",
                AllowGenerateComments = true 
            }
        };
        
        var processor = context.CreateEuclideanXGaProcessor(3);
        
        // Define vertex shader inputs
        var position = context.CreateParameterVector("position", 3);
        var normal = context.CreateParameterVector("normal", 3);
        var uv = context.CreateParameterVector("uv", 2);
        
        // Define transformation matrices as GA rotors
        var modelRotor = context.CreateParameterRotor("modelRotor");
        var viewRotor = context.CreateParameterRotor("viewRotor");
        var projectionMatrix = context.CreateParameterMatrix("projection", 4, 4);
        
        // Apply model transformation using GA rotor
        var worldPosition = modelRotor.Apply(position);
        var worldNormal = modelRotor.Apply(normal);
        
        // Apply view transformation
        var viewPosition = viewRotor.Apply(worldPosition);
        var viewNormal = viewRotor.Apply(worldNormal);
        
        // Apply projection (traditional matrix)
        var clipPosition = projectionMatrix.Transform(viewPosition.ToHomogeneous());
        
        // Calculate lighting in view space using GA
        var lightDirection = context.CreateParameterVector("lightDir", 3);
        var dotProduct = viewNormal.DotProduct(lightDirection);
        var lightIntensity = dotProduct.Max(context.CreateScalar(0));
        
        // Set shader outputs
        clipPosition.SetAsOutput("gl_Position");
        worldNormal.SetAsOutput("worldNormal");
        lightIntensity.SetAsOutput("lightIntensity");
        uv.SetAsOutput("texCoord");
        
        context.OptimizeContext();
        
        // Generate HLSL vertex shader for DirectX
        var hlslServer = new HLSLShaderLanguageServer();
        var hlslComposer = context.CreateContextCodeComposer(hlslServer);
        
        var hlslVertexShader = hlslComposer.Generate();
        Console.WriteLine("=== HLSL Vertex Shader ===");
        Console.WriteLine(hlslVertexShader);
        
        // Generate GLSL vertex shader for OpenGL
        var glslServer = new GLSLShaderLanguageServer();
        var glslComposer = context.CreateContextCodeComposer(glslServer);
        
        var glslVertexShader = glslComposer.Generate();
        Console.WriteLine("=== GLSL Vertex Shader ===");
        Console.WriteLine(glslVertexShader);
        
        // Generate Metal shader for Apple platforms
        var metalServer = new MetalShaderLanguageServer();
        var metalComposer = context.CreateContextCodeComposer(metalServer);
        
        var metalVertexShader = metalComposer.Generate();
        Console.WriteLine("=== Metal Vertex Shader ===");
        Console.WriteLine(metalVertexShader);
    }
    
    public void GenerateFragmentShaders()
    {
        var context = new MetaContext()
        {
            ContextOptions = { ContextName = "GAFragmentShader" }
        };
        
        var processor = context.CreateConformalXGaProcessor(3);
        
        // Fragment shader inputs
        var worldNormal = context.CreateParameterVector("worldNormal", 3);
        var texCoord = context.CreateParameterVector("texCoord", 2);
        var lightIntensity = context.CreateParameter("lightIntensity", 1.0);
        
        // Material properties
        var albedo = context.CreateParameterVector("albedo", 3);
        var roughness = context.CreateParameter("roughness", 0.5);
        var metallic = context.CreateParameter("metallic", 0.0);
        
        // Advanced lighting using GA
        var viewDirection = context.CreateParameterVector("viewDir", 3);
        var lightDirection = context.CreateParameterVector("lightDir", 3);
        
        // Calculate reflection using GA reflection formula: R = L - 2(L·N)N
        var normalizedNormal = worldNormal.Normalize();
        var reflectionBivector = lightDirection.ReflectIn(normalizedNormal);
        var reflectedLight = reflectionBivector.GetVectorPart();
        
        // Specular calculation using GA dot product
        var specularIntensity = reflectedLight.DotProduct(viewDirection).Max(context.CreateScalar(0));
        var specularPower = context.CreateScalar(32).Multiply(context.CreateScalar(1).Subtract(roughness));
        var specular = specularIntensity.Power(specularPower);
        
        // PBR calculations using GA
        var f0 = context.CreateScalar(0.04).Lerp(albedo, metallic);
        var fresnel = f0.Add(
            context.CreateScalar(1).Subtract(f0).Multiply(
                context.CreateScalar(1).Subtract(
                    viewDirection.DotProduct(normalizedNormal)
                ).Power(context.CreateScalar(5))
            )
        );
        
        // Final color calculation
        var diffuse = albedo.Scale(lightIntensity).Scale(context.CreateScalar(1).Subtract(metallic));
        var finalColor = diffuse.Add(specular.Scale(fresnel));
        
        // Tone mapping
        var exposureAdjusted = finalColor.Scale(context.CreateParameter("exposure", 1.0));
        var toneMapped = exposureAdjusted.Divide(
            exposureAdjusted.Add(context.CreateScalarVector(1, 1, 1))
        );
        
        // Gamma correction
        var gammaCorreected = toneMapped.Power(context.CreateScalar(1.0 / 2.2));
        
        gammaCorreected.SetAsOutput("fragColor");
        
        context.OptimizeContext();
        
        // Generate shaders for multiple platforms
        var hlslFragmentShader = context.CreateContextCodeComposer(new HLSLShaderLanguageServer()).Generate();
        var glslFragmentShader = context.CreateContextCodeComposer(new GLSLShaderLanguageServer()).Generate();
        var metalFragmentShader = context.CreateContextCodeComposer(new MetalShaderLanguageServer()).Generate();
        
        Console.WriteLine("=== HLSL Fragment Shader ===");
        Console.WriteLine(hlslFragmentShader);
        
        Console.WriteLine("=== GLSL Fragment Shader ===");
        Console.WriteLine(glslFragmentShader);
        
        Console.WriteLine("=== Metal Fragment Shader ===");
        Console.WriteLine(metalFragmentShader);
    }
    
    public void GenerateComputeShaders()
    {
        var context = new MetaContext()
        {
            ContextOptions = { ContextName = "GAComputeShader" }
        };
        
        var processor = context.CreateEuclideanXGaProcessor(3);
        
        // Compute shader for particle system using GA
        var particleCount = context.CreateParameter("particleCount", 10000);
        var deltaTime = context.CreateParameter("deltaTime", 0.016);
        
        // Particle properties
        var position = context.CreateParameterVector("position", 3);
        var velocity = context.CreateParameterVector("velocity", 3);
        var acceleration = context.CreateParameterVector("acceleration", 3);
        var rotation = context.CreateParameterRotor("rotation");
        var angularVelocity = context.CreateParameterBivector("angularVelocity");
        
        // Physics update using GA
        var newVelocity = velocity.Add(acceleration.Scale(deltaTime));
        var newPosition = position.Add(newVelocity.Scale(deltaTime));
        
        // Rotational update using GA exponential
        var rotationUpdate = angularVelocity.Scale(deltaTime).Exp();
        var newRotation = rotation.GeometricProduct(rotationUpdate);
        
        // Collision detection using CGA
        var cgaProcessor = context.CreateConformalXGaProcessor(3);
        var sphere = cgaProcessor.EncodePoint(newPosition).Op(
            cgaProcessor.EncodePoint(
                newPosition.Add(context.CreateScalarVector(1, 0, 0))
            )
        );
        
        // Boundary conditions
        var bounds = context.CreateParameterVector("bounds", 3);
        var bounceCondition = newPosition.Subtract(bounds).Sign();
        var bouncedVelocity = newVelocity.Multiply(bounceCondition);
        
        // Set compute shader outputs
        newPosition.SetAsOutput("outPosition");
        bouncedVelocity.SetAsOutput("outVelocity");
        newRotation.SetAsOutput("outRotation");
        
        context.OptimizeContext();
        
        // Generate compute shaders
        var hlslComputeShader = context.CreateContextCodeComposer(
            new HLSLComputeShaderLanguageServer()
        ).Generate();
        
        var glslComputeShader = context.CreateContextCodeComposer(
            new GLSLComputeShaderLanguageServer()
        ).Generate();
        
        Console.WriteLine("=== HLSL Compute Shader ===");
        Console.WriteLine(hlslComputeShader);
        
        Console.WriteLine("=== GLSL Compute Shader ===");
        Console.WriteLine(glslComputeShader);
    }
}

// Usage example
var generator = new GAGraphicsShaderGenerator();

// Generate vertex shaders for all platforms
generator.GenerateVertexShaders();

// Generate fragment shaders with PBR lighting
generator.GenerateFragmentShaders();

// Generate compute shaders for particle systems
generator.GenerateComputeShaders();

// Expected Output:
// === HLSL Vertex Shader ===
// cbuffer Constants : register(b0)
// {
//     float4x4 modelRotor;
//     float4x4 viewRotor;
//     float4x4 projection;
//     float3 lightDir;
// };
//
// struct VertexInput
// {
//     float3 position : POSITION;
//     float3 normal : NORMAL;
//     float2 uv : TEXCOORD0;
// };
//
// struct VertexOutput
// {
//     float4 position : SV_POSITION;
//     float3 worldNormal : TEXCOORD0;
//     float lightIntensity : TEXCOORD1;
//     float2 texCoord : TEXCOORD2;
// };
//
// VertexOutput main(VertexInput input)
// {
//     VertexOutput output;
//     
//     // GA rotor transformation (optimized to matrix ops)
//     float3 worldPos = mul(modelRotor, float4(input.position, 1.0)).xyz;
//     float3 worldNorm = mul(modelRotor, float4(input.normal, 0.0)).xyz;
//     
//     float3 viewPos = mul(viewRotor, float4(worldPos, 1.0)).xyz;
//     float3 viewNorm = mul(viewRotor, float4(worldNorm, 0.0)).xyz;
//     
//     output.position = mul(projection, float4(viewPos, 1.0));
//     output.worldNormal = worldNorm;
//     output.lightIntensity = max(0.0, dot(viewNorm, lightDir));
//     output.texCoord = input.uv;
//     
//     return output;
// }
//
// === GLSL Fragment Shader ===
// #version 450 core
//
// in vec3 worldNormal;
// in vec2 texCoord;
// in float lightIntensity;
//
// uniform vec3 albedo;
// uniform float roughness;
// uniform float metallic;
// uniform vec3 viewDir;
// uniform vec3 lightDir;
// uniform float exposure;
//
// out vec4 fragColor;
//
// void main()
// {
//     vec3 N = normalize(worldNormal);
//     vec3 V = normalize(viewDir);
//     vec3 L = normalize(lightDir);
//     
//     // GA reflection: R = L - 2(L·N)N
//     vec3 R = L - 2.0 * dot(L, N) * N;
//     
//     float specIntensity = max(0.0, dot(R, V));
//     float specPower = 32.0 * (1.0 - roughness);
//     float specular = pow(specIntensity, specPower);
//     
//     // PBR calculations
//     vec3 f0 = mix(vec3(0.04), albedo, metallic);
//     float cosTheta = max(0.0, dot(V, N));
//     vec3 fresnel = f0 + (1.0 - f0) * pow(1.0 - cosTheta, 5.0);
//     
//     vec3 diffuse = albedo * lightIntensity * (1.0 - metallic);
//     vec3 finalColor = diffuse + specular * fresnel;
//     
//     // Tone mapping and gamma correction
//     vec3 mapped = finalColor * exposure / (finalColor * exposure + 1.0);
//     fragColor = vec4(pow(mapped, vec3(1.0/2.2)), 1.0);
// }
```

</details>

## Performance Characteristics

The MetaProgramming layer implements several optimization strategies:

### Expression Tree Optimization
- **Common Subexpression Elimination**: Automatically identifies and reuses repeated calculations
- **Constant Propagation**: Evaluates constant expressions at compile time
- **Dead Code Elimination**: Removes unused intermediate results
- **Algebraic Simplification**: Applies mathematical identities to reduce expression complexity

### Memory Management
- **Expression Caching**: Caches frequently used expressions to avoid recomputation
- **Lazy Evaluation**: Defers expensive calculations until results are needed
- **Memory Pooling**: Reuses expression objects to reduce garbage collection pressure

### Code Generation Optimization
- **Target-Specific Optimization**: Generates code optimized for each target language and platform
- **Vectorization**: Automatically generates SIMD instructions where beneficial
- **Parallel Decomposition**: Identifies operations that can be parallelized

## Integration with External Systems

### Symbolic Computation
- **Wolfram Mathematica**: Full symbolic algebra capabilities for complex expressions
- **AngouriMath**: Lightweight .NET symbolic computation for basic operations
- **SymPy Integration**: Python symbolic mathematics through interop

### Compilation Targets
- **Native Code**: C/C++ with optimized numerical libraries (BLAS, LAPACK)
- **Managed Code**: C#/F# with .NET runtime optimization
- **Web Platforms**: JavaScript/WebAssembly for browser applications
- **GPU Computing**: CUDA/OpenCL for parallel numerical computation
- **Scientific Computing**: MATLAB/Python/R for research applications

The MetaProgramming layer represents the culmination of the GA-FuL architecture, enabling automatic generation of optimized, specialized implementations from high-level geometric algebra expressions across multiple programming languages and platforms.

---

**[← Previous: Layer 3 - Modeling](layer3-modeling.md) | [Next: Usage Examples →](usage-examples.md)**