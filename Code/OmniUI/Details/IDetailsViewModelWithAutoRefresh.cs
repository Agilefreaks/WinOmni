namespace OmniUI.Details
{
    using System;
    using OmniUI.Framework.Models;

    public interface IDetailsViewModelWithAutoRefresh<TModel> : IDetailsViewModel<TModel>, IDisposable
        where TModel : IModel
    {
    }
}