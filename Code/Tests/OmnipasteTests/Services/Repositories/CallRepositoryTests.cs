namespace OmnipasteTests.Services.Repositories
{
    using System.Linq;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class CallRepositoryTests
    {
        private PhoneCallRepository _subject;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;

            _subject = new PhoneCallRepository();
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void GetByContact_Always_ReturnsCallsForContact()
        {
            const string PhoneNumber = "0722123123";
            var phoneNumbers = new[] { new PhoneNumber { Number = PhoneNumber } };
            var contactInfo = new ContactInfo { PhoneNumbers = phoneNumbers };
            var call1 = new LocalPhoneCall
                            {
                                UniqueId = "42",
                                ContactInfo = contactInfo
                                    
                            };
            var call2 = new LocalPhoneCall
                            {
                                UniqueId = "1",
                                ContactInfo =
                                    new ContactInfo
                                        {
                                            PhoneNumbers =
                                                new[]
                                                    {
                                                        new PhoneNumber
                                                            {
                                                                Number =
                                                                    "5987120439217094"
                                                            }
                                                    }
                                        }
                            };
            var observable = _subject.Save(call1)
                .Select(_ => _subject.Save(call2))
                .Select(_ => _subject.GetByContact(contactInfo))
                .Switch();

            var result = _testScheduler.Start(() => observable);

            result.Messages.First().Value.Value.Count().Should().Be(1);
            result.Messages.First().Value.Value.First().Should().Be(call1);
        }

        [Test]
        public void GetByContact_WhenCallPhoneNumberContainsPrefix_ReturnsCallForContact()
        {
            const string PhoneNumber = "0722123123";
            var phoneNumbers = new[] { new PhoneNumber { Number = "+40" + PhoneNumber } };
            var contactInfo = new ContactInfo { PhoneNumbers = phoneNumbers };
            var call1 = new LocalPhoneCall { UniqueId = "42", ContactInfo = contactInfo };
            var call2 = new LocalPhoneCall
                            {
                                UniqueId = "1",
                                ContactInfo =
                                    new ContactInfo
                                        {
                                            PhoneNumbers =
                                                new[]
                                                    {
                                                        new PhoneNumber
                                                            {
                                                                Number =
                                                                    "5987120439217094"
                                                            }
                                                    }
                                        }
                            };
            var observable = _subject.Save(call1)
                .Select(_ => _subject.SaveSynchronous(call2))
                .Select(_ => _subject.GetByContact(contactInfo))
                .Switch();

            var result = _testScheduler.Start(() => observable);

            result.Messages.First().Value.Value.Count().Should().Be(1);
            result.Messages.First().Value.Value.First().Should().Be(call1);
        }
    }
}
