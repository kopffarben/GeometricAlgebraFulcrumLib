using GeometricAlgebraFulcrumLib.Utilities.Text.Text.Structured;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Utilities.Text;

/// <summary>
/// Tests for ListTextComposer - Structured list-based text generation with separators
/// </summary>
[TestFixture]
public class ListTextComposerTests
{
    #region Construction and Basic Operations

    [Test]
    public void Constructor_Default_ShouldCreateEmptyList()
    {
        var composer = new ListTextComposer();

        Assert.That(composer.Count, Is.EqualTo(0));
        Assert.That(composer.Separator, Is.EqualTo(string.Empty));
        Assert.That(composer.ToString(), Is.EqualTo(string.Empty));
    }

    [Test]
    public void Constructor_WithSeparator_ShouldSetSeparator()
    {
        var composer = new ListTextComposer(", ");

        Assert.That(composer.Separator, Is.EqualTo(", "));
    }

    [Test]
    public void Add_ShouldAddItemToList()
    {
        var composer = new ListTextComposer();

        composer.Add("Item1");

        Assert.That(composer.Count, Is.EqualTo(1));
    }

    [Test]
    public void Add_MultipleItems_ShouldAddAll()
    {
        var composer = new ListTextComposer();

        composer.Add("Item1")
            .Add("Item2")
            .Add("Item3");

        Assert.That(composer.Count, Is.EqualTo(3));
    }

    #endregion

    #region Separator Tests

    [Test]
    public void Generate_WithSeparator_ShouldJoinItemsCorrectly()
    {
        var composer = new ListTextComposer(", ");

        composer.Add("Apple")
            .Add("Banana")
            .Add("Cherry");

        var result = composer.Generate();

        Assert.That(result, Is.EqualTo("Apple, Banana, Cherry"));
    }

    [Test]
    public void Generate_WithoutSeparator_ShouldConcatenateItems()
    {
        var composer = new ListTextComposer();

        composer.Add("A").Add("B").Add("C");

        var result = composer.Generate();

        Assert.That(result, Is.EqualTo("ABC"));
    }

    #endregion

    #region Prefix/Suffix Tests

    [Test]
    public void Generate_WithFinalPrefix_ShouldPrependToResult()
    {
        var composer = new ListTextComposer(", ")
        {
            FinalPrefix = "["
        };

        composer.Add("A").Add("B");

        var result = composer.Generate();

        Assert.That(result, Is.EqualTo("[A, B"));
    }

    [Test]
    public void Generate_WithFinalSuffix_ShouldAppendToResult()
    {
        var composer = new ListTextComposer(", ")
        {
            FinalSuffix = "]"
        };

        composer.Add("A").Add("B");

        var result = composer.Generate();

        Assert.That(result, Is.EqualTo("A, B]"));
    }

    [Test]
    public void Generate_WithPrefixAndSuffix_ShouldWrapResult()
    {
        var composer = new ListTextComposer(", ")
        {
            FinalPrefix = "[",
            FinalSuffix = "]"
        };

        composer.Add("A").Add("B").Add("C");

        var result = composer.Generate();

        Assert.That(result, Is.EqualTo("[A, B, C]"));
    }

    #endregion

    #region ReverseItems Tests

    [Test]
    public void Generate_WithReverseItems_ShouldReverseOrder()
    {
        var composer = new ListTextComposer(", ")
        {
            ReverseItems = true
        };

        composer.Add("First")
            .Add("Second")
            .Add("Third");

        var result = composer.Generate();

        Assert.That(result, Is.EqualTo("Third, Second, First"));
    }

    #endregion

    #region AddIfNotEmpty Tests

    [Test]
    public void AddIfNotEmpty_WithEmptyString_ShouldNotAdd()
    {
        var composer = new ListTextComposer();

        composer.AddIfNotEmpty("")
            .AddIfNotEmpty("Valid")
            .AddIfNotEmpty(null);

        Assert.That(composer.Count, Is.EqualTo(1));
        Assert.That(composer.Generate(), Is.EqualTo("Valid"));
    }

    [Test]
    public void AddRangeIfNotEmpty_ShouldFilterEmptyStrings()
    {
        var composer = new ListTextComposer(", ");

        composer.AddRangeIfNotEmpty("A", "", "B", null, "C");

        Assert.That(composer.Count, Is.EqualTo(3));
        Assert.That(composer.Generate(), Is.EqualTo("A, B, C"));
    }

    #endregion

    #region ToString Tests

    [Test]
    public void ToString_ShouldCallGenerate()
    {
        var composer = new ListTextComposer(" | ");

        composer.Add("X").Add("Y").Add("Z");

        Assert.That(composer.ToString(), Is.EqualTo("X | Y | Z"));
    }

    #endregion
}
