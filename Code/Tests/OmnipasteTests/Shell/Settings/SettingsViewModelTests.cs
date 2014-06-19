namespace OmnipasteTests.Shell.Settings
{
    using FluentAssertions;
    using MahApps.Metro.Controls;
    using NUnit.Framework;
    using Omnipaste.Shell.Settings;

    [TestFixture]
    public class SettingsViewModelTests
    {
        private ISettingsViewModel _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new SettingsViewModel();
        }

        [Test]
        public void NewInstance_IsNotOpened()
        {
            _subject.IsOpen.Should().BeFalse();
        }

        [Test]
        public void NewInstance_ShouldBePlacedOnTheRight()
        {
            _subject.Position.Should().Be(Position.Right);
        }
    }
}