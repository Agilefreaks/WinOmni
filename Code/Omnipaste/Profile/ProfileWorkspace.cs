namespace Omnipaste.Profile
{
    using System.Windows;
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.Profile.UserProfile;
    using OmniUI.Attributes;
    using OmniUI.Workspaces;

    [UseView(typeof(WorkspaceView))]
    public class ProfileWorkspace : Conductor<IScreen>.Collection.AllActive, IProfileWorkspace
    {
        [Inject]
        public IUserProfileViewModel UserProfileViewModel { get; set; }

        // TODO: override OnActivate and activate the user profile
        // this
        protected override void OnActivate()
        {
            base.OnActivate();
            this.ActivateItem(UserProfileViewModel);
        }
    }
}