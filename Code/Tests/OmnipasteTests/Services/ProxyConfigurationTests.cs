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

        [Test]
        public void Equals_BothInstancesAreEmpty_ReturnsTrue()
        {
            var proxyConfiguration1 = new ProxyConfiguration();
            var proxyConfiguration2 = new ProxyConfiguration();

            Equals(proxyConfiguration1, proxyConfiguration2).Should().BeTrue();
        }

        [Test]
        public void Equals_BothInstancesHaveTheSameFields_ReturnsTrue()
        {
            var proxyConfiguration1 = new ProxyConfiguration
                                          {
                                              Address = "test1",
                                              Password = "test2",
                                              Port = 3,
                                              Type = ProxyTypeEnum.None,
                                              Username = "test3"
                                          };
            var proxyConfiguration2 = new ProxyConfiguration
                                          {
                                              Address = "test1",
                                              Password = "test2",
                                              Port = 3,
                                              Type = ProxyTypeEnum.None,
                                              Username = "test3"
                                          };

            Equals(proxyConfiguration1, proxyConfiguration2).Should().BeTrue();
        }

        [Test]
        public void Equals_InstancesHaveDifferentFields_ReturnsFalse()
        {
            var proxyConfiguration1 = new ProxyConfiguration
                                          {
                                              Address = "test2",
                                              Password = "test1",
                                              Port = 3,
                                              Type = ProxyTypeEnum.None,
                                              Username = "test3"
                                          };
            var proxyConfiguration2 = new ProxyConfiguration
                                          {
                                              Address = "test1",
                                              Password = "test2",
                                              Port = 4,
                                              Type = ProxyTypeEnum.None,
                                              Username = "test"
                                          };

            Equals(proxyConfiguration1, proxyConfiguration2).Should().BeFalse();
        }
    }
}