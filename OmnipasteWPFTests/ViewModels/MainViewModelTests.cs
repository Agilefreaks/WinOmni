namespace OmnipasteWPFTests.ViewModels
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
    using OmnipasteWPF.ViewModels.GetTokenFromUser;
    using OmnipasteWPF.ViewModels.MainView;
    using OmnipasteWPF.ViewModels.TrayIcon;

    [TestFixture]
    public class MainViewModelTests : ViewModelTests
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
            this._mockActivationService = new Mock<IActivationService>();
            this._mockEventAggregator = new Mock<IEventAggregator>();
            this._mockApplicationWrapper = new Mock<IApplicationWrapper>();
            this.MockIOCProvider.Setup(x => x.GetTypeFromContainer<IActivationService>())
                            .Returns(this._mockActivationService.Object);
            this.MockIOCProvider.Setup(x => x.GetTypeFromContainer<IEventAggregator>())
                            .Returns(this._mockEventAggregator.Object);
            this.MockIOCProvider.Setup(x => x.GetTypeFromContainer<IApplicationWrapper>())
                           .Returns(this._mockApplicationWrapper.Object);
            this._mockGetTokenFromUserViewModel = new Mock<IGetTokenFromUserViewModel>();
            ViewModelBase.ServiceProvider.Add(typeof(IUIVisualizerService), this.MockUiVisualizerService.Object);
            this._mockTrayIconViewModel = new Mock<ITrayIconViewModel>();
            this._subject = new MainViewModel(this.MockIOCProvider.Object)
                           {
                               GetTokenFromUserViewModel =
                                   this._mockGetTokenFromUserViewModel.Object,
                               TrayIconViewModel = this._mockTrayIconViewModel.Object
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
            this._subject.ActivationService.Should().NotBeNull();
        }

        [Test]
        public void Ctor_Always_ShouldCallEventAggregatorSubscribeWithSelf()
        {
            this._mockEventAggregator.Verify(x => x.Subscribe(this._subject), Times.Once());
        }

        [Test]
        public void RunActivationProcess_Always_CallsActivationServiceRun()
        {
            this._subject.RunActivationProcess();

            this._mockActivationService.Verify(x => x.Run());
        }

        [Test]
        public void Handle_WithGetTokenFromUserMessageAlways_CallsUiVisualizerServiceShowDialogWithCorrectParameters()
        {
            this._subject.Handle(new GetTokenFromUserMessage());

            this.MockUiVisualizerService.Verify(x => x.ShowDialog("GetTokenFromUser", this._mockGetTokenFromUserViewModel.Object));
        }

        [Test]
        public void Handle_WithGetTokenFromUserMessageAndShowDialogIsSuccessfull_CallsEventAggregatorPublishGetTokenFromUserRequestResult()
        {
            this.MockUiVisualizerService.Setup(x => x.ShowDialog("GetTokenFromUser", this._mockGetTokenFromUserViewModel.Object))
                                   .Returns(true);

            this._subject.Handle(new GetTokenFromUserMessage());

            this._mockEventAggregator.Verify(x => x.Publish(It.IsAny<TokenRequestResutMessage>()), Times.Once());
        }

        [Test]
        public void Handle_WithGetTokenFromUserMessageAndShowDialogIsSuccessful_PublishesACorrectTokenRequestResultMessage()
        {
            this._mockGetTokenFromUserViewModel.Setup(x => x.Token).Returns("testToken");
            this.MockUiVisualizerService.Setup(x => x.ShowDialog("GetTokenFromUser", this._mockGetTokenFromUserViewModel.Object))
                                   .Returns(true);

            this._subject.Handle(new GetTokenFromUserMessage());

            this._mockEventAggregator.Verify(
                x =>
                x.Publish(
                    It.Is<TokenRequestResutMessage>(
                        r => r.Status == TokenRequestResultMessageStatusEnum.Successful && r.Token == "testToken")),
                Times.Once());
        }

        [Test]
        public void Handle_WithGetTokenFromUserMessageAndShowDialogIsCanceled_PublishesACorrectTokenRequestResultMessage()
        {
            this.MockUiVisualizerService.Setup(x => x.ShowDialog("GetTokenFromUser", this._mockGetTokenFromUserViewModel.Object))
                                   .Returns(false);

            this._subject.Handle(new GetTokenFromUserMessage());

            this._mockEventAggregator.Verify(
                x =>
                x.Publish(
                    It.Is<TokenRequestResutMessage>(r => r.Status == TokenRequestResultMessageStatusEnum.Canceled)),
                Times.Once());
        }

        [Test]
        public void Handle_WithGetTokenFromUserMessageAndShowDialogIsUnsuccessful_PublishesACorrectTokenRequestResultMessage()
        {
            this.MockUiVisualizerService.Setup(x => x.ShowDialog("GetTokenFromUser", this._mockGetTokenFromUserViewModel.Object))
                                   .Returns((bool?)null);

            this._subject.Handle(new GetTokenFromUserMessage());

            this._mockEventAggregator.Verify(
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
            this._mockActivationService.Setup(x => x.CurrentStep).Returns(mockActivationStep.Object);

            this._subject.RunActivationProcess();

            this._mockApplicationWrapper.Verify(x => x.ShutDown(), Times.Once());
        }

        [Test]
        public void RunActivationProcess_ActivationServiceCurrentStepIsNull_CallsApplicationWrapperShutdown()
        {
            this._mockActivationService.Setup(x => x.CurrentStep).Returns((IActivationStep)null);

            this._subject.RunActivationProcess();

            this._mockApplicationWrapper.Verify(x => x.ShutDown(), Times.Once());
        }

        [Test]
        public void RunActivationProcess_ActivationServiceCurrentStepIsNotFailedOrNull_CallsStartOnTrayViewModel()
        {
            var mockActivationStep = new Mock<IActivationStep>();
            mockActivationStep.Setup(x => x.GetId()).Returns(typeof(Finished));
            this._mockActivationService.Setup(x => x.CurrentStep).Returns(mockActivationStep.Object);

            this._subject.RunActivationProcess();

            this._mockTrayIconViewModel.Verify(x => x.Start(), Times.Once());
        }
    }
}
