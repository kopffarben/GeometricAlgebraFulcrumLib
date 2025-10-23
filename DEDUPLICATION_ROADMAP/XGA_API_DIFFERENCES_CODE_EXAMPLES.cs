/*
 * XGa API Differences: Float64 vs Generic - Code Examples
 *
 * This file demonstrates the key API differences between
 * XGaFloat64 and XGa<T> implementations with concrete examples.
 */

using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Float64.Processors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Multivectors;
using GeometricAlgebraFulcrumLib.Algebra.GeometricAlgebra.Generic.Processors;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Float64;
using GeometricAlgebraFulcrumLib.Algebra.Scalars.Generic;

namespace GeometricAlgebraFulcrumLib.Examples;

/// <summary>
/// Example 1: Composer API Differences
/// Generic is MUCH more user-friendly with multiple overloads
/// </summary>
public static class ComposerExample
{
    public static void Float64ComposerExample()
    {
        var processor = XGaFloat64Processor.Euclidean;

        // Float64: Must explicitly cast to double
        var vector = processor
            .CreateVectorComposer()
            .SetVectorTerm(0, 5.0)      // Must be double - no int overload
            .SetVectorTerm(1, 2.5)
            .SetVectorTerm(2, 3.0)      // Must be double - no int overload
            .GetVector();

        // ❌ These DON'T work in Float64:
        // .SetVectorTerm(0, 5)                          // No int overload
        // .SetVectorTerm(1, new Float64Scalar(2.5))     // No Float64Scalar overload
        // .SetVectorTerm(2, "3/7")                      // No string overload (symbolic)
    }

    public static void GenericComposerExample<T>()
    {
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

        // Generic: Multiple convenient overloads
        var vector = processor
            .CreateVectorComposer()
            .SetVectorTerm(0, 5)        // ✅ int works!
            .SetVectorTerm(1, 2.5)      // ✅ double works!
            .SetVectorTerm(2, 3.0)      // ✅ double works!
            .GetVector();

        // ✅ These ALSO work in Generic:
        var vector2 = processor
            .CreateVectorComposer()
            .SetVectorTerm(0, scalarProcessor.ScalarFromNumber(5))       // Scalar<T>
            .SetVectorTerm(1, 2.5)
            .GetVector();

        // For symbolic/meta-programming:
        // .SetVectorTerm(2, "3/7")  // ✅ string works (if processor supports)
    }

    /// <summary>
    /// Generic ALSO has SetTrivectorTerm which Float64 lacks completely!
    /// </summary>
    public static void GenericTrivectorExample()
    {
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

        // ✅ Generic has SetTrivectorTerm
        var multivector = processor
            .CreateMultivectorComposer()
            .SetTrivectorTerm(0, 1, 2, 1.0)     // e_0 ∧ e_1 ∧ e_2
            .SetTrivectorTerm(1, 2, 3, 0.5)     // e_1 ∧ e_2 ∧ e_3
            .GetMultivector();
    }

    public static void Float64NoTrivectorExample()
    {
        var processor = XGaFloat64Processor.Euclidean;

        // ❌ Float64 does NOT have SetTrivectorTerm
        // Must use generic SetTerm method:
        var multivector = processor
            .CreateMultivectorComposer()
            .SetTerm(new[] { 0, 1, 2 }, 1.0)    // More cumbersome
            .SetTerm(new[] { 1, 2, 3 }, 0.5)
            .GetMultivector();
    }
}

