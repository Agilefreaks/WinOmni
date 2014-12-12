namespace Omnipaste.DetailsViewModel
{
    using System;

    public interface IDetailsViewModelWithAutoRefresh<TModel> : IDetailsViewModel<TModel>, IDisposable
    {
    }
}