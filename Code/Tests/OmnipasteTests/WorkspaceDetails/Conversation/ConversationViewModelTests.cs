namespace OmnipasteTests.WorkspaceDetails.Conversation
{
    using System.Collections.ObjectModel;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Conversations.Conversation;
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

            _mockConversationContainerViewModel =  new Mock<IConversationContainerViewModel> { DefaultValue = DefaultValue.Mock };
            _mockConversationHeaderViewModel = new Mock<IConversationHeaderViewModel> { DefaultValue = DefaultValue.Mock };

            _subject = new ConversationViewModel(_mockConversationHeaderViewModel.Object, _mockConversationContainerViewModel.Object);
        }

        [Test]
        public void RecepientsSet_Always_SetsRecepientsOnViewModels()
        {
            var contactModels = new ObservableCollection<ContactModel>();
            _subject.Recipients = contactModels;

            _mockConversationHeaderViewModel.VerifySet(m => m.Recipients = contactModels);
            _mockConversationContainerViewModel.VerifySet(m => m.Recipients = contactModels);
        }
    }
}