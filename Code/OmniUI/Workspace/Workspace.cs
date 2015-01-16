namespace OmniUI.Workspace
{
    using Caliburn.Micro;

    public class Workspace : Conductor<IScreen>.Collection.AllActive, IWorkspace
    {
        private string _background;

        private readonly IScreen _defaultScreen;

        protected Workspace(IScreen defaultScreen)
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