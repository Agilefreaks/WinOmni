﻿namespace OmnipasteTests.WorkspaceDetails.Conversation
{
    using System.Collections.ObjectModel;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Models;
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
            var contactInfoPresenters = new ObservableCollection<ContactModel>();
            _subject.Recipients = contactInfoPresenters;

            _mockConversationHeaderViewModel.VerifySet(m => m.Recipients = contactInfoPresenters);
            _mockConversationContainerViewModel.VerifySet(m => m.Recipients = contactInfoPresenters);
        }
    }
}