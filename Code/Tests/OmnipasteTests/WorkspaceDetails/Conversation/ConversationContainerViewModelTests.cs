namespace OmnipasteTests.WorkspaceDetails.Conversation
{
    using System.Collections.ObjectModel;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.SMSComposer;
    using Omnipaste.WorkspaceDetails.Conversation;

    [TestFixture]
    public class ConversationContainerViewModelTests
    {
        private IConversationContainerViewModel _subject;

        private Mock<ISMSComposerViewModel> _mockSmsComposerViewModel;

        private Mock<IConversationContentViewModel> _mockConversationContentViewModel;

        [SetUp]
        public void SetUp()
        {
            _mockSmsComposerViewModel = new Mock<ISMSComposerViewModel>();
            _mockConversationContentViewModel = new Mock<IConversationContentViewModel>();

            _subject = new ConversationContainerViewModel
                           {
                               SMSComposer = _mockSmsComposerViewModel.Object,
                               ConversationContentViewModel = _mockConversationContentViewModel.Object,
                               Model = new ContactInfoPresenter(new ContactEntity())
                           };
        }

        [Test]
        public void RecepientsSet_Always_SetsRecepientsOnViewModels()
        {
            var contactInfoPresenters = new ObservableCollection<ContactInfoPresenter>();
            _subject.Recipients = contactInfoPresenters;

            _mockSmsComposerViewModel.VerifySet(m => m.Recipients = contactInfoPresenters);
        }

        [Test]
        public void Recepients_WhenMoreOrTwo_SetsModelToNullOnConversationContentViewModel()
        {
            _subject.Recipients = new ObservableCollection<ContactInfoPresenter>();
            
            _subject.Activate();
            _subject.Recipients.Add(new ContactInfoPresenter(new ContactEntity()));
            _subject.Recipients.Add(new ContactInfoPresenter(new ContactEntity()));

            _mockConversationContentViewModel.VerifySet(m => m.Model = null);
        }
    }
}