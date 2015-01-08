namespace Omnipaste.Event
{
    using System;
    using Caliburn.Micro;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Dialog;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.MasterEventList.Calling;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using OmniUI.Details;
    using OmniUI.Models;

    public class EventViewModel : DetailsViewModelBase<IConversationItem>, IEventViewModel
    {
        #region Constructors and Destructors

        public EventViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        #endregion

        #region Public Properties

        [Inject]
        public ICallingViewModel CallingViewModel { get; set; }

        public string Content
        {
            get
            {
                return Model.Content;
            }
        }

        [Inject]
        public IDevices Devices { get; set; }

        [Inject]
        public IDialogViewModel DialogViewModel { get; set; }

        [Inject]
        public ICallRepository CallRepository { get; set; }

        public IEventAggregator EventAggregator { get; set; }

        public DateTime Time
        {
            get
            {
                return Model.Time;
            }
        }

        public string Title
        {
            get
            {
                return string.IsNullOrWhiteSpace(Model.ContactInfo.Name) ? Model.ContactInfo.Phone : Model.ContactInfo.Name;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void CallBack()
        {
            Devices.Call(Model.ContactInfo.Phone)
                .RunToCompletion(OnCallStarted, dispatcher: DispatcherProvider.Current);
        }

        private void OnCallStarted(EmptyModel model)
        {
            ShowCallingNotification();
            CallRepository.Save(new Call { ContactInfo = new ContactInfo { Phone = Model.ContactInfo.Phone } });
        }

        public void SendSms()
        {
            EventAggregator.PublishOnCurrentThread(new SendSmsMessage { Recipient = Model.ContactInfo.Phone });
        }

        #endregion

        #region Methods

        private void ShowCallingNotification()
        {
            DialogViewModel.ActivateItem(CallingViewModel);
        }

        #endregion
    }
}