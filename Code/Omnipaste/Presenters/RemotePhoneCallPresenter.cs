namespace Omnipaste.Presenters
{
    using Omnipaste.Models;

    public class RemotePhoneCallPresenter : PhoneCallPresenter<RemotePhoneCallEntity>
    {
        /*
         * Use the Build method on PhoneCallPresenter
         */
        public RemotePhoneCallPresenter(RemotePhoneCallEntity phoneCallEntity)
            : base(phoneCallEntity)
        {
        }
    }
}