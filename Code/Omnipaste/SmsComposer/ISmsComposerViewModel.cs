namespace Omnipaste.SmsComposer
{
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;

    public interface ISmsComposerViewModel : IScreen, IHandle<SendSmsMessage>
    {
        #region Public Properties

        SmsMessage Model { get; set; }

        SmsComposerStatusEnum State { get; set; }

        bool CanSend { get; }

        #endregion

        #region Public Methods and Operators

        void Send();

        #endregion
    }
}