namespace OmnipasteTests.Presenters.Factories
{
    using System.Reactive.Linq;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Presenters.Factories;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ConversationPresenterFactoryTests
    {
        private Mock<IContactRepository> _mockContactRepository;

        private ConversationPresenterFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _mockContactRepository = new Mock<IContactRepository>();
            _factory = new ConversationPresenterFactory(_mockContactRepository.Object);
        }

        [Test]
        public void CreateLocalPhoneCallPresenter()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactInfo { UniqueId = "42" }));

            var localPhoneCallPresenter = _factory.Create<LocalPhoneCallPresenter, LocalPhoneCall>(new LocalPhoneCall { ContactInfoUniqueId = "42" }).Wait();

            localPhoneCallPresenter.ContactInfoPresenter.UniqueId.Should().Be("42");
        }

        [Test]
        public void CreateRemotePhoneCallPresenter()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactInfo { UniqueId = "42" }));

            var localPhoneCallPresenter = _factory.Create<RemotePhoneCallPresenter, RemotePhoneCall>(new RemotePhoneCall { ContactInfoUniqueId = "42" }).Wait();

            localPhoneCallPresenter.ContactInfoPresenter.UniqueId.Should().Be("42");
        }

        [Test]
        public void CreateLocalSmsMessagePresenter()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactInfo { UniqueId = "42" }));

            var localPhoneCallPresenter = _factory.Create<LocalSmsMessagePresenter, LocalSmsMessage>(new LocalSmsMessage { ContactInfoUniqueId = "42" }).Wait();

            localPhoneCallPresenter.ContactInfoPresenter.UniqueId.Should().Be("42");
        }

        [Test]
        public void CreateRemoteSmsMessagePresenter()
        {
            _mockContactRepository.Setup(m => m.Get("42"))
                .Returns(Observable.Return(new ContactInfo { UniqueId = "42" }));

            var localPhoneCallPresenter = _factory.Create<RemoteSmsMessagePresenter, RemoteSmsMessage>(new RemoteSmsMessage { ContactInfoUniqueId = "42" }).Wait();

            localPhoneCallPresenter.ContactInfoPresenter.UniqueId.Should().Be("42");
        }
    }
}