namespace Omnipaste.Presenters.Factories
{
    using System;
    using Omnipaste.Models;

    public interface IConversationPresenterFactory
    {
        IObservable<TPresenter> Create<TPresenter, TModel>(TModel model)
            where TPresenter : IConversationPresenter
            where TModel : ConversationBaseModel;
    }
}