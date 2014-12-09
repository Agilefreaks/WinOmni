namespace Omnipaste.Workspaces
{
    using Caliburn.Micro;

    public class Workspace : Conductor<IScreen>, IWorkspace
    {
        private readonly IScreen _defaultScreen;

        public Workspace(IScreen defaultScreen)
        {
            _defaultScreen = defaultScreen;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(_defaultScreen);
        }
    }
}