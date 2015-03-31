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
    public class PhoneCallModelFactoryTests
    {
        private PhoneCallModelFactory _subject;

        private Mock<IContactRepository> _mockContactRepository;

        private ContactEntity _contactEntity;

        [SetUp]
        public void SetUp()
        {
            _mockContactRepository = new Mock<IContactRepository>();
            _contactEntity = new ContactEntity();
            _mockContactRepository.Setup(m => m.Get(It.IsAny<string>())).Returns(Observable.Return(_contactEntity));

            _subject = new PhoneCallModelFactory(_mockContactRepository.Object);
        }

        [Test]
        public void Create_WithALocalPhoneCall_ReturnsALocalPhoneCallPresenter()
        {
            _subject.Create(new LocalPhoneCallEntity()).Wait().Should().BeOfType<LocalPhoneCallModel>();
        }

        [Test]
        public void Create_WithARemotePhoneCall_ReturnsARemotePhoneCallPresenter()
        {
            _subject.Create(new RemotePhoneCallEntity()).Wait().Should().BeOfType<RemotePhoneCallModel>();
        }

        [Test]
        public void Create_WithAPhoneCall_WillSetTheContactInfoPresenter()
        {
            var phoneCallPresenter = _subject.Create(new RemotePhoneCallEntity()).Wait();

            phoneCallPresenter.ContactModel.BackingEntity.Should().Be(_contactEntity);
        }
    }
}
