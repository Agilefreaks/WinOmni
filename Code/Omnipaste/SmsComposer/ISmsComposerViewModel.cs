namespace Omnipaste.SMSComposer
{
    using System.Collections.Generic;
    using Caliburn.Micro;
    using Omnipaste.Models;
    using Omnipaste.Presenters;

    public interface ISMSComposerViewModel : IScreen
    {
        string Message { get; set; }

        IList<ContactInfoPresenter> Recipients { get; set; }

        void Send();
    }
}