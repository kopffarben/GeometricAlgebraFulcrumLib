using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace GAF.Gen;

/// <summary>
/// Roslyn Syntax Rewriter that transforms Float64 code to Float32.
/// Handles comprehensive transformations including types, literals, math functions, and BitConverter calls.
/// Special handling: Filters out existing Float32 namespace references to avoid conflicts.
/// </summary>
public class Float32SyntaxRewriter : CSharpSyntaxRewriter
{
    // Namespaces that should be filtered out (already Float32)
    private static readonly string[] Float32NamespacesToFilter = new[]
    {
        "GeometricAlgebraFulcrumLib.Algebra.Scalars.Float32",
        "GeometricAlgebraFulcrumLib.Algebra.LinearAlgebra.Float32",
        "GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float32",
        "GeometricAlgebraFulcrumLib.Modeling.Geometry.CGa.Float32",
        "GeometricAlgebraFulcrumLib.Modeling.Geometry.PGa.Float32",
        "GeometricAlgebraFulcrumLib.Modeling.Geometry.VGa.Float32",
    };

    // Track the current class being visited for blacklist checking
    private string? _currentClassName;

    // Track if we're inside a Vector<Complex>.Real() or .Imaginary() invocation
    // to prevent incorrect casting of these methods (they return Vector<double>, not double)
    private bool _insideVectorComplexMethod;

    public Float32SyntaxRewriter() : base(visitIntoStructuredTrivia: false)
    {
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

    public override SyntaxNode? VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
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
        // Track the current class name for blacklist checking (using original name before transformation)
        var previousClassName = _currentClassName;
        _currentClassName = node.Identifier.Text;

        // Also track if it's a partial class
        var isPartial = node.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

        var newName = ReplaceFloat64ToFloat32(node.Identifier.Text);
        var result = base.VisitClassDeclaration(
            node.WithIdentifier(SyntaxFactory.Identifier(newName))
        );

        // Restore previous class name after visiting
        _currentClassName = previousClassName;

        return result;
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
        // Track the current record name for blacklist checking (using original name before transformation)
        var previousClassName = _currentClassName;
        _currentClassName = node.Identifier.Text;

        var newName = ReplaceFloat64ToFloat32(node.Identifier.Text);
        var result = base.VisitRecordDeclaration(
            node.WithIdentifier(SyntaxFactory.Identifier(newName))
        );

        // Restore previous class name after visiting
        _currentClassName = previousClassName;

        return result;
    }

