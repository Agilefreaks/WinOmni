using System.Windows.Forms;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OmniCommon.Interfaces;
using Omnipaste;

namespace OmnipasteTests
{
    using System;
    using System.Threading.Tasks;
    using CustomizedClickOnce.Common;
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

        private Mock<IClickOnceHelper> _mockClickOnceHelper;

        private Mock<IOmniService> _mockOmniService;

        [SetUp]
        public void Setup()
        {
            _mockOmniclipboard = new Mock<IOmniClipboard> { DefaultValue = DefaultValue.Mock };
            _mockConfigureDialog = new Mock<IConfigureDialog>();
            _mockClickOnceHelper = new Mock<IClickOnceHelper>();
            _mockOmniService = new Mock<IOmniService> { DefaultValue = DefaultValue.Mock };
            Func<bool> startOmniServiceFunc = () => true;
            _mockOmniService.Setup(x => x.Start()).Returns(() =>
                {
                    var task = new Task<bool>(startOmniServiceFunc);
                    task.Start();
                    return task;
                });
            _subject = new MainFormWrapper
                {
                    OmniClipboard = _mockOmniclipboard.Object,
                    ApplicationDeploymentInfoProvider = new MockApplicationDeploymentInfoProvider(),
                    ConfigureForm = _mockConfigureDialog.Object,
                    ClickOnceHelper = _mockClickOnceHelper.Object,
                    OmniService = _mockOmniService.Object
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

        [Test]
        public void PerformInitialization_Always_ShouldCallClickOnceHelperStartupShortcutExists()
        {
            _subject.CallPerformInitializations();

            _mockClickOnceHelper.Verify(x => x.StartupShortcutExists(), Times.Once());
        }

        [Test]
        public void PerformInitialization_StartupShortcutExists_ShouldSetAutoStartCheckboxChecked()
        {
            _mockClickOnceHelper.Setup(x => x.StartupShortcutExists()).Returns(true);
            _subject.CallPerformInitializations();

            _subject.AutoStartCheckbox.Checked.Should().BeTrue();
        }

        [Test]
        public void PerformInitialization_StartupShortcutDoesNotExist_ShouldSetAutoStartCheckboxNotChecked()
        {
            _mockClickOnceHelper.Setup(x => x.StartupShortcutExists()).Returns(false);
            _subject.CallPerformInitializations();

            _subject.AutoStartCheckbox.Checked.Should().BeFalse();
        }
    }
}
