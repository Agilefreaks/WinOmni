namespace Omnipaste.SMSComposer
{
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using Omnipaste.Models;

    public class InlineSMSComposerViewModel : SMSComposerViewModel, IInlineSMSComposerViewModel
    {
        #region Constructors and Destructors

        public InlineSMSComposerViewModel(IDevices devices, ISMSMessageFactory smsMessageFactory)
            : base(devices, smsMessageFactory)
        {
        }

        #endregion

        #region Public Properties

        public ContactInfo ContactInfo { get; set; }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            Model = SMSMessageFactory.Create(ContactInfo);
            base.OnActivate();
        }

        protected override void OnSentSMS(EmptyModel model)
        {
            base.OnSentSMS(model);
            Model = SMSMessageFactory.Create(ContactInfo);
        }

        #endregion
    }
}