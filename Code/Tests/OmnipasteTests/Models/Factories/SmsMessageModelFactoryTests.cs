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
    public class SmsMessageModelFactoryTests
    {
        private SmsMessageModelFactory _subject;

        private Mock<IContactRepository> _mockContactRepository;

        private ContactEntity _contactEntity;

        [SetUp]
        public void SetUp()
        {
            _mockContactRepository = new Mock<IContactRepository>();
            _contactEntity = new ContactEntity();
            _mockContactRepository.Setup(m => m.Get(It.IsAny<string>())).Returns(Observable.Return(_contactEntity));

            _subject = new SmsMessageModelFactory(_mockContactRepository.Object);
        }

        [Test]
        public void Create_WithALocalSmsMessage_ReturnsALocalSmsMessagePresenter()
        {
            _subject.Create(new LocalSmsMessageEntity()).Wait().Should().BeOfType<LocalSmsMessageModel>();
        }

        [Test]
        public void Create_WithARemoteSmsMessage_ReturnsARemoteSmsMessagePresenter()
        {
            _subject.Create(new RemoteSmsMessageEntity()).Wait().Should().BeOfType<RemoteSmsMessageModel>();
        }

        [Test]
        public void Create_WithASmsMessage_WillSetTheContactInfoPresenter()
        {
            var phoneCallPresenter = _subject.Create(new LocalSmsMessageEntity()).Wait();

            phoneCallPresenter.ContactModel.BackingEntity.Should().Be(_contactEntity);
        }

    }
}