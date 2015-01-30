namespace OmnipasteTests.ContactList
{
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.ContactList;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ContactInfoViewModelTests
    {
        private ContactInfoViewModel _subject;

        private ContactInfoPresenter _contactInfoPresenter;

        private ContactInfo _contactInfo;

        private Mock<IContactRepository> _mockContactRepository;

        [SetUp]
        public void SetUp()
        {
            _contactInfo = new ContactInfo { FirstName = "test", LastName = "test", IsStarred = false };
            _contactInfoPresenter = new ContactInfoPresenter(_contactInfo);
            _mockContactRepository = new Mock<IContactRepository> { DefaultValue = DefaultValue.Mock };

            _subject = new ContactInfoViewModel
                           {
                               Model = _contactInfoPresenter,
                               ContactRepository = _mockContactRepository.Object
                           };
        }

        [Test]
        public void OnIsStarredChanged_AfterActivate_UpdatesModel()
        {
            ((IActivate)_subject).Activate();

            _contactInfoPresenter.IsStarred = true;

            _contactInfo.IsStarred.Should().BeTrue();
        }

        [Test]
        public void OnIsStarredChanged_AfterActivate_SavesModel()
        {
            ((IActivate)_subject).Activate();

            _contactInfoPresenter.IsStarred = true;

            _mockContactRepository.Verify(m => m.Save(_contactInfo));
        }

        [Test]
        public void OnIsStarredChanged_AfterDeactivate_DoesNotSaveModel()
        {
            ((IActivate)_subject).Activate();
            ((IDeactivate)_subject).Deactivate(false);

            _contactInfoPresenter.IsStarred = false;

            _mockContactRepository.Verify(m => m.Save(It.IsAny<ContactInfo>()), Times.Never());
        }
    }
}
