using System.Windows.Forms;
using ClipboardWrapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Omniclipboard;
using Omniclipboard.Services;
using Omnipaste;

namespace OmnipasteTests
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
        private Mock<IOmniclipboard> _mockOmniclipboard;
        private Mock<IActivationDataProvider> _mockActivationDataProvider;
        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void Setup()
        {
            _mockClipboardWrapper = new Mock<IClipboardWrapper>();
            _mockOmniclipboard = new Mock<IOmniclipboard>();
            _mockActivationDataProvider = new Mock<IActivationDataProvider> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService>();
            _subject = new MainFormWrapper
                {
                    ClipboardWrapper = _mockClipboardWrapper.Object,
                    Omniclipboard = _mockOmniclipboard.Object,
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
        public void AssureClipboardIsInitialized_OmniclipboardIsInitialized_SetsCanSendDataTrue()
        {
            _mockOmniclipboard.Setup(x => x.IsInitialized).Returns(true);

            _subject.CallAssureClipboardIsInitialized();

            _subject.CanSendData.Should().BeTrue();
        }

        [Test]
        public void AssureClipboardIsInitialized_OmniclipboardIsNotInitialized_SetsCanSendDataFalse()
        {
            _mockOmniclipboard.Setup(x => x.IsInitialized).Returns(false);

            _subject.CallAssureClipboardIsInitialized();

            _subject.CanSendData.Should().BeFalse();
        }

        [Test]
        public void WndProc_MessageWasHandledAndHasDataAndCanSendData_CallsOmniclipboardCopyWithTheData()
        {
            var message = new Message();
            var result = new ClipboardMessageHandleResult { MessageHandled = true, MessageData = "tst here" };
            _mockClipboardWrapper.Setup(cw => cw.HandleClipboardMessage(message)).Returns(result);
            _subject.SetCanSendData(true);

            _subject.CallWndProc(message);

            _mockOmniclipboard.Verify(x => x.Copy("tst here"), Times.Once());
        }

        [Test]
        public void WndProc_MessageWasHandledAndHasDataAndCannotSendData_DoesNotCallOmniclipboardCopyWithTheData()
        {
            var message = new Message();
            var result = new ClipboardMessageHandleResult { MessageHandled = true, MessageData = "tst here" };
            _mockClipboardWrapper.Setup(cw => cw.HandleClipboardMessage(message)).Returns(result);

            _subject.CallWndProc(message);

            _mockOmniclipboard.Verify(x => x.Copy("tst here"), Times.Never());
        }
    }
}
