namespace OmnipasteTests.Conversations
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Conversations;
    using Omnipaste.Conversations.ContactList;
    using Omnipaste.Conversations.Conversation;
    using Omnipaste.Framework;
    using Omnipaste.Framework.Models;
    using OmniUI.Details;
    using OmniUI.Workspaces;

    [TestFixture]
    public class NewMessageWorkspaceTests
    {
        private NewMessageWorkspace _subject;

        private Mock<IContactListViewModel> _mockContactListViewModel;

        private Mock<IDetailsConductorViewModel> _mockDetailsConductorViewModel;

        private Mock<IDetailsViewModelFactory> _mockDetailsViewModelFactory;

        [SetUp]
        public void Setup()
        {
            _mockContactListViewModel = new Mock<IContactListViewModel>();
            _mockDetailsConductorViewModel = new Mock<IDetailsConductorViewModel>();
            _mockDetailsViewModelFactory = new Mock<IDetailsViewModelFactory>();
            _subject = new NewMessageWorkspace(_mockContactListViewModel.Object, _mockDetailsConductorViewModel.Object)
                           {
                               DetailsViewModelFactory = _mockDetailsViewModelFactory.Object
                           };
        }

        [Test]
        public void Constructor_Always_SetsCanSelectMultipleItemsToTrueOnTheContactListViewModel()
        {
	        _mockContactListViewModel.VerifySet(vm => vm.CanSelectMultipleItems = true);
        }

        [Test]
        public void Activate_DetailsConductorHasNoActiveItem_CreatesAndActivatesADetailsViewModelForTheSelectedContactsCollection()
        {
            var selectedContacts = new ObservableCollection<ContactModel>();
            _mockContactListViewModel.Setup(mock => mock.SelectedContacts).Returns(selectedContacts);
            var mockDetailsViewModel = SetupMockConversationViewModel();

            ((IActivate)_subject).Activate();

            _mockDetailsViewModelFactory.Verify(x => x.Create(selectedContacts), Times.Once());
            _mockDetailsConductorViewModel.Verify(x => x.ActivateItem(mockDetailsViewModel.Object), Times.Once());
        }

        [Test]
        public void Activate_DetailsConductorHasNoActiveItem_SetsTheHeaderOfTheCreatedConversationViewModelInEditMode()
        {
            var mockDetailsViewModel = SetupMockConversationViewModel();

            ((IActivate)_subject).Activate();

            ((IConversationHeaderViewModel)mockDetailsViewModel.Object.HeaderViewModel).State.Should()
                .Be(ConversationHeaderStateEnum.Edit);
        }

        private Mock<IDetailsViewModelWithHeader> SetupMockConversationViewModel()
        {
            var mockDetailsViewModel = new Mock<IDetailsViewModelWithHeader>();
            _mockDetailsViewModelFactory.Setup(x => x.Create(It.IsAny<ObservableCollection<ContactModel>>()))
                .Returns(mockDetailsViewModel.Object);
            var mockConversationHeaderViewModel = new Mock<IConversationHeaderViewModel>();
            mockConversationHeaderViewModel.SetupAllProperties();
            mockDetailsViewModel.SetupGet(x => x.HeaderViewModel).Returns(mockConversationHeaderViewModel.Object);

            return mockDetailsViewModel;
        }
    }
}