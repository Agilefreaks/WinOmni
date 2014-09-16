namespace Omnipaste.DataProviders
{
    using NDesk.Options;

    public class ArgumentsDataProvider : IArgumentsDataProvider
    {
        public string AuthorizationKey { get; private set; }

        public ArgumentsDataProvider(IArgumentsProvider argumentsProvider = null)
        {
            argumentsProvider = argumentsProvider ?? new EnvironmentArgumentsProvider();
            var optionSet = new OptionSet { { "authorizationKey:", v => { AuthorizationKey = v; } } };
            optionSet.Parse(argumentsProvider.GetCommandLineArgs());
        }
    }
}