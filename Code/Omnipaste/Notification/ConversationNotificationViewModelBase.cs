namespace Omnipaste.Notification
{
    using System.Reactive.Threading.Tasks;
    using System.Threading.Tasks;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using OmniUI.Services;

    public abstract class ConversationNotificationViewModelBase : ResourceBasedNotificationViewModel<IConversationItem>,
                                                                  IConversationNotificationViewModel
    {
        #region Fields

        private readonly ICommandService _commandService;

        private bool _canReplyWithSms = true;

        #endregion

        #region Constructors and Destructors

        protected ConversationNotificationViewModelBase(ICommandService commandService)
        {
            _commandService = commandService;
        }

        #endregion

        #region Public Properties

        public bool CanReplyWithSMS
        {
            get
            {
                return _canReplyWithSms;
            }
            set
            {
                if (value.Equals(_canReplyWithSms))
                {
                    return;
                }
                _canReplyWithSms = value;
                NotifyOfPropertyChange();
            }
        }

        public override object Identifier
        {
            get
            {
                return Resource.UniqueId;
            }
        }

        public override string Line1
        {
            get
            {
                return string.IsNullOrWhiteSpace(Resource.ContactInfo.Name)
                           ? Resource.ContactInfo.Phone
                           : Resource.ContactInfo.Name;
            }
        }

        public override string Line2
        {
            get
            {
                return Resource.Content;
            }
        }

        #endregion

        #region Public Methods and Operators

        public async Task ReplyWithSMS()
        {
            CanReplyWithSMS = false;

            var command = new ComposeSMSCommand(new ContactInfoPresenter(Resource.ContactInfo));
            await _commandService.Execute(command).ToTask();

            Dismiss();
        }

        #endregion
    }
}