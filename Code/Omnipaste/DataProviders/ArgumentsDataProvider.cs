namespace Omnipaste.DataProviders
{
    using NDesk.Options;
    using OmniCommon.DataProviders;

    public class ArgumentsDataProvider : IArgumentsDataProvider
    {
        public string AuthorizationKey { get; private set; }

        public bool Minimized { get; private set; }

        public bool EnableLog { get; private set; }

        public ArgumentsDataProvider(IArgumentsProvider argumentsProvider = null)
        {
            argumentsProvider = argumentsProvider ?? new EnvironmentArgumentsProvider();
            var optionSet = new OptionSet
                                {
                                    { "authorizationKey:", v => { AuthorizationKey = v; } },
                                    { "minimized", v => { Minimized = true; } },
                                    { "log", v => { EnableLog = true; } }
                                };
            optionSet.Parse(argumentsProvider.GetCommandLineArgs());
        }
    }
}