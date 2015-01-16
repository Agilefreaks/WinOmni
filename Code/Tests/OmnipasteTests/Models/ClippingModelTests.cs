namespace OmnipasteTests.Models
{
    using Clipboard.Models;
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Models;

    [TestFixture]
    public class ClippingModelTests
    {
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
    }
}
