namespace Omnipaste.Framework.DetailsViewModel
{
    using System;
    using OmniUI.Details;
    using OmniUI.Framework.Models;

    public interface IDetailsViewModelWithAutoRefresh<TModel> : IDetailsViewModel<TModel>, IDisposable
        where TModel : IModel
    {
    }
}