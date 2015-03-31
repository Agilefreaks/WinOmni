namespace OmnipasteTests.Models.Factories
{
    using System;
    using System.Reactive.Linq;
    using Clipboard.Dto;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Models.Factories;
    using Omnipaste.Properties;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ActivityModelFactoryTests
    {
        private Mock<IContactRepository> _mockContactRepository;

        private ActivityModelFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _mockContactRepository = new Mock<IContactRepository>();
            _factory = new ActivityModelFactory(_mockContactRepository.Object);
        }

        [Test]
        public void Create_WithClipping_SetsTypeToClipping()
        {
            var activityPresenter = _factory.Create(new ClippingEntity()).Wait();

            activityPresenter.Type.Should().Be(ActivityTypeEnum.Clipping);
        }

        [Test]
        public void Create_WithClipping_SetsContent()
        {
            var activityPresenter = _factory.Create(new ClippingEntity { Content = "some" }).Wait();

            activityPresenter.Content.Should().Be("some");
        }

        [Test]
        public void Create_WithClippingAndSourceIsCloud_SetsDeviceToCloud()
        {
            var activityPresenter =
                _factory.Create(new ClippingEntity { Source = ClippingDto.ClippingSourceEnum.Cloud }).Wait();

            activityPresenter.Device.Should().Be(Resources.FromCloud);
        }

        [Test]
        public void Create_WithClipping_SetsSourceId()
        {
            var activityPresenter = _factory.Create(new ClippingEntity { UniqueId = "42" }).Wait();

            activityPresenter.SourceId.Should().Be("42");
        }

        [Test]
        public void Create_WithLocalPhoneCall_SetsTypeToCall()
        {
            var localPhoneCall = new LocalPhoneCallEntity();
            SetupContactRepository(localPhoneCall);
            var activityPresenter = _factory.Create(localPhoneCall).Wait();

            activityPresenter.Type.Should().Be(ActivityTypeEnum.Call);
        }

        [Test]
        public void Create_WithLocalPhoneCall_SetsDeviceToLocal()
        {
            var localPhoneCall = new LocalPhoneCallEntity();
            SetupContactRepository(localPhoneCall);
            var activityPresenter = _factory.Create(localPhoneCall).Wait();

            activityPresenter.Device.Should().Be(Resources.FromLocal);
        }

        [Test]
        public void Create_WithRemotePhoneCall_SetsDeviceToRemote()
        {
            var remotePhoneCall = new RemotePhoneCallEntity();
            SetupContactRepository(remotePhoneCall);
            var activityPresenter = _factory.Create(remotePhoneCall).Wait();

            activityPresenter.Device.Should().Be(Resources.FromCloud);
        }

        [Test]
        public void Create_WithPhoneCall_SetsContactInfo()
        {
            var remotePhoneCall = new RemotePhoneCallEntity();
            var contactInfo = new ContactEntity();
            SetupContactRepository(remotePhoneCall, contactInfo);
            var activityPresenter = _factory.Create(remotePhoneCall).Wait();

            activityPresenter.ContactEntity.Should().Be(contactInfo);
        }

        [Test]
        public void Create_WithLocalSmsMessage_SetsTypeToMessage()
        {
            var localSmsMessage = new LocalSmsMessageEntity();
            SetupContactRepository(localSmsMessage);
            var activityPresenter = _factory.Create(localSmsMessage).Wait();

            activityPresenter.Type.Should().Be(ActivityTypeEnum.Message);
        }

        [Test]
        public void Create_WithLocalSmsMessage_SetsDeviceToLocal()
        {
            var localSmsMessage = new LocalSmsMessageEntity();
            SetupContactRepository(localSmsMessage);
            var activityPresenter = _factory.Create(localSmsMessage).Wait();

            activityPresenter.Device.Should().Be(Resources.FromLocal);
        }

        [Test]
        public void Create_WithRemoteSmsMessage_SetsDeviceToRemote()
        {
            var remoteSmsMessage = new RemoteSmsMessageEntity();
            SetupContactRepository(remoteSmsMessage);
            var activityPresenter = _factory.Create(remoteSmsMessage).Wait();

            activityPresenter.Device.Should().Be(Resources.FromCloud);
        }

        [Test]
        public void Create_WithSmsMessage_SetsContactInfo()
        {
            var remoteSmsMessage = new RemoteSmsMessageEntity();
            var contactInfo = new ContactEntity();
            SetupContactRepository(remoteSmsMessage, contactInfo);
            var activityPresenter = _factory.Create(remoteSmsMessage).Wait();

            activityPresenter.ContactEntity.Should().Be(contactInfo);
        }

        [Test]
        public void Create_WithSmsMessage_SetsContent()
        {
            var remoteSmsMessage = new RemoteSmsMessageEntity { Content = "something" };
            SetupContactRepository(remoteSmsMessage);
            var activityPresenter = _factory.Create(remoteSmsMessage).Wait();

            activityPresenter.Content.Should().Be("something");
        }

        [Test]
        public void Create_WithSmsMessageAndNullContent_SetsContentToStringEmpty()
        {
            var remoteSmsMessage = new RemoteSmsMessageEntity { Content = null };
            SetupContactRepository(remoteSmsMessage);
            var activityPresenter = _factory.Create(remoteSmsMessage).Wait();

            activityPresenter.Content.Should().Be(String.Empty);
        }

        [Test]
        public void Create_WithUpdateInfo_SetsTypeToVersion()
        {
            var activityPresenter = _factory.Create(new UpdateEntity()).Wait();
            activityPresenter.Type.Should().Be(ActivityTypeEnum.Version);
        }

        [Test]
        public void Create_WithUpdateInfoWasInstalled_SetsContentToNewVersionInstalled()
        {
            var activityPresenter = _factory.Create(new UpdateEntity { WasInstalled = true }).Wait();
            activityPresenter.Content.Should().Be(Resources.NewVersionInstalled);
        }

        [Test]
        public void Create_WithUpdateInfoNotInstalled_SetsContentToNewVersionAvailable()
        {
            var activityPresenter = _factory.Create(new UpdateEntity { WasInstalled = false }).Wait();
            activityPresenter.Content.Should().Be(Resources.NewVersionAvailable);
        }

        [Test]
        public void Create_WithUpdateInfo_SetsBackingModelToUpdateInfoPresenter()
        {
            var updateInfo = new UpdateEntity();
            var activityPresenter = _factory.Create(updateInfo).Wait();

            activityPresenter.SourceId.Should().Be(updateInfo.UniqueId);            
        }

        private void SetupContactRepository<T>(T conversationModel, ContactEntity contactEntity = null)
            where T : ConversationEntity
        {
            contactEntity = contactEntity ?? new ContactEntity();
            _mockContactRepository.Setup(m => m.Get(conversationModel.ContactInfoUniqueId)).Returns(Observable.Return(contactEntity));
        }
    }
}