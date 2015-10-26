namespace OmnipasteTests.Conversations.Conversation
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Conversations.Conversation;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;

    [TestFixture]
    public class ConversationViewModelTests
    {
        private IConversationViewModel _subject;

        private Mock<IConversationHeaderViewModel> _mockConversationHeaderViewModel;

        private Mock<IConversationContainerViewModel> _mockConversationContainerViewModel;

        [SetUp]
        public void Setup()
        {

            _mockConversationContainerViewModel = new Mock<IConversationContainerViewModel>
                                                      {
                                                          DefaultValue = DefaultValue.Mock
                                                      };
            _mockConversationHeaderViewModel = new Mock<IConversationHeaderViewModel>
                                                   {
                                                       DefaultValue = DefaultValue.Mock
                                                   };

            _subject = new ConversationViewModel(
                _mockConversationHeaderViewModel.Object,
                _mockConversationContainerViewModel.Object);
        }

        [Test]
        public void RecepientsSet_Always_SetsRecepientsOnViewModels()
        {
            var contactModels = new ObservableCollection<ContactModel>();
            _subject.Recipients = contactModels;

            _mockConversationHeaderViewModel.VerifySet(m => m.Recipients = contactModels);
            _mockConversationContainerViewModel.VerifySet(m => m.Recipients = contactModels);
        }

        [Test]
        public void RecipientsCollectionChanged_WhenThereIsOnlyOneItem_SetsModelWithTheItemInTheCollection()
        {
            var contactInfoPresenter = new ContactModel(new ContactEntity());
            var contactInfoPresenters = new ObservableCollection<ContactModel>();
            _subject.Recipients = contactInfoPresenters;

            _subject.Activate();
            contactInfoPresenters.Add(contactInfoPresenter);

            _mockConversationHeaderViewModel.VerifySet(vm => vm.Model = contactInfoPresenter);
            _mockConversationContainerViewModel.VerifySet(vm => vm.Model = contactInfoPresenter);
        }

        [Test]
        public void RecepientsCollectionChanged_WhenMoreOrTwo_SetsModelToNull()
        {
            _subject.Recipients = new ObservableCollection<ContactModel>();

            _subject.Activate();
            _subject.Recipients.Add(new ContactModel(new ContactEntity()));
            _subject.Recipients.Add(new ContactModel(new ContactEntity()));

            _mockConversationHeaderViewModel.VerifySet(m => m.Model = null);
            _mockConversationContainerViewModel.VerifySet(m => m.Model = null);
        }

        [Test]
        public void OnDeactivate_HeaderStateIsDeleted_SetsOwnStateToDeleted()
        {
            _subject.Activate();
            _mockConversationHeaderViewModel.Setup(x => x.State).Returns(ConversationHeaderStateEnum.Deleted);

            _subject.Deactivate(false);

            _subject.State.Should().Be(ConversationViewModelStateEnum.Deleted);
        }

        [Test]
        public void RecepientsCollectionChanged_WhenTwoOrMoreRecipientAreSelectedAndViewModelWasDeactivated_DoesNotModifyModel()
        {
            _subject.Recipients = new ObservableCollection<ContactModel>();
            _subject.Activate();
            _subject.Recipients.Add(new ContactModel(new ContactEntity()));
            _subject.Deactivate(false);

            _subject.Recipients.Add(new ContactModel(new ContactEntity()));

            _mockConversationHeaderViewModel.VerifySet(m => m.Model = null, Times.Never);
            _mockConversationContainerViewModel.VerifySet(m => m.Model = null, Times.Never);
        }
    }
}