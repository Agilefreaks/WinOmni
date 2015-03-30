namespace OmniUI.Details
{
    using Caliburn.Micro;
    using OmniUI.Presenters;

    public interface IDetailsViewModel : IScreen
    {
        IPresenter Model { get; set; }
    }

    public interface IDetailsViewModel<TPresenter> : IDetailsViewModel
        where TPresenter : IPresenter
    {
        new TPresenter Model { get; set; }
    }
}