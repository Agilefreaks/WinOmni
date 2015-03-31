namespace Omnipaste.Models.Factories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public interface IPhoneCallPresenterFactory
    {
        IObservable<IConversationModel> Create(PhoneCallEntity phoneCallEntity);
    }

    public class PhoneCallModelFactory : ConversationModelFactory, IPhoneCallPresenterFactory
    {
        public PhoneCallModelFactory(IContactRepository contactRepository)
            : base(contactRepository)
        {
        }

        public IObservable<IConversationModel> Create(PhoneCallEntity phoneCallEntity)
        {
            return ContactRepository.Get(phoneCallEntity.ContactInfoUniqueId).Select(
                ci =>
                    {
                        var localPhoneCall = phoneCallEntity as LocalPhoneCallEntity;
                        var phoneCallPresenter = localPhoneCall != null ? 
                            (PhoneCallModel)new LocalPhoneCallModel(localPhoneCall) : 
                            new RemotePhoneCallModel((RemotePhoneCallEntity)phoneCallEntity);
                        phoneCallPresenter.ContactModel = new ContactModel(ci);

                        return phoneCallPresenter;
                    });
        }
    }
}