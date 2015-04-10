namespace OmnipasteTests.Framework.Models.Factories
{
    using System.Reactive.Linq;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Models.Factories;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ConversationModelFactoryTests
    {
        private Mock<IContactRepository> _mockContactRepository;

        private ConversationModelFactory _subject;

        [SetUp]
        public void SetUp()
        {
            _mockContactRepository = new Mock<IContactRepository>();
            _subject = new ConversationModelFactory(_mockContactRepository.Object);
        }

        [Test]
        public void CreateLocalPhoneCallModel()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactEntity { UniqueId = "42" }));

            var localPhoneCallModel = _subject.Create<LocalPhoneCallModel, LocalPhoneCallEntity>(new LocalPhoneCallEntity { ContactUniqueId = "42" }).Wait();

            localPhoneCallModel.ContactModel.UniqueId.Should().Be("42");
        }

        [Test]
        public void CreateRemotePhoneCallModel()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactEntity { UniqueId = "42" }));

            var remotePhoneCallModel = _subject.Create<RemotePhoneCallModel, RemotePhoneCallEntity>(new RemotePhoneCallEntity { ContactUniqueId = "42" }).Wait();

            remotePhoneCallModel.ContactModel.UniqueId.Should().Be("42");
        }

        [Test]
        public void CreateLocalSmsMessageModel()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactEntity { UniqueId = "42" }));

            var localSmsMessageModel = _subject.Create<LocalSmsMessageModel, LocalSmsMessageEntity>(new LocalSmsMessageEntity { ContactUniqueId = "42" }).Wait();

            localSmsMessageModel.ContactModel.UniqueId.Should().Be("42");
        }

        [Test]
        public void CreateRemoteSmsMessageModel()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactEntity { UniqueId = "42" }));

            var remoteSmsMessageModel = _subject.Create<RemoteSmsMessageModel, RemoteSmsMessageEntity>(new RemoteSmsMessageEntity { ContactUniqueId = "42" }).Wait();

            remoteSmsMessageModel.ContactModel.UniqueId.Should().Be("42");
        }
    }
}