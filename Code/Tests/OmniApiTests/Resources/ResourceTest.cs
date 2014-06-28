namespace OmniApiTests.Resources
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources;

    public class ResourceTest
    {
        private TestResource _subject;

        public class TestResource : Resource<TestResource.ITestApi>
        {
            public interface ITestApi
            {
            }
        }

        [SetUp]
        public void SetUp()
        {
            _subject = new TestResource();
        }

        [Test]
        public void AccessToken_Always_PrefixesWithBearer()
        {
            _subject.Token = new Token("access_token", "refresh_token");

            _subject.AccessToken.Should().Be("bearer access_token");
        }

        [Test]
        public void Authorize_Always_WrapsInAAuthorizationObserver()
        {
            _subject.Authorize(Observable.Empty<string>()).Should().BeSameAs(typeof(AnonymousObservable<string>));
        }
    }
}