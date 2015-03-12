namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using OmniCommon.Helpers;
    using Omnipaste.Helpers;
    using Omnipaste.Models;

    public class ContactRepository : SecurePermanentRepository<ContactInfo>, IContactRepository
    {
        public ContactRepository()
            : base("contacts")
        {
        }

        #region IContactRepository Members

        public IObservable<ContactInfo> GetByPhoneNumber(string phoneNumber)
        {
            return Get(contact => contact.PhoneNumbers.Any(pn => PhoneNumberMatcher.IsMatch(pn.Number, phoneNumber)));
        }

        public IObservable<ContactInfo> GetOrCreateByPhoneNumber(string phoneNumber)
        {
            return
                GetByPhoneNumber(phoneNumber)
                    .Catch<ContactInfo, Exception>(
                        e =>
                            {
                                return
                                    Save(
                                        new ContactInfo
                                            {
                                                PhoneNumbers =
                                                    new[] { new PhoneNumber { Number = phoneNumber } }
                                            })
                                        .Select(o => o.Item);
                            });
        }

        public IObservable<ContactInfo> UpdateLastActivityTime(
            ContactInfo contactInfo,
            DateTime? lastActivityTime = null)
        {
            contactInfo.LastActivityTime = TimeHelper.UtcNow;
            return Save(contactInfo).Select(o => o.Item);
        }

        #endregion
    }
}