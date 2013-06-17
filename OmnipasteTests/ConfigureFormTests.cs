namespace OmnipasteTests
{
    using System;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using Omnipaste;
    using Omnipaste.Services;

    [TestFixture]
    public class ConfigureFormTests
    {
        private ConfigureForm _subject;
        private Mock<IActivationDataProvider> _mockActivationDataProvider;
        private Mock<IConfigurationService> _mockConfigurationService;
        private Mock<IApplicationDeploymentInfoProvider> _mockApplicationDeploymentInfoProvider;
        private Mock<ITokenInputForm> _mockTokenInputForm;

        [SetUp]
        public void Setup()
        {
            _mockActivationDataProvider = new Mock<IActivationDataProvider> { DefaultValue = DefaultValue.Mock };
            _mockActivationDataProvider.Setup(x => x.GetActivationData(It.IsAny<string>())).Returns(new ActivationData { Email = "testEmail" });
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockApplicationDeploymentInfoProvider = new Mock<IApplicationDeploymentInfoProvider> { DefaultValue = DefaultValue.Mock };
            _mockTokenInputForm = new Mock<ITokenInputForm> { DefaultValue = DefaultValue.Mock };
            _subject = new ConfigureForm
                {
                    ActivationDataProvider = _mockActivationDataProvider.Object,
                    ConfigurationService = _mockConfigurationService.Object,
                    ApplicationDeploymentInfoProvider = _mockApplicationDeploymentInfoProvider.Object,
                    TokenInputForm = _mockTokenInputForm.Object
                };
        }

        [Test]
        public void AssureClipboardIsInitialized_ActivationUriPresent_CallsActivationDataProviderGetActivationDataWithTheToken()
        {
            _mockApplicationDeploymentInfoProvider.Setup(x => x.HasValidActivationUri).Returns(true);
            _mockApplicationDeploymentInfoProvider.Setup(x => x.ActivationUri).Returns(new Uri("http://test.com?token=testToken"));

            _subject.AssureClipboardIsInitialized();

            _mockActivationDataProvider.Verify(x => x.GetActivationData("testToken"));
        }

        [Test]
        public void AssureClipboardIsInitialized_ActivationUriNotPresent_GetTheTokenFromTheTokenInputForm()
        {
            _mockApplicationDeploymentInfoProvider.Setup(x => x.HasValidActivationUri).Returns(false);
            _mockTokenInputForm.SetupGet(x => x.Token).Returns("testToken");

            _subject.AssureClipboardIsInitialized();

            _mockTokenInputForm.Verify(x => x.ShowDialog(), Times.Once());
            _mockActivationDataProvider.Verify(x => x.GetActivationData("testToken"));
        }

        [Test]
        public void AssureClipboardIsInitialized_Always_CallsUpdateCommunicationChannelWithTheResultOfGetActivationData()
        {
            _mockActivationDataProvider.Setup(x => x.GetActivationData(It.IsAny<string>())).Returns(new ActivationData { Email = "testC" });

            _subject.AssureClipboardIsInitialized();

            _mockConfigurationService.Verify(x => x.UpdateCommunicationChannel("testC"));
        }

        [Test]
        public void AssureClipboardIsInitialized_IfTheClipboardInitializationFailsAndMaxRetriesHasNotBeenReached_RetriesToGetTheActivationData()
        {
            _mockActivationDataProvider.Setup(x => x.GetActivationData(It.IsAny<string>())).Returns(new ActivationData());

            _subject.AssureClipboardIsInitialized();

            _mockActivationDataProvider.Verify(x => x.GetActivationData(It.IsAny<string>()), Times.Exactly(1 + ConfigureForm.MaxRetryCount));
        }
    }
}