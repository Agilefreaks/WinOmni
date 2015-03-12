namespace Omnipaste.Models
{
    using PhoneCalls.Models;

    public class LocalPhoneCall : PhoneCall
    {
        public LocalPhoneCall()
        {
        }

        public LocalPhoneCall(PhoneCallDto phoneCallDto)
            : base(phoneCallDto)
        {
        }

        public override SourceType Source
        {
            get
            {
                return SourceType.Local;
            }
        }
    }
}