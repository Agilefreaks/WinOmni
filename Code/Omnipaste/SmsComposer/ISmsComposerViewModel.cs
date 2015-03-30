namespace Omnipaste.SMSComposer
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Omnipaste.Presenters;

    public interface ISMSComposerViewModel : IScreen
    {
        string Message { get; set; }

        ObservableCollection<ContactInfoPresenter> Recipients { get; set; }

        void Send();
    }
}