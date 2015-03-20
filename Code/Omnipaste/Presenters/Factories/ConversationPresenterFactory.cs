namespace Omnipaste.Presenters.Factories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public class ConversationPresenterFactory : IConversationPresenterFactory
    {
        protected readonly IContactRepository ContactRepository;

        public ConversationPresenterFactory(IContactRepository contactRepository)
        {
            ContactRepository = contactRepository;
        }

        public IObservable<TPresenter> Create<TPresenter, TModel>(TModel model)
            where TPresenter : IConversationPresenter
            where TModel : ConversationBaseModel
        {
            var presenter = (TPresenter)Activator.CreateInstance(typeof(TPresenter), model);
            return ContactRepository.Get(model.ContactInfoUniqueId).Select(c => (TPresenter)presenter.SetContactInfoPresenter(new ContactInfoPresenter(c)));
        }
    }
}
