namespace OmnipasteTests.Profile
{
    using System.Linq;
    using Caliburn.Micro;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Profile;
    using Omnipaste.Profile.UserProfile;

    [TestFixture]
    public class ProfileWorkspaceTests
    {
        private Mock<IUserProfileViewModel> _mockProfileViewModel;
        private IProfileWorkspace _subject;

        [SetUp]
        public void SetUp()
        {
            _mockProfileViewModel = new Mock<IUserProfileViewModel>();
            _subject = new ProfileWorkspace();

            _subject.UserProfileViewModel = _mockProfileViewModel.Object;
        }

        [Test]
        public void OnActivate_Always_AddsUserProfileViewModelToItems()
        {
            _subject.Activate();

            ((Conductor<IScreen>.Collection.AllActive)_subject).Items.First().Should().Be(_mockProfileViewModel.Object);
        }
    }
}