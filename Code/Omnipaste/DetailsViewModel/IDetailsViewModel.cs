namespace Omnipaste.DetailsViewModel
{
    using Caliburn.Micro;

    public interface IDetailsViewModel<TEntity> : IScreen
    {
        TEntity Model { get; set; }
    }
}