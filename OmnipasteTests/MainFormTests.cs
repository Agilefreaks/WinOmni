using System.Windows.Forms;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OmniCommon.Interfaces;
using Omnipaste;

namespace OmnipasteTests
{
    using Omnipaste.Services;

    [TestFixture]
    public class MainFormTests
    {
        private class MainFormWrapper : MainForm
        {
            public void CallWndProc(Message message)
            {
                WndProc(ref message);
            }

            public void CallPerformInitializations()
            {
                PerformInitializations();
            }
        }

        private MainFormWrapper _subject;

        private Mock<IOmniClipboard> _mockOmniclipboard;

        private Mock<IConfigureDialog> _mockConfigureDialog;

        [SetUp]
        public void Setup()
        {
            _mockOmniclipboard = new Mock<IOmniClipboard> { DefaultValue = DefaultValue.Mock };
            _mockConfigureDialog = new Mock<IConfigureDialog>();
            _subject = new MainFormWrapper
                {
                    OmniClipboard = _mockOmniclipboard.Object,
                    ApplicationDeploymentInfoProvider = new MockApplicationDeploymentInfoProvider(),
                    ConfigureForm = _mockConfigureDialog.Object
                };
        }

        [Test]
        public void PerformInitialization_Always_ShouldSetTheNotifyIconVisible()
        {
            _subject.CallPerformInitializations();

            _subject.IsNotificationIconVisible.Should().BeTrue();
        }

        [Test]
        public void OnActivate_Always_ShouldSetADefferingLoggerOnTheOmniClipboard()
        {
            _subject.CallPerformInitializations();

            _mockOmniclipboard.VerifySet(x => x.Logger = It.IsAny<SimpleDefferingLogger>());
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
