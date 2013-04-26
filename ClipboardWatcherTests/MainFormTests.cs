using System.Windows.Forms;
using ClipboardWatcher;
using ClipboardWatcher.Core;
using ClipboardWatcher.Core.Services;
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

            public void CallAssureClipboardIsInitialized()
            {
                AssureClipboardIsInitialized();
            }

            public void SetCanSendData(bool value)
            {
                CanSendData = value;
            }
        }

        MainFormWrapper _subject;
        private Mock<IClipboardWrapper> _mockClipboardWrapper;
        private Mock<ICloudClipboard> _mockCloudClipboard;
        private Mock<IActivationDataProvider> _mockActivationDataProvider;
        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void Setup()
        {
            _mockClipboardWrapper = new Mock<IClipboardWrapper>();
            _mockCloudClipboard = new Mock<ICloudClipboard>();
            _mockActivationDataProvider = new Mock<IActivationDataProvider> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService>();
            _subject = new MainFormWrapper
                {
                    ClipboardWrapper = _mockClipboardWrapper.Object,
                    CloudClipboard = _mockCloudClipboard.Object,
                    ActivationDataProvider = _mockActivationDataProvider.Object,
                    ConfigurationService = _mockConfigurationService.Object
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
        public void Ctor_Always_SetsCanSendDataFalse()
        {
            _subject.CanSendData.Should().BeFalse();
        }

        [Test]
        public void AssureClipboardIsInitialized_CloudClipboardIsInitialized_SetsCanSendDataTrue()
        {
            _mockCloudClipboard.Setup(x => x.IsInitialized).Returns(true);

            _subject.CallAssureClipboardIsInitialized();

            _subject.CanSendData.Should().BeTrue();
        }

        [Test]
        public void AssureClipboardIsInitialized_CloudClipboardIsNotInitialized_SetsCanSendDataFalse()
        {
            _mockCloudClipboard.Setup(x => x.IsInitialized).Returns(false);

            _subject.CallAssureClipboardIsInitialized();

            _subject.CanSendData.Should().BeFalse();
        }

        [Test]
        public void WndProc_MessageWasHandledAndHasDataAndCanSendData_CallsCloudClipboardCopyWithTheData()
        {
            var message = new Message();
            var result = new ClipboardMessageHandleResult { MessageHandled = true, MessageData = "tst here" };
            _mockClipboardWrapper.Setup(cw => cw.HandleClipboardMessage(message)).Returns(result);
            _subject.SetCanSendData(true);

            _subject.CallWndProc(message);

            _mockCloudClipboard.Verify(x => x.Copy("tst here"), Times.Once());
        }

        [Test]
        public void WndProc_MessageWasHandledAndHasDataAndCannotSendData_DoesNotCallCloudClipboardCopyWithTheData()
        {
            var message = new Message();
            var result = new ClipboardMessageHandleResult { MessageHandled = true, MessageData = "tst here" };
            _mockClipboardWrapper.Setup(cw => cw.HandleClipboardMessage(message)).Returns(result);

            _subject.CallWndProc(message);

            _mockCloudClipboard.Verify(x => x.Copy("tst here"), Times.Never());
        }
    }
}
