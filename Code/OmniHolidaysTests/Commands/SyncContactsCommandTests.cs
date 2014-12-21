namespace OmniHolidaysTests.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using Contacts.Api.Resources.v1;
    using Contacts.Handlers;
    using Contacts.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using OmniHolidays.Commands;

    [TestFixture]
    public class SyncContactsCommandTests
    {
        private SyncContactsCommand _subject;

        private TestScheduler _testScheduler;

        private Mock<IContacts> _mockContacts;

        private Mock<IContactsHandler> _mockContactsHandler;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<ISyncs> _mockSyncs;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            _mockContacts = new Mock<IContacts> { DefaultValue = DefaultValue.Mock };
            _mockSyncs = new Mock<ISyncs> { DefaultValue = DefaultValue.Mock };
            _mockContactsHandler = new Mock<IContactsHandler> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService.Setup(m => m.DeviceIdentifier).Returns("42");

            SchedulerProvider.Default = _testScheduler;

            _subject = new SyncContactsCommand
            {
                Contacts = _mockContacts.Object,
                Syncs = _mockSyncs.Object,
                ContactsHandler = _mockContactsHandler.Object,
                ConfigurationService = _mockConfigurationService.Object
            };
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void Execute_WhenContactsReturnsContacts_ReturnsCommandResult()
        {
            var contactList = new ContactList { Contacts = new List<Contact> { new Contact { FirstName = "User" } } };
            _mockContacts.Setup(m => m.Get("42")).Returns(Observable.Return(contactList));

            var observer = _testScheduler.Start(() => _subject.Execute());

            observer.Messages.First().Value.Kind.Should().Be(NotificationKind.OnNext);
            observer.Messages.First().Value.Value.Should().Be(contactList);
        }

        [Test]
        public void Execute_WhenContactsGetFails_CallsSync()
        {
            var getObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<ContactList>>(
                        100,
                        Notification.CreateOnError<ContactList>(new Exception())));
            _mockContacts.Setup(m => m.Get("42")).Returns(getObservable);

            _testScheduler.Start(() => _subject.Execute());

            _mockSyncs.Verify(m => m.Post(It.Is<Sync>(s => s.Identifier == "42")));
        }

        [Test]
        public void Execute_WhenContactGetFailsSyncSucceedsAndHandlerReturnsValue_ReturnsObservable()
        {
            var contactList = new ContactList { Contacts = CreateContacts() };
            var failingGetObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<ContactList>>(
                        100,
                        Notification.CreateOnError<ContactList>(new Exception())));
            var getObservables = new Queue<IObservable<ContactList>>();
            getObservables.Enqueue(failingGetObservable);
            getObservables.Enqueue(Observable.Return(contactList));
            _mockContacts.Setup(m => m.Get("42")).Returns(getObservables.Dequeue);
            MockSyncResource();
            MockContactsHandler();

            var observer = _testScheduler.Start(() => _subject.Execute());

            observer.Messages.First().Value.Kind.Should().Be(NotificationKind.OnNext);
            observer.Messages.First().Value.Value.Should().Be(contactList);
            failingGetObservable.Subscriptions.Count.Should().Be(1);
        }

        private void MockSyncResource()
        {
            var mockSyncObservable = Observable.Return(new EmptyModel(), SchedulerProvider.Default);
            _mockSyncs.Setup(m => m.Post(It.IsAny<Sync>())).Returns(mockSyncObservable);
        }

        private void MockContactsHandler()
        {
            var mockUnitObservable = Observable.Return(new Unit(), SchedulerProvider.Default);
            _mockContactsHandler.Setup(m => m.Subscribe(It.IsAny<IObserver<Unit>>()))
                .Returns<IObserver<Unit>>(mockUnitObservable.Subscribe);
        }

        private static List<Contact> CreateContacts()
        {
            //generate a contact list with a random contact name
            return new List<Contact> { new Contact { FirstName = Path.GetRandomFileName() } };
        }

        private IObservable<TResult> CreateFailObservable<TResult>()
        {
            return _testScheduler.CreateColdObservable(
                    new Recorded<Notification<TResult>>(
                        100,
                        Notification.CreateOnError<TResult>(new Exception())));
        }

        private IObservable<TResult> CreateSuccessObservable<TResult>(TResult content)
        {
            return
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<TResult>>(100, Notification.CreateOnNext(content)),
                    new Recorded<Notification<TResult>>(200, Notification.CreateOnCompleted<TResult>()));
        }
    }
}
