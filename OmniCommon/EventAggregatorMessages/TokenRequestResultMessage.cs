namespace OmniCommon.EventAggregatorMessages
{
    public class TokenRequestResultMessage
    {
        public TokenRequestResultMessageStatusEnum Status { get; set; }

        public string ActivationCode { get; set; }

        public TokenRequestResultMessage(TokenRequestResultMessageStatusEnum status, string activationCode = null)
        {
            Status = status;
            ActivationCode = activationCode;
        }
    }
}