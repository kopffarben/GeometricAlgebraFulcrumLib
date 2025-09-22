using GeometricAlgebraFulcrumLib.Mathematica.Utilities.Structures.Expression;
using GeometricAlgebraFulcrumLib.Utilities.Text.Text.Linear;

namespace GeometricAlgebraFulcrumLib.Mathematica.Utilities.Structures.Test;

/// <summary>
/// Utility class providing common functionality for Mathematica structure testing
/// </summary>
public static class TestUtils
{
    private const int DefaultPrecision = 10;

    /// <summary>
    /// Linear text composer for logging test results
    /// </summary>
    public static readonly LinearTextComposer Log = new();

    /// <summary>
    /// Mathematica interface for symbolic computation
    /// </summary>
    public static readonly MathematicaInterface Cas = MathematicaInterface.Create();

    static TestUtils()
    {
        Log.AddStopWatchHeader();
    }

    /// <summary>
    /// Adds a formatted starting message for a test
    /// </summary>
    /// <param name="text">The test description</param>
    public static void AddTestStartingMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Test message cannot be null or empty", nameof(text));

        Log.AppendLineAtNewLine("".PadLeft(80, '='));
        Log.AppendLine(text);
        Log.AppendLine();
    }

    /// <summary>
    /// Adds a formatted completion message for a test
    /// </summary>
    /// <param name="text">The completion message</param>
    public static void AddTestCompletionMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Completion message cannot be null or empty", nameof(text));

        Log.AppendLineAtNewLine(text);
        Log.AppendLine("".PadLeft(80, '='));
    }

    /// <summary>
    /// Creates a descriptive string representation of a scalar with default precision
    /// </summary>
    /// <param name="scalar">The scalar to describe</param>
    /// <returns>Formatted string representation</returns>
    public static string DescribeScalar(MathematicaScalar scalar)
    {
        return DescribeScalar(scalar, DefaultPrecision);
    }

    /// <summary>
    /// Creates a descriptive string representation of a scalar with specified precision
    /// </summary>
    /// <param name="scalar">The scalar to describe</param>
    /// <param name="precision">Numerical precision for evaluation</param>
    /// <returns>Formatted string representation</returns>
    public static string DescribeScalar(MathematicaScalar scalar, int precision)
    {
        if (scalar == null)
            throw new ArgumentNullException(nameof(scalar));
        if (precision < 1)
            throw new ArgumentException("Precision must be positive", nameof(precision));

        return $"{scalar} = ({scalar.N(precision)})";
    }

    /// <summary>
    /// Creates a descriptive string representation of a vector
    /// </summary>
    /// <param name="vector">The vector to describe</param>
    /// <returns>String representation of the vector</returns>
    public static string DescribeVector(MathematicaVector vector)
    {
        if (vector == null)
            throw new ArgumentNullException(nameof(vector));

        return vector.ToString();
    }

    /// <summary>
    /// Creates a descriptive string representation of a matrix
    /// </summary>
    /// <param name="matrix">The matrix to describe</param>
    /// <returns>String representation of the matrix</returns>
    public static string DescribeMatrix(MathematicaMatrix matrix)
    {
        if (matrix == null)
            throw new ArgumentNullException(nameof(matrix));

        return matrix.ToString();
    }

    /// <summary>
    /// Adds a test result for a scalar to the log
    /// </summary>
    /// <param name="message">Test description</param>
    /// <param name="result">Scalar result</param>
    public static void AddTest(string message, MathematicaScalar result)
    {
        ValidateTestParameters(message, result);
        
        Log.AppendAtNewLine(message);
        Log.AppendLine(DescribeScalar(result));
        Log.AppendLine();
    }

    /// <summary>
    /// Adds a test result for a vector to the log
    /// </summary>
    /// <param name="message">Test description</param>
    /// <param name="result">Vector result</param>
    public static void AddTest(string message, MathematicaVector result)
    {
        ValidateTestParameters(message, result);
        
        Log.AppendAtNewLine(message);
        Log.AppendLine(DescribeVector(result));
        Log.AppendLine();
    }

    /// <summary>
    /// Adds a test result for a matrix to the log
    /// </summary>
    /// <param name="message">Test description</param>
    /// <param name="result">Matrix result</param>
    public static void AddTest(string message, MathematicaMatrix result)
    {
        ValidateTestParameters(message, result);
        
        Log.AppendAtNewLine(message);
        Log.AppendLine(DescribeMatrix(result));
        Log.AppendLine();
    }

    /// <summary>
    /// Adds a test result for a generic object to the log
    /// </summary>
    /// <typeparam name="T">Type of the result object</typeparam>
    /// <param name="message">Test description</param>
    /// <param name="result">Generic result</param>
    public static void AddTest<T>(string message, T result)
    {
        ValidateTestParameters(message, result);
        
        Log.AppendAtNewLine(message);
        Log.AppendLine(result?.ToString() ?? "null");
        Log.AppendLine();
    }

    /// <summary>
    /// Validates common test parameters
    /// </summary>
    /// <param name="message">Test message to validate</param>
    /// <param name="result">Result object to validate</param>
    private static void ValidateTestParameters(string message, object? result)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Test message cannot be null or empty", nameof(message));
        
        // Note: result can be null, which is a valid test case
    }

    /// <summary>
    /// Clears the log content
    /// </summary>
    public static void ClearLog()
    {
        Log.Clear();
    }

    /// <summary>
    /// Gets the current log content
    /// </summary>
    /// <returns>Current log content as string</returns>
    public static string GetLogContent()
    {
        return Log.ToString();
    }
}