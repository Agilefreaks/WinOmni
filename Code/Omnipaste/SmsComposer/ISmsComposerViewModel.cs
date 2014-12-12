namespace Omnipaste.SMSComposer
{
    using Caliburn.Micro;
    using Omnipaste.Models;

    public interface ISMSComposerViewModel : IScreen
    {
        #region Public Properties

        bool CanSend { get; }

        SMSMessage Model { get; set; }

        #endregion

        #region Public Methods and Operators

        void Send();

        #endregion
    }
}