namespace Omnipaste.Profile
{
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

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(UserProfileViewModel);
        }
    }
}