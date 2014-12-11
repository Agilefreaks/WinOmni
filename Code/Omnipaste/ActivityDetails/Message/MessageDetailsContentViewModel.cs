namespace Omnipaste.ActivityDetails.Message
{
    using Ninject;
    using OmniApi.Resources.v1;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Activity.Models;

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
        public IDevices Devices { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

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
            Devices.SendSms(_contactInfo.Phone, ReplyContent)
                .RunToCompletion(model => { CanReply = true; }, error => { CanReply = true; });
        }

        #endregion
    }
}