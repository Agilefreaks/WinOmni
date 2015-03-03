namespace OmnipasteTests.Models
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using PhoneCalls.Models;

    [TestFixture]
    public class PhoneCallTests
    {
        [TearDown]
        public void TearDown()
        {
            TimeHelper.Reset();
        }

        [Test]
        public void CtorWithCall_AlwaysAssignsAUniqueId()
        {
            new PhoneCall(new PhoneCallDto()).UniqueId.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void CtorWithCall_AlwaysCopiesId()
        {
            const string Id = "42";
            new PhoneCall(new PhoneCallDto { Id = Id }).Id.Should().Be(Id);
        }

        [Test]
        public void CtorWithCall_Always_AssignsTime()
        {
            var dateTime = new DateTime(2014, 1,1);
            TimeHelper.UtcNow = dateTime;
            new PhoneCall(new PhoneCallDto()).Time.Should().Be(dateTime);
        }

        [Test]
        public void CtorWithCall_Always_SetsSourceToRemote()
        {
            new PhoneCall(new PhoneCallDto()).Source.Should().Be(SourceType.Remote);
        }
    }
}
