namespace Omnipaste.Models
{
    public class RemotePhoneCallModel : PhoneCallModel<RemotePhoneCallEntity>
    {
        public RemotePhoneCallModel(RemotePhoneCallEntity backingEntity)
            : base(backingEntity)
        {
        }
    }
}