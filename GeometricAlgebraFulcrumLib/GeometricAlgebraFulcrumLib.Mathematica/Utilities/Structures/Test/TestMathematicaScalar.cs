using GeometricAlgebraFulcrumLib.Mathematica.Utilities.Structures.Expression;
using GeometricAlgebraFulcrumLib.Mathematica.Utilities.Structures.ExprFactory;

namespace GeometricAlgebraFulcrumLib.Mathematica.Utilities.Structures.Test;

/// <summary>
/// Test suite for MathematicaScalar functionality
/// </summary>
public static class TestMathematicaScalar
{
    /// <summary>
    /// Gets the Mathematica interface instance
    /// </summary>
    public static MathematicaInterface Cas => TestUtils.Cas;

    /// <summary>
    /// Tests scalar construction from various sources
    /// </summary>
    public static void ConstructionTest()
    {
        TestUtils.AddTestStartingMessage("MathematicaScalar Creation Test Started.");

        try
        {
            // Test rational creation
            var s = MathematicaScalar.CreateRational(Cas, 3, 5);
            TestUtils.AddTest("Try create scalar from rational 3/5 ... ", s);

            // Test symbol creation
            s = MathematicaScalar.CreateSymbol(Cas, "x3");
            TestUtils.AddTest("Try create scalar from symbol name ... ", s);

            // Test numeric creations
            s = MathematicaScalar.Create(Cas, 5);
            TestUtils.AddTest("Try create scalar from integer 5 ... ", s);

            s = MathematicaScalar.Create(Cas, 5.0f);
            TestUtils.AddTest("Try create scalar from float 5.0f ... ", s);

            s = MathematicaScalar.Create(Cas, 5.0d);
            TestUtils.AddTest("Try create scalar from double 5.0d ... ", s);

            // Test expression creation
            var expression = Mfs.Plus["v1".ToSymbolExpr(), Mfs.Power[5.ToExpr(), "n".ToSymbolExpr()]];
            s = MathematicaScalar.Create(Cas, expression);
            TestUtils.AddTest("Try create scalar from expression object ... ", s);

            // Test text expression creation
            s = MathematicaScalar.Create(Cas, @"Pi + 5 / 3");
            TestUtils.AddTest("Try create scalar from expression text \"Pi + 5 / 3\" ... ", s);
        }
        catch (Exception ex)
        {
            TestUtils.AddTest("Error during construction test", ex.Message);
        }

        TestUtils.AddTestCompletionMessage("MathematicaScalar Creation Test Completed.");
    }

    /// <summary>
    /// Tests basic arithmetic and mathematical operations on scalars
    /// </summary>
    public static void BasicOpsTest()
    {
        TestUtils.AddTestStartingMessage("MathematicaScalar Basic Operations Test Started.");

        try
        {
            var s1 = MathematicaScalar.CreateRational(Cas, -3, 5);
            var s2 = MathematicaScalar.CreateRational(Cas, 9, 5);
            var s3 = MathematicaScalar.Create(Cas, "Pi");

            // Test arithmetic operations
            TestArithmeticOperations(s1, s2, s3);

            // Test mathematical functions
            TestMathematicalFunctions(s1, s2);

            // Test differentiation
            TestDifferentiation();
        }
        catch (Exception ex)
        {
            TestUtils.AddTest("Error during basic operations test", ex.Message);
        }

        TestUtils.AddTestCompletionMessage("MathematicaScalar Basic Operations Test Completed.");
    }

    /// <summary>
    /// Tests arithmetic operations between scalars
    /// </summary>
    private static void TestArithmeticOperations(MathematicaScalar s1, MathematicaScalar s2, MathematicaScalar s3)
    {
        TestUtils.AddTest("Try negate rational -3/5 ... ", -s1);
        TestUtils.AddTest("Try add rationals -3/5 and 9/5 ... ", s1 + s2);
        TestUtils.AddTest("Try subtract rationals -3/5 and 9/5 ... ", s1 - s2);
        TestUtils.AddTest("Try multiply rationals -3/5 and 9/5 ... ", s1 * s2);
        TestUtils.AddTest("Try divide rationals -3/5 and 9/5 ... ", s1 / s2);
        TestUtils.AddTest("Try raise rational 9/5 to the power of Pi ... ", s2 ^ s3);
    }

    /// <summary>
    /// Tests mathematical functions applied to scalars
    /// </summary>
    private static void TestMathematicalFunctions(MathematicaScalar s1, MathematicaScalar s2)
    {
        // Basic functions
        TestUtils.AddTest("Try apply Abs to rational -3/5 ... ", s1.Abs());
        TestUtils.AddTest("Try apply Sqrt to rational -3/5 ... ", s1.Sqrt());
        TestUtils.AddTest("Try apply Exp to rational 9/5 ... ", s2.Exp());

        // Trigonometric functions
        TestUtils.AddTest("Try apply Sin to rational 9/5 ... ", s2.Sin());
        TestUtils.AddTest("Try apply Cos to rational 9/5 ... ", s2.Cos());
        TestUtils.AddTest("Try apply Tan to rational 9/5 ... ", s2.Tan());

        // Hyperbolic functions
        TestUtils.AddTest("Try apply Sinh to rational 9/5 ... ", s2.Sinh());
        TestUtils.AddTest("Try apply Cosh to rational 9/5 ... ", s2.Cosh());
        TestUtils.AddTest("Try apply Tanh to rational 9/5 ... ", s2.Tanh());

        // Logarithmic functions
        TestUtils.AddTest("Try apply Log to rational 9/5 ... ", s2.Log());
        TestUtils.AddTest("Try apply Log10 to rational 9/5 ... ", s2.Log10());
        TestUtils.AddTest("Try apply Log2 to rational 9/5 ... ", s2.Log2());
    }

