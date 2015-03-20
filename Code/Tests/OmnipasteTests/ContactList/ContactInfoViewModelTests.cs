/* TODO
namespace OmnipasteTests.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.ContactList;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.Workspaces;
    using OmnipasteTests.Helpers;
    using OmniUI.Workspace;

    [TestFixture]
    public class ContactInfoViewModelTests
    {
        private ContactInfoViewModel _subject;

        private ContactInfoPresenter _contactInfoPresenter;

        private ContactInfo _contactInfo;

        private Mock<IContactRepository> _mockContactRepository;

        private Mock<IUiRefreshService> _mockUiRefreshService;

        private Mock<IWorkspaceDetailsViewModelFactory> _mockDetailsViewModelFactory;

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

            _contactInfo = new ContactInfo { FirstName = "test", LastName = "test", IsStarred = false, PhoneNumbers = new[] { new PhoneNumber { Number = "42" } } };
            _contactInfoPresenter = new ContactInfoPresenter(_contactInfo);
            _mockContactRepository = new Mock<IContactRepository> { DefaultValue = DefaultValue.Mock };
            _mockUiRefreshService = new Mock<IUiRefreshService> { DefaultValue = DefaultValue.Mock };
            _mockDetailsViewModelFactory = new Mock<IWorkspaceDetailsViewModelFactory> { DefaultValue = DefaultValue.Mock };
            _mockConversationProvider = new Mock<IConversationProvider>();
            _mockConversation = new Mock<IConversationContext> { DefaultValue = DefaultValue.Mock };
            _mockConversationProvider.Setup(x=> x.ForContact(It.IsAny<ContactInfo>())).Returns(_mockConversation.Object);
            _mockSessionManager = new Mock<ISessionManager> { DefaultValue = DefaultValue.Mock };
            _mockSessionManager.SetupAllProperties();
            _subject = new ContactInfoViewModel(_mockSessionManager.Object)
                           {
                               Model = _contactInfoPresenter,
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
        public void IsSelected_WhenSessionSelectedContactIsSameAsContactInfoId_ReturnsTrue()
        {
            _mockSessionManager.SetupGet(m => m[ContactInfoViewModel.SessionSelectionKey])
                .Returns(_contactInfo.UniqueId);

            _subject.IsSelected.Should().BeTrue();
        }

        [Test]
        public void IsSelected_WhenSessionSelectedContactIsNotSameAsContactInfoId_ReturnsFalse()
        {
            _mockSessionManager.SetupGet(m => m[ContactInfoViewModel.SessionSelectionKey])
                .Returns("other");

            _subject.IsSelected.Should().BeFalse();
        }

        [Test]
        public void OnIsStarredChanged_AfterActivate_UpdatesModel()
        {
            ((IActivate)_subject).Activate();

            _contactInfoPresenter.IsStarred = true;

            _contactInfo.IsStarred.Should().BeTrue();
        }

        [Test]
        public void OnIsStarredChanged_AfterActivate_SavesModel()
        {
            ((IActivate)_subject).Activate();

            _contactInfoPresenter.IsStarred = true;

            _mockContactRepository.Verify(m => m.Save(_contactInfo));
        }

        [Test]
        public void OnIsStarredChanged_AfterDeactivate_DoesNotSaveModel()
        {
            ((IActivate)_subject).Activate();
            ((IDeactivate)_subject).Deactivate(false);

            _contactInfoPresenter.IsStarred = false;

            _mockContactRepository.Verify(m => m.Save(It.IsAny<ContactInfo>()), Times.Never());
        }

        [Test]
        public void ShowDetails_Always_ActivatesAnActivityDetailsViewModelInItsParentActivityWorkspace()
        {
            var mockWorkspace = new Mock<IPeopleWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            mockDetailsConductor.Verify(x => x.ActivateItem(It.IsAny<IWorkspaceDetailsViewModel>()), Times.Once());
        }

        [Test]
        public void ShowDetails_Always_StoresSelectedContactInfoInSession()
        {
            var mockWorkspace = new Mock<IPeopleWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            _subject.Parent = mockWorkspace.Object;
            
            _subject.ShowDetails();

            _mockSessionManager.VerifySet(m => m[ContactInfoViewModel.SessionSelectionKey] = _contactInfo.UniqueId);
        }

        [Test]
        public void OnLoaded_WhenMessageIsLastConversationItemWithContact_PopulatesLastActivityInfoWithMessage()
        {
            var message = new TestSmsMessage { Time = new DateTime(2014, 1, 1), Content = "test", ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            var call = new RemotePhoneCall { Time = new DateTime(2013, 12, 31), ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            SetupConversation(message, call);

            _subject.OnLoaded();
            _testScheduler.AdvanceBy(1000);

            _subject.LastActivityInfo.Should().Be(message.Content);
        }

        [Test]
        public void OnLoaded_WhenAllMessagesAreViewed_PopulatesHasNotViewedMessagesWithFalse()
        {
            var message = new TestSmsMessage { Time = new DateTime(2014, 1, 1), Content = "test", WasViewed = true, ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            var call = new RemotePhoneCall { Time = new DateTime(2013, 12, 31), ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            SetupConversation(message, call);

            _subject.OnLoaded();
            _testScheduler.AdvanceBy(1000);

            _subject.HasNotViewedMessages.Should().Be(false);
        }

        [Test]
        public void OnLoaded_WhenOneMessageIsNotViewed_PopulatesHasNotViewedMessagesWithTrue()
        {
            var message = new TestSmsMessage { Time = new DateTime(2014, 1, 1), Content = "test", WasViewed = false, ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            var call = new RemotePhoneCall { Time = new DateTime(2013, 12, 31), ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            SetupConversation(message, call);

            _subject.OnLoaded();
            _testScheduler.AdvanceBy(1000);

            _subject.HasNotViewedMessages.Should().Be(true);
        }

        [Test]
        public void OnLoaded_WhenRemoteCallIsLastConversationItemWithContact_PopulatesLastActivityInfoWithCallText()
        {
            var message = new TestSmsMessage { Time = new DateTime(2013, 12, 31), Content = "test", ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            var call = new RemotePhoneCall { Time = new DateTime(2014, 1, 1), ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            SetupConversation(message, call);

            _subject.OnLoaded();
            _testScheduler.AdvanceBy(1000);

            _subject.LastActivityInfo.Should().Be(Omnipaste.Properties.Resources.IncommingCallLabel);
        }

        [Test]
        public void OnLoaded_WhenLocalCallIsLastConversationItemWithContact_PopulatesLastActivityInfoWithCallText()
        {
            var message = new TestSmsMessage { Time = new DateTime(2013, 12, 31), Content = "test", ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            var call = new LocalPhoneCall { Time = new DateTime(2014, 1, 1), ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            SetupConversation(message, call);

            _subject.OnLoaded();
            _testScheduler.AdvanceBy(1000);

            _subject.LastActivityInfo.Should().Be(Omnipaste.Properties.Resources.OutgoingCallLabel);
        }

        [Test]
        public void OnLoaded_WhenAllCallsAreViewed_PopulatesHasNotViewedCallsWithFalse()
        {
            var message = new TestSmsMessage { Time = new DateTime(2014, 1, 1), Content = "test", ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            var call = new RemotePhoneCall { Time = new DateTime(2013, 12, 31), WasViewed = true, ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            SetupConversation(message, call);

            _subject.OnLoaded();
            _testScheduler.AdvanceBy(1000);

            _subject.HasNotViewedCalls.Should().Be(false);
        }

        [Test]
        public void OnLoaded_WhenOneCallIsNotViewed_PopulatesHasNotViewedCallsWithTrue()
        {
            var message = new TestSmsMessage { Time = new DateTime(2014, 1, 1), Content = "test", ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            var call = new RemotePhoneCall { Time = new DateTime(2013, 12, 31), WasViewed = false, ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            SetupConversation(message, call);

            _subject.OnLoaded();
            _testScheduler.AdvanceBy(1000);

            _subject.HasNotViewedCalls.Should().Be(true);
        }

        [Test]
        public void MessageIsAddedToConversation_AfterLoaded_PopulatesLastActivityInfoWithMessageContent()
        {
            var message = new TestSmsMessage { Time = new DateTime(2013, 12, 31), Content = "test", ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            var messageOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IConversationPresenter>>(
                        200,
                        Notification.CreateOnNext((IConversationPresenter)message)));
            _mockConversation.SetupGet(x => x.Updated).Returns(messageOperationObservable);
            SetupConversation(message);
            _subject.OnLoaded();

            _testScheduler.AdvanceBy(1000);

            _subject.LastActivityInfo.Should().Be(message.Content);
        }

        [Test]
        public void MessageIsDeletedFromConversation_AfterLoaded_SetsLastActivityInfoToEmptyString()
        {
            var message = new TestSmsMessage { Time = new DateTime(2013, 12, 31), Content = "test", ContactInfo = new ContactInfo { PhoneNumbers = _contactInfo.PhoneNumbers } };
            var observable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IConversationPresenter>>(
                        200,
                        Notification.CreateOnNext((IConversationPresenter)message)));
            _mockConversation.SetupGet(x => x.Updated).Returns(observable);
            var observable1 =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<IConversationPresenter>>>(
                        100,
                        Notification.CreateOnNext(new List<IConversationPresenter> { message }.AsEnumerable())));
            var observable2 =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<IConversationPresenter>>>(
                        100,
                        Notification.CreateOnNext(Enumerable.Empty<IConversationPresenter>())));
            var results = new[] { observable1, observable2 };
            var index = 0;
            _mockConversation.Setup(x => x.GetItems()).Returns(() => results[index++]);
            _subject.OnLoaded();

            _testScheduler.AdvanceBy(1000);

            _subject.LastActivityInfo.Should().Be(string.Empty);
        }

        private void SetupConversation(params IConversationPresenter[] items)
        {
            var observable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<IConversationPresenter>>>(
                        100,
                        Notification.CreateOnNext(items.AsEnumerable())));
            _mockConversation.Setup(x => x.GetItems()).Returns(observable);
        }
    }
}
*/
