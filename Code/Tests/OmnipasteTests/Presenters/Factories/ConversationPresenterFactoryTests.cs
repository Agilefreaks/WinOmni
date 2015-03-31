namespace OmnipasteTests.Presenters.Factories
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
    public class ConversationPresenterFactoryTests
    {
        private Mock<IContactRepository> _mockContactRepository;

        private ConversationModelFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _mockContactRepository = new Mock<IContactRepository>();
            _factory = new ConversationModelFactory(_mockContactRepository.Object);
        }

        [Test]
        public void CreateLocalPhoneCallPresenter()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactEntity { UniqueId = "42" }));

            var localPhoneCallPresenter = _factory.Create<LocalPhoneCallModel, LocalPhoneCallEntity>(new LocalPhoneCallEntity { ContactInfoUniqueId = "42" }).Wait();

            localPhoneCallPresenter.ContactModel.UniqueId.Should().Be("42");
        }

        [Test]
        public void CreateRemotePhoneCallPresenter()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactEntity { UniqueId = "42" }));

            var localPhoneCallPresenter = _factory.Create<RemotePhoneCallModel, RemotePhoneCallEntity>(new RemotePhoneCallEntity { ContactInfoUniqueId = "42" }).Wait();

            localPhoneCallPresenter.ContactModel.UniqueId.Should().Be("42");
        }

        [Test]
        public void CreateLocalSmsMessagePresenter()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactEntity { UniqueId = "42" }));

            var localPhoneCallPresenter = _factory.Create<LocalSmsMessageModel, LocalSmsMessageEntity>(new LocalSmsMessageEntity { ContactInfoUniqueId = "42" }).Wait();

            localPhoneCallPresenter.ContactModel.UniqueId.Should().Be("42");
        }

        [Test]
        public void CreateRemoteSmsMessagePresenter()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactEntity { UniqueId = "42" }));

            var localPhoneCallPresenter = _factory.Create<RemoteSmsMessageModel, RemoteSmsMessageEntity>(new RemoteSmsMessageEntity { ContactInfoUniqueId = "42" }).Wait();

            localPhoneCallPresenter.ContactModel.UniqueId.Should().Be("42");
        }
    }
}