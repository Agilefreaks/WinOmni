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

        public IObservable<PhoneCall> Create(PhoneCallDto phoneCallDto)
        {
            return
                ContactRepository.GetOrCreateByPhoneNumber(phoneCallDto.Number)
                    .Select(contact => new PhoneCall(phoneCallDto) { ContactInfo = contact })
                    .Select(pc => _phoneCallRepository.Save(pc))
                    .Switch()
                    .Select(e => e.Item);

        }
    }
}