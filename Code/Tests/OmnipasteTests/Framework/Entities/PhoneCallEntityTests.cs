﻿namespace OmnipasteTests.Framework.Entities
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Entities;
    using PhoneCalls.Dto;

    [TestFixture]
    public class PhoneCallEntityTests
    {
        [TearDown]
        public void TearDown()
        {
            TimeHelper.Reset();
        }

        [Test]
        public void CtorWithCall_AlwaysAssignsAUniqueId()
        {
            new TestPhoneCallEntity(new PhoneCallDto()).UniqueId.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void CtorWithCall_AlwaysCopiesId()
        {
            const string Id = "42";
            new TestPhoneCallEntity(new PhoneCallDto { Id = Id }).Id.Should().Be(Id);
        }

        [Test]
        public void CtorWithCall_Always_AssignsTime()
        {
            var dateTime = new DateTime(2014, 1, 1);
            TimeHelper.UtcNow = dateTime;
            new TestPhoneCallEntity(new PhoneCallDto()).Time.Should().Be(dateTime.ToUniversalTime());
        }

        #region Nested type: TestPhoneCallEntity

        private class TestPhoneCallEntity : PhoneCallEntity
        {
            public TestPhoneCallEntity(PhoneCallDto phoneCallDto)
                : base(phoneCallDto)
            {
            }

            public override SourceType Source
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }

        #endregion
    }
}