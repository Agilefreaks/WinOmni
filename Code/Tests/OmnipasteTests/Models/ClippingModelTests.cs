namespace OmnipasteTests.Models
{
    using System;
    using Clipboard.Dto;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Entities;
    using Omnipaste.Models;

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
            new ClippingEntity(new ClippingDto()).UniqueId.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void CtorWithClipping_AlwaysCopiesId()
        {
            const string Id = "42";
            new ClippingEntity(new ClippingDto { Id = Id }).Id.Should().Be(Id);
        }

        [Test]
        public void IsLink_WhenClippingTypeIsUrl_ReturnsTrue()
        {
            var subject = new ClippingEntity(new ClippingDto { Type = ClippingDto.ClippingTypeEnum.Url });

            subject.IsLink.Should().BeTrue();
        }
    }
}
