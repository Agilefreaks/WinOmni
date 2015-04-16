namespace OmnipasteTests.ContactList.Contact
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Conversations;
    using Omnipaste.Conversations.ContactList.Contact;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Services;
    using Omnipaste.Framework.Services.Providers;
    using Omnipaste.Framework.Services.Repositories;
    using Omnipaste.Properties;
    using Omnipaste.WorkspaceDetails;
    using OmniUI.Details;
    using OmniUI.Framework.Services;
    using OmniUI.Workspaces;

    [TestFixture]
    public class ContactViewModelTests
    {
        private ContactViewModel _subject;

        private ContactModel _contactModel;

        private ContactEntity _contactEntity;

        private Mock<IContactRepository> _mockContactRepository;

        private Mock<IUiRefreshService> _mockUiRefreshService;

        private Mock<IDetailsViewModelFactory> _mockDetailsViewModelFactory;

        private TestScheduler _testScheduler;

        private Mock<IConversationProvider> _mockConversationProvider;

        private Mock<IConversationContext> _mockConversation;

        private Mock<ISessionManager> _mockSessionManager;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;

            _contactEntity = new ContactEntity { FirstName = "test", LastName = "test", IsStarred = false, PhoneNumbers = new[] { new PhoneNumber { Number = "42" } } };
            _contactModel = new ContactModel(_contactEntity);
            _mockContactRepository = new Mock<IContactRepository> { DefaultValue = DefaultValue.Mock };
            _mockUiRefreshService = new Mock<IUiRefreshService> { DefaultValue = DefaultValue.Mock };
            _mockDetailsViewModelFactory = new Mock<IDetailsViewModelFactory> { DefaultValue = DefaultValue.Mock };
            _mockConversationProvider = new Mock<IConversationProvider>();
            _mockConversation = new Mock<IConversationContext> { DefaultValue = DefaultValue.Mock };
            _mockConversationProvider.Setup(x=> x.ForContact(It.IsAny<ContactEntity>())).Returns(_mockConversation.Object);
            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };
            _mockSessionManager.SetupAllProperties();
            _subject = new ContactViewModel(_mockSessionManager.Object)
                           {
                               Model = _contactModel,
                               ContactRepository = _mockContactRepository.Object,
                               UiRefreshService = _mockUiRefreshService.Object,
                               DetailsViewModelFactory = _mockDetailsViewModelFactory.Object,
                               ConversationProvider = _mockConversationProvider.Object
                           };
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
        }

        [Test]
        public void IsSelected_WhenSessionSelectedContactIsSameAsContactId_ReturnsTrue()
        {
            var messageOperationObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<SessionItemChangeEventArgs>>(
                    200,
                    Notification.CreateOnNext(new SessionItemChangeEventArgs(ContactViewModel.SessionSelectionKey, _contactEntity.UniqueId, null))));
            _mockSessionManager.SetupGet(m => m.ItemChangedObservable).Returns(messageOperationObservable);
            
            _subject.OnLoaded();
            _testScheduler.Start();

            _subject.IsSelected.Should().BeTrue();
        }

        [Test]
        public void IsSelected_WhenSessionSelectedContactIsNotSameAsContactId_ReturnsFalse()
        {
            _subject.IsSelected = true;
            var messageOperationObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<SessionItemChangeEventArgs>>(
                    200,
                    Notification.CreateOnNext(new SessionItemChangeEventArgs(ContactViewModel.SessionSelectionKey, "other", null))));
            _mockSessionManager.SetupGet(m => m.ItemChangedObservable).Returns(messageOperationObservable);

            _subject.OnLoaded();
            _testScheduler.Start();

            _subject.IsSelected.Should().BeFalse();
        }

        [Test]
        public void OnIsStarredChanged_AfterActivate_UpdatesModel()
        {
            ((IActivate)_subject).Activate();

            _contactModel.IsStarred = true;

            _contactEntity.IsStarred.Should().BeTrue();
        }

        [Test]
        public void OnIsStarredChanged_AfterActivate_SavesModel()
        {
            ((IActivate)_subject).Activate();

            _contactModel.IsStarred = true;

            _mockContactRepository.Verify(m => m.Save(_contactEntity));
        }

        [Test]
        public void OnIsStarredChanged_AfterDeactivate_DoesNotSaveModel()
        {
            ((IActivate)_subject).Activate();
            ((IDeactivate)_subject).Deactivate(false);

            _contactModel.IsStarred = false;

            _mockContactRepository.Verify(m => m.Save(It.IsAny<ContactEntity>()), Times.Never());
        }

        [Test]
        public void ShowDetails_Always_ActivatesAnActivityDetailsViewModelInItsParentActivityWorkspace()
        {
            var mockWorkspace = new Mock<IConversationWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            mockDetailsConductor.Verify(x => x.ActivateItem(It.IsAny<IDetailsViewModelWithHeader>()), Times.Once());
        }

        [Test]
        public void ShowDetails_Always_StoresSelectedContactInSession()
        {
            var mockWorkspace = new Mock<IConversationWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            _subject.Parent = mockWorkspace.Object;
            
            _subject.ShowDetails();

            _mockSessionManager.VerifySet(m => m[ContactViewModel.SessionSelectionKey] = _contactEntity.UniqueId);
        }

        [Test]
        public void OnLoaded_WhenMessageIsLastConversationItemWithContact_PopulatesLastActivityInfoWithMessage()
        {
            var message = new RemoteSmsMessageModel(new RemoteSmsMessageEntity { Time = new DateTime(2014, 1, 1), Content = "test" });
            var call = new RemotePhoneCallModel(new RemotePhoneCallEntity { Time = new DateTime(2013, 12, 31) });
            SetupConversation(message, call);

            _subject.OnLoaded();
            _testScheduler.Start();

            _subject.LastActivityInfo.Should().Be(message.Content);
        }

        [Test]
        public void OnLoaded_WhenAllMessagesAreViewed_PopulatesHasNotViewedMessagesWithFalse()
        {
            var message = new RemoteSmsMessageModel(new RemoteSmsMessageEntity { Time = new DateTime(2014, 1, 1), Content = "test", WasViewed = true });
            var call = new RemotePhoneCallModel(new RemotePhoneCallEntity { Time = new DateTime(2013, 12, 31) });
            SetupConversation(message, call);

            _subject.OnLoaded();
            _testScheduler.Start();

            _subject.HasNotViewedMessages.Should().Be(false);
        }
        
        [Test]
        public void OnLoaded_WhenOneMessageIsNotViewed_PopulatesHasNotViewedMessagesWithTrue()
        {
            var message = new RemoteSmsMessageModel(new RemoteSmsMessageEntity { Time = new DateTime(2014, 1, 1), Content = "test", WasViewed = false });
            var call = new RemotePhoneCallModel(new RemotePhoneCallEntity { Time = new DateTime(2013, 12, 31) });
            SetupConversation(message, call);

            _subject.OnLoaded();
            _testScheduler.Start();

            _subject.HasNotViewedMessages.Should().Be(true);
        }

        [Test]
        public void OnLoaded_WhenRemoteCallIsLastConversationItemWithContact_PopulatesLastActivityInfoWithCallText()
        {
            var message = new RemoteSmsMessageModel(new RemoteSmsMessageEntity { Time = new DateTime(2013, 12, 31) });
            var call = new RemotePhoneCallModel(new RemotePhoneCallEntity { Time = new DateTime(2014, 1, 1) });
            SetupConversation(message, call);

            _subject.OnLoaded();
            _testScheduler.Start();

            _subject.LastActivityInfo.Should().Be(Resources.IncommingCallLabel);
        }

        [Test]
        public void OnLoaded_WhenLocalCallIsLastConversationItemWithContact_PopulatesLastActivityInfoWithCallText()
        {
            var message = new RemoteSmsMessageModel(new RemoteSmsMessageEntity { Time = new DateTime(2013, 12, 31), Content = "test" });
            var call = new LocalPhoneCallModel(new LocalPhoneCallEntity { Time = new DateTime(2014, 1, 1) });
            SetupConversation(message, call);

            _subject.OnLoaded();
            _testScheduler.Start();

            _subject.LastActivityInfo.Should().Be(Resources.OutgoingCallLabel);
        }

        [Test]
        public void OnLoaded_WhenAllCallsAreViewed_PopulatesHasNotViewedCallsWithFalse()
        {
            var message = new RemoteSmsMessageModel(new RemoteSmsMessageEntity { Time = new DateTime(2014, 1, 1), Content = "test" });
            var call = new RemotePhoneCallModel(new RemotePhoneCallEntity { Time = new DateTime(2013, 12, 31), WasViewed = true });
            SetupConversation(message, call);

            _subject.OnLoaded();
            _testScheduler.Start();

            _subject.HasNotViewedCalls.Should().Be(false);
        }

        [Test]
        public void OnLoaded_WhenOneCallIsNotViewed_PopulatesHasNotViewedCallsWithTrue()
        {
            var message = new RemoteSmsMessageModel(new RemoteSmsMessageEntity { Time = new DateTime(2014, 1, 1), Content = "test" });
            var call = new RemotePhoneCallModel(new RemotePhoneCallEntity { Time = new DateTime(2013, 12, 31), WasViewed = false });
            SetupConversation(message, call);

            _subject.OnLoaded();
            _testScheduler.Start();

            _subject.HasNotViewedCalls.Should().Be(true);
        }

        [Test]
        public void MessageIsAddedToConversation_AfterLoaded_PopulatesLastActivityInfoWithMessageContent()
        {
            var message = new RemoteSmsMessageModel(new RemoteSmsMessageEntity { Time = new DateTime(2013, 12, 31), Content = "test" });
            var messageOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IConversationModel>>(
                        200,
                        Notification.CreateOnNext((IConversationModel)message)));
            _mockConversation.SetupGet(x => x.Updated).Returns(messageOperationObservable);
            SetupConversation(message);
            _subject.OnLoaded();

            _testScheduler.AdvanceBy(1000);

            _subject.LastActivityInfo.Should().Be(message.Content);
        }

        private void SetupConversation(params IConversationModel[] items)
        {
            _mockConversation.Setup(x => x.GetItems()).Returns(items.ToObservable(_testScheduler));
        }
    }
}
