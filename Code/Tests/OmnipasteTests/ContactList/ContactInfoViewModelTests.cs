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
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.Workspaces;
    using OmniUI.Workspace;

    [TestFixture]
    public class ContactInfoViewModelTests
    {
        private ContactInfoViewModel _subject;

        private ContactInfoPresenter _contactInfoPresenter;

        private ContactInfo _contactInfo;

        private Mock<IContactRepository> _mockContactRepository;

        private Mock<IWorkspaceDetailsViewModelFactory> _mockDetailsViewModelFactory;

        [SetUp]
        public void SetUp()
        {
            _contactInfo = new ContactInfo { FirstName = "test", LastName = "test", IsStarred = false };
            _contactInfoPresenter = new ContactInfoPresenter(_contactInfo);
            _mockContactRepository = new Mock<IContactRepository> { DefaultValue = DefaultValue.Mock };
            _mockDetailsViewModelFactory = new Mock<IWorkspaceDetailsViewModelFactory> { DefaultValue = DefaultValue.Mock };

            _subject = new ContactInfoViewModel
                           {
                               Model = _contactInfoPresenter,
                               ContactRepository = _mockContactRepository.Object,
                               DetailsViewModelFactory = _mockDetailsViewModelFactory.Object
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

        [Test]
        public void ShowDetails_Always_ActivatesAnActivityDetailsViewModelInItsParentActivityWorkspace()
        {
            var mockWorkspace = new Mock<IMessageWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            mockDetailsConductor.Verify(x => x.ActivateItem(It.IsAny<IWorkspaceDetailsViewModel>()), Times.Once());
        }
    }
}
