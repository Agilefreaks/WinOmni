namespace OmnipasteTests.Entities
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Models;
    using Omnipaste.Entities;

    [TestFixture]
    public class UserEntityTests
    {
        [Test]
        public void Ctor_WithUserInfo_SetsFirstNameToTheUserInfoFirstName()
        {
            var userContactInfo = new UserEntity(UserInfo.BeginBuild().WithFirstName("someName").Build());

            userContactInfo.FirstName.Should().Be("someName");
        }

        [Test]
        public void Ctor_WithUserInfo_SetsLastNameToTheUserInfoLastName()
        {
            var userContactInfo = new UserEntity(UserInfo.BeginBuild().WithLastName("someName").Build());

            userContactInfo.LastName.Should().Be("someName");
        }

        [Test]
        public void Ctor_WithUserInfo_SetsImageUriToTheUserInfoImageUrl()
        {
            var userContactInfo = new UserEntity(UserInfo.BeginBuild().WithImageUrl("http://some_url/").Build());

            userContactInfo.ImageUri.ToString().Should().Be("http://some_url/");
        }

        [Test]
        public void Ctor_WithUserInfoNull_DoesNotThrowException()
        {
            Action actionToTest = () => new UserEntity();

            actionToTest.ShouldNotThrow<Exception>();
        }
    }
}