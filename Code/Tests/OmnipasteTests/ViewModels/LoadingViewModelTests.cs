namespace OmnipasteTests.ViewModels
{
    using System.Threading;
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Loading;
    using Omnipaste.UserToken;

    [TestFixture]
    public class LoadingViewModelTests
    {
        private ILoadingViewModel _subject;

        private IEventAggregator _eventAggregator;

        private Mock<IUserTokenViewModel> _mockUserTokenViewModel;

        [SetUp]
        public void SetUp()
        {
            _eventAggregator = new EventAggregator();
            _mockUserTokenViewModel = new Mock<IUserTokenViewModel>();
            _subject = new LoadingViewModel(_eventAggregator) { UserTokenViewModel = _mockUserTokenViewModel.Object };
            var parent = new DialogViewModel();
            parent.ActivateItem(_subject);
        }

        [Test]
        public void NewInstance_IsInLoadingState()
        {
            _subject.State.Should().Be(LoadingViewModelStateEnum.Loading);
        }

        [Test]
        public void Handle_GetTokenFromUser_SetsStateToAwaitingUserTokenInput()
        {
            _eventAggregator.PublishOnCurrentThread(new GetTokenFromUserMessage());

            _subject.State.Should().Be(LoadingViewModelStateEnum.AwaitingUserTokenInput);
        }

        [Test]
        public void Handle_GetTokenFromUser_SetsTheMessageOnTheUserTokenViewModel()
        {
            _eventAggregator.PublishOnCurrentThread(new GetTokenFromUserMessage { Message = "message" });

            _mockUserTokenViewModel.VerifySet(vm => vm.Message = "message");
        }

        [Test]
        public void StateChange_Always_TriggersNotifyOfPropertyChanged()
        {
            string changedProperty = string.Empty;
            _subject.PropertyChanged += (sender, args) => changedProperty = args.PropertyName;

            _subject.State = LoadingViewModelStateEnum.AwaitingUserTokenInput;

            changedProperty.Should().Be("State");
        }
    }
}