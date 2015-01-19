namespace OmnipasteTests.Models
{
    using System;
    using System.Security.Cryptography;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using PhoneCalls.Models;

    [TestFixture]
    public class CallTests
    {
        [TearDown]
        public void TearDown()
        {
            TimeHelper.Reset();
        }

        [Test]
        public void CtorWithCall_AlwaysAssignsAUniqueId()
        {
            new Call(new PhoneCall()).UniqueId.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void CtorWithCall_AlwaysCopiesId()
        {
            const string Id = "42";
            new Call(new PhoneCall { Id = Id }).Id.Should().Be(Id);
        }

        [Test]
        public void CtorWithCall_Always_AssignsTime()
        {
            var dateTime = new DateTime(2014, 1,1);
            TimeHelper.UtcNow = dateTime;
            new Call(new PhoneCall()).Time.Should().Be(dateTime);
        }

        [Test]
        public void CtorWithCall_Always_SetsSourceToRemote()
        {
            new Call(new PhoneCall()).Source.Should().Be(SourceType.Remote);
        }
    }
}
