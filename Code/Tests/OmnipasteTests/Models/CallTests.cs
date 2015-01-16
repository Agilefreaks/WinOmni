namespace OmnipasteTests.Models
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Models;
    using PhoneCalls.Models;

    [TestFixture]
    public class CallTests
    {
        [Test]
        public void CtorWithClipping_AlwaysAssignsAUniqueId()
        {
            new Call(new PhoneCall()).UniqueId.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void CtorWithClipping_AlwaysCopiesId()
        {
            const string Id = "42";
            new Call(new PhoneCall { Id = Id }).Id.Should().Be(Id);
        }
    }
}
