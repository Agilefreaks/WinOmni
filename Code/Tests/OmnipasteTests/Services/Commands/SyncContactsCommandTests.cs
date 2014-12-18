namespace OmnipasteTests.Services.Commands
{
    using System;
    using System.Collections.Generic;
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
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using Omnipaste.Services.Commands;

    [TestFixture]
    public class SyncContactsCommandTests
    {
        private SyncContactsCommand _subject;

        private Mock<IContacts> _mockContacts;

        private Mock<IContactsHandler> _mockContactsHandler;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<ISyncs> _mockSyncs;

        private DeviceInfo _deviceInfo;

        [SetUp]
        public void SetUp()
        {
            _mockContacts = new Mock<IContacts> { DefaultValue = DefaultValue.Mock };
            _mockSyncs = new Mock<ISyncs> { DefaultValue = DefaultValue.Mock };
            _mockContactsHandler = new Mock<IContactsHandler> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService> { DefaultValue = DefaultValue.Mock };

            _deviceInfo = new DeviceInfo { Identifier = "42" };
            _mockConfigurationService.Setup(m => m.DeviceInfos).Returns(new List<DeviceInfo> { _deviceInfo });

            _subject = new SyncContactsCommand(_mockContacts.Object, _mockSyncs.Object, _mockContactsHandler.Object, _mockConfigurationService.Object);
        }

        [Test]
        public void Process_WhenContactsReturnsContacts_ReturnsCommandResult()
        {
            var testScheduler = new TestScheduler();
            var contactList = new ContactList { Contacts = new List<Contact> { new Contact { ContactName = "User" } } };
            _mockContacts.Setup(m => m.GetAll()).Returns(Observable.Return(contactList));

            var observer = testScheduler.Start(() => _subject.Execute());

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
            _mockContacts.Setup(m => m.GetAll()).Returns(getObservable);

            testScheduler.Start(() => _subject.Execute());

            _mockSyncs.Verify(m => m.Post(It.Is<Sync>(s => s.Identifier == _deviceInfo.Identifier)));
        }

        [Test]
        public void Process_WhenContactGetFailsSyncSucceedsAndHandlerReturnsValue_ReturnsObservable()
        {
            var testScheduler = new TestScheduler();
            var contactList = new ContactList { Contacts = new List<Contact> { new Contact { ContactName = "Contact" } } };
            var getObservable =
                testScheduler.CreateColdObservable(
                    new Recorded<Notification<ContactList>>(
                        100,
                        Notification.CreateOnError<ContactList>(new Exception())));
            _mockContacts.Setup(m => m.GetAll()).Returns(getObservable);
            _mockSyncs.Setup(m => m.Post(It.Is<Sync>(s => s.Identifier == _deviceInfo.Identifier))).Returns(Observable.Return(new EmptyModel()));
            _mockContactsHandler.Setup(m => m.Subscribe(It.IsAny<IObserver<ContactList>>()))
                .Callback<IObserver<ContactList>>(o => o.OnNext(contactList));

            var observer = testScheduler.Start(() => _subject.Execute());

            observer.Messages.First().Value.Kind.Should().Be(NotificationKind.OnNext);
            observer.Messages.First().Value.Value.ContactList.Should().Be(contactList);
        }
    }
}
