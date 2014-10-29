namespace OmniApiTests.Resources
{
    using System;
    using Refit;

    public interface ITestApi
    {
        [Get("/somePath")]
        IObservable<string> Get();
    }
}