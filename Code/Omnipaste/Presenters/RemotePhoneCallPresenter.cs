namespace Omnipaste.Presenters
{
    using Omnipaste.Models;

    public class RemotePhoneCallPresenter : PhoneCallPresenter<RemotePhoneCall>
    {
        /*
         * Use the Build method on PhoneCallPresenter
         */
        public RemotePhoneCallPresenter(RemotePhoneCall phoneCall)
            : base(phoneCall)
        {
        }
    }
}