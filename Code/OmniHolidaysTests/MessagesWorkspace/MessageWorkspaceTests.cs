namespace OmniHolidaysTests.MessagesWorkspace
{
    using Moq;
    using NUnit.Framework;
    using OmniHolidays.MessagesWorkspace;
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniHolidays.MessagesWorkspace.MessageDetails;
    using OmniUI.Workspace;

    [TestFixture]
    public class MessageWorkspaceTests
    {
        private IMessagesWorkspace _subject;

        private Mock<IDetailsConductorViewModel> _mockDetailsConductor;

        private Mock<IMessageDetailsViewModel> _mockMessageDetailsViewModel;

        [SetUp]
        public void Setup()
        {
            _mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            _mockMessageDetailsViewModel = new Mock<IMessageDetailsViewModel>();
            _subject = new MessagesWorkspace(
                new Mock<IContactListViewModel>().Object,
                _mockDetailsConductor.Object)
                           {
                               MessageDetailsViewModel = _mockMessageDetailsViewModel.Object
                           };
        }

        [Test]
        public void Activate_Alwas_ActivatesTheMessageDetailsViewModelInTheDetailsConductor()
        {
            _subject.Activate();

            _mockDetailsConductor.Verify(x => x.ActivateItem(_mockMessageDetailsViewModel.Object), Times.Once());
        }
    }
}