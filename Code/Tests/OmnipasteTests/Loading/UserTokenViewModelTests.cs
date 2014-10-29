namespace OmnipasteTests.Loading
{
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using Omnipaste.Loading.UserToken;

    [TestFixture]
    public class UserTokenViewModelTests
    {
        private Mock<IApplicationService> _mockApplicationService;

        [Test]
        public void OnActivate_Always_SetsActivationCodeToEmptyString()
        {
            var mockEventAggregator = new Mock<IEventAggregator>();
            _mockApplicationService = new Mock<IApplicationService>();
            IUserTokenViewModel subject = new UserTokenViewModel(mockEventAggregator.Object, _mockApplicationService.Object)
                                          {
                                              ActivationCode = "previous activation code"
                                          };

            subject.Activate();

            subject.ActivationCode.Should().Be(string.Empty);
        }
    }
}