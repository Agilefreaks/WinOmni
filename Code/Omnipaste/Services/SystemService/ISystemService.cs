namespace Omnipaste.Services.SystemService
{
    using System;
    using Ninject;

    public interface ISystemService : IStartable
    {
        event EventHandler<EventArgs> Resumed;

        event EventHandler<EventArgs> Suspended;
    }
}