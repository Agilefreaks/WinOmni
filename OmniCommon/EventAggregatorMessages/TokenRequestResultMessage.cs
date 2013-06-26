namespace OmniCommon.EventAggregatorMessages
{
    public class TokenRequestResultMessage
    {
        public TokenRequestResultMessageStatusEnum Status { get; set; }

        public string Token { get; set; }

        public TokenRequestResultMessage(TokenRequestResultMessageStatusEnum status, string token = null)
        {
            Status = status;
            Token = token;
        }
    }
}