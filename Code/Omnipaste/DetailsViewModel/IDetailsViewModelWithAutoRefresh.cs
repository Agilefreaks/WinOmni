namespace Omnipaste.DetailsViewModel
{
    using System;
    using OmniUI.Details;
    using OmniUI.Presenters;

    public interface IDetailsViewModelWithAutoRefresh<TModel> : IDetailsViewModel<TModel>, IDisposable
        where TModel : IPresenter
    {
    }
}