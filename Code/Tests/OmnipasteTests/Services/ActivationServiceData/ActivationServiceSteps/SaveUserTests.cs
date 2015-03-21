namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniCommon;
    using OmniCommon.Helpers;
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

            _mockConfigurationService.VerifySet(x => x.UserInfo = It.IsAny<UserInfo>());
        }
        
        [Test]
        public void Execute_WhenParameterValueIsUser_SetsPropertiesOnUserInfo()
        {
            var user = new User
            {
                FirstName = "First",
                LastName = "Last",
                Email = "test@user.com",
                ImageUrl = "http://image.com",
                ContactsUpdatedAt = DateTime.Now
            };
            _subject.Parameter = new DependencyParameter
            {
                Value = user
            };
            UserInfo userInfo = null;
            _mockConfigurationService.SetupSet(x => x.UserInfo = It.IsAny<UserInfo>())
                .Callback<UserInfo>(data => userInfo = data);

            _subject.Execute().Wait();

            userInfo.FirstName.Should().Be(user.FirstName);
            userInfo.LastName.Should().Be(user.LastName);
            userInfo.Email.Should().Be(user.Email);
            userInfo.ImageUrl.Should().Be(user.ImageUrl);
        }

        [Test]
        public void Execute_SMSSuffixEnabledSettingDoesNotExistUserHasViaOmnipasteTrue_SetsConfigurationServiceUseSMSPrefixTrue()
        {
            var user = new User
            {
                FirstName = "First",
                LastName = "Last",
                Email = "test@user.com",
                ImageUrl = "http://image.com",
                ViaOmnipaste = true
            };
            _subject.Parameter = new DependencyParameter
            {
                Value = user
            };
            _mockConfigurationService.Setup(x => x.HasSavedValueFor(ConfigurationProperties.SMSSuffixEnabled))
                .Returns(false);

            _subject.Execute().Wait();

            _mockConfigurationService.VerifySet(x => x.IsSMSSuffixEnabled = true);
        }

        [Test]
        public void Execute_SMSSuffixEnabledSettingDoesExistsUserHasViaOmnipasteTrue_DoesNotChangeSetting()
        {
            var user = new User
            {
                FirstName = "First",
                LastName = "Last",
                Email = "test@user.com",
                ImageUrl = "http://image.com",
                ViaOmnipaste = true
            };
            _subject.Parameter = new DependencyParameter
            {
                Value = user
            };
            _mockConfigurationService.Setup(x => x.HasSavedValueFor(ConfigurationProperties.SMSSuffixEnabled))
                .Returns(true);

            _subject.Execute().Wait();

            _mockConfigurationService.VerifySet(x => x.IsSMSSuffixEnabled = It.IsAny<bool>(), Times.Never());
        }

        [Test]
        public void Execute_WhenUpdatedAtIsNotTheSameAsLocal_UpdatesInfo()
        {
            using (TimeHelper.Freez())
            {
                var user = new User { FirstName = "Crocobaur", UpdatedAt = TimeHelper.UtcNow };
                var userInfo = new UserInfo { FirstName = "Croco", UpdatedAt = TimeHelper.UtcNow.AddDays(-1) };
                _mockConfigurationService.Setup(m => m.UserInfo).Returns(userInfo);
                _subject.Parameter = new DependencyParameter { Value = user };

                _subject.Execute().Wait();

                _mockConfigurationService.VerifySet(
                    x => x.UserInfo = It.Is<UserInfo>(ui => ui.FirstName == "Crocobaur"));
            }
        }

        [Test]
        public void Execute_Never_OverwritesContactsUpdatedAt()
        {
            using (TimeHelper.Freez())
            {
                _subject.Parameter = new DependencyParameter { Value = new User() };
                var userInfo = new UserInfo().SetContactsUpdatedAt(TimeHelper.UtcNow);
                _mockConfigurationService.SetupGet(x => x.UserInfo).Returns(userInfo);

                _subject.Execute().Wait();

                _mockConfigurationService.VerifySet(
                    x => x.UserInfo = It.Is<UserInfo>(ui => ui.ContactsUpdatedAt == TimeHelper.UtcNow));
            }
        }
    }
}
