namespace Omnipaste.SendSms
{
    using Caliburn.Micro;

    public interface ISendSmsViewModel : IScreen
    {
        #region Public Properties

        string Message { get; set; }

        string Recipient { get; set; }

        #endregion

        #region Public Methods and Operators

        void Send();

        #endregion
    }
}