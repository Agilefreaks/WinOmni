namespace OmniCommon.EventAggregatorMessages
{
    public class TokenRequestResutMessage
    {
        public TokenRequestResultMessageStatusEnum Status { get; set; }

        public string Token { get; set; }
    }
}