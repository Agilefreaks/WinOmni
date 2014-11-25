namespace OmniApiTests.Models
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmniApi.Models;

    [TestFixture]
    public class UserInfoTests
    {
        private UserInfo _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new UserInfo();
        }

        [Test]
        public void Email_AfterConstructor_IsEmptyString()
        {
            _subject.Email.Should().Be(string.Empty);
        }
    }
}