    public override SyntaxNode? VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        // Constructor names must match the class/struct name
        var newName = ReplaceFloat64ToFloat32(node.Identifier.Text);
        return base.VisitConstructorDeclaration(
            node.WithIdentifier(SyntaxFactory.Identifier(newName))
        );
    }

    // ============================================================
    // OPERATOR & CONVERSION DECLARATIONS: Skip double overloads to prevent duplicates
    // ============================================================

    public override SyntaxNode? VisitOperatorDeclaration(OperatorDeclarationSyntax node)
    {
        // SKIP: Operator overloads with 'float' parameters in Float64 code
        // In UnaryBinaryOps files, there are both float and double versions
        // Example in Float64: operator +(XGaFloat64Multivector, float) AND operator +(XGaFloat64Multivector, double)
        // We keep only the double version, which transforms to float in Float32
        // This prevents duplicates: both would become operator +(XGaFloat32Multivector, float)
        if (HasFloatParameter(node.ParameterList))
        {
            return null; // Remove this node from the generated code
        }

        // KEEP: Operators with double parameters - they will be transformed to float
        // Example: operator +(XGaFloat64Multivector, double) → operator +(XGaFloat32Multivector, float)

        return base.VisitOperatorDeclaration(node);
    }

    /// <summary>
    /// Checks if any parameter in the parameter list has type 'float'
    /// </summary>
    private static bool HasFloatParameter(ParameterListSyntax parameterList)
    {
        foreach (var parameter in parameterList.Parameters)
        {
            if (IsFloatType(parameter.Type))
                return true;
        }
        return false;
    }

    public override SyntaxNode? VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
    {
        // SKIP: Conversion operators with 'double' parameter (incoming conversions)
        // Example: implicit operator Float64Scalar(double)
        // This would duplicate: implicit operator Float64Scalar(float)
        // Both become: implicit operator Float32Scalar(float) after transformation
        if (HasDoubleParameter(node.ParameterList))
        {
            return null; // Remove this node
        }

        // SKIP: explicit operator float(Float64Scalar)
        // This becomes redundant because implicit operator double(Float64Scalar)
        // transforms to implicit operator float(Float32Scalar)
        if (IsFloatType(node.Type))
        {
            // Only keep if it's NOT an explicit conversion (i.e., keep implicit only)
            // But since there shouldn't be implicit operator float in Float64,
            // we can safely skip all float conversions
            return null; // Remove this node
        }

        // KEEP: Conversion operators with 'double' return type (outgoing conversions)
        // Example: implicit operator double(Float64Scalar)
        // This becomes: implicit operator float(Float32Scalar)
        // This is needed! Don't remove it - let the type transformation handle it

        return base.VisitConversionOperatorDeclaration(node);
    }

    public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        // SPECIAL CASE: ToDouble() must return 'double' to satisfy IConvertible
        // Don't transform the return type for ToDouble method
        if (node.Identifier.Text == "ToDouble")
        {
            // Keep the return type as 'double', but transform the body
            var visitedBody = node.Body != null ? (BlockSyntax?)Visit(node.Body) : null;
            var visitedExpressionBody = node.ExpressionBody != null ? (ArrowExpressionClauseSyntax?)Visit(node.ExpressionBody) : null;
            var visitedParameterList = (ParameterListSyntax?)Visit(node.ParameterList);

            return node
                .WithBody(visitedBody)
                .WithExpressionBody(visitedExpressionBody)
                .WithParameterList(visitedParameterList ?? node.ParameterList);
        }

        // SKIP: Extension methods with 'this float' parameter
        // In Float64 Utils files, there are extension methods for both float and double
        // Example: IsEqualTo(this float scalar1, ...) AND IsEqualTo(this double scalar1, ...)
        // After transformation, both become: IsEqualTo(this float scalar1, ...)
        // We keep only the double version, which transforms to float in Float32
        if (HasFloatThisParameter(node))
        {
            return null; // Remove this method from the generated code
        }

        // BLACKLIST: Skip specific methods with float parameters that would create duplicates
        // These methods exist in both float and double versions in Float64 code
        // After transformation, both become float versions, causing duplicate method errors
        if (IsBlacklistedMethod(node))
        {
            return null; // Remove this method from the generated code
        }

        // TRANSFORM: Method names containing Float64 → Float32
        // Example: GetXGaFloat64Scalar → GetXGaFloat32Scalar
        var methodName = node.Identifier.Text;
        if (methodName.IndexOf("Float64", System.StringComparison.Ordinal) >= 0)
        {
            var newMethodName = ReplaceFloat64ToFloat32(methodName);
            var newIdentifier = SyntaxFactory.Identifier(
                node.Identifier.LeadingTrivia,
                newMethodName,
                node.Identifier.TrailingTrivia
            );
            node = node.WithIdentifier(newIdentifier);
        }

        // TRANSFORM: LinVector method names → LinFloat32Vector
        // Examples: ToLinVector2D → ToLinFloat32Vector2D, CreateLinVector → CreateLinFloat32Vector
        // This matches the transformation done in VisitInvocationExpression for call sites
        if (methodName.StartsWith("ToLin") || methodName.StartsWith("CreateLin") || methodName.StartsWith("CreateUnitLin"))
        {
            var newMethodName = methodName
                .Replace("ToLinVector", "ToLinFloat32Vector")
                .Replace("CreateLinVector", "CreateLinFloat32Vector")
                .Replace("CreateUnitLinVector", "CreateUnitLinFloat32Vector");

            if (newMethodName != methodName)
            {
                var newIdentifier = SyntaxFactory.Identifier(
                    node.Identifier.LeadingTrivia,
                    newMethodName,
                    node.Identifier.TrailingTrivia
                );
                node = node.WithIdentifier(newIdentifier);
            }
        }

        return base.VisitMethodDeclaration(node);
    }

    /// <summary>
    /// Checks if the method is an extension method with 'this float' or 'this SomeType<float>' as first parameter
    /// </summary>
    private static bool HasFloatThisParameter(MethodDeclarationSyntax method)
    {
        // Extension methods must be static
        if (!method.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)))
            return false;

        // Check if first parameter has 'this' modifier
        if (method.ParameterList.Parameters.Count > 0)
        {
            var firstParam = method.ParameterList.Parameters[0];
            if (firstParam.Modifiers.Any(m => m.IsKind(SyntaxKind.ThisKeyword)))
            {
                // Check if type is 'float' OR contains 'float' in generic type arguments
                // Example: this float x → true
                // Example: this IPair<float> x → true
                // Example: this IPair<double> x → false
                return ContainsFloatType(firstParam.Type);
            }
        }

        return false;
    }

    /// <summary>
    /// Recursively checks if a type syntax contains 'float' anywhere (direct type or generic type argument)
    /// </summary>
    private static bool ContainsFloatType(TypeSyntax? typeSyntax)
    {
        if (typeSyntax == null)
            return false;

        // Direct float type
        if (IsFloatType(typeSyntax))
            return true;

        // Generic type with type arguments: e.g., IPair<float>, Dictionary<string, float>
        if (typeSyntax is GenericNameSyntax genericName)
        {
            foreach (var typeArg in genericName.TypeArgumentList.Arguments)
            {
                if (ContainsFloatType(typeArg))
                    return true;
            }
        }

        // Qualified name: e.g., System.Collections.Generic.List<float>
        if (typeSyntax is QualifiedNameSyntax qualifiedName)
        {
            return ContainsFloatType(qualifiedName.Right);
        }

        return false;
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
                SyntaxFactory.Token(
                    node.Keyword.LeadingTrivia,
                    SyntaxKind.FloatKeyword,
                    node.Keyword.TrailingTrivia
                )
            );
        }

        return base.VisitPredefinedType(node);
    }

    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        var text = node.Identifier.Text;

        // SKIP: Already Float32 - don't transform again
        // This handles cases where Float64 code already references Float32 types
        if (text.IndexOf("Float32", System.StringComparison.Ordinal) >= 0)
        {
            // Already Float32, keep as-is
            return base.VisitIdentifierName(node);
        }

        // SKIP: External extension methods from Utilities.Structures
        // GetFloat64 is defined in external assembly and returns double
        // Keep as-is, will be cast at invocation site
        if (text == "GetFloat64" || text == "GetFloat32")
        {
            return base.VisitIdentifierName(node);
        }

        // Float64 → Float32 (in type names and internal method names)
        if (text.IndexOf("Float64", System.StringComparison.Ordinal) >= 0)
        {
            var newText = ReplaceFloat64ToFloat32(text);
            return node.WithIdentifier(
                SyntaxFactory.Identifier(
                    node.Identifier.LeadingTrivia,
                    newText,
                    node.Identifier.TrailingTrivia
                )
            );
        }

        // ToLinVector → ToLinFloat32Vector (standalone function calls)
        // This handles cases like: return ToLinVector4D(vector) or return ToLinVector()
        // where the function is called without an object prefix
        if (text.StartsWith("ToLinVector") || text.StartsWith("CreateLinVector") || text.StartsWith("CreateUnitLinVector"))
        {
            var newText = text
                .Replace("ToLinVector", "ToLinFloat32Vector")
                .Replace("CreateLinVector", "CreateLinFloat32Vector")
                .Replace("CreateUnitLinVector", "CreateUnitLinFloat32Vector");

            if (newText != text)
            {
                return node.WithIdentifier(
                    SyntaxFactory.Identifier(
                        node.Identifier.LeadingTrivia,
                        newText,
                        node.Identifier.TrailingTrivia
                    )
                );
            }
        }

        // Math → MathF transformation is now handled in VisitMemberAccessExpression
        // to be more selective (only for floating-point methods, not for Max/Min/Abs with integers)
        // So we don't blindly transform "Math" identifier here

        return base.VisitIdentifierName(node);
    }

    public override SyntaxNode? VisitGenericName(GenericNameSyntax node)
    {
        var text = node.Identifier.Text;

        // SKIP: Already Float32 - don't transform again
        if (text.IndexOf("Float32", System.StringComparison.Ordinal) >= 0)
        {
            return base.VisitGenericName(node);
        }

        // Generic types with Float64 in name
        if (text.IndexOf("Float64", System.StringComparison.Ordinal) >= 0)
        {
            var newText = ReplaceFloat64ToFloat32(text);
            return base.VisitGenericName(
                node.WithIdentifier(
                    SyntaxFactory.Identifier(
                        node.Identifier.LeadingTrivia,
                        newText,
                        node.Identifier.TrailingTrivia
                    )
                )
            );
        }

        // MathNet.Numerics types: Vector<double> → Vector<float>, Matrix<double> → Matrix<float>
        // IMPORTANT: Vector<Complex> and Matrix<Complex> should NOT be transformed
        // because Complex is always double-based (no float-Complex in .NET)
        if ((text == "Vector" || text == "Matrix") && node.TypeArgumentList != null)
        {
            var typeArgs = node.TypeArgumentList.Arguments;

            // Check if type argument is Complex - if so, don't transform
            foreach (var typeArg in typeArgs)
            {
                if (typeArg is IdentifierNameSyntax identifier && identifier.Identifier.Text == "Complex")
                {
                    // Vector<Complex> or Matrix<Complex> - keep as-is, don't transform
                    return base.VisitGenericName(node);
                }
            }

            // Check if any type argument is double and needs to be transformed to float
            bool needsTransform = false;
            foreach (var typeArg in typeArgs)
            {
                if (typeArg is PredefinedTypeSyntax predefined &&
                    predefined.Keyword.IsKind(SyntaxKind.DoubleKeyword))
                {
                    needsTransform = true;
                    break;
                }
            }

            if (needsTransform)
            {
                // Transform the type arguments through the visitor
                // This will convert double → float
                return base.VisitGenericName(node);
            }
        }

        return base.VisitGenericName(node);
    }

    // ============================================================
    // 4. USING DIRECTIVES: Float64 → Float32 in imports, filter Float32 namespaces
    // ============================================================

    public override SyntaxNode? VisitUsingDirective(UsingDirectiveSyntax node)
    {
        var nameText = node.Name?.ToString();
        if (nameText == null)
            return base.VisitUsingDirective(node);

        // MathNet.Numerics.LinearAlgebra.Double → MathNet.Numerics.LinearAlgebra.Single
        if (nameText == "MathNet.Numerics.LinearAlgebra.Double")
        {
            var newNameSyntax = SyntaxFactory.ParseName("MathNet.Numerics.LinearAlgebra.Single");
            return node.WithName(newNameSyntax);
        }

        // DON'T FILTER: Keep Float32 using directives as they are needed
        // The generated Float32 code may reference Float32Utils and other Float32 types
        // Only transform Float64 → Float32 in using directives
        if (nameText.IndexOf("Float64", System.StringComparison.Ordinal) >= 0)
        {
            var newName = ReplaceFloat64ToFloat32(nameText);
            var newNameSyntax = SyntaxFactory.ParseName(newName);

            return node.WithName(newNameSyntax);
        }

        return base.VisitUsingDirective(node);
    }

    // ============================================================
    // 5. NUMERIC LITERALS: Add 'f' suffix, handle 'd' suffix
    // ============================================================

    public override SyntaxNode? VisitLiteralExpression(LiteralExpressionSyntax node)
    {
        if (node.IsKind(SyntaxKind.NumericLiteralExpression))
        {
            var text = node.Token.Text;
            var value = node.Token.Value;

            // Handle double literals that need to become float
            if (value is double doubleValue)
            {
                // If it has 'd' or 'D' suffix, replace with 'f'
                if (text.EndsWith("d", System.StringComparison.OrdinalIgnoreCase))
                {
                    var newText = text.Substring(0, text.Length - 1) + "f";
                    var newToken = SyntaxFactory.Literal(
                        node.Token.LeadingTrivia,
                        newText,
                        (float)doubleValue,
                        node.Token.TrailingTrivia
                    );
                    return node.WithToken(newToken);
                }

                // If it has decimal point but no suffix, add 'f'
                // Also handle scientific notation (e.g., 1e-12)
                if ((text.IndexOf('.') >= 0 || text.IndexOf('e') >= 0 || text.IndexOf('E') >= 0) &&
                    !text.EndsWith("f", System.StringComparison.OrdinalIgnoreCase) &&
                    !text.EndsWith("m", System.StringComparison.OrdinalIgnoreCase))
                {
                    var newText = text + "f";
                    var newToken = SyntaxFactory.Literal(
                        node.Token.LeadingTrivia,
                        newText,
                        (float)doubleValue,
                        node.Token.TrailingTrivia
                    );
                    return node.WithToken(newToken);
                }
            }
        }

        return base.VisitLiteralExpression(node);
    }

    // ============================================================
    // 6. MEMBER ACCESS: Math.Sin → MathF.Sin, BitConverter transformations
    // ============================================================

    public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        // Declare memberName once at the beginning of the method to avoid scope conflicts
        var memberName = node.Name.Identifier.Text;

        // Math.XXX → MathF.XXX (only for floating-point methods)
        if (node.Expression is IdentifierNameSyntax identifier)
        {
            if (identifier.Identifier.Text == "Math")
            {

                // Only transform Math → MathF for floating-point specific methods
                // DO NOT transform Max, Min, Abs when they might be used with integers
                // These methods are overloaded and context-dependent
                var floatingPointMethods = new HashSet<string>
                {
                    "Sin", "Cos", "Tan", "Asin", "Acos", "Atan", "Atan2",
                    "Sinh", "Cosh", "Tanh", "Asinh", "Acosh", "Atanh",
                    "Sqrt", "Cbrt", "Pow", "Exp", "Log", "Log10", "Log2",
                    "Floor", "Ceiling", "Round", "Truncate",
                    "SinCos", "SinCosPi", "CosPi", "SinPi", "TanPi",  // .NET 7+ tuple methods
                    "PI", "E", "Tau" // Constants
                };

                if (floatingPointMethods.Contains(memberName))
                {
                    var newExpression = SyntaxFactory.IdentifierName("MathF");
                    return base.VisitMemberAccessExpression(
                        node.WithExpression(newExpression)
                    );
                }

                // For Max, Min, Abs, Sign - keep as Math
                // The C# compiler will select the correct overload based on argument types
                // Math.Max(int, int) stays as Math.Max
                // Math.Max(double, double) will be handled by keeping the arguments as-is
                // and relying on implicit conversions if needed
            }

            // BitConverter.DoubleToUInt64Bits → BitConverter.SingleToUInt32Bits
            if (identifier.Identifier.Text == "BitConverter")
            {
                var memberName2 = node.Name.Identifier.Text;
                var newMemberName = memberName2 switch
                {
                    "DoubleToUInt64Bits" => "SingleToUInt32Bits",
                    "DoubleToInt64Bits" => "SingleToInt32Bits",
                    "UInt64BitsToDouble" => "UInt32BitsToSingle",
                    "Int64BitsToDouble" => "Int32BitsToSingle",
                    _ => memberName2
                };

                if (newMemberName != memberName2)
                {
                    var newName = SyntaxFactory.IdentifierName(newMemberName);
                    return base.VisitMemberAccessExpression(
                        node.WithName(newName)
                    );
                }
            }

            // double.XXX → float.XXX (for static methods like double.IsNaN)
            if (identifier.Identifier.Text == "double")
            {
                var newExpression = SyntaxFactory.IdentifierName("float");
                return base.VisitMemberAccessExpression(
                    node.WithExpression(newExpression)
                );
            }
        }

        // Cast Complex properties (Magnitude, Real, Imaginary) to float
        // Complex is a .NET type that always returns double
        // IMPORTANT: Skip casting if we're inside a Vector<Complex>.Real() or .Imaginary() method call
        if ((memberName == "Magnitude" || memberName == "Real" || memberName == "Imaginary") && !_insideVectorComplexMethod)
        {
            // Check if the expression might be a Complex type
            // We can't perfectly determine the type without semantic analysis,
            // but these property names are unique to Complex
            var visitedNode = (MemberAccessExpressionSyntax)base.VisitMemberAccessExpression(node)!;

            // Wrap in cast: (float)complexNumber.Magnitude
            var castExpression = SyntaxFactory.CastExpression(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword)),
                visitedNode
            );

            return castExpression;
        }

        return base.VisitMemberAccessExpression(node);
    }

    // ============================================================
    // 6.5. INVOCATION EXPRESSIONS: Transform method calls
    // ============================================================

    public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        // Handle Random.NextDouble() → (float)Random.NextDouble()
        // Since there's no Random.NextSingle() in older .NET versions
        if (node.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var memberName = memberAccess.Name.Identifier.Text;

            // SPECIAL CASE: MathNet.Numerics Vector<Complex>.Real() and .Imaginary() methods
            // These return Vector<double>, NOT scalar double, so they should NOT be cast to float
            // The VisitMemberAccessExpression incorrectly adds (float) cast thinking they are Complex properties
            // We need to skip cast insertion for these method invocations
            if (memberName == "Real" || memberName == "Imaginary")
            {
                // Set flag to prevent casting in VisitMemberAccessExpression
                var previousFlag = _insideVectorComplexMethod;
                _insideVectorComplexMethod = true;

                var result = base.VisitInvocationExpression(node);

                // Restore previous flag state
                _insideVectorComplexMethod = previousFlag;

                return result;
            }

            // NextDouble() needs cast to float
            if (memberName == "NextDouble")
            {
                // Visit the base expression first to transform any Float64 → Float32
                var visitedNode = (InvocationExpressionSyntax)base.VisitInvocationExpression(node)!;

                // Wrap in cast: (float)expression
                var castExpression = SyntaxFactory.CastExpression(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword)),
                    visitedNode
                );

                return castExpression;
            }

            // GetFloat64 from Utilities.Structures (external assembly, not generated)
            // Cast the result instead of changing method name: (float)random.GetFloat64()
            if (memberName == "GetFloat64" || memberName == "GetFloat32")
            {
                // Keep the method name as-is, but cast the result to float
                var visitedNode = (InvocationExpressionSyntax)base.VisitInvocationExpression(node)!;

                // Wrap in cast: (float)expression
                var castExpression = SyntaxFactory.CastExpression(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword)),
                    visitedNode
                );

                return castExpression;
            }

            // Math.BitDecrement, Math.BitIncrement, Math.FusedMultiplyAdd
            // These methods don't have MathF equivalents and return double
            // Keep as Math.X() but cast the result to float: (float)Math.BitDecrement(value)
            if (memberName == "BitDecrement" || memberName == "BitIncrement" || memberName == "FusedMultiplyAdd")
            {
                // Visit the base expression first to transform any Float64 → Float32
                var visitedNode = (InvocationExpressionSyntax)base.VisitInvocationExpression(node)!;

                // Wrap in cast: (float)expression
                var castExpression = SyntaxFactory.CastExpression(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword)),
                    visitedNode
                );

                return castExpression;
            }

            // .ToArray() on Vector<double> or Vector<Complex>.Real()/Imaginary()
            // Needs to be transformed to: .ToArray().Select(x => (float)x).ToArray()
            // This handles MathNet.Numerics complex eigenvector conversions
            if (memberName == "ToArray")
            {
                // Check if this is a Vector<double> or Vector<Complex> method
                // by looking at the parent expression type
                var visitedNode = (InvocationExpressionSyntax)base.VisitInvocationExpression(node)!;

                // Check if the expression chain contains .Real() or .Imaginary()
                // These return Vector<double> which needs element-wise float conversion
                var expressionText = memberAccess.Expression.ToString();
                if (expressionText.Contains(".Real()") || expressionText.Contains(".Imaginary()"))
                {
                    // Transform: .ToArray() → .ToArray().Select(x => (float)x).ToArray()
                    var selectLambda = SyntaxFactory.SimpleLambdaExpression(
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("x")),
                        SyntaxFactory.CastExpression(
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword)),
                            SyntaxFactory.IdentifierName("x")
                        )
                    );

                    var selectCall = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            visitedNode,
                            SyntaxFactory.IdentifierName("Select")
                        ),
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(selectLambda)
                            )
                        )
                    );

                    var finalToArray = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            selectCall,
                            SyntaxFactory.IdentifierName("ToArray")
                        ),
                        SyntaxFactory.ArgumentList()
                    );

                    return finalToArray;
                }
            }

            // .Abs() extension method - keep as-is, Float32Utils.Abs() extension exists
            // Previously transformed to MathF.Abs() but that breaks method chaining
            // Leave it as cosAngle.Abs() and let Float32Utils extension method handle it

            // MathNet.Numerics L2Norm() returns double, needs casting ONLY when used in arithmetic
            // BUT: Do NOT cast when it's part of a method chain like .L2Norm().IsNearZero()
            // because the cast precedence will be wrong: (float)expr.L2Norm().IsNearZero()
            // gets parsed as (float)(expr.L2Norm().IsNearZero()) which tries to cast bool to float!
            //
            // Check if parent is a MemberAccessExpression (meaning this is chained)
            // If chained, don't cast - let the chained method handle the double
            if (memberName == "L2Norm")
            {
                // Check if this invocation is the expression part of a parent MemberAccessExpression
                // If so, it's being chained with another method call, so don't cast
                var parent = node.Parent;
                bool isChained = parent is MemberAccessExpressionSyntax memberAccessParent &&
                                 memberAccessParent.Expression == node;

                if (isChained)
                {
                    // Don't cast - it's chained with another method
                    // Just visit normally
                    return base.VisitInvocationExpression(node);
                }

                // Not chained - safe to cast
                var visitedNode = (InvocationExpressionSyntax)base.VisitInvocationExpression(node)!;

                // Wrap in cast: (float)expression
                var castExpression = SyntaxFactory.CastExpression(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword)),
                    visitedNode
                );

                return castExpression;
            }

            // value.IsFinite() → float.IsFinite(value)
            // double.IsFinite(value) → float.IsFinite(value)
            // In .NET, double.IsFinite can be called as instance or static
            // But float.IsFinite MUST be called statically: float.IsFinite(value)
            if (memberName == "IsFinite")
            {
                // Visit children first to transform any Float64 → Float32
                var visitedNode = (InvocationExpressionSyntax)base.VisitInvocationExpression(node)!;
                var visitedMemberAccess = (MemberAccessExpressionSyntax)visitedNode.Expression;

                var staticAccess = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword)),
                    SyntaxFactory.IdentifierName("IsFinite")
                );

                // Check if it's already a static call (e.g., double.IsFinite(value) or float.IsFinite(value))
                if (visitedMemberAccess.Expression is PredefinedTypeSyntax)
                {
                    // Already static call - just replace the type with float and keep original arguments
                    var newInvocation = SyntaxFactory.InvocationExpression(
                        staticAccess,
                        visitedNode.ArgumentList // Keep original arguments
                    );
                    return newInvocation;
                }
                else
                {
                    // Instance call - transform: expression.IsFinite() → float.IsFinite(expression)
                    var newInvocation = SyntaxFactory.InvocationExpression(
                        staticAccess,
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(visitedMemberAccess.Expression)
                            )
                        )
                    );
                    return newInvocation;
                }
            }

            // Extension methods that need Float32 versions
            // ToLinVector2D/3D/4D() → ToLinFloat32Vector2D/3D/4D()
            // This handles: LinBasisVector, ILinFloat32Vector*, Vector<double>, IPair<Float32Scalar>, etc.
            var linVectorConversionMethods = new HashSet<string>
            {
                "ToLinVector", "ToLinVector2D", "ToLinVector3D", "ToLinVector4D",
                "CreateLinVector", "CreateUnitLinVector"
            };

            if (linVectorConversionMethods.Contains(memberName))
            {
                // Transform: axis.ToLinVector2D() → axis.ToLinFloat32Vector2D()
                // Transform: array.CreateLinVector() → array.CreateLinFloat32Vector()
                var float32MethodName = memberName
                    .Replace("ToLin", "ToLinFloat32")
                    .Replace("CreateLin", "CreateLinFloat32")
                    .Replace("CreateUnitLin", "CreateUnitLinFloat32");

                var visitedNode = (MemberAccessExpressionSyntax)base.Visit(memberAccess)!;
                var newMemberAccess = visitedNode.WithName(SyntaxFactory.IdentifierName(float32MethodName));

                return base.VisitInvocationExpression(
                    node.WithExpression(newMemberAccess)
                );
            }

            // XGaParseTerms() → XGaParseTermsFloat32()
            // Transforms the shared infrastructure method to Float32 version
            if (memberName == "XGaParseTerms")
            {
                var visitedNode = (MemberAccessExpressionSyntax)base.Visit(memberAccess)!;
                var newMemberAccess = visitedNode.WithName(SyntaxFactory.IdentifierName("XGaParseTermsFloat32"));

                return base.VisitInvocationExpression(
                    node.WithExpression(newMemberAccess)
                );
            }

            // VectorPairToVectorPairRotationQuaternion() → VectorPairToVectorPairRotationFloat32Quaternion()
            // Transforms quaternion conversion from Float64 to Float32 version
            if (memberName == "VectorPairToVectorPairRotationQuaternion")
            {
                var visitedNode = (MemberAccessExpressionSyntax)base.Visit(memberAccess)!;
                var newMemberAccess = visitedNode.WithName(SyntaxFactory.IdentifierName("VectorPairToVectorPairRotationFloat32Quaternion"));

                return base.VisitInvocationExpression(
                    node.WithExpression(newMemberAccess)
                );
            }

            // GetFloat64Numbers() → GetFloat32Numbers()
            // Transforms random number generator from Float64 to Float32 version
            if (memberName == "GetFloat64Numbers")
            {
                var visitedNode = (MemberAccessExpressionSyntax)base.Visit(memberAccess)!;
                var newMemberAccess = visitedNode.WithName(SyntaxFactory.IdentifierName("GetFloat32Numbers"));

                return base.VisitInvocationExpression(
                    node.WithExpression(newMemberAccess)
                );
            }

            // Other GetXXXFloat64 methods (for internal types) → GetXXXFloat32
            // Example: GetXGaFloat64Scalar → GetXGaFloat32Scalar (already handled in VisitMethodDeclaration)
            // We don't transform invocations here because method declarations are already renamed

            // Matrix.Build.DenseOfArray - ensure type argument matches
            // This is handled by VisitGenericName, so just pass through
        }

        return base.VisitInvocationExpression(node);
    }

    // ============================================================
    // 7. DEFAULT LITERAL: default(double) → default(float)
    // ============================================================

    public override SyntaxNode? VisitDefaultExpression(DefaultExpressionSyntax node)
    {
        // This handles default(double) syntax
        return base.VisitDefaultExpression(node);
    }

    // ============================================================
    // 8. TYPEOF EXPRESSION: typeof(double) → typeof(float)
    // ============================================================

    public override SyntaxNode? VisitTypeOfExpression(TypeOfExpressionSyntax node)
    {
        // This handles typeof(double) syntax
        return base.VisitTypeOfExpression(node);
    }

    // ============================================================
    // 9. CAST EXPRESSIONS: (double)x → (float)x
    // ============================================================

    public override SyntaxNode? VisitCastExpression(CastExpressionSyntax node)
    {
        // The type in cast will be handled by VisitPredefinedType
        return base.VisitCastExpression(node);
    }

    // ============================================================
    // 10. OBJECT CREATION: new Double(...) → new Single(...)
    // ============================================================

    public override SyntaxNode? VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
    {
        // Type transformations will be handled by VisitIdentifierName
        return base.VisitObjectCreationExpression(node);
    }

    // ============================================================
    // HELPER METHODS
    // ============================================================

    /// <summary>
    /// Checks if any parameter in the parameter list has type 'double'
    /// </summary>
    private static bool HasDoubleParameter(ParameterListSyntax parameterList)
    {
        foreach (var parameter in parameterList.Parameters)
        {
            if (IsDoubleType(parameter.Type))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if a method is blacklisted (should be skipped during code generation)
    /// Blacklisted methods are those with float parameters in Float64 code that would create
    /// duplicate method declarations after transformation to Float32
    /// </summary>
    private bool IsBlacklistedMethod(MethodDeclarationSyntax node)
    {
        var methodName = node.Identifier.Text;
        var paramCount = node.ParameterList.Parameters.Count;
        var isStatic = node.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));

        // Check if all parameters are float type
        var allParamsAreFloat = node.ParameterList.Parameters.All(p =>
            p.Type is PredefinedTypeSyntax predefined &&
            predefined.Keyword.IsKind(SyntaxKind.FloatKeyword));

        if (!allParamsAreFloat)
            return false;

        // ONLY blacklist Vector Create methods that we know work
        if (_currentClassName != null)
        {
            var className = _currentClassName;

            // Vector methods in records - THESE WORK
            if ((className == "LinFloat64Vector2D" && methodName == "Create" && paramCount == 2 && isStatic) ||
                (className == "LinFloat64Vector3D" && methodName == "Create" && paramCount == 3 && isStatic))
            {
                return true;
            }

            // Processor methods with float parameters that have duplicate double versions
            // After transformation, both become float versions causing CS0111 errors
            if (className == "XGaFloat64Processor")
            {
                if ((methodName == "PureScalingRotor2D" && paramCount == 2 && allParamsAreFloat) ||
                    (methodName == "PureScalingRotor3D" && paramCount == 4 && allParamsAreFloat))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if ALL parameters are primitive numeric types (float, double, int, long, etc.)
    /// Used to identify factory/constructor overloads that likely have duplicate versions
    /// </summary>
    private static bool AllParametersArePrimitiveNumeric(ParameterListSyntax parameterList)
    {
        if (parameterList.Parameters.Count == 0)
            return false;

        foreach (var parameter in parameterList.Parameters)
        {
            if (!IsPrimitiveNumericType(parameter.Type))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if any parameter has a default value
    /// Methods with default parameters are typically unique interface implementations, not duplicate overloads
    /// </summary>
    private static bool HasDefaultParameterValues(ParameterListSyntax parameterList)
    {
        foreach (var parameter in parameterList.Parameters)
        {
            if (parameter.Default != null)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if a return type is a primitive type (numeric, bool, char, string, void)
    /// Interface implementation methods typically return primitive types
    /// Factory methods typically return complex types
    /// </summary>
    private static bool IsPrimitiveReturnType(TypeSyntax? typeSyntax)
    {
        if (typeSyntax == null)
            return false;

        if (typeSyntax is PredefinedTypeSyntax predefinedType)
        {
            // All predefined types are considered primitive for our purposes
            return predefinedType.Keyword.IsKind(SyntaxKind.FloatKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.DoubleKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.IntKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.LongKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.ShortKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.ByteKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.UIntKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.ULongKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.UShortKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.SByteKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.DecimalKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.BoolKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.CharKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.StringKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword);
        }

        return false;
    }

    /// <summary>
    /// Checks if a type is a primitive numeric type
    /// </summary>
    private static bool IsPrimitiveNumericType(TypeSyntax? typeSyntax)
    {
        if (typeSyntax == null)
            return false;

        if (typeSyntax is PredefinedTypeSyntax predefinedType)
        {
            return predefinedType.Keyword.IsKind(SyntaxKind.FloatKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.DoubleKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.IntKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.LongKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.ShortKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.ByteKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.UIntKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.ULongKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.UShortKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.SByteKeyword) ||
                   predefinedType.Keyword.IsKind(SyntaxKind.DecimalKeyword);
        }

        return false;
    }

    /// <summary>
    /// Checks if a type syntax represents 'double'
    /// </summary>
    private static bool IsDoubleType(TypeSyntax? typeSyntax)
    {
        if (typeSyntax == null)
            return false;

        // Check for predefined 'double' keyword
        if (typeSyntax is PredefinedTypeSyntax predefinedType)
        {
            return predefinedType.Keyword.IsKind(SyntaxKind.DoubleKeyword);
        }

        // Check for 'double' as identifier (shouldn't happen, but be safe)
        var typeString = typeSyntax.ToString();
        return typeString == "double" || typeString == "Double" || typeString == "System.Double";
    }

    /// <summary>
    /// Checks if a type syntax represents 'float'
    /// </summary>
    private static bool IsFloatType(TypeSyntax? typeSyntax)
    {
        if (typeSyntax == null)
            return false;

        // Check for predefined 'float' keyword
        if (typeSyntax is PredefinedTypeSyntax predefinedType)
        {
            return predefinedType.Keyword.IsKind(SyntaxKind.FloatKeyword);
        }

        // Check for 'float' as identifier (shouldn't happen, but be safe)
        var typeString = typeSyntax.ToString();
        return typeString == "float" || typeString == "Single" || typeString == "System.Single";
    }

    private static string ReplaceFloat64ToFloat32(string text)
    {
        return text
            .Replace("Float64", "Float32")
            .Replace("float64", "float32")
            .Replace("FLOAT64", "FLOAT32");
    }
}
