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
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using Omnipaste.Services.Commands;

    [TestFixture]
    public class SyncContactsCommandTests
    {
        private SyncContactsCommand _subject;

        private TestScheduler _testScheduler;

        private Mock<IContacts> _mockContacts;

        private Mock<IContactsHandler> _mockContactsHandler;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<ISyncs> _mockSyncs;

        private DeviceInfo _deviceInfo;

        private List<DeviceInfo> _deviceInfos;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            _mockContacts = new Mock<IContacts> { DefaultValue = DefaultValue.Mock };
            _mockSyncs = new Mock<ISyncs> { DefaultValue = DefaultValue.Mock };
            _mockContactsHandler = new Mock<IContactsHandler> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService> { DefaultValue = DefaultValue.Mock };

            _deviceInfo = new DeviceInfo { Identifier = "42" };
            _deviceInfos = new List<DeviceInfo> { _deviceInfo };
            _mockConfigurationService.Setup(m => m.DeviceInfos).Returns(_deviceInfos);
            
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
        public void Process_WhenContactsReturnsContacts_ReturnsCommandResult()
        {
            var contactList = new ContactList { Contacts = new List<Contact> { new Contact { ContactName = "User" } } };
            _mockContacts.Setup(m => m.Get(_deviceInfo.Identifier)).Returns(Observable.Return(contactList));

            var observer = _testScheduler.Start(() => _subject.Execute());

            observer.Messages.First().Value.Kind.Should().Be(NotificationKind.OnNext);
            observer.Messages.First().Value.Value.First().Should().Be(contactList);
        }

        [Test]
        public void Process_WhenContactsGetFails_CallsSync()
        {
            var getObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<ContactList>>(
                        100,
                        Notification.CreateOnError<ContactList>(new Exception())));
            _mockContacts.Setup(m => m.Get(_deviceInfo.Identifier)).Returns(getObservable);

            _testScheduler.Start(() => _subject.Execute());

            _mockSyncs.Verify(m => m.Post(It.Is<Sync>(s => s.Identifier == _deviceInfo.Identifier)));
        }

        [Test]
        public void Process_WhenContactGetFailsSyncSucceedsAndHandlerReturnsValue_ReturnsObservable()
        {
            var contactList = new ContactList { Contacts = new List<Contact> { new Contact { ContactName = "Contact" } } };
            var failingGetObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<ContactList>>(
                        100,
                        Notification.CreateOnError<ContactList>(new Exception())));
            var getObservables = new Queue<IObservable<ContactList>>();
            getObservables.Enqueue(failingGetObservable);
            getObservables.Enqueue(Observable.Return(contactList));
            _mockContacts.Setup(m => m.Get(_deviceInfo.Identifier)).Returns(getObservables.Dequeue);
            _mockSyncs.Setup(m => m.Post(It.Is<Sync>(s => s.Identifier == _deviceInfo.Identifier)))
                .Returns(Observable.Return(new EmptyModel()));
            _mockContactsHandler.Setup(m => m.Subscribe(It.IsAny<IObserver<Unit>>())).Callback<IObserver<Unit>>(o => o.OnNext(Unit.Default));

            var observer = _testScheduler.Start(() => _subject.Execute());

            observer.Messages.First().Value.Kind.Should().Be(NotificationKind.OnNext);
            observer.Messages.First().Value.Value.First().Should().Be(contactList);
            failingGetObservable.Subscriptions.Count.Should().Be(1);
        }

        [Test]
        public void Process_WhenThereAreMoreDevices_AggregatesResultFromEach()
        {
            _deviceInfos.Add(new DeviceInfo { Identifier = "other" });

            var contactList1 = new ContactList
                                   {
                                       Contacts =
                                           new List<Contact> { new Contact { ContactName = "Contact1" } }
                                   };
            var contactList2 = new ContactList
                                   {
                                       Contacts =
                                           new List<Contact> { new Contact { ContactName = "Contact2" } }
                                   };
            var observables =
                new Queue<IObservable<ContactList>>(
                    new[]
                        {
                            CreateFailObservable<ContactList>(), CreateSuccessObservable(contactList1),
                            CreateFailObservable<ContactList>(), CreateSuccessObservable(contactList2)
                        });
            _mockContacts.Setup(m => m.Get(It.IsAny<string>())).Returns(observables.Dequeue);
            _mockSyncs.Setup(m => m.Post(It.IsAny<Sync>()))
                .Returns(Observable.Return(new EmptyModel(), SchedulerProvider.Default));
            var unitObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<Unit>>(100, Notification.CreateOnNext(new Unit())));
            _mockContactsHandler.Setup(m => m.Subscribe(It.IsAny<IObserver<Unit>>()))
                .Returns<IObserver<Unit>>(o => unitObservable.Subscribe(o));

            var observer = _testScheduler.Start(() => _subject.Execute());

            observer.Messages[0].Value.Value[0].Should().Be(contactList1);
            observer.Messages[0].Value.Value[1].Should().Be(contactList2);
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
