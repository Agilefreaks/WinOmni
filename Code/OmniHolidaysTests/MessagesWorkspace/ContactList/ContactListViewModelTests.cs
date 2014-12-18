namespace OmniHolidaysTests.MessagesWorkspace.ContactList
{
    using Moq;
    using NUnit.Framework;
    using OmniHolidays.MessagesWorkspace;
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniHolidays.MessagesWorkspace.MessageDetails;
    using OmniUI.Workspace;

    [TestFixture]
    public class ContactListViewModelTests
    {
        private ContactListViewModel _subject;

        private Mock<IContactListHeaderViewModel> _mockContactListHeader;

        private Mock<IContactListContentViewModel> _mockContactListContent;

        private Mock<IMessageDetailsViewModel> _mockMessageDetails;

        [SetUp]
        public void Setup()
        {
            _mockContactListHeader = new Mock<IContactListHeaderViewModel>();
            _mockContactListContent = new Mock<IContactListContentViewModel>();
            _mockMessageDetails = new Mock<IMessageDetailsViewModel>();
            _subject = new ContactListViewModel(
                _mockContactListHeader.Object,
                _mockContactListContent.Object,
                _mockMessageDetails.Object);
        }

        [Test]
        public void Activate_Alwas_ActivatesTheMessageDetailsViewModelInTheWorkspaceDetailsConductor()
        {
            var mockWorkspace = new Mock<IMessagesWorkspace>();
            _subject.Parent = mockWorkspace.Object;
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            ((IContactListViewModel)_subject).Activate();

            mockDetailsConductor.Verify(x => x.ActivateItem(_mockMessageDetails.Object), Times.Once());
        }
    }
}