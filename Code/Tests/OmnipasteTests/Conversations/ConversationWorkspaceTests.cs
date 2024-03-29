﻿namespace OmnipasteTests.Conversations
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
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using OmniUI.Details;
    using OmniUI.Workspaces;

    [TestFixture]
    public class ConversationWorkspaceTests
    {
        private IConversationWorkspace _subject;

        private Mock<IContactListViewModel> _mockContactListViewModel;

        private Mock<IDetailsConductorViewModel> _mockDetailsConductor;

        private Mock<IDetailsViewModelFactory> _mockDetailsViewModelFactory;

        [SetUp]
        public void Setup()
        {
            _mockContactListViewModel = new Mock<IContactListViewModel>();
            _mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            _subject = new ConversationWorkspace(_mockContactListViewModel.Object, _mockDetailsConductor.Object);
            _mockDetailsViewModelFactory = new Mock<IDetailsViewModelFactory>();
            _subject.DetailsViewModelFactory = _mockDetailsViewModelFactory.Object;
        }

        [Test]
        public void SelectedContactsAreCleared_WorkspaceIsActivatedAndADetailsViewModelsIsCurrentlyActive_ItDeactivatesTheDetailsViewModel()
        {
            var activeDetails = new Mock<IScreen>();
            _mockDetailsConductor.SetupGet(x => x.ActiveItem).Returns(activeDetails);
            var observableCollection = new ObservableCollection<ContactModel> { new ContactModel(new ContactEntity()) };
            _mockContactListViewModel.SetupGet(x => x.SelectedContacts).Returns(observableCollection);
            _subject.Activate();

            observableCollection.Clear();

            _mockDetailsConductor.Verify(x => x.DeactivateItem(activeDetails, true));
        }

        [Test]
        public void AContactIsSelected_WorkspaceIsActivatedAndANoDetailsViewModelsIsCurrentlyActive_ItCreatesACorrespondingDetailsViewModel()
        {
            _mockDetailsConductor.SetupGet(x => x.ActiveItem).Returns(null);
            var observableCollection = new ObservableCollection<ContactModel>();
            _mockContactListViewModel.SetupGet(x => x.SelectedContacts).Returns(observableCollection);
            _subject.Activate();

            observableCollection.Add(new ContactModel(new ContactEntity()));

            _mockDetailsViewModelFactory.Verify(x => x.Create(observableCollection));
        }

        [Test]
        public void AContactIsSelected_WorkspaceIsActivatedAndANoDetailsViewModelsIsCurrentlyActive_ItActivatesTheCreatedDetailsViewModel()
        {
            _mockDetailsConductor.SetupGet(x => x.ActiveItem).Returns(null);
            var mockDetailsViewModel = new Mock<IDetailsViewModelWithHeader>();
            _mockDetailsViewModelFactory.Setup(x => x.Create(It.IsAny<ObservableCollection<ContactModel>>())).Returns(mockDetailsViewModel.Object);
            var observableCollection = new ObservableCollection<ContactModel>();
            _mockContactListViewModel.SetupGet(x => x.SelectedContacts).Returns(observableCollection);
            _subject.Activate();

            observableCollection.Add(new ContactModel(new ContactEntity()));

            _mockDetailsConductor.Verify(x => x.ActivateItem(mockDetailsViewModel.Object));
        }

        [Test]
        public void Deactivate_AConversationViewModelIsInTheDeletedState_ClosesTheConversation()
        {
            var mockConversationViewModel = new Mock<IConversationViewModel>();
            _mockDetailsConductor.Setup(x => x.ActiveItem).Returns(mockConversationViewModel.Object);
            _mockContactListViewModel.Setup(x => x.SelectedContacts).Returns(new ObservableCollection<ContactModel>());
            _subject.Activate();
            mockConversationViewModel.Setup(x => x.State).Returns(ConversationViewModelStateEnum.Deleted);

            _subject.Deactivate(false);

            _mockDetailsConductor.Verify(x => x.DeactivateItem(mockConversationViewModel.Object, true));
        }

        [Test]
        public void Deactivate_AConversationViewModelIsInTheDeletedState_ClearsTheCurrentContactSelection()
        {
            var contacts = new ObservableCollection<ContactModel> { new ContactModel(new ContactEntity()) };
            var mockConversationViewModel = new Mock<IConversationViewModel>();
            _mockDetailsConductor.Setup(x => x.ActiveItem).Returns(mockConversationViewModel.Object);
            _mockContactListViewModel.Setup(x => x.SelectedContacts).Returns(contacts);
            _subject.Activate();
            mockConversationViewModel.Setup(x => x.State).Returns(ConversationViewModelStateEnum.Deleted);

            _subject.Deactivate(false);

            contacts.Count.Should().Be(0);
        }
    }
}