namespace Omnipaste.DetailsViewModel
{
    using Caliburn.Micro;

    public abstract class DetailsViewModelBase<TEntity> : Screen, IDetailsViewModel<TEntity>
    {
        #region Public Properties

        public TEntity Model { get; set; }

        #endregion
    }
}