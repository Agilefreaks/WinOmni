namespace Omnipaste.Models.Factories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public interface IConversationModelFactory
    {
        IObservable<TPresenter> Create<TPresenter, TModel>(TModel model)
            where TPresenter : IConversationModel
            where TModel : ConversationEntity;
    }

    public class ConversationModelFactory : IConversationModelFactory
    {
        protected readonly IContactRepository ContactRepository;

        public ConversationModelFactory(IContactRepository contactRepository)
        {
            ContactRepository = contactRepository;
        }

        public IObservable<TPresenter> Create<TPresenter, TModel>(TModel model)
            where TPresenter : IConversationModel
            where TModel : ConversationEntity
        {
            var presenter = (TPresenter)Activator.CreateInstance(typeof(TPresenter), model);
            return ContactRepository.Get(model.ContactInfoUniqueId).Select(c => (TPresenter)presenter.SetContactModel(new ContactModel(c)));
        }
    }
}