/// <summary>
/// Example 2: MapScalars API - CRITICAL DIFFERENCE!
/// Generic has full MapScalars family, Float64 has NONE
/// </summary>
public static class MapScalarsExample
{
    public static void GenericMapScalarsExample()
    {
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

        var vector = processor.Vector(1.5, -2.3, 3.7);

        // ✅ Generic can map scalars easily:
        var absVector = vector.MapScalars(x => Math.Abs(x));
        var scaledVector = vector.MapScalars(x => x * 2.0);
        var roundedVector = vector.MapScalars(x => Math.Round(x));

        // ✅ Can also use IndexSet in mapping:
        var indexMappedVector = vector.MapScalars((id, x) =>
            id.FirstIndex == 0 ? x * 2 : x
        );

        // ✅ Vector-specific: Can use int index directly
        var vectorMappedVector = vector.MapScalars((index, x) =>
            index == 0 ? x * 2 : x
        );

        // ✅ Can remap basis vectors:
        var remappedVector = vector.MapBasisVectors(index => index + 1);

        // ✅ Can map to different processor/type:
        // var rationalVector = vector.MapScalars(
        //     rationalProcessor,
        //     x => ERational.Create((int)x, 1)
        // );
    }

    public static void Float64NoMapScalarsExample()
    {
        var processor = XGaFloat64Processor.Euclidean;
        var vector = processor.Vector(1.5, -2.3, 3.7);

        // ❌ Float64 does NOT have MapScalars!
        // Must manually rebuild using composer:
        var absVector = processor
            .CreateVectorComposer()
            .AddTerms(vector.IdScalarPairs.Select(p =>
                new KeyValuePair<IndexSet, double>(p.Key, Math.Abs(p.Value))
            ))
            .GetVector();

        // This is much more cumbersome and error-prone!
    }
}

/// <summary>
/// Example 3: Times/Divide Overloads
/// Generic has many more overloads than Float64
/// </summary>
public static class TimesExample
{
    public static void Float64TimesExample()
    {
        var processor = XGaFloat64Processor.Euclidean;
        var vector = processor.Vector(1, 2, 3);

        // ✅ Float64 has:
        var result1 = vector.Times(2.0);  // double

        // ❌ Float64 does NOT have:
        // var result2 = vector.Times(new Float64Scalar(2.0));  // No Float64Scalar overload
        // var result3 = vector.Divide(new Float64Scalar(2.0)); // No Float64Scalar overload
    }

    public static void GenericTimesExample()
    {
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);
        var vector = processor.Vector(1, 2, 3);

        // ✅ Generic has many overloads:
        var result1 = vector.Times(2.0);                              // double
        var result2 = vector.Times(2);                                // int
        var result3 = vector.Times(scalarProcessor.ScalarFromNumber(2)); // Scalar<T>
        var result4 = vector.Divide(scalarProcessor.ScalarFromNumber(2)); // Scalar<T>

        // Much more flexible!
    }
}

/// <summary>
/// Example 4: Operator Overloads
/// Both have extensive operators, but with type differences
/// </summary>
public static class OperatorExample
{
    public static void Float64OperatorExample()
    {
        var processor = XGaFloat64Processor.Euclidean;
        var v1 = processor.Vector(1, 2, 3);
        var v2 = processor.Vector(4, 5, 6);

        // ✅ Float64 operators work with primitives:
        var result1 = v1 + v2;
        var result2 = v1 * 2.0;              // double
        var result3 = 2.0 * v1;              // double
        var result4 = v1 / 2.0;              // double

        // ✅ Float64 has implicit double conversion for scalar:
        var scalar = processor.Scalar(5.0);
        double primitiveValue = scalar;      // implicit conversion

        // ✅ Comparison operators:
        bool comparison = scalar > 3.0;
        bool equality = scalar == 5.0;
    }

    public static void GenericOperatorExample()
    {
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);
        var v1 = processor.Vector(1, 2, 3);
        var v2 = processor.Vector(4, 5, 6);

        // ✅ Generic operators work with many types:
        var result1 = v1 + v2;
        var result2 = v1 * 2.0;              // double
        var result3 = v1 * 2;                // int
        var result4 = v1 * scalarProcessor.ScalarFromNumber(2); // Scalar<T>

        // ❌ Generic does NOT have implicit T conversion (commented out):
        var scalar = processor.Scalar(5.0);
        // double primitiveValue = scalar;   // NO implicit conversion
        double primitiveValue = scalar.ScalarValue; // Must use .ScalarValue

