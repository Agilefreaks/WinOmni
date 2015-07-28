namespace Omnipaste.Profile
{
    using Caliburn.Micro;
    using Omnipaste.Profile.UserProfile;

    public interface IProfileWorkspace : IScreen, IConductor
    {
        IUserProfileViewModel UserProfileViewModel { get; set; }
    }
}