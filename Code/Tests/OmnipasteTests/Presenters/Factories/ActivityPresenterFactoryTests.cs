namespace OmnipasteTests.Presenters.Factories
{
    using System;
    using System.Reactive.Linq;
    using Clipboard.Models;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Models;
    using Omnipaste.Presenters.Factories;
    using Omnipaste.Properties;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ActivityPresenterFactoryTests
    {
        private Mock<IContactRepository> _mockContactRepository;

        private ActivityPresenterFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _mockContactRepository = new Mock<IContactRepository>();
            _factory = new ActivityPresenterFactory(_mockContactRepository.Object);
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
            SetupContactRepository(localPhoneCall);
            var activityPresenter = _factory.Create(localPhoneCall).Wait();

            activityPresenter.Type.Should().Be(ActivityTypeEnum.Call);
        }

        [Test]
        public void Create_WithLocalPhoneCall_SetsDeviceToLocal()
        {
            var localPhoneCall = new LocalPhoneCall();
            SetupContactRepository(localPhoneCall);
            var activityPresenter = _factory.Create(localPhoneCall).Wait();

            activityPresenter.Device.Should().Be(Resources.FromLocal);
        }

        [Test]
        public void Create_WithRemotePhoneCall_SetsDeviceToRemote()
        {
            var remotePhoneCall = new RemotePhoneCall();
            SetupContactRepository(remotePhoneCall);
            var activityPresenter = _factory.Create(remotePhoneCall).Wait();

            activityPresenter.Device.Should().Be(Resources.FromCloud);
        }

        [Test]
        public void Create_WithPhoneCall_SetsContactInfo()
        {
            var remotePhoneCall = new RemotePhoneCall();
            var contactInfo = new ContactInfo();
            SetupContactRepository(remotePhoneCall, contactInfo);
            var activityPresenter = _factory.Create(remotePhoneCall).Wait();

            activityPresenter.ContactInfo.Should().Be(contactInfo);
        }

        [Test]
        public void Create_WithLocalSmsMessage_SetsTypeToMessage()
        {
            var localSmsMessage = new LocalSmsMessage();
            SetupContactRepository(localSmsMessage);
            var activityPresenter = _factory.Create(localSmsMessage).Wait();

            activityPresenter.Type.Should().Be(ActivityTypeEnum.Message);
        }

        [Test]
        public void Create_WithLocalSmsMessage_SetsDeviceToLocal()
        {
            var localSmsMessage = new LocalSmsMessage();
            SetupContactRepository(localSmsMessage);
            var activityPresenter = _factory.Create(localSmsMessage).Wait();

            activityPresenter.Device.Should().Be(Resources.FromLocal);
        }

        [Test]
        public void Create_WithRemoteSmsMessage_SetsDeviceToRemote()
        {
            var remoteSmsMessage = new RemoteSmsMessage();
            SetupContactRepository(remoteSmsMessage);
            var activityPresenter = _factory.Create(remoteSmsMessage).Wait();

            activityPresenter.Device.Should().Be(Resources.FromCloud);
        }

        [Test]
        public void Create_WithSmsMessage_SetsContactInfo()
        {
            var remoteSmsMessage = new RemoteSmsMessage();
            var contactInfo = new ContactInfo();
            SetupContactRepository(remoteSmsMessage, contactInfo);
            var activityPresenter = _factory.Create(remoteSmsMessage).Wait();

            activityPresenter.ContactInfo.Should().Be(contactInfo);
        }

        [Test]
        public void Create_WithSmsMessage_SetsContent()
        {
            var remoteSmsMessage = new RemoteSmsMessage { Content = "something" };
            SetupContactRepository(remoteSmsMessage);
            var activityPresenter = _factory.Create(remoteSmsMessage).Wait();

            activityPresenter.Content.Should().Be("something");
        }

        [Test]
        public void Create_WithSmsMessageAndNullContent_SetsContentToStringEmpty()
        {
            var remoteSmsMessage = new RemoteSmsMessage { Content = null };
            SetupContactRepository(remoteSmsMessage);
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

        private void SetupContactRepository<T>(T conversationModel, ContactInfo contactInfo = null)
            where T : ConversationBaseModel
        {
            contactInfo = contactInfo ?? new ContactInfo();
            _mockContactRepository.Setup(m => m.Get(conversationModel.ContactInfoUniqueId)).Returns(Observable.Return(contactInfo));
        }
    }
}