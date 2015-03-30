namespace OmniUI.Details
{
    using Caliburn.Micro;
    using OmniUI.Attributes;

    [UseView(typeof(DetailsViewWithHeader))]
    public abstract class DetailsViewModelWithHeaderBase<THeader, TContent> : Conductor<IScreen>.Collection.AllActive,
                                                                              IDetailsViewModelWithHeader<THeader, TContent>
        where THeader : IDetailsViewModel
        where TContent : IDetailsViewModel
    {
        private object _model;

        protected DetailsViewModelWithHeaderBase(THeader headerViewModel, TContent containerViewModel)
        {
            HeaderViewModel = headerViewModel;
            ContentViewModel = containerViewModel;
        }

        public TContent ContentViewModel { get; private set; }

        public THeader HeaderViewModel { get; private set; }

        public object Model
        {
            get
            {
                return _model;
            }
            set
            {
                if (Equals(value, _model))
                {
                    return;
                }
                _model = value;
                HeaderViewModel.Model = _model;
                ContentViewModel.Model = _model;
                NotifyOfPropertyChange();
            }
        }

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
    }
}