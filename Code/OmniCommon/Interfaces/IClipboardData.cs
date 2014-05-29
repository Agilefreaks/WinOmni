namespace OmniCommon.Interfaces
{
    public interface IClipboardData
    {
        object GetSender();

        string GetData();
    }
}