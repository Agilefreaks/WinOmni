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
    public class SmsMessagePresenterFactoryTests
    {
        private Mock<IContactRepository> _mockContactRepository;

        private SmsMessageModelFactory _factory;

        private ContactEntity _contactEntity;

        [SetUp]
        public void SetUp()
        {
            _mockContactRepository = new Mock<IContactRepository>();
            _contactEntity = new ContactEntity();
            _mockContactRepository.Setup(m => m.Get(It.IsAny<string>())).Returns(Observable.Return(_contactEntity));

            _factory = new SmsMessageModelFactory(_mockContactRepository.Object);
        }

        [Test]
        public void Create_WithALocalSmsMessage_ReturnsALocalSmsMessagePresenter()
        {
            _factory.Create(new LocalSmsMessageEntity()).Wait().Should().BeOfType<LocalSmsMessageModel>();
        }

        [Test]
        public void Create_WithARemoteSmsMessage_ReturnsARemoteSmsMessagePresenter()
        {
            _factory.Create(new RemoteSmsMessageEntity()).Wait().Should().BeOfType<RemoteSmsMessageModel>();
        }

        [Test]
        public void Create_WithASmsMessage_WillSetTheContactInfoPresenter()
        {
            var phoneCallPresenter = _factory.Create(new LocalSmsMessageEntity()).Wait();

            phoneCallPresenter.ContactModel.BackingEntity.Should().Be(_contactEntity);
        }

    }
}