namespace Omnipaste.SendSms
{
    using Caliburn.Micro;

    public interface ISendSmsViewModel : IScreen
    {
        #region Public Properties

        SmsMessage Model { get; set; }

        #endregion

        #region Public Methods and Operators

        void Send();

        #endregion
    }
}