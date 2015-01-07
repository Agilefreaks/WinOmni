namespace OmniCommon.DataProviders
{
    public interface IArgumentsDataProvider
    {
        string AuthorizationKey { get; }

        bool Minimized { get; }

        bool EnableLog { get; }

        bool Updated { get; }
    }
}