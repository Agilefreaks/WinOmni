namespace Omnipaste.Presenters
{
    using Omnipaste.Models;

    public class LocalPhoneCallPresenter : PhoneCallPresenter<LocalPhoneCall>
    {
        public LocalPhoneCallPresenter(LocalPhoneCall phoneCall)
            : base(phoneCall)
        {
        }
    }
}