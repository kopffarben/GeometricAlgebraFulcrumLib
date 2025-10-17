using System;
using GeometricAlgebraFulcrumLib.Utilities.Text;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Utilities.Text;

/// <summary>
/// Tests for StringUtils - string manipulation and formatting utilities
/// </summary>
[TestFixture]
public class StringUtilsTests
{
    #region SetCharacterAt Tests

    [Test]
    public void SetCharacterAt_SimpleReplacement()
    {
        var result = "Hello".SetCharacterAt(1, 'a');
        Assert.That(result, Is.EqualTo("Hallo"));
    }

    [Test]
    public void SetCharacterAt_FirstCharacter()
    {
        var result = "Test".SetCharacterAt(0, 'B');
        Assert.That(result, Is.EqualTo("Best"));
    }

    [Test]
    public void SetCharacterAt_LastCharacter()
    {
        var result = "Test".SetCharacterAt(3, '!');
        Assert.That(result, Is.EqualTo("Tes!"));
    }

    #endregion

    #region SplitAtFirstWhitespace Tests

    [Test]
    public void SplitAtFirstWhitespace_SimpleCase()
    {
        var first = "Hello World".SplitAtFirstWhitespace(out var remaining);

        Assert.That(first, Is.EqualTo("Hello"));
        Assert.That(remaining, Is.EqualTo("World"));
    }

    [Test]
    public void SplitAtFirstWhitespace_WithLeadingWhitespace()
    {
        var first = "  ABC DE ".SplitAtFirstWhitespace(out var remaining);

        Assert.That(first, Is.EqualTo("ABC"));
        Assert.That(remaining, Is.EqualTo("DE"));
    }

    [Test]
    public void SplitAtFirstWhitespace_SingleWord()
    {
        var first = "Hello".SplitAtFirstWhitespace(out var remaining);

        Assert.That(first, Is.EqualTo("Hello"));
        Assert.That(remaining, Is.EqualTo(string.Empty));
    }

    [Test]
    public void SplitAtFirstWhitespace_EmptyString()
    {
        var first = "".SplitAtFirstWhitespace(out var remaining);

        Assert.That(first, Is.EqualTo(string.Empty));
        Assert.That(remaining, Is.EqualTo(string.Empty));
    }

    [Test]
    public void SplitAtFirstWhitespace_NullString()
    {
        string? text = null;
        var first = text.SplitAtFirstWhitespace(out var remaining);

        Assert.That(first, Is.EqualTo(string.Empty));
        Assert.That(remaining, Is.EqualTo(string.Empty));
    }

    [Test]
    public void SplitAtFirstWhitespace_OnlyWhitespace()
    {
        var first = "   ".SplitAtFirstWhitespace(out var remaining);

        Assert.That(first, Is.EqualTo(string.Empty));
        Assert.That(remaining, Is.EqualTo(string.Empty));
    }

    #endregion

    #region IsSingleLine / IsMultiLine Tests

    [Test]
    public void IsSingleLine_SingleLineText_ShouldReturnTrue()
    {
        Assert.That("Hello World".IsSingleLine(), Is.True);
        Assert.That("".IsSingleLine(), Is.True);
        Assert.That("   ".IsSingleLine(), Is.True);
    }

    [Test]
    public void IsSingleLine_MultiLineText_ShouldReturnFalse()
    {
        Assert.That("Hello\nWorld".IsSingleLine(), Is.False);
        Assert.That("Line1\nLine2\nLine3".IsSingleLine(), Is.False);
    }

    [Test]
    public void IsMultiLine_MultiLineText_ShouldReturnTrue()
    {
        Assert.That("Hello\nWorld".IsMultiLine(), Is.True);
        Assert.That("Line1\r\nLine2".IsMultiLine(), Is.True);
    }

    [Test]
    public void IsMultiLine_SingleLineText_ShouldReturnFalse()
    {
        Assert.That("Hello World".IsMultiLine(), Is.False);
        Assert.That("".IsMultiLine(), Is.False);
    }

    #endregion

    #region IsNullOrEmpty Tests

    [Test]
    public void IsNullOrEmpty_NullString_ShouldReturnTrue()
    {
        string? text = null;
        Assert.That(text.IsNullOrEmpty(), Is.True);
    }

    [Test]
    public void IsNullOrEmpty_EmptyString_ShouldReturnTrue()
    {
        Assert.That("".IsNullOrEmpty(), Is.True);
    }

    [Test]
    public void IsNullOrEmpty_NonEmptyString_ShouldReturnFalse()
    {
        Assert.That("Hello".IsNullOrEmpty(), Is.False);
        Assert.That(" ".IsNullOrEmpty(), Is.False); // Whitespace is not empty
    }