        // ✅ Comparison operators use ScalarProcessor:
        var scalarDouble = processor.Scalar(5.0);
        bool comparison = scalarDouble > 3.0;  // Uses processor subtraction + IsPositive()
        bool equality = scalarDouble == 5.0;   // Uses processor subtraction + IsZero()
    }
}

/// <summary>
/// Example 5: Utils/Conversions
/// Float64 has MANY utilities, Generic has almost NONE
/// </summary>
public static class UtilsExample
{
    public static void Float64UtilsExample()
    {
        var processor = XGaFloat64Processor.Euclidean;

        // ✅ Float64 has extensive conversions:

        // From IEnumerable<double>:
        var scalarList = new[] { 1.0, 2.0, 3.0 };
        var vector1 = scalarList.CreateXGaVector(processor);

        // From LinVector2D/3D/4D:
        var linVector3D = LinVector3D.Create(1, 2, 3);
        var vector2 = linVector3D.ToXGaVector(processor);

        // Back to LinVector3D:
        var backToLin = vector2.ToLinVector3D();

        // From MathNet.Numerics:
        // var mathNetVector = Vector.Build.Dense(new[] { 1.0, 2.0, 3.0 });
        // var vector3 = mathNetVector.ToXGaVector(processor);

        // Geometric constructors:
        var unitVector = 45.0.DegreesToRadians().CreateUnitXGaFloat64Vector(0, 1);
        var phasor = 30.0.DegreesToRadians().CreateXGaPhasor(2.5, 0, 1);
    }

    public static void GenericUtilsExample()
    {
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);

        // ❌ Generic has almost NO utilities!

        // ❌ No CreateXGaVector from IEnumerable<T>
        // ❌ No LinVector conversions
        // ❌ No MathNet.Numerics conversions
        // ❌ No geometric constructors (unit vectors, phasors)

        // Only has:
        // - Outermorphism mapping (OmMap)
        // That's basically it!

        // Must manually create vectors:
        var vector = processor
            .CreateVectorComposer()
            .SetVectorTerm(0, 1.0)
            .SetVectorTerm(1, 2.0)
            .SetVectorTerm(2, 3.0)
            .GetVector();
    }
}

/// <summary>
/// Example 6: Return Type Differences
/// Float64 returns primitives, Generic returns wrapped types
/// </summary>
public static class ReturnTypeExample
{
    public static void Float64ReturnTypes()
    {
        var processor = XGaFloat64Processor.Euclidean;
        var scalar = processor.Scalar(5.0);
        var vector = processor.Vector(1, 2, 3);

        // Float64 returns primitive double:
        double scalarValue = scalar.Scalar();           // returns double
        double normValue = vector.ENorm().ScalarValue;  // ENorm() returns Float64Scalar
        double norm = vector.Norm().ScalarValue;        // Norm() returns Float64Scalar
    }

    public static void GenericReturnTypes()
    {
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);
        var scalar = processor.Scalar(5.0);
        var vector = processor.Vector(1, 2, 3);

        // Generic returns Scalar<T>:
        Scalar<double> scalarValue = scalar.Scalar();          // returns Scalar<T>
        Scalar<double> normValue = vector.ENorm();             // returns Scalar<T>
        Scalar<double> norm = vector.Norm();                   // returns Scalar<T>

        // To get primitive value:
        double primitiveValue = scalarValue.ScalarValue;
        double primitiveNorm = normValue.ScalarValue;
    }
}

