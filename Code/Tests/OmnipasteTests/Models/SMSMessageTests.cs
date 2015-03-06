﻿namespace OmnipasteTests.Models
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using OmnipasteTests.Helpers;
    using SMS.Models;

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
            new TestSmsMessage(new SmsMessageDto()).UniqueId.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void CtorWithMessage_AlwaysCopiesId()
        {
            const string Id = "42";
            new TestSmsMessage(new SmsMessageDto { Id = Id }).Id.Should().Be(Id);
        }

        [Test]
        public void CtorWithMessage_Always_AssignsTime()
        {
            var dateTime = new DateTime(2014, 1, 1);
            TimeHelper.UtcNow = dateTime;
            new TestSmsMessage(new SmsMessageDto()).Time.Should().Be(dateTime);
        }

        [Test]
        public void CtorWithClipping_Always_AssignsTime()
        {
            var dateTime = new DateTime(2014, 1, 1);
            TimeHelper.UtcNow = dateTime;
            new TestSmsMessage(new SmsMessageDto()).Time.Should().Be(dateTime);
        }
    }
}