namespace Omnipaste.Presenters
{
    using Omnipaste.Entities;
    using Omnipaste.Models;

    public class LocalPhoneCallPresenter : PhoneCallPresenter<LocalPhoneCallEntity>
    {
        public LocalPhoneCallPresenter(LocalPhoneCallEntity phoneCallEntity)
            : base(phoneCallEntity)
        {
        }
    }
}