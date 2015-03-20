namespace Omnipaste.Presenters
{
    using Omnipaste.Models;

    public class RemoteSmsMessagePresenter :  SmsMessagePresenter<RemoteSmsMessage>
    {
        public RemoteSmsMessagePresenter(RemoteSmsMessage backingModel)
            : base(backingModel)
        {
        }
    }
}