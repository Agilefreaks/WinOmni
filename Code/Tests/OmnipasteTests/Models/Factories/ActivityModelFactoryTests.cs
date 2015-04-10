namespace OmnipasteTests.Models.Factories
{
    using System;
    using System.Reactive.Linq;
    using Clipboard.Dto;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.ActivityList.Activity;
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
            var activityModel = _factory.Create(new ClippingEntity()).Wait();

            activityModel.Type.Should().Be(ActivityTypeEnum.Clipping);
        }

        [Test]
        public void Create_WithClipping_SetsContent()
        {
            var activityModel = _factory.Create(new ClippingEntity { Content = "some" }).Wait();

            activityModel.Content.Should().Be("some");
        }

        [Test]
        public void Create_WithClippingAndSourceIsCloud_SetsDeviceToCloud()
        {
            var activityModel =
                _factory.Create(new ClippingEntity { Source = ClippingDto.ClippingSourceEnum.Cloud }).Wait();

            activityModel.Device.Should().Be(Resources.FromCloud);
        }

        [Test]
        public void Create_WithClipping_SetsSourceId()
        {
            var activityModel = _factory.Create(new ClippingEntity { UniqueId = "42" }).Wait();

            activityModel.SourceId.Should().Be("42");
        }

        [Test]
        public void Create_WithLocalPhoneCall_SetsTypeToCall()
        {
            var localPhoneCall = new LocalPhoneCallEntity();
            SetupContactRepository(localPhoneCall);
            var activityModel = _factory.Create(localPhoneCall).Wait();

            activityModel.Type.Should().Be(ActivityTypeEnum.Call);
        }

        [Test]
        public void Create_WithLocalPhoneCall_SetsDeviceToLocal()
        {
            var localPhoneCall = new LocalPhoneCallEntity();
            SetupContactRepository(localPhoneCall);
            var activityModel = _factory.Create(localPhoneCall).Wait();

            activityModel.Device.Should().Be(Resources.FromLocal);
        }

        [Test]
        public void Create_WithRemotePhoneCall_SetsDeviceToRemote()
        {
            var remotePhoneCall = new RemotePhoneCallEntity();
            SetupContactRepository(remotePhoneCall);
            var activityModel = _factory.Create(remotePhoneCall).Wait();

            activityModel.Device.Should().Be(Resources.FromCloud);
        }

        [Test]
        public void Create_WithPhoneCall_SetsContact()
        {
            var remotePhoneCall = new RemotePhoneCallEntity();
            var contactEntity = new ContactEntity();
            SetupContactRepository(remotePhoneCall, contactEntity);
            var activityModel = _factory.Create(remotePhoneCall).Wait();

            activityModel.ContactEntity.Should().Be(contactEntity);
        }

        [Test]
        public void Create_WithLocalSmsMessage_SetsTypeToMessage()
        {
            var localSmsMessage = new LocalSmsMessageEntity();
            SetupContactRepository(localSmsMessage);
            var activityModel = _factory.Create(localSmsMessage).Wait();

            activityModel.Type.Should().Be(ActivityTypeEnum.Message);
        }

        [Test]
        public void Create_WithLocalSmsMessage_SetsDeviceToLocal()
        {
            var localSmsMessage = new LocalSmsMessageEntity();
            SetupContactRepository(localSmsMessage);
            var activityModel = _factory.Create(localSmsMessage).Wait();

            activityModel.Device.Should().Be(Resources.FromLocal);
        }

        [Test]
        public void Create_WithRemoteSmsMessage_SetsDeviceToRemote()
        {
            var remoteSmsMessage = new RemoteSmsMessageEntity();
            SetupContactRepository(remoteSmsMessage);
            var activityModel = _factory.Create(remoteSmsMessage).Wait();

            activityModel.Device.Should().Be(Resources.FromCloud);
        }

        [Test]
        public void Create_WithSmsMessage_SetsContact()
        {
            var remoteSmsMessage = new RemoteSmsMessageEntity();
            var contactEntity = new ContactEntity();
            SetupContactRepository(remoteSmsMessage, contactEntity);
            var activityModel = _factory.Create(remoteSmsMessage).Wait();

            activityModel.ContactEntity.Should().Be(contactEntity);
        }

        [Test]
        public void Create_WithSmsMessage_SetsContent()
        {
            var remoteSmsMessage = new RemoteSmsMessageEntity { Content = "something" };
            SetupContactRepository(remoteSmsMessage);
            var activityModel = _factory.Create(remoteSmsMessage).Wait();

            activityModel.Content.Should().Be("something");
        }

        [Test]
        public void Create_WithSmsMessageAndNullContent_SetsContentToStringEmpty()
        {
            var remoteSmsMessage = new RemoteSmsMessageEntity { Content = null };
            SetupContactRepository(remoteSmsMessage);
            var activityModel = _factory.Create(remoteSmsMessage).Wait();

            activityModel.Content.Should().Be(String.Empty);
        }

        [Test]
        public void Create_WithUpdateEntity_SetsTypeToVersion()
        {
            var activityModel = _factory.Create(new UpdateEntity()).Wait();
            activityModel.Type.Should().Be(ActivityTypeEnum.Version);
        }

        [Test]
        public void Create_WithUpdateEntityWasInstalled_SetsContentToNewVersionInstalled()
        {
            var activityModel = _factory.Create(new UpdateEntity { WasInstalled = true }).Wait();
            activityModel.Content.Should().Be(Resources.NewVersionInstalled);
        }

        [Test]
        public void Create_WithUpdateEntityNotInstalled_SetsContentToNewVersionAvailable()
        {
            var activityModel = _factory.Create(new UpdateEntity { WasInstalled = false }).Wait();
            activityModel.Content.Should().Be(Resources.NewVersionAvailable);
        }

        [Test]
        public void Create_WithUpdateEntity_SetsBackingModelToUpdateModel()
        {
            var updateEntity = new UpdateEntity();
            var activityModel = _factory.Create(updateEntity).Wait();

            activityModel.SourceId.Should().Be(updateEntity.UniqueId);            
        }

        private void SetupContactRepository<T>(T conversationModel, ContactEntity contactEntity = null)
            where T : ConversationEntity
        {
            contactEntity = contactEntity ?? new ContactEntity();
            _mockContactRepository.Setup(m => m.Get(conversationModel.ContactUniqueId)).Returns(Observable.Return(contactEntity));
        }
    }
}