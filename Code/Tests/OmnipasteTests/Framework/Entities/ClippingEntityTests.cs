namespace OmnipasteTests.Framework.Entities
{
    using Clipboard.Dto;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Entities;

    [TestFixture]
    public class ClippingEntityTests
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
