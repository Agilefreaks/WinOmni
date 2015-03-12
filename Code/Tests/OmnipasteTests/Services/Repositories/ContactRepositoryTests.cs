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
        public void GetByPhoneNumber_Always_ReturnsContact()
        {
            var contact1 = new ContactInfo
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
            var contact2 = new ContactInfo
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
            var contact1 = new ContactInfo
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
            var contact2 = new ContactInfo
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
        public void GetOrCreateByPhoneNumber_WhenTheContactExists_ReturnsTheContact()
        {
            var contact1 = new ContactInfo
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

            var observable = _subject.Save(contact1)
                .Select(_ => _subject.GetOrCreateByPhoneNumber(PhoneNumber))
                .Switch();

            var result = _testScheduler.Start(() => observable);

            result.Messages.First().Value.Value.UniqueId.Should().Be(contact1.UniqueId);
        }

        [Test]
        public void GetOrCreateByPhoneNumber_WhenThereIsNoContactStored_WillSaveTheContact()
        {
            var result = _testScheduler.Start(() => _subject.GetOrCreateByPhoneNumber(PhoneNumber).Select(_ => _subject.GetByPhoneNumber(PhoneNumber)).Switch());
            
            result.Messages.First().Value.Value.PhoneNumber.Should().Be(PhoneNumber);
        }

        [Test]
        public void Save_WillNotExpire()
        {
            var testableObserver = _testScheduler.CreateObserver<ContactInfo>();
            _testScheduler.Schedule(() => _subject.Save(new ContactInfo { UniqueId = "42" }));
            _testScheduler.Schedule(new TimeSpan(0, 23, 59, 0), () => _subject.Get("42").Subscribe(testableObserver));
            _testScheduler.Schedule(new TimeSpan(1, 0, 0, 1), () => _subject.Get("42").Subscribe(testableObserver));

            _testScheduler.Start();

            testableObserver.Messages.First().Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages.Last().Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }
    }
}
