namespace OmnipasteWPFTests.ViewModels
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmnipasteWPF.ViewModels.TrayIcon;

    [TestFixture]
    public class TrayIconViewModelTests
    {
        private ITrayIconViewModel _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new TrayIconViewModel();
        }

        [Test]
        public void Ctor_Alwways_SetsTrayIconVisibleToFalse()
        {
            _subject.TrayIconVisible.Should().BeFalse();
        }

        [Test]
        public void Start_Always_SetsTrayIconVisibleToTrue()
        {
            _subject.Start();

            _subject.TrayIconVisible.Should().BeTrue();
        }
    }
}