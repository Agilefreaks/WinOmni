namespace Omnipaste.SMSComposer
{
    using Omnipaste.Models;
    using OmniUI.Models;

    public interface IInlineSMSComposerViewModel : ISMSComposerViewModel
    {
        #region Public Properties

        ContactInfo ContactInfo { get; set; }

        #endregion
    }
}