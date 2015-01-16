namespace OmnipasteTests.Models
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using SMS.Models;

    [TestFixture]
    public class MessageTests
    {
        [TearDown]
        public void TearDown()
        {
            TimeHelper.Reset();
        }

        [Test]
        public void CtorWithMessage_AlwaysAssignsAUniqueId()
        {
            new Message(new SmsMessage()).UniqueId.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void CtorWithMessage_AlwaysCopiesId()
        {
            const string Id = "42";
            new Message(new SmsMessage { Id = Id }).Id.Should().Be(Id);
        }

        [Test]
        public void CtorWithMessage_Always_AssignsTime()
        {
            var dateTime = new DateTime(2014, 1, 1);
            TimeHelper.UtcNow = dateTime;
            new Message(new SmsMessage()).Time.Should().Be(dateTime);
        }

        [Test]
        public void CtorWithMessage_Always_SetsSourceToRemote()
        {
            new Message(new SmsMessage()).Source.Should().Be(SourceType.Remote);
        }
    }
}
