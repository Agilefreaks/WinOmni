namespace OmnipasteWPFTests
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmnipasteWPF.ViewModels;

    [TestFixture]
    public class MainViewModelTests
    {
        private MainViewModel _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new MainViewModel();
        }

        [Test]
        public void Ctor_Always_ShouldSetActivationServiceToAnInstanceOfActivationService()
        {
            _subject.ActivationService.Should().NotBeNull();
        }
    }
}
