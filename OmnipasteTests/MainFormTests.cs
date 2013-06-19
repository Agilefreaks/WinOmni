namespace OmnipasteTests
{
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using CustomizedClickOnce.Common;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using OmniCommon.Services;
    using Omnipaste;
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

        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<IOmniService> _mockOmniService;

        private CommunicationSettings _communicationSettings;

        [SetUp]
        public void Setup()
        {
            _mockOmniclipboard = new Mock<IOmniClipboard> { DefaultValue = DefaultValue.Mock };
            _mockConfigureDialog = new Mock<IConfigureDialog>();
            _mockClickOnceHelper = new Mock<IClickOnceHelper>();
            _mockOmniService = new Mock<IOmniService> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService>();
            _communicationSettings = new CommunicationSettings();
            _mockConfigurationService.Setup(x => x.CommunicationSettings).Returns(_communicationSettings);
            _mockOmniService = new Mock<IOmniService>();
            SetupMockOmniService();
            _subject = new MainFormWrapper
                {
                    OmniClipboard = _mockOmniclipboard.Object,
                    ApplicationDeploymentInfoProvider = new MockApplicationDeploymentInfoProvider(),
                    ConfigureForm = _mockConfigureDialog.Object,
                    ClickOnceHelper = _mockClickOnceHelper.Object,
                    ConfigurationService = _mockConfigurationService.Object,
                    OmniService = _mockOmniService.Object
                };
        }

        [Test]
        public void PerformInitialization_ChannelIsLoaded_ShouldSetTheNotifyIconVisible()
        {
            _communicationSettings.Channel = "test";

            _subject.CallPerformInitializations();

            _subject.IsNotificationIconVisible.Should().BeTrue();
        }

        [Test]
        public void PerformInitialization_ChannelIsNotLoaded_ShouldCallConfigureFormShowDialog()
        {
            _communicationSettings.Channel = null;

            _subject.CallPerformInitializations();

            _mockConfigureDialog.Verify(x => x.ShowDialog(), Times.Once());
        }

        [Test]
        public void PerformInitialization_ChannelIsNotLoadedAndConfigureFormFails_DoesNotCallOmniServiceStart()
        {
            _communicationSettings.Channel = null;
            _mockConfigureDialog.Setup(x => x.Succeeded).Returns(false);

            _subject.CallPerformInitializations();

            _mockOmniService.Verify(x => x.Start(), Times.Never());
        }

        [Test]
        public void PerformInitialization_ChannelIsNotLoadedAndConfigureFormSucceeds_CallsOmniServiceStart()
        {
            _communicationSettings.Channel = null;
            _mockConfigureDialog.Setup(x => x.Succeeded).Returns(true);

            _subject.CallPerformInitializations();

            _mockOmniService.Verify(x => x.Start(), Times.Once());
        }

        [Test]
        public void OnActivate_CanLoadConfiguration_ShouldSetADefferingLoggerOnTheOmniClipboard()
        {
            _communicationSettings.Channel = "test";

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
        public void PerformInitialization_WhenConfigurationLoaded_ShouldCallClickOnceHelperStartupShortcutExists()
        {
            _communicationSettings.Channel = "42";
            _subject.CallPerformInitializations();

            _mockClickOnceHelper.Verify(x => x.StartupShortcutExists(), Times.Once());
        }

        [Test]
        public void PerformInitialization_StartupShortcutExistsAndConfigurationLoaded_ShouldSetAutoStartCheckboxChecked()
        {
            _mockClickOnceHelper.Setup(x => x.StartupShortcutExists()).Returns(true);
            _communicationSettings.Channel = "42";
            _subject.CallPerformInitializations();

            _subject.AutoStartButton.Checked.Should().BeTrue();
        }

        [Test]
        public void PerformInitialization_StartupShortcutDoesNotExist_ShouldSetAutoStartCheckboxNotChecked()
        {
            _mockClickOnceHelper.Setup(x => x.StartupShortcutExists()).Returns(false);
            _subject.CallPerformInitializations();

            _subject.AutoStartButton.Checked.Should().BeFalse();
        }

        private void SetupMockOmniService()
        {
            _mockOmniService.Setup(x => x.Start()).Returns(
                () =>
                {
                    var task = new Task<bool>(() => false);
                    task.Start();
                    return task;
                });
        }
    }
}
