namespace Omnipaste.Presenters
{
    using Omnipaste.Models;

    public class RemoteSmsMessagePresenter :  SmsMessagePresenter<RemoteSmsMessageEntity>
    {
        public RemoteSmsMessagePresenter(RemoteSmsMessageEntity backingModel)
            : base(backingModel)
        {
        }
    }
}