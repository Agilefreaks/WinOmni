namespace Omnipaste.DetailsViewModel
{
    using Caliburn.Micro;
    using Omnipaste.Event;

    public abstract class DetailsViewModelBase<TEntity> : Screen, IDetailsViewModel<TEntity>
    {
        #region Constructors and Destructors

        #endregion

        #region Public Properties

        public TEntity Model { get; set; }

        #endregion
    }
}