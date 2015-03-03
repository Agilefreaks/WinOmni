namespace OmnipasteTests.Services.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
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
            var phoneNumber = "0722123123";
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
                .Select(_ => _subject.GetByPhoneNumber(phoneNumber))
                .Switch();

            var result = _testScheduler.Start(() => observable);

            result.Messages.First().Value.Value.Should().Be(contact1);
        }

        [Test]
        public void GetByPhoneNumber_WhenCallPhoneNumberContainsPrefix_ReturnsCallForContact()
        {
            var phoneNumber = "0722123123";
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
                .Select(_ => _subject.GetByPhoneNumber(phoneNumber))
                .Switch();

            var result = _testScheduler.Start(() => observable);

            result.Messages.First().Value.Value.Should().Be(contact1);
        }

        [Test]
        public void GetOrCreateByPhoneNumber_WhenTheContactExists_ReturnsTheContact()
        {
            var phoneNumber = "0722123123";

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
                .Select(_ => _subject.GetOrCreateByPhoneNumber(phoneNumber))
                .Switch();

            var result = _testScheduler.Start(() => observable);

            result.Messages.First().Value.Value.Should().Be(contact1);
        }

        [Test]
        public void GetOrCreateByPhoneNumber_WhenThereIsNoContactStored_WillSaveTheContact()
        {
            var phoneNumber = "0722123123";
            
            var observable = _subject.GetOrCreateByPhoneNumber(phoneNumber);
            _testScheduler.Start(() => observable);
            
            var testObservable = _subject.GetByPhoneNumber(phoneNumber);
            var result = _testScheduler.Start(() => testObservable);
            result.Messages.First().Value.Value.PhoneNumber.Should().Be(phoneNumber);
        }
    }
}
