namespace OmnipasteTests.Loading
{
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Loading;
    using Omnipaste.Loading.ActivationFailed;
    using Omnipaste.Loading.AndroidInstallGuide;
    using Omnipaste.Loading.UserToken;

    [TestFixture]
    public class LoadingViewModelTests
    {
        private ILoadingViewModel _subject;

        private IEventAggregator _eventAggregator;

        private Mock<IUserTokenViewModel> _mockUserTokenViewModel;

        private Mock<IActivationFailedViewModel> _mockActivationFailedViewModel;

        private Mock<IAndroidInstallGuideViewModel> _mockAndroidInstallGuideViewModel;

        private LoadingViewModel _instance;

        [SetUp]
        public void SetUp()
        {
            _eventAggregator = new EventAggregator();
            _mockUserTokenViewModel = new Mock<IUserTokenViewModel>();
            _mockActivationFailedViewModel = new Mock<IActivationFailedViewModel>();
            _mockAndroidInstallGuideViewModel = new Mock<IAndroidInstallGuideViewModel>();
            _instance = new LoadingViewModel(_eventAggregator)
                            {
                                UserTokenViewModel = _mockUserTokenViewModel.Object,
                                ActivationFailedViewModel = _mockActivationFailedViewModel.Object,
                                AndroidInstallGuideViewModel = _mockAndroidInstallGuideViewModel.Object
                            };
            _subject = _instance;
            var parent = new DialogViewModel();
            parent.ActivateItem(_subject);
        }

        [Test]
        public void NewInstance_IsInLoadingState()
        {
            _subject.State.Should().Be(LoadingViewModelStateEnum.Loading);
        }

        [Test]
        public void Handle_GetTokenFromUser_SetsStateToOther()
        {
            _eventAggregator.PublishOnCurrentThread(new GetTokenFromUserMessage());

            _subject.State.Should().Be(LoadingViewModelStateEnum.Other);
        }

        [Test]
        public void Handle_GetTokenFromUser_SetsTheMessageOnTheUserTokenViewModel()
        {
            _eventAggregator.PublishOnCurrentThread(new GetTokenFromUserMessage { Message = "message" });

            _mockUserTokenViewModel.VerifySet(vm => vm.Message = "message");
        }

        [Test]
        public void Handle_GetTokenFromUser_SendsAShowShellMessage()
        {
            var mockEventAggregator = new Mock<IEventAggregator>();
            _instance.EventAggregator = mockEventAggregator.Object;

            _eventAggregator.PublishOnCurrentThread(new GetTokenFromUserMessage());

            mockEventAggregator.Verify(x => x.Publish(It.IsAny<ShowShellMessage>(), It.IsAny<System.Action<System.Action>>()));
        }

        [Test]
        public void Handle_ActivationFailed_SetsStateToOtherAndActiveItem()
        {
            _eventAggregator.PublishOnCurrentThread(new ActivationFailedMessage());

            _subject.State.Should().Be(LoadingViewModelStateEnum.Other);
            _subject.ActiveItem.Should().Be(_mockActivationFailedViewModel.Object);
        }

        [Test]
        public void Handle_ActivationFailed_SendsAShowShellMessage()
        {
            var mockEventAggregator = new Mock<IEventAggregator>();
            _instance.EventAggregator = mockEventAggregator.Object;

            _eventAggregator.PublishOnCurrentThread(new ActivationFailedMessage());

            mockEventAggregator.Verify(x => x.Publish(It.IsAny<ShowShellMessage>(), It.IsAny<System.Action<System.Action>>()));
        }

        [Test]
        public void StateChange_Always_TriggersNotifyOfPropertyChanged()
        {
            string changedProperty = string.Empty;
            _subject.PropertyChanged += (sender, args) => changedProperty = args.PropertyName;

            _subject.State = LoadingViewModelStateEnum.Other;

            changedProperty.Should().Be("State");
        }

        [Test]
        public void Handle_ShowAndroidInstallGuide_SetsStateToOther()
        {
            _eventAggregator.PublishOnCurrentThread(new ShowAndroidInstallGuideMessage());

            _subject.State.Should().Be(LoadingViewModelStateEnum.Other);
            _subject.ActiveItem.Should().Be(_mockAndroidInstallGuideViewModel.Object);
        }

        [Test]
        public void Handle_ShowAndroidInstallGuide_SendsAShowShellMessage()
        {
            var mockEventAggregator = new Mock<IEventAggregator>();
            _instance.EventAggregator = mockEventAggregator.Object;

            _eventAggregator.PublishOnCurrentThread(new GetTokenFromUserMessage());

            mockEventAggregator.Verify(x => x.Publish(It.IsAny<ShowShellMessage>(), It.IsAny<System.Action<System.Action>>()));
        }
    }
}