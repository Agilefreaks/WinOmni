namespace OmnipasteTests.Conversations
{
    using System.Collections.ObjectModel;
    using System.Threading;
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Conversations;
    using Omnipaste.Conversations.ContactList;
    using Omnipaste.Conversations.Conversation;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using OmniUI.Workspaces;

    [TestFixture]
    public class NewMessageWorkspaceTests
    {
        private INewMessageWorkspace _subject;

        private Mock<IContactListViewModel> _mockContactListViewModel;

        private Mock<IDetailsConductorViewModel> _mockDetailsConductorViewModel;

        private Mock<IConversationViewModel> _mockConversationViewModel;

        private Mock<IConversationHeaderViewModel> _mockConversationHeaderViewModel;

        private ObservableCollection<ContactModel> _selectedContacts;

        [SetUp]
        public void Setup()
        {
            _mockContactListViewModel = new Mock<IContactListViewModel>();
            _selectedContacts = new ObservableCollection<ContactModel>();
            _mockContactListViewModel.Setup(x => x.SelectedContacts).Returns(_selectedContacts);
            _mockConversationViewModel = new Mock<IConversationViewModel>();
            _mockConversationHeaderViewModel = new Mock<IConversationHeaderViewModel>();
            _mockConversationHeaderViewModel.SetupAllProperties();
            _mockConversationHeaderViewModel.Object.Recipients = new ObservableCollection<ContactModel>();
            _mockConversationViewModel.Setup(x => x.HeaderViewModel).Returns(_mockConversationHeaderViewModel.Object);
            _mockDetailsConductorViewModel = new Mock<IDetailsConductorViewModel>();
            _mockDetailsConductorViewModel.SetupProperty(x => x.ActiveItem);
            _subject = new NewMessageWorkspace(
                _mockContactListViewModel.Object,
                _mockConversationViewModel.Object,
                _mockDetailsConductorViewModel.Object);
        }

        [Test]
        public void Constructor_Always_SetsCanSelectMultipleItemsToFalseOnTheContactListViewModel()
        {
	        _mockContactListViewModel.VerifySet(vm => vm.CanSelectMultipleItems = false);
        }

        [Test]
        public void Constructor_Always_SetsTheConversationViewModelAsTheActiveItemInTheDetailsCondctorViewModel()
        {
            _mockDetailsConductorViewModel.Object.ActiveItem.Should().Be(_mockConversationViewModel.Object);
        }

        [Test]
        public void Constructor_Always_SetsTheHeaderOfTheCreatedConversationViewModelInEditMode()
        {
            _mockConversationHeaderViewModel.Object.State.Should().Be(ConversationHeaderStateEnum.Edit);
        }

        [Test]
        public void OnActivate_ANewSelectedContactIsAddedInTheContactListViewModel_AddsTheContactInTheRecipientsListOfTheConversationHeader()
        {
            var contactModel = new ContactModel(new ContactEntity());

            ((IActivate)_subject).Activate();
            _selectedContacts.Add(contactModel);

            _mockConversationHeaderViewModel.Object.Recipients.Contains(contactModel).Should().BeTrue();
        }

        [Test]
        public void Deactivate_TheConversationModelIsInTheDeletedState_ClearsTheSelectedContacts()
        {
            var contactCollection = new ObservableCollection<ContactModel>();
            _mockContactListViewModel.Setup(x => x.SelectedContacts).Returns(contactCollection);
            contactCollection.Add(new ContactModel(new ContactEntity()));
            _subject.Activate();
            _mockConversationViewModel.Setup(x => x.State).Returns(ConversationViewModelStateEnum.Deleted);

            _subject.Deactivate(false);

            contactCollection.Count.Should().Be(0);
        }

        [Test]
        public void Deactivate_TheConversationModelIsInTheDeletedState_SetsTheConversationHeaderInEditMode()
        {
            _subject.Activate();
            _mockConversationViewModel.Setup(x => x.State).Returns(ConversationViewModelStateEnum.Deleted);

            _subject.Deactivate(false);

            _mockConversationHeaderViewModel.VerifySet(x => x.State = ConversationHeaderStateEnum.Edit);
        }

        [Test]
        public void Deactivate_TheConversationModelIsInTheDeletedState_SetsRecipientsToAnEmptyCollection()
        {
            _subject.Activate();
            _mockConversationViewModel.Setup(x => x.State).Returns(ConversationViewModelStateEnum.Deleted);

            _subject.Deactivate(false);

            _mockConversationViewModel.VerifySet(x => x.Recipients = It.IsAny<ObservableCollection<ContactModel>>());
        }

        [Test]
        public void Deactivate_TheConversationModelIsInTheDeletedState_SetsTheStateOfTheConversationViewModelToNormal()
        {
            _subject.Activate();
            _mockConversationViewModel.Setup(x => x.State).Returns(ConversationViewModelStateEnum.Deleted);

            _subject.Deactivate(false);

            _mockConversationViewModel.VerifySet(x => x.State = ConversationViewModelStateEnum.Normal);
        }
    }
}