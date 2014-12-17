namespace Omnipaste.DetailsViewModel
{
    using System;
    using OmniUI.Details;

    public interface IDetailsViewModelWithAutoRefresh<TModel> : IDetailsViewModel<TModel>, IDisposable
    {
    }
}