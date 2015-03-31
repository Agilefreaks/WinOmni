namespace Omnipaste.Models
{
    public class RemoteSmsMessageModel :  SmsMessageModel<RemoteSmsMessageEntity>
    {
        public RemoteSmsMessageModel(RemoteSmsMessageEntity backingEntity)
            : base(backingEntity)
        {
        }
    }
}