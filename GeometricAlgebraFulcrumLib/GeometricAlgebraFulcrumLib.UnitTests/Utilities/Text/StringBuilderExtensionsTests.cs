using System.Numerics;
using System.Text;
using GeometricAlgebraFulcrumLib.Utilities.Text;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Utilities.Text;

/// <summary>
/// Tests for StringBuilderExtensions - StringBuilder extensions for Complex numbers
/// NOTE: The parameterless Append(Complex) method is shadowed by .NET's built-in method,
/// so we primarily test the format-string overloads and AppendComplexNumber methods.
/// </summary>
[TestFixture]
public class StringBuilderExtensionsTests
{
    #region Append Complex with Format Tests

    [Test]
    public void Append_WithFormat_RealOnly_ShouldFormatCorrectly()
    {
        var sb = new StringBuilder();
        var number = new Complex(5.123456, 0);

        sb.Append(number, "F2");

        Assert.That(sb.ToString(), Is.EqualTo("5,12")); // German locale uses comma
    }

    [Test]
    public void Append_WithFormat_ImaginaryOnly_ShouldFormatCorrectly()
    {
        var sb = new StringBuilder();
        var number = new Complex(0, 3.789012);

        sb.Append(number, "F2");

        Assert.That(sb.ToString(), Is.EqualTo("3,79 i")); // German locale uses comma
    }

    [Test]
    public void Append_WithFormat_BothParts_ShouldFormatCorrectly()
    {
        var sb = new StringBuilder();
        var number = new Complex(5.123456, 3.789012);

        sb.Append(number, "F2");

        var result = sb.ToString();
        Assert.That(result, Does.Contain("5,12")); // German locale
        Assert.That(result, Does.Contain("3,79")); // German locale
        Assert.That(result, Does.Contain(" + ")); // Operator between parts
        Assert.That(result, Does.Contain("i"));
    }

    [Test]
    public void Append_WithFormatG_ShouldUseGeneralFormat()
    {
        var sb = new StringBuilder();
        var number = new Complex(5.0, 3.0);

        sb.Append(number, "G");

        var result = sb.ToString();
        Assert.That(result, Does.Contain("5"));
        Assert.That(result, Does.Contain("3"));
        Assert.That(result, Does.Contain("i"));
    }

    [Test]
    public void Append_WithFormat_Zero_ShouldAppendNothing()
    {
        var sb = new StringBuilder();
        var number = Complex.Zero;

        sb.Append(number, "G");

        Assert.That(sb.ToString(), Is.EqualTo(string.Empty));
    }

    [Test]
    public void Append_WithFormat_NegativeImaginary_ShouldFormatCorrectly()
    {
        var sb = new StringBuilder();
        var number = new Complex(5.0, -3.0);

        sb.Append(number, "G");

        var result = sb.ToString();
        Assert.That(result, Does.Contain("5"));
        Assert.That(result, Does.Contain(" - ")); // Minus operator
        Assert.That(result, Does.Contain("3")); // Absolute value
        Assert.That(result, Does.Contain("i"));
    }

    #endregion

    #region AppendLine Complex Tests

    [Test]
    public void AppendLine_WithFormat_ShouldFormatAndAddNewline()
    {
        var sb = new StringBuilder();
        var number = new Complex(5.123, 3.789);

        sb.AppendLine(number, "F1");

        var result = sb.ToString();
        Assert.That(result, Does.Contain("5,1")); // German locale
        Assert.That(result, Does.Contain("3,8")); // German locale
        Assert.That(result, Does.Contain(" + ")); // Operator
        Assert.That(result, Does.EndWith(System.Environment.NewLine));
    }

    [Test]
    public void AppendLine_RealOnly_ShouldAppendWithNewline()
    {
        var sb = new StringBuilder();
        var number = new Complex(5.0, 0);

        sb.AppendLine(number, "G");

        var result = sb.ToString();
        Assert.That(result, Does.Contain("5"));
        Assert.That(result, Does.Not.Contain("i"));
        Assert.That(result, Does.EndWith(System.Environment.NewLine));
    }

    #endregion

    #region AppendComplexNumber Tests

    [Test]
    public void AppendComplexNumber_RealAndImaginary_ShouldAppend()
    {
        var sb = new StringBuilder();

        sb.AppendComplexNumber(5.0, 3.0);

        var result = sb.ToString();
        Assert.That(result, Does.Contain("5"));
        Assert.That(result, Does.Contain("3"));
        Assert.That(result, Does.Contain("i"));
    }

    [Test]
    public void AppendComplexNumber_RealOnly_ShouldAppendRealPart()
    {
        var sb = new StringBuilder();

        sb.AppendComplexNumber(5.0, 0);

        Assert.That(sb.ToString(), Is.EqualTo("5"));
    }

    [Test]
    public void AppendComplexNumber_ImaginaryOnly_ShouldAppendImaginaryPart()
    {
        var sb = new StringBuilder();

        sb.AppendComplexNumber(0, 3.0);

        Assert.That(sb.ToString(), Is.EqualTo("3 i"));
    }

    [Test]
    public void AppendComplexNumber_WithFormat_ShouldFormatNumbers()
    {
        var sb = new StringBuilder();

        sb.AppendComplexNumber(5.123, 3.789, "F1");

        var result = sb.ToString();
        Assert.That(result, Does.Contain("5,1")); // German locale
        Assert.That(result, Does.Contain("3,8")); // German locale
        Assert.That(result, Does.Contain(" + ")); // Operator
        Assert.That(result, Does.Contain("i"));
    }

    [Test]
    public void AppendComplexNumber_ZeroZero_ShouldAppendNothing()
    {
        var sb = new StringBuilder();

        sb.AppendComplexNumber(0, 0);

        Assert.That(sb.ToString(), Is.EqualTo(string.Empty));
    }

    #endregion

    #region Chaining Tests

    [Test]
    public void AppendComplexNumber_ShouldSupportChaining()
    {
        var sb = new StringBuilder();

        sb.AppendComplexNumber(1.0, 2.0)
          .Append(" + ")
          .AppendComplexNumber(3.0, 4.0);

        var result = sb.ToString();
        Assert.That(result, Does.Contain("1"));
        Assert.That(result, Does.Contain("2"));
        Assert.That(result, Does.Contain("3"));
        Assert.That(result, Does.Contain("4"));
    }

    [Test]
    public void Append_WithFormat_ShouldSupportChaining()
    {
        var sb = new StringBuilder();
        var number1 = new Complex(1.5, 2.5);
        var number2 = new Complex(3.5, 4.5);

        sb.Append(number1, "F1").Append(" ; ").Append(number2, "F1");

        var result = sb.ToString();
        Assert.That(result, Does.Contain("1,5")); // German locale
        Assert.That(result, Does.Contain("2,5")); // German locale
        Assert.That(result, Does.Contain("3,5")); // German locale
        Assert.That(result, Does.Contain("4,5")); // German locale
        Assert.That(result, Does.Contain(";")); // Separator between complex numbers
    }

    #endregion
}
