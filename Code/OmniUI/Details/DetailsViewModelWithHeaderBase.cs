namespace OmniUI.Details
{
    using Caliburn.Micro;

    public abstract class DetailsViewModelWithHeaderBase<THeader, TContent> : Conductor<IScreen>.Collection.AllActive,
                                                                              IDetailsViewModelWithHeader<THeader, TContent>
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

        public TContent ContentViewModel { get; private set; }

        public THeader HeaderViewModel { get; private set; }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(HeaderViewModel);
            ActivateItem(ContentViewModel);
        }

        protected override void OnDeactivate(bool close)
        {
            DeactivateItem(HeaderViewModel, close);
            DeactivateItem(ContentViewModel, close);
            base.OnDeactivate(close);
        }

        #endregion
    }
}