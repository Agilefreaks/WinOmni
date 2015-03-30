namespace OmniUI.Details
{
    using Caliburn.Micro;
    using OmniUI.Presenters;

    public abstract class DetailsViewModelBase<TModel> : Screen, IDetailsViewModel<TModel>
        where TModel : class, IPresenter
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
                
                TryUnhookModel(_model);
                _model = value;
                TryHookModel(_model);
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Explicit Interface Properties

        IPresenter IDetailsViewModel.Model
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
            TryHookModel(_model);

            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            TryUnhookModel(_model);

            base.OnDeactivate(close);
        }

        protected virtual void UnhookModel(TModel model)
        {
        }

        protected virtual void HookModel(TModel model)
        {
        }

        private void TryHookModel(TModel model)
        {
            if (model != null && IsActive)
            {
                HookModel(model);
            }
        }

        private void TryUnhookModel(TModel model)
        {
            if (model != null)
            {
                UnhookModel(model);
            }
        }
    }
}