using System;
using NUnit.Framework;
using Omniclipboard.ExtensionMethods;

namespace OmniclipboardTests.ExtensionMethods
{
    [TestFixture]
    public class UriExtensionMethodsTests
    {
        [Test]
        public void GetQueryStringParameters_UriHasQueryParameters_ReturnsACollectionOfTheParametersAndTheirValues()
        {
            const string queryString = "http://www.test.com/testpage.application?token=value";
            var uri = new Uri(queryString);
            var result = uri.GetQueryStringParameters();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("value", result["token"]);
        }
    }
}