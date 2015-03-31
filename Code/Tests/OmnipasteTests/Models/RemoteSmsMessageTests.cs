namespace OmnipasteTests.Models
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using SMS.Models;

    [TestFixture]
    public class LocalSmsMessageTests
    {
        [Test]
        public void Source_Always_ReturnsLocal()
        {
            new LocalSmsMessageEntity(new SmsMessageDto()).Source.Should().Be(SourceType.Local);
        }
    }
}
