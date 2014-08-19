namespace Omnipaste.Event
{
    using Caliburn.Micro;

    public abstract class DetailsViewModelBase<TEntity> : Screen, IDetailsViewModel<TEntity>
    {
        #region Constructors and Destructors

        #endregion

        #region Public Properties

        public TEntity Model { get; set; }

        #endregion
    }
}