namespace OmnipasteTests.ContactList
{
    using FluentAssertions;
    using Microsoft.Practices.ServiceLocation;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Conversations.ContactList;
    using Omnipaste.Conversations.ContactList.Contact;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;

    [TestFixture]
    public class ContactViewModelFactoryTests
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
            var contactModel = new ContactModel(new ContactEntity());
            var mockContactViewModel = new Mock<IContactViewModel>();
            _mockServiceLocator.Setup(m => m.GetInstance<IContactViewModel>())
                .Returns(mockContactViewModel.Object);
            
            var result = _subject.Create<IContactViewModel>(contactModel);

            result.Should().Be(mockContactViewModel.Object);
            mockContactViewModel.VerifySet(m => m.Model = contactModel);
        }
    }
}
