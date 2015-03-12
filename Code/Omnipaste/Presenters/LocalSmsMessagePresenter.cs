namespace Omnipaste.Presenters
{
    using Omnipaste.Models;

    public class LocalSmsMessagePresenter : SmsMessagePresenter<LocalSmsMessage>
    {
        public LocalSmsMessagePresenter(LocalSmsMessage backingModel)
            : base(backingModel)
        {
        }
    }
}