namespace OmniApi.Resources
{
    using OmniApi.Models;
    using OmniCommon.Interfaces;

    public interface IResource
    {
        IConfigurationService ConfigurationService { get; set; }

        Token Token { get; }
    }
}