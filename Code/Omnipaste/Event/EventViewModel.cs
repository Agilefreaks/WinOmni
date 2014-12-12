﻿namespace Omnipaste.Event
{
    using System;
    using Caliburn.Micro;
    using Events.Models;
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
    using Omnipaste.Services;

    public class EventViewModel : DetailsViewModelBase<Event>, IEventViewModel
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
        public ICallStore CallStore { get; set; }

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
                return string.IsNullOrWhiteSpace(Model.ContactName) ? Model.PhoneNumber : Model.ContactName;
            }
        }

        public EventTypeEnum Type
        {
            get
            {
                return Model.Type;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void CallBack()
        {
            Devices.Call(Model.PhoneNumber)
                .RunToCompletion(OnCallStarted, dispatcher: DispatcherProvider.Current);
        }

        private void OnCallStarted(EmptyModel model)
        {
            ShowCallingNotification();
            CallStore.AddCall(new Call { ContactInfo = new ContactInfo { Phone = Model.PhoneNumber } });
        }

        public void SendSms()
        {
            EventAggregator.PublishOnCurrentThread(new SendSmsMessage { Recipient = Model.PhoneNumber });
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