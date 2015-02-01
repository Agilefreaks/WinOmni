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
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.Workspaces;
    using OmniUI.Workspace;

    [TestFixture]
    public class ContactInfoViewModelTests
    {
        private ContactInfoViewModel _subject;

        private ContactInfoPresenter _contactInfoPresenter;

        private ContactInfo _contactInfo;

        private Mock<IContactRepository> _mockContactRepository;

        private Mock<IMessageRepository> _mockMessageRepository;

        private Mock<ICallRepository> _mockCallRepository;

        private Mock<IUiRefreshService> _mockUiRefreshService;

        private Mock<IWorkspaceDetailsViewModelFactory> _mockDetailsViewModelFactory;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            SchedulerProvider.Dispatcher = _testScheduler;

            _contactInfo = new ContactInfo { FirstName = "test", LastName = "test", IsStarred = false };
            _contactInfoPresenter = new ContactInfoPresenter(_contactInfo);
            _mockContactRepository = new Mock<IContactRepository> { DefaultValue = DefaultValue.Mock };
            _mockMessageRepository = new Mock<IMessageRepository> { DefaultValue = DefaultValue.Mock };
            _mockCallRepository = new Mock<ICallRepository> { DefaultValue = DefaultValue.Mock };
            _mockUiRefreshService = new Mock<IUiRefreshService> { DefaultValue = DefaultValue.Mock };
            _mockDetailsViewModelFactory = new Mock<IWorkspaceDetailsViewModelFactory> { DefaultValue = DefaultValue.Mock };

            _subject = new ContactInfoViewModel
                           {
                               Model = _contactInfoPresenter,
                               ContactRepository = _mockContactRepository.Object,
                               MessageRepository = _mockMessageRepository.Object,
                               CallRepository = _mockCallRepository.Object,
                               UiRefreshService = _mockUiRefreshService.Object,
                               DetailsViewModelFactory = _mockDetailsViewModelFactory.Object
                           };
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
            SchedulerProvider.Dispatcher = null;
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
            var mockWorkspace = new Mock<IMessageWorkspace>();
            var mockDetailsConductor = new Mock<IDetailsConductorViewModel>();
            mockWorkspace.SetupGet(x => x.DetailsConductor).Returns(mockDetailsConductor.Object);
            _subject.Parent = mockWorkspace.Object;

            _subject.ShowDetails();

            mockDetailsConductor.Verify(x => x.ActivateItem(It.IsAny<IWorkspaceDetailsViewModel>()), Times.Once());
        }

        [Test]
        public void OnLoaded_WhenMessageIsLastConversationItemWithContact_PopulatesLastActivityInfoWithMessage()
        {
            var message = new Message { Time = new DateTime(2014, 1, 1), Content = "test" };
            var messageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Message>>>(
                        100,
                        Notification.CreateOnNext(new List<Message> { message }.AsEnumerable())));
            var call = new Call { Time = new DateTime(2013, 12, 31), Source = SourceType.Remote };
            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Call>>>(
                        100,
                        Notification.CreateOnNext(new List<Call> { call }.AsEnumerable())));
            _mockMessageRepository.Setup(m => m.GetAll(It.IsAny<Func<Message, bool>>())).Returns(messageObservable);
            _mockCallRepository.Setup(m => m.GetAll(It.IsAny<Func<Call, bool>>())).Returns(callObservable);

            _subject.OnLoaded();
            _testScheduler.AdvanceBy(1000);

            _subject.LastActivityInfo.Should().Be(message.Content);
        }

        [Test]
        public void OnLoaded_WhenMessageIsLastConversationItemWithContact_PopulatesLastActivityTimeWithMessageTime()
        {
            var message = new Message { Time = new DateTime(2014, 1, 1), Content = "test" };
            var messageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Message>>>(
                        100,
                        Notification.CreateOnNext(new List<Message> { message }.AsEnumerable())));
            var call = new Call { Time = new DateTime(2013, 12, 31), Source = SourceType.Remote };
            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Call>>>(
                        100,
                        Notification.CreateOnNext(new List<Call> { call }.AsEnumerable())));
            _mockMessageRepository.Setup(m => m.GetAll(It.IsAny<Func<Message, bool>>())).Returns(messageObservable);
            _mockCallRepository.Setup(m => m.GetAll(It.IsAny<Func<Call, bool>>())).Returns(callObservable);

            _subject.OnLoaded();
            _testScheduler.AdvanceBy(1000);

            _subject.LastActivityTime.Value.Should().Be(message.Time);
        }

        [Test]
        public void OnLoaded_WhenRemoteCallIsLastConversationItemWithContact_PopulatesLastActivityInfoWithCallText()
        {
            var message = new Message { Time = new DateTime(2013, 12, 31), Content = "test" };
            var messageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Message>>>(
                        100,
                        Notification.CreateOnNext(new List<Message> { message }.AsEnumerable())));
            var call = new Call { Time = new DateTime(2014, 1, 1), Source = SourceType.Remote };
            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Call>>>(
                        100,
                        Notification.CreateOnNext(new List<Call> { call }.AsEnumerable())));
            _mockMessageRepository.Setup(m => m.GetAll(It.IsAny<Func<Message, bool>>())).Returns(messageObservable);
            _mockCallRepository.Setup(m => m.GetAll(It.IsAny<Func<Call, bool>>())).Returns(callObservable);

            _subject.OnLoaded();
            _testScheduler.AdvanceBy(1000);

            _subject.LastActivityInfo.Should().Be(Omnipaste.Properties.Resources.IncommingCallLabel);
        }

        [Test]
        public void OnLoaded_WhenLocalCallIsLastConversationItemWithContact_PopulatesLastActivityInfoWithCallText()
        {
            var message = new Message { Time = new DateTime(2013, 12, 31), Content = "test" };
            var messageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Message>>>(
                        100,
                        Notification.CreateOnNext(new List<Message> { message }.AsEnumerable())));
            var call = new Call { Time = new DateTime(2014, 1, 1), Source = SourceType.Local };
            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Call>>>(
                        100,
                        Notification.CreateOnNext(new List<Call> { call }.AsEnumerable())));
            _mockMessageRepository.Setup(m => m.GetAll(It.IsAny<Func<Message, bool>>())).Returns(messageObservable);
            _mockCallRepository.Setup(m => m.GetAll(It.IsAny<Func<Call, bool>>())).Returns(callObservable);

            _subject.OnLoaded();
            _testScheduler.AdvanceBy(1000);

            _subject.LastActivityInfo.Should().Be(Omnipaste.Properties.Resources.OutgoingCallLabel);
        }

        [Test]
        public void MessageIsSaved_AfterLoaded_PopulatesLastActivityInfoWithMessageContent()
        {
            var message = new Message { Time = new DateTime(2013, 12, 31), Content = "test" };
            var messageOperationObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<RepositoryOperation<Message>>>(
                        200,
                        Notification.CreateOnNext(
                            new RepositoryOperation<Message>(RepositoryMethodEnum.Create, message))));
            _mockMessageRepository.SetupGet(m => m.OperationObservable).Returns(messageOperationObservable);
            var messageObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Message>>>(
                        100,
                        Notification.CreateOnNext(new List<Message> { message }.AsEnumerable())));
            _mockMessageRepository.Setup(m => m.GetAll(It.IsAny<Func<Message, bool>>())).Returns(messageObservable);
            var callObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<IEnumerable<Call>>>(
                        100,
                        Notification.CreateOnNext(Enumerable.Empty<Call>())));
            _mockCallRepository.Setup(m => m.GetAll(It.IsAny<Func<Call, bool>>())).Returns(callObservable);
            _subject.OnLoaded();

            _testScheduler.AdvanceBy(1000);

            _subject.LastActivityInfo.Should().Be(message.Content);
        }
    }
}
