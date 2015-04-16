namespace OmniUI.Workspaces
{
    using Caliburn.Micro;

    public abstract class MasterDetailsWorkspace : Conductor<IScreen>.Collection.AllActive, IMasterDetailsWorkspace
    {
        #region Fields

        private readonly IDetailsConductorViewModel _detailsConductor;

        private readonly IScreen _masterScreen;

        private string _background;

        #endregion

        #region Constructors and Destructors

        protected MasterDetailsWorkspace(IScreen masterScreen, IDetailsConductorViewModel detailsConductor)
        {
            _masterScreen = masterScreen;
            _detailsConductor = detailsConductor;
        }

        #endregion

        #region Public Properties

        public IDetailsConductorViewModel DetailsConductor
        {
            get
            {
                return _detailsConductor;
            }
        }

        public IScreen MasterScreen
        {
            get
            {
                return _masterScreen;
            }
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

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(_masterScreen);
            ActivateItem(DetailsConductor);
        }

        #endregion
    }
}