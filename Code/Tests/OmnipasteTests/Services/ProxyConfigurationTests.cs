namespace OmnipasteTests.Services
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon;

    [TestFixture]
    public class ProxyConfigurationTests
    {
        private ProxyConfiguration _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new ProxyConfiguration();
        }

        [Test]
        public void Ctor_Always_SetsTypeToNone()
        {
            _subject.Type.Should().Be(ProxyTypeEnum.None);
        }

        [Test]
        public void Empty_Always_ReturnsInstanceWithTypeNone()
        {
            ProxyConfiguration.Empty().Type.Should().Be(ProxyTypeEnum.None);
        }

        [Test]
        public void Empty_Always_ReturnsInstanceWithAddressNull()
        {
            ProxyConfiguration.Empty().Address.Should().Be(null);
        }

        [Test]
        public void Empty_Always_ReturnsInstanceWithPort0()
        {
            ProxyConfiguration.Empty().Port.Should().Be(0);
        }

        [Test]
        public void Empty_Always_ReturnsInstanceWithUsernameNull()
        {
            ProxyConfiguration.Empty().Username.Should().Be(null);
        }

        [Test]
        public void Empty_Always_ReturnsInstanceWithPasswordNull()
        {
            ProxyConfiguration.Empty().Username.Should().Be(null);
        }
    }
}