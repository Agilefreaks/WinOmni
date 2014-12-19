namespace OmniHolidays.Providers
{
    using System;
    using System.Collections.Generic;

    public interface IMessageDefinitionProvider
    {
        IObservable<IList<MessageDefinition>> Get();
    }
}