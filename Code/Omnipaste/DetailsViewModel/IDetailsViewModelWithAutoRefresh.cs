namespace Omnipaste.DetailsViewModel
{
    using System;
    using OmniUI.Details;
    using OmniUI.Models;

    public interface IDetailsViewModelWithAutoRefresh<TModel> : IDetailsViewModel<TModel>, IDisposable
        where TModel : IModel
    {
    }
}