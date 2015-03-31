namespace OmnipasteTests.Models
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using OmnipasteTests.Helpers;
    using SMS.Dto;

    [TestFixture]
    public class SmsMessageTests
    {
        [TearDown]
        public void TearDown()
        {
            TimeHelper.Reset();
        }

        [Test]
        public void CtorWithMessage_AlwaysAssignsAUniqueId()
        {
            new TestSmsMessageEntity(new SmsMessageDto()).UniqueId.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void CtorWithMessage_AlwaysCopiesId()
        {
            const string Id = "42";
            new TestSmsMessageEntity(new SmsMessageDto { Id = Id }).Id.Should().Be(Id);
        }

        [Test]
        public void CtorWithSmsMessage_Always_AssignsTime()
        {
            var dateTime = new DateTime(2014, 1, 1);
            TimeHelper.UtcNow = dateTime;
            new TestSmsMessageEntity(new SmsMessageDto()).Time.Should().Be(dateTime.ToUniversalTime());
        }
    }
}