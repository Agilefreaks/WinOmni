namespace OmnipasteTests.Presenters.Factories
{
    using System.Reactive.Linq;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Presenters.Factories;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class SmsMessagePresenterFactoryTests
    {
        private Mock<IContactRepository> _mockContactRepository;

        private SmsMessagePresenterFactory _factory;

        private ContactEntity _contactEntity;

        [SetUp]
        public void SetUp()
        {
            _mockContactRepository = new Mock<IContactRepository>();
            _contactEntity = new ContactEntity();
            _mockContactRepository.Setup(m => m.Get(It.IsAny<string>())).Returns(Observable.Return(_contactEntity));

            _factory = new SmsMessagePresenterFactory(_mockContactRepository.Object);
        }

        [Test]
        public void Create_WithALocalSmsMessage_ReturnsALocalSmsMessagePresenter()
        {
            _factory.Create(new LocalSmsMessageEntity()).Wait().Should().BeOfType<LocalSmsMessagePresenter>();
        }

        [Test]
        public void Create_WithARemoteSmsMessage_ReturnsARemoteSmsMessagePresenter()
        {
            _factory.Create(new RemoteSmsMessageEntity()).Wait().Should().BeOfType<RemoteSmsMessagePresenter>();
        }

        [Test]
        public void Create_WithASmsMessage_WillSetTheContactInfoPresenter()
        {
            var phoneCallPresenter = _factory.Create(new LocalSmsMessageEntity()).Wait();

            phoneCallPresenter.ContactInfoPresenter.BackingModel.Should().Be(_contactEntity);
        }

    }
}