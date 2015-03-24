namespace OmnipasteTests.Presenters.Factories
{
    using System;
    using System.Reactive.Linq;
    using Clipboard.Models;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Presenters.Factories;
    using Omnipaste.Properties;
    using Omnipaste.Services;

    [TestFixture]
    public class ActivityPresenterFactoryTests
    {
        private Mock<IPhoneCallPresenterFactory> _mockPhoneCallPresenterFactory;

        private Mock<ISmsMessagePresenterFactory> _mockSmsMessagePresenterFactory;

        private ActivityPresenterFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _mockPhoneCallPresenterFactory = new Mock<IPhoneCallPresenterFactory>();
            _mockSmsMessagePresenterFactory = new Mock<ISmsMessagePresenterFactory>();
            _factory = new ActivityPresenterFactory(_mockPhoneCallPresenterFactory.Object, _mockSmsMessagePresenterFactory.Object);
        }

        [Test]
        public void Create_WithClipping_SetsTypeToClipping()
        {
            var activityPresenter = _factory.Create(new ClippingModel()).Wait();

            activityPresenter.Type.Should().Be(ActivityTypeEnum.Clipping);
        }

        [Test]
        public void Create_WithClipping_SetsContent()
        {
            var activityPresenter = _factory.Create(new ClippingModel { Content = "some" }).Wait();

            activityPresenter.Content.Should().Be("some");
        }

        [Test]
        public void Create_WithClippingAndSourceIsCloud_SetsDeviceToCloud()
        {
            var activityPresenter =
                _factory.Create(new ClippingModel { Source = Clipping.ClippingSourceEnum.Cloud }).Wait();

            activityPresenter.Device.Should().Be(Resources.FromCloud);
        }

        [Test]
        public void Create_WithClipping_SetsSourceId()
        {
            var activityPresenter = _factory.Create(new ClippingModel { UniqueId = "42" }).Wait();

            activityPresenter.SourceId.Should().Be("42");
        }

        [Test]
        public void Create_WithLocalPhoneCall_SetsTypeToCall()
        {
            var localPhoneCall = new LocalPhoneCall();
            SetupPhoneCallPresenterFactory<LocalPhoneCall, LocalPhoneCallPresenter>(localPhoneCall);
            var activityPresenter = _factory.Create(localPhoneCall).Wait();

            activityPresenter.Type.Should().Be(ActivityTypeEnum.Call);
        }

        [Test]
        public void Create_WithLocalPhoneCall_SetsDeviceToLocal()
        {
            var localPhoneCall = new LocalPhoneCall();
            SetupPhoneCallPresenterFactory<LocalPhoneCall, LocalPhoneCallPresenter>(localPhoneCall);
            var activityPresenter = _factory.Create(localPhoneCall).Wait();

            activityPresenter.Device.Should().Be(Resources.FromLocal);
        }

        [Test]
        public void Create_WithRemotePhoneCall_SetsDeviceToRemote()
        {
            var remotePhoneCall = new RemotePhoneCall();
            SetupPhoneCallPresenterFactory<RemotePhoneCall, RemotePhoneCallPresenter>(remotePhoneCall);
            var activityPresenter = _factory.Create(remotePhoneCall).Wait();

            activityPresenter.Device.Should().Be(Resources.FromCloud);
        }

        [Test]
        public void Create_WithPhoneCall_SetsContactInfo()
        {
            var remotePhoneCall = new RemotePhoneCall();
            var contactInfoPresenter = new ContactInfoPresenter(new ContactInfo());
            SetupPhoneCallPresenterFactory<RemotePhoneCall, RemotePhoneCallPresenter>(remotePhoneCall, contactInfoPresenter);
            var activityPresenter = _factory.Create(remotePhoneCall).Wait();

            ContactInfoPresenter contactInfo = activityPresenter.ExtraData.ContactInfo;
            contactInfo.Should().Be(contactInfoPresenter);
        }

        [Test]
        public void Create_WithLocalSmsMessage_SetsTypeToMessage()
        {
            var localSmsMessage = new LocalSmsMessage();
            SetupSmsMessagePresenterFactory<LocalSmsMessage, LocalSmsMessagePresenter>(localSmsMessage);
            var activityPresenter = _factory.Create(localSmsMessage).Wait();

            activityPresenter.Type.Should().Be(ActivityTypeEnum.Message);
        }

        [Test]
        public void Create_WithLocalSmsMessage_SetsDeviceToLocal()
        {
            var localSmsMessage = new LocalSmsMessage();
            SetupSmsMessagePresenterFactory<LocalSmsMessage, LocalSmsMessagePresenter>(localSmsMessage);
            var activityPresenter = _factory.Create(localSmsMessage).Wait();

            activityPresenter.Device.Should().Be(Resources.FromLocal);
        }

        [Test]
        public void Create_WithRemoteSmsMessage_SetsDeviceToRemote()
        {
            var remoteSmsMessage = new RemoteSmsMessage();
            SetupSmsMessagePresenterFactory<RemoteSmsMessage, RemoteSmsMessagePresenter>(remoteSmsMessage);
            var activityPresenter = _factory.Create(remoteSmsMessage).Wait();

            activityPresenter.Device.Should().Be(Resources.FromCloud);
        }

        [Test]
        public void Create_WithSmsMessage_SetsContactInfo()
        {
            var remoteSmsMessage = new RemoteSmsMessage();
            var contactInfoPresenter = new ContactInfoPresenter(new ContactInfo());
            SetupSmsMessagePresenterFactory<RemoteSmsMessage, RemoteSmsMessagePresenter>(remoteSmsMessage, contactInfoPresenter);
            var activityPresenter = _factory.Create(remoteSmsMessage).Wait();

            ContactInfoPresenter contactInfo = activityPresenter.ExtraData.ContactInfo;
            contactInfo.Should().Be(contactInfoPresenter);
        }

        [Test]
        public void Create_WithSmsMessage_SetsContent()
        {
            var remoteSmsMessage = new RemoteSmsMessage { Content = "something" };
            SetupSmsMessagePresenterFactory<RemoteSmsMessage, RemoteSmsMessagePresenter>(remoteSmsMessage);
            var activityPresenter = _factory.Create(remoteSmsMessage).Wait();

            activityPresenter.Content.Should().Be("something");
        }

        [Test]
        public void Create_WithSmsMessageAndNullContent_SetsContentToStringEmpty()
        {
            var remoteSmsMessage = new RemoteSmsMessage { Content = null };
            SetupSmsMessagePresenterFactory<RemoteSmsMessage, RemoteSmsMessagePresenter>(remoteSmsMessage);
            var activityPresenter = _factory.Create(remoteSmsMessage).Wait();

            activityPresenter.Content.Should().Be(String.Empty);
        }

        [Test]
        public void Create_WithUpdateInfo_SetsTypeToVersion()
        {
            var activityPresenter = _factory.Create(new UpdateInfo()).Wait();
            activityPresenter.Type.Should().Be(ActivityTypeEnum.Version);
        }

        [Test]
        public void Create_WithUpdateInfoWasInstalled_SetsContentToNewVersionInstalled()
        {
            var activityPresenter = _factory.Create(new UpdateInfo { WasInstalled = true }).Wait();
            activityPresenter.Content.Should().Be(Resources.NewVersionInstalled);
        }

        [Test]
        public void Create_WithUpdateInfoNotInstalled_SetsContentToNewVersionAvailable()
        {
            var activityPresenter = _factory.Create(new UpdateInfo { WasInstalled = false }).Wait();
            activityPresenter.Content.Should().Be(Resources.NewVersionAvailable);
        }

        [Test]
        public void Create_WithUpdateInfo_SetsBackingModelToUpdateInfoPresenter()
        {
            var updateInfo = new UpdateInfo();
            var activityPresenter = _factory.Create(updateInfo).Wait();

            activityPresenter.SourceId.Should().Be(updateInfo.UniqueId);            
        }

        private void SetupPhoneCallPresenterFactory<T, TPresenter>(T phoneCall)
            where T : PhoneCall
            where TPresenter : PhoneCallPresenter
        {
            SetupPhoneCallPresenterFactory<T, TPresenter>(phoneCall, new ContactInfoPresenter(new ContactInfo()));
        }

        private void SetupPhoneCallPresenterFactory<T, TPresenter>(T phoneCall, ContactInfoPresenter contactInfoPresenter)
            where T : PhoneCall
            where TPresenter : PhoneCallPresenter
        {
            var presenter = (TPresenter)Activator.CreateInstance(typeof(TPresenter), phoneCall);
            _mockPhoneCallPresenterFactory.Setup(m => m.Create(phoneCall))
                .Returns(Observable.Return(presenter.SetContactInfoPresenter(contactInfoPresenter)).Cast<TPresenter>());
        }

        private void SetupSmsMessagePresenterFactory<T, TPresenter>(T smsMessage)
            where T : SmsMessage
            where TPresenter : SmsMessagePresenter
        {
            SetupSmsMessagePresenterFactory<T, TPresenter>(smsMessage, new ContactInfoPresenter(new ContactInfo()));
        }

        private void SetupSmsMessagePresenterFactory<T, TPresenter>(T smsMessage, ContactInfoPresenter contactInfoPresenter)
            where T : SmsMessage
            where TPresenter : SmsMessagePresenter
        {
            var presenter = (TPresenter)Activator.CreateInstance(typeof(TPresenter), smsMessage);
            _mockSmsMessagePresenterFactory.Setup(m => m.Create(smsMessage))
                .Returns(Observable.Return(presenter.SetContactInfoPresenter(contactInfoPresenter)).Cast<TPresenter>());
        }

    }
}