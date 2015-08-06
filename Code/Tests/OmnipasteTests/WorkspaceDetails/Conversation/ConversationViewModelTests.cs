namespace OmnipasteTests.WorkspaceDetails.Conversation
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.WorkspaceDetails.Conversation;

    [TestFixture]
    public class ConversationViewModelTests
    {
        private IConversationViewModel _subject;

        private Mock<IConversationHeaderViewModel> _mockConversationHeaderViewModel;

        private Mock<IConversationContainerViewModel> _mockConversationContainerViewModel;

        [SetUp]
        public void Setup()
        {

            _mockConversationContainerViewModel =  new Mock<IConversationContainerViewModel> { DefaultValue = DefaultValue.Mock };
            _mockConversationHeaderViewModel = new Mock<IConversationHeaderViewModel> { DefaultValue = DefaultValue.Mock };

            _subject = new ConversationViewModel(_mockConversationHeaderViewModel.Object, _mockConversationContainerViewModel.Object);
        }

        [Test]
        public void RecepientsSet_Always_SetsRecepientsOnViewModels()
        {
            var contactInfoPresenters = new ObservableCollection<ContactInfoPresenter>();
            _subject.Recipients = contactInfoPresenters;

            _mockConversationHeaderViewModel.VerifySet(m => m.Recipients = contactInfoPresenters);
            _mockConversationContainerViewModel.VerifySet(m => m.Recipients = contactInfoPresenters);
        }

        [Test]
        public void RecipientsCollectionChanged_WhenThereIsOnlyOneItem_SetsModelWithTheItemInTheCollection()
        {
            var contactInfoPresenter = new ContactInfoPresenter(new ContactInfo());
            var contactInfoPresenters = new ObservableCollection<ContactInfoPresenter>();
            _subject.Recipients = contactInfoPresenters;

            _subject.Activate();
            contactInfoPresenters.Add(contactInfoPresenter);

            _mockConversationHeaderViewModel.VerifySet(vm => vm.Model = contactInfoPresenter);
            _mockConversationContainerViewModel.VerifySet(vm => vm.Model = contactInfoPresenter);
        }

        [Test]
        public void RecepientsCollectionChanged_WhenMoreOrTwo_SetsModelToNull()
        {
            _subject.Recipients = new ObservableCollection<ContactInfoPresenter>();

            _subject.Activate();
            _subject.Recipients.Add(new ContactInfoPresenter(new ContactInfo()));
            _subject.Recipients.Add(new ContactInfoPresenter(new ContactInfo()));

            _mockConversationHeaderViewModel.VerifySet(m => m.Model = null);
            _mockConversationContainerViewModel.VerifySet(m => m.Model = null);
        }
    }
}

