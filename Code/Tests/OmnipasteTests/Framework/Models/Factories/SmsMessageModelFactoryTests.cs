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
        public void Create_WithALocalSmsMessage_ReturnsALocalSmsMessageModel()
        {
            _subject.Create(new LocalSmsMessageEntity()).Wait().Should().BeOfType<LocalSmsMessageModel>();
        }

        [Test]
        public void Create_WithARemoteSmsMessage_ReturnsARemoteSmsMessageModel()
        {
            _subject.Create(new RemoteSmsMessageEntity()).Wait().Should().BeOfType<RemoteSmsMessageModel>();
        }

        [Test]
        public void Create_WithASmsMessage_WillSetTheContactModel()
        {
            var conversationModel = _subject.Create(new LocalSmsMessageEntity()).Wait();

            conversationModel.ContactModel.BackingEntity.Should().Be(_contactEntity);
        }

    }
}