using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OmniCommon;
using OmniCommon.Interfaces;
using OmniCommon.Services;

namespace OmniCommonTests
{
    [TestFixture]
    public class OmniServiceTests
    {
        OmniService _subject;
        private Mock<ILocalClipboard> _mockLocalClipboard;
        private Mock<IOmniClipboard> _mockOmniClipboard;

        [SetUp]
        public void Setup()
        {
            _mockLocalClipboard = new Mock<ILocalClipboard>();
            _mockOmniClipboard = new Mock<IOmniClipboard>();
            _subject = new OmniService(_mockLocalClipboard.Object, _mockOmniClipboard.Object);
            _subject.Start();
        }

        [Test]
        public void OnReceivingRemoteData_Always_WillNotSendItBack()
        {
            var tasks = new Task[1];
            _mockLocalClipboard.Setup(mock => mock.SendData("test-data"))
                               .Callback<string>(data =>
                                   {
                                       tasks[0] = (Task.Factory.StartNew(() =>
                                       {
                                           Thread.Sleep(1000);
                                           _mockLocalClipboard.Raise(mock => mock.DataReceived += null, new ClipboardEventArgs(data));
                                       }));
                                   });
            _mockOmniClipboard.Raise(mock => mock.DataReceived += null, new ClipboardEventArgs("test-data"));

            Task.WaitAll(tasks);

            _mockLocalClipboard.Verify(mock => mock.SendData("test-data"), Times.Once());
            _mockOmniClipboard.Verify(mock => mock.SendData(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void OnReceivingLocalData_Always_WillNotSendItBack()
        {
            var tasks = new Task[1];
            _mockOmniClipboard.Setup(mock => mock.SendData("test-data")).Callback<string>(data =>
                {
                    tasks[0] = (Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(1000);
                            _mockOmniClipboard.Raise(mock => mock.DataReceived += null, new ClipboardEventArgs(data));
                        }));
                });
            _mockLocalClipboard.Raise(mock => mock.DataReceived += null, new ClipboardEventArgs("test-data"));

            Task.WaitAll(tasks);

            _mockOmniClipboard.Verify(mock => mock.SendData("test-data"), Times.Once());
            _mockLocalClipboard.Verify(mock => mock.SendData(It.IsAny<string>()), Times.Never());
        }
    }
}
