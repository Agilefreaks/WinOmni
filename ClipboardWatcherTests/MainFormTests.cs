using System.Windows.Forms;
using ClipboardWatcher;
using ClipboardWatcher.Core;
using ClipboardWrapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ClipboardWatcherTests
{
    [TestFixture]
    public class MainFormTests
    {
        private class MainFormWrapper : MainForm
        {
            public void CallWndProc(Message message)
            {
                base.WndProc(ref message);
            }
        }

        MainFormWrapper _subject;
        private Mock<IClipboardWrapper> _mockClipboardWrapper;
        private Mock<ICloudClipboard> _mockCloudClipboard;

        [SetUp]
        public void Setup()
        {
            _mockClipboardWrapper = new Mock<IClipboardWrapper>();
            _mockCloudClipboard = new Mock<ICloudClipboard>();
            _subject = new MainFormWrapper
                {
                    ClipboardWrapper = _mockClipboardWrapper.Object,
                    CloudClipboard = _mockCloudClipboard.Object
                };
        }

        [Test]
        public void OnActivate_Always_ShouldSetTheNotifyIconVisible()
        {
            _subject.Activate();

            _subject.IsNotificationIconVisible.Should().BeTrue();
        }

        [Test]
        public void WndProc_Always_ShouldCallClipboardWrapperHandleClipboardMessage()
        {
            var message = new Message();
            _mockClipboardWrapper.Setup(cw => cw.HandleClipboardMessage(message)).Returns(new ClipboardMessageHandleResult());

            _subject.CallWndProc(message);

            _mockClipboardWrapper.Verify(cw => cw.HandleClipboardMessage(message), Times.Once());
        }

        [Test]
        public void WndProc_MessageWasHandledAndHasData_CallsCloudClipboardCopyWithTheData()
        {
            var message = new Message();
            var result = new ClipboardMessageHandleResult { MessageHandled = true, MessageData = "tst here" };
            _mockClipboardWrapper.Setup(cw => cw.HandleClipboardMessage(message)).Returns(result);

            _subject.CallWndProc(message);

            _mockCloudClipboard.Verify(x => x.Copy("tst here"), Times.Once());
        }
    }
}
