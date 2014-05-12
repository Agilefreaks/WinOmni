using System.Reactive.Subjects;
using System.Threading.Tasks;
using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using OmniCommon.Interfaces;
using OmniCommon.Models;
using OmniSync;

namespace OmniSyncTests
{
    [TestFixture]
    public class OmniSyncServiceTests
    {
        private IOmniSyncService _subject;

        private MoqMockingKernel _kernel;

        private Mock<IWebsocketConnectionFactory> _mockWebsocketConnectionFactory;

        private ReplaySubject<OmniMessage> _replaySubject;

        private Mock<IWebsocketConnection> _mockWebsocketConnection;

        private Mock<IOmniMessageHandler> _mockOmniMessageHandler;

        [SetUp]
        public void SetUp()
        {
            _replaySubject = new ReplaySubject<OmniMessage>();

            _kernel = new MoqMockingKernel();
            _kernel.Bind<IOmniSyncService>().To<OmniSyncService>();

            _mockWebsocketConnection = _kernel.GetMock<IWebsocketConnection>();
            _mockOmniMessageHandler = _kernel.GetMock<IOmniMessageHandler>();
            _mockWebsocketConnectionFactory = _kernel.GetMock<IWebsocketConnectionFactory>();

            _mockWebsocketConnectionFactory.Setup(f => f.Create(It.IsAny<string>()))
                .Returns(Task.Factory.StartNew(() => _mockWebsocketConnection.Object));
            _mockWebsocketConnection.Setup(c => c.Connect()).Returns(_replaySubject);

            _subject = _kernel.Get<IOmniSyncService>();
        }

        [Test]
        public void NewInstance_HasStatusStopped()
        {
            Assert.That(_subject.Status, Is.EqualTo(ServiceStatusEnum.Stopped));
        }

        [Test]
        public async Task Start_SetsTheStatusToStarted()
        {
            await _subject.Start();

            Assert.That(_subject.Status, Is.EqualTo(ServiceStatusEnum.Started));
        }

        [Test]
        public async Task Start_WhenAlreadyStarted_ReturnsRegistrationResultWithTheRegistrationId()
        {
            _mockWebsocketConnection.SetupGet(c => c.RegistrationId).Returns("registration_id");
            await _subject.Start();

            var registrationResult = await _subject.Start();

            Assert.AreEqual("registration_id", registrationResult.Data);
        }

        [Test]
        public async Task Start_ConnectsTheWebsocket()
        {
            await _subject.Start();

            _mockWebsocketConnection.Verify(c => c.Connect(), Times.Once());
        }

        [Test]
        public async Task Start_SubscribesMessageHandlers()
        {
            await _subject.Start();

            _mockOmniMessageHandler.Verify(omh => omh.SubscribeTo(_replaySubject), Times.Once());
        }
    }
}