namespace Omnipaste.SMSComposer
{
    using OmniApi.Models;
    using Omnipaste.Models;
    using SMS.Resources.v1;
    
    public class InlineSMSComposerViewModel : SMSComposerViewModel, IInlineSMSComposerViewModel
    {
        #region Constructors and Destructors

        public InlineSMSComposerViewModel(ISMSMessages smsMessages, ISMSMessageFactory smsMessageFactory)
            : base(smsMessages, smsMessageFactory)
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