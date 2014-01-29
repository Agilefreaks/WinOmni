namespace Omni
{
    public class OmniServiceStatusChanged
    {
        public OmniServiceStatusChanged()
        {
        }

        public OmniServiceStatusChanged(OmniServiceStatusEnum status)
        {
            this.Status = status;
        }

        public OmniServiceStatusEnum Status { get; set; }
    }
}