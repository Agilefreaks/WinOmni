namespace OmnipasteTests.Models
{
    using System;
    using Clipboard.Models;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using SMS.Models;

    [TestFixture]
    public class ClippingModelTests
    {
        [TearDown]
        public void TearDown()
        {
            TimeHelper.Reset();
        }

        [Test]
        public void CtorWithClipping_AlwaysAssignsAUniqueId()
        {
            new ClippingModel(new Clipping()).UniqueId.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void CtorWithClipping_AlwaysCopiesId()
        {
            const string Id = "42";
            new ClippingModel(new Clipping { Id = Id }).Id.Should().Be(Id);
        }

        [Test]
        public void IsLink_WhenClippingTypeIsUrl_ReturnsTrue()
        {
            var subject = new ClippingModel(new Clipping { Type = Clipping.ClippingTypeEnum.Url });

            subject.IsLink.Should().BeTrue();
        }

        [Test]
        public void CtorWithClipping_Always_AssignsTime()
        {
            var dateTime = new DateTime(2014, 1, 1);
            TimeHelper.UtcNow = dateTime;
            new SmsMessage(new SmsMessageDto()).Time.Should().Be(dateTime);
        }

        [Test]
        public void CtorWithCall_Always_SetsSourceToRemote()
        {
            new SmsMessage(new SmsMessageDto()).Source.Should().Be(SourceType.Remote);
        }
    }
}
