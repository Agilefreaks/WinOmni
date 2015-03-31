namespace Omnipaste.Presenters.Factories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public interface IPhoneCallPresenterFactory
    {
        IObservable<IConversationPresenter> Create(PhoneCallEntity phoneCallEntity);
    }

    public class PhoneCallPresenterFactory : ConversationPresenterFactory, IPhoneCallPresenterFactory
    {
        public PhoneCallPresenterFactory(IContactRepository contactRepository)
            : base(contactRepository)
        {
        }

        public IObservable<IConversationPresenter> Create(PhoneCallEntity phoneCallEntity)
        {
            return ContactRepository.Get(phoneCallEntity.ContactInfoUniqueId).Select(
                ci =>
                    {
                        var localPhoneCall = phoneCallEntity as LocalPhoneCallEntity;
                        var phoneCallPresenter = localPhoneCall != null ? 
                            (PhoneCallPresenter)new LocalPhoneCallPresenter(localPhoneCall) : 
                            new RemotePhoneCallPresenter((RemotePhoneCallEntity)phoneCallEntity);
                        phoneCallPresenter.ContactInfoPresenter = new ContactInfoPresenter(ci);

                        return phoneCallPresenter;
                    });
        }
    }
}