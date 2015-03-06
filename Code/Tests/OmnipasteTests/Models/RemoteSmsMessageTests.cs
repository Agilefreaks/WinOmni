namespace OmnipasteTests.Models
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Models;
    using SMS.Models;

    [TestFixture]
    public class LocalSmsMessageTests
    {
        [Test]
        public void Source_Always_ReturnsLocal()
        {
            new LocalSmsMessage(new SmsMessageDto()).Source.Should().Be(SourceType.Local);
        }
    }
}
