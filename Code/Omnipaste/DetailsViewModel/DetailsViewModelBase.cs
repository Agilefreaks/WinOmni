namespace Omnipaste.DetailsViewModel
{
    using Caliburn.Micro;

    public abstract class DetailsViewModelBase<TEntity> : Screen, IDetailsViewModel<TEntity>
    {
        #region Fields

        private TEntity _model;

        #endregion

        #region Public Properties

        public virtual TEntity Model
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
    }
}