namespace SMS.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using OmniApi.Models;

    public interface ISMSMessages
    {
        IObservable<EmptyModel> Send(IEnumerable<string> messages, IEnumerable<string> phoneNumbers);
    }
}