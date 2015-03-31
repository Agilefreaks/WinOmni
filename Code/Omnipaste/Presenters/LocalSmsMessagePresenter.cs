namespace Omnipaste.Presenters
{
    using Omnipaste.Entities;
    using Omnipaste.Models;

    public class LocalSmsMessagePresenter : SmsMessagePresenter<LocalSmsMessageEntity>
    {
        public LocalSmsMessagePresenter(LocalSmsMessageEntity backingModel)
            : base(backingModel)
        {
        }
    }
}