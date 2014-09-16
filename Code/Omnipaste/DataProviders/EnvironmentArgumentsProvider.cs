namespace Omnipaste.DataProviders
{
    using System;
    using System.Collections.Generic;

    public class EnvironmentArgumentsProvider : IArgumentsProvider
    {
        public IEnumerable<string> GetCommandLineArgs()
        {
            return Environment.GetCommandLineArgs();
        }
    }
}