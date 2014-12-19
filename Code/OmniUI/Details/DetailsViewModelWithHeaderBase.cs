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

        public TContent ContentViewModel { get; protected set; }

        public THeader HeaderViewModel { get; protected set; }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(HeaderViewModel);
            ActivateItem(ContentViewModel);
        }

        #endregion
    }
}