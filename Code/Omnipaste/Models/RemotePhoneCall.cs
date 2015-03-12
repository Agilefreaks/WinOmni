namespace Omnipaste.Models
{
    using PhoneCalls.Models;

    public class RemotePhoneCall : PhoneCall
    {
        public RemotePhoneCall()
        {
        }

        public RemotePhoneCall(PhoneCallDto phoneCallDto)
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