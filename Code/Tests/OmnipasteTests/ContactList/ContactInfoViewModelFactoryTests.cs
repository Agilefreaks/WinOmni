namespace OmnipasteTests.ContactList
{
    using FluentAssertions;
    using Microsoft.Practices.ServiceLocation;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.ContactList;
    using Omnipaste.Models;
    using Omnipaste.Presenters;

    [TestFixture]
    public class ContactInfoViewModelFactoryTests
    {
        private IContactInfoViewModelFactory _subject;

        private Mock<IServiceLocator> _mockServiceLocator;

        [SetUp]
        public void SetUp()
        {
            _mockServiceLocator = new Mock<IServiceLocator>();

            _subject = new ContactInfoViewModelFactory(_mockServiceLocator.Object);
        }

        [Test]
        public void Create_Always_AssignsModelOnResult()
        {
            var contactInfoPresenter = new ContactInfoPresenter(new ContactInfo());
            var mockContactInfoViewModel = new Mock<IContactInfoViewModel>();
            _mockServiceLocator.Setup(m => m.GetInstance<IContactInfoViewModel>())
                .Returns(mockContactInfoViewModel.Object);
            
            var result = _subject.Create<IContactInfoViewModel>(contactInfoPresenter);

            result.Should().Be(mockContactInfoViewModel.Object);
            mockContactInfoViewModel.VerifySet(m => m.Model = contactInfoPresenter);
        }
    }
}
