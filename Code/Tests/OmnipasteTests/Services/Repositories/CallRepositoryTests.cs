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
        private CallRepository _subject;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;

            _subject = new CallRepository();
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void GetByContact_Always_ReturnsCallsForContact()
        {
            var phoneNumber = "0722123123";
            var call1 = new Call { UniqueId = "42", ContactInfo = new ContactInfo { Phone = phoneNumber } };
            var call2 = new Call { UniqueId = "1", ContactInfo = new ContactInfo { Phone = "5987120439217094" } };
            var observable = _subject.Save(call1)
                .Select(_ => _subject.Save(call2))
                .Select(_ => _subject.GetByContact(new ContactInfo { Phone = phoneNumber }))
                .Switch();

            var result = _testScheduler.Start(() => observable);

            result.Messages.First().Value.Value.Count().Should().Be(1);
            result.Messages.First().Value.Value.First().Should().Be(call1);
        }

        [Test]
        public void GetByContact_WhenCallPhoneNumberContainsPrefix_ReturnsCallForContact()
        {
            var phoneNumber = "0722123123";
            var call1 = new Call { UniqueId = "42", ContactInfo = new ContactInfo { Phone = "+40" + phoneNumber } };
            var call2 = new Call { UniqueId = "1", ContactInfo = new ContactInfo { Phone = "5987120439217094" } };
            var observable = _subject.Save(call1)
                .Select(_ => _subject.SaveSynchronous(call2))
                .Select(_ => _subject.GetByContact(new ContactInfo { Phone = phoneNumber }))
                .Switch();

            var result = _testScheduler.Start(() => observable);

            result.Messages.First().Value.Value.Count().Should().Be(1);
            result.Messages.First().Value.Value.First().Should().Be(call1);
        }
    }
}
