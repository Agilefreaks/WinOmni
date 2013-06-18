namespace OmniCommon.EventAggregatorMessages
{
    using OmniCommon.Services;

    public class OmniServiceStatusChanged
    {
        public OmniServiceStatusChanged()
        {
        }

        public OmniServiceStatusChanged(OmniServiceStatusEnum status)
        {
            Status = status;
        }

        public OmniServiceStatusEnum Status { get; set; }
    }
}