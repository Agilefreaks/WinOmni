namespace Omnipaste.DetailsViewModel
{
    using Caliburn.Micro;

    public interface IDetailsViewModel : IScreen
    {
        object Model { get; set; }
    }

    public interface IDetailsViewModel<TEntity> : IDetailsViewModel
    {
        new TEntity Model { get; set; }
    }
}