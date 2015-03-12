namespace Omnipaste.Factories
{
    using System;
    using System.Reactive.Linq;
    using Ninject;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using PhoneCalls.Models;

    public class PhoneCallFactory : IPhoneCallFactory
    {
        private readonly IPhoneCallRepository _phoneCallRepository;

        [Inject]
        public IContactRepository ContactRepository { get; set; }

        public PhoneCallFactory(IPhoneCallRepository phoneCallRepository)
        {
            _phoneCallRepository = phoneCallRepository;
        }

        public IObservable<T> Create<T>(PhoneCallDto phoneCallDto)
            where T : PhoneCall
        {
            PhoneCall phoneCall = (T)Activator.CreateInstance(typeof(T), phoneCallDto);
            return
                ContactRepository.GetOrCreateByPhoneNumber(phoneCallDto.Number)
                    .Select(contact => ContactRepository.UpdateLastActivityTime(contact))
                    .Switch()
                    .Select(contact => _phoneCallRepository.Save(phoneCall.SetContactInfo(contact)))
                    .Switch()
                    .Select(e => e.Item)
                    .Cast<T>();

        }
    }
}