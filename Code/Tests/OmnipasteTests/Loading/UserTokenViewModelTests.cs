namespace OmnipasteTests.Loading
{
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Loading.UserToken;

    [TestFixture]
    public class UserTokenViewModelTests
    {
        [Test]
        public void OnActivate_Always_SetsActivationCodeToEmptyString()
        {
            var mockEventAggregator = new Mock<IEventAggregator>();
            IUserTokenViewModel subject = new UserTokenViewModel(mockEventAggregator.Object)
                                          {
                                              ActivationCode = "previous activation code"
                                          };

            subject.Activate();

            subject.ActivationCode.Should().Be(string.Empty);
        }
    }
}