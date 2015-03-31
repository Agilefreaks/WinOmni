namespace Omnipaste.Framework.Models.Factories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Services.Repositories;

    public interface IConversationModelFactory
    {
        IObservable<TConversationModel> Create<TConversationModel, TModel>(TModel model)
            where TConversationModel : IConversationModel
            where TModel : ConversationEntity;
    }

    public class ConversationModelFactory : IConversationModelFactory
    {
        protected readonly IContactRepository ContactRepository;

        public ConversationModelFactory(IContactRepository contactRepository)
        {
            ContactRepository = contactRepository;
        }

        public IObservable<TConversationModel> Create<TConversationModel, TModel>(TModel model)
            where TConversationModel : IConversationModel
            where TModel : ConversationEntity
        {
            var conversationModel = (TConversationModel)Activator.CreateInstance(typeof(TConversationModel), model);
            return ContactRepository.Get(model.ContactUniqueId).Select(c => (TConversationModel)conversationModel.SetContactModel(new ContactModel(c)));
        }
    }
}
