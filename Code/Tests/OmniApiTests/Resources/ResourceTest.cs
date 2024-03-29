﻿namespace OmniApiTests.Resources
{
    using System;
    using System.Net.Http;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Dto;
    using OmniApi.Resources;
    using OmniApi.Resources.v1;
    using OmniApi.Support;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using Refit;

    [TestFixture]
    public class ResourceTest
    {
        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<IOAuth2> _mockOAuth2;

        private Mock<IWebProxyFactory> _mockWebProxyFactory;

        private TestResource _subject;

        [SetUp]
        public void SetUp()
        {
            _mockConfigurationService = new Mock<IConfigurationService> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService.SetupGet(m => m.AccessToken).Returns("AccessToken");
            _mockConfigurationService.SetupGet(m => m.RefreshToken).Returns("RefreshToken");
            _mockConfigurationService.SetupGet(m => m[ConfigurationProperties.BaseUrl]).Returns("http://test.com");

            _mockWebProxyFactory = new Mock<IWebProxyFactory>();

            _mockOAuth2 = new Mock<IOAuth2>();
            _subject = new TestResource(_mockConfigurationService.Object, _mockWebProxyFactory.Object)
                           {
                               OAuth2 =
                                   _mockOAuth2
                                   .Object
                           };
        }

        [Test]
        public void Token_Always_GetsReadFromConfigurationService()
        {
            _subject.TokenDto.ShouldBeEquivalentTo(new TokenDto("AccessToken", "RefreshToken"));
        }

        [Test]
        public void AccessToken_Always_PrefixesWithBearer()
        {
            _subject.AccessToken.Should().Be("bearer AccessToken");
        }

        [Test]
        public void Constructor_WhenOAuth2IsNull_DoesNotThrow()
        {
            _subject = new TestResource(_mockConfigurationService.Object, _mockWebProxyFactory.Object);
        }

        [Test]
        public void GetRestApi_WhenOAuth2IsNull_ThrowsArgumentNullException()
        {
            _subject = new TestResource(_mockConfigurationService.Object, _mockWebProxyFactory.Object);
            Action action = () => { var tmp = _subject.ResourceApi; };

            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void GetRestApi_WhenOAuth2IsNotNullAndHandlerIsNull_ThrowsArgumentNullException()
        {
            _subject = new TestResource(_mockConfigurationService.Object, _mockWebProxyFactory.Object)
                           {
                               OAuth2 =
                                   _mockOAuth2
                                   .Object
                           };
            Action action = () => { var tmp = _subject.ResourceApi; };

            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void GetRestApi_WhenOAuth2IsNotNullAndHandlerIsNotNull_ThrowsArgumentNullException()
        {
            _subject = new TestResource(_mockConfigurationService.Object, _mockWebProxyFactory.Object)
                           {
                               OAuth2 =
                                   _mockOAuth2
                                   .Object,
                               ResponseHandler
                                   =
                                   new Mock
                                   <
                                   IHttpResponseMessageHandler
                                   >()
                                   .Object
                           };
            Action action = () => { var tmp = _subject.ResourceApi; };

            action.ShouldNotThrow();
        }

        #region Nested type: TestResource

        public class TestResource : ResourceWithAuthorization<ITestApi>
        {
            public TestResource(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
                : base(configurationService, webProxyFactory)
            {
            }

            protected override ITestApi CreateResourceApi(HttpClient httpClient)
            {
                return RestService.For<ITestApi>(httpClient);
            }
        }

        #endregion
    }
}