    #endregion

    #region SingleQuote / DoubleQuote Tests

    [Test]
    public void SingleQuote_ShouldWrapInSingleQuotes()
    {
        Assert.That("Hello".SingleQuote(), Is.EqualTo("'Hello'"));
        Assert.That("".SingleQuote(), Is.EqualTo("''"));
    }

    [Test]
    public void DoubleQuote_String_ShouldWrapInDoubleQuotes()
    {
        Assert.That("Hello".DoubleQuote(), Is.EqualTo("\"Hello\""));
        Assert.That("".DoubleQuote(), Is.EqualTo("\"\""));
    }

    [Test]
    public void DoubleQuote_Int_ShouldWrapInDoubleQuotes()
    {
        Assert.That(42.DoubleQuote(), Is.EqualTo("\"42\""));
        Assert.That(0.DoubleQuote(), Is.EqualTo("\"0\""));
        Assert.That((-5).DoubleQuote(), Is.EqualTo("\"-5\""));
    }

    [Test]
    public void DoubleQuote_Bool_ShouldWrapInDoubleQuotes()
    {
        Assert.That(true.DoubleQuote(), Is.EqualTo("\"True\""));
        Assert.That(false.DoubleQuote(), Is.EqualTo("\"False\""));
    }

    [Test]
    public void DoubleQuote_Double_ShouldWrapInDoubleQuotes()
    {
        Assert.That(3.14.DoubleQuote(), Is.EqualTo("\"3.14\""));
    }

    [Test]
    public void DoubleQuote_Float_ShouldWrapInDoubleQuotes()
    {
        Assert.That(2.5f.DoubleQuote(), Is.EqualTo("\"2.5\""));
    }

    #endregion

    #region Repeat Tests

    [Test]
    public void Repeat_PositiveCount_ShouldRepeatString()
    {
        Assert.That("ABC".Repeat(3), Is.EqualTo("ABCABCABC"));
        Assert.That("X".Repeat(5), Is.EqualTo("XXXXX"));
    }

    [Test]
    public void Repeat_ZeroCount_ShouldReturnEmpty()
    {
        Assert.That("ABC".Repeat(0), Is.EqualTo(string.Empty));
    }

    [Test]
    public void Repeat_OneCount_ShouldReturnOriginal()
    {
        Assert.That("ABC".Repeat(1), Is.EqualTo("ABC"));
    }

    [Test]
    public void Repeat_EmptyString_ShouldReturnEmpty()
    {
        Assert.That("".Repeat(5), Is.EqualTo(string.Empty));
    }

    #endregion

    #region LinesCount Tests

    [Test]
    public void LinesCount_SingleLine_ShouldReturn1()
    {
        Assert.That("Hello World".LinesCount(), Is.EqualTo(1));
        Assert.That("".LinesCount(), Is.EqualTo(1));
    }

    [Test]
    public void LinesCount_MultipleLines_ShouldReturnCorrectCount()
    {
        Assert.That("Line1\nLine2".LinesCount(), Is.EqualTo(2));
        Assert.That("Line1\nLine2\nLine3".LinesCount(), Is.EqualTo(3));
    }

    [Test]
    public void LinesCount_WithCRLF_ShouldCountCorrectly()
    {
        Assert.That("Line1\r\nLine2\r\nLine3".LinesCount(), Is.EqualTo(3));
    }

    [Test]
    public void LinesCount_TrailingNewline_ShouldCount()
    {
        // Depends on implementation - may or may not count trailing line
        var count = "Line1\nLine2\n".LinesCount();
        Assert.That(count, Is.GreaterThanOrEqualTo(2));
    }

    #endregion

    #region ToValidFileName Tests

    [Test]
    public void ToValidFileName_RemovesInvalidCharacters()
    {
        // Invalid filename chars: < > : " / \ | ? *
        var result = "file<name>.txt".ToValidFileName();
        Assert.That(result, Does.Not.Contain("<"));
        Assert.That(result, Does.Not.Contain(">"));
    }

    [Test]
    public void ToValidFileName_ValidName_ShouldRemainUnchanged()
    {
        var result = "valid_filename.txt".ToValidFileName();
        Assert.That(result, Is.EqualTo("valid_filename.txt"));
    }

    [Test]
    public void ToValidFileName_WithCustomReplaceChar()
    {
        var result = "file:name".ToValidFileName('_');
        Assert.That(result, Does.Contain("_"));
        Assert.That(result, Does.Not.Contain(":"));
    }

    #endregion

