namespace OmniApi.Resources
{
    using global::OmniApi.Models;

    public interface IActivationTokens
    {
        ActivationModel Activate(string token);
    }
}