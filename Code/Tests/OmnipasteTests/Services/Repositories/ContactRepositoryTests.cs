namespace OmnipasteTests.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class ContactRepositoryTests
    {
        private const string PhoneNumber = "0722123123";

        private ContactRepository _subject;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;

            _subject = new ContactRepository();
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void GetByContactIdOrPhoneNumber_WhenThereIsAContactWithContactId_ReturnsIt()
        {
            var contact = new ContactEntity { ContactId = 42 };
            var observable = _subject.Save(contact).Select(_ => _subject.GetByContactIdOrPhoneNumber(42, String.Empty)).Switch();

            var result = _testScheduler.Start(() => observable);

            var contactEntity = result.Messages.First().Value.Value;
            contactEntity.ContactId.Should().Be(42);
        }

        [Test]
        public void GetByContactIdOrPhoneNumber_WhenThereIsAContactWithPhoneNumber_ReturnsIt()
        {
            var contact = new ContactEntity().AddPhoneNumber("123");
            var observable = _subject.Save(contact).Select(_ => _subject.GetByContactIdOrPhoneNumber(null, "123")).Switch();

            var result = _testScheduler.Start(() => observable);

            var contactEntity = result.Messages.First().Value.Value;
            contactEntity.PhoneNumber.Should().Be("123");
        }

        [Test]
        public void CreateIfNone_WhenTheObservableReturnsAException_CreatesANewContact()
        {
            var observable = _subject.CreateIfNone(_subject.GetByContactIdOrPhoneNumber(42, "123"), c => c.AddPhoneNumber("123").SetContactId(42)).Select(_ => _subject.GetAll()).Switch();

            var result = _testScheduler.Start(() => observable);

            var contactEntity = result.Messages.First().Value.Value.First();
            contactEntity.PhoneNumber.Should().Be("123");
            contactEntity.ContactId.Should().Be(42);
        }

        [Test]
        public void GetByPhoneNumber_WhenContactExists_ReturnsContact()
        {
            var contact1 = new ContactEntity
            {
                UniqueId = "42",
                PhoneNumbers =
                    new List<PhoneNumber>
                                           {
                                               new PhoneNumber
                                                   {
                                                       Number = "0722123123",
                                                       Type = "Home"
                                                   }
                                           }
            };
            var contact2 = new ContactEntity
            {
                UniqueId = "1",
                PhoneNumbers =
                    new List<PhoneNumber>
                                           {
                                               new PhoneNumber
                                                   {
                                                       Number = "987800879237",
                                                       Type = "Home"
                                                   }
                                           }
            };
            var observable = _subject.Save(contact1)
                .Select(_ => _subject.Save(contact2))
                .Select(_ => _subject.GetByPhoneNumber(PhoneNumber))
                .Switch();

            var result = _testScheduler.Start(() => observable);

            result.Messages.First().Value.Value.UniqueId.Should().Be(contact1.UniqueId);
        }

        [Test]
        public void GetByPhoneNumber_WhenCallPhoneNumberContainsPrefix_ReturnsCallForContact()
        {
            var contact1 = new ContactEntity
                               {
                                   UniqueId = "42",
                                   PhoneNumbers =
                                       new List<PhoneNumber>
                                           {
                                               new PhoneNumber
                                                   {
                                                       Number = "+40722123123",
                                                       Type = "Home"
                                                   }
                                           }
                               };
            var contact2 = new ContactEntity
                               {
                                   UniqueId = "1",
                                   PhoneNumbers =
                                       new List<PhoneNumber>
                                           {
                                               new PhoneNumber
                                                   {
                                                       Number = "987800879237",
                                                       Type = "Home"
                                                   }
                                           }
                               };
            var observable = _subject.Save(contact1)
                .Select(_ => _subject.Save(contact2))
                .Select(_ => _subject.GetByPhoneNumber(PhoneNumber))
                .Switch();

            var result = _testScheduler.Start(() => observable);

            result.Messages.First().Value.Value.UniqueId.Should().Be(contact1.UniqueId);
        }

        [Test]
        public void Save_WillNotExpire()
        {
            var testableObserver = _testScheduler.CreateObserver<ContactEntity>();
            _testScheduler.Schedule(() => _subject.Save(new ContactEntity { UniqueId = "42" }));
            _testScheduler.Schedule(new TimeSpan(0, 23, 59, 0), () => _subject.Get("42").Subscribe(testableObserver));
            _testScheduler.Schedule(new TimeSpan(1, 0, 0, 1), () => _subject.Get("42").Subscribe(testableObserver));

            _testScheduler.Start();

            testableObserver.Messages.First().Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages.Last().Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }
    }
}
