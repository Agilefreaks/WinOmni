namespace OmnipasteTests.Models
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Models;
    using SMS.Models;

    [TestFixture]
    public class RemoteSmsMessageTests
    {
        [Test]
        public void Source_Always_ReturnsRemote()
        {
            new RemoteSmsMessage(new SmsMessageDto()).Source.Should().Be(SourceType.Remote);
        }
    }
}
