namespace OmniUI.Details
{
    using Caliburn.Micro;
    using OmniUI.Models;

    public interface IDetailsViewModel : IScreen
    {
        IModel Model { get; set; }
    }

    public interface IDetailsViewModel<TPresenter> : IDetailsViewModel
        where TPresenter : IModel
    {
        new TPresenter Model { get; set; }
    }
}