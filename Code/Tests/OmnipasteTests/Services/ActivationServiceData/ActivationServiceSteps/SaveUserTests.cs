﻿namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Reactive.Linq;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class SaveUserTests
    {
        private SaveUser _subject;
        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void SetUp()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            _subject = new SaveUser(_mockConfigurationService.Object);
        }

        [Test]
        public void Execute_WhenParameterValueIsUser_SetsUserInfo()
        {
            _subject.Parameter = new DependencyParameter { Value = new User() };
            
            _subject.Execute().Wait();

            _mockConfigurationService.VerifySet(x => x.UserInfo);
        }
        
        [Test]
        public void Execute_WhenParameterValueIsUser_SetsPropertiesOnUserInfo()
        {
            var user = new User
            {
                FirstName = "First",
                LastName = "Last",
                Email = "test@user.com",
                ImageUrl = "http://image.com"
            };
            _subject.Parameter = new DependencyParameter
            {
                Value = user
            };
            UserInfo userInfo = null;
            _mockConfigurationService.SetupSet(x => x.UserInfo)
                .Callback(data => userInfo = data);

            _subject.Execute().Wait();

            userInfo.FirstName.Should().Be(user.FirstName);
            userInfo.LastName.Should().Be(user.LastName);
            userInfo.Email.Should().Be(user.Email);
            userInfo.ImageUrl.Should().Be(user.ImageUrl);
        }
    }
}