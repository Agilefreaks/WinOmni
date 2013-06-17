using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OmniCommon.Interfaces;
using OmniCommon.Services;

namespace OmniCommonTests
{
    using FluentAssertions;

    [TestFixture]
    public class OmniServiceTests
    {
        private OmniService _subject;
        private Mock<ILocalClipboard> _mockLocalClipboard;
        private Mock<IOmniClipboard> _mockOmniClipboard;

        [SetUp]
        public void Setup()
        {
            _mockLocalClipboard = new Mock<ILocalClipboard>();
            _mockOmniClipboard = new Mock<IOmniClipboard>();
            _mockLocalClipboard.Setup(x => x.Initialize()).Returns(() => Task.Factory.StartNew(() => true));
            _mockOmniClipboard.Setup(x => x.Initialize()).Returns(() => Task.Factory.StartNew(() => true));
            _subject = new OmniService(_mockLocalClipboard.Object, _mockOmniClipboard.Object);
        }

        [Test]
        public void OnReceivingRemoteData_Always_WillSendItToLocalClipboard()
        {
            var startTask = _subject.Start();
            Task.WaitAll(startTask);

            _subject.DataReceived(CreateClipboardDataFrom(_mockOmniClipboard.Object));

            _mockLocalClipboard.Verify(mock => mock.PutData("test-data"), Times.Once());
        }

        [Test]
        public void OnReceivingLocalData_Always_WillSendItToRemoteClipboard()
        {
            var startTask = _subject.Start();
            Task.WaitAll(startTask);

            _subject.DataReceived(CreateClipboardDataFrom(_mockLocalClipboard.Object));

            _mockOmniClipboard.Verify(mock => mock.PutData("test-data"), Times.Once());
        }

        [Test]
        public void OnReceivingRemoteData_Always_WillNotSendItBack()
        {
            var startTask = _subject.Start();
            Task.WaitAll(startTask);
            Task echoTask = null;
            _mockLocalClipboard.Setup(mock => mock.PutData("test-data")).Callback<string>(data =>
                {
                    echoTask = Task.Factory.StartNew(() =>
                            {
                                Thread.Sleep(500);
                                _subject.DataReceived(CreateClipboardDataFrom(_mockLocalClipboard.Object));
                            });
                });

            _subject.DataReceived(CreateClipboardDataFrom(_mockOmniClipboard.Object));

            echoTask.Should().NotBeNull();
            Task.WaitAll(echoTask);
            _mockOmniClipboard.Verify(mock => mock.PutData(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void OnReceivingLocalData_Always_WillNotSendItBack()
        {
            var startTask = _subject.Start();
            Task.WaitAll(startTask);
            Task echoTask = null;
            _mockOmniClipboard.Setup(mock => mock.PutData("test-data")).Callback<string>(data =>
                {
                    echoTask = Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(1000);
                            _subject.DataReceived(CreateClipboardDataFrom(_mockOmniClipboard.Object));
                        });
                });

            _subject.DataReceived(CreateClipboardDataFrom(_mockLocalClipboard.Object));

            echoTask.Should().NotBeNull();
            Task.WaitAll(echoTask);
            _mockLocalClipboard.Verify(mock => mock.PutData(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Start_StartIsInProgress_ReturnsTheSameTask()
        {
            _mockLocalClipboard.Setup(x => x.Initialize()).Callback(() =>
                {
                    Thread.Sleep(500);
                    Task.Factory.StartNew(() => true);
                });
            _mockOmniClipboard.Setup(x => x.Initialize()).Callback(() => Task.Factory.StartNew(() => true));

            var startTask = _subject.Start();
            var startTask2 = _subject.Start();

            startTask2.Should().BeSameAs(startTask);
        }

        [Test]
        public void Start_StartIsNotInProgress_ReturnsANewTask()
        {
            _mockLocalClipboard.Setup(x => x.Initialize()).Returns(() => Task.Factory.StartNew(() => true));
            _mockOmniClipboard.Setup(x => x.Initialize()).Returns(() => Task.Factory.StartNew(() => true));

            var startTask = _subject.Start();
            Task.WaitAll(startTask);
            var startTask2 = _subject.Start();

            startTask2.Should().NotBeSameAs(startTask);
        }

        [Test]
        public void Stop_Always_CallsLocalClipboardRemoveDataReceiver()
        {
            _subject.Stop();

            _mockLocalClipboard.Verify(x => x.RemoveDataReceive(_subject), Times.Once());
        }

        [Test]
        public void Stop_Always_CallsOmniClipboardRemoveDataReceiver()
        {
            _subject.Stop();

            _mockOmniClipboard.Verify(x => x.RemoveDataReceive(_subject), Times.Once());
        }

        [Test]
        public void Stop_Always_CallsLocalClipboardDispose()
        {
            _subject.Stop();

            _mockLocalClipboard.Verify(x => x.Dispose(), Times.Once());
        }

        [Test]
        public void Stop_Always_CallsOmniClipboardDispose()
        {
            _subject.Stop();

            _mockOmniClipboard.Verify(x => x.Dispose(), Times.Once());
        }

        private static IClipboardData CreateClipboardDataFrom(IClipboard clipboard)
        {
            return new ClipboardData(clipboard, "test-data");
        }
    }
}
