namespace OmnipasteTests.Entities
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Entities;
    using SMS.Dto;

    [TestFixture]
    public class LocalSmsMessageEntityTests
    {
        [Test]
        public void Source_Always_ReturnsLocal()
        {
            new LocalSmsMessageEntity(new SmsMessageDto()).Source.Should().Be(SourceType.Local);
        }
    }
}