/// <summary>
/// Example 7: Product Operations - Identical in Structure
/// Both versions have the same products, just with type differences
/// </summary>
public static class ProductExample
{
    public static void Float64ProductsExample()
    {
        var processor = XGaFloat64Processor.Euclidean;
        var v1 = processor.Vector(1, 2, 3);
        var v2 = processor.Vector(4, 5, 6);

        // ✅ All products work identically:
        var gp = v1.Gp(v2);     // Geometric product
        var op = v1.Op(v2);     // Outer product (wedge)
        var sp = v1.Sp(v2);     // Scalar product
        var lcp = v1.Lcp(v2);   // Left contraction
        var rcp = v1.Rcp(v2);   // Right contraction
        var cp = v1.Cp(v2);     // Commutator product
        var acp = v1.Acp(v2);   // Anti-commutator product
        var hip = v1.Hip(v2);   // Hestenes inner product
        var fdp = v1.Fdp(v2);   // Fat-dot product
    }

    public static void GenericProductsExample()
    {
        var scalarProcessor = ScalarProcessorOfFloat64.Instance;
        var processor = XGaProcessor<double>.CreateEuclidean(scalarProcessor);
        var v1 = processor.Vector(1, 2, 3);
        var v2 = processor.Vector(4, 5, 6);

        // ✅ All products work identically (only return types differ):
        var gp = v1.Gp(v2);     // Geometric product
        var op = v1.Op(v2);     // Outer product (wedge)
        var sp = v1.Sp(v2);     // Scalar product
        var lcp = v1.Lcp(v2);   // Left contraction
        var rcp = v1.Rcp(v2);   // Right contraction
        var cp = v1.Cp(v2);     // Commutator product
        var acp = v1.Acp(v2);   // Anti-commutator product
        var hip = v1.Hip(v2);   // Hestenes inner product
        var fdp = v1.Fdp(v2);   // Fat-dot product

        // Return types are XGaMultivector<T> instead of XGaFloat64Multivector
    }
}

/// <summary>
/// Summary of Key Differences
/// </summary>
public static class Summary
{
    /*
     * CRITICAL DIFFERENCES:
     *
     * 1. MapScalars Family - ❌ FEHLT in Float64
     *    - Generic: Full MapScalars API for transformations
     *    - Float64: KEINE MapScalars Methoden
     *    Impact: Float64 kann Skalare nicht flexibel transformieren
     *
     * 2. Composer Overloads - Generic ist VIEL benutzerfreundlicher
     *    - Generic: 7-8 Überladungen pro Methode (int, long, float, double, string, T, Scalar<T>, IScalar<T>)
     *    - Float64: Nur double Überladungen
     *    - Generic hat SetTrivectorTerm, Float64 NICHT
     *    Impact: Generic ist viel angenehmer zu nutzen
     *
     * 3. Utils/Conversions - Float64 ist VIEL praktischer
     *    - Float64: Viele Konvertierungen (LinVector, MathNet.Numerics, etc.)
     *    - Generic: Fast KEINE Konvertierungen
     *    Impact: Generic ist schwer für praktische Anwendungen nutzbar
     *
     * 4. Times/Divide Overloads
     *    - Generic: Viele Überladungen (int, double, T, Scalar<T>, IScalar<T>)
     *    - Float64: Nur double
     *    Impact: Generic ist flexibler
     *
     * 5. Return Types
     *    - Float64: Gibt primitive double zurück
     *    - Generic: Gibt Scalar<T> zurück
     *    Impact: APIs sind nicht direkt austauschbar
     *
     * IDENTISCHE ASPEKTE:
     *
     * ✅ Core Multivector API (Negative, Reverse, Inverse, etc.)
     * ✅ Product Operations (Gp, Op, Sp, Lcp, Rcp, etc.)
     * ✅ Part Extraction (GetScalarPart, GetVectorPart, etc.)
     * ✅ Storage Types (Uniform, Graded, specialized types)
     *
     * EMPFEHLUNGEN:
     *
     * 1. Float64 BRAUCHT MapScalars API (kritisch)
     * 2. Generic BRAUCHT mehr Utils (kritisch für praktische Nutzung)
     * 3. Float64 Composers sollten mehr Überladungen haben (Benutzerfreundlichkeit)
     * 4. Dokumentation der Unterschiede verbessern
     */
}
