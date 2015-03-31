namespace Omnipaste.Framework.DataProviders
{
    using System.Collections.Generic;

    public interface IArgumentsProvider
    {
        IEnumerable<string> GetCommandLineArgs();
    }
}