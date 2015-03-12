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
    public class SmsMessagePresenterFactoryTests
    {
        private Mock<IContactRepository> _mockContactRepository;

        private SmsMessagePresenterFactory _factory;

        private ContactInfo _contactInfo;

        [SetUp]
        public void SetUp()
        {
            _mockContactRepository = new Mock<IContactRepository>();
            _contactInfo = new ContactInfo();
            _mockContactRepository.Setup(m => m.Get(It.IsAny<string>())).Returns(Observable.Return(_contactInfo));

            _factory = new SmsMessagePresenterFactory(_mockContactRepository.Object);
        }

        [Test]
        public void Create_WithALocalSmsMessage_ReturnsALocalSmsMessagePresenter()
        {
            _factory.Create(new LocalSmsMessage()).Wait().Should().BeOfType<LocalSmsMessagePresenter>();
        }

        [Test]
        public void Create_WithARemoteSmsMessage_ReturnsARemoteSmsMessagePresenter()
        {
            _factory.Create(new RemoteSmsMessage()).Wait().Should().BeOfType<RemoteSmsMessagePresenter>();
        }

        [Test]
        public void Create_WithASmsMessage_WillSetTheContactInfoPresenter()
        {
            var phoneCallPresenter = _factory.Create(new LocalSmsMessage()).Wait();

            phoneCallPresenter.ContactInfoPresenter.BackingModel.Should().Be(_contactInfo);
        }

    }
}