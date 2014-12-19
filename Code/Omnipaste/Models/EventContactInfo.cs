namespace Omnipaste.Models
{
    using System.Linq;
    using OmniUI.Models;
    using Events.Models;

    public class EventContactInfo : ContactInfo
    {
        public EventContactInfo(Event @event)
        {
            var nameParts = string.IsNullOrWhiteSpace(@event.ContactName)
                                ? new string[0]
                                : @event.ContactName.Split(NamePartSeparator[0]);
            if (nameParts.Length == 1)
            {
                FirstName = nameParts.First();
            }
            else if (nameParts.Length > 1)
            {
                FirstName = string.Join(NamePartSeparator, nameParts.Take(nameParts.Length - 1));
                LastName = nameParts.Last();
            }

            Phone = @event.PhoneNumber;
        }
    }
}