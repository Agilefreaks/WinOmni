namespace Omnipaste.ActivityDetails.Message
{
    using System;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Activity.Models;
    using Omnipaste.Models;
    using Omnipaste.Services;

    public class MessageDetailsContentViewModel : ActivityDetailsContentViewModel, IMessageDetailsContentViewModel
    {
        #region Fields

        private bool _canReply;

        private ContactInfo _contactInfo;

        private string _replyContent;

        #endregion

        #region Constructors and Destructors

        public MessageDetailsContentViewModel()
        {
            CanReply = true;
        }

        #endregion

        #region Public Properties

        public bool CanReply
        {
            get
            {
                return _canReply;
            }
            set
            {
                if (value.Equals(_canReply))
                {
                    return;
                }
                _canReply = value;
                NotifyOfPropertyChange();
            }
        }

        [Inject]
        public IConversationViewModel ConversationViewModel { get; set; }

        [Inject]
        public IDevices Devices { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        [Inject]
        public IMessageStore MessageStore { get; set; }

        public override Activity Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
                _contactInfo = value.ExtraData.ContactInfo;
            }
        }

        public string ReplyContent
        {
            get
            {
                return _replyContent;
            }
            set
            {
                if (value == _replyContent)
                {
                    return;
                }
                _replyContent = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Reply()
        {
            CanReply = false;
            Devices.SendSms(_contactInfo.Phone, ReplyContent).RunToCompletion(OnSentSMS, OnSendSMSError);
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            ConversationViewModel.ContactInfo = Model.ExtraData.ContactInfo;
            ConversationViewModel.Activate();
        }

        private void OnSendSMSError(Exception exception)
        {
            CanReply = true;
        }

        private void OnSentSMS(EmptyModel model)
        {
            CanReply = true;
            MessageStore.AddMessage(new Message { ContactInfo = _contactInfo, Content = ReplyContent });
            ReplyContent = string.Empty;
        }

        #endregion
    }
}