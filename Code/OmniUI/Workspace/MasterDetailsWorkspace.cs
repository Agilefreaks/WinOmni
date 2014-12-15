namespace OmniUI.Workspace
{
    using Caliburn.Micro;

    public abstract class MasterDetailsWorkspace : Workspace, IMasterDetailsWorkspace
    {
        #region Fields

        private readonly IDetailsConductorViewModel _detailsConductor;

        private readonly IScreen _masterScreen;

        #endregion

        #region Constructors and Destructors

        protected MasterDetailsWorkspace(IScreen masterScreen, IDetailsConductorViewModel detailsConductor)
            : base(masterScreen)
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

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(DetailsConductor);
        }

        #endregion
    }
}