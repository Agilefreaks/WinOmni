using System.Windows.Forms;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OmniCommon.Interfaces;
using OmniCommon.Services;
using Omnipaste;
using PubNubClipboard;
using WindowsClipboard.Interfaces;

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
        }

        MainFormWrapper _subject;
        private Mock<IWindowsClipboard> _mockClipboardWrapper;
        private Mock<IPubNubClipboard> _mockOmniclipboard;
        private Mock<IActivationDataProvider> _mockActivationDataProvider;
        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void Setup()
        {
            _mockClipboardWrapper = new Mock<IWindowsClipboard> { DefaultValue = DefaultValue.Mock };
            _mockOmniclipboard = new Mock<IPubNubClipboard> { DefaultValue = DefaultValue.Mock };
            _mockActivationDataProvider = new Mock<IActivationDataProvider> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService> { DefaultValue = DefaultValue.Mock };
            _subject = new MainFormWrapper
                {
                    WindowsClipboard = _mockClipboardWrapper.Object,
                    OmniClipboard = _mockOmniclipboard.Object,
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
        public void WndProc_Always_ShouldRaiseHandleClipboardMessage()
        {
            var message = new Message();
            var callCount = 0;
            _subject.HandleClipboardMessage += (ref Message status) =>
                {
                    callCount++;
                    status.Should().Be(message);
                    return true;
                };

            _subject.CallWndProc(message);

            callCount.Should().Be(1);
        }
    }
}
