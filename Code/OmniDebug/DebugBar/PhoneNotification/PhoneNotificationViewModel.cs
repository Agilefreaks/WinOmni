namespace OmniDebug.DebugBar.PhoneNotification
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using OmniCommon.Models;
    using OmniDebug.Services;
    using PhoneCalls.Dto;

    public class PhoneNotificationViewModel : PropertyChangedBase, IDebugBarPanel
    {
        private string _notificationContactName;

        private string _notificationPhoneNumber;

        private readonly IOmniServiceWrapper _omniServiceWrapper;

        private readonly IPhoneCallsWrapper _phoneCallsWrapper;

        public PhoneNotificationViewModel(IOmniServiceWrapper omniServiceWrapper, IPhoneCallsWrapper phoneCallsWrapper)
        {
            _omniServiceWrapper = omniServiceWrapper;
            _phoneCallsWrapper = phoneCallsWrapper;

            NotificationContactName = "Some Contact";
            NotificationPhoneNumber = "0788999666";
        }

        public string NotificationContactName
        {
            get
            {
                return _notificationContactName;
            }
            set
            {
                if (value == _notificationContactName)
                {
                    return;
                }
                _notificationContactName = value;
                NotifyOfPropertyChange();
            }
        }

        public string NotificationPhoneNumber
        {
            get
            {
                return _notificationPhoneNumber;
            }
            set
            {
                if (value == _notificationPhoneNumber)
                {
                    return;
                }
                _notificationPhoneNumber = value;
                NotifyOfPropertyChange();
            }
        }

        public void SimulateCall()
        {
            var id = Guid.NewGuid().ToString();
            _phoneCallsWrapper.MockGet(
                id,
                new PhoneCallDto
                    {
                        Id = id,
                        Number = NotificationPhoneNumber,
                        State = PhoneCallState.Starting,
                        Type = PhoneCallType.Incoming
                    });
            _omniServiceWrapper.SimulateMessage(
                new OmniMessage
                    {
                        Type = "phone_call_received",
                        Payload = new Dictionary<string, string> { { "id", id } }
                    });
        }
    }
}