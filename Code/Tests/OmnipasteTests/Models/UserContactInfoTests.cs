namespace OmnipasteTests.Models
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Models;
    using Omnipaste.Models;

    [TestFixture]
    public class UserUserUserContactInfoTests
    {
        [Test]
        public void Ctor_WithUserInfo_SetsFirstNameToTheUserInfoFirstName()
        {
            var UserContactInfo = new UserContactInfo(new UserInfo { FirstName = "someName" });

            UserContactInfo.FirstName.Should().Be("someName");
        }

        [Test]
        public void Ctor_WithUserInfo_SetsLastNameToTheUserInfoLastName()
        {
            var UserContactInfo = new UserContactInfo(new UserInfo { LastName = "someName" });

            UserContactInfo.LastName.Should().Be("someName");
        }

        [Test]
        public void Ctor_WithUserInfo_SetsImageUriToTheUserInfoImageUrl()
        {
            var UserContactInfo = new UserContactInfo(new UserInfo { ImageUrl = "http://some_url/" });

            UserContactInfo.ImageUri.ToString().Should().Be("http://some_url/");
        }
    }
}