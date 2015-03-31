namespace OmniApiTests.Models
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmniApi.Dto;

    [TestFixture]
    public class UserTests
    {
        private UserDto _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new UserDto();
        }

        [Test]
        public void Email_AfterConstructor_IsEmptyString()
        {
            _subject.Email.Should().Be(string.Empty);
        }
    }
}