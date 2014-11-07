namespace Omnipaste.SmsComposer
{
    using System;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Properties;

    public class SMSFactory : ISMSFactory
    {
        #region Static Fields

        private static readonly string DefaultMessageSuffix = string.Format(
            "{0}{1}",
            Environment.NewLine,
            Resources.SentFromOmnipaste);

        #endregion

        #region Fields

        private readonly IConfigurationService _configurationService;

        #endregion

        #region Constructors and Destructors

        public SMSFactory(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        #endregion

        #region Public Methods and Operators

        public SmsMessage Create()
        {
            return new SmsMessage { Message = GetMessageSuffix() };
        }

        public SmsMessage Create(SendSmsMessage sendSmsMessage)
        {
            return new SmsMessage
                       {
                           Message = string.Format("{0}{1}", sendSmsMessage.Message, GetMessageSuffix()),
                           Recipient = sendSmsMessage.Recipient
                       };
        }

        #endregion

        #region Methods

        private string GetMessageSuffix()
        {
            return _configurationService.IsSMSSuffixEnabled ? DefaultMessageSuffix : string.Empty;
        }

        #endregion
    }
}