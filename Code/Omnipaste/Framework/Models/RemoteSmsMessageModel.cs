namespace Omnipaste.Framework.Models
{
    using Omnipaste.Framework.Entities;

    public class RemoteSmsMessageModel :  SmsMessageModel<RemoteSmsMessageEntity>
    {
        public RemoteSmsMessageModel(RemoteSmsMessageEntity backingEntity)
            : base(backingEntity)
        {
        }
    }
}