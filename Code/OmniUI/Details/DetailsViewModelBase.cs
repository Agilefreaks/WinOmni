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
                if (_model != null && IsActive)
                {
                    UnhookModel(_model);
                }
                _model = value;
                if (_model != null && IsActive)
                {
                    HookModel(_model);
                }
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

        protected override void OnActivate()
        {
            if (_model != null)
            {
                HookModel(_model);
            }
            
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            if (_model != null)
            {
                UnhookModel(_model);
            }

            base.OnDeactivate(close);
        }

        protected virtual void UnhookModel(TModel model)
        {
        }

        protected virtual void HookModel(TModel model)
        {
        }
    }
}