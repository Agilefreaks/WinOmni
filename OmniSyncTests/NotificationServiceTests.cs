using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using OmniSync;
using WampSharp;
using WampSharp.Auxiliary.Client;

namespace OmniSyncTests
{
    [TestFixture]
    public class NotificationServiceTests
    {
        private INotificationService _subject;

        private MoqMockingKernel _kernel;

        private Mock<IWampChannelFactory<JToken>> _mockWampChannelFactory;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _kernel.Bind<INotificationService>().To<NotificationService>();
            _mockWampChannelFactory = _kernel.GetMock<IWampChannelFactory<JToken>>();
            _mockWampChannelFactory.Setup(f => f.CreateChannel("asdf"));
            _subject = _kernel.Get<INotificationService>();
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
    }
}