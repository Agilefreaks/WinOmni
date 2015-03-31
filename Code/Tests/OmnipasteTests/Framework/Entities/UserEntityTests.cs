namespace OmnipasteTests.Framework.Entities
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniCommon.Models;
    using Omnipaste.Framework.Entities;

    [TestFixture]
    public class UserEntityTests
    {
        [Test]
        public void Ctor_WithUserInfo_SetsFirstNameToTheUserInfoFirstName()
        {
            var userEntity = new UserEntity(UserInfo.BeginBuild().WithFirstName("someName").Build());

            userEntity.FirstName.Should().Be("someName");
        }

        [Test]
        public void Ctor_WithUserInfo_SetsLastNameToTheUserInfoLastName()
        {
            var userEntity = new UserEntity(UserInfo.BeginBuild().WithLastName("someName").Build());

            userEntity.LastName.Should().Be("someName");
        }

        [Test]
        public void Ctor_WithUserInfo_SetsImageUriToTheUserInfoImageUrl()
        {
            var userEntity = new UserEntity(UserInfo.BeginBuild().WithImageUrl("http://some_url/").Build());

            userEntity.ImageUri.ToString().Should().Be("http://some_url/");
        }

        [Test]
        public void Ctor_WithUserInfoNull_DoesNotThrowException()
        {
            Action actionToTest = () => new UserEntity();

            actionToTest.ShouldNotThrow<Exception>();
        }
    }
}