namespace Omnipaste.SMSComposer
{
    using Omnipaste.Models;

    public interface IInlineSMSComposerViewModel : ISMSComposerViewModel
    {
        #region Public Properties

        ContactInfo ContactInfo { get; set; }

        #endregion
    }
}