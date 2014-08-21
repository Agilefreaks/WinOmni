namespace Omnipaste.SendSms
{
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;

    public interface ISendSmsViewModel : IScreen, IHandle<SendSmsMessage>
    {
        #region Public Properties

        SmsMessage Model { get; set; }

        SendSmsStatusEnum State { get; set; }

        bool CanSend { get; }

        #endregion

        #region Public Methods and Operators

        void Send();

        #endregion
    }
}