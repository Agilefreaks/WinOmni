namespace OmniUI.Details
{
    using Caliburn.Micro;
    using OmniUI.Framework.Models;

    public interface IDetailsViewModel : IScreen
    {
        IModel Model { get; set; }
    }

    public interface IDetailsViewModel<TModel> : IDetailsViewModel
        where TModel : IModel
    {
        new TModel Model { get; set; }
    }
}