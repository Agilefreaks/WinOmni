namespace OmnipasteTests.Models.Factories
{
    using System.Reactive.Linq;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Models.Factories;
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
        public void CreateLocalPhoneCallPresenter()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactEntity { UniqueId = "42" }));

            var localPhoneCallPresenter = _subject.Create<LocalPhoneCallModel, LocalPhoneCallEntity>(new LocalPhoneCallEntity { ContactInfoUniqueId = "42" }).Wait();

            localPhoneCallPresenter.ContactModel.UniqueId.Should().Be("42");
        }

        [Test]
        public void CreateRemotePhoneCallPresenter()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactEntity { UniqueId = "42" }));

            var localPhoneCallPresenter = _subject.Create<RemotePhoneCallModel, RemotePhoneCallEntity>(new RemotePhoneCallEntity { ContactInfoUniqueId = "42" }).Wait();

            localPhoneCallPresenter.ContactModel.UniqueId.Should().Be("42");
        }

        [Test]
        public void CreateLocalSmsMessagePresenter()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactEntity { UniqueId = "42" }));

            var localPhoneCallPresenter = _subject.Create<LocalSmsMessageModel, LocalSmsMessageEntity>(new LocalSmsMessageEntity { ContactInfoUniqueId = "42" }).Wait();

            localPhoneCallPresenter.ContactModel.UniqueId.Should().Be("42");
        }

        [Test]
        public void CreateRemoteSmsMessagePresenter()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactEntity { UniqueId = "42" }));

            var localPhoneCallPresenter = _subject.Create<RemoteSmsMessageModel, RemoteSmsMessageEntity>(new RemoteSmsMessageEntity { ContactInfoUniqueId = "42" }).Wait();

            localPhoneCallPresenter.ContactModel.UniqueId.Should().Be("42");
        }
    }
}