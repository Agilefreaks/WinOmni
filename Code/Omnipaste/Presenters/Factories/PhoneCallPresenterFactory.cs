namespace Omnipaste.Presenters.Factories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public interface IPhoneCallPresenterFactory
    {
        IObservable<IConversationPresenter> Create(PhoneCall phoneCall);
    }

    public class PhoneCallPresenterFactory : ConversationPresenterFactory, IPhoneCallPresenterFactory
    {
        public PhoneCallPresenterFactory(IContactRepository contactRepository)
            : base(contactRepository)
        {
        }

        public IObservable<IConversationPresenter> Create(PhoneCall phoneCall)
        {
            return ContactRepository.Get(phoneCall.ContactInfoUniqueId).Select(
                ci =>
                    {
                        var localPhoneCall = phoneCall as LocalPhoneCall;
                        var phoneCallPresenter = localPhoneCall != null ? 
                            (PhoneCallPresenter)new LocalPhoneCallPresenter(localPhoneCall) : 
                            new RemotePhoneCallPresenter((RemotePhoneCall)phoneCall);
                        phoneCallPresenter.ContactInfoPresenter = new ContactInfoPresenter(ci);

                        return phoneCallPresenter;
                    });
        }
    }
}