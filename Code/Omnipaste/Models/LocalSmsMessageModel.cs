namespace Omnipaste.Models
{
    using Omnipaste.Entities;

    public class LocalSmsMessageModel : SmsMessageModel<LocalSmsMessageEntity>
    {
        public LocalSmsMessageModel(LocalSmsMessageEntity backingEntity)
            : base(backingEntity)
        {
        }
    }
}