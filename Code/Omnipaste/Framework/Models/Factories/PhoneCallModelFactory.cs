namespace Omnipaste.Framework.Models.Factories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Services.Repositories;

    public interface IPhoneCallModelFactory
    {
        IObservable<IConversationModel> Create(PhoneCallEntity phoneCallEntity);
    }

    public class PhoneCallModelFactory : ConversationModelFactory, IPhoneCallModelFactory
    {
        public PhoneCallModelFactory(IContactRepository contactRepository)
            : base(contactRepository)
        {
        }

        public IObservable<IConversationModel> Create(PhoneCallEntity phoneCallEntity)
        {
            return ContactRepository.Get(phoneCallEntity.ContactUniqueId).Select(
                ci =>
                    {
                        var localPhoneCall = phoneCallEntity as LocalPhoneCallEntity;
                        var phoneCallModel = localPhoneCall != null ? 
                            (PhoneCallModel)new LocalPhoneCallModel(localPhoneCall) : 
                            new RemotePhoneCallModel((RemotePhoneCallEntity)phoneCallEntity);
                        phoneCallModel.ContactModel = new ContactModel(ci);

                        return phoneCallModel;
                    });
        }
    }
}