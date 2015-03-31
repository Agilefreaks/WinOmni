namespace Omnipaste.Framework.Models
{
    using Omnipaste.Framework.Entities;

    public class LocalPhoneCallModel : PhoneCallModel<LocalPhoneCallEntity>
    {
        public LocalPhoneCallModel(LocalPhoneCallEntity backingEntity)
            : base(backingEntity)
        {
        }
    }
}