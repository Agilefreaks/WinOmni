namespace OmnipasteTests.Services
{
    using System.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using OmniCommon.Settings;
    using Omnipaste.Services;
    using System.Reactive;

    [TestFixture]
    public class ConfigurationServiceTests
    {
        private Mock<IConfigurationContainer> _mockConfigurationContainer;

        private IConfigurationService _subject;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _mockConfigurationContainer = new Mock<IConfigurationContainer>();
            _subject = new ConfigurationService(_mockConfigurationContainer.Object);
            _testScheduler = new TestScheduler();
        }

        [Test]
        public void SaveAuthSettings_SetsAllSettingsNeededForAuthentication()
        {
            _subject.SaveAuthSettings(new OmnipasteCredentials("token", "refresh token"));

            _mockConfigurationContainer.Verify(cp => cp.SetValue(ConfigurationProperties.AccessToken, "token"));
            _mockConfigurationContainer.Verify(cp => cp.SetValue(ConfigurationProperties.RefreshToken, "refresh token"));
        }

        [Test]
        public void ClearSettings_Always_ClearsAllStoredSettings()
        {
            _subject.ClearSettings();

            _mockConfigurationContainer.Verify(provider => provider.ClearAll(), Times.Once());
        }

        [Test]
        public void ClearSettings_Always_EmitsASettingsChangedDataEventForUserInfo()
        {
            var observer = _testScheduler.CreateObserver<SettingsChangedData>();
            _subject.SettingsChangedObservable.Subscribe(observer);

            _subject.ClearSettings();

            observer.Messages.Any(
                message =>
                message.Value.Kind == NotificationKind.OnNext
                && message.Value.Value.SettingName == ConfigurationProperties.UserInfo).Should().BeTrue();
        }

        [Test]
        public void ClearSettings_Always_EmitsASettingsChangedDataEventForProxyConfiguration()
        {
            var observer = _testScheduler.CreateObserver<SettingsChangedData>();
            _subject.SettingsChangedObservable.Subscribe(observer);

            _subject.ClearSettings();

            observer.Messages.Any(
                message =>
                message.Value.Kind == NotificationKind.OnNext
                && message.Value.Value.SettingName == ConfigurationProperties.ProxyConfiguration).Should().BeTrue();
        }

        [Test]
        public void GetDeviceIdentifier_WhenTereIsAnExistingIdentifierSaved_ReturnsTheExistingIdentifier()
        {
            _mockConfigurationContainer.Setup(cp => cp.GetValue("DeviceIdentifier")).Returns("123456");

            var deviceIdentifier = _subject.DeviceIdentifier;

            deviceIdentifier.Should().Be("123456");
        }

        [Test]
        public void GetDeviceIdentifier_WhenThereIsNoIdentifierSaved_ReturnsNull()
        {
            _mockConfigurationContainer.Setup(cp => cp.GetValue("DeviceIdentifier")).Returns((string)null);

            var deviceIdentifier = _subject.DeviceIdentifier;

            deviceIdentifier.Should().Be(null);
        }

        [Test]
        public void GetProxyConfiguration_ConfigurationProviderDoesNotContainAProxyConfiguration_ReturnsEmptyProxyConfiguration()
        {
            _mockConfigurationContainer.Setup(x => x.GetValue(ConfigurationProperties.ProxyConfiguration))
                .Returns((string)null);

            var proxyConfiguration = _subject.ProxyConfiguration;

            proxyConfiguration.Should().Be(ProxyConfiguration.Empty());
        }

        [Test]
        public void GetProxyConfiguration_ConfigurationProviderHasInvalidData_ReturnsEmptyProxyConfiguration()
        {
            _mockConfigurationContainer.Setup(x => x.GetValue(ConfigurationProperties.ProxyConfiguration))
                .Returns("some-garbled-data");

            var proxyConfiguration = _subject.ProxyConfiguration;

            proxyConfiguration.Should().Be(ProxyConfiguration.Empty());
        }

        [Test]
        public void GetProxyConfiguration_Always_CanReadAConfigurationPreviouslySaved()
        {
            string valueSet = null;
            var proxyConfiguration = new ProxyConfiguration
                                         {
                                             Address = "testA",
                                             Password = "testP",
                                             Username = "testU",
                                             Port = 12,
                                             Type = ProxyTypeEnum.Socks4
                                         };
            _mockConfigurationContainer.Setup(
                x => x.SetValue(ConfigurationProperties.ProxyConfiguration, It.IsAny<string>()))
                .Callback<string, string>((key, value) => valueSet = value);

            _subject.ProxyConfiguration = proxyConfiguration;
            valueSet.Should().NotBeNull();
            _mockConfigurationContainer.Setup(x => x.GetValue(ConfigurationProperties.ProxyConfiguration)).Returns(valueSet);

            _subject.ProxyConfiguration.Should().Be(proxyConfiguration);
        }

        [Test]
        public void GetUserInfo_WhenUserInfoWasPreviouslySaved_ReturnsUserInfo()
        {
            string valueSet = null;
            var userInfo =
                UserInfo.BeginBuild()
                    .WithEmail("test@user.com")
                    .WithFirstName("First")
                    .WithLastName("Last")
                    .WithImageUrl("http://example.com")
                    .Build();

            _mockConfigurationContainer.Setup(
                x => x.SetValue(ConfigurationProperties.UserInfo, It.IsAny<string>()))
                .Callback<string, string>((key, value) => valueSet = value);

            _subject.UserInfo = userInfo;
            valueSet.Should().NotBeNull();
            _mockConfigurationContainer.Setup(x => x.GetValue(ConfigurationProperties.UserInfo)).Returns(valueSet);

            _subject.UserInfo.Should().NotBe(userInfo);
            _subject.UserInfo.Email.Should().Be(userInfo.Email);
            _subject.UserInfo.FirstName.Should().Be(userInfo.FirstName);
            _subject.UserInfo.LastName.Should().Be(userInfo.LastName);
            _subject.UserInfo.ImageUrl.Should().Be(userInfo.ImageUrl);
        }

        [Test]
        public void GetDeviceKeyPair_WhenDeviceKeyPairWasPreviouslySaved_ReturnsDeviceKeyPair()
        {
            string valueSet = null;
            var keyPair = new KeyPair
            {
                Public = "public",
                Private = "private"
            };

            _mockConfigurationContainer.Setup(
                x => x.SetValue(ConfigurationProperties.DeviceKeyPair, It.IsAny<string>()))
                .Callback<string, string>((key, value) => valueSet = value);

            _subject.DeviceKeyPair = keyPair;
            valueSet.Should().NotBeNull();
            _mockConfigurationContainer.Setup(x => x.GetValue(ConfigurationProperties.DeviceKeyPair)).Returns(valueSet);

            _subject.DeviceKeyPair.Should().NotBe(keyPair);
            _subject.DeviceKeyPair.Public.Should().Be(keyPair.Public);
            _subject.DeviceKeyPair.Private.Should().Be(keyPair.Private);
        }

        [Test]
        public void IsSMSSuffixEnabled_ConfigurationProviderHasAInvalidEntry_ReturnsTrue()
        {
            _mockConfigurationContainer.Setup(x => x.GetValue(ConfigurationProperties.SMSSuffixEnabled)).Returns("test");

            _subject.IsSMSSuffixEnabled.Should().BeTrue();
        }

        [Test]
        public void IsSMSSuffixEnabled_ConfigurationProviderHasAFalseEntryStored_ReturnsFalse()
        {
            _mockConfigurationContainer.Setup(x => x.GetValue(ConfigurationProperties.SMSSuffixEnabled)).Returns("false");

            _subject.IsSMSSuffixEnabled.Should().BeFalse();
        }

        [Test]
        public void IsSMSSuffixEnabled_ConfigurationProviderHasATrueEntryStored_ReturnsTrue()
        {
            _mockConfigurationContainer.Setup(x => x.GetValue(ConfigurationProperties.SMSSuffixEnabled)).Returns("true");

            _subject.IsSMSSuffixEnabled.Should().BeTrue();
        }

        [Test]
        public void SetIsSMSSuffixEnabled_Always_SetsItAsTextOnTheConfigurationProvider()
        {
            _subject.IsSMSSuffixEnabled = true;

            _mockConfigurationContainer.Verify(x => x.SetValue(ConfigurationProperties.SMSSuffixEnabled, "True"));
        }

        [Test]
        public void SetDeviceIdentifier_Always_SavesTheGivenValue()
        {
            _subject.DeviceIdentifier = "someId";

            _mockConfigurationContainer.Verify(x => x.SetValue(ConfigurationProperties.DeviceIdentifier, "someId"));
        }

        [Test]
        public void SetUserInfo_Always_TriggersOnNext()
        {
            var observer = _testScheduler.CreateObserver<SettingsChangedData>();
            _subject.SettingsChangedObservable.Subscribe(observer);

            _subject.UserInfo = new UserInfo();

            observer.Messages.Count.Should().Be(1);
            observer.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            observer.Messages[0].Value.Value.SettingName.Should().Be(ConfigurationProperties.UserInfo);
        }

        [Test]
        public void IsNewDevice_NoDeviceIdIsStored_ReturnsTrue()
        {
            _mockConfigurationContainer.Setup(x => x.GetValue(ConfigurationProperties.DeviceId))
                .Returns(string.Empty);

            _subject.IsNewDevice.Should().BeTrue();
        }

        [Test]
        public void IsNewDevice_ADeviceIdIsStoredAndNoDeviceIdChangesHaveOccured_ReturnsFalse()
        {
            _mockConfigurationContainer.Setup(x => x.GetValue(ConfigurationProperties.DeviceId))
                .Returns("someId");

            _subject.IsNewDevice.Should().BeFalse();
        }

        [Test]
        public void IsNewDevice_ADeviceIdIsStoredButACallToSaveANewDeviceIdHasBeenMade_ReturnsTrue()
        {
            _mockConfigurationContainer.Setup(x => x.GetValue(ConfigurationProperties.DeviceId))
                .Returns("someId");
            _subject.DeviceId = "someNewId";

            _subject.IsNewDevice.Should().BeTrue();
        }
    }
}