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
    public class PhoneCallPresenterFactoryTests
    {
        private Mock<IContactRepository> _mockContactRepository;

        private PhoneCallPresenterFactory _factory;

        private ContactInfo _contactInfo;

        [SetUp]
        public void SetUp()
        {
            _mockContactRepository = new Mock<IContactRepository>();
            _contactInfo = new ContactInfo();
            _mockContactRepository.Setup(m => m.Get(It.IsAny<string>())).Returns(Observable.Return(_contactInfo));

            _factory = new PhoneCallPresenterFactory(_mockContactRepository.Object);
        }

        [Test]
        public void Create_WithALocalPhoneCall_ReturnsALocalPhoneCallPresenter()
        {
            _factory.Create(new LocalPhoneCall()).Wait().Should().BeOfType<LocalPhoneCallPresenter>();
        }

        [Test]
        public void Create_WithARemotePhoneCall_ReturnsARemotePhoneCallPresenter()
        {
            _factory.Create(new RemotePhoneCall()).Wait().Should().BeOfType<RemotePhoneCallPresenter>();
        }

        [Test]
        public void Create_WithAPhoneCall_WillSetTheContactInfoPresenter()
        {
            var phoneCallPresenter = _factory.Create(new RemotePhoneCall()).Wait();

            phoneCallPresenter.ContactInfoPresenter.BackingModel.Should().Be(_contactInfo);
        }
    }
}
