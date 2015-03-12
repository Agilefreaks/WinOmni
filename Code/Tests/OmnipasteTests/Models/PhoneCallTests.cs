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
        private class TestPhoneCall : PhoneCall
        {
            public TestPhoneCall(PhoneCallDto phoneCallDto)
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
            new TestPhoneCall(new PhoneCallDto()).UniqueId.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void CtorWithCall_AlwaysCopiesId()
        {
            const string Id = "42";
            new TestPhoneCall(new PhoneCallDto { Id = Id }).Id.Should().Be(Id);
        }

        [Test]
        public void CtorWithCall_Always_AssignsTime()
        {
            var dateTime = new DateTime(2014, 1,1);
            TimeHelper.UtcNow = dateTime;
            new TestPhoneCall(new PhoneCallDto()).Time.Should().Be(dateTime.ToUniversalTime());
        }
    }
}
