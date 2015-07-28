namespace Omnipaste.Profile
{
    using Caliburn.Micro;
    using Omnipaste.Profile.UserProfile;
    using OmniUI.Attributes;
    using OmniUI.Workspaces;

    [UseView(typeof(WorkspaceView))]
    public class ProfileWorkspace : Conductor<IScreen>.Collection.AllActive, IProfileWorkspace
    {
        // TODO: fix this property
        public IUserProfileViewModel UserProfileViewModel { get; set; }

        // TODO: override OnActivate and activate the user profile
    }
}