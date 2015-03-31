namespace OmnipasteTests.Models
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using SMS.Models;

    [TestFixture]
    public class RemoteSmsMessageTests
    {
        [Test]
        public void Source_Always_ReturnsRemote()
        {
            new RemoteSmsMessageEntity(new SmsMessageDto()).Source.Should().Be(SourceType.Remote);
        }
    }
}