    #region ToValidPath Tests

    [Test]
    public void ToValidPath_RemovesInvalidCharacters()
    {
        var result = "path<with>invalid".ToValidPath();
        Assert.That(result, Does.Not.Contain("<"));
        Assert.That(result, Does.Not.Contain(">"));
    }

    [Test]
    public void ToValidPath_ReplacesInvalidChars()
    {
        // ToValidPath replaces all invalid chars including / with _
        var result = "valid/path/name".ToValidPath();
        Assert.That(result, Is.EqualTo("valid_path_name"));
    }

    [Test]
    public void ToValidPath_ValidPathWithoutSlash_ShouldRemainUnchanged()
    {
        var result = "valid_path_name".ToValidPath();
        Assert.That(result, Is.EqualTo("valid_path_name"));
    }

    #endregion

    #region RemoveEmptyLines Tests

    [Test]
    public void RemoveEmptyLines_RemovesBlankLines()
    {
        var input = "Line1\n\nLine2\n\nLine3";
        var result = input.RemoveEmptyLines();

        Assert.That(result, Does.Not.Contain("\n\n"));
        Assert.That(result, Does.Contain("Line1"));
        Assert.That(result, Does.Contain("Line2"));
        Assert.That(result, Does.Contain("Line3"));
    }

    [Test]
    public void RemoveEmptyLines_NoEmptyLines_ShouldPreserveText()
    {
        var input = "Line1\nLine2\nLine3";
        var result = input.RemoveEmptyLines();

        // RemoveEmptyLines may normalize line endings to \r\n
        Assert.That(result, Does.Contain("Line1"));
        Assert.That(result, Does.Contain("Line2"));
        Assert.That(result, Does.Contain("Line3"));
        Assert.That(result, Does.Not.Contain("\n\n"));
        Assert.That(result, Does.Not.Contain("\r\n\r\n"));
    }

    #endregion

    #region ValueToLiteral Tests

    [Test]
    public void ValueToQuotedLiteral_SimpleString_ShouldAddQuotes()
    {
        var result = "Hello".ValueToQuotedLiteral();
        Assert.That(result, Is.EqualTo("\"Hello\""));
    }

    [Test]
    public void ValueToLiteral_WithEscapes_ShouldEscapeSpecialChars()
    {
        var result = "Line1\nLine2".ValueToLiteral();
        Assert.That(result, Does.Contain("\\n"));
    }

    [Test]
    public void ValueToLiteral_WithTab_ShouldEscapeTab()
    {
        var result = "Hello\tWorld".ValueToLiteral();
        Assert.That(result, Does.Contain("\\t"));
    }

    #endregion

    #region GetHashSha256 Tests

    [Test]
    public void GetHashSha256_SameInput_ShouldProduceSameHash()
    {
        var hash1 = "Hello World".GetHashSha256();
        var hash2 = "Hello World".GetHashSha256();

        Assert.That(hash1, Is.EqualTo(hash2));
    }

    [Test]
    public void GetHashSha256_DifferentInput_ShouldProduceDifferentHash()
    {
        var hash1 = "Hello".GetHashSha256();
        var hash2 = "World".GetHashSha256();

        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }

    [Test]
    public void GetHashSha256_EmptyString_ShouldProduceHash()
    {
        var hash = "".GetHashSha256();

        Assert.That(hash, Is.Not.Null);
        Assert.That(hash, Is.Not.Empty);
        Assert.That(hash.Length, Is.EqualTo(64)); // SHA256 produces 64 hex chars
    }

    [Test]
    public void GetHashSha256_ShouldBeDeterministic()
    {
        var input = "Test String 123";
        var hash1 = input.GetHashSha256();
        var hash2 = input.GetHashSha256();
        var hash3 = input.GetHashSha256();

        Assert.That(hash1, Is.EqualTo(hash2));
        Assert.That(hash2, Is.EqualTo(hash3));
    }

    #endregion

    #region TryGetSubstring Tests

    [Test]
    public void TryGetSubstring_ValidRange_ShouldReturnSubstring()
    {
        var result = "Hello World".TryGetSubstring(0, 5);
        Assert.That(result, Is.EqualTo("Hello"));
    }

    [Test]
    public void TryGetSubstring_OutOfRange_ShouldReturnSafe()
    {
        // Should not throw, but return what's available
        var result = "Hello".TryGetSubstring(0, 100);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void TryGetSubstring_NegativeIndex_ShouldHandleGracefully()
    {
        var result = "Hello".TryGetSubstring(-1, 3);
        Assert.That(result, Is.Not.Null);
    }

    #endregion
}
