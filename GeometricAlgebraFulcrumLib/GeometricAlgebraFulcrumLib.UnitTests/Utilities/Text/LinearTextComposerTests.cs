using GeometricAlgebraFulcrumLib.Utilities.Text.Text.Linear;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Utilities.Text;

/// <summary>
/// Tests for LinearTextComposer - Advanced text generation with line management and indentation
/// </summary>
[TestFixture]
public class LinearTextComposerTests
{
    #region Construction and Basic Operations

    [Test]
    public void Constructor_ShouldCreateEmptyComposer()
    {
        var composer = new LinearTextComposer();

        Assert.That(composer.LinesCount, Is.EqualTo(0));
        Assert.That(composer.StoredLinesCount, Is.EqualTo(0));
        Assert.That(composer.CurrentText, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Append_ShouldAddToLineBuffer()
    {
        var composer = new LinearTextComposer();

        composer.Append("Hello");

        Assert.That(composer.LineBufferText, Is.EqualTo("Hello"));
        Assert.That(composer.LinesCount, Is.EqualTo(1)); // Buffer counts as line
        Assert.That(composer.StoredLinesCount, Is.EqualTo(0)); // Not stored yet
    }

    [Test]
    public void AppendLine_ShouldStoreLineAndClearBuffer()
    {
        var composer = new LinearTextComposer();

        composer.AppendLine("Hello");

        Assert.That(composer.LineBufferText, Is.EqualTo(string.Empty));
        Assert.That(composer.StoredLinesCount, Is.EqualTo(1));
        Assert.That(composer.CurrentText, Does.Contain("Hello"));
    }

    [Test]
    public void AppendMultipleLines_ShouldStoreAll()
    {
        var composer = new LinearTextComposer();

        composer.AppendLine("Line 1");
        composer.AppendLine("Line 2");
        composer.AppendLine("Line 3");

        Assert.That(composer.StoredLinesCount, Is.EqualTo(3));
        var text = composer.CurrentText;
        Assert.That(text, Does.Contain("Line 1"));
        Assert.That(text, Does.Contain("Line 2"));
        Assert.That(text, Does.Contain("Line 3"));
    }

    #endregion

    #region Clear Operations

    [Test]
    public void Clear_ShouldRemoveAllTextAndResetHeaders()
    {
        var composer = new LinearTextComposer();
        composer.AppendLine("Test");
        composer.IncreaseIndentation();

        composer.Clear();

        Assert.That(composer.LinesCount, Is.EqualTo(0));
        Assert.That(composer.IndentationLevel, Is.EqualTo(0));
        Assert.That(composer.CurrentText, Is.EqualTo(string.Empty));
    }

    [Test]
    public void ClearText_ShouldRemoveTextButKeepHeaders()
    {
        var composer = new LinearTextComposer();
        composer.IncreaseIndentation();
        composer.AppendLine("Test");

        composer.ClearText();

        Assert.That(composer.LinesCount, Is.EqualTo(0));
        Assert.That(composer.IndentationLevel, Is.EqualTo(1)); // Indentation preserved
    }

    #endregion

    #region Indentation Tests

    [Test]
    public void IncreaseIndentation_ShouldIncrementLevel()
    {
        var composer = new LinearTextComposer();

        composer.IncreaseIndentation();

        Assert.That(composer.IndentationLevel, Is.EqualTo(1));
        Assert.That(composer.IndentationString, Is.Not.Empty);
    }

    [Test]
    public void IncreaseIndentation_Multiple_ShouldStackLevels()
    {
        var composer = new LinearTextComposer();

        composer.IncreaseIndentation();
        composer.IncreaseIndentation();
        composer.IncreaseIndentation();

        Assert.That(composer.IndentationLevel, Is.EqualTo(3));
    }

    [Test]
    public void DecreaseIndentation_ShouldDecrementLevel()
    {
        var composer = new LinearTextComposer();
        composer.IncreaseIndentation();
        composer.IncreaseIndentation();

        composer.DecreaseIndentation();

        Assert.That(composer.IndentationLevel, Is.EqualTo(1));
    }

    [Test]
    public void ClearIndentation_ShouldResetToZero()
    {
        var composer = new LinearTextComposer();
        composer.IncreaseIndentation();
        composer.IncreaseIndentation();

        composer.ClearIndentation();

        Assert.That(composer.IndentationLevel, Is.EqualTo(0));
    }

    [Test]
    public void IncreaseIndentation_WithCustomString_ShouldUseCustomIndent()
    {
        var composer = new LinearTextComposer();

        composer.IncreaseIndentation(">>>>");

        Assert.That(composer.IndentationString, Does.Contain(">"));
    }

    #endregion

    #region Chaining Tests

    [Test]
    public void Methods_ShouldSupportChaining()
    {
        var composer = new LinearTextComposer();

        composer.Append("Hello")
            .Append(" ")
            .AppendLine("World")
            .IncreaseIndentation()
            .AppendLine("Indented");

        Assert.That(composer.StoredLinesCount, Is.EqualTo(2));
        Assert.That(composer.IndentationLevel, Is.EqualTo(1));
    }

    #endregion
}