    /// <summary>
    /// Tests differentiation operations
    /// </summary>
    private static void TestDifferentiation()
    {
        var s = MathematicaScalar.Create(Cas, @"3 x ^ 2 + 2 Pi ^ x - Sin[x]");
        var d = MathematicaScalar.CreateSymbol(Cas, "x");
        TestUtils.AddTest("Try differentiate 3 x ^ 2 + 2 Pi ^ x - Sin[x] w.r.t. x ... ", s.DiffBy(d));
    }

    /// <summary>
    /// Tests boolean operations and comparisons on scalars
    /// </summary>
    public static void IsOpsTest()
    {
        TestUtils.AddTestStartingMessage("MathematicaScalar 'Is' Operations Test Started.");

        try
        {
            // Test zero checking operations
            TestZeroOperations();

            // Test scalar comparison operations
            TestScalarComparisons();

            // Test constant checking operations
            TestConstantOperations();
        }
        catch (Exception ex)
        {
            TestUtils.AddTest("Error during 'Is' operations test", ex.Message);
        }

        TestUtils.AddTestCompletionMessage("MathematicaScalar 'Is' Operations Test Completed.");
    }

    /// <summary>
    /// Tests operations related to zero checking
    /// </summary>
    private static void TestZeroOperations()
    {
        var testCases = new[]
        {
            (MathematicaScalar.Create(Cas, 0.0f), "0.0f"),
            (MathematicaScalar.CreateSymbol(Cas, "Pi"), "Pi"),
            (MathematicaScalar.Create(Cas, @"x - x"), "(x - x)"),
            (MathematicaScalar.Create(Cas, @"3 x ^ 2 + 2 Pi ^ x - Sin[x]"), "3 x ^ 2 + 2 Pi ^ x - Sin[x]")
        };

        foreach (var (scalar, description) in testCases)
        {
            TestUtils.AddTest($"Try apply IsPossibleZero() to {description} ... ", scalar.IsPossibleZero());
            TestUtils.AddTest($"Try apply IsEqualZero() to {description} ... ", scalar.IsEqualZero());
        }
    }

    /// <summary>
    /// Tests scalar comparison operations
    /// </summary>
    private static void TestScalarComparisons()
    {
        var testCases = new[]
        {
            (MathematicaScalar.Create(Cas, 5.0f), MathematicaScalar.Create(Cas, 5), "5.0f", "5"),
            (MathematicaScalar.Create(Cas, 5.1f), MathematicaScalar.Create(Cas, 5), "5.1f", "5"),
            (MathematicaScalar.Create(Cas, 1.0f), MathematicaScalar.Create(Cas, "Sin[Pi]"), "1.0f", "Sin[Pi]"),
            (MathematicaScalar.Create(Cas, -1.0f), MathematicaScalar.Create(Cas, "Cos[Pi]"), "-1.0f", "Cos[Pi]")
        };

        foreach (var (s1, s2, desc1, desc2) in testCases)
        {
            TestUtils.AddTest($"Try apply IsPossibleScalar() to {desc1} with {desc2} ... ", s1.IsPossibleScalar(s2));
            TestUtils.AddTest($"Try apply IsEqualScalar() to {desc1} with {desc2} ... ", s1.IsEqualScalar(s2));
        }
    }

    /// <summary>
    /// Tests operations related to constant checking
    /// </summary>
    private static void TestConstantOperations()
    {
        var constantTestCases = new[]
        {
            (MathematicaScalar.Create(Cas, -1.0f), "-1.0f"),
            (MathematicaScalar.Create(Cas, "Sin[Pi]"), "Sin[Pi]"),
            (MathematicaScalar.Create(Cas, "Sin[x]"), "Sin[x]"),
            (MathematicaScalar.Create(Cas, "Sin[x - x]"), "Sin[x - x]")
        };

        foreach (var (scalar, description) in constantTestCases)
        {
            TestUtils.AddTest($"Try apply IsConstant() to {description} ... ", scalar.IsConstant());
        }

        var nonZeroRealTestCases = new[]
        {
            (MathematicaScalar.Create(Cas, 0.0f), "0.0f"),
            (MathematicaScalar.Create(Cas, -1.0f), "-1.0f"),
            (MathematicaScalar.Create(Cas, "Sin[Pi]"), "Sin[Pi]"),
            (MathematicaScalar.Create(Cas, "Sin[x]"), "Sin[x]"),
            (MathematicaScalar.Create(Cas, "Sin[x - x]"), "Sin[x - x]"),
            (MathematicaScalar.Create(Cas, "3 + i * Sin[Pi / 2]"), "3 + i * Sin[Pi / 2]")
        };

        foreach (var (scalar, description) in nonZeroRealTestCases)
        {
            TestUtils.AddTest($"Try apply IsNonZeroRealConstant() to {description} ... ", scalar.IsNonZeroRealConstant());
        }
    }
}