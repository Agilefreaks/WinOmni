namespace OmniApiTests.Dto
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmniApi.Dto;

    [TestFixture]
    public class UserDtoTests
    {
        private UserDto _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new UserDto();
        }

        [Test]
        public void Ctor_Always_InitsStringFieldsToEmpty()
        {
            _subject.FirstName.Should().Be(string.Empty);
            _subject.LastName.Should().Be(string.Empty);
            _subject.Email.Should().Be(string.Empty);
            _subject.ImageUrl.Should().Be(string.Empty);
        }
    }
}