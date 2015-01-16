namespace Omnipaste.Workspaces
{
    using Caliburn.Micro;
    using OmniUI.Workspace;

    public abstract class SingleItemWorkspace : Conductor<IScreen>, IWorkspace
    {
        private string _background;

        private readonly IScreen _defaultScreen;

        protected SingleItemWorkspace(IScreen defaultScreen)
        {
            _defaultScreen = defaultScreen;
        }

        public virtual string Background
        {
            get
            {
                return _background;
            }
            set
            {
                if (value == _background)
                {
                    return;
                }
                _background = value;
                NotifyOfPropertyChange();
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(_defaultScreen);
        }
    }
}