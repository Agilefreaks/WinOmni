namespace OmnipasteTests.UserToken
{
    using Caliburn.Micro;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.UserToken;

    [TestFixture]
    public class UserTokenViewModelTests
    {
        private IUserTokenViewModel _subject;

        private Mock<IEventAggregator> _eventAggregator;

        [SetUp]
        public void SetUp()
        {
            _eventAggregator = new Mock<IEventAggregator>();
            _subject = new UserTokenViewModel(_eventAggregator.Object);
        }

        [Test]
        public void Cancel_Alwyas_CallsPublishWithCanceled()
        {
            _subject.Cancel();

            _eventAggregator.Verify(x => x.Publish(It.Is<TokenRequestResultMessage>(t => t.Status == TokenRequestResultMessageStatusEnum.Canceled)));
        }

        [Test]
        public void Ok_Always_CallsPublishWithOkAndToken()
        {
            _subject.Token = "42";

            _subject.Ok();

            _eventAggregator.Verify(x => x.Publish(It.Is<TokenRequestResultMessage>(t => t.Status == TokenRequestResultMessageStatusEnum.Successful && t.Token == "42")));
        }
    }
}