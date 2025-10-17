using GeometricAlgebraFulcrumLib.Utilities.Text.Loggers.Progress;
using NUnit.Framework;

namespace GeometricAlgebraFulcrumLib.UnitTests.Utilities.Text;

/// <summary>
/// Tests for ProgressComposer - Progress reporting and tracking for long-running operations
/// </summary>
[TestFixture]
public class ProgressComposerTests
{
    #region Construction and Basic Properties

    [Test]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var progress = new ProgressComposer();

        Assert.That(progress.Enabled, Is.True);
        Assert.That(progress.Status, Is.EqualTo(ProgressComposerStatus.NotRunning));
        Assert.That(progress.History, Is.Not.Null);
    }

    [Test]
    public void Enabled_ShouldBeSettable()
    {
        var progress = new ProgressComposer();

        progress.Enabled = false;

        Assert.That(progress.Enabled, Is.False);
    }

    #endregion

    #region Status Tests

    [Test]
    public void Status_InitiallyNotRunning()
    {
        var progress = new ProgressComposer();

        Assert.That(progress.Status, Is.EqualTo(ProgressComposerStatus.NotRunning));
    }

    [Test]
    public void Status_IsReadOnly()
    {
        // Status has internal setter, so we can only read it
        var progress = new ProgressComposer();

        Assert.That(progress.Status, Is.EqualTo(ProgressComposerStatus.NotRunning));

        // Status can only be changed internally by the class
        // We verify that the property is accessible and has correct initial value
    }

    #endregion

    #region IsReady Extension Tests

    [Test]
    public void IsReady_WhenEnabledAndNotNull_ShouldReturnTrue()
    {
        var progress = new ProgressComposer
        {
            Enabled = true
        };

        Assert.That(progress.IsReady(), Is.True);
    }

    [Test]
    public void IsReady_WhenDisabled_ShouldReturnFalse()
    {
        var progress = new ProgressComposer
        {
            Enabled = false
        };

        Assert.That(progress.IsReady(), Is.False);
    }

    [Test]
    public void IsReady_WhenNull_ShouldReturnFalse()
    {
        ProgressComposer progress = null;

        Assert.That(progress.IsReady(), Is.False);
    }

    #endregion

    #region DisableAfterNextReport Tests

    [Test]
    public void DisableAfterNextReport_InitiallyFalse()
    {
        var progress = new ProgressComposer();

        Assert.That(progress.DisableAfterNextReport, Is.False);
    }

    [Test]
    public void DisableAfterNextReport_CanBeSet()
    {
        var progress = new ProgressComposer();

        progress.DisableAfterNextReport = true;

        Assert.That(progress.DisableAfterNextReport, Is.True);
    }

    #endregion
}
