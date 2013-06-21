namespace OmniCommon.DataProviders
{
    public interface IActivationDataProvider
    {
        ActivationData GetActivationData(string token);
    }
}