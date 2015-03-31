namespace Omnipaste.Framework.EventAggregatorMessages
{
    public class TokenRequestResultMessage
    {
        #region Constructors and Destructors

        public TokenRequestResultMessage(TokenRequestResultMessageStatusEnum status, string activationCode = null)
        {
            Status = status;
            ActivationCode = activationCode;
        }

        #endregion

        #region Public Properties

        public string ActivationCode { get; set; }

        public TokenRequestResultMessageStatusEnum Status { get; set; }

        #endregion
    }
}