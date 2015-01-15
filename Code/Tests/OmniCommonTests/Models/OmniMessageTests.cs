namespace OmniCommonTests.Models
{
    using System.Collections.Generic;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Models;

    [TestFixture]
    public class OmniMessageTests
    {
        [Test]
        public void Ctor_Always_InitializesPayload()
        {
            new OmniMessage().Payload.Should().NotBeNull();
        }

        [Test]
        public void GetPayload_WhenPayloadIsNull_ReturnsNull()
        {
            var subject = new OmniMessage { Payload = null };

            subject.GetPayload("id").Should().BeNull();
        }

        [Test]
        public void GetPayload_WhenPayloadDoesNotContainKey_ReturnsNull()
        {
            var subject = new OmniMessage { Payload = new Dictionary<string, string>() };

            subject.GetPayload("id").Should().BeNull();
        }

        [Test]
        public void GetPayload_WhenPayloadContainsKey_ReturnsValue()
        {
            var subject = new OmniMessage { Payload = new Dictionary<string, string> { { "id", "42" } } };

            subject.GetPayload("id").Should().Be("42");
        }
    }
}
