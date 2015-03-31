namespace Omnipaste.Models
{
    using Omnipaste.Entities;
    using PhoneCalls.Models;

    public class RemotePhoneCallEntity : PhoneCallEntity
    {
        public RemotePhoneCallEntity()
        {
        }

        public RemotePhoneCallEntity(PhoneCallDto phoneCallDto)
            : base(phoneCallDto)
        {
        }

        public override SourceType Source
        {
            get
            {
                return SourceType.Remote;
            }
        }
    }
}