namespace OmnipasteTests.Shell
{
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.Configuration;
    using Omnipaste.ContextMenu;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Shell;
    using Omnipaste.UserToken;

    [TestFixture]
    public class ShellViewModelTests
    {
        private IShellViewModel _subject;

        private Mock<IConfigurationViewModel> _configurationViewModel;

        [SetUp]
        public void SetUp()
        {
            _configurationViewModel = new Mock<IConfigurationViewModel>();
            _subject = new ShellViewModel(_configurationViewModel.Object, new EventAggregator());
        }

        [Test]
        public void ViewLoaded_Always_SetsActiveIteamAndCallStart()
        {
            _subject.Activate();

            _subject.ActiveItem.Should().Be(_configurationViewModel.Object);
            _configurationViewModel.Verify(c => c.Start(), Times.Once());
        }

        [Test]
        public void Handle_WithGetTokenFromUserMessage_SetsActiveItemToken()
        {
            var mockUserTokenViewModel = new Mock<IUserTokenViewModel>();
            _subject.UserTokenViewModel = mockUserTokenViewModel.Object;

            _subject.Handle(new GetTokenFromUserMessage());

            _subject.ActiveItem.Should().Be(mockUserTokenViewModel.Object);
        }

        [Test]
        public void Handle_WithTokenRequestResultMessage_SetsActiveItemToConfiguration()
        {
            _subject.Handle(new TokenRequestResultMessage(TokenRequestResultMessageStatusEnum.Successful));

            _subject.ActiveItem.Should().Be(_configurationViewModel.Object);
        }

        [Test]
        public void Handle_WithConfigurationCompletedMessage_SetsActiveItemToContextMenuViewModel()
        {
            var mockContextMenuViewModel = new Mock<IContextMenuViewModel>();
            _subject.ContextMenuViewModel = mockContextMenuViewModel.Object;

            _subject.Handle(new ConfigurationCompletedMessage());

            _subject.ActiveItem.Should().Be(mockContextMenuViewModel.Object);
        }
    }
}