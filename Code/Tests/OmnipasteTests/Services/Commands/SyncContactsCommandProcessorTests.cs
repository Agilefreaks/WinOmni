namespace OmnipasteTests.Services.Commands
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
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
    using Omnipaste.Services.Commands;
    using Refit;

    [TestFixture]
    public class SyncContactsCommandProcessorTests
    {
        private SyncContactsCommand _subject;

        private Mock<IContacts> _mockContacts;

        private Mock<IContactsHandler> _mockContactsHandler;

        [SetUp]
        public void SetUp()
        {
            _mockContacts = new Mock<IContacts> { DefaultValue = DefaultValue.Mock };
            _mockContactsHandler = new Mock<IContactsHandler> { DefaultValue = DefaultValue.Mock };
            _subject = new SyncContactsCommand(_mockContacts.Object, _mockContactsHandler.Object);
        }

        [Test]
        public void Process_WhenContactsReturnsContacts_ReturnsCommandResult()
        {
            var testScheduler = new TestScheduler();
            var contactList = new ContactList();
            _mockContacts.Setup(m => m.Get()).Returns(Observable.Return(contactList));

            var observer = testScheduler.Start(() => _subject.Execute(new SyncContactsParams()));

            observer.Messages.First().Value.Kind.Should().Be(NotificationKind.OnNext);
            observer.Messages.First().Value.Value.ContactList.Should().Be(contactList);
        }

        [Test]
        public void Process_WhenContactsGetFails_CallsSync()
        {
            var testScheduler = new TestScheduler();
            var getObservable =
                testScheduler.CreateColdObservable(
                    new Recorded<Notification<ContactList>>(
                        100,
                        Notification.CreateOnError<ContactList>(new Exception())));
            _mockContacts.Setup(m => m.Get()).Returns(getObservable);

            testScheduler.Start(() => _subject.Execute(new SyncContactsParams()));
            
            _mockContacts.Verify(m => m.Sync());
        }

        [Test]
        public void Process_WhenContactGetFailsSyncSucceedsAndHandlerReturnsValue_ReturnsObservable()
        {
            var testScheduler = new TestScheduler();
            var contactList = new ContactList();
            var getObservable =
                testScheduler.CreateColdObservable(
                    new Recorded<Notification<ContactList>>(
                        100,
                        Notification.CreateOnError<ContactList>(new Exception())));
            _mockContacts.Setup(m => m.Get()).Returns(getObservable);
            _mockContacts.Setup(m => m.Sync()).Returns(Observable.Return(new EmptyModel()));
            _mockContactsHandler.Setup(m => m.Subscribe(It.IsAny<IObserver<ContactList>>()))
                .Callback<IObserver<ContactList>>(o => o.OnNext(contactList));

            var observer = testScheduler.Start(() => _subject.Execute(new SyncContactsParams()));

            observer.Messages.First().Value.Kind.Should().Be(NotificationKind.OnNext);
            observer.Messages.First().Value.Value.ContactList.Should().Be(contactList);
        }
    }
}
