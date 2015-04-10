namespace OmnipasteTests.Framework.Entities
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Framework.Entities;
    using SMS.Dto;

    [TestFixture]
    public class RemoteSmsMessageEntityTests
    {
        [Test]
        public void Source_Always_ReturnsRemote()
        {
            new RemoteSmsMessageEntity(new SmsMessageDto()).Source.Should().Be(SourceType.Remote);
        }
    }
}
