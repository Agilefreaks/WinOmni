namespace OmniUI.Details
{
    using Caliburn.Micro;

    public abstract class DetailsViewModelBase<TModel> : Screen, IDetailsViewModel<TModel>
        where TModel : class
    {
        #region Fields

        private TModel _model;

        #endregion

        #region Public Properties

        public virtual TModel Model
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
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Explicit Interface Properties

        object IDetailsViewModel.Model
        {
            get
            {
                return Model;
            }
            set
            {
                Model = value as TModel;
            }
        }

        #endregion
    }
}