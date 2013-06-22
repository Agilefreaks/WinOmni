namespace OmnipasteWPFTests
{
    using Caliburn.Micro;
    using Cinch;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.EventAggregatorMessages;
    using OmniCommon.Interfaces;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;
    using OmnipasteWPF.DataProviders;
    using OmnipasteWPF.ViewModels;
    using OmnipasteWPF.ViewModels.GetTokenFromUser;
    using OmnipasteWPF.ViewModels.MainView;
    using OmnipasteWPF.ViewModels.TrayIcon;
    using ViewModelBase = Cinch.ViewModelBase;

    [TestFixture]
    public class MainViewModelTests : ViewModelTestsBase
    {
        private MainViewModel _subject;

        private Mock<IActivationService> _mockActivationService;

        private Mock<IEventAggregator> _mockEventAggregator;

        private Mock<IGetTokenFromUserViewModel> _mockGetTokenFromUserViewModel;

        private Mock<IApplicationWrapper> _mockApplicationWrapper;

        private Mock<ITrayIconViewModel> _mockTrayIconViewModel;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _mockActivationService = new Mock<IActivationService>();
            _mockEventAggregator = new Mock<IEventAggregator>();
            _mockApplicationWrapper = new Mock<IApplicationWrapper>();
            MockIOCProvider.Setup(x => x.GetTypeFromContainer<IActivationService>())
                            .Returns(_mockActivationService.Object);
            MockIOCProvider.Setup(x => x.GetTypeFromContainer<IEventAggregator>())
                            .Returns(_mockEventAggregator.Object);
            MockIOCProvider.Setup(x => x.GetTypeFromContainer<IApplicationWrapper>())
                           .Returns(_mockApplicationWrapper.Object);
            _mockGetTokenFromUserViewModel = new Mock<IGetTokenFromUserViewModel>();
            ViewModelBase.ServiceProvider.Add(typeof(IUIVisualizerService), MockUiVisualizerService.Object);
            _mockTrayIconViewModel = new Mock<ITrayIconViewModel>();
            _subject = new MainViewModel(MockIOCProvider.Object)
                           {
                               GetTokenFromUserViewModel =
                                   _mockGetTokenFromUserViewModel.Object,
                               TrayIconViewModel = _mockTrayIconViewModel.Object
                           };
        }

        [TearDown]
        public void TearDown()
        {
            ViewModelBase.ServiceProvider.Remove(typeof(IUIVisualizerService));
        }

        [Test]
        public void Ctor_Always_ShouldSetActivationServiceToAnInstanceOfActivationService()
        {
            _subject.ActivationService.Should().NotBeNull();
        }

        [Test]
        public void Ctor_Always_ShouldCallEventAggregatorSubscribeWithSelf()
        {
            _mockEventAggregator.Verify(x => x.Subscribe(_subject), Times.Once());
        }

        [Test]
        public void RunActivationProcess_Always_CallsActivationServiceRun()
        {
            _subject.RunActivationProcess();

            _mockActivationService.Verify(x => x.Run());
        }

        [Test]
        public void Handle_WithGetTokenFromUserMessageAlways_CallsUiVisualizerServiceShowDialogWithCorrectParameters()
        {
            _subject.Handle(new GetTokenFromUserMessage());

            MockUiVisualizerService.Verify(x => x.ShowDialog("GetTokenFromUser", _mockGetTokenFromUserViewModel.Object));
        }

        [Test]
        public void Handle_WithGetTokenFromUserMessageAndShowDialogIsSuccessfull_CallsEventAggregatorPublishGetTokenFromUserRequestResult()
        {
            MockUiVisualizerService.Setup(x => x.ShowDialog("GetTokenFromUser", _mockGetTokenFromUserViewModel.Object))
                                   .Returns(true);

            _subject.Handle(new GetTokenFromUserMessage());

            _mockEventAggregator.Verify(x => x.Publish(It.IsAny<TokenRequestResutMessage>()), Times.Once());
        }

        [Test]
        public void Handle_WithGetTokenFromUserMessageAndShowDialogIsSuccessful_PublishesACorrectTokenRequestResultMessage()
        {
            _mockGetTokenFromUserViewModel.Setup(x => x.Token).Returns("testToken");
            MockUiVisualizerService.Setup(x => x.ShowDialog("GetTokenFromUser", _mockGetTokenFromUserViewModel.Object))
                                   .Returns(true);

            _subject.Handle(new GetTokenFromUserMessage());

            _mockEventAggregator.Verify(
                x =>
                x.Publish(
                    It.Is<TokenRequestResutMessage>(
                        r => r.Status == TokenRequestResultMessageStatusEnum.Successful && r.Token == "testToken")),
                Times.Once());
        }

        [Test]
        public void Handle_WithGetTokenFromUserMessageAndShowDialogIsCanceled_PublishesACorrectTokenRequestResultMessage()
        {
            MockUiVisualizerService.Setup(x => x.ShowDialog("GetTokenFromUser", _mockGetTokenFromUserViewModel.Object))
                                   .Returns(false);

            _subject.Handle(new GetTokenFromUserMessage());

            _mockEventAggregator.Verify(
                x =>
                x.Publish(
                    It.Is<TokenRequestResutMessage>(r => r.Status == TokenRequestResultMessageStatusEnum.Canceled)),
                Times.Once());
        }

        [Test]
        public void Handle_WithGetTokenFromUserMessageAndShowDialogIsUnsuccessful_PublishesACorrectTokenRequestResultMessage()
        {
            MockUiVisualizerService.Setup(x => x.ShowDialog("GetTokenFromUser", _mockGetTokenFromUserViewModel.Object))
                                   .Returns((bool?)null);

            _subject.Handle(new GetTokenFromUserMessage());

            _mockEventAggregator.Verify(
                x =>
                x.Publish(
                    It.Is<TokenRequestResutMessage>(r => r.Status == TokenRequestResultMessageStatusEnum.Canceled)),
                Times.Once());
        }

        [Test]
        public void RunActivationProcess_ActivationServiceCurrentStepIsFailed_CallsApplicationWrapperShutdown()
        {
            var mockActivationStep = new Mock<IActivationStep>();
            mockActivationStep.Setup(x => x.GetId()).Returns(typeof(Failed));
            _mockActivationService.Setup(x => x.CurrentStep).Returns(mockActivationStep.Object);

            _subject.RunActivationProcess();

            _mockApplicationWrapper.Verify(x => x.ShutDown(), Times.Once());
        }

        [Test]
        public void RunActivationProcess_ActivationServiceCurrentStepIsNull_CallsApplicationWrapperShutdown()
        {
            _mockActivationService.Setup(x => x.CurrentStep).Returns((IActivationStep)null);

            _subject.RunActivationProcess();

            _mockApplicationWrapper.Verify(x => x.ShutDown(), Times.Once());
        }

        [Test]
        public void RunActivationProcess_ActivationServiceCurrentStepIsNotFailedOrNull_SetsTrayIconViewModelTrayIconVisibleTrue()
        {
            var mockActivationStep = new Mock<IActivationStep>();
            mockActivationStep.Setup(x => x.GetId()).Returns(typeof(Finished));
            _mockActivationService.Setup(x => x.CurrentStep).Returns(mockActivationStep.Object);

            _subject.RunActivationProcess();

            _mockTrayIconViewModel.VerifySet(x => x.TrayIconVisible = true);
        }
    }
}
