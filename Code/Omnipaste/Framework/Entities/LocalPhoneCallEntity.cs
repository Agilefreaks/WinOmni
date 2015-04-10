namespace Omnipaste.Framework.Entities
{
    using PhoneCalls.Dto;

    public class LocalPhoneCallEntity : PhoneCallEntity
    {
        public LocalPhoneCallEntity()
        {
        }

        public LocalPhoneCallEntity(PhoneCallDto phoneCallDto)
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