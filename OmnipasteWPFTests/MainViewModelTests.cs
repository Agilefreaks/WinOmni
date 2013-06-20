namespace OmnipasteWPFTests
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using OmnipasteWPF.ViewModels;

    [TestFixture]
    public class MainViewModelTests : ViewModelTestsBase
    {
        private MainViewModel _subject;

        private Mock<IActivationService> _mockActivationService;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _mockActivationService = new Mock<IActivationService>();
            MockIOCProvider.Setup(x => x.GetTypeFromContainer<IActivationService>())
                            .Returns(_mockActivationService.Object);
            _subject = new MainViewModel(MockIOCProvider.Object);
        }

        [Test]
        public void Ctor_Always_ShouldSetActivationServiceToAnInstanceOfActivationService()
        {
            _subject.ActivationService.Should().NotBeNull();
        }

        [Test]
        public void StartActivationProcess_Always_CallsActivationServiceRun()
        {
            _subject.StartActivationProcess();

            _mockActivationService.Verify(x => x.Run());
        }
    }
}
