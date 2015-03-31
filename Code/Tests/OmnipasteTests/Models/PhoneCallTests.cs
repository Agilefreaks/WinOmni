namespace OmnipasteTests.Models
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using PhoneCalls.Dto;

    [TestFixture]
    public class PhoneCallTests
    {
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
            var dateTime = new DateTime(2014, 1,1);
            TimeHelper.UtcNow = dateTime;
            new TestPhoneCallEntity(new PhoneCallDto()).Time.Should().Be(dateTime.ToUniversalTime());
        }
    }
}
