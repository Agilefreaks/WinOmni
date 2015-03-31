namespace Omnipaste.Presenters.Factories
{
    using System;
    using Omnipaste.Entities;
    using Omnipaste.Models;

    public interface IConversationPresenterFactory
    {
        IObservable<TPresenter> Create<TPresenter, TModel>(TModel model)
            where TPresenter : IConversationPresenter
            where TModel : ConversationEntity;
    }
}