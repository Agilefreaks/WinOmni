namespace OmnipasteTests.Conversations.Conversation
{
    using System.Collections.ObjectModel;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Conversations.Conversation;
    using Omnipaste.Conversations.Conversation.SMSComposer;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;

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
                               Model = new ContactModel(new ContactEntity())
                           };
        }

        [Test]
        public void RecepientsSet_Always_SetsRecepientsOnViewModels()
        {
            var contactModels = new ObservableCollection<ContactModel>();
            _subject.Recipients = contactModels;

            _mockSmsComposerViewModel.VerifySet(m => m.Recipients = contactModels);
        }
    }
}