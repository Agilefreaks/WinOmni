namespace OmnipasteTests.ContactList
{
    using FluentAssertions;
    using Microsoft.Practices.ServiceLocation;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.ContactList;
    using Omnipaste.ContactList.Contact;
    using Omnipaste.Entities;
    using Omnipaste.Models;

    [TestFixture]
    public class ContactInfoViewModelFactoryTests
    {
        private IContactViewModelFactory _subject;

        private Mock<IServiceLocator> _mockServiceLocator;

        [SetUp]
        public void SetUp()
        {
            _mockServiceLocator = new Mock<IServiceLocator>();

            _subject = new ContactViewModelFactory(_mockServiceLocator.Object);
        }

        [Test]
        public void Create_Always_AssignsModelOnResult()
        {
            var contactInfoPresenter = new ContactModel(new ContactEntity());
            var mockContactInfoViewModel = new Mock<IContactViewModel>();
            _mockServiceLocator.Setup(m => m.GetInstance<IContactViewModel>())
                .Returns(mockContactInfoViewModel.Object);
            
            var result = _subject.Create<IContactViewModel>(contactInfoPresenter);

            result.Should().Be(mockContactInfoViewModel.Object);
            mockContactInfoViewModel.VerifySet(m => m.Model = contactInfoPresenter);
        }
    }
}
