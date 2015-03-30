namespace OmnipasteTests.WorkspaceDetails.Conversation
{
    using System.Collections.ObjectModel;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Presenters;
    using Omnipaste.SMSComposer;
    using Omnipaste.WorkspaceDetails.Conversation;

    [TestFixture]
    public class ConversationContainerViewModelTests
    {
        private IConversationContainerViewModel _subject;

        private Mock<ISMSComposerViewModel> _mockSmsComposerViewModel;

        [SetUp]
        public void SetUp()
        {
            _mockSmsComposerViewModel = new Mock<ISMSComposerViewModel>();

            _subject = new ConversationContainerViewModel { SMSComposer = _mockSmsComposerViewModel.Object };
        }

        [Test]
        public void RecepientsSet_Always_SetsRecepientsOnViewModels()
        {
            var contactInfoPresenters = new ObservableCollection<ContactInfoPresenter>();
            _subject.Recipients = contactInfoPresenters;

            _mockSmsComposerViewModel.VerifySet(m => m.Recipients = contactInfoPresenters);
        }
    }
}