namespace Omnipaste.Framework.Models
{
    using Omnipaste.Framework.Entities;

    public class LocalSmsMessageModel : SmsMessageModel<LocalSmsMessageEntity>
    {
        public LocalSmsMessageModel(LocalSmsMessageEntity backingEntity)
            : base(backingEntity)
        {
        }
    }
}