namespace OmnipasteTests.Models
{
    using System;
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
            var userContactInfo = new UserContactEntity(UserInfo.BeginBuild().WithFirstName("someName").Build());

            userContactInfo.FirstName.Should().Be("someName");
        }

        [Test]
        public void Ctor_WithUserInfo_SetsLastNameToTheUserInfoLastName()
        {
            var userContactInfo = new UserContactEntity(UserInfo.BeginBuild().WithLastName("someName").Build());

            userContactInfo.LastName.Should().Be("someName");
        }

        [Test]
        public void Ctor_WithUserInfo_SetsImageUriToTheUserInfoImageUrl()
        {
            var userContactInfo = new UserContactEntity(UserInfo.BeginBuild().WithImageUrl("http://some_url/").Build());

            userContactInfo.ImageUri.ToString().Should().Be("http://some_url/");
        }

        [Test]
        public void Ctor_WithUserInfoNull_DoesNotThrowException()
        {
            Action actionToTest = () => new UserContactEntity();

            actionToTest.ShouldNotThrow<Exception>();
        }
    }
}