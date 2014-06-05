namespace OmnipasteTests.ViewModels
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Loading;

    [TestFixture]
    public class LoadingViewModelTests
    {
        private ILoadingViewModel _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new LoadingViewModel();
        }

        [Test]
        public void NewInstance_IsInLoadingState()
        {
            _subject.State.Should().Be(LoadingViewModelStateEnum.Loading);
        }

        [Test]
        public void Handle_GetTokenFromUser_SetsStateToAwaitingUserTokenInput()
        {
            _subject.Handle(new GetTokenFromUserMessage());

            _subject.State.Should().Be(LoadingViewModelStateEnum.AwaitingUserTokenInput);
        }

        [Test]
        public void StateChange_Always_TriggersNotifyOfPropertyChanged()
        {
            string changedProperty = string.Empty;
            _subject.PropertyChanged += (sender, args) => changedProperty = args.PropertyName;

            _subject.State = LoadingViewModelStateEnum.AwaitingUserTokenInput;

            changedProperty.Should().Be("State");
        }

        [Test]
        public void Handle_WithConfigurationCompleteMessage_ClosesTheDialog()
        {
            var wasClosed = false;
            _subject.AttemptingDeactivation += (sender, args) => wasClosed = args.WasClosed;

            _subject.Handle(new ConfigurationCompletedMessage());

            wasClosed.Should().Be(false);
        }
    }
}