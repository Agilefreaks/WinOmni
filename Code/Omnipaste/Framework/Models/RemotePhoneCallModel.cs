namespace Omnipaste.Framework.Models
{
    using Omnipaste.Framework.Entities;

    public class RemotePhoneCallModel : PhoneCallModel<RemotePhoneCallEntity>
    {
        public RemotePhoneCallModel(RemotePhoneCallEntity backingEntity)
            : base(backingEntity)
        {
        }
    }
}