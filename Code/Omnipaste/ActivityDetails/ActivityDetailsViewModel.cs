namespace Omnipaste.ActivityDetails
{
    using Omnipaste.Presenters;
    using OmniUI.Attributes;
    using OmniUI.Details;

    [UseView(typeof(DetailsViewWithHeader))]
    public class ActivityDetailsViewModel :
        DetailsViewModelWithHeaderBase<IActivityDetailsHeaderViewModel, IActivityDetailsContentViewModel>,
        IActivityDetailsViewModel
    {
        #region Fields

        private ActivityPresenter _model;

        #endregion

        #region Constructors and Destructors

        public ActivityDetailsViewModel(
            IActivityDetailsHeaderViewModel headerViewModel,
            IActivityDetailsContentViewModel contentViewModel)
            : base(headerViewModel, contentViewModel)
        {
        }

        #endregion

        #region Public Properties

        public ActivityPresenter Model
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

        #endregion
    }
}