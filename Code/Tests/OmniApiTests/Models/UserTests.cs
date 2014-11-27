namespace OmniApiTests.Models
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmniApi.Models;

    [TestFixture]
    public class UserTests
    {
        private User _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new User();
        }

        [Test]
        public void Email_AfterConstructor_IsEmptyString()
        {
            _subject.Email.Should().Be(string.Empty);
        }
    }
}