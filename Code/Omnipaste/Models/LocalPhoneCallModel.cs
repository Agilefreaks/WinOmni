namespace Omnipaste.Models
{
    using Omnipaste.Entities;

    public class LocalPhoneCallModel : PhoneCallModel<LocalPhoneCallEntity>
    {
        public LocalPhoneCallModel(LocalPhoneCallEntity backingEntity)
            : base(backingEntity)
        {
        }
    }
}