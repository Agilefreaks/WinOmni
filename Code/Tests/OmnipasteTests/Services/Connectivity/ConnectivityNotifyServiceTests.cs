namespace OmnipasteTests.Services.Connectivity
{
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Services.Connectivity;

    [TestFixture]
    public class ConnectivityNotifyServiceTests
    {
        private MoqMockingKernel _kernel;

        private IConnectivityNotifyService _subject;

        private Mock<IConnectivityHelper> _mockConnectivityHelper;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _mockConnectivityHelper = _kernel.GetMock<IConnectivityHelper>();
            _kernel.Bind<IConnectivityHelper>().ToConstant(_mockConnectivityHelper.Object);
        }

        [Test]
        public void Ctor_SetsThePreviousStateToTheCurrentConnectivityStatus()
        {
            _mockConnectivityHelper.SetupGet(ch => ch.InternetConnected).Returns(true);
            
            _subject = _kernel.Get<ConnectivityNotifyService>();

            _subject.PreviouslyConnected.Should().Be(true);
        }
    }
}