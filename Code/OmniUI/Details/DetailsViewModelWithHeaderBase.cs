namespace OmniUI.Details
{
    using Caliburn.Micro;

    public abstract class DetailsViewModelWithHeaderBase<THeader, TContent> : Screen,
                                                                              IDetailsViewModelWithHeader
                                                                                  <THeader, TContent>
        where THeader : IScreen where TContent : IScreen
    {
        #region Constructors and Destructors

        protected DetailsViewModelWithHeaderBase(THeader headerViewModel, TContent contentViewModel)
        {
            HeaderViewModel = headerViewModel;
            ContentViewModel = contentViewModel;
        }

        #endregion

        #region Public Properties

        public TContent ContentViewModel { get; protected set; }

        public THeader HeaderViewModel { get; protected set; }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            HeaderViewModel.Activate();
            ContentViewModel.Activate();
        }

        protected override void OnDeactivate(bool close)
        {
            HeaderViewModel.Deactivate(close);
            ContentViewModel.Deactivate(close);
            base.OnDeactivate(close);
        }

        #endregion
    }
}