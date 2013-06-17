namespace Omnipaste.Services
{
    public interface IActivationDataProvider
    {
        ActivationData GetActivationData(string token);
    }